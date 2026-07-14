using System.Windows;
using System.Windows.Controls;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

public partial class ChangePasswordDialog : Window
{
    public ChangePasswordDialog(ChangePasswordViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        // Allow the ViewModel to close this window via Cancel command
        vm.CloseAction = () => this.Close();
    }

    /// <summary>
    /// Sync PasswordBox values to the ViewModel before executing the command.
    /// PasswordBox.Password is not a DependencyProperty, so we cannot use
    /// regular data-binding. This code-behind approach is the standard WPF
    /// workaround for MVVM dialogs.
    /// </summary>
    private void ChangeBtn_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ChangePasswordViewModel vm) return;

        // Copy PasswordBox values to the bindable ViewModel properties
        vm.CurrentPassword = CurrentPwd.Password;
        vm.NewPassword = NewPwd.Password;
        vm.ConfirmPassword = ConfirmPwd.Password;

        // Execute the command
        vm.ChangePasswordCommand.Execute(null);
    }

    /// <summary>
    /// Clear error message whenever the user types in any password field,
    /// giving them immediate feedback that the previous error is dismissed.
    /// </summary>
    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        // The ViewModel already clears messages via partial On*Changed methods,
        // but those fire only when the property is set programmatically.
        // Since we only sync on button click, we clear manually here.
        if (DataContext is ChangePasswordViewModel vm)
        {
            // Only clear if there's a message showing (avoid unnecessary clearing)
            if (vm.ErrorMessage != null)
            {
                vm.ErrorMessage = null;
                vm.IsSuccess = false;
            }
        }
    }
}
