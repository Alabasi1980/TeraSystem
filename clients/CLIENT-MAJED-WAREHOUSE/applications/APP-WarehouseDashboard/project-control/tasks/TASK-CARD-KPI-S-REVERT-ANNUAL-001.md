# TASK-CARD-KPI-S-REVERT-ANNUAL-001

## المهمة: تصحيح فشل تصميم KPI S — الرجوع للشكل السابق وإضافة الإجمالي السنوي فقط

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-20  
**النوع:** UI correction / rollback-scoped fix  
**الوكيل:** ui-designer  

---

## سبب المهمة

Majed أوضح أن التغييرات الأخيرة فشلت تصميميًا. المطلوب الأصلي كان بسيطًا:

> إضافة الإجمالي السنوي تحت الإجمالي الكلي فقط.

وليس:
- تغيير شكل بطاقة S.
- نقل المجاميع إلى يسار البطاقة.
- تكبير S.
- إعادة تركيب التخطيط.

## الهدف النهائي

إرجاع KPI مقاس S إلى الشكل الأقرب لما كان قبل تغييرات:
- `TASK-CARD-KPI-SMALL-COMPOSE-001`
- `TASK-CARD-KPI-S-SIZE-TUNE-001`
- أي أثر تصميمي غير مطلوب

مع إضافة واحدة فقط:

```text
الإجمالي الكلي: 1.6M
إجمالي 2026: 1.6M
```

أي أن `إجمالي السنة` يظهر مباشرة تحت `الإجمالي الكلي` في نفس منطقة المجاميع، بنفس الأسلوب البصري السابق قدر الإمكان.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Required Corrections

1. ألغِ أي أثر لتغيير تصميم S الذي:
   - يجعل المجاميع mini-panel يسار مستقل.
   - يكبّر بطاقة KPI S عبر `grid-column: span 4` أو height forced.
   - يغير ترتيب الرقم/المجاميع عن الشكل السابق.
2. أبقِ S بنفس تصميمه السابق البسيط:
   - العنوان والأزرار كما كانت.
   - الرقم ونسبة التغير كما كانت.
   - المجاميع تحت/قرب الرقم كما كانت، وليس كتخطيط جديد.
   - السبارك كما كان.
3. أضف/اظهر السطر الثاني فقط:
   - row 1: `الإجمالي الكلي`
   - row 2: `إجمالي السنة` / `إجمالي 2026`
4. لا تغيّر M/L.
5. لا تغيّر البيانات أو JS إلا إذا كان ضرورياً جدًا لإزالة أثر تصميمي خاطئ.

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- No backend/data/API/database/migrations/config/packages.
- No fake/hardcoded totals.
- No new visual redesign.
- Do not write `design-source/REFERENCES.md` or any extra file.

## Acceptance Criteria

- [x] S يعود للشكل السابق المقبول قدر الإمكان.
- [x] S يعرض `الإجمالي الكلي`.
- [x] S يعرض تحته مباشرة `إجمالي 2026` عندما يكون موجودًا.
- [x] لا يوجد mini-panel يسار جديد.
- [x] لا يوجد forced S span/height جديد.
- [x] لا تداخل أو قص للرقم.
- [x] M/L بدون تغيير.
- [x] Build succeeds or fallback build succeeds.

## Handback / Review Summary

- File changed: `Index.cshtml` only.
- Rejected design effects verified absent:
  - No KPI S `grid-column: span 4` / forced `height: 240px` rule.
  - No small left mini-panel / two-column `.wd-kpi__main` design.
- S remains simple stacked layout: value → change → totals → sparkline.
- Annual total is shown by preserving first two `.wd-kpi-grandtotal__row` rows in `.wd-kpi--size-small`.
- Row 3+ remains hidden.
- Build verification: PASS via fallback OutDir, 0 warnings, 0 errors.

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: CSS-only correction, no auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-20
