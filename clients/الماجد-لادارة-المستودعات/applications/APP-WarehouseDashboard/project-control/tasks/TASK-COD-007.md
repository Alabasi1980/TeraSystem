# TASK-COD-007 — Config-Driven Sync Scheduling + Status Tracking

## Task Information
- **TASK-ID:** TASK-COD-007
- **Phase:** B3 — Infrastructure
- **Status:** ✅ Accepted (build PASS)
- **Assigned To:** engineering-agent
- **Depends On:** TASK-COD-005 (SyncEngineService base)
- **Design Reference:** `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §5.2; `06_DATA_MODEL_PREPARATION.md` §1.3

## Objective
Make the Sync Engine scheduling configurable via the `SyncSettings` table (read via ADO.NET in Api) and expose thread-safe run-status fields for the upcoming Sync API.

## Handback (by engineering-agent, 2026-07-13)

**File changed:** `WarehouseDashboard.Api/Services/SyncEngineService.cs` (rewritten).

### What was implemented
1. **Config-driven scheduling (ADO.NET):** `LoadSyncSettingsAsync` reads `SELECT IntervalMinutes, IsAutoSyncEnabled, LastSyncTimestamp FROM SyncSettings WHERE Id = 1` via `Microsoft.Data.SqlClient`. `ExecuteAsync` loop: if `IsAutoSyncEnabled == false` → auto runs skipped (polls every 5 min); if enabled → `PeriodicTimer(IntervalMinutes)` (clamped min 1), recreates timer when config changes.
2. **Safe default when no row / DB unreachable:** `IntervalMinutes=30`, `IsAutoSyncEnabled=false` → never auto-syncs until explicitly configured. Fallback log throttled.
3. **Thread-safe status fields:** `IsRunning` (bool), `LastSyncTime` (DateTime? UTC), `LastStatus` (`"Never"→"Running"/"Success"/"Failed"`), `LastRecordCount` (int, sum over mappings). Guarded by `_statusLock`.
4. **Overlap safety:** `SemaphoreSlim` acquired inside `RunSyncOnceAsync` → manual trigger during auto-run skipped safely. `RunSyncOnceAsync` stays public. Per-table try/catch + retry (3) preserved.

**Build:** `dotnet build` → PASS (0W/0E). No DB connection in this env.
**Note:** Writing `LastSyncTime` back to `SyncSettings` on success deferred to a later persistence task (this task is read-only on SyncSettings per spec).

## Tera Review & Decision
- [x] Allowed Write Targets respected (src/Api + task file)
- [x] No secrets in outputs (SQL_PASSWORD via env var)
- [x] In scope (scheduling config + status fields)
- [x] Acceptance criteria met (build PASS, safe default, thread-safe fields)
- [x] Handback recorded

| Item | Value |
|---|---|
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
| Notes | SyncSettings read via ADO.NET (Api has no Web DbContext). Auto-sync off until configured. |