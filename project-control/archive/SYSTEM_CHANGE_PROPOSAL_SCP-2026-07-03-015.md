# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-03-015

## Title:
**Step C — Operational Enforcement of Preparation Documentation Governance**
ربط حالات دورة حياة الوثائق (Lifecycle Header) بسلوك SoftwareDesignerAgent و Tera و Pre-Execution Gate

## Request Type:
Runtime Enforcement / Agent Behavior Update / Gate Integration

## Problem:
Steps A و B أنشأت حوكمة الوثائق (policy + templates + catalog metadata)، لكن **لا يوجد Enforcement عملي يجبر المنظومة على احترام حالات الوثائق**:

1. **SoftwareDesignerAgent** لا يتحقق حاليًا من Lifecycle Header — يقرأ أي ملف تحضير بغض النظر عن حالته
2. **Tera** لا يطبّق انتقالات الحالة (Draft → Under Cross-Review → MBA → System Approved) بشكل نظامي
3. **Pre-Execution Gate** لا يتحقق من جاهزية الوثائق (حالتها ≥ Module Baseline Approved)
4. **TASK-PREP** لا ينشئ الملفات مع Lifecycle Header تلقائيًا
5. لا يوجد رادع يمنع استهلاك وثيقة قبل اكتمالها

## Evidence:
- `tera-system/TeraPreExecutionGate.md` §5 (Checklist) — البند 1 يتحقق فقط من "ارتباط المهمة بخطة التنفيذ"، لا من حالة وثائق التحضير
- `.opencode/agents/tera-software-designer.md` §4 (What It Reads) — يقرأ الملفات مباشرة دون فحص Lifecycle Header أو الحالة
- `tera-system/TeraAgent.md` §4.3–4.4 — يعرّف Phase 4/5 دون transition منطقي لحالات الوثائق
- `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` §41 — موجود (من Step B) لكن لا يوجد إلزام باستخدامه في TASK-PREP

## Affected Files:

### Update
1. `.opencode/agents/tera-software-designer.md` — إضافة قاعدة فحص Lifecycle Header
2. `tera-system/TeraPreExecutionGate.md` — إضافة بند تحقق من حالة وثائق التحضير
3. `tera-system/TeraAgent.md` — إضافة transition logic لحالات الوثائق في Phase 4
4. `tera-system/TeraSubAgents.md` §6.9 — تحديث سلوك SoftwareDesignerAgent
5. `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` — تحديث قالب TASK-PREP (أو إضافة ملاحظة)
6. `tera-system/AGENT_ACTIVATION_MATRIX.md` — إضافة تفعيل Checker بعد Maker
7. `.opencode/agents/tera.md` — إضافة قواعد Enforcement

### No new files
- Strict Anti-Bloat: كل التغييرات داخل ملفات موجودة

---

## Proposed Change:

### 1. SoftwareDesignerAgent — فحص Lifecycle Header

**في `.opencode/agents/tera-software-designer.md`:**

#### 1.1 قاعدة جديدة في Section 4 (What It Reads):
```text
### 4.1 Lifecycle Header Consumption Gate (جديد — حوكمة الوثائق)

قبل قراءة أي ملف تحضيري، يجب:

1. **التحقق من وجود Lifecycle Header** في بداية الملف (Section 41 من TERA_RUNTIME_TEMPLATES.md).
   - إذا غاب الـ Header ← يرفع `Design Gap` ولا يقرأ الملف.
2. **التحقق من Current State في الـ Header ≥ Module Baseline Approved**.
   - إذا كانت الحالة أقل (`Draft` أو `Under Cross-Review`) ← يرفع `Design Gap`: "Document [name] is at [state], requires ≥ MBA".
   - لا يقرأ الملف ولا يخمن.
3. **التحقق من أن Baseline Module يغطي الموديول المطلوب في المهمة**.
   - إذا كان `Baseline Module: Inventory` والمهمة عن `Sales` ← يرفع `Module Coverage Gap`.
4. إذا اجتازت جميع الفحوصات ← يقرأ الملف بشكل طبيعي.
```

### 2. TeraPreExecutionGate — إضافة بند تحقق وثائقي

**في `tera-system/TeraPreExecutionGate.md` §5 (Checklist):**

إضافة بنود جديدة:

| # | سؤال التحقق | النتيجة المطلوبة |
|---|---|---|
| 24 | هل جميع وثائق التحضير المرتبطة بالمهمة تحتوي على Lifecycle Header؟ | Yes |
| 25 | هل حالة كل وثيقة تحضيرية ≥ Module Baseline Approved للموديول المستهدف؟ | Yes |
| 26 | هل تم التحقق من تطابق الحالة بين PREPARATION_PLAN.md (Section 9) والـ Header في كل ملف؟ | Yes |
| 27 | إذا رفع SoftwareDesignerAgent Design Gap متعلق بحالة وثيقة، هل تم حلّه قبل البوابة؟ | Yes |

### 3. TeraAgent — State Transition Logic

**في `tera-system/TeraAgent.md` §4.3 (Phase 4):**

إضافة خطوات جديدة بعد استلام Handback من Maker وقبل الانتقال إلى الملف التالي:

```text
[جديد — State Transition بعد Handback]
6.5. بعد استلام TASK-PREP Handback من Maker:
   a. تحقق من وجود Lifecycle Header في الملف (Section 41).
      - إذا غاب ← أعد الملف إلى Maker مع طلب إضافة الـ Header.
   b. تأكد أن Current State = "Draft" (أو "Under Cross-Review" إذا كان Checker).
   c. سجّل الحالة في PREPARATION_PLAN.md Section 9.

[جديد — State Transition بعد Cross-Review]
11.5. بعد استلام Cross-Review من Checker:
   a. إذا وجد Checker مشاكل ← أعد الملف إلى Maker (Draft) مع documented findings.
   b. إذا وافق Checker ← قدّم الحالة إلى "Under Cross-Review".
   c. راجع findings بنفسك (Tera):
      - إذا وجدت تناقضات مع وثائق أخرى ← أعد إلى Maker مع documented findings.
      - إذا لا توجد تناقضات ← قدّم الحالة إلى "Module Baseline Approved" (أو "System Approved" إذا كان الملف يغطي كل الموديولات).
   d. إذا كان Owner Approval مطلوبًا في الـ Header ← اعرض ملخصًا للمالك (Majed) للاعتماد.
   e. سجّل الحالة الجديدة في PREPARATION_PLAN.md Section 9 + في الـ Header.
```

**في `tera-system/TeraAgent.md` §4.4 (Phase 5):**

تحديث الخطوة 1 (Execution Readiness Check) لتشمل التحقق من Lifecycle Header.

### 4. AGENT_ACTIVATION_MATRIX — إضافة Checker لكل Prep Task

**في `tera-system/AGENT_ACTIVATION_MATRIX.md`:**

إضافة قاعدة:
```text
| Trigger | Action | Agent |
|---|---|---|
| TASK-PREP Handback received from Maker | Activate Checker agent for cross-review | As defined in PREPARATION_PLAN.md |
| Cross-review completed with PASS | Advance document state; if MBA reached → notify Tera for Phase 5 readiness | Tera |
```

### 5. TASK-PREP Template — تضمين Lifecycle Header

**في `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`:**

إضافة تعليمة إلزامية في بداية قالب TASK-PREP (أو في مقدمة Section 27/28):

```text
> **إلزامي:** يجب أن يبدأ الملف الناتج بـ Lifecycle Header القياسي (Section 41).
> Tera يرفض أي Handback بدون Header.
```

### 6. tera.md — قاعدة Enforcement

**في `.opencode/agents/tera.md`:**

إضافة قاعدة في Section 7 (Important Restrictions):
```text
- ❌ لا تفوّض SoftwareDesignerAgent قبل التأكد من أن وثائق التحضير ≥ Module Baseline Approved.
- ❌ لا تمرّر Pre-Execution Gate لوثيقة تحضير دون Lifecycle Header وحالة ≥ MBA.
- ❌ لا تقبل Handback من Maker بدون Lifecycle Header في الملف الناتج.
```

---

## Why This Is Necessary:

بدون Step C:
- SoftwareDesignerAgent سيقرأ وثائق Draft ويكوّن تصاميم على معلومات غير مستقرة ← إعادة عمل
- Pre-Execution Gate سيمرّر مهام معتمدة على وثائق غير جاهزة ← أخطاء تنفيذية
- لا يوجد حافز لـ Maker أو Checker لتحديث حالة الوثيقة ← الحوكمة تبقى نظرية
- Lifecycle Header سيكون موجودًا في القوالب لكن غير مفعّل فعليًا

---

## Rejected Alternatives:

1. **إنشاء LifecycleEnforcerAgent جديد** ← مرفوض (Anti-Bloat — Tera هو المنظم).
2. **إضافة Enforcement كاملة داخل SoftwareDesignerAgent فقط** ← مرفوض (يحتاج Tera و Gates).
3. **تأجيل Enforcement إلى مرحلة لاحقة** ← مرفوض (الحوكمة بدون Enforcement غير فعالة).

---

## Anti-Bloat Check:

- **ما المشكلة؟** غياب Enforcement runtime يجعل الحوكمة نظرية.
- **لماذا لا يكفي تعديل ملف واحد؟** لأن التغيير يمتد عبر 3 طبقات: Agent + Gate + Orchestrator.
- **هل الإضافة تقلل التعقيد؟** نعم — تمنع إعادة العمل والتصميم على معلومات غير مستقرة.
- **أثر سلبي على التوكنز؟** طفيف — قراءة Header (بضعة أسطر) قبل الملف.
- **طريقة أصغر؟** لا — هذا هو الحد الأدنى من Enforcement المطلوب.

---

## Risk:
- **منخفضة** — لا تغيير في بنية الملفات أو تعريفات العملاء الجوهرية
- إضافة فحوصات فقط قبل الاستهلاك والتنفيذ
- إذا تسببت الفحوصات في رفض وثائق Draft مشروعة → يكفي تعديل حالة الـ Header

---

## Rollback Plan:
1. `git restore .opencode/agents/tera-software-designer.md`
2. `git restore tera-system/TeraPreExecutionGate.md`
3. `git restore tera-system/TeraAgent.md`
4. `git restore tera-system/TeraSubAgents.md`
5. `git restore tera-system/AGENT_ACTIVATION_MATRIX.md`
6. `git restore tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`
7. `git restore .opencode/agents/tera.md`
8. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md

---

## Approval Required:
**Yes — Majed approval required before any edits**
