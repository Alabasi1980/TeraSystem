# TASK-COD-004 — Oracle Extraction Service (Generic, Reusable)

**Status:** ✅ Done
**Owner:** EngineeringAgent
**Date:** 2026-07-13

## Objective
Implement `OracleExtractionService` that connects to Oracle (READ-ONLY) and returns a
`DataTable` for a given SQL query — the EXTRACTION half of the Sync Engine.

## Handback

### Summary of implementation
Implemented the generic, reusable Oracle extraction service per the integration
document (`14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §1, §2, §3). Code compiles
(`dotnet build` → **0 warnings, 0 errors**).

**Files changed (all under `src/WarehouseDashboard.Api/`):**

1. **`Services/OracleExtractionService.cs`** — replaced placeholder with real implementation:
   - Constructor takes `IConfiguration`; resolves the connection string at construction
     via `ConnectionStringHelper.ResolveOracle(configuration)` (fails fast if empty).
   - `Task<DataTable> ExtractAsync(string oracleSql, CancellationToken = default)`:
     - Opens `OracleConnection` (ODP.NET Managed) → `OracleCommand` (`CommandType.Text`)
       → `OracleDataReader` → `DataTable.Load(reader)`.
     - Column names preserved exactly as returned by Oracle; types come from ODP.NET
       (e.g. `NUMBER(p,0)`→`long/int`, `NUMBER(p,s)`→`decimal`, `DATE/TIMESTAMP`→`DateTime`,
       `CLOB`→`string`, `BLOB`→`byte[]`, `VARCHAR2/NVARCHAR2`→`string`).
     - READ-ONLY guard rejects anything whose first statement is not `SELECT`/`WITH`
       (leading comments tolerated); INSERT/UPDATE/DELETE/MERGE/DDL rejected by design.
     - `using` disposal of connection/command/reader; `OracleException` caught and
       wrapped with ORA code + query context (never swallowed). Other exceptions propagate.
   - Stateless — opens a fresh connection per call, so safe as a singleton.

2. **`Infrastructure/ConnectionStringHelper.cs`** — added:
   ```csharp
   public static string ResolveOracle(IConfiguration configuration)
   ```
   Reads `ConnectionStrings:Oracle` from config and substitutes `{ORACLE_PASSWORD}`
   from the `ORACLE_PASSWORD` environment variable (reuses existing `Resolve`).

3. **`Program.cs`** — registration changed to:
   ```csharp
   builder.Services.AddSingleton<OracleExtractionService>();
   ```
   Chosen **Singleton** because the service is stateless (new connection per call) and
   is consumed by the Sync Engine background service. Documented inline.

### Build result
`dotnet build` (WarehouseDashboard.Api, net8.0): **Build succeeded. 0 Warning(s), 0 Error(s).**
No real Oracle connection attempted (client network only) — logic verified by compilation
and code review.

### DataTable contract (consumer = `SqlServerLoadService`, TASK-COD-005)
- **Column names:** preserved exactly as Oracle returns them (no renaming here; any
  Oracle→SQL Server naming unification happens downstream during load/mapping).
- **Column types:** native .NET types from ODP.NET `OracleDataReader` schema (see §3.1
  of the integration doc). `SqlBulkCopy` will read these directly.
- **Row set:** all rows from the query are loaded into the table.
- **Lifecycle:** caller owns the returned `DataTable`; the service disposes the
  Oracle reader/command/connection before returning.

### Security (verified)
- No Oracle password or secret written to any file. `ORACLE_PASSWORD` is read only at
  runtime from the environment variable via `ConnectionStringHelper`.
- Read-only enforcement is defense-in-depth on top of the Oracle `READ ONLY` account
  privilege recommended in the integration doc.

### Not done (out of scope)
- `SyncEngineService.cs` and `SqlServerLoadService.cs` remain placeholders — load logic
  is **TASK-COD-005**.

### Example usage (for downstream reference)
```csharp
// Injected singleton:
var table = await _oracleExtraction.ExtractAsync(
    "SELECT WAREHOUSE_ID, WAREHOUSE_NAME, LAST_UPDATE FROM WAREHOUSE_STOCK",
    stoppingToken);
// table.Columns -> ["WAREHOUSE_ID", "WAREHOUSE_NAME", "LAST_UPDATE"]
// table.Rows -> populated; types from ODP.NET.
```

---

## Tera Review & Decision
- [x] Allowed Write Targets respected (src/Api + task file)
- [x] No secrets in outputs (ORACLE_PASSWORD via env var only)
- [x] In scope (generic extraction service)
- [x] Acceptance criteria met (build PASS, read-only guard, DataTable contract defined)
- [x] Handback recorded

| Item | Value |
|---|---|
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
