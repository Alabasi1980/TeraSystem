using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models.Dto;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Service for SQL Server View discovery and schema introspection.
/// Extracted from ReportService.cs during refactoring (TASK-FIX-REFACTOR-001).
/// </summary>
public class ReportViewService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReportViewService> _logger;

    public ReportViewService(
        IConfiguration configuration,
        ILogger<ReportViewService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string? GetConnectionString()
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogError("SQL Server connection string is not configured.");
        }
        return connectionString;
    }

    /// <summary>
    /// Retrieves all available Views from SQL Server (INFORMATION_SCHEMA.VIEWS).
    /// </summary>
    public async Task<List<ViewInfo>> GetAvailableViewsAsync(CancellationToken ct = default)
    {
        var results = new List<ViewInfo>();
        var connectionString = GetConnectionString();
        if (connectionString is null) return results;

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand(
                "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS ORDER BY TABLE_SCHEMA, TABLE_NAME",
                conn);
            cmd.CommandTimeout = 15;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                results.Add(new ViewInfo
                {
                    Schema = reader.GetString(0),
                    Name = reader.GetString(1),
                    FullName = $"[{reader.GetString(0)}].[{reader.GetString(1)}]"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve available SQL Server Views.");
        }

        return results;
    }

    /// <summary>
    /// Retrieves column metadata for a specific View.
    /// </summary>
    public async Task<List<ViewColumnInfo>> GetViewColumnsAsync(string viewName, CancellationToken ct = default)
    {
        var results = new List<ViewColumnInfo>();
        var connectionString = GetConnectionString();
        if (connectionString is null) return results;

        // Parse schema and table name from "schema.table" or "[schema].[table]"
        var parts = ParseViewName(viewName);
        if (parts is null)
        {
            _logger.LogWarning("Invalid view name format: {ViewName}", viewName);
            return results;
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand(
                "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE " +
                "FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table " +
                "ORDER BY ORDINAL_POSITION",
                conn);
            cmd.Parameters.AddWithValue("@schema", parts.Value.Schema);
            cmd.Parameters.AddWithValue("@table", parts.Value.Name);
            cmd.CommandTimeout = 15;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                results.Add(new ViewColumnInfo
                {
                    ColumnName = reader.GetString(0),
                    DataType = reader.GetString(1),
                    IsNullable = reader.GetString(2) == "YES",
                    MaxLength = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    NumericPrecision = reader.IsDBNull(4) ? null : reader.GetByte(4),
                    NumericScale = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve columns for view {ViewName}.", viewName);
        }

        return results;
    }

    /// <summary>
    /// RB-SEC-002: Validates that a specific view exists in SQL Server (INFORMATION_SCHEMA.VIEWS).
    /// </summary>
    public async Task<bool> ValidateViewExistsAsync(string viewName, CancellationToken ct = default)
    {
        var connectionString = GetConnectionString();
        if (connectionString is null) return false;

        var parts = ParseViewName(viewName);
        if (parts is null) return false;

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table",
                conn);
            cmd.Parameters.AddWithValue("@schema", parts.Value.Schema);
            cmd.Parameters.AddWithValue("@table", parts.Value.Name);
            cmd.CommandTimeout = 15;
            var count = (int)await cmd.ExecuteScalarAsync(ct);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate view existence for {ViewName}.", viewName);
            return false;
        }
    }

    /// <summary>
    /// Parses view name in formats: "schema.table", "[schema].[table]", "table" (uses dbo).
    /// </summary>
    private static (string Schema, string Name)? ParseViewName(string viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName)) return null;

        var cleaned = viewName.Replace("[", "").Replace("]", "");
        var parts = cleaned.Split('.');

        if (parts.Length == 2)
            return (parts[0].Trim(), parts[1].Trim());
        if (parts.Length == 1)
            return ("dbo", parts[0].Trim());

        return null;
    }
}
