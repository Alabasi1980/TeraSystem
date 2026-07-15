using System.Windows;

namespace TeraQuotation.Views.Dialogs;

/// <summary>
/// User's choice from the UnsavedChangesDialog.
/// </summary>
public enum UnsavedChangesChoice
{
    Save,
    Discard,
    Cancel
}

/// <summary>
/// Modal dialog asking the user what to do with unsaved changes.
/// Shows three options: save and exit, exit without saving, or cancel.
/// </summary>
public partial class UnsavedChangesDialog : Window
{
    /// <summary>
    /// The user's final choice. Defaults to Cancel if window is closed.
    /// </summary>
    public UnsavedChangesChoice Choice { get; private set; } = UnsavedChangesChoice.Cancel;

    public UnsavedChangesDialog()
    {
        InitializeComponent();
    }

    private void SaveAndExit_Click(object sender, RoutedEventArgs e)
    {
        Choice = UnsavedChangesChoice.Save;
        DialogResult = true;
        Close();
    }

    private void Discard_Click(object sender, RoutedEventArgs e)
    {
        Choice = UnsavedChangesChoice.Discard;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Choice = UnsavedChangesChoice.Cancel;
        DialogResult = false;
        Close();
    }
}
