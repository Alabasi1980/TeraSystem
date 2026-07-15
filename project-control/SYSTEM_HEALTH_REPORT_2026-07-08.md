# SYSTEM_HEALTH_REPORT

**Scan Date:** 2026-07-08  
**Scan Scope:** Full — `tera-system/`, `.opencode/agents/`, `project-control/`, `tera-workshop/`, `clients/`, `project-preparation/`  
**Performed by:** TeraSystemEvolutionAgent (حارس)

---

## Summary

| البند | العدد |
|-------|-------|
| إجمالي الملفات المفحوصة | ~70 ملفاً |
| مشاكل مكتشفة | 15 |
| مشاكل حرجة (Critical) | 2 |
| مشاكل عالية (High) | 3 |
| مشاكل متوسطة (Medium) | 5 |
| مشاكل منخفضة (Low) | 5 |
| توصيات | 8 |

---

## Detailed Findings

---

### 🔴 CRITICAL

#### C-1: مجلد `project-preparation/` غير موجود على مستوى المنظومة

| الملف | المشكلة |
|-------|---------|
| `tera-system/TeraArchitectureMap.md` §3 | يُعرِّف المجلد كطبقة أساسية: "Internal Tera preparation outputs" |
| `tera-system/TeraPolicyMap.md` §3 | يحدد `project-preparation/28_UI_UX_GUIDELINES.md` كمصدر executable لقواعد التصميم |
| `.opencode/agents/tera.md` | يشير إلى `project-preparation/` في مسارات متعددة |

**الواقع:** المجلد **غير موجود** على مستوى النظام. ملف `28_UI_UX_GUIDELINES.md` يوجد فقط داخل `dashboard-premium-prototype/project-preparation/` وهو خاص بمشروع البروتوتايب، وليس ملفاً نظامياً.

**الأثر:** أي Agent يحاول قراءة `project-preparation/28_UI_UX_GUIDELINES.md` سيفشل. الخريطة المعمارية لا تعكس الواقع.

**التوصية:** إنشاء مجلد `project-preparation/` على مستوى النظام ونقل `28_UI_UX_GUIDELINES.md` إليه (أو إنشاء نسخة نظامية للمشاريع الجديدة).

---

#### C-2: مسارات القوالب في `TeraPolicyMap.md` كلها خاطئة

| الملف | المشكلة |
|-------|---------|
| `tera-system/TeraPolicyMap.md` (الأسطر 58–75) | 18 قالباً مسجل بامتداد `.html` في `tera-workshop/` مباشرة |

**الواقع:** القوالب موجودة فعلياً كملفات `.md` في مجلدات فرعية تحت `tera-workshop/client-templates/`:

| في PolicyMap (خاطئ) | الواقع الفعلي |
|---------------------|--------------|
| `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html` | `tera-workshop/client-templates/commercial/APPLICATION_PROPOSAL_TEMPLATE.md` |
| `tera-workshop/SCOPE_OF_WORK_TEMPLATE.html` | `tera-workshop/client-templates/commercial/SCOPE_OF_WORK_TEMPLATE.md` |
| `tera-workshop/TECHNICAL_PROPOSAL_TEMPLATE.html` | `tera-workshop/client-templates/commercial/TECHNICAL_PROPOSAL_TEMPLATE.md` |
| `tera-workshop/QUOTATION_TEMPLATE.html` | `tera-workshop/client-templates/commercial/QUOTATION_TEMPLATE.md` |
| `tera-workshop/SOFTWARE_SERVICES_AGREEMENT_TEMPLATE.html` | `tera-workshop/client-templates/contractual/SOFTWARE_SERVICES_AGREEMENT_TEMPLATE.md` |
| `tera-workshop/CHANGE_REQUEST_FORM.html` | `tera-workshop/client-templates/contractual/CHANGE_REQUEST_FORM.md` |
| ... (وكل الـ 18) | ... (موزعة على 5 مجلدات فرعية) |

**الأثر:** أي Agent يقرأ `TeraPolicyMap.md` كمصدر للحقيقة سيحصل على مسارات خاطئة.

**التوصية:** تحديث جميع مسارات الـ 18 قالباً في `TeraPolicyMap.md` لتعكس المسار الصحيح والامتداد الصحيح.

---

### 🟠 HIGH

#### H-1: `clients/README.md` غير موجود

| الملف | المشكلة |
|-------|---------|
| `tera-system/TeraPolicyMap.md` السطر 80 | "Client workspace guide: `clients/README.md`" |

**الواقع:** الملف غير موجود.

**التوصية:** إنشاء `clients/README.md` يشرح هيكل مجلدات العملاء (حتى لو كان ملفاً قصيراً).

---

#### H-2: ترميز أسماء المجلدات العربية تالف في `clients/`

**الموقع:** `clients/`  
**المشكلة:** مجلدان بأسماء عربية مشوشة (وا的地下  �����-  ���-���꩟�-�饧-  ) نتيجة ترميز غير صحيح.

**الأثر:** صعوبة التعامل مع هذين العميلين عبر CLI، واحتمال فقدان البيانات.

**التوصية:** إعادة تسمية المجلدين إلى تسمية لاتينية منظمة (مثلاً `CLIENT-ALFARES` و `CLIENT-OMRAN`).

---

#### H-3: `TeraSubAgents.md` حجمه 1375 سطراً — تضخم خطير

| الملف | الحجم | الحالة |
|-------|-------|--------|
| `tera-system/TeraSubAgents.md` | 1,375 سطراً | ⚠️ يتجاوز الحد بثلاثة أضعاف |

**المشكلة:** الملف يحتوي تعريفات كاملة لـ 10 وكلاء فرعيين غير موجودين فعلياً (لم يتم إنشاؤهم كملفات). هذه التعريفات تمثل قوالب/مراجع للتوليد المستقبلي فقط، لكنها تشغل حجماً هائلاً.

**الوكلاء الموجودون كتعريفات فقط (لا يوجد لهم ملفات فعلية):**
- 6.1 SecurityAgent
- 6.2 IntegrationAgent
- 6.3 DevOpsDeploymentAgent
- 6.4 PerformanceAgent
- 6.5 ComplianceAgent
- 6.6 ReportingAnalyticsAgent
- 6.7 MaintenanceMigrationAgent
- 6.8 ProjectControlAgent
- 6.10 QualityReviewCoordinatorAgent
- 6.11 PlanComplianceReviewAgent

**التوصية:** دراسة نقل التعريفات الكاملة لهؤلاء الوكلاء إلى ملف منفصل (مثلاً `TeraSubAgentsDefinitions.md` أو `TeraAgentTemplates.md`) والاحتفاظ في `TeraSubAgents.md` بقائمة مختصرة مع إشارات المرجع.

---

### 🟡 MEDIUM

#### M-1: ملفات Runtime كبيرة جداً

| الملف | السطور | التوصية |
|-------|--------|---------|
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | 1,186 | تقسيم حسب الوظيفة (Phase templates, Proposal templates, etc.) |
| `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | 1,110 | تقسيم حسب نوع البروتوكول |
| `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` | 469 | فوق حد 400 — تقسيم خفيف |
| `tera-system/TeraPreExecutionGate.md` | 563 | فوق حد 400 — دراسة التقسيم |

**الأثر:** ملفات ضخمة تستهلك توكنز كثيرة عند قراءتها وتصعب الصيانة.

**التوصية:** تقسيم الملفات التي تتجاوز 500 سطر إلى 2-3 أجزاء منطقية.

---

#### M-2: ملفات Agent تجاوزت حد الـ 400 سطر

| الملف | السطور | الحالة |
|-------|--------|--------|
| `tera-system-evolution.md` | 451 | ⚠️ أنا نفسي تجاوزت الحد! (الأقلية اليوم) |
| `domain-expert-agent.md` | 398 | 🟡 على وشك تجاوز الحد |

**التوصية:** `tera-system-evolution.md` ← تقسيم الأقسام الثابتة إلى ملفات مساعدة في `tera-system/agent-helpers/`.

---

#### M-3: 18 ملف SCP متراكمة في `project-control/`

**الموقع:** `project-control/SYSTEM_CHANGE_PROPOSAL_SCP-*.md`

**المشكلة:** ملفات مقترحات التغيير تتراكم في جذر `project-control/` بدون أرشفة. مع الوقت سيصبح المجلد فوضوياً.

**التوصية:** إنشاء مجلد `project-control/archive/` ونقل الملفات القديمة إليه (بعضها موجود بالفعل في `archive/` والبعض الآخر لا يزال في الجذر).

---

#### M-4: `AGENT_DEPENDENCY_MAP.md` لا يزال غير مشار إليه في `TeraPolicyMap.md`

**المشكلة:** تم إنشاء الملف في SCP-2026-07-07-083 لكن `TeraPolicyMap.md` لا يذكره.

**التوصية:** إضافة إدخال لـ `AGENT_DEPENDENCY_MAP.md` في `TeraPolicyMap.md`.

---

#### M-5: `PROJECT_ACTIVITY_LOG.md` في جذر `project-control/` وكذلك داخل `dashboard-premium-prototype/project-control/`

**المشكلة:** وجود نسختين من سجل النشاط — واحدة نظامية وأخرى خاصة بالبروتوتايب. لا يوجد تمييز واضح أيهما الرسمي.

**التوصية:** توثيق في `TeraPolicyMap.md` أن `project-control/PROJECT_ACTIVITY_LOG.md` هو السجل الرسمي للنظام.

---

### 🟢 LOW

#### L-1: `generated-agents/opencode/GENERATED_AGENTS_MANIFEST.md` فارغ

المشكلة: الملف موجود لكنه لا يحتوي أي إدخالات. التحذير بسيط — سيمتلئ عند الحاجة.

#### L-2: ترقيم الأسئلة في Question Bank متقادم

كما هو موثق في `TERA_RUNTIME_PROTOCOLS.md` السطر 1364: الأسئلة لا تزال تستخدم ترقيماً قديماً لا يتطابق مع الترقيم المعتمد للمجالات في `discovery-domains.md`.

#### L-3: `domain-expert-agent.md` (398 سطراً) يقترب من حد التقسيم

تحذير استباقي — عند الإضافة التالية يجب دراسة التقسيم.

#### L-4: `TeraPreparationDocumentationGovernance.md` غير مشار إليه في TeraPolicyMap.md

الملف موجود في `tera-system/` لكنه غير مدرج في `TeraPolicyMap.md`.

#### L-5: ملف `playwright-mcp/` وملفات `.png` متعددة غير متعقبة في git

الملفات (`*.png`, `.playwright-mcp/`) تظهر في `git status` كملفات غير متعقبة. يجب إضافتها إلى `.gitignore`.

---

## File Size Summary

```
   السطور  الملف
   ─────── ────────────────────────────────────────
    1,375  tera-system/TeraSubAgents.md
    1,186  tera-system/runtime/TERA_RUNTIME_TEMPLATES.md
    1,110  tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md
      563  tera-system/TeraPreExecutionGate.md
      564  .opencode/agents/tera-client-engagement.md
      491  .opencode/agents/tera.md
      469  tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md
      451  .opencode/agents/tera-system-evolution.md  ← فوق 400
      398  .opencode/agents/domain-expert-agent.md   ← يقترب من 400
      335  .opencode/agents/application-blueprint.md
      294  .opencode/agents/design-reviewer.md
      280  .opencode/agents/auditor.md
      275  .opencode/agents/domain-research-agent.md
      266  .opencode/agents/engineering-agent.md
      256  .opencode/agents/ui-designer.md
      248  tera-system/TeraTokenPolicy.md
      204  .opencode/agents/monitor.md
      197  tera-system/design-system/DESIGN_REVIEW_STANDARDS.md
      189  tera-system/TOOLING_AND_MCP_POLICY.md
      180  .opencode/agents/tera-software-designer.md
       68  tera-system/TERA_AGENT_CONDUCT.md
```

---

## Validation Checks (Anti-Bloat, Policy, Architecture)

| الفحص | النتيجة |
|-------|---------|
| ✅ Anti-Bloat Overview | تضخم موجود يستحق المعالجة (SubAgents 1.3K، Runtime files 1.1K) |
| ✅ Policy Map Check | **فشل** — مسارات القوالب كلها خاطئة + missing entry لـ AGENT_DEPENDENCY_MAP |
| ✅ Architecture Map Check | **فشل** — project-preparing/ غير موجود |
| ✅ No client-app contamination | ✅ سليم |
| ✅ No stale/deprecated agent references | **تحذير** — references لـ TeraAgent.md و TeraClientEngagement.md موجودة في سجلات قديمة (مقبولة لأنها أرشيفية) |
| ✅ Git status | 15 ملفاً متغيراً غير ملتزمة + ~30 ملفاً غير متعقب (معظمها screenshots و playwright logs) |

---

## Recommended Actions (مرتبة حسب الأولوية)

### أولوية عاجلة (هذا الأسبوع)
1. **C-1:** إنشاء `project-preparation/` + نقل/إنشاء `28_UI_UX_GUIDELINES.md` + تحديث `TeraArchitectureMap.md` إذا لزم
2. **C-2:** تحديث جميع مسارات القوالب في `TeraPolicyMap.md` إلى المسارات الصحيحة

### أولوية عالية (الأيام القادمة)
3. **H-1:** إنشاء `clients/README.md`
4. **H-2:** إعادة تسمية مجلدات العملاء (fix encoding)
5. **H-3:** دراسة تقسيم `TeraSubAgents.md` (نقل التعريفات غير الفعلية إلى ملف منفصل)
6. **M-3:** نقل ملفات SCP القديمة إلى `project-control/archive/`

### أولوية متوسطة (الدورة القادمة)
7. **M-1:** تقسيم `TERA_RUNTIME_TEMPLATES.md` و `TERA_RUNTIME_PROTOCOLS.md`
8. **M-2:** تقسيم `tera-system-evolution.md` إلى ملفات مساعدة
9. **M-4:** إضافة `AGENT_DEPENDENCY_MAP.md` إلى `TeraPolicyMap.md`
10. **L-5:** إضافة `.gitignore` مناسب

---

## Approval Required: ✅ Yes

أي تعديل على هذه التوصيات يحتاج موافقتك يا Majed قبل التنفيذ.

---

*التقرير أنتجه حارس — TeraSystemEvolutionAgent*
