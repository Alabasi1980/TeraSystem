# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-06-073 — حل التعارض الدائري في B.7 (تقسيم إلى B.7a + B.7b)

---

### Request Type
System Bug / Policy Conflict Resolution

### Problem
تعارض دائري (Circular Dependency) في بوابة Tera Handoff Readiness Gate (B.7):

1. **الملف الرئيسي** (السطر 161): ينص على التحقق من B.7 **قبل** إنتاج `TERA_HANDOFF_PACKAGE.md`
2. **ملف البوابات** (gates.md السطر 105): `TERA_HANDOFF_PACKAGE.md` هو **مدخل مطلوب** لفحص B.7
3. **ملف البوابات** (gates.md السطر 108): عند PASS: `TERA_HANDOFF_PACKAGE.md` جاهز للتسليم

```
الدائرة المنطقية:
  → لا تنتج الحزمة قبل B.7 (الملف الرئيسي 161)
  → لكن لا تستطيع فحص B.7 بدون الحزمة (gates.md 105)
  → تناقض ← نموذج ضعيف يختار مساراً خاطئاً أو يتوقف
```

### Evidence
- `.opencode/agents/tera-client-engagement.md:161`: `← التحقق من Tera Handoff Readiness Gate (B.7) ... قبل TERA_HANDOFF_PACKAGE.md`
- `tera-system/client-helpers/tera-client-engagement-gates.md:105`: `المدخلات المطلوبة | TERA_HANDOFF_PACKAGE.md, ...`
- `tera-system/client-helpers/tera-client-engagement-gates.md:108`: `عند PASS: TERA_HANDOFF_PACKAGE.md جاهز للتسليم`
- `tera-system/client-helpers/tera-client-engagement-gates.md:106`: الشروط 9-10 تتطلب وجود الحزمة (تحتوي على وثائق، جميع العناصر Confirmed)

### Affected Files
1. `tera-system/client-helpers/tera-client-engagement-gates.md` — إعادة هيكلة B.7
2. `.opencode/agents/tera-client-engagement.md` — تحديث المراجع إلى B.7 وتدفق العمل

### Proposed Change

#### 1. تقسيم B.7 إلى بوابتين منفصلتين

**B.7a — Handoff Draft Readiness Gate (بوابة جاهزية مسودة الهاندوف)**
- **التوقيت:** **قبل** إنتاج `TERA_HANDOFF_PACKAGE.md`
- **الهدف:** التأكد من أن جميع المتطلبات المسبقة للهاندوف قد اكتملت قبل صياغة الحزمة
- **المدخلات:** `CLIENT_INTAKE.md`, `SCOPE_SUMMARY.md`, `FEATURE_LIST.md`, `DRAFT_QUOTATION.md`, `CLIENT_DECISION_LOG.md`, `CHANGE_REQUEST_LOG.md`, `CLIENT_BRIEF.md`
- **شروط النجاح:** (الشروط السابقة 1-8 من B.7 القديم — لا تشترط وجود الحزمة)
  1. Approval Consistency Check = PASS (B.6)
  2. Quotation Readiness Gate = PASS (B.4)
  3. Final Scope Reconciliation = PASS (B.3)
  4. Budget-to-Scope documented (B.2)
  5. CLIENT_DECISION_LOG.md: صفر `Pending Approval` ← **MR3**
  6. Quotation معتمد من Majed (Level 2 Approved)
  7. جميع CHANGE_REQUEST_LOG.md محسومة
  8. Workspace structure `clients/CLIENT-*/applications/APP-*/client-engagement/` جاهز
- **شروط الإيقاف:**
  1. Approval Consistency = FAIL ← توقف
  2. أي قرار `Pending Approval` ← توقف ← **MR3**
  3. Level 2 Quotation غير معتمد من Majed ← توقف
  4. CHANGE_REQUEST غير محسوم ← توقف
  5. Workspace structure غير جاهز ← توقف
- **الإخراج:** PASS/FAIL مع قائمة الفجوات المانعة
- **عند PASS:** يُسمح بإنتاج `TERA_HANDOFF_PACKAGE.md` كمسودة أولية
- **هل يمنع الانتقال؟** نعم — لا يمكن إنتاج الحزمة قبل PASS

**B.7b — Final Handoff Package Gate (بوابة الحزمة النهائية للهاندوف)**
- **التوقيت:** **بعد** إنتاج `TERA_HANDOFF_PACKAGE.md` (كمسودة)
- **الهدف:** التأكد من أن حزمة الهاندوف نفسها مكتملة ومتسقة وجاهزة للتسليم
- **المدخلات:** `TERA_HANDOFF_PACKAGE.md`, `CLIENT_INTAKE.md`, `SCOPE_SUMMARY.md`, `FEATURE_LIST.md`, `DRAFT_QUOTATION.md`, `CLIENT_DECISION_LOG.md`, `CHANGE_REQUEST_LOG.md`, هيكل مساحة العمل
- **شروط النجاح:** (الشروط 9-10 من B.7 القديم + بقية الشروط المتعلقة بالحزمة)
  1. `TERA_HANDOFF_PACKAGE.md` يحتوي على جميع الوثائق الأساسية: CLIENT_BRIEF أو SCOPE_SUMMARY + FEATURE_LIST + DRAFT_QUOTATION + CLIENT_DECISION_LOG + CHANGE_REQUEST_LOG
  2. جميع العناصر في حزمة الهاندوف موسومة بـ `[Confirmed by Majed]` — لا `[Research Hint]` ولا `[Assumption]` ولا `[Unresolved]` ← **MR1**
  3. الحزمة متسقة مع المصادر (CLIENT_INTAKE.md, SCOPE_SUMMARY.md, FEATURE_LIST.md, DRAFT_QUOTATION.md)
  4. جميع CLIENT_DECISION_LOG.md مدخلاتها منعكسة في الحزمة
- **شروط الإيقاف:**
  1. أي وثيقة أساسية ناقصة من TERA_HANDOFF_PACKAGE.md ← توقف
  2. أي عنصر موسوم بـ `[Research Hint]` أو `[Assumption]` أو `[Unresolved]` داخل الحزمة ← توقف
  3. عدم اتساق بين الحزمة والمصادر ← توقف
- **الإخراج:** PASS/FAIL + قائمة الفجوات المانعة عند FAIL
- **عند PASS:** `TERA_HANDOFF_PACKAGE.md` جاهز للتسليم إلى ApplicationBlueprintAgent
- **هل يمنع الانتقال؟** نعم — لا يمكن تسليم الحزمة قبل PASS

#### 2. تحديث المراجع في الملف الرئيسي

تحديث كل مرجع لـ B.7 في `.opencode/agents/tera-client-engagement.md`:
- السطر 44: `B.1–B.7` → `B.1–B.7b`
- السطر 48: `B.6/B.7` → `B.6/B.7a/B.7b`
- السطر 126: `الهاندوف (B.7)` → `الهاندوف (B.7a/B.7b)`
- السطر 127: `الهاندوف (B.7)` → `الهاندوف (B.7a/B.7b)`
- السطر 128: `B.7 Tera Handoff Readiness` → `B.7a Handoff Draft Readiness + B.7b Final Handoff Package`
- السطر 132: `B.1-B.7` → `B.1-B.7b`
- السطر 161: تعديل التدفق إلى: `B.7a → TERA_HANDOFF_PACKAGE.md (مسودة) → B.7b`
- السطر 364, 366: `B.1–B.7` → `B.1–B.7b`
- السطر 380: تحديث وصف B.7 → B.7a/B.7b
- السطر 423: `B.1–B.7` → `B.1–B.7b`
- السطر 564: تحديث مرجع B.7

#### 3. تحديث تدفق العمل (A.4، السطر 161)

من:
```
← التحقق من Tera Handoff Readiness Gate (B.7) (يشمل Approval Consistency Check B.6) قبل TERA_HANDOFF_PACKAGE.md
← بعد الاعتماد: إنتاج TERA_HANDOFF_PACKAGE.md ← Majed يراجع
```

إلى:
```
← التحقق من Handoff Draft Readiness Gate (B.7a) (يشمل Approval Consistency Check B.6) قبل إنتاج TERA_HANDOFF_PACKAGE.md
← عند PASS B.7a: إنتاج TERA_HANDOFF_PACKAGE.md (مسودة أولية) ← Majed يراجع
← التحقق من Final Handoff Package Gate (B.7b) بعد اكتمال مسودة الحزمة
← عند PASS B.7b: الحزمة جاهزة للتسليم إلى ApplicationBlueprintAgent
```

### Why This Is Necessary
- **يزيل التناقض المنطقي:** لم يعد هناك سؤال "هل أفحص البوابة قبل الحزمة أم بعدها؟"
- **يحدد توقيتاً واضحاً لكل بوابة:** B.7a قبل الصياغة، B.7b بعد الصياغة
- **يمنع التوقف التام:** النموذج الضعيف لن يتوقف بسبب دائرة منطقية غير قابلة للحل
- **يحافظ على جميع الضوابط:** لم تفقد أي خانة من شروط B.7 القديمة — فقط أُعيد توزيعها بمنطق أوضح

### Rejected Alternatives
1. **إبقاء B.7 كما هو مع تعديل التدفق فقط (حذف "قبل TERA_HANDOFF_PACKAGE.md")**: هذا يخفي المشكلة بدلاً من حلها — سيبقى الشرط "المدخلات المطلوبة: TERA_HANDOFF_PACKAGE.md" مقابل "عند PASS: الحزمة جاهزة" غامضاً.
2. **إزالة TERA_HANDOFF_PACKAGE.md من المدخلات**: هذا يجعل B.7 يتجاهل جودة الحزمة نفسها — ثغرة حوكمة.
3. **جعل B.7 يتحقق من الحزمة فقط (إزالة الشروط 1-8)**: هذا يفقد الضوابط المسبقة للهاندوف — تضعيف للحوكمة.

### Anti-Bloat Check
| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | تعارض دائري يمنع التدفق المنطقي للهاندوف |
| لماذا لا يكفي تعديل ملف موجود؟ | البوابة الحالية تحاول فعل شيئين في وقتين مختلفين — الحل هو الفصل وليس التعديل التجميلي |
| لماذا لا يكفي عميل موجود؟ | هذه مشكلة هيكلية في تعريف البوابة، ليست مشكلة في عميل |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — تزيل دائرة منطقية وتوضح التسلسل الزمني |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | طفيف — إضافة بوابة واحدة (B.7a + B.7b مجتمعة تساوي تقريباً حجم B.7 القديم) |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — الفصل إلى بوابتين هو الحل الوحيد لكسر الدائرة دون فقدان أي شرط |

### Risk
- **منخفض** — لا تغيير في محتوى الشروط، فقط إعادة توزيع وتوضيح توقيت الفحص
- **مخاطر التطبيق:** نسيان تحديث أحد المراجع في الملف الرئيسي (ستُعالج بالفحص الشامل بعد التعديل)
- **مخاطر التشغيل:** قد يحتاج TCEA لجلسة تدريب قصيرة لفهم الفرق بين B.7a و B.7b

### Rollback Plan
استعادة النسخة الحالية من gates.md (B.7 غير مقسم) والملف الرئيسي (السطر 161 وكل مراجع B.7) من git.

### Approval Required
- [ ] Majed — Approved
- [ ] Majed — Approved with Conditions
- [ ] Majed — Rejected
