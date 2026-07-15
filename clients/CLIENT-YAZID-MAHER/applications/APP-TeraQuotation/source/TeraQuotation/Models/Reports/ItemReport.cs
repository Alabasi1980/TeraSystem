namespace TeraQuotation.Models.Reports;

public record ItemReport(string ItemName, int UsageCount, List<string> QuotationNumbers);
