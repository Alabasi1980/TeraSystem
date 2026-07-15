namespace TeraQuotation.Models.Reports;

public class TopItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public int RequestCount { get; set; }
    public decimal? AveragePrice { get; set; }
}
