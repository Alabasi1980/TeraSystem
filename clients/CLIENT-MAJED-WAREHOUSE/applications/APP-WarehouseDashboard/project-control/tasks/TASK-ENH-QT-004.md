# TASK-ENH-QT-004 — JOIN Builder: منشئ استعلامات متعددة الجداول بصرياً

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** High

---

## 1. الوصف

إضافة **JOIN Builder** بصري إلى QueryTester. يسمح للمستخدم باختيار جدولين (أو أكثر)، تحديد نوع JOIN، مطابقة الأعمدة (ON clause)، واختيار أعمدة SELECT — ثم توليد SQL كامل.

**لا يحتاج أي تغيير في الـ Backend** — الـ Schema API موجود ✅، فقط Frontend (+ توليد SQL في JS).

---

## 2. مكان الإضافة

أضف قسم JOIN Builder **بين** SELECT Builder و WHERE Builder في `Index.cshtml`:

```
[Source Tabs]
[Schema Browser] | [SELECT Builder]
                  | [🔗 JOIN Builder] ← جديد
                  | [WHERE Builder]
                  | [SQL Editor]
                  | [Results]
```

---

## 3. التصميم والوظائف

### A. هيكل القسم
```html
<section class="wd-card qt-join-builder">
  <div class="wd-card__header">
    <span class="wd-card__title">🔗 منشئ JOIN</span>
    <span class="wd-badge">اختياري</span>
  </div>
  <div class="wd-card__body">
    <div id="joinClauses">
      <!-- JOIN clauses will be added here -->
    </div>
    <div class="qt-join-actions">
      <button class="qt-btn qt-btn--sm" onclick="addJoinClause()">➕ إضافة JOIN</button>
      <button class="qt-btn qt-btn--primary qt-btn--sm" onclick="generateJoinQuery()">✨ إنشاء الاستعلام</button>
    </div>
    
    <!-- Output columns selector (appears after tables are selected) -->
    <div class="qt-join-columns" id="joinColumns" hidden>
      <div class="qt-join-columns__header">أعمدة SELECT:</div>
      <div class="qt-join-columns__grid" id="joinColumnsGrid"></div>
    </div>
  </div>
</section>
```

### B. JOIN Clause Row
كل JOIN يتكون من:
```
┌─────────────────────────────────────────────────────────────┐
│ [الجدول الأساسي ▼] [AS a]  [نوع JOIN ▼]  [الجدول الثاني ▼] [AS b] │
│                                                              │
│ ON: [a.عمود ▼]  [عامل ▼]  [b.عمود ▼]       [➕][🗑]          │
│     [a.عمود ▼]  [عامل ▼]  [b.عمود ▼]       [🗑]             │
│     [➕ إضافة شرط]                                              │
│                                                              │
│ 🗑 حذف هذا JOIN                                               │
└─────────────────────────────────────────────────────────────┘
```

**أنواع JOIN:**
- `INNER JOIN`
- `LEFT JOIN`
- `RIGHT JOIN`
- `FULL OUTER JOIN`
- `CROSS JOIN` (يخفي ON conditions)

**عوامل المقارنة في ON:**
- `=` (الأكثر استخداماً)
- `<>`
- `>`, `<`, `>=`, `<=`

### C. اختيار أعمدة SELECT
عند اختيار جدولين، تظهر **قائمة بأعمدة كلا الجدولين** مع Checkboxes لتحديد ما نريد في SELECT:
```
☐ a.ID      ☑ a.ItemName    ☑ a.Price
☐ b.ID      ☑ b.Quantity    ☑ b.Total     ☐ b.Discount
[✔ الكل] [إلغاء الكل]
```

البريـد يسبق اسم العمود (`a.` / `b.`) حتى نعرف مصدر كل عمود.

### D. زر إنشاء الاستعلام
زر `✨ إنشاء الاستعلام` يولد SQL كامل ويضعه في textarea:
```sql
SELECT a.ItemName, a.Price, b.Quantity, b.Total
FROM Items a
INNER JOIN Sales b ON a.ID = b.ItemID
WHERE ...
```

**آلية التوليد:**
1. بناء SELECT من الأعمدة المختارة
2. FROM + alias للجدول الأول
3. JOIN + نوع + alias للجدول الثاني
4. ON مع الشروط المحددة
5. إلحاق أي SQL موجود في textarea (إذا كان هناك WHERE من WHERE Builder)

---

## 4. JavaScript Functions المطلوبة

### `addJoinClause()`
- تنشئ DIV جديد لـ JOIN clause
- تملأ dropdowns الجداول من الـ Schema Browser data (currentTables)
- dropdowns الأعمدة تتعبأ عند اختيار جدول
- أزرار: ➕ إضافة شرط ON جديد، 🗑 حذف الشرط، 🗑 حذف JOIN كامل

### `populateJoinTableDropdowns()`
- تملأ جميع `<select>` الخاصة بالجداول في JOIN clauses بقائمة الجداول من currentSource
- تُستدعى عند تغيير المصدر (source tabs)

### `onJoinTableChange(selectEl)`
- عند اختيار جدول: تملأ `<select>` الخاصة بالأعمدة
- تحديث قسم "أعمدة SELECT" بالأعمدة الجديدة

### `onJoinTableChange2(selectEl)`
- نفس الشيء للجدول الثاني في JOIN

### `addJoinOnCondition(joinEl)`
- تضيف صف ON جديد داخل JOIN clause (عمود + عامل + عمود)

### `removeJoinOnCondition(btn)`
- تحذف صف ON

### `removeJoinClause(btn)`
- تحذف JOIN clause كامل

### `updateSelectColumns()`
- تقرأ جميع الجداول المختارة في JOIN clauses
- تعرض Checkboxes للأعمدة (مجموعة حسب الجدول)
- تسمح باختيار الكل / إلغاء الكل

### `generateJoinQuery()`
1. تقرأ جميع JOIN clauses
2. لكل JOIN: table1 + alias1 + joinType + table2 + alias2 + ON conditions
3. تقرأ الأعمدة المختارة من `joinColumnsGrid`
4. تبني SQL: `SELECT a.col1, a.col2, b.col1 FROM table1 a INNER JOIN table2 b ON ...`
5. تضعه في `textarea#sqlInput`
6. تظهر Toast نجاح

---

## 5. CSS Styles

أضف في نهاية بلوك `<style>`:

```css
/* ===== JOIN Builder ===== */
.qt-join-builder { margin-bottom: 16px; }
.qt-join-clause {
    border: 1px solid var(--c-border);
    border-radius: var(--radius-md);
    padding: 12px 16px;
    margin-bottom: 12px;
    background: var(--c-surface);
    animation: wdFadeUp var(--dur-norm) var(--ease) both;
}
.qt-join-clause__header {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 12px;
    flex-wrap: wrap;
}
.qt-join-clause__select {
    padding: 6px 10px;
    border: 1px solid var(--c-border);
    border-radius: var(--radius-sm);
    font-family: var(--font-ar);
    font-size: 13px;
    background: var(--c-surface);
    min-width: 130px;
}
.qt-join-clause__alias {
    width: 50px;
    padding: 6px 8px;
    border: 1px solid var(--c-border);
    border-radius: var(--radius-sm);
    font-family: monospace;
    font-size: 12px;
    text-align: center;
}
.qt-join-type {
    font-weight: 700;
    color: var(--c-primary);
    font-size: 13px;
    padding: 0 4px;
}

.qt-join-conditions {
    margin-bottom: 8px;
}
.qt-join-condition {
    display: flex;
    align-items: center;
    gap: 6px;
    margin-bottom: 6px;
    flex-wrap: wrap;
    padding: 6px 8px;
    background: var(--c-bg);
    border-radius: var(--radius-sm);
}
.qt-join-condition select {
    padding: 4px 8px;
    border: 1px solid var(--c-border);
    border-radius: var(--radius-sm);
    font-size: 12px;
    min-width: 100px;
    background: var(--c-surface);
}
.qt-join-condition__op {
    min-width: 60px !important;
}

.qt-join-actions {
    display: flex;
    gap: 8px;
    margin-top: 8px;
}

.qt-join-columns {
    margin-top: 16px;
    padding-top: 12px;
    border-top: 1px solid var(--c-border);
}
.qt-join-columns__header {
    font-size: 13px;
    font-weight: 700;
    color: var(--c-text);
    margin-bottom: 8px;
}
.qt-join-columns__grid {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
}
.qt-join-columns__grid label {
    display: flex;
    align-items: center;
    gap: 4px;
    font-size: 12px;
    padding: 4px 10px;
    background: var(--c-surface-muted);
    border: 1px solid var(--c-border);
    border-radius: var(--radius-sm);
    cursor: pointer;
    transition: border-color var(--dur-fast) var(--ease);
}
.qt-join-columns__grid label:hover {
    border-color: var(--c-primary);
}

/* Responsive */
@media (max-width: 768px) {
    .qt-join-clause__header { flex-direction: column; align-items: stretch; }
    .qt-join-clause__select { width: 100%; }
    .qt-join-condition { flex-direction: column; align-items: stretch; }
    .qt-join-condition select { width: 100%; }
}
```

---

## 6. بيانات Schema (متاحة)

```javascript
// مصدر الجداول (من Schema Browser)
const currentTables = []; // يتم تعبئته من Schema Browser
const source = currentSource; // "SqlServer" أو "Oracle"

// لجلب الجداول
fetch('?handler=Tables&source=' + currentSource)
  .then(r => r.json())
  .then(tables => { ... });
// tables = [{ schema, tableName }]

// لجلب الأعمدة لجدول
fetch('?handler=Columns&source=' + currentSource + '&table=' + tableName)
  .then(r => r.json())
  .then(cols => { ... });
// cols = [{ name, dataType, nullable }]
```

---

## 7. Allowed Write Target

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml`

**ممنوع تعديل أي ملف آخر** (خاصة Index.cshtml.cs، Schema Browser، إلخ).

---

## 8. Acceptance Criteria

1. ✅ إضافة JOIN Builder كقسم جديد بين SELECT Builder و WHERE Builder
2. ✅ إضافة JOIN clause: جدول أول + alias + نوع JOIN + جدول ثاني + alias
3. ✅ 5 أنواع JOIN: INNER, LEFT, RIGHT, FULL, CROSS
4. ✅ ON conditions: أعمدة + عوامل مقارنة
5. ✅ إضافة/حذف ON conditions
6. ✅ إضافة/حذف JOIN clauses متعددة
7. ✅ اختيار أعمدة SELECT (Checkboxes مع اسم الجدول prefix)
8. ✅ زر "✨ إنشاء الاستعلام" يولد SQL كامل ويضعه في textarea
9. ✅ CROSS JOIN يخفي ON conditions تلقائياً
10. ✅ `dotnet build` PASS — 0 errors, 0 warnings

---

## 9. ⚠️ قواعد مهمة

- **حافظ على كل الوظائف الحالية:** Schema Browser، SELECT Builder، WHERE Builder، Source Tabs، Editor، Results، History، Shortcuts
- **لا تلمس** أي JS أو CSS موجود — فقط أضف الجديد
- **اقرأ الملف من القرص أولاً**
- لا تغيير في الـ Backend
- جميع الـ Schema APIs موجودة وتعمل

---

## 10. After Completion

أعد:
1. ملخص التغييرات (ماذا أضفت)
2. شرح لكيفية عمل JOIN Builder
3. تأكيد `dotnet build` PASS
4. Auditor Decision: NOT_REQUIRED
