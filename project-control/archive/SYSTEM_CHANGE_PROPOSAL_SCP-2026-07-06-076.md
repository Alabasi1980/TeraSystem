# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-06-076 — توحيد تعريف مجالات Discovery الـ 13 في مصدر واحد

---

### Request Type
System Bug / Policy Conflict Resolution

### Problem
ثلاثة ملفات تعرف مجالات Discovery الـ 13 بترتيب وأسماء مختلفة:

| المصدر | Domain 1 | Domain 2 | Domain 3 |
|--------|:--------:|:--------:|:--------:|
| **Question Bank** (سطر 23-35) | Business Context & Value | **Integrations & APIs** | **Users & Roles** |
| **Runtime Template §35** (سطر 1487-1499) | Business & Goals | **Users, Roles & Access** | **Process & Workflow** |
| **Runtime Protocols** (سطر 1327-1329) | Administrative | **Functional** | — |

النتيجة: **Domain 2 في Question Bank = Integrations، Domain 2 في Template = Users.** نموذج يظن أنه غطى "Domain 2" من Question Bank (Integrations) ويكتشف لاحقاً أن Template يتوقع Users — خطأ تراكمي في DISCOVERY_COVERAGE_SUMMARY.md.

### Evidence
- `tera-system/TeraApplicationQuestionBank.md:21-35` — 13 domain list بترتيب مختلف تماماً
- `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md:1487-1499` — Domain Coverage Matrix بترتيب مختلف
- `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md:1327-1329` — تصنيف مختلف (Administrative/Functional/Technical)
- `tera-system/client-helpers/tera-client-engagement-gates.md:21` — B.1 تشير إلى "جميع Domains الـ 13" دون تعريف موحد

### Affected Files
1. **جديد:** `tera-system/client-helpers/tera-client-engagement-discovery-domains.md` — المصدر الرسمي
2. `tera-system/TeraApplicationQuestionBank.md` — استبدال القائمة بإشارة
3. `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` §35 — تحديث الترقيم
4. `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` — تحديث مرجع الترقيم
5. `tera-system/TeraPolicyMap.md` — إضافة إدخال للمصدر الجديد

### Proposed Change

#### 1. إنشاء المصدر الرسمي

ملف جديد: `tera-system/client-helpers/tera-client-engagement-discovery-domains.md`

المحتوى المخطط (~45 سطراً):

```markdown
---
description: Canonical source for the 13 mandatory TCEA discovery domains — single source of truth for numbering, naming, aliases, coverage, and blocking rules.
---

# Discovery Domains — المجالات الـ 13 الرسمية للاكتشاف

> هذا الملف هو **المصدر الوحيد المعتمد** لتعريف وترقيم مجالات Discovery الـ 13.
> جميع الملفات الأخرى (Question Bank, Templates, Protocols) تشير إلى هذا الملف بدلاً من تعريف المجالات من جديد.

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
| 13 | Acceptance, Commercials & Warranty | Acceptance, Commercials & Warranty | Acceptance criteria, budget, payment plan, warranty | نعم | نعم |

## القواعد

1. **هذا الترقيم هو المعتمد فقط.** لا يُستخدم ترقيم آخر في أي ملف.
2. **جميع Domains الـ 13 إلزامية التغطية** في DISCOVERY_COVERAGE_SUMMARY.md — لكن Minimum Coverage يختلف حسب حجم المشروع.
3. **Blocks Pricing** = هذا المجال إذا كان Missing أو Partial يمنع إنتاج DRAFT_QUOTATION.md (Level 2).
4. **Blocks Handoff** = هذا المجال إذا كان Missing أو Partial يمنع PASS في B.7b.
5. الأسماء المقبولة (Aliases) يمكن استخدامها في الأسئلة والحوارات، لكن الاسم الرسمي يُستخدم في الوثائق الرسمية.
```

#### 2. تحديث Question Bank (السطور 21-42)

**من:**
```
TCEA must still evaluate mandatory discovery coverage across these 13 operating domains:

1. Business Context & Value
2. Integrations & APIs
...
13. Acceptance, Commercials & Warranty

**Rule:** ...
```

**إلى:**
```
التعريف الرسمي والترقيم المعتمد للمجالات موجود في:
tera-system/client-helpers/tera-client-engagement-discovery-domains.md

هذا الملف (Question Bank) يبقى مصدراً للأسئلة فقط — وليس لتعريف المجالات أو ترقيمها.
```

مع تضمين ملاحظة: "الترقيم في هذا الملف قديم — راجع discovery-domains.md للترقيم المعتمد. Questions مرتبطة حالياً بالترقيم القديم ويجري تحديثها."

#### 3. تحديث Runtime Template §35 (السطور 1487-1499)

ضمان أن الترقيم في Domain Coverage Matrix يطابق المصدر الرسمي تماماً. حالياً الترقيم في Template متطابق مع المصدر الرسمي المقترح (لأن Template يملك الترتيب الأكثر منطقية). فقط نضمن تطابق الأسماء:

| # في Template حالياً | الاسم الرسمي الجديد | هل يتطابق؟ |
|:--------------------:|:-------------------:|:----------:|
| 1 Business & Goals | Business & Goals | ✅ |
| 2 Users, Roles & Access | Users, Roles & Access | ✅ |
| 3 Process & Workflow | Process & Workflow | ✅ |
| 4 Data & Content | Data & Content | ✅ |
| 5 Scope & MVP | Scope & MVP | ✅ |
| 6 Screens & UX | Screens & UX | ✅ |
| 7 Notifications Engine | Notifications Engine | ✅ |
| 8 Reports & Dashboards | Reports & Dashboards | ✅ |
| 9 Design & Branding | Design & Branding | ✅ |
| 10 Technical, Hosting & Compliance | Technical, Hosting & Compliance | ✅ |
| 11 Security & Audit | Security & Audit | ✅ |
| 12 Integrations & APIs | Integrations & APIs | ✅ |
| 13 Acceptance, Commercials & Warranty | Acceptance, Commercials & Warranty | ✅ |

إضافة تعليق في أعلى الجدول:
```
> ترقيم المجالات حسب المصدر الرسمي: tera-system/client-helpers/tera-client-engagement-discovery-domains.md
```

#### 4. تحديث Runtime Protocols (السطر 1327-1329)

**من:**
```
- **Domain 1** (Administrative): Q1.1–Q1.8 (client, problem, goal)
- **Domain 2** (Functional): Q2.1–Q2.8 (users, workflows, roles)
- **Domain 4** (Technical): Q4.1–Q4.6 (stack, integrations, environment)
```

**إلى:**
```
- **Domain 1** (Business & Goals): الأسئلة الأساسية (العميل، المشكلة، الهدف)
- **Domain 2** (Users, Roles & Access): المستخدمون، الصلاحيات، الأدوار
- **Domain 4** (Data & Content): البيانات، المحتوى، الهيكل
```

(تصحيح الترقيم ليتطابق مع المصدر الرسمي — Domain 2 = Users, Domain 4 = Data & Content)

#### 5. تحديث TeraPolicyMap.md

إضافة إدخال:
```
| TCEA discovery domains | `tera-system/client-helpers/tera-client-engagement-discovery-domains.md` | — | Canonical source for the 13 domains numbering, naming, aliases, and blocking rules. |
```

### Why This Is Necessary
- **يزيل الخطر التراكمي**: Domain 2 في Question Bank = Integrations لكن Domain 2 في Template = Users — هذا خطأ مباشر يصل إلى DISCOVERY_COVERAGE_SUMMARY.md
- **مصدر واحد:** كل الملفات تشير إلى نفس الترقيم والتسمية
- **Aliases مقبولة:** يسمح بالمرونة اللغوية (Business & Goals / Business Context & Value) مع الحفاظ على الاسم الرسمي
- **يمنع التكرار:** Question Bank لم يعد يحتاج تعريف المجالات — فقط يشير إلى المصدر الرسمي

### Rejected Alternatives
1. **تعديل Question Bank فقط ليطابق Template**: لا يحل المشكلة — يبقى Protocol ثالث بترقيم مختلف
2. **تعديل Template فقط ليطابق Question Bank**: يخسر الترتيب المنطقي (Business → Users → Process → Data → Scope → ...) الموجود في Template
3. **دمج القائمة في protocols.md**: يخلط بين البيانات (domain definitions) والمنطق (operational protocols)

### Anti-Bloat Check
| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | 3 أنظمة ترقيم مختلفة تسبب أخطاء تراكمية في DISCOVERY_COVERAGE_SUMMARY.md |
| لماذا لا يكفي تعديل ملف موجود؟ | 3 ملفات تتأثر — إنشاء مصدر واحد أفضل من محاولة مزامنة 3 ملفات يدوياً |
| لماذا لا يكفي عميل موجود؟ | مشكلة تناسق بيانات بين ملفات، ليس في عميل |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — مصدر واحد بدلاً من 3 تعريفات مختلفة |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | ~45 سطراً في ملف جديد، يُقرأ فقط عند Discovery أو عند الشك في ترقيم Domain |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — وجود 3 تعريفات مختلفة يتطلب مصدراً موحداً وحذف التعريفات المكررة. الملف الجديد ~45 سطراً وهو الحد الأدنى |

### Risk
- **متوسط** — تغيير ترقيم في Question Bank و Protocols قد يسبب ارتباكاً مؤقتاً
- **مخاطر التطبيق:** أسئلة Question Bank مرتبطة حالياً بالترقيم القديم (Q1.x, Q2.x...). تحديثها ليس ضمن نطاق هذا SCP — نضيف ملاحظة في Question Bank أن الترقيم قديم وسيُحدّث لاحقاً
- **مخاطر التشغيل:** النماذج الحالية التي اعتادت الترقيم القديم قد تحتاج جلسة واحدة للتكيف

### Rollback Plan
حذف `discovery-domains.md`، استعادة Question Bank (السطور 21-42)، استاحة Runtime Protocols (السطر 1327-1329)، استعادة TeraPolicyMap.md من git.

### Approval Required
- [ ] Majed — Approved
- [ ] Majed — Approved with Conditions
- [ ] Majed — Rejected
