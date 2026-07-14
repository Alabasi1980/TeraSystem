using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WarehouseDashboard.Web.Data;
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

    public DrillDownModel(WarehouseDashboardDbContext db, ILogger<DrillDownModel> logger)
    {
        _db = db;
        _logger = logger;
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
                .Select(l => new LevelDto(l.Id, l.Level, l.DisplayName, l.TargetChartType, l.DrillDownQuery))
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
        int? id)
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

    private static bool IsUniqueLevelViolation(DbUpdateException ex)
    {
        // SQL Server: 2601 = duplicate key, 2627 = unique constraint violation.
        if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
        {
            return sqlEx.Number is 2601 or 2627;
        }

        return false;
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

    public LevelDto(int id, int level, string displayName, string targetChartType, string drillDownQuery)
    {
        Id = id;
        Level = level;
        DisplayName = displayName;
        TargetChartType = targetChartType;
        DrillDownQuery = drillDownQuery;
    }
}
