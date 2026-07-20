# TASK-CARD-KPI-S-TOTALS-ALIGN-001

## المهمة: إصلاح موقع ومحاذاة الإجماليات في بطاقة KPI الصغيرة فقط

**الحالة:** Accepted / Closed  
**التاريخ:** 2026-07-20  
**النوع:** UI CSS alignment fix  
**الوكيل:** ui-designer  

---

## المطلوب الآن فقط

إصلاح موقع الإجماليات في بطاقة KPI مقاس **S** فقط.

لا تنفذ الآن مهمة فورمات المبالغ العامة، فهي مؤجلة لمهمة لاحقة.

## مشكلة العميل

في لقطة S الحالية:
- الإجماليات ظاهرة لكن متداخلة/قريبة من السبارك.
- المطلوب أن تكون الإجماليات **جهة اليسار** في البطاقة الصغيرة.
- قد تكون المشكلة محاذاة فقط.
- تمييز الليبل عن القيمة:
  - كلمات مثل `الإجمالي الكلي` و `إجمالي 2026` بلون مختلف وغامق.
  - القيم بلون أسود غامق.
  - القيم جهة اليسار وليس اليمين.

## خارج نطاق هذه المهمة

لا تنفذ الآن:
- تعميم رمز الدينار الأردني `د.أ`.
- 3 خانات عشرية.
- إلغاء K/M وإظهار المبالغ كاملة.
- أي فورمات عام للمبالغ في النظام.

هذه ستكون مهمة ثانية بعد إنهاء هذه المهمة.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Required Fix

1. في `.wd-kpi--size-small` فقط:
   - اجعل بلوك الإجماليات يظهر جهة اليسار بصريًا داخل البطاقة.
   - لا تدفعه فوق السبارك ولا تجعله يتداخل معه.
   - حافظ على الشكل البسيط الحالي، بدون redesign جديد.
2. داخل الإجماليات:
   - الليبل (`الإجمالي الكلي`, `إجمالي 2026`) لون أغمق/مميز عن السابق.
   - القيمة لون أسود/غامق واضح.
   - القيمة تكون جهة اليسار داخل صف الإجماليات.
3. أبقِ الصفين ظاهرين في S:
   - `الإجمالي الكلي`
   - `إجمالي 2026`
4. لا تغيّر M/L.
5. لا تغيّر JS data formatting الآن.

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- No backend/data/API/database/migrations/config/packages.
- No extra files.
- No general money-format change in this task.
- No forced S size increase.
- No broad redesign.

## Acceptance Criteria

- [x] S: الإجماليات جهة اليسار بصريًا.
- [x] S: لا تداخل بين الإجماليات والسبارك.
- [x] S: يظهر الصفان `الإجمالي الكلي` و `إجمالي 2026`.
- [x] S: الليبلات أغمق/مميزة.
- [x] S: القيم غامقة وواضحة ومحاذاة يسار داخل صفوف الإجماليات.
- [x] M/L بدون تغيير بصري جوهري.
- [x] Build succeeds or fallback build succeeds.

## Handback / Review Summary

- File changed: `Index.cshtml` only.
- CSS-only S-scoped alignment fix.
- Totals block in `.wd-kpi--size-small` is pushed visually left using RTL-safe inline margin.
- Totals rows use a small grid so values sit visually left and labels sit right/readable.
- Labels are darker/distinct; values use strong dark text.
- Added bottom spacing between totals and sparkline to reduce overlap.
- No general money-format changes were made.
- Build verification: PASS via fallback OutDir, 0 warnings, 0 errors.

## Post-Execution Review Gate

- Result: PASS
- Auditor Review Decision: NOT_REQUIRED
- Reason: CSS-only S alignment fix, no auth/data/API/security/migration/shared infrastructure change.
- Closed: 2026-07-20
