using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseDashboard.Web.Models;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.Api.Dashboard;

/// <summary>
/// POST /api/dashboard/cardbuilder/preview
/// Generates a live preview for the Card Builder wizard.
/// Accepts CardPreviewRequest, returns CardPreviewResult with chart config + sample data.
/// </summary>
[IgnoreAntiforgeryToken]
public class CardBuilderModel : PageModel
{
    private readonly CardBuilderService _cardBuilderService;

    public CardBuilderModel(CardBuilderService cardBuilderService)
    {
        _cardBuilderService = cardBuilderService;
    }

    /// <summary>
    /// Generates preview data for the Card Builder.
    /// </summary>
    /// <param name="request">Preview request with chart type, query, and options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>CardPreviewResult with chart config and sample data.</returns>
    public async Task<IActionResult> OnPostPreviewAsync(
        [FromBody] CardPreviewRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new CardPreviewResult
            {
                Status = "error",
                ErrorMessage = "طلب غير صالح: البيانات مفقودة."
            });
        }

        var result = await _cardBuilderService.GetPreviewAsync(request, cancellationToken);
        return new JsonResult(result);
    }

    /// <summary>
    /// Gets a card template by ID.
    /// </summary>
    /// <param name="id">Template ID.</param>
    /// <returns>CardTemplate or 404.</returns>
    public IActionResult OnGetTemplate(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new { error = "معرف القالب مطلوب." });
        }

        var template = _cardBuilderService.GetTemplate(id);
        if (template is null)
        {
            return NotFound(new { error = $"القالب '{id}' غير موجود." });
        }

        return new JsonResult(template);
    }

    /// <summary>
    /// Gets all available card templates.
    /// </summary>
    public IActionResult OnGetTemplates()
    {
        var templates = _cardBuilderService.GetAllTemplates();
        return new JsonResult(templates);
    }
}
