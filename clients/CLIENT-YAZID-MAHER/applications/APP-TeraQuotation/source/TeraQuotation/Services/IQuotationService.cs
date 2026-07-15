using TeraQuotation.Models;

namespace TeraQuotation.Services;

public interface IQuotationService
{
    Task<List<Quotation>> GetAllQuotationsAsync();
    Task<Quotation?> GetQuotationByIdAsync(int id);
    Task<Quotation?> GetQuotationByNumberAsync(string number);
    Task<string> GenerateNextQuoteNumberAsync();
    Task<Quotation> CreateQuotationAsync(Quotation quotation);
    Task UpdateQuotationAsync(Quotation quotation);
    Task DeleteQuotationAsync(int id);
    Task<List<Quotation>> SearchQuotationsAsync(string? search, string? status);
}
