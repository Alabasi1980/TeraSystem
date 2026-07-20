# TASK-FIX-REPORTS-02: JavaScript Cleanup — alert→toast + class names + applyLayout

**Status:** Assigned  
**Priority:** P0 — Critical  
**Type:** JS Fix  
**Created:** 2026-07-20  
**Depends on:** TASK-FIX-REPORTS-01 (completed)  
**Audit Reference:** QUAUD-REPORT-DESIGN-QUALITY-2026-07-20-001 (STOP-003, CAU-004, CAU-005, FLAG-003, FLAG-004)

---

## Objective

Fix the JavaScript in both Reports pages to: (1) replace all `alert()` calls with toast notifications, (2) fix CSS class names in JS-generated HTML, (3) implement `applyLayout()`, (4) add error boundary around AG Grid, (5) extract `escapeHtml()`.

## Context

After TASK-FIX-REPORTS-01, the CSS classes in the `<style>` blocks were updated to use shared classes from `blue-theme.css`/`_CardsLayout`. But the **JavaScript** still generates HTML with old class names that have no matching CSS. Also, all user feedback uses browser `alert()` instead of the toast system.

## Scope — What to Fix

### 1. STOP-003: Replace ALL `alert()` with Toast Notifications

The `_CardsLayout` layout provides a toast container: `<div id="wd-toast-host" class="wd-toast-host" aria-live="polite" aria-atomic="true">`

**Toast HTML structure** (from blue-theme.css):
```html
<div class="wd-toast wd-toast--success">
    <span class="wd-toast__icon">✓</span>
    <span class="wd-toast__msg">Message here</span>
</div>
```

**Create a shared toast function** at the top of each page's `<script>`:
```javascript
function showToast(message, type) {
    type = type || 'success';
    var host = document.getElementById('wd-toast-host');
    if (!host) return;
    var icon = type === 'success' ? '✓' : '✕';
    var toast = document.createElement('div');
    toast.className = 'wd-toast wd-toast--' + type;
    toast.innerHTML = '<span class="wd-toast__icon">' + icon + '</span><span class="wd-toast__msg">' + escapeHtml(message) + '</span>';
    host.appendChild(toast);
    setTimeout(function() { toast.remove(); }, 4000);
}
```

**Replace every `alert()` call:**

In **Reports/Index.cshtml**:
- Line ~935: `alert('الرجاء إدخال اسم للتصميم.');` → `showToast('الرجاء إدخال اسم للتصميم.', 'error');`
- Line ~964: `alert('✅ تم حفظ التصميم بنجاح!');` → `showToast('تم حفظ التصميم بنجاح!', 'success');`
- Line ~968: `alert('❌ خطأ في حفظ التصميم.');` → `showToast('خطأ في حفظ التصميم.', 'error');`
- Line ~971: `alert('❌ تعذر الاتصال بالخادم.');` → `showToast('تعذر الاتصال بالخادم.', 'error');`

In **ReportBuilder/Index.cshtml**:
- Line ~320: `alert('الرجاء إدخال اسم التقرير.');` → `showToast('الرجاء إدخال اسم التقرير.', 'error');`
- Line ~322: `alert('الرجاء اختيار View.');` → `showToast('الرجاء اختيار View.', 'error');`
- Line ~445: `alert('لا توجد أعمدة قابلة للفلترة...');` → `showToast('لا توجد أعمدة قابلة للفلترة...', 'error');`
- Line ~513: `alert('الرجاء اختيار View.');` → `showToast('الرجاء اختيار View.', 'error');`
- Line ~560: `alert('الرجاء إدخال اسم التقرير واختيار View.');` → `showToast('الرجاء إدخال اسم التقرير واختيار View.', 'error');`
- Line ~577: `alert('يجب اختيار عمود واحد على الأقل.');` → `showToast('يجب اختيار عمود واحد على الأقل.', 'error');`
- Line ~606: `alert('✅ تم حفظ التقرير بنجاح!');` → `showToast('تم حفظ التقرير بنجاح!', 'success');`
- Line ~609: `alert('❌ خطأ: ' + ...);` → `showToast('خطأ: ' + ..., 'error');`
- Line ~614: `alert('❌ تعذر الاتصال بالخادم.');` → `showToast('تعذر الاتصال بالخادم.', 'error');`

### 2. Fix CSS Class Names in JS-Generated HTML

**Reports/Index.cshtml** — Replace in JavaScript string concatenation:
- `.wd-empty-state` → `.wd-empty` (3 occurrences in JS)
- `.wd-empty-state__icon` → `.wd-empty__icon` (3 occurrences in JS)
- `.wd-skeleton` → `.wd-skeleton-wrap` (1 occurrence in JS)
- `.wd-skel-row` → `.wd-skel` (5 occurrences in JS — note: the shared class is `.wd-skel` not `.wd-skel-row`)

**Also in Reports/Index.cshtml HTML (static):**
- Line 398-399: Already fixed to `.wd-empty` / `.wd-empty__icon` ✓

### 3. CAU-005: Implement `applyLayout()` Function

**Reports/Index.cshtml** — The `applyLayout()` function (lines 998-1003) is currently a stub:
```javascript
function applyLayout(layoutId) {
    // Load layout details from dropdown - simplified approach
    var sel = document.getElementById('layoutSelect');
    // The layout state is preserved in AG Grid's column state
}
```

**Implement it properly:**
```javascript
async function applyLayout(layoutId) {
    if (!layoutId || !gridApi) return;
    try {
        var resp = await fetch('/api/reports-data/layoutDetail?layoutId=' + layoutId);
        // If the API doesn't have a detail endpoint, fetch from layouts list
        // and find the matching one
        var layouts = [];
        if (currentReportId) {
            var listResp = await fetch('/api/reports-data/layouts?reportId=' + currentReportId);
            layouts = await listResp.json();
        }
        var layout = Array.isArray(layouts) ? layouts.find(function(l) { return l.id == layoutId; }) : null;
        if (!layout) return;

        // Apply column visibility
        if (layout.visibleColumns) {
            try {
                var visibleCols = JSON.parse(layout.visibleColumns);
                visibleCols.forEach(function(vc) {
                    var parts = vc.split(':');
                    if (parts.length === 2) {
                        gridColumnApi.setColumnVisible(parts[0], parts[1] === 'true');
                    }
                });
            } catch(e) {}
        }

        // Apply column widths
        if (layout.columnWidths) {
            try {
                var widths = JSON.parse(layout.columnWidths);
                widths.forEach(function(w) {
                    var parts = w.split(':');
                    if (parts.length === 2) {
                        var col = gridColumnApi.getColumn(parts[0]);
                        if (col) col.setActualWidth(parseInt(parts[1]));
                    }
                });
            } catch(e) {}
        }

        // Apply sort state
        if (layout.sortState) {
            try {
                var sortModel = JSON.parse(layout.sortState);
                gridApi.applySort({ sortModel: sortModel });
            } catch(e) {}
        }

        // Apply filter values to filter bar
        if (layout.filterValues) {
            try {
                var filters = JSON.parse(layout.filterValues);
                // Set filter input values
                Object.keys(filters).forEach(function(key) {
                    // Find the filter input by column name
                    if (currentReportDef && currentReportDef.filters) {
                        currentReportDef.filters.forEach(function(f, idx) {
                            if (f.columnName === key) {
                                var inputId = 'filter-' + f.id + '-' + idx;
                                var el = document.getElementById(inputId);
                                if (el) el.value = filters[key];
                            }
                        });
                    }
                });
            } catch(e) {}
        }

        showToast('تم تحميل التصميم.', 'success');
    } catch(e) {
        console.error('Failed to apply layout', e);
    }
}
```

### 4. FLAG-003: Add Error Boundary Around AG Grid

**Reports/Index.cshtml** — The `new agGrid.Grid(gridDiv, gridOptions)` call (line ~836) has no error handling. If the CDN fails or the grid initialization throws, the page shows a blank area.

**Wrap it:**
```javascript
try {
    new agGrid.Grid(gridDiv, gridOptions);
} catch (e) {
    console.error('AG Grid initialization failed', e);
    container.innerHTML = '<div class="wd-empty"><div class="wd-empty__icon">⚠️</div><h3>خطأ في تحميل الجدول</h3><p>تعذر تحميل مكتبة الجدول. تأكد من اتصالك بالإنترنت.</p></div>';
}
```

### 5. FLAG-004: Extract `escapeHtml()` to Shared Utility

Both pages define their own `escapeHtml()` function. Since they're on separate pages, we can't share a single file. But we should make the implementations identical and add a comment noting they should be unified.

**Ensure both pages use the same implementation:**
```javascript
function escapeHtml(text) {
    if (text == null) return '';
    var d = document.createElement('div');
    d.textContent = String(text);
    return d.innerHTML;
}
```

The Reports/Index.cshtml version is correct. The ReportBuilder/Index.cshtml version (line 429) uses `if (!text) return '';` which is slightly different (falsy values like 0 return ''). Fix it to match:
```javascript
function escapeHtml(text) {
    if (text == null) return '';
    var d = document.createElement('div');
    d.textContent = String(text);
    return d.innerHTML;
}
```

### 6. CAU-004: Inline Event Handlers (Partial Fix)

While we can't refactor all inline handlers in this task (too risky), we should at least note that the `onclick` handlers in the sidebar items use string concatenation with user data:
```javascript
html += '<li ... onclick="selectReport(' + r.id + ')" ...>';
```

This is safe because `r.id` is an integer from the API. No XSS risk here. Leave as-is for now.

## Files to Modify

1. `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/Reports/Index.cshtml` — JS section only
2. `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/ReportBuilder/Index.cshtml` — JS section only

## What NOT to Change

- Do NOT modify the `<style>` sections — that's done in TASK-FIX-REPORTS-01
- Do NOT modify any `.cs` files
- Do NOT modify `_CardsLayout.cshtml` or `blue-theme.css`
- Do NOT change the page structure or HTML layout

## Acceptance Criteria

- [ ] Zero `alert()` calls remain in either page
- [ ] `showToast()` function exists and works (success + error variants)
- [ ] All JS-generated HTML uses correct class names (`.wd-empty`, `.wd-empty__icon`, `.wd-skeleton-wrap`, `.wd-skel`)
- [ ] `applyLayout()` is implemented and functional
- [ ] AG Grid initialization is wrapped in try/catch
- [ ] `escapeHtml()` implementations match between pages
- [ ] All existing functionality preserved

## Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Reports\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\ReportBuilder\Index.cshtml`

---

**Before editing any existing file, read the current file from disk first. Preserve unrelated changes, including changes made by another Tera session or sub-agent. Do not overwrite, revert, or delete unrelated changes based on memory or an older snapshot.**
