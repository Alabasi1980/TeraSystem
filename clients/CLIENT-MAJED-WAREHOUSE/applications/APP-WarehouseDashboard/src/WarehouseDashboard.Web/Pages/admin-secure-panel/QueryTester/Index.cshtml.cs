using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin Query Tester (TASK-COD-010, Objective A).
///
/// Lets an authenticated admin validate the SQL query they intend to assign to a
/// dashboard card. The query is executed READ-ONLY against SQL Server via
/// <see cref="Microsoft.Data.SqlClient"/> and the connection string is resolved
/// from configuration with the {SQL_PASSWORD} placeholder substituted from the
/// SQL_PASSWORD environment variable (never stored in source or config).
///
/// The <see cref="SqlReadonlyGuard"/> runs server-side before execution so that
/// no INSERT/UPDATE/DELETE/MERGE/DDL can ever run through this endpoint.
/// </summary>
public class QueryTesterModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueryTesterModel> _logger;

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

        if (!SqlReadonlyGuard.IsReadOnly(request.Sql, out var guardReason))
        {
            return Json(new { success = false, errorMessage = guardReason });
        }

        var connectionString = ConnectionStringHelper.Resolve(_configuration.GetConnectionString("SqlServer"));
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
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[columns[i].Name] = NormalizeValue(reader.GetValue(i));
                }
                rows.Add(row);
            }

            await reader.CloseAsync();

            return Json(new
            {
                success = true,
                columns,
                rows,
                rowCount = rows.Count,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds
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
public class QueryRunRequest
{
    public string? Sql { get; set; }
}

/// <summary>Column metadata returned to the client for dynamic grid generation.</summary>
public class ColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
}
