# TASK-COD-FIX-024 — Fix SqlTable DataSourceType Mapping and Legacy Render Guard

> **Status:** Accepted — Code fix complete, browser render verification pending  
> **Created:** 2026-07-18  
> **Approved By:** Majed  
> **Owner:** TeraAgent  
> **Assigned Agent:** EngineeringAgent  
> **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## 1. Objective

Fix the saved Card Builder SqlTable mapping that causes public dashboard render failures.

Current saved row evidence:

```text
SqlQuery = SELECT * FROM [stg_st_invoice]
DataSourceType = View
```

This is invalid because `DashboardService.BuildSql()` treats `View` as a table/view name and wraps `SqlQuery` as:

```sql
SELECT * FROM [SELECT * FROM [stg_st_invoice]]
```

The correct behavior for Card Builder `SqlTable` source is:

```text
DataSourceType = SQL Query
SqlQuery = SELECT * FROM [stg_st_invoice]
```

---

## 2. Source Finding

- **Audit Report:** `project-control/audit-reports/QUAUD-CARDBUILDER-001-2026-07-18-001.md`
- **Finding:** F-008 — SqlTable → `DataSourceType = "View"` mapping is wrong.

---

## 3. Scope

### In Scope

1. Fix Card Builder save mapping so future SqlTable cards save as `DataSourceType = "SQL Query"` when `SqlQuery` is a full SQL statement.
2. Add a defensive legacy guard in `DashboardService.BuildSql()` so existing malformed rows with `DataSourceType = "View"` but `SqlQuery` beginning with `SELECT` are executed as SQL Query instead of being wrapped as a view name.
3. Keep the fix surgical and build-verified.

### Out of Scope

1. Do not run database updates, migrations, or direct SQL repair commands in this task.
2. Do not migrate Syncfusion preview rendering to Chart.js.
3. Do not fix KPI SQL injection in this task.
4. Do not redesign admin Cards list or Edit page.
5. Do not add packages.
6. Do not modify database schema.

---

## 4. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-COD-FIX-024.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\audit-reports\QUAUD-CARDBUILDER-001-2026-07-18-001.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\tera-system\profiles\dotnet-razorpages-adonet.md`

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`

---

## 6. Acceptance Criteria

1. Future Card Builder `SqlTable` saves use `DataSourceType = "SQL Query"` when `SqlQuery` contains a full query like `SELECT * FROM [table]`.
2. Existing malformed cards with `DataSourceType = "View"` and `SqlQuery` beginning with `SELECT` no longer render as invalid `SELECT * FROM [SELECT ...]`.
3. Normal `View` behavior remains valid for actual view/table names.
4. No database schema, migration, package, or direct DB update changes occur.
5. `dotnet build` succeeds, or fallback build succeeds if the running app locks normal output.

---

## 7. Security Sensitivity

- **Level:** Medium
- **Reason:** Touches SQL execution path selection. The SQL remains admin-configured, but the change must not introduce user-input SQL construction.
- **SecurityAgent:** Not required for this focused mapping correction, but Post-Execution Review must verify no new raw user-input concatenation is introduced.

---

## 8. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Focused mapping + legacy guard only. |
| One objective only | PASS | Fix wrong SqlTable mapping/render failure. |
| No deferrable work included | PASS | Preview and KPI SQL injection deferred. |
| No UI unless explicitly requested | PASS | No UI change. |
| No API unless explicitly requested | PASS | No API changes. |
| No Auth unless explicitly requested | PASS | No auth changes. |
| No schema/migration unless explicitly requested | PASS | No DB schema changes. |
| No real secrets outside approved local environment files | PASS | No secrets. |
| Secret handling plan documented and redacted | PASS | No secrets logged. |
| CLI side effects checked | PASS | Build output only. |
| No internal contradiction between constraints and outputs | PASS | Scope and write targets align. |
| Allowed Write Targets are narrow | PASS | Two existing C# files only. |
| Acceptance criteria are testable | PASS | New save and dashboard render can verify. |

**Gate Status:** PASS

---

## 9. Delegation Notes

EngineeringAgent must:

1. Read current files from disk before editing.
2. Preserve unrelated changes.
3. Keep change minimal.
4. Do not run direct database update/SQL repair commands.
5. Do not alter schema, migrations, packages, or preview rendering.
6. Explain whether existing saved malformed cards are handled by the defensive guard.

---

## 10. Handback Placeholder

## 10. Engineering Handback

- **Task ID:** TASK-COD-FIX-024
- **Agent:** EngineeringAgent
- **Status:** Done
- **Files Modified:**
  - `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs`
  - `src/WarehouseDashboard.Web/Pages/DashboardService.cs`
- **Exact Change Summary:**
  - Changed Card Builder save mapping to save builder-created cards as `DataSourceType = "SQL Query"`.
  - Added a focused legacy guard in `DashboardService.BuildSql()` so `DataSourceType = "View"` with `SqlQuery` starting with `SELECT` or `WITH` returns the stored SQL verbatim.
  - Preserved normal View behavior for actual view/table names.
- **Future SqlTable Saves:**
  - `SqlTable` cards storing `SELECT * FROM [table]` now save with `DataSourceType = "SQL Query"` instead of `"View"`.
- **Existing Malformed Cards:**
  - Existing rows like `DataSourceType = View` + `SqlQuery = SELECT * FROM [...]` are guarded at render time and no longer become `SELECT * FROM [SELECT ...]`.
- **Commands Run:**
  - `dotnet build` — blocked by running app executable lock.
  - fallback `dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\TASK-COD-FIX-024-build` — succeeded with 0 warnings and 0 errors.
- **Issues/Risks:**
  - No direct DB repair was run; existing database row may still display `View` in admin list until edited/resaved, but dashboard render is guarded.
  - Other out-of-scope findings remain: Chart.js preview migration and KPI SQL injection.

---

## 11. Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | Only the two allowed C# files changed. |
| No unauthorized files created | PASS | Only Tera control task file was created. |
| No unauthorized files deleted | PASS | None. |
| No unauthorized packages added | PASS | None. |
| No unauthorized UI/CSS/theme changes | PASS | None. |
| UI Acceptance Gate passed for UI tasks | N/A | Backend mapping/render guard. |
| No real secrets outside approved local environment files | PASS | No secrets involved. |
| Secrets redacted in docs/logs/config references | PASS | No secrets logged. |
| No unauthorized ORM models/entities/migrations | PASS | None. |
| No unapproved business validation moved to DB constraints | PASS | None. |
| No unauthorized API/Auth created | PASS | None. |
| Acceptance criteria satisfied | PASS | Future mapping fixed, legacy render guard added, fallback build passed. |
| CLI side effects reviewed | PASS | Build output only; fallback temp path used due running process lock. |
| Task file and core project-control records reviewed | PASS | Task, registry, and activity log updated. |
| No secret leakage in task files/logs/reports/handbacks | PASS | None found. |
| No duplicate project-control IDs created | PASS | New ID: TASK-COD-FIX-024. |
| Any out-of-target changes classified | N/A | None. |
| Independent review decision recorded | PASS | See below. |
| Auditor review decision recorded | PASS | NOT_REQUIRED for focused mapping correction. |

**Gate Status:** PASS

**Independent Review:**
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required — no user-input SQL construction added; legacy guard only changes handling of admin-configured SQL-like values
- QAAndAcceptanceAgent: Recommended if dashboard render still fails after restart
- Auditor: NOT_REQUIRED — focused fix for known Auditor finding F-008

**Final Task Decision:** Accepted as code fix. Browser dashboard render verification remains the next operational step.
