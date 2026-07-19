using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseDashboard.Web.Services;
using DashboardModel = WarehouseDashboard.Web.Models.Dashboard;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Dashboards;

/// <summary>
/// Create a new dashboard. Handles form validation and slug auto-generation.
/// </summary>
public class CreateModel : PageModel
{
    private readonly DashboardManageService _dashboardService;

    public CreateModel(DashboardManageService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [BindProperty]
    public DashboardInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        // Defaults
        Input.IsActive = true;
        Input.SortOrder = await GetNextSortOrderAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var dashboard = new DashboardModel
        {
            Name = Input.Name,
            Slug = Input.Slug,
            Description = Input.Description,
            Icon = string.IsNullOrWhiteSpace(Input.Icon) ? "📊" : Input.Icon,
            SortOrder = Input.SortOrder,
            IsActive = Input.IsActive,
            IsDefault = Input.IsDefault
        };

        var (success, error) = await _dashboardService.CreateAsync(dashboard);

        if (!success)
        {
            ErrorMessage = error;
            return Page();
        }

        TempData["ToastMessage"] = "تم إنشاء الداشبورد بنجاح.";
        TempData["ToastType"] = "success";
        return RedirectToPage("Index");
    }

    private async Task<int> GetNextSortOrderAsync()
    {
        var dashboards = await _dashboardService.GetAllAsync();
        return dashboards.Count > 0 ? dashboards.Max(d => d.SortOrder) + 1 : 0;
    }
}

/// <summary>Input model for Create/Edit dashboard forms.</summary>
public class DashboardInput
{
    [Required(ErrorMessage = "اسم الداشبورد مطلوب.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "اسم الداشبورد يجب أن يكون بين 1 و 200 حرف.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "المعرف (Slug) مطلوب.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "المعرف (Slug) يجب أن يكون بين 1 و 200 حرف.")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف.")]
    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = "📊";

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDefault { get; set; }
}
