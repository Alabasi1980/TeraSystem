using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Text.Json;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.OracleQueryLab;

/// <summary>
/// Admin Oracle Query Lab (TASK-COD-031).
///
/// Lets an authenticated admin write, test, and validate Oracle SQL queries
/// before assigning them to dashboard cards. The query is executed READ-ONLY
/// against Oracle via Oracle.ManagedDataAccess and the connection string is
/// resolved from configuration via <see cref="ConnectionStringHelper.ResolveOracle"/>.
///
/// The <see cref="OracleReadonlyGuard"/> runs server-side before execution so
/// that no INSERT/UPDATE/DELETE/MERGE/DDL can ever run through this endpoint.
/// </summary>
public class OracleQueryLabModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OracleQueryLabModel> _logger;

    /// <summary>Maximum rows returned by the query executor.</summary>
    private const int MaxRows = 10_000;

    public OracleQueryLabModel(IConfiguration configuration, ILogger<OracleQueryLabModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    /// <summary>Health check: tests the Oracle connection with SELECT 1 FROM DUAL (5s timeout).</summary>
    public async Task<IActionResult> OnGetHealthAsync()
    {
        try
        {
            var connectionString = ConnectionStringHelper.ResolveOracle(_configuration);
            if (string.IsNullOrEmpty(connectionString))
                return Json(new { connected = false });

            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();
            await using var cmd = new OracleCommand("SELECT 1 FROM DUAL", connection);
            cmd.CommandTimeout = 5;
            await cmd.ExecuteScalarAsync();
            return Json(new { connected = true });
        }
        catch
        {
            return Json(new { connected = false });
        }
    }

    /// <summary>Executes a read-only Oracle query and returns the result grid (dynamic columns).</summary>
    public async Task<IActionResult> OnPostRunAsync([FromBody] OracleQueryRunRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Sql))
        {
            return Json(new { success = false, errorMessage = "الرجاء إدخال استعلام Oracle." });
        }

        if (!OracleReadonlyGuard.IsReadOnly(request.Sql, out var guardReason))
        {
            return Json(new { success = false, errorMessage = guardReason });
        }

        var connectionString = ConnectionStringHelper.ResolveOracle(_configuration);
        if (string.IsNullOrEmpty(connectionString))
        {
            return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة بيانات Oracle غير متوفرة حالياً." });
        }

        try
        {
            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new OracleCommand(request.Sql, connection);
            command.CommandTimeout = 30;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await using var reader = await command.ExecuteReaderAsync();
            stopwatch.Stop();

            var columns = new List<OracleColumnInfoResponse>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(new OracleColumnInfoResponse
                {
                    Name = reader.GetName(i),
                    DataType = reader.GetFieldType(i).Name
                });
            }

            var rows = new List<Dictionary<string, object?>>();
            int rowCount = 0;
            while (await reader.ReadAsync() && rowCount < MaxRows)
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[columns[i].Name] = NormalizeValue(reader.GetValue(i));
                }
                rows.Add(row);
                rowCount++;
            }

            return Json(new
            {
                success = true,
                columns,
                rows,
                rowCount = rows.Count,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds
            });
        }
        catch (OracleException ex)
        {
            _logger.LogError(ex, "Oracle Query Lab execution failed (Oracle error).");
            return Json(new
            {
                success = false,
                errorMessage = $"خطأ Oracle ({ex.Number}): {ex.Message}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oracle Query Lab execution failed.");
            return Json(new
            {
                success = false,
                errorMessage = "حدث خطأ أثناء تنفيذ الاستعلام. يرجى المحاولة لاحقاً."
            });
        }
    }

    /// <summary>Browses the Oracle schema: lists tables and their columns for the current user.</summary>
    public async Task<IActionResult> OnPostSchemaAsync()
    {
        var connectionString = ConnectionStringHelper.ResolveOracle(_configuration);
        if (string.IsNullOrEmpty(connectionString))
        {
            return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة بيانات Oracle غير متوفرة حالياً." });
        }

        try
        {
            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();

            // Single query: get all columns grouped by table (replaces N+1).
            await using (var cmd = new OracleCommand(
                "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE " +
                "FROM ALL_TAB_COLUMNS WHERE OWNER = USER ORDER BY TABLE_NAME, COLUMN_ID", connection))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                var tableDict = new Dictionary<string, OracleTableInfo>(StringComparer.OrdinalIgnoreCase);

                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetString(0);
                    if (!tableDict.TryGetValue(tableName, out var tableInfo))
                    {
                        tableInfo = new OracleTableInfo { Name = tableName };
                        tableDict[tableName] = tableInfo;
                    }

                    tableInfo.Columns.Add(new OracleColumnInfo
                    {
                        Name = reader.GetString(1),
                        DataType = reader.GetString(2),
                        DataLength = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                        Nullable = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                    });
                }

                var tables = tableDict.Values.OrderBy(t => t.Name).ToList();
                return Json(new { success = true, tables });
            }
        }
        catch (OracleException ex)
        {
            _logger.LogError(ex, "Oracle Query Lab schema browse failed (Oracle error).");
            return Json(new
            {
                success = false,
                errorMessage = "تعذر استكشاف مخطط قاعدة البيانات. يرجى المحاولة لاحقاً."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oracle Query Lab schema browse failed.");
            return Json(new
            {
                success = false,
                errorMessage = "حدث خطأ أثناء استكشاف مخطط قاعدة البيانات."
            });
        }
    }

    private static object? NormalizeValue(object value)
    {
        if (value is DBNull or null)
        {
            return null;
        }

        if (value is byte[])
        {
            return "<binary>";
        }

        return value;
    }

    /// <summary>Deterministic camelCase JSON response helper.</summary>
    private ContentResult Json(object data)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return new ContentResult
        {
            Content = JsonSerializer.Serialize(data, options),
            ContentType = "application/json; charset=utf-8"
        };
    }
}

/// <summary>Request body for the Run handler.</summary>
public class OracleQueryRunRequest
{
    public string? Sql { get; set; }
}

/// <summary>Column metadata returned to the client for dynamic grid generation.</summary>
public class OracleColumnInfoResponse
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
}

/// <summary>Column metadata for schema browsing.</summary>
public class OracleColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public int? DataLength { get; set; }
    public string Nullable { get; set; } = string.Empty;
}

/// <summary>Table metadata for schema browsing.</summary>
public class OracleTableInfo
{
    public string Name { get; set; } = string.Empty;
    public List<OracleColumnInfo> Columns { get; set; } = new();
}
