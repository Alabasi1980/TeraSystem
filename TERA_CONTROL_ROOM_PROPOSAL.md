# Tera Control Room

## وثيقة الفكرة والتصميم وخطة التنفيذ

**الإصدار:** 3.0  
**الحالة:** مقترح نهائي جاهز لتحويله إلى مواصفات تنفيذية  
**نطاق الوثيقة:** تعريف النظام، الأدوار، دورة العمل، السياسات، الأدلة، العزل، المراجعة، وحدود الـMVP  
**القرار المقترح:** بناء النظام كتطبيق خارجي فوق OpenCode، والبدء بنسخة CLI ضيقة قبل أي واجهة ويب أو تكامل عميق.

---

## 1. الملخص التنفيذي

**Tera Control Room** هو نظام مركزي لتشغيل وإدارة منظومة عملاء Tera من خلال نقطة دخول واحدة.

يرسل المستخدم الهدف إلى **Supervisor** بدل فتح بروفايلات العملاء يدويًا ونقل السياق والنتائج بينهم. يقوم النظام بعد ذلك بتصنيف الطلب، اختيار المسار المناسب، تشغيل العميل المسؤول، جمع الأدلة، تنفيذ المراجعة، إدارة التصحيح، ثم رفع القرارات المهمة فقط إلى المستخدم.

المبدأ التشغيلي:

```text
Majed يحدد الهدف ويعتمد القرارات المهمة.
Supervisor يدير دورة العمل والجلسات والسياسات.
TeraAgent يدير تنفيذ تطبيقات العملاء.
عملاء التنفيذ يعملون تحت TeraAgent فقط.
المراجعون يراجعون باستقلال ولا ينفذون الإصلاحات.
الكود والسياسات والأدلة تحدد الانتقال والقبول.
```

النظام ليس دردشة جماعية مفتوحة بين العملاء، وليس عميلًا ذكيًا يقرر كل شيء. هو **Workflow Orchestrator محكوم بقواعد ثابتة، صلاحيات دقيقة، ومخرجات منظمة**.

---

## 2. المشكلة التي يحلها النظام

التشغيل اليدوي الحالي يتطلب من المستخدم أن:

- يحدد العميل المناسب لكل طلب.
- يفتح جلسة أو بروفايل كل عميل.
- ينقل السياق والمخرجات بين العملاء.
- يقرر متى تبدأ المراجعة ومن يراجع.
- يتابع دورات التصحيح بنفسه.
- يجمع نتائج Git والاختبارات والتقارير.
- يفرّق بين قرار تشغيلي عادي وقرار يحتاج موافقته.
- يمنع تداخل الصلاحيات والأدوار يدويًا.

هذا يجعل المستخدم مدير جلسات بدل أن يكون مالك المنظومة وصاحب القرار.

الحل هو طبقة تشغيل مركزية تتولى هذه الأعمال آليًا دون إلغاء الأدوار الحالية أو السماح لأي عميل بتجاوز اختصاصه.

---

## 3. أهداف النظام

يجب أن يحقق النظام ما يلي:

1. توفير نقطة دخول واحدة لكل الطلبات.
2. اختيار المسار والعميل المناسبين تلقائيًا وفق قواعد معلنة.
3. تشغيل كل عميل داخل جلسة مستقلة وبصلاحيات محددة.
4. الحفاظ على استقلال المراجعة عن التنفيذ.
5. جمع أدلة فعلية قابلة للتحقق لكل مهمة.
6. إدارة التصحيحات التشغيلية تلقائيًا ضمن حدود ثابتة.
7. تصعيد القرارات المهمة فقط إلى المستخدم.
8. منع الحلقات، التضخم، وتجاوز الميزانية.
9. حفظ سجل كامل يوضح ماذا حدث ولماذا تم اتخاذ القرار.
10. تمكين إضافة عملاء ومسارات جديدة لاحقًا دون إعادة بناء النظام.

---

## 4. ما لا يهدف النظام إلى فعله

في النسخة الأولى لا يهدف النظام إلى:

- استبدال OpenCode أو تعديل نواته.
- دمج جميع العملاء داخل Context واحد.
- إنشاء دردشة حرة بين العملاء.
- السماح للـSupervisor بتعديل كود التطبيقات.
- السماح للمراجع بإصلاح ما راجعه.
- اتخاذ قرارات تجارية أو معمارية حساسة دون المستخدم.
- تنفيذ Merge أو Push أو Production Deployment تلقائيًا.
- بناء Web UI قبل إثبات صحة التشغيل عبر CLI.
- تغيير السياسات أو البروفايلات ذاتيًا.
- تشغيل كل العملاء الموجودين في المنظومة على كل مهمة.

---

## 5. المبادئ الحاكمة

### 5.1 القواعد قبل الاجتهاد

```text
Deterministic Rules
→ Model Classification
→ Owner Escalation عند الشك
```

تستخدم النماذج لفهم الطلب، استخراج البيانات، التلخيص، وتقديم التوصية. أما السماح والمنع، شروط القبول، حدود الصلاحيات، وانتقالات Workflow فيجب أن تنفذها البرمجية والسياسات.

### 5.2 أقل صلاحية ممكنة

```text
Default Deny + Explicit Allowlist
```

لا يحصل أي عميل على صلاحية قراءة أو كتابة أو تنفيذ أو شبكة إلا إذا كانت مصرحًا بها لمهمته.

### 5.3 لا قبول بلا دليل

لا يعتمد وصف العميل لعمله وحده. يجب ربط القرار بأدلة مثل:

- Git Diff.
- الملفات المتغيرة.
- نتائج Build وLint وTests.
- Exit Codes.
- Screenshots عند مراجعة الواجهة.
- نتائج Migration أو Schema Validation عند الحاجة.

### 5.4 فصل التنفيذ عن المراجعة

المنفذ لا يعتمد عمله، والمراجع لا يعدل التنفيذ الذي يراجعه.

### 5.5 مخرجات منظمة فقط

التواصل التشغيلي بين مكونات النظام يتم بواسطة عقود بيانات محددة، وليس محادثات نصية حرة.

### 5.6 التصميم الكامل لا يعني تفعيل كل شيء من البداية

تبقى المعمارية الكاملة موثقة، لكن أول MVP يفعّل أقل عدد من الأدوار والمسارات اللازمة لإثبات النظام.

---

## 6. الهيكل العام الكامل

```text
Majed — Owner Authority
│
└── Tera Control Room
    │
    ├── Supervisor Runtime
    ├── Workflow Engine
    ├── Policy Engine
    ├── Agent Registry
    ├── Done Criteria Registry
    ├── Session Manager
    ├── Evidence Collector
    ├── Decision Queue
    ├── Audit Store
    └── OpenCode Adapter
         │
         ├── Tera Strategic Advisor
         ├── TeraClientEngagementAgent
         ├── ApplicationBlueprintAgent
         ├── TeraAgent
         │    ├── Software Designer Agent
         │    ├── UI Designer Agent
         │    ├── Engineering Agent
         │    ├── QA Agent
         │    ├── Domain Research Agent
         │    └── Domain Expert Agent
         │
         ├── Monitor Agent
         ├── Auditor Agent
         ├── Design Reviewer Agent
         └── TeraSystemEvolutionAgent
```

يوجد مستويان منفصلان للإدارة:

1. **Supervisor:** يدير النظام الكامل، المسارات، الجلسات، المراجعات، القرارات، والسياسات.
2. **TeraAgent:** يدير تنفيذ التطبيقات وعملاء التنفيذ الفرعيين فقط.

---

## 7. حدود السلطات

| الجهة | السلطة الأساسية | لا يحق لها |
|---|---|---|
| Majed | اعتماد القرارات المهمة وتحديد الهدف | إدارة كل جلسة أو تصحيح صغير يدويًا |
| Supervisor | إدارة Workflow والجلسات والسياسات | تعديل التطبيق أو تغيير الأدوار أو قبول مخاطرة حساسة |
| TeraAgent | إدارة تنفيذ تطبيقات العملاء | اتخاذ قرار تجاري أو تجاوز سياسة Supervisor |
| Executor | تعديل الملفات المصرح بها | اعتماد عمله أو تغيير النطاق |
| Reviewer | إصدار Findings وقرار مراجعة | تعديل الملفات أو التواصل الحر مع المنفذ |
| Policy Engine | تطبيق القواعد الثابتة | تفسير المتطلبات أو اختراع استثناءات |

---

## 8. دور المستخدم

دور المستخدم هو:

```text
Owner & Important-Decision Approver
```

### يقوم المستخدم بـ

- إرسال الهدف أو الطلب.
- تقديم المعلومات التي لا يمكن استنتاجها.
- اعتماد تغيير النطاق أو السعر أو المدة أو المعمارية.
- قبول أو رفض المخاطر المهمة.
- اعتماد النشر أو الدمج النهائي وفق السياسة.
- إيقاف المهمة أو تغيير الأولوية عند الحاجة.

### لا يحتاج المستخدم إلى

- اختيار البروفايل المناسب لكل مرحلة.
- نقل التقارير بين العملاء.
- تشغيل المراجعين يدويًا.
- متابعة كل رسالة داخلية.
- اعتماد التصحيحات الصغيرة داخل النطاق.
- تحديد ترتيب العملاء في كل مهمة.

---

## 9. Supervisor Agent

Supervisor هو واجهة النظام التشغيلية والمسؤول عن تنسيق جميع المكونات.

### مسؤولياته

- استقبال الطلب.
- إنشاء Request وTask وSession IDs.
- تصنيف الطلب وفق Routing Rules.
- اختيار العميل الرئيسي من Agent Registry.
- إنشاء Task Contract.
- تحديد Done Criteria قبل التنفيذ.
- إنشاء بيئة Git معزولة عند وجود تعديل ملفات.
- تشغيل جلسة OpenCode بالعميل المطلوب.
- التحقق من Structured Handback.
- جمع الأدلة التقنية.
- تحديد مستوى المراجعة.
- تشغيل المراجع المناسب.
- تطبيق نتائج Policy Engine.
- إدارة دورات التصحيح ضمن الحدود.
- فتح Owner Decision عند الحاجة.
- إغلاق المهمة وحفظ السجل النهائي.

### ما لا يحق له

- تعديل ملفات تطبيق العميل.
- إعادة تعريف دور أي عميل أثناء التشغيل.
- منح صلاحيات غير موجودة في Registry.
- استدعاء منفذي التطبيق مباشرة بدل TeraAgent.
- تجاوز قاعدة مانعة.
- تغيير Scope أو Price أو Architecture رئيسية.
- قبول Critical Finding أو مخاطرة High دون السياسة المناسبة.
- تعديل ملفات منظومة Tera الأساسية دون موافقة المستخدم.

### قاعدة حاكمة

```text
Supervisor يدير الأدوار ولا يعيد تعريفها.
Supervisor يطبق القواعد ولا يستبدلها باجتهاده.
```

---

## 10. TeraAgent

TeraAgent هو السلطة التشغيلية الوحيدة لتنفيذ تطبيقات العملاء.

### مسؤولياته

- قراءة المهمة المعتمدة.
- تحليل نوع التنفيذ.
- تقسيم المهمة إلى Subtasks عند الحاجة.
- تحديد العميل الفرعي المناسب.
- تشغيل Software Designer عندما تحتاج المهمة Technical Specification.
- تشغيل UI Designer لتعديلات الواجهة.
- تشغيل Engineering Agent للـBackend وAPI وDB وBusiness Logic.
- تشغيل QA Agent للاختبارات.
- جمع Handbacks من العملاء الفرعيين.
- التحقق من اكتمال التنفيذ بالنسبة للمهمة.
- إعادة Subtask الناقصة إلى المنفذ.
- تسليم Handback موحد إلى Supervisor.

### ما لا يحق له

- اعتماد المهمة النهائية بدل Supervisor.
- اتخاذ قرار تغيير نطاق أو معمارية رئيسية.
- استدعاء Reviewer مستقل نيابة عن Supervisor.
- الكتابة داخل التطبيق بنفسه إذا كان دوره إداريًا فقط.
- السماح لعميل فرعي بتجاوز نطاق الملفات المحدد.

### قاعدة التنفيذ

```text
أي تعديل داخل تطبيقات العملاء يمر عبر TeraAgent.
Supervisor لا يستدعي UI Designer أو Engineering Agent مباشرة.
```

---

## 11. العملاء الرئيسيون الكاملون

### 11.1 Tera Strategic Advisor

**النطاق:** القرارات الاستراتيجية، البدائل، المخاطر، الأدوات، المعمارية، واعتماد مشاريع مفتوحة المصدر.

**المخرج:** بدائل، تقييم، مخاطر، توصية، وقرار مطلوب من المستخدم عند وجود أثر مهم.

### 11.2 TeraClientEngagementAgent

**النطاق:** اكتشاف العميل، تحليل الطلب، النطاق، فرص القيمة، التسعير، التغيير التجاري، وحزمة التسليم.

**لا يعتمد:** السعر النهائي، الالتزام التجاري، تغيير النطاق، أو المدة الملزمة.

### 11.3 ApplicationBlueprintAgent

**النطاق:** تحويل Handoff مؤكد إلى Blueprint قبل التحضير التفصيلي والتنفيذ.

**المخرج:** حدود التطبيق، المكونات، تدفقات العمل، المخاطر، الأسئلة المفتوحة، وقرارات التقنية المطلوبة.

### 11.4 Monitor Agent

**النطاق:** مطابقة التنفيذ مع المهمة والخطط المعتمدة.

يراجع:

- تنفيذ جميع البنود.
- الانحراف عن الخطة.
- زحف النطاق.
- تغييرات غير معلنة.
- تطابق Handback مع Git Diff.

### 11.5 Auditor Agent

**النطاق:** الحوكمة، التوثيق، الجودة الهندسية، الامتثال، وجاهزية الإغلاق.

يراجع:

- اكتمال السجلات.
- الالتزام بالسياسات.
- جودة التوثيق.
- الفجوات الهندسية المؤثرة.
- شروط الإغلاق.

### 11.6 Design Reviewer Agent

**النطاق:** مراجعة UI/UX مقابل التصميم أو المرجع البصري المعتمد.

يراجع:

- التباعد والألوان والتوكينز.
- Responsive Behavior.
- RTL والمقروئية.
- الحالات والتفاعلات.
- التطابق البصري عبر Screenshots أو Browser Preview.

### 11.7 TeraSystemEvolutionAgent

**النطاق:** مراجعة وتطوير منظومة Tera نفسها، وليس تطبيقات العملاء.

يستدعى عند وجود:

- تداخل أو تضارب أدوار.
- نقص عميل أو بروتوكول.
- تكرار أو تضخم في ملفات المنظومة.
- سياسة تحتاج تعديلًا.
- اقتراح تطوير ذاتي ناتج عن تجربة تشغيلية.

أي تغيير مهم يقترحه يحتاج موافقة المستخدم قبل التنفيذ.

---

## 12. عملاء TeraAgent الفرعيون

| العميل | المسؤولية | تعديل التطبيق | اعتماد العمل |
|---|---|---:|---:|
| Software Designer | إعداد Technical Specification | لا | لا |
| UI Designer | تنفيذ UI وكود الواجهة | نعم ضمن النطاق | لا |
| Engineering Agent | Backend وAPI وDB وBusiness Logic | نعم ضمن النطاق | لا |
| QA Agent | تصميم وتشغيل الاختبارات وإصدار النتائج | لا | لا |
| Domain Research Agent | جمع معلومات خارجية ضمن Brief | لا | لا |
| Domain Expert Agent | تحليل المعرفة وتحويلها إلى متطلبات | لا | لا |

---

## 13. نطاق الـMVP الفعلي

المعمارية الكاملة تبقى كما هي، لكن أول نسخة تشغيلية تفعل مسارًا واحدًا فقط:

```text
Majed
→ Supervisor
→ TeraAgent
→ Executor
→ QA/Reviewer
→ Evidence
→ Decision
→ Revision عند الحاجة
→ Owner Approval عند القرار المهم
```

### الأدوار الفعالة في MVP-1

1. Supervisor.
2. TeraAgent.
3. Executor واحد أو اثنان حسب نوع المهمة.
4. QA/Reviewer واحد.
5. Majed عند Owner Approval Gate.

### العملاء غير المفعّلين في البداية

يبقون مسجلين في Registry لكن بحالة:

```json
{
  "enabled": false,
  "activation_phase": 2
}
```

لا يتم حذفهم أو دمجهم نهائيًا، بل يؤجل تشغيلهم إلى ما بعد نجاح المسار الأساسي.

---

## 14. مكونات النظام البرمجية

### 14.1 Supervisor Runtime

ينفذ دورة المهمة ويربط المكونات ببعضها.

### 14.2 Workflow Engine

يدير الحالات والانتقالات وشروط الدخول والخروج لكل حالة.

### 14.3 Policy Engine

يطبق القواعد الثابتة ويصدر قرارات مثل:

- APPROVED.
- REVISION_REQUIRED.
- BLOCKED.
- OWNER_APPROVAL_REQUIRED.

### 14.4 Agent Registry

مصدر الحقيقة للأدوار، الصلاحيات، الاستدعاء، حدود الملفات، الأوامر، النموذج، والميزانية.

### 14.5 Done Criteria Registry

يحدد شروط انتهاء كل نوع مهمة قبل بدء تنفيذها.

### 14.6 Session Manager

ينشئ الجلسات، يحفظ حالتها، ويستأنفها أو يغلقها.

### 14.7 OpenCode Adapter

يشغل جلسات OpenCode بالبروفايل ومسار المشروع والسياق المحدد.

### 14.8 Evidence Collector

يجمع Git Diff، نتائج الأوامر، التقارير، والملفات المطلوبة للقرار.

### 14.9 Decision Queue

يحفظ القرارات التي تحتاج موافقة المستخدم ويستأنف Workflow بعد الرد.

### 14.10 Audit Store

يحفظ Timeline، السياسات المستخدمة، نسخ البروفايلات، الأدلة، والقرارات.

---

## 15. دورة الطلب الكاملة

### الخطوة 1: استقبال الهدف

مثال:

```text
نفذ شاشة إدارة العملاء حسب النطاق المعتمد.
```

ينشئ النظام:

- Request ID.
- Task ID.
- Session ID.
- Created At.
- Project ID.
- Requester.

### الخطوة 2: التحقق الأولي

يتحقق النظام من:

- وجود المشروع.
- حالة Git.
- وجود Scope أو Task Reference.
- عدم وجود قرار معلق يمنع التنفيذ.
- صلاحية تشغيل العميل المطلوب.
- توفر الميزانية والحدود.

### الخطوة 3: التصنيف

يستخدم النظام القواعد أولًا، ثم النموذج عند الحاجة.

نتيجة التصنيف يجب أن تحتوي على:

```json
{
  "request_type": "APPLICATION_EXECUTION",
  "risk_level": "STANDARD",
  "primary_agent": "tera-agent",
  "confidence": 0.94,
  "requires_owner_input": false
}
```

إذا كان التصنيف غير معروف أو Confidence أقل من الحد المحدد، لا يخمّن النظام، بل ينشئ Clarification أو Owner Decision.

### الخطوة 4: إنشاء Task Contract

قبل تشغيل أي عميل، ينشئ Supervisor عقد مهمة يحدد الهدف والنطاق والأدلة والحدود.

### الخطوة 5: تحديد Done Criteria

يربط النظام نوع المهمة بقائمة شروط إنهاء إلزامية.

### الخطوة 6: إنشاء Worktree

إذا كانت المهمة تعدل ملفات، ينشئ النظام Worktree وفرعًا مستقلًا ويسجل Base Commit.

### الخطوة 7: تشغيل TeraAgent

يرسل Supervisor إلى TeraAgent السياق المصرح به فقط.

### الخطوة 8: تنفيذ العملاء الفرعيين

يدير TeraAgent التنفيذ والاختبارات الداخلية.

### الخطوة 9: استلام Handback

يجب أن يطابق Handback الـSchema المحدد. المخرج غير الصالح يعاد تلقائيًا.

### الخطوة 10: جمع الأدلة

يجمع Evidence Collector الأدلة من Git والأوامر والجلسات.

### الخطوة 11: المراجعة

يختار Supervisor مستوى المراجعة ويشغل المراجع المناسب.

### الخطوة 12: القرار

يطبق Policy Engine القواعد على الأدلة والـFindings.

### الخطوة 13: التصحيح أو التصعيد

- التصحيح الواضح داخل النطاق يعاد تلقائيًا إلى TeraAgent.
- القرار المهم يرفع إلى المستخدم.
- تجاوز الحدود يوقف المهمة.

### الخطوة 14: الإغلاق

لا تغلق المهمة إلا بعد:

- تحقق Done Criteria.
- صدور قرار نهائي.
- حفظ Evidence Bundle.
- تسجيل Timeline النهائي.
- تحديد حالة Worktree والفرع.

---

## 16. Session Router

يحدد Session Router العميل الرئيسي وفق نوع الطلب.

| نوع الطلب | المسار الأساسي |
|---|---|
| رأي أو مقارنة استراتيجية | Tera Strategic Advisor |
| مشروع عميل، Scope، Pricing، Change Request | TeraClientEngagementAgent |
| تحويل Handoff إلى Blueprint | ApplicationBlueprintAgent |
| تحضير أو تنفيذ تطبيق | TeraAgent |
| مطابقة التنفيذ بالخطة | Monitor |
| تدقيق الجودة والحوكمة | Auditor |
| مراجعة واجهة | Design Reviewer |
| تطوير منظومة Tera | TeraSystemEvolutionAgent |

### قاعدة الطلبات متعددة المجالات

لا يتم تشغيل العملاء بالتوازي عشوائيًا. ينشئ النظام Pipeline مرتبًا.

مثال:

```text
طلب مشروع عميل جديد
→ TeraClientEngagementAgent
→ Owner Approval
→ ApplicationBlueprintAgent
→ Blueprint Approval
→ TeraAgent
→ Review
```

---

## 17. Task Contract

كل مهمة يجب أن تملك عقدًا قبل التنفيذ.

```json
{
  "schema_version": "1.0",
  "task_id": "TASK-004",
  "project_id": "APP-CUSTOMERS",
  "task_type": "UI_CHANGE",
  "title": "إنشاء شاشة إدارة العملاء",
  "objective": "تنفيذ الشاشة حسب النطاق والتصميم المعتمد",
  "in_scope": [],
  "out_of_scope": [],
  "references": [],
  "allowed_read_paths": [],
  "allowed_write_paths": [],
  "allowed_commands": [],
  "required_evidence": [],
  "done_criteria_id": "UI_CHANGE_V1",
  "risk_level": "STANDARD",
  "limits": {},
  "owner_approval_rules": []
}
```

أي عمل خارج `in_scope` يسجل كـDeferred Note أو Scope Change ولا ينفذ تلقائيًا.

---

## 18. Definition of Done

لا تبدأ المهمة قبل تحديد شروط انتهائها.

ملف مقترح:

```text
config/done-criteria.json
```

مثال:

```json
{
  "BACKEND_CHANGE_V1": {
    "required": [
      "build_passed",
      "mandatory_tests_passed",
      "no_blocking_findings",
      "git_diff_collected",
      "changed_files_declared",
      "handback_schema_valid"
    ]
  },
  "UI_CHANGE_V1": {
    "required": [
      "build_passed",
      "ui_review_completed",
      "rtl_checked_when_applicable",
      "responsive_states_checked",
      "screenshots_collected",
      "no_blocking_findings"
    ]
  },
  "DATABASE_CHANGE_V1": {
    "required": [
      "migration_reviewed",
      "rollback_plan_available",
      "database_tests_passed",
      "backup_requirement_checked",
      "owner_approval_if_destructive"
    ]
  }
}
```

لا يستطيع المراجع أو Supervisor اعتبار المهمة منتهية إذا كان شرط إلزامي مفقودًا.

---

## 19. Structured Communication

لا يعتمد النظام على النص الحر كعقد بين العملاء.

### 19.1 الحد الأدنى لكل Handback

```json
{
  "schema_version": "1.0",
  "task_id": "TASK-004",
  "agent_id": "engineering-agent",
  "status": "COMPLETED",
  "summary": "",
  "files_changed": [],
  "commands_executed": [],
  "tests_executed": [],
  "evidence_references": [],
  "known_issues": [],
  "scope_deviations": [],
  "recommended_next_action": "REVIEW"
}
```

### 19.2 التحقق

```text
Handback Received
→ JSON Schema Validation
→ Task ID Validation
→ Agent Permission Validation
→ Evidence Reference Validation
```

عند الفشل:

```text
INVALID_HANDBACK
→ Return to Agent
```

لا يحول Supervisor تقريرًا حرًا طويلًا إلى اعتماد رسمي دون Handback صالح.

---

## 20. حالات Workflow

```text
CREATED
→ VALIDATING
→ CLASSIFYING
→ ROUTED
→ CONTRACT_READY
→ PREPARING_WORKSPACE
→ EXECUTING
→ HANDOFF_VALIDATING
→ EVIDENCE_COLLECTING
→ REVIEWING
→ DECIDING
→ APPROVED
  أو REVISION_REQUIRED
  أو OWNER_APPROVAL_REQUIRED
  أو BLOCKED
  أو LIMIT_EXCEEDED
  أو FAILED
→ CLOSED
```

كل حالة تعرف:

- المسؤول الحالي.
- شروط الدخول.
- الإجراءات المسموحة.
- الأدلة المطلوبة.
- شروط الخروج.
- Timeout.
- الانتقالات الممكنة.

لا يسمح بانتقال غير معرف في Workflow Schema.

---

## 21. Agent Registry

Agent Registry هو المصدر المركزي لتعريف العملاء.

```json
{
  "id": "engineering-agent",
  "profile_path": ".opencode/agents/engineering-agent.md",
  "type": "tera-subagent",
  "enabled": true,
  "invoked_by": ["tera-agent"],
  "reports_to": "tera-agent",
  "can_manage_agents": false,
  "can_approve_own_work": false,
  "capabilities": ["backend", "api", "database", "business_logic"],
  "filesystem_policy_id": "ENGINEERING_FS_V1",
  "command_policy_id": "NODE_SAFE_COMMANDS_V1",
  "network_policy_id": "NO_NETWORK",
  "default_limits_id": "STANDARD_EXECUTOR_LIMITS",
  "model_policy_id": "CODING_MODEL_STANDARD"
}
```

يجب أن يحدد Registry:

- من يستطيع استدعاء العميل.
- إلى من يرفع النتيجة.
- البروفايل المستخدم.
- أنواع المهام المسموحة.
- صلاحيات الملفات.
- صلاحيات الأوامر.
- صلاحية الشبكة.
- صلاحيات Git.
- النموذج المسموح.
- الوقت والتوكن والتكلفة.
- عدد المحاولات.
- حالة التفعيل.

---

## 22. نموذج الصلاحيات

مثال لسياسة ملفات وأوامر:

```json
{
  "filesystem": {
    "read": [
      "src/**",
      "tests/**",
      "project-control/**"
    ],
    "write": [
      "src/features/customers/**",
      "tests/customers/**"
    ],
    "deny": [
      ".env*",
      ".git/**",
      "infra/**",
      "secrets/**"
    ]
  },
  "commands": {
    "allow": [
      "npm test",
      "npm run lint",
      "npm run build"
    ],
    "deny_arbitrary_shell": true
  },
  "git": {
    "can_commit": false,
    "can_push": false,
    "can_merge": false,
    "can_rebase": false
  },
  "network": {
    "enabled": false
  }
}
```

يجب تنفيذ الصلاحيات تقنيًا قدر الإمكان، لا الاكتفاء بكتابتها داخل البرومبت.

---

## 23. عزل التنفيذ باستخدام Git Worktree

الخيار الافتراضي لكل مهمة تعدل الملفات:

```text
Git Worktree + Dedicated Task Branch
```

استخدام Branch داخل نفس Workspace يكون Fallback فقط.

### قبل إنشاء Worktree

يتحقق النظام من:

- أن المستودع صحيح.
- أن Base Branch محدد.
- أن Working Tree الأساسية نظيفة.
- عدم وجود Worktree للمهمة نفسها.
- عدم وجود Branch متعارض.
- تسجيل Base Commit.

### البنية المقترحة

```text
repository/
worktrees/
└── TASK-004/
```

### اسم الفرع

```text
task/TASK-004-customer-management-screen
```

### القيود

- لا تعديل مباشر على main أو master.
- لا Push تلقائي.
- لا Merge تلقائي.
- لا حذف Worktree قبل حفظ Evidence Bundle.
- أي تغيير خارج Worktree يعتبر Policy Violation.

---

## 24. Evidence Bundle

لكل مهمة حزمة أدلة موحدة.

```json
{
  "schema_version": "1.0",
  "task": {},
  "session": {},
  "task_contract": {},
  "agent_profile_versions": [],
  "registry_version": "",
  "policy_version": "",
  "base_commit": "",
  "head_commit": "",
  "changed_files": [],
  "git_diff_path": "",
  "commands": [],
  "command_results": [],
  "build_result": {},
  "lint_result": {},
  "test_results": [],
  "screenshots": [],
  "agent_handbacks": [],
  "review_findings": [],
  "policy_evaluation": {},
  "owner_decisions": [],
  "final_decision": {},
  "timeline_path": ""
}
```

### الأدلة الإلزامية حسب المهمة

| نوع المهمة | الأدلة الأساسية |
|---|---|
| Code Change | Diff، Changed Files، Build، Tests، Exit Codes |
| UI Change | ما سبق + Screenshots + UI Review |
| DB Change | Migration Files، Schema Diff، Tests، Rollback Plan |
| Config Change | Before/After، Validation Result، Impact Assessment |
| Documentation | Files Changed، Required Sections Check، Review Result |

نقص دليل إلزامي يمنع الاعتماد.

---

## 25. Policy Engine

Policy Engine ينفذ قواعد قابلة للتفسير والاختبار.

أمثلة:

```text
Build Failed
→ REVISION_REQUIRED

Mandatory Test Failed
→ REVISION_REQUIRED

Handback Schema Invalid
→ RETURN_TO_AGENT

Critical Finding Confirmed
→ BLOCKED أو OWNER_APPROVAL_REQUIRED

Scope Change Detected
→ OWNER_APPROVAL_REQUIRED

Destructive Migration Detected
→ OWNER_APPROVAL_REQUIRED

Changed File Outside Allowlist
→ BLOCKED

Tests Passed + Done Criteria Satisfied + No Blocking Findings
→ APPROVED
```

### ترتيب التقييم

1. Security and Permission Rules.
2. Scope Rules.
3. Evidence Completeness.
4. Build and Test Results.
5. Review Findings.
6. Done Criteria.
7. Budget and Limits.
8. Final Decision Rule.

### الاستثناءات

لا يسمح باستثناء صامت. أي تجاوز لقاعدة قابلة للاستثناء يتطلب:

- Exception ID.
- القاعدة المتجاوزة.
- السبب.
- الأثر.
- صاحب الموافقة.
- مدة صلاحية الاستثناء.

---

## 26. مستويات المراجعة

| المستوى | الاستخدام | المشاركون |
|---|---|---|
| Automated | تحقق Schema، صلاحيات، Build، Tests | النظام فقط |
| Light | تغيير صغير منخفض المخاطر | QA أو Reviewer واحد |
| Standard | أغلب مهام التطبيق | QA + Monitor أو Auditor حسب النوع |
| Critical | Security، Permissions، Data، Migration، Architecture | Monitor + Auditor + متخصص + Owner عند الحاجة |
| UI | تغيير بصري مؤثر | Design Reviewer + الاختبارات المناسبة |

### عوامل تحديد المستوى

- نوع الملفات المعدلة.
- حجم Diff.
- وجود DB Migration.
- وجود Permission أو Authentication Change.
- حساسية البيانات.
- تغيير Architecture.
- فشل سابق.
- مرحلة المشروع.
- عدد المكونات المتأثرة.

---

## 27. استقلال المراجعة

في الجولة الأولى:

- يرى المراجع المهمة والأدلة اللازمة فقط.
- لا يرى رأي مراجع آخر قبل إصدار تقريره.
- لا يتواصل مباشرة مع المنفذ.
- لا يغير الملفات.
- لا يعتمد اقتراحات خارج نطاق المراجعة كمتطلبات إلزامية.

بعد صدور التقارير، يقوم Supervisor بتوحيد النتائج وفق الأدلة والسياسات.

لا يستخدم Debate إلا عند وجود تعارض جوهري لا تحسمه الأدلة، وبحد أقصى جولة واحدة في المراحل اللاحقة. Debate غير مفعل في MVP-1.

---

## 28. نموذج Finding

```json
{
  "finding_id": "FND-014",
  "task_id": "TASK-004",
  "reviewer_id": "auditor",
  "severity": "HIGH",
  "category": "SECURITY",
  "title": "صلاحية وصول أوسع من نطاق المهمة",
  "evidence": [],
  "impact": "",
  "required_action": "",
  "affected_files": [],
  "blocking": true,
  "confidence": 0.97
}
```

### مستويات الخطورة

| المستوى | التعريف | الأثر الافتراضي |
|---|---|---|
| Critical | خطر أمني، فقد بيانات، أو كسر رئيسي | يمنع الاعتماد |
| High | مشكلة مؤثرة يجب إصلاحها غالبًا | تصحيح إلزامي |
| Medium | مشكلة مهمة تعتمد على السياق | قرار Policy/Supervisor |
| Low | ملاحظة بسيطة | لا توقف المهمة |
| Suggestion | تحسين اختياري | لا يتحول إلى Required Action تلقائيًا |

لا يقبل Finding بلا دليل أو مرجع واضح.

---

## 29. دورات التصحيح

عندما يكون التصحيح واضحًا وداخل النطاق:

```text
Supervisor
→ Required Actions إلى TeraAgent
→ TeraAgent يعيد التكليف للمنفذ
→ QA
→ Evidence Collection
→ Re-Review
```

### قواعد التصحيح

- لا يعاد إرسال التقرير الكامل دون حاجة.
- يرسل فقط Findings المعتمدة وRequired Actions.
- يحتفظ النظام بتاريخ الجولات.
- لا يغير Task Contract أثناء التصحيح.
- أي تغيير مطلوب خارج العقد يتحول إلى Owner Approval أو مهمة جديدة.

---

## 30. الحدود القاسية

ملف مقترح:

```text
config/runtime-limits.json
```

إعدادات أولية:

```json
{
  "max_revision_cycles": 2,
  "max_review_rounds": 2,
  "max_active_agents": 3,
  "max_session_minutes": 45,
  "max_command_retries": 2,
  "max_debate_rounds": 0,
  "max_task_tokens": 200000,
  "max_task_cost_usd": 10
}
```

القيم النهائية قابلة للتعديل حسب النموذج والتجربة، لكن يجب أن تكون محددة قبل التشغيل.

عند التجاوز:

```text
LIMIT_EXCEEDED
→ Stop New Actions
→ Preserve Current State
→ Collect Available Evidence
→ OWNER_APPROVAL_REQUIRED
```

لا يقرر النموذج الاستمرار بعد تجاوز الحد.

---

## 31. Owner Approval Gates

تحتاج موافقة المستخدم الحالات التالية:

- تغيير نطاق معتمد.
- اعتماد سعر أو خصم أو التزام تجاري.
- تغيير مدة أو موعد ملزم.
- تغيير Architecture رئيسية.
- اختيار Stack نهائي بين بدائل مؤثرة.
- حذف بيانات أو Destructive Migration.
- تغيير نموذج الصلاحيات أو الأمان.
- قبول Critical أو High Risk بدل إصلاحها.
- Push أو Merge أو Production Deployment عند اشتراط السياسة.
- تجاوز الوقت أو التوكن أو التكلفة.
- تجاوز عدد دورات التصحيح.
- تعديل ملفات أو سياسات منظومة Tera الأساسية.
- تعارض مهم لا تحسمه الأدلة.
- تنفيذ عمل خارج Task Contract.

### نموذج القرار

```json
{
  "event": "OWNER_APPROVAL_REQUIRED",
  "decision_id": "DEC-014",
  "task_id": "TASK-004",
  "title": "تغيير بنية قاعدة البيانات",
  "reason": "المتطلب لا يمكن تنفيذه بأمان ضمن البنية الحالية",
  "evidence_references": [],
  "options": [
    {
      "id": "A",
      "summary": "تعديل البنية الآن",
      "impact": "Migration وزيادة مدة التنفيذ",
      "risk": "MEDIUM"
    },
    {
      "id": "B",
      "summary": "الإبقاء على البنية الحالية",
      "impact": "تنفيذ أسرع مع قيد مستقبلي",
      "risk": "LOW"
    }
  ],
  "recommendation": "A"
}
```

يرد المستخدم بالخيار أو قرار واضح، ويسجل النظام الرد ثم يستأنف المهمة.

---

## 32. القرارات التي لا تحتاج موافقة المستخدم

- اختيار العميل وفق Routing Rules.
- اختيار عميل TeraAgent الفرعي.
- إنشاء Worktree وفرع المهمة.
- تشغيل Build وLint وTests المسموحة.
- إعادة Handback غير صالح.
- إعادة التصحيح الواضح داخل النطاق.
- تشغيل مستوى المراجعة المحدد بالسياسة.
- رفض Suggestion غير مؤثر.
- إغلاق جلسة عميل بعد حفظ مخرجاتها.
- إيقاف مهمة عند مخالفة قاعدة مانعة.
- تسجيل Deferred Note خارج النطاق.

---

## 33. التكامل مع OpenCode

لا يتم تعديل OpenCode داخليًا في البداية.

```text
Tera Control Room
└── OpenCode Adapter
    ├── Start Session
    ├── Select Agent Profile
    ├── Set Project/Worktree Path
    ├── Pass Task Context
    ├── Apply Permission Configuration
    ├── Observe Output
    ├── Receive Structured Handback
    └── Close or Suspend Session
```

### متطلبات Adapter

- تشغيل جلسة ببروفايل محدد.
- تحديد مجلد العمل.
- تمرير Task Contract.
- تمرير الملفات المرجعية دون كشف ملفات إضافية.
- الحصول على مخرجات قابلة للقراءة الآلية.
- التقاط الأخطاء وExit Status.
- إيقاف الجلسة عند Timeout.
- دعم Resume عند الحالات المسموحة.

كل عميل رئيسي يعمل داخل جلسة مستقلة. لا تجمع جميع العملاء في Context واحد.

---

## 34. CLI في النسخة الأولى

أمثلة أوامر:

```bash
tera-room start --project APP-001 --request "نفذ شاشة إدارة العملاء"
tera-room status TASK-004
tera-room sessions TASK-004
tera-room evidence TASK-004
tera-room findings TASK-004
tera-room decisions
tera-room approve DEC-014 --option A
tera-room reject DEC-014 --reason "حافظ على البنية الحالية"
tera-room stop TASK-004
tera-room resume TASK-004
tera-room cleanup TASK-004
```

### المخرجات الافتراضية للمستخدم

تعرض CLI:

- حالة المهمة.
- المرحلة الحالية.
- العميل النشط.
- نسبة اكتمال شروط Done.
- آخر نتيجة Build/Tests.
- Findings المانعة.
- القرار المطلوب إن وجد.

لا تعرض جميع المحادثات الداخلية افتراضيًا، لكن تسمح بفتح السجل عند الحاجة.

---

## 35. التخزين المقترح

```text
tera-control-room/
├── config/
│   ├── agents-registry.json
│   ├── routing-rules.json
│   ├── policies.json
│   ├── done-criteria.json
│   ├── runtime-limits.json
│   └── schemas/
│       ├── task-contract.schema.json
│       ├── handback.schema.json
│       ├── finding.schema.json
│       └── decision.schema.json
│
├── sessions/
│   └── SESSION-001/
│       ├── session.json
│       ├── timeline.jsonl
│       ├── state.json
│       └── logs/
│
├── tasks/
│   └── TASK-004/
│       ├── task.json
│       ├── task-contract.json
│       ├── evidence.json
│       ├── git-diff.patch
│       ├── commands.jsonl
│       ├── handbacks/
│       ├── reviews/
│       ├── decisions/
│       └── final-decision.md
│
└── worktrees/
    └── TASK-004/
```

يمكن حفظ نسخة مختصرة من النتيجة داخل `project-control/` في مشروع التطبيق، مع إبقاء سجلات التشغيل التفصيلية داخل Control Room.

---

## 36. سجل الأحداث والمراجعة

كل إجراء مؤثر يسجل في `timeline.jsonl`.

مثال:

```json
{
  "timestamp": "2026-07-15T12:30:00Z",
  "event": "AGENT_SESSION_STARTED",
  "task_id": "TASK-004",
  "session_id": "SESSION-009",
  "actor": "supervisor",
  "target": "tera-agent",
  "policy_version": "POL-1.0",
  "details": {}
}
```

يجب تسجيل:

- إنشاء المهمة.
- التصنيف والتوجيه.
- نسخ السياسات والبروفايلات المستخدمة.
- فتح وإغلاق الجلسات.
- الأوامر وExit Codes.
- تغييرات الحالة.
- Findings.
- قرارات Policy Engine.
- موافقات المستخدم.
- حالات الفشل والاستئناف.

---

## 37. معالجة الفشل

### 37.1 فشل تشغيل العميل

```text
Retry ضمن max_command_retries
→ ثم FAILED أو OWNER_APPROVAL_REQUIRED حسب السبب
```

### 37.2 خروج غير منظم

- حفظ آخر Output.
- تعليم الجلسة `ABNORMAL_EXIT`.
- عدم اعتبار المهمة مكتملة.
- محاولة استخراج Handback فقط إذا كان صالحًا.

### 37.3 فشل Build أو Tests

ينتقل إلى `REVISION_REQUIRED` دون اجتهاد.

### 37.4 تعطل Supervisor

عند إعادة التشغيل يقرأ `state.json` وTimeline ويستأنف من آخر حالة آمنة، ولا يعيد تنفيذ أمر مؤثر قبل التحقق من نتيجته السابقة.

### 37.5 Worktree غير صالح

توقف المهمة ويمنع أي تنفيذ حتى إصلاح بيئة Git.

### 37.6 Evidence ناقص

لا يعتمد القرار. يعاد جمع الدليل أو يرفع نقص غير قابل للحل للمستخدم.

---

## 38. الأمان

الحد الأدنى المطلوب:

- منع قراءة `.env` وملفات الأسرار.
- عدم تمرير مفاتيح API داخل Task Context.
- إخفاء القيم الحساسة من Logs.
- تعطيل الشبكة افتراضيًا.
- Allowlist للأوامر.
- منع Arbitrary Shell للمهام التي لا تحتاجه.
- عدم السماح للعميل بتعديل Registry أو Policies.
- حماية فرع main/master.
- توقيع أو Hash للملفات المهمة في Evidence Bundle.
- تسجيل أي Policy Violation وإيقاف المهمة حسب الخطورة.

---

## 39. ملفات المواصفات المطلوبة قبل الكود

يجب إنشاء الملفات التالية واعتمادها:

```text
01_SUPERVISOR_AGENT_SPEC.md
02_AGENT_REGISTRY_SPEC.md
03_WORKFLOW_STATE_MACHINE.md
04_ROUTING_RULES_SPEC.md
05_TASK_CONTRACT_SPEC.md
06_DONE_CRITERIA_SPEC.md
07_STRUCTURED_HANDOFF_PROTOCOL.md
08_POLICY_ENGINE_RULES.md
09_PERMISSION_MODEL.md
10_GIT_WORKTREE_PROTOCOL.md
11_EVIDENCE_BUNDLE_SPEC.md
12_REVIEW_PROTOCOL.md
13_OWNER_APPROVAL_RULES.md
14_RUNTIME_LIMITS_SPEC.md
15_OPENCODE_ADAPTER_SPEC.md
16_STORAGE_AND_RECOVERY_SPEC.md
17_CLI_COMMAND_SPEC.md
```

لا يبدأ بناء النظام الكامل قبل حسم هذه العقود. يمكن بناء Prototype صغير بالتوازي لاختبار قابلية التكامل مع OpenCode فقط.

---

## 40. مراحل التنفيذ

### المرحلة 0: إثبات التكامل

الهدف: التأكد أن التطبيق الخارجي يستطيع تشغيل جلسة OpenCode، تمرير Context، واستلام مخرج منظم.

المخرجات:

- Adapter Prototype.
- جلسة عميل واحدة.
- Handback JSON صالح.
- تسجيل Exit Status.

### المرحلة 1: Core Workflow

- إنشاء Task وSession.
- State Machine.
- Agent Registry ثابت.
- Routing محدود لمسار تنفيذ واحد.
- Task Contract.
- Schema Validation.

### المرحلة 2: Execution Isolation

- إنشاء Worktree.
- تطبيق صلاحيات الملفات والأوامر.
- تشغيل TeraAgent وExecutor.
- تسجيل Base وHead Commit.

### المرحلة 3: Evidence and Done Criteria

- جمع Git Diff.
- جمع Build/Tests/Exit Codes.
- Done Criteria Registry.
- منع الاعتماد عند نقص الأدلة.

### المرحلة 4: Review and Revision

- تشغيل QA/Reviewer واحد.
- Findings Schema.
- Policy Decision.
- دورتا تصحيح بحد أقصى.

### المرحلة 5: Owner Decision Queue

- عرض القرار.
- تسجيل الموافقة أو الرفض.
- استئناف Workflow.

### المرحلة 6: توسيع العملاء والمسارات

بعد نجاح MVP-1:

- Monitor.
- Auditor.
- Design Reviewer.
- Strategic Advisor.
- TCEA.
- Blueprint Agent.
- TeraSystemEvolutionAgent.

### المرحلة 7: Web UI

يبنى فقط بعد استقرار CLI والبروتوكولات.

### المرحلة 8: تكامل أعمق اختياري

يبحث تعديل OpenCode أو إنشاء Fork فقط إذا أثبت Adapter الخارجي وجود قيود حقيقية لا يمكن تجاوزها.

---

## 41. نطاق MVP-1

### يشمل

1. مشروع Git محلي واحد لكل مهمة.
2. CLI.
3. Supervisor Runtime محدود.
4. Workflow لمسار Application Execution فقط.
5. Agent Registry ثابت.
6. TeraAgent.
7. Executor واحد على الأقل.
8. QA/Reviewer واحد.
9. Task Contract.
10. Structured Handbacks.
11. Done Criteria.
12. Git Worktree.
13. Evidence Bundle أساسي.
14. Policy Engine بقواعد أساسية.
15. حد أقصى لدورتي تصحيح.
16. Owner Approval Queue.
17. تخزين محلي وسجل Timeline.

### لا يشمل

- Web UI.
- تعدد المستخدمين.
- تشغيل موزع على عدة أجهزة.
- Debate.
- كل العملاء الرئيسيين.
- Auto Merge أو Auto Push.
- Production Deployment.
- Dynamic Model Routing معقد.
- تعلم ذاتي أو تعديل تلقائي للسياسات.
- قاعدة بيانات مركزية؛ يمكن البدء بملفات JSON/JSONL محلية.

---

## 42. سيناريو الاختبار الأساسي للـMVP

### المهمة

تنفيذ تعديل صغير ومحدد داخل تطبيق تجريبي.

### التدفق المتوقع

1. يرسل Majed الهدف.
2. ينشئ Supervisor Task Contract.
3. ينشأ Worktree.
4. يشغل TeraAgent.
5. يكلف TeraAgent Executor.
6. ينفذ العميل التعديل.
7. يعيد Handback صالحًا.
8. تجمع نتائج Git وBuild وTests.
9. يعمل QA/Reviewer.
10. إذا توجد مشكلة، تعاد للتصحيح.
11. بعد النجاح، يصدر APPROVED.
12. تحفظ الحزمة والسجل.
13. يبقى Merge قرارًا منفصلًا.

### حالات يجب اختبارها

- نجاح من أول جولة.
- Build Failed.
- Test Failed.
- Handback غير صالح.
- تعديل ملف خارج Allowlist.
- Scope Change مكتشف.
- تجاوز عدد التصحيحات.
- توقف العميل قبل التسليم.
- نقص Evidence.
- قرار يحتاج موافقة المستخدم.

---

## 43. معايير قبول MVP-1

يعتبر MVP-1 ناجحًا إذا استطاع:

1. استقبال طلب واحد عبر CLI.
2. إنشاء Task Contract صالح.
3. تشغيل TeraAgent دون فتح يدوي للبروفايل.
4. تشغيل Executor داخل Worktree مستقل.
5. منع الكتابة خارج المسار المسموح.
6. استلام Handback منظم والتحقق منه.
7. جمع Git Diff وBuild وTests وExit Codes.
8. تشغيل Reviewer مستقل.
9. إدارة تصحيح واحد على الأقل تلقائيًا.
10. منع الإغلاق عند فشل Done Criteria.
11. تصعيد قرار مهم بصيغة منظمة.
12. حفظ Timeline يفسر جميع الانتقالات.
13. إعادة تشغيل النظام واستئناف مهمة غير مكتملة.
14. عدم تنفيذ Push أو Merge تلقائي.
15. عدم تجاوز أي عميل لصلاحياته.

---

## 44. مؤشرات القياس

| المؤشر | الهدف الأولي |
|---|---:|
| دقة Routing في المسارات المفعلة | 95% أو أكثر |
| Handbacks الصالحة من أول محاولة | 90% أو أكثر |
| القرارات القابلة لإعادة الإنتاج من الأدلة | 100% |
| القرارات المهمة المصعّدة بشكل صحيح | 100% |
| تجاوز الصلاحيات غير المكتشف | صفر |
| متوسط دورات التصحيح | أقل من دورتين |
| المهام التي تحتاج تدخلًا تشغيليًا يدويًا | أقل من 20% |
| الملاحظات الكاذبة المانعة | أقل من 10% |
| فقدان سجل أو Evidence | صفر |

---

## 45. المخاطر والمعالجة

### 45.1 تضخم النظام قبل إثباته

**المعالجة:** مسار واحد وأربعة أدوار فعالة في MVP-1.

### 45.2 Supervisor يتخذ قرارًا غير صحيح

**المعالجة:** قواعد ثابتة، Confidence Threshold، وتصعيد عند عدم اليقين.

### 45.3 تداخل الأدوار

**المعالجة:** Registry واضح ومنع الاستدعاء غير المصرح.

### 45.4 تقارير تبدو صحيحة دون تنفيذ فعلي

**المعالجة:** Evidence Bundle وربط Handback بـGit ونتائج الأوامر.

### 45.5 مراجعات متكررة بلا نهاية

**المعالجة:** حدود قاسية وجولات محددة.

### 45.6 فساد المستودع

**المعالجة:** Worktree، Base Commit، ومنع التعديل على الفرع الرئيسي.

### 45.7 فقدان السياق

**المعالجة:** Task Contract، Structured Handback، Session State، وTimeline.

### 45.8 اعتماد زائد على البرومبت

**المعالجة:** تنفيذ السياسات والصلاحيات في الكود والملفات المنظمة.

### 45.9 قيود OpenCode غير معروفة

**المعالجة:** مرحلة 0 لإثبات Adapter قبل استكمال البناء.

### 45.10 تكلفة تشغيل مرتفعة

**المعالجة:** حدود توكن وتكلفة، عدم تشغيل مراجعين غير لازمين، وتخفيض عدد العملاء النشطين.

---

## 46. قرارات التصميم النهائية

1. بناء Tera Control Room كتطبيق خارجي فوق OpenCode.
2. عدم إنشاء Fork في البداية.
3. Supervisor منسق محكوم بالقواعد، وليس عقلًا عامًا.
4. TeraAgent يبقى السلطة الوحيدة لتنفيذ تطبيقات العملاء.
5. عدم السماح لـSupervisor باستدعاء منفذي التطبيق مباشرة.
6. عدم السماح للمراجع بالتنفيذ.
7. اعتماد Structured Handbacks وJSON Schema Validation.
8. تعريف Done Criteria قبل كل مهمة.
9. اعتبار Evidence Bundle شرطًا للقرار.
10. استخدام Git Worktree كخيار افتراضي.
11. فرض Default Deny وExplicit Allowlist.
12. وضع حدود فعلية للوقت والتوكن والعملاء والجولات.
13. تشغيل أربعة أدوار فقط في MVP-1.
14. إبقاء بقية المعمارية والعملاء دون تفعيل إلى مراحل لاحقة.
15. البدء بـCLI والتخزين المحلي.
16. تأجيل Web UI وDebate وAuto Merge.
17. تصعيد القرارات المهمة فقط إلى Majed.
18. عدم الانتقال للبناء الكامل قبل نجاح Prototype التكامل واختبارات MVP.

---

## 47. الخطوة التنفيذية التالية

الخطوة الصحيحة بعد اعتماد هذه الوثيقة هي:

1. إنشاء ملفات المواصفات السبعة عشر المذكورة في القسم 39.
2. بناء Prototype صغير لـOpenCode Adapter.
3. اختبار تشغيل عميل واحد واستلام Handback JSON.
4. تثبيت حدود ما يمكن وما لا يمكن التحكم به عبر OpenCode.
5. بناء Core Workflow الخاص بـMVP-1 فقط.
6. تنفيذ سيناريوهات الفشل والنجاح قبل توسيع الأدوار.

لا يبدأ تطوير Web UI، ولا تفعيل جميع العملاء، ولا تعديل OpenCode قبل نجاح هذه المراحل.

---

## 48. الخلاصة

Tera Control Room هو طبقة تشغيل وحوكمة تحول منظومة Tera من مجموعة عملاء يتم تشغيلهم يدويًا إلى نظام منضبط يدير الطلب من بدايته إلى قراره النهائي.

التدفق المستهدف:

```text
Majed يرسل الهدف
→ Supervisor ينشئ عقد المهمة ويطبق القواعد
→ TeraAgent يدير التنفيذ داخل Worktree معزول
→ النظام يجمع الأدلة
→ مراجع مستقل يصدر Findings منظمة
→ Policy Engine يقرر القبول أو التصحيح أو التصعيد
→ Majed يتدخل فقط في القرار المهم
→ يحفظ النظام السجل والأدلة بصورة قابلة للمراجعة
```

النجاح لا يعتمد على كثرة العملاء، بل على صرامة العقود، صحة الأدلة، وضوح الصلاحيات، وقدرة النظام على إنهاء مسار واحد كامل دون إدارة يدوية.
