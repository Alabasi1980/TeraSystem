---
description: Canonical source for the 13 mandatory TCEA discovery domains — single source of truth for numbering, naming, aliases, coverage, and blocking rules.
---

# Discovery Domains — المجالات الـ 13 الرسمية للاكتشاف

> هذا الملف هو **المصدر الوحيد المعتمد** لتعريف وترقيم مجالات Discovery الـ 13.
> جميع الملفات الأخرى (Question Bank, Templates, Protocols) تشير إلى هذا الملف بدلاً من تعريف المجالات من جديد.
>
> **لا تقرأ هذا الملف افتراضياً.** اقرأه فقط عند بدء Discovery لعميل جديد، أو عند الشك في ترقيم أو اسم Domain معين.

---

## جدول المجالات الرسمي

| # | الاسم الرسمي (Canonical) | الأسماء المقبولة (Aliases) | Minimum Coverage | يحجب التسعير؟ | يحجب الهاندوف؟ |
|:-:|:-------------------------|:--------------------------|:----------------:|:-------------:|:--------------:|
| 1 | Business & Goals | Business Context & Value, Administrative | Client profile, problem, goal, decision maker | لا | نعم |
| 2 | Users, Roles & Access | Users & Roles, Functional | User types, roles hierarchy, permissions model | لا | نعم |
| 3 | Process & Workflow | Workflow & Operations | Core business processes, approval flows, states | نعم | نعم |
| 4 | Data & Content | Data & Content | Entities, relationships, data volume, file types | نعم | نعم |
| 5 | Scope & MVP | Scope & MVP | In/out scope, priorities, phases | نعم | نعم |
| 6 | Screens & UX | Screens & UX | Screen count, UI complexity, responsive needs | نعم | نعم |
| 7 | Notifications Engine | Notifications Engine | Email, SMS, in-app, push, templates | لا | نعم |
| 8 | Reports & Dashboards | Reports & Dashboards | Report types, charts, filters, export | نعم | نعم |
| 9 | Design & Branding | Design & Branding | Brand guidelines, design source (Figma/etc), UI kit | لا | نعم |
| 10 | Technical, Hosting & Compliance | Technical, Hosting & Compliance | Stack, hosting, domain, SSL, compliance needs | نعم | نعم |
| 11 | Security & Audit | Security & Audit | Auth method, data sensitivity, audit trail | نعم | نعم |
| 12 | Integrations & APIs | Integrations & APIs | Third-party APIs, webhooks, data sync | نعم | نعم |
| 13 | Acceptance, Commercials & Warranty | Acceptance, Commercials & Warranty | Acceptance criteria, budget, payment plan, warranty, support terms | نعم | نعم |

---

## القواعد

1. **هذا الترقيم هو المعتمد فقط.** لا يُستخدم ترقيم آخر في أي ملف من ملفات المنظومة.
2. **جميع Domains الـ 13 إلزامية التغطية** في `DISCOVERY_COVERAGE_SUMMARY.md` — لكن عمق التغطية (Minimum Coverage) يختلف حسب حجم المشروع (صغير/متوسط/معقد/غامض).
3. **Blocks Pricing = نعم** ← هذا المجال إذا كان `Missing` أو `Partial` دون حل يمنع إنتاج `DRAFT_QUOTATION.md` (Level 2).
4. **Blocks Handoff = نعم** ← هذا المجال إذا كان `Missing` أو `Partial` دون حل يمنع PASS في B.7b (Final Handoff Package Gate).
5. الأسماء المقبولة (Aliases) يمكن استخدامها في الأسئلة والحوارات مع Majed، لكن **الاسم الرسمي (Canonical)** يُستخدم في جميع الوثائق الرسمية (`DISCOVERY_COVERAGE_SUMMARY.md`, `DRAFT_QUOTATION.md`, `TERA_HANDOFF_PACKAGE.md`).
6. **المجال 13 (Acceptance, Commercials & Warranty)** يتطلب تغطية 3 جوانب داخلية على الأقل: (أ) معايير القبول والاختبارات, (ب) الميزانية وخطة الدفع, (ج) الضمان والصيانة.

---

## علاقة هذا المصل بالملفات الأخرى

| الملف | العلاقة |
|-------|---------|
| `TeraApplicationQuestionBank.md` | يستخدم هذا الترقيم كمرجع للأسئلة — راجع discovery-domains.md للترقيم المعتمد |
| `TERA_RUNTIME_TEMPLATES.md §35` | Domain Coverage Matrix تستخدم هذا الترقيم والترتيب |
| `TERA_RUNTIME_PROTOCOLS.md` (Smart Interview) | تستخدم أسماء المجالات من هذا المصدر |
| `gates.md` (B.1 Discovery Coverage Gate) | تشير إلى "جميع Domains الـ 13" — تعريفها هنا |
