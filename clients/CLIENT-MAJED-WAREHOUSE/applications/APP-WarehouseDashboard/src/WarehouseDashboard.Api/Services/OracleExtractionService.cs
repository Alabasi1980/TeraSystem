using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using WarehouseDashboard.Api.Infrastructure;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// Extracts data from Oracle in a READ-ONLY manner for the Sync Engine
/// (the EXTRACTION half of the sync pipeline).
///
/// Pipeline: OracleConnection -> OracleCommand -> OracleDataReader -> DataTable.
///
/// The returned <see cref="DataTable"/> preserves the Oracle column names exactly
/// as returned by the query and carries the .NET types resolved by ODP.NET
/// (see 14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md §2 and §3). It is the contract
/// consumed downstream by <c>SqlServerLoadService</c> via SqlBulkCopy.
///
/// This service NEVER executes INSERT/UPDATE/DELETE — only SELECT (or WITH ... SELECT).
/// The read-only guard below is defense-in-depth on top of the Oracle READ-ONLY
/// account privilege recommended in the integration document (§1.2).
/// </summary>
public class OracleExtractionService
{
    private readonly string _connectionString;
    private readonly ILogger<OracleExtractionService> _logger;

    /// <summary>
    /// Initializes the service and resolves the Oracle connection string from
    /// <c>ConnectionStrings:Oracle</c> plus the <c>ORACLE_PASSWORD</c> environment
    /// variable (via <see cref="ConnectionStringHelper.ResolveOracle"/>).
    /// The password is read at runtime only — never from source or config files.
    /// </summary>
    public OracleExtractionService(IConfiguration configuration, ILogger<OracleExtractionService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        _connectionString = ConnectionStringHelper.ResolveOracle(configuration);
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException(
                "Oracle connection string is empty. Ensure 'ConnectionStrings:Oracle' is configured in " +
                "appsettings.json and the ORACLE_PASSWORD environment variable is set at runtime.");
        }
    }

    /// <summary>
    /// Executes a read-only SELECT against Oracle and returns the result as a populated
    /// <see cref="DataTable"/>.
    /// </summary>
    /// <param name="oracleSql">A single read-only SELECT (or WITH ... SELECT) statement.</param>
    /// <param name="cancellationToken">Propagates cancellation (e.g. Sync Engine shutdown).</param>
    /// <returns>A <see cref="DataTable"/> with original Oracle column names and ODP.NET .NET types.</returns>
    /// <exception cref="ArgumentException">When <paramref name="oracleSql"/> is null/empty.</exception>
    /// <exception cref="InvalidOperationException">
    /// When the statement is not read-only, or when an Oracle failure occurs (wrapped with query context).
    /// </exception>
    public async Task<DataTable> ExtractAsync(
        string oracleSql,
        IReadOnlySet<string>? numericTextColumns = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(oracleSql))
        {
            throw new ArgumentException("Oracle SQL query must not be null or empty.", nameof(oracleSql));
        }

        if (!IsReadOnlyQuery(oracleSql))
        {
            throw new InvalidOperationException(
                "Only read-only SELECT (or WITH ... SELECT) statements are permitted for Oracle extraction. " +
                "INSERT/UPDATE/DELETE/MERGE/DDL are rejected by design.");
        }

        try
        {
            // Fresh connection per call — the service is stateless and connections are
            // disposed promptly (using) so the pool can reclaim them.
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = connection.CreateCommand();
            command.CommandText = oracleSql;
            command.CommandType = CommandType.Text;

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            // Load schema from reader (column names + ODP.NET inferred types)
            // We use a two-phase approach to handle Oracle NUMBER values that overflow
            // .NET decimal (38-digit NUMBER vs 29-digit decimal max). Phase 1 loads schema
            // only; Phase 2 reads rows with per-cell overflow protection.
            var schemaTable = reader.GetSchemaTable();
            var table = BuildDataTableFromSchema(schemaTable, numericTextColumns);
            while (await reader.ReadAsync(cancellationToken))
            {
                var row = table.NewRow();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                    {
                        row[i] = DBNull.Value;
                    }
                    else
                    {
                        if (IsNumericTextColumn(reader.GetName(i), numericTextColumns))
                        {
                            row[i] = GetOracleValueAsExactString(reader, i);
                            continue;
                        }

                        try
                        {
                            row[i] = reader.GetValue(i);
                        }
                        catch (Exception ex) when (ex is OverflowException || ex is InvalidCastException)
                        {
                            var colName = i < table.Columns.Count ? table.Columns[i].ColumnName : "?";
                            _logger.LogWarning(
                                "Oracle extraction overflow on non-NumericText column #{ColIdx} '{ColName}': {Error}. " +
                                "Setting to NULL. Mark this mapping column as NumericText to preserve oversized NUMBER values.",
                                i, colName, ex.Message);
                            row[i] = DBNull.Value;
                        }
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
        catch (OracleException ex)
        {
            // Wrap with query context — never swallow. Caller (Sync Engine) logs + per-table rollback.
            throw new InvalidOperationException(
                $"Oracle extraction failed. ORA-{ex.Number}: {ex.Message}. " +
                $"Query context: '{Truncate(oracleSql, 200)}'.", ex);
        }
    }

    /// <summary>
    /// Builds a <see cref="DataTable"/> from a schema table returned by
    /// <see cref="System.Data.Common.DbDataReader.GetSchemaTable()"/>.
    /// Each row in the schema table describes one result-set column.
    /// </summary>
    private static DataTable BuildDataTableFromSchema(
        DataTable? schemaTable,
        IReadOnlySet<string>? numericTextColumns)
    {
        var table = new DataTable();
        if (schemaTable is null) return table;

        var colNameCol = schemaTable.Columns["ColumnName"];
        var dataTypeCol = schemaTable.Columns["DataType"];
        var allowDBNullCol = schemaTable.Columns["AllowDBNull"];

        foreach (System.Data.DataRow schemaRow in schemaTable.Rows)
        {
            var name = colNameCol is not null ? schemaRow[colNameCol] as string ?? "Column" + table.Columns.Count : "Column" + table.Columns.Count;
            var type = IsNumericTextColumn(name, numericTextColumns)
                ? typeof(string)
                : dataTypeCol is not null ? schemaRow[dataTypeCol] as Type ?? typeof(string) : typeof(string);
            var allowNull = allowDBNullCol is not null && schemaRow[allowDBNullCol] is bool b && b;
            table.Columns.Add(new DataColumn(name, type) { AllowDBNull = allowNull });
        }

        return table;
    }

    private static bool IsNumericTextColumn(string columnName, IReadOnlySet<string>? numericTextColumns)
    {
        return numericTextColumns is not null && numericTextColumns.Contains(columnName);
    }

    private static string GetOracleValueAsExactString(OracleDataReader reader, int ordinal)
    {
        var value = reader.GetOracleValue(ordinal);
        return value switch
        {
            OracleDecimal oracleDecimal => oracleDecimal.ToString(),
            INullable { IsNull: true } => string.Empty,
            _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
        };
    }

    /// <summary>
    /// Read-only guard: only the leading statement may begin with SELECT or WITH (CTE).
    /// Leading whitespace and SQL comments (-- and /* */) are tolerated.
    /// This intentionally inspects only the FIRST statement keyword to avoid false
    /// positives on column/data containing words like "update".
    /// </summary>
    private static bool IsReadOnlyQuery(string sql)
    {
        var trimmed = StripLeadingComments(sql).TrimStart();
        if (trimmed.Length == 0)
            return false;

        var firstWord = GetFirstWord(trimmed);
        return firstWord.Equals("SELECT", StringComparison.OrdinalIgnoreCase)
            || firstWord.Equals("WITH", StringComparison.OrdinalIgnoreCase);
    }

    private static string StripLeadingComments(string sql)
    {
        var result = sql;
        while (true)
        {
            result = result.TrimStart();
            if (result.StartsWith("--", StringComparison.Ordinal))
            {
                var newline = result.IndexOf('\n');
                result = newline >= 0 ? result[(newline + 1)..] : string.Empty;
                continue;
            }

            if (result.StartsWith("/*", StringComparison.Ordinal))
            {
                var end = result.IndexOf("*/", StringComparison.Ordinal);
                result = end >= 0 ? result[(end + 2)..] : string.Empty;
                continue;
            }

            break;
        }

        return result;
    }

    private static string GetFirstWord(string text)
    {
        var end = text.IndexOfAny(new[] { ' ', '\t', '\n', '\r', '(', ';' });
        return end < 0 ? text : text[..end];
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
