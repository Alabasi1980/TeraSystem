using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TeraQuotation.Views;

namespace TeraQuotation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void ShowLogin()
    {
        var loginView = App.ServiceProvider.GetRequiredService<LoginView>();
        MainFrame.Navigate(loginView);
    }
}
