# PROJECT_STATE.md

# حالة المشروع المختصرة — Tera Project State

## 1. الغرض

هذا الملف هو الذاكرة المختصرة والرسمية للمشروع الحالي.

يستخدمه Tera Agent والعملاء الفرعيون لتقليل إعادة قراءة كل الملفات وتقليل استهلاك التوكنز.

لا يستبدل ملفات المشروع التفصيلية، بل يلخص الحالة الحالية والقرارات المعتمدة.

---

## 2. تعريف المشروع

| البند | القيمة |
|---|---|
| اسم المشروع | تطبيق إدارة الشيكات |
| نوع المشروع | تطبيق ويب مستقل |
| حجم المشروع | صغير / MVP |
| المرحلة الحالية | قبل أول تشغيل OpenCode / Readiness Review |
| بيئة التشغيل | OpenCode |
| التقنية المعتمدة | Next.js + TypeScript + PostgreSQL + Prisma |
| مصدر التفاصيل | `project-preparation/` |

---

## 3. القرارات المعتمدة

| القرار | المصدر | الحالة |
|---|---|---|
| المشروع MVP صغير وليس ERP | `project-preparation/TERA_PROJECT_DECISION.md` | معتمد |
| التقنية المعتمدة: Next.js, TypeScript, PostgreSQL, Prisma | `project-preparation/08_TECHNICAL_ARCHITECTURE.md` | معتمد |
| لا يبدأ التنفيذ قبل Readiness Review | `tera-system/TeraAgent.md` | معتمد |
| لا يتم نقل العملاء الفرعيين إلى `.opencode/agents/` إلا عند الحاجة | `tera-system/TeraAgent.md` | معتمد |
| `tera-system/` مرجع نظامي read-only أثناء التنفيذ | `.opencode/agents/tera.md` | معتمد |

---

## 4. الملفات التحضيرية الحالية

| الملف | الحالة | ملاحظة |
|---|---|---|
| `project-preparation/00_PROJECT_INPUTS.md` | موجود | تم تحديث التقنية المحسومة لاحقًا |
| `project-preparation/TERA_PROJECT_DECISION.md` | موجود | قرار افتتاحي للمشروع |
| `project-preparation/01_PROJECT_BRIEF.md` | موجود | مراجعة أثناء Readiness |
| `project-preparation/02_SCOPE_AND_BOUNDARIES.md` | موجود | مراجعة أثناء Readiness |
| `project-preparation/03_MODULES_AND_FEATURES.md` | موجود | مراجعة أثناء Readiness |
| `project-preparation/04_USERS_ROLES_PERMISSIONS.md` | موجود | مراجعة أثناء Readiness |
| `project-preparation/05_BUSINESS_WORKFLOWS.md` | موجود | مراجعة أثناء Readiness |
| `project-preparation/06_DATA_MODEL_PREPARATION.md` | موجود | مراجعة أثناء Readiness |
| `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md` | موجود | مراجعة أثناء Readiness |
| `project-preparation/08_TECHNICAL_ARCHITECTURE.md` | موجود | التقنية محسومة |
| `project-preparation/09_IMPLEMENTATION_PLAN.md` | موجود | يحتاج اعتماد قبل التنفيذ |
| `project-preparation/10_TESTING_AND_ACCEPTANCE.md` | موجود | يحتاج مراجعة مع خطة التنفيذ |
| `project-preparation/28_UI_UX_GUIDELINES.md` | موجود | دليل الستايل المعتمد |

---

## 5. العملاء الفرعيون

| العميل | الحالة | ملاحظة |
|---|---|---|
| `Tera Agent` | مفعل داخل `.opencode/agents/tera.md` | Primary Agent |
| العملاء المولدون داخل `generated-agents/opencode/` | غير مفعلين | لا يتم نقلهم إلا بقرار تيرا |
| `EngineeringAgent` | غير موجود/غير مفعل | قد يكون مطلوبًا لاحقًا عند بدء التنفيذ |

---

## 6. سياسة السياق الحالية

| البند | القرار |
|---|---|
| Full Context | ممنوع افتراضيًا |
| السياق الافتراضي | `PROJECT_STATE.md` + ملفات محددة في المهمة |
| المهام عالية التكلفة | تحتاج موافقة |
| وضع العمل قبل التنفيذ | Plan Mode |
| Build Mode | يحتاج موافقة صريحة |

---

## 7. المخاطر أو الفجوات المفتوحة

| البند | الحالة | الإجراء |
|---|---|---|
| لم يتم تنفيذ Readiness Review بعد | Open | تشغيل تيرا في Plan Mode |
| لم يتم تفعيل أي Sub-Agent فعليًا | Open | تيرا يقرر بعد المراجعة |
| لم يتم بدء التنفيذ البرمجي | Open | لا يبدأ قبل TASK-ID واعتماد خطة |

---

## 8. آخر ملخص سياق

المنظومة جاهزة مبدئيًا لأول تشغيل داخل OpenCode.  
الخطوة التالية هي تشغيل Tera في Plan Mode لقراءة `tera-system/` و`project-preparation/` وتنفيذ Readiness Review بدون كتابة كود وبدون نقل العملاء الفرعيين.

---

## 9. آخر تحديث

| البند | القيمة |
|---|---|
| Updated By | Tera System Preparation |
| Update Reason | إضافة سياسة إدارة التوكنز والذاكرة المختصرة قبل أول تشغيل |
| Next Step | Tera Readiness Review |
