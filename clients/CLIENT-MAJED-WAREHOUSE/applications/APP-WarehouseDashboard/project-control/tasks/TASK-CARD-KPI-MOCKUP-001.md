# TASK-CARD-KPI-MOCKUP-001

## المهمة: تنفيذ تصميم KPI المرجعي (Mockup العميل) مع مرونة الحجم

**الحالة:** Accepted  
**التاريخ:** 2026-07-20  
**النوع:** UI Redesign — pixel-close to client mockup  
**الأولوية:** High  

---

## الهدف

تنفيذ بطاقة KPI **كما في الصورة المرجعية** التي رسمها العميل، مع مرونة تكبير/تصغير حسب حجم البطاقة، وإزالة المساحات المهدورة.

---

## المرجع البصري (مصدر الحقيقة)

صورة العميل mockup:
- يمين: رقم كبير `542.4K` + شارة تغيير `77.1%` **تحت** الرقم + مجاميع مرتبة (قيمة ثم تسمية RTL)
- يسار: صندوق **أعلى التصنيفات** بحدود خفيفة، جدول: نسبة٪ | قيمة | كود بلون ذهبي/برتقالي (حتى 5 صفوف في L)
- أسفل: sparkline ذهبي/برتقالي ناعم بعرض كامل، نقاط، آخر نقطة أوضح، gradient خفيف
- الهيدر الحالي للبطاقة يبقى (KPI / تحديث / تفاصيل / العنوان) — خارج نطاق إعادة بناء الهيدر

---

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

Also updated (research + handback):
- `src/WarehouseDashboard.Web/design-source/REFERENCES.md`
- this task file

---

## UI Source / Rules / Acceptance

- UI Source: mockup العميل + `28_UI_UX_GUIDELINES.md` tokens
- UI Rules: RTL، توكنز زرقاء؛ لون الكود/السبارك الذهبي مسموح كـ accent وظيفي من mockup (`#E0A106` / `#E8A317`)
- UI Acceptance: `UI_ACCEPTANCE_GATE.md`
- Design Gap: إذا حقل الكود غير موجود في البيانات → استخدم Category name أو id متاح؛ لا تخترع backend

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe unit | PASS | KPI mockup layout only |
| One objective | PASS | Match client mockup + flexible sizes |
| No API/Auth/Schema | PASS | Frontend only |
| Allowed Write Targets narrow | PASS | Index.cshtml |
| Design source | PASS | Client mockup explicit |

**Gate Status: PASS**

---

## Acceptance Criteria

- [x] التخطيط يطابق mockup: تصنيفات يسار (صندوق) + رقم/تغيير/مجاميع يمين + سبارك أسفل
- [x] شارة التغيير تحت الرقم (ليس بجانبه بشكل فوضوي)
- [x] جدول التصنيفات: ٪ | قيمة | كود/معرّف بلون ذهبي
- [x] حتى 5 صفوف في Large؛ أقل في Medium؛ مخفي/مختصر في Small
- [x] سبارك ذهبي ناعم بنقاط وآخر نقطة بارزة
- [x] لا فراغات مهدورة كبيرة فوق/تحت المحتوى
- [x] مرن مع S/M/L + container queries
- [x] `dotnet build` succeeds
- [x] simple / withChange / composite لا تنكسر

---

## Vitality & Polish Checklist

- [x] N/A — Skeleton — خارج النطاق (KPI body mockup only)
- [x] N/A — Toast — خارج النطاق
- [x] N/A — Connection — خارج النطاق
- [x] N/A — Search — خارج النطاق
- [x] ✅ — Micro-animations — إبقاء wdKpiHeroIn / wdKpiBadgeIn / wdSparklineIn
- [x] ✅ — Empty States — إخفاء صندوق التصنيفات إن لا بيانات (`is-empty` + `:has` full-width main)
- [x] N/A — Realistic Data — بيانات API موجودة؛ تنسيق أرقام/نسب فقط
- [x] ✅ — البروتوتايب حي — spark markers + density sync + resize plumbing محفوظ

---

## Implementation Summary

**File:** `Index.cshtml` (KPI CSS + KPI JS only)

### Layout / CSS
- `.wd-kpi` height 100%, padding 12–14px, RTL, container queries
- `.wd-kpi__cluster` flex 1 + `justify-content: space-between` (row grows, spark bottom)
- `.wd-kpi__row` grid: main `1.15fr` then categories `0.95fr` (RTL → main right, cats left)
- Hero column: value huge `clamp(36px, 12cqi, 64px)` weight 800; change badge **below** number
- Change pill: primary-tinted calm border/bg; subtle up/down tint
- Categories: bordered soft panel 12px radius; title centered + bottom border
- Table LTR columns: `% | value | code(gold warning)`
- Grand totals: value bold then muted label; align under hero
- Spark heights: S 44 / M 56 / L 68–72
- Density: size classes + CQ; S hide cats; M 3–4 rows; L 5 rows
- Empty cats → `:has(.is-empty)` collapses row to 1 column

### JS (preserved plumbing)
- `wdSyncKpiDensity` unchanged contract (size class + spark resize)
- `wdDestroySparkline` / `wdRenderSparkline` kept; gold default `#E8A317`, markers all points, last size 7 + ring, soft orange fill, tighter grid padding
- `wdRenderCategoryBreakdown`: slice(0,5); columns pct|val|code from Code/CategoryCode/ItemCode/Id/Category
- `wdRenderGrandTotal`: HTML order value then label
- `wdRenderKpiCard` structure preserved (main → categories → spark)

---

## Build Result

```
dotnet build (WarehouseDashboard.Web)
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:10.59
```

**Status: Completed → Accepted by TeraAgent**

---

## Post-Execution Review — TeraAgent

| Check | Result | Notes |
|---|---|---|
| Allowed Write Targets | PASS* | Index.cshtml primary; REFERENCES.md = Approved deviation (research log only) |
| Mockup structure | PASS | categories panel left, hero+badge below+totals right, gold spark bottom |
| Badge under value | PASS | hero flex-direction column |
| Table % \| val \| code gold | PASS | |
| Density S/M/L + CQ | PASS | |
| Build | PASS | 0 errors |
| Auditor | NOT_REQUIRED | UI mockup match |

**Gate: PASS — Accepted ✅**
