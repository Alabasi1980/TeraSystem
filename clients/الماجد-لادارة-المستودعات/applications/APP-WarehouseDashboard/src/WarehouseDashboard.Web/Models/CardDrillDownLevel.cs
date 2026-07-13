namespace WarehouseDashboard.Web.Models;

/// <summary>
/// A drill-down level for a dashboard card. Each level defines a deeper query and the
/// chart type used to render it. Stored in the <c>CardDrillDownLevels</c> config table
/// (see 06_DATA_MODEL_PREPARATION.md §1.2).
/// </summary>
public class CardDrillDownLevel
{
    /// <summary>Primary key (identity).</summary>
    public int Id { get; set; }

    /// <summary>Parent card id (FK → DashboardCards.Id). Cascade delete.</summary>
    public int ParentCardId { get; set; }

    /// <summary>Ordinal of this drill-down level (starts at 1). Default 1. CHECK: >= 1.</summary>
    public int Level { get; set; }

    /// <summary>Display name shown to the user for this level (max 200 chars).</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>SQL query for this drill-down level (nvarchar(max)). Must accept at least one parameter.</summary>
    public string DrillDownQuery { get; set; } = string.Empty;

    /// <summary>
    /// Chart type used to render this level: <c>Bar, Line, Pie, KPI, Table, Gauge</c> (max 50 chars).
    /// Enforced by CHECK constraint <c>CK_CardDrillDownLevels_TargetChartType</c>.
    /// </summary>
    public string TargetChartType { get; set; } = string.Empty;

    /// <summary>Navigation to the owning dashboard card.</summary>
    public DashboardCard Card { get; set; } = null!;
}
