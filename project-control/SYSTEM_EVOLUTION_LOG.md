# SYSTEM_EVOLUTION_LOG.md

## غرض هذا السجل

سجل خاص بتغييرات تطوير منظومة Tera نفسها — ليس لتتبع أعمال تطبيقات العملاء أو مهام المشاريع.

هذا السجل يُستخدم بواسطة `TeraSystemEvolutionAgent` لتسجيل كل تغيير مُنفَّذ على المنظومة بعد موافقة المالك.

---

## صيغة الإدخال

```text
تاريخ: YYYY-MM-DD
معرف التغيير: SCP-YYYY-MM-DD-NNN
مصدر الطلب: User Request / Auditor Report / Monitor Report / Policy Conflict / Research / Other
نوع التغيير: New Agent / Policy Update / Architecture Update / Protocol Change / Agent Improvement / Anti-Bloat / Other
الملفات المعدلة:
- مسار الملف 1
- مسار الملف 2
الملخص:
الموافقة: Majed — Approved / Approved with Conditions
التحقق من الصحة: Validation Passed / Needs Follow-up
المخاطر:
ملاحظات الاسترجاع (Rollback):

### SCP-2026-07-05-045 — TeraDesignReviewer Functional Awareness section (role awareness polish)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-045
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Documentation / Role Awareness / Runtime Polish
الملفات المعدلة:
- UPDATE: tera-system/TeraDesignReviewer.md
الملخص:
تمت إضافة قسم `TeraDesignReviewer Functional Awareness` في TeraDesignReviewer لتعزيز الوعي الوظيفي وحدود مراجعة التصميم قبل قسم التفعيل، مع صقل لغوي وتوضيح دور المراجعة مقابل القبول النهائي.
الموافقة: Majed — Approved
التحقق من الصحة: Pending
المخاطر: منخفضة — لا تغيير في الصلاحيات أو السلوك التنفيذي، فقط توضيح تنظيمي.
ملاحظات الاسترجاع (Rollback):
1. إزالة قسم `TeraDesignReviewer Functional Awareness` من `tera-system/TeraDesignReviewer.md`

### SCP-2026-07-05-046 — ApplicationBlueprintAgent Functional Awareness section (role awareness polish)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-046
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Documentation / Role Awareness / Runtime Polish
الملفات المعدلة:
- UPDATE: .opencode/agents/application-blueprint.md
الملخص:
تمت إضافة قسم `ApplicationBlueprintAgent Functional Awareness` في بداية ملف agent لرفع الوعي الوظيفي وحدود التحويل من handoff إلى blueprint قبل بوابة البدء، مع صقل لغوي وتوضيح دور الـ blueprint مقابل التحضير التنفيذي.
الموافقة: Majed — Approved
التحقق من الصحة: Pending
المخاطر: منخفضة — لا تغيير في الصلاحيات أو السلوك التنفيذي، فقط توضيح تنظيمي.
ملاحظات الاسترجاع (Rollback):
1. إزالة قسم `ApplicationBlueprintAgent Functional Awareness` من `.opencode/agents/application-blueprint.md`

### SCP-2026-07-05-047 — TeraSystemEvolutionAgent Core Functional Roles section (role awareness polish)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-047
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Documentation / Role Awareness / Runtime Polish
الملفات المعدلة:
- UPDATE: .opencode/agents/tera-system-evolution.md
الملخص:
تمت إضافة قسم `Core Functional Roles` في TeraSystemEvolutionAgent لتعزيز الوعي الوظيفي وحدود الحراسة والتطور قبل قسم المسؤوليات الرئيسية، مع صقل لغوي وتوضيح دور الحوكمة التطورية مقابل إدارة العملاء.
الموافقة: Majed — Approved
التحقق من الصحة: Pending
المخاطر: منخفضة — لا تغيير في الصلاحيات أو السلوك التنفيذي، فقط توضيح تنظيمي.
ملاحظات الاسترجاع (Rollback):
1. إزالة قسم `Core Functional Roles` من `.opencode/agents/tera-system-evolution.md`
```

### SCP-2026-07-05-042 — TeraAuditor Core Functional Roles section (role awareness polish)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-042
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Documentation / Role Awareness / Runtime Polish
الملفات المعدلة:
- UPDATE: tera-system/TeraAuditor.md
الملخص:
تمت إضافة قسم `Core Functional Roles` في TeraAuditor لتعزيز الوعي الوظيفي وحدود التدقيق قبل قسم المراجع المعتمدة، مع صقل لغوي وتخفيف تداخل الدور بين التدقيق والحوكمة والتوصية.
الموافقة: Majed — Approved
التحقق من الصحة: Pending
المخاطر: منخفضة — لا تغيير في الصلاحيات أو السلوك التنفيذي، فقط توضيح تنظيمي.
ملاحظات الاسترجاع (Rollback):
1. إزالة قسم `Core Functional Roles` من `tera-system/TeraAuditor.md`
```

### SCP-2026-07-05-043 — TeraAgent Core Functional Roles section (role awareness polish)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-043
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Documentation / Role Awareness / Runtime Polish
الملفات المعدلة:
- UPDATE: tera-system/TeraAgent.md
الملخص:
تمت إضافة قسم `Core Functional Roles` في بداية TeraAgent لتعزيز الوعي الوظيفي وحدود الصلاحية قبل الملفات المرجعية والمسؤوليات الأساسية، مع تنقيح لغوي وصياغة أكثر حوكمة.
الموافقة: Majed — Approved
التحقق من الصحة: Pending
المخاطر: منخفضة — لا تغيير في الصلاحيات أو السلوك التنفيذي، فقط توضيح تنظيمي.
ملاحظات الاسترجاع (Rollback):
1. إزالة قسم `Core Functional Roles` من `tera-system/TeraAgent.md`
```

### SCP-2026-07-05-042 — TCEA Core Functional Roles section (role awareness polish)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-042
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Documentation / Role Awareness / Runtime Polish
الملفات المعدلة:
- UPDATE: .opencode/agents/tera-client-engagement.md
الملخص:
تمت إضافة قسم `Core Functional Roles` في بداية TCEA لتعزيز الوعي الوظيفي وحدود الصلاحية قبل قسم المسؤوليات الأساسية، مع تنقيح لغوي بسيط للنص.
الموافقة: Majed — Approved
التحقق من الصحة: Pending
المخاطر: منخفضة — لا تغيير في الصلاحيات أو السلوك، فقط توضيح تنظيمي.
ملاحظات الاسترجاع (Rollback):
1. إزالة قسم `Core Functional Roles` من `.opencode/agents/tera-client-engagement.md`
```

---

## سجل التغييرات

### Baseline Reset — SCP-2026-07-03-RESET

```text
تاريخ: 2026-07-03
معرف التغيير: SCP-2026-07-03-RESET
مصدر الطلب: User Request (Majed)
نوع التغيير: Anti-Bloat / Historical Reset
الملفات المعدلة:
- project-control/SYSTEM_EVOLUTION_LOG.md
- project-control/ISSUES_AND_GAPS.md
- project-control/TASK_REGISTRY.md
- project-control/tasks/TASK-SYS-ENGINEERING-GOVERNANCE-001.md
الملخص:
Hard Reset كامل للسجلات التاريخية داخل مساحة النظام النشطة، مع الإبقاء على قوالب السجلات فقط كبداية جديدة للمنظومة.
الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed
المخاطر: فقدان السجل الظاهر داخل workspace الحالي مقابل بدء نظيف.
ملاحظات الاسترجاع (Rollback):
استعادة السجلات من Git عند الحاجة.
```

### SCP-2026-07-03-017 — Continuous Improvement Policy

```text
تاريخ: 2026-07-03
معرف التغيير: SCP-2026-07-03-017
مصدر الطلب: User Request (Majed)
نوع التغيير: Policy Addition + Gate Update
الملفات المعدلة:
- CREATE: tera-system/TERA_CONTINUOUS_IMPROVEMENT_POLICY.md
- UPDATE: tera-system/TeraPolicyMap.md
- UPDATE: tera-system/TeraArchitectureMap.md
- UPDATE: tera-system/TeraPreExecutionGate.md (§2.5)
- UPDATE: tera-system/TeraAgent.md (§15)
- UPDATE: tera-system/TeraSubAgents.md (§14.1-14.5 — جميع العملاء الأساسيين)
- UPDATE: tera-system/TeraClientEngagement.md (§13 — Self-Improvement & Gap Reporting)
- UPDATE: .opencode/agents/auditor.md (إضافة مرجع السياسة في قائمة القراءة)
- UPDATE: .opencode/agents/monitor.md (إضافة مرجع السياسة في قائمة القراءة)
- UPDATE: .opencode/agents/design-reviewer.md (إضافة مرجع السياسة في قائمة القراءة)
- UPDATE: .opencode/agents/tera-client-engagement.md (إضافة مرجع السياسة في قائمة القراءة)
- UPDATE: .opencode/agents/tera-system-evolution.md (إضافة مرجع السياسة في قائمة القراءة)
- CREATE: project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-03-017.md
الملخص:
إضافة سياسة التحسين المستمر (Continuous Improvement Policy) كسياسة رسمية في tera-system/.
السياسة توجّه جميع العملاء الأساسيين إلى مراقبة فجوات المنظومة والإبلاغ عنها عبر AGENT_GAPS_LOG.md.
تم تحديث TeraPreExecutionGate.md بإضافة §2.5 (تذكير التحسين المستمر) كخطوة استباقية قبل التنفيذ.
تم تحديث TeraPolicyMap.md و TeraArchitectureMap.md بإضافة مرجع السياسة الجديدة.
تم تحديث TeraAgent.md §15 بربط المرجع بالسياسة الجديدة.
تم تحديث TeraSubAgents.md §14.1-14.5 لإضافة مرجع السياسة لجميع العملاء الأساسيين (Auditor, Monitor, DesignReviewer, TeraSystemEvolutionAgent, TCEA).
تم تحديث TeraClientEngagement.md بإضافة §13 (Self-Improvement & Gap Reporting) مشابهاً لقسم TeraAgent.
تم تحديث ملفات .opencode/agents/ الخمسة (auditor, monitor, design-reviewer, tera-client-engagement, tera-system-evolution) بإضافة السياسة كمرجع إلزامي في قائمة القراءة.
الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed
المخاطر: منخفضة — لا تغيير في سلوك تنفيذي أو صلاحيات.
ملاحظات الاسترجاع (Rollback):
1. حذف tera-system/TERA_CONTINUOUS_IMPROVEMENT_POLICY.md
2. إزالة السطر المضاف من TeraPolicyMap.md
3. إزالة القسم §2.5 من TeraPreExecutionGate.md
4. إزالة المرجع المضاف من TeraAgent.md §15
5. إزالة المرجع المضاف من TeraArchitectureMap.md
6. إزالة الإشارات المضافة من TeraSubAgents.md §14.1-14.5
7. إزالة §13 من TeraClientEngagement.md
8. إزالة السطور المضافة من .opencode/agents/auditor.md
9. إزالة السطور المضافة من .opencode/agents/monitor.md
10. إزالة السطور المضافة من .opencode/agents/design-reviewer.md
11. إزالة السطور المضافة من .opencode/agents/tera-client-engagement.md
12. إزالة السطر المضاف من .opencode/agents/tera-system-evolution.md
```

### SCP-2026-07-04-018 — Understanding Confirmation Gate for TCEA

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-018
مصدر الطلب: AGENT_GAPS_LOG.md (GAP-001) + User Request (Majed)
نوع التغيير: Process Gap Fix + Runtime Sync + Limited Operational Remediation
الملفات المعدلة:
- UPDATE: project-control/AGENT_GAPS_LOG.md
- UPDATE: tera-system/TeraClientEngagement.md
- UPDATE: .opencode/agents/tera-client-engagement.md
- UPDATE: clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/CLIENT_INTAKE.md
- UPDATE: clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/CLIENT_BRIEF.md
- UPDATE: clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/SCOPE_SUMMARY.md
- CREATE: project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-018.md
الملخص:
تم إصلاح فجوة TCEA التي كانت تسمح بالانتقال من Discovery إلى ملفات النطاق دون Understanding Confirmation Gate صريحة.
أضيفت بوابة إلزامية داخل المرجع `TeraClientEngagement.md` وداخل runtime `.opencode/agents/tera-client-engagement.md` تمنع إنتاج CLIENT_BRIEF / SCOPE_SUMMARY / DRAFT_QUOTATION / TERA_HANDOFF_PACKAGE قبل تأكيد Majed للملخص.
كما تم تنفيذ remediation تشغيلية محدودة على التطبيق الحالي: توثيق حالة الفهم داخل `CLIENT_INTAKE.md` كـ pending، ووضع تنبيه حوكمي على `CLIENT_BRIEF.md` و `SCOPE_SUMMARY.md` بأنهما غير baseline حتى confirmation.
تم تحديث GAP-001 إلى `Applied`.
الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed
المخاطر: منخفضة — تضيف Gate قصيرة لكنها تمنع بناء scope على فهم غير مؤكد.
ملاحظات الاسترجاع (Rollback):
1. إزالة Understanding Confirmation Gate من `tera-system/TeraClientEngagement.md`
2. إزالة التحديثات المناظرة من `.opencode/agents/tera-client-engagement.md`
3. إزالة remediation من ملفات التطبيق الحالية إذا لزم
4. إعادة صياغة GAP-001 إذا تقرر سحب التغيير
```

### SCP-2026-07-04-019 — ApplicationBlueprintAgent + Blueprint Confirmation Gate

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-019
مصدر الطلب: User Request (Majed)
نوع التغيير: New Agent + Architecture Update + Pre-Preparation Governance
الملفات المعدلة:
- CREATE: tera-system/TeraApplicationBlueprint.md
- UPDATE: tera-system/TeraPolicyMap.md
- UPDATE: tera-system/TeraArchitectureMap.md
- UPDATE: tera-system/TeraClientEngagement.md
- UPDATE: tera-system/TeraAgent.md
- UPDATE: tera-system/Tera_Project_Preparation_Files.md
- UPDATE: tera-system/TeraPreparationDocumentationGovernance.md
- CREATE: .opencode/agents/application-blueprint.md
- UPDATE: .opencode/agents/tera-client-engagement.md
- UPDATE: .opencode/agents/tera.md
- CREATE: project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-019.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تمت إضافة `ApplicationBlueprintAgent` كعميل جلسة رئيسي مستقل لمرحلة blueprinting فقط بين TCEA و TeraAgent formal preparation.
تم إنشاء مصدر حقيقة جديد يحدد هويته، حدوده، قاعدة `BLOCKED_BY_UNCONFIRMED_HANDOFF`، قاعدة `No Stack Finalization`, المخرجات الرسمية، وضوابط `draft-seeds/`.
تمت إضافة `Blueprint Confirmation Gate` التي تمنع TeraAgent من استخدام `project-preparation/APPLICATION_BLUEPRINT.md` في التحضير الرسمي ما لم تصبح حالته `approved_for_preparation`.
كما تم تحديث الخرائط والـ runtime لربط التدفق الجديد وتوضيح أن `draft-seeds/` ليست baseline ولا صالحة downstream مباشرة.
الموافقة: Majed — Approved with Conditions
التحقق من الصحة: Validation Passed
المخاطر: متوسطة — تضيف Agent جديداً وبوابة جديدة، لكنها مضبوطة بحدود واضحة لمنع التضخم وتداخل الأدوار.
ملاحظات الاسترجاع (Rollback):
1. حذف `tera-system/TeraApplicationBlueprint.md`
2. حذف `.opencode/agents/application-blueprint.md`
3. إزالة مراجع الـ blueprint من `TeraPolicyMap.md` و `TeraArchitectureMap.md`
4. إزالة تحديثات التدفق من `TeraClientEngagement.md` و `TeraAgent.md` و runtime files
5. إزالة قسم Blueprint artifacts من `Tera_Project_Preparation_Files.md`
6. إزالة قواعد pre-baseline blueprint من `TeraPreparationDocumentationGovernance.md`
7. الإبقاء على أي ملفات blueprint موجودة كأرشيف فقط وعدم اعتمادها runtime
```

### SCP-2026-07-04-022 — TCEA Mandatory 13-Domain Client Discovery Framework

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-022
مصدر الطلب: User Request (Majed) + GAP-002
نوع التغيير: Process Governance Upgrade + Runtime Sync + Pricing/Discovery Alignment
الملفات المعدلة:
- UPDATE: tera-system/TeraClientEngagement.md
- UPDATE: .opencode/agents/tera-client-engagement.md
- UPDATE: tera-system/TeraApplicationQuestionBank.md
- UPDATE: tera-system/TeraClientPolicy.md
- UPDATE: tera-system/TeraPricingPolicy.md
- UPDATE: tera-system/TeraApplicationBlueprint.md
- UPDATE: tera-system/TeraPolicyMap.md
- UPDATE: tera-system/TeraArchitectureMap.md
- UPDATE: tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
- UPDATE: .opencode/agents/application-blueprint.md
- UPDATE: project-control/AGENT_GAPS_LOG.md
- CREATE: project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-022.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تمت ترقية TeraClientEngagementAgent بإضافة إطار اكتشاف إلزامي من 13 مجالاً، مع Discovery Completeness Matrix داخل `DISCOVERY_COVERAGE_SUMMARY.md`، وDiscovery Coverage Gate، وQuotation Readiness Gate، وTera Handoff Readiness Gate.
أصبح الانتقال من Discovery إلى CLIENT_BRIEF / SCOPE_SUMMARY / FEATURE_LIST / DRAFT_QUOTATION / TERA_HANDOFF_PACKAGE محكوماً بتغطية إلزامية مرئية وباعتماد Majed.
كما تم الحفاظ على مبدأ منع التضخم عبر قاعدة `Mandatory Coverage ≠ Mandatory Deep Interview`، والسماح بـ Level 1 Preliminary Estimate كنطاق غير ملزم قبل اكتمال التغطية الكاملة، مع حظر Level 2 Draft Quotation قبل جاهزية الاقتباس.
تم كذلك تحديث Question Bank والسياسات المرجعية والتصميم blueprintي ليتوافق مع جودة handoff الجديدة.
الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed
المخاطر: متوسطة — تزيد الحوكمة والوضوح، لكن قد تبطئ بعض المشاريع الصغيرة إذا أسيء تطبيق العمق. تم تخفيف ذلك بقواعد Depth Scaling وملف إضافي واحد فقط.
ملاحظات الاسترجاع (Rollback):
1. إزالة أقسام 13-domain framework والبوابات الجديدة من `TeraClientEngagement.md`
2. إزالة القواعد التشغيلية المناظرة من `.opencode/agents/tera-client-engagement.md`
3. إزالة Discovery Coverage template من `TERA_RUNTIME_TEMPLATES.md`
4. إزالة تحديثات `TeraApplicationQuestionBank.md` و`TeraClientPolicy.md` و`TeraPricingPolicy.md` و`TeraApplicationBlueprint.md`
5. إزالة مرجع الإطار من `TeraPolicyMap.md` وعودة flow في `TeraArchitectureMap.md` إن لزم
6. الإبقاء على أي `DISCOVERY_COVERAGE_SUMMARY.md` موجودة كأرشيف أو دمجها لاحقاً في `CLIENT_INTAKE.md` إذا تقرر سحب التغيير
```

### SCP-2026-07-04-023 — Central Agent Conduct Gate

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-023
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Improvement / Anti-Bloat / Runtime Governance
الملفات المعدلة:
- CREATE: tera-system/TERA_AGENT_CONDUCT.md
- UPDATE: tera-system/TeraPolicyMap.md
- UPDATE: tera-system/TeraArchitectureMap.md
- UPDATE: .opencode/agents/tera.md
- UPDATE: .opencode/agents/tera-client-engagement.md
- UPDATE: .opencode/agents/application-blueprint.md
- UPDATE: .opencode/agents/tera-system-evolution.md
- UPDATE: .opencode/agents/auditor.md
- UPDATE: .opencode/agents/monitor.md
- UPDATE: .opencode/agents/design-reviewer.md
- UPDATE: .opencode/agents/tera-software-designer.md
- UPDATE: project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-023.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم إنشاء ملف مركزي واحد `TERA_AGENT_CONDUCT.md` ليكون بوابة السلوك الإجباري للعملاء الأساسيين، ويحتوي على القواعد الحاسمة المشتركة، Pre-Action Gate، وقاعدة عدم اليقين، مع ربط مختصر بمسار Gap Reporting الرسمي.
تمت إضافة مرجع Conduct Gate قصير في أعلى العملاء الأساسيين والعميل `tera-software-designer.md`، مع إزالة أقسام Self-Improvement المكررة من بعض ملفات runtime والإبقاء على Agent Gap Management الخاص بـ TeraSystemEvolutionAgent.
الهدف هو تقوية الالتزام، تقليل المبادرة غير المصرح بها، وتنظيف التكرار دون تضخيم ملفات العملاء.
الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed
المخاطر: منخفضة — تضيف Gate سلوكية مركزية وقد تزيد التوقفات الوقائية، لكنها تقلل drift وتكرار القواعد.
ملاحظات الاسترجاع (Rollback):
1. حذف `tera-system/TERA_AGENT_CONDUCT.md`
2. إزالة Conduct Gate references من ملفات `.opencode/agents/`
3. استعادة أقسام Self-Improvement المحذوفة إذا تقرر الرجوع
4. إزالة مرجع الملف من `TeraPolicyMap.md` و `TeraArchitectureMap.md`
```

### SCP-2026-07-04-024 — Tera Agent Cleanup Pass

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-024
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Improvement / Anti-Bloat / Formatting Cleanup
الملفات المعدلة:
- UPDATE: tera-system/TeraAgent.md
- UPDATE: .opencode/agents/tera.md
الملخص:
تمت إزالة التكرار المباشر `Final intake rule` من TeraAgent.md، وتصحيح ترقيم الأقسام الفرعية في قسمي قواعد الأدوات/المصادر وسياسة عدد العملاء.
كما تم حذف قسم `Current Verification Task` المؤقت من runtime tera.md لخفض التداخل التنظيمي وإبقاء العميل التشغيلي مختصرًا.
الموافقة: Majed — Approved
التحقق من الصحة: Pending
المخاطر: منخفضة — تعديل تنظيمي محدود بدون تغيير في منطق الأدوار أو الصلاحيات.
ملاحظات الاسترجاع (Rollback):
1. إعادة السطر المكرر في TeraAgent.md إذا لزم.
2. إعادة ترقيم العناوين الفرعية إلى الصيغة السابقة إذا تقرر الرجوع.
3. استعادة قسم Current Verification Task في .opencode/agents/tera.md إذا تقرر الرجوع.
```

### SCP-2026-07-04-025 — Phased Tera Agent Governance Cleanup (Pass A)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-025
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Improvement / Anti-Bloat / Source-of-Truth Stabilization
الملفات المعدلة:
- UPDATE: tera-system/TeraAgent.md
- UPDATE: .opencode/agents/tera.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم تنفيذ Pass A فقط من SCP-025 لتثبيت مصادر الحقيقة للقواعد الحساسة قبل أي تخفيف أوسع.
في `TeraAgent.md` تم تقليص قسم Design Governance إلى القواعد العليا فقط مع إحالة التفاصيل إلى `tera-system/design-system/`، وتقليص قسم Pre-Execution Gate إلى قاعدة الإلزام والربط المرجعي فقط مع إحالة التفاصيل إلى `TeraPreExecutionGate.md` وملفات runtime وprofiles، وتقليص قسم Plan Mode / Build Mode إلى بيان حوكمي مختصر مع إبقاء التشغيل الفعلي في runtime.
وفي `.opencode/agents/tera.md` تم تقليص قسم UI Design Source Protocol إلى القواعد التشغيلية الثلاث العليا فقط مع إبقاء المرجع الرسمي في `tera-system/design-system/`.
الموافقة: Majed — Approved with Checkpoints
التحقق من الصحة: Validation Passed for Pass A only; Pass B/C pending owner review
المخاطر: منخفضة إلى متوسطة — تم تقليل التكرار الحساس بدون حذف المصدر الرسمي للقواعد.
ملاحظات الاسترجاع (Rollback):
1. استعادة الأقسام المختصرة من Git إذا تقرر الرجوع عن Pass A.
2. الإبقاء على Passes B/C متوقفة لحين مراجعة المالك للـ diff.
```

### SCP-2026-07-04-025 — Phased Tera Agent Governance Cleanup (Pass B)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-025
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Improvement / Anti-Bloat / Source Slimming
الملفات المعدلة:
- UPDATE: tera-system/TeraAgent.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم تنفيذ Pass B فقط من SCP-025 لتخفيف `TeraAgent.md` مع الحفاظ على دقته كملف هوية ومرجعية عليا. تم اختصار تفاصيل المراحل 3–7 وتحويلها من خطوات تشغيلية مطولة إلى بنية مختصرة تتضمن: الهدف، المدخلات الحرجة، المخرجات الرسمية، قواعد المنع الكبرى، والمرجع التشغيلي. بقيت التفاصيل الإجرائية في `TERA_RUNTIME_CHECKLISTS.md` و `TERA_RUNTIME_TEMPLATES.md` و `TERA_RUNTIME_PROTOCOLS.md` و `TeraPreExecutionGate.md`.
الموافقة: Majed — Pass B Authorized
التحقق من الصحة: Validation Passed for Pass B only; Pass C not started
المخاطر: منخفضة — تم تخفيف المصدر دون حذف المراجع التشغيلية أو القواعد العليا.
ملاحظات الاسترجاع (Rollback):
1. استعادة كتلة المراحل 3–7 السابقة من Git إذا تقرر الرجوع عن Pass B.
2. إبقاء Pass C متوقفاً حتى قرار المالك.
```

### SCP-2026-07-04-025 — Phased Tera Agent Governance Cleanup (Pass C)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-025
مصدر الطلب: User Request (Majed) — Pass C Authorized
نوع التغيير: Agent Improvement / Anti-Bloat / Runtime Compression
الملفات المعدلة:
- UPDATE: .opencode/agents/tera.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم تنفيذ Pass C (المرحلة الأخيرة) من SCP-025 لضغط ملف runtime `.opencode/agents/tera.md` من 785 سطراً إلى 551 سطراً (انخفاض ~30%) مع الحفاظ على جميع triggers التشغيلية.

التغييرات الرئيسية حسب القسم:
- §2 (System Reference Files): 47→15 سطراً — تحويل القائمة النصية إلى صيغة مجموعات (source of truth / operational) مع الاحتفاظ بجميع الإشارات للملفات المهمة.
- §3 (Runtime Loading Rules): 117→47 سطراً — تحويل قوائم الـ triggers الـ19 من صيغة prose مع bullets إلى جدول مضغوط (Read This File / Before Doing This)، مع بقاء ملخصات Domain Intelligence و Application Discovery.
- §6 (Project Intake Gate): 45→28 سطراً — إزالة تكرار قواعد output location (موجودة في §7)، ضغط القوائم.
- §10 (Decision and Anti-Bloat Rules): 30→27 سطراً — ضغط طفيف مع بقاء جميع معايير القرار.
- §12 (Execution Orchestration Core): 128→48 سطراً — ضغط جوهري: تحويل أمثلة صيغ التسجيل والتقسيم والتشخيص من كتل Markdown كاملة إلى مراجع inline، مع بقاء جميع القواعد التشغيلية (task lifecycle, logging, TASK-ID size, acceptance, issues, self-diagnosis, handback).
- §13 (Safety Gates): بدون تغيير جوهري — الحفاظ على كامل محتوى الأمان.
- §18 (Git Commit & Push): 42→14 سطراً — ضغط الخطوات والقواعد إلى صيغة موجزة مع بقاء جميع قواعد الأمان والموافقة.
الموافقة: Majed — Pass C Authorized via approval gate
التحقق من الصحة: Validation Passed — جميع الـ triggers التشغيلية محفوظة، أزواج code blocks متوازنة، جميع الأقسام مرقمة.
المخاطر: منخفضة — ضغط تنسيقي فقط بدون تغيير في القواعد أو الصلاحيات أو الأدوار.
ملاحظات الاسترجاع (Rollback):
1. استعادة النسخة السابقة من `.opencode/agents/tera.md` من Git إذا تقرر الرجوع عن Pass C.
```

### SCP-2026-07-04-026 — Compliance Record لكل TASK-ID

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-026
مصدر الطلب: GAP-003 (TeraAgent)
نوع التغيير: Agent Process Improvement / Policy Update / Anti-Bloat
الملفات المعدلة:
- UPDATE: tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md (قاعدة Compliance Record + Mid-Task Checkpoint)
- UPDATE: tera-system/runtime/TERA_RUNTIME_TEMPLATES.md (قالب §33)
- UPDATE: .opencode/agents/tera.md §12 (تحديث قاعدة الإغلاق + Mid-Task Checkpoint)
- UPDATE: .opencode/agents/monitor.md (إضافة مسؤوليات التحقق)
- UPDATE: project-control/AGENT_GAPS_LOG.md (GAP-003 → Applied)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم إضافة **Compliance Record إلزامي** لكل TASK-ID كشرط للإغلاق، و **Mid-Task Compliance Checkpoint** كحماية أثناء التنفيذ، لسد فجوة غياب سجل امتثال موحد يربط Handback + Git diff + القواعد + الانحراف التراكمي داخل المهمة الواحدة.

التغييرات:
1. `TERA_RUNTIME_PROTOCOLS.md`: 
   - قاعدة إلزامية — لا إغلاق TASK-ID بدون Compliance Record (Handback + gates + Git diff matching).
   - Mid-Task Compliance Checkpoint — بعد كل خطوة منطقية من Tool Calls، Tera يتوقف ويسجل Checkpoint ذاتي (Allowed Write Targets + No secrets + In scope). سطر واحد.
2. `TERA_RUNTIME_TEMPLATES.md` §33: قالب Compliance Record بجدول 9 بنود (Pre-Execution Gate, Write Targets, Secrets, Design Source, Post-Execution Review, Activity Log, Handback, Git diff match, Commands) مع PASS/FAIL/N/A و Verified By.
3. `tera.md §12`: 
   - تحديث قاعدة الإغلاق من شرط واحد (Post-Execution Gate PASS) إلى 3 شروط: Gate PASS + Compliance COMPLIANT + Handback recorded.
   - إضافة Mid-Task Compliance Checkpoint كفقرة جديدة.
4. `monitor.md`: إضافة مسؤوليتين — التحقق من اكتمال Compliance Record، والمطابقة بين Handback و Git diff.

الـ Compliance Record داخل ملف TASK-ID نفسه — لا ملفات جديدة. Mid-Task Checkpoint = سطر واحد فقط.
الموافقة: Majed — Approved
التحقق من الصحة: Implementation Complete — جميع التعديلات متسقة. فجوة 15% مغلقة بـ 3 طبقات دفاع (Mid-Task + Compliance Record + Monitor audit).
المخاطر: منخفضة — التغيير يوثق إجراءات موجودة أصلاً، لا يغير صلاحيات أو أدوار.
ملاحظات الاسترجاع (Rollback):
1. إزالة قاعدة Compliance Record و Mid-Task Checkpoint من TERA_RUNTIME_PROTOCOLS.md.
2. إزالة القالب §33 من TERA_RUNTIME_TEMPLATES.md.
3. إعادة قاعدة الإغلاق في tera.md §12 إلى الصيغة السابقة وإزالة Mid-Task Checkpoint.
4. إزالة مسؤوليات Monitor من monitor.md.
```

### SCP-2026-07-04-027 — TCEA 6 Improvements (GAP-004)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-027
مصدر الطلب: GAP-004 (TCEA — 6 اقتراحات تحسين)
نوع التغيير: Agent Improvement / Policy Update / Anti-Bloat
الملفات المعدلة:
- UPDATE: tera-system/TeraClientEngagement.md (5 أقسام)
- UPDATE: tera-system/runtime/TERA_RUNTIME_TEMPLATES.md (قالب §35)
- UPDATE: project-control/AGENT_GAPS_LOG.md (GAP-004 → Applied)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم تنفيذ 6 تحسينات في ملف TCEA المصدر والملفات المرتبطة بناءً على اقتراحات TCEA نفسه:

1. §3.6.1 (Handoff Readiness Gate): استبدال قائمة الـ 17 بند المنفصلة بإشارة مباشرة إلى §6.2 (25 حقلاً) — يمنع تضارب القائمتين مستقبلاً.
2. §3.2.3 (Discovery Completeness Matrix): إضافة ملاحظة أن Domain 13 يحتاج تغطية 3 جوانب داخلية على الأقل (قبول + ميزانية + ضمان/صيانة) — بدون تقسيم الـ Framework نفسه.
3. §3.2.4 (Discovery Coverage Gate): إضافة قاعدة — إذا تغيرت حالة Domain بعد اعتماد الملف، يجب التحديث وإعادة العرض على Majed.
4. §3.2.5 (Depth Scaling Rule): إضافة Question Budget — Small 10-15, Medium 20-35, Complex deeper.
5. §5.2 (Clarification): إضافة مسار توضيح لـ ApplicationBlueprintAgent بنفس آلية TeraAgent (CLARIFICATION_REQUEST.md → Majed → TCEA).
6. TERA_RUNTIME_TEMPLATES.md §35: إضافة قالب Discovery Coverage Summary بجدول 13 مجالاً + قرار التغطية.
الموافقة: Majed — Approved (عبر Question Flow)
التحقق من الصحة: Implementation Complete — جميع التغييرات في ملفات موجودة، لا ملفات جديدة.
المخاطر: منخفضة — تغييرات دقيقة في ملف مصدر واحد + قالب، لا تغيير في صلاحيات أو أدوار.
ملاحظات الاسترجاع (Rollback):
1. إزالة التغييرات من §§3.2.3, 3.2.4, 3.2.5, 3.6.1, 5.2 في TeraClientEngagement.md.
2. إزالة قالب §35 من TERA_RUNTIME_TEMPLATES.md.
```

### SCP-2026-07-04-028 — TCEA Self-Check + Uncertainty Protocol + Monitor Discovery Audit (GAP-005)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-028
مصدر الطلب: GAP-005 (اعتراف TCEA بفجواته الهيكلية + تحليل TeraSystemEvolutionAgent)
نوع التغيير: Agent Process Improvement / Policy Gap Closure / Anti-Bloat
الملفات المعدلة:
- UPDATE: tera-system/TeraClientEngagement.md (§3.2.6 Self-Check + §3.2.7 Uncertainty Protocol)
- UPDATE: tera-system/runtime/TERA_RUNTIME_TEMPLATES.md §35 (3 أعمدة جديدة)
- UPDATE: .opencode/agents/monitor.md (Random Discovery Audit)
- UPDATE: project-control/AGENT_GAPS_LOG.md (GAP-005 → Applied)
- UPDATE: project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-028.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم تطبيق 3 تحسينات نظامية بناءً على اعتراف TCEA بفجواته وبعد تحليل TeraSystemEvolutionAgent:

1. Self-Check Protocol (§3.2.6): قبل "Complete" لأي Domain، يجب توثيق المصدر (Majed/Websearch/Inference/Unknown)،
   تأكيد Majed (Yes/No/Partially)، وخطورة الخطأ (L/M/H). القاعدة: Inference/Unknown + High = ممنوع Complete.

2. Uncertainty Protocol (§3.2.7): صلاحية إلزامية لـ TCEA ليقول "لا أعرف" في 3 حالات (مصدر غير مؤكد بخطورة عالية،
   معلومة أحدث من 2025، طلب غير مألوف). آلية: UNCERTAINTY_NOTICE + توقف + رفع لـ Majed.
   Websearch متاح دائماً عند عدم التأكد (بدون انتظار موافقة).

3. Monitor — Random Discovery Audit: تمديد monitor.md ليشمل مراجعة DISCOVERY_COVERAGE_SUMMARY.md بأمر Majed
   (كشف domains Complete بمصدر Inference/Unknown + High risk).

جميع التغييرات في ملفات موجودة — لا ملفات جديدة، لا عملاء جدد، لا MCPs.
الموافقة: Majed — Approved ("أوصي بالتنفيذ فوراً")
التحقق من الصحة: Implementation Complete — Anti-Bloat Gate PASS (0 ملفات جديدة).
المخاطر: منخفضة — تحسينات في ملفات موجودة فقط، لا تغيير في صلاحيات أساسية.
ملاحظات الاسترجاع (Rollback):
1. إزالة §§3.2.6, 3.2.7 من TeraClientEngagement.md.
2. إزالة الأعمدة 3 الجديدة من §35 في TERA_RUNTIME_TEMPLATES.md.
3. إزالة فقرة Random Discovery Audit من monitor.md.
```

### SCP-2026-07-04-029 — Fulfilling SCP-017 Claims (CI Policy Ref + §14 Gap Reporting)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-029
مصدر الطلب: Blocked Items (unfulfilled SCP-017 claims identified during system audit)
نوع التغيير: Policy Reference Completion / Gap Closure
الملفات المعدلة:
- UPDATE: tera-system/TeraAgent.md (إضافة §39 Continuous Improvement & Gap Reporting)
- UPDATE: tera-system/TeraSubAgents.md (إضافة §14 Gap Reporting & Continuous Improvement)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم إكمال مطالبتين غير محققتين من SCP-017:

1. TeraAgent.md §39: إضافة مرجع رسمي لـ TERA_CONTINUOUS_IMPROVEMENT_POLICY.md مع 4 قواعد:
   - تذكير العملاء قبل كل تفويض
   - فحص Handback لفجوات نظامية
   - التزام TeraAgent نفسه بالإبلاغ
   - عدم تسجيل تفاصيل صغيرة

2. TeraSubAgents.md §14 (14.1-14.5): إضافة قسم كامل لـ Gap Reporting:
   - 14.1: الإشارة إلى TERA_CONTINUOUS_IMPROVEMENT_POLICY.md
   - 14.2: تعريف AGENT_GAPS_LOG.md كسجل رسمي
   - 14.3: 6 أنواع من الفجوات التي يجب الإبلاغ عنها
   - 14.4: آلية الإبلاغ (3 خطوات)
   - 14.5: قاعدة مهمة (لا تعطيل، لا تفاصيل صغيرة)
الموافقة: Majed — Approved ("نفذ")
التحقق من الصحة: Implementation Complete — 0 ملفات جديدة، 0 عملاء جدد.
المخاطر: منخفضة — إضافة مراجع ووعي فقط، لا تغيير في صلاحيات.
ملاحظات الاسترجاع (Rollback):
1. إزالة §39 من TeraAgent.md.
2. إزالة §14 من TeraSubAgents.md.
```

### SCP-2026-07-04-030 — Replace ExecutionPreparationAgent with SoftwareDesignerAgent

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-030
مصدر الطلب: قرار Majed — إزالة EPA واستبداله بـ SDA (بعد تحليل المخاطر)
نوع التغيير: Architecture Cleanup / Agent Replacement / Cross-System Update
الملفات المعدلة (7 files, 17 changes):
- UPDATE: tera-system/AGENT_ACTIVATION_MATRIX.md (4 تحديثات: تعريف SDA + Medium/ERP/SaaS)
- UPDATE: tera-system/AGENT_PERMISSION_MODEL.md (استبدال صف EPA بـ SDA)
- UPDATE: tera-system/TeraAgent.md (استبدال EPA بـ SDA في قائمة Helper Agents)
- UPDATE: tera-system/TeraSubAgents.md (3 تحديثات: سطر 94 + سطر 1014 + استبدال §6.9 بالكامل)
- UPDATE: tera-system/TeraPreExecutionGate.md (تحديث reference + قاعدة التحضير)
- UPDATE: tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md (4 تحديثات: Decision Matrix + Bad pattern + Escalation + Helper limits)
- UPDATE: project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-030.md
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
إزالة كل reference لـ ExecutionPreparationAgent من الملفات النظامية (14 reference)
واستبدالها بـ SoftwareDesignerAgent (9 references جديدة + وصف كامل للدور).

التغييرات الرئيسية:
1. AGENT_ACTIVATION_MATRIX: تعريف SDA مع شرط التفعيل للمهام المعقدة (مستوى 5-6)
2. AGENT_PERMISSION_MODEL: SDA صلاحية WRITE_DOCS (Technical Specification)
3. TeraAgent: SDA في قائمة Helper Agents المعتمدين
4. TeraSubAgents: §6.9 كامل لـ SDA بدل EPA مع Technical Specification
5. TeraPreExecutionGate: SDA بدل EPA في reference + قاعدة التحضير
6. TERA_RUNTIME_PROTOCOLS: SDA بدل EPA في 4 مسارات تفويض

تم الحفاظ على السجلات التاريخية: RESPONSE_TO_TEAM_REVIEW, SCP-016, tera-software-designer.md
الموافقة: Majed — Approved ("ضم المتبقي وابدأ التنفيذ")
التحقق من الصحة: Implementation Complete — Anti-Bloat Gate PASS.
المخاطر: منخفضة — SDA موجود فعلياً في `.opencode/agents/` ويعمل، التعديلات تجعل النظام متسقاً مع الواقع.
ملاحظات الاسترجاع (Rollback):
1. إعادة EPA إلى AGENT_ACTIVATION_MATRIX.md (4 أماكن)
2. إعادة EPA إلى AGENT_PERMISSION_MODEL.md
3. إعادة EPA إلى TeraAgent.md Helper list
4. إعادة EPA في TeraSubAgents.md (3 أماكن)
5. إعادة EPA في TeraPreExecutionGate.md (مكانين)
6. إعادة EPA في TERA_RUNTIME_PROTOCOLS.md (4 أماكن)
```

### SCP-2026-07-04-031 — ABA Quality & Honesty Gates (بوابة النزاهة + مؤشرات الانحراف + التدقيق الذاتي)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-031
مصدر الطلب: Self-assessment من ApplicationBlueprintAgent + تحليل TeraSystemEvolutionAgent
نوع التغيير: Agent Capability Improvement / Policy Addition / Anti-Bloat
الملفات المعدلة (3 files):
- UPDATE: tera-system/TeraApplicationBlueprint.md (إضافة 3 أقسام + تعديل مخرجات + إعادة ترقيم)
- UPDATE: .opencode/agents/application-blueprint.md (مزامنة 5 تغييرات من المصدر)
- UPDATE: project-control/AGENT_GAPS_LOG.md (إضافة ABA + SDA للشمل + GAP-006)
الملخص:
تمت إضافة 3 ضوابط هيكلية لـ ApplicationBlueprintAgent بناءً على تحليله الذاتي لفجواته:

1. بوابة النزاهة (§2) — Honesty Protocol (توقف عند نقص المعلومات، وثق، اسأل Majed) +
   "لا أعلم" كخيار رسمي + Pacing Mandate (الدقة أهم من السرعة).

2. مؤشرات الانحراف (§11) — 4 محفزات توقف إجباري (تضارب المصادر، تغطية غير كافية،
   توصية بدون بيانات، تجاوز الدور).

3. بوابة التدقيق الذاتي (§13) — مراجعة كل توصية مقابل مصدرها، تعليم الافتراضات،
   تقييم ثقة كل قسم (High/Medium/Low)، حظر التسليم عند Low confidence.

4. تعديل المخرجات — BLUEPRINT_OPEN_QUESTIONS.md من اختياري إلى إلزامي عند عدم اليقين.

5. تحديث AGENT_GAPS_LOG.md — إضافة ApplicationBlueprintAgent و SoftwareDesignerAgent
   لقائمة العملاء + تسجيل GAP-006 كـ Applied.

تم رفض طلب إنشاء ملف TERA_AGENT_QUALITY_CONTROL.md (Anti-Bloat — دُمج المحتوى في الملفات الموجودة).
الموافقة: Majed — Approved ("نفذ واعطني تقرير")
التحقق من الصحة: Implementation Complete — Anti-Bloat Gate PASS (0 ملفات جديدة).
المخاطر: منخفضة — ضوابط في ملفات موجودة، لا تغيير في صلاحيات أو أدوار.
ملاحظات الاسترجاع (Rollback):
1. TeraApplicationBlueprint.md: حذف §2, §11, §13 وإعادة ترقيم الأقسام إلى §1-13.
2. application-blueprint.md: إزالة الإضافات (Honesty Gate, Deviation Detectors, Self-Verification Gate)
   وإعادة BLUEPRINT_OPEN_QUESTIONS.md إلى اختياري فقط.
3. AGENT_GAPS_LOG.md: إزالة ABA و SDA من قائمة الشمل، حذف GAP-006.
```

### SCP-2026-07-04-032 — DesignReviewer Improvements (معاينة + توكينز + Source of Truth + تنظيف مراجع)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-032
مصدر الطلب: تحليل DesignReviewer الذاتي + تحليل TeraSystemEvolutionAgent + موافقة Majed
نوع التغيير: Agent Capability Improvement / Stale Reference Cleanup / Anti-Bloat
الملفات المعدلة (8 files, 1 new):
- UPDATE: .opencode/agents/design-reviewer.md (webfetch: allow, browser protocol, Token Verification, Source of Truth ref)
- CREATE: tera-system/TeraDesignReviewer.md (Source of Truth لناقد — 12 قسماً)
- UPDATE: tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md (تحديث §11)
- UPDATE: .opencode/agents/monitor.md (إزالة WORKSPACE_GOVERNANCE_MODEL.md)
- UPDATE: .opencode/agents/auditor.md (إزالة WORKSPACE_GOVERNANCE_MODEL.md)
- UPDATE: opencode.json (تفعيل Playwright MCP; `browser: true` جرّبت ثم أُزيلت — OpenCode لا تعرفه)
- UPDATE: project-control/AGENT_GAPS_LOG.md (إضافة GAP-007 كـ Applied)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم تطوير DesignReviewer (ناقد) بناءً على تحليله لـ 3 فجوات.

1. **المعاينة البصرية**: تفعيل **Playwright MCP** للمعاينة البصرية.
   ناقد الآن يستخدم `browser_navigate` و `browser_screenshot` لرؤية الواجهة المنفّذة
   وأخذ لقطات شاشة حقيقية. `webfetch` أصبح Fallback فقط عندما يكون التطبيق غير شغال.
   (ملاحظة: `browser: true` جُرّبت لكن OpenCode لا تعرفها — أُزيلت.)

2. **فحص التوكينز**: إضافة Design Token Verification كخطوة Process منهجية (4 خطوات:
   تحديد المصدر، استخراج القائمة، grep قاعدة الكود، توثيق الانحرافات).
   لا حاجة لأداة جديدة — grep و glob كافيان.

3. **مرجع WORKSPACE_GOVERNANCE_MODEL.md الميت**: حذف من 3 عملاء (ناقد، مدقق، رقيب)
   — كان أول ملف في قائمة القراءة لكنه غير موجود.

4. **إنشاء Source of Truth**: TeraDesignReviewer.md (12 قسماً — الهوية، الموقع، الغرض،
   التفعيل، المدخلات، المخرجات، الصلاحيات، بروتوكول المعاينة، التحقق من التوكينز، الحدود،
   العلاقات، التحسين المستمر).

تم رفض إضافة MCP تصوير أو أداة Screenshot (Anti-Bloat — غير مبررة حالياً).
الموافقة: Majed — Approved (عبر Question Flow)
التحقق من الصحة: Implementation Complete — Anti-Bloat Gate PASS (ملف جديد واحد فقط = Source of Truth ضروري).
المخاطر: منخفضة — webfetch: allow يمنح صلاحية معاينة دون موافقة مسبقة لكنها محدودة بالنص فقط.
ملاحظات الاسترجاع (Rollback):
1. design-reviewer.md: إعادة webfetch إلى ask، إزالة Limited Preview Protocol و Token Verification.
2. حذف tera-system/TeraDesignReviewer.md.
3. ENGINEERING_AGENT_RESPONSIBILITIES.md: إعادة §11 إلى الصيغة السابقة.
4. monitor.md + auditor.md: إعادة إضافة WORKSPACE_GOVERNANCE_MODEL.md.
5. opencode.json: إعادة Playwright MCP إلى `"enabled": false`.
6. AGENT_GAPS_LOG.md: حذف GAP-007.
```

### SCP-2026-07-04-033 — DesignReviewer Improvements II (قاعدة معايير + بروتوتايب + مساعدة بصرية + توكينز معمّق)

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-033
مصدر الطلب: تقرير DesignReviewer الذاتي + تحليل TeraSystemEvolutionAgent + موافقة Majed
نوع التغيير: Agent Capability Improvement / Knowledge Base Creation / Process Expansion
الملفات المعدلة (6 files, 1 new):
- UPDATE: tera-system/TeraDesignReviewer.md (إضافة بروتوتايب §10، مساعدة بصرية، توكينز معمّق §9.2، v1.1)
- CREATE: tera-system/design-system/DESIGN_REVIEW_STANDARDS.md (قاعدة معايير — 9 أقسام)
- UPDATE: .opencode/agents/design-reviewer.md (write: ask + description محدّث + 4 بروتوكولات + Output format مطوّر)
- UPDATE: tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md (تحديث §11: إضافة بروتوتايب + مرجع DESIGN_REVIEW_STANDARDS)
- UPDATE: project-control/AGENT_GAPS_LOG.md (إضافة GAP-008 كـ Applied)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم تطوير DesignReviewer (ناقد) في 4 محاور بناءً على تقريره الذاتي:

1. **قاعدة معايير موحدة (DESIGN_REVIEW_STANDARDS.md)** — 9 أقسام مع قوائم تفتيش قابلة للتنفيذ:
   - اكتمال الحالات، الاتساق البصري، الجودة البصرية، نظافة التصميم، UX، الاستجابة، RTL/Arabic (7.1-7.5)، التوكينز (3-layer)، الاتساق الوظيفي.
   - هذا ملف مرجعي، ت read-only — يقرأه ناقد قبل كل مراجعة.

2. **بروتوكول المساعدة البصرية (Visual Assistance Protocol)** — عندما يحتاج ناقد تأكيداً بصرياً لا يستطيع الحصول عليه من Playwright MCP
   (لون دقيق، بكسل، واجهة لا تعمل محلياً)، يطلب صورة من Majed بصيغة محددة ويحللها.

3. **بروتوكول البروتوتايب (Prototype Protocol)** — بناء HTML/CSS سريع في project-control/prototypes/
   (عند طلب Majed). مؤقت — يُحذف بعد الاعتماد. write: ask — كل كتابة تتطلب موافقة.

4. **توسيع فحص التوكينز** — إضافة §9.2 (3-layer architecture: Primitive → Semantic → Component).

ملاحظة: write: ask هو أوسع من المطلوب (OpenCode لا يدعم تقييد المسارات).
الانضباط الذاتي في وصف agent يحدّد الاستخدام: الكتابة فقط لـ project-control/prototypes/.
الموافقة: Majed — شفهياً: "نفذ"
التحقق من الصحة: Anti-Bloat Gate PASS (ملف جديد واحد = مرجع معرفي، ليس سياسة).
المخاطر: منخفضة — write: ask يمنح صلاحية كتابة لكن كل عملية تخضع لموافقة Majed.
ملاحظات الاسترجاع (Rollback):
1. .opencode/agents/design-reviewer.md: إعادة write: ask → write: deny، حذف البروتوكولات المضافة، إعادة Output format القديم.
2. حذف tera-system/design-system/DESIGN_REVIEW_STANDARDS.md.
3. tera-system/TeraDesignReviewer.md: حذف §10 (بروتوتايب) و §9.2 (توكينز معمّق)، إعادة §9 القديم، إعادة v1.0.
4. tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md: إعادة §11 للصيغة السابقة.
5. AGENT_GAPS_LOG.md: حذف GAP-008.
```

### SCP-2026-07-04-034 — Monitor (رقيب) — تمكين صلاحية Bash/Git للتدقيق

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-034
مصدر الطلب: تقرير Monitor الذاتي + تحليل TeraSystemEvolutionAgent + موافقة Majed
نوع التغيير: Permission Upgrade / Agent Capability Improvement
الملفات المعدلة (3 files, 0 new):
- UPDATE: .opencode/agents/monitor.md (bash: deny → ask, description محدّث, إضافة Git Audit Protocol)
- UPDATE: project-control/AGENT_GAPS_LOG.md (إضافة GAP-009 كـ Applied)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم رفع صلاحية bash في monitor.md من deny إلى ask ليتمكن Monitor من تنفيذ
بند "Cross-check Handback vs Git diff" المطلوب في تعريفه.

التغيير الوحيد: bash: deny → bash: ask + إضافة Git Audit Protocol.

Git Audit Protocol يُحدد:
- الأوامر المسموح بها: git diff --name-only, git log --oneline, git show --stat (قراءة فقط)
- الانضباط الذاتي: bash فقط لأوامر git read-only — أي أمر آخر يحتاج تبريراً صريحاً
- التوثيق: تسجيل نتائج git diff في التقرير

المقترحات المرفوضة (Anti-Bloat):
- ❌ Compliance Auditor Skill (يقوم به Monitor نفسه)
- ❌ Git MCP (غير ضروري — git يعمل عبر bash)
- ❌ CI/CD تكامل (تضخم مبكر — لا Production ولا pipelines)

الموافقة: Majed — "موافق"
التحقق من الصحة: Anti-Bloat Gate PASS (لا ملفات جديدة).
المخاطر: منخفضة — bash: ask = كل أمر يخضع لموافقة Majed.
ملاحظات الاسترجاع (Rollback):
1. .opencode/agents/monitor.md: إعادة bash: ask → bash: deny، حذف Git Audit Protocol.
2. AGENT_GAPS_LOG.md: حذف GAP-009.
```

### SCP-2026-07-04-035 — Monitor (رقيب) — Source of Truth + ميثاق التدقيق + القواعد السبعة

```text
تاريخ: 2026-07-04
معرف التغيير: SCP-2026-07-04-035
مصدر الطلب: تقرير Monitor الذاتي + تحليل TeraSystemEvolutionAgent + موافقة Majed
نوع التغيير: Source of Truth Creation / Agent Capability Improvement / Process Formalization
الملفات المعدلة (6 files, 1 new):
- CREATE: tera-system/TeraMonitor.md (Source of Truth — 8 أقسام)
- UPDATE: .opencode/agents/monitor.md (إضافة System Reference + اختصار What you do بالإشارة للمصدر)
- UPDATE: tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md (توسيع §6 — القواعد السبعة + صلاحية الرفض)
- UPDATE: tera-system/TeraPolicyMap.md (إضافة إدخال Monitor audit framework)
- UPDATE: project-control/AGENT_GAPS_LOG.md (إضافة GAP-010 كـ Applied)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم إنشاء TeraMonitor.md — Source of Truth لـ Monitor (رقيب) في tera-system/.
يحتوي 8 أقسام تغطي:

§1-3: الهوية، الموقع، الغرض
§4: **المراجع المعتمدة (Reference Hierarchy)** — 8 مستويات من الدستور إلى سجل النشاط
§5: **قواعد التدقيق السبعة الثابتة** (The 7 Immutable Audit Rules):
    1. مطابقة الخطة  2. الترتيب والتبعيات  3. بوابة الهندسة
    4. سجل الامتثال  5. Handback vs Git Diff  6. زحف النطاق  7. الانحراف المعماري
§6: **صلاحية رفض الخطة** (Plan Rejection Authority) — 4 شروط واضحة
§7: العلاقة مع بقية العملاء
§8: مرجع التحسين المستمر

تم رفض إنشاء MONITOR_CHARTER.md في project-control/ (موقع خطأ) و MONITOR_AUDIT_TRAIL.md
(تضخم — التدقيق التراكمي يُسجل في PROJECT_ACTIVITY_LOG.md الموجود).
الموافقة: Majed — "نفذ"
التحقق من الصحة: Anti-Bloat Gate PASS (ملف جديد واحد في tera-system/ = نمط متسق مع TeraDesignReviewer.md).
المخاطر: منخفضة — الميثاق يحدد صلاحية الرفض بـ 4 شروط فقط.
ملاحظات الاسترجاع (Rollback):
1. حذف tera-system/TeraMonitor.md.
2. .opencode/agents/monitor.md: إزالة System Reference، إعادة What you do القديم.
3. ENGINEERING_AGENT_RESPONSIBILITIES.md: إعادة §6 للصيغة السابقة.
4. TeraPolicyMap.md: حذف إدخال Monitor.
5. AGENT_GAPS_LOG.md: حذف GAP-010.

### SCP-2026-07-05-036 — TeraAuditor.md — Source of Truth لـ Auditor (مدقق)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-036
مصدر الطلب: تقرير Auditor الذاتي + تحليل TeraSystemEvolutionAgent + موافقة Majed
نوع التغيير: Source of Truth Creation / Agent Capability Improvement / Policy Update
الملفات المعدلة (6 files, 1 new):
- CREATE: tera-system/TeraAuditor.md (Source of Truth — 10 أقسام)
- UPDATE: .opencode/agents/auditor.md (إضافة System Reference + بروتوكول التراكم + تحديث Output Format)
- UPDATE: tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md (توسيع §5: منهجية 6 مراحل + تراكم + بروتوكول عدم يقين + مرجع)
- UPDATE: tera-system/TeraPolicyMap.md (إضافة إدخال Auditor audit framework)
- UPDATE: project-control/AGENT_GAPS_LOG.md (إضافة GAP-011 كـ Applied)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md
الملخص:
تم إنشاء TeraAuditor.md — Source of Truth لـ Auditor (مدقق) في tera-system/.
يحتوي 10 أقسام تغطي:

§1: الهوية — من أنا، ما لا أفعله أبداً
§2: الموقع في المنظومة — مكاني في هرم Tera
§3: الغرض — ما هي وظيفتي بالضبط
§4: المراجع المعتمدة (Reference Hierarchy) — 7 مستويات من الدستور إلى سجلات المهام
§5: منهجية التدقيق المتدرجة (6 مراحل) — استيعاب ← توثيق ← تدقيق هندسي ← مطابقة ← تقرير ← توصية
§6: جدول تصنيف النتائج — PASS / NEEDS_FIX / BLOCKED / DEFERRED + معايير كل حالة
§7: بروتوكول عدم اليقين والبحث — متى أتوقف، متى أطلب WebSearch/WebFetch
§8: بروتوكول التراكم — بناءً على آخر تدقيق في PROJECT_ACTIVITY_LOG.md
§9: العلاقة مع بقية العملاء — رقيب، ناقد، مهندس، TeraAgent
§10: مرجع التحسين المستمر

تم رفض إنشاء AUDIT_TRAIL.md (Anti-Bloat — التراكم يُسجل في PROJECT_ACTIVITY_LOG.md الموجود).
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat Gate PASS (ملف جديد واحد في tera-system/ = نمط متسق مع TeraMonitor.md و TeraDesignReviewer.md).
المخاطر: منخفضة — لا تغيير في صلاحيات أو أدوار. TeraAuditor.md وثيقة دستورية فقط.
ملاحظات الاسترجاع (Rollback):
1. حذف tera-system/TeraAuditor.md.
2. .opencode/agents/auditor.md: إزالة System Reference، إزالة بروتوكول التراكم، إعادة Output Format إلى PASS/NEEDS_FIX/BLOCKED.
3. ENGINEERING_AGENT_RESPONSIBILITIES.md: إعادة §5 للصيغة السابقة (13 سطراً).
4. TeraPolicyMap.md: حذف إدخال Auditor.
5. AGENT_GAPS_LOG.md: حذف GAP-011.
```

### SCP-2026-07-05-037 — TCEA Runtime Sync — Self-Check + Uncertainty + §35 (Runtime Gap from SCP-028)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-037
مصدر الطلب: System Gap — اكتشفها حارس بعد أن أنتج TCEA ملفات Tamayuz بدون Self-Check / Uncertainty (نظامي، وليس تقصيراً من المستشار)
نوع التغيير: Runtime Sync / Agent Process Fix
الملفات المعدلة (1 file, 0 new):
- UPDATE: .opencode/agents/tera-client-engagement.md (مزامنة الـ Runtime مع البروتوكولات المضافة في SCP-028)
الملخص:
تم اكتشاف فجوة نظامية: SCP-028 (GAP-005) أضاف Self-Check Protocol (§3.2.6) و Uncertainty Protocol (§3.2.7) إلى
TeraClientEngagement.md (مصدر الحقيقة) والـ §35 إلى TERA_RUNTIME_TEMPLATES.md، لكن ملف runtime
.tera-client-engagement.md لم يتم تحديثه — فالمستشار يعمل من Runtime قديم لا يذكره بهذه البروتوكولات.

التغييرات في tera-client-engagement.md:
1. §3 (تدفق العمل): إضافة 3 خطوات إلزامية — Self-Check لكل Domain، Uncertainty Protocol، واستخدام §35
2. §5 (جديد): Self-Check & Uncertainty Protocols — ملخص تنفيذي للبروتوكولات مع القواعد الحاسمة
3. §7 (الملفات): ربط DISCOVERY_COVERAGE_SUMMARY.md بـ §35
4. §9 (المصادر): إضافة TERA_RUNTIME_TEMPLATES.md إلى قائمة المراجع
5. §10 (تسعير): إعادة ترقيم (كان §9) بعد إضافة §5 الجديد
6. Last Synced: تحديث من 2026-07-04 إلى 2026-07-05

سبب عدم اكتشافها مبكراً: Tamayuz كان أول عميل حقيقي بعد SCP-028، فظهرت الفجوة فوراً في أول استخدام.
الموافقة: Majed — Approved (شفهياً في جلسة العمل)
التحقق من الصحة: Runtime file sections numbered 1→10, all internal references consistent.
0 ملفات جديدة، 0 عملاء جدد، 0 MCPs.
المخاطر: منخفضة — يضيف وعياً بالبروتوكولات الموجودة أصلاً في مصدر الحقيقة.
ملاحظات الاسترجاع (Rollback):
1. إزالة الخطوات المضافة من §3 (أسطر 70-72)
2. حذف §5 (Self-Check & Uncertainty Protocols) بالكامل
3. إعادة ترقيم الأقسام: §6→§5, §7→§6, §8→§7, §9→§8, §10→§9
4. إعادة Last Synced إلى 2026-07-04
```

### SCP-2026-07-05-038 — TCEA Strengthening: 4 New Governance Rules

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-038
مصدر الطلب: User Request (Majed) — تقرير تحليل ثغرات TCEA بعد عميل Mawthooq
نوع التغيير: Agent Improvement / Policy Addition / Process Gate Addition
الملفات المعدلة (3 files, 0 new):
- UPDATE: tera-system/TeraClientEngagement.md (إضافة §§3.3.1, 3.3.2, 3.3.3 + تحديث §3.6.1 + تحديث §5.1)
- UPDATE: .opencode/agents/tera-client-engagement.md (مزامنة الـ 4 قواعد في runtime + تحديث Last Synced)
الملخص:
تمت إضافة 4 قواعد حوكمة لـ TCEA بناءً على تحليل Majed لثغرات عمله مع عميل "الموثوق":

1. **Final Scope Reconciliation Gate (§3.3.1):** قبل Handoff، يجب توحيد حالة كل ميزة في FEATURE_LIST.md
   إلى واحدة من 4 حالات (Included in MVP / Optional / Phase 2 / Out of Scope).

2. **Budget-to-Scope Control Rule (§3.3.2):** عندما تكون الميزانية محدودة، تُصنف الميزات غير الأساسية
   كـ Optional أو Phase 2 (وليس MVP).

3. **Client Decision Register (§3.3.3):** كل قرار مهم من العميل يُسجل في CLIENT_DECISION_LOG.md
   بإحدى 4 حالات موحدة (Approved / Deferred / Conditional / Not Finalized).

4. **Approval Consistency Rule (§3.6.1):** لا يجوز TERA_HANDOFF_PACKAGE.md بحالة Approved
   إذا بقيت ملفات المصدر (CLIENT_INTAKE, SCOPE_SUMMARY, FEATURE_LIST, DRAFT_QUOTATION,
   CLIENT_DECISION_LOG) بحالة Draft أو Pending.

تم رفض إنشاء ملفات جديدة أو عملاء جدد (Anti-Bloat).
الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed — Anti-Bloat ✅ (0 ملفات جديدة)، لا تضخم، لا تكرار، لا تعديل صلاحيات، لا تلويث لتطبيقات العملاء
المخاطر: منخفضة — قواعد إجرائية في ملفات موجودة، لا تغيير في صلاحيات أو أدوار.
ملاحظات الاسترجاع (Rollback):
1. tera-system/TeraClientEngagement.md: حذف §§3.3.1, 3.3.2, 3.3.3، إزالة الإضافة من §3.6.1،
   إعادة §5.1 للصيغة السابقة.
2. .opencode/agents/tera-client-engagement.md: إزالة الإشارات للقواعد الأربع من §§3, 7،
   إعادة Last Synced إلى 2026-07-05 (SCP-037).
```

### SCP-2026-07-05-039 — تفعيل آلية التسعير v4.2 رسمياً لـ TCEA + أدوات التسعير

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-039
مصدر الطلب: User Request (Majed) — اعتماد آلية التسعير لمستشار TCEA
نوع التغيير: Policy Activation / Agent Workflow Upgrade / Tooling Deployment
الملفات المعدلة (6 files, 4 new + 2 updates):
- UPDATE: .opencode/agents/tera-client-engagement.md (إعادة كتابة §9 + §10 كاملة — ربط إلزامي بأدوات التسعير)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md (هذا الإدخال)
- CREATE: project-control/TeraPricingCalculator.xlsx (حاسبة Excel — 6 أوراق عمل)
- CREATE: project-control/TeraPriceQuoteTemplate.docx (قالب عرض سعر وورد احترافي)
- CREATE: project-control/TRAINING_GUIDE_TCEA.md (دليل تدريب المستشار — 10 أقسام)
- UPDATE: project-control/PRICING_SCORECARD_APPLICATION_MAWTHOOQ.md (تحديث إلى الأرقام الجديدة ~500 JOD)
الملخص:
تم تفعيل آلية التسعير TeraPricingPolicy.md v4.2 رسمياً لاستخدام TeraClientEngagementAgent.
التغييرات الرئيسية:

1. تعريف TCEA (.opencode/agents/tera-client-engagement.md):
   - تحديث Section 9 (المصادر): من "v0.1 Draft" إلى "v4.2 — معتمدة — إلزامية لكل عرض سعر"
   - إضافة جميع أدوات التسعير كمراجع إلزامية (حاسبة Excel، قالب Word، دليل التدريب، مثال تطبيقي)
   - إعادة كتابة Section 10 (Pricing Workflow) بالكامل لتطابق آلية v4.2:
     * 10.3: 14 حقل معلومات إلزامية قبل التسعير
     * 10.4: 15 خطوة إلزامية للتسعير (Level 1→2→3)
     * 10.5: جدول الأدوات المستخدمة
     * 10.6: قائمة الاعتماد الإلزامية (15 سؤالاً — يجب الإجابة بـ "نعم" قبل إرسال أي عرض)
     * 10.7: الأخطاء الممنوعة (مخالفات وعواقبها)
     * 10.8: 8 قواعد صارمة

2. أدوات التسعير الجديدة:
   - TeraPricingCalculator.xlsx: حاسبة Excel بستة أوراق عمل، خلايا صفراء للإدخال فقط، قوائم منسدلة، معادلات تلقائية
   - TeraPriceQuoteTemplate.docx: قالب عرض سعر احترافي (Word) بقسم التوقيع وشروط الدفع
   - TRAINING_GUIDE_TCEA.md: دليل تدريب يغطي 10 أقسام (خطوات، أخطاء، أسئلة، قرارات)

3. المثال التطبيقي:
   - تحديث PRICING_SCORECARD_APPLICATION_MAWTHOOQ.md من 1,605 JOD إلى ~500 JOD
   - التحقق: التجربة الفعلية على Mawthooq أعطت 497 JOD (فارق -3 JOD عن الهدف)

4. فحص النطاق (Range Check):
   - Anchor Points: 20%=170, 40%=350, 60%=700, 70%=1,400, 80%=2,400 — تصاعد سلس
   - Add-ons: مخفضة من ×0.35-0.40 مع قاعدة تناسب (50%)
   - التحليل المدفوع: مخفض إلى 35-500 JOD
   - الدعم الشهري: مخفض إلى 20-300 JOD
   - Min Fee: 100 JOD
   - Internal Rate: 4 JOD/ساعة
الموافقة: Majed — Approved (عبر التجربة المباشرة على مشروع Mawthooq)
التحقق من الصحة: Anti-Bloat Gate ✅ — ملفات الأداة (Excel + Word) ليست ملفات system/policy بل أدوات تشغيلية.
قائمة الاعتماد (15 سؤالاً) تمت إجابتها جميعاً بـ "نعم" على مشروع Mawthooq.
المخاطر: منخفضة — TCEA لا يزال ينتج مسودات فقط، Majed يعتمد السعر النهائي.
ملاحظات الاسترجاع (Rollback):
1. .opencode/agents/tera-client-engagement.md: استعادة §9 و §10 من النسخة السابقة.
2. حذف TeraPricingCalculator.xlsx من project-control/.
3. حذف TeraPriceQuoteTemplate.docx من project-control/.
4. حذف TRAINING_GUIDE_TCEA.md من project-control/.
5. استعادة PRICING_SCORECARD_APPLICATION_MAWTHOOQ.md من النسخة السابقة.
```

### SCP-2026-07-05-040 — إلزام قالب الخطاب الرسمي letterhead-master-fixed-print.html لكل مراسلات الزبائن

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-040
مصدر الطلب: User Request (Majed) — توحيد الهوية الرسمية في مراسلات الزبائن
نوع التغيير: Policy Enforcement / Reference Addition
الملفات المعدلة (3 files, 0 new):
- UPDATE: .opencode/agents/tera-client-engagement.md (إضافة §9 مرجع، §10.5 أداة، §10.8 قاعدة، تحديث Level 3)
- UPDATE: project-control/TRAINING_GUIDE_TCEA.md (إضافة أداة + قاعدة في الملخص)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md (هذا الإدخال)
الملخص:
تم إلزام استخدام قالب الخطاب الرسمي للمؤسسة (letterhead-master-fixed-print.html) لجميع المراسلات
الرسمية مع الزبائن. تمت الإشارة إليه في:
- تعريف TCEA: Section 9 (مرجع إلزامي)، Section 10.5 (أداة إلزامية)، Section 10.8 (قاعدة صارمة)
- Training Guide: جدول الأدوات + الملخص النهائي
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat ✅ (0 ملفات جديدة)
المخاطر: منخفضة
ملاحظات الاسترجاع (Rollback):
1. إزالة مرجع branding من Section 9 في tera-client-engagement.md
2. إزالة سطر القالب من Section 10.5
3. إزالة القاعدة من Section 10.8
4. إزالة التحديث من TRAINING_GUIDE_TCEA.md
```

### SCP-2026-07-05-041 — تنظيف project-control/ وأرشفة الملفات التاريخية

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-041
مصدر الطلب: User Request (Majed) — تنظيف وترتيب project-control/
نوع التغيير: Anti-Bloat / Cleanup / Archival
الملفات المعدلة:
- DELETE: project-control/TeraPriceQuoteTemplate.docx (ملف القالب — تم الاستغناء عنه لصالح letterhead-master-fixed-print.html)
- DELETE: project-control/PRICING_SCORECARD_REPORT_v2.md (مكرر — لا مراجع له)
- DELETE: project-control/OPERATOR_AUTH_POLICY.md (مهمل — لا مراجع له)
- MOVE → archive/: 25 ملفاً تاريخياً (جميع مقترحات SCP السابقة + ملفات تسعير قديمة + GOVERNANCE_LAYER_REPORT)
- UPDATE: .opencode/agents/tera-client-engagement.md (إزالة مراجع .docx, توجيه إلى letterhead مباشرة)
- UPDATE: project-control/TRAINING_GUIDE_TCEA.md (إزالة مراجع .docx, توجيه إلى letterhead مباشرة)
- UPDATE: project-control/PROJECT_ACTIVITY_LOG.md (تحديث مسار GOVERNANCE_LAYER_REPORT)
- UPDATE: project-control/SYSTEM_EVOLUTION_LOG.md (هذا الإدخال + تحديث مسارات SCP إلى archive/)
- DELETE: project-preparation/ بالكامل (5 ملفات — كانت خاصة بتطبيق الفارس للصيانة المحذوف)
- DELETE: tera-system/Foundation Logo and LetterHead/ (4 ملفات — مكررة في tera-workshop/client-templates/branding/)
الملخص:
تم تنظيف project-control/ بالكامل: 3 ملفات محذوفة، 25 ملفاً تاريخياً منقولة إلى archive/،
وتحديث جميع المراجع في system files لتعكس المسارات الجديدة.
ثم حذف project-preparation/ بالكامل (5 ملفات) بموافقة Majed — كانت كلها خاصة بتطبيق الفارس للصيانة الذي أُزيل.
ثم حذف tera-system/Foundation Logo and LetterHead/ (~2.5MB) — مكرر في tera-workshop/client-templates/branding/.
الآن:
- project-control/ يحتوي على 20 ملفاً نشطاً (بدلاً من 48)
- project-preparation/ أُزيل بالكامل
- tera-system/ نظيف (31 مدخلاً نشطاً)
- جميع الملفات التاريخية متوفرة في archive/
الموافقة: Majed — Approved (project-control cleanup) + Direct instruction (project-preparation deletion)
التحقق من الصحة: Anti-Bloat ✅ — 28 ملفاً أقل من قبل، لا تضخم، لا تكرار
المخاطر: منخفضة — جميع المراجع محدّثة، لا مسارات مكسورة
ملاحظات الاسترجاع (Rollback):
1. إعادة الملفات من archive/ إلى project-control/
2. استعادة الملفات المحذوفة من git
3. إعادة المراجع في .opencode/agents/tera-client-engagement.md
4. إعادة المراجع في TRAINING_GUIDE_TCEA.md و PROJECT_ACTIVITY_LOG.md
```

### SCP-2026-07-05-041 — تحديث TeraClientEngagement.md §11 لمطابقة TeraPricingPolicy.md v4.2

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-041
مصدر الطلب: User Request (Majed) — بناءً على تحليل TeraSystemEvolutionAgent لقول TCEA "pricing isn't ready yet"
نوع التغيير: Policy Conflict Resolution / Source of Truth Alignment
الملفات المعدلة (1 file):
- UPDATE: tera-system/TeraClientEngagement.md (إعادة كتابة §11 بالكامل)
الملخص:
تم تحديث §11 في TeraClientEngagement.md من "Pricing System (v0.1 — Draft — Calibration Required)"
إلى "(v4.2 — معتمدة)" لمطابقة TeraPricingPolicy.md v4.2 المعتمدة فعلياً.

التغييرات الرئيسية:
1. **اللقب والحالة**: من "v0.1 Draft — Calibration Required" إلى "v4.2 — معتمدة"
2. **المعادلة**: من "Feature Base Price × Complexity Factor + Risk Buffer + Margin" إلى معادلة v4.2 الكاملة (Interpolation, Multipliers, Risk Margin, Rush Premium, Min Profitable Price)
3. **Rubric**: من 6 معايير (0-3) إلى 12 معياراً (0-5) بأوزان — Complexity Index
4. **Base Price**: من "ساعات × 15-25 JOD" إلى Anchor Points باستيفاء خطي (0%=75 → 80%=2,400 JOD)
5. **Minimum Price**: من "500-700 / 1,200 JOD" إلى Minimum Engagement Fee 100 JOD + Min Profitable Price (ساعات × 4 JOD)
6. **Discovery**: من "50-100 JOD" إلى 35-500 JOD حسب حجم المشروع
7. **Risk**: من "0%/+10%/+20%" إلى 4 مستويات (5%-20%) + Rush Premium منفصل
8. **خطوات Level 2**: من 6 خطوات قديمة إلى 10 خطوات محدثة تشير للحاسبة والسياسة
9. **خطة الدفع**: من نص حر إلى جدول منظم (3 مستويات)
10. **الصيانة**: تحديث نطاق الاشتراك الشهري (20-300 JOD)
11. **أضيف**: قالب الخطاب الرسمي الإلزامي، تفاصيل الضمان

الموافقة: Majed — Approved (عبر الأمر "نفذ")
التحقق من الصحة:
- Anti-Bloat Gate ✅ — تعديل ملف موجود، لا ملفات جديدة
- Policy Map Check ✅ — TeraPricingPolicy.md يبقى مصدر الحقيقة الوحيد للتسعير
- Architecture Map Check ✅ — لا تغيير في أدوار المجلدات
- No client-app contamination ✅ — الملف في tera-system/
- No stale references ✅ — تمت إزالة جميع إشارات v0.1 Draft
- No duplicated rules ✅ — §11 الآن ملخص متسق مع v4.2 وليس مصدراً بديلاً
- Runtime sync required: ❌ لا — تعريف TCEA (.opencode/agents/tera-client-engagement.md) كان محدثاً أصلاً لـ v4.2

المخاطر: منخفضة — §11 ملخص تشغيلي، والسياسة v4.2 كانت معتمدة ومستخدمة فعلاً
ملاحظات الاسترجاع (Rollback):
1. استعادة §11 من النسخة السابقة في git: `git checkout -- tera-system/TeraClientEngagement.md`
2. لا ملفات أخرى متأثرة
```

### SCP-2026-07-05-042 — إضافة Consultation Response Protocol لموازنة دور TCEA بين الاستكشاف والاستشارة

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-042
مصدر الطلب: Agent Gap (أبلغ عنها TCEA بنفسه) — المستشار يشعر بخلل في التوازن
نوع التغيير: Agent Improvement / Process Protocol Addition
الملفات المعدلة (2 files):
- UPDATE: tera-system/TeraClientEngagement.md (إضافة §3.2.8 Consultation Response Protocol)
- UPDATE: .opencode/agents/tera-client-engagement.md (تحديث §1 + إضافة §5.3)
الملخص:
تمت إضافة Consultation Response Protocol كبروتوكول ثالث في ملف المستشار بعد Self-Check و Uncertainty، لموازنة دور المستشار بين الاستكشاف (أسئلة) والاستشارة (تحليل وتوصيات).

التغييرات الرئيسية:
1. **المصدر** (TeraClientEngagement.md §3.2.8):
   - بروتوكول استشاري متكامل: فهم مختصر ← اقتراحات ← مخاطر ← أسئلة متابعة ← تقسيم مرحلي
   - قاعدة التوازن: Self-Check + Uncertainty = دفاعي, Consultation Response = هجومي — كلاهما إلزامي
   - أمثلة توضيحية (بدون VS مع Consultation Response)
   - حدود البروتوكول (لا إلغاء Gates، اقتراحات ≠ التزامات)

2. **الرنتايم** (.opencode/agents/tera-client-engagement.md):
   - تحديث §1 (Client Discovery Consultant): إضافة ذكر التحليل والتوصية والـ phase guidance
   - إضافة §5.3 Consultation Response Protocol (5 عناصر + قاعدة التوازن + حدود)

ما لم يتغير:
- ✅ Self-Check Protocol باقٍ إلزامياً
- ✅ Uncertainty Protocol باقٍ إلزامياً
- ✅ جميع Gates الحوكمة (Discovery Coverage, Quotation, Handoff)
- ✅ جميع حدود الصلاحية (لا اعتماد نهائي، لا تخطي)

الموافقة: Majed — Approved (عبر الأمر "نفذ")
التحقق من الصحة:
- Anti-Bloat Gate ✅ — تعديل ملفين موجودين، لا ملفات جديدة
- Policy Map Check ✅ — لا تغيير في مراجع السياسات
- Architecture Map Check ✅ — لا تغيير في أدوار المجلدات
- No client-app contamination ✅ — الملفات في tera-system/ و .opencode/agents/
- Governance preserved ✅ — البروتوكولات الدفاعية باقية، Consultation Response يكملها ولا يلغيها
- Runtime sync required: ❌ لا — التحديث مباشر في ملف الرنتايم

المخاطر: منخفضة — البروتوكول الجديد لا يغير صلاحيات أو Gates، الاقتراحات لا تساوي اعتماداً
ملاحظات الاسترجاع (Rollback):
1. TeraClientEngagement.md: حذف §3.2.8 (Consultation Response Protocol)
2. .opencode/agents/tera-client-engagement.md: حذف §5.3 + استعادة §1
```

### SCP-2026-07-05-043 — تنظيف ملف TCEA — إزالة التكرار والتعارض

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-043
مصدر الطلب: User Request (Majed) — بعد فحص الحارس لوجود تكرار/تعارض
نوع التغيير: System Maintenance / Anti-Bloat / Policy Conflict Resolution
الملفات المعدلة (1 file):
- UPDATE: .opencode/agents/tera-client-engagement.md (3 تعديلات)

الملخص:
تم تنظيف ملف TCEA من 3 مشاكل: تكرار بين §1 و §2، نقص الإشارة لـ Consultation Response
في §3، وتعريف أداة التسعير في §6.

التغييرات:
1. **دمج §1 (Core Functional Roles) + §2 (مسؤولياتك الأساسية)** في جدول واحد ثنائي اللغة
   بـ 9 أدوار موحّدة — إزالة التكرار والتقسيم المختلف بلغتين
2. **إضافة Consultation Response Protocol** إلى تدفق العمل §3
   — بعد الحوار الاستكشافي، سطر جديد يوجّه لتطبيق §5.3
3. **تصحيح أداة التسعير في §6**:
   - قبل: "باستخدام TeraPricingPolicy.md"
   - بعد: "باستخدام TeraPricingCalculator.xlsx (حسب TeraPricingPolicy.md)"

الموافقة: Majed — Approved (عبر الأمر "نفذ")
التحقق من الصحة:
- Anti-Bloat Gate ✅ — ملف واحد فقط، إزالة 15 سطراً مكرراً
- Policy Map Check ✅ — لا تغيير في مراجع السياسات
- Architecture Map Check ✅ — لا تغيير في أدوار المجلدات
- No client-app contamination ✅ — الملف في .opencode/agents/
- No stale references ✅ — جميع المراجع محدثة
- No duplicated rules ✅ — §1/§2 الآن جدول واحد = لا تكرار
- Runtime sync required: ❌ لا

المخاطر: منخفضة جداً — تعديلات شكلية/تنظيمية فقط، لا تغيير في صلاحيات أو Gates
ملاحظات الاسترجاع (Rollback):
1. استعادة .opencode/agents/tera-client-engagement.md من git
```

### SCP-2026-07-05-046 — تصحيح 5 تضاربات هيكلية في ملف TCEA الرنتايم

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-046
مصدر الطلب: Proactive System Stewardship (تحليل عميق)
نوع التغيير: Anti-Bloat / System Maintenance
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md

الملخص:
تصحيح 5 مشاكل هيكلية في ملف TCEA الرنتايم بعد تحليل عميق للتضارب الذهني والتنفيذي:

1. **إصلاح الترقيم** — تغيير §1 المكرر (الأدوار والمسؤوليات) إلى §2، ليصبح التسلسل:
   §1 هويتك ← §2 الأدوار ← §3 تدفق العمل ← §4 Websearch ← §5 بروتوكولات ← ... §10

2. **تحديث عنوان §5** — من "Self-Check & Uncertainty Protocols" إلى
   "Mandatory Operating Protocols — بروتوكولات العمل الإلزامية"
   (لأن القسم يحتوي 3 بروتوكولات: Self-Check + Uncertainty + Consultation Response)

3. **توضيح مراجع §3** — إضافة ملاحظة في بداية §3:
   "جميع الإشارات إلى أقسام مرقمة مثل (§3.2.7), (§3.3.2) وغيرها تشير إلى
   TeraClientEngagement.md (مصدر الحقيقة)"
   + توحيد مرجع Self-Check Protocol بإزالة اسم الملف الزائد (للاتساق مع بقية المراجع)

4. **إزالة تكرار قاعدة 13 Domain** — كانت مذكورة في 3 مواضع (سطر 79، 107، 254):
   - أُبقيَت في تدفق العمل (سطر 79) — مكانها الطبيعي
   - أُزيلَت من القاعدة الإلزامية الإضافية (سطر 107)
   - أُزيلَت من §8 (مصدر الأسئلة) — اختصار §8 لجملة واحدة فقط

5. **إعادة تنظيم §9 (المصادر المرجعية)** — من قائمة مسطحة بـ 12 مرجعاً بدون ترتيب
   إلى 4 فئات بأولويات واضحة:
   - إلزامي قبل بدء العمل (4 مراجع)
   - للتسعير عند إنتاج عرض سعر (مرجعان)
   - مرجعي عند الحاجة (3 مراجع)
   - قوالب للمراسلات والوثائق (3 مراجع)

الموافقة: Majed — Approved (عبر الأمر "نفذ")
التحقق من الصحة:
- Anti-Bloat Gate ✅ — ملف واحد فقط، إزالة تكرار، لا ملفات جديدة
- Policy Map Check ✅ — لا تغيير في مراجع السياسات أو مصدر الحقيقة
- Architecture Map Check ✅ — لا تغيير في أدوار المجلدات
- No client-app contamination ✅ — الملف في .opencode/agents/
- No stale references ✅ — الملاحظة الجديدة في §3 توضح مصدر جميع المراجع
- No duplicated rules ✅ — قاعدة 13 Domain الآن في موضع واحد فقط
- Runtime sync required: ❌ لا — التغييرات في TCEA فقط، لا تؤثر على عملاء آخرين

المخاطر: منخفضة جداً — تعديلات هيكلية/شكلية/تنظيمية فقط:
- إصلاح ترقيم (لم يكن هناك مرجع داخلي لـ §2 من قبل لأنه لم يكن موجوداً)
- تغيير عنوان §5 (لا يؤثر على محتوى البروتوكولات)
- إضافة ملاحظة توضيحية (لا تغير سلوكاً)
- إزالة تكرار (القاعدة لا تزال موجودة في موضعها الأصلي)
- إعادة تنظيم §9 (جميع المراجع محفوظة، فقط إعادة ترتيب)

ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-054 — TCEA: إضافة Runtime Load Order + إزالة الاعتماد على الذاكرة الضمنية

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-054
مصدر الطلب: Audit body finding #3 (الاعتماد على الذاكرة الضمنية للنموذج)
نوع التغيير: Runtime Reliability / Instruction Precision
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة هيكلة §9 + إضافة §9.5 + تحديث §10)

الملخص:
إزالة جميع العبارات غير القابلة للتحقق واستبدالها بتعليمات صريحة:
1. إضافة §9.5 Runtime Load Order — 6 جداول ترتيب تحميل حسب السياق
2. استبدال 5 عبارات ضمنية ("مرة واحدة", "عند الشك", "عند الحاجة") بعبارات قابلة للتنفيذ
3. إضافة قاعدة حاسمة: "إذا لم يكن دليل صريح أنك قرأت الملفات، فلا تبدأ المهمة"

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: 0 عبارات "مرة واحدة" أو "عند الشك" متبقية في TCEA.
المخاطر: منخفضة.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-055 — TCEA: ضبط Consultation Response Protocol في قالب إلزامي صارم

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-055
مصدر الطلب: Audit body finding #4 (أسلوب الرد واسع — عرضة للحشو والاستعراض)
نوع التغيير: Response Format Enforcement / Anti-Bloat
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة بناء §5.3 بالكامل)

الملخص:
تحويل Consultation Response Protocol من إرشادات مفتوحة إلى قالب رد إجباري بحدود صارمة:

1. قالب إلزامي من 5 أقسام بحدود قصوى:
   - ما فهمته: سطران كحد أقصى
   - المخاطر/الملاحظات: 1-3 فقط
   - الاقتراحات: 1-3 فقط
   - الأسئلة: حتى 5 كحد أقصى
   - التقسيم المرحلي: فقط إذا كان واضحاً

2. إضافة 7 قواعد صارمة لمكافحة الحشو:
   - لا تكرر كلام Majed بصياغة طويلة
   - لا تعط أكثر من 3 اقتراحات إلا إذا طُلب
   - لا تسأل أكثر من 5 أسئلة في الدفعة
   - لا تقدم Roadmap إلا إذا طُلب
   - لا تحوّل الاقتراح إلى قرار
   - لا تحلل أكثر من اللازم
   - إذا المعلومات قليلة، اعترف بدلاً من التخمين

3. تحديث مرجع §5.3 في تدفق العمل (السطر 78)

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: قالب الرد الآن إلزامي ومقيد بحدود عددية واضحة. Anti-Bloat Gate PASS.
المخاطر: منخفضة — يقلل الحشو والاستعراض دون التأثير على جودة التحليل.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-054
مصدر الطلب: Audit body finding #3 (الاعتماد على الذاكرة الضمنية للنموذج — "مرة واحدة قبل أول استخدام", "عند الشك")
نوع التغيير: Runtime Reliability / Instruction Precision
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة هيكلة §9 + إضافة §9.5 + تحديث §10)

الملخص:
إزالة جميع العبارات غير القابلة للتحقق للنموذج واستبدالها بتعليمات صريحة قابلة للتنفيذ:

1. إضافة §9.5 (Runtime Load Order) — جدول ترتيب تحميل الملفات حسب السياق:
   - عند تشغيل Session
   - عند بدء Discovery
   - عند بدء أول مهمة Pricing في Session
   - عند إنتاج DRAFT_QUOTATION.md
   - عند إنتاج DISCOVERY_COVERAGE_SUMMARY.md
   - عند إنتاج TERA_HANDOFF_PACKAGE.md

2. استبدال 5 عبارات ضمنية:
   - "مرة واحدة قبل أول استخدام" ← "في أول Session تسعيرية عمرياً فقط، ثم عند خطأ Proportion Check"
   - "اقرأه قبل أول استخدام" ← "في أول Session تسعيرية عمرياً فقط؛ ثم عند تحذير Proportion Check"
   - "ارجع إليه عند الشك" ← "اقرأه قبل أول استخدام للحاسبة في كل Session — مرجع إلزامي"
   - "مرجعي — عند الحاجة" ← جدول triggers محدد لكل ملف: متى يُقرأ بالضبط
   - "اقرأها عند التشغيل" ← "اقرأ عند كل Session من TCEA"

3. إضافة قاعدة حاسمة في نهاية §9.5:
   "إذا لم يكن في الرد الحالي دليل صريح أنك قرأت الملفات المطلوبة للسياق الحالي، فلا تبدأ المهمة"

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: 0 عبارات "مرة واحدة" أو "عند الشك" متبقية في TCEA. Anti-Bloat Gate PASS — لا ملفات جديدة.
المخاطر: منخفضة — يقلل التخبط ويزيد موثوقية التنفيذ.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-054 — TCEA: إضافة Runtime Load Order + إزالة الاعتماد على الذاكرة الضمنية

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-054
مصدر الطلب: Audit body finding #3
نوع التغيير: Runtime Reliability / Instruction Precision
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة هيكلة §9 + إضافة §9.5)

الملخص:
إزالة العبارات غير القابلة للتحقق ("مرة واحدة", "عند الشك") واستبدالها بتعليمات صريحة:
1. إضافة §9.5 Runtime Load Order — 6 جداول ترتيب تحميل حسب السياق
2. استبدال 5 عبارات ضمنية بعبارات قابلة للتنفيذ
3. قاعدة حاسمة: "إذا لم يكن دليل صريح أنك قرأت الملفات، فلا تبدأ"

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: 0 عبارات "مرة واحدة" أو "عند الشك" متبقية.
المخاطر: منخفضة.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-055 — TCEA: ضبط Consultation Response Protocol في قالب إلزامي صارم

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-055
مصدر الطلب: Audit body finding #4
نوع التغيير: Response Format Enforcement / Anti-Bloat
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة بناء §5.3 بالكامل)

الملخص:
تحويل Consultation Response Protocol إلى قالب رد إجباري بحدود صارمة:
1. قالب إلزامي: ما فهمته (سطران) + مخاطر (1-3) + اقتراحات (1-3) + أسئلة (حتى 5) + تقسيم (إن لزم)
2. 7 قواعد صارمة لمكافحة الحشو والاستعراض
3. تحديث مرجع §5.3 في تدفق العمل

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: القالب الآن إلزامي ومقيد. Anti-Bloat Gate PASS.
المخاطر: منخفضة.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-056 — TCEA: إضافة §5.4 Source Classification Tags + تطبيقها على جميع البوابات

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-056
مصدر الطلب: Audit body finding #5 (فصل المؤكد عن الاسترشادي عن الافتراضي)
نوع التغيير: Source Classification / Anti-Hallucination
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إضافة §5.4 + تحديث §5.1 + تحديث 3 بوابات في §11)

الملخص:
إضافة نظام الوسوم الإلزامية الأربعة لمصادر المعلومات:

1. إضافة §5.4 — Source Classification Tags:
   - [Confirmed by Majed] ← ✅ مسموح في النطاق المعتمد
   - [Research Hint] ← ❌ ممنوع حتى التأكيد
   - [Assumption] ← ❌ ممنوع حتى التأكيد
   - [Unresolved] ← ❌ ممنوع حتى القرار

2. تحديث §5.1 (Self-Check Protocol) لاستخدام الوسوم الجديدة

3. إضافة قاعدة الحسم في 3 بوابات:
   - §11.3 Final Scope Reconciliation: أي ميزة In Scope موسومة بـ [Research Hint]/[Assumption]/[Unresolved] ← توقف
   - §11.4 Quotation Readiness: أي عنصر تسعير مبني على وسم غير مؤكد ← توقف
   - §11.7 Tera Handoff Readiness: أي عنصر في الهاندوف غير موسوم بـ [Confirmed by Majed] ← توقف

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: جميع البوابات الثلاث تفرض وسم [Confirmed by Majed]. Anti-Bloat Gate PASS.
المخاطر: منخفضة — يمنع دخول التخمين والافتراضات إلى النطاق والتسعير المعتمدين.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```


### SCP-2026-07-05-047 — Merge TeraMonitor.md into monitor.md (Phase 1: Dual-to-Single File)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-047
مصدر الطلب: Majed (طلب تحسين معماري)
نوع التغيير: Anti-Bloat / Architecture Simplification
الملفات المعدلة:
- .opencode/agents/monitor.md (دمج محتوى TeraMonitor.md بالكامل — 117←251 سطراً)
- tera-system/TeraMonitor.md (حذف — بعد نقل المحتوى)
- tera-system/TeraPolicyMap.md (تحديث مرجع Monitor)
- tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md (3 تحديثات للمراجع)
الملخص: دمج ملف مصدر الحقيقة TeraMonitor.md (154 سطراً) في ملف التنفيذ monitor.md (117 سطراً) لينتج ملف واحد (251 سطراً) لكل العميل. تم إزالة 10 محتويات كانت في TeraMonitor.md وغير موجودة في monitor.md (الهوية الكاملة، الموقع في المنظومة، الغرض المفصّل، التدرج الهرمي للمراجع، قاعدة صارمة، التدقيق التراكمي، آلية تنفيذ القواعد، صلاحية رفض الخطة كاملة، العلاقات مع العملاء، مرجع التحسين المستمر). تم حذف TeraMonitor.md وتحديث جميع المراجع. هذا هو النموذج الأول (Phase 1) لتوحيد ملفات العملاء.
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat Gate PASS — ملف واحد بدلاً من اثنين، إزالة 40 سطراً من التكرار. Git diff --check نظيف. جميع المراجع محدثة.
المخاطر: منخفضة — تم الحفاظ على كل المحتوى من كلا الملفين، المراجع محدثة في 3 ملفات.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/monitor.md
2. git checkout -- tera-system/TeraPolicyMap.md
3. git checkout -- tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md
4. git checkout HEAD -- tera-system/TeraMonitor.md (استرجاع الملف المحذوف)
```

### SCP-2026-07-05-048 — Merge TeraAuditor.md into auditor.md (Phase 2: Dual-to-Single File)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-048
مصدر الطلب: Majed (استكمال توحيد ملفات العملاء)
نوع التغيير: Anti-Bloat / Architecture Simplification
الملفات المعدلة:
- .opencode/agents/auditor.md (دمج محتوى TeraAuditor.md بالكامل — 104←~370 سطراً)
- tera-system/TeraAuditor.md (حذف — بعد نقل المحتوى)
- tera-system/TeraPolicyMap.md (تحديث مرجع Auditor)
- tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md (تحديث مرجعين)
الملخص: دمج ملف مصدر الحقيقة TeraAuditor.md (288 سطراً) في ملف التنفيذ auditor.md (104 سطراً) لينتج ملف واحد (~370 سطراً) لكل العميل. تمت إضافة 14 محتوى من TeraAuditor.md: الهوية الكاملة، الموقع في المنظومة، الغرض المفصّل مع 7 أدوار وظيفية، التدرج الهرمي للمراجع، قاعدة صارمة، منهجية 6 مراحل كاملة، جدول تصنيف النتائج مع تعليمات إضافية، بروتوكول عدم اليقين والبحث، بروتوكول التراكم، العلاقات مع العملاء، مرجع التحسين المستمر. تم حذف TeraAuditor.md وتحديث جميع المراجع. Phase 2 من توحيد ملفات العملاء (بعد Monitor).
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat Gate PASS — ملف واحد بدلاً من اثنين. 0 مراجع متبقية لـ TeraAuditor.md في tera-system/ أو .opencode/. Git diff --check نظيف.
المخاطر: منخفضة — تم الحفاظ على كل المحتوى من كلا الملفين، المراجع محدثة في ملفين.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/auditor.md
2. git checkout -- tera-system/TeraPolicyMap.md
3. git checkout -- tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md
4. git checkout HEAD -- tera-system/TeraAuditor.md (استرجاع الملف المحذوف)
```

### SCP-2026-07-05-049 — إصلاح 3 مشاكل في مصدر الحقيقة TeraClientEngagement.md

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-049
مصدر الطلب: Proactive System Stewardship (تحليل مصدر الحقيقة)
نوع التغيير: System Maintenance / Anti-Bloat
الملفات المعدلة:
- tera-system/TeraClientEngagement.md

الملخص:
تصحيح 3 مشاكل هيكلية في ملف مصدر الحقيقة TeraClientEngagement.md بعد تحليل موازٍ للرنتايم:

1. **إصلاح ترقيم §3.7 المكرر** — كان هناك قسمان برقم "### 3.7" (Delivery & Handover و Maintenance & Support):
   - §3.7 Delivery & Handover (بقي)
   - §3.7 Maintenance & Support ← §3.8 Maintenance & Support
   - §3.8 Commercial Estimation Support ← §3.9 Commercial Estimation Support
   - §3.8.1 Quotation Readiness Gate ← §3.9.1
   - §3.8.2 Level 1 vs Level 2 Rule ← §3.9.2
   النتيجة: 9 مجموعات متسلسلة (3.1 → 3.9) كما يصف العنوان

2. **حذف علامة "(جديد)" من بروتوكولين معتمدين**:
   - §3.2.7 Uncertainty Protocol — حذف "(جديد)" من العنوان
   - §3.2.8 Consultation Response Protocol — حذف "(جديد)" من العنوان
   البروتوكولان معتمدان ومستقران — العلامة المؤقتة لم تعد دقيقة

3. **توحيد تنسيق عنوان §13**:
   - من: `## §13. 📝 Self-Improvement & Gap Reporting`
   - إلى: `## 13. Self-Improvement & Gap Reporting`
   توافقاً مع نمط `## NUM. Title` المستخدم في باقي الأقسام (1-12)

الموافقة: Majed — Approved (عبر الأمر "نفذ")
التحقق من الصحة:
- Anti-Bloat Gate ✅ — ملف واحد، إصلاحات مباشرة، لا إضافات
- Policy Map Check ✅ — لا تغيير في مرجع TCEA (يبقى TeraClientEngagement.md)
- Architecture Map Check ✅ — لا تغيير
- No stale references ✅ — تم التحقق بـ grep: لا إشارات لـ §3.8 في أي ملف
- No broken cross-references ✅ — Quotation Readiness Gate يُشار إليها بالاسم وليس الرقم
- Runtime sync required: ❌ لا — الرنتايم لا يشير للأرقام المتغيرة (3.8، 3.9)

المخاطر: معدومة — جميع التغييرات شكلية/ترقيمية، تم التحقق من عدم وجود مراجع للأرقام القديمة
ملاحظات الاسترجاع (Rollback):
1. git checkout -- tera-system/TeraClientEngagement.md
```

### SCP-2026-07-05-049 — Merge TeraDesignReviewer.md into design-reviewer.md (Phase 3: Dual-to-Single File)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-049
مصدر الطلب: Majed (استكمال توحيد ملفات العملاء)
نوع التغيير: Anti-Bloat / Architecture Simplification
الملفات المعدلة:
- .opencode/agents/design-reviewer.md (دمج محتوى TeraDesignReviewer.md بالكامل — 199←~390 سطراً)
- tera-system/TeraDesignReviewer.md (حذف — بعد نقل المحتوى)
- tera-system/design-system/DESIGN_REVIEW_STANDARDS.md (تحديث مرجع Source of Truth)
- tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md (تحديث مرجع)
الملخص: دمج ملف مصدر الحقيقة TeraDesignReviewer.md (258 سطراً) في ملف التنفيذ design-reviewer.md (199 سطراً) لينتج ملف واحد (~390 سطراً). تمت إضافة 11 محتوى من TeraDesignReviewer.md: الهوية الكاملة، الموقع في المنظومة، الغرض المفصّل مع 7 أدوار وظيفية، العلاقات مع 3 عملاء، شروط التفعيل، المراجع المعتمدة والمدخلات/المخرجات، الصلاحيات، الضوابط العربية للمعاينة البصرية، قواعد البروتوتايب بالعربية، مرجع التحسين المستمر. تم حذف TeraDesignReviewer.md وتحديث جميع المراجع. Phase 3 من توحيد ملفات العملاء (بعد Monitor و Auditor).
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat Gate PASS — ملف واحد بدلاً من اثنين. 0 مراجع متبقية لـ TeraDesignReviewer.md. Git diff --check نظيف.
المخاطر: منخفضة — تم الحفاظ على كل المحتوى من كلا الملفين.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/design-reviewer.md
2. git checkout -- tera-system/design-system/DESIGN_REVIEW_STANDARDS.md
3. git checkout -- tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md
4. git checkout HEAD -- tera-system/TeraDesignReviewer.md (استرجاع الملف المحذوف)
```

### SCP-2026-07-05-050 — Merge TeraApplicationBlueprint.md into application-blueprint.md (Phase 4: Dual-to-Single File)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-050
مصدر الطلب: Majed (استكمال توحيد ملفات العملاء)
نوع التغيير: Anti-Bloat / Architecture Simplification
الملفات المعدلة:
- .opencode/agents/application-blueprint.md (دمج محتوى TeraApplicationBlueprint.md بالكامل — 165←~450 سطراً)
- tera-system/TeraApplicationBlueprint.md (حذف — بعد نقل المحتوى)
- .opencode/agents/tera.md (تحديث مرجعين — إزالة TeraApplicationBlueprint.md من القائمة، تحديث المرجع في جدول الاستخدام)
- tera-system/TeraAgent.md (تحديث مرجع — TeraApplicationBlueprint.md ← application-blueprint.md)
- tera-system/TeraArchitectureMap.md (تحديث مرجع)
- tera-system/TeraPreparationDocumentationGovernance.md (تحديث مرجع)
- tera-system/TeraPolicyMap.md (تحديث مرجع — إزالة مسار المصدر)
الملخص: دمج ملف مصدر الحقيقة TeraApplicationBlueprint.md (401 سطراً) في ملف التنفيذ application-blueprint.md (165 سطراً) لينتج ملف واحد (~450 سطراً). تمت إضافة 14 محتوى من TeraApplicationBlueprint.md: الهوية الكاملة بالعربية، الموقع في المنظومة، المسؤوليات الـ 10 مع الأدوار، العلاقات مع 5 عملاء، شروط التفعيل، قواعد التفعيل المتعدد، المراجع، الأدوات، المدخلات والمخرجات، القيود الإضافية، ممنوعات إضافية، المبادئ، بروتوكول Self-Verification، مرجع التحسين المستمر. تم حذف TeraApplicationBlueprint.md وتحديث 6 ملفات مرجعية. Phase 4 من توحيد ملفات العملاء (بعد Monitor, Auditor, DesignReviewer).
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat Gate PASS — ملف واحد بدلاً من اثنين. 0 مراجع متبقية لـ TeraApplicationBlueprint.md في الملفات النشطة. Git diff --check نظيف.
المخاطر: منخفضة — تم الحفاظ على كل المحتوى من كلا الملفين.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/application-blueprint.md
2. git checkout -- .opencode/agents/tera.md
3. git checkout -- tera-system/TeraAgent.md
4. git checkout -- tera-system/TeraArchitectureMap.md
5. git checkout -- tera-system/TeraPreparationDocumentationGovernance.md
6. git checkout -- tera-system/TeraPolicyMap.md
7. git checkout HEAD -- tera-system/TeraApplicationBlueprint.md (استرجاع الملف المحذوف)
```

### SCP-2026-07-05-051 — Merge TeraClientEngagement.md into tera-client-engagement.md (Phase 5: Dual-to-Single File)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-051
مصدر الطلب: Majed (استكمال توحيد ملفات العملاء)
نوع التغيير: Anti-Bloat / Architecture Simplification
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (دمج محتوى TeraClientEngagement.md — 452←~520 سطراً + إضافة §12 و §13)
- tera-system/TeraClientEngagement.md (حذف — بعد نقل المحتوى)
- tera-system/TeraPolicyMap.md (تحديث مرجع مصدر الحقيقة)
- .opencode/agents/tera.md (إزالة TeraClientEngagement.md من قائمة المراجع)
- project-control/archive/SYSTEM_CHANGE_PROPOSAL_2026-07-02.md (أرشفة SCP قديم من جذر المنظومة)
- project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-05-041.md (أرشفة SCP منفذ)
- project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-05-042.md (أرشفة SCP منفذ)
الملخص: دمج ملف مصدر الحقيقة TeraClientEngagement.md (974 سطراً، 13 قسماً) في ملف التنفيذ tera-client-engagement.md (452 سطراً، 10 أقسام). تم تحويل 10 مراجع خارجية `tera-system/TeraClientEngagement.md` إلى مراجع داخلية، وإزالة سطر System Reference، وإضافة §12 Client Document Library (مصفوفة تفعيل 24 نموذجاً) و §13 Self-Improvement & Gap Reporting. تم حذف TeraClientEngagement.md وتحديث TeraPolicyMap.md و tera.md. كما تمت أرشفة 3 SCPs قديمة منفذة. Phase 5 من توحيد ملفات العملاء (بعد Monitor, Auditor, DesignReviewer, ApplicationBlueprint).
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat Gate PASS — ملف واحد بدلاً من اثنين. 0 مراجع متبقية لـ TeraClientEngagement.md في الملفات النشطة. Git diff --check نظيف.
المخاطر: متوسطة — TCEA هو أكبر عميل (974 سطراً مصدر)، لكن المحتوى الرئيسي كان موجوداً مسبقاً في ملف التنفيذ. تمت الإضافة الانتقائية للقسمين المفقودين فقط (Anti-Bloat).
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
2. git checkout -- tera-system/TeraPolicyMap.md
3. git checkout -- .opencode/agents/tera.md
4. git checkout HEAD -- tera-system/TeraClientEngagement.md (استرجاع الملف المحذوف)
```

### SCP-2026-07-05-052 — Merge TeraAgent.md into tera.md (Phase 6: Dual-to-Single File — Final Phase)

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-052
مصدر الطلب: Majed (استكمال توحيد ملفات العملاء — المرحلة الأخيرة)
نوع التغيير: Anti-Bloat / Architecture Simplification
الملفات المعدلة:
- .opencode/agents/tera.md (دمج محتوى TeraAgent.md — إضافة §19 Final Rule + §20 Continuous Improvement; إعادة ترقيم §20→§21; تحديث frontmatter; ترقية المراجع الداخلية)
- tera-system/TeraAgent.md (حذف — آخر ملف مصدر حقيقة مزدوج)
- tera-system/TeraPolicyMap.md (تحديث 7 مراجع — tera-system/TeraAgent.md ← .opencode/agents/tera.md)
- tera-system/TeraArchitectureMap.md (تحديث مرجع)
- tera-system/TeraPreparationDocumentationGovernance.md (تحديث مرجع)
- tera-system/TERA_USER_GUIDE.md (تحديث مرجع)
- tera-system/TeraPreExecutionGate.md (تحديث مرجع)
- tera-system/TeraSubAgents.md (تحديث مرجع)
- tera-system/Tera_Project_Preparation_Files.md (تحديث مرجع)
- tera-system/AGENT_GENERATION_TEMPLATE.md (تحديث مرجعين)
- tera-system/runtime/MVP_DEFINITION_PROTOCOL.md (تحديث مرجعين)
- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md (تحديث مرجع)
الملخص: آخر مرحلة من مشروع توحيد ملفات العملاء (Phase 6/6). تم دمج TeraAgent.md (1,401 سطراً، 39 قسماً، ~60KB) في .opencode/agents/tera.md (558 سطراً، 20 قسماً). أضيف §19 القاعدة النهائية (Final Rule) و §20 التحسين المستمر (Continuous Improvement) من TeraAgent.md، مع إعادة ترقيم §20 القديم إلى §21. تم تحديث frontmatter: إزالة System Reference، إضافة Source of Truth. تم تحديث 11 ملفاً مرجعياً (13 مرجعاً إجمالاً). تم حذف TeraAgent.md. بذلك أصبح كل عميل أساسي في المنظومة يتبع نمط ملف واحد: `.opencode/agents/AGENT.md` هو مصدر الحقيقة وملف التنفيذ معاً.
الموافقة: Majed — Approved
التحقق من الصحة: Anti-Bloat Gate PASS — ملف واحد بدلاً من اثنين (~87KB→~27KB محمل). 0 مراجع متبقية لـ TeraAgent.md في الملفات النشطة.
المخاطر: عالية — TeraAgent.md كان أكثر ملف مرجعي في المنظومة (15+ ملفاً). تم تحديث جميع الملفات النشطة بشكل منهجي.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera.md
2. git checkout -- tera-system/TeraPolicyMap.md
3. git checkout -- tera-system/TeraArchitectureMap.md
4. git checkout -- tera-system/TeraPreparationDocumentationGovernance.md
5. git checkout -- tera-system/TERA_USER_GUIDE.md
6. git checkout -- tera-system/TeraPreExecutionGate.md
7. git checkout -- tera-system/TeraSubAgents.md
8. git checkout -- tera-system/Tera_Project_Preparation_Files.md
9. git checkout -- tera-system/AGENT_GENERATION_TEMPLATE.md
10. git checkout -- tera-system/runtime/MVP_DEFINITION_PROTOCOL.md
11. git checkout -- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
12. git checkout HEAD -- tera-system/TeraAgent.md (استرجاع آخر ملف حقيقة مزدوج)
```

### SCP-2026-07-05-052 — TCEA: إصلاح 7 إحالات رقمية مكسورة وإضافة §11 Quality Gates

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-052
مصدر الطلب: Audit body finding (ثغرة مرصودة من هيئة تدقيق خارجية) + تنفيذ مباشر بتفويض من Majed
نوع التغيير: Bug Fix / Structural Completion
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إصلاح 7 إحالات + إضافة قسم §11 بالكامل)

الملخص:
إصلاح ثغرة الإحالات المكسورة في TCEA:

1. تصحيح 3 إحالات قديمة: §3.2.6→§5.1, §3.2.7→§5.2, §3.2.8→§5.3 (Self-Check, Uncertainty, Consultation Response — المحتوى موجود فعلاً تحت §5)
2. تصحيح 4 إحالات إلى غير موجود: §3.3.1→§11.1 (Final Scope Reconciliation Gate), §3.3.2→§11.2 (Budget-to-Scope Control Rule), §3.3.3→§11.3 (Client Decision Register), §3.6.1→§11.4 (Approval Consistency Check)
3. تعديل الملاحظة التمهيدية (السطر 73) لتعكس أرقام الأقسام الصحيحة
4. إضافة §11 Quality Gates كامل — 4 تعريفات مفصّلة مع: تعريف، قواعد، جهة التنفيذ، جهة الاعتماد، مخرجات البوابة/MAN gates

الموافقة: Majed — Approval pre-granted for this fix (تفويض مباشر)
التحقق من الصحة: تمت قراءة الملف بالكامل بعد التعديل — 0 إحالات مكسورة متبقية. Anti-Bloat Gate PASS — لا ملفات جديدة، فقط إضافة قسم داخل الملف الحالي.
المخاطر: منخفضة — §11 يضيف تعريفات كانت مفقودة؛ لا يغير صلاحيات أو سلوك TCEA بل يكملها.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-053 — TCEA §11: ترقية إلى الصيغة التشغيلية + إضافة 3 بوابات مفقودة

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-053
مصدر الطلب: Audit body finding #2 (Gates بحاجة تعريف تنفيذي صريح)
نوع التغيير: Structural Upgrade / Operational Precision
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة بناء §11 + تحديث 5 إحالات في تدفق العمل)

الملخص:
ترقية جميع تعريفات البوابات في §11 إلى الصيغة التشغيلية الموحدة المطلوبة:
- الاسم | الهدف | المدخلات المطلوبة | شروط النجاح | شروط الإيقاف | الإخراج الإلزامي | هل يمنع الانتقال؟

التغييرات:
1. إعادة بناء 3 بوابات موجودة إلى الصيغة الجديدة:
   - §11.2 Budget-to-Scope Control Rule
   - §11.3 Final Scope Reconciliation Gate (كانت §11.1 سابقاً)
   - §11.6 Approval Consistency Check (كانت §11.4 سابقاً)

2. إضافة 3 بوابات جديدة كانت مذكورة في تدفق العمل لكن بدون تعريف:
   - §11.1 Discovery Coverage Gate (جديد — كان مذكوراً في السطر 85 بدون تعريف)
   - §11.4 Quotation Readiness Gate (جديد — كان مذكوراً في السطر 91 بدون تعريف)
   - §11.7 Tera Handoff Readiness Gate (جديد — كان مذكوراً في السطر 93 بدون تعريف)

3. ترقية Client Decision Register إلى §11.5 بالصيغة التشغيلية (كانت §11.3)

4. تحديث 5 إحالات في تدفق العمل (§3) لتتوافق مع الترقيم الجديد:
   - Discovery Coverage Gate → §11.1
   - Client Decision Register → §11.5 (كانت §11.3)
   - Final Scope Reconciliation Gate → §11.3 (كانت §11.1)
   - Quotation Readiness Gate → §11.4
   - Tera Handoff Readiness Gate → §11.7 (مع Approval Consistency §11.6)

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: 0 إحالات مكسورة. جميع البوابات الـ 7 في §11 تتبع الصيغة الموحدة. Anti-Bloat Gate PASS — لا ملفات جديدة.
المخاطر: منخفضة — §11 أصبح أكثر وضوحاً وإلزامية؛ لا يغير صلاحيات أو سلوك TCEA بل يمنع القفز العشوائي بين المراحل.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-054 — TCEA: إضافة Runtime Load Order + إزالة الاعتماد على الذاكرة الضمنية

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-054
مصدر الطلب: Audit body finding #3
نوع التغيير: Runtime Reliability / Instruction Precision
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة هيكلة §9 + إضافة §9.5)

الملخص:
إزالة العبارات غير القابلة للتحقق واستبدالها بتعليمات صريحة:
1. إضافة §9.5 Runtime Load Order — 6 جداول ترتيب تحميل حسب السياق
2. استبدال 5 عبارات ضمنية ("مرة واحدة", "عند الشك") بعبارات قابلة للتنفيذ
3. قاعدة حاسمة: "إذا لم يكن دليل صريح أنك قرأت الملفات، فلا تبدأ"

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: 0 عبارات "مرة واحدة" أو "عند الشك" متبقية.
المخاطر: منخفضة.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-055 — TCEA: ضبط Consultation Response Protocol في قالب إلزامي صارم

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-055
مصدر الطلب: Audit body finding #4
نوع التغيير: Response Format Enforcement / Anti-Bloat
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة بناء §5.3 بالكامل)

الملخص:
تحويل Consultation Response Protocol إلى قالب رد إجباري بحدود صارمة:
1. قالب إلزامي: ما فهمته (سطران) + مخاطر (1-3) + اقتراحات (1-3) + أسئلة (حتى 5) + تقسيم (إن لزم)
2. 7 قواعد صارمة لمكافحة الحشو والاستعراض
3. تحديث مرجع §5.3 في تدفق العمل

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: القالب الآن إلزامي ومقيد. Anti-Bloat Gate PASS.
المخاطر: منخفضة.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-056 — TCEA: إضافة §5.4 Source Classification Tags + تطبيقها على جميع البوابات

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-056
مصدر الطلب: Audit body finding #5
نوع التغيير: Source Classification / Anti-Hallucination
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إضافة §5.4 + تحديث §5.1 + 3 بوابات)

الملخص:
إضافة نظام الوسوم الإلزامية الأربعة:
1. §5.4 — [Confirmed by Majed] / [Research Hint] / [Assumption] / [Unresolved]
2. قاعدة الحسم: لا يدخل النطاق أو التسعير المعتمد إلا [Confirmed by Majed]
3. فرضت في: §11.3, §11.4, §11.7

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: جميع البوابات تفرض الوسم. Anti-Bloat Gate PASS.
المخاطر: منخفضة.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```

### SCP-2026-07-05-057 — TCEA: تقوية Uncertainty Protocol مع قالب STOP — UNCERTAINTY BLOCK

```text
تاريخ: 2026-07-05
معرف التغيير: SCP-2026-07-05-057
مصدر الطلب: Audit body finding #6
نوع التغيير: Operational Enforcement / Anti-Hallucination
الملفات المعدلة:
- .opencode/agents/tera-client-engagement.md (إعادة بناء §5.2 + تحديث مرجع التدفق)

الملخص:
تحويل Uncertainty Protocol من مجرد "قل لا أعرف" إلى آلية توقف إجبارية بقالب صارم:
1. إضافة قالب STOP — UNCERTAINTY BLOCK الإجباري (5 حقول)
2. مثال تطبيقي مدمج
3. آلية تطبيق: أخرج القالب ← ارفع لـ Majed ← سجّل ← لا تنتقل
4. Websearch يبقى خطوة أولى قبل التوقف
5. تحديث مرجع §5.2 في تدفق العمل

الموافقة: Majed — Approval pre-granted
التحقق من الصحة: القالب الإجباري يمنع النموذج من قول "لا أعرف" ثم المتابعة بصمت.
المخاطر: منخفضة.
ملاحظات الاسترجاع (Rollback):
1. git checkout -- .opencode/agents/tera-client-engagement.md
```
