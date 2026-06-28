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
| المرحلة الحالية | **MVP Complete ✅ — all 5 screens built and accepted** (Banks S03, Parties S04, Checks S02, Users S05, Nav Hub). All issues resolved. Build PASS on all routes. |
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
| دورة العملاء الفرعيين الموحدة: `Generated Draft` ثم `Activated` ثم `Inactive` عند الحاجة | `tera-system/TeraAgent.md` / `.opencode/agents/tera.md` | معتمد |
| تقليل حمل Tera عبر `ProjectControlAgent` و`ExecutionPreparationAgent` فقط | `tera-system/TeraAgent.md` / `tera-system/TeraSubAgents.md` | معتمد |
| `QualityReviewCoordinatorAgent` مفعّل للمراجعات الدورية متعددة المجالات دون أن يحل محل مراجعات القبول أو القرار النهائي | `tera-system/TeraAgent.md` / `tera-system/TeraSubAgents.md` | معتمد |
| Tera مثبت كـ `Primary Project Orchestrator / Decision Owner` والعملاء المساندون يعملون وفق Trigger Rules لا وفق سلسلة إلزامية ثابتة | `tera-system/TeraAgent.md` / `.opencode/agents/tera.md` | معتمد |
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
| `EngineeringAgent` | مفعل داخل `.opencode/agents/EngineeringAgent.md` | **مُحدّث** — Backend/Logic فقط (actions.ts, Prisma, Auth, Middleware, Config). لا يكتب UI. |
| `FrontendAgent` | مفعل داخل `.opencode/agents/FrontendAgent.md` | **جديد** — واجهة المستخدم (page.tsx, CSS, RTL, Modal, Toast). يعمل مع EngineeringAgent. |
| `RequirementsScopeAgent` | مولّد داخل `generated-agents/opencode/` | غير مفعل — للمراجعة والتجهيز فقط |
| `BusinessWorkflowAgent` | مفعل داخل `.opencode/agents/BusinessWorkflowAgent.md` | جاهز لمراجعة Workflow الشيكات قبل S02 |
| `DataDesignAgent` | مولّد داخل `generated-agents/opencode/` | غير مفعل — للمراجعة والتجهيز فقط |
| `UIUXStructureAgent` | مفعل داخل `.opencode/agents/UIUXStructureAgent.md` | جاهز لتدقيق مواصفات الشاشات قبل FrontendAgent |
| `ProjectControlAgent` | مفعل داخل `.opencode/agents/ProjectControlAgent.md` | جاهز للتحديثات الإدارية لـ `project-control/` |
| `ExecutionPreparationAgent` | مفعل داخل `.opencode/agents/ExecutionPreparationAgent.md` | جديد - يجهز Task Package قبل تفويض التنفيذ، دون قرار أو تنفيذ كود |
| `QualityReviewCoordinatorAgent` | مفعل داخل `.opencode/agents/QualityReviewCoordinatorAgent.md` | جديد - ينسق مراجعات جودة دورية متعددة المجالات ويرفع تقريرًا موحدًا إلى Tera دون تنفيذ أو اعتماد |
| `SecurityAgent` | مفعل داخل `.opencode/agents/SecurityAgent.md` | **مفعّل** — أنجز TASK-0005 (Security Review) بنجاح ✅ |
| `QAAndAcceptanceAgent` | مفعل داخل `.opencode/agents/QAAndAcceptanceAgent.md` | جاهز للمراجعة المستقلة بعد المهام التنفيذية، خاصة شاشة الشيكات S02 |
| `DocumentationHandoverAgent` | مفعل داخل `.opencode/agents/DocumentationHandoverAgent.md` | جاهز للتسليم والتوثيق النهائي |
| `ReportingAnalyticsAgent` | مفعل داخل `.opencode/agents/ReportingAnalyticsAgent.md` | جاهز لتصميم تقرير كشف الشيكات |
| عملاء آخرون | غير مولدين | `IntegrationAgent`, `DevOpsDeploymentAgent`, `PerformanceAgent`, `ComplianceAgent`, `MaintenanceMigrationAgent`, `SolutionArchitectureAgent` — لا حاجة حالياً |

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
| TASK-0001 — TASK-0011 | Closed ✅ | جميع مهام التأسيس والتحضير والأساس التقني مكتملة |
| TASK-0012 | **Accepted ✅** | شاشة إدارة الشيكات S02 — Backend (actions.ts) + Frontend (page.tsx) + Nav activation. Build PASS. Post-Execution Gate PASS. SecurityAgent review completed. QAAndAcceptanceAgent review completed. **Accepted.** |
| TASK-0013 | Closed ✅ | Fix ISSUE-0007: NaN amount bypass — `isNaN()` guard added to check validation. Build PASS. |
| TASK-0014 | Closed ✅ | Fix ISSUE-0008: Invalid date crash — explicit date validation with Arabic error messages. Build PASS. |
| TASK-0015 | **Accepted ✅** | شاشة إدارة المستخدمين S05 (Users Screen) — آخر شاشات MVP. EngineeringAgent (Server Actions, 245 lines) + FrontendAgent (page.tsx, 837 lines) + Nav activation. Build PASS. |
| ISSUE-0003 | Resolved ✅ | Secret exposure cleaned |
| ISSUE-0004 | Resolved ✅ | SEC-001 User Enumeration — fixed in TASK-0007 |
| ISSUE-0005 | Resolved ✅ | SEC-002 Logout cookie — fixed in TASK-0007 |
| ISSUE-0006 | Resolved ✅ | Server-side validation gap fixed in TASK-0011 before Checks S02 |
| ISSUE-0007 | **Resolved ✅** | NaN amount bypass (Medium) — fixed in TASK-0013 |
| ISSUE-0008 | **Resolved ✅** | Invalid date crash (Low) — fixed in TASK-0014 |
| DEC-0004 (Secret Handling Rules) | Active ✅ | قواعد إلزامية جديدة للأسرار في Pre-Execution Gate و Post-Execution Review Gate و handbacks |
| DEC-0011 (requireAdmin Defense in Depth) | Active ✅ | `requireAdmin()` مضافة في `lib/auth.ts`. جميع Server Actions تتحقق من صلاحية ADMIN قبل التنفيذ. |

---

## 9. آخر ملخص سياق

**MVP Complete ✅ — جميع شاشات MVP الخمس مكتملة ومعتمدة.**

1. **Banks S03** (TASK-0007) — CRUD for banks ✅
2. **Parties S04** (TASK-0008) — CRUD for parties ✅
3. **Navigation Hub** (TASK-0009) — Main page with 4 nav cards ✅
4. **Checks S02** (TASK-0012) — Full checks management with create, edit, print, status workflow, filters ✅
5. **Users S05** (TASK-0015) — Admin CRUD for users with activation/deactivation, self-protection ✅

All issues resolved:
- ISSUE-0006: Server-side validation fixed in TASK-0011 ✅
- ISSUE-0007: NaN amount bypass fixed in TASK-0013 ✅
- ISSUE-0008: Invalid date crash fixed in TASK-0014 ✅

Build verification: `npm run build` PASS on all routes ✅.
**All 15 tasks completed. 0 open issues. 0 blockers.**
**المرحلة التالية:** توثيق التسليم (DocumentationHandoverAgent) أو مراجعة خريطة الطريق للمرحلة الثانية.

---

## 10. آخر تحديث

| البند | القيمة |
|---|---|
| Updated By | ProjectControlAgent under Tera direction |
| Update Reason | MVP complete ✅. All 5 screens built and accepted. TASK-0013 (ISSUE-0007 fix), TASK-0014 (ISSUE-0008 fix), TASK-0015 (Users S05) all closed/accepted. All issues resolved. Build PASS on all routes. |
| Next Step | Ready for handover documentation or roadmap review. |
