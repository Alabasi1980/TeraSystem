# Design Source Protocol

## Purpose

Determine the visual design source before any frontend/UI execution planning or implementation.

## Mandatory Rule

```text
No Frontend Execution Planning without Design Source Decision.
No UI Implementation without project-preparation/28_UI_UX_GUIDELINES.md when visual style matters.
```

## Design Source Modes

| Mode | Use When | Required Output |
|---|---|---|
| `INTERNAL_TERA_KIT` | No external source; admin/ERP/CRM/dashboard default | Compact or full `28_UI_UX_GUIDELINES.md` from internal kit |
| `GETDESIGN_MD` | A suitable `getdesign.md` DESIGN.md exists | Raw DESIGN.md + conversion into `28_UI_UX_GUIDELINES.md` |
| `FIGMA_DESIGN_FILE` | Client provides a Figma design file with frames, components, tokens, or auto-layout | Figma analysis → extracted tokens → `28_UI_UX_GUIDELINES.md` |
| `USER_PROVIDED_REFERENCE` | Client provides screenshots, colors, CSS, brand notes, or informal sketches | Raw source notes + extracted executable rules |
| `EXTERNAL_URL_ANALYSIS` | Client provides reference website and analysis is available/approved | Analysis report + executable rules |
| `HYBRID` | Combine internal kit with client colors or external reference | Explicit precedence rules |
| `NO_UI` | API/backend-only or no visual UI in current phase | Record `No UI` in plan/task |
| `N/A` | Design decision does not apply to the current record | Record reason |

## Activation by Phase

| Phase | Design Responsibility |
|---|---|
| Phase 1 | Collect preferences, screenshots, brand colors, references, RTL/LTR needs |
| Phase 3 | Decide whether `28_UI_UX_GUIDELINES.md`, design-source files, or UIVisualDesignerAgent are required |
| Phase 4 | Generate/activate UIVisualDesignerAgent only when design governance is needed now |
| Phase 5 | Block frontend TASK-COD generation until Design Source Decision exists |
| Phase 6 | EngineeringAgent follows `28_UI_UX_GUIDELINES.md` and UI Acceptance Gate |

## Selection Rules

1. If the user provides a specific reference, use `USER_PROVIDED_REFERENCE` or `EXTERNAL_URL_ANALYSIS`.
2. If the user provides a Figma design file (link, file, or detailed screenshots with component structure), use `FIGMA_DESIGN_FILE`.
3. If the user asks for a known style direction and `getdesign.md` has a suitable DESIGN.md, use `GETDESIGN_MD` as a source, not as final scope.
4. If the project is internal admin/ERP/CRM/dashboard without a design source, use `INTERNAL_TERA_KIT` starting with `KIT_ADMIN_DASHBOARD.md`.
5. If the project has no UI, record `NO_UI` and do not create design files.
6. Do not mention famous brands to clients as identity promises. Describe style qualities instead.

When `FIGMA_DESIGN_FILE` is selected as the official design source, Tera must use `FIGMA_INTEGRATION.md` and ask the user for the required official Figma adoption inputs. The user is not expected to remember that checklist.

## Required Decision Record

Every UI project must record:

```text
Design Source Mode:
Selected Source:
Why selected:
What will be used:
What will not be used:
Client overrides:
Risk of brand imitation:
Final executable file: project-preparation/28_UI_UX_GUIDELINES.md
```
