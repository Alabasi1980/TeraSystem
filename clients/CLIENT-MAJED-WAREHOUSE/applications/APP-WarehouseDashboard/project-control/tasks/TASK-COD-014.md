# TASK-COD-014: Filtering + Search

## Task Information
- **TASK-ID:** TASK-COD-014
- **Phase:** B5 — Dashboard UI
- **Status:** 🔵 In Progress
- **Assigned To:** engineering-agent
- **Depends On:** TASK-COD-011 (Dashboard Main Page)
- **Design Reference:** `28_UI_UX_GUIDELINES.md`

## Objective
Add search + filtering to the public Dashboard so the user can quickly find cards.

## Behavior
1. A **search box** at the top of the dashboard: as the user types, filter the visible cards by `Title` (client-side filter over the already-rendered cards; case-insensitive, Arabic/English).
2. A **ChartType filter** (dropdown: All / Bar / Line / Pie / KPI / Table / Gauge): filters cards by their type.
3. Filtering is client-side (over the card metadata already loaded by the Index page) — do NOT re-query the DB per keystroke. If a card is hidden by the filter, it collapses smoothly.
4. Show a "no results" state when the filter matches nothing.
5. Blue theme (reuse `wwwroot/css/blue-theme.css`); integrate cleanly with the existing Sync Status Bar (TASK-COD-013) and the dashboard grid.

## Requirements
- Modify the Dashboard `Index.cshtml` (and its `.cs` if needed) to add the search box + filter dropdown + JS filtering logic. Keep the existing card-fetch/skeleton behavior intact.
- Vitality: smooth collapse/expand on filter, toast or subtle hint when no results, micro-animation.
- `dotnet build` must PASS. No DB in this environment.

## Allowed Write Targets
- `src/WarehouseDashboard.Web/Pages/Index.cshtml` (+ `.cs` if needed)
- `project-control/tasks/TASK-COD-014.md`

## Vitality & Polish Checklist (mandatory)
- [x] Skeleton/empty on no-results
- [x] Toast (optional hint when no results)
- [N/A] Connection Status Indicator
- [x] Search (this IS the search task)
- [x] Micro-animations (filter transition)

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

## Handback

**Status:** DONE ✅ — build passes, behavior implemented, no secrets written.

### Files Changed
| File | Change |
|---|---|
| `src/WarehouseDashboard.Web/Pages/Index.cshtml` | Added filter bar (search input + ChartType `<select>`), `data-title` + `data-chart-type` attributes on each card, a no-results empty state, page-scoped `<style>` reusing `blue-theme.css` tokens, an `info` toast variant, and a client-side filtering IIFE in the Scripts section. |
| `project-control/tasks/TASK-COD-014.md` | This Handback section. |

> `Index.cshtml.cs` was **not** modified — `CardLayoutInfo` already exposes `Title` and `ChartType`, sufficient to render the filter attributes server-side.

### Behavior Implemented
1. **Search box** below the page header (below the SyncStatusBar topbar): filters visible cards by `data-title` (case-insensitive, Arabic + English via `toLocaleLowerCase`). Purely client-side over already-rendered card metadata — **no DB query per keystroke**; typing is debounced 120ms.
2. **ChartType dropdown**: All / Bar / Line / Pie / KPI / Table / Gauge — filters by `data-chart-type`. Combines with the search (AND logic).
3. **Smooth collapse**: hidden cards fade + scale (`wd-card--fade`) then `display:none` via a per-element sequence guard that survives rapid keystrokes. Shown cards animate back in.
4. **No-results state**: when 0 cards match, a "لا توجد بطاقات مطابقة" empty state (`wdFadeUp` animation) is revealed, plus an `info` toast hint on first transition to empty.
5. **Existing behavior intact**: per-card fetch loop, skeleton, manual refresh (`wdRefreshAll`), error/retry, and the SyncStatusBar (connection badge + refresh button in `_DashboardLayout.cshtml`) are completely untouched. Hidden cards still load their data in the background so re-showing is instant.

### Build Result
- `dotnet build` → **Build succeeded. 0 Warning(s), 0 Error(s).**
- Note: first attempt failed only due to a file lock — a running `WarehouseDashboard.Web` instance (PID 20672) held the output `.exe`. Stopped that dev instance and rebuilt clean. No compile errors were present (only MSB302x copy-lock messages).

### Vitality / Polish
- [x] Smooth collapse/expand transition (opacity + scale).
- [x] Micro-animation: `wdFadeUp` entrance on empty state; fade/scale on filter.
- [x] Subtle hint/toast when no results (`info` toast + empty-state panel).
- [x] Blue theme tokens (`--c-primary`, `--c-border`, `--c-bg`, `--c-surface`, `--c-text-muted`, `--c-info`, focus ring `rgba(31,78,121,.18)`) — no hardcoded off-palette colors.
- [x] RTL-aware (`inset-inline-end` for the search icon; Arabic labels).

### Out of Scope / Notes
- The dashboard grid uses server-assigned `grid-column-start`/`grid-row-start`; filtered-out cards keep their slot (gaps may appear) rather than reflowing — correct for a manually-positioned dashboard. Collapse is the animation; positions are preserved.
- No changes to `blue-theme.css` (not an allowed write target); new component classes live in a page-scoped `<style>` that consumes the shared tokens.

---

## Tera Review & Decision
- [x] Allowed Write Targets respected (Index.cshtml + task file)
- [x] No secrets
- [x] In scope (search + filter)
- [x] Acceptance criteria met (build PASS 0E/0W, client-side filter, vitality honored, existing behavior intact)
- [x] Handback recorded

| Item | Value |
|---|---|
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
