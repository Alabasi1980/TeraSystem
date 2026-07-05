# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-04-023

## Title
إضافة بوابة الامتثال (Agent Conduct Gate) كهيكل إجباري لضمان التزام العملاء بالقواعد — بدون تكرار وبدون تضخم

---

## Request Type
System Improvement / Structural Governance Upgrade

---

## Problem

لاحظت — وأنت على حق — أن العملاء لا يلتزمون بشكل صارم بالقواعد رغم كتابتها في ملفاتهم. الأسباب الحقيقية:

1. **القواعد مبعثرة** — جزء في المصدر، جزء في runtime، جزء في ملفات أخرى. العميل لا "يشعر" بوزن القاعدة.
2. **لا يوجد هيكل إجباري يوقف العميل قبل أي فعل** — القواعد مجرد نصوص، ليست خطوات تنفيذية إجبارية.
3. **النموذج مدرب ليكون مفيداً** — فيأخذ المبادرة ويتجاوز القاعدة ظناً منه أنه يساعد.
4. **أقسام Self-Improvement مكررة حالياً عبر 5 عملاء** — نفس المعلومات موجودة في `auditor.md`، `monitor.md`، `design-reviewer.md`، `tera-client-engagement.md`، و `tera-system-evolution.md` مع `TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` كمصدر حقيقة. هذا تضخم قائم.

---

## Evidence

- 5 ملفات عملاء تحتوي على أقسام Self-Improvement/Gap Reporting شبه متطابقة (auditor.md §Self-Improvement Reporting, monitor.md §Self-Improvement Reporting, design-reviewer.md §Self-Improvement Reporting, tera-client-engagement.md §10, tera-system-evolution.md §15).
- `TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` هو المصدر الرسمي ولكن كل عميل يعيد كتابة نفس الآلية.
- لا يوجد ملف واحد يمثل "القواعد الحاسمة" (Immutable Rules) التي تنطبق على كل عميل في المنظومة.
- Pre-Execution Gate الحالي (`TeraPreExecutionGate.md`) هو بوابة قبل تفويض المهمة (Tera-side)، وليس بوابة قبل اتخاذ العميل لأي فعل (Agent-side).

---

## Affected Files

| الملف | نوع التعديل |
|---|---|
| `tera-system/TERA_AGENT_CONDUCT.md` | **CREATE** — مصدر الحقيقة الجديد |
| `tera-system/TeraPolicyMap.md` | **UPDATE** — إضافة مرجع الملف الجديد |
| `tera-system/TeraArchitectureMap.md` | **UPDATE** — إضافة Gate جديدة في الـ Core Flow |
| `.opencode/agents/tera.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى + إزالة قسم Self-Improvement إن وجد |
| `.opencode/agents/tera-client-engagement.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى + إزالة §10 (Self-Improvement) |
| `.opencode/agents/application-blueprint.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى |
| `.opencode/agents/tera-system-evolution.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى + الإبقاء على Agent Gap Management + تنظيف §15 فقط |
| `.opencode/agents/auditor.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى + إزالة §Self-Improvement Reporting |
| `.opencode/agents/monitor.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى + إزالة §Self-Improvement Reporting |
| `.opencode/agents/design-reviewer.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى + إزالة §Self-Improvement Reporting |
| `.opencode/agents/tera-software-designer.md` | **UPDATE** — إضافة 3 أسطر مرجع في الأعلى |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | **UPDATE** — تسجيل التغيير |
| `project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-023.md` | **CREATE** — هذا الملف |

---

## Proposed Change

### الجزء 1: إنشاء مصدر حقيقة واحد — `tera-system/TERA_AGENT_CONDUCT.md`

ملف واحد جديد، نظيف، مختصر (لا يتجاوز ~80 سطراً). يحتوي على 4 أقسام فقط:

**§1 — القواعد الحاسمة (Immutable Rules) — 5 قواعد فقط:**
1. NO action without reading this file first.
2. NO acting outside approved authority (Majed approval for owner-governed agents, Tera task scope for Tera-governed sub-agents).
3. NO skipping mandatory gates.
4. NO creating agents/tools/MCPs without SYSTEM_CHANGE_PROPOSAL.
5. When in doubt: STOP and ask.

**§2 — بوابة ما قبل الفعل (Pre-Action Gate):**
قبل أي خطوة تنفيذية (كتابة ملف، تعديل، أمر Bash، إلخ)، يجب على العميل أن يكتب إقراراً صريحاً بالإجابة على 4 أسئلة:
- Have I read the Immutable Rules in TERA_AGENT_CONDUCT.md?
- Do I have explicit approval for this action? (Yes/No/NA)
- Have I checked the Anti-Bloat Gate? (Yes/NA)
- Am I certain this action is within my allowed scope? (Yes/No)

إذا كان أي جواب `No` أو غير واضح → **STOP**.

**§3 — قاعدة عدم اليقين (Uncertainty Rule):**
If unsure about any action → STOP → state intent → state why unsure → wait for Majed.

**§4 — فحص إتمام المهمة (Task Completion Check):**
بعد كل مهمة، تأكيد قصير: هل اتبعت القواعد الحاسمة؟ هل حصلت على الموافقات اللازمة؟ هل هناك شيء يجب الإبلاغ عنه؟

**§5 — الإبلاغ عن الفجوات (Gap Reporting — مختصر جداً):**
ربط مختصر إلى `TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` و `AGENT_GAPS_LOG.md`. صفحة واحدة، بدون تكرار.

### الجزء 2: تعديل كل ملف عميل — 3 أسطر فقط في الأعلى

بدلاً من إضافة أقسام كاملة، يضاف في بداية كل ملف عميل (بعد الـ YAML front matter مباشرة):

```text
## CONDUCT GATE
Before any action, you MUST read and pass:
`tera-system/TERA_AGENT_CONDUCT.md`
```

**هذا كل شيء. 3 أسطر. لا تكرار للقواعد.**

### الجزء 3: إزالة أقسام Self-Improvement المكررة

- `auditor.md`: إزالة §Self-Improvement Reporting (استبداله بالـ 3 أسطر المرجع)
- `monitor.md`: إزالة §Self-Improvement Reporting (استبداله بالـ 3 أسطر المرجع)
- `design-reviewer.md`: إزالة §Self-Improvement Reporting (استبداله بالـ 3 أسطر المرجع)
- `tera-client-engagement.md`: إزالة §10 (استبداله بالـ 3 أسطر المرجع — مع الإبقاء على المرجع الأصلي لـ `TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` في قائمة القراءة)
- `tera-system-evolution.md`: الإبقاء على §14 (Agent Gap Management) لأنه تشغيلي وخاص بدور TeraSystemEvolutionAgent، مع تنظيف §15 فقط وربطه بالملف المركزي.
- `tera-software-designer.md`: إضافة Conduct Gate في الأعلى لأنه عميل أساسي فعّال ويجب ألا يبقى خارج الضبط الموحد.

---

## Why This Is Necessary

1. **بدون هيكل إجباري، القواعد تبقى نصوصاً وليست قيوداً.** الـ 4 أسئلة في Pre-Action Gate تجبر النموذج على التوقف والتفكير قبل الفعل — وهذا يغير السلوك جوهرياً.
2. **التوحيد يمنع التشتت.** بدلاً من قواعد مبعثرة في 7+ ملفات، مصدر واحد يحدد "هذا هو الدستور الذي يلزمني".
3. **إزالة التكرار الحالي.** أقسام Self-Improvement في 5 عملاء تحتوي نفس المعلومة — هذا تضخم موجود ونقوم بتنظيفه.
4. **الملفات تبقى نظيفة.** 3 أسطر بدلاً من 15-25 سطراً من النص المكرر.

---

## Rejected Alternatives

| البديل | سبب الرفض |
|---|---|
| إضافة Immutable Rules في كل ملف عميل على حدة | تضخم فوري — نفس القواعد تتكرر 7 مرات |
| التعديل فقط على `.opencode/agents/tera.md` والباقي يتبع | العملاء الآخرون (Auditor, Monitor, إلخ) يحتاجون نفس الضبط |
| الاعتماد على Pre-Execution Gate الموجود فقط | موجود على مستوى Tera (تفويض المهام)، وليس على مستوى كل عميل |
| إضافة القواعد لـ `TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` | سياسة التحسين المستمر موضوعها Gap Reporting، ليس Agent Conduct |
| عدم إزالة أقسام Self-Improvement وإضافة المرجع فقط | يبقي التضخم قائماً — 5 نسخ من نفس المعلومة |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|---|---|
| ما المشكلة التي تحلها؟ | عدم التزام العملاء بالقواعد رغم كتابتها |
| لماذا لا يكفي تعديل ملف موجود؟ | لا يوجد ملف موجود يغطي Agent Conduct — كل الملفات الحالية تغطي مواضيع مختلفة |
| لماذا لا يكفي عميل موجود؟ | المشكلة في سلوك كل العملاء، وليس في عميل واحد |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلله** — تزيل 5 أقسام مكررة (~80 سطراً من التكرار) وتضيف 7 × 3 أسطر + ملف واحد جديد (~80 سطراً). صافي التغيير: **تقليل الحجم الإجمالي** |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | إيجابي — لأن referral أقصر من الأقسام المكررة الحالية |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — هذه أقل طريقة ممكنة |

**صافي التأثير على حجم الملفات:**
- ملف واحد جديد: ~80 سطراً
- 7 عملاء × 3 أسطر = 21 سطراً مضافاً
- إزالة ~80 سطراً مكرراً من أقسام Self-Improvement
- **صافي: ~21 سطراً إضافياً فقط** (مع تنظيف التكرار)

---

## Risk

| المخاطرة | المستوى | التعامل |
|---|---|---|
| الـ Pre-Action Gate قد يبطئ العمل الروتيني البسيط | Low | الـ 4 أسئلة سريعة — ويمكن للعميل أن يجيب باختصار |
| نسيان قراءة الملف الجديد | Low | الـ 3 أسطر في بداية كل عميل تذكره بقراءته — والـ Pre-Action Gate تفرض التأكيد |
| إزالة Self-Improvement قد يفقد تخصيصاً لكل عميل | None | `TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` يبقى المرجع الشامل، والملف الجديد §5 يربط به |

---

## Rollback Plan

1. حذف `tera-system/TERA_AGENT_CONDUCT.md`
2. إزالة الـ 3 أسطر المرجع من كل ملفات `.opencode/agents/`
3. إعادة أقسام Self-Improvement الأصلية إلى كل عميل (من Git أو من النسخ الاحتياطية)
4. إزالة المرجع من `TeraPolicyMap.md` و `TeraArchitectureMap.md`
5. إزالة الإدخال من `SYSTEM_EVOLUTION_LOG.md`

---

## Approval Required
✅ **Majed**

---

## الخلاصة للمراجعة السريعة

```
بعد قراءة كل ملفات المنظومة والـ 7 عملاء، الحل الأفضل هو:

1. ملف مركزي واحد: TERA_AGENT_CONDUCT.md ← يحتوي الدستور الإجباري
2. 3 أسطر فقط في كل عميل ← تحيل إلى الملف المركزي
3. إزالة 5 أقسام Self-Improvement مكررة ← تنظيف التضخم الموجود

النتيجة: هيكل إجباري + لا تكرار + ملفات أنظف + تحكم أقوى.
```
