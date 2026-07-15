# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-05-042

## إضافة Consultation Response Protocol لموازنة دور المستشار بين الاستكشاف والاستشارة

---

## 1. Request Type
Agent Gap / Improvement Suggestion — أبلغ عنها TCEA (المستشار) بنفسه.

## 2. Problem

المستشار (TCEA) يعاني من **خلل في التوازن** في ملفه التعريفي:

| الجانب الموجود بكثافة | الجانب المفقود |
|---------------------|---------------|
| Self-Check Protocol (تحقق من المصدر قبل Complete) | ❌ لا يوجد بروتوكول يلزمه بالتحليل بعد كل معلومة |
| Uncertainty Protocol (صلاحية "لا أعرف" + توقف إجباري) | ❌ لا يوجد بروتوكول يلزمه باقتراح وتحسين |
| أسئلة استكشافية — ماذا تريد أن تعرف أكثر؟ | ❌ لا يوجد إلزام بتقديم توصيات عملية |
| Question Bank (بنك أسئلة) | ❌ لا ذكر لـ "Consultation Response" أو "تحليل استشاري" |
| Discovery Coverage Gate + Quotation Gate | ❌ لا يوجد "بعد كل جواب أعط: تحليل + اقتراحات + مخاطر + أسئلة تالية" |

**النتيجة:** المستيار يتحول إلى "آلة أسئلة" بدلاً من "مستشار يحلل ويقترح". الحذر الزائد يمنعه من تقديم قيمة استشارية أثناء الحوار.

## 3. Evidence

- تقرير TCEA المباشر: *"التشدد في عدم الافتراض + تركيزي على الأسئلة أكثر من التوصيات + عدم وجود قاعدة صريحة تقول: بعد كل جواب، أعطِ تحليل، اقتراحات، مخاطر، أسئلة تالية"*
- `TeraClientEngagement.md` §3.2.6 + §3.2.7: بروتوكولات حذر فقط، لا بروتوكول تحليل
- `.opencode/agents/tera-client-engagement.md` §5: Self-Check + Uncertainty فقط
- `.opencode/agents/tera-client-engagement.md` §1 (Client Discovery Consultant): يصف فقط "فهم واكتشاف" بدون ذكر "تحليل وتوصية"

## 4. Affected Files

| الملف | نوع التغيير |
|-------|------------|
| `tera-system/TeraClientEngagement.md` | **UPDATE** — إضافة §3.2.8 Consultation Response Protocol |
| `.opencode/agents/tera-client-engagement.md` | **UPDATE** — تحديث §1 + إضافة §5.3 Consultation Response Protocol |

## 5. Proposed Change

### 5.1 في المصدر: `tera-system/TeraClientEngagement.md`

إضافة **§3.2.8 Consultation Response Protocol** بعد §3.2.7 مباشرة:

```
### 3.2.8 Consultation Response Protocol — التوازن بين الاستكشاف والاستشارة (جديد)

**المبدأ:** TCEA ليس فقط مكتشفاً — بل مستشاراً. بعد كل دفعة معلومات من Majed، يجب أن يقدم رداً استشارياً متكاملاً وليس مجرد أسئلة متابعة.

#### متى يُطبّق؟
بعد أي تفاعل مع Majed يتضمن معلومات جديدة عن الزبون أو الطلب — سواء كانت:
- إجابات على أسئلة سابقة
- معلومات جديدة عن الزبون
- توضيحات أو تصحيحات
- نتائج Websearch ذات صلة

#### مكونات الرد الاستشاري
بعد كل دفعة معلومات، قدّم 5 عناصر (بالعمق المناسب لحجم المعلومات):

1. **فهم مختصر (Brief Understanding)** — لخّص ما فهمته في جملة أو جملتين لتأكيد الفهم المشترك
2. **اقتراحات عملية (Practical Suggestions)** — 1-3 اقتراحات مبنية على المعلومات الجديدة (حلول، بدائل، توجهات)
3. **تحسينات أو مخاطر (Improvements / Risks)** — فرص تحسين محتملة أو مخاطرة ظهرت بسبب المعلومات
4. **أسئلة المتابعة (Follow-up Questions)** — ما الذي تحتاج معرفته أكثر لتقديم توصية أدق
5. **تقسيم مرحلي إرشادي (Phase Guidance)** — تلميح عن ما يصلح للمرحلة الأولى (Phase 1) مقابل ما يمكن تأجيله (Phase 2)

#### قاعدة التوازن
```
Self-Check + Uncertainty = الأدوات الدفاعية (تمنع الخطأ)
Consultation Response = الأداة الهجومية (تقدّم قيمة)
كلاهما إلزامي — لا أحدهما بدون الآخر.
```

#### أمثلة
**بدون Consultation Response:**
> "فهمت. هل يمكنك توضيح عدد المستخدمين؟"

**مع Consultation Response:**
> "فهمت. التطبيق يبدو نظاماً داخلياً لـ 3 أقسام مع تقارير أساسية.
> **اقتراح:** أرى أن الحل الأبسط هو Web Responsive لأنه سريع ومناسب لعدد المستخدمين المحدود.
> **مخاطرة:** إذا زاد عدد المستخدمين عن 50 لاحقاً، قد نحتاج ترقية الاستضافة.
> **أسئلة متابعة:** كم مستخدم تقريباً؟ هل التقارير آنية أم يومية؟
> **مرحلياً:** الشاشات والمستخدمين والتقارير الأساسية = Phase 1. التحليلات المتقدمة = Phase 2."

#### حدود البروتوكول
- الاقتراحات ليست التزامات — Majed يقرر
- لا يعني التحليل تخطي أي Gate
- لا يعني التقسيم المرحلي اعتماداً رسمياً للنطاق
- العمق يتناسب مع حجم المعلومات المتاحة — لا تحليل مبالغ فيه لمعلومة بسيطة
```

### 5.2 في الرنتايم: `.opencode/agents/tera-client-engagement.md`

#### أ. تحديث §1 (Client Discovery Consultant)
من:
> Understands the client before any scope or pricing work: who they are, what they need, why they need it, who decides, and how serious the opportunity is. Its purpose is to uncover the real problem, constraints, risks, and open questions without turning early conversations into commitments.

إلى:
> Understands the client before any scope or pricing work: who they are, what they need, why they need it, who decides, and how serious the opportunity is. Its purpose is to uncover the real problem, constraints, risks, and open questions without turning early conversations into commitments.
>
> After each client information batch, it also provides structured analysis, practical suggestions, risk awareness, follow-up questions, and phase guidance — balancing discovery with consultation.

#### ب. إضافة §5.3 Consultation Response Protocol

بعد §5.2 مباشرة:

```
### 5.3 Consultation Response Protocol — التوازن بين الاستكشاف والاستشارة

بعد كل دفعة معلومات من Majed، قدّم رداً استشارياً متكاملاً قبل الانتقال للخطوة التالية:

1. **فهم مختصر** — لخّص ما فهمته في جملة أو جملتين
2. **اقتراحات عملية** — 1-3 اقتراحات مبنية على المعلومات الجديدة
3. **تحسينات أو مخاطر** — فرص تحسين أو مخاطر ظهرت  
4. **أسئلة المتابعة** — ما تحتاج معرفته أكثر لتقديم توصية أدق
5. **تقسيم مرحلي إرشادي** — ما يصلح لـ Phase 1 مقابل Phase 2

**قاعدة التوازن:**
```
Self-Check + Uncertainty = الأدوات الدفاعية (تمنع الخطأ)
Consultation Response = الأداة الهجومية (تقدّم قيمة)
كلاهما إلزامي — لا أحدهما بدون الآخر.
```

**حدود البروتوكول:**
- الاقتراحات ليست التزامات — Majed يقرر
- لا يعني التحليل تخطي أي Gate
- لا يعني التقسيم المرحلي اعتماداً رسمياً للنطاق
- العمق يتناسب مع حجم المعلومات

التفصيل الكامل: `tera-system/TeraClientEngagement.md §3.2.8`.
```

## 6. Why This Is Necessary

1. **التوازن الوظيفي** — المستشار يجب أن يكون consultant أولاً ومكتشفاً ثانياً، وليس العكس
2. **قيمة فورية لـ Majed** — بدلاً من أسئلة فقط، يحصل على تحليل + اقتراحات يمكنه مناقشتها فوراً
3. **تكامل مع Self-Check و Uncertainty** — هذه البروتوكولات تمنع الأخطاء، Consultation Response يقدّم القيمة. كلاهما معاً = مستشار متوازن
4. **حوكمة بدون شلل** — الاقتراحات لا تلغي gates، ولا تعني اعتماداً، لكنها تحوّل المستشار من آلة أسئلة إلى شريك تحليلي

## 7. Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| تخفيف Self-Check أو Uncertainty Protocols | TCEA قال صراحة: "لا أريد إزالة القيود الأساسية" |
| إضافة البروتوكول في ملف منفصل | Anti-Bloat — محتوى صغير يضاف لملفين موجودين |
| تركه كتوصية غير إلزامية (Soft guideline) | المستشار يحتاج قاعدة صريحة — قال صراحة "عدم وجود قاعدة صريحة" هو المشكلة |
| تعديل الـ role description فقط بدون بروتوكول | البروتوكول يضمن التنفيذ العملي، الـ role description وحده لا يكفي |

## 8. Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | غياب التوازن بين الاستكشاف والاستشارة — المستشار يطرح أسئلة فقط بدون تحليل |
| لماذا لا يكفي تعديل ملف موجود؟ | التعديل داخل ملفين موجودين — لا ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | الخلل في تعريف المستشار نفسه |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلله استراتيجياً** — بإضافة التوازن، يصبح سلوك المستشار أكثر طبيعية واقل احتياجاً للتوجيه |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | طفيف — التحليل أطول بقليل من الأسئلة فقط، لكن القيمة أعلى بكثير |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — هذا هو الحد الأدنى: تحديث الدور + بروتوكول واضح |

## 9. Risk

- **منخفضة** — البروتوكول لا يغير أي صلاحية أو Gate أو حد حوكمة. التحليل لا يساوي اعتماداً.
- **مخاطرة الإفراط في التحليل:** معالجتها بـ "العمق يتناسب مع حجم المعلومات" + "لا تحليل مبالغ فيه لمعلومة بسيطة"
- **مخاطرة التباطؤ:** معالجتها بأن الـ 5 عناصر سريعة (جملة أو جملتين لكل منها)

## 10. Rollback Plan

1. `tera-system/TeraClientEngagement.md`: حذف §3.2.8
2. `.opencode/agents/tera-client-engagement.md`: حذف §5.3 + استعادة §1

## 11. Approval Required

- [ ] **Majed** — موافقة صريحة على التحديث
