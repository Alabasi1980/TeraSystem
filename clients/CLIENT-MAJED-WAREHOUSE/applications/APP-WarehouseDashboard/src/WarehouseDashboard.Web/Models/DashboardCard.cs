namespace WarehouseDashboard.Web.Models;

/// <summary>
/// A single dashboard card configuration (chart/grid) shown on the warehouse dashboard.
/// Stored in the <c>DashboardCards</c> config table (see 06_DATA_MODEL_PREPARATION.md §1.1).
/// </summary>
public class DashboardCard
{
    /// <summary>Primary key (identity).</summary>
    public int Id { get; set; }

    /// <summary>Display title of the card (max 200 chars).</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Chart type: one of <c>Bar, Line, Pie, KPI, Table, Gauge</c> (max 50 chars).
    /// Enforced by CHECK constraint <c>CK_DashboardCards_ChartType</c>.
    /// </summary>
    public string ChartType { get; set; } = string.Empty;

    /// <summary>SQL query (or view name) returning the card's data (nvarchar(max)).</summary>
    public string SqlQuery { get; set; } = string.Empty;

    /// <summary>
    /// Source of <see cref="SqlQuery"/>: <c>SQL Query</c> or <c>View</c> (max 50 chars).
    /// Enforced by CHECK constraint <c>CK_DashboardCards_DataSourceType</c>.
    /// Default: <c>'SQL Query'</c>.
    /// </summary>
    public string DataSourceType { get; set; } = "SQL Query";

    /// <summary>Grid X position. Default 0.</summary>
    public int GridPositionX { get; set; }

    /// <summary>Grid Y position. Default 0.</summary>
    public int GridPositionY { get; set; }

    /// <summary>Grid width in columns (1-12). Default 4. CHECK: BETWEEN 1 AND 12.</summary>
    public int GridWidth { get; set; }

    /// <summary>Grid height in rows (1-6). Default 2. CHECK: BETWEEN 1 AND 6.</summary>
    public int GridHeight { get; set; }

    /// <summary>Auto-refresh interval in seconds (0 = no auto-refresh). Default 0. CHECK: >= 0.</summary>
    public int RefreshInterval { get; set; }

    /// <summary>Whether the card is shown on the dashboard (1 = visible, 0 = hidden). Default true.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Record creation timestamp (DB default GETUTCDATE()).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Record last-update timestamp (DB default GETUTCDATE()).</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Drill-down levels belonging to this card.</summary>
    public ICollection<CardDrillDownLevel> DrillDownLevels { get; set; } = new List<CardDrillDownLevel>();
}
