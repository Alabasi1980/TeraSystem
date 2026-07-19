# TASK-DASH-004: ApexCharts + KPI Enhancement + Filters + Dark Mode + Drill-down Modals

> **Status:** Approved  
> **Assigned:** engineering-agent  
> **Priority:** High  
> **Size:** Large (single task, multiple coordinated changes)  
> **Created:** 2026-07-18  
> **Depends On:** TASK-DASH-001, TASK-DASH-002, TASK-DASH-003 (all complete)

---

## Objective

Comprehensive UI/UX overhaul of the Warehouse Dashboard public-facing page:
1. Replace Chart.js with ApexCharts for better interactivity, RTL support, and drill-down capabilities
2. Enhance KPI widgets with ApexCharts sparklines and animated counters
3. Add Quick Date Presets to the filter bar
4. Improve Dark Mode (midnight theme) with proper chart colors
5. Add Drill-down Modals (in-page, not page navigation)
6. Improve Skeleton Loading and Empty States

---

## Scope

### In Scope
- **Chart Engine Migration:** Chart.js → ApexCharts (CDN)
- **KPI Enhancement:** ApexCharts sparklines, animated counters, change badges
- **Quick Date Presets:** Today, Yesterday, Last 7 Days, Last 30 Days, This Month, Custom Range
- **Dark Mode Improvements:** Chart colors, card backgrounds, toast colors
- **Drill-down Modals:** In-page modals with breadcrumb navigation
- **Skeleton Loading:** Improved shimmer effects
- **Empty States:** Better illustrations and messaging

### Out of Scope
- Backend changes (API endpoints, database schema)
- Admin panel modifications
- New dashboard creation features
- Export/Print functionality (future enhancement)

---

## Technical Requirements

### 1. Chart.js → ApexCharts Migration

**File:** `Pages/_DashboardLayout.cshtml`
- Remove Chart.js CDN: `<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.7/dist/chart.umd.min.js"></script>`
- Add ApexCharts CDN: `<script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>`

**File:** `Pages/Index.cshtml` (inline JS section)
- Replace all `new Chart(...)` calls with `new ApexCharts(...)` calls
- Update chart configuration to ApexCharts format:
  - Bar charts: `chart: { type: 'bar' }`
  - Line charts: `chart: { type: 'line' }`
  - Pie charts: `chart: { type: 'pie' }`
  - Doughnut charts: `chart: { type: 'donut' }`
- Update color palette to use ApexCharts format
- Add RTL support: `chart: { rtl: true, toolbar: { show: false } }`
- Add zoom/pan for line/bar charts: `chart: { zoom: { enabled: true } }`

**File:** `wwwroot/js/dashboard-utils.js`
- No changes needed (utility functions remain the same)

### 2. KPI Widget Enhancement

**File:** `Pages/Index.cshtml` (inline JS section)
- Update `wdKpiHtml()` function to use ApexCharts for sparklines
- Replace `wdRenderSparkline()` with ApexCharts mini sparkline
- Add animated counter with easing (already exists, keep as-is)
- Add trend indicators (↑↓→) with color coding (already exists, keep as-is)

**ApexCharts Sparkline Config:**
```javascript
{
  chart: { type: 'line', height: 60, sparkline: { enabled: true } },
  stroke: { curve: 'smooth', width: 2 },
  colors: ['#1F4E79'],
  tooltip: { enabled: false }
}
```

### 3. Quick Date Presets

**File:** `Pages/Index.cshtml` (HTML section)
- Add date preset buttons to filter bar:
  ```html
  <div class="wd-filterbar__presets">
    <button class="wd-preset-btn active" data-preset="today">اليوم</button>
    <button class="wd-preset-btn" data-preset="yesterday">أمس</button>
    <button class="wd-preset-btn" data-preset="7days">آخر 7 أيام</button>
    <button class="wd-preset-btn" data-preset="30days">آخر 30 يوم</button>
    <button class="wd-preset-btn" data-preset="month">هذا الشهر</button>
    <button class="wd-preset-btn" data-preset="custom">مخصص</button>
  </div>
  ```

**File:** `Pages/Index.cshtml` (JS section)
- Add click handlers for preset buttons
- Store selected preset in `window.WD_DATE_PRESET`
- Pass preset to card fetch API: `/api/dashboard/card/{id}?preset={preset}`
- Add custom date range picker (HTML5 date inputs)

**File:** `wwwroot/css/blue-theme.css`
- Add preset button styles:
  ```css
  .wd-filterbar__presets { display: flex; gap: var(--sp-2); flex-wrap: wrap; }
  .wd-preset-btn {
    padding: 6px 12px; border-radius: var(--radius-md);
    border: 1px solid var(--c-border); background: var(--c-surface);
    font-family: var(--font-ar); font-size: 13px; font-weight: 600;
    color: var(--c-text-muted); cursor: pointer;
    transition: all var(--dur-fast) var(--ease);
  }
  .wd-preset-btn:hover { background: var(--c-surface-muted); color: var(--c-text); }
  .wd-preset-btn.active { background: var(--c-primary); color: #fff; border-color: var(--c-primary); }
  ```

### 4. Dark Mode Improvements

**File:** `wwwroot/css/blue-theme.css`
- Update `[data-theme="midnight"]` section with chart-specific colors:
  ```css
  [data-theme="midnight"] .apexcharts-canvas { background: transparent !important; }
  [data-theme="midnight"] .apexcharts-text { fill: var(--c-text) !important; }
  [data-theme="midnight"] .apexcharts-gridline { stroke: var(--c-border) !important; }
  [data-theme="midnight"] .apexcharts-tooltip { background: var(--c-surface) !important; color: var(--c-text) !important; }
  ```
- Update card backgrounds for dark mode (already exists, verify)
- Update toast colors for dark mode (already exists, verify)

### 5. Drill-down Modals

**File:** `Pages/Index.cshtml` (HTML section)
- Add modal container:
  ```html
  <div id="wd-drill-modal" class="wd-modal" hidden>
    <div class="wd-modal__overlay"></div>
    <div class="wd-modal__panel">
      <div class="wd-modal__header">
        <h3 class="wd-modal__title"></h3>
        <button class="wd-modal__close" aria-label="إغلاق">×</button>
      </div>
      <div class="wd-modal__breadcrumb"></div>
      <div class="wd-modal__body"></div>
    </div>
  </div>
  ```

**File:** `Pages/Index.cshtml` (JS section)
- Update `wdOpenDrill()` to open modal instead of navigating:
  ```javascript
  window.wdOpenDrill = function(id) {
    var modal = document.getElementById('wd-drill-modal');
    modal.hidden = false;
    document.body.style.overflow = 'hidden';
    // Fetch drill data and render in modal
    fetch('/api/dashboard/drill/' + id)
      .then(r => r.json())
      .then(data => { /* render drill-down content */ });
  };
  ```
- Add breadcrumb navigation for nested drill-downs
- Add close button handler
- Add escape key handler

**File:** `wwwroot/css/blue-theme.css`
- Add modal styles:
  ```css
  .wd-modal { position: fixed; inset: 0; z-index: 3000; display: flex; align-items: center; justify-content: center; }
  .wd-modal[hidden] { display: none; }
  .wd-modal__overlay { position: absolute; inset: 0; background: rgba(10,37,64,0.6); backdrop-filter: blur(4px); }
  .wd-modal__panel { position: relative; background: var(--c-surface); border-radius: var(--radius-lg); box-shadow: var(--shadow-xl); width: min(900px, 90vw); max-height: 85vh; display: flex; flex-direction: column; animation: wdSlideUp 0.3s ease both; }
  .wd-modal__header { display: flex; align-items: center; justify-content: space-between; padding: var(--sp-6); border-bottom: 1px solid var(--c-border); }
  .wd-modal__title { font-size: 18px; font-weight: 700; margin: 0; }
  .wd-modal__close { background: none; border: none; font-size: 24px; cursor: pointer; color: var(--c-text-muted); padding: 4px 8px; border-radius: var(--radius-md); }
  .wd-modal__close:hover { background: var(--c-surface-muted); }
  .wd-modal__breadcrumb { padding: var(--sp-3) var(--sp-6); font-size: 13px; color: var(--c-text-muted); }
  .wd-modal__body { flex: 1; overflow: auto; padding: var(--sp-6); }
  ```

### 6. Skeleton Loading Improvements

**File:** `wwwroot/css/blue-theme.css`
- Enhance shimmer animation:
  ```css
  .wd-skel {
    background: linear-gradient(90deg, var(--c-surface-muted) 25%, rgba(143,188,222,0.15) 50%, var(--c-surface-muted) 75%);
    background-size: 200% 100%;
    animation: wdShimmer 1.5s ease-in-out infinite;
  }
  ```
- Add skeleton variants for different card types (chart, KPI, table)

### 7. Empty States Improvements

**File:** `Pages/Index.cshtml`
- Update empty state SVG illustrations (already good, keep as-is)
- Add animated entrance for empty states

---

## Acceptance Criteria

| # | Criterion | Verified |
|---|---|---|
| 1 | ApexCharts CDN loaded and functional | [ ] |
| 2 | All chart types render correctly (Bar, Line, Pie, Gauge) | [ ] |
| 3 | KPI sparklines render with ApexCharts | [ ] |
| 4 | Quick Date Presets functional and styled | [ ] |
| 5 | Dark mode chart colors correct | [ ] |
| 6 | Drill-down modals open/close correctly | [ ] |
| 7 | Skeleton loading animations smooth | [ ] |
| 8 | Empty states display correctly | [ ] |
| 9 | RTL layout correct for all new elements | [ ] |
| 10 | Mobile responsive | [ ] |
| 11 | Build succeeds with 0 errors | [ ] |

---

## Files to Modify

| File | Changes |
|---|---|
| `Pages/_DashboardLayout.cshtml` | Replace Chart.js CDN with ApexCharts |
| `Pages/Index.cshtml` | Update chart rendering, add modal, add date presets |
| `wwwroot/css/blue-theme.css` | Add modal styles, preset styles, dark mode chart colors |
| `wwwroot/js/dashboard-utils.js` | No changes (keep as-is) |

---

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\_DashboardLayout.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
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

1. **Preserve existing functionality:** Do not break any existing features
2. **Test all chart types:** Ensure Bar, Line, Pie, Gauge, KPI all work
3. **RTL support:** All new elements must be RTL-compatible
4. **Dark mode:** Test all new elements in midnight theme
5. **Mobile:** Ensure responsive design works on all screen sizes
6. **Performance:** ApexCharts is lighter than Chart.js, ensure no performance regression
7. **Accessibility:** Add proper ARIA labels to modals and interactive elements
