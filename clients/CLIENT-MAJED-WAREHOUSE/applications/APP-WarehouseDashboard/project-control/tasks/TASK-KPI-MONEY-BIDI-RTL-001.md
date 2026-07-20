# TASK-KPI-MONEY-BIDI-RTL-001

## المهمة: إصلاح اتجاه عرض القيم المالية في KPI داخل واجهة عربية RTL

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** UI CSS RTL/BiDi refinement  
**الوكيل:** ui-designer  

---

## المشكلة من الصورة

بعض القيم المالية لا تحترم الاتجاه العربي بسبب ترتيب BiDi:

- يظهر رمز العملة قبل الرقم بصرياً: `د.أ 1,226,558.020`
- المطلوب أن يظهر الرقم ثم العملة: `1,226,558.020 د.أ`
- الإجماليات تحتاج أيضاً عزل اتجاه أوضح بين الليبل العربي والقيمة الرقمية.

## المطلوب

- إصلاح عرض القيم المالية داخل KPI بحيث تظهر القيم المالية ككتلة معزولة:
  - الرقم أولاً
  - ثم `د.أ`
- الحفاظ على اتجاه الواجهة RTL والليبلات العربية.
- عدم تغيير الفورمات النصي أو JavaScript logic.
- عدم تغيير الأرقام أو البيانات.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- Prefer CSS-only solution.
- No JS money-format logic changes unless CSS-only is impossible; if impossible, STOP/ASK.
- No backend/data/API/database/migrations/config/packages.
- No layout redesign.

## Suggested Direction

Use targeted CSS for monetary value elements, for example:

```css
.wd-kpi__value,
.wd-kpi-grandtotal__value,
.wd-kpi-breakdown__val {
    direction: ltr;
    unicode-bidi: isolate;
}
```

Tune alignment carefully so the UI remains visually Arabic/RTL.

Check existing selectors first because `.wd-kpi-grandtotal__value` may already have some direction rules in S-specific selectors.

## Acceptance Criteria

- [x] Main KPI value displays visually as `1,226,558.020 د.أ`, not `د.أ 1,226,558.020`.
- [x] Grand totals display value and `د.أ` in correct visual order.
- [x] Breakdown money values remain correct.
- [x] Arabic labels remain RTL and readable.
- [x] No money-format logic changed.
- [x] Build succeeds.

## Vitality & Polish Checklist

- N/A — RTL/BiDi display refinement only.

## Handback / Review Summary

### CSS changes in `Index.cshtml`

- `.wd-kpi__value`:
  - `direction: ltr`
  - `unicode-bidi: isolate`
  - `text-align: start`
- `.wd-kpi-grandtotal__value`:
  - `direction: ltr`
  - `unicode-bidi: isolate`
  - `text-align: start`
- `.wd-kpi-breakdown__val`:
  - `direction: ltr`
  - `unicode-bidi: isolate`

### Verification

- Build: PASS
- Warnings: 0
- Errors: 0
- Changed application file: `Index.cshtml` only
- JS money-format logic: unchanged
- Arabic labels: untouched

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: CSS-only RTL/BiDi display refinement. No auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-21
