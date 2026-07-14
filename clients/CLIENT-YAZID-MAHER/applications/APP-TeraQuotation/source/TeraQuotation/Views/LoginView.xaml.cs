using System.Windows;
using System.Windows.Controls;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

public partial class LoginView : UserControl
{
    private readonly LoginViewModel _viewModel;

    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        Loaded += async (s, e) => await viewModel.LoadedCommand.ExecuteAsync(null);
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        var password = PasswordInput.Password;
        await _viewModel.LoginCommand.ExecuteAsync(password);
        PasswordInput.Clear();
    }

    private async void SetPassword_Click(object sender, RoutedEventArgs e)
    {
        var password = PasswordInput.Password;
        await _viewModel.SetPasswordCommand.ExecuteAsync(password);
        PasswordInput.Clear();
    }
}
