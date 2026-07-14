# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-13-094 — تصحيح انضباط المسارات عند تفويض العملاء الفرعيين (جذر GAP-001, GAP-002, GAP-003)

---

### Title

تصحيح **انضباط المسارات (Path Discipline)** عند تفويض العملاء الفرعيين — معالجة جذرية لثلاث فجوات: EngineeringAgent يكتب في المسار الخطأ، TeraAgent يفوّض بمسارات نسبية، و TeraAgent ينشئ ملفات العملاء في جذر المنظومة

### Request Type

Policy Update + Agent Improvement + Protocol Change

### Problem

ثلاث فجوات منفصلة لكنها تشترك في جذر واحد: **عدم وجود آلية إجبارية للتحقق من المسار الصحيح** عند التفويض والتنفيذ.

| الفجوة | المشكلة | الجذر |
|--------|---------|-------|
| GAP-001 | EngineeringAgent أنشأ كود WPF في `src/` (جذر المنظومة) بدلاً من `clients/.../APP-TeraQuotation/source/` | لم يتحقق من أن المسار النهائي يقع ضمن Allowed Write Targets قبل الكتابة |
| GAP-002 | TeraAgent فوّض EngineeringAgent بمسار نسبي `src/TeraQuotation/` غير آمن | لم يحدد Allowed Write Targets كمسارات كاملة (absolute) في التفويض |
| GAP-003 | TeraAgent أنشأ ملفات تحضير وتحكم في جذر `project-preparation/` و `project-control/` بدلاً من مسار العميل | لم يطبّق Two-Tier Write System رغم وجود القاعدة في `TERA_RUNTIME_PROTOCOLS.md §3.1` |

**الملخص:** النظام يملك القواعد (Two-Tier System في Protocols §3.1 و tera.md §7)، لكنها غير مُلزمة ولا منفَّذة في 3 نقاط حرجة:
1. تعريف EngineeringAgent لا يحتوي أي شرط للتحقق من المسار قبل الكتابة
2. قالب التفويض (Delegation Protocol) في `TeraSubAgents.md §9` لا يفرض مسارات كاملة
3. TeraAgent ليس لديه آلية تذكير أو Checkpoint لتفعيل Two-Tier Write System عند بداية مشروع عميل

### Evidence

- **GAP-001**: تم إنشاء مجلد `src/` في جذر المنظومة، وملفات .csproj و .cs في غير مكانها الصحيح
- **GAP-002**: نص التفويض كان: `Allowed Write Targets: src/TeraQuotation/` (مسار نسبي بدون جذر)
- **GAP-003**: 8 ملفات تحضير و 3 ملفات تحكم أنشئت في جذر المنظومة وتم نقلها يدوياً لاحقاً
- النظام يملك الحل نظرياً في `TERA_RUNTIME_PROTOCOLS.md §3.1` و `tera.md §7` لكن لا توجد آلية إجبارية

### Affected Files

1. `.opencode/agents/tera.md` — تعزيز قواعد التفويض (إضافة قاعدة صريحة: Allowed Write Targets = مسارات كاملة)
2. `.opencode/agents/engineering-agent.md` — إضافة **Path Validation Gate** (فحص إلزامي قبل كل كتابة)
3. `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` — تقوية §3.1 بإضافة Path Enforcement
4. `tera-system/TeraSubAgents.md` — تحديث §9 (Delegation Protocol) لفرض المسارات الكاملة + إضافة حقل `ClientAppPath`

### Proposed Change

#### التعديل 1 — `.opencode/agents/tera.md` — تعزيز قواعد التفويض

**موقع الإضافة:** §12 (Execution Orchestration Core) — إضافة بند جديد قبل نهاية القسم:

```markdown
### Absolute Path Delegation Rule (قاعدة المسارات الكاملة)

**قاعدة إلزامية عند تفويض أي عميل فرعي بمهمة كتابة ملفات:**

- يجب أن تكون `Allowed Write Targets` في التفويض **مسارات كاملة (Absolute Paths)**، وليست نسبية.
- استثناء واحد: إذا كتبت المسار نسبةً إلى `ClientAppPath` المحدد صراحةً في التفويض، يجب تعريف `ClientAppPath` أولاً.
- قبل كتابة التفويض، راجع §7 (Project Output Location — Two-Tier Write System) لتأكيد المسار الصحيح.

**مثال صحيح:**
```
Allowed Write Targets:
- D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\
```

**مثال ممنوع:**
```
Allowed Write Targets:
- src/TeraQuotation/
```

**التدقيق:** قبل إرسال التفويض، تحقق: "هل Allowed Write Targets مسار كامل وموجود ضمن clients/.../applications/APP-*/source/ أو المسار المحدد للمشروع؟"
```

#### التعديل 2 — `tera.md` — إضافة Checkpoint لبداية مشروع عميل

**موقع الإضافة:** بعد §7 مباشرة أو في §12 — إضافة تذكير:

```markdown
### Client Project Path Checkpoint (إلزامي عند بداية مشروع عميل خارجي)

عند بدء مشروع عميل خارجي، قبل أي تفويض:
1. اقرأ `clients/README.md` للهيكل المطلوب
2. تحقق من وجود مجلد العميل تحت `clients/CLIENT-*/applications/APP-*/`
3. حدد `ClientAppPath` = المسار الكامل لمجلد التطبيق
4. أي ملف تحضير أو تحكم → داخل `ClientAppPath/project-preparation/` أو `ClientAppPath/project-control/`
5. أي ملف كود → داخل `ClientAppPath/source/` أو المسار المتفق عليه في الخطة
6. سجّل `ClientAppPath` في `PROJECT_STATE.md` للمرجعية
```

#### التعديل 3 — `.opencode/agents/engineering-agent.md` — إضافة Path Validation Gate

**موقع الإضافة:** بعد §10 (صلاحياتي) أو في بداية §4 — إضافة قسم جديد:

```markdown
## 10.1 Path Validation Gate — بوابة التحقق من المسار (قاعدة إلزامية)

**قبل كتابة أو إنشاء أي ملف، يجب تنفيذ هذا الفحص:**

```text
Path Validation Gate:
1. المسار المستهدف = المسار الذي سأكتب فيه الملف
2. هل المسار المسموح (Allowed Write Targets) محدد في التفويض؟
   - لا → STOP. أطلب من TeraAgent توضيح Allowed Write Targets
3. هل المسار النهائي Fully Resolved Path (وليس نسبياً)؟
   - نسبي → أحلّه إلى مسار كامل نسبةً إلى Workspace Root
4. هل المسار النهائي يبدأ بـ Allowed Write Targets المحدد في التفويض؟
   - نعم → أكمل
   - لا → STOP. أبلغ TeraAgent أن المسار خارج النطاق المسموح
5. هل المسار النهائي خارج مجلدات النظام المحمية (tera-system/, .opencode/, project-control/ الجذر, project-preparation/ الجذر)؟
   - خارج → أحتاج تأكيداً إضافياً قبل الكتابة
```

**أمثلة:**
| الموقف | المسار المستلم | المسار النهائي | الفحص | الإجراء |
|--------|---------------|----------------|------|--------|
| تفويض صحيح | `clients/.../APP-TeraQuotation/source/` | `D:\...\clients\...\APP-TeraQuotation\source\Models\Invoice.cs` | ✅ يبدأ بـ Allowed Write Targets | أكتب |
| مسار نسبي | `src/TeraQuotation/` | بعد الحل: `D:\...\TeraSystem\src\TeraQuotation\` | ❌ خارج clients/ | STOP — أطلب توضيحاً |
| كتابة في جذر النظام | `project-control/` | `D:\...\TeraSystem\project-control\` | ⚠️ مجلد قالب نظامي | أتأكد: هل هذا لمشروع عميل؟ |

**القاعدة الذهبية:**
```
When in doubt about the path → STOP AND ASK.
Do not assume. Do not guess. Do not write outside Allowed Write Targets.
```
```

#### التعديل 4 — `tera-system/TeraSubAgents.md` — تحديث §9 (Delegation Protocol)

**موقع التعديل:** §9 (بروتوكول تفويض المهمة) — إضافة قواعد للمسارات:

```markdown
### قاعدة إلزامية — المسارات في التفويض

- **Allowed Write Targets** يجب أن تكون **مسارات كاملة (Absolute Paths)** أو مسارات نسبةً إلى `ClientAppPath` (يُعرَّف في التفويض).
- حقل إضافي إلزامي لمشاريع العملاء الخارجيين:
  ```text
  ClientAppPath: [المسار الكامل لمجلد التطبيق، مثل: D:\...\clients\CLIENT-XXX\applications\APP-XXX]
  ```
- **ممنوع** استخدام مسارات نسبية بدون `ClientAppPath`.
- العميل الفرعي **ملزم** بالتحقق من أن المسار النهائي يقع داخل `Allowed Write Targets` قبل أي كتابة.

### Checkpoint للعملاء الفرعيين

قبل بدء أي مهمة كتابة، يجب على العميل الفرعي تأكيد:
```text
[Path Check] Allowed Write Targets are absolute or relative to ClientAppPath: ✅
[Path Check] Final target path starts with Allowed Write Targets: ✅
[Path Check] Not writing to system template folders (root project-preparation/ or project-control/): ✅
```
```

#### التعديل 5 — `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` — تقوية §3.1

**موقع التعديل:** §3.1 (Write Location Protocol) — إضافة قاعدة إنفاذ:

بعد قائمة "When starting a client project:" (الأسطر 194-198)، أضف:

```markdown
### Path Enforcement Rule (قاعدة إنفاذ المسار)

- **TeraAgent مسؤول** عن ضبط `ClientAppPath` و `Allowed Write Targets` كمسارات كاملة قبل أي تفويض.
- **EngineeringAgent و UI Designer** ملزمان بتشغيل Path Validation Gate (بوابة التحقق من المسار) قبل أي كتابة.
- **مخالفة المسار = فشل المهمة.** إذا اكتشف TeraAgent أثناء Post-Execution Review أن أي ملف كُتب خارج Allowed Write Targets، تُصنَّف المخالفة وتُسجَّل في ISSUES_AND_GAPS.md كـ Critical/High حسب درجة الضرر.
- **التكرار:** أول مخالفة → Needs Fix + تسجيل. التكرار → إيقاف التفويض مؤقتاً وإعلام Majed.
```

### Why This Is Necessary

1. **الجذر وليس الأعراض:** الفجوات الثلاث سببها واحد — لا يوجد فحص إلزامي للمسار. إصلاح كل فجوة وحدها يعالج العَرَض فقط.
2. **المنظومة تملك القواعد النظرية لكن تفتقد الإنفاذ:** Two-Tier Write System موجود في `TERA_RUNTIME_PROTOCOLS.md` لكنه غير مفعَّل في سلوك العملاء.
3. **منع التلوث:** كتابة ملفات العملاء في جذر المنظومة يلوث Git history ويجعل الفصل بين المشاريع مستحيلاً مع نمو العملاء.
4. **حماية النظام:** كتابة كود في `tera-system/` أو `.opencode/agents/` قد يكسر المنظومة نفسها.
5. **استمرارية الأعمال:** مع تعدد العملاء (YAEE, Noor، وقريباً Armada و Albalqa)، تزايد خطر اختلاط المسارات يصبح حرجاً.

### Rejected Alternatives

1. **إصلاح كل فجوة على حدة (3 SCPs منفصلة)** — مرفوض: تكلفة تنفيذ أعلى بدون فائدة إضافية. الفجوات الثلاث مترابطة ويجب معالجتها في تمريرة واحدة.
2. **إضافة عميل فرعي جديد (PathGuardAgent)** — مرفوض: Anti-Bloat. يمكن إضافة الفحص لـ EngineeringAgent و TeraAgent مباشرة.
3. **إضافة ملف جديد (PathGovernance.md)** — مرفوض: Anti-Bloat. القواعد موجودة في Protocols.md وتحتاج تقوية فقط.
4. **إضافة Gate جديد (PathGate)** — مرفوض: البوابات الحالية (Pre-Execution, Post-Execution) كافية. الفحص يكون قبل الكتابة وليس Gate مستقلاً.
5. **إصلاح EngineeringAgent فقط وترك TeraAgent** — مرفوض: GAP-002 و GAP-003 من TeraAgent نفسه. EngineeringAgent يحمي نفسه لكنه لا يحمي المنظومة من أخطاء TeraAgent.

### Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | كتابة الملفات في مسارات خاطئة — تلويث جذر المنظومة، اختلاط Git history، عدم قابلية التوسع |
| لماذا لا يكفي تعديل ملف موجود؟ | نعدّل 4 ملفات موجودة — لا ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | العملاء أنفسهم هم المستهدفون — TeraAgent و EngineeringAgent |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — تمنع أخطاء المسار قبل حدوثها بدلاً من تنظيف الفوضى بعدها |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | ضئيل — التعديلات صغيرة وموجهة (بنود فحص سريع) |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | تم التصغير: لا عملاء جدد، لا بوابات جديدة، لا ملفات جديدة — فحص في 3-4 أسطر لكل ملف |

### Risk

- **منخفض** — التعديلات تضيف فحصاً وتذكيراً، لا تغير صلاحيات أو سياسات قائمة
- **خطر رفض العميل للمهمة بسبب المسار** — مقصود: من الأفضل الرفض المبكر عن الكتابة الخاطئة
- **خطر إبطاء التفويض** — ضئيل: الفحص يستغرق ثوانٍ ويمنع ساعات من التنظيف

### Rollback Plan

1. `tera.md`: إزالة Absolute Path Delegation Rule + Client Project Path Checkpoint
2. `engineering-agent.md`: إزالة §10.1 Path Validation Gate بكامله
3. `TeraSubAgents.md`: استرجاع §9 Delegation Protocol إلى النسخة السابقة
4. `TERA_RUNTIME_PROTOCOLS.md`: إزالة Path Enforcement Rule من §3.1
5. `AGENT_GAPS_LOG.md`: إعادة GAP-001, GAP-002, GAP-003 إلى Pending

### Approval Required

- ✅ Majed
