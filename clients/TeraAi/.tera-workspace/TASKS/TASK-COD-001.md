# TASK-COD-001: Phase 4.2 Context API Prototype

## 1. Task Information

| Field | Value |
|---|---|
| **TASK-ID** | TASK-COD-001 |
| **Task Type** | Coding |
| **Phase** | Phase 4.2 — Engine Gateway Context API Prototype |
| **Build Mode Approved** | Yes — Majed approved on 2026-07-10 |
| **Status** | Accepted with external follow-up issue |
| **Assigned To** | EngineeringAgent (after gate PASS only) |
| **Created** | 2026-07-10 |
| **Linked Plan Item** | `clients/TeraAi/.tera-workspace/PLANS/06-phase4.2-context-api-limited-design.md` |
| **Linked Batch** | N/A — single-task prototype |
| **Active Technology Profile** | `effect-bun-opencode` (Approved) |
| **Design Source Decision** | N/A |
| **UI Acceptance Gate Required** | N/A |

## 2. Objective

Implement the smallest safe prototype of the Engine Gateway Context API using stdio JSON Lines:

1. Handshake request/response.
2. ContextRequest.
3. ContextResponse.
4. Protocol-only stdout and diagnostic-only stderr.

This task must not implement Task API, Approval API, Event Stream, Config Bridge, or any TeraSystem task-state mutation.

## 3. Reference Files

- `clients/TeraAi/.tera-workspace/TERA_GATEWAY_PROTOCOL_SPEC.md`
- `clients/TeraAi/.tera-workspace/TERA_ENGINE_CONTRACT.md`
- `clients/TeraAi/.tera-workspace/PLANS/06-phase4.2-context-api-limited-design.md`
- `tera-system/profiles/effect-bun-opencode.md`

## 4. Allowed Write Targets

EngineeringAgent planning handback recommends an opencode-only implementation. These write targets are proposed for approval:

- `clients/TeraAi/packages/opencode/src/gateway/protocol.ts`
- `clients/TeraAi/packages/opencode/src/gateway/stdio.ts`
- `clients/TeraAi/packages/opencode/src/cli/cmd/gateway.ts`
- `clients/TeraAi/packages/opencode/src/index.ts`
- `clients/TeraAi/packages/opencode/test/gateway/context-api.test.ts`
- Optional only if needed: `clients/TeraAi/packages/opencode/test/lib/cli-process.ts`

No `packages/core/**` writes are approved for this prototype.

## 5. Forbidden Files / Actions

- Do not write to `.tera-workspace/` from engine code.
- Do not implement Task API.
- Do not implement Approval API.
- Do not implement Event Stream.
- Do not implement Config Bridge.
- Do not add persistent platform/task/session state.
- Do not add dependencies without approval.
- Do not modify database, migrations, auth, or deployment files.
- Do not alter git remotes, branches, or push behavior.

## 6. Acceptance Criteria

1. Handshake accepts compatible `contract_version = 1.2`.
2. Handshake rejects incompatible contract versions with structured protocol error.
3. ContextRequest is accepted only after successful handshake.
4. ContextResponse returns the same correlation `id` as ContextRequest.
5. stdout is JSON Lines protocol output only.
6. stderr is diagnostics/logging only.
7. Oversized ContextRequest follows `MESSAGE_TOO_LARGE` behavior.
8. No Task API / Approval API / Event Stream / Config Bridge is added.
9. Scoped typecheck or relevant verification passes.
10. Handback lists all changed files and commands run.

## 6.1 Execution Gates

| Gate | Result | Notes |
|---|---|---|
| Orchestration Decision Matrix | Helper Agent | Code required; Tera must delegate to EngineeringAgent. |
| Model Capability Gate | Current model acceptable with safeguards | Narrow prototype, clear protocol, limited files. |
| Pre-Execution Gate | PASS | Majed approved `effect-bun-opencode`, Build Mode, and proposed write targets. |

## 6.2 CLI / Tool Side Effects

| Command / Tool | Allowed? | Expected Side Effects | Approval Needed? |
|---|---|---|---|
| package-local `bun run typecheck` | Yes after delegation | Read/compile verification only | No |
| package-local tests | Yes if scoped | Test output only | No |
| dependency installation | No | Modifies lock/package files | Yes |
| repo-wide commands | No | Broad side effects | Yes |

## 6.3 UI / Frontend Requirements

N/A — no UI work.

## 7. TASK-ID Size Check

```md
Requested Work: Handshake + ContextRequest + ContextResponse prototype only.
Can it fit one TASK-ID? Yes.
Reason: It is the smallest useful Gateway proof, excludes Task API/Approval/Event Stream.
Proposed Split:
- Not needed unless EngineeringAgent discovers package boundary conflict.
```

## 8. Sub-Agent Output Review

| Item | Result |
|---|---|
| Output is actionable | Yes |
| Files reviewed or modified are listed | Yes — recommended write targets listed above |
| Completed work is explicit | Yes — planning only |
| Constraints or risks are stated | Yes |
| Maps to acceptance criteria | Yes |
| Stayed within TASK-ID scope | Yes |
| Acceptance Decision | Accept planning handback |
| Rejection Reasons | N/A |

## 9. Execution Report / Agent Handback

```text
Task ID: TASK-COD-001
Agent: EngineeringAgent
Status: NEEDS_REVIEW
Files Created:
- clients/TeraAi/packages/opencode/src/gateway/protocol.ts
- clients/TeraAi/packages/opencode/src/gateway/stdio.ts
- clients/TeraAi/packages/opencode/src/cli/cmd/gateway.ts
- clients/TeraAi/packages/opencode/test/gateway/context-api.test.ts
Files Modified:
- clients/TeraAi/packages/opencode/src/index.ts
Commands Run:
- bun test test/gateway/context-api.test.ts — final PASS, 7 tests passed
- bun run typecheck — FAIL due existing/unrelated src/plugin/index.ts plugin type mismatch outside allowed write targets
Summary: Implemented minimal stdio JSON Lines Gateway prototype with handshake, context request/response, stdout/stderr separation, MESSAGE_TOO_LARGE behavior, and supported_methods ["context"] only.
Assumptions: Prototype engine_version is 0.1.0. Context is acknowledged only; no durable context/task/session state is persisted.
Issues or Missing Information: Package typecheck blocked by pre-existing/unrelated src/plugin/index.ts issue outside TASK-COD-001 scope.
Decisions Needed from Tera: Review implementation diff and decide whether scoped PASS is sufficient or create a separate fix task for plugin typecheck.
Recommendation: Review implementation; separately route plugin typecheck issue if confirmed unrelated.
```

### 9.1 EngineeringAgent Acceptance Criteria Results

| # | Result | Notes |
|---|---|---|
| 1 | PASS | Handshake accepts contract_version 1.2 |
| 2 | PASS | Incompatible versions rejected with structured protocol output |
| 3 | PASS | Context before handshake rejected |
| 4 | PASS | ContextResponse preserves request id |
| 5 | PASS | stdout writer emits protocol JSON Lines only |
| 6 | PASS | diagnostics go to stderr only |
| 7 | PASS | oversized ContextRequest returns MESSAGE_TOO_LARGE |
| 8 | PASS | no Task API / Approval API / Event Stream / Config Bridge added |
| 9 | PARTIAL | scoped test passes; typecheck fails on unrelated plugin issue |
| 10 | PASS | changed files and commands listed |

## 10. Tera Review

| Check | Result | Notes |
|---|---|---|
| TASK objective completed? | PASS | Handshake + ContextRequest + ContextResponse implemented. |
| Output matches approved scope? | PASS | No Task API, Approval API, Event Stream, or Config Bridge. |
| No files outside Allowed Write Targets? | PASS | Code changes are within approved opencode targets. Control/profile docs updated by Tera. |
| No forbidden files created? | PASS | No `.tera-workspace/` engine writes; no `packages/core/**` changes. |
| No extra libraries added? | PASS | No dependency additions reported or observed. |
| No secrets or real `.env`? | PASS | No secrets observed. |
| Technology Profile respected? | PASS | `effect-bun-opencode` respected. |
| UI/UX rules respected if UI exists? | N/A | No UI task. |
| UI Acceptance Gate passed if UI exists? | N/A | No UI task. |
| Acceptance Criteria passed? | PASS_WITH_EXTERNAL_ISSUE | Scoped tests pass; package typecheck fails due unrelated plugin mismatch. |
| Rollback needed? | No | Prototype is contained and scoped. |

## 11. Notes

Majed approved the Technology Profile, Build Mode, and proposed write targets on 2026-07-10. Ready for EngineeringAgent implementation.

## 12. Post-Execution Review Result

| Item | Status |
|---|---|
| Gate Result | PASS_WITH_EXTERNAL_ISSUE |
| Reviewer | Tera |
| Review Date | 2026-07-10 |
| Notes | Scoped gateway tests pass. Package typecheck failure documented as GAP-0001 because it occurs in `src/plugin/index.ts`, outside TASK-COD-001 allowed write targets. |

## 13. Final Tera Decision

| Item | Value |
|---|---|
| Final Status | Accepted with external follow-up issue |
| Registry Updated | Pending |
| Activity Log Updated | Yes |
| Project State Updated | N/A |
| Issues/Gaps Updated | Yes — GAP-0001 |
| Next Action | Decide whether to create separate TASK-COD-FIX for plugin typecheck before broader Gateway work. |
