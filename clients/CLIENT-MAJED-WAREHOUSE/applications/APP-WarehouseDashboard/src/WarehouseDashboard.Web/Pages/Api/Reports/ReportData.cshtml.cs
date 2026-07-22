using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models.Dto;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.Api.Reports;

/// <summary>
/// API endpoints for report data operations.
/// GET  /api/reports-data/views              → List available SQL Server Views
/// GET  /api/reports-data/columns            → Get columns for a view (?view=dbo.MyView)
/// POST /api/reports-data/execute            → Execute a report with filters
/// GET  /api/reports-data/options            → Get filter dropdown options (?view=X&column=Y)
/// GET  /api/reports-data/layouts            → Get layouts for a report (?reportId=5)
/// POST /api/reports-data/saveLayout         → Save a layout
/// POST /api/reports-data/deleteLayout       → Delete a layout
/// </summary>
[IgnoreAntiforgeryToken]
public class ReportDataModel : PageModel
{
    private readonly ReportViewService _viewService;
    private readonly ReportExecutionService _executionService;
    private readonly ReportLayoutService _layoutService;
    private readonly ILogger<ReportDataModel> _logger;

    public ReportDataModel(
        ReportViewService viewService,
        ReportExecutionService executionService,
        ReportLayoutService layoutService,
        ILogger<ReportDataModel> logger)
    {
        _viewService = viewService;
        _executionService = executionService;
        _layoutService = layoutService;
        _logger = logger;
    }

    /// <summary>GET /api/reports-data/views — List all available SQL Server Views.</summary>
    public async Task<IActionResult> OnGetViewsAsync(CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        var views = await _viewService.GetAvailableViewsAsync(ct);
        return new JsonResult(views);
    }

    /// <summary>
    /// GET /api/reports-data/columns?view=dbo.MyView
    /// Returns column metadata for a given view.
    /// </summary>
    public async Task<IActionResult> OnGetColumnsAsync([FromQuery] string view, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(view))
            return new JsonResult(new { error = "View name is required." }) { StatusCode = 400 };

        var columns = await _viewService.GetViewColumnsAsync(view, ct);
        return new JsonResult(columns);
    }

    /// <summary>
    /// POST /api/reports-data/execute
    /// Body: { reportId: 5, filterValues: { "ColumnName": "value" } }
    /// Returns report data with columns metadata + rows.
    /// </summary>
    public async Task<IActionResult> OnPostExecuteAsync([FromBody] ReportExecuteRequest request, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (request is null || request.ReportId <= 0)
            return new JsonResult(new { success = false, errorMessage = "ReportId is required." });

        var result = await _executionService.ExecuteReportAsync(request.ReportId, request.FilterValues, ct);
        return new JsonResult(result);
    }

    /// <summary>
    /// GET /api/reports-data/options?view=dbo.MyView&column=Category
    /// Returns distinct values for a column (Dropdown filter options).
    /// </summary>
    public async Task<IActionResult> OnGetOptionsAsync([FromQuery] string view, [FromQuery] string column, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(view) || string.IsNullOrWhiteSpace(column))
            return new JsonResult(new { error = "View and column are required." }) { StatusCode = 400 };

        var options = await _executionService.GetFilterOptionsAsync(view, column, ct);
        return new JsonResult(options);
    }

    /// <summary>
    /// GET /api/reports-data/parameterOptions?reportId=5&amp;filterId=3
    /// Returns parameter options for a filter using its configured OptionsQuery.
    /// </summary>
    public async Task<IActionResult> OnGetParameterOptionsAsync(
        [FromQuery] int reportId,
        [FromQuery] int filterId,
        CancellationToken ct)
    {
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }
        if (reportId <= 0 || filterId <= 0)
            return new JsonResult(new { error = "ReportId and FilterId are required." }) { StatusCode = 400 };

        var options = await _executionService.GetParameterOptionsAsync(reportId, filterId, ct);
        return new JsonResult(options);
    }

    /// <summary>GET /api/reports-data/layouts?reportId=5 — Get all layouts for a report.</summary>
    public async Task<IActionResult> OnGetLayoutsAsync([FromQuery] int reportId, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (reportId <= 0)
            return new JsonResult(new { error = "ReportId is required." }) { StatusCode = 400 };

        var layouts = await _layoutService.GetLayoutsAsync(reportId, ct);
        return new JsonResult(layouts);
    }

    /// <summary>
    /// POST /api/reports-data/saveLayout
    /// Body: { reportId: 5, layoutName: "...", columnOrder: "...", ... }
    /// </summary>
    public async Task<IActionResult> OnPostSaveLayoutAsync([FromBody] ReportLayoutApiRequest request, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (request is null || request.ReportId <= 0 || string.IsNullOrWhiteSpace(request.LayoutName))
            return new JsonResult(new { error = "ReportId and LayoutName are required." }) { StatusCode = 400 };

        var saveRequest = new ReportLayoutSaveRequest
        {
            LayoutName = request.LayoutName,
            IsDefault = request.IsDefault,
            ColumnOrder = request.ColumnOrder,
            VisibleColumns = request.VisibleColumns,
            ColumnWidths = request.ColumnWidths,
            FilterValues = request.FilterValues,
            SortState = request.SortState
        };

        var id = await _layoutService.SaveLayoutAsync(request.ReportId, saveRequest, ct);
        return new JsonResult(new { id, success = true });
    }

    /// <summary>
    /// POST /api/reports-data/deleteLayout
    /// Body: { layoutId: 5 }
    /// </summary>
    public async Task<IActionResult> OnPostDeleteLayoutAsync([FromBody] LayoutDeleteRequest request, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (request is null || request.LayoutId <= 0)
            return new JsonResult(new { error = "LayoutId is required." }) { StatusCode = 400 };

        var success = await _layoutService.DeleteLayoutAsync(request.LayoutId, ct);
        if (!success)
            return new JsonResult(new { error = "Layout not found." }) { StatusCode = 404 };

        return new JsonResult(new { success = true });
    }

    /// <summary>
    /// POST /api/reports-data/preview
    /// Body: { viewName: "dbo.MyView", top: 10 }
    /// Returns first N rows from a View for preview purposes.
    /// </summary>
    public async Task<IActionResult> OnPostPreviewAsync([FromBody] PreviewRequest request, CancellationToken ct)
    {
        // Admin session check
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(request.ViewName))
        {
            return new JsonResult(new { success = false, errorMessage = "ViewName is required." });
        }

        try
        {
            var top = request.Top > 0 ? Math.Min(request.Top, 20) : 10;

            // Get column metadata using existing method
            var columns = await _viewService.GetViewColumnsAsync(request.ViewName, ct);
            if (columns.Count == 0)
            {
                return new JsonResult(new { success = false, errorMessage = "View not found or has no columns." });
            }

            // Execute SELECT TOP N
            var connectionString = ConnectionStringHelper.ResolveSql(
                HttpContext.RequestServices.GetRequiredService<IConfiguration>());
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return new JsonResult(new { success = false, errorMessage = "Connection string not configured." });
            }

            // RB-SEC-002: Validate view exists before executing preview
            var viewExists = await _viewService.ValidateViewExistsAsync(request.ViewName, ct);
            if (!viewExists)
            {
                return new JsonResult(new { success = false, errorMessage = "الـ View المحدد غير موجود." });
            }

            var sql = $"SELECT TOP {top} * FROM {request.ViewName} WITH (NOLOCK)";

            await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
            cmd.CommandTimeout = 15;
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var colMeta = new List<object>();
            var rows = new List<Dictionary<string, object?>>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                colMeta.Add(new { name = reader.GetName(i), dataType = reader.GetFieldType(i).Name });
            }

            while (await reader.ReadAsync(ct))
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var val = reader.GetValue(i);
                    row[reader.GetName(i)] = val is DBNull ? null : val;
                }
                rows.Add(row);
            }

            return new JsonResult(new
            {
                success = true,
                columns = colMeta,
                rows,
                rowCount = rows.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in preview for view {ViewName}", request.ViewName);
            return new JsonResult(new { success = false, errorMessage = "حدث خطأ أثناء معالجة الطلب. الرجاء المحاولة مرة أخرى." });
        }
    }

    /// <summary>
    /// POST /api/reports-data/executeQuery
    /// Body: { query: "SELECT ..." }
    /// Executes a read-only SELECT query and returns up to 20 rows for testing/preview.
    /// Used by the Report Builder to test parameter queries.
    /// RB-SEC-001: Restricted to SELECT-only queries with read-only safety.
    /// </summary>
    public async Task<IActionResult> OnPostExecuteQueryAsync([FromBody] ExecuteQueryRequest request, CancellationToken ct)
    {
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(request.Query))
            return new JsonResult(new { error = "Query is required." }) { StatusCode = 400 };

        // RB-SEC-001: Validate query is SELECT-only
        var (isValid, errorMessage) = ValidateAndSanitizeQuery(request.Query);
        if (!isValid)
        {
            return new JsonResult(new { success = false, errorMessage });
        }

        try
        {
            var connectionString = ConnectionStringHelper.ResolveSql(
                HttpContext.RequestServices.GetRequiredService<IConfiguration>());
            if (string.IsNullOrWhiteSpace(connectionString))
                return new JsonResult(new { success = false, errorMessage = "Connection string not configured." });

            // RB-SEC-001: Log the query for audit trail
            _logger.LogWarning("ExecuteQuery being run: {Query}", request.Query);

            await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            // RB-SEC-001: Set read-uncommitted isolation level for safety
            await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " + request.Query, conn);
            cmd.CommandTimeout = 30;
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));

            var rows = new List<Dictionary<string, object?>>();
            int count = 0;
            while (await reader.ReadAsync(ct) && count < 20)
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var val = reader.GetValue(i);
                    row[columns[i]] = val is DBNull ? null : val;
                }
                rows.Add(row);
                count++;
            }

            return new JsonResult(new { success = true, columns, rows, rowCount = rows.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing ad-hoc query");
            return new JsonResult(new { success = false, errorMessage = "حدث خطأ أثناء معالجة الطلب. الرجاء المحاولة مرة أخرى." });
        }
    }

    /// <summary>
    /// RB-SEC-001: Validates that a query is SELECT-only and does not contain forbidden keywords.
    /// </summary>
    private static (bool IsValid, string? ErrorMessage) ValidateAndSanitizeQuery(string query)
    {
        var trimmed = query.Trim();
        var trimmedUpper = trimmed.ToUpperInvariant();

        // Must start with SELECT
        if (!trimmedUpper.StartsWith("SELECT"))
        {
            return (false, "يُسمح فقط باستعلامات SELECT.");
        }

        // Forbidden keywords that could modify data or schema
        var forbidden = new[] { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "EXEC", "EXECUTE", "TRUNCATE", "MERGE", "GRANT", "REVOKE" };

        foreach (var keyword in forbidden)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmedUpper, $@"\b{keyword}\b"))
            {
                return (false, "يُسمح فقط باستعلامات SELECT.");
            }
        }

        return (true, null);
    }

    // ======================================================================
    // Request DTOs
    // ======================================================================

    public class ReportExecuteRequest
    {
        public int ReportId { get; set; }
        public Dictionary<string, object?>? FilterValues { get; set; }
    }

    public class ReportLayoutApiRequest
    {
        public int ReportId { get; set; }
        public string LayoutName { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string? ColumnOrder { get; set; }
        public string? VisibleColumns { get; set; }
        public string? ColumnWidths { get; set; }
        public string? FilterValues { get; set; }
        public string? SortState { get; set; }
    }

    public class LayoutDeleteRequest
    {
        public int LayoutId { get; set; }
    }

    public class PreviewRequest
    {
        public string ViewName { get; set; } = string.Empty;
        public int Top { get; set; } = 10;
    }

    public class ExecuteQueryRequest
    {
        public string Query { get; set; } = string.Empty;
    }
}
