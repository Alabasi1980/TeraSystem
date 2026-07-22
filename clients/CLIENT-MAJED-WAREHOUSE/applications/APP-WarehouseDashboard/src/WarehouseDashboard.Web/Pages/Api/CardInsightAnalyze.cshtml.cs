using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.Api;

/// <summary>
/// POST /api/card-insights/analyze
/// Accepts { cardId, mode, depthLevel } and returns AI analysis with depth metadata.
/// Called by the front-end Side Panel via AJAX.
/// </summary>
[IgnoreAntiforgeryToken]
public class CardInsightAnalyzeModel : PageModel
{
    private readonly CardInsightService _cardInsightService;
    private readonly CardSummaryBuilderFactory _builderFactory;
    private readonly WarehouseDashboardDbContext _db;
    private readonly ILogger<CardInsightAnalyzeModel> _logger;

    public CardInsightAnalyzeModel(
        CardInsightService cardInsightService,
        CardSummaryBuilderFactory builderFactory,
        WarehouseDashboardDbContext db,
        ILogger<CardInsightAnalyzeModel> logger)
    {
        _cardInsightService = cardInsightService;
        _builderFactory = builderFactory;
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Read JSON body manually — [FromBody] is unreliable in Razor Pages handler methods.
        AnalyzeRequest? request;
        using (var reader = new StreamReader(Request.Body))
        {
            var body = await reader.ReadToEndAsync();
            try
            {
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                request = JsonSerializer.Deserialize<AnalyzeRequest>(body, jsonOptions);
            }
            catch
            {
                request = null;
            }
        }

        if (request is null)
        {
            return new JsonResult(new CardInsightResponse
            {
                Success = false,
                ErrorMessage = "الطلب غير صالح: البيانات مفقودة."
            });
        }

        // 1. Load card
        var card = await _db.DashboardCards.FindAsync(new object[] { request.CardId });
        if (card is null)
        {
            return new JsonResult(new CardInsightResponse
            {
                Success = false,
                ErrorMessage = "البطاقة غير موجودة."
            });
        }

        if (!card.AssistantEnabled)
        {
            return new JsonResult(new CardInsightResponse
            {
                Success = false,
                ErrorMessage = "المساعد غير مفعّل لهذه البطاقة."
            });
        }

        // 2b. Load drill levels for side panel display
        var drillLevels = await _db.Set<CardDrillDownLevel>()
            .Where(d => d.ParentCardId == card.Id)
            .OrderBy(d => d.Level)
            .Select(d => new DrillLevelInfo
            {
                Level = d.Level,
                DisplayName = d.DisplayName,
                TargetChartType = d.TargetChartType
            })
            .ToListAsync();

        // 3. Build summary for depth metadata
        CardSummary? summary = null;
        try
        {
            var builder = _builderFactory.GetBuilder(card.ChartType);
            summary = await builder.BuildSummaryAsync(card, request.DepthLevel);
        }
        catch (Exception ex)
        {
            // Builder failed (e.g., SQL error) — still call AI with basic card info.
            _logger.LogWarning(ex,
                "CardSummary builder failed for card {CardId} (type {ChartType}). " +
                "Falling back to basic analysis with default depth metadata.",
                card.Id, card.ChartType);
        }

        bool isFullDataReached = summary?.IsFullDataReached ?? false;
        bool hasDeeperData = summary?.HasDeeperData ?? false;
        string depthLabel = summary?.DepthLabel ?? GetDepthLabel(request.DepthLevel);

        // 4. Call AI with caching (pass summary for rich data analysis)
        var result = await _cardInsightService.AnalyzeCardWithCacheAsync(
            cardId: card.Id,
            mode: request.Mode,
            depthLevel: request.DepthLevel,
            dataScopeLabel: depthLabel,
            isFullDataReached: isFullDataReached,
            hasDeeperData: hasDeeperData,
            summary: summary);

        result.AvailableDrillLevels = drillLevels;
        result.HasDateColumn = !string.IsNullOrEmpty(card.DateColumn);

        return new JsonResult(result);
    }

    /// <summary>
    /// Returns a human-readable Arabic label for the given depth level.
    /// Used as a fallback when the builder doesn't provide one.
    /// </summary>
    private static string GetDepthLabel(int depthLevel) => depthLevel switch
    {
        1 => "آخر 3 أشهر",
        2 => "آخر 6 أشهر",
        3 => "آخر سنة",
        4 => "آخر 3 سنوات",
        5 => "آخر 5 سنوات",
        6 => "آخر 10 سنوات / كل البيانات المتاحة",
        _ => "النطاق الافتراضي"
    };

}
