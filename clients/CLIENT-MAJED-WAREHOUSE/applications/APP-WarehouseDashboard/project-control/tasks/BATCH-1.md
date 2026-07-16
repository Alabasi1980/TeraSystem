# BATCH-1 — Quick Wins CSS Polish

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement Batch

---

## Objective

Implement 4 quick-win improvements across the app.

## Files to Modify

### Shared Files:
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\_DashboardLayout.cshtml`

### Individual Pages (for empty states):
- `Pages\admin-secure-panel\TableMappings\Index.cshtml` (empty state illustration)
- `Pages\admin-secure-panel\Sync\Index.cshtml` (empty state illustration)

---

## Task 1: Page Transitions

Add a fade+slide-up animation to page content transitions.

**In `blue-theme.css`**, add to `.wd-content`:
```css
.wd-content {
    animation: wdPageEnter 0.35s ease both;
}

@keyframes wdPageEnter {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}
```

Also add a similar transition to `.wd-page` class (used in admin pages):
```css
.wd-page {
    animation: wdPageEnter 0.35s ease both;
}
```

## Task 2: Floating Scroll-to-Top Button

Add a subtle floating button that appears after scrolling down 300px.

**Content changes to `_DashboardLayout.cshtml`**:
- Add the button HTML before `</body>`:
```html
<button id="wd-scroll-top" class="wd-scroll-top" aria-label="العودة للأعلى" title="العودة للأعلى">
    <svg><!-- chevron-up icon --></svg>
</button>
```

**CSS additions to `blue-theme.css`**:
```css
.wd-scroll-top {
    position: fixed; bottom: 24px; inset-inline-end: 24px;
    width: 44px; height: 44px; border-radius: var(--radius-full);
    background: var(--c-surface); border: 1px solid var(--c-border);
    box-shadow: var(--shadow-lg); cursor: pointer;
    display: flex; align-items: center; justify-content: center;
    opacity: 0; transform: translateY(16px);
    transition: opacity var(--dur-norm) var(--ease), transform var(--dur-norm) var(--ease), box-shadow var(--dur-norm) var(--ease);
    z-index: 999; pointer-events: none;
}
.wd-scroll-top.is-visible {
    opacity: 1; transform: translateY(0); pointer-events: auto;
}
.wd-scroll-top:hover {
    box-shadow: 0 8px 24px rgba(10,37,64,0.2); transform: translateY(-2px);
}
.wd-scroll-top svg {
    width: 20px; height: 20px; color: var(--c-primary);
}
```

**JS addition to `_DashboardLayout.cshtml`** (before `</body>`):
```html
<script>
    (function() {
        var btn = document.getElementById('wd-scroll-top');
        if (!btn) return;
        var handler = function() {
            btn.classList.toggle('is-visible', window.scrollY > 300);
        };
        window.addEventListener('scroll', handler, { passive: true });
        btn.addEventListener('click', function() {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    })();
</script>
```

## Task 3: Enhanced Shimmer Loading

Refine the skeleton shimmer in `blue-theme.css` to be more subtle and premium.

**Update the existing shimmer keyframes:**
- Make shimmer smoother and slower
- Add a subtle gradient tint
- Slightly round the animation

Current (update in `blue-theme.css` around line 228-236):
```css
@keyframes wdShimmer {
    0% { background-position: 200% 0; }
    100% { background-position: -200% 0; }
}
.wd-skel {
    background: linear-gradient(90deg, var(--c-surface-muted) 25%, rgba(143,188,222,0.08) 50%, var(--c-surface-muted) 75%);
    background-size: 200% 100%;
    animation: wdShimmer 1.6s ease-in-out infinite;
}
```

## Task 4: Empty State SVG Illustrations

Create 3 simple inline SVG illustrations for empty states (not just icons - simple mini illustrations):

**a)** For **Cards/Table Mappings empty state** (no mappings yet) → A simple mini illustration of a database/server with a plus sign
**b)** For **Sync Dashboard empty state** (no sync mappings active) → A simple mini illustration of a sync/refresh symbol
**c)** For **Dashboard empty state** (no cards yet) → A simple mini illustration of a dashboard layout

These should be:
- Inline SVGs in the respective .cshtml files
- ~80-100px size
- Use soft blue tones from the palette
- Simple line-art style (2-3 colors max, using currentColor and soft fills)
- Replace existing `.wd-empty__icon` content

## Acceptance Criteria

- [ ] Pages have smooth entrance animation
- [ ] Scroll-to-top button appears after scrolling and works smoothly
- [ ] Shimmer loading looks more refined and premium
- [ ] Empty states have proper illustrations (not just icons)
- [ ] All changes pass build
- [ ] No regressions on existing functionality

## Build Command

```powershell
dotnet build --configuration Release
```
From: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

Return build result.
