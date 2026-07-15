# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-03-016

## Title:
**SoftwareDesignerAgent Activation and Fast Path Rules — التعديل على قاعدة الإلزام**

إعادة تعريف متى يكون `SoftwareDesignerAgent` إلزامياً ومتى يسمح بـ `Fast Path` للمهام البسيطة، بناءً على توصيات الفريق وموافقة المالك.

---

## Request Type:
Agent Behavior Update / Policy Refinement

## Problem:

الوضع الحالي (من SCP-012) يجعل `SoftwareDesignerAgent` **إلزامياً لكل `TASK-COD-*` بدون استثناء**:

```text
- Mandatory for EVERY task
- No Fast Path exemption
- No Low-risk exemption
- ExecutionPreparationAgent removed
```

هذا يؤدي إلى:

1. **تضخم غير ضروري للمهام البسيطة** — typo, label, CSS بسيط تحتاج Technical Specification كاملة
2. **استهلاك توكنز أعلى** — كل مهمة حتى البسيطة تولّد `TECHNICAL_SPECIFICATION.md`
3. **إبطاء المهام الصغيرة** — دورة تصميم كاملة لمهمة قد تستغرق دقيقة تنفيذ
4. **خطر بيروقراطي** — المستخدم قد يتجنب المنظومة للمهام البسيطة

## Evidence:

- `tera-system/TeraPreExecutionGate.md` §3.6: "**لا يجوز تجاوز SoftwareDesignerAgent لأي سبب.** لا Fast Path ولا Low-risk"
- `.opencode/agents/tera-software-designer.md`: "**Mandatory** for every `TASK-COD-*` — no Fast Path exemption"
- `.opencode/agents/tera.md` §6.1: "**`SoftwareDesignerAgent` is mandatory for EVERY task** ... No Fast Path exemption. No Medium/High/Critical threshold."
- `tera-system/TeraAgent.md` §6.1: "**SoftwareDesignerAgent إلزامي لكل المهام** — لا Fast Path ولا Low-risk exemption"
- `tera-system/AGENT_ACTIVATION_MATRIX.md`: `MANDATORY` — لا يُتجاوز

---

## Proposed Change:

### A. SoftwareDesignerAgent — إلزامي (Mandatory) للمهام ذات الأثر

يكون `SoftwareDesignerAgent` **إلزامياً ولا يُتجاوز** إذا كانت المهمة تمس أيّاً من:

| المجال | أمثلة |
|---|---|
| **Database / Schema** | جداول، حقول، علاقات، Migration، Seed data |
| **API / Routes / Endpoints** | REST endpoints، GraphQL، Webhooks |
| **Business Logic** | قواعد عمل، معادلات، حالات، انتقالات workflow |
| **Security / Permissions / Auth** | صلاحيات، أدوار، JWT، Middleware |
| **Financial / Inventory Logic** | حركات مالية، مخزون، كلفة، إيصالات |
| **Workflow** | مسارات عمل، موافقات، حالات |
| **Cross-module Behavior** | تأثير على أكثر من موديول |
| **Architecture** | تغيير هيكل المشروع، الطبقات، patterns |
| **Migration / Integration** | ترحيل بيانات، تكامل خارجي |
| **UI Structure or UX Design** | شاشة جديدة، إعادة هيكلة شاشة، تغيير تدفق مستخدم |
| **Multiple Files with Side Effects** | مهمة تمس أكثر من ملفين أو لها آثار جانبية |

---

### B. Fast Path — مسموح للمهام منخفضة الخطورة

يُسمح بـ **Fast Path** (Tera يراجع المهمة مباشرة بدلاً من Technical Specification كاملة) إذا تحققت **كل** الشروط التالية:

| # | الشرط |
|---|---|
| 1 | **Low-risk** حسب تقييم Tera |
| 2 | **تعديل ملف واحد فقط** (أو ملفان مرتبطان مباشرة) |
| 3 | **لا DB impact** — لا جداول، لا حقول، لا Migration |
| 4 | **لا API impact** — لا endpoints جديدة، لا تعديل موجودة |
| 5 | **لا Business Logic impact** — لا قواعد عمل، لا معادلات |
| 6 | **لا Security / Permissions impact** |
| 7 | **لا Financial / Inventory impact** |
| 8 | **لا Cross-module impact** |
| 9 | **لا تغيير في بنية UI/UX** — تعديل بسيط فقط (نص، لون، spacing) |
| 10 | **Acceptance Criteria واضحة وقابلة للاختبار** |
| 11 | **Tera يستطيع مراجعة المخرجات مباشرة** دون حاجة لخبير تقني |

#### أمثلة Fast Path (مسموح)

```
- تصحيح خطأ إملائي (typo) في نص عرض
- تعديل label في شاشة
- تعديل CSS بسيط (لون، حجم خط، spacing)
- تعديل رسالة تأكيد أو تنبيه غير مرتبطة بمنطق أعمال
- تحديث تعليق أو توثيق بسيط
- إعادة ترتيب عناصر في قائمة (دون تغيير وظيفي)
```

#### أمثلة لا تدخل Fast Path (ممنوع)

```
- إضافة حقل جديد إلى نموذج بيانات
- تعديل Validation على حقل موجود
- إضافة أو تعديل API endpoint
- تعديل Schema قاعدة البيانات
- تعديل صلاحية أو دور مستخدم
- تعديل تقرير يعتمد على بيانات
- تعديل Workflow أو حالة
- تعديل منطق مالي أو مخزني
- تعديل شاشة إدخال رئيسية
```

---

### C. سلوك Tera في Fast Path

عند استخدام Fast Path:

1. **لا يُفعّل SoftwareDesignerAgent**
2. **Tera يقوم بمراجعة تقنية مباشرة** (Task Review) دون `TECHNICAL_SPECIFICATION.md` كاملة
3. **يُنتج فقط Task Engineering Decision مختصرة** داخل ملف المهمة بدلاً من Technical Specification
4. **يُطبّق Pre-Execution Gate بشكل طبيعي** (فحص المهمة وليس فحص Technical Specification)
5. **يُطبّق Post-Execution Review Gate إلزامياً** — لا استثناء
6. **يُسجّل سبب استخدام Fast Path** في ملف المهمة

#### الفرق بين المسارين:

```text
Normal Path (SDA Mandatory):
  TASK-COD → SoftwareDesignerAgent → TECHNICAL_SPECIFICATION.md → Pre-Execution Gate → تنفيذ → Post-Execution Review

Fast Path:
  TASK-COD → Tera Task Review (مباشر) → Pre-Execution Gate → تنفيذ → Post-Execution Review
```

---

## Affected Files:

### Update (all existing — لا ملفات جديدة)

| الملف | التغيير |
|---|---|
| `.opencode/agents/tera-software-designer.md` | تغيير قاعدة Activation من `Mandatory for every` إلى `Mandatory for impactful tasks` + إضافة Fast Path exception |
| `tera-system/TeraPreExecutionGate.md` §3.6 | تحديث: "لا Fast Path" ← "Fast Path مسموح للمهام منخفضة الخطورة حسب تعريف SCP-016" |
| `tera-system/TeraAgent.md` §4.4 + §5.4 + §6.1 | تحديث قواعد الإلزام و Fast Path |
| `tera-system/TeraSubAgents.md` §6.9 | تحديث تعريف SoftwareDesignerActor — إلزامي للمهام ذات الأثر |
| `.opencode/agents/tera.md` §6.1 + §6.2.1 | تحديث قاعدة "Mandatory for EVERY task" |
| `tera-system/AGENT_ACTIVATION_MATRIX.md` | تحديث شرط تفعيل SoftwareDesignerAgent |

### لا ملفات جديدة
- Strict Anti-Bloat: 6 ملفات موجودة فقط

---

## Why This Is Necessary:

بدون SCP-016:
- **مهمة typo** ستولّد Technical Specification كاملة ← هدر وقت وتوكنز
- **مستخدم سيتجنب المنظومة** للمهام البسيطة
- **مبدأ Anti-Bloat** في Tera نفسه ينكسر — التضخم ممنوع على العملاء لكن مسموح للنظام؟

مع SCP-016:
- المهام المؤثرة تحصل على **تصميم تقني كامل** (لا تخمين)
- المهام البسيطة تمر بـ **Fast Path خفيف** (لا بيروقراطية)
- التوازن بين **الجودة والسرعة**

---

## Rejected Alternatives:

1. **إلغاء SoftwareDesignerAgent بالكامل** — مرفوض، المهام المعقدة تحتاجه
2. **جعله اختيارياً بدون معايير** — مرفوض، سيؤدي إلى تخطيه في مهام تحتاجه فعلاً
3. **الاكتفاء بـ "Medium/High/Critical"** — مرفوض (من التقرير الأصلي): Low-risk تمس DB تحتاج SDA رغم خطورتها المنخفضة
4. **لا تغيير (إبقاء الوضع الحالي)** — مرفوض بقرار المالك

---

## Anti-Bloat Check:

- **ما المشكلة؟** إلزام SDA لكل مهمة يسبب تضخماً للمهام البسيطة
- **لماذا لا يكفي تعديل ملف واحد؟** لأن الإلزام منتشر عبر 6 ملفات نظامية
- **هل الإضافة تقلل التعقيد؟** نعم — تمنع Technical Specifications غير الضرورية
- **أثر سلبي على التوكنز؟** إيجابي — تقليل توليد Technical Specifications للمهام البسيطة
- **طريقة أصغر؟** لا — 6 ملفات هو الحد الأدنى المطلوب

---

## Risk:

- **منخفضة** — لا يمس حوكمة الوثائق (Steps A+B+C)
- **قابلة للتراجع** — العودة إلى الإلزام المطلق سهل (git restore)
- **خطر وحيد:** مهمة بسيطة قد تمر دون SDA ولها أثر جانبي غير متوقع
  - **المعالجة:** Post-Execution Review Gate إلزامي حتى في Fast Path
  - **المعالجة 2:** قائمة المنع (جدول 11 شرطاً) تمنع Fast Path إذا كان هناك أي أثر

---

## Rollback Plan:

1. `git restore .opencode/agents/tera-software-designer.md`
2. `git restore tera-system/TeraPreExecutionGate.md`
3. `git restore tera-system/TeraAgent.md`
4. `git restore tera-system/TeraSubAgents.md`
5. `git restore .opencode/agents/tera.md`
6. `git restore tera-system/AGENT_ACTIVATION_MATRIX.md`
7. حذف هذا الإدخال من SYSTEM_EVOLUTION_LOG.md

---

## Approval Required:
**Yes — Majed approval required before any edits**
