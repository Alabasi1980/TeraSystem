# TASK-COD-FIX-021 — Fix Card Builder Save Antiforgery Token

> **Status:** Accepted — Code fix complete, browser save verification pending  
> **Created:** 2026-07-18  
> **Approved By:** Majed  
> **Owner:** TeraAgent  
> **Assigned Agent:** EngineeringAgent  
> **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## 1. Objective

Fix the direct blocker preventing Card Builder Step 5 save from reaching `OnPostAsync`.

Auditor finding F-001 confirmed that the Card Builder form posts without an ASP.NET Core antiforgery token, causing Razor Pages to reject the POST before the server save handler runs.

---

## 2. Source Finding

- **Audit Report:** `project-control/audit-reports/QUAUD-CARDBUILDER-001-2026-07-18-001.md`
- **Finding:** F-001 — No Antiforgery Token
- **Severity:** STOP
- **Root Cause:** `Builder.cshtml` uses a plain POST form without rendering an antiforgery token.

---

## 3. Scope

### In Scope

1. Add the required ASP.NET Core Razor Pages antiforgery token to the Card Builder form.
2. Keep the fix minimal and surgical.
3. Verify the project builds after the change.

### Out of Scope

1. Do not migrate Syncfusion preview code to Chart.js in this task.
2. Do not modify `card-builder.js`.
3. Do not fix KPI SQL injection in this task.
4. Do not change database schema or run migrations.
5. Do not add packages.
6. Do not alter UI design, layout, styling, or step flow.

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Program.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\audit-reports\QUAUD-CARDBUILDER-001-2026-07-18-001.md`

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`

---

## 6. Acceptance Criteria

1. `Builder.cshtml` form includes a valid ASP.NET Core antiforgery token.
2. Native `form.submit()` continues to submit the wizard form normally.
3. No unrelated files are modified.
4. No Syncfusion/Chart.js preview migration is attempted in this task.
5. No database/schema/config/package changes occur.
6. `dotnet build` for `WarehouseDashboard.Web` succeeds, or if blocked by a running app lock, the handback clearly explains the blocker and uses a safe verification fallback.

---

## 7. Security Sensitivity

- **Level:** Medium
- **Reason:** The task touches ASP.NET Core request verification behavior for a POST form, but does not alter authentication, authorization, sessions, roles, or secrets.
- **SecurityAgent:** Not required for this surgical fix; Auditor already identified the missing antiforgery token. Post-execution review must verify no antiforgery protection is disabled.

---

## 8. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | One-line/surgical save blocker fix only. |
| One objective only | PASS | Add antiforgery token to Card Builder form. |
| No deferrable work included | PASS | Chart.js, SQL injection, and other audit findings deferred to later tasks. |
| No UI unless explicitly requested | PASS | No visual UI change intended. |
| No API unless explicitly requested | PASS | No API changes. |
| No Auth unless explicitly requested | PASS | Does not create auth; preserves existing antiforgery validation. |
| No schema/migration unless explicitly requested | PASS | No DB changes. |
| No real secrets outside approved local environment files | PASS | No secrets involved. |
| Secret handling plan documented and redacted | PASS | N/A — no secrets. |
| CLI side effects checked | PASS | `dotnet build` may produce build outputs only. |
| No internal contradiction between constraints and outputs | PASS | Scope and write target align. |
| Allowed Write Targets are narrow | PASS | Single CSHTML file. |
| Acceptance criteria are testable | PASS | Build + browser save verification. |

**Gate Status:** PASS

---

## 9. Delegation Notes

EngineeringAgent must:

1. Before editing the existing file, read the current file from disk.
2. Preserve unrelated changes.
3. Make only the minimal antiforgery-token fix.
4. Do not disable antiforgery validation.
5. Do not edit JavaScript, C#, CSS, migrations, packages, or config in this task.

---

## 10. Handback Placeholder

## 10. Engineering Handback

- **Task ID:** TASK-COD-FIX-021
- **Agent:** EngineeringAgent
- **Status:** Completed
- **Files Modified:**
  - `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml`
- **Exact Change Summary:**
  - Added `@Html.AntiForgeryToken()` immediately inside the existing Card Builder POST form.
  - No JavaScript, C#, CSS, config, package, migration, database, or preview API files were modified.
- **Commands Run:**
  - `dotnet build` — blocked by running `WarehouseDashboard.Web.exe` process lock.
  - `dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\WarehouseDashboard-Web-build-check-TASK-COD-FIX-021` — succeeded with 0 warnings and 0 errors.
  - `git diff -- Builder.cshtml` — confirmed only one line added.
- **Issues/Risks:**
  - Normal build requires stopping the running Web app process.
  - Other audit findings remain open and out of scope.

---

## 11. Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | Only `Builder.cshtml` code file changed. |
| No unauthorized files created | PASS | Only Tera-created control task file added. |
| No unauthorized files deleted | PASS | None. |
| No unauthorized packages added | PASS | None. |
| No unauthorized UI/CSS/theme changes | PASS | No visual/styling changes. |
| UI Acceptance Gate passed for UI tasks | N/A | Non-visual form security token fix. |
| No real secrets outside approved local environment files | PASS | No secrets involved. |
| Secrets redacted in docs/logs/config references | PASS | No secrets referenced. |
| No unauthorized ORM models/entities/migrations | PASS | None. |
| No unapproved business validation moved to DB constraints | PASS | None. |
| No unauthorized API/Auth created | PASS | No API/auth changes; existing antiforgery validation preserved. |
| Acceptance criteria satisfied | PASS | Token added; fallback build succeeded; no unrelated code changes. |
| CLI side effects reviewed | PASS | Build output only; fallback used approved temp path. |
| Task file and core project-control records reviewed | PASS | Task, registry, and activity log updated. |
| No secret leakage in task files/logs/reports/handbacks | PASS | None found. |
| No duplicate project-control IDs created | PASS | New ID: TASK-COD-FIX-021. |
| Any out-of-target changes classified | N/A | None. |
| Independent review decision recorded | PASS | See below. |
| Auditor review decision recorded | PASS | NOT_REQUIRED for one-line surgical fix; original Auditor finding already identified the exact issue. |

**Gate Status:** PASS

**Independent Review:**
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required — preserves antiforgery validation; no auth/session/role changes
- QAAndAcceptanceAgent: Recommended only if Majed wants automated/browser verification; manual browser test is sufficient for this surgical task
- Auditor: NOT_REQUIRED — one-line fix for Auditor finding F-001 with direct evidence

**Final Task Decision:** Accepted as code fix. Browser save verification remains the next operational step.
