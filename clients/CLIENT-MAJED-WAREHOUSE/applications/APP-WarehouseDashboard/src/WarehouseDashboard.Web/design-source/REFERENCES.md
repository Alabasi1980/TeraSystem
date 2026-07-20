# REFERENCES — TASK-CARD-KPI-MOCKUP-001 (KPI Card Body)

## Research Sources

### 1. Client Mockup (source of truth)
- **Source:** Client-provided KPI card mockup (السندات)
- **Inspiration:** Right hero number + change pill below; left bordered “أعلى التصنيفات” panel; full-width gold sparkline with point markers
- **What I took:** Layout split, gold code column, value-then-label totals, spark gold + last-point ring
- **What I avoided:** Rebuilding card header; rainbow per-row category colors

### 2. Dribbble — Analytics KPI Cards with Sparkline
- **Keywords:** KPI card sparkline, dashboard metric card categories breakdown
- **Inspiration:** Large hero metric, compact secondary stats, mini area chart under content
- **What I took:** Hero-first hierarchy, sparkline as footer anchor, soft bordered side panel
- **What I avoided:** Dense multi-chart clutter inside a single KPI tile

### 3. Linear / Vercel-style metric density
- **Source:** Linear app metrics + Vercel dashboard patterns (public marketing/docs visuals)
- **Inspiration:** Calm change badges, tabular numbers, generous but purposeful whitespace
- **What I took:** Primary-tinted neutral change pill (not screaming green), tight type scale
- **What I avoided:** Oversized empty voids; decorative borders without function

### 4. ApexCharts Sparkline Patterns
- **Source:** ApexCharts sparkline + markers docs / community demos
- **Inspiration:** Smooth area stroke, gradient fill, discrete last-point emphasis
- **What I took:** markers on all points + larger last marker with light ring; reduced grid padding
- **What I avoided:** Heavy axes/toolbars inside KPI body

### 5. Refactoring UI — Hierarchy & Tables
- **Source:** Refactoring UI (hierarchy, muted labels, accent for secondary identifiers)
- **Inspiration:** Muted % column, bold values, accent (gold/warning) for codes
- **What I took:** Unified dark value text; gold code; centered muted panel title with hairline
- **What I avoided:** Per-row rainbow category coloring

## Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| DOM column order | main then categories | RTL: main on right, categories on left (matches mockup) |
| Grid | `1.15fr` / `0.95fr` | Fills body; categories panel readable without crushing hero |
| Change badge | Below number, primary-tinted pill | Mockup shows calm pill under hero, not baseline row |
| Grand totals | Value then label | Matches mockup RTL reading order |
| Categories columns | `% \| value \| code` (table LTR) | Exact mockup column order; code in warning gold |
| Category rows | slice 0–5; CSS hides for M/S | L=5, M=3–4, S=hidden panel |
| Spark color | `#E8A317` gold default | Mockup fidelity; gold/orange family only for override |
| Cluster flex | `flex:1` + space-between | Row grows; spark pinned bottom; no hatched voids |
| Density | size classes + container queries | Keep `wdSyncKpiDensity` resize path |
