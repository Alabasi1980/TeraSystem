using System.Windows;

namespace TeraQuotation.Services;

public interface INavigationService
{
    void NavigateTo<TView>() where TView : FrameworkElement;
    void GoBack();
}
