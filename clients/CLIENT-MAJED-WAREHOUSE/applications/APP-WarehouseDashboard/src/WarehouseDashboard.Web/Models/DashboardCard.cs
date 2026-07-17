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

    /// <summary>Color palette ID (e.g., "primary", "secondary", "accent"). Default: "primary".</summary>
    public string ColorPalette { get; set; } = "primary";

    /// <summary>Whether the card is shown on the dashboard (1 = visible, 0 = hidden). Default true.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Record creation timestamp (DB default GETUTCDATE()).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Record last-update timestamp (DB default GETUTCDATE()).</summary>
    public DateTime UpdatedAt { get; set; }

    // === Advanced KPI: Column Mappings ===

    /// <summary>Numeric value column name (e.g., "Quantity", "Amount"). Used by KPI cards.</summary>
    public string ValueColumn { get; set; } = string.Empty;

    /// <summary>Date column name for time-based filtering (e.g., "ItemDate"). Used by KPI cards.</summary>
    public string DateColumn { get; set; } = string.Empty;

    /// <summary>Category column name for grouping (optional, e.g., "WarehouseId"). Used by KPI cards.</summary>
    public string CategoryColumn { get; set; } = string.Empty;

    // === Advanced KPI: Mode & Change Settings ===

    /// <summary>KPI display mode: "simple" (value only), "withChange" (value + change %), "composite" (all). Default: "simple".</summary>
    public string KpiMode { get; set; } = "simple";

    /// <summary>Whether to show percentage change from previous period. Default: false.</summary>
    public bool ShowChange { get; set; } = false;

    /// <summary>Source for change comparison: "previousPeriod", "previousMonth", "previousYear", "customQuery". Default: "previousPeriod".</summary>
    public string ChangeSource { get; set; } = "previousPeriod";

    // === Advanced KPI: Sparkline Settings ===

    /// <summary>Whether to show sparkline trend chart. Default: false.</summary>
    public bool ShowSparkline { get; set; } = false;

    /// <summary>Number of months for sparkline data (3, 6, or 12). Default: 6.</summary>
    public int SparklineMonths { get; set; } = 6;

    // === Advanced KPI: Grand Total Settings ===

    /// <summary>Whether to show grand total value. Default: false.</summary>
    public bool ShowGrandTotal { get; set; } = false;

    /// <summary>Source for grand total: "sameTable" (no date filter), "customQuery", "savedQuery". Default: "sameTable".</summary>
    public string GrandTotalSource { get; set; } = "sameTable";

    // === Advanced KPI: Date Filter Settings ===

    /// <summary>Date filter mode: "dashboard" (from dashboard filter), "fixed" (fixed date range), "relative" (last N days). Default: "dashboard".</summary>
    public string DateFilterMode { get; set; } = "dashboard";

    /// <summary>Fixed start date (ISO format) when DateFilterMode is "fixed". Default: empty.</summary>
    public string FixedStartDate { get; set; } = string.Empty;

    /// <summary>Fixed end date (ISO format) when DateFilterMode is "fixed". Default: empty.</summary>
    public string FixedEndDate { get; set; } = string.Empty;

    /// <summary>Number of days for relative date filter when DateFilterMode is "relative". Default: 30.</summary>
    public int RelativeDays { get; set; } = 30;

    /// <summary>Drill-down levels belonging to this card.</summary>
    public ICollection<CardDrillDownLevel> DrillDownLevels { get; set; } = new List<CardDrillDownLevel>();
}
