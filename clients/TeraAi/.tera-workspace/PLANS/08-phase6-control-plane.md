# Phase 6 (معاد توجيهها): Control Plane MVP + Task Execution حقيقي

> وثيقة تصميم — **بانتظار اعتماد المستخدم قبل أي TASK-COD-***.
> القرار: تأجيل Phase 6 الأصلية (Quality Gates) والبدء بإثبات "أول حلقة عمل حقيقية".

---

## 1. الهدف (Goal)

إثبات أن منظومة TeraOpenCode تعمل كـ **حلقة مغلقة حقيقية**:
```
Control Plane (سائق)
   │  spawn child process + stdio
   ▼
TeraOpenCode Gateway (handshake → context → task)
   │  تنفيذ فعلي داخل Workspace
   ▼
نتيجة حقيقية ← تُستقبل وتُسجَّل في log
```

بدون هذه الخطوة، الـ Gateway (Phase 4+5) يبقى هيكلاً مُختبَراً لكنه **لا ينفّذ شيئاً** (`task.create` حالياً stub يخزّن فقط).

---

## 2. لماذا هذا الترتيب (Pivot Rationale)

- `task.create` الحالي = stub (يخزّن في Map، يرجع `"created"`، لا تنفيذ).
- لا يوجد سائق خارجي يشغّل المحرك ويتحدث البروتوكول.
- بناء Quality Gates (Phase 6 الأصلية) فوق هذا = بوابات على هيكل فارغ.
- **الأثمن أولاً**: إثبات الحلقة، ثم تُبنى الـ Gates عليها لاحقاً.

---

## 3. المعمارية

```
┌─────────────────────────┐         stdio (JSON Lines)        ┌──────────────────────────┐
│  Control Plane MVP       │ ──── handshake/context/task ───►  │  TeraOpenCode Gateway      │
│  (سائق جديد)            │ ◄─── approval.request/result ──   │  (server side - موجود)    │
│                         │ ◄─── task.result (عبر poll) ──    │                           │
│  - يشغّل المحرك child   │                                   │  - يستقبل task            │
│  - يتعامل مع Approval    │                                   │  - ينفّذ (جديد) داخل       │
│  - يسجّل النتائج في log  │                                   │    workspace.directory     │
└─────────────────────────┘                                   └──────────────────────────┘
```

نقطة دخول المحرك الموجودة: `bun run tera gateway` (bin `tera` → `src/cli/cmd/gateway.ts` → `GatewayStdio.runGatewayStdio`).
نمط الـ spawn موجود أصلاً في `test/gateway/gateway-integration.test.ts` (يُعاد استخدامه).

> ⚠️ ملاحظة: `src/control-plane/` الموجود في الـ fork هو أداة OpenCode الداخلية لمحاكاة workspaces بعيدة (debug plugin) — **لا علاقة له بسائقنا**. السائق الجديد سيُوضع في وحدة باسم مميّز (`src/control-plane-mvp/`) لتجنّب التصادم.

---

## 4. النطاق

### In Scope
- **TASK-COD-010**: Real Task Execution — `task.create` ينفّذ إجراءً معرّفاً داخل `workspace.directory` ويرجع نتيجة حقيقية.
- **TASK-COD-011**: Control Plane MVP — سائق CLI يشغّل المحرك، يرسل handshake→context→task، يتعامل مع Approval، يسجّل النتيجة في ملف log.
- (اختياري) **TASK-COD-012**: ربط hook وسطي للـ Gates في Gateway (هيكلي فقط، بلا منطق Gate — تحضيراً لـ Phase 6 لاحقاً).

### Out of Scope (مؤجّل صراحةً)
- Phase 6 الأصلية (Security/Naming/Quality/Doc Gates) — حتى توجد مهام حقيقية لتُفحص.
- اختبار مع عميل حقيقي (Mawthooq) — بعد إثبات الحلقة محلياً.
- Templates / Artifact Storage (كانت مؤجّلة في Phase 5).
- Agent مستقل ذاتي بالكامل — خارج نطاق MVP.
- UI / قاعدة بيانات — لا حاجة لها الآن.

---

## 5. التصميم: TASK-COD-010 (Real Task Execution)

### 5.1 حمولة المهمة (Task Payload)
```typescript
type TaskAction =
  | { type: "shell"; command: string; args?: string[] }
  | { type: "script"; path: string; args?: string[] }
```
يُضاف `action` إلى طلب `task.create`.

### 5.2 التنفيذ
1. تحقق من أن Workspace `status === "active"` (مرفوض إن `archived` — من 009).
2. إن `type: "script"` → تحقق المسار عبر `resolveWorkspacePath` (حارس 008 يمنع الهروب خارج Workspace).
3. إن `risk_level === "critical"` → يمر أولاً بـ `approval.request` (موجود). إن رُفض → `status = "failed"`.
4. شغّل العملية داخل `workspace.directory` (يفضّل `ChildProcessSpawner.ChildProcessSpawner` + `ChildProcess.make` حسب AGENTS.md، أو `Bun.spawn` كحد أدنى).
5. التقط `stdout` / `stderr` / `exitCode`.
6. حدّث `WorkspaceRecord.tasks[id]` = `{ status: "completed" | "failed", result: { stdout, stderr, exitCode, finishedAt } }`.

### 5.3 الاستعلام
- `task.status` يرجع السجل كاملاً (بما فيه `result` عند اكتماله).
- الحالة الانتقالية: `created → running → completed | failed`.
- **الاستقبال في Control Plane**: عبر polling `task.status` حتى تستقر الحالة (أبسط لـ MVP؛ تمديد البروتوكول بـ `task.result` push مؤجّل).

### 5.4 السلامة (Security)
- التنفيذ محصور في `workspace.directory` فقط (حارس المسار).
- لا تنفيذ لمسارات خارجية.
- Approval gate للـ critical كما هو.
- لا شبكة خارجية مفتوحة ضمن MVP (يمكن تشديدها لاحقاً).

---

## 6. التصميم: TASK-COD-011 (Control Plane MVP)

### 6.1 الموقع
`src/control-plane-mvp/` — سائق مستقل عن `src/control-plane/` الموجود.

### 6.2 الإعداد (JSON)
```json
{
  "workspace_id": "ws_demo",
  "workspace_dir": ".tera-workspace/demo",
  "auto_approve": true,
  "task": { "type": "shell", "command": "echo hello-from-task" }
}
```

### 6.3 التدفق
1. `spawn` المحرك: `bun run tera gateway` (stdio).
2. `handshake` (workspace_id + workspace_dir).
3. `context` (بيانات أولية بسيطة).
4. `task.create` بالحمولة.
5. عند استقبال `approval.request`:
   - إن `auto_approve` → يرد `approval.response` بموافقة.
   - وإلا ينتظر إدخال المستخدم (MVP: auto-approve افتراضياً).
6. يراقب `task.status` (poll) حتى `completed`/`failed`.
7. يكتب سطر JSON لكل حدث في `control-plane.log` (handshake, context, task_sent, approval, result).
8. يطبع الملخّص ويخرج.

### 6.4 التحقق
- تشغيل السائق بمهمة تجريبية → يطبع نتيجة حقيقية (`hello-from-task`).
- `control-plane.log` يحتوي كامل التبادل.
- لا UI، لا DB.

---

## 7. خطة التنفيذ المقترحة

| المهمة | الوصف | الأولوية | الترتيب |
|---|---|---|---|
| TASK-COD-010 | Real Task Execution (engine side) | 🥇 | أولاً (الجزء الناقص فعلياً) |
| TASK-COD-011 | Control Plane MVP (driver) | 🥇 | ثانياً (يختبر 010) |
| TASK-COD-012 | Gate middleware hook (هيكلي) | 🥈 | اختياري — تحضير Phase 6 |

> الترتيح: 010 قبل 011 لأن السائق بلا تنفيذ حقيقي لا يثبت شيئاً؛ لكنهما يُراجعان معاً كـ "أول حلقة عمل".

---

## 8. معايير الإكمال (DoD)

| # | المعيار |
|---|---|
| 1 | `task.create` ينفّذ أمراً حقيقياً داخل Workspace ويرجع stdout/exitCode |
| 2 | النتيجة محصورة داخل `workspace.directory` (لا هروب مسار) |
| 3 | Control Plane يشغّل المحرك، يكمل الحلقة، يسجّل النتيجة في log |
| 4 | 61 اختباراً حالياً لا تنكسر + اختبارات جديدة للتنفيذ والسائق |
| 5 | `bun run typecheck` نظيف |
| 6 | لا UI / لا DB / لا تضخم |

---

## 9. المخاطر

| الخطر | التخفيف |
|---|---|
| انفجار نطاق "التنفيذ" (agent كامل) | حصر MVP في shell/script داخل Workspace فقط |
| تصادم مع `src/control-plane` الموجود | استخدام `src/control-plane-mvp/` |
| حلقة غير متزامنة (result push) | الـ MVP يستخدم polling `task.status` فقط |
| أمان التنفيذ | حارس المسار من 008 + Approval gate للـ critical |
