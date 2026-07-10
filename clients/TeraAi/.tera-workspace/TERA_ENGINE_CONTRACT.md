# TERA ENGINE CONTRACT

## الإصدار: 1.2
## الحالة: ✅ Approved — اعتمد من ماجد
## التاريخ: 2026-07-10 (آخر تحديث: 2026-07-10)
## الملف: TERA_ENGINE_CONTRACT.md
## المسار: .tera-workspace/

---

# مقدمة

يحدد هذا العقد العلاقة المعمارية والتشغيلية بين مكوني النظام الأساسيين:

```
TeraSystem Platform (المنصة — Control Plane)
     │
     │  Engine Contract v1.2
     │
     ▼
TeraOpenCode Engine (محرك التنفيذ — Execution Engine)
     │
     │  يعمل ضد
     │
     ▼
Project Workspace (مساحة عمل المشروع)
```

---

# 1. تعريف الطبقات

## 1.1 TeraSystem Platform

المنصة الأم والـ Control Plane. تملك:

- العملاء والمشاريع (Customers & Projects)
- المهام والقرارات (Tasks & Decisions)
- السياسات وجودة التطوير (Policies & Gates)
- الجلسات والذاكرة التنظيمية (Sessions & Memory)
- سير العمل ومراحل التطبيق (Workflows)
- الموافقات (Approvals)
- التقارير ولوحات التحكم (Reports & Dashboards)

**ملكية الحالة:** TeraSystem تملك الحالة الدائمة وحالة الأعمال بالكامل.

## 1.2 TeraOpenCode Engine

محرك تنفيذ برمجي **مركزي واحد**. ينفذ:

- قراءة مساحة عمل المشروع والعمل عليها
- تعديل الملفات وإنشائها (ضمن صلاحية المهمة)
- تشغيل أوامر Shell (ضمن Capability Envelope)
- إدارة Git
- تشغيل الاختبارات
- البحث والتنقل في الكود (Glob, Grep, Read)
- التفاعل مع نماذج الذكاء الاصطناعي
- تنفيذ مهام برمجية ضمن القيود التي تحددها المنصة

**ملكية الحالة:**

```
TeraSystem تملك الحالة الدائمة وحالة الأعمال.

TeraOpenCode:
  ├── لا يملك حالة دائمة.
  ├── يمكنه تشغيل Runtime Session مؤقتة أثناء تنفيذ مهمة.
  ├── Runtime Session = جلسة عمل نشطة واحدة (مهمة واحدة).
  ├── تبدأ عند استلام التكليف، وتنتهي عند إرسال النتيجة.
  ├── بعد الانتهاء: لا تبقى حالة في المحرك.
  └── الفرق الجوهري: Runtime Session ≠ State. المحرك لا يخزّن تاريخاً ولا ملفات حالة.
```

**المقارنة:**

| البند | TeraSystem Platform | TeraOpenCode Engine |
|---|---|---|
| الحالة الدائمة | ✅ تملكها بالكامل | ❌ لا يملكها |
| Runtime Session مؤقتة | ✅ Platform Session (أطول) | ✅ Engine Runtime Session (أقصر — مهمة واحدة) |
| تاريخ التنفيذ | ✅ يُسجَّل في المنصة | ❌ لا يحتفظ به |
| ملفات الحالة | ✅ في `.tera-workspace/` وفي المنصة | ❌ لا ملفات حالة |

## 1.3 Project Workspace

مساحة عمل المشروع المستقلة. تحتوي:

- كود التطبيق (Source Code) — **ملكية العميل**
- ملفات المشروع (Project Files)
- التبعيات (Dependencies)
- إعدادات البيئة (Environment Config)
- `.tera-workspace/` (نسخة من سياق المنصة — **للقراءة فقط من المحرك**)

**الـ Workspace لا يحتوي على نسخة من المحرك.**

**ملكية الكود:** العميل/المؤسسة تملك الكود. المحرك جهة **مخوّلة بالتعديل** ضمن حدود المهمة المحددة، وليس الجهة الوحيدة القادرة على تعديله.

---

# 2. نموذج الجلسات (Session Model)

## 2.1 الأنواع الثلاثة

| النوع | المالك | المدة | الغرض |
|---|---|---|---|
| **Platform Session** | TeraSystem | طويلة (أيام/أسابيع) | تتبع سير المشروع بالكامل |
| **Engine Runtime Session** | TeraOpenCode | مؤقتة (مهمة واحدة) | تنفيذ مهمة محددة ثم الانتهاء |
| **Session Result/Summary** | TeraSystem | دائمة | ملخص ما تم — يُسجَّل في المنصة |

## 2.2 دورة حياة الجلسة

```
Platform Session (على طول حياة المشروع)
  │
  ├── تكليف 1 ← Engine Runtime Session #1 → Session Result #1 → Platform تقيّم
  │
  ├── تكليف 2 ← Engine Runtime Session #2 → Session Result #2 → Platform تقيّم
  │
  ├── تكليف 3 ← Engine Runtime Session #3 → Session Result #3 → Platform تقيّم
  │
  └── إغلاق المشروع ← Session Summary نهائي
```

**قاعدة جوهرية:** المحرك لا يملك جلسة دائمة. كل مهمة هي جلسة مستقلة تبدأ وتنتهي.

---

# 3. نموذج المهام (Task Model)

## 3.1 دورة حياة المهمة

```
Platform تنشئ المهمة
  │
  ▼
Platform تُرسل TaskAssignment إلى المحرك
  │  (مع Capability Envelope)
  │
  ▼
Engine Runtime Session تبدأ
  │  ┌─────────────────────────────────────────────┐
  │  │ Capability Envelope يُفرض وقائيًا           │
  │  │ قبل كل File/Shell/Network/Git operation     │
  │  └─────────────────────────────────────────────┘
  │
  ▼
Engine تنفذ وتُرسل Execution Status/Result
  │  (تقرير بالحالة فقط — لا تغيير في Task State)
  │
  ▼
Platform تستقبل النتيجة
  │
  ├── Gate Check (Quality, Security, Naming...)
  │
  ├── Approval Check (إذا كان مطلوباً)
  │
  ├── Platform وحدها تقرر Task State الجديد
  │    (in_progress, review, completed, failed, blocked)
  │
  └── Platform تُسجّل Session Result
```

**مبدأ أساسي:** المحرك لا يغيّر Task State. المحرك يُرسل **Execution Status/Result** فقط. المنصة وحدها تقرر حالة المهمة بعد Gates والموافقات.

## 3.2 الفرق بين Status و State

| المصطلح | من يُحدده | أمثلة |
|---|---|---|
| **Execution Status** | Engine | running, completed, failed, error |
| **Task State** | Platform only | pending, in_progress, review, completed, failed, blocked, cancelled |

---

# 4. آلية الاتصال

## 4.1 الاتصال الحالي (Phase 3 — File-based)

```
TeraSystem ←→ .tera-workspace/ (ملفات مارك داون) ←→ TeraOpenCode Engine
```

- المحرك **يقرأ** من `.tera-workspace/` عبر `read_tera_workspace`
- المحرك **لا يكتب** في `.tera-workspace/` — أبداً في الحالة الحالية
- المحرك **يكتب** في Project Workspace فقط (كود المشروع)
- لا يوجد API أو IPC بعد

**الحالة الرسمية:**
- `.tera-workspace/`: **قراءة فقط** من المحرك
- `Project Workspace/`: **قراءة وكتابة** من المحرك (ضمن Capability Envelope)

## 4.2 الاتصال المستقبلي — Request/Response (Phase 4-5)

بدلاً من Event Stream، نبدأ بقناة **Request/Response بسيطة** عبر Local IPC أو HTTP:

```
TeraSystem Platform
     │
     ├── Request ←→ Response (stdio / localhost HTTP)
     │     │
     │     ├── ContextRequest / ContextResponse
     │     ├── TaskAssignmentRequest / TaskResultResponse
     │     └── ApprovalRequest / ApprovalResponse
     │
     └── Workspace Store (ملفات)
```

**لماذا Request/Response بدلاً من Event Stream؟**

| المعيار | Request/Response | Event Stream |
|---|---|---|
| التعقيد | بسيط | معقد |
| التزامن | واضح | يحتاج اتصال مفتوح |
| التشخيص | سهل | صعب |
| الاحتياج الحالي | يكفي لـ Phase 4 | لا حاجة تشغيلية مثبتة |
| الصيانة | سهلة | تحتاج خادم دائم |

**القاعدة:** Event Stream يتأجل حتى تظهر حاجة تشغيلية مثبتة (multi-engine, real-time monitoring, إلخ).

## 4.3 الاتصال المستقبلي — API (Phase 6+)

```
TeraSystem Platform (Control Plane)
     │
     ├── REST / WebSocket API ←→ TeraOpenCode Engine
     │         │
     │         └── Engine Gateway (Context, Task, Result, Approval APIs)
     │
     └── Workspace Store (ملفات / DB)
```

| المرحلة | الآلية | متى |
|---|---|---|
| 1 (الآن) | File-based عبر `.tera-workspace/` (قراءة فقط) | Phase 3 |
| 2 | Request/Response عبر Local IPC | Phase 4-5 |
| 3 | REST API كاملة | Phase 6+ |

---

# 5. Capability Envelope

كل تكليف من المنصة إلى المحرك **يجب أن يتضمن** Capability Envelope يحدد بالضبط ما هو مسموح وما هو ممنوع.

## 5.1 التعريف

```typescript
interface CapabilityEnvelope {
  // مسارات مسموح بالقراءة منها
  allowed_read_paths: string[]        // ["src/", "tests/", "package.json"]

  // مسارات مسموح بالكتابة فيها
  allowed_write_paths: string[]       // ["src/components/", "tests/"]

  // مسارات ممنوعة تماماً (لا قراءة ولا كتابة)
  forbidden_paths: string[]           // ["*.env", ".secrets/", "node_modules/"]

  // أوامر Shell مسموحة
  allowed_commands: string[]          // ["bun test", "bun run build", "git status"]

  // أوامر Shell ممنوعة
  forbidden_commands: string[]        // ["rm -rf", "curl", "wget", "npm publish"]

  // سياسة الشبكة
  network_policy: {
    internet_access: boolean          // true / false
    allowed_domains: string[]         // ["registry.npmjs.org"] (إذا كان مسموحاً)
  }

  // متطلبات موافقة
  approval_required: {
    destructive_actions: boolean      // حذف ملفات، تغييرات جذرية
    security_sensitive: boolean       // تغييرات أمان، .env
    major_dependency_changes: boolean  // ترقية تبعيات رئيسية
  }
}
```

## 5.2 أمثلة

**مهمة بسيطة:**
```json
{
  "allowed_read_paths": ["src/", "tests/", "package.json", "tsconfig.json"],
  "allowed_write_paths": ["src/components/NewButton.tsx"],
  "forbidden_paths": ["*.env", ".secrets/", "node_modules/", ".git/"],
  "allowed_commands": ["bun test", "bun run build"],
  "forbidden_commands": ["rm -rf", "npm publish", "git push"],
  "network_policy": { "internet_access": false, "allowed_domains": [] },
  "approval_required": { "destructive_actions": true, "security_sensitive": true, "major_dependency_changes": true }
}
```

**مهمة موسّعة:**
```json
{
  "allowed_read_paths": ["src/", "tests/", "docs/", "package.json"],
  "allowed_write_paths": ["src/", "tests/", "docs/"],
  "forbidden_paths": ["*.env", ".secrets/", "node_modules/"],
  "allowed_commands": ["bun test", "bun run build", "bunx eslint"],
  "forbidden_commands": ["rm -rf", "npm publish"],
  "network_policy": { "internet_access": true, "allowed_domains": ["registry.npmjs.org"] },
  "approval_required": { "destructive_actions": true, "security_sensitive": true, "major_dependency_changes": true }
}
```

## 5.3 آلية الإلزام (Enforcement)

**الـ Capability Envelope تُفرض وقائيًا (Proactive Enforcement)، وليست مجرد قاعدة يبلّغ المحرك عن انتهاكها بعد وقوعها.**

```
قبل كل File/Shell/Network/Git operation:
  │
  ├── المحرك يتحقق من الإجراء ضد Capability Envelope
  │     │
  │     ├── مسموح → التنفيذ
  │     │
  │     ├── ممنوع → الرفض الفوري + EngineError
  │     │            (الإجراء لا ينفذ أبداً)
  │     │
  │     └── يتطلب موافقة → ApprovalRequest
  │                         (الانتظار حتى الرد)
  │
  └── المنصة لا تعتمد على تقرير المحرك فقط
        ├── المنصة تتحقق بشكل مستقل عند الضرورة
        └── المحرك يُبلّغ عن الانتهاكات كخطأ إضافي
```

**القاعدة الجوهرية:**

```
❌ لا يُسمح بتنفيذ إجراء ثم الإبلاغ عن انتهاك بعده.
✅ الإجراء يُرفض قبل التنفيذ إذا تجاوز الـ Envelope.
✅ المحرك مسؤول عن التحقق قبل التنفيذ.
✅ المنصة تتحقق بشكل مستقل عند الضرورة.
```

## 5.4 قواعد الانتهاك (عند وقوعه فعلياً)

```
إذا المحرك تجاوز Capability Envelope (يجب ألا يحدث أصلاً):
  1. المنصة توقف المهمة فوراً
  2. المنصة تُسجّل الحدث بالتفصيل
  3. تُقيّم التأثير على Project Workspace
  4. تقرر: إعادة المهمة / إخطار المستخدم / إلغاء
  5. لا يُسمح للمحرك بتجاوز الحدود مرة أخرى في نفس المهمة
  6. التقرير يُستخدم للتعلم والتحسين، لا كبديل عن الإلزام
```

---

# 6. Schemas

**ملاحظة عامة:** جميع الـ Schemas تتضمن حقول التوافق والتتبع التالية بشكل موحد:

```typescript
// الحقول المشتركة في جميع الـ Schemas
interface CommonFields {
  contract_version: string        // إصدار Engine Contract (مثلاً "1.2")
  platform_version: string        // إصدار TeraSystem Platform
  engine_version: string          // إصدار TeraOpenCode Engine
  workspace_id: string            // معرّف Project Workspace
  project_id: string              // معرّف المشروع
  correlation_id: string          // معرّف تتبع عابر لربط الطلب بالرد
}
```

## 6.1 TaskAssignment

```typescript
interface TaskAssignment extends CommonFields {
  // معرّف المهمة الفريد
  task_id: string

  // وصف المهمة
  description: string

  // المدخلات المطلوبة
  inputs: {
    context?: string          // سياق المنصة (ملف مارك داون أو JSON)
    files?: string[]         // ملفات مرجعية
    previous_results?: string[]  // نتائج مهام سابقة
  }

  // Capability Envelope
  capabilities: CapabilityEnvelope

  // الأولوية
  priority: "low" | "normal" | "high" | "critical"

  // معرّف الجلسة (المنصة)
  platform_session_id: string

  // المهلة الزمنية (اختياري)
  deadline?: string          // ISO 8601
}
```

## 6.2 ExecutionResult

```typescript
interface ExecutionResult extends CommonFields {
  // معرّف المهمة
  task_id: string

  // معرّف جلسة التنفيذ
  engine_runtime_session_id: string

  // حالة التنفيذ
  status: "completed" | "failed" | "error" | "timeout" | "blocked"

  // المخرجات
  outputs: {
    files_changed?: string[]       // ملفات تم تعديلها
    files_created?: string[]       // ملفات جديدة
    files_deleted?: string[]       // ملفات محذوفة
    commands_run?: string[]        // أوامر تم تشغيلها
    test_results?: {               // نتائج الاختبارات
      total: number
      passed: number
      failed: number
    }
    summary?: string               // ملخص تنفيذي
  }

  // الأخطاء (إذا وجدت)
  errors?: EngineError[]

  // مدة التنفيذ
  duration_ms: number

  // الطوابع الزمنية
  started_at: string               // ISO 8601
  completed_at: string             // ISO 8601

  // ملاحظات المحرك
  notes?: string
}
```

## 6.3 ApprovalRequest

```typescript
interface ApprovalRequest extends CommonFields {
  // معرّف الطلب
  request_id: string

  // معرّف المهمة
  task_id: string

  // نوع الإجراء المطلوب الموافقة عليه
  action_type: "destructive" | "security_sensitive" | "major_dependency" | "other"

  // وصف الإجراء
  description: string

  // التفاصيل
  details: {
    affected_files?: string[]
    affected_commands?: string[]
    risk_level?: "low" | "medium" | "high"
  }

  // مهلة الرد
  response_deadline?: string       // ISO 8601
}

interface ApprovalResponse extends CommonFields {
  request_id: string
  approved: boolean
  reason?: string
  approved_by: string              // معرّف المستخدم أو "system"
}
```

## 6.4 EngineEvent

```typescript
interface EngineEvent extends CommonFields {
  // معرّف الحدث
  event_id: string

  // نوع الحدث
  event_type:
    | "engine.started"           // المحرك بدأ
    | "engine.progress"          // تقدم في التنفيذ
    | "engine.completed"         // اكتمل
    | "engine.failed"            // فشل
    | "engine.approval_needed"   // يحتاج موافقة
    | "engine.gate_request"      // طلب تحقق من Gate

  // الاتجاه
  direction: "platform_to_engine" | "engine_to_platform"

  // الحمولة
  payload: Record<string, unknown>

  // الطابع الزمني
  timestamp: string              // ISO 8601

  // معرّف المهمة
  task_id: string
}
```

**ملاحظة:** EngineEvent معرّف هنا لغرض التوثيق. **الـ Event Stream itself مُؤجَّل** حتى تظهر حاجة تشغيلية مثبتة. حالياً لا يوجد قناة أحداث فعّالة.

## 6.5 EngineError

```typescript
interface EngineError extends CommonFields {
  // معرّف الخطأ
  error_id: string

  // نوع الخطأ
  error_type:
    | "tool_failure"             // خطأ في أداة تنفيذ
    | "permission_denied"        // محاولة تجاوز صلاحية
    | "capability_violation"     // تجاوز Capability Envelope
    | "model_error"              // خطأ في نموذج AI
    | "timeout"                  // انتهاء المهلة
    | "internal_error"           // خطأ داخلي في المحرك

  // الرسالة
  message: string

  // التفاصيل
  details?: Record<string, unknown>

  // الخطأ الأصلي (إذا كان هناك سلسلة)
  cause?: string

  // الطابع الزمني
  timestamp: string

  // هل يُوقف المهمة؟
  fatal: boolean
}
```

---

# 7. ملكية البيانات (Data Ownership)

| نوع البيانات | المالك | أين تخزن | من يعدّلها |
|---|---|---|---|
| كود المشروع | **العميل/المؤسسة** | Project Workspace | **المحرك مخوّل** (ضمن Capability Envelope) + المطورون |
| سياسات المشروع | TeraSystem | Workspace + المنصة | المنصة فقط |
| المهام | TeraSystem | المنصة | المنصة فقط |
| القرارات | TeraSystem | المنصة | المنصة فقط (المحرك يقترح) |
| الجلسات | TeraSystem | المنصة | المنصة فقط |
| صلاحيات المستخدمين | TeraSystem | المنصة | المنصة فقط |
| نتائج التنفيذ | TeraSystem | المنصة | المحرك يُنتج، المنصة تملك |
| Runtime Session (مؤقتة) | TeraOpenCode | الذاكرة فقط (تنتهي عند المهمة) | المحرك فقط |
| Session Result/Summary | TeraSystem | المنصة (دائم) | المنصة فقط |

**المبدأ:**
- المحرك **يقرأ** من المنصة ما يحتاج لتنفيذ المهمة.
- المحرك **يكتب** إلى Project Workspace (كود المشروع) فقط.
- المحرك **يقرأ** من `.tera-workspace/` ولا يكتب فيه.
- المحرك **لا يملك** حالة دائمة — Runtime Session تنتهي عند إرسال النتيجة.

---

# 8. الصلاحيات (Permissions)

## 8.1 صلاحيات المحرك داخل Project Workspace

| الإجراء | مسموح؟ | ملاحظة |
|---|---|---|
| قراءة ملفات | ✅ نعم | ضمن `allowed_read_paths` من Capability Envelope |
| إنشاء ملفات | ✅ نعم | ضمن `allowed_write_paths` |
| تعديل ملفات | ✅ نعم | ضمن `allowed_write_paths` |
| حذف ملفات | ⚠️ يعتمد | فقط إذا كان `destructive_actions: true` في ApprovalRequired |
| تشغيل أوامر Shell | ✅ نعم | ضمن `allowed_commands` فقط |
| تشغيل Git | ✅ نعم | ضمن `allowed_commands` |
| الوصول للإنترنت | ⚠️ يعتمد | حسب `network_policy.internet_access` |

**جميع العمليات أعلاه تُفحص وقائيًا** ضد Capability Envelope قبل التنفيذ.

## 8.2 صلاحيات المحرك تجاه `.tera-workspace/`

| الإجراء | مسموح؟ | ملاحظة |
|---|---|---|
| قراءة أي ملف | ✅ نعم | عبر `read_tera_workspace` |
| كتابة أي ملف | ❌ لا | **ممنوع أبداً** — تابع للمنصة حصراً |
| حذف أي ملف | ❌ لا | ممنوع |
| تعديل محتوى | ❌ لا | ممنوع |

## 8.3 صلاحيات المحرك تجاه المنصة

| الإجراء | مسموح؟ | ملاحظة |
|---|---|---|
| قراءة السياسات (Context) | ✅ نعم | عبر SystemContext |
| قراءة `.tera-workspace/` | ✅ نعم | عبر `read_tera_workspace` |
| قراءة سجل المهام | ⏸️ معلق | بعد بناء Engine Gateway |
| تغيير حالة مهمة | ❌ لا | فقط المنصة — المحرك يُرسل ExecutionResult فقط |
| تسجيل قرار | ❌ لا | فقط المنصة (المحرك يقترح عبر ApprovalRequest) |
| تجاوز Gate | ❌ لا | ممنوع قطعاً |

## 8.4 حدود المحرك (ما لا يحق له فعله)

```
❌ تغيير سياسات TeraSystem
❌ تعديل سجل القرارات
❌ تغيير حالة المهام (يُرسل ExecutionResult فقط)
❌ تجاوز Quality Gates
❌ الوصول لبيانات عميل آخر
❌ تعديل إعدادات المنصة
❌ حذف `.tera-workspace/`
❌ كتابة أو تعديل أي ملف في `.tera-workspace/`
❌ تجاوز Capability Envelope المحدد في التكليف
```

---

# 9. معالجة الأخطاء (Error Handling)

## 9.1 أنواع الأخطاء

| نوع الخطأ | مثال | من يعالجه |
|---|---|---|
| `tool_failure` | أداة تنفيذ فشلت | المحرك (Retry / Fallback) |
| `permission_denied` | محاولة وصول لمسار ممنوع | المحرك (رفض + EngineError → المنصة) |
| `capability_violation` | تجاوز Capability Envelope | **يُرفض وقائياً** — لا يُنفذ أصلاً |
| `model_error` | LLM لا يستجيب | المحرك (Retry / Fallback) |
| `timeout` | انتهاء المهلة | المنصة (إعادة جدولة أو إلغاء) |
| `internal_error` | خطأ غير متوقع | المنصة (Session Recovery) |

## 9.2 قاعدة الأخطاء الأساسية

```
المحرك يفشل بأمان (Fail Gracefully):
  1. يوقف المهمة الحالية
  2. يُبلّغ المنصة بالخطأ (EngineError)
  3. لا يترك Project Workspace في حالة غير مستقرة
  4. لا يفقد بيانات غير قابلة للاسترجاع
  5. لا يتجاوز Capability Envelope أثناء التعافي

المنصة تعالج الأخطاء التي يعجز المحرك عن معالجتها:
  1. تُسجّل الخطأ
  2. تُقيّم التأثير
  3. تقرر: إعادة المحاولة / تغيير المهمة / إخطار المستخدم / إلغاء
```

---

# 10. الإصدار والتوافق (Versioning & Compatibility)

## 10.1 إصدارات المحرك

```
TeraOpenCode Engine vX.Y.Z

X = Major (تغيير في Engine Contract يتطلب ترقية المنصة أيضاً)
Y = Minor (إضافة إمكانية مع الحفاظ على التوافق مع Contract الحالي)
Z = Patch (إصلاح أخطاء — شفاف للـ Contract)
```

## 10.2 قواعد التوافق

| تغيير | يتطلب | يؤثر على |
|---|---|---|
| ترقية Major في Engine | تحديث المنصة أيضاً + Contract version bump | Engine Contract |
| ترقية Minor في Engine | متوافق مع الإصدار السابق | الأدوات فقط |
| ترقية Patch في Engine | شفاف | لا شيء |
| تغيير في Capability Envelope | تحديث Engine + Platform معاً | Engine Contract |
| إضافة schema جديد | Minor version + Platform support | Engine Contract |

## 10.3 علاقة الإصدارات

```
TeraSystem Platform vA.B
     │
     │  Engine Contract vN.M
     │
     ▼
TeraOpenCode Engine vX.Y.Z
```

- Platform vA.B تعمل مع أي Engine يتوافق مع Contract vN.M
- Platform و Engine لهما دورات إصدار مستقلة

---

# 11. حدود ما يحق للمحرك تغييره

## ✅ ضمن صلاحيات المحرك (ضمن Capability Envelope)

```
📁 Project Workspace/
  ├── 📁 src/              ← يعدّل وينشئ ويحذف (ضمن allowed_write_paths)
  ├── 📁 tests/            ← يعدّل وينشئ ويحذف
  ├── 📄 README.md         ← يمكن تحديثه
  ├── 📄 package.json      ← يمكن تحديثه ضمن المهمة
  ├── 📄 tsconfig.json     ← يمكن تعديله
  ├── 📁 public/           ← يمكن تعديله
  └── 📁 docs/             ← يمكن تعديله
```

## ❌ خارج صلاحيات المحرك

```
❌ .tera-workspace/          ← تابع للمنصة (قراءة فقط — لا كتابة أبداً)
❌ سياسات TeraSystem         ← تابع للمنصة حصراً
❌ Gate configurations       ← المنصة فقط
❌ بيانات العملاء الآخرين    ← صلاحية المنصة
❌ صلاحيات المستخدمين        ← المنصة فقط
❌ ملفات الحالة (state)      ← المحرك لا يملك ملفات حالة
```

## ⚠️ يتطلب موافقة المنصة (عبر ApprovalRequest)

```
⚠️ تغيير بنية المشروع الجذرية
⚠️ حذف ملفات حساسة (.env, secrets)
⚠️ تغيير إعدادات الأمان
⚠️ ترقية تبعيات رئيسية
⚠️ تغييرات جذرية في الـ API
⚠️ أي إجراء يتجاوز Capability Envelope
```

---

# 12. التصنيف المعماري للأدوات

## 12.1 التصنيف الحالي

| الأداة | التصنيف | الحالة |
|---|---|---|
| Bash, Write, Edit, Read, Glob, Grep | **Engine Tool** (أدوات تنفيذ برمجي) | ✅ موجودة ومستقرة |
| WebFetch, WebSearch | **Engine Tool** (أدوات بحث خارجي) | ✅ موجودة |
| apply_patch, question, skill, todowrite | **Engine Tool** | ✅ موجودة |
| `read_tera_workspace` | **Read-Only Adapter** (جسر قراءة من `.tera-workspace/` فقط) | ✅ موجودة — آمنة |
| `tera_list_tasks`, `tera_check_gates` | **Control Plane Tool** | ⏸️ معلقة — بعد Engine Contract |
| `tera_decision_log`, `tera_policy_config` | **Control Plane Tool** | 🔮 مستقبل |
| Config Bridge | **Control Plane Tool** | ⏸️ معلقة — بعد Engine Contract |

## 12.2 خطة انتقال read_tera_workspace

`read_tera_workspace` أداة مؤقتة (Transitional Adapter) تُستخدم حتى جاهزية Engine Gateway. لا يُسمح ببقاء مسارين دائمين ومختلفين لمصدر السياق.

| المرحلة | الحالة | وصف |
|---|---|---|
| **Phase 3 (الآن)** | **Active** | الأداة الوحيدة للوصول لبيانات المنصة من داخل المحرك |
| **Phase 4 (Gateway)** | **Fallback** | Engine Gateway يصبح القناة الأساسية. `read_tera_workspace` تبقى كـ fallback مؤقت إذا فشل IPC |
| **Phase 4.5** | **Deprecated** | إعلان `read_tera_workspace` كـ deprecated. أي استدعاء يُسجّل تحذيراً. لا تطوير جديد عليها |
| **Phase 5** | **Removed** | حذف `read_tera_workspace` نهائياً. Gateway يكفي. لا مساران دائمان |

**الجدول الزمني المتوقع:**

```
Phase 3.2.1 (الآن)        → read_tera_workspace = Active
Phase 4 (بناء Gateway)    → read_tera_workspace = Fallback
Phase 4.5 (تثبيت Gateway) → read_tera_workspace = Deprecated
Phase 5 (Workspace Mgmt)  → read_tera_workspace = Removed
```

**القاعدة:** لا مساران دائمان ومختلفان لمصدر السياق. Gateway يحل محل `read_tera_workspace` تدريجياً.

---

# 13. التزامات الطرفين

## TeraSystem Platform تتعهد بـ:

1. توفير سياق دقيق للمحرك قبل كل تكليف
2. عدم تغيير Engine Contract دون إشعار مسبق ومراجعة
3. احترام فصل الـ Workspaces
4. عدم الوصول لملفات المحرك الداخلية
5. توفير آلية تصعيد أخطاء للمستخدم
6. تحديد Capability Envelope واضح لكل مهمة
7. معالجة ExecutionResult واتخاذ القرار المناسب
8. بناء Engine Gateway ليحل محل `read_tera_workspace`

## TeraOpenCode Engine يتعهد بـ:

1. فرض Capability Envelope وقائيًا قبل كل عملية (File/Shell/Network/Git)
2. عدم تجاوز Capability Envelope المحدد في التكليف
3. عدم كتابة أو تعديل ملفات `.tera-workspace/`
4. عدم الاحتفاظ بحالة دائمة خارج Runtime Session
5. التعامل مع كل Project Workspace بشكل مستقل
6. الفشل بأمان عند حدوث خطأ
7. الإبلاغ عن أي محاولة اختراق للحدود (EngineError)
8. إرسال ExecutionResult واضح بعد كل مهمة
9. عدم محاولة تغيير Task State

---

# 14. Definition of Done — إغلاق Phase 3

| # | البند | يتحقق بـ | الحالة |
|---|---|---|---|
| 1 | Engine Contract v1.2 معتمد | قرار صريح من ماجد | ✅ |
| 2 | `read_tera_workspace` يعمل ومستقر | TypeCheck + Manual Test + Commit | ✅ |
| 3 | `read_tera_workspace` مُصنّف كـ Read-Only Adapter | Engine Contract Section 12 | ✅ |
| 4 | Context Source (Phase 3.1) يعمل ومستقر | TypeCheck + Manual Test | ✅ |
| 5 | `.tera-workspace/` لا يكتب فيه المحرك | Engine Contract Section 8.2 | ✅ |
| 6 | Task Ownership واضح (المنصة تملك، المحرك يُرسل فقط) | Engine Contract Section 3 | ✅ |
| 7 | Capability Envelope معرّف ومفهوم + يُفرض وقائيًا | Engine Contract Section 5 | ✅ |
| 8 | Session Model مفهوم ومُوثّق | Engine Contract Section 2 | ✅ |
| 9 | ملكية الكود واضحة (العميل يملك، المحرك مخوّل بالتعديل) | Engine Contract Section 1.3 | ✅ |
| 10 | Roadmap محدَّث ومقبول | ROADMAP.md | ✅ |
| 11 | لا يوجد تناقض في الوثائق | مراجعة شاملة | ✅ |
| 12 | لا يوجد أدوات جديدة تغير حالة النظام | لا commits جديدة بأدوات Control Plane | ✅ |

**النتيجة:** جميع بنود DoD محققة. Phase 3 مكتمل.

---

# 15. خريطة التطبيق

| المرحلة | النشاط | يتضمن |
|---|---|---|
| **✅ Phase 3 (مكتمل)** | بناء الأدوات القرائية + إقرار العقد | Context Source, read_tera_workspace (Active), Engine Contract v1.2 |
| **🔵 Phase 4 (مفتوح — تصميم)** | بناء Engine Gateway | Context API (Local IPC), Task API, Result API, Approval API |
| **Phase 5** | Workspace Management + حذف read_tera_workspace | Workspace Registry, Multi-Client Isolation, read_tera_workspace = Removed |
| **Phase 6** | Gateway API + Quality Gates | REST API, Gate Framework, Security Gates |
| **بعيد المدى** | محركات متعددة | Design Engine, Test Engine, Deploy Engine |

---

# الخلاصة

```
TeraSystem يقرر ماذا.
TeraOpenCode ينفذ كيف — ضمن Capability Envelope المحدد.
Capability Envelope تُفرض وقائيًا قبل كل عملية.
TeraSystem تستقبل ExecutionResult وتُطبّق Gates والموافقات.
TeraSystem وحدها تغير Task State.
TeraOpenCode لا يملك حالة دائمة — Runtime Session تنتهي عند إرسال النتيجة.
العميل يملك الكود، والمحرك جهة مخوّلة بالتعديل.
الـ .tera-workspace/ للقراءة فقط من المحرك — لا كتابة أبداً.
read_tera_workspace = أداة مؤقتة، Gateway يحل محلها.
الـ Engine Contract هو حدود العلاقة بين المنصة والمحرك.
```

---

*هذه وثيقة **Approved v1.2** — اعتمد من ماجد.*
*آخر تحديث: 2026-07-10.*
