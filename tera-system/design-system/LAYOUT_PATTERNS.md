# Layout Patterns

## Purpose

Provide approved layout patterns that can be selected or adapted per project.

## Patterns

### 1. Admin Shell

Use for ERP, CRM, dashboards, internal systems.

```text
Topbar + collapsible sidebar + wide content area + cards/tables
```

Rules:
- Sidebar width: 260px expanded / 72px collapsed by default.
- Content max width: full responsive canvas unless form-focused.
- Header area contains page title, breadcrumbs, primary action.

### 2. Data Table Workspace

Use for data-heavy management screens.

Rules:
- Filters above table or in collapsible panel.
- Primary action in header.
- Row actions compact and consistent.
- Pagination or infinite scroll must be chosen explicitly.

### 3. Form Detail Layout

Use for create/edit/detail workflows.

Rules:
- Group fields into sections.
- Sticky action bar only when forms are long.
- Validation appears near fields.

### 4. Public Landing Layout

Use for public websites and marketing pages.

Rules:
- Hero, value propositions, proof, CTA, FAQ.
- Not default for internal ERP/admin apps.

## Forbidden

- Do not mix unrelated layout patterns in one screen without decision.
- Do not create separate screens when one management workspace is sufficient for MVP.
