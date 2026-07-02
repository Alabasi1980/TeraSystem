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

### الإدخال الأول — SCP-2026-07-01-001

```
تاريخ: 2026-07-01
معرف التغيير: SCP-2026-07-01-001
مصدر الطلب: User Request (Majed)
نوع التغيير: New Governance Session Agent
الملفات المعدلة:
- تم إنشاء: .opencode/agents/tera-system-evolution.md
- تم إنشاء: project-control/SYSTEM_EVOLUTION_LOG.md
- تم تحديث: tera-system/TeraSubAgents.md (§14.4)
- تم تحديث: tera-system/AGENT_ACTIVATION_MATRIX.md (§2.4)
- تم تحديث: tera-system/TeraPolicyMap.md
- تم تحديث: tera-system/TeraArchitectureMap.md
- تم تحديث: tera-system/TeraSystemMaintenanceChecklist.md
- تم تحديث: .opencode/agents/tera.md
الملخص:
إنشاء TeraSystemEvolutionAgent كعميل حوكمة مستقل لتطوير منظومة Tera نفسها،
مع سجل تطور نظامي (SYSTEM_EVOLUTION_LOG.md) وحوكمة صارمة (Anti-Bloat Gate,
الموافقة قبل التنفيذ، عدم العمل على تطبيقات العملاء).
الموافقة: Majed — Approved with conditions
التحقق من الصحة: Git diff --check: PASS
المخاطر: منخفضة — العميل مستقل، لا يمس تطبيقات العملاء، ولا ينفذ بدون موافقة.
ملاحظات الاسترجاع (Rollback):
1. حذف .opencode/agents/tera-system-evolution.md
2. حذف project-control/SYSTEM_EVOLUTION_LOG.md
3. عكس التغييرات في TeraSubAgents.md / AGENT_ACTIVATION_MATRIX.md
4. عكس التغييرات في TeraPolicyMap.md / TeraArchitectureMap.md
5. عكس التغييرات في TeraSystemMaintenanceChecklist.md / tera.md
```


### الإدخال الثاني — SCP-2026-07-01-002

```
تاريخ: 2026-07-01
معرف التغيير: SCP-2026-07-01-002
مصدر الطلب: User Request (Majed)
نوع التغيير: Agent Improvement / System Feedback Loop
الملفات المعدلة:
- تم إنشاء: project-control/AGENT_GAPS_LOG.md
- تم تحديث: .opencode/agents/auditor.md
- تم تحديث: .opencode/agents/monitor.md
- تم تحديث: .opencode/agents/design-reviewer.md
- تم تحديث: .opencode/agents/tera-system-evolution.md
- تم تحديث: tera-system/TeraSubAgents.md
- تم تحديث: tera-system/TeraPolicyMap.md
- تم تحديث: tera-system/TeraArchitectureMap.md
الملخص:
إنشاء خط تغذية راجعة لتطوير العملاء الأساسيين عبر AGENT_GAPS_LOG.md، بحيث يسجل العملاء فجواتهم ومشكلاتهم واقتراحاتهم، ويقوم TeraSystemEvolutionAgent بمراجعتها وتحديد حالتها (Pending / Under Review / Approved / Applied / Rejected / Duplicate / Deferred) قبل أي تطوير فعلي.
الموافقة: Majed — Approved
التحقق من الصحة: Git diff --check: PASS
المخاطر: منخفضة إلى متوسطة — تم ضبطها عبر منع التنفيذ الذاتي، منع التكرار، وحصر المعالجة في TeraSystemEvolutionAgent.
ملاحظات الاسترجاع (Rollback):
1. حذف project-control/AGENT_GAPS_LOG.md
2. عكس إضافات Self-Improvement Reporting في ملفات العملاء
3. عكس تحديثات TeraSubAgents.md / TeraPolicyMap.md / TeraArchitectureMap.md
4. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال الثالث — SCP-2026-07-01-003

```
تاريخ: 2026-07-01
معرف التغيير: SCP-2026-07-01-003
مصدر الطلب: User Request (Majed) — التحقق من Developer Best Practice.md
نوع التغيير: Policy Update / Gap Closure
الملفات المعدلة:
- تم تحديث: tera-system/engineering-governance/ENGINEERING_BEST_PRACTICES.md
- تم تحديث: tera-system/engineering-governance/ENGINEERING_REVIEW_CHECKLIST.md
- تم تحديث: tera-system/engineering-governance/ENGINEERING_GOVERNANCE_GATE.md
- تم حذف: temp/Developer Best Practice.md
- تم حذف: temp/Note01.md (كان قد أُعيدت تسميته)
الملخص:
سد الفجوات بين مصدر المالك (Developer Best Practice.md) ومنظومة Tera:
- إضافة §9 Naming Conventions
- إضافة §17 External Integration
- إضافة §18 Idempotency and Concurrency
- تحديث AI Governance من 8 إلى 11 قاعدة
- تحديث Non-Negotiables من 10 إلى 12 قاعدة
- إضافة فحوصات للمسميات والتكامل الخارجي والتزامن في بوابة المراجعة
- حذف ملف المصدر بعد استيعاب محتواه
الموافقة: Majed — Approved (مباشر)
التحقق من الصحة: Git diff --check
المخاطر: منخفضة
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/engineering-governance/ENGINEERING_BEST_PRACTICES.md
2. git restore tera-system/engineering-governance/ENGINEERING_REVIEW_CHECKLIST.md
3. git restore tera-system/engineering-governance/ENGINEERING_GOVERNANCE_GATE.md
4. استرجاع temp/Developer Best Practice.md من git
5. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال الرابع — SCP-2026-07-02-001

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-001
مصدر الطلب: User Request (Majed) — إنشاء TeraClientEngagementAgent
نوع التغيير: New Governance Session Agent + Policy Update + Architecture Update + Protocol Update
الملفات المنشأة:
- tera-system/TeraClientEngagement.md (مصدر الحقيقة المرجعي — 5 أجزاء)
- .opencode/agents/tera-client-engagement.md (ملف العميل النشط لجلسات OpenCode)
الملفات المعدلة (النظام الأساسي):
- tera-system/TeraAgent.md — تحويل Phase 1 (14 تغييراً), إزالة مسؤوليات الزبون, تحديث شجرة $39
- tera-system/TeraSubAgents.md — إضافة §14.5 TeraClientEngagementAgent
- tera-system/AGENT_ACTIVATION_MATRIX.md — إضافة §2.4 CLIENT_REQUEST trigger
- tera-system/AGENT_PERMISSION_MODEL.md — إضافة TeraClientEngagementAgent (WRITE_DOCS)
- tera-system/TeraPolicyMap.md — إضافة المصادر الجديدة
- tera-system/TeraArchitectureMap.md — تحديث صف Client engagement مع Client Engagement Workflow
- tera-system/TeraSystemMaintenanceChecklist.md — إضافة §7.1 فحوصات خاصة
- tera-system/TeraClientPolicy.md — إضافة ملاحظة نظامية: مسؤولية تنفيذ السياسة انتقلت
- tera-system/TeraProjectIntakePolicy.md — إضافة ملاحظة: مسار بديل مع TERA_HANDOFF_PACKAGE.md
الملفات المعدلة (ملفات العملاء):
- .opencode/agents/tera.md — إضافة مرجع TeraClientEngagement.md + ملاحظة عن مسار التعامل مع الزبون
الملفات المعدلة (Runtime — تحديث محدود):
- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md — تحديث §14 و §18 مع ملاحظات نظامية
- tera-system/runtime/TERA_RUNTIME_TEMPLATES.md — تحديث §21 و §25 مع ملاحظات نظامية
- tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md — تحديث Phase 1 + مسار بديل
الملخص:
إنشاء TeraClientEngagementAgent كعميل حوكمة مستقل لإدارة دورة حياة الزبون.
العميل الجديد: اسمه TeraClientEngagementAgent، معرفه CLIENT_ENGAGEMENT_AGENT،
نوعه Client Lifecycle Session Agent (جلسة حوكمة مستقلة).
يحل محل TeraAgent في كل ما يتعلق بالزبون: Discovery, Proposal, Approval, Change Control.
TeraAgent لم يعد يتواصل مع الزبون — يستقبل TERA_HANDOFF_PACKAGE.md جاهزة.
كل التواصل عبر Majed فقط. لا عملاء فرعيين. لا MCPs. لا CRM.
المصطلح المعماري: "Client Engagement Workflow" (وليست Layer).
مجلد client-engagement/ يُنشأ فقط عند وجود تطبيق عميل فعلي.
مصدر الأسئلة: TeraApplicationQuestionBank.md + أسئلة استشارية/تجارية إضافية.
Websearch تلقائي بعد الفهم الأولي.
التسعير: Scope-Based Pricing + Effort Estimation — مسودات فقط.
كل الوثائق مسودات (Draft-only) حتى موافقة Majed الصريحة.
الموافقة: Majed — Approved with Conditions

### التصحيح الثاني (Second Pass) — 2026-07-02

بعد اختبار TeraClientEngagementAgent، اكتُشفت بقايا مسؤوليات زبون لم تُنقل. تم تصحيحها:

الملفات المعدلة إضافياً:
- tera-system/TeraAgent.md:
  - §3 — إزالة 7 مسؤوليات زبون وإبقاء 3 بنود حوكمة فقط + إضافة ملاحظة نظامية
  - §13 — إزالة ClientDiscoveryAgent, ProposalScopeAgent, ClientApprovalReviewAgent, ChangeControlAgent من قائمة العملاء المشروطوين
  - §14 — إعادة كتابة كاملة: مسار خارجي (تخطي Discovery) + مسار داخلي (Discovery كامل)
  - §34 — تحديث شرط Build Mode ليشير إلى TERA_HANDOFF_PACKAGE.md بدلاً من client approval package
  - Phase 7 — توثيق أن Client Handover Package من TCEA
  - إضافة بروتوكول Maintenance & Support Bridge بعد Phase 7
- tera-system/AGENT_PERMISSION_MODEL.md §3.3 — استبدال 4 عملاء فرعيين بـ TeraClientEngagementAgent
- tera-system/AGENT_ACTIVATION_MATRIX.md §2.3 — إزالة جدول العملاء + إضافة ملاحظة نظامية; §2.4 — دمج صفي TCEA في صف واحد

### التصحيح الثالث (Third Pass — ملفات إضافية) — 2026-07-02

بعد فحص كامل لملفات `tera-system/`، اكتُشفت ملفات إضافية تحتوي مراجع قديمة:

الملفات المعدلة:
- tera-system/TERA_PROJECT_DECISION.md — §1 (مرجع TERA_HANDOFF_PACKAGE.md), §7 (TCEA سياق), §13 (حكم Build Mode)
- tera-system/TeraApplicationQuestionBank.md — إضافة توطئة عن مسار Handoff البديل
- tera-system/TERA_USER_GUIDE.md — تحديث المطالب (إحالة إلى TCEA بدلاً من إعداد الحزمة مباشرة)
- tera-system/TeraScenarioStressTests.md — تحديث 3 سيناريوهات (اكتشاف, تغيير, تصميم)
- tera-system/Tera_Project_Preparation_Files.md — إضافة ملاحظة ملكية TCEA
- tera-system/TeraPreExecutionGate.md — إضافة TERA_HANDOFF_PACKAGE.md إلى بوابات التحقق
- tera-system/TeraTokenPolicy.md — تحديث شرط Build Mode

التحقق من الصحة: Git diff --check: PASS
المخاطر:
- متوسطة: تعديل TeraAgent.md جوهري — تحول 14 مقطعاً في التعديل الأول + 6 مقاطع في التصحيح الثاني
- منخفضة للبقية
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/TeraAgent.md (أكبر ملف تغيير)
2. git restore tera-system/AGENT_PERMISSION_MODEL.md
3. git restore tera-system/AGENT_ACTIVATION_MATRIX.md
4. حذف tera-system/TeraClientEngagement.md
5. حذف .opencode/agents/tera-client-engagement.md
6. عكس تغييرات TeraSubAgents.md
7. عكس تغييرات TeraPolicyMap.md / TeraArchitectureMap.md / TeraSystemMaintenanceChecklist.md
8. عكس تغييرات TeraClientPolicy.md / TeraProjectIntakePolicy.md
9. عكس تغييرات .opencode/agents/tera.md
10. عكس تغييرات runtime (TERA_RUNTIME_PROTOCOLS.md / TEMPLATES.md / CHECKLISTS.md)
11. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال الخامس — SCP-2026-07-02-002

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-002
مصدر الطلب: User Request (Majed) — اكتشاف بقايا عمل تيرا القديم في 5 ملفات
نوع التغيير: Policy Update / Architecture Update / Anti-Bloat

الملفات المعدلة:

1. tera-system/TeraSubAgents.md §6.0
   - استبدال جدول العملاء الأربعة القدامى (ClientDiscoveryAgent, ProposalScopeAgent,
     ClientApprovalReviewAgent, ChangeControlAgent) بنص إزالة وإحالة إلى TCEA §14.5

2. tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md §18
   - إضافة توطئة نظامية: توضح أن البروتوكول يُستخدم من TCEA للمشاريع الخارجية
     ومن TeraAgent للمشاريع الداخلية
   - تغيير "Guide Tera" إلى "Guide the responsible agent"

3. tera-system/TeraProjectIntakePolicy.md
   - إعادة تنظيم كاملة: توضيح أن الملف للمشاريع الداخلية فقط
   - §1: إعادة كتابة الهدف
   - §2: إزالة متطلبات client workspace للمشاريع الخارجية
   - §6: إعادة كتابة — "Majed" بدلاً من "the client"، إزالة "For external client projects"
   - §7: إزالة الأسئلة الخاصة بالعميل الخارجي
   - §8: إزالة شروط client approval preparation

4. tera-system/TERA_PROJECT_DECISION.md
   - §2: تقسيم إلى مسارين — خارجي (TERA_HANDOFF_PACKAGE.md) / داخلي (project-inputs/)
   - §6: تحديث المصادر لتعكس المسارين
   - §7: تصحيح "Ikhtiyar" → "اختياري"
   - §11: تقسيم صف Discovery/Intake إلى صفين — خارجي/داخلي
   - §13: توضيح مصدر 00_PROJECT_INPUTS.md حسب المسار

5. tera-system/TeraClientPolicy.md
   - إعادة تنظيم 6 أقسام:
     - Header: توضيح أن السياسة تحدد القواعد—المنفذ يختلف حسب نوع المشروع
     - §1: إعادة كتابة الهدف مع توضيح المنفذ
     - §5.2: توضيح أن الـ Proposal يُنتج من TCEA (خارجي) أو Tera (داخلي)
     - §6: تقسيم Client Discovery إلى قسمين—خارجي (TCEA) وداخلي (Tera)
     - §7: توضيح أن Approval Package من TCEA للمشاريع الخارجية
     - §8: توضيح أن Change Control من TCEA للمشاريع الخارجية

الملخص:
إعادة تنظيم 5 ملفات في tera-system/ لإزالة التناقض بينها وبين نموذج TCEA.
بدلاً من إضافة "ملاحظات نظامية" فوق المحتوى القديم (ترقيع)، تمت إعادة كتابة
الأقسام المتأثرة لتكون بمسارين واضحين: خارجي (TCEA) / داخلي (Tera).
المبدأ: المحتوى نفسه صحيح—المنفذ هو الذي يختلف.

الموافقة: Majed — Approved
التحقق من الصحة: Git diff --check: PASS
المخاطر:
- متوسطة: TeraClientPolicy.md أكبر ملف تعديل (~100 سطر جديد)
- منخفضة للبقية

ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/TeraClientPolicy.md
2. git restore tera-system/TERA_PROJECT_DECISION.md
3. git restore tera-system/TeraProjectIntakePolicy.md
4. git restore tera-system/TeraSubAgents.md
5. git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
6. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال الثاني عشر — SCP-2026-07-02-009

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-009
مصدر الطلب: User Request (Majed) — Phase 5 Scoped Runtime Override
نوع التغيير: Runtime Sync / Delegation Control Enhancement / Policy Update

الملفات المعدلة:
- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
- tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
- project-control/tasks/TASK_TEMPLATE.md
- tera-system/AGENT_PERMISSION_MODEL.md
- tera-system/TeraSubAgents.md
- tera-system/TeraAgent.md
- .opencode/agents/tera.md
- tera-system/TeraPolicyMap.md
- tera-system/TeraArchitectureMap.md

الملخص:
إضافة نموذج رسمي لـ Scoped Runtime Override يسمح لـ Tera بتعديل بعض حدود التفويض أثناء المهمة
دون إعادة التفويض من الصفر، ولكن فقط داخل نطاق المهمة المعتمد.

تم تعريف ما يجوز تعديله مثل:
- Allowed Write Targets
- Allowed Sources
- Context Scope
- Review escalation
- Reassign Writer

وتم منع ما يلي صراحةً:
- تغيير scope المعتمد للمهمة
- تجاوز Pre/Post Gates
- منح قبول نهائي
- استخدام Runtime Override كبديل عن إعادة التخطيط عندما تتغير طبيعة المهمة

القيد غير القابل للتفاوض بقي ثابتاً:
No acceptance without physical review.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check: PASS, runtime-override grep: PASS)
المخاطر:
- متوسطة — زيادة مرونة التشغيل قد تُساء إذا لم تُوثق جيداً؛ تم احتواؤها عبر منع التوسع الصامت وربط كل override بسجل واضح.
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
2. git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
3. git restore project-control/tasks/TASK_TEMPLATE.md
4. git restore tera-system/AGENT_PERMISSION_MODEL.md
5. git restore tera-system/TeraSubAgents.md
6. git restore tera-system/TeraAgent.md
7. git restore .opencode/agents/tera.md
8. git restore tera-system/TeraPolicyMap.md
9. git restore tera-system/TeraArchitectureMap.md
10. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال الحادي عشر — SCP-2026-07-02-008

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-008
مصدر الطلب: User Request (Majed) — Phase 4 Fast Path for Low-Risk Tasks
نوع التغيير: Runtime Sync / Delegation Efficiency Enhancement / Policy Update

الملفات المعدلة:
- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
- tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md
- tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
- tera-system/TeraAgent.md
- .opencode/agents/tera.md
- tera-system/TeraPolicyMap.md

الملخص:
إضافة Fast Path رسمي للمهام منخفضة المخاطر لتقليل الاحتكاك التحضيري فقط.
تم تعريف شروط الأهلية والاستبعاد، وتأكيد أن Fast Path:
- لا يلغي TASK-ID
- لا يلغي Allowed Write Targets
- لا يلغي Acceptance Criteria
- لا يلغي Handback
- لا يلغي Post-Execution Review

القيد غير القابل للتفاوض بقي ثابتاً:
No acceptance without physical review.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check: PASS, fast-path invariant grep: PASS)
المخاطر:
- متوسطة — خطر تصنيف مهمة غير مناسبة كـ low-risk، تم احتواؤه عبر disqualifiers صريحة وتثبيت المراجعة الفعلية.
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
2. git restore tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md
3. git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
4. git restore tera-system/TeraAgent.md
5. git restore .opencode/agents/tera.md
6. git restore tera-system/TeraPolicyMap.md
7. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال العاشر — SCP-2026-07-02-007

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-007
مصدر الطلب: User Request (Majed) — Phase 3 Intervention Logging
نوع التغيير: Agent Improvement / Runtime Sync / Policy Update

الملفات المعدلة:
- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
- tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
- tera-system/TeraSubAgents.md
- tera-system/AGENT_ACTIVATION_MATRIX.md
- tera-system/TeraPolicyMap.md
- tera-system/TeraArchitectureMap.md
- tera-system/TeraAgent.md
- .opencode/agents/tera.md

الملخص:
إضافة نموذج رسمي لتسجيل تدخلات Tera على العملاء الفرعيين داخل المشروع.
تم اعتماد أنواع التدخل التالية:
- Stop
- Narrow
- Restrict
- Suspend
- Reinstate

وتم تعريف أماكن التسجيل الرسمية:
- `project-control/SUB_AGENT_STATUS.md`
- `project-control/PROJECT_ACTIVITY_LOG.md` عند الحاجة
- task file عند ارتباط التدخل بمهمة محددة

كما تم تثبيت أن Intervention Logging:
- لا يساوي قرار قبول
- لا يستبدل Emergency Response للحوادث الشديدة
- لا يكسر القاعدة الذهبية:

No acceptance without physical review.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check: PASS, intervention grep: PASS)
المخاطر:
- منخفضة إلى متوسطة — تمت إضافة طبقة توثيق تشغيلية فقط دون تخفيف البوابات أو المراجعة النهائية.
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
2. git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
3. git restore tera-system/TeraSubAgents.md
4. git restore tera-system/AGENT_ACTIVATION_MATRIX.md
5. git restore tera-system/TeraPolicyMap.md
6. git restore tera-system/TeraArchitectureMap.md
7. git restore tera-system/TeraAgent.md
8. git restore .opencode/agents/tera.md
9. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال التاسع — SCP-2026-07-02-006

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-006
مصدر الطلب: User Request (Majed) — Phase 2 Trust Metadata Layer
نوع التغيير: Agent Improvement / Policy Update / Runtime Sync

الملفات المعدلة:
- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
- tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
- tera-system/TeraSubAgents.md
- tera-system/AGENT_PERMISSION_MODEL.md
- tera-system/AGENT_ACTIVATION_MATRIX.md
- tera-system/TeraPolicyMap.md
- tera-system/TeraArchitectureMap.md
- tera-system/TeraAgent.md
- .opencode/agents/tera.md

الملخص:
إضافة طبقة Trust Metadata للعملاء الفرعيين دون تغيير سلوك القبول النهائي.
تم اعتماد `project-control/SUB_AGENT_STATUS.md` كمصدر تشغيلي داخل المشروع لحالة العميل
وجودته ومستوى الثقة الحالي (New / Observed / Verified / Trusted / Restricted / Suspended).

تم تثبيت القواعد التالية صراحةً:
- Trust Level ≠ Permission Level
- Trust Level ≠ Activation Trigger
- Trust Level ≠ Acceptance Authority
- No Trust Level may bypass Post-Execution Review Gate

القيد غير القابل للتفاوض بقي ثابتاً:
No acceptance without physical review.
لا يجوز قبول مخرجات أي عميل فرعي دون فتح الملفات الفعلية والتحقق من التنفيذ عملياً.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check: PASS, trust invariant grep: PASS)
المخاطر:
- منخفضة إلى متوسطة — تمت إضافة Metadata تشغيلية فقط دون تخفيف البوابات أو الصلاحيات.
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
2. git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
3. git restore tera-system/TeraSubAgents.md
4. git restore tera-system/AGENT_PERMISSION_MODEL.md
5. git restore tera-system/AGENT_ACTIVATION_MATRIX.md
6. git restore tera-system/TeraPolicyMap.md
7. git restore tera-system/TeraArchitectureMap.md
8. git restore tera-system/TeraAgent.md
9. git restore .opencode/agents/tera.md
10. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال الثامن — SCP-2026-07-02-005

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-005
مصدر الطلب: User Request (Majed) — Phase 1 Stabilization before Trusted Delegation
نوع التغيير: Anti-Bloat / Policy Update / Architecture Sync / Agent Improvement

الملفات المعدلة:
- project-control/AGENT_GAPS_LOG.md
- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
- tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
- tera-system/TeraSubAgents.md
- tera-system/TeraPolicyMap.md
- tera-system/TeraArchitectureMap.md
- tera-system/TeraApplicationQuestionBank.md
- tera-system/TeraProjectIntakePolicy.md

الملخص:
تنفيذ المرحلة التمهيدية الأولى قبل أي تطوير لنموذج Trusted Delegation.
تمت مزامنة نقطة البداية التشغيلية بحيث يبدأ TeraAgent بعد Handoff من TeraClientEngagementAgent،
وتنظيف المراجع القديمة في البروتوكولات والقوالب والخرائط، وإدراج TeraAgent صراحةً في
AGENT_GAPS_LOG.md ضمن العملاء المسموح لهم بالتبليغ عن فجواتهم.

قيد غير قابل للتفاوض تم الحفاظ عليه دون أي تخفيف:
No acceptance without physical review.
لا يجوز قبول مخرجات أي عميل فرعي دون فتح الملفات الفعلية والتحقق من التنفيذ عملياً.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check: PASS, stale reference grep: PASS)
المخاطر:
- منخفضة إلى متوسطة — تغييرات مرجعية وتزامن مسؤوليات، دون تغيير سلوك القبول أو بوابات المراجعة.
ملاحظات الاسترجاع (Rollback):
1. git restore project-control/AGENT_GAPS_LOG.md
2. git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
3. git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
4. git restore tera-system/TeraSubAgents.md
5. git restore tera-system/TeraPolicyMap.md
6. git restore tera-system/TeraArchitectureMap.md
7. git restore tera-system/TeraApplicationQuestionBank.md
8. git restore tera-system/TeraProjectIntakePolicy.md
9. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال السادس — SCP-2026-07-02-003

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-003
مصدر الطلب: User Request (Majed) — إعادة تعريف TeraSystemEvolutionAgent كـ System Steward/Guardian
نوع التغيير: Agent Improvement / System Governance Hardening

الملفات المعدلة:
- .opencode/agents/tera-system-evolution.md

الملخص:
إعادة كتابة كاملة لتعريف TeraSystemEvolutionAgent ليصبح العميل المسؤول بوضوح عن
سلامة، تنظيم، تنظيف، تحديث، تطوير، ومنع تضخم منظومة Tera، وعلى رأسها tera-system/.

التغيير أضاف:
- Core Mandate واضح: tera-system/ هو المسؤولية الأولى والمستمرة.
- Primary Responsibility Domains: tera-system/، .opencode/agents/، وسجلات project-control.
- Core Duties: الصيانة، التطوير، حماية الحقيقة، حل التناقضات، سلامة المعمارية، Anti-Bloat، حوكمة العملاء الأساسيين، وإدارة فجوات العملاء.
- Priority Order واضح عند التعارض.
- Operating Modes: استباقي، تفاعلي، معالجة فجوات، وبحث إلى تغيير نظامي.
- Authority Model: Detect broadly / Propose broadly / Execute narrowly / Never edit without approval.
- Validation Gates إضافية تشمل stale references و no unauthorized privilege expansion.
- حدود نهائية أوضح تمنع التنفيذ أو تضخيم المنظومة بدون موافقة.

الموافقة: Majed — Approved
التحقق من الصحة: Git diff --check: PASS
المخاطر:
- متوسطة: تعريف عميل حساس أُعيدت كتابته بالكامل.
- منخفضة تنفيذياً: لا توجد زيادة صلاحيات تنفيذية؛ بقيت edit/write/bash = ask.

ملاحظات الاسترجاع (Rollback):
1. git restore .opencode/agents/tera-system-evolution.md
2. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

### الإدخال السابع — SCP-2026-07-02-004

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-004
مصدر الطلب: User Request (Majed) — حملة تنظيف شاملة للمنظومة
نوع التغيير: Anti-Bloat / Agent Improvement / Policy Update / Architecture Update

الملفات المعدلة:

1. tera-system/TeraAgent.md
   - إعادة كتابة كاملة وفق هيكل جديد (14 قسماً)
   - إزالة: Client Discovery، Smart Interview، إنشاء مساحة العمل، Slash Commands
   - إضافة: حدود واضحة (لا يطور المنظومة، لا يعدّل tera-system/)
   - إضافة: مبدأ البدء — لا عمل بدون Handoff من TCEA
   - إعادة تنظيم: Phases 2-7 في §4، الحوكمة في §6، العملاء في §5، إلخ

2. .opencode/agents/tera.md
   - إعادة كتابة لتتوافق مع TeraAgent.md الجديد
   - إزالة Slash Commands (موجودة في .opencode/commands/)
   - تركيز على التشغيل العملي مع إحالة التفاصيل إلى TeraAgent.md

3. tera-system/TeraClientEngagement.md
   - إضافة §3.6 (إنشاء مساحة العمل) كمسؤولية جديدة
   - تحديث §5.1 (تدفق قبل التنفيذ) ليشمل إنشاء مساحة العمل قبل التسليم

4. .opencode/agents/tera-client-engagement.md
   - إضافة "إنشاء مساحة العمل" إلى المسؤوليات
   - تحديث تدفق العمل
   - إضافة قسم "مسموح به" مع إنشاء مساحة العمل

5. tera-system/TeraPolicyMap.md
   - تحديث مراجع TeraAgent.md (Section 36 → §1.6، Section 5 → §4)

6. tera-system/TeraArchitectureMap.md
   - تحديث Core Flow ليعكس مرحلتي TCEA و TeraAgent بوضوح
   - تحديث وصف Project intake layer

الملخص:
حملة تنظيف شاملة لإعادة تعريف TeraAgent وتحديد حدوده بدقة.
القرارات الرئيسية:
- كل ما يخص الزبون/العميل (Discovery، Proposal، Intake) من اختصاص TCEA حصراً.
- TCEA ينشئ مساحة العمل ويسلّمها جاهزة لـ TeraAgent.
- TeraAgent يبدأ فقط بعد استلام Handoff Package معتمد.
- TeraAgent لا يطور المنظومة ولا يعدّل tera-system/.
- Slash Commands أزيلت من ملف TeraAgent (موجودة في .opencode/commands/).
- إعادة هيكلة كاملة للملف إلى 14 قسماً منظمة.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check: PASS, Anti-Bloat Gate: PASS)

### تحسين لاحق — 2026-07-02 — سد ثغرات `tera.md`

بعد مراجعة المخاطر مع Majed، تم تحسين `tera.md`:
- إضافة §2 🛡️ **Hard Rules** — 10 قواعد صارمة مدمجة (لا تحتاج فتح ملف خارجي)
- سد فجوة **Section 8** (Quick Reference) — إضافة: Post-Execution Review، Emergency Response، Commit/Tag/Push، Secret Redaction
- إضافة §11 **Safety Net** — قاعدة "إذا شككت ابقَ في Plan Mode" + خطوات عملية للمواقف غير المتوقعة
- ترقيم الأقسام (من 11 قسماً بدلاً من 9)
المخاطر:
- متوسطة: إعادة كتابة ملفي TeraAgent الأساسيين
- منخفضة للبقية
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/TeraAgent.md
2. git restore .opencode/agents/tera.md
3. git restore tera-system/TeraClientEngagement.md
4. git restore .opencode/agents/tera-client-engagement.md
5. git restore tera-system/TeraPolicyMap.md
6. git restore tera-system/TeraArchitectureMap.md
7. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

---

## SCP-2026-07-03-012: إنشاء SoftwareDesignerAgent مع إلغاء ExecutionPreparationAgent

| الحقل | القيمة |
|---|---|
| **Date** | 2026-07-03 |
| **Change ID** | SCP-2026-07-03-012 |
| **Request Source** | SYSTEM_CHANGE_PROPOSAL (معتمد من Majed) |
| **Change Type** | New Core Agent / Architecture Upgrade / Agent Replacement |
| **Proposed by** | TeraSystemEvolutionAgent |
| **Approved by** | Majed |

### ما تم تنفيذه

1. **إنشاء `SoftwareDesignerAgent`** — عميل تصميم تقني إلزامي لكل مهمة تنفيذية `TASK-COD-*`.
2. **إلغاء `ExecutionPreparationAgent`** — استبداله بالكامل.
3. **دمج `Task Engineering Review`** داخل `TECHNICAL_SPECIFICATION.md` — لم تعد خطوة منفصلة.
4. **إضافة قالب `TECHNICAL_SPECIFICATION.md`** في `TERA_RUNTIME_TEMPLATES.md` §31.5.
5. **تحديث كل الملفات النظامية** لتعكس الـ Agent الجديد.

### ما تغير

| الملف | التغيير |
|---|---|
| `.opencode/agents/tera-software-designer.md` | **جديد** — تعريف الـ Agent النشط |
| `tera-system/TeraSubAgents.md` §6.9 | **استبدال** — ExecutionPreparationAgent ← SoftwareDesignerAgent |
| `tera-system/AGENT_ACTIVATION_MATRIX.md` | **تحديث** — تفعيل إلزامي لكل المشاريع |
| `tera-system/AGENT_PERMISSION_MODEL.md` | **تحديث** — `PLAN_ONLY` للـ Agent الجديد |
| `tera-system/TeraAgent.md` §§5, 6 | **تحديث** — استبدال الإشارات |
| `tera-system/TeraPreExecutionGate.md` §3.6 | **تحديث** — Technical Specification إلزامي |
| `project-control/tasks/TASK_TEMPLATE.md` §6.1 | **تحديث** — ربط Technical Specification |
| `tera-system/TeraPolicyMap.md` | **تحديث** — إضافة مصدر حقيقة |
| `tera-system/TeraArchitectureMap.md` | **تحديث** — إضافة SoftwareDesignerAgent في Core Flow |
| `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | **تحديث** — استبدال إشارات الـ Orchestration |
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | **تحديث** — إضافة قالب Technical Specification |
| `tera-system/TeraSystemMaintenanceChecklist.md` | **تحديث** — إضافة فحوصات الـ Agent |
| `.opencode/agents/tera.md` | **تحديث** — استبدال الإشارات |

### ما أُزيل

- **`ExecutionPreparationAgent`** من `TeraSubAgents.md` §6.9 (استُبدل)
- **`Task Engineering Review` كخطوة منفصلة** (أُدمج في Technical Specification)
- **Fast Path exemption** للتصميم التقني (لم يعد مسموحاً)

### Validation

- ✅ All ExecutionPreparationAgent references updated or converted to history notes
- ✅ No stale references in client apps or task files
- ✅ 12 files modified + 1 new file = 237 lines added, ~132 removed
- ✅ Anti-Bloat Gate: PASS — replaces + integrates, no new bloat
- ✅ Architecture Map consistency: SoftwareDesignerAgent in Core Flow
- ✅ Policy Map: new source of truth added
- ✅ No client-app contamination
- ✅ No unauthorized privilege expansion (PLAN_ONLY only)

### المخاطر

| المخاطرة | المستوى | خطة التخفيف |
|---|---|---|
| إلغاء ExecutionPreparationAgent قد يؤثر على مهام قديمة | منخفض | الـ Agent الجديد يدمج جميع وظائف القديم |
| Technical Specification تطيل وقت التحضير | منخفض | الـ Agent يعمل بالتوازي مع Tera |
| اعتماد على وثائق تحضير غير مكتملة | متوسط | الـ Agent ينتج Design Gap بدلاً من التخمين |

### ملاحظات الاسترجاع (Rollback)

1. `git restore .opencode/agents/tera.md`
2. `git restore tera-system/TeraSubAgents.md`
3. `git restore tera-system/AGENT_ACTIVATION_MATRIX.md`
4. `git restore tera-system/AGENT_PERMISSION_MODEL.md`
5. `git restore tera-system/TeraAgent.md`
6. `git restore tera-system/TeraPreExecutionGate.md`
7. `git restore project-control/tasks/TASK_TEMPLATE.md`
8. `git restore tera-system/TeraPolicyMap.md`
9. `git restore tera-system/TeraArchitectureMap.md`
10. `git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md`
11. `git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`
12. `git restore tera-system/TeraSystemMaintenanceChecklist.md`
13. `Remove-Item -LiteralPath ".opencode/agents/tera-software-designer.md"`
14. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

---

### الإدخال الثالث عشر — SCP-2026-07-02-010

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-010
مصدر الطلب: User Request (Majed) — Phase 6 Review Model Refinement
نوع التغيير: Policy Update / Anti-Bloat / Agent Improvement / Runtime Sync

الملفات المعدلة:

1. tera-system/TeraSubAgents.md
   - توحيد كل تحذيرات Trust Metadata في §3.5 كمصدر حقيقة وحيد (Single Source of Truth)
   - إضافة قواعد Intervention Logging و Scoped Runtime Override للملخص الأمني

2. tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
   - استبدال قسم "Critical rules for Trust Metadata" المكرر بإشارة إلى TeraSubAgents.md §3.5
   - إضافة قاعدة **Trust Decay / Re-evaluation**: إعادة تقييم مستوى Trust عند 2 `Needs Fix` في 5 مهام أو بعد 15 مهمة دون مراجعة
   - ربط مستوى `Trusted` كشرط أساسي لأهلية `Fast Path`

3. tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
   - تحديث قالب `SUB_AGENT_STATUS.md` (§38.1): استبدال عمود `Last Intervention` بعمود `Last Event` شامل للتدخلات و Override
   - إضافة ملاحظة محفزات إعادة تقييم الثقة في قواعد القالب
   - تحديث قالب سجل التدخل (§38.2): توضيح استخدام `Last Event` في SUB_AGENT_STATUS.md

4. tera-system/AGENT_PERMISSION_MODEL.md
   - إضافة §5.4 "صلاحية العرض السريع (Fast Path)": اشتراط مستوى `Trusted` لـ Fast Path، منع `Restricted/Suspended` تلقائياً، إعادة تقييم كل 15 مهمة

5. tera-system/TeraPolicyMap.md
   - تحديث صف Sub-agent trust metadata: تغيير مصدر الحقيقة إلى `TeraSubAgents.md §3.5` وإضافة ملاحظة محفزات إعادة التقييم

6. حذف 4 ملفات .backup مهملة (تنظيف Anti-Bloat فوري):
   - tera-system/TeraAgent.md.backup
   - tera-system/TeraClientEngagement.md.backup
   - .opencode/agents/tera.md.backup
   - .opencode/agents/tera-client-engagement.md.backup

الملخص:
تنقيح نموذج المراجعة والثقة بعد استكمال مراحل 1-5 (Trust Metadata, Intervention Logging, Fast Path, Scoped Runtime Override).
تم توحيد التحذيرات الأمنية المتكررة في 3 ملفات إلى مصدر واحد في TeraSubAgents.md، إضافة دورة حياة لمستوى الثقة (Trust Decay Rule)، تبسيط سجل الحالة التشغيلية، وربط الثقة الصريحة بصلاحية Fast Path.
القيد غير القابل للتفاوض بقي ثابتاً:
No acceptance without physical review.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check: PASS، لا توجد أخطاء بناء)
المخاطر:
- منخفضة إلى متوسطة: تغييرات هيكلية في الحوكمة، تم احتواؤها عبر Source of Truth مركزي وقواعد Decay واضحة
ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/TeraSubAgents.md
2. git restore tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
3. git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
4. git restore tera-system/AGENT_PERMISSION_MODEL.md
5. git restore tera-system/TeraPolicyMap.md
6. git checkout tera-system/TeraAgent.md.backup tera-system/TeraClientEngagement.md.backup .opencode/agents/tera.md.backup .opencode/agents/tera-client-engagement.md.backup (استعادة النسخ الاحتياطية)
7. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```

---

### الإدخال الرابع عشر — SCP-2026-07-02-011

```
تاريخ: 2026-07-02
معرف التغيير: SCP-2026-07-02-011
مصدر الطلب: User Request (Majed) — Upgrade Task Preparation into Task Engineering Review Capability
نوع التغيير: Agent Improvement / Protocol Change / Runtime Sync / Anti-Bloat

الملفات المعدلة:
- tera-system/TeraSubAgents.md
- tera-system/AGENT_ACTIVATION_MATRIX.md
- tera-system/TeraPreExecutionGate.md
- tera-system/TeraAgent.md
- .opencode/agents/tera.md
- project-control/tasks/TASK_TEMPLATE.md
- project-control/SYSTEM_EVOLUTION_LOG.md

الملخص:
تنفيذ Minimal Task Engineering Upgrade دون إنشاء طبقة مصنع مستقلة أو عميل جديد.

ما تم اعتماده:
- ترقية `ExecutionPreparationAgent` ليشمل `Task Engineering Review` قبل `Pre-Execution Gate` عند الحاجة.
- إضافة قسم `Task Engineering Review` داخل `TASK_TEMPLATE.md`.
- تثبيت قاعدة أن المهام `Medium / High / Critical` لا تصل إلى `Pre-Execution Gate: PASS`
  إلا بعد اكتمال `Task Engineering Review` وصدور قرار `APPROVED_FOR_GATE`.
- تحديث مصفوفة التفعيل لتفعيل `ExecutionPreparationAgent` حسب الخطورة والتعقيد،
  مع إبقاء `Fast Path` للمهام `Low-risk` فقط.
- توضيح الفرق المعماري والتشغيلي بين:
  `Task Engineering Review` / `Pre-Execution Gate` / `Sub-Agent Execution` / `Post-Execution Review`.

ما لم يتم فعله عمداً (Anti-Bloat):
- لم يُنشأ عميل جديد باسم `TaskEngineeringFactoryAgent`.
- لم يُنشأ مجلد نظامي جديد مثل `tera-system/task-engineering/`.
- لم تُعد بناء دورة حياة المهمة بالكامل.

ملاحظة معمارية:
- لم يُنشأ مجلد `task-engineering-reviews/` فعليًا لأن لا توجد مساحة تطبيق نشطة حاليًا تحت
  `clients/CLIENT-*/applications/APP-*/`.
- بقي المسار موثقًا كوجهة رسمية مستقبلية داخل ملفات المهمة والبوابات فقط.

الموافقة: Majed — Approved
التحقق من الصحة: Validation Passed (git diff --check على الملفات المستهدفة: PASS)
المخاطر:
- منخفضة إلى متوسطة — تمت إضافة طبقة صقل خفيفة قبل البوابة دون تخفيف سلطة `Pre-Execution Gate`.
- يوجد خطر تداخل مفاهيمي بين `Task Engineering Review` و`Pre-Execution Gate` إذا لم يلتزم Tera بالفرق المثبت؛ تم احتواؤه عبر تعريف صريح في الملفات الأساسية والـ runtime.

ملاحظات الاسترجاع (Rollback):
1. git restore tera-system/TeraSubAgents.md
2. git restore tera-system/AGENT_ACTIVATION_MATRIX.md
3. git restore tera-system/TeraPreExecutionGate.md
4. git restore tera-system/TeraAgent.md
5. git restore .opencode/agents/tera.md
6. git restore project-control/tasks/TASK_TEMPLATE.md
7. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md
```
