# TASK-ENH-QT-007 — أعمدة Sortable مع ترتيب ← ينعكس في SQL

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** High

---

## 1. الوصف

استبدال عرض الأعمدة (Checkboxes) في **SELECT Builder** و **JOIN Builder** بقوائم **Sortable** يمكن ترتيبها بـ ↑↓، والترتيب ينعكس في ترتيب SELECT في SQL.

---

## 2. المواقع المستهدفة

| الموقع | الوضع الحالي | المطلوب |
|--------|-------------|---------|
| SELECT Builder — أعمدة (`#builderColumns`) | Checkboxes في grid | قائمة Sortable مع ↑↓ |
| JOIN Builder — أعمدة SELECT (`#joinColumnsGrid`) | Checkboxes في grid | قائمة Sortable مع ↑↓ |

---

## 3. التصميم

### A. هيكل القائمة Sortable

```
┌─────────────────────────────────────────────────────────┐
│ [✔ الكل] [إلغاء الكل]                                   │
│                                                          │
│ ⠿ ☑ a.ItemName     (nvarchar)    [↑] [↓]               │
│ ⠿ ☑ a.Price        (decimal)     [↑] [↓]               │
│ ⠿ ☐ a.Category     (nvarchar)    [↑] [↓] ← غير محدد    │
│ ⠿ ☑ b.Quantity     (int)         [↑] [↓]               │
│ ⠿ ☑ b.SaleDate     (datetime)    [↑] [↓]               │
└─────────────────────────────────────────────────────────┘
```

كل عنصر يحتوي على:
- `⠿` أيقونة ترتيب
- ☑ Checkbox (تحديد/إلغاء)
- اسم العمود مع الـ alias prefix
- نوع البيانات بين قوسين (لون ثانوي)
- ↑ زر للرفع (تعطيل إذا كان الأول)
- ↓ زر للخفض (تعطيل إذا كان الأخير)

### B. CSS
```css
/* ===== Sortable Column List ===== */
.qt-sortable-list {
  display: flex;
  flex-direction: column;
  gap: 4px;
  max-height: 300px;
  overflow-y: auto;
  padding: 4px;
}
.qt-sortable-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 10px;
  background: var(--c-surface);
  border: 1px solid var(--c-border);
  border-radius: var(--radius-sm);
  transition: background var(--dur-fast), border-color var(--dur-fast);
  font-size: 13px;
}
.qt-sortable-item:hover {
  background: var(--c-surface-muted);
  border-color: var(--c-secondary);
}
.qt-sortable-item .qt-sortable-handle {
  color: var(--c-text-muted);
  cursor: grab;
  font-size: 14px;
  user-select: none;
  opacity: 0.5;
}
.qt-sortable-item:hover .qt-sortable-handle {
  opacity: 1;
}
.qt-sortable-item .qt-sortable-checkbox {
  margin: 0;
  cursor: pointer;
}
.qt-sortable-item .qt-sortable-label {
  flex: 1;
  color: var(--c-text);
  font-weight: 500;
}
.qt-sortable-item .qt-sortable-type {
  color: var(--c-text-muted);
  font-size: 11px;
  margin: 0 8px;
}
.qt-sortable-item .qt-sortable-btn {
  width: 24px;
  height: 24px;
  border: none;
  background: transparent;
  color: var(--c-text-muted);
  cursor: pointer;
  border-radius: var(--radius-sm);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  transition: background var(--dur-fast), color var(--dur-fast);
}
.qt-sortable-item .qt-sortable-btn:hover {
  background: var(--c-surface-muted);
  color: var(--c-primary);
}
.qt-sortable-item .qt-sortable-btn:disabled {
  opacity: 0.2;
  cursor: default;
}
.qt-sortable-item .qt-sortable-btn:disabled:hover {
  background: transparent;
  color: var(--c-text-muted);
}
```

### C. JavaScript — دالة createSortableList

```javascript
// ---- Create a sortable column list ----
// container: the div element to mount into
// columns: array of { value, label, type, checked }
// onChange: callback(sortedColumns) — returns array of { value, label, checked } in display order
function createSortableList(container, columns, onChange) {
  container.innerHTML = '';
  container.className = 'qt-sortable-list';
  
  var items = columns.map(function(col) {
    return { value: col.value, label: col.label, type: col.type || '', checked: col.checked !== false };
  });
  
  function render() {
    var html = items.map(function(item, idx) {
      var checkedAttr = item.checked ? 'checked' : '';
      var upDisabled = idx === 0 ? 'disabled' : '';
      var downDisabled = idx === items.length - 1 ? 'disabled' : '';
      return '<div class="qt-sortable-item" data-index="' + idx + '">' +
        '<span class="qt-sortable-handle">⠿</span>' +
        '<input type="checkbox" class="qt-sortable-checkbox" ' + checkedAttr + ' data-idx="' + idx + '">' +
        '<span class="qt-sortable-label">' + escapeHtml(item.label) + '</span>' +
        (item.type ? '<span class="qt-sortable-type">(' + escapeHtml(item.type) + ')</span>' : '') +
        '<button class="qt-sortable-btn qt-sortable-up" ' + upDisabled + ' data-idx="' + idx + '" title="رفع">↑</button>' +
        '<button class="qt-sortable-btn qt-sortable-down" ' + downDisabled + ' data-idx="' + idx + '" title="خفض">↓</button>' +
        '</div>';
    }).join('');
    container.innerHTML = html;
    
    // Recalculate checked states
    container.querySelectorAll('.qt-sortable-checkbox').forEach(function(cb) {
      var idx = parseInt(cb.dataset.idx);
      cb.checked = items[idx].checked;
    });
  }
  
  // Event delegation
  container.addEventListener('click', function(e) {
    var btn = e.target.closest('.qt-sortable-btn');
    if (btn) {
      var idx = parseInt(btn.dataset.idx);
      if (btn.classList.contains('qt-sortable-up') && idx > 0) {
        // Move item up
        var temp = items[idx];
        items[idx] = items[idx - 1];
        items[idx - 1] = temp;
        render();
        notifyChange();
      } else if (btn.classList.contains('qt-sortable-down') && idx < items.length - 1) {
        // Move item down
        var temp = items[idx];
        items[idx] = items[idx + 1];
        items[idx + 1] = temp;
        render();
        notifyChange();
      }
    }
    
    var cb = e.target.closest('.qt-sortable-checkbox');
    if (cb) {
      var idx = parseInt(cb.dataset.idx);
      items[idx].checked = cb.checked;
      notifyChange();
    }
  });
  
  // Select all / deselect all
  container.selectAll = function(select) {
    items.forEach(function(item) { item.checked = select; });
    render();
    notifyChange();
  };
  
  // Get selected columns in order
  container.getSelected = function() {
    return items.filter(function(item) { return item.checked; }).map(function(item) { return item.value; });
  };
  
  // Get all items (for external use)
  container.getItems = function() {
    return items;
  };
  
  function notifyChange() {
    if (onChange) onChange(container.getSelected());
  }
  
  render();
  return container;
}
```

---

## 4. التغييرات المطلوبة

### A. SELECT Builder — استبدال `#builderColumns`

**استبدال:**
```html
<div class="qt-builder-columns" id="builderColumns">
  <span class="qt-builder-hint">اختر جدولاً لاختيار الأعمدة.</span>
</div>
```
**بـ:**
```html
<div style="margin-bottom:6px;display:flex;gap:8px">
  <button class="qt-btn qt-btn--xs qt-btn--ghost" onclick="selectAllBuilderCols(true)" type="button">✔ الكل</button>
  <button class="qt-btn qt-btn--xs qt-btn--ghost" onclick="selectAllBuilderCols(false)" type="button">إلغاء الكل</button>
</div>
<div class="qt-builder-columns" id="builderColumns">
  <span class="qt-builder-hint">اختر جدولاً لاختيار الأعمدة.</span>
</div>
```

**تحديث `loadBuilderColumns()`:**

استبدل توليد HTML للـ Checkboxes باستخدام `createSortableList`:

```javascript
function loadBuilderColumns() {
  var tableName = document.getElementById('builderTableWrapper').getValue();
  var container = document.getElementById('builderColumns');
  
  if (!tableName) {
    container.innerHTML = '<span class="qt-builder-hint">اختر جدولاً لاختيار الأعمدة.</span>';
    return;
  }
  
  // Check cache first, then fetch if needed
  function renderColumns() {
    if (!joinColumnCache[tableName]) {
      container.innerHTML = '<span class="qt-builder-hint"><span class="qt-spinner"></span> جارٍ التحميل...</span>';
      fetchAndCacheColumns(tableName, function() { renderColumns(); });
      return;
    }
    
    var cols = joinColumnCache[tableName].map(function(col) {
      return { value: col.name, label: col.name, type: col.dataType, checked: true };
    });
    
    container.innerHTML = '';
    createSortableList(container, cols);
  }
  
  renderColumns();
}

function selectAllBuilderCols(select) {
  var list = document.querySelector('#builderColumns .qt-sortable-list');
  if (list && list.selectAll) list.selectAll(select);
}
```

**تحديث `generateSelect()`:**

اقرأ الأعمدة المختارة بالترتيب من sortable list:

```javascript
function generateSelect() {
  var tableName = document.getElementById('builderTableWrapper').getValue();
  if (!tableName) {
    showToast('الرجاء اختيار جدول أولاً.', 'warning');
    return;
  }
  
  var list = document.querySelector('#builderColumns .qt-sortable-list');
  var selectedCols = list ? list.getSelected() : [];
  
  if (selectedCols.length === 0) {
    showToast('الرجاء اختيار عمود واحد على الأقل.', 'warning');
    return;
  }
  
  var sql = 'SELECT\n  ' + selectedCols.join(',\n  ') + '\nFROM ' + tableName;
  editor.setValue(sql);
  showToast('تم إنشاء استعلام SELECT.', 'success');
  saveToHistory(editor.getValue(), currentSource, 0);
}
```

### B. JOIN Builder — استبدال `#joinColumnsGrid`

**تحديث `updateJoinColumns()`:**

استبدل توليد HTML الـ checkboxes الحالي بـ `createSortableList`:

```javascript
function updateJoinColumns() {
  var clauses = document.querySelectorAll('.qt-join-clause');
  var allCols = [];
  
  clauses.forEach(function(c) {
    var t1Wrap = c.querySelector('.qt-join-table1-wrapper');
    var t2Wrap = c.querySelector('.qt-join-table2-wrapper');
    var table1 = t1Wrap ? t1Wrap.getValue() : '';
    var table2 = t2Wrap ? t2Wrap.getValue() : '';
    
    var aliasInputs = c.querySelectorAll('.qt-join-clause__alias');
    var alias1 = aliasInputs.length > 0 ? aliasInputs[0].value : 'a';
    var alias2 = aliasInputs.length > 1 ? aliasInputs[1].value : 'b';
    
    // Collect columns from table1
    if (table1 && joinColumnCache[table1]) {
      joinColumnCache[table1].forEach(function(col) {
        allCols.push({ value: alias1 + '.' + col.name, label: alias1 + '.' + col.name, type: col.dataType, checked: true });
      });
    }
    
    // Collect columns from table2
    if (table2 && joinColumnCache[table2]) {
      joinColumnCache[table2].forEach(function(col) {
        allCols.push({ value: alias2 + '.' + col.name, label: alias2 + '.' + col.name, type: col.dataType, checked: true });
      });
    }
  });
  
  var joinColumns = document.getElementById('joinColumns');
  var grid = document.getElementById('joinColumnsGrid');
  
  if (allCols.length === 0) {
    joinColumns.hidden = true;
    return;
  }
  
  joinColumns.hidden = false;
  grid.innerHTML = '';
  createSortableList(grid, allCols);
}
```

**تحديث `generateJoinQuery()`:**

استبدل قراءة selected columns لاستخدام `getSelected()` من sortable list:

```javascript
// Inside generateJoinQuery(), find where columns are collected.
// Replace checkbox scanning with:
var colList = document.querySelector('#joinColumnsGrid .qt-sortable-list');
var selectedCols = colList ? colList.getSelected() : [];
if (selectedCols.length > 0) {
  selectPart = selectedCols.join(', ');
} else {
  selectPart = alias1 + '.*'; // fallback
}
```

### C. تحديث selectAllJoinCols

```javascript
function selectAllJoinCols(select) {
  var list = document.querySelector('#joinColumnsGrid .qt-sortable-list');
  if (list && list.selectAll) list.selectAll(select);
}
```

---

## 5. ⚠️ قواعد

- **لا مكتبات خارجية** — ↑↓ أزرار بدلاً من drag & drop (أبسط وأدق)
- **لا تغيير في الـ Backend**
- **اقرأ الملف من القرص أولاً**
- حافظ على كل الوظائف الحالية
- طريقة التوليد (generateSelect, generateJoinQuery, applyWhere) يجب أن تبقى تعمل

---

## 6. Allowed Write Target

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml`

---

## 7. Acceptance Criteria

1. ✅ SELECT Builder: أعمدة Sortable مع ↑↓ + checkbox + type
2. ✅ JOIN Builder: أعمدة SELECT Sortable مع ↑↓ + checkbox + type
3. ✅ ↑↓ تغير الترتيب في القائمة
4. ✅ الترتيب ينعكس في `generateSelect()` (ترتيب أعمدة SELECT)
5. ✅ الترتيب ينعكس في `generateJoinQuery()` (ترتيب أعمدة SELECT)
6. ✅ selectAll/دeselectAll تعمل
7. ✅ `dotnet build` PASS

---

## 8. After Completion

أعد:
1. ملخص التغييرات
2. كيف ينعكس الترتيب في SQL
3. `dotnet build` PASS
4. Auditor: NOT_REQUIRED
