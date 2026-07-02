# AGENT_PERMISSION_MODEL.md

# نموذج صلاحيات العملاء الفرعيين — Sub-Agent Permission Model

يحدد هذا الملف **مستويات الصلاحيات** الرسمية لكل عميل فرعي في منظومة Tera.

---

## القاعدة الحاكمة

> **لكل عميل صلاحية محددة مسبقًا. لا يحق لأي عميل تجاوز صلاحيته الافتراضية دون قرار صريح من Tera.**

الصلاحية تحدد:
- ماذا يمكن للعميل أن يفعل.
- ماذا يمكن للعميل أن يكتب.
- ماذا يمكن للعميل أن يقرأ.
- متى يحتاج موافقة إضافية.

---

## 1. هرم الصلاحيات (من الأضعف إلى الأقوى)

```
                          ┌─────────────────────┐
                          │  DEPLOY_WITH_APPROVAL │
                          │  (نشر + Build + Run) │
                          └──────────┬──────────┘
                                     │
                          ┌──────────▼──────────┐
                          │     WRITE_CODE       │
                          │ (كتابة كود + تعديل) │
                          └──────────┬──────────┘
                                     │
                          ┌──────────▼──────────┐
                          │     RUN_TESTS        │
                          │ (تشغيل اختبارات)    │
                          └──────────┬──────────┘
                                     │
                          ┌──────────▼──────────┐
                          │    WRITE_CONTROL     │
                          │ (سجلات التحكم فقط)  │
                          └──────────┬──────────┘
                                     │
                          ┌──────────▼──────────┐
                          │     WRITE_DOCS       │
                          │ (كتابة توثيق وتحضير)│
                          └──────────┬──────────┘
                                     │
                          ┌──────────▼──────────┐
                          │     PLAN_ONLY        │
                          │ (تحليل + خطط فقط)   │
                          └──────────┬──────────┘
                                     │
                          ┌──────────▼──────────┐
                          │     READ_ONLY        │
                          │ (قراءة فقط — لا كتابة)│
                          └─────────────────────┘
```

الصلاحية الأعلى تشمل الصلاحيات الأدنى (إلا إذا نص على غير ذلك).

---

## 2. تعريف كل مستوى صلاحية

### 2.1 READ_ONLY

| البند | القيمة |
|---|---|
| الكود | ❌ لا يقرأ كود التطبيق إلا بإذن صريح |
| ملفات التحضير | ✅ يقرأ ما يسمح له Tera به |
| ملفات التحكم | ✅ يقرأ فقط |
| ملفات project-control | ✅ يقرأ فقط |
| وحدات التخزين الخارجية | ❌ لا يقرأ بدون إذن |
| كتابة أي ملف | ❌ ممنوع |
| تشغيل أدوات | ❌ ممنوع |
| إصدار تقارير | ✅ تقارير داخلية فقط لتيرا |

**متى يستخدم:** للعملاء الذين دورهم مراجعة أو تحليل فقط دون تعديل.

---

### 2.2 PLAN_ONLY

| البند | القيمة |
|---|---|
| قراءة | ✅ مثل READ_ONLY |
| كتابة ملفات تحضير | ✅ خطط وتحليلات فقط |
| كتابة ملفات تحكم | ❌ ممنوع |
| كتابة كود | ❌ ممنوع |
| تشغيل اختبارات | ❌ ممنوع |
| إصدار تقارير | ✅ تقارير تحليلية |

**متى يستخدم:** للعملاء الذين ينتجون تحليلات وخططًا لكن لا ينفذون ولا يتحكمون.

---

### 2.3 WRITE_DOCS

| البند | القيمة |
|---|---|
| قراءة | ✅ كل ما يلزم للمهمة |
| كتابة ملفات تحضير | ✅ نطاقه المحدد فقط |
| كتابة ملفات تصميم | ✅ حسب المهمة |
| كتابة ملفات تحكم | ❌ ممنوع إلا بتفويض |
| كتابة كود | ❌ ممنوع |
| تشغيل اختبارات | ❌ ممنوع |
| إنشاء ملفات جديدة | ✅ ضمن `Allowed Write Targets` فقط |

**متى يستخدم:** للعملاء الذين ينتجون توثيقًا أو تصميمًا أو تحضيرًا، مثل UIVisualDesignerAgent, DocumentationHandoverAgent.

---

### 2.4 WRITE_CONTROL

| البند | القيمة |
|---|---|
| قراءة | ✅ كل project-control |
| كتابة سجلات التحكم | ✅ مسموح |
| كتابة TASK-ID / ISSUE-ID | ✅ إنشاء وتحديث |
| كتابة نشاط | ✅ تسجيل |
| كتابة ملفات تحضير | ❌ إلا بتفويض صريح من Tera |
| كتابة كود | ❌ ممنوع |
| تشغيل اختبارات | ❌ ممنوع |
| تغيير حالة مهمة | ❌ إلا بعد موافقة Tera |

**متى يستخدم:** حصريًا لـ ProjectControlAgent وبعض المهام الإدارية بتفويض.

---

### 2.5 RUN_TESTS

| البند | القيمة |
|---|---|
| قراءة | ✅ كل الملفات اللازمة للاختبار |
| كتابة نصوص اختبارية | ✅ مسموح |
| تشغيل اختبارات | ✅ مسموح |
| كتابة تقارير اختبار | ✅ مسموح |
| كتابة كود تطبيق | ❌ ممنوع |
| كتابة ملفات تحضير | ❌ ممنوع |
| كتابة ملفات تحكم | ❌ إلا بتفويض |
| تعديل بيانات دائمة | ❌ ممنوع |
| حذف بيانات | ❌ ممنوع |

**متى يستخدم:** لـ QAAndAcceptanceAgent وأي عميل يحتاج تشغيل اختبارات.

---

### 2.6 WRITE_CODE

| البند | القيمة |
|---|---|
| قراءة | ✅ كل الملفات اللازمة |
| كتابة كود جديد | ✅ مسموح |
| تعديل كود موجود | ✅ مسموح |
| إنشاء ملفات تطبيق | ✅ مسموح ضمن المهمة |
| تشغيل أوامر build/run | ✅ مسموح |
| تشغيل اختبارات | ✅ مسموح للتحقق |
| كتابة ملفات تحضير | ❌ ممنوع |
| كتابة ملفات تحكم | ❌ ممنوع |
| تعديل project-control | ❌ ممنوع |
| DEPLOY | ❌ ممنوع بدون موافقة إضافية |

**متى يستخدم:** لـ EngineeringAgent وأي عميل تنفيذي.

---

### 2.7 DEPLOY_WITH_APPROVAL

| البند | القيمة |
|---|---|
| كل صلاحيات WRITE_CODE | ✅ |
| نشر على بيئة | ✅ بعد موافقة صريحة |
| تعديل CI/CD | ✅ بعد موافقة صريحة |
| تغيير إعدادات بيئة | ✅ بعد موافقة صريحة |
| الوصول إلى production | ✅ بعد موافقة صريحة لكل مرة |
| تنفيذ migrations في production | ✅ بعد موافقة صريحة وخطة rollback |

**متى يستخدم:** لـ DevOpsDeploymentAgent فقط، وبعد موافقة Tera أو المستخدم لكل عملية نشر على حدة.

---

## 3. مصفوفة الصلاحية الافتراضية لكل عميل

### 3.1 العملاء الأساسيون

| العميل | المعرف | الصلاحية الافتراضية | هل يمكن رفعها؟ | ملاحظة |
|---|---|---|---|---|
| RequirementsScopeAgent | `REQ_SCOPE_AGENT` | `WRITE_DOCS` | إلى PLAN_ONLY إذا كان التحليل فقط | يكتب ملفات النطاق والمتطلبات فقط |
| BusinessWorkflowAgent | `BUSINESS_WORKFLOW_AGENT` | `WRITE_DOCS` | — | يكتب ملفات سير العمل فقط |
| UIUXStructureAgent | `UI_UX_STRUCTURE_AGENT` | `WRITE_DOCS` | — | يكتب هيكل الشاشات فقط |
| UIVisualDesignerAgent | `UI_VISUAL_DESIGNER_AGENT` | `WRITE_DOCS` | إلى READ_ONLY إذا كان مراجعة فقط | يكتب دليل التصميم والتوكينز فقط |
| DataDesignAgent | `DATA_DESIGN_AGENT` | `WRITE_DOCS` | — | يكتب نموذج البيانات فقط |
| SolutionArchitectureAgent | `SOLUTION_ARCH_AGENT` | `WRITE_DOCS` | إلى PLAN_ONLY إذا كان تحليلًا فقط | يكتب المعمارية فقط |
| EngineeringAgent | `ENGINEERING_AGENT` | `WRITE_CODE` | إلى READ_ONLY إذا كانت مراجعة كود فقط | مستوى عالٍ، يجب مراقبته |
| QAAndAcceptanceAgent | `QA_ACCEPTANCE_AGENT` | `RUN_TESTS` | إلى READ_ONLY إذا كان مراجعة فقط | يشغل اختبارات ويكتب تقارير |
| DocumentationHandoverAgent | `DOC_HANDOVER_AGENT` | `WRITE_DOCS` | — | يكتب وثائق التسليم فقط |

### 3.2 العملاء المشروطون

| العميل | المعرف | الصلاحية الافتراضية | هل يمكن رفعها؟ | ملاحظة |
|---|---|---|---|---|
| SecurityAgent | `SECURITY_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` عند توثيق findings | لا يكتب كودًا أبدًا |
| IntegrationAgent | `INTEGRATION_AGENT` | `WRITE_DOCS` | إلى `WRITE_CODE` لتعديل كود التكامل فقط إذا فوضه Tera صراحة | يوثق التكاملات |
| DevOpsDeploymentAgent | `DEVOPS_DEPLOYMENT_AGENT` | `WRITE_CODE` | إلى `DEPLOY_WITH_APPROVAL` عند النشر | يتطلب موافقة لكل نشر |
| PerformanceAgent | `PERFORMANCE_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` عند توثيق التوصيات | يحلل فقط |
| ComplianceAgent | `COMPLIANCE_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` عند توثيق الملاحظات | يحلل فقط |
| ReportingAnalyticsAgent | `REPORTING_ANALYTICS_AGENT` | `WRITE_DOCS` | — | يوثق متطلبات التقارير |
| MaintenanceMigrationAgent | `MAINTENANCE_MIGRATION_AGENT` | `WRITE_DOCS` | — | يخطط للترحيل |
| ProjectControlAgent | `PROJECT_CONTROL_AGENT` | `WRITE_CONTROL` | — | يتحكم في سجلات المشروع فقط |
| ExecutionPreparationAgent | `EXECUTION_PREPARATION_AGENT` | `WRITE_DOCS` | إلى `PLAN_ONLY` إذا ما زالت الخطة غير ناضجة | يجهز Task Packages فقط |
| QualityReviewCoordinatorAgent | `QUALITY_REVIEW_COORDINATOR_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` عند تسليم تقرير فقط | ينسق المراجعة فقط |
| PlanComplianceReviewAgent | `PLAN_COMPLIANCE_REVIEW_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` عند تسليم التقرير فقط | يراجع توافق الخطة فقط |
| DomainResearchAgent | `DOMAIN_RESEARCH_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` عند تسليم التقرير | يبحث فقط |
| DomainExpertAgent | `DOMAIN_EXPERT_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` عند تسليم التحليل | يحلل فقط |

### 3.3 عملاء التعامل مع العملاء الخارجيين

> **ملاحظة نظامية:** `ClientDiscoveryAgent` و `ProposalScopeAgent` و `ClientApprovalReviewAgent` و `ChangeControlAgent` أُزيلوا من هذا الجدول. مسؤولياتهم دُمجت في `TeraClientEngagementAgent` (عميل حوكمة مستقل، ليس تابعاً لـ Tera). راجع `tera-system/TeraClientEngagement.md`.

| العميل | المعرف | الصلاحية الافتراضية | هل يمكن رفعها؟ | ملاحظة |
|---|---|---|---|---|
| TeraClientEngagementAgent | `CLIENT_ENGAGEMENT_AGENT` | `WRITE_DOCS` | لا — يبقى `WRITE_DOCS` | عميل جلسة حوكسة مستقل (ليس Sub-Agent). يدير دورة حياة الزبون وينتج `TERA_HANDOFF_PACKAGE.md`.

### 3.4 عملاء جلسات الحوكمة الرئيسية

هؤلاء يعملون كجلسات OpenCode مستقلة يفتحها المستخدم يدويًا، وليسوا عملاء فرعيين تحت Tera.

| العميل | المعرف | الصلاحية الافتراضية | هل يمكن رفعها؟ | ملاحظة |
|---|---|---|---|---|
| Auditor | `AUDITOR_AGENT` | `READ_ONLY` + `bash: ask` for local Git commit after explicit owner approval. No edit/write unless separately authorized for a specific report file. | إلى `WRITE_CONTROL` لتوثيق تقرير محدد فقط | لا push، لا تعديل كود، لا commit قبل قبول المالك الصريح |
| Monitor | `MONITOR_AGENT` | `READ_ONLY` | إلى `WRITE_DOCS` لتسليم تقرير محدد فقط | يراجع توافق الخطة ولا يصحح التنفيذ |
| Design Reviewer | `DESIGN_REVIEWER_AGENT` | `READ_ONLY` | إلى `RUN_TESTS` للمعاينة/الفحص البصري بعد موافقة المالك | لا ينفذ UI ولا يغير التصميم |

| TeraClientEngagementAgent | "CLIENT_ENGAGEMENT_AGENT" | "WRITE_DOCS" | إلى "READ_ONLY" إذا كان الموقف يحتاج مراجعة فقط، أو إلى "WRITE_CONTROL" لتوثيق سجلات محددة بعد الموافقة | يكتب مسودات وثائق واستبيانات وحزم تسليم فقط. لا يكتب كوداً ولا يعدل تطبيقات ولا يعتمد العقود أو الأسعار النهائية |
---

## 4. رفع الصلاحية أو خفضها

### 4.1 رفع الصلاحية

يتم رفع صلاحية عميل فقط عندما:

1. Tera يقرر أن المهمة تتطلب صلاحية أعلى.
2. يتم توثيق رفع الصلاحية في `TASK-ID` أو `DECISIONS_LOG.md`.
3. الرفع يكون للمهمة الحالية فقط (لا يصبح دائمًا).

مثال:
```
EngineeringAgent يبدأ بـ WRITE_CODE
Tera يحتاجه لنشر fix عاجل → يرفع إلى DEPLOY_WITH_APPROVAL
بعد النشر → يعود إلى WRITE_CODE
```

### 4.2 خفض الصلاحية

يتم خفض صلاحية عميل عندما:

1. المهمة لا تحتاج الصلاحية العالية.
2. هناك خطر من التعديل غير المصرح به.
3. العميل يستخدم للمراجعة فقط.

مثال:
```
EngineeringAgent يُستخدم لمراجعة كود فقط → يخفض إلى READ_ONLY
```

### 4.3 صلاحية افتراضية للأدوات / MCPs

| الأداة | الصلاحية الافتراضية | ملاحظة |
|---|---|---|
| Playwright / Browser MCP | `RUN_TESTS` | لا يعدل كودًا |
| API Testing MCP | `RUN_TESTS` | يختبر APIs فقط |
| Git/GitHub MCP | `READ_ONLY` | الكتابة تتطلب موافقة |
| Database MCP | `READ_ONLY` | يحظر أي تعديل |

---

## 5. قواعد الصلاحية الإضافية

### 5.1 الصلاحية الأقل هي الأصل

```
عند الشك بين صلاحيتين، اختر الصلاحية الأقل.
يمكن رفع الصلاحية لاحقًا بقرار من Tera، لكن لا يمكن استرجاع تغيير غير مصرح به.
```

### 5.2 لا صلاحية دائمة لمشاريع متعددة

```
صلاحية العميل تُحدد لكل مشروع على حدة.
لا تنتقل الصلاحية من مشروع إلى آخر.
```

### 5.3 تسجيل أي تغيير في الصلاحية

```
أي رفع أو خفض للصلاحية يُسجل في `DECISIONS_LOG.md`.
```

### 5.4 صلاحية العرض السريع (Fast Path)

```
Fast Path مسموح بمهام منخفضة المخاطر فقط.
الشرط الأساسي: المستوى الثقة (Trust Level) للعميل يجب أن يكون `Trusted`.
Trust Level 'Restricted' أو 'Suspended' يمنع Fast Path تلقائيًا.
المستوى Trusted يُعاد تقييمه كل 15 مهمة أو عندما يُنتج 2 'Needs Fix' ضمن 5 مهام.
```

### 5.3.1 ملاحظة عن Trust Metadata

```text
Trust Level ≠ Permission Level.

مستوى الثقة يُستخدم لتخطيط التفويض ومتابعة الاعتمادية داخل `SUB_AGENT_STATUS.md`.
لكنه لا يرفع الصلاحية، ولا يمنح أي قبول تلقائي، ولا يكسر قاعدة:
No acceptance without physical review.
```

### 5.3.2 ملاحظة عن Scoped Runtime Override

```text
Scoped Runtime Override ≠ Permission Escalation.

Runtime Override يضبط حدود التفويض داخل المهمة الحالية فقط.
أما رفع/خفض الصلاحية فيبقى خاضعاً لقرار صريح وتوثيق مستقل.
ولا يجوز استخدام Runtime Override لتجاوز الصلاحية أو منح قبول نهائي.
```

### 5.4 صلاحية الأدوات

```
أي أداة أو MCP تُستخدم ضمن مهمة عميل تخضع لصلاحية ذلك العميل.
إذا كانت صلاحية العميل READ_ONLY، فالأداة أيضًا READ_ONLY.
```

### 5.5 صلاحية خاصة: `NO_WRITE`

```
للعملاء الذين يحتاجون قراءة فقط ولا يملكون أي صلاحية كتابة.
مطبق افتراضيًا على DomainResearchAgent و DomainExpertAgent و QualityReviewCoordinatorAgent.
يمكن رفعه إلى `WRITE_DOCS` فقط لتسليم التقرير.
```

---

## 6. الملخص — قاعدة الصلاحية الذهبية

> **العميل لا يملك صلاحية أكثر مما يحتاجه لإنجاز مهمته المحددة.**
>
> Tera يقرر الصلاحية لكل مهمة، ويسجل أي تغيير.
> لا توجد صلاحية دائمة أو شاملة لأي عميل.
