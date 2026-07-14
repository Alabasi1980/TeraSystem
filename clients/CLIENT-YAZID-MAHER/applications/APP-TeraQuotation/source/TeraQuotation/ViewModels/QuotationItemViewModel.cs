using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TeraQuotation.Models;

namespace TeraQuotation.ViewModels;

/// <summary>
/// نموذج عرض لعنصر في عرض السعر — مُستخدَم في شاشة التفاصيل (QuotationDetailViewModel).
/// يحتفظ ببيانات الموردين الثلاثة الأصلية (Fixed 1/2/3) للاسترجاع من قاعدة البيانات.
/// </summary>
public partial class QuotationItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private string? _itemName;

    [ObservableProperty]
    private decimal _quantity = 1;

    [ObservableProperty]
    private string _unit = "قطعة";

    [ObservableProperty]
    private string? _supplier1Type;

    [ObservableProperty]
    private decimal? _supplier1Price;

    [ObservableProperty]
    private string? _supplier2Type;

    [ObservableProperty]
    private decimal? _supplier2Price;

    [ObservableProperty]
    private string? _supplier3Type;

    [ObservableProperty]
    private decimal? _supplier3Price;

    [ObservableProperty]
    private ObservableCollection<Supplier>? _suppliers;
}
