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
| Current Phase | Phase 3 complete ✅ (Auth + Security Review); Phase 4 prep complete ✅ |
| Next Task | `TASK-0007` — SEC fixes + Banks Screen (S03) |
| Current Lead Agent | `Tera Agent` |
| Expected Execution Agent | `EngineeringAgent` |

## Last Closed Tasks

- `TASK-0001` — Scaffold Next.js + TypeScript + Prisma + `.env.example`
- `TASK-0002` — Prisma data models
- `TASK-0003` — Local database creation + first Prisma migration
- `TASK-0004` — Authentication + Roles (login, JWT, middleware, seed)
- `TASK-0005` — SecurityAgent independent security review of TASK-0004 (PASS ✅, Closed)
- `TASK-0006` — Preparation: workflow-rules.md + screen-spec-s03.md
- `TASK-0006` — Preparation: workflow-rules.md + screen-spec-s03.md (Closed ✅)
- `ISSUE-0004` — SEC-001 (User Enumeration) planned fix within TASK-0007
- `ISSUE-0005` — SEC-002 (Logout cookie flags) planned fix within TASK-0007

## Active Rules You Must Not Miss

- Read `project-preparation/PROJECT_RULES.md` before creating, approving, or delegating any new task.
- `Pre-Execution Gate: PASS` is required before any implementation delegation.
- `Post-Execution Review Gate` is required before `Accepted` or `Closed`.
- Real secrets are allowed only in approved local environment files or local environment variables.
- Never write real secrets into task files, logs, handbacks, or config/code fallback values.
- Post-execution review must also inspect the core `project-control/` files, not only application code.
- If a secret appears in any task file, log, report, or control record, the task cannot PASS review.
- After each implementation task, decide whether `ProjectControlAgent`, `SecurityAgent`, or `QAAndAcceptanceAgent` must perform an independent follow-up review.
- Project-control IDs must stay unique and sequential.
- Use only the smallest relevant context for the current task.

## Active Decisions / Important Notes

- `DEC-0004` — Secret handling and redaction rules are active across the system.
- `DEC-0005` — `TERA_ACTIVE_CONTEXT.md` is now the first startup file for ongoing sessions.
- `DEC-0006` — SecurityAgent independent review required before next phase ✅ (fulfilled — TASK-0005 complete).
- `DEC-0007` — Post-execution review must inspect core `project-control` files and include an explicit independent-review decision when relevant.
- `ISSUE-0003` — Secret exposure incident from TASK-0003 was resolved; do not reintroduce secrets into docs or config fallbacks.

## Available Sub-Agents

| Agent | Use |
|---|---|
| `EngineeringAgent` | Implementation tasks inside `checks-management/` after gate approval |
| `ProjectControlAgent` | Updating task/log/decision/issue records under Tera direction |
| `SecurityAgent` | Independent follow-up review for Auth, Secrets, Permissions, Middleware, and Config tasks |
| `QAAndAcceptanceAgent` | Independent follow-up review for UI, Workflow, and acceptance checks |
| `RequirementsScopeAgent` | Scope review only when scope changes |
| `BusinessWorkflowAgent` | Workflow review only when business process changes |
| `DataDesignAgent` | Data review only when schema/business data design changes |
| `UIUXStructureAgent` | UI structure review only when screen/design scope changes |

## Read Next Only If Needed

- Current task details: `project-control/tasks/TASK-0006.md` (Preparation) or next task `project-control/tasks/TASK-0007.md`
- Compact official project memory: `project-control/PROJECT_STATE.md`
- Active rules: `project-preparation/PROJECT_RULES.md`
- Plan details: `project-preparation/09_IMPLEMENTATION_PLAN.md`
- Auth/roles requirements: `project-preparation/04_USERS_ROLES_PERMISSIONS.md`
- UI/login guidance: `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md`
- UI style guide: `project-preparation/28_UI_UX_GUIDELINES.md`
- System execution rules: `tera-system/TeraPreExecutionGate.md`

## Update Rules

- Keep this file short and practical.
- Do not copy full logs or long execution notes here.
- Do not store secrets here.
- Update it after each important task or when a phase closes.
