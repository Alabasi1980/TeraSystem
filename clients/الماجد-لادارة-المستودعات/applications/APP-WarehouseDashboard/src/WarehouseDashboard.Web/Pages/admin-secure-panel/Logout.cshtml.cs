using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Logout: clears the admin session flag and redirects to the Login page.
/// Reachable without authentication (excluded from AdminAuthMiddleware).
/// </summary>
public class LogoutModel : PageModel
{
    private const string SessionKey = "AdminAuthenticated";

    public IActionResult OnGet()
    {
        HttpContext.Session.Remove(SessionKey);
        HttpContext.Session.Clear();
        return RedirectToPage("/admin-secure-panel/Login");
    }
}
