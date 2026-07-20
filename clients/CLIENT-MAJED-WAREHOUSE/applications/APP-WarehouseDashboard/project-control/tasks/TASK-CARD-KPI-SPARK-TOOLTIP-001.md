# TASK-CARD-KPI-SPARK-TOOLTIP-001

## المهمة: إصلاح Tooltip منحنى KPI ليكون نظيفًا وواضحًا

**الحالة:** Accepted  
**التاريخ:** 2026-07-20  
**النوع:** UI polish (Frontend only)

---

## المشكلة (من لقطة العميل)

عند hover على نقاط السبارك لاين:
- النص مكسور/متداخل: `6.8+عن الشهر السابق:K`
- خلفية غير واضحة وحدود زرقاء غريبة
- خلط RTL للأرقام مع العربية في نفس السطر

---

## المطلوب

1. تنسيق CSS كامل لـ `.wd-spark-tooltip` وفرعياته
2. إعادة هيكلة HTML الـ tooltip: أسطر منفصلة، أرقام `dir="ltr"`، عربي منفصل
3. شكل نظيف: خلفية بيضاء/surface، ظل خفيف، padding، border radius، z-index عالٍ، بدون تداخل مع الخط
4. محتوى مقترح:
   - سطر 1: اسم الشهر (إن وُجد)
   - سطر 2: القيمة كبيرة وواضحة (9.1K) — LTR
   - سطر 3: التغير عن السابق بشكل نظيف مثل: `+6.8K عن الشهر السابق` مع الرقم LTR

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Pre-Execution Gate: PASS

صغيرة، هدف واحد، UI فقط.

## Acceptance Criteria

- [x] tooltip يظهر بخلفية واضحة وظل وحدود ناعمة
- [x] لا تداخل حروف عربية مع K/+
- [x] القيمة والتغير مقروءان فورًا
- [x] يعمل على كل نقاط المنحنى
- [x] build succeeds

---

## Handback

**Status:** Completed  
**File:** `src/WarehouseDashboard.Web/Pages/Index.cshtml` only  

### Changes
1. **CSS** — full `.wd-spark-tooltip` + sub-elements; kill Apex default chrome (blue box / transparent bg); sparkline `overflow: visible` so tooltip is not clipped.
2. **JS `deltaHtml`** — number and Arabic label in separate spans; number uses `direction:ltr` + `unicode-bidi:isolate` (no single mixed RTL string).
3. **Tooltip HTML** — month / value (`dir="ltr"`) / delta structure.
4. **Sparkline chart** — `chart.rtl: false` (LTR time series); `tooltip.intersect: true`; theme light/dark.

### Verify
- `dotnet build` → succeeded, 0 warnings, 0 errors.
