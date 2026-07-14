namespace TeraQuotation.Services;

public interface IAuthService
{
    Task<bool> IsFirstTimeAsync();
    Task<(bool Success, string Error)> SetPasswordAsync(string password);
    Task<(bool Success, string Error)> ValidatePasswordAsync(string password);
}
