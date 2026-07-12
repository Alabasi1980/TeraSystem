# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-06-074 — حل تعارض إنشاء مساحة العمل (فصل التخطيط عن التنفيذ)

---

### Request Type
System Bug / Policy Conflict Resolution

### Problem
تعارض تسلسلي بين ثلاث نقاط في ملف TCEA:

1. **A.2 (السطر 81):** "set up the workspace **only after** approval and handoff readiness are confirmed"
2. **التدفق (السطر 165):** إنشاء مساحة العمل **بعد** B.7b — صحيح ومنطقي
3. **B.7a الشرط 8 (gates.md السطر 106):** "Workspace structure ... **جاهز**" — أي مساحة العمل يجب أن تكون موجودة **قبل** PASS B.7a

```
الدائرة المنطقية:
  → B.7a تشترط وجود مساحة العمل فعلية (جاهز)
  → لكن التدفق ينشئ مساحة العمل بعد B.7b
  → ولا يجوز إنشاؤها قبل تأكيد الهاندوف (A.2)
  → تناقض: لا يمكن PASS B.7a بدون مساحة عمل، ولا يمكن إنشاء مساحة عمل قبل B.7a
```

### Evidence
- `.opencode/agents/tera-client-engagement.md:81`: "set up the workspace only after approval and handoff readiness are confirmed"
- `.opencode/agents/tera-client-engagement.md:165`: إنشاء مساحة العمل بعد B.7b
- `tera-system/client-helpers/tera-client-engagement-gates.md:106` (B.7a الشرط 8): "Workspace structure `clients/CLIENT-*/applications/APP-*/client-engagement/` جاهز"
- `tera-system/client-helpers/tera-client-engagement-gates.md:107` (B.7a شرط الإيقاف 5): "Workspace structure غير جاهز ← توقف"
- `.opencode/agents/tera-client-engagement.md:113` (Role 7): "بعد اعتماد الهاندوف"

### Affected Files
1. `tera-system/client-helpers/tera-client-engagement-gates.md` — تعديل B.7a الشرط 8 + إضافة Workspace Verification
2. `.opencode/agents/tera-client-engagement.md` — تحديث A.2 (81), التدفق (161-166), الملاحة (380), D.1 (564)

### Proposed Change

#### المبدأ: فصل "Workspace Plan" عن "Workspace Creation"

| المرحلة | ماذا | هل ينفّذ (mkdir/files)؟ | متى |
|---------|------|:-----------------------:|:----:|
| **Workspace Plan** | تحديد المسار، الاسم، الهيكل المتوقع | ❌ لا — تخطيط فقط | **قبل** B.7a |
| **Workspace Creation** | إنشاء المجلدات والمجلدات الفرعية فعلياً | ✅ نعم | **بعد** B.7b + موافقة Majed |
| **Workspace Verification** | التأكد من أن الهيكل أنشئ بشكل صحيح | ❌ لا — فحص فقط | **بعد** الإنشاء |

---

#### 1. تعديل B.7a في gates.md — استبدال الشرط 8

**من:**
```
8. Workspace structure `clients/CLIENT-*/applications/APP-*/client-engagement/` جاهز
```
وشرط الإيقاف:
```
5. Workspace structure غير جاهز ← توقف
```

**إلى:**
```
8. Workspace Plan confirmed by Majed (المسار المتوقع: clients/<CLIENT>/applications/<APP>/client-engagement/، الاسم الرسمي، والهيكل المتوقع)
```
وشرط الإيقاف:
```
5. Workspace Plan غير مؤكد من Majed ← توقف
```

**التأثير:** B.7a لم يعد يشترط وجود مجلدات فعلية — فقط خطة متفق عليها مع Majed. هذا يزيل التعارض الدائري لأن التخطيط (غير تنفيذي) يمكن أن يحدث قبل B.7a دون مشكلة.

---

#### 2. إضافة Workspace Verification في gates.md (بعد B.7b)

إضافة قسم جديد (ليس B.7c — لأنه ليس بوابة انتقال بين Modes، بل فحص بعد التنفيذ):

```markdown
## Workspace Verification — التحقق من مساحة العمل

| البند | التفاصيل |
|-------|----------|
| **التوقيت** | بعد إنشاء مساحة العمل فعلياً (بعد PASS B.7b + موافقة Majed) |
| **الهدف** | التأكد من أن هيكل المجلدات قد أُنشئ بشكل صحيح قبل وضع الملفات |
| **قائمة التحقق** | 1. المجلد الرئيسي `clients/CLIENT-NAME/` موجود<br>2. المجلد `applications/APP-NAME/` موجود<br>3. المجلد `client-engagement/` موجود داخل APPLICATION<br>4. جميع المسارات تطابق Workspace Plan المعتمد |
| **عند الفشل** | أعلم Majed وأعد إنشاء الهيكل المفقود — لا يمنع التسليم |
```

هذا ليس بوابة مستقلة — إنه فحص خفيف بعد الإنشاء. لا يمنع الانتقال، ولا يحتاج PASS/FAIL رسمي.

---

#### 3. تحديث تدفق العمل (A.4، الأسطر 161-166)

**من:**
```
← التحقق من Handoff Draft Readiness Gate (B.7a) (يشمل Approval Consistency Check B.6) قبل إنتاج TERA_HANDOFF_PACKAGE.md
← عند PASS B.7a: إنتاج TERA_HANDOFF_PACKAGE.md (مسودة أولية) ← Majed يراجع
← التحقق من Final Handoff Package Gate (B.7b) بعد اكتمال مسودة الحزمة
← عند PASS B.7b: الحزمة جاهزة للتسليم إلى ApplicationBlueprintAgent
← إنشاء مساحة العمل: clients/CLIENT-*/applications/APP-*/ مع المجلدات الفرعية
← وضع TERA_HANDOFF_PACKAGE.md + DRAFT_QUOTATION.md داخل client-engagement/
```

**إلى:**
```
← تأكيد Workspace Plan مع Majed (المسار، الاسم، الهيكل المتوقع — تخطيط فقط، لا إنشاء)
← التحقق من Handoff Draft Readiness Gate (B.7a) (يشمل Approval Consistency Check B.6) + Workspace Plan مؤكد
← عند PASS B.7a: إنتاج TERA_HANDOFF_PACKAGE.md (مسودة أولية) ← Majed يراجع
← التحقق من Final Handoff Package Gate (B.7b) بعد اكتمال مسودة الحزمة
← عند PASS B.7b + موافقة Majed: إنشاء مساحة العمل clients/CLIENT-*/applications/APP-*/client-engagement/ فعلياً
← Workspace Verification — التحقق من أن الهيكل أُنشئ بشكل صحيح
← وضع TERA_HANDOFF_PACKAGE.md + DRAFT_QUOTATION.md داخل client-engagement/
← تسليم مساحة العمل الجاهزة + الحزمة إلى ApplicationBlueprintAgent عبر Majed
```

---

#### 4. تحديث A.2 (السطر 81)

**من:**
```
set up the workspace only after approval and handoff readiness are confirmed
```

**إلى:**
```
plan the workspace (path, name, structure) before B.7a, but create it only after B.7b PASS and Majed approval
```

---

#### 5. تحديث الجدول الملاحة (B.7a السطر) في الملف الرئيسي

إضافة إشارة أن B.7a يتضمن شرط Workspace Plan.

---

### Why This Is Necessary
- **يزيل الدائرة المنطقية الثانية** بعد إصلاح B.7a/B.7b
- **يفصل بوضوح بين التخطيط والتنفيذ**: التخطيط (غير ضار) قبل البوابة، والتنفيذ (mkdir) بعد الاعتماد
- **يمنع إنشاء مجلدات مبكراً**: لا يتم إنشاء أي شيء على القرص قبل PASS B.7b + موافقة Majed
- **يحافظ على مبدأ A.2**: مساحة العمل تبقى "بعد تأكيد الهاندوف"

### Rejected Alternatives
1. **إزالة الشرط 8 من B.7a بالكامل**: هذا يفقد التحقق من أن مسار الـ workspace معروف ومتفق عليه قبل إنتاج الحزمة — الحزمة تحتاج تعرف أين ستوضع
2. **نقل الشرط 8 إلى B.7b**: B.7b تتحقق من الحزمة نفسها، وليس من الهيكل — غير مناسب
3. **إنشاء مساحة العمل قبل B.7a (حل تدفق بديل)**: هذا يخالف A.2 الذي يشترط "بعد تأكيد الهاندوف"

### Anti-Bloat Check
| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | تعارض تسلسلي: B.7a تطلب مساحة عمل جاهزة قبل أن يُسمح بإنشائها |
| لماذا لا يكفي تعديل ملف موجود؟ | سنعدل ملفين موجودين (gates.md + main) — لا ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | مشكلة في تعريف البوابات والتدفق، وليس في عميل |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — تفصل التخطيط عن التنفيذ وتزيل تناقضاً |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | الحد الأدنى — Workspace Verification فقرة قصيرة |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — الحل هو الفصل بين التخطيط والتنفيذ |

### Risk
- **منخفض** — لا تغيير في شروط الحوكمة، فقط توضيح توقيت "التخطيط" مقابل "التنفيذ"
- **مخاطر التطبيق:** قد يخلط TCEA بين Workspace Plan و Workspace Creation (عالجها بتسمية واضحة في التدفق)
- **Workspace Verification:** فتح الملفات للتحقق من وجودها (read-only) — لا خطر تنفيذياً

### Rollback Plan
استعادة gates.md (الشرط 8 في B.7a) والملف الرئيسي (A.2، التدفق، الملاحة) من git.

### Approval Required
- [ ] Majed — Approved
- [ ] Majed — Approved with Conditions
- [ ] Majed — Rejected
