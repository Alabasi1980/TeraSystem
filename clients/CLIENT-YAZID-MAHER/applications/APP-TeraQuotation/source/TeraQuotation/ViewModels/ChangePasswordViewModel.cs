using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TeraQuotation.Services;

namespace TeraQuotation.ViewModels;

public partial class ChangePasswordViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    public Action? CloseAction { get; set; }

    [ObservableProperty]
    private string? _currentPassword;

    [ObservableProperty]
    private string? _newPassword;

    [ObservableProperty]
    private string? _confirmPassword;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isSuccess;

    partial void OnCurrentPasswordChanged(string? value) => ClearMessages();
    partial void OnNewPasswordChanged(string? value) => ClearMessages();
    partial void OnConfirmPasswordChanged(string? value) => ClearMessages();

    public ChangePasswordViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    private void ClearMessages()
    {
        ErrorMessage = null;
        IsSuccess = false;
    }

    [RelayCommand]
    async Task ChangePasswordAsync()
    {
        // Validate
        if (string.IsNullOrWhiteSpace(CurrentPassword))
        {
            ErrorMessage = "الرجاء إدخال كلمة المرور الحالية";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "الرجاء إدخال كلمة المرور الجديدة";
            return;
        }

        if (NewPassword.Length < 4)
        {
            ErrorMessage = "كلمة المرور الجديدة يجب أن تكون 4 أحرف على الأقل";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "كلمة المرور الجديدة وتأكيدها غير متطابقين";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        // Verify current password
        var (valid, _) = await _authService.ValidatePasswordAsync(CurrentPassword);
        if (!valid)
        {
            ErrorMessage = "كلمة المرور الحالية غير صحيحة";
            IsLoading = false;
            return;
        }

        // Set new password
        var (success, setErr) = await _authService.SetPasswordAsync(NewPassword);
        IsLoading = false;

        if (success)
        {
            IsSuccess = true;
            ErrorMessage = "تم تغيير كلمة المرور بنجاح";

            // Clear fields
            CurrentPassword = null;
            NewPassword = null;
            ConfirmPassword = null;
        }
        else
        {
            ErrorMessage = setErr;
        }
    }

    [RelayCommand]
    void Cancel()
    {
        CloseAction?.Invoke();
    }
}
