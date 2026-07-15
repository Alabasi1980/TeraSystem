# Phase 5: Workspace Management — Design Overview

## الملف: 07-phase5-workspace-management-design.md
## المسار: .tera-workspace/PLANS/
## التاريخ: 2026-07-12
## الحالة: Draft

---

## 1. الهدف

بناء نظام إدارة Workspaces داخل المحرك (TeraOpenCode) لتمكين:
- التعامل مع مشاريع عملاء متعددة بنفس المحرك
- عزل تام بين كل Workspace وآخر (Multi-Client Isolation)
- سجل Workspaces نشط يتضمن metadata ومهام

---

## 2. المعمارية المقترحة

```
TeraOpenCode Engine
  ├── Gateway (stdio IPC) ← موجود حالياً
  ├── Workspace Registry ← جديد
  │     ├── WorkspaceStore (in-memory Map)
  │     └── Workspace CRUD عبر Gateway (workspace.* methods)
  └── Isolation Layer ← جديد
        ├── File System Isolation (مسارات منفصلة)
        ├── Task Isolation (مهام كل Workspace منفصلة)
        └── State Isolation (حالة كل Workspace منفصلة)
```

---

## 3. Workspace Registry — التفاصيل

### 3.1 البيانات (WorkspaceRecord)

```typescript
interface WorkspaceRecord {
  id: string              // workspace_id من handshake
  projectId: string       // project_id من handshake
  directory: string       // مسار الـ workspace على القرص
  sessions: Map<string, GatewaySession>  // الجلسات النشطة
  tasks: TaskStore        // مهام الـ workspace
  createdAt: string       // ISO timestamp
  lastActiveAt: string    // ISO timestamp
  status: "active" | "idle" | "closed"
}
```

### 3.2 Gateway Methods الجديدة

```json
// workspace.list — قائمة الـ workspaces النشطة
// workspace.status — حالة Workspace معين
// workspace.close — إغلاق Workspace (تنظيف الذاكرة)
```

### 3.3 العلاقة مع الـ Gateway الحالي

```
Gateway Session الآن:
  session.handshake.workspaceID  ← معرف الـ Workspace

المطلوب:
  session.handshake.workspaceID → WorkspaceRegistry.get(id)
    → WorkspaceRecord معزول بمهامه وجلساته
```

---

## 4. Multi-Client Isolation — التفاصيل

### 4.1 أنواع العزل

| النوع | الوصف | الآلية |
|---|---|---|
| File System | كل Workspace له مساره الخاص | مسارات نسبية من workspace.directory |
| Task State | مهام Workspace لا تتداخل | TaskStore معزول لكل Workspace |
| Memory | Session/State منفصلة لكل Workspace | Map<string, WorkspaceRecord> |
| Gateway | كل طلب يمر عبر workspace_id | التحقق من تطابق workspace_id مع الجلسة |

### 4.2 قواعد العزل

```
1. لا يمكن لـ Workspace A رؤية مهام Workspace B
2. لا يمكن لـ Workspace A الوصول لملفات Workspace B
3. كل Workspace له TaskStore منفصل (Map في الذاكرة)
4. Workspace يُغلق → كل مهامه وجلساته تُلغى
5. Workspace_id إلزامي لكل طلب Gateway بعد Handshake
```

---

## 5. التكامل مع الموجود

### 5.1 ما يتغير

| المكون | التغيير |
|---|---|
| `protocol.ts` | إضافة workspace.* methods |
| `task-handlers.ts` | ربط TaskStore بـ WorkspaceRecord |
| `approval-handlers.ts` | ربط Approval بـ WorkspaceRecord |
| `gateway.ts` (CLI) | دعم --workspace-dir parameter |

### 5.2 ما لا يتغير

- `stdio.ts` — لا تغيير
- `context-api.test.ts` — لا تغيير
- `GATEWAY_API_REFERENCE.md` — تحديث

---

## 6. خطة التنفيذ

| المهمة | الوصف | الأولوية |
|---|---|---|
| TASK-COD-006 | Workspace Registry (WorkspaceRecord + WorkspaceStore) | 🥇 1 |
| TASK-COD-007 | ربط Gateway الحالي بـ Workspace Registry | 🥇 2 |
| TASK-COD-008 | TaskStore معزول لكل Workspace | 🥇 3 |
| TASK-COD-009 | Multi-Client Isolation (File System + State) | 🥇 4 |
| TASK-COD-010 | Workspace Cleanup & Lifecycle | 🥈 5 |

---

## 7. DoD لـ Phase 5

| # | البند |
|---|---|
| 1 | Workspace Registry يدعم create/list/get/close |
| 2 | كل Workspace له TaskStore معزول |
| 3 | جلسات Gateway مرتبطة بـ Workspace |
| 4 | Multi-Client Isolation: ملفات + مهام + حالة |
| 5 | اختبارات تكاملية تمر |
| 6 | توثيق Workspace API مكتمل |
| 7 | read_tera_workspace جاهز للحذف (قرار منفصل) |
