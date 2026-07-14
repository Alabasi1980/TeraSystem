using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Api.Infrastructure;
using WarehouseDashboard.Api.Models;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// Sync Engine background service (the orchestration half of the Sync pipeline).
///
/// On a <see cref="PeriodicTimer"/> driven by the <c>SyncSettings</c> table it runs a
/// full-refresh cycle: for each configured <see cref="TableMapping"/> it extracts the Oracle
/// source via <see cref="OracleExtractionService"/> and loads it into the SQL Server target via
/// <see cref="SqlServerLoadService"/>. A <see cref="SemaphoreSlim"/> prevents overlapping cycles
/// — this covers BOTH the automatic schedule and manual triggers (a manual trigger arriving
/// while a cycle runs is skipped). Per-table failures are isolated (logged, retried up to
/// <see cref="MaxRetries"/>, then skipped) so one bad table never blocks the others.
///
/// Scheduling is CONFIG-DRIVEN. <c>SyncSettings</c> (singleton row, Id = 1) in SQL Server
/// supplies <c>IntervalMinutes</c> and <c>IsAutoSyncEnabled</c>. The Api project intentionally
/// does NOT share the Web DbContext, so the row is read with a direct ADO.NET
/// <see cref="SqlConnection"/>. If the row is missing (DB not seeded) or the DB is unreachable,
/// SAFE DEFAULTS are used: <c>IntervalMinutes = 30</c>, <c>IsAutoSyncEnabled = false</c>. The
/// engine therefore NEVER auto-syncs until explicitly configured.
///
/// Runtime status (<see cref="IsRunning"/>, <see cref="LastSyncTime"/>, <see cref="LastStatus"/>,
/// <see cref="LastRecordCount"/>) is exposed thread-safely for the upcoming sync-status API
/// endpoint (see 14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md §5).
/// </summary>
public class SyncEngineService : BackgroundService
{
    private readonly OracleExtractionService _oracle;
    private readonly SqlServerLoadService _load;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncEngineService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private const int MaxRetries = 3;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    // While auto-sync is disabled (or the DB is unreachable), the engine still polls the
    // SyncSettings table on this cadence to detect when configuration becomes available.
    private static readonly TimeSpan ConfigPollInterval = TimeSpan.FromMinutes(5);

    // Safe fallback used when the SyncSettings row does not exist or the DB is unreachable.
    // By design the engine stays OFF until a real configuration row is present.
    private static readonly SyncEngineSettings DefaultSettings = new(
        IntervalMinutes: 30,
        IsAutoSyncEnabled: false,
        LastSyncTimestamp: null);

    // ---- Runtime status (thread-safe; read by the upcoming status API) -----------------
    private readonly object _statusLock = new();
    private bool _isRunning;
    private DateTime? _lastSyncTime;
    private string _lastStatus = "Never";
    private int _lastRecordCount;

    /// <summary>True while a sync cycle (auto or manual) is executing.</summary>
    public bool IsRunning => LockRead(() => _isRunning);

    /// <summary>UTC timestamp of the last successfully completed cycle (null = never).</summary>
    public DateTime? LastSyncTime => LockRead(() => _lastSyncTime);

    /// <summary>
    /// Status of the most recent cycle: <c>"Never"</c> (no run yet), <c>"Running"</c>,
    /// <c>"Success"</c>, or <c>"Failed"</c>.
    /// </summary>
    public string LastStatus => LockRead(() => _lastStatus);

    /// <summary>Total rows loaded across all mappings in the most recent cycle.</summary>
    public int LastRecordCount => LockRead(() => _lastRecordCount);

    // Throttles the "using defaults" warning so it is logged once per degradation, not every poll.
    private bool _loggedConfigFallback;

    // Actual source -> target mappings are configured later by the client/admin, so this
    // starts EMPTY. Example (uncomment and adjust once the Oracle schemas are known):
    //
    // _mappings.Add(new TableMapping
    // {
    //     OracleSource    = "WMS.WAREHOUSE_STOCK",  // Oracle table or view name
    //     SourceType      = "Table",                // "Table" | "View" | "Query"
    //     SqlTargetTable  = "stg_WarehouseStock"    // SQL Server staging table
    // });
    private readonly List<TableMapping> _mappings = new();

    public SyncEngineService(
        OracleExtractionService oracle,
        SqlServerLoadService load,
        IConfiguration configuration,
        ILogger<SyncEngineService> logger)
    {
        _oracle = oracle;
        _load = load;
        _configuration = configuration;
        _logger = logger;

        // Load table mappings from configuration (appsettings.json "TableMappings" section).
        var mappings = _configuration.GetSection("TableMappings").Get<List<TableMapping>>();
        if (mappings is { Count: > 0 })
        {
            _mappings.AddRange(mappings);
        }

        _logger.LogInformation(
            "SyncEngineService initialized. {Count} mapping(s) loaded from configuration.", _mappings.Count);
    }

    /// <summary>
    /// Runs exactly one full-refresh cycle over all configured mappings. Safe to call
    /// manually (e.g. from the future manual-trigger endpoint). One table failing must not
    /// stop the others. Overlapping invocations are skipped (semaphore), so a manual trigger
    /// arriving during an automatic run simply returns without doing anything.
    /// </summary>
    public async Task RunSyncOnceAsync(CancellationToken ct)
    {
        // Prevent overlap: if a cycle (auto or manual) is already running, skip this invocation.
        if (!await _semaphore.WaitAsync(TimeSpan.Zero, ct))
        {
            _logger.LogWarning("Sync cycle skipped: a previous cycle is still running.");
            return;
        }

        try
        {
            // Mark running and reset the per-run counters; LastStatus = "Running".
            SetStatus(running: true, status: "Running", lastSyncTime: null, recordCount: 0);

            _logger.LogInformation("Sync cycle started. {Count} mapping(s) configured.", _mappings.Count);

            var totalRows = 0;
            foreach (var mapping in _mappings)
            {
                if (mapping is null)
                    continue;

                var target = mapping.SqlTargetTable;
                var succeeded = false;

                for (var attempt = 1; attempt <= MaxRetries; attempt++)
                {
                    try
                    {
                        var oracleSql = BuildOracleQuery(mapping);
                        _logger.LogInformation(
                            "Sync [{Target}] attempt {Attempt}/{Max}: extracting '{Source}'.",
                            target, attempt, MaxRetries, mapping.OracleSource);

                        var data = await _oracle.ExtractAsync(oracleSql, ct);
                        await _load.LoadTableAsync(target, data, ct);

                        _logger.LogInformation(
                            "Sync [{Target}] succeeded: {RowCount} row(s) loaded.", target, data.Rows.Count);
                        totalRows += data.Rows.Count;
                        succeeded = true;
                        break;
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        // Shutdown requested — abort the whole cycle.
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Sync [{Target}] attempt {Attempt}/{Max} failed: {Message}",
                            target, attempt, MaxRetries, ex.Message);

                        if (attempt < MaxRetries)
                        {
                            try
                            {
                                await Task.Delay(RetryDelay, ct);
                            }
                            catch (OperationCanceledException) when (ct.IsCancellationRequested)
                            {
                                throw;
                            }
                        }
                    }
                }

                if (!succeeded)
                {
                    _logger.LogError(
                        "Sync [{Target}] exhausted {Max} attempts and was skipped; continuing with remaining tables.",
                        target, MaxRetries);
                }
            }

            _logger.LogInformation(
                "Sync cycle finished. {TotalRows} row(s) loaded across all mappings.", totalRows);

            // Success: record completion time + totals.
            SetStatus(running: false, status: "Success", lastSyncTime: DateTime.UtcNow, recordCount: totalRows);

            // Persist the sync timestamp so the dashboard "last sync" indicator is current.
            // Wrapped in try/catch: a failure here must not invalidate a successful data load.
            try
            {
                var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    await using var conn = new SqlConnection(connectionString);
                    await conn.OpenAsync(ct);
                    await using var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE SyncSettings SET LastSyncTimestamp = GETUTCDATE() WHERE Id = 1";
                    await cmd.ExecuteNonQueryAsync(ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Sync completed successfully but failed to update LastSyncTimestamp in SyncSettings.");
            }
        }
        catch (OperationCanceledException)
        {
            // Shutdown requested mid-cycle. Reset the running flag but keep the last known
            // status/timestamp so the API reflects the interruption rather than "Running" forever.
            SetRunning(false);
            _logger.LogInformation("Sync cycle aborted (cancellation requested).");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync cycle failed.");
            SetStatus(running: false, status: "Failed", lastSyncTime: null, recordCount: 0);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "SyncEngineService started. Scheduling is config-driven via the SyncSettings table (SQL Server).");

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
                    "Auto-sync is DISABLED (IsAutoSyncEnabled=false). Automatic runs are skipped; a manual " +
                    "trigger remains available. Re-checking SyncSettings in {Poll}.",
                    ConfigPollInterval);

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

            // Auto-sync enabled: drive the cycle with a PeriodicTimer at the configured interval.
            // The timer is recreated each outer-loop iteration so that interval changes (or a
            // disable) are honored after the next cycle / config reload.
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(settings.IntervalMinutes));
            _logger.LogInformation(
                "Auto-sync ENABLED. Interval={Interval}. Next automatic cycle is scheduled.",
                settings.IntervalMinutes);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await RunSyncOnceAsync(stoppingToken);

                    // Reload config after each cycle to pick up runtime changes.
                    var reloaded = await LoadSyncSettingsAsync(stoppingToken);
                    if (!reloaded.IsAutoSyncEnabled || reloaded.IntervalMinutes != settings.IntervalMinutes)
                    {
                        _logger.LogInformation(
                            "SyncSettings changed (IsAutoSyncEnabled={Enabled}, IntervalMinutes={Interval}). " +
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

        _logger.LogInformation("SyncEngineService is stopping.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SyncEngineService received stop signal.");
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Loads the scheduling configuration from the <c>SyncSettings</c> table (row Id = 1) via
    /// ADO.NET. The Api project does NOT have the Web DbContext, so this reads directly with a
    /// <see cref="SqlConnection"/>. If the row is missing (DB not seeded) or the DB is
    /// unreachable, SAFE DEFAULTS are returned (interval 30 min, auto-sync disabled) so the
    /// engine never auto-syncs until explicitly configured.
    /// </summary>
    private async Task<SyncEngineSettings> LoadSyncSettingsAsync(CancellationToken ct)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (!_loggedConfigFallback)
            {
                _logger.LogWarning(
                    "SQL Server connection string is empty (is SQL_PASSWORD set?). Using safe defaults: " +
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
            // Column names are fixed by the data model (06_DATA_MODEL_PREPARATION.md §1.3); the
            // literal Id = 1 is not user input, so there is no SQL-injection surface here.
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

                // Clamp interval to a sane minimum of 1 minute.
                if (interval < 1)
                {
                    _logger.LogWarning(
                        "SyncSettings.IntervalMinutes={Value} is below the minimum; clamping to 1.", interval);
                    interval = 1;
                }

                _loggedConfigFallback = false; // real config present
                return new SyncEngineSettings(interval, enabled, lastSyncTimestamp);
            }

            // Row does not exist yet (DB not seeded) -> safe default (engine stays off).
            if (!_loggedConfigFallback)
            {
                _logger.LogWarning(
                    "SyncSettings row (Id=1) not found. Using safe defaults: Interval=30min, AutoSync=disabled. " +
                    "Auto-sync will remain off until the row is configured.");
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
                    "Failed to read SyncSettings (DB unreachable or not seeded). Using safe defaults: " +
                    "Interval=30min, AutoSync=disabled.");
                _loggedConfigFallback = true;
            }

            return DefaultSettings;
        }
    }

    // ---- Status helpers (all mutations go through _statusLock for visibility) ----------

    private void SetRunning(bool running)
    {
        lock (_statusLock)
        {
            _isRunning = running;
        }
    }

    private void SetStatus(bool running, string status, DateTime? lastSyncTime, int recordCount)
    {
        lock (_statusLock)
        {
            _isRunning = running;
            _lastStatus = status;
            if (lastSyncTime.HasValue)
                _lastSyncTime = lastSyncTime;
            _lastRecordCount = recordCount;
        }
    }

    private T LockRead<T>(Func<T> reader)
    {
        lock (_statusLock)
        {
            return reader();
        }
    }

    /// <summary>
    /// Builds the Oracle SELECT passed to <see cref="OracleExtractionService.ExtractAsync"/>.
    /// For a "Query" source the text is used verbatim (the extraction service still enforces
    /// read-only). For "Table"/"View" sources it is wrapped as SELECT * FROM &lt;source&gt;,
    /// with the source validated as a safe identifier to avoid injection.
    /// </summary>
    private static string BuildOracleQuery(TableMapping mapping)
    {
        if (mapping.SourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
            return mapping.OracleSource;

        if (!IsSafeOracleIdentifier(mapping.OracleSource))
        {
            throw new InvalidOperationException(
                $"Oracle source '{mapping.OracleSource}' is not a safe table/view identifier " +
                "(allowed: letters, digits, underscore, dot).");
        }

        return $"SELECT * FROM {mapping.OracleSource}";
    }

    private static bool IsSafeOracleIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        foreach (var c in identifier)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                return false;
        }

        return true;
    }

    /// <summary>
    /// Resolved scheduling configuration for the sync engine, read from the SyncSettings row
    /// (Id = 1). Immutable; a fresh instance is produced on every load.
    /// </summary>
    private sealed record SyncEngineSettings(int IntervalMinutes, bool IsAutoSyncEnabled, DateTime? LastSyncTimestamp);
}
