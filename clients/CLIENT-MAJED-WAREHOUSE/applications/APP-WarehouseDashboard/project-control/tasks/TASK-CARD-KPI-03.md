# TASK-CARD-KPI-03 — Fix Sparkline Date Range + Add "Last Month" Filter

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-KPI-03 |
| **المجموعة** | CARD-DESIGN-EXECUTION / Phase C |
| **النوع** | Backend (KpiQueryBuilder + API) + Frontend (filter button) |
| **الأولوية** | High |
| **الحالة** | ✅ Accepted — Auditor PASS |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | Majed visual feedback — sparkline issues with "هذا الشهر" filter |

---

## 1. الهدف

إصلاح مشكلتين في Sparkline + إضافة فلتر جديد:

1. **Sparkline لا يظهر مع فلتر "هذا الشهر"** — السبب: الاستعلام يأخذ `dateRange.From` مباشرةً بدلاً من الرجوع 6 أشهر
2. **Sparkline يحضر شهر واحد فقط لا 6** — نفس السبب
3. **إضافة فلتر "الشهر الماضي"** — لم يكن موجوداً في قائمة الفلاتر

---

## 2. تحليل المشكلة

### 2.1 السبب الجذرية — KpiQueryBuilder.BuildSparklineQuery

```csharp
// الكود الحالي (الخاطئ):
var startDate = dateRange is not null
    ? dateRange.From  // ← يستخدم تاريخ بداية الفلتر مباشرة
    : DateTime.UtcNow.AddMonths(-6);

return $"SELECT ... WHERE {dateCol} >= '{startDate:yyyy-MM-dd}' ...";
```

**عند فلتر "هذا الشهر":**
- `dateRange.From` = 2026-07-01 (أول يوم في الشهر)
- الاستعلام: `WHERE [date] >= '2026-07-01'`
- النتيجة: شهر واحد فقط = نقطة واحدة = "لا توجد بيانات اتجاه كافية"

**عند فلتر "آخر 30 يوم":**
- `dateRange.From` = 2026-06-19 (قبل 30 يوم)
- الاستعلام: `WHERE [date] >= '2026-06-19'`
- النتيجة: شهرين (يونيو + يوليو) = نقطتان = Sparkline يعمل

### 2.2 الحل المطلوب

Sparkline يجب أن يعرض دائماً **6 أشهر** (أو `SparklineMonths`) بغض النظر عن الفلتر الحالي. الفلتر يحدد القيمة الرئيسية، لكن الانحراف يعرض الاتجاه التاريخي.

---

## 3. النطاق

### In Scope

**A. Backend — KpiQueryBuilder.cs (الإصلاح الجذري):**
- `BuildSparklineQuery`: عند وجود `dateRange`، الرجوع `SparklineMonths` (افتراضي 6) شهراً من `dateRange.From` بدلاً من استخدام `dateRange.From` مباشرة
- المثال: إذا `dateRange.From = 2026-07-01` و `SparklineMonths = 6` → `startDate = 2026-01-01`

**B. Backend — Card.cshtml.cs (إضافة preset جديد):**
- إضافة `lastMonth` preset في `ResolvePresetDates`
- `lastMonth` = أول يوم في الشهر الماضي ← آخر يوم في الشهر الماضي

**C. Frontend — Index.cshtml (إضافة زر الفلتر):**
- إضافة زر `الشهر الماضي` بعد زر `هذا الشهر`
- `data-preset="lastMonth"`
- إضافة case في `getPresetDates` function

**D. Frontend — Index.cshtml (الـ indicator):**
- تحديث الـ indicator logic ليتضمن `lastMonth`

### Out of Scope

1. لا نغير Card Builder
2. لا نغير قاعدة البيانات
3. لا نغير المكتبات
4. لا نغير الكروت غير KPI

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\Dashboard\Card.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\Dashboard\Card.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

---

## 6. الملفات المتأثرة (للمراجعة فقط — لا تعدل)

- `DashboardService.cs` — لا يتغير (يمرر `effectiveDateRange` كما هو)
- `Builder.cshtml` — لا يتغير

---

## 7. Acceptance Criteria

1. **Sparkline مع "هذا الشهر"**: يعرض 6 أشهر سابقة من البيانات (وليس شهر واحد فقط)
2. **Sparkline مع "آخر 30 يوم"**: يعرض 6 أشهر سابقة (لا تغيير عن الحالي)
3. **Sparkline مع "اليوم"**: يعرض 6 أشهر سابقة
4. **Sparkline مع "أمس"**: يعرض 6 أشهر سابقة
5. **Sparkline مع "مخصص"**: يعرض 6 أشهر قبل تاريخ البداية المخصص
6. **فلتر "الشهر الماضي"**: زر جديد يظهر ويwork بشكل صحيح
7. **الـ indicator** يعرض "مقارنة بالشهر الماضي" عند اختيار الفلتر الجديد
8. **Dotnet build** ينجح
9. **لا توجد تأثيرات جانبية** على KPI value أو change percentage

---

## 8. التفاصيل التقنية للإصلاح

### 8.1 KpiQueryBuilder.cs — BuildSparklineQuery

```csharp
// الكود المُحسّن:
private static string BuildSparklineQuery(DashboardCard card, DashboardService.DateRange? dateRange = null)
{
    var baseQuery = NormalizeBaseQuery(card.SqlQuery);
    var valueCol = NumericExpression(card.ValueColumn);
    var dateCol = SanitizeIdentifier(card.DateColumn);

    var sparklineMonths = card.SparklineMonths > 0 ? card.SparklineMonths : 6;

    // Sparkline ALWAYS goes back SparklineMonths from today (or from range start),
    // never limited to the dashboard filter range.
    DateTime startDate;
    if (dateRange is not null)
    {
        // Go back SparklineMonths from the filter's start date
        startDate = dateRange.From.AddMonths(-sparklineMonths);
    }
    else
    {
        startDate = DateTime.UtcNow.AddMonths(-sparklineMonths);
    }

    return $"SELECT FORMAT({dateCol}, 'yyyy-MM') AS Month, SUM({valueCol}) AS MonthlyValue " +
           $"FROM ({baseQuery}) AS _base " +
           $"WHERE {dateCol} >= '{startDate:yyyy-MM-dd}' " +
           $"GROUP BY FORMAT({dateCol}, 'yyyy-MM') " +
           $"ORDER BY Month";
}
```

### 8.2 Card.cshtml.cs — ResolvePresetDates

```csharp
// إضافة case جديد:
"lastmonth" => new DashboardService.DateRange(
    new DateTime(today.Year, today.Month, 1).AddMonths(-1),
    new DateTime(today.Year, today.Month, 1).AddTicks(-1)
),
```

### 8.3 Index.cshtml — زر الفلتر

```html
<button class="wd-preset-btn" data-preset="lastMonth">الشهر الماضي</button>
```

### 8.4 Index.cshtml — getPresetDates

```javascript
case 'lastMonth':
    start = new Date(now.getFullYear(), now.getMonth() - 1, 1);
    end = new Date(now.getFullYear(), now.getMonth(), 0);
    break;
```

---

## 9. Security Sensitivity

- **Level:** Low
- **Reason:** SQL query date range logic and UI button addition only. No auth, no secrets, no schema changes.

---

## 10. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | 3 related fixes in one focused task |
| One objective only | PASS | Fix sparkline date range + add lastMonth filter |
| No deferrable work included | PASS | All 3 items are directly related |
| Allowed Write Targets are narrow | PASS | 3 files only |
| Acceptance criteria are testable | PASS | Visual + functional |

**Gate Status:** PASS

---

## 11. Delegation Notes

1. Before editing any file, read the current file from disk.
2. Preserve ALL unrelated code.
3. The sparkline fix is in `KpiQueryBuilder.cs` only — do not change `DashboardService.cs`.
4. The `lastMonth` preset must work in both backend (`Card.cshtml.cs`) and frontend (`Index.cshtml`).
5. Test with dotnet build.
6. The sparkline should show 6 months of data regardless of which filter is selected.

---

## 12. Handback Placeholder

---

> **Prepared by:** TeraAgent
> **Mode:** Plan Mode — No code written in this document
