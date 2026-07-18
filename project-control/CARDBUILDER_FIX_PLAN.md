# خطة الإصلاح الشاملة — Card Builder Wizard (ما بعد التعديلات الهيكلية)

> **تاريخ:** 2026-07-18
> **المصدر:** تقرير Auditor (`QUAUD-CARDBUILDER-001-2026-07-18-001.md`) + الدراسة الأولية لـ TeraAgent
> **الحالة:** بانتظار الاعتماد

---

## خلاصة الموقف

| البند | القيمة |
|---|---|
| **المشاكل المكتشفة** | 4 STOP (مانعة), 6 CAUTION, 5 FLAG |
| **السبب المباشر لعدم الحفظ** | F-001: عدم وجود Antiforgery Token في النموذج |
| **سبب تعطل المعاينة** | F-002/F-003: `card-builder.js` لا يزال يستخدم Syncfusion `ej.*` رغم إزالته |
| **مخاطر إضافية** | F-004: SQL Injection في KPI queries · F-008: خطأ في نوع مصدر SqlTable |

---

## هيكل الإصلاح — المجموعات والأولويات

```
Priority 1 (Blockers)      Priority 2 (Data Integrity)     Priority 3 (UX/Data Loss)     Priority 4 (Housekeeping)
┌─────────────────┐        ┌──────────────────────┐       ┌──────────────────────┐       ┌──────────────────────┐
│ F-001: Antiforgery │       │ F-004: SQL Injection  │      │ F-005: displayName    │      │ F-009: Dead Code     │
│ F-002/003: Chart.js│       │ F-008: DataSourceType │      │ F-006: GridX/Y int    │      │ F-011: Clone API     │
│ F-012: renderChart │       │                       │      │ F-010: description    │      │ F-014: CSRF Risk     │
│                     │       │                       │      │ F-007: name cleanup   │      │ F-013: Validation    │
│                     │       │                       │      │ F-015: errors         │      │                      │
└─────────────────┘        └──────────────────────┘       └──────────────────────┘       └──────────────────────┘
```

---

## Priority 1 — Blockers (الحفظ والمعاينة)

### TASK-COD-FIX-021 — Fix Antiforgery Token + Save Pipeline
| الحقل | القيمة |
|---|---|
| **المرجع** | F-001 (STOP) |
| **سبب الحظر** | عدم وجود `@Html.AntiForgeryToken()` في النموذج ← الخادم يرفض POST بخطأ 400 |
| **الملفات** | `Builder.cshtml`, `Builder.cshtml.cs` |
| **الإجراء** | 1. إضافة `@Html.AntiForgeryToken()` داخل `<form>` في `Builder.cshtml` ← هذا الحل الوحيد المطلوب لـ F-001 |
| **الأثر** | بعد هذا الإصلاح، الحفظ سيعمل (ما لم يكن هناك خطأ في ربط الحقول) |
| **المسؤول** | EngineeringAgent |

### TASK-COD-FIX-022 — Replace Syncfusion with Chart.js in card-builder.js
| الحقل | القيمة |
|---|---|
| **المرجع** | F-002 (STOP), F-003 (STOP), F-012 (FLAG) |
| **سبب الحظر** | `renderChart()` يستخدم `global.ej.grids.Grid` و `global.ej.charts.Chart` (Syncfusion) لكن Syncfusion غير محمّل |
| **الملفات** | `card-builder.js`, `CardBuilderService.cs` (BuildChartConfig), `_CardsLayout.cshtml` |
| **الإجراء** | 1. إعادة كتابة `renderChart()` في `card-builder.js` لاستخدام Chart.js API بدلاً من Syncfusion: `new Chart(ctx, {...})`<br>2. إعادة كتابة `BuildChartConfig()` في `CardBuilderService.cs` لإرجاع تنسيق Chart.js بدلاً من Syncfusion (أو جعلها ترجع columns+rows فقط ويتولى JS الرسم)<br>3. التأكد من أن Chart.js محمّل في `_CardsLayout.cshtml` (موجود حالياً ✅)<br>4. تنفيذ render للأنواع: Bar, Line, Pie (Chart.js native), Gauge (دونات Chart.js), Table (HTML table عادي), KPI (يحتاج معالجة خاصة) |
| **الأثر** | بعد الإصلاح، المعاينة ستعمل مباشرة في الـ Builder |
| **المسؤول** | UI Designer (للجزء الأمامي) + EngineeringAgent (للجزء الخلفي) |

---

## Priority 2 — Data Integrity (سلامة البيانات)

### TASK-COD-FIX-023 — Fix SQL Injection in KPI Queries
| الحقل | القيمة |
|---|---|
| **المرجع** | F-004 (STOP) |
| **خطورة** | SQL Injection — إدخال أسماء الأعمدة مباشرة في الاستعلام بدون معالجة |
| **الملفات** | `KpiQueryBuilder.cs` |
| **الإجراء** | 1. استخدام `SqlParameter` لأسماء الأعمدة (مع تحقق من وجود العمود)<br>2. أو تحقق من أن اسم العمود مطابق لأحد الأعمدة المعروفة في الجدول<br>3. يجب أن لا يُسمح لأي إدخال مستخدم بالوصول مباشرة إلى SQL |
| **المسؤول** | EngineeringAgent |

### TASK-COD-FIX-024 — Fix DataSourceType for SqlTable Source
| الحقل | القيمة |
|---|---|
| **المرجع** | F-008 (CAUTION) |
| **المشكلة** | `Builder.cshtml.cs` سطر 322: `DataSourceType = dto.SourceType == "SqlTable" ? "View" : "SQL Query"` — يخبّئ الجدول كـ View ويتوقّع `SELECT * FROM [ViewName]`، لكن `DashboardService.BuildSql()` عند `DataSourceType = "View"` يضيف `SELECT * FROM [...]` حول القيمة، مما يسبب خطأ SQL |
| **الملفات** | `Builder.cshtml.cs` (سطر 322) |
| **الإجراء** | تغيير `DataSourceType` ليكون دائماً `"SQL Query"` وحفظ الـ SqlQuery كاملاً `SELECT * FROM [table]` — أو تغيير تعامل BuildSql مع الـ View |
| **المسؤول** | EngineeringAgent |

---

## Priority 3 — UX and Data Loss Prevention

### TASK-COD-FIX-025 — Fix Form Field Binding & Data Loss
| الحقل | القيمة |
|---|---|
| **المرجع** | F-005 (CAUTION), F-006 (CAUTION), F-010 (CAUTION) |
| **المشاكل** | F-005: حقل `displayName` لا يحتوي `value="@Model.DisplayName"` ← يفقد القيمة عند خطأ التحقق<br>F-006: `GridX` و `GridY` من نوع `int` (int non-nullable) لكن القيمة الافتراضية `-1` تتصارع مع الإرسال الفارغ<br>F-010: حقل `description` يُرسل لكن يُتجاهل في الخادم |
| **الملفات** | `Builder.cshtml`, `Builder.cshtml.cs`, `DashboardCard.cs` |
| **الإجراء** | 1. إضافة `value="@Model.DisplayName"` إلى حقل العرض<br>2. جعل `GridX` و `GridY` من نوع `int?` (nullable) أو معالجة القيم بشكل صحيح<br>3. إضافة خاصية `Description` إلى `DashboardCard` و `DashboardCardDto` وحفظها في DB |
| **المسؤول** | EngineeringAgent |

### TASK-COD-FIX-026 — Fix Form Cleanup & Validation
| الحقل | القيمة |
|---|---|
| **المرجع** | F-007 (CAUTION), F-015 (FLAG) |
| **المشاكل** | F-007: `wb-source-type` غير مضمن في `cleanupDuplicateNames()` ← يمكن أن يسبب تعارض في_POST<br>F-015: `novalidate` على الفورم + عناصر `<span class="wd-error" data-for="...">` بدون محتوى ← لا تظهر رسائل الخطأ للمستخدم |
| **الملفات** | `card-builder.js` (دالة cleanupDuplicateNames), `Builder.cshtml` (عناصر wd-error) |
| **الإجراء** | 1. إضافة `'wb-source-type'` و `'wb-source-id'` إلى قائمة `cleanupDuplicateNames`<br>2. إما إزالة `novalidate` من الفورم أو ربط عناصر `wd-error` بعرض رسائل التحقق من الخادم |
| **المسؤول** | EngineeringAgent |

---

## Priority 4 — Housekeeping & Hardening

### TASK-COD-FIX-027 — Remove Dead Code + Fix Clone API
| الحقل | القيمة |
|---|---|
| **المرجع** | F-009 (CAUTION), F-011 (FLAG) |
| **المشاكل** | F-009: 3 دوال ميتة في `Builder.cshtml.cs`: `OnPostPreviewAsync`, `BuildCardFromRequest`, `RenderPreview`<br>F-011: API الاستنساخ `/api/dashboard/cardbuilder/clone/{id}` غير موجود (404) |
| **الملفات** | `Builder.cshtml.cs`, `CardBuilder.cshtml.cs` |
| **الإجراء** | 1. إزالة الدوال الميتة من `Builder.cshtml.cs`<br>2. إضافة endpoint الاستنساخ في `CardBuilder.cshtml.cs` (OnGetClone) |
| **المسؤول** | EngineeringAgent |

### TASK-COD-FIX-028 — Security Hardening: Antiforgery on Preview API
| الحقل | القيمة |
|---|---|
| **المرجع** | F-014 (FLAG) |
| **المشكلة** | `[IgnoreAntiforgeryToken]` على `CardBuilder.cshtml.cs` ← أي موقع خارجي يستطيع استدعاء Preview API |
| **الملف** | `CardBuilder.cshtml.cs` |
| **الإجراء** | إزالة `[IgnoreAntiforgeryToken]` وإضافة إرسال الـ token مع طلبات AJAX في `card-builder.js` عبر `headers: { 'RequestVerificationToken': ... }` |
| **المسؤول** | EngineeringAgent |

### TASK-COD-FIX-029 — Client-side Range Validation on Grid Inputs
| الحقل | القيمة |
|---|---|
| **المرجع** | F-013 (FLAG) |
| **المشكلة** | حقول Grid (width=1-12, height=1-6) لا يوجد تحقق client-side |
| **الملفات** | `card-builder.js` |
| **الإجراء** | إضافة تحقق client-side عند الإدخال مع منع القيم خارج النطاق |
| **المسؤول** | EngineeringAgent |

---

## التسلسل الزمني المقترح للتنفيذ

```text
الأسبوع 1 (Blockers):
  └─ TASK-COD-FIX-021 (Antiforgery)     ← سطر واحد، دقائق
  └─ TASK-COD-FIX-022 (Chart.js)        ← 1-2 يوم (الأكبر)
  └─ TASK-COD-FIX-023 (SQL Injection)   ← 2-3 ساعات

الأسبوع 2 (Data Integrity + UX):
  └─ TASK-COD-FIX-024 (DataSourceType)  ← ساعة
  └─ TASK-COD-FIX-025 (Form Binding)    ← 2-3 ساعات
  └─ TASK-COD-FIX-026 (Form Cleanup)    ← ساعة

الأسبوع 3 (Housekeeping):
  └─ TASK-COD-FIX-027 (Dead Code)       ← ساعة
  └─ TASK-COD-FIX-028 (Antiforgery API) ← ساعة
  └─ TASK-COD-FIX-029 (Validation)      ← ساعة
```

---

## العلاقة بين المهام والملفات

| الملف | يتأثر بالمهام |
|---|---|
| `Builder.cshtml` | FIX-021, FIX-025, FIX-026 |
| `Builder.cshtml.cs` | FIX-021, FIX-024, FIX-025, FIX-027 |
| `card-builder.js` | FIX-022, FIX-026, FIX-028, FIX-029 |
| `CardBuilderService.cs` | FIX-022 (BuildChartConfig) |
| `KpiQueryBuilder.cs` | FIX-023 |
| `DashboardCard.cs` | FIX-025 |
| `CardBuilderModels.cs` | FIX-025 (DashboardCardDto) |
| `CardBuilder.cshtml.cs` | FIX-027, FIX-028 |
| `_CardsLayout.cshtml` | FIX-022 (تأكيد وجود Chart.js) |

---

## اختبار القبول بعد الإصلاح

1. فتح الـ Builder → إكمال الخطوات الخمس → حفظ ← يجب أن تظهر رسالة Toast "تم الحفظ بنجاح"
2. العودة إلى قائمة البطاقات ← يجب أن تظهر البطاقة الجديدة
3. فتح Dashboard عام ← يجب أن تظهر البطاقة مع بياناتها
4. المعاينة المباشرة في الـ Builder يجب أن تعمل لكل أنواع البطاقات
5. حفظ بطاقة KPI مع withChange/composite ← يجب أن تحفظ وتعرض بشكل صحيح
6. حفظ بطاقة من مصدر SqlTable ← يجب أن تعرض على الـ Dashboard بدون أخطاء
7. الاستنساخ (Clone) ← يجب أن يعمل بشكل صحيح

---

**الخطوة التالية:** بعد اعتماد الخطة، سنبدأ بـ TASK-COD-FIX-021 (Antiforgery — سطر واحد، يصلح الحفظ فوراً) ثم ننتقل إلى TASK-COD-FIX-022 (Chart.js — أكبر مهمة).
