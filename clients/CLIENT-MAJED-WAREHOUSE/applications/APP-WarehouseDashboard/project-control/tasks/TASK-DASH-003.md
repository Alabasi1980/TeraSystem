# TASK-DASH-003: Slug-Based Routing + Dashboard Tab Bar + DashboardService Integration

## Overview

**Goal:** Add slug-based routing to the public dashboard page (`/{slug?}`), display a tab bar for switching between dashboards, and update `DashboardService` to filter cards by dashboard.

**Status:** Approved
**Assigned To:** engineering-agent-dotnet
**Created:** 2026-07-18

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Routing + tabs + service filter — tightly coupled |
| One objective only | PASS | Single goal: make dashboards navigable |
| No deferrable work included | PASS | Core multi-dashboard navigation |
| No UI unless explicitly requested | PASS | Tab bar IS the task |
| No API unless explicitly requested | PASS | No new API endpoints |
| No Auth unless explicitly requested | PASS | Public pages only |
| No schema/migration unless explicitly requested | PASS | Schema done in DASH-001 |
| No real secrets | PASS | No secrets involved |
| CLI side effects checked | PASS | No CLI commands |
| Allowed Write Targets are narrow | PASS | 3 files edited |
| Acceptance criteria are testable | PASS | Navigation + tab switching testable |

**Gate Status:** PASS

---

## Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml` (EDIT)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml.cs` (EDIT)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\_DashboardLayout.cshtml` (EDIT)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs` (EDIT)

**DO NOT touch any other files.**

---

## Implementation Requirements

### 1. Edit `Pages/Index.cshtml.cs` — Add Slug Parameter

Change the `@page` directive approach. The page model needs:

```csharp
[BindProperty(SupportsGet = true)]
public string? Slug { get; set; }

public Dashboard? CurrentDashboard { get; set; }
public List<Dashboard> AllDashboards { get; set; } = new();
```

In `OnGetAsync()`:
1. Load all active dashboards ordered by SortOrder (for the tab bar)
2. If `Slug` is provided, find the dashboard by slug
3. If `Slug` is null/empty, find the default dashboard (IsDefault == true)
4. If no dashboard found (invalid slug), redirect to `/` (default)
5. Load cards filtered by `DashboardId = CurrentDashboard.Id`
6. Keep existing `CardsWithDrill` logic

**Important:** Inject `DashboardManageService` (or use DbContext directly) to look up dashboards.

### 2. Edit `Pages/Index.cshtml` — Add Tab Bar + Dynamic Title

#### Tab Bar
Add a tab bar BELOW the topbar and ABOVE the filter bar. Structure:

```html
<nav class="wd-tab-bar" aria-label="التنقل بين الداشبوردات">
    @foreach (var dash in Model.AllDashboards)
    {
        var isActive = dash.Id == Model.CurrentDashboard?.Id;
        <a href="/@(string.IsNullOrEmpty(dash.Slug) ? "" : dash.Slug)"
           class="wd-tab @(isActive ? "wd-tab--active" : "")"
           aria-current="@(isActive ? "page" : "false")">
            <span class="wd-tab__icon">@dash.Icon</span>
            <span class="wd-tab__label">@dash.Name</span>
        </a>
    }
</nav>
```

#### Dynamic Title
Change the page title from hardcoded "لوحة المعلومات" to:
```html
<h2 class="wd-page-title">@Model.CurrentDashboard?.Name</h2>
```

### 3. Edit `Pages/_DashboardLayout.cshtml` — Add Tab Bar CSS

Add the tab bar styles INSIDE the `<style>` section or as a separate `<style>` block. The tab bar should be:

```css
/* Dashboard Tab Bar */
.wd-tab-bar {
    display: flex;
    align-items: center;
    gap: var(--sp-1);
    padding: var(--sp-2) var(--sp-6);
    background: var(--c-surface);
    border-bottom: 1px solid var(--c-border);
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
    scrollbar-width: none;
}
.wd-tab-bar::-webkit-scrollbar { display: none; }

.wd-tab {
    display: inline-flex;
    align-items: center;
    gap: var(--sp-2);
    padding: var(--sp-2) var(--sp-4);
    border-radius: var(--radius-md);
    font-family: var(--font-ar);
    font-size: 14px;
    font-weight: 600;
    color: var(--c-text-muted);
    text-decoration: none;
    white-space: nowrap;
    transition: all var(--dur-fast) var(--ease);
    border: 1px solid transparent;
}
.wd-tab:hover {
    color: var(--c-primary);
    background: var(--c-surface-muted);
}
.wd-tab--active {
    color: var(--c-primary);
    background: rgba(31, 78, 121, 0.08);
    border-color: rgba(31, 78, 121, 0.2);
}
.wd-tab__icon {
    font-size: 16px;
    line-height: 1;
}
```

Also add responsive rules for mobile (single row, scrollable).

### 4. Edit `Pages/DashboardService.cs` — Filter by DashboardId

The existing `GetActiveCardsAsync()` method should optionally accept a `dashboardId` parameter:

```csharp
public Task<List<DashboardCard>> GetActiveCardsAsync(int? dashboardId = null, CancellationToken ct = default)
{
    var query = _db.DashboardCards.Where(c => c.IsActive);
    
    if (dashboardId.HasValue)
        query = query.Where(c => c.DashboardId == dashboardId.Value);
    
    return query
        .OrderBy(c => c.GridPositionY)
        .ThenBy(c => c.GridPositionX)
        .ToListAsync(ct);
}
```

This is backward-compatible — existing calls without dashboardId still work.

---

## Acceptance Criteria

1. ✅ `Index.cshtml.cs` accepts `Slug` parameter and loads correct dashboard
2. ✅ Default dashboard loads when no slug provided
3. ✅ Invalid slug redirects to default dashboard
4. ✅ Tab bar displays all active dashboards with icons and names
5. ✅ Active tab is highlighted with `wd-tab--active` class
6. ✅ Clicking a tab navigates to `/{slug}` (or `/` for default)
7. ✅ Page title shows current dashboard name
8. ✅ Cards are filtered by DashboardId
9. ✅ `DashboardService.GetActiveCardsAsync` accepts optional dashboardId
10. ✅ Tab bar is responsive (scrollable on mobile)
11. ✅ `dotnet build` succeeds with 0 errors

---

## Verification

1. Navigate to `/` — should show default dashboard with its cards
2. Navigate to `/warehouses` (if such dashboard exists) — should show only that dashboard's cards
3. Tab bar should show all active dashboards
4. Clicking tabs should navigate correctly
5. Invalid slug should redirect to default
6. `dotnet build` — 0 errors
