namespace WarehouseDashboard.Web.Pages;

/// <summary>
/// Serialisable payload returned by <c>/api/dashboard/drill/{cardId}/{level}</c> describing the
/// result of executing one drill-down level's query. Mirrors <see cref="CardDataResult"/> but adds
/// drill-specific metadata (level ordinal, display name, next-level indicator) so the Drill page
/// can render the level and decide whether a row/point can go deeper.
/// </summary>
public class DrillDataResult
{
    /// <summary>Parent card id.</summary>
    public int CardId { get; set; }

    /// <summary>Parent card title (also the breadcrumb root).</summary>
    public string CardTitle { get; set; } = string.Empty;

    /// <summary>Ordinal of the level that produced this result (starts at 1).</summary>
    public int Level { get; set; }

    /// <summary>Display name of this level (e.g. "تفاصيل المنطقة").</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Chart type to render: Bar / Line / Pie / KPI / Table / Gauge.</summary>
    public string ChartType { get; set; } = string.Empty;

    /// <summary>True when a deeper level (Level + 1) exists for this card.</summary>
    public bool HasNextLevel { get; set; }

    /// <summary>
    /// Column name in the current level's result set whose value should be passed as
    /// <c>@p0</c> to the next level. Null/empty = first column fallback.
    /// </summary>
    public string? ParameterColumn { get; set; }

    /// <summary>
    /// Column name used for human-readable labels in the breadcrumb. Null/empty =
    /// falls back to ParameterColumn.
    /// </summary>
    public string? LabelColumn { get; set; }

    /// <summary>
    /// When true, the next level requires a parent value (selected row's
    /// ParameterColumn value) to execute. Used by client to enforce row selection
    /// before navigation.
    /// </summary>
    public bool NextRequiresParentValue { get; set; }

    /// <summary>
    /// One of: <c>success</c>, <c>empty</c>, <c>error</c>, <c>none</c> (level not configured).
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

    /// <summary>First cell of the first row — used by KPI / Gauge levels.</summary>
    public object? KpiValue { get; set; }
}
