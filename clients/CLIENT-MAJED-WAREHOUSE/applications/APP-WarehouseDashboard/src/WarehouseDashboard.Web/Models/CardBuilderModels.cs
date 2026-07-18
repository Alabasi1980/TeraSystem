using System.ComponentModel.DataAnnotations;

namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Request model for building a DashboardCard from the Card Builder wizard.
/// Contains all wizard step data needed to construct a card configuration.
/// </summary>
public class CardBuilderRequest
{
    /// <summary>Display title of the card.</summary>
    [Required(ErrorMessage = "الرجاء إدخال العنوان.")]
    [StringLength(200, ErrorMessage = "العنوان طويل جداً (الحد الأقصى 200 حرفاً).")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Chart type: Bar, Line, Pie, KPI, Table, Gauge.</summary>
    [Required(ErrorMessage = "الرجاء اختيار نوع الرسم.")]
    public string ChartType { get; set; } = "Bar";

    /// <summary>Data source type: "SQL Query" or "View".</summary>
    [Required(ErrorMessage = "الرجاء اختيار مصدر البيانات.")]
    public string DataSourceType { get; set; } = "SQL Query";

    /// <summary>SQL query or View name (depending on DataSourceType).</summary>
    [Required(ErrorMessage = "الرجاء إدخال استعلام SQL أو اسم العرض.")]
    public string SqlQuery { get; set; } = string.Empty;

    /// <summary>Grid X position (0-11).</summary>
    public int GridPositionX { get; set; }

    /// <summary>Grid Y position (0-5).</summary>
    public int GridPositionY { get; set; }

    /// <summary>Grid width in columns (1-12).</summary>
    [Range(1, 12, ErrorMessage = "العرض يجب أن يكون بين 1 و 12.")]
    public int GridWidth { get; set; } = 4;

    /// <summary>Grid height in rows (1-6).</summary>
    [Range(1, 6, ErrorMessage = "الارتفاع يجب أن يكون بين 1 و 6.")]
    public int GridHeight { get; set; } = 2;

    /// <summary>Auto-refresh interval in seconds (0 = disabled).</summary>
    [Range(0, int.MaxValue, ErrorMessage = "الفاصل الزمني يجب أن يكون صفراً أو أكثر.")]
    public int RefreshInterval { get; set; }

    /// <summary>Whether the card is active/when the card is visible on the dashboard.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Optional: ID of an existing card to clone from.</summary>
    public int? CloneFromCardId { get; set; }

    /// <summary>Optional: Template ID if built from a template.</summary>
    public string? TemplateId { get; set; }

    /// <summary>Drill-down levels configuration (optional).</summary>
    public List<CardDrillDownInput> DrillDownLevels { get; set; } = new();

    // === Advanced KPI Fields (Step 4) ===
    public string ValueColumn { get; set; } = string.Empty;
    public string DateColumn { get; set; } = string.Empty;
    public string CategoryColumn { get; set; } = string.Empty;
    public string KpiMode { get; set; } = "simple";
    public bool ShowChange { get; set; } = false;
    public string ChangeSource { get; set; } = "previousPeriod";
    public bool ShowSparkline { get; set; } = false;
    public int SparklineMonths { get; set; } = 6;
    public bool ShowGrandTotal { get; set; } = false;
    public string GrandTotalSource { get; set; } = "sameTable";
    public string DateFilterMode { get; set; } = "dashboard";
    public string FixedStartDate { get; set; } = string.Empty;
    public string FixedEndDate { get; set; } = string.Empty;
    public int RelativeDays { get; set; } = 30;

    /// <summary>Aggregation method for KPI ValueColumn: Sum, Count, Avg, Min, Max, None.</summary>
    public string AggregationType { get; set; } = "Sum";
}

/// <summary>
/// Input model for a single drill-down level in the builder.
/// </summary>
public class CardDrillDownInput
{
    /// <summary>Ordinal level (1-based).</summary>
    public int Level { get; set; }

    /// <summary>Display name for this drill-down level.</summary>
    [Required]
    [StringLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>SQL query for this drill-down level (must accept @p0 parameter).</summary>
    [Required]
    public string DrillDownQuery { get; set; } = string.Empty;

    /// <summary>Chart type for rendering this level.</summary>
    [Required]
    public string TargetChartType { get; set; } = "Bar";
}

/// <summary>
/// Request model for the live preview API.
/// Contains minimal data needed to execute a preview query and return chart config + sample data.
/// </summary>
public class CardPreviewRequest
{
    /// <summary>Chart type: Bar, Line, Pie, KPI, Table, Gauge.</summary>
    [Required]
    public string ChartType { get; set; } = string.Empty;

    /// <summary>Data source type: "SQL Query" or "View".</summary>
    [Required]
    public string DataSourceType { get; set; } = "SQL Query";

    /// <summary>SQL query or View name.</summary>
    [Required]
    public string SqlQuery { get; set; } = string.Empty;

    /// <summary>Optional: Oracle source table name (for template variable substitution).</summary>
    public string? SqlSource { get; set; }

    /// <summary>Optional: Value field name for chart series.</summary>
    public string? ValueField { get; set; }

    /// <summary>Optional: Category field name for chart X-axis.</summary>
    public string? CategoryField { get; set; }

    /// <summary>Optional: Additional options (template variables, etc.).</summary>
    public Dictionary<string, object?>? Options { get; set; }

    /// <summary>Optional: Limit rows for preview (default 10).</summary>
    [Range(1, 100)]
    public int PreviewRowLimit { get; set; } = 10;
}

/// <summary>
/// Result returned by the Preview API containing chart configuration and sample data.
/// </summary>
public class CardPreviewResult
{
    /// <summary>Chart type as sent in the request.</summary>
    public string ChartType { get; set; } = string.Empty;

    /// <summary>Chart configuration object (series, axes, legend, etc.).</summary>
    public object? ChartConfig { get; set; }

    /// <summary>Sample data rows (limited to PreviewRowLimit).</summary>
    public List<Dictionary<string, object?>> SampleData { get; set; } = new();

    /// <summary>Column names from the query result.</summary>
    public List<string> Columns { get; set; } = new();

    /// <summary>Status: "success", "empty", "error".</summary>
    public string Status { get; set; } = "error";

    /// <summary>Error message if Status == "error".</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>First cell value (for KPI/Gauge preview).</summary>
    public object? KpiValue { get; set; }

    /// <summary>Execution time in milliseconds.</summary>
    public long ExecutionTimeMs { get; set; }
}

/// <summary>
/// Predefined card template for the builder wizard.
/// </summary>
public class CardTemplate
{
    /// <summary>Unique template identifier (e.g., "total-stock", "sales-trend").</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Display name in Arabic.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description in Arabic.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Chart type for this template.</summary>
    public string ChartType { get; set; } = "Bar";

    /// <summary>Data source type: "SQL Query" or "View".</summary>
    public string DataSourceType { get; set; } = "SQL Query";

    /// <summary>Pre-built SQL query with placeholders like {TableName}.</summary>
    public string SqlQueryTemplate { get; set; } = string.Empty;

    /// <summary>Default grid width (1-12).</summary>
    public int DefaultGridWidth { get; set; } = 4;

    /// <summary>Default grid height (1-6).</summary>
    public int DefaultGridHeight { get; set; } = 2;

    /// <summary>Default refresh interval in seconds.</summary>
    public int DefaultRefreshInterval { get; set; } = 0;

    /// <summary>Required Oracle source table name (for Step 2 table selection).</summary>
    public string? RequiredOracleSource { get; set; }

    /// <summary>Suggested drill-down levels for this template.</summary>
    public List<CardDrillDownInput> SuggestedDrillDowns { get; set; } = new();
}

/// <summary>
/// Dropdown option.
/// </summary>
public record SelectOption(string Value, string Text);