namespace TeraQuotation.Models.Reports;

public class SupplierComparisonDto
{
    public int QuotationId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Supplier1Name { get; set; }
    public decimal? Supplier1Price { get; set; }
    public string? Supplier2Name { get; set; }
    public decimal? Supplier2Price { get; set; }
    public string? Supplier3Name { get; set; }
    public decimal? Supplier3Price { get; set; }
    public decimal? BestPrice { get; set; }
    public string? BestSupplier { get; set; }
}
