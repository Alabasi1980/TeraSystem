# TASK-COD-027 — Edit Card via Builder Wizard

> **Status:** ✅ Accepted  
> **Agent:** engineering-agent  
> **Created:** 2026-07-18  
> **Completed:** 2026-07-18  
> **Phase:** 6 (Implementation)  
> **Priority:** High  
> **Build:** PASS (0 warnings, 0 errors)

---

## 1. Objective

Add the ability to edit an existing dashboard card through the existing multi-step Card Builder wizard (`/admin-secure-panel/Cards/Builder`). Currently, clicking "تعديل" goes to a simple form (`Edit.cshtml`). The user wants an "Edit in Builder" option that pre-fills all card data correctly without data loss.

---

## 2. Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| AC-1 | `Builder?edit=ID` route exists and loads the card data | Open URL with edit param |
| AC-2 | All 27 card fields are pre-filled in their correct wizard steps | UI inspection |
| AC-3 | KPI-specific fields (ValueColumn, KpiMode, etc.) show when card type is KPI | UI inspection |
| AC-4 | Source type "View" (DB) maps to "SqlTable" in Builder and back to "View" on save | Create SqlTable card → Edit via Builder → Save |
| AC-5 | Editing a card does not change its ID or create a duplicate | DB check |
| AC-6 | The simple `Edit.cshtml` remains functional and untouched | Open old edit link |
| AC-7 | Cards Grid has both "تعديل" (simple) and "تعديل بالمعالج" (builder) links | UI inspection |
| AC-8 | Build succeeds with no errors | `dotnet build` PASS |
| AC-9 | No data loss after round-trip edit → save | Compare DB row before/after edit |

---

## 3. Files to Modify

### 3.1 Builder PageModel — Add Edit Mode

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs`

- Add `[BindProperty(SupportsGet = true)] public string? EditId { get; set; }`
- Add `private async Task LoadEditDataAsync()` method that loads existing `DashboardCard` by ID and maps all properties to the model properties
- Modify `OnGetAsync()` to call `LoadEditDataAsync()` after `LoadCloneDataAsync()` (or instead of it)
- Modify `OnPostAsync()` to detect edit mode (`EditId` present) and update existing entity instead of creating new one
- Ensure `OnPostAsync` redirects to `Index` after edit save
- Map the DB `DataSourceType` value to the Builder `SourceType` value:
  - DB `"View"` → Builder `"SqlTable"`, and parse the table name from `SqlQuery` (e.g., `SELECT * FROM [table]` or `SELECT SUM([col]) FROM [table]`) using regex `FROM\s+\[([^\]]+)\]` to set `SourceId`. If parsing fails, fall back to `"CustomSQL"` with `CustomSql = SqlQuery`.
  - DB `"SQL Query"` → Builder `"CustomSQL"`, with `CustomSql = SqlQuery`.
  - Do NOT use `"SavedQuery"` for fallback unless the card was originally created from a saved query (we have no stored metadata for that).

### 3.2 Builder UI — Pass Edit Mode

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml`

- Add hidden field: `<input type="hidden" name="editId" id="wb-h-editId" value="@Model.EditId" />`
- Add `editId` to the inline script initialization:
  ```js
  initialData: {
      cardType: '@Model.CardType',
      sourceType: '@Model.SourceType',
      sourceId: '@Model.SourceId',
      customSql: '@Model.CustomSql',
      title: '@Model.Title',
      displayName: '@Model.DisplayName',
      aggregationType: '@Model.AggregationType'
  }
  ```
- Pass `editId: '@Model.EditId'` to the CardBuilderWizard constructor

### 3.3 Card Builder JavaScript — Pre-fill in Edit Mode

**File:** `src/WarehouseDashboard.Web/wwwroot/js/card-builder.js`

- Accept `editId` in the constructor options
- When `editId` is present, set the wizard into edit mode
- Pre-fill all fields from `initialData`:
  - `cardType` → select type card
  - `sourceType` → select source type dropdown
  - `customSql` → fill textarea
  - `title` → fill title input
  - `displayName` → fill display name input
  - `aggregationType` → select aggregation dropdown
  - Grid fields, color palette, refresh interval from `syncHiddenInputs` / state
- Ensure KPI panels are shown/hidden based on cardType
- Ensure source panels are shown/hidden based on sourceType
- On save, keep `editId` in the form so POST handler knows to update

### 3.4 Cards Grid — Add Builder Edit Link

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/CardsGrid.razor`

- Add a link next to the existing edit link:
  ```html
  <a class="wd-link" href="/admin-secure-panel/Cards/Builder?edit=@item.Id">تعديل بالمعالج</a>
  ```

### 3.5 Cards Index — Add Builder Edit Link

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Index.cshtml`

- In the Radzen grid actions column (since Index uses `CardsGrid.razor`), no change is needed here if we modify `CardsGrid.razor`. But if the grid is replaced, add the link here instead.

---

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\CardsGrid.razor
```

---

## 5. Allowed Tools

- `read` — read existing files before editing
- `edit` — modify existing files
- `bash` — `dotnet build` to verify compilation
- `glob` / `grep` — find files and search code

---

## 6. Data Mapping Reference

### DB → BuilderModel

| DB Column | BuilderModel Property | Wizard Step |
|---|---|---|
| `ChartType` | `CardType` | 1 |
| `DataSourceType` | `SourceType` | 2 |
| `SqlQuery` | `SqlQuery` / `SourceId` / `CustomSql` | 2 |
| `Title` | `Title` / `DisplayName` | 3 |
| `ValueColumn` | `ValueColumn` | 4 |
| `DateColumn` | `DateColumn` | 4 |
| `CategoryColumn` | `CategoryColumn` | 4 |
| `KpiMode` | `KpiMode` | 4 |
| `ShowChange` | `ShowChange` | 4 |
| `ChangeSource` | `ChangeSource` | 4 |
| `ShowSparkline` | `ShowSparkline` | 4 |
| `SparklineMonths` | `SparklineMonths` | 4 |
| `ShowGrandTotal` | `ShowGrandTotal` | 4 |
| `GrandTotalSource` | `GrandTotalSource` | 4 |
| `DateFilterMode` | `DateFilterMode` | 4 |
| `FixedStartDate` | `FixedStartDate` | 4 |
| `FixedEndDate` | `FixedEndDate` | 4 |
| `RelativeDays` | `RelativeDays` | 4 |
| `GridPositionX` | `GridX` | 5 |
| `GridPositionY` | `GridY` | 5 |
| `GridWidth` | `GridWidth` | 5 |
| `GridHeight` | `GridHeight` | 5 |
| `ColorPalette` | `ColorPalette` | 5 |
| `RefreshInterval` | `RefreshInterval` | 5 |
| `AggregationType` | `AggregationType` | 3 |

### DataSourceType Mapping

| DB Value | Builder SourceType | Notes |
|---|---|---|
| `"View"` | `"SqlTable"` | Parse table name from `SqlQuery`; fallback to `"CustomSQL"` |
| `"SQL Query"` | `"CustomSQL"` | Load `SqlQuery` into `CustomSql` field |

---

## 7. Save Behavior

- If `EditId` is present → update existing entity:
  ```csharp
  var existing = await _db.DashboardCards.FindAsync(id);
  // map all dto properties to existing entity
  await _db.SaveChangesAsync();
  return RedirectToPage("/admin-secure-panel/Cards/Index");
  ```
- If `EditId` is not present → create new entity (current behavior)

---

## 8. Risk

**Medium** — Touching the core card builder. Requires careful mapping to avoid data loss. The simple Edit page remains untouched as a fallback.

---

## 9. Delegation Instructions

Before editing any existing file, read the current file from disk first. Preserve unrelated changes, including changes made by another Tera session or sub-agent. Do not overwrite, revert, or delete unrelated changes based on memory or an older snapshot.

Execute `dotnet build` after all edits to verify compilation. Return the build output in your handback.

---

> **Assigned by:** TeraAgent — 2026-07-18  
> **Task ID:** TASK-COD-027
