using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace TeraQuotation.Services;

public class NavigationService : INavigationService
{
    private readonly Frame _frame;
    private readonly Stack<FrameworkElement> _history = new();

    public NavigationService(Frame frame)
    {
        _frame = frame;
    }

    /// <summary>
    /// Checks if the current page's DataContext implements IUnsavedChangesAware
    /// and has unsaved changes. If so, prompts the user to confirm.
    /// </summary>
    /// <returns>True if navigation should proceed, false to cancel.</returns>
    private async Task<bool> CheckUnsavedChangesAsync()
    {
        if (_frame.Content is FrameworkElement currentPage
            && currentPage.DataContext is IUnsavedChangesAware aware
            && aware.HasUnsavedChanges)
        {
            return await aware.ConfirmNavigateAwayAsync();
        }

        return true; // No unsaved changes — proceed
    }

    public void NavigateTo<TView>() where TView : FrameworkElement
    {
        var view = App.ServiceProvider.GetRequiredService<TView>();
        _history.Push(view);
        _frame.Navigate(view);
    }

    public async Task<bool> TryNavigateToAsync<TView>() where TView : FrameworkElement
    {
        if (!await CheckUnsavedChangesAsync())
            return false;

        var view = App.ServiceProvider.GetRequiredService<TView>();
        _history.Push(view);
        _frame.Navigate(view);
        return true;
    }

    public void GoBack()
    {
        if (_history.Count > 1)
        {
            _history.Pop();
            _frame.Navigate(_history.Peek());
        }
    }

    public async Task<bool> TryGoBackAsync()
    {
        if (!await CheckUnsavedChangesAsync())
            return false;

        if (_history.Count > 1)
        {
            _history.Pop();
            _frame.Navigate(_history.Peek());
            return true;
        }

        return false;
    }
}
