using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.DataExports;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;

    public IndexModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>Base URL for the Sync API, passed to client-side JS.</summary>
    public string SyncApiBaseUrl { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        SyncApiBaseUrl = _configuration["SyncApi:BaseUrl"] ?? _configuration["SyncApiBaseUrl"] ?? string.Empty;
        return Page();
    }
}
