using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin Panel home. Reachable only when authenticated (enforced by AdminAuthMiddleware).
/// Dashboard card CRUD UI is delivered in a later task.
/// </summary>
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
