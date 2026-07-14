using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using TeraQuotation.Helpers;
using TeraQuotation.Models;
using TeraQuotation.Services;

namespace TeraQuotation.ViewModels;

public partial class QuotationFormViewModel : ObservableObject, Services.IUnsavedChangesAware
{
    private readonly IQuotationService _quotationService;
    private readonly ISettingsService _settingsService;
    private readonly INavigationService _navigationService;
    private readonly IPdfService _pdfService;
    private readonly IOutlookService _outlookService;

    private List<Item> _allItems = new();
    private Quotation? _savedQuotation;

    public QuotationFormViewModel(
        IQuotationService quotationService,
        ISettingsService settingsService,
        INavigationService navigationService,
        IPdfService pdfService,
        IOutlookService outlookService)
    {
        _quotationService = quotationService;
        _settingsService = settingsService;
        _navigationService = navigationService;
        _pdfService = pdfService;
        _outlookService = outlookService;

        // Track items collection changes for computed properties
        Items.CollectionChanged += OnItemsCollectionChanged;
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

    [ObservableProperty]
    private string _status = "Draft";

    [ObservableProperty]
    private DateTime? _lastSaveTime;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    [ObservableProperty]
    private bool _isSaved;

    // ══════════════════════════════════════════════
    //  العنوان الديناميكي
    // ══════════════════════════════════════════════

    public string TitleText =>
        string.IsNullOrEmpty(QuoteNumber)
            ? "عرض سعر جديد"
            : $"تعديل عرض سعر {QuoteNumber}";

    public string LastSaveText
    {
        get
        {
            if (!IsSaved) return "غير محفوظ ●";
            if (LastSaveTime.HasValue)
                return $"آخر حفظ: {LastSaveTime.Value:HH:mm}";
            return "غير محفوظ ●";
        }
    }

    partial void OnQuoteNumberChanged(string? value)
    {
        OnPropertyChanged(nameof(TitleText));
    }

    partial void OnIsSavedChanged(bool value)
    {
        OnPropertyChanged(nameof(LastSaveText));
    }

    partial void OnLastSaveTimeChanged(DateTime? value)
    {
        OnPropertyChanged(nameof(LastSaveText));
    }

    partial void OnHasUnsavedChangesChanged(bool value)
    {
        OnPropertyChanged(nameof(LastSaveText));
    }

    // ══════════════════════════════════════════════
    //  نص الإرشادات (Guidance Strip)
    // ══════════════════════════════════════════════

    public string GuidanceText
    {
        get
        {
            if (SelectedSuppliers.Count == 0)
                return "ابدأ بإضافة مورد واحد على الأقل، ثم أضف المواد المطلوبة.";

            if (Items.Count == 0)
                return "تمت إضافة الموردين. أضف المواد التي تريد تسعيرها.";

            if (!AllItemsPriced)
                return "بعض المواد لم تكتمل أسعارها بعد. أكمل الأسعار لتفعيل الطباعة النهائية.";

            return "العرض مكتمل وجاهز للطباعة أو التصدير.";
        }
    }

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
    //  العنصر المحدد (للوحة التسعير)
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private QuotationItemRow? _selectedItem;

    partial void OnSelectedItemChanged(QuotationItemRow? value)
    {
        OnPropertyChanged(nameof(HasSelectedItem));
        OnPropertyChanged(nameof(PricingPanelTitle));
        OnPropertyChanged(nameof(SelectedItemQuantityText));
        OnPricingChanged();
    }

    public bool HasSelectedItem => SelectedItem != null;
    public string PricingPanelTitle => SelectedItem != null ? $"تسعير: {SelectedItem.DisplayName}" : "";
    public string SelectedItemQuantityText =>
        SelectedItem != null ? $"الكمية: {SelectedItem.Quantity:N0} {SelectedItem.Unit}" : "";

    // ══════════════════════════════════════════════
    //  خصائص محسوبة لحالة الأزرار
    // ══════════════════════════════════════════════

    public bool HasMaterials => Items.Count > 0;

    public bool AllItemsPriced
    {
        get
        {
            if (Items.Count == 0) return false;
            return Items.All(i => IsItemFullyPriced(i));
        }
    }

    public bool CanPrintWithoutPrices => HasMaterials;
    public bool CanPrintFinal => AllItemsPriced;
    public bool CanExportPdf => IsSaved;
    public bool CanSendOutlook => IsSaved;

    /// <summary>
    /// يُحدّث جميع الخصائص المحسوبة عند تغيير Items.
    /// </summary>
    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Subscribe to property changes on new items
        if (e.NewItems != null)
        {
            foreach (QuotationItemRow item in e.NewItems)
                item.PropertyChanged += OnItemPropertyChanged;
        }

        // Unsubscribe from removed items
        if (e.OldItems != null)
        {
            foreach (QuotationItemRow item in e.OldItems)
                item.PropertyChanged -= OnItemPropertyChanged;
        }

        RefreshComputedStates();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(QuotationItemRow.Item) or nameof(QuotationItemRow.Quantity) or nameof(QuotationItemRow.Unit))
        {
            RefreshComputedStates();
        }
    }

    /// <summary>
    /// يُحدّث جميع الخصائص المحسوبة ويعلم الواجهة.
    /// </summary>
    public void RefreshComputedStates()
    {
        OnPropertyChanged(nameof(HasMaterials));
        OnPropertyChanged(nameof(AllItemsPriced));
        OnPropertyChanged(nameof(CanPrintWithoutPrices));
        OnPropertyChanged(nameof(CanPrintFinal));
        OnPropertyChanged(nameof(CanExportPdf));
        OnPropertyChanged(nameof(CanSendOutlook));
        OnPropertyChanged(nameof(GuidanceText));
        HasUnsavedChanges = !IsSaved;
        OnPropertyChanged(nameof(LastSaveText));
    }

    /// <summary>
    /// يُستدعى عندما يتغير تسعير مادة ما (لتحديث حالة الأزرار).
    /// </summary>
    public void OnPricingChanged()
    {
        RefreshComputedStates();
    }

    // ══════════════════════════════════════════════
    //  حالة تسعير العنصر (لشاشة المواد)
    // ══════════════════════════════════════════════

    /// <summary>
    /// هل جميع أسعار هذا العنصر مكتملة عند جميع الموردين؟
    /// </summary>
    public bool IsItemFullyPriced(QuotationItemRow item)
    {
        if (SelectedSuppliers.Count == 0) return false;
        return SelectedSuppliers.All(s =>
            item.SupplierCells.TryGetValue(s.Id, out var cell) &&
            cell.Price.HasValue &&
            cell.Price.Value > 0);
    }

    /// <summary>
    /// هل بدأ تسعير هذا العنصر (واحد على الأقل)?
    /// </summary>
    public bool IsItemPartiallyPriced(QuotationItemRow item)
    {
        if (SelectedSuppliers.Count == 0) return false;
        return SelectedSuppliers.Any(s =>
            item.SupplierCells.TryGetValue(s.Id, out var cell) &&
            cell.Price.HasValue);
    }

    /// <summary>
    /// نص حالة تسعير العنصر.
    /// </summary>
    public string GetItemPricingStatus(QuotationItemRow item)
    {
        if (IsItemFullyPriced(item)) return "مكتمل";
        if (IsItemPartiallyPriced(item)) return "ناقص سعر";
        return "لم يبدأ التسعير";
    }

    /// <summary>
    /// يحسب أفضل سعر للعنصر ويعيد (قيمة, اسم المورد).
    /// </summary>
    public (decimal? BestPrice, string? BestSupplierName) GetItemBestPrice(QuotationItemRow item)
    {
        decimal? minPrice = null;
        string? bestSupplier = null;

        foreach (var supplier in SelectedSuppliers)
        {
            if (item.SupplierCells.TryGetValue(supplier.Id, out var cell) && cell.Price.HasValue)
            {
                if (!minPrice.HasValue || cell.Price.Value < minPrice.Value)
                {
                    minPrice = cell.Price.Value;
                    bestSupplier = supplier.Name;
                }
            }
        }

        return (minPrice, bestSupplier);
    }

    /// <summary>
    /// هل هذا المورد لديه أفضل سعر للعنصر المحدد؟
    /// </summary>
    public bool IsSupplierBestPrice(QuotationItemRow item, int supplierId)
    {
        if (item == null) return false;

        var (bestPrice, _) = GetItemBestPrice(item);
        if (!bestPrice.HasValue) return false;

        if (item.SupplierCells.TryGetValue(supplierId, out var cell) && cell.Price.HasValue)
            return cell.Price.Value == bestPrice.Value;

        return false;
    }

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

            // ربط تغييرات SelectedSuppliers بتحديث الإرشادات
            SelectedSuppliers.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(GuidanceText));
                RefreshComputedStates();
            };
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
        SelectedItem = row;

        ToastHelper.ShowSuccess("تمت إضافة المادة");
    }

    /// <summary>
    /// يحذف صف من الجدول.
    /// </summary>
    public void RemoveItemRow(QuotationItemRow? row)
    {
        if (row != null)
        {
            if (SelectedItem == row)
                SelectedItem = null;

            Items.Remove(row);
            ToastHelper.ShowSuccess("تم حذف المادة");
        }
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

        ToastHelper.ShowSuccess("تمت إضافة المورد");
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

        ToastHelper.ShowSuccess("تم حذف المورد");
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
            HasUnsavedChanges = false;

            // إنشاء عرض السعر
            var quotation = new Quotation
            {
                QuoteNumber = QuoteNumber!,
                Date = Date,
                Description = Description ?? "",
                Status = Status
            };

            if (_savedQuotation != null)
            {
                // تحديث العرض الحالي
                quotation.Id = _savedQuotation.Id;
                quotation.CreatedAt = _savedQuotation.CreatedAt;
                quotation.Items = _savedQuotation.Items;
            }

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

            _savedQuotation = quotation;
            IsSaved = true;
            LastSaveTime = DateTime.Now;
            Status = "Draft";

            ToastHelper.ShowSuccess($"تم حفظ عرض السعر {quotation.QuoteNumber} بنجاح");
        }
        catch (Exception ex)
        {
            HasUnsavedChanges = true;
            ToastHelper.ShowError($"تعذر حفظ العرض. حاول مرة أخرى.\n{ex.Message}");
        }
    }

    // ══════════════════════════════════════════════
    //  الطباعة بدون أسعار
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task PrintWithoutPricesAsync()
    {
        if (!HasMaterials)
        {
            ToastHelper.ShowWarning("أضف مادة واحدة على الأقل للطباعة");
            return;
        }

        try
        {
            // حفظ أولاً
            if (!IsSaved) await SaveAsync();
            if (!IsSaved) return;

            var quotation = await BuildQuotationForPrint();
            if (quotation == null) return;

            // استخدام الـ PrintHelper الموجود في المشروع
            PrintHelper.PrintQuotation(quotation, showPrices: false);

            Status = "UpdatedWithPrices";
            ToastHelper.ShowSuccess("تمت طباعة عرض السعر بدون أسعار");
        }
        catch (Exception ex)
        {
            ToastHelper.ShowError($"خطأ في الطباعة: {ex.Message}");
        }
    }

    // ══════════════════════════════════════════════
    //  الطباعة النهائية
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task PrintFinalAsync()
    {
        if (!AllItemsPriced)
        {
            ToastHelper.ShowWarning("لا يمكن الطباعة النهائية — توجد مواد بدون أسعار");
            return;
        }

        try
        {
            if (!IsSaved) await SaveAsync();
            if (!IsSaved) return;

            var quotation = await BuildQuotationForPrint();
            if (quotation == null) return;

            PrintHelper.PrintQuotation(quotation, showPrices: true);

            Status = "Printed";
            ToastHelper.ShowSuccess("تمت طباعة عرض السعر النهائية");
        }
        catch (Exception ex)
        {
            ToastHelper.ShowError($"خطأ في الطباعة: {ex.Message}");
        }
    }

    // ══════════════════════════════════════════════
    //  تصدير PDF
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task ExportPdfAsync()
    {
        if (!IsSaved)
        {
            ToastHelper.ShowWarning("احفظ العرض أولاً قبل تصدير PDF");
            return;
        }

        try
        {
            var quotation = await BuildQuotationForPrint();
            if (quotation == null) return;

            var pdfBytes = await _pdfService.GenerateQuotationPdfAsync(quotation.Id);

            string safeFileName = SanitizeFileName($"عرض_سعر_{quotation.QuoteNumber}.pdf");
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = System.IO.Path.Combine(desktopPath, safeFileName);

            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

            Status = "PDFExported";
            ToastHelper.ShowSuccess($"تم تصدير PDF بنجاح إلى سطح المكتب");
        }
        catch (Exception ex)
        {
            ToastHelper.ShowError($"خطأ في تصدير PDF: {ex.Message}");
        }
    }

    // ══════════════════════════════════════════════
    //  الإرسال عبر Outlook
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task SendViaOutlookAsync()
    {
        if (!IsSaved)
        {
            ToastHelper.ShowWarning("احفظ العرض أولاً، ثم يمكنك الإرسال عبر Outlook");
            return;
        }

        try
        {
            var quotation = await BuildQuotationForPrint();
            if (quotation == null) return;

            var pdfBytes = await _pdfService.GenerateQuotationPdfAsync(quotation.Id);
            var safeFileName = SanitizeFileName($"عرض_سعر_{quotation.QuoteNumber}.pdf");
            var pdfPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), safeFileName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            _outlookService.SendQuotationEmail(quotation.QuoteNumber, pdfPath);

            Status = "SentViaOutlook";
            ToastHelper.ShowSuccess("تم فتح بريد Outlook مع المرفق");
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            ToastHelper.ShowWarning("تعذر فتح Outlook. الرجاء التأكد من تثبيت Outlook.");
        }
        catch (Exception ex)
        {
            ToastHelper.ShowError($"خطأ في الإرسال عبر Outlook: {ex.Message}");
        }
    }

    // ══════════════════════════════════════════════
    //  أدوات مساعدة للتصدير والطباعة
    // ══════════════════════════════════════════════

    /// <summary>
    /// يبني كائن Quotation من البيانات الحالية للطباعة/التصدير.
    /// </summary>
    private async Task<Quotation?> BuildQuotationForPrint()
    {
        if (_savedQuotation == null) return null;

        var quotation = await _quotationService.GetQuotationByIdAsync(_savedQuotation.Id);
        if (quotation == null) return null;

        quotation.Date = Date;
        quotation.Description = Description ?? "";

        // مزامنة العناصر
        quotation.Items.Clear();
        foreach (var row in Items)
        {
            if (row.ItemId == 0) continue;

            var qi = new QuotationItem
            {
                QuotationId = quotation.Id,
                ItemId = row.ItemId,
                Quantity = row.Quantity,
                Unit = row.Unit,
            };

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
        return quotation;
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalid = System.IO.Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Select(ch => invalid.Contains(ch) ? '_' : ch));
    }

    // ══════════════════════════════════════════════
    //  IUnsavedChangesAware
    // ══════════════════════════════════════════════

    /// <summary>
    /// Shows the UnsavedChangesDialog with 3 options:
    /// "حفظ ثم خروج" → saves then returns true,
    /// "خروج بدون حفظ" → returns true,
    /// "إلغاء" → returns false (stay).
    /// </summary>
    public async Task<bool> ConfirmNavigateAwayAsync()
    {
        var dialog = new Views.Dialogs.UnsavedChangesDialog();
        dialog.Owner = System.Windows.Application.Current.MainWindow;
        dialog.ShowDialog();

        switch (dialog.Choice)
        {
            case Views.Dialogs.UnsavedChangesChoice.Save:
                await SaveAsync();
                return IsSaved; // true if save succeeded
            case Views.Dialogs.UnsavedChangesChoice.Discard:
                return true;
            case Views.Dialogs.UnsavedChangesChoice.Cancel:
            default:
                return false;
        }
    }

    // ══════════════════════════════════════════════
    //  التنقل
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task GoToList()
    {
        await _navigationService.TryGoBackAsync();
    }

    [RelayCommand]
    async Task GoToSettings()
    {
        await _navigationService.TryNavigateToAsync<Views.SettingsView>();
    }
}
