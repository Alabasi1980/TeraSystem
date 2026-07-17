# TASK-KPI-FIX-018 — Fix Card Save Pipeline (SqlQuery, DataSourceType, TempData, Hide "التالي")

## Task Info
| Field | Value |
|---|---|
| **Task ID** | TASK-KPI-FIX-018 |
| **Status** | Accepted — Closed |
| **Priority** | P0 — Blocking |
| **Type** | Bug Fix — Data Loss + Dashboard Visibility |
| **Requested By** | Majed |
| **Created** | 2026-07-17 |
| **Based On** | Audit QUAUD-SAVE-DASHBOARD-PIPELINE-2026-07-17-001 |

## The Problem
When a user saves a card:
1. `SqlQuery` is saved as the **bare table name** (e.g., `"stg_WarehouseStock"`) instead of a real SQL query → dashboard tries to execute it → syntax error → card doesn't render
2. `DataSourceType` is saved as `"Template"`/`"SqlTable"`/`"CustomSQL"` instead of `"SQL Query"` or `"View"` → `DashboardService.BuildSql()` fails because it only handles `"View"` or falls through to verbatim query
3. **No success message**: Builder sets `TempData["SuccessMessage"]` but `Cards/Index.cshtml.cs` reads `TempData["ToastMessage"]` → mismatch → user sees nothing
4. **"التالي" button** visible (disabled) on Step 5 → user expects more steps

## Root Cause Chain
```
Builder.cshtml.cs:318: SqlQuery = dto.CustomSql ?? (dto.SourceId ?? "")
  → For SqlTable: CustomSql=null, SourceId="stg_WarehouseStock"
  → SqlQuery = "stg_WarehouseStock"  ← NOT A REAL SQL QUERY!

Builder.cshtml.cs:317: DataSourceType = dto.SourceType ?? "SQL Query"
  → For SqlTable: SourceType="SqlTable"
  → DataSourceType = "SqlTable"  ← SHOULD BE "View" or "SQL Query"
```

## Detailed Changes — 4 Files

### Allowed Write Targets
1. `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
2. `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js`
3. `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
4. `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Index.cshtml` (if it shows ToastMessage)

---

### FIX 1: Add `wb-h-sqlQuery` Hidden Input (Builder.cshtml)

**File:** `Builder.cshtml` — after the existing hidden inputs (~line 406, after `wb-h-refreshInterval` and before the KPI fields section):

**Change:** Add one new hidden input:
```html
<!-- SQL Query to execute for card data -->
<input type="hidden" name="sqlQuery" id="wb-h-sqlQuery" value="">
```

**Why:** Currently no hidden input carries the actual SQL query to the server. The `customSql` is only set for CustomSQL, and `sourceId` is just a table name. Adding `wb-h-sqlQuery` creates a dedicated channel for the actual card query.

---

### FIX 2: Sync SqlQuery from Client State (card-builder.js)

**File:** `card-builder.js` — in `syncHiddenInputs()` function (~line 1042–1057)

**Current `syncHiddenInputs()`:**
```js
CardBuilderWizard.prototype.syncHiddenInputs = function () {
    var s = this.state;
    if ($('wb-h-cardType')) $('wb-h-cardType').value = s.cardType;
    if ($('wb-h-sourceType')) $('wb-h-sourceType').value = s.sourceType;
    if ($('wb-h-sourceId')) $('wb-h-sourceId').value = s.sourceId;
    if ($('wb-h-customSql')) $('wb-h-customSql').value = s.customSql;
    if ($('wb-h-gridWidth')) $('wb-h-gridWidth').value = s.gridWidth;
    if ($('wb-h-gridHeight')) $('wb-h-gridHeight').value = s.gridHeight;
    if ($('wb-h-gridX')) $('wb-h-gridX').value = s.gridX;
    if ($('wb-h-gridY')) $('wb-h-gridY').value = s.gridY;
    if ($('wb-h-colorPalette')) $('wb-h-colorPalette').value = s.palette;
    if ($('wb-h-refreshInterval')) $('wb-h-refreshInterval').value = s.refreshInterval;

    // Sync KPI hidden fields (TASK-KPI-006)
    this.syncKpiHiddenFields();
};
```

**Change:** Add a new block AFTER the grid/refresh syncing and BEFORE the KPI sync:

```js
    // Sync SqlQuery — build proper SQL based on source type
    var sqlQuery = '';
    if (s.sourceType === 'Template' || s.sourceType === 'SavedQuery') {
      sqlQuery = s.previewSql || '';
    } else if (s.sourceType === 'SqlTable') {
      sqlQuery = s.selectedTable ? 'SELECT * FROM [' + s.selectedTable.sqlTargetTable + ']' : '';
    } else if (s.sourceType === 'CustomSQL') {
      sqlQuery = s.customSql || '';
    }
    if ($('wb-h-sqlQuery')) $('wb-h-sqlQuery').value = sqlQuery;
```

**Logic explanation:**
- **Template**: `s.previewSql` already has the template SQL with `{TableName}` replaced (without `TOP 10`)
- **SqlTable**: Build `SELECT * FROM [tableName]` — a full table select; will be treated as "View" on the server
- **CustomSQL**: Use `s.customSql` (the user's SQL)
- **SavedQuery**: `s.previewSql` as fallback

---

### FIX 3: Fix OnPostAsync Entity Creation (Builder.cshtml.cs)

**File:** `Builder.cshtml.cs` — `OnPostAsync()` lines 310–354

**Change the entity creation section (current lines 313–340):**

**Current (WRONG):**
```csharp
DataSourceType = dto.SourceType ?? "SQL Query",
SqlQuery = dto.CustomSql ?? (dto.SourceId ?? ""),
```

**New (CORRECT):**
```csharp
DataSourceType = dto.SourceType == "SqlTable" ? "View" : "SQL Query",
SqlQuery = dto.SqlQuery,
```

**Also need to add `SqlQuery` BindProperty and DTO field:**

**Add BindProperty** (near other KPI/hidden fields, around line 160 area):
```csharp
/// <summary>SQL query to execute for card data.</summary>
[BindProperty]
[JsonPropertyName("sqlQuery")]
public string SqlQuery { get; set; } = string.Empty;
```

**Add to `DashboardCardDto`** (~line ~592 area):
```csharp
public string SqlQuery { get; set; } = string.Empty;
```

**Update `BuildDashboardCard()`** — add mapping for SqlQuery (after `CustomSql` line):
```csharp
SqlQuery = SqlQuery,
```

**Update `BuildCardFromRequest()`** — add mapping:
```csharp
SqlQuery = request.SqlQuery,
```

**Add to `PreviewRequest`** class:
```csharp
public string SqlQuery { get; set; } = string.Empty;
```

---

### FIX 4: Fix TempData Key (Builder.cshtml.cs)

**File:** `Builder.cshtml.cs` — line ~344

**Current:**
```csharp
TempData["SuccessMessage"] = action == "saveAndAddAnother"
```

**Change to:**
```csharp
TempData["ToastMessage"] = action == "saveAndAddAnother"
```

And add the ToastType:
```csharp
TempData["ToastType"] = "success";
```

**Why:** `Cards/Index.cshtml.cs` line 39 reads `TempData["ToastMessage"]` (not `SuccessMessage`). The mismatch means the success message is set but never displayed.

---

### FIX 5: Hide "التالي" Button on Last Step (card-builder.js)

**File:** `card-builder.js` — `updateFooter()` function ~line 907-920

**Current:**
```js
if (next) {
  var canNext = this.validateStepSilent(step) && step < maxStep;
  next.disabled = !canNext;
  next.setAttribute('aria-disabled', canNext ? 'false' : 'true');
}
```

**Change to:**
```js
if (next) {
  if (step >= maxStep) {
    hide(next);
  } else {
    show(next);
    var canNext = this.validateStepSilent(step);
    next.disabled = !canNext;
    next.setAttribute('aria-disabled', canNext ? 'false' : 'true');
  }
}
```

**Why:** When user reaches the last step (Step 5 for KPI, Step 4 for non-KPI), "التالي" should be completely hidden, not just disabled. This removes the confusion of a disabled button that suggests more steps exist.

---

## Acceptance Criteria
- [ ] New hidden input `wb-h-sqlQuery` exists in Builder.cshtml
- [ ] `syncHiddenInputs()` sets `wb-h-sqlQuery` to proper SQL query for each source type
- [ ] `OnPostAsync()` saves `SqlQuery` from submitted form field, not from `CustomSql ?? SourceId`
- [ ] `DataSourceType` = `"View"` for SqlTable sources, `"SQL Query"` for others
- [ ] `TempData["ToastMessage"]` is set (not `SuccessMessage`)
- [ ] `TempData["ToastType"] = "success"` is also set
- [ ] `updateFooter()` hides (not just disables) "التالي" button on last step
- [ ] Build succeeds with 0 errors, 0 warnings
- [ ] After rebuild + hard refresh: saving a card shows success toast, card appears on dashboard

## Mandatory Governance
- Read current file from disk before editing (Fresh File Read Rule)
- Preserve unrelated changes
- Do not introduce secrets
- Report build result using fallback temp output if app process locks files

## Verification Command
```
dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\KPI-FIX-018-check
```
Run from:
`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

## Engineering Handback
- **Build:** 0 warnings, 0 errors ✅
- **Files changed:** 3 (Builder.cshtml, card-builder.js, Builder.cshtml.cs)
- **Index.cshtml:** No changes needed — already reads `TempData["ToastMessage"]` correctly
- **Unrelated revert:** appsettings.json (license key change) reverted

## Tera Post-Execution Review
- **Allowed Write Targets:** PASS
- **Scope:** PASS — all 5 fixes implemented
- **Secrets:** PASS — no secrets introduced
- **Unrelated changes:** PASS — reverted license key change
- **Auditor Review Decision:** NOT_REQUIRED (focused bug fix with clear acceptance criteria)
- **Status:** Accepted — Closed
