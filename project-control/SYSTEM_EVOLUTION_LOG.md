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
- CREATE: project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-03-017.md
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
- CREATE: project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-018.md
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
- CREATE: project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-019.md
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
- CREATE: project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-022.md
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
- UPDATE: project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-023.md
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
- UPDATE: project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-028.md
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
- UPDATE: project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-030.md
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
