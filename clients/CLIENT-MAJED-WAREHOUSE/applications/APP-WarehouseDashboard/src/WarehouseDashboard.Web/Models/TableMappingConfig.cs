using System.ComponentModel.DataAnnotations;

namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Config-driven table mapping stored in SQL Server. Managed via Admin Panel.
/// Replaces the static TableMappings array in appsettings.json.
/// Each row defines an Oracle source (table/view/query) that is synced into
/// a SQL Server target table by the <c>SyncEngineService</c>.
/// </summary>
public class TableMappingConfig
{
    public int Id { get; set; }

    /// <summary>Human-readable name for this mapping. Used as the primary identifier in Card Builder and UI. Must be unique.</summary>
    [Required]
    // No MaxLength — Name is seeded from OracleSource which can be a long query.
    // Uniqueness is enforced at the application layer.
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Oracle source identifier: a table name, a view name, or a full SQL query.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string OracleSource { get; set; } = string.Empty;

    /// <summary>
    /// The kind of OracleSource. One of: "Table", "View", "Query".
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SourceType { get; set; } = "Table";

    /// <summary>
    /// Target table name in SQL Server (e.g. "stg_WarehouseStock").
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string SqlTargetTable { get; set; } = string.Empty;

    /// <summary>Whether this mapping is active and included in sync cycles.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>UTC timestamp when this mapping was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when this mapping was last updated.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the last successful sync for this mapping.</summary>
    public DateTime? LastSyncAt { get; set; }

    /// <summary>Number of records loaded in the last sync cycle.</summary>
    public int SyncRecordCount { get; set; }

    /// <summary>Last error message from a failed sync (null if no error).</summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Sync strategy: "Full" (truncate + reload) or "Incremental" (append new rows only).
    /// </summary>
    [MaxLength(10)]
    public string SyncMode { get; set; } = "Full";

    /// <summary>
    /// Oracle column name used for incremental watermarking (e.g. "LAST_UPDATE_DATE").
    /// Only applicable when <see cref="SyncMode"/> is "Incremental".
    /// </summary>
    [MaxLength(128)]
    public string? IncrementalColumn { get; set; }
}
