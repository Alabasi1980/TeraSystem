# QUAUD — TASK-CARD-KPI-04: CategoryColumn Breakdown Table (Top 5)

| Field | Value |
|---|---|
| **Audit ID** | QUAUD-TASK-CARD-KPI-04-2026-07-19-001 |
| **Task Reviewed** | TASK-CARD-KPI-04 |
| **Invoked By** | TeraAgent |
| **Audit Mode** | Standard |
| **Scope** | Changed Code (4 files) |
| **Report Path** | `project-control/audit-reports/QUAUD-TASK-CARD-KPI-04-2026-07-19-001.md` |
| **Date** | 2026-07-19 |

---

## Evidence Sources Used

- Task file: `TASK-CARD-KPI-04.md`
- `KpiQueryBuilder.cs` (251 lines)
- `CardDataResult.cs` (66 lines)
- `DashboardService.cs` (534 lines)
- `Index.cshtml` (1809 lines)
- `dashboard-utils.js` (93 lines)
- `QUALITY_GATE_THRESHOLDS.md`
- `ENGINEERING_REVIEW_CHECKLIST.md`
- Build evidence: 0 warnings, 0 errors (provided by orchestrator)

---

## Overall Quality Gate: BLOCKED

| Condition | Result |
|---|---|
| Open STOP findings | **1** |
| Open CAUTION findings | 0 |
| Open FLAG findings | 1 (advisory) |
| Overall | **BLOCKED** |

---

## Findings Summary

| Severity | Count |
|---|---|
| STOP | 1 |
| CAUTION | 0 |
| FLAG | 1 |
| BASELINE_DEBT | 1 |

---

## Finding 1 — STOP

| Field | Value |
|---|---|
| **Finding ID** | QUAUD-KPI04-001 |
| **Rule ID** | QG-FUNC-001 (Functional correctness — undefined reference) |
| **Domain** | Frontend / Runtime Correctness |
| **Severity** | STOP |
| **Location** | `Index.cshtml`, line 995 |
| **Evidence** | `escHtml` is called at line 995 of `Index.cshtml`, but this function is **not defined anywhere** in the application. The shared utility in `dashboard-utils.js` (line 42, exported at line 88) defines `escapeHtml`. A grep across the entire application found `escHtml` appears only in the task specification and the implementation — never as a function definition. |
| **Expected Standard** | HTML-escaped output using the project's `escapeHtml()` utility (defined in `dashboard-utils.js` and exposed as `window.escapeHtml`). All other occurrences in `Index.cshtml` (lines 819, 903, 957, 958, 1236, 1240, 1310, 1312) correctly use `escapeHtml`. |
| **Observed Condition** | `escHtml(category)` is called instead of `escapeHtml(category)`. This will throw `ReferenceError: escHtml is not defined` at runtime whenever category breakdown data is present and `wdRenderCategoryBreakdown` attempts to render it. |
| **Impact** | **Critical functional failure.** The category breakdown table will never render. The `ReferenceError` is caught by the `try-catch` in `wdRenderCard` (line 1020), which replaces the **entire KPI card body** with an error state (`wdErrorHtml`). This means not only does the breakdown fail — the main KPI value, change badge, sparkline, and grand total that were already rendered into the DOM are **overwritten with an error**. This violates: Acceptance Criterion #1 (table appears), #9 (no side effects — causes card regression). |
| **Recommended Action** | Change `escHtml` to `escapeHtml` on line 995 of `Index.cshtml`. One-character fix. |
| **Changed Code / Baseline** | New in this diff (introduced by this task). Not baseline debt. |
| **Confidence** | High |
| **Blocking** | Yes |
| **Blocking Reason** | Prevents the primary feature from functioning and causes regression in the entire KPI card. |
| **Waiver Allowed** | No — ordinary waiver not appropriate for undefined function reference that crashes the feature. |
| **Required Owner** | EngineeringAgent (fix task) |
| **Referral** | N/A |
| **Status** | **Open** |

---

## Finding 2 — FLAG (Advisory)

| Field | Value |
|---|---|
| **Finding ID** | QUAUD-KPI04-002 |
| **Rule ID** | QG-UI-001 (UI polish — empty container artifact) |
| **Domain** | Frontend / CSS |
| **Severity** | FLAG |
| **Location** | `Index.cshtml`, lines 835-837 (container creation) and lines 317-350 (CSS) |
| **Evidence** | In `wdKpiHtml` (line 835-837), a breakdown container `<div class="wd-kpi__breakdown">` is created for all `composite` and `withChange` KPI cards, regardless of whether breakdown data exists. The CSS class `.wd-kpi-breakdown` (used on the inner content div) applies `margin-top: 8px`, `border-top: 1px solid rgba(255,255,255,0.1)`, and `padding-top: 6px`. When no breakdown data is present, the outer container remains in the DOM as an empty div — while unstyled itself, the border artifact will appear once data renders. More importantly, the container is **always visible** in the DOM, which could cause layout space issues in tight cards. |
| **Expected Standard** | The breakdown container should be hidden by default and only shown when `wdRenderCategoryBreakdown` populates it with data. |
| **Observed Condition** | Empty breakdown container is always present in composite/withChange KPI cards. |
| **Impact** | Minor visual artifact — faint separator line visible in cards without breakdown data. Does not affect functionality. |
| **Recommended Action** | Add `display:none` to the container's initial state, or remove the container creation from `wdKpiHtml` and create it dynamically in `wdRenderCategoryBreakdown`. Low priority. |
| **Changed Code / Baseline** | New in this diff. |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | N/A (advisory) |
| **Referral** | N/A |
| **Status** | **Open** |

---

## Finding 3 — BASELINE_DEBT

| Field | Value |
|---|---|
| **Finding ID** | QUAUD-KPI04-003 |
| **Rule ID** | QG-HEUR-002 (File size heuristic) |
| **Domain** | Code / Structure |
| **Severity** | BASELINE_DEBT (not introduced by this task) |
| **Location** | `DashboardService.cs` (534 lines), `Index.cshtml` (1809 lines) |
| **Evidence** | `DashboardService.cs` is 534 lines (above 500-line Caution heuristic). `Index.cshtml` is 1809 lines (well above any threshold). Both files were already large before this task. The current diff added ~22 lines to `DashboardService.cs` and ~85 lines to `Index.cshtml`. This task did not introduce the size issue. |
| **Expected Standard** | QUALITY_GATE_THRESHOLDS.md §4: File length Flag candidate at 300+ lines, Caution at 500+ lines. Context override: structurally coherent single-responsibility files may be classified as FLAG. |
| **Observed Condition** | Pre-existing file sizes, not worsened by this task. |
| **Impact** | Maintenance overhead but not introduced by this change. |
| **Recommended Action** | Track separately. `Index.cshtml` inline JS is a known debt area (see TASK-COD-FIX-001 which tracks JS duplication). No action required for this task. |
| **Changed Code / Baseline** | BASELINE_DEBT — pre-existing. |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | N/A |
| **Referral** | N/A |
| **Status** | **Baseline** |

---

## Verification Checklist

| # | Check | Result | Notes |
|---|---|---|---|
| 1 | KpiQueryBuilder: `BuildCategoryBreakdownQuery` exists | ✅ PASS | Lines 135-153. Matches task spec. |
| 2 | KpiQueryBuilder: Method wired into `Build()` | ✅ PASS | Lines 54-58. Condition: CategoryColumn + ValueColumn set, inside KPI+non-simple guard. |
| 3 | KpiQueries: `BreakdownSql` property exists | ✅ PASS | Line 250. |
| 4 | CardDataResult: `KpiCategoryBreakdown` property exists | ✅ PASS | Line 62. Correct type `List<Dictionary<string, object?>>?`. |
| 5 | DashboardService: Breakdown query execution | ✅ PASS | Lines 259-281. Uses `ExecuteQueryAsync`. |
| 6 | DashboardService: Percentage calculation | ✅ PASS | Lines 267-273. `(val / total) * 100` with zero-division guard. |
| 7 | Index.cshtml: `wdRenderCategoryBreakdown` function exists | ✅ PASS | Lines 979-1003. |
| 8 | Index.cshtml: CSS exists | ✅ PASS | Lines 317-350. All BEM classes present. |
| 9 | Index.cshtml: Called in `wdRenderCard` | ✅ PASS | Lines 1034-1040. Correctly checks data existence before calling. |
| 10 | Index.cshtml: `escHtml` → `escapeHtml` | ❌ **FAIL** | Line 995 uses undefined `escHtml`. Will crash at runtime. **STOP finding QUAUD-KPI04-001.** |
| 11 | No unrelated code changed | ✅ PASS | All changes within task scope. |
| 12 | No secrets | ✅ PASS | No hardcoded secrets, credentials, or connection strings. SQL uses `SanitizeIdentifier` consistently. |
| 13 | No schema changes | ✅ PASS | Only DTO property addition. No database migrations. |
| 14 | Build: 0 warnings, 0 errors | ✅ PASS | Confirmed by orchestrator. |
| 15 | SQL injection protection | ✅ PASS | `SanitizeIdentifier` used for category/value columns. Date filter uses `yyyy-MM-dd` format. |
| 16 | Error handling consistency | ✅ PASS | Breakdown catch block intentionally silent (consistent with "don't fail the card" requirement). |

---

## Handback to Orchestrator

| Field | Value |
|---|---|
| **Status** | BLOCKED |
| **Report Path** | `project-control/audit-reports/QUAUD-TASK-CARD-KPI-04-2026-07-19-001.md` |
| **Blocking Findings** | QUAUD-KPI04-001: `escHtml` is undefined on line 995 of `Index.cshtml`. Must be changed to `escapeHtml`. |
| **Recommended Next Action** | Fix the typo on line 995 of `Index.cshtml`: change `escHtml(category)` to `escapeHtml(category)`. Rebuild and re-audit. |

---

> **Prepared by:** Auditor Agent (مُدقق)
> **Mode:** Standard diff-first audit
> **Conduct gate:** Read and passed (TERA_AGENT_CONDUCT.md)
