using Microsoft.EntityFrameworkCore;
using TeraQuotation.Data;
using TeraQuotation.Models.Reports;

namespace TeraQuotation.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SupplierReport>> GetSupplierReportAsync()
    {
        var suppliers = await _db.Suppliers.ToListAsync();
        var result = new List<SupplierReport>();

        foreach (var s in suppliers)
        {
            var items = await _db.QuotationItems
                .Where(qi => qi.Supplier1Type == s.Name
                          || qi.Supplier2Type == s.Name
                          || qi.Supplier3Type == s.Name)
                .Include(qi => qi.Quotation)
                .Include(qi => qi.Item)
                .ToListAsync();

            result.Add(new SupplierReport(
                s.Name,
                items.Select(qi => qi.QuotationId).Distinct().Count(),
                items.Select(qi => qi.Item.Name).Distinct().ToList()
            ));
        }

        return result;
    }

    public async Task<List<ItemReport>> GetItemReportAsync()
    {
        var items = await _db.Items.ToListAsync();
        var result = new List<ItemReport>();

        foreach (var i in items)
        {
            var qis = await _db.QuotationItems
                .Where(qi => qi.ItemId == i.Id)
                .Include(qi => qi.Quotation)
                .ToListAsync();

            result.Add(new ItemReport(
                i.Name,
                qis.Count,
                qis.Select(qi => qi.Quotation.QuoteNumber).Distinct().ToList()
            ));
        }

        return result;
    }

    public async Task<QuotationStats> GetQuotationStatsAsync()
    {
        var all = await _db.Quotations.ToListAsync();
        var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        // Determine top supplier
        var topSupplier = await _db.QuotationItems
            .GroupBy(qi => qi.Supplier1Type)
            .Where(g => g.Key != null)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        // Determine top item
        var topItem = await _db.QuotationItems
            .GroupBy(qi => qi.ItemId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        string? topItemName = null;
        if (topItem > 0)
        {
            var item = await _db.Items.FindAsync(topItem);
            topItemName = item?.Name;
        }

        return new QuotationStats(
            all.Count,
            all.Count(q => q.Status == "Draft"),
            all.Count(q => q.Status == "UpdatedWithPrices"),
            all.Count(q => q.Status == "Printed"),
            all.Count(q => q.Status == "PDFExported"),
            all.Count(q => q.Status == "SentViaOutlook"),
            all.Count(q => q.Date >= weekStart),
            all.Count(q => q.Date >= monthStart),
            topSupplier,
            topItemName
        );
    }

    public Task<List<SupplierComparisonDto>> GetSupplierComparisonAsync(int quotationId)
    {
        throw new NotImplementedException();
    }

    public Task<List<TopItemDto>> GetTopItemsAsync(int topN, DateTime? from, DateTime? to)
    {
        throw new NotImplementedException();
    }

    public Task<List<QuotationHistoryDto>> GetQuotationHistoryAsync(DateTime? from, DateTime? to, string? status)
    {
        throw new NotImplementedException();
    }

    public Task<MonthlyTotalDto> GetMonthlyTotalAsync(int year)
    {
        throw new NotImplementedException();
    }
}
