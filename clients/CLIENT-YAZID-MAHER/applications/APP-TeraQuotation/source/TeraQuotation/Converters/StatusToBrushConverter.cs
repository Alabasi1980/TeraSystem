using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TeraQuotation.Converters;

/// <summary>
/// Converts a quotation status string to a coloured Brush for display in the DataGrid.
/// </summary>
public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value as string;
        return status switch
        {
            "Draft"            => new SolidColorBrush(Colors.Gray),
            "UpdatedWithPrices" => new SolidColorBrush(Color.FromRgb( 33, 150, 243)), // Blue
            "Printed"          => new SolidColorBrush(Color.FromRgb( 76, 175,  80)), // Green
            "PDFExported"      => new SolidColorBrush(Color.FromRgb(255, 152,   0)), // Orange
            "SentViaOutlook"   => new SolidColorBrush(Color.FromRgb(156,  39, 176)), // Purple
            _                  => new SolidColorBrush(Colors.Gray),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
