using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Data;

namespace WarehouseDashboard.Web.Pages.Api.Dashboard;

/// <summary>
/// GET /api/dashboard/card/{id}
/// Executes a single active card's data query via the approved DashboardService
/// pattern (config = EF, data = ADO.NET) and returns a resilient
/// <see cref="CardDataResult"/>. This is what the public dashboard fetches
/// per card (driving skeleton → chart/empty/error hydration).
/// </summary>
public class CardModel : PageModel
{
    private readonly DashboardService _dashboardService;

    public CardModel(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> OnGetAsync(int id, string? preset, string? dateFrom, string? dateTo, CancellationToken cancellationToken)
    {
        var dateRange = ResolvePresetDates(preset, dateFrom, dateTo);
        var result = await _dashboardService.GetCardDataByIdAsync(id, dateRange, cancellationToken);
        return new JsonResult(result);
    }

    /// <summary>
    /// Converts a dashboard filter preset string into a <see cref="DashboardService.DateRange"/>.
    /// Supports named presets (today, yesterday, 7days, 30days, month) and
    /// a "custom" preset with explicit dateFrom/dateTo strings.
    /// </summary>
    private static DashboardService.DateRange? ResolvePresetDates(string? preset, string? dateFrom = null, string? dateTo = null)
    {
        if (string.IsNullOrWhiteSpace(preset))
            return null;

        // Handle custom preset with explicit date range
        if (string.Equals(preset, "custom", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(dateFrom) && !string.IsNullOrWhiteSpace(dateTo)
                && DateTime.TryParse(dateFrom, out var from) && DateTime.TryParse(dateTo, out var to))
            {
                // Include full end-of-day for the To date
                return new DashboardService.DateRange(from, to.AddDays(1).AddTicks(-1));
            }
            return null;
        }

        var today = DateTime.Today;

        return preset.ToLowerInvariant() switch
        {
            "today" => new DashboardService.DateRange(today, today.AddDays(1).AddTicks(-1)),
            "yesterday" => new DashboardService.DateRange(today.AddDays(-1), today.AddTicks(-1)),
            "7days" => new DashboardService.DateRange(today.AddDays(-6), today.AddDays(1).AddTicks(-1)),
            "30days" => new DashboardService.DateRange(today.AddDays(-29), today.AddDays(1).AddTicks(-1)),
            "month" => new DashboardService.DateRange(new DateTime(today.Year, today.Month, 1), today.AddDays(1).AddTicks(-1)),
            "lastmonth" => new DashboardService.DateRange(
                new DateTime(today.Year, today.Month, 1).AddMonths(-1),
                new DateTime(today.Year, today.Month, 1).AddTicks(-1)
            ),
            _ => null
        };
    }
}
