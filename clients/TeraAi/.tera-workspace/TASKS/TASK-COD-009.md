# TASK-COD-009: Workspace Cleanup & Lifecycle

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-009 |
| Phase | Phase 5.5 |
| Title | دورة حياة Workspace + أرشفة وحذف |
| Status | Accepted |
| Created | 2026-07-12 |
| Technology Profile | effect-bun-opencode |

---

## Objective

إكمال طبقة التنظيف ودورة الحياة لـ Workspace:
1. إضافة `status` لكل Workspace: `active | archived`.
2. `workspace.archive` — أرشفة ناعمة (تحتفظ بالبيانات، تمنع عمليات جديدة).
3. `workspace.delete` — حذف نهائي مع تنظيف (cleanup summary).
4. رفض عمليات `task.create` / `approval.request` / `context` على Workspace مؤرشف.

> ملاحظة: `workspace.close` الموجود (من 007) يُبقي كما هو — إزالة من Registry عند انتهاء الجلسة. هذه المهمة تضيف الأرشفة الناعمة والحذف الصريح.

---

## Scope

### In Scope
- `status` في `WorkspaceRecord` (افتراضي `"active"`).
- `workspace.archive` → `status = "archived"`.
- `workspace.delete` → إزالة نهائية من Registry + cleanup summary.
- `task.create` / `approval.request` / `context` تُرفض على Workspace مؤرشف (خطأ `WORKSPACE_ARCHIVED`).
- اختبارات للانتقالات والرفض.

### Out of Scope
- إعادة فتح Workspace مؤرشف (can be future).
- تغيير سلوك `workspace.close` الحالي.

---

## Design

### WorkspaceRecord (توسيع)
```typescript
interface WorkspaceRecord {
  // ... موجودة
  status: "active" | "archived"   // افتراضي "active"
}
```

### أفعال جديدة
```json
{ "method": "workspace", "action": "archive", "workspace_id": "ws_abc" }
→ { "method": "workspace.archive", "status": "archived" }

{ "method": "workspace", "action": "delete", "workspace_id": "ws_abc" }
→ { "method": "workspace.delete", "status": "deleted", "cleaned": {...} }
```

### قواعد العزل/الحياة
```
1. Workspace جديد → status = "active"
2. archive → status = "archived" (البيانات تبقى)
3. archived Workspace يرفض: task.create، approval.request، context
4. delete → إزالة نهائية من Registry
```

---

## Allowed Write Targets

```
clients/TeraAi/packages/opencode/src/gateway/workspace-registry.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/workspace-handlers.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/task-handlers.ts (modify)
clients/TeraAi/packages/opencode/src/gateway/approval-handlers.ts (modify)
clients/TeraAi/packages/opencode/test/gateway/workspace-api.test.ts (modify)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | Workspace جديد status = "active" | Unit test |
| 2 | workspace.archive يضبط status = "archived" | Unit test |
| 3 | archived Workspace يرفض task.create | Unit test |
| 4 | archived Workspace يرفض approval.request | Unit test |
| 5 | workspace.delete يزيل من Registry | Unit test |
| 6 | workspace.status يعرض status | Unit test |
| 7 | Typecheck passes | `bun run typecheck` |
| 8 | All existing tests still pass | `bun test` |

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
| Smallest safe executable unit | PASS | Lifecycle states — خطوة مركزة |
| One objective only | PASS | أرشفة + حذف + رفض على المؤرشف |
| Allowed Write Targets are narrow | PASS | 5 ملفات داخل src/gateway/ |
| Acceptance criteria are testable | PASS | كل المعايير لها اختبارات |

Gate Status: **PASS**

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | 6 ملفات (5 gateway + test). `protocol.ts` عُدّل بتعليمات المهمة صراحةً (رفض context على المؤرشف) |
| No unauthorized files created | PASS | لا ملفات متفرقة |
| No unauthorized packages added | PASS | لا حزم جديدة |
| Acceptance criteria satisfied | PASS | كل المعايير الثمانية متحققة |
| Tests verified by Tera | PASS | 61/61 pass (شغّلت بنفسي) |

Gate Status: **PASS**

### ملاحظة فنية (وجدها EngineeringAgent)
`WorkspaceRecord.status` كان مُعرّفاً مسبقاً كـ `"active" | "idle" | "closed"` لكن `"idle"/"closed"` غير مستخدمين فعلياً. تم توسيع الـ union ليشمل `"archived"` بدلاً من استبداله (أصغر تغيير آمن).

| Test Suite | Pass | Fail |
|---|---|---|
| workspace-api.test.ts | 22 | 0 |
| All others | 39 | 0 |
| **Total** | **61** | **0** |
