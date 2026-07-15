using System.Windows.Controls;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

public partial class ReportsView : UserControl
{
    public ReportsView(ReportsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
