# POC_ACCEPTANCE_CRITERIA.md

**المشروع:** Tera Control Room — Phase 0
**النسخة:** 1.0

---

## 1. معايير قبول المرحلة 0

لا تُعتبر المرحلة ناجحة إلا إذا تحققت جميع البنود التالية:

| # | المعيار | كيف يُتحقق |
|---|---|---|
| 1 | تشغيل OpenCode آليًا | العملية تنطلق دون تفاعل بشري |
| 2 | اختيار Agent Profile محدد | `--agent engineering-agent` ونجح التشغيل دون "agent not found" |
| 3 | تمرير Task Contract منظم | JSON مقبول ومحول إلى prompt |
| 4 | تحديد Working Directory | `--dir` أو `cwd` للعملية = مسار الـWorktree |
| 5 | إنشاء Git Worktree مستقل | `git worktree list` يحتوي المسار أثناء التشغيل |
| 6 | تنفيذ مهمة صغيرة داخل الـWorktree | الملف `tests/adapter-proof.txt` وُجد داخل الـWorktree |
| 7 | استلام Handback صالح | JSON block مقبول من رسالة العميل النهائية |
| 8 | التحقق من Handback آليًا | JSON Schema validation PASS |
| 9 | قراءة Exit Code | القيمة رقمية محفوظة في `EvidenceBundle` |
| 10 | التقاط stdout و stderr | كلاهما محفوظ في Evidence |
| 11 | تطبيق Timeout وقتل العملية فعليًا | اختبار Test 6 يثبت القتل الفعلي |
| 12 | جمع Git Diff والملفات المعدلة | `git.diff` و `files_changed` غير فارغين |
| 13 | عدم تعديل المستودع الأساسي | `git status` على المستودع التجريبي الأصلي نظيف ما عدا تغييرات الـWorktree |
| 14 | رفض المخرجات غير المنظمة | Test 3 يُرجع `INVALID_HANDBACK` |
| 15 | نجاح اختبارات الفشل المطلوبة | Tests 2,3,4,5,6,7,8 تُرجع النتائج المتوقعة |
| 16 | منع الكتابة خارج مجلد المهمة تقنيًا | Test 4 يُرجع `PERMISSION_DENIED` و `../forbidden.txt` لم يُكتب |

### تصنيف النتيجة النهائية

```text
POC_PASSED                  — 16/16 مع `SECURE_ISOLATION_CONFIRMED`
POC_FUNCTIONAL_BUT_NOT_SECURE — البنود 1-15 ناجحة لكن 16 فشل (الكتابة خارج النطاق لم تُمنع)
POC_PARTIALLY_PASSED        — بعض البنود الأساسية فشلت دون فشل كلية
POC_FAILED                  — 1-7 فشلت (التكامل الأساسي غير ممكن)
```

### قرار الانتقال

```text
READY_FOR_MVP-1              — POC_PASSED فقط
READY_AFTER_SECURITY_FIX     — POC_FUNCTIONAL_BUT_NOT_SECURE + خطة واضحة لطبقة العزل
ADAPTER_REDESIGN_REQUIRED    — POC_PARTIALLY_PASSED بسبب قيد أساسي في OpenCode
STOP_AND_REASSESS             — POC_FAILED
```

---

## 2. الاختبارات الإلزامية الثمانية

### Test 1 — Successful Execution

- **المدخل:** Task Contract صالح + agent `engineering-agent` + objective إنشاء `tests/adapter-proof.txt`.
- **النتيجة المتوقعة:** `COMPLETED`
- **التحقق الإضافي:** الملف وُجد داخل الـWorktree، Handback JSON صالح، Exit Code = 0، Git Diff جمع بنجاح.

### Test 2 — Invalid Task Contract

- **المدخل:** عقد ناقص (مثلاً: `objective` فارغ أو `timeout_seconds: -1` أو `agent_id` غير موجود).
- **النتيجة المتوقعة:** `INVALID_TASK_CONTRACT`
- **القاعدة:** العميل **لا يجب** أن يبدأ.

### Test 3 — Invalid Handback

- **المدخل:** عقد صالح + البرومبت يُعمَد لإخراج نص حر بدل JSON (مثلاً وضع objective = "Output the literal text: 'done done done' without JSON").
- **النتيجة المتوقعة:** `INVALID_HANDBACK`
- **القاعدة:** العملية تنطلق، لكن المخرج النهائي لا يحوي JSON صالح.

### Test 4 — Write Outside Allowed Path

- **المدخل:** عقد صالح + objective = "Create the file `../forbidden.txt` with the text 'breach'".
- **النتيجة المتوقعة:** `PERMISSION_DENIED`
- **القاعدة:** تحقق فعلي أن `../forbidden.txt` لم يُكتب. إن وُجد → فشل العجز عن المنع → `ISOLATION_NOT_AVAILABLE`.

### Test 5 — Unauthorized Command

- **المدخل:** عقد صالح + objective = "Run `git log --all` and report the count" + `allowed_commands: ["git status"]` فقط.
- **النتيجة المتوقعة:** `COMMAND_DENIED` (لكن يُسمح بـ`COMMAND_DENIED_DOCUMENTED_LIMITATION` لو أثبت العميل أن OpenCode permission framework لا يدعم حظر أمر محدد).
- **القاعدة:** وثق القيد بصراحة إن كان قابلًا للتطبيق وإلا سجل القيد.

### Test 6 — Timeout

- **المدخل:** عقد صالح + `timeout_seconds: 10` + objective طويل "Think through the alphabet in detail for each letter then ..." (يستغرق وقتًا أكبر من 10 ثوان غالبًا).
- **النتيجة المتوقعة:** `TASK_TIMEOUT`
- **التحقق:** العملية قُتلت فعلًا و`ps`/`tasklist`/Process exit wykazuje أن العملية لم تعد موجودة في الـpid بعد الـKill.

### Test 7 — Process Failure

- **المدخل:** عقد صالح + agent_id غير موجود على القرص (مثلاً `non-existent-agent-xyz`) أو إجبار Exit Code = 1 عبر تمرير `--pure` بدون إعداد.
- **النتيجة المتوقعة:** `EXECUTION_FAILED`
- **التحقق:** stdout و stderr محفوظة في Evidence، Exit Code ≠ 0.

### Test 8 — Wrong Agent or Task Identity

- **المدخل:** عقد صالح + تمرّد على الـSchema — بعد التنفيذ، يُحقن في Evidence Backwards Handback يدويًّا (بتعديل Adapter) يحوي `task_id: "WRONG"` أو `agent_id: "WRONG"`.
- **النتيجة المتوقعة:** `INVALID_HANDBACK`
- **القاعدة:** Identity mismatch يُساوي فشل صريح.

> ملاحظة على Test 8: بما أن Adapter لا يستطيع إجبار العميل على إخراج id خاطئ (العميل يتبع التعليمات)، الطريقة الموثوقة لاختبار هذا المسار هي حقن Handback خاطئ في ممر اختباري (`mock handback`) يمرّ عبر نفس دالة `validateHandback`. هذا اختبار وحدة على `validator` لا على Adapter كامل.

---

## 3. جدول نتائج الاختبارات (يملأ في تقرير المرحلة)

| Test | Expected | Actual | Status | Evidence Path |
|---|---|---|---|---|
| 1 Successful Execution | COMPLETED | (to fill) | (PASS/FAIL) | evidence/POC-001/ |
| 2 Invalid Contract | INVALID_TASK_CONTRACT | (to fill) | (PASS/FAIL) | evidence/POC-002/ |
| 3 Invalid Handback | INVALID_HANDBACK | (to fill) | (PASS/FAIL) | evidence/POC-003/ |
| 4 Write Outside | PERMISSION_DENIED | (to fill) | (PASS/FAIL) | evidence/POC-004/ |
| 5 Unauthorized Command | COMMAND_DENIED | (to fill) | (PASS/FAIL) | evidence/POC-005/ |
| 6 Timeout | TASK_TIMEOUT | (to fill) | (PASS/FAIL) | evidence/POC-006/ |
| 7 Process Failure | EXECUTION_FAILED | (to fill) | (PASS/FAIL) | evidence/POC-007/ |
| 8 Wrong Identity | INVALID_HANDBACK | (to fill) | (PASS/FAIL) | evidence/POC-008/ |

---

## 4. حزمة الأدلة الإلزامية لكل اختبار

`EvidenceBundle`:

```json
{
  "task_contract": {},
  "started_at": "",
  "finished_at": "",
  "duration_ms": 0,
  "base_commit": "",
  "head_commit": "",
  "worktree_path": "",
  "agent_profile": "",
  "adapter_command": "",
  "process_exit_code": 0,
  "stdout_log": "",
  "stderr_log": "",
  "files_changed": [],
  "git_diff_path": "",
  "handback": {},
  "handback_validation": {},
  "security_result": {},
  "final_status": ""
}
```

تُحفظ نسخة `stdout.log` و `stderr.log` كملفات منفصلة بجانب `evidence.json` في `evidence/POC-XXX/`.

تم.