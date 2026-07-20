# TASK-CARD-KPI-SMALL-001

## المهمة: إصلاح تخطيط KPI للحجم الصغير فقط — لا تداخل + إظهار التغير والمجاميع

**الحالة:** Completed  
**التاريخ:** 2026-07-20  
**النوع:** UI fix (small size only)

---

## المطلوب (Small فقط)

من لقطات العميل:
1. **لا تداخل** بين الرقم/المعلومات والمنحنى (sparkline)
2. **إظهار نسبة التغير** بوضوح
3. **إظهار المجاميع** بوضوح (سطر أو سطرين مضغوطين)
4. **جدول أعلى التصنيفات يبقى مخفيًا** في الصغير (ممتاز — لا تغيّر)
5. **الحجم الوسط والكبير لا يتأثران إطلاقًا**

## التخطيط المستهدف للصغير

```
┌─────────────────────────┐
│  header (موجود)         │
│  1.3M                   │  ← رقم
│  [589.5%]               │  ← شارة تغير واضحة
│  1.5M الإجمالي الكلي    │  ← مجاميع مضغوطة
│  ────────────────────   │
│  ·———·———·———● spark    │  ← تحت المعلومات، بدون تداخل
└─────────────────────────┘
```

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## قيود التنفيذ

- عدّل فقط قواعد `.wd-kpi--size-small` و/أو `@container kpi` للضيق/القصير
- **لا** تغيّر تخطيط M/L العام (mockup)
- ألغِ أي `display:none` على `.wd-kpi__grandtotal` / change في small إذا كان يمنع الإظهار
- في container queries التي تخفي grandtotal عند max-height — استثنِ أو خفّف للصغير بحيث تظهر المجاميع متى وُجدت مساحة معقولة
- stack عمودي صارم: main فوق، spark أسفل، gap واضح (6–8px)، overflow hidden بدون تراكب
- spark أنحف في S (~36–40px) ليترك مكانًا للرقم + التغير + مجاميع
- الرقم أصغر قليلًا إن لزم (clamp) حتى لا يزاحم المنحنى

## Pre-Execution Gate: PASS

## Post-Execution Review Gate: PASS
| Check | Status |
|---|---|
| Allowed Write Targets | ✅ Index.cshtml only |
| Small — no overlap | ✅ row column flex 0 1 auto, spark pinned bottom margin-top auto |
| Small — change visible | ✅ display inline-flex important |
| Small — totals visible | ✅ display flex important, max 2 rows |
| Small — categories hidden | ✅ display none important |
| M/L unchanged | ✅ lines 530–594 untouched |
| Build | ✅ 0 errors |
| Auditor Decision | NOT_REQUIRED (CSS-only, single file, no auth/data/security) |

**Closed — 2026-07-20**

## Acceptance Criteria

- [x] Small: لا تداخل بين الرقم/التغير/المجاميع والسبارك
- [x] Small: نسبة التغير ظاهرة
- [x] Small: المجاميع ظاهرة (إن توفرت بيانات)
- [x] Small: التصنيفات مخفية
- [x] Medium/Large: بدون تغيير بصري جوهري
- [x] build succeeds

---

## Handback

**Status:** Completed  
**Date:** 2026-07-20  
**File changed:** `Index.cshtml` (CSS only — small density + softened CQ)

### What changed
1. **`.wd-kpi--size-small` stack:** single-column row (`flex: 0 1 auto`), main not growing into spark, cluster `justify-content: flex-start`, spark section `margin-top: auto` + `flex-shrink: 0`.
2. **Value:** `clamp(22px, 10cqi, 32px)` — smaller to free room.
3. **Change badge:** forced `display: inline-flex` (visible).
4. **Grand totals:** forced `display: flex` on small; up to 2 compact rows at 10px; categories stay `display: none`.
5. **Spark:** ~38px height on small.
6. **Container queries softened:** removed grandtotal hide at 160/170/130px; only hide totals at extreme `max-height: 110px`. Compact CQ keeps value + change + totals + thin spark.

### Verify
- `dotnet build` → succeeded (0 errors; exe copy warnings only — process locked by running app).
