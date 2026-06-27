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
| المرحلة الحالية | Readiness Review مكتمل — EngineeringAgent مولّد — بانتظار أول TASK-ID تنفيذي |
| بيئة التشغيل | OpenCode |
| التقنية المعتمدة | Next.js + TypeScript + PostgreSQL + Prisma |
| مصدر التفاصيل | `project-preparation/` |

---

## 3. القرارات المعتمدة

| القرار | المصدر | الحالة |
|---|---|---|
| المشروع MVP صغير وليس ERP | `project-preparation/TERA_PROJECT_DECISION.md` | معتمد |
| التقنية المعتمدة: Next.js, TypeScript, PostgreSQL, Prisma | `project-preparation/08_TECHNICAL_ARCHITECTURE.md` | معتمد |
| جميع ملفات التحضير 00–10 معتمدة من المستخدم | موافقة المستخدم | معتمد |
| مصدر التصميم: Tera-Decided Design, Clean Admin UI, Light Theme, RTL | `project-preparation/28_UI_UX_GUIDELINES.md` | معتمد |
| Readiness Review مكتمل — المشروع جاهز للتنفيذ | `tera-system/TeraAgent.md` | مكتمل |
| EngineeringAgent مولّد في `generated-agents/opencode/EngineeringAgent.md` | أمر المستخدم | مكتمل |
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
| `EngineeringAgent` | مولّد داخل `generated-agents/opencode/EngineeringAgent.md` | غير مفعل في `.opencode/agents/` — يُستخدم في المهام البرمجية عند تفويض Tera |
| `RequirementsScopeAgent` | مولّد داخل `generated-agents/opencode/` | غير مفعل — للمراجعة والتجهيز فقط |
| `BusinessWorkflowAgent` | مولّد داخل `generated-agents/opencode/` | غير مفعل — للمراجعة والتجهيز فقط |
| `DataDesignAgent` | مولّد داخل `generated-agents/opencode/` | غير مفعل — للمراجعة والتجهيز فقط |
| `UIUXStructureAgent` | مولّد داخل `generated-agents/opencode/` | غير مفعل — للمراجعة والتجهيز فقط |
| `ProjectControlAgent` | مولّد داخل `generated-agents/opencode/` | غير مفعل — للتحديثات الإدارية لـ `project-control/` |
| عملاء آخرون | غير مولدين | سيولّدون عند الحاجة في مراحلهم |

---

## 6. سياسة السياق الحالية

| البند | القرار |
|---|---|
| Full Context | ممنوع افتراضيًا |
| السياق الافتراضي | `PROJECT_STATE.md` + ملفات محددة في المهمة |
| المهام عالية التكلفة | تحتاج موافقة |
| وضع العمل قبل التنفيذ | Plan Mode |
| Build Mode | يحتاج موافقة صريحة |
| Pre-Execution Gate | إلزامي قبل أي تفويض أو تنفيذ |

---

## 7. المخاطر أو الفجوات المفتوحة

| البند | الحالة | الإجراء |
|---|---|---|
| Readiness Review | مكتمل ✅ | المشروع جاهز للتنفيذ — راجع تقرير المراجعة |
| EngineeringAgent | مولّد ✅ | جاهز للتفويض بعد اجتياز TASK-ID لبوابة Pre-Execution Gate |
| لم يتم بدء التنفيذ البرمجي | Open | لا يبدأ قبل TASK-ID معتمدة وصريحة من Tera |

---

## 8. آخر ملخص سياق

Readiness Review مكتمل ✅ — المشروع جاهز للتنفيز.
جميع ملفات التحضير (00–10 + 28) معتمدة.
EngineeringAgent مولّد وجاهز للتفويض.
أول مهمة مقترحة: TASK-0001 — Scaffold تقني صغير.
قبل تفويض EngineeringAgent، يجب أن يجتاز TASK-0001 بوابة Pre-Execution Gate.
النطاق الافتراضي الآمن لأول مهمة Next.js + Prisma: Scaffold Next.js + TypeScript + تثبيت Prisma + إنشاء `.env.example` فقط، بدون Prisma models، بدون db push، بدون migration، وبدون اختبار اتصال فعلي.

---

## 9. آخر تحديث

| البند | القيمة |
|---|---|
| Updated By | Tera Agent |
| Update Reason | Readiness Review مكتمل — EngineeringAgent مولّد — تحديث حالة المشروع |
| Next Step | إنشاء TASK-0001، تشغيل Pre-Execution Gate، ثم عرض المهمة المصححة للاعتماد قبل التفويض |
