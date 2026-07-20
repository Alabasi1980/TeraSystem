# TASK-KPI-HERO-TYPOGRAPHY-002

## المهمة: تخفيض إضافي لحجم خط القيمة الرئيسية في KPI

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** UI CSS typography refinement  
**الوكيل:** ui-designer  

---

## طلب Majed

بعد التخفيض السابق، ما زالت القيمة الرئيسية في بطاقة KPI، خصوصاً مقاس **M**، كبيرة وتضغط بصرياً على منطقة التصنيفات.

## المطلوب

- تخفيض إضافي لحجم خط `.wd-kpi__value`.
- التركيز على M/L، مع ضبط S بحذر إن احتاج.
- الحفاظ على وضوح الرقم.
- منع التداخل مع التصنيفات/الإجماليات/السبارك.
- لا تغيّر فورمات المبالغ أو JavaScript logic.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- CSS typography/layout only.
- No JS money-format logic changes.
- No backend/data/API/database/migrations/config/packages.

## Current values to tune

Current post-typography values are approximately:

```css
.wd-kpi__value { font-size: clamp(25px, 8.5cqi, 45px); }
.wd-kpi--size-small .wd-kpi__value { font-size: clamp(15px, 7cqi, 22px); }
.wd-kpi--size-medium .wd-kpi__value { font-size: clamp(24px, 7.5cqi, 36px); }
.wd-kpi--size-large .wd-kpi__value { font-size: clamp(28px, 8.5cqi, 45px); }
```

Suggested direction:

```css
Base: around clamp(21px, 7cqi, 38px)
S: around clamp(13px, 6cqi, 18px)
M: around clamp(20px, 6.2cqi, 30px)
L: around clamp(24px, 7cqi, 38px)
```

Also reduce container query overrides accordingly so they do not re-enlarge values.

## Acceptance Criteria

- [x] M: main value visibly smaller and no longer collides/presses into breakdown area.
- [x] L: main value still strong but less dominant.
- [x] S: remains readable and not overcrowded.
- [x] No money-format logic changed.
- [x] Build succeeds.

## Vitality & Polish Checklist

- N/A — Typography refinement only.

## Handback / Review Summary

### CSS changes in `Index.cshtml`

- Base `.wd-kpi__value`: `clamp(25px, 8.5cqi, 45px)` → `clamp(21px, 7cqi, 38px)`
- S: `clamp(15px, 7cqi, 22px)` → `clamp(13px, 6cqi, 18px)`
- M: `clamp(24px, 7.5cqi, 36px)` → `clamp(20px, 6.2cqi, 30px)`
- L: `clamp(28px, 8.5cqi, 45px)` → `clamp(24px, 7cqi, 38px)`
- Compact container query: `clamp(13px, 6cqi, 18px)`
- Very short container query: `clamp(12px, 5.5cqi, 17px)`
- Large roomy container query: `clamp(24px, 7cqi, 38px)`

### Verification

- Build: PASS
- Warnings: 0
- Errors: 0
- Changed application file: `Index.cshtml` only
- JS money-format logic: unchanged

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: CSS-only typography refinement. No auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-21
