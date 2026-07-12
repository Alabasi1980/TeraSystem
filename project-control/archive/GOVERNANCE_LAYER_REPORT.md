# تقرير تنفيذ Sub-Agent Governance & Tooling Readiness Layer

## تاريخ التقرير: 2026-06-30

---

## 1. الملفات المضافة (3 ملفات جديدة)

| الملف | المسار | الحجم | الوظيفة |
|---|---|---|---|
| **AGENT_ACTIVATION_MATRIX.md** | `tera-system/AGENT_ACTIVATION_MATRIX.md` | جديد | مصفوفة تفعيل العملاء: Triggers، المراحل، متى لا يُفعّل، الحد الأدنى من المدخلات، أمثلة لكل نوع مشروع |
| **AGENT_PERMISSION_MODEL.md** | `tera-system/AGENT_PERMISSION_MODEL.md` | جديد | 7 مستويات صلاحية (READ_ONLY → DEPLOY_WITH_APPROVAL)، الصلاحية الافتراضية لكل عميل، قواعد الرفع والخفض |
| **TOOLING_AND_MCP_POLICY.md** | `tera-system/TOOLING_AND_MCP_POLICY.md` | جديد | سياسة الأدوات: Agent vs Tool vs MCP، 4 MCPs مسموحة الآن، شروط الاستخدام، MCPs المؤجلة، قواعد الأمان |

---

## 2. الملفات المعدلة (4 ملفات)

| الملف | التعديل |
|---|---|
| **AGENT_GENERATION_TEMPLATE.md** | إضافة: Activation Trigger، Phase Usage، Default Permission Level، توسيع Allowed Tools ليشمل MCPs، إضافة Tool Restrictions و Escalation Rules، تحديث Output/Handback Format |
| **TeraSubAgents.md** | إضافة 3 ملفات حوكمة إلى جدول العلاقات (Section 2). إضافة 3.5 حوكمة التفعيل والصلاحيات والأدوات. إضافة 3.6 قواعد التواصل والتفاعل |
| **TeraAgent.md** | إضافة 3 ملفات حوكمة إلى جدول الملفات المرجعية (Section 2). إضافة Section 37: Sub-Agent Governance & Tooling Readiness Layer مع 7 قواعد إلزامية |
| **.opencode/agents/tera.md** | إضافة 3 ملفات حوكمة إلى قائمة system reference files. إضافة 9 قواعد تحميل جديدة للـ Runtime Loading Rules (Section 3) |

---

## 3. كيف يتم تحديد تفعيل العملاء الآن؟

### الخطوات:

```
1. Tera يحدد المرحلة الحالية (Phase X).
2. Tera يحدد نوع المشروع (صغير / متوسط / ERP / SaaS).
3. Tera يراجع AGENT_ACTIVATION_MATRIX.md:
   - هل يوجد Trigger واضح لتفعيل العميل؟
   - هل تم تجاوز الحد الأدنى من المدخلات؟
   - هل المرحلة الحالية مناسبة؟
4. إذا توفر Trigger → Tera يقرر التفعيل.
5. إذا لم يتوفر Trigger → لا يُفعّل العميل.
6. يسجل Tera قرار التفعيل في PROJECT_ACTIVITY_LOG.md.
```

### القاعدة الذهبية:
> لا يتم تفعيل العميل لأنه موجود، بل لأنه مطلوب بسبب Trigger واضح.

### أنواع Triggers:
- `PHASE_GATE` — بداية مرحلة
- `DOCUMENT_READY` — وجود ملف معين
- `DECISION_MADE` — قرار من Tera
- `COMPLEXITY_SIGNAL` — مؤشر تعقيد
- `RISK_SIGNAL` — مؤشر خطر
- `USER_REQUEST` — طلب المستخدم
- `EXTERNAL_FACTOR` — عامل خارجي
- `REVIEW_NEEDED` — حاجة مراجعة
- `PHASE_7_GATE` — بداية التسليم

### أمثلة حسب نوع المشروع:

| المشروع | العملاء المفعّلون نموذجيًا |
|---|---|
| صغير (CRUD) | RequirementsScopeAgent (اختياري) + EngineeringAgent |
| متوسط | Requirements + Workflow + UI/UX + Data + Architecture + Engineering + QA + Security |
| ERP | معظم العملاء الأساسيين والمشروطون + Domain Agents |
| SaaS | UI/UX + Security + DevOps + Performance + Engineering + QA |

---

## 4. كيف يتم ضبط صلاحيات العملاء الآن؟

### مستويات الصلاحية (7 مستويات):

| المستوى | الوصف | مثال عميل |
|---|---|---|
| `READ_ONLY` | قراءة فقط | DomainResearchAgent, ComplianceAgent |
| `PLAN_ONLY` | تحليل وخطط فقط | (حالات خاصة) |
| `WRITE_DOCS` | كتابة توثيق وتحضير | UIVisualDesignerAgent, DataDesignAgent |
| `WRITE_CONTROL` | كتابة سجلات تحكم | ProjectControlAgent |
| `RUN_TESTS` | تشغيل اختبارات | QAAndAcceptanceAgent, Playwright MCP |
| `WRITE_CODE` | كتابة كود | EngineeringAgent |
| `DEPLOY_WITH_APPROVAL` | نشر بموافقة | DevOpsDeploymentAgent |

### آلية العمل:
1. لكل عميل **صلاحية افتراضية** محددة في `AGENT_PERMISSION_MODEL.md`.
2. Tera يقرر قبل كل تفويض: هل الصلاحية الافتراضية كافية؟
3. إذا احتاج العميل صلاحية أعلى → Tera يقرر الرفع للـ TASK-ID الحالي فقط.
4. إذا كان الخطر عاليًا → Tera يخفض الصلاحية للمهمة الحالية.
5. أي تغيير صلاحية يُسجل في `DECISIONS_LOG.md`.
6. **القاعدة:** عند الشك، اختر الصلاحية الأقل.

---

## 5. كيف يتم استخدام MCPs الحالية؟

### MCPs المسموحة الآن (4 MCPs):

| MCP | الصلاحية | لمن؟ | الاستخدام |
|---|---|---|---|
| **Playwright/Browser** | `RUN_TESTS` | QAAndAcceptanceAgent | UI Testing، Smoke Tests، E2E، Phase 6 Review، Phase 7 QA |
| **API Testing** | `RUN_TESTS` | QAAndAcceptanceAgent, IntegrationAgent | اختبار APIs، تحقق من التكاملات |
| **Git/GitHub** | `READ_ONLY` (كتابة بموافقة) | EngineeringAgent, QAAndAcceptanceAgent, ProjectControlAgent | قراءة كود، مراجعة تغييرات، إنشاء PRs |
| **Database Read-Only** | `READ_ONLY` | DataDesignAgent, SolutionArchitectureAgent, QAAndAcceptanceAgent | تحليل البيانات، التحقق من المخطط |

### شروط استخدام MCPs:
1. **Trigger واضح** — استخدام MCP يكون بناءً على حاجة في `TASK-ID` فقط.
2. **لا أسرار Production** — يمنع استخدام MCP مع بيانات Production دون عزل.
3. **تسجيل النتائج** — نتيجة أي MCP تُسجل في ملفات رسمية.
4. **لا تعديل مباشر** — MCP Write يحتاج موافقة صريحة.
5. **DB Read-Only فقط** — أي MCP Database يقرأ فقط.
6. **بيئة اختبارية** — الأدوات تستخدم في Development/Staging أولاً.

### آلية التفعيل:
```
Tera → يحدد الحاجة في TASK-ID → يحدد MCP المطلوب → يحدد الصلاحية → 
يسجل القرار → العميل يستخدم MCP → يسجل النتائج → Tera يراجع
```

---

## 6. كيف سيتم التعامل مع MCPs مستقبلًا؟

### خريطة الطريق:

| متى | MCP | السبب |
|---|---|---|
| **حاليًا** | 4 MCPs (Playwright, API, Git, DB) | محدودة ومدروسة، تلبي الاحتياج الحالي |
| **بعد مشروع متوسط** | Docker/DevOps MCP | فقط عندما نحتاج نشرًا فعليًا |
| **بعد مشروع ERP** | Data Migration MCP | فقط عند وجود ترحيل بيانات |
| **عند اعتماد Figma** | Figma MCP (قراءة فقط) | فقط بعد أول مشروع Figma متكرر |
| **عند وجود Production** | Monitoring MCP | فقط بعد تشغيل التطبيق فعليًا |

### قواعد إضافة MCP جديد:
1. لا يُضاف MCP جديد **بدون حاجة فعلية من مشروع واقعي**.
2. كل MCP جديد يمر عبر:
   ```
   تحليل الحاجة → قرار Tera → تحديث TOOLING_AND_MCP_POLICY.md → تفعيل
   ```
3. MCP Write يتطلب ضوابط أمنية إضافية قبل الإضافة.

---

## 7. كيف يمنع هذا التعديل تضخم العملاء والأدوات؟

### 7.1 منع تضخم العملاء

| الآلية | كيف تمنع التضخم |
|---|---|
| **Activation Triggers** | لا يُفعّل عميل دون سبب واضح ومحدد |
| **مصفوفة المشاريع** | المشروع الصغير يستخدم 1-2 عميل فقط |
| **لا تفعيل مستقبلي** | لا يُفعّل عميل لسيناريوهات قد تحدث مستقبلًا |
| **قاعدة "عند الشك لا تُفعّل"** | عدم التفعيل هو الأصل |
| **التسجيل الإلزامي** | كل تفعيل يُسجل في `PROJECT_ACTIVITY_LOG.md` مع السبب |
| **عدم إنشاء عملاء جدد دائمين** | لا يُضاف عميل دائم جديد في هذه المهمة |
| **توليد موديولات ERP حسب الحاجة** | Domain Module Agents تُولد عند الحاجة فقط |

### 7.2 منع تضخم الأدوات

| الآلية | كيف تمنع التضخم |
|---|---|
| **الحد الأدنى** | 4 MCPs فقط — وكلها ضرورية ومحدودة |
| **لا أداة بدون حاجة** | أي MCP جديد يحتاج مشروعًا فعليًا يبرره |
| **القراءة هي الأصل** | معظم MCPs Read-Only |
| **الكتابة تحتاج موافقة** | لا كتابة بدون موافقة Tera لكل عملية |
| **التأجيل** | MCPs غير الضرورية مؤجلة وليست ملغاة |
| **التسجيل** | كل استخدام MCP يُسجل |

### 7.3 القواعد العشر الحاكمة (مطبقة في كل الملفات)

1. ✅ No direct sub-agent-to-sub-agent communication
2. ✅ Tera is the only orchestrator
3. ✅ Files are the source of truth
4. ✅ No agent activation without a clear trigger
5. ✅ No agent writes outside allowed targets
6. ✅ No tool/MCP use without policy approval
7. ✅ No write/deploy tools without explicit user approval
8. ✅ Generate project-specific agents only when a general agent is insufficient
9. ✅ No new permanent agents added in this task
10. ✅ MCP usage is minimal, controlled, and trigger-based

---

## 8. خلاصة

### ما تم إنجازه:
- ✅ 3 ملفات حوكمة جديدة (Activation Matrix + Permission Model + MCP Policy)
- ✅ تحديث 4 ملفات رئيسية (Agent Template + SubAgents + TeraAgent + tera.md)
- ✅ 7 مستويات صلاحية محددة لكل عميل
- ✅ 4 MCPs مسموحة الآن مع قواعد استخدام واضحة
- ✅ 9 أنواع Triggers لتفعيل العملاء
- ✅ مصفوفة لكل نوع مشروع (صغير/متوسط/ERP/SaaS)
- ✅ 10 قواعد حاكمة مطبقة في كل ملف
- ✅ خريطة طريق MCPs المستقبلية

### ما لم يتغير:
- ❌ لا عملاء جدد دائمين
- ❌ لا MCPs غير ضرورية الآن
- ❌ لا تغيير في بنية TeraSubAgents.md الأصلية
- ❌ لا حذف لأي عميل حالي
- ❌ لا تغيير في workflow المشاريع

### التوصية النهائية:
المنظومة الآن جاهزة لمشاريع صغيرة ومتوسطة مع حوكمة واضحة. الخطوة التالية هي اختبار هذه الحوكمة على مشروع حقيقي (متوسط أو ERP) لضبط أي ثغرات عملية قبل توسيع MCPs أو العملاء.
