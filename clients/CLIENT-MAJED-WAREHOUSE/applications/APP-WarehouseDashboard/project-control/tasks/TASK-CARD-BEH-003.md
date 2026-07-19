# TASK-CARD-BEH-003 — DateFilterMode: Wire Dashboard Preset Through to SQL Queries

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-BEH-003 |
| **المجموعة** | CARD-DESIGN-EXECUTION |
| **النوع** | Backend / C# (API + Service + Query Builder) |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (Auditor PASS) |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `DASHBOARD_CARD_DESIGN_EXECUTION_PLAN.md` — Phase B / CARD-BEH-03 |
| **القرارات المعتمدة** | DateFilterMode يُطبَّق على **كل البطاقات** (وليس KPI فقط) — Majedconfirmed |

---

## 1. الهدف

ربط **فلتر التاريخ في الداشبورد** (شريط التواريخ) مع **الاستعلامات الفعلية** لكل بطاقة، بحيث تتبع البطاقات **وضع DateFilterMode** الخاص بها:

- `dashboard` (الافتراضي): تتبع فلتر التاريخ في الداشبورد (preset)
- `fixed`: تتبع `FixedStartDate` / `FixedEndDate` (مهمة تالية BEH-004)
- `relative`: تتبع `RelativeDays` (مهمة تالية BEH-004)

**هذه المهمة تغطي وضع `dashboard` فقط** — الأهم والأكثر استخداماً.

---

## 2. الوضع الحالي (المشكلة)

### المسار الحالي (معطل):

```
Frontend: wdLoadCard(id) → fetch('/api/dashboard/card/{id}?preset=today')
    ↓
API: Card.cshtml.cs → OnGetAsync(id) → _dashboardService.GetCardDataByIdAsync(id)
    ⚠️ preset parameter يُستقبل لكنه يُتجاهل تماماً
    ↓
DashboardService: GetCardDataByIdAsync(id) → GetCardDataAsync(card)
    ↓
BuildSql(card): يبني الاستعلام بدون أي فلتر تاريخ
    ↓
النتيجة: كل البطاقات تعرض جميع البيانات بدون فلتر زمني
```

### المسار المطلوب:

```
Frontend: wdLoadCard(id) → fetch('/api/dashboard/card/{id}?preset=today')
    ↓
API: Card.cshtml.cs → OnGetAsync(id, preset) → _dashboardService.GetCardDataByIdAsync(id, presetDates)
    ✅ preset يُحوَّل إلى نطاق تواريخ (from, to)
    ↓
DashboardService: GetCardDataByIdAsync(id, dateRange) → GetCardDataAsync(card, dateRange)
    ↓
BuildSql(card, dateRange): يضيف WHERE [DateColumn] >= @from AND [DateColumn] <= @to
    ✅ فلتر التاريخ مُطبَّق
    ↓
KpiQueryBuilder.Build(card, dateRange): يستخدم نطاق التاريخ لحساب Change/Sparkline
    ✅ KPI يتبع نفس الفلتر
```

---

## 3. النطاق — 4 ملفات

### 3.1 `Card.cshtml.cs` (API Endpoint)

```csharp
// Current:
public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
{
    var result = await _dashboardService.GetCardDataByIdAsync(id, cancellationToken);
    return new JsonResult(result);
}

// Target:
public async Task<IActionResult> OnGetAsync(int id, string? preset, CancellationToken cancellationToken)
{
    var dateRange = ResolvePresetDates(preset);
    var result = await _dashboardService.GetCardDataByIdAsync(id, dateRange, cancellationToken);
    return new JsonResult(result);
}
```

**Adding `ResolvePresetDates(string? preset)`** — converts preset string to (DateTime from, DateTime to):
- `"today"` → (today 00:00, today 23:59)
- `"yesterday"` → (yesterday 00:00, yesterday 23:59)
- `"7days"` → (today - 6 days, today)
- `"30days"` → (today - 29 days, today)
- `"month"` → (first day of current month, today)
- `"custom"` → null (no filter — custom dates handled separately)
- `null` / unknown → null (no filter)

Return type: `ValueTuple<DateTime, DateTime>?` (null = no filter)

### 3.2 `DashboardService.cs`

```csharp
// Add DateRange record:
public record DateRange(DateTime From, DateTime To);

// Update method signatures:
public async Task<CardDataResult> GetCardDataByIdAsync(int cardId, DateRange? dateRange = null, CancellationToken ct = default)
public async Task<CardDataResult> GetCardDataAsync(DashboardCard card, DateRange? dateRange = null, CancellationToken ct = default)

// Update BuildSql:
private static string BuildSql(DashboardCard card, DateRange? dateRange = null)
{
    // ... existing logic ...
    
    // After building baseSql, before returning:
    if (dateRange.HasValue && !string.IsNullOrEmpty(card.DateColumn))
    {
        var dateCol = SanitizeIdentifier(card.DateColumn);
        var from = dateRange.Value.From.ToString("yyyy-MM-dd");
        var to = dateRange.Value.To.ToString("yyyy-MM-dd");
        baseSql = $"SELECT * FROM ({baseSql.TrimEnd(';')}) AS _datefiltered " +
                  $"WHERE {dateCol} >= '{from}' AND {dateCol} <= '{to}'";
    }
    
    return baseSql;
}
```

**Key rule:** Only apply date filter when BOTH conditions are met:
1. `dateRange` is not null (frontend sent a preset)
2. `card.DateColumn` is not null/empty (card has a date column to filter on)

### 3.3 `KpiQueryBuilder.cs`

```csharp
// Update Build to accept DateRange:
public static KpiQueries Build(DashboardCard card, DateRange? dateRange = null)
{
    // Pass dateRange to BuildChangeQuery and BuildSparklineQuery
}

// Update BuildChangeQuery:
// When dateRange is provided, use it instead of calculated previous period:
private static string BuildChangeQuery(DashboardCard card, DateRange? dateRange = null)
{
    var baseQuery = card.SqlQuery.Trim().TrimEnd(';');
    var valueCol = NumericExpression(card.ValueColumn);
    var dateCol = SanitizeIdentifier(card.DateColumn);

    DateTime prevStart, prevEnd;
    if (dateRange.HasValue)
    {
        // Previous period = same duration, immediately before the filtered range
        var duration = dateRange.Value.To - dateRange.Value.From;
        prevEnd = dateRange.Value.From;
        prevStart = prevEnd - duration;
    }
    else
    {
        // Fallback: use existing GetPreviousPeriodRange
        (prevStart, prevEnd) = GetPreviousPeriodRange(card);
    }

    return $"SELECT SUM({valueCol}) AS PreviousValue FROM ({baseQuery}) AS _base " +
           $"WHERE {dateCol} >= '{prevStart:yyyy-MM-dd}' AND {dateCol} < '{prevEnd:yyyy-MM-dd}'";
}

// Update BuildSparklineQuery:
// When dateRange is provided, use it as the sparkline window instead of last N months:
private static string BuildSparklineQuery(DashboardCard card, DateRange? dateRange = null)
{
    var baseQuery = card.SqlQuery.Trim().TrimEnd(';');
    var valueCol = NumericExpression(card.ValueColumn);
    var dateCol = SanitizeIdentifier(card.DateColumn);

    var startDate = dateRange.HasValue 
        ? dateRange.Value.From 
        : DateTime.UtcNow.AddMonths(-(card.SparklineMonths > 0 ? card.SparklineMonths : 6));

    return $"SELECT FORMAT({dateCol}, 'yyyy-MM') AS Month, SUM({valueCol}) AS MonthlyValue " +
           $"FROM ({baseQuery}) AS _base " +
           $"WHERE {dateCol} >= '{startDate:yyyy-MM-dd}' " +
           $"GROUP BY FORMAT({dateCol}, 'yyyy-MM') " +
           $"ORDER BY Month";
}
```

### 3.4 `Index.cshtml` (Frontend — NO CHANGE)

The frontend already sends `?preset=` correctly. No changes needed.

---

## 4. التسلسل المنطقي

```
1. Card.cshtml.cs:
   - Add ResolvePresetDates(preset) → (DateTime, DateTime)?
   - Update OnGetAsync to accept preset param and resolve dates
   
2. DashboardService.cs:
   - Add DateRange record
   - Update method signatures (add DateRange? dateRange = null)
   - Update BuildSql to apply WHERE clause when dateRange + DateColumn exist
   
3. KpiQueryBuilder.cs:
   - Update Build, BuildChangeQuery, BuildSparklineQuery to accept DateRange?
   - Use dateRange for change period and sparkline window
   
4. Test:
   - dotnet build --no-restore
   - Verify all existing tests still pass
```

---

## 5. قواعد السلامة

1. **لا فلتر بدون DateColumn:** إذا البطاقة ليس لها `DateColumn` → لا يُضاف WHERE (البطاقة تعرض كل بياناتها)
2. **لا فلتر بدون preset:** إذا `preset` = null → لا يُضاف WHERE
3. **SQL Injection:** التواريخ تُمرَّر كـ `yyyy-MM-dd` ثابتة (آمنة) — لا `SqlParameter` needed for date literals
4. **Grand Total:** `BuildGrandTotalQuery` لا يتغير — الإجمالي الكلي لا يتأثر بفلتر التاريخ
5. **Compatibility:** جميع المعاملات الافتراضية `= null` → السلوك الحالي يبقى كما هو إذا لم يُرسل preset

---

## 6. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\Dashboard\Card.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs
```

---

## 7. UI Acceptance

| # | المعيار | Status |
|---|---|---|
| AC-1 | `preset=today` يُرجع بيانات ليوم واحد فقط (إذا البطاقة لها DateColumn) | ☐ |
| AC-2 | `preset=30days` يُرجع بيانات آخر 30 يوم | ☐ |
| AC-3 | البطاقات بدون `DateColumn` لا تتأثر بفلتر التاريخ (تعرض كل البيانات) | ☐ |
| AC-4 | KPI Change query يستخدم نطاق التاريخ نفسه للحساب | ☐ |
| AC-5 | KPI Sparkline يعرض بيانات النطاق المحدد | ☐ |
| AC-6 | `preset=null` (أو غير محدد) → السلوك الحالي (بدون فلتر) | ☐ |
| AC-7 | `dotnet build --no-restore` ناجح | ☐ |
| AC-8 | لا توجد أخطاء في console عند تحميل الداشبورد | ☐ |

---

## 8. Pre-Execution Gate Result

**Result:** PASS

### سبب PASS
- القرارات معتمدة (DateFilterMode = كل البطاقات)
- البنية جاهزة (DateFilterMode + DateColumn موجودان في الـ Model)
- هدف واحد واضح (توصيل preset → SQL)
- 4 ملفات فقط — أصغر وحدة ممكنة
- لا مكتبات جديدة
- لا تغيير في الـ Frontend
- لا تغيير في DB schema
- `DateRange? dateRange = null` → backward compatible

---

## 9. Notes for Agent

1. اقرأ كل الملفات الأربعة من القرص أولاً.
2. حافظ على كل السلوك القائم — `dateRange = null` يعني لا تغيير.
3. `SanitizeIdentifier` موجود في `KpiQueryBuilder.cs` — أعد استخدامه في `DashboardService.cs` (أو انقله لـ static helper مشترك).
4. لا تستخدم `SqlParameter` للتواريخ — `yyyy-MM-dd` literals آمنة في SQL Server.
5. `BuildGrandTotalQuery` لا يتغير.
6. شغّل `dotnet build --no-restore` بعد التعديل.
7. هذا مهم لدقة العرض — أي خطأ سيظهر بيانات خاطئة للمستخدم.
