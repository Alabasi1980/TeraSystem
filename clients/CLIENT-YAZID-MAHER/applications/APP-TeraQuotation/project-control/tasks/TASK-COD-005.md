# TASK-COD-005: Quotation Form (S3) — الجوهر

> **Batch 3 — Quotation Core | اليوم 3**
> **الحالة:** ✅ Approved (جاهز للتفويض)

---

## 1. الوصف

تنفيذ شاشة إنشاء عرض السعر (S3) — جوهر التطبيق. تشمل تسلسل تلقائي، جدول القطع مع 3 موردين، إضافة من الكتالوج، Quick-Add، حفظ.

## 2. المخرجات

- [ ] `Views/QuotationFormView.xaml` + `.cs`
- [ ] `ViewModels/QuotationFormViewModel.cs`
- [ ] تحديث `MainWindow.xaml` + NavigationService للتنقل بين Settings ↔ QuotationForm
- [ ] التنفيذ الحقيقي لـ `QuotationService` (بدلاً من stub)
- [ ] تحديث DI

## 3. Allowed Write Targets

```
{DIR}\Views\QuotationFormView.xaml
{DIR}\Views\QuotationFormView.xaml.cs
{DIR}\ViewModels\QuotationFormViewModel.cs
{DIR}\Services\QuotationService.cs
{DIR}\Services\IQuotationService.cs (تحديث)
{DIR}\App.xaml.cs (تحديث)
{DIR}\MainWindow.xaml (تحديث)
```
حيث `{DIR}` = `D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation`

---

## 4. Acceptance Criteria

- ✅ `dotnet build` — 0 Errors
- ✅ تسلسل تلقائي: Q-001, Q-002, Q-003...
- ✅ إضافة قطعة من كتالوج القطع (ComboBox)
- ✅ Quick-Add قطعة جديدة (تظهر نافذة صغيرة أو Inline)
- ✅ جدول بكل قطعة + 3 موردين × (نوع/سعر)
- ✅ حفظ كمسودة (Draft)
- ✅ أزرار: حفظ، إلغاء
- ✅ RTL بالكامل

---

## 5. واجهة QuotationForm

```
┌─────────────────────────────────────────────────────────────┐
│  إنشاء عرض سعر جديد                          [الصفحة الرئيسية] │
│                                                              │
│  رقم العرض: Q-004    التاريخ: 13/07/2026                     │
│  الوصف: [________________________________________]          │
│                                                              │
│  ┌─────────┬────────┬────────┬──────┬────────┬──────┬──────┐ │
│  │ القطعة  │ م1-نوع │ م1-سعر│م2-نوع│م2-سعر │م3-نوع│م3-سعر│ │
│  ├─────────┼────────┼────────┼──────┼────────┼──────┼──────┤ │
│  │ [زمام]  │ أصلي   │ 15.00  │ تجاري│ 12.00  │ تجاري│ 10.50│ │
│  │ [فلتر]  │ أصلي   │ 25.00  │ —    │ —      │ —    │ —    │ │
│  └─────────┴────────┴────────┴──────┴────────┴──────┴──────┘ │
│                                                              │
│  [إضافة قطعة]  [Quick-Add جديدة]                             │
│                                                              │
│  [💾 حفظ] [🖨️ طباعة بدون أسعار] [🖨️ طباعة نهائية] [📄 PDF]  │
└─────────────────────────────────────────────────────────────┘
```

---

## 6. Task Log

| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — 0 Errors |
| 2026-07-13 | 🟢 Post-Execution Review: PASS |
