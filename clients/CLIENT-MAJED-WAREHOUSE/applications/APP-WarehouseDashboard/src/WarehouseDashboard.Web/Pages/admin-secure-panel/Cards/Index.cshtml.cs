using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Cards;

/// <summary>
/// Lists all <see cref="DashboardCard"/> configurations in a Syncfusion Grid and
/// exposes a POST delete handler (cascades to CardDrillDownLevels).
/// Reachable only when authenticated (AdminAuthMiddleware).
/// </summary>
public class IndexModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;

    public IndexModel(WarehouseDashboardDbContext db)
    {
        _db = db;
    }

    /// <summary>Lightweight projection sent to the grid (avoids serializing nav props).</summary>
    public List<CardRow> Cards { get; set; } = new();

    public string? ToastMessage { get; set; }
    public string? ToastType { get; set; }

    public async Task OnGetAsync()
    {
        Cards = await _db.DashboardCards
            .OrderBy(c => c.GridPositionY)
            .ThenBy(c => c.GridPositionX)
            .Select(c => new CardRow(
                c.Id, c.Title, c.ChartType, c.DataSourceType, c.IsActive,
                c.GridPositionX, c.GridPositionY, c.GridWidth, c.GridHeight, c.RefreshInterval))
            .ToListAsync();

        if (TempData["ToastMessage"] is string message)
        {
            ToastMessage = message;
            ToastType = TempData["ToastType"] as string ?? "success";
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var card = await _db.DashboardCards.FindAsync(id);
        if (card is null)
        {
            TempData["ToastMessage"] = "البطاقة غير موجودة أو سبق حذفها.";
            TempData["ToastType"] = "error";
            return RedirectToPage();
        }

        // EF cascade removes the related CardDrillDownLevels (configured in DbContext).
        _db.DashboardCards.Remove(card);
        await _db.SaveChangesAsync();

        TempData["ToastMessage"] = "تم حذف البطاقة بنجاح.";
        TempData["ToastType"] = "success";
        return RedirectToPage();
    }
}

/// <summary>Row view-model for the cards grid.</summary>
public record CardRow(
    int Id,
    string Title,
    string ChartType,
    string DataSourceType,
    bool IsActive,
    int GridPositionX,
    int GridPositionY,
    int GridWidth,
    int GridHeight,
    int RefreshInterval);
