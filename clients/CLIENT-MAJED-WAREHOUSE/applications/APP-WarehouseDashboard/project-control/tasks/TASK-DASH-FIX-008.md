# TASK-DASH-FIX-008 — Fix Dashboard Card Layout Overlap

## Status
Implemented / Build Verified / Awaiting User Runtime Test

## Problem
Cards overlap when resized because they use explicit `grid-column-start` / `grid-row-start` inline styles that override CSS Grid auto-placement. When a card is resized (S/M/L), its span classes change but the hardcoded position stays fixed, causing overlap.

## Root Cause
In `Index.cshtml` line 427:
```html
style="grid-column-start: @colStart; grid-row-start: @rowStart; height: @(cardHeight)px; ..."
```
This forces each card to a fixed grid cell regardless of its size. CSS Grid's `grid-auto-flow: row dense` is ignored because explicit positions override it.

## Solution
Remove `grid-column-start` and `grid-row-start` from the card inline style. Let CSS Grid auto-flow handle placement using only `wd-span-N` (width) and `wd-row-M` (height) classes. The grid already has `grid-auto-flow: row dense` which fills gaps automatically.

## Exact Changes Required

### File: `Index.cshtml`
Full path: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

**Line 427** — Change:
```html
style="grid-column-start: @colStart; grid-row-start: @rowStart; height: @(cardHeight)px; animation-delay: @(i * 70)ms;@(canDrill ? " cursor:pointer;" : "")"
```
To:
```html
style="height: @(cardHeight)px; animation-delay: @(i * 70)ms;@(canDrill ? " cursor:pointer;" : "")"
```

That's the only change needed. CSS Grid auto-flow takes over.

### Why this works:
- `wd-span-N` classes set `grid-column-end: span N` → width
- `wd-row-M` classes set `grid-row-end: span M` → height
- `grid-auto-flow: row dense` on `.wd-dashboard-grid` → auto-places cards, filling gaps
- When resize button changes span classes, CSS Grid re-flows automatically
- When SortableJS reorders DOM, CSS Grid re-flows by DOM order
- No overlapping because CSS Grid manages all positioning

## Acceptance Criteria
- Cards do not overlap after resizing (S/M/L)
- Cards auto-arrange in a clean grid layout
- Drag-and-drop reorder still works
- Layout save still works (sends sequential Y order + width/height)
- `dotnet build --no-restore` passes with 0 errors

## Allowed Files (absolute paths)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

## Notes
- Before editing, read the current file from disk first. Preserve unrelated changes.
- No model/schema/CSS changes needed — only the Razor template inline style.
- The `colStart` and `rowStart` variables become unused after this change — they can be left as-is or removed as cleanup.

## Handback
- Actor: engineering-agent-dotnet
- Files changed: `Index.cshtml` (1 line edit)
- Edit: Removed `grid-column-start: @colStart; grid-row-start: @rowStart;` from card inline style on line 427
- Build: `dotnet build --no-restore` — 0 errors, 0 warnings
- No schema/CSS/JS changes needed

## Post-Execution Review
- Actual changed files reviewed: PASS
- Allowed Write Targets respected: PASS
- No secrets introduced: PASS
- Scope respected: PASS
- Build verification: PASS
- Runtime browser verification: Pending user test
- Auditor Review Decision: NOT_REQUIRED
- Reason: Single inline style attribute removal; CSS Grid auto-flow takes over.
