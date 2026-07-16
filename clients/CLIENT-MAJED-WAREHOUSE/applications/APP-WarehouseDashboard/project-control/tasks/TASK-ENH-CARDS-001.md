# TASK-ENH-CARDS-001 — Cards List Polish

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement (emoji → SVG)

---

## Objective

Replace remaining emoji in Cards List page with SVG icon.

## File to Modify

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Index.cshtml`

## What to Change

**Line ~19:** `▦` emoji in the "لا توجد بطاقات بعد" empty state → Replace with a grid/layout SVG icon

Current empty state (approximately):
```html
<div class="wd-card wd-empty">
    <div class="wd-empty__icon">▦</div>
    <h3>لا توجد بطاقات بعد</h3>
    ...
```

Replace `▦` with an inline SVG grid/layout icon (~32-34px) using currentColor.

## Build
```powershell
dotnet build --configuration Release
```
From: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

Return final file and build result.
