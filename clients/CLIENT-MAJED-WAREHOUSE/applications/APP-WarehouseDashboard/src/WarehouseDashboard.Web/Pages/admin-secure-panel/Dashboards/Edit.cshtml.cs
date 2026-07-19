using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Dashboards;

/// <summary>
/// Edit an existing dashboard. Loads entity, validates, and updates.
/// Shows card count and prevents removing the last default.
/// </summary>
public class EditModel : PageModel
{
    private readonly DashboardManageService _dashboardService;

    public EditModel(DashboardManageService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [BindProperty]
    public DashboardInput Input { get; set; } = new();

    public int CardCount { get; set; }
    public bool WasDefault { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var dashboard = await _dashboardService.GetByIdAsync(id);
        if (dashboard is null)
        {
            TempData["ToastMessage"] = "الداشبورد غير موجود.";
            TempData["ToastType"] = "error";
            return RedirectToPage("Index");
        }

        // Card count via query
        var allWithCount = await _dashboardService.GetAllWithCardCountAsync();
        var match = allWithCount.FirstOrDefault(x => x.Dashboard.Id == id);
        CardCount = match.CardCount;

        Input = new DashboardInput
        {
            Name = dashboard.Name,
            Slug = dashboard.Slug,
            Description = dashboard.Description,
            Icon = dashboard.Icon,
            SortOrder = dashboard.SortOrder,
            IsActive = dashboard.IsActive,
            IsDefault = dashboard.IsDefault
        };

        WasDefault = dashboard.IsDefault;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var dashboard = await _dashboardService.GetByIdAsync(id);
        if (dashboard is null)
        {
            TempData["ToastMessage"] = "الداشبورد غير موجود.";
            TempData["ToastType"] = "error";
            return RedirectToPage("Index");
        }

        // Refresh card count for re-display
        var allWithCount = await _dashboardService.GetAllWithCardCountAsync();
        var match = allWithCount.FirstOrDefault(x => x.Dashboard.Id == id);
        CardCount = match.CardCount;
        WasDefault = dashboard.IsDefault;

        if (!ModelState.IsValid)
            return Page();

        dashboard.Name = Input.Name;
        dashboard.Slug = Input.Slug;
        dashboard.Description = Input.Description;
        dashboard.Icon = string.IsNullOrWhiteSpace(Input.Icon) ? "\U0001F4CA" : Input.Icon;
        dashboard.SortOrder = Input.SortOrder;
        dashboard.IsActive = Input.IsActive;
        dashboard.IsDefault = Input.IsDefault;

        var (success, error) = await _dashboardService.UpdateAsync(dashboard);

        if (!success)
        {
            ErrorMessage = error;
            return Page();
        }

        TempData["ToastMessage"] = "تم حفظ التغييرات بنجاح.";
        TempData["ToastType"] = "success";
        return RedirectToPage("Index");
    }
}
