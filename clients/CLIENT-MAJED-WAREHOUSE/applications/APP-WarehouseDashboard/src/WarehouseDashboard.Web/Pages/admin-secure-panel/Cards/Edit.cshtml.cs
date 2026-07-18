using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Cards;

/// <summary>
/// Edit an existing <see cref="DashboardCard"/>. Loads the entity into
/// <see cref="CardEditorInput"/>, validates, then updates via DbContext.
/// </summary>
public class EditModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;

    public EditModel(WarehouseDashboardDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public CardEditorInput Input { get; set; } = new();

    public List<SelectOption> ChartTypeOptions => CardEditorInput.ChartTypeOptions;
    public List<SelectOption> DataSourceTypeOptions => CardEditorInput.DataSourceTypeOptions;
    public List<SelectOption> AggregationTypeOptions => CardEditorInput.AggregationTypeOptions;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var card = await _db.DashboardCards.FindAsync(id);
        if (card is null)
        {
            return NotFound();
        }

        Input = new CardEditorInput
        {
            Title = card.Title,
            ChartType = card.ChartType,
            DataSourceType = card.DataSourceType,
            SqlQuery = card.SqlQuery,
            GridPositionX = card.GridPositionX,
            GridPositionY = card.GridPositionY,
            GridWidth = card.GridWidth,
            GridHeight = card.GridHeight,
            RefreshInterval = card.RefreshInterval,
            IsActive = card.IsActive,
            AggregationType = card.AggregationType ?? "Sum"
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var card = await _db.DashboardCards.FindAsync(id);
        if (card is null)
        {
            TempData["ToastMessage"] = "البطاقة غير موجودة أو سبق حذفها.";
            TempData["ToastType"] = "error";
            return RedirectToPage("Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        card.Title = Input.Title.Trim();
        card.ChartType = Input.ChartType;
        card.DataSourceType = Input.DataSourceType;
        card.SqlQuery = Input.SqlQuery.Trim();
        card.GridPositionX = Input.GridPositionX;
        card.GridPositionY = Input.GridPositionY;
        card.GridWidth = Input.GridWidth;
        card.GridHeight = Input.GridHeight;
        card.RefreshInterval = Input.RefreshInterval;
        card.IsActive = Input.IsActive;
        card.AggregationType = Input.AggregationType ?? "Sum";
        // UpdatedAt is handled by the DB default (GETUTCDATE()) on update.

        await _db.SaveChangesAsync();

        TempData["ToastMessage"] = "تم حفظ التغييرات بنجاح.";
        TempData["ToastType"] = "success";
        return RedirectToPage("Index");
    }
}
