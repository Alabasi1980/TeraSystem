namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Config-driven per-column type mapping stored in SQL Server.
/// Each row defines an override for a column in a <see cref="TableMappingConfig"/>:
/// SQL Server name, data type, length/precision/scale, nullability, exclusion,
/// default value, and an optional transformation expression for future use.
/// Managed via Admin Panel (Column Mapping Editor).
/// </summary>
public class ColumnMapping
{
    public int Id { get; set; }

    /// <summary>FK to the parent <see cref="TableMappingConfig"/>.</summary>
    public int TableMappingConfigId { get; set; }

    /// <summary>Column name as it appears in the Oracle source.</summary>
    public string OracleColumnName { get; set; } = string.Empty;

    /// <summary>Column name in the SQL Server target. Defaults to <see cref="OracleColumnName"/>.</summary>
    public string SqlColumnName { get; set; } = string.Empty;

    /// <summary>Target SQL Server data type (e.g. NVARCHAR, INT, BIGINT, DECIMAL).</summary>
    public string SqlDataType { get; set; } = string.Empty;

    /// <summary>Max length for string types (e.g. 255). Null when not applicable.</summary>
    public int? SqlMaxLength { get; set; }

    /// <summary>Precision for numeric types (e.g. 18). Null when not applicable.</summary>
    public int? SqlPrecision { get; set; }

    /// <summary>Scale for numeric types (e.g. 2). Null when not applicable.</summary>
    public int? SqlScale { get; set; }

    /// <summary>Whether the column allows NULL values. Defaults to true.</summary>
    public bool IsNullable { get; set; } = true;

    /// <summary>Whether this column is excluded from sync operations.</summary>
    public bool IsExcluded { get; set; }

    /// <summary>Default value expression for the column (e.g. "GETUTCDATE()").</summary>
    public string? DefaultValue { get; set; }

    /// <summary>Optional transformation expression for future ETL use (e.g. "UPPER([Name])").</summary>
    public string? TransformationExpression { get; set; }

    /// <summary>Ordinal position of this column within the mapping. Used for display ordering.</summary>
    public int SortOrder { get; set; }

    // Navigation
    public TableMappingConfig? TableMappingConfig { get; set; }
}
