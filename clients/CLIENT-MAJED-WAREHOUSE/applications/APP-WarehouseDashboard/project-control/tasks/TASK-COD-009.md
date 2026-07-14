# TASK-COD-009: DashboardCards CRUD (List + Editor)

## Task Information
- **TASK-ID:** TASK-COD-009
- **Phase:** B4 — Admin Screens
- **Status:** 🔵 In Progress
- **Assigned To:** engineering-agent
- **Depends On:** TASK-COD-008 (Admin Auth)
- **Design Reference:** `06_DATA_MODEL_PREPARATION.md` §1.1; `28_UI_UX_GUIDELINES.md` (Blue theme)

## Objective
Build the Admin Panel CRUD screens for `DashboardCards` under the protected `/admin-secure-panel/Cards/` area (Razor Pages in `WarehouseDashboard.Web`).

## Screens (all under `/admin-secure-panel/Cards/`)
1. **Index (List):** grid of all cards — Title, ChartType, DataSourceType, IsActive, Grid position/size; actions Edit / Delete. Use Syncfusion Grid for display.
2. **Create / Edit (Editor):** form fields matching `DashboardCards` columns:
   - Title (text), ChartType (dropdown: Bar/Line/Pie/KPI/Table/Gauge), DataSourceType (dropdown: SQL Query/View),
   - SqlQuery (textarea — the SQL or view name), GridPositionX, GridPositionY, GridWidth (1-12), GridHeight (1-6),
   - RefreshInterval (int seconds), IsActive (checkbox).
   - Server-side validation matching the CHECK constraints (ChartType/DataSourceType enums, GridWidth 1-12, GridHeight 1-6, RefreshInterval >= 0).
   - On POST: save via `WarehouseDashboardDbContext` (EF Core). `CreatedAt`/`UpdatedAt` set automatically.
3. **Delete:** confirm + remove (cascade deletes its DrillDownLevels).

## Requirements
- Use `WarehouseDashboardDbContext` (already has `DashboardCards` + `CardDrillDownLevels` DbSets).
- Follow the **Blue theme** in `28_UI_UX_GUIDELINES.md` (palette, cards, spacing). Keep it clean and professional (internal admin tool, but consistent with the dashboard look).
- Use Syncfusion components where they improve UX (Grid, Dropdown, TextBox) — register via the existing Syncfusion setup.
- Read-only on Oracle (this task never touches Oracle).
- No secrets in files; SQL_PASSWORD via env var (DbContext already uses ConnectionStringHelper).

## Allowed Write Targets
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/`
- `src/WarehouseDashboard.Web/` (related .cs files)
- `project-control/tasks/TASK-COD-009.md`

## Vitality & Polish Checklist (UI task — mandatory)
- [x] Skeleton Loading / Shimmer for card list load
- [x] Toast Notifications for save/delete feedback
- [x] Empty State when no cards exist
- [x] Micro-animations on save (subtle)
- [N/A] Connection Status Indicator (admin internal)
- [N/A] Search (small list; can add later)

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
| Final Status | ✅ Accepted |
| Date | 2026-07-13 |
| Reviewer | TeraAgent |
| Notes | Blue theme inlined in _CardsLayout (deviation from global CSS — extract in B5/B6). Cross-task compile fix applied by 010 (transparent). |

---

## Handback

**Objective:** Build the Admin Panel CRUD screens for `DashboardCards` under the protected
`/admin-secure-panel/Cards/` area (Razor Pages), using Syncfusion + the Blue theme, with full
validation mirroring the `DashboardCards` CHECK constraints, cascade delete of drill-down levels,
and the mandatory vitality polish.

**Files created (all under `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/`):**

| File | Purpose |
|------|---------|
| `_CardsLayout.cshtml` | Self-contained Blue-theme layout (design tokens, topbar, toast host, Syncfusion CDN `27.2.3` matched to the installed `Syncfusion.EJ2.AspNet.Core 27.*`, inline CSS — see note below). |
| `_ViewImports.cshtml` | Namespace `...AdminSecurePanel.Cards` + MVC + Syncfusion tag helpers. |
| `_ViewStart.cshtml` | Sets `Layout = "_CardsLayout"` for the area. |
| `Index.cshtml` / `Index.cshtml.cs` | Grid list + skeleton shimmer + empty state + POST delete handler (cascades to `CardDrillDownLevels`). |
| `Create.cshtml` / `Create.cshtml.cs` | Editor form (create). |
| `Edit.cshtml` / `Edit.cshtml.cs` | Editor form (edit, pre-filled). |
| `CardEditorInput.cs` | Bound input model + `IValidatableObject` mirroring the CHECK constraints + dropdown option lists. |

**Build result:** `dotnet build -c Debug` → **Build succeeded, 0 Error(s)** (clean rebuild verified).
One pre-existing warning (`CS8600` null conversion) exists in the out-of-scope COD-010
`DrillDown/Index.cshtml.cs` and is unrelated to this task.

**Vitality / Polish checklist (mandatory — confirmed):**
- [x] **Skeleton shimmer** on list load (`.wd-skel-row` shimmer, hidden on grid `created` + a 700 ms fallback so it never sticks if Syncfusion is slow/offline).
- [x] **Toast notifications** on save & delete (success/error, slide-in animation, rendered via a `Toast` section into the toast host).
- [x] **Empty state** when no cards exist (`wd-empty` card with icon + message + CTA).
- [x] **Micro-animation on save** — editor card entrance (`wdFadeUp`) + toast slide-in.
- [x] Connection-status indicator & search — marked **N/A** per task (internal screen).

**Requirements honored:**
- `WarehouseDashboardDbContext` injected; **read-only on Oracle** (only config tables touched).
- Blue theme tokens from `28_UI_UX_GUIDELINES.md §1` applied (the 11-colour palette, spacing 4px scale, radius, shadow, motion, RTL logical properties).
- Syncfusion components used: **Grid** (list), **DropDownList** (ChartType / DataSourceType), **TextBox** (Title / SqlQuery multiline), **NumericTextBox** (grid coords/size/refresh), **CheckBox** (IsActive). License registered via `SYNCFUSION_LICENSE_KEY` (already in `Program.cs`).
- Server-side validation mirrors every CHECK constraint: `ChartType` ∈ {Bar,Line,Pie,KPI,Table,Gauge}, `DataSourceType` ∈ {SQL Query,View}, `GridWidth` 1–12, `GridHeight` 1–6, `RefreshInterval` ≥ 0; friendly Arabic `asp-validation-for` messages shown.
- On valid POST: add/update via DbContext; `CreatedAt`/`UpdatedAt` left to DB defaults; redirect to Index with a success toast.
- Delete = POST handler `OnPostDeleteAsync(int id)`; EF cascade removes the card's `CardDrillDownLevels`.
- **No secrets** in any file; `SQL_PASSWORD` resolved via `ConnectionStringHelper` env var (unchanged). Plaintext never written.

**Notes / deviation (scope):**
- The write-target rule restricted me to the `Cards/` folder (+ related `.cs` + this task file), so I did **not** modify the global `_Layout.cshtml` or create `wwwroot/css/blue-theme.css`. Instead the Blue-theme CSS + Syncfusion links are inlined into `Cards/_CardsLayout.cshtml`.
  *Recommendation (TeraAgent):* later extract the inlined CSS to `wwwroot/css/blue-theme.css` and call it from the global `_Layout.cshtml` (as `28_UI_UX_GUIDELINES.md §1` intends) so the whole app shares one design system. This is a non-functional refactor and does not affect this task's behaviour.
- The Syncfusion scripts/styles use CDN `27.2.3` (verified reachable). For production, pin this to the exact resolved `Syncfusion.EJ2.AspNet.Core` patch version, or switch to `app.UseSyncfusion()` local serving.

**Status:** DONE — code complete, builds clean. Awaiting TeraAgent / QA-Agent review (allowed-write-targets, no-secrets, scope, acceptance, vitality, handback).
