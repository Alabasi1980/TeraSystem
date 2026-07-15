using System.Windows.Controls;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

public partial class QuotationListView : UserControl
{
    public QuotationListView(QuotationListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += async (s, e) =>
        {
            await viewModel.LoadCommand.ExecuteAsync(null);
        };
    }
}
