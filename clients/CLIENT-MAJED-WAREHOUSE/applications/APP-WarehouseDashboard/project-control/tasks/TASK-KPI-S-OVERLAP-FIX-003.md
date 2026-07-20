# TASK-KPI-S-OVERLAP-FIX-003

## المهمة: إصلاح تداخل بطاقة KPI الصغيرة بعد تخفيض الخط

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** UI CSS S-only overlap fix  
**الوكيل:** ui-designer  

---

## المشكلة

بعد تخفيض خط القيمة الرئيسية، ظهرت مشكلة في بطاقة KPI الصغيرة **S**:

- السباركلاين يمر فوق/خلف الإجماليات.
- نسبة التغير قريبة جداً من السبارك.
- الإجماليات غير منفصلة بصرياً عن السبارك.

## المطلوب

إصلاح S فقط بحيث:

- لا يتداخل السبارك مع القيمة الرئيسية أو نسبة التغير أو الإجماليات.
- تبقى القيمة الرئيسية مختصرة ومقروءة.
- تبقى نسبة التغير واضحة.
- تبقى الإجماليات ظاهرة ومقروءة.
- لا يتغير M/L.
- لا يتغير منطق فورمات المبالغ.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- S-only CSS/layout changes.
- No M/L changes.
- No JS money-format logic changes.
- No backend/data/API/database/migrations/config/packages.
- No new feature.

## Suggested Direction

Use S-only selectors only. Consider a combination of:

- Reduce S sparkline height if needed.
- Move sparkline lower or reserve clear space above it.
- Ensure `.wd-kpi__sparkline-section` does not visually overlap text.
- Move totals above sparkline with enough bottom clearance.
- If needed, increase stacking clarity using S-only `position/z-index` for text areas, but avoid absolute positioning unless necessary.
- Tune previous S values:
  - `.wd-kpi--size-small .wd-kpi__hero { transform: translateY(-2px); }`
  - `.wd-kpi--size-small .wd-kpi__grandtotal/.wd-kpi__details { margin-top: -10px; margin-bottom: 12px; }`

## Acceptance Criteria

- [x] S: السبارك لا يمر فوق الإجماليات.
- [x] S: السبارك لا يلمس/يغطي نسبة التغير.
- [x] S: القيمة الرئيسية واضحة ومرفوعة بشكل مناسب.
- [x] S: الإجماليات واضحة ومقروءة.
- [x] M/L بدون تغيير.
- [x] Money-format logic unchanged.
- [x] Build succeeds.

## Vitality & Polish Checklist

- N/A — S-only spacing/overlap refinement.

## Handback / Review Summary

### CSS changes in `Index.cshtml`

- Added S-only stacking protection:
  - `.wd-kpi--size-small .wd-kpi__hero { position: relative; z-index: 2; }`
  - `.wd-kpi--size-small .wd-kpi__grandtotal/.wd-kpi__details { position: relative; z-index: 2; }`
- Relaxed S totals upward pull:
  - `margin-top: -10px` → `margin-top: -6px`
- Increased S totals clearance before sparkline:
  - `margin-bottom: 12px` → `margin-bottom: 16px`
- Added S-only sparkline separation:
  - `.wd-kpi--size-small .wd-kpi__sparkline-section { margin-top: 6px; position: relative; z-index: 0; }`
- Reduced S sparkline footprint:
  - `.wd-kpi--size-small .wd-kpi__sparkline { height: 32px; max-height: 34px; min-height: 28px; }`

### Verification

- Build: PASS
- Warnings: 0
- Errors: 0
- Changed application file: `Index.cshtml` only
- JS money-format logic: unchanged
- M/L behavior: untouched

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: S-only CSS overlap/spacing refinement. No auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-21
