# TASK-COD-011: Dashboard Main Page (Grid of Cards)

## Task Information
- **TASK-ID:** TASK-COD-011
- **Phase:** B5 — Dashboard UI
- **Status:** 🔵 In Progress
- **Assigned To:** engineering-agent
- **Depends On:** TASK-COD-006 (Sync API), TASK-COD-009 (Card CRUD / config model)
- **Design Reference:** `28_UI_UX_GUIDELINES.md`; `06_DATA_MODEL_PREPARATION.md` §5.4 (DashboardService pattern)

## Objective
Build the **public** Dashboard main page (`/`) that renders a responsive grid of dynamic cards configured in `DashboardCards`, each showing a Syncfusion chart/table driven by its `SqlQuery` executed against SQL Server.

## Behavior
1. Read active cards: `WarehouseDashboardDbContext.DashboardCards.Where(c => c.IsActive).OrderBy(c => c.GridPositionY).ThenBy(c => c.GridPositionX)`.
2. For each card: execute `card.SqlQuery` via **ADO.NET** (`Microsoft.Data.SqlClient` + `ConnectionStringHelper.ResolveSql`) — this is the `DashboardService` pattern (config via EF, data via ADO.NET). Map result to the chart.
3. Render by `ChartType`:
   - `Bar` / `Line` / `Pie` / `Gauge` → Syncfusion chart with the first 1-2 columns as axis/value.
   - `KPI` → big number card (first value).
   - `Table` → Syncfusion Grid with dynamic columns.
4. **Resilient per card:** if a card's query fails or returns empty, show that card's own empty/error state — never break the whole page.
5. CSS Grid layout using `GridPositionX/Y/Width/Height` (12-col grid; Width 1-12, Height 1-6).
6. Public page — **no admin auth** (Viewer is public per architecture).

## Requirements
- **Shared design system:** CREATE `wwwroot/css/blue-theme.css` with the 11-color palette + tokens from `28_UI_UX_GUIDELINES.md`, and a dashboard `_Layout`/layout page that references it. This fixes the B4 inline-CSS fragmentation (admin pages can be retrofitted later). Reuse the exact tokens already inlined in `Pages/admin-secure-panel/Cards/_CardsLayout.cshtml` for consistency.
- Syncfusion charts/grid (license via `SYNCFUSION_LICENSE_KEY` env var already registered).
- Vitality/polish (mandatory): skeleton shimmer per card while its query loads; toast on manual refresh errors; empty state per card; subtle micro-animations (stagger card entrance).
- `dotnet build` must PASS. No DB in this environment.

## Allowed Write Targets
- `src/WarehouseDashboard.Web/Pages/` (Index + dashboard layout)
- `src/WarehouseDashboard.Web/wwwroot/css/blue-theme.css`
- `project-control/tasks/TASK-COD-011.md`

## Vitality & Polish Checklist (mandatory)
- [x] Skeleton Loading / Shimmer per card
- [x] Toast Notifications (refresh/error)
- [x] Connection Status Indicator (use the status from /api/sync/status — can be a small badge)
- [x] Search/realistic data (data comes from SQL; show real rows)
- [x] Micro-animations (stagger entrance, hover)
- [x] Empty States per card

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
