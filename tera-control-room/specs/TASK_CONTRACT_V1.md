# TASK_CONTRACT_V1.md

**المشروع:** Tera Control Room — Phase 0
**النسخة:** 1.0

---

## 1. الغرض

تعريف عقد المهمة المنظّم الذي يُمرَّر إلى OpenCode Adapter قبل أي تنفيذ. العقد هو الطريقة الوحيدة لتمرير الهدف والحدود والأدلة إلى العميل في هذه المرحلة.

---

## 2. الحقول الإلزامية

```json
{
  "schema_version": "1.0",
  "task_id": "POC-001",
  "agent_id": "engineering-agent",
  "objective": "Create tests/adapter-proof.txt with the required content.",
  "working_directory": "<task-worktree-path>",
  "allowed_read_paths": ["tests/**"],
  "allowed_write_paths": ["tests/**"],
  "allowed_commands": ["git status", "git diff"],
  "timeout_seconds": 300,
  "expected_handback_schema": "handback-v1"
}
```

| الحقل | النوع | مطلوب | الوصف |
|---|---|---|---|
| `schema_version` | string | ✅ | يجب أن يكون `"1.0"` |
| `task_id` | string | ✅ | معرّف فريد للمهمة، غير فارغ |
| `agent_id` | string | ✅ | اسم البروفايل في `.opencode/agents/` — يجب أن يكون موجودًا وأن يكون primary لا subagent |
| `objective` | string | ✅ | وصف الهدف المباشر للعميل — يتم تمريره في البرومبت |
| `working_directory` | string | ✅ | المسار المطلق للـWorktree |
| `allowed_read_paths` | array<string> | ✅ | أنماط glob مسموحة للقراءة — يستخدمها التحقق اللاحق |
| `allowed_write_paths` | array<string> | ✅ | أنماط glob مسموحة للكتابة — يستخدمها التحقق اللاحق |
| `allowed_commands` | array<string> | ✅ | قائمة الأوامر المصرح بها — تمرير معلوماتيًا في المرحلة 0 (لا تنفيذ حقيقي لحظر الأوامر) |
| `timeout_seconds` | number | ✅ | حد أعلى بالم ثوانٍ |
| `expected_handback_schema` | string | ✅ | يجب أن يكون `"handback-v1"` |

---

## 3. قواعد التحقق الأولي

عقد يفشل أيًّا من الاختبارات التالية يُرفض مباشرة قبل تشغيل العميل بالنتيجة:

```text
INVALID_TASK_CONTRACT
```

وقائمة الأخطاء المكتشفة.

**التحققات الإلزامية:**

1. JSON صالح (إن أمكن تحليله).
2. `schema_version === "1.0"`.
3. `task_id` غير فارغ ولا يحوي مسافات/أحرف غير صالحة لتسمية المجلدات.
4. `agent_id` غير فارغ.
   - التحقق من وجود البروفايل: ينفّذ `opencode agent list` وتُطابق النتيجة. يجب أن يكون `mode === "primary"` (لا subagent).
   - في حالة عدم التمكن من سرد العملاء (مثلاً بيئة без `opencode` مثبت)، يُتاح الإدخال كقيمة صريحة في `task_id` لكن يُسجل القيد.
5. `objective` غير فارغ وطوله ≥ 10 أحرف.
6. `working_directory` مسار مطلق ويحد آخر مكوّن فيه اسمًا صالحًا كاسم فرع.
7. `allowed_read_paths` و `allowed_write_paths` غير فارغين (انظر قاعدة Default Deny في المقترح).
8. `allowed_commands` مصفوفة (يمكن أن تكون فارغة — يعني أنه لا أوامر مصرح بها).
9. `timeout_seconds` عدد > 0 و ≤ 3600.
10. `expected_handback_schema === "handback-v1"`.

---

## 4. الانتقال إلى Prompt

بعد قبول العقد، يُبنى البرومبت الذي يُمرر إلى OpenCode كالتالي (المحتوى داخل الكتل):

```text
You are executing Task ${task_id} as agent ${agent_id}.

OBJECTIVE:
${objective}

CONSTRAINTS:
- You may only write to paths matching: ${allowed_write_paths.join(", ")}.
- You may only read from paths matching: ${allowed_read_paths.join(", ")}.
- Authorized commands: ${allowed_commands.length ? allowed_commands.join("; ") : "(none)"}.
- Timeout: ${timeout_seconds}s.

REQUIRED HANDBACK:
When you finish, output a single fenced JSON block (```json ... ```)
conforming to the handback-v1 schema:
- schema_version: "1.0"
- task_id: must equal "${task_id}"
- agent_id: must equal "${agent_id}"
- status: one of COMPLETED, FAILED, PARTIAL_SUCCESS
- summary: short text
- files_changed: list of relative paths actually modified
- commands_executed: array of { command, exit_code }
- known_issues: array of short notes
- recommended_next_action: REVIEW or RETRY or ESCALATE

Do not output anything after the Handback JSON block. Do not output prose handback.
```

**قاعدة فك الـHandback:** يُبحث stdout JSONL عن آخر رسالة نصية بصرية للعميل، ثم يُستخرج أول كتلة ```json ...``` منها. إن لم توجد كتلة أو الكتلة غير قابلة للتحليل → `INVALID_HANDBACK`.

---

## 5. رفض خارج العقد

أي تعديل يحاول العميل القيام به خارج `allowed_write_paths` يجب اكتشافه عبر فحص Git Diff بعد التنفيذ. الخروج عن النطاق يُسجل كـ **Policy Violation**، ونتيجة الاختبار تكون:

```text
PERMISSION_DENIED
```

بشرط أن نتحقق فعليًا أن الملف المنتهك لم يُكتب (cleanup إذا وُجد).

تم.