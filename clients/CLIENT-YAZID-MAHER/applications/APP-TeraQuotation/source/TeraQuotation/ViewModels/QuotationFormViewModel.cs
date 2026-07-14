using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TeraQuotation.Models;
using TeraQuotation.Services;

namespace TeraQuotation.ViewModels;

public partial class QuotationFormViewModel : ObservableObject
{
    private readonly IQuotationService _quotationService;
    private readonly ISettingsService _settingsService;
    private readonly INavigationService _navigationService;

    private List<Item> _allItems = new();

    public QuotationFormViewModel(
        IQuotationService quotationService,
        ISettingsService settingsService,
        INavigationService navigationService)
    {
        _quotationService = quotationService;
        _settingsService = settingsService;
        _navigationService = navigationService;
    }

    // ══════════════════════════════════════════════
    //  بيانات العرض (Header)
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private string? _quoteNumber;

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    private string? _description;

    // ══════════════════════════════════════════════
    //  الموردين
    // ══════════════════════════════════════════════

    /// <summary>الموردون المحددون لهذا العرض (يظهر أعمدتهم في الجدول).</summary>
    [ObservableProperty]
    private ObservableCollection<Supplier> _selectedSuppliers = new();

    /// <summary>الموردون المتاحون للإضافة (بعد استبعاد المحددين).</summary>
    [ObservableProperty]
    private ObservableCollection<Supplier> _availableSuppliers = new();

    // ══════════════════════════════════════════════
    //  عناصر الجدول
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private ObservableCollection<QuotationItemRow> _items = new();

    // ══════════════════════════════════════════════
    //  بحث القطع (داخل الخلية)
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private string? _itemSearchText;

    [ObservableProperty]
    private ObservableCollection<Item> _filteredItems = new();

    [ObservableProperty]
    private QuotationItemRow? _currentItemSearchRow;

    // ══════════════════════════════════════════════
    //  بحث الموردين
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private string? _supplierSearchText;

    [ObservableProperty]
    private ObservableCollection<Supplier> _filteredSuppliers = new();

    // ══════════════════════════════════════════════
    //  الحد الأقصى
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private int _maxSuppliers = 3;

    // ══════════════════════════════════════════════
    //  التهيئة
    // ══════════════════════════════════════════════

    public async Task InitializeAsync()
    {
        try
        {
            QuoteNumber = await _quotationService.GenerateNextQuoteNumberAsync();

            _allItems = await _settingsService.GetAllItemsAsync();
            FilteredItems = new ObservableCollection<Item>(_allItems);

            var allSuppliers = await _settingsService.GetAllSuppliersAsync();
            AvailableSuppliers = new ObservableCollection<Supplier>(allSuppliers);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في تحميل البيانات: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    // ══════════════════════════════════════════════
    //  إدارة صفوف العناصر
    // ══════════════════════════════════════════════

    /// <summary>
    /// يضيف صف فارغ جديد. يُملأ تلقائياً بخلايا الموردين المحددين.
    /// </summary>
    [RelayCommand]
    void AddItemRow()
    {
        var row = new QuotationItemRow();

        foreach (var supplier in SelectedSuppliers)
            row.EnsureSupplierCell(supplier.Id);

        Items.Add(row);
    }

    /// <summary>
    /// يحذف صف من الجدول.
    /// </summary>
    public void RemoveItemRow(QuotationItemRow? row)
    {
        if (row != null)
            Items.Remove(row);
    }

    /// <summary>
    /// يختار قطعة لصف معين. تُنسخ الوحدة تلقائياً من تعريف القطعة.
    /// </summary>
    public void SelectItemForRow(QuotationItemRow row, Item item)
    {
        row.Item = item;
    }

    // ══════════════════════════════════════════════
    //  إدارة الموردين
    // ══════════════════════════════════════════════

    /// <summary>
    /// يضيف مورداً للعرض ويُنشئ عمودَيْن ديناميكياً (نوع + سعر).
    /// </summary>
    [RelayCommand]
    void AddSupplier(Supplier? supplier)
    {
        if (supplier == null) return;
        if (SelectedSuppliers.Count >= MaxSuppliers) return;
        if (SelectedSuppliers.Any(s => s.Id == supplier.Id)) return;

        SelectedSuppliers.Add(supplier);
        AvailableSuppliers.Remove(supplier);

        // إضافة خلية مورد فارغة لكل صف موجود
        foreach (var row in Items)
            row.EnsureSupplierCell(supplier.Id);
    }

    /// <summary>
    /// يحذف مورداً من العرض ويحذف العمودَيْن المقابلَيْن.
    /// </summary>
    public void RemoveSupplier(Supplier? supplier)
    {
        if (supplier == null) return;

        SelectedSuppliers.Remove(supplier);
        AvailableSuppliers.Add(supplier);

        // حذف خلية المورد من كل صف
        foreach (var row in Items)
            row.RemoveSupplierCell(supplier.Id);
    }

    // ══════════════════════════════════════════════
    //  البحث عن قطعة (داخل الخلية)
    // ══════════════════════════════════════════════

    public void SearchItems()
    {
        if (string.IsNullOrWhiteSpace(ItemSearchText))
        {
            FilteredItems = new ObservableCollection<Item>(_allItems);
        }
        else
        {
            FilteredItems = new ObservableCollection<Item>(
                _allItems.Where(i =>
                    i.Name.Contains(ItemSearchText, StringComparison.OrdinalIgnoreCase)));
        }
    }

    /// <summary>
    /// يختار قطعة من نتائج البحث ويُعيّنها للصف الحالي المفتوح.
    /// </summary>
    public void SelectItemForCurrentRow(Item? item)
    {
        if (item == null || CurrentItemSearchRow == null) return;

        SelectItemForRow(CurrentItemSearchRow, item);
    }

    // ══════════════════════════════════════════════
    //  البحث عن مورد
    // ══════════════════════════════════════════════

    public void SearchSuppliers()
    {
        if (string.IsNullOrWhiteSpace(SupplierSearchText))
        {
            FilteredSuppliers = new ObservableCollection<Supplier>(AvailableSuppliers);
        }
        else
        {
            FilteredSuppliers = new ObservableCollection<Supplier>(
                AvailableSuppliers.Where(s =>
                    s.Name.Contains(SupplierSearchText, StringComparison.OrdinalIgnoreCase)));
        }
    }

    /// <summary>
    /// يختار مورداً من نتائج البحث ويضيفه للعرض.
    /// </summary>
    public void SelectSupplierFromSearch(Supplier? supplier)
    {
        if (supplier == null) return;
        AddSupplier(supplier);
    }

    // ══════════════════════════════════════════════
    //  حفظ عرض السعر
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task SaveAsync()
    {
        try
        {
            // إنشاء عرض السعر
            var quotation = new Quotation
            {
                QuoteNumber = QuoteNumber!,
                Date = Date,
                Description = Description ?? "",
                Status = "Draft"
            };

            quotation = await _quotationService.CreateQuotationAsync(quotation);

            // إضافة العناصر
            foreach (var row in Items)
            {
                if (row.ItemId == 0) continue; // تخطي صفوف فارغة

                var qi = new QuotationItem
                {
                    QuotationId = quotation.Id,
                    ItemId = row.ItemId,
                    Quantity = row.Quantity,
                    Unit = row.Unit,
                };

                // ملء بيانات الموردين حسب الترتيب (حد أقصى 3)
                for (int i = 0; i < SelectedSuppliers.Count && i < 3; i++)
                {
                    var supplierId = SelectedSuppliers[i].Id;
                    if (!row.SupplierCells.TryGetValue(supplierId, out var cell)) continue;

                    switch (i)
                    {
                        case 0:
                            qi.Supplier1Name = SelectedSuppliers[i].Name;
                            qi.Supplier1Type = cell.Type;
                            qi.Supplier1Price = cell.Price;
                            break;
                        case 1:
                            qi.Supplier2Name = SelectedSuppliers[i].Name;
                            qi.Supplier2Type = cell.Type;
                            qi.Supplier2Price = cell.Price;
                            break;
                        case 2:
                            qi.Supplier3Name = SelectedSuppliers[i].Name;
                            qi.Supplier3Type = cell.Type;
                            qi.Supplier3Price = cell.Price;
                            break;
                    }
                }

                quotation.Items.Add(qi);
            }

            await _quotationService.UpdateQuotationAsync(quotation);

            System.Windows.MessageBox.Show(
                $"تم حفظ عرض السعر {quotation.QuoteNumber} بنجاح.",
                "نجاح",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            // الانتقال لقائمة عروض الأسعار
            _navigationService.NavigateTo<Views.QuotationListView>();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في حفظ عرض السعر: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    void GoToSettings()
    {
        _navigationService.NavigateTo<Views.SettingsView>();
    }
}
