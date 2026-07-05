# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-04-026 — Compliance Record لكل TASK-ID

**Request Type:** Agent Process Improvement / Policy Update
**Source:** GAP-003 (TeraAgent) — Pending → Under Review → Approved by Majed (principle + detail level confirmed)

---

## Problem

حالياً بعد تنفيذ TASK-ID، العملية كالتالي:
1. Handback يُسجل في ملف TASK-ID (ما يقوله TeraAgent إنه فعله)
2. Git diff يظهر ما تغير فعلاً في الكود
3. **لا يوجد Compliance Record** يربط هذين المصدرين بالقواعد (Pre-Execution Gate PASS، Allowed Write Targets، Secrets، إلخ)

هذا يخلق فجوات:
- **Monitor** لا يملك مرجعاً موحداً للتدقيق — يضطر لمقارنة Handback مع Git diff يدوياً
- **Auditor** لا يملك سجل امتثال ليحكم على الالتزام بالقواعد
- **TeraAgent** نفسه قد يتجاوز خطوة توثيق دون أن يُكتشف

---

## Evidence

- GAP-003 في `project-control/AGENT_GAPS_LOG.md`
- التجربة العملية مع Alfares أظهرت أن Monitor لم يتمكن من التحقق من المطابقة بين الـ Handback والتغييرات الفعلية
- الـ Post-Execution Review الحالي يركز على جودة التنفيذ لكنه لا يوفر "بصمة امتثال" موجز لمقارنة Handback + Git diff

---

## Affected Files

| الملف | نوع التغيير |
|---|---|
| `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | UPDATE — إضافة قاعدة Compliance Record الإلزامية |
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | UPDATE — إضافة قالب Compliance Record كفقرة جديدة في §32 أو §33 |
| `.opencode/agents/tera.md` | UPDATE — §12: تحديث قاعدة الإغلاق لتشمل Compliance Record |
| `.opencode/agents/monitor.md` | UPDATE — إضافة مسؤولية التحقق من Compliance Record ومطابقة Handback مع Git diff |
| `project-control/AGENT_GAPS_LOG.md` | UPDATE — GAP-003 → Applied |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | UPDATE — تسجيل التغيير |

---

## Proposed Change

### 1. TERA_RUNTIME_PROTOCOLS.md — قاعدة Compliance Record

بعد الـ "After execution" steps الحالية (§4 steps 1–9)، أضف القاعدة التالية:

```text
### Compliance Record (Mandatory)

After completing the Post-Execution Review Gate and before any task may become Accepted or Closed,
Tera must add a Compliance Record section to the task file (`project-control/tasks/TASK-COD-XXX.md`)
as a governance summary that ties together gates, outputs, and handback.

No task may become Accepted or Closed without:
1. Handback recorded in TASK-ID file.
2. Compliance Record (all applicable gates checked + commands documented).
3. Git diff matches Handback content (verified by Monitor when active; otherwise self-verified by Tera).
```

### 2. TERA_RUNTIME_TEMPLATES.md — قالب Compliance Record

أضف قالباً جديداً كقسم §33 (أو بعده) أو دمجه مع §32. الأفضل: إضافته كفقرة منفصلة بعد §32 مباشرة:

```markdown
## 33. Compliance Record (Task Closure Governance Summary)

هذا القسم يُضاف إلى `project-control/tasks/TASK-COD-XXX.md` كآخر قسم قبل الإغلاق.
وهو المرجع المعتمد لـ Monitor للتحقق من مطابقة Handback + Git diff + القواعد.

```markdown
## Compliance Record

| # | Check | Result | Verified By |
|---|---|---|---|
| 1 | Pre-Execution Gate: PASS documented in task file | PASS / N/A | Tera |
| 2 | Allowed Write Targets respected | PASS / FAIL | Tera |
| 3 | No secrets/tokens/passwords in outputs or logs | PASS / FAIL | Tera |
| 4 | Design Source Decision documented (if UI exists) | PASS / N/A | Tera |
| 5 | Post-Execution Review: PASS | PASS / FAIL | Tera |
| 6 | PROJECT_ACTIVITY_LOG.md updated | PASS / FAIL | Tera |
| 7 | Handback recorded in TASK-ID file | PASS / FAIL | Tera |
| 8 | Git diff matches Handback description | PASS / FAIL / PENDING | Monitor* |
| 9 | CLI/commands documented (if any) | Done / N/A | Tera |

*Item 8: Monitor يتحقق عند نشاطه. إذا لم يكن Monitor نشطاً، يوثق Tera الفحص الذاتي.

Compliance Status: COMPLIANT / NON-COMPLIANT / PENDING_MONITOR_REVIEW
```
```

### 3. tera.md §12 — تحديث قاعدة الإغلاق

تحديث السطر الموجود من:

```
No task may become Accepted/Closed before `Post-Execution Review Gate: PASS`. Results recorded in task/control files, not only in chat.
```

إلى:

```
No task may become Accepted/Closed before:
1. Post-Execution Review Gate: PASS
2. Compliance Record: COMPLIANT (all applicable checks passed)
3. Handback recorded in task file
Results recorded in task/control files, not only in chat.
```

### 4. monitor.md — إضافة مسؤولية Compliance Record

بعد "What you do" list الحالي، أضف:

```
- Verify Compliance Record completeness in task files before reporting.
- Cross-check Handback vs Git diff for each closed task using Compliance Record item 8.
- If Compliance Record is missing or NON-COMPLIANT, flag it as a deviation.
```

---

## Why This Is Necessary

1. **Monitor يحتاج مرجعاً موضوعياً** — حالياً ليس لديه checklist واضح للمطابقة بين Handback و Git diff
2. **Auditor يحتاج سجل امتثال** — لا يوجد توثيق رسمي أن TeraAgent التزم بكل البوابات قبل إغلاق المهمة
3. **TeraAgent يحتاج حماية من الانحراف التدريجي** — مع طول الجلسات، بدون checklist إلزامي، يمكن تخطي خطوة دون كشف
4. **الشفافية** — وجود Compliance Record يجعل حالة الامتثال لكل TASK-ID مرئية لـ Majed في أي وقت

---

## Rejected Alternatives

| البديل | سبب الرفض |
|---|---|
| الاعتماد على Post-Execution Review فقط | لا يربط صراحةً Handback مع Git diff، ولا يوفر بصمة موجز لـ Monitor |
| Compliance Record في ملف منفصل | يزيد التضخم — الأفضل ضمن ملف TASK-ID نفسه (anti-bloat) |
| إضافة الـ 9 بنود إلى Post-Execution Review مباشرة | يخلط بين مراجعة الجودة (Post-Execution) وبصمة الامتثال (Compliance) — هما هدفان مختلفان |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|---|---|
| ما المشكلة التي تحلها؟ | غياب سجل امتثال موحد يربط Handback + Git diff + القواعد |
| لماذا لا يكفي تعديل ملف موجود؟ | هذا التغيير يعدّل 4 ملفات موجودة — لا ينشئ ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | المشكلة في العملية وليس في العميل؛ التغيير يضبط العملية |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | تقلل التعقيد لـ Monitor و Auditor بإعطائهم مرجعاً واضحاً. تزيد قليلاً على TeraAgent لكنها توثيق لما يفعله أصلاً |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | ضئيل — Compliance Record ~15 سطراً إضافياً في ملف TASK-ID |
| هل توجد طريقة أصغر؟ | لا — 9 بنود هي الحد الأدنى لتغطية البوابات الإلزامية كلها |

---

## Risk

- **منخفض:** Compliance Record يوثق إجراءات موجودة أصلاً — لا يغير صلاحيات أو أدوار
- **تأقلم TeraAgent:** قد ينسى TeraAgent إضافة Compliance Record في البداية. يكتشفه Monitor أو يظهر عدم اكتمال المهمة

## Rollback Plan

1. إزالة القاعدة المضافة من `TERA_RUNTIME_PROTOCOLS.md`
2. إزالة القالب من `TERA_RUNTIME_TEMPLATES.md`
3. إعادة قاعدة الإغلاق في `tera.md §12` إلى الصيغة السابقة
4. إزالة مسؤوليات Monitor المضافة من `monitor.md`

---

## Approval Required: ✅ Majed

