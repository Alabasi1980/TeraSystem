# TASK-CARD-KPI-S-TOTALS-2ROWS-001

## المهمة: إظهار سطري المجاميع في KPI مقاس S مثل M

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-20  
**النوع:** UI CSS/layout fix  
**الوكيل:** ui-designer  

---

## دليل العميل

Majed أرسل صورتين لنفس بطاقة `السندات`:

- في مقاس **M** تظهر المجاميع كاملة:
  - `الإجمالي الكلي`
  - `إجمالي 2026`
- في مقاس **S** يظهر فقط:
  - `الإجمالي الكلي`

إذن البيانات موجودة، والمشكلة في CSS/كثافة مقاس S أو container query يخفي/يقص السطر الثاني.

## الهدف

في مقاس KPI **S** يجب أن يظهر نفس سطري المجاميع الموجودين في M:

1. `الإجمالي الكلي`
2. `إجمالي السنة` مثل `إجمالي 2026`

مع الحفاظ على:
- المجاميع جهة اليسار.
- الرقم ونسبة التغير جهة اليمين.
- السبارك أسفل.
- التصنيفات مخفية.
- M/L بدون تغيير.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Implementation Notes

اقرأ `Index.cshtml` من القرص أولاً. راجع خصوصًا:

- `.wd-kpi--size-small .wd-kpi-grandtotal__row:nth-child(n+3)`
- `@@container kpi (max-height: 130px)` وفيه `nth-child(n+2)`
- `@@container kpi (min-height: 131px)` override السابق
- أي `overflow`, `height`, `max-height`, أو `line-height` يجعل panel يعرض سطرًا واحدًا فقط

## Required Fix

- أصلح السبب الفعلي لإخفاء الصف الثاني في S.
- لا تعتمد فقط على تكبير إضافي إذا كان السطر مخفيًا بقاعدة CSS.
- في S، أظهر `nth-child(1)` و `nth-child(2)` دائمًا عندما يوجدان.
- أخفِ فقط `nth-child(n+3)` في S.
- إذا كانت `max-height: 130px` هي السبب، اجعل تأثيرها لا يخفي السطر الثاني لـ `.wd-kpi--size-small`، أو انقلها إلى حالة extreme فقط.
- حافظ على panel compact ولا تسمح بالتداخل مع sparkline.

## Constraints

- Before editing existing file, read current file from disk first.
- Write only to allowed target.
- No backend/data/API/database/migration/config/package changes.
- No fake/hardcoded totals.
- Do not change M/L visual design.

## Acceptance Criteria

- [x] S: يظهر `الإجمالي الكلي`.
- [x] S: يظهر `إجمالي السنة` / `إجمالي 2026`.
- [x] S: لا تداخل مع الرقم/التغير/السبارك.
- [x] S: التصنيفات مخفية.
- [x] M: يبقى كما في صورة العميل ويعرض السطرين.
- [x] L: لا تغيير بصري جوهري.
- [x] Build succeeds or fallback build succeeds if process lock exists.

## Handback / Review Summary

- Root cause: `@@container kpi (max-height: 130px)` hid `.wd-kpi-grandtotal__row:nth-child(n+2)` globally, so S lost the second total row.
- Runtime fix in `Index.cshtml`:
  - compact-height hide now applies only to non-small KPI cards using `.wd-kpi:not(.wd-kpi--size-small)`.
  - small KPI explicitly shows rows 1 and 2.
  - small KPI hides rows 3+ only.
- Build verification: PASS via fallback OutDir, 0 warnings, 0 errors.
- Governance note: ui-designer also wrote `design-source/REFERENCES.md` outside Allowed Write Targets. Tera did not delete it without Majed approval.

## Post-Execution Review Gate

- Result: PASS_WITH_GOVERNANCE_NOTE
- Auditor Review Decision: NOT_REQUIRED
- Reason: CSS-only small KPI fix; no auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-20
