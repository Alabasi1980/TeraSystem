# TASK-COD-005 — SQL Server Load Service (LOAD half) + TableMapping + Sync Orchestration

**Status:** ✅ Done (pending Tera acceptance)
**Owner:** EngineeringAgent
**Date:** 2026-07-13

## Objective
Implement the LOAD half of the Sync Engine: take an Oracle-extracted `DataTable`
(from `OracleExtractionService`, TASK-COD-004) and bulk-copy it into a SQL Server
target table with per-table transactional safety (DELETE + INSERT in one transaction,
rollback on failure). Add the `TableMapping` model and wire the orchestration loop
into `SyncEngineService`.

## Handback

### Summary of implementation
Implemented the SQL Server LOAD service, the `TableMapping` model, and the sync
orchestration per `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §2–§4 and
`06_DATA_MODEL_PREPARATION.md` §3.3. Code compiles (`dotnet build` → **0 warnings, 0 errors**).

**Files created/changed (all under `src/WarehouseDashboard.Api/`):**

1. **`Models/TableMapping.cs`** (new) — the mapping model:
   - `OracleSource` (table name / view name / SQL query)
   - `SourceType` (`"Table"` | `"View"` | `"Query"`)
   - `SqlTargetTable` (e.g. `"stg_WarehouseStock"`)

2. **`Services/SqlServerLoadService.cs`** (replaced placeholder) — the LOAD half:
   - Constructor takes `IConfiguration` + `ILogger<SqlServerLoadService>`; resolves the
     SQL connection string at construction via `ConnectionStringHelper.ResolveSql(configuration)`
     (fail-fast if empty; password comes only from the `SQL_PASSWORD` env var).
   - `Task LoadTableAsync(string targetTable, DataTable data, CancellationToken = default)`:
     - **Create-if-not-exists:** if `targetTable` is absent from `INFORMATION_SCHEMA.TABLES`,
       CREATE it from the `DataTable` schema, mapping CLR types → SQL types:
       `int/long/short/byte`→`BIGINT`; `decimal`→`DECIMAL(18,2)`; `string`→`NVARCHAR(MAX)`;
       `DateTime/DateTimeOffset`→`DATETIME2`; `byte[]`→`VARBINARY(MAX)`; `double/float`→`FLOAT`;
       `bool`→`BIT`; `Guid`→`UNIQUEIDENTIFIER`; fallback `NVARCHAR(MAX)` (Unicode-safe) + warning.
     - **Transactional full-refresh:** `BEGIN TRANSACTION` → `DELETE FROM [target]` →
       `SqlBulkCopy.WriteToServer(data)` (column names matched by name) → `COMMIT`.
       On any exception → `ROLLBACK` (target left unchanged) and rethrow.
     - `SqlBulkCopyOptions.Default`, `BatchSize = 1000`, `Microsoft.Data.SqlClient`.
   - **SQL-injection guard:** table and column identifiers are validated (`[A-Za-z0-9_]`;
     optional `schema.table`) before being embedded in the dynamic DDL/DML — no string
     concatenation of untrusted input.

3. **`Services/SyncEngineService.cs`** (replaced placeholder `BackgroundService`) — orchestration:
   - Private `List<TableMapping> _mappings` initialized **EMPTY** (one commented example included).
   - Public `Task RunSyncOnceAsync(CancellationToken ct)`: for each mapping, builds the Oracle
     query (verbatim for `Query`; `SELECT * FROM <source>` for `Table`/`View`, source validated),
     calls `OracleExtractionService.ExtractAsync(...)` then `SqlServerLoadService.LoadTableAsync(...)`.
     Each table is wrapped in try/catch — one failing table is logged and **does not stop the others**.
     Simple retry: max `MaxRetries = 3` attempts with a short `RetryDelay` (5 s) between attempts;
     `OperationCanceledException` (shutdown) is propagated immediately.
   - `ExecuteAsync` loop: `PeriodicTimer` (30 min) + `SemaphoreSlim(1,1)` to prevent overlapping
     cycles (a tick during a running cycle is skipped, not queued). `StopAsync` logs and delegates
     to `base.StopAsync` (which cancels `stoppingToken` → clean shutdown).
   - Injected: `OracleExtractionService`, `SqlServerLoadService`, `ILogger<SyncEngineService>`.

4. **`Infrastructure/ConnectionStringHelper.cs`** — added:
   ```csharp
   public static string ResolveSql(IConfiguration configuration)
   ```
   Reads `ConnectionStrings:SqlServer` and substitutes `{SQL_PASSWORD}` from the `SQL_PASSWORD`
   environment variable (reuses existing `Resolve`).

5. **`Program.cs`** — registration updated:
   ```csharp
   builder.Services.AddSingleton<OracleExtractionService>();   // unchanged
   builder.Services.AddSingleton<SqlServerLoadService>();      // was: new SqlServerLoadService(resolvedString)
   builder.Services.AddHostedService<SyncEngineService>();     // unchanged
   ```
   `SqlServerLoadService` now resolves its connection string itself (from `IConfiguration` +
   env var), so the manual pre-resolution in `Program.cs` was removed. Existing registrations
   and the env-var warning block are preserved.

### Build result
`dotnet build` (WarehouseDashboard.Api, net8.0): **Build succeeded. 0 Warning(s), 0 Error(s).**
No real SQL Server / Oracle connection attempted (client network only) — logic verified by
compilation and code review.

### Dynamic-create deviation (documented)
The data-model document (rule **G5**, §5.3) states Data Tables are created **manually** by the
client/DBA, not by the application. This task explicitly requires create-if-not-exists as a
**pragmatic Phase-1 deviation** until the client provides the exact Oracle schemas.
`SqlServerLoadService` therefore auto-CREATES the target table on first load when missing,
deriving the schema from the `DataTable` CLR column types. The branch is isolated in
`EnsureTableExistsAsync` and clearly commented; once the client supplies fixed schemas it can be
removed and tables created explicitly (restoring strict G5 compliance). Where a table already
exists (manually created by the DBA), the service skips CREATE and proceeds with DELETE + bulk
copy, matching by column name.

### DB logging deferred
Full structured logging to the **SyncLogs / ErrorLogs** tables (EF Core, `WarehouseDashboardLogContext`)
is **explicitly deferred to a later task**. Per TASK-COD-005, this implementation logs start/end and
per-table success/failure/retry via `ILogger` only. The per-table isolation and retry structure are
already in place so DB logging can be layered in without changing the control flow.

### Security (verified)
- No `SQL_PASSWORD` or any secret is written to any file. The password is read only at runtime from
  the `SQL_PASSWORD` environment variable via `ConnectionStringHelper`.
- Dynamic DDL/DML identifiers (table and column names) are validated against a safe character set
  before use — no SQL injection via mapping configuration.
- Oracle read-only enforcement (TASK-COD-004) remains intact; the LOAD service never queries Oracle.

### Not done (out of scope)
- Actual `TableMapping` entries (left EMPTY — configured later by the client/admin).
- SyncLogs/ErrorLogs DB persistence (deferred).
- Manual-trigger / status REST endpoints (documented in §5 of the integration doc — later task).

### Example mapping (for downstream reference)
```csharp
// Inside SyncEngineService constructor, once Oracle schemas are known:
_mappings.Add(new TableMapping
{
    OracleSource   = "WMS.WAREHOUSE_STOCK",
    SourceType     = "Table",
    SqlTargetTable = "stg_WarehouseStock"
});
// RunSyncOnceAsync: SELECT * FROM WMS.WAREHOUSE_STOCK  ->  bulk copy into stg_WarehouseStock
```

---

## Tera Review & Decision
- [x] Allowed Write Targets respected (src/Api + task file only)
- [x] No secrets in outputs (SQL_PASSWORD via env var only)
- [x] In scope (LOAD service + TableMapping + orchestration)
- [x] Acceptance criteria met (build PASS, per-table rollback, retry, overlap guard)
- [x] Handback recorded
- Note: dynamic-create deviation from G5 documented (pragmatic Phase 1); SyncLogs/ErrorLogs DB logging deferred to later task (ILogger only now).

| Item | Value |
|---|---|
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
