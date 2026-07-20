# Visual References — TASK-CARD-KPI-S-TOTALS-2ROWS-001

Date: 2026-07-20

Scope: compact KPI card density fix. The goal is not a redesign; references were reviewed to validate compact analytics-card hierarchy: primary value, secondary totals, and micro-chart can coexist without hiding meaningful summary rows.

## References

1. Behance — KPI Dashboard UI/UX  
   https://www.behance.net/gallery/199233985/KPI-Dashboard-UIUX  
   - Inspire: compact KPI cards retain more than one metric line beneath/near the headline value.
   - Avoid: introducing new decorative chrome or changing the client’s current palette/layout.

2. Behance — Modern KPI Dashboard Cards UI Kit — 2025  
   https://www.behance.net/gallery/227305143/Modern-KPI-Dashboard-Cards-UI-Kit-2025  
   - Inspire: strict content prioritization in small KPI cards.
   - Avoid: fake/demo values; keep live data/rendering intact.

3. Behance — Advanced SaaS Analytics Dashboards & KPI UI/UX - P1  
   https://www.behance.net/gallery/129162191/Advanced-SaaS-Analytics-Dashboards-KPI-UIUX-P1  
   - Inspire: dense dashboard cards can keep secondary numeric context visible with reduced typography.
   - Avoid: changing M/L behavior while fixing S density.

4. Awwwards — Algorithmic Trading Dashboard  
   https://www.awwwards.com/sites/algorithmic-trading-dashboard  
   - Inspire: data visualization cards prioritize quick numeric scan plus chart context.
   - Avoid: over-animated or large-card treatment for this small CSS bug.

5. Awwwards — LUNO Admin Dashboard Template  
   https://www.awwwards.com/sites/luno-admin-dashboard-template  
   - Inspire: compact admin dashboard information hierarchy.
   - Avoid: broad template-level redesign.

## Direction

Minimal CSS-only correction: preserve the established S composition, override only the conflicting compact-height rule for `.wd-kpi--size-small`, and continue hiding third-and-beyond total rows.
