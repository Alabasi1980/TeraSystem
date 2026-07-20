using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.Api.OracleBrowser;

/// <summary>
/// POST /api/oracle-browser/preview
/// Body: { source, sourceType, limit }
/// Returns a JSON <see cref="DataPreviewResult"/> for the wizard's Step 3.
/// </summary>
[IgnoreAntiforgeryToken]
public class PreviewModel : PageModel
{
    private readonly OracleSchemaService _oracleSchema;
    private readonly ILogger<PreviewModel> _logger;

    public PreviewModel(OracleSchemaService oracleSchema, ILogger<PreviewModel> logger)
    {
        _oracleSchema = oracleSchema;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync(
        [FromBody] PreviewRequest request,
        CancellationToken ct)
    {
        // Admin session check — skip if AdminAuth:Bypass is enabled
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("AdminAuth:Bypass") &&
            HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(request.Source))
        {
            return new JsonResult(new { error = "Source is required." }) { StatusCode = 400 };
        }

        try
        {
            var limit = request.Limit > 0 ? Math.Min(request.Limit, 100) : 10;
            var result = await _oracleSchema.PreviewDataAsync(
                request.Source,
                request.SourceType ?? "Table",
                limit,
                ct);

            return new JsonResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to preview data for '{Source}'.", request.Source);
            return new JsonResult(new DataPreviewResult { ErrorMessage = ex.Message }) { StatusCode = 500 };
        }
    }

    public class PreviewRequest
    {
        public string Source { get; set; } = "";
        public string SourceType { get; set; } = "Table";
        public int Limit { get; set; } = 10;
    }
}
