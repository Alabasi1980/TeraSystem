using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TeraQuotation.Data;
using TeraQuotation.Models;

namespace TeraQuotation.Services;

public class PdfService : IPdfService
{
    private readonly AppDbContext _db;

    static PdfService()
    {
        // QuestPDF requires a license setting (Community is free for qualifying projects)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public PdfService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<byte[]> GenerateQuotationPdfAsync(int quotationId)
    {
        return await BuildQuotationPdfAsync(quotationId, showPrices: true);
    }

    public async Task<byte[]> GenerateQuotationWithoutPricesPdfAsync(int quotationId)
    {
        return await BuildQuotationPdfAsync(quotationId, showPrices: false);
    }

    public async Task<byte[]> GenerateReportPdfAsync(string reportType, object data)
    {
        // Placeholder for future report PDF generation
        throw new NotImplementedException("Report PDF generation is not yet implemented.");
    }

    private async Task<byte[]> BuildQuotationPdfAsync(int quotationId, bool showPrices)
    {
        var quotation = await _db.Quotations
            .Include(q => q.Items)
                .ThenInclude(qi => qi.Item)
            .FirstOrDefaultAsync(q => q.Id == quotationId);

        if (quotation == null)
            throw new InvalidOperationException($"Quotation with ID {quotationId} not found.");

        // Load letterhead settings from the Settings table (key-value pairs)
        var settings = await _db.Settings
            .ToDictionaryAsync(s => s.Key, s => s.Value);

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // Header with letterhead
                page.Header().Element(c => ComposeHeader(c, settings, quotation));

                // Content: Items table (with or without prices)
                page.Content().Element(c => ComposeContent(c, quotation, showPrices));

                // Footer with page numbers
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("الصفحة ");
                    text.CurrentPageNumber();
                    text.Span(" من ");
                    text.TotalPages();
                });
            });
        });

        return doc.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, Dictionary<string, string> settings, Quotation quotation)
    {
        container.Column(col =>
        {
            // Quote number and date
            col.Item().Row(row =>
            {
                row.RelativeItem().AlignLeft().Text($"عرض سعر رقم: {quotation.QuoteNumber}")
                    .SemiBold().FontSize(14);
                row.RelativeItem().AlignRight().Text($"التاريخ: {quotation.Date:yyyy-MM-dd}");
            });

            // Company letterhead info
            if (settings.TryGetValue("CompanyName", out var companyName))
                col.Item().Text(companyName).FontSize(12);

            if (settings.TryGetValue("CompanyAddress", out var companyAddress))
                col.Item().Text(companyAddress).FontSize(9).FontColor(Colors.Grey.Darken1);

            if (settings.TryGetValue("CompanyPhone", out var companyPhone))
                col.Item().Text(companyPhone).FontSize(9).FontColor(Colors.Grey.Darken1);

            if (settings.TryGetValue("CompanyEmail", out var companyEmail))
                col.Item().Text(companyEmail).FontSize(9).FontColor(Colors.Grey.Darken1);

            // Separator line
            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private static void ComposeContent(IContainer container, Quotation quotation, bool showPrices)
    {
        container.Column(col =>
        {
            // Description
            if (!string.IsNullOrEmpty(quotation.Description))
                col.Item().PaddingBottom(5).Text($"الوصف: {quotation.Description}");

            // Signature if present
            if (!string.IsNullOrEmpty(quotation.SignatureName))
                col.Item().PaddingBottom(10).Text($"التوقيع: {quotation.SignatureName}");

            // Items table
            col.Item().Table(table =>
            {
                int columnCount = showPrices ? 7 : 1;

                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Item name — wider

                    if (showPrices)
                    {
                        columns.RelativeColumn(2); // Supplier1 type
                        columns.RelativeColumn(2); // Supplier1 price
                        columns.RelativeColumn(2); // Supplier2 type
                        columns.RelativeColumn(2); // Supplier2 price
                        columns.RelativeColumn(2); // Supplier3 type
                        columns.RelativeColumn(2); // Supplier3 price
                    }
                });

                // Header row — styling applied via HeaderCellStyle on each cell
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("القطعة").SemiBold();

                    if (showPrices)
                    {
                        header.Cell().Element(HeaderCellStyle).Text("م1-نوع").SemiBold();
                        header.Cell().Element(HeaderCellStyle).Text("م1-سعر").SemiBold();
                        header.Cell().Element(HeaderCellStyle).Text("م2-نوع").SemiBold();
                        header.Cell().Element(HeaderCellStyle).Text("م2-سعر").SemiBold();
                        header.Cell().Element(HeaderCellStyle).Text("م3-نوع").SemiBold();
                        header.Cell().Element(HeaderCellStyle).Text("م3-سعر").SemiBold();
                    }
                });

                // Data rows
                foreach (var item in quotation.Items)
                {
                    table.Cell().Element(CellStyle).Text(item.Item?.Name ?? "-").FontSize(9);

                    if (showPrices)
                    {
                        table.Cell().Element(CellStyle).Text(item.Supplier1Type ?? "-").FontSize(9);
                        table.Cell().Element(CellStyle).Text(
                            item.Supplier1Price?.ToString("N2") ?? "-").FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Supplier2Type ?? "-").FontSize(9);
                        table.Cell().Element(CellStyle).Text(
                            item.Supplier2Price?.ToString("N2") ?? "-").FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Supplier3Type ?? "-").FontSize(9);
                        table.Cell().Element(CellStyle).Text(
                            item.Supplier3Price?.ToString("N2") ?? "-").FontSize(9);
                    }
                }
            });
        });
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.PaddingHorizontal(2).PaddingVertical(1).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3);
    }

    private static IContainer HeaderCellStyle(IContainer container)
    {
        return container.PaddingVertical(3).BorderBottom(1).BorderColor(Colors.Black);
    }
}
