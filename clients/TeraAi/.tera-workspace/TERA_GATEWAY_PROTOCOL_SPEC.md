# TERA GATEWAY PROTOCOL SPECIFICATION

## الملف: TERA_GATEWAY_PROTOCOL_SPEC.md
## المسار: .tera-workspace/
## الإصدار: 1.1
## التاريخ: 2026-07-10
## الحالة: Draft — Pending Phase 4.1 Review
## مرجع: Engine Contract v1.2

---

# 1. المقدمة

يحدد هذا البروتوكول آلية الاتصال الرسمية بين TeraSystem Platform و TeraOpenCode Engine عبر stdio IPC.

**الهدف:** قناة اتصال واضحة، موثوقة، وقابلة للتشخيص بين المنصة والمحرك.

**النوع:** Request/Response via stdio (JSON Lines).

**ملاحظة الحالة:** هذه وثيقة **Draft**. لا تُعتمد إلا بعد انتهاء مراجعة Phase 4.1.

---

# 2. مبادئ التصميم

| المبدأ | الشرح |
|---|---|
| **stdout نظيف** | رسائل البروتوكول فقط — لا logs ولا debug output |
| **stderr للـ logs** | كل output غير البروتوكولي يذهب لـ stderr |
| **JSON Lines** | كل رسالة = سطر واحد = JSON صالح |
| **Framing واضح** | لا يوجد framing معقد — سطر واحد = رسالة |
| **Correlation ID** | كل طلب له معرّف فريد يُعاد في الرد |
| **Timeout + Grace Period** | انتهاء مهلة → إلغاء → فحص → kill |
| **Crash Safety** | سلوك واضح عند انهيار المحرك |
| **Stateless (business)** | لا حالة أعمال دائمة — Runtime Session مؤقتة فقط |

---

# 3. نموذج الاتصال

```
┌─────────────────────┐                    ┌─────────────────────┐
│  TeraSystem         │                    │  TeraOpenCode       │
│  Platform           │                    │  Engine             │
│                     │                    │                     │
│  stdout ────────────────────────────────→ stdin               │
│        ←────────────────────────────────  stdout              │
│  stderr ←─────────────────────────────── stderr               │
│                     │                    │                     │
│  يشغّل المحرك كـ   │                    │  يستقبل أوامر من   │
│  child process      │                    │  stdin، يرد عبر    │
│                     │                    │  stdout             │
└─────────────────────┘                    └─────────────────────┘
```

**الاتجاه:**
- المنصة → stdin → المحرك (طلبات)
- المحرك → stdout → المنصة (ردود)
- المحرك → stderr → المنصة (logs)

---

# 4. نموذج الحالة (State Model)

## 4.1 ما يملكه المحرك وما لا يملكه

```
TeraOpenCode Engine:
  ├── ❌ لا يملك durable business state
  │     (لا سجل مهام، لا سجل قرارات، لا ذاكرة بين جلسات)
  │
  ├── ✅ يملك ephemeral runtime state أثناء عمر المهمة
  │     (حالة التنفيذ الحالية، مؤقت، ينتهي عند النتيجة أو الإلغاء)
  │
  └── ❌ لا يملك أي حالة بعد إرسال TaskResult
```

## 4.2 الفرق الجوهري

| البند | durable business state | ephemeral runtime state |
|---|---|---|
| **المالك** | TeraSystem فقط | TeraOpenCode (مؤقتاً) |
| **المدة** | دائمة | عمر المهمة فقط |
| **الأمثلة** | سجل المهام، القرارات، الجلسات | ملفات مؤقتة في الذاكرة، سياق المهمة |
| **بعد المهمة** | يبقى في المنصة | يُمحى — لا أثر |

## 4.3 دورة حياة الـ Runtime State

```
تبدأ: عند استلام TaskAssignment
  │
  ├── المحرك يحفظ context المهمة في الذاكرة
  ├── المحرك يحفظ files المدخلة مؤقتاً
  ├── المحرك يتتبع progress التنفيذ
  │
  └── تنتهي عند أحد:
       ├── إرسال TaskResult (completed/failed/error/cancelled)
       ├── انتهاء timeout
       ├── استلام task.cancel
       └── crash
```

---

# 5. تنسيق الرسائل (Message Format)

## 5.1 البنية الأساسية

```json
{
  "type": "request | response | notification | error",
  "id": "corr_abc123",
  "timestamp": "2026-07-10T14:30:00.000Z",
  "payload": { ... }
}
```

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `type` | string | ✅ | نوع الرسالة |
| `id` | string | ✅ | معرّف التتبع (correlation_id) |
| `timestamp` | string | ✅ | ISO 8601 UTC |
| `payload` | object | ✅ | محتوى الرسالة |

## 5.2 أنواع الرسائل

| النوع | الاتجاه | الوصف |
|---|---|---|
| `request` | Platform → Engine | طلب تنفيذ |
| `response` | Engine → Platform | رد على طلب |
| `notification` | كلاهما | إشعار لا يحتاج رد |
| `error` | كلاهما | خطأ |

---

# 6. Handshake — التأكيد الأولي

## 6.1 التسلسل

```
Platform → Engine: HandshakeRequest
Engine → Platform: HandshakeResponse
```

## 6.2 HandshakeRequest

```json
{
  "type": "request",
  "id": "handshake_001",
  "timestamp": "2026-07-10T14:30:00.000Z",
  "payload": {
    "method": "handshake",
    "contract_version": "1.2",
    "platform_version": "0.1.0",
    "engine_version": "0.1.0",
    "workspace_id": "ws_abc123",
    "project_id": "proj_xyz789"
  }
}
```

## 6.3 HandshakeResponse (نجاح)

```json
{
  "type": "response",
  "id": "handshake_001",
  "timestamp": "2026-07-10T14:30:00.100Z",
  "payload": {
    "method": "handshake",
    "status": "ok",
    "engine_version": "0.1.0",
    "contract_version": "1.2",
    "supported_methods": ["context", "task.assign", "task.result", "approval.request", "approval.response"]
  }
}
```

## 6.4 HandshakeResponse (فشل)

```json
{
  "type": "response",
  "id": "handshake_001",
  "timestamp": "2026-07-10T14:30:00.100Z",
  "payload": {
    "method": "handshake",
    "status": "error",
    "error": {
      "code": "VERSION_MISMATCH",
      "message": "Engine requires contract_version >= 1.3, got 1.2"
    }
  }
}
```

## 6.5 قواعد Handshake

```
1. المنصة تُرسل HandshakeRequest بعد تشغيل المحرك.
2. المحرك يتحقق من contract_version:
   - متوافق → HandshakeResponse (status: ok)
   - غير متوافق → HandshakeResponse (status: error) + إنهاء
3. إذا لم يُرسل المحرك رد خلال 5 ثوانٍ → المنصة تُغلق العملية.
4. Handshake يُصرّر مرة واحدة فقط عند بدء الجلسة.
```

---

# 7. طرق الاتصال (Methods)

## 7.1 Context Request

**الغرض:** المنصة تُرسل السياق للمحرك.

```json
{
  "type": "request",
  "id": "ctx_001",
  "timestamp": "2026-07-10T14:30:01.000Z",
  "payload": {
    "method": "context",
    "context_type": "system | project | task",
    "context_data": "-markdown or JSON string-",
    "workspace_id": "ws_abc123",
    "project_id": "proj_xyz789",
    "capabilities": {
      "allowed_read_paths": ["src/", "tests/"],
      "allowed_write_paths": ["src/"],
      "forbidden_paths": ["*.env", ".secrets/"],
      "allowed_commands": ["bun test"],
      "forbidden_commands": ["rm -rf"],
      "network_policy": { "internet_access": false, "allowed_domains": [] },
      "approval_required": { "destructive_actions": true, "security_sensitive": true, "major_dependency_changes": true }
    }
  }
}
```

**الرد:**

```json
{
  "type": "response",
  "id": "ctx_001",
  "timestamp": "2026-07-10T14:30:01.200Z",
  "payload": {
    "method": "context",
    "status": "ok",
    "acknowledged": true
  }
}
```

## 7.2 Task Assignment

**الغرض:** المنصة تُكلّف المحرك بمهمة.

```json
{
  "type": "request",
  "id": "task_001",
  "timestamp": "2026-07-10T14:30:02.000Z",
  "payload": {
    "method": "task.assign",
    "task_id": "t_abc123",
    "description": "Create a login form component",
    "inputs": {
      "context": "We need a React login form...",
      "files": ["src/components/"],
      "previous_results": []
    },
    "capabilities": { ... },
    "priority": "normal",
    "platform_session_id": "ps_xyz789",
    "deadline": "2026-07-10T15:00:00.000Z"
  }
}
```

## 7.3 Task Result

**الغرض:** المحرك يُرسل نتيجة تنفيذ المهمة.

```json
{
  "type": "response",
  "id": "task_001",
  "timestamp": "2026-07-10T14:35:00.000Z",
  "payload": {
    "method": "task.result",
    "task_id": "t_abc123",
    "engine_runtime_session_id": "ers_abc123",
    "status": "completed | failed | error | timeout | blocked | cancelled | approval_timeout",
    "outputs": {
      "files_changed": ["src/components/LoginForm.tsx"],
      "files_created": ["src/components/LoginForm.tsx"],
      "files_deleted": [],
      "commands_run": ["bun test src/components/LoginForm.test.tsx"],
      "test_results": { "total": 3, "passed": 3, "failed": 0 },
      "summary": "Created LoginForm component with email/password fields"
    },
    "errors": [],
    "duration_ms": 30000,
    "started_at": "2026-07-10T14:30:02.500Z",
    "completed_at": "2026-07-10T14:35:00.000Z",
    "notes": "All tests passing"
  }
}
```

## 7.4 Approval Request

**الغرض:** المحرك يطلب موافقة على إجراء حساس.

```json
{
  "type": "request",
  "id": "appr_001",
  "timestamp": "2026-07-10T14:32:00.000Z",
  "payload": {
    "method": "approval.request",
    "request_id": "ar_abc123",
    "task_id": "t_abc123",
    "action_type": "destructive",
    "description": "Delete old test files that are no longer needed",
    "details": {
      "affected_files": ["tests/old-test.tsx"],
      "affected_commands": ["rm tests/old-test.tsx"],
      "risk_level": "medium"
    },
    "response_deadline": "2026-07-10T14:35:00.000Z"
  }
}
```

**الرد:**

```json
{
  "type": "response",
  "id": "ar_abc123",
  "timestamp": "2026-07-10T14:33:00.000Z",
  "payload": {
    "method": "approval.response",
    "request_id": "ar_abc123",
    "approved": true,
    "reason": "Approved - old tests are obsolete",
    "approved_by": "majed"
  }
}
```

---

# 8. Correlation ID Rules

## 8.1 تكوين المعرّف

```
corr_[prefix]_[timestamp]_[random]

أمثلة:
  ctx_20260710_143001_a3b2c1
  task_20260710_143002_d4e5f6
  appr_20260710_143200_g7h8i9
```

## 8.2 القواعد

| القاعدة | الشرح |
|---|---|
| **فريد** | لا يمكن تكرار معرّف في نفس الجلسة |
| **ثابت** |一旦 أُرسل، لا يتغير |
| **مرتبط** | الرد يحمل نفس `id` الخاص بالطلب |
| **قابل للتتبع** | يُستخدم لربط الطلب بالرد في السجلات |
| **لا يتكرر** | كل طلب جديد = معرّف جديد |

---

# 9. Timeout Rules

## 9.1 المهل الزمنية

| الإجراء | المهلة | عند الانتهاء |
|---|---|---|
| **Handshake** | 5 ثوانٍ | المنصة تُغلق العملية |
| **Context** | 10 ثوانٍ | المحرك يُبلغ بخطأ timeout |
| **Task Assignment** | يعتمد على المهمة | حسب deadline في TaskAssignment |
| **Approval Request** | حسب response_deadline | **الإجراء لا يُنفذ. المهمة تصبح blocked أو approval_timeout** |
| **استجابة عامة** | 30 ثانية كحد أقصى | المنصة تُعيد المحاولة أو تُلغي |

## 9.2 قاعدة Approval Timeout — حماية صارمة

```
⚠️ قاعدة مطلقة: المحرك لا يكمل أي إجراء محمي بدون موافقة.

عند انتهاء approval timeout:
  1. الإجراء المحمي لا يُنفذ نهائياً
  2. المحرك لا يتجاوز الـ Capability Envelope
  3. المحرك يُرسل TaskResult بحالة "blocked" أو "approval_timeout"
  4. المنصة تُسجّل الحدث وتُخطر المستخدم
  5. لا يُسمح للمحرك باتخاذ بديل أو المتابعة بدون موافقة

❌ ممنوع: المحرك يكمل دون موافقة ويُبلّغ بعدها
❌ ممنوع: المحرك يُoclara أن الإجراء "ليس محمياً" ويتجاوز الـ Envelope
✅ المطلوب: الإجراء يتوقف → المهمة تصبح blocked → المنصة تقرر
```

## 9.3 سلوك Timeout العام

```
عند انتهاء أي timeout:
  1. المنصة تُرسل task.cancel
  2. المنصة تنتظر grace period (5 ثوانٍ)
  3. إذا لم يتوقف المحرك → المنصة تقتل child process (SIGTERM، ثم SIGKILL بعد 3 ثوانٍ)
  4. المنصة تُقيّم الحالة وتُخطر المستخدم
  5. لا تُرسل أي طلب آخر للمحرك حتى إعادة التشغيل
```

---

# 10. Cancellation

## 10.1 إلغاء المهمة (من المنصة)

```json
{
  "type": "notification",
  "id": "cancel_001",
  "timestamp": "2026-07-10T14:35:00.000Z",
  "payload": {
    "method": "task.cancel",
    "task_id": "t_abc123",
    "reason": "Customer changed requirements"
  }
}
```

## 10.2 سلوك الإلغاء

```
1. المنصة تُرسل task.cancel
2. المحرك يُوقف التنفيذ فوراً
3. المحرك يُرسل task.result بحالة "cancelled"
4. المحرك لا يُعدّل أي ملف بعد الإلغاء
5. إذا كان هناك ملفات معدّلة جزئياً → المحرك يُبلغ بها في outputs.files_changed
```

## 10.3 Grace Period + Force Kill

```
عند إرسال task.cancel:
  │
  ├── المنصة تُرسل task.cancel عبر stdin
  │
  ├── المنصة تنتظر grace period = 5 ثوانٍ
  │     │
  │     ├── المحرك أرسل task.result خلال Grace Period → قبول
  │     │
  │     └── المحرك لم يُرسل → المنصة تقتل العملية:
  │           1. SIGTERM (إشعار بلطف)
  │           2. انتظار 3 ثوانٍ
  │           3. SIGKILL (قتل قسري)
  │
  └── المنصة تُقيّم الحالة وتُخطر المستخدم
```

---

# 11. Crash / Restart Behavior

## 11.1 انهيار المحرك (Engine Crash)

```
عند انهيار المحرك (exit code != 0 أو قطع stdout):
  1. المنصة تُسجّل الحدث
  2. المنصة تُقيّم الحالة:
     - لم يبدأ تنفيذ → إعادة تشغيل المحرك
     - كان ينفذ → مراجعة Workspace لتحديد ما تم
     - أرسل نتيجة قبل الانهيار → قبول النتيجة
  3. المنصة تُخطر المستخدم
  4. لا تُرسل أي طلب آخر للمحرك المنهار
```

## 11.2 إعادة التشغيل (Engine Restart)

```
عند إعادة تشغيل المحرك:
  1. المنصة تُرسل HandshakeRequest جديد
  2. المحرك الجديد يتحقق من contract_version
  3. إذا كان هناك مهمة جارية:
     - المحرك لا يعرف بها ( Stateless business — لا حالة أعمال دائمة )
     - المنصة تُعيد تقييم الحالة
     - المنصة تُرسل task.assign جديد إذا كان مطلوباً
```

---

# 12. Message Size Limits (موحد)

**القاعدة الموحدة:** جميع الأحجام بالبايت، وتُحذف الرسائل الأكبر.

| نوع الرسالة | الحد الأقصى (بايت) | الحد الأقصى (مقروء) |
|---|---|---|
| HandshakeRequest | 4,096 | 4 KB |
| HandshakeResponse | 4,096 | 4 KB |
| ContextRequest | 1,048,576 | 1 MB |
| ContextResponse | 4,096 | 4 KB |
| TaskAssignment | 65,536 | 64 KB |
| TaskResult | 262,144 | 256 KB |
| ApprovalRequest | 16,384 | 16 KB |
| ApprovalResponse | 4,096 | 4 KB |
| task.cancel | 4,096 | 4 KB |
| Error | 16,384 | 16 KB |

**القواعد:**
- أي رسالة تتجاوز الحد الأقصى → **تُرفض ولا تُرسل**
- إذا كان المحتوى أكبر → استخدم File Reference بدلاً من الإدراج
- stderr logs ليس لها حد أقصى (لكن يُنصح بالاعتدال)

## 12.1 File References — حماية صارمة

### البنية

```json
{
  "path": "src/components/LoginForm.tsx",
  "reference": "src/components/LoginForm.tsx"
}
```

### القواعد الأمنية (إلزامية)

```
1. canonicalization:
   - المسار يُحوّل إلىcanonical path قبل أي استخدام
   - مثال: src/../src/foo.ts → src/foo.ts

2. منع absolute paths:
   - لا يُسمح بمسارات مطلقة (C:\..., /home/...)
   - كل مسار يجب أن يكون نسبياً من Workspace Root

3. منع directory traversal:
   - لا يُسمح بـ "../" في أي مسار
   - مثال: ../../etc/passwd → ❌ مرفوض

4. منع symlink escape:
   - إذا كان الملف symlink → يُتحقق أن الهدف داخل Workspace
   - symlink يشير خارج Workspace → ❌ مرفوض

5. تطابق workspace_id:
   - الـ reference يجب أن يتوافق مع workspace_id في الطلب
   - مثال: إذا كان workspace_id = "ws_abc123" → المسار داخل ws_abc123/

6. فحص Capability Envelope:
   - قبل قراءة أي ملف → فحص ضد allowed_read_paths
   - قبل كتابة أي ملف → فحص ضد allowed_write_paths
   - الملف في forbidden_paths → ❌ مرفوض دائماً

7. لا inline content للملفات الكبيرة:
   - ملف > 64KB → يُمرّر كـ reference فقط
   - ملف < 64KB → يمكن إدراجه كـ inline_content (اختياري)
```

### مثال كامل

```json
{
  "method": "task.assign",
  "inputs": {
    "files": [
      {
        "path": "src/components/LoginForm.tsx",
        "reference": "src/components/LoginForm.tsx"
      }
    ]
  }
}
```

---

# 13. Error Handling

## 13.1 تنسيق الخطأ

```json
{
  "type": "error",
  "id": "err_001",
  "timestamp": "2026-07-10T14:35:00.000Z",
  "payload": {
    "method": "task.result",
    "error_type": "tool_failure | permission_denied | capability_violation | model_error | timeout | internal_error",
    "error_code": "ERR_TOOL_FAILURE",
    "message": "Read tool failed: file not found",
    "details": {
      "file": "src/missing.ts",
      "tool": "read"
    },
    "fatal": true
  }
}
```

## 13.2 قواعد الأخطاء

| القاعدة | الشرح |
|---|---|
| **stdout نظيف** | لا أخطاء في stdout — stderr فقط للـ logs |
| **الخطأ رد** | الخطأ يُرسل كرد على الطلب (نفس correlation_id) |
| **fatal vs non-fatal** | `fatal: true` → توقف المهمة. `fatal: false` → يمكن المتابعة |
| **لا سلسلة أخطاء** | لا يُرسل المحرك أخطاء متعددة لنفس المشكلة |

---

# 14. stderr Logging

## 14.1 ما يذهب لـ stderr

```
✅ stderr:
  - Debug logs
  - Warning logs
  - Error logs
  - Performance metrics
  - Progress indicators

❌ stdout (محظور):
  - أي شيء غير JSON Lines
  - Logs
  - Debug output
  - Prints عادية
```

## 14.2 تنسيق stderr (اختياري)

```
[2026-07-10T14:30:00.000Z] [INFO] Engine started
[2026-07-10T14:30:01.000Z] [DEBUG] Context received, 2048 bytes
[2026-07-10T14:30:02.000Z] [INFO] Task assigned: t_abc123
[2026-07-10T14:30:03.000Z] [WARN] Large file detected: 2.1MB
[2026-07-10T14:35:00.000Z] [INFO] Task completed: t_abc123 (30s)
```

---

# 15. جدول الطريقة الكامل

```
1.  Platform يشغّل TeraOpenCode كـ child process
2.  Platform يُرسل HandshakeRequest
3.  Engine يرد بـ HandshakeResponse
4.  Platform يُرسل ContextRequest
5.  Engine يرد بـ ContextResponse
6.  Platform يُرسل TaskAssignment
7.  Engine يبدأ Runtime Session (ephemeral state تبدأ)
8.  Engine يرفض أي إجراء يتجاوز Capability Envelope
9.  Engine يُرسل ApprovalRequest إذا احتاج موافقة
    ⚠️ Engine ينتظر ApprovalResponse — لا يكمل بدون موافقة
10. Platform يرد بـ ApprovalResponse
11. Engine يُكمل التنفيذ
12. Engine يُرسل TaskResult (ephemeral state تنتهي)
13. Platform يستقبل النتيجة
14. Platform يُطبّق Gates والموافقات
15. Platform يُغير Task State
16. تكرار من الخطوة 6 للمهمة التالية
17. عند الانتهاء → Platform يُغلق العملية
```

---

# 16. الأحداث المستقبلية (Future Events)

**مُؤجَّل** — لا يُنفَّذ في Phase 4.

عند الحاجة التشغيلية المثبتة، يمكن إضافة:

```json
{
  "type": "notification",
  "id": "evt_001",
  "payload": {
    "method": "engine.progress",
    "task_id": "t_abc123",
    "progress": 0.65,
    "message": "65% complete"
  }
}
```

---

# 17. ملخص الطرق

| الطريقة | الاتجاه | تحتاج رد؟ | الحالة |
|---|---|---|---|
| `handshake` | P→E → E→P | ✅ نعم | Phase 4 |
| `context` | P→E → E→P | ✅ نعم | Phase 4 |
| `task.assign` | P→E | ❌ (الرد = task.result) | Phase 4 |
| `task.result` | E→P | ❌ (رد على task.assign) | Phase 4 |
| `task.cancel` | P→E | ❌ (notification) | Phase 4 |
| `approval.request` | E→P | ✅ نعم (إلزامي) | Phase 4 |
| `approval.response` | P→E | ❌ (رد على approval.request) | Phase 4 |
| `engine.progress` | E→P | ❌ (notification) | مستقبل |
| `gate.check` | P→E | ✅ نعم | Phase 6 |

---

# 18. ملخص التعديلات (Changelog)

| الإصدار | التعديل |
|---|---|
| v1.0 | النسخة الأولى |
| v1.1 | إضافة: ephemeral runtime state model، approval timeout protection (لا يكمل بدون موافقة)، timeout → cancel → grace period → kill، File Reference protection (canonicalization, no absolute, no ../, no symlink, workspace_id match, envelope check)، توحيد message size limits، حالة Draft — Pending Review |

---

*هذه وثيقة **Draft v1.1** — الحالة: **Pending Phase 4.1 Review**.*
*لا تُعتمد إلا بعد انتهاء المراجعة والموافقة.*
