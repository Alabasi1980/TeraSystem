# TASK-COD-007: Sync Engine — Config-Driven Scheduling & Run-Status Tracking

## 1. Task Information

| Field | Value |
|---|---|
| **TASK-ID** | TASK-COD-007 |
| **Task Type** | Coding |
| **Phase** | Phase 1 — Sync Engine |
| **Build Mode Approved** | Yes |
| **Status** | Done |
| **Assigned To** | EngineeringAgent (مهندس) |
| **Created** | 2026-07-13 |
| **Linked Plan Item** | Sync Engine scheduling (14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md §5.2) |
| **Active Technology Profile** | `dotnet-razorpages-adonet.md` (.NET Expert) |
| **Design Source Decision** | N/A (backend logic) |
| **UI Acceptance Gate Required** | No |

## 2. Objective

Make the Sync Engine scheduling configurable via the `SyncSettings` table (SQL Server, ADO.NET —
the Api project has no Web DbContext) and expose thread-safe run-status fields for the upcoming
sync API task.

## 3. Reference Files

- `WarehouseDashboard.Api/Services/SyncEngineService.cs` (modified)
- `WarehouseDashboard.Api/Infrastructure/ConnectionStringHelper.cs` (read only)
- `WarehouseDashboard.Api/Services/OracleExtractionService.cs`, `SqlServerLoadService.cs` (read only)
- `project-preparation/06_DATA_MODEL_PREPARATION.md` §1.3 (SyncSettings columns)
- `project-preparation/14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §5.2 (scheduling)

## 4. Allowed Write Targets

- `clients/الماجد-لادارة-المستودعات/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Api/**`
- `project-control/tasks/TASK-COD-007.md`

## 5. Forbidden Files / Actions

- No changes to Web project / DbContext.
- No secrets / plaintext passwords (SQL_PASSWORD via env var through `ConnectionStringHelper`).
- No new NuGet packages added.

## 6. Acceptance Criteria

1. ✅ Scheduling read from `SyncSettings` (Id=1) via ADO.NET `SqlConnection`
   (`SELECT IntervalMinutes, IsAutoSyncEnabled, LastSyncTimestamp FROM SyncSettings WHERE Id = 1`).
2. ✅ Config reloaded on startup and after every cycle; timer recreated when interval/enable changes;
   polls every 5 min while disabled.
3. ✅ `PeriodicTimer` interval = `SyncSettings.IntervalMinutes` clamped to min 1.
4. ✅ `IsAutoSyncEnabled == false` → automatic runs skipped (manual trigger still allowed).
5. ✅ Missing/unreachable `SyncSettings` → safe default `IntervalMinutes=30`, `IsAutoSyncEnabled=false`.
6. ✅ Status fields exposed (thread-safe): `IsRunning`, `LastSyncTime`, `LastStatus`
   (`"Never"|"Running"|"Success"|"Failed"`), `LastRecordCount` (= sum of rows loaded across mappings).
7. ✅ `RunSyncOnceAsync` kept public; `SemaphoreSlim` moved inside it so manual triggers also skip
   safely when a cycle is running. Per-table try/catch + retry preserved.
8. ✅ `dotnet build` passes (0 warnings, 0 errors).

## 6.1 Execution Gates

| Gate | Result | Notes |
|---|---|---|
| Orchestration Decision Matrix | Direct | EngineeringAgent executed directly per Tera task |
| Model Capability Gate | Current model sufficient | |
| Pre-Execution Gate | PASS | Conduct gate read; task within allowed targets |

## 6.2 CLI / Tool Side Effects

| Command / Tool | Allowed? | Expected Side Effects | Approval Needed? |
|---|---|---|---|
| `dotnet build` | Yes | Compiled `WarehouseDashboard.Api` (no DB access) | No |

## 9. Execution Report / Agent Handback

```text
Task ID: TASK-COD-007
Agent: EngineeringAgent (مهندس)
Status: Done
Files Created: (none — modified existing)
Files Modified:
  - WarehouseDashboard.Api/Services/SyncEngineService.cs
Commands Run:
  - dotnet build  (Result: Build succeeded, 0 Warning(s), 0 Error(s))
Summary:
  Reworked SyncEngineService to be config-driven and to expose runtime status:
  1) Added IConfiguration injection; resolve SQL connection string at call time via
     ConnectionStringHelper.ResolveSql(config) (password from SQL_PASSWORD env var, never stored).
  2) Added LoadSyncSettingsAsync(ct): reads SyncSettings (Id=1) with ADO.NET SqlConnection.
     Returns safe defaults (IntervalMinutes=30, IsAutoSyncEnabled=false) when the row is
     missing OR the DB is unreachable OR the connection string is empty. Logs the fallback once
     (throttled via _loggedConfigFallback) to avoid log spam on every poll.
  3) ExecuteAsync now loops: if IsAutoSyncEnabled=false it waits ConfigPollInterval (5 min) and
     re-checks. If enabled, it creates a PeriodicTimer(IntervalMinutes) and, after each cycle,
     reloads config; if interval/enable changed it breaks out to recreate the timer. Interval is
     clamped to a minimum of 1 minute.
  4) Moved SemaphoreSlim acquisition INTO RunSyncOnceAsync so an overlapping manual trigger is
     skipped (returns immediately) — keeps auto + manual mutually exclusive.
  5) Added thread-safe status fields via a single _statusLock:
       - IsRunning            (bool, true between start and finish of a cycle)
       - LastSyncTime         (DateTime? UTC, set on success)
       - LastStatus           (string: "Never" initial, then "Running"/"Success"/"Failed")
       - LastRecordCount      (int, sum of rows loaded across all mappings in the run)
     On start of a run: IsRunning=true, LastStatus="Running", counters reset.
     On success: LastStatus="Success", LastSyncTime=UtcNow, LastRecordCount=totalRows.
     On failure: LastStatus="Failed". On cancellation: IsRunning=false (status preserved).
  6) LastRecordCount = sum of data.Rows.Count over successfully loaded mappings only.
Status fields default behavior when no SyncSettings row exists:
  - Engine does NOT auto-sync (IsAutoSyncEnabled default = false). Manual trigger still works.
  - IsRunning=false, LastStatus="Never", LastSyncTime=null, LastRecordCount=0 until a run occurs.
  - The service keeps polling (every 5 min) so it picks up configuration as soon as the row exists.
Assumptions:
  - SyncSettings row is a singleton (Id=1) per data model §1.3.
  - The upcoming API controller will inject SyncEngineService (registered as hosted singleton) to
    read the status fields; writing LastSyncTimestamp back to the DB is left to that API/persistence
    task (this task is read-only on SyncSettings as specified).
  - Connection string password supplied via SQL_PASSWORD env var (no secrets in files).
Issues or Missing Information:
  - None blocking. DB not reachable from this network, so DB read path is compile-verified only;
    behavior on missing row / unreachable DB is implemented and falls back safely by design.
Decisions Needed from Tera:
  - None. Optional follow-up (separate task): persist LastSyncTime back into SyncSettings on success.
Recommendation:
  - Accept. Ready for the sync API task (trigger + status + config endpoints from §5).
```

## 10. Tera Review

| Check | Result | Notes |
|---|---|---|
| TASK objective completed? | PASS | Scheduling config + status fields implemented |
| Output matches approved scope? | PASS | Only SyncEngineService changed under Api |
| No files outside Allowed Write Targets? | PASS | Only Api service + this task file |
| No forbidden files created? | PASS | |
| No extra libraries added? | PASS | Only existing Microsoft.Data.SqlClient used |
| No secrets or real `.env`? | PASS | SQL_PASSWORD via env var through ConnectionStringHelper |
| Technology Profile respected? | PASS | .NET / ADO.NET per profile |
| UI/UX rules respected if UI exists? | N/A | Backend only |
| Acceptance Criteria passed? | PASS | All 8 criteria met |
| Rollback needed? | No | |

## 12. Post-Execution Review Result

| Item | Status |
|---|---|
| Gate Result | PASS |
| Reviewer | EngineeringAgent (self-check) |
| Review Date | 2026-07-13 |
| Notes | Build clean; safe-default behavior verified by code path analysis |

## 13. Final Tera Decision

| Item | Value |
|---|---|
| Final Status | Accepted (pending Tera confirmation) |
| Registry Updated | No |
| Activity Log Updated | No |
| Project State Updated | No / N/A |
| Issues/Gaps Updated | No / N/A |
| Next Action | Proceed to sync API task (trigger/status/config endpoints) |
