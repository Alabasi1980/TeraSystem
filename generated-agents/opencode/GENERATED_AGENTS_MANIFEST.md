# GENERATED_AGENTS_MANIFEST.md

## معلومات المانيفست

| البند | القيمة |
|---|---|
| المشروع | نظام إدارة الشيكات (Checks Management System) |
| بيئة العمل | OpenCode |
| تاريخ التوليد | 27 يونيو 2026 |
| تم التوليد بواسطة | Tera Agent |
| المرحلة | التحضير مكتمل — التنفيذ معتمد — EngineeringAgent مولّد |

---

## العملاء الذين تم توليدهم

### 1. RequirementsScopeAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/RequirementsScopeAgent.md` |
| المعرف | `REQ_SCOPE_AGENT` |
| الفئة | أساسي |
| سبب التوليد | ضروري لتحليل المتطلبات، كتابة Project Brief، تحديد النطاق والحدود، وتحديد المستخدمين والصلاحيات |
| الملفات المسموح بكتابتها | `project-preparation/01_PROJECT_BRIEF.md`, `project-preparation/02_SCOPE_AND_BOUNDARIES.md`, `project-preparation/04_USERS_ROLES_PERMISSIONS.md` |

### 2. BusinessWorkflowAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/BusinessWorkflowAgent.md` |
| المعرف | `BUSINESS_WORKFLOW_AGENT` |
| الفئة | أساسي |
| سبب التوليد | ضروري لتوثيق حالات الشيكات ومسارات العمل والانتقالات بين الحالات — وهو جوهر تطبيق الشيكات |
| الملفات المسموح بكتابتها | `project-preparation/05_BUSINESS_WORKFLOWS.md`, `project-preparation/12_BUSINESS_RULES.md` (عند الطلب) |

### 3. DataDesignAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/DataDesignAgent.md` |
| المعرف | `DATA_DESIGN_AGENT` |
| الفئة | أساسي |
| سبب التوليد | ضروري لتحليل الكيانات والعلاقات (شيك، بنك، جهة، حالة) قبل التصميم الفني |
| الملفات المسموح بكتابتها | `project-preparation/06_DATA_MODEL_PREPARATION.md` |

### 4. UIUXStructureAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/UIUXStructureAgent.md` |
| المعرف | `UI_UX_STRUCTURE_AGENT` |
| الفئة | أساسي |
| سبب التوليد | ضروري لتحديد الشاشات المطلوبة وهيكل الواجهة وعلاقتها بالصلاحيات ومسارات العمل |
| الملفات المسموح بكتابتها | `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md`, `project-preparation/28_UI_UX_GUIDELINES.md` (بتفويض صريح من Tera عند وجود مصدر تصميم أو طلب Style Guide) |

### 5. ProjectControlAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/ProjectControlAgent.md` |
| المعرف | `PROJECT_CONTROL_AGENT` |
| الفئة | مشروط / إداري |
| سبب التوليد | مطلوب قبل التنفيذ لضبط المهام، النتائج، المراجعات، المشاكل، الفجوات، والقرارات داخل `project-control/` |
| الملفات المسموح بكتابتها | `project-control/TASK_REGISTRY.md`, `project-control/PROJECT_ACTIVITY_LOG.md`, `project-control/ISSUES_AND_GAPS.md`, `project-control/DECISIONS_LOG.md`, `project-control/tasks/*.md` |

### 6. EngineeringAgent (مُحدّث — بعد التقسيم)

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/EngineeringAgent.md` |
| المعرف | `ENGINEERING_AGENT` |
| الفئة | أساسي |
| الدور بعد التقسيم | **Backend + Logic فقط** — Server Actions, Prisma, Business Logic, Auth, Middleware, DB, Config |
| سبب التوليد | تم توليده عند بدء مرحلة التنفيذ البرمجي. لاحقًا تم تحديثه (2026-06-27) ليكون مسؤولاً عن الخلفية والمنطق فقط، وإزالة مسؤولية UI/Styling لصالح FrontendAgent. |
| الملفات المسموح بكتابتها | `actions.ts`, `lib/`, `prisma/`, `middleware.ts`, `.ts` files (وليس `.tsx`). **ممنوع: `page.tsx`, `.css`, `.tsx` files** |
| يعمل مع | FrontendAgent — EngineeringAgent ينتج actions.ts، FrontendAgent يبني page.tsx فوقه |

### 7. SecurityAgent

| البند | القيمة |
|---|---|
| الملف | `.opencode/agents/SecurityAgent.md` |
| المعرف | `SECURITY_AGENT` |
| الفئة | مشروط / مراجعة أمنية |
| سبب التفعيل | أصبح مطلوبًا بعد TASK-0004 لأن المهمة تشمل Auth/JWT/Passwords/Middleware/Permissions، وبعد ظهور ISSUE-0003 المتعلق بتسرب أسرار وتحديث قواعد Post-Execution Review. |
| الملفات المسموح بكتابتها | لا يكتب افتراضيًا. يسلّم تقرير مراجعة إلى Tera فقط؛ يسجل Tera أو ProjectControlAgent التقرير في `project-control/` عند الحاجة. |

---

## العملاء الذين تم توليدهم في الدفعة الثانية (2026-06-27)

### 8. QAAndAcceptanceAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/QAAndAcceptanceAgent.md` |
| المعرف | `QA_ACCEPTANCE_AGENT` |
| الفئة | أساسي |
| سبب التوليد | ضروري لمراجعة شاشة الشيكات (S02) الأكثر تعقيدًا، وللاختبارات المستقلة بعد كل مهمة تنفيذية، وللتسليم النهائي. يسمح بتوزيع العمل: بينما EngineeringAgent ينفذ شاشة جديدة، يراجع QAAndAcceptanceAgent الشاشة السابقة. |
| الملفات المسموح بكتابتها | لا يكتب افتراضيًا. يسلّم تقرير مراجعة إلى Tera فقط. قد يحدّث `project-preparation/10_TESTING_AND_ACCEPTANCE.md` بتفويض صريح. |

### 9. DocumentationHandoverAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/DocumentationHandoverAgent.md` |
| المعرف | `DOC_HANDOVER_AGENT` |
| الفئة | أساسي |
| سبب التوليد | ضروري لتجهيز مستندات التسليم، دليل المستخدم، وتعليمات التشغيل قبل التسليم النهائي. يمكن العمل بالتوازي مع EngineeringAgent في المراحل المتأخرة. |
| الملفات المسموح بكتابتها | `project-preparation/11_DELIVERY_AND_HANDOVER.md`, `project-preparation/30_USER_MANUAL_DRAFT.md`, `project-preparation/31_MAINTENANCE_AND_SUPPORT.md` (بتفويض صريح) |

### 10. ReportingAnalyticsAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/ReportingAnalyticsAgent.md` |
| المعرف | `REPORTING_ANALYTICS_AGENT` |
| الفئة | مشروط |
| سبب التوليد | مفيد لتصميم تقرير كشف الشيكات وتحديد هيكله (فلاتر، أعمدة، صلاحيات) قبل أن ينفذه EngineeringAgent. يتيح توزيع العمل: ReportingAnalyticsAgent يصمم التقرير بالتوازي مع مهام EngineeringAgent الأخرى. |
| الملفات المسموح بكتابتها | `project-preparation/13_REPORTS_AND_DASHBOARDS.md`, `project-preparation/18_IMPORT_EXPORT_DATA.md` (بتفويض صريح) |

---

## العملاء الذين تم توليدهم في الدفعة الثالثة — تقسيم Backend/UI (2026-06-27)

### 11. FrontendAgent (جديد — واجهة المستخدم)

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/FrontendAgent.md` |
| المعرف | `FRONTEND_AGENT` |
| الفئة | أساسي |
| سبب التوليد | تقسيم مسؤولية EngineeringAgent إلى عميلين: EngineeringAgent (Backend/Logic) + FrontendAgent (UI/Styling). FrontendAgent مسؤول عن كل page.tsx، CSS، RTL، Modal، Toast، واتساق التصميم عبر جميع الشاشات وفق `28_UI_UX_GUIDELINES.md`. |
| الملفات المسموح بكتابتها | `app/*/page.tsx`, `app/*/layout.tsx`, `app/globals.css`, `.css` / `.module.css` files. **ممنوع: `actions.ts`, `lib/`, `prisma/`, `middleware.ts`** |
| يعمل مع | EngineeringAgent — EngineeringAgent ينتج actions.ts (Server Actions + Business Logic)، FrontendAgent يبني page.tsx فوقه ويربط الواجهة بالخلفية |
| متى يستخدم | بعد أن ينتج EngineeringAgent ملف actions.ts لشاشة معينة، أو عند تعديل/توحيد واجهة قائمة |

### 12. ExecutionPreparationAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/ExecutionPreparationAgent.md` |
| المعرّف | `EXECUTION_PREPARATION_AGENT` |
| الفئة | مشروط / مساعد رئيسي |
| سبب التوليد | مطلوب لتخفيف الحمل المتكرر عن Tera في تجهيز Task Packages قبل التنفيذ، خاصة مع المهام الأثقل القادمة. يحول قرار Tera إلى حزمة مهمة واضحة جاهزة للمراجعة والبوابة والتفويض، دون أن يقرر أو ينفذ كودًا. |
| الملفات المسموح بكتابتها | `project-control/tasks/*.md` فقط |
| الحالة الحالية | مفعّل داخل `.opencode/agents/ExecutionPreparationAgent.md` |

---

### 13. QualityReviewCoordinatorAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/QualityReviewCoordinatorAgent.md` |
| المعرّف | `QUALITY_REVIEW_COORDINATOR_AGENT` |
| الفئة | مشروط / تنسيق مراجعة |
| سبب التوليد | مطلوب لسد فجوة بين مراجعات نهاية المهام وبين المراجعات الدورية الأوسع. ينسق مراجعات جودة عبر UI/Engineering/Security/QA/Documentation قبل التعمق في مراحل كبيرة أو قبل Release أو عند ظهور debt/inconsistency signals، ثم يجمعها في تقرير واحد يرفعه إلى Tera. |
| الملفات المسموح بكتابتها | لا يكتب افتراضيًا داخل المشروع؛ يسلّم Quality Review Report إلى Tera، ويقوم Tera أو ProjectControlAgent بتوثيقه عند الحاجة |
| الحالة الحالية | مفعّل داخل `.opencode/agents/QualityReviewCoordinatorAgent.md` |

---

### 14. PlanComplianceReviewAgent

| البند | القيمة |
|---|---|
| الملف | `generated-agents/opencode/PlanComplianceReviewAgent.md` |
| المعرّف | `PLAN_COMPLIANCE_REVIEW_AGENT` |
| الفئة | مشروط / مراجعة توافق الخطة |
| سبب التوليد | مطلوب لإضافة طبقة رسمية لمراجعة توافق التنفيذ مع `PROJECT_MASTER_PLAN.md` و`PROJECT_DETAILED_EXECUTION_PLAN.md` واكتشاف البنود المنفذة، المؤجلة، الملغاة، الخارجة عن النطاق، أو التي تحتاج إصلاحًا قبل اعتماد مرحلة أو MVP. |
| الملفات المسموح بكتابتها | لا يكتب افتراضيًا داخل المشروع؛ يسلّم Plan Compliance Report إلى Tera، ويقوم Tera أو `ProjectControlAgent` بتوثيقه عند الحاجة |
| الحالة الحالية | مفعّل داخل `.opencode/agents/PlanComplianceReviewAgent.md` |

---

## العملاء الذين لم يتم توليدهم — مع السبب

| العميل | سبب عدم التوليد الآن |
|---|---|
| `IntegrationAgent` | لا يوجد تكاملات خارجية في النسخة الأولى من MVP. |
| `DevOpsDeploymentAgent` | لا يوجد نشر فعلي أو CI/CD في هذه المرحلة. |
| `PerformanceAgent` | تطبيق صغير — لا متطلبات أداء خاصة. |
| `ComplianceAgent` | لا متطلبات قانونية أو تنظيمية. |
| `MaintenanceMigrationAgent` | لا يوجد نظام قائم أو ترحيل بيانات. |
| `SolutionArchitectureAgent` | ملف `08_TECHNICAL_ARCHITECTURE.md` موجود والقرارات التقنية مستقرة. لا حاجة لتفعيله الآن. |

✅ **جميع العملاء التاليين تم توليدهم مسبقًا:**  
`RequirementsScopeAgent`, `BusinessWorkflowAgent`, `DataDesignAgent`, `UIUXStructureAgent`, `ProjectControlAgent`, `EngineeringAgent`, `SecurityAgent` (مفعّل), `QAAndAcceptanceAgent`, `DocumentationHandoverAgent`, `ReportingAnalyticsAgent`, `FrontendAgent`, `ExecutionPreparationAgent`, `QualityReviewCoordinatorAgent`, `PlanComplianceReviewAgent`

✅ **العملاء المثبتون فعليًا داخل `.opencode/agents/` بعد DEC-0009:**  
`tera`, `EngineeringAgent`, `FrontendAgent`, `ProjectControlAgent`, `ExecutionPreparationAgent`, `QualityReviewCoordinatorAgent`, `PlanComplianceReviewAgent`, `SecurityAgent`, `QAAndAcceptanceAgent`, `BusinessWorkflowAgent`, `UIUXStructureAgent`, `ReportingAnalyticsAgent`, `DocumentationHandoverAgent`

⏸️ **مولدون لكن غير مثبتين حاليًا:**  
`RequirementsScopeAgent`, `DataDesignAgent` — لا توجد حاجة نشطة الآن لتغيير النطاق أو إعادة تصميم البيانات.

---

## ملاحظات إضافية

- تم توليد العملاء في مجلد `generated-agents/opencode/` كنسخ مرجعية/تحضيرية مناسبة لـ OpenCode.
- بعد DEC-0009، تم تثبيت مجموعة مختارة داخل `.opencode/agents/` لاستخدامها مباشرة في المهام القادمة.
- لا يعني وجود نسخة في `generated-agents/opencode/` أنها نشطة تلقائيًا؛ النسخة النشطة هي الموجودة داخل `.opencode/agents/`.
- لا يتم نقل أي عميل إضافي إلى `.opencode/agents/` إلا عند وجود حاجة فعلية لاستخدامه.
- يمكن استخدام ملفات `generated-agents/opencode/` كمرجع لتحديث أو إعادة توليد العملاء النشطين لاحقًا.
- `RequirementsScopeAgent` و`DataDesignAgent` بقيا مولدين فقط لأن النطاق ونموذج البيانات مستقران حاليًا.
- لا يجوز لأي عميل فرعي تعديل الملفات المملوكة لعميل آخر دون توجيه صريح من Tera Agent.
- قبل نقل أي عميل إلى `.opencode/agents/` يجب تضييق `Allowed Sources` و `Allowed Write Targets` حسب المرحلة والمهمة.
- عملاء التحليل لا يقرأون كود التطبيق إلا بتصريح صريح من Tera Agent.
- يجب ألا ينشئ `BusinessWorkflowAgent` الملف `project-preparation/12_BUSINESS_RULES.md` إلا إذا وافق Tera صراحة على إنشاء هذا الملف.
- ✅ قواعد `MVP Constraints` مضمّنة في كل عميل مولد عبر `AGENT_GENERATION_TEMPLATE.md`.
- قبل تنفيذ أي واجهة، يجب أن يحسم Tera مصدر التصميم البصري: تصميم افتراضي من Tera، ملفات Style من المستخدم، أو مرجع خارجي مثل `getdesign.md`.
- عند وجود مصدر تصميم بصري، يحفظ الأصل أو الإشارة إليه داخل `design-source/` وتلخص قواعد التنفيذ في `project-preparation/28_UI_UX_GUIDELINES.md`.
- `project-preparation/PROJECT_RULES.md` هو ملف القواعد المشتركة الخاصة بالمشروع. إذا وجد، يجب قراءته وتمرير قيوده للعملاء الفرعيين في التفويضات المرتبطة.
- `ProjectControlAgent` لا يقرر ولا ينفذ ولا يغلق المهام بنفسه؛ دوره توثيق السجلات تحت قرار Tera فقط.
- كل تسليم من عميل فرعي يجب أن يوثق داخل `project-control/tasks/[TASK-ID].md` قبل تحويل المهمة إلى `Accepted` أو `Closed`.
- إذا لم يكن العميل الفرعي مفوضًا بالكتابة داخل `project-control/`، فيجب على Tera أو `ProjectControlAgent` تسجيل التسليم فور استلامه.
- `QualityReviewCoordinatorAgent` لا يستبدل `QAAndAcceptanceAgent`; الأول للمراجعة الدورية متعددة المجالات، والثاني لمراجعة قبول مهمة/شاشة/Workflow محدد.
- `PlanComplianceReviewAgent` لا يستبدل `ProjectControlAgent` ولا `QAAndAcceptanceAgent` ولا `QualityReviewCoordinatorAgent`; دوره مراجعة توافق التنفيذ مع الخطة والـ roadmap فقط.
- `DocumentationHandoverAgent` يستخدم عند اكتمال مرحلة قابلة للتوثيق أو قبل handoff/release، حتى لا يحمل Tera كامل أعمال التوثيق بنفسه.
- `DocumentationHandoverAgent` لا يستخدم للتسليمات الداخلية العادية بين المهام؛ `Handoff Readiness Gate` يخص جاهزية المرحلة أو Release أو حزمة توثيق/تسليم.
- قبل تعديل أي ملف داخل `.opencode/agents/` يجب التحقق أن النسخة النشطة موجودة فعليًا؛ وإذا كان الملف موجودًا فقط داخل `generated-agents/opencode/` فلا يتم تفعيل Agent جديد لمجرد المزامنة.
- العملاء الفرعيون لا ينشئون ولا يفعّلون ولا يعدّلون ولا يفوّضون Agents آخرين إلا إذا كلفهم Tera صراحةً بذلك ضمن مهمة نظامية.
- `Security Sensitivity Levels` تحدد قبل التفويض، بينما `Independent Review Decision` تؤكد بعد التنفيذ؛ لا يغني أحدهما عن الآخر.

### Unified Activation Policy

- Tera identifies:
  - sub-agents needed now
  - sub-agents likely needed later
- All new sub-agents start as `Generated Draft` inside `generated-agents/opencode/`.
- A generated sub-agent is not active until it is copied into `.opencode/agents/`.
- Before activation, Tera must narrow `Allowed Sources`, narrow `Allowed Write Targets`, confirm non-overlap with current active agents, and record the activation reason.
- Tera may generate an additional sub-agent later whenever a missing specialization, review bottleneck, or useful parallelization opportunity appears.
- After copying a newly activated sub-agent into `.opencode/agents/`, Tera must ask the user to restart the current environment so the activation becomes effective.
- The current minimal support layer around Tera is limited to `ProjectControlAgent` and `ExecutionPreparationAgent`.
- `QualityReviewCoordinatorAgent` is an optional review-coordination layer, not a planning layer and not a replacement for specialist review agents.
- `PlanComplianceReviewAgent` is an optional roadmap-compliance review layer and does not replace project-control, QA, or quality review roles.
- Tera is the `Primary Project Orchestrator / Decision Owner`; helper agents reduce operational load but do not inherit final authority.
- Do not over-delegate small tasks through long helper-agent chains without a clear trigger.
- Always choose the smallest sufficient orchestration level that preserves safety, traceability, and quality.
- `PlanningCoordinatorAgent` remains deferred for larger phases or larger projects.
