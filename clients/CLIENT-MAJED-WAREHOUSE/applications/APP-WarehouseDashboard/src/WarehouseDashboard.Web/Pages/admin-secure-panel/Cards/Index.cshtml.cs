using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Cards;

/// <summary>
/// Lists all <see cref="DashboardCard"/> configurations in a data grid and
/// exposes POST handlers for Delete and Clone actions.
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

    /// <summary>Card ID → number of configured Drill Down levels.</summary>
    public Dictionary<int, int> CardDrillLevelCounts { get; set; } = new();

    public string? ToastMessage { get; set; }
    public string? ToastType { get; set; }

    public async Task OnGetAsync()
    {
        Cards = await _db.DashboardCards
            .OrderBy(c => c.GridPositionY)
            .ThenBy(c => c.GridPositionX)
            .Select(c => new CardRow(
                c.Id, c.Title, c.Description, c.ChartType, c.DataSourceType,
                c.IsActive, c.ColorPalette, c.GridWidth, c.GridHeight,
                c.RefreshInterval, c.DateFilterMode ?? "dashboard", c.KpiMode ?? "simple"))
            .ToListAsync();

        // Load Drill Down level counts per card
        var cardIds = Cards.Select(c => c.Id).ToList();
        CardDrillLevelCounts = await _db.CardDrillDownLevels
            .Where(d => cardIds.Contains(d.ParentCardId))
            .GroupBy(d => d.ParentCardId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

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

    /// <summary>
    /// Clones an existing card and redirects to the Card Builder wizard with pre-filled data.
    /// </summary>
    public async Task<IActionResult> OnPostCloneAsync(int id)
    {
        var card = await _db.DashboardCards
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card is null)
        {
            TempData["ToastMessage"] = "البطاقة غير موجودة أو سبق حذفها.";
            TempData["ToastType"] = "error";
            return RedirectToPage();
        }

        // Redirect to Builder with clone parameter
        return RedirectToPage("/admin-secure-panel/Cards/Builder", new { clone = id });
    }
}

/// <summary>Row view-model for the cards grid.</summary>
public record CardRow(
    int Id,
    string Title,
    string? Description,
    string ChartType,
    string DataSourceType,
    bool IsActive,
    string ColorPalette,
    int GridWidth,
    int GridHeight,
    int RefreshInterval,
    string DateFilterMode,
    string KpiMode);
