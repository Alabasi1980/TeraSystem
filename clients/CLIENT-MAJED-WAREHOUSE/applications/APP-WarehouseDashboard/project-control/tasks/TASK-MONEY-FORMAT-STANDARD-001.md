# TASK-MONEY-FORMAT-STANDARD-001

## المهمة: توحيد فورمات المبالغ في بطاقات KPI

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** JavaScript utility + KPI render update  
**الوكيل:** engineering-agent-dotnet  

---

## المطلوب

تغيير طريقة عرض القيم المالية في بطاقات KPI من الاختصار (K/M/B) إلى عرض **كامل مع د.أ + 3 خانات عشرية + فواصل**.

### الفورمات الجديد

| الحالة | الحالي | الجديد |
|---|---|---|
| Grand Totals | `14.7M` | `14,700,000.000 د.أ` |
| Sparkline tooltip | `1.2M` | `1,200,000.000 د.أ` |
| Breakdown values | `534K` | `534,000.000 د.أ` |
| Drill-down KPI | `14.7M` | `14,700,000.000 د.أ` |
| **Hero Value (القيمة الرئيسية)** | `14.7M` | **يبقى مختصراً** `14.7M` |

### أين: `formatMoney` أضف دالة جديدة في `dashboard-utils.js`

```js
function formatMoney(v) {
    var n = toNum(v);
    // Format with commas, 3 decimal places, full number (no K/M/B)
    var parts = n.toFixed(3).split('.');
    var intPart = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    return intPart + '.' + parts[1] + ' د.أ';
}
```

**ملاحظة:** `formatNum` تبقى كما هي للقيمة الرئيسية.

### أين تستخدم في `Index.cshtml` (4 أماكن)

1. **`wdRenderGrandTotal`** (line ~1706, ~1717) — تغيير `formatNum(toNum(...))` → `formatMoney(toNum(...))`
2. **`formatSparkValue`** (line ~1550) — تغيير `formatNum(toNum(val))` → `formatMoney(toNum(val))`
3. **Breakdown value** (line ~1687) — تغيير `formatNum(toNum(value))` → `formatMoney(toNum(value))`
4. **Drill-down KPI** (line ~2244) — تغيير `formatNum(val)` → `formatMoney(val)`

### أماكن تبقى كما هي (تستخدم `formatNum`)

- Hero KPI value (line ~1451) — يبقى `formatNum`
- `animateCountUp` (line ~1337, ~1341) — يبقى `formatNum` (لأنه يعرض القيمة الرئيسية)

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\dashboard-utils.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing any file, read current file from disk first.
- Write only to allowed targets.
- No extra files.
- No backend/data/API/database/migrations/config/packages.
- No CSS changes.
- `formatNum` must remain unchanged.
- `formatMoney` must be a separate function.

## Acceptance Criteria

- [x] `dashboard-utils.js`: `formatMoney(num)` function added (commas + 3 decimals + ` د.أ`).
- [x] `Index.cshtml`: Grand totals use `formatMoney`.
- [x] `Index.cshtml`: Sparkline tooltip uses `formatMoney`.
- [x] `Index.cshtml`: Breakdown values use `formatMoney`.
- [x] `Index.cshtml`: Drill-down KPI uses `formatMoney`.
- [x] `Index.cshtml`: Hero KPI value still uses `formatNum` (abbreviated).
- [x] No CSS, backend, or config changes.
- [x] Build succeeds.

## Vitality & Polish Checklist

- N/A — هذا تعديل فورمات JS، ليس UI واجهة جديدة.

## Handback / Review Summary

### Files changed

**`dashboard-utils.js`:**
- Added JSDoc line for `formatMoney`
- Added `function formatMoney(v)` between `formatNum` and `escapeHtml`
- Exposed `window.formatMoney = formatMoney`

**`Index.cshtml` (5 changes):**
| # | Location | Change |
|---|---|---|
| 1 | `formatSparkValue` (line ~1550) | `formatNum` → `formatMoney` |
| 2 | Breakdown value (line ~1687) | `formatNum` → `formatMoney` |
| 3 | Grand Total all-time (line ~1706) | `formatNum` → `formatMoney` |
| 4 | Grand Total year-to-date (line ~1717) | `formatNum` → `formatMoney` |
| 5 | Drill-down KPI (line ~2244) | `formatNum` → `formatMoney` |

### Not changed (verified)
- Hero KPI value (line 1451): still `window.formatNum` ✓
- `animateCountUp` (lines 1337, 1341): still `formatNum` ✓
- No CSS, backend, config, or package files ✓

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: Pure client-side JS formatting change. No auth/data/API/security/migration/shared infrastructure.
- Closed: 2026-07-21
