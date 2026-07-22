# TASK-QT-FIX-001 — إصلاح SELECT Builder و JOIN Builder في QueryTester

> **تاريخ الإنشاء:** 2026-07-21
> **الحالة:** Draft
> **المكلّف:** engineering-agent-dotnet

---

## 1. الوصف

إصلاح مشكلتين في صفحة `QueryTester` (`/admin-secure-panel/QueryTester`):

### المشكلة الأولى — SELECT Builder لا يقرأ الأعمدة
- عند اختيار جدول وتحديد أعمدة ثم الضغط على "إنشاء SELECT" → يظهر خطأ "الرجاء اختيار عمود واحد على الأقل"
- زرا "تحديد الكل" و "إلغاء الكل" لا يعملان
- إضافة عمود من شجرة المتصفح لا يعمل

### المشكلة الثانية — JOIN Builder لا تظهر قائمة الأعمدة
- عند اختيار الجدولين في JOIN، لا تظهر قوائم الأعمدة لكل جدول لاختيار شروط ON

---

## 2. السبب الجذري (Root Cause)

### المشكلة الأولى: `document.querySelector` يبحث عن عنصر وليد وليس الحاوية نفسها

دالة `createSortableList()` تجعل الحاوية نفسها تأخذ `className = 'qt-sortable-list'`. لكن الكود يبحث عن `#builderColumns .qt-sortable-list` (بمسافة) مما يعني "ابحث عن عنصر وليد". وبما أن الحاوية نفسها هي `qt-sortable-list`، فإن `querySelector` يرجع `null`.

**المواقع المتأثرة (4 مواقع):**
```javascript
// Index.cshtml lines ~1013, ~1031, ~1038, ~1049
document.querySelector('#builderColumns .qt-sortable-list')  // ← null
document.querySelector('.qt-sortable-list')                  // ← null (ابناً وليس الأب)
container.querySelector('.qt-sortable-list')                 // ← null (نفس المشكلة)
```

### المشكلة الثانية: `onJoinTableChange(this)` حيث `this` = window

في `addJoinClause()` عند إنشاء الـ searchable dropdowns:
```javascript
createSearchableDropdown(table1Wrap, getTableOptions(), function() {
    onJoinTableChange(this);  // ← this = window (ليس table1Wrap)
});
```
المفترض تمرير `table1Wrap` أو `table2Wrap` مباشرة بدلاً من `this`.

---

## 3. التعديلات المطلوبة

**الملف المستهدف:** `Pages/admin-secure-panel/QueryTester/Index.cshtml`

### Fix 1 — SELECT Builder (4 تغييرات)

| # | الدالة | السطر (تقريباً) | الكود الحالي (الخاطئ) | الكود بعد التصحيح |
|---|---|---|---|---|
| 1 | `selectAllBuilderColumns()` | ~1031 | `document.querySelector('#builderColumns .qt-sortable-list')` | `document.getElementById('builderColumns')` |
| 2 | `deselectAllBuilderColumns()` | ~1037 | `document.querySelector('#builderColumns .qt-sortable-list')` | `document.getElementById('builderColumns')` |
| 3 | `generateSelect()` | ~1049 | `document.querySelector('#builderColumns .qt-sortable-list')` | `document.getElementById('builderColumns')` |
| 4 | `addColumnToBuilder()` | ~1020 | `container.querySelector('.qt-sortable-list')` | `container` (لأن container نفسه هو القائمة) |

### Fix 2 — JOIN Builder (تغيير واحد)

| # | الدالة | السطر (تقريباً) | الكود الحالي (الخاطئ) | الكود بعد التصحيح |
|---|---|---|---|---|
| 5 | `addJoinClause()` | ~1571-1576 | `onJoinTableChange(this)` | `onJoinTableChange(table1Wrap)` و `onJoinTableChange(table2Wrap)` |

---

## 4. معايير القبول (Acceptance Criteria)

| # | المعيار |
|---|---|
| ✅ AC1 | اختيار جدول، تحديد أعمدة، الضغط على "إنشاء SELECT" → ينشئ استعلام SELECT صحيح |
| ✅ AC2 | زر "✔ الكل" يحدد كل الأعمدة في SELECT Builder |
| ✅ AC3 | زر "إلغاء الكل" يلغي تحديد كل الأعمدة |
| ✅ AC4 | النقر على عمود من شجرة المتصفح يضيفه ويحدده في قائمة الأعمدة |
| ✅ AC5 | في JOIN Builder، اختيار جدولين → تظهر قوائم أعمدة كل جدول في شروط ON |
| ✅ AC6 | `dotnet build` مع 0 errors و 0 warnings |

---

## 5. Pre-Execution Gate

```
[✅] 1. Allowed Write Targets محددة بوضوح
[✅] 2. المهمة ضمن النطاق المعتمد (Bug Fix)
[✅] 3. لا توجد secrets أو credentials
[✅] 4. لا تغيير في الـ Schema أو الـ API
[✅] 5. Auditor Review: NOT_REQUIRED (تصحيح JavaScript محلي)
```

**Auditor Review Decision:** NOT_REQUIRED — تغييرات JavaScript محصورة في ملف واحد، لا يوجد تغيير في SQL/Schema/API/Auth.

---

## 6. Vitality & Polish Checklist

| البند | الحالة | ملاحظة |
|---|---|---|
| Skeleton Loading | N/A | لا ينطبق — إصلاح أخطاء وليس UI جديد |
| Toast Notifications | ✅ موجودة مسبقاً |
| Connection Status | N/A | لا ينطبق |
| Search | N/A | لا ينطبق |
| Micro-animations | N/A | لا ينطبق |
| Empty States | N/A | لا ينطبق |
| Realistic Data | N/A | لا ينطبق |

---

## 7. Handback (يملأ بعد التنفيذ)

**تاريخ الاستلام:** 2026-07-21
**الحالة:** ✅ Accepted
**Build Result:** PASS (0 errors, 0 warnings)
**ملاحظات:** جميع التعديلات الخمسة تم تطبيقها والتحقق منها بنجاح. Build سليم. تم التعديل على الملف: `Pages/admin-secure-panel/QueryTester/Index.cshtml` فقط.

## 8. Post-Execution Review

| المعيار | النتيجة |
|---|---|
| Allowed Write Targets محترمة | ✅ ملف واحد فقط: Index.cshtml |
| لا Secrets | ✅ |
| ضمن Scope المهمة | ✅ |
| Build PASS (0 errors, 0 warnings) | ✅ |
| جميع ACs مستوفاة | ✅ (AC1-AC6) |

**Auditor Review Decision:** NOT_REQUIRED — تغييرات JavaScript محصورة، لا SQL/Schema/API/Auth

---

## 8. Post-Execution Review

| المعيار | النتيجة |
|---|---|
| Allowed Write Targets محترمة | |
| لا Secrets | |
| ضمن Scope المهمة | |
| Build PASS (0 errors, 0 warnings) | |
| جميع ACs مستوفاة | |
