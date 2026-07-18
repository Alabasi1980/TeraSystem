# TASK-COD-FIX-029-DIAG — Diagnostic Logging in CompareSchemasAsync

## Objective
Add targeted diagnostic logging inside `OracleSchemaService.CompareSchemasAsync` to reveal exactly why `BRANCH_ID` (or other columns) end up in `ColumnsToAdd` when they already exist in the SQL Server target table.

## Changes Made

### File: `src/WarehouseDashboard.Web/Services/OracleSchemaService.cs`

#### Section A — Entry State (after line 232)
Logs the following at method entry (after columns and overrides are loaded):
- Oracle column count + first 15 column names
- SQL Server column count + first 15 column names
- Override count

#### Section B — Per-Column Diagnostic (inside main foreach loop, first 20 columns)
For each of the first 20 Oracle columns, logs:
- Oracle column name and data type
- Whether an override was found (by OracleColumnName)
- If override found: `SqlColumnName`, `SqlDataType`, `SqlMaxLength`
- `EffectiveSqlName` (result of `GetEffectiveSqlColumnName`)
- `EffectiveSqlKey` (result of `NormalizeColumnName`)
- Whether `EffectiveSqlKey` was **FOUND** in `sqlColumnDict` (true/false)
- If found: the matched SQL column name and type
- The action taken:
  - `SKIP (excluded)` — column excluded via override
  - `SKIP (exists)` — column found in SQL, types match
  - `MODIFY` — column found in SQL, types differ
  - `ADD` — column not found in `sqlColumnDict`

#### Section C — Final Diff Lists (after loop)
Logs the complete final lists:
- `ColumnsToAdd = [names]`
- `ColumnsToModify = [names]`
- `ColumnsToRemove = [names]`

#### Section D — Remove Section Logging (lines 397–417)
For each SQL column not found in the oracle column set:
- Logs the SQL column name
- Indicates whether it **will be removed** or is **excluded via override** (NOT removing)

## How to Use

1. **Run the application** in a console/PowerShell window.
2. **Navigate to the mapping page**, press **تطبيق (Apply Schema)**.
3. **Copy all log lines** containing `[WM-DIAG SCHEMA]` from the console output.
4. **Send those lines** to the developer for analysis.

The diagnostic tag to filter: **`[WM-DIAG SCHEMA]`**

## Files Changed
- `src/WarehouseDashboard.Web/Services/OracleSchemaService.cs`

## Build Result
- **Build:** Succeeded (0 warnings, 0 errors)
- **Tool:** `dotnet build WarehouseDashboard.Web.csproj /p:UseAppHost=false`
- **Note:** Running instance (PID 22608) had to be stopped first because it locked the output DLL.
