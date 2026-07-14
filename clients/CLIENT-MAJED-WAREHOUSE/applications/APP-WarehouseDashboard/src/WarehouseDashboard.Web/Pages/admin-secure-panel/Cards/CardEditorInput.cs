using System.ComponentModel.DataAnnotations;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Cards;

/// <summary>
/// Bound input model for the Create/Edit card editor.
/// Server-side validation mirrors the CHECK constraints defined on the
/// <c>DashboardCards</c> table (see 06_DATA_MODEL_PREPARATION.md §1.1):
///   - ChartType / DataSourceType must be from the allowed sets
///   - GridWidth BETWEEN 1 AND 12
///   - GridHeight BETWEEN 1 AND 6
///   - RefreshInterval >= 0
/// The database also enforces these; this class provides friendly, early errors.
/// </summary>
public class CardEditorInput : IValidatableObject
{
    public static readonly string[] AllowedChartTypes = { "Bar", "Line", "Pie", "KPI", "Table", "Gauge" };
    public static readonly string[] AllowedDataSourceTypes = { "SQL Query", "View" };

    public static List<SelectOption> ChartTypeOptions =>
        AllowedChartTypes.Select(x => new SelectOption(x, x)).ToList();

    public static List<SelectOption> DataSourceTypeOptions =>
        AllowedDataSourceTypes.Select(x => new SelectOption(x, x)).ToList();

    [Required(ErrorMessage = "الرجاء إدخال العنوان.")]
    [StringLength(200, ErrorMessage = "العنوان طويل جداً (الحد الأقصى 200 حرفاً).")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "الرجاء اختيار نوع الرسم.")]
    public string ChartType { get; set; } = "Bar";

    [Required(ErrorMessage = "الرجاء اختيار مصدر البيانات.")]
    public string DataSourceType { get; set; } = "SQL Query";

    [Required(ErrorMessage = "الرجاء إدخال استعلام SQL أو اسم العرض.")]
    public string SqlQuery { get; set; } = string.Empty;

    public int GridPositionX { get; set; }

    public int GridPositionY { get; set; }

    [Range(1, 12, ErrorMessage = "العرض يجب أن يكون بين 1 و 12.")]
    public int GridWidth { get; set; } = 4;

    [Range(1, 6, ErrorMessage = "الارتفاع يجب أن يكون بين 1 و 6.")]
    public int GridHeight { get; set; } = 2;

    [Range(0, int.MaxValue, ErrorMessage = "الفاصل الزمني يجب أن يكون صفراً أو أكثر.")]
    public int RefreshInterval { get; set; }

    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!AllowedChartTypes.Contains(ChartType))
        {
            yield return new ValidationResult(
                "نوع الرسم البياني غير صالح. القيم المسموحة: Bar, Line, Pie, KPI, Table, Gauge.",
                new[] { nameof(ChartType) });
        }

        if (!AllowedDataSourceTypes.Contains(DataSourceType))
        {
            yield return new ValidationResult(
                "مصدر البيانات غير صالح. القيم المسموحة: SQL Query, View.",
                new[] { nameof(DataSourceType) });
        }
    }
}

/// <summary>Simple value/text pair for Syncfusion dropdown data sources.</summary>
public record SelectOption(string Value, string Text);
