using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TeraQuotation.Models;
using TeraQuotation.Services;
using TeraQuotation.Views;

namespace TeraQuotation.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly INavigationService _navigationService;

    public SettingsViewModel(ISettingsService settingsService, INavigationService navigationService)
    {
        _settingsService = settingsService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    void GoToQuotationForm()
    {
        _navigationService.NavigateTo<Views.QuotationFormView>();
    }

    [RelayCommand]
    void GoToReports()
    {
        _navigationService.NavigateTo<Views.ReportsView>();
    }

    [RelayCommand]
    void ChangePassword()
    {
        var vm = App.ServiceProvider.GetRequiredService<ChangePasswordViewModel>();
        var dialog = new ChangePasswordDialog(vm);
        dialog.Owner = App.Current.MainWindow;
        dialog.ShowDialog();
    }

    // ============== Suppliers ==============

    [ObservableProperty]
    private ObservableCollection<Supplier> _suppliers = new();

    [ObservableProperty]
    private Supplier? _selectedSupplier;

    [ObservableProperty]
    private string? _newSupplierName;

    [ObservableProperty]
    private string? _newSupplierContact;

    [ObservableProperty]
    private string? _newSupplierNotes;

    [RelayCommand]
    private async Task LoadSuppliersAsync()
    {
        try
        {
            var list = await _settingsService.GetAllSuppliersAsync();
            Suppliers = new ObservableCollection<Supplier>(list);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحميل الموردين: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task AddSupplierAsync()
    {
        if (string.IsNullOrWhiteSpace(NewSupplierName))
        {
            System.Windows.MessageBox.Show("الرجاء إدخال اسم المورد", "تنبيه",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var supplier = new Supplier
            {
                Name = NewSupplierName.Trim(),
                ContactInfo = NewSupplierContact?.Trim(),
                Notes = NewSupplierNotes?.Trim()
            };

            var added = await _settingsService.AddSupplierAsync(supplier);
            Suppliers.Add(added);

            // Clear form
            NewSupplierName = string.Empty;
            NewSupplierContact = string.Empty;
            NewSupplierNotes = string.Empty;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في إضافة المورد: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task UpdateSupplierAsync()
    {
        if (SelectedSupplier == null) return;

        try
        {
            await _settingsService.UpdateSupplierAsync(SelectedSupplier);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحديث المورد: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteSupplierAsync(int id)
    {
        var result = System.Windows.MessageBox.Show("هل أنت متأكد من حذف هذا المورد؟", "تأكيد الحذف",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            await _settingsService.DeleteSupplierAsync(id);
            var toRemove = Suppliers.FirstOrDefault(s => s.Id == id);
            if (toRemove != null) Suppliers.Remove(toRemove);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في حذف المورد: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    // ============== Items ==============

    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    private Item? _selectedItem;

    [ObservableProperty]
    private string? _itemSearch;

    [ObservableProperty]
    private string? _newItemName;

    [ObservableProperty]
    private string? _newItemDescription;

    [ObservableProperty]
    private string? _newItemUnit;

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        try
        {
            var list = await _settingsService.GetAllItemsAsync();
            Items = new ObservableCollection<Item>(list);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحميل القطع: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task SearchItemsAsync()
    {
        try
        {
            var list = await _settingsService.SearchItemsAsync(ItemSearch ?? string.Empty);
            Items = new ObservableCollection<Item>(list);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في البحث: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task AddItemAsync()
    {
        if (string.IsNullOrWhiteSpace(NewItemName))
        {
            System.Windows.MessageBox.Show("الرجاء إدخال اسم القطعة", "تنبيه",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var item = new Item
            {
                Name = NewItemName.Trim(),
                Description = NewItemDescription?.Trim(),
                Unit = string.IsNullOrWhiteSpace(NewItemUnit) ? "قطعة" : NewItemUnit.Trim()
            };

            var added = await _settingsService.AddItemAsync(item);
            Items.Add(added);

            NewItemName = string.Empty;
            NewItemDescription = string.Empty;
            NewItemUnit = string.Empty;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في إضافة القطعة: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task UpdateItemAsync()
    {
        if (SelectedItem == null) return;

        try
        {
            await _settingsService.UpdateItemAsync(SelectedItem);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحديث القطعة: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteItemAsync(int id)
    {
        var result = System.Windows.MessageBox.Show("هل أنت متأكد من حذف هذه القطعة؟", "تأكيد الحذف",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            await _settingsService.DeleteItemAsync(id);
            var toRemove = Items.FirstOrDefault(i => i.Id == id);
            if (toRemove != null) Items.Remove(toRemove);
        }
        catch (DbUpdateException)
        {
            System.Windows.MessageBox.Show(
                "لا يمكن حذف هذه القطعة لأنها مرتبطة بعرض سعر موجود.\n" +
                "قم بإزالة القطعة من عروض الأسعار أولاً ثم حاول مجدداً.",
                "تعذر الحذف",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في حذف القطعة: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    // ============== Signatures ==============

    [ObservableProperty]
    private ObservableCollection<Signature> _signatures = new();

    [ObservableProperty]
    private Signature? _selectedSignature;

    [ObservableProperty]
    private string? _newSignatureName;

    [RelayCommand]
    private async Task LoadSignaturesAsync()
    {
        try
        {
            var list = await _settingsService.GetAllSignaturesAsync();
            Signatures = new ObservableCollection<Signature>(list);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحميل التوقيعات: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task AddSignatureAsync()
    {
        if (string.IsNullOrWhiteSpace(NewSignatureName))
        {
            System.Windows.MessageBox.Show("الرجاء إدخال اسم التوقيع", "تنبيه",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var signature = new Signature
            {
                Name = NewSignatureName.Trim()
            };

            var added = await _settingsService.AddSignatureAsync(signature);
            Signatures.Add(added);

            NewSignatureName = string.Empty;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في إضافة التوقيع: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteSignatureAsync(int id)
    {
        var result = System.Windows.MessageBox.Show("هل أنت متأكد من حذف هذا التوقيع؟", "تأكيد الحذف",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            await _settingsService.DeleteSignatureAsync(id);
            var toRemove = Signatures.FirstOrDefault(s => s.Id == id);
            if (toRemove != null) Signatures.Remove(toRemove);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في حذف التوقيع: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task MoveUpSignatureAsync(int id)
    {
        try
        {
            await _settingsService.MoveSignatureUpAsync(id);
            await LoadSignaturesAsync(); // reload to reflect new order
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحريك التوقيع: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task MoveDownSignatureAsync(int id)
    {
        try
        {
            await _settingsService.MoveSignatureDownAsync(id);
            await LoadSignaturesAsync(); // reload to reflect new order
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحريك التوقيع: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    // ============== Letterhead ==============

    [ObservableProperty]
    private string? _companyName;

    [ObservableProperty]
    private string? _companyAddress;

    [ObservableProperty]
    private string? _companyPhone;

    [ObservableProperty]
    private string? _companyEmail;

    [RelayCommand]
    private async Task LoadLetterheadAsync()
    {
        try
        {
            CompanyName = await _settingsService.GetSettingAsync("CompanyName") ?? string.Empty;
            CompanyAddress = await _settingsService.GetSettingAsync("CompanyAddress") ?? string.Empty;
            CompanyPhone = await _settingsService.GetSettingAsync("CompanyPhone") ?? string.Empty;
            CompanyEmail = await _settingsService.GetSettingAsync("CompanyEmail") ?? string.Empty;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في تحميل بيانات الشركة: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task SaveLetterheadAsync()
    {
        try
        {
            await _settingsService.SetSettingAsync("CompanyName", CompanyName ?? string.Empty);
            await _settingsService.SetSettingAsync("CompanyAddress", CompanyAddress ?? string.Empty);
            await _settingsService.SetSettingAsync("CompanyPhone", CompanyPhone ?? string.Empty);
            await _settingsService.SetSettingAsync("CompanyEmail", CompanyEmail ?? string.Empty);

            System.Windows.MessageBox.Show("تم حفظ بيانات الشركة بنجاح.", "نجاح",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"خطأ في حفظ بيانات الشركة: {ex.Message}", "خطأ",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}
