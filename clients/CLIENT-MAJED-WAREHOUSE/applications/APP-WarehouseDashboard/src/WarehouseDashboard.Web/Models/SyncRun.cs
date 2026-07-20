namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Represents one sync run cycle (automatic or manual).
/// Stored in the <c>SyncRuns</c> table for persistent logging.
/// Each run has zero or more <see cref="SyncRunDetail"/> entries (one per table mapping).
/// </summary>
public class SyncRun
{
    public int Id { get; set; }

    /// <summary>UTC timestamp when the cycle started.</summary>
    public DateTime StartTime { get; set; }

    /// <summary>UTC timestamp when the cycle finished (null while running).</summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Final status: <c>"Running"</c>, <c>"Success"</c>, <c>"Partial"</c>,
    /// <c>"Failed"</c>, or <c>"Cancelled"</c>.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// What triggered this run: <c>"Auto"</c> (scheduled), <c>"Manual"</c> (full),
    /// or <c>"Manual (selected)"</c> (selected mappings).
    /// </summary>
    public string TriggerType { get; set; } = string.Empty;

    /// <summary>Total number of records loaded across all mappings in this run.</summary>
    public int? TotalRecordCount { get; set; }

    /// <summary>Duration of the run in seconds (derived from StartTime / EndTime).</summary>
    public double? TotalDurationSeconds { get; set; }

    /// <summary>UTC timestamp when this record was created.</summary>
    public DateTime CreatedAt { get; set; }

    // Navigation
    public ICollection<SyncRunDetail> Details { get; set; } = new List<SyncRunDetail>();
}
