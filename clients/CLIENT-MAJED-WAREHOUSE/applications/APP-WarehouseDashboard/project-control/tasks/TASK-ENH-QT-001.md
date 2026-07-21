# TASK-ENH-QT-001 — Backend: Dual Source + Schema API

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** engineering-agent-dotnet
> **أولوية:** High (Foundation for QT Enhancement)

---

## 1. الوصف

تطوير الـ Backend لـ QueryTester ليدعم:
1. **مصدرين**: SQL Server + Oracle (اختيار المستخدم)
2. **Schema Browser API**: قائمة جداول + قائمة أعمدة لكل مصدر
3. **حد أقصى للنتائج**: 1000 صف (MaxRows)

---

## 2. التغييرات المطلوبة

### A. ملف: `Pages/admin-secure-panel/QueryTester/Index.cshtml.cs`

#### A.1 إضافة خاصية `Source` إلى `QueryRunRequest`
```csharp
public class QueryRunRequest
{
    public string? Sql { get; set; }
    public string? Source { get; set; }  // "SqlServer" أو "Oracle"
}
```

#### A.2 تعديل `OnPostRunAsync`
- قراءة `request.Source` (القيمة الافتراضية: "SqlServer")
- إذا `SqlServer` → استخدم `SqlConnection` + `SqlReadonlyGuard` (كما هو حالياً)
- إذا `Oracle` → استخدم `OracleConnection` + `OracleReadonlyGuard` (مثل OracleQueryLab)
- إضافة `MaxRows = 1000` — تقليم النتائج بعد 1000 صف
- في `Reader.ReadAsync`: أضف counter، إذا تجاوز 1000 → break
- إرجاع `truncated: true` في response إذا تم اقتطاع النتائج

#### A.3 إضافة Schema Handler: `OnGetTablesAsync`
```csharp
public async Task<IActionResult> OnGetTablesAsync([FromQuery] string source)
```
- `source = "SqlServer"` → `SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME`
- `source = "Oracle"` → `SELECT OWNER, TABLE_NAME FROM ALL_TABLES ORDER BY OWNER, TABLE_NAME`
- إرجاع `[{ schema, tableName }]`

#### A.4 إضافة Schema Handler: `OnGetColumnsAsync`
```csharp
public async Task<IActionResult> OnGetColumnsAsync([FromQuery] string source, [FromQuery] string table)
```
- `source = "SqlServer"` → `SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table ORDER BY ORDINAL_POSITION`
- `source = "Oracle"` → `SELECT COLUMN_NAME, DATA_TYPE, NULLABLE FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = :table ORDER BY COLUMN_ID`
- إرجاع `[{ name, dataType, nullable }]`

#### A.5 إضافة حد للنتائج
- `private const int MaxRows = 1000;`
- أثناء `ReadAsync`: count++، if (count > MaxRows) break;
- إرجاع `truncated` boolean في الـ response

### B. ملفات مرجعية موجودة

| المرجع | المسار |
|--------|--------|
| OracleQueryLab Backend | `Pages/admin-secure-panel/OracleQueryLab/Index.cshtml.cs` — استخدمه كنموذج لاتصال Oracle |
| SqlReadonlyGuard | `Infrastructure/SqlReadonlyGuard.cs` — موجود مسبقاً |
| OracleReadonlyGuard | موجود في نفس مساحة الاسم (Infrastructure/) |
| ConnectionStringHelper | `Infrastructure/ConnectionStringHelper.cs` — موجود مع `ResolveSql()` + `ResolveOracle()` |

---

## 3. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml.cs`

**ممنوع تعديل:** أي ملف آخر.

---

## 4. Acceptance Criteria

1. ✅ `QueryRunRequest` فيه `Source` property (افتراضي "SqlServer")
2. ✅ `OnPostRunAsync` يقبل source parameter وينفذ على المصدر الصحيح
3. ✅ Oracle execution يستخدم `OracleConnection` + `OracleReadonlyGuard`
4. ✅ SQL Server execution يستخدم `SqlConnection` + `SqlReadonlyGuard`
5. ✅ حد أقصى 1000 صف مع `truncated` flag في response
6. ✅ `OnGetTablesAsync` يُعيد قائمة جداول (schema + tableName) للمصدر المحدد
7. ✅ `OnGetColumnsAsync` يُعيد قائمة أعمدة (name + dataType + nullable) لجدول معين
8. ✅ `dotnet build` PASS — 0 errors, 0 warnings

---

## 5. Fresh File Read Rule (إلزامي)

**قبل تعديل الملف، اقرأ النسخة الحالية من القرص أولاً.** احتفظ بالتغييرات غير المرتبطة.

---

## 6. After Completion

أعد:
1. ملخص التغييرات التي أجريتها
2. تأكيد `dotnet build` PASS (أظهر الـ output الفعلي)
3. Auditor Decision Recommendation: NOT_REQUIRED
