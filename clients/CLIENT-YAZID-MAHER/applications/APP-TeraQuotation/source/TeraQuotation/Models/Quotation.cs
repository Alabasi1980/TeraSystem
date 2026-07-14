namespace TeraQuotation.Models;

public class Quotation
{
    public int Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, UpdatedWithPrices, Printed, PDFExported, SentViaOutlook
    public string? SignatureName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}
