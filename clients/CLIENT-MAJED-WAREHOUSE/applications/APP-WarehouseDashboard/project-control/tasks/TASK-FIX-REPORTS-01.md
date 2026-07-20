# TASK-FIX-REPORTS-01: Layout Integration + CSS Cleanup

**Status:** Assigned  
**Priority:** P0 — Critical  
**Type:** UI Fix  
**Created:** 2026-07-20  
**Audit Reference:** QUAUD-REPORT-DESIGN-QUALITY-2026-07-20-001 (STOP-001, STOP-002, CAU-001, CAU-002, CAU-003, FLAG-001)

---

## Objective

Fix the Reports System pages so they are **visually integrated** with the rest of the admin panel, using the shared design system (`_CardsLayout` + `blue-theme.css`) instead of standalone hardcoded CSS.

## Scope — What to Fix

### 1. STOP-001: Remove Layout Override (Both Pages)

**Reports/Index.cshtml** (line 5):
```
Current:  Layout = "_Layout";
Fix:      DELETE this line entirely.
```

**ReportBuilder/Index.cshtml** (line 5):
```
Current:  Layout = "_Layout";
Fix:      DELETE this line entirely.
```

**Why:** `admin-secure-panel/_ViewStart.cshtml` already sets `Layout = "/Pages/admin-secure-panel/Cards/_CardsLayout.cshtml"` for ALL admin pages. By removing the explicit override, both pages will automatically get the rich admin layout (topbar, theme switcher, sync status, keyboard shortcuts, blue-theme.css integration).

**After removing Layout override, the pages will render inside `_CardsLayout` which provides:**
- `<body class="wd-app">` — body with proper styling
- `_Header` partial — topbar with brand, connection status, sync status, theme switcher, logout
- `<main class="wd-content">` — content wrapper with padding and max-width
- `#wd-toast-host` — toast notification container
- Keyboard shortcuts HUD
- All CSS from `blue-theme.css` (design tokens, buttons, cards, skeleton, empty states, toasts, animations)

### 2. STOP-002 + CAU-001 + CAU-002: Remove ALL Duplicated CSS

After removing the Layout override, the pages will inherit these classes from `_CardsLayout`/`blue-theme.css`:
- `.wd-card`, `.wd-card__title` — card container
- `.wd-btn`, `.wd-btn--primary`, `.wd-btn--ghost`, `.wd-btn--danger`, `.wd-btn--success` — buttons
- `.wd-input`, `.wd-select`, `.wd-textarea` — form controls
- `.wd-field`, `.wd-field label` — form fields
- `.wd-form`, `.wd-form__actions` — form layout
- `.wd-empty`, `.wd-empty__icon`, `.wd-empty h3`, `.wd-empty p` — empty states
- `.wd-skeleton-wrap`, `.wd-skel-row`, `.wd-skel` — skeleton loading
- `.wd-toast-host`, `.wd-toast`, `.wd-toast--success`, `.wd-toast--error` — toasts
- `.wd-hidden` — utility
- `@keyframes wdFadeUp`, `wdToastIn`, `wdShimmer` — animations
- All design tokens: `--c-primary`, `--c-secondary`, `--c-border`, `--sp-*`, `--radius-*`, `--shadow-*`, `--dur-*`, `--font-ar`

**Remove from Reports/Index.cshtml (lines 11-322):**
- Remove the `* { box-sizing: border-box; }` reset (line 13) — already in blue-theme.css
- Remove `:root` variables (lines 15-18) — use design tokens instead
- Remove `.wd-page` styles (lines 21-27) — keep only the unique parts, use tokens
- Remove `.wd-breadcrumb` styles (lines 29-31) — keep, but use tokens
- Remove `.wd-sidebar` styles (lines 41-105) — keep, but replace hardcoded colors with tokens
- Remove `.wd-report-info` styles (lines 114-128) — keep, but use tokens
- Remove `.wd-filter-bar` styles (lines 131-172) — keep, but use tokens
- Remove `.wd-table-area` styles (lines 175-216) — keep, but use tokens
- Remove `.wd-empty-state` styles (lines 224-233) — REPLACE with `.wd-empty` from blue-theme.css
- Remove `.wd-modal-overlay`, `.wd-modal` styles (lines 236-286) — keep modal-specific styles, use tokens
- Remove `.wd-skeleton`, `.wd-skel-row` styles (lines 289-295) — REPLACE with `.wd-skeleton-wrap`, `.wd-skel-row`, `.wd-skel` from blue-theme.css
- Remove ALL `@keyframes` (lines 297-312) — already in blue-theme.css
- Remove hardcoded colors: `#2E6DA4` → `var(--c-secondary)`, `#1F4E79` → `var(--c-primary)`, `#F7FAFD` → `var(--c-surface-muted)`, `#FAFCFE` → `var(--c-surface-muted)`, `#F0F4F8` → `var(--c-surface-muted)`, `#EDF2F7` → `var(--c-surface-muted)`

**Remove from ReportBuilder/Index.cshtml (lines 8-148):**
- Remove `* { box-sizing: border-box; }` reset (line 10)
- Remove `:root` variables (lines 12-16) — use design tokens
- Remove `.wd-page` styles (lines 18-20) — keep unique parts
- Remove `.wd-breadcrumb` styles (lines 22-23) — keep, use tokens
- Remove `.wd-card` styles (lines 49-56) — ALREADY EXISTS in blue-theme.css
- Remove `.wd-form`, `.wd-field` styles (lines 59-76) — ALREADY EXISTS in _CardsLayout
- Remove `.wd-btn` styles (lines 78-96) — ALREADY EXISTS in blue-theme.css
- Remove `.wd-input`, `.wd-select` styles (lines 63-73) — ALREADY EXISTS in _CardsLayout
- Remove `.wd-hidden` (line 136) — ALREADY EXISTS in blue-theme.css
- Remove `@keyframes wdFadeUp` (lines 139-142) — ALREADY EXISTS in blue-theme.css
- Remove hardcoded colors: `#2E6DA4` → `var(--c-secondary)`, `#1F4E79` → `var(--c-primary)`, `#309E6A` → `var(--c-success)`, `#D64545` → `var(--c-error)`

### 3. CAU-003: Unify Shimmer Animation

**Reports/Index.cshtml** defines shimmer as:
```css
.wd-skel-row {
    background: linear-gradient(90deg, var(--c-surface-muted) 25%, #dbe7f3 50%, var(--c-surface-muted) 75%);
    background-size: 400% 100%;
    animation: wdShimmer 1.2s ease-in-out infinite;
}
```

**blue-theme.css** defines it as:
```css
.wd-skel {
    background: linear-gradient(90deg, var(--c-surface-muted) 25%, rgba(143,188,222,0.18) 50%, var(--c-surface-muted) 75%);
    background-size: 200% 100%;
    animation: wdShimmer 1.5s ease-in-out infinite;
}
```

**Fix:** Remove the Reports-specific shimmer. Use the blue-theme.css classes:
- Change `.wd-skel-row` containers to use class `wd-skeleton-wrap`
- Change inner shimmer elements to use class `wd-skel` (not `.wd-skel-row` which is the grid container)

### 4. FLAG-001: Fix RTL Arrow Direction

**ReportBuilder/Index.cshtml** uses LTR arrows for "Previous" button:
- Line 227: `→ السابق` — should be `السابق ←` (or use RTL-appropriate arrow)
- Line 245: `→ السابق` — same fix
- Line 261: `→ السابق` — same fix

**Fix:** Change `→ السابق` to `السابق ←` in all three locations.

## What NOT to Change

- Do NOT modify the `<script>` sections (JavaScript) — that's TASK-FIX-REPORTS-02
- Do NOT modify the API files (`ReportManage.cshtml.cs`, `ReportData.cshtml.cs`) — that's TASK-FIX-REPORTS-03
- Do NOT modify `_CardsLayout.cshtml` or `blue-theme.css`
- Do NOT change the page functionality — only CSS/layout integration
- Do NOT touch any `.cs` code-behind files for these pages

## Files to Modify

1. `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/Reports/Index.cshtml`
2. `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/ReportBuilder/Index.cshtml`

## Design Token Reference

Replace hardcoded values with these tokens:

| Hardcoded | Token |
|---|---|
| `#2E6DA4` | `var(--c-secondary)` |
| `#1F4E79` | `var(--c-primary)` |
| `#163A5A` | `var(--c-primary-strong)` |
| `#F7FAFD` | `var(--c-surface-muted)` |
| `#FAFCFE` | `var(--c-surface-muted)` |
| `#F0F4F8` | `var(--c-surface-muted)` |
| `#EDF2F7` | `var(--c-surface-muted)` |
| `#309E6A` | `var(--c-success)` |
| `#248A5A` | `var(--c-success)` (darker shade → use `color-mix` or keep as-is for hover) |
| `#D64545` | `var(--c-error)` |
| `12px` (border-radius) | `var(--radius-lg)` |
| `8px` (border-radius) | `var(--radius-md)` |
| `6px` (border-radius) | `var(--radius-md)` or `var(--radius-sm)` |
| `24px` (padding) | `var(--sp-6)` |
| `16px` (padding/margin) | `var(--sp-4)` |
| `12px` (gap/padding) | `var(--sp-3)` |
| `8px` (gap) | `var(--sp-2)` |
| `6px` (gap) | `var(--sp-2)` |
| `4px` (gap) | `var(--sp-1)` |
| `0 2px 8px rgba(0,0,0,0.05)` | `var(--shadow-sm)` |
| `0 2px 8px rgba(0,0,0,0.06)` | `var(--shadow-sm)` |
| `0 20px 60px rgba(0,0,0,0.3)` | `var(--shadow-xl)` |
| `font-family: 'Cairo', sans-serif` | `font-family: var(--font-ar)` |

## Acceptance Criteria

- [ ] Both pages render with the full admin topbar (brand, connection status, sync status, theme switcher, logout)
- [ ] No `Layout = "_Layout"` in either page
- [ ] Zero hardcoded hex colors (`#2E6DA4`, `#1F4E79`, etc.) — all replaced with design tokens
- [ ] Zero duplicated CSS classes that exist in blue-theme.css or _CardsLayout
- [ ] Theme switching works (Blue/Emerald/Midnight all look correct)
- [ ] Skeleton loading uses shared shimmer animation
- [ ] RTL arrows are correct in ReportBuilder wizard
- [ ] Page functionality is preserved (all JS still works)
- [ ] Visual consistency with Cards pages

## Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Reports\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\ReportBuilder\Index.cshtml`

## Allowed Read Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-Washboard.Web\Pages\admin-secure-panel\Cards\_CardsLayout.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\_ViewStart.cshtml`

---

**Before editing any existing file, read the current file from disk first. Preserve unrelated changes, including changes made by another Tera session or sub-agent. Do not overwrite, revert, or delete unrelated changes based on memory or an older snapshot.**
