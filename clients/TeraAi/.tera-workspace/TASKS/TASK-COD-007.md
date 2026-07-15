# TASK-COD-007: Gateway-Workspace Binding + Isolation

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-007 |
| Phase | Phase 5.2 |
| Title | ربط Gateway بـ Workspace Registry + TaskStore معزول |
| Status | Accepted |
| Created | 2026-07-12 |
| Technology Profile | effect-bun-opencode |

---

## Objective

ربط Gateway الحالي بـ Workspace Registry لتحقيق:
1. **TaskStore معزول لكل Workspace** — مهام كل Workspace منفصلة
2. **ApprovalStore معزول لكل Workspace** — approvals لكل Workspace منفصلة
3. **workspace.close** method — تنظيف الذاكرة عند إغلاق Workspace
4. **Gateway Session tracking** — تتبع الجلسات النشطة لكل Workspace

---

## Scope

### In Scope
- TaskStore مرتبط بـ WorkspaceRecord (بدون Map عام)
- ApprovalStore مرتبط بـ WorkspaceRecord
- Gateway method: `workspace.close`
- تنظيف TaskStore + ApprovalStore عند close
- اختبارات للعزل (مهام Workspace A لا تظهر في Workspace B)

### Out of Scope (Deferred)
- File System Isolation (TASK-COD-009)
- read_tera_workspace removal

---

## Design

### TaskStore في WorkspaceRecord
```typescript
interface WorkspaceRecord {
  // ... موجودة
  tasks: Map<string, string>    // task_id → status
  approvals: ApprovalRecord[]   // سجل الموافقات
  sessions: string[]            // معرفات الجلسات النشطة
}
```

### workspace.close method
```json
{
  "method": "workspace",
  "action": "close",
  "workspace_id": "ws_abc"
}
→ {
  "method": "workspace.close",
  "status": "closed",
  "cleaned": { "tasks": 3, "approvals": 2, "sessions": 1 }
}
```

### قواعد العزل
```
1. task.create يخزن المهمة في WorkspaceRecord.tasks
2. task.status يقرأ من WorkspaceRecord.tasks
3. task.cancel يحدث WorkspaceRecord.tasks
4. approval.request يخزن في WorkspaceRecord.approvals
5. workspace.close ينظف كل شيء
6. Workspace A لا يرى مهام Workspace B
```

---

## Allowed Write Targets

```
clients/TeraAi/packages/opencode/src/gateway/workspace-registry.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/workspace-handlers.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/task-handlers.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/approval-handlers.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/protocol.ts (modify)
clients/TeraAi/packages/opencode/test/gateway/workspace-api.test.ts (modify)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | Workspace A tasks isolated from Workspace B | Unit test |
| 2 | workspace.close cleans all tasks + approvals | Unit test |
| 3 | workspace.close returns cleanup summary | Unit test |
| 4 | Workspace A approvals isolated from Workspace B | Unit test |
| 5 | Typecheck passes | `bun run typecheck` |
| 6 | All existing tests still pass | `bun test` |

---

## Verification

```bash
cd clients/TeraAi/packages/opencode
bun run typecheck
bun test test/gateway/
```

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Workspace isolation — single focused step |
| One objective only | PASS | Bind Gateway to Workspace Registry |
| Allowed Write Targets are narrow | PASS | 6 files within src/gateway/ |
| Acceptance criteria are testable | PASS | All criteria have tests |

Gate Status: **PASS**

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | All 6 files within scope |
| No unauthorized files created | PASS | No new files outside scope |
| No unauthorized packages added | PASS | No new packages |
| Acceptance criteria satisfied | PASS | All 6 criteria met |
| Task isolation verified | PASS | Workspace A ≠ Workspace B |

Gate Status: **PASS**

| Test Suite | Pass | Fail |
|---|---|---|
| workspace-api.test.ts | 12 | 0 |
| All others | 39 | 0 |
| **Total** | **51** | **0** |
