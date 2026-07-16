using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Api.Infrastructure;
using WarehouseDashboard.Api.Models;
using WarehouseDashboard.Api.Services;

namespace WarehouseDashboard.Api.Controllers;

/// <summary>
/// REST API for controlling and monitoring the Sync Engine
/// (see 14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md §5).
///
/// <para>
/// SECURITY (Phase 1, internal API): these endpoints intentionally have NO authentication.
/// The planned protection is IIS IP &amp; Domain Restrictions, applied at the web-server layer
/// (see §5.2). Do not expose this API outside the trusted internal network.
/// </para>
/// </summary>
[ApiController]
[Route("api/sync")]
public class SyncController : ControllerBase
{
    private readonly SyncEngineService _syncEngine;
    private readonly SyncRunLogStore _logStore;
    private readonly SyncRunProgressStore _progressStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncController> _logger;

    public SyncController(
        SyncEngineService syncEngine,
        SyncRunLogStore logStore,
        SyncRunProgressStore progressStore,
        IConfiguration configuration,
        ILogger<SyncController> logger)
    {
        _syncEngine = syncEngine;
        _logStore = logStore;
        _progressStore = progressStore;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/sync/trigger — manually triggers one full-refresh sync cycle.
    /// Overlapping runs are skipped by the engine's internal semaphore; in that case the
    /// cycle returns immediately and the log record reflects the engine's current status.
    /// </summary>
    [HttpPost("trigger")]
    public async Task<IActionResult> Trigger(CancellationToken cancellationToken)
    {
        var record = _logStore.BeginRun("Manual");

        try
        {
            await _syncEngine.RunSyncOnceAsync(cancellationToken);

            // The engine owns the authoritative status; reflect it in the log entry.
            _logStore.CompleteRun(record, _syncEngine.LastStatus, _syncEngine.LastRecordCount, null);

            return Ok(new
            {
                status = "triggered",
                message = "Sync cycle executed. Check /api/sync/status and /api/sync/logs for results."
            });
        }
        catch (OperationCanceledException)
        {
            _logStore.CompleteRun(record, "Cancelled", _syncEngine.LastRecordCount,
                "Sync was cancelled (request aborted or host shutting down).");
            return Ok(new
            {
                status = "triggered",
                message = "Sync cycle was cancelled."
            });
        }
        catch (Exception ex)
        {
            // The engine normally swallows non-cancellation exceptions; this is defensive.
            _logStore.CompleteRun(record, "Failed", _syncEngine.LastRecordCount, ex.Message);
            _logger.LogError(ex, "Manual sync trigger failed unexpectedly.");
            return Ok(new
            {
                status = "triggered",
                message = $"Sync cycle failed: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// POST /api/sync/trigger-selected — triggers a sync cycle for the specified mapping IDs
    /// only. Accepts a JSON body like <c>{ "mappingIds": [1, 3, 5] }</c>.
    ///
    /// Unlike <c>POST /api/sync/trigger</c>, this endpoint returns immediately with a
    /// <c>runId</c> and executes the actual sync in a background task. The caller polls
    /// <c>GET /api/sync/progress?runId=...</c> to watch live per-mapping progress.
    ///
    /// This endpoint does NOT acquire the engine's semaphore, so it can be called even while
    /// the automatic background cycle is running.
    ///
    /// Returns <c>400 Bad Request</c> if <c>mappingIds</c> is null or empty.
    /// </summary>
    [HttpPost("trigger-selected")]
    public async Task<IActionResult> TriggerSelected([FromBody] SelectedSyncRequest request, CancellationToken ct)
    {
        if (request?.MappingIds is null || request.MappingIds.Count == 0)
        {
            return BadRequest(new
            {
                status = "error",
                message = "mappingIds is required and must not be empty."
            });
        }

        var logRecord = _logStore.BeginRun("Manual (selected)");

        _logger.LogInformation(
            "Manual selected-sync triggered for {Count} mapping ID(s): [{Ids}]",
            request.MappingIds.Count, string.Join(", ", request.MappingIds));

        // Resolve mappings to populate display names in the progress run.
        var mappings = await _syncEngine.LoadMappingsByIdsAsync(request.MappingIds, ct);
        var run = _progressStore.CreateRun(request.MappingIds, mappings);

        // Fire-and-forget: run the sync in the background. CancellationToken.None is
        // intentional so the sync is not aborted when the HTTP request is cancelled.
        _ = Task.Run(async () =>
        {
            try
            {
                var result = await _syncEngine.RunSelectedMappingsAsync(
                    request.MappingIds, CancellationToken.None, run);

                _progressStore.CompleteRun(
                    run.RunId,
                    result.Status == "success" || result.Status == "partial"
                        ? result.Status
                        : "failed");

                // Collect error messages from failed mappings so the log record is useful.
                var failedErrors = result.Mappings
                    .Where(m => m.Status == "failed" && !string.IsNullOrEmpty(m.Error))
                    .Select(m => $"{m.TargetTable}: {m.Error}");
                var combinedError = failedErrors.Any() ? string.Join("; ", failedErrors) : null;

                _logStore.CompleteRun(logRecord, result.Status == "success" ? "Success" : result.Status == "partial" ? "Partial" : "Failed", result.TotalRows, combinedError);
            }
            catch (Exception ex)
            {
                _progressStore.CompleteRun(run.RunId, "failed");
                _logStore.CompleteRun(logRecord, "Failed", 0, ex.Message);
                _logger.LogError(ex, "Background sync run {RunId} failed.", run.RunId);
            }
        }, CancellationToken.None);

        return Ok(new { runId = run.RunId, status = "started" });
    }

    /// <summary>
    /// GET /api/sync/progress?runId={guid} — returns the current live progress of a background
    /// sync run started by <c>POST /api/sync/trigger-selected</c>.
    ///
    /// Returns <c>404 Not Found</c> if the runId does not exist or has been cleaned up
    /// (runs older than 5 minutes are evicted).
    /// </summary>
    [HttpGet("progress")]
    public IActionResult Progress([FromQuery] Guid runId)
    {
        var run = _progressStore.GetRun(runId);
        if (run is null)
        {
            return NotFound(new
            {
                status = "error",
                message = "Run not found or expired."
            });
        }

        // Update elapsed time on each poll so the caller always gets a fresh value.
        run.ElapsedSeconds = Math.Round((DateTime.UtcNow - run.StartedAt).TotalSeconds, 1);

        return Ok(run);
    }

    /// <summary>
    /// GET /api/sync/status — current runtime status of the sync engine.
    /// </summary>
    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            isRunning = _syncEngine.IsRunning,
            lastSyncTime = _syncEngine.LastSyncTime,
            lastStatus = _syncEngine.LastStatus,
            lastRecordCount = _syncEngine.LastRecordCount
        });
    }

    /// <summary>
    /// GET /api/sync/logs — the most recent sync runs (newest first, max 100).
    ///
    /// NOTE: backed by an in-memory ring buffer (see <see cref="SyncRunLogStore"/>) until the
    /// structured SyncLogs / ErrorLogs DB tables are implemented. Lost on restart.
    /// </summary>
    [HttpGet("logs")]
    public IActionResult Logs()
    {
        var records = _logStore.GetRecent().Select(r => new
        {
            startTime = r.StartTime,
            endTime = r.EndTime,
            status = r.Status,
            recordCount = r.RecordCount,
            duration = r.DurationSeconds,
            triggerType = r.TriggerType,
            errorMessage = r.ErrorMessage
        });

        return Ok(records);
    }

    /// <summary>
    /// GET /api/sync/mappings — returns all active table mappings from the database.
    /// Used by the Sync Dashboard to display the mappings table with checkboxes.
    /// </summary>
    [HttpGet("mappings")]
    public async Task<IActionResult> Mappings(CancellationToken ct)
    {
        var mappings = await _syncEngine.LoadMappingsFromDbAsync(ct);
        return Ok(mappings.Select(m => new
        {
            m.Id,
            name = m.Name,
            m.OracleSource,
            m.SourceType,
            m.SqlTargetTable,
            isActive = true
        }));
    }

    /// <summary>
    /// GET /api/sync/config — current sync scheduling configuration, read from the
    /// <c>SyncSettings</c> table (row Id = 1) via ADO.NET. Returns safe defaults if the
    /// row is missing or the database is unreachable.
    /// </summary>
    [HttpGet("config")]
    public async Task<IActionResult> Config(CancellationToken cancellationToken)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning(
                "SQL Server connection string is empty (is SQL_PASSWORD set?). Returning safe defaults.");
            return Ok(SafeDefaults());
        }

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            // Column names are fixed by the data model; the literal Id = 1 is not user input,
            // so there is no SQL-injection surface here.
            command.CommandText =
                "SELECT IntervalMinutes, IsAutoSyncEnabled, LastSyncTimestamp " +
                "FROM SyncSettings WHERE Id = 1";

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var interval = reader.GetInt32(reader.GetOrdinal("IntervalMinutes"));
                var enabled = reader.GetBoolean(reader.GetOrdinal("IsAutoSyncEnabled"));
                var lastSyncTimestamp = reader.IsDBNull(reader.GetOrdinal("LastSyncTimestamp"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("LastSyncTimestamp"));

                return Ok(new
                {
                    intervalMinutes = interval,
                    isAutoSyncEnabled = enabled,
                    lastSyncTimestamp
                });
            }

            // Row does not exist yet (DB not seeded) -> safe defaults.
            return Ok(SafeDefaults());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to read SyncSettings (DB unreachable or not seeded). Returning safe defaults.");
            return Ok(SafeDefaults());
        }
    }

    private static object SafeDefaults() =>
        new
        {
            intervalMinutes = 30,
            isAutoSyncEnabled = false,
            lastSyncTimestamp = (DateTime?)null
        };
}
