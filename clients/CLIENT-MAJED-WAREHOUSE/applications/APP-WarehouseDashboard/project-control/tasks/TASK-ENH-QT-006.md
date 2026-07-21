# TASK-ENH-QT-006 — قوائم Searchable لاختيار الجداول والأعمدة

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** High

---

## 1. الوصف

استبدال كل القوائم المنسدلة (`<select>`) في JOIN Builder و SELECT Builder بقوائم **Searchable** — تكتب حرفين وتصفّي القائمة.

**بدون مكتبات خارجية** — JS خالص + CSS.

---

## 2. المواقع المستهدفة

| الموقع | نوع البيانات | الوظيفة |
|--------|-------------|---------|
| JOIN Builder — اختيار الجدول الأول (`qt-join-table1`) | جداول | اختيار جدول للـ JOIN |
| JOIN Builder — اختيار الجدول الثاني (`qt-join-table2`) | جداول | اختيار جدول للـ JOIN |
| JOIN Builder — اختيار العمود في ON (`qt-join-condition__col`) | أعمدة | اختيار عمود للمقارنة |
| JOIN Builder — اختيار العمود في ON (`qt-join-condition__col2`) | أعمدة | اختيار عمود للمقارنة |
| SELECT Builder — اختيار الجدول (`builderTable`) | جداول | اختيار جدول للأعمدة |

---

## 3. التصميم

### A. هيكل Searchable Dropdown

```html
<div class="qt-searchable" data-target="table1_0">
  <div class="qt-searchable__input-wrapper">
    <input class="qt-searchable__input" type="text" placeholder="ابحث عن جدول..." autocomplete="off" />
    <span class="qt-searchable__arrow">▼</span>
  </div>
  <div class="qt-searchable__dropdown" hidden>
    <div class="qt-searchable__option" data-value="Items">dbo.Items</div>
    <div class="qt-searchable__option" data-value="Sales">dbo.Sales</div>
    ...
    <div class="qt-searchable__option" data-value="Stock">dbo.Stock</div>
  </div>
</div>
```

### B. CSS
```css
.qt-searchable {
  position: relative;
  min-width: 150px;
  font-size: 13px;
}
.qt-searchable__input-wrapper {
  display: flex;
  align-items: center;
  border: 1px solid var(--c-border);
  border-radius: var(--radius-sm);
  background: var(--c-surface);
  transition: border-color var(--dur-fast) var(--ease);
}
.qt-searchable__input-wrapper:focus-within {
  border-color: var(--c-primary);
  box-shadow: 0 0 0 3px rgba(31,78,121,0.12);
}
.qt-searchable__input {
  flex: 1;
  border: none;
  outline: none;
  padding: 6px 10px;
  font-family: var(--font-ar);
  font-size: 13px;
  background: transparent;
  color: var(--c-text);
  min-width: 0;
}
.qt-searchable__arrow {
  padding: 0 8px;
  color: var(--c-text-muted);
  font-size: 10px;
  cursor: pointer;
  user-select: none;
}
.qt-searchable__dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  z-index: 1000;
  max-height: 220px;
  overflow-y: auto;
  background: var(--c-surface);
  border: 1px solid var(--c-border);
  border-radius: var(--radius-sm);
  box-shadow: var(--shadow-md);
  margin-top: 2px;
}
.qt-searchable__option {
  padding: 6px 10px;
  cursor: pointer;
  font-size: 13px;
  color: var(--c-text);
  transition: background var(--dur-fast);
}
.qt-searchable__option:hover,
.qt-searchable__option.highlighted {
  background: var(--c-surface-muted);
}
.qt-searchable__option.selected {
  background: rgba(31,78,121,0.1);
  font-weight: 600;
}
.qt-searchable__option.no-results {
  color: var(--c-text-muted);
  font-style: italic;
  cursor: default;
}
.qt-searchable__option .qt-option-type {
  color: var(--c-text-muted);
  font-size: 11px;
  margin-left: 8px;
}
```

### C. JavaScript

أنشئ دالة واحدة مُستخدمة لكل القوائم:

```javascript
// ---- Create a searchable dropdown ----
function createSearchableDropdown(container, options, onSelect) {
  // container: the div element to put the searchable in
  // options: array of { value: string, label: string, type?: string }
  // onSelect: callback function(value, label)

  container.innerHTML = '';
  container.className = 'qt-searchable';
  
  // Input wrapper
  var wrapper = document.createElement('div');
  wrapper.className = 'qt-searchable__input-wrapper';
  
  var input = document.createElement('input');
  input.className = 'qt-searchable__input';
  input.type = 'text';
  input.placeholder = 'ابحث...';
  input.autocomplete = 'off';
  
  var arrow = document.createElement('span');
  arrow.className = 'qt-searchable__arrow';
  arrow.textContent = '▼';
  
  wrapper.appendChild(input);
  wrapper.appendChild(arrow);
  
  // Dropdown
  var dropdown = document.createElement('div');
  dropdown.className = 'qt-searchable__dropdown';
  dropdown.hidden = true;
  
  container.appendChild(wrapper);
  container.appendChild(dropdown);
  
  var selectedValue = '';
  var highlightedIndex = -1;
  var filteredOptions = [];
  
  function renderDropdown() {
    var query = input.value.toLowerCase().trim();
    filteredOptions = options.filter(function(opt) {
      return opt.label.toLowerCase().includes(query);
    });
    
    if (filteredOptions.length === 0) {
      dropdown.innerHTML = '<div class="qt-searchable__option no-results">لا توجد نتائج</div>';
      return;
    }
    
    dropdown.innerHTML = filteredOptions.map(function(opt, idx) {
      var cls = 'qt-searchable__option';
      if (idx === highlightedIndex) cls += ' highlighted';
      if (opt.value === selectedValue) cls += ' selected';
      var typeHtml = opt.type ? ' <span class="qt-option-type">(' + escapeHtml(opt.type) + ')</span>' : '';
      return '<div class="' + cls + '" data-index="' + idx + '">' + escapeHtml(opt.label) + typeHtml + '</div>';
    }).join('');
  }
  
  function openDropdown() {
    highlightedIndex = -1;
    renderDropdown();
    dropdown.hidden = false;
  }
  
  function closeDropdown() {
    dropdown.hidden = true;
  }
  
  function selectItem(value, label) {
    selectedValue = value;
    input.value = label;
    container.dataset.value = value;
    closeDropdown();
    if (onSelect) onSelect(value, label);
  }
  
  // Input events
  input.addEventListener('focus', openDropdown);
  input.addEventListener('input', function() {
    selectedValue = '';
    container.dataset.value = '';
    highlightedIndex = -1;
    openDropdown();
  });
  
  input.addEventListener('keydown', function(e) {
    if (e.key === 'ArrowDown') {
      e.preventDefault();
      highlightedIndex = Math.min(highlightedIndex + 1, filteredOptions.length - 1);
      renderDropdown();
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      highlightedIndex = Math.max(highlightedIndex - 1, -1);
      renderDropdown();
    } else if (e.key === 'Enter') {
      e.preventDefault();
      if (highlightedIndex >= 0 && highlightedIndex < filteredOptions.length) {
        var opt = filteredOptions[highlightedIndex];
        selectItem(opt.value, opt.label);
      }
    } else if (e.key === 'Escape') {
      closeDropdown();
    }
  });
  
  // Dropdown click delegation
  dropdown.addEventListener('click', function(e) {
    var optEl = e.target.closest('.qt-searchable__option');
    if (!optEl || optEl.classList.contains('no-results')) return;
    var idx = parseInt(optEl.dataset.index);
    if (idx >= 0 && idx < filteredOptions.length) {
      var opt = filteredOptions[idx];
      selectItem(opt.value, opt.label);
    }
  });
  
  // Close on outside click
  document.addEventListener('click', function(e) {
    if (!container.contains(e.target)) {
      closeDropdown();
    }
  });
  
  // Arrow click to toggle
  arrow.addEventListener('click', function() {
    if (dropdown.hidden) { openDropdown(); } 
    else { closeDropdown(); }
  });
  
  // Set value programmatically
  container.setValue = function(value) {
    var match = options.find(function(o) { return o.value === value; });
    if (match) {
      selectedValue = match.value;
      input.value = match.label;
      container.dataset.value = match.value;
    } else {
      selectedValue = '';
      input.value = '';
      container.dataset.value = '';
    }
  };
  
  container.getValue = function() {
    return container.dataset.value || '';
  };
}
```

---

## 4. التغييرات المطلوبة في الصفحة

### A. JOIN Builder — استبدال `<select>` للجداول

في `addJoinClause()`:
- استبدل `<select class="qt-join-clause__select qt-join-table1"...>` بـ `<div class="qt-join-table1-wrapper"></div>`
- استبدل `<select class="qt-join-clause__select qt-join-table2"...>` بـ `<div class="qt-join-table2-wrapper"></div>`
- بعد `insertAdjacentHTML`، نادي `createSearchableDropdown(wrapper, tableOptions, callback)` لكل wrap

```javascript
function initJoinTableDropdowns() {
  var tables = window.currentTables || [];
  var tableOptions = tables.map(function(t) {
    var label = t.schema ? t.schema + '.' + t.tableName : t.tableName;
    return { value: t.tableName, label: label, type: t.schema };
  });
  
  document.querySelectorAll('.qt-join-table1-wrapper').forEach(function(wrap) {
    createSearchableDropdown(wrap, tableOptions, function(value) {
      // Trigger table change
      onJoinTableChange(wrap);
    });
  });
  
  document.querySelectorAll('.qt-join-table2-wrapper').forEach(function(wrap) {
    createSearchableDropdown(wrap, tableOptions, function(value) {
      onJoinTableChange(wrap);
    });
  });
}
```

### B. JOIN Builder — استبدال `<select>` للأعمدة في ON

في `addJoinCondition()`:
- استبدل `<select class="qt-join-condition__col">` بـ `<div class="qt-join-condition__col-wrapper"></div>`
- استبدل `<select class="qt-join-condition__col2">` بـ `<div class="qt-join-condition__col2-wrapper"></div>`
- املأها بعد إنشاء الـ row بـ `createSearchableDropdown`

```javascript
function populateConditionCols(row, table1, table2, clauseIndex) {
  var col1Wrapper = row.querySelector('.qt-join-condition__col-wrapper');
  var col2Wrapper = row.querySelector('.qt-join-condition__col2-wrapper');
  
  if (col1Wrapper) {
    var cols1 = getColumnsForTable(table1, clauseIndex, 'a');
    createSearchableDropdown(col1Wrapper, cols1, function() { updateJoinColumns(); });
  }
  if (col2Wrapper) {
    var cols2 = getColumnsForTable(table2, clauseIndex, 'b');
    createSearchableDropdown(col2Wrapper, cols2, function() { updateJoinColumns(); });
  }
}

function getColumnsForTable(tableName, clauseIndex, defaultAlias) {
  if (!tableName || !joinColumnCache[tableName]) return [];
  var alias = getAliasForTable(clauseIndex, tableName, defaultAlias);
  return joinColumnCache[tableName].map(function(col) {
    return { value: alias + '.' + col.name, label: alias + '.' + col.name, type: col.dataType };
  });
}
```

### C. SELECT Builder — استبدال `<select id="builderTable">`

- استبدل `<select id="builderTable">` بـ `<div id="builderTableWrapper"></div>`
- في `populateBuilderDropdown()`، استخدم `createSearchableDropdown` بدلاً من إعداد innerHTML

---

## 5. تحديث الدوال المتأثرة

| الدالة | التأثير |
|--------|---------|
| `onJoinTableChange(selectEl)` | المعامل الآن هو `wrapper` (div) وليس `selectEl`. استخدم `wrapper.getValue()` بدلاً من `selectEl.value` |
| `onJoinTypeChange(selectEl)` | ما زال `<select>` — لا يتأثر |
| `addJoinClause()` | استبدال HTML + إضافة `initJoinTableDropdowns()` بعد `insertAdjacentHTML` |
| `addJoinCondition(btn)` | استبدال `<select>` بـ `<div>` في HTML + تغيير populateConditionCols |
| `populateJoinTableDropdowns()` | تغيير لاستخدام `createSearchableDropdown` |
| `populateBuilderDropdown(tables)` | تغيير لاستخدام `createSearchableDropdown` |
| `loadBuilderColumns()` | قراءة `document.getElementById('builderTableWrapper').getValue()` بدلاً من `builderTable.value` |
| `generateJoinQuery()` | أخذ قيم الجداول من `wrapper.getValue()` |
| `updateJoinColumns()` | قراءة أسماء الجداول من `wrapper.getValue()` |

---

## 6. القواعد

- **لا مكتبات خارجية** — JS خالص
- **لا تغيير في الـ Backend**
- **اقرأ الملف من القرص أولاً**
- حافظ على كل الوظائف الحالية
- الأعمدة في ON condition: col1 تعرض أعمدة table1 فقط، col2 تعرض أعمدة table2 فقط (كما طُبّق سابقاً)

---

## 7. Allowed Write Target

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml`

---

## 8. Acceptance Criteria

1. ✅ JOIN Builder جدول أول/ثاني: searchable dropdown مع تصفية أثناء الكتابة
2. ✅ ON conditions أعمدة: searchable dropdown مع تصفية أثناء الكتابة
3. ✅ SELECT Builder جدول: searchable dropdown
4. ✅ ↑↓ أسهم و Enter و Escape في القوائم
5. ✅ ضغطة خارج القائمة → تقفل
6. ✅ كل الدوال القديمة تشتغل (runQuery, generateJoinQuery, generateSelect, applyWhere...)
7. ✅ `dotnet build` PASS

---

## 9. After Completion

أعد:
1. ملخص كل تغيير (أين تم استبدال select بـ searchable)
2. تحديث الدوال المتأثرة وكيف تغيّرت (خاصة onJoinTableChange, generateJoinQuery)
3. `dotnet build` PASS
4. Auditor: NOT_REQUIRED
