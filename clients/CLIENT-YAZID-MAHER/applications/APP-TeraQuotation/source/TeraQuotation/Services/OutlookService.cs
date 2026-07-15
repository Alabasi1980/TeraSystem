using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Outlook;

namespace TeraQuotation.Services;

public class OutlookService : IOutlookService
{
    public void SendQuotationEmail(string quoteNumber, string pdfPath, string recipientEmail = "")
    {
        var outlook = new Application();
        var mail = outlook.CreateItem(OlItemType.olMailItem) as MailItem;
        if (mail == null) throw new System.Exception("تعذر إنشاء بريد Outlook");

        mail.Subject = $"عرض سعر رقم: {quoteNumber}";
        mail.Body = $"السلام عليكم،\n\nنرفق لكم عرض السعر رقم {quoteNumber}.\n\nمع الشكر";
        if (!string.IsNullOrEmpty(recipientEmail))
            mail.To = recipientEmail;

        if (File.Exists(pdfPath))
            mail.Attachments.Add(pdfPath);

        mail.Display(); // يعرض البريد جاهزاً للإرسال (لا يرسل تلقائياً)
    }
}
