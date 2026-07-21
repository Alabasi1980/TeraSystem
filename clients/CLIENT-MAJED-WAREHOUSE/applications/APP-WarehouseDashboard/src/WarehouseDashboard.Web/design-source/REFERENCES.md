# REFERENCES — TASK-ENH-QT-002 (QueryTester Schema Browser + SELECT Builder)

## Research Sources

### 1. Retool — SQL Query Builder
- **Source:** Retool (retool.com) — Embedded database query builder with left-side schema browser
- **Inspiration:** Two-panel layout: left sidebar (~280px) shows tree of database tables/columns, right panel contains SQL editor + results. Tree expands on click to show columns with data types.
- **What I took:** Two-panel flex layout, expandable tree items with chevron toggle, column click adds to editor
- **What I avoided:** Overly complex drag-drop; this uses simple click-to-add pattern

### 2. Supabase — Table Editor / SQL Editor
- **Source:** Supabase (supabase.com) — Database management UI with schema sidebar
- **Inspiration:** Schema sidebar with table list, column information panel, SQL editor with syntax highlighting area. Clean card-based results table.
- **What I took:** Source tabs concept (extended to SQL Server / Oracle switching), SELECT builder with column checkboxes, generated SQL injection into textarea
- **What I avoided:** Supabase's real-time subscription features (not needed for read-only query tester)

### 3. Azure Data Studio — Schema Browser
- **Source:** Azure Data Studio (Microsoft) — Database tree explorer
- **Inspiration:** Expand/collapse table nodes, column metadata (name + type), keyboard-friendly navigation
- **What I took:** Tree structure with `▶`/`▼` toggles, column type badges next to names, loading indicators per node
- **What I avoided:** Multi-select, drag-drop reorder (too complex for this scope)

### 4. SQL Server Management Studio — Object Explorer
- **Source:** SSMS — Object Explorer panel
- **Inspiration:** Hierarchical tree: Database → Tables → Columns. Click column to select. Clear visual hierarchy with indentation.
- **What I took:** Tree item padding/indentation pattern, hover highlight on rows, distinct icons for tables vs columns
- **What I avoided:** Object Explorer's multi-level nesting (only Tables → Columns is needed)

### 5. Refactoring UI — Two-Panel Layout Principles
- **Source:** Refactoring UI book — Layout, spacing, hierarchy
- **Inspiration:** Sidebar + main content with clear visual separation via background color and border. Sidebar has `flex-shrink: 0` to prevent collapse.
- **What I took:** 280px fixed sidebar, `gap: 16px` between panels, sidebar uses slightly muted background, main panel uses white surface
- **What I avoided:** Sidebar collapse animation (kept simple for performance)

## Direction

Clean two-panel QueryTester with schema browser on the left (280px) and SQL editor + results on the right. Source tabs (SQL Server / Oracle) at top switch the database context. Schema browser loads tables via AJAX and expands to show columns with data types. Clicking a column adds it to the SELECT builder (or directly appends to SQL). SELECT builder provides a table dropdown + column checkboxes + "Generate SELECT" button. All existing query/run/copy functionality preserved. Responsive: sidebar hides below 768px with toggle button.

## Design System Tokens
- Primary: `#1F4E79`, Secondary: `#2E6DA4`
- Background: `#F3F7FB`, Surface: `#FFFFFF`, Surface Muted: `#EAF1F8`
- Border: `#D4E2F0`, Text: `#102A43`, Text Muted: `#5B7A99`
- Radii: 6px / 10px / 14px
- Shadows: subtle `0 1px 3px` to medium `0 4px 16px`

Date: 2026-07-21
