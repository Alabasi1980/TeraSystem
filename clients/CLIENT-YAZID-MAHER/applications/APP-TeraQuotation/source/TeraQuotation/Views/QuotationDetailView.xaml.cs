using System.Windows;
using System.Windows.Controls;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

/// <summary>
/// Simplified read-only detail view for an existing quotation.
/// Editing functionality is now handled by QuotationFormView (master-detail workspace).
/// </summary>
public partial class QuotationDetailView : UserControl
{
    private readonly QuotationDetailViewModel _viewModel;

    public QuotationDetailView(QuotationDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // Register local converter so XAML bindings can use it
        Resources["InverseNullToBoolConverter"] = new InverseNullToBoolConverter();

        Loaded += async (s, e) =>
        {
            await viewModel.InitializeAsync();
        };
    }
}

/// <summary>
/// Converts a Collection.Count (int) to bool: true when Count > 0.
/// </summary>
internal class InverseNullToBoolConverter : System.Windows.Data.IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is int count)
            return count > 0;
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
