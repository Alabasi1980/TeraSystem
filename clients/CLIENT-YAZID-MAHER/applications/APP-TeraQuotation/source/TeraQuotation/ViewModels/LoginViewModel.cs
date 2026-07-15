using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TeraQuotation.Services;

namespace TeraQuotation.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isFirstTime;

    [ObservableProperty]
    private bool _isLoading;

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task LoadedAsync()
    {
        IsFirstTime = await _authService.IsFirstTimeAsync();
    }

    [RelayCommand]
    public async Task LoginAsync(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            ErrorMessage = "الرجاء إدخال كلمة المرور";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        var (success, error) = await _authService.ValidatePasswordAsync(password);
        if (success)
        {
            // Clear password from memory
            password = null;

            // Navigate to main screen — will navigate to Settings for now
            _navigationService.NavigateTo<Views.SettingsView>();
        }
        else
        {
            ErrorMessage = error;
        }

        IsLoading = false;
    }

    [RelayCommand]
    public async Task SetPasswordAsync(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            ErrorMessage = "الرجاء إدخال كلمة المرور";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        var (success, error) = await _authService.SetPasswordAsync(password);

        // Clear password from memory immediately
        password = null;

        if (success)
        {
            IsFirstTime = false;
            ErrorMessage = "✅ تم تعيين كلمة المرور بنجاح. سجّل دخولك.";
        }
        else
        {
            ErrorMessage = error;
        }

        IsLoading = false;
    }

}
