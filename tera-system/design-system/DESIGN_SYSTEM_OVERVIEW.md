# Tera Design System Overview

## Purpose

This layer governs visual design, UI style, layout rules, design tokens, component rules, and UI acceptance inside Tera projects.

It prevents Tera or EngineeringAgent from inventing random UI styling during implementation.

## Core Rule

```text
Design Governance Layer exists always.
Full activation is conditional.
```

Tera chooses the design level by project type:

| Project Type | Design Governance Level |
|---|---|
| API only / backend only | No UI Design Layer required |
| Simple internal CRUD | Internal Kit + compact `28_UI_UX_GUIDELINES.md` |
| ERP / CRM / dashboard | Full Design Governance |
| SaaS / public UI / design-sensitive app | Full Design Governance + `getdesign.md` when useful |
| Client provides Figma design file | `FIGMA_DESIGN_FILE` — requires structured extraction into `28_UI_UX_GUIDELINES.md` |
| Client screenshots / reference website / brand notes | `USER_PROVIDED_REFERENCE` or `EXTERNAL_URL_ANALYSIS` |

## Source to Execution Flow

```text
Design Source Decision
-> Raw design source saved in project-preparation/design-source/
-> Converted into project-preparation/28_UI_UX_GUIDELINES.md
-> EngineeringAgent implements from 28_UI_UX_GUIDELINES.md only
-> UI_ACCEPTANCE_GATE verifies result
```

## Official Files

| File | Role |
|---|---|
| `DESIGN_SOURCE_PROTOCOL.md` | Design source modes and activation rules |
| `DESIGN_MD_INTEGRATION.md` | `getdesign.md` / DESIGN.md protocol |
| `EXTERNAL_REFERENCE_ANALYSIS.md` | Screenshots, URLs, brand notes, user references |
| `FIGMA_INTEGRATION.md` | Figma design file protocol (when FIGMA_DESIGN_FILE is active) |
| `INTERNAL_KITS_INDEX.md` | Internal fallback kits |
| `DESIGN_TOKENS_SCHEMA.md` | Approved token schema |
| `COMPONENT_LIBRARY_SCHEMA.md` | Component rule schema |
| `LAYOUT_PATTERNS.md` | Layout patterns |
| `RTL_LTR_RULES.md` | Arabic/English direction rules |
| `ACCESSIBILITY_RULES.md` | Accessibility baseline |
| `UI_ACCEPTANCE_GATE.md` | UI task acceptance gate |
| `kits/KIT_ADMIN_DASHBOARD.md` | First internal kit |

## Final Project Output

The final executable UI design decision is always:

```text
project-preparation/28_UI_UX_GUIDELINES.md
```

Raw sources such as DESIGN.md, Figma files/exports, screenshots, color palettes, and external references belong in:

```text
project-preparation/design-source/
```
