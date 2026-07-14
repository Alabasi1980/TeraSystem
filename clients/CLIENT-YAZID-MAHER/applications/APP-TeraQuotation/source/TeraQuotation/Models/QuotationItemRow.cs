using CommunityToolkit.Mvvm.ComponentModel;

namespace TeraQuotation.Models;

/// <summary>
/// يمثل صفاً واحداً في جدول عرض السعر.
/// كل صف = قطعة واحدة مع أسعارها عند الموردين المختلفين.
/// الأعمدة الديناميكية (الموردين) تُدار عبر <see cref="SupplierCells"/>.
/// </summary>
public partial class QuotationItemRow : ObservableObject
{
    [ObservableProperty]
    private Item? _item;

    [ObservableProperty]
    private decimal _quantity = 1;

    [ObservableProperty]
    private string _unit = "قطعة";

    /// <summary>
    /// بيانات الموردين لكل صف: supplierId → SupplierCell (نوع + سعر).
    /// تُملأ تلقائياً عند إضافة مورد أو إضافة صف.
    /// </summary>
    public Dictionary<int, SupplierCell> SupplierCells { get; } = new();

    // ── خصائص محسوبة للعرض في DataGrid ──

    public string DisplayName => Item?.Name ?? "\u2014";
    public int ItemId => Item?.Id ?? 0;

    // ── عند تغيير القطعة: نسخ الوحدة تلقائياً ──

    partial void OnItemChanged(Item? value)
    {
        if (value != null)
            Unit = value.Unit;

        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(ItemId));
    }

    // ── إدارة خلايا الموردين ──

    /// <summary>
    /// يضمن وجود خلية فارغة لمورد معين. يُستدعى عند إضافة مورد أو إضافة صف.
    /// </summary>
    public void EnsureSupplierCell(int supplierId)
    {
        if (!SupplierCells.ContainsKey(supplierId))
            SupplierCells[supplierId] = new SupplierCell();
    }

    /// <summary>
    /// يحذف خلية مورد معين. يُستدعى عند حذف مورد من العرض.
    /// </summary>
    public void RemoveSupplierCell(int supplierId)
    {
        SupplierCells.Remove(supplierId);
    }
}

/// <summary>
/// يمثل خلية واحدة لبيانات مورد معين داخل صف (نوع + سعر).
/// </summary>
public class SupplierCell
{
    public string? Type { get; set; }
    public decimal? Price { get; set; }
}
