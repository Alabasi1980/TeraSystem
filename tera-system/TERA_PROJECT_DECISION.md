# TERA_PROJECT_DECISION.md

# قرار تيرا الافتتاحي للمشروع

## 1. ملخص فهم المشروع

اكتب هنا فهم تيرا المختصر لفكرة التطبيق، المشكلة التي يحلها، والمخرجات المتوقعة.

---

## 2. تصنيف حجم المشروع

| البند | القرار |
|---|---|
| حجم المشروع | صغير / متوسط / كبير / ERP أو نظام موسع |
| سبب التصنيف | ... |
| مستوى التوثيق المطلوب | منخفض / متوسط / عالي |

---

## 3. Intake Readiness

| Item | Status | Source | Notes |
|---|---|---|---|
| Application Idea | Missing / Partial / Complete | `project-inputs/01_APPLICATION_IDEA.md` | ... |
| Technical Context | Missing / Partial / Complete | `project-inputs/02_TECHNICAL_CONTEXT.md` | ... |
| Technology Profile Candidate | Missing / Found / Needs Creation | ... | ... |
| Can Tera proceed to preparation? | Yes / No | ... | ... |

---

## 3.1 Client Approval Readiness

Use this section for external client projects.

| Item | Status | Source | Notes |
|---|---|---|---|
| Client Profile | Missing / Partial / Complete / Not External Client | `clients/CLIENT-*/CLIENT_PROFILE.md` | ... |
| Client Contacts | Missing / Partial / Complete / Not External Client | `clients/CLIENT-*/CONTACTS.md` | ... |
| Approval Authority | Unknown / User-confirmed / Documented / Not External Client | `CONTACTS.md` | ... |
| Client Application Folder | Missing / Identified / Created / Not External Client | `clients/CLIENT-*/applications/APP-*/` | ... |
| Client Approval Package | Missing / Draft / Complete / Approved / Not External Client | `client-approval/` | ... |
| Execution Authorization | Pending / Approved / Blocked / Not External Client | `10_CLIENT_APPROVAL_RECORD.md` | ... |
| Can Tera enter Build Mode? | Yes / No | ... | ... |

External client rule:

```text
No Client Approval Package = No Implementation
No Approved Scope = No Build Mode
```

---

## 4. الملفات التي يجب إنشاؤها

| الملف | سبب إنشائه | الأولوية |
|---|---|---|
| `...` | ... | عالية / متوسطة / منخفضة |

---

## 5. الملفات غير المطلوبة حاليًا

| الملف | سبب عدم إنشائه الآن |
|---|---|
| `...` | ... |

---

## 6. المعلومات الناقصة

| المعلومة الناقصة | أثرها | هل يمكن المتابعة مؤقتًا؟ | الافتراض المؤقت |
|---|---|---|---|
| ... | ... | نعم / لا | ... |

---

## 7. المخاطر الأولية

| الخطر | الأثر | الإجراء المقترح |
|---|---|---|
| ... | ... | ... |

---

## 8. الموديولات المتوقعة

| الموديول | الوصف المختصر | الأولوية |
|---|---|---|
| ... | ... | أساسي / مهم / لاحق |

---

## 9. العملاء الفرعيون المطلوبون مبدئيًا

| العميل الفرعي | سبب الحاجة إليه | المرحلة |
|---|---|---|
| ... | ... | ... |

---

## 10. ترتيب العمل المقترح

1. ...
2. ...
3. ...

---

## 11. قرار السياق والتوكنز

| البند | القرار |
|---|---|
| مستوى حساسية التوكنز | منخفض / متوسط / عالي / حرج |
| نوع السياق الافتراضي | Task Context / Summary Context / Full Context |
| هل يلزم `PROJECT_STATE.md`؟ | نعم / لا |
| هل توجد مهام تحتاج موافقة تكلفة؟ | نعم / لا |
| العملاء المتوقعون لاستهلاك توكنز عالٍ | ... |
| قاعدة قراءة الملفات | ملفات محددة فقط / ملفات كاملة عند الحاجة |
| وضع التشغيل الافتراضي | Plan Mode |
| هل يسمح بـ Build Mode الآن؟ | لا / نعم مع سبب |

### ملاحظات سياسة السياق

- ...
- ...

### Model Tier Plan

| المرحلة / نوع العمل | Recommended Model Tier | Minimum Acceptable Tier | User Approval Needed? | السبب |
|---|---|---|---|---|
| Discovery / Intake | Medium | Light | No | جمع وفهم أولي يمكن تقليل تكلفته عند وضوح المدخلات. |
| Scope / Requirements | Medium | Medium | No | يحتاج اتساقًا وتحليل حدود المشروع. |
| Technical Architecture | Strong | Medium | Yes if high risk | قرارات معمارية قد تؤثر على التنفيذ طويلًا. |
| UI Structure | Medium | Light | No | غالبًا نطاقه محدود ويمكن تقسيمه. |
| Data Model | Strong | Medium | Yes if complex | يؤثر على العلاقات، القيود، والهجرة المستقبلية. |
| Implementation Task | Medium | Medium | Depends | التنفيذ العادي يحتاج توازنًا بين الدقة والتكلفة. |
| Security-Sensitive Task | Strong | Strong | Yes | لا يسمح بمستوى ضعيف في المصادقة، الصلاحيات، أو البيانات الحساسة. |
| Simple Formatting / Documentation cleanup | Light | Light | No | لا يحتاج استدلالًا عميقًا أو سياقًا واسعًا. |
| Broad Review / Consistency Review | Strong | Medium | Yes if broad | يحتاج ربطًا واسعًا بين الملفات والقرارات. |

This plan is a cost-control guide, not a permanent lock. Each TASK-ID must still run its own Model Capability Assessment when risk, complexity, context, or verification difficulty changes.


---

## 12. قرار تيرا النهائي

```text
قرار تيرا:
سيتم إنشاء الملفات التالية:
- ...

لن يتم إنشاء الملفات التالية حاليًا:
- ...

سبب القرار:
- ...

الخطوة التالية:
- ...
```
---

## 13. بروتوكول ما بعد الاعتماد

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
11. في مشاريع العملاء الخارجيين، لا يتم الانتقال إلى التنفيذ البرمجي إلا بعد اكتمال واعتماد حزمة العميل داخل `clients/.../client-approval/` وتسجيل `Execution Authorization` في سجل اعتماد العميل.

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
