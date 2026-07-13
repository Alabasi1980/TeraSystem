using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;

namespace WarehouseDashboard.Web.Pages;

/// <summary>
/// Public dashboard main page ("/"). Renders a responsive 12-column CSS grid of
/// cards configured in <c>DashboardCards</c>. Each card's data is fetched
/// independently by the client from <c>/api/dashboard/card/{id}</c> (which uses the
/// approved DashboardService pattern), so a failing card shows its own state and
/// never breaks the page. No admin auth — Viewers are public.
/// </summary>
public class IndexModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(WarehouseDashboardDbContext db, IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>Layout metadata for each active card (positions + sizes).</summary>
    public List<CardLayoutInfo> Cards { get; set; } = new();

    /// <summary>
    /// Ids of active cards that have at least one configured drill-down level. Used by the
    /// client to mark a card as clickable (navigates to its Drill page). Cards without levels
    /// are not interactive.
    /// </summary>
    public HashSet<int> CardsWithDrill { get; set; } = new();

    /// <summary>
    /// Set when the configuration database itself is unreachable, so the page can
    /// show a friendly banner instead of a hard 500. Card data failures are handled
    /// per card on the client.
    /// </summary>
    public bool ConfigError { get; set; }

    /// <summary>Base URL for the Sync API (read from SyncApi:BaseUrl config).</summary>
    public string SyncApiBaseUrl { get; private set; } = string.Empty;

    public async Task OnGetAsync()
    {
        SyncApiBaseUrl = _configuration["SyncApi:BaseUrl"] ?? "https://localhost:5001";

        try
        {
            Cards = await _db.DashboardCards
                .Where(c => c.IsActive)
                .OrderBy(c => c.GridPositionY)
                .ThenBy(c => c.GridPositionX)
                .Select(c => new CardLayoutInfo(
                    c.Id, c.Title, c.ChartType, c.GridPositionX, c.GridPositionY,
                    c.GridWidth, c.GridHeight, c.RefreshInterval))
                .ToListAsync();

            CardsWithDrill = new HashSet<int>(
                await _db.CardDrillDownLevels
                    .Where(l => l.ParentCardId != 0)
                    .Select(l => l.ParentCardId)
                    .Distinct()
                    .ToListAsync());
        }
        catch (Exception ex)
        {
            // Config store unavailable — render an empty, friendly dashboard.
            _logger.LogWarning(ex, "Failed to load dashboard config.");
            ConfigError = true;
            Cards = new List<CardLayoutInfo>();
        }
    }
}

/// <summary>Lightweight projection of a card's layout metadata for the grid.</summary>
public record CardLayoutInfo(
    int Id,
    string Title,
    string ChartType,
    int GridPositionX,
    int GridPositionY,
    int GridWidth,
    int GridHeight,
    int RefreshInterval);
