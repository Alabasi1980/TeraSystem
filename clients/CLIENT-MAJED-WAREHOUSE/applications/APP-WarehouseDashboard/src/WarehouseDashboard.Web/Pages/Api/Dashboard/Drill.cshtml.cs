using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Pages.Api.Dashboard;

/// <summary>
/// GET /api/dashboard/drill/{cardId}/{level}?parentValue=...
///
/// Executes a single drill-down level's query via the approved DashboardService pattern
/// (config = EF Core, data = ADO.NET, read-only) and returns a resilient
/// <see cref="DrillDataResult"/>.
///
/// Parameter convention (SECURITY): when the configured query contains a <c>@p0</c> placeholder,
/// the clicked row's first-column value (<c>parentValue</c>) is bound through a
/// <see cref="SqlParameter"/> — NEVER via string concatenation. If the query has no <c>@p0</c>,
/// it is executed verbatim. Chaining to deeper levels uses the same convention.
/// </summary>
public class DrillModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly IConfiguration _config;

    public DrillModel(WarehouseDashboardDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<IActionResult> OnGetAsync(int cardId, int level, string? parentValue, CancellationToken cancellationToken)
    {
        if (level < 1)
        {
            return Json(new DrillDataResult
            {
                CardId = cardId,
                Level = level,
                Status = "error",
                ErrorMessage = "رقم المستوى غير صالح."
            });
        }

        var card = await _db.DashboardCards.FindAsync(new object[] { cardId }, cancellationToken);
        if (card is null)
        {
            return Json(new DrillDataResult
            {
                CardId = cardId,
                Level = level,
                Status = "error",
                ErrorMessage = "البطاقة غير موجودة أو غير نشطة."
            });
        }

        var config = await _db.CardDrillDownLevels
            .Where(l => l.ParentCardId == cardId && l.Level == level)
            .OrderBy(l => l.Level)
            .FirstOrDefaultAsync(cancellationToken);

            if (config is null)
            {
                // Level not configured — not an error per se, just nothing to show here.
                return Json(new DrillDataResult
                {
                    CardId = cardId,
                    CardTitle = card.Title,
                    Level = level,
                    Status = "none",
                    ErrorMessage = "لا يوجد مستوى تعمّق بهذا الترتيب."
                });
            }

        var hasNext = await _db.CardDrillDownLevels
            .AnyAsync(l => l.ParentCardId == cardId && l.Level == level + 1, cancellationToken);

        var result = new DrillDataResult
        {
            CardId = cardId,
            CardTitle = card.Title,
            Level = level,
            DisplayName = config.DisplayName,
            ChartType = config.TargetChartType,
            HasNextLevel = hasNext,
            Status = "error"
        };

        try
        {
            var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
            var connString = ConnectionStringHelper.Resolve(connTemplate);

            if (string.IsNullOrWhiteSpace(connString))
            {
                result.ErrorMessage = "لم يتم ضبط سلسلة الاتصال بقاعدة البيانات (SQL_PASSWORD).";
                return Json(result);
            }

            var sql = config.DrillDownQuery;

            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 60 };

            // SAFE parameter binding: only @p0 is ever bound, via SqlParameter.
            if (sql.Contains("@p0", StringComparison.OrdinalIgnoreCase))
            {
                cmd.Parameters.Add(new SqlParameter("@p0", SqlParamValue(parentValue)));
            }

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var colCount = reader.FieldCount;
            var columns = new List<string>(colCount);
            for (var i = 0; i < colCount; i++)
            {
                columns.Add(reader.GetName(i) is { Length: > 0 } name ? name : $"Column{i + 1}");
            }
            result.Columns = columns;

            // Cap payloads so a single heavy query can't break the page.
            var rowLimit = config.TargetChartType.Equals("Table", StringComparison.OrdinalIgnoreCase) ? 500 : 200;
            var rows = new List<Dictionary<string, object?>>(rowLimit);
            var count = 0;
            while (count < rowLimit && await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>(colCount, StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < colCount; i++)
                {
                    row[columns[i]] = ConvertCell(reader.GetValue(i));
                }
                rows.Add(row);
                count++;
            }
            result.Rows = rows;

            if (rows.Count == 0)
            {
                result.Status = "empty";
                return Json(result);
            }

            result.KpiValue = rows[0].Values.FirstOrDefault();
            result.Status = "success";
        }
        catch (Exception ex)
        {
            result.Status = "error";
            result.ErrorMessage = Sanitize(ex.Message);
        }

        return Json(result);
    }

    /// <summary>
    /// Serialises <paramref name="data"/> to JSON using the camelCase policy so the
    /// client (which reads <c>data.columns</c>, <c>data.status</c>, …) matches the payload.
    /// Mirrors the admin DrillDown API's <c>Json</c> helper.
    /// </summary>
    private static ContentResult Json(object data)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return new ContentResult
        {
            Content = JsonSerializer.Serialize(data, options),
            ContentType = "application/json; charset=utf-8"
        };
    }

    /// <summary>
    /// Converts the incoming string parameter to a typed SQL value so numeric warehouse keys
    /// are bound as numbers (not quoted strings). Falls back to <see cref="DBNull"/> when null.
    /// </summary>
    private static object SqlParamValue(string? raw)
    {
        if (raw is null)
        {
            return DBNull.Value;
        }

        if (int.TryParse(raw, out var i))
        {
            return i;
        }
        if (long.TryParse(raw, out var l))
        {
            return l;
        }
        if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
        {
            return d;
        }
        if (bool.TryParse(raw, out var b))
        {
            return b;
        }

        return raw;
    }

    /// <summary>Converts a DataReader cell into a JSON-friendly value.</summary>
    private static object? ConvertCell(object value)
    {
        if (value is DBNull or null)
        {
            return null;
        }

        if (value is DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        if (value is byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        return value;
    }

    /// <summary>
    /// Strips any accidental secret leakage (resolved password) from an error message before
    /// it is sent to the browser.
    /// </summary>
    private static string Sanitize(string message)
    {
        var password = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;
        var cleaned = message.Replace("{SQL_PASSWORD}", "***", StringComparison.Ordinal);
        if (password.Length > 0)
        {
            cleaned = cleaned.Replace(password, "***", StringComparison.Ordinal);
        }
        return cleaned;
    }
}
