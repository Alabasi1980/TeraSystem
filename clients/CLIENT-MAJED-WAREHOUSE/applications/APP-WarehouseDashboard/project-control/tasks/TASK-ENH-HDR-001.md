# TASK-ENH-HDR-001 — Header Topbar Redesign

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement
> **Scope:** Visual-only (HTML + CSS)

---

## Objective

Redesign the topbar/header across both layouts to be more professional, fix usability issues, and add logo image support.

## Current State

### Files Affected:
1. **`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css`** — Shared CSS (lines 63-91: topbar styles)
2. **`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\_DashboardLayout.cshtml`** — Public dashboard layout (lines 25-39: header HTML)
3. **`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\_CardsLayout.cshtml`** — Admin panel layout (lines 311-319: header HTML + lines 75-103: inline topbar CSS)

## Requirements

### 1. Logo Link to Homepage
- Wrap the entire `.wd-topbar__brand` content in an `<a href="/">` tag
- Must work on both layouts (Dashboard + Admin)
- Style the link to inherit the brand text color (white) and remove default link styling

### 2. Logout Button Fix
- The current `.wd-btn--ghost` on dark background causes text color clash (blue text on blue background)
- Fix: Use a dedicated `.wd-btn--on-dark` style variant (already exists in blue-theme.css line 151-152) for the logout button in the admin layout
- Ensure text is clearly visible (white text with subtle border)

### 3. Professional Header Redesign
- Add support for a logo image (`<img>`) next to the app name
- Logo image dimensions: height ~36-40px, width auto (aspect-ratio preserved)
- Logo should be stored in `wwwroot/images/` (create the folder if needed)
- Add a placeholder/fallback: if no logo image exists, show the existing `.wd-logo-dot` as fallback
- Apply subtle visual enhancement to the header:
  - Slight gradient or subtle shadow for depth
  - Better vertical alignment
  - Smooth transition effects on hover for interactive elements
  - Consider a slightly taller header (68-72px) to accommodate the logo properly
- Must be responsive (logo scales down on mobile)

### 4. Consistency
- Both layouts (Dashboard + Admin) must share the same header visual design
- The admin layout currently has inline styles — consolidate topbar CSS into blue-theme.css where possible
- The admin layout's inline topbar styles should match blue-theme.css exactly

## Acceptance Criteria

- [ ] Logo/brand area is wrapped in `<a href="/">` on both layouts
- [ ] Logout button text is clearly readable (white on dark blue)
- [ ] Header supports an `<img>` logo with proper sizing (36-40px height)
- [ ] Fallback to `.wd-logo-dot` when no image is provided
- [ ] Header looks professional and polished (not just a flat colored bar)
- [ ] Responsive on mobile (logo scales, layout adapts)
- [ ] No CSS regressions on other components
- [ ] Build passes with no errors

## Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WashboardDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\_DashboardLayout.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\_CardsLayout.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\images\` (new folder for logo placeholder)

## Notes

- Design system tokens are in `blue-theme.css` `:root` variables — use them
- The `.wd-btn--on-dark` class already exists and works well on dark backgrounds
- Keep the existing BEM naming convention (`.wd-topbar__brand`, `.wd-topbar__actions`, etc.)
- The header should feel modern and clean — think along the lines of Vercel/Linear dashboard headers
