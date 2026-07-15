using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TeraQuotation.Helpers;

/// <summary>
/// Lightweight toast notification helper.
/// Creates a temporary overlay window that auto-dismisses after a set duration.
/// Based on design tokens from 28_UI_UX_GUIDELINES.md §5.6.
/// </summary>
public static class ToastHelper
{
    private static readonly TimeSpan SuccessDuration = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan ErrorDuration = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan WarningDuration = TimeSpan.FromSeconds(4);

    /// <summary>
    /// Shows a success toast with the given message.
    /// Green background, white text, 3-second duration.
    /// </summary>
    public static void ShowSuccess(string message)
    {
        ShowToast(message, "#16A34A", Colors.White, SuccessDuration);
    }

    /// <summary>
    /// Shows an error toast with the given message.
    /// Red background, white text, 5-second duration.
    /// </summary>
    public static void ShowError(string message)
    {
        ShowToast(message, "#DC2626", Colors.White, ErrorDuration);
    }

    /// <summary>
    /// Shows a warning toast with the given message.
    /// Amber background, dark text, 4-second duration.
    /// </summary>
    public static void ShowWarning(string message)
    {
        ShowToast(message, "#D97706", Colors.White, WarningDuration);
    }

    private static void ShowToast(string message, string backgroundColorHex, Color foregroundColor, TimeSpan duration)
    {
        // Ensure we're on the UI thread
        if (Application.Current?.Dispatcher == null) return;

        if (!Application.Current.Dispatcher.CheckAccess())
        {
            Application.Current.Dispatcher.Invoke(() =>
                ShowToast(message, backgroundColorHex, foregroundColor, duration));
            return;
        }

        // Find the active window to position relative to it
        var owner = Application.Current.Windows.OfType<Window>()
            .FirstOrDefault(w => w.IsActive) ?? Application.Current.MainWindow;

        if (owner == null) return;

        // Parse background colour
        var bgColor = (Color)ColorConverter.ConvertFromString(backgroundColorHex);

        // Create toast window
        var toast = new Window
        {
            Content = CreateToastContent(message, bgColor, foregroundColor),
            Background = Brushes.Transparent,
            AllowsTransparency = true,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
            Topmost = true,
            ResizeMode = ResizeMode.NoResize,
            SizeToContent = SizeToContent.WidthAndHeight,
            Owner = owner,
            WindowStartupLocation = WindowStartupLocation.Manual,
        };

        // Position at top-right (RTL-friendly) of owner window
        toast.Loaded += (s, e) =>
        {
            if (owner.WindowState == WindowState.Normal)
            {
                toast.Left = owner.Left + owner.Width - toast.Width - 20;
                toast.Top = owner.Top + 20;
            }
            else
            {
                toast.Left = SystemParameters.WorkArea.Right - toast.Width - 20;
                toast.Top = SystemParameters.WorkArea.Top + 20;
            }
        };

        toast.Show();

        // Auto-dismiss with fade-out animation
        var timer = new DispatcherTimer
        {
            Interval = duration,
            IsEnabled = true,
        };
        timer.Tick += (s, e) =>
        {
            timer.Stop();

            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300),
            };

            fadeOut.Completed += (se, ae) => toast.Close();
            toast.BeginAnimation(Window.OpacityProperty, fadeOut);
        };
        timer.Start();
    }

    private static FrameworkElement CreateToastContent(string message, Color bgColor, Color fgColor)
    {
        var border = new System.Windows.Controls.Border
        {
            Background = new SolidColorBrush(bgColor),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16, 10, 16, 10),
            MaxWidth = 400,
            Margin = new Thickness(4),
            Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                BlurRadius = 12,
                Opacity = 0.15,
                ShadowDepth = 3,
                Color = Colors.Black,
            },
        };

        var text = new System.Windows.Controls.TextBlock
        {
            Text = message,
            Foreground = new SolidColorBrush(fgColor),
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            TextWrapping = System.Windows.TextWrapping.Wrap,
            FlowDirection = FlowDirection.RightToLeft,
            TextAlignment = System.Windows.TextAlignment.Right,
        };

        border.Child = text;
        return border;
    }
}
