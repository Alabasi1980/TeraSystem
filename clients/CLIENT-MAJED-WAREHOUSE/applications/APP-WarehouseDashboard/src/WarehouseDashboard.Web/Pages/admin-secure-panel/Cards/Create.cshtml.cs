using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Cards;

/// <summary>
/// Redirects to the new Card Builder wizard page.
/// The old Create page is deprecated in favor of the multi-step builder.
/// </summary>
public class CreateModel : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/admin-secure-panel/Cards/Builder");
    }
}
