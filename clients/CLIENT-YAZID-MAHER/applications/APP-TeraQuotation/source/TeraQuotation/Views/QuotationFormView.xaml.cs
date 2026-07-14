using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using TeraQuotation.Models;
using TeraQuotation.Services;
using TeraQuotation.ViewModels;

namespace TeraQuotation.Views;

public partial class QuotationFormView : UserControl
{
    private readonly QuotationFormViewModel _viewModel;

    public QuotationFormView(QuotationFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // Subscribe to DB health status
        var healthService = App.ServiceProvider.GetRequiredService<IDbHealthService>();
        UpdateFormConnectionIndicator(healthService.IsConnected);
        healthService.ConnectionStatusChanged += isConnected =>
            Dispatcher.BeginInvoke(() => UpdateFormConnectionIndicator(isConnected));

        Loaded += async (s, e) =>
        {
            await viewModel.InitializeAsync();

            // Update pricing status on material cards after loading
            UpdateMaterialCardsStatus();
            UpdateSupplierCardsStatus();
        };

        // Update material status badges when selection changes
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(QuotationFormViewModel.SelectedItem)
                or nameof(QuotationFormViewModel.SelectedSuppliers))
            {
                Dispatcher.BeginInvoke(() =>
                {
                    UpdateMaterialCardsStatus();
                    UpdateSupplierCardsStatus();
                });
            }
        };

        // Update when items collection changes
        viewModel.Items.CollectionChanged += (s, e) =>
            Dispatcher.BeginInvoke(UpdateMaterialCardsStatus);

        viewModel.SelectedSuppliers.CollectionChanged += (s, e) =>
            Dispatcher.BeginInvoke(UpdateSupplierCardsStatus);
    }

    // ══════════════════════════════════════════════
    //  تحديث بطاقات المواد (حالة التسعير، أفضل سعر)
    // ══════════════════════════════════════════════

    /// <summary>
    /// يُحدّث حالة التسعير وأفضل سعر لكل بطاقة مادة في القائمة.
    /// </summary>
    private void UpdateMaterialCardsStatus()
    {
        if (MaterialsListBox == null) return;

        foreach (var item in MaterialsListBox.Items)
        {
            if (item is not QuotationItemRow row) continue;

            var container = MaterialsListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (container == null) continue;

            // Find the status badge and text
            var statusBadge = FindVisualChild<Border>(container, "ItemStatusBadge");
            var statusText = FindVisualChild<TextBlock>(container, "ItemStatusText");
            var bestPriceText = FindVisualChild<TextBlock>(container, "BestPriceText");

            if (statusText != null)
            {
                var pricingStatus = _viewModel.GetItemPricingStatus(row);
                statusText.Text = pricingStatus;

                // Update badge style based on status
                if (statusBadge != null)
                {
                    switch (pricingStatus)
                    {
                        case "مكتمل":
                            statusBadge.Background = FindResource("SuccessBgBrush") as Brush;
                            statusText.Foreground = FindResource("SuccessBrush") as Brush;
                            break;
                        case "ناقص سعر":
                            statusBadge.Background = FindResource("WarningBgBrush") as Brush;
                            statusText.Foreground = FindResource("WarningBrush") as Brush;
                            break;
                        default:
                            statusBadge.Background = FindResource("BorderLightBrush") as Brush;
                            statusText.Foreground = FindResource("TextSecondaryBrush") as Brush;
                            break;
                    }
                }
            }

            // Best price
            if (bestPriceText != null)
            {
                var (bestPrice, bestSupplier) = _viewModel.GetItemBestPrice(row);
                if (bestPrice.HasValue && bestSupplier != null)
                {
                    bestPriceText.Text = $"أفضل سعر: {bestPrice.Value:N2}";
                    bestPriceText.Visibility = Visibility.Visible;
                }
                else
                {
                    bestPriceText.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    /// <summary>
    /// يُحدّث حالة التسعير لكل بطاقة مورد في اللوحة اليمنى.
    /// </summary>
    private void UpdateSupplierCardsStatus()
    {
        // This is called when supplier status changes
        // The supplier cards are in the right panel via ItemsControl
        // We find them by traversing the visual tree
        RefreshSupplierStatusVisuals();
    }

    private void RefreshSupplierStatusVisuals()
    {
        // Find the right panel's ItemsControl for suppliers
        // The supplier cards template has a TextBlock named "SupplierStatusText"
        // We'll update it by looking through the visual tree

        var rightPanelScroll = FindVisualChild<ScrollViewer>(this, null);
        if (rightPanelScroll == null) return;

        // Walk through the visual tree to find supplier status texts
        FindAndUpdateSupplierStatusTexts(this);
    }

    private void FindAndUpdateSupplierStatusTexts(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is TextBlock tb && tb.Name == "SupplierStatusText")
            {
                // Find the data context (Supplier)
                if (tb.DataContext is Supplier supplier)
                {
                    tb.Text = GetSupplierPricingStatus(supplier);
                }
            }

            FindAndUpdateSupplierStatusTexts(child);
        }
    }

    /// <summary>
    /// يحسب حالة تسعير مورد معين عبر جميع المواد.
    /// </summary>
    private string GetSupplierPricingStatus(Supplier supplier)
    {
        int totalItems = 0;
        int pricedItems = 0;

        foreach (var row in _viewModel.Items)
        {
            if (row.ItemId == 0) continue;
            totalItems++;

            if (row.SupplierCells.TryGetValue(supplier.Id, out var cell) && cell.Price.HasValue && cell.Price.Value > 0)
                pricedItems++;
        }

        if (totalItems == 0) return "لم يبدأ";
        if (pricedItems == 0) return "لم يبدأ";
        if (pricedItems >= totalItems) return "مكتمل";
        return "ناقص";
    }

    // ══════════════════════════════════════════════
    //  إضافة مادة من نموذج الإضافة
    // ══════════════════════════════════════════════

    /// <summary>
    /// معالج النقر على زر "🔍" بجانب حقل اسم المادة.
    /// يفتح نافذة البحث عن قطعة.
    /// </summary>
    private void ItemSearchFromAddForm_Click(object sender, RoutedEventArgs e)
    {
        // For the add form, we'll use a temporary row approach
        // Set up the search popup to target the sender
        _viewModel.ItemSearchText = NewItemNameBox.Text;
        _viewModel.SearchItems();

        ItemSearchPopup.PlacementTarget = sender as FrameworkElement;
        ItemSearchPopup.IsOpen = true;
        ItemSearchBox.Focus();
    }

    /// <summary>
    /// معالج النقر على زر "+ إضافة مادة" في النموذج.
    /// </summary>
    private void AddMaterialFromForm_Click(object sender, RoutedEventArgs e)
    {
        // Clear previous validation messages
        ClearAddFormValidation();

        bool isValid = true;

        // Validate item name — must not be empty
        if (string.IsNullOrWhiteSpace(NewItemNameBox.Text))
        {
            NewItemNameValidation.Visibility = Visibility.Visible;
            isValid = false;
        }

        // Validate quantity — must be a positive number
        if (!decimal.TryParse(NewItemQuantityBox.Text, out var qty) || qty <= 0)
        {
            NewItemQuantityValidation.Visibility = Visibility.Visible;
            isValid = false;
        }

        if (!isValid) return;

        // Create a new row and set its values
        _viewModel.AddItemRowCommand.Execute(null);

        // Set the last added row's qty and unit
        if (_viewModel.Items.Count > 0)
        {
            var lastRow = _viewModel.Items[^1];
            lastRow.Quantity = qty;
            lastRow.Unit = string.IsNullOrWhiteSpace(NewItemUnitBox.Text) ? "قطعة" : NewItemUnitBox.Text;
        }

        // Update material card status
        Dispatcher.BeginInvoke(UpdateMaterialCardsStatus);
    }

    /// <summary>
    /// يخفي جميع رسائل التحقق من صحة نموذج الإضافة.
    /// </summary>
    private void ClearAddFormValidation()
    {
        NewItemNameValidation.Visibility = Visibility.Collapsed;
        NewItemQuantityValidation.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// معالج تغيير النص في حقل اسم المادة — يخفي رسالة التحقق.
    /// </summary>
    private void NewItemNameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        NewItemNameValidation.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// معالج تغيير النص في حقل الكمية — يخفي رسالة التحقق.
    /// </summary>
    private void NewItemQuantityBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        NewItemQuantityValidation.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// يختار قطعة من نتائج البحث للصف الجديد (من نموذج الإضافة).
    /// </summary>
    private void SelectItemForNewRow(Item item)
    {
        _viewModel.AddItemRowCommand.Execute(null);

        if (_viewModel.Items.Count > 0)
        {
            var lastRow = _viewModel.Items[^1];
            _viewModel.SelectItemForRow(lastRow, item);

            // Update display
            NewItemNameBox.Text = item.Name;
            NewItemUnitBox.Text = item.Unit;
        }
    }

    // ══════════════════════════════════════════════
    //  بحث عن قطعة (داخل الخلية — من الجدول القديم)
    // ══════════════════════════════════════════════

    /// <summary>
    /// يُفتح عند النقر على زر القطعة داخل خلية القائمة.
    /// (مُحتفظ به للتوافق مع الكود القديم—يُستخدم حالياً من نموذج الإضافة أيضاً)
    /// </summary>
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

        if (lb.SelectedItem is Item item)
        {
            // Check if we're adding from the add-form or editing an existing row
            if (_viewModel.CurrentItemSearchRow != null)
            {
                // Existing row edit
                _viewModel.SelectItemForCurrentRow(item);
            }
            else
            {
                // New row from add form
                SelectItemForNewRow(item);
            }

            ItemSearchPopup.IsOpen = false;
            lb.SelectedItem = null;
            ItemSearchBox.Text = "";
        }
    }

    // ══════════════════════════════════════════════
    //  بحث عن مورد
    // ══════════════════════════════════════════════

    private void SupplierSearchButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SupplierSearchText = "";
        _viewModel.SearchSuppliers();

        SupplierSearchPopup.PlacementTarget = sender as FrameworkElement;
        SupplierSearchPopup.IsOpen = true;
        SupplierSearchBox.Focus();
    }

    private void SupplierSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.SupplierSearchText = SupplierSearchBox.Text;
        _viewModel.SearchSuppliers();
    }

    private void SupplierResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox lb) return;
        if (lb.SelectedItem is not Supplier supplier) return;

        _viewModel.SelectSupplierFromSearch(supplier);

        SupplierSearchPopup.IsOpen = false;
        lb.SelectedItem = null;
        SupplierSearchBox.Text = "";

        // Update visual states
        Dispatcher.BeginInvoke(UpdateSupplierCardsStatus);
    }

    // ══════════════════════════════════════════════
    //  إجراءات الصفوف (مُحتفظ بها من الكود القديم)
    // ══════════════════════════════════════════════

    private void RemoveRow_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not QuotationItemRow row) return;

        _viewModel.RemoveItemRow(row);
        Dispatcher.BeginInvoke(UpdateMaterialCardsStatus);
    }

    private void RemoveSupplierChip_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not Supplier supplier) return;

        _viewModel.RemoveSupplier(supplier);
        Dispatcher.BeginInvoke(UpdateSupplierCardsStatus);
    }

    // ══════════════════════════════════════════════
    //  لوحة التسعير — أحداث الخلايا
    // ══════════════════════════════════════════════

    /// <summary>
    /// معالج تغيير نوع القطعة في لوحة التسعير.
    /// </summary>
    private void PartTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox combo) return;
        if (combo.DataContext is not Supplier supplier) return;
        if (_viewModel.SelectedItem == null) return;

        var selectedType = combo.SelectedItem as string;
        if (_viewModel.SelectedItem.SupplierCells.TryGetValue(supplier.Id, out var cell))
        {
            cell.Type = selectedType;
            _viewModel.OnPricingChanged();
        }
    }

    /// <summary>
    /// معالج تغيير سعر الوحدة في لوحة التسعير.
    /// </summary>
    private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox tb) return;
        if (tb.DataContext is not Supplier supplier) return;
        if (_viewModel.SelectedItem == null) return;

        if (_viewModel.SelectedItem.SupplierCells.TryGetValue(supplier.Id, out var cell))
        {
            cell.Price = decimal.TryParse(tb.Text, out var price) ? price : null;
            _viewModel.OnPricingChanged();

            // Update material cards and best price badges
            Dispatcher.BeginInvoke(() =>
            {
                UpdateMaterialCardsStatus();
                UpdatePricingBestPriceBadges();
            });
        }
    }

    /// <summary>
    /// معالج تغيير الملاحظة في لوحة التسعير.
    /// </summary>
    private void NoteTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Note is preserved for future use (stored in memory)
        // Currently does not map to a model field — reserved for enhancement
    }

    /// <summary>
    /// يُحدّث شارات "أفضل سعر" في لوحة التسعير لكل مورد.
    /// </summary>
    private void UpdatePricingBestPriceBadges()
    {
        if (_viewModel.SelectedItem == null) return;
        if (PricingItemsControl == null) return;

        foreach (var item in PricingItemsControl.Items)
        {
            if (item is not Supplier supplier) continue;

            var container = PricingItemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
            if (container == null) continue;

            var badge = FindVisualChild<Border>(container, "PricingBestPriceBadge");
            if (badge == null) continue;

            bool isBest = _viewModel.IsSupplierBestPrice(_viewModel.SelectedItem, supplier.Id);
            badge.Visibility = isBest ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    /// <summary>
    /// يُحدّث مؤشر اتصال قاعدة البيانات في رأس النموذج.
    /// </summary>
    private void UpdateFormConnectionIndicator(bool isConnected)
    {
        if (isConnected)
        {
            FormConnectionDot.Fill = FindResource("SuccessBrush") as Brush ?? Brushes.Green;
            FormConnectionText.Text = "متصلة";
        }
        else
        {
            FormConnectionDot.Fill = FindResource("DangerBrush") as Brush ?? Brushes.Red;
            FormConnectionText.Text = "غير متصلة";
        }
    }

    // ══════════════════════════════════════════════
    //  أداة مساعدة: البحث في الشجرة البصرية
    // ══════════════════════════════════════════════

    /// <summary>
    /// يبحث في الشجرة البصرية عن أول عنصر من النوع T بالاسم المحدد.
    /// </summary>
    private static T? FindVisualChild<T>(DependencyObject parent, string? childName) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T typedChild)
            {
                bool nameMatches = string.IsNullOrEmpty(childName);

                if (!nameMatches && child is FrameworkElement fe)
                    nameMatches = fe.Name == childName;

                if (nameMatches)
                    return typedChild;
            }

            var result = FindVisualChild<T>(child, childName);
            if (result != null)
                return result;
        }

        return null;
    }
}
