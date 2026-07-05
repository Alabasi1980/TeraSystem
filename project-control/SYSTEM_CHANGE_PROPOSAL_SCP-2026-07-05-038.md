# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-05-038

**Title:** TCEA Strengthening — 4 New Governance Rules (Final Scope Reconciliation + Budget-to-Scope Control + Approval Consistency + Decision Register)

**Request Type:** Agent Improvement / Policy Update / Process Gap Closure

**Date:** 2026-07-05

---

## Problem

TCEA (مُستشار) أنتج حزمة تسليم لعميل "مؤسسة الموثوق لصيانة الأجهزة" كشفت 4 فجوات نظامية في سلوكه وإعداداته:

1. **تضارب النطاق بين الملفات:** ميزات وُصفت بأنها داخل MVP في ملف وخارج النطاق في ملف آخر (قطع الغيار، التقارير) — مما يسبب ارتباكاً downstream عند ApplicationBlueprintAgent و TeraAgent.

2. **عدم اتساق حالة الاعتماد:** TERA_HANDOFF_PACKAGE.md بحالة "Approved" بينما ملفات المصدر (CLIENT_INTAKE, SCOPE_SUMMARY, FEATURE_LIST) لا تزال "Draft" — مما يخلق تناقضاً في مصدر الحقيقة.

3. **توسع النطاق رغم الميزانية المحدودة:** العميل قال صراحة "الميزانية محدودة" و"أبسط حل"، لكن TCEA ضمّن ميزات غير أساسية (قطع غيار كاملة، تقارير تحليلية) في MVP دون تصنيفها كـ Optional أو Phase 2.

4. **غياب توثيق موحد للقرارات:** قرارات العميل الهامة وثقت بتفسيرات مختلفة بدون حالات موحدة (معتمد / مؤجل / مشروط / غير محسوم) — مما يجعل مرجعيتها غير واضحة.

---

## Evidence

1. تقرير Majed عن ثغرات TCEA — تصنيف: 15% هوية / 35% سلوك تحليلي / 50% قوانين وإعدادات ناقصة.
2. مراجعة ملفات عميل Mawthooq: SCOPE_SUMMARY.md, FEATURE_LIST.md, DRAFT_QUOTATION.md — تباين في تصنيف الميزات.
3. CLIENT_DECISION_LOG.md — حالات القرارات غير موحدة.
4. CLIENT_INTAKE.md — حالة المشروع "confirmed" بينما ملفات أخرى "draft".

---

## Affected Files

| ملف | نوع التغيير |
|-----|-------------|
| `tera-system/TeraClientEngagement.md` | إضافة 3 أقسام جديدة + تحديث 1 قسم |
| `.opencode/agents/tera-client-engagement.md` | مزامنة الـ 4 قواعد في runtime |
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | تحديث قالب FEATURE_LIST.md (إضافة عمود Final Status) |

**عدد الملفات الجديدة: 0**

---

## Proposed Change

### 1. Final Scope Reconciliation Gate (§3.3.1 — جديد في TeraClientEngagement.md)

قبل Tera Handoff Readiness Gate، يُلزَم TCEA بإنتاج Scope Reconciliation داخل `FEATURE_LIST.md` تحدد لكل ميزة حالة واحدة فقط:
- `✅ Included in MVP` — ضمن نطاق المرحلة الأولى نهائياً
- `◉ Optional if budget allows` — تضاف إذا بقيت ميزانية
- `⏳ Phase 2` — مؤجل للمرحلة الثانية صراحة
- `❌ Out of Scope` — خارج النطاق نهائياً

**قواعد صارمة:**
- لا يجوز أن تكون الميزة الواحدة في حالتين مختلفتين عبر ملفات متعددة.
- لا يجوز إنتاج `TERA_HANDOFF_PACKAGE.md` قبل إتمام Scope Reconciliation.
- كل حالة تحتاج تبريراً مختصراً بسطر واحد.

### 2. Budget-to-Scope Control Rule (§3.3.2 — جديد في TeraClientEngagement.md)

عندما يصرح العميل بأن الميزانية محدودة أو يطلب "الحد الأدنى":
- تُصنف كل ميزة كـ: Essential / Important / Nice-to-have
- **Essential** ← يمكن أن تكون في MVP
- **Important** + ميزانية محدودة ← Optional أو Phase 2
- **Nice-to-have** + ميزانية محدودة ← Phase 2 أو Out of Scope
- لا يجوز وضع Important أو Nice-to-have في MVP مع ميزانية محدودة دون تبرير كتابي من Majed

### 3. Client Decision Register (§3.3.3 — جديد في TeraClientEngagement.md)

توثيق كل قرار مهم في `CLIENT_DECISION_LOG.md` بـ 4 حالات موحدة:
- `✅ معتمد (Approved)` — بعد تأكيد صريح من Majed
- `⏳ مؤجل (Deferred)` — لمرحلة لاحقة
- `⚠️ مشروط (Conditional)` — معتمد بشرط
- `❓ غير محسوم (Not Finalized)` — كلام غير ملزم بعد

**قاعدة:** لا Approved لأي قرار لم يؤكده Majed. لا Handoff مع أي قرار Not Finalized.

### 4. Approval Consistency Rule (إضافة إلى §3.6.1 Tera Handoff Readiness Gate)

لا يجوز وضع `TERA_HANDOFF_PACKAGE.md` بحالة `Approved` إذا كان أي من:
- CLIENT_INTAKE.md
- SCOPE_SUMMARY.md
- FEATURE_LIST.md
- DRAFT_QUOTATION.md
- CLIENT_DECISION_LOG.md

لا يزال بحالة `Draft` أو `Pending`. الحزمة تأخذ حالة أقل ملف مصدر.

### تدفق العمل الجديد (تحديث §5.1 في TeraClientEngagement.md + §3 في runtime)

```
... ← بعد الموافقة: إنتاج ملفات النطاق حسب الحاجة
    ← **Budget-to-Scope Control Rule** — عند التصنيف
    ← **Client Decision Register** — مع كل قرار
    ← **Final Scope Reconciliation Gate** — توحيد كل الميزات
    ← Quotation Readiness Gate + DRAFT_QUOTATION.md
    ← **Approval Consistency Check** — ضمن Handoff Gate
    ← Tera Handoff Readiness Gate
    ← TERA_HANDOFF_PACKAGE.md
```

---

## Why This Is Necessary

بدون هذه القواعد الأربع:

1. **TCEA سيستمر بإنتاج حزم تسليم غير متسقة** — يضع الميزات بتصنيفات مختلفة في ملفات متعددة، مما يربك ApplicationBlueprintAgent و TeraAgent ويسبب إعادة عمل.

2. **تضارب حالة الاعتماد سيبقى مصدر خطأ** — فريق التنفيذ لن يعرف أي ملف هو المعتمد فعلاً.

3. **توسع النطاق سيبقى يضغط الميزانية** — ميزات غير ضرورية تدخل MVP لأن TCEA لم يربط تصنيفها بالميزانية المعلنة.

4. **قرارات العميل ستبقى غير موثقة بشكل نظامي** — قد تُفقد أو تُفسر بشكل خاطئ في مراحل لاحقة.

هذه القواعد الأربع تغطي 85% من الثغرات التي شخّصها Majed (35% سلوك + 50% إعدادات).

---

## Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| إنشاء عميل جديد "ScopeControllerAgent" | تضخم غير مبرر — القواعد تحتاج تقوية القوانين، ليس عميلاً جديداً |
| إنشاء ملف SCOPE_RECONCILIATION.md منفصل | تكرار مع FEATURE_LIST.md الموجود — يضاف العمود إليه |
| إنشاء ملف CLIENT_APPROVAL.md منفصل | CLIENT_DECISION_LOG.md الموجود يكفي بعد توحيد الحالات |
| تعديل هوية TCEA | غير مطلوب — الهوية جيدة، المشكلة في القواعد والسلوك (15% فقط هوية) |
| إضافة MCPs أو أدوات جديدة | غير ضروري — القواعد إجرائية ووثائقية بحتة |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | 4 فجوات في اتساق النطاق، الاعتماد، الميزانية، وتوثيق القرارات |
| لماذا لا يكفي تعديل ملف موجود؟ | التعديل في ملفات موجودة بالفعل — 0 ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | TCEA نفسه هو العميل المعني — نقوّي قواعده |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — تمنع التناقضات قبل حدوثها، وتوحّد حالات القرارات |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | ضئيل — إضافة 3 أقسام قصيرة (50-80 سطراً إجمالاً) في ملف المصدر |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — 4 قواعد فقط، كل واحدة تغطي فجوة محددة. لا ملفات جديدة |

**النتيجة: ✅ PASS**

---

## Risk

| المخاطرة | المستوى | التخفيف |
|----------|---------|---------|
| زيادة وقت TCEA في مرحلة Scope Packaging | منخفض | القواعد واضحة ومدمجة في التدفق الحالي — لا خطوات إضافية كثيرة |
| قد يشعر TCEA بتقييد زائد | منخفض | القواعد تعزز دقته وتمنع الأخطاء — لا تقيد تحليله الإبداعي |
| تعارض مع قواعد قديمة في نفس الملف | منخفض | القواعد الجديدة تكمل الموجودة ولا تتعارض معها (Self-Check, Uncertainty, Depth Scaling) |
| نسيان TCEA تطبيق القواعد في أول عميل بعد التحديث | منخفض | القواعد تنضاف للمصدر والـ runtime معاً — والـ Conduct Gate تذكره بقراءة المصدر |

---

## Rollback Plan

1. `tera-system/TeraClientEngagement.md`: حذف §§3.3.1, 3.3.2, 3.3.3 + إزالة الإضافة من §3.6.1
2. `.opencode/agents/tera-client-engagement.md`: إزالة الإشارات للقواعد الأربع من §§3, 7
3. `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`: إزالة عمود Final Status من قالب FEATURE_LIST.md
4. إعادة `Last Synced` في runtime إن لزم

---

## Approval Required

- [ ] Majed — موافقة على SYSTEM_CHANGE_PROPOSAL
- [ ] Majed — تحديد أي تعديل إن لزم على صياغة القواعد
