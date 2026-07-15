# TASK-COD-006: Workspace Registry

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-006 |
| Phase | Phase 5.1 |
| Title | Workspace Registry —WorkspaceRecord + WorkspaceStore |
| Status | Accepted |
| Created | 2026-07-12 |
| Technology Profile | effect-bun-opencode |

---

## Objective

Create the Workspace Registry inside the Gateway:

- `WorkspaceStore` — in-memory registry of active workspaces
- `WorkspaceRecord` — data model for each workspace
- Gateway methods: `workspace.list`, `workspace.status`
- Bind workspace_id from Handshake to WorkspaceRecord

---

## Scope

### In Scope
- `workspace-registry.ts` — WorkspaceRecord type + WorkspaceStore class (in-memory Map)
- `workspace-handlers.ts` — workspace.list, workspace.status handlers
- Gateway routing for `workspace.*` methods
- Update `supported_methods` to include "workspace"
- Unit tests for workspace registry operations

### Out of Scope (Deferred)
- Per-workspace TaskStore isolation (TASK-COD-008)
- Multi-Client File System Isolation (TASK-COD-009)
- Workspace lifecycle cleanup (TASK-COD-010)
- read_tera_workspace removal

---

## Design Reference

See `.tera-workspace/PLANS/07-phase5-workspace-management-design.md`

### WorkspaceRecord
```typescript
interface WorkspaceRecord {
  id: string
  projectId: string
  directory: string
  createdAt: string
  lastActiveAt: string
  status: "active" | "idle" | "closed"
}
```

### Gateway Methods

#### workspace.list
```json
{
  "method": "workspace.list",
  "action": "list"
}
→ {
  "method": "workspace.list",
  "workspaces": [{ "id": "ws_abc", "projectId": "...", "status": "active" }]
}
```

#### workspace.status
```json
{
  "method": "workspace.status",
  "action": "status",
  "workspace_id": "ws_abc"
}
→ {
  "method": "workspace.status",
  "workspace": { "id": "ws_abc", "status": "active", ... }
}
```

### Handshake Integration
When a handshake is accepted, the workspace_id from the handshake is used to create/update a WorkspaceRecord in the WorkspaceStore.

---

## Allowed Write Targets

```
clients/TeraAi/packages/opencode/src/gateway/workspace-registry.ts (new)
clients/TeraAi/packages/opencode/src/gateway/workspace-handlers.ts (new)
clients/TeraAi/packages/opencode/src/gateway/protocol.ts (modify)
clients/TeraAi/packages/opencode/test/gateway/workspace-api.test.ts (new)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | WorkspaceStore creates a record on successful handshake | Unit test |
| 2 | workspace.list returns all active workspaces | Unit test |
| 3 | workspace.status returns workspace metadata | Unit test |
| 4 | workspace.status returns error for unknown workspace | Unit test |
| 5 | Gateway announces `supported_methods` includes "workspace" | Unit test |
| 6 | Typecheck passes | `bun run typecheck` |
| 7 | All existing tests still pass (39/39) | `bun test` |

---

## Verification Commands

```bash
# From packages/opencode
bun run typecheck
bun test test/gateway/
```

---

## Vitality & Polish Checklist

N/A — This is a protocol-level task, not a UI task.

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Workspace Registry — single focused unit |
| One objective only | PASS | WorkspaceRecord + WorkspaceStore |
| No deferrable work included | PASS | Phase 5 starts here |
| No UI unless explicitly requested | PASS | No UI involved |
| No API unless explicitly requested | PASS | This IS the approved scope |
| No Auth unless explicitly requested | PASS | No Auth involved |
| No schema/migration unless explicitly requested | PASS | No database involved |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| Secret handling plan documented and redacted | PASS | N/A |
| CLI side effects checked | PASS | Only typecheck and test |
| No internal contradiction between constraints and outputs | PASS | No contradictions |
| Allowed Write Targets are narrow | PASS | 4 files only |
| Acceptance criteria are testable | PASS | All criteria have unit tests |

Gate Status: **PASS**

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | 4 files created/modified |
| No unauthorized files created | PASS | Only within scope |
| No unauthorized files deleted | PASS | No deletions |
| No unauthorized packages added | PASS | No new packages |
| No real secrets outside approved local environment files | PASS | No secrets |
| Acceptance criteria satisfied | PASS | All 7 criteria met |
| CLI side effects reviewed | PASS | Only typecheck and test |
| No secret leakage | PASS | No secrets |
| Independent review decision recorded | PASS | SecurityAgent not required |

Gate Status: **PASS**

| Test Suite | Pass | Fail |
|---|---|---|
| workspace-api.test.ts | 7 | 0 |
| All others | 39 | 0 |
| **Total** | **46** | **0** (+1 hook timeout pre-existing)
