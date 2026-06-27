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

## 3. الملفات التي يجب إنشاؤها

| الملف | سبب إنشائه | الأولوية |
|---|---|---|
| `...` | ... | عالية / متوسطة / منخفضة |

---

## 4. الملفات غير المطلوبة حاليًا

| الملف | سبب عدم إنشائه الآن |
|---|---|
| `...` | ... |

---

## 5. المعلومات الناقصة

| المعلومة الناقصة | أثرها | هل يمكن المتابعة مؤقتًا؟ | الافتراض المؤقت |
|---|---|---|---|
| ... | ... | نعم / لا | ... |

---

## 6. المخاطر الأولية

| الخطر | الأثر | الإجراء المقترح |
|---|---|---|
| ... | ... | ... |

---

## 7. الموديولات المتوقعة

| الموديول | الوصف المختصر | الأولوية |
|---|---|---|
| ... | ... | أساسي / مهم / لاحق |

---

## 8. العملاء الفرعيون المطلوبون مبدئيًا

| العميل الفرعي | سبب الحاجة إليه | المرحلة |
|---|---|---|
| ... | ... | ... |

---

## 9. ترتيب العمل المقترح

1. ...
2. ...
3. ...

---

## 10. قرار تيرا النهائي

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

## 11. بروتوكول ما بعد الاعتماد

عند موافقة صاحب المشروع على القرار، لا يبدأ التنفيذ البرمجي مباشرة.

الخطوة الأولى بعد الاعتماد:

1. ينشئ Tera مهمة رسمية بصيغة التفويض المعتمدة في `TeraSubAgents.md`.
2. يتم استدعاء `RequirementsScopeAgent` أولًا لإنتاج أو مراجعة ملفات النطاق الأساسية فقط:
   - `01_PROJECT_BRIEF.md`
   - `02_SCOPE_AND_BOUNDARIES.md`
   - `04_USERS_ROLES_PERMISSIONS.md` عند الحاجة.
3. يحدد Tera في التفويض:
   - `Task ID`
   - الهدف.
   - الملفات المرجعية.
   - الملفات المسموح بتعديلها.
   - القيود.
   - معايير القبول.
4. لا يكتب العميل الفرعي في أي ملف غير مصرح به.
5. بعد تسليم النتيجة، يراجع Tera المخرج ويقرر: قبول، رفض، إعادة عمل، أو طلب قرار من صاحب المشروع.
6. لا يتم إنشاء `03_MODULES_AND_FEATURES.md` أو `09_IMPLEMENTATION_PLAN.md` إلا بواسطة Tera بعد قبول مخرجات النطاق الأساسية.
7. لا يتم الانتقال إلى التنفيذ البرمجي إلا بعد اعتماد خطة التنفيذ ومعايير القبول.

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
