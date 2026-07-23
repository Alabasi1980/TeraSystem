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
    private readonly SyncQueueService _queue;
    private readonly SyncRunLogStore _logStore;
    private readonly SyncRunProgressStore _progressStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncController> _logger;
    private readonly ExportExcelService _exportExcel;

    public SyncController(
        SyncEngineService syncEngine,
        SyncQueueService queue,
        SyncRunLogStore logStore,
        SyncRunProgressStore progressStore,
        IConfiguration configuration,
        ILogger<SyncController> logger,
        ExportExcelService exportExcel)
    {
        _syncEngine = syncEngine;
        _queue = queue;
        _logStore = logStore;
        _progressStore = progressStore;
        _configuration = configuration;
        _logger = logger;
        _exportExcel = exportExcel;
    }

    /// <summary>
    /// POST /api/sync/trigger — enqueues a full-refresh sync cycle. The request is added to
    /// the centralized sync queue and will be processed when all prior requests complete.
    /// Returns the queue position immediately without waiting for execution.
    /// </summary>
    [HttpPost("trigger")]
    public IActionResult Trigger()
    {
        var position = _queue.EnqueueFull("Manual");

        return Ok(new
        {
            status = "queued",
            position,
            message = $"Full sync queued. Position: {position} in queue."
        });
    }

    /// <summary>
    /// POST /api/sync/trigger-selected — enqueues a sync cycle for the specified mapping IDs
    /// only. Accepts a JSON body like <c>{ "mappingIds": [1, 3, 5] }</c>.
    ///
    /// The request is added to the centralized sync queue and will be processed when all prior
    /// requests complete. The caller receives a <c>runId</c> to poll
    /// <c>GET /api/sync/progress?runId=...</c> for live per-mapping progress.
    ///
    /// Multiple Selected requests with overlapping IDs are merged into a single batch.
    /// If a Full sync is already queued, the Selected request is absorbed (Full covers all).
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

        _logger.LogInformation(
            "Manual selected-sync triggered for {Count} mapping ID(s): [{Ids}]",
            request.MappingIds.Count, string.Join(", ", request.MappingIds));

        // Resolve mappings to populate display names in the progress run.
        var mappings = await _syncEngine.LoadMappingsByIdsAsync(request.MappingIds, ct);
        var run = _progressStore.CreateRun(request.MappingIds, mappings);

        // Enqueue the request — the queue handles sequencing and log/progress tracking.
        var position = _queue.EnqueueSelected(
            request.MappingIds,
            progress: run,
            logRecord: null, // Queue service creates its own log record
            triggerType: "Manual (selected)");

        return Ok(new
        {
            status = "queued",
            position,
            runId = run.RunId,
            message = $"Selected sync queued. Position: {position} in queue."
        });
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
    /// GET /api/sync/queue — returns the current state of the centralized sync queue:
    /// whether a sync is processing, the current sync details, and pending requests.
    /// </summary>
    [HttpGet("queue")]
    public IActionResult Queue()
    {
        var status = _queue.GetQueueStatus();
        return Ok(status);
    }

    /// <summary>
    /// GET /api/sync/status — current runtime status of the sync engine.
    /// Falls back to <c>LastSyncTimestamp</c> from the SyncSettings DB table when the
    /// in-memory runtime status has not been initialized (survives API restarts).
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> Status(CancellationToken ct)
    {
        var lastSyncTime = _syncEngine.LastSyncTime;
        var lastStatus = _syncEngine.LastStatus;

        // If in-memory status was never set (e.g. after restart), try reading from DB.
        if (lastSyncTime is null && lastStatus == "Never")
        {
            try
            {
                var connStr = ConnectionStringHelper.ResolveSql(_configuration);
                if (!string.IsNullOrWhiteSpace(connStr))
                {
                    await using var conn = new SqlConnection(connStr);
                    await conn.OpenAsync(ct);
                    await using var cmd = conn.CreateCommand();
                    cmd.CommandText =
                        "SELECT LastSyncTimestamp FROM SyncSettings WHERE Id = 1";
                    var dbVal = await cmd.ExecuteScalarAsync(ct);
                    if (dbVal is not null && dbVal != DBNull.Value)
                    {
                        lastSyncTime = (DateTime)dbVal;
                        lastStatus = "Success";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex,
                    "Could not read LastSyncTimestamp from SyncSettings for status fallback. " +
                    "Returning in-memory defaults.");
            }
        }

        return Ok(new
        {
            isRunning = _syncEngine.IsRunning,
            lastSyncTime,
            lastStatus,
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
    /// GET /api/sync/{id:int}/export-excel — downloads a formatted Excel file
    /// for the target table of the given mapping.
    /// </summary>
    [HttpGet("{id:int}/export-excel")]
    public async Task<IActionResult> ExportExcel(int id, CancellationToken ct)
    {
        // 1. Load mapping to get SqlTargetTable
        var mappings = await _syncEngine.LoadMappingsByIdsAsync(new List<int> { id }, ct);
        var mapping = mappings.FirstOrDefault();
        if (mapping is null)
            return NotFound(new { message = "Mapping not found." });

        // 2. Check SqlTargetTable is not empty
        if (string.IsNullOrWhiteSpace(mapping.SqlTargetTable))
            return BadRequest(new { message = "Mapping has no target table." });

        // 3. Generate Excel
        var fileName = $"{mapping.SqlTargetTable}_{DateTime.Now:yyyy-MM-dd_HHmm}.xlsx";
        var bytes = await _exportExcel.GenerateAsync(mapping.SqlTargetTable, mapping.Name ?? mapping.SqlTargetTable, ct);

        if (bytes is null)
            return NotFound(new { message = $"Table '{mapping.SqlTargetTable}' not found or empty." });

        // 4. Return file
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    /// <summary>
    /// GET /api/sync/mappings — returns ALL table mappings from the database (active and inactive).
    /// Used by the Sync Dashboard to display the mappings table with toggle switches.
    /// Note: Uses its own query instead of LoadMappingsFromDbAsync because the sync engine
    /// method only returns active mappings (WHERE IsActive = 1).
    /// </summary>
    [HttpGet("mappings")]
    public async Task<IActionResult> Mappings(CancellationToken ct)
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
                SELECT Id, Name, OracleSource, SourceType, SqlTargetTable,
                       IsActive, LastSyncAt, SyncRecordCount
                FROM TableMappings
                ORDER BY OracleSource
                """;

            var results = new List<object>();

            // Step 1: Read all mapping data first (reader must be closed before additional queries).
            var rawMappings = new List<(int Id, string Name, string OracleSource, string SourceType,
                string SqlTargetTable, bool IsActive, DateTime? LastSyncAt, int SyncRecordCount)>();

            await using (var reader = await cmd.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    rawMappings.Add((
                        reader.GetInt32(reader.GetOrdinal("Id")),
                        reader.IsDBNull(reader.GetOrdinal("Name"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("OracleSource")),
                        reader.GetString(reader.GetOrdinal("SourceType")),
                        reader.GetString(reader.GetOrdinal("SqlTargetTable")),
                        reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        reader.IsDBNull(reader.GetOrdinal("LastSyncAt"))
                            ? (DateTime?)null
                            : reader.GetDateTime(reader.GetOrdinal("LastSyncAt")),
                        reader.GetInt32(reader.GetOrdinal("SyncRecordCount"))
                    ));
                }
            }

            // Step 2: Query actual row count for each target table and build results.
            foreach (var (id, name, oracleSource, sourceType, sqlTargetTable, isActive, lastSyncTime, lastRecordCount) in rawMappings)
            {
                int? storedRecordCount = null;
                if (!string.IsNullOrWhiteSpace(sqlTargetTable))
                {
                    try
                    {
                        string countQuery;
                        if (sqlTargetTable.Contains('.'))
                        {
                            var parts = sqlTargetTable.Split('.');
                            countQuery = $"SELECT COUNT(1) FROM [{parts[0].Replace("]", "]]")}].[{parts[1].Replace("]", "]]")}]";
                        }
                        else
                        {
                            countQuery = $"SELECT COUNT(1) FROM [{sqlTargetTable.Replace("]", "]]")}]";
                        }

                        await using var countCmd = conn.CreateCommand();
                        countCmd.CommandText = countQuery;
                        var countVal = await countCmd.ExecuteScalarAsync(ct);
                        if (countVal is not null && countVal != DBNull.Value)
                            storedRecordCount = Convert.ToInt32(countVal);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to count rows for table {Table}.", sqlTargetTable);
                    }
                }

                results.Add(new
                {
                    id,
                    name,
                    oracleSource,
                    sourceType,
                    sqlTargetTable,
                    isActive,
                    lastSyncTime,
                    lastRecordCount,
                    storedRecordCount
                });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load mappings for dashboard. Returning empty list.");
            return Ok(Array.Empty<object>());
        }
    }

    /// <summary>
    /// GET /api/sync/exportable-mappings — returns all active mappings with table-existence
    /// and row-count information from SQL Server, plus the last sync timestamp.
    /// Used by the public DataExports page to show which exports are available.
    ///
    /// <para>
    /// For each mapping the endpoint checks whether the target SQL table exists
    /// (<c>OBJECT_ID</c>) and, if it does, retrieves <c>COUNT(1)</c>. Tables that do not
    /// exist or are empty are returned with <c>hasData: false</c> and <c>rowCount: null</c>.
    /// The <c>lastSyncTime</c> per mapping falls back to the global
    /// <c>SyncSettings.LastSyncTimestamp</c> when the mapping-level value is null.
    /// </para>
    ///
    /// <para>
    /// If the database is unreachable the endpoint returns the mapping list without row
    /// counts and logs a warning — it never throws for the caller.
    /// </para>
    /// </summary>
    [HttpGet("exportable-mappings")]
    public async Task<IActionResult> ExportableMappings(CancellationToken ct)
    {
        var mappings = await _syncEngine.LoadMappingsFromDbAsync(ct);
        var connStr = ConnectionStringHelper.ResolveSql(_configuration);

        // General fallback timestamp from SyncSettings table
        DateTime? generalLastSync = null;

        if (!string.IsNullOrWhiteSpace(connStr))
        {
            try
            {
                await using var conn = new SqlConnection(connStr);
                await conn.OpenAsync(ct);

                // 1. Read the global LastSyncTimestamp once.
                await using var tsCmd = conn.CreateCommand();
                tsCmd.CommandText = "SELECT LastSyncTimestamp FROM SyncSettings WHERE Id = 1";
                var tsVal = await tsCmd.ExecuteScalarAsync(ct);
                if (tsVal is not null && tsVal != DBNull.Value)
                    generalLastSync = (DateTime)tsVal;

                // 2. Build results — one connection reused for all table checks.
                var results = new List<object>();

                foreach (var mapping in mappings)
                {
                    bool hasData = false;
                    int? rowCount = null;

                    if (!string.IsNullOrWhiteSpace(mapping.SqlTargetTable))
                    {
                        try
                        {
                            // Build safe OBJECT_ID check and COUNT query
                            // (table names come from our own configuration — not user input)
                            string objCheck;
                            string countQuery;

                            if (mapping.SqlTargetTable.Contains('.'))
                            {
                                // Schema-qualified: dbo.stg_WarehouseStock
                                var parts = mapping.SqlTargetTable.Split('.');
                                objCheck = $"IF OBJECT_ID(N'{mapping.SqlTargetTable.Replace("'", "''")}', N'U') IS NOT NULL SELECT 1 ELSE SELECT 0";
                                countQuery = $"SELECT COUNT(1) FROM [{parts[0].Replace("]", "]]")}].[{parts[1].Replace("]", "]]")}]";
                            }
                            else
                            {
                                objCheck = $"IF OBJECT_ID(N'[{mapping.SqlTargetTable.Replace("]", "]]")}]', N'U') IS NOT NULL SELECT 1 ELSE SELECT 0";
                                countQuery = $"SELECT COUNT(1) FROM [{mapping.SqlTargetTable.Replace("]", "]]")}]";
                            }

                            await using var objCmd = conn.CreateCommand();
                            objCmd.CommandText = objCheck;
                            var exists = (int)(await objCmd.ExecuteScalarAsync(ct))!;

                            if (exists == 1)
                            {
                                await using var countCmd = conn.CreateCommand();
                                countCmd.CommandText = countQuery;
                                var countVal = await countCmd.ExecuteScalarAsync(ct);
                                rowCount = countVal is not null && countVal != DBNull.Value
                                    ? Convert.ToInt32(countVal)
                                    : 0;
                                hasData = rowCount > 0;
                            }
                            // else: table does not exist → hasData: false, rowCount: null
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex,
                                "exportable-mappings: failed to inspect table '{Table}' for mapping {Id}. " +
                                "Returning mapping without row data.",
                                mapping.SqlTargetTable, mapping.Id);
                        }
                    }

                    results.Add(new
                    {
                        id = mapping.Id,
                        name = !string.IsNullOrWhiteSpace(mapping.Name)
                            ? mapping.Name
                            : mapping.SqlTargetTable,
                        sqlTargetTable = mapping.SqlTargetTable,
                        sourceType = mapping.SourceType,
                        hasData,
                        rowCount,
                        lastSyncTime = mapping.LastSyncAt ?? generalLastSync
                    });
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "exportable-mappings: database error. Returning mappings without row data.");
            }
        }

        // Fallback: DB unavailable or error — return mappings without DB-derived info.
        return Ok(mappings.Select(m => new
        {
            id = m.Id,
            name = !string.IsNullOrWhiteSpace(m.Name)
                ? m.Name
                : m.SqlTargetTable,
            sqlTargetTable = m.SqlTargetTable,
            sourceType = m.SourceType,
            hasData = false,
            rowCount = (int?)null,
            lastSyncTime = m.LastSyncAt
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
