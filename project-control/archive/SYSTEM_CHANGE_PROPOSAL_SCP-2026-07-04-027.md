# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-04-027 — TCEA 6 Improvements (GAP-004)

**Request Type:** Agent Process Improvement / Policy Update / Anti-Bloat
**Source:** TCEA Proposals (6 items) — Reviewed and recommended by TeraSystemEvolutionAgent

---

## Problem Summary

TCEA حدد 6 فجوات في ملفه المصدر (`TeraClientEngagement.md`) والملفات المرتبطة. بعد تحليلها، تبين أن جميعها حقيقية وتحتاج معالجة.

---

## Proposed Changes

### Item 1 — توحيد Handoff Readiness Gate مع Handoff Package Fields

**File:** `tera-system/TeraClientEngagement.md §3.6.1`

**Current:** Gate conditions قائمة منفصلة (17 بنداً) تختلف عن Package fields (§6.2 — 25 حقلاً). 8 حقول ناقصة من الـ Gate.

**Change:** استبدال القائمة المنفصلة في §3.6.1 بإشارة مباشرة إلى §6.2 كقائمة كاملة، مع حذف القائمة القديمة لمنع التضارب مستقبلاً.

---

### Item 2 — إضافة قالب Discovery Coverage Summary

**File:** `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`

**Current:** لا يوجد قالب — §3.2.3 يحدد المحتوى لكن لا يحدد التنسيق.

**Change:** إضافة قالب Discovery Coverage Summary كجدول (13 صفاً × أعمدة: Domain, Status, Reason, Impact, Risk, Blocks Pricing?, Blocks Handoff?) مع ملخص القرار.

---

### Item 3 — إضافة مسار توضيح ApplicationBlueprintAgent

**File:** `tera-system/TeraClientEngagement.md §5.2`

**Current:** §5.2 يعالج فقط CLARIFICATION_REQUEST.md من TeraAgent.

**Change:** إضافة مسار موازٍ لـ ApplicationBlueprintAgent في §5.2، بنفس التدفق (عبر Majed).

---

### Item 4 — إضافة قاعدة تحديث Discovery Coverage بعد التغيير

**File:** `tera-system/TeraClientEngagement.md §3.2.4`

**Current:** لا توجد قاعدة تلزم التحديث بعد اعتماد الملف.

**Change:** إضافة قاعدة: "إذا تغيرت حالة أي Domain Discovery بعد اعتماد Discovery Coverage Summary، يجب تحديث الملف وإعادة عرضه على Majed."

---

### Item 5 — إضافة ملاحظة المجال 13 المركب

**File:** `tera-system/TeraClientEngagement.md §3.2.3`

**Current:** Domain 13 يُعامل كحقل واحد في المصفوفة مع أنه يغطي مواضيع متعددة.

**Change:** إضافة ملاحظة أسفل المصفوفة: "Domain 13 (Acceptance, Commercials & Warranty) يحتاج تغطية 3 جوانب على الأقل: (أ) معايير القبول والاختبارات, (ب) الميزانية وخطة الدفع, (ج) الضمان والصيانة."

---

### Item 6 — إضافة Question Budget

**File:** `tera-system/TeraClientEngagement.md §3.2.5`

**Current:** Question Budget موجود في `TeraApplicationQuestionBank.md` فقط.

**Change:** إضافة سطر في §3.2.5: "Question Budget: Small 10–15 questions, Medium 20–35, Complex deeper per domain."

---

## Affected Files

| الملف | التغيير |
|---|---|
| `tera-system/TeraClientEngagement.md` | UPDATE — §§3.2.3, 3.2.4, 3.2.5, 3.6.1, 5.2 |
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | UPDATE — إضافة قالب Discovery Coverage Summary |
| `project-control/AGENT_GAPS_LOG.md` | UPDATE — إضافة GAP-004 → Applied |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | UPDATE |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|---|---|
| ما المشكلة التي تحلها؟ | 6 فجوات في ملف TCEA المصدر |
| لماذا لا يكفي تعديل ملف موجود؟ | كل التغييرات في ملفات موجودة — لا ملفات جديدة |
| هل الإضافة تقلل التعقيد أم تزيده؟ | تقلل — توحيد Gate مع Package يمنع تضارب مستقبلي، والقالب يمنع تنسيقات مختلفة |
| أثر التوكنز؟ | ضئيل — بضع أسطر لكل تغيير |
| طريقة أصغر؟ | هذه هي الطريقة الأصغر — تعديلات دقيقة في ملفات موجودة |

---

## Risk

- **منخفض جداً:** تغييرات دقيقة في ملف مصدر واحد + قالب — لا تغيير في صلاحيات أو أدوار أو تدفقات رئيسية.

## Rollback Plan

1. إزالة التغييرات من §§3.2.3, 3.2.4, 3.2.5, 3.6.1, 5.2 في TeraClientEngagement.md
2. إزالة قالب Discovery Coverage Summary من TERA_RUNTIME_TEMPLATES.md

---

## Approval Required: ✅ Majed (تمت الموافقة عبر Question Flow)
