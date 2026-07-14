namespace TeraQuotation.Models.Reports;

public class QuotationHistoryDto
{
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? TotalPrice { get; set; }
}
