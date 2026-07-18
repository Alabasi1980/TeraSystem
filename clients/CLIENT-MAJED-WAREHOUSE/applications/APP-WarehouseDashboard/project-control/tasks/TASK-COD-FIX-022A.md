# TASK-COD-FIX-022A — Fix Card Builder ModelState Save Return + Safe Save Diagnostics

> **Status:** Accepted — Code fix complete, browser save verification pending  
> **Created:** 2026-07-18  
> **Approved By:** Majed  
> **Owner:** TeraAgent  
> **Assigned Agent:** EngineeringAgent  
> **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## 1. Objective

Fix the current Card Builder save failure where the Step 5 POST returns HTTP 200, resets the wizard to Step 1, and inserts no row into `DashboardCards`.

Browser Network evidence from Majed shows:

```text
action = save
sourceType = SqlTable
sqlQuery = SELECT * FROM [stg_st_invoice]
valueColumn = FINAL_SUM_INVOICE
gridX = [empty]
gridY = [empty]
__RequestVerificationToken = present
```

This means the previous antiforgery blocker is resolved. The current likely blocker is `ModelState.IsValid == false` before save because empty `gridX`/`gridY` values are bound to non-nullable `int` properties.

---

## 2. Source Findings

- **Audit Report:** `project-control/audit-reports/QUAUD-CARDBUILDER-001-2026-07-18-001.md`
- **Relevant Findings:**
  - F-006 — `GridX`/`GridY` are `int` while the form submits empty values.
  - F-007 — `wb-source-type` remains named and causes duplicate `sourceType` posting.
  - F-015 — form returns validation errors without clear field-level visibility.

---

## 3. Scope

### In Scope

1. Fix `gridX`/`gridY` binding so empty values do not invalidate `ModelState`.
2. Remove duplicate posting for visible source type control by updating `cleanupDuplicateNames()`.
3. Add safe, temporary/diagnostic server-side logging around `OnPostAsync`:
   - POST started.
   - `action` value.
   - `ModelState.IsValid` result.
   - ModelState error field names and sanitized messages.
   - Whether `SqlQuery` is present or empty, without logging full SQL if not necessary.
   - Before and after `_db.SaveChangesAsync()`.
4. Improve visible failure feedback enough so a failed save is not silent.

### Out of Scope

1. Do not migrate Syncfusion preview rendering to Chart.js.
2. Do not fix KPI SQL injection in this task.
3. Do not change database schema or run migrations.
4. Do not add packages.
5. Do not redesign UI or wizard steps.
6. Do not implement the full pre-save preview feature in this task; this is a focused save-path fix and diagnostics task.

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-COD-FIX-022A.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\audit-reports\QUAUD-CARDBUILDER-001-2026-07-18-001.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\tera-system\profiles\dotnet-razorpages-adonet.md`

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js`

---

## 6. Acceptance Criteria

1. Submitting empty `gridX`/`gridY` no longer makes `ModelState` invalid.
2. Duplicate `sourceType` form posting is removed or made harmless by ensuring the hidden field is canonical.
3. If `ModelState` fails, server logs identify the failing field names and sanitized messages.
4. Save path logs show whether `_db.SaveChangesAsync()` is reached.
5. No real secrets or full tokens are logged.
6. No database schema, migration, package, or preview migration changes occur.
7. `dotnet build` succeeds, or fallback build succeeds if the running app locks normal output.

---

## 7. Security Sensitivity

- **Level:** Medium
- **Reason:** Adds logging around save flow and touches form submission behavior. Must not log secrets, antiforgery token values, credentials, or full sensitive payloads.
- **SecurityAgent:** Not required for this focused diagnostic save-path task if logs are sanitized and token values are not recorded.

---

## 8. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Focused on save ModelState failure and diagnostics. |
| One objective only | PASS | Make current save failure visible/fixable and allow save path to proceed. |
| No deferrable work included | PASS | Chart.js and SQL injection fixes deferred. |
| No UI unless explicitly requested | PASS | Only minimal validation visibility if needed; no redesign. |
| No API unless explicitly requested | PASS | No API endpoint changes. |
| No Auth unless explicitly requested | PASS | Antiforgery stays enabled. |
| No schema/migration unless explicitly requested | PASS | No DB changes. |
| No real secrets outside approved local environment files | PASS | No secrets. |
| Secret handling plan documented and redacted | PASS | Do not log token values or credentials. |
| CLI side effects checked | PASS | Build output only. |
| No internal contradiction between constraints and outputs | PASS | Allowed files align with scope. |
| Allowed Write Targets are narrow | PASS | Three existing files only. |
| Acceptance criteria are testable | PASS | Network POST, logs, build, and DB insert can verify. |

**Gate Status:** PASS

---

## 9. Delegation Notes

EngineeringAgent must:

1. Read current files from disk before editing.
2. Preserve unrelated changes.
3. Prefer minimal fix: either make `GridX`/`GridY` nullable on server or ensure the hidden inputs submit a safe sentinel such as `-1`. Choose the cleanest low-risk option and document the choice.
4. Add `wb-source-type` to `cleanupDuplicateNames()` if appropriate so only hidden `wb-h-sourceType` posts as canonical.
5. Add sanitized logging only. Do not log real tokens, secrets, or credentials.
6. Do not modify Chart.js/Syncfusion preview logic in this task.

---

## 10. Handback Placeholder

## 10. Engineering Handback

- **Task ID:** TASK-COD-FIX-022A
- **Agent:** EngineeringAgent
- **Status:** Completed
- **Files Modified:**
  - `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs`
  - `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml`
  - `src/WarehouseDashboard.Web/wwwroot/js/card-builder.js`
- **Exact Change Summary:**
  - Changed server bind properties `GridX` and `GridY` from non-nullable `int` to nullable `int?`.
  - Updated `BuildDashboardCard()` normalization so `null` / empty / negative means automatic placement and `0+` remains valid explicit position.
  - Added `wb-source-type` to `cleanupDuplicateNames()` so hidden `wb-h-sourceType` remains canonical.
  - Changed validation summary to `All` with `role="alert"` and `aria-live="polite"`.
  - Updated `card-builder.js` cache-buster to `v=20260718-022A`.
  - Added safe save diagnostics in `OnPostAsync` without logging token values, secrets, credentials, full connection strings, or full SQL payloads.
- **Chosen gridX/gridY Approach:** Nullable server-side binding (`int?`) because it is low-risk and preserves `0` as a valid explicit coordinate while allowing blank values to bind as automatic placement.
- **Commands Run:**
  - `dotnet build` — succeeded with 0 warnings and 0 errors.
- **Issues/Risks:**
  - Live browser POST/database insert verification still required.
  - Existing out-of-scope findings remain open: Syncfusion preview rendering, KPI SQL injection, and SqlTable DataSourceType mapping for dashboard render.

---

## 11. Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | Only the three allowed application files changed. |
| No unauthorized files created | PASS | Only Tera control task file was created. |
| No unauthorized files deleted | PASS | None. |
| No unauthorized packages added | PASS | None. |
| No unauthorized UI/CSS/theme changes | PASS | Only validation-summary behavior; no design/style redesign. |
| UI Acceptance Gate passed for UI tasks | N/A | Non-visual save-path/validation fix. |
| No real secrets outside approved local environment files | PASS | No secrets involved. |
| Secrets redacted in docs/logs/config references | PASS | No tokens/secrets logged. |
| No unauthorized ORM models/entities/migrations | PASS | None. |
| No unapproved business validation moved to DB constraints | PASS | None. |
| No unauthorized API/Auth created | PASS | Antiforgery remains enabled; no API/auth changes. |
| Acceptance criteria satisfied | PASS | Nullable grid binding, duplicate sourceType cleanup, diagnostics, and build all complete. |
| CLI side effects reviewed | PASS | Build only. |
| Task file and core project-control records reviewed | PASS | Task, registry, and activity log updated. |
| No secret leakage in task files/logs/reports/handbacks | PASS | None found. |
| No duplicate project-control IDs created | PASS | New ID: TASK-COD-FIX-022A. |
| Any out-of-target changes classified | N/A | None. |
| Independent review decision recorded | PASS | See below. |
| Auditor review decision recorded | PASS | NOT_REQUIRED for focused implementation of known Auditor findings F-006/F-007. |

**Gate Status:** PASS

**Independent Review:**
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required — logs are sanitized and no auth/security behavior is weakened
- QAAndAcceptanceAgent: Recommended if browser save still fails after this fix
- Auditor: NOT_REQUIRED — focused fix for already-audited findings

**Final Task Decision:** Accepted as code fix. Browser save verification remains the next operational step.
