# AGENT_IMPROVEMENT_SUGGESTIONS.md

## Purpose

Central log for **Agent Improvement Suggestions (AIS)** — structured, non-active improvement proposals submitted by core agents based on real work experience.

This is **not** a bug or gap log. For bugs, gaps, and missing capabilities, use `AGENT_GAPS_LOG.md`.

---

## Rules

1. Protocol reference: `tera-system/AIS_PROTOCOL.md`
2. Only core agents listed in §2 of the protocol may submit suggestions
3. Each suggestion must follow the official template
4. Maximum 3 suggestions per task/session (unless critical conflict)
5. Suggestions are NOT active rules — they require review and formal implementation
6. Statuses: Proposed → Under Review → Approved for SCP → Rejected → Deferred → Implemented → Verified

---

## Suggestion Template

```markdown
## AIS-NNNN — [Agent Name] — [Short Title]

**Date:** YYYY-MM-DD
**Agent:** [Agent Name]
**Related Task / Session:** TASK-ID or DRYRUN-ID
**Severity:** Low / Medium / High
**Type:** Ambiguity / Missing Rule / Conflict / Workflow Improvement / Quality Risk / Cost Control / Client Handling / Skill Gap / Pattern Discovery

### Observation
What happened during the work?

### Evidence
Quote the exact instruction, file, output, or situation that caused the issue.

### Impact
What risk does this create if not fixed?

### Proposed Improvement
What should be changed or added?

### Suggested Target File
Which file likely needs update?

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.
```

---

## سجل الاقتراحات

<!-- تبدأ الاقتراحات من هنا -->

## AIS-0005 — TeraClientEngagementAgent — Future-Proof Discovery: توثيق معلومات إضافية قابلة للتوسع

**Date:** 2026-07-06
**Agent:** TeraClientEngagementAgent (مُستشار)
**Related Task / Session:** Discovery لعميل شركة العمران الحديثة للمقاولات
**Severity:** Medium
**Type:** Client Handling / Pattern Discovery

### Observation

أثناء Discovery مع شركة العمران الحديثة، طلب Majed مني توثيق معلومات إضافية عن العميل تتجاوز متطلباته الحالية (الميزانية المحدودة 2000 دينار). الهدف: بناء أساس قابل للتوسع بحيث إذا جاء زبون مشابه بإمكانيات أكبر في المستقبل، نكون قد بنينا نظاماً على أساس صحيح ولا نحتاج إعادة بناء من الصفر.

مثال: الزبون الحالي لا يحتاج تفصيل ساعات العمال لكل عامل (يكفيه إجمالي الفريق)، لكننا لو نسأل عن هيكل العمالة وعددهم وتوزيعهم، هذه المعلومة تفيد في:
- بناء هيكل قاعدة بيانات مرن يتسع لهذا التفصيل مستقبلاً
- إذا جاء زبون آخر بنفس المجال لكنه يريد التفصيل من البداية

### Evidence

طلب Majed المباشر:
> "يجب ان نكون في النهاية حصلنا على كل ما يلزمنا حتى لو كان اكثر من متطلبات العميل لاننا عندما نبني نظام نبنيه قابل للتوسعة وليس محدود ... اي معلومات اضافية ستساعدنا في انشاء اساس صحيح للتطبيق حتى لو لم تنفذ للزبون الحالي ولكنها قد تلزمنا لزبون اخر"

التعليمات الحالية في `tera-client-engagement.md` §A.5.2 و A.6.5 تركز على "لا توسع النطاق دون تأكيد Majed" وهو صحيح كقاعدة حوكمة، لكن لا يوجد منهجية واضحة لـ "اكتشف أوسع من النطاق — وثق كمرجع، لا كالتزام".

### Impact

بدون هذا التحسين:
1. كل عميل جديد يُكتشف ضمن نطاقه الضيق فقط → التطبيق يُبنى على أساس محدود
2. عميل مستقبلي باحتياجات أوسع → نحتاج إعادة بناء أو تعديل جذري
3. فرصة بناء "منصة معرفية" عبر العملاء تفوت — كل عميل نبدأ من الصفر

### Proposed Improvement

إضافة قاعدة جديدة في Discovery Protocol (في `tera-client-engagement.md` أو `protocols.md`) بعنوان **Future-Proof Discovery Rule**:

```
Future-Proof Discovery Rule:
- خلال Discovery، اسأل عن جوانب إضافية تتجاوز النطاق الحالي للعميل إذا كانت:
  (أ) تساعد في بناء أساس قابل للتوسع (قاعدة بيانات، هيكل أدوار، هيكل تكاليف)
  (ب) قد تفيد في مشاريع مستقبلية لنفس العميل أو لغيره
- هذه المعلومات توثق في قسم منفصل تحت اسم "Future-Proof Notes / ملاحظات التوسع المستقبلي"
- لا تدخل في النطاق المعتمد (Approved Scope)
- لا تدخل في التسعير (Quotation)
- تحمل وسم [Future-Proof Reference] — وهو وسم استرشادي فقط
- الهدف: بناء منصة معرفية قابلة لإعادة الاستخدام عبر العملاء
```

### Suggested Target File
`tera-system/client-helpers/tera-client-engagement-protocols.md` — إضافة قسم A.6.9 أو ملحق لـ A.6.x عن Future-Proof Discovery

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.

---

**Status:** Implemented (SCP-2026-07-06-086)
**Verified by:** Majed approval on 2026-07-06

---

## AIS-0001 — System — AIS Protocol Initialization

**Date:** 2026-07-06
**Agent:** TeraSystemEvolutionAgent (حارس)
**Related Task / Session:** SYSTEM — Audit Response
**Severity:** Low
**Type:** Workflow Improvement

### Observation
The external audit body identified that agents have no structured channel to propose self-improvements from real work. The system had only AGENT_GAPS_LOG.md for bugs and gaps.

### Evidence
Audit finding, confirmed by Majed and analyzed by Hares.

### Impact
Without AIS, valuable experiential knowledge from daily work is lost — recurring patterns, skill gaps, and efficiency opportunities go undocumented.

### Proposed Improvement
Implement the full AIS Protocol as defined in `tera-system/AIS_PROTOCOL.md`.

### Suggested Target File
- `tera-system/AIS_PROTOCOL.md`
- `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md`
- All core agent definition files (add AIS section)

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.

---

**Status:** Implemented (SCP-2026-07-06-083)
**Verified by:** Majed approval on 2026-07-06

---

## AIS-0002 — TeraSystemEvolutionAgent — ترقيم مكرر في Sections 16

**Date:** 2026-07-06
**Agent:** TeraSystemEvolutionAgent (حارس)
**Related Task / Session:** Self-review after AIS implementation
**Severity:** Low
**Type:** Conflict

### Observation
يوجد قسمان برقم §16 في نفس الملف: الأول "Self-Improvement Protocol" والثاني "Change Logging". هذا يسبب التباساً عند الإشارة إلى الأقسام.

### Evidence
سطر 479: `## 16. Self-Improvement Protocol`
سطر 487: `## 16. Change Logging`
في ملف `.opencode/agents/tera-system-evolution.md`

### Impact
الإشارات إلى الأقسام قد تكون غير دقيقة. القارئ قد يصل إلى القسم الخطأ.

### Proposed Improvement
إعادة ترقيم الأقسام كالتالي:
- §16 ← Self-Improvement Protocol (يبقى)
- §17 ← Change Logging (كان §16)
- §18 ← Allowed / Forbidden Examples (كان §17)
- §19 ← Final Boundaries (كان §18)

### Suggested Target File
`.opencode/agents/tera-system-evolution.md` — تعديل أرقام الأقسام

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.

---

**Status:** Implemented (SCP-2026-07-06-084)
**Verified by:** Majed approval on 2026-07-06

---

## AIS-0003 — TeraSystemEvolutionAgent — الملفات المرجعية لا تذكر AIS

**Date:** 2026-07-06
**Agent:** TeraSystemEvolutionAgent (حارس)
**Related Task / Session:** Self-review after AIS implementation
**Severity:** Medium
**Type:** Missing Rule

### Observation
§9 (Mandatory Reference Files) يسرد 5 ملفات يجب قراءتها قبل أي مقترح تعديل، لكن لا يذكر `AIS_PROTOCOL.md` أو `AGENT_IMPROVEMENT_SUGGESTIONS.md` رغم أن البروتوكول أصبح جزءاً من المنظومة.

### Evidence
سطور 301-307 في `.opencode/agents/tera-system-evolution.md`:
```text
tera-system/TeraSystemMaintenanceChecklist.md
tera-system/TeraPolicyMap.md
tera-system/TeraArchitectureMap.md
tera-system/TERA_CONTINUOUS_IMPROVEMENT_POLICY.md
project-control/AGENT_GAPS_LOG.md
```
— لا يوجد `AIS_PROTOCOL.md` ولا `AGENT_IMPROVEMENT_SUGGESTIONS.md`

### Impact
عند بدء جلسة تطوير، قد ينسى حارس قراءة AIS_PROTOCOL.md مما يضعف فهمه لدورة معالجة AIS.

### Proposed Improvement
إضافة `tera-system/AIS_PROTOCOL.md` إلى قائمة الملفات المرجعية الإلزامية في §9.

### Suggested Target File
`.opencode/agents/tera-system-evolution.md` §9

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.

---

**Status:** Implemented (SCP-2026-07-06-084)
**Verified by:** Majed approval on 2026-07-06

---

## AIS-0004 — TeraSystemEvolutionAgent — أنواع الطلبات لا تشمل AIS

**Date:** 2026-07-06
**Agent:** TeraSystemEvolutionAgent (حارس)
**Related Task / Session:** Self-review after AIS implementation
**Severity:** Medium
**Type:** Missing Rule

### Observation
§10 (Official Workflow) يسرد 9 أنواع طلبات يمكن أن تبدأ دورة العمل، لكن لا يذكر "AIS suggestion" كنوع مستقل.

### Evidence
سطور 316-325 في `.opencode/agents/tera-system-evolution.md`:
```
1. System bug
2. Agent gap
3. Policy conflict
4. Anti-bloat review
5. Research topic
6. Owner improvement request
7. Client-app-derived system gap
8. Agent self-reported gap
9. Proactive system stewardship finding
```
— لا يوجد "AIS suggestion"

### Impact
دورة معالجة AIS غير مدمجة في الـ Workflow الرسمي. حارس قد يتخطى خطوة معالجة AIS أثناء العمل.

### Proposed Improvement
إضافة "AIS suggestion" كنوع طلب عاشر في §10.

### Suggested Target File
`.opencode/agents/tera-system-evolution.md` §10

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.

---

**Status:** Implemented (SCP-2026-07-06-084)
**Verified by:** Majed approval on 2026-07-06
