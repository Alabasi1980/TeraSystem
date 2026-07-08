# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-07-079

## Title
حظر TeraAgent من كتابة أي كود برمجي — إضافة قاعدة صارمة وصريحة

## Request Type
Owner Improvement Request (مشكلة سلوكية مكررة)

## Problem
TeraAgent يكتب الكود البرمجي بنفسه بدلاً من تفويض العملاء الفرعيين المختصين (EngineeringAgent, UI Designer, إلخ)، مع أن دوره الأساسي هو التنسيق والإدارة والمراجعة، وليس التنفيذ البرمجي.

### الأدلة:
1. **التعريف الحالي في `tera.md` سطر 19:** `"You are not a direct implementation agent by default."` — عبارة `by default` تجعل القاعدة ضعيفة وقابلة للالتفاف: "أنا لست منفذاً افتراضياً… لكن يمكنني التنفيذ إذا احتجت."
2. **غياب الحظر الصريح:** Section 9 (Important Restrictions) تمنع Tera من بدء الكود قبل التحضير، تعديل نظام Tera، إنشاء كل الملفات تلقائياً... لكن لا يوجد أي بند يمنعه صراحة من كتابة الكود.
3. **تضارب الصلاحيات:** TeraAgent يمتلك صلاحيات `write` و`edit` و`bash` — نفس صلاحيات EngineeringAgent — مما يمنحه القدرة التقنية على كتابة الكود دون أي مانع.
4. **تجاهل العملاء المختصين:** EngineeringAgent و UI Designer و SoftwareDesignerAgent مصممون خصيصاً لكتابة الكود بكفاءة عالية، ولديهم تعريفات متخصصة (أنماط التصميم، SOLID، Clean Code، 12-Factor App… إلخ)، لكن TeraAgent يتجاوزهم.
5. **انتهاك لنموذج التفويض:** `TeraSubAgents.md` ينص بوضوح: `"Tera Agent هو المالك الوحيد للقرار، والعملاء الفرعيون منفذون متخصصون محدودو النطاق."` — فإذا كان TeraAgent يكتب الكود، فهو يصبح منفذاً ومالك قرار في آن واحد، وهذا يلغي التقسيم المنطقي للعمل.

### الملاحظات السلوكية من Majed:
- TeraAgent "دائماً يحاول أن يكتب الأكواد ولا يستدعي عملاءه الفرعيين للكتابة"
- العملاء الفرعيون "لديهم قدرات ممتازة جداً"
- TeraAgent "قدراته كإدارة وقيادة فريق وتحكم وتدقيق ومتابعة… فالمفروض أنه لا يكتب ولو سطر واحد برمجي"

## Affected Files
1. **`.opencode/agents/tera.md`** — المصدر الأساسي للتعديل (إضافة قاعدة صارمة)
2. **`tera-system/AGENT_PERMISSION_MODEL.md`** — تحديث اختياري لتوضيح حدود TeraAgent ضمن هرم الصلاحيات
3. **`tera-system/TeraSubAgents.md`** — تحديث اختياري لتوثيق القاعدة في سجل العملاء

## Proposed Change

### التعديل الأساسي: إضافة حظر صريح في `.opencode/agents/tera.md`

#### (أ) تقوية العبارة الافتتاحية (السطر 19 الحالي):
**قبل:**
```
You are not a direct implementation agent by default.
```
**بعد:**
```
You are a pure orchestrator. You are FORBIDDEN from writing any programming code yourself — not even one line. Your role is to manage, plan, delegate, review, and decide. Code writing is exclusively the responsibility of your sub-agents (EngineeringAgent, UI Designer, etc.).
```

#### (ب) إضافة قاعدة جديدة في Section 9 (Important Restrictions):
إضافة البند التالي كأول بند في قائمة المحظورات:
```
- Write, edit, modify, or generate any programming code (HTML, CSS, JavaScript, TypeScript, Python, C#, SQL, Bash scripts, configuration files that contain code logic, API routes, database migrations, or any file whose primary purpose is to be executed or compiled). You may create non-code files: task files, reports, plans, documentation, preparation files, control records, and agent definitions.
```

#### (ج) إضافة Section فرعي جديد بعد Section 9 باسم `9.1 Code Boundary Rule`:

```markdown
## 9.1 Code Boundary Rule (قاعدة حدود الكود)

This is a **hard boundary**, not a guideline:

| TeraAgent MAY create | TeraAgent MUST NOT create |
|---|---|
| `*.md` (documentation, plans, tasks, reports) | `*.html` (application pages, templates, email templates with inline CSS) |
| `.opencode/agents/*.md` (sub-agent definitions) | `*.css`, `*.scss`, `*.less` (stylesheets) |
| `project-control/*.md` (control records) | `*.js`, `*.ts`, `*.jsx`, `*.tsx` (scripts, components) |
| `project-preparation/*.md` (analysis, design, prep) | `*.py`, `*.cs`, `*.java`, `*.go`, `*.php`, `*.rb` (backend code) |
| `tera-system/runtime/*.md` (system maintenance only) | `*.sql`, `*.prisma` (database schema/migrations) |
| `clients/.../*.md` (client documentation) | `*.json`, `*.yaml`, `*.yml`, `*.toml` (config with logic) |
| | `*.sh`, `*.ps1`, `*.bat` (shell scripts) |
| | `Dockerfile`, `docker-compose.yml`, `nginx.conf` (infra config) |
| | Any file that `bash`, `node`, `python`, or a compiler would execute |

### If code is needed:
1. Delegate to **EngineeringAgent** for backend, database, API, business logic, or full-stack code.
2. Delegate to **UI Designer** (`ui-designer`) for frontend visual implementation, HTML/CSS/JSX with styling.
3. Delegate to **tera-software-designer** for Technical Specifications before complex coding tasks.
4. **Never** write the code yourself — even for "quick", "simple", "trivial", or "obvious" fixes.

### Rule enforcement:
- If you catch yourself about to use `write` or `edit` on a code file: **STOP**.
- Ask: "Is this file executable, compilable, or does it contain programming logic?"
- If YES → delegate to the appropriate sub-agent.
- If you already wrote code: report it as a violation in `PROJECT_ACTIVITY_LOG.md` and do not continue.
```

#### (د) تحديث Section 12 (Execution Orchestration Core) — إضافة تذكير:
في نهاية Section 12 قبل Section 13، إضافة:
```
### Code Writing Delegation Rule

TeraAgent does NOT write application code. Every TASK-COD-* that requires code changes must be delegated to the appropriate sub-agent. TeraAgent's role during Phase 6 is:
- Assign tasks to sub-agents with clear acceptance criteria
- Review sub-agent handbacks
- Run Post-Execution Review Gate
- Accept, reject, or request fixes
- Manage task lifecycle

TeraAgent does not touch code files directly. Period.
```

## Why This Is Necessary

1. **يمنع التضارب المنطقي:** لا يمكن أن يكون TeraAgent "مالك القرار الوحيد" و"المنفذ" في نفس الوقت — هذا يلغي نموذج التفويض بالكامل.
2. **يضمن الجودة:** العملاء الفرعيون (EngineeringAgent, UI Designer) لديهم تعريفات متخصصة بـ Clean Code, SOLID, 12-Factor App... إلخ. TeraAgent لا يملك هذه التعريفات المتخصصة.
3. **يحافظ على اقتصاد التوكنز:** TeraAgent إذا كتب الكود ثم راجعه ثم دققه — يستهلك توكنز أكثر من تفويض EngineeringAgent مباشرة.
4. **يمنع الانجراف:** القاعدة الضعيفة الحالية ("by default") تسمح لـ TeraAgent بتبرير كتابة الكود في كل مرة بأنها "حالة استثنائية".
5. **يحمي دور TeraAgent الحقيقي:** TeraAgent قيمته الحقيقية في الإدارة والمراجعة والتحكم — لو كان سيكتب الكود بنفسه، فلماذا النظام كله موجود؟

## Rejected Alternatives

### البديل 1: تذكير ناعم فقط (بدون قاعدة صارمة)
- **لماذا رُفض:** "by default" الحالي فشل. المشكلة سلوكية وتحتاج قاعدة صارمة لا لبس فيها.
- **النتيجة المتوقعة:** سيعود TeraAgent لكتابة الكود في غضون أيام.

### البديل 2: سحب صلاحيات `write` و `edit` من TeraAgent
- **لماذا رُفض:** TeraAgent يحتاج `write` و `edit` لإنشاء ملفات `project-control/` و `project-preparation/` و `.opencode/agents/` وملفات النظام الأخرى. لا يمكن سحبها.
- **النتيجة المتوقعة:** سيتعطل TeraAgent عن أداء مهامه الإدارية الأساسية.

### البديل 3: إضافة EngineeringAgent كخطوة إلزامية قبل أي كتابة كود
- **لماذا رُفض:** هذا معقد جداً ويضيف Gate جديد غير ضروري. الحل الأبسط هو القاعدة الصريحة: إذا كان الهدف كوداً → فوّض فوراً.

### البديل 4: ترك الوضع كما هو مع مراقبة
- **لماذا رُفض:** Majed لاحظ المشكلة بنفسه. هذا ليس حلاً. هذا تأجيل للمشكلة.

## Anti-Bloat Check

| السؤال | الإجابة |
|---|---|
| ما المشكلة التي تحلها؟ | TeraAgent يكتب الكود بنفسه بدل تفويض المختصين |
| لماذا لا يكفي تعديل ملف موجود؟ | التعديل على ملف موجود (tera.md) هو المقترح بالضبط — لا إنشاء ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | لا نحتاج عميلاً جديداً — نحتاج تقييداً على TeraAgent الحالي |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | تقلل التعقيد: قاعدة واحدة صريحة تحل محل الغموض الحالي وتقلل التدخلات اليدوية |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | أثر إيجابي: تفويض EngineeringAgent للكود أكثر كفاءة من TeraAgent يكتب ويراجع نفسه |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | هذه هي الطريقة الأصغر: تعديل ملف واحد (tera.md) بعدة إضافات محددة |

## Risk
**منخفض.** التعديل يضيف قاعدة صارمة في تعريف TeraAgent فقط. لا يغير صلاحياته التقنية، ولا يغير بنية النظام، ولا يؤثر على عملاء آخرين.

المخاطر المحتملة:
- **خطر ضئيل:** TeraAgent قد "يحتج" بأن بعض ملفات `.md` تحتوي على أكواد (مثل `TECHNICAL_SPECIFICATION.md` التي قد تحتوي على pseudo-code). **الرد:** القاعدة تحدد أن الملفات التي تُنفَّذ أو تُجمَّع هي الممنوعة. التوثيق بقي آمناً.

## Rollback Plan
استعادة النسخة السابقة من `.opencode/agents/tera.md` من git. التعديلات سطحية ومحدودة في 3-4 مواضع داخل الملف.

## Approval Required
Majed — Approval required before any modification to `.opencode/agents/tera.md`.
