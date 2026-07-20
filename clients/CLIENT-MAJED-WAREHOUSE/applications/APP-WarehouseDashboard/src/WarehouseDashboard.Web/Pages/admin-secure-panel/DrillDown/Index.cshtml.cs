using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin Drill-Down Configuration (TASK-COD-010, Objective B).
///
/// Manage the <see cref="CardDrillDownLevel"/> rows for a selected
/// <see cref="DashboardCard"/>. Levels are persisted through
/// <see cref="WarehouseDashboardDbContext"/>; the unique index
/// (ParentCardId, Level) is enforced and surfaced as a friendly error.
/// </summary>
public class DrillDownModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ILogger<DrillDownModel> _logger;
    private readonly IConfiguration _config;

    public DrillDownModel(WarehouseDashboardDbContext db, ILogger<DrillDownModel> logger, IConfiguration config)
    {
        _db = db;
        _logger = logger;
        _config = config;
    }

    /// <summary>Cards available in the selector (populated on GET).</summary>
    public List<CardOption> Cards { get; set; } = new();

    public void OnGet()
    {
        Cards = _db.DashboardCards
            .OrderBy(c => c.Title)
            .Select(c => new CardOption(c.Id, c.Title))
            .ToList();
    }

    /// <summary>Returns the drill-down levels for a card as JSON.</summary>
    public async Task<IActionResult> OnGetLevelsAsync(int cardId)
    {
        if (cardId <= 0)
        {
            return Json(new { success = false, errorMessage = "معرّف البطاقة غير صالح." });
        }

        try
        {
            var levels = await _db.CardDrillDownLevels
                .Where(l => l.ParentCardId == cardId)
                .OrderBy(l => l.Level)
                .Select(l => new LevelDto(l.Id, l.Level, l.DisplayName, l.TargetChartType, l.DrillDownQuery,
                    l.ParameterColumn, l.LabelColumn, l.ColumnAliases, l.RequiresParentValue))
                .ToListAsync();

            return Json(new { success = true, levels });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load drill-down levels for card {CardId}.", cardId);
            return Json(new { success = false, errorMessage = "تعذر تحميل المستويات. يرجى المحاولة لاحقاً." });
        }
    }

    /// <summary>Creates or updates a drill-down level.</summary>
    public async Task<IActionResult> OnPostSaveAsync(
        int parentCardId,
        int level,
        string displayName,
        string targetChartType,
        string drillDownQuery,
        int? id,
        string? parameterColumn,
        string? labelColumn,
        bool requiresParentValue,
        string? columnAliases)
    {
        if (parentCardId <= 0)
        {
            return Json(new { success = false, errorMessage = "الرجاء اختيار بطاقة." });
        }

        if (level < 1)
        {
            return Json(new { success = false, errorMessage = "المستوى يجب أن يكون رقماً 1 أو أكثر." });
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Json(new { success = false, errorMessage = "الرجاء إدخال اسم العرض." });
        }

        var allowedTypes = new[] { "Bar", "Line", "Pie", "KPI", "Table", "Gauge" };
        if (!allowedTypes.Contains(targetChartType))
        {
            return Json(new { success = false, errorMessage = "نوع الرسم غير صالح." });
        }

        if (string.IsNullOrWhiteSpace(drillDownQuery))
        {
            return Json(new { success = false, errorMessage = "الرجاء إدخال استعلام SQL للتنقّل العميق." });
        }

        var card = await _db.DashboardCards.FindAsync(parentCardId);
        if (card is null)
        {
            return Json(new { success = false, errorMessage = "البطاقة المحددة غير موجودة." });
        }

        CardDrillDownLevel? entity;
        if (id.HasValue && id.Value > 0)
        {
            entity = await _db.CardDrillDownLevels.FindAsync(id.Value);
            if (entity is null)
            {
                return Json(new { success = false, errorMessage = "مستوى التنقّل غير موجود." });
            }
        }
        else
        {
            entity = new CardDrillDownLevel();
            _db.CardDrillDownLevels.Add(entity);
        }

        entity.ParentCardId = parentCardId;
        entity.Level = level;
        entity.DisplayName = displayName.Trim();
        entity.TargetChartType = targetChartType;
        entity.DrillDownQuery = drillDownQuery;
        entity.ParameterColumn = parameterColumn?.Trim();
        entity.LabelColumn = labelColumn?.Trim();
        entity.ColumnAliases = columnAliases?.Trim();
        entity.RequiresParentValue = requiresParentValue;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueLevelViolation(ex))
        {
            return Json(new
            {
                success = false,
                errorMessage = $"المستوى {level} موجود بالفعل لهذه البطاقة. استخدم رقم مستوى مختلفاً."
            });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to save drill-down level for card {CardId}.", parentCardId);
            return Json(new { success = false, errorMessage = "تعذر حفظ البيانات. يرجى المحاولة لاحقاً." });
        }

        return Json(new { success = true, id = entity.Id, level = entity.Level });
    }

    /// <summary>Deletes a drill-down level.</summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (id <= 0)
        {
            return Json(new { success = false, errorMessage = "معرّف المستوى غير صالح." });
        }

        try
        {
            var entity = await _db.CardDrillDownLevels.FindAsync(id);
            if (entity is null)
            {
                return Json(new { success = false, errorMessage = "مستوى التنقّل غير موجود." });
            }

            _db.CardDrillDownLevels.Remove(entity);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to delete drill-down level {Id}.", id);
            return Json(new { success = false, errorMessage = "تعذر حذف البيانات. يرجى المحاولة لاحقاً." });
        }
    }

    /// <summary>
    /// POST ?handler=TestQuery
    /// Safely executes a Drill Down SQL query for testing purposes.
    /// Validates: SELECT/WITH only, @p0 via SqlParameter, max 100 rows,
    /// 30s timeout, error sanitization, ParameterColumn/LabelColumn validation.
    /// </summary>
    public async Task<IActionResult> OnPostTestQueryAsync(
        string drillDownQuery,
        string? parameterColumn,
        string? labelColumn,
        string? testParameterValue)
    {
        // 1. Validate query is SELECT or WITH
        var trimmedSql = (drillDownQuery ?? string.Empty).TrimStart();
        if (!trimmedSql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) &&
            !trimmedSql.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
        {
            return Json(new
            {
                success = false,
                errorMessage = "يُسمح فقط باستعلامات SELECT أو WITH لأسباب أمنية."
            });
        }

        if (string.IsNullOrWhiteSpace(trimmedSql))
        {
            return Json(new { success = false, errorMessage = "الرجاء إدخال استعلام SQL." });
        }

        // 2. Resolve connection string
        var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
        var connString = ConnectionStringHelper.Resolve(connTemplate);

        if (string.IsNullOrWhiteSpace(connString))
        {
            return Json(new { success = false, errorMessage = "لم يتم ضبط سلسلة الاتصال بقاعدة البيانات." });
        }

        try
        {
            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(trimmedSql, conn)
            {
                CommandTimeout = 30 // max 30 seconds for test queries
            };

            // Only @p0 is ever bound, via SqlParameter
            if (trimmedSql.Contains("@p0", StringComparison.OrdinalIgnoreCase))
            {
                cmd.Parameters.Add(new SqlParameter("@p0", SqlParamValue(testParameterValue)));
            }

            await using var reader = await cmd.ExecuteReaderAsync();

            // Build column schema
            var colCount = reader.FieldCount;
            var columns = new List<string>(colCount);
            for (var i = 0; i < colCount; i++)
            {
                columns.Add(reader.GetName(i) is { Length: > 0 } name ? name : $"Column{i + 1}");
            }

            // Validate ParameterColumn exists in result
            var warnings = new List<string>();
            if (!string.IsNullOrWhiteSpace(parameterColumn))
            {
                var found = columns.Exists(c => string.Equals(c, parameterColumn, StringComparison.OrdinalIgnoreCase));
                if (!found)
                {
                    warnings.Add($"⚠ عمود الباراميتر '{parameterColumn}' غير موجود في النتيجة. الأعمدة المتاحة: {string.Join(", ", columns)}");
                }
            }

            // Validate LabelColumn exists in result
            if (!string.IsNullOrWhiteSpace(labelColumn))
            {
                var found = columns.Exists(c => string.Equals(c, labelColumn, StringComparison.OrdinalIgnoreCase));
                if (!found)
                {
                    warnings.Add($"⚠ عمود التسمية '{labelColumn}' غير موجود في النتيجة.");
                }
            }

            // Read max 100 rows
            var maxRows = 100;
            var rows = new List<Dictionary<string, object?>>(maxRows);
            var count = 0;
            while (count < maxRows && await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>(colCount, StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < colCount; i++)
                {
                    row[columns[i]] = ConvertCell(reader.GetValue(i));
                }
                rows.Add(row);
                count++;
            }

            return Json(new
            {
                success = true,
                columns,
                rows,
                rowCount = count,
                warnings = warnings.Count > 0 ? warnings : null
            });
        }
        catch (Exception ex)
        {
            var safeMessage = Sanitize(ex.Message);
            return Json(new { success = false, errorMessage = safeMessage });
        }
    }

    private static bool IsUniqueLevelViolation(DbUpdateException ex)
    {
        // SQL Server: 2601 = duplicate key, 2627 = unique constraint violation.
        if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
        {
            return sqlEx.Number is 2601 or 2627;
        }

        return false;
    }

    /// <summary>
    /// Converts the incoming string parameter to a typed SQL value so numeric warehouse keys
    /// are bound as numbers (not quoted strings). Falls back to <see cref="DBNull"/> when null.
    /// </summary>
    private static object SqlParamValue(string? raw)
    {
        if (raw is null) return DBNull.Value;
        if (int.TryParse(raw, out var i)) return i;
        if (long.TryParse(raw, out var l)) return l;
        if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
        if (bool.TryParse(raw, out var b)) return b;
        return raw;
    }

    /// <summary>Converts a DataReader cell into a JSON-friendly value.</summary>
    private static object? ConvertCell(object value)
    {
        if (value is DBNull or null) return null;
        if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm",
            System.Globalization.CultureInfo.InvariantCulture);
        if (value is byte[] bytes) return Convert.ToBase64String(bytes);
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
            cleaned = cleaned.Replace(password, "***", StringComparison.Ordinal);
        return cleaned;
    }

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

/// <summary>Option for the card selector.</summary>
public class CardOption
{
    public int Id { get; }
    public string Title { get; }

    public CardOption(int id, string title)
    {
        Id = id;
        Title = title;
    }
}

/// <summary>Level payload returned to the client grid.</summary>
public class LevelDto
{
    public int Id { get; }
    public int Level { get; }
    public string DisplayName { get; }
    public string TargetChartType { get; }
    public string DrillDownQuery { get; }
    public string? ParameterColumn { get; }
    public string? LabelColumn { get; }
    public string? ColumnAliases { get; }
    public bool RequiresParentValue { get; }

    public LevelDto(int id, int level, string displayName, string targetChartType, string drillDownQuery,
        string? parameterColumn, string? labelColumn, string? columnAliases, bool requiresParentValue)
    {
        Id = id;
        Level = level;
        DisplayName = displayName;
        TargetChartType = targetChartType;
        DrillDownQuery = drillDownQuery;
        ParameterColumn = parameterColumn;
        LabelColumn = labelColumn;
        ColumnAliases = columnAliases;
        RequiresParentValue = requiresParentValue;
    }
}
