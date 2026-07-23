using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;
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

        var result = await ExecuteDrillQueryAsync(cardId, level, parentValue, cancellationToken);
        return Json(result);
    }

    /// <summary>
    /// GET /api/dashboard/drill/{cardId}/{level}/export?parentValue=...
    /// Returns a formatted .xlsx file for the drill-down data.
    /// </summary>
    public async Task<IActionResult> OnGetExcelAsync(int cardId, int level, string? parentValue, CancellationToken cancellationToken)
    {
        DrillDataResult data;
        try
        {
            data = await ExecuteDrillQueryAsync(cardId, level, parentValue, cancellationToken);
        }
        catch (Exception)
        {
            return new ContentResult
            {
                Content = "{\"success\":false,\"errorMessage\":\"حدث خطأ أثناء تصدير البيانات.\"}",
                ContentType = "application/json"
            };
        }

        if (data.Status != "success")
        {
            return new ContentResult
            {
                Content = "{\"success\":false,\"errorMessage\":\"" + (data.ErrorMessage ?? "لا توجد بيانات للتصدير.") + "\"}",
                ContentType = "application/json"
            };
        }

        // Build Excel using ClosedXML
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Drill Data");
        ws.RightToLeft = true;

        var colCount = data.Columns.Count;
        var rowCount = data.Rows.Count;

        // Header
        for (int i = 0; i < colCount; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = data.Columns[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E79");
            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetOutsideBorderColor(XLColor.FromHtml("#D4E2F0"));
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Data rows
        for (int r = 0; r < rowCount; r++)
        {
            var row = data.Rows[r];
            for (int c = 0; c < colCount; c++)
            {
                var colName = data.Columns[c];
                row.TryGetValue(colName, out var val);
                var cell = ws.Cell(r + 2, c + 1);

                if (val == null)
                {
                    cell.Value = "";
                }
                else if (val is int || val is long || val is short || val is byte)
                {
                    cell.Value = Convert.ToInt64(val);
                    cell.Style.NumberFormat.Format = "#,##0";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else if (val is decimal || val is double || val is float)
                {
                    cell.Value = Convert.ToDouble(val);
                    cell.Style.NumberFormat.Format = "#,##0.00";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else if (val is DateTime dt)
                {
                    cell.Value = dt;
                    cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                }
                else
                {
                    cell.Value = val.ToString() ?? "";
                }

                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                cell.Style.Border.SetOutsideBorderColor(XLColor.FromHtml("#E0E0E0"));
            }
        }

        // AutoFilter
        if (rowCount > 0 && colCount > 0)
        {
            ws.Range(1, 1, 1 + rowCount, colCount).SetAutoFilter();
        }

        // Column widths
        ws.Columns().AdjustToContents(1, 100);
        ws.Columns().Width = 12; // uniform width
        ws.Column(1).Width = 5; // row number column won't exist, use default

        // Freeze header
        ws.SheetView.FreezeRows(1);

        // Return file
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        var fileName = $"Drill_{cardId}_Level_{level}_{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>
    /// Executes the drill-down query for the given card/level/parentValue and returns a
    /// <see cref="DrillDataResult"/> with the raw data. Shared by both JSON and Excel handlers.
    /// </summary>
    private async Task<DrillDataResult> ExecuteDrillQueryAsync(int cardId, int level, string? parentValue, CancellationToken ct)
    {
        var card = await _db.DashboardCards.FindAsync(new object[] { cardId }, ct);
        if (card is null)
        {
            return new DrillDataResult
            {
                CardId = cardId,
                Level = level,
                Status = "error",
                ErrorMessage = "البطاقة غير موجودة أو غير نشطة."
            };
        }

        var config = await _db.CardDrillDownLevels
            .Where(l => l.ParentCardId == cardId && l.Level == level)
            .OrderBy(l => l.Level)
            .FirstOrDefaultAsync(ct);

        if (config is null)
        {
            // Level not configured — not an error per se, just nothing to show here.
            return new DrillDataResult
            {
                CardId = cardId,
                CardTitle = card.Title,
                Level = level,
                Status = "none",
                ErrorMessage = "لا يوجد مستوى تعمّق بهذا الترتيب."
            };
        }

        var nextLevel = await _db.CardDrillDownLevels
            .Where(l => l.ParentCardId == cardId && l.Level == level + 1)
            .Select(l => new { l.RequiresParentValue })
            .FirstOrDefaultAsync(ct);

        var hasNext = nextLevel != null;

        var result = new DrillDataResult
        {
            CardId = cardId,
            CardTitle = card.Title,
            Level = level,
            DisplayName = config.DisplayName,
            ChartType = config.TargetChartType,
            ParameterColumn = config.ParameterColumn,
            LabelColumn = config.LabelColumn,
            ColumnAliases = config.ColumnAliases,
            HasNextLevel = hasNext,
            NextRequiresParentValue = nextLevel?.RequiresParentValue ?? false,
            Status = "error"
        };

        // If THIS level requires a parent value but none was provided → error out
        // (Level 1 typically has RequiresParentValue = false)
        if (config.RequiresParentValue && string.IsNullOrWhiteSpace(parentValue))
        {
            result.Status = "error";
            result.ErrorMessage = "هذا المستوى يتطلب قيمة من المستوى السابق. يرجى اختيار عنصر من المستوى السابق.";
            return result;
        }

        try
        {
            var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
            var connString = ConnectionStringHelper.Resolve(connTemplate);

            if (string.IsNullOrWhiteSpace(connString))
            {
                result.ErrorMessage = "لم يتم ضبط سلسلة الاتصال بقاعدة البيانات (SQL_PASSWORD).";
                return result;
            }

            var sql = config.DrillDownQuery;

            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 60 };

            // SAFE parameter binding: only @p0 is ever bound, via SqlParameter.
            if (sql.Contains("@p0", StringComparison.OrdinalIgnoreCase))
            {
                cmd.Parameters.Add(new SqlParameter("@p0", SqlParamValue(parentValue)));
            }

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var colCount = reader.FieldCount;
            var columns = new List<string>(colCount);
            for (var i = 0; i < colCount; i++)
            {
                columns.Add(reader.GetName(i) is { Length: > 0 } name ? name : $"Column{i + 1}");
            }
            result.Columns = columns;

            // Validate ParameterColumn references a real column (when specified)
            if (!string.IsNullOrWhiteSpace(config.ParameterColumn))
            {
                var parameterColumnFound = columns.Exists(
                    c => string.Equals(c, config.ParameterColumn, StringComparison.OrdinalIgnoreCase));
                if (!parameterColumnFound)
                {
                    result.Status = "error";
                    result.ErrorMessage = $"عمود الباراميتر المحدد '{config.ParameterColumn}' غير موجود في نتيجة الاستعلام. الأعمدة المتاحة: {string.Join(", ", columns)}";
                    return result;
                }
            }

            // Validate LabelColumn references a real column (when specified)
            if (!string.IsNullOrWhiteSpace(config.LabelColumn))
            {
                var labelColumnFound = columns.Exists(
                    c => string.Equals(c, config.LabelColumn, StringComparison.OrdinalIgnoreCase));
                if (!labelColumnFound)
                {
                    result.Status = "error";
                    result.ErrorMessage = $"عمود التسمية المحدد '{config.LabelColumn}' غير موجود في نتيجة الاستعلام. الأعمدة المتاحة: {string.Join(", ", columns)}";
                    return result;
                }
            }

            // Apply column aliases if configured (after validation so original names are used for checks)
            if (!string.IsNullOrWhiteSpace(config.ColumnAliases))
            {
                var aliasPairs = config.ColumnAliases.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var pair in aliasPairs)
                {
                    var eqIndex = pair.IndexOf('=');
                    if (eqIndex > 0)
                    {
                        var colName = pair[..eqIndex].Trim();
                        var alias = pair[(eqIndex + 1)..].Trim();
                        if (!string.IsNullOrWhiteSpace(colName) && !string.IsNullOrWhiteSpace(alias))
                        {
                            // Find matching column (case-insensitive) and replace it
                            for (var i = 0; i < result.Columns.Count; i++)
                            {
                                if (string.Equals(result.Columns[i], colName, StringComparison.OrdinalIgnoreCase))
                                {
                                    result.Columns[i] = alias;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Cap payloads so a single heavy query can't break the page.
            var rowLimit = config.TargetChartType.Equals("Table", StringComparison.OrdinalIgnoreCase) ? 500 : 200;
            var rows = new List<Dictionary<string, object?>>(rowLimit);
            var count = 0;
            while (count < rowLimit && await reader.ReadAsync(ct))
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
                return result;
            }

            result.KpiValue = rows[0].Values.FirstOrDefault();
            result.Status = "success";
        }
        catch (Exception ex)
        {
            result.Status = "error";
            result.ErrorMessage = Sanitize(ex.Message);
        }

        return result;
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
