using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TeraQuotation.Helpers;
using TeraQuotation.Models;
using TeraQuotation.Services;

namespace TeraQuotation.ViewModels;

public partial class QuotationDetailViewModel : ObservableObject
{
    private readonly IQuotationService _quotationService;
    private readonly ISettingsService _settingsService;
    private readonly INavigationService _navigationService;
    private readonly NavigationParameter _navigationParameter;
    private readonly IPdfService _pdfService;
    private readonly IOutlookService _outlookService;

    private Quotation? _quotation;
    private List<Item> _allItems = new();

    // ══════════════════════════════════════════════
    //  بيانات العرض (Header)
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private int _quotationId;

    [ObservableProperty]
    private string? _quoteNumber;

    [ObservableProperty]
    private DateTime _quoteDate = DateTime.Today;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string _status = "Draft";

    // ══════════════════════════════════════════════
    //  الموردين
    // ══════════════════════════════════════════════

    /// <summary>الموردون المحددون لهذا العرض (يظهر أعمدتهم في الجدول).</summary>
    [ObservableProperty]
    private ObservableCollection<Supplier> _selectedSuppliers = new();

    /// <summary>جميع الموردين من قاعدة البيانات.</summary>
    [ObservableProperty]
    private ObservableCollection<Supplier> _allSuppliers = new();

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
    //  الحالة
    // ══════════════════════════════════════════════

    [ObservableProperty]
    private ObservableCollection<string> _availableStatuses = new();

    [ObservableProperty]
    private string? _selectedNextStatus;

    // ══════════════════════════════════════════════
    //  Constructor
    // ══════════════════════════════════════════════

    public QuotationDetailViewModel(
        IQuotationService quotationService,
        ISettingsService settingsService,
        INavigationService navigationService,
        NavigationParameter navigationParameter,
        IPdfService pdfService,
        IOutlookService outlookService)
    {
        _quotationService = quotationService;
        _settingsService = settingsService;
        _navigationService = navigationService;
        _navigationParameter = navigationParameter;
        _pdfService = pdfService;
        _outlookService = outlookService;
    }

    // ══════════════════════════════════════════════
    //  التهيئة
    // ══════════════════════════════════════════════

    public async Task InitializeAsync()
    {
        try
        {
            if (_navigationParameter.Value is not int id)
            {
                System.Windows.MessageBox.Show(
                    "لم يتم تحديد عرض السعر المطلوب.",
                    "خطأ",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                _navigationService.GoBack();
                return;
            }

            _quotation = await _quotationService.GetQuotationByIdAsync(id);
            if (_quotation == null)
            {
                System.Windows.MessageBox.Show(
                    "عرض السعر غير موجود.",
                    "خطأ",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                _navigationService.GoBack();
                return;
            }

            // ---- تحميل Header ----
            QuotationId = _quotation.Id;
            QuoteNumber = _quotation.QuoteNumber;
            QuoteDate = _quotation.Date;
            Description = _quotation.Description;
            Status = _quotation.Status;

            // ---- تحميل جميع الموردين ----
            var allSuppliers = await _settingsService.GetAllSuppliersAsync();
            AllSuppliers = new ObservableCollection<Supplier>(allSuppliers);

            // ---- استخراج الموردين المحفوظين من العناصر ----
            await BuildSelectedSuppliersFromSavedDataAsync();

            // ---- تحويل العناصر إلى QuotationItemRow ----
            BuildItemRows();

            // ---- تحميل الكتالوج ----
            _allItems = await _settingsService.GetAllItemsAsync();

            // ---- الحالة التالية ----
            RebuildAvailableStatuses();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في تحميل عرض السعر: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// يقرأ أسماء الموردين المحفوظة في العناصر ويُنشئ قائمة SelectedSuppliers.
    /// </summary>
    private Task BuildSelectedSuppliersFromSavedDataAsync()
    {
        var supplierNames = new List<string>();

        foreach (var qi in _quotation!.Items)
        {
            if (!string.IsNullOrWhiteSpace(qi.Supplier1Name) && !supplierNames.Contains(qi.Supplier1Name))
                supplierNames.Add(qi.Supplier1Name);
            if (!string.IsNullOrWhiteSpace(qi.Supplier2Name) && !supplierNames.Contains(qi.Supplier2Name))
                supplierNames.Add(qi.Supplier2Name);
            if (!string.IsNullOrWhiteSpace(qi.Supplier3Name) && !supplierNames.Contains(qi.Supplier3Name))
                supplierNames.Add(qi.Supplier3Name);
        }

        var matched = new ObservableCollection<Supplier>();
        foreach (var name in supplierNames)
        {
            var supplier = AllSuppliers.FirstOrDefault(s => s.Name == name);
            if (supplier != null)
                matched.Add(supplier);
        }

        SelectedSuppliers = matched;
        return Task.CompletedTask;
    }

    /// <summary>
    /// يحوّل عناصر Quotation إلى صفوف QuotationItemRow مع ملء SupplierCells.
    /// </summary>
    private void BuildItemRows()
    {
        var rows = new ObservableCollection<QuotationItemRow>();

        foreach (var qi in _quotation!.Items)
        {
            var row = new QuotationItemRow
            {
                Item = qi.Item,
                Quantity = qi.Quantity,
                Unit = qi.Unit,
            };

            // ملء خلايا الموردين حسب الترتيب
            for (int i = 0; i < SelectedSuppliers.Count && i < 3; i++)
            {
                var supplierId = SelectedSuppliers[i].Id;
                row.EnsureSupplierCell(supplierId);

                switch (i)
                {
                    case 0:
                        row.SupplierCells[supplierId].Type = qi.Supplier1Type;
                        row.SupplierCells[supplierId].Price = qi.Supplier1Price;
                        break;
                    case 1:
                        row.SupplierCells[supplierId].Type = qi.Supplier2Type;
                        row.SupplierCells[supplierId].Price = qi.Supplier2Price;
                        break;
                    case 2:
                        row.SupplierCells[supplierId].Type = qi.Supplier3Type;
                        row.SupplierCells[supplierId].Price = qi.Supplier3Price;
                        break;
                }
            }

            rows.Add(row);
        }

        Items = rows;
    }

    // ══════════════════════════════════════════════
    //  إدارة صفوف العناصر
    // ══════════════════════════════════════════════

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
    /// يختار قطعة لصف معين.
    /// </summary>
    public void SelectItemForRow(QuotationItemRow row, Item item)
    {
        row.Item = item;
    }

    // ══════════════════════════════════════════════
    //  بحث عن قطعة (داخل الخلية)
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
    //  الحالة
    // ══════════════════════════════════════════════

    private void RebuildAvailableStatuses()
    {
        var next = Status switch
        {
            "Draft"             => "UpdatedWithPrices",
            "UpdatedWithPrices" => "Printed",
            "Printed"           => "PDFExported",
            "PDFExported"       => "SentViaOutlook",
            _                   => null
        };

        AvailableStatuses = next is not null
            ? new ObservableCollection<string> { next }
            : new ObservableCollection<string>();

        SelectedNextStatus = next;
    }

    // ══════════════════════════════════════════════
    //  حفظ
    // ══════════════════════════════════════════════

    /// <summary>
    /// يُزامن صفوف الجدول مع كائن Quotation ثم يحفظها.
    /// </summary>
    private void SyncItemsToQuotation()
    {
        if (_quotation == null) return;

        _quotation.Items.Clear();
        foreach (var row in Items)
        {
            if (row.ItemId == 0) continue;

            var qi = new QuotationItem
            {
                QuotationId = _quotation.Id,
                ItemId = row.ItemId,
                Quantity = row.Quantity,
                Unit = row.Unit,
            };

            // ملء بيانات الموردين حسب الترتيب
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

            _quotation.Items.Add(qi);
        }
    }

    [RelayCommand]
    async Task SaveAsync()
    {
        try
        {
            if (_quotation == null) return;

            _quotation.Date = QuoteDate;
            _quotation.Description = Description ?? "";
            _quotation.Status = Status;

            SyncItemsToQuotation();

            await _quotationService.UpdateQuotationAsync(_quotation);

            System.Windows.MessageBox.Show(
                $"تم حفظ عرض السعر {_quotation.QuoteNumber} بنجاح.",
                "نجاح",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            _navigationService.GoBack();
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

    // ══════════════════════════════════════════════
    //  تحديث الحالة
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task AdvanceStatusAsync()
    {
        var next = SelectedNextStatus;
        if (string.IsNullOrEmpty(next)) return;

        try
        {
            if (_quotation == null) return;

            _quotation.Status = next;
            await _quotationService.UpdateQuotationAsync(_quotation);

            Status = next;
            RebuildAvailableStatuses();

            System.Windows.MessageBox.Show(
                $"تم تحديث الحالة إلى \"{next}\".",
                "نجاح",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في تحديث الحالة: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    // ══════════════════════════════════════════════
    //  الطباعة بدون أسعار
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task PrintWithoutPricesAsync()
    {
        if (Status != "Draft")
        {
            System.Windows.MessageBox.Show(
                "يمكن طباعة بدون أسعار فقط لعروض السعر التي لم تُحدّث أسعارها بعد (Draft).",
                "تنبيه",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (_quotation == null) return;

        try
        {
            _quotation.Date = QuoteDate;
            _quotation.Description = Description ?? "";
            SyncItemsToQuotation();

            PrintHelper.PrintQuotation(_quotation, showPrices: false);

            _quotation.Status = "UpdatedWithPrices";
            await _quotationService.UpdateQuotationAsync(_quotation);

            Status = "UpdatedWithPrices";
            RebuildAvailableStatuses();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في الطباعة: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    // ══════════════════════════════════════════════
    //  الطباعة النهائية
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task PrintFinalAsync()
    {
        if (Status != "UpdatedWithPrices")
        {
            System.Windows.MessageBox.Show(
                "يرجى تحديث الأسعار أولاً (طباعة بدون أسعار) قبل الطباعة النهائية.",
                "تنبيه",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (_quotation == null) return;

        try
        {
            _quotation.Date = QuoteDate;
            _quotation.Description = Description ?? "";
            SyncItemsToQuotation();

            PrintHelper.PrintQuotation(_quotation, showPrices: true);

            _quotation.Status = "Printed";
            await _quotationService.UpdateQuotationAsync(_quotation);

            Status = "Printed";
            RebuildAvailableStatuses();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في الطباعة: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    // ══════════════════════════════════════════════
    //  تصدير PDF
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task ExportPdfAsync()
    {
        if (Status != "Printed" && Status != "PDFExported")
        {
            System.Windows.MessageBox.Show(
                "يرجى طباعة عرض السعر نهائياً أولاً قبل تصدير PDF.",
                "تنبيه",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (_quotation == null) return;

        try
        {
            _quotation.Date = QuoteDate;
            _quotation.Description = Description ?? "";
            SyncItemsToQuotation();

            await _quotationService.UpdateQuotationAsync(_quotation);

            var pdfBytes = await _pdfService.GenerateQuotationPdfAsync(_quotation.Id);

            string safeFileName = SanitizeFileName($"عرض_سعر_{_quotation.QuoteNumber}.pdf");
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = System.IO.Path.Combine(desktopPath, safeFileName);

            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

            _quotation.Status = "PDFExported";
            await _quotationService.UpdateQuotationAsync(_quotation);

            Status = "PDFExported";
            RebuildAvailableStatuses();

            System.Windows.MessageBox.Show(
                $"تم تصدير PDF بنجاح.\nالملف: {filePath}",
                "نجاح",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في تصدير PDF: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    // ══════════════════════════════════════════════
    //  الإرسال عبر Outlook
    // ══════════════════════════════════════════════

    [RelayCommand]
    async Task SendViaOutlookAsync()
    {
        if (Status != "PDFExported" && Status != "SentViaOutlook")
        {
            System.Windows.MessageBox.Show(
                "يرجى تصدير PDF أولاً قبل الإرسال عبر Outlook.",
                "تنبيه",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (_quotation == null) return;

        try
        {
            _quotation.Date = QuoteDate;
            _quotation.Description = Description ?? "";
            SyncItemsToQuotation();
            await _quotationService.UpdateQuotationAsync(_quotation);

            var pdfBytes = await _pdfService.GenerateQuotationPdfAsync(_quotation.Id);
            var safeFileName = SanitizeFileName($"عرض_سعر_{_quotation.QuoteNumber}.pdf");
            var pdfPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), safeFileName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            _outlookService.SendQuotationEmail(_quotation.QuoteNumber, pdfPath);

            _quotation.Status = "SentViaOutlook";
            await _quotationService.UpdateQuotationAsync(_quotation);

            Status = "SentViaOutlook";
            RebuildAvailableStatuses();

            System.Windows.MessageBox.Show(
                "تم فتح بريد Outlook مع المرفق.",
                "إرسال",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
        catch (System.Runtime.InteropServices.COMException ex)
        {
            System.Windows.MessageBox.Show(
                $"تعذر فتح Outlook. الرجاء التأكد من تثبيت Outlook.\n{ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في الإرسال عبر Outlook: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalid = System.IO.Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Select(ch => invalid.Contains(ch) ? '_' : ch));
    }

    // ══════════════════════════════════════════════
    //  التنقل
    // ══════════════════════════════════════════════

    [RelayCommand]
    void GoToList()
    {
        _navigationService.GoBack();
    }
}
