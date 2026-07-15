using System.Globalization;
using System.Windows.Data;

namespace TeraQuotation.Converters;

/// <summary>
/// Converts an integer (typically a Collection.Count) to bool:
/// true when the value is greater than 0.
/// Used to enable/disable controls based on whether a collection has items.
/// (Referenced in QuotationDetailView.xaml via InverseNullToBoolConverter key)
/// </summary>
public class InverseNullToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
            return count > 0;
        if (value is double d)
            return d > 0;
        if (value is long l)
            return l > 0;
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
