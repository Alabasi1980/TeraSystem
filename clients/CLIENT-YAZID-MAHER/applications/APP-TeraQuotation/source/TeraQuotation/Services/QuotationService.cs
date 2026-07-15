using Microsoft.EntityFrameworkCore;
using TeraQuotation.Data;
using TeraQuotation.Models;

namespace TeraQuotation.Services;

public class QuotationService : IQuotationService
{
    private readonly AppDbContext _db;

    public QuotationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Quotation>> GetAllQuotationsAsync()
    {
        return await _db.Quotations.OrderByDescending(q => q.CreatedAt).ToListAsync();
    }

    public async Task<Quotation?> GetQuotationByIdAsync(int id)
    {
        return await _db.Quotations.Include(q => q.Items).ThenInclude(qi => qi.Item)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Quotation?> GetQuotationByNumberAsync(string number)
    {
        return await _db.Quotations.Include(q => q.Items).ThenInclude(qi => qi.Item)
            .FirstOrDefaultAsync(q => q.QuoteNumber == number);
    }

    public async Task<string> GenerateNextQuoteNumberAsync()
    {
        var last = await _db.Quotations.OrderByDescending(q => q.Id).FirstOrDefaultAsync();
        if (last == null) return "Q-001";

        if (int.TryParse(last.QuoteNumber.Replace("Q-", ""), out int num))
            return $"Q-{(num + 1):D3}";

        return "Q-001";
    }

    public async Task<Quotation> CreateQuotationAsync(Quotation quotation)
    {
        quotation.QuoteNumber = await GenerateNextQuoteNumberAsync();
        quotation.CreatedAt = DateTime.UtcNow;
        quotation.UpdatedAt = DateTime.UtcNow;
        _db.Quotations.Add(quotation);
        await _db.SaveChangesAsync();
        return quotation;
    }

    public async Task UpdateQuotationAsync(Quotation quotation)
    {
        quotation.UpdatedAt = DateTime.UtcNow;
        _db.Quotations.Update(quotation);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteQuotationAsync(int id)
    {
        var q = await _db.Quotations.Include(qt => qt.Items).FirstOrDefaultAsync(qt => qt.Id == id);
        if (q != null)
        {
            _db.QuotationItems.RemoveRange(q.Items);
            _db.Quotations.Remove(q);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<Quotation>> SearchQuotationsAsync(string? search, string? status)
    {
        var query = _db.Quotations.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(q => q.QuoteNumber.Contains(search) || q.Description.Contains(search));
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(q => q.Status == status);
        return await query.OrderByDescending(q => q.CreatedAt).ToListAsync();
    }
}
