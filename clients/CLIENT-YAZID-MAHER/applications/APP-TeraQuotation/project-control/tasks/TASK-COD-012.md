# TASK-COD-012: Data Model + HandyControl + Settings Refactoring

> **المطلوب من الزبون — الدفعة الأولى**
> **الحالة:** ✅ Approved

---

## 1. الوصف
تحديث Data Model لإضافة الكمية والوحدة + تثبيت HandyControl + تحديث شاشة الإعدادات.

## 2. المخرجات

### Data Model
- [ ] `Models/Item.cs` — إضافة `string Unit` (الوحدة)
- [ ] `Models/QuotationItem.cs` — إضافة `decimal Quantity` + `string Unit`
- [ ] EF Core Migration جديدة

### HandyControl
- [ ] تثبيت `HandyControl` NuGet
- [ ] تحديث `App.xaml` لتحميل ثيمات HandyControl

### Settings UI
- [ ] تحديث Tab القطع — إضافة حقل "الوحدة" (قطعة/كيلو/متر/علبة/卷...)
- [ ] تحديث QuotationForm — إضافة حقل الكمية في الصفوف
- [ ] تحديث QuotationDetail — نفس الشيء

## 3. Allowed Write Targets
```
{DIR}\Models\Item.cs
{DIR}\Models\QuotationItem.cs
{DIR}\TeraQuotation.csproj
{DIR}\App.xaml
{DIR}\Data\AppDbContext.cs
{DIR}\ViewModels\SettingsViewModel.cs
{DIR}\Views\SettingsView.xaml
{DIR}\ViewModels\QuotationFormViewModel.cs
{DIR}\ViewModels\QuotationDetailViewModel.cs
{DIR}\Views\QuotationFormView.xaml
{DIR}\Views\QuotationDetailView.xaml
```

## 4. Acceptance
- ✅ dotnet build — 0 Errors
- ✅ Item يحتوي Unit
- ✅ QuotationItem يحتوي Quantity + Unit
- ✅ Settings → Tab القطع يعرض حقل الوحدة
- ✅ HandyControl يعمل في المشروع

## 5. Task Log
| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — HandyControl 3.6.0-rc3, Unit/Quantity, Settings UI, Migration |
| 2026-07-13 | 🟢 PASS |
