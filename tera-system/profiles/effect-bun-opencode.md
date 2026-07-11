# Technology Profile: effect-bun-opencode

## 1. Profile Identity

- Profile ID: `effect-bun-opencode`
- Language: TypeScript
- Framework: Effect TS / OpenCode-style monorepo architecture
- Database: Not applicable for current task
- ORM: Not applicable
- Package Manager / CLI: Bun
- Default Project Type: TypeScript monorepo engine / CLI / core package work
- Status: Approved — Majed approved for TASK-COD-001 on 2026-07-10

## 2. Applicability

Use this profile when working on the TeraOpenCode engine codebase, especially files under `clients/TeraAi/packages/*` that use TypeScript, Effect services, tool definitions, system-context sources, protocol handling, or Bun-based package workflows.

This profile is intended for the current Phase 4 Context API prototype and similar engine-integration tasks.

## 3. Default Execution Order

1. Read the approved protocol/design documents.
2. Identify the smallest package-level integration point.
3. Add or modify schemas/types before runtime behavior when needed.
4. Implement the minimal runtime path.
5. Register only the minimal exported surface required by the task.
6. Run package-local checks only, not repo-root tests.
7. Report changed files, commands, and verification results.

## 4. First Task Rule

The safest first implementation task is a narrow, non-state-changing prototype that validates protocol shape and message handling without adding Task API, Approval API, Event Stream, Config Bridge, or persistent platform state.

For Phase 4.2, the first allowed implementation is: Handshake + ContextRequest + ContextResponse over JSON Lines stdio, scoped to a prototype entrypoint or gateway module.

## 5. Scaffold Rules

- Do not scaffold a new application.
- Do not add a new framework.
- Do not add runtime dependencies unless explicitly required and approved.
- Prefer existing repository patterns for TypeScript modules, exports, schemas, and service registration.
- Keep files close to the relevant package boundary.

## 6. ORM / Database Rules

Not applicable for the current engine gateway prototype.

Forbidden by default:
- Database schema changes
- Migrations
- ORM configuration
- Persistent task/session storage

## 7. CLI Side Effects

Allowed commands must be package-local and explicitly listed in the task file.

Common allowed verification commands:
- `bun run typecheck` from the relevant package directory when available
- package-local tests if a test script exists and is scoped

Forbidden by default unless separately approved:
- repo-wide destructive cleanup
- dependency installation
- publishing
- migration or database commands
- force push / amend / reset
- starting long-running servers unless the task explicitly requires it

## 8. Forbidden Defaults

Do not create by default:
- Task API
- Approval API
- Event Stream
- REST API
- Config Bridge
- persistent platform state
- new agent files
- broad refactors
- unrelated renames
- new dependencies

## 9. Pre-Execution Gate Additions

Before delegation:

- Confirm `TERA_GATEWAY_PROTOCOL_SPEC.md` version and status.
- Confirm the task is limited to Context API prototype.
- Confirm stdout/stderr rules are included in acceptance criteria.
- Confirm no `.tera-workspace/` writes are allowed.
- Confirm `read_tera_workspace` remains fallback only and is not removed.
- Confirm allowed write targets are package-specific.
- Confirm no Task State changes are introduced.

## 10. Acceptance Criteria Patterns

For the Phase 4.2 Context API prototype:

1. Handshake accepts compatible `contract_version` and rejects incompatible versions.
2. ContextRequest is accepted only after successful handshake.
3. ContextResponse uses the same correlation `id` as ContextRequest.
4. stdout emits protocol JSON Lines only.
5. stderr is used for diagnostics/logs only.
6. Oversized context is rejected according to `MESSAGE_TOO_LARGE` behavior.
7. No Task API, Approval API, Event Stream, Config Bridge, or platform state mutation is added.
8. Typecheck or scoped verification passes.
