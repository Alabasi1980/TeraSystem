namespace WarehouseDashboard.Web.Models;

/// <summary>
/// A dashboard grouping multiple <see cref="DashboardCard"/>s into a tab/page.
/// Stored in the <c>Dashboards</c> config table.
/// </summary>
public class Dashboard
{
    /// <summary>Primary key (identity).</summary>
    public int Id { get; set; }

    /// <summary>Display name of the dashboard (required, max 200 chars).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for public routing (required, unique, case-insensitive, max 200 chars).
    /// Auto-generated from <see cref="Name"/> if not provided.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>Optional description shown as tooltip or subtitle (max 500 chars).</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Emoji or icon identifier for the dashboard tab. Default: "📊".</summary>
    public string Icon { get; set; } = "📊";

    /// <summary>Display order among dashboards. Lower values appear first. Default 0.</summary>
    public int SortOrder { get; set; }

    /// <summary>Whether this dashboard is shown to end users. Default true.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Whether this is the default (landing) dashboard. Only one dashboard can be default. Default false.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Record creation timestamp (DB default GETUTCDATE()).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Record last-update timestamp (DB default GETUTCDATE()).</summary>
    public DateTime UpdatedAt { get; set; }
}
