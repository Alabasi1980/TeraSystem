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

---

# Visual References — TASK-CARD-KPI-REDESIGN-01

Date: 2026-07-19

## Scope
Redesign the Warehouse Dashboard KPI card content layout so composite KPIs can show value, comparison, totals, sparkline, and compact category breakdown without internal scrolling or overlap.

## References reviewed

1. Behance — KPI Dashboard UI/UX  
   URL: https://www.behance.net/gallery/199233985/KPI-Dashboard-UIUX  
   Inspire: strong central number hierarchy with support metrics grouped below.  
   Avoid: oversized decorative chart areas that would steal height from small cards.

2. Behance — Modern KPI Dashboard Cards UI Kit — 2025  
   URL: https://www.behance.net/gallery/227305143/Modern-KPI-Dashboard-Cards-UI-Kit-2025  
   Inspire: compact KPI modules with concise peer metric labels and soft card contrast.  
   Avoid: purely LTR assumptions; the Warehouse card must remain RTL-aware.

3. Behance — Advanced SaaS Analytics Dashboards & KPI UI/UX - P1  
   URL: https://www.behance.net/gallery/129162191/Advanced-SaaS-Analytics-Dashboards-KPI-UIUX-P1  
   Inspire: trend charts docked as supporting visuals under executive KPI values.  
   Avoid: over-dense dashboard ornaments and heavy gradients.

4. Behance — KPI Lab | Analytics Dashboard  
   URL: https://www.behance.net/gallery/246138831/KPI-Lab-Analytics-Dashboard  
   Inspire: analytical card layouts that split primary value from secondary facts.  
   Avoid: tiny low-contrast labels that fail with Arabic text.

5. Awwwards — Algorithmic Trading Dashboard  
   URL: https://www.awwwards.com/sites/algorithmic-trading-dashboard  
   Inspire: bottom trend-line confidence and financial dashboard compactness.  
   Avoid: high-noise trading density; Majed’s dashboard needs calm executive clarity.

## Direction chosen
- Use a three-zone KPI architecture: hero value, two peer metric blocks, bottom visual dock.
- Move drill affordance into the header controls while preserving `wdOpenDrill(cardId)` behavior.
- Limit category breakdown to top 3 rows in JavaScript and reinforce with CSS hiding.
- Remove KPI internal scrolling; use graceful hiding/minimizing on small cards.

---

# Visual References — ADMIN-DRILLDOWN-MODAL-COMPACT-01

Date: 2026-07-20

## Scope
Fix the Admin DrillDown configuration modal so it is fully visible above the admin chrome, uses available width, removes double scrolling, and compresses RTL form spacing without losing functionality.

## References reviewed

1. Behance — Add New Facility Modal Admin Dashboard design  
   URL: https://www.behance.net/gallery/238166149/Add-New-Facility-Modal-Admin-Dashboard-design  
   Inspire: modal-first admin form with clear header/footer and compact fields.  
   Avoid: narrow dialog proportions that create unnecessary vertical scrolling.

2. Behance — Add Customer Modal Admin Dashboard design  
   URL: https://www.behance.net/gallery/237425523/Add-Customer-Modal-Admin-Dashboard-design  
   Inspire: form grouping inside a focused overlay with simple actions.  
   Avoid: excess decorative spacing that harms dense configuration workflows.

3. Behance — Smart lock Admin Portal UI Design  
   URL: https://www.behance.net/gallery/181448705/Smart-lock-Admin-Portal-UI-Design  
   Inspire: clean SaaS/admin surface rhythm and calm contrast.  
   Avoid: LTR-only assumptions; this page remains Arabic RTL.

4. Behance — Cloud Admin Panel User Interface Design  
   URL: https://www.behance.net/gallery/186732809/Cloud-Admin-Panel-User-Interface-Design  
   Inspire: broad admin layout usage and concise control grouping.  
   Avoid: large empty panels that waste viewport height.

5. Awwwards — LUNO Admin Dashboard Template  
   URL: https://www.awwwards.com/sites/luno-admin-dashboard-template  
   Inspire: full admin overlay confidence and practical dashboard density.  
   Avoid: template-heavy styling inconsistent with the existing Warehouse Dashboard theme.

## Direction chosen
- Use a high-z-index fixed overlay sized to `100dvh` so the modal sits above the admin navbar.
- Make the modal a 3-row grid: fixed header, single scrollable body, fixed footer.
- Expand width to ~1120px and switch the form grid to 4 columns on wide screens.
- Reduce padding, section spacing, field gaps, and SQL textarea height for a compact configuration workflow.
