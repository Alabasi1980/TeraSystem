# SYSTEM_CHANGE_PROPOSAL_SCP-012

```text
Title: ترقية QAAndAcceptanceAgent إلى عميل اختبارات حقيقي (ف+تنفيذ)
Request Type: Agent Enhancement + Role Expansion
Date: 2026-07-12
Source: Majed (Owner Decision) + QA_AGENT_SPECIFICATION.md
```

---

## 1. Problem

```text
TeraAgent يقبل مهام TASK-COD بدون التحقق الفعلي من صحة الكود.
المثال: TASK-COD-001 (Oracle Test) قُبل كوده بدون تشغيل dotnet build أو dotnet run.

السبب: لا يوجد عميل فرعي يملك صلاحية 실행 CLI لاختبار الكود فعلياً.
QAAndAcceptanceAgent الحالي يكتب خطط اختبار ومعايير قبول فقط — لا ينفّذ اختبارات فعلية.
```

---

## 2. Decision

**الخيار A المختار:** دمج دور التنفيذ في `QAAndAcceptanceAgent` الموجود بدلاً من إنشاء عميل جديد.

**المبرر:**
- لا يُضاف عميل جديد → التوسع أقل
- `QAAndAcceptanceAgent` موجود بالفعل كعميل أساسي
- صلاحية `RUN_TESTS` موجودة بالفعل في نموذج الصلاحيات
- يُفرّق بين دورَين واضحَين داخل عميل واحد: **التخطيط** vs **التنفيذ**

---

## 3. Affected Files

| الملف | نوع التغيير | الأثر |
|---|---|---|
| `tera-system/TeraSubAgents.md` §5.7 | **تعديل جوهري** — توسيع الدور والصلاحيات والأدوات | تأثير مباشر على توليد العميل في المشاريع |
| `tera-system/AGENT_ACTIVATION_MATRIX.md` §2.1 | **إضافة** — triggers جديدة لتفعيل QA execution | تأثير على قواعد التفعيل |
| `tera-system/AGENT_PERMISSION_MODEL.md` §3.1 | **توضيح** — تأكيد صلاحية RUN_TESTS + أدوات CLI | تأثير على نموذج الصلاحيات |
| `tera-system/TOOLING_AND_MCP_POLICY.md` | **لا تغيير** — يُغطّي بالفعل Playwright + API Testing لـ QA | — |
| `tera-system/AGENT_DEPENDENCY_MAP.md` | **تحديث** — إضافة QAAndAcceptanceAgent إذا لزم | تأثير على خريطة العلاقات |

**ملفات لا تحتاج تعديل:**
- `.opencode/agents/` — لا يُنشأ ملف جديد (العميل مشروط ويُولّد لكل مشروع)
- `TeraPolicyMap.md` — لا تغيير على مصادر الحقيقة
- `TeraArchitectureMap.md` — لا تغيير على الطبقات

---

## 4. Proposed Change

### 4.1 تعديل TeraSubAgents.md §5.7 — توسيع الدور

**الحالة الحالية:**

```text
QAAndAcceptanceAgent
- الدور: تحديد الاختبارات ومعايير القبول ومراجعة جاهزية المخرجات
- الصلاحية: RUN_TESTS (في AGENT_PERMISSION_MODEL)
- الأدوات: غير محددة صراحةً
```

**الحالة المقترحة:**

```text
QAAndAcceptanceAgent
- الدور: [محسّن] تخطيط + تنفيذ + مراجعة اختبارات
- الوضع الأول: Planning Mode (خطط اختبار + معايير قبول)
- الوضع الثاني: Execution Mode (تشغيل CLI + إنتاج تقارير فعلية)
- الصلاحية: RUN_TESTS (تغطي كلا الوضعين)
- الأدوات: bash, read, write, glob, grep, webfetch
```

#### التفصيل — الوضعين:

**Planning Mode (الوضع الحالي — يُحافظ عليه):**
```text
المدخل: مهمة تحتاج خطة اختبار أو معايير قبول
الإجراء:
  1. قراءة ملفات التحضير والتصميم
  2. تحديد الاختبارات المطلوبة لكل معيار قبول
  3. كتابة خطة الاختبار في 10_TESTING_AND_ACCEPTANCE.md
  4. أو كتابة معايير القبول في ملف المهمة
المخرج: خطة اختبار / معايير قبول
الصلاحية: WRITE_DOCS (كالحالي)
الأدوات: read, write, glob, grep (لا bash)
```

**Execution Mode (الجديد — يُضاف):**
```text
المدخل: مهمة TASK-COD-* جاهزة للاختبار بعد التنفيذ
الإجراء:
  1. قراءة ملف المهمة ومعايير القبول
  2. تحديد اختبارات التنفيذ المطلوبة (build? test? run? connect?)
  3. تنفيذ الاختبارات بالتسلسل:
     a. dotnet restore (للمشاريع .NET)
     b. dotnet build
     c. dotnet test (إن وُجدت اختبارات)
     d. dotnet run --no-build (للمشاريع القابلة للتشغيل)
     e. اختبار اتصال قاعدة البيانات (إن وُجد)
     f. اختبار API endpoints (إن وُجدت)
  4. جمع النتائج (logs, exit codes, output)
  5. إنتاج تقرير اختبار رسمي
  6. إعادة التقرير إلى Tera
المخرج: تقرير اختبار رسمي (PASS / PARTIAL / FAIL)
الصلاحية: RUN_TESTS + bash
الأدوات: bash, read, write, glob, grep, webfetch
```

#### التكامل مع سير العمل:

```text
TeraAgent: "TASK-COD-XXX — يحتاج اختبار"
       │
       ▼
QAAndAcceptanceAgent: 
  ├── Planning Mode: إذا كانت المهمة تحتاج خطة اختبار
  │   → ينتج خطة الاختبار
  │
  └── Execution Mode: إذا كانت المهمة منفذة وتحتاج تحقق
      → ينفذ الاختبارات
      → ينتج تقرير اختبار (PASS/PARTIAL/FAIL)
       │
       ▼
TeraAgent:
  ├── ✅ PASS → قبول المهمة
  ├── ⚠️ PARTIAL → قبول مع ملاحظات
  └── ❌ FAIL → إعادة للمطور مع التقرير
```

### 4.2 تعديل AGENT_ACTIVATION_MATRIX.md §2.1

**إضافة Trigger جديد:**

```text
| QAAndAcceptanceAgent | QA_ACCEPTANCE_AGENT | DOCUMENT_READY: بعد تنفيذ TASK-COD يحتاج اختبار | 6 | إذا كانت المهمة بسيطة ويمكن لـ Tera مراجعتها مباشرة | ملف المهمة + معايير القبول |
```

**تحديث Trigger الحالي ليشمل:**

```text
| QAAndAcceptanceAgent | QA_ACCEPTANCE_AGENT | PHASE_GATE + DOCUMENT_READY: 
  - Planning: قبل إعداد خطة التنفيذ (Phase 5)
  - Execution: بعد تنفيذ TASK-COD يحتاج اختبار (Phase 6)
  - Review: قبل Phase 7 | 5–6–7 |
```

### 4.3 تعديل AGENT_PERMISSION_MODEL.md §3.1

**تحديث ملاحظة QAAndAcceptanceAgent:**

```text
| QAAndAcceptanceAgent | QA_ACCEPTANCE_AGENT | RUN_TESTS | 
  إلى READ_ONLY إذا كان مراجعة فقط | 
  Planning Mode: WRITE_DOCS |
  Execution Mode: RUN_TESTS + bash |
  يشغل اختبارات CLI فعلياً ويكتب تقارير |
```

### 4.4 تعديل TeraSubAgents.md §5.7 — الأدوات

**إضافة قسم أدوات:**

```text
### الأدوات المتاحة

| الأداة | Planning Mode | Execution Mode |
|---|---|---|
| bash | ❌ | ✅ (dotnet, npm, curl, ping) |
| read | ✅ | ✅ |
| write | ✅ (خطط + تقارير) | ✅ (تقارير اختبار) |
| glob | ✅ | ✅ |
| grep | ✅ | ✅ |
| webfetch | ❌ | ✅ (اختبار API endpoints) |
```

### 4.5 تعديل TeraSubAgents.md §5.7 — الممنوعات

**إضافة:**

```text
### Execution Mode — ممنوعات إضافية

- لا يعدّل كود الإنتاج (أي تعديل يحتاج Design Gap)
- لا ينشر التطبيقات (Deploy)
- لا يصل إلى كلمات مرور حقيقية — يستخدم Placeholders فقط
- لا يحذف ملفات
- لا يشغّل أوامر ضارة (drop table, delete, format)
- لا يتجاوز مهلة زمنية 60 ثانية لكل اختبار تشغيل
- لا يستخدم بيانات حقيقية — يطلب Placeholders دائماً
```

### 4.6 إضافة نموذج تقرير الاختبار (في TeraSubAgents.md §5.7)

**Template:**

```markdown
# Test Report — TASK-COD-XXX

## Task Information
- **Task:** TASK-COD-XXX — [وصف المهمة]
- **اختبار:** QAAndAcceptanceAgent Execution Mode ([التاريخ والوقت])
- **البيئة:** [OS, .NET version, tools]

## Test Results

| # | الاختبار | النتيجة | التفاصيل |
|---|---|---|---|
| 1 | `dotnet restore` | ✅ PASS | جميع الحزم موجودة |
| 2 | `dotnet build` | ✅ PASS | 0 errors, 3 warnings |
| 3 | `dotnet test` | ✅ PASS | 5 passed, 0 failed |
| 4 | `dotnet run` | ✅ PASS | Connected successfully |

## Pass/Fail Decision
- ✅ **PASS** — كل الاختبارات ناجحة — جاهز للقبول
- ⚠️ **PARTIAL** — نجاح جزئي — يحتاج ملاحظات
- ❌ **FAIL** — فشل — يُعاد للمطور مع التقرير

## Full Output
```text
[إخراج الـ console هنا]
```

## QAAndAcceptanceAgent Signature
[التاريخ والوقت]
```

### 4.7 تحديث AGENT_DEPENDENCY_MAP.md

**إضافة صف QAAndAcceptanceAgent:**

```text
| **QAAndAcceptanceAgent** | `tera.md` (يُفعّله Tera) | لا يستدعي عملاء آخرين | ملفات المهمة، ملفات التحضير، logs |
```

---

## 5. Why This Is Necessary

1. **الفجوة حقيقية:** لا يوجد عميل ينفذ اختبارات CLI فعلياً. `EngineeringAgent` يكتب الكود لكن لا يختبره. `QAAndAcceptanceAgent` يخطط لكن لا ينفذ.
2. **السلامة:** بدون اختبار فعلي، يُقبل كود قد لا يعمل أو يحتوي أخطاء بناء.
3. **الكفاءة:** إضافة وضع تنفيذ لعميل موجود أقل تعقيداً من إنشاء عميل جديد + تحديد العلاقة بينهما.
4. **الاتساق:** يحافظ على مبدأ الفصل: التخطيط (Planning) والتنفيذ (Execution) داخل عميل واحد متعدد الأدوار.

---

## 6. Rejected Alternatives

### البديل 1: إنشاء qa-agent منفصل (المواصفة الأصلية)

```text
السبب في الرفض: تكرار مع QAAndAcceptanceAgent + حاجة لتعريف العلاقة + ملف جديد في .opencode/agents/
النتيجة: تضخم + تعقيد + خطر تضارب
```

### البديل 2: تعديل EngineeringAgent ليشمل الاختبار

```text
السبب في الرفض: خلط بين التنفيذ والمراجعة — EngineeringAgent يكتب الكود ولا يجب أن يختبره بنفسه
النتيجة: فقدان الاستقلالية في المراجعة
```

### البديل 3: استخدام Monitor أو Auditor للاختبار

```text
السبب في الرفض: Monitor يراجع التوافق مع الخطة، Auditor يراجع الجودة العامة — لا ينفّذ اختبارات CLI
النتيجة: خلط بين المراجعات الإدارية والاختبارات التقنية
```

---

## 7. Anti-Bloat Check

| السؤال | الإجابة |
|---|---|
| ما المشكلة التي تحلها؟ | فجوة حقيقية: لا يوجد عميل ينفّذ اختبارات CLI |
| لماذا لا يكفي تعديل ملف موجود؟ | يُضاف لملف موجود (TeraSubAgents.md §5.7) — لا ملف جديد |
| لماذا لا يكفي عميل موجود؟ | يُطوّر عميل موجود (QAAndAcceptanceAgent) — لا عميل جديد |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | تزيده بشكل طفيف (وضع واحد إضافي) — لكن المبرر قوي |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | نعم — كل مهمة اختبار تستهلك جلسة إضافية. لكن هذا ضروري |
| هل توجد طريقة أصغر؟ | لا — هذا أقل تعقيداً من خيارات أخرى |

---

## 8. Risk

| المخاطرة | الاحتمال | التأثير | المعالجة |
|---|---|---|---|
| البيئة لا تحتوي .NET SDK | عالي | منع الاختبار | QA يبلغ Tera أن البيئة غير مناسبة |
| Oracle/SQL Server غير متاحين | عالي | فشل اختبار الاتصال | QA يسجّل "غير متاح" ويُبلغ Tera |
| مدة الاختبار طويلة | متوسط | استهلاك Token | timeout 60 ثانية كحد أقصى لكل اختبار |
| خلط Planning و Execution في عميل واحد | منخفض | تعقيد دور العميل | فصل واضح بالوضعين + وصف دقيق لكل وضع |
| استخدام بيانات حقيقية عن غير قصد | منخفض | كشف أسرار | QA يرفض بيانات حقيقية — يطلب Placeholders |

---

## 9. Rollback Plan

```text
1. إعادة TeraSubAgents.md §5.7 إلى النسخة الحالية (بدون Execution Mode)
2. إعادة AGENT_ACTIVATION_MATRIX.md إلى النسخة الحالية (بدون Trigger الجديد)
3. إعادة AGENT_PERMISSION_MODEL.md إلى النسخة الحالية (بدون التوضيحات الجديدة)
4. إعادة AGENT_DEPENDENCY_MAP.md إلى النسخة الحالية (بدون الصف الجديد)
5. لا يتأثر أي ملف آخر لأن التغييرات محدودة بهذه الملفات الأربعة
```

---

## 10. Approval Required

```text
✅ Majed: وافق على المبدأ (QC confirmed 2026-07-12)
⏳ Majed: ينتظر الاعتماد النهائي على هذا SCP بعد المراجعة
⏳ Hares: ينتظر الاعتماد لتنفيذ التعديلات الأربعة
```

---

## 11. Implementation Plan (بعد الاعتماد)

```text
المرحلة 1: تعديل TeraSubAgents.md §5.7 (التوسيع الأكبر)
  → إضافة Execution Mode + الأدوات + الممنوعات + نموذج التقرير

المرحلة 2: تعديل AGENT_ACTIVATION_MATRIX.md §2.1
  → إضافة/تحديث Trigger لـ QA Execution

المرحلة 3: تعديل AGENT_PERMISSION_MODEL.md §3.1
  → توضيح الصلاحيات حسب الوضعين

المرحلة 4: تعديل AGENT_DEPENDENCY_MAP.md
  → إضافة صف QAAndAcceptanceAgent

المرحلة 5: Validation Gates
  → Anti-Bloat Check ✅
  → Policy Map Check ✅
  → Architecture Map Check ✅
  → No client-app contamination ✅
  → No unauthorized privilege expansion ✅
  → git diff --check ✅

المرحلة 6: تسجيل في SYSTEM_EVOLUTION_LOG.md
```

---

## 12. Expected Outcome

```text
بعد التنفيذ:
- QAAndAcceptanceAgent يصبح عميل اختبارات كامل:
  ✅ يخطط اختبارات (Planning Mode — كالحالي)
  ✅ ينفّذ اختبارات CLI (Execution Mode — جديد)
  ✅ ينتج تقارير اختبار رسمية (جديد)
  ✅ يُمرّر نتائج PASS/PARTIAL/FAIL إلى Tera (جديد)

- لا يُنشأ عميل جديد (توفير التعقيد)
- لا يُخلّط بين التخطيط والتنفيذ (فصل واضح بالوضعين)
- يُغلق فجوة TASK-COD بدون اختبار (الفجوة الأصلية)
```
