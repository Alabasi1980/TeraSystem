using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using DashboardModel = WarehouseDashboard.Web.Models.Dashboard;

namespace WarehouseDashboard.Web.Pages;

/// <summary>
/// Public dashboard main page ("/" or "/{slug}"). Renders a responsive 12-column CSS grid of
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

    /// <summary>URL-friendly slug bound from the route.</summary>
    [BindProperty(SupportsGet = true)]
    public string? Slug { get; set; }

    /// <summary>The currently active dashboard resolved from <see cref="Slug"/>.</summary>
    public DashboardModel? CurrentDashboard { get; set; }

    /// <summary>All active dashboards ordered by SortOrder, used by the tab bar.</summary>
    public List<DashboardModel> AllDashboards { get; set; } = new();

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

    public async Task<IActionResult> OnGetAsync()
    {
        SyncApiBaseUrl = _configuration["SyncApi:BaseUrl"] ?? _configuration["SyncApiBaseUrl"] ?? string.Empty;

        try
        {
            // Load all active dashboards for the tab bar.
            AllDashboards = await _db.Dashboards
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .ToListAsync();

            // Resolve the current dashboard from slug or default.
            if (!string.IsNullOrWhiteSpace(Slug))
            {
                CurrentDashboard = AllDashboards
                    .FirstOrDefault(d => string.Equals(d.Slug, Slug.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (CurrentDashboard is null)
            {
                CurrentDashboard = AllDashboards.FirstOrDefault(d => d.IsDefault)
                                   ?? AllDashboards.FirstOrDefault();

                // If a slug was provided but no match found, redirect to the resolved default.
                if (!string.IsNullOrWhiteSpace(Slug) && CurrentDashboard is not null)
                {
                    var redirectSlug = string.IsNullOrEmpty(CurrentDashboard.Slug)
                        ? string.Empty
                        : CurrentDashboard.Slug;
                    return RedirectToPage(new { slug = redirectSlug });
                }
            }

            // Load cards filtered by the current dashboard.
            var dashboardId = CurrentDashboard?.Id;

            Cards = await _db.DashboardCards
                .Where(c => c.IsActive && c.DashboardId == dashboardId)
                .OrderBy(c => c.GridPositionY)
                .ThenBy(c => c.GridPositionX)
                .Select(c => new CardLayoutInfo(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.ChartType,
                    c.ColorPalette,
                    c.GridPositionX,
                    c.GridPositionY,
                    c.GridWidth,
                    c.GridHeight,
                    c.RefreshInterval,
                    c.DateFilterMode ?? "dashboard",
                    c.FixedStartDate ?? "",
                    c.FixedEndDate ?? "",
                    c.RelativeDays,
                    c.AssistantEnabled))
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

        return Page();
    }

    /// <summary>
    /// AJAX handler to persist dashboard card layout (position + size).
    /// Called by SortableJS onEnd and resize button clicks from the frontend.
    /// </summary>
    public async Task<IActionResult> OnPostSaveLayoutAsync([FromBody] SaveLayoutRequest request)
    {
        try
        {
            if (request?.Cards == null || request.Cards.Count == 0)
            {
                return new JsonResult(new { success = false, error = "No cards provided." });
            }

            // Load all affected cards in a single query
            var cardIds = request.Cards.Select(c => c.CardId).ToList();
            var cards = await _db.DashboardCards
                .Where(c => cardIds.Contains(c.Id))
                .ToListAsync();

            var cardDict = cards.ToDictionary(c => c.Id);

            foreach (var item in request.Cards)
            {
                if (!cardDict.TryGetValue(item.CardId, out var card)) continue;

                card.GridPositionX = Math.Clamp(item.GridPositionX, 1, 12);
                card.GridPositionY = Math.Clamp(item.GridPositionY, 1, 100);
                card.GridWidth = Math.Clamp(item.GridWidth, 1, 12);
                card.GridHeight = Math.Clamp(item.GridHeight, 1, 6);
                card.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Dashboard layout saved: {Count} cards updated.", cards.Count);

            return new JsonResult(new { success = true, updated = cards.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save dashboard layout.");
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
}

/// <summary>Lightweight projection of a card's layout metadata for the grid.</summary>
public record CardLayoutInfo(
    int Id,
    string Title,
    string Description,
    string ChartType,
    string ColorPalette,
    int GridPositionX,
    int GridPositionY,
    int GridWidth,
    int GridHeight,
    int RefreshInterval,
    string DateFilterMode,
    string FixedStartDate,
    string FixedEndDate,
    int RelativeDays,
    bool AssistantEnabled);

// === Layout persistence models (TASK-DASH-005) ===

/// <summary>Request body for the SaveLayout AJAX endpoint.</summary>
public class SaveLayoutRequest
{
    /// <summary>The dashboard being updated (currently unused, but reserved for multi-dashboard support).</summary>
    public int DashboardId { get; set; }

    /// <summary>Ordered list of card layout items matching the new grid order.</summary>
    public List<CardLayoutItem> Cards { get; set; } = new();
}

/// <summary>A single card's new position and size.</summary>
public class CardLayoutItem
{
    /// <summary>Primary key of the card to update.</summary>
    public int CardId { get; set; }

    /// <summary>Column position (1-based, 1–12).</summary>
    public int GridPositionX { get; set; }

    /// <summary>Row position (1-based, ascending by visual order).</summary>
    public int GridPositionY { get; set; }

    /// <summary>Column span (1–12).</summary>
    public int GridWidth { get; set; }

    /// <summary>Row span (1–6).</summary>
    public int GridHeight { get; set; }
}
