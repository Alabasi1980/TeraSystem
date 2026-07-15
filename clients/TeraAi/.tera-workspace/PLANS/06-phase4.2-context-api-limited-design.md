# Phase 4.2 — Context API Limited Design

## الملف: 06-phase4.2-context-api-limited-design.md
## المسار: .tera-workspace/PLANS/
## الإصدار: 1.0
## التاريخ: 2026-07-10
## الحالة: 🔵 Design Scope — لا تنفيذ برمجي بعد
## مرجع: TERA_GATEWAY_PROTOCOL_SPEC.md v1.2

---

# 1. الهدف

تصميم أول واجهة محدودة من Engine Gateway: **Context API**.

الغرض هو استبدال القراءة المباشرة من `.tera-workspace/` تدريجيًا بقناة بروتوكول رسمية عبر stdio IPC.

---

# 2. النطاق المحدود

## داخل النطاق

| العنصر | الوصف |
|---|---|
| ContextRequest | رسالة من Platform إلى Engine تحتوي السياق المطلوب |
| ContextResponse | إقرار من Engine بأنه استلم السياق |
| Handshake dependency | لا يقبل ContextRequest قبل Handshake ناجح |
| Capability Envelope | يرسل مع ContextRequest ويُستخدم كحد تنفيذ وقائي |
| Message size | يلتزم بحد 1MB للـ ContextRequest |
| stdout/stderr | stdout للبروتوكول فقط، stderr للـ logs |

## خارج النطاق

```
❌ Task Assignment
❌ ExecutionResult
❌ Approval API
❌ Event Stream
❌ REST API
❌ حذف read_tera_workspace الآن
❌ أي أداة تغير حالة TeraSystem
```

---

# 3. ContextRequest

```json
{
  "type": "request",
  "id": "ctx_20260710_143001_a3b2c1",
  "timestamp": "2026-07-10T14:30:01.000Z",
  "payload": {
    "method": "context",
    "contract_version": "1.2",
    "platform_version": "0.1.0",
    "engine_version": "0.1.0",
    "workspace_id": "ws_abc123",
    "project_id": "proj_xyz789",
    "context_type": "system | project | task",
    "context_data": "markdown-or-json-string",
    "capabilities": {
      "allowed_read_paths": ["src/", "tests/"],
      "allowed_write_paths": ["src/"],
      "forbidden_paths": ["*.env", ".secrets/"],
      "allowed_commands": ["bun test"],
      "forbidden_commands": ["rm -rf"],
      "network_policy": { "internet_access": false, "allowed_domains": [] },
      "approval_required": {
        "destructive_actions": true,
        "security_sensitive": true,
        "major_dependency_changes": true
      }
    }
  }
}
```

---

# 4. ContextResponse

```json
{
  "type": "response",
  "id": "ctx_20260710_143001_a3b2c1",
  "timestamp": "2026-07-10T14:30:01.200Z",
  "payload": {
    "method": "context",
    "status": "ok",
    "acknowledged": true
  }
}
```

---

# 5. أخطاء Context API

| error_code | متى يحدث | السلوك |
|---|---|---|
| CONTEXT_BEFORE_HANDSHAKE | وصل context قبل handshake | رفض الطلب |
| VERSION_MISMATCH | contract_version غير متوافق | رفض الطلب |
| MESSAGE_TOO_LARGE | context_data يتجاوز 1MB | structured error أو إغلاق عند تهديد framing |
| INVALID_WORKSPACE | workspace_id لا يطابق الجلسة | رفض الطلب |
| INVALID_CAPABILITY_ENVELOPE | envelope ناقص أو غير صالح | رفض الطلب |
| MALFORMED_JSON | الرسالة ليست JSON صالح | إغلاق الجلسة عند تهديد framing |

---

# 6. العلاقة مع read_tera_workspace

| المرحلة | السلوك |
|---|---|
| قبل Context API | read_tera_workspace هو المصدر |
| بعد Context API يعمل | Gateway هو المصدر الأساسي |
| عند فشل Gateway | read_tera_workspace fallback مؤقت |
| Phase 4.5 | read_tera_workspace deprecated |
| Phase 5 | read_tera_workspace removed |

**قاعدة:** لا يبقى مساران دائمان للسياق.

---

# 7. Definition of Done — Context API Design

| # | البند |
|---|---|
| 1 | ContextRequest schema واضح |
| 2 | ContextResponse schema واضح |
| 3 | Error cases موثقة |
| 4 | علاقة read_tera_workspace موضحة |
| 5 | حدود الحجم والـ correlation_id متوافقة مع Protocol Spec v1.2 |
| 6 | لا تنفيذ برمجي قبل مراجعة التصميم |

---

*هذه وثيقة تصميم محدودة. التنفيذ يبدأ فقط بعد مراجعتها.*