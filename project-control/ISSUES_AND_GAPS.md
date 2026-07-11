# ISSUES_AND_GAPS.md

> **Purpose:** Track open issues, gaps, and risks during the project.

## Required Gap Format

```md
## GAP-XXXX - [Short Title]

- Source Task:
- Discovered By:
- Severity: Critical / High / Medium / Low
- Status: Open / Deferred / In Progress / Resolved / Cancelled
- Description:
- Impact:
- Recommended Action:
- Target Task / Phase:
- Owner:
```

**Severity:** Critical / High / Medium / Low
**Status:** Open / Deferred / In Progress / Resolved / Cancelled

**Phase 7 Rule:** No hidden open issues. Any unresolved issue must be documented as Resolved / Deferred / Won't Fix / Requires TASK-COD-FIX before project closure.

## Open Issues and Gaps

## GAP-0001 - Package typecheck blocked by plugin type mismatch

- Source Task: TASK-COD-001
- Discovered By: EngineeringAgent / Tera Review
- Severity: Medium
- Status: Resolved
- Description: `bun run typecheck` from `clients/TeraAi/packages/opencode` fails in `src/plugin/index.ts` due a plugin type mismatch between local `packages/plugin/src/index` and installed `@opencode-ai/plugin` types.
- Impact: TASK-COD-001 scoped tests pass, but package-wide typecheck cannot be used as a clean acceptance signal until this unrelated issue is fixed.
- Recommended Action: Resolved by `TASK-COD-FIX-001`; keep fix narrow and do not expand TASK-COD-001.
- Target Task / Phase: TASK-COD-FIX-001 / Phase 4 follow-up before broader Gateway work
- Owner: Tera / EngineeringAgent after separate approval

### Resolution

- Resolved By: TASK-COD-FIX-001
- Resolution Date: 2026-07-10
- Verification:
  - `bun run typecheck` from `clients/TeraAi/packages/opencode` — PASS
  - `bun test test/gateway/context-api.test.ts` — PASS, 7/7
