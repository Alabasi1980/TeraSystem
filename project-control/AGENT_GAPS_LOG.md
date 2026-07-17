# AGENT_GAPS_LOG.md

> **سجل الفجوات النظامية — للإبلاغ عن نقص في صلاحيات أو تعريفات أو سلوك العملاء الفرعيين**
> النظام: Tera
> آلية المعالجة: `TeraSystemEvolutionAgent (Hares)`

## المبدأ

```text
Agents observe and report.
TeraSystemEvolutionAgent reviews and proposes.
Majed approves.
Only then changes are implemented.
```

لا يجوز للعملاء تعديل أنفسهم أو تعديل عملاء آخرين. الفجوة تُرفع هنا وتُعالج عبر Hares.

## حالات الفجوات

| الحالة | المعنى | المسؤول |
|:-------|:-------|:--------|
| `Pending` | فجوة مرفوعة ولم تُفحص بعد | مقدم الفجوة |
| `Under Review` | قيد المراجعة من Hares | TeraSystemEvolutionAgent |
| `Approved` | تمت الموافقة على المعالجة → تصبح SYSTEM_CHANGE_PROPOSAL | TeraSystemEvolutionAgent بعد موافقة Majed/مقدم الفجوة |
| `Applied` | تم تنفيذ التغيير في SYSTEM_EVOLUTION_LOG.md | TeraSystemEvolutionAgent |
| `Rejected` | رُفضت مع سبب واضح | TeraSystemEvolutionAgent |
| `Duplicate` | فجوة مكررة — أُغلقت | TeraSystemEvolutionAgent |
| `Deferred` | أُرجئت لدورة مراجعة لاحقة | TeraSystemEvolutionAgent |

## قواعد الإبلاغ

1. سجل الفجوة فقط.
2. لا تحاول إصلاح الفجوة بنفسك إذا كانت تتعلق بتعريف عميل آخر.
3. إذا كانت الفجوة `Rejected` أو `Duplicate` أو `Applied`، لا تعد فتحها دون سبب جديد.
4. إذا كانت الفجوة `Pending` أو `Under Review` أو `Approved`، لا تفتح فجوة مكررة — انتظر دور Hares.
5. لا تسجل تفاصيل صغيرة — الفجوة يجب أن تكون قابلة للقياس أو ذات أثر واضح.

## سجل الفجوات

```text
## [YYYY-MM-DD] - [Agent Name] - GAP-XXX

- Title:
- Agent:
- Gap Type: Bug / Missing Capability / Policy Gap / Tool Gap / Permission Gap / Performance / Documentation Gap / Improvement
- Severity: Critical / High / Medium / Low
- Description:
- Evidence:
- Impact:
- Recommended Action:
- Suggested Target File:
- Status: Pending / Under Review / Approved / Applied / Rejected / Duplicate / Deferred
```

---

## GAP-003 — Verification fallback needed when running app locks normal dotnet build

- **Title:** أوامر التحقق لا توضّح مساراً بديلاً عند قفل ملفات build بسبب تشغيل التطبيق
- **Agent:** EngineeringAgent / Tera workflow
- **Gap Type:** Improvement / Documentation Gap
- **Severity:** Low
- **Description:** أثناء TASK-KPI-FIX-016 فشل `dotnet build` العادي بسبب قفل ملفات `bin\Debug\net8.0` من عملية تشغيل التطبيق، رغم أن التجميع نفسه يمكن التحقق منه بنجاح باستخدام مجلد output مؤقت.
- **Evidence:** EngineeringAgent reported normal `dotnet build` blocked by running `WarehouseDashboard.Web` process, while `dotnet build -o C:\Users\Fares\AppData\Local\Temp\opencode\WarehouseDashboard-build-check` succeeded with 0 warnings and 0 errors.
- **Impact:** قد تظهر المهمة كأنها فشلت رغم أن الكود يترجم بنجاح، مما يربك قبول المهام عندما يكون التطبيق قيد التشغيل محلياً.
- **Recommended Action:** إضافة إرشاد رسمي في بروتوكول/قوالب التفويض يسمح بفحص compile fallback إلى مجلد مؤقت عند وجود file lock، مع تسجيل ذلك كتحقق بديل وليس بديلاً عن إعادة تشغيل التطبيق قبل التشغيل الفعلي.
- **Suggested Target File:** `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` أو قوالب التفويض ذات الصلة
- **Status:** Pending

---

## GAP-001 — EngineeringAgent يكتب الكود في المسار الخطأ

- **Title:** EngineeringAgent لا يلتزم بمسار مشروع العميل عند إنشاء الملفات
- **Agent:** EngineeringAgent (سلوك خاطئ) + TeraAgent (إشراف قاصر)
- **Gap Type:** Bug / Policy Gap
- **Severity:** Critical
- **Description:** EngineeringAgent عند تفويضه بمهمة TASK-COD-001 أنشأ مشروع WPF بالكامل في `D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\src\TeraQuotation\` (جذر المنظومة) بدلاً من مسار العميل الصحيح داخل `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/source/`. وهو لم يتحقق من أن المسار الذي أعطاه إياه TeraAgent في مهمة `src/TeraQuotation/` هو مسار نسبي غير آمن.
- **Evidence:** تم إنشاء مجلد `src/` في جذر المنظومة، وملفات .csproj و .cs في غير مكانها الصحيح. تم نقلها يدوياً بعد اكتشاف الخطأ.
- **Impact:** 
  - تلويث جذر المنظومة بكود لا ينتمي إليها
  - اختلاط Git history بين كود TeraSystem وتطبيق العميل
  - صعوبة الفصل بين المشاريع مستقبلاً
- **Recommended Action:**
  1. إضافة قاعدة صارمة في تعريف/تعليمات EngineeringAgent: **يجب كتابة الكود فقط داخل المسار المحدد في Allowed Write Targets حرفياً. إذا كان المسار نسبياً، يستخدم المسار الكامل من جذر الـ Workspace + المسار النسبي.**
  2. إضافة فحص مسبق: قبل إنشاء أي ملف، تحقق أن المسار النهائي لا يقع خارج `clients/` أو المجلدات المسموحة.
  3. إضافة قاعدة في TeraAgent: عند تفويض EngineeringAgent، يجب تحديد Allowed Write Targets كمسارات كاملة (absolute) وليس نسبية.
- **Suggested Target File:** تعريف EngineeringAgent (built-in — يحتاج SYSTEM_CHANGE_PROPOSAL عبر Hares)
- **Status:** Applied
- **Hares Review:** 2026-07-13 — تم التنفيذ عبر SCP-2026-07-13-094
- **SCP:** `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-13-094.md`

---

## GAP-002 — TeraAgent لم يحدد Allowed Write Targets كاملاً

- **Title:** TeraAgent فوّض EngineeringAgent بمسار نسبي غير آمن
- **Agent:** TeraAgent
- **Gap Type:** Policy Gap / Improvement
- **Severity:** High
- **Description:** في تفويض TASK-COD-001، كتبت Allowed Write Targets كـ `src/TeraQuotation/` (نسبي) بدلاً من المسار الكامل للعميل. هذا جعل EngineeringAgent ينشئ المشروع في `src/` تحت جذر المنظومة.
- **Evidence:** نص التفويض: ```## Allowed Write Targets
src/TeraQuotation/
├── Models/*.cs
├── Data/AppDbContext.cs
├── Data/Migrations/
├── TeraQuotation.csproj```
- **Impact:** تسبب بخطأ فادح في مسار الملفات. استغرق تصحيحه وقتاً وجهوداً.
- **Recommended Action:** 
  1. توثيق قاعدة: **Allowed Write Targets يجب أن تكون مسارات كاملة (absolute paths) أو مسارات نسبة إلى مسار مشروع العميل المحدد مسبقاً.**
  2. إضافة حقل `ClientAppPath` في كل تفويض لعميل فرعي يحدد جذر التطبيق.
- **Suggested Target File:** `.opencode/agents/tera.md` أو `TERA_RUNTIME_PROTOCOLS.md`
- **Status:** Applied
- **Hares Review:** 2026-07-13 — تم التنفيذ عبر SCP-2026-07-13-094
- **SCP:** `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-13-094.md`

---

## GAP-003 — TeraAgent وضع ملفات العميل في جذر المنظومة بدلاً من مسار العميل

- **Title:** TeraAgent لم يفهم أن project-preparation/ و project-control/ في الجذر هما قوالب فقط
- **Agent:** TeraAgent
- **Gap Type:** Policy Gap / Documentation Gap
- **Severity:** Critical
- **Description:** TeraAgent أنشأ جميع ملفات تحضير وتحكم TeraQuotation في `project-preparation/` و `project-control/` في جذر المنظومة. الصواب: كل عميل له مساره الخاص، وجذر المنظومة للقوالب فقط. سبب الخطأ: قراءة حرفية للنص "Project preparation content outputs must be created inside project-preparation/" دون فهم أن هذا ينطبق على القوالب، وأن كل مشروع عميل له مساره المستقل.
- **Evidence:** 8 ملفات تحضير و 3 ملفات تحكم أنشئت في جذر المنظومة وتم نقلها إلى `clients/.../APP-TeraQuotation/`.
- **Impact:** 
  - اختلاط ملفات العملاء مع قوالب المنظومة
  - عدم قابلية توسّع المنظومة لعملاء متعددين
  - صعوبة عزل مشروع عميل عن آخر
- **Recommended Action:** 
  1. تحديث TeraAgent system prompt أو TERA_RUNTIME_PROTOCOLS.md لتوضيح أن `project-preparation/` و `project-control/` في الجذر للقوالب فقط
  2. إضافة قاعدة: **كل عميل له مسار `project-preparation/` و `project-control/` خاص داخل مسار العميل**
  3. للمشاريع الداخلية (غير العملاء)، يُستخدم الجذر
- **Suggested Target File:** `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` أو `.opencode/agents/tera.md` §7
- **Status:** Applied
- **Hares Review:** 2026-07-13 — تم التنفيذ عبر SCP-2026-07-13-094
- **SCP:** `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-13-094.md`
