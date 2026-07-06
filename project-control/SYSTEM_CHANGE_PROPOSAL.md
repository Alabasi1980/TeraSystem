# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-06-083 — Agent Improvement Suggestions (AIS) Protocol

---

### Title

إنشاء بروتوكول **Agent Improvement Suggestions (AIS)** — نظام اقتراحات تطوير العملاء من واقع العمل

### Request Type

New System Feature / Governance Enhancement — استجابة لتوصية هيئة التدقيق الخارجي + تحليل TeraSystemEvolutionAgent

### Problem

حالياً، المنظومة تملك فقط `AGENT_GAPS_LOG.md` لتسجيل **المشاكل والفجوات** (شيء مكسور أو مفقود). لكن لا يوجد نظام يسمح للعملاء بتسجيل **تحسينات استباقية** مما يلاحظونه أثناء العمل:

- مهارة يفتقدونها تتكرر عبر المشاريع
- نمط متكرر يمكن توثيقه كـ Best Practice
- تحسين في سير العمل يرفع الكفاءة دون أن يكون "خطأ" حالياً
- غموض في التعليمات لم يمنع العمل لكنه أبطأه
- فرصة لتحسين جودة المخرجات على المدى البعيد

### Evidence

- **AGENT_GAPS_LOG.md** يركز على الفجوات الحادة (أخطاء، صلاحيات ناقصة، سياسات متضاربة) — ولا يصلح للتحسينات التراكمية
- **TERA_CONTINUOUS_IMPROVEMENT_POLICY.md** §3 يشجع على ملاحظة فرص التحسين لكن لا يوفر آلية تسجيل منضبطة لها
- **الهيئة الخارجية** اقترحت حلاً متكاملاً بآلية واضحة، قالب، وشروط تسجيل
- **تحليل حارس** أكد أن إضافة Skill/Pattern types + قاعدة فاصلة GAP/AIS تجعل الحل أكمل

### Affected Files

#### إنشاء ملفين جديدين:

1. `tera-system/AIS_PROTOCOL.md` — البروتوكول العام لجميع العملاء
2. `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` — السجل المركزي للاقتراحات (مع قالب + أول إدخال ترحيبي)

#### تعديل ملفات العملاء (إضافة قسم AIS):

3. `.opencode/agents/tera.md` — TeraAgent
4. `.opencode/agents/tera-client-engagement.md` — TCEA
5. `.opencode/agents/tera-system-evolution.md` — حارس (إضافة مسؤولية المعالجة)
6. `.opencode/agents/monitor.md` — Monitor
7. `.opencode/agents/auditor.md` — Auditor
8. `.opencode/agents/design-reviewer.md` — DesignReviewer
9. `.opencode/agents/application-blueprint.md` — ABA
10. `.opencode/agents/tera-software-designer.md` — SoftwareDesigner

#### تعديل ملفات النظام:

11. `tera-system/TeraPolicyMap.md` — إضافة إدخال AIS
12. `tera-system/TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` — إضافة مرجع متبادل

### Proposed Change

#### الجزء 1 — إنشاء `tera-system/AIS_PROTOCOL.md`

بروتوكول مستقل بذاته (وليس ضمن مجلد `governance/` جديد — توفيراً للطبقات) يتضمن:

1. **الغرض والنطاق**
2. **ما يسمح باقتراحه** — قائمة الأنواع (بما فيها Skill Gap و Pattern Discovery من تحليل حارس)
3. **ما يمنع اقتراحه** — قاعدة Anti-Bloat للاقتراحات
4. **شروط التسجيل** — الشرط الـ 6 (Repeated Friction, Blocking Ambiguity, Quality Risk, Scope Risk, Missing Rule, Conflict, Client Confusion)
5. **قاعدة Anti-Spam** — max 3 per task/session
6. **قاعدة AIS ≠ GAP** — قاعدة فاصلة واضحة:
   ```
   GAP = يمنع التنفيذ الصحيح أو ينتج مخرجات خاطئة ← AGENT_GAPS_LOG.md
   AIS = التنفيذ صحيح لكن يمكن أن يكون أفضل ← AGENT_IMPROVEMENT_SUGGESTIONS.md
   ```
7. **القالب الرسمي** (عربي + إنجليزي)
8. **Status Lifecycle**: Proposed → Under Review → Approved for SCP → Rejected → Deferred → Implemented → Verified
9. **دورة المعالجة**: عميل يُسجل → Majed يراجع أولياً → حارس يحلل → SCP → تنفيذ

#### الجزء 2 — إنشاء `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md`

سجل مركزي مع:
- قواعد التسجيل
- القالب الرسمي
- إدخال ترحيبي يشرح النظام

#### الجزء 3 — إضافة قسم AIS في كل ملف عميل (فقرة واحدة ثابتة)

نص موحد (مع تكييف بسيط حسب دور العميل) يضاف في نهاية كل ملف عميل:

```
## Self-Improvement Suggestions (AIS)

This agent may propose improvements to its own operating instructions
or related system files when it detects repeated friction, ambiguity,
missing rules, workflow weakness, or quality risks.

Rules:
- The agent must NOT modify itself or any governance file.
- The agent must record structured suggestions only in:
  `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md`
- Each suggestion must include: observation, evidence, impact,
  proposed improvement, suggested target file, severity, and task ID.
- Maximum 3 suggestions per task/session unless a critical conflict is found.
- Cosmetic wording changes are not allowed.

The suggestion is NOT active. It requires review by Majed
and formal implementation through TeraSystemEvolutionAgent (Hares).
```

#### الجزء 4 — تعديل ملفي النظام

- **TeraPolicyMap.md**: إضافة إدخال:
  ```
  | Agent improvement suggestions | `tera-system/AIS_PROTOCOL.md` | `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` | Protocol + central log for agent self-improvement suggestions |
  ```

- **TERA_CONTINUOUS_IMPROVEMENT_POLICY.md**: إضافة مرجع إلى AIS في §6 (العلاقة مع بقية المنظومة)

#### الجزء 5 — تعديل تعريف حارس (tera-system-evolution.md)

إضافة مسؤولية صريحة بأن حارس هو المسؤول عن:
- مراجعة دورية لـ AGENT_IMPROVEMENT_SUGGESTIONS.md
- تحليل وتصنيف الاقتراحات
- تحويل المعتمد منها إلى SYSTEM_CHANGE_PROPOSAL
- رفض أو تأجيل الضعيف منها

### Why This Is Necessary

1. **سد فجوة نظامية**: العملاء يشاهدون أنماطاً يومياً — بدون نظام، تضيع هذه المعرفة.
2. **استباقي لا تفاعلي**: GAPS تعالج ما انكسر. AIS يمنع الكسر قبل حدوثه.
3. **تكامل مع الموجود**: لا يكرر GAPS_LOG ولا يتعارض معه — بل يكمله.
4. **توصية الهيئة**: نظام مقترح من مدقق خارجي محايد — اعتماده يرفع نضج المنظومة.
5. **تطوير تراكمي**: كل مشروع يغذي خبرة العملاء للمشروع التالي.

### Rejected Alternatives

1. **دمج AIS في AGENT_GAPS_LOG.md** — مرفوض: يخلط بين الخلل والتحسين، ويجعل السجل غير قابل للتصفية.
2. **السماح للعملاء بتعديل أنفسهم مباشرة** — مرفوض: يكسر الحوكمة.
3. **إنشاء مجلد governance/** — مرفوض (مؤقتاً): طبقة إضافية بدون ضرورة حالية. البروتوكول في `tera-system/` كافٍ.
4. **عدم وجود Anti-Spam** — مرفوض: يتحول إلى ضوضاء.

### Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | لا يوجد نظام للتحسينات الاستباقية — اقتراحات العملاء تضيع |
| لماذا لا يكفي تعديل ملف موجود؟ | الحل أكبر من تعديل — يحتاج ملف بروتوكول + سجل + تحديث 10 عملاء |
| لماذا لا يكفي عميل موجود؟ | النظام يشمل جميع العملاء — يحتاج بروتوكولاً مركزياً |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد التنظيمي** — تقنن ما كان يضيع كملاحظات متناثرة |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | **هامشي جداً** — فقرة واحدة في نهاية كل عميل + ملفان جديدان صغيران |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | تم تصغير الحل: لا مجلد جديد، قالب موحد، نص موحد للعملاء |

### Risk

- **منخفض** — البروتوكول لا يغير صلاحيات ولا سياسات قائمة. الاقتراحات غير نافذة حتى تمر بـ SCP + موافقة.
- **خطر الضوضاء** — يعالجه Anti-Spam + شرط Evidence + مراجعة Majed + رفض حارس للضعيف.
- **خطر التضخم في ملفات العملاء** — فقرة واحدة بنص موحد، لا تضخم.

### Rollback Plan

1. حذف `tera-system/AIS_PROTOCOL.md`
2. حذف `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md`
3. إزالة قسم AIS من ملفات العملاء العشرة
4. إزالة إدخال TeraPolicyMap.md
5. إزالة المرجع من TERA_CONTINUOUS_IMPROVEMENT_POLICY.md

### Approval Required

- ✅ Majed

---

Prepared by: TeraSystemEvolutionAgent (حارس)
Date: 2026-07-06
