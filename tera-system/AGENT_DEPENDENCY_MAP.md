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
| **tera.md** | — (الأب) | `ui-designer.md`, `engineering-agent.md`, `tera-software-designer.md`, `application-blueprint.md`, `domain-research-agent.md`, `domain-expert-agent.md` | `tera-system/*.md`, `project-preparation/`, `project-control/` |
| **ui-designer.md** | `tera.md`, `tera-system-evolution.md` (للأغراض النظامية) | `design-reviewer.md` (ناقد يراجعه) | `28_UI_UX_GUIDELINES.md`, `tera-system/design-system/*.md` |
| **engineering-agent.md** | `tera.md` | `ui-designer.md` (مصمم يسبقه)، `tera-software-designer.md` (يسبقه للمهام المعقدة) | `TECHNICAL_SPECIFICATION.md`, `28_UI_UX_GUIDELINES.md` |
| **tera-software-designer.md** | `tera.md` | `engineering-agent.md` (ينفذ الـ Spec) | `project-preparation/*.md`, `28_UI_UX_GUIDELINES.md`, `PROJECT_RULES.md` |
| **design-reviewer.md** | — (مستقل — يستدعيه Majed) | `TeraAgent`, `EngineeringAgent` (مراجعة مخرجاتهم) | `28_UI_UX_GUIDELINES.md`, `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` |
| **tera-client-engagement.md** | — (مستقل — يستدعيه Majed) | `domain-research-agent.md`, `domain-expert-agent.md`, `application-blueprint.md` | `tera-system/client-helpers/*.md`, `tera-system/TeraPricingPolicy.md` |
| **application-blueprint.md** | `tera-client-engagement.md`, `tera.md` | `domain-research-agent.md`, `domain-expert-agent.md` | `project-preparation/`, `client-engagement/` |
| **domain-research-agent.md** | `tera-client-engagement.md`, `tera.md`, `application-blueprint.md`, `tera-system-evolution.md` | `domain-expert-agent.md` (يسلم له للتحليل) | — (بحث خارجي) |
| **domain-expert-agent.md** | `tera-client-engagement.md`, `tera.md`, `application-blueprint.md`, `tera-system-evolution.md` | — | `domain-research-agent.md` (Domain Research Report) |
| **tera-system-evolution.md** | — (مستقل — يستدعيه Majed) | `ui-designer.md`, `domain-research-agent.md`, `domain-expert-agent.md` (للأغراض النظامية) | `tera-system/*.md`, `.opencode/agents/*.md` |
| **tera-strategic-advisor.md** | — (مستقل — يستدعيه Majed فقط) | لا يستدعي عملاء ولا يديرهم؛ قد يوصي Majed بالرجوع إلى TeraAgent أو حارس أو غيرهم | ملفات وسياقات القرار فقط، مصادر خارجية عند الحاجة |
| **auditor.md** | — (مستقل — يستدعيه Majed) | يراجع مخرجات `tera.md`, `engineering-agent.md` | `project-control/*.md`, `tera-system/*.md` |
| **monitor.md** | — (مستقل — يستدعيه Majed) | يراجع مخرجات `tera.md`, `engineering-agent.md` | `project-control/*.md`, `tera-system/*.md` |
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
| `tera-system/TeraSubAgents.md` | 1,502 | 🔴 > 1000 — تم تبسيط 6 عملاء (ملفات runtime). المتبقي: 12 عميل بدون runtime يحتفظون بتعريفاتهم الكاملة |
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | 1,186 | 🔴 تقسيم إجباري (> 1000) |
| `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | 1,110 | 🔴 تقسيم إجباري (> 1000) |
| `tera-client-engagement.md` | 564 | 🟢 < 700 — لا حاجة |
| `tera.md` | 491 | 🟢 < 700 — لا حاجة |
| `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` | 469 | 🟢 < 700 — لا حاجة |
| `tera-system/TeraPreExecutionGate.md` | 563 | 🟢 < 700 — لا حاجة |
| `tera-system-evolution.md` | 451 | 🟢 < 700 — لا حاجة |
| `tera-strategic-advisor.md` | 323 | 🟢 < 700 — لا حاجة |
| `domain-expert-agent.md` | 398 | 🟢 < 700 — لا حاجة |
| `application-blueprint.md` | 335 | 🟢 < 700 — لا حاجة |
| باقي ملفات agents | 180–322 | 🟢 < 700 — لا حاجة |

**ملاحظة:** الملفات التي تتجاوز 700 سطر وقريبة من الحد (700–1000) ستتم دراستها عند الحاجة الفعلية للتعديل التالي.
