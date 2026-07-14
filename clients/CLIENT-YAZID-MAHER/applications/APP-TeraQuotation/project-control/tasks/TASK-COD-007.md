# TASK-COD-007: طباعة A4 + PDF (QuestPDF)

> **Batch 3 — Quotation Core | اليوم 3-4**
> **الحالة:** ✅ Approved

---

## 1. الوصف
تنفيذ طباعة عروض الأسعار بصيغتين: طباعة A4 مباشرة (FixedDocument) وتصدير PDF (QuestPDF). طباعة مرحلية: بدون أسعار (للموردين) ونهائية.

## 2. المخرجات
- [ ] `Services/IPdfService.cs` — تحديث
- [ ] `Services/PdfService.cs` — تنفيذ حقيقي (QuestPDF)
- [ ] `Helpers/PrintHelper.cs` — طباعة A4 (FixedDocument + PrintDialog)
- [ ] ربط أزرار الطباعة/PDF في QuotationDetailView

## 3. Allowed Write Targets
```
{DIR}\Services\PdfService.cs
{DIR}\Services\IPdfService.cs
{DIR}\Helpers\PrintHelper.cs
{DIR}\ViewModels\QuotationDetailViewModel.cs
{DIR}\App.xaml.cs
```
`{DIR}` = المسار الكامل لمجلد source/TeraQuotation

## 4. ملاحظات تقنية
- **QuestPDF** مثبّت بالفعل (2026.7.1)
- طباعة A4 عبر FixedDocument + PrintDialog في WPF
- طباعة بدون أسعار: إخفاء أعمدة الأسعار
- طباعة نهائية: كل البيانات مع الترويسة والتوقيع
- PDF يُحفظ في Desktop باسم `عرض_سعر_Q-001.pdf`
- الترويسة (شعار، اسم شركة، عنوان) من Settings

## 5. Acceptance
- ✅ dotnet build — 0 Errors
- ✅ زر "طباعة بدون أسعار" → يعرض PrintDialog
- ✅ زر "طباعة نهائية" → يعرض PrintDialog
- ✅ زر "PDF" → يُنشئ ملف PDF Desktop
- ✅ الترويسة + التوقيع يظهران في الطباعة
- ✅ الأعمدة: قطعة، مورد1-نوع/سعر، مورد2-نوع/سعر، مورد3-نوع/سعر

## 6. Task Log
| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — PdfService (QuestPDF), PrintHelper (FixedDocument), ربط الأزرار، 0 Errors |
| 2026-07-13 | 🟢 PASS |
