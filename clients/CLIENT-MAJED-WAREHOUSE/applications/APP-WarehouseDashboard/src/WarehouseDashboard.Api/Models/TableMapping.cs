namespace WarehouseDashboard.Api.Models;

/// <summary>
/// Maps an Oracle source (table / view / query) to a SQL Server staging target.
/// Consumed by <see cref="Services.SyncEngineService"/> to drive the full-refresh pipeline:
/// Oracle extraction (<see cref="Services.OracleExtractionService"/>) followed by
/// SQL Server bulk load (<see cref="Services.SqlServerLoadService"/>).
/// </summary>
public class TableMapping
{
    /// <summary>
    /// Primary key from the TableMappings DB table. Assigned by the database and used
    /// to select specific mappings for manual sync (<c>POST /api/sync/trigger-selected</c>).
    /// </summary>
    public int Id { get; set; }

    /// <summary>Human-readable name for this mapping. Used as the primary display identifier.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Oracle source identifier: a table name, a view name, or a full SQL query.
    /// Passed verbatim (for "Query") or wrapped as <c>SELECT * FROM <source></c>
    /// (for "Table"/"View") to <c>OracleExtractionService.ExtractAsync</c>.
    /// </summary>
    public string OracleSource { get; set; } = string.Empty;

    /// <summary>
    /// The kind of <see cref="OracleSource"/>. One of: "Table", "View", "Query".
    /// Used for diagnostics and to decide how the source is queried; the source string
    /// itself is always passed through as-is.
    /// </summary>
    public string SourceType { get; set; } = "Query";

    /// <summary>
    /// Target table name in SQL Server (e.g. "stg_WarehouseStock"). Receives the
    /// full-refresh bulk copy. May be schema-qualified (e.g. "dbo.stg_WarehouseStock").
    /// </summary>
    public string SqlTargetTable { get; set; } = string.Empty;

    /// <summary>
    /// Sync strategy: "Full" (truncate + reload all rows) or "Incremental" (append rows
    /// newer than the last watermark). Defaults to "Full".
    /// </summary>
    public string SyncMode { get; set; } = "Full";

    /// <summary>
    /// Oracle column name used for incremental watermarking (e.g. "LAST_UPDATE_DATE").
    /// Only relevant when <see cref="SyncMode"/> is "Incremental".
    /// </summary>
    public string? IncrementalColumn { get; set; }

    /// <summary>
    /// UTC timestamp of the last successful sync for this mapping. Used as the watermark
    /// boundary for incremental syncs.
    /// </summary>
    public DateTime? LastSyncAt { get; set; }
}
