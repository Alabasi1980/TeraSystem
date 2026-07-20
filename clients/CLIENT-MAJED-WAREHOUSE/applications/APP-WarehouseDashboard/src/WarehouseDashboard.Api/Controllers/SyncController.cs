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
            await _syncEngine.RunSyncOnceAsync(cancellationToken, "Manual");

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
    /// GET /api/sync/logs — returns persisted sync run history (newest first).
    /// Reads from the SyncRuns DB table via ADO.NET.
    /// </summary>
    [HttpGet("logs")]
    public async Task<IActionResult> Logs(CancellationToken ct)
    {
        var connStr = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connStr))
            return Ok(Array.Empty<object>());

        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT Id, StartTime, EndTime, Status, TriggerType,
                       TotalRecordCount, TotalDurationSeconds
                FROM SyncRuns
                ORDER BY StartTime DESC
                """;

            var records = new List<object>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                records.Add(new
                {
                    id = reader.GetInt32(0),
                    startTime = reader.GetDateTime(1),
                    endTime = reader.IsDBNull(2) ? null : (DateTime?)reader.GetDateTime(2),
                    status = reader.GetString(3),
                    triggerType = reader.GetString(4),
                    recordCount = reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5),
                    duration = reader.IsDBNull(6) ? null : (double?)reader.GetDouble(6)
                });
            }

            return Ok(records);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read sync logs from database.");
            return Ok(Array.Empty<object>());
        }
    }

    /// <summary>
    /// GET /api/sync/logs/{runId} — returns a single sync run with its per-mapping details.
    /// </summary>
    [HttpGet("logs/{runId:int}")]
    public async Task<IActionResult> LogDetail(int runId, CancellationToken ct)
    {
        var connStr = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connStr))
            return NotFound();

        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync(ct);

            // Fetch the run header.
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT Id, StartTime, EndTime, Status, TriggerType,
                       TotalRecordCount, TotalDurationSeconds
                FROM SyncRuns
                WHERE Id = @runId
                """;
            cmd.Parameters.AddWithValue("@runId", runId);

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (!await reader.ReadAsync(ct))
            {
                return NotFound(new { message = $"Sync run {runId} not found." });
            }

            var run = new
            {
                id = reader.GetInt32(0),
                startTime = reader.GetDateTime(1),
                endTime = reader.IsDBNull(2) ? null : (DateTime?)reader.GetDateTime(2),
                status = reader.GetString(3),
                triggerType = reader.GetString(4),
                recordCount = reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5),
                duration = reader.IsDBNull(6) ? null : (double?)reader.GetDouble(6)
            };

            // Close the header reader before fetching details.
            await reader.DisposeAsync();

            // Fetch per-mapping details.
            await using var detailCmd = conn.CreateCommand();
            detailCmd.CommandText = """
                SELECT TargetTable, RowsLoaded, Status, DurationSeconds, ErrorMessage
                FROM SyncRunDetails
                WHERE SyncRunId = @runId
                ORDER BY TargetTable
                """;
            detailCmd.Parameters.AddWithValue("@runId", runId);

            var details = new List<object>();
            await using var detailReader = await detailCmd.ExecuteReaderAsync(ct);
            while (await detailReader.ReadAsync(ct))
            {
                details.Add(new
                {
                    targetTable = detailReader.GetString(0),
                    rowsLoaded = detailReader.IsDBNull(1) ? null : (int?)detailReader.GetInt32(1),
                    status = detailReader.GetString(2),
                    durationSeconds = detailReader.IsDBNull(3) ? null : (double?)detailReader.GetDouble(3),
                    errorMessage = detailReader.IsDBNull(4) ? null : detailReader.GetString(4)
                });
            }

            return Ok(new { run, details });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read sync log detail for run {RunId}.", runId);
            return NotFound(new { message = $"Error reading sync run {runId} details." });
        }
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
