namespace TeraQuotation.Models.Reports;

public record SupplierReport(string SupplierName, int QuotationCount, List<string> Items);
