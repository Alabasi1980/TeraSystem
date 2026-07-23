# TASK-DRILL-SMARTTABLE-001 — الجدول الذكي في مودال التنقل العميق

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Approved for execution
> **المكلّف:** ui-designer
> **أولوية:** High

---

## 1. الوصف

تحسين عرض الجدول في مودال Drill-down بإضافة:
1. **Sort** — ترتيب بالضغط على رأس العمود ▲/▼
2. **Search** — حقل بحث داخل المودال لتصفية الصفوف
3. **Pagination** — 25/50/100 صف لكل صفحة
4. **Column Summaries** — SUM/AVG/COUNT في footer الجدول
5. **تنسيق خلايا** — أرقام ← toLocaleString، تواريخ ← ar-SA، null ← —

---

## 2. الملف المستهدف

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

**⚠️ ملف ضخم (~3200 سطر). اقرأه من القرص أولاً. لا تلمس أي وظيفة خارج المودال.**

---

## 3. هيكل المودال بعد التحديث

```html
<div id="wd-drill-modal" class="wd-modal" hidden>
  <div class="wd-modal__overlay"></div>
  <div class="wd-modal__panel wd-modal__panel--xl"> <!-- wider panel -->
    <div class="wd-modal__header">
      <h3 class="wd-modal__title">...</h3>
      <button class="wd-modal__close">X</button>
    </div>
    
    <!-- Breadcrumb -->
    <div class="wd-modal__breadcrumb" id="wd-drill-modal-breadcrumb"></div>
    
    <!-- 🆕 Toolbar: Search + Pagination info + Page size -->
    <div class="wd-drill-toolbar" id="wd-drill-toolbar" hidden>
      <div class="wd-drill-toolbar__search">
        <svg width="16" height="16">search icon</svg>
        <input type="text" id="wd-drill-search" placeholder="بحث في الجدول..." />
      </div>
      <div class="wd-drill-toolbar__info" id="wd-drill-toolbar-info"></div>
      <select id="wd-drill-page-size" class="wd-drill-toolbar__size">
        <option value="25">25</option>
        <option value="50" selected>50</option>
        <option value="100">100</option>
      </select>
      <span class="wd-drill-toolbar__label">صف لكل صفحة</span>
    </div>
    
    <!-- Body (rendered by JS) -->
    <div class="wd-modal__body" id="wd-drill-modal-body"></div>
    
    <!-- 🆕 Pagination footer -->
    <div class="wd-drill-pagination" id="wd-drill-pagination" hidden>
      <button class="wd-btn wd-btn--ghost wd-btn--sm" id="wd-page-prev">‹ السابق</button>
      <span id="wd-page-info"></span>
      <button class="wd-btn wd-btn--ghost wd-btn--sm" id="wd-page-next">التالي ›</button>
    </div>
    
    <!-- Footer (existing: prev/next level + export) -->
    <div class="wd-modal__footer" id="wd-drill-modal-footer"></div>
  </div>
</div>
```

---

## 4. التغييرات المطلوبة في JavaScript

### A. إضافة متغيرات الحالة

عند `wdOpenDrill`، أضف إلى `__drillState`:

```javascript
__drillState.sortColumn = null;
__drillState.sortAsc = true;
__drillState.searchQuery = '';
__drillState.pageSize = 50;
__drillState.currentPage = 1;
__drillState.filteredRows = null; // rows after search filter
```

### B. إعادة كتابة `wdRenderGrid()`

استبدل الدالة الحالية بهذه:

```javascript
function wdRenderGrid(viz, data) {
  var cols = data.columns || [];
  var rows = data.rows || [];
  var st = window.__drillState;
  
  // Empty state
  if (cols.length === 0 || rows.length === 0) {
    viz.innerHTML = wdGridEmptyHtml();
    return;
  }
  
  st.filteredRows = null; // reset filter on new data
  
  // Show toolbar + pagination
  document.getElementById('wd-drill-toolbar').hidden = false;
  document.getElementById('wd-drill-pagination').hidden = false;
  
  renderSmartGrid(viz, cols, rows);
}

function renderSmartGrid(viz, cols, allRows) {
  var st = window.__drillState;
  if (!st) return;
  
  // Determine which rows to show (after search + sort)
  var rows = st.searchQuery ? applySearch(allRows, cols, st.searchQuery) : allRows;
  st.filteredRows = rows;
  
  // Sort if column selected
  if (st.sortColumn) {
    rows = sortRows(rows, st.sortColumn, st.sortAsc, cols);
  }
  
  // Paginate
  var totalRows = rows.length;
  var pageSize = st.pageSize;
  var totalPages = Math.max(1, Math.ceil(totalRows / pageSize));
  if (st.currentPage > totalPages) st.currentPage = totalPages;
  var start = (st.currentPage - 1) * pageSize;
  var pageRows = rows.slice(start, start + pageSize);
  
  // Calculate summaries for all filtered rows (not just page)
  var summaries = calcSummaries(cols, rows);
  
  // Build table HTML
  var html = '<div class="wd-table-wrap">';
  html += '<div class="wd-table-scroll">';
  html += '<table class="wd-table wd-table--smart">';
  
  // Header
  html += '<thead><tr>';
  html += '<th class="wd-table__row-num">#</th>';
  cols.forEach(function(c, idx) {
    var sortIcon = st.sortColumn === c ? (st.sortAsc ? ' ▲' : ' ▼') : '';
    html += '<th onclick="wdDrillSort(\'' + escapeHtml(c) + '\')" style="cursor:pointer;white-space:nowrap;background:' + (idx % 2 === 0 ? 'var(--c-primary)' : 'var(--c-primary-strong)') + ';">'
      + escapeHtml(c) + sortIcon + '</th>';
  });
  html += '</tr></thead>';
  
  // Body
  html += '<tbody>';
  pageRows.forEach(function(r, idx) {
    var rowNum = start + idx + 1;
    html += '<tr class="wd-table__row">';
    html += '<td class="wd-table__row-num">' + rowNum + '</td>';
    cols.forEach(function(c) {
      html += '<td>' + formatDrillCell(r[c]) + '</td>';
    });
    html += '</tr>';
  });
  html += '</tbody>';
  
  // Summary footer
  if (summaries.length > 0) {
    html += '<tfoot class="wd-table__summaries"><tr>';
    html += '<td></td>'; // row number column
    summaries.forEach(function(s) {
      html += '<td><span class="wd-summary-label">' + s.label + '</span> <span class="wd-summary-value">' + s.value + '</span></td>';
    });
    html += '</tr></tfoot>';
  }
  
  html += '</table>';
  html += '</div>'; // .wd-table-scroll
  html += '</div>'; // .wd-table-wrap
  
  viz.innerHTML = html;
  
  // Update toolbar info
  document.getElementById('wd-drill-toolbar-info').textContent = totalRows + ' صف';
  
  // Update pagination
  document.getElementById('wd-page-info').textContent = 'صفحة ' + st.currentPage + ' من ' + totalPages;
  document.getElementById('wd-page-prev').disabled = st.currentPage <= 1;
  document.getElementById('wd-page-next').disabled = st.currentPage >= totalPages;
}
```

### C. دوال مساعدة جديدة

```javascript
function applySearch(rows, cols, query) {
  var q = query.toLowerCase().trim();
  if (!q) return rows;
  return rows.filter(function(r) {
    return cols.some(function(c) {
      var val = r[c];
      return val != null && String(val).toLowerCase().includes(q);
    });
  });
}

function sortRows(rows, colName, asc, cols) {
  var isNumericCol = isNumericColumn(rows, colName);
  var sorted = rows.slice().sort(function(a, b) {
    var va = a[colName], vb = b[colName];
    if (va == null && vb == null) return 0;
    if (va == null) return 1;
    if (vb == null) return -1;
    if (isNumericCol) return asc ? va - vb : vb - va;
    return asc ? String(va).localeCompare(String(vb)) : String(vb).localeCompare(String(va));
  });
  return sorted;
}

function isNumericColumn(rows, colName) {
  for (var i = 0; i < Math.min(rows.length, 20); i++) {
    var val = rows[i][colName];
    if (val != null && typeof val === 'number') return true;
    if (val != null && typeof val === 'string' && !isNaN(parseFloat(val))) return true;
  }
  return false;
}

function calcSummaries(cols, rows) {
  var summaries = [];
  cols.forEach(function(c) {
    var numericRows = rows.filter(function(r) { return r[c] != null && typeof r[c] === 'number'; });
    if (numericRows.length > 0) {
      var sum = 0, count = 0;
      numericRows.forEach(function(r) { sum += r[c]; count++; });
      var avg = count > 0 ? sum / count : 0;
      summaries.push({
        label: c,
        value: '∑ ' + formatDrillNumber(sum) + ' | μ ' + formatDrillNumber(avg) + ' | # ' + count
      });
    }
  });
  return summaries;
}

function formatDrillCell(val) {
  if (val == null) return '<span class="wd-table__null">—</span>';
  if (typeof val === 'number') return '<span class="wd-table__number">' + formatDrillNumber(val) + '</span>';
  if (typeof val === 'string' && /^\d{4}-\d{2}-\d{2}/.test(val)) {
    return '<span class="wd-table__date">' + val + '</span>';
  }
  return '<span class="wd-table__text">' + escapeHtml(String(val)) + '</span>';
}

function formatDrillNumber(val) {
  if (val == null) return '—';
  return Number(val).toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 2 });
}

function wdDrillSort(colName) {
  var st = window.__drillState;
  if (!st) return;
  if (st.sortColumn === colName) {
    st.sortAsc = !st.sortAsc;
  } else {
    st.sortColumn = colName;
    st.sortAsc = true;
  }
  st.currentPage = 1;
  var viz = document.getElementById('wd-drill-modal-body');
  var data = st.currentData;
  if (viz && data) renderSmartGrid(viz, data.columns || [], data.rows || []);
}
```

### D. تحديث wdOpenDrill لإظهار الـ Toolbar

في `wdOpenDrill()`:

```javascript
// Show toolbar
document.getElementById('wd-drill-toolbar').hidden = false;
document.getElementById('wd-drill-pagination').hidden = false;
```

### E. تحديث wdCloseDrillModal لإخفاء الـ Toolbar

```javascript
document.getElementById('wd-drill-toolbar').hidden = true;
document.getElementById('wd-drill-pagination').hidden = true;
```

### F. إضافة Event Listeners

في `document.addEventListener('DOMContentLoaded', ...)`:

```javascript
// Search
document.getElementById('wd-drill-search').addEventListener('input', function() {
  var st = window.__drillState;
  if (!st) return;
  st.searchQuery = this.value;
  st.currentPage = 1;
  var viz = document.getElementById('wd-drill-modal-body');
  var data = st.currentData;
  if (viz && data) renderSmartGrid(viz, data.columns || [], data.rows || []);
});

// Page size
document.getElementById('wd-drill-page-size').addEventListener('change', function() {
  var st = window.__drillState;
  if (!st) return;
  st.pageSize = parseInt(this.value) || 50;
  st.currentPage = 1;
  var viz = document.getElementById('wd-drill-modal-body');
  var data = st.currentData;
  if (viz && data) renderSmartGrid(viz, data.columns || [], data.rows || []);
});

// Previous page
document.getElementById('wd-page-prev').addEventListener('click', function() {
  var st = window.__drillState;
  if (!st || st.currentPage <= 1) return;
  st.currentPage--;
  var viz = document.getElementById('wd-drill-modal-body');
  var data = st.currentData;
  if (viz && data) renderSmartGrid(viz, data.columns || [], data.rows || []);
});

// Next page
document.getElementById('wd-page-next').addEventListener('click', function() {
  var st = window.__drillState;
  if (!st) return;
  var rows = st.filteredRows || (st.currentData ? st.currentData.rows : []);
  var totalPages = Math.ceil(rows.length / st.pageSize);
  if (st.currentPage >= totalPages) return;
  st.currentPage++;
  var viz = document.getElementById('wd-drill-modal-body');
  var data = st.currentData;
  if (viz && data) renderSmartGrid(viz, data.columns || [], data.rows || []);
});
```

---

## 5. CSS جديد

أضف إلى نهاية `<style>`:

```css
/* ===== Drill Smart Table (TASK-DRILL-SMARTTABLE-001) ===== */
.wd-modal__panel--xl { max-width: 95vw; width: 1400px; }

/* Toolbar */
.wd-drill-toolbar {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px 24px;
  background: var(--c-surface-muted);
  border-bottom: 1px solid var(--c-border);
  flex-wrap: wrap;
}
.wd-drill-toolbar__search {
  display: flex;
  align-items: center;
  gap: 6px;
  flex: 1;
  min-width: 200px;
}
.wd-drill-toolbar__search input {
  flex: 1;
  padding: 6px 10px;
  border: 1px solid var(--c-border);
  border-radius: var(--radius-sm);
  font-family: var(--font-ar);
  font-size: 13px;
  background: var(--c-surface);
}
.wd-drill-toolbar__search input:focus {
  border-color: var(--c-primary);
  outline: none;
  box-shadow: 0 0 0 3px rgba(31,78,121,0.12);
}
.wd-drill-toolbar__info { font-size: 13px; color: var(--c-text-muted); white-space: nowrap; }
.wd-drill-toolbar__label { font-size: 12px; color: var(--c-text-muted); }
.wd-drill-toolbar__size {
  padding: 4px 8px;
  border: 1px solid var(--c-border);
  border-radius: var(--radius-sm);
  font-size: 13px;
  background: var(--c-surface);
}

/* Smart Table */
.wd-table--smart .wd-table__row-num {
  width: 40px;
  text-align: center;
  color: var(--c-text-muted);
  font-size: 12px;
  font-weight: 500;
}
.wd-table__row-num { text-align: center; color: var(--c-text-muted); font-size: 12px; }
.wd-table__number { direction: ltr; text-align: right; display: block; font-variant-numeric: tabular-nums; }
.wd-table__date { direction: ltr; display: block; font-size: 13px; }
.wd-table__null { color: var(--c-text-muted); font-style: italic; }
.wd-table__text { text-align: start; }

/* Column Summaries Footer */
.wd-table__summaries { border-top: 2px solid var(--c-primary); }
.wd-table__summaries td { 
  padding: 8px 12px; 
  font-size: 12px; 
  background: var(--c-surface-muted); 
  vertical-align: middle;
}
.wd-summary-label { font-weight: 600; color: var(--c-text-muted); }
.wd-summary-value { color: var(--c-text); direction: ltr; display: block; font-size: 11px; }

/* Pagination */
.wd-drill-pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 8px 24px;
  background: var(--c-surface-muted);
  border-top: 1px solid var(--c-border);
  font-size: 13px;
  color: var(--c-text-muted);
}
```

---

## 6. تحديث HTML للمودال

استبدل محتوى `<div id="wd-drill-modal">` الحالي بهذا:

```html
<div id="wd-drill-modal" class="wd-modal" hidden aria-modal="true" role="dialog">
  <div class="wd-modal__overlay" tabindex="-1"></div>
  <div class="wd-modal__panel wd-modal__panel--xl">
    <div class="wd-modal__header">
      <h3 class="wd-modal__title" id="wd-drill-modal-title">
        <span class="wd-modal__title-icon">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <line x1="18" y1="20" x2="18" y2="10"/>
            <line x1="12" y1="20" x2="12" y2="4"/>
            <line x1="6" y1="20" x2="6" y2="14"/>
          </svg>
        </span>
        <span id="wd-drill-modal-title-text"></span>
      </h3>
      <button class="wd-modal__close" aria-label="إغلاق" type="button">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="M18 6 6 18"/><path d="m6 6 12 12"/>
        </svg>
      </button>
    </div>
    <div class="wd-modal__breadcrumb" id="wd-drill-modal-breadcrumb"></div>
    
    <!-- 🆕 Smart Table Toolbar -->
    <div class="wd-drill-toolbar" id="wd-drill-toolbar" hidden>
      <div class="wd-drill-toolbar__search">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input type="text" id="wd-drill-search" placeholder="بحث في الجدول..." autocomplete="off" />
      </div>
      <span class="wd-drill-toolbar__info" id="wd-drill-toolbar-info"></span>
      <select id="wd-drill-page-size" class="wd-drill-toolbar__size">
        <option value="25">25</option>
        <option value="50" selected>50</option>
        <option value="100">100</option>
      </select>
      <span class="wd-drill-toolbar__label">صف لكل صفحة</span>
    </div>
    
    <div class="wd-modal__body" id="wd-drill-modal-body"></div>
    
    <!-- 🆕 Pagination -->
    <div class="wd-drill-pagination" id="wd-drill-pagination" hidden>
      <button class="wd-btn wd-btn--ghost wd-btn--sm" id="wd-page-prev" type="button">‹ السابق</button>
      <span id="wd-page-info"></span>
      <button class="wd-btn wd-btn--ghost wd-btn--sm" id="wd-page-next" type="button">التالي ›</button>
    </div>
    
    <div class="wd-modal__footer" id="wd-drill-modal-footer"></div>
  </div>
</div>
```

---

## 7. قواعد

- **لا تلمس أي وظيفة خارج المودال** (لا تلمس KPI cards, charts, filter bar, Sync, etc.)
- **لا تلمس C# backend**
- حافظ على كل الوظائف الحالية (breadcrumb, navigation, export CSV, selection, etc.)
- `wdRenderGrid` دعها كما هي للمودال — لا تؤثر على dashboard cards نفسها

---

## 8. Acceptance Criteria

1. ✅ Sort: ضغط على رأس العمود → ترتيب ▲/▼
2. ✅ Search: input داخل المودال يُصفّي الصفوف
3. ✅ Pagination: 25/50/100 مع أزرار السابق/التالي + رقم الصفحة
4. ✅ Column Summaries: SUM/AVG/COUNT في footer الجدول
5. ✅ Cell Formatting: أرقام ← toLocaleString, null ← —, تواريخ ←
6. ✅ Row numbers (#)
7. ✅ `dotnet build` PASS

---

## 9. After Completion

أعد:
1. ملخص كل تغيير في المودال
2. `dotnet build` PASS
3. Auditor: NOT_REQUIRED
