# SYSTEM_CHANGE_PROPOSAL

---

| الحقل | القيمة |
|---|---|
| **Title** | إعادة هيكلة `TeraAgent.md` وتحديث الملفات المرتبطة — إسناد استقبال المشروع و Client Discovery إلى TeraClientEngagementAgent |
| **Request Type** | System Evolution — إعادة هيكلة وتنظيف شامل |
| **Date** | 2026-07-02 |
| **Proposed by** | TeraSystemEvolutionAgent |

---

## Problem

1. **تضخم ملف `TeraAgent.md`** (~915 سطر) — يحاول أن يحتوي هوية العميل + كل بروتوكولات التشغيل + كل التفاصيل التنفيذية.
2. **تداخل مسؤوليات** — TeraAgent يقوم بـ Client Discovery للمشاريع الداخلية، وهو عمل يخص `TeraClientEngagementAgent`. هذا يسبب ازدواجية في المسؤوليات وارتباك.
3. **تكرار بين `TeraAgent.md` و `.opencode/agents/tera.md`** — نفس المحتوى ببنيتين مختلفتين، مما يسبب تناقضات.
4. **غياب حدود واضحة** — TeraAgent ليس لديه نص صريح يمنعه من تطوير المنظومة (System Evolution) أو التعديل على `tera-system/`.
5. **معلومات قديمة** — Phase 1 الحالية (Client Discovery + Smart Interview) لم تعد تعكس التوزيع الصحيح للمسؤوليات.
6. **Slash Commands** موجودة داخل `TeraAgent.md` وهي خاصة بـ OpenCode، وموجودة أصلاً في `.opencode/commands/`. هذا تكرار غير ضروري.

---

## Evidence

1. `tera-system/TeraAgent.md` يحتوي أقساماً كاملة عن Client Discovery (المجموعة 1 في القائمة الشاملة للمهام).
2. `tera-system/TeraClientEngagement.md` (Section 3.2) يحدد Client Discovery كمسؤولية أساسية لـ TCEA، لكن `TeraAgent.md` لا يزال يحتوي نفس المحتوى.
3. `TeraSubAgents.md` (Section 6.0) يوثق أن `ClientDiscoveryAgent` وغيره أُزيلوا ودُمجوا في TCEA، مما يثبت أن الاتجاه الصحيح هو إسناد هذه المهام لـ TCEA.
4. `.opencode/agents/tera.md` يحتوي 987 سطراً، غالبيتها مكررة من `TeraAgent.md` مع اختلافات في البنية.
5. Slash Commands (Section 17 في `TeraAgent.md` و Section 18 في `tera.md`) مكررة مع `.opencode/commands/tera-*.md`.

---

## Affected Files

### Primary (تعديل مباشر)

| الملف | نوع التغيير |
|---|---|
| `tera-system/TeraAgent.md` | **إعادة كتابة كاملة** وفق الهيكل المنقح |
| `.opencode/agents/tera.md` | **إعادة كتابة كاملة** لتتوافق مع الهيكل الجديد لـ TeraAgent.md |

### Secondary (تعديل جزئي)

| الملف | نوع التغيير |
|---|---|
| `tera-system/TeraClientEngagement.md` | تحديث Section 5.1 ليشمل إنشاء مساحة العمل (`clients/CLIENT-*/applications/APP-*/`) قبل تسليم الحزمة لـ TeraAgent |
| `.opencode/agents/tera-client-engagement.md` | تحديث تدفق العمل ليعكس إنشاء مساحة العمل |
| `tera-system/TeraPolicyMap.md` | تحديث الإشارات إلى TeraAgent لتعكس الحدود الجديدة |
| `tera-system/TeraArchitectureMap.md` | تحديث خريطة تدفق المسؤوليات إن لزم |
| `tera-system/TeraSystemMaintenanceChecklist.md` | تحديث إن لزم بعد التغيير |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | تسجيل التغيير (بعد التنفيذ) |

### Read-only review (مراجعة فقط)

| الملف | السبب |
|---|---|
| `tera-system/TeraSubAgents.md` | مراجعة إن كانت الإشارات لـ TeraAgent تحتاج تحديثاً |
| `tera-system/TeraProjectIntakePolicy.md` | مراجعة إن كانت السياسة تشير إلى Client Discovery من TeraAgent |
| `tera-system/TeraApplicationQuestionBank.md` | مراجعة — قد ينتقل مرجعيتها من TeraAgent إلى TCEA |

---

## Proposed Change

### Part A — إعادة كتابة `tera-system/TeraAgent.md`

الهيكل الجديد المقترح:

```
§1. الهوية والحدود
    - من هو TeraAgent
    - مبدأ البدء: لا عمل بدون Handoff معتمد من TCEA
    - ❌ لا Client Discovery، لا زبون مباشر
    - ❌ لا يطور المنظومة — هذا لـ TeraSystemEvolutionAgent
    - ❌ لا يعدّل tera-system/ أو تعريفات العملاء

§2. المبادئ الأساسية والقواعد الذهبية
    - No approved handoff = No project start
    - القرار النهائي للمالك (Majed)
    - فلسفة الأداة المناسبة

§3. مسؤوليات TeraAgent الأساسية (موجزة)

§4. دورة حياة المشروع — المراحل 2→7
    4.1 Phase 2 — Project Decision Formation
    4.2 Phase 3 — Preparation Planning
    4.3 Phase 4 — Sub-Agent Generation & Delegation
    4.4 Phase 5 — Execution Planning
    4.5 Phase 6 — Implementation
    4.6 Phase 7 — Delivery & Closure

§5. إدارة العملاء الفرعيين (تفاصيل)

§6. بوابات الحوكمة (Gates)

§7. حوكمة التصميم (Design Governance)

§8. مكافحة التضخم (Anti-Bloat)

§9. سجلات المشروع وإدارة الحالة (Project Control)

§10. إدارة Technology Profile

§11. Domain Intelligence (بحث وتحليل)

§12. إدارة السياق والتكاليف (Token Management)

§13. إدارة Git والإصدارات

§14. الملفات المرجعية الأساسية
```

**ما يُزال مقارنة بالملف الحالي:**
- أي Reference إلى Client Discovery / Smart Interview كجزء من عمل TeraAgent.
- إنشاء `01_APPLICATION_IDEA.md` و `02_TECHNICAL_CONTEXT.md` من محادثة خام.
- إنتاج `APPLICATION_PROPOSAL.html`.
- استخدام `TeraApplicationQuestionBank.md` كأداة عمل مباشرة.
- Slash Commands (Section 17 في الملف الحالي).
- تكرار غير ضروري مع ملفات أخرى.

**ما يُضاف:**
- حدود واضحة: لا يطور المنظومة، لا يعدّل `tera-system/`.
- إشارة واضحة: كل مشروع يبدأ باستلام `TERA_HANDOFF_PACKAGE.md` أو `INTERNAL_HANDOFF_PACKAGE.md` من TCEA.

### Part B — إعادة كتابة `.opencode/agents/tera.md`

- جعله نسخة OpenCode مختصرة من `TeraAgent.md` الجديد.
- إزالة Slash Commands (موجودة في `.opencode/commands/`).
- تحديث قائمة الملفات المرجعية.

### Part C — تحديث `TeraClientEngagement.md`

- إضافة: TCEA ينشئ مساحة العمل `clients/CLIENT-*/applications/APP-*/` قبل تسليم الحزمة.
- تحديث تدفق العمل في Section 5 ليعكس أن TCEA يسلم مساحة عمل جاهزة + Handoff Package.

---

## Why This Is Necessary

1. **القضاء على تضخم الملفات** — ملف TeraAgent.md سيصبح أقل حجماً وأكثر تركيزاً.
2. **فصل المسؤوليات بشكل نظيف** — TCEA للعميل، TeraAgent للتنفيذ التقني.
3. **إزالة التداخل والتكرار** — لا مهام مكررة بين العميلين.
4. **وضوح الحدود** — TeraAgent يعرف بالضبط أين تنتهي مسؤوليته وأين تبدأ مسؤولية غيره.
5. **سهولة الصيانة مستقبلاً** — ملفات أصغر وأكثر تخصيصاً.

---

## Rejected Alternatives

| البديل | سبب الرفض |
|---|---|
| إبقاء الوضع الحالي | يستمر التضخم والتداخل والارتباك |
| إنشاء ملفات إضافية بدلاً من إعادة الكتابة | يزيد التضخم بدلاً من حله |
| تعديل جزئي فقط | المشكلة هيكلية وليست سطحية — تحتاج إعادة هيكلة كاملة |
| حذف النسخة من `.opencode/agents/tera.md` | هذا هو ملف التشغيل الفعلي لـ OpenCode — لا يمكن حذفه |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|---|---|
| ما المشكلة التي تحلها؟ | تضخم، تداخل، تكرار، نقص حدود واضحة |
| لماذا لا يكفي تعديل ملف موجود؟ | المشكلة هيكلية — إعادة الكتابة هي الحل الوحيد لضمان الاتساق |
| لماذا لا يكفي عميل موجود؟ | المسألة في إعادة تعريف العميل الموجود وليس إنشاء عميل جديد |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — ملف واحد أنظف بدلاً من ملفين متضخمين متكررين |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | **إيجابي** — الملفات الأصغر تعني استهلاكاً أقل للتوكنز |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — إعادة الكتابة هي الحل الأنسب للهيكلة الجديدة |

---

## Risk

| المخاطرة | الاحتمال | التأثير | خطة التخفيف |
|---|---|---|---|
| فقدان معلومات مهمة أثناء إعادة الكتابة | منخفض | مرتفع | مراجعة الملف الأصلي كاملاً قبل الكتابة، والاحتفاظ بنسخة احتياطية |
| عدم اتساق مع ملفات أخرى بعد التعديل | متوسط | متوسط | مراجعة الملفات المرتبطة (Affected Files) وتحديثها معاً |
| خطأ في تعريف حدود TeraAgent الجديدة | منخفض | مرتفع | الاعتماد على القرارات التي اتخذناها معاً أثناء التحليل |
| عدم تطابق `tera.md` مع `TeraAgent.md` بعد التعديل | متوسط | متوسط | كتابة `tera.md` مباشرة بعد `TeraAgent.md` بنفس الجلسة |

---

## Rollback Plan

1. **نسخة احتياطية**: يتم حفظ نسخة من الملفات التالية قبل التعديل:
   - `tera-system/TeraAgent.md` ← `tera-system/_backup_TeraAgent.md`
   - `.opencode/agents/tera.md` ← `.opencode/agents/_backup_tera.md`

2. **إذا ظهر خطأ جسيم بعد التنفيذ**:
   - استعادة الملفات من النسخ الاحتياطية.
   - تسجيل الـ Rollback في `project-control/SYSTEM_EVOLUTION_LOG.md`.
   - تقديم تقرير بالأسباب والحل المقترح البديل.

3. **إذا ظهر خطأ جزئي**:
   - معالجة الجزء المتأثر فقط.
   - لا حاجة لـ Rollback كامل.

---

## Approval Required

| الجهة | الحالة |
|---|---|
| **Majed** | **⏳ Pending** |

---

## Notes

- هذا الـ Proposal هو الأول في حملة تنظيف شاملة لمنظومة Tera.
- بعد الموافقة، سيتم تنفيذ التغييرات بالترتيب التالي:
  1. `tera-system/TeraAgent.md`
  2. `.opencode/agents/tera.md`
  3. `tera-system/TeraClientEngagement.md`
  4. `.opencode/agents/tera-client-engagement.md`
  5. الملفات الثانوية المتبقية
  6. تسجيل التغيير في `project-control/SYSTEM_EVOLUTION_LOG.md`
