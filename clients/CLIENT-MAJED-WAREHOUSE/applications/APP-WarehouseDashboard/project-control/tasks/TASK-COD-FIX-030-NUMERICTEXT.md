# TASK-COD-FIX-030-NUMERICTEXT

Status: DONE

## Implemented

- Added `ColumnMapping.IsNumericText` metadata with EF migration `AddColumnMappingNumericText`.
- Persisted `isNumericText` through the Table Mapping column editor JSON and edit loading.
- Added a minimal “Numeric text” checkbox; Oracle `NUMBER` columns switched to NVARCHAR/VARCHAR auto-mark as NumericText.
- Schema comparison/DDL now resolves NumericText overrides to `NVARCHAR(MAX)` or configured text length and can alter existing decimal/numeric columns.
- API sync loads NumericText columns per mapping and passes them to Oracle extraction.
- Oracle extraction creates string `DataTable` columns for NumericText and reads Oracle native values via `GetOracleValue()` / `OracleDecimal.ToString()` to avoid decimal overflow data loss.
- Card Builder active mappings expose `numericTextColumns`; generated KPI SQL for mapped SQL tables wraps NumericText value columns with `TRY_CAST([Column] AS DECIMAL(28,6))`.
- Advanced KPI generated SUM queries now use `TRY_CAST` for value columns.

## Verification

- `dotnet build -c Release` in `src/WarehouseDashboard.Web` passed.
- `dotnet build -c Release` in `src/WarehouseDashboard.Api` passed.
- Debug builds were attempted but blocked by running local processes locking Debug outputs (`WarehouseDashboard.Web` PID 14500, `WarehouseDashboard.Api` PID 6424). Release builds were used for verification.

## Database migration

- Migration file: `20260718120000_AddColumnMappingNumericText.cs`
- Database update was not executed.
- Apply manually if startup does not auto-migrate:

```powershell
dotnet ef database update --project "src\WarehouseDashboard.Web\WarehouseDashboard.Web.csproj"
```

## Retest stg_st_invoice_dtl

1. Apply migration if not auto-applied.
2. Edit mapping Id=6.
3. Mark `PRICE`, `FIRST_PRICE`, `PRICE_A_DISC`, `PRICE_A_TAX` as SQL `NVARCHAR(MAX)` and `Numeric text`.
4. Apply schema for the mapping so target columns become NVARCHAR.
5. Run selected sync for mapping Id=6.
6. Verify the four columns contain non-null string values where Oracle has oversized NUMBER values.
