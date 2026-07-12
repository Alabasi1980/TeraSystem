# UI Acceptance Gate

## Purpose

This gate is required for every `TASK-COD-*` that creates or modifies UI, frontend layout, visual styling, UI components, or interaction states.

## Mandatory Rule

```text
Any UI implementation task must pass UI_ACCEPTANCE_GATE before it can be accepted or closed.
```

## Pre-Task Requirements

- `Design Source Decision` exists.
- `project-preparation/28_UI_UX_GUIDELINES.md` exists for visual UI work.
- Raw sources are saved in `project-preparation/design-source/` when applicable.
- TASK file includes UI Source, UI Rules, UI Acceptance, and Design Gap Handling.
- If `Design Source Mode = FIGMA_DESIGN_FILE`: Figma analysis must be completed and rules extracted into `28_UI_UX_GUIDELINES.md` before implementation.

## Gate Checklist

| Check | Required Result |
|---|---|
| UI source mode is recorded | PASS |
| `28_UI_UX_GUIDELINES.md` used as primary design source | PASS |
| No invented colors, spacing, typography, or component style | PASS |
| Component rules followed | PASS |
| Layout pattern followed | PASS |
| RTL/LTR rules followed | PASS / N/A |
| Responsive behavior matches rules | PASS / N/A |
| Accessibility baseline checked | PASS |
| Forbidden styling avoided | PASS |
| Design gaps recorded instead of guessed | PASS / N/A |
| Existing component patterns reused where available | PASS / N/A |
| **If FIGMA_DESIGN_FILE: Figma tokens/components matched** | PASS / N/A |

## Outcomes

```text
PASS
NEEDS_FIX
BLOCKED_DESIGN_GAP
```

`BLOCKED_DESIGN_GAP` means the task cannot continue until Tera or the user resolves a missing design rule.

## UI Acceptance Gate Result

Use this template inside the related `project-control/tasks/TASK-COD-XXX.md` before accepting or closing any UI/Frontend task.

```markdown
## UI Acceptance Gate Result

| Check | Result | Evidence | Notes |
|---|---|---|---|
| UI source mode recorded | PASS / FAIL |  |  |
| `28_UI_UX_GUIDELINES.md` used as primary design source | PASS / FAIL |  |  |
| No invented colors, spacing, typography, or component style | PASS / FAIL |  |  |
| Component rules followed | PASS / FAIL |  |  |
| Layout pattern followed | PASS / FAIL |  |  |
| RTL/LTR rules followed | PASS / FAIL / N/A |  |  |
| Responsive behavior matches rules | PASS / FAIL / N/A |  |  |
| Accessibility baseline checked | PASS / FAIL |  |  |
| Forbidden styling avoided | PASS / FAIL |  |  |
| Design gaps recorded instead of guessed | PASS / FAIL / N/A |  |  |
| Existing component patterns reused where available | PASS / FAIL / N/A |  |  |
| If FIGMA_DESIGN_FILE: Figma tokens/components matched | PASS / FAIL / N/A |  |  |

Gate Status: PASS / NEEDS_FIX / BLOCKED_DESIGN_GAP

Design Gaps:
- ...

Required Fixes:
- ...

Reviewer:
- Tera / UIVisualDesignerAgent / QAAndAcceptanceAgent / Other
```
