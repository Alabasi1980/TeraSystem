using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Api.Infrastructure;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// The LOAD half of the Sync Engine: bulk-copies an Oracle-extracted <see cref="DataTable"/>
/// into a SQL Server staging table with per-table transactional safety
/// (DELETE + INSERT in a single transaction; rollback on failure).
///
/// Per the integration document (14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md §3, §4) and
/// 06_DATA_MODEL_PREPARATION.md §3.3, the column names from the source <see cref="DataTable"/>
/// (which preserve the Oracle column names produced by <see cref="OracleExtractionService"/>)
/// are matched by name to the target SQL Server columns.
///
/// DEVIATION (documented, pragmatic for Phase 1):
/// The data-model document (rule G5, §5.3) states Data Tables are created manually by the
/// client/DBA. Until the client provides the exact Oracle schemas, this service will
/// CREATE the target table on first load if it does not already exist, deriving the schema
/// from the DataTable's CLR column types (see <see cref="MapClrTypeToSql"/>). Once the
/// client supplies fixed schemas, this auto-create branch can be removed and tables created
/// explicitly (restoring strict compliance with G5).
/// </summary>
public class SqlServerLoadService
{
    private readonly string _connectionString;
    private readonly ILogger<SqlServerLoadService> _logger;

    public SqlServerLoadService(IConfiguration configuration, ILogger<SqlServerLoadService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        _connectionString = ConnectionStringHelper.ResolveSql(configuration);
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException(
                "SQL Server connection string is empty. Ensure 'ConnectionStrings:SqlServer' is configured in " +
                "appsettings.json and the SQL_PASSWORD environment variable is set at runtime.");
        }
    }

    /// <summary>
    /// Full-refresh load of <paramref name="data"/> into <paramref name="targetTable"/>.
    /// Opens a SQL Server connection, creates the target table if missing, then within a
    /// single transaction: DELETEs all existing rows and SqlBulkCopy-inserts the new rows.
    /// On any failure the transaction is rolled back (target left unchanged) and the
    /// exception is rethrown for the caller (Sync Engine) to log and retry.
    /// </summary>
    public async Task LoadTableAsync(string targetTable, DataTable data, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Columns.Count == 0)
            throw new ArgumentException("Source DataTable has no columns; cannot load.", nameof(data));

        var (schema, name) = SplitTable(targetTable);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        // Create-if-not-exists (outside the refresh transaction — DDL).
        await EnsureTableExistsAsync(connection, schema, name, data, ct);

        using var transaction = connection.BeginTransaction();
        try
        {
            // 1) Clear current contents (full refresh).
            using (var deleteCmd = connection.CreateCommand())
            {
                deleteCmd.Transaction = transaction;
                deleteCmd.CommandText = $"DELETE FROM [{schema}].[{name}]";
                var deleted = await deleteCmd.ExecuteNonQueryAsync(ct);
                _logger.LogDebug("Cleared {DeletedRows} existing row(s) from [{Schema}].[{Name}].", deleted, schema, name);
            }

            // 2) Bulk insert the extracted rows (column names matched by name).
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = $"[{schema}].[{name}]",
                BatchSize = 1000
            };

            foreach (DataColumn column in data.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(data, ct);

            transaction.Commit();
            _logger.LogInformation("Loaded {RowCount} row(s) into [{Schema}].[{Name}] (full refresh).",
                data.Rows.Count, schema, name);
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception rbEx)
            {
                _logger.LogError(rbEx, "Rollback failed for [{Schema}].[{Name}].", schema, name);
            }

            throw;
        }
    }

    /// <summary>
    /// Incremental load of <paramref name="data"/> into <paramref name="targetTable"/>.
    /// Unlike <see cref="LoadTableAsync"/> this method does NOT delete existing rows —
    /// it only appends the newly extracted rows via <see cref="SqlBulkCopy"/>.
    /// The target table is created automatically if it does not yet exist.
    /// </summary>
    public async Task LoadTableIncrementalAsync(string targetTable, DataTable data, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Columns.Count == 0)
            throw new ArgumentException("Source DataTable has no columns; cannot load.", nameof(data));

        var (schema, name) = SplitTable(targetTable);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        // Create-if-not-exists (DDL, outside any refresh transaction).
        await EnsureTableExistsAsync(connection, schema, name, data, ct);

        // Bulk insert only — no DELETE. Existing rows are preserved.
        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = $"[{schema}].[{name}]",
            BatchSize = 1000
        };

        foreach (DataColumn column in data.Columns)
        {
            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        }

        await bulkCopy.WriteToServerAsync(data, ct);

        _logger.LogInformation("Loaded {RowCount} row(s) into [{Schema}].[{Name}] (incremental, no delete).",
            data.Rows.Count, schema, name);
    }

    private async Task EnsureTableExistsAsync(
        SqlConnection connection, string schema, string name, DataTable data, CancellationToken ct)
    {
        await using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText =
            "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @name";
        checkCmd.Parameters.Add(new SqlParameter("@schema", schema));
        checkCmd.Parameters.Add(new SqlParameter("@name", name));

        var exists = await checkCmd.ExecuteScalarAsync(ct) != null;
        if (exists)
        {
            _logger.LogDebug("Target table [{Schema}].[{Name}] already exists; skipping CREATE.", schema, name);
            return;
        }

        _logger.LogWarning(
            "Target table [{Schema}].[{Name}] does not exist. Auto-creating from source schema " +
            "(Phase 1 pragmatic deviation from manual-table rule G5).", schema, name);

        var columnDefinitions = new List<string>();
        foreach (DataColumn column in data.Columns)
        {
            if (!IsSafeIdentifier(column.ColumnName))
            {
                throw new InvalidOperationException(
                    $"Column '{column.ColumnName}' from the Oracle source is not a safe identifier and " +
                    "cannot be used to create the target table.");
            }

            var sqlType = MapClrTypeToSql(column.DataType);
            columnDefinitions.Add($"[{column.ColumnName}] {sqlType} NULL");
        }

        var createSql = $"CREATE TABLE [{schema}].[{name}] (\n  " +
                        string.Join(",\n  ", columnDefinitions) + "\n)";

        await using var createCmd = connection.CreateCommand();
        createCmd.CommandText = createSql;
        await createCmd.ExecuteNonQueryAsync(ct);

        _logger.LogInformation("Created target table [{Schema}].[{Name}] with {ColumnCount} column(s).",
            schema, name, columnDefinitions.Count);
    }

    /// <summary>
    /// Maps a CLR <see cref="Type"/> (as resolved by ODP.NET in the source DataTable) to a
    /// SQL Server type per 06_DATA_MODEL_PREPARATION.md §3.3. Unknown types fall back to
    /// NVARCHAR(MAX) (Unicode-safe) and are logged as a warning.
    /// </summary>
    private string MapClrTypeToSql(Type clrType)
    {
        if (clrType == typeof(int) || clrType == typeof(long) || clrType == typeof(short) || clrType == typeof(byte))
            return "BIGINT";
        if (clrType == typeof(decimal))
            return "DECIMAL(18,2)";
        if (clrType == typeof(double) || clrType == typeof(float))
            return "FLOAT";
        if (clrType == typeof(bool))
            return "BIT";
        if (clrType == typeof(DateTime) || clrType == typeof(DateTimeOffset))
            return "DATETIME2";
        if (clrType == typeof(byte[]))
            return "VARBINARY(MAX)";
        if (clrType == typeof(Guid))
            return "UNIQUEIDENTIFIER";
        if (clrType == typeof(string))
            return "NVARCHAR(MAX)";

        _logger.LogWarning(
            "Unrecognized CLR type '{Type}' for column; falling back to NVARCHAR(MAX).", clrType.Name);
        return "NVARCHAR(MAX)";
    }

    /// <summary>
    /// Splits a (possibly schema-qualified) table name and validates both identifiers to
    /// prevent SQL injection via the dynamic DDL/DML. Only [A-Za-z0-9_] are allowed.
    /// </summary>
    private static (string schema, string name) SplitTable(string targetTable)
    {
        if (string.IsNullOrWhiteSpace(targetTable))
            throw new ArgumentException("Target table name must not be null or empty.", nameof(targetTable));

        var parts = targetTable.Split('.', 2);
        var schema = parts.Length == 2 ? parts[0].Trim() : "dbo";
        var name = parts.Length == 2 ? parts[1].Trim() : parts[0].Trim();

        if (!IsSafeIdentifier(schema) || !IsSafeIdentifier(name))
        {
            throw new ArgumentException(
                $"Table name '{targetTable}' contains invalid characters. " +
                "Only letters, digits, and underscores are allowed (optional 'schema.table' form).",
                nameof(targetTable));
        }

        return (schema, name);
    }

    private static bool IsSafeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        foreach (var c in identifier)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
                return false;
        }

        return true;
    }
}
