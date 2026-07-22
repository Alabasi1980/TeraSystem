# TASK-BUILDER-UI-003 — Value Format Display: Dashboard Render

| البند | القيمة |
|---|---|
| **المعرف** | TASK-BUILDER-UI-003 |
| **المجموعة** | KPI Enhancement — Value Format Display |
| **النوع** | Frontend (JS + Razor) |
| **الأولوية** | Medium |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-21 |
| **الاعتماد على** | TASK-BUILDER-BEH-002 ✅ (Backend + Builder) |
| **المرجع** | طلب المستخدم: عرض القيمة حسب ValueFormatType بدلاً من فرض د.أ |

---

## 1. الهدف

تعديل دالة عرض بطاقات KPI في الداشبورد لاستخدام `card.valueFormatType` و `card.valueUnit` (التي أضيفت في TASK-BUILDER-BEH-002) بدلاً من فرض `formatMoney` + `د.أ` دائماً.

### الفورمات المطلوبة

| ValueFormatType | Size S (مختصر) | Size M/L (كامل) |
|---|---|---|
| `Currency` | `14.7M د.أ` | `14,700,000.000 د.أ` |
| `Number` | `14.7M` | `14,700,000.000` |
| `Percentage` | `14.7%` | `14.700%` |
| `Custom` (unit="وحدة") | `14.7M وحدة` | `14,700,000.000 وحدة` |

---

## 2. التغييرات المطلوبة

### 2.1 `dashboard-utils.js` — إضافة `formatKpiValue()`

أضف دالة جديدة بعد `formatMoney` (بعد السطر 49):

```javascript
/**
 * Format a KPI value according to the card's configured value format type.
 * Respects abbreviated (small) vs full (medium/large) display modes.
 * 
 * @param {number|string} value - The numeric value to format
 * @param {string} formatType - "Currency" | "Number" | "Percentage" | "Custom"
 * @param {string} unit - Custom unit suffix (only for "Custom" format)
 * @param {boolean} isSmall - true for abbreviated format (size S), false for full (M/L)
 * @returns {string} Formatted value string
 */
function formatKpiValue(value, formatType, unit, isSmall) {
    var n = toNum(value);
    if (n === 0 && formatType === 'Currency') return '0.000 د.أ';
    if (n === 0 && formatType === 'Number') return '0';
    if (n === 0 && formatType === 'Percentage') return '0%';
    
    // Build the numeric portion
    var numericPart;
    if (isSmall) {
        // Abbreviated format (K/M/B)
        numericPart = formatNum ? formatNum(n) : n.toString();
    } else {
        // Full format with commas and 3 decimal places
        var parts = n.toFixed(3).split('.');
        var intPart = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        numericPart = intPart + '.' + parts[1];
    }
    
    switch (formatType) {
        case 'Number':
            return numericPart;
        case 'Percentage':
            return numericPart + '%';
        case 'Custom':
            return numericPart + ' ' + (unit || '');
        case 'Currency':
        default:
            return numericPart + ' د.أ';
    }
}
```

**أضف التصدير** مع الدوال الأخرى في نهاية الملف (بعد السطر 97):
```javascript
window.formatKpiValue = formatKpiValue;
```

### 2.2 `Pages/Index.cshtml` — تحديث `wdRenderKpiCard()`

**التغيير 1 — Hero Value (القيمة الرئيسية):** (السطور 1586-1588)
استبدل:
```javascript
? (window.formatNum ? window.formatNum(window.toNum(rawValue)) + ' د.أ' : rawValue + ' د.أ')
: (window.formatMoney ? window.formatMoney(window.toNum(rawValue)) : rawValue))
```
بـ:
```javascript
? (window.formatKpiValue ? window.formatKpiValue(rawValue, card.valueFormatType, card.valueUnit, true) : rawValue)
: (window.formatKpiValue ? window.formatKpiValue(rawValue, card.valueFormatType, card.valueUnit, false) : rawValue))
```

**التغيير 2 — Change value (قيمة التغير):** (السطر ~1462)
استبدل:
```javascript
? (window.formatNum ? window.formatNum(target) + ' د.أ' : target + ' د.أ')
: (window.formatMoney ? window.formatMoney(target) : (window.formatNum ? window.formatNum(target) : target));
```
بـ:
```javascript
? (window.formatKpiValue ? window.formatKpiValue(target, card.valueFormatType, card.valueUnit, true) : target)
: (window.formatKpiValue ? window.formatKpiValue(target, card.valueFormatType, card.valueUnit, false) : target);
```
*ملاحظة:* قد يكون هذا الخط في سياق مختلف (سطر 1461-1462). تحقق من السياق قبل التعديل.

**التغيير 3 — Grand Total rendering** (السطور ~1843, ~1853):
استبدل:
```javascript
var val = formatMoney(toNum(card.kpiGrandTotal));
var valYtd = formatMoney(toNum(card.kpiYearToDateTotal));
```
بـ (مع إضافة access إلى card.valueFormatType):
```javascript
var val = formatKpiValue(toNum(card.kpiGrandTotal), card.valueFormatType, card.valueUnit, false);
var valYtd = formatKpiValue(toNum(card.kpiYearToDateTotal), card.valueFormatType, card.valueUnit, false);
```

**التغيير 4 — Sparkline tooltip** (السطر ~1687):
استبدل:
```javascript
return formatMoney ? formatMoney(toNum(val)) : toNum(val).toLocaleString('ar-SA');
```
بـ:
```javascript
return formatKpiValue ? formatKpiValue(toNum(val), formatType, unit, false) : toNum(val).toLocaleString('ar-SA');
```
*ملاحظة:* يجب أن تكون دالة tooltip قادرة على الوصول إلى `formatType` و `unit`. تأكد من تمرير `card.valueFormatType` و `card.valueUnit` أو تعريفهما في الـ closure المناسب.

**التغيير 5 — Category Breakdown** (السطر ~1824):
استبدل:
```javascript
html += '<td class="wd-kpi-breakdown__val">' + wdKpiMoneyHtml(formatMoney(toNum(value))) + '</td>';
```
بـ:
```javascript
html += '<td class="wd-kpi-breakdown__val">' + wdKpiMoneyHtml(formatKpiValue(toNum(value), card.valueFormatType, card.valueUnit, false)) + '</td>';
```

**التغيير 6 — Generic grid/table format** (السطر ~2380):
استبدل:
```javascript
var displayVal = val != null ? (typeof formatMoney === 'function' ? formatMoney(val) : val) : '—';
```
بـ (تحقق من السياق — قد لا تتعلق بـ KPI مباشرة):
إذا كان ضمن سياق KPI، استخدم `formatKpiValue` بدلاً من `formatMoney`.

---

### 2.3 `Pages/Dashboard/Drill.cshtml` — تحديث الـ KPI Format (اختياري)

إذا كانت صفحة الـ Drill تستخدم `formatNum` مع ` د.أ` (خط 75)، استبدلها بـ `formatKpiValue` مع formatType المناسب.

---

## 3. فلو التكامل الكامل

```
Builder.cshtml (Step 4)
  → المستخدم يختار Currency/Number/Percentage/Custom
  → card-builder.js يخزّن في hidden inputs
  → POST → Builder.cshtml.cs يحفظ في DB

DashboardService.cs
  → يقرأ card.ValueFormatType + card.ValueUnit
  → يمرّرها في CardDataResult.ValueFormatType + ValueUnit

Index.cshtml (wdRenderKpiCard)
  → يقرأ card.valueFormatType + card.valueUnit
  → يستخدم formatKpiValue() لعرض القيمة بالتنسيق الصحيح
```

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\dashboard-utils.js`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Dashboard\Drill.cshtml` (optional)

---

## 5. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\dashboard-utils.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

---

## 6. Acceptance Criteria

| # | المعيار | طريقة التحقق |
|---|---------|-------------|
| AC-1 | Currency → القيمة مع د.أ (مختصر S / كامل M/L) | عرض المبلغ مع د.أ |
| AC-2 | Number → القيمة بدون أي لاحقة | عرض الرقم فقط |
| AC-3 | Percentage → القيمة مع % | عرض النسبة مع % |
| AC-4 | Custom + "وحدة" → القيمة مع "وحدة" | عرض الرقم + مسافة + وحدة |
| AC-5 | Size S يستخدم الصيغة المختصرة (K/M/B) لـ Currency/Number/Custom | `14.7K` بدلاً من `14,700` |
| AC-6 | Size M/L يستخدم الصيغة الكاملة | `14,700,000.000` بدلاً من `14.7M` |
| AC-7 | البطاقات القديمة (بدون ValueFormatType) تظهر كـ Currency افتراضياً | `د.أ` يظهر |
| AC-8 | Dotnet build ينجح | `dotnet build` PASS |

---

## 7. Security Sensitivity

- **Level:** Low
- **Reason:** Display formatting only. No auth, no secrets, no DB changes.

---

## 8. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Frontend display formatting |
| One objective only | PASS | Use ValueFormatType in dashboard render |
| Allowed Write Targets are narrow | PASS | 2-3 files |
| Acceptance criteria are testable | PASS | Visual inspection |

**Gate Status:** ✅ PASS

---

## 9. Delegation Instructions

1. **قبل تعديل أي ملف، اقرأ الملف الحالي من القرص أولاً.**
2. احتفظ بأي تغييرات غير مرتبطة.
3. أضف `formatKpiValue()` في `dashboard-utils.js` داخل IIFE (قبل إغلاق `})`).
4. قم بتصديرها مع الدوال الأخرى (بجانب `window.formatMoney = formatMoney`).
5. في `Index.cshtml`، ابحث عن `wdRenderKpiCard(card)` وعدّل **كل** استخدامات `formatMoney` و `formatNum` مع ` د.أ` في سياق KPI لتستخدم `formatKpiValue` بدلاً من ذلك.
6. تذكّر: `card.valueFormatType` و `card.valueUnit` أصبحا متاحين في كائن `card` بفضل TASK-BUILDER-BEH-002.
7. في `wdRenderGrandTotal`، الـ `card` متاح أيضاً — استخدم `card.valueFormatType` و `card.valueUnit`.
8. حرصاً على التوافقية: إذا كان `card.valueFormatType` غير موجود (بطاقة قديمة)، استخدم `'Currency'` كقيمة افتراضية.
9. تأكد من أن `formatKpiValue` تتعامل مع حالة `n === 0` بشكل مناسب لكل formatType.
10. بعد كل التعديلات، نفذ `dotnet build` وتأكد من PASS.
11. أرجع build output + ملخص التعديلات في الـ Handback.

---

## 10. Handback

| البند | القيمة |
|---|---|
| **الحالة** | ✅ Accepted |
| **التاريخ** | 2026-07-21 |
| **المعرف** | TASK-BUILDER-UI-003 |
| **التنفيذ** | engineering-agent-dotnet |

### التعديلات المنفذة (ملفان)

**1. `dashboard-utils.js` — دالة `formatKpiValue()` جديدة (أسطر 51-86):**
- تدعم 4 تنسيقات: Currency → `د.أ`, Number → بدون لاحقة, Percentage → `%`, Custom → نص مخصص
- Size S تستخدم `formatNum` (مختصر K/M/B)
- Size M/L تستخدم الفاصلة الكاملة + 3 خانات عشرية
- القيم الصفرية تحصل على تنسيق مناسب لكل نوع
- تم تصديرها كـ `window.formatKpiValue`

**2. `Pages/Index.cshtml` — 6 مواقع تم تحديثها:**

| الموقع | التغيير |
|---|---|
| `animateCountUp()` | يمرر formatType/unit ويستخدم `formatKpiValue` |
| `wdRenderKpiCard` hero value | يستخدم `card.valueFormatType` بدلاً من formatMoney+د.أ الثابت |
| Sparkline tooltip | يستخدم `formatKpiValue` مع formatType |
| Category Breakdown | يستخدم `formatKpiValue` مع formatType |
| Grand Total | يقرأ `card.valueFormatType` ويستخدم `formatKpiValue` |
| All call sites | تمرر `card.valueFormatType \|\| 'Currency'` و `card.valueUnit \|\| ''` |

### Build
✅ **Build succeeded. 0 Error(s) 0 Warning(s)**

### Acceptance Criteria

| # | المعيار | الحالة |
|---|---------|--------|
| AC-1 | Currency → د.أ | ✅ |
| AC-2 | Number → بدون لاحقة | ✅ |
| AC-3 | Percentage → % | ✅ |
| AC-4 | Custom + وحدة → نص مخصص | ✅ |
| AC-5 | Size S مختصر (K/M/B) | ✅ |
| AC-6 | Size M/L كامل | ✅ |
| AC-7 | Old cards → Currency افتراضي | ✅ |
| AC-8 | Build PASS | ✅ |

### Post-Execution Review

| Check | Result |
|---|---|
| Allowed Write Targets | ✅ (ملفان فقط) |
| No secrets | ✅ |
| In scope | ✅ |
| Acceptance criteria met | ✅ (8/8 AC) |
| Auditor Review Decision | NOT_REQUIRED |

---

> **Prepared by:** TeraAgent
> **Delegated to:** engineering-agent-dotnet — 2026-07-21
> **Final Status:** ✅ Accepted — 2026-07-21
