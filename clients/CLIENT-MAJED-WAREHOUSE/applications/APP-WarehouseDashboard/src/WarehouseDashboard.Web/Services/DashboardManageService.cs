using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Scoped service for CRUD operations on <see cref="Dashboard"/> entities.
/// Handles validation, slug uniqueness, default-dashboard rules, and card-count queries.
/// </summary>
public class DashboardManageService
{
    private readonly WarehouseDashboardDbContext _db;

    public DashboardManageService(WarehouseDashboardDbContext db)
    {
        _db = db;
    }

    /// <summary>Returns all dashboards ordered by SortOrder then Name.</summary>
    public async Task<List<Dashboard>> GetAllAsync()
    {
        return await _db.Dashboards
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .ToListAsync();
    }

    /// <summary>Returns a single dashboard by ID, or null if not found.</summary>
    public async Task<Dashboard?> GetByIdAsync(int id)
    {
        return await _db.Dashboards.FindAsync(id);
    }

    /// <summary>Returns a dashboard by its URL slug (case-insensitive), or null.</summary>
    public async Task<Dashboard?> GetBySlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        return await _db.Dashboards
            .FirstOrDefaultAsync(d => d.Slug == slug.Trim());
    }

    /// <summary>Returns the default dashboard (IsDefault == true), or null.</summary>
    public async Task<Dashboard?> GetDefaultAsync()
    {
        return await _db.Dashboards
            .FirstOrDefaultAsync(d => d.IsDefault);
    }

    /// <summary>
    /// Creates a new dashboard. Auto-sets CreatedAt/UpdatedAt.
    /// Validates slug uniqueness (case-insensitive).
    /// If IsDefault is true, clears the previous default.
    /// </summary>
    public async Task<(bool Success, string? Error)> CreateAsync(Dashboard dashboard)
    {
        dashboard.Name = dashboard.Name?.Trim() ?? string.Empty;
        dashboard.Slug = dashboard.Slug?.Trim() ?? string.Empty;
        dashboard.Description = dashboard.Description?.Trim() ?? string.Empty;
        dashboard.Icon = string.IsNullOrWhiteSpace(dashboard.Icon) ? "\U0001F4CA" : dashboard.Icon.Trim();

        if (string.IsNullOrWhiteSpace(dashboard.Name))
            return (false, "اسم الداشبورد مطلوب.");

        if (string.IsNullOrWhiteSpace(dashboard.Slug))
            return (false, "المعرف (Slug) مطلوب.");

        // Validate slug format: lowercase alphanumeric + hyphens
        if (!IsValidSlug(dashboard.Slug))
            return (false, "المعرف (Slug) يجب أن يحتوي على أحرف إنجليزية صغيرة وأرقام وشرطات فقط.");

        // Slug uniqueness check (case-insensitive)
        var slugExists = await _db.Dashboards.AnyAsync(d => d.Slug == dashboard.Slug);
        if (slugExists)
            return (false, "المعرف (Slug) مستخدم بالفعل. اختر معرفاً آخر.");

        // If setting as default, clear previous default
        if (dashboard.IsDefault)
        {
            var previousDefault = await _db.Dashboards.FirstOrDefaultAsync(d => d.IsDefault);
            if (previousDefault is not null)
            {
                previousDefault.IsDefault = false;
            }
        }

        var now = DateTime.UtcNow;
        dashboard.CreatedAt = now;
        dashboard.UpdatedAt = now;

        _db.Dashboards.Add(dashboard);
        await _db.SaveChangesAsync();

        return (true, null);
    }

    /// <summary>
    /// Updates an existing dashboard. Auto-sets UpdatedAt.
    /// Validates slug uniqueness excluding self.
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateAsync(Dashboard dashboard)
    {
        var existing = await _db.Dashboards.FindAsync(dashboard.Id);
        if (existing is null)
            return (false, "الداشبورد غير موجود.");

        dashboard.Name = dashboard.Name?.Trim() ?? string.Empty;
        dashboard.Slug = dashboard.Slug?.Trim() ?? string.Empty;
        dashboard.Description = dashboard.Description?.Trim() ?? string.Empty;
        dashboard.Icon = string.IsNullOrWhiteSpace(dashboard.Icon) ? "\U0001F4CA" : dashboard.Icon.Trim();

        if (string.IsNullOrWhiteSpace(dashboard.Name))
            return (false, "اسم الداشبورد مطلوب.");

        if (string.IsNullOrWhiteSpace(dashboard.Slug))
            return (false, "المعرف (Slug) مطلوب.");

        if (!IsValidSlug(dashboard.Slug))
            return (false, "المعرف (Slug) يجب أن يحتوي على أحرف إنجليزية صغيرة وأرقام وشرطات فقط.");

        // Slug uniqueness check excluding self
        var slugExists = await _db.Dashboards.AnyAsync(d => d.Slug == dashboard.Slug && d.Id != dashboard.Id);
        if (slugExists)
            return (false, "المعرف (Slug) مستخدم بالفعل. اختر معرفاً آخر.");

        // If setting as default, clear previous default
        if (dashboard.IsDefault && !existing.IsDefault)
        {
            var previousDefault = await _db.Dashboards
                .FirstOrDefaultAsync(d => d.IsDefault && d.Id != dashboard.Id);
            if (previousDefault is not null)
            {
                previousDefault.IsDefault = false;
            }
        }

        existing.Name = dashboard.Name;
        existing.Slug = dashboard.Slug;
        existing.Description = dashboard.Description;
        existing.Icon = dashboard.Icon;
        existing.SortOrder = dashboard.SortOrder;
        existing.IsActive = dashboard.IsActive;
        existing.IsDefault = dashboard.IsDefault;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return (true, null);
    }

    /// <summary>
    /// Deletes a dashboard by ID. Prevents deleting the default dashboard.
    /// Prevents deleting a dashboard that still has cards assigned to it.
    /// </summary>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var dashboard = await _db.Dashboards.FindAsync(id);
        if (dashboard is null)
            return (false, "الداشبورد غير موجود أو سبق حذفه.");

        if (dashboard.IsDefault)
            return (false, "لا يمكن حذف الداشبورد الافتراضي. قم بتعيين داشبورد افتراضي آخر أولاً.");

        var hasCards = await _db.DashboardCards.AnyAsync(c => c.DashboardId == id);
        if (hasCards)
            return (false, "لا يمكن حذف الداشبورد لأنه يحتوي على بطاقات. انقل البطاقات إلى داشبورد آخر أولاً.");

        _db.Dashboards.Remove(dashboard);
        await _db.SaveChangesAsync();

        return (true, null);
    }

    /// <summary>
    /// Reorders dashboards by providing an ordered list of IDs.
    /// Each ID's position in the list becomes its SortOrder.
    /// </summary>
    public async Task ReorderAsync(List<int> orderedIds)
    {
        var dashboards = await _db.Dashboards
            .Where(d => orderedIds.Contains(d.Id))
            .ToListAsync();

        foreach (var dashboard in dashboards)
        {
            var index = orderedIds.IndexOf(dashboard.Id);
            if (index >= 0)
            {
                dashboard.SortOrder = index;
                dashboard.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Returns all dashboards with their card count.
    /// </summary>
    public async Task<List<(Dashboard Dashboard, int CardCount)>> GetAllWithCardCountAsync()
    {
        var results = await _db.Dashboards
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .Select(d => new
            {
                d,
                cardCount = _db.DashboardCards.Count(c => c.DashboardId == d.Id)
            })
            .ToListAsync();

        return results.Select(x => (x.d, x.cardCount)).ToList();
    }

    private static bool IsValidSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        foreach (var c in slug)
        {
            if (!char.IsAsciiLetterLower(c) && !char.IsDigit(c) && c != '-')
                return false;
        }

        return true;
    }
}
