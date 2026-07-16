# HANDBACK_SCHEMA_V1.md

**المشروع:** Tera Control Room — Phase 0
**النسخة:** 1.0

---

## 1. الغرض

تعريف Schema منظم للناتج الذي يجب أن يصدره العميل بعد تنفيذ المهمة. هذا الـSchema هو وحده المقبول كـ"Structured Handback".

> **توحيد المصطلح:** نعتمد **HANDBACK** (لا `HANDOVER`) في كل الوثائق والملفات والكود والأدلة تماشيًا مع سياسة المستخدم.

---

## 2. الحقول الإلزامية

```json
{
  "schema_version": "1.0",
  "task_id": "POC-001",
  "agent_id": "engineering-agent",
  "status": "COMPLETED",
  "summary": "Created the requested proof file.",
  "files_changed": ["tests/adapter-proof.txt"],
  "commands_executed": [
    { "command": "git status", "exit_code": 0 }
  ],
  "known_issues": [],
  "recommended_next_action": "REVIEW"
}
```

| الحقل | النوع | مطلوب | القيم المقبولة |
|---|---|---|---|
| `schema_version` | string | ✅ | `"1.0"` فقط |
| `task_id` | string | ✅ | يجب أن يساوي `task_id` في عقد المهمة (Identity Match) |
| `agent_id` | string | ✅ | يجب أن يساوي `agent_id` في عقد المهمة |
| `status` | string | ✅ | واحدة من `COMPLETED`, `FAILED`, `PARTIAL_SUCCESS` |
| `summary` | string | ✅ | نص قصير ≥ 5 أحرف |
| `files_changed` | array<string> | ✅ | قائمة بالمسارات النسبية المعدّلة فعلًا (يمكن أن تكون `[]` لو لم تُعدّل ملفات) |
| `commands_executed` | array<{command, exit_code}> | ✅ | يمكن أن تكون `[]` (لا حقل `command` غير نص أو `exit_code` غير رقم) |
| `known_issues` | array<string> | ✅ | يمكن أن تكون `[]` |
| `recommended_next_action` | string | ✅ | واحدة من `REVIEW`, `RETRY`, `ESCALATE` |

---

## 3. قواعد الرفض

الـHandback يكون **`INVALID_HANDBACK`** عند أي من:

1. JSON غير صالح.
2. `schema_version !== "1.0"`.
3. `task_id` لا يساوي قيمة عقد المهمة.
4. `agent_id` لا يساوي قيمة عقد المهمة.
5. `status` ليست ضمن المجموعة المقبولة.
6. `summary` فارغ أو أقصر من 5 أحرف.
7. `files_changed` ليست مصفوفة، أو فيها قيمة ليست string.
8. `commands_executed` ليست مصفوفة، أو فيها عنصر لا يحوي `command` (string) و `exit_code` (number).
9. `known_issues` ليست مصفوفة من strings.
10. `recommended_next_action` ليس ضمن `REVIEW`, `RETRY`, `ESCALATE`.
11. أي حقل إلزامي مفقود.
12. Handback بلا `files_changed` واضح أو `status: COMPLETED` بلا قائمة ملفات ممكن أن يكون قاعدة واقعية (للمهام التحليلية) لكن لاختبارات المرحلة 0 نطلب أن `files_changed` موجودة حتى لو `[]`.

---

## 4. فك الـHandback من stdout JSONL

### 4.1 شكل الأحداث

عند `opencode run --format json`، تُكتب الأحداث إلى stdout بصيغة JSONL، كل سطر:

```json
{"type":"text","timestamp":1234567890,"sessionID":"...","part":{"type":"text","text":"...","time":{"end":1234567890}}}
```

### 4.2 استراتيجية الاستخراج

1. اجمع كل الأحداث من نوع `text` بصيغة JSON على stdout.
2. اجمع آخر رسالة نصية بصرية للعميل (الحدث الأخير بـ`time.end` محدد).
3. ابحث في `.text` عن أول كتلة ```` ```json ... ``` ```` (fenced JSON block) باستخدام regex: ` ```json\s*\n([\s\S]*?)\n``` `.
4. حلل النتيجة كـJSON.
5. مررها على Schema التحقق أعلاه.
6. عند الفشل على أي خطوة → `INVALID_HANDBACK`.

### 4.3 الحالات الخاصة

- لا أحداث `text` نهائيًا → `INVALID_HANDBACK`.
- آخر event ليس من عميل (مثلاً `error` event) → `INVALID_HANDBACK` ما لم يكن خطأ عملية (فيرجَع `EXECUTION_FAILED` بدلاً منه من طبقة Adapter).
- كتل JSON متعددة → الأولى فقط هي المرشح، وغيرها يُسجل في stdout.log لكن لا يُعتبر handback.

---

## 5. دالة `validateHandback` (واجهة مفترضة)

```text
validateHandback(rawText, expectedTaskId, expectedAgentId): 
  → { valid: true, handback: Handback } 
  | { valid: false, errors: string[] }
```

يُعاد استدعاؤها في Test 8 Right Identity بمختلف قيم `expectedTaskId` و`expectedAgentId` لاختبار مطابقة الهوية.

---

## 6. علاقة الـHandback بالأدلة الفعلية

الـHandback يصف ما يعتقد العميل أنه فعله. الـEvidence هو ما نتحقق منه نحن. كل من:

- `files_changed` في Handback
- `files_changed` من Git Diff الفعلي

يجب أن يتطابقا في المرحلة الإنتاجية. في المرحلة 0 نسجل كلاهما في `EvidenceBundle` ونوثق أي عدم تطابق كـ`Deferred Note` (مرحلة 3 تخصها)، لكن لا نمنع القبول لأنه قد يكون العميل ذكر ملفات لم يعدّلها أو لم يذكر ملفات عدّلها.

تم.