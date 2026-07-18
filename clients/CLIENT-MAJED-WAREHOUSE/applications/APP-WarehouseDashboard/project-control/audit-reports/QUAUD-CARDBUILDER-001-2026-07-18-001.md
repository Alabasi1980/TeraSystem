# QUAUD Report

## Audit ID: QUAUD-CARDBUILDER-001
**Task Reviewed:** AUDIT-CARDBUILDER-001 (Pre-save comprehensive audit — Card Builder Wizard)
**Invoked By:** TeraAgent
**Audit Mode:** Full Risk-Based
**Date:** 2026-07-18

**Scope:** Entire Card Builder Wizard implementation (5 steps + Save pipeline):
- Builder.cshtml (Razor page)
- Builder.cshtml.cs (PageModel)
- card-builder.js (Client wizard controller)
- card-templates.js (Template/palette data)
- _CardsLayout.cshtml (Layout with Chart.js CDN)
- _ViewImports.cshtml (Tag helpers, Radzen imports)
- CardBuilderService.cs (Server-side preview + build)
- CardBuilderModels.cs (DTOs)
- DashboardCard.cs (EF entity)
- Index.cshtml.cs (Card list)
- Pages/Api/Dashboard/CardBuilder.cshtml.cs (Preview API)
- Pages/Api/Dashboard/Card.cshtml.cs (Card data API)
- DashboardService.cs (Query execution)
- CardDataResult.cs (API result)
- KpiQueryBuilder.cs (Advanced KPI queries)
- WarehouseDashboard.Web.csproj (Package refs)
- Program.cs (Startup config)
- card-builder.css (Wizard styles)

**Report Path:** project-control/audit-reports/QUAUD-CARDBUILDER-001-2026-07-18-001.md

**Evidence Sources Used:**
- All source files listed above (direct reading)
- No QA reports, SecurityAgent reports, or git diff provided
- GOVERNANCE: QUALITY_GATE_THRESHOLDS.md, ENGINEERING_BEST_PRACTICES.md

---

## Overall Quality Gate: BLOCKED

| Severity | Count |
|---|---|
| STOP | 4 |
| CAUTION | 6 |
| FLAG | 5 |
| BASELINE_DEBT | 2 |

---

## Findings Summary

### Critical Path: The Save Pipeline is Broken — 3 STOPS Blocking It

---

## FINDING-001 (STOP)

| Field | Value |
|---|---|
| **Finding ID** | F-001 |
| **Rule ID** | HARD-SEC-001 |
| **Domain** | Security / Form Submission |
| **Severity** | `STOP` |
| **Location** | `Builder.cshtml` line 46 + `Builder.cshtml.cs` (no `[IgnoreAntiforgeryToken]`) |
| **Evidence** | `<form method="post" id="wb-form" class="wb-form" novalidate>` at line 46 — no `@Html.AntiForgeryToken()`, no `asp-page` tag helper, no `asp-antiforgery="true"`. Program.cs uses default antiforgery (global `[AutoValidateAntiforgeryToken]`) with `services.AddRazorPages()`. |
| **Expected Standard** | All Razor Pages form POSTs must include `@Html.AntiForgeryToken()` or use the `asp-page` tag helper (which auto-generates it). |
| **Observed Condition** | The `<form>` tag is plain HTML. No antiforgery token is rendered in the form. |
| **Impact** | **Save is completely broken.** The server rejects the POST with HTTP 400 (AntiforgeryValidationException). No card is ever saved. |
| **Recommended Action** | Add `@Html.AntiForgeryToken()` inside the form (before `</form>` at line 428) and/or change to `<form method="post" asp-page="Builder">`. |
| **Changed Code / Baseline** | New — missing from original implementation. |
| **Confidence** | High |
| **Blocking** | Yes |
| **Blocking Reason** | Save cannot succeed without the antiforgery token. |
| **Waiver Allowed** | No |
| **Required Owner** | Developer implementing the save pipeline |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-002 (STOP)

| Field | Value |
|---|---|
| **Finding ID** | F-002 |
| **Rule ID** | HARD-CLIENT-001 |
| **Domain** | Client-side Rendering / Syncfusion Migration |
| **Severity** | `STOP` |
| **Location** | `wwwroot/js/card-builder.js` lines 822-836 (function `renderChart`) |
| **Evidence** | Line 823: `this._previewComp = new global.ej.grids.Grid(gridCfg, host);`
Line 835: `this._previewComp = new global.ej.charts.Chart(cfg, host);`
The Syncfusion NuGet package `Syncfusion.Blazor` (or similar) has been removed — confirmed by `WarehouseDashboard.Web.csproj` (no Syncfusion references). Chart.js is loaded as CDN (`chart.umd.min.js`). Radzen.Blazor is used for grid components. |
| **Expected Standard** | After removing Syncfusion, all references to `ej.*` objects must be replaced with equivalent Chart.js / Radzen APIs. |
| **Observed Condition** | `global.ej` is undefined after Syncfusion removal. The `renderChart()` call throws: `TypeError: Cannot read properties of undefined (reading 'grids')` (or similar). This exception is caught at line 837-840 but already corrupts state. |
| **Impact** | **Live preview rendering is completely broken.** All chart/grid visual previews fail. Additionally, the error at line 837 (`catch (e)`) calls `setConnection('offline')` and `setPreviewState('error')` which may set misleading app state. |
| **Recommended Action** | Replace the Syncfusion-specific chart rendering with Chart.js 4.x API:
  - For charts (Bar, Line, Pie, KPI, Gauge): use `new Chart(ctx, config)` with Chart.js config format
  - For tables: use HTML table rendering or Radzen.Blazor equivalent (or simple DOM table)
  - The server must also return Chart.js-compatible config (see F-003) |
| **Changed Code / Baseline** | New — introduced by Syncfusion → Chart.js migration. |
| **Confidence** | High |
| **Blocking** | Yes |
| **Blocking Reason** | Chart/Grid rendering exception breaks preview. While this does not directly break the save form submission, it prevents visual confirmation and may give false error state. |
| **Waiver Allowed** | No (Syncfusion removal is a completed architectural change) |
| **Required Owner** | Frontend developer |
| **Referral** | DesignReviewer for visual rendering correctness |
| **Status** | Open |

---

## FINDING-003 (STOP)

| Field | Value |
|---|---|
| **Finding ID** | F-003 |
| **Rule ID** | HARD-SERVER-002 |
| **Domain** | Server-side / Chart Configuration |
| **Severity** | `STOP` |
| **Location** | `Services/CardBuilderService.cs` lines 384-469 (method `BuildChartConfig`) |
| **Evidence** | The method returns anonymous objects with Syncfusion-style structure, e.g.:
- Line 422: `{ series: [{ type: "Pie", dataSource: seriesData, xName: "x", yName: "y" }] }`
- Line 436: `type = chartType == "KPI" ? "Column" : "LinearGauge"`
- Line 444-448: `{ columns: [{ field: c, headerText: c, width: "120" }], dataSource: rows }` — Syncfusion Grid config
This config is serialized to JSON and sent to the client, where card-builder.js expects to consume it with Syncfusion constructors. After the migration, the client needs Chart.js config format. |
| **Expected Standard** | Server `BuildChartConfig()` must return Chart.js-compatible config (datasets, labels, options) OR the client must be able to transform it. The config should not reference Syncfusion types like "LinearGauge", "Column" series, etc. |
| **Observed Condition** | The `ChartConfig` JSON payload is in Syncfusion format that cannot be consumed by Chart.js. |
| **Impact** | Even if F-002 is fixed on the client side, the preview data from the server is in the wrong format. The entire preview pipeline (server → client → render) is non-functional. |
| **Recommended Action** | Rewrite `BuildChartConfig` to return Chart.js-compatible configuration:
  - For Bar/Line: `{ type: 'bar'/'line', data: { labels: [...], datasets: [{ data: [...] }] } }`
  - For Pie: `{ type: 'doughnut', data: { labels: [...], datasets: [{ data: [...] }] } }`
  - For Table: return column definitions + rows as raw data (client renders HTML/Radzen grid)
  - For KPI: return the single value for client-side display |
| **Changed Code / Baseline** | New — introduced by Syncfusion → Chart.js migration. |
| **Confidence** | High |
| **Blocking** | Yes |
| **Blocking Reason** | Preview pipeline yields incompatible data from server to client. |
| **Waiver Allowed** | No |
| **Required Owner** | Backend developer (CardBuilderService) |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-004 (STOP)

| Field | Value |
|---|---|
| **Finding ID** | F-004 |
| **Rule ID** | HARD-SEC-002 |
| **Domain** | SQL Injection (KPI Queries) |
| **Severity** | `STOP` |
| **Location** | `Pages/KpiQueryBuilder.cs` lines 130-134 |
| **Evidence** | Method `SanitizeIdentifier` simply wraps column names in brackets after removing `[`, `]`, and `;`. However, the value is **directly interpolated** into SQL strings at lines 64, 82-86, 98 via string concatenation. |
| **Expected Standard** | Never concatenate user-provided column names into SQL. Use `SqlParameter` with `sysname` or parameterized queries. |
| **Observed Condition** | The `card.ValueColumn` and `card.DateColumn` come from user selection in the wizard. While `SanitizeIdentifier` is an improvement over raw concatenation, it is **insufficient**: bracket escaping (`]]`) is not handled, and `SqlParameter` should be used. |
| **Impact** | A user who can craft column names could inject SQL through the KPI column mapping fields. The column names are stored in the database and executed later, making this a stored/persistent injection vector. |
| **Recommended Action** | Replace string interpolation with `SqlParameter("@column", value)` and `SqlCommand`. For column/table names (which cannot be parameterized), validate against a strict allowlist or use `SqlConnection.GetSchema()` to verify the column exists before using it. |
| **Changed Code / Baseline** | Baseline — pre-existing pattern. However, it is exposed/worsened by the current wizard implementation which allows arbitrary column name entry. |
| **Confidence** | Medium |
| **Blocking** | Yes |
| **Blocking Reason** | Persistent SQL injection risk through stored column names. |
| **Waiver Allowed** | Only with explicit Majed acceptance and a documented compensating control. |
| **Required Owner** | Backend developer (SecurityAgent should deep-review) |
| **Referral** | SecurityAgent |
| **Status** | Open |

---

## FINDING-005 (CAUTION)

| Field | Value |
|---|---|
| **Finding ID** | F-005 |
| **Rule ID** | HEUR-FORM-001 |
| **Domain** | Form Binding / Data Integrity |
| **Severity** | `CAUTION` |
| **Location** | `Builder.cshtml` line 181 + `Builder.cshtml.cs` |
| **Evidence** | Line 175: `value="@Model.Title"` (present for title) BUT line 181: missing `value="@Model.DisplayName"` for displayName. Both inputs have `name="displayName"` and the PageModel has `[BindProperty] public string DisplayName`. |
| **Expected Standard** | All bound input fields should have `value` attributes that output the current model value for consistent re-render on validation errors. |
| **Observed Condition** | The `displayName` input has no `value` attribute. When the server returns `Page()` after a validation error, `Model.DisplayName` is populated (from model binding) but the input field renders empty because there's no `value="@Model.DisplayName"`. |
| **Impact** | **User data loss on validation error.** If the user fills in displayName, submits, and server validation fails, the display name field appears empty, forcing the user to retype it. This is a poor UX that could lead to data entry errors. |
| **Recommended Action** | Add `value="@Model.DisplayName"` to the displayName input at line 181. |
| **Changed Code / Baseline** | New — existing in current code. |
| **Confidence** | High |
| **Blocking** | No (but blocks user trust) |
| **Blocking Reason** | N/A |
| **Waiver Allowed** | Yes (minor UX issue) |
| **Required Owner** | Frontend developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-006 (CAUTION)

| Field | Value |
|---|---|
| **Finding ID** | F-006 |
| **Rule ID** | HEUR-FORM-002 |
| **Domain** | Form Binding / Data Integrity |
| **Severity** | `CAUTION` |
| **Location** | `Builder.cshtml.cs` lines 96-106 + `wwwroot/js/card-builder.js` lines 1044-1051 |
| **Evidence** | `GridX` and `GridY` are `[BindProperty] public int` (non-nullable, default -1). The hidden inputs at lines 405-406 have `name="gridX"` and `name="gridY"` with empty default values. When empty string is submitted, ASP.NET Core cannot bind "''" to `int` — it skips the field, leaving the default (-1). The `cleanupDuplicateNames()` at line 1044 removes names from display inputs but the hidden inputs still post with the potential for empty string. |
| **Expected Standard** | Nullable `int?` should be used for optional grid positions, or the binding should handle empty strings gracefully. |
| **Observed Condition** | `int GridX { get; set; } = -1;` — The model defaults to -1 when no value is bound. The syncHiddenInputs writes `s.gridX` (which can be '') to the hidden input. When posted, empty string fails to bind, so GridX stays -1. `BuildDashboardCard()` then checks `GridX >= 0` and returns null for auto-placement. This actually works correctly BY ACCIDENT because -1 fails the ≥ 0 check. But if someone sets GridX to 0 (a valid position), the emptiness check fails. |
| **Impact** | Functionally works for the default case, but zero values ("position 0") would be incorrectly treated as "not set" (since they pass ≥ 0 but represent a valid grid position). |
| **Recommended Action** | Change `GridX`/`GridY` to `public int? GridX { get; set; }` (nullable). Update the hidden input sync and the `BuildDashboardCard()` check accordingly. |
| **Changed Code / Baseline** | New — current code. |
| **Confidence** | Medium |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Backend developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-007 (CAUTION)

| Field | Value |
|---|---|
| **Finding ID** | F-007 |
| **Rule ID** | HEUR-FORM-003 |
| **Domain** | Form Duplicate Names |
| **Severity** | `CAUTION` |
| **Location** | `wwwroot/js/card-builder.js` lines 1044-1051 (`cleanupDuplicateNames`) |
| **Evidence** | The cleanup list includes all duplicate display inputs EXCEPT `wb-source-type` (the source type `<select>`). This element has `name="sourceType"` (line 110 in cshtml) which CONFLICTS with the hidden `wb-h-sourceType` which also has `name="sourceType"` (line 400). The cleanup list includes `wb-sql-table` (for `sourceId`), `wb-grid-width` (for `gridWidth`), etc., but NOT `wb-source-type`. |
| **Expected Standard** | Only one form field should post with each `name` attribute to avoid ambiguous model binding. |
| **Observed Condition** | The `<select id="wb-source-type" name="sourceType">` keeps its `name` attribute, while the hidden `wb-h-sourceType` also has `name="sourceType"`. Two values with the same name are posted. ASP.NET Core model binder takes the last value (the hidden input), so the functionality works — but it's fragile and confusing. |
| **Impact** | Low. Functionally correct because the hidden input (which appears later in the DOM) wins the binding. But it may confuse developers and is technically sending duplicate data. |
| **Recommended Action** | Add `'wb-source-type'` to the cleanup list in `cleanupDuplicateNames()`. |
| **Changed Code / Baseline** | New |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Frontend developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-008 (CAUTION)

| Field | Value |
|---|---|
| **Finding ID** | F-008 |
| **Rule ID** | HEUR-DATA-001 |
| **Domain** | Data Integrity / Model Mapping |
| **Severity** | `CAUTION` |
| **Location** | `Builder.cshtml.cs` lines 317-345 (`OnPostAsync` entity mapping) + `Pages/DashboardService.cs` line 316-326 (`BuildSql`) |
| **Evidence** | The entity creation in `OnPostAsync` maps `dto.SourceType == "SqlTable" ? "View" : "SQL Query"` to `entity.DataSourceType` (line 322). However, the client's `buildPreviewRequest` (card-builder.js line 595-603) always hardcodes `dataSourceType: 'SQL Query'`. The SQL table source type returns a raw `SELECT * FROM [table]` query, not a View name. The server-side `BuildSql` (DashboardService line 316) distinguishes View vs SQL Query by the `DataSourceType` field. |
| **Expected Standard** | The data source type should be consistently mapped between the wizard's source type selection and the stored `DataSourceType` on `DashboardCard`. |
| **Observed Condition** | When saving a `SqlTable` source, `DataSourceType` is set to `"View"` but the stored `SqlQuery` is a raw `SELECT * FROM [table]` — not a view name. This causes `BuildSql` to treat it as a View and wrap it: `SELECT * FROM [SELECT * FROM [tablename]]` — a **syntax error** when the card is rendered on the dashboard. |
| **Impact** | **Cards from SqlTable source will not render on the dashboard.** The query breaks because the stored SQL is misinterpreted. |
| **Recommended Action** | Either:
  a) Store `DataSourceType = "SQL Query"` for SqlTable sources (keeping the raw query), OR
  b) Store the actual view name (not the raw query) when it's a view
  The simplest fix: change line 322 to not map `SqlTable → View`. Map it to `"SQL Query"` instead. |
| **Changed Code / Baseline** | New — introduced in the current implementation. |
| **Confidence** | High |
| **Blocking** | No (but produces broken cards) |
| **Waiver Allowed** | Yes |
| **Required Owner** | Backend developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-009 (CAUTION)

| Field | Value |
|---|---|
| **Finding ID** | F-009 |
| **Rule ID** | HEUR-ARCH-001 |
| **Domain** | Dead Code / Architecture |
| **Severity** | `CAUTION` |
| **Location** | `Builder.cshtml.cs` lines 379-392, 511-544, 546-560 |
| **Evidence** | Three methods are **never called**:
1. `OnPostPreviewAsync([FromBody] PreviewRequest request)` (line 379) — The JS calls `/api/dashboard/cardbuilder?handler=Preview` which routes to `Pages/Api/Dashboard/CardBuilder.cshtml.cs`, NOT the Builder.cshtml.cs. This method is dead.
2. `BuildCardFromRequest(PreviewRequest request)` (line 511) — Only called by the dead `OnPostPreviewAsync`. Dead.
3. `RenderPreview(DashboardCardDto card)` (line 546) — Only called by the dead `OnPostPreviewAsync`. Returns a useless skeleton placeholder anyway. |
| **Expected Standard** | Dead code should be removed to reduce maintenance burden and confusion. |
| **Observed Condition** | These methods exist but serve no purpose. |
| **Impact** | Low. No functional impact, but increases code surface area and may confuse future developers. |
| **Recommended Action** | Remove the dead methods and the associated `PreviewRequest` class from Builder.cshtml.cs (keep the one in CardBuilderModels.cs). |
| **Changed Code / Baseline** | New |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Backend developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-010 (CAUTION)

| Field | Value |
|---|---|
| **Finding ID** | F-010 |
| **Rule ID** | HEUR-DATA-002 |
| **Domain** | Data Integrity |
| **Severity** | `CAUTION` |
| **Location** | `Builder.cshtml` line 196 + `Builder.cshtml.cs` (no `Description` BindProperty) |
| **Evidence** | The form has `<textarea id="wb-description" class="wd-textarea" name="description"...>` at line 195. The server model has no `[BindProperty] public string Description` property. The `DashboardCardDto` also has no `Description` field. The `DashboardCard` entity has no `Description` column. |
| **Expected Standard** | Form data that is submitted should be received and stored by the server, or the input should be removed. |
| **Observed Condition** | The `description` field is submitted but silently discarded. |
| **Impact** | Low — the field is marked "(اختياري)" but the data is lost. Users may fill it out and wonder why it's never shown. |
| **Recommended Action** | Either:
  a) Add a `Description` column to `DashboardCard`, add `[BindProperty]` to PageModel, and map it in `BuildDashboardCard()`, OR
  b) Remove the description field from the wizard UI |
| **Changed Code / Baseline** | New |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-011 (FLAG)

| Field | Value |
|---|---|
| **Finding ID** | F-011 |
| **Rule ID** | HEUR-JS-001 |
| **Domain** | Client Logic |
| **Severity** | `FLAG` |
| **Location** | `wwwroot/js/card-builder.js` lines 1134-1163 (`tryClone`) |
| **Evidence** | The `tryClone()` method fetches `/api/dashboard/cardbuilder/clone/{id}` but this endpoint does not exist in `CardBuilder.cshtml.cs` (which only has `OnPostPreviewAsync`, `OnGetTemplate`, `OnGetTemplates`). The clone API endpoint (`OnPostCloneAsync`) is in `Index.cshtml.cs` and is called via form POST, not via this fetch URL. |
| **Expected Standard** | All API endpoints the client calls must exist on the server. |
| **Observed Condition** | The fetch call to `/api/dashboard/cardbuilder/clone/{id}` will return 404. The `.catch()` handler silently ignores the error. |
| **Impact** | Clone functionality via client-side wizard is broken. The fallback to `readInitialDom()` may partially pre-fill from server-rendered values, but the clone data won't enrich the form. |
| **Recommended Action** | Either:
  a) Add the clone endpoint to `CardBuilder.cshtml.cs`, OR
  b) Remove the client-side `tryClone()` since the server already handles cloning via `LoadCloneDataAsync()` in `OnGetAsync()` |
| **Changed Code / Baseline** | New |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Backend developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-012 (FLAG)

| Field | Value |
|---|---|
| **Finding ID** | F-012 |
| **Rule ID** | HEUR-JS-002 |
| **Domain** | Client Logic |
| **Severity** | `FLAG` |
| **Location** | `wwwroot/js/card-builder.js` lines 795-841 (`renderChart`) |
| **Evidence** | The `renderChart` has no Chart.js implementation at all. It exclusively used Syncfusion objects. After F-002 and F-003 are fixed, this entire function needs a complete rewrite. |
| **Expected Standard** | Should render with Chart.js for charts and DOM/Radzen for tables. |
| **Observed Condition** | Total absence of Chart.js rendering code. |
| **Impact** | See F-002. Listed separately as a FLAG because it's the task-scope item. |
| **Recommended Action** | Rewrite using Chart.js 4.x API. See F-002 for details. |
| **Changed Code / Baseline** | New |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Frontend developer |
| **Referral** | DesignReviewer |
| **Status** | Open |

---

## FINDING-013 (FLAG)

| Field | Value |
|---|---|
| **Finding ID** | F-013 |
| **Rule ID** | HEUR-JS-003 |
| **Domain** | Client Logic |
| **Severity** | `FLAG` |
| **Location** | `wwwroot/js/card-builder.js` lines 512-520 (`wireVisual`) |
| **Evidence** | The grid width/height inputs have `min` and `max` HTML attributes, but the JS only does `parseInt(gw.value, 10) || 1` without clamping to the valid range. Similarly for grid height. For grid X/Y, values could be out of range. |
| **Expected Standard** | Server-side validation exists (Range attributes on GridWidth/GridHeight), but client-side should also validate before submission and display helpful errors. |
| **Observed Condition** | No range validation in JS. Values out of range would be rejected by the server (ModelState invalid) and cause a generic error rather than a field-specific message. |
| **Impact** | Low — server catches it, but UX is poor (generic error rather than field-level). |
| **Recommended Action** | Add range validation to the `wireVisual` event handlers, clamping values and showing field-level errors. |
| **Changed Code / Baseline** | New |
| **Confidence** | Medium |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Frontend developer |
| **Referral** | None |
| **Status** | Open |

---

## FINDING-014 (FLAG)

| Field | Value |
|---|---|
| **Finding ID** | F-014 |
| **Rule ID** | HEUR-JS-004 |
| **Domain** | Client Logic / Error Handling |
| **Severity** | `FLAG` |
| **Location** | `wwwroot/js/card-builder.js` line 1139 |
| **Evidence** | The `executePreview()` fetch call does not include any antiforgery token header. The API endpoint (`CardBuilder.cshtml.cs`) uses `[IgnoreAntiforgeryToken]` which allows this, but it's a permanent bypass. |
| **Expected Standard** | AJAX calls should include the antiforgery token in request headers (e.g., `RequestVerificationToken`). The `[IgnoreAntiforgeryToken]` should only be temporary. |
| **Observed Condition** | The preview API permanently bypasses antiforgery protection. |
| **Impact** | The preview endpoint is vulnerable to CSRF. An attacker could force a user's browser to execute arbitrary preview queries against the SQL Server. |
| **Recommended Action** | Add the antiforgery token to AJAX requests and remove `[IgnoreAntiforgeryToken]` from CardBuilder.cshtml.cs. Use `@Html.AntiForgeryToken()` in the layout and include the token via JS header: `'RequestVerificationToken': antiforgeryToken`. |
| **Changed Code / Baseline** | Baseline — pre-existing in the API endpoint. |
| **Confidence** | High |
| **Blocking** | No (FLAG) |
| **Waiver Allowed** | Yes |
| **Required Owner** | Backend developer |
| **Referral** | SecurityAgent for deep review |
| **Status** | Open |

---

## FINDING-015 (FLAG)

| Field | Value |
|---|---|
| **Finding ID** | F-015 |
| **Rule ID** | HEUR-ACCESS-001 |
| **Domain** | Accessibility |
| **Severity** | `FLAG` |
| **Location** | `Builder.cshtml` line 48 and 176-177, 181-182 |
| **Evidence** | `asp-validation-summary="ModelOnly"` shows only model-level errors. The per-field error spans have `data-for` attributes but no client-side validation framework is wired to populate them. The inputs use `required` HTML5 attributes but the form has `novalidate` — disabling browser native validation. |
| **Expected Standard** | Errors should be visible to all users (including screen readers). `aria-describedby` and `aria-live` regions should announce validation errors. |
| **Observed Condition** | The form has `novalidate` (line 46) which disables browser validation. Client-side validation appears manual (via `validateStep`), but the per-field error spans (`data-for` attributes) are never populated — only a step-level error div is shown. |
| **Impact** | Screen readers may not receive timely error feedback. Users don't see which specific field has the error. |
| **Recommended Action** | Either enable HTML5 validation or wire the `data-for` error spans from the JS validation logic. Also ensure `aria-invalid` is set on error fields. |
| **Changed Code / Baseline** | New |
| **Confidence** | Medium |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | Frontend developer |
| **Referral** | DesignReviewer for UX review |
| **Status** | Open |

---

## Finding: BASELINE_DEBT-001

| Field | Value |
|---|---|
| **Finding ID** | BD-001 |
| **Rule ID** | BASELINE-DEP-001 |
| **Domain** | Dependency / NuGet |
| **Severity** | `FLAG` (Baseline Debt) |
| **Location** | `WarehouseDashboard.Web.csproj` line 13 |
| **Evidence** | `Microsoft.EntityFrameworkCore.Design` version `8.0.*` with floating wildcard — this is generally fine. But `Radzen.Blazor` version `5.*` is also floating. No explicit vulnerability scan was available. |
| **Impact** | Unknown vulnerabilities. |
| **Confidence** | Low — no analyzer report available. |
| **Status** | Defeered |

---

## Finding: BASELINE_DEBT-002

| Field | Value |
|---|---|
| **Finding ID** | BD-002 |
| **Rule ID** | BASELINE-ARCH-001 |
| **Domain** | Architecture / Migration |
| **Severity** | `FLAG` (Baseline Debt) |
| **Location** | `Services/CardBuilderService.cs` line 27-131 |
| **Evidence** | `_templates` are defined as `static readonly` in the service class. The same templates are duplicated in `wwwroot/js/card-templates.js` as `global.CardBuilderTemplates`. This dual-source-of-truth pattern is fragile — any change to templates requires updating both files. |
| **Expected Standard** | Templates should be served from a single source: either the server API or client-side only. |
| **Observed Condition** | Both sources exist independently. |
| **Impact** | Templates could drift between client and server. For example, if a template is added to the server but not the JS, the client won't show it (and vice versa). |
| **Confidence** | High |
| **Status** | Open — not blocking current task |

---

## Handback to Orchestrator

| Field | Value |
|---|---|
| **Status** | `BLOCKED` |
| **Report Path** | `project-control/audit-reports/QUAUD-CARDBUILDER-001-2026-07-18-001.md` |
| **Blocking Findings** | F-001 (Missing Antiforgery Token), F-002 (Syncfusion client refs), F-003 (Syncfusion server config), F-004 (SQL Injection in KPI queries) |
| **Recommended Next Action** | Create fix tasks (TASK-COD-XXX) for the 4 STOP findings in priority order:
1. **F-001**: Add `@Html.AntiForgeryToken()` to form — this is the #1 reason save doesn't work
2. **F-002**: Replace Syncfusion client rendering with Chart.js (card-builder.js `renderChart()`)
3. **F-003**: Rewrite server `BuildChartConfig()` to return Chart.js format (CardBuilderService.cs)
4. **F-004**: Fix SQL injection in KpiQueryBuilder.cs (use parameters, not interpolation)
Then address CAUTION findings F-005 through F-010.

**Key insight for the user's question "Save in Step 5 still doesn't work":** The #1 reason is F-001 — the form has no antiforgery token. The server silently rejects the POST with HTTP 400 before `OnPostAsync` even runs. |

