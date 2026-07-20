using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.Api.OracleBrowser;

/// <summary>
/// GET /api/oracle-browser/list-objects?type=table|view
/// Returns a JSON array of <see cref="OracleObject"/> for the wizard's Step 2.
/// </summary>
[IgnoreAntiforgeryToken]
public class ListObjectsModel : PageModel
{
    private readonly OracleSchemaService _oracleSchema;
    private readonly ILogger<ListObjectsModel> _logger;

    public ListObjectsModel(OracleSchemaService oracleSchema, ILogger<ListObjectsModel> logger)
    {
        _oracleSchema = oracleSchema;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(
        [FromQuery] string type = "table",
        CancellationToken ct = default)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        try
        {
            var objects = type.Equals("view", StringComparison.OrdinalIgnoreCase)
                ? await _oracleSchema.ListOracleViewsAsync(ct)
                : await _oracleSchema.ListOracleTablesAsync(ct);

            return new JsonResult(objects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list Oracle objects (type={Type}).", type);
            return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }
}
