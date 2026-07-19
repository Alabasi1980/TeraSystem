namespace WarehouseDashboard.Web.Pages;

/// <summary>
/// Serialisable payload returned by <c>/api/dashboard/card/{id}</c> describing the
/// result of executing a single card's query. The public dashboard hydrates one
/// widget from this per card — and NEVER lets one failing card break
/// the whole page (each card carries its own status).
/// </summary>
public class CardDataResult
{
    /// <summary>Card id (matches <see cref="Models.DashboardCard.Id"/>).</summary>
    public int CardId { get; set; }

    /// <summary>Chart type: Bar / Line / Pie / KPI / Table / Gauge.</summary>
    public string ChartType { get; set; } = string.Empty;

    /// <summary>Display title (also shown in the card header).</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// One of: <c>success</c>, <c>empty</c>, <c>error</c>.
    /// The client renders the appropriate state for each value.
    /// </summary>
    public string Status { get; set; } = "error";

    /// <summary>Localized, safe error message (secrets stripped) when <see cref="Status"/> == error.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Result column names (schema), in query order.</summary>
    public List<string> Columns { get; set; } = new();

    /// <summary>
    /// Result rows. Each row maps column name → cell value. Values are already
    /// converted to JSON-friendly types (numbers/strings/bool/null).
    /// </summary>
    public List<Dictionary<string, object?>> Rows { get; set; } = new();

    /// <summary>First cell of the first row — used by KPI / Gauge cards.</summary>
    public object? KpiValue { get; set; }

    // === Advanced KPI Properties ===

    /// <summary>Main KPI numeric value (extracted from ValueColumn).</summary>
    public object? KpiMainValue { get; set; }

    /// <summary>Change percentage from previous period (e.g., 12.5 for +12.5%).</summary>
    public decimal? KpiChangePercent { get; set; }

    /// <summary>Change direction: "up", "down", or "flat".</summary>
    public string KpiChangeDirection { get; set; } = "flat";

    /// <summary>Configured KPI comparison source, used only for non-sensitive display context.</summary>
    public string? ChangeSource { get; set; }

    /// <summary>Sparkline data points (monthly values for trend chart).</summary>
    public List<Dictionary<string, object?>>? KpiSparklineData { get; set; }

    /// <summary>Grand total value (all-time, no date filter).</summary>
    public object? KpiGrandTotal { get; set; }

    /// <summary>Top 5 category breakdown with values and percentages.</summary>
    public List<Dictionary<string, object?>>? KpiCategoryBreakdown { get; set; }

    /// <summary>KPI display mode from card config: "simple", "withChange", "composite".</summary>
    public string KpiMode { get; set; } = "simple";
}
