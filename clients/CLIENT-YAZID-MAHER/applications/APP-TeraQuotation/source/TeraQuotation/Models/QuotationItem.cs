namespace TeraQuotation.Models;

public class QuotationItem
{
    public int Id { get; set; }
    public int QuotationId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public string Unit { get; set; } = "قطعة";
    public string? Supplier1Name { get; set; }
    public string? Supplier1Type { get; set; }
    public decimal? Supplier1Price { get; set; }
    public string? Supplier2Name { get; set; }
    public string? Supplier2Type { get; set; }
    public decimal? Supplier2Price { get; set; }
    public string? Supplier3Name { get; set; }
    public string? Supplier3Type { get; set; }
    public decimal? Supplier3Price { get; set; }

    public Quotation Quotation { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
