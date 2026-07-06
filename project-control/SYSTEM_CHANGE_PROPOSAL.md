# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-06-037

---

### Title

إدراج `discovery-domains.md` في التحميل الإلزامي عند بداية Discovery (C.4 + D.1)

### Request Type

System Bug / Missing Reference — استيفاء ملاحظة هيئة التدقيق الخارجي

### Problem

الملف `tera-system/client-helpers/tera-client-engagement-discovery-domains.md` يُعرف نفسه كـ **المصدر الرسمي الوحيد** للمجالات الـ 13 (سطر 7-10)، ويأمر بقراءته عند بداية Discovery، لكن:

1. **جدول C.4 🟢 Required Now** في `.opencode/agents/tera-client-engagement.md` — تحت "بداية Discovery" — لا يدرجه. موجود فقط: `TeraApplicationQuestionBank.md` و `TeraClientPolicy.md`.
2. **جدول D.1 Routing Table** — لا يحتوي على أي إدخال للمجالات الـ 13 أو `discovery-domains.md`.

### Evidence

- **discovery-domains.md:7-10:** *"هذا الملف هو المصدر الوحيد المعتمد... اقرأه فقط عند بدء Discovery لعميل جديد"*
- **tera-client-engagement.md:410-411:** قائمة Required Now عند Discovery تخلو من هذا الملف
- **tera-client-engagement.md:554-578:** D.1 Routing Table لا يتضمن إدخالاً للمجالات الـ 13
- **TeraPolicyMap.md:33:** يعترف بـ `discovery-domains.md` كـ "Canonical source for the 13 domains" — لكن الرنتايم لا يحمّله إلزامياً

### Affected Files

1. `.opencode/agents/tera-client-engagement.md` — جدول C.4 🟢 Required Now (سطر ~411)
2. `.opencode/agents/tera-client-engagement.md` — جدول D.1 Routing Table (سطر ~554-578)

### Proposed Change

#### تعديل 1: إضافة discovery-domains.md إلى 🟢 Required Now — بداية Discovery

بعد السطر `| | \`tera-system/TeraClientPolicy.md\` | سياسة العميل — اقرأه قبل CLIENT_INTAKE.md |` — أضف:

```markdown
| | `tera-system/client-helpers/tera-client-engagement-discovery-domains.md` | المصدر الرسمي للمجالات الـ 13 — اقرأه قبل إنتاج DISCOVERY_COVERAGE_SUMMARY.md وقبل تقييم B.1 |
```

(اتباعاً لنمط الجدول: أول صف من Trigger "بداية Discovery" يذكر المسبب والصفوف التالية تترك الخلية الأولى فارغة)

#### تعديل 2: إضافة discovery-domains.md إلى D.1 Routing Table

أضف السطر التالي في المكان المناسب (قبل إدخال B.1 Discovery Coverage Gate):

```markdown
| **13 Discovery Domains** — تحتاج تعريف/ترقيم/Blocking Rules للمجالات | `discovery-domains.md` | جدول المجالات الرسمي + قواعد Blocks Pricing / Blocks Handoff |
```

(باستخدام الاسم المختصر `discovery-domains.md` لاتساقًا مع بقية إدخالات D.1)

### Why This Is Necessary

1. **الملف المصدر يأمر بقراءته** عند بداية Discovery لكن جدول التحميل لا يدرجه — هذا تناقض.
2. **المحتوى تشغيلي مهم:** الترقيم الرسمي، الأسماء المعتمدة، Minimum Coverage، وقواعد الحجب (يحجب التسعير؟ يحجب الهاندوف؟) — هذه معلومات يحتاجها TCEA قبل إنتاج `DISCOVERY_COVERAGE_SUMMARY.md` وقبل تقييم `B.1 Discovery Coverage Gate`.
3. **ملاحظة هيئة التدقيق الخارجي** — هذه هي الملاحظة الوحيدة المتبقية، وإغلاقها ينهي مراجعة التدقيق بالكامل.
4. **TeraPolicyMap.md** يعترف به كـ "Canonical source" — الرنتايم يجب أن يعكس هذا.

### Rejected Alternatives

1. **تعديل discovery-domains.md لإزالة عبارة "اقرأني عند بداية Discovery"** — مرفوض لأن الملف فعلاً يحتاج قراءته في ذلك التوقيت. المشكلة في الرنتايم، لا في المصدر.
2. **تحويله إلى Reference Only** — مرفوض لأنه ملف تشغيلي مهم يحتاجه TCEA قبل إنتاج مخرجات Discovery.
3. **إضافته في C.4 فقط دون D.1** — D.1 هو نظام الملاحة الأساسي، وإغفاله يضعف فائدة الجدول.

### Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | فجوة في جدول التحميل — ملف إلزامي غير مذكور |
| لماذا لا يكفي تعديل ملف موجود؟ | التعديل هو في ملف موجود — إضافة سطرين لملف الرنتايم نفسه |
| لماذا لا يكفي عميل موجود؟ | TCEA هو العميل المستهدف — التعديل في ملف تعريفه |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — تمنع TCEA من تخطي ملف إلزامي لأنه غير مذكور في الجدول |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | لا — إضافة سطرين نص فقط |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — سطرين كافيان لحل الفجوة بالكامل |

### Risk

- **منخفض.** لا تغيير في المنطق أو السياسات. مجرد إضافة مرجعين في جداول موجودة.
- إذا كان هناك تزامن رنتايم مطلوب لـ `.opencode/agents/tera.md` — الغالب لا، لأن `tera-client-engagement.md` هو ملف TCEA وليس `tera.md`.

### Rollback Plan

- إزالة السطرين المضافة من `.opencode/agents/tera-client-engagement.md` باستخدام edit.

### Approval Required

- ✅ Majed

---

Prepared by: TeraSystemEvolutionAgent (حارس)
Date: 2026-07-06
