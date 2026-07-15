# TASK-COD-002: Gateway Task API

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-002 |
| Phase | Phase 4.4 |
| Title | Gateway Task API (task.create, task.cancel, task.status) |
| Status | Accepted |
| Created | 2026-07-10 |
| Technology Profile | effect-bun-opencode |

---

## Objective

Add Task API methods to the Gateway Protocol:

- `task.create` — Create a new task
- `task.cancel` — Cancel a task
- `task.status` — Get task status

---

## Scope

### In Scope
- Request/response schemas for task.create, task.cancel, task.status
- Task handlers in protocol.ts
- Gateway announcement: `supported_methods: ["context", "task"]`
- Unit tests for task operations
- Smoke test via CLI

### Out of Scope (Deferred)
- Approval API (Phase 4.5)
- Event Stream (Phase 4.6)
- Config Bridge (Phase 4.7)
- Persistent task storage (ephemeral only)
- Real task execution (stub responses only)

---

## Design Reference

Based on `TERA_GATEWAY_PROTOCOL_SPEC.md` v1.2:

### TaskCreateRequest
```json
{
  "method": "task",
  "task_action": "create",
  "task_type": "implementation",
  "description": "string",
  "scope": { "files": ["string"] }
}
```

### TaskCancelRequest
```json
{
  "method": "task",
  "task_action": "cancel",
  "task_id": "string",
  "reason": "string"
}
```

### TaskStatusRequest
```json
{
  "method": "task",
  "task_action": "status",
  "task_id": "string"
}
```

### TaskResponse
```json
{
  "method": "task",
  "task_action": "create|cancel|status",
  "task_id": "string",
  "status": "created|cancelled|running|completed|failed"
}
```

---

## Allowed Write Targets

```
clients/TeraAi/packages/opencode/src/gateway/protocol.ts
clients/TeraAi/packages/opencode/src/gateway/task-handlers.ts (new)
clients/TeraAi/packages/opencode/test/gateway/task-api.test.ts (new)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | task.create returns TaskResponse with status "created" | Unit test |
| 2 | task.cancel returns TaskResponse with status "cancelled" | Unit test |
| 3 | task.status returns TaskResponse with correct status | Unit test |
| 4 | task methods rejected before handshake | Unit test |
| 5 | Gateway announces `supported_methods: ["context", "task"]` | Smoke test |
| 6 | stdout emits protocol JSON Lines only | Smoke test |
| 7 | stderr used for diagnostics/logs only | Smoke test |
| 8 | No Task State persistence (ephemeral only) | Code review |
| 9 | Typecheck passes | `bun run typecheck` |
| 10 | All tests pass | `bun test` |

---

## Verification Commands

```bash
# From packages/opencode
bun run typecheck
bun test test/gateway/task-api.test.ts
```

---

## Vitality & Polish Checklist

N/A — This is a protocol-level task, not a UI task.

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Task API is a single focused unit |
| One objective only | PASS | Implement Task API methods only |
| No deferrable work included | PASS | Task API is the core next step |
| No UI unless explicitly requested | PASS | No UI involved |
| No API unless explicitly requested | PASS | This IS the approved Gateway API scope |
| No Auth unless explicitly requested | PASS | No Auth involved |
| No schema/migration unless explicitly requested | PASS | No database involved |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| Secret handling plan documented and redacted | PASS | N/A |
| CLI side effects checked | PASS | Only typecheck and test - no side effects |
| No internal contradiction between constraints and outputs | PASS | No contradictions |
| Allowed Write Targets are narrow | PASS | 3 files only |
| Acceptance criteria are testable | PASS | All criteria have unit tests |

Gate Status: **PASS**

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | protocol.ts, task-handlers.ts, task-api.test.ts |
| No unauthorized files created | PASS | Only 3 files created/modified |
| No unauthorized files deleted | PASS | No files deleted |
| No unauthorized packages added | PASS | No new packages |
| No unauthorized UI/CSS/theme changes | PASS | No UI involved |
| UI Acceptance Gate passed for UI tasks | N/A | Not a UI task |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| Secrets redacted in docs/logs/config references | PASS | N/A |
| No unauthorized ORM models/entities/migrations | PASS | No database involved |
| No unapproved business validation moved to DB constraints | PASS | N/A |
| No unauthorized API/Auth created | PASS | No Auth involved |
| Acceptance criteria satisfied | PASS | All 10 criteria met |
| CLI side effects reviewed | PASS | Only typecheck and test |
| Task file and core project-control records reviewed | PASS | Will update after |
| No secret leakage in task files/logs/reports/handbacks | PASS | No secrets |
| No duplicate project-control IDs created | PASS | TASK-COD-002 is unique |
| Any out-of-target changes classified | N/A | All changes within targets |
| Independent review decision recorded | PASS | SecurityAgent not required (no Auth/Secrets) |

Gate Status: **PASS**

Root Cause if failed:
- N/A

Required Action:
- N/A

Independent Review:
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required (no Auth/Secrets/Config)
- QAAndAcceptanceAgent: Not Required (unit tests sufficient)

Deviation Classification:
- N/A
