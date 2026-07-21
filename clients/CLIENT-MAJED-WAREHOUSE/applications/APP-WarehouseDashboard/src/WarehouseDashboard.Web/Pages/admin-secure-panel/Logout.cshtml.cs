using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Logout: clears the admin session flag and renders the logout page.
/// Automatically redirects to the Login page after a configurable delay.
/// Reachable without authentication (excluded from AdminAuthMiddleware).
/// </summary>
public class LogoutModel : PageModel
{
    private const string SessionKey = "AdminAuthenticated";

    /// <summary>
    /// Number of seconds before automatic redirect to the login page.
    /// </summary>
    public int RedirectDelaySeconds { get; set; } = 4;

    /// <summary>
    /// URL to redirect to after the countdown finishes.
    /// </summary>
    public string RedirectUrl { get; set; } = "/admin-secure-panel/Login";

    public IActionResult OnGet()
    {
        HttpContext.Session.Remove(SessionKey);
        HttpContext.Session.Clear();
        return Page();
    }
}
