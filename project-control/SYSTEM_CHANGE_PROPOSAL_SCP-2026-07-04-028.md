# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-04-028 — TCEA Self-Check + Uncertainty Protocol + Monitor Discovery Audit

**Request Type:** Agent Process Improvement / Policy Gap Closure

---

### Problem

TCEA شخصياً اعترف بوجود 3 فجوات هيكلية تمنعه من العمل عند 95%+ باستمرار:

1. **لا صلاحية صريحة ليقول "لا أعرف"** — يخمن بدلاً من أن يتوقف.
2. **لا بروتوكول داخلي يمنعه من الاستعجال** — الـ Gates الحالية خارجية وتعتمد على Majed لاكتشاف النقص.
3. **لا مراجعة خارجية غير متوقعة للـ Discovery** — Monitor يراجع Compliance Records فقط.

تم التحقق من هذه الفجوات بمراجعة ملفات النظام (انظر Evidence).

---

### Evidence

1. ملف `TeraClientEngagement.md`:
   - §3.2.3: يطلب توثيق `افتراض مؤقت` للمجالات غير المكتملة، لكن لا يوجد بند يسمح بـ "لا أعرف — أتوقف".
   - §3.2.4-§3.2.5-§3.8.1-§3.6.1: كلها Gates خارجية (بموافقة Majed)، لا Self-Check داخلي.
   - §8: Websearch Protocol مربوط بمرحلة زمنية (بعد الحوار الأولي)، وليس بحالة عدم التأكد.
   - لا يوجد أي §3.2.6 أو §3.2.7 أو أي بروتوكول شك.

2. ملف `TERA_RUNTIME_PROTOCOLS.md` §The No-Guessing Rule (سطور 752-771):
   - مكتوب لـ TeraAgent تحديداً، وليس لـ TCEA.
   - لا ينطبق systemically على TCEA.

3. ملف `monitor.md`:
   - يراجع Compliance Records و Handback vs Git diff فقط.
   - لا توجد أي مسؤولية لمراجعة Discovery Coverage Summary.

---

### Affected Files

| File | Change |
|------|--------|
| `tera-system/TeraClientEngagement.md` | إضافة §3.2.6 Self-Check Protocol + §3.2.7 Uncertainty Protocol |
| `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` §35 | إضافة 3 أعمدة Self-Check إلى Domain Coverage Matrix |
| `.opencode/agents/monitor.md` | إضافة مسؤولية مراجعة Discovery العشوائية |

---

### Proposed Change

#### 1. §3.2.6 Self-Check Protocol (TeraClientEngagement.md)

بعد §3.2.5، يُضاف قسم إلزامي جديد:

```text
### 3.2.6 Self-Check Protocol (إلزامي — لكل Domain)

قبل أن يضع TCEA أي Domain كـ `Complete`، يجب أن يجيب على الأسئلة الثلاثة التالية ويوثقها في `DISCOVERY_COVERAGE_SUMMARY.md`:

1. **ما مصدر هذه المعلومة؟**
   - `Majed (صراحة)` / `Websearch` / `Inference (استنتاج)` / `Unknown (غير معروف)`
2. **هل أكدها Majed صراحة؟**
   - `Yes` / `No` / `Partially`
3. **ما الخطورة لو كانت هذه المعلومة خاطئة؟**
   - `Low` / `Medium` / `High`

القاعدة: إذا كان المصدر = `Inference` أو `Unknown` والخطورة = `High`، لا يجوز وضع `Complete`. يجب أن يكون `Partial` مع `Uncertainty Notice` والتوقف لطلب تأكيد من Majed.
```

#### 2. إضافة 3 أعمدة لجدول §35 في TERA_RUNTIME_TEMPLATES.md

جدول Domain Coverage Matrix الحالي (13 صفاً) يُضاف له 3 أعمدة:

| Source of Info | Confirmed by Majed? | Risk if Wrong |
|---|---|---|
| Majed / Websearch / Inference / Unknown | Yes / No / Partial | L / M / H |

#### 3. §3.2.7 Uncertainty Protocol (TeraClientEngagement.md)

```text
### 3.2.7 Uncertainty Protocol — صلاحية "لا أعرف" الإلزامية (جديد)

TCEA لديه صلاحية — بل واجب — أن يقول "لا أعرف" في الحالات التالية:

1. **مصدر المعلومة غير مؤكد** (Inference / Unknown) مع خطورة High ← توقف إجباري
2. **معلومة خارج تاريخ تدريبه** (أحدث من 2025) ← يجب البحث قبل الافتراض
3. **طلب عميل غير مألوف تماماً** ← يوقف التخمين ويطلب توجيهاً

الآلية:
- إذا تحقق شرط التوقف: يكتب `UNCERTAINTY_NOTICE` داخل `DISCOVERY_COVERAGE_SUMMARY.md`
- يرفع لـ Majed صراحة: "هذه المعلومة غير مؤكدة — لا أستطيع المتابعة بدون تأكيد"
- لا يستمر في Domain التالي حتى يحصل على رد

**استخدام Websearch في حالة عدم التأكد:**
إذا كان المصدر = `Inference` أو `Unknown` (ولو بمخاطر Medium)،
يمكن لـ TCEA استخدام Websearch فوراً لتقليل عدم التأكد قبل رفع الـ Uncertainty Notice.
لا يحتاج انتظار موافقة منفصلة — Websearch متاح دائماً عند عدم التأكد.
```

#### 4. Monitor — إضافة مراجعة Discovery عشوائية (monitor.md)

إضافة فقرة إلى قسم `## What you do` (بعد السطر 63):

```text
- **Random Discovery Audit (اختياري — بأمر Majed):** When Majed requests, review
  the application's `client-engagement/DISCOVERY_COVERAGE_SUMMARY.md` for:
  - Completeness of the 13-domain matrix
  - Self-Check Protocol answers (source, confirmation, risk)
  - Consistency between domain status and actual documentation
  - Flag any domain marked `Complete` but sourced from `Inference`/`Unknown` with High risk
  Report findings to Majed. This audit is triggered explicitly by Majed, not automatic.
```

---

### Why This Is Necessary

1. **يسد الفجوة المعترف بها من TCEA نفسه** — ليس افتراضاً مني، بل اعتراف من العميل المعني.
2. **يكمل الـ 3 Gates الخارجية بـ Self-Check داخلي** — الـ Gates تمنع الانتقال دون موافقتك، والـ Self-Check يمنع TCEA من التخمين أصلاً.
3. **يكمل 3-layer defense** — Mid-Task (لـ TeraAgent) → Self-Check (لـ TCEA) → Monitor audit (للجميع).
4. **Websearch يصبح reactive (عند عدم التأكد) وليس proactive فقط (بعد الحوار الأولي)** — هذا يزيد الدقة بشكل كبير.

---

### Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| إنشاء AuditAgent جديد | انتفاخ غير مبرر — Monitor الموجود يمكن تمديده ببساطة |
| إنشاء ملف UNCERTAINTY_LOG.md منفصل | يزيد عدد الملفات دون داع — `DISCOVERY_COVERAGE_SUMMARY.md` موجود أصلاً |
| Self-Check خارج الملف (في المحادثة فقط) | غير موثق — يضيع في تاريخ المحادثة |
| جعل Monitor audit تلقائياً لكل مشروع | استهلاك غير ضروري للجلسات — Audit بأمر Majed فقط |

---

### Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | 3 فجوات هيكلية تمنع TCEA من العمل بدقة 95%+ |
| لماذا لا يكفي تعديل ملف موجود؟ | التعديل في ملفات موجودة أصلاً (TCEA, TEMPLATES, Monitor) — لا ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | Monitor موجود وسيُمدّد — لا عميل جديد |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | تقلل التعقيد — تمنع التخمين الذي يسبب fix لاحقاً |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | لا — إضافات صغيرة في ملفات موجودة |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — 3 أعمدة في جدول + فقرتين في TCEA + فقرة في Monitor |

---

### Risk

| المخاطرة | الاحتمال | التأثير | التخفيف |
|-----------|----------|---------|---------|
| TCEA يفرط في استخدام "لا أعرف" | منخفض — لأنه يريد إنهاء المشاريع | تأخير طفيف | الـ Uncertainty Protocol يتطلب سبباً واضحاً |
| Self-Check يبطئ Discovery | منخفض — 3 أسئلة لكل Domain | ~دقيقة إضافية لكل Domain | مقبول جداً مقارنة بخطأ يكتشف متأخراً |
| Monitor audit يتطلب جلسة إضافية | منخفض — بأمر Majed فقط | حسب الرغبة | Audit ليس تلقائياً |

---

### Rollback Plan

1. إزالة §3.2.6 و §3.2.7 من `TeraClientEngagement.md`.
2. إزالة الأعمدة 3 الجديدة من جدول §35 في `TERA_RUNTIME_TEMPLATES.md`.
3. إزالة فقرة Random Discovery Audit من `monitor.md`.
4. تحديث `SYSTEM_EVOLUTION_LOG.md` و `AGENT_GAPS_LOG.md`.

---

### Approval Required

- [x] Majed — موافقة مبدئية عبر المحادثة ("نعم قم بالانشاء")
- [ ] تأكيد نهائي بعد قراءة الـ Proposal
