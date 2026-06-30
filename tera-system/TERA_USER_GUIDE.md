# دليل التعامل مع Tera

## من فكرة المشروع إلى التنفيذ المنضبط

---

> **Target audience:** Human users of the Tera system.
> **Purpose:** Practical prompts and guidance for interacting with Tera effectively.
> **Note:** This guide focuses on usage, not system internals. For Tera's identity and rules, see `tera-system/TeraAgent.md`.

---

## 1. Role Summary

**Your role:** Decision owner. You explain the idea, set constraints, approve/reject decisions, monitor for bloat, and approve phase transitions.

**Tera's role:** Project orchestrator. Tera understands the project, prepares files, selects sub-agents, prevents bloat, splits execution into batches, reviews outputs, and reports to you.

---

## 2. Starting a New Project

Use a clear prompt:

```text
سنبدأ مشروعًا جديدًا باستخدام منظومة Tera.

المطلوب الآن مرحلة التحضير فقط، وليس التنفيذ البرمجي.

فكرة المشروع:
[اكتب فكرة المشروع هنا]

المعلومات التقنية المتوفرة:
- نوع التطبيق:
- البيئة:
- التقنية المفضلة إن وجدت:
- قاعدة البيانات إن وجدت:
- المستخدمون المتوقعون:
- هل يوجد تكاملات؟
- هل يوجد تصميم أو ألوان أو CSS؟
- هل المشروع MVP أم نظام كبير؟

القيود:
- لا تبدأ بكتابة كود.
- لا تنشئ كل الملفات.
- لا تولّد كل العملاء الفرعيين.
- لا تعدّل أي ملف داخل tera-system.
- ضع ملفات التحضير التحليلية داخل `project-preparation/`، وملفات التحكم والخطط والمهام داخل `project-control/`.
- إذا احتجت عملاء فرعيين، ولّدهم فقط داخل generated-agents/opencode.

المطلوب منك الآن:
1. ابدأ أولًا بمرحلة Project Intake.
2. أنشئ أو حدّث:
   - project-inputs/01_APPLICATION_IDEA.md
   - project-inputs/02_TECHNICAL_CONTEXT.md
3. إذا كانت المعلومات ناقصة، اسألني ثم أكمل ملفات project-inputs.
4. بعد اكتمال الحد الأدنى من الـ intake فقط:
   - أنشئ project-preparation/00_PROJECT_INPUTS.md
   - أنشئ project-preparation/TERA_PROJECT_DECISION.md
5. ثم حدّد الملفات التحضيرية المطلوبة فقط.
6. حدّد الملفات غير المطلوبة الآن مع السبب.
7. حدّد العملاء الفرعيين المطلوبين مبدئيًا.
8. لا تنشئ باقي الملفات ولا العملاء بعد، فقط اكتب قرارك وانتظر موافقتي.
```

### Quick start (minimal)

```text
أريد بدء مشروع جديد.

فكرة التطبيق:
[وصف بسيط]

المعلومات التقنية المتوفرة:
[ما تعرفه، أو قل: غير محددة]

ابدأ فقط بمرحلة Project Intake.
إذا كانت المعلومات ناقصة، اسألني ثم أنشئ ملفات project-inputs.
```

---

## 3. Starting an External Client Project

```text
سنبدأ مشروع عميل خارجي باستخدام منظومة Tera.

المطلوب الآن:
- مرحلة Client Discovery
- تجهيز ملفات العميل
- تجهيز حزمة اعتماد العميل
- لا تبدأ تنفيذًا برمجيًا

بيانات العميل المتوفرة:
- اسم العميل:
- نوع العميل: فرد / شركة / مؤسسة
- مجال عمله:

جهات التواصل المتوفرة:
- الاسم:
- الدور:
- الهاتف إن وجد:
- البريد إن وجد:
- هل يملك صلاحية اعتماد؟ نعم / لا / غير معروف

فكرة التطبيق:
[فكرة العميل]

المعلومات التقنية المتوفرة:
[ما تعرفه، أو قل: غير محددة]

المراجع أو الذوق:
- شعار أو ألوان:
- مواقع أو تطبيقات تعجب العميل:
- أمثلة لا تعجبه:

قيود مهمة:
- الوثائق الموجهة للعميل بالعربية.
- كل عميل له مجلد داخل clients/.
- كل تطبيق له مجلد داخل applications/.
- حزمة اعتماد العميل إلزامية قبل التنفيذ.
- لا تبدأ Build Mode قبل اعتماد النطاق وحزمة العميل.

المطلوب من Tera الآن:
1. اسألني فقط عن المعلومات الناقصة الضرورية.
2. صغ الأسئلة بطريقة يمكنني إرسالها للعميل.
3. أنشئ أو حدّث ملفات العميل داخل clients/.
4. أنشئ أو حدّث project-inputs/ حسب الحاجة.
5. جهز Client Approval Package فقط بعد اكتمال الحد الأدنى من المعلومات.
6. انتظر موافقتي قبل أي تنفيذ برمجي.
```

---

## 4. Resuming an Existing Project

```text
المشروع قائم ومستمر.

راجع الملفات التالية:
- project-control/TASK_REGISTRY.md
- project-control/DECISIONS_LOG.md
- project-control/PROJECT_ACTIVITY_LOG.md
- project-control/ISSUES_AND_GAPS.md
- project-preparation/TERA_PROJECT_DECISION.md
- project-preparation/09_IMPLEMENTATION_PLAN.md

المطلوب:
1. أخبرني بوضع المشروع الحالي.
2. حدد آخر مهمة مكتملة.
3. حدد آخر قرار معتمد.
4. حدد أي مهام مفتوحة أو مؤجلة أو محظورة.
5. اقترح الخطوة التالية فقط.

لا تبدأ أي تنفيذ.
لا تولد عملاء جدد.
لا تنقل أي عميل إلى .opencode/agents/.
لا تعدل ملفات التطبيق.

انتظر مني استئنافًا صريحًا قبل أي تنفيذ.
```

---

## 5. Sensitive or Large Projects

```text
هذا مشروع حساس ويحتاج دقة عالية.
تعامل معه كـ High-Risk Project.

المطلوب:
- لا تفترض قرارات غير مذكورة.
- وثّق كل افتراض.
- اسأل عند وجود غموض مؤثر.
- لا تختصر التحليل على حساب الدقة.
- لا تبدأ التنفيذ قبل موافقة صريحة.
- راجع النطاق والمخاطر قبل كل مرحلة.
- أي قرار متعلق بالأمان أو البيانات أو الصلاحيات يجب عرضه علي أولًا.
```

For large projects, add:

```text
هذا مشروع متوسط/كبير، لذلك:
- لا تعتبره MVP صغيرًا.
- لا تطبق التقليل بشكل يضر التحليل.
- قسّم المشروع إلى مراحل.
- اقترح الملفات والعملاء حسب الحاجة الفعلية.
- وضّح ما سيتم تنفيذه الآن وما يؤجل.
```

---

## 6. What to Monitor in Tera's Reports

- **Did Tera modify `tera-system/`?** Not allowed during normal project execution.
- **Did Tera create too many files?** Check for unnecessary, mergeable, or phase-3 files.
- **Did Tera generate too many sub-agents?** Agents should match the current phase only.
- **Did Tera start code without approval?** This is unacceptable — stop immediately.
- **Did Tera invent random design?** UI must follow the approved `28_UI_UX_GUIDELINES.md`.
- **Did Tera split execution into small batches?** No full-app builds in one command.

---

## 7. If Tera Misbehaves

### Over-generating files

```text
أوقف التوسع.

المطلوب الآن Reduction Pass فقط.
راجع ما أنشأته وحدد:
1. ما الملفات التي يمكن دمجها؟
2. ما الملفات التي يمكن تأجيلها؟
3. ما الملفات غير الضرورية؟
4. ما القرار النهائي الأقل حجمًا؟

لا تنشئ ملفات جديدة.
لا تبدأ تنفيذًا.
```

### Proposing too many screens

```text
عدد الشاشات المقترح كبير بالنسبة لحجم المشروع.

طبّق MVP Anti-Bloat Rule:
- ادمج القائمة والإضافة والتعديل والتفاصيل إن أمكن.
- لا تنشئ شاشة لكل إجراء.
- أعطني العدد الأولي والعدد النهائي بعد التقليل.
- لا تعدّل ملفات أخرى إلا ملفات UI المعنية.
```

### Generating too many agents

```text
راجع العملاء المولدين.

المطلوب:
- لا تولّد عملاء جدد.
- حدد من منهم مطلوب للمرحلة الحالية فقط.
- حدد من يجب تأجيله.
- لا تنقل أي عميل إلى .opencode/agents الآن.
- حدّث Manifest فقط إذا لزم.
```

### Started code too early

```text
توقف فورًا.
لا نبدأ التنفيذ البرمجي قبل اعتماد المرحلة الحالية.

المطلوب:
1. أوقف أي إنشاء ملفات تطبيق.
2. أعطني تقريرًا بما تم تغييره.
3. وضح لماذا تجاوزت المرحلة.
4. لا تتابع إلا بعد موافقتي.
```

### Deviated from approved design

```text
توقف عن تنفيذ الواجهة.
راجع project-preparation/28_UI_UX_GUIDELINES.md.

المطلوب:
- لا تضف ألوانًا أو components جديدة.
- لا تستخدم مكتبة UI غير معتمدة.
- لا تغيّر spacing أو visual pattern بدون موافقة.
- صحح الانحرافات فقط.
```

---

## 8. Project-Specific Rules

For custom rules per project, use:

```text
project-preparation/PROJECT_RULES.md
```

If you have general rules for all Tera projects, they belong in `tera-system/` instead.

| Rule type | Location |
|---|---|
| Single-project rule | `project-preparation/PROJECT_RULES.md` |
| System-wide rule | `tera-system/` |
| Design/style rule | `project-preparation/28_UI_UX_GUIDELINES.md` and raw sources in `project-preparation/design-source/` |
| Sub-agent rule | Agent file + Manifest |

### Prompt to create rules

```text
أريد إضافة قواعد خاصة بهذا المشروع.

المطلوب:
1. أنشئ أو حدّث: project-preparation/PROJECT_RULES.md
2. لا تعدّل tera-system.
3. لا تبدأ تنفيذًا.
4. وثق القواعد التالية كقواعد ملزمة لهذا المشروع:
[اكتب القواعد هنا]
5. بعد التحديث، أخبرني كيف ستؤثر هذه القواعد على التحليل والتنفيذ.
```

---

## 9. Phase Transition Prompts

### Approve analysis → move to architecture

```text
أعتمد مرحلة التحليل.
انتقل فقط إلى Technical Architecture.

المطلوب:
1. لا تبدأ كود.
2. لا تنشئ ملفات تطبيق.
3. لا تولّد EngineeringAgent.
4. أنشئ فقط: project-preparation/08_TECHNICAL_ARCHITECTURE.md

يجب أن يحتوي الملف على:
- التقنية المقترحة.
- قاعدة البيانات.
- سبب الاختيار.
- ما لم يتم اختياره ولماذا.
- بنية بسيطة غير متضخمة.
- ما هو خارج المعمارية الآن.
```

### Approve architecture → move to planning

```text
أعتمد ملف: project-preparation/08_TECHNICAL_ARCHITECTURE.md
انتقل فقط إلى Implementation Planning.

المطلوب:
1. لا تبدأ كود.
2. لا تنشئ ملفات تطبيق.
3. لا تولّد EngineeringAgent الآن.
4. أنشئ فقط: project-preparation/09_IMPLEMENTATION_PLAN.md
```

### Approve plan → move to testing/acceptance

```text
أعتمد خطة التنفيذ.
انتقل فقط إلى Testing and Acceptance Preparation.

المطلوب:
1. لا تبدأ كود.
2. لا تولّد EngineeringAgent.
3. لا تولّد QAAgent إلا إذا كان ضروريًا.
4. أنشئ فقط: project-preparation/10_TESTING_AND_ACCEPTANCE.md
```

### Approve design direction

```text
قبل تنفيذ الواجهة، يجب حسم مصدر التصميم.

قرار التصميم:
[ INTERNAL_TERA_KIT / getdesign.md DESIGN.md / FIGMA_DESIGN_FILE / صور أو CSS / موقع مرجعي / ألوان وهوية محددة ]

المطلوب:
1. لا تبدأ كود.
2. لا تولّد EngineeringAgent.
3. أنشئ أو حدّث فقط: project-preparation/28_UI_UX_GUIDELINES.md
```

#### Official Figma design source

إذا قلت إن Figma هو مصدر التصميم الرسمي، يجب على Tera ألا يتوقع منك تذكر التفاصيل المطلوبة. يجب أن يطلب منك تلقائيًا:

```text
- رابط Figma أو الملف أو الإطارات المصدرة.
- الصفحات/Frames المعتمدة فقط.
- Frames الممنوع استخدامها.
- درجة الالتزام: exact / close match / guided adaptation.
- اتجاه الواجهة: RTL / LTR / الاثنين.
- العناصر المطلوب استخراجها: colors, typography, spacing, layout, components, states, responsive assumptions, RTL/LTR.
- الحالات الناقصة التي يجب فحصها: empty/loading/error/disabled/mobile/tablet.
- القيود: no API, no MCP, no plugin, no direct EngineeringAgent implementation from Figma.
- أي هوية أو ألوان مشروع يجب أن تتجاوز Figma.
```

### Start controlled implementation

```text
أعتمد ملفات التحضير الحالية، وأوافق على بدء مرحلة التنفيذ البرمجي المنضبط تحت إدارة Tera.

المطلوب الآن:
1. لا تنفذ التطبيق كاملًا دفعة واحدة.
2. لا تطلب مني تحديد كل مهمة برمجية يدويًا.
3. طبّق Execution Orchestration Protocol.
4. ولّد EngineeringAgent فقط إذا كان مطلوبًا.
5. قبل الدفعة الأولى، اعرض علي: الاسم، الهدف، العميل، الملفات المرجعية، المسموح/الممنوع، معايير القبول، فحص التصميم.
6. لا تبدأ قبل موافقتي.
```

### Post-execution review

```text
نفّذ Post-Execution Review Gate على المهمة المنفذة.

المطلوب:
1. راجع الناتج الفعلي، وليس تقرير Sub-Agent فقط.
2. افحص الملفات المتغيرة، الحزم، الأوامر المنفذة، والآثار الجانبية.
3. أعطني النتيجة: PASS / NEEDS_FIX / BLOCKED
4. لا تغيّر حالة المهمة إلى Accepted أو Closed قبل اجتياز البوابة.
```

---

## 10. How to Know Tera Is Working Correctly

Tera is working correctly when:

- Does not start coding too early.
- Asks for approval at phase gates.
- Chooses the minimum sufficient file set.
- Reduces screens when possible.
- Does not generate agents without reason.
- Documents the reason for each decision.
- Distinguishes between generated agents and active agents.
- Follows approved design.
- Splits execution into small batches.
- Reviews after each batch.
- Runs `Post-Execution Review Gate` before accepting any task.

---

## 11. Conclusion

Treat Tera as a technical project manager, not a command executor.

You provide: ideas, constraints, approvals, and corrections.

Tera provides: decisions, preparation files, plans, sub-agents, execution batches, and review reports.

When Tera follows this pattern, the system is working correctly.
