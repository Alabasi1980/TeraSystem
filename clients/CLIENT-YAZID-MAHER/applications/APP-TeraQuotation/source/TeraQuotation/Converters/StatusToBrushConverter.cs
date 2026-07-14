using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TeraQuotation.Converters;

/// <summary>
/// Converts a quotation status string to a coloured Brush for display in the DataGrid.
/// Uses design token colours from 28_UI_UX_GUIDELINES.md §3.1.
/// </summary>
public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value as string;
        return status switch
        {
            // Draft → TextSecondary (#6B7280)
            "Draft"            => new SolidColorBrush(Color.FromRgb(107, 114, 128)),
            // UpdatedWithPrices → Primary (#2563EB)
            "UpdatedWithPrices" => new SolidColorBrush(Color.FromRgb( 37,  99, 235)),
            // Printed → Success (#16A34A)
            "Printed"          => new SolidColorBrush(Color.FromRgb( 22, 163,  74)),
            // PDFExported → Warning (#D97706)
            "PDFExported"      => new SolidColorBrush(Color.FromRgb(217, 119,   6)),
            // SentViaOutlook → Purple (kept for distinct visibility)
            "SentViaOutlook"   => new SolidColorBrush(Color.FromRgb(147,  51, 234)),
            _                  => new SolidColorBrush(Color.FromRgb(107, 114, 128)),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
