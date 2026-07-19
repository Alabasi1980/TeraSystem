# TASK-DASH-002: Dashboard Management Service + Admin CRUD Pages

## Overview

**Goal:** Create a `DashboardManageService` for CRUD operations on dashboards, and build admin pages (List, Create, Edit) under `/admin-secure-panel/Dashboards/` so admins can create, edit, reorder, and delete dashboards.

**Status:** Approved
**Assigned To:** engineering-agent-dotnet
**Created:** 2026-07-18

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Service + 3 admin pages (List/Create/Edit) |
| One objective only | PASS | Dashboard CRUD management only |
| No deferrable work included | PASS | Core admin functionality for dashboards |
| No UI unless explicitly requested | PASS | Admin CRUD pages ARE the task |
| No API unless explicitly requested | PASS | No REST API changes |
| No Auth unless explicitly requested | PASS | Admin pages already behind existing auth middleware |
| No schema/migration unless explicitly requested | PASS | Schema done in DASH-001 |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| CLI side effects checked | PASS | No CLI commands needed |
| Allowed Write Targets are narrow | PASS | Service + 6 new files |
| Acceptance criteria are testable | PASS | Build + navigation testable |

**Gate Status:** PASS

---

## Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Services\DashboardManageService.cs` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\Index.cshtml` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\Index.cshtml.cs` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\Create.cshtml` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\Create.cshtml.cs` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\Edit.cshtml` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\Edit.cshtml.cs` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\_ViewStart.cshtml` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Dashboards\_ViewImports.cshtml` (NEW)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Program.cs` (EDIT — register service only)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Index.cshtml` (EDIT — add Dashboards nav card)

**DO NOT touch any other files.**

---

## Implementation Requirements

### 1. Create `Services/DashboardManageService.cs`

A scoped service injected via DI. Methods:

```csharp
public class DashboardManageService
{
    private readonly WarehouseDashboardDbContext _db;

    public DashboardManageService(WarehouseDashboardDbContext db) { _db = db; }

    // Get all dashboards ordered by SortOrder
    public Task<List<Dashboard>> GetAllAsync();

    // Get a single dashboard by ID
    public Task<Dashboard?> GetByIdAsync(int id);

    // Get a dashboard by slug (for public routing)
    public Task<Dashboard?> GetBySlugAsync(string slug);

    // Get the default dashboard (IsDefault == true)
    public Task<Dashboard?> GetDefaultAsync();

    // Create a new dashboard. Auto-sets CreatedAt/UpdatedAt.
    // Validates slug uniqueness (case-insensitive).
    public Task<(bool Success, string? Error)> CreateAsync(Dashboard dashboard);

    // Update an existing dashboard. Auto-sets UpdatedAt.
    // Validates slug uniqueness excluding self.
    // Prevents removing IsDefault if it's the only default.
    public Task<(bool Success, string? Error)> UpdateAsync(Dashboard dashboard);

    // Delete a dashboard. Prevents deleting the default dashboard.
    // Prevents deleting if it has active cards (must move cards first).
    public Task<(bool Success, string? Error)> DeleteAsync(int id);

    // Reorder dashboards by providing ordered list of IDs
    public Task ReorderAsync(List<int> orderedIds);

    // Get dashboard with card count
    public Task<List<(Dashboard Dashboard, int CardCount)>> GetAllWithCardCountAsync();
}
```

### 2. Register in `Program.cs`

Add one line after the existing service registrations (around line 48):
```csharp
builder.Services.AddScoped<DashboardManageService>();
```

### 3. Create Admin Pages

Use the same layout pattern as the existing Cards admin pages. All pages should use `Layout = "_CardsLayout"` (from the Cards folder — reference it properly).

**IMPORTANT:** The `_CardsLayout.cshtml` is at `Pages/admin-secure-panel/Cards/_CardsLayout.cshtml`. For Dashboards pages, create a `_ViewStart.cshtml` that sets `Layout = "../Cards/_CardsLayout"` or copy the pattern.

#### Index.cshtml (Dashboard List)
- Breadcrumb: `لوحة الإدارة > الداشبوردات`
- Page title: `إدارة الداشبوردات`
- Button: `+ داشبورد جديدة`
- Table/grid showing: Name, Slug, Icon, Card Count, IsDefault badge, IsActive badge, SortOrder, Actions (Edit/Delete)
- Delete confirmation dialog (JavaScript confirm)
- Sortable rows (visual indicator)

#### Create.cshtml (Create Dashboard)
- Breadcrumb: `لوحة الإدارة > الداشبوردات > إنشاء`
- Form fields:
  - Name (required, text input)
  - Slug (required, text input, auto-generate from Name on client-side)
  - Description (optional, textarea)
  - Icon (emoji picker or text input, default "📊")
  - SortOrder (number input)
  - IsActive (checkbox, default true)
  - IsDefault (checkbox — warn if another default exists)
- Submit button + Cancel link
- Validation errors display

#### Edit.cshtml (Edit Dashboard)
- Same form as Create but pre-populated
- Show card count for this dashboard
- Warning if changing IsDefault
- Prevent deleting the last default

### 4. Add Nav Card to Admin Index

Edit `Pages/admin-secure-panel/Index.cshtml` to add a new nav card for "إدارة الداشبوردات" in the grid. Place it as the FIRST card (before "إدارة البطاقات"). Use icon `📋` and link to `/admin-secure-panel/Dashboards`.

---

## Acceptance Criteria

1. ✅ `DashboardManageService.cs` created with all methods listed above
2. ✅ Service registered in `Program.cs` as Scoped
3. ✅ `Dashboards/Index.cshtml` — list page shows all dashboards with card counts
4. ✅ `Dashboards/Create.cshtml` — create form with all fields
5. ✅ `Dashboards/Edit.cshtml` — edit form with pre-populated values
6. ✅ Slug auto-generation from Name (client-side JS)
7. ✅ Delete prevents removing default dashboard
8. ✅ Delete prevents removing dashboard with active cards
9. ✅ Admin Index page has new "Dashboards" nav card
10. ✅ `dotnet build` succeeds with 0 errors
11. ✅ All pages use existing design system (blue-theme.css classes)

---

## Design Notes

- Follow the exact same visual patterns as the existing Cards admin pages
- Use `wd-btn`, `wd-card`, `wd-form`, `wd-input`, `wd-select`, `wd-badge` classes from blue-theme.css
- Breadcrumb pattern: same as Cards pages
- Form layout: same 2-column grid as Cards Create/Edit
- Badge for IsDefault: use `wd-badge--on` with text "افتراضية"
- Badge for IsActive: use `wd-badge--on`/`wd-badge--off`

---

## Verification

After implementation:
1. Navigate to `/admin-secure-panel/Dashboards` — should show the default dashboard
2. Create a new dashboard — should appear in the list
3. Edit a dashboard — changes should persist
4. Try to delete the default dashboard — should be prevented
5. Run `dotnet build` — 0 errors
