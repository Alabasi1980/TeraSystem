# TASK-DRILL-MODAL-FIX-001

> **Status:** In Progress  
> **Assigned To:** UI Designer  
> **Created:** 2026-07-20  
> **Priority:** High  
> **Phase:** E (Level Renderers & Export) — Bug Fix

---

## 1. Objective

Fix the drill-down modal CSS issues reported by the user:
1. Modal sticks to the top instead of being centered
2. Modal is not fully visible
3. z-index may be too low (conflicting with other elements)

---

## 2. Current State

### CSS in `blue-theme.css` (lines 680-726):
```css
.wd-modal {
    position: fixed; inset: 0; z-index: 3000;
    display: flex; align-items: center; justify-content: center;
    padding: var(--sp-4);
}
.wd-modal[hidden] { display: none !important; }
.wd-modal__overlay {
    position: absolute; inset: 0;
    background: rgba(10,37,64,0.6);
    backdrop-filter: blur(4px);
    animation: wdFadeIn 0.2s ease both;
}
.wd-modal__panel {
    position: relative;
    background: var(--c-surface);
    border: 1px solid var(--c-border);
    border-radius: var(--radius-lg);
    box-shadow: var(--shadow-xl);
    width: min(900px, 90vw);
    max-height: 85vh;
    display: flex; flex-direction: column;
    animation: wdSlideUp 0.3s ease both;
}
```

### HTML in `Index.cshtml` (lines 1096-1111):
```html
<div id="wd-drill-modal" class="wd-modal" hidden aria-modal="true" role="dialog">
    <div class="wd-modal__overlay" tabindex="-1"></div>
    <div class="wd-modal__panel">
        <div class="wd-modal__header">
            <h3 class="wd-modal__title" id="wd-drill-modal-title"></h3>
            <button class="wd-modal__close" aria-label="إغلاق" type="button">&times;</button>
        </div>
        <div class="wd-modal__breadcrumb" id="wd-drill-modal-breadcrumb"></div>
        <div class="wd-modal__body" id="wd-drill-modal-body">
            <div class="wd-skeleton-wrap" aria-hidden="true">
                <div class="wd-skel wd-skel--tall"></div>
            </div>
        </div>
        <div class="wd-modal__footer" id="wd-drill-modal-footer"></div>
    </div>
</div>
```

---

## 3. Issues to Fix

### Issue 1: Modal Sticks to Top
**Symptom:** Modal appears at the top of the screen instead of being vertically centered.

**Possible Causes:**
- `align-items: center` not working due to missing height on parent
- `min-height` not set on `.wd-modal`
- Flexbox context issue

**Fix:**
```css
.wd-modal {
    position: fixed; inset: 0; z-index: 3000;
    display: flex; align-items: center; justify-content: center;
    padding: var(--sp-4);
    min-height: 100vh; /* Ensure full viewport height */
}
```

### Issue 2: Modal Not Fully Visible
**Symptom:** Modal content is cut off or not showing completely.

**Possible Causes:**
- `max-height: 85vh` too restrictive
- Overflow hidden on body or parent
- z-index conflict with other elements

**Fix:**
```css
.wd-modal__panel {
    position: relative;
    background: var(--c-surface);
    border: 1px solid var(--c-border);
    border-radius: var(--radius-lg);
    box-shadow: var(--shadow-xl);
    width: min(900px, 90vw);
    max-height: 85vh;
    display: flex; flex-direction: column;
    animation: wdSlideUp 0.3s ease both;
    z-index: 3001; /* Ensure panel is above overlay */
}
```

### Issue 3: z-index Conflict
**Symptom:** Other elements appear above the modal.

**Possible Causes:**
- z-index 3000 may be too low
- Other elements have higher z-index
- Stacking context issues

**Fix:**
```css
.wd-modal {
    z-index: 9999; /* Increase to ensure it's above everything */
}
```

---

## 4. Files to Modify

| File | Changes |
|---|---|
| `src/WarehouseDashboard.Web/wwwroot/css/blue-theme.css` | Update `.wd-modal` and `.wd-modal__panel` CSS |

---

## 5. Acceptance Criteria

- [ ] Modal is vertically and horizontally centered on screen
- [ ] Modal is fully visible without being cut off
- [ ] Modal appears above all other elements (z-index working)
- [ ] Modal overlay covers the entire viewport
- [ ] Close button works (X button and overlay click)
- [ ] Escape key closes the modal
- [ ] Modal body is scrollable if content exceeds viewport
- [ ] Mobile responsive (modal adapts to smaller screens)

---

## 6. Testing Steps

1. Open the dashboard
2. Click "تفاصيل" button on any card with Drill configured
3. Verify modal appears centered on screen
4. Verify modal is fully visible (not cut off)
5. Verify overlay covers entire viewport
6. Click close button (X) — modal should close
7. Open modal again, click overlay — modal should close
8. Open modal again, press Escape — modal should close
9. Open modal, resize browser window — modal should remain centered
10. Test on mobile viewport — modal should adapt

---

## 7. Notes

- The user reported this issue with a screenshot showing the modal stuck at the top
- The current CSS looks correct syntactically, so the issue may be a browser rendering quirk or conflict with other styles
- Test in both Chrome and Firefox to ensure cross-browser compatibility

---

> **Prepared by:** TeraAgent  
> **Mode:** Plan Mode — No code written in this document
