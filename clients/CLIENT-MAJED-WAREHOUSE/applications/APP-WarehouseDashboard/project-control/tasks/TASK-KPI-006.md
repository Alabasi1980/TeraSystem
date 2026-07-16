# TASK-KPI-006 — Builder UI — KPI Settings Step

| البند | القيمة |
|---|---|
| **المعرف** | TASK-KPI-006 |
| **المجموعة** | KPI |
| **النوع** | Frontend — Razor Page + JavaScript |
| **الوكيل** | ui-designer |
| **الأولوية** | Critical |
| **الحالة** | DONE |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

Card Builder لا يوجد فيه خيار لضبط إعدادات KPI المتقدمة (KpiMode, ValueColumn, DateColumn, ShowChange, ShowSparkline, ShowGrandTotal, DateFilterMode, إلخ).

**المطلوب:** إضافة خطوة جديدة في الـ Wizard لضبط هذه الإعدادات عندما يُختار نوع البطاقة KPI.

---

## 2. الهدف

إضافة **خطوة 4 جديدة** في Card Builder Wizard:
1. **KPI Mode** — اختيار نوع KPI (simple / withChange / composite)
2. **Column Mappings** — تعيين الأعمدة (ValueColumn, DateColumn, CategoryColumn)
3. **Change Settings** — إعدادات نسبة التغير
4. **Sparkline Settings** — إعدادات رسم الـ Sparkline
5. **Grand Total Settings** — إعدادات الإجمالي العام
6. **Date Filter Settings** — إعدادات الفلتر الزمني

---

## 3. التصميم المقترح

### 3.1 التعديلات المطلوبة في Builder.cshtml

**الملاحظة:** الخطوة الجديدة تظهر **فقط عندما يُختار نوع البطاقة KPI**. للأنواع الأخرى (Bar, Line, Pie, Table, Gauge)، الخطوة مخفية تماماً.

#### 3.1.1 تحديث Step Indicator

أضف خطوة جديدة في `wb-steps` بعد الخطوة 3:

```html
<li class="wb-step" data-step="4">
    <span class="wb-step__number" aria-hidden="true">4</span>
    <span class="wb-step__label">إعدادات KPI</span>
    <span class="wb-step__bar" aria-hidden="true"></span>
</li>
```

وقم بتحديث الخطوة 4 الحالية لت become خطوة 5:

```html
<li class="wb-step" data-step="5">
    <span class="wb-step__number" aria-hidden="true">5</span>
    <span class="wb-step__label">الشكل</span>
</li>
```

#### 3.1.2 إضافة Step Panel جديد (خطوة 4 — KPI Settings)

أضف fieldset جديد بعد خطوة 3 (Basic Fields) وقبل خطوة 4 الحالية (Visual Settings):

```html
<!-- Step 4: KPI Settings (only shown when chartType == 'KPI') -->
<fieldset class="wb-step-panel wd-hidden" data-step="4" aria-labelledby="wb-step4-title" role="group" id="wb-step-kpi">
    <legend id="wb-step4-title" class="wb-step-panel__title">
        <svg class="wb-icon" aria-hidden="true"><use href="#wb-icon-kpi"></use></svg>
        إعدادات المؤشر KPI
    </legend>
    <p class="wb-step-panel__hint">تكوين طريقة عرض المؤشر والبيانات المتعددة.</p>

    <!-- KPI Mode -->
    <div class="wb-visual-section">
        <h4 class="wb-visual-section__title">نوع المؤشر</h4>
        <div class="wb-kpi-mode-picker" id="wb-kpi-mode-picker" role="radiogroup" aria-label="نوع المؤشر">
            <label class="wb-kpi-mode-card" data-mode="simple" role="radio" aria-checked="true" tabindex="0">
                <span class="wb-kpi-mode-card__name">بسيط</span>
                <span class="wb-kpi-mode-card__desc">قيمة واحدة فقط</span>
            </label>
            <label class="wb-kpi-mode-card" data-mode="withChange" role="radio" aria-checked="false" tabindex="-1">
                <span class="wb-kpi-mode-card__name">مع تغير</span>
                <span class="wb-kpi-mode-card__desc">قيمة + نسبة التغير</span>
            </label>
            <label class="wb-kpi-mode-card" data-mode="composite" role="radio" aria-checked="false" tabindex="-1">
                <span class="wb-kpi-mode-card__name">متقدم</span>
                <span class="wb-kpi-mode-card__desc">قيمة + تغير + Sparkline + إجمالي</span>
            </label>
        </div>
        <input type="hidden" name="kpiMode" id="wb-h-kpiMode" value="simple">
    </div>

    <!-- Column Mappings -->
    <div class="wb-visual-section">
        <h4 class="wb-visual-section__title">تعيينات الأعمدة</h4>
        <div class="wd-form__grid">
            <div class="wd-field">
                <label class="wd-field__label" for="wb-kpi-value-column">عمود القيمة <span class="wb-required" aria-hidden="true">*</span></label>
                <select id="wb-kpi-value-column" class="wd-select" name="valueColumn" aria-describedby="wb-kpi-value-hint">
                    <option value="">اختر عموداً...</option>
                </select>
                <span id="wb-kpi-value-hint" class="wd-field__hint">العمود الرقمي للقيمة الرئيسية</span>
            </div>
            <div class="wd-field">
                <label class="wd-field__label" for="wb-kpi-date-column">عمود التاريخ <span class="wb-required" aria-hidden="true">*</span></label>
                <select id="wb-kpi-date-column" class="wd-select" name="dateColumn" aria-describedby="wb-kpi-date-hint">
                    <option value="">اختر عموداً...</option>
                </select>
                <span id="wb-kpi-date-hint" class="wd-field__hint">عمود التاريخ للفلترة الزمنية</span>
            </div>
            <div class="wd-field">
                <label class="wd-field__label" for="wb-kpi-category-column">عمود التصنيف (اختياري)</label>
                <select id="wb-kpi-category-column" class="wd-select" name="categoryColumn">
                    <option value="">بدون تصنيف</option>
                </select>
            </div>
        </div>
    </div>

    <!-- Change Settings (shown only for withChange and composite) -->
    <div class="wb-visual-section wb-kpi-change-section wd-hidden" id="wb-kpi-change-section">
        <h4 class="wb-visual-section__title">إعدادات نسبة التغير</h4>
        <div class="wd-form__grid">
            <div class="wd-field">
                <label class="wd-field__label" for="wb-kpi-change-source">مقارنة بـ</label>
                <select id="wb-kpi-change-source" class="wd-select" name="changeSource">
                    <option value="previousPeriod">الفترة السابقة</option>
                    <option value="previousMonth">الشهر السابق</option>
                    <option value="previousYear">السنة السابقة</option>
                </select>
            </div>
        </div>
    </div>

    <!-- Sparkline Settings (shown only for composite) -->
    <div class="wb-visual-section wb-kpi-sparkline-section wd-hidden" id="wb-kpi-sparkline-section">
        <h4 class="wb-visual-section__title">إعدادات رسم الاتجاه</h4>
        <div class="wd-form__grid">
            <div class="wd-field">
                <label class="wd-field__label" for="wb-kpi-sparkline-months">عدد الأشهر</label>
                <select id="wb-kpi-sparkline-months" class="wd-select" name="sparklineMonths">
                    <option value="3">3 أشهر</option>
                    <option value="6" selected>6 أشهر</option>
                    <option value="12">12 شهر</option>
                </select>
            </div>
        </div>
    </div>

    <!-- Grand Total Settings (shown only for composite) -->
    <div class="wb-visual-section wb-kpi-total-section wd-hidden" id="wb-kpi-total-section">
        <h4 class="wb-visual-section__title">إعدادات الإجمالي العام</h4>
        <div class="wd-form__grid">
            <div class="wd-field">
                <label class="wd-field__label" for="wb-kpi-grand-total-source">مصدر الإجمالي</label>
                <select id="wb-kpi-grand-total-source" class="wd-select" name="grandTotalSource">
                    <option value="sameTable">نفس الجدول (بدون فلتر)</option>
                    <option value="customQuery">استعلام مخصص</option>
                </select>
            </div>
        </div>
    </div>

    <!-- Date Filter Settings (shown only for withChange and composite) -->
    <div class="wb-visual-section wb-kpi-date-section wd-hidden" id="wb-kpi-date-section">
        <h4 class="wb-visual-section__title">إعدادات الفلتر الزمني</h4>
        <div class="wd-form__grid">
            <div class="wd-field">
                <label class="wd-field__label" for="wb-kpi-date-filter-mode">طريقة الفلترة</label>
                <select id="wb-kpi-date-filter-mode" class="wd-select" name="dateFilterMode">
                    <option value="dashboard">حسب فلتر لوحة المعلومات</option>
                    <option value="fixed">نطاق ثابت</option>
                    <option value="relative">آخر N يوم</option>
                </select>
            </div>
            <div class="wd-field wb-kpi-fixed-dates wd-hidden" id="wb-kpi-fixed-dates">
                <label class="wd-field__label" for="wb-kpi-fixed-start">تاريخ البداية</label>
                <input type="date" id="wb-kpi-fixed-start" class="wd-input" name="fixedStartDate">
            </div>
            <div class="wd-field wb-kpi-fixed-dates wd-hidden" id="wb-kpi-fixed-end">
                <label class="wd-field__label" for="wb-kpi-fixed-end-input">تاريخ النهاية</label>
                <input type="date" id="wb-kpi-fixed-end-input" class="wd-input" name="fixedEndDate">
            </div>
            <div class="wd-field wb-kpi-relative-days wd-hidden" id="wb-kpi-relative-days">
                <label class="wd-field__label" for="wb-kpi-relative-days-input">عدد الأيام</label>
                <input type="number" id="wb-kpi-relative-days-input" class="wd-input" name="relativeDays" min="1" max="365" value="30">
            </div>
        </div>
    </div>

    <div class="wb-step-panel__error wd-hidden" id="wb-step4-kpi-error" role="alert" aria-live="polite"></div>
</fieldset>
```

#### 3.1.3 تحديث الخطوة 4 الحالية (Visual Settings)

غيّر `data-step="4"` إلى `data-step="5"` في fieldset Visual Settings.

#### 3.1.4 إضافة Hidden Fields

أضف هذه الحقول المخفية داخل الـ form:

```html
<input type="hidden" name="valueColumn" id="wb-h-valueColumn" value="">
<input type="hidden" name="dateColumn" id="wb-h-dateColumn" value="">
<input type="hidden" name="categoryColumn" id="wb-h-categoryColumn" value="">
<input type="hidden" name="showChange" id="wb-h-showChange" value="false">
<input type="hidden" name="changeSource" id="wb-h-changeSource" value="previousPeriod">
<input type="hidden" name="showSparkline" id="wb-h-showSparkline" value="false">
<input type="hidden" name="sparklineMonths" id="wb-h-sparklineMonths" value="6">
<input type="hidden" name="showGrandTotal" id="wb-h-showGrandTotal" value="false">
<input type="hidden" name="grandTotalSource" id="wb-h-grandTotalSource" value="sameTable">
<input type="hidden" name="dateFilterMode" id="wb-h-dateFilterMode" value="dashboard">
<input type="hidden" name="fixedStartDate" id="wb-h-fixedStartDate" value="">
<input type="hidden" name="fixedEndDate" id="wb-h-fixedEndDate" value="">
<input type="hidden" name="relativeDays" id="wb-h-relativeDays" value="30">
```

### 3.2 التعديلات المطلوبة في card-builder.js

#### 3.2.1 تحديث Step Navigation

في `card-builder.js`، يجب تحديث منطق التنقل بين الخطوات:
- الخطوات 1-3 كما كانت
- الخطوة 4 (KPI Settings) تظهر فقط عندما `chartType == 'KPI'`
- الخطوة 5 (Visual Settings) هي الخطوة الأخيرة

#### 3.2.2 تحديث KPI Mode Picker

عند اختيار KPI Mode:
- `simple`: إخفاء جميع الأقسام الفرعية
- `withChange`: إظهار Change Settings + Date Filter
- `composite`: إظهار جميع الأقسام (Change + Sparkline + Grand Total + Date Filter)

#### 3.2.3 تحديث Column Dropdowns

عند اختيار SqlTable source، يجب تحميل أعمدة الجدول في:
- `wb-kpi-value-column`
- `wb-kpi-date-column`
- `wb-kpi-category-column`

#### 3.2.4 تحديث Hidden Fields

عند الحفظ، يجب نقل القيم من الحقول المرئية إلى الحقول المخفية:
```javascript
document.getElementById('wb-h-kpiMode').value = selectedMode;
document.getElementById('wb-h-valueColumn').value = document.getElementById('wb-kpi-value-column').value;
// ... إلخ
```

#### 3.2.5 تحديث showChange / showSparkline / showGrandTotal

هذه الحقول boolean، يجب تعيينها بناءً على KpiMode:
```javascript
if (kpiMode === 'withChange' || kpiMode === 'composite') {
    document.getElementById('wb-h-showChange').value = 'true';
} else {
    document.getElementById('wb-h-showChange').value = 'false';
}

if (kpiMode === 'composite') {
    document.getElementById('wb-h-showSparkline').value = 'true';
    document.getElementById('wb-h-showGrandTotal').value = 'true';
} else {
    document.getElementById('wb-h-showSparkline').value = 'false';
    document.getElementById('wb-h-showGrandTotal').value = 'false';
}
```

### 3.3 CSS — تنسيقات KPI Mode Picker

أضف إلى `card-builder.css`:

```css
/* ===== KPI Mode Picker (TASK-KPI-006) ===== */
.wb-kpi-mode-picker {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: var(--sp-3);
    margin-bottom: var(--sp-4);
}

.wb-kpi-mode-card {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: var(--sp-2);
    padding: var(--sp-4);
    border: 2px solid var(--c-border);
    border-radius: var(--radius-lg);
    background: var(--c-surface);
    cursor: pointer;
    transition: all var(--dur-fast) var(--ease);
    text-align: center;
}

.wb-kpi-mode-card:hover {
    border-color: var(--c-primary);
    background: var(--c-surface-hover);
}

.wb-kpi-mode-card[aria-checked="true"] {
    border-color: var(--c-primary);
    background: rgba(31, 78, 121, 0.06);
    box-shadow: 0 0 0 3px rgba(31, 78, 121, 0.18);
}

.wb-kpi-mode-card__name {
    font-weight: 700;
    font-size: 14px;
    color: var(--c-text);
}

.wb-kpi-mode-card__desc {
    font-size: 12px;
    color: var(--c-text-muted);
}
```

---

## 4. النطاق

### المطلوب
- [x] **A. تعديل `Builder.cshtml`** — إضافة خطوة KPI (HTML + Hidden Fields)
- [x] **B. تعديل `card-builder.js`** — تحديث منطق التنقل + KPI Mode Picker + Column Dropdowns + Hidden Fields
- [x] **C. تعديل `card-builder.css`** — تنسيقات KPI Mode Picker
- [x] **D. التأكد من عمل `dotnet build -c Release` بدون أخطاء**

### غير المطلوب
- لا تغيير في `Index.cshtml.cs`
- لا تغيير في `DashboardService.cs`
- لا تغيير في `Builder.cshtml.cs` (يجب أن يكون متوافقاً بالفعل مع الحقول الجديدة)
- لا تغيير في `Index.cshtml`

---

## 5. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\card-builder.css
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-KPI-006.md
```

---

## 6. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | خطوة KPI تظهر فقط عندما يُختار نوع البطاقة KPI | ✅ |
| AC-2 | KPI Mode Picker يعمل (simple / withChange / composite) | ✅ |
| AC-3 | Column Dropdowns تُحمّل من الجدول المختار | ✅ |
| AC-4 | الأقسام الفرعية تظهر/تختفي حسب KpiMode | ✅ |
| AC-5 | Hidden Fields تُملأ تلقائياً عند الحفظ | ✅ |
| AC-6 | `dotnet build -c Release` ينجح بدون أخطاء | ✅ |
| AC-7 | لا توجد أسرار في الملفات المعدلة | ✅ |

---

## 7. Pre-Execution Gate Result

**Result:** PASS

- Active Technology Profile: `dotnet-razorpages-adonet`
- Smallest safe executable unit: Yes — تعديل UI فقط
- Single goal: Yes — إضافة إعدادات KPI في Card Builder
- UI task: **Yes** — جزء أساسي من واجهة إنشاء البطاقات
- Security sensitivity: Low
- Database impact: None
- Secrets handling: N/A

---

## 8. Engineering Handback

**Status:** DONE

**Files changed:**
1. `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml` — Step indicator (5 steps), KPI Settings fieldset (Step 4), Visual Settings moved to Step 5, KPI hidden fields added
2. `src/WarehouseDashboard.Web/wwwroot/js/card-builder.js` — `isKpiStepVisible()`, `initKpiModePicker()`, `initDateFilterMode()`, `syncKpiHiddenFields()`, updated step navigation (`next`, `goToStep`, `updateStepUI`, `updateFooter`, `validateStep`, `validateStepSilent`), extended `populateMeasurementColumns` for KPI dropdowns, updated `cleanupDuplicateNames`, `selectType` calls `updateStepUI`
3. `src/WarehouseDashboard.Web/wwwroot/css/card-builder.css` — KPI mode picker grid/card styles, KPI step indicator visibility rule

**Build result:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**Issues / Gaps observed:**
- None — all tasks executed as specified

**Secrets check:** Confirmed — no secrets written to any file.

**Acceptance Criteria:**
| # | Criterion | Status |
|---|---|---|
| AC-1 | KPI step shows only when cardType == KPI | ✅ |
| AC-2 | KPI Mode Picker works (simple / withChange / composite) | ✅ |
| AC-3 | Column Dropdowns populate from selected table | ✅ |
| AC-4 | Sub-sections show/hide based on KpiMode | ✅ |
| AC-5 | Hidden Fields auto-populated on save | ✅ |
| AC-6 | `dotnet build -c Release` succeeds (0 errors) | ✅ |
| AC-7 | No secrets in modified files | ✅ |

---

## 9. Tera Post-Execution Review

| البند | القيمة |
|---|---|
| **Result** | ✅ **PASS — ACCEPTED** |
| **Builder.cshtml** | ✅ Step indicator → 5 خطوات + KPI Settings fieldset + Hidden Fields |
| **card-builder.js** | ✅ isKpiStepVisible + initKpiModePicker + initDateFilterMode + syncKpiHiddenFields + تحديث التنقل |
| **card-builder.css** | ✅ KPI Mode Picker grid/cards + step indicator visibility |
| **Backward Compatibility** | ✅ بطاقات非-KPI تعمل كما كانت |
| **Build** | ✅ 0 errors, 0 warnings |
| **Secrets** | ✅ No secrets |

**Verdict: ACCEPTED** — All 7 acceptance criteria met.
