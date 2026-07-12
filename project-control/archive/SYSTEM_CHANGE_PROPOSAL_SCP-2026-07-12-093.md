# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-12-093

## Title
**إصلاح فجوة GAP-014: إلزام المستشار الاستراتيجي بقراءة الملفات المصدر قبل التوصيات متوسطة/عالية الأثر**

## Request Type
Agent Capability Improvement / Process Gap Fix

## Source
GAP-014 — TeraStrategicAdvisor (اعتراف ذاتي) + تحليل TeraSystemEvolutionAgent (حارس)

---

## Problem

المستشار الاستراتيجي أصدر توصيات حاسمة أثناء مشروع WarehouseDashboard (اعتماد Blueprint، توازي Batches، الموافقة على Build Mode) دون قراءة الملفات المصدر الفعلية. اعتمد بدلاً من ذلك على ملخصات Majed وتقارير المُدقق.

**السبب الجذري:** §8 (Impact Classification Gate) في `.opencode/agents/tera-strategic-advisor.md` ينص على:

| المستوى | السلوك المطلوب |
|:------:|---------------|
| Medium impact | `inspect relevant context if available` |
| High impact | `do not issue a final recommendation before checking relevant evidence` |

- **High impact** واضح وصارم ✅
- **Medium impact** فضفاض — `inspect relevant context if available` يسمح بتفسير "السياق" على أنه ملخصات وتقارير بدلاً من المصادر الأولية
- الـ `if available` تجعل الشرط اختيارياً وليس إلزامياً

---

## Evidence

- `AGENT_GAPS_LOG.md` GAP-014 — اعتراف المستشار بالفجوة
- المقارنة بين §8 (Medium) و §8 (High) في ملف `tera-strategic-advisor.md` — الأول فضفاض، الثاني صارم

---

## Affected Files

1. `.opencode/agents/tera-strategic-advisor.md` — تعديل §8 + §9

---

## Proposed Change

### 1. تعديل §8 — Impact Classification Gate

**من:**

```
| Medium impact | affects direction, scope, architecture, or cost | inspect relevant context if available |
```

**إلى:**

```
| Medium impact | affects direction, scope, architecture, or cost | read the relevant source files yourself; do not rely solely on summaries or second-hand reports |
```

### 2. إضافة سطر في §9 — Evidence and Research Rules

بعد السطر `Read only what is relevant to the question.` أضف قاعدة جديدة:

```
Important: "Reading" means inspecting the source files themselves (blueprints, plans, architecture docs, data models — whichever is relevant). Summaries, reports from other agents, and verbal briefings are supporting context, not substitutes for primary source inspection.
```

---

## Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| إنشاء ملف جديد بدلاً من تعديل الموجود | تضخم — التعديل بسيط ويُحل في الملف نفسه |
| جعل الشرط "High impact فقط" | غير كافٍ — معظم توصيات المستشار متوسطة الأثر وليست عالية |
| ترك الوضع الحالي | الفجوة حدثت فعلياً — إهمالها يخاطر بتكرارها |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | المستشار يصدر توصيات دون قراءة المصادر |
| لماذا لا يكفي تعديل ملف موجود؟ | التعديل في الملف نفسه |
| لماذا لا يكفي عميل موجود؟ | المستشار هو العميل المعني |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلّله** — قاعدة واضحة تمنع إعادة العمل مستقبلاً |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | لا — يجب قراءة المصادر ذات الصلة وهو واجب |

---

## Risk

| المخاطرة | مستواها | التخفيف |
|----------|---------|---------|
| إبطاء التوصيات بقراءة غير ضرورية | 🟢 منخفض | القاعدة تنص على "الملفات ذات الصلة" فقط — ليس كل شيء |

---

## Rollback Plan

1. إعادة §8 إلى `inspect relevant context if available`
2. حذف السطر المضاف في §9

---

## Approval Required

Majed:
- [ ] Approval: تعديل §8 في `tera-strategic-advisor.md`
- [ ] Approval: إضافة قاعدة في §9
