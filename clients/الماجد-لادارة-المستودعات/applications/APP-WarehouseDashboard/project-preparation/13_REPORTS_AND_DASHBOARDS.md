# 13_REPORTS_AND_DASHBOARDS.md — WarehouseDashboard

> **النوع:** Reports & Dashboards Definition Document (Technical / Data Perspective)
> **الحالة:** `Module Baseline Approved`
> **Baseline Module:** WarehouseDashboard
> **تاريخ الإعداد:** 2026-07-12
> **الجهة المعدة:** Software Designer Agent (مُصمم) — TASK-PREP-009
> **الملفات المرجعية:** `06_DATA_MODEL_PREPARATION.md` (MBA), `08_TECHNICAL_ARCHITECTURE.md` (MBA), `01_PROJECT_BRIEF.md`, `APPLICATION_BLUEPRINT.md`
> **ملاحظة هامة:** هذا المستند **وصفي/تعريفي فقط**. لا يحتوي على أي كود تنفيذي (C# / ADO.NET / Razor). استعلامات SQL المذكورة فيه هي **أمثلة توضيحية لشكل البيانات (Expected Data Shape)** التي يُدخلها Admin في حقل `SqlQuery` — وليست كوداً قابلاً للتسليم المباشر.

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Module Baseline Approved` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | N/A — ثابت طوال Phase 1 |
| **Document Type** | Reports & Dashboards Definition |
| **Version** | 1.0.0 |

---

## نبذة عن الوثيقة

توثق هذه الوثيقة **تعريفات التقارير واللوحات (Dashboards)** لتطبيق **WarehouseDashboard** من منظور **بياني/تقني** (Data & Technical Perspective). هي تبني على:

- `06_DATA_MODEL_PREPARATION.md` — خاصة جدول `DashboardCards` وجدول `CardDrillDownLevels`.
- `08_TECHNICAL_ARCHITECTURE.md` — خاصة §6 (Dashboard Architecture) و§6.4 (Drill Down).
- `APPLICATION_BLUEPRINT.md` — قائمة أنواع البطاقات المدعومة (Bar, Line, Pie, KPI, Table, Gauge).

الهدف: توفير مرجع موحّد يحدد **شكل البيانات (Data Shape)** المطلوب لكل نوع رسم بياني، **مخطط تكوين البطاقة**، آلية **Drill Down**، **تعريفات KPI**، وآلية **Data Binding** عبر ADO.NET — بحيث يستطيع Admin بناء أي تقرير/بطاقة ديناميكياً دون تدخل برمجي.

---

## القسم 1: أنواع الرسوم البيانية الستة (6 Chart Types)

كل بطاقة تحمل `ChartType` واحداً من القيم الست المسموحة (تُفرض عبر `CK_DashboardCards_ChartType`):
`Bar`, `Line`, `Pie`, `KPI`, `Table`, `Gauge`.

لكل نوع، يحدد الجدول التالي **متطلبات شكل البيانات (Data Shape Requirements)** — أي الأعمدة و الأنواع التي يجب أن يُرجعها `SqlQuery` لكي تعرض البطاقة بشكل صحيح.

### 1.1 Bar Chart (رسم الأعمدة / المقارنة بين الفئات)

| البند | التفاصيل |
|-------|----------|
| **الاستخدام** | مقارنة قيم بين فئات منفصلة (مثل: المخزون لكل منطقة) |
| **مكوّن Syncfusion** | Column / Bar Chart |
| **شكل البيانات المطلوب** | عمودان على الأقل: `Category` (نص) + `Value` (رقمي). يُسمح بأكثر من عمود قيمة (Series متعددة) |
| **الشكل المتوقع (مثال توضيحي)** | `Region` (nvarchar) ، `TotalQty` (decimal) |
| **ملاحظات** | يُفضّل ترتيب `Value` تنازلياً عبر `ORDER BY` في الاستعلام لعرض أفضل |

### 1.2 Line Chart (الرسم الخطي / الاتجاهات الزمنية)

| البند | التفاصيل |
|-------|----------|
| **الاستخدام** | عرض الاتجاه عبر الزمن (مثل: حركة الصادر والوارد شهرياً) |
| **مكوّن Syncfusion** | Line Chart |
| **شكل البيانات المطلوب** | عمود المحور الأفقي `X` (تاريخ/وقت أو فئة مرتبة) + عمود/أكثر قيمة رقمية `Y` (Series) |
| **الشكل المتوقع (مثال توضيحي)** | `Month` (datetime أو nvarchar مرتب) ، `InboundQty` (decimal) ، `OutboundQty` (decimal) |
| **ملاحظات** | يجب أن تكون قيم `X` مرتبة زمنياً تصاعدياً لرسم خط صحيح |

### 1.3 Pie Chart (الرسم الدائري / التوزيع النسبي)

| البند | التفاصيل |
|-------|----------|
| **الاستخدام** | إظهار النسبة المئوية لكل جزء من الكل (مثل: توزيع المخزون حسب الفئة) |
| **مكوّن Syncfusion** | Pie Chart (يمكن Doughnut) |
| **شكل البيانات المطلوب** | عمود `Label` (نص) + عمود `Value` (رقمي موجب) |
| **الشكل المتوقع (مثال توضيحي)** | `Category` (nvarchar) ، `Value` (decimal) |
| **ملاحظات** | القيم يجب أن تكون `>= 0`؛ النسب تُحسب تلقائياً من مجموع القيم |

### 1.4 KPI Card (بطاقة مؤشر رئيسي)

| البند | التفاصيل |
|-------|----------|
| **الاستخدام** | عرض رقم رئيسي واحد مع مؤشر اتجاه (صعود/هبوط) |
| **مكوّن Syncfusion** | Custom HTML + Syncfusion Sparkline (للاتجاه) |
| **شكل البيانات المطلوب** | صف واحد: `MetricName` (نص) + `CurrentValue` (رقمي). اختيارياً `PreviousValue` (رقمي) لحساب الاتجاه |
| **الشكل المتوقع (مثال توضيحي)** | `MetricName` (nvarchar) ، `CurrentValue` (decimal) ، `PreviousValue` (decimal, NULL مسموح) |
| **ملاحظات** | يُرجع عادةً صفاً واحداً؛ إن أرجع الاستعلام عدة صفوف تُستخدم أول قيمة أو `SUM` تجميعي |

### 1.5 Table (جدول بيانات تفصيلي)

| البند | التفاصيل |
|-------|----------|
| **الاستخدام** | عرض تفصيلي لصفوف بأي عدد من الأعمدة (مثل: قائمة المنتجات في مستودع) |
| **مكوّن Syncfusion** | HTML Table أو Syncfusion Grid (اختياري) |
| **شكل البيانات المطلوب** | **حر** — أي عدد من الأعمدة بأي أنواع (نص/رقم/تاريخ). لا يوجد شكل ثابت |
| **الشكل المتوقع (مثال توضيحي)** | `ProductCode` , `ProductName` , `WarehouseName` , `Qty` , `LastUpdated` |
| **ملاحظات** | يدعم التصفية والبحث (انظر القسم 6). يُنصح بتقييد عدد الصفوف عبر `TOP` أو ترقيم |

### 1.6 Gauge (مؤشر الأداء)

| البند | التفاصيل |
|-------|----------|
| **الاستخدام** | عرض قيمة ضمن نطاق (مثل: نسبة الإشغال أو معدل المزامنة) |
| **مكوّن Syncfusion** | Circular Gauge |
| **شكل البيانات المطلوب** | صف واحد: `Metric` (نص) + `Value` (رقمي) + `Min` (رقمي) + `Max` (رقمي). اختيارياً `Target` (رقمي) |
| **الشكل المتوقع (مثال توضيحي)** | `Metric` (nvarchar) ، `Value` (decimal) ، `Min` (decimal) ، `Max` (decimal) ، `Target` (decimal, NULL مسموح) |
| **ملاحظات** | `Value` يجب أن يقع ضمن `[Min, Max]` لعرض صحيح للمؤشر |

**جدول ملخص متطلبات الشكل:**

| ChartType | الأعمدة الإلزامية | الأنواع المفضّلة | عدد الصفوف المتوقع |
|-----------|-------------------|-----------------|---------------------|
| Bar | Category, Value | nvarchar, decimal | ≥ 1 |
| Line | X, Y (≥1) | datetime/nvarchar, decimal | ≥ 2 |
| Pie | Label, Value | nvarchar, decimal | ≥ 1 |
| KPI | CurrentValue (+ اختياري PreviousValue) | decimal | 1 |
| Table | حر | أي | أي |
| Gauge | Value, Min, Max | decimal | 1 |

---

## القسم 2: مخطط تكوين البطاقة (Card Configuration Schema)

يربط هذا المخطط كل حقل في كيان `DashboardCard` (من `06_DATA_MODEL_PREPARATION.md` §1.1) بدوره في تعريف التقرير/البطاقة. البطاقة تُحفظ في جدول `DashboardCards` وتُقرأ وقت التشغيل لبناء Dashboard ديناميكياً.

| حقل الجدول | النوع في SQL | دوره في تكوين البطاقة (Reports/Dashboards) | ملاحظات وحوكمة |
|------------|--------------|---------------------------------------------|----------------|
| `Id` | int (PK) | المعرّف الفريد للبطاقة — يُستخدم في Drill Down و API | Auto-increment |
| `Title` | nvarchar(200) | عنوان البطاقة المعروض في الـ Header | نص عربي/إنجليزي |
| `ChartType` | nvarchar(50) | **نوع العرض** — يحدد مكوّن Syncfusion المستخدم | مقيد بـ `CK_DashboardCards_ChartType` (الأنواع الستة) |
| `SqlQuery` | nvarchar(max) | **مصدر البيانات** — استعلام SQL أو اسم View يُنفَّذ عبر ADO.NET | ينفَّذ وقت العرض (انظر القسم 5) |
| `DataSourceType` | nvarchar(50) | `'SQL Query'` أو `'View'` — يوضح طبيعة `SqlQuery` | مقيد بـ `CK_DashboardCards_DataSourceType` |
| `GridPositionX` | int | موضع البطاقة أفقياً في الشبكة (يبدأ 0) | يُرتب مع `Y` عبر `IX_DashboardCards_GridPositionX_GridPositionY` |
| `GridPositionY` | int | موضع البطاقة عمودياً في الشبكة (يبدأ 0) | |
| `GridWidth` | int | عرض البطاقة (1–12) يمثل أعمدة الشبكة | مقيد بـ `CK_DashboardCards_GridWidth` |
| `GridHeight` | int | ارتفاع البطاقة (1–6) يمثل صفوف الشبكة | مقيد بـ `CK_DashboardCards_GridHeight` |
| `RefreshInterval` | int | فترة التحديث التلقائي بالثواني (`0` = لا تحديث) | مقيد بـ `CK_DashboardCards_RefreshInterval` |
| `IsActive` | bit | `1` = معروضة في Dashboard، `0` = مخفية | تُرشَّح عبر `WHERE IsActive = 1` |
| `CreatedAt` | datetime2 | وقت الإنشاء | تلقائي |
| `UpdatedAt` | datetime2 | وقت آخر تعديل | تلقائي |

**قواعد التكوين (من `08_TECHNICAL_ARCHITECTURE.md` §7.4):**
- `ChartType` يجب أن يتطابق مع شكل البيانات المُرجَع من `SqlQuery` (القسم 1).
- `GridWidth` + `GridPositionX` يجب ألا يتجاوزا 12 (عرض الشبكة الكلي).
- `RefreshInterval` قيمة اختيارية؛ التحديث يتم عبر إعادة تنفيذ `SqlQuery` في الخلفية.

---

## القسم 3: تعريفات Drill Down (متعدد المستويات)

يُعرّف Drill Down في جدول `CardDrillDownLevels` (من `06_DATA_MODEL_PREPARATION.md` §1.2) — كل صف يمثّل مستوى تعمّق واحد مرتبط ببطاقة رئيسية عبر `ParentCardId`.

### 3.1 المبدأ متعدد المستويات (Multi-Level Approach)

```
Level 0  →  البطاقة الرئيسية (DashboardCards.ChartType)
   │        عند النقر على عنصر (data-point)
   ▼
Level 1  →  CardDrillDownLevels WHERE ParentCardId = X AND Level = 1
   │        (TargetChartType + DrillDownQuery بمعامل)
   │        عند النقر على عنصر داخله
   ▼
Level 2  →  CardDrillDownLevels WHERE ParentCardId = X AND Level = 2
   │
   ▼  (يتكرر حسب عدد المستويات المعرّفة)
```

- كل مستوى له `Level` (ترتيب يبدأ من 1) — مضمون عدم التكرار عبر `IX_CardDrillDownLevels_ParentCardId_Level` (Unique).
- `TargetChartType` يحدد نوع العرض في هذا المستوى (أي من الأنواع الستة).
- عند حذف البطاقة الأم تُحذف كل مستوياتها تلقائياً (`ON DELETE CASCADE`).

### 3.2 تمرير المعاملات (Parameter Passing)

آلية تمرير القيمة المختارة من مستوى لأدنى:

1. **التقاط القيمة:** عند نقر المستخدم على عنصر في البطاقة (مثلاً عمود "الرياض" في Bar Chart)، تُلتقط قيمة ذلك العنصر (مثل `Region = 'الرياض'`).
2. **تمرير المعامل:** تُمرَّر القيمة إلى صفحة Drill Down عبر الرابط:
   `/Dashboard/DrillDown?cardId={ParentCardId}&level={N}&param={Value}`
   (حسب `08_TECHNICAL_ARCHITECTURE.md` §6.2).
3. **تنفيذ الاستعلام:** `DrillDownQuery` في المستوى `N` يُنفَّذ عبر ADO.NET باستخدام المعامل — مثلاً استعلام يقبل `@Region`.
4. **السلسلة (Chaining):** كل مستوى أعمق يأخذ معاملات المستويات السابقة + معامله الجديد، فيسمح بالتعاكس عبر عدة مستويات (Region → Warehouse → Product).
5. **Breadcrumb:** شريط `_BreadcrumbPartial.cshtml` يعرض مسار المستويات ويسمح بالعودة لأي مستوى سابق.

**قواعد المعاملات:**
- `DrillDownQuery` **يجب أن يقبل معامل واحد على الأقل** (حسب `06_DATA_MODEL_PREPARATION.md` §1.2).
- تنفيذ المعامل عبر `SqlCommand` مع `SqlParameter` (ADO.NET) — يمنع SQL Injection.
- أسماء المعاملات متفق عليها بين مستويات البطاقة (مثل `@Region`, `@WarehouseName`, `@ProductId`).

---

## القسم 4: تعريفات KPI (صيغ وحسابات)

بطاقة `KPI` تعرض قيمة مجمّعة واحدة. الصيغ الحسابية تُطبَّق داخل `SqlQuery` (عبر `SUM`, `COUNT`, `AVG`) بحيث يُرجع الاستعلام القيمة الجاهزة — لا حساب في الواجهة.

| الصيغة | الوصف | شكل البيانات المطلوب من الاستعلام | مثال توضيحي للغرض |
|--------|-------|-----------------------------------|---------------------|
| `SUM` | مجموع قيمة عددية | `CurrentValue` = ناتج `SUM(Column)` | إجمالي المخزون: `SUM(TotalQty)` |
| `COUNT` | عدد الصفوف/السجلات | `CurrentValue` = ناتج `COUNT(*)` أو `COUNT(DISTINCT Col)` | عدد المستودعات النشطة: `COUNT(*)` |
| `AVG` | المتوسط الحسابي | `CurrentValue` = ناتج `AVG(Column)` | متوسط كمية المنتج: `AVG(Qty)` |

**اتجاه KPI (Trend):** لعرض مؤشر صعود/هبوط، يُرجع الاستعلام عمودين:
- `CurrentValue` = القيمة الحالية (مثلاً مخزون هذا الشهر).
- `PreviousValue` = القيمة السابقة (مثلاً مخزون الشهر الماضي) — اختياري.
تُحسب نسبة التغير في الواجهة: `(CurrentValue - PreviousValue) / PreviousValue`.

**ملاحظة:** الحسابات المعقدة (نسب مئوية، تراكمات) تُنجز في `SqlQuery` نفسه؛ البطاقة تعرض النتيجة فقط.

---

## القسم 5: ربط البيانات (Data Binding) عبر ADO.NET

آلية تنفيذ `SqlQuery` / `View` وربط النتيجة بالبطاقة، مبنية على `06_DATA_MODEL_PREPARATION.md` §5.4 و `08_TECHNICAL_ARCHITECTURE.md` §6.1.

### 5.1 تدفق الربط (Binding Flow)

```
1. Page Load (Dashboard / DrillDown)
       │
       ▼
2. DashboardService.GetCards()  [يقرأ التكوين عبر EF Core]
       └── SELECT * FROM DashboardCards WHERE IsActive = 1
       │
       ▼
3. لكل بطاقة نشطة:
       a. تنفيذ SqlQuery عبر ADO.NET (SqlCommand + SqlDataReader)
          موجَّه ضد SQL Server — قد يقرأ من Data Tables
       b. تخزين النتيجة في CardViewModel (DataTable + ChartType + Position)
       │
       ▼
4. تمرير List<CardViewModel> إلى Razor Page
       │
       ▼
5. Razor يعرض كل بطاقة بمكوّن Syncfusion حسب ChartType
```

### 5.2 قواعد الربط (من حوكمة `06_DATA_MODEL_PREPARATION.md` §5)

| القاعدة | التفاصيل |
|---------|----------|
| **Config → EF Core** | قراءة `DashboardCards` و `CardDrillDownLevels` عبر EF Core DbContext |
| **Data Query → ADO.NET** | تنفيذ `SqlQuery` عبر `SqlCommand`/`SqlDataReader` مباشرة (لا EF Core) — القاعدة G6 |
| **لا كتابة (Write)** | الاستعلام Query فقط (SELECT) — لا INSERT/UPDATE/DELETE من Dashboard |
| **نفس SQL Server** | كل التنفيذ ضد قاعدة `WarehouseDashboard` (Config + Data Tables معاً) |
| **الأمان** | أي معامل في Drill Down يُمرَّر عبر `SqlParameter` (منع Injection) |
| **مخطط الألوان** | جميع المكونات تطبّق الهوية الزرقاء (11 لوناً — القيد C3) |

### 5.3 شكل البيانات الناتج (Result Shape)

بعد تنفيذ `SqlQuery`، تُربط الأعمدة الناتجة بالمكوّن كالتالي (مثال توضيحي لشكل البيانات):

- Bar/Line: العمود الأول = المحور الفئوي/الزمني، بقية الأعمدة الرقمية = Series.
- Pie: العمود الأول = Label، العمود الثاني = Value.
- KPI: الصف الأول، العمود المسمى `CurrentValue` (واختيارياً `PreviousValue`).
- Table: الأعمدة تُعرض كما هي بترتيب الظهور.
- Gauge: الصف الأول يحتوي `Value`, `Min`, `Max` (واختيارياً `Target`).

---

## القسم 6: التصفية والبحث (Filtering & Search) في الجداول

بطاقة `Table` هي الوحيدة التي تتطلب تصفية/بحث صريح ضمن البطاقة.

### 6.1 آليتان للتصفية

| الآلية | الوصف | متى تُستخدم |
|--------|-------|--------------|
| **تصفية من جهة الخادم (Server-Side)** | Admin يضمّن `WHERE` / `LIKE` داخل `SqlQuery` نفسه | عند الرغبة في تقليل حجم البيانات المُرسَلة (مثلاً `WHERE Region = 'الرياض'`) |
| **تصفية من جهة العميل (Client-Side)** | صندوق بحث (Search Box) في رأس البطاقة يُرشّح الصفوف المعروضة من النتيجة المحمّلة مسبقاً | للنتائج الصغيرة/المتوسطة — بحث فوري دون إعادة تنفيذ الاستعلام |

### 6.2 متطلبات شكل البيانات للبحث

- التصفية النصية تعتمد على أعمدة نصية (`nvarchar`) — يُفضّل أن تكون قابلة للبحث (مثل `ProductName`, `WarehouseName`).
- التصفية الرقمية/التاريخية تعتمد على أعمدة `decimal` / `datetime2` — تدعم نطاقات (من–إلى).
- يُنصح بتقييد عدد الصفوف في `SqlQuery` (عبر `TOP` أو ترقيم) لتفادي تحميل جداول ضخمة في الواجهة.

### 6.3 السلوك حسب State Management (`08_TECHNICAL_ARCHITECTURE.md` §6.5)

| الحالة | سلوك البحث/التصفية |
|--------|---------------------|
| Loading | يُعطّل صندوق البحث حتى اكتمال التحميل |
| Empty | يظهر "لا توجد بيانات" — لا نتائج للبحث |
| Error | يظهر خطأ البطاقة — لا بحث |
| Success | صندوق البحث فعّال + نتائج التصفية اللحظية |

---

## القسم 7: مثال واقعي (Realistic Example) — مخزون المستودعات حسب المنطقة

مثال شامل يربط كل الأقسام السابقة: بطاقة رئيسية تعرض المخزون حسب المنطقة، مع Drill Down متعدد المستويات.

### 7.1 المستوى 0 — البطاقة الرئيسية (Bar Chart)

- **Title:** `المستودعات حسب المنطقة`
- **ChartType:** `Bar`
- **DataSourceType:** `SQL Query`
- **شكل البيانات المطلوب:** `Region` (nvarchar) + `TotalQty` (decimal)
- **الشكل المتوقع للنتيجة (مثال توضيحي):**

| Region | TotalQty |
|--------|----------|
| الرياض | 15,234 |
| جدة | 12,876 |
| الدمام | 8,432 |
| مكة | 5,119 |

### 7.2 المستوى 1 — Drill Down (Pie Chart) عند النقر على "الرياض"

- **ParentCardId:** معرّف البطاقة الرئيسية
- **Level:** `1`
- **DisplayName:** `تفاصيل مستودعات الرياض`
- **TargetChartType:** `Pie`
- **المعامل المُمرَّر:** `@Region = 'الرياض'`
- **شكل البيانات المطلوب:** `WarehouseName` (nvarchar) + `TotalQty` (decimal)
- **الشكل المتوقع للنتيجة:**

| WarehouseName | TotalQty |
|---------------|----------|
| مستودع الشمال | 7,500 |
| مستودع الجنوب | 4,210 |
| مستودع المركز | 3,524 |

### 7.3 المستوى 2 — Drill Down (Table) عند النقر على مستودع

- **Level:** `2`
- **DisplayName:** `المنتجات في المستودع`
- **TargetChartType:** `Table`
- **المعاملات:** `@Region = 'الرياض'` + `@WarehouseName = 'مستودع الشمال'`
- **شكل البيانات المطلوب:** حر — مثلاً `ProductCode`, `ProductName`, `Qty`, `LastUpdated`
- **الشكل المتوقع للنتيجة:**

| ProductCode | ProductName | Qty | LastUpdated |
|-------------|-------------|-----|-------------|
| P-001 | زيت محرك | 1,200 | 2026-07-12 |
| P-002 | إطارات | 850 | 2026-07-11 |
| P-003 | فلاتر | 430 | 2026-07-12 |

> يدعم هذا الجدول **البحث/التصفية** (القسم 6) — مثلاً البحث عن "زيت".

### 7.4 المستوى 3 (اختياري) — Drill Down (Line Chart) عند النقر على منتج

- **Level:** `3`
- **DisplayName:** `حركة المنتج شهرياً`
- **TargetChartType:** `Line`
- **المعاملات:** `@Region` + `@WarehouseName` + `@ProductId`
- **شكل البيانات المطلوب:** `Month` (datetime/nvarchar مرتب) + `InboundQty` (decimal) + `OutboundQty` (decimal)
- **الشكل المتوقع للنتيجة:**

| Month | InboundQty | OutboundQty |
|-------|-----------|-------------|
| 2026-01 | 320 | 290 |
| 2026-02 | 410 | 360 |
| 2026-03 | 380 | 400 |

### 7.5 ربط المثال بتدفق النظام

```
DashboardCards (Bar: المخزون حسب المنطقة)
      │ نقر "الرياض"
      ▼
CardDrillDownLevels L1 (Pie: @Region)
      │ نقر "مستودع الشمال"
      ▼
CardDrillDownLevels L2 (Table: @Region + @WarehouseName)
      │ نقر "زيت محرك"
      ▼
CardDrillDownLevels L3 (Line: @Region + @WarehouseName + @ProductId)
```

كل مستوى يُنفَّذ عبر ADO.NET بـ `SqlParameter`، ويربط Breadcrumb المسار: `المنطقة: الرياض › المستودع: الشمال › المنتج: زيت محرك`.

---

## القسم 8: ملخص الامتثال (Compliance Checklist)

| البند | الحالة | المرجع |
|-------|:------:|--------|
| الأنواع الستة مغطاة بشكل بيانات | ✅ | القسم 1 |
| مخطط تكوين البطاقة مربوط بـ DashboardCards | ✅ | القسم 2 |
| Drill Down متعدد المستويات + تمرير معاملات | ✅ | القسم 3 |
| تعريفات KPI (SUM/COUNT/AVG) | ✅ | القسم 4 |
| Data Binding عبر ADO.NET موثّق | ✅ | القسم 5 |
| التصفية والبحث في الجداول | ✅ | القسم 6 |
| مثال واقعي (مخزون حسب المنطقة) | ✅ | القسم 7 |
| Lifecycle Header | ✅ | الحالة: Module Baseline Approved |

---

## القسم 9: فجوات التصميم (Design Gaps / Issues)

| # | الفجوة | التأثير | الحل المقترح |
|:-:|--------|:-------:|-------------|
| 1 | `01_PROJECT_BRIEF.md` و `APPLICATION_BLUEPRINT.md` **لا يحتويان على Lifecycle Header** بالصيغة المعتمدة (§4.1 من تعريف الوكيل) — حالتهما `preparation` / `approved_for_preparation` أقل من `MBA` | متوسط — استُخدما للسياق فقط؛ المصادر الموثوقة (06, 08) عند `MBA` | يُنصح بإضافة Lifecycle Header رسمي لهذين الملفين أو رفعهما إلى `MBA` لضمان حوكمة القراءة مستقبلاً |
| 2 | هيكل Data Tables (الجداول المنقولة من Oracle) **مؤجّل** (06 §3.4) — أسماء الأعمدة الحقيقية غير معروفة بعد | متوسط — الأمثلة في هذا المستند توضيحية بأسماء افتراضية (`Region`, `TotalQty`, ...) | تُحدَّد أثناء التنفيذ حسب جداول Oracle الفعلية؛ لا تأثير على شكل البيانات المطلوب |
| 3 | لا توجد اتفاقية موحّدة لأسماء المعاملات في Drill Down (`@Region` vs `@RegionName`) | منخفض | يُحددها Admin وقت التكوين — يُنصح بـ Convention موثّق في Query Tester |

---

> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-009
> **تاريخ:** 2026-07-12
> **الحالة:** `Module Baseline Approved` ✅
> **المرجع المعتمد:** `06_DATA_MODEL_PREPARATION.md` (MBA) ، `08_TECHNICAL_ARCHITECTURE.md` (MBA)
