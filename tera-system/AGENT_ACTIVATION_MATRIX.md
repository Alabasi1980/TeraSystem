# AGENT_ACTIVATION_MATRIX.md

# مصفوفة تفعيل العملاء الفرعيين — Sub-Agent Activation Matrix

هذا الملف يحدد **متى يتم تفعيل كل عميل فرعي** في منظومة Tera، ومتى **لا** يتم تفعيله.

---

## القاعدة الحاكمة

> **لا يتم تفعيل العميل لأنه موجود، بل لأنه مطلوب بسبب Trigger واضح.**

Tera هو المسؤول الوحيد عن قرار التفعيل. العميل لا يُفعّل نفسه ولا يُفعّل عملاء آخرين.

---

## 1. أنواع Triggers

| نوع Trigger | وصف | مثال |
|---|---|---|
| `PHASE_GATE` | بداية مرحلة معينة | Phase 4 → Preparation, Phase 6 → Implementation |
| `DOCUMENT_READY` | وجود ملف معين جاهز | `03_MODULES_AND_FEATURES.md` معتمد → DataDesignAgent |
| `DECISION_MADE` | قرار من Tera أو المستخدم | "نحتاج تكامل API خارجي" → IntegrationAgent |
| `COMPLEXITY_SIGNAL` | مؤشر تعقيد | حجم بيانات كبير → PerformanceAgent |
| `RISK_SIGNAL` | مؤشر خطر | بيانات حساسة → SecurityAgent |
| `USER_REQUEST` | طلب مباشر من المستخدم | "أضف Domain Research" → DomainResearchAgent |
| `EXTERNAL_FACTOR` | عامل خارجي | مشروع عميل خارجي → Client Engagement Agents |
| `REVIEW_NEEDED` | حاجة مراجعة | بعد عدة مهام تنفيذية → QualityReviewCoordinatorAgent |
| `PHASE_7_GATE` | بداية Phase 7 | QAAndAcceptanceAgent + DocumentationHandoverAgent |

---

## 2. مصفوفة التفعيل حسب العميل

### 2.1 العملاء الأساسيون

| العميل | المعرف | Trigger التفعيل | المرحلة | متى لا يُفعّل | الحد الأدنى من المدخلات |
|---|---|---|---|---|---|
| RequirementsScopeAgent | `REQ_SCOPE_AGENT` | `DOCUMENT_READY`: بعد `TERA_PROJECT_DECISION.md` أو `00_PROJECT_INPUTS.md` | 2–3 | إذا كانت الفكرة بسيطة وواضحة بالكامل ويمكن لـ Tera كتابة `01_PROJECT_BRIEF.md` مباشرة | `00_PROJECT_INPUTS.md` أو `TERA_PROJECT_DECISION.md` |
| BusinessWorkflowAgent | `BUSINESS_WORKFLOW_AGENT` | `DOCUMENT_READY`: بعد اعتماد `01_PROJECT_BRIEF.md` + `02_SCOPE_AND_BOUNDARIES.md` + وجود workflows | 3–4 | إذا كان التطبيق لا يحتوي أي دورة عمل أو حالات (مثل CRUD بسيط بدون موافقات) | `01_PROJECT_BRIEF.md` + `02_SCOPE_AND_BOUNDARIES.md` |
| UIUXStructureAgent | `UI_UX_STRUCTURE_AGENT` | `DOCUMENT_READY`: بعد اعتماد الموديولات ومسارات العمل | 4 | إذا كان المشروع API-only أو Backend-only بدون واجهة مستخدم | `03_MODULES_AND_FEATURES.md` + `05_BUSINESS_WORKFLOWS.md` |
| UIVisualDesignerAgent | `UI_VISUAL_DESIGNER_AGENT` | `COMPLEXITY_SIGNAL`: وجود Frontend مهم أو مصدر تصميم (Figma, getdesign.md, screenshots) | 4–5 | إذا كان المشروع API-only، أو UI بسيط جدًا بدون متطلبات بصرية | `07_SCREENS_AND_UI_STRUCTURE.md` + مصدر التصميم |
| DataDesignAgent | `DATA_DESIGN_AGENT` | `DOCUMENT_READY`: بعد وضوح الموديولات والعمليات | 4 | إذا كان التطبيق لا يملك بيانات مترابطة (مثل موقع بسيط بمحتوى ثابت) | `03_MODULES_AND_FEATURES.md` + `05_BUSINESS_WORKFLOWS.md` |
| SolutionArchitectureAgent | `SOLUTION_ARCH_AGENT` | `PHASE_GATE`: قبل التنفيذ (Phase 5) | 5 | إذا كان المشروع صغيرًا جدًا والتقنيات محددة مسبقًا بدون قرارات معمارية مؤثرة | `08_TECHNICAL_ARCHITECTURE.md` أو `00_PROJECT_INPUTS.md` |
| EngineeringAgent | `ENGINEERING_AGENT` | `PHASE_GATE`: عند وجود مهمة تنفيذية مع `Pre-Execution Gate: PASS` | 6 | لا يُفعّل بدون `TASK-COD-*` معتمد. لا يُفعّل لتحضير أو تحليل | ملفات التحليل والتصميم المعتمدة + `TASK-ID` + `Pre-Execution Gate: PASS` |
| QAAndAcceptanceAgent | `QA_ACCEPTANCE_AGENT` | `PHASE_GATE`: قبل إعداد خطة التنفيذ وبعد التنفيذ وقبل Phase 7 | 5–6–7 | إذا كانت المهمة بسيطة ومعايير القبول واضحة ويمكن لـ Tera مراجعتها مباشرة | `10_TESTING_AND_ACCEPTANCE.md` أو ملفات المهمة المنفذة |
| DocumentationHandoverAgent | `DOC_HANDOVER_AGENT` | `PHASE_7_GATE`: عند قرب التسليم أو في Phase 7 | 7 | إذا كان المشروع داخليًا small ولن يُسلّم لطرف آخر | ملفات التحليل والتصميم المعتمدة |

---

### 2.2 العملاء المشروطون

| العميل | المعرف | Trigger التفعيل | المرحلة | متى لا يُفعّل | الحد الأدنى من المدخلات |
|---|---|---|---|---|---|
| SecurityAgent | `SECURITY_AGENT` | `RISK_SIGNAL`: بيانات حساسة، Auth، صلاحيات مدفوعات، أسرار، Middleware، Config حرجة | 5–6–7 | إذا لم يكن هناك أي بيانات حساسة أو صلاحيات أو تكاملات خارجية | `04_USERS_ROLES_PERMISSIONS.md` + `08_TECHNICAL_ARCHITECTURE.md` |
| IntegrationAgent | `INTEGRATION_AGENT` | `EXTERNAL_FACTOR`: API خارجي، بوابة دفع، Webhooks، ERP خارجي | 5–6 | إذا كان المشروع لا يتصل بأي خدمة خارجية | `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` أو ما يعادلها |
| DevOpsDeploymentAgent | `DEVOPS_DEPLOYMENT_AGENT` | `EXTERNAL_FACTOR / USER_REQUEST`: نشر فعلي، CI/CD، Docker، Cloud، Domain/SSL | 6–7 | إذا كان المشروع في بيئة محلية فقط أو لا يحتاج نشر رسمي بعد | `22_DEPLOYMENT_AND_ENVIRONMENTS.md` أو `08_TECHNICAL_ARCHITECTURE.md` |
| PerformanceAgent | `PERFORMANCE_AGENT` | `COMPLEXITY_SIGNAL`: حجم بيانات كبير، مستخدمون كثر، SLA، بطء متوقع | 5–6 | إذا كان المشروع صغيرًا وعدد المستخدمين محدودًا | `06_DATA_MODEL_PREPARATION.md` + `32_PERFORMANCE_REQUIREMENTS.md` |
| ComplianceAgent | `COMPLIANCE_AGENT` | `RISK_SIGNAL`: متطلبات قانونية، بيانات شخصية، مالية، قطاع منظم | 5–6–7 | إذا لم يكن المشروع خاضعًا لأي تنظيم أو امتثال | `34_COMPLIANCE_AND_LEGAL_NOTES.md` أو `02_SCOPE_AND_BOUNDARIES.md` |
| ReportingAnalyticsAgent | `REPORTING_ANALYTICS_AGENT` | `COMPLEXITY_SIGNAL`: تقارير كثيرة، Dashboard، KPIs، تصدير | 5–6 | إذا كان التطبيق لا يحتوي تقارير أو لوحات بيانات | `13_REPORTS_AND_DASHBOARDS.md` |
| MaintenanceMigrationAgent | `MAINTENANCE_MIGRATION_AGENT` | `EXTERNAL_FACTOR`: نظام قائم، ترحيل بيانات، Legacy | 5–6–7 | إذا كان المشروع جديدًا بالكامل بدون ترحيل | `31_MAINTENANCE_AND_SUPPORT.md` أو `00_PROJECT_INPUTS.md` |
| ProjectControlAgent | `PROJECT_CONTROL_AGENT` | `REVIEW_NEEDED`: عند الحاجة لتحديث سجلات `project-control` أو فحص اتساق | 4–5–6–7 | إذا لم تكن هناك حاجة لتحديث سجلات متعددة أو فحص اتساق | ملفات project-control الحالية |
| SoftwareDesignerAgent | `SOFTWARE_DESIGNER_AGENT` | `COMPLEXITY_SIGNAL`: مهمة متعددة العملاء، أو تتجاوز 3 ملفات، أو تحمل مخاطر، أو تحتاج Technical Specification | 5–6 | إذا كانت المهمة بسيطة ويمكن لـ Tera تجهيز Technical Specification مباشرة | ملفات التحليل المعتمدة + `TECHNICAL_SPECIFICATION.md` |
| QualityReviewCoordinatorAgent | `QUALITY_REVIEW_COORDINATOR_AGENT` | `REVIEW_NEEDED`: قبل مرحلة تنفيذ كبيرة، أو بعد عدة مهام، أو قبل Release | 5–6–7 | إذا كان المشروع صغيرًا والمهام قليلة ويمكن لـ Tera متابعتها يدويًا | `PROJECT_STATE.md` + `TASK_REGISTRY.md` |
| PlanComplianceReviewAgent | `PLAN_COMPLIANCE_REVIEW_AGENT` | `REVIEW_NEEDED`: نهاية Phase، أو بعد دفعة مهام رئيسية، أو قبل قبول MVP | 5–6–7 | إذا كان التنفيذ متوافقًا بوضوح مع الخطة ولا توجد انحرافات ظاهرة | `PROJECT_MASTER_PLAN.md` + `TASK_REGISTRY.md` |
| DomainResearchAgent | `DOMAIN_RESEARCH_AGENT` | `COMPLEXITY_SIGNAL / USER_REQUEST`: مجال غير مألوف، أو حاجة بحث خارجي | 1–2–3 | إذا كان المجال معروفًا بالكامل أو المستخدم قدم جميع المعلومات المطلوبة | Domain Research Brief من Tera |
| DomainExpertAgent | `DOMAIN_EXPERT_AGENT` | `COMPLEXITY_SIGNAL`: بعد اكتمال Domain Research، أو عند الحاجة لتحليل متقدم | 2–3–4 | إذا لم يُظهر Domain Research نتائج تستدعي تحليلًا إضافيًا | Domain Research Report + Domain Research Brief |

---

### 2.3 عملاء التعامل مع العملاء الخارجيين

| العميل | المعرف | Trigger التفعيل | المرحلة | متى لا يُفعّل | الحد الأدنى من المدخلات |
|---|---|---|---|---|---|
| ClientDiscoveryAgent | `CLIENT_DISCOVERY_AGENT` | `EXTERNAL_FACTOR`: بداية مشروع عميل خارجي | Client Discovery (قبل Phase 1) | إذا كان المشروع داخليًا (غير موجه لعميل خارجي) | `CLIENT_PROFILE.md` + `project-inputs/` |
| ProposalScopeAgent | `PROPOSAL_SCOPE_AGENT` | `EXTERNAL_FACTOR`: بعد اكتمال Client Discovery واعتماد الفهم | Client Discovery (قبل Phase 1) | إذا لم يكتمل Client Discovery بعد | `project-inputs/` كاملة |
| ClientApprovalReviewAgent | `CLIENT_APPROVAL_REVIEW_AGENT` | `EXTERNAL_FACTOR`: قبل إرسال حزمة اعتماد العميل أو قبل Build Mode | ما قبل التنفيذ | إذا لم تكتمل حزمة الاعتماد بعد | `clients/.../client-approval/` |
| ChangeControlAgent | `CHANGE_CONTROL_AGENT` | `EXTERNAL_FACTOR`: طلب تغيير جديد بعد اعتماد النطاق | أي مرحلة بعد اعتماد النطاق | إذا لم يظهر أي طلب تغيير | `11_CHANGE_CONTROL.md` أو طلب التغيير الجديد |

---

## 3. مصفوفة التفعيل حسب نوع المشروع

### 3.1 مشروع صغير (CRUD بسيط)

| العميل | هل يُفعّل؟ | ملاحظة |
|---|---|---|
| RequirementsScopeAgent | اختياري | إذا كانت الفكرة بسيطة وواضحة، يمكن لـ Tera كتابة `01_PROJECT_BRIEF.md` مباشرة |
| BusinessWorkflowAgent | لا | إذا لم يكن هناك workflows |
| UIUXStructureAgent | اختياري | إذا كان هناك واجهة بسيطة |
| UIVisualDesignerAgent | لا | إلا إذا قدم المستخدم ألوانًا أو مصدر تصميم |
| DataDesignAgent | اختياري | إذا كانت البيانات مترابطة |
| SolutionArchitectureAgent | لا | التقنيات محددة مسبقًا |
| EngineeringAgent | نعم | مع `Pre-Execution Gate: PASS` |
| QAAndAcceptanceAgent | اختياري | يمكن لـ Tera مراجعة المهمة مباشرة |
| DocumentationHandoverAgent | لا | |
| SecurityAgent | لا | إلا إذا كان هناك Auth |
| باقي المشروطون | لا | لا triggers كافية |

### 3.2 مشروع متوسط (تطبيق ويب متكامل مع Dashboard)

| العميل | هل يُفعّل؟ | ملاحظة |
|---|---|---|
| RequirementsScopeAgent | نعم | |
| BusinessWorkflowAgent | نعم | إذا كان هناك workflows |
| UIUXStructureAgent | نعم | |
| UIVisualDesignerAgent | اختياري | إذا كان هناك متطلبات بصرية |
| DataDesignAgent | نعم | |
| SolutionArchitectureAgent | نعم | |
| EngineeringAgent | نعم | |
| QAAndAcceptanceAgent | نعم | |
| DocumentationHandoverAgent | اختياري | إذا كان هناك تسليم لطرف آخر |
| SecurityAgent | نعم | إذا كان هناك Auth أو صلاحيات |
| PerformanceAgent | اختياري | إذا توقعنا حجم مستخدمين متوسط |
| ReportingAnalyticsAgent | اختياري | إذا كان هناك Dashboard |
| ProjectControlAgent | اختياري | عند تعدد المهمات |
| SoftwareDesignerAgent | اختياري | عند تعقيد المهمات |

### 3.3 مشروع ERP (نظام تخطيط موارد مؤسسة)

| العميل | هل يُفعّل؟ | ملاحظة |
|---|---|---|
| RequirementsScopeAgent | نعم | |
| BusinessWorkflowAgent | نعم | أساسي في ERP |
| UIUXStructureAgent | نعم | |
| UIVisualDesignerAgent | نعم | إذا كان UI مهمًا |
| DataDesignAgent | نعم | أساسي |
| SolutionArchitectureAgent | نعم | |
| EngineeringAgent | نعم | مع تفعيل متكرر عبر موديولات متعددة |
| QAAndAcceptanceAgent | نعم | |
| DocumentationHandoverAgent | نعم | ERP يحتاج توثيق تسليم كامل |
| SecurityAgent | نعم | ERP يحتوي صلاحيات وبيانات حساسة |
| IntegrationAgent | نعم | تكاملات خارجية متوقعة |
| DevOpsDeploymentAgent | نعم | نشر في بيئات متعددة |
| PerformanceAgent | نعم | حجم بيانات كبير |
| ComplianceAgent | اختياري | حسب المجال |
| ReportingAnalyticsAgent | نعم | ERP يحتوي تقارير كثيرة |
| MaintenanceMigrationAgent | نعم | ERP يحتاج ترحيل بيانات |
| ProjectControlAgent | نعم | إدارة تتبع متقدمة |
| SoftwareDesignerAgent | نعم | مهام متعددة ومعقدة تحتاج Technical Specification |
| QualityReviewCoordinatorAgent | نعم | بعد مجموعات مهام |
| PlanComplianceReviewAgent | نعم | قبل قبول مراحل |
| DomainResearchAgent | اختياري | لمجالات ERP غير المألوفة |
| DomainExpertAgent | اختياري | لتحليل متقدم للمجال |
| Client Engagement Agents | اختياري | إذا كان ERP لعميل خارجي |

### 3.4 مشروع SaaS (منصة خدمية)

| العميل | هل يُفعّل؟ | ملاحظة |
|---|---|---|
| RequirementsScopeAgent | نعم | |
| BusinessWorkflowAgent | نعم | |
| UIUXStructureAgent | نعم | |
| UIVisualDesignerAgent | نعم | UI مهم لـ SaaS |
| DataDesignAgent | نعم | |
| SolutionArchitectureAgent | نعم | |
| EngineeringAgent | نعم | |
| QAAndAcceptanceAgent | نعم | |
| DocumentationHandoverAgent | نعم | توثيق للمستخدمين |
| SecurityAgent | نعم | Auth/Multi-tenant أساسي |
| IntegrationAgent | اختياري | حسب التكاملات |
| DevOpsDeploymentAgent | نعم | نشر مستمر |
| PerformanceAgent | نعم | قابلية التوسع |
| ReportingAnalyticsAgent | اختياري | حسب الاحتياج |
| ProjectControlAgent | اختياري | عند تعدد المهمات |
| SoftwareDesignerAgent | اختياري | عند تعقيد المهمات |

---

## 4. قواعد التفعيل الصارمة

### 4.1 لا تفعيل تلقائي للعملاء

```
تفعيل العميل يتم بقرار من Tera فقط، استنادًا إلى Trigger واضح.
لا يتم تفعيل العميل تلقائيًا لمجرد دخول مرحلة أو وجود ملف جاهز.
```

### 4.2 لا تفعيل بدون مدخلات كافية

```
لا يُفعّل عميل إذا لم تتوفر المدخلات الدنيا المطلوبة.
إذا كانت المدخلات ناقصة، إما:
  - أن يكمل Tera المدخلات بنفسه
  - أو يُفعّل عميلًا آخر لتجهيز المدخلات أولاً
  - أو يطلب من المستخدم إكمالها
```

### 4.3 لا تفعيل عميل موجود بالفعل في مهمة حاليًا

```
لا يتم تفعيل نسخة جديدة من عميل موجود حاليًا في مهمة نشطة،
إلا إذا كان العميل جاهزًا لتسلم مهمة جديدة ومستقلة.
```

### 4.4 لا تفعيل عميل لمتطلبات مستقبلية

```
لا يتم تفعيل عميل بناءً على متطلبات أو سيناريوهات قد تحدث في المستقبل.
التفعيل يكون للحاجة الحالية فقط.
```

### 4.5 عميل واحد لكل اختصاص في كل مرة

```
لا يتم تفعيل عميلين لهما نفس الاختصاص (مثل اثنين EngineeringAgent) في وقت واحد
على نفس الموديول، إلا في حالات المهام المتوازية المستقلة تمامًا.
```

---

## 5. تسجيل قرار التفعيل

عند تفعيل أي عميل فرعي، يسجل Tera في `PROJECT_ACTIVITY_LOG.md`:

```
- Agent ID
- Activation Trigger
- Task ID المستهدف
- المرحلة الحالية
- Brief reason for activation
```

---

## 6. إلغاء التفعيل أو التعطيل المؤقت

إذا تم تفعيل عميل ثم تبين أنه غير مطلوب:

1. يتم إنهاء مهمته الحالية بحالة `Cancelled` مع سبب واضح.
2. يسجل Tera الإلغاء في `PROJECT_ACTIVITY_LOG.md`.
3. لا يُعاد تفعيله إلا عند ظهور Trigger جديد ومستقل.

---

## 7. أمثلة على تطبيق القواعد

### مثال 1: مشروع ERP جديد

```
Trigger: بداية Phase 4 (Preparation)
Tera يقرر: تفعيل RequirementsScopeAgent و BusinessWorkflowAgent و DataDesignAgent
لا يُفعّل: SolutionArchitectureAgent (لأنه Phase 5)، SecurityAgent (لا يوجد موديول حساس بعد)
```

### مثال 2: طلب تغيير من عميل

```
Trigger: طلب إضافة تكامل دفع خارجي
Tera يقرر: تفعيل IntegrationAgent
لا يُفعّل: ChangeControlAgent (لأنه سيقترح فقط، ويمكن لـ Tera إدارة ذلك)
```

### مثال 3: مراجعة بعد 5 مهام تنفيذية

```
Trigger: REVIEW_NEEDED
Tera يقرر: تفعيل QualityReviewCoordinatorAgent
بعد تقريره: قد يُفعّل SecurityAgent إذا وجد findings أمنية
```

---

## 8. الملخص — قاعدة التفعيل الذهبية

> **عند الشك، لا تُفعّل.**
>
> العميل هو أداة متخصصة تُستدعى للحاجة الحالية فقط،
> وليس عضوا دائمًا في فريق المشروع.
