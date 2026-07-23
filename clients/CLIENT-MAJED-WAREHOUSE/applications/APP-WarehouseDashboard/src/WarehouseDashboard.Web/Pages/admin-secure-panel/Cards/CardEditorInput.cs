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

    // Advanced KPI: Column Mappings
    public string ValueColumn { get; set; } = string.Empty;
    public string DateColumn { get; set; } = string.Empty;
    public string CategoryColumn { get; set; } = string.Empty;

    // Advanced KPI: Mode & Change
    public string KpiMode { get; set; } = "simple";
    public bool ShowChange { get; set; } = false;
    public string ChangeSource { get; set; } = "previousPeriod";

    // Advanced KPI: Sparkline
    public bool ShowSparkline { get; set; } = false;
    public int SparklineMonths { get; set; } = 6;

    // Advanced KPI: Grand Total
    public bool ShowGrandTotal { get; set; } = false;
    public string GrandTotalSource { get; set; } = "sameTable";

    // Advanced KPI: Date Filter
    public string DateFilterMode { get; set; } = "dashboard";
    public string FixedStartDate { get; set; } = string.Empty;
    public string FixedEndDate { get; set; } = string.Empty;
    public int RelativeDays { get; set; } = 30;

    // Advanced KPI: Aggregation Type
    public string AggregationType { get; set; } = "Sum";

    // Value Format Settings (TASK-BUILDER-BEH-002)
    public string ValueFormatType { get; set; } = "Currency";
    public string ValueUnit { get; set; } = "";

    public static readonly string[] AllowedAggregationTypes = { "Sum", "Count", "Avg", "Min", "Max", "None" };

    // === Builder Original Source Type ===

    /// <summary>
    /// Original SourceType from Step 2 of the Card Builder: "Template", "SqlView", "SqlTable", "CustomSQL".
    /// Default: "SqlTable". This is the value of the source dropdown at card creation.
    /// </summary>
    public string OriginalSourceType { get; set; } = "SqlTable";

    /// <summary>
    /// Source-specific identifier: table name for SqlTable, view name for SqlView, template ID for Template.
    /// Empty for CustomSQL. Used to reconstruct the Step 2 selection when editing.
    /// </summary>
    public string OriginalSourceId { get; set; } = "";

    public static readonly string[] AllowedOriginalSourceTypes = { "Template", "SqlView", "SqlTable", "CustomSQL" };

    public static List<SelectOption> AggregationTypeOptions =>
        AllowedAggregationTypes.Select(x => new SelectOption(x, x)).ToList();

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

/// <summary>Simple value/text pair for dropdown data sources.</summary>
public record SelectOption(string Value, string Text);
