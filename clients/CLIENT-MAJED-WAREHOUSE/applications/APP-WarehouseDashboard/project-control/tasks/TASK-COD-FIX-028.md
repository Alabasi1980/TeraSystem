# TASK-COD-FIX-028

Status: DONE

## Summary
- Loaded persisted `ColumnMappings` with table mappings and passed them into the edit wizard.
- Updated edit-mode wizard behavior to prefer stored column mapping values and merge only source-preview metadata/missing columns.
- Updated schema comparison to use effective SQL target column names (`SqlColumnName` when set, otherwise Oracle column name), with trim/case normalization.
- Added generated `ADD COLUMN` de-duplication in schema DDL generation.

## Verification
- `dotnet build WarehouseDashboard.Web.csproj` failed because the running app locked `bin\Debug\net8.0\WarehouseDashboard.Web.exe`.
- `dotnet build WarehouseDashboard.Web.csproj /p:UseAppHost=false /p:OutputPath="C:\Users\Fares\AppData\Local\Temp\opencode\wh-build\"` succeeded with 0 warnings and 0 errors.

## Notes
- `BRANCH_ID` duplicate ADD is prevented by comparing SQL Server columns against the effective SQL target name and by a final per-run ADD-column de-dupe guard.
