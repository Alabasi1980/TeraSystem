using System.Windows;
using System.Windows.Controls;
using TeraQuotation.Models;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

public partial class QuotationDetailView : UserControl
{
    private readonly QuotationDetailViewModel _viewModel;
    private readonly HashSet<DataGridColumn> _supplierColumns = new();
    private bool _isLoadingCell;

    public QuotationDetailView(QuotationDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // Register local converter so XAML bindings can use it
        Resources["InverseNullToBoolConverter"] = new InverseNullToBoolConverter();

        Loaded += async (s, e) =>
        {
            await viewModel.InitializeAsync();
            RebuildSupplierColumns();
        };

        // إعادة بناء الأعمدة عند تغيير قائمة الموردين
        viewModel.SelectedSuppliers.CollectionChanged += (s, e) =>
            Dispatcher.BeginInvoke(RebuildSupplierColumns);
    }

    // ══════════════════════════════════════════════
    //  إدارة الأعمدة الديناميكية (الموردين)
    // ══════════════════════════════════════════════

    /// <summary>
    /// يحذف الأعمدة القديمة للموردين ويُنشئ أعمدة جديدة حسب القائمة المحددة.
    /// كل مورد يحصل على عمودَيْن: "النوع" + "السعر".
    /// </summary>
    private void RebuildSupplierColumns()
    {
        // ── حذف الأعمدة القديمة ──
        foreach (var col in _supplierColumns)
            ItemsDataGrid.Columns.Remove(col);
        _supplierColumns.Clear();

        // ── تأكد من وجود خلايا موردين لكل صف ──
        foreach (var row in _viewModel.Items)
            foreach (var supplier in _viewModel.SelectedSuppliers)
                row.EnsureSupplierCell(supplier.Id);

        // ── إضافة الأعمدة الجديدة (قبل عمود الحذف) ──
        var deleteColumn = ItemsDataGrid.Columns
            .FirstOrDefault(c => c.Header?.ToString() == "حذف");

        int insertIndex = deleteColumn != null
            ? ItemsDataGrid.Columns.IndexOf(deleteColumn)
            : ItemsDataGrid.Columns.Count;

        foreach (var supplier in _viewModel.SelectedSuppliers)
        {
            var typeCol = CreateDynamicColumn(supplier, isPrice: false);
            ItemsDataGrid.Columns.Insert(insertIndex, typeCol);
            _supplierColumns.Add(typeCol);
            insertIndex++;

            var priceCol = CreateDynamicColumn(supplier, isPrice: true);
            ItemsDataGrid.Columns.Insert(insertIndex, priceCol);
            _supplierColumns.Add(priceCol);
            insertIndex++;
        }
    }

    /// <summary>
    /// يُنشئ عموداً ديناميكياً (نوع أو سعر) لمورد معين.
    /// </summary>
    private DataGridTemplateColumn CreateDynamicColumn(Supplier supplier, bool isPrice)
    {
        var header = isPrice
            ? $"{supplier.Name} \u2014 سعر"
            : $"{supplier.Name} \u2014 نوع";

        var col = new DataGridTemplateColumn
        {
            Header = header,
            Width = new DataGridLength(100),
        };

        var factory = new FrameworkElementFactory(typeof(TextBox));
        factory.SetValue(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        factory.SetValue(TextBox.MarginProperty, new Thickness(2));
        factory.SetValue(TextBox.FontSizeProperty, 12.0);
        factory.SetValue(TextBox.TagProperty, new SupplierColumnTag(supplier.Id, isPrice));

        if (isPrice)
        {
            factory.SetValue(TextBox.TextAlignmentProperty, TextAlignment.Center);
            factory.SetValue(TextBox.WidthProperty, 80.0);
        }
        else
        {
            factory.SetValue(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        }

        factory.AddHandler(FrameworkElement.LoadedEvent,
            new RoutedEventHandler(OnDynamicCellLoaded));
        factory.AddHandler(TextBox.TextChangedEvent,
            new TextChangedEventHandler(OnDynamicCellTextChanged));

        col.CellTemplate = new DataTemplate { VisualTree = factory };
        return col;
    }

    /// <summary>
    /// يُعبّئ خلية المورد بالبيانات عند تحميل TextBox.
    /// </summary>
    private void OnDynamicCellLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox tb) return;
        if (tb.DataContext is not QuotationItemRow row) return;
        if (tb.Tag is not SupplierColumnTag tag) return;

        _isLoadingCell = true;

        if (row.SupplierCells.TryGetValue(tag.SupplierId, out var cell))
        {
            tb.Text = tag.IsPrice
                ? (cell.Price?.ToString("N2") ?? "")
                : (cell.Type ?? "");
        }
        else
        {
            tb.Text = "";
        }

        _isLoadingCell = false;
    }

    /// <summary>
    /// يكتب قيمة الخلية في القاموس عند تغيير النص.
    /// </summary>
    private void OnDynamicCellTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoadingCell) return;
        if (sender is not TextBox tb) return;
        if (tb.DataContext is not QuotationItemRow row) return;
        if (tb.Tag is not SupplierColumnTag tag) return;

        if (!row.SupplierCells.TryGetValue(tag.SupplierId, out var cell)) return;

        if (tag.IsPrice)
        {
            cell.Price = decimal.TryParse(tb.Text, out var price) ? price : null;
        }
        else
        {
            cell.Type = string.IsNullOrWhiteSpace(tb.Text) ? null : tb.Text;
        }
    }

    // ══════════════════════════════════════════════
    //  بحث عن قطعة (داخل الخلية)
    // ══════════════════════════════════════════════

    private void ItemSearch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not QuotationItemRow row) return;

        _viewModel.CurrentItemSearchRow = row;
        _viewModel.ItemSearchText = "";
        _viewModel.SearchItems();

        ItemSearchPopup.PlacementTarget = btn;
        ItemSearchPopup.IsOpen = true;
        ItemSearchBox.Focus();
    }

    private void ItemSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.ItemSearchText = ItemSearchBox.Text;
        _viewModel.SearchItems();
    }

    private void ItemResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox lb) return;
        if (lb.SelectedItem is not Item item) return;

        _viewModel.SelectItemForCurrentRow(item);

        ItemSearchPopup.IsOpen = false;
        lb.SelectedItem = null;
        ItemSearchBox.Text = "";
    }

    // ══════════════════════════════════════════════
    //  إجراءات الصفوف
    // ══════════════════════════════════════════════

    private void RemoveRow_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not QuotationItemRow row) return;

        _viewModel.RemoveItemRow(row);
    }
}

/// <summary>
/// Converts a Collection.Count (int) to bool: true when Count > 0.
/// Enables the ComboBox when there are available next statuses.
/// </summary>
internal class InverseNullToBoolConverter : System.Windows.Data.IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is int count)
            return count > 0;
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
