using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models.Dto;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Service for report layout management (via EF Core).
/// Extracted from ReportService.cs during refactoring (TASK-FIX-REFACTOR-001).
/// </summary>
public class ReportLayoutService
{
    private readonly ILogger<ReportLayoutService> _logger;
    private readonly WarehouseDashboardDbContext _db;

    public ReportLayoutService(
        ILogger<ReportLayoutService> logger,
        WarehouseDashboardDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Saves a new layout for a report.
    /// </summary>
    public async Task<int> SaveLayoutAsync(int reportId, ReportLayoutSaveRequest request, CancellationToken ct = default)
    {
        var layout = new WarehouseDashboard.Web.Models.ReportLayout
        {
            ReportId = reportId,
            LayoutName = request.LayoutName,
            IsDefault = request.IsDefault,
            ColumnOrder = request.ColumnOrder,
            VisibleColumns = request.VisibleColumns,
            ColumnWidths = request.ColumnWidths,
            FilterValues = request.FilterValues,
            SortState = request.SortState,
            CreatedAt = DateTime.UtcNow
        };

        // If this is the default, unmark other defaults for this report
        if (request.IsDefault)
        {
            var existingDefaults = await _db.ReportLayouts
                .Where(l => l.ReportId == reportId && l.IsDefault)
                .ToListAsync(ct);
            foreach (var d in existingDefaults)
                d.IsDefault = false;
        }

        _db.ReportLayouts.Add(layout);
        await _db.SaveChangesAsync(ct);
        return layout.Id;
    }

    /// <summary>
    /// Gets all layouts for a report.
    /// </summary>
    public async Task<List<ReportLayoutDto>> GetLayoutsAsync(int reportId, CancellationToken ct = default)
    {
        return await _db.ReportLayouts
            .Where(l => l.ReportId == reportId)
            .OrderByDescending(l => l.IsDefault)
            .ThenByDescending(l => l.CreatedAt)
            .Select(l => new ReportLayoutDto
            {
                Id = l.Id,
                LayoutName = l.LayoutName,
                IsDefault = l.IsDefault,
                ColumnOrder = l.ColumnOrder,
                VisibleColumns = l.VisibleColumns,
                ColumnWidths = l.ColumnWidths,
                FilterValues = l.FilterValues,
                SortState = l.SortState
            })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Deletes a saved layout.
    /// </summary>
    public async Task<bool> DeleteLayoutAsync(int layoutId, CancellationToken ct = default)
    {
        var layout = await _db.ReportLayouts.FindAsync(new object[] { layoutId }, ct);
        if (layout is null) return false;

        _db.ReportLayouts.Remove(layout);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
