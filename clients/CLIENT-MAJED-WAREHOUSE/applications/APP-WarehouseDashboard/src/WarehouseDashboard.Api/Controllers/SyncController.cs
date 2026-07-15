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
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncController> _logger;

    public SyncController(
        SyncEngineService syncEngine,
        SyncRunLogStore logStore,
        IConfiguration configuration,
        ILogger<SyncController> logger)
    {
        _syncEngine = syncEngine;
        _logStore = logStore;
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
