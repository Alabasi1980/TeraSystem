# TERA GATEWAY PROTOCOL SPECIFICATION

## الملف: TERA_GATEWAY_PROTOCOL_SPEC.md
## المسار: .tera-workspace/
## الإصدار: 1.0
## التاريخ: 2026-07-10
## الحالة: ✅ Approved — المخرج الأول لـ Phase 4
## مرجع: Engine Contract v1.2

---

# 1. المقدمة

يحدد هذا البروتوكول آلية الاتصال الرسمية بين TeraSystem Platform و TeraOpenCode Engine عبر stdio IPC.

**الهدف:** قناة اتصال واضحة، موثوقة، وقابلة للتشخيص بين المنصة والمحرك.

**النوع:** Request/Response via stdio (JSON Lines).

---

# 2. مبادئ التصميم

| المبدأ | الشرح |
|---|---|
| **stdout نظيف** | رسائل البروتوكول فقط — لا logs ولا debug output |
| **stderr للـ logs** | كل output غير البروتوكولي يذهب لـ stderr |
| **JSON Lines** | كل رسالة = سطر واحد = JSON صالح |
| **Framing واضح** | لا يوجد framing معقد — سطر واحد = رسالة |
| **Correlation ID** | كل طلب له معرّف فريد يُعاد في الرد |
| **Timeout** | كل طلب له حد أقصى زمني |
| **Crash Safety** | سلوك واضح عند انهيار المحرك |
| **Minimal State** | المحرك Stateless — لا حالة بين الطلبات |

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

# 4. تنسيق الرسائل (Message Format)

## 4.1 البنية الأساسية

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

## 4.2 أنواع الرسائل

| النوع | الاتجاه | الوصف |
|---|---|---|
| `request` | Platform → Engine | طلب تنفيذ |
| `response` | Engine → Platform | رد على طلب |
| `notification` | كلاهما | إشعار لا يحتاج رد |
| `error` | كلاهما | خطأ |

---

# 5. Handshake — التأكيد الأولي

## 5.1 التسلسل

```
Platform → Engine: HandshakeRequest
Engine → Platform: HandshakeResponse
```

## 5.2 HandshakeRequest

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

## 5.3 HandshakeResponse (نجاح)

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

## 5.4 HandshakeResponse (فشل)

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

## 5.5 قواعد Handshake

```
1. المنصة تُرسل HandshakeRequest بعد تشغيل المحرك.
2. المحرك يتحقق من contract_version:
   - متوافق → HandshakeResponse (status: ok)
   - غير متوافق → HandshakeResponse (status: error) + إنهاء
3. إذا لم يُرسل المحرك رد خلال 5 ثوانٍ → المنصة تُغلق العملية.
4. Handshake يُصرّر مرة واحدة فقط عند بدء الجلسة.
```

---

# 6. طرق الاتصال (Methods)

## 6.1 Context Request

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

## 6.2 Task Assignment

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

## 6.3 Task Result

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
    "status": "completed | failed | error | timeout | blocked",
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

## 6.4 Approval Request

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

# 7. Correlation ID Rules

## 7.1 تكوين المعرّف

```
corr_[prefix]_[timestamp]_[random]

أمثلة:
  ctx_20260710_143001_a3b2c1
  task_20260710_143002_d4e5f6
  appr_20260710_143200_g7h8i9
```

## 7.2 القواعد

| القاعدة | الشرح |
|---|---|
| **فريد** | لا يمكن تكرار معرّف في نفس الجلسة |
| **ثابت** |一旦 أُرسل، لا يتغير |
| **مرتبط** | الرد يحمل نفس `id` الخاص بالطلب |
| **قابل للتتبع** | يُستخدم لربط الطلب بالرد في السجلات |
| **لا يتكرر** | كل طلب جديد = معرّف جديد |

---

# 8. Timeout Rules

## 8.1 المهل الزمنية

| الإجراء | المهلة | عند الانتهاء |
|---|---|---|
| **Handshake** | 5 ثوانٍ | المنصة تُغلق العملية |
| **Context** | 10 ثوانٍ | المحرك يُبلغ بخطأ timeout |
| **Task Assignment** | يعتمد على المهمة | حسب deadline في TaskAssignment |
| **Approval Request** | حسب response_deadline | المحرك يُكمل دون موافقة (يُسجل كـ blocked) |
| **استجابة عامة** | 30 ثانية كحد أقصى | المنصة تُعيد المحاولة أو تُلغي |

## 8.2 سلوك Timeout

```
عند انتهاء المهلة:
  1. المنصة تُسجّل الخطأ
  2. تقرر: إعادة المحاولة / إلغاء المهمة / إخطار المستخدم
  3. المحرك لا يُ executed أي شيء بعد انتهاء المهلة
```

---

# 9. Cancellation

## 9.1 إلغاء المهمة

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

## 9.2 سلوك الإلغاء

```
1. المنصة تُرسل task.cancel
2. المحرك يُوقف التنفيذ فوراً
3. المحرك يُرسل task.result بحالة "cancelled"
4. المحرك لا يُعدّل أي ملف بعد الإلغاء
5. إذا كان هناك ملفات معدّلة جزئياً → المحرك يُبلغ بها في outputs.files_changed
```

---

# 10. Crash / Restart Behavior

## 10.1 انهيار المحرك (Engine Crash)

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

## 10.2 إعادة التشغيل (Engine Restart)

```
عند إعادة تشغيل المحرك:
  1. المنصة تُرسل HandshakeRequest جديد
  2. المحرك الجديد يتحقق من contract_version
  3. إذا كان هناك مهمة جارية:
     - المحرك لا يعرف بها (Stateless)
     - المنصة تُعيد تقييم الحالة
     - المنصة تُرسل task.assign جديد إذا كان مطلوباً
```

## 10.3 لا State بين الجلسات

```
المحرك لا يحتفظ بأي حالة بين طلب وآخر.
كل طلب مستقل و.self-contained.
المنصة هي الجهة الوحيدة التي تملك تاريخ التنفيذ.
```

---

# 11. Message Size Limits

## 11.1 الحد الأقصى للرسائل

| نوع الرسالة | الحد الأقصى |
|---|---|
| HandshakeRequest | 4 KB |
| HandshakeResponse | 4 KB |
| Context | 1 MB |
| TaskAssignment | 64 KB |
| TaskResult | 256 KB |
| ApprovalRequest | 16 KB |
| ApprovalResponse | 4 KB |
| Notification | 16 KB |
| Error | 16 KB |

## 11.2 تمرير الملفات الكبيرة

**المشكلة:** لا يُسمح بإدراج محتوى ملفات كبيرة داخل JSON.

**الحل:** تمرير الملفات كمراجع (references) بدلاً من إدراجها:

```json
{
  "method": "task.assign",
  "inputs": {
    "files": [
      {
        "path": "src/components/LoginForm.tsx",
        "reference": "file://ws_abc123/src/components/LoginForm.tsx"
      }
    ]
  }
}
```

**القواعد:**
- `reference` = مسار نسبي من Workspace Root
- المحرك يقرأ الملف من المسار مباشرة (لا نسخة في JSON)
- إذا كان الملف كبيراً جداً (>1MB) → يُمرّر كـ `reference` دائماً
- إذا كان الملف صغيراً (<64KB) → يمكن إدراجه كـ `inline_content` (اختياري)

---

# 12. Error Handling

## 12.1 تنسيق الخطأ

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

## 12.2 قواعد الأخطاء

| القاعدة | الشرح |
|---|---|
| **stdout نظيف** | لا أخطاء في stdout — stderr فقط للـ logs |
| **الخطأ رد** | الخطأ يُرسل كرد على الطلب (نفس correlation_id) |
| **fatal vs non-fatal** | `fatal: true` → توقف المهمة. `fatal: false` → يمكن المتابعة |
| **لا سلسلة أخطاء** | لا يُرسل المحرك أخطاء متعددة لنفس المشكلة |

---

# 13. stderr Logging

## 13.1 ما يذهب لـ stderr

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

## 13.2 تنسيق stderr (اختياري)

```
[2026-07-10T14:30:00.000Z] [INFO] Engine started
[2026-07-10T14:30:01.000Z] [DEBUG] Context received, 2048 bytes
[2026-07-10T14:30:02.000Z] [INFO] Task assigned: t_abc123
[2026-07-10T14:30:03.000Z] [WARN] Large file detected: 2.1MB
[2026-07-10T14:35:00.000Z] [INFO] Task completed: t_abc123 (30s)
```

---

# 14. جدول الطريقة الكامل

```
1. Platform يشغّل TeraOpenCode كـ child process
2. Platform يُرسل HandshakeRequest
3. Engine يرد بـ HandshakeResponse
4. Platform يُرسل ContextRequest
5. Engine يرد بـ ContextResponse
6. Platform يُرسل TaskAssignment
7. Engine يبدأ Runtime Session
8. Engine يرفض أي إجراء يتجاوز Capability Envelope
9. Engine يُرسل ApprovalRequest إذا احتاج موافقة
10. Platform يرد بـ ApprovalResponse
11. Engine يُكمل التنفيذ
12. Engine يُرسل TaskResult
13. Platform يستقبل النتيجة
14. Platform يُطبّق Gates والموافقات
15. Platform يُغير Task State
16. تكرار من الخطوة 6 للمهمة التالية
17. عند الانتهاء → Platform يُغلق العملية
```

---

# 15. الأحداث المستقبلية (Future Events)

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

# 16. ملخص الطرق

| الطريقة | الاتجاه | تحتاج رد؟ | الحالة |
|---|---|---|---|
| `handshake` | P→E → E→P | ✅ نعم | Phase 4 |
| `context` | P→E → E→P | ✅ نعم | Phase 4 |
| `task.assign` | P→E | ❌ (الرد = task.result) | Phase 4 |
| `task.result` | E→P | ❌ (رد على task.assign) | Phase 4 |
| `task.cancel` | P→E | ❌ (notification) | Phase 4 |
| `approval.request` | E→P | ✅ نعم | Phase 4 |
| `approval.response` | P→E | ❌ (رد على approval.request) | Phase 4 |
| `engine.progress` | E→P | ❌ (notification) | مستقبل |
| `gate.check` | P→E | ✅ نعم | Phase 6 |

---

*هذه وثيقة **Approved v1.0** — البروتوكول الرسمي لـ Tera Gateway.*
*آخر تحديث: 2026-07-10.*
