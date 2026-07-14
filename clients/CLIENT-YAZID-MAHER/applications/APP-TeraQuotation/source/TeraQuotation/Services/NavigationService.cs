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

    public void NavigateTo<TView>() where TView : FrameworkElement
    {
        var view = App.ServiceProvider.GetRequiredService<TView>();
        _history.Push(view);
        _frame.Navigate(view);
    }

    public void GoBack()
    {
        if (_history.Count > 1)
        {
            _history.Pop();
            _frame.Navigate(_history.Peek());
        }
    }
}
