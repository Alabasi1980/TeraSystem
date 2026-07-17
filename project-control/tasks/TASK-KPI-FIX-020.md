# TASK-KPI-FIX-020 — Repair DashboardCards Schema + Visible Save Errors

## Task Info
| Field | Value |
|---|---|
| **Task ID** | TASK-KPI-FIX-020 |
| **Status** | Accepted — Closed |
| **Priority** | P0 — Blocking |
| **Type** | Database Migration / Save Failure Fix |
| **Requested By** | Majed |
| **Created** | 2026-07-17 |

## Problem
The user completed all five wizard steps and clicked save, but no row was inserted into `DashboardCards`.

Current evidence strongly indicates `SaveChangesAsync()` fails because the EF `DashboardCard` entity contains fields that are not guaranteed to exist in the actual SQL Server `DashboardCards` table.

Known high-confidence example:
- `DashboardCard.ColorPalette` exists in the model.
- `WarehouseDashboardDbContextModelSnapshot` does not include `ColorPalette`.
- Existing migration list contains only `InitialCreate`.
- Therefore the database likely does not contain `DashboardCards.ColorPalette`, causing insert failure.

Also, `InitialCreate.cs` does not include the advanced KPI columns, while the current entity/DbContext includes them. The database schema must be verified and repaired to match the current model.

## Decision
This is an explicit database-apply task. `dotnet ef database update` is approved for this task only, because saving cards is blocked.

## Scope
Repair the SQL Server `DashboardCards` schema to support the current `DashboardCard` entity and make server-side save errors visible in the Builder UI.

## Allowed Write Targets
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Migrations\`

## Required Work

### 1. DbContext configuration
Add explicit configuration for `ColorPalette` in `WarehouseDashboardDbContext.cs` under `DashboardCard` mapping:

```csharp
entity.Property(e => e.ColorPalette)
    .HasMaxLength(50)
    .HasDefaultValue("primary");
```

### 2. Create/repair migration
Create a migration that safely ensures these `DashboardCards` columns exist in the actual database:

- `ColorPalette` nvarchar(50) NOT NULL DEFAULT 'primary'
- `ValueColumn` nvarchar(100) NOT NULL DEFAULT ''
- `DateColumn` nvarchar(100) NOT NULL DEFAULT ''
- `CategoryColumn` nvarchar(100) NOT NULL DEFAULT ''
- `KpiMode` nvarchar(50) NOT NULL DEFAULT 'simple'
- `ShowChange` bit NOT NULL DEFAULT 0
- `ChangeSource` nvarchar(50) NOT NULL DEFAULT 'previousPeriod'
- `ShowSparkline` bit NOT NULL DEFAULT 0
- `SparklineMonths` int NOT NULL DEFAULT 6
- `ShowGrandTotal` bit NOT NULL DEFAULT 0
- `GrandTotalSource` nvarchar(50) NOT NULL DEFAULT 'sameTable'
- `DateFilterMode` nvarchar(50) NOT NULL DEFAULT 'dashboard'
- `FixedStartDate` nvarchar(50) NOT NULL DEFAULT ''
- `FixedEndDate` nvarchar(50) NOT NULL DEFAULT ''
- `RelativeDays` int NOT NULL DEFAULT 30

Important: Because migration snapshot/history may be inconsistent, verify the actual database schema first. If needed, use a migration with guarded SQL such as `IF COL_LENGTH('DashboardCards','ColumnName') IS NULL ALTER TABLE ...` to avoid duplicate-column errors.

### 3. Apply migration
Run:

```powershell
dotnet ef database update
```

from:

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web
```

Do not print real connection strings or passwords in the handback.

### 4. Visible save error
Add a model-level validation summary near the top of `Builder.cshtml` form so server-side save errors are visible if `SaveChangesAsync()` fails again.

Recommended:

```html
<div asp-validation-summary="ModelOnly" class="wd-error"></div>
```

Place it immediately after `<form ...>` and before Step 1 fieldset.

## Verification
1. `dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\KPI-FIX-020-check`
2. `dotnet ef database update`
3. Verify actual SQL Server columns exist after update.
4. If possible, insert/save one test card through the app or verify `DashboardCards` insert path is no longer blocked by schema errors.

## Acceptance Criteria
- [ ] `WarehouseDashboardDbContext.cs` explicitly configures `ColorPalette`.
- [ ] A new migration exists under `Migrations/`.
- [ ] The migration covers `ColorPalette` and any missing current `DashboardCard` columns.
- [ ] `dotnet ef database update` succeeds.
- [ ] Actual `DashboardCards` table contains `ColorPalette` and all advanced KPI columns listed above.
- [ ] Builder page shows server-side model errors instead of failing silently.
- [ ] Build succeeds with 0 errors.

## Mandatory Governance
- Read current files from disk before editing.
- Preserve unrelated changes.
- Do not print secrets, passwords, or full connection strings.
- This task may change the database schema only for `DashboardCards` columns listed above.

## Engineering Handback
- **Files changed:**
  - `WarehouseDashboardDbContext.cs`
  - `Builder.cshtml`
  - `Migrations/20260717134445_RepairDashboardCardsSchemaForBuilderSave.cs`
  - `Migrations/20260717134445_RepairDashboardCardsSchemaForBuilderSave.Designer.cs`
  - `Migrations/WarehouseDashboardDbContextModelSnapshot.cs`
- **Migration:** `20260717134445_RepairDashboardCardsSchemaForBuilderSave`
- **Migration strategy:** guarded SQL using `IF COL_LENGTH(...) IS NULL` for required `DashboardCards` builder/KPI columns.
- **Database update:** `dotnet ef database update` succeeded and applied the migration.
- **Schema verification:** actual SQL Server `DashboardCards` confirmed to contain `ColorPalette` and all required advanced KPI columns listed in this task.
- **Build:** `dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\KPI-FIX-020-check` succeeded with 0 warnings and 0 errors.
- **Test data:** No test card data was created by EngineeringAgent.

## Tera Review Notes
- Initial code review confirms guarded migration exists and covers all required columns.
- `WarehouseDashboardDbContext.cs` now configures `ColorPalette` with max length 50 and default `primary`.
- `Builder.cshtml` now includes a model-level validation summary to surface server-side save errors.
- Final Auditor verification: PASS — no STOP or CAUTION findings. One non-blocking FLAG: no actual test card insert was performed by EngineeringAgent.
- Status: Accepted — Closed.
