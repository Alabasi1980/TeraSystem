namespace WarehouseDashboard.Api.Models;

/// <summary>
/// Request body for <c>POST /api/sync/trigger-selected</c>. Specifies which table mappings
/// (by their database primary key) to include in a manual sync cycle.
/// </summary>
public class SelectedSyncRequest
{
    /// <summary>
    /// The set of <see cref="TableMapping.Id"/> values to sync. Must contain at least one ID.
    /// </summary>
    public List<int> MappingIds { get; set; } = new();
}

/// <summary>
/// Top-level response returned by <c>POST /api/sync/trigger-selected</c>.
/// </summary>
public class SelectedSyncResult
{
    /// <summary>
    /// Overall outcome: <c>"success"</c> (all mappings completed without error),
    /// <c>"partial"</c> (some mappings failed), or <c>"failed"</c> (all mappings failed).
    /// </summary>
    public string Status { get; set; } = "success";

    /// <summary>Total number of rows loaded across all successfully completed mappings.</summary>
    public int TotalRows { get; set; }

    /// <summary>Per-mapping detailed results.</summary>
    public List<MappingSyncResult> Mappings { get; set; } = new();
}

/// <summary>
/// Per-mapping result included in <see cref="SelectedSyncResult.Mappings"/>.
/// </summary>
public class MappingSyncResult
{
    /// <summary>Primary key of the <see cref="TableMapping"/> that was processed.</summary>
    public int Id { get; set; }

    /// <summary>The target SQL Server table name (e.g. <c>"stg_WarehouseStock"</c>).</summary>
    public string TargetTable { get; set; } = "";

    /// <summary>Outcome for this mapping: <c>"success"</c> or <c>"failed"</c>.</summary>
    public string Status { get; set; } = "";

    /// <summary>Number of rows loaded for this mapping (0 on failure).</summary>
    public int Rows { get; set; }

    /// <summary>Duration of the mapping sync in seconds (null if not measured).</summary>
    public double? DurationSeconds { get; set; }

    /// <summary>Populated only when <see cref="Status"/> is <c>"failed"</c>.</summary>
    public string? Error { get; set; }
}

// ---- Live-progress models (TASK-ENH-003) ----------------------------------------------

/// <summary>
/// Overall progress of a single background sync run initiated by <c>POST /api/sync/trigger-selected</c>.
/// Stored in-memory by <see cref="Services.SyncRunProgressStore"/> and polled by the UI via
/// <c>GET /api/sync/progress?runId=...</c>.
/// </summary>
public class SyncRunProgress
{
    /// <summary>Unique identifier for this run, returned to the caller immediately.</summary>
    public Guid RunId { get; set; }

    /// <summary>
    /// Overall status: <c>"running"</c> while the background task is executing,
    /// <c>"completed"</c> when every mapping finished, or <c>"failed"</c> on critical error.
    /// </summary>
    public string OverallStatus { get; set; } = "running";

    /// <summary>
    /// Aggregate progress (0–100) calculated from completed mappings over total mappings.
    /// </summary>
    public int OverallPercent { get; set; }

    /// <summary>Total number of rows loaded across all mappings so far.</summary>
    public int TotalRowsSoFar { get; set; }

    /// <summary>UTC timestamp when the run was created.</summary>
    public DateTime StartedAt { get; set; }

    /// <summary>Seconds elapsed since <see cref="StartedAt"/> (set by the API on each poll).</summary>
    public double ElapsedSeconds { get; set; }

    /// <summary>Per-mapping progress entries, one per requested mapping ID.</summary>
    public List<MappingProgress> Mappings { get; set; } = new();
}

/// <summary>
/// Per-mapping progress entry within a <see cref="SyncRunProgress"/>.
/// </summary>
public class MappingProgress
{
    /// <summary>The <see cref="TableMapping.Id"/> this entry refers to.</summary>
    public int MappingId { get; set; }

    /// <summary>The target SQL Server table name (e.g. <c>"stg_WarehouseStock"</c>).</summary>
    public string TargetTable { get; set; } = "";

    /// <summary>
    /// Current phase: <c>"pending"</c>, <c>"running"</c>, <c>"completed"</c>, or <c>"failed"</c>.
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>Number of rows extracted / loaded so far.</summary>
    public int RowsSoFar { get; set; }

    /// <summary>Progress estimate (0–100). 100 when <see cref="Status"/> is <c>"completed"</c>.</summary>
    public int Percent { get; set; }

    /// <summary>Populated only when <see cref="Status"/> is <c>"failed"</c>.</summary>
    public string? Error { get; set; }
}
