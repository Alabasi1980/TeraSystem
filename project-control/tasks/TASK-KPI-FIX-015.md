# TASK-KPI-FIX-015 — Cache-Busting for Builder.cshtml Script Tag

## Task Info
| Field | Value |
|---|---|
| **Task ID** | TASK-KPI-FIX-015 |
| **Status** | Approved |
| **Priority** | Medium |
| **Type** | Prevention / Maintenance |
| **Requested By** | Majed |
| **Created** | 2026-07-17 |
| **Related** | AUDIT-KPI-RENDER-2026-07-17-001 |

## Description
Add cache-busting query parameter to the `card-builder.js` script tag in `Builder.cshtml` to prevent browsers from serving stale JavaScript files. This is a preventive fix — the current Step 4 rendering issue is caused by browser cache serving an old version of `card-builder.js`.

## Acceptance Criteria
- [ ] `Builder.cshtml` script tag for `card-builder.js` includes a cache-busting query parameter (e.g., `?v=20260717` or similar version marker)
- [ ] No other functional changes — only the script `src` attribute is modified
- [ ] Build succeeds with 0 errors

## Scope
**ONE file, ONE line change:**
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml` — line containing `<script src="~/js/card-builder.js"></script>`

## Change
**Current:**
```html
<script src="~/js/card-builder.js"></script>
```

**New:**
```html
<script src="~/js/card-builder.js?v=20260717"></script>
```

## Constraints
- Do NOT modify any other files
- Do NOT change any JavaScript logic
- Do NOT change any CSS
- Only modify the script `src` attribute

## Vitality & Polish Checklist
N/A — This is a one-line cache-busting fix, not a UI feature.

## Allowed Write Targets
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
