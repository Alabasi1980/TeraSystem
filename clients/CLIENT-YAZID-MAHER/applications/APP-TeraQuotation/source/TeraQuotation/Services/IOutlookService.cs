namespace TeraQuotation.Services;

public interface IOutlookService
{
    void SendQuotationEmail(string quoteNumber, string pdfPath, string recipientEmail = "");
}
