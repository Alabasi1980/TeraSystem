# Visual References — TASK-ENH-QT-004: JOIN Builder for Query Tester

Date: 2026-07-21

## Scope
Add a visual JOIN Builder section to the Query Tester page that allows users to build multi-table JOIN queries visually — with table selection, alias, JOIN type, ON conditions, column checkboxes, and SQL generation.

## References reviewed

1. **Dribbble — SQL Query Builder UI**
   URL: https://dribbble.com/search/sql-query-builder
   Inspire: Visual approach to JOIN construction with side-by-side table selectors and drag-free condition rows.
   Avoid: Overly complex tree-based builders that are hard to use on mobile.

2. **Dribbble — Visual Query Builder Dashboard**
   URL: https://dribbble.com/search/query-builder-ui
   Inspire: Card-based layout for query clauses with clear visual hierarchy between SELECT, JOIN, WHERE sections.
   Avoid: Heavy visual styling that distracts from the functional purpose.

3. **Refactoring UI — Card Design Patterns**
   URL: https://refactoringui.com/
   Inspire: Clean card separation with subtle border and shadow; compact inline form controls for conditions.
   Avoid: Adding decorative elements that don't serve the query-building workflow.

4. **Awwwards — Admin Dashboard Templates**
   URL: https://www.awwwards.com/websites/query-builder/
   Inspire: Two-panel layout with schema sidebar and main query construction area.
   Avoid: Template-heavy styling inconsistent with the existing Warehouse Dashboard Query Tester theme.

5. **SQL Server Management Studio — Query Designer (Desktop reference)**
   URL: https://learn.microsoft.com/en-us/sql/ssms/visual-db-tools/query-designer
   Inspire: The JOIN condition grid pattern (table1.column = table2.column) with operator selection.
   Avoid: Desktop-only interaction patterns that don't translate to web.

## Direction chosen
- Use a card-based JOIN Builder section between SELECT Builder and WHERE Builder.
- Each JOIN clause is a bordered card with table1/alias/JOIN_TYPE/table2/alias in a single header row.
- ON conditions are stacked rows with column-operator-column selects and remove button.
- CROSS JOIN type hides ON conditions automatically.
- A column selector section shows checkboxes with alias-prefixed column names after table selection.
- SQL generation produces complete SELECT ... FROM ... JOIN ... ON ... statements.
- Fully integrated with existing source switching, table loading, and SQL textarea.
