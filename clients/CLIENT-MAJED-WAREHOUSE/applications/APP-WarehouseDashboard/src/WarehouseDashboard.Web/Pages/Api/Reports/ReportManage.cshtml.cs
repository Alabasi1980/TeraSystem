using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Models.Dto;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.Api.Reports;

/// <summary>
/// API endpoints for managing reports (CRUD + toggle).
/// GET  /api/reports/list        → List all reports
/// GET  /api/reports/detail/5    → Get single report with columns/filters
/// POST /api/reports/create      → Create new report
/// POST /api/reports/update/5    → Update existing report
/// POST /api/reports/delete/5    → Delete a report
/// POST /api/reports/toggle/5    → Toggle report enabled/disabled
/// </summary>
[IgnoreAntiforgeryToken]
public class ReportManageModel : PageModel
{
    private readonly ReportCrudService _crudService;

    public ReportManageModel(ReportCrudService crudService)
    {
        _crudService = crudService;
    }

    /// <summary>GET /api/reports/list — Returns all reports.</summary>
    public async Task<IActionResult> OnGetListAsync(CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        var reports = await _crudService.GetAllReportsAsync(ct);
        return new JsonResult(reports);
    }

    /// <summary>GET /api/reports/detail/{id} — Returns single report with columns/filters/layouts.</summary>
    public async Task<IActionResult> OnGetDetailAsync(int id, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        var report = await _crudService.GetReportAsync(id, ct);
        if (report is null)
            return new JsonResult(new { error = "Report not found" }) { StatusCode = 404 };
        return new JsonResult(report);
    }

    /// <summary>POST /api/reports/create — Creates a new report.</summary>
    public async Task<IActionResult> OnPostCreateAsync([FromBody] ReportCreateRequest request, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ViewName))
            return new JsonResult(new { error = "Name and ViewName are required." }) { StatusCode = 400 };

        var id = await _crudService.CreateReportAsync(request, ct);
        return new JsonResult(new { id, success = true });
    }

    /// <summary>POST /api/reports/update/{id} — Updates an existing report.</summary>
    public async Task<IActionResult> OnPostUpdateAsync(int id, [FromBody] ReportCreateRequest request, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ViewName))
            return new JsonResult(new { error = "Name and ViewName are required." }) { StatusCode = 400 };

        var success = await _crudService.UpdateReportAsync(id, request, ct);
        if (!success)
            return new JsonResult(new { error = "Report not found." }) { StatusCode = 404 };

        return new JsonResult(new { success = true });
    }

    /// <summary>POST /api/reports/delete/{id} — Deletes a report.</summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        var success = await _crudService.DeleteReportAsync(id, ct);
        if (!success)
            return new JsonResult(new { error = "Report not found." }) { StatusCode = 404 };

        return new JsonResult(new { success = true });
    }

    /// <summary>POST /api/reports/toggle/{id} — Toggles report enabled/disabled.</summary>
    public async Task<IActionResult> OnPostToggleAsync(int id, CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        var success = await _crudService.ToggleReportAsync(id, ct);
        if (!success)
            return new JsonResult(new { error = "Report not found." }) { StatusCode = 404 };

        return new JsonResult(new { success = true });
    }
}
