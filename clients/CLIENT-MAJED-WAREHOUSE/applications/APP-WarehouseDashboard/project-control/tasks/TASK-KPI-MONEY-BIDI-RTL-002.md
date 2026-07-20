# TASK-KPI-MONEY-BIDI-RTL-002

## المهمة: تصحيح نهائي لاتجاه العملة وترتيب أعمدة إجماليات KPI

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** UI RTL/BiDi + KPI totals column order refinement  
**الوكيل:** ui-designer  

---

## المشكلة

التصحيح السابق لم يحقق المطلوب بصرياً بالكامل:

1. رمز العملة `د.أ` يجب أن يظهر **بعد الرقم بصرياً**، أي على **يسار الرقم للمستخدم** في الواجهة العربية.
2. صفوف الإجماليات معكوسة: يجب أن يظهر عمود الليبلات قبل عمود المبالغ في القراءة العربية.

## المطلوب بصرياً

```text
الإجمالي الكلي:   1,558,483.620 د.أ
إجمالي 2026:      1,558,483.620 د.أ
```

أي:
- الليبل العربي على اليمين.
- المبلغ على يسار الليبل.
- داخل المبلغ: الرقم ثم `د.أ` بصرياً، و`د.أ` تكون على يسار الرقم للمستخدم.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- Prefer CSS-first, but frontend render markup/order may be adjusted if required.
- No backend/data/API/database/migrations/config/packages.
- Do not change numeric values.
- Do not change decimal precision or comma formatting.
- Scope to KPI money/total display only.

## Acceptance Criteria

- [x] Main KPI value visually shows number then `د.أ` (`1,226,558.020 د.أ`).
- [x] Grand total values visually show number then `د.أ`.
- [x] In grand total rows, label column is before amount column in Arabic reading order: label right, amount left.
- [x] Breakdown values still readable and do not regress.
- [x] M/L and S remain visually stable.
- [x] Build succeeds.

## Notes for implementer

- Previous task added `direction: ltr; unicode-bidi: isolate` to money values. Re-evaluate because this may not be enough for Arabic currency suffix inside an RTL page.
- Possible solution may require:
  - `unicode-bidi: plaintext` / `isolate-override` tuning,
  - `dir`/span isolation in rendered HTML,
  - or changing grand-total row grid/DOM order to label then value.
- Do not guess: verify visually/structurally from the generated HTML/CSS rules.

## Vitality & Polish Checklist

- N/A — RTL/BiDi alignment refinement only.

## Handback / Review Summary

### Changes in `Index.cshtml`

- Added `.wd-kpi-money` wrapper styling:
  - `display: inline-flex`
  - `direction: rtl`
  - isolated number/currency spans
- Added `wdKpiMoneyHtml(value)` helper to split existing formatted money text into:
  - number span: `dir="ltr"`
  - currency span: `dir="rtl"`
- Applied wrapper to:
  - KPI hero initial render
  - KPI hero final count-up render
  - Grand total values
  - Breakdown money values
- Changed grand total DOM order to label then value.
- Grand total row now uses RTL grid so label is right and amount is left.

### Critical correction after review

- Initial implementation used `dir="ltr"` on money wrapper, which could place `د.أ` on the wrong side.
- Corrected wrapper to `dir="rtl"` / `direction: rtl`, with DOM order number then currency.
- Result: in RTL UI, number appears on the right and `د.أ` appears to its left, as requested.

### Verification

- Build: PASS
- Warnings: 0
- Errors: 0
- Changed application file: `Index.cshtml` only
- `dashboard-utils.js`: unchanged
- Numeric format output: unchanged

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: Frontend KPI RTL/BiDi display refinement only. No auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-21
