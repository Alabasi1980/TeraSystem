using System.Data;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Reads Oracle table/view structure via ALL_TAB_COLUMNS and SQL Server
/// structure via INFORMATION_SCHEMA.COLUMNS. Provides schema comparison
/// for the dynamic table mapping admin page.
/// </summary>
public class OracleSchemaService
{
    private readonly string _oracleConnectionString;
    private readonly string _sqlConnectionString;
    private readonly ILogger<OracleSchemaService> _logger;

    public OracleSchemaService(IConfiguration configuration, ILogger<OracleSchemaService> logger)
    {
        _oracleConnectionString = ConnectionStringHelper.ResolveOracle(configuration);
        _sqlConnectionString = ConnectionStringHelper.Resolve(configuration.GetConnectionString("SqlServer"));
        _logger = logger;
    }

    /// <summary>
    /// Reads column metadata from Oracle ALL_TAB_COLUMNS for the given source
    /// (table, view, or subquery-derived alias). For "Query" source types the
    /// source is wrapped as a subquery to extract column metadata without data.
    /// </summary>
    public async Task<List<ColumnInfo>> GetOracleTableColumnsAsync(
        string oracleSource,
        string sourceType = "Table",
        CancellationToken ct = default)
    {
        var columns = new List<ColumnInfo>();

        if (string.IsNullOrWhiteSpace(_oracleConnectionString))
        {
            _logger.LogWarning("Oracle connection string is empty. Cannot read schema for '{Source}'.", oracleSource);
            return columns;
        }

        // Build the query based on source type
        string sql;
        if (sourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
        {
            // For queries, wrap as subquery to get column metadata without executing full data
            sql = $"""
                SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, DATA_PRECISION, DATA_SCALE, NULLABLE
                FROM ALL_TAB_COLUMNS
                WHERE OWNER || '.' || TABLE_NAME IN (
                    SELECT OWNER || '.' || TABLE_NAME FROM ALL_TABLES WHERE 1 = 0
                )
                """;
            // Fallback: try to read from the source directly using column_name from a dummy query
            sql = $"""
                SELECT * FROM ({SanitizeQuery(oracleSource)}) WHERE ROWNUM <= 0
                """;
        }
        else
        {
            // For Table/View: parse OWNER.TABLE or just TABLE
            var (owner, tableName) = ParseOracleIdentifier(oracleSource);
            sql = owner is null
                ? $"""
                    SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, DATA_PRECISION, DATA_SCALE, NULLABLE
                    FROM ALL_TAB_COLUMNS
                    WHERE TABLE_NAME = '{EscapeSql(tableName)}'
                    ORDER BY COLUMN_ID
                    """
                : $"""
                    SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, DATA_PRECISION, DATA_SCALE, NULLABLE
                    FROM ALL_TAB_COLUMNS
                    WHERE OWNER = '{EscapeSql(owner)}' AND TABLE_NAME = '{EscapeSql(tableName)}'
                    ORDER BY COLUMN_ID
                    """;
        }

        try
        {
            await using var conn = new OracleConnection(_oracleConnectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                columns.Add(new ColumnInfo
                {
                    ColumnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME")),
                    DataType = reader.GetString(reader.GetOrdinal("DATA_TYPE")),
                    MaxLength = reader.IsDBNull(reader.GetOrdinal("DATA_LENGTH"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("DATA_LENGTH")),
                    Precision = reader.IsDBNull(reader.GetOrdinal("DATA_PRECISION"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("DATA_PRECISION")),
                    Scale = reader.IsDBNull(reader.GetOrdinal("DATA_SCALE"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("DATA_SCALE")),
                    IsNullable = reader.GetString(reader.GetOrdinal("NULLABLE")) == "Y"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read Oracle schema for '{Source}'.", oracleSource);
        }

        return columns;
    }

    /// <summary>
    /// Reads column metadata from SQL Server INFORMATION_SCHEMA.COLUMNS
    /// for the given target table.
    /// </summary>
    public async Task<List<ColumnInfo>> GetSqlServerTableColumnsAsync(
        string targetTable,
        CancellationToken ct = default)
    {
        var columns = new List<ColumnInfo>();

        if (string.IsNullOrWhiteSpace(_sqlConnectionString))
        {
            _logger.LogWarning("SQL Server connection string is empty. Cannot read schema for '{Table}'.", targetTable);
            return columns;
        }

        // Parse schema.table or just table
        var (schema, tableName) = ParseSqlServerIdentifier(targetTable);

        const string sql = """
            SELECT
                COLUMN_NAME,
                DATA_TYPE,
                CHARACTER_MAXIMUM_LENGTH AS MAX_LENGTH,
                NUMERIC_PRECISION,
                NUMERIC_SCALE,
                IS_NULLABLE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName
            ORDER BY ORDINAL_POSITION
            """;

        try
        {
            await using var conn = new SqlConnection(_sqlConnectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Schema", schema);
            cmd.Parameters.AddWithValue("@TableName", tableName);
            cmd.CommandTimeout = 30;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                columns.Add(new ColumnInfo
                {
                    ColumnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME")),
                    DataType = reader.GetString(reader.GetOrdinal("DATA_TYPE")),
                    MaxLength = reader.IsDBNull(reader.GetOrdinal("MAX_LENGTH"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("MAX_LENGTH")),
                    Precision = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION"))
                        ? null
                        : (int)reader.GetByte(reader.GetOrdinal("NUMERIC_PRECISION")),
                    Scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE"))
                        ? null
                        : (int)reader.GetByte(reader.GetOrdinal("NUMERIC_SCALE")),
                    IsNullable = reader.GetString(reader.GetOrdinal("IS_NULLABLE")) == "YES"
                });
            }
        }
        catch (SqlException ex) when (ex.Number == 208) // Invalid object name
        {
            // Table doesn't exist yet — return empty list
            _logger.LogInformation("Target table '{Table}' does not exist yet.", targetTable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read SQL Server schema for '{Table}'.", targetTable);
        }

        return columns;
    }

    /// <summary>
    /// Compares Oracle source columns against SQL Server target columns and
    /// returns a diff report showing what needs to be added, removed, or modified.
    /// </summary>
    public async Task<SchemaDiffResult> CompareSchemasAsync(
        string oracleSource,
        string targetTable,
        string sourceType = "Table",
        CancellationToken ct = default)
    {
        var oracleColumns = await GetOracleTableColumnsAsync(oracleSource, sourceType, ct);
        var sqlColumns = await GetSqlServerTableColumnsAsync(targetTable, ct);

        var result = new SchemaDiffResult();

        // If SQL target doesn't exist (empty), all Oracle columns need to be added
        if (sqlColumns.Count == 0)
        {
            result.ColumnsToAdd = oracleColumns;
            return result;
        }

        var sqlColumnDict = sqlColumns.ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

        foreach (var oracleCol in oracleColumns)
        {
            if (sqlColumnDict.TryGetValue(oracleCol.ColumnName, out var sqlCol))
            {
                // Column exists — check if type needs modification
                var oracleSqlServerType = MapOracleToSqlServer(oracleCol);
                var currentSqlServerType = FormatSqlServerType(sqlCol);

                if (!string.Equals(oracleSqlServerType, currentSqlServerType, StringComparison.OrdinalIgnoreCase))
                {
                    result.ColumnsToModify.Add(new ColumnDiff
                    {
                        ColumnName = oracleCol.ColumnName,
                        CurrentType = currentSqlServerType,
                        NewType = oracleSqlServerType
                    });
                }
            }
            else
            {
                // Column exists in Oracle but not in SQL Server
                result.ColumnsToAdd.Add(oracleCol);
            }
        }

        // Check for columns in SQL Server that are not in Oracle (candidates for removal)
        var oracleColumnNames = oracleColumns
            .Select(c => c.ColumnName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var sqlCol in sqlColumns)
        {
            if (!oracleColumnNames.Contains(sqlCol.ColumnName))
            {
                result.ColumnsToRemove.Add(sqlCol.ColumnName);
            }
        }

        return result;
    }

    /// <summary>
    /// Maps an Oracle column type to the corresponding SQL Server type string.
    /// </summary>
    public static string MapOracleToSqlServer(ColumnInfo column)
    {
        var oracleType = column.DataType.ToUpperInvariant();

        return oracleType switch
        {
            // VARCHAR2 → NVARCHAR(MAX) if > 4000, else NVARCHAR(n)
            "VARCHAR2" => (column.MaxLength ?? 0) > 4000 ? "NVARCHAR(MAX)" : $"NVARCHAR({column.MaxLength ?? 4000})",

            // CHAR → NCHAR(n)
            "CHAR" => $"NCHAR({column.MaxLength ?? 1})",

            // NUMBER → INT / BIGINT / DECIMAL depending on precision
            "NUMBER" => MapOracleNumber(column.Precision, column.Scale),

            // DATE / TIMESTAMP → DATETIME2
            "DATE" => "DATETIME2",
            "TIMESTAMP" => "DATETIME2",
            "TIMESTAMP(6)" => "DATETIME2",
            "TIMESTAMP WITH TIME ZONE" => "DATETIMEOFFSET",
            "TIMESTAMP WITH LOCAL TIME ZONE" => "DATETIME2",

            // CLOB → NVARCHAR(MAX)
            "CLOB" => "NVARCHAR(MAX)",

            // NCLOB → NVARCHAR(MAX)
            "NCLOB" => "NVARCHAR(MAX)",

            // BLOB → VARBINARY(MAX)
            "BLOB" => "VARBINARY(MAX)",

            // RAW → VARBINARY(n)
            "RAW" => $"VARBINARY({column.MaxLength ?? 4000})",

            // INTEGER → INT
            "INTEGER" => "INT",

            // SMALLINT → SMALLINT
            "SMALLINT" => "SMALLINT",

            // FLOAT → FLOAT
            "FLOAT" => "FLOAT",

            // DOUBLE PRECISION → FLOAT
            "DOUBLE PRECISION" => "FLOAT",

            // BOOLEAN → BIT
            "BOOLEAN" => "BIT",

            // Default: NVARCHAR(MAX)
            _ => "NVARCHAR(MAX)"
        };
    }

    /// <summary>
    /// Maps Oracle NUMBER(p,s) to appropriate SQL Server type.
    /// </summary>
    private static string MapOracleNumber(int? precision, int? scale)
    {
        // NUMBER with no precision → DECIMAL(38,10) or FLOAT
        if (precision is null || precision == 0)
            return "FLOAT";

        var p = precision.Value;
        var s = scale ?? 0;

        // NUMBER(p,0) — integer types
        if (s == 0)
        {
            return p switch
            {
                <= 9 => "INT",
                <= 18 => "BIGINT",
                _ => $"DECIMAL({p}, 0)"
            };
        }

        // NUMBER(p,s) — decimal
        return $"DECIMAL({p}, {s})";
    }

    /// <summary>
    /// Formats a SQL Server column type string from ColumnInfo for display.
    /// </summary>
    private static string FormatSqlServerType(ColumnInfo column)
    {
        var type = column.DataType.ToUpperInvariant();

        return type switch
        {
            "NVARCHAR" => (column.MaxLength ?? 0) <= 0 ? "NVARCHAR(MAX)" : $"NVARCHAR({column.MaxLength})",
            "VARCHAR" => (column.MaxLength ?? 0) <= 0 ? "VARCHAR(MAX)" : $"VARCHAR({column.MaxLength})",
            "NCHAR" => $"NCHAR({column.MaxLength ?? 1})",
            "CHAR" => $"CHAR({column.MaxLength ?? 1})",
            "VARBINARY" => (column.MaxLength ?? 0) <= 0 ? "VARBINARY(MAX)" : $"VARBINARY({column.MaxLength})",
            "DECIMAL" or "NUMERIC" => column.Precision.HasValue
                ? $"DECIMAL({column.Precision}{(column.Scale.HasValue ? $",{column.Scale}" : "")})"
                : "DECIMAL(18,2)",
            _ => type
        };
    }

    /// <summary>
    /// Parses "OWNER.TABLE" or "TABLE" into (Owner, Table) tuple.
    /// </summary>
    private static (string? Owner, string Table) ParseOracleIdentifier(string identifier)
    {
        var dotIndex = identifier.LastIndexOf('.');
        if (dotIndex > 0 && dotIndex < identifier.Length - 1)
        {
            return (identifier[..dotIndex], identifier[(dotIndex + 1)..]);
        }

        return (null, identifier);
    }

    /// <summary>
    /// Parses "dbo.Table" or "Table" into (Schema, Table) tuple. Defaults schema to "dbo".
    /// </summary>
    private static (string Schema, string Table) ParseSqlServerIdentifier(string identifier)
    {
        var dotIndex = identifier.LastIndexOf('.');
        if (dotIndex > 0 && dotIndex < identifier.Length - 1)
        {
            return (identifier[..dotIndex], identifier[(dotIndex + 1)..]);
        }

        return ("dbo", identifier);
    }

    /// <summary>
    /// Basic SQL escape for Oracle identifiers (single-quote doubling).
    /// For production, consider using bind parameters or Oracle's DBMS_ASSERT.
    /// </summary>
    private static string EscapeSql(string value) => value.Replace("'", "''", StringComparison.Ordinal);

    /// <summary>
    /// Lists all Oracle tables visible to the connected user, with column count
    /// and whether each table is already mapped in the SQL Server TableMappings table.
    /// </summary>
    public async Task<List<OracleObject>> ListOracleTablesAsync(CancellationToken ct = default)
    {
        return await ListOracleObjectsAsync("TABLE", ct);
    }

    /// <summary>
    /// Lists all Oracle views visible to the connected user, with column count
    /// and whether each view is already mapped in the SQL Server TableMappings table.
    /// </summary>
    public async Task<List<OracleObject>> ListOracleViewsAsync(CancellationToken ct = default)
    {
        return await ListOracleObjectsAsync("VIEW", ct);
    }

    /// <summary>
    /// Previews the first <paramref name="limit"/> rows from an Oracle table, view, or query.
    /// Returns column names, types, and sample data rows.
    /// </summary>
    public async Task<DataPreviewResult> PreviewDataAsync(
        string oracleSource,
        string sourceType,
        int limit = 10,
        CancellationToken ct = default)
    {
        var result = new DataPreviewResult();

        if (string.IsNullOrWhiteSpace(_oracleConnectionString))
        {
            result.ErrorMessage = "Oracle connection string is empty.";
            return result;
        }

        try
        {
            string sql;
            if (sourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
            {
                sql = $"SELECT * FROM ({SanitizeQuery(oracleSource)}) WHERE ROWNUM <= {limit}";
            }
            else
            {
                sql = $"SELECT * FROM {oracleSource} WHERE ROWNUM <= {limit}";
            }

            await using var conn = new OracleConnection(_oracleConnectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 10;

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            // Read column metadata
            for (var i = 0; i < reader.FieldCount; i++)
            {
                result.Columns.Add(reader.GetName(i));
                result.ColumnTypes.Add(reader.GetFieldType(i)?.Name ?? "UNKNOWN");
            }

            // Read rows
            while (await reader.ReadAsync(ct))
            {
                var row = new Dictionary<string, object?>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row[result.Columns[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                result.Rows.Add(row);
            }

            // Get total row count (separate lightweight query)
            string countSql;
            if (sourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
            {
                countSql = $"SELECT COUNT(*) FROM ({SanitizeQuery(oracleSource)})";
            }
            else
            {
                countSql = $"SELECT COUNT(*) FROM {oracleSource}";
            }

            await using var countCmd = conn.CreateCommand();
            countCmd.CommandText = countSql;
            countCmd.CommandTimeout = 10;
            var totalObj = await countCmd.ExecuteScalarAsync(ct);
            result.TotalRows = totalObj is not null ? Convert.ToInt32(totalObj) : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to preview data for '{Source}'.", oracleSource);
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Validates a user-provided SQL query and returns its column metadata.
    /// Rejects non-SELECT (non-read-only) statements.
    /// </summary>
    public async Task<QueryValidationResult> ValidateQueryAsync(
        string query,
        CancellationToken ct = default)
    {
        var result = new QueryValidationResult();

        if (string.IsNullOrWhiteSpace(_oracleConnectionString))
        {
            result.ErrorMessage = "Oracle connection string is empty.";
            return result;
        }

        var trimmed = query.Trim();

        // Check read-only: only SELECT or WITH (CTE) allowed
        result.IsReadOnly = IsReadOnlyQuery(trimmed);
        if (!result.IsReadOnly)
        {
            result.ErrorMessage = "Only SELECT or WITH (CTE) queries are allowed. INSERT, UPDATE, DELETE, DROP, etc. are rejected.";
            return result;
        }

        try
        {
            var sql = $"SELECT * FROM ({SanitizeQuery(trimmed)}) WHERE ROWNUM <= 0";

            await using var conn = new OracleConnection(_oracleConnectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 10;

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            for (var i = 0; i < reader.FieldCount; i++)
            {
                result.Columns.Add(new ColumnInfo
                {
                    ColumnName = reader.GetName(i),
                    DataType = reader.GetFieldType(i)?.Name ?? "UNKNOWN"
                });
            }

            result.IsValid = true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Query validation failed for: {Query}", trimmed);
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Internal: Lists Oracle objects (tables or views) with column count and mapping status.
    /// </summary>
    private async Task<List<OracleObject>> ListOracleObjectsAsync(
        string objectType,
        CancellationToken ct)
    {
        var objects = new List<OracleObject>();

        if (string.IsNullOrWhiteSpace(_oracleConnectionString))
        {
            _logger.LogWarning("Oracle connection string is empty. Cannot list {ObjectType}s.", objectType);
            return objects;
        }

        try
        {
            await using var conn = new OracleConnection(_oracleConnectionString);
            await conn.OpenAsync(ct);

            // Extract owner from connection string or use current schema
            var owner = ExtractOwnerFromConnection(conn);

            // Query tables/views with column count
            string sql;
            if (objectType == "VIEW")
            {
                sql = """
                    SELECT v.OWNER, v.VIEW_NAME AS OBJECT_NAME, 'VIEW' AS OBJECT_TYPE,
                           COUNT(c.COLUMN_NAME) AS COLUMN_COUNT
                    FROM ALL_VIEWS v
                    LEFT JOIN ALL_TAB_COLUMNS c ON v.OWNER = c.OWNER AND v.VIEW_NAME = c.TABLE_NAME
                    WHERE v.OWNER = :owner
                    GROUP BY v.OWNER, v.VIEW_NAME
                    ORDER BY v.VIEW_NAME
                    """;
            }
            else
            {
                sql = """
                    SELECT t.OWNER, t.TABLE_NAME AS OBJECT_NAME, 'TABLE' AS OBJECT_TYPE,
                           COUNT(c.COLUMN_NAME) AS COLUMN_COUNT
                    FROM ALL_TABLES t
                    LEFT JOIN ALL_TAB_COLUMNS c ON t.OWNER = c.OWNER AND t.TABLE_NAME = c.TABLE_NAME
                    WHERE t.OWNER = :owner
                    GROUP BY t.OWNER, t.TABLE_NAME
                    ORDER BY t.TABLE_NAME
                    """;
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new OracleParameter(":owner", OracleDbType.Varchar2) { Value = owner });
            cmd.CommandTimeout = 15;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var objectName = reader.GetString(reader.GetOrdinal("OBJECT_NAME"));
                objects.Add(new OracleObject
                {
                    Owner = reader.GetString(reader.GetOrdinal("OWNER")),
                    ObjectName = objectName,
                    ObjectType = objectType,
                    ColumnCount = reader.GetInt32(reader.GetOrdinal("COLUMN_COUNT"))
                });
            }

            // Mark already-mapped objects
            await MarkAlreadyMappedAsync(objects, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list Oracle {ObjectType}s.", objectType);
        }

        return objects;
    }

    /// <summary>
    /// Checks which objects from the given list are already present in the SQL Server
    /// TableMappings table (by matching OracleSource = OWNER.OBJECT_NAME).
    /// </summary>
    private async Task MarkAlreadyMappedAsync(
        List<OracleObject> objects,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_sqlConnectionString) || objects.Count == 0)
            return;

        try
        {
            await using var conn = new SqlConnection(_sqlConnectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT OracleSource FROM TableMappings";
            cmd.CommandTimeout = 10;

            var mappedSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                mappedSources.Add(reader.GetString(reader.GetOrdinal("OracleSource")));
            }

            foreach (var obj in objects)
            {
                var fullName = $"{obj.Owner}.{obj.ObjectName}";
                obj.IsAlreadyMapped = mappedSources.Contains(fullName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check TableMappings for already-mapped status.");
        }
    }

    /// <summary>
    /// Extracts the Oracle schema owner from the connection string username.
    /// </summary>
    private static string ExtractOwnerFromConnection(OracleConnection conn)
    {
        // Parse User ID from the connection string
        var builder = new OracleConnectionStringBuilder(conn.ConnectionString);
        return (builder.UserID ?? string.Empty).ToUpperInvariant();
    }

    /// <summary>
    /// Determines if the given query is read-only (SELECT or WITH only).
    /// </summary>
    private static bool IsReadOnlyQuery(string query)
    {
        var upper = query.TrimStart().ToUpperInvariant();
        return upper.StartsWith("SELECT") || upper.StartsWith("WITH");
    }

    /// <summary>
    /// Sanitizes a user-provided query for schema introspection.
    /// Only allows SELECT statements (read-only).
    /// </summary>
    private static string SanitizeQuery(string query)
    {
        var trimmed = query.Trim();
        if (!IsReadOnlyQuery(trimmed))
        {
            throw new InvalidOperationException("Only SELECT queries are allowed for schema introspection.");
        }

        return trimmed;
    }
}

/// <summary>
/// Metadata for a single database column.
/// </summary>
public class ColumnInfo
{
    public string ColumnName { get; set; } = "";
    public string DataType { get; set; } = "";
    public long? MaxLength { get; set; }
    public bool IsNullable { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
}

/// <summary>
/// Result of comparing Oracle source schema against SQL Server target schema.
/// </summary>
public class SchemaDiffResult
{
    public List<ColumnInfo> ColumnsToAdd { get; set; } = new();
    public List<string> ColumnsToRemove { get; set; } = new();
    public List<ColumnDiff> ColumnsToModify { get; set; } = new();

    public bool HasChanges => ColumnsToAdd.Count > 0 || ColumnsToRemove.Count > 0 || ColumnsToModify.Count > 0;

    public string Summary => $"Added: {ColumnsToAdd.Count}, Removed: {ColumnsToRemove.Count}, Modified: {ColumnsToModify.Count}";
}

/// <summary>
/// Describes a single column type change between Oracle and SQL Server.
/// </summary>
public class ColumnDiff
{
    public string ColumnName { get; set; } = "";
    public string CurrentType { get; set; } = "";
    public string NewType { get; set; } = "";
}

// ──────────────────────────────────────────────────────────────────────────
//  Oracle Browser Wizard models (TASK-COD-029)
// ──────────────────────────────────────────────────────────────────────────

/// <summary>
/// Represents a single Oracle table or view visible to the connected user.
/// </summary>
public class OracleObject
{
    public string Owner { get; set; } = "";
    public string ObjectName { get; set; } = "";
    public string ObjectType { get; set; } = "";
    public int ColumnCount { get; set; }
    public bool IsAlreadyMapped { get; set; }
}

/// <summary>
/// Result of previewing data from an Oracle source.
/// </summary>
public class DataPreviewResult
{
    public List<string> Columns { get; set; } = new();
    public List<string> ColumnTypes { get; set; } = new();
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public int TotalRows { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of validating a user-provided SQL query against Oracle.
/// </summary>
public class QueryValidationResult
{
    public bool IsValid { get; set; }
    public List<ColumnInfo> Columns { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public bool IsReadOnly { get; set; }
}
