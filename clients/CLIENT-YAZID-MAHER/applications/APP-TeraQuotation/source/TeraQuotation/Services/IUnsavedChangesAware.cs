namespace TeraQuotation.Services;

/// <summary>
/// Marks a ViewModel as aware of unsaved changes.
/// NavigationService checks this before navigating away.
/// </summary>
public interface IUnsavedChangesAware
{
    /// <summary>Whether there are unsaved changes that would be lost.</summary>
    bool HasUnsavedChanges { get; }

    /// <summary>
    /// Called before navigating away. Should show a confirmation dialog.
    /// Return true to allow navigation, false to cancel.
    /// </summary>
    Task<bool> ConfirmNavigateAwayAsync();
}
