using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using TeraQuotation.Models;

namespace TeraQuotation.Helpers;

/// <summary>
/// Helper for printing quotations directly via WPF PrintDialog with a FixedDocument (A4).
/// </summary>
public static class PrintHelper
{
    private const double A4WidthInches = 8.27;
    private const double A4HeightInches = 11.69;
    private static readonly double A4WidthDpi = 96.0 * A4WidthInches;
    private static readonly double A4HeightDpi = 96.0 * A4HeightInches;

    /// <summary>
    /// Shows a PrintDialog and prints the quotation as an A4 document.
    /// </summary>
    /// <param name="quotation">The quotation to print (must have Items loaded).</param>
    /// <param name="showPrices">True to include supplier price columns; false for item names only.</param>
    public static void PrintQuotation(Quotation quotation, bool showPrices)
    {
        var document = BuildFixedDocument(quotation, showPrices);

        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
        {
            dlg.PrintDocument(document.DocumentPaginator, $"عرض سعر {quotation.QuoteNumber}");
        }
    }

    /// <summary>
    /// Builds a FixedDocument for the quotation ready to print/spool.
    /// </summary>
    private static FixedDocument BuildFixedDocument(Quotation quotation, bool showPrices)
    {
        var document = new FixedDocument();
        document.DocumentPaginator.PageSize = new Size(A4WidthDpi, A4HeightDpi);

        var pageContent = new PageContent();
        var fixedPage = new FixedPage
        {
            Width = A4WidthDpi,
            Height = A4HeightDpi,
            Margin = new Thickness(0)
        };

        // Root container with RTL and margins
        var rootStack = new StackPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Margin = new Thickness(50, 50, 50, 50)
        };

        // ---- Header ----
        // Quote number + Date
        var headerRow = new Grid();
        headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var quoteNumberBlock = new TextBlock
        {
            Text = $"عرض سعر رقم: {quotation.QuoteNumber}",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        Grid.SetColumn(quoteNumberBlock, 0);
        headerRow.Children.Add(quoteNumberBlock);

        var dateBlock = new TextBlock
        {
            Text = $"التاريخ: {quotation.Date:yyyy-MM-dd}",
            FontSize = 12,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(dateBlock, 1);
        headerRow.Children.Add(dateBlock);

        rootStack.Children.Add(headerRow);

        // Description
        if (!string.IsNullOrEmpty(quotation.Description))
        {
            rootStack.Children.Add(new TextBlock
            {
                Text = $"الوصف: {quotation.Description}",
                FontSize = 11,
                Margin = new Thickness(0, 10, 0, 5)
            });
        }

        // Signature
        if (!string.IsNullOrEmpty(quotation.SignatureName))
        {
            rootStack.Children.Add(new TextBlock
            {
                Text = $"التوقيع: {quotation.SignatureName}",
                FontSize = 11,
                Margin = new Thickness(0, 0, 0, 10)
            });
        }

        // Separator
        rootStack.Children.Add(new Rectangle
        {
            Height = 1,
            Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
            Margin = new Thickness(0, 0, 0, 10)
        });

        // ---- Items table ----
        int columnCount = showPrices ? 7 : 1;

        var tableGrid = new Grid();
        tableGrid.ShowGridLines = true;

        // Column definitions
        double[] columnWidths = showPrices
            ? [2.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0]
            : [1.0];

        foreach (var width in columnWidths)
        {
            tableGrid.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(width, GridUnitType.Star) });
        }

        // Row definitions: header + data rows
        tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int i = 0; i < quotation.Items.Count; i++)
        {
            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // ---- Header row ----
        string[] headers = showPrices
            ? ["القطعة", "م1-نوع", "م1-سعر", "م2-نوع", "م2-سعر", "م3-نوع", "م3-سعر"]
            : ["القطعة"];

        for (int col = 0; col < headers.Length; col++)
        {
            var cell = new TextBlock
            {
                Text = headers[col],
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(4, 3, 4, 3),
                Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, col);
            tableGrid.Children.Add(cell);
        }

        // ---- Data rows ----
        for (int row = 0; row < quotation.Items.Count; row++)
        {
            var item = quotation.Items.ElementAt(row);
            int gridRow = row + 1; // skip header

            // Column 0: item name
            var nameCell = new TextBlock
            {
                Text = item.Item?.Name ?? "-",
                FontSize = 10,
                Padding = new Thickness(4, 2, 4, 2)
            };
            Grid.SetRow(nameCell, gridRow);
            Grid.SetColumn(nameCell, 0);
            tableGrid.Children.Add(nameCell);

            if (showPrices)
            {
                // Supplier 1 type
                var s1tCell = new TextBlock { Text = item.Supplier1Type ?? "-", FontSize = 10, Padding = new Thickness(4, 2, 4, 2) };
                Grid.SetRow(s1tCell, gridRow); Grid.SetColumn(s1tCell, 1);
                tableGrid.Children.Add(s1tCell);

                // Supplier 1 price
                var s1pCell = new TextBlock { Text = item.Supplier1Price?.ToString("N2") ?? "-", FontSize = 10, Padding = new Thickness(4, 2, 4, 2) };
                Grid.SetRow(s1pCell, gridRow); Grid.SetColumn(s1pCell, 2);
                tableGrid.Children.Add(s1pCell);

                // Supplier 2 type
                var s2tCell = new TextBlock { Text = item.Supplier2Type ?? "-", FontSize = 10, Padding = new Thickness(4, 2, 4, 2) };
                Grid.SetRow(s2tCell, gridRow); Grid.SetColumn(s2tCell, 3);
                tableGrid.Children.Add(s2tCell);

                // Supplier 2 price
                var s2pCell = new TextBlock { Text = item.Supplier2Price?.ToString("N2") ?? "-", FontSize = 10, Padding = new Thickness(4, 2, 4, 2) };
                Grid.SetRow(s2pCell, gridRow); Grid.SetColumn(s2pCell, 4);
                tableGrid.Children.Add(s2pCell);

                // Supplier 3 type
                var s3tCell = new TextBlock { Text = item.Supplier3Type ?? "-", FontSize = 10, Padding = new Thickness(4, 2, 4, 2) };
                Grid.SetRow(s3tCell, gridRow); Grid.SetColumn(s3tCell, 5);
                tableGrid.Children.Add(s3tCell);

                // Supplier 3 price
                var s3pCell = new TextBlock { Text = item.Supplier3Price?.ToString("N2") ?? "-", FontSize = 10, Padding = new Thickness(4, 2, 4, 2) };
                Grid.SetRow(s3pCell, gridRow); Grid.SetColumn(s3pCell, 6);
                tableGrid.Children.Add(s3pCell);
            }
        }

        rootStack.Children.Add(tableGrid);

        // Add root to page
        fixedPage.Children.Add(rootStack);
        ((UIElement)fixedPage).UpdateLayout();

        pageContent.Child = fixedPage;
        document.Pages.Add(pageContent);

        return document;
    }
}
