# TASK-ENH-SYNC-LOGS-001 — Sync Logs Polish

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement (emoji → SVG)

---

## Objective

Replace remaining emoji in Sync Logs page with SVG icon.

## File to Modify

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\SyncLogs\Index.cshtml`

## What to Change

**Line ~63:** `&#128203;` (📋 emoji) in the empty state → Replace with a clipboard/list SVG icon

The empty state currently:
```html
<div id="emptyState" class="wd-empty" hidden>
    <span class="wd-empty__icon">&#128203;</span>
    <h3>لا توجد سجلات مزامنة بعد</h3>
    <p>ستظهر هنا سجلات الدورات بعد تنفيذ أول مزامنة.</p>
</div>
```

Replace the emoji with an inline SVG document/list icon (~32-34px) using currentColor.

## What NOT to change
- Page structure, grid, refresh logic
- Any C#/Razor code
- Skeleton loading

## Build
```powershell
dotnet build --configuration Release
```
From: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

Return final file and build result.
