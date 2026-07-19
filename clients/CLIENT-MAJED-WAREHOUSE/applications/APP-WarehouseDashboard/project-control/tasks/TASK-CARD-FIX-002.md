# TASK-CARD-FIX-002 — Fix SqlQuery Storage: Store Table Name, Not Pre-Aggregated SQL

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-FIX-002 |
| **المجموعة** | CARD-DESIGN-EXECUTION |
| **النوع** | Bug Fix / Backend + Frontend |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | Critical |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `CARD_SETTINGS_UTILIZATION_ANALYSIS_AND_PLAN.md` — §4.4 DateFilterMode |
| **السبب** | card-builder.js يخزّن استعلاماً مُجمَّعاً مسبقاً كـ SqlQuery → يمنع تطبيق الفلتر + يسبب تجميعاً مزدوجاً |

---

## 1. الهدف

إصلاح آلية حفظ `SqlQuery` في Card Builder بحيث:
- **بطاقات KPI:** تُخزَّن **اسم الجدول فقط** (مثلاً `[stg_st_invoice]`)
- **باقي الأنواع:** تبقى `SELECT * FROM [table]` كما هي
- **`BuildSql` في Backend:** يتولى التجميع + الفلترdinamically

---

## 2. تدفق المشكلة

```
المستخدم: اختار جدول stg_st_invoice + AggregationType=Sum + ValueColumn=FINAL_SUM_INVOICE

card-builder.js → buildSqlTableQueryForSave():
  SqlQuery = "SELECT SUM([FINAL_SUM_INVOICE]) AS [FINAL_SUM_INVOICE] FROM [stg_st_invoice]"
             ^^^^^ مُجمَّع مسبقاً!

DashboardService.BuildSql(card):
  → detects alreadyAggregated = TRUE (لأنه يحتوي SUM())
  → skips aggregation ✅ (هذا جيد)
  → tries ApplyDateFilter → WHERE [INV_DATE] >= ...
  → INV_DATE غير موجود في النتيجة! ❌

النتيجة: خطأ "Invalid column name 'INV_DATE'"
```

---

## 3. الإصلاح المطلوب

### 3.1 `card-builder.js` — `buildSqlTableQueryForSave()`

```javascript
// BEFORE (خاطئ):
CardBuilderWizard.prototype.buildSqlTableQueryForSave = function () {
    var table = this.state.selectedTable;
    if (!table || !table.sqlTargetTable) return '';

    if (this.state.cardType === 'KPI') {
      var valueColumn = $('wb-kpi-value-column') ? $('wb-kpi-value-column').value : '';
      if (valueColumn) {
        return 'SELECT SUM(' + this.buildNumericExpression(table, valueColumn) + ') AS [' + valueColumn + '] FROM [' + table.sqlTargetTable + ']';
      }
    }

    return 'SELECT * FROM [' + table.sqlTargetTable + ']';
};

// AFTER (صحيح):
CardBuilderWizard.prototype.buildSqlTableQueryForSave = function () {
    var table = this.state.selectedTable;
    if (!table || !table.sqlTargetTable) return '';

    // KPI cards: store table name only — BuildSql handles aggregation + date filter
    if (this.state.cardType === 'KPI') {
      return '[' + table.sqlTargetTable + ']';
    }

    // Other card types: SELECT * is fine (no aggregation needed)
    return 'SELECT * FROM [' + table.sqlTargetTable + ']';
};
```

### 3.2 `DashboardService.cs` — `BuildSql()` — فرع non-View

```csharp
// BEFORE:
else
{
    baseSql = card.SqlQuery;
}

// AFTER:
else
{
    var trimmed = card.SqlQuery.Trim().TrimEnd(';').Trim();
    // Handle bare table name: [stg_st_invoice] or stg_st_invoice
    if (!trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
        && !trimmed.Contains(" FROM ", StringComparison.OrdinalIgnoreCase))
    {
        var safe = trimmed.StartsWith("[", StringComparison.Ordinal) ? trimmed : $"[" + trimmed + "]";
        baseSql = $"SELECT * FROM {safe}";
    }
    else
    {
        baseSql = trimmed;
    }
}
```

---

## 4. التوافق مع البطاقات الحالية

**البطاقات الحالية** لديها `SqlQuery` مُجمَّع مسبقاً:
```sql
SELECT SUM([FINAL_SUM_INVOICE]) AS [FINAL_SUM_INVOICE] FROM [stg_st_invoice]
```

**`BuildSql` الحالي** يتعامل معها بشكل صحيح:
```csharp
var alreadyAggregated = upperSql.Contains("SUM(") || ...;
if (alreadyAggregated) return baseSql;  // يتخطاها ✅
```

**لا نحتاج هجرة** — البطاقات الحالية ستبقى تعمل. الإصلاح ي affects فقط البطاقات الجديدة.

---

## 5. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs
```

---

## 6. UI Acceptance

| # | المعيار | Status |
|---|---|---|
| AC-1 | بطاقة KPI جديدة تُخزّن اسم الجدول فقط في SqlQuery | ☐ |
| AC-2 | بطاقة Chart/Table تُخزّن `SELECT * FROM [table]` | ☐ |
| AC-3 | BuildSql يبني استعلام صحيح لاسم الجدول المخزّن | ☐ |
| AC-4 |فلتر التاريخ يعمل مع البطاقات الجديدة | ☐ |
| AC-5 | البطاقات الحالية (المُجمَّعة مسبقاً) لا تتأثر | ☐ |
| AC-6 | `dotnet build --no-restore` ناجح | ☐ |

---

## 7. Pre-Execution Gate

**Result:** PASS

- إصلاح bug حرج
- ملفان فقط
- لا تغيير في DB schema
- لا تغيير في API
- توافق مع البيانات الحالية
