namespace WarehouseDashboard.Api.Models;

/// <summary>
/// Represents one sync run for the TEMPORARY in-memory log (see <see cref="Services.SyncRunLogStore"/>).
///
/// This is a stopgap until the structured <c>SyncLogs</c> / <c>ErrorLogs</c> DB tables are created
/// (deferred to a later task). It intentionally mirrors the schema described in
/// 14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md §4.3 but lives only in memory and is lost on restart.
/// Do NOT use it for audit or compliance — it is for live monitoring only.
/// </summary>
public class SyncRunRecord
{
    /// <summary>UTC time the run started.</summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>UTC time the run finished (null while still running).</summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Final status: <c>"Running"</c> (in progress / abandoned), <c>"Success"</c>,
    /// <c>"Failed"</c>, or <c>"Cancelled"</c>.
    /// </summary>
    public string Status { get; set; } = "Running";

    /// <summary>Total rows loaded across all mappings in this run.</summary>
    public int RecordCount { get; set; }

    /// <summary>What triggered the run — e.g. <c>"Manual"</c> (API) or <c>"Auto"</c> (schedule).</summary>
    public string TriggerType { get; set; } = "Manual";

    /// <summary>Error detail if the run failed; null otherwise.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Duration of the run in seconds, derived from <see cref="StartTime"/> / <see cref="EndTime"/>.
    /// Null while the run is still in progress.
    /// </summary>
    public double? DurationSeconds =>
        EndTime.HasValue ? Math.Round((EndTime.Value - StartTime).TotalSeconds, 3) : null;
}
