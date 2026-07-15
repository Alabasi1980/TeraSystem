namespace TeraQuotation.Services;

public interface IPdfService
{
    Task<byte[]> GenerateQuotationPdfAsync(int quotationId);
    Task<byte[]> GenerateQuotationWithoutPricesPdfAsync(int quotationId);
    Task<byte[]> GenerateReportPdfAsync(string reportType, object data);
}
