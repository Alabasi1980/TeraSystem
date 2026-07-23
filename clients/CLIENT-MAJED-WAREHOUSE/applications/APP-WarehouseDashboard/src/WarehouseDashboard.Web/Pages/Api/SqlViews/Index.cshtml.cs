using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Pages.Api.SqlViews;

/// <summary>
/// GET /api/sqlviews — returns list of all SQL Server Views in the warehouse database.
/// Used by the Card Builder wizard Step 2 (عرض SQL (View) source type).
/// Reads from INFORMATION_SCHEMA.VIEWS to discover all user-defined views.
/// </summary>
public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var connectionString = ConnectionStringHelper.Resolve(_configuration.GetConnectionString("SqlServer"));
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return new JsonResult(Array.Empty<object>());
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT 
                    SCHEMA_NAME(schema_id) AS [Schema],
                    name AS [Name],
                    OBJECT_DEFINITION(object_id) AS [Definition],
                    create_date AS [CreatedAt],
                    modify_date AS [UpdatedAt]
                FROM sys.views
                WHERE is_ms_shipped = 0
                ORDER BY SCHEMA_NAME(schema_id), name
                """;

            var views = new List<object>();
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                views.Add(new
                {
                    schema = reader.GetString(reader.GetOrdinal("Schema")),
                    name = reader.GetString(reader.GetOrdinal("Name")),
                    definition = reader.IsDBNull(reader.GetOrdinal("Definition"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Definition")),
                    created = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    updated = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return new JsonResult(views);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list SQL Server views.");
            HttpContext.Response.StatusCode = 500;
            return new JsonResult(new { error = "Failed to read SQL Server views." });
        }
    }

    /// <summary>
    /// GET /api/sqlviews/exportexcel?schema=...&amp;name=...
    /// Exports the contents of a SQL Server view as a formatted .xlsx file.
    /// Validates schema and name to prevent SQL injection before executing the view.
    /// </summary>
    public async Task<IActionResult> OnGetExportExcelAsync(string schema, string name, CancellationToken cancellationToken)
    {
        // Validate schema and name: only alphanumeric + underscore allowed
        if (string.IsNullOrWhiteSpace(schema) || !Regex.IsMatch(schema, @"^[a-zA-Z0-9_]+$") ||
            string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, @"^[a-zA-Z0-9_]+$"))
        {
            return new JsonResult(new { success = false, errorMessage = "Invalid schema or view name." });
        }

        var connectionString = ConnectionStringHelper.Resolve(_configuration.GetConnectionString("SqlServer"));
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return new JsonResult(new { success = false, errorMessage = "Connection string is not configured." });
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);

            // Check that the view exists in INFORMATION_SCHEMA.VIEWS
            await using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = """
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS
                    WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @name
                    """;
                checkCmd.Parameters.AddWithValue("@schema", schema);
                checkCmd.Parameters.AddWithValue("@name", name);

                var count = (int)await checkCmd.ExecuteScalarAsync(cancellationToken);
                if (count == 0)
                {
                    return new JsonResult(new { success = false, errorMessage = $"View '{schema}.{name}' does not exist." });
                }
            }

            // Execute the view query — schema and name are already validated as safe
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM [{schema}].[{name}]";
            cmd.CommandTimeout = 120;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            var fieldCount = reader.FieldCount;

            // Collect column names
            var columnNames = new List<string>(fieldCount);
            for (var i = 0; i < fieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            // Build Excel workbook using ClosedXML
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(name);
            ws.RightToLeft = true;

            // Header row with blue background, white bold font, centered
            for (var i = 0; i < fieldCount; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = columnNames[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E79");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                cell.Style.Border.SetOutsideBorderColor(XLColor.FromHtml("#E0E0E0"));
            }

            // Data rows
            var rowIndex = 2;
            while (await reader.ReadAsync(cancellationToken))
            {
                for (var i = 0; i < fieldCount; i++)
                {
                    var cell = ws.Cell(rowIndex, i + 1);
                    var value = reader.GetValue(i);

                    if (value == null || value is DBNull)
                    {
                        cell.Value = string.Empty;
                    }
                    else if (value is int || value is long || value is short || value is byte)
                    {
                        cell.Value = Convert.ToInt64(value);
                        cell.Style.NumberFormat.Format = "#,##0";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    else if (value is decimal || value is double || value is float)
                    {
                        cell.Value = Convert.ToDouble(value);
                        cell.Style.NumberFormat.Format = "#,##0.00";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    else if (value is DateTime dt)
                    {
                        cell.Value = dt;
                        cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                    }
                    else
                    {
                        cell.Value = value.ToString() ?? string.Empty;
                    }

                    cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    cell.Style.Border.SetOutsideBorderColor(XLColor.FromHtml("#E0E0E0"));
                }
                rowIndex++;
            }

            // AutoFilter on all data
            if (rowIndex > 2 && fieldCount > 0)
            {
                ws.Range(1, 1, rowIndex - 1, fieldCount).SetAutoFilter();
            }

            // AutoFit column widths
            ws.Columns().AdjustToContents(1, 100);

            // Freeze header row
            ws.SheetView.FreezeRows(1);

            // Return file
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"{name}_{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export view {Schema}.{Name}.", schema, name);
            return new JsonResult(new { success = false, errorMessage = "Failed to export view data." });
        }
    }
}
