# TASK-KPI-S-VERTICAL-ALIGN-002

## المهمة: ضبط محاذاة بطاقة KPI الصغيرة — رفع الإجماليات والقيمة

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** UI CSS S-only layout refinement  
**الوكيل:** ui-designer  

---

## طلب Majed

في بطاقة KPI الصغيرة **S**:

- رفع الإجماليات للأعلى لتكون بمحاذاة نسبة التغير.
- رفع القيمة الرئيسية للأعلى قليلاً.
- الحفاظ على القراءة وعدم التداخل مع السبارك.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- S-only changes.
- No M/L changes.
- No JS money-format logic changes.
- No backend/data/API/database/migrations/config/packages.
- No new features.

## Acceptance Criteria

- [x] S: الإجماليات مرفوعة للأعلى ومحاذية تقريباً مع نسبة التغير.
- [x] S: القيمة الرئيسية مرفوعة للأعلى قليلاً.
- [x] S: لا تداخل بين القيمة/النسبة/الإجماليات/السبارك.
- [x] M/L بدون تغيير.
- [x] Money format logic unchanged.
- [x] Build succeeds.

## Vitality & Polish Checklist

- N/A — S-only spacing/alignment refinement.

## Handback / Review Summary

### CSS changes in `Index.cshtml`

- Added S-only hero lift:
  - `.wd-kpi--size-small .wd-kpi__hero { transform: translateY(-2px); }`
- Adjusted S-only totals spacing:
  - `margin-top: -10px`
  - `margin-bottom: 12px`
- Updated comment marker to `VALIGN-002`.

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
- Reason: S-only CSS spacing/alignment refinement. No auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-21
