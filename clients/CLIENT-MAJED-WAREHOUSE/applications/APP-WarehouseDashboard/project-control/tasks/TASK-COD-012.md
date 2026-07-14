# TASK-COD-012: Drill Down Pages

## Task Information
- **TASK-ID:** TASK-COD-012
- **Phase:** B5 — Dashboard UI
- **Status:** 🔵 In Progress
- **Assigned To:** engineering-agent
- **Depends On:** TASK-COD-011 (Dashboard Main Page), TASK-COD-009 (CardDrillDownLevels config)
- **Design Reference:** `06_DATA_MODEL_PREPARATION.md` §1.2; `28_UI_UX_GUIDELINES.md`

## Objective
Clicking a card (or a drill icon on a card) opens a Drill-Down page that shows the card's first drill level, with the ability to go deeper into subsequent levels (2+ levels), with breadcrumb navigation.

## Behavior
1. Card click → navigate to `/Dashboard/Drill/{cardId}` (or `/Drill/{cardId}`).
2. Load `CardDrillDownLevels` for the card ordered by `Level`. Show Level 1: execute `Level1.DrillDownQuery` via ADO.NET (read-only, `ConnectionStringHelper.ResolveSql`), render by `TargetChartType` (Bar/Line/Pie/KPI/Table/Gauge).
3. Clicking a data row (or a drill button on a row) → go to Level 2: pass the clicked row's identifier (e.g., first column value) as a parameter into Level 2's `DrillDownQuery`. Support a simple, safe parameter convention (SQL parameter — NEVER string concatenation; use `SqlParameter` with a `@p0` placeholder in the query, value taken from the clicked row). If the query has no parameter, just run it.
4. Breadcrumb: Card Title › Level 1 (DisplayName) › Level 2 … allowing back-navigation.
5. Resilient per level (error/empty states). Blue theme (reuse `wwwroot/css/blue-theme.css`).

## Requirements
- ADO.NET read-only execution (same pattern as DashboardService). Per-level try/catch.
- Vitality: skeleton on level load, toast on errors, empty state per level, micro-animations, breadcrumb.
- `dotnet build` must PASS. No DB in this environment.

## Allowed Write Targets
- `src/WarehouseDashboard.Web/Pages/` (Drill pages + any controller/endpoint for drill data)
- `project-control/tasks/TASK-COD-012.md`

## Vitality & Polish Checklist (mandatory)
- [x] Skeleton Loading per level
- [x] Toast on errors
- [x] Empty States per level
- [x] Micro-animations (breadcrumb, level entrance)
- [N/A] Connection Status Indicator (use inherited bar)
- [N/A] Search

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

---

## Handback

**Engineer:** engineering-agent · **TASK-COD-012** · Drill-Down pages for WarehouseDashboard

### Files created (under `src/WarehouseDashboard.Web/Pages/`)
| File | Purpose |
|------|---------|
| `DrillDataResult.cs` | Payload returned by the drill API — mirrors `CardDataResult` and adds `Level`, `DisplayName`, `HasNextLevel` for drill navigation. |
| `Api/Dashboard/Drill.cshtml` + `Drill.cshtml.cs` | Razor Pages API: `GET /api/dashboard/drill/{cardId:int}/{level:int}?parentValue=...`. Read-only ADO.NET. Per-level try/catch. CamelCase JSON (mirrors admin `DrillDown` `Json` helper so the client reads `data.columns`/`data.status`/…). |
| `Dashboard/Drill.cshtml` + `Drill.cshtml.cs` | Public drill page `GET /Dashboard/Drill/{cardId}`. Server loads card title + ordered levels (EF Core, no data query); client fetches each level's data via the API. Breadcrumb + vitality states. Reuses `_DashboardLayout` + `wwwroot/css/blue-theme.css`. |

### Files modified
| File | Change |
|------|--------|
| `Index.cshtml` + `Index.cshtml.cs` | Entry point: cards that have ≥1 configured drill level become clickable (`wdOpenDrill(id)` → `/Dashboard/Drill/{id}`), with a hover "›" affordance and keyboard support. Drillable set computed via `CardsWithDrill` (distinct `ParentCardId` from `CardDrillDownLevels`). |

### Parameter-passing convention (security)
- Each level's `DrillDownQuery` **may** contain a single `@p0` placeholder.
- Bound **only** via `SqlParameter("@p0", value)` — **never** string concatenation.
- The clicked row's **first column value** becomes `parentValue`:
  - Numeric-looking values are bound as `int`/`long`/`decimal`; `bool` as bool; otherwise as `string`. `null` → `DBNull.Value`.
  - If the query has **no** `@p0`, it runs verbatim (no parameter added).
- **Chaining:** deeper levels use the same convention. The drill chain travels in the URL as `?level=N&pv=val1|val2|...` (pipe-separated). The `@p0` for the current level = `pv.split('|')[level-2]`. Clicking a row/point pushes the first-column value and navigates to `level+1`.
- Breadcrumb back-navigation rebuilds the correct `pv` prefix for each earlier level.

### Build result
- `dotnet build` → **succeeded, 0 Warning(s), 0 Error(s).**
- Note: the build initially failed only with `MSB3021/MSB3027` file-copy errors because a previously running instance of the app held a lock on `bin\Debug\net8.0\WarehouseDashboard.Web.exe|.dll`. After releasing that process the build passed cleanly. This is an environment artifact, not a code defect. No DB is required to build.

### Vitality / polish delivered
- **Skeleton** shimmer on every level load.
- **Toast** on level/connection errors via shared `wd-toast-host`.
- **Empty state** per level (`empty` / `none`) and **error state** with retry button (per-level try/catch in the API).
- **Micro-animations:** `wdFadeUp` entrance on the drill card and breadcrumb; breadcrumb items animate in.
- **Breadcrumb:** `Card Title › Level 1 › Level 2 …` with clickable back-navigation.
- Blue theme reused (`_DashboardLayout` + `blue-theme.css` tokens) — no new CSS files (all scoped inline, staying within the project's allowed write targets).

### Security checklist
- Read-only ADO.NET (`SqlCommand`/`SqlDataReader`), same `DashboardService` pattern.
- Only `@p0` is ever bound, via `SqlParameter` — no concatenation of any user/row value into SQL.
- `SQL_PASSWORD` resolved from env var via `ConnectionStringHelper.Resolve` (never stored in source/config).
- `Sanitize()` strips the resolved password from any error message before it reaches the browser.
- No secrets written.

### Out of scope / notes
- Deeper-than-configured navigation is guarded (breadcrumb + `HasNextLevel`); the API returns `status:"none"` for an unconfigured level.
- Charts (Bar/Line/Pie) drill on point click; Table drills on row-select or the "تعمّق" command button; KPI/Gauge drill on click of the value when a next level exists.

---

## Tera Review & Decision
- [x] Allowed Write Targets respected (Pages + task file)
- [x] No secrets; secure @p0 SqlParameter only
- [x] In scope (drill-down pages + API)
- [x] Acceptance criteria met (build PASS 0E/0W, breadcrumb, secure param passing, vitality honored)
- [x] Handback recorded

| Item | Value |
|---|---|
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
