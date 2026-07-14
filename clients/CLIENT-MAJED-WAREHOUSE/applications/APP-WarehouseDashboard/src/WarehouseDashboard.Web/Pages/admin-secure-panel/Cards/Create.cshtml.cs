using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Cards;

/// <summary>
/// Create a new <see cref="DashboardCard"/>. Validates via <see cref="CardEditorInput"/>
/// (mirrors the table CHECK constraints) then persists through <see cref="WarehouseDashboardDbContext"/>.
/// </summary>
public class CreateModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;

    public CreateModel(WarehouseDashboardDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public CardEditorInput Input { get; set; } = new();

    public List<SelectOption> ChartTypeOptions => CardEditorInput.ChartTypeOptions;
    public List<SelectOption> DataSourceTypeOptions => CardEditorInput.DataSourceTypeOptions;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var card = new DashboardCard
        {
            Title = Input.Title.Trim(),
            ChartType = Input.ChartType,
            DataSourceType = Input.DataSourceType,
            SqlQuery = Input.SqlQuery.Trim(),
            GridPositionX = Input.GridPositionX,
            GridPositionY = Input.GridPositionY,
            GridWidth = Input.GridWidth,
            GridHeight = Input.GridHeight,
            RefreshInterval = Input.RefreshInterval,
            IsActive = Input.IsActive
            // CreatedAt / UpdatedAt are handled by DB defaults (GETUTCDATE()).
        };

        _db.DashboardCards.Add(card);
        await _db.SaveChangesAsync();

        TempData["ToastMessage"] = "تمت إضافة البطاقة بنجاح.";
        TempData["ToastType"] = "success";
        return RedirectToPage("Index");
    }
}
