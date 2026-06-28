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
| Current Phase | **MVP Complete ✅ — all 5 screens done** (Banks S03, Parties S04, Checks S02, Users S05, Nav Hub) + system-maintenance sync complete + model-capability gate active |
| Next Task | Handover documentation or roadmap review |
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
- `TASK-0011` - ISSUE-0006 fix — server-side validation/normalization in Banks/Parties actions
- `TASK-0012` - Checks Screen S02 — actions.ts (9 Server Actions) + page.tsx (full UI) + nav activation
- `TASK-0013` - ISSUE-0007 fix — NaN amount validation guard in check actions
- `TASK-0014` - ISSUE-0008 fix — invalid date crash in check actions
- `TASK-0015` - Users Screen S05 — actions.ts + page.tsx + nav activation **✅ FINAL MVP SCREEN**
- `TASK-0016` - Tera System Maintenance Upgrade — lifecycle, decision matrix, security sensitivity, handoff readiness
- `TASK-0017` - Tera System Maintenance Upgrade — Model Capability Gate and pre-execution model suitability rules
- `TASK-0018` - Tera System Maintenance Upgrade — roadmap tracking and PlanComplianceReviewAgent

## Recent System Changes

- `2026-06-27`: EngineeringAgent split into EngineeringAgent (Backend/Logic) + FrontendAgent (UI/Styling)
- `2026-06-27`: Unified sub-agent lifecycle: draft first, activate on need, restart after activation
- `2026-06-27`: Minimal support layer added: `ProjectControlAgent` + `ExecutionPreparationAgent`
- `2026-06-27`: TASK-0008 (Parties S04) + TASK-0009 (Main Nav Hub) completed. requireAdmin() extended to Parties. Root page replaced with RTL navigation hub.
- `2026-06-28`: `QualityReviewCoordinatorAgent` added and activated for periodic cross-domain quality reviews before larger phases, before release, or when quality debt signals appear.
- `2026-06-28`: Tera role refined as `Primary Project Orchestrator / Decision Owner` with explicit helper-agent trigger rules and anti-over-delegation rules.
- `2026-06-28`: Tera system maintenance synced lifecycle ordering, decision-matrix discipline, escalation ladder, security sensitivity levels, handoff readiness gate, and active/generated agent verification rules.
- `2026-06-28`: `Model Capability Gate` added. Before execution, Tera now assesses whether the current model is sufficient, sufficient with safeguards, requires escalation, or should be replaced/split for safer execution.
- `2026-06-28`: `PROJECT_MASTER_PLAN.md` and `PROJECT_DETAILED_EXECUTION_PLAN.md` added as mandatory roadmap-tracking files for medium/large projects. `PlanComplianceReviewAgent` activated for roadmap-compliance reviews after phases, major task batches, or before MVP acceptance.
- `2026-06-28`: TASK-0011 closed. ISSUE-0006 resolved; `npm run build` PASS. SecurityAgent and QAAndAcceptanceAgent not required for this backend-only validation fix per Tera decision.
- `2026-06-28`: TASK-0012 (Checks Screen S02) completed. 3 sub-tasks: EngineeringAgent (actions.ts), FrontendAgent (page.tsx + nav). Build PASS. SecurityAgent review found ISSUE-0007 (NaN bypass) and ISSUE-0008 (date crash). QAAndAcceptanceAgent review PASS with minor notes. TASK-0012 submitted, awaiting user acceptance.
- `2026-06-28`: TASK-0013 (ISSUE-0007 NaN fix) + TASK-0014 (ISSUE-0008 date fix) closed. Both fixes were simple validation guards in `app/checks/actions.ts` with no side effects. ISSUE-0007 → Resolved, ISSUE-0008 → Resolved.
- `2026-06-28`: TASK-0015 (Users Screen S05) accepted — **last MVP screen**. EngineeringAgent + FrontendAgent completed all 3 sub-tasks. Users CRUD with activation/deactivation, self-protection, password hashing. Build PASS. **MVP COMPLETE ✅**

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
- `PlanComplianceReviewAgent` may review roadmap compliance and flag drift or contradictions, but only Tera decides phase status, corrective tasks, or acceptance.
- Apply the Orchestration Decision Matrix before `Pre-Execution Gate`.
- `Security Sensitivity Levels` are decided before delegation; `Independent Review Decision` is confirmed after execution.
- `Model Capability Gate` is active: after orchestration planning and before `Pre-Execution Gate`, Tera assesses whether the current model is sufficient, acceptable with safeguards, needs escalation, or whether the task should be split first.
- `Handoff Readiness Gate` is only for phase/release/documentation handoff readiness, not normal internal task handbacks.
- `PROJECT_MASTER_PLAN.md` and `PROJECT_DETAILED_EXECUTION_PLAN.md` are mandatory roadmap-tracking files when they exist; read them before selecting the next major task and keep their statuses updated.
- Do not treat `Deferred`, `Cancelled`, `Out of Scope`, or `Moved to Later Phase` roadmap items as missing work.
- Tera must explicitly decide whether `SecurityAgent` is needed for `Auth`, `JWT`, `Cookies`, `Middleware/Proxy`, `API Routes`, `Server Actions`, `Permissions`, `Role checks`, `Data Mutations`, `Secrets`, or `Config`.
- Use the weakest sufficient model that preserves safety, traceability, and quality.
- Do not ask the user about stronger models for every small task; escalate only when risk, prior failure, criticality, verification difficulty, or explicit cost-control preference justifies it.
- Do not route every small task through a long helper-agent chain unless there is a clear trigger.
- Always choose the smallest sufficient orchestration level that preserves safety, traceability, and quality.
- Before modifying any file inside `.opencode/agents/`, verify the active version actually exists.
- Sub-agents must not create, activate, modify, or delegate to other sub-agents unless Tera explicitly assigns that in a system-level task.
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
- `DEC-0015` - Orchestration Decision Matrix, Security Sensitivity Levels, Handoff Readiness Gate, and escalation/verification rules are now active.
- `DEC-0016` - `Model Capability Gate` is now active before `Pre-Execution Gate` for pre-execution model suitability assessment.
- `DEC-0017` - Mandatory roadmap tracking via `PROJECT_MASTER_PLAN.md` + `PROJECT_DETAILED_EXECUTION_PLAN.md`, plus `PlanComplianceReviewAgent` for roadmap-compliance review.
- `SUB_AGENT_STATUS.md` is the lightweight manager review for sub-agent usage, load, quality, and update need.
- `ISSUE-0006` - Resolved in TASK-0011: server-side validation/normalization added to Banks/Parties actions before Checks S02.
- `ISSUE-0007` - Resolved in TASK-0013: NaN amount bypass fixed — `isNaN()` guard added to check validation.
- `ISSUE-0008` - Resolved in TASK-0014: invalid date crash fixed — explicit `Date` parsing with validation.
- `TASK-0015` - Accepted ✅: Users Screen S05 completed — final MVP screen.
- `ISSUE-0003` - Secret exposure incident from TASK-0003 was resolved; do not reintroduce secrets into docs or config fallbacks.

## Available Sub-Agents

| Agent | Use |
|---|---|
| `EngineeringAgent` | Backend/Logic: actions.ts, Prisma, Auth, Middleware, DB, Config |
| `FrontendAgent` | UI/Styling: page.tsx, components, CSS, RTL |
| `ProjectControlAgent` | Updating task/log/decision/issue records under Tera direction |
| `ExecutionPreparationAgent` | Preparing task packages before Tera review, gating, and delegation |
| `QualityReviewCoordinatorAgent` | Coordinating periodic quality reviews across UI, engineering, security, QA, and documentation |
| `PlanComplianceReviewAgent` | Reviewing execution progress against the approved master and detailed plans |
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
- `PlanComplianceReviewAgent` is available now, but should be used for phase/MVP roadmap compliance review, not after every small task.
- `PlanningCoordinatorAgent` remains deferred for larger phases or larger projects.

## Read Next Only If Needed

- Latest implementation details: `project-control/tasks/TASK-0017.md`
- Compact official project memory: `project-control/PROJECT_STATE.md`
- Sub-agent health snapshot: `project-control/SUB_AGENT_STATUS.md`
- Active rules: `project-preparation/PROJECT_RULES.md`
- Plan details: `project-preparation/09_IMPLEMENTATION_PLAN.md`
- Roadmap overview: `project-control/PROJECT_MASTER_PLAN.md`
- Roadmap details: `project-control/PROJECT_DETAILED_EXECUTION_PLAN.md`
- UI structure: `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md`
- UI style guide: `project-preparation/28_UI_UX_GUIDELINES.md`
- System execution rules: `tera-system/TeraPreExecutionGate.md`

## Update Rules

- Keep this file short and practical.
- Do not copy full logs or long execution notes here.
- Do not store secrets here.
- Update it after each important task or when a phase closes.
