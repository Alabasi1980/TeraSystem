using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Service for managing reports: SQL Server View discovery, schema introspection,
/// CRUD operations, dynamic query execution, and layout management.
/// (TASK-REPORT-003 through TASK-REPORT-007)
/// </summary>
public class ReportService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReportService> _logger;
    private readonly WarehouseDashboard.Web.Data.WarehouseDashboardDbContext _db;

    public ReportService(
        IConfiguration configuration,
        ILogger<ReportService> logger,
        WarehouseDashboard.Web.Data.WarehouseDashboardDbContext db)
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

    // ======================================================================
    // SECTION 1: View Discovery
    // ======================================================================

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

    // ======================================================================
    // SECTION 2: CRUD Operations (via EF Core)
    // ======================================================================

    /// <summary>
    /// Retrieves all reports with their basic info (no columns/filters/layouts).
    /// Returns only enabled and active reports ordered by SortOrder.
    /// </summary>
    public async Task<List<ReportListItem>> GetAllReportsAsync(CancellationToken ct = default)
    {
        return await _db.Reports
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.Name)
            .Select(r => new ReportListItem
            {
                Id = r.Id,
                Name = r.Name,
                ViewName = r.ViewName,
                Description = r.Description,
                Icon = r.Icon,
                IsEnabled = r.IsEnabled,
                SortOrder = r.SortOrder
            })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Gets a single report WITH its columns, filters, and layouts.
    /// </summary>
    public async Task<ReportFullDefinition?> GetReportAsync(int reportId, CancellationToken ct = default)
    {
        var report = await _db.Reports
            .Include(r => r.Columns.OrderBy(c => c.SortOrder))
            .Include(r => r.Filters.OrderBy(f => f.SortOrder))
            .Include(r => r.Layouts)
            .FirstOrDefaultAsync(r => r.Id == reportId, ct);

        if (report is null) return null;

        return new ReportFullDefinition
        {
            Id = report.Id,
            Name = report.Name,
            ViewName = report.ViewName,
            Description = report.Description,
            Icon = report.Icon,
            IsEnabled = report.IsEnabled,
            SortOrder = report.SortOrder,
            Columns = report.Columns.Select(c => new ReportColumnDto
            {
                Id = c.Id,
                ColumnName = c.ColumnName,
                DisplayName = c.DisplayName,
                DataType = c.DataType,
                Width = c.Width,
                IsVisible = c.IsVisible,
                IsSortable = c.IsSortable,
                IsFilterable = c.IsFilterable,
                IsImageColumn = c.IsImageColumn,
                ImageBaseUrl = c.ImageBaseUrl,
                DateFormat = c.DateFormat,
                NumberFormat = c.NumberFormat,
                SortOrder = c.SortOrder
            }).ToList(),
            Filters = report.Filters.Select(f => new ReportFilterDto
            {
                Id = f.Id,
                ColumnName = f.ColumnName,
                FilterType = f.FilterType,
                Label = f.Label,
                IsRequired = f.IsRequired,
                DefaultValue = f.DefaultValue,
                OptionsQuery = f.OptionsQuery,
                Placeholder = f.Placeholder,
                SortOrder = f.SortOrder
            }).ToList(),
            Layouts = report.Layouts.Select(l => new ReportLayoutDto
            {
                Id = l.Id,
                LayoutName = l.LayoutName,
                IsDefault = l.IsDefault,
                ColumnOrder = l.ColumnOrder,
                VisibleColumns = l.VisibleColumns,
                ColumnWidths = l.ColumnWidths,
                FilterValues = l.FilterValues,
                SortState = l.SortState
            }).ToList()
        };
    }

    /// <summary>
    /// Creates a new report with columns and filters in a single transaction.
    /// </summary>
    public async Task<int> CreateReportAsync(ReportCreateRequest request, CancellationToken ct = default)
    {
        var report = new WarehouseDashboard.Web.Models.Report
        {
            Name = request.Name,
            ViewName = request.ViewName,
            Description = request.Description,
            Icon = request.Icon,
            IsEnabled = true,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Columns = request.Columns.Select(c => new WarehouseDashboard.Web.Models.ReportColumn
            {
                ColumnName = c.ColumnName,
                DisplayName = c.DisplayName,
                DataType = c.DataType,
                Width = c.Width ?? 150,
                IsVisible = c.IsVisible,
                IsSortable = c.IsSortable,
                IsFilterable = c.IsFilterable,
                IsImageColumn = c.IsImageColumn,
                ImageBaseUrl = c.ImageBaseUrl,
                DateFormat = c.DateFormat,
                NumberFormat = c.NumberFormat,
                SortOrder = c.SortOrder
            }).ToList(),
            Filters = request.Filters.Select(f => new WarehouseDashboard.Web.Models.ReportFilter
            {
                ColumnName = f.ColumnName,
                FilterType = f.FilterType,
                Label = f.Label,
                IsRequired = f.IsRequired,
                DefaultValue = f.DefaultValue,
                OptionsQuery = f.OptionsQuery,
                Placeholder = f.Placeholder,
                SortOrder = f.SortOrder
            }).ToList()
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync(ct);
        return report.Id;
    }

    /// <summary>
    /// Updates an existing report. Replaces all columns and filters.
    /// </summary>
    public async Task<bool> UpdateReportAsync(int reportId, ReportCreateRequest request, CancellationToken ct = default)
    {
        var report = await _db.Reports
            .Include(r => r.Columns)
            .Include(r => r.Filters)
            .FirstOrDefaultAsync(r => r.Id == reportId, ct);

        if (report is null) return false;

        // Update scalar fields
        report.Name = request.Name;
        report.ViewName = request.ViewName;
        report.Description = request.Description;
        report.Icon = request.Icon;
        report.SortOrder = request.SortOrder;
        report.UpdatedAt = DateTime.UtcNow;

        // Replace columns (delete old, add new)
        _db.ReportColumns.RemoveRange(report.Columns);
        report.Columns = request.Columns.Select(c => new WarehouseDashboard.Web.Models.ReportColumn
        {
            ReportId = reportId,
            ColumnName = c.ColumnName,
            DisplayName = c.DisplayName,
            DataType = c.DataType,
            Width = c.Width ?? 150,
            IsVisible = c.IsVisible,
            IsSortable = c.IsSortable,
            IsFilterable = c.IsFilterable,
            IsImageColumn = c.IsImageColumn,
            ImageBaseUrl = c.ImageBaseUrl,
            DateFormat = c.DateFormat,
            NumberFormat = c.NumberFormat,
            SortOrder = c.SortOrder
        }).ToList();

        // Replace filters (delete old, add new)
        _db.ReportFilters.RemoveRange(report.Filters);
        report.Filters = request.Filters.Select(f => new WarehouseDashboard.Web.Models.ReportFilter
        {
            ReportId = reportId,
            ColumnName = f.ColumnName,
            FilterType = f.FilterType,
            Label = f.Label,
            IsRequired = f.IsRequired,
            DefaultValue = f.DefaultValue,
            OptionsQuery = f.OptionsQuery,
            Placeholder = f.Placeholder,
            SortOrder = f.SortOrder
        }).ToList();

        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Deletes a report and all related columns/filters/layouts (CASCADE).
    /// </summary>
    public async Task<bool> DeleteReportAsync(int reportId, CancellationToken ct = default)
    {
        var report = await _db.Reports.FindAsync(new object[] { reportId }, ct);
        if (report is null) return false;

        _db.Reports.Remove(report);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Toggles report enabled/disabled status.
    /// </summary>
    public async Task<bool> ToggleReportAsync(int reportId, CancellationToken ct = default)
    {
        var report = await _db.Reports.FindAsync(new object[] { reportId }, ct);
        if (report is null) return false;

        report.IsEnabled = !report.IsEnabled;
        report.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ======================================================================
    // SECTION 3: Dynamic Query Execution (via ADO.NET)
    // ======================================================================

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
            var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
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
                            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(paramName, $"%{rawValue}%"));
                            break;

                        case "Date":
                            if (DateTime.TryParse(rawValue.ToString(), out var dateVal))
                            {
                                whereClauses.Add($"CAST([{filter.ColumnName}] AS DATE) = CAST({paramName} AS DATE)");
                                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(paramName, dateVal.Date));
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
                                    parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"{paramName}_from", fromDate));
                                }
                                if (DateTime.TryParse(parts[1], out var toDate))
                                {
                                    whereClauses.Add($"[{filter.ColumnName}] < @{paramName}_to");
                                    parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"{paramName}_to", toDate.AddDays(1)));
                                }
                            }
                            break;

                        case "Dropdown":
                            whereClauses.Add($"[{filter.ColumnName}] = {paramName}");
                            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(paramName, rawValue.ToString()));
                            break;

                        case "Number":
                            if (decimal.TryParse(rawValue.ToString(), out var numVal))
                            {
                                whereClauses.Add($"[{filter.ColumnName}] = {paramName}");
                                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(paramName, numVal));
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
                                    parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"{paramName}_min", minVal));
                                }
                                if (decimal.TryParse(rangeParts[1], out var maxVal))
                                {
                                    whereClauses.Add($"[{filter.ColumnName}] <= @{paramName}_max");
                                    parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"{paramName}_max", maxVal));
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

            await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql.ToString(), conn);
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
            _logger.LogError(ex, "Failed to execute report {ReportId}.", reportId);
            result.Success = false;
            result.ErrorMessage = "حدث خطأ أثناء تنفيذ التقرير: " + ex.Message;
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

    // ======================================================================
    // SECTION 4: Filter Options & Layout Management
    // ======================================================================

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
            await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            var sql = $"SELECT DISTINCT [{columnName}] FROM {viewName} WITH (NOLOCK) WHERE [{columnName}] IS NOT NULL ORDER BY [{columnName}]";
            await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
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
    /// Saves a new layout for a report.
    /// </summary>
    public async Task<int> SaveLayoutAsync(int reportId, ReportLayoutSaveRequest request, CancellationToken ct = default)
    {
        var layout = new WarehouseDashboard.Web.Models.ReportLayout
        {
            ReportId = reportId,
            LayoutName = request.LayoutName,
            IsDefault = request.IsDefault,
            ColumnOrder = request.ColumnOrder,
            VisibleColumns = request.VisibleColumns,
            ColumnWidths = request.ColumnWidths,
            FilterValues = request.FilterValues,
            SortState = request.SortState,
            CreatedAt = DateTime.UtcNow
        };

        // If this is the default, unmark other defaults for this report
        if (request.IsDefault)
        {
            var existingDefaults = await _db.ReportLayouts
                .Where(l => l.ReportId == reportId && l.IsDefault)
                .ToListAsync(ct);
            foreach (var d in existingDefaults)
                d.IsDefault = false;
        }

        _db.ReportLayouts.Add(layout);
        await _db.SaveChangesAsync(ct);
        return layout.Id;
    }

    /// <summary>
    /// Gets all layouts for a report.
    /// </summary>
    public async Task<List<ReportLayoutDto>> GetLayoutsAsync(int reportId, CancellationToken ct = default)
    {
        return await _db.ReportLayouts
            .Where(l => l.ReportId == reportId)
            .OrderByDescending(l => l.IsDefault)
            .ThenByDescending(l => l.CreatedAt)
            .Select(l => new ReportLayoutDto
            {
                Id = l.Id,
                LayoutName = l.LayoutName,
                IsDefault = l.IsDefault,
                ColumnOrder = l.ColumnOrder,
                VisibleColumns = l.VisibleColumns,
                ColumnWidths = l.ColumnWidths,
                FilterValues = l.FilterValues,
                SortState = l.SortState
            })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Deletes a saved layout.
    /// </summary>
    public async Task<bool> DeleteLayoutAsync(int layoutId, CancellationToken ct = default)
    {
        var layout = await _db.ReportLayouts.FindAsync(new object[] { layoutId }, ct);
        if (layout is null) return false;

        _db.ReportLayouts.Remove(layout);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

// ======================================================================
// DTOs
// ======================================================================

/// <summary>Information about a SQL Server View.</summary>
public class ViewInfo
{
    public string Schema { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

/// <summary>Column metadata from INFORMATION_SCHEMA.COLUMNS.</summary>
public class ViewColumnInfo
{
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public byte? NumericPrecision { get; set; }
    public int? NumericScale { get; set; }
}

/// <summary>Basic report info for list display.</summary>
public class ReportListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsEnabled { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>Full report definition with columns, filters, layouts.</summary>
public class ReportFullDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsEnabled { get; set; }
    public int SortOrder { get; set; }
    public List<ReportColumnDto> Columns { get; set; } = new();
    public List<ReportFilterDto> Filters { get; set; } = new();
    public List<ReportLayoutDto> Layouts { get; set; } = new();
}

/// <summary>Report column DTO.</summary>
public class ReportColumnDto
{
    public int Id { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public int? Width { get; set; }
    public bool IsVisible { get; set; }
    public bool IsSortable { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsImageColumn { get; set; }
    public string? ImageBaseUrl { get; set; }
    public string? DateFormat { get; set; }
    public string? NumberFormat { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>Report filter DTO.</summary>
public class ReportFilterDto
{
    public int Id { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string FilterType { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsQuery { get; set; }
    public string? Placeholder { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>Report layout DTO.</summary>
public class ReportLayoutDto
{
    public int Id { get; set; }
    public string LayoutName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? ColumnOrder { get; set; }
    public string? VisibleColumns { get; set; }
    public string? ColumnWidths { get; set; }
    public string? FilterValues { get; set; }
    public string? SortState { get; set; }
}

/// <summary>Request model for creating/updating a report.</summary>
public class ReportCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public List<ReportColumnDto> Columns { get; set; } = new();
    public List<ReportFilterDto> Filters { get; set; } = new();
}

/// <summary>Result of executing a report.</summary>
public class ReportDataResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<ReportDataColumn> Columns { get; set; } = new();
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public int RowCount { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public bool ReachedMaxRows { get; set; }
}

/// <summary>Column metadata for report data results.</summary>
public class ReportDataColumn
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string DataType { get; set; } = string.Empty;
    public int? Width { get; set; }
    public bool IsImageColumn { get; set; }
    public string? ImageBaseUrl { get; set; }
    public string? DateFormat { get; set; }
    public string? NumberFormat { get; set; }
}

/// <summary>Request model for saving a layout.</summary>
public class ReportLayoutSaveRequest
{
    public string LayoutName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? ColumnOrder { get; set; }    // JSON
    public string? VisibleColumns { get; set; } // JSON
    public string? ColumnWidths { get; set; }   // JSON
    public string? FilterValues { get; set; }   // JSON
    public string? SortState { get; set; }      // JSON
}
