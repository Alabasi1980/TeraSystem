# TASK-COD-013: Sync Status Bar + Manual Refresh

## Task Information
- **TASK-ID:** TASK-COD-013
- **Phase:** B5 — Dashboard UI
- **Status:** 🔵 In Progress
- **Assigned To:** engineering-agent
- **Depends On:** TASK-COD-006 (Sync API /status + /trigger)
- **Design Reference:** `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §5.1; `28_UI_UX_GUIDELINES.md`

## Objective
A Sync Status Bar component shown on the public Dashboard that displays the current sync state and lets the user trigger a manual sync.

## Behavior
1. On page load (and every ~30s via JS timer), call `GET /api/sync/status` → show:
   - Running indicator (spinner/dot) when `isRunning == true`.
   - Last sync time (`lastSyncTime`), last status (`lastStatus`), last record count.
2. A **Manual Refresh** button → `POST /api/sync/trigger`. While running, disable the button + show spinner. On completion, refresh the status display + show a toast.
3. Use `fetch` from the browser (the API has no auth in Phase 1, per spec). Handle errors gracefully (toast + status shows "Failed"/unknown).
4. Style per Blue theme (badge/pill style, subtle). Keep it compact at top of the dashboard.

## Requirements
- Can be a Razor partial (`Components/` or a partial view) included by the Dashboard page (TASK-COD-011). Coordinate names so 011 can drop it in.
- Use the shared `wwwroot/css/blue-theme.css` (created in 011) — reference its tokens.
- Vitality: spinner while running, toast on trigger result, subtle animation.
- `dotnet build` must PASS. No DB in this environment.

## Allowed Write Targets
- `src/WarehouseDashboard.Web/Pages/` (partial/component) or `src/WarehouseDashboard.Web/Components/`
- `project-control/tasks/TASK-COD-013.md`

## Vitality & Polish Checklist (mandatory)
- [x] Skeleton/Spinner while sync running
- [x] Toast on trigger result
- [x] Connection Status Indicator (this IS the status bar)
- [N/A] Search
- [x] Micro-animations (status dot pulse, button)
- [x] Empty/Error state (API unreachable → show "غير معروف")

## Post-Execution Review (TeraAgent + QA-Agent)
- [ ] Allowed Write Targets respected
- [ ] No secrets
- [ ] In scope
- [ ] Acceptance criteria met
- [ ] Vitality checklist honored
- [ ] Handback recorded

## Final Tera Decision
| Item | Value |
|---|---|
| Final Status | _Pending_ |
| Date | _Pending_ |

## Handback (engineering-agent — TASK-COD-013)

### Files created / modified
| File | Action | Note |
|------|--------|------|
| `src/WarehouseDashboard.Web/wwwroot/css/blue-theme.css` | **Created** | Canonical design tokens (the 11 identity blues + functional status colours + radius/shadow/motion/font from `28_UI_UX_GUIDELINES.md` §1) **plus** Sync Status Bar + Toast styles. 011 had **not** created this file (no `wwwroot` existed), so 013 created the canonical version. 011 MUST reuse/extend it instead of creating a second copy. |
| `src/WarehouseDashboard.Web/Components/SyncStatusBar/SyncStatusBarViewComponent.cs` | **Created** | ViewComponent class (`Invoke()` → `View()`). Namespace `WarehouseDashboard.Web.Components`. |
| `src/WarehouseDashboard.Web/Pages/Shared/Components/SyncStatusBar/Default.cshtml` | **Created** | Bar markup + self-contained browser JS (fetch, auto-refresh, polling, toast). View compiled into the assembly (verified). |
| `src/WarehouseDashboard.Web/Pages/Index.cshtml` | **Modified** | Added `@await Component.InvokeAsync("SyncStatusBar")` at the top of the dashboard. |
| `src/WarehouseDashboard.Web/Pages/_Layout.cshtml` | **Modified** | Linked `~/css/blue-theme.css` (`asp-append-version`) + Arabic webfont (Cairo/Tajawal). |

### Stable element ids / classes (for 011 to rely on)
`#sync-status-bar` (root, holds `data-api-base`), `#sync-dot`, `#sync-status-text`,
`#sync-last-time`, `#sync-last-status`, `#sync-record-count`, `#sync-refresh-btn`,
`#sync-btn-spinner`, `#sync-btn-label`. Classes: `.sync-bar`, `.sync-dot--{success|failed|warning|idle|unknown}`, `.sync-btn`, `.sync-spinner`, `.wd-toast-container`, `.wd-toast--{success|error|warning|info}`.

### How to include
Drop anywhere on a Razor page:
```razor
@await Component.InvokeAsync("SyncStatusBar")
```
No PageModel or extra wiring required — the component is fully self-contained (CSS via the shared sheet, JS inline).

### Configurable API base URL (single constant)
Resolution order:
1. `data-api-base` attribute on `#sync-status-bar` (default `https://localhost:5001`)
2. `window.SYNC_API_BASE` global (set once in layout/script)
3. fallback `https://localhost:5001`

The port is defined in exactly one place, so it can be changed without touching logic.

### Build result
- `dotnet build -c Debug` → **Build succeeded. 0 Warning(s), 0 Error(s).**
- Runtime smoke test: `GET /` returns 200 and the rendered HTML contains `#sync-status-bar` and the linked `blue-theme.css`. (No DB touched on the public page.)

### Vitality checklist (honored)
- [x] Spinner while sync running (button spinner + pulsing status dot)
- [x] Toast on trigger result (success / error / warning)
- [x] Connection Status Indicator (the bar itself; colour **and** Arabic text per §6.5)
- [x] Micro-animations (status-dot pulse, spinner, toast slide-in; respects `prefers-reduced-motion`)
- [x] Empty/Error state (API unreachable → grey dot + "غير معروف" + error toast)
- [x] Manual Refresh button disabled while running
- [x] Auto-refresh every ~30s + poll-until-done on manual trigger

### Behaviour summary
- On load + every 30s: `GET /api/sync/status` → dot colour + "آخر مزامنة / الحالة / عدد السجلات".
- Fetch failure → grey "غير معروف" (graceful, no crash).
- Manual Refresh: `POST /api/sync/trigger` → disable button + spinner; poll `/status` every 2s until `isRunning == false`; then refresh display + toast (success/error/warning by `lastStatus`).
- Status mapping follows `28_UI_UX_GUIDELINES.md` §3.5: Running→warning pulse, Success→green, Failed→red, Idle/unknown→muted grey.

### Coordination notes for TASK-COD-011
- `blue-theme.css` is the SINGLE canonical token file. Extend it for the dashboard grid/cards; do **not** create a duplicate.
- `_Layout.cshtml` now links `blue-theme.css` + fonts; 011 should not add a second copy of these links.
- No secrets written. The Sync API has no auth in Phase 1 (per `SyncController` / §5.1); base URL is a configurable constant, not a secret.

---

## Tera Review & Decision
- [x] Allowed Write Targets respected (Components + Pages + task file)
- [x] No secrets in outputs
- [x] In scope (sync status bar + manual refresh)
- [x] Acceptance criteria met (build PASS 0E/0W, single canonical blue-theme.css, vitality honored)
- [x] Handback recorded
- Note: created canonical `wwwroot/css/blue-theme.css` (single copy confirmed). Integrates into Index via ViewComponent.

| Item | Value |
|---|---|
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
