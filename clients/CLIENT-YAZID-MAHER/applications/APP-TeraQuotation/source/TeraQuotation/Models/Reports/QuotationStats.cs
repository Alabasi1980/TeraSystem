namespace TeraQuotation.Models.Reports;

public record QuotationStats(
    int Total,
    int Draft,
    int UpdatedWithPrices,
    int Printed,
    int PdfExported,
    int SentViaOutlook,
    int ThisWeek,
    int ThisMonth,
    string? TopSupplier,
    string? TopItem
);
