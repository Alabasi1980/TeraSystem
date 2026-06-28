# Tera Active Context

## Purpose

This file is the startup handoff for new sessions in this ongoing project.

It is intentionally short.
It does not replace the official source files.

Use it to start fast, then open only the needed official files.

## Session Start Rule

For any new session in this project:

1. Read this file first.
2. Then read only the files needed for the current task.
3. Do not reopen all system/project files unless there is a conflict, ambiguity, review need, or explicit user request.

## Current Project Status

| Item | Status |
|---|---|
| Project | Checks Management App |
| Size | Small MVP |
| Runtime | OpenCode |
| Approved Tech | Next.js + TypeScript + PostgreSQL + Prisma |
| Current Phase | Phase 4 complete - Parties + Main Page built |
| Next Task | Fix ISSUE-0006 server-side validation before Checks S02 |
| Current Lead Agent | `Tera Agent` |
| Expected Execution Agents | `EngineeringAgent` + `FrontendAgent` |

## Last Closed Tasks

- `TASK-0001` - Scaffold Next.js + TypeScript + Prisma + `.env.example`
- `TASK-0002` - Prisma data models
- `TASK-0003` - Local database creation + first Prisma migration
- `TASK-0004` - Authentication + Roles
- `TASK-0005` - SecurityAgent independent security review of TASK-0004
- `TASK-0006` - Preparation: workflow-rules.md + screen-spec-s03.md
- `TASK-0007` - SEC-001 + SEC-002 fixes + Banks Screen S03
- `TASK-0008` - Parties Screen S04 — actions.ts + page.tsx
- `TASK-0009` - Main Navigation Hub — page.tsx + layout.tsx + actions.ts
- `TASK-0010` - Periodic Quality Review — QualityReviewCoordinatorAgent report

## Recent System Changes

- `2026-06-27`: EngineeringAgent split into EngineeringAgent (Backend/Logic) + FrontendAgent (UI/Styling)
- `2026-06-27`: Unified sub-agent lifecycle: draft first, activate on need, restart after activation
- `2026-06-27`: Minimal support layer added: `ProjectControlAgent` + `ExecutionPreparationAgent`
- `2026-06-27`: TASK-0008 (Parties S04) + TASK-0009 (Main Nav Hub) completed. requireAdmin() extended to Parties. Root page replaced with RTL navigation hub.
- `2026-06-28`: `QualityReviewCoordinatorAgent` added and activated for periodic cross-domain quality reviews before larger phases, before release, or when quality debt signals appear.
- `2026-06-28`: Tera role refined as `Primary Project Orchestrator / Decision Owner` with explicit helper-agent trigger rules and anti-over-delegation rules.

## Active Rules You Must Not Miss

- Read `project-preparation/PROJECT_RULES.md` before creating, approving, or delegating any new task.
- `Pre-Execution Gate: PASS` is required before any implementation delegation.
- `Post-Execution Review Gate` is required before `Accepted` or `Closed`.
- Real secrets are allowed only in approved local environment files or local environment variables.
- Never write real secrets into task files, logs, handbacks, reports, or config/code fallback values.
- Post-execution review must also inspect the core `project-control/` files, not only application code.
- If a secret appears in any task file, log, report, or control record, the task cannot PASS review.
- After each implementation task, decide whether `ProjectControlAgent`, `SecurityAgent`, or `QAAndAcceptanceAgent` must perform an independent follow-up review.
- Distinguish between task acceptance review and periodic quality review: `QAAndAcceptanceAgent` checks task/screen/workflow acceptance, while `QualityReviewCoordinatorAgent` coordinates broader periodic quality reviews across multiple domains.
- Do not assume that only the currently active sub-agents are available; Tera may generate an additional agent later when needed.
- New sub-agents start in `generated-agents/opencode/` and become active only after specialization plus copy into `.opencode/agents/`.
- After activating a new sub-agent in `.opencode/agents/`, Tera must ask for environment restart.
- `ExecutionPreparationAgent` may prepare task packages, but only Tera decides scope, timing, delegation, acceptance, or closure.
- `ProjectControlAgent` may manage control records and traceability checks, but only Tera decides final status changes.
- `QualityReviewCoordinatorAgent` may coordinate review scope and consolidate specialist findings, but only Tera decides what becomes a task, issue, deferred item, or accepted recommendation.
- Tera must explicitly decide whether `SecurityAgent` is needed for `Auth`, `JWT`, `Cookies`, `Middleware/Proxy`, `API Routes`, `Server Actions`, `Permissions`, `Role checks`, `Data Mutations`, `Secrets`, or `Config`.
- Do not route every small task through a long helper-agent chain unless there is a clear trigger.
- Local temp logs such as `.codex-dev-*.log` / `.codex-dev-*.err` are cleanup-required if they appear in Git.
- Project-control IDs must stay unique and sequential.
- Use only the smallest relevant context for the current task.

## Active Decisions / Important Notes

- `DEC-0004` - Secret handling and redaction rules are active across the system.
- `DEC-0005` - `TERA_ACTIVE_CONTEXT.md` is the startup file for ongoing sessions.
- `DEC-0007` - Post-execution review must inspect core `project-control` files and include an explicit independent-review decision when relevant.
- `DEC-0008` - EngineeringAgent split into Backend/Logic; FrontendAgent owns UI/Styling.
- `DEC-0010` - Unified sub-agent lifecycle: needed now vs likely later, draft first, activate only after specialization, restart after activation.
- `DEC-0011` - `requireAdmin()` pattern is active for future Server Actions.
- `DEC-0012` - Tera now uses a minimal support layer: `ProjectControlAgent` for control records and `ExecutionPreparationAgent` for task-package preparation.
- `DEC-0013` - `QualityReviewCoordinatorAgent` is active for periodic cross-domain quality reviews; it coordinates findings only and does not replace specialist reviewers.
- `DEC-0014` - Tera is fixed as `Primary Project Orchestrator / Decision Owner`; helper agents are trigger-based, over-delegation is disallowed, and temp dev logs are cleanup-required before new work.
- `SUB_AGENT_STATUS.md` is the lightweight manager review for sub-agent usage, load, quality, and update need.
- `ISSUE-0006` - Planned: add server-side validation to Banks/Parties actions before Checks S02.
- `ISSUE-0003` - Secret exposure incident from TASK-0003 was resolved; do not reintroduce secrets into docs or config fallbacks.

## Available Sub-Agents

| Agent | Use |
|---|---|
| `EngineeringAgent` | Backend/Logic: actions.ts, Prisma, Auth, Middleware, DB, Config |
| `FrontendAgent` | UI/Styling: page.tsx, components, CSS, RTL |
| `ProjectControlAgent` | Updating task/log/decision/issue records under Tera direction |
| `ExecutionPreparationAgent` | Preparing task packages before Tera review, gating, and delegation |
| `QualityReviewCoordinatorAgent` | Coordinating periodic quality reviews across UI, engineering, security, QA, and documentation |
| `SecurityAgent` | Independent follow-up review for Auth, Secrets, Permissions, Middleware, and Config tasks |
| `QAAndAcceptanceAgent` | Independent follow-up review for UI, Workflow, and acceptance checks |
| `BusinessWorkflowAgent` | Workflow review only when business process changes |
| `UIUXStructureAgent` | UI structure review only when screen/design scope changes |
| `ReportingAnalyticsAgent` | Report specifications before implementation |
| `DocumentationHandoverAgent` | Delivery documentation and final handover |

Activation note:
- The list above shows the currently active agents only.
- Tera may activate additional agents later if a real need appears.
- `QualityReviewCoordinatorAgent` is available now, but no review session should start until Tera or the user explicitly requests one.
- `PlanningCoordinatorAgent` remains deferred for larger phases or larger projects.

## Read Next Only If Needed

- Latest implementation details: `project-control/tasks/TASK-0009.md`
- Compact official project memory: `project-control/PROJECT_STATE.md`
- Sub-agent health snapshot: `project-control/SUB_AGENT_STATUS.md`
- Active rules: `project-preparation/PROJECT_RULES.md`
- Plan details: `project-preparation/09_IMPLEMENTATION_PLAN.md`
- UI structure: `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md`
- UI style guide: `project-preparation/28_UI_UX_GUIDELINES.md`
- System execution rules: `tera-system/TeraPreExecutionGate.md`

## Update Rules

- Keep this file short and practical.
- Do not copy full logs or long execution notes here.
- Do not store secrets here.
- Update it after each important task or when a phase closes.
