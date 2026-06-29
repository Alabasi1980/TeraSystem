# TeraTokenPolicy.md

# سياسة إدارة السياق وتقليل التوكنز في منظومة Tera Agent

## 1. الهدف

هذا الملف يحدد سياسة رسمية لإدارة السياق والتوكنز داخل منظومة **Tera Agent** عند العمل على مشاريع برمجية متعددة الملفات والعملاء الفرعيين.

الهدف ليس تقليل التوكنز بأي ثمن، بل:

- تقليل التكلفة والهدر.
- منع قراءة ملفات غير لازمة.
- الحفاظ على دقة القرار والتنفيذ.
- تمرير أقل سياق كافٍ لكل مهمة.
- جعل ملفات المشروع هي مصدر الحقيقة بدل الاعتماد على ذاكرة المحادثة.

---

## 2. القاعدة الأساسية

لا يُسمح لـ Tera Agent أو أي Sub-Agent باستخدام **Full Context** افتراضيًا.

السياق الافتراضي لأي مهمة أو Session جارية هو:

```text
TERA_ACTIVE_CONTEXT.md (if exists) + Task Context + الملفات المحددة صراحة في التفويض
```

أي قراءة كاملة لعدة ملفات أو لكل المشروع تحتاج سببًا واضحًا وقد تحتاج موافقة المستخدم.

---

## 3. مصادر السياق الرسمية

يعتمد Tera على المصادر التالية بالترتيب:

| المصدر | الاستخدام |
|---|---|
| `project-control/TERA_ACTIVE_CONTEXT.md` | نقطة البداية المختصرة للجلسات الجارية واستئناف العمل |
| `project-control/PROJECT_STATE.md` | الذاكرة المختصرة المعتمدة للمشروع |
| `project-preparation/TERA_PROJECT_DECISION.md` | قرار المشروع الافتتاحي |
| `project-preparation/PROJECT_RULES.md` | قواعد المشروع الخاصة عند وجودها |
| ملفات `project-preparation/` المعتمدة | تفاصيل النطاق، البيانات، الشاشات، التقنية، الاختبار |
| `project-control/` | المهام، القرارات، الفجوات، سجل النشاط |
| الكود الفعلي | عند دخول مرحلة التنفيذ فقط |

المحادثة ليست مصدر حقيقة دائم.  
أي قرار مهم يجب أن يُحفظ في ملف رسمي.

---

## 4. أنواع السياق

| النوع | الوصف | متى يستخدم |
|---|---|---|
| `Full Context` | قراءة ملف كامل أو مجموعة ملفات كاملة | عند التأسيس، المراجعة الشاملة، أو وجود تعارض |
| `Task Context` | أقل سياق مطلوب للمهمة الحالية | الوضع الافتراضي |
| `Summary Context` | ملخص معتمد من `PROJECT_STATE.md` أو ملخص ملف | عند كبر الملفات أو تكرار الرجوع إليها |
| `Diff Context` | التغييرات فقط بدل إعادة قراءة الملف كاملًا | عند مراجعة تعديل أو تنفيذ جزئي |
| `Retrieved Context` | مقتطفات محددة من ملفات أو كود | عند البحث عن جزء مرتبط بالمهمة |
| `Context Pack` | حزمة سياق قصيرة لمرحلة أو عميل | عند تكرار نوع معين من المهام |

---

## 5. متى يسمح بقراءة ملف كامل؟

يسمح بقراءة ملف كامل فقط في الحالات التالية:

- بداية مشروع أو مرحلة جديدة.
- ملف صغير ومباشر ولا يسبب تكلفة واضحة.
- مراجعة اتساق بين ملفات.
- ملف يحتوي قرارات أساسية أو تقنية حاسمة.
- وجود تعارض أو غموض لا يحله الملخص.
- قبل تسليم مرحلة حرجة.
- عند طلب المستخدم ذلك صراحة.

يجب أن يذكر Tera سبب القراءة الكاملة إذا كانت المهمة قد تستهلك سياقًا كبيرًا.

---

## 6. متى يمنع Full Context؟

يمنع استخدام Full Context عندما:

- تكون المهمة صغيرة ومحددة.
- المطلوب تعديل موضع واحد أو ملف واحد.
- توجد خلاصة كافية في `PROJECT_STATE.md`.
- توجد ملفات كثيرة غير مرتبطة بالمهمة.
- العميل الفرعي لا يحتاج إلا ملفًا أو قسمًا محددًا.
- يمكن استخدام diff أو مقتطفات بدل الملف الكامل.

---

## 7. سياسة استدعاء العملاء الفرعيين

قبل استدعاء أي Sub-Agent، يجب على Tera تحديد:

```text
Task ID:
Requested Agent:
Objective:
Context Type:
Reference Files:
Required Sections:
Allowed Write Targets:
Token Budget:
Expected Output Limit:
Approval Required: Yes/No
```

لا يستدعي Tera العميل الفرعي بعبارة عامة مثل:

```text
راجع المشروع كاملًا
```

إلا إذا كانت المهمة فعلًا مراجعة شاملة وموافقًا عليها.

---

## 8. Token Budget

كل مهمة يجب أن تصنف إلى واحدة من هذه المستويات:

| المستوى | الوصف | القرار |
|---|---|---|
| `Low` | قراءة محدودة ومخرج قصير | ينفذ مباشرة |
| `Medium` | عدة ملفات أو تحليل متوسط | ينفذ مع ضبط المخرجات |
| `High` | قراءة واسعة أو تحليل عميق أو عدة عملاء | يحتاج توضيح سبب |
| `Critical` | مشروع كامل، بحث عميق، مراجعة شاملة، توليد عملاء متعددين | يحتاج موافقة المستخدم |

---

## 9. حدود المخرجات

المخرجات الافتراضية للعملاء الفرعيين يجب أن تكون مختصرة.

الصيغة الافتراضية:

```text
Task ID:
Agent:
Status:
Files Updated:
Summary:
Decisions:
Assumptions:
Issues:
Needs Tera Decision:
Recommendation:
```

لا يعيد العميل شرح المشروع كاملًا.  
يذكر فقط ما يخص المهمة.

---

## 10. Model Routing

لا تربط السياسة بأسماء نماذج ثابتة.

هذه سياسة إلزامية لاختيار أضعف مستوى نموذج كافٍ قبل قرارات المراحل، مهام التنفيذ، التفويضات الكبرى، المراجعات الواسعة، أو العمليات عالية التكلفة.

### Mandatory Model Tier Recommendation

Classification levels:

| Model Tier | Use When | Avoid When |
|---|---|---|
| `Light Model` | formatting, simple edits, short summaries, checklist completion, small file updates, non-risky wording changes | architecture, security, complex reasoning, multi-file execution |
| `Medium Model` | normal analysis, scoped implementation planning, structured documentation, limited code review, ordinary task delegation | critical security, hard architecture, unclear scope, high-risk refactoring |
| `Strong Model` | architecture decisions, security-sensitive tasks, complex debugging, broad consistency review, cross-file reasoning, high-risk implementation, unresolved contradictions | routine formatting, simple summaries, small isolated edits |

Rules:

- Tera must use the weakest sufficient model that preserves quality, safety, and traceability.
- Tera must not recommend `Strong Model` just because it is available.
- Tera must not use `Light Model` for tasks with high risk, unclear scope, security sensitivity, or difficult verification.
- If `Medium Model` is enough, Tera should explicitly say that `Strong Model` is not necessary.
- If `Strong Model` is required or strongly recommended, Tera must explain the reason briefly.
- If the environment does not support model switching, Tera must still reduce context, split tasks, and warn the user when the current model may be insufficient.

إذا كانت البيئة لا تدعم اختيار النموذج، يلتزم Tera على الأقل بتقليل السياق والمخرجات.

---

## 11. Human Approval Gates

يجب طلب موافقة المستخدم قبل:

- قراءة كل ملفات المشروع.
- تشغيل عميل فرعي على نطاق واسع.
- توليد عدة عملاء دفعة واحدة.
- تشغيل مهمة عالية التكلفة.
- مراجعة أمنية أو معمارية شاملة.
- الانتقال من Plan Mode إلى Build Mode.
- تنفيذ كود أو أوامر Shell مؤثرة.
- حذف أو إعادة هيكلة ملفات.

---


## 12. Pre-Execution Gate وسلامة نطاق المهمة

قبل أي مهمة تنفيذية، يجب على Tera تطبيق:

```text
tera-system/TeraPreExecutionGate.md
```

الهدف من هذه البوابة هو تقليل التوكنز وتقليل الأخطاء معًا، لأن المهمة الصغيرة الواضحة تحتاج سياقًا أقل وتقل فيها احتمالات التوسع.

قواعد إلزامية:

- لا تفويض قبل نتيجة `Pre-Execution Gate: PASS`.
- إذا فشلت البوابة، يصحح Tera المهمة ذاتيًا بدل طلب تفاصيل من المستخدم.
- أي عنصر يمكن تأجيله لا يدخل في المهمة الحالية.
- أي مهمة تحتوي أكثر من هدف مستقل يجب تقسيمها.
- لا تستخدم سياقًا أو أوامر أوسع من حدود المهمة.

عناصر تعتبر توسعًا ممنوعًا افتراضيًا ما لم تذكر صراحة:

```text
UI, API, Auth, database models or entities, schema changes, database migrations, database apply commands, seed data,
external services, Docker, CI/CD, reusable components,
service/repository layers, state management, extra documentation.

Technology-specific context must be loaded from the active Technology Profile only.
Do not include rules from unrelated technology profiles in task context.
```

---

## 13. PROJECT_STATE.md

يجب تحديث:

```text
project-control/PROJECT_STATE.md
```

بعد:

- اعتماد قرار تيرا.
- اعتماد مرحلة.
- إغلاق مهمة.
- ظهور قرار مهم.
- تغيير نطاق.
- اكتشاف فجوة مؤثرة.
- تنفيذ compaction أو تلخيص مرحلي.

هذا الملف يجب أن يبقى مختصرًا.  
لا يتحول إلى سجل تفصيلي طويل؛ التفاصيل تبقى في ملفاتها الرسمية.

---

## 13.1 TERA_ACTIVE_CONTEXT.md

إذا وجد الملف:

```text
project-control/TERA_ACTIVE_CONTEXT.md
```

فيجب أن يكون نقطة البداية الأولى لأي Session جديدة في مشروع قائم.

وظيفته:

- تلخيص الحالة التشغيلية الحالية
- تلخيص القواعد النشطة
- توضيح آخر المهام المغلقة والمهمة التالية
- توجيه Tera إلى الملفات التي يجب قراءتها عند الحاجة فقط

قيوده:

- لا يحتوي أسرارًا
- لا يحتوي سجلات كاملة
- لا يحتوي تفاصيل تنفيذ طويلة
- لا يصبح نسخة من `PROJECT_STATE.md` أو `PROJECT_ACTIVITY_LOG.md`
- يحدث بعد كل Task مهمة أو عند إغلاق مرحلة

القاعدة:

```text
TERA_ACTIVE_CONTEXT.md = startup handoff
PROJECT_STATE.md = official compact project memory
Other files = detailed source of truth when needed
```

---

## 14. قواعد منع التكرار

يُمنع على Tera أو أي Sub-Agent:

- إعادة شرح معلومات محفوظة في `PROJECT_STATE.md` دون حاجة.
- نسخ محتوى ملفات كاملة في الرد.
- إعادة تلخيص ملف لم يتغير.
- استخدام تاريخ المحادثة بدل الملفات.
- تمرير نفس الملف لعدة عملاء بلا سبب.
- إنتاج تقارير طويلة إذا كان المطلوب قرارًا قصيرًا.

---

## 15. Plan Mode و Build Mode

في OpenCode أو أي بيئة مشابهة:

- مرحلة التحليل والتحضير والمراجعة تتم في `Plan Mode`.
- التنفيذ البرمجي يتم فقط في `Build Mode`.
- لا ينتقل Tera إلى `Build Mode` إلا بعد موافقة صريحة من المستخدم.
- عند الشك، يبقى Tera في `Plan Mode`.

---

## 16. Prompt Caching

إذا كانت البيئة أو المزود يدعم Prompt Caching:

- ضع التعليمات الثابتة أولًا.
- ضع تفاصيل المهمة المتغيرة آخرًا.
- لا تعدل التعليمات الثابتة بلا ضرورة.
- لا تعتمد على الكاش كضمان؛ اعتبره تحسينًا فقط.

---

## 17. قاعدة الجودة

تقليل التوكنز لا يجوز أن يؤدي إلى:

- فقدان قرار مهم.
- تجاهل قيد من قيود المشروع.
- تفويت متطلب أمني أو مالي.
- إخفاء نقص في المعلومات.
- تنفيذ مهمة بناءً على ملخص غير كافٍ.

إذا كان الاختصار يهدد الجودة، يستخدم Tera سياقًا أوسع ويذكر السبب.

---

## 18. القاعدة النهائية

أفضل سياق هو **أصغر سياق كافٍ** لاتخاذ قرار صحيح أو تنفيذ مهمة صحيحة.

Tera مسؤول عن التوازن بين:

```text
تكلفة أقل + دقة كافية + وضوح قابل للمراجعة
```
