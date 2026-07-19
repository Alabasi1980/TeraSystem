# TASK-CARD-KPI-04 — CategoryColumn Breakdown Table (Top 5)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-KPI-04 |
| **المجموعة** | CARD-DESIGN-EXECUTION / Phase C |
| **النوع** | Backend (KpiQueryBuilder + CardDataResult + DashboardService) + Frontend (Index.cshtml) |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | D2 Decision — CategoryColumn Breakdown (approved by Majed) |

---

## 1. الهدف

تفعيل `CategoryColumn` في بطاقات KPI المتقدمة لعرض **جدول تفصيلي** يُظهر أعلى 5 تصنيفات بالقيمة مرتبة من الأكبر إلى الأصغر.

### قرارات Majed المعتمدة:
- **5 تصنيفات** (وليس 3)
- **تلقائي** عند تحديد `CategoryColumn` (لا يحتاج إعداد `ShowCategoryBreakdown`)
- **عرض فقط** — لا drill-down، لا فلتر، فقط بيانات مرتبة

---

## 2. التصميم المستهدف

```
┌─────────────────────────────────┐
│  📦 إجمالي المخزون              │
│  1,250,000                      │
│  ↑ +12.3%  مقارنة بالشهر السابق│
│                                 │
│  ── حسب التصنيف ──              │
│  إلكترونيات    450,000  36%    │
│  ملابس         322,000  26%    │
│  أغذية         275,000  22%    │
│  مستلزمات طبية 150,000  12%    │
│  أخرى          103,000   8%    │
│  ─────────────────────────      │
│  الإجمالي: 1,250,000            │
└─────────────────────────────────┘
```

---

## 3. النطاق

### In Scope

**A. Backend — KpiQueryBuilder.cs:**
- Method جديد `BuildCategoryBreakdownQuery(card, dateRange)`
- الاستعلام: `SELECT TOP 5 CategoryColumn, SUM(ValueColumn) AS CategoryValue FROM ... GROUP BY CategoryColumn ORDER BY CategoryValue DESC`
- يُضاف لـ `KpiQueries` كـ `BreakdownSql`
- يُفعّل عندما:
  - `card.ChartType == "KPI"`
  - `card.KpiMode != "simple"`
  - `!string.IsNullOrEmpty(card.CategoryColumn)`
  - `!string.IsNullOrEmpty(card.ValueColumn)`

**B. Backend — CardDataResult.cs:**
- Property جديد: `public List<Dictionary<string, object?>>? KpiCategoryBreakdown { get; set; }`
- كل عنصر: `{ Category: "电子产品", Value: 450000, Percentage: 36.0 }`

**C. Backend — DashboardService.cs:**
- تنفيذ `BreakdownSql` وتحويل النتائج إلى قائمة مع النسب المئوية
- حساب النسبة لكل تصنيف: `(CategoryValue / TotalValue) * 100`

**D. Frontend — Index.cshtml:**
- Function جديد `wdRenderCategoryBreakdown(container, data, cardId)`
- جدول HTML بسيط: اسم التصنيف + القيمة + النسبة المئوية
- ألوان من `ColorPalette` — كل صف بلون مختلف (تدرج)
- RTL support
- Compact design — لا يأخذ مساحة كبيرة

### Out of Scope

1. لا نغير Card Builder
2. لا نغير قاعدة البيانات
3. لا نضيف drill-down أو فلتر
4. لا نغير Sparkline أو Change

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\CardDataResult.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\CardDataResult.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

---

## 6. التفاصيل التقنية

### 6.1 KpiQueryBuilder.cs — BuildCategoryBreakdownQuery

```csharp
private static string BuildCategoryBreakdownQuery(DashboardCard card, DashboardService.DateRange? dateRange = null)
{
    var baseQuery = NormalizeBaseQuery(card.SqlQuery);
    var valueCol = NumericExpression(card.ValueColumn);
    var categoryCol = SanitizeIdentifier(card.CategoryColumn);

    // Apply date filter if provided
    string dateFilter = "";
    if (dateRange is not null && !string.IsNullOrEmpty(card.DateColumn))
    {
        var dateCol = SanitizeIdentifier(card.DateColumn);
        dateFilter = $" WHERE {dateCol} >= '{dateRange.From:yyyy-MM-dd}' AND {dateCol} < '{dateRange.To:yyyy-MM-dd}'";
    }

    return $"SELECT TOP 5 {categoryCol} AS Category, SUM({valueCol}) AS CategoryValue " +
           $"FROM ({baseQuery}) AS _base" +
           $"{dateFilter} " +
           $"GROUP BY {categoryCol} " +
           $"ORDER BY CategoryValue DESC";
}
```

### 6.2 KpiQueries.cs — إضافة BreakdownSql

```csharp
public class KpiQueries
{
    public string? ChangeSql { get; set; }
    public string? SparklineSql { get; set; }
    public string? GrandTotalSql { get; set; }
    public string? BreakdownSql { get; set; }  // NEW
}
```

### 6.3 DashboardService.cs — تنفيذ Breakdown

بعد تنفيذ `BreakdownSql`:
```csharp
if (kpiQueries.BreakdownSql != null)
{
    try
    {
        var breakdownData = await ExecuteQueryAsync(kpiQueries.BreakdownSql, ct);
        if (breakdownData != null && breakdownData.Count > 0)
        {
            // Calculate total for percentages
            var total = breakdownData.Sum(r =>
                Convert.ToDouble(r.ContainsKey("CategoryValue") ? r["CategoryValue"] : 0));

            // Add percentage to each row
            foreach (var row in breakdownData)
            {
                var val = Convert.ToDouble(row.ContainsKey("CategoryValue") ? row["CategoryValue"] : 0);
                row["Percentage"] = total > 0 ? Math.Round((val / total) * 100, 1) : 0;
            }
            result.KpiCategoryBreakdown = breakdownData;
        }
    }
    catch (Exception ex)
    {
        // Log but don't fail the card
    }
}
```

### 6.4 Frontend — wdRenderCategoryBreakdown

```javascript
function wdRenderCategoryBreakdown(container, data, cardId) {
    var rows = Array.isArray(data) ? data : [];
    if (rows.length === 0) {
        container.style.display = 'none';
        return;
    }

    var pal = wdGetPalette(cardId);
    var html = '<div class="wd-kpi-breakdown">';
    html += '<div class="wd-kpi-breakdown__title">─ حسب التصنيف ─</div>';
    html += '<table class="wd-kpi-breakdown__table">';

    rows.forEach(function(row, i) {
        var category = row.Category || row.category || row.CategoryName || '—';
        var value = row.CategoryValue || row.categoryValue || row.Value || 0;
        var pct = row.Percentage || row.percentage || 0;
        var color = pal[i % pal.length] || 'var(--c-primary)';

        html += '<tr>';
        html += '<td class="wd-kpi-breakdown__cat" style="color:' + color + '">' + escHtml(category) + '</td>';
        html += '<td class="wd-kpi-breakdown__val">' + formatNum(toNum(value)) + '</td>';
        html += '<td class="wd-kpi-breakdown__pct">' + pct + '%</td>';
        html += '</tr>';
    });

    html += '</table></div>';
    container.innerHTML = html;
    container.style.display = '';
}
```

### 6.5 CSS (في Index.cshtml)

```css
.wd-kpi-breakdown {
    margin-top: 8px;
    border-top: 1px solid rgba(255,255,255,0.1);
    padding-top: 6px;
}
.wd-kpi-breakdown__title {
    font-size: 11px;
    color: var(--c-text-secondary);
    text-align: center;
    margin-bottom: 4px;
}
.wd-kpi-breakdown__table {
    width: 100%;
    border-collapse: collapse;
    font-size: 12px;
}
.wd-kpi-breakdown__table td {
    padding: 3px 4px;
}
.wd-kpi-breakdown__cat {
    font-weight: 500;
    white-space: nowrap;
}
.wd-kpi-breakdown__val {
    text-align: left;
    font-variant-numeric: tabular-nums;
}
.wd-kpi-breakdown__pct {
    text-align: left;
    color: var(--c-text-secondary);
    font-size: 11px;
    min-width: 36px;
}
```

---

## 7. Acceptance Criteria

1. **الجدول يظهر** عندما يكون `CategoryColumn` محدداً في Card Builder
2. **5 تصنيفات** مرتبة من الأكبر إلى الأصغر
3. **النسبة المئوية** صحيحة وتحتسب من إجمالي الـ 5 تصنيفات
4. **الألوان** تأتي من `ColorPalette` — كل صف بلون مختلف
5. **RTL support** — الجدول يتوافق مع الاتجاه العربي
6. **Compact design** — لا يأخذ مساحة كبيرة في البطاقة
7. **لا يظهر** عندما يكون `CategoryColumn` فارغاً
8. **Dotnet build** ينجح
9. **لا توجد تأثيرات جانبية** على KPI value أو change أو sparkline

---

## 8. Security Sensitivity

- **Level:** Low
- **Reason:** SQL query addition + frontend table rendering only. No auth, no secrets, no schema changes.

---

## 9. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | CategoryColumn breakdown only |
| One objective only | PASS | Display top 5 categories by value |
| No deferrable work included | PASS | All related to CategoryColumn |
| Allowed Write Targets are narrow | PASS | 4 files only |
| Acceptance criteria are testable | PASS | Visual + functional |

**Gate Status:** PASS

---

## 10. Delegation Notes

1. Before editing any file, read the current file from disk.
2. Preserve ALL unrelated code.
3. The breakdown is ONLY for KPI composite mode with CategoryColumn set.
4. The breakdown is automatic — no new Card Builder setting needed.
5. The breakdown shows top 5 only, sorted by value DESC.
6. Each row gets a color from the card's ColorPalette.
7. The breakdown does NOT affect the main KPI value, change, or sparkline.
8. Test with dotnet build.

---

## 11. Handback Placeholder

---

> **Prepared by:** TeraAgent
> **Mode:** Plan Mode — No code written in this document
