using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Handles SQL Server DDL operations for dynamic table mappings:
/// CREATE TABLE from Oracle schema, ALTER TABLE for schema drift,
/// and generates SQL statements for preview before applying.
/// </summary>
public class SchemaManagementService
{
    private readonly string _sqlConnectionString;
    private readonly OracleSchemaService _oracleSchema;
    private readonly ILogger<SchemaManagementService> _logger;

    public SchemaManagementService(
        IConfiguration configuration,
        OracleSchemaService oracleSchema,
        ILogger<SchemaManagementService> logger)
    {
        _sqlConnectionString = Infrastructure.ConnectionStringHelper.Resolve(
            configuration.GetConnectionString("SqlServer"));
        _oracleSchema = oracleSchema;
        _logger = logger;
    }

    /// <summary>
    /// Creates a SQL Server table matching the Oracle source schema.
    /// When <paramref name="columnOverrides"/> is provided, excluded columns are
    /// skipped and overridden names/types/nullability are applied.
    /// </summary>
    public async Task CreateTableFromOracleAsync(
        string oracleSource,
        string targetTable,
        string sourceType = "Table",
        List<ColumnMapping>? columnOverrides = null,
        CancellationToken ct = default)
    {
        var columns = await _oracleSchema.GetOracleTableColumnsAsync(oracleSource, sourceType, ct);
        if (columns.Count == 0)
        {
            throw new InvalidOperationException(
                $"No columns found for Oracle source '{oracleSource}'. " +
                "Check that the source exists and the Oracle connection is working.");
        }

        var createSql = GenerateCreateTableSql(columns, targetTable, columnOverrides);
        _logger.LogInformation("Creating table '{Table}' with {Count} columns.", targetTable, columns.Count);

        await ExecuteNonQueryAsync(createSql, ct);
    }

    /// <summary>
    /// Applies a schema diff (add columns, modify types, drop columns) to an existing table.
    /// When <paramref name="columnOverrides"/> is provided, excluded columns are
    /// skipped and overridden names/types/nullability are applied.
    /// </summary>
    public async Task ApplySchemaChangesAsync(
        string targetTable,
        SchemaDiffResult diff,
        List<ColumnMapping>? columnOverrides = null,
        CancellationToken ct = default)
    {
        if (!diff.HasChanges)
        {
            _logger.LogInformation("No schema changes to apply for '{Table}'.", targetTable);
            return;
        }

        var statements = GenerateAlterStatements(diff, targetTable, columnOverrides);
        if (statements.Count == 0)
        {
            _logger.LogInformation("No ALTER statements generated for '{Table}'.", targetTable);
            return;
        }

        // Execute all ALTER statements in a single transaction
        await using var conn = new SqlConnection(_sqlConnectionString);
        await conn.OpenAsync(ct);

        await using var transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
        try
        {
            foreach (var sql in statements)
            {
                _logger.LogDebug("Executing: {Sql}", sql);

                await using var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = sql;
                cmd.CommandTimeout = 60;
                await cmd.ExecuteNonQueryAsync(ct);
            }

            await transaction.CommitAsync(ct);
            _logger.LogInformation(
                "Applied {Count} schema change(s) to '{Table}'.", statements.Count, targetTable);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Generates ALTER TABLE SQL statements from a schema diff for preview purposes.
    /// When <paramref name="columnOverrides"/> is provided, overridden names, types,
    /// nullability, and exclusion are applied to the generated statements.
    /// Does NOT execute them.
    /// </summary>
    public List<string> GenerateAlterStatements(
        SchemaDiffResult diff,
        string targetTable,
        List<ColumnMapping>? columnOverrides = null)
    {
        var statements = new List<string>();
        var safeName = QuoteSqlServerIdentifier(targetTable);

        // Build a lookup of overrides keyed by OracleColumnName
        var overrideDict = columnOverrides?
            .GroupBy(o => o.OracleColumnName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        // ADD columns
        foreach (var col in diff.ColumnsToAdd)
        {
            string colName;
            string sqlType;
            bool isNullable;

            if (overrideDict is not null &&
                overrideDict.TryGetValue(col.ColumnName, out var ov))
            {
                if (ov.IsExcluded) continue;
                colName = !string.IsNullOrWhiteSpace(ov.SqlColumnName)
                    ? ov.SqlColumnName
                    : col.ColumnName;
                sqlType = OracleSchemaService.FormatOverrideType(ov);
                isNullable = ov.IsNullable;
            }
            else
            {
                colName = col.ColumnName;
                sqlType = OracleSchemaService.MapOracleToSqlServer(col);
                isNullable = col.IsNullable;
            }

            var nullable = isNullable ? "NULL" : "NOT NULL";
            var defaultClause = GetDefaultValueForType(sqlType);

            if (defaultClause is not null)
            {
                statements.Add($"ALTER TABLE {safeName} ADD [{colName}] {sqlType} {nullable} DEFAULT {defaultClause};");
            }
            else
            {
                statements.Add($"ALTER TABLE {safeName} ADD [{colName}] {sqlType} {nullable};");
            }
        }

        // ALTER (modify) columns
        foreach (var colDiff in diff.ColumnsToModify)
        {
            if (overrideDict is not null &&
                overrideDict.TryGetValue(colDiff.ColumnName, out var ov))
            {
                if (ov.IsExcluded) continue;
                var newType = OracleSchemaService.FormatOverrideType(ov);
                statements.Add($"ALTER TABLE {safeName} ALTER COLUMN [{colDiff.ColumnName}] {newType};");
            }
            else
            {
                statements.Add($"ALTER TABLE {safeName} ALTER COLUMN [{colDiff.ColumnName}] {colDiff.NewType};");
            }
        }

        // DROP columns
        foreach (var colName in diff.ColumnsToRemove)
        {
            // Skip if the column is excluded via override (matched by SqlColumnName)
            if (overrideDict is not null &&
                overrideDict.Values.Any(o =>
                    o.IsExcluded &&
                    string.Equals(o.SqlColumnName, colName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            statements.Add($"ALTER TABLE {safeName} DROP COLUMN [{colName}];");
        }

        return statements;
    }

    /// <summary>
    /// Checks whether a table exists in SQL Server.
    /// </summary>
    public async Task<bool> TableExistsAsync(string tableName, CancellationToken ct = default)
    {
        var (schema, table) = ParseSchemaTable(tableName);

        const string sql = """
            SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName
            """;

        await using var conn = new SqlConnection(_sqlConnectionString);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("@Schema", schema);
        cmd.Parameters.AddWithValue("@TableName", table);

        var result = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result) > 0;
    }

    /// <summary>
    /// Generates a CREATE TABLE statement from Oracle column metadata.
    /// When <paramref name="columnOverrides"/> is provided, excluded columns are
    /// skipped and overridden names, types, and nullability are applied.
    /// </summary>
    private static string GenerateCreateTableSql(
        List<ColumnInfo> columns,
        string tableName,
        List<ColumnMapping>? columnOverrides = null)
    {
        // Build a lookup of overrides keyed by OracleColumnName
        var overrideDict = columnOverrides?
            .GroupBy(o => o.OracleColumnName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var sb = new StringBuilder();
        sb.AppendLine($"CREATE TABLE {QuoteSqlServerIdentifier(tableName)} (");

        // Filter out excluded columns, then iterate
        var visibleColumns = columns.Where(c =>
            overrideDict is null ||
            !overrideDict.TryGetValue(c.ColumnName, out var ov) ||
            !ov.IsExcluded).ToList();

        for (var i = 0; i < visibleColumns.Count; i++)
        {
            var col = visibleColumns[i];

            string colName;
            string sqlType;
            bool isNullable;

            if (overrideDict is not null &&
                overrideDict.TryGetValue(col.ColumnName, out var ov))
            {
                colName = !string.IsNullOrWhiteSpace(ov.SqlColumnName)
                    ? ov.SqlColumnName
                    : col.ColumnName;
                sqlType = OracleSchemaService.FormatOverrideType(ov);
                isNullable = ov.IsNullable;
            }
            else
            {
                colName = col.ColumnName;
                sqlType = OracleSchemaService.MapOracleToSqlServer(col);
                isNullable = col.IsNullable;
            }

            var nullable = isNullable ? "NULL" : "NOT NULL";
            var comma = i < visibleColumns.Count - 1 ? "," : "";

            sb.AppendLine($"    [{colName}] {sqlType} {nullable}{comma}");
        }

        sb.AppendLine(");");
        return sb.ToString();
    }

    /// <summary>
    /// Returns a DEFAULT clause for known SQL Server types, or null if no safe default.
    /// </summary>
    private static string? GetDefaultValueForType(string sqlType)
    {
        return sqlType.ToUpperInvariant() switch
        {
            "INT" or "BIGINT" or "SMALLINT" => "0",
            "DECIMAL" or "NUMERIC" or "FLOAT" => "0",
            "BIT" => "0",
            "DATETIME2" or "DATETIMEOFFSET" => "GETUTCDATE()",
            "NVARCHAR(MAX)" or "VARCHAR(MAX)" or "VARBINARY(MAX)" => null,
            _ when sqlType.StartsWith("NVARCHAR", StringComparison.OrdinalIgnoreCase) => "N''",
            _ when sqlType.StartsWith("VARCHAR", StringComparison.OrdinalIgnoreCase) => "''",
            _ when sqlType.StartsWith("NCHAR", StringComparison.OrdinalIgnoreCase) => "N''",
            _ when sqlType.StartsWith("CHAR", StringComparison.OrdinalIgnoreCase) => "''",
            _ when sqlType.StartsWith("VARBINARY", StringComparison.OrdinalIgnoreCase) => null,
            _ => null
        };
    }

    /// <summary>
    /// Quotes a SQL Server identifier as [schema].[table] or just [table].
    /// </summary>
    private static string QuoteSqlServerIdentifier(string identifier)
    {
        var dotIndex = identifier.LastIndexOf('.');
        if (dotIndex > 0 && dotIndex < identifier.Length - 1)
        {
            var schema = identifier[..dotIndex];
            var table = identifier[(dotIndex + 1)..];
            return $"[{schema}].[{table}]";
        }

        return $"[{identifier}]";
    }

    /// <summary>
    /// Parses "dbo.Table" or "Table" into (Schema, Table). Defaults schema to "dbo".
    /// </summary>
    private static (string Schema, string Table) ParseSchemaTable(string identifier)
    {
        var dotIndex = identifier.LastIndexOf('.');
        if (dotIndex > 0 && dotIndex < identifier.Length - 1)
        {
            return (identifier[..dotIndex], identifier[(dotIndex + 1)..]);
        }

        return ("dbo", identifier);
    }

    /// <summary>
    /// Executes a non-query SQL statement against SQL Server.
    /// </summary>
    private async Task ExecuteNonQueryAsync(string sql, CancellationToken ct)
    {
        await using var conn = new SqlConnection(_sqlConnectionString);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = 60;
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
