using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Api.Infrastructure;
using WarehouseDashboard.Api.Models;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// Type of sync request in the queue.
/// </summary>
public enum SyncRequestType
{
    /// <summary>Sync all active table mappings.</summary>
    Full,

    /// <summary>Sync only the specified mapping IDs.</summary>
    Selected
}

/// <summary>
/// Represents a single sync request queued for processing.
/// </summary>
public class SyncRequest
{
    /// <summary>Unique identifier for this request.</summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>Whether this is a Full or Selected sync.</summary>
    public SyncRequestType Type { get; init; }

    /// <summary>Mapping IDs for Selected syncs (empty for Full).</summary>
    public List<int> MappingIds { get; init; } = new();

    /// <summary>UTC timestamp when this request was enqueued.</summary>
    public DateTime QueuedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Trigger type passed to the engine (e.g. "Manual", "Auto").</summary>
    public string TriggerType { get; init; } = "Auto";

    /// <summary>Progress run for Selected syncs (null for Full).</summary>
    public SyncRunProgress? Progress { get; init; }

    /// <summary>In-memory log record for this request.</summary>
    public SyncRunRecord? LogRecord { get; init; }
}

/// <summary>
/// Snapshot of the queue status for the <c>GET /api/sync/queue</c> endpoint.
/// </summary>
public class SyncQueueStatus
{
    /// <summary>True while a sync operation is actively executing.</summary>
    public bool IsProcessing { get; set; }

    /// <summary>The request currently being processed (null if idle).</summary>
    public CurrentSyncInfo? CurrentSync { get; set; }

    /// <summary>Pending requests waiting in the queue (position 1 = next to process).</summary>
    public List<QueueEntry> Queue { get; set; } = new();

    /// <summary>Number of pending requests.</summary>
    public int TotalQueued => Queue.Count;
}

/// <summary>Info about the currently executing sync.</summary>
public class CurrentSyncInfo
{
    /// <summary>Type of sync running ("Full" or "Selected").</summary>
    public string Type { get; set; } = "";

    /// <summary>UTC timestamp when processing started.</summary>
    public DateTime StartedAt { get; set; }

    /// <summary>Elapsed time since processing started.</summary>
    public string Elapsed { get; set; } = "";
}

/// <summary>A single entry in the pending queue.</summary>
public class QueueEntry
{
    /// <summary>1-based position in the queue.</summary>
    public int Position { get; set; }

    /// <summary>Type of sync request.</summary>
    public string Type { get; set; } = "";

    /// <summary>Mapping IDs (only for Selected requests).</summary>
    public List<int>? MappingIds { get; set; }

    /// <summary>UTC timestamp when this request was enqueued.</summary>
    public DateTime QueuedAt { get; set; }
}

/// <summary>
/// Centralized sync queue that ensures only one sync operation runs at a time and no
/// request is ever skipped. Replaces the previous SemaphoreSlim skip-on-conflict pattern.
///
/// <para>
/// Queue processing rules:
/// 1. One at a time — only one sync operation executes at any given moment.
/// 2. Full absorbs Selected — a Full request removes all pending Selected requests from the queue.
/// 3. Selected merges — multiple Selected requests merge their IDs into one batch.
/// 4. No skipping — every request waits in queue until executed.
/// 4. Deduplication — if Full is already queued, new Selected requests are absorbed.
/// </para>
///
/// <para>
/// This service also hosts the auto-sync timer (previously in SyncEngineService). The timer
/// reads the <c>SyncSettings</c> table via ADO.NET and enqueues Full requests on the configured
/// interval. This avoids a circular dependency between the queue and the engine.
/// </para>
/// </summary>
public class SyncQueueService : BackgroundService
{
    private readonly SyncEngineService _engine;
    private readonly SyncRunLogStore _logStore;
    private readonly SyncRunProgressStore _progressStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncQueueService> _logger;

    // ---- Queue state (all mutations under _lock) ----
    private readonly object _lock = new();
    private readonly List<SyncRequest> _pending = new();

    // ---- Current processing state ----
    private SyncRequest? _currentRequest;
    private DateTime? _currentStartedAt;

    // ---- Signal for the processing loop ----
    private readonly SemaphoreSlim _itemAvailable = new(0);

    // ---- Safe defaults (mirrors SyncEngineService) ----
    private static readonly TimeSpan ConfigPollInterval = TimeSpan.FromMinutes(5);
    private bool _loggedConfigFallback;

    /// <summary>True while a sync is actively executing (set during processing).</summary>
    public bool IsProcessing => _currentRequest is not null;

    public SyncQueueService(
        SyncEngineService engine,
        SyncRunLogStore logStore,
        SyncRunProgressStore progressStore,
        IConfiguration configuration,
        ILogger<SyncQueueService> logger)
    {
        _engine = engine;
        _logStore = logStore;
        _progressStore = progressStore;
        _configuration = configuration;
        _logger = logger;
    }

    // ---- Public API: Enqueue --------------------------------------------------------

    /// <summary>
    /// Enqueues a Full sync request. Any pending Selected requests in the queue are absorbed
    /// (a Full sync covers all tables, so individual Selected requests become redundant).
    /// Returns the 1-based position in the queue.
    /// </summary>
    public int EnqueueFull(string triggerType = "Manual")
    {
        lock (_lock)
        {
            // If a Full request is already pending, don't add a duplicate.
            var existingFullIndex = _pending.FindIndex(r => r.Type == SyncRequestType.Full);
            if (existingFullIndex >= 0)
            {
                var pos = existingFullIndex + 1;
                _logger.LogInformation(
                    "EnqueueFull: Full sync already pending at position {Position}. Absorbing new request.", pos);
                return pos;
            }

            // Absorb all pending Selected requests — Full covers them.
            var absorbed = _pending.RemoveAll(r => r.Type == SyncRequestType.Selected);
            if (absorbed > 0)
            {
                _logger.LogInformation(
                    "EnqueueFull: Absorbed {Count} pending Selected request(s).", absorbed);
            }

            var request = new SyncRequest
            {
                Type = SyncRequestType.Full,
                TriggerType = triggerType
            };
            _pending.Add(request);
            var position = _pending.Count;

            _logger.LogInformation(
                "EnqueueFull: Full sync queued at position {Position}.", position);

            _itemAvailable.Release();
            return position;
        }
    }

    /// <summary>
    /// Enqueues a Selected sync request for the specified mapping IDs. If a Full request is
    /// already pending, this request is absorbed. If a Selected request with overlapping IDs
    /// is already pending, the IDs are merged.
    /// Returns the 1-based position in the queue.
    /// </summary>
    public int EnqueueSelected(
        List<int> mappingIds,
        SyncRunProgress? progress = null,
        SyncRunRecord? logRecord = null,
        string triggerType = "Manual (selected)")
    {
        if (mappingIds is null || mappingIds.Count == 0)
        {
            _logger.LogWarning("EnqueueSelected called with empty mappingIds. Ignoring.");
            return 0;
        }

        lock (_lock)
        {
            // If a Full request is already pending, this Selected is absorbed.
            if (_pending.Any(r => r.Type == SyncRequestType.Full))
            {
                _logger.LogInformation(
                    "EnqueueSelected: Full sync already pending — Selected request for {Count} ID(s) absorbed.",
                    mappingIds.Count);
                // Return the Full's position so the caller knows.
                var fullPos = _pending.FindIndex(r => r.Type == SyncRequestType.Full) + 1;
                return fullPos;
            }

            // Try to merge with an existing Selected request.
            var existingSelectedIndex = _pending.FindIndex(r => r.Type == SyncRequestType.Selected);
            if (existingSelectedIndex >= 0)
            {
                var existing = _pending[existingSelectedIndex];
                var mergedIds = existing.MappingIds.Union(mappingIds).ToList();

                // Create a new merged request (replacing the old one) to carry the combined IDs.
                // Use the newer progress/log records if provided.
                _pending[existingSelectedIndex] = new SyncRequest
                {
                    Type = SyncRequestType.Selected,
                    MappingIds = mergedIds,
                    TriggerType = triggerType,
                    Progress = progress ?? existing.Progress,
                    LogRecord = logRecord ?? existing.LogRecord
                };

                var position = existingSelectedIndex + 1;
                _logger.LogInformation(
                    "EnqueueSelected: Merged {NewCount} ID(s) into existing Selected request at position {Position}. " +
                    "Total IDs now: {TotalCount}.",
                    mappingIds.Count, position, mergedIds.Count);

                _itemAvailable.Release();
                return position;
            }

            // No existing Selected or Full — add a new request.
            var request = new SyncRequest
            {
                Type = SyncRequestType.Selected,
                MappingIds = new List<int>(mappingIds),
                TriggerType = triggerType,
                Progress = progress,
                LogRecord = logRecord
            };
            _pending.Add(request);
            var pos = _pending.Count;

            _logger.LogInformation(
                "EnqueueSelected: Selected sync for {Count} ID(s) queued at position {Position}.",
                mappingIds.Count, pos);

            _itemAvailable.Release();
            return pos;
        }
    }

    // ---- Public API: Status ---------------------------------------------------------

    /// <summary>
    /// Returns a snapshot of the current queue status for the <c>GET /api/sync/queue</c> endpoint.
    /// </summary>
    public SyncQueueStatus GetQueueStatus()
    {
        lock (_lock)
        {
            var status = new SyncQueueStatus
            {
                IsProcessing = _currentRequest is not null,
                CurrentSync = _currentRequest is not null && _currentStartedAt.HasValue
                    ? new CurrentSyncInfo
                    {
                        Type = _currentRequest.Type == SyncRequestType.Full ? "Full" : "Selected",
                        StartedAt = _currentStartedAt.Value,
                        Elapsed = (DateTime.UtcNow - _currentStartedAt.Value).ToString(@"hh\:mm\:ss")
                    }
                    : null,
                Queue = _pending.Select((r, i) => new QueueEntry
                {
                    Position = i + 1,
                    Type = r.Type == SyncRequestType.Full ? "Full" : "Selected",
                    MappingIds = r.Type == SyncRequestType.Selected ? r.MappingIds : null,
                    QueuedAt = r.QueuedAt
                }).ToList()
            };

            return status;
        }
    }

    // ---- BackgroundService ----------------------------------------------------------

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "SyncQueueService started. Auto-sync timer and queue processor are active.");

        // Run the auto-sync timer in the background (fire-and-forget; it loops internally).
        _ = RunAutoSyncTimerAsync(stoppingToken);

        // Main queue processing loop — waits for items and processes them one at a time.
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait until an item is available.
                await _itemAvailable.WaitAsync(stoppingToken);

                // Process all available items sequentially.
                while (true)
                {
                    SyncRequest? request;
                    lock (_lock)
                    {
                        if (_pending.Count == 0)
                            break;

                        request = _pending[0];
                        _pending.RemoveAt(0);
                    }

                    await ProcessRequestAsync(request, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Host is shutting down — exit gracefully.
        }

        _logger.LogInformation("SyncQueueService is stopping.");
    }

    // ---- Processing ----------------------------------------------------------------

    /// <summary>
    /// Processes a single sync request by calling the appropriate engine method.
    /// Handles log records, progress tracking, and error recovery.
    /// </summary>
    private async Task ProcessRequestAsync(SyncRequest request, CancellationToken ct)
    {
        _currentRequest = request;
        _currentStartedAt = DateTime.UtcNow;

        var logRecord = request.LogRecord ?? _logStore.BeginRun(request.TriggerType);

        try
        {
            if (request.Type == SyncRequestType.Full)
            {
                _logger.LogInformation("Processing Full sync request {RequestId}.", request.Id);
                await _engine.RunSyncOnceAsync(ct, request.TriggerType);
                _logStore.CompleteRun(logRecord, _engine.LastStatus, _engine.LastRecordCount, null);
            }
            else
            {
                _logger.LogInformation(
                    "Processing Selected sync request {RequestId} for {Count} mapping(s).",
                    request.Id, request.MappingIds.Count);

                var result = await _engine.RunSelectedMappingsAsync(
                    request.MappingIds, ct, request.Progress);

                // Complete the progress run.
                if (request.Progress is not null)
                {
                    var progressStatus = result.Status is "success" or "partial"
                        ? result.Status
                        : "failed";
                    _progressStore.CompleteRun(request.Progress.RunId, progressStatus);
                }

                // Collect error messages from failed mappings.
                var failedErrors = result.Mappings
                    .Where(m => m.Status == "failed" && !string.IsNullOrEmpty(m.Error))
                    .Select(m => $"{m.TargetTable}: {m.Error}");
                var combinedError = failedErrors.Any() ? string.Join("; ", failedErrors) : null;

                var logStatus = result.Status switch
                {
                    "success" => "Success",
                    "partial" => "Partial",
                    _ => "Failed"
                };
                _logStore.CompleteRun(logRecord, logStatus, result.TotalRows, combinedError);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            _logger.LogInformation("Sync request {RequestId} cancelled (shutdown).", request.Id);
            _logStore.CompleteRun(logRecord, "Cancelled", 0, "Sync was cancelled (host shutting down).");

            if (request.Progress is not null)
                _progressStore.CompleteRun(request.Progress.RunId, "failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync request {RequestId} failed unexpectedly.", request.Id);
            _logStore.CompleteRun(logRecord, "Failed", 0, ex.Message);

            if (request.Progress is not null)
                _progressStore.CompleteRun(request.Progress.RunId, "failed");
        }
        finally
        {
            _currentRequest = null;
            _currentStartedAt = null;
        }
    }

    // ---- Auto-sync timer (moved from SyncEngineService) ----------------------------

    /// <summary>
    /// Drives the automatic sync schedule. Reads <c>SyncSettings</c> from SQL Server via
    /// ADO.NET. When auto-sync is enabled, enqueues Full requests on the configured interval.
    /// When disabled (or DB unreachable), polls every 5 minutes to detect configuration changes.
    /// </summary>
    private async Task RunAutoSyncTimerAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            SyncEngineSettings settings;
            try
            {
                settings = await LoadSyncSettingsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (!settings.IsAutoSyncEnabled)
            {
                _logger.LogInformation(
                    "Auto-sync is DISABLED. Re-checking SyncSettings in {Poll}.", ConfigPollInterval);

                try
                {
                    await Task.Delay(ConfigPollInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                continue;
            }

            // Auto-sync enabled: drive the cycle with a PeriodicTimer.
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(settings.IntervalMinutes));
            _logger.LogInformation(
                "Auto-sync ENABLED. Interval={Interval} min. Next cycle scheduled.",
                settings.IntervalMinutes);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    _logger.LogInformation("Auto-sync timer tick — enqueueing Full sync.");
                    EnqueueFull("Auto");

                    // Reload config after each cycle to pick up runtime changes.
                    var reloaded = await LoadSyncSettingsAsync(stoppingToken);
                    if (!reloaded.IsAutoSyncEnabled || reloaded.IntervalMinutes != settings.IntervalMinutes)
                    {
                        _logger.LogInformation(
                            "SyncSettings changed (Enabled={Enabled}, Interval={Interval}). " +
                            "Reconfiguring schedule.",
                            reloaded.IsAutoSyncEnabled, reloaded.IntervalMinutes);
                        break; // exit inner loop; outer loop recreates the timer (or pauses)
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Auto-sync timer stopped.");
    }

    /// <summary>
    /// Loads the scheduling configuration from the <c>SyncSettings</c> table (row Id = 1) via
    /// ADO.NET. If the row is missing or the DB is unreachable, safe defaults are returned
    /// (interval 30 min, auto-sync disabled).
    /// </summary>
    private async Task<SyncEngineSettings> LoadSyncSettingsAsync(CancellationToken ct)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (!_loggedConfigFallback)
            {
                _logger.LogWarning(
                    "SQL Server connection string is empty. Using safe defaults: " +
                    "Interval=30min, AutoSync=disabled.");
                _loggedConfigFallback = true;
            }

            return DefaultSettings;
        }

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(ct);

            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT IntervalMinutes, IsAutoSyncEnabled, LastSyncTimestamp " +
                "FROM SyncSettings WHERE Id = 1";

            await using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                var interval = reader.GetInt32(reader.GetOrdinal("IntervalMinutes"));
                var enabled = reader.GetBoolean(reader.GetOrdinal("IsAutoSyncEnabled"));
                var lastSyncTimestamp = reader.IsDBNull(reader.GetOrdinal("LastSyncTimestamp"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("LastSyncTimestamp"));

                if (interval < 1)
                {
                    _logger.LogWarning(
                        "SyncSettings.IntervalMinutes={Value} below minimum; clamping to 1.", interval);
                    interval = 1;
                }

                _loggedConfigFallback = false;
                return new SyncEngineSettings(interval, enabled, lastSyncTimestamp);
            }

            if (!_loggedConfigFallback)
            {
                _logger.LogWarning(
                    "SyncSettings row (Id=1) not found. Using safe defaults: " +
                    "Interval=30min, AutoSync=disabled.");
                _loggedConfigFallback = true;
            }

            return DefaultSettings;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (!_loggedConfigFallback)
            {
                _logger.LogWarning(ex,
                    "Failed to read SyncSettings (DB unreachable). Using safe defaults.");
                _loggedConfigFallback = true;
            }

            return DefaultSettings;
        }
    }

    private static readonly SyncEngineSettings DefaultSettings = new(
        IntervalMinutes: 30,
        IsAutoSyncEnabled: false,
        LastSyncTimestamp: null);
}

/// <summary>
/// Resolved scheduling configuration for the sync engine. Immutable.
/// </summary>
internal sealed record SyncEngineSettings(
    int IntervalMinutes,
    bool IsAutoSyncEnabled,
    DateTime? LastSyncTimestamp);
