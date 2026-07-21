using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WarehouseDashboard.Api.Infrastructure;
using WarehouseDashboard.Api.Models;

namespace WarehouseDashboard.Api.Controllers;

/// <summary>
/// REST API for managing dynamic Oracle-to-SQL Server table mappings.
/// Reads/writes the <c>TableMappings</c> config table via ADO.NET.
///
/// <para>
/// SECURITY (Phase 1, internal API): these endpoints intentionally have NO authentication.
/// The planned protection is IIS IP &amp; Domain Restrictions, applied at the web-server layer.
/// Do not expose this API outside the trusted internal network.
/// </para>
/// </summary>
[ApiController]
[Route("api/tablemappings")]
public class TableMappingController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TableMappingController> _logger;

    public TableMappingController(IConfiguration configuration, ILogger<TableMappingController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/tablemappings — list all table mappings from the DB.
    /// Used by the SyncEngineService to load active mappings.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Ok(Array.Empty<object>());
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT Id, OracleSource, SourceType, SqlTargetTable, IsActive,
                       CreatedAt, UpdatedAt, LastSyncAt, SyncRecordCount, ErrorMessage,
                       SyncMode, IncrementalColumn, InitialSyncStartDate
                FROM TableMappings
                ORDER BY OracleSource
                """;

            var mappings = new List<object>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                mappings.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("Id")),
                    oracleSource = reader.GetString(reader.GetOrdinal("OracleSource")),
                    sourceType = reader.GetString(reader.GetOrdinal("SourceType")),
                    sqlTargetTable = reader.GetString(reader.GetOrdinal("SqlTargetTable")),
                    isActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    createdAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    updatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    lastSyncAt = reader.IsDBNull(reader.GetOrdinal("LastSyncAt"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("LastSyncAt")),
                    syncRecordCount = reader.GetInt32(reader.GetOrdinal("SyncRecordCount")),
                    errorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("ErrorMessage")),
                    syncMode = reader.GetString(reader.GetOrdinal("SyncMode")),
                    incrementalColumn = reader.IsDBNull(reader.GetOrdinal("IncrementalColumn"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("IncrementalColumn")),
                    initialSyncStartDate = reader.IsDBNull(reader.GetOrdinal("InitialSyncStartDate"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("InitialSyncStartDate"))
                });
            }

            return Ok(mappings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list table mappings.");
            return StatusCode(500, new { error = "Failed to read table mappings." });
        }
    }

    /// <summary>
    /// GET /api/tablemappings/active — list only active table mappings.
    /// Used by the SyncEngineService to load mappings for sync cycles.
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Ok(Array.Empty<object>());
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn, InitialSyncStartDate
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
                    OracleSource = reader.GetString(reader.GetOrdinal("OracleSource")),
                    SourceType = reader.GetString(reader.GetOrdinal("SourceType")),
                    SqlTargetTable = reader.GetString(reader.GetOrdinal("SqlTargetTable")),
                    SyncMode = reader.GetString(reader.GetOrdinal("SyncMode")),
                    IncrementalColumn = reader.IsDBNull(reader.GetOrdinal("IncrementalColumn"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("IncrementalColumn")),
                    InitialSyncStartDate = reader.IsDBNull(reader.GetOrdinal("InitialSyncStartDate"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("InitialSyncStartDate"))
                });
            }

            return Ok(mappings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list active table mappings.");
            return StatusCode(500, new { error = "Failed to read active table mappings." });
        }
    }

    /// <summary>
    /// POST /api/tablemappings — create a new table mapping.
    /// The SQL Server target table is NOT created automatically (use apply-schema for that).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMappingRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OracleSource))
            return BadRequest(new { error = "OracleSource is required." });

        if (string.IsNullOrWhiteSpace(request.SqlTargetTable))
            return BadRequest(new { error = "SqlTargetTable is required." });

        var sourceType = string.IsNullOrWhiteSpace(request.SourceType) ? "Table" : request.SourceType;
        if (!new[] { "Table", "View", "Query" }.Contains(sourceType))
            return BadRequest(new { error = "SourceType must be one of: Table, View, Query." });

        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(500, new { error = "SQL Server connection string is not configured." });
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            // Check uniqueness
            await using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = """
                SELECT COUNT(1) FROM TableMappings
                WHERE OracleSource = @OracleSource OR SqlTargetTable = @SqlTargetTable
                """;
            checkCmd.Parameters.AddWithValue("@OracleSource", request.OracleSource);
            checkCmd.Parameters.AddWithValue("@SqlTargetTable", request.SqlTargetTable);

            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(ct)) > 0;
            if (exists)
            {
                return Conflict(new { error = "A mapping with this OracleSource or SqlTargetTable already exists." });
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO TableMappings (OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn, InitialSyncStartDate, IsActive, CreatedAt, UpdatedAt)
                OUTPUT INSERTED.Id
                VALUES (@OracleSource, @SourceType, @SqlTargetTable, @SyncMode, @IncrementalColumn, @InitialSyncStartDate, 1, GETUTCDATE(), GETUTCDATE())
                """;
            cmd.Parameters.AddWithValue("@OracleSource", request.OracleSource);
            cmd.Parameters.AddWithValue("@SourceType", sourceType);
            cmd.Parameters.AddWithValue("@SqlTargetTable", request.SqlTargetTable);
            cmd.Parameters.AddWithValue("@SyncMode", string.IsNullOrWhiteSpace(request.SyncMode) ? "Full" : request.SyncMode);
            cmd.Parameters.AddWithValue("@IncrementalColumn", (object?)request.IncrementalColumn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InitialSyncStartDate", (object?)request.InitialSyncStartDate ?? DBNull.Value);

            var newId = await cmd.ExecuteScalarAsync(ct);

            _logger.LogInformation(
                "Table mapping created: Id={Id}, '{Source}' -> '{Target}' (mode={Mode}).",
                newId, request.OracleSource, request.SqlTargetTable, request.SyncMode ?? "Full");

            return CreatedAtAction(nameof(GetAll), new { }, new
            {
                id = newId,
                oracleSource = request.OracleSource,
                sourceType = sourceType,
                sqlTargetTable = request.SqlTargetTable,
                syncMode = request.SyncMode ?? "Full",
                incrementalColumn = request.IncrementalColumn,
                isActive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create table mapping.");
            return StatusCode(500, new { error = "Failed to create table mapping." });
        }
    }

    /// <summary>
    /// PUT /api/tablemappings/{id} — update an existing table mapping.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMappingRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OracleSource))
            return BadRequest(new { error = "OracleSource is required." });

        if (string.IsNullOrWhiteSpace(request.SqlTargetTable))
            return BadRequest(new { error = "SqlTargetTable is required." });

        var sourceType = string.IsNullOrWhiteSpace(request.SourceType) ? "Table" : request.SourceType;

        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(500, new { error = "SQL Server connection string is not configured." });
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE TableMappings
                SET OracleSource = @OracleSource,
                    SourceType = @SourceType,
                    SqlTargetTable = @SqlTargetTable,
                    SyncMode = @SyncMode,
                    IncrementalColumn = @IncrementalColumn,
                    InitialSyncStartDate = @InitialSyncStartDate,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id
                """;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@OracleSource", request.OracleSource);
            cmd.Parameters.AddWithValue("@SourceType", sourceType);
            cmd.Parameters.AddWithValue("@SqlTargetTable", request.SqlTargetTable);
            cmd.Parameters.AddWithValue("@SyncMode", string.IsNullOrWhiteSpace(request.SyncMode) ? "Full" : request.SyncMode);
            cmd.Parameters.AddWithValue("@IncrementalColumn", (object?)request.IncrementalColumn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InitialSyncStartDate", (object?)request.InitialSyncStartDate ?? DBNull.Value);

            var affected = await cmd.ExecuteNonQueryAsync(ct);
            if (affected == 0)
            {
                return NotFound(new { error = $"Mapping with Id={id} not found." });
            }

            _logger.LogInformation("Table mapping {Id} updated (mode={Mode}).", id, request.SyncMode ?? "Full");
            return Ok(new
            {
                id,
                oracleSource = request.OracleSource,
                sourceType,
                sqlTargetTable = request.SqlTargetTable,
                syncMode = request.SyncMode ?? "Full",
                incrementalColumn = request.IncrementalColumn
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update table mapping {Id}.", id);
            return StatusCode(500, new { error = "Failed to update table mapping." });
        }
    }

    /// <summary>
    /// PATCH /api/tablemappings/{id}/toggle — toggle the IsActive flag.
    /// </summary>
    [HttpPatch("{id:int}/toggle")]
    public async Task<IActionResult> Toggle(int id, CancellationToken ct)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(500, new { error = "SQL Server connection string is not configured." });
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            // Read current state
            await using var readCmd = conn.CreateCommand();
            readCmd.CommandText = "SELECT IsActive FROM TableMappings WHERE Id = @Id";
            readCmd.Parameters.AddWithValue("@Id", id);

            var result = await readCmd.ExecuteScalarAsync(ct);
            if (result is null)
            {
                return NotFound(new { error = $"Mapping with Id={id} not found." });
            }

            var currentActive = (bool)result;

            // Toggle
            await using var updateCmd = conn.CreateCommand();
            updateCmd.CommandText = """
                UPDATE TableMappings
                SET IsActive = @IsActive, UpdatedAt = GETUTCDATE()
                WHERE Id = @Id
                """;
            updateCmd.Parameters.AddWithValue("@Id", id);
            updateCmd.Parameters.AddWithValue("@IsActive", !currentActive);
            await updateCmd.ExecuteNonQueryAsync(ct);

            _logger.LogInformation("Table mapping {Id} toggled from {From} to {To}.", id, currentActive, !currentActive);

            return Ok(new { id, isActive = !currentActive });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle table mapping {Id}.", id);
            return StatusCode(500, new { error = "Failed to toggle table mapping." });
        }
    }

    /// <summary>
    /// DELETE /api/tablemappings/{id} — soft-disable a mapping (set IsActive = false).
    /// Hard delete is not supported through the API.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(500, new { error = "SQL Server connection string is not configured." });
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE TableMappings
                SET IsActive = 0, UpdatedAt = GETUTCDATE()
                WHERE Id = @Id
                """;
            cmd.Parameters.AddWithValue("@Id", id);

            var affected = await cmd.ExecuteNonQueryAsync(ct);
            if (affected == 0)
            {
                return NotFound(new { error = $"Mapping with Id={id} not found." });
            }

            _logger.LogInformation("Table mapping {Id} soft-deleted (IsActive = 0).", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to soft-delete table mapping {Id}.", id);
            return StatusCode(500, new { error = "Failed to delete table mapping." });
        }
    }
}

/// <summary>
/// Request body for creating or updating a table mapping.
/// </summary>
public class CreateMappingRequest
{
    public string OracleSource { get; set; } = string.Empty;
    public string SourceType { get; set; } = "Table";
    public string SqlTargetTable { get; set; } = string.Empty;
    public string SyncMode { get; set; } = "Full";
    public string? IncrementalColumn { get; set; }
    public DateTime? InitialSyncStartDate { get; set; }
}
