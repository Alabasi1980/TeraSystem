# AGENT_DEPENDENCY_MAP.md

## الغرض
خريطة العلاقات بين ملفات العملاء في `.opencode/agents/`. تساعد في:
- تتبع أثر أي تعديل على عميل — ما الملفات الأخرى التي قد تتأثر
- كشف المراجع المكسورة بعد التحديثات
- تحديد ترتيب التعديل الآمن

---

## 🔗 خريطة العلاقات

| Agent | يتم استدعاؤه بواسطة | يستدعي/يشير إلى | يقرأ من |
|-------|-------------------|----------------|---------|
| **tera.md** | — (الأب) | `ui-designer.md`, `engineering-agent.md`, `engineering-agent-dotnet.md` (.NET specialist), `tera-software-designer.md`, `application-blueprint.md`, `domain-research-agent.md`, `domain-expert-agent.md`, `production-erp-expert.md`, `auditor.md` | `tera-system/*.md`, `project-preparation/`, `project-control/` |
| **ui-designer.md** | `tera.md`, `tera-system-evolution.md` (للأغراض النظامية) | `design-reviewer.md` (ناقد يراجعه) | `28_UI_UX_GUIDELINES.md`, `tera-system/design-system/*.md` |
| **engineering-agent.md** | `tera.md` | `ui-designer.md` (مصمم يسبقه)، `tera-software-designer.md` (يسبقه للمهام المعقدة)؛ يقرأ `engineering-agent-core.md` كمرجع إلزامي | `tera-system/engineering-helpers/engineering-agent-core.md`, `TECHNICAL_SPECIFICATION.md`, `28_UI_UX_GUIDELINES.md` |
| **engineering-agent-dotnet.md** | `tera.md` | .NET specialist — يقرأ `engineering-agent-core.md` كمرجع إلزامي + profile نشط | `tera-system/engineering-helpers/engineering-agent-core.md`, `tera-system/profiles/[ACTIVE_PROFILE].md`, `TECHNICAL_SPECIFICATION.md` |
| **tera-software-designer.md** | `tera.md` | `engineering-agent.md` أو `engineering-agent-dotnet.md` (ينفذ الـ Spec) | `project-preparation/*.md`, `28_UI_UX_GUIDELINES.md`, `PROJECT_RULES.md` |
| **design-reviewer.md** | — (مستقل — يستدعيه Majed) | `TeraAgent`, `EngineeringAgent` (مراجعة مخرجاتهم) | `28_UI_UX_GUIDELINES.md`, `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` |
| **tera-client-engagement.md** | — (مستقل — يستدعيه Majed) | `domain-research-agent.md`, `domain-expert-agent.md`, `production-erp-expert.md`, `application-blueprint.md` | `tera-system/client-helpers/*.md`, `tera-system/TeraPricingPolicy.md` |
| **application-blueprint.md** | `tera-client-engagement.md`, `tera.md` | `domain-research-agent.md`, `domain-expert-agent.md`, `production-erp-expert.md` | `project-preparation/`, `client-engagement/` |
| **domain-research-agent.md** | `tera-client-engagement.md`, `tera.md`, `application-blueprint.md`, `tera-system-evolution.md` | `domain-expert-agent.md` (يسلم له للتحليل) | — (بحث خارجي) |
| **domain-expert-agent.md** | `tera-client-engagement.md`, `tera.md`, `application-blueprint.md`, `tera-system-evolution.md` | — | `domain-research-agent.md` (Domain Research Report) |
| **production-erp-expert.md** | Majed مباشرة، `tera.md`, `tera-client-engagement.md`, `application-blueprint.md`؛ وEngineering/QA فقط عبر Tera-approved task scope | `domain-research-agent.md` عند نقص المعرفة وبقرار المستدعي | `tera-system/knowledge-base/manufacturing/`, ملفات discovery/blueprint المحددة |
| **tera-system-evolution.md** | — (مستقل — يستدعيه Majed) | `ui-designer.md`, `domain-research-agent.md`, `domain-expert-agent.md` (للأغراض النظامية) | `tera-system/*.md`, `.opencode/agents/*.md` |
| **tera-strategic-advisor.md** | — (مستقل — يستدعيه Majed فقط) | لا يستدعي عملاء ولا يديرهم؛ قد يوصي Majed بالرجوع إلى TeraAgent أو حارس أو غيرهم | ملفات وسياقات القرار فقط، مصادر خارجية عند الحاجة |
| **auditor.md** | `tera.md`؛ و`monitor.md` فقط عند طلب Majed | يراجع مخرجات `tera.md`, `engineering-agent.md`, `engineering-agent-dotnet.md`؛ يحيل findings إلى `SecurityAgent`, `DesignReviewer`, `QAAndAcceptanceAgent`, أو `ProjectControlAgent` عبر الوكيل المستدعي | `project-control/*.md`, `project-control/audit-reports/`, `tera-system/engineering-governance/*.md`, الملفات المعدلة |
| **monitor.md** | — (مستقل — يستدعيه Majed) | يراجع مخرجات `tera.md`, `engineering-agent.md`, `engineering-agent-dotnet.md`; قد يستدعي `auditor.md` فقط عندما يطلب Majed تحدي/تحقق جودة مستقل | `project-control/*.md`, `tera-system/*.md` |
| **QAAndAcceptanceAgent** | `tera.md` (يُفعّله Tera) | لا يستدعي عملاء آخرين | ملفات التحضير، ملفات المهمة المنفذة، logs، مخرجات CLI |
| **qa-agent.md** | `tera.md` (يُفعّله Tera) | لا يستدعي عملاء آخرين | ملفات التحضير، ملفات المهمة، logs، مخرجات CLI، `project-control/test-reports/` |

---

## 📏 ترتيب التعديل الآمن

عند تعديل أي Agent، اتبع هذا التسلسل لتجنب كسر المراجع:

```
1. غيّر الملف الأساسي
2. راجع AGENT_DEPENDENCY_MAP.md — من يشير لهذا الملف؟
3. افتح كل ملف يشير إليه وتحقق من أن المراجع ما زالت صحيحة
4. إذا وجدت مرجعاً مكسوراً → صححه
5. سجل التغيير
```

### مثال: تعديل `ui-designer.md`
```
ui-designer.md
  ↑ يشير إليه: tera.md (Section 8 — يستدعيه)
  ↑ يشير إليه: design-reviewer.md (Section 11 — يقارن نفسه به)
  ↑ يشير إليه: engineering-agent.md (Section 3 — يتكامل معه)
  ↑ يشير إليه: tera-system-evolution.md (Section 7.1 — يستدعيه لأغراض نظامية)

يجب فتح: tera.md + design-reviewer.md + engineering-agent.md + tera-system-evolution.md
وتأكيد أن الإشارات ما زالت صحيحة بعد التعديل.
```

---

## ⚠️ تنبيهات حجم الملف

الجدول أدناه يستخدم القاعدة المحدّثة: < 700 = ✅, 700–1000 = 🟡 دراسة, > 1000 = 🔴 تقسيم إجباري.

| الملف | الحجم (سطور) | الحالة |
|-------|-------------|--------|
| `tera-system/TeraSubAgents.md` | 1,672 | 🔴 > 1000 — ملف Registry طويل تاريخياً؛ أضيف ProductionERPExpert دون تقسيم استباقي لأن تقسيم TeraSubAgents يتم عند الحاجة الفعلية للتوليد لا الآن |
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | 1,186 | 🔴 تقسيم إجباري (> 1000) |
| `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | 1,110 | 🔴 تقسيم إجباري (> 1000) |
| `tera.md` | 867 | 🟡 700–1000 — دراسة فصل منطقي لاحقاً؛ لا تقسيم الآن لأن التعديل Runtime summary محدود |
| `tera-system/TeraPreExecutionGate.md` | 800 | 🟡 700–1000 — دراسة لاحقة عند تعديل كبير؛ لا تقسيم ضمن SCP-098 |
| `tera-client-engagement.md` | 749 | 🟡 700–1000 — دراسة فصل منطقي لاحقاً؛ لا تقسيم الآن لأن التعديل محدود لاستدعاء ProductionERPExpert |
| `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` | 469 | 🟢 < 700 — لا حاجة |
| `auditor.md` | 465 | 🟢 < 700 — لا حاجة |
| `engineering-agent-dotnet.md` | 209 | 🟢 < 700 — لا حاجة |
| `engineering-agent.md` | 56 | 🟢 < 700 — fallback compact; no duplicated core rules |
| `tera-system/engineering-helpers/engineering-agent-core.md` | 127 | 🟢 < 700 — no language-specific content |
| `tera-system-evolution.md` | 451 | 🟢 < 700 — لا حاجة |
| `tera-strategic-advisor.md` | 323 | 🟢 < 700 — لا حاجة |
| `monitor.md` | 276 | 🟢 < 700 — لا حاجة |
| `domain-expert-agent.md` | 398 | 🟢 < 700 — لا حاجة |
| `production-erp-expert.md` | 506 | 🟢 < 700 — لا حاجة |
| `application-blueprint.md` | 458 | 🟢 < 700 — لا حاجة |
| باقي ملفات agents | 180–322 | 🟢 < 700 — لا حاجة |

**ملاحظة:** الملفات التي تتجاوز 700 سطر وقريبة من الحد (700–1000) ستتم دراستها عند الحاجة الفعلية للتعديل التالي.
