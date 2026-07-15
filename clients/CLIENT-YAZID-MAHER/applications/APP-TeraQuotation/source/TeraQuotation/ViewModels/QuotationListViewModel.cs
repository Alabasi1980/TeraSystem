using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TeraQuotation.Models;
using TeraQuotation.Services;

namespace TeraQuotation.ViewModels;

public partial class QuotationListViewModel : ObservableObject
{
    private readonly IQuotationService _quotationService;
    private readonly INavigationService _navigationService;
    private readonly NavigationParameter _navigationParameter;

    [ObservableProperty]
    private ObservableCollection<Quotation> _quotations = new();

    [ObservableProperty]
    private Quotation? _selectedQuotation;

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private string? _selectedStatusFilter;

    [ObservableProperty]
    private ObservableCollection<string> _statusFilters = new()
    {
        "الكل",
        "Draft",
        "UpdatedWithPrices",
        "Printed",
        "PDFExported",
        "SentViaOutlook"
    };

    public QuotationListViewModel(
        IQuotationService quotationService,
        INavigationService navigationService,
        NavigationParameter navigationParameter)
    {
        _quotationService = quotationService;
        _navigationService = navigationService;
        _navigationParameter = navigationParameter;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        await SearchAsync();
    }

    [RelayCommand]
    async Task SearchAsync()
    {
        try
        {
            // "الكل" or empty means no filter
            var status = (SelectedStatusFilter == "الكل" || string.IsNullOrEmpty(SelectedStatusFilter))
                ? null
                : SelectedStatusFilter;

            var results = await _quotationService.SearchQuotationsAsync(SearchText, status);
            Quotations = new ObservableCollection<Quotation>(results);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"خطأ في تحميل قائمة عروض الأسعار: {ex.Message}",
                "خطأ",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    void OpenQuotation(Quotation? q)
    {
        if (q == null) return;

        // Pass the quotation ID to the detail view via navigation parameter
        _navigationParameter.Value = q.Id;
        _navigationService.NavigateTo<Views.QuotationDetailView>();
    }

    [RelayCommand]
    void NewQuotation()
    {
        _navigationService.NavigateTo<Views.QuotationFormView>();
    }

    [RelayCommand]
    void Home()
    {
        _navigationService.NavigateTo<Views.SettingsView>();
    }
}
