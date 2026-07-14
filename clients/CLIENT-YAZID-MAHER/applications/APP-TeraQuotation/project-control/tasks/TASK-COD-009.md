# TASK-COD-009: Outlook Integration + Final Polish

> **Batch 4 — Reports & Polish | اليوم 4-5**
> **الحالة:** ✅ Approved

---

## 1. الوصف
ربط Outlook Desktop (MAPI) لإرسال عروض الأسعار بالبريد الإلكتروني + لمسات نهائية: عنوان النافذة، أيقونة، معالجة الأخطاء، تحسينات UX.

## 2. المخرجات

### Outlook
- [ ] `Services/IOutlookService.cs` — واجهة إرسال البريد
- [ ] `Services/OutlookService.cs` — تنفيذ عبر Outlook Interop (MAPI)
- [ ] ربط زر "إرسال عبر Outlook" في QuotationDetailView

### Final Polish
- [ ] عنوان النافذة: **TeraQuotation - نظام إدارة عروض الأسعار**
- [ ] أيقونة بسيطة (يمكن أن تكون رمزية عبر XAML Path)
- [ ] `dotnet build` — 0 Errors (مع تنظيف أي تحذيرات مشروع)
- [ ] تحسينات: Focus/Selection في DataGrid، رسائل تأكيد للحذف، كشف اغلاق التطبيق دون حفظ

## 3. Allowed Write Targets
```
{DIR}\Services\OutlookService.cs
{DIR}\ViewModels\QuotationDetailViewModel.cs
{DIR}\Views\MainWindow.xaml
```
`{DIR}` = المسار الكامل

## 4. Acceptance
- ✅ dotnet build — 0 Errors
- ✅ زر "إرسال عبر Outlook" → ينشئ بريد جديد مع PDF مرفق
- ✅ عنوان النافذة = TeraQuotation - نظام إدارة عروض الأسعار
- ✅ لا أخطاء وقت التشغيل في السيناريوهات الأساسية

## 5. Task Log
| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — Outlook Interop + PDF مرفق + أيقونة + عنوان + تحسينات |
| 2026-07-13 | 🟢 PASS |
