using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TeraQuotation.Helpers;
using TeraQuotation.Models.Reports;
using TeraQuotation.Services;
using TeraQuotation.Views;

namespace TeraQuotation.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly INavigationService _navigation;

    public ReportsViewModel(IReportService reportService, INavigationService navigation)
    {
        _reportService = reportService;
        _navigation = navigation;
    }

    // ============== Properties ==============

    [ObservableProperty]
    private object? _currentReport;

    [ObservableProperty]
    private string? _reportTitle;

    [ObservableProperty]
    private bool _hasReport;

    [ObservableProperty]
    private string? _backupMessage;

    [ObservableProperty]
    private bool _isSupplierReport;

    [ObservableProperty]
    private bool _isItemReport;

    [ObservableProperty]
    private bool _isStatsReport;

    [ObservableProperty]
    private bool _isBackupVisible;

    // ============== Commands ==============

    [RelayCommand]
    private async Task LoadSupplierReport()
    {
        try
        {
            var data = await _reportService.GetSupplierReportAsync();
            CurrentReport = new ObservableCollection<SupplierReport>(data);
            ReportTitle = "تقرير الموردين";
            UpdateVisibility(supplier: true, item: false, stats: false, backup: false);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحميل تقرير الموردين: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task LoadItemReport()
    {
        try
        {
            var data = await _reportService.GetItemReportAsync();
            CurrentReport = new ObservableCollection<ItemReport>(data);
            ReportTitle = "تقرير القطع";
            UpdateVisibility(supplier: false, item: true, stats: false, backup: false);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحميل تقرير القطع: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task LoadStatsReport()
    {
        try
        {
            var data = await _reportService.GetQuotationStatsAsync();
            CurrentReport = data;
            ReportTitle = "إحصائيات العروض";
            UpdateVisibility(supplier: false, item: false, stats: true, backup: false);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحميل الإحصائيات: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task Backup()
    {
        try
        {
            var filePath = await BackupHelper.BackupAsync();
            BackupMessage = $"✅ تم إنشاء النسخة الاحتياطية بنجاح:\n{filePath}";
            UpdateVisibility(supplier: false, item: false, stats: false, backup: true);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في إنشاء النسخة الاحتياطية: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task Restore()
    {
        try
        {
            var result = await BackupHelper.RestoreAsync();
            if (result != null)
            {
                System.Windows.MessageBox.Show(
                    $"✅ تم استعادة النسخة الاحتياطية بنجاح.\nسيتم تطبيق التغييرات بعد إعادة تشغيل التطبيق.",
                    "نجاح",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في استعادة النسخة الاحتياطية: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Home()
    {
        _navigation.NavigateTo<SettingsView>();
    }

    // ============== Helpers ==============

    private void UpdateVisibility(bool supplier, bool item, bool stats, bool backup)
    {
        IsSupplierReport = supplier;
        IsItemReport = item;
        IsStatsReport = stats;
        IsBackupVisible = backup;
        HasReport = supplier || item || stats || backup;
    }
}
