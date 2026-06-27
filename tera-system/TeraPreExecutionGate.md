# TeraPreExecutionGate.md

# بوابة ما قبل التنفيذ — Pre-Execution Gate

## 1. الغرض

هذا الملف يعرّف بوابة تحقق إلزامية قبل أن يعتمد Tera أي مهمة تنفيذية أو يفوضها إلى عميل فرعي.

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
| 1 | هل المهمة مرتبطة مباشرة بمرحلة معتمدة في خطة التنفيذ؟ | Yes |
| 2 | هل المهمة أصغر وحدة تنفيذية ممكنة؟ | Yes |
| 3 | هل تحتوي المهمة على هدف واحد فقط؟ | Yes |
| 4 | هل يوجد أي عنصر يمكن تأجيله دون كسر المهمة؟ | No |
| 5 | هل تضيف المهمة شاشة أو UI دون أن تكون مهمة UI؟ | No |
| 6 | هل تضيف المهمة API أو Route دون طلب صريح؟ | No |
| 7 | هل تضيف Auth أو Roles أو Sessions دون طلب صريح؟ | No |
| 8 | هل تضيف Prisma models أو جداول دون أن تكون مهمة Data Schema؟ | No |
| 9 | هل تنفذ migration أو `db push` دون أن تكون مهمة قاعدة بيانات معتمدة؟ | No |
| 10 | هل تنشئ ملف `.env` بقيم تشغيل فعلية بدل `.env.example`؟ | No |
| 11 | هل تضيف مكتبات غير مطلوبة مباشرة للمهمة؟ | No |
| 12 | هل تكتب خارج Allowed Write Targets؟ | No |
| 13 | هل تعدل `tera-system/` أو `project-preparation/` أثناء التنفيذ؟ | No |
| 14 | هل الأوامر المقترحة تحتاج موافقة المستخدم قبل التنفيذ؟ | Yes إذا كانت مؤثرة |
| 15 | هل تم فحص الآثار الجانبية لكل أمر Shell / CLI مقترح؟ | Yes |
| 16 | هل يوجد أمر ينشئ ملفًا أو يعدل ملفًا أو يشغل توليد كود خارج نطاق المهمة؟ | No |
| 17 | هل يوجد تناقض بين القيود والمخرجات أو Allowed Write Targets؟ | No |
| 18 | هل معايير القبول قابلة للاختبار بوضوح؟ | Yes |
| 19 | هل يوجد مسار تراجع آمن إذا فشل التنفيذ؟ | Yes |

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
Prisma Data Models
Migrations
db push
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
| إنشاء ملفات | `.env`, `schema.prisma`, config files | يسمح فقط إذا كانت ضمن Allowed Write Targets |
| تعديل ملفات | `package.json`, lock file, config | يسمح فقط إذا كان مذكورًا في المهمة |
| توليد كود | Prisma Client, generated files | ممنوع إلا إذا كانت المهمة مخصصة لذلك |
| تشغيل قاعدة بيانات | `db push`, `migrate`, seed | ممنوع إلا في مهمة قاعدة بيانات معتمدة |
| الاتصال بخدمة خارجية | package registry, DB, API | يحتاج موافقة إذا كان مؤثرًا |
| حذف أو استبدال ملفات | cleanup, overwrite | يحتاج تصريح واضح |

إذا كان الأمر ينشئ أثرًا جانبيًا غير مسموح، يجب على Tera أن يختار أحد الإجراءات التالية قبل عرض المهمة:

1. استبدال الأمر بخطوات يدوية آمنة.
2. تضييق الأمر أو تغيير الخيارات إن كان ذلك ممكنًا.
3. إضافة خطوة تنظيف صريحة إذا كان الأثر الجانبي مؤقتًا وآمنًا.
4. فشل البوابة بنتيجة `NEEDS_REVISION` وتصحيح المهمة ذاتيًا.
5. طلب موافقة صريحة من المستخدم إذا كان تجاوز النطاق ضروريًا.

### قاعدة Prisma الخاصة

عند استخدام Prisma، تطبق القواعد التالية:

- لا تستخدم `npx prisma init` إذا كانت المهمة تمنع إنشاء `.env`، إلا إذا كانت المهمة تتضمن صراحة معالجة هذا الأثر الجانبي.
- إذا كان الهدف هو إنشاء `.env.example` فقط، فالخيار الآمن هو إنشاء `prisma/schema.prisma` يدويًا بدل الاعتماد على أمر ينشئ `.env`.
- يسمح بإنشاء ملف `prisma/schema.prisma` الأساسي فقط إذا كان يحتوي على:
  - `generator client`
  - `datasource db`
  - بدون أي `model`
- يمنع إنشاء أي `model` داخل `schema.prisma` إلا في مهمة Schema معتمدة.
- يمنع تشغيل:
  - `npx prisma db push`
  - `npx prisma migrate`
  - `npx prisma generate`
  - اختبار اتصال قاعدة البيانات
  إلا إذا كانت المهمة مخصصة لذلك ومعتمدة صراحة.

### قاعدة التعارض الداخلي

إذا احتوت المهمة على قيد ومخرج يتعارضان، يجب أن تفشل البوابة.

أمثلة:

| التعارض | الحكم الصحيح |
|---|---|
| ممنوع إنشاء `.env` لكن الأمر المقترح ينشئ `.env` | `NEEDS_REVISION` |
| ممنوع Schema لكن Allowed Write Targets تسمح بـ `schema.prisma` دون توضيح | تصحيح الصياغة إلى: ملف Schema أساسي بدون Models |
| ممنوع توليد كود لكن الأمر يشغل `prisma generate` | `NEEDS_REVISION` |
| ممنوع اتصال قاعدة بيانات لكن المعيار يطلب `db push` | `NEEDS_REVISION` |

لا يجوز اعتبار المهمة `PASS` قبل إزالة التعارض.

---

## 7. قاعدة مهام التأسيس التقني

في المهام الأولى من أي مشروع تقني، يجب الفصل بين:

1. Scaffold المشروع.
2. إعداد ORM أو أدوات التطوير.
3. إنشاء Schema.
4. تشغيل Migration أو db push.
5. تنفيذ UI.
6. تنفيذ Auth.

مثال:

```text
TASK-0001 = Scaffold only
TASK-0002 = Prisma schema
TASK-0003 = Migration / db push
TASK-0004 = أول شاشة أو موديول
```

لا يجوز دمج هذه الخطوات في مهمة واحدة إلا إذا كان المشروع صغيرًا جدًا ووافق المستخدم صراحة.

---

## 8. قاعدة TASK-0001 الافتراضية

إذا كانت أول مهمة تنفيذية لمشروع Next.js + Prisma، فالنطاق الآمن الافتراضي هو:

```text
Scaffold Next.js + TypeScript + تثبيت Prisma + إنشاء prisma/schema.prisma الأساسي + إنشاء .env.example فقط
```

النطاق المسموح افتراضيًا:

```text
- إنشاء مشروع Next.js + TypeScript.
- تثبيت الحزم الضرورية فقط.
- إنشاء ملف prisma/schema.prisma الأساسي يدويًا إن لزم.
- إنشاء .env.example فقط.
- عدم إنشاء .env.
```

صيغة `schema.prisma` المسموحة في TASK-0001:

```prisma
generator client {
  provider = "prisma-client-js"
}

datasource db {
  provider = "postgresql"
  url      = env("DATABASE_URL")
}
```

الممنوع افتراضيًا في TASK-0001:

```text
لا Prisma models
لا ConnectionTest model
لا db push
لا migration
لا prisma generate
لا اختبار اتصال فعلي بقاعدة البيانات
لا UI
لا API
لا Auth
لا .env بقيم فعلية
لا استخدام أوامر CLI ينتج عنها .env دون معالجة صريحة
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
| No real secrets or `.env` values | PASS / FAIL | ... |
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
Read project state → Identify next task → Draft task → Check CLI side effects → Run checklist → Revise until PASS → Ask approval
```

لا يعتمد على الذاكرة أو الاستنتاج العام. يعتمد على القائمة والفحص.

---

## 12. القاعدة النهائية

لا يبدأ التنفيذ لأن المهمة تبدو منطقية تقنيًا.

يبدأ التنفيذ فقط إذا كانت المهمة:

```text
محددة + صغيرة + قابلة للاختبار + لا تحتوي توسعًا + لا تحتوي آثار أوامر جانبية مخالفة + اجتازت Pre-Execution Gate
```
