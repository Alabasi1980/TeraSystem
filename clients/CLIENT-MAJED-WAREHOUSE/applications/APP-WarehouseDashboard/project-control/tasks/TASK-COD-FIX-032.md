# TASK-COD-FIX-032

## Objective
Fix the sync mode (Full/Incremental) feature end-to-end: the wizard UI collects `SyncMode` and `IncrementalColumn` but these values are never saved to the database, and even if they were, `SqlServerLoadService` always does DELETE+INSERT (full refresh). This fix makes the sync type selection actually functional.

## Status: ACCEPTED

## ClientAppPath
`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## Problem Analysis

### Gap 1: C# Binding (Web Project)
- **File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml.cs`
- The hidden form sends `SyncMode` and `IncrementalColumn` fields, but the PageModel has NO `[BindProperty]` for them.
- `OnPostAddAsync` creates a `TableMappingConfig` without setting `SyncMode` or `IncrementalColumn`.
- `OnPostEditAsync` updates the mapping without setting `SyncMode` or `IncrementalColumn`.
- Result: `SyncMode` always stays at its DB default `"Full"`.

### Gap 2: Load Strategy (API Project)
- **File:** `src/WarehouseDashboard.Api/Services/SqlServerLoadService.cs`
- `LoadTableAsync` always does `DELETE FROM [table]` + `SqlBulkCopy.WriteToServer` (full refresh).
- For incremental sync, it should INSERT only (without deleting existing rows).

### Gap 3: SyncEngine Pass-through
- **File:** `src/WarehouseDashboard.Api/Services/SyncEngineService.cs`
- `BuildOracleQuery` already handles the Oracle WHERE clause for incremental mode correctly.
- But `LoadTableAsync` is called without knowing the sync mode — it always does full refresh.

---

## Required Changes

### Fix 1: TableMappings/Index.cshtml.cs (Web Project)
1. Add `[BindProperty] public string SyncMode { get; set; } = "Full";`
2. Add `[BindProperty] public string? IncrementalColumn { get; set; }`
3. In `OnPostAddAsync`, set `SyncMode = SyncMode` and `IncrementalColumn = IncrementalColumn` on the new mapping.
4. In `OnPostEditAsync`, set `mapping.SyncMode = SyncMode` and `mapping.IncrementalColumn = IncrementalColumn`.

### Fix 2: SqlServerLoadService.cs (API Project)
1. Add a new method `LoadTableIncrementalAsync(string targetTable, DataTable data, CancellationToken ct)` that does INSERT only (no DELETE).
2. OR modify `LoadTableAsync` to accept a `bool isIncremental` parameter.
3. The incremental method should still do `EnsureTableExistsAsync` and then just `SqlBulkCopy.WriteToServer` without the DELETE step.

### Fix 3: SyncEngineService.cs (API Project)
1. In `RunSyncOnceAsync` and `RunSelectedMappingsAsync`, pass the `mapping.SyncMode` to decide which load method to call.
2. If `mapping.SyncMode == "Incremental"`, call the incremental load method.
3. If `mapping.SyncMode == "Full"` (default), call the existing full-refresh load method.

---

## Allowed Write Targets
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\TableMappings\Index.cshtml.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SqlServerLoadService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs`

## Forbidden
- Do NOT modify the wizard JS or HTML (Step 5 UI already works correctly).
- Do NOT create new files, services, or abstractions.
- Do NOT modify the `TableMapping` model (properties already exist).
- Do NOT add EF Core migrations.
- Do NOT modify any other files.

---

## Acceptance Criteria
1. `SyncMode` and `IncrementalColumn` are saved to the database when creating/editing a mapping via the wizard.
2. When `SyncMode == "Full"`, behavior is identical to current (DELETE + INSERT).
3. When `SyncMode == "Incremental"`, only new rows are INSERTed (existing rows are preserved).
4. `BuildOracleQuery` WHERE clause for incremental already works — no changes needed there.
5. Both projects build cleanly (0 errors).

## Verification
- `dotnet build` for both Web and API projects — must produce 0 errors.
- Manual test: create a mapping with SyncMode=Full → verify it does DELETE+INSERT.
- Manual test: create a mapping with SyncMode=Incremental → verify it does INSERT only.

## Vitality & Polish Checklist
N/A — This is a backend logic fix, not a UI task.

---

## Handback

Status: DONE
Task ID: TASK-COD-FIX-032
Files changed:
1. `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml.cs`
2. `src/WarehouseDashboard.Api/Services/SqlServerLoadService.cs`
3. `src/WarehouseDashboard.Api/Services/SyncEngineService.cs`

Behavior implemented:
- Fix 1: Added `[BindProperty]` for `SyncMode` (default "Full") and `IncrementalColumn` (nullable) to the PageModel. Both properties are now saved in `OnPostAddAsync` and `OnPostEditAsync`.
- Fix 2: Added `LoadTableIncrementalAsync` to `SqlServerLoadService` — performs INSERT only (no DELETE), preserving existing rows.
- Fix 3: Updated `RunSyncOnceAsync` and `RunSelectedMappingsAsync` in `SyncEngineService` to check `mapping.SyncMode` and call the appropriate load method.

Verification performed and result:
- `dotnet build WarehouseDashboard.Web.csproj`: 0 Warning(s), 0 Error(s) ✅
- `dotnet build WarehouseDashboard.Api.csproj`: 0 Warning(s), 0 Error(s) ✅ (after fixing SqlBulkCopy constructor)

Verification not performed and reason:
- Manual test (Full vs Incremental sync behavior) requires running app with Oracle + SQL Server.

Risks / assumptions / follow-ups:
- `LoadTableIncrementalAsync` has no dedup logic. If watermark drift causes re-fetch of already-synced rows, duplicates may occur. The `BuildOracleQuery` WHERE clause mitigates this under normal operation.
- Incremental insert has no transaction wrapping (single SqlBulkCopy call). Acceptable trade-off for append-only.

STOP/ASK decisions, if any: None.
