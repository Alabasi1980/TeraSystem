# مرجع API بوابة TeraGateway

**الملف:** GATEWAY_API_REFERENCE.md  
**المسار:** `.tera-workspace/`  
**الإصدار:** 1.0  
**التاريخ:** 2026-07-12  
**المرجع:** بروتوكول TeraGateway v1.2

---

## 1. نظرة عامة

**TeraGateway** هي واجهة اتصال نصية (CLI Gateway) بين **TeraSystem Platform** ومحرك **TeraOpenCode**. تهدف القناة إلى توفير اتصال واضح، موثوق، وقابل للتشخيص بين المنصة والمحرك.

تعمل البوابة عبر **stdio JSON Lines** — حيث تستمع إلى stdin وتكتب الردود على stdout. يمكن للمنصة تشغيل البوابة كعملية تابعة (child process) والتواصل معها بسهولة دون الحاجة إلى HTTP أو WebSocket.

### لماذا Gateway بدلاً من HTTP API؟
- لا حاجة لخادم HTTP قائم — اتصال مباشر عبر stdio
- لا مشاكل CORS أو جدران نارية
- لا حاجة لمصادقة شبكية (المصادقة عبر البروتوكول نفسه)
- بدء فوري — لا وقت إقلاع للخادم
- مثالي للتكامل مع أدوات CLI والأنظمة النصية

---

## 2. طبقة النقل (Transport)

### stdio — JSON Lines

- **stdin**: المنصة تُرسل الطلبات (requests) إلى المحرك عبر stdin
- **stdout**: المحرك يرد (responses/errors) عبر stdout — خط JSON واحد لكل رسالة
- **stderr**: التشخيصات والسجلات فقط (logs, warnings, diagnostics)

```
المنصة → stdin ← المحرك  (طلبات)
المحرك → stdout ← المنصة (ردود)
المحرك → stderr ← المنصة (سجلات تشخيصية)
```

### قواعد stdout الصارمة

| ما يسمح على stdout | ما يمنع على stdout |
|---|---|
| `response` كـ JSON Line | أي output غير JSON |
| `error` كـ JSON Line | logs نصية |
| `notification` كـ JSON Line | debug output |
| | رسائل طباعة عادية |

### تنسيق stderr (اختياري — مقترح)

```
[2026-07-10T14:30:00.000Z] [INFO] Engine started
[2026-07-10T14:30:01.000Z] [WARN] Large message detected
[2026-07-10T14:30:02.000Z] [ERROR] Unexpected error occurred
```

---

## 3. غلاف الرسالة (Message Envelope)

كل رسالة — طلباً كانت أم رداً — تتبع هذا الهيكل الموحد:

```json
{
  "type": "request | response | notification | error",
  "id": "corr_abc123",
  "timestamp": "2026-07-10T14:30:00.000Z",
  "payload": { ... }
}
```

### الحقول

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `type` | string | ✅ | نوع الرسالة |
| `id` | string | ✅ | معرّف التتبع (correlation ID) — يُعاد في الرد كما هو |
| `timestamp` | string | ✅ | طابع زمني ISO 8601 UTC |
| `payload` | object | ✅ | محتوى الرسالة الخاص بالطريقة |

### أنواع الرسائل

| النوع | الاتجاه | الوصف |
|---|---|---|
| `request` | Platform → Engine | طلب لتنفيذ إجراء |
| `response` | Engine → Platform | رد ناجح على طلب |
| `notification` | كلا الاتجاهين | إشعار أحادي الاتجاه لا يحتاج رد |
| `error` | Engine → Platform | خطأ بروتوكولي منظم |

---

## 4. طريقة Handshake — المصافحة

### الغرض
تأكيد التوافق بين المنصة والمحرك قبل بدء أي اتصال. المصافحة هي أول رسالة ترسلها المنصة بعد تشغيل المحرك، ولا يمكن إرسال أي طلب آخر قبل نجاحها.

### الطلب (HandshakeRequest)

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

### الرد عند النجاح (HandshakeResponse)

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
    "supported_methods": ["context", "task", "approval"]
  }
}
```

### الرد عند فشل الإصدار (HandshakeResponse — VERSION_MISMATCH)

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
      "message": "Engine supports contract_version 1.2, got 9.9"
    }
  }
}
```

### حقول الطلب

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `method` | string | ✅ | يجب أن يكون `"handshake"` |
| `contract_version` | string | ✅ | إصدار بروتوكول الاتصال (يجب أن يطابق `1.2`) |
| `platform_version` | string | ✅ | إصدار المنصة |
| `engine_version` | string | ✅ | إصدار المحرك |
| `workspace_id` | string | ✅ | معرّف مساحة العمل |
| `project_id` | string | ✅ | معرّف المشروع |

### حقول الرد (نجاح)

| الحقل | النوع | الوصف |
|---|---|---|
| `method` | string | `"handshake"` |
| `status` | string | `"ok"` |
| `engine_version` | string | إصدار المحرك الفعلي |
| `contract_version` | string | إصدار البروتوكول المعتمد |
| `supported_methods` | string[] | قائمة الطرق المدعومة |

### قواعد المصافحة

1. تُرسل المصافحة مرة واحدة فقط عند بدء الجلسة
2. يجب أن يطابق `contract_version` إصدار المحرك، وإلا يُرفض الطلب ويُغلق الاتصال
3. يجب توفير `workspace_id` و `project_id` — بدونهما يُرفض الطلب
4. حد حجم الرسالة: **4,096 بايت** (4 KB)

---

## 5. طريقة Context — السياق

### الغرض
إرسال سياق التنفيذ من المنصة إلى المحرك بعد نجاح المصافحة. السياق يحدد البيئة التي سيعمل فيها المحرك: الصلاحيات، المسارات المسموحة، الأوامر المسموحة، إلخ.

### الطلب (ContextRequest)

```json
{
  "type": "request",
  "id": "ctx_001",
  "timestamp": "2026-07-10T14:30:01.000Z",
  "payload": {
    "method": "context",
    "context_type": "project",
    "context_data": "Full project context in markdown or JSON",
    "workspace_id": "ws_abc123",
    "project_id": "proj_xyz789",
    "capabilities": {
      "allowed_read_paths": ["src/", "tests/"],
      "allowed_write_paths": ["src/"],
      "forbidden_paths": ["*.env", ".secrets/"],
      "allowed_commands": ["bun test"],
      "forbidden_commands": ["rm -rf"],
      "network_policy": {
        "internet_access": false,
        "allowed_domains": []
      },
      "approval_required": {
        "destructive_actions": true,
        "security_sensitive": true,
        "major_dependency_changes": true
      }
    }
  }
}
```

### الرد عند النجاح (ContextResponse)

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

### حقول الطلب

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `method` | string | ✅ | `"context"` |
| `context_type` | string | ✅ | نوع السياق: `"system"`, `"project"`, أو `"task"` |
| `context_data` | string | ✅ | محتوى السياق (نص أو JSON) |
| `workspace_id` | string | ✅ | يجب أن يطابق `workspace_id` من المصافحة |
| `project_id` | string | ✅ | يجب أن يطابق `project_id` من المصافحة |
| `capabilities` | object | ✅ | غلاف الصلاحيات — يحدد حدود عمل المحرك |

### هيكل غلاف الصلاحيات (Capability Envelope)

| الحقل | النوع | الوصف |
|---|---|---|
| `allowed_read_paths` | string[] | مسارات يُسمح للمحرك بقراءتها |
| `allowed_write_paths` | string[] | مسارات يُسمح للمحرك بالكتابة فيها |
| `forbidden_paths` | string[] | مسارات ممنوعة (أنماط glob) |
| `allowed_commands` | string[] | أوامر shell مسموح بتنفيذها |
| `forbidden_commands` | string[] | أوامر shell ممنوعة |
| `network_policy.internet_access` | boolean | هل يُسمح بالاتصال بالإنترنت |
| `network_policy.allowed_domains` | string[] | نطاقات مسموح بالاتصال بها (إذا كان `internet_access = true`) |
| `approval_required.destructive_actions` | boolean | هل تحتاج الإجراءات التدميرية موافقة |
| `approval_required.security_sensitive` | boolean | هل تحتاج الإجراءات الأمنية موافقة |
| `approval_required.major_dependency_changes` | boolean | هل تحتاج تغييرات التبعيات موافقة |

### قواعد السياق

1. يجب إرسال المصافحة بنجاح قبل أي طلب سياق
2. يجب أن يطابق `workspace_id` و `project_id` القيم من المصافحة
3. غلاف الصلاحيات إلزامي — بدونه يُرفض الطلب
4. حد حجم الرسالة: **1,048,576 بايت** (1 MB)

---

## 6. طرق Task — المهام

### الغرض
إدارة دورة حياة المهام في المحرك: إنشاء، إلغاء، واستعلام عن حالة المهام.

### 6.1 task.create — إنشاء مهمة

#### الطلب

```json
{
  "type": "request",
  "id": "task_001",
  "timestamp": "2026-07-10T14:30:02.000Z",
  "payload": {
    "method": "task",
    "action": "create",
    "task_id": "t_abc123",
    "description": "Create a login form component",
    "inputs": {
      "files": ["src/components/"],
      "context": "We need a React login form..."
    }
  }
}
```

#### الرد

```json
{
  "type": "response",
  "id": "task_001",
  "timestamp": "2026-07-10T14:30:02.200Z",
  "payload": {
    "method": "task",
    "action": "create",
    "status": "created",
    "task_id": "t_abc123"
  }
}
```

### 6.2 task.cancel — إلغاء مهمة

#### الطلب

```json
{
  "type": "request",
  "id": "task_cancel_001",
  "timestamp": "2026-07-10T14:35:00.000Z",
  "payload": {
    "method": "task",
    "action": "cancel",
    "task_id": "t_abc123",
    "reason": "Customer changed requirements"
  }
}
```

#### الرد

```json
{
  "type": "response",
  "id": "task_cancel_001",
  "timestamp": "2026-07-10T14:35:00.100Z",
  "payload": {
    "method": "task",
    "action": "cancel",
    "status": "cancelled",
    "task_id": "t_abc123"
  }
}
```

### 6.3 task.status — استعلام حالة مهمة

#### الطلب

```json
{
  "type": "request",
  "id": "task_status_001",
  "timestamp": "2026-07-10T14:30:05.000Z",
  "payload": {
    "method": "task",
    "action": "status",
    "task_id": "t_abc123"
  }
}
```

#### الرد (مهمة موجودة)

```json
{
  "type": "response",
  "id": "task_status_001",
  "timestamp": "2026-07-10T14:30:05.100Z",
  "payload": {
    "method": "task",
    "action": "status",
    "status": "created",
    "task_id": "t_abc123"
  }
}
```

#### الرد (مهمة غير موجودة)

```json
{
  "type": "response",
  "id": "task_status_001",
  "timestamp": "2026-07-10T14:30:05.100Z",
  "payload": {
    "method": "task",
    "action": "status",
    "status": "unknown",
    "task_id": "t_nonexistent"
  }
}
```

### حقول طلبات Task

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `method` | string | ✅ | `"task"` |
| `action` | string | ✅ | أحد: `"create"`, `"cancel"`, `"status"` |
| `task_id` | string | ✅ | معرّف المهمة |
| `description` | string | ✘ | وصف المهمة (لـ `create`) |
| `inputs` | object | ✘ | مدخلات المهمة (لـ `create`) |
| `reason` | string | ✘ | سبب الإلغاء (لـ `cancel`) |

### الحالات الممكنة للمهام

| الحالة | الوصف |
|---|---|
| `created` | المهمة أُنشئت بنجاح |
| `cancelled` | المهمة أُلغيت |
| `unknown` | لا توجد مهمة بهذا المعرّف |

### قواعد المهام

1. يجب إرسال المصافحة بنجاح قبل أي طلب مهمة
2. `task_id` إلزامي في جميع الإجراءات — بدونه يُرفض الطلب
3. `action` إلزامي — بدونه يُرفض الطلب بخطأ `INVALID_REQUEST`
4. حالة المهام مؤقتة (ephemeral) — تختفي عند إنهاء العملية
5. حد حجم الرسالة: **65,536 بايت** (64 KB)

---

## 7. طرق Approval — الموافقات

### الغرض
إدارة طلبات الموافقة على الإجراءات الحساسة. المحرك يطلب موافقة، والمنصة ترد.

### 7.1 approval.request — طلب موافقة

#### الطلب (من المحرك إلى المنصة)

```json
{
  "type": "request",
  "id": "appr_001",
  "timestamp": "2026-07-10T14:32:00.000Z",
  "payload": {
    "method": "approval.request",
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

#### الرد (من المحرك - موافقة تلقائية للـ stub)

```json
{
  "type": "response",
  "id": "appr_001",
  "timestamp": "2026-07-10T14:32:00.200Z",
  "payload": {
    "method": "approval.response",
    "approved": true,
    "reason": "Stub approval — auto-approved by engine gateway",
    "approved_by": "engine"
  }
}
```

#### الرد (رفض — critical risk)

```json
{
  "type": "response",
  "id": "appr_002",
  "timestamp": "2026-07-10T14:32:00.200Z",
  "payload": {
    "method": "approval.response",
    "approved": false,
    "reason": "Stub denial — critical risk level requires platform review",
    "approved_by": "engine"
  }
}
```

### 7.2 approval.response — رد الموافقة (من المنصة)

#### الطلب (من المنصة إلى المحرك)

```json
{
  "type": "request",
  "id": "appr_resp_001",
  "timestamp": "2026-07-10T14:33:00.000Z",
  "payload": {
    "method": "approval.response",
    "approved": true,
    "reason": "Approved — old tests are obsolete",
    "approved_by": "majed"
  }
}
```

#### الرد (تأكيد الاستلام)

```json
{
  "type": "response",
  "id": "appr_resp_001",
  "timestamp": "2026-07-10T14:33:00.100Z",
  "payload": {
    "method": "approval.response",
    "acknowledged": true
  }
}
```

### حقول طلبات الموافقة

#### approval.request

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `method` | string | ✅ | `"approval.request"` |
| `task_id` | string | ✅ | معرّف المهمة المرتبطة |
| `action_type` | string | ✅ | أحد: `"destructive"`, `"security_sensitive"`, `"dependency_change"` |
| `description` | string | ✘ | وصف الإجراء المطلوب الموافقة عليه |
| `details.risk_level` | string | ✘ | مستوى الخطورة: `"low"`, `"medium"`, `"high"`, `"critical"` |
| `details.affected_files` | string[] | ✘ | الملفات المتأثرة |
| `details.affected_commands` | string[] | ✘ | الأوامر المتأثرة |
| `response_deadline` | string | ✘ | مهلة الرد (ISO 8601) |

#### approval.response (من المنصة)

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `method` | string | ✅ | `"approval.response"` |
| `approved` | boolean | ✅ | `true` للموافقة، `false` للرفض |
| `reason` | string | ✘ | سبب القرار |
| `approved_by` | string | ✘ | من اعتمد القرار |

### قواعد الموافقة

1. **approval.response يجب أن يحمل نفس `id` الخاص بـ `approval.request`** — هذا هو Correlation ID الوحيد
2. `action_type` يجب أن يكون أحد القيم الصالحة، وإلا يُرفض الطلب
3. `risk_level` إذا زُوِّدَ، يجب أن يكون أحد: `low`, `medium`, `high`, `critical`
4. مستوى `critical` يُرفض تلقائياً في الـ stub الحالي
5. يجب إرسال المصافحة بنجاح قبل أي طلب موافقة
6. حد حجم `approval.request`: **16,384 بايت** (16 KB)
7. حد حجم `approval.response`: **4,096 بايت** (4 KB)

---

## 8. رموز الأخطاء (Error Codes)

### 8.1 أخطاء بروتوكولية

| الرمز | الوصف | السبب |
|---|---|---|
| `MALFORMED_JSON` | الرسالة ليست JSON صالح | خطأ في الـ framing أو JSON غير قابل للتحليل |
| `INVALID_REQUEST` | هيكل الطلب غير صحيح | `type` ليس `"request"`، أو `payload` مفقود، أو `method` مفقود |
| `UNSUPPORTED_METHOD` | الطريقة غير مدعومة | `payload.method` ليس ضمن الطرق المدعومة |
| `MESSAGE_TOO_LARGE` | حجم الرسالة تجاوز الحد | تجاوز الحد المسموح لكل طريقة |

### 8.2 أخطاء المصافحة (Handshake)

| الرمز | الوصف | السبب |
|---|---|---|
| `VERSION_MISMATCH` | عدم تطابق إصدار البروتوكول | `contract_version` لا يطابق إصدار المحرك |
| `HANDSHAKE_REQUIRED` | المصافحة مطلوبة أولاً | (ضمني — الأخطاء الأخرى تشير إليه) |

### 8.3 أخطاء السياق (Context)

| الرمز | الوصف | السبب |
|---|---|---|
| `CONTEXT_BEFORE_HANDSHAKE` | طلب سياق قبل المصافحة | لم تُرسل المصافحة بعد |
| `INVALID_WORKSPACE` | عدم تطابق workspace/project | `workspace_id` أو `project_id` لا يطابق المصافحة |
| `INVALID_CAPABILITY_ENVELOPE` | غلاف الصلاحيات غير صالح | هيكل `capabilities` غير مكتمل أو خاطئ |

### 8.4 أخطاء المهام (Task)

| الرمز | الوصف | السبب |
|---|---|---|
| `TASK_BEFORE_HANDSHAKE` | طلب مهمة قبل المصافحة | لم تُرسل المصافحة بعد |
| `INVALID_ACTION` | إجراء مهمة غير معروف | `action` ليس `"create"` أو `"cancel"` أو `"status"` |
| `INVALID_REQUEST` | طلب مهمة غير صحيح | `task_id` أو `action` مفقود |

### 8.5 أخطاء الموافقة (Approval)

| الرمز | الوصف | السبب |
|---|---|---|
| `APPROVAL_BEFORE_HANDSHAKE` | طلب موافقة قبل المصافحة | لم تُرسل المصافحة بعد |
| `INVALID_REQUEST` | طلب موافقة غير صحيح | `task_id` مفقود، أو `action_type` غير صالح، أو `risk_level` غير صالح |

### 8.6 هيكل رسالة الخطأ

```json
{
  "type": "error",
  "id": "err_001",
  "timestamp": "2026-07-10T14:35:00.000Z",
  "payload": {
    "method": "task",
    "error_type": "protocol_error",
    "error_code": "INVALID_REQUEST",
    "message": "Task request requires an action field",
    "fatal": false
  }
}
```

| الحقل | النوع | الوصف |
|---|---|---|
| `type` | string | `"error"` |
| `id` | string | معرّف التتبع (من الطلب الأصلي أو `"unknown"`) |
| `payload.method` | string | الطريقة التي حدث فيها الخطأ |
| `payload.error_type` | string | `"protocol_error"` (حالياً) |
| `payload.error_code` | string | رمز الخطأ من الجدول أعلاه |
| `payload.message` | string | وصف الخطأ |
| `payload.fatal` | boolean | `true` ← يجب إيقاف الجلسة، `false` ← يمكن المتابعة |

---

## 9. الطرق المدعومة حالياً

| الطريقة | الوصف | الحالة |
|---|---|---|
| `context` | إرسال سياق التنفيذ | ✅ active |
| `task` | إدارة المهام (create, cancel, status) | ✅ active |
| `approval` | إدارة الموافقات (request, response) | ✅ active |

المحرك يُعلن عن الطرق المدعومة في رد المصافحة عبر حقل `supported_methods`:

```json
"supported_methods": ["context", "task", "approval"]
```

---

## 10. حدود أحجام الرسائل

| نوع الرسالة | الحد الأقصى (بايت) | الحد الأقصى (مقروء) |
|---|---|---|
| HandshakeRequest | 4,096 | 4 KB |
| HandshakeResponse | 4,096 | 4 KB |
| ContextRequest | 1,048,576 | 1 MB |
| ContextResponse | 4,096 | 4 KB |
| Task (create/cancel/status) | 65,536 | 64 KB |
| ApprovalRequest | 16,384 | 16 KB |
| ApprovalResponse | 4,096 | 4 KB |

### قواعد الحجم

1. **المرسل مسؤول** عن التحقق من حجم الرسالة قبل الإرسال
2. **المستقبل يتحقق** من الحجم عند الاستلام قبل المعالجة
3. إذا تجاوزت الرسالة الحد → المحرك يرد بـ `MESSAGE_TOO_LARGE` عبر stdout كـ JSON Line
4. إذا كان الحجم يهدد الأمان أو الذاكرة → يحق للمستقبل إغلاق الجلسة فوراً
5. للمحتوى الكبير → استخدم **File Reference** بدلاً من الإدراج المباشر

---

## 11. قواعد الارتباط (Correlation Rules)

### المبدأ الأساسي

**الرد يحمل نفس `id` الذي جاء به الطلب.** هذا هو Correlation ID الوحيد في البروتوكول.

```
طلب:  { "id": "ctx_001", ... }
رد:    { "id": "ctx_001", ... }  ← نفس id
```

### القواعد

| القاعدة | الشرح |
|---|---|
| **فريد** | لا يمكن تكرار `id` في نفس الجلسة (مسؤولية المرسل) |
| **ثابت** | بمجرد إرسال `id`، لا يتغير |
| **مرتبط** | الرد يحمل نفس `id` — هذا يربط الطلب بالرد |
| **قابل للتتبع** | يُستخدم لربط الطلب بالرد في السجلات |
| **غير مكرر** | كل طلب جديد يحتاج `id` جديد |

### مثال تتبع

```
المنصة ←→ المحرك
1. ترسل: { "id": "task_001", "payload": { "method": "task", "action": "create", "task_id": "t_100" } }
2. تستقبل: { "id": "task_001", "payload": { "method": "task", "action": "create", "status": "created", "task_id": "t_100" } }
   → المنصة تعرف أن الطلب task_001 اكتمل لأن id تطابق
3. ترسل: { "id": "task_002", "payload": { "method": "task", "action": "status", "task_id": "t_100" } }
4. تستقبل: { "id": "task_002", "payload": { "method": "task", "action": "status", "status": "created", "task_id": "t_100" } }
   → المنصة تعرف أن هذا الرد خاص بالاستعلام task_002
```

### قاعدة الموافقة (Approval Correlation)

**approval.response يجب أن يحمل نفس `id` الخاص بـ `approval.request`.** لا نستخدم `request_id` مستقلاً في Phase 4.

```
طلب approval.request:   { "id": "appr_001", "payload": { "method": "approval.request", ... } }
رد approval.response:    { "id": "appr_001", "payload": { "method": "approval.response", "approved": true } }
                         → نفس id بالضبط
```

---

## 12. تدفق الاتصال الكامل (Interaction Flow)

```
┌─────────────────────────────────────────────────────────────┐
│ 1. Platform يشغّل البوابة كـ child process                   │
│    (bun run --conditions=browser ./src/index.ts gateway)     │
├─────────────────────────────────────────────────────────────┤
│ 2. Platform ←→ Engine: Handshake                            │
│    Platform تُرسل HandshakeRequest                           │
│    Engine يرد بـ HandshakeResponse (ok / error)              │
├─────────────────────────────────────────────────────────────┤
│ 3. Platform ←→ Engine: Context                              │
│    Platform تُرسل ContextRequest (بعد نجاح المصافحة)          │
│    Engine يرد بـ ContextResponse (acknowledged)              │
├─────────────────────────────────────────────────────────────┤
│ 4. Platform ←→ Engine: Task Management                      │
│    Platform تُرسل task.create / task.cancel / task.status    │
│    Engine يرد بحالة المهمة                                   │
├─────────────────────────────────────────────────────────────┤
│ 5. Engine → Platform: Approval Request (عند الحاجة)         │
│    Engine يُرسل approval.request                             │
│    (في الـ stub الحالي، المحرك يقرر تلقائياً)                │
│    Engine يرد بـ approval.response مع approved أو denied     │
├─────────────────────────────────────────────────────────────┤
│ 6. Platform → Engine: Approval Response (عند الحاجة)        │
│    Platform تُرسل approval.response                          │
│    Engine يؤكد الاستلام                                      │
├─────────────────────────────────────────────────────────────┤
│ 7. Platform تُغلق stdin ← Engine ينهي الجلسة                  │
└─────────────────────────────────────────────────────────────┘
```

---

## 13. رسائل الخطأ النموذجية — أمثلة

### طلب قبل المصافحة

```json
{
  "type": "error",
  "id": "ctx_001",
  "timestamp": "2026-07-10T14:30:01.000Z",
  "payload": {
    "method": "context",
    "error_type": "protocol_error",
    "error_code": "CONTEXT_BEFORE_HANDSHAKE",
    "message": "ContextRequest requires a successful handshake first",
    "fatal": false
  }
}
```

### رسالة كبيرة جداً

```json
{
  "type": "error",
  "id": "ctx_001",
  "timestamp": "2026-07-10T14:30:01.000Z",
  "payload": {
    "method": "context",
    "error_type": "protocol_error",
    "error_code": "MESSAGE_TOO_LARGE",
    "message": "ContextRequest exceeds 1048576 bytes",
    "fatal": false
  }
}
```

### طريقة غير مدعومة

```json
{
  "type": "error",
  "id": "unknown",
  "timestamp": "2026-07-10T14:30:01.000Z",
  "payload": {
    "method": "unsupported_method",
    "error_type": "protocol_error",
    "error_code": "UNSUPPORTED_METHOD",
    "message": "Gateway method is not supported: unsupported_method",
    "fatal": false
  }
}
```

---

*نهاية المرجع — الإصدار 1.0 — بروتوكول TeraGateway v1.2*
