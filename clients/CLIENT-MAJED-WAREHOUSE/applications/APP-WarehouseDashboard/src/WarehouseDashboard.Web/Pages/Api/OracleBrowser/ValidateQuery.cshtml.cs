using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.Api.OracleBrowser;

/// <summary>
/// POST /api/oracle-browser/validate-query
/// Body: { query }
/// Returns a JSON <see cref="QueryValidationResult"/> for the wizard's Step 2 (Query mode).
/// </summary>
[IgnoreAntiforgeryToken]
public class ValidateQueryModel : PageModel
{
    private readonly OracleSchemaService _oracleSchema;
    private readonly ILogger<ValidateQueryModel> _logger;

    public ValidateQueryModel(OracleSchemaService oracleSchema, ILogger<ValidateQueryModel> logger)
    {
        _oracleSchema = oracleSchema;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync(
        [FromBody] ValidateQueryRequest request,
        CancellationToken ct)
    {
        // Admin session check
        if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
        {
            return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        }

        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return new JsonResult(new QueryValidationResult
            {
                IsValid = false,
                ErrorMessage = "Query is required."
            });
        }

        try
        {
            var result = await _oracleSchema.ValidateQueryAsync(request.Query, ct);
            return new JsonResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate query.");
            return new JsonResult(new QueryValidationResult
            {
                IsValid = false,
                ErrorMessage = ex.Message
            }) { StatusCode = 500 };
        }
    }

    public class ValidateQueryRequest
    {
        public string Query { get; set; } = "";
    }
}
