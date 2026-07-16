# TASK-KPI-004 — DashboardService Multi-Query Execution for Advanced KPI

| البند | القيمة |
|---|---|
| **المعرف** | TASK-KPI-004 |
| **المجموعة** | KPI |
| **النوع** | Backend — Service Layer |
| **الوكيل** | engineering-agent |
| **الأولوية** | Critical |
| **الحالة** | DONE |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

بطاقة KPI المتقدمة تحتاج **عدة استعلامات** من نفس الجدول بفلاتر مختلفة:
- القيمة الرئيسية (المستعلام الأساسي)
- نسبة التغير عن الفترة السابقة
- بيانات Sparkline (قيم شهرية)
- الإجمالي العام (بدون فلتر زمني)

الخدمة الحالية (`DashboardService`) تنفذ **استعلام واحد فقط** لكل بطاقة.

---

## 2. الهدف

توسيع `DashboardService` لتنفيذ الاستعلامات المتعددة لبطاقات KPI المتقدمة مع الحفاظ على التوافق مع البطاقات العادية.

---

## 3. التصميم المقترح

### 3.1 CardDataResult — خصائص جديدة

إضافة خصائص KPI إلى `CardDataResult.cs`:

```csharp
// === Advanced KPI Properties ===

/// <summary>Main KPI numeric value (extracted from ValueColumn).</summary>
public object? KpiMainValue { get; set; }

/// <summary>Change percentage from previous period (e.g., 12.5 for +12.5%).</summary>
public decimal? KpiChangePercent { get; set; }

/// <summary>Change direction: "up", "down", or "flat".</summary>
public string KpiChangeDirection { get; set; } = "flat";

/// <summary>Sparkline data points (monthly values for trend chart).</summary>
public List<Dictionary<string, object?>>? KpiSparklineData { get; set; }

/// <summary>Grand total value (all-time, no date filter).</summary>
public object? KpiGrandTotal { get; set; }

/// <summary>KPI display mode from card config: "simple", "withChange", "composite".</summary>
public string KpiMode { get; set; } = "simple";
```

### 3.2 DashboardService — تعديل GetCardDataAsync

المنطق الجديد:

```
1. جلب البطاقة من قاعدة البيانات
2. تنفيذ الاستعلام الأساسي (كما هو حالياً) → KpiMainValue
3. إذا كان KpiMode == "withChange" أو "composite":
   a. بناء استعلام التغير → تنفيذ → KpiChangePercent
4. إذا كان KpiMode == "composite":
   b. بناء استعلام Sparkline → تنفيذ → KpiSparklineData
   c. بناء استعلام Grand Total → تنفيذ → KpiGrandTotal
5. إرجاع النتيجة الكاملة
```

### 3.3 بناء الاستعلامات الإضافية

**مهم جداً:** الاستعلامات الإضافية تُبنى من `SqlQuery` الأساسي بتعديل WHERE clause:

#### Change Query (previous period)
```sql
-- المبدأ: نأخذ نفس الاستعلام الأساسي، نضيف فلتر تاريخ للفترة السابقة
-- إذا كان SqlQuery يحتوي SELECT ... FROM ... WHERE ... → نضيف AND DateColumn BETWEEN ...
-- إذا لم يكن هناك WHERE → نضيف WHERE DateColumn BETWEEN ...
```

**الخيارات حسب `ChangeSource`:**
- `previousPeriod`: نفس الفترة السابقة (مثل: إذا كان النطاق 2026-Q2، ن查 2026-Q1)
- `previousMonth`: الشهر السابق
- `previousYear`: نفس الشهر من السنة السابقة

#### Sparkline Query (monthly aggregation)
```sql
-- نأخذ نفس الجدول، نضيف: SELECT DateColumn, SUM(ValueColumn) ... GROUP BY MONTH(DateColumn)
-- مع فلتر آخر N أشهر
```

#### Grand Total Query
```sql
-- نفس الاستعلام الأساسي بدون فلتر التاريخ
```

### 3.4 الطريقة المقترحة للتنفيذ

**الأمثل:** إنشاء method مساعد `BuildKpiQueries(DashboardCard card)` يُرجع:
```csharp
public class KpiQueries
{
    public string? ChangeSql { get; set; }
    public string? SparklineSql { get; set; }
    public string? GrandTotalSql { get; set; }
}
```

ثم في `GetCardDataAsync` بعد التنفيذ الأساسي:
```csharp
if (card.ChartType.Equals("KPI", StringComparison.OrdinalIgnoreCase) 
    && !string.IsNullOrEmpty(card.KpiMode) 
    && card.KpiMode != "simple")
{
    var kpiQueries = BuildKpiQueries(card);
    // Execute each query sequentially
    if (kpiQueries.ChangeSql != null) { ... }
    if (kpiQueries.SparklineSql != null) { ... }
    if (kpiQueries.GrandTotalSql != null) { ... }
}
```

---

## 4. النطاق

### المطلوب
- [x] **A. تعديل `CardDataResult.cs`** — إضافة 6 خصائص KPI
- [x] **B. تعديل `DashboardService.cs`** — إضافة منطق KPI المتعدد في `GetCardDataAsync`
- [x] **C. إنشاء `KpiQueryBuilder` class** — بناء الاستعلامات الإضافية من `SqlQuery` الأساسي
- [x] **D. التأكد من عمل `dotnet build -c Release` بدون أخطاء**

### غير المطلوب
- لا تغيير في `Index.cshtml.cs`
- لا تغيير في `Builder.cshtml`
- لا تغيير في `card-builder.js`
- لا تغيير في صفحة Dashboard (`Index.cshtml`)

---

## 5. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\CardDataResult.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-KPI-004.md
```

---

## 6. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | `CardDataResult.cs` يحتوي 6 خصائص KPI الجديدة | ✅ |
| AC-2 | `DashboardService.GetCardDataAsync` ينفذ استعلامات متعددة لبطاقات KPI | ✅ |
| AC-3 | `KpiQueryBuilder` يبني الاستعلامات الصحيحة من `SqlQuery` الأساسي | ✅ |
| AC-4 | البطاقات非-KPI (Bar, Line, Pie, Table, Gauge) تعمل كما كانت | ✅ |
| AC-5 | `dotnet build -c Release` ينجح بدون أخطاء | ✅ |
| AC-6 | لا توجد أسرار في الملفات المعدلة | ✅ |

---

## 7. Pre-Execution Gate Result

**Result:** PASS

- Active Technology Profile: `dotnet-razorpages-adonet`
- Smallest safe executable unit: Yes — تعديل Service Layer فقط
- Single goal: Yes — توسيع DashboardService لدعم KPI المتعدد
- UI task: No
- Security sensitivity: Medium (يكتب SQL queries — لكنها admin-controlled)
- Database impact: None (قراءة فقط)
- Secrets handling: N/A

---

## 8. Engineering Handback

**Status:** DONE

### Files Changed

| # | File | Action | Lines |
|---|------|--------|-------|
| 1 | `src/WarehouseDashboard.Web/Pages/CardDataResult.cs` | Modified — added 6 Advanced KPI properties | 40 → 60 |
| 2 | `src/WarehouseDashboard.Web/Pages/KpiQueryBuilder.cs` | **Created** — static helper class + `KpiQueries` DTO | 150 lines |
| 3 | `src/WarehouseDashboard.Web/Pages/DashboardService.cs` | Modified — added `ExecuteScalarQueryAsync`, `ExecuteQueryAsync`, KPI multi-query block in `GetCardDataAsync` | 232 → 371 |

### Build Result

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:02.10
```

### What Works
- **CardDataResult** now carries: `KpiMainValue`, `KpiChangePercent`, `KpiChangeDirection`, `KpiSparklineData`, `KpiGrandTotal`, `KpiMode`
- **KpiQueryBuilder** builds Change/Sparkline/GrandTotal SQL from the card's base `SqlQuery` + KPI config fields
- **DashboardService.GetCardDataAsync** now:
  - For `ChartType != "KPI"` or `KpiMode == "simple"` → behavior unchanged (backward compatible)
  - For `ChartType == "KPI"` with `KpiMode == "withChange"` → executes change query, calculates % and direction
  - For `ChartType == "KPI"` with `KpiMode == "composite"` → executes change + sparkline + grand total queries
  - Each additional query is wrapped in try/catch — failure of one doesn't break the card
- **SQL injection protection**: all column names go through `SanitizeIdentifier()` (bracket-wrapping + character stripping)

### Issues / Gaps
- None observed. All 4 tasks completed successfully.
- `GetPreviewAsync` is **not** affected — no advanced KPI logic there (by design; preview is for card builder, not dashboard rendering).

### Secrets Check
- ✅ No secrets written to any file
- ✅ No hardcoded credentials, connection strings, or tokens
- ✅ All connection strings resolved via `ConnectionStringHelper.Resolve()` at runtime

---

## 9. Tera Post-Execution Review

| البند | القيمة |
|---|---|
| **Result** | ✅ **PASS — ACCEPTED** |
| **CardDataResult.cs** | ✅ 6 خصائص KPI جديدة (KpiMainValue, KpiChangePercent, KpiChangeDirection, KpiSparklineData, KpiGrandTotal, KpiMode) |
| **KpiQueryBuilder.cs** | ✅ فئة جديدة — تبني Change/Sparkline/GrandTotal SQL من `SqlQuery` الأساسي مع SanitizeIdentifier للحماية من SQL Injection |
| **DashboardService.cs** | ✅ منطق KPI المتعدد — ExecuteScalarQueryAsync + ExecuteQueryAsync + كتلة KPI في GetCardDataAsync |
| **Backward Compatibility** | ✅ بطاقات非-KPI تعمل كما كانت |
| **Error Isolation** | ✅ كل استعلام KPI في try/catch مستقل |
| **Build** | ✅ 0 errors, 0 warnings |
| **Secrets** | ✅ No secrets |

**Verdict: ACCEPTED** — All 6 acceptance criteria met.
