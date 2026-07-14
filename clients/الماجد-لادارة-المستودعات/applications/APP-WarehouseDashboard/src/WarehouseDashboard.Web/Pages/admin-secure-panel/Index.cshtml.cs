using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin Panel home — navigation hub for Cards CRUD, QueryTester, DrillDown, SyncLogs, and SyncSettings.
/// </summary>
public class IndexModel : PageModel
{
    public void OnGet()
    {
        ViewData["Title"] = "لوحة الإدارة";
    }
}
