# SYSTEM_EVOLUTION_LOG.md

## غرض هذا السجل

سجل خاص بتغييرات تطوير منظومة Tera نفسها — ليس لتتبع أعمال تطبيقات العملاء أو مهام المشاريع.

هذا السجل يُستخدم بواسطة `TeraSystemEvolutionAgent` لتسجيل كل تغيير مُنفَّذ على المنظومة بعد موافقة المالك.

---

## صيغة الإدخال

```text
تاريخ: YYYY-MM-DD
معرف التغيير: SCP-YYYY-MM-DD-NNN
مصدر الطلب: User Request / Auditor Report / Monitor Report / Policy Conflict / Research / Other
نوع التغيير: New Agent / Policy Update / Architecture Update / Protocol Change / Agent Improvement / Anti-Bloat / Other
الملفات المعدلة:
- مسار الملف 1
- مسار الملف 2
الملخص:
الموافقة: Majed — Approved / Approved with Conditions
التحقق من الصحة: Validation Passed / Needs Follow-up
المخاطر:
ملاحظات الاسترجاع (Rollback):

---

## 🗂️ أرشيف الإدخالات السابقة

جميع الإدخالات السابقة (SCP-045 إلى SCP-2026-07-06-072-HOTFIX-012)
محفوظة في النسخة الاحتياطية خارج مساحة العمل.

السجل يستأنف من هنا بإدخالات جديدة فقط.

---

## إدخالات جديدة — تبدأ من هنا

## SCP-2026-07-06-073 — حل التعارض الدائري في B.7 (تقسيم إلى B.7a + B.7b)

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-073
- مصدر الطلب: Auditor Report (ملاحظات الهيئة الخارجية)
- نوع التغيير: Policy Conflict Resolution
- الملفات المعدلة:
  - `tera-system/client-helpers/tera-client-engagement-gates.md` — تقسيم B.7 إلى B.7a + B.7b
  - `.opencode/agents/tera-client-engagement.md` — تحديث ~9 مراجع وتدفق العمل
  - `tera-system/client-helpers/tera-client-engagement-protocols.md` — تحديث مرجع B.7 في قائمة Failsafe
- الملخص:
  - تم تقسيم B.7 (Tera Handoff Readiness Gate) إلى بوابتين متسلسلتين زمنياً:
    - **B.7a — Handoff Draft Readiness Gate**: قبل إنتاج TERA_HANDOFF_PACKAGE.md — تتحقق من المتطلبات المسبقة (B.6, B.4, B.3, B.2, Pending Approvals, Quotation, Change Requests, Workspace)
    - **B.7b — Final Handoff Package Gate**: بعد صياغة الحزمة — تتحقق من اكتمالها واتساقها وخلوها من [Assumption]/[Research Hint]/[Unresolved]
  - تم تحديث جميع المراجع في الملف الرئيسي (A.0, A.3, A.4, B, C.4, D.1) وفي protocols.md
  - دائرة B.7 المنطقية (لا حزمة قبل البوابة ولا بوابة بدون حزمة) قد حُلّت
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed (Zero stale references, Anti-Bloat OK, No client-app contamination, No privilege expansion, git diff --check OK)
- المخاطر: منخفض — إعادة توزيع شروط موجودة فقط، لا إضافة أو حذف
- ملاحظات الاسترجاع (Rollback): استعادة gates.md من git ثم إعادة المراجع في tera-client-engagement.md و protocols.md

## SCP-2026-07-06-074 — حل تعارض إنشاء مساحة العمل (فصل التخطيط عن التنفيذ)

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-074
- مصدر الطلب: Auditor Report (ملاحظات الهيئة الخارجية)
- نوع التغيير: Policy Conflict Resolution
- الملفات المعدلة:
  - `tera-system/client-helpers/tera-client-engagement-gates.md` — تعديل B.7a الشرط 8 + إضافة Workspace Verification
  - `.opencode/agents/tera-client-engagement.md` — تحديث A.2 (81)، التدفق (161-166)، الملاحة (380)، D.1 (564)
- الملخص:
  - تم فصل "Workspace Plan" (تخطيط المسار والهيكل قبل B.7a) عن "Workspace Creation" (إنشاء المجلدات فعلياً بعد B.7b + موافقة Majed)
  - B.7a الشرط 8: من "Workspace structure ... جاهز" ← "Workspace Plan confirmed by Majed"
  - تمت إضافة Workspace Verification كفحص خفيف بعد الإنشاء
  - التدفق A.4: تأكيد الخطة قبل B.7a ← الإنشاء الفعلي بعد B.7b ← التحقق بعد الإنشاء
  - A.2 (81): تحديث الصياغة لتعكس التخطيط قبل والإنشاء بعد
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed (Anti-Bloat OK, No stale references, No client-app contamination, No privilege expansion, git diff --check OK)
- المخاطر: منخفض — فصل التخطيط (غير تنفيذي) عن الإنشاء (تنفيذي)، لا تغيير في شروط الحوكمة
- ملاحظات الاسترجاع (Rollback): استعادة gates.md (الشرط 8 في B.7a) والملف الرئيسي (A.2، التدفق، الملاحة، D.1) من git

## SCP-2026-07-06-075 — نقل خطوات التسعير العشر من دليل التدريب إلى pricing.md

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-075
- مصدر الطلب: Auditor Report (ملاحظات الهيئة الخارجية)
- نوع التغيير: Content Relocation / Agent Improvement
- الملفات المعدلة:
  - `tera-system/client-helpers/tera-client-engagement-pricing.md` — إضافة الخطوات العشر + تحديث 3 مراجع
  - `.opencode/agents/tera-client-engagement.md` — تحديث C.4 (Required If Triggered + Reference Only)
  - `project-control/TRAINING_GUIDE_TCEA.md` — إضافة ملاحظة في §3 عن نقل الخطوات
- الملخص:
  - نُقلت الخطوات العشر التشغيلية لتسعير Level 2 من TRAINING_GUIDE_TCEA.md §3 إلى pricing.md كقائمة تشغيل مضغوطة (~20 سطراً)
  - أزيلت قاعدة "مرة واحدة في العمر" من جميع المراجع — لم يعد TRAINING_GUIDE مطلوباً للتشغيل اليومي
  - TRAINING_GUIDE_TCEA.md تحوّل إلى مرجع تدريب بحت (يُقرأ فقط عند تحذير Proportion Check)
  - أي نموذج في أي جلسة يجد الخطوات مباشرة في pricing.md — لا حاجة لذاكرة سابقة
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed (Anti-Bloat OK, Zero stale references, No client-app contamination, No privilege expansion, git diff --check OK)
- المخاطر: منخفض — لا تغيير في محتوى الخطوات، فقط نقلها إلى الموقع الصحيح
- ملاحظات الاسترجاع (Rollback): استعادة pricing.md (الخطوات العشر والمراجع الثلاثة)، الملف الرئيسي (C.4)، و TRAINING_GUIDE_TCEA.md §3 (الملاحظة) من git

## SCP-2026-07-06-076 — توحيد تعريف مجالات Discovery الـ 13 في مصدر واحد

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-076
- مصدر الطلب: Auditor Report (ملاحظات الهيئة الخارجية)
- نوع التغيير: Data Standardization / Policy Conflict Resolution
- الملفات المعدلة:
  - **جديد:** `tera-system/client-helpers/tera-client-engagement-discovery-domains.md` — المصدر الرسمي للمجالات
  - `tera-system/TeraApplicationQuestionBank.md` — استبدال القائمة بإشارة + ملاحظة ترقيم
  - `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` §35 — إضافة تعليق مرجعي في Domain Coverage Matrix
  - `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` — تصحيح ترقيم المجالات في Smart Interview
  - `tera-system/TeraPolicyMap.md` — إضافة إدخال للمصدر الرسمي
- الملخص:
  - ثلاثة أنظمة ترقيم مختلفة كانت موجودة (Question Bank, Template, Protocols) تسبب أخطاء تراكمية
  - تم إنشاء مصدر واحد رسمي بـ 13 Domain + Canonical Name + Aliases + Minimum Coverage + Blocks Pricing/Handoff
  - Question Bank: استبدال القائمة المكررة بإشارة إلى المصدر الرسمي
  - Template: إضافة تعليق مرجعي (الأسماء متطابقة أصلاً مع المصدر الرسمي)
  - Protocols: تصحيح Domain 1/2/4 من (Administrative/Functional/Technical) إلى (Business & Goals / Users, Roles & Access / Data & Content)
  - PolicyMap: إضافة إدخال للمصدر الجديد
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed (Anti-Bloat OK, No stale references, No client-app contamination, No privilege expansion, git diff --check OK)
- المخاطر: متوسط — تغيير ترقيم في Protocol قد يربك النماذج الحالية مؤقتاً. تمت إضافة ملاحظة توضيحية.
- ملاحظات الاسترجاع (Rollback): حذف discovery-domains.md، استعادة Question Bank, Template §35, Protocols, PolicyMap من git

## SCP-2026-07-06-077 — إصلاح تعارض B.7b: "هيكل مساحة العمل" ← "Workspace Plan"

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-077
- مصدر الطلب: Auditor Report (ملاحظة الهيئة الخارجية — Finding #1)
- نوع التغيير: Policy Conflict Resolution
- الملف المتأثر:
  - `tera-system/client-helpers/tera-client-engagement-gates.md:119` — مدخلات B.7b
- الملخص:
  - B.7b كانت تطلب "هيكل مساحة العمل" كمدخل، لكن مساحة العمل تُنشأ بعد B.7b (حسب main:164-165)
  - تم استبدال "هيكل مساحة العمل" بـ "Workspace Plan المعتمد من Majed (تخطيط فقط — لا إنشاء مجلدات)"
  - تتطابق الآن مع صياغة B.7a (السطر 106-107) والتي كانت صحيحة سابقاً
- الموافقة: Majed — Approved
- التحقق من الصحة: 0 مراجع "هيكل مساحة العمل" في الملفات النشطة ✅، Consistency مع الملف الرئيسي ✅، git diff --check ✅
- المخاطر: منخفض جداً
- ملاحظات الاسترجاع (Rollback): `git checkout -- tera-system/client-helpers/tera-client-engagement-gates.md`

## SCP-2026-07-06-078 — تقسيم B.6 إلى B.6a (Source) + B.6b (Package)

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-078
- مصدر الطلب: Auditor Report (ملاحظة الهيئة الخارجية — Finding #2)
- نوع التغيير: Policy Conflict Resolution
- الملفات المعدلة:
  - `tera-system/client-helpers/tera-client-engagement-gates.md` — تقسيم B.6 → B.6a + B.6b، تحديث B.7a → B.6a، تحديث B.7b → إضافة B.6b
  - `.opencode/agents/tera-client-engagement.md` — 6 مراجع قديمة لـ B.6 تم تحديثها إلى B.6a/B.6b
  - `tera-system/client-helpers/tera-client-engagement-protocols.md` — تحديث مرجعين (B.6 → B.6a + B.6b)
- الملخص:
  - B.6 كان يخدم غرضين متعارضين: فحص المصادر + فحص الحزمة، بينما B.7a يستخدمه قبل وجود الحزمة
  - **B.6a (Source Approval Consistency)**: قبل صياغة الحزمة — تفحص المصادر فقط، لا تذكر الحزمة. تُستخدم في B.7a
  - **B.6b (Package Approval Consistency)**: بعد صياغة الحزمة — تفحص أن حالة الحزمة لا تتجاوز أقل حالة من مصادرها. تُستخدم في B.7b
  - 8 مراجع قديمة محدثة في 3 ملفات نشطة
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ 0 مراجع قديمة لـ B.6 في الملفات النشطة، Consistency بين gates.md + main + protocols، Anti-Bloat OK (تقسيم موجود لم يضف شروطاً جديدة)
- المخاطر: منخفض — إعادة هيكلة لمنطق موجود فقط
- ملاحظات الاسترجاع (Rollback): `git checkout -- tera-system/client-helpers/tera-client-engagement-gates.md tera-system/client-helpers/tera-client-engagement-protocols.md .opencode/agents/tera-client-engagement.md`

## SCP-2026-07-06-079 — تصحيح قاعدة الإحالة للأقسام في A.4

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-079
- مصدر الطلب: Auditor Report (ملاحظة الهيئة الخارجية — Finding #3)
- نوع التغيير: Policy Conflict Resolution
- الملف المعدل:
  - `.opencode/agents/tera-client-engagement.md:140` — سطر واحد
- الملخص:
  - السطر 140 كان يقول "جميع الإشارات إلى أقسام مرقمة تشير إلى أقسام داخل هذا الملف نفسه" — وهذا عكس الواقع (A.6.x في protocols.md، B.x في gates.md، A.8.x في pricing.md)
  - تم استبداله بـ: "الإشارات إلى الأقسام المرقمة قد تكون في الملف الرئيسي أو في ملف مساعد. استخدم D.1 Routing Table للوصول إلى الملف الصحيح. لا تستنتج محتوى أي قسم من اسمه فقط."
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ سطر واحد، لا مراجع قديمة، متسق مع D.1 Routing Table
- المخاطر: منخفض جداً
- ملاحظات الاسترجاع (Rollback): `git checkout -- .opencode/agents/tera-client-engagement.md`

## SCP-2026-07-06-080 — حذف القالب القديم §13A Discovery Coverage Summary

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-080
- مصدر الطلب: Auditor Report (ملاحظة الهيئة الخارجية — Finding #4)
- نوع التغيير: Anti-Bloat / Data Standardization
- الملف المعدل:
  - `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` — حذف §13A (37 سطراً)
- الملخص:
  - قالبان لنفس المخرج بترتيب مختلف: §13A (قديم، ترتيب مختلف، أقل شمولاً) و §35 (جديد، حسب المصدر الرسمي discovery-domains.md)
  - §13A كان يستخدم ترتيباً قديماً (Business Context → Integrations → Users → Workflow...) يختلف عن الـ 13 الرسمي
  - §13A لم يكن مرجعاً من أي ملف نشط
  - تم حذفه بالكامل — §35 هو المعتمد الوحيد
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ 0 مراجع لـ §13A في أي ملف، git diff --check OK
- المخاطر: منخفض جداً
- ملاحظات الاسترجاع (Rollback): `git checkout -- tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`

## SCP-2026-07-06-082 — إدراج discovery-domains.md في التحميل الإلزامي عند بداية Discovery

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-082
- مصدر الطلب: Auditor Report (ملاحظة الهيئة الخارجية — Finding #6 — آخر ملاحظة متبقية)
- نوع التغيير: Documentation Fix (Missing Reference in Runtime Loading Table)
- الملف المعدل:
  - `.opencode/agents/tera-client-engagement.md` — تعديلان (سطران)
- الملخص:
  - تمت إضافة `tera-system/client-helpers/tera-client-engagement-discovery-domains.md` إلى **🟢 Required Now** تحت "بداية Discovery" (سطر 412)
  - تمت إضافة "**13 Discovery Domains**" إلى **D.1 Routing Table** كسطر مستقل قبل B.1 (سطر 564)
  - يحل الملاحظة الوحيدة المتبقية من هيئة التدقيق الخارجي
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ git diff --check OK، Anti-Bloat Gate PASS، لا تضارب مع TeraPolicyMap.md
- المخاطر: منخفض جداً — مجرد إضافة مرجعين في جداول موجودة، دون تغيير في السياسات أو الصلاحيات
- ملاحظات الاسترجاع (Rollback): `git checkout -- .opencode/agents/tera-client-engagement.md`

---

## SCP-2026-07-06-081 — تصحيح مرجع Source of Truth (TeraClientEngagement.md لم يعد موجوداً)

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-081
- مصدر الطلب: Auditor Report (ملاحظة الهيئة الخارجية — Finding #5)
- نوع التغيير: Documentation Fix (Stale Reference)
- الملف المعدل:
  - `.opencode/agents/tera-client-engagement.md:19` — سطر واحد
- الملخص:
  - السطر 19 كان يشير إلى `tera-system/TeraClientEngagement.md` كمصدر، لكن الملف لم يعد موجوداً (دُمج في SCP-051)
  - تم استبداله بـ: `Source of Truth: .opencode/agents/tera-client-engagement.md` + ملاحظة تاريخية مختصرة
  - 0 مراجع أخرى للملف القديم في الملفات النشطة
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ 0 مراجع قديمة لـ TeraClientEngagement.md
- المخاطر: منخفض جداً
- ملاحظات الاسترجاع (Rollback): `git checkout -- .opencode/agents/tera-client-engagement.md`
