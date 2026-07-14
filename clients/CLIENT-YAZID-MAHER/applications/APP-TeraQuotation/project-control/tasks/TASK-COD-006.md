# TASK-COD-006: Quotation List (S4) + Detail View

> **Batch 3 — Quotation Core | اليوم 3**
> **الحالة:** ✅ Approved

---

## 1. الوصف

إنشاء شاشة عرض قائمة العروض (S4) + شاشة تفاصيل/تعديل عرض السعر.

## 2. المخرجات

- [ ] `Views/QuotationListView.xaml` + `.cs` — قائمة العروض
- [ ] `Views/QuotationDetailView.xaml` + `.cs` — تفاصيل عرض (لتعديله وطباعته)
- [ ] `ViewModels/QuotationListViewModel.cs`
- [ ] `ViewModels/QuotationDetailViewModel.cs`
- [ ] تحديث DI + Navigation

## 3. Allowed Write Targets

```
{DIR}\Views\QuotationListView.xaml
{DIR}\Views\QuotationListView.xaml.cs
{DIR}\Views\QuotationDetailView.xaml
{DIR}\Views\QuotationDetailView.xaml.cs
{DIR}\ViewModels\QuotationListViewModel.cs
{DIR}\ViewModels\QuotationDetailViewModel.cs
{DIR}\App.xaml.cs
{DIR}\ViewModels\QuotationFormViewModel.cs
```
حيث `{DIR}` = `D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation`

## 4. Acceptance

- ✅ `dotnet build` — 0 Errors
- ✅ عرض جميع العروض المحفوظة في DataGrid
- ✅ بحث (رقم عرض، وصف)
- ✅ تصفية حسب الحالة (Draft, UpdatedWithPrices, Printed...)
- ✅ فتح عرض → تعديله (إضافة/حذف قطع، تغيير أسعار)
- ✅ تغيير حالة العرض
- ✅ زر العودة للقائمة

## 5. Task Log

| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — 10 ملفات، 0 Errors |
| 2026-07-13 | 🟢 PASS |
