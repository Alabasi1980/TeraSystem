# TASK-KPI-FIX-019 — Fix CustomSQL Input Listener

## Task Info
| Field | Value |
|---|---|
| **Task ID** | TASK-KPI-FIX-019 |
| **Status** | Accepted — Closed |
| **Priority** | High |
| **Type** | Bug Fix — Data Loss |
| **Requested By** | Majed |
| **Created** | 2026-07-17 |
| **Based On** | Audit QUAUD-SAVE-DASHBOARD-PIPELINE (F-003) |

## The Problem
When user selects "SQL مخصص (متقدم)" as source type and types a SQL query, the `wb-custom-sql` textarea has **no `input` event listener**. This means `this.state.customSql` is never updated from user input — it stays as the initial empty value.

When the form submits, `syncHiddenInputs()` reads `s.customSql = ''` → saves empty SQL → card has no data → dashboard shows error.

## Root Cause
`wireFields()` at card-builder.js line 496-501 only wires `wb-title` and `wb-display-name`. The `wb-custom-sql` textarea exists in the HTML but no JavaScript code listens to its `input` event.

## The Fix — One Change in One File

### File: `card-builder.js`
### Function: `wireFields()` (~line 496)

**Current:**
```js
CardBuilderWizard.prototype.wireFields = function () {
    var self = this;
    var title = $('wb-title'), dn = $('wb-display-name');
    if (title) title.addEventListener('input', function () { self.state.title = title.value; self.updateFooter(); });
    if (dn) dn.addEventListener('input', function () { self.state.displayName = dn.value; self.updateFooter(); });
};
```

**New:**
```js
CardBuilderWizard.prototype.wireFields = function () {
    var self = this;
    var title = $('wb-title'), dn = $('wb-display-name'), cs = $('wb-custom-sql');
    if (title) title.addEventListener('input', function () { self.state.title = title.value; self.updateFooter(); });
    if (dn) dn.addEventListener('input', function () { self.state.displayName = dn.value; self.updateFooter(); });
    if (cs) cs.addEventListener('input', function () {
        self.state.customSql = cs.value;
        self.state.previewSql = cs.value;
        if ($('wb-h-customSql')) $('wb-h-customSql').value = cs.value;
        self.schedulePreview();
        self.updateFooter();
    });
};
```

### Logic
- `self.state.customSql = cs.value` — updates the SQL state on every keystroke
- `self.state.previewSql = cs.value` — updates preview SQL (used for live preview)
- `$('wb-h-customSql').value = cs.value` — keeps hidden input in sync immediately
- `self.schedulePreview()` — triggers live preview update (debounced 400ms)
- `self.updateFooter()` — re-evaluates validation state

## Allowed Write Targets
1. `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js`

## Acceptance Criteria
- [ ] `wb-custom-sql` textarea has an `input` event listener
- [ ] On keystroke: `state.customSql` updates, `state.previewSql` updates, hidden input syncs, preview refreshes
- [ ] Build succeeds with 0 errors, 0 warnings
- [ ] CustomSQL card saves with the correct SQL query and appears on dashboard

## Mandatory Governance
- Read current file from disk before editing
- Preserve unrelated changes
- No secrets

## Verification Command
```
dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\KPI-FIX-019-check
```
Run from:
`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

## Engineering Handback
- **File changed:** `card-builder.js` — `wireFields()` function
- **Change:** Added `cs = $('wb-custom-sql')` + input event listener that syncs `state.customSql`, `state.previewSql`, hidden input, schedules preview, and updates footer validation
- **Build:** 0 warnings, 0 errors ✅
- **No unrelated changes**

## Tera Post-Execution Review
- **Allowed Write Targets:** PASS — one file, one function
- **Scope:** PASS — exactly the fix needed
- **Secrets:** PASS
- **Auditor Review Decision:** NOT_REQUIRED
- **Status:** Accepted — Closed
