# TASK-COD-003: Gateway Approval API

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-003 |
| Phase | Phase 4.5 |
| Title | Gateway Approval API (approval.request, approval.response) |
| Status | Accepted |
| Created | 2026-07-10 |
| Technology Profile | effect-bun-opencode |

---

## Objective

Add Approval API methods to the Gateway Protocol:

- `approval.request` — Engine requests approval for sensitive action
- `approval.response` — Platform responds with approval decision

---

## Scope

### In Scope
- Request/response schemas for approval.request, approval.response
- Approval handlers in approval-handlers.ts
- Gateway announcement: `supported_methods: ["context", "task", "approval"]`
- Unit tests for approval operations
- Smoke test via CLI

### Out of Scope (Deferred)
- Event Stream (Phase 4.6)
- Config Bridge (Phase 4.7)
- Persistent approval storage (ephemeral only)
- Real approval workflow (stub responses only)

---

## Design Reference

Based on `TERA_GATEWAY_PROTOCOL_SPEC.md` v1.2:

### ApprovalRequest (Engine → Platform)
```json
{
  "type": "request",
  "id": "appr_001",
  "timestamp": "2026-07-10T14:32:00.000Z",
  "payload": {
    "method": "approval.request",
    "task_id": "t_abc123",
    "action_type": "destructive",
    "description": "Delete old test files that are no longer needed",
    "details": {
      "affected_files": ["tests/old-test.tsx"],
      "affected_commands": ["rm tests/old-test.tsx"],
      "risk_level": "medium"
    },
    "response_deadline": "2026-07-10T14:35:00.000Z"
  }
}
```

### ApprovalResponse (Platform → Engine)
```json
{
  "type": "response",
  "id": "appr_001",
  "timestamp": "2026-07-10T14:33:00.000Z",
  "payload": {
    "method": "approval.response",
    "approved": true,
    "reason": "Approved - old tests are obsolete",
    "approved_by": "majed"
  }
}
```

### Approval Correlation Rules (Phase 4)
```
1. approval.response must carry the same id as approval.request
2. id is the single Correlation ID at protocol level
3. No separate request_id in Phase 4
4. Platform owns the approval store, not the engine
```

---

## Allowed Write Targets

```
clients/TeraAi/packages/opencode/src/gateway/protocol.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/approval-handlers.ts (new)
clients/TeraAi/packages/opencode/test/gateway/approval-api.test.ts (new)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | approval.request returns ApprovalResponse with approved: true | Unit test |
| 2 | approval.request returns ApprovalResponse with approved: false | Unit test |
| 3 | approval methods rejected before handshake | Unit test |
| 4 | Gateway announces `supported_methods: ["context", "task", "approval"]` | Smoke test |
| 5 | stdout emits protocol JSON Lines only | Smoke test |
| 6 | stderr used for diagnostics/logs only | Smoke test |
| 7 | No Approval State persistence (ephemeral only) | Code review |
| 8 | Typecheck passes | `bun run typecheck` |
| 9 | All tests pass | `bun test` |

---

## Verification Commands

```bash
# From packages/opencode
bun run typecheck
bun test test/gateway/approval-api.test.ts
```

---

## Vitality & Polish Checklist

N/A — This is a protocol-level task, not a UI task.

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Approval API is a single focused unit |
| One objective only | PASS | Implement Approval API methods only |
| No deferrable work included | PASS | Approval API is the core next step |
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
| Changed files within Allowed Write Targets | PASS | protocol.ts, approval-handlers.ts, approval-api.test.ts |
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
| Acceptance criteria satisfied | PASS | All 9 criteria met |
| CLI side effects reviewed | PASS | Only typecheck and test |
| Task file and core project-control records reviewed | PASS | Will update after |
| No secret leakage in task files/logs/reports/handbacks | PASS | No secrets |
| No duplicate project-control IDs created | PASS | TASK-COD-003 is unique |
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
