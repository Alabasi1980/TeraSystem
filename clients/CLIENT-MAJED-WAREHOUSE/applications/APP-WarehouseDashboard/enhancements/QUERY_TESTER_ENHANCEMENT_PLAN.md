# خطة تطوير QueryTester — Query Tester Enhancement Plan

> **تاريخ الإنشاء:** 2026-07-21
> **الحالة:** ✅ معتمدة للتنفيذ (المرحلة الأولى)
> **المشروع:** WarehouseDashboard — الماجد لادارة المستودعات

---

## 1. الرؤية

تحويل `QueryTester` من محرر SQL بسيط إلى **أداة تحليل بصرية متكاملة** تمكّن المستخدم من بناء استعلامات SQL بسهولة دون حفظ الصياغة، مع دعم Oracle و SQL Server معاً.

---

## 2. هيكلة التطوير — 4 مهام

### ✅ TASK-ENH-QT-001 — Backend: Dual Source + Schema API (engineering-agent-dotnet) — مكتمل
- إضافة `source` parameter (SqlServer / Oracle) إلى API
- دعم تنفيذ الاستعلامات على Oracle (باستخدام `Oracle.ManagedDataAccess.Core`)
- API جديد: `GET ?handler=Tables&source=SqlServer` — قائمة الجداول
- API جديد: `GET ?handler=Columns&source=SqlServer&table=Name` — قائمة الأعمدة
- نفس الشيء لـ Oracle (`USER_TABLES`, `USER_TAB_COLUMNS`)
- حد أقصى للنتائج (MaxRows = 1000)
- `dotnet build` PASS

### ✅ TASK-ENH-QT-002 — Frontend: Schema Browser + SELECT Builder (ui-designer) — مكتمل
- مصدر البيانات: اختيار Oracle / SQL Server
- لوحة جانبية (يمين أو يسار) تعرض شجرة: Tables → Columns (مع الأنواع)
- ضغطة على Table → تظهر أعمدته
- ضغطة على Column → تضاف للاستعلام
- SELECT Builder: اختار Table ← تظهر Checkboxes للأعمدة ← يتولّد `SELECT col1, col2 FROM Table`
- `dotnet build` PASS

### ✅ TASK-ENH-QT-003 — Frontend: WHERE Builder + Enhanced Results + History (ui-designer) — مكتمل
- WHERE Builder بصري: أضف شرط (Column + Operator + Value) مع دعم DatePicker
- Enhanced Results: ترتيب بالضغط على رأس العمود، تنسيق الأرقام والتواريخ، CSV Export
- Query History (localStorage): آخر 50 استعلام مع إعادة تشغيل بنقرة
- Keyboard shortcuts: Ctrl+Enter, Ctrl+Shift+C
- تنسيق متجاوب ونظيف
- `dotnet build` PASS

---

## 3. التبعيات (Dependencies)

```
TASK-ENH-QT-001 (Backend) ← TASK-ENH-QT-002 (يعتمد على الـ API)
                           ← TASK-ENH-QT-003 (يعتمد على API التنفيذ)
```

**ترتيب التنفيذ:** 001 → 002 → 003

---

## 4. القرارات التقنية

| القرار | الخيار | المبرر |
|--------|--------|--------|
| **قاعدة البيانات** | SqlClient (SQL Server) + Oracle.ManagedDataAccess (Oracle) | موجودان بالفعل في المشروع |
| **حد النتائج** | 1000 صف كحد أقصى | يمنع التحميل الزائد، مناسب للاختبار |
| **حماية التعديل** | SqlReadonlyGuard + OracleReadonlyGuard | أمان متعدد الطبقات |
| **Schema API** | INFORMATION_SCHEMA.TABLES (SQL), USER_TABLES (Oracle) | قياسي وآمن |
| **الترتيب (Sort)** | Client-side (JS sort) | لا يحتاج round-trip للخادم، مناسب لـ 1000 صف |
| **التاريخ** | localStorage | بسيط، لا يحتاج تخزين خادم |
| **إطار الواجهة** | Razor Pages + JS أصلي (بدون مكتبات خارجية) | متسق مع باقي المشروع |
