namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Per-mapping detail record for a <see cref="SyncRun"/>.
/// Stores the outcome of syncing one table mapping (extraction + load).
/// </summary>
public class SyncRunDetail
{
    public int Id { get; set; }

    /// <summary>FK to the parent <see cref="SyncRun"/>.</summary>
    public int SyncRunId { get; set; }

    /// <summary>FK to the <see cref="TableMappingConfig"/> (nullable for historical safety).</summary>
    public int? TableMappingId { get; set; }

    /// <summary>Name of the target SQL Server table (e.g. "stg_WarehouseStock").</summary>
    public string TargetTable { get; set; } = string.Empty;

    /// <summary>Sync strategy: "Full" or "Incremental".</summary>
    public string SyncMode { get; set; } = "Full";

    /// <summary>
    /// Outcome: <c>"Success"</c>, <c>"Failed"</c>, or <c>"Skipped"</c>.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Number of rows extracted from Oracle.</summary>
    public int RowsExtracted { get; set; }

    /// <summary>Number of rows loaded into SQL Server.</summary>
    public int RowsLoaded { get; set; }

    /// <summary>Number of attempts made before succeeding or exhausting retries.</summary>
    public int Attempts { get; set; }

    /// <summary>Duration in seconds for this mapping's processing.</summary>
    public double? DurationSeconds { get; set; }

    /// <summary>Error message if the mapping failed; null otherwise.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>UTC timestamp when this record was created.</summary>
    public DateTime CreatedAt { get; set; }

    // Navigation
    public SyncRun SyncRun { get; set; } = null!;
    public TableMappingConfig? TableMapping { get; set; }
}
