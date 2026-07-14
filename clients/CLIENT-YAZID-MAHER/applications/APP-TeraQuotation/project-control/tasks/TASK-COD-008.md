# TASK-COD-008: Reports (S5) + Backup

> **Batch 4 — Reports & Polish | اليوم 4**
> **الحالة:** ✅ Approved

---

## 1. الوصف
الشاشة الخامسة S5: 4 تقارير + وظيفة Backup/Restore للبيانات.

## 2. المخرجات
- [ ] `Views/ReportsView.xaml` + `.cs` — الشاشة الخامسة
- [ ] `ViewModels/ReportsViewModel.cs`
- [ ] `Helpers/BackupHelper.cs` — تصدير/استيراد SQLite

## 3. Allowed Write Targets
```
{DIR}\Views\ReportsView.xaml
{DIR}\Views\ReportsView.xaml.cs
{DIR}\ViewModels\ReportsViewModel.cs
{DIR}\Helpers\BackupHelper.cs
{DIR}\Services\IReportService.cs
{DIR}\Services\ReportService.cs
{DIR}\App.xaml.cs
```
`{DIR}` = المسار الكامل

## 4. التقارير
1. **تقرير الموردين** — كل مورد مع عدد العروض المخصصة له
2. **تقرير القطع** — كل قطعة مع عدد مرات ظهورها في العروض
3. **تقرير العروض** — إحصائيات (عدد العروض، حسب الحالة، حسب التاريخ)
4. **Backup** — زر "تصدير نسخة احتياطية" (copy SQLite file) + زر "استيراد"

## 5. Acceptance
- ✅ dotnet build — 0 Errors
- ✅ 4 أزرار تقارير → تعرض تقارير في DataGrid داخل نفس الشاشة
- ✅ كل تقرير: جدول + عنوان + تاريخ
- ✅ Backup → نسخ ملف `TeraQuotation.db` إلى Desktop
- ✅ Restore → اختيار ملف `.db` من الجهاز واستبدال القاعدة

## 6. Task Log
| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — 12 ملف، 0 Errors، 4 تقارير + Backup + Restore |
| 2026-07-13 | 🟢 PASS |
