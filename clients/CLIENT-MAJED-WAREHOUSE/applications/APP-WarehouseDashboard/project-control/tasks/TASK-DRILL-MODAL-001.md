# TASK-DRILL-MODAL-001 — Phase C: Modal State Machine + Level Navigation

| البند | القيمة |
|---|---|
| **المعرف** | TASK-DRILL-MODAL-001 |
| **المجموعة** | DRILL-DOWN (Phase C) |
| **النوع** | JavaScript + CSHTML (Dashboard page) |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `project-preparation/DRILL_DOWN_DEVELOPMENT_PLAN.md` v3.0 — §5, §12 Phase C |

---

## 1. الهدف

إعادة هيكلة مودال Drill Down الموجود في `Index.cshtml` ليعمل مع API المستويات الجديد:
- `/api/dashboard/drill/{cardId}/{level}?parentValue=...`
- يدعم التنقل بين المستويات داخل **نفس المودال** (Same Modal, Swap Content)
- يستخدم `ParameterColumn` و `LabelColumn` و `NextRequiresParentValue` من الـ API
- Breadcrumb ديناميكي للمستويات
- حالات التحميل/الفارغ/الخطأ لكل مستوى

---

## 2. السياق الحالي

يوجد مودال Drill موجود حالياً في `Index.cshtml` (الأسطر 858-872 + الدوال 1564-1650) لكنه:

| المشكلة | التفاصيل |
|---|---|
| API قديم | يستدعي `/api/dashboard/drill/{id}` بدون level parameter |
| لا يدعم المستويات | يفتح المستوى الأول فقط |
| لا يدعم ParameterColumn | لا يستخدم `ParameterColumn` لنقل القيم بين المستويات |
| لا يدعم NextRequiresParentValue | لا يعرف إذا كان المستوى التالي يحتاج قيمة |
| الفتح من البطاقة | يفتح بالضغط على البطاقة كاملة، وليس بزر **تفاصيل** منفصل |
| Breadcrumb بسيط | يستخدم titles فقط، لا أسماء المستويات |

### 2.1 API الجديد المستهدف

```
GET /api/dashboard/drill/{cardId}/{level}?parentValue={value}

Response:
{
  cardId, cardTitle, level, displayName, chartType,
  hasNextLevel, parameterColumn, labelColumn,
  nextRequiresParentValue,
  status: "success" | "empty" | "error" | "none",
  errorMessage,
  columns: string[],
  rows: Dictionary<string, object?>[],
  kpiValue
}
```

حقول مهمة للـ State Machine:
- `level`: رقم المستوى الحالي
- `hasNextLevel`: هل يوجد مستوى تالٍ؟
- `parameterColumn`: أي عمود يؤخذ منه @p0 للمستوى التالي
- `nextRequiresParentValue`: هل المستوى التالي يحتاج قيمة قبل الانتقال؟
- `status`: حالة النتيجة

### 2.2 مصادر البيان للمودال

كل بطاقة في `Index.cshtml` لديها `data-card-id`. وعند بناء الـ HTML، البطاقات التي لديها Drill تحصل على class `wd-card--drillable`. في المستقبل (TASK-DRILL-ENTRY-001) سيُضاف زر **تفاصيل**.

### 2.3 الملفات المعنية

- **الملف الرئيسي**: `Pages/Index.cshtml` (2087 سطراً)
  - Modal HTML: الأسطر 858-872
  - `wdOpenDrill`: الأسطر 1566-1635
  - `wdDrillNavigate`: الأسطر 1637-1639
  - `wdCloseDrillModal`: الأسطر 1641-1650
  - CSS: الأسطر 474-501 (breadcrumb), 374-393 (drillable card)
- **API**: `Pages/Api/Dashboard/Drill.cshtml.cs` (محدّث في Phase A — لا تلمسه)
- **Payload**: `Pages/DrillDataResult.cs` (محدّث — لا تلمسه)

---

## 3. التغييرات المطلوبة

### 3.1 إعادة هيكلة `wdOpenDrill` إلى State Machine

استبدل دالة `wdOpenDrill` بنظام State Machine جديد. استخدم `window.__drillState` لتخزين الحالة:

```javascript
window.__drillState = {
    cardId: null,           // البطاقة الحالية
    cardTitle: null,        // عنوان البطاقة
    currentLevel: 1,        // المستوى الحالي
    maxLevel: null,         // أقصى مستوى معروف
    trail: [],              // مسار breadcrumb [{level, displayName, labelValue}]
    parameterValue: null,   // القيمة المختارة من المستوى الحالي للمستوى التالي
    parameterColumn: null,  // العمود الذي يؤخذ منه parameterValue
    labelColumn: null,      // العمود المعروض في Breadcrumb
    nextRequiresParentValue: false, // هل المستوى التالي يحتاج قيمة
    loading: false          // هل المودال قيد التحميل
};
```

### 3.2 الدوال الجديدة المطلوبة

#### `wdOpenDrill(cardId, cardTitle)`
- فتح المودال
- إعادة تعيين `__drillState` للبطاقة الجديدة
- `currentLevel = 1`, `trail = [{level: 1, displayName: cardTitle, labelValue: null}]`
- استدعاء `wdLoadLevel()`
- عرض Skeleton في `bodyEl`

#### `wdLoadLevel()`
- قراءة `currentLevel` من `__drillState`
- بناء URL: `/api/dashboard/drill/{cardId}/{currentLevel}?parentValue={parameterValue}`
- جلب البيانات
- تحديث `__drillState.maxLevel` و `hasNextLevel` و `nextRequiresParentValue` و `parameterColumn` و `labelColumn` من الـ response
- تحديث Breadcrumb
- استدعاء `wdRenderLevel(data)` لعرض المحتوى

#### `wdRenderLevel(data)`
- عرض المحتوى حسب `data.chartType`:
  - `Table`: جدول مع row selection
  - `Bar/Line/Pie`: رسم بياني + قائمة اختيار
  - `KPI/Gauge`: بطاقة + زر "مستوى تالٍ" إذا `hasNextLevel`
- إظهار/إخفاء زر "المستوى التالي" حسب `hasNextLevel` و `nextRequiresParentValue`
- إذا `nextRequiresParentValue == true` و لم يختر المستخدم قيمة → تعطيل زر المستوى التالي

#### `wdSelectRow(value, label)`
- عند اختيار صف/عنصر في المستوى الحالي
- تخزين `parameterValue = value` و `parameterLabel = label` في `__drillState`
- تحديث `trail`:
  ```javascript
  trail[trail.length - 1].labelValue = label; // تحديث آخر مستوى
  ```
- تفعيل زر "المستوى التالي" إذا كان موجوداً

#### `wdNavigateToLevel(level)`
- التنقل لمستوى معين (من Breadcrumb أو زر المستوى التالي)
- تحديث `__drillState.currentLevel` و `__drillState.parameterValue`
- قطع `__drillState.trail` إلى الطول المناسب
- استدعاء `wdLoadLevel()`

#### `wdNextLevel()`
- اختصار: `wdNavigateToLevel(__drillState.currentLevel + 1)`
- فقط إذا `hasNextLevel == true` و (إذا `nextRequiresParentValue` → `parameterValue != null`)

#### `wdCloseDrillModal()` (موجودة، تحتاج تحديث طفيف)
- إعادة تعيين `__drillState`
- إخفاء المودال وتنظيف المحتوى

### 3.3 تحديث Breadcrumb

استبدل آلية Breadcrumb الحالية لتعمل مع المستويات:

```javascript
function wdRenderBreadcrumb() {
    var crumbEl = document.getElementById('wd-drill-modal-breadcrumb');
    var state = window.__drillState;
    if (!crumbEl || !state) return;
    
    var html = state.trail.map(function(t, i) {
        var label = t.labelValue || t.displayName;
        if (i === state.trail.length - 1) {
            return '<span class="wd-modal__crumb wd-modal__crumb--active">' + escapeHtml(label) + '</span>';
        }
        return '<button class="wd-modal__crumb wd-modal__crumb--link" onclick="wdNavigateToLevel(' + t.level + ')">' + escapeHtml(label) + '</button>';
    }).join('<span class="wd-modal__crumb-sep" aria-hidden="true">/</span>');
    
    crumbEl.innerHTML = html;
}
```

### 3.4 عرض حالات المحتوى

#### تحميل (Loading)
```html
<div class="wd-skeleton-wrap">
    <div class="wd-skel wd-skel--tall" style="height:300px;"></div>
</div>
```

#### فارغ (Empty) — `data.status === "empty"`
```html
<div class="wd-empty">
    <span class="wd-empty__icon">📊</span>
    <p>لا توجد بيانات لهذا المستوى.</p>
</div>
```

#### خطأ (Error) — `data.status === "error"`
```html
<div class="wd-empty">
    <span class="wd-empty__icon">❌</span>
    <p>{data.errorMessage}</p>
    <button onclick="wdLoadLevel()" class="wd-btn wd-btn--sm">إعادة المحاولة</button>
</div>
```

#### غير موجود (None) — `data.status === "none"`
```html
<div class="wd-empty">
    <span class="wd-empty__icon">ℹ️</span>
    <p>{data.errorMessage}</p>
</div>
```

### 3.5 إضافة مؤشرات التنقل

أضف بعد `bodyEl` في المودال منطقة تنقل (`wd-modal__footer`):
- زر "المستوى السابق" (يظهر إذا `currentLevel > 1`)
- زر "المستوى التالي" (يظهر إذا `hasNextLevel == true`)
- Badge "آخر مستوى" (إذا `hasNextLevel == false`)
- رسالة تعليمات: "اختر صفاً/عنصراً للانتقال للمستوى التالي" (إذا `nextRequiresParentValue == true`)

### 3.6 تحديث `wdCloseDrillModal`

```javascript
function wdCloseDrillModal() {
    window.__drillState = null; // إعادة تعيين الحالة
    var modal = document.getElementById('wd-drill-modal');
    if (modal) {
        modal.hidden = true;
        document.body.style.overflow = '';
        var body = document.getElementById('wd-drill-modal-body');
        if (body) body.innerHTML = '';
    }
}
```

### 3.7 تحديث HTML المودال

أضف منطقة footer في المودال للتنقل:

```
<div class="wd-modal__footer" id="wd-drill-modal-footer">
    <!-- تُعبأ ديناميكياً بواسطة JavaScript -->
</div>
```

### 3.8 قابلية الاختبار

تأكد من أن `wdOpenDrill(cardId, cardTitle)` يمكن استدعاؤها من أي مكان (window scope) لتسهيل ربط زر "تفاصيل" في المستقبل.

---

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

**فقط هذا الملف.** لا تلمس `.cshtml.cs` أو أي ملف آخر.

---

## 5. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | `__drillState` يُدار كحالة مركزية (cardId, currentLevel, parameterValue, trail) |
| AC-2 | `wdOpenDrill(cardId, cardTitle)` يعيد تعيين الحالة ويحمل Level 1 |
| AC-3 | `wdLoadLevel()` يستدعي `/api/dashboard/drill/{cardId}/{level}?parentValue=...` |
| AC-4 | `wdRenderLevel(data)` يعرض Table أو Chart أو KPI/Gauge حسب `chartType` |
| AC-5 | `wdSelectRow(value, label)` يخزن القيمة المختارة ويفعّل زر المستوى التالي |
| AC-6 | `wdNavigateToLevel(level)` ينتقل لمستوى معين ويحدّث الـ trail |
| AC-7 | Breadcrumb يُظهر trail كاملاً مع أسماء المستويات والقيم المختارة |
| AC-8 | عناصر Breadcrumb السابقة قابلة للنقر للعودة لذلك المستوى |
| AC-9 | زر "المستوى التالي" يظهر فقط إذا `hasNextLevel == true` |
| AC-10 | زر "المستوى التالي" مُعطّل إذا `nextRequiresParentValue == true` و `parameterValue == null` |
| AC-11 | Badge "آخر مستوى" يظهر إذا `hasNextLevel == false` |
| AC-12 | زر "المستوى السابق" يظهر إذا `currentLevel > 1` |
| AC-13 | تعليمات "اختر صفاً..." تظهر إذا `nextRequiresParentValue == true` |
| AC-14 | حالة التحميل (Skeleton) تظهر أثناء جلب البيانات |
| AC-15 | حالة الخطأ تظهر رسالة الخطأ + زر "إعادة المحاولة" |
| AC-16 | حالة "فارغ" تظهر رسالة "لا توجد بيانات" |
| AC-17 | حالة "غير موجود" (none) تظهر رسالة `errorMessage` |
| AC-18 | `wdCloseDrillModal()` يعيد تعيين `__drillState` ويخفي المودال |
| AC-19 | المودال يُغلق بالضغط على زر الإغلاق أو الـ overlay أو Escape |
| AC-20 | لا تسريبات للذاكرة — تدمير الرسوم البيانية عند إغلاق المودال |
| AC-21 | `dotnet build` — 0 errors, 0 warnings |
| AC-22 | Encoding عربي سليم |
| AC-23 | لا secrets |

---

## 6. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ ملف واحد — JavaScript في CSHTML |
| لا تغيير في قاعدة البيانات | ✅ |
| API جاهز | ✅ — Level-based Drill API (Phase A) |
| لا تغيير Auth | ✅ |
| Build متوقع | ✅ 0 errors, 0 warnings |

**Gate Status:** ✅ PASS

---

## 7. ملاحظات للوكيل المنفذ

1. **اقرأ الملف كاملاً أولاً** — `Index.cshtml` يحتوي 2087 سطراً.
2. احتفظ بكل الشيفرات الموجودة — أنت تعدّل فقط `wdOpenDrill`, `wdCloseDrillModal` و الـ modal HTML.
3. **لا تحذف الدوال الموجودة** — أضف الدوال الجديدة بجانبها.
4. لا تلمس دوال `wdLoadCard`, `wdRenderChart`, `wdRenderGrid`, `wdRenderGauge` — ستحتاجها داخل المودال.
5. `escapeHtml()` موجودة — استخدمها.
6. `wdEmptyHtml()` و `wdErrorHtml(msg, id)` موجودات — استخدمها لحالات الخطأ والفارغة.
7. المودال الحالي يفتح من `wd-drill-modal` — حافظ على نفس IDs.
8. دوال `wdRenderChart`, `wdRenderGrid`, `wdRenderGauge` تأخذ (div, data) — استخدمها.
9. **شغّل `dotnet build`** بعد التعديل — 0 errors, 0 warnings.
10. لا تنسى معالجة حالات:
   - المستوى الأول: `parameterValue = null` غالباً
   - `@p0` في الاستعلام مع `parentValue` فارغ → API يرجع error تلقائياً (موجود من Phase A)

---

## 8. Vitality & Polish Checklist

- ✅ Skeleton Loading/Shimmer — موجود في `wdLoadLevel`
- ✅ Toast Notifications — موجودة مسبقاً
- ✅ Empty State — "لا توجد بيانات"
- ✅ Error State — رسالة + إعادة محاولة
- ✅ None State — رسالة إعلامية

---

**End of TASK-DRILL-MODAL-001**
