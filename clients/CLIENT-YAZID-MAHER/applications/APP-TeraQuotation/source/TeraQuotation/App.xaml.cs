using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TeraQuotation.Data;
using TeraQuotation.Services;
using TeraQuotation.ViewModels;
using TeraQuotation.Views;

namespace TeraQuotation;

public partial class App : Application
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Data
        var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TeraQuotation.db");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IQuotationService, QuotationService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IReportService, ReportService>();
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<IOutlookService, OutlookService>();
        services.AddSingleton<IBackupService, BackupService>();
        services.AddSingleton<NavigationParameter>();
        services.AddSingleton<IDbHealthService, DbHealthService>();
        services.AddSingleton<INavigationService>(sp =>
        {
            var mainWindow = sp.GetRequiredService<MainWindow>();
            return new NavigationService(mainWindow.FindName("MainFrame") as System.Windows.Controls.Frame
                ?? throw new InvalidOperationException("MainFrame not found in MainWindow"));
        });

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<QuotationFormViewModel>();
        services.AddTransient<QuotationListViewModel>();
        services.AddTransient<QuotationDetailViewModel>();
        services.AddTransient<ChangePasswordViewModel>();

        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<SettingsView>();
        services.AddTransient<ReportsView>();
        services.AddTransient<QuotationFormView>();
        services.AddTransient<QuotationListView>();
        services.AddTransient<QuotationDetailView>();
        services.AddTransient<ChangePasswordDialog>();
        services.AddSingleton<MainWindow>();

        ServiceProvider = services.BuildServiceProvider();

        // Apply migrations on startup
        using (var scope = ServiceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        // Initial DB health check (fire-and-forget)
        var healthService = ServiceProvider.GetRequiredService<IDbHealthService>();
        _ = healthService.CheckConnectionAsync();

        // Show MainWindow with login
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
        mainWindow.ShowLogin();
    }
}
