# TASK-ENH-DASH-001 — Public Dashboard Polish

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement (Icon replacement + polish)

---

## Objective

Polish the Public Dashboard page by replacing emoji icons with SVG icons and minor CSS adjustments.

## File to Modify

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

## Current Emoji Locations (4 spots)

1. **Line 147** — Config error: `<div class="wd-empty__icon">⚠</div>`
2. **Line 155** — No cards: `<div class="wd-empty__icon">▦</div>`
3. **Line 164** — Filter search icon: `<span class="wd-filterbar__icon" aria-hidden="true">🔍</span>`
4. **Line 216** — Filter empty state: `<div class="wd-filter-empty__icon" aria-hidden="true">🔍</div>`

## Requirements

### 1. Replace ALL 4 emoji with inline SVG icons
- ⚠ → Warning/exclamation triangle SVG
- ▦ → Grid/layout SVG
- 🔍 → Magnifying glass/search SVG (appears twice — reuse the same SVG)

### 2. CSS Adjustments
- Update `.wd-filterbar__icon` from `font-size: 15px` to use SVG-friendly sizing (width/height or font-size for the container)
- SVGs should use `currentColor` and be ~18-20px for the filter icon, ~32-34px for empty state icons
- Keep the same `aria-hidden="true"` approach

### 3. What NOT to change
- Page structure, layout, breadcrumb, title
- Card rendering, JS logic
- Filter behavior
- Responsive breakpoints
- Any existing functionality

## Design Tokens
```css
--c-primary: #1F4E79;
--c-text-muted: #5B7A99;
--c-surface-muted: #EAF1F8;
--c-border: #D4E2F0;
```

## Build
```powershell
dotnet build --configuration Release
```
From: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

Return final file and build result.
