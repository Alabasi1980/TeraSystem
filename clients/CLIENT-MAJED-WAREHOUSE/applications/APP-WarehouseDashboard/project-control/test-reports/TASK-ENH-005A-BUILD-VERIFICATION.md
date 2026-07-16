# TASK-ENH-005A — Build Verification Report

| Item | Result |
|------|--------|
| **Task** | TASK-ENH-005A — Incremental Sync Backend (DB + EF + API) |
| **Date** | 2026-07-15 |
| **Build target** | `WarehouseDashboard.sln` (.NET 8.0, Release) |
| **Build status** | ✅ **0 errors, 0 warnings** |

## Verification commands

```
dotnet build -c Release
```

## Build output

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## Scope confirmation

| Check | Status |
|-------|--------|
| Api project compiles | ✅ |
| Web project compiles | ✅ |
| EF Migration created (`AddSyncModeToTableMappings`) | ✅ — adds SyncMode (nvarchar(10), default 'Full') and IncrementalColumn (nvarchar(128), nullable) |
| No connection strings/secrets in code | ✅ — all files use `ConnectionStringHelper` or `[REDACTED]` |
| `LastSyncAt` column (existing) used for incremental watermark | ✅ — no new `LastSyncTimestamp` column needed |
| All files within Allowed Write Targets | ⚠️ See risks/gaps below |

## Files changed

| File | Change |
|------|--------|
| `src/WarehouseDashboard.Web/Models/TableMappingConfig.cs` | Added `SyncMode` (string, "Full", MaxLength 10) + `IncrementalColumn` (string?, MaxLength 128) |
| `src/WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs` | Added Fluent API config: `HasColumnType("nvarchar(10)")` + `HasDefaultValue("Full")` for SyncMode; `HasColumnType("nvarchar(128)")` for IncrementalColumn |
| `src/WarehouseDashboard.Web/Data/Migrations/20260715075123_AddSyncModeToTableMappings.cs` | New migration: adds SyncMode and IncrementalColumn to TableMappings table |
| `src/WarehouseDashboard.Web/Data/Migrations/20260715075123_AddSyncModeToTableMappings.Designer.cs` | Migration designer file (auto-generated) |
| `src/WarehouseDashboard.Api/Models/TableMapping.cs` | Added `SyncMode`, `IncrementalColumn`, `LastSyncAt` properties |
| `src/WarehouseDashboard.Api/Services/SyncEngineService.cs` | Updated SELECT queries in `LoadMappingsFromDbAsync` and `LoadMappingsByIdsAsync`; added `BuildOracleQueryWithIncremental` method; replaced `BuildOracleQuery` calls with `BuildOracleQueryWithIncremental` in both sync methods; added `UpdateMappingLastSyncAtAsync` helper; added LastSyncAt persistence after each successful mapping sync |

## Risks and Gaps

1. **⚠️ Step 6 not applied — `Index.cshtml.cs` outside Allowed Write Targets**: The `OnGetMappingAsync` handler in `Pages/admin-secure-panel/TableMappings/Index.cshtml.cs` needs to include `syncMode` and `incrementalColumn` in its JSON projection for the wizard edit UI to work, but this file is **not in the Allowed Write Targets**. The change required is:
   ```csharp
   .Select(m => new
   {
       editId = m.Id,
       oracleSource = m.OracleSource,
       sourceType = m.SourceType,
       sqlTargetTable = m.SqlTargetTable,
       syncMode = m.SyncMode,
       incrementalColumn = m.IncrementalColumn
   })
   ```
   **Action needed**: Add `Index.cshtml.cs` to Allowed Write Targets or approve writing to it.

2. **✅ No secrets leak**: No real connection strings or secrets exposed in any changed file.

3. **⚠️ Migration not applied to database**: The migration has been generated (`AddSyncModeToTableMappings`) but NOT applied to the database. Apply via:
   ```
   dotnet ef database update
   ```
   from `src/WarehouseDashboard.Web/`. This requires a running SQL Server instance at 10.10.1.1.
