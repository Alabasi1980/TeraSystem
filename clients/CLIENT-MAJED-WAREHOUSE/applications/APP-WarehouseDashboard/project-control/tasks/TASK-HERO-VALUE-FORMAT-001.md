# TASK-HERO-VALUE-FORMAT-001

## المهمة: تحسين فورمات القيمة الرئيسية في KPI (حسب الحجم)

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** JavaScript format enhancement  
**الوكيل:** engineering-agent-dotnet  

---

## المطلوب

تغيير فورمات القيمة الرئيسية (Hero Value) في بطاقات KPI لتعتمد على حجم البطاقة:

| الحجم | الشكل | مثال |
|---|---|---|
| **S** (صغير) | مختصر + د.أ | `14.7M د.أ` |
| **M** (وسط) | كامل مثل الإجماليات | `14,700,000.000 د.أ` |
| **L** (كبير) | كامل مثل الإجماليات | `14,700,000.000 د.أ` |

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

DO NOT write to any other files.

## Constraints

- Before editing, read current file from disk first.
- Write only to allowed target (Index.cshtml).
- No CSS changes.
- `formatNum` and `formatMoney` functions in `dashboard-utils.js` must remain unchanged.

## Detailed Changes

### Change 1 — `wdRenderKpiCard` (lines ~1448-1452)

تحديد فورمات القيمة الرئيسية حسب `sizeClass`:

**Before:**
```js
var display = card.kpiFormatted !== null && card.kpiFormatted !== undefined
    ? card.kpiFormatted
    : (rawValue !== null && rawValue !== undefined
        ? (window.formatNum ? window.formatNum(window.toNum(rawValue)) : rawValue)
        : '—');
```

**After:**
```js
var display = card.kpiFormatted !== null && card.kpiFormatted !== undefined
    ? card.kpiFormatted
    : (rawValue !== null && rawValue !== undefined
        ? (sizeClass === 'wd-kpi--size-small'
            ? (window.formatNum ? window.formatNum(window.toNum(rawValue)) + ' د.أ' : rawValue + ' د.أ')
            : (window.formatMoney ? window.formatMoney(window.toNum(rawValue)) : rawValue))
        : '—');
```

**المنطق:**
- إذا `sizeClass === 'wd-kpi--size-small'` → استخدم `formatNum` (مختصر) + ` د.أ`
- وإلا (M/L) → استخدم `formatMoney` (كامل مع د.أ)

**ملاحظة:** `sizeClass` متاح بالفعل في الدالة (محسوب من `data-grid-w` في الأسطر 1427-1434).

### Change 2 — `animateCountUp` (lines ~1337-1341)

تحديث القيمة النهائية بعد انتهاء الأنيميشن حسب حجم البطاقة:

**Before (lines 1337-1341):**
```js
el.textContent = formatNum ? formatNum(current) : current.toLocaleString();
if (progress < 1) {
    requestAnimationFrame(step);
} else {
    el.textContent = formatNum ? formatNum(target) : target.toLocaleString();
}
```

**After:**
```js
el.textContent = formatNum ? formatNum(current) : current.toLocaleString();
if (progress < 1) {
    requestAnimationFrame(step);
} else {
    // Final value: format depends on kpi size class (S vs M/L)
    var kpiEl = el.closest('.wd-kpi');
    var isSmall = kpiEl && kpiEl.classList.contains('wd-kpi--size-small');
    el.textContent = isSmall
        ? (window.formatNum ? window.formatNum(target) + ' د.أ' : target + ' د.أ')
        : (window.formatMoney ? window.formatMoney(target) : (window.formatNum ? window.formatNum(target) : target));
}
```

**المنطق:**
- أثناء الأنيميشن: يبقى `formatNum` (مختصر) لجميع الأحجام — أنعم للحركة.
- بعد الأنيميشن: يتحقق من حجم البطاقة عبر `el.closest('.wd-kpi').classList`.
- S → `formatNum(target) + ' د.أ'`
- M/L → `formatMoney(target)`

## Verification

```
dotnet build "D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\WarehouseDashboard.Web.csproj" -p:OutDir="C:\Users\Fares\AppData\Local\Temp\opencode\wd-hero-format-build\"
```

## Acceptance Criteria

- [x] S: القيمة الرئيسية تظهر مختصرة + ` د.أ` (مثل `14.7M د.أ`)
- [x] M/L: القيمة الرئيسية تظهر كاملة مع د.أ (مثل `14,700,000.000 د.أ`)
- [x] animateCountUp: القيمة النهائية تستخدم الفورمات الصحيح حسب الحجم
- [x] formatNum / formatMoney لم يتغيرا
- [x] Build succeeds

## Handback / Review Summary

### Changes in Index.cshtml (2 edits)

**Change 1 — `wdRenderKpiCard` display variable:**
- S: `formatNum(rawValue) + ' د.أ'` (مختصر مع العملة)
- M/L: `formatMoney(rawValue)` (كامل مع العملة)

**Change 2 — `animateCountUp` final value:**
- أثناء الأنيميشن: `formatNum` لجميع الأحجام (سلس)
- بعد الأنيميشن: يتحقق من `.wd-kpi--size-small` في الـ DOM
  - S → `formatNum(target) + ' د.أ'`
  - M/L → `formatMoney(target)`

### Not changed
- `dashboard-utils.js` لم يتغير
- `formatNum` و `formatMoney` بدون تغيير
- لا CSS ولا backend ولا config

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: Pure client-side JS format change. No auth/data/API/security/migration.
- Closed: 2026-07-21
