# Figma Integration Protocol

## Purpose

Define how Tera handles a Figma design file as a formal design source when `Design Source Mode = FIGMA_DESIGN_FILE`.

## Core Rule

```text
No frontend implementation directly from Figma.
Always extract executable rules into 28_UI_UX_GUIDELINES.md first.
```

## When to Use FIGMA_DESIGN_FILE

Use `FIGMA_DESIGN_FILE` instead of `USER_PROVIDED_REFERENCE` when the client provides:

- A Figma file (link, `.fig` file, or exported frames).
- Structured screenshots showing component variants, states, or auto-layout.
- Figma variables or design tokens.
- A design system or component library defined in Figma.

Do NOT use `FIGMA_DESIGN_FILE` for:

- A single screenshot without structure (use `USER_PROVIDED_REFERENCE`).
- A reference website or inspiration link (use `EXTERNAL_URL_ANALYSIS`).

## Flow

```text
Client provides Figma file / link / structured exports
        ↓
Tera records Design Source Mode = FIGMA_DESIGN_FILE
        ↓
UIVisualDesignerAgent analyzes the Figma source:
  - Extracts colors, typography, spacing
  - Maps frames/pages to project screens
  - Documents component variants and states
  - Identifies layout patterns and constraints
  - Notes responsive assumptions
  - Records missing or unclear design areas as Design Gaps
        ↓
Updates project-preparation/28_UI_UX_GUIDELINES.md with:
  - Section 3: Figma file reference
  - Section 3.5: Figma Source Mapping (optional sub-section)
  - Sections 5-8: Tokens, components, layout, RTL rules
        ↓
EngineeringAgent implements from 28_UI_UX_GUIDELINES.md only
        ↓
UI Acceptance Gate verifies against Figma-derived rules
```

## UIVisualDesignerAgent Responsibilities

When `FIGMA_DESIGN_FILE` is active, UIVisualDesignerAgent must:

1. Reference the Figma file in `project-preparation/design-source/`.
2. Extract all available design tokens (colors, typography, spacing).
3. Map Figma components to project components.
4. Document component variants, states (hover, focus, disabled, loading, error, empty).
5. Note responsive behavior if the Figma includes multiple frame sizes.
6. Record any missing design details as Design Gaps.
7. Never copy the Figma file as executable spec — the final output is always `28_UI_UX_GUIDELINES.md`.

## Restrictions

- EngineeringAgent must NOT open or read Figma files directly.
- EngineeringAgent must NOT implement UI from Figma screenshots without extracted rules.
- Figma fonts that are not available in the project stack must be noted as Design Gaps.
- Figma components that cannot be replicated exactly in the chosen UI framework must be noted with alternatives.
