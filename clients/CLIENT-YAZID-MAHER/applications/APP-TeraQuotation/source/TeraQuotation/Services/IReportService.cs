using TeraQuotation.Models.Reports;

namespace TeraQuotation.Services;

public interface IReportService
{
    Task<List<SupplierReport>> GetSupplierReportAsync();
    Task<List<ItemReport>> GetItemReportAsync();
    Task<QuotationStats> GetQuotationStatsAsync();
    Task<List<SupplierComparisonDto>> GetSupplierComparisonAsync(int quotationId);
    Task<List<TopItemDto>> GetTopItemsAsync(int topN, DateTime? from, DateTime? to);
    Task<List<QuotationHistoryDto>> GetQuotationHistoryAsync(DateTime? from, DateTime? to, string? status);
    Task<MonthlyTotalDto> GetMonthlyTotalAsync(int year);
}
