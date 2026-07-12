# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-04-021

**Title:** TCEA Mandatory 13-Domain Client Discovery Framework — إضافة نظام اكتشاف إلزامي ومنظم لـ TeraClientEngagementAgent

**Request Type:** Process Evolution / Governance Enhancement / Consulting Capability Upgrade

**Date:** 2026-07-04

**Proposed by:** TeraClientEngagementAgent (self-gap analysis after first client trial)

---

## 1. Executive Summary

بعد اختبار TCEA على أول عميل حقيقي (Alfares Maintenance Foundation)، ظهر ضعف جوهري في دور TCEA كاستشاري اكتشاف. كان الأداء مقبولاً في جمع المعلومات الأساسية (المشكلة، المستخدمين، الحالات، النطاق، السعر)، لكنه فشل في تغطية مجالات حاسمة مثل التصميم، المحتوى، التقنية، الأمان، ومعايير القبول.

السبب الجذري ليس نقص سؤال واحد، بل **غياب بوابة تشغيلية إلزامية** تمنع TCEA من الانتقال إلى النطاق/السعر/الهاندوف قبل تغطية Discovery كامل ومنظم.

هذا المقترح يضيف `TCEA Mandatory 13-Domain Client Discovery Framework` كنظام إلزامي، مع بوابات اكتشاف وتسعير وتسليم، مع الحفاظ على مبدأ: `Fixed mandatory framework + Flexible execution inside the framework`.

---

## 2. Current Problem

### 2.1 التشخيص

| المشكلة | التفاصيل | الدليل |
|---------|----------|--------|
| **تغطية Discovery غير مكتملة** | TCEA ركز على Business/Users/Workflow/Pricing وأهمل Design، Data، Technical، Security، Acceptance | ملفات العميل الحالي لا تحتوي معلومات عن التصميم، الهوية، المحتوى، التقنية، الأمان |
| **الانتقال السريع للنطاق** | انتقل TCEA إلى CLIENT_BRIEF.md و SCOPE_SUMMARY.md قبل تأكيد فهم كامل لكل المجالات | GAP-001 سُجل ثم عُولج جزئياً، لكن لا يزال هناك نقص في المجالات الأخرى |
| **غياب الـ Consulting Mindset** | TCEA تصرف كجامع معلومات وليس كمستشار اكتشاف يغطي كل جوانب التطبيق | لم يُطرح أي سؤال عن التصميم، الـ UX، الهوية البصرية، خيارات الاستضافة، إلخ |
| **لا يوجد معيار لاكتمال Discovery** | لا توجد Matrix تحدد أي المجالات اكتملت وأيها ناقصة وأيها غير applicable | لا يمكن حالياً تحديد "جاهزية" Discovery بشكل موضوعي |
| **Impact على ApplicationBlueprintAgent** | عدم اكتمال معلومات التصميم والبيانات والتقنية يجعل عمل ApplicationBlueprintAgent تخمينياً | الـ Blueprint سيُبنى على فراغات في مجالات حساسة |

### 2.2 أثر المشكلة

1. **على المبرمجين (TeraAgent):** يستلمون حزمة handoff ناقصة — يضطرون للتخمين أو طلب توضيح إضافي.
2. **على الإدارة (Majed):** يضطر للتواصل مع العميل عدة مرات إضافية لسد الفجوات.
3. **على العميل:** قد يستلم Proposal غير دقيق في جوانب التصميم والتقنية.
4. **على السمعة:** أول عميل — أي نقص سيؤثر على ثقة العميل والمصداقية.

---

## 3. Evidence From Trial

| المجال | الحالة في التطبيق الحالي | المشكلة |
|--------|-------------------------|---------|
| Business Context | ✅ موجود | OK |
| Users & Roles | ✅ موجود | OK |
| Workflow | ✅ موجود | OK |
| Design & Branding | ❌ **غائب تماماً** | لم يُطرح أي سؤال — لا شعار، لا ألوان، لا ستايل، لا مراجع |
| Data & Content | ⚠️ جزئي | وُجدت الحقول لكن بدون تفصيل، ولا أسئلة عن القوائم الثابتة أو استيراد البيانات |
| Screens & UX | ⚠️ جزئي | وُجدت الشاشات الرئيسية لكن بدون تفصيل عن كل شاشة ومحتواها |
| Technical & Hosting | ❌ **غائب** | لم يُطرح أي سؤال عن الاستضافة أو التقنية |
| Security & Audit | ❌ **غائب** | لم يُطرح أي سؤال |
| Notifications | ✅ موجود | لكن محدود (داخلي فقط) |
| Reports & Dashboards | ✅ موجود | لكن بدون تفصيل المعادلات والتعريفات |
| Acceptance Criteria | ❌ **غائب** | لم يُطرح أي سؤال عن معايير القبول |
| Integrations | ✅ موجود | لكن مؤجل لـ Phase 2 |
| Commercial | ✅ موجود | السعر موجود لكن بدون تفصيل الميزانية من العميل |

---

## 4. Root Cause

```text
لا توجد آلية تشغيلية صارمة تمنع TCEA من الانتقال
إلى النطاق/السعر/الهاندوف قبل إكمال Discovery Coverage كامل.

Understanding Confirmation Gate (الحالية) تتحقق فقط من:
"هل فهمت الفكرة العامة بشكل صحيح؟"

لكن لا تتحقق من:
"هل غطيت كل المجالات المطلوبة قبل النطاق والسعر؟"
```

---

## 5. Proposed Change

إضافة **TCEA Mandatory 13-Domain Client Discovery Framework** كنظام إلزامي يتكون من:

### 5.1 المبدأ العام

```text
Fixed Mandatory Framework:
  13 مجالاً إلزامياً — لا يمكن حذف أو تجاوز أي مجال
  +
Flexible Execution Inside the Framework:
  عمق الأسئلة يتغير حسب حجم المشروع
  المجال غير المناسب = Not Applicable + سبب
  المجال الناقص = توثيق الأثر (هل يمنع التسعير؟ هل يمنع الهاندوف؟)
```

### 5.2 المجالات الـ 13 الإلزامية

| # | المجال | الهدف الأساسي | إلزامي؟ | قابل للـ Not Applicable؟ |
|---|--------|---------------|---------|--------------------------|
| 1 | Business Context & Value | فهم العميل، المشكلة، الهدف التجاري، قياس النجاح | ✅ نعم | ❌ أبداً |
| 2 | Integrations & APIs | التكاملات الخارجية، APIs، التكاليف | ✅ نعم | ✅ نعم |
| 3 | Users & Roles | المستخدمون، الأدوار، الصلاحيات، التسلسل الهرمي | ✅ نعم | ❌ أبداً |
| 4 | Workflow & Operations | سير العمل، الحالات، الموافقات، الاستثناءات | ✅ نعم | ❌ أبداً |
| 5 | Scope & MVP | الميزات الإلزامية، المؤجلة، خارج النطاق | ✅ نعم | ❌ أبداً |
| 6 | Data & Content | الكيانات، الحقول، القوائم، الاستيراد/التصدير | ✅ نعم | ❌ أبداً |
| 7 | Notifications Engine | التنبيهات، القنوات، التوقيت، التكاليف | ✅ نعم | ✅ نعم |
| 8 | Screens & UX | الشاشات، الأدوار، الجهاز المستهدف، التجربة | ✅ نعم | ❌ أبداً |
| 9 | Design & Branding | الهوية، الشعار، الألوان، الخطوط، المراجع | ✅ نعم | ✅ نعم (إذا كان UI فقط) |
| 10 | Reports & Dashboards | التقارير، المؤشرات، المعادلات، الفلاتر | ✅ نعم | ✅ نعم |
| 11 | Technical, Hosting & Compliance | التقنية، الاستضافة، الدومين، الامتثال | ✅ نعم | ❌ أبداً |
| 12 | Security & Audit | المصادقة، الصلاحيات، السجلات، التشفير | ✅ نعم | ✅ نعم (للمشاريع البسيطة) |
| 13 | Acceptance, Commercials & Warranty | معايير القبول، الميزانية، الجدول، الضمان | ✅ نعم | ❌ أبداً |

### 5.3 Depth Scaling Rule

| تصنيف المشروع | عمق الأسئلة لكل مجال | إجمالي الأسئلة التقريبي |
|--------------|----------------------|------------------------|
| صغير واضح | مختصر — سؤال أو سؤالين لكل مجال | 10–15 سؤالاً إجمالياً |
| متوسط | متوسط — 2-4 أسئلة لكل مجال | 20–35 سؤالاً إجمالياً |
| معقد | تفصيلي — 4-7 أسئلة لكل مجال | 35–60 سؤالاً |
| غامض | تحليل منفصل (Paid Discovery) قبل الأسئلة التفصيلية | — |

**القاعدة:** كل مجال يجب أن يُغطّى بسؤال واحد على الأقل (أو وضع `Not Applicable`). لا يمكن تخطي أي مجال.

---

## 6. New Gates Required

### 6.1 Discovery Coverage Gate (جديدة)

**الموقع:** بعد Understanding Confirmation Gate وقبل Scope Packaging

**الإجراء:**
1. TCEA ينتج `DISCOVERY_COVERAGE_SUMMARY.md` (أو يُدرج في `CLIENT_INTAKE.md`).
2. يحتوي على Matrix بجميع المجالات الـ 13 مع حالتها.
3. TCEA يصدر قراراً: `Ready for Scope` / `Needs More Discovery` / `Blocked`.
4. يعرض على Majed للاعتماد.

**الانتقال يُمنع** إذا كان أي مجال إلزامي بحالة `Missing` بدون سبب مقنع.

### 6.2 Quotation Readiness Gate (جديدة)

**الموقع:** قبل إنتاج `DRAFT_QUOTATION.md`

**شروط العبور:**
- Discovery Coverage معتمد
- MVP Scope واضح
- Out of Scope واضح
- Screens estimate > 0
- Reports/Dashboards معروفة (أو مؤجلة)
- Integrations معروفة (أو غير مطلوبة)
- Design direction معروف (أو مفترض)
- Technical/Hosting assumption مسجل
- Security assumptions مسجلة
- Commercial risks موثقة

**إذا كان شيء غير واضح:** يجب توثيقه كافتراض أو عامل خطر (Assumption / Risk Factor)، وليس تجاهله.

### 6.3 Tera Handoff Readiness Gate (مطوّرة من البوابة الحالية)

**الموقع:** قبل إنتاج `TERA_HANDOFF_PACKAGE.md`

**شروط العبور الإضافية:**
- Discovery Coverage معتمد
- Quotation Ready (Level 2 معتمد)
- كل المجالات الـ 13 مغطاة (Complete / Not Applicable)
- أي Missing له أثر موثق وسبب
- الأسئلة المفتوحة مصنفة إلى: `Blocking` / `Non-blocking` / `Deferred` / `Assumption`

**الانتقال يُمنع** إذا كان هناك أي `Blocking` question مفتوح.

---

## 7. New Rules Required

### A. Mandatory Discovery Coverage Rule
لا يجوز لـ TCEA إنشاء/تحديث `CLIENT_BRIEF.md`، `SCOPE_SUMMARY.md`، `FEATURE_LIST.md`، `DRAFT_QUOTATION.md`، أو `TERA_HANDOFF_PACKAGE.md` قبل إنتاج واعتماد Discovery Coverage Summary.

### B. Completeness Matrix Rule
كل مجال في Discovery Coverage Summary يجب أن يكون بحالة واحدة من: `Complete` / `Partial` / `Missing` / `Deferred` / `Not Applicable`. مع توثيق السبب، الأثر، الخطر، والافتراض المؤقت إن وجد.

### C. Question Budget Rule
عدد الأسئلة الإجمالي يجب أن يتناسب مع تصنيف المشروع. للمشاريع الصغيرة 10–15 سؤالاً كافٍ. للمشاريع المتوسطة 20–35. للمعقدة أعمق.

### D. Depth Scaling Rule
المجالات الـ 13 إلزامية دائماً. عمق الأسئلة يتغير حسب حجم المشروع. لا يجوز حذف مجال بالكامل إلا بـ `Not Applicable` + سبب.

### E. No Ignorance Rule
إذا كان مجال ما غير واضح، يجب توثيقه كـ `Assumption` أو `Risk` وليس تجاهله. الافتراض يحتاج تأكيد لاحق.

### F. Question Source Rule
استخدم `TeraApplicationQuestionBank.md` كمصدر أساسي + أسئلة استشارية/تجارية إضافية. استخرج الأسئلة حسب المجال من الـ Question Bank. إذا كان الـ Question Bank لا يغطي مجالاً كافياً، أضف أسئلتك الخاصة.

---

## 8. Expected File Changes

### 8.1 ملفات نظامية (System Files)

| الملف | نوع التغيير | الوصف |
|-------|------------|-------|
| `tera-system/TeraClientEngagement.md` | **تحديث** | إضافة §14: TCEA Mandatory 13-Domain Framework، تحديث §3.2 (Discovery)، إضافة البوابات الجديدة |
| `.opencode/agents/tera-client-engagement.md` | **تحديث** | إضافة القواعد التشغيلية المختصرة للـ 13-Domain Framework |
| `tera-system/TeraPolicyMap.md` | **تحديث** | إضافة مرجع Discovery Coverage Summary إن لزم |
| `tera-system/TeraApplicationQuestionBank.md` | **مراجعة** | التأكد من أن الأسئلة تغطي المجالات الـ 13 بشكل كافٍ لكل مستوى عمق |
| `project-control/AGENT_GAPS_LOG.md` | **تحديث** | تسجيل GAP جديد |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | **تحديث** | بعد الاعتماد والتنفيذ |

### 8.2 ملفات المشروع (مجلد client-engagement/)

| الملف | نوع التغيير | الوصف |
|-------|------------|-------|
| `CLIENT_INTAKE.md` | **تحديث محتوى** | إضافة حقل Discovery Coverage Status أو ربط مع DISCOVERY_COVERAGE_SUMMARY |
| `DISCOVERY_COVERAGE_SUMMARY.md` | **جديد (مستقل أو داخل CLIENT_INTAKE)** | Matrix الـ 13 مجال مع الحالة والأثر |

---

## 9. New Files or Templates Proposed

| الملف | الإلزامية | المحتوى |
|-------|-----------|---------|
| `DISCOVERY_COVERAGE_SUMMARY.md` | جديد — داخل `client-engagement/` | Matrix الـ 13 مجال مع الحالة (Complete/Partial/Missing/Deferred/Not Applicable) + سبب + أثر + خطر + افتراض |
| أو يُدمج داخل `CLIENT_INTAKE.md` كقسم موسّع | اختياري — حسب تفضيل Majed | نفس المحتوى لكن ضمن ملف الـ Intake |

---

## 10. Impact on TCEA Runtime Behavior

| السلوك الحالي | السلوك المتوقع بعد التغيير |
|--------------|---------------------------|
| يبدأ Discovery، يجمع معلومات، ينتقل مباشرة للنطاق | يبدأ Discovery، يجمع معلومات، ينتج DISCOVERY_COVERAGE_SUMMARY، يعرض على Majed، يحصل على اعتماد، ثم ينتقل للنطاق |
| ينسى مجالات كاملة (مثل التصميم) | كل المجالات الـ 13 تُطرح (ولو بسؤال واحد أو Not Applicable مع سبب) |
| ينتج DRAFT_QUOTATION دون تأكيد تغطية Discovery | لا ينتج DRAFT_QUOTATION قبل عبور Quotation Readiness Gate |
| ينتج TERA_HANDOFF_PACKAGE دون مراجعة شاملة | لا ينتج TERA_HANDOFF_PACKAGE قبل عبور Handoff Readiness Gate |
| الأسئلة المفتوحة غير مصنفة | الأسئلة المفتوحة مصنفة (Blocking/Non-blocking/Deferred/Assumption) |
| Discovery Coverage غير موثق | Discovery Coverage Summary موثق ومعتمد |

---

## 11. Impact on TeraAgent

**إيجابي:**
- حزمة Handoff أكثر اكتمالاً — تقلل الحاجة إلى CLARIFICATION_REQUEST.md
- معلومات التصميم والتقنية والأمان موجودة مسبقاً
- Discovery Coverage Summary يعطي TeraAgent نظرة واضحة عن مستوى اكتمال المعلومات

**تأثير محدود:**
- TeraAgent لا يتغير — نفس التدفق (يستلم Handoff Package من ApplicationBlueprintAgent)
- لكن جودة المدخلات ستكون أعلى

---

## 12. Impact on ApplicationBlueprintAgent

**إيجابي:**
- يستلم DISCOVERY_COVERAGE_SUMMARY كمدخل إضافي (إن وُجد)
- معلومات التصميم والتقنية والبيانات تكون متوفرة في ملفات client-engagement/
- يقل الاضطرار لوضع افتراضات في الـ Blueprint

**تأثير محدود:**
- لا يتغير تعريف ApplicationBlueprintAgent
- يبقى دوره: تحويل Handoff المعتمد إلى Blueprint

---

## 13. Impact on Pricing Workflow

**تغيير في التسلسل:**

| الوضع الحالي | الوضع بعد التغيير |
|-------------|-------------------|
| Level 1 → Level 2 (بعد توثيق النطاق) → Level 3 | Level 1 → **Discovery Coverage Gate** → Level 2 (بعد تغطية الـ 13 مجال) → Level 3 |

**تأثير إيجابي:**
- تقدير Level 2 يكون أدق لأن معلومات التصميم والتقنية والأمان متوفرة
- Risk Buffer يُحتسب بدقة أكبر
- يقل احتمال تغير السعر بعد Level 2 بسبب اكتشاف متطلبات جديدة

---

## 14. Impact on Client Files Structure

**تغيير طفيف — إضافة اختيارية:**

```
clients/CLIENT-*/applications/APP-*/client-engagement/
├── CLIENT_INTAKE.md ← (قد يُدمج فيه Discovery Coverage)
├── DISCOVERY_COVERAGE_SUMMARY.md ← (جديد — اختياري مستقل)
├── CLIENT_BRIEF.md
├── SCOPE_SUMMARY.md
├── FEATURE_LIST.md
├── DRAFT_QUOTATION.md
├── TERA_HANDOFF_PACKAGE.md
├── CLIENT_DECISION_LOG.md
└── CHANGE_REQUEST_LOG.md
```

---

## 15. Anti-Bloat Controls

### 15.1 كيف نمنع التضخم في المشاريع الصغيرة

| آلية منع التضخم | كيف تعمل |
|-----------------|----------|
| **Question Budget** | المشروع الصغير: 10–15 سؤالاً فقط عبر كل المجالات. سؤال أو سؤالين لكل مجال كافٍ |
| **سقف Number of Rounds** | المشروع الصغير: 2-3 جولات كحد أقصى. لا دورات لا نهائية من الأسئلة |
| **Depth Scaling** | المشروع الصغير: مختصر لكل مجال. ليس هناك حاجة لأسئلة عميقة عن أمان أو تقنية معقدة |
| **Not Applicable** | المجال غير المناسب: يكتب `Not Applicable` مع سطر سبب واحد. لا حاجة لأسئلة |
| **تجمع الأسئلة** | لا سؤال منفرد — تُطرح الأسئلة في مجموعات من 4-7 أسئلة كحد أقصى |
| **حد زمني** | Discovery للمشروع الصغير يجب ألا يتجاوز 3 جولات أو جلسة واحدة مركزة |

### 15.2 مبدأ الحوكمة

```text
Mandatory Coverage ≠ Mandatory Deep Interview
التغطية إلزامية — التعمق حسب حجم المشروع
الهدف: منع النقص، وليس تضخيم المشاريع الصغيرة
```

---

## 16. Risks of the Change

| المخاطرة | الاحتمال | التأثير | خطة التخفيف |
|----------|---------|---------|-------------|
| تحويل Discovery إلى استبيان طويل غير عملي | متوسط | مرتفع | Question Budget + Depth Scaling + Not Applicable |
| إبطاء المشاريع الصغيرة ببوابات كثيرة | منخفض | متوسط | Discovery Coverage Gate سريع للمشاريع الصغيرة (يمكن أن يكون 5 دقائق) |
| تعقيد ملف CLIENT_INTAKE.md | متوسط | منخفض | يمكن وضع DISCOVERY_COVERAGE_SUMMARY كملف منفصل |
| مقاومة TCEA للالتزام بالمجالات الإلزامية | منخفض | مرتفع | القاعدة إلزامية ومضمنة في التعريف التشغيلي |
| مجال معين يحتاج خبرة غير متوفرة | منخفض | متوسط | يمكن استخدام Websearch + اقتراح افتراضيات + توثيق كـ Assumption |

---

## 17. Risks of Not Applying the Change

| المخاطرة | التأثير |
|----------|---------|
| استمرار Discovery غير مكتمل في المشاريع القادمة | فقدان ثقة العملاء، إعادة عمل، هدر وقت |
| TCEA لا يتطور من مجمع معلومات إلى مستشار اكتشاف | ضعف جودة الخدمة، صعوبة في المشاريع المتوسطة والمعقدة |
| ApplicationBlueprintAgent يبني Blueprint على أساس ناقص | تخمين في التصميم والتقنية يؤدي إلى إعادة عمل |
| تكرار GAP-001 و GAP-002 في كل عميل جديد | زيادة العبء على Majed لسد الفجوات يدوياً |
| أول عميل قد يكون الأخير إذا كانت التجربة سيئة | خطر على سمعة Tera كمنظومة متكاملة |

---

## 18. Implementation Plan

### المرحلة 1: تحديث ملفات النظام (جلسة تطوير واحدة)

| الخطوة | الملف | الإجراء |
|--------|------|---------|
| 1.1 | `tera-system/TeraClientEngagement.md` | إضافة §14 كامل: Mandatory 13-Domain Framework |
| 1.2 | `tera-system/TeraClientEngagement.md` | تحديث §3.2 (Discovery) لربطه بالـ 13 Domain |
| 1.3 | `tera-system/TeraClientEngagement.md` | تحديث §10 (الملفات الإلزامية) لإضافة DISCOVERY_COVERAGE_SUMMARY |
| 1.4 | `.opencode/agents/tera-client-engagement.md` | تحديث ليعكس البوابات الجديدة والقواعد المختصرة |

### المرحلة 2: remediation للعميل الحالي

| الخطوة | الإجراء |
|--------|---------|
| 2.1 | إنشاء DISCOVERY_COVERAGE_SUMMARY.md للعميل الحالي |
| 2.2 | تعبئة Matrix حسب المعلومات المتوفرة |
| 2.3 | تحديد الفجوات والأسئلة المتبقية |
| 2.4 | عرض على Majed لاعتماد Discovery Coverage أو تحديد جولة أسئلة تكميلية |

### المرحلة 3: تسجيل التغيير

| الخطوة | الملف | الإجراء |
|--------|------|---------|
| 3.1 | `project-control/AGENT_GAPS_LOG.md` | إضافة GAP-002 |
| 3.2 | `project-control/SYSTEM_EVOLUTION_LOG.md` | تسجيل التغيير بعد التنفيذ |

---

## 19. Testing Plan

| الاختبار | كيف يُنفّذ | معيار النجاح |
|----------|-----------|-------------|
| Unit Test 1: Small Project | تشغيل TCEA على مشروع صغير وهمي والتأكد من طرح 10-15 سؤالاً فقط عبر المجالات الـ 13 | الأسئلة لا تتجاوز 15، وكل مجال مغطى |
| Unit Test 2: Missing Domain | حذف مجال واحد وإنتاج DISCOVERY_COVERAGE_SUMMARY — يجب أن يرفض TCEA الانتقال للنطاق | Discovery Coverage Gate تمنع الانتقال |
| Unit Test 3: Not Applicable | تعيين مجال كـ Not Applicable — يجب أن يسمح TCEA بالمرور مع سبب واضح | Disovery Coverage Gate تسمح بالمرور |
| Unit Test 4: Question Budget | مشروع متوسط — يجب ألا يتجاوز 35 سؤالاً | الالتزام بـ Question Budget |
| Integration Test | تطبيق كامل: Discovery → Coverage Summary → Scope → Price → Handoff مع العميل الحالي | كل البوابات تعبر بشكل صحيح |

---

## 20. Acceptance Criteria

يعتبر التغيير ناجحاً إذا:

1. ☐ `TeraClientEngagement.md` يحتوي §14 كامل مع الـ 13 Domain Framework
2. ☐ `.opencode/agents/tera-client-engagement.md` يحتوي البوابات الثلاث الجديدة
3. ☐ `DISCOVERY_COVERAGE_SUMMARY.md` أصبح ملفاً معترفاً به في المنظومة
4. ☐ أول مشروع جديد يُظهر أن TCEA يطرح أسئلة تغطي كل المجالات الـ 13 (ولو بسؤال واحد)
5. ☐ أول مشروع جديد لا ينتقل للنطاق قبل عبور Discovery Coverage Gate
6. ☐ GAP-002 مسجل في AGENT_GAPS_LOG.md وتغيرت حالته بعد التطبيق
7. ☐ العميل الحالي (Alfares) لديه DISCOVERY_COVERAGE_SUMMARY معتمد

---

## 21. Rollback Plan

| الخطوة | الإجراء |
|--------|---------|
| 1 | إزالة §14 من `tera-system/TeraClientEngagement.md` |
| 2 | إزالة البوابات الجديدة من `.opencode/agents/tera-client-engagement.md` |
| 3 | إعادة حالة GAP-002 في `AGENT_GAPS_LOG.md` إلى `Rejected` أو حذفه |
| 4 | استعادة نسخة `TeraClientEngagement.md` من Git إن لزم |
| 5 | الإبقاء على DISCOVERY_COVERAGE_SUMMARY.md كأرشيف إن وُجد |

---

## 22. Recommendation

**Implement with Adjustments** — مع التعديلات التالية على النص الأصلي المُرسل للمطور:

### التعديلات المقترحة على النص الأصلي

1. **توضيح تسلسل البوابات:**
   ```
   Understanding Confirmation Gate (حالية)
     → Discovery Coverage Gate (جديدة)
       → Quotation Readiness Gate (جديدة)
         → Handoff Readiness Gate (مطوّرة)
   ```
   كل بوابة تعتمد على التي قبلها.

2. **تقسيم المجال 13:**
   - `13a. Acceptance Criteria` — معايير القبول، الاختبار، الشخص المخول
   - `13b. Commercial Terms` — الميزانية، الدفع، الضمان، الصيانة (يُدمج مع نظام التسعير الموجود)

3. **إضافة Question Budget:**
   - صغير: 10–15 سؤالاً
   - متوسط: 20–35 سؤالاً
   - معقد: أعمق (35–60)
   - يمنع تحويل Discovery إلى استبيان لا نهائي

4. **إضافة ربط مع ApplicationBlueprintAgent:**
   - `DISCOVERY_COVERAGE_SUMMARY` يكون متاحاً كمدخل استرشادي لـ ApplicationBlueprintAgent
   - ليس إلزامياً للـ Blueprint لكنه مفيد

---

## 23. Open Questions for Majed

| # | السؤال | الخيارات |
|---|--------|----------|
| 1 | هل تفضل `DISCOVERY_COVERAGE_SUMMARY` كملف منفصل أم كقسم داخل `CLIENT_INTAKE.md`؟ | منفصل / داخل CLIENT_INTAKE.md |
| 2 | هل المجالات الـ 13 كما هي أم توافق على تقسيم المجال 13 إلى 2 (Acceptance + Commercial) ليصبح 14 مجالاً؟ | 13 / 14 مجالاً |
| 3 | هل Question Budget (10-15 للصغير، 20-35 للمتوسط) مناسب أم يحتاج تعديلاً؟ | مناسب / يحتاج تعديل |
| 4 | هل تريد remediation للعميل الحالي (Alfares) بإنشاء DISCOVERY_COVERAGE_SUMMARY + جولة أسئلة تكميلية؟ | نعم / لا / لاحقاً |
| 5 | هل توافق على أن Discovery Coverage Gate تكون قبل إنتاج **أي** ملف نطاق، حتى CLIENT_BRIEF.md؟ | نعم / CLIENT_BRIEF فقط قبلها / لا |
| 6 | هل تريد أن تكون Discovery Coverage Summary مرئية لـ ApplicationBlueprintAgent كمدخل استرشادي؟ | نعم / لا / لاحقاً |

---

## 24. References

- `project-control/AGENT_GAPS_LOG.md` — GAP-001 (الفجوة السابقة، تم حلها)
- `project-control/SYSTEM_EVOLUTION_LOG.md` — سجل التغييرات السابقة
- `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-018.md` — إضافة Understanding Confirmation Gate
- `tera-system/TeraClientEngagement.md` — الملف المستهدف
- `.opencode/agents/tera-client-engagement.md` — الملف التشغيلي المستهدف
- `tera-system/TeraApplicationQuestionBank.md` — بنك الأسئلة المرجعي
- `tera-system/TeraClientPolicy.md` — سياسة التعامل مع العميل
- `tera-system/TeraPricingPolicy.md` — سياسة التسعير
- `tera-system/TeraApplicationBlueprint.md` — مرجع الـ Blueprint Agent
- `tera-system/TeraPolicyMap.md` — خريطة السياسات
- `tera-system/TeraArchitectureMap.md` — خريطة المعمارية

---

## Approval

| الجهة | الحالة |
|-------|--------|
| **Majed** | ⏳ Pending |

---

## Notes

- هذا المقترح يبني على GAP-001 (الذي تم حله) ويوسع الحوكمة لتشمل كل مجالات Discovery
- يلتزم بمبدأ: `Fixed mandatory framework + Flexible execution inside the framework`
- يمنع التضخم عبر Question Budget و Depth Scaling Rule
- لا يؤثر على عمل TeraAgent أو ApplicationBlueprintAgent بشكل سلبي
- بعد الاعتماد، يُنفّذ عبر TeraSystemEvolutionAgent في جلسة تطوير واحدة
