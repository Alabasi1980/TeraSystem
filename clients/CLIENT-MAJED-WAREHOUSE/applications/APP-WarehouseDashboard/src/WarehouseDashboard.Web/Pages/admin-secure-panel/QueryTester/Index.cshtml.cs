using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Text.Json;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin Query Tester (TASK-COD-010, Objective A, enhanced TASK-ENH-QT-001).
///
/// Lets an authenticated admin validate SQL queries they intend to assign to a
/// dashboard card. Supports dual data sources:
///   - SQL Server via <see cref="Microsoft.Data.SqlClient"/>
///   - Oracle via <see cref="Oracle.ManagedDataAccess.Client"/>
///
/// Includes a Schema Browser API to list tables and columns for both sources.
/// Queries are executed READ-ONLY via source-specific guards:
///   <see cref="SqlReadonlyGuard"/> for SQL Server
///   <see cref="OracleReadonlyGuard"/> for Oracle
/// Results are limited to <see cref="MaxRows"/> = 1000 rows with a truncated flag.
/// </summary>
public class QueryTesterModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueryTesterModel> _logger;

    /// <summary>Maximum rows returned by the query executor.</summary>
    private const int MaxRows = 1000;

    public QueryTesterModel(IConfiguration configuration, ILogger<QueryTesterModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    /// <summary>Executes a read-only SQL batch and returns the result grid (dynamic columns).</summary>
    public async Task<IActionResult> OnPostRunAsync([FromBody] QueryRunRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Sql))
        {
            return Json(new { success = false, errorMessage = "الرجاء إدخال استعلام SQL." });
        }

        var source = string.IsNullOrWhiteSpace(request.Source) ? "SqlServer" : request.Source;

        if (source == "SqlServer")
        {
            if (!SqlReadonlyGuard.IsReadOnly(request.Sql, out var guardReason))
            {
                return Json(new { success = false, errorMessage = guardReason });
            }

            var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
            if (string.IsNullOrEmpty(connectionString))
            {
                return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة البيانات غير متوفرة حالياً." });
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand(request.Sql, connection);
                command.CommandTimeout = 30;

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                await using var reader = await command.ExecuteReaderAsync();
                stopwatch.Stop();

                var columns = new List<ColumnInfo>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(new ColumnInfo
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

                await reader.CloseAsync();

                return Json(new
                {
                    success = true,
                    columns,
                    rows,
                    rowCount = rows.Count,
                    elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    truncated = rowCount >= MaxRows
                });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Query Tester execution failed (SQL error).");
                return Json(new
                {
                    success = false,
                    errorMessage = "تعذر تنفيذ الاستعلام. تأكد من صحة الصياغة وأن الجداول والحقول موجودة."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Query Tester execution failed.");
                return Json(new
                {
                    success = false,
                    errorMessage = "حدث خطأ أثناء تنفيذ الاستعلام. يرجى المحاولة لاحقاً."
                });
            }
        }
        else if (source == "Oracle")
        {
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

                var columns = new List<ColumnInfo>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(new ColumnInfo
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
                        try
                        {
                            row[columns[i].Name] = NormalizeValue(reader.GetValue(i));
                        }
                        catch
                        {
                            // If GetValue fails for this specific column, try GetOracleValue fallback
                            try
                            {
                                row[columns[i].Name] = reader.GetOracleValue(i)?.ToString();
                            }
                            catch
                            {
                                row[columns[i].Name] = "<unreadable>";
                            }
                        }
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
                    elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    truncated = rowCount >= MaxRows
                });
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Oracle Query Tester execution failed (Oracle error).");
                return Json(new
                {
                    success = false,
                    errorMessage = $"خطأ Oracle ({ex.Number}): {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oracle Query Tester execution failed.");
                return Json(new
                {
                    success = false,
                    errorMessage = "حدث خطأ أثناء تنفيذ الاستعلام. يرجى المحاولة لاحقاً."
                });
            }
        }
        else
        {
            return Json(new { success = false, errorMessage = $"مصدر البيانات غير معروف: '{source}'. يرجى اختيار SqlServer أو Oracle." });
        }
    }

    /// <summary>Schema browser: lists tables for the given source.</summary>
    public async Task<IActionResult> OnGetTablesAsync([FromQuery] string source)
    {
        var resolved = string.IsNullOrWhiteSpace(source) ? "SqlServer" : source;

        if (resolved == "SqlServer")
        {
            var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
            if (string.IsNullOrEmpty(connectionString))
            {
                return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة البيانات غير متوفرة حالياً." });
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";
                await using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = 15;

                await using var reader = await command.ExecuteReaderAsync();
                var tables = new List<object>();
                while (await reader.ReadAsync())
                {
                    tables.Add(new { schema = reader.GetString(0), tableName = reader.GetString(1) });
                }

                return Json(new { success = true, tables });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Schema table list failed (SQL Server).");
                return Json(new { success = false, errorMessage = "تعذر تحميل قائمة الجداول." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schema table list failed.");
                return Json(new { success = false, errorMessage = "حدث خطأ أثناء تحميل قائمة الجداول." });
            }
        }
        else if (resolved == "Oracle")
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

                const string sql = "SELECT OWNER, TABLE_NAME FROM ALL_TABLES ORDER BY OWNER, TABLE_NAME";
                await using var command = new OracleCommand(sql, connection);
                command.CommandTimeout = 15;

                await using var reader = await command.ExecuteReaderAsync();
                var tables = new List<object>();
                while (await reader.ReadAsync())
                {
                    tables.Add(new { schema = reader.GetString(0), tableName = reader.GetString(1) });
                }

                return Json(new { success = true, tables });
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Schema table list failed (Oracle).");
                return Json(new { success = false, errorMessage = "تعذر تحميل قائمة الجداول." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schema table list failed.");
                return Json(new { success = false, errorMessage = "حدث خطأ أثناء تحميل قائمة الجداول." });
            }
        }
        else
        {
            return Json(new { success = false, errorMessage = $"مصدر البيانات غير معروف: '{resolved}'." });
        }
    }

    /// <summary>Schema browser: lists columns for the given table and source.</summary>
    public async Task<IActionResult> OnGetColumnsAsync([FromQuery] string source, [FromQuery] string table)
    {
        if (string.IsNullOrWhiteSpace(table))
        {
            return Json(new { success = false, errorMessage = "الرجاء تحديد اسم الجدول." });
        }

        var resolved = string.IsNullOrWhiteSpace(source) ? "SqlServer" : source;

        if (resolved == "SqlServer")
        {
            var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
            if (string.IsNullOrEmpty(connectionString))
            {
                return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة البيانات غير متوفرة حالياً." });
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table ORDER BY ORDINAL_POSITION";
                await using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@table", table);
                command.CommandTimeout = 15;

                await using var reader = await command.ExecuteReaderAsync();
                var columns = new List<object>();
                while (await reader.ReadAsync())
                {
                    columns.Add(new { name = reader.GetString(0), dataType = reader.GetString(1), nullable = reader.GetString(2) });
                }

                return Json(new { success = true, columns });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Schema column list failed (SQL Server).");
                return Json(new { success = false, errorMessage = "تعذر تحميل قائمة الأعمدة." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schema column list failed.");
                return Json(new { success = false, errorMessage = "حدث خطأ أثناء تحميل قائمة الأعمدة." });
            }
        }
        else if (resolved == "Oracle")
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

                const string sql = "SELECT COLUMN_NAME, DATA_TYPE, NULLABLE FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = UPPER(:table) ORDER BY COLUMN_ID";
                await using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter("table", table));
                command.CommandTimeout = 15;

                await using var reader = await command.ExecuteReaderAsync();
                var columns = new List<object>();
                while (await reader.ReadAsync())
                {
                    columns.Add(new { name = reader.GetString(0), dataType = reader.GetString(1), nullable = reader.IsDBNull(2) ? string.Empty : reader.GetString(2) });
                }

                return Json(new { success = true, columns });
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Schema column list failed (Oracle).");
                return Json(new { success = false, errorMessage = "تعذر تحميل قائمة الأعمدة." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schema column list failed.");
                return Json(new { success = false, errorMessage = "حدث خطأ أثناء تحميل قائمة الأعمدة." });
            }
        }
        else
        {
            return Json(new { success = false, errorMessage = $"مصدر البيانات غير معروف: '{resolved}'." });
        }
    }

    /// <summary>Schema browser: lists views for the given source.</summary>
    public async Task<IActionResult> OnGetViewsAsync([FromQuery] string source)
    {
        var resolved = string.IsNullOrWhiteSpace(source) ? "SqlServer" : source;

        if (resolved == "SqlServer")
        {
            var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
            if (string.IsNullOrEmpty(connectionString))
                return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة البيانات غير متوفرة حالياً." });

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS ORDER BY TABLE_SCHEMA, TABLE_NAME";
                await using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = 15;

                await using var reader = await command.ExecuteReaderAsync();
                var views = new List<object>();
                while (await reader.ReadAsync())
                {
                    views.Add(new { schema = reader.GetString(0), viewName = reader.GetString(1) });
                }

                return Json(new { success = true, views });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Schema view list failed (SQL Server).");
                return Json(new { success = false, errorMessage = "تعذر تحميل قائمة الفيوز." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schema view list failed.");
                return Json(new { success = false, errorMessage = "حدث خطأ أثناء تحميل قائمة الفيوز." });
            }
        }
        else if (resolved == "Oracle")
        {
            var connectionString = ConnectionStringHelper.ResolveOracle(_configuration);
            if (string.IsNullOrEmpty(connectionString))
                return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة بيانات Oracle غير متوفرة حالياً." });

            try
            {
                await using var connection = new OracleConnection(connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT OWNER, VIEW_NAME FROM ALL_VIEWS ORDER BY OWNER, VIEW_NAME";
                await using var command = new OracleCommand(sql, connection);
                command.CommandTimeout = 15;

                await using var reader = await command.ExecuteReaderAsync();
                var views = new List<object>();
                while (await reader.ReadAsync())
                {
                    views.Add(new { schema = reader.GetString(0), viewName = reader.GetString(1) });
                }

                return Json(new { success = true, views });
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Schema view list failed (Oracle).");
                return Json(new { success = false, errorMessage = "تعذر تحميل قائمة الفيوز." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schema view list failed.");
                return Json(new { success = false, errorMessage = "حدث خطأ أثناء تحميل قائمة الفيوز." });
            }
        }
        else
        {
            return Json(new { success = false, errorMessage = $"مصدر البيانات غير معروف: '{resolved}'." });
        }
    }

    /// <summary>Creates a VIEW in the database from the current query.</summary>
    public async Task<IActionResult> OnPostCreateViewAsync([FromBody] CreateViewRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.ViewName))
            return Json(new { success = false, errorMessage = "الرجاء إدخال اسم الـ View." });
        if (string.IsNullOrWhiteSpace(request.Sql))
            return Json(new { success = false, errorMessage = "الرجاء إدخال استعلام SQL." });

        // Validate view name (alphanumeric + underscore only, must start with letter or underscore)
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.ViewName, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            return Json(new { success = false, errorMessage = "اسم الـ View غير صالح. استخدم أحرف إنجليزية وأرقام و _ فقط، وابدأ بحرف أو _." });

        var source = string.IsNullOrWhiteSpace(request.Source) ? "SqlServer" : request.Source;

        if (source == "SqlServer")
        {
            var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
            if (string.IsNullOrEmpty(connectionString))
                return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة البيانات غير متوفرة حالياً." });

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var createSql = $"CREATE VIEW [{request.ViewName}] AS{Environment.NewLine}{request.Sql}";
                _logger.LogInformation("Creating view: {ViewName}", request.ViewName);

                await using var command = new SqlCommand(createSql, connection);
                command.CommandTimeout = 30;
                await command.ExecuteNonQueryAsync();

                return Json(new { success = true, message = $"تم إنشاء VIEW [{request.ViewName}] بنجاح." });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Create view failed (SQL Server).");
                return Json(new { success = false, errorMessage = $"خطأ SQL: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create view failed.");
                return Json(new { success = false, errorMessage = $"حدث خطأ: {ex.Message}" });
            }
        }
        else if (source == "Oracle")
        {
            var connectionString = ConnectionStringHelper.ResolveOracle(_configuration);
            if (string.IsNullOrEmpty(connectionString))
                return Json(new { success = false, errorMessage = "إعدادات الاتصال بقاعدة بيانات Oracle غير متوفرة حالياً." });

            try
            {
                await using var connection = new OracleConnection(connectionString);
                await connection.OpenAsync();

                var createSql = $"CREATE VIEW {request.ViewName} AS{Environment.NewLine}{request.Sql}";
                _logger.LogInformation("Creating Oracle view: {ViewName}", request.ViewName);

                await using var command = new OracleCommand(createSql, connection);
                command.CommandTimeout = 30;
                await command.ExecuteNonQueryAsync();

                return Json(new { success = true, message = $"تم إنشاء VIEW {request.ViewName} بنجاح." });
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Create view failed (Oracle).");
                return Json(new { success = false, errorMessage = $"خطأ Oracle ({ex.Number}): {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create view failed.");
                return Json(new { success = false, errorMessage = $"حدث خطأ: {ex.Message}" });
            }
        }
        else
        {
            return Json(new { success = false, errorMessage = $"مصدر البيانات غير معروف: '{source}'." });
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

        // OracleDecimal can overflow when cast to .NET decimal.
        // Convert to string first for safe JSON serialization.
        if (value is OracleDecimal oracleDecimal)
        {
            try
            {
                // Try the normal .NET decimal conversion first (works for most values).
                return (decimal)oracleDecimal.Value;
            }
            catch (OverflowException)
            {
                // Value too large for .NET decimal — fall back to string representation.
                return oracleDecimal.ToString();
            }
        }

        // OracleClob — convert to string for safe JSON serialization.
        if (value is OracleClob clob)
        {
            return clob.Value;
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
public class QueryRunRequest
{
    public string? Sql { get; set; }
    public string Source { get; set; } = "SqlServer";
}

/// <summary>Request body for the CreateView handler.</summary>
public class CreateViewRequest
{
    public string? ViewName { get; set; }
    public string? Sql { get; set; }
    public string Source { get; set; } = "SqlServer";
}

/// <summary>Column metadata returned to the client for dynamic grid generation.</summary>
public class ColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
}
