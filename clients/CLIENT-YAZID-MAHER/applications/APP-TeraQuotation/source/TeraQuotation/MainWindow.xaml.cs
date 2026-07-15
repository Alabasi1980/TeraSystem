using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using TeraQuotation.Services;
using TeraQuotation.Views;

namespace TeraQuotation;

public partial class MainWindow : Window
{
    private readonly IDbHealthService _healthService;

    public MainWindow()
    {
        InitializeComponent();

        _healthService = App.ServiceProvider.GetRequiredService<IDbHealthService>();

        // Subscribe to connection status changes
        _healthService.ConnectionStatusChanged += OnConnectionStatusChanged;

        // Set initial status
        UpdateConnectionStatus(_healthService.IsConnected);

        // Also check on load in case the initial check already completed
        Loaded += async (s, e) =>
        {
            await _healthService.CheckConnectionAsync();
        };
    }

    public void ShowLogin()
    {
        var loginView = App.ServiceProvider.GetRequiredService<LoginView>();
        MainFrame.Navigate(loginView);
    }

    private void OnConnectionStatusChanged(bool isConnected)
    {
        // Ensure we're on the UI thread
        Dispatcher.BeginInvoke(() => UpdateConnectionStatus(isConnected));
    }

    private void UpdateConnectionStatus(bool isConnected)
    {
        if (isConnected)
        {
            ConnectionDot.Fill = FindResource("SuccessBrush") as Brush ?? Brushes.Green;
            ConnectionText.Text = "قاعدة البيانات: متصلة";
        }
        else
        {
            ConnectionDot.Fill = FindResource("DangerBrush") as Brush ?? Brushes.Red;
            ConnectionText.Text = "قاعدة البيانات: غير متصلة";
        }
    }
}
