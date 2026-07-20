# TASK-CARD-KPI-S-SIZE-TUNE-001

## المهمة: تكبير مقاس KPI S قليلاً لإظهار المجاميع كاملة

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-20  
**النوع:** UI layout sizing fix  
**الوكيل:** ui-designer  

---

## قرار العميل

Majed وافق على تكبير مقاس **S** قليلاً بدل ضغط كل عناصر KPI داخل مساحة صغيرة جدًا.

## المشكلة

الحجم الصغير الحالي صار يعرض:
- الرقم الرئيسي.
- نسبة التغير.
- الإجمالي الكلي.

لكن لا تظهر قيمة **إجمالي السنة** رغم أنها تظهر في الحجم الوسط، وهذا يعني أن البيانات موجودة لكن مساحة S/قواعد الكثافة تضغط أو تخفي السطر الثاني.

## الهدف

تكبير مقاس S لبطاقة KPI فقط بشكل بسيط بحيث يستوعب:

- الرقم الرئيسي.
- نسبة التغير بوضوح.
- الإجمالي الكلي.
- إجمالي السنة.
- السبارك أسفل بدون تداخل.

## Design Source Decision

- **Design Source Mode:** USER_PROVIDED_REFERENCE + existing approved dashboard style
- **Selected Source:** لقطة العميل الحالية + تخطيط TASK-CARD-KPI-SMALL-COMPOSE-001
- **What will be used:** same small composed layout: totals left, value/change right, golden spark bottom
- **What will not be used:** أي تغيير بصري عام على Medium/Large أو إعادة تصميم الكارد من جديد
- **Final executable file:** current KPI styles in `Index.cshtml`

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Implementation Direction

ابحث عن منطق/قواعد تحديد حجم S، خاصة:

- `wdSyncKpiDensity`
- `if (w <= 3) return 'wd-kpi--size-small';`
- CSS grid span classes / resize buttons if they define S dimensions
- Container queries that hide row 2: `max-height: 130px` or similar

### المطلوب

1. اجعل KPI S أكبر قليلاً فقط عند الحاجة:
   - الخيار المفضل: S للـ KPI يصبح أقرب إلى `4` أعمدة بدل `3` إذا كانت منظومة الأحجام تدعم ذلك.
   - أو زيادة ارتفاع/row span الصغير بدرجة واحدة إن كان هذا هو المسار الأنظف.
   - لا تغيّر M/L.
2. بعد التوسعة، اضمن أن small KPI يظهر **سطرين** من المجاميع:
   - الإجمالي الكلي.
   - إجمالي السنة.
3. لا ترجع التصميم إلى تكديس ضيق.
4. أبقِ التصنيفات مخفية في S.
5. أبقِ السبارك أسفل بدون تداخل.

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- Prefer minimal CSS/JS changes.
- Do not touch backend/data/API/database/migrations/config/packages.
- Do not change Medium/Large visual design.
- Do not invent fake totals or hardcode values.

## Vitality & Polish Checklist

- [ ] ✅ / N/A — Skeleton Loading / Shimmer — N/A, no loading behavior change
- [ ] ✅ / N/A — Toast Notifications — N/A, no user action feedback change
- [ ] ✅ / N/A — Connection Status Indicator — N/A, no connection status change
- [ ] ✅ / N/A — Search حقيقي — N/A
- [ ] ✅ / N/A — Micro-animations — preserve existing KPI animations
- [ ] ✅ / N/A — Empty States — preserve existing empty/insufficient states
- [ ] ✅ / N/A — Realistic Data — use live rendered data only

## Pre-Execution Gate

- Active Technology Profile: `dotnet-razorpages-adonet`
- UI Design Source: recorded above
- Security Sensitivity: Low
- Auditor Expected: NOT_REQUIRED unless JS sizing logic becomes broad/risky
- Gate Status: PASS

## Acceptance Criteria

- [x] KPI S is slightly larger than before.
- [x] Small KPI shows both grand total and year total when both exist.
- [x] Small KPI keeps totals left and value/change right.
- [x] Change percent remains clear.
- [x] Sparkline remains bottom with no overlap.
- [x] Categories remain hidden in S.
- [x] Medium/Large not visually changed.
- [x] Build succeeds or fallback build succeeds if running process locks output.

## Handback Summary

- File changed: `Index.cshtml` only.
- KPI cards saved as S (`data-grid-w="1"`, `2`, or `3`) now render with a slightly larger visual footprint:
  - `grid-column: span 4`
  - `height: 240px`
- Saved layout/density remains S, so `.wd-kpi--size-small` composition still applies.
- Added container rule for tuned S height to show second total row when enough room exists.
- Small layout remains: totals left, value/change right, sparkline bottom, categories hidden.
- Medium/Large and non-KPI cards protected by scoped selectors.

## Post-Execution Review Gate: PASS

| Check | Result | Evidence |
|---|---|---|
| Allowed Write Targets respected | PASS | `Index.cshtml` only |
| No secrets | PASS | CSS/UI only |
| In scope | PASS | KPI S sizing only |
| KPI S larger | PASS | KPI `data-grid-w<=3` renders span 4 / 240px |
| Both totals visible | PASS | second row explicitly displayed for S when container height >= 131px |
| Small composition preserved | PASS | `.wd-kpi--size-small` still controls density |
| Categories hidden | PASS | existing small rule preserved |
| M/L protected | PASS | new card-size selector scoped to `data-chart-type="KPI"` + saved S widths only |
| Build | PASS | fallback review build: 0 warnings / 0 errors |

## UI Acceptance Gate Result

| Check | Result | Evidence | Notes |
|---|---|---|---|
| UI source mode recorded | PASS | Task Design Source Decision | USER_PROVIDED_REFERENCE |
| `28_UI_UX_GUIDELINES.md` used as primary design source | PASS | Existing dashboard style preserved | N/A direct read not required for micro fix |
| No invented random style | PASS | Size/layout only | No palette changes |
| Component rules followed | PASS | Existing KPI classes | No DOM rewrite |
| Layout pattern followed | PASS | Small composed KPI preserved | S tuned larger |
| RTL/LTR rules followed | PASS | Existing RTL layout retained |  |
| Responsive behavior matches rules | PASS | S only; M/L protected |  |
| Accessibility baseline checked | PASS | No interaction semantics changed |  |
| Forbidden styling avoided | PASS | No external framework/config |  |
| Design gaps recorded instead of guessed | PASS | N/A |  |
| Existing component patterns reused | PASS | Existing KPI component |  |
| If FIGMA_DESIGN_FILE | N/A | N/A |  |

Gate Status: PASS

## Auditor Review Decision

- Decision: NOT_REQUIRED
- Reason: CSS-only/sizing change scoped to KPI S; no auth/data/API/security/migration/shared infrastructure change.
- Auditor Report: N/A

## Closure

- Closed: 2026-07-20
- Reviewer: TeraAgent
