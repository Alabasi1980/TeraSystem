using Microsoft.Data.SqlClient;

namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Read-only ADO.NET query helper for the AI assistant data layer.
/// Only supports SELECT operations via parameterized queries.
/// No INSERT, UPDATE, DELETE, or schema changes.
/// </summary>
public class ReadOnlyQueryHelper
{
    private readonly string _connectionString;

    public ReadOnlyQueryHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServerReadOnly")
            ?? throw new InvalidOperationException("SqlServerReadOnly connection string not found.");
    }

    /// <summary>
    /// Executes a parameterized SELECT query and returns the results as a list of dictionaries.
    /// Each dictionary represents one row, with column names (case-insensitive) as keys.
    /// </summary>
    /// <param name="sql">The SQL SELECT query to execute.</param>
    /// <param name="parameters">Optional named parameters for the query. Always parameterized.</param>
    /// <returns>A list of rows, each represented as a dictionary of column-name to value.</returns>
    public async Task<List<Dictionary<string, object?>>> QueryAsync(
        string sql, Dictionary<string, object>? parameters = null)
    {
        var results = new List<Dictionary<string, object?>>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.CommandTimeout = 30;

        if (parameters is not null)
        {
            foreach (var kvp in parameters)
            {
                command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
            }
        }

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object value = reader.GetValue(i);
                row[reader.GetName(i)] = value is DBNull ? null! : value;
            }
            results.Add(row);
        }

        return results;
    }
}
