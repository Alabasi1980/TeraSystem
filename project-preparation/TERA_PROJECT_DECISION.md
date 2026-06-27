# TERA_PROJECT_DECISION.md

# قرار تيرا الافتتاحي للمشروع

## 1. ملخص فهم المشروع

**تطبيق ويب بسيط لإدارة الشيكات** — يسمح بتسجيل الشيكات الصادرة (المدفوعة من المؤسسة) والواردة (المقبوضة من العملاء)، متابعة حالة كل شيك (مسجل، مستلم، مصرف، مرتجع، ملغي)، معرفة تاريخ الاستحقاق، البنك، الجهة الطرفية، المبلغ، وطباعة كشف بسيط.

هذا تطبيق **مستقل بذاته**، وليس جزءًا من ERP متكامل. النسخة الأولى هي MVP لا تحتوي على تسجيل محاسبي تفصيلي أو تكاملات خارجية.

---

## 2. تصنيف حجم المشروع

| البند | القرار |
|---|---|
| **حجم المشروع** | **صغير** |
| سبب التصنيف | نطاق محدود (تسجيل + متابعة + كشف)، موديول واحد، لا تكامل خارجي، لا ERP، MVP |
| مستوى التوثيق المطلوب | **منخفض إلى متوسط** — التوثيق الأساسي يكفي |
| عدد العملاء الفرعيين المتوقع | **4–5 عملاء أساسيين** فقط |

---

## 3. الملفات التي يجب إنشاؤها — القرار المبدئي

بناءً على حجم المشروع (صغير)، سأقرر إنشاء الملفات التالية خلال مرحلة التحضير:

| الملف | سبب إنشائه | الأولوية |
|---|---|---|
| `00_PROJECT_INPUTS.md` | ✅ تم إنشاؤه — تجميع كل المدخلات | عالية |
| `TERA_PROJECT_DECISION.md` | ✅ هذا الملف — قرار تيرا الافتتاحي | عالية |
| `01_PROJECT_BRIEF.md` | تحويل الفكرة إلى ملخص تنفيذي واضح | عالية |
| `02_SCOPE_AND_BOUNDARIES.md` | تحديد حدود المشروع بدقة (داخل/خارج النطاق) | عالية |
| `03_MODULES_AND_FEATURES.md` | تقسيم التطبيق إلى موديول وميزات واضحة | عالية |
| `04_USERS_ROLES_PERMISSIONS.md` | تحديد المستخدمين والأدوار — يؤثر على كل شيء | عالية |
| `05_BUSINESS_WORKFLOWS.md` | تحديد مسارات عمل الشيكات (الحالات والانتقالات) | عالية |
| `06_DATA_MODEL_PREPARATION.md` | تحديد الكيانات (شيك، بنك، جهة، حالة) والعلاقات | عالية |
| `07_SCREENS_AND_UI_STRUCTURE.md` | تحديد الشاشات المطلوبة ووظائفها | عالية |
| `08_TECHNICAL_ARCHITECTURE.md` | تثبيت القرارات التقنية (لغة، DB، Framework) | عالية |
| `09_IMPLEMENTATION_PLAN.md` | خطة تنفيذ عملية مرتبة | متوسطة |
| `10_TESTING_AND_ACCEPTANCE.md` | تحديد اختبارات المشروع ومعايير القبول | متوسطة |
| `11_DELIVERY_AND_HANDOVER.md` | تجهيز التسليم النهائي | منخفضة (لاحقًا) |
| `12_BUSINESS_RULES.md` | توثيق قواعد العمل الخاصة بالشيكات (اختياري — قد تدمج مع 05) | منخفضة |

---

## 4. الملفات غير المطلوبة حاليًا — مع السبب

| الملف | سبب عدم إنشائه الآن |
|---|---|
| `13_REPORTS_AND_DASHBOARDS.md` | الكشف البسيط يمكن توثيقه داخل ملف الشاشات، لا حاجة لملف منفصل |
| `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` | لا يوجد تكامل خارجي في النسخة الأولى |
| `15_SECURITY_AND_ACCESS_CONTROL.md` | لا بيانات حساسة أو صلاحيات معقدة أو متطلبات أمنية متقدمة |
| `16_AUDIT_LOG_AND_ACTIVITY_TRACKING.md` | لا حاجة لتتبع التعديلات في MVP بسيط |
| `17_NOTIFICATIONS_AND_ALERTS.md` | لم تطلب الإشعارات في النسخة الأولى |
| `18_IMPORT_EXPORT_DATA.md` | لم يطلب استيراد أو تصدير بيانات |
| `19_DATABASE_DESIGN.md` | يكفي ملف `06_DATA_MODEL_PREPARATION.md` لمشروع صغير |
| `20_API_CONTRACTS.md` | لا يوجد Frontend/Backend منفصل مؤكد بعد |
| `21_VALIDATION_AND_ERROR_HANDLING.md` | يمكن دمج قواعد التحقق داخل ملفات أخرى |
| `22_DEPLOYMENT_AND_ENVIRONMENTS.md` | لا نشر فعلي مؤكد في هذه المرحلة |
| `23_BACKUP_AND_RECOVERY.md` | لا حاجة فورية — مشروع صغير في مرحلة التطوير |
| `24_CLIENT_REVIEW_NOTES.md` | لا يوجد عميل خارجي بمراجعات متكررة |
| `25_CHANGE_REQUESTS.md` | لا يوجد نطاق ثابت بعد، التغييرات جزء من التأسيس |
| `26_RISKS_AND_ASSUMPTIONS.md` | يمكن توثيق المخاطر داخل ملفات أخرى |
| `27_DECISIONS_LOG.md` | لا حاجة له كملف تحضيري مستقل في مشروع صغير؛ قرارات التشغيل والتنفيذ تسجل في `project-control/DECISIONS_LOG.md` |
| `28_UI_UX_GUIDELINES.md` | التصميم إداري بسيط — يمكن توثيقه في ملف الشاشات |
| `29_SAMPLE_DATA_AND_SEEDING.md` | مفيد لاحقًا ولكن ليس ضروريًا في مرحلة التحضير |
| `30_USER_MANUAL_DRAFT.md` | لاحقًا بعد اكتمال التطبيق |
| `31_MAINTENANCE_AND_SUPPORT.md` | لا صيانة محددة بعد |
| `32_PERFORMANCE_REQUIREMENTS.md` | مشروع صغير — لا متطلبات أداء خاصة |
| `33_MULTI_TENANCY_OR_COMPANY_STRUCTURE.md` | جهة واحدة فقط |
| `34_COMPLIANCE_AND_LEGAL_NOTES.md` | لا متطلبات قانونية أو تنظيمية |
| `35_ROADMAP_AND_FUTURE_PHASES.md` | غير ضروري في هذه المرحلة — قد يضاف لاحقًا |

---

## 5. المعلومات الناقصة

| المعلومة الناقصة | الأثر | متابعة مؤقتًا؟ | الافتراض المؤقت |
|---|---|---|---|
| لغة البرمجة والتقنيات | يؤخر Technical Architecture | نعم | سيتم اقتراحها عند مرحلة 08 |
| قاعدة البيانات | يؤخر Data Design | نعم | سيتم اقتراحها لاحقًا |
| عدد المستخدمين | غير حاسم في MVP | نعم | أقل من 10 |
| نظام الصلاحيات | يؤثر على الشاشات | نعم | مدير + مستخدم (افتراضي) |

---

## 6. المخاطر الأولية

| الخطر | الأثر | الإجراء المقترح |
|---|---|---|
| عدم حسم التقنيات مبكرًا | تأخير في بدء التنفيذ | سيتم اتخاذ قرار في `08_TECHNICAL_ARCHITECTURE.md` |
| توسع النطاق أثناء التحليل (scope creep) | إضافة ميزات خارج MVP | توثيق النطاق في `02_SCOPE_AND_BOUNDARIES.md` وإغلاقه |
| عدم وضوح متطلبات صلاحيات المستخدمين | إعادة تصميم الشاشات لاحقًا | افتراض نموذج صلاحيات بسيط والتأكيد مع صاحب المشروع |

---

## 7. الموديولات المتوقعة (مبدئيًا)

| الموديول | الوصف | الأولوية |
|---|---|---|
| إدارة الشيكات | تسجيل، تعديل، عرض، حذف الشيكات | أساسي |
| إدارة البنوك | تسجيل بيانات البنوك المستخدمة | أساسي |
| إدارة الجهات/الأطراف | العملاء أو الموردون المرتبطون بالشيكات | أساسي |
| متابعة الحالات | تغيير حالة الشيك وتتبعه | أساسي |
| طباعة الكشوفات | عرض وطباعة كشف بالشيكات وفلاتر | أساسي |

---

## 8. العملاء الفرعيون المطلوبون مبدئيًا

بناءً على حجم المشروع (صغير — MVP)، أقترح العملاء التاليين:

| العميل الفرعي | سبب الحاجة | المرحلة |
|---|---|---|
| `RequirementsScopeAgent` | تحليل المتطلبات بدقة، كتابة Project Brief، تحديد النطاق والصلاحيات | تحليل |
| `BusinessWorkflowAgent` | توثيق حالات الشيكات ومسار العمل (مسجل ← مصرف ← ملغي، إلخ) | تحليل |
| `UIUXStructureAgent` | تحديد الشاشات المطلوبة وعلاقتها بالصلاحيات ومسارات العمل | تصميم |
| `DataDesignAgent` | تصميم نموذج البيانات: شيك، بنك، جهة، حالة، علاقاتها | تصميم |
| `SolutionArchitectureAgent` | اقتراح التقنيات المناسبة لمشروع ويب صغير | تصميم |
| `EngineeringAgent` | تنفيذ البرمجة بعد اعتماد التحليل والتصميم | تنفيذ |
| `QAAndAcceptanceAgent` | تحديد اختبارات ومعايير القبل لكل موديول | اختبار |

### عملاء غير مطلوبين حاليًا

| العميل | سبب عدم الحاجة الآن |
|---|---|
| `SecurityAgent` | لا بيانات حساسة أو صلاحيات معقدة |
| `IntegrationAgent` | لا تكاملات خارجية |
| `DevOpsDeploymentAgent` | لا نشر فعلي في هذه المرحلة |
| `PerformanceAgent` | تطبيق صغير — لا متطلبات أداء |
| `ComplianceAgent` | لا متطلبات قانونية |
| `ReportingAnalyticsAgent` | التقارير بسيطة (كشف واحد) — تدمج في UIUX |
| `MaintenanceMigrationAgent` | لا نظام قائم أو ترحيل بيانات |
| `DocumentationHandoverAgent` | مطلوب لاحقًا قبل التسليم، ليس الآن |

---

## 9. ترتيب العمل المقترح

```
المرحلة 1 — التحليل:
  1. إنشاء 01_PROJECT_BRIEF.md (REQ_SCOPE_AGENT)
  2. إنشاء 02_SCOPE_AND_BOUNDARIES.md (REQ_SCOPE_AGENT)
  3. إنشاء 04_USERS_ROLES_PERMISSIONS.md (REQ_SCOPE_AGENT)
  4. إنشاء 05_BUSINESS_WORKFLOWS.md (BUSINESS_WORKFLOW_AGENT)
  5. إنشاء 12_BUSINESS_RULES.md (اختياري — BUSINESS_WORKFLOW_AGENT)

المرحلة 2 — التصميم:
  6. إنشاء 06_DATA_MODEL_PREPARATION.md (DATA_DESIGN_AGENT)
  7. إنشاء 07_SCREENS_AND_UI_STRUCTURE.md (UI_UX_STRUCTURE_AGENT)
  8. إنشاء 08_TECHNICAL_ARCHITECTURE.md (SOLUTION_ARCH_AGENT)

المرحلة 3 — التخطيط للتنفيذ:
  9. إنشاء 09_IMPLEMENTATION_PLAN.md (Tera Agent)
  10. إنشاء 10_TESTING_AND_ACCEPTANCE.md (QA_ACCEPTANCE_AGENT)

المرحلة 4 — التسليم:
  11. إنشاء 11_DELIVERY_AND_HANDOVER.md (بعد الانتهاء)
  12. توثيق إضافي عند الحاجة (DOC_HANDOVER_AGENT)
```

---

## 10. قرار تيرا النهائي

```text
قرار تيرا:
- المشروع: نظام إدارة الشيكات — Web App MVP
- حجم المشروع: صغير
- مستوى التوثيق: أساسي (11 ملفًا تحضيريًا رئيسيًا)

الملفات التي سيتم إنشاؤها (حسب الموافقة):
  00_PROJECT_INPUTS.md ✅ (تم)
  TERA_PROJECT_DECISION.md ✅ (تم)
  01_PROJECT_BRIEF.md
  02_SCOPE_AND_BOUNDARIES.md
  03_MODULES_AND_FEATURES.md
  04_USERS_ROLES_PERMISSIONS.md
  05_BUSINESS_WORKFLOWS.md
  06_DATA_MODEL_PREPARATION.md
  07_SCREENS_AND_UI_STRUCTURE.md
  08_TECHNICAL_ARCHITECTURE.md
  09_IMPLEMENTATION_PLAN.md
  10_TESTING_AND_ACCEPTANCE.md
  11_DELIVERY_AND_HANDOVER.md
  (12_BUSINESS_RULES.md اختياري — يُدمج مع 05 أو يُنشأ عند الحاجة)

الملفات غير المطلوبة الآن:
  جميع الملفات من 13 إلى 35 — أسبابها موثقة أعلاه.

العملاء الفرعيون المطلوبون مبدئيًا (7 عملاء):
  RequirementsScopeAgent, BusinessWorkflowAgent, UIUXStructureAgent,
  DataDesignAgent, SolutionArchitectureAgent, EngineeringAgent,
  QAAndAcceptanceAgent

العملاء غير المطلوبين الآن:
  SecurityAgent, IntegrationAgent, DevOpsDeploymentAgent,
  PerformanceAgent, ComplianceAgent, ReportingAnalyticsAgent,
  MaintenanceMigrationAgent, DocumentationHandoverAgent

توليد العملاء الفرعيين الفعليين:
  لا — سيتم توليدهم بعد الموافقة على هذا القرار
  وعند الحاجة الفعلية لكل عميل في مرحلته.

سبب القرار:
  - المشروع صغير ومحدود النطاق (MVP).
  - التوثيق الأساسي (11 ملفًا + اختياري) يغطي احتياجات التحليل والتصميم والتنفيذ والتسليم.
  - العملاء الأساسيون السبعة يغطون الأدوار المطلوبة دون تضخيم أو تضارب.
  - باقي الملفات والعملاء غير مبررين حاليًا.

الخطوة التالية:
  ⏸ انتظار موافقة صاحب المشروع على القرار قبل بدء إنشاء الملفات وتوليد العملاء.
```

---

**تاريخ القرار:** 26 يونيو 2026  
**صادر عن:** Tera Agent  
**الحالة:** ⏳ بانتظار الاعتماد
---

## 11. بروتوكول ما بعد الاعتماد

عند موافقة صاحب المشروع على القرار، لا يبدأ التنفيذ البرمجي مباشرة.

الخطوة الأولى بعد الاعتماد:

1. ينشئ Tera سجل مهمة رسميًا داخل `project-control/tasks/` ويحصل على `TASK-ID`.
2. يسجل المهمة في `project-control/TASK_REGISTRY.md`.
3. ينشئ Tera مهمة رسمية بصيغة التفويض المعتمدة في `TeraSubAgents.md`.
4. يتم استدعاء `RequirementsScopeAgent` أولًا لإنتاج أو مراجعة ملفات النطاق الأساسية فقط:
   - `01_PROJECT_BRIEF.md`
   - `02_SCOPE_AND_BOUNDARIES.md`
   - `04_USERS_ROLES_PERMISSIONS.md` عند الحاجة.
5. يحدد Tera في التفويض:
   - `Task ID`
   - الهدف.
   - الملفات المرجعية.
   - الملفات المسموح بتعديلها.
   - القيود.
   - معايير القبول.
6. لا يكتب العميل الفرعي في أي ملف غير مصرح به.
7. بعد تسليم النتيجة، يراجع Tera المخرج ويقرر: قبول، رفض، إعادة عمل، أو طلب قرار من صاحب المشروع.
8. يحدث Tera أو `ProjectControlAgent` سجلات `project-control/` حسب القرار.
9. لا يتم إنشاء `03_MODULES_AND_FEATURES.md` أو `09_IMPLEMENTATION_PLAN.md` إلا بواسطة Tera بعد قبول مخرجات النطاق الأساسية.
10. لا يتم الانتقال إلى التنفيذ البرمجي إلا بعد اعتماد خطة التنفيذ ومعايير القبول.

صيغة أول تفويض بعد الاعتماد:

```text
Task ID: TERA-REQ-001
Requested Agent: RequirementsScopeAgent
Stage: Scope Confirmation
Reason for Invocation: Project decision approved; scope files must be produced before planning or implementation.
Objective: Produce the approved project brief and scope boundaries without expanding the MVP.
Reference Files:
- project-preparation/00_PROJECT_INPUTS.md
- project-preparation/PROJECT_RULES.md (if exists)
- project-preparation/TERA_PROJECT_DECISION.md
Allowed Write Targets:
- project-preparation/01_PROJECT_BRIEF.md
- project-preparation/02_SCOPE_AND_BOUNDARIES.md
- project-preparation/04_USERS_ROLES_PERMISSIONS.md
Constraints:
- Follow MVP Constraints.
- Do not add features, screens, agents, or files outside the approved phase.
- Do not start implementation planning.
Expected Outputs:
- Approved brief.
- Approved scope boundaries.
- Missing decisions list, if any.
Acceptance Criteria:
- Scope is clear, bounded, and testable.
- Out-of-scope items are explicit.
- No unnecessary expansion is introduced.
Return Status Required: Done / Blocked / Needs Clarification / Rework Needed
```
