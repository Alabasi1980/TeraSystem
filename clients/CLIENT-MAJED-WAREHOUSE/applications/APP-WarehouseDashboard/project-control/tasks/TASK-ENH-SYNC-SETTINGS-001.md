# TASK-ENH-SYNC-SETTINGS-001 — Sync Settings Polish

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement

---

## Objective

Polish the Sync Settings page with minor visual improvements.

## File to Modify

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\SyncSettings\Index.cshtml`

## What to Change

### 1. Replace inline style with CSS class
Line 114: `<span style="font-size:14px; color:var(--c-text);">` → add a `.wd-toggle-label` CSS class

### 2. Replace Toast icons with inline SVG
Lines 139: `✓` / `✕` → SVG checkmark / X icons (currentColor, consistent with other pages)

### 3. Minor visual polish
- Add small inline SVG icons next to each form field label (clock for interval, toggle for auto-sync, history for last sync)
- Slightly improve card padding
- Ensure spacing is consistent

### 4. Use CSS variables consistently
- Replace any hardcoded values with CSS variables

## What NOT to change
- Page structure, breadcrumb, form fields (IntervalMinutes, IsAutoSyncEnabled, LastSyncTimestamp)
- Form submission logic
- Toggle behavior
- Responsive layout
- Any C#/Razor code

## Build
```powershell
dotnet build --configuration Release
```
From: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

Return final file and build result.
