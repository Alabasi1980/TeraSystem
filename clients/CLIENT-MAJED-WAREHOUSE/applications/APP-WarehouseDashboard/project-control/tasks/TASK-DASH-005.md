# TASK-DASH-005: Dashboard Layout Customization (Drag & Drop + Resize + Per-card Refresh)

> **Status:** Approved  
> **Assigned:** engineering-agent  
> **Priority:** High  
> **Size:** Medium  
> **Created:** 2026-07-19  
> **Depends On:** TASK-DASH-004 (complete)

---

## Objective

Enable users to customize their dashboard layout by:
1. Drag-and-drop card reordering (SortableJS)
2. Card resize (Small/Medium/Large presets)
3. Per-card refresh button
4. Persist layout changes to database

---

## Current State Analysis

### Database Schema (Already Supports Layout)
The `DashboardCard` model already has:
```csharp
public int GridPositionX { get; set; }  // Grid X position (default 0)
public int GridPositionY { get; set; }  // Grid Y position (default 0)
public int GridWidth { get; set; }      // 1-12 columns (default 4)
public int GridHeight { get; set; }     // 1-6 rows (default 2)
```

### What's Missing
1. **Backend API** to save card positions/sizes
2. **Frontend UI** for drag-and-drop and resize
3. **Frontend API calls** to persist changes

---

## Scope

### In Scope
- **Drag-and-drop reordering** using SortableJS library
- **Card resize** with 3 preset sizes (Small/Medium/Large)
- **Per-card refresh** button
- **Layout persistence** via API to database
- **Visual feedback** during drag operations
- **Responsive handling** for mobile devices

### Out of Scope
- Free-form positioning (grid-based only)
- Card creation/deletion (existing admin panel)
- Dashboard templates
- Export/Print functionality

---

## Technical Requirements

### 1. Backend API Endpoints

**File:** `Pages/Index.cshtml.cs` (add new handler methods)

Add Razor Page handler methods for layout operations:

```csharp
// POST handler: Save card layout (position + size)
public async Task<IActionResult> OnPostSaveLayoutAsync([FromBody] SaveLayoutRequest request)
{
    // Update each card's GridPositionX, GridPositionY, GridWidth, GridHeight
    // Return success/failure
}

// Request model
public class SaveLayoutRequest
{
    public int DashboardId { get; set; }
    public List<CardLayoutItem> Cards { get; set; } = new();
}

public class CardLayoutItem
{
    public int CardId { get; set; }
    public int GridPositionX { get; set; }
    public int GridPositionY { get; set; }
    public int GridWidth { get; set; }
    public int GridHeight { get; set; }
}
```

### 2. Frontend: SortableJS Integration

**File:** `Pages/_DashboardLayout.cshtml`
- Add SortableJS CDN:
  ```html
  <script src="https://cdn.jsdelivr.net/npm/sortablejs@1.15.0/Sortable.min.js"></script>
  ```

**File:** `Pages/Index.cshtml` (JS section)
- Initialize SortableJS on `.wd-dashboard-grid`
- Configuration:
  ```javascript
  var sortable = new Sortable(grid, {
      animation: 200,
      ghostClass: 'wd-card--ghost',
      dragClass: 'wd-card--drag',
      handle: '.wd-card__header',  // Drag from header only
      onEnd: function(evt) {
          // Update card positions
          // Save layout to backend
      }
  });
  ```

### 3. Frontend: Card Resize

**File:** `Pages/Index.cshtml` (HTML section)
- Add resize controls to each card header:
  ```html
  <div class="wd-card__resize">
      <button class="wd-resize-btn" data-size="small" title="صغير">S</button>
      <button class="wd-resize-btn" data-size="medium" title="متوسط">M</button>
      <button class="wd-resize-btn" data-size="large" title="كبير">L</button>
  </div>
  ```

**File:** `Pages/Index.cshtml` (JS section)
- Handle resize button clicks
- Update card classes: `wd-span-4` → `wd-span-6` etc.
- Save layout to backend

**Size Presets:**
| Size | GridWidth | GridHeight | CSS Classes |
|---|---|---|---|
| Small | 3 | 2 | `wd-span-3 wd-row-2` |
| Medium | 6 | 3 | `wd-span-6 wd-row-3` |
| Large | 9 | 4 | `wd-span-9 wd-row-4` |

### 4. Frontend: Per-card Refresh

**File:** `Pages/Index.cshtml` (HTML section)
- Add refresh button to each card header:
  ```html
  <button class="wd-card__refresh" title="تحديث البطاقة" aria-label="تحديث البطاقة">
      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M23 4v6h-6"/><path d="M1 20v-6h6"/>
          <path d="M3.51 9a9 9 0 0114.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0020.49 15"/>
      </svg>
  </button>
  ```

**File:** `Pages/Index.cshtml` (JS section)
- Handle refresh button click
- Call `wdLoadCard(cardId, true)` with skeleton
- Add loading spinner animation

### 5. CSS Styles

**File:** `wwwroot/css/blue-theme.css`

Add styles for:
```css
/* Drag-and-drop states */
.wd-card--ghost { opacity: 0.4; border: 2px dashed var(--c-primary); }
.wd-card--drag { box-shadow: var(--shadow-xl); transform: scale(1.02); }

/* Resize controls */
.wd-card__resize { display: flex; gap: 4px; }
.wd-resize-btn {
    width: 24px; height: 24px;
    border: 1px solid var(--c-border);
    border-radius: var(--radius-sm);
    background: var(--c-surface);
    font-size: 11px; font-weight: 600;
    cursor: pointer; color: var(--c-text-muted);
}
.wd-resize-btn:hover { background: var(--c-surface-muted); color: var(--c-text); }
.wd-resize-btn.active { background: var(--c-primary); color: #fff; border-color: var(--c-primary); }

/* Refresh button */
.wd-card__refresh {
    width: 28px; height: 28px;
    display: flex; align-items: center; justify-content: center;
    border: none; border-radius: var(--radius-full);
    background: transparent; cursor: pointer;
    color: var(--c-text-muted);
    transition: all var(--dur-fast) var(--ease);
}
.wd-card__refresh:hover { background: var(--c-surface-muted); color: var(--c-primary); }
.wd-card__refresh.spinning svg { animation: wdSpin 1s linear infinite; }
@keyframes wdSpin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }

/* Layout save indicator */
.wd-layout-saved {
    position: fixed; bottom: 24px; inset-inline-start: 24px;
    background: var(--c-success); color: #fff;
    padding: 8px 16px; border-radius: var(--radius-md);
    font-size: 13px; font-weight: 600;
    animation: wdFadeUp 0.3s ease both;
    z-index: 1000;
}
```

---

## Acceptance Criteria

| # | Criterion | Verified |
|---|---|---|
| 1 | SortableJS loaded and functional | [ ] |
| 2 | Cards can be dragged and dropped to reorder | [ ] |
| 3 | Drag visual feedback (ghost, drag class) works | [ ] |
| 4 | Card resize buttons (S/M/L) visible and functional | [ ] |
| 5 | Resize updates card grid classes correctly | [ ] |
| 6 | Per-card refresh button works | [ ] |
| 7 | Layout changes persist to database | [ ] |
| 8 | Layout loads correctly on page refresh | [ ] |
| 9 | Mobile: touch drag works | [ ] |
| 10 | Build succeeds with 0 errors | [ ] |

---

## Files to Modify

| File | Changes |
|---|---|
| `Pages/_DashboardLayout.cshtml` | Add SortableJS CDN |
| `Pages/Index.cshtml` | Add resize controls, refresh buttons, JS logic |
| `Pages/Index.cshtml.cs` | Add SaveLayout handler |
| `wwwroot/css/blue-theme.css` | Add drag/resize/refresh styles |

---

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\_DashboardLayout.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css
```

---

## Vitality & Polish Checklist

- [ ] ✅ Skeleton Loading / Shimmer — لكل بطاقة، جدول، ورسم بياني
- [ ] ✅ Toast Notifications — للتغذية الراجعة (نجاح، فشل، تحذير)
- [ ] ✅ Connection Status Indicator — مؤشر حي (متصل/غير متصل)
- [ ] ✅ Search حقيقي — في الجداول (إن وُجدت)
- [ ] ✅ Micro-animations — Stagger entries، Hover effects، Number counters
- [ ] ✅ Empty States — لكل قسم (لا توجد بيانات)
- [ ] ✅ Realistic Data — أسماء، أرقام، تفاصيل تبدو حقيقية

---

## Notes for Agent

1. **Preserve existing functionality:** Do not break filtering, search, theme switching, drill-down modals
2. **Test drag-and-drop:** Ensure cards can be reordered and layout persists
3. **Test resize:** Ensure cards resize correctly and layout persists
4. **Test per-card refresh:** Ensure individual cards can be refreshed
5. **RTL support:** All new elements must be RTL-compatible
6. **Mobile:** Touch drag must work on mobile devices
7. **Performance:** Ensure drag operations are smooth (no lag)
