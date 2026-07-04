# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-04-018

## Title:
إضافة Understanding Confirmation Gate إلزامية داخل TeraClientEngagementAgent قبل إنتاج ملفات النطاق

---

## Request Type:
Process Gap Fix + Runtime Sync + Limited Operational Remediation

---

## Problem:
1. تم اكتشاف أن `TeraClientEngagementAgent` يستطيع الانتقال من الحوار الاستكشافي مباشرة إلى إنتاج `CLIENT_BRIEF.md` و `SCOPE_SUMMARY.md` دون خطوة تأكيد فهم صريحة من Majed.
2. هذا يخلق احتمال بناء مستندات نطاق على فهم غير مؤكد.
3. السلوك الحالي يتعارض مع القواعد الموجودة فعلاً في `TeraClientPolicy.md`, `TeraProjectIntakePolicy.md`, `TeraApplicationQuestionBank.md`, و `TERA_RUNTIME_PROTOCOLS.md`.
4. التطبيق العميل الحالي دخل بالفعل في هذه الحالة، لذلك يلزم إصلاح نظامي + إيقاف تشغيلي آمن على الملفات الحالية.

---

## Evidence:
- `project-control/AGENT_GAPS_LOG.md` — GAP-001 يصف الفجوة بشكل صحيح.
- `tera-system/TeraClientPolicy.md` يعرّف discovery على أنه: استماع، فهم، تأكيد.
- `tera-system/TeraProjectIntakePolicy.md` Stage 1 & 2 يشترطان confirmation قبل التقدم.
- `tera-system/TeraApplicationQuestionBank.md` يربط Smart Interview و formal preparation بتأكيد Discovery summary.
- `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` ينص صراحةً: `No project preparation before documented and confirmed understanding.`
- `tera-system/TeraClientEngagement.md` و `.opencode/agents/tera-client-engagement.md` لا يفرضان هذه الخطوة بشكل تشغيلي واضح قبل ملفات النطاق.

---

## Affected Files:

| نوع التغيير | الملف |
|-------------|-------|
| **تحديث** | `project-control/AGENT_GAPS_LOG.md` |
| **تحديث** | `tera-system/TeraClientEngagement.md` |
| **تحديث** | `.opencode/agents/tera-client-engagement.md` |
| **تحديث تشغيلي محدود** | `clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/CLIENT_INTAKE.md` |
| **تحديث تشغيلي محدود** | `clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/CLIENT_BRIEF.md` |
| **تحديث تشغيلي محدود** | `clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/SCOPE_SUMMARY.md` |
| **تسجيل** | `project-control/SYSTEM_EVOLUTION_LOG.md` |

---

## Proposed Change:

### 1. تحديث `TeraClientEngagement.md`
إضافة قاعدة تشغيل صريحة:
- بعد Discovery الأساسي أو بعد Smart Interview، ينتج TCEA **Understanding Summary**.
- يعرضه على Majed بصيغة تأكيد صريحة.
- لا يجوز إنتاج `CLIENT_BRIEF.md`, `SCOPE_SUMMARY.md`, `DRAFT_QUOTATION.md`, أو `TERA_HANDOFF_PACKAGE.md` قبل confirmation.
- يجب توثيق نتيجة التأكيد داخل `CLIENT_INTAKE.md`.

### 2. تحديث `.opencode/agents/tera-client-engagement.md`
إضافة نفس البوابة بصيغة تشغيلية مختصرة + تحديث `Last Synced`.

### 3. Remediation للتطبيق الحالي
- إضافة قسم/حقل `Understanding Confirmation` داخل `CLIENT_INTAKE.md` للحالة الحالية.
- وضع تنبيه حوكمي واضح داخل `CLIENT_BRIEF.md` و `SCOPE_SUMMARY.md` بأنهما غير قابلين للاعتماد قبل confirmation.
- هذا لا يصحح المحتوى تلقائياً، لكنه يمنع الاستمرار على أساس غير مؤكد.

### 4. تحديث `AGENT_GAPS_LOG.md`
تحويل GAP-001 من `Pending` إلى `Applied` بعد التنفيذ، مع ملاحظات حل واضحة.

---

## Why This Is Necessary:
- الفجوة تمس **أساس الفهم** قبل النطاق، وهي ليست ملاحظة تجميلية.
- أي وثائق لاحقة أو handoff مبنية على فهم غير مؤكد قد تسبب إعادة عمل أو سوء scope.
- النظام يملك القاعدة نظرياً بالفعل، لكن لا يفرضها في TCEA تشغيلياً بالشكل الكافي.
- إصلاحها الآن يمنع تكرارها مع أول عميل وأول رحلة تشغيلية.

---

## Rejected Alternatives:

| البديل | سبب الرفض |
|--------|-----------|
| تجاهل الفجوة ومتابعة العمل | خطر تراكمي على جودة scope والحزمة المسلّمة |
| الاكتفاء بتنبيه شفهي لـ TCEA دون تعديل الملفات | لا يغيّر النظام نفسه، وقد تتكرر الفجوة |
| تعديل `TeraClientPolicy.md` أو `TERA_RUNTIME_PROTOCOLS.md` فقط | القاعدة موجودة هناك فعلاً؛ المشكلة في غياب enforcement داخل TCEA نفسه |
| إعادة كتابة ملفات النطاق الحالية بالكامل فوراً | أوسع من المطلوب حالياً؛ يكفي إيقافها وتوسيمها لحين confirmation |

---

## Anti-Bloat Check:

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | منع إنتاج scope على فهم غير مؤكد |
| لماذا لا يكفي تعديل ملف موجود؟ | سنعدل فقط الملفات التشغيلية/المرجعية التي ينشأ فيها الخلل فعلاً |
| لماذا لا يكفي عميل موجود؟ | لا نضيف عميلًا؛ نصحح سلوك TCEA نفسه |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | تقلله — تضيف Gate صغيرة تمنع إعادة العمل لاحقاً |
| هل يوجد أثر سلبي على التوكنز؟ | ضئيل جداً — خطوة summary + confirmation قصيرة |
| هل توجد طريقة أصغر؟ | نعم، ولذلك اقتصر التعديل على TCEA + remediation تشغيلية محدودة فقط |

---

## Risk:
- **منخفض إلى متوسط**: قد يبطئ خطوة مبكرة قصيرة، لكنه يمنع أخطاء scope أكبر لاحقاً.
- **مخاطرة تشغيلية حالية**: الوثائق الحالية ستبقى غير قابلة للاعتماد حتى يتم confirmation.

---

## Rollback Plan:
- إزالة تعديلات `TeraClientEngagement.md` و `.opencode/agents/tera-client-engagement.md`.
- إزالة التنبيهات/القسم المضاف من ملفات التطبيق الحالية.
- إعادة حالة GAP-001 إذا لزم.

---

## Approval Required:
Majed — Approved via conversation on 2026-07-04 with explicit instruction to stop work and fix the gap first.
