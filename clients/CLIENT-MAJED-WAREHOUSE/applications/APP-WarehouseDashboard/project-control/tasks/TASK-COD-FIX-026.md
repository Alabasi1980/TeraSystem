# TASK-COD-FIX-026 Handback

## Status
DONE

## Files changed
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml.cs`
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml`
- `src/WarehouseDashboard.Web/wwwroot/js/table-mapping-wizard.js`

## Root cause confirmed
- `SaveColumnMappingsAsync` deserialized wizard JSON with default `System.Text.Json` options. The browser sends camelCase keys (`sqlDataType`, `sqlMaxLength`), while the DTO used PascalCase properties, so values could fail to bind and fall back to `NVARCHAR(MAX)`.
- The Razor page referenced `table-mapping-wizard.js` without cache busting, so old browser-cached JavaScript could keep generating/displaying stale mappings.
- `renderColumnMetadata` displayed suggestions from `.NET` `columnTypes` instead of Oracle-native `oracleColumnTypes`, so Table/View metadata could visually show `NVARCHAR(MAX)` even when the mapping editor had the Oracle length.

## Fix summary
- Added case-insensitive JSON deserialization plus explicit `JsonPropertyName` mappings for `ColumnMappingDto`.
- Added `asp-append-version="true"` to the wizard script reference.
- Updated metadata display to prefer `result.oracleColumnTypes` and format the suggested SQL type using the same Oracle parsing/suggestion path used by the column mapping editor.

## Verification performed
- `dotnet build "WarehouseDashboard.Web.csproj"`
- Result: PASS, 0 warnings, 0 errors.

## Manual browser retest needed
1. Hard refresh the admin TableMappings page.
2. Add or edit a Table/View mapping for an Oracle source with `VARCHAR2(255)`.
3. Confirm the columns tab displays Oracle type `VARCHAR2(255)` and suggested SQL type `NVARCHAR(255)`.
4. Confirm the mapping editor JSON before submit contains `sqlDataType: "NVARCHAR"` and `sqlMaxLength: 255`.
5. Save and verify the stored `ColumnMappings` row has `SqlDataType = NVARCHAR` and `SqlMaxLength = 255`.
6. Preview/apply schema and confirm generated DDL resolves to `NVARCHAR(255)`.

## Gaps / risks
- Query source mode still relies on provider/.NET metadata and keeps the existing safe fallback behavior where native Oracle lengths are unavailable.
