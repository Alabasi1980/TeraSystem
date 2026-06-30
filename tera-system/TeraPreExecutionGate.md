# TeraPreExecutionGate.md

# بوابة ما قبل التنفيذ وما بعده — Pre/Post Execution Gates

## 1. الغرض

هذا الملف يعرّف بوابة تحقق إلزامية قبل التنفيذ، وبوابة مراجعة إلزامية بعد التنفيذ، قبل أن يعتمد Tera أي مهمة تنفيذية أو يفوضها إلى عميل فرعي أو يقبل نتيجتها النهائية.

الهدف هو أن يستطيع Tera قيادة تطبيق صغير أو متوسط حتى عند استخدام نموذج ذكاء متوسط أو ضعيف، وذلك عبر قواعد تشغيل واضحة وقابلة للفحص، بدل الاعتماد على الاستنتاج الحر.

هذه البوابة لا تجعل المستخدم مراجعًا تفصيليًا. المستخدم يعتمد أو يرفض القرار النهائي، أما Tera فهو المسؤول عن اكتشاف توسع المهمة أو تعارضها قبل عرضها.

---

## 2. متى تطبق البوابة؟

تطبق قبل كل حالة من الحالات التالية:

- إنشاء أو عرض أي `TASK-ID` تنفيذية.
- تغيير حالة مهمة من `Draft` إلى `Approved`.
- تفويض أي Sub-Agent للتنفيذ.
- الانتقال من `Plan Mode` إلى `Build Mode`.
- تشغيل أوامر Shell مؤثرة مثل `npm`, `npx`, `prisma`, `git`, `docker`.
- إنشاء أو تعديل كود التطبيق.

لا يجوز تخطي هذه البوابة لأن المهمة تبدو بسيطة.

---

## 3. قاعدة التشغيل الأساسية

قبل التفويض، يجب أن تكون المهمة:

```text
Smallest Safe Executable Unit
```

أي أصغر وحدة تنفيذية آمنة تحقق خطوة واحدة واضحة من الخطة، دون إدخال أعمال يمكن تأجيلها.

إذا احتوت المهمة على أكثر من هدف مستقل، يجب تقسيمها.

---

## 3.1 Orchestration Decision Matrix Prerequisite

Before running the `Pre-Execution Gate`, Tera must apply the Orchestration Decision Matrix defined in `tera-system/TeraAgent.md`.

This pre-task step determines:

- the smallest sufficient orchestration level
- whether helper agents are needed
- the expected review surfaces
- the initial security sensitivity
- whether handoff readiness is relevant at all
- whether the task maps cleanly to `PROJECT_MASTER_PLAN.md`, `PROJECT_DETAILED_EXECUTION_PLAN.md`, and the current `EXECUTION_BATCH_PLAN.md` when those files exist

Important clarification:

- `Security Sensitivity Levels` are determined during task preparation before delegation.
- `Independent Review Decision` is confirmed again after execution inside `Post-Execution Review Gate`.
- One does not replace the other.

## 3.2 Model Capability Gate Prerequisite

Before running the `Pre-Execution Gate`, Tera must also apply `Model Capability Gate`.

Purpose:

- decide whether the current model is suitable for the planned task
- decide whether safeguards are enough
- decide whether a stronger model should be recommended or required
- decide whether the task should be split before execution

`Model Capability Gate` comes after orchestration planning and task-package preparation, and before `Pre-Execution Gate`.

It does not replace:

- `ExecutionPreparationAgent`
- `SecurityAgent`
- `QAAndAcceptanceAgent`
- `ProjectControlAgent`
- `Post-Execution Review Gate`

It informs whether the current model is sufficient for the planned work and what extra safeguards are needed.

If the outcome is:

- `Current model sufficient`
- `Current model acceptable with safeguards`

then `Pre-Execution Gate` may continue.

If the outcome is:

- `Stronger model recommended`

Tera may continue only with documented safeguards or pause to ask the user when the risk is meaningful.

If the outcome is:

- `Stronger model required`
- `Split task before execution`

Tera must not proceed as a normal implementation delegation until that decision is resolved.

## 3.3 Technology Profile Gate

Before evaluating any implementation task, Tera must determine the active Technology Profile.

Profile sources:

1. `project-control/PROJECT_STATE.md`
2. `project-preparation/08_TECHNICAL_ARCHITECTURE.md`
3. Ask the user only if the technology is still unclear

Rules:

- Do not apply technology rules unless they belong to the active profile.
- Do not borrow rules from an inactive profile.
- `Pre-Execution Gate` = General Gate + Active Technology Profile additions.
- Technology-specific examples and command restrictions belong in:

```text
tera-system/profiles/
```

## 3.4 Client Approval Gate

Before evaluating any implementation task for an external client project, Tera must verify:

- client profile exists in `clients/`.
- contacts and approval authority are documented.
- client approval package exists under `clients/.../client-approval/`.
- approved scope is documented.
- design direction is approved before final UI work.
- `Execution Authorization` is approved and recorded before Build Mode.

If any required client approval item is missing, the Pre-Execution Gate result must be `BLOCKED`.

## 3.5 Direct Execution Exception Policy

الأصل أن Tera لا ينفذ كود التطبيق مباشرة، بل يدير العملاء الفرعيين ويراجع نتائجهم.

يسمح لتيرا بالتنفيذ المباشر فقط في الحالات التالية:

1. تعديل توثيقي صغير داخل ملفات التحكم أو التحضير.
2. إصلاح تنسيقي بسيط لا يغير منطق النظام.
3. إجراء طارئ يمنع توقف المنظومة، بشرط:
   - أن يكون محدودًا جدًا.
   - أن يتم توثيقه في `project-control/PROJECT_ACTIVITY_LOG.md`.
   - أن يتم تسجيل سبب عدم تفويضه إلى عميل فرعي.
   - أن يخضع لاحقًا إلى `Post-Execution Review Gate`.

أي تنفيذ كود تطبيقي أو تعديل معماري أو بناء شاشة أو API أو قاعدة بيانات يجب أن يذهب إلى العميل المختص، ولا يسمح لتيرا بتنفيذه مباشرة.

---

## 4. مخرجات البوابة

كل مهمة تنفيذية يجب أن تحتوي على قسم واضح باسم:

```text
Pre-Execution Gate Result
```

والنتيجة تكون واحدة فقط:

```text
PASS
NEEDS_REVISION
BLOCKED
```

- `PASS`: المهمة ضيقة وآمنة وجاهزة لطلب اعتماد المستخدم.
- `NEEDS_REVISION`: تيرا يجب أن يصحح المهمة ذاتيًا قبل عرضها للاعتماد.
- `BLOCKED`: توجد معلومة ناقصة أو تعارض يمنع التنفيذ.

إذا كانت النتيجة ليست `PASS`، لا يجوز تفويض العميل الفرعي.

---

## 5. Checklist إلزامي

يجب على Tera فحص البنود التالية بنمط نعم/لا:

| # | سؤال التحقق | النتيجة المطلوبة |
|---|---|---|
| 1 | هل المهمة مرتبطة مباشرة بمرحلة أو بند معتمد في خطة التنفيذ، وفي `PROJECT_MASTER_PLAN.md` / `PROJECT_DETAILED_EXECUTION_PLAN.md` / `EXECUTION_BATCH_PLAN.md` إن وُجدت؟ | Yes |
| 2 | هل المهمة أصغر وحدة تنفيذية ممكنة؟ | Yes |
| 3 | هل تحتوي المهمة على هدف واحد فقط؟ | Yes |
| 4 | هل يوجد أي عنصر يمكن تأجيله دون كسر المهمة؟ | No |
| 5 | هل تضيف المهمة شاشة أو UI دون أن تكون مهمة UI؟ | No |
| 6 | هل تضيف المهمة API أو Route دون طلب صريح؟ | No |
| 7 | هل تضيف Auth أو Roles أو Sessions دون طلب صريح؟ | No |
| 8 | هل تضيف Database models / entities / schema objects دون أن تكون مهمة Data Schema معتمدة؟ | No |
| 9 | هل تنفذ database migration / apply commands دون أن تكون مهمة قاعدة بيانات معتمدة؟ | No |
| 10 | هل تنشئ `.env` فعليًا خارج مهمة محلية معتمدة أو دون خطة Secret Handling واضحة؟ | No |
| 11 | هل ستظهر أي secrets حقيقية في ملف مهمة أو سجل أو handback أو ملف config/code؟ | No |
| 12 | هل تضيف مكتبات غير مطلوبة مباشرة للمهمة؟ | No |
| 13 | هل تكتب خارج Allowed Write Targets؟ | No |
| 14 | هل تعدل `tera-system/` أو `project-preparation/` أثناء التنفيذ؟ | No |
| 15 | هل الأوامر المقترحة تحتاج موافقة المستخدم قبل التنفيذ؟ | Yes إذا كانت مؤثرة |
| 16 | هل تم فحص الآثار الجانبية لكل أمر Shell / CLI مقترح؟ | Yes |
| 17 | هل يوجد أمر ينشئ ملفًا أو يعدل ملفًا أو يشغل توليد كود خارج نطاق المهمة؟ | No |
| 18 | هل يوجد تناقض بين القيود والمخرجات أو Allowed Write Targets؟ | No |
| 19 | هل معايير القبول قابلة للاختبار بوضوح؟ | Yes |
| 20 | هل يوجد مسار تراجع آمن إذا فشل التنفيذ؟ | Yes |

إذا فشل أي بند، يجب على Tera تصحيح المهمة قبل عرضها.

---

## 6. قواعد منع تضخم المهمة

يمنع داخل أي مهمة تنفيذية إضافة أي عنصر غير مطلوب صراحة في عنوان المهمة أو معايير قبولها.

العناصر التالية تعتبر توسعًا افتراضيًا ما لم تذكر صراحة:

```text
UI
Dashboard
API Routes
Authentication
Authorization
Database Models / Entities / Schema Objects
Database Migrations
Database Apply Commands
Seed Data
External Services
Docker
CI/CD
Testing Framework
Reusable Components
Service Layer
Repository Layer
State Management
README أو توثيق إضافي
```

إذا رأى Tera أن أحد هذه العناصر مفيد، يسجله كـ:

```text
Deferred / Proposed Next Task
```

ولا يدخله في المهمة الحالية.

---

## 6.1 بوابة آثار الأوامر الجانبية — CLI / Tool Side Effects Gate

قبل اعتماد أي مهمة تحتوي على أوامر Shell أو CLI، يجب على Tera فحص كل أمر مقترح وفق السؤال التالي:

```text
ما الملفات أو التغييرات أو العمليات التي سينتجها هذا الأمر افتراضيًا؟
```

لا يجوز اعتماد أمر لمجرد أنه شائع أو منطقي تقنيًا. يجب التأكد أن آثاره الجانبية لا تخالف نطاق المهمة.

### قواعد الفحص

يجب على Tera فحص كل أمر من ناحية:

| نوع الأثر | أمثلة | القرار |
|---|---|---|
| إنشاء ملفات | `.env`, ORM schema/configuration files, config files | يسمح فقط إذا كانت ضمن Allowed Write Targets |
| تعديل ملفات | `package.json`, lock file, config | يسمح فقط إذا كان مذكورًا في المهمة |
| توليد كود | ORM-generated files, framework-generated files | ممنوع إلا إذا كانت المهمة مخصصة لذلك |
| تشغيل قاعدة بيانات | apply commands, migrations, seed | ممنوع إلا في مهمة قاعدة بيانات معتمدة |
| الاتصال بخدمة خارجية | package registry, DB, API | يحتاج موافقة إذا كان مؤثرًا |
| حذف أو استبدال ملفات | cleanup, overwrite | يحتاج تصريح واضح |

إذا كان الأمر ينشئ أثرًا جانبيًا غير مسموح، يجب على Tera أن يختار أحد الإجراءات التالية قبل عرض المهمة:

1. استبدال الأمر بخطوات يدوية آمنة.
2. تضييق الأمر أو تغيير الخيارات إن كان ذلك ممكنًا.
3. إضافة خطوة تنظيف صريحة إذا كان الأثر الجانبي مؤقتًا وآمنًا.
4. فشل البوابة بنتيجة `NEEDS_REVISION` وتصحيح المهمة ذاتيًا.
5. طلب موافقة صريحة من المستخدم إذا كان تجاوز النطاق ضروريًا.

### Technology Profile Rule

Technology-specific ORM, scaffold, and database command rules must be loaded from the active Technology Profile only.

Examples:

- ORM/framework init commands
- ORM schema/configuration files
- database models / entities
- migration commands
- database apply commands
- generation commands
- stack-specific first-task limits

General rule:

```text
Do not place stack-specific execution logic in the generic gate when it belongs to a Technology Profile.
```

Database-layer validation rule:

```text
Schema definitions may define field types and relations.
Business validation rules such as amount > 0 must not be implemented as database constraints unless explicitly approved.
```

### Secret Handling and Redaction Rule

الأسرار الحقيقية مثل كلمات المرور، مفاتيح API، رموز الوصول، وسلاسل الاتصال الحقيقية يسمح بها فقط في:

- `.env`
- `.env.local`
- متغيرات البيئة المحلية
- مخزن أسرار محلي يوافق عليه المستخدم صراحة

ويمنع منعًا باتًا ظهورها داخل:

- `project-control/`
- `project-preparation/`
- `generated-agents/`
- `tera-system/`
- ملفات `TASK-*.md`
- `PROJECT_ACTIVITY_LOG.md`
- `DECISIONS_LOG.md`
- `ISSUES_AND_GAPS.md`
- نصوص handback
- أوامر موثقة داخل المهام
- ملفات الكود أو config مثل `prisma.config.ts`
- أي fallback value داخل الكود أو الإعدادات

إذا احتاجت المهمة سرًا حقيقيًا، يجب على Tera أن يكتب الخطة بصيغة مرجعية فقط، مثل:

```text
Use local environment secret.
Create/update .env locally with DATABASE_URL from user-provided secret.
```

ولا يجوز كتابة السر نفسه داخل ملف المهمة أو السجل أو التقرير.

ويمتد هذا المنع أيضًا إلى:

- ردود المحادثة.
- handback texts.
- review notes.
- incident descriptions.
- activity logs.
- decision logs.
- post-task summaries.

عند توثيق سلسلة اتصال أو أمر يستخدم سرًا، يجب استخدام صيغة redacted فقط، مثل:

```text
postgresql://postgres:[REDACTED]@localhost:5432/myapp_db
```

وعند توثيق أي حادثة `Secret Exposure` أو حادثة أمنية مشابهة، يمنع تكرار القيمة المسرّبة نهائيًا في أي ملف أو تقرير أو handback أو log أو رد محادثة. يجب استخدام `[REDACTED]` فقط حتى عند وصف الحادثة نفسها.

القاعدة الأساسية:

```text
Any real secret outside approved local environment files = gate failure.
```

ويعتبر وجود fallback حقيقي داخل ملفات config/code مخالفة مباشرة، حتى لو كان الملف محليًا.

### قاعدة التعارض الداخلي

إذا احتوت المهمة على قيد ومخرج يتعارضان، يجب أن تفشل البوابة.

أمثلة:

| التعارض | الحكم الصحيح |
|---|---|
| ممنوع إنشاء `.env` لكن الأمر المقترح ينشئ `.env` | `NEEDS_REVISION` |
| ممنوع Schema لكن Allowed Write Targets تسمح بملف ORM schema دون توضيح | تصحيح الصياغة إلى: ملف schema/config أساسي فقط |
| ممنوع توليد كود لكن الأمر يشغل ORM/framework generation command | `NEEDS_REVISION` |
| ممنوع اتصال قاعدة بيانات لكن المعيار يطلب database apply command | `NEEDS_REVISION` |

لا يجوز اعتبار المهمة `PASS` قبل إزالة التعارض.

---

## 6.2 بوابة المراجعة بعد التنفيذ — Post-Execution Review Gate

### الغرض

بعد تسليم أي Sub-Agent لمهمة تنفيذية، يجب على Tera مراجعة الناتج الفعلي قبل قبول المهمة.

القاعدة الأساسية:

```text
Do not accept an implementation task based on the Sub-Agent report alone.
```

لا يجوز لـ Tera قبول المهمة اعتمادًا على تقرير العميل الفرعي فقط.
يجب أن يراجع الملفات الفعلية، والأوامر المنفذة، والآثار الجانبية، والتوافق مع النطاق ومعايير القبول.

### متى تطبق؟

تطبق بعد كل مهمة تنفيذية ينتج عنها واحد أو أكثر من التالي:

- إنشاء ملفات.
- تعديل ملفات.
- حذف ملفات.
- تثبيت حزم.
- تشغيل أوامر Shell أو CLI.
- إنشاء مشروع Scaffold.
- تعديل إعدادات.
- إنشاء أو تعديل Schema.
- تنفيذ UI.
- تنفيذ API.
- أي تغيير داخل كود التطبيق.

### القاعدة الإلزامية

قبل تغيير حالة أي مهمة إلى:

```text
Accepted
Closed
```

يجب تنفيذ `Post-Execution Review Gate`.

إذا لم تجتز المهمة هذه البوابة، يجب أن تبقى حالتها واحدة من:

```text
Submitted
Needs Fix
Blocked
```

بحسب نتيجة المراجعة.

### Checklist إلزامي

القالب الرسمي لتسجيل هذه المراجعة داخل ملف المهمة موجود في:

```text
tera-system/runtime/TERA_RUNTIME_TEMPLATES.md Section 32
```

| # | فحص المراجعة بعد التنفيذ | النتيجة المطلوبة |
|---|---|---|
| 1 | هل الملفات التي تغيرت تقع داخل Allowed Write Targets؟ | Yes |
| 2 | هل تم إنشاء أي ملف خارج النطاق؟ | No |
| 3 | هل تم حذف أي ملف دون تفويض؟ | No |
| 4 | هل تم تثبيت أي مكتبة غير مطلوبة صراحة؟ | No |
| 5 | هل توجد ملفات Auto-generated غير مطابقة للنطاق؟ | No |
| 6 | هل توجد ملفات توثيق مثل README لم تكن مطلوبة؟ | No |
| 7 | هل توجد إعدادات CSS أو UI غير مطلوبة؟ | No |
| 8 | هل تم إدخال Tailwind أو Bootstrap أو مكتبة UI دون موافقة؟ | No |
| 9 | هل توجد Dark Mode classes أو Theme غير مطابق لـ `28_UI_UX_GUIDELINES.md`؟ | No |
| 10 | هل تم إنشاء `.env` فعلي خارج مهمة محلية معتمدة أو دون ضوابط الأسرار؟ | No |
| 11 | هل ظهرت أي secrets حقيقية داخل ملفات المهمة أو السجلات أو handback؟ | No |
| 12 | هل ظهرت أي secrets حقيقية داخل code/config أو fallback values؟ | No |
| 13 | هل تم توثيق الأوامر وقيم الاتصال بصيغة redacted فقط؟ | Yes |
| 14 | هل يحتوي ORM schema أو تعريف الكيانات على models / entities غير مصرح بها؟ | No |
| 15 | هل تم تحويل قواعد Business Validation إلى database constraints دون موافقة صريحة؟ | No |
| 16 | هل تم تشغيل database apply / migration / generation commands دون تفويض؟ | No |
| 17 | هل تم إنشاء API أو Route دون أن تكون المهمة API؟ | No |
| 18 | هل تم إنشاء Auth أو Roles أو Sessions دون تفويض؟ | No |
| 19 | هل تم الالتزام بمعايير القبول؟ | Yes |
| 20 | هل المخرجات قابلة للتشغيل أو الاختبار؟ | Yes |
| 21 | هل توجد آثار جانبية من أوامر CLI لم تكن متوقعة؟ | No |
| 22 | هل تم تحديث ملف المهمة والسجلات الإدارية الأساسية بشكل صحيح وبدون IDs مكررة؟ | Yes |
| 23 | هل تمت مراجعة `project-control/TASK_REGISTRY.md` بعد التنفيذ؟ | Yes |
| 24 | هل تمت مراجعة `project-control/PROJECT_ACTIVITY_LOG.md` بعد التنفيذ؟ | Yes |
| 25 | هل تمت مراجعة `project-control/PROJECT_STATE.md` بعد التنفيذ؟ | Yes |
| 26 | هل تمت مراجعة `project-control/ISSUES_AND_GAPS.md` بعد التنفيذ؟ | Yes |
| 27 | هل تمت مراجعة `project-control/DECISIONS_LOG.md` وملف المهمة نفسه بعد التنفيذ؟ | Yes |
| 28 | هل تمت مراجعة `project-control/TERA_ACTIVE_CONTEXT.md` إذا كان موجودًا؟ | Yes / N/A |
| 29 | هل تم تصنيف أي تعديل خارج Allowed Write Targets إلى `Approved deviation` أو `Needs user approval` أو `Reverted`؟ | Yes / N/A |
| 30 | هل قرر Tera بوضوح إن كانت المهمة تحتاج مراجعة مستقلة من `ProjectControlAgent` أو `SecurityAgent` أو `QAAndAcceptanceAgent`؟ | Yes |

### Control Files Review Rule

بعد كل مهمة تنفيذية، لا يكتفي Tera بمراجعة الكود أو الملفات المتغيرة فقط.
يجب عليه مراجعة ملفات التحكم الأساسية التالية أيضًا قبل قبول المهمة:

- `project-control/TASK_REGISTRY.md`
- `project-control/PROJECT_ACTIVITY_LOG.md`
- `project-control/PROJECT_STATE.md`
- `project-control/ISSUES_AND_GAPS.md`
- `project-control/DECISIONS_LOG.md`
- `project-control/tasks/[TASK-ID].md`
- `project-control/TERA_ACTIVE_CONTEXT.md` إذا كان موجودًا

والغرض من هذه المراجعة هو التأكد من:

- عدم وجود IDs مكررة أو غير متسلسلة.
- عدم وجود تسريب أسرار داخل السجلات أو ملفات المهام.
- عدم وجود تناقض بين حالة المهمة والسجل والنشاط والقرارات.
- عدم بقاء handback أو review أو incident description بصياغة غير منقحة.
- عدم وجود فجوة توثيقية بين ما نُفذ فعليًا وما سُجِّل إداريًا.

### نتائج البوابة

نتيجة `Post-Execution Review Gate` يجب أن تكون واحدة فقط:

```text
PASS
NEEDS_FIX
BLOCKED
```

- `PASS`: المخرجات مطابقة للنطاق ومعايير القبول، ولا توجد آثار جانبية غير مصرح بها.
- `NEEDS_FIX`: ظهرت مخالفات قابلة للإصلاح ضمن نفس المهمة.
- `BLOCKED`: ظهرت مشكلة لا يمكن إصلاحها دون قرار من المستخدم أو تعديل نطاق المهمة.

بعد نتيجة البوابة، يسجل Tera قرار المهمة النهائي بشكل منفصل:

```text
Accepted / Needs Fix / Blocked / Rework Needed / Deferred / Cancelled
```

لا يجوز استخدام `Accepted` أو `Closed` إلا إذا كانت نتيجة `Post-Execution Review Gate` هي `PASS`.

لا يجوز أن تحصل المهمة على `PASS` إذا ظهر أي secret حقيقي خارج ملفات البيئة المحلية المعتمدة.
وإذا ظهرت قيمة سرية داخل task file أو log أو report أو handback أو issue record أو decision note، فإن نتيجة `Post-Execution Review Gate` تصبح `NEEDS_FIX` أو `BLOCKED` حتى لو كان الكود نفسه صحيحًا.

### Allowed Write Target Deviation Rule

أي تعديل خارج `Allowed Write Targets` يجب أن يُصنَّف صراحةً كأحد الخيارات التالية:

```text
Approved deviation
Needs user approval
Reverted
```

ولا يجوز اعتبار التعديل مقبولًا فقط لأنه "مفيد" أو "غير ضار".
إذا لم يُصنَّف الانحراف بوضوح، فلا يجوز أن تحصل المهمة على `PASS`.

### قاعدة `NEEDS_FIX`

إذا كانت النتيجة `NEEDS_FIX`، يجب على Tera:

1. ألا يقبل المهمة.
2. يحدد المخالفات بوضوح.
3. يطلب من العميل الفرعي إصلاحها.
4. يحتفظ بنفس `TASK-ID`.
5. يسجل سبب المشكلة.
6. يوضح إن كان السبب من تفويض Tera أو من تنفيذ العميل.

### Root Cause Rule

إذا كان سبب المخالفة هو أن تفويض Tera كان غير دقيق، يجب على Tera أن يسجل ذلك بوضوح.

مثال:

```text
Root Cause: Tera delegation used a framework scaffold command without the required restrictive flags, causing forbidden default files to be generated.
```

ثم يضيف Tera توصية عملية لتجنب تكرار الخطأ في المهام القادمة.

### Independent Review Decision Rule

بعد كل مهمة تنفيذية، يجب على Tera أن يقرر صراحةً هل تحتاج المهمة مراجعة مستقلة إضافية قبل القبول النهائي:

- `ProjectControlAgent` عندما تكون الحاجة إلى مراجعة السجلات، الاتساق، التسلسل، أو اكتمال التوثيق.
- `SecurityAgent` عندما تشمل المهمة `Auth`, `JWT`, `Cookies`, `Middleware`, `Proxy`, `API Routes`, `Server Actions`, `Permissions`, `Role checks`, `Data Mutations`, `Secrets`, `Config`, أو أي سلوك أمني مشابه.
- `QAAndAcceptanceAgent` عندما تشمل المهمة `UI`, `Workflow`, أو `Acceptance Criteria` تحتاج تحققًا وظيفيًا مستقلًا.

إذا قرر Tera أن المراجعة المستقلة غير مطلوبة، يجب أن يذكر السبب داخل نتيجة المراجعة بعد التنفيذ.

### قاعدة خاصة بمهام Scaffold

أي مهمة Scaffold تستخدم أمرًا مثل:

- framework scaffold command
- `vite`
- `create-react-app`
- أي generator مشابه

يجب بعد التنفيذ مراجعة الملفات والحزم الناتجة فعليًا، وليس الاكتفاء بأن الأمر نجح.

يجب على Tera تصنيف الناتج إلى:

```text
Allowed boilerplate
Needs cleanup
Forbidden
```

أمثلة الفحص التفصيلية الخاصة بكل stack يجب أن تأتي من الـ Technology Profile النشط.

### قاعدة إزالة الحزم وآثارها

عند منع حزمة أو طلب إزالتها، يجب على Tera ألا يكتفي بفحص `package.json` فقط.

يجب فحص الآثار التالية:

- `package.json`
- `package-lock.json` / `pnpm-lock.yaml` / `yarn.lock`
- ملفات الإعدادات
- `imports`
- `class names` أو `usages` داخل ملفات المصدر
- القوالب أو الملفات المولدة

القاعدة الأساسية:

```text
The task cannot PASS if forbidden package traces remain in lockfiles or source code.
```

إذا بقي أثر الحزمة الممنوعة في `lockfiles` أو الكود أو القوالب المولدة، فلا يجوز أن تجتاز المهمة `Post-Execution Review Gate` بنتيجة `PASS`.

### Local Temp Files Hygiene Rule

أي ملفات تشغيل محلية أو مؤقتة ناتجة عن dev server أو Codex أو التشغيل المحلي لا يجب أن تدخل Git.

أمثلة:

- `.codex-dev-*.log`
- `.codex-dev-*.err`
- local temp run logs
- logs تحتوي مسارات محلية أو IP أو تفاصيل dev server

إذا اكتشف Tera أن مثل هذه الملفات دخلت commit أو ظهرت كملفات متتبعة داخل Git، فيجب تصنيف الحالة:

```text
Cleanup required
```

ويجب تنظيفها وإضافة ignore مناسب قبل الانتقال إلى مهمة جديدة.

### قالب يضاف داخل ملف المهمة بعد التسليم

```markdown
## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS / FAIL | ... |
| No unauthorized files created | PASS / FAIL | ... |
| No unauthorized files deleted | PASS / FAIL | ... |
| No unauthorized packages added | PASS / FAIL | ... |
| No unauthorized UI/CSS/theme changes | PASS / FAIL | ... |
| No real secrets outside approved local environment files | PASS / FAIL | ... |
| Secrets redacted in docs/logs/config references | PASS / FAIL | ... |
| No unauthorized ORM models/entities/migrations | PASS / FAIL | ... |
| No unapproved business validation moved to DB constraints | PASS / FAIL | ... |
| No unauthorized API/Auth created | PASS / FAIL | ... |
| Acceptance criteria satisfied | PASS / FAIL | ... |
| CLI side effects reviewed | PASS / FAIL | ... |
| Task file and core project-control records reviewed | PASS / FAIL | ... |
| No secret leakage in task files/logs/reports/handbacks | PASS / FAIL | ... |
| No duplicate project-control IDs created | PASS / FAIL | ... |
| Any out-of-target changes classified | PASS / FAIL / N/A | ... |
| Independent review decision recorded | PASS / FAIL | ... |

Gate Status: PASS / NEEDS_FIX / BLOCKED

Root Cause if failed:
- ...

Required Action:
- ...

Independent Review:
- ProjectControlAgent: Required / Not Required
- SecurityAgent: Required / Not Required
- QAAndAcceptanceAgent: Required / Not Required

Deviation Classification:
- Approved deviation / Needs user approval / Reverted / N/A
```

---

## 7. قاعدة مهام التأسيس التقني

في المهام الأولى من أي مشروع تقني، يجب الفصل بين:

1. Scaffold المشروع.
2. إعداد ORM أو أدوات التطوير.
3. إنشاء Schema.
4. تشغيل migration أو database apply commands.
5. تنفيذ UI.
6. تنفيذ Auth.

مثال:

```text
TASK-0001 = Scaffold only
TASK-0002 = ORM schema / entities
TASK-0003 = Migration / database apply
TASK-0004 = أول شاشة أو موديول
```

لا يجوز دمج هذه الخطوات في مهمة واحدة إلا إذا كان المشروع صغيرًا جدًا ووافق المستخدم صراحة.

---

## 8. قاعدة TASK-0001 الافتراضية

قواعد أول مهمة تنفيذية، وحدود scaffold وORM وقاعدة البيانات، يجب أن تأتي من الـ Technology Profile النشط.

```text
Use active Technology Profile:
tera-system/profiles/[active-profile].md
```

إذا كانت هناك حاجة لتجاوز هذا النطاق، يجب على Tera طلب موافقة المستخدم صراحة وشرح السبب.

---

## 9. قالب قسم البوابة داخل المهمة

يجب أن يضاف إلى كل ملف مهمة تنفيذية:

```markdown
## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS / FAIL | ... |
| One objective only | PASS / FAIL | ... |
| No deferrable work included | PASS / FAIL | ... |
| No UI unless explicitly requested | PASS / FAIL | ... |
| No API unless explicitly requested | PASS / FAIL | ... |
| No Auth unless explicitly requested | PASS / FAIL | ... |
| No schema/migration unless explicitly requested | PASS / FAIL | ... |
| No real secrets outside approved local environment files | PASS / FAIL | ... |
| Secret handling plan documented and redacted | PASS / FAIL | ... |
| CLI side effects checked | PASS / FAIL | ... |
| No internal contradiction between constraints and outputs | PASS / FAIL | ... |
| Allowed Write Targets are narrow | PASS / FAIL | ... |
| Acceptance criteria are testable | PASS / FAIL | ... |

Gate Status: PASS / NEEDS_REVISION / BLOCKED
Required Action:
- ...
```

---

## 10. سلوك Tera عند فشل البوابة

إذا فشلت البوابة:

1. لا يطلب من المستخدم اكتشاف الخلل.
2. لا يطلب من المستخدم كتابة تفاصيل التصحيح.
3. يصحح Tera المهمة ذاتيًا بناءً على القواعد.
4. يترك الحالة `Draft`.
5. يذكر باختصار ما أزاله ولماذا.
6. يعرض النسخة المصححة فقط للاعتماد.

---

## 11. قاعدة النماذج الضعيفة

عند استخدام نموذج ذكاء ضعيف أو متوسط، يجب على Tera الالتزام بالنمط التالي:

```text
Read project state → Identify next task → Prepare/draft task package → Check CLI side effects → Run checklist → Revise until PASS → Ask approval
```

### ExecutionPreparationAgent Preparation Rule

Tera may use `ExecutionPreparationAgent` to prepare the initial task package before delegation.
This agent may draft scope, references, write targets, acceptance criteria, risk notes, and reviewer suggestions only.
Tera must still review that package himself, run the full `Pre-Execution Gate`, and keep final authority over approval, delegation, and closure.

لا يعتمد على الذاكرة أو الاستنتاج العام. يعتمد على القائمة والفحص.

---

## 12. القاعدة النهائية

لا يبدأ التنفيذ لأن المهمة تبدو منطقية تقنيًا.

يبدأ التنفيذ فقط إذا كانت المهمة:

```text
محددة + صغيرة + قابلة للاختبار + لا تحتوي توسعًا + لا تحتوي آثار أوامر جانبية مخالفة + اجتازت Pre-Execution Gate
```
