# TASK-CARD-UX-002 — Apply Per-Card ColorPalette to Chart / Gauge / Sparkline

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-UX-002 |
| **المجموعة** | CARD-DESIGN-EXECUTION |
| **النوع** | UI / JavaScript + CSS |
| **الوكيل المقترح** | ui-designer |
| **الأولوية** | High |
| **الحالة** | Accepted |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `DASHBOARD_CARD_DESIGN_EXECUTION_PLAN.md` — Phase A / CARD-UX-03 |
| **القرار المعتمد** | Majed: **A — كل بطاقة تستخدم لوحتها الخاصة دائماً** |

---

## 1. الهدف

جعل كل بطاقة تُستخدم **`ColorPalette` الخاصة بها فعلياً** عند رسم:
- Bar / Line / Pie
- Gauge
- Sparkline (في KPI متقدم)

بدلاً من الألوان العامة الثابتة الحالية.

---

## 2. الوضع الحالي

- `data-color-palette` موجود على كل `<article>` في DOM ✅
- `window.WD_CARDS[i].colorPalette` موجود ✅
- لكن وظائف الرسم تستخدم **ألواناً عامة ثابتة**:

| الوظيفة | الموقع | الألوان الحالية |
|---|---|---|
| `wdRenderChart` | `Index.cshtml:686` | `colors: isPie ? PALETTE : ['#1F4E79']` |
| `wdRenderGauge` | `Index.cshtml:753` | `var gaugeColor = '#2E6DA4'` |
| `wdRenderSparkline` | `Index.cshtml:572` | `var sparkColor = isDark ? '#6baadf' : '#1F4E79'` |

---

## 3. تعريفات الألوان (Color Palettes)

هذه هي اللوحات المعتمدة (من `Builder.cshtml.cs` — `ColorPalettes`):

| المعرف | الألوان |
|---|---|
| `primary` | `#1F4E79`, `#2E6DA4`, `#8FBCDE` |
| `secondary` | `#2E6DA4`, `#1F4E79`, `#8FBCDE` |
| `accent` | `#0A2540`, `#1F4E79`, `#2E6DA4` |
| `success` | `#1E9E6A`, `#28A745`, `#4CD97B` |
| `warning` | `#E0A106`, `#FFC107`, `#FFD54F` |
| `info` | `#2E6DA4`, `#17A2B8`, `#4FC3F7` |
| `custom` | `#1F4E79`, `#E0A106`, `#1E9E6A`, `#D64545`, `#2E6DA4`, `#8FBCDE` |

**يجب تضمين هذه التعريفات في JS الداشبورد** لأن `card-templates.js` غير محمّل في صفحة `Index.cshtml`.

---

## 4. النطاق

### المطلوب

1. تضمين تعريفات الـ 7 لوحات ألوان في قسم `<script>` بـ `Index.cshtml`.
2. تعديل `wdRenderChart` لتستخدم ألوان لوحة البطاقة بدل الثابت.
3. تعديل `wdRenderGauge` لتستخدم لون اللوحة (اللون الأول) بدل الثابت.
4. تعديل `wdRenderSparkline` لتستخدم اللون الأول من لوحة البطاقة.
5. الحفاظ على `var PALETTE = ...` كـ fallback للبطاقات التي لا تحمل لوحة محددة.

### كيفية الحصول على اللوحة لكل بطاقة
- استخدام `data-color-palette` من عنصر الـ DOM: `document.getElementById('card-' + card.cardId).getAttribute('data-color-palette')`
- أو البحث في `window.WD_CARDS` عن `card.cardId`
- إذا كانت القيمة فارغة أو غير معروفة → استخدام `primary` كإفتراضي.

### غير المطلوب

- لا تغيير في الـ API
- لا تغيير في الـ Backend
- لا تغيير في KPI badge colors (هي CSS-native وتعمل)
- لا إضافة مكتبات جديدة
- لا تغيير في شريط التواريخ أو الفلترة
- لا تغيير في الهيكل العام للبطاقة
- لا تغيير في header/tooltip

---

## 5. UI Source

- المرجع البصري: بطاقات تنفيذية فاخرة، بيضاء/زرقاء، كل بطاقة لها شخصية لونية خاصة.
- المستخدم اعتمد القرار: **A — كل بطاقة تستخدم لوحتها دائماً**.
- `project-preparation/DASHBOARD_CARD_DESIGN_EXECUTION_PLAN.md` — §4.1 (ColorPalette)
- `project-preparation/28_UI_UX_GUIDELINES.md`

---

## 6. UI Rules

1. لا Syncfusion.
2. لا تضيف مكتبة جديدة.
3. تعريفات الألوان يجب أن تكون مطابقة تماماً للوحات في `Builder.cshtml.cs`.
4. اللوحة الأولى (`primary`) = #1F4E79, #2E6DA4, #8FBCDE — هي الافتراضية العالمية.
5. يجب أن تبقى ألوان الـ `wd-kpi__change` (badge التغير) كما هي (CSS) — هذه لا تتغير مع اللوحة.
6. الحفاظ على RTL.

---

## 7. UI Acceptance

| # | المعيار | Status |
|---|---|---|
| AC-1 | تعاريف اللوحات السبع متوفرة في JS الداشبورد | ☐ |
| AC-2 | Bar/Line/Pie تستخدم ألوان لوحة البطاقة المحددة | ☐ |
| AC-3 | Gauge يستخدم اللون الأول من لوحة البطاقة | ☐ |
| AC-4 | Sparkline يستخدم اللون الأول من لوحة البطاقة | ☐ |
| AC-5 | البطاقات بدون لوحة (فارغة/غير معروفة) تستخدم `primary` كإفتراضي | ☐ |
| AC-6 | `dotnet build --no-restore` ينجح بدون أخطاء | ☐ |

---

## 8. Design Gap Handling

إذا احتاجت أي وظيفة رسم (مثل Pie أو Gauge) إلى معالجة خاصة لتوزيع الألوان المتعددة، قم بتنفيذ أبسط نسخة تعمل (مثلاً استخدم أول لونين/ثلاثة من اللوحة)، وسجّل الفجوة في الـ Handback بدل التوسع.

---

## 9. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css
```

(ملاحظة: قد لا تحتاج `blue-theme.css` — تعتمد على التنفيذ. إذا لم تحتجها، لا تلمسها.)

---

## 10. Pre-Execution Gate Result

**Result:** PASS

### سبب PASS
- هدف واحد واضح
- القرار المعتمد من Majed موجود
- البنية التحتية (data-color-palette) جاهزة
- لا يحتاج backend logic
- لا يحتاج API
- لا يحتاج مكتبات جديدة
- أصغر وحدة تالية في الخطة

---

## 11. Vitality & Polish Checklist

- [x] N/A — Skeleton Loading / Shimmer — لا تغيير
- [x] N/A — Toast Notifications — لا تغيير
- [x] N/A — Connection Status Indicator — لا تغيير
- [x] N/A — Search — لا تغيير
- [x] ✅ Micro-animations — لو أمكن، hover effect بسيط على الألوان
- [x] N/A — Empty States — لا تغيير
- [x] N/A — Realistic Data — تعتمد على البيانات

---

## 12. Notes for Agent

1. اقرأ `Index.cshtml` الحالي من القرص أولاً.
2. حافظ على كل السلوك القائم (loading, fetch, drill, filter, resize).
3. لا تلمس `blue-theme.css` إلا إذا كان هناك CSS ضروري للتغيير (غالباً لا).
4. لا تضيف ملف JS خارجي جديد.
5. اختبر `dotnet build --no-restore` بعد التعديل.
6. إذا احتجت `card-templates.js` معرفاً للوحات — لا تحمّله في الصفحة، بل انسخ تعريفات اللوحات السبع مباشرة داخل `Index.cshtml`.

### ملاحظة مهمة عن ملف card-templates.js
صفحة الداشبورد لا تحمّل `card-templates.js` (محجوز لصفحة Builder فقط). لذلك لا تعتمد عليه — انسخ تعريفات اللوحات مباشرة.

---

## 13. UI Designer Handback

**Status:** DONE

### Files Changed
- `src/WarehouseDashboard.Web/Pages/Index.cshtml`

### Approach
- Added `COLOR_PALETTES` JS object (7 palettes matching Builder.cshtml.cs exactly).
- Added `wdGetPalette(cardId)` helper with 3-level lookup logic:
  1. `data-color-palette` DOM attribute
  2. `window.WD_CARDS[i].colorPalette`
  3. Defaults to `primary`
- Updated `wdRenderChart`, `wdRenderGauge`, `wdRenderSparkline` to use per-card palette via `wdGetPalette`.

### Build Result
- `dotnet build --no-restore` → Build succeeded. 0 warnings, 0 errors.

### Acceptance Criteria
- AC-1 ✅ — 7 palette definitions in JS
- AC-2 ✅ — Bar/Line/Pie use per-card palette
- AC-3 ✅ — Gauge uses first palette colour
- AC-4 ✅ — Sparkline uses first palette colour
- AC-5 ✅ — Unknown/empty → `primary` or `PALETTE` fallback
- AC-6 ✅ — Build passes

---

## 14. Tera Post-Execution Review

**Result:** PASS

### Review Summary
- Fresh file read completed.
- Scope respected: only `Index.cshtml` changed; no CSS, no backend, no libraries.
- No Syncfusion.
- Build re-run by Tera: PASS (0 warnings / 0 errors).
- Auditor report: `QUAUD-TASK-CARD-UX-002-2026-07-19-001.md`
- Auditor result: **PASS** (6/6 AC, scope respected, non-blocking FLAG only)

### Auditor Review Decision
- **REQUIRED → COMPLETED**
- Report: `project-control/audit-reports/QUAUD-TASK-CARD-UX-002-2026-07-19-001.md`
- Non-blocking finding: Gauge gradient could be improved (FLAG-CUX2-001) — optional low-priority follow-up.

### Final Decision
- **Accepted**
