namespace TeraQuotation.Services;

/// <summary>
/// Singleton service to pass a parameter (e.g. quotation ID) between views during navigation.
/// Set before NavigateTo, read in the target view's initialization.
/// </summary>
public class NavigationParameter
{
    public object? Value { get; set; }
}
