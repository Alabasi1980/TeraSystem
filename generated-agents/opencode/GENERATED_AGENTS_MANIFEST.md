# GENERATED_AGENTS_MANIFEST.md

## معلومات المانيفست

| البند | القيمة |
|---|---|
| المشروع | نظام إدارة الشيكات (Checks Management System) |
| بيئة العمل | OpenCode |
| تاريخ التوليد | 26 يونيو 2026 |
| تم التوليد بواسطة | Tera Agent |
| المرحلة | التحضير مكتمل — التنفيذ بانتظار الاعتماد |

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

---

## العملاء الذين لم يتم توليدهم — مع السبب

| العميل | سبب عدم التوليد الآن |
|---|---|
| `SolutionArchitectureAgent` | لم يتم توليده؛ ملف `08_TECHNICAL_ARCHITECTURE.md` موجود وتم تثبيت القرار التقني، ولا حاجة لتفعيله الآن |
| `EngineeringAgent` | يجب توليده أو تفعيله قبل أول مهمة تنفيذ برمجي بعد اعتماد صاحب المشروع |
| `QAAndAcceptanceAgent` | لم يتم توليده؛ ملف `10_TESTING_AND_ACCEPTANCE.md` موجود، ويتم توليده لاحقًا عند بدء مراجعات الاختبار الفعلية |
| `DocumentationHandoverAgent` | مطلوب قبل التسليم النهائي فقط |
| `SecurityAgent` | لا يوجد بيانات حساسة أو صلاحيات معقدة أو متطلبات أمنية متقدمة |
| `IntegrationAgent` | لا يوجد تكاملات خارجية في النسخة الأولى |
| `DevOpsDeploymentAgent` | لا يوجد نشر فعلي في هذه المرحلة |
| `PerformanceAgent` | تطبيق صغير — لا متطلبات أداء خاصة |
| `ComplianceAgent` | لا متطلبات قانونية أو تنظيمية |
| `ReportingAnalyticsAgent` | التقارير بسيطة (كشف واحد) — يمكن تغطيتها ضمن UIUX |
| `MaintenanceMigrationAgent` | لا يوجد نظام قائم أو ترحيل بيانات |

---

## ملاحظات إضافية

- تم توليد العملاء التحليلية و`ProjectControlAgent` في مجلد `generated-agents/opencode/` — صيغة الملفات مناسبة لبيئة OpenCode.
- العملاء الحاليون داخل `generated-agents/opencode/` هم **عملاء مولدون للمراجعة والتجهيز فقط**، وليسوا عملاء OpenCode فعّالين بعد.
- لا يصبح هؤلاء العملاء فعّالين داخل OpenCode إلا بعد مراجعتهم واعتمادهم ثم نسخهم إلى `.opencode/agents/`.
- لا يتم نقل أي عميل إلى `.opencode/agents/` إلا عند وجود حاجة فعلية لاستخدامه.
- يمكن استخدام هذه الملفات كمرجع لتفويض المهام لكل عميل فرعي لاحقًا، لكنها لا تعمل كعملاء نشطين من هذا المجلد.
- قبل التنفيذ البرمجي يجب توليد أو تفعيل `EngineeringAgent` فقط، مع تفعيل `ProjectControlAgent` عند الحاجة لتحديث سجلات `project-control/`.
- العملاء الآخرون مثل `QAAndAcceptanceAgent` و`DocumentationHandoverAgent` يؤجلون إلى مراحلهم الفعلية.
- لا يجوز لأي عميل فرعي تعديل الملفات المملوكة لعميل آخر دون توجيه صريح من Tera Agent.
- قبل نقل أي عميل إلى `.opencode/agents/` يجب تضييق `Allowed Sources` و `Allowed Write Targets` حسب المرحلة والمهمة.
- عملاء التحليل لا يقرأون كود التطبيق إلا بتصريح صريح من Tera Agent.
- يجب ألا ينشئ `BusinessWorkflowAgent` الملف `project-preparation/12_BUSINESS_RULES.md` إلا إذا وافق Tera صراحة على إنشاء هذا الملف.
- ✅ قواعد `MVP Constraints` مضمّنة في كل عميل مولد عبر `AGENT_GENERATION_TEMPLATE.md`.
- قبل تنفيذ أي واجهة، يجب أن يحسم Tera مصدر التصميم البصري: تصميم افتراضي من Tera، ملفات Style من المستخدم، أو مرجع خارجي مثل `getdesign.md`.
- عند وجود مصدر تصميم بصري، يحفظ الأصل أو الإشارة إليه داخل `design-source/` وتلخص قواعد التنفيذ في `project-preparation/28_UI_UX_GUIDELINES.md`.
- `project-preparation/PROJECT_RULES.md` هو ملف القواعد المشتركة الخاصة بالمشروع. إذا وجد، يجب قراءته وتمرير قيوده للعملاء الفرعيين في التفويضات المرتبطة.
- `ProjectControlAgent` لا يقرر ولا ينفذ ولا يغلق المهام بنفسه؛ دوره توثيق السجلات تحت قرار Tera فقط.
