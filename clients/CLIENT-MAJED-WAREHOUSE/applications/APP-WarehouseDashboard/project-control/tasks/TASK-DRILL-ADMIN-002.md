# TASK-DRILL-ADMIN-002 — Phase B: UI for Test Query + Parameter Contract Fields

| البند | القيمة |
|---|---|
| **المعرف** | TASK-DRILL-ADMIN-002 |
| **المجموعة** | DRILL-DOWN (Phase B) |
| **النوع** | Frontend (Razor CSHTML + JavaScript) |
| **الوكيل المقترح** | ui-designer |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `project-preparation/DRILL_DOWN_DEVELOPMENT_PLAN.md` v3.0 — §10, §11.2 |

---

## 1. الهدف

تحديث واجهة Admin DrillDown (`admin-secure-panel/DrillDown/Index.cshtml`) لتشمل:
1. حقول Parameter Contract الجديدة (`ParameterColumn`, `LabelColumn`, `RequiresParentValue`) في نموذج الإضافة/التعديل
2. زر **اختبار الاستعلام** مع عرض النتائج في Preview داخل نفس النموذج
3. عرض الحقول الجديدة في جدول المستويات

---

## 2. السياق

- **Backend جاهز** (TASK-DRILL-ADMIN-001 ✅): `OnPostTestQueryAsync` handler يعمل، `OnPostSaveAsync` يستقبل `parameterColumn`, `labelColumn`, `requiresParentValue`
- الملف الذي سيُعدّل: `Pages/admin-secure-panel/DrillDown/Index.cshtml`
- النمط الحالي: تصميم RTL عربي متسق مع باقي صفحات Admin، يستخدم CSS variables (`var(--c-primary)`, `var(--c-surface)`, إلخ)

### 2.1 Backend API Contract لاختبار الاستعلام

```
POST ?handler=TestQuery
Form Data:
  drillDownQuery: string (required)
  parameterColumn: string? (optional)
  labelColumn: string? (optional)
  testParameterValue: string? (optional)

Response (success):
{
  success: true,
  columns: string[],
  rows: Dictionary<string, object?>[],
  rowCount: number,
  warnings: string[] | null
}

Response (failure):
{
  success: false,
  errorMessage: string
}
```

### 2.2 Backend Save Contract

```
POST ?handler=Save
Form Data:
  parentCardId: int
  level: int
  displayName: string
  targetChartType: string
  drillDownQuery: string
  id: int? (for edit)
  parameterColumn: string? (NEW)
  labelColumn: string? (NEW)
  requiresParentValue: bool (NEW)
```

---

## 3. التغييرات المطلوبة

### 3.1 إضافة الحقول الجديدة إلى جدول المستويات

في دالة `renderLevels`، أضف عمودين جديدين في الجدول بعد عمود "النوع" وقبل "الاستعلام":
- **عمود الباراميتر**: يعرض قيمة `parameterColumn` أو "—" إذا كانت فارغة
- **يتطلب قيمة**: يعرض "نعم" / "لا" حسب `requiresParentValue`

### 3.2 إضافة الحقول الجديدة إلى النموذج (Form Panel)

في `<div id="formPanel">`، بعد حقل `queryInput` وقبل `wd-actions`، أضف:

1. **حقل `ParameterColumn`** — input نصي (text)
   - التسمية: "عمود الباراميتر (اختياري)"
   - التلميح: "اسم العمود الذي تمرّر قيمته للمستوى التالي"
   - المكان: بعد queryInput وقبل labelColumn

2. **حقل `LabelColumn`** — input نصي (text)
   - التسمية: "عمود التسمية (اختياري)"
   - التلميح: "اسم العمود المعروض في Breadcrumb"

3. **حقل `RequiresParentValue`** — checkbox
   - التسمية: "يتطلب قيمة من المستوى السابق"
   - بجانبه أيقونة ❔ للمساعدة توضح أن المستوى الأول عادة لا يتطلب قيمة

4. **حقل `testParameterValue`** — input نصي (text) يظهر فقط إذا الاستعلام يحتوي `@p0`
   - التسمية: "قيمة اختبار للمعامل @p0 (اختياري)"
   - مرتبط بزر اختبار الاستعلام

### 3.3 إضافة زر "اختبار الاستعلام" ومكان عرض النتائج

أضف داخل النموذج بعد حقول Parameter contract وقبل `wd-actions`:

1. **زر "اختبار الاستعلام"** — زر ثانوي (`wd-btn wd-btn--secondary`)
   - عند الضغط: يقرأ `queryInput.value` + `parameterColumn` + `labelColumn` + `testParameterValue`
   - يستدعي `?handler=TestQuery` عبر POST مع `RequestVerificationToken`
   - يظهر مؤشر تحميل spinner/skeleton أثناء الانتظار
   - يعرض النتائج في منطقة preview

2. **منطقة Preview** — تظهر بعد الزر
   - حالة النجاح:
     - عدد الصفوف: "تم استرجاع X صف"
     - جدول صغير يعرض أول 10 صفوف مع الأعمدة (scroll أفقي)
     - التحذيرات (إذا وجدت): تظهر بأيقونة ⚠ ولون أصفر
   - حالة الفشل:
     - رسالة الخطأ بالعربي مع أيقونة ❌
   - الحالة الفارغة:
     - "لا توجد نتائج للاستعلام المحدد"

### 3.4 تحديث دالة saveLevel لإرسال الحقول الجديدة

في دالة `saveLevel`:
- أضف `fd.append('parameterColumn', ...)` من `parameterColumnInput`
- أضف `fd.append('labelColumn', ...)` من `labelColumnInput`
- أضف `fd.append('requiresParentValue', ...)` من `requiresParentValueInput.checked`

### 3.5 تحديث دوال openForm/editLevel

عند فتح النموذج:
- `openForm`: القيم الافتراضية لـ parameterColumn و labelColumn فارغة، requiresParentValue = false
- `editLevel`: اقرأ القيم من `level.parameterColumn`, `level.labelColumn`, `level.requiresParentValue` (متوفرة من الـ backend)

### 3.6 إظهار/إخفاء حقل `testParameterValue`

إذا كان الـ `queryInput` يحتوي على `@p0` (يتغير عند الكتابة أو عند فتح التعديل)، أظهر حقل `testParameterValue`. وإلا، اخفه.

---

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\DrillDown\Index.cshtml
```

**فقط هذا الملف.** لا تلمس `.cshtml.cs`.

---

## 5. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | جدول المستويات يعرض `parameterColumn` و `requiresParentValue` |
| AC-2 | النموذج يحتوي على حقول `ParameterColumn`, `LabelColumn`, `RequiresParentValue` |
| AC-3 | حقل `testParameterValue` يظهر فقط إذا الاستعلام يحتوي `@p0` |
| AC-4 | زر "اختبار الاستعلام" موجود في النموذج |
| AC-5 | زر الاختبار يستدعي `?handler=TestQuery` عبر POST مع anti-forgery token |
| AC-6 | مؤشر تحميل يظهر أثناء تنفيذ الاختبار |
| AC-7 | جدول Preview يعرض أول 10 صفوف مع الأعمدة |
| AC-8 | عدد الصفوف يظهر في الـ Preview |
| AC-9 | التحذيرات (إن وجدت) تظهر بلون أصفر مع ⚠ |
| AC-10 | أخطاء الاختبار تظهر بلون أحمر مع ❌ |
| AC-11 | دالة saveLevel ترسل `parameterColumn`, `labelColumn`, `requiresParentValue` |
| AC-12 | `openForm` يضبط القيم الافتراضية للحقول الجديدة |
| AC-13 | `editLevel` يقرأ ويعرض الحقول الجديدة من `level` object |
| AC-14 | `dotnet build` — 0 errors, 0 warnings |
| AC-15 | التصميم متسق مع باقي صفحات Admin (CSS variables, RTL, Arabic) |
| AC-16 | لا secrets في أي ملف |

---

## 6. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ ملف UI واحد |
| لا تغيير في قاعدة البيانات | ✅ |
| Backend API متوافق | ✅ |
| لا تغيير Auth | ✅ |

**Gate Status:** ✅ PASS

---

## 7. ملاحظات للوكيل المنفذ

1. **اقرأ الملف قبل تعديله** — الـ CSHTML يحتوي 362 سطراً حالياً.
2. النمط المستخدم: `var(--c-*)` متغيرات CSS، كلاس `wd-*`.
3. اللغه: كل النصوص بالعربي.
4. Anti-forgery token متاح عبر `document.querySelector('input[name=__RequestVerificationToken]').value`.
5. كل المسميات متوافقة مع CamelCase الذي يستخدمه الـ backend.
6. بعد التعديل، شغّل `dotnet build` للتأكد من 0 errors (الـ CSHTML يُتحقق منه أثناء الـ build).
7. لا تنسى التنسيق RTL.

---

## 8. Vitality & Polish Checklist

- ✅ / N/A — Skeleton Loading / Shimmer: نعم، عند اختبار الاستعلام
- ✅ / N/A — Toast Notifications: موجودة مسبقاً
- ✅ / N/A — Search حقيقي: N/A (صفحة تكوين، لا search)
- ✅ / N/A — Empty State: نعم، "اختر بطاقة لعرض مستويات التنقّل"

---

**End of TASK-DRILL-ADMIN-002**
