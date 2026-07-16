# POC_ADAPTER_SCOPE.md

**المشروع:** Tera Control Room
**المرحلة:** 0 — OpenCode Adapter Proof of Concept
**الإصدار:** 1.0
**الحالة:** معتمدة للتنفيذ التجريبي

---

## 1. الهدف من الوثيقة

تحديد نطاق **المرحلة 0 فقط** من مشروع Tera Control Room. هذه الوثيقة لا تُعرّف النظام الكامل — يبقى المقترح الكامل في `TERA_CONTROL_ROOM_PROPOSAL.md` هو المرجع طويل المدى.

المرحلة 0 = إثبات أن الطريق التالي قابل للتنفيذ تقنيًا:

```text
Test Command
→ OpenCode Adapter
→ تشغيل Agent Profile محدد
→ تمرير Task Contract منظم
→ تشغيل العميل داخل Git Worktree معزول
→ تنفيذ مهمة اختبار صغيرة
→ استلام Structured Handback
→ التحقق من Schema
→ قراءة Exit Code
→ جمع Git Diff والأدلة
→ إصدار نتيجة نجاح أو فشل واضحة
```

---

## 2. خارج النطاق صراحةً

| العنصر | الحكم |
|---|---|
| Supervisor Runtime | مؤجل |
| Workflow Engine / State Machine | مؤجل |
| Policy Engine كامل | مؤجل |
| Agent Registry ديناميكي | مؤجل |
| Done Criteria Registry | مؤجل |
| Multi-Agent Orchestration | مؤجل |
| Reviewer / Auditor / Monitor / Design Reviewer | مؤجل |
| Owner Decision Queue | مؤجل |
| CLI كامل (`tera-room start/status/...`) | مؤجل — ننفذ أمر اختبار واحد فقط |
| Web UI | ممنوع في هذه المرحلة |
| تعديل OpenCode أو إنشاء Fork | ممنوع |
| Push / Merge تلقائي | ممنوع |
| أي ميزة من المراحل 1+ | مؤجل |

---

## 3. المكونات المطلوبة في هذه المرحلة فقط

### 3.1 OpenCode Adapter (`source/adapter/`)

**المسؤوليات:**
- تشغيل OpenCode كعملية فرعية (`opencode run`).
- تمرير `--agent <name>` لتحديد البروفايل.
- تمرير `--dir <worktree-path>` لتحديد مجلد العمل.
- تمرير `--format json` لإخراج JSONL أحداث.
- تمرير `--auto` للسماح بالأدوات دون طلب إذن (الاختبار يحتاج وضعًا غير تفاعلي).
- التقاط stdout (JSONL) و stderr كنص.
- التقاط Exit Code.
- تطبيق Timeout عبر قتل العملية فعليًا (`process.kill`) عند تجاوزه.
- استخراج الـ Handback من آخر رسالة نصية في الأحداث.
- إعادة `AdapterResult` منظمة إلى المستدعي.

**ما لا يحتويه الـ Adapter:**
- قواعد Supervisor أو Routing.
- قرارات Policy.
- إدارة جلسات متعددة.
- Resume/Continue منطق.

**القرار التقني:** نهج **CLI subprocess** (الأبسط الكافي لإثبات التكامل). نهج HTTP/SDK هو بديل مرتب للمراحل اللاحقة لكنه خارج نطاق هذه التجربة.

### 3.2 Runner (`source/runner/`)

**المسؤوليات:**
- استقبال Task Contract.
- التحقق الأولي من صحة العقد (`INVALID_TASK_CONTRACT` عند الفشل).
- إنشاء Git Worktree مستقل للمهمة مع فرع جديد.
- تسجيل Base Commit قبل التنفيذ.
- استدعاء Adapter داخل الـWorktree.
- تطبيق طبقة العزل (محاولة Docker ثم fallback موثق).
- جمع الأدلة بعد التنفيذ (Git Status, Git Diff, Head Commit).
- التنظيف (Cleanup) للـWorktree وBranch بعد حفظ الأدلة، أو الاحتفاظ وفق إعداد صريح.

### 3.3 Contracts (`source/contracts/`)

- تعريف TypeScript للـ `TaskContract` و `Handback` و `EvidenceBundle` و `AdapterResult`.
- أنواع نتيجة الحالات (`COMPLETED`, `INVALID_TASK_CONTRACT`, ... إلخ).
- لا تحتوي على منطق تنفيذي — تعريفات فقط.

### 3.4 Schemas (`source/schemas/`)

- ملفات JSON Schema الصريحة:
  - `task-contract.schema.json` (يستخدمها EngineeringAgent للتحقق قبليًا إن أردنا).
  - `handback.schema.json` — **المرجع** لقبول/رفض مخرج العميل.
- استيرادها أو نسخها بجانب كود التحقق.

> ملاحظة توحيد المصطلح: لغة هذه المرحلة تعتمد **HANDBACK** (لا HANDOVER) في كل الملفات والكود والأدلة تماشيًا مع سياسة المستخدم.

### 3.5 Evidence (`source/evidence/`)

- آلية كتابة `EvidenceBundle` JSON لكل تشغيل.
- الموقع الافتراضي: `evidence/POC-<id>/evidence.json` و `evidence/POC-<id>/stdout.log` و `evidence/POC-<id>/stderr.log` و `evidence/POC-<id>/git.diff`.

### 3.6 Tests (`source/tests/`)

- 8 اختبارات إلزامية واضحة (انظر `POC_ACCEPTANCE_CRITERIA.md`).
- مسار خرج اختبار واحد: `bun run source/tests/run-poc.ts` (أو ما يعادله) ينفذ كل الاختبارات وينتج جدول نتائج.

---

## 4. مساحة الاختبار التجريبية

### 4.1 المستودع التجريبي

المسار: `tera-control-room/test-workspace/`

- مستودع Git صغير مستقل (لا تابع لمستودع TeraSystem الرئيسي).
- يحتوي ملف `README.md` و ملف `placeholder.txt` (محتوى ثابت كـ Base Commit).
- يُستخدم لإنشاء Worktree داخله، فلا تؤثر اختباراتنا على المستودع الرئيسي.

### 4.2 مهمة الاختبار الطبيعية

```text
إنشاء الملف: tests/adapter-proof.txt
ويحتوي على قيمة محددة مسبقًا داخل Task Contract.
```

---

## 5. القيود التقنية المسجلة (Discovered Constraints)

سُجلت هذه القيود خلال فحص البيئة:

| القيد | الأثر |
|---|---|
| OpenCode لا يدعم Structured Handback مدمجًا | الحل: تعليم العميل عبر Prompt بإخراج JSON في رسالة نصية بالشكل المتفق عليه |
| OpenCode لا يدعم Timeout مدمج في الوضع غير التفاعلي | الحل: قتل العملية خارجيًا عند تجاوز المدة |
| OpenCode يمنح/يمنع الأدوات على مستوى الـPermission (bash/edit/...) لا مستوى المسار أو الأمر | الحل: تقييد البروفايل عند الحاجة + التحقق من Diff بعد التنفيذ لاكتشاف الانتهاكات |
| Git Worktree يعزل الفروع لا الملفات | الحل: محاولة Docker للعزل الفعلي + اختبار الكتابة خارج النطاق |

أي قيد إضافي يُكتشف أثناء التنفيذ يُسجل في تقرير المرحلة.

---

## 6. توقعات التشغيل

```bash
# من جذر مجلد tera-control-room
bun install           # إن أُنشئ package.json
bun run source/tests/run-poc.ts
```

الناتج المتوقع: تقرير Console يحتوي جدول الاختبارات الثمانية + ملفات أدلة في `evidence/POC-001/` + ملف `poc-report.md`.

---

## 7. عدم تجاوز النطاق

كل ميزة خارج جدول المكونات في القسم 3 تُسجل كـ **Deferred Note** في تقرير المرحلة ولا تُنفذ. أمثلة:

- إعادة المحاولة Retry.
- Logging إلى ملفات مركزية.
- Webhooks / Web UI.
- إدارة Worktrees متعددة في نفس الوقت.
- تتبع التكلفة/التوكن.

تم.