using System.Windows;

namespace TeraQuotation.Services;

public interface INavigationService
{
    /// <summary>
    /// Navigate to a view (synchronous, no unsaved-changes check).
    /// </summary>
    void NavigateTo<TView>() where TView : FrameworkElement;

    /// <summary>
    /// Navigate to a view with unsaved-changes check.
    /// Returns true if navigation succeeded, false if cancelled by user.
    /// </summary>
    Task<bool> TryNavigateToAsync<TView>() where TView : FrameworkElement;

    /// <summary>
    /// Go back to the previous view (synchronous, no unsaved-changes check).
    /// </summary>
    void GoBack();

    /// <summary>
    /// Go back to the previous view with unsaved-changes check.
    /// Returns true if navigation succeeded, false if cancelled by user.
    /// </summary>
    Task<bool> TryGoBackAsync();
}
