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

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        // Note: we still need the DbContext to check if the card exists and to pass
        // it to the service. DashboardService already holds its own DbContext reference.
        // We fetch the card through DashboardService's data layer.
        var result = await _dashboardService.GetCardDataByIdAsync(id, cancellationToken);
        return new JsonResult(result);
    }
}
