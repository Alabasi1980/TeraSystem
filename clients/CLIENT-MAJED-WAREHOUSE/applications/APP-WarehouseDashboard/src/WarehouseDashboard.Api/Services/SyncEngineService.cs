using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Api.Infrastructure;
using WarehouseDashboard.Api.Models;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// Sync Engine — the execution half of the Sync pipeline.
///
/// For each configured <see cref="TableMapping"/> it extracts the Oracle source via
/// <see cref="OracleExtractionService"/> and loads it into the SQL Server target via
/// <see cref="SqlServerLoadService"/>. Per-table failures are isolated (logged, retried up to
/// <see cref="MaxRetries"/>, then skipped) so one bad table never blocks the others.
///
/// Concurrency is managed externally by <see cref="SyncQueueService"/>, which ensures only
/// one sync operation runs at a time. This service no longer uses a semaphore — the queue
/// controls sequencing. The auto-sync timer also lives in <see cref="SyncQueueService"/>.
///
/// Runtime status (<see cref="IsRunning"/>, <see cref="LastSyncTime"/>, <see cref="LastStatus"/>,
/// <see cref="LastRecordCount"/>) is exposed thread-safely for the sync-status API endpoint.
/// </summary>
public class SyncEngineService
{
    private readonly OracleExtractionService _oracle;
    private readonly SqlServerLoadService _load;
    private readonly SyncRunProgressStore _progressStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncEngineService> _logger;

    private const int MaxRetries = 3;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

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

    // Table mappings are now loaded dynamically from the TableMappings DB table
    // at the start of each sync cycle (not from appsettings.json).
    private List<TableMapping> _mappings = new();

    public SyncEngineService(
        OracleExtractionService oracle,
        SqlServerLoadService load,
        SyncRunProgressStore progressStore,
        IConfiguration configuration,
        ILogger<SyncEngineService> logger)
    {
        _oracle = oracle;
        _load = load;
        _progressStore = progressStore;
        _configuration = configuration;
        _logger = logger;

        _logger.LogInformation(
            "SyncEngineService initialized. Mappings will be loaded from DB at each sync cycle.");
    }

    /// <summary>
    /// Runs exactly one full-refresh cycle over all configured mappings. Safe to call
    /// manually (e.g. from the queue service). One table failing must not stop the others.
    /// Concurrency is managed by <see cref="SyncQueueService"/> — this method assumes
    /// exclusive access. Mappings are reloaded from the DB at the start of each cycle.
    /// </summary>
    public async Task RunSyncOnceAsync(CancellationToken ct, string triggerType = "Auto")
    {
        // Tracking variables — declared here so they are accessible in catch blocks too.
        var cycleStartTime = DateTime.UtcNow;
        var mappingResults = new List<MappingSyncResult>();
        var totalRows = 0;

        try
        {
            // Reload mappings from DB at the start of each cycle.
            _mappings = await LoadMappingsFromDbAsync(ct);

            // Mark running and reset the per-run counters; LastStatus = "Running".
            SetStatus(running: true, status: "Running", lastSyncTime: null, recordCount: 0);

            _logger.LogInformation("Sync cycle started. {Count} mapping(s) loaded from DB.", _mappings.Count);

            foreach (var mapping in _mappings)
            {
                if (mapping is null)
                    continue;

                var target = mapping.SqlTargetTable;
                var succeeded = false;
                var mappingStartTime = DateTime.UtcNow;
                var mappingRows = 0;

                for (var attempt = 1; attempt <= MaxRetries; attempt++)
                {
                    try
                    {
                        var oracleSql = BuildOracleQuery(mapping, mapping.LastSyncAt);
                        _logger.LogInformation(
                            "Sync [{Target}] attempt {Attempt}/{Max}: extracting '{Source}' (mode={Mode}).",
                            target, attempt, MaxRetries, mapping.OracleSource, mapping.SyncMode);

                        var data = await _oracle.ExtractAsync(oracleSql, mapping.NumericTextColumns, ct);

                        if (mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase))
                        {
                            await _load.LoadTableIncrementalAsync(target, data, ct);
                        }
                        else
                        {
                            await _load.LoadTableAsync(target, data, ct);
                        }

                        _logger.LogInformation(
                            "Sync [{Target}] succeeded: {RowCount} row(s) loaded (mode={Mode}).", target, data.Rows.Count, mapping.SyncMode);
                        totalRows += data.Rows.Count;
                        mappingRows = data.Rows.Count;
                        succeeded = true;

                        // Update LastSyncAt (always) and IncrementalWatermarkAt (only for incremental).
                        await UpdateLastSyncAtAsync(mapping.Id, data.Rows.Count,
                            mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase), ct);

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

                // Record per-mapping result.
                mappingResults.Add(new MappingSyncResult
                {
                    Id = mapping.Id,
                    TargetTable = target,
                    Status = succeeded ? "success" : "failed",
                    Rows = mappingRows,
                    DurationSeconds = Math.Round((DateTime.UtcNow - mappingStartTime).TotalSeconds, 3),
                    Error = !succeeded
                        ? $"Exhausted {MaxRetries} attempt(s). See logs for details."
                        : null
                });

                if (!succeeded)
                {
                    _logger.LogError(
                        "Sync [{Target}] exhausted {Max} attempts and was skipped; continuing with remaining tables.",
                        target, MaxRetries);
                }
            }

            // Compute overall status based on per-mapping results.
            var successCount = mappingResults.Count(m => m.Status == "success");
            var failCount = mappingResults.Count(m => m.Status == "failed");
            string overallStatus;
            if (failCount == 0)
                overallStatus = "Success";
            else if (successCount > 0)
                overallStatus = "Partial";
            else
                overallStatus = "Failed";

            _logger.LogInformation(
                "Sync cycle finished. Status={Status}, {TotalRows} row(s) loaded across {Success}/{Total} mapping(s).",
                overallStatus, totalRows, successCount, mappingResults.Count);

            // Record completion time + totals.
            SetStatus(running: false, status: overallStatus, lastSyncTime: DateTime.UtcNow, recordCount: totalRows);

            // Persist sync run log to DB (wrapped: failure must not invalidate a successful data load).
            await PersistSyncRunToDbAsync(
                cycleStartTime, DateTime.UtcNow, overallStatus, triggerType,
                totalRows, DateTime.UtcNow - cycleStartTime, mappingResults, ct);

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

            // Persist cancelled state to DB (best-effort).
            await PersistSyncRunToDbAsync(
                cycleStartTime, DateTime.UtcNow, "Cancelled", triggerType,
                totalRows, DateTime.UtcNow - cycleStartTime, mappingResults, ct);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync cycle failed.");
            SetStatus(running: false, status: "Failed", lastSyncTime: null, recordCount: 0);

            // Persist failed state to DB (best-effort).
            await PersistSyncRunToDbAsync(
                cycleStartTime, DateTime.UtcNow, "Failed", triggerType,
                totalRows, DateTime.UtcNow - cycleStartTime, mappingResults, ct);
        }
    }

    /// <summary>
    /// Runs a sync cycle for only the specified table mappings (by primary key). Unlike
    /// <see cref="RunSyncOnceAsync"/> this method accepts a subset of mappings and supports
    /// progress tracking via <see cref="SyncRunProgress"/>. Concurrency is managed by
    /// <see cref="SyncQueueService"/>. Returns detailed per-mapping results.
    /// </summary>
    public async Task<SelectedSyncResult> RunSelectedMappingsAsync(
        List<int> mappingIds,
        CancellationToken ct,
        SyncRunProgress? progress = null)
    {
        // Mark running and reset the per-run counters.
        SetStatus(running: true, status: "Running (selected)", lastSyncTime: null, recordCount: 0);

        _logger.LogInformation(
            "Selected sync started for {Count} mapping(s).", mappingIds?.Count ?? 0);

        var mappings = await LoadMappingsByIdsAsync(mappingIds ?? new List<int>(), ct);

        if (mappings.Count == 0)
        {
            _logger.LogWarning(
                "Selected sync: no active mappings found for the requested IDs. No work performed.");

            SetStatus(running: false, status: "Success (none matched)", lastSyncTime: DateTime.UtcNow, recordCount: 0);

            return new SelectedSyncResult
            {
                Status = "success",
                TotalRows = 0,
                Mappings = (mappingIds ?? new List<int>()).Select(id => new MappingSyncResult
                {
                    Id = id,
                    TargetTable = "(not found)",
                    Status = "skipped",
                    Rows = 0,
                    Error = "No active mapping matches this ID."
                }).ToList()
            };
        }

        var totalRows = 0;
        var result = new SelectedSyncResult();
        var runId = progress?.RunId ?? Guid.Empty;
        var cycleStartTime = DateTime.UtcNow;

        foreach (var mapping in mappings)
        {
            if (mapping is null)
                continue;

            var target = mapping.SqlTargetTable;
            var mappingResult = new MappingSyncResult
            {
                Id = mapping.Id,
                TargetTable = target,
                Status = "failed",
                Rows = 0
            };
            var mappingStartTime = DateTime.UtcNow;

            // Signal progress: this mapping is now running.
            _progressStore.UpdateMapping(runId, mapping.Id, "running", 0);

            var succeeded = false;

            for (var attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    var oracleSql = BuildOracleQuery(mapping, mapping.LastSyncAt);
                    _logger.LogInformation(
                        "Selected sync [{Target}] (Id={Id}) attempt {Attempt}/{Max}: extracting '{Source}' (mode={Mode}).",
                        target, mapping.Id, attempt, MaxRetries, mapping.OracleSource, mapping.SyncMode);

                    var data = await _oracle.ExtractAsync(oracleSql, mapping.NumericTextColumns, ct);

                    // Signal progress: rows extracted, about to load.
                    _progressStore.UpdateMapping(runId, mapping.Id, "running", data.Rows.Count);

                    if (mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase))
                    {
                        await _load.LoadTableIncrementalAsync(target, data, ct);
                    }
                    else
                    {
                        await _load.LoadTableAsync(target, data, ct);
                    }

                    _logger.LogInformation(
                        "Selected sync [{Target}] (Id={Id}) succeeded: {RowCount} row(s) loaded (mode={Mode}).",
                        target, mapping.Id, data.Rows.Count, mapping.SyncMode);

                    totalRows += data.Rows.Count;
                    mappingResult.Rows = data.Rows.Count;
                    mappingResult.Status = "success";
                    mappingResult.DurationSeconds = Math.Round((DateTime.UtcNow - mappingStartTime).TotalSeconds, 3);
                    succeeded = true;

                    // Signal progress: mapping completed successfully.
                    _progressStore.UpdateMapping(runId, mapping.Id, "completed", data.Rows.Count);

                    // Update LastSyncAt (always) and IncrementalWatermarkAt (only for incremental).
                    // Full syncs must NOT overwrite IncrementalWatermarkAt — otherwise the
                    // user's InitialSyncStartDate would be bypassed on the next incremental run.
                    await UpdateLastSyncAtAsync(mapping.Id, data.Rows.Count,
                        mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase), ct);

                    break;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    // Shutdown requested — abort the whole selected sync.
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Selected sync [{Target}] (Id={Id}) attempt {Attempt}/{Max} failed: {Message}",
                        target, mapping.Id, attempt, MaxRetries, ex.Message);

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
                mappingResult.Status = "failed";
                mappingResult.Rows = 0;
                mappingResult.DurationSeconds = Math.Round((DateTime.UtcNow - mappingStartTime).TotalSeconds, 3);
                mappingResult.Error = $"Exhausted {MaxRetries} attempt(s). See logs for details.";
                _logger.LogError(
                    "Selected sync [{Target}] (Id={Id}) exhausted {Max} attempts and was skipped.",
                    target, mapping.Id, MaxRetries);

                // Signal progress: mapping failed after all retries.
                _progressStore.UpdateMapping(runId, mapping.Id, "failed", 0, mappingResult.Error);
            }

            result.Mappings.Add(mappingResult);
        }

        // Determine overall status.
        var successCount = result.Mappings.Count(m => m.Status == "success");
        var failCount = result.Mappings.Count(m => m.Status == "failed");

        if (failCount == 0)
        {
            result.Status = "success";
        }
        else if (successCount > 0)
        {
            result.Status = "partial";
        }
        else
        {
            result.Status = "failed";
        }

        result.TotalRows = totalRows;

        _logger.LogInformation(
            "Selected sync finished. Status={Status}, {TotalRows} row(s) loaded across {SuccessCount}/{TotalCount} mapping(s).",
            result.Status, totalRows, successCount, result.Mappings.Count);

        SetStatus(
            running: false,
            status: result.Status == "failed" ? "Failed" : "Success",
            lastSyncTime: DateTime.UtcNow,
            recordCount: totalRows);

        // Persist the sync timestamp so the dashboard "last sync" indicator is current.
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
            _logger.LogWarning(ex, "Selected sync completed but failed to update LastSyncTimestamp in SyncSettings.");
        }

        // Persist sync run log to DB (best-effort, must not fail the request).
        var runStatus = result.Status switch
        {
            "success" => "Success",
            "partial" => "Partial",
            _ => "Failed"
        };
        await PersistSyncRunToDbAsync(
            cycleStartTime, DateTime.UtcNow, runStatus, "Manual",
            totalRows, DateTime.UtcNow - cycleStartTime, result.Mappings, ct);

        return result;
    }

    /// <summary>
    /// Persists a sync run (and its per-mapping details) to the SyncRuns / SyncRunDetails tables.
    /// Called from <see cref="RunSyncOnceAsync"/> and <see cref="RunSelectedMappingsAsync"/> for
    /// all terminal statuses (Success, Partial, Failed, Cancelled).
    /// Entirely self-contained try/catch — a failure here must never interrupt or invalidate the
    /// sync cycle itself.
    /// </summary>
    private async Task PersistSyncRunToDbAsync(
        DateTime startTime,
        DateTime endTime,
        string status,
        string triggerType,
        int totalRows,
        TimeSpan totalDuration,
        List<MappingSyncResult>? mappingResults,
        CancellationToken ct)
    {
        try
        {
            var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            // Insert the SyncRun header row; retrieve the generated primary key.
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO SyncRuns (StartTime, EndTime, Status, TriggerType, TotalRecordCount, TotalDurationSeconds)
                OUTPUT INSERTED.Id
                VALUES (@start, @end, @status, @trigger, @count, @duration)
                """;
            cmd.Parameters.AddWithValue("@start", startTime);
            cmd.Parameters.AddWithValue("@end", endTime);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@trigger", triggerType);
            cmd.Parameters.AddWithValue("@count", totalRows);
            cmd.Parameters.AddWithValue("@duration", totalDuration.TotalSeconds);

            var runId = (int)await cmd.ExecuteScalarAsync(ct);

            // Insert per-mapping detail rows.
            if (mappingResults is not null && mappingResults.Count > 0)
            {
                foreach (var mr in mappingResults)
                {
                    await using var detailCmd = conn.CreateCommand();
                    detailCmd.CommandText = """
                        INSERT INTO SyncRunDetails (SyncRunId, TargetTable, RowsLoaded, Status, DurationSeconds, ErrorMessage)
                        VALUES (@runId, @targetTable, @rowsLoaded, @status, @duration, @error)
                        """;
                    detailCmd.Parameters.AddWithValue("@runId", runId);
                    detailCmd.Parameters.AddWithValue("@targetTable", mr.TargetTable);
                    detailCmd.Parameters.AddWithValue("@rowsLoaded", mr.Rows);
                    detailCmd.Parameters.AddWithValue("@status", mr.Status);
                    detailCmd.Parameters.AddWithValue("@duration", (object?)mr.DurationSeconds ?? DBNull.Value);
                    detailCmd.Parameters.AddWithValue("@error", (object?)mr.Error ?? DBNull.Value);

                    await detailCmd.ExecuteNonQueryAsync(ct);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to persist sync run log to database (run status: {Status}, trigger: {Trigger}). " +
                "The sync cycle itself completed successfully; only the audit record is missing.",
                status, triggerType);
        }
    }

    /// <summary>
    /// Loads active table mappings from the <c>TableMappings</c> DB table via ADO.NET.
    /// Called at the start of every sync cycle so runtime changes (added/removed/toggled
    /// via the Admin Panel) are picked up without restarting the service.
    /// Returns an empty list if the DB is unreachable or the table doesn't exist yet.
    /// </summary>
    public async Task<List<TableMapping>> LoadMappingsFromDbAsync(CancellationToken ct)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogDebug("SQL Server connection string is empty. No mappings loaded.");
            return new List<TableMapping>();
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT Id, OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn, LastSyncAt, InitialSyncStartDate, IncrementalWatermarkAt,
                       IsActive, SyncRecordCount
                FROM TableMappings
                WHERE IsActive = 1
                ORDER BY OracleSource
                """;

            var mappings = new List<TableMapping>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                mappings.Add(new TableMapping
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    OracleSource = reader.GetString(reader.GetOrdinal("OracleSource")),
                    SourceType = reader.GetString(reader.GetOrdinal("SourceType")),
                    SqlTargetTable = reader.GetString(reader.GetOrdinal("SqlTargetTable")),
                    SyncMode = reader.GetString(reader.GetOrdinal("SyncMode")),
                    IncrementalColumn = reader.IsDBNull(reader.GetOrdinal("IncrementalColumn"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("IncrementalColumn")),
                    LastSyncAt = reader.IsDBNull(reader.GetOrdinal("LastSyncAt"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("LastSyncAt")),
                    InitialSyncStartDate = reader.IsDBNull(reader.GetOrdinal("InitialSyncStartDate"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("InitialSyncStartDate")),
                    IncrementalWatermarkAt = reader.IsDBNull(reader.GetOrdinal("IncrementalWatermarkAt"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("IncrementalWatermarkAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    SyncRecordCount = reader.GetInt32(reader.GetOrdinal("SyncRecordCount"))
                });
            }

            // Close the reader before reusing the connection for LoadNumericTextColumnsAsync
            await reader.DisposeAsync();

            await LoadNumericTextColumnsAsync(conn, mappings, ct);

            _logger.LogInformation("Loaded {Count} active mapping(s) from DB.", mappings.Count);
            return mappings;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load table mappings from DB. Using previous mappings.");
            return _mappings.Count > 0 ? new List<TableMapping>(_mappings) : new List<TableMapping>();
        }
    }

    /// <summary>
    /// Loads only the table mappings whose primary keys are in <paramref name="mappingIds"/>
    /// and whose <c>IsActive</c> flag is <c>1</c>. Uses a parameterized IN clause to prevent
    /// SQL injection. Returns an empty list if none of the requested IDs match active mappings.
    /// </summary>
    public async Task<List<TableMapping>> LoadMappingsByIdsAsync(List<int> mappingIds, CancellationToken ct)
    {
        if (mappingIds is null || mappingIds.Count == 0)
        {
            _logger.LogDebug("LoadMappingsByIdsAsync called with empty list. Returning no mappings.");
            return new List<TableMapping>();
        }

        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogDebug("SQL Server connection string is empty. No mappings loaded.");
            return new List<TableMapping>();
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            // Build a parameterized IN clause: WHERE Id IN (@p0, @p1, @p2, ...)
            var paramNames = new List<string>();
            var parameters = new List<SqlParameter>();
            for (var i = 0; i < mappingIds.Count; i++)
            {
                var paramName = $"@p{i}";
                paramNames.Add(paramName);
                parameters.Add(new SqlParameter(paramName, System.Data.SqlDbType.Int) { Value = mappingIds[i] });
            }

            var inClause = string.Join(", ", paramNames);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"""
                SELECT Id, OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn, LastSyncAt, InitialSyncStartDate, IncrementalWatermarkAt,
                       IsActive, SyncRecordCount
                FROM TableMappings
                WHERE IsActive = 1
                  AND Id IN ({inClause})
                ORDER BY OracleSource
                """;

            cmd.Parameters.AddRange(parameters.ToArray());

            var mappings = new List<TableMapping>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                mappings.Add(new TableMapping
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    OracleSource = reader.GetString(reader.GetOrdinal("OracleSource")),
                    SourceType = reader.GetString(reader.GetOrdinal("SourceType")),
                    SqlTargetTable = reader.GetString(reader.GetOrdinal("SqlTargetTable")),
                    SyncMode = reader.GetString(reader.GetOrdinal("SyncMode")),
                    IncrementalColumn = reader.IsDBNull(reader.GetOrdinal("IncrementalColumn"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("IncrementalColumn")),
                    LastSyncAt = reader.IsDBNull(reader.GetOrdinal("LastSyncAt"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("LastSyncAt")),
                    InitialSyncStartDate = reader.IsDBNull(reader.GetOrdinal("InitialSyncStartDate"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("InitialSyncStartDate")),
                    IncrementalWatermarkAt = reader.IsDBNull(reader.GetOrdinal("IncrementalWatermarkAt"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("IncrementalWatermarkAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    SyncRecordCount = reader.GetInt32(reader.GetOrdinal("SyncRecordCount"))
                });
            }

            // Close the reader before reusing the connection for LoadNumericTextColumnsAsync
            await reader.DisposeAsync();

            await LoadNumericTextColumnsAsync(conn, mappings, ct);

            _logger.LogInformation(
                "Loaded {Count} mapping(s) from DB for requested IDs ({Requested} requested).",
                mappings.Count, mappingIds.Count);

            return mappings;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load mappings by IDs from DB. Returning empty list.");
            return new List<TableMapping>();
        }
    }

    private static async Task LoadNumericTextColumnsAsync(
        SqlConnection conn,
        List<TableMapping> mappings,
        CancellationToken ct)
    {
        if (mappings.Count == 0)
            return;

        var mappingById = mappings.ToDictionary(m => m.Id);
        var paramNames = new List<string>();
        await using var cmd = conn.CreateCommand();

        for (var i = 0; i < mappings.Count; i++)
        {
            var name = $"@id{i}";
            paramNames.Add(name);
            cmd.Parameters.Add(new SqlParameter(name, System.Data.SqlDbType.Int) { Value = mappings[i].Id });
        }

        cmd.CommandText = $"""
            SELECT TableMappingConfigId, OracleColumnName, SqlColumnName
            FROM ColumnMappings
            WHERE IsNumericText = 1
              AND IsExcluded = 0
              AND TableMappingConfigId IN ({string.Join(", ", paramNames)})
            """;

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var mappingId = reader.GetInt32(reader.GetOrdinal("TableMappingConfigId"));
            if (!mappingById.TryGetValue(mappingId, out var mapping))
                continue;

            if (!reader.IsDBNull(reader.GetOrdinal("OracleColumnName")))
                mapping.NumericTextColumns.Add(reader.GetString(reader.GetOrdinal("OracleColumnName")));

            if (!reader.IsDBNull(reader.GetOrdinal("SqlColumnName")))
                mapping.NumericTextColumns.Add(reader.GetString(reader.GetOrdinal("SqlColumnName")));
        }
    }

    // ---- Status helpers (all mutations go through _statusLock for visibility) ----------

    /// <summary>
    /// Updates <c>LastSyncAt</c> (always) and <c>IncrementalWatermarkAt</c> (only for incremental syncs)
    /// for a successfully synced mapping. <c>LastSyncAt</c> serves the dashboard "last synced" display
    /// and is updated on every sync. <c>IncrementalWatermarkAt</c> is the incremental boundary and
    /// must NOT be overwritten by full syncs. Failures here are logged but never propagate — a failed
    /// watermark update must not invalidate a successful data load.
    /// </summary>
    private async Task UpdateLastSyncAtAsync(int mappingId, int recordCount, bool isIncremental, CancellationToken ct)
    {
        try
        {
            var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
            if (string.IsNullOrWhiteSpace(connectionString)) return;

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();

            // LastSyncAt is always updated (for display).
            // IncrementalWatermarkAt is updated ONLY for incremental syncs (watermark).
            if (isIncremental)
            {
                cmd.CommandText = """
                    UPDATE TableMappings
                    SET LastSyncAt = GETUTCDATE(),
                        IncrementalWatermarkAt = GETUTCDATE(),
                        SyncRecordCount = @RecordCount,
                        UpdatedAt = GETUTCDATE()
                    WHERE Id = @Id
                    """;
            }
            else
            {
                cmd.CommandText = """
                    UPDATE TableMappings
                    SET LastSyncAt = GETUTCDATE(),
                        SyncRecordCount = @RecordCount,
                        UpdatedAt = GETUTCDATE()
                    WHERE Id = @Id
                    """;
            }

            cmd.Parameters.AddWithValue("@Id", mappingId);
            cmd.Parameters.AddWithValue("@RecordCount", recordCount);
            await cmd.ExecuteNonQueryAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update LastSyncAt for mapping {Id}.", mappingId);
        }
    }

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
    ///
    /// When <paramref name="mapping"/> uses <c>SyncMode = "Incremental"</c> with a valid
    /// <c>IncrementalColumn</c>, a <c>WHERE</c> clause is appended to fetch only rows
    /// newer than the watermark. The effective start date is determined by:
    /// 1. <c>IncrementalWatermarkAt</c> (set by previous incremental syncs)
    /// 2. <c>InitialSyncStartDate</c> (first-run configuration)
    /// 3. <paramref name="lastSyncAt"/> (fallback — should not normally be reached)
    /// </summary>
    private static string BuildOracleQuery(TableMapping mapping, DateTime? lastSyncAt)
    {
        string query;

        if (mapping.SourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
        {
            query = mapping.OracleSource;
        }
        else
        {
            if (!IsSafeOracleIdentifier(mapping.OracleSource))
            {
                throw new InvalidOperationException(
                    $"Oracle source '{mapping.OracleSource}' is not a safe table/view identifier " +
                    "(allowed: letters, digits, underscore, dot).");
            }

            query = $"SELECT * FROM {mapping.OracleSource}";
        }

        // Incremental WHERE clause: applicable to ALL source types (Table, View, Query)
        // when SyncMode is Incremental and a valid column + watermark exist.
        if (mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(mapping.IncrementalColumn) &&
            IsSafeOracleIdentifier(mapping.IncrementalColumn))
        {
            // Determine effective start date:
            // 1. Use IncrementalWatermarkAt (from the last incremental sync) as primary.
            // 2. Fall back to InitialSyncStartDate for the very first incremental run.
            // 3. Fall back to lastSyncAt as a safety net (should not normally be reached).
            // LastSyncAt is deliberately NOT used as the primary watermark because it is
            // updated on EVERY sync (including full syncs) and would incorrectly narrow
            // the incremental boundary if a full sync ran after an incremental one.
            DateTime? effectiveStartDate;
            if (mapping.IncrementalWatermarkAt.HasValue)
            {
                effectiveStartDate = mapping.IncrementalWatermarkAt;
            }
            else if (mapping.InitialSyncStartDate.HasValue)
            {
                effectiveStartDate = mapping.InitialSyncStartDate;
            }
            else
            {
                effectiveStartDate = lastSyncAt; // fallback — shouldn't happen
            }
            
            if (effectiveStartDate.HasValue)
            {
                var ts = effectiveStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                // Use >= (not >) to include rows _on_ the start date, and TO_DATE
                // instead of TIMESTAMP literal for reliable Oracle DATE column comparison.
                var whereClause = $" WHERE {mapping.IncrementalColumn} >= TO_DATE('{ts}', 'YYYY-MM-DD HH24:MI:SS')";
                
                // For Query sources: append WHERE only if the query doesn't already have one.
                if (mapping.SourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
                {
                    if (!query.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
                    {
                        query += whereClause;
                    }
                }
                else
                {
                    query += whereClause;
                }
            }
        }

        return query;
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
}
