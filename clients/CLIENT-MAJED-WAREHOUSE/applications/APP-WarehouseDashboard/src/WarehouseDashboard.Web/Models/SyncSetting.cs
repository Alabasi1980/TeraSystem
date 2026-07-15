namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Singleton sync configuration row (Id = 1).
/// Stored in the <c>SyncSettings</c> config table.
/// </summary>
public class SyncSetting
{
    /// <summary>Primary key (identity). The single config row uses Id = 1.</summary>
    public int Id { get; set; }

    /// <summary>Sync interval in minutes (default 30).</summary>
    public int IntervalMinutes { get; set; }

    /// <summary>Whether automatic sync is enabled (default false).</summary>
    public bool IsAutoSyncEnabled { get; set; }

    /// <summary>Timestamp of the last successful sync (nullable).</summary>
    public DateTime? LastSyncTimestamp { get; set; }
}
