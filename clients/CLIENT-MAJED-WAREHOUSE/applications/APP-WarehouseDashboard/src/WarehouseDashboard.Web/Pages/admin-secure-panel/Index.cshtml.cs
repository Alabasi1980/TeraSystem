using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin Panel home — navigation hub for Cards CRUD, QueryTester, DrillDown, SyncLogs, and SyncSettings.
/// Shows live count badges on each nav card.
/// </summary>
public class IndexModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;

    public IndexModel(WarehouseDashboardDbContext db)
    {
        _db = db;
    }

    public int CardsCount { get; set; }
    public int DashboardsCount { get; set; }
    public int MappingsCount { get; set; }
    public int SyncLogsCount { get; set; }
    public int DrillLevelsCount { get; set; }

    public async Task OnGetAsync()
    {
        ViewData["Title"] = "لوحة الإدارة";
        DashboardsCount = await _db.Dashboards.CountAsync();
        CardsCount = await _db.DashboardCards.CountAsync();
        MappingsCount = await _db.TableMappings.CountAsync();
        DrillLevelsCount = await _db.CardDrillDownLevels.CountAsync();
        // SyncLogsCount: no EF Core entity for SyncLogs yet (in-memory store in API project), defaults to 0.
    }
}
