using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models.Dto;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Service for dynamic query execution and filter/parameter options.
/// Extracted from ReportService.cs during refactoring (TASK-FIX-REFACTOR-001).
/// </summary>
public class ReportExecutionService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReportExecutionService> _logger;
    private readonly WarehouseDashboardDbContext _db;

    public ReportExecutionService(
        IConfiguration configuration,
        ILogger<ReportExecutionService> logger,
        WarehouseDashboardDbContext db)
    {
        _configuration = configuration;
        _logger = logger;
        _db = db;
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
    /// Executes a report with the given filter values.
    /// Builds a parameterized SQL query dynamically from the report definition.
    /// Returns columns metadata + rows data + elapsed time.
    /// </summary>
    public async Task<ReportDataResult> ExecuteReportAsync(
        int reportId,
        Dictionary<string, object?>? filterValues,
        CancellationToken ct = default)
    {
        var result = new ReportDataResult();

        try
        {
            // 1. Get report definition
            var report = await _db.Reports
                .Include(r => r.Columns.OrderBy(c => c.SortOrder))
                .Include(r => r.Filters)
                .FirstOrDefaultAsync(r => r.Id == reportId, ct);

            if (report is null)
            {
                result.Success = false;
                result.ErrorMessage = "التقرير غير موجود.";
                return result;
            }

            if (!report.Columns.Any())
            {
                result.Success = false;
                result.ErrorMessage = "التقرير لا يحتوي على أعمدة.";
                return result;
            }

            // 2. Build column list for SELECT
            var columnNames = report.Columns
                .Where(c => c.IsVisible)
                .OrderBy(c => c.SortOrder)
                .Select(c => $"[{c.ColumnName}]")
                .ToList();

            // If no visible columns, use all
            if (!columnNames.Any())
            {
                columnNames = report.Columns
                    .OrderBy(c => c.SortOrder)
                    .Select(c => $"[{c.ColumnName}]")
                    .ToList();
            }

            // 3. Build the base SELECT query
            var selectClause = string.Join(", ", columnNames);
            var sql = new System.Text.StringBuilder();
            sql.Append($"SELECT {selectClause} FROM {report.ViewName} WITH (NOLOCK)");

            // 4. Build WHERE clause from filter values
            var parameters = new List<SqlParameter>();
            var whereClauses = new List<string>();

            if (filterValues is not null && filterValues.Count > 0)
            {
                foreach (var filter in report.Filters.OrderBy(f => f.SortOrder))
                {
                    if (!filterValues.TryGetValue(filter.ColumnName, out var rawValue))
                        continue;

                    if (rawValue is null || (rawValue is string s && string.IsNullOrWhiteSpace(s)))
                        continue;

                    string paramName = $"@p_{filter.ColumnName.Replace(" ", "_")}";

                    switch (filter.FilterType)
                    {
                        case "Text":
                            whereClauses.Add($"[{filter.ColumnName}] LIKE {paramName}");
                            parameters.Add(new SqlParameter(paramName, $"%{rawValue}%"));
                            break;

                        case "Date":
                            if (DateTime.TryParse(rawValue.ToString(), out var dateVal))
                            {
                                whereClauses.Add($"CAST([{filter.ColumnName}] AS DATE) = CAST({paramName} AS DATE)");
                                parameters.Add(new SqlParameter(paramName, dateVal.Date));
                            }
                            break;

                        case "DateRange":
                            // expects value format: "start|end" or JSON { start, end }
                            var parts = rawValue.ToString()?.Split('|');
                            if (parts is not null && parts.Length == 2)
                            {
                                if (DateTime.TryParse(parts[0], out var fromDate))
                                {
                                    whereClauses.Add($"[{filter.ColumnName}] >= @{paramName}_from");
                                    parameters.Add(new SqlParameter($"{paramName}_from", fromDate));
                                }
                                if (DateTime.TryParse(parts[1], out var toDate))
                                {
                                    whereClauses.Add($"[{filter.ColumnName}] < @{paramName}_to");
                                    parameters.Add(new SqlParameter($"{paramName}_to", toDate.AddDays(1)));
                                }
                            }
                            break;

                        case "Dropdown":
                            whereClauses.Add($"[{filter.ColumnName}] = {paramName}");
                            parameters.Add(new SqlParameter(paramName, rawValue.ToString()));
                            break;

                        case "Number":
                            if (decimal.TryParse(rawValue.ToString(), out var numVal))
                            {
                                whereClauses.Add($"[{filter.ColumnName}] = {paramName}");
                                parameters.Add(new SqlParameter(paramName, numVal));
                            }
                            break;

                        case "NumberRange":
                            // expects value format: "min|max"
                            var rangeParts = rawValue.ToString()?.Split('|');
                            if (rangeParts is not null && rangeParts.Length == 2)
                            {
                                if (decimal.TryParse(rangeParts[0], out var minVal))
                                {
                                    whereClauses.Add($"[{filter.ColumnName}] >= @{paramName}_min");
                                    parameters.Add(new SqlParameter($"{paramName}_min", minVal));
                                }
                                if (decimal.TryParse(rangeParts[1], out var maxVal))
                                {
                                    whereClauses.Add($"[{filter.ColumnName}] <= @{paramName}_max");
                                    parameters.Add(new SqlParameter($"{paramName}_max", maxVal));
                                }
                            }
                            break;
                    }
                }
            }

            if (whereClauses.Any())
            {
                sql.Append(" WHERE ");
                sql.Append(string.Join(" AND ", whereClauses));
            }

            sql.Append(" ORDER BY ");
            // Use first visible column as default sort if available
            var firstCol = report.Columns
                .Where(c => c.IsVisible && c.IsSortable)
                .OrderBy(c => c.SortOrder)
                .Select(c => c.ColumnName)
                .FirstOrDefault();

            if (firstCol is not null)
                sql.Append($"[{firstCol}] ASC");
            else
                sql.Append("(SELECT NULL)"); // no sort = natural order

            // 5. Execute the query
            var connectionString = GetConnectionString();
            if (connectionString is null)
            {
                result.Success = false;
                result.ErrorMessage = "إعدادات الاتصال بقاعدة البيانات غير متوفرة.";
                return result;
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand(sql.ToString(), conn);
            cmd.Parameters.AddRange(parameters.ToArray());
            cmd.CommandTimeout = 60;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            stopwatch.Stop();

            // 6. Build column metadata from actual reader schema
            var columnMeta = new List<ReportDataColumn>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnMeta.Add(new ReportDataColumn
                {
                    Name = reader.GetName(i),
                    DataType = reader.GetFieldType(i).Name
                });
            }

            // 7. Read rows (max 10,000)
            var rows = new List<Dictionary<string, object?>>();
            int rowCount = 0;
            const int maxRows = 10_000;

            while (await reader.ReadAsync(ct) && rowCount < maxRows)
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[columnMeta[i].Name] = NormalizeValue(reader.GetValue(i));
                }
                rows.Add(row);
                rowCount++;
            }

            // 8. Attach display info from ReportColumns definition
            var displayColumns = report.Columns
                .Where(c => c.IsVisible)
                .OrderBy(c => c.SortOrder)
                .Select(c => new ReportDataColumn
                {
                    Name = c.ColumnName,
                    DisplayName = c.DisplayName,
                    DataType = c.DataType,
                    Width = c.Width,
                    IsImageColumn = c.IsImageColumn,
                    ImageBaseUrl = c.ImageBaseUrl,
                    DateFormat = c.DateFormat,
                    NumberFormat = c.NumberFormat
                }).ToList();

            // If no visible columns, use all columns with raw names
            if (!displayColumns.Any())
            {
                displayColumns = columnMeta;
            }

            result.Success = true;
            result.Columns = displayColumns;
            result.Rows = rows;
            result.RowCount = rows.Count;
            result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            result.ReachedMaxRows = rowCount >= maxRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute report {ReportId}. Error: {ErrorMessage}", reportId, ex.Message);
            result.Success = false;
            result.ErrorMessage = "حدث خطأ أثناء تنفيذ التقرير.";
        }

        return result;
    }

    /// <summary>
    /// Normalizes a raw database value for JSON serialization.
    /// </summary>
    private static object? NormalizeValue(object value)
    {
        if (value is DBNull or null)
            return null;

        if (value is byte[])
            return "<binary>";

        if (value is DateTime dt)
            return dt.ToString("yyyy-MM-dd HH:mm:ss");

        if (value is decimal || value is double || value is float)
            return value; // Preserve numeric types for AG Grid

        if (value is short || value is int || value is long)
            return Convert.ToInt64(value);

        return value.ToString();
    }

    /// <summary>
    /// Gets distinct values from a View column for Dropdown filter options.
    /// </summary>
    public async Task<List<string>> GetFilterOptionsAsync(
        string viewName,
        string columnName,
        CancellationToken ct = default)
    {
        var results = new List<string>();
        var connectionString = GetConnectionString();
        if (connectionString is null) return results;

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            var sql = $"SELECT DISTINCT [{columnName}] FROM {viewName} WITH (NOLOCK) WHERE [{columnName}] IS NOT NULL ORDER BY [{columnName}]";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 30;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var val = reader.GetValue(0);
                if (val is not null && val != DBNull.Value)
                    results.Add(val.ToString() ?? string.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get filter options for {ViewName}.{ColumnName}.", viewName, columnName);
        }

        return results;
    }

    /// <summary>
    /// Gets parameter options for a report filter using its configured OptionsQuery.
    /// </summary>
    public async Task<List<ParameterOption>> GetParameterOptionsAsync(
        int reportId,
        int filterId,
        CancellationToken ct = default)
    {
        var results = new List<ParameterOption>();
        try
        {
            var filter = await _db.ReportFilters
                .FirstOrDefaultAsync(f => f.Id == filterId && f.ReportId == reportId, ct);
            if (filter is null || string.IsNullOrWhiteSpace(filter.OptionsQuery))
                return results;
            if (string.IsNullOrWhiteSpace(filter.ValueColumn) || string.IsNullOrWhiteSpace(filter.TextColumn))
                return results;

            var connectionString = GetConnectionString();
            if (connectionString is null) return results;

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = new SqlCommand(filter.OptionsQuery, conn);
            cmd.CommandTimeout = 30;
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                var value = reader[filter.ValueColumn]?.ToString();
                var text = reader[filter.TextColumn]?.ToString();
                if (value is not null)
                    results.Add(new ParameterOption { Value = value, Text = text ?? value });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get parameter options for filter {FilterId} in report {ReportId}.", filterId, reportId);
        }
        return results;
    }
}
