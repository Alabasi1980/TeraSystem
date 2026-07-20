# TASK-CARD-KPI-SMALL-COMPOSE-001

## المهمة: إعادة تركيب KPI في الحجم الصغير فقط — مجاميع يسار + تغير أوضح

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-20  
**النوع:** UI visual/layout fix  
**الوكيل:** ui-designer  

---

## ملاحظة العميل

في الحجم الصغير الحالي:
- المجاميع غير ظاهرة بوضوح.
- نسبة التغير صغيرة وغير واضحة.
- المطلوب: وضع المجاميع جهة اليسار عندما يكون الحجم صغير فقط، وإظهار نسبة التغير بشكل أجمل وأوضح.

## Design Source Decision

- **Design Source Mode:** USER_PROVIDED_REFERENCE + existing approved dashboard style
- **Selected Source:** لقطة العميل الحالية + `28_UI_UX_GUIDELINES.md` + نمط KPI mockup المعتمد
- **What will be used:** RTL dashboard style, blue card surface, golden sparkline, compact professional admin/dashboard card language
- **What will not be used:** أي تغيير على Medium/Large أو إعادة تصميم عامة للكارد
- **Final executable file:** `project-preparation/28_UI_UX_GUIDELINES.md` + current `Index.cshtml` component patterns

## Required Small-only Visual Direction

Small card should become a composed two-zone mini KPI:

```text
┌──────────────────────────────┐
│  refresh     details   info  title │
│                              │
│  totals stack      320.6K    │
│  إجمالي كلي        ▲ 3466%   │
│  سنوي/آخر...       واضح     │
│                              │
│  ─────── golden sparkline ─── │
└──────────────────────────────┘
```

### Required behavior

1. **Small only:** Move grand totals/details to the left side of the value area.
2. **Small only:** Make change percentage more visible:
   - Larger badge than current.
   - Stronger background/border.
   - Optional tiny arrow/icon if data direction exists.
   - Must not look like a faint small label.
3. **Small only:** Keep categories hidden.
4. **Small only:** Sparkline stays bottom, full-width, golden, no overlap.
5. **Medium/Large:** no visual change.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing the existing file, read current file from disk first.
- Modify only CSS/markup/JS needed for KPI small rendering in `Index.cshtml`.
- Prefer CSS-only if current DOM already contains grand totals and change badge.
- Do not touch backend, database, APIs, migrations, packages, config, or unrelated cards.
- Do not change Medium/Large KPI mockup layout.
- Use existing tokens and colors; no random palette.

## Vitality & Polish Checklist

- [ ] ✅ / N/A — Skeleton Loading / Shimmer — N/A, no loading behavior change
- [ ] ✅ / N/A — Toast Notifications — N/A, no user action feedback change
- [ ] ✅ / N/A — Connection Status Indicator — N/A, no connection status change
- [ ] ✅ / N/A — Search حقيقي — N/A, no table/search area
- [ ] ✅ / N/A — Micro-animations — preserve existing KPI/spark animations; add subtle change badge polish only if safe
- [ ] ✅ / N/A — Empty States — preserve existing insufficient/empty KPI states
- [ ] ✅ / N/A — Realistic Data — use real rendered data, no fake values

## Pre-Execution Gate

- Active Technology Profile: `dotnet-razorpages-adonet`
- UI Design Source: recorded above
- Allowed target: absolute path only
- Security Sensitivity: Low (CSS/UI only)
- Auditor Expected: NOT_REQUIRED unless broad JS/markup changes occur
- Gate Status: PASS

## Acceptance Criteria

- [x] Small: المجاميع ظاهرة جهة اليسار بوضوح.
- [x] Small: نسبة التغير أوضح وأكبر من الحالية.
- [x] Small: لا تداخل مع السبارك أو الرقم.
- [x] Small: التصنيفات تبقى مخفية.
- [x] Medium/Large: لا تغيير بصري جوهري.
- [x] Build succeeds or fallback build succeeds if running app locks output.

## Handback Summary

- File changed: `Index.cshtml` only.
- Small KPI `.wd-kpi__main` became a two-column grid scoped under `.wd-kpi--size-small`.
- Right side: hero value + clearer change badge.
- Left side: grand totals/details inside compact mini-panel.
- Sparkline remains bottom/full-width; categories remain hidden.
- Medium/Large protected by scoping all new styling under `.wd-kpi--size-small`.

## Post-Execution Review Gate: PASS

| Check | Result | Evidence |
|---|---|---|
| Allowed Write Targets respected | PASS | `Index.cshtml` only |
| No secrets | PASS | CSS/UI only |
| In scope | PASS | Small KPI layout only |
| Small totals left visible | PASS | `.wd-kpi--size-small .wd-kpi__grandtotal` grid-column 2 + mini panel |
| Change percent clearer | PASS | small-specific font/padding/border/shadow |
| No S overlap | PASS | main grid + spark section bottom flow |
| Categories hidden | PASS | `.wd-kpi--size-small .wd-kpi__categories { display: none !important; }` |
| Medium/Large unchanged | PASS | M/L rules not changed; new rules scoped to size-small |
| Build | PASS | Normal build blocked by running process PID 5824; fallback build succeeded 0 warnings / 0 errors |

## UI Acceptance Gate Result

| Check | Result | Evidence | Notes |
|---|---|---|---|
| UI source mode recorded | PASS | Task Design Source Decision | USER_PROVIDED_REFERENCE |
| `28_UI_UX_GUIDELINES.md` used as primary design source | PASS | Task design source | Existing dashboard style preserved |
| No invented random style | PASS | Uses existing tokens/color-mix | No new palette |
| Component rules followed | PASS | Existing KPI component classes | Small-only density |
| Layout pattern followed | PASS | Responsive small composition | Two-zone mini KPI |
| RTL/LTR rules followed | PASS | `direction: rtl` in small main | Visual right/left placement |
| Responsive behavior matches rules | PASS | Scoped `.wd-kpi--size-small` | M/L protected |
| Accessibility baseline checked | PASS | No interactive behavior changed | N/A for semantics |
| Forbidden styling avoided | PASS | No external frameworks/config | CSS only |
| Design gaps recorded instead of guessed | PASS | Gap recorded in AGENT_GAPS_LOG | Research-doc target conflict |
| Existing component patterns reused | PASS | Existing KPI DOM/classes | No broad rewrite |
| If FIGMA_DESIGN_FILE | N/A | N/A | N/A |

Gate Status: PASS

## Auditor Review Decision

- Decision: NOT_REQUIRED
- Reason: CSS-only, single existing Razor file, no auth/data/security/API/migration/shared infrastructure change.
- Auditor Report: N/A

## Closure

- Closed: 2026-07-20
- Reviewer: TeraAgent
