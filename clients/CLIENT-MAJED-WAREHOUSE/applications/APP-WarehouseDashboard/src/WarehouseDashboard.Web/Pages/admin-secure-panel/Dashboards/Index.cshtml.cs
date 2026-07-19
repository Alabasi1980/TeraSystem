using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseDashboard.Web.Services;
using DashboardEntity = WarehouseDashboard.Web.Models.Dashboard;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Dashboards;

/// <summary>
/// Lists all dashboards with card counts.
/// Supports POST handlers for Delete and Reorder actions.
/// </summary>
public class IndexModel : PageModel
{
    private readonly DashboardManageService _dashboardService;

    public IndexModel(DashboardManageService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public List<(DashboardEntity Dashboard, int CardCount)> Dashboards { get; set; } = new();

    public string? ToastMessage { get; set; }
    public string? ToastType { get; set; }

    public async Task OnGetAsync()
    {
        Dashboards = await _dashboardService.GetAllWithCardCountAsync();

        if (TempData["ToastMessage"] is string message)
        {
            ToastMessage = message;
            ToastType = TempData["ToastType"] as string ?? "success";
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var (success, error) = await _dashboardService.DeleteAsync(id);

        if (!success)
        {
            TempData["ToastMessage"] = error ?? "حدث خطأ أثناء الحذف.";
            TempData["ToastType"] = "error";
        }
        else
        {
            TempData["ToastMessage"] = "تم حذف الداشبورد بنجاح.";
            TempData["ToastType"] = "success";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReorderAsync(string orderedIds)
    {
        if (string.IsNullOrWhiteSpace(orderedIds))
            return RedirectToPage();

        var ids = orderedIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        await _dashboardService.ReorderAsync(ids);

        TempData["ToastMessage"] = "تم تحديث ترتيب الداشبوردات.";
        TempData["ToastType"] = "success";
        return RedirectToPage();
    }
}
