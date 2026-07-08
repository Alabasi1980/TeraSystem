# project-preparation/

## Role
Internal Tera preparation outputs — the bridge between **project intake** and **execution planning**.

This folder contains project-specific preparation files that Tera generates **after** the Project Decision is made and **before** Phase 5 Execution Planning begins.

## When This Folder Is Created
- Created at the system root as a container for all project preparation files.
- Each client project may also have its own `project-preparation/` inside its application folder (e.g., `clients/CLIENT-*/applications/APP-*/project-preparation/`).

## What Goes Here

| File | Purpose |
|------|---------|
| `28_UI_UX_GUIDELINES.md` | Executable visual design rules for EngineeringAgent — design tokens, components, layout, RTL rules |
| `APPLICATION_BLUEPRINT.md` | High-level application blueprint when required |
| `draft-seeds/` | Optional draft seeds for blueprinting |

## What Does NOT Go Here
- Client-facing approval packages → `clients/CLIENT-*/`
- Execution plans or task IDs → `project-control/`
- Core system policies → `tera-system/`
- Design source raw assets → `project-preparation/design-source/`

## Source of Truth
`TeraPolicyMap.md` in `tera-system/` governs the official file catalog and mapping.
