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
| المرحلة الحالية | Phase 3 مكتمل ✅ — TASK-0004 (Auth) + TASK-0005 (Security Review). SecurityAgent: PASS ✅. مستعد لـ Phase 4 (شاشة إدارة الشيكات = TASK-0006). |
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
| `SecurityAgent` | مفعل داخل `.opencode/agents/SecurityAgent.md` | **مفعّل** — أنجز TASK-0005 (Security Review) بنجاح ✅ |
| عملاء آخرون | غير مولدين | سيولّدون عند الحاجة في مراحلهم |

---

## 6. القواعد الإلزامية (PROJECT_RULES.md)

`project-preparation/PROJECT_RULES.md` هو **مرجع إلزامي** يجب مراجعته قبل:
- إنشاء أي TASK جديدة.
- تنفيذ أو تفويض أي TASK لعميل فرعي.
- أي قرار تصميم أو نطاق أو تقني.

يحتوي `PROJECT_RULES.md` حاليًا على قواعد في 4 أقسام:
1. القواعد العامة
2. تتبع المهام (Traceability)
3. مراجعة القواعد قبل التنفيذ (Rules Review Gate)
4. إدارة قاعدة البيانات (Database Migrations)

**لا يجوز الاعتماد على ذاكرة المحادثة بدل قراءة `PROJECT_RULES.md`.**  
يجب أن يحتوي كل ملف TASK على قسم `Context Check` يوضح الملفات المرجعية التي راجعها Tera.

---

## 7. سياسة السياق الحالية

| البند | القرار |
|---|---|
| Full Context | ممنوع افتراضيًا |
| السياق الافتراضي | `PROJECT_STATE.md` + ملفات محددة في المهمة |
| المهام عالية التكلفة | تحتاج موافقة |
| وضع العمل قبل التنفيذ | Plan Mode |
| Build Mode | يحتاج موافقة صريحة |
| Pre-Execution Gate | إلزامي قبل أي تفويض أو تنفيذ |
| Post-Execution Review Gate | إلزامي بعد كل مهمة تنفيذية قبل `Accepted` أو `Closed`، ويشمل مراجعة ملفات `project-control` الأساسية وفحص السجلات لأي Secret Exposure أو خلل اتساق |

---

## 8. المخاطر أو الفجوات المفتوحة

| البند | الحالة | الإجراء |
|---|---|---|
| Readiness Review | مكتمل ✅ | المشروع جاهز للتنفيذ — راجع تقرير المراجعة |
| EngineeringAgent | مولّد ✅ | جاهز للتفويض بعد اجتياز TASK-ID لبوابة Pre-Execution Gate |
| TASK-0001 | Closed ✅ | Scaffold Next.js + Prisma + .env.example |
| TASK-0002 | Closed ✅ | Prisma Data Models — 4 models + prisma.config.ts |
| TASK-0003 | Closed ✅ | إنشاء قاعدة البيانات checks_management + أول Migration عبر `prisma migrate dev` (PostgreSQL 18 + Prisma 7.8.0) |
| TASK-0004 | Closed ✅ | إعداد المصادقة والصلاحيات — Login + JWT + Middleware + Seed (Admin/User roles) |
| TASK-0005 | Closed ✅ | المراجعة الأمنية المستقلة لـ TASK-0004 — SecurityAgent: PASS ✅. لا Blocker. |
| TASK-0006 | Closed ✅ | تحضير مواصفات التنفيذ — workflow-rules.md + screen-spec-s03.md. جاهز لـ TASK-0007. |
| ISSUE-0003 (Secret Exposure) | Resolved ✅ | تم تنظيف التسرب في prisma.config.ts + تحديث قواعد المنظومة — راجع ISSUES_AND_GAPS.md |
| DEC-0004 (Secret Handling Rules) | Active ✅ | قواعد إلزامية جديدة للأسرار في Pre-Execution Gate و Post-Execution Review Gate و handbacks |

---

## 9. آخر ملخص سياق

Readiness Review مكتمل ✅ — المشروع جاهز للتنفيذ.
جميع ملفات التحضير (00–10 + 28) معتمدة.
`project-preparation/PROJECT_RULES.md` معتمد بــ 4 أقسام من القواعد.
EngineeringAgent مولّد وجاهز للتفويض.
TASK-0001 (Scaffold) — **مغلق** ✅.
TASK-0002 (Prisma Data Models) — **مغلق** ✅.
TASK-0003 (Database + Migration) — **مغلق** ✅.
TASK-0004 (Auth + Roles) — **مغلق** ✅.
⚠️ **Security Remediation**: TASK-0003 كشف عن تسرب كلمة مرور حقيقية في fallback داخل `prisma.config.ts`. تم التنظيف وتحديث قواعد المنظومة بالكامل. راجع ISSUE-0003 و DEC-0004.
**إلزامية:** قبل أي TASK جديدة، يجب مراجعة PROJECT_STATE.md و PROJECT_RULES.md.

---

## 10. آخر تحديث

| البند | القيمة |
|---|---|
| Updated By | Tera Agent / Project Owner |
| Update Reason | TASK-0006 (Preparation) complete. workflow-rules.md and screen-spec-s03.md ready. | |
| Next Step | TASK-0007 — SEC fixes (001+002) + شاشة إدارة البنوك (S03) — EngineeringAgent |
