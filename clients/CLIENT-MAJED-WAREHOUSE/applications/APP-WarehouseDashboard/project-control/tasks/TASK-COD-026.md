# TASK-COD-026 — KPI Aggregation Type (Dynamic Calculation Mode)

> **Status:** ✅ Accepted  
> **Agent:** engineering-agent  
> **Created:** 2026-07-18  
> **Completed:** 2026-07-18  
> **Phase:** 6 (Implementation)  
> **Priority:** High  
> **Build:** PASS (0 warnings, 0 errors)

---

## 1. Objective

Add an **AggregationType** field to KPI cards so the user can choose how the `ValueColumn` is calculated:
- **Sum** (default) — `SUM(column)` — total of all rows
- **Count** — `COUNT(column)` — number of rows
- **Avg** — `AVG(column)` — average value
- **Min** — `MIN(column)` — smallest value
- **Max** — `MAX(column)` — largest value
- **None** — raw value (current behavior, first row)

### Problem Solved

Currently, `DashboardService` reads `rows[0][ValueColumn]` — the first row only. For a KPI showing "Total Invoices" with 1000 rows, it displays one row's value (~2,100) instead of the sum (1,652,027).

---

## 2. Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| AC-1 | `DashboardCard.AggregationType` property exists with default `"Sum"` | DB migration + model check |
| AC-2 | Builder Step 3 shows Aggregation dropdown (only when KPI type selected) | UI check |
| AC-3 | Builder saves `AggregationType` to DB correctly | Create card → verify DB |
| AC-4 | `DashboardService.BuildSql()` wraps query with aggregation when `AggregationType != "None"` | API response check |
| AC-5 | Each aggregation type produces correct SQL | Unit/integration test |
| AC-6 | Edit page shows AggregationType dropdown | UI check |
| AC-7 | Clone preserves AggregationType | Clone card → verify |
| AC-8 | Non-KPI cards ignore AggregationType (no wrapping) | Bar/Line card still works |
| AC-9 | Build succeeds with no errors | `dotnet build` PASS |

---

## 3. Files to Modify

### 3.1 Model — Add Property

**File:** `src/WarehouseDashboard.Web/Models/DashboardCard.cs`

Add after line 108 (`RelativeDays`):
```csharp
/// <summary>
/// Aggregation method for KPI ValueColumn: "Sum", "Count", "Avg", "Min", "Max", "None".
/// Default: "Sum". Only applied when ChartType == "KPI".
/// </summary>
public string AggregationType { get; set; } = "Sum";
```

### 3.2 Input Model — Add Property + Validation

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/CardEditorInput.cs`

Add after `RelativeDays` (line 76):
```csharp
public string AggregationType { get; set; } = "Sum";
```

Add to `AllowedChartTypes` validation — no change needed (it's independent of chart type).

Add static list:
```csharp
public static readonly string[] AllowedAggregationTypes = { "Sum", "Count", "Avg", "Min", "Max", "None" };

public static List<SelectOption> AggregationTypeOptions =>
    AllowedAggregationTypes.Select(x => new SelectOption(x, x)).ToList();
```

### 3.3 Builder PageModel — Bind Property

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs`

Add BindProperty after `RelativeDays`:
```csharp
[BindProperty]
[JsonPropertyName("aggregationType")]
public string AggregationType { get; set; } = "Sum";
```

Update `BuildDashboardCard()` — add `AggregationType = AggregationType`.

Update `BuildCardFromRequest()` — add `AggregationType = request.AggregationType`.

Update `PreviewRequest` class — add `public string AggregationType { get; set; } = "Sum";`.

Update `DashboardCardDto` class — add `public string AggregationType { get; set; } = "Sum";`.

Update `LoadCloneDataAsync()` — add `AggregationType = req.AggregationType ?? "Sum"`.

### 3.4 Builder UI — Add Dropdown

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml`

In Step 3 (Basic Fields), after the `wb-preview-sql` textarea and before the `wb-description` field, add:

```html
<div class="wd-field" id="wb-aggregation-field" style="display:none;">
    <label class="wd-field__label" for="wb-aggregation-type">طريقة الحساب</label>
    <select id="wb-aggregation-type" class="wd-select" name="aggregationType" aria-describedby="wb-aggregation-hint">
        <option value="Sum">Sum — المجموع</option>
        <option value="Count">Count — العدد</option>
        <option value="Avg">Avg — المتوسط</option>
        <option value="Min">Min — الأدنى</option>
        <option value="Max">Max — الأعلى</option>
        <option value="None">None — القيمة الخام</option>
    </select>
    <span id="wb-aggregation-hint" class="wd-field__hint">كيف يُحسب الرقم من العامود المختار</span>
</div>
```

Add hidden field for form submission (near other hidden fields):
```html
<input type="hidden" name="aggregationType" id="wb-h-aggregationType" value="Sum">
```

### 3.5 DashboardService — Apply Aggregation in SQL

**File:** `src/WarehouseDashboard.Web/Pages/DashboardService.cs`

Modify `BuildSql()` method:

```csharp
private static string BuildSql(DashboardCard card)
{
    var baseSql = card.DataSourceType.Equals("View", StringComparison.OrdinalIgnoreCase)
        ? $"SELECT * FROM [{card.SqlQuery.Trim().TrimEnd(';').Trim()}]"
        : card.SqlQuery;

    // KPI aggregation: wrap base query
    if (card.ChartType.Equals("KPI", StringComparison.OrdinalIgnoreCase)
        && !string.IsNullOrEmpty(card.AggregationType)
        && card.AggregationType != "None"
        && !string.IsNullOrEmpty(card.ValueColumn))
    {
        var col = card.ValueColumn.Trim('[', ']').Trim();
        var aggFunc = card.AggregationType.ToUpperInvariant(); // SUM, COUNT, AVG, MIN, MAX
        return $"SELECT {aggFunc}([{col}]) AS [{col}] FROM ({baseSql.TrimEnd(';')}) AS _agg_src";
    }

    return baseSql;
}
```

**Important:** The column alias in the outer SELECT must match `ValueColumn` so `rows[0].TryGetValue(valueCol, ...)` still works.

### 3.6 Edit PageModel — Bind Property

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Edit.cshtml.cs`

In `OnGetAsync` — add `AggregationType = card.AggregationType ?? "Sum"` to the Input mapping.

In `OnPostAsync` — add `card.AggregationType = Input.AggregationType` to the update logic.

### 3.7 Edit UI — Add Dropdown

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Edit.cshtml`

Add after the `IsActive` checkbox field:
```html
<div class="wd-field">
    <label asp-for="Input.AggregationType">طريقة الحساب (KPI)</label>
    <select asp-for="Input.AggregationType" class="wd-select" asp-items="@(new SelectList(Model.AggregationTypeOptions, "Value", "Text"))">
        <option value="">اختر...</option>
    </select>
</div>
```

In `EditModel` — add:
```csharp
public List<SelectOption> AggregationTypeOptions => CardEditorInput.AggregationTypeOptions;
```

---

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\DashboardCard.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\CardEditorInput.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Edit.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Edit.cshtml
```

---

## 5. Allowed Tools

- `read` — read existing files before editing
- `edit` — modify existing files
- `bash` — `dotnet build` to verify compilation
- `glob` / `grep` — find files and search code

---

## 6. Expected SQL Behavior

| AggregationType | Generated SQL | Result |
|---|---|---|
| Sum | `SELECT SUM([FINAL_SUM_INVOICE]) AS [FINAL_SUM_INVOICE] FROM (...)` | 1,652,027 |
| Count | `SELECT COUNT([FINAL_SUM_INVOICE]) AS [FINAL_SUM_INVOICE] FROM (...)` | 1000 |
| Avg | `SELECT AVG([FINAL_SUM_INVOICE]) AS [FINAL_SUM_INVOICE] FROM (...)` | 1,652.03 |
| Min | `SELECT MIN([FINAL_SUM_INVOICE]) AS [FINAL_SUM_INVOICE] FROM (...)` | smallest |
| Max | `SELECT MAX([FINAL_SUM_INVOICE]) AS [FINAL_SUM_INVOICE] FROM (...)` | largest |
| None | `SELECT * FROM [stg_st_invoice]` (raw) | first row |

---

## 7. Risk

**Low** — Additive change. No existing behavior altered when `AggregationType = "None"` or for non-KPI cards.

---

## 8. Delegation Instructions

Before editing any existing file, read the current file from disk first. Preserve unrelated changes, including changes made by another Tera session or sub-agent. Do not overwrite, revert, or delete unrelated changes based on memory or an older snapshot.

Execute `dotnet build` after all edits to verify compilation. Return the build output in your handback.

---

> **Assigned by:** TeraAgent — 2026-07-18  
> **Task ID:** TASK-COD-026
