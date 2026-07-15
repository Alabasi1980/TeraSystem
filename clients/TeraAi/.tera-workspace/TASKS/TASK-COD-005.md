# TASK-COD-005: Gateway Integration Tests + Documentation

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-005 |
| Phase | Phase 4.7 |
| Title | Gateway Integration Tests and API Documentation |
| Status | Accepted |
| Created | 2026-07-12 |
| Technology Profile | effect-bun-opencode |

---

## Objective

Complete Phase 4.7 requirements:
1. Add integration tests for Gateway CLI (end-to-end stdin/stdout)
2. Add Gateway API reference documentation
3. All existing tests continue to pass

---

## Scope

### In Scope
- Integration tests that spawn `tera gateway` CLI process and test full protocol flow
- Gateway API reference documentation (`.tera-workspace/GATEWAY_API_REFERENCE.md`)
- Update `supported_methods` assertions across all existing tests

### Out of Scope (Deferred)
- Event Stream (future)
- Config Bridge (future)
- Persistent state (ephemeral only)
- Real task execution (stub only)

---

## Design Requirements

### Integration Tests
Create `test/gateway/gateway-integration.test.ts` with tests that:
1. Spawn `bun run --conditions=browser ./src/index.ts gateway` as a child process
2. Send handshake via stdin
3. Read response from stdout
4. Send context request
5. Read response
6. Send task request
7. Read response
8. Send approval request
9. Read response
10. Verify all responses are valid JSON Lines
11. Verify stderr is used only for diagnostics

### Documentation
Create `.tera-workspace/GATEWAY_API_REFERENCE.md` with:
1. Overview of Gateway protocol
2. Transport layer (stdio JSON Lines)
3. Message envelope format
4. Handshake method
5. Context method
6. Task methods (create, cancel, status)
7. Approval methods (request, response)
8. Error codes
9. Supported methods list
10. Message size limits

---

## Allowed Write Targets

```
clients/TeraAi/packages/opencode/test/gateway/gateway-integration.test.ts (new)
clients/TeraAi/.tera-workspace/GATEWAY_API_REFERENCE.md (new)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | Integration test spawns gateway process successfully | Integration test |
| 2 | Integration test sends handshake and receives response | Integration test |
| 3 | Integration test sends context and receives response | Integration test |
| 4 | Integration test sends task and receives response | Integration test |
| 5 | Integration test sends approval and receives response | Integration test |
| 6 | All responses are valid JSON Lines | Integration test |
| 7 | Gateway API reference documents all methods | Code review |
| 8 | All existing unit tests still pass (27/27) | `bun test` |
| 9 | Typecheck passes | `bun run typecheck` |

---

## Verification Commands

```bash
# From packages/opencode
bun run typecheck
bun test test/gateway/
```

---

## Vitality & Polish Checklist

N/A — This is a test/documentation task, not a UI task.

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Tests + docs, single phase step |
| One objective only | PASS | Gateway validation + documentation |
| No deferrable work included | PASS | Required for Phase 4 closure |
| No UI unless explicitly requested | PASS | No UI involved |
| No API unless explicitly requested | PASS | No new API methods |
| No Auth unless explicitly requested | PASS | No Auth involved |
| No schema/migration unless explicitly requested | PASS | No database involved |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| Secret handling plan documented and redacted | PASS | N/A |
| CLI side effects checked | PASS | Only typecheck and test |
| No internal contradiction between constraints and outputs | PASS | No contradictions |
| Allowed Write Targets are narrow | PASS | 2 files only |
| Acceptance criteria are testable | PASS | All criteria have tests |

Gate Status: **PASS**

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | gateway-integration.test.ts, GATEWAY_API_REFERENCE.md |
| No unauthorized files created | PASS | Only 2 files created |
| No unauthorized files deleted | PASS | No files deleted |
| No unauthorized packages added | PASS | No new packages |
| No unauthorized UI/CSS/theme changes | PASS | No UI involved |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| Acceptance criteria satisfied | PASS | All 9 criteria met |
| CLI side effects reviewed | PASS | Only typecheck and test |
| Task file and core project-control records reviewed | PASS | Will update after |
| No secret leakage in task files/logs/reports/handbacks | PASS | No secrets |
| No duplicate project-control IDs created | PASS | TASK-COD-005 is unique |

Gate Status: **PASS**

Root Cause if failed:
- N/A

Required Action:
- N/A

Independent Review:
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required
- QAAndAcceptanceAgent: Not Required (all tests pass)

Deviation Classification:
- N/A
