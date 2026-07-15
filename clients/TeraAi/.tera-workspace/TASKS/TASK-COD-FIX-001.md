# TASK-COD-FIX-001: Fix plugin type mismatch blocking opencode typecheck

## 1. Task Information

| Field | Value |
|---|---|
| **TASK-ID** | TASK-COD-FIX-001 |
| **Task Type** | Coding / Fix |
| **Phase** | Phase 4 follow-up blocker fix |
| **Build Mode Approved** | Yes — Majed approved on 2026-07-10 |
| **Status** | Accepted / Closed |
| **Assigned To** | EngineeringAgent (planning first) |
| **Created** | 2026-07-10 |
| **Linked Issue** | GAP-0001 |
| **Active Technology Profile** | `effect-bun-opencode` |
| **Design Source Decision** | N/A |
| **UI Acceptance Gate Required** | N/A |

## 2. Objective

Resolve the package typecheck failure in `clients/TeraAi/packages/opencode/src/plugin/index.ts` caused by plugin type mismatch between local workspace plugin types and installed upstream `@opencode-ai/plugin` types.

The purpose is to restore clean `bun run typecheck` from `clients/TeraAi/packages/opencode` before expanding Gateway work.

## 3. Reference Files

- `project-control/ISSUES_AND_GAPS.md` — GAP-0001
- `clients/TeraAi/packages/opencode/src/plugin/index.ts`
- `clients/TeraAi/packages/opencode/package.json`
- `clients/TeraAi/packages/plugin/package.json`
- `clients/TeraAi/packages/plugin/src/index.ts`
- `tera-system/profiles/effect-bun-opencode.md`

## 4. Proposed Investigation Scope

Planning-only read targets:

- `clients/TeraAi/packages/opencode/src/plugin/**`
- `clients/TeraAi/packages/plugin/src/**`
- `clients/TeraAi/packages/opencode/package.json`
- `clients/TeraAi/packages/plugin/package.json`
- package workspace config files if required for dependency/source resolution

## 4.1 Proposed Allowed Write Targets

EngineeringAgent planning recommends a single-file fix:

- `clients/TeraAi/packages/opencode/src/plugin/index.ts`

No other write targets are proposed.

## 5. Forbidden Files / Actions

- Do not modify TASK-COD-001 gateway implementation.
- Do not add dependencies without explicit approval.
- Do not run repo-root tests.
- Do not change plugin runtime behavior unless the typecheck fix requires it and Tera approves.
- Do not alter public plugin API broadly without explicit design note.
- Do not use `any` as a shortcut fix.

## 6. Acceptance Criteria

1. Root cause of plugin type mismatch is identified.
2. Exact minimal write targets are proposed before implementation.
3. Fix plan avoids broad refactor and dependency additions by default.
4. `bun run typecheck` from `clients/TeraAi/packages/opencode` passes after implementation.
5. No regression to TASK-COD-001 scoped test.

## 6.1 Execution Gates

| Gate | Result | Notes |
|---|---|---|
| Orchestration Decision Matrix | Helper Agent | Code fix required; Tera delegates to EngineeringAgent. |
| Model Capability Gate | Current model acceptable with safeguards | Narrow type mismatch investigation and fix. |
| Pre-Execution Gate | PASS | Majed approved Build Mode and single allowed write target. |

## 6.2 CLI / Tool Side Effects

| Command / Tool | Allowed? | Expected Side Effects | Approval Needed? |
|---|---|---|---|
| `bun run typecheck` from `clients/TeraAi/packages/opencode` | Yes after implementation approval | Compile/type verification only | No |
| Scoped tests related to plugin/gateway | Yes after implementation approval | Test output only | No |
| Dependency install/update | No by default | Lock/package changes | Yes |

## 7. TASK-ID Size Check

```md
Requested Work: Fix plugin type mismatch blocking opencode typecheck.
Can it fit one TASK-ID? Yes, if root cause is limited to plugin type source/import mismatch.
Reason: Single verification blocker, single package typecheck target.
Proposed Split:
- Split only if investigation finds broad workspace dependency/version strategy issue.
```

## 8. Planning Handback

```text
Task ID: TASK-COD-FIX-001
Agent: EngineeringAgent
Status: Planning Done / Implementation Pending Approval
Files Created: None
Files Modified: None
Commands Run: Read-only inspection only
Root Cause: `opencode-gitlab-auth` and `opencode-poe-auth` publish plugin types against upstream `@opencode-ai/plugin` and `@opencode-ai/sdk`, while this repo uses local `@tera-system/plugin` and `@tera-system/sdk`. TypeScript treats them as different plugin contracts when inserted into local `PluginInstance[]`.
Minimal Fix Plan: Add a narrow compatibility adapter in `clients/TeraAi/packages/opencode/src/plugin/index.ts`, treating upstream plugin imports as `unknown` at the boundary and validating them through existing runtime function shape before returning local `PluginInstance`. Do not add dependencies, change public API, or use `any`.
Proposed Allowed Write Targets: `clients/TeraAi/packages/opencode/src/plugin/index.ts` only.
Verification Commands: `bun run typecheck`; `bun test test/gateway/context-api.test.ts` from `clients/TeraAi/packages/opencode`.
Risks: Runtime structural compatibility appears likely but should remain narrow. If typecheck reveals additional mismatches outside this file, stop and revise.
Recommendation: Proceed.
No files were modified.
```

## 9. Execution Report / Agent Handback

```text
Task ID: TASK-COD-FIX-001
Agent: EngineeringAgent
Status: DONE
Files Created: None
Files Modified:
- clients/TeraAi/packages/opencode/src/plugin/index.ts
Commands Run:
- bun run typecheck — passed
- bun test test/gateway/context-api.test.ts — passed, 7 pass / 0 fail
- git status --short; git diff -- clients/TeraAi/packages/opencode/src/plugin/index.ts — inspection only
Summary: Added a narrow local compatibility adapter for the two upstream-typed third-party plugins. GitlabAuthPlugin and PoeAuthPlugin pass through an unknown boundary and runtime shape validation before returning local PluginInstance hooks.
Assumptions: Existing unrelated working tree changes were pre-existing and outside this task scope.
Issues or Missing Information: Multiple unrelated modified/untracked files exist in working tree; EngineeringAgent did not modify them.
System Gaps Observed: None.
Recommendation: Ready for Tera review.
```

## 10. Tera Review

| Check | Result | Notes |
|---|---|---|
| TASK objective completed? | PASS | Plugin type mismatch fixed. |
| Output matches approved scope? | PASS | One-file compatibility adapter only. |
| No files outside Allowed Write Targets? | PASS | Only `src/plugin/index.ts` changed for implementation. |
| No forbidden files created? | PASS | No new files/dependencies. |
| No extra libraries added? | PASS | No dependency changes. |
| No secrets or real `.env`? | PASS | No secrets observed. |
| Technology Profile respected? | PASS | `effect-bun-opencode`. |
| UI/UX rules respected if UI exists? | N/A | No UI. |
| UI Acceptance Gate passed if UI exists? | N/A | No UI. |
| Acceptance Criteria passed? | PASS | typecheck and gateway regression test pass. |
| Rollback needed? | No | Fix is narrow and verified. |

## 11. Notes

This task exists because TASK-COD-001 passed scoped tests but package typecheck is blocked by GAP-0001.

## 12. Post-Execution Review Result

| Item | Status |
|---|---|
| Gate Result | PASS |
| Reviewer | Tera |
| Review Date | 2026-07-10 |
| Notes | `bun run typecheck` passes and TASK-COD-001 gateway regression remains green. |

## 13. Final Tera Decision

| Item | Value |
|---|---|
| Final Status | Accepted / Closed |
| Registry Updated | N/A |
| Activity Log Updated | Yes |
| Project State Updated | N/A |
| Issues/Gaps Updated | Yes — GAP-0001 resolved |
| Next Action | Continue Gateway work from clean typecheck baseline. |
