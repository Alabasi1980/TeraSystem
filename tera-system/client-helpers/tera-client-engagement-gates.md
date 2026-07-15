---
description: TCEA operational gates — B.1 through B.7b detailed definitions for quality control.
---
# TCEA Gates — ملف المساعد البوابات

> **اقرأني فقط عندما:** تحتاج التحقق من شرط Gate قبل الانتقال بين Modes (A→B Discovery→Pricing, B→C Pricing→Handoff).
>
> **لا تقرأني:** في بداية Session أو أثناء Discovery الروتيني.
>
> **إظهار تأكيد القراءة:** فقط إذا كانت الجلسة Audit/Debug، أو طلب Majed ذلك صراحة، أو كان القرار عالي الأثر. مثال عند الحاجة فقط: "📖 قرأت gates.md — Gate [اسم البوابة] = [PASS/FAIL]"

---

## B.1 Discovery Coverage Gate — بوابة تغطية الاستكشاف

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Discovery Coverage Gate |
| **الهدف** | ضمان أن جميع مجالات الاستكشاف الـ 13 قد غُطّيت بشكل كافٍ قبل الانتقال إلى تصنيف المشروع والتسعير المبدئي |
| **المدخلات المطلوبة** | `DISCOVERY_COVERAGE_SUMMARY.md` (بعد تطبيق Self-Check Protocol A.6.1 على كل Domain) |
| **شروط النجاح** | 1. جميع Domains الـ 13 مغطاة — إما `Complete` أو `Partial` مع `UNCERTAINTY_NOTICE`<br>2. كل Domain `Complete`: مصدر المعلومة واضح، Majed confirmed، والخطورة `Low` أو `Medium`<br>3. كل Domain `Partial`: `UNCERTAINTY_NOTICE` موجود ومرفوع لـ Majed<br>4. لا يوجد Domain بخطورة `High` بدون تأكيد Majed |
| **شروط الإيقاف (Blocking Conditions)** | 1. Domain بخطورة `High` بدون تأكيد Majed ← توقف إجباري<br>2. Domain غير مغطى (لا `Complete` ولا `Partial`) ← توقف<br>3. `UNCERTAINTY_NOTICE` مرفوع ولم يحصل رد من Majed ← توقف |
| **الإخراج الإلزامي** | `DISCOVERY_COVERAGE_SUMMARY.md` + قرار البوابة: **PASS/FAIL**. إذا كان FAIL، يُذكر السبب بوضوح: `Needs More Info` أو `Rejected`. |
| **هل يمنع الانتقال؟** | **نعم** — لا يمكن تصنيف المشروع أو البدء بتحليل النطاق والتسعير قبل PASS |

---

## B.2 Budget-to-Scope Control Rule — قاعدة الموازنة بين النطاق والميزانية

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Budget-to-Scope Control Rule |
| **الهدف** | مواءمة النطاق المقترح مع ميزانية العميل عبر تصنيف الميزات بالأولوية وحساب الجدوى المالية قبل التسعير |
| **المدخلات المطلوبة** | `FEATURE_LIST.md` (الميزات), `CLIENT_INTAKE.md` (ميزانية العميل من Majed), `CLIENT_DECISION_LOG.md` (للتسجيل) |
| **شروط النجاح** | 1. جميع الميزات مصنفة حسب الأولوية: P1 (Must-have), P2 (Should-have), P3 (Nice-to-have)<br>2. تكلفة P1 محسوبة تقديرياً وموثقة في `CLIENT_DECISION_LOG.md`<br>3. إذا P1 ≤ الميزانية → تم توزيع الباقي على P2/P3 مع توثيق<br>4. إذا P1 > الميزانية → تم رفع خيارات لـ Majed (تقليل النطاق / زيادة الميزانية / تقسيم مرحلي) وأخذ قرار موثق |
| **شروط الإيقاف (Blocking Conditions)** | 1. ميزانية العميل غير معروفة ← توقف واسأل Majed صراحة<br>2. P1 غير مقدرة ← توقف<br>3. P1 > الميزانية ولم يتم توثيق قرار Majed ← توقف |
| **الإخراج الإلزامي** | `CLIENT_DECISION_LOG.md` محدّث بتصنيف الأولويات + قرار توزيع الميزانية |
| **هل يمنع الانتقال؟** | **نعم** — يمنع إنتاج `DRAFT_QUOTATION.md` قبل توثيق القرار |

---

## B.3 Final Scope Reconciliation Gate — بوابة توحيد النطاق النهائي

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Final Scope Reconciliation Gate |
| **الهدف** | توحيد حالة جميع الميزات في `FEATURE_LIST.md` قبل التسعير، وضمان عدم وجود ميزات غير مصنّفة أو معلقة بدون قرار |
| **المدخلات المطلوبة** | `FEATURE_LIST.md`, `CLIENT_DECISION_LOG.md` (قرارات الميزانية والتغيير), Budget-to-Scope documentation |
| **شروط النجاح** | 1. كل ميزة في `FEATURE_LIST.md` لها حالة: `In Scope` / `Out of Scope` / `Deferred` / `Pending Decision`<br>2. كل ميزة `In Scope` لها أولوية: P1, P2, P3<br>3. لا توجد ميزة بحالة `Undefined` أو `Unclassified`<br>4. لا توجد ميزة `In Scope` تعتمد على ميزة `Deferred` أو `Pending Decision`<br>5. Budget-to-Scope (B.2) مطبّق وموثّق<br>6. **كل ميزة في `In Scope` تحمل وسماً من A.6.4 — ولا يجوز أن تكون `[Research Hint]` أو `[Assumption]` أو `[Unresolved]`** |
| **شروط الإيقاف (Blocking Conditions)** | 1. أي ميزة بحالة `Undefined` ← توقف<br>2. ميزة `In Scope` بدون أولوية ← توقف<br>3. ميزة تعتمد على أخرى معلقة ← توقف<br>4. P1 > الميزانية بدون قرار Majed ← توقف<br>5. **أي ميزة `In Scope` موسومة بـ `[Research Hint]` أو `[Assumption]` أو `[Unresolved]` ← توقف — يجب ترقية الوسم إلى `[Confirmed by Majed]`** ← **MR1** |
| **الإخراج الإلزامي** | `FEATURE_LIST.md` محدّثة ومكتملة (كل الميزات: حالة + أولوية + وسم A.6.4) + `CLIENT_DECISION_LOG.md` محدّث |
| **هل يمنع الانتقال؟** | **نعم** — لا يمكن إنتاج `DRAFT_QUOTATION.md` قبل PASS |

---

## B.4 Quotation Readiness Gate — بوابة جاهزية التسعير

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Quotation Readiness Gate |
| **الهدف** | التأكد من اكتمال جميع متطلبات التسعير قبل إنتاج `DRAFT_QUOTATION.md` (Level 2) — منع القفز إلى التسعير دون اكتمال الأساسيات |
| **المدخلات المطلوبة** | `CLIENT_INTAKE.md`, `DISCOVERY_COVERAGE_SUMMARY.md` (مع قرار البوابة), `FEATURE_LIST.md` (بعد Reconciliation — جميع العناصر موسومة بـ `[Confirmed by Majed]`), `CLIENT_DECISION_LOG.md`, قائمة TeraPricingPolicy.md §2 (10 بنود تسعيرية), `TeraPricingCalculator.xlsx` (للجاهزية) |
| **شروط النجاح** | 1. Understanding Summary confirmed by Majed<br>2. Discovery Coverage Gate = PASS (B.1)<br>3. Final Scope Reconciliation Gate = PASS (B.3)<br>4. Budget-to-Scope Control Rule documented (B.2)<br>5. معلومات التسعير الأساسية كاملة (حسب TeraPricingPolicy.md §2 — 10 بنود)<br>6. جميع الافتراضات عالية الخطورة (High-risk) محلولة أو موثقة وواضحة لـ Majed ← **MR2**<br>7. **جميع عناصر النطاق والتسعير موسومة بـ `[Confirmed by Majed]` — لا `[Research Hint]` ولا `[Assumption]` ولا `[Unresolved]`** ← **MR1** |
| **شروط الإيقاف (Blocking Conditions)** | 1. Understanding Summary غير مؤكد أو لم يؤكده Majed ← توقف<br>2. Discovery Coverage Gate ≠ PASS ← توقف<br>3. Final Scope Reconciliation Gate ≠ PASS ← توقف<br>4. Budget-to-Scope غير موثق ← توقف<br>5. أي معلومة تسعيرية أساسية ناقصة (من TeraPricingPolicy.md §2) ← توقف<br>6. أي افتراض High-risk غير محسوم ← توقف ← **MR2**<br>7. **أي عنصر تسعير مبني على `[Research Hint]` أو `[Assumption]` ← توقف — يجب ترقية الوسم إلى `[Confirmed by Majed]`** ← **MR1** |
| **الإخراج الإلزامي** | تقرير **PASS/FAIL** + قائمة **Blocking Gaps** (الفجوات المانعة) إذا كان FAIL |
| **هل يمنع الانتقال؟** | **نعم** — لا يمكن إنتاج `DRAFT_QUOTATION.md` قبل PASS |

---

## B.5 CLIENT_DECISION_LOG.md — سجل قرارات العميل

| البند | التفاصيل |
|-------|----------|
| **الاسم** | CLIENT_DECISION_LOG.md |
| **الهدف** | توثيق كل قرار يُتخذ أثناء دورة حياة العميل — تغييرات النطاق، تعديلات السعر، تحولات الأولوية — في سجل واحد قابل للتتبع |
| **المدخلات المطلوبة** | القرارات الصادرة عن Majed أو العميل خلال كل مرحلة من تدفق العمل |
| **شروط النجاح** | 1. كل إدخال يحتوي على: Decision ID \| Date \| Topic \| Decision \| Rationale \| Status \| Source<br>2. جميع القرارات مسجلة فور حدوثها — لا تأجيل<br>3. قبل Tera Handoff: كل الإدخالات بحالة `Approved` أو `Deferred` — صفر `Pending Approval` |
| **الحالات المسموحة** | `Approved` (تم الاعتماد) \| `Deferred` (أُجّل) \| `Conditional` (معلق على شرط) \| `Pending Approval` (بانتظار الاعتماد) |
| **شروط الإيقاف (Blocking Conditions)** | 1. أي إدخال بحالة `Pending Approval` عند Tera Handoff ← يمنع PASS في B.7a (Handoff Draft Readiness) و B.7b (Final Handoff Package) ← **MR3**<br>2. قرار تغيير نطاق أو سعر غير موثق ← يعتبر مخالفة |
| **الإخراج الإلزامي** | `CLIENT_DECISION_LOG.md` محدّث باستمرار |
| **هل يمنع الانتقال؟** | **نعم** — بشكل غير مباشر: يمنع Tera Handoff إذا بقي أي إدخال `Pending Approval` |

---

## B.6a Source Approval Consistency — فحص اتساق المصادر (قبل الحزمة)

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Source Approval Consistency Check |
| **الهدف** | التأكد من أن جميع وثائق المصدر جاهزة ومعتمدة **قبل** صياغة `TERA_HANDOFF_PACKAGE.md` — لا يُذكر الحزمة لأنها لم تُنتج بعد |
| **المدخلات المطلوبة** | `CLIENT_INTAKE.md`, `SCOPE_SUMMARY.md`, `FEATURE_LIST.md`, `DRAFT_QUOTATION.md`, `CLIENT_DECISION_LOG.md`, `CHANGE_REQUEST_LOG.md`, `CLIENT_BRIEF.md` |
| **شروط النجاح** | 1. **لا مستندات عالقة:** لا يوجد مستند `Draft` يجب أن يكون `Approved`<br>2. **القرارات محسومة:** CLIENT_DECISION_LOG.md: 0 `Pending Approval`<br>3. **اتساق النطاق:** SCOPE_SUMMARY.md متطابق مع FEATURE_LIST.md — لا ميزات يتيمة<br>4. **اتساق السعر:** DRAFT_QUOTATION.md متوافق مع النطاق والميزات الموثقة<br>5. **حسم طلبات التغيير:** جميع CHANGE_REQUEST_LOG.md محسومة (Approved/Rejected/Deferred) |
| **شروط الإيقاف (Blocking Conditions)** | 1. أي مستند مصدر بحالة `Draft` ويتطلب `Approved` ← توقف<br>2. أي قرار بحالة `Pending Approval` ← توقف<br>3. SCOPE_SUMMARY.md لا يتطابق مع FEATURE_LIST.md ← توقف<br>4. CHANGE_REQUEST غير محسوم ← توقف |
| **الإخراج الإلزامي** | تقرير **PASS/FAIL** مع قائمة الاختبارات الراسبة. عند PASS فقط يُسمح بصياغة `TERA_HANDOFF_PACKAGE.md` كمسودة أولية |
| **هل يمنع الانتقال؟** | **نعم** — يمنع إنتاج مسودة `TERA_HANDOFF_PACKAGE.md` |

## B.6b Package Approval Consistency — فحص اتساق الحزمة (بعد الحزمة)

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Package Approval Consistency Check |
| **الهدف** | التأكد من أن حالة `TERA_HANDOFF_PACKAGE.md` متسقة مع مصادرها — لا يمكن أن تكون الحزمة `Approved` إذا كان أي مصدر لا يزال `Draft` أو `Pending Approval` |
| **المدخلات المطلوبة** | `TERA_HANDOFF_PACKAGE.md`, `CLIENT_INTAKE.md`, `SCOPE_SUMMARY.md`, `FEATURE_LIST.md`, `DRAFT_QUOTATION.md`, `CLIENT_DECISION_LOG.md`, `CHANGE_REQUEST_LOG.md` |
| **شروط النجاح** | 1. **اتساق الحالة:** TERA_HANDOFF_PACKAGE.md تأخذ أقل حالة من جميع المصادر — إذا أي مصدر `Draft` أو `Pending`، فالحزمة لا يمكن أن تكون `Approved`<br>2. **جميع المصادر معتمدة:** لا يوجد مصدر بحالة `Draft` يتطلب `Approved` |
| **شروط الإيقاف (Blocking Conditions)** | 1. TERA_HANDOFF_PACKAGE.md بحالة أعلى من أقل مصدر (مثلاً الحزمة `Approved` وأحد المصادر `Draft`) ← توقف |
| **الإخراج الإلزامي** | تقرير **PASS/FAIL**. عند PASS فقط يمكن إعلان `TERA_HANDOFF_PACKAGE.md` كـ `Approved` وجاهزة للتسليم |
| **هل يمنع الانتقال؟** | **نعم** — يمنع تسليم `TERA_HANDOFF_PACKAGE.md` النهائية |

---

## B.7a Handoff Draft Readiness Gate — بوابة جاهزية مسودة الهاندوف

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Handoff Draft Readiness Gate |
| **الهدف** | التأكد من اكتمال جميع المتطلبات المسبقة للهاندوف **قبل** صياغة `TERA_HANDOFF_PACKAGE.md` — منع إنتاج حزمة على أساس غير مكتمل |
| **المدخلات المطلوبة** | `CLIENT_INTAKE.md`, `SCOPE_SUMMARY.md`, `FEATURE_LIST.md`, `DRAFT_QUOTATION.md`, `CLIENT_DECISION_LOG.md`, `CHANGE_REQUEST_LOG.md`, `CLIENT_BRIEF.md` |
| **شروط النجاح** | 1. Source Approval Consistency = PASS (B.6a)<br>2. Quotation Readiness Gate = PASS (B.4)<br>3. Final Scope Reconciliation = PASS (B.3)<br>4. Budget-to-Scope documented (B.2)<br>5. CLIENT_DECISION_LOG.md: صفر `Pending Approval` ← **MR3**<br>6. Quotation معتمد من Majed (Level 2 Approved)<br>7. جميع CHANGE_REQUEST_LOG.md محسومة<br>8. Workspace Plan confirmed by Majed (المسار المتوقع: `clients/CLIENT-NAME/applications/APP-NAME/`، الاسم الرسمي، والهيكل المتوقع) — تخطيط فقط لا إنشاء مجلدات |
| **شروط الإيقاف (Blocking Conditions)** | 1. Source Approval Consistency = FAIL ← توقف<br>2. أي قرار `Pending Approval` ← توقف ← **MR3**<br>3. Level 2 Quotation غير معتمد من Majed ← توقف<br>4. CHANGE_REQUEST غير محسوم ← توقف<br>5. Workspace Plan غير مؤكد من Majed ← توقف |
| **الإخراج الإلزامي** | تقرير **PASS/FAIL** + قائمة **Blocking Gaps** عند FAIL. عند PASS: يُسمح بإنتاج `TERA_HANDOFF_PACKAGE.md` كمسودة أولية |
| **هل يمنع الانتقال؟** | **نعم** — لا يمكن إنتاج `TERA_HANDOFF_PACKAGE.md` قبل PASS |

---

## B.7b Final Handoff Package Gate — بوابة الحزمة النهائية للهاندوف

| البند | التفاصيل |
|-------|----------|
| **الاسم** | Final Handoff Package Gate |
| **الهدف** | التأكد من أن `TERA_HANDOFF_PACKAGE.md` نفسها مكتملة ومتسقة وجاهزة للتسليم إلى ApplicationBlueprintAgent / TeraAgent — بعد صياغتها |
| **المدخلات المطلوبة** | `TERA_HANDOFF_PACKAGE.md`, `CLIENT_INTAKE.md`, `SCOPE_SUMMARY.md`, `FEATURE_LIST.md`, `DRAFT_QUOTATION.md`, `CLIENT_DECISION_LOG.md`, `CHANGE_REQUEST_LOG.md`, Workspace Plan المعتمد من Majed (تخطيط فقط — لا إنشاء مجلدات) |
| **شروط النجاح** | 1. `TERA_HANDOFF_PACKAGE.md` تحتوي على جميع الوثائق الأساسية: CLIENT_BRIEF أو SCOPE_SUMMARY + FEATURE_LIST + DRAFT_QUOTATION + CLIENT_DECISION_LOG + CHANGE_REQUEST_LOG<br>2. **جميع العناصر في حزمة الهاندوف موسومة بـ `[Confirmed by Majed]` — لا `[Research Hint]` ولا `[Assumption]` ولا `[Unresolved]`** ← **MR1**<br>3. الحزمة متسقة مع المصادر (CLIENT_INTAKE.md, SCOPE_SUMMARY.md, FEATURE_LIST.md, DRAFT_QUOTATION.md)<br>4. جميع إدخالات CLIENT_DECISION_LOG.md منعكسة في الحزمة<br>5. Package Approval Consistency = PASS (B.6b) |
| **شروط الإيقاف (Blocking Conditions)** | 1. أي وثيقة أساسية ناقصة من `TERA_HANDOFF_PACKAGE.md` ← توقف<br>2. **أي عنصر موسوم بـ `[Research Hint]` أو `[Assumption]` أو `[Unresolved]` داخل الحزمة ← توقف — يجب أن يكون `[Confirmed by Majed]`** ← **MR1**<br>3. عدم اتساق بين الحزمة والمصادر ← توقف<br>4. Package Approval Consistency = FAIL (B.6b) ← توقف |
| **الإخراج الإلزامي** | تقرير **PASS/FAIL** + قائمة **Blocking Gaps** عند FAIL. عند PASS: `TERA_HANDOFF_PACKAGE.md` جاهز للتسليم إلى ApplicationBlueprintAgent |
| **هل يمنع الانتقال؟** | **نعم** — لا يمكن تسليم الحزمة إلى ApplicationBlueprintAgent أو TeraAgent قبل PASS |

---

## Workspace Verification — التحقق من مساحة العمل

> هذا فحص خفيف بعد إنشاء المجلدات — ليس بوابة انتقال بين Modes، ولا يحتاج PASS/FAIL رسمي.

| البند | التفاصيل |
|-------|----------|
| **التوقيت** | بعد إنشاء مساحة العمل فعلياً (بعد PASS B.7b + موافقة Majed) |
| **الهدف** | التأكد من أن هيكل المجلدات قد أُنشئ بشكل صحيح وفق Workspace Plan، قبل وضع الملفات والتسليم |
| **قائمة التحقق** | 1. المجلد الرئيسي `clients/CLIENT-NAME/` موجود<br>2. المجلد `applications/APP-NAME/` موجود<br>3. المجلد `client-engagement/` موجود داخل APPLICATION<br>4. جميع المسارات تطابق Workspace Plan المعتمد من Majed |
| **عند الفشل** | أعلم Majed وأعد إنشاء الهيكل المفقود — هذا الفحص لا يمنع التسليم، لكن الهيكل الناقص يُبلغ عنه |
