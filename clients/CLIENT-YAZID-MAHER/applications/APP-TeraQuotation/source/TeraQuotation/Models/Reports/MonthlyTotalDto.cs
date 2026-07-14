namespace TeraQuotation.Models.Reports;

public class MonthlyTotalDto
{
    public int Year { get; set; }
    public List<MonthlyBreakdown> MonthlyBreakdown { get; set; } = new();
    public int TotalQuotations { get; set; }
    public decimal? GrandTotal { get; set; }
}

public class MonthlyBreakdown
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal? Total { get; set; }
}
