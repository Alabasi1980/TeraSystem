using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;

namespace WarehouseDashboard.Web.Pages.Dashboard;

/// <summary>
/// Public drill-down page: <c>/Dashboard/Drill/{cardId}</c>.
///
/// The server only loads the card title and the ordered list of drill levels (config via EF Core)
/// and passes them to the client. Each level's data is fetched independently by the client from
/// <c>/api/dashboard/drill/{cardId}/{level}?parentValue=...</c>, so a failing level shows its own
/// state and never breaks the page. No admin auth — Viewers are public.
///
/// Deeper levels are reached by appending <c>?level=N&amp;pv=CHAIN</c> to the URL, where
/// <c>pv</c> is a pipe-separated chain of the first-column values clicked on the way down. This
/// drives both breadcrumb back-navigation and the <c>@p0</c> value sent to the API for the level.
/// </summary>
public class DrillModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ILogger<DrillModel> _logger;

    public DrillModel(WarehouseDashboardDbContext db, ILogger<DrillModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Card id from the route.</summary>
    public int CardId { get; set; }

    /// <summary>Card title (breadcrumb root).</summary>
    public string CardTitle { get; set; } = string.Empty;

    /// <summary>Ordered drill levels for the card (metadata only — no data queries here).</summary>
    public List<DrillLevelInfo> Levels { get; set; } = new();

    /// <summary>
    /// Set when the configuration database itself is unreachable, so the page can show a friendly
    /// banner instead of a hard 500. Level data failures are handled per level on the client.
    /// </summary>
    public bool ConfigError { get; set; }

    public async Task OnGetAsync(int cardId)
    {
        CardId = cardId;

        try
        {
            var card = await _db.DashboardCards.FindAsync(new object[] { cardId });
            if (card is null)
            {
                // Render an empty shell; the client will show a friendly "not found" message.
                CardTitle = string.Empty;
                Levels = new List<DrillLevelInfo>();
                return;
            }

            CardTitle = card.Title;

            Levels = await _db.CardDrillDownLevels
                .Where(l => l.ParentCardId == cardId)
                .OrderBy(l => l.Level)
                .Select(l => new DrillLevelInfo(l.Level, l.DisplayName, l.TargetChartType))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load drill-down config for card {CardId}.", cardId);
            ConfigError = true;
            CardTitle = string.Empty;
            Levels = new List<DrillLevelInfo>();
        }
    }
}

/// <summary>Lightweight projection of a drill level's metadata for breadcrumb + navigation.</summary>
public record DrillLevelInfo(int Level, string DisplayName, string ChartType);
