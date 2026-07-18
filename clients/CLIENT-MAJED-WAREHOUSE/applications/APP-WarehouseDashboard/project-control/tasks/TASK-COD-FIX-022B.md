# TASK-COD-FIX-022B — Fix Optional BindProperty Required Validation

> **Status:** Accepted — Code fix complete, browser save verification pending  
> **Created:** 2026-07-18  
> **Approved By:** Majed  
> **Owner:** TeraAgent  
> **Assigned Agent:** EngineeringAgent  
> **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## 1. Objective

Fix the current Card Builder save failure caused by optional wizard fields being treated as required by ASP.NET Core model validation.

After TASK-COD-FIX-022A, browser-visible ModelState errors are:

```text
The CustomSql field is required.
The FixedStartDate field is required.
The FixedEndDate field is required.
The CloneId field is required.
The TemplateId field is required.
```

These fields are optional for the current save scenario and must not block `DashboardCards` insertion.

---

## 2. Root Cause

With nullable reference types enabled, ASP.NET Core treats non-nullable `string` properties as implicitly required during model binding/validation. Several optional wizard fields are declared as non-nullable `string`, so empty/missing values invalidate `ModelState` before `SaveChangesAsync()`.

---

## 3. Scope

### In Scope

1. Mark optional string BindProperty fields as nullable or otherwise prevent implicit required validation.
2. Preserve explicit `[Required]` validation for truly required fields only: `Title`, `DisplayName`, and any essential canonical fields if already required by logic.
3. Add conditional validation only where needed:
   - `CustomSql` required only when `SourceType == "CustomSQL"`.
   - `FixedStartDate` / `FixedEndDate` required only when `DateFilterMode == "fixed"` and the selected KPI mode/path actually uses fixed dates.
4. Keep existing safe diagnostics from TASK-COD-FIX-022A.
5. Build verification.

### Out of Scope

1. Do not migrate Syncfusion preview rendering to Chart.js.
2. Do not fix SqlTable `DataSourceType` dashboard rendering issue in this task.
3. Do not fix KPI SQL injection in this task.
4. Do not change database schema or migrations.
5. Do not add packages.
6. Do not redesign UI or wizard steps.
7. Do not disable antiforgery validation.

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-COD-FIX-022B.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\audit-reports\QUAUD-CARDBUILDER-001-2026-07-18-001.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\tera-system\profiles\dotnet-razorpages-adonet.md`

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml` only if required for cache-busting or minimal validation display; do not redesign.

---

## 6. Acceptance Criteria

1. `CustomSql`, `FixedStartDate`, `FixedEndDate`, `CloneId`, and `TemplateId` no longer fail ModelState when omitted/empty in a normal `SqlTable` KPI save.
2. `Title` and `DisplayName` remain explicitly required.
3. `CustomSql` is conditionally required only for `SourceType == "CustomSQL"`.
4. Fixed dates are conditionally required only when `DateFilterMode == "fixed"`.
5. Existing safe diagnostics remain and do not log tokens/secrets.
6. No DB schema, migration, package, or preview migration changes occur.
7. `dotnet build` succeeds, or fallback build succeeds if the running app locks normal output.

---

## 7. Security Sensitivity

- **Level:** Low/Medium
- **Reason:** Adjusts server validation and preserves diagnostics. Does not alter auth, sessions, roles, secrets, or database schema.
- **SecurityAgent:** Not required if no security behavior is weakened and no sensitive values are logged.

---

## 8. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Focused validation fix. |
| One objective only | PASS | Optional fields must not block save. |
| No deferrable work included | PASS | Other audit fixes deferred. |
| No UI unless explicitly requested | PASS | Only minimal display/cache-bust allowed if necessary. |
| No API unless explicitly requested | PASS | No API changes. |
| No Auth unless explicitly requested | PASS | Antiforgery preserved. |
| No schema/migration unless explicitly requested | PASS | No DB changes. |
| No real secrets outside approved local environment files | PASS | No secrets. |
| Secret handling plan documented and redacted | PASS | Do not log token values. |
| CLI side effects checked | PASS | Build output only. |
| No internal contradiction between constraints and outputs | PASS | Scope and write targets align. |
| Allowed Write Targets are narrow | PASS | Primarily one PageModel file. |
| Acceptance criteria are testable | PASS | Browser save + ModelState logs + build. |

**Gate Status:** PASS

---

## 9. Delegation Notes

EngineeringAgent must:

1. Read current files from disk before editing.
2. Preserve unrelated changes.
3. Use the smallest safe fix.
4. Avoid broad refactors.
5. Do not disable validation globally.
6. Do not remove explicit `[Required]` from Title/DisplayName.
7. Keep diagnostics safe and redacted.

---

## 10. Handback Placeholder

## 10. Engineering Handback

- **Task ID:** TASK-COD-FIX-022B
- **Agent:** EngineeringAgent
- **Status:** Done
- **Files Modified:**
  - `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs`
- **Exact Change Summary:**
  - Made optional wizard `BindProperty` string fields nullable to prevent ASP.NET Core implicit required validation from blocking normal saves.
  - Preserved explicit `[Required]` validation on `Title` and `DisplayName`.
  - Added conditional validation before save:
    - `CustomSql` required only when `SourceType == "CustomSQL"`.
    - `FixedStartDate` and `FixedEndDate` required only when `DateFilterMode == "fixed"`.
  - Added safe null coalescing when building `DashboardCardDto`.
  - Existing safe diagnostics from TASK-COD-FIX-022A remain unchanged.
- **Fields Made Nullable/Optional:**
  - `SourceId`, `CustomSql`, `ValueColumn`, `DateColumn`, `CategoryColumn`, `FixedStartDate`, `FixedEndDate`, `CloneId`, `TemplateId`.
- **Commands Run:**
  - `dotnet build` — blocked by running app executable lock.
  - fallback `dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\warehouse-dashboard-build-022b` — succeeded with 0 warnings and 0 errors.
- **Issues/Risks:**
  - Browser save/database insert verification still required.
  - Existing out-of-scope findings remain: SqlTable dashboard rendering, Syncfusion/Chart.js preview migration, KPI SQL injection.

---

## 11. Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | Only `Builder.cshtml.cs` changed. |
| No unauthorized files created | PASS | Only Tera control task file was created. |
| No unauthorized files deleted | PASS | None. |
| No unauthorized packages added | PASS | None. |
| No unauthorized UI/CSS/theme changes | PASS | None. |
| UI Acceptance Gate passed for UI tasks | N/A | Server validation fix. |
| No real secrets outside approved local environment files | PASS | No secrets involved. |
| Secrets redacted in docs/logs/config references | PASS | No tokens/secrets logged. |
| No unauthorized ORM models/entities/migrations | PASS | None. |
| No unapproved business validation moved to DB constraints | PASS | None. |
| No unauthorized API/Auth created | PASS | No auth/API behavior weakened. |
| Acceptance criteria satisfied | PASS | Optional validation fixed, conditional validation added, fallback build passed. |
| CLI side effects reviewed | PASS | Build output only; fallback temp path used due running process lock. |
| Task file and core project-control records reviewed | PASS | Task, registry, and activity log updated. |
| No secret leakage in task files/logs/reports/handbacks | PASS | None found. |
| No duplicate project-control IDs created | PASS | New ID: TASK-COD-FIX-022B. |
| Any out-of-target changes classified | N/A | None. |
| Independent review decision recorded | PASS | See below. |
| Auditor review decision recorded | PASS | NOT_REQUIRED for focused validation fix. |

**Gate Status:** PASS

**Independent Review:**
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required
- QAAndAcceptanceAgent: Recommended if browser save still fails
- Auditor: NOT_REQUIRED — focused fix for directly observed ModelState errors

**Final Task Decision:** Accepted as code fix. Browser save verification remains the next operational step.
