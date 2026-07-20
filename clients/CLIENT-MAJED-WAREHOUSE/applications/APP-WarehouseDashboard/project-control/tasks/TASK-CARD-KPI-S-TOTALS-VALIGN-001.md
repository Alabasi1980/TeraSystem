# TASK-CARD-KPI-S-TOTALS-VALIGN-001

## المهمة: رفع إجماليات KPI S لمحاذاة القيمة الرئيسية

**الحالة:** Approved  
**التاريخ:** 2026-07-21  
**النوع:** UI CSS alignment fix  
**الوكيل:** ui-designer  

---

## المطلوب

في بطاقة KPI مقاس **S** فقط:

- ارفع بلوك الإجماليات للأعلى.
- اجعله بمحاذاة عمودية مع القيمة الرئيسية تقريبًا.
- حافظ على الإجماليات جهة اليسار.
- حافظ على الصفين:
  - `الإجمالي الكلي`
  - `إجمالي 2026`
- لا تلمس فورمات المبالغ العام.
- لا تغيّر M/L.

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Constraints

- Before editing `Index.cshtml`, read current file from disk first.
- Write only to allowed target.
- No extra files.
- No backend/data/API/database/migrations/config/packages.
- No money formatting standardization in this task.
- No redesign; alignment only.

## Acceptance Criteria

- [ ] S: الإجماليات مرفوعة للأعلى ومحاذية تقريبًا للقيمة الرئيسية.
- [ ] S: الإجماليات تبقى جهة اليسار.
- [ ] S: لا تداخل مع السبارك.
- [ ] S: الصفان ظاهران.
- [ ] M/L بدون تغيير.
- [ ] Build succeeds or fallback build succeeds.
