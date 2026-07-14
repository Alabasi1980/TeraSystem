using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Sync Logs page — displays recent sync run history from the API's in-memory log store.
/// The page renders an empty Syncfusion Grid shell; JavaScript fetches
/// GET /api/sync/logs and populates it client-side.
/// </summary>
public class SyncLogsModel : PageModel
{
    public void OnGet()
    {
        ViewData["Title"] = "سجلات المزامنة";
    }
}
