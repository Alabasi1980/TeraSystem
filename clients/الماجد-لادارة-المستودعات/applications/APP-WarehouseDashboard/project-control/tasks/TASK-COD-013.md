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
