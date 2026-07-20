# TASK-KPI-HERO-TYPOGRAPHY-001

## المهمة: تخفيض حجم خط القيمة الرئيسية وتحسين بطاقة KPI الصغيرة

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-21  
**النوع:** UI CSS typography/layout refinement  
**الوكيل:** ui-designer  

---

## طلب Majed

بعد تطبيق فورمات المبالغ الكامل، أصبحت القيمة الرئيسية كبيرة جداً خصوصاً في M/L، والبطاقة S تحتاج تحسين إضافي بسبب ضيق المساحة وتداخل القيمة مع الإجماليات والسبارك.

## المطلوب

1. تخفيض حجم خط القيمة الرئيسية `.wd-kpi__value` تقريباً **30%**.
2. الحفاظ على وضوح الرقم ووزنه البصري.
3. تحسين خاص للبطاقة الصغيرة **S**:
   - منع التداخل بين القيمة الرئيسية والإجماليات والسبارك.
   - إبقاء القيمة الرئيسية قابلة للقراءة.
   - الحفاظ على الفورمات الحالي:
     - S: مختصر + `د.أ`
     - M/L: كامل مع `د.أ` و3 خانات عشرية.
4. لا تغيّر منطق الفورمات أو البيانات.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- No backend/data/API/database/migrations/config/packages.
- No change to `dashboard-utils.js`.
- No change to money-format logic.
- No new features.
- Scope is typography/layout only.

## Suggested Direction

Current main value sizes are roughly:

```css
.wd-kpi--size-small .wd-kpi__value { font-size: clamp(22px, 10cqi, 32px); }
.wd-kpi--size-medium .wd-kpi__value { font-size: clamp(34px, 11cqi, 52px); }
.wd-kpi--size-large .wd-kpi__value { font-size: clamp(40px, 12cqi, 64px); }
```

Target approximate 30% reduction:

```css
S: clamp(15px, ~7cqi, 22px)
M: clamp(24px, ~8cqi, 36px)
L: clamp(28px, ~8.5cqi, 45px)
```

But designer may tune visually to avoid cramped S.

For S, also consider:
- Slightly tighter letter-spacing if needed.
- Smaller line-height.
- Reduced vertical gaps around hero/totals.
- Ensure totals and sparkline do not overlap.
- Avoid absolute positioning unless necessary.

## Acceptance Criteria

- [x] M/L: main value font visually reduced by about 30% and no longer dominates/overflows.
- [x] S: main value fits without overlap with totals or sparkline.
- [x] S: totals remain visible and readable.
- [x] No money-format logic changed.
- [x] No M/L layout regression.
- [x] Build succeeds.

## Vitality & Polish Checklist

- N/A — Typography/layout refinement only; no new screen or component.

## Handback / Review Summary

### CSS changes in `Index.cshtml`

- Base `.wd-kpi__value`: `clamp(36px, 12cqi, 64px)` → `clamp(25px, 8.5cqi, 45px)`
- S: `clamp(22px, 10cqi, 32px)` → `clamp(15px, 7cqi, 22px)`
- M: `clamp(34px, 11cqi, 52px)` → `clamp(24px, 7.5cqi, 36px)`
- L: `clamp(40px, 12cqi, 64px)` → `clamp(28px, 8.5cqi, 45px)`
- Compact container query adjusted to `clamp(15px, 7cqi, 22px)`
- Very short container query adjusted to `clamp(14px, 6.5cqi, 20px)`
- Large roomy container query adjusted to `clamp(28px, 8.5cqi, 45px)`

### S layout refinement

- Reduced S gaps around main/hero.
- Removed aggressive S totals negative margin.
- S totals now use `margin-top: 0`, `margin-bottom: 6px`.
- Tightened S totals width/columns/gap to reduce crowding.

### Verification

- Build: PASS
- Warnings: 0
- Errors: 0
- Money-format JS logic: unchanged
- Changed application file: `Index.cshtml` only

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: CSS-only typography/layout refinement. No auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-21
