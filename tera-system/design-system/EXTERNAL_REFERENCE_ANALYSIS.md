# External Reference Analysis

## Purpose

Convert user-provided visual references into executable UI rules without guessing.

Supported references:

- screenshots
- Figma notes or exports
- CSS/theme files
- brand/color files
- website URLs
- user descriptions of layout and colors
- `getdesign.md` DESIGN.md files

## Process

1. Store or summarize raw reference in `project-preparation/design-source/`.
2. Extract observable rules:
   - color palette
   - typography
   - spacing density
   - layout pattern
   - component styles
   - states and interactions
   - RTL/LTR behavior
   - accessibility risks
3. Ask only for missing decisions that materially affect execution.
4. Convert approved rules into `28_UI_UX_GUIDELINES.md`.
5. Record open design gaps if the source is incomplete.

## Screenshot Handling

Screenshots are visual references, not complete specifications. Tera must infer cautiously and record uncertainty.

If a screenshot lacks hover states, responsive behavior, accessibility states, or component variants, record them as `Open Design Gaps` instead of guessing.

## External URL Handling

External URLs may be used as inspiration or analysis input. They must not become mandatory scope or a brand-copying target.

## Required Output Sections

```text
Reference Type:
Observed Rules:
Unknown / Missing Rules:
Client Decisions Needed:
Approved Conversion:
Forbidden Styling:
```
