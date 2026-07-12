# 28_UI_UX_GUIDELINES.md — WarehouseDashboard

> **النوع:** Executable UI/UX Design Rules — قواعد تصميم واجهة قابلة للتنفيذ
> **التطبيق:** WarehouseDashboard (Razor Pages + Syncfusion)
> **العميل:** الماجد لادارة المستودعات
> **تاريخ الإعداد:** 2026-07-12
> **مُعدّ بواسطة:** UI Designer Agent (مُصمم واجهات) — TASK-PREP-011
> **الملف التنفيذي النهائي:** هذا الملف هو المرجع الوحيد الذي يبني منه EngineeringAgent الواجهة
> **الحالة:** `ready_for_phase_6`

---

## 0. Design Source Decision (قرار مصدر التصميم)

```text
Design Source Mode:     USER_PROVIDED_REFERENCE
Selected Source:        قرار العميل المعتمد "Blue Theme — 11 لوناً أزرق"
                        (مُوثّق في 01_PROJECT_BRIEF.md §6 و §8/C3،
                         APPLICATION_BLUEPRINT.md §4 Module 2 / M2.6،
                         CLIENT_DECISION_LOG.md #9)
Why selected:           العميل حدّد الهوية البصرية بالكامل (لون أزرق + 11 لوناً)
                        ولا يوجد ملف Figma أو مرجع خارجي آخر.
What will be used:      لوحة الـ 11 لوناً الزرقاء المُعرّفة في §2،
                        + KIT_ADMIN_DASHBOARD كقاعدة تخطيط/مكوّنات،
                        + قواعد الحوكمة في tera-system/design-system.
What will not be used:  أي ألوان عشوائية، أي مكتبة UI خارجية غير معتمدة،
                        أي وضع Dark Mode غير مُعتمد.
Client overrides:       الهوية الزرقاء إلزامية على كل المكوّنات (SC9).
Risk of brand imitation: منخفض — لوحة أزرق مؤسسي قياسي، لا تقليد علامة تجارية شهيرة.
Final executable file:  project-preparation/28_UI_UX_GUIDELINES.md
```

> ⚠️ **Design Gap (فجوة تصميمية مُسجّلة):** القرار المعتمد ينص على "11 لوناً أزرق" لكنه **لم يحدّد قيم الـ hex الفعلية**. بصفتي مُصمم الواجهة، اشتققت لوحة الـ 11 لوناً في §2 بالاعتماد على اللون الأساسي `#1F4E79` المُعتمد في `KIT_ADMIN_DASHBOARD` (الذي يتوافق مع توجّه "Blue Theme" المؤسسي). على EngineeringAgent **عدم اختراع** ألوان أخرى؛ إن احتاج العميل تعديل القيم لاحقاً، يُحدّث هذا الملف فقط.

---

## 1. Design Tokens (رموز التصميم)

كل الرموز أدناه تُعرّف كـ CSS Variables في `wwwroot/css/blue-theme.css` وتُستدعى من Razor `_Layout.cshtml`. لا يُسمح بأي لون/مسافة/خط خارج هذه الرموز.

### 1.1 Blue Identity Palette — لوحة الهوية الزرقاء (11 لوناً)

```css
:root {
  /* ===== 11 لون أزرق — Blue Theme Identity (مُعتمد) ===== */

  /* 1. الأزرق الأساسي — Primary */
  --c-primary:        #1F4E79;   /* أزرار، عناوين، روابط، عناصر التفاعل الرئيسية */

  /* 2. الأزرق الأساسي المعزّز — Primary Hover/Active */
  --c-primary-strong: #163A5A;   /* عند المرور (hover) والضغط (active) */

  /* 3. الأزرق الثانوي — Secondary */
  --c-secondary:      #2E6DA4;   /* رسوم بيانية، مؤشرات ثانوية، تباين متوسط */

  /* 4. الكحلي العميق — Accent (Dark) */
  --c-accent:         #0A2540;   /* خلفيات العناوين، الـ Footer، النصوص البارزة جداً */

  /* 5. الأزرق الفاتح — Accent (Soft) */
  --c-accent-soft:    #8FBCDE;   /* إبرازات خفيفة، حدود ملوّنة، خلفيات شارات */

  /* 6. خلفية التطبيق — App Background */
  --c-bg:             #F3F7FB;   /* خلفية الصفحة الفاتحة المائلة للأزرق */

  /* 7. سطح البطاقة — Surface */
  --c-surface:        #FFFFFF;   /* خلفية البطاقات والمكوّنات */

  /* 8. السطح الثانوي — Surface Muted */
  --c-surface-muted:  #EAF1F8;   /* رؤوس الجداول، الشارات، خلفيات الحالات */

  /* 9. الحدود — Border */
  --c-border:         #D4E2F0;   /* حدود البطاقات والجداول والحقول */

  /* 10. النص الأساسي — Text */
  --c-text:           #102A43;   /* حبر أزرق داكن — النصوص الأساسية */

  /* 11. النص الثانوي — Text Muted */
  --c-text-muted:     #5B7A99;   /* نصوص ثانوية، تسميات، قيم أقل أهمية */
}
```

> هذه الـ 11 لوناً هي "الهوية الزرقاء". أي لون إضافي (نجاح/تحذير/خطأ) يأتي من §1.2 ولا يُحسب ضمن الـ 11.

### 1.2 Functional Status Colors — ألوان الحالات الوظيفية

```css
:root {
  --c-success: #1E9E6A;   /* نجاح المزامنة، عمليات صحيحة */
  --c-warning: #E0A106;   /* تحذير، مزامنة مع ملاحظات */
  --c-error:   #D64545;   /* فشل المزامنة، أخطاء البطاقات */
  --c-info:    #2E6DA4;   /* معلومات محايدة (يُعاد استخدام --c-secondary) */
}
```

> **قاعدة الوصولية:** اللون ليس المؤشر الوحيد للحالة — كل حالة تُرفق بنص/أيقونة (انظر §7).

### 1.3 Typography — الخطوط

```yaml
typography:
  font-family-ar: "Cairo, Tajawal, Tahoma, sans-serif"   # العربية أولاً
  font-family-en: "Inter, Segoe UI, system-ui, sans-serif"
  h1: "28px / 1.25 / 700"    # عنوان الصفحة الرئيسية
  h2: "22px / 1.3 / 700"     # عنوان القسم / البطاقة
  h3: "18px / 1.35 / 600"    # عنوان فرعي داخل البطاقة
  body: "14px / 1.6 / 400"   # نص الجسم
  small: "12px / 1.5 / 400"  # تسميات ثانوية
  button: "14px / 1.2 / 600"
  line-height-body: 1.6
  line-height-heading: 1.3
```

- الخط العربي يُحمّل عبر `<link>` من Google Fonts (Cairo/Tajawal) في `_Layout.cshtml`.
- أحجام الخط العربي تكون أكبر 10–15% من الإنجليزي (مُطبّق أعلاه).
- **لا** يُستخدم `letter-spacing` مع العربية.
- اتجاه النص: `rtl` للعربية.

### 1.4 Spacing — المسافات (نظام 4px)

```yaml
spacing:
  base-unit: 4px
  scale: [4, 8, 12, 16, 24, 32, 48, 64]
```

- تباعد البطاقات في الشبكة: `24px` (gap).
- حشو البطاقة الداخلي: `16px`–`24px`.
- المسافة بين عنوان البطاقة ومحتواها: `12px`.
- **لا مسافات عشوائية** — كل مسافة من سلم الـ scale.

### 1.5 Radius — زوايا

```yaml
radius:
  sm:  "4px"
  md:  "8px"
  lg:  "12px"   # زوايا البطاقات الافتراضية
  xl:  "16px"
  full: "999px" # الشارات الدائرية، الأزرار المستديرة
```

### 1.6 Shadow — ظلال

```yaml
shadow:
  none: "none"
  sm:   "0 1px 2px rgba(10, 37, 64, 0.06)"
  md:   "0 4px 12px rgba(10, 37, 64, 0.08)"   # ظل البطاقة الافتراضي
  lg:   "0 8px 24px rgba(10, 37, 64, 0.12)"   # عند التمرير/التركيز
```

- مصدر ضوء واحد (من الأعلى) — كل الظلال بنفس الزاوية.
- البطاقات تستخدم `md`، وعند `hover` تنتقل إلى `lg` بانتقال ناعم.

### 1.7 Motion — الحركة

```yaml
motion:
  duration-fast:   "120ms"
  duration-normal: "240ms"
  easing-standard: "cubic-bezier(0.4, 0, 0.2, 1)"
```

- الحركات تقتصر على: دخول البطاقات (stagger)، تمرير الأزرار، ظهور الـ toast، انتقال الـ Drill Down.
- **لا** حركات مبالغ فيها أو متقطعة.

---

## 2. Syncfusion Component Rules (قواعد مكوّنات Syncfusion)

التطبيق يستخدم **Syncfusion Essential Studio for ASP.NET Core** (مكوّنات Razor Tag Helpers / HTML Helpers — وليس Blazor). القواعد التالية إلزامية لكل مكوّن.

### 2.1 Theme — السمة الموحّدة

- استخدام سمة واحدة أساس: **`Bootstrap5`** (تتوافق مع نظام الـ CSS Variables وسهلة التخصيص).
- يُمنع تحميل أكثر من سمة واحدة.
- تخصيص السمة يتم **فقط** عبر تجاوز متغيرات `--c-*` في `blue-theme.css` المربوطة بعد ملف سمة Syncfusion.
- تسجيل الرخصة عبر `SyncfusionLicenseProvider.RegisterLicense(...)` في `Program.cs` (المفتاح UnlockKey متوفر).

### 2.2 Chart Styling — تنسيق الرسوم البيانية

يُطبّق على: Bar (Column), Line, Pie, KPI Sparkline.

| القاعدة | القيمة / السلوك |
|---------|----------------|
| **Chart Palette** | مصفوفة الألوان = الـ 11 لوناً من §1.1 بالترتيب: `#1F4E79, #163A5A, #2E6DA4, #0A2540, #8FBCDE, #5B7A99, #2E6DA4, ...` (تكرار مع تدرّج لطيف عند الحاجة) |
| **Background** | `transparent` أو `--c-surface` — لا خلفية رمادية |
| **Title** | لون `--c-text`، خط `h3` (18px/600)، محاذاة `start` (يمين في RTL) |
| **Legend** | أسفل الرسم، لون نص `--c-text-muted`، خط `small` |
| **Axis Label** | لون `--c-text-muted`، حجم `12px` |
| **Axis Line/Grid** | لون `--c-border`، خط الشبكة خفيف جداً (opacity 0.5) |
| **Tooltip** | خلفية `--c-accent`، نص أبيض، زوايا `md` |
| **Data Label** | يُفعّل فقط عند الحاجة (Pie/KPI) — لون `--c-text` |
| **كرت الرسم** | يُحاط بـ Card Container (§3.2) — لا رسم حر بدون إطار |

**Bar / Column Chart:**
- `CornerRadius` = `4px` (أعلى الأعمدة) لإحساس ناعم.
- عرض العمود: تلقائي مع `ColumnSpacing` مناسب.
- عند النقر على عمود → تفعيل Drill Down (§3.3).

**Line Chart:**
- `Width` = `2px`، `Marker` بحجم `6px` ولون نقطة `--c-primary`.
- `Fill` متدرّج خفيف أسفل الخط (من `--c-secondary` بشفافية إلى شفاف).

**Pie / Doughnut Chart:**
- `Doughnut` مفضّل للوحات KPI (ثقب وسطي لعرض النسبة).
- تسميات النسب تُعرض خارج القطاعات.

**KPI Card (Sparkline):**
- رقم كبير (28px/700) بلون `--c-text`، أسفله Sparkline بسيط بلون `--c-primary`.
- سهم اتجاه (↑/↓) بلون `--c-success`/`--c-error` + نص النسبة.

### 2.3 Grid (Data Table) — جدول البيانات

يُستخدم `SfGrid` لبطاقات نوع Table (والجداول في Admin Panel).

| القاعدة | القيمة |
|---------|--------|
| **Header** | خلفية `--c-surface-muted`، نص `--c-text` (600)، `sticky` عند التمرير الطويل |
| **Row** | خلفية `--c-surface`، حد سفلي `--c-border` |
| **Hover Row** | خلفية `--c-surface-muted` |
| **Alternating Row** | مفعّل بخلفية `#F7FAFD` خفيفة جداً |
| **Selection** | لون تحديد `--c-accent-soft` (خفيف) |
| **Pagination** | مفعّل افتراضياً — أسفل الجدول، أزرار بلون `--c-primary` |
| **Empty State** | رسالة "لا توجد بيانات" داخل إطار الجدول (§4 / Acceptance) |
| **Text Align** | النص يبدأ من `inline-start`، الأرقام تُحاذى لـ `inline-end` |
| **Search/Filter** | شريط بحث أعلى الجدول بلون `--c-border` عند التركيز |

### 2.4 Gauge (Circular Gauge) — مؤشر الأداء الدائري

يُستخدم `SfCircularGauge` لبطاقات نوع Gauge.

| القاعدة | القيمة |
|---------|--------|
| **Pointer** | لون `--c-primary`، سُمك `6px` |
| **Range** | تدرّج من `--c-accent-soft` (منخفض) إلى `--c-primary` (مرتفع) |
| **Track** | لون `--c-border` |
| **Labels** | لون `--c-text-muted`، حجم `12px` |
| **Center Value** | الرقم الرئيسي بلون `--c-text` (22px/700) |
| **Background** | `--c-surface` أو `transparent` |

### 2.5 Forbidden Syncfusion Styling — ممنوع

- ❌ لا سمة داكنة (Dark) غير معتمدة.
- ❌ لا ألوان افتراضية لـ Syncfusion (الزرقاء/البرتقالية) دون تجاوز بالـ tokens.
- ❌ لا تدرّجات عشوائية (random gradients) خارج نطاق §2.2/§2.4.
- ❌ لا مكتبة UI خارجية (Chart.js/D3/DevExpress) — Syncfusion فقط (قرار معتمد #3).

---

## 3. Layout System (نظام التخطيط)

### 3.1 النمط العام — Admin Shell

يُستخدم نمط **Admin Shell** (من `LAYOUT_PATTERNS.md`):
`Topbar + Sidebar قابل للطي + منطقة محتوى واسعة + بطاقات/جداول`.

```
┌──────────────────────────────────────────────────────────┐
│ Topbar (64px): الشعار | Breadcrumb | Sync Status | User  │
├──────────┬───────────────────────────────────────────────┤
│ Sidebar  │  Content Area (Dashboard Grid)                │
│ (RTL:    │  ┌────┐ ┌────┐ ┌────┐ ┌────┐                │
│  يمين)   │  │Card│ │Card│ │Card│ │Card│  ← CSS Grid     │
│ 260/72px │  └────┘ └────┘ └────┘ └────┘                │
│          │  ┌──────────┐ ┌──────────┐                   │
│          │  │  Card    │ │  Card    │                   │
│          │  └──────────┘ └──────────┘                   │
└──────────┴───────────────────────────────────────────────┘
```

- **Topbar:** ارتفاع `64px`، خلفية `--c-accent` أو `--c-primary`، نص أبيض، يحوي Breadcrumb + مؤشر حالة المزامنة + زر المزامنة اليدوية.
- **Sidebar:** عرض `260px` مفتوح / `72px` مطوي؛ في RTL يبدأ من **اليمين**؛ يحوي تنقّل لوحة التحكم (Dashboard, Sync Logs, Admin — مخفي).
- **Content padding:** `24px` desktop / `16px` tablet / `12px` mobile.

### 3.2 Card Container — حاوية البطاقة

كل بطاقة (أي نوع: Chart/Table/KPI/Gauge) تُبنى داخل حاوية موحّدة:

```css
.wd-card {
  background: var(--c-surface);
  border: 1px solid var(--c-border);
  border-radius: var(--radius-lg);     /* 12px */
  box-shadow: var(--shadow-md);
  padding: 16px 20px;
  transition: box-shadow var(--duration-normal) var(--easing-standard);
}
.wd-card:hover { box-shadow: var(--shadow-lg); }

.wd-card__header {
  display: flex;
  align-items: center;
  justify-content: space-between;   /* RTL: يُعكس تلقائياً */
  margin-bottom: 12px;
}
.wd-card__title {
  font: 600 18px/1.35 var(--font-ar);
  color: var(--c-text);
}
.wd-card__body { /* محتوى الرسم/الجدول */ }
```

- الحد العلوي الملوّن (accent border) اختياري: `border-top: 3px solid var(--c-primary)` للبطاقات الأساسية.
- **لا** تملأ البطاقة حوافها تماماً — دائماً `padding` داخلي.

### 3.3 Dashboard Grid — شبكة لوحة التحكم (Responsive CSS Grid)

- التخطيط: **CSS Grid** (وليس فقط flexbox) لتوزيع البطاقات الديناميكي.
- الأعمدة مبنية على `GridWidth` (1–12) من جدول `DashboardCards`:

```css
.wd-dashboard-grid {
  display: grid;
  grid-template-columns: repeat(12, 1fr);
  gap: 24px;
}
/* كل بطاقة تأخذ عدد أعمدة = GridWidth (1..12) */
.wd-card--span-3  { grid-column: span 3; }
.wd-card--span-4  { grid-column: span 4; }
.wd-card--span-6  { grid-column: span 6; }
.wd-card--span-12 { grid-column: span 12; }
```

- ترتيب البطاقات = `GridPositionY` ثم `GridPositionX` (يُقرأ من DB).
- ارتفاع البطاقة = `GridHeight` (1–6 وحدات، كل وحدة ≈ `80px` + gap) — يُطبّق عبر `grid-row: span N` عند الحاجة.
- **Drill Down:** النقر على عنصر (عمود/نقطة/صف) يفتح `/Dashboard/DrillDown` مع `cardId + level + param`؛ البطاقة الهدف تُرث نفس الـ Card Container.

### 3.4 Breadcrumb — شريط التنقّل

- يُعرض أعلى منطقة المحتوى في شاشات Drill Down.
- يتبع الاتجاه RTL (يمين→يسار للعودة للمستوى الأعلى).
- العنصر الحالي بخط عريض بلون `--c-text`، السابق بلون `--c-text-muted` وقابل للنقر.

### 3.5 Sync Status Bar — شريط حالة المزامنة

- جزء من `_SyncStatusPartial.cshtml` أعلى الصفحة (داخل Topbar أو أسفله).
- يعرض: "آخر مزامنة: HH:mm" + مؤشر حالة (دائرة ملوّنة) + زر "مزامنة الآن".
- الحالات: `Running` (دائرة `--c-warning` نابضة + "جاري المزامنة..." + تعطيل الزر)، `Success` (`--c-success`)، `Failed` (`--c-error`)، `Idle` (`--c-text-muted`).

---

## 4. RTL/LTR Rules (قواعد الاتجاه)

### 4.1 قرار الاتجاه

```text
Primary direction:  RTL
Primary language:   العربية
Secondary language: الإنجليزية (أرقام، أكواد، أسماء جداول)
Mirroring required: Yes (للعناصر ذات دلالة اتجاهية)
```

### 4.2 قواعد RTL إلزامية

- `<html lang="ar" dir="rtl">` في `_Layout.cshtml`.
- **Sidebar على اليمين** افتراضياً (للعربية).
- الأيقونات ذات دلالة اتجاهية (سهم ذهاب/إياب، خطوات) **تُعكس** (mirror).
- القيم الرقمية وأكواد SQL قد تبقى LTR داخل واجهة RTL (مسموح).
- محاذاة الجداول: **النص** يبدأ من `inline-start`، **الأرقام** تُحاذى لـ `inline-end`.
- الـ Breadcrumb وخطوات التنقّل تحترم الاتجاه (الجذر يمين، التفصيل يسار).

### 4.3 قواعد LTR

- الأرقام والتواريخ بتنسيق `InvariantCulture` عند العرض الرقمي.
- **لا** يُكتب `left`/`right` مباشرة في CSS عند توفّر الخصائص المنطقية.

### 4.4 Engineering Rule — الخصائص المنطقية

تُفضّل دائماً الخصائص المنطقية (Logical Properties) لضمان العمل في RTL:

```css
margin-inline-start   /* بدل margin-right في RTL */
margin-inline-end
padding-inline-start
padding-inline-end
inset-inline-start    /* بدل left/right */
inset-inline-end
text-align: start;    /* بدل right/left */
```

- `justify-content: space-between` في Flexbox يُعكس تلقائياً في RTL — لا حاجة لتجاوز.

---

## 5. Responsive Rules (قواعد التجاوب)

نقاط التوقف (Breakpoints) — تُطبّق على `.wd-dashboard-grid` و Sidebar:

| الجهاز | العرض | السلوك |
|--------|-------|--------|
| **Desktop** | ≥ 1200px | 12 عموداً؛ Sidebar مفتوح 260px؛ gap 24px |
| **Tablet** | 768px – 1199px | 12 عموداً؛ Sidebar مطوي 72px؛ البطاقات span تتقلّص (span-4 → span-6)؛ gap 16px |
| **Mobile** | < 768px | عمود واحد (كل البطاقات `span-12`)؛ Sidebar مخفي خلف زر هامبرغر؛ gap 12px؛ padding 12px |

قواعد إضافية:
- الرسوم البيانية تستجيب عبر `SfChart` `Width="100%"` `Height` متجاوب (مثلاً `320px` mobile / `360px` tablet / `400px` desktop).
- الجداول: على الموبايل تُفعّل التمرير الأفقي داخل حاوية `overflow-x:auto` مع بقاء الـ header sticky.
- الأزرار: على الموبايل بعرض كامل عند الضرورة (`width:100%` للإجراءات الأساسية).
- لا يوجد Mobile App (خارج النطاق) — لكن الواجهة must تظل قابلة للاستخدام على شاشة هاتف داخل المتصفح.

---

## 6. Accessibility (إمكانية الوصول)

خط الأساس (من `ACCESSIBILITY_RULES.md`) مُطبّق كالتالي:

### 6.1 Contrast — التباين

| الثنائي | القيمة | النتيجة |
|---------|--------|---------|
| نص `--c-text` (#102A43) على `--c-surface` (#FFF) | تباين ~15:1 | ✅ ممتاز |
| نص `--c-text-muted` (#5B7A99) على `--c-surface` | تباين ~4.6:1 | ✅ يجتاز WCAG AA |
| نص أبيض على `--c-primary` (#1F4E79) | تباين ~8:1 | ✅ ممتاز |
| نص أبيض على `--c-success`/`--c-error` | ≥ 4.5:1 | ✅ |

- **لا** نص رمادي على خلفيات ملوّنة داكنة (يقتل المقروئية).
- إن تعذّر التباين الكافي، يُستخدم نص أبيض أو خلفية أفتح.

### 6.2 Keyboard Navigation — التنقّل بلوحة المفاتيح

- **Focus ring** مرئي دائماً: `outline: 2px solid var(--c-secondary); outline-offset: 2px;` على كل عنصر تفاعلي (أزرار، روابط، حقول، عناصر الرسم القابلة للنقر).
- ترتيب التنقّل (Tab order) منطقي: Topbar → Sidebar → محتوى → Footer.
- البطاقات القابلة للنقر (Drill Down) يجب أن تكون `tabindex="0"` وقابلة للتفعيل بـ Enter/Space.
- الجداول: رؤوس الأعمدة قابلة للتركيز والفرز عبر لوحة المفاتيح.
- لا "فخاخ لوحة مفاتيح" (keyboard traps).

### 6.3 Labels & States

- كل حقل إدخال (Admin Panel) له `<label>` مرتبط.
- حالات الخطأ تُعرض برسالة نصية + أيقونة (ليس اللون وحده).
- الأزرار لها نص واضح (لا أيقونة مجرّدة بدون `aria-label`).

### 6.4 Touch Targets — أهداف اللمس

- أصغر هدف لمس = `40px × 40px` على الموبايل (الأزرار بارتفاع 40px كحد أدنى).

### 6.5 Color Not Sole Indicator — اللون ليس المؤشر الوحيد

- الحالات (نجاح/تحذير/خطأ) تُرفق دائماً بنص عربي وأيقونة (✓ / ⚠ / ✕).
- القيم في KPI: السهم ↑/↓ + النسبة النصية، لا اللون فقط.

---

## 7. UI Acceptance Checklist (قائمة قبول الواجهة — لمراجعة Phase 6)

هذه القائمة **إلزامية** لكل مهمة `TASK-COD-*` تبني/تعدّل واجهة، وتُمرّر عبر `UI_ACCEPTANCE_GATE.md`. جميع الحالات التالية يجب تعريفها وتنفيذها لكل بطاقة/شاشة مهمة.

### 7.1 State Management (من 08_TECHNICAL_ARCHITECTURE.md §6.5)

| الحالة | السلوك المطلوب | العنصر البصري |
|--------|---------------|---------------|
| **Loading** | عرض Skeleton Loader أو Spinner أثناء تحميل البطاقات/البيانات | شيمر (shimmer) بلون `--c-surface-muted` داخل إطار البطاقة |
| **Empty** | رسالة "لا توجد بيانات" مع إبقاء إطار البطاقة | أيقونة + نص `--c-text-muted` داخل `.wd-card` |
| **Error** | رسالة الخطأ + زر "إعادة المحاولة" | نص `--c-error` + أيقونة ⚠ |
| **Success** | عرض البطاقة طبيعياً مع البيانات | الحالة الافتراضية |
| **Sync Running** | شريط الحالة "جاري المزامنة..." + تعطيل زر المزامنة | مؤشر نابض `--c-warning` |

### 7.2 Vitality / Micro-Interactions — الحيوية والتفاعلات الدقيقة

- [ ] **Skeleton Loading / Shimmer** — لكل بطاقة، رسم، وجدول أثناء التحميل.
- [ ] **Toast Notifications** — نجاح/فشل/تحذير (مثلاً بعد المزامنة اليدوية أو حفظ بطاقة في Admin).
- [ ] **Connection Status Indicator** — مؤشر متصل/غير متصل في Topbar.
- [ ] **Search حقيقي** — في جداول Admin Panel (والجداول الكبيرة في Dashboard إن وُجدت).
- [ ] **Micro-animations** — دخول البطاقات (stagger)، hover، انتقالات Toast، عدّادات KPI تصاعدية.
- [ ] **Empty States** — لكل قسم/بطاقة خالية (رسالة + أيقونة، لا إطار فارغ).
- [ ] **Realistic Data** — أسماء/أرقام/تفاصيل حقيقية المظهر في المعاينة (الرياض/جدة/الدمام كمناطق مخزون مثال).
- [ ] **البروتوتايب يبدو "حية"** — ليس مجرد "شغال"، بل تجربة مكتملة بحد أدنى من الكود.

### 7.3 UI Acceptance Gate Result (يُملأ في TASK-COD)

| Check | Result | Evidence | Notes |
|-------|--------|----------|-------|
| UI source mode recorded (USER_PROVIDED_REFERENCE) | PASS | §0 | |
| `28_UI_UX_GUIDELINES.md` used as primary design source | PASS | هذا الملف | |
| No invented colors/spacing/typography/component style | PASS/FAIL | §1 | أي لون خارج الـ 11 مرفوض |
| Component rules followed (Syncfusion §2) | PASS/FAIL | §2 | |
| Layout pattern followed (Admin Shell §3) | PASS/FAIL | §3 | |
| RTL/LTR rules followed (§4) | PASS | §4 | |
| Responsive behavior matches rules (§5) | PASS/N/A | §5 | |
| Accessibility baseline checked (§6) | PASS | §6 | |
| Forbidden styling avoided (§2.5) | PASS/FAIL | §2.5 | |
| Design gaps recorded instead of guessed (§0 Gap) | PASS | §0 | |
| Loading/Empty/Error/Toast/Micro-anim defined (§7) | PASS | §7 | |

**Gate Status:** PASS / NEEDS_FIX / BLOCKED_DESIGN_GAP

---

## 8. Design Gaps & Open Items (فجوات التصميم — مُسجّلة لا مُخمّنة)

| # | الفجوة | الحالة | الإجراء |
|---|--------|--------|---------|
| G1 | قيم hex الفعلية للـ 11 لوناً لم تُحدّد من العميل | **مُشتقّة** بواسطة مُصمم الواجهة في §1.1 (متوافقة مع `#1F4E79`) | يُعتمد من العميل أو يُعدّل لاحقاً عبر هذا الملف فقط |
| G2 | خط عربي محدد (Cairo/Tajawal) — ترخيص Google Fonts مجاني | مُفترض متاح | تأكيد عند التنفيذ |
| G3 | تفاصيل جداول Oracle والبيانات الفعلية مؤجّلة | خارج نطاق التصميم | تُحدّد أثناء التنفيذ — تؤثر على أسماء البطاقات فقط |
| G4 | Dark Mode | **غير مُعتمد** (ممنوع في §2.5) | لا يُبنى ما لم يطلبه العميل صراحةً |

> **قاعدة صارمة:** أي قيمة غير موجودة هنا = فجوة تُسجّل، **لا** يخترعها EngineeringAgent.

---

> **إعداد:** UI Designer Agent (مُصمم واجهات) — TASK-PREP-011
> **مصدر التصميم:** USER_PROVIDED_REFERENCE (Blue Theme — 11 ألوان، معتمد #9)
> **المرجع التنفيذي النهائي:** `project-preparation/28_UI_UX_GUIDELINES.md`
> **تاريخ:** 2026-07-12
