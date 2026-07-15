# TASK-COD-008: Multi-Client Isolation (File System + State)

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-008 |
| Phase | Phase 5.4 |
| Title | عزل متعدد العملاء — نظام الملفات + تأكيد العزل |
| Status | Accepted |
| Created | 2026-07-12 |
| Technology Profile | effect-bun-opencode |

---

## Objective

إكمال طبقة العزل بين العملاء (Multi-Client Isolation):
1. **File System Isolation** — كل Workspace له مساره الخاص، مع حماية من traversal خارج المسار.
2. **State Isolation (تأكيد)** — TaskStore + ApprovalStore معزولان لكل Workspace (منجز في 007، يُعاد تأكيده باختبار).

> ملاحظة: "TaskStore Isolation" المخطط كـ TASK-COD-008 في تصميم Phase 5 قد تم تنفيذه ضمن TASK-COD-007، لذا هذه المهمة تركز على File System Isolation + تأكيد العزل القائم.

---

## Scope

### In Scope
- إضافة `directory` إلى `WorkspaceRecord` (من handshake `workspace_dir` أو افتراضي `.tera-workspace/<id>`).
- دالة حارس `resolveWorkspacePath(workspaceId, inputPath)` ترجع المسار المطلق داخل دليل Workspace، ترفض:
  - المسارات المطلقة خارج دليل Workspace
  - traversal (`../`) الخارج من دليل Workspace
- ربط handshake بقراءة `workspace_dir` وتخزينه.
- اختبارات للحارس (traversal rejection + cross-workspace isolation).

### Out of Scope (Deferred)
- تطبيق الحارس على أدوات ملفات فعلية داخل Gateway (لا توجد أداة ملفات حالياً — الحارس جاهز للاستخدام لاحقاً).
- read_tera_workspace removal (مؤجل).

---

## Design

### WorkspaceRecord (توسيع)
```typescript
interface WorkspaceRecord {
  // ... موجودة
  directory: string   // مسار العمل المطلق لكل Workspace
}
```

### حارس المسار
```typescript
function resolveWorkspacePath(workspaceId: string, inputPath: string):
  | { ok: true; path: string }
  | { ok: false; error: "PATH_TRAVERSAL" | "OUTSIDE_WORKSPACE" | "WORKSPACE_NOT_FOUND" }
```

### handshake (توسيع)
```json
{
  "method": "handshake",
  "version": "1.2",
  "workspace_id": "ws_abc",
  "workspace_dir": "/abs/path/to/ws_abc"   // اختياري
}
→ directory = workspace_dir || path.resolve(".tera-workspace", workspace_id)
```

---

## Allowed Write Targets

```
clients/TeraAi/packages/opencode/src/gateway/workspace-registry.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/protocol.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/workspace-handlers.ts (modify if needed)
clients/TeraAi/packages/opencode/test/gateway/workspace-api.test.ts (modify)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | `directory` مخزن لكل Workspace | Unit test |
| 2 | `resolveWorkspacePath` يرفض traversal خارج الدليل | Unit test |
| 3 | `resolveWorkspacePath` يرفض مساراً مطلقاً خارج Workspace | Unit test |
| 4 | `resolveWorkspacePath` يقبل مساراً صالحاً داخل Workspace | Unit test |
| 5 | Workspace A لا يرى ملفات Workspace B (عبر الحارس) | Unit test |
| 6 | Typecheck passes | `bun run typecheck` |
| 7 | All existing tests still pass | `bun test` |

---

## Verification

```bash
cd clients/TeraAi/packages/opencode
bun run typecheck
bun test test/gateway/
```

---

## Vitality & Polish Checklist

[ ] ✅ / N/A — لا UI في هذه المهمة (خادم/مكتبة) — N/A بسبب طبيعة المهمة (non-UI)

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | File System Isolation — خطوة مركزة |
| One objective only | PASS | عزل نظام الملفات + تأكيد العزل |
| Allowed Write Targets are narrow | PASS | 4 ملفات داخل src/gateway/ |
| Acceptance criteria are testable | PASS | كل المعايير لها اختبارات |

Gate Status: **PASS**

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | 3 ملفات فقط (registry, protocol, test) |
| No unauthorized files created | PASS | لا ملفات متفرعة (diag-spawn.ts أُزيل) |
| No unauthorized packages added | PASS | لا حزم جديدة |
| Acceptance criteria satisfied | PASS | كل المعايير السبعة متحققة |
| Tests verified by Tera | PASS | 55/55 pass (شغّلت بنفسي) |

Gate Status: **PASS**

| Test Suite | Pass | Fail |
|---|---|---|
| workspace-api.test.ts | 16 | 0 |
| All others | 39 | 0 |
| **Total** | **55** | **0** |
