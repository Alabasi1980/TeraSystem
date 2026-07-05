# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-04-034

## Title
**تمكين Monitor (رقيب) من صلاحية Bash/Git للتدقيق — مقارنة Handback vs Git Diff**

## Request Type
Permission Upgrade / Agent Capability Improvement

## Source
تقرير Monitor (رقيب) الذاتي — تحليل وتوصية من TeraSystemEvolutionAgent (حارس)

---

## Problem

Monitor (رقيب) مطلوب منه في تعريفه الحالي (سطر 62 في `monitor.md`):

> **"Cross-check Handback vs Git diff for each closed task using Compliance Record item 8. If they do not match, flag the discrepancy."**

لكن صلاحياته الحالية تمنعه من تنفيذ هذا البند:

| المطلوب | الموجود | المشكلة |
|---------|---------|---------|
| تشغيل `git diff --name-only` | `bash: deny` | 🚨 لا يستطيع تشغيل أي أمر شل |
| تشغيل `git log --oneline` | `bash: deny` | 🚨 لا يستطيع قراءة تاريخ git |
| مقارنة Handback vs التنفيذ الفعلي | قراءة ملفات فقط | 🚨 غير كافٍ — يحتاج git diff |

---

## Evidence

- `.opencode/agents/monitor.md` سطر 10: `bash: deny`
- `.opencode/agents/monitor.md` سطر 62: `Cross-check Handback vs Git diff` — غير قابل للتنفيذ حالياً
- `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md`: ينص على أن Monitor يتحقق من Git diff مقابل Handback
- `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` §33: البند 8 من Compliance Record: `Git diff matches Handback description — (يتحقق منه Monitor)`

---

## Affected Files

1. `.opencode/agents/monitor.md` (Runtime — `bash: deny` → `bash: ask` + تحديث description + إضافة انضباط ذاتي)
2. `project-control/AGENT_GAPS_LOG.md` (إضافة GAP-009)
3. `project-control/SYSTEM_EVOLUTION_LOG.md` (تسجيل SCP-034)

**لا يوجد ملفات جديدة** — Anti-Bloat محفوظ.

---

## Proposed Change

### 1. ترقية صلاحية Bash في `.opencode/agents/monitor.md`

```yaml
# Frontmatter
description: >-
  Independent plan compliance monitor for checking execution against approved
  master and detailed plans. Verifies task completion via git diff cross-check,
  compliance records, and engineering governance drift detection.

permission:
  bash: ask   # ← من deny إلى ask — لأوامر git diff/git/log للتدقيق
```

### 2. إضافة انضباط ذاتي — "Git Audit Protocol"

إضافة قسم جديد في `monitor.md` بعد §Output format وقبل نهاية الملف:

```
## Git Audit Protocol

When performing Cross-check Handback vs Git diff (required per §What you do):

1. **Request bash access**: Tell Majed which git command you need and why.
2. **Standard commands** (read-only, for audit only):
   - `git diff --name-only HEAD~1` — قائمة الملفات المغيَّرة في آخر commit
   - `git diff HEAD~1 -- [file]` — التغييرات في ملف محدد
   - `git log --oneline -10` — آخر 10 commits
   - `git show --stat HEAD` — إحصائيات آخر commit
3. **Never modify**: These commands are read-only. Do not request write operations.
4. **Document**: Record the git diff results in your report.

**Discipline note**: The permission `bash: ask` is for git read-only audit commands only.
Any non-git or write-related bash command requires explicit justification.
```

### 3. تحديث وصف Frontmatter

توسيع الـ description ليعكس قدرات التدقيق الجديدة.

---

## Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| `bash: allow` (بدون سؤال) | مخاطرة عالية — Monitor لا يحتاج تنفيذ أو كتابة، فقط قراءة git |
| إضافة Git MCP | تضخم — git commands تعمل عبر bash بدون MCP إضافي |
| إنشاء Skill (Compliance Auditor) | غير مبرر — Monitor يقوم بالتدقيق بنفسه عبر قراءة الملفات + bash |
| تكامل CI/CD | تضخم مبكر — لا يوجد Production أو pipelines في هذه المرحلة |
| إنشاء أداة traceability تلقائية | غير مبرر — grep/glob كافيان لتتبع TASK-IDs في الكود |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | Monitor لا يستطيع تنفيذ بند أساسي من تعريفه (مقارنة Handback vs Git diff) |
| لماذا لا يكفي تعديل ملف موجود؟ | ✔ — التعديل في ملف موجود فقط (`monitor.md`) |
| لماذا لا يكفي عميل موجود؟ | ✔ — Monitor هو العميل المسؤول عن هذه المهمة |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلّله** — تمكن Monitor من أداء وظيفته المطلوبة دون حلول بديلة |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | لا — git commands قصيرة ومحدودة |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — `bash: deny` → `ask` هو أصغر تغيير ممكن |

---

## Risk

| المخاطرة | مستواها | التخفيف |
|-----------|---------|---------|
| Monitor يطلب bash لأغراض غير git | 🟢 منخفض | الانضباط الذاتي + `ask` = أنت توافق على كل أمر |
| Monitor ينفذ أوامر git modifiying (commit, push) | 🟢 منخفض جداً | ممنوع صراحة في البروتوكول + أنت توافق على كل أمر |
| تضخم في طلبات bash (كل مراجعة تطلب 3-4 أوامر) | 🟢 منخفض | الأوامر سريعة وخفيفة — `git diff --name-only` يأخذ <1 ثانية |

---

## Rollback Plan

1. `.opencode/agents/monitor.md`: إعادة `bash: ask` → `bash: deny`، حذف §Git Audit Protocol.
2. `project-control/AGENT_GAPS_LOG.md`: حذف GAP-009.
3. `project-control/SYSTEM_EVOLUTION_LOG.md`: حذف إدخال SCP-034.

---

## Approval Required

Majed — موافقة على SCP-034:
- [ ] Approval: `bash: deny` → `bash: ask` في monitor.md
- [ ] Approval: إضافة Git Audit Protocol
- [ ] Approval: تسجيل GAP-009
