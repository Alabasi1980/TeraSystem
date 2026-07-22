using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models.Dto;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Service for report CRUD operations (via EF Core).
/// Extracted from ReportService.cs during refactoring (TASK-FIX-REFACTOR-001).
/// </summary>
public class ReportCrudService
{
    private readonly ILogger<ReportCrudService> _logger;
    private readonly WarehouseDashboardDbContext _db;

    public ReportCrudService(
        ILogger<ReportCrudService> logger,
        WarehouseDashboardDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Retrieves all reports with their basic info (no columns/filters/layouts).
    /// Returns only enabled and active reports ordered by SortOrder.
    /// </summary>
    public async Task<List<ReportListItem>> GetAllReportsAsync(CancellationToken ct = default)
    {
        return await _db.Reports
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.Name)
            .Select(r => new ReportListItem
            {
                Id = r.Id,
                Name = r.Name,
                ViewName = r.ViewName,
                Description = r.Description,
                Icon = r.Icon,
                IsEnabled = r.IsEnabled,
                SortOrder = r.SortOrder
            })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Gets a single report WITH its columns, filters, and layouts.
    /// </summary>
    public async Task<ReportFullDefinition?> GetReportAsync(int reportId, CancellationToken ct = default)
    {
        var report = await _db.Reports
            .Include(r => r.Columns.OrderBy(c => c.SortOrder))
            .Include(r => r.Filters.OrderBy(f => f.SortOrder))
            .Include(r => r.Layouts)
            .FirstOrDefaultAsync(r => r.Id == reportId, ct);

        if (report is null) return null;

        return new ReportFullDefinition
        {
            Id = report.Id,
            Name = report.Name,
            ViewName = report.ViewName,
            Description = report.Description,
            Icon = report.Icon,
            IsEnabled = report.IsEnabled,
            SortOrder = report.SortOrder,
            Columns = report.Columns.Select(c => new ReportColumnDto
            {
                Id = c.Id,
                ColumnName = c.ColumnName,
                DisplayName = c.DisplayName,
                DataType = c.DataType,
                Width = c.Width,
                IsVisible = c.IsVisible,
                IsSortable = c.IsSortable,
                IsFilterable = c.IsFilterable,
                IsImageColumn = c.IsImageColumn,
                ImageBaseUrl = c.ImageBaseUrl,
                DateFormat = c.DateFormat,
                NumberFormat = c.NumberFormat,
                SortOrder = c.SortOrder
            }).ToList(),
            Filters = report.Filters.Select(f => new ReportFilterDto
            {
                Id = f.Id,
                ColumnName = f.ColumnName,
                FilterType = f.FilterType,
                Label = f.Label,
                IsRequired = f.IsRequired,
                DefaultValue = f.DefaultValue,
                OptionsQuery = f.OptionsQuery,
                Placeholder = f.Placeholder,
                SortOrder = f.SortOrder
            }).ToList(),
            Layouts = report.Layouts.Select(l => new ReportLayoutDto
            {
                Id = l.Id,
                LayoutName = l.LayoutName,
                IsDefault = l.IsDefault,
                ColumnOrder = l.ColumnOrder,
                VisibleColumns = l.VisibleColumns,
                ColumnWidths = l.ColumnWidths,
                FilterValues = l.FilterValues,
                SortState = l.SortState
            }).ToList()
        };
    }

    /// <summary>
    /// Creates a new report with columns and filters in a single transaction.
    /// </summary>
    public async Task<int> CreateReportAsync(ReportCreateRequest request, CancellationToken ct = default)
    {
        var report = new WarehouseDashboard.Web.Models.Report
        {
            Name = request.Name,
            ViewName = request.ViewName,
            Description = request.Description,
            Icon = request.Icon,
            IsEnabled = true,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Columns = request.Columns.Select(c => new WarehouseDashboard.Web.Models.ReportColumn
            {
                ColumnName = c.ColumnName,
                DisplayName = c.DisplayName,
                DataType = c.DataType,
                Width = c.Width ?? 150,
                IsVisible = c.IsVisible,
                IsSortable = c.IsSortable,
                IsFilterable = c.IsFilterable,
                IsImageColumn = c.IsImageColumn,
                ImageBaseUrl = c.ImageBaseUrl,
                DateFormat = c.DateFormat,
                NumberFormat = c.NumberFormat,
                SortOrder = c.SortOrder
            }).ToList(),
            Filters = request.Filters.Select(f => new WarehouseDashboard.Web.Models.ReportFilter
            {
                ColumnName = f.ColumnName,
                FilterType = f.FilterType,
                Label = f.Label,
                IsRequired = f.IsRequired,
                DefaultValue = f.DefaultValue,
                OptionsQuery = f.OptionsQuery,
                ValueColumn = f.ValueColumn,
                TextColumn = f.TextColumn,
                Placeholder = f.Placeholder,
                SortOrder = f.SortOrder
            }).ToList()
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync(ct);
        return report.Id;
    }

    /// <summary>
    /// Updates an existing report. Replaces all columns and filters.
    /// </summary>
    public async Task<bool> UpdateReportAsync(int reportId, ReportCreateRequest request, CancellationToken ct = default)
    {
        var report = await _db.Reports
            .Include(r => r.Columns)
            .Include(r => r.Filters)
            .FirstOrDefaultAsync(r => r.Id == reportId, ct);

        if (report is null) return false;

        // Update scalar fields
        report.Name = request.Name;
        report.ViewName = request.ViewName;
        report.Description = request.Description;
        report.Icon = request.Icon;
        report.SortOrder = request.SortOrder;
        report.UpdatedAt = DateTime.UtcNow;

        // Replace columns (delete old, add new)
        _db.ReportColumns.RemoveRange(report.Columns);
        report.Columns = request.Columns.Select(c => new WarehouseDashboard.Web.Models.ReportColumn
        {
            ReportId = reportId,
            ColumnName = c.ColumnName,
            DisplayName = c.DisplayName,
            DataType = c.DataType,
            Width = c.Width ?? 150,
            IsVisible = c.IsVisible,
            IsSortable = c.IsSortable,
            IsFilterable = c.IsFilterable,
            IsImageColumn = c.IsImageColumn,
            ImageBaseUrl = c.ImageBaseUrl,
            DateFormat = c.DateFormat,
            NumberFormat = c.NumberFormat,
            SortOrder = c.SortOrder
        }).ToList();

        // Replace filters (delete old, add new)
        _db.ReportFilters.RemoveRange(report.Filters);
        report.Filters = request.Filters.Select(f => new WarehouseDashboard.Web.Models.ReportFilter
        {
            ReportId = reportId,
            ColumnName = f.ColumnName,
            FilterType = f.FilterType,
            Label = f.Label,
            IsRequired = f.IsRequired,
            DefaultValue = f.DefaultValue,
                OptionsQuery = f.OptionsQuery,
                ValueColumn = f.ValueColumn,
                TextColumn = f.TextColumn,
                Placeholder = f.Placeholder,
                SortOrder = f.SortOrder
            }).ToList();

        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Deletes a report and all related columns/filters/layouts (CASCADE).
    /// </summary>
    public async Task<bool> DeleteReportAsync(int reportId, CancellationToken ct = default)
    {
        var report = await _db.Reports.FindAsync(new object[] { reportId }, ct);
        if (report is null) return false;

        _db.Reports.Remove(report);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Toggles report enabled/disabled status.
    /// </summary>
    public async Task<bool> ToggleReportAsync(int reportId, CancellationToken ct = default)
    {
        var report = await _db.Reports.FindAsync(new object[] { reportId }, ct);
        if (report is null) return false;

        report.IsEnabled = !report.IsEnabled;
        report.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
