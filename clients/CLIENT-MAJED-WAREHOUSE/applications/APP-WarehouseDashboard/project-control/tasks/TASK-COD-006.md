# TASK-COD-006 — Sync REST API Endpoints

- **Status:** `DONE`
- **Agent:** EngineeringAgent (مهندس)
- **Date:** 2026-07-13
- **Project:** WarehouseDashboard.Api
- **Reference:** `project-preparation/14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §5.1 / §5.2
- **Depends on:** TASK-COD-007 (`SyncEngineService` status properties + `RunSyncOnceAsync`)

---

## Objective

Implement the four Sync REST API endpoints in `WarehouseDashboard.Api`, per
`14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §5.1:

1. `POST /api/sync/trigger`
2. `GET  /api/sync/status`
3. `GET  /api/sync/logs`
4. `GET  /api/sync/config`

---

## What was produced

### New files (under `src/WarehouseDashboard.Api/`)

| File | Purpose |
|------|---------|
| `Controllers/SyncController.cs` | The 4 endpoints (attribute routing `[Route("api/sync")]`, `[ApiController]`). |
| `Models/SyncRunRecord.cs` | Small record class for one sync run: `StartTime`, `EndTime`, `Status`, `RecordCount`, `TriggerType`, `ErrorMessage`, plus computed `DurationSeconds`. |
| `Services/SyncRunLogStore.cs` | Thread-safe, in-memory ring buffer (`List<SyncRunRecord>`, max 100) singleton. `BeginRun` / `CompleteRun` / `GetRecent` (newest first). |

### Modified files

| File | Change |
|------|--------|
| `Program.cs` | Registered `SyncEngineService` as a **singleton** AND as the hosted background service via `AddHostedService(sp => sp.GetRequiredService<SyncEngineService>())` so the controller-injected engine and the running engine are the **same instance** (shared status). Added `AddSingleton<SyncRunLogStore>()`. |

`HealthController.cs` was left **intact**.

---

## Endpoints (behavior)

### 1. `POST /api/sync/trigger`
- Awaits `_syncEngine.RunSyncOnceAsync(cancellationToken)`.
- Opens an in-memory run record (status `"Running"`) at start; finalizes it with the
  engine's `LastStatus` / `LastRecordCount` when the call returns (or `"Cancelled"` /
  `"Failed"` on exception).
- Returns `{ "status": "triggered", "message": "..." }`.
- Overlap guard is delegated to the engine's `SemaphoreSlim` (a trigger arriving during a
  running cycle returns immediately).

### 2. `GET /api/sync/status`
- Returns `{ "isRunning", "lastSyncTime", "lastStatus", "lastRecordCount" }` read directly
  from the `SyncEngineService` status properties.

### 3. `GET /api/sync/logs`
- Returns the in-memory ring buffer (newest first, max 100) as
  `{ startTime, endTime, status, recordCount, duration, triggerType, errorMessage }`.
- **Temporary** — see note below.

### 4. `GET /api/sync/config`
- Reads `SyncSettings` (row `Id = 1`) via ADO.NET (`Microsoft.Data.SqlClient` +
  `ConnectionStringHelper.ResolveSql`).
- Returns `{ "intervalMinutes", "isAutoSyncEnabled", "lastSyncTimestamp" }`.
- On empty connection string / missing row / DB unreachable → safe defaults
  `{ "intervalMinutes": 30, "isAutoSyncEnabled": false, "lastSyncTimestamp": null }`.

---

## IMPORTANT — In-memory logs are temporary

`GET /api/sync/logs` is backed by `SyncRunLogStore`, an **in-memory ring buffer**, not a
database. This is explicitly a stopgap until the structured `SyncLogs` / `ErrorLogs` DB
tables are created (deferred task). Consequences:

- Data is **lost on process restart**.
- It must **NOT** be used for audit or compliance — live monitoring only.
- Replacement by DB-backed logging is tracked as a follow-up task.

This is documented in code comments on `SyncRunLogStore`, `SyncRunRecord`, and `SyncController.Logs`.

---

## Security notes

- **No authentication** on these endpoints (Phase 1 internal API, per spec §5.2).
- A code comment in `SyncController` records that the planned protection is **IIS IP &
  Domain Restrictions** at the web-server layer. Do not expose this API outside the
  trusted internal network.
- SQL password is resolved from the `SQL_PASSWORD` env var via `ConnectionStringHelper`
  (never hardcoded). No secrets are written to any file.
- The `SyncSettings` query uses a fixed `WHERE Id = 1` literal (no user input) — no
  SQL-injection surface.

---

## Build result

```
dotnet build WarehouseDashboard.Api.csproj -v minimal
  Build succeeded.
  0 Warning(s)
  0 Error(s)
```

No DB connection is required to build; the `/config` endpoint degrades gracefully to safe
defaults when the DB is unreachable (verified by design — this environment has no SQL Server).

---

## Handback to TeraAgent

- **Files changed/created:** `Controllers/SyncController.cs` (new), `Models/SyncRunRecord.cs`
  (new), `Services/SyncRunLogStore.cs` (new), `Program.cs` (modified).
- **Endpoints live:** `POST /api/sync/trigger`, `GET /api/sync/status`,
  `GET /api/sync/logs`, `GET /api/sync/config`.
- **Status:** `DONE`.
- **Blockers:** None. DB logging (replacing the in-memory ring buffer) is a deferred follow-up.

---

## Tera Review & Decision
- [x] Allowed Write Targets respected (src/Api + task file)
- [x] No secrets in outputs (SQL_PASSWORD via env var)
- [x] In scope (4 sync API endpoints)
- [x] Acceptance criteria met (build PASS, all 4 endpoints, graceful config fallback, shared engine instance)
- [x] Handback recorded
- Note: in-memory /logs is temporary until SyncLogs/ErrorLogs DB tables built (deferred). No API auth per Phase-1 spec (IIS IP restriction planned).

| Item | Value |
|---|---|
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
