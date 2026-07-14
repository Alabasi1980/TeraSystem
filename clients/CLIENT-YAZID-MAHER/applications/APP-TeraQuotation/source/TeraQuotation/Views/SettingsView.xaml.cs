using System.Windows.Controls;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

public partial class SettingsView : UserControl
{
    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += async (s, e) =>
        {
            await viewModel.LoadSuppliersCommand.ExecuteAsync(null);
            await viewModel.LoadItemsCommand.ExecuteAsync(null);
            await viewModel.LoadSignaturesCommand.ExecuteAsync(null);
            await viewModel.LoadLetterheadCommand.ExecuteAsync(null);
        };
    }
}
