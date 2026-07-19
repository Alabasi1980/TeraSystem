# Visual References — TASK-CARD-KPI-02

Date: 2026-07-19

## Scope
Improve KPI card sparkline rendering for the Warehouse Dashboard: clearer area/line visual, end-point emphasis, smooth hover markers, and monthly tooltip context.

## References reviewed

1. Behance — Advanced SaaS Analytics Dashboards & KPI UI/UX - P1  
   URL: https://www.behance.net/gallery/129162191/Advanced-SaaS-Analytics-Dashboards-KPI-UIUX-P1  
   Inspire: strong KPI hierarchy with compact trend charts that support the number instead of competing with it.  
   Avoid: over-dense dashboard decoration and non-Arabic spacing assumptions.

2. Behance — Advanced SaaS Analytics Dashboards & KPI UI/UX — Set 01  
   URL: https://www.behance.net/gallery/93727935/Advanced-SaaS-Analytics-Dashboards-KPI-UIUX-Set-01  
   Inspire: restrained gradients and soft data-card contrast.  
   Avoid: hardcoded palette choices; this task must use Card Builder palette first.

3. Behance — KPI Dashboard - Cyber Security  
   URL: https://www.behance.net/gallery/243790529/KPI-Dashboard-Cyber-Security  
   Inspire: more confident chart strokes and visible data-change affordances.  
   Avoid: dark cyber styling that does not match blue-theme warehouse UI.

4. Awwwards — Algorithmic Trading Dashboard  
   URL: https://www.awwwards.com/sites/algorithmic-trading-dashboard  
   Inspire: interactive financial-style data clarity and tooltip-first exploration.  
   Avoid: high-noise trading UI; KPI card should remain calm.

5. Awwwards — Viture Dashboard  
   URL: https://www.awwwards.com/sites/viture-dashboard  
   Inspire: polished dashboard micro-interactions and elegant dark/light adaptability.  
   Avoid: decorative motion that distracts from quick KPI reading.

## Direction chosen
- Use an ApexCharts area sparkline at ~90px height.
- Keep the card KPI value/change rendering intact.
- Use `wdGetPalette(cardId)` for stroke/fill/marker colors with existing theme fallback.
- Tooltip provides month, formatted value, and previous-month delta where available.
- Insufficient sparkline data gets a deliberate Arabic empty state.
