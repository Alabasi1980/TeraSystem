# QUAUD-SAVE-DASHBOARD-PIPELINE-2026-07-17-001

**Audit ID:** QUAUD-SAVE-DASHBOARD-PIPELINE-2026-07-17-001  
**Task Reviewed:** Card Save-to-Dashboard Pipeline (comprehensive audit)  
**Invoked By:** Majed (direct request)  
**Audit Mode:** Full Risk-Based  
**Scope:** Expanded — entire save pipeline, display pipeline, and UX flow  
**Report Path:** project-control/audit-reports/QUAUD-SAVE-DASHBOARD-PIPELINE-2026-07-17-001.md  
**Evidence Sources Used:**
- Builder.cshtml.cs (OnPostAsync, BuildDashboardCard, LoadCloneDataAsync, DashboardCardDto)
- Builder.cshtml (hidden inputs, form structure)
- card-builder.js (syncHiddenInputs, submitForm, getPreviewSql, buildPreviewRequest, updateFooter, wireSave, wireCustomSql)
- Index.cshtml.cs (CardLayoutInfo projection, OnGetAsync)
- Index.cshtml (client-side card rendering, wdLoadCard)
- DashboardService.cs (BuildSql, GetCardDataByIdAsync, GetCardDataAsync)
- Card.cshtml.cs (API endpoint /api/dashboard/card/{id})
- DashboardCard.cs (entity model)
- Cards/Index.cshtml.cs (ToastMessage reading)
- Cards/Index.cshtml (toast display)
- Edit.cshtml.cs (existing edit flow for comparison)

---

## Overall Quality Gate: BLOCKED

**Reason:** Multiple STOP-severity findings in the save pipeline and SQL query construction chain that prevent saved cards from rendering on the dashboard.

---

## Findings Summary

| Severity | Count |
|----------|-------|
| STOP     | 4     |
| CAUTION  | 4     |
| FLAG     | 2     |
| BASELINE_DEBT | 0 |

---

## Finding 1 (STOP) — SqlQuery contains non-SQL values for non-CustomSQL source types

**Finding ID:** F-001  
**Rule ID:** HARD-SQL-QUERY-CONSTRUCTION  
**Domain:** Save Pipeline / Data Integrity  
**Severity:** STOP  
**Location:** Builder.cshtml.cs:318  
**Evidence:**  
`csharp
// Builder.cshtml.cs line 318
SqlQuery = dto.CustomSql ?? (dto.SourceId ?? ""),
`

Combined with BuildDashboardCard() at line 474:
`csharp
CustomSql = SourceType == "CustomSQL" ? CustomSql : null,
`

**Expected Standard:** SqlQuery must contain a valid SQL query string or a view name that DashboardService.BuildSql() can execute against SQL Server.

**Observed Condition:** For every non-CustomSQL source type (Template, SavedQuery, SqlTable), CustomSql is forced to 
ull by the BuildDashboardCard() ternary expression. The fallback dto.SourceId is **not** a SQL query but rather a table name or identifier:

| SourceType | CustomSql | SourceId | SqlQuery saved to DB | Expected SQL |
|-----------|-----------|----------|---------------------|--------------|
| Template  | null      | table name (e.g., INVENTORY_ITEMS) | "INVENTORY_ITEMS" | SELECT * FROM [INVENTORY_ITEMS] |
| SqlTable  | null      | table name (e.g., INVENTORY_ITEMS) | "INVENTORY_ITEMS" | SELECT * FROM [INVENTORY_ITEMS] |
| SavedQuery| null      | query ID | Some ID string | The actual saved query SQL |
| CustomSQL | actual SQL | (empty)  | Actual SQL ✅ | Actual SQL ✅ |

**Trace through to dashboard display failure:**

1. Card saved with SqlQuery = "INVENTORY_ITEMS" and DataSourceType = "SqlTable"
2. Dashboard Card.cshtml.cs calls DashboardService.GetCardDataByIdAsync(id)
3. GetCardDataAsync() calls BuildSql(card) (DashboardService.cs:88)
4. BuildSql() checks card.DataSourceType.Equals("View") — this is alse (it's "SqlTable")
5. Falls through to eturn card.SqlQuery; → returns "INVENTORY_ITEMS"
6. 
ew SqlCommand("INVENTORY_ITEMS", conn) → SQL Server throws syntax error
7. Exception caught → esult.Status = "error" with sanitized message
8. Client receives { status: "error", errorMessage: "..." }
9. wdRenderCard() renders error state → card body shows error, not data

**Impact:** ALL cards created through the Card Builder with source types Template, SqlTable, or SavedQuery fail to load data on the dashboard. Only CustomSQL cards work (because the actual SQL is saved in customSql and that value is stored in both CustomSql and SourceId fields... wait — actually check: for CustomSQL, dto.CustomSql is NOT null, so dto.CustomSql ?? (dto.SourceId ?? "") returns the actual SQL. But dto.SourceId could also be non-empty for CustomSQL, it's just that dto.CustomSql is prioritized.)

Actually, for CustomSQL mode: BuildDashboardCard() sets CustomSql = CustomSql (the actual SQL text). And SourceId could be whatever the hidden input wb-h-sourceId contains (probably empty or stale). So SqlQuery = actualSql ?? "" = actualSql. This works.

For SqlTable mode: BuildDashboardCard() sets CustomSql = null. SourceId = the table name (e.g., "INVENTORY_ITEMS"). So SqlQuery = null ?? "INVENTORY_ITEMS" = "INVENTORY_ITEMS". This FAILS.

**Recommended Action:** The server-side save logic must construct a valid SQL query based on the source type:

- **SqlTable:** SqlQuery = $"SELECT * FROM [{dto.SourceId}]" and set DataSourceType = "View" (since it's selecting from a table/view)
- **Template:** The template's sqlQueryTemplate should have been resolved server-side (with {TableName} substitution) and the resolved SQL saved as SqlQuery. Set DataSourceType = "SQL Query".
- **SavedQuery:** Resolve the saved query to its SQL text and save that. Set DataSourceType = "SQL Query".
- **CustomSQL:** Current behavior is correct — save the SQL verbatim. Set DataSourceType = "SQL Query".

Alternatively, include the preview SQL (which is properly constructed client-side) in the form submission as customSql for all source types, not just CustomSQL.

**Confidence:** High  
**Blocking:** Yes  
**Blocking Reason:** Cards cannot display data  
**Waiver Allowed:** No  
**Required Owner:** TeraAgent / EngineeringAgent

---

## Finding 2 (STOP) — DataSourceType mapping broken for builder source types

**Finding ID:** F-002  
**Rule ID:** HARD-DATASOURCETYPE-MAPPING  
**Domain:** Save Pipeline / Data Integrity  
**Severity:** STOP  
**Location:** Builder.cshtml.cs:317  
**Evidence:**
`csharp
DataSourceType = dto.SourceType ?? "SQL Query",
`
Where dto.SourceType is one of: "Template", "SavedQuery", "SqlTable", "CustomSQL".

**Expected Standard:** DashboardCard.DataSourceType must be "SQL Query" or "View" (per DashboardCard.cs line 28-29 documentation and the CHECK constraint). DashboardService.BuildSql() explicitly checks for "View" to wrap the query; otherwise it treats SqlQuery as raw SQL.

**Observed Condition:** The builder stores its internal source type names (Template, SavedQuery, etc.) directly into DataSourceType. These values do not match any checked value in BuildSql(). As a result:
- BuildSql() never enters the "View" branch for any builder-created card
- All cards get eturn card.SqlQuery — which for non-CustomSQL is just a table name (see F-001)

**Impact:** Same as F-001 — every builder-created card from non-CustomSQL sources fails to load. The DataSourceType value prevents BuildSql() from applying the correct SQL construction logic.

**Recommended Action:** Map the builder source types to the entity's expected values:
- Template → "SQL Query" (the template resolved to a SQL query)
- SavedQuery → "SQL Query" (resolved to SQL)
- SqlTable → "View" (it's a table/view name to wrap in SELECT * FROM [...])
- CustomSQL → "SQL Query" (raw SQL)

**Confidence:** High  
**Blocking:** Yes  
**Blocking Reason:** Same as F-001  
**Waiver Allowed:** No  
**Required Owner:** TeraAgent / EngineeringAgent

---

## Finding 3 (STOP) — CustomSQL textarea missing input event listener, state.customSql never updates

**Finding ID:** F-003  
**Rule ID:** HARD-CUSTOMSQL-STALE-STATE  
**Domain:** Save Pipeline  
**Severity:** STOP  
**Location:** card-builder.js  
**Evidence:**
- cleanupDuplicateNames() (line 1034): removes 
ame attribute from wb-custom-sql textarea, meaning the textarea value is never submitted natively
- syncHiddenInputs() (line 1047): updates hidden input from s.customSql state
- There is **no event listener** on wb-custom-sql that updates 	his.state.customSql when the user types
- Compare with wb-title and wb-display-name (lines 499-500) which DO have input listeners

**Expected Standard:** When the user types custom SQL, state.customSql must be updated so that syncHiddenInputs() captures the typed value into the hidden input before form submission.

**Observed Condition:** The data flow is:
1. User types SQL in #wb-custom-sql textarea
2. state.customSql remains the **initial page-load value** (likely empty string)
3. syncHiddenInputs() sets wb-h-customSql.value = s.customSql → stale/empty
4. Form submits empty customSql → SqlQuery saved as empty string

**Impact:** CustomSQL cards cannot be saved with user-typed SQL. The query is always empty/stale. Combined with F-001, this means ALL builder-created cards fail.

**Recommended Action:** Add an input event listener to wb-custom-sql:
`javascript
var customSql = wb-custom-sql;
if (customSql) {
    customSql.addEventListener('input', function () {
        self.state.customSql = customSql.value;
    });
}
`

**Confidence:** High  
**Blocking:** Yes  
**Blocking Reason:** CustomSQL mode completely broken  
**Waiver Allowed:** No  
**Required Owner:** TeraAgent / EngineeringAgent

---

## Finding 4 (STOP) — Save confirmation message lost between Builder and Cards/Index

**Finding ID:** F-004  
**Rule ID:** HARD-TEMPDATA-KEY-MISMATCH  
**Domain:** UX / Save Confirmation  
**Severity:** STOP  
**Location:** Builder.cshtml.cs:344-346 → Cards/Index.cshtml.cs:39-43  
**Evidence:**

Builder.cshtml.cs sets:
`csharp
TempData["SuccessMessage"] = "تم حفظ البطاقة بنجاح.";
`

Cards/Index.cshtml.cs reads:
`csharp
if (TempData["ToastMessage"] is string message)
{
    ToastMessage = message;
    ToastType = TempData["ToastType"] as string ?? "success";
}
`

**Expected Standard:** The redirect target page must read the TempData key that the source page wrote. Either both should use "SuccessMessage" or both should use "ToastMessage" (and include "ToastType").

**Observed Condition:** The Builder writes TempData["SuccessMessage"], but Cards/Index reads TempData["ToastMessage"]. The success message key is **never read**. When the user saves and is redirected to the cards list, no success toast appears.

**Impact:** User receives no visual confirmation after saving. The page silently loads, and the user may think the save failed or the card was lost.

**Recommended Action:** Change Builder.cshtml.cs lines 344-346 to use the same TempData keys that Cards/Index.cshtml.cs expects:
`csharp
TempData["ToastMessage"] = action == "saveAndAddAnother"
    ? "تم حفظ البطاقة. جاري إنشاء بطاقة جديدة..."
    : "تم حفظ البطاقة بنجاح.";
TempData["ToastType"] = "success";
`

**Confidence:** High  
**Blocking:** Yes  
**Blocking Reason:** Users get zero feedback after save operation  
**Waiver Allowed:** No  
**Required Owner:** TeraAgent / EngineeringAgent

---

## Finding 5 (CAUTION) — "التالي" button visible (though disabled) on last step

**Finding ID:** F-005  
**Rule ID:** UX-BUTTON-VISIBILITY  
**Domain:** UX  
**Severity:** CAUTION  
**Location:** card-builder.js:907-920 (updateFooter)  
**Evidence:**
`javascript
if (next) {
    var canNext = this.validateStepSilent(step) && step < maxStep;
    next.disabled = !canNext;
    next.setAttribute('aria-disabled', canNext ? 'false' : 'true');
}
`

**Expected Standard:** On the final step, the "التالي" (Next) button should be hidden, not just disabled. A disabled button that the user can never use adds visual clutter and may confuse users.

**Observed Condition:** At step 5 (or step 4 when KPI is not visible), step < maxStep is alse (5 < 5), so canNext is alse. The button is disabled but still visible in the footer next to the enabled Save buttons. From Builder.cshtml lines 496-499:
`html
<button type="button" class="wd-btn wd-btn--primary wb-btn-next" id="wb-btn-next" disabled aria-disabled="true">
    التالي
    <svg class="wb-icon" aria-hidden="true"><use href="#wb-icon-chevron-left"></use></svg>
</button>
`

**Impact:** Minor UX confusion. The user may wonder why "التالي" is shown at the last step.

**Recommended Action:** Add CSS or a class to hide the next button on the final step:
`javascript
if (next) {
    if (step >= maxStep) {
        next.style.display = 'none';
    } else {
        next.style.display = '';
        next.disabled = !canNext;
    }
}
`

**Confidence:** High  
**Blocking:** No  
**Waiver Allowed:** Yes  
**Required Owner:** TeraAgent

---

## Finding 6 (CAUTION) — "saveAndAddAnother" redirects back to Builder but Builder doesn't display TempData success message

**Finding ID:** F-006  
**Rule ID:** UX-SAVE-ADD-ANOTHER-FEEDBACK  
**Domain:** UX / Save Confirmation  
**Severity:** CAUTION  
**Location:** Builder.cshtml.cs:348-350  
**Evidence:**
`csharp
if (action == "saveAndAddAnother")
{
    return RedirectToPage("/admin-secure-panel/Cards/Builder");
}
`

The Builder page is a PageModel that only sets TempData["SuccessMessage"] (never reads it on OnGetAsync). Unlike Cards/Index which has a ToastMessage display mechanism, Builder.cshtml has no @section Toast or temp-data display.

**Expected Standard:** After "Save and Add Another" redirects back to the Builder page, a success toast or banner should confirm the previous card was saved.

**Observed Condition:** The redirect lands on a fresh Builder page. The user sees a blank slate with no indication their previous card was saved successfully.

**Impact:** Users may not know whether the save completed. They might navigate away to check, losing their new card configuration.

**Recommended Action:** Either:
- Read TempData["SuccessMessage"] in Builder.OnGetAsync() and pass it to the view for display as a toast/alert, or
- Redirect to /admin-secure-panel/Cards/Index with TempData["ToastMessage"] set (like the normal save path)

**Confidence:** High  
**Blocking:** No  
**Waiver Allowed:** Yes  
**Required Owner:** TeraAgent

---

## Finding 7 (CAUTION) — Preview SQL is properly constructed but not submitted for save

**Finding ID:** F-007  
**Rule ID:** DEFAULT-PREVIEW-SAVE-GAP  
**Domain:** Save Pipeline / Architecture  
**Severity:** CAUTION  
**Location:** card-builder.js:578-586 (getPreviewSql) vs Builder.cshtml.cs:318  
**Evidence:**
`javascript
// getPreviewSql() correctly constructs SQL per source type:
switch (this.state.sourceType) {
    case 'CustomSQL': return (this.state.customSql || '').trim();
    case 'SqlTable': return this.state.selectedTable ? ('SELECT TOP 10 * FROM [' + this.state.selectedTable.sqlTargetTable + ']') : '';
    case 'Template': return (this.state.previewSql || '').trim();
    case 'SavedQuery': return '';
}
`

But on save, only customSql (hidden input) and sourceId (hidden input) are submitted. The properly constructed preview SQL is never sent to the server.

**Expected Standard:** The server should receive the actual SQL query to save. The client-side or server-side should construct the SQL from the source type and identifier.

**Observed Condition:** The preview path builds correct SQL but the save path does not use it. The server receives only raw identifiers (table names, template IDs) that it cannot execute as SQL.

**Recommended Action (architectural fix):** Add the resolved SQL to the form submission. Either:
1. **Save the preview SQL client-side:** In syncHiddenInputs(), compute the SQL string (same logic as getPreviewSql()) and store it in a hidden input like wb-h-sqlQuery that the server reads.
2. **Resolve server-side:** The server should have logic to construct the SQL query from SourceType + SourceId/CustomSql, similar to what the client does in getPreviewSql().

Option 1 is simpler and less error-prone since the client already has the logic.

**Confidence:** Medium  
**Blocking:** No (this is a design finding; F-001/F-002 are the concrete bugs)  
**Waiver Allowed:** Yes  
**Required Owner:** TeraAgent

---

## Finding 8 (CAUTION) — No event listener on CustomSQL textarea prevents state sync

**Finding ID:** F-008  
**Rule ID:** DEFAULT-INPUT-SYNC  
**Domain:** Save Pipeline  
**Severity:** CAUTION  
**Location:** card-builder.js (missing listener for wb-custom-sql)  
**Evidence:**
- wb-title has an input listener (line 499)
- wb-display-name has an input listener (line 500)
- Grid inputs have input listeners (lines 507-511)
- wb-custom-sql has **no** input/change listener

**Expected Standard:** All user-modifiable input fields should sync their values to 	his.state to ensure syncHiddenInputs() captures the latest values.

**Observed Condition:** See F-003 for the detailed analysis. This finding is the architectural pattern gap, while F-003 is the concrete bug.

**Impact:** Any custom SQL typed by the user is lost on save. The preview also uses outdated SQL.

**Recommended Action:** Add input listener as described in F-003, or use a delegated change listener pattern for all form inputs.

**Confidence:** High  
**Blocking:** No  
**Waiver Allowed:** Yes  
**Required Owner:** TeraAgent

---

## Finding 9 (FLAG) — Index.cshtml.cs CardLayoutInfo does not include SqlQuery (by design, but worth noting)

**Finding ID:** F-009  
**Rule ID:** DEFAULT-PROJECTION-AWARENESS  
**Domain:** Dashboard Display  
**Severity:** FLAG  
**Location:** Index.cshtml.cs:57-60  
**Evidence:**
`csharp
.Select(c => new CardLayoutInfo(
    c.Id, c.Title, c.ChartType, c.GridPositionX, c.GridPositionY,
    c.GridWidth, c.GridHeight, c.RefreshInterval))
.ToListAsync();
`

**Expected Standard:** The card index projection does NOT include SqlQuery, DataSourceType, or other query-related fields. This is correct by design — card data is loaded asynchronously by the client from /api/dashboard/card/{id}.

**Observed Condition:** CardLayoutInfo is a lean projection with only layout metadata. This is proper architecture (CQRS-like separation of config and data). No bug here — the issue is downstream in the API layer where SqlQuery/DataSourceType are invalid.

**Impact:** None (informational). The dashboard grid renders correctly with card positions and sizes. The actual data failure happens when the client calls the API.

**Confidence:** High  
**Blocking:** No  
**Waiver Allowed:** Yes  
**Required Owner:** N/A

---

## Finding 10 (FLAG) — step 4 KPI hidden input wb-h-kpiMode exists in Builder.cshtml but not in the main hidden inputs section

**Finding ID:** F-010  
**Rule ID:** DEFAULT-HIDDEN-DISTRIBUTION  
**Domain:** Save Pipeline  
**Severity:** FLAG  
**Location:** Builder.cshtml:228  
**Evidence:** The wb-h-kpiMode hidden input is at line 228 inside the KPI step section (fieldset step 4), while all other hidden inputs are at lines 397-421 in the dedicated hidden-inputs section at the end of the form.

**Expected Standard:** All hidden inputs should be in one predictable location for maintainability.

**Observed Condition:** wb-h-kpiMode at line 228 is inside the Step 4 fieldset. It works functionally (it's still inside the form), but it's an outlier. Other KPI hidden inputs (wb-h-valueColumn, etc.) are at lines 409-421. This inconsistency could cause confusion during maintenance.

**Impact:** None functionally. Low maintainability concern.

**Recommended Action:** Move wb-h-kpiMode to the hidden inputs section (lines 408-421) for consistency.

**Confidence:** High  
**Blocking:** No  
**Waiver Allowed:** Yes  
**Required Owner:** TeraAgent

---

## Critical Question Answer: Why does the saved card not appear on the dashboard?

**Complete trace from clicking "Save" to dashboard rendering:**

### Save Phase (Builder → DB)

1. **User clicks "حفظ"** → wireSave() fires submitForm('save')
2. **submitForm() calls syncHiddenInputs()** — copies 	his.state values into hidden inputs (#wb-h-cardType, #wb-h-sourceType, #wb-h-sourceId, #wb-h-customSql, etc.)
3. **Form submits via native POST** to /admin-secure-panel/Cards/Builder
4. **BuilderModel.OnPostAsync("save")** (Builder.cshtml.cs:296) receives bound properties:
   - CardType = "KPI", SourceType = "SqlTable", SourceId = "INVENTORY_ITEMS", CustomSql = "" (empty, because not CustomSQL mode)
5. **BuildDashboardCard()** creates DTO — sets CustomSql = null (because SourceType != "CustomSQL"), SourceId = "INVENTORY_ITEMS"
6. **Entity created** (line 313-340):
   - DataSourceType = "SqlTable" ❌ (should be "View" or "SQL Query")
   - SqlQuery = null ?? "INVENTORY_ITEMS" = "INVENTORY_ITEMS" ❌ (should be "SELECT * FROM [INVENTORY_ITEMS]")
7. **Saved to DB** with invalid SqlQuery and wrong DataSourceType
8. **TempData set**: TempData["SuccessMessage"] = "تم حفظ البطاقة بنجاح." ❌ (should be TempData["ToastMessage"])
9. **Redirect** to /admin-secure-panel/Cards/Index
10. **Cards/Index page loads** — reads TempData["ToastMessage"] → not found → no success toast shown ❌

### Display Phase (DB → Browser)

11. **Index.cshtml.cs.OnGetAsync()** loads CardLayoutInfo projection — layout only, no SqlQuery (correct by design)
12. **Page renders** card grid with data-card-id, data-title, data-chart-type attributes
13. **Client DOMContentLoaded** — iterates WD_CARDS and calls wdLoadCard(id, false)
14. **wdLoadCard() fetches** /api/dashboard/card/{id} (Card.cshtml.cs:30)
15. **Card API calls DashboardService.GetCardDataByIdAsync(id)** (DashboardService.cs:46)
16. **GetCardDataAsync()** fetches full DashboardCard entity from EF → has SqlQuery = "INVENTORY_ITEMS", DataSourceType = "SqlTable"
17. **BuildSql(card)** (DashboardService.cs:316-326):
    - card.DataSourceType.Equals("View") → false (it's "SqlTable")
    - Returns card.SqlQuery as-is → returns "INVENTORY_ITEMS"
18. **
ew SqlCommand("INVENTORY_ITEMS", conn)** → SQL Server rejects this as invalid SQL syntax ❌
19. **Exception caught** → esult.Status = "error", esult.ErrorMessage = DataHelper.Sanitize(ex.Message)
20. **API returns JSON** with { status: "error", errorMessage: "Invalid object name 'INVENTORY_ITEMS'." }
21. **Client wdRenderCard()** sees card.status === 'error' → renders wdErrorHtml() in the card body
22. **User sees error state** instead of chart data ❌

### Root Cause Chain

`
Save: SourceType="SqlTable"
  → DataSourceType stored as "SqlTable" (wrong — should be "View")
  → SqlQuery stored as "INVENTORY_ITEMS" (wrong — should be "SELECT * FROM [INVENTORY_ITEMS]")
  → BuildSql() doesn't enter "View" branch
  → Returns bare table name as SQL
  → SQL Server syntax error
  → Card data API returns error
  → Dashboard shows error state
`

The fix requires repairing steps 6-7: either construct proper SQL server-side based on source type, or submit the already-constructed preview SQL from the client alongside the source identifiers.

---

## Priority-Ordered Recommendations

### P0 — Critical (fix immediately)

1. **Fix SqlQuery construction** (F-001/F-002/F-007): Ensure that when a card is saved, SqlQuery contains actual executable SQL, not a table name or ID. The server should construct SELECT * FROM [{SourceId}] for SqlTable sources (and set DataSourceType = "View"), resolve template SQL with {TableName} substitution, and resolve saved queries to their SQL text.

2. **Fix CustomSQL input sync** (F-003/F-008): Add an input event listener to #wb-custom-sql textarea so state.customSql reflects what the user types.

3. **Fix TempData success message** (F-004): Change TempData["SuccessMessage"] to TempData["ToastMessage"] (and add TempData["ToastType"]) in Builder.cshtml.cs so the redirect target shows the confirmation toast.

### P1 — High (fix this iteration)

4. **Fix "saveAndAddAnother" success feedback** (F-006): Either display TempData on the Builder page or redirect to Cards/Index instead.

5. **Hide "التالي" on last step** (F-005): Add conditional display logic in updateFooter() to hide the button on the final step.

### P2 — Medium (next iteration)

6. **Consolidate hidden inputs** (F-010): Move wb-h-kpiMode to the hidden inputs section.

---

*End of Audit Report*
