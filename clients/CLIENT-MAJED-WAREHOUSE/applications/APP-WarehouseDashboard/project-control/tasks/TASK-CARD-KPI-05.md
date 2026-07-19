# TASK-CARD-KPI-05 — GrandTotalSource: All-Time + Year-to-Date

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-KPI-05 |
| **المجموعة** | CARD-DESIGN-EXECUTION / Phase C |
| **النوع** | Backend (KpiQueryBuilder + DashboardService + CardDataResult) + Frontend (Index.cshtml + Builder.cshtml) |
| **الأولوية** | High |
| **الحالة** | ✅ Accepted — Auditor PASS |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | D5 Decision — GrandTotalSource (approved by Majed) |

---

## 1. الهدف

تفعيل `GrandTotalSource` في بطاقات KPI المتقدمة لعرض إجمالي أو إجماليين:
- **الإجمالي الكلي** (all-time) — بدون فلتر زمني
- **الإجمالي السنوي** (year-to-date) — لسنة محددة من الفلتر

### قرارات Majed المعتمدة:
- إعادة استخدام الحقل `GrandTotalSource` الموجود (بدون تعديل قاعدة البيانات)
- ثلاث قيم: `allTime` / `yearToDate` / `both`
- الافتراضي: `both` (كلاهما)

---

## 2. التصميم المستهدف

### عند `GrandTotalSource = "both"`:
```
┌─────────────────────────────────┐
│  📦 عدد الواردات                │
│  342                            │
│                                 │
│  ──────────────────────         │
│  الإجمالي الكلي: 12,500        │
│  إجمالي 2026:    3,200         │
└─────────────────────────────────┘
```

### عند `GrandTotalSource = "allTime"`:
```
┌─────────────────────────────────┐
│  📦 عدد الواردات                │
│  342                            │
│                                 │
│  ──────────────────────         │
│  الإجمالي الكلي: 12,500        │
└─────────────────────────────────┘
```

### عند `GrandTotalSource = "yearToDate"`:
```
┌─────────────────────────────────┐
│  📦 عدد الواردات                │
│  342                            │
│                                 │
│  ──────────────────────         │
│  إجمالي 2026:    3,200         │
└─────────────────────────────────┘
```

---

## 3. النطاق

### In Scope

**A. Backend — KpiQueryBuilder.cs:**
- Method جديد `BuildYearToDateQuery(card, dateRange)` — يجمع القيم للسنة المحددة فقط
- تعديل `Build()` لتمرير `GrandTotalSource` وبناء الاستعلام(s) الصحيح(s)
- تحديث `GrandTotalSql` ليكون `allTime` أو `yearToDate` أو كلاهما

**B. Backend — KpiQueries.cs:**
- Property جديد `YearToDateSql`

**C. Backend — CardDataResult.cs:**
- Property جديد `KpiYearToDateTotal`

**D. Backend — DashboardService.cs:**
- تنفيذ `YearToDateSql` وحفظ النتيجة

**E. Frontend — Index.cshtml:**
- تعديل `wdRenderGrandTotal` لعرض القيمة(s) حسب `card.grandTotalSource`
- `allTime` → الإجمالي الكلي فقط
- `yearToDate` → الإجمالي السنوي فقط
- `both` → كلاهما

**F. Frontend — Builder.cshtml:**
- تحديث الـ dropdown ليعرض:
  - `allTime` → الإجمالي الكلي (بدون فلتر زمني)
  - `yearToDate` → الإجمالي السنوي (لفترة السنة الحالية)
  - `both` → كلاهما (الافتراضي)

### Out of Scope

1. لا نغير قاعدة البيانات
2. لا نغير النموذج (DashboardCard)
3. لا نغير Sparkline أو Change
4. لا نغير CategoryColumn

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\CardDataResult.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\CardDataResult.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`

---

## 6. التفاصيل التقنية

### 6.1 KpiQueryBuilder.cs — BuildYearToDateQuery

```csharp
private static string BuildYearToDateQuery(DashboardCard card, DashboardService.DateRange? dateRange = null)
{
    var baseQuery = NormalizeBaseQuery(card.SqlQuery);
    var valueCol = NumericExpression(card.ValueColumn);
    var dateCol = SanitizeIdentifier(card.DateColumn);

    // Determine the year from the active filter or today
    int year;
    if (dateRange is not null)
    {
        year = dateRange.From.Year;
    }
    else
    {
        year = DateTime.UtcNow.Year;
    }

    var yearStart = $"{year}-01-01";
    var yearEnd = $"{year + 1}-01-01";

    return $"SELECT SUM({valueCol}) AS YearToDateTotal FROM ({baseQuery}) AS _base " +
           $"WHERE {dateCol} >= '{yearStart}' AND {dateCol} < '{yearEnd}'";
}
```

### 6.2 KpiQueryBuilder.cs — تعديل Build()

```csharp
// Grand Total — depends on GrandTotalSource
if (card.ShowGrandTotal && !string.IsNullOrEmpty(card.ValueColumn))
{
    var source = card.GrandTotalSource ?? "both";

    if (source == "allTime" || source == "both")
    {
        queries.GrandTotalSql = BuildGrandTotalQuery(card);
    }

    if (source == "yearToDate" || source == "both")
    {
        if (!string.IsNullOrEmpty(card.DateColumn))
        {
            queries.YearToDateSql = BuildYearToDateQuery(card, dateRange);
        }
    }
}
```

### 6.3 KpiQueries.cs — إضافة YearToDateSql

```csharp
public class KpiQueries
{
    public string? ChangeSql { get; set; }
    public string? SparklineSql { get; set; }
    public string? GrandTotalSql { get; set; }
    public string? YearToDateSql { get; set; }  // NEW
    public string? BreakdownSql { get; set; }
}
```

### 6.4 CardDataResult.cs — إضافة KpiYearToDateTotal

```csharp
/// <summary>Year-to-date total value (filtered to current year).</summary>
public object? KpiYearToDateTotal { get; set; }
```

### 6.5 DashboardService.cs — تنفيذ YearToDateSql

بعد تنفيذ GrandTotalSql:
```csharp
if (kpiQueries.YearToDateSql != null)
{
    try
    {
        var ytdTotal = await ExecuteScalarQueryAsync(kpiQueries.YearToDateSql, ct);
        result.KpiYearToDateTotal = ytdTotal;
    }
    catch (Exception ex)
    {
        // Log but don't fail the card
    }
}
```

### 6.6 Frontend — Index.cshtml — تعديل wdRenderGrandTotal

```javascript
function wdRenderGrandTotal(container, card) {
    var source = card.grandTotalSource || 'both';
    var html = '';

    if (source === 'allTime' || source === 'both') {
        if (card.kpiGrandTotal !== null && card.kpiGrandTotal !== undefined) {
            var val = formatNum(toNum(card.kpiGrandTotal));
            html += '<div class="wd-kpi-grandtotal__row">';
            html += '<span class="wd-kpi-grandtotal__label">الإجمالي الكلي:</span>';
            html += '<span class="wd-kpi-grandtotal__value">' + val + '</span>';
            html += '</div>';
        }
    }

    if (source === 'yearToDate' || source === 'both') {
        if (card.kpiYearToDateTotal !== null && card.kpiYearToDateTotal !== undefined) {
            var val = formatNum(toNum(card.kpiYearToDateTotal));
            var year = new Date().getFullYear();
            html += '<div class="wd-kpi-grandtotal__row">';
            html += '<span class="wd-kpi-grandtotal__label">إجمالي ' + year + ':</span>';
            html += '<span class="wd-kpi-grandtotal__value">' + val + '</span>';
            html += '</div>';
        }
    }

    if (!html) {
        container.style.display = 'none';
        return;
    }

    container.innerHTML = html;
    container.style.display = '';
}
```

### 6.7 Frontend — Builder.cshtml — تحديث الـ dropdown

```html
<select id="wb-kpi-grand-total-source" class="wd-select" name="grandTotalSource">
    <option value="both">كلاهما (الإجمالي الكلي + السنوي)</option>
    <option value="allTime">الإجمالي الكلي فقط</option>
    <option value="yearToDate">الإجمالي السنوي فقط</option>
</select>
```

### 6.8 CSS (في Index.cshtml)

```css
.wd-kpi-grandtotal {
    margin-top: 8px;
    border-top: 1px solid rgba(255,255,255,0.1);
    padding-top: 6px;
}
.wd-kpi-grandtotal__row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 12px;
    padding: 2px 0;
}
.wd-kpi-grandtotal__label {
    color: var(--c-text-secondary);
}
.wd-kpi-grandtotal__value {
    font-weight: 600;
    font-variant-numeric: tabular-nums;
}
```

---

## 7. Acceptance Criteria

1. **`GrandTotalSource = "both"`** → يعرض الإجمالي الكلي + الإجمالي السنوي
2. **`GrandTotalSource = "allTime"`** → يعرض الإجمالي الكلي فقط
3. **`GrandTotalSource = "yearToDate"`** → يعرض الإجمالي السنوي فقط
4. **السنة المحددة** تأتي من الفلتر النشط (أو السنة الحالية إذا لم يكن هناك فلتر)
5. **Card Builder** يعرض القيمة الثلاثة في الـ dropdown
6. **القيمة الافتراضية** هي `both` (كلاهما)
7. **Dotnet build** ينجح
8. **لا توجد تأثيرات جانبية** على KPI value أو change أو sparkline أو breakdown

---

## 8. Security Sensitivity

- **Level:** Low
- **Reason:** SQL query addition + frontend rendering only. No auth, no secrets, no schema changes.

---

## 9. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | GrandTotalSource implementation only |
| One objective only | PASS | Display all-time and/or year-to-date totals |
| No deferrable work included | PASS | All related to GrandTotalSource |
| Allowed Write Targets are narrow | PASS | 5 files only |
| Acceptance criteria are testable | PASS | Visual + functional |

**Gate Status:** PASS

---

## 10. Delegation Notes

1. Before editing any file, read the current file from disk.
2. Preserve ALL unrelated code.
3. The GrandTotalSource controls which totals are shown — not a new setting.
4. The year is derived from the active date filter or today's date.
5. The Builder dropdown must have the three options with Arabic labels.
6. The default value for new cards should be `both`.
7. Test with dotnet build.

---

## 11. Handback Placeholder

---

> **Prepared by:** TeraAgent
> **Mode:** Plan Mode — No code written in this document
