# TASK-COD-SYNC-QUEUE-01: Centralized Sync Queue System

## Status: Approved
## Phase: 6 (Implementation)
## Created: 2026-07-21
## Priority: High

---

## Objective

Replace the current skip-on-conflict semaphore pattern with a **centralized sync queue** that ensures:
1. Only one sync operation runs at a time
2. No sync request is ever lost (skipped)
3. Selected sync requests are merged when possible
4. All tables get synced without failure due to contention

## Problem Statement

Currently, `RunSyncOnceAsync` and `RunSelectedMappingsAsync` both use `SemaphoreSlim` with `TimeSpan.Zero` (non-blocking). If a sync is already running, the new request is **skipped**. This means:
- Tables requested during a running sync may never sync
- Manual selected syncs get dropped if auto-sync is running
- User sees "skipped" status with no way to guarantee execution

## Solution: Single Sync Queue

### Architecture

```
┌─────────────────────────────────────────────────┐
│              SyncQueueService                    │
│  (Singleton, manages all sync requests)          │
├─────────────────────────────────────────────────┤
│  Queue: ConcurrentQueue<SyncRequest>             │
│  Processing: Single background task              │
│  Merge Logic: Selected → Full = Full covers all  │
└─────────────────────────────────────────────────┘
         ↓ processes one at a time
┌─────────────────────────────────────────────────┐
│           SyncEngineService                      │
│  (Executes the actual sync work)                 │
└─────────────────────────────────────────────────┘
```

### SyncRequest Types

| Type | Description | Merge Behavior |
|------|-------------|----------------|
| `Full` | Sync all tables | Absorbs any pending Selected requests |
| `Selected` | Sync specific table IDs | Merges with other Selected requests; absorbed by Full |

### Queue Processing Rules

1. **One at a time**: Only one sync operation executes
2. **Full absorbs Selected**: If a Full request is queued, any pending Selected requests are absorbed (Full covers them)
3. **Selected merges**: Multiple Selected requests merge their IDs into one batch
4. **No skipping**: Every request waits in queue until executed
5. **Deduplication**: If Full is already queued, new Selected requests are absorbed; if Selected for same IDs is queued, new request updates the existing one

### API Changes

| Endpoint | Before | After |
|----------|--------|-------|
| `POST /api/sync/trigger` | Runs immediately or skips | Queues Full request, returns position |
| `POST /api/sync/trigger-selected` | Runs immediately or skips | Queues Selected request, returns position |
| `GET /api/sync/queue` | N/A (new) | Returns queue status and position |

### Response Format

```json
// POST /api/sync/trigger
{
  "status": "queued",
  "position": 2,
  "message": "Full sync queued. Position: 2 in queue."
}

// POST /api/sync/trigger-selected
{
  "status": "queued",
  "position": 1,
  "runId": "guid-here",
  "message": "Selected sync queued. Position: 1 in queue."
}

// GET /api/sync/queue
{
  "isProcessing": true,
  "currentSync": {
    "type": "Full",
    "startedAt": "2026-07-21T10:00:00Z",
    "elapsed": "00:02:30"
  },
  "queue": [
    { "position": 1, "type": "Selected", "mappingIds": [1,3,5], "queuedAt": "..." },
    { "position": 2, "type": "Full", "queuedAt": "..." }
  ],
  "totalQueued": 2
}
```

## Implementation Plan

### Step 1: Create `SyncQueueService.cs`

New file: `src/WarehouseDashboard.Api/Services/SyncQueueService.cs`

- `ConcurrentQueue<SyncRequest>` for pending requests
- `SyncRequest? _currentRequest` for the running request
- `SemaphoreSlim _processor` to ensure single processing
- Background processing loop via `BackgroundService`
- `EnqueueFull()` → adds Full request, absorbs pending Selected
- `EnqueueSelected(List<int> ids)` → adds/merges Selected request
- `GetQueueStatus()` → returns current queue state
- Callback to `SyncEngineService` to execute the actual work

### Step 2: Modify `SyncEngineService.cs`

- Remove `SemaphoreSlim _semaphore` (no longer needed)
- Remove semaphore checks from `RunSyncOnceAsync` and `RunSelectedMappingsAsync`
- Keep retry logic per-table (unchanged)
- The queue controls concurrency, not the engine

### Step 3: Modify `SyncController.cs`

- `Trigger()` → calls `_queue.EnqueueFull()` instead of `_syncEngine.RunSyncOnceAsync()`
- `TriggerSelected()` → calls `_queue.EnqueueSelected()` instead of fire-and-forget
- Add `GET /api/sync/queue` endpoint

### Step 4: Register in DI

- `Program.cs`: Register `SyncQueueService` as hosted service + singleton

### Step 5: Update Background Service

- `SyncEngineService.ExecuteAsync()` → calls `_queue.EnqueueFull()` for auto-sync timer
- Queue processes requests sequentially

## Files to Modify

| File | Action |
|------|--------|
| `src/WarehouseDashboard.Api/Services/SyncQueueService.cs` | **CREATE** — New queue service |
| `src/WarehouseDashboard.Api/Services/SyncEngineService.cs` | **MODIFY** — Remove semaphore, accept queue callbacks |
| `src/WarehouseDashboard.Api/Controllers/SyncController.cs` | **MODIFY** — Use queue, add queue status endpoint |
| `src/WarehouseDashboard.Api/Program.cs` | **MODIFY** — Register SyncQueueService |

## Acceptance Criteria

- [ ] Only one sync runs at a time (queue enforces this)
- [ ] No sync request is skipped — all requests are queued and executed
- [ ] Full sync absorbs pending Selected requests
- [ ] Multiple Selected requests merge into one batch
- [ ] `GET /api/sync/queue` returns accurate queue status
- [ ] Auto-sync timer enqueues Full requests (not direct execution)
- [ ] Manual trigger enqueues requests (not direct execution)
- [ ] Build succeeds with no errors
- [ ] Per-table retry logic preserved (unchanged)
- [ ] Progress tracking (`SyncRunProgressStore`) still works for Selected syncs

## Out of Scope

- Priority system (e.g., urgent vs normal) — future enhancement
- Queue persistence (in-memory only) — server restart clears queue
- Cancel/deque individual requests — future enhancement
- Web UI for queue visualization — future enhancement

## Risk

- **Low**: This is a concurrency improvement, not a feature change
- **Mitigation**: Existing retry logic and error handling preserved
- **Test**: Verify by triggering multiple syncs rapidly

---

## Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncQueueService.cs` (CREATE)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs` (EDIT)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Controllers\SyncController.cs` (EDIT)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Program.cs` (EDIT)

## Allowed Read Sources

- All files under `src/WarehouseDashboard.Api/` — read-only reference
- `src/WarehouseDashboard.Api/Models/TableMapping.cs`
- `src/WarehouseDashboard.Api/Infrastructure/SyncRunProgressStore.cs`
- `src/WarehouseDashboard.Api/Infrastructure/SyncRunLogStore.cs`
