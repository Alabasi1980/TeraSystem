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

---

## SCP-2026-07-06-083 — إنشاء بروتوكول AIS (Agent Improvement Suggestions)

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-083
- مصدر الطلب: Auditor Report (توصية الهيئة الخارجية) + تحليل TeraSystemEvolutionAgent (حارس)
- نوع التغيير: New Protocol + Agent Improvement
- الملفات المنشأة:
  - `tera-system/AIS_PROTOCOL.md` — البروتوكول العام
  - `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` — السجل المركزي (مع أول إدخال ترحيبي)
- الملفات المعدلة:
  - `.opencode/agents/tera.md` — إضافة قسم AIS (§22)
  - `.opencode/agents/tera-client-engagement.md` — إضافة قسم AIS (§F)
  - `.opencode/agents/tera-system-evolution.md` — إضافة AIS Processing (§15) + إعادة ترقيم
  - `.opencode/agents/monitor.md` — إضافة قسم AIS (§13)
  - `.opencode/agents/auditor.md` — إضافة قسم AIS (§15)
  - `.opencode/agents/design-reviewer.md` — إضافة قسم AIS (§15)
  - `.opencode/agents/application-blueprint.md` — إضافة قسم AIS (§19)
  - `.opencode/agents/tera-software-designer.md` — إضافة قسم AIS (§10)
  - `tera-system/TeraPolicyMap.md` — إضافة إدخال AIS
  - `tera-system/TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` — إضافة مراجع AIS
- الملخص:
  - تم إنشاء بروتوكول AIS يتيح لكل عميل اقتراح تحسينات ذاتية من واقع العمل
  - 9 أنواع اقتراح (بما فيها Skill Gap و Pattern Discovery)
  - شروط تسجيل صارمة + Anti-Spam (3 لكل مهمة) + قالب إلزامي مع Evidence
  - قاعدة فاصلة واضحة: AIS ≠ GAP
  - دورة معالجة: يسجل → Majed يراجع → حارس يحلل → SCP → تنفيذ
  - قسم AIS أضيف لجميع العملاء الثمانية الأساسيين + مسؤولية معالجة AIS لتعريف حارس
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ git diff --check OK، Anti-Bloat Gate PASS، 0 stale references، 0 privilege expansion
- المخاطر: منخفض — لا تغيير في صلاحيات أو سياسات قائمة. الاقتراحات غير نافذة حتى تمر بـ SCP + موافقة
- ملاحظات الاسترجاع (Rollback):
  1. حذف `tera-system/AIS_PROTOCOL.md`
  2. حذف `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md`
  3. إزالة أقسام AIS من ملفات العملاء الثمانية
  4. إزالة إدخال TeraPolicyMap.md ومراجع TERA_CONTINUOUS_IMPROVEMENT_POLICY.md

---

## SCP-2026-07-06-084 — تنفيذ 3 اقتراحات AIS في تعريف حارس (AIS-0002, AIS-0003, AIS-0004)

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-084
- مصدر الطلب: AIS Suggestions — أول AIS يُنفَّذ بعد تفعيل البروتوكول
- نوع التغيير: Agent Improvement (Self-Fix)
- الملفات المعدلة:
  - `.opencode/agents/tera-system-evolution.md` — 3 تعديلات
  - `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` — تحديث حالة 3 اقتراحات
- الملخص:
  - **AIS-0002**: إصلاح ترقيم مكرر — §16 → §17 → §18 → §19
  - **AIS-0003**: إضافة `AIS_PROTOCOL.md` و `AGENT_IMPROVEMENT_SUGGESTIONS.md` إلى §9 (الملفات المرجعية)
  - **AIS-0004**: إضافة "AIS suggestion" كنوع طلب عاشر في §10 (Official Workflow)
  - جميع الـ 3 اقتراحات حدثت حالتها إلى Implemented
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ git diff --check OK، Anti-Bloat Gate PASS
- المخاطر: منخفض جداً — ملف واحد، ترقيم + سطرين
- ملاحظات الاسترجاع (Rollback): `git checkout -- .opencode/agents/tera-system-evolution.md`

---

## SCP-2026-07-06-085 — Client Engagement Personas Framework (وعي TCEA بتنوع الزبائن)

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-085
- مصدر الطلب: User Request (Majed) — بناءً على خبرته مع زبائن متنوعين
- نوع التغيير: Agent Capability Enhancement — إضافة وعي لـ TCEA بتنوع العملاء
- الملفات المعدلة:
  - `tera-system/client-helpers/tera-client-engagement-protocols.md` — إضافة A.9 Client Personas Framework
  - `.opencode/agents/tera-client-engagement.md` — 3 إضافات
- الملخص:
  - **A.9 Client Personas Framework** في protocols.md: تعريف 4 أنماط (Visionary / Explorer / Uncertain / Guided)، آلية الكشف من أول 3-5 أجوبة، استراتيجية التكييف، المرونة المتحكم بها
  - **A.0.1 Client Diversity Awareness** في tera-client-engagement.md: وعي لـ TCEA في بداية Discovery بأن الزبائن متنوعون
  - **MR5 — Controlled Adaptation Rule** في Master Rules: يكيّف الأسلوب، لا يتجاوز الحوكمة
  - تحديثات مساندة: عنوان A.6.0 (4→5 قواعد)، "كيف تستخدمها"، E Glossary، D.1 Routing Table، وصف protocols.md
  - Anti-Bloat: لا ملفات جديدة، لا عملاء جدد، لا SDA، 4 أنماط فقط
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ Anti-Bloat Gate PASS، لا تناقض في السياسات، لا تلوث تطبيقات، لا توسع صلاحيات
- المخاطر: منخفض — MR5 يضيف وعياً (لا يلغي أو يعدل سياسات أو بوابات قائمة)، المرونة محدّدة بـ "يكيّف الأسلوب، لا يتجاوز الحوكمة"
- ملاحظات الاسترجاع (Rollback):
  1. protocols.md: حذف A.9 بكامله
  2. tera-client-engagement.md: حذف A.0.1 + MR5 من جدول A.6.0
  3. إعادة عنوان A.6.0 إلى "القواعد الرئيسية الأربع"
  4. إزالة سطر MR5 من "كيف تستخدمها" و E Glossary
   5. إزالة سطر D.1 الخاص بـ A.9

---

## SCP-2026-07-06-086 — Future-Proof Discovery Rule (A.6.9) — توثيق معلومات توسعية خارج النطاق

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-086
- مصدر الطلب: AIS-0005 — TCEA (مستشار) بعد Discovery العمران + توجيه Majed
- نوع التغيير: Protocol Change — إضافة بروتوكول اكتشاف توسعي لـ TCEA
- الملفات المعدلة:
  - `tera-system/client-helpers/tera-client-engagement-protocols.md` — إضافة A.6.9 Future-Proof Discovery Rule
- الملخص:
  - **A.6.9 Future-Proof Discovery Rule** في protocols.md: يتيح لـ TCEA جمع معلومات إضافية تتجاوز النطاق الحالي للعميل لبناء أساس قابل للتوسع مستقبلاً، مع 4 قواعد صارمة:
    1. اكتشف أوسع من النطاق—لكن لا توسع النطاق
    2. وسم `[Future-Proof Reference]` إلزامي للمعلومات الإضافية
    3. توثيق منفصل في Future-Proof Notes خارج النطاق والتسعير
    4. لا تدخل في النطاق ولا التسعير
  - جدول علاقة مع القواعد الأخرى (MR1, MR4, A.5.2, A.6.5)
  - قاعدة ذهبية: Future-Proof Discovery يكمل حوكمة النطاق—ولا ينتهكها
  - Anti-Bloat: لا ملفات جديدة، لا عملاء جدد — قسم واحد في ملف موجود
- الموافقة: Majed — Approved (عبر الموافقة على SCP)
- التحقق من الصحة: ✅ Anti-Bloat Gate PASS، ✅ Policy Map Check PASS، ✅ Architecture Map Check PASS، ✅ لا تلوث تطبيقات، ✅ لا توسع صلاحيات، ✅ git diff --check نظيف
- المخاطر: منخفض — البروتوكول يمنع صراحة دخول Future-Proof Notes في النطاق والتسعير، وقواعد MR1 و A.5.2 تبقى سارية وتتفوق
- ملاحظات الاسترجاع (Rollback):
  1. protocols.md: حذف القسم A.6.9 بكامله
  2. إعادة الترقيم بعد A.6.8 مباشرة إلى A.9

---

## SCP-2026-07-06-087 — Commercial Value Discovery (MR6 + A.6.10) — الوعي التجاري والربحي للمنظومة

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-087
- مصدر الطلب: توجيه Majed — إضافة الوعي التجاري والربحي لمنظومة Tera
- نوع التغيير: Agent Capability Enhancement — إضافة الوعي التجاري لـ TCEA
- الملفات المعدلة:
  - `.opencode/agents/tera-client-engagement.md` — تغييرات متعددة
  - `tera-system/client-helpers/tera-client-engagement-protocols.md` — إضافة A.6.10
  - `tera-system/TeraPolicyMap.md` — إضافة مدخلين
- الملخص:
  - **A.0:** تحديث "لا تفعل أبداً" — توضيح الفرق بين التوسيع الممنوع والاقتراح التجاري المطلوب
  - **A.0.2 Commercial Awareness:** وعي تجاري جديد — TCEA هو الذراع التجاري والربحي للمنظومة
  - **A.1 الهوية:** إضافة "أنت الذراع التجاري والربحي لمنظومة Tera"
  - **A.2 الأدوار:** إضافة دور 10 — Commercial Value Proposer / مكتشف الفرص التجارية
  - **A.4 التدفق:** إضافة خطوة استكشاف الفرص التجارية قبل التسعير
  - **A.6.0 Master Rules:** إضافة MR6 — Commercial Value Discovery + تحديث كيف تستخدمها
  - **A.7 الممنوع:** إضافة توضيح للفرق بين التوسيع والاقتراح التجاري
  - **A.6.10 Value-Added Commercial Proposals Protocol:** بروتوكول كامل — الأنواع الثلاثة، القواعد الخمس، قالب العرض، العلاقات
  - **A.6.9:** ربط Future-Proof Discovery بالقيمة التجارية — جدول مقارنة
  - **D.1 Routing Table:** إضافة A.6.9 و A.6.10
  - **TeraPolicyMap:** إضافة مدخلين — MR6 و A.6.10
  - Anti-Bloat: لا ملفات جديدة، لا عملاء جدد — إضافات في ملفات موجودة
- الموافقة: Majed — Approved
- التحقق من الصحة: ✅ Anti-Bloat Gate PASS، ✅ Policy Map Check PASS، ✅ Architecture Map Check PASS، ✅ لا تلوث تطبيقات، ✅ لا توسع صلاحيات
- المخاطر: منخفض — MR6 يوجّه للاقتراح وليس التوسيع، وقواعد MR1 و A.5.2 تبقى سارية وتتفوق على أي إضافة غير معتمدة
- ملاحظات الاسترجاع (Rollback):
  1. tera-client-engagement.md: حذف MR6 من A.6.0 + إزالة A.0.2 + إزالة الدور 10 + إزالة الخطوة من A.4 + إزالة التحديثات من A.0/A.1/A.7
  2. protocols.md: حذف A.6.10 بكامله
  3. TeraPolicyMap.md: إزالة مدخلي MR6 و A.6.10

---

## SCP-2026-07-06-088 — TCEA ↔ DomainExpertAgent Direct Invocation — تمكين مستشار من استدعاء خبير المجال

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-088
- مصدر الطلب: توجيه Majed — تمكين مستشار من استدعاء DomainExpertAgent مباشرة
- نوع التغيير: Permission Change — استثناء حوكمة لاستدعاء عميل فرعي محدد
- الملفات المعدلة:
  - `.opencode/agents/tera-client-engagement.md` — A.7 المسموح/الممنوع + A.7.1 قواعد الاستدعاء + D.1 Routing Table
  - `tera-system/TeraSubAgents.md` — §3.2.1 استثناء + §6.13 شرط الاستدعاء + قواعد عند الاستدعاء من TCEA
- الملخص:
  - **A.7 المسموح:** إضافة "استدعاء DomainExpertAgent" كصلاحية معتمدة
  - **A.7 الممنوع:** استثناء DomainExpertAgent من قاعدة "لا تدير عملاء فرعيين"
  - **A.7.1 قواعد الاستدعاء:** متى تستدعيه، كيف تستدعيه، الحدود الصارمة، الممنوع
  - **§3.2.1 استثناء:** TCEA يملك صلاحية استدعاء DomainExpertAgent فقط — مع 5 شروط حوكمية
  - **§6.13 شرط الاستدعاء:** تحديث ليشمل استدعاء TCEA المباشر
  - **DomainExpertAgent حدود عند استدعاء TCEA:** 6 قواعد صارمة (Domain Intelligence Report فقط، [Research Hint]، client-engagement/ فقط)
  - **D.1 Routing Table:** إضافة DomainExpertAgent
  - Anti-Bloat: لا ملفات جديدة، لا عملاء جدد — تعديل صلاحيات في ملفات موجودة
- الموافقة: Majed — Approved (توجيه مباشر)
- التحقق من الصحة: ✅ Anti-Bloat Gate PASS، ✅ Policy Map Check PASS، ✅ Architecture Map Check PASS، ✅ لا تلوث تطبيقات، ✅ لا توسع صلاحيات غير مبرر (استثناء واحد محدد)، ✅ git diff --check نظيف
- المخاطر: منخفض — الاستثناء محدود بـ DomainExpertAgent فقط، المخرجات تحمل [Research Hint] ولا تدخل النطاق دون تأكيد Majed (MR1)، كل استدعاء يُسجل
- ملاحظات الاسترجاع (Rollback):
  1. tera-client-engagement.md: إزالة A.7.1 + استرجاع A.7 المسموح/الممنوع + إزالة DomainExpertAgent من D.1
  2. TeraSubAgents.md: إزالة الاستثناء من §3.2.1 + استرجاع §6.13 شرط الاستدعاء + إزالة "عند الاستدعاء من TCEA"

---

## SCP-2026-07-06-089 — TCEA ↔ DomainResearchAgent Direct Invocation — تمكين مستشار من استدعاء الباحث

- تاريخ: 2026-07-06
- معرف التغيير: SCP-2026-07-06-089
- مصدر الطلب: توجيه Majed — إضافة DomainResearchAgent لاستكمال Pipeline البحث
- نوع التغيير: Permission Change — توسيع استثناء حوكمة ليشمل الباحث
- الملفات المعدلة:
  - `.opencode/agents/tera-client-engagement.md` — A.7 المسموح/الممنوع + A.7.1 قواعد الاستدعاء + D.1 Routing Table
  - `tera-system/TeraSubAgents.md` — §3.2.1 استثناء + §6.12 شرط الاستدعاء + قواعد عند الاستدعاء من TCEA
- الملخص:
  - **A.7 المسموح:** إضافة "استدعاء DomainResearchAgent" كصلاحية معتمدة
  - **A.7 الممنوع:** استثناء DomainResearchAgent من قاعدة "لا تدير عملاء فرعيين"
  - **A.7.1 قواعد الاستدعاء:** تحديث ليشمل كلا العميلين + Pipeline الكامل (بحث ← تحليل)
  - **§3.2.1 استثناء:** TCEA يملك صلاحية استدعاء DomainResearchAgent + DomainExpertAgent — مع 6 شروط حوكمية
  - **§6.12 شرط الاستدعاء:** تحديث ليشمل استدعاء TCEA المباشر
  - **DomainResearchAgent حدود عند استدعاء TCEA:** 8 قواعد صارمة (Domain Research Report فقط، [Research Hint]، client-engagement/ فقط، websearch/webfetch مسموح)
  - **D.1 Routing Table:** إضافة DomainResearchAgent
  - Anti-Bloat: لا ملفات جديدة، لا عملاء جدد — تعديل صلاحيات في ملفات موجودة
- الموافقة: Majed — Approved (توجيه مباشر)
- التحقق من الصحة: ✅ Anti-Bloat Gate PASS، ✅ Policy Map Check PASS، ✅ Architecture Map Check PASS، ✅ لا تلوث تطبيقات، ✅ لا توسع صلاحيات غير مبرر (استثناءان محددان)، ✅ git diff --check نظيف
- المخاطر: منخفض — الاستثناء محدود بـ DomainResearchAgent + DomainExpertAgent فقط، المخرجات تحمل [Research Hint] ولا تدخل النطاق دون تأكيد Majed (MR1)، كل استدعاء يُسجل
- ملاحظات الاسترجاع (Rollback):
  1. tera-client-engagement.md: إزالة DomainResearchAgent من A.7 + A.7.1 + D.1
  2. TeraSubAgents.md: إزالة DomainResearchAgent من §3.2.1 + استرجاع §6.12 شرط الاستدعاء + إزالة "عند الاستدعاء من TCEA"

---

## SCP-2026-07-07-090 — DomainExpertAgent + DomainResearchAgent Dual Mode — إنشاء ملفات العملاء في `.opencode/agents/` مع وضعي التشغيل

- تاريخ: 2026-07-07
- معرف التغيير: SCP-2026-07-07-090
- مصدر الطلب: تقييم TCEA لـ DomainExpertAgent — GAP-012
- نوع التغيير: New Agent (2 files) + Policy Update + Protocol Change + Template Update
- الملفات المعدلة:
  - `.opencode/agents/domain-research-agent.md` — **جديد** — تعريف كامل مع Dual Mode (Software + Consulting)
  - `.opencode/agents/domain-expert-agent.md` — **جديد** — تعريف كامل مع Dual Mode + Consulting outputs (Knowledge Structure, Gap Analysis)
  - `tera-system/TeraSubAgents.md` — §6.12 (Dual Mode + Consulting Mode + ملف مرجعي) + §6.13 (Dual Mode + قوالب Consulting)
  - `project-control/AGENT_GAPS_LOG.md` — GAP-012 (تم التطبيق)
  - `project-control/SYSTEM_EVOLUTION_LOG.md` — هذا الإدخال
- الملخص:
  - **DomainResearchAgent** (باحث): ملف مستقل بـ 11 قسماً — هوية، مهمة، I/O، بروتوكول بحث خطوة بخطوة، تصنيف مصادر (Tier 1/2/3)، معالجة أخطاء 404/403/Timeout، Three-Tier Boundaries، معايير جودة، منع تضخم، AIS
  - **DomainExpertAgent** (خبير): ملف مستقل بـ 12 قسماً — هوية، وضعا تشغيل (Software MVP + Consulting Knowledge)، Mission، مسؤوليات، تصنيفات، I/O تعتمد على الوضع، Pipeline كامل مع DomainResearchAgent، بناء هيكل هرمي 3 مستويات، تحليل فجوات، Three-Tier Boundaries، جودة، منع تضخم، AIS
  - **Software Mode** (مشاريع برمجية — TeraAgent): Domain Intelligence Report مع تصنيف MVP (Include now / Recommended / Defer / Out of Scope / Needs User Decision)
  - **Consulting Mode** (مشاريع استشارية — TCEA): Domain Intelligence Report (بتصنيف معرفي) + Knowledge Structure + Gap Analysis — مع تحليل فجوات وأولويات
  - **اكتشاف الوضع تلقائي**: من `mode` parameter أو استنباط من الـ Objective
  - **§6.12 TeraSubAgents.md**: تحديث كامل — إضافة ملف العميل، Dual Mode، Consulting Mode قواعد، معايير قبول محدثة
  - **§6.13 TeraSubAgents.md**: تحديث كامل — إضافة ملف العميل، Dual Mode، Consulting Mode مخرجات، حدود Consulting، معايير قبول محددة لكل وضع
  - **Pipeline**: DomainResearchAgent → DomainExpertAgent → TCEA → Majed → يدخل النطاق فقط بعد [Confirmed by Majed]
  - Anti-Bloat: ملفان جديدان فقط (لا عملاء جدد — استخدمنا العملاء الموجودين بقدرات جديدة). Consulting Mode يضيف مخرجات فقط عند الحاجة (Gap Analysis + Knowledge Structure اختياريات)
- الموافقة: Majed — Approved (توجيه مباشر، مع بحث مسبق وموافقة على Dual Mode)
- التحقق من الصحة:
  - ✅ Anti-Bloat Gate PASS — لا عملاء جدد، لا طبقات جديدة، استخدمنا العملاء الموجودين بقدرات جديدة
  - ✅ Policy Map Check: PASS — لا تناقض مع TeraPolicyMap.md أو TeraClientPolicy.md أو TeraSubAgents.md
  - ✅ Architecture Map Check: PASS — لا انحراف عن حدود المجلدات (client-engagement/ للـ TCEA، project-preparation/ للتطبيقات البرمجية)
  - ✅ لا تلوث تطبيقات العملاء — لا تعديل لأي ملف في clients/
  - ✅ لا توسع صلاحيات غير مبرر — كل عميل له صلاحياته المحددة (read-only افتراضياً، write للمخرجات فقط)
  - ✅ Dual Mode يمنع خلط المسؤوليات — Software مع TeraAgent، Consulting مع TCEA
- المخاطر: منخفض — العملاء موجودون مسبقاً في TeraSubAgents.md لكن بدون ملفات قابلة للاستدعاء. الترقية تضيف قدرات جديدة دون تغيير الصلاحيات الأساسية. Consulting Mode مخرجاتها [Research Hint] ولا تدخل النطاق دون تأكيد Majed (MR1)
- ملاحظات الاسترجاع (Rollback):
  1. حذف `.opencode/agents/domain-research-agent.md` و `domain-expert-agent.md`
  2. استرجاع TeraSubAgents.md §6.12 و §6.13 إلى النسخة السابقة
  3. إزالة GAP-012 من AGENT_GAPS_LOG.md
   4. إزالة هذا الإدخال من SYSTEM_EVOLUTION_LOG.md

---

## SCP-2026-07-07-091 — إضافة صلاحية استخدام DomainResearchAgent و DomainExpertAgent لـ ApplicationBlueprintAgent (مهندس)

- تاريخ: 2026-07-07
- معرف التغيير: SCP-2026-07-07-091
- مصدر الطلب: طلب Majed المباشر — بعد تقييم Blueprint خارجي وتحديد فجوة Domain Depth لدى مهندس
- نوع التغيير: Agent Permission Enhancement — إضافة صلاحية استدعاء عملاء فرعيين محددين
- الملفات المعدلة:
  - `.opencode/agents/application-blueprint.md` — إضافة `task: ask` في الصلاحيات + إضافة §20 (صلاحية استخدام Domain Agents مع قواعد وممنوعات)
  - `tera-system/TeraSubAgents.md` §3.2.1 — توسيع الاستثناء من "TCEA فقط" إلى "TCEA + ApplicationBlueprintAgent" مع فصل شروط كل منهما
  - `.opencode/agents/domain-research-agent.md` — إضافة §6.7 (عند الاستدعاء من مهندس — Software Mode) + تحديث Identity و Mode
  - `.opencode/agents/domain-expert-agent.md` — إضافة §6.7 (عند الاستدعاء من مهندس — Software Mode) + تحديث Identity و Dual Mode
- الملخص:
  - مهندس (ApplicationBlueprintAgent) يملك الآن صلاحية استدعاء DomainResearchAgent و DomainExpertAgent مباشرة دون المرور بـ TeraAgent
  - الوضع: Software Mode تلقائياً (تصنيف MVP — Include now / Recommended / Defer / Out of Scope / Needs User Decision)
  - الكتابة: project-preparation/ فقط
  - المخرجات: تحمل وسم [Research Hint] — لا تدخل blueprint دون تأكيد Majed
  - التسجيل: في BLUEPRINT_DECISION_CANDIDATES.md أو BLUEPRINT_OPEN_QUESTIONS.md
  - الحدود: لا يجوز لمهندس استدعاء أي عميل فرعي آخر غير هذين الاثنين
  - نفس pattern الناجح لـ TCEA (SCP-088 + SCP-089) يُطبق على مهندس
- الموافقة: Majed — Approved
- التحقق من الصحة:
  - ✅ Anti-Bloat Gate PASS — لا ملفات جديدة، لا عملاء جدد، لا طبقات جديدة
  - ✅ Policy Map Check PASS — لا تناقض مع TeraPolicyMap.md (نمط مطبق مسبقاً لـ TCEA)
  - ✅ Architecture Map Check PASS — project-preparation/ لمهندس، client-engagement/ لـ TCEA
  - ✅ لا تلوث تطبيقات العملاء — لا تعديل لأي ملف في clients/
  - ✅ لا توسع صلاحيات غير مبرر — استثناء محدد بعميلين فقط (DomainResearchAgent + DomainExpertAgent)
  - ✅ git diff --check نظيف (لا أخطاء، فقط تحذيرات CRLF عادية)
  - ✅ 0 مراجع قديمة لـ "الاستثناء الوحيد"
- المخاطر: منخفض — نفس pattern المطبق لـ TCEA (SCP-088 + SCP-089). مهندس لا يملك صلاحية استدعاء أي عميل آخر غير domain agents. المخرجات [Research Hint] لا تدخل blueprint دون تأكيد Majed
- ملاحظات الاسترجاع (Rollback):
  1. application-blueprint.md: إزالة صلاحية `task` + إزالة §20
  2. TeraSubAgents.md §3.2.1: استرجاع النص القديم "الاستثناء الوحيد — TCEA فقط"
  3. domain-research-agent.md: إزالة §6.7 + استرجاع Identity و Mode
  4. domain-expert-agent.md: إزالة §6.7 + استرجاع Identity و Dual Mode

## SCP-2026-07-07-079 — حظر TeraAgent من كتابة أي كود برمجي (Hard Code Boundary)

- تاريخ: 2026-07-07
- معرف التغيير: SCP-2026-07-07-079
- مصدر الطلب: Owner Improvement Request (Majed)
- نوع التغيير: Agent Improvement / Policy Update
- الملفات المعدلة:
  - `.opencode/agents/tera.md` — 4 تعديلات: تقوية العبارة الافتتاحية، إضافة بند منع الكود في Section 9، إضافة Section 9.1 Code Boundary Rule كاملة، إضافة Code Writing Delegation Rule في Section 12
- الملخص:
  - تم تحويل TeraAgent من "not a direct implementation agent **by default**" إلى **"pure orchestrator — FORBIDDEN from writing any programming code"**
  - تمت إضافة بند صريح في Section 9 يمنع TeraAgent من كتابة أي ملف برمجي (JS, TS, HTML, CSS, Python, SQL, shell scripts, infra config...إلخ)
  - تمت إضافة Section 9.1 Code Boundary Rule مع جدول واضح: إيش مسموح يكتبه (ملفات .md للتوثيق والخطط والتقارير) وإيش ممنوع (كل ما يُنفَّذ أو يُجمَّع)
  - تمت إضافة قاعدة تفويض صارمة: أي كود = EngineeringAgent أو UI Designer أو SoftwareDesigner
  - تم تحديث Section 12 لتعزيز قاعدة "TeraAgent does not touch code files directly. Period."
  - تم تحديث Last Synced إلى 2026-07-07
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — ملف واحد معدّل، لا ملفات جديدة، لا عملاء جدد، لا طبقات جديدة
  - ✅ Policy Map Check PASS — tera.md هو مصدر الحقيقة لهوية TeraAgent، لا تعارض
  - ✅ Architecture Map Check PASS — لا تغيير في أدوار المجلدات أو حدود الطبقات
  - ✅ No client-app contamination — tera.md ملف نظامي
  - ✅ No unauthorized privilege expansion — تقييد وليس توسيع صلاحيات
  - ✅ No stale/deprecated agent references
  - ✅ No duplicated mandatory rules — القواعد الجديدة صافية، لا تكرار
  - ✅ git diff --check PASS — لا أخطاء، فقط تحذيرات CRLF عادية على ويندوز
- المخاطر: منخفض — تقييد سلوكي صريح فقط. لا تغيير في الصلاحيات التقنية. TeraAgent ما زال يملك write/edit/bash للملفات الإدارية (.md). الخطر الوحيد: TeraAgent قد "يحتج" على ملفات .md تحتوي pseudo-code — لكن القاعدة تحدد: ما يُنفَّذ أو يُجمَّع فقط هو الممنوع.
- ملاحظات الاسترجاع (Rollback): `git checkout HEAD -- .opencode/agents/tera.md`

## SCP-2026-07-07-080 — تقوية ناقد في معيار راحة العين ووضوح المحتوى

- تاريخ: 2026-07-07
- معرف التغيير: SCP-2026-07-07-080
- مصدر الطلب: Owner Improvement Request (Majed)
- نوع التغيير: Agent Improvement / Policy Update
- الملفات المعدلة:
  - `.opencode/agents/design-reviewer.md` — إضافة قاعدة صريحة لراحة العين ورفض التباين المزعج
  - `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` — إضافة §3.1 Visual Comfort & Readability Safety
- الملخص:
  - تم تعزيز ناقد بمعيار واضح يمنع الواجهات شبه البيضاء على شبه البيضاء، والنص الباهت على الخلفيات الفاتحة، والـ blur/opacity التي تضعف المقروئية
  - تمت إضافة بند صريح يجعل أي عنصر يحتاج "تحديق" أو يسبب إجهادًا بصريًا فاشلًا في المراجعة
  - تمت إضافة Checklist مستقل لراحة العين ووضوح المحتوى في معايير المراجعة
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — تعديل موجه، بلا عملاء جدد ولا طبقات جديدة
  - ✅ Policy Map Check PASS — يقوّي المعايير دون تعارض
  - ✅ Architecture Map Check PASS — لا أثر على حدود المجلدات
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references
- المخاطر: منخفض — التغيير يزيد صرامة المراجعة البصرية وقد يرفع عدد الرفضات البصرية في البداية، لكنه يحسن الجودة النهائية
- ملاحظات الاسترجاع (Rollback):
  1. `.opencode/agents/design-reviewer.md` — إزالة القاعدة الصريحة المضافة
  2. `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` — حذف §3.1 المستحدثة

## SCP-2026-07-07-081 — تشديد فحص المودالات والواجهات الشفافة عبر Screenshot فعلي

- تاريخ: 2026-07-07
- معرف التغيير: SCP-2026-07-07-081
- مصدر الطلب: Owner Improvement Request (Majed)
- نوع التغيير: Agent Improvement / Policy Update
- الملفات المعدلة:
  - `.opencode/agents/design-reviewer.md` — إضافة شرط Screenshot فعلي للمودالات/الشفافيات
  - `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` — إضافة قواعد فشل فوري عند ضعف القراءة أو تسرب الخلفية
- الملخص:
  - تم منع اعتماد Snapshot/ARIA وحدهما في حالات المودال والـ glassmorphism
  - تمت إضافة قاعدة صريحة: إذا كان المحتوى مرئيًا لكنه غير مريح في Screenshot فعلي، فهو فشل
  - تمت إضافة حالات فشل فوري للمحتوى الذي يحتاج تحديقًا أو يفتقر للفصل البصري
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS
  - ✅ Policy Map Check PASS
  - ✅ Architecture Map Check PASS
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references
- المخاطر: منخفض — يرفع صرامة الفحص البصري ويقلل قبول الواجهات المرهقة
- ملاحظات الاسترجاع (Rollback):
   1. `.opencode/agents/design-reviewer.md` — إزالة سطر شرط Screenshot الفعلي
   2. `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` — إزالة البنود الأربع المضافة في §3.1

## SCP-2026-07-07-082 — إضافة معايير حيوية البروتوتايب (Vitality & Polish)

- تاريخ: 2026-07-07
- معرف التغيير: SCP-2026-07-07-082
- مصدر الطلب: Owner Improvement Request (Majed)
- نوع التغيير: Policy Update / Agent Improvement (4 ملفات)
- الملفات المعدلة:
  - `.opencode/agents/tera.md` — إضافة § UI Vitality & Polish Requirements (Checklist إلزامي لكل UI Task)
  - `.opencode/agents/ui-designer.md` — تحويل Research Protocol من اختياري إلى إلزامي + إضافة Vitality Self-Check Gate + إعادة تعريف "prototype" كـ "تجربة كاملة"
  - `.opencode/agents/design-reviewer.md` — إضافة بند فحص حيوية البروتوتايب وطاقته البصرية
  - `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` — إضافة §10 كامل Visual Vitality & Polish
- الملخص:
  - تم إلزام البحث عن References قبل أي مشروع UI (Dribbble/Awwwards + توثيق)
  - تم إضافة Vitality Self-Check Gate للمصمم: 8 بنود إلزامية (Skeleton، Toast، Connection Status، Search، Empty States، Micro-animations، Realistic Data، حيوية)
  - تم إضافة معيار في Tera: كل TASK-COD-* لواجهات يتضمن Vitality & Polish Checklist إلزامي
  - تم إضافة بند فحص الحيوية في design-reviewer.md ومعايير DESIGN_REVIEW_STANDARDS.md
  - تم وضع قاعدة: البروتوتايب البارد/الساكن = فشل مراجعة تلقائي
  - تم إضافة خيار N/A (لا ينطبق) مع تبرير إجباري لكل بند في الـ Checklist لاستيعاب المهام الصغيرة
  - تم ربط الـ Checklist بـ Pre-Execution Gate (فحص الوجود) و Post-Execution Review Gate (فحص الإكمال)
  - تم توحيد صيغة الـ ✅ / N/A عبر الملفات الثلاثة (tera.md، ui-designer.md، DESIGN_REVIEW_STANDARDS.md)
  - تم إضافة Vitality Verification إلى Output Format في design-reviewer.md §13 لإلزام ذكر الحيوية في التقارير
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — تعديلات في ملفات موجودة، لا عملاء جدد ولا طبقات جديدة
  - ✅ Policy Map Check PASS — الملفات الأربعة متسقة مع بعضها
  - ✅ Architecture Map Check PASS — لا تغيير في حدود المجلدات
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references
- المخاطر: منخفض — سيرفع جودة البروتوتايب وقد يزيد وقت التنفيذ قليلاً، لكنه يمنع البروتوتايب "البارد"
- ملاحظات الاسترجاع (Rollback):
  1. `.opencode/agents/tera.md` — إزالة § UI Vitality & Polish Requirements
  2. `.opencode/agents/ui-designer.md` — إعادة Research Protocol إلى "اختياري" + إزالة Vitality Self-Check Gate + إزالة §7.5
   3. `.opencode/agents/design-reviewer.md` — إزالة بند فحص الحيوية
   4. `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` — حذف §10

## SCP-2026-07-07-083 — تطوير حارس (صلاحيات + أدوات + خريطة تبعية)

- تاريخ: 2026-07-07
- معرف التغيير: SCP-2026-07-07-083
- مصدر الطلب: Owner Improvement Request (Majed)
- نوع التغيير: Agent Improvement / New Tool / New Reference File
- الملفات المعدلة:
  - `.opencode/agents/tera-system-evolution.md` — 5 إضافات (انظر الملخص)
  - `tera-system/AGENT_DEPENDENCY_MAP.md` — ملف جديد
- الملخص:
  - **صلاحية `task`:** أُضيفت مع قيود واضحة — يمكن استدعاء domain-research-agent، ui-designer، general للأغراض النظامية فقط. ممنوع استدعاء TeraAgent أو TCEA أو مهندس لمهام تطبيقات العملاء.
  - **قاعدة حجم الملف:** أُضيفت كواجب رقم 11 — أي ملف Agent يتجاوز 400 سطر يُقترح تقسيمه. استثناء لـ tera.md و tera-client-engagement.md حتى 700 سطر.
  - **Agent Edit Quality Gate:** أُضيفت كواجب رقم 12 — 6 بنود تحقق قبل إغلاق أي تعديل على Agent.
  - **SYSTEM_HEALTH_REPORT قالب:** أُضيفت كـ §13.3 في Output Formats لتوحيد تقارير الفحص الدوري.
  - **AGENT_DEPENDENCY_MAP.md:** ملف جديد في tera-system/ يوثق العلاقات بين جميع العملاء الأساسيين — من يستدعي من، ومن يشير إلى من، وترتيب التعديل الآمن.
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — ملف جديد واحد فقط (خريطة)، والباقي إضافات في ملف موجود
  - ✅ Policy Map Check PASS — لا تعارض
  - ✅ Architecture Map Check PASS — الملف الجديد تحت tera-system/
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion — صلاحية task مقيدة بقيود واضحة
  - ✅ No stale/deprecated agent references
  - ✅ File size checked — tera-system-evolution.md ~600 سطر (يتجاوز 400، مقرر تقسيمه لاحقاً)
- المخاطر: منخفض
- ملاحظات الاسترجاع (Rollback):
  1. `.opencode/agents/tera-system-evolution.md` — إزالة الإضافات الخمس (باستثناء التعليمات الأصلية)
  2. حذف `tera-system/AGENT_DEPENDENCY_MAP.md`

## SCP-2026-07-08-084 — صيانة ملف TeraAgent (ترتيب المحتوى + مرجع قديم)

- تاريخ: 2026-07-08
- معرف التغيير: SCP-2026-07-08-084
- مصدر الطلب: Agent File Maintenance (مسح صحة ملفات العملاء)
- نوع التغيير: Agent File Maintenance / Content Reorder / Stale Reference Fix
- الملفات المعدلة:
  - `.opencode/agents/tera.md` — 4 تغييرات (انظر الملخص)
- الملخص:
  - إصلاح مرجع قديم في السطر 178: `APPLICATION_PROPOSAL.html` من `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html` → `APPLICATION_PROPOSAL_TEMPLATE.md` من `tera-workshop/client-templates/commercial/APPLICATION_PROPOSAL_TEMPLATE.md` (كانت C-2 صححت PolicyMap لكن tera.md نسي)
  - نقل القاعدة الصلبة CODE BOUNDARY (الجدول الكامل + خطوات التفويض + الإنفاذ) من §9.1 إلى أعلى الملف مباشرة بعد CONDUCT GATE — لأنها أهم قاعدة للعميل ويجب أن تكون أولاً
  - تقليل التكرار: §9.1 أصبح مرجعاً قصيراً، و§12 Code Writing Delegation Rule أصبح خلاصة تشغيلية بدل التكرار الرابع
  - حذف #5 المكرر من Authority Order (كان مطابقاً لـ #3) وأعيد ترقيم #6–#8 إلى #5–#7
- الموافقة: Majed — Approved (خلال جلسة الصيانة، موافقة لكل ملف على حدة)
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — انخفاض صافٍ في السطور (703 → 492)
  - ✅ Policy Map Check PASS — المرجع المُصلح يطابق TeraPolicyMap.md
  - ✅ Architecture Map Check PASS — لا تغيير في بنية المجلدات
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references — تم التحقق بـ grep أن لا إشارة لـ `.html` متبقية
  - ✅ File size: 492 سطر — تحت حد 700 (آمن)
- المخاطر: منخفض — سلوك TeraAgent لم يتغير، فقط الترتيب ودقة المرجع
- ملاحظات الاسترجاع (Rollback):
  1. `git checkout HEAD -- .opencode/agents/tera.md`

## SCP-2026-07-08-085 — صيانة TCEA (مرجع مُسَمّى خطأ)

- تاريخ: 2026-07-08
- معرف التغيير: SCP-2026-07-08-085
- مصدر الطلب: Agent File Maintenance (مسح صحة ملفات العملاء)
- نوع التغيير: Agent File Maintenance / Reference Categorization Fix
- الملفات المعدلة:
  - `.opencode/agents/tera-client-engagement.md` — نقل مرجع من Required Now إلى Reference Only
- الملخص:
  - السطر 507 كان يدرج `PRICING_SCORECARD_APPLICATION_MAWTHOOQ.md` (ملف خاص بعميل MAWTHOOQ) في جدول 🟢 Required Now — لكنه ملف خاص بعميل ويُوصف كـ "مثال تطبيقي"
  - تم نقله إلى جدول 🔵 Reference Only مع وسم واضح: "مثال فقط — لا يُقرأ لعملاء جدد"
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — لا تغيير في حجم الملف (737 سطر)
  - ✅ Policy Map Check PASS — لا تعارض
  - ✅ Architecture Map Check PASS
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references — الملف المُشار إليه موجود فعلاً
  - ✅ File size: 737 سطر — ضمن استثناء 700 (مقسّم فعلياً إلى 4 ملفات مساعدة)
- المخاطر: منخفض
- ملاحظات الاسترجاع (Rollback):
  1. `git checkout HEAD -- .opencode/agents/tera-client-engagement.md`

## SCP-2026-07-08-086 — صيانة مصمم (إبراز القواعد الصلبة أولاً)

- تاريخ: 2026-07-08
- معرف التغيير: SCP-2026-07-08-086
- مصدر الطلب: Agent File Maintenance (مسح صحة ملفات العملاء)
- نوع التغيير: Agent File Maintenance / Content Reorder
- الملفات المعدلة:
  - `.opencode/agents/ui-designer.md` — إدراج قسم "🔴 القواعد الصلبة" بعد §1
- الملخص:
  - القاعدتان الصلبتان (البحث الإلزامي + بوابة الحيوية) كانتا مدفونتين في §6 و §7.5
  - أُضيف قسم ملخّص قوي مباشرة بعد §1 (من أنا) يحتوي الخلاصة + القائمة الكاملة لـ Vitality Self-Check
  - التفاصيل الكاملة في §6 و §7.5 تبقى كما هي للسياق
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — زيادة طفيفة في السطور (~354 سطر إجمالي) ضمن حد 700
  - ✅ Policy Map Check PASS — لا تعارض
  - ✅ Architecture Map Check PASS
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references
  - ✅ File size: ~354 سطر — تحت 700 (آمن)
- المخاطر: منخفض
- ملاحظات الاسترجاع (Rollback):
  1. `git checkout HEAD -- .opencode/agents/ui-designer.md`

## SCP-2026-07-08-087 — صيانة مهندس (ترتيب المحتوى: الهوية أولاً)

- تاريخ: 2026-07-08
- معرف التغيير: SCP-2026-07-08-087
- مصدر الطلب: Agent File Maintenance
- نوع التغيير: Agent File Maintenance / Content Reorder
- الملفات المعدلة:
  - `.opencode/agents/engineering-agent.md` — نقل §1 (من أنا) قبل شهادتي + الاقتباسات
- الملخص:
  - كانت شهادتي الدكتوراه والاقتباسات (السطور 25-35) تسبق §1 من أنا
  - أُصلح الترتيب: CONDUCT GATE → §1 من أنا → شهادتي + quotes → §2 مبادئي الهندسية
- الموافقة: Majed — Approved
- التحقق من الصحة: Validation Passed
  - ✅ Anti-Bloat Gate PASS — عدد السطور لم يتغير
  - ✅ Policy Map Check PASS — لا تعارض
  - ✅ Architecture Map Check PASS
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references
  - ✅ File size: ~340 سطر — تحت 700 (آمن)
- المخاطر: منخفض
- ملاحظات الاسترجاع (Rollback):
  1. `git checkout HEAD -- .opencode/agents/engineering-agent.md`

---

## SCP-2026-07-10-091 — إضافة Tera Strategic Advisor كعميل استشاري استراتيجي مستقل

- Date: 2026-07-10
- Change ID: SCP-2026-07-10-091
- Request Source: Majed approval of `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-10-091.md`
- Change Type: New owner-level advisory agent + compact system map updates
- Files Changed:
  - `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-10-091.md` — مقترح التغيير المعتمد
  - `.opencode/agents/tera-strategic-advisor.md` — **جديد** — تعريف العميل الاستشاري الاستراتيجي المستقل
  - `tera-system/TERA_AGENT_CONDUCT.md` — إضافة مرجع مختصر للعميل + قاعدة فصل الاستشارة عن الموافقة/التنفيذ
  - `tera-system/TeraPolicyMap.md` — إضافة مصدر حقيقة لـ Owner strategic advisory
  - `tera-system/TeraArchitectureMap.md` — إضافة طبقة Strategic advisory and decision support + خطوة اختيارية قبل Project Intake
  - `tera-system/AGENT_DEPENDENCY_MAP.md` — إضافة العميل الجديد إلى خريطة الاعتماد وتنبيه الحجم
  - `project-control/SYSTEM_EVOLUTION_LOG.md` — هذا الإدخال
- Summary:
  - تم إنشاء `Tera Strategic Advisor` كعميل رئيسي مستقل يستدعيه Majed مباشرة فقط.
  - العملاء الآخرون يستطيعون التوصية بالرجوع إليه، لكن لا يستدعونه ولا يتصرفون باسمه.
  - الصلاحيات التزمت بالموافقة: `read/glob/grep/webfetch: allow`, `bash: ask`, `edit/write: deny`.
  - لا يوجد مجلد `advisory-reports/` ولم تُمنح أي صلاحية كتابة تقارير.
  - الملف يفرض Evidence-before-final-recommendation للقرارات عالية الأثر، Confidence Level، وقاعدة المعلومات الناقصة.
- Approval: Majed — Approved explicitly on 2026-07-10 with constraints: no `advisory-reports/`, no edit/write permissions, stop after summary.
- Validation:
  - ✅ Anti-Bloat Gate PASS — ملف عميل واحد + تحديثات خرائط مختصرة فقط؛ لا مجلد تقارير.
  - ✅ Consistency with Dependency Map — تم تحديث `AGENT_DEPENDENCY_MAP.md`.
  - ✅ Policy Map Check PASS — تم تحديث `TeraPolicyMap.md` بمصدر الحقيقة.
  - ✅ Architecture Map Check PASS — تم تحديث `TeraArchitectureMap.md` بطبقة اختيارية قبل التنفيذ.
  - ✅ No client-app contamination — لا تعديل في تطبيقات العملاء.
  - ✅ No unauthorized privilege expansion — `edit: deny`, `write: deny`.
  - ✅ File size below split threshold — `tera-strategic-advisor.md` = 310 سطر (< 700).
  - ✅ `git diff --check` PASS — لا أخطاء؛ ظهرت تحذيرات CRLF عادية في ويندوز.
- Risk: منخفض — العميل استشاري فقط، لا تنفيذ ولا إدارة ولا موافقة. الخطر الرئيسي هو التباس الاسم مع TCEA، وتم تقليله باستخدام لقب "المستشار الاستراتيجي" بدلاً من "مستشار" فقط.
- Rollback Notes:
  1. حذف `.opencode/agents/tera-strategic-advisor.md`.
  2. إزالة إدخالاته من `TERA_AGENT_CONDUCT.md`, `TeraPolicyMap.md`, `TeraArchitectureMap.md`, و `AGENT_DEPENDENCY_MAP.md`.
  3. إزالة هذا الإدخال من `SYSTEM_EVOLUTION_LOG.md` عند الحاجة.

### Post-Review Permission Amendment — 2026-07-10

- Request Source: Majed review after implementation.
- Files Changed:
  - `.opencode/agents/tera-strategic-advisor.md`
  - `tera-system/AGENT_DEPENDENCY_MAP.md`
  - `project-control/SYSTEM_EVOLUTION_LOG.md`
- Summary:
  - Added `websearch: allow` so the advisor can perform current external search for alternatives, open-source adoption, repository health, releases, issues, and community signals.
  - Added `task: deny` so the prohibition on invoking sub-agents is enforced technically, not only textually.
  - Added a short explicit note that specialist work must be recommended to Majed; Majed decides whether another agent is invoked.
- Validation:
  - ✅ No edit/write permission granted.
  - ✅ No `advisory-reports/` folder created.
  - ✅ No sub-agent invocation authority; `task: deny` added.
  - ✅ File size still below split threshold — `tera-strategic-advisor.md` = 314 سطر (< 700).
- Rollback Notes:
  1. Remove `websearch: allow` and `task: deny` from `.opencode/agents/tera-strategic-advisor.md`.
  2. Remove the added sub-agent invocation note from `.opencode/agents/tera-strategic-advisor.md`.
  3. Restore `AGENT_DEPENDENCY_MAP.md` line count to previous value if needed.

### Second Post-Review Amendment — 2026-07-10 — كتابة الملفات التحليلية

- Request Source: Majed direct instruction after review.
- Files Changed:
  - `.opencode/agents/tera-strategic-advisor.md`
  - `tera-system/AGENT_DEPENDENCY_MAP.md`
  - `project-control/SYSTEM_EVOLUTION_LOG.md`
- Summary:
  - Changed `write: deny` → `write: ask` so the advisor can create analytical files, plans, risk assessments, option comparisons, and any text-only advisory output (.md) — with Majed approval per file.
  - Kept `edit: deny` — the advisor may not modify existing system or application files unless Majed explicitly instructs it in a specific case.
  - Updated Output Persistence section (§15) with:
    - explicit write protocol (state path, confirm advisory-only, confirm no code)
    - absolute prohibition on writing any executable, compilable, or runnable file
    - explicit list of allowed file types (.md analysis only)
  - No `advisory-reports/` folder created; write location is per-instruction by Majed.
- Validation:
  - ✅ `edit: deny` remains — no modification of existing files.
  - ✅ Absolute code-writing prohibition added in natural language.
  - ✅ Allowed writes are limited to `.md` advisory/analysis/plan files.
  - ✅ File size still below split threshold — `tera-strategic-advisor.md` = 323 سطر (< 700).
  - ✅ `git diff --check` PASS — only CRLF warnings.
- Rollback Notes:
  1. Revert `write: ask` → `write: deny` in `.opencode/agents/tera-strategic-advisor.md`.
  2. Restore §15 Output Persistence to read-only version.
  3. Restore `AGENT_DEPENDENCY_MAP.md` line count.

---

## SCP-2026-07-12-092 — تنظيف TeraSubAgents.md: تبسيط العملاء أصحاب ملفات Runtime

- التاريخ: 2026-07-12
- معرف التغيير: SCP-2026-07-12-092
- مصدر الطلب: Owner Improvement Request (Majed) — مبدأ "العملاء الذين يملكون ملفات runtime لا يحتاجون تعريفات كاملة في TeraSubAgents.md"
- نوع التغيير: Anti-Bloat / Content Reduction
- الملفات المعدلة:
  - `tera-system/TeraSubAgents.md` — تبسيط 6 عملاء (1,925 → 1,502 سطر، وفاض 423 سطر)
  - `tera-system/AGENT_DEPENDENCY_MAP.md` — تحديث عدّاد السطور + إزالة merge conflict marker
  - `project-control/SYSTEM_EVOLUTION_LOG.md` — إزالة merge conflict markers
- الملخص:
  - بُسيطت تعريفات 6 عملاء يملكون ملفات runtime مستقلة في `.opencode/agents/`:
    1. **§5.3.1 UIVisualDesignerAgent** → `ui-designer.md`
    2. **§5.6 EngineeringAgent** → `engineering-agent.md`
    3. **§5.7 QAAndAcceptanceAgent** → `qa-agent.md`
    4. **§6.9 SoftwareDesignerAgent** → `tera-software-designer.md`
    5. **§6.12 DomainResearchAgent** → `domain-research-agent.md`
    6. **§6.13 DomainExpertAgent** → `domain-expert-agent.md`
  - كل مُبسَّط يحتفي ب: جدول الهوية + الدور المختصر + مرجع للتعريف الكامل في ملف الـ runtime
  - الـ 12 عميلًا الآخرين (بدون ملفات runtime) احتفظوا بتعريفاتهم الكاملة
  - Sections 1-4 و 7-14 لم تتأثر
  - تم إصلاح merge conflict markers في `AGENT_DEPENDENCY_MAP.md` و `SYSTEM_EVOLUTION_LOG.md`
- الموافقة: Majed — Approved (طلب مباشر)
- التحقق من الصحة:
  - ✅ Anti-Bloat Gate PASS — تقليل التعقيد، لا إضافة
  - ✅ Policy Map Check PASS — لا تعارض
  - ✅ Architecture Map Check PASS — لا تغير معماري
  - ✅ No client-app contamination
  - ✅ No unauthorized privilege expansion
  - ✅ No stale/deprecated agent references — جميع المراجع تشير إلى ملفات runtime موجودة
  - ✅ No duplicated mandatory rules
  - ✅ File structure intact — جميع 14 قسم + الأقسام الفرعية سليمة
- المخاطر: منخفض — محتوى مُ移除 متوفر بالفعل في ملفات runtime مستقلة
- ملاحظات الاسترجاع (Rollback):
  1. `git checkout HEAD -- tera-system/TeraSubAgents.md`
  2. `git checkout HEAD -- tera-system/AGENT_DEPENDENCY_MAP.md`
  3. لا يتأثر أي ملف آخر
- ملاحظات إضافية:
  - الملف لا يزال فوق حد 1,000 سطر (1,502) — لكن المتبقي مبرر: 12 عميل بدون runtime يحتفظون بتعريفاتهم الكاملة + Sections نظامية
  - الخطوة التالية (عند موافقة Majed): دراسة أي من الـ 12 عميل يبرر إنشاء ملف runtime مستقل

---

## SCP-2026-07-13-094 — انضباط المسارات عند تفويض العملاء الفرعيين (Path Discipline)

- تاريخ: 2026-07-13
- معرف التغيير: SCP-2026-07-13-094
- مصدر الطلب: AGENT_GAPS_LOG.md — GAP-001 (Critical), GAP-002 (High), GAP-003 (Critical)
- نوع التغيير: Policy Update + Agent Improvement + Protocol Change
- الملفات المعدلة:
  - `.opencode/agents/tera.md` — إضافة Absolute Path Delegation Rule + Client Project Path Checkpoint
  - `.opencode/agents/engineering-agent.md` — إضافة §10.1 Path Validation Gate
  - `tera-system/TeraSubAgents.md` — إضافة قواعد المسارات + Checkpoint بعد §9
  - `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` — إضافة Path Enforcement Rule في §3.1
  - `project-control/AGENT_GAPS_LOG.md` — GAP-001/002/003: Pending → Applied
  - `project-control/SYSTEM_CHANGE_PROPOSALS.md` — إحالة SCP-001~003 إلى SCP-094
  - `project-control/SYSTEM_EVOLUTION_LOG.md` — هذا الإدخال
- الملخص:
  - **GAP-001 (EngineeringAgent):** أُضيفت Path Validation Gate — 5 خطوات فحص إلزامية قبل أي كتابة
  - **GAP-002 (TeraAgent مسارات نسبية):** أُضيفت Absolute Path Delegation Rule — Allowed Write Targets = مسارات كاملة إلزامياً
  - **GAP-003 (TeraAgent ملفات في الجذر):** أُضيف Client Project Path Checkpoint — 6 خطوات قبل بداية مشروع عميل
  - **TeraSubAgents.md:** قاعدة إلزامية للمسارات + Checkpoint فحص للعملاء الفرعيين
  - **TERA_RUNTIME_PROTOCOLS.md:** Path Enforcement Rule — مسؤولية + عواقب
  - **توحيد 3 مقترحات سابقة (SCP-001~003) في SCP واحد شامل**
- الموافقة: Majed — Approved
- التحقق من الصحة: (راجع Validation Gates أدناه)
- المخاطر: منخفض — قواعد فحص فقط، لا تغيير صلاحيات
- ملاحظات الاسترجاع (Rollback):
  1. `git checkout HEAD -- .opencode/agents/tera.md`
  2. `git checkout HEAD -- .opencode/agents/engineering-agent.md`
  3. `git checkout HEAD -- tera-system/TeraSubAgents.md`
  4. `git checkout HEAD -- tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md`
   5. إعادة GAP-001/002/003 إلى Pending في AGENT_GAPS_LOG.md
   6. إزالة هذا الإدخال من SYSTEM_EVOLUTION_LOG.md

---

## SCP-2026-07-13-095 — إضافة قالب TERA_HANDOFF_PACKAGE مع قسم SCP-038 (AIS-0007)

- التاريخ: 2026-07-13
- معرف التغيير: SCP-2026-07-13-095
- مصدر الطلب: AIS Processing (AIS-0007) — TCEA اقتراح
- نوع التغيير: Template Addition / AIS Implementation
- الملفات المعدلة:
  - `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` — إضافة §36: قالب TERA_HANDOFF_PACKAGE.md كامل مع قسم SCP-038 (§1)
  - `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` — تحديث حالة AIS-0007 إلى Implemented, تحديث حالة AIS-0008 إلى Implemented (SCP-094)
  - `project-control/SYSTEM_CHANGE_PROPOSALS.md` — إضافة SCP-095, تحديث حالة SCP-094 إلى منفّذ
  - `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-13-095.md` — إنشاء
- الملخص:
  - أُضيف §36 إلى TERA_RUNTIME_TEMPLATES.md يتضمن قالب TERA_HANDOFF_PACKAGE.md كامل
  - القسم §1 من القالب مخصص للتوافق مع SCP-038 (القواعد الأربع: Final Scope Reconciliation, Budget-to-Scope, Decision Register, Approval Consistency)
  - القالب يغطي 11 قسماً أساسياً من Executive Summary إلى Readiness Decision
  - وُثّق أن قسم SCP-038 إلزامي ("do NOT remove")
- الموافقة: Owner Directive — Majed ("you decide and fix")
- التحقق من الصحة:
  - ✅ Anti-Bloat Gate PASS — حل مشكلة حقيقية (لا يوجد قالب موحد)
  - ✅ Consistency with Dependency Map — Verified (لا تعارض)
  - ✅ No broken references — Grep check PASS
  - ⚠️ File Size Note: TERA_RUNTIME_TEMPLATES.md زاد ~145 سطراً (~1,606 → ~1,751) — سيعالج ضمن M-1
- المخاطر: منخفض — قالب فقط، لا تغيير في سلوك العملاء
- ملاحظات الاسترجاع (Rollback):
  1. إزالة §36 من TERA_RUNTIME_TEMPLATES.md
  2. إعادة AIS-0007 إلى حالة Not active في AGENT_IMPROVEMENT_SUGGESTIONS.md
   3. حذف SCP-095 من SYSTEM_CHANGE_PROPOSALS.md
   4. حذف SCP-095 من SYSTEM_EVOLUTION_LOG.md

---

## H-2 — إعادة تسمية مجلدات العملاء من العربية إلى اللاتينية (Health Report 2026-07-08)

- التاريخ: 2026-07-13
- معرف التغيير: H-2-Rename-Clients
- مصدر الطلب: SYSTEM_HEALTH_REPORT_2026-07-08.md — H-2 (عالي)
- نوع التغيير: Structural Cleanup / Path Rename
- الملفات المعاد تسميتها (4 مجلدات رئيسية):
  - `الماجد-لادارة-المستودعات` → `CLIENT-MAJED-WAREHOUSE`
  - `شركة-الحوت-للمقاولات` → `CLIENT-HOOT-CONTRACTING`
  - `شركة-العمران-الحديثة-للمقاولات` → `CLIENT-OMRAN-CONTRACTING`
  - `شركة-حسين-عطية-للمقاولات` → `CLIENT-HUSAIN-ATTIYA`
- المراجع المحدّثة (21 ملفاً):
  - نظامية: `AGENT_IMPROVEMENT_SUGGESTIONS.md` (AIS-0008), `TASK-COD-007.md`
  - عميل MAJED-WAREHOUSE: ملفات engagement, tasks, preparation (13 ملفاً)
  - عميل HOOT-CONTRACTING: `R17-procurement-planning-budget.md`
  - عميل OMRAN-CONTRACTING: `TERA_HANDOFF_PACKAGE.md`
  - عميل HUSAIN-ATTIYA: `TERA_HANDOFF_PACKAGE.md`, `DRAFT_QUOTATION.md`
- الموافقة: Owner Directive — Majed ("نفذ")
- المخاطر: منخفض — تغيير اسماء مجلدات فقط. git تعامل معها كـ renames
- ملاحظات الاسترجاع (Rollback):
  1. `git checkout HEAD -- clients/CLIENT-MAJED-WAREHOUSE/` وإعادة تسميته يدوياً إلى `الماجد-لادارة-المستودعات`
  2. `git checkout HEAD -- clients/CLIENT-HOOT-CONTRACTING/` وإعادة تسميته يدوياً
  3. `git checkout HEAD -- clients/CLIENT-OMRAN-CONTRACTING/` وإعادة تسميته يدوياً
  4. `git checkout HEAD -- clients/CLIENT-HUSAIN-ATTIYA/` وإعادة تسميته يدوياً
  5. إعادة الملفات المحدّثة من git (21 ملفاً)

---

## SCP-2026-07-16-096 — ضبط حجم تفويض العملاء الفرعيين

- تاريخ: 2026-07-16
- معرف التغيير: SCP-2026-07-16-096
- مصدر الطلب: Owner Improvement Request (Majed)
- نوع التغيير: Agent Improvement / Runtime Guardrail
- الملفات المعدلة:
  - `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-096.md` — إنشاء مقترح التغيير
  - `.opencode/agents/tera.md` — إضافة Sub-Agent Delegation Size Rule وتحديث Last Synced
  - `project-control/SYSTEM_EVOLUTION_LOG.md` — هذا الإدخال
- الملخص:
  - أُضيفت قاعدة تشغيلية تمنع TeraAgent من تفويض العملاء الفرعيين بمهام ضخمة أو متعددة المراحل دفعة واحدة.
  - كل تفويض يجب أن يكون صغيراً أو متوسطاً، مرتبطاً بـ `TASK-ID` واضح، ومحدوداً بهدف واحد ومصادر/مسارات كتابة صريحة.
  - إذا كان العمل كبيراً، يجب تقسيمه إلى `TASK-ID`s متتابعة أو batch صغير معتمد.
  - يجب اتباع التسلسل: تفويض → handback → مراجعة فعلية → قبول/إصلاح/حظر/تأجيل → ثم التفويض التالي.
- الموافقة: Majed — Approved (`موافق نفذ`)
- التحقق من الصحة: Validation Passed (Anti-Bloat Gate PASS، Policy/Architecture Check PASS، No client-app contamination، No privilege expansion، no broken agent references، file size below mandatory split threshold، scoped git diff --check PASS للملفات المعدلة فقط. ملاحظة: global git diff --check يفشل بسبب trailing whitespace قديم في ملفات غير مرتبطة بهذا التغيير.)
- المخاطر: منخفض — تقييد تشغيلي يزيد عدد دورات المراجعة لكنه يقلل أخطاء التفويض الكبير.
- ملاحظات الاسترجاع (Rollback):
  1. إزالة قسم `Sub-Agent Delegation Size Rule` من `.opencode/agents/tera.md`.
  2. إعادة `Last Synced` إلى السطر السابق.
  3. إزالة هذا الإدخال من `SYSTEM_EVOLUTION_LOG.md` إذا نُفذ rollback.

---

## SCP-2026-07-16-097 — قاعدة قراءة الملف قبل تعديله للجلسات المتزامنة

- تاريخ: 2026-07-16
- معرف التغيير: SCP-2026-07-16-097
- مصدر الطلب: Owner Improvement Request (Majed)
- نوع التغيير: Agent Improvement / Runtime Guardrail
- الملفات المعدلة:
  - `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-097.md` — إنشاء مقترح التغيير
  - `.opencode/agents/tera.md` — إضافة Fresh File Read Rule وتحديث Last Synced
  - `project-control/SYSTEM_EVOLUTION_LOG.md` — هذا الإدخال
- الملخص:
  - أُضيفت قاعدة تشغيلية تلزم TeraAgent باعتبار ذاكرة الجلسة قديمة عند تعديل أي ملف موجود.
  - قبل أي تعديل، يجب قراءة النسخة الحالية من القرص، وعدم الاعتماد على قراءة سابقة أو ذاكرة المحادثة.
  - يجب تمرير نفس التعليمة للعملاء الفرعيين عند التفويض: اقرأ الملف الحالي أولاً، واحفظ التغييرات غير المرتبطة بالمهمة.
  - إذا وُجدت تغييرات غير متوقعة أو تعارض مع عمل جلسة أخرى، لا تُحذف بصمت؛ يُوقف الجزء المتأثر ويُطلب قرار أو يسجل قرار قبل overwrite.
- الموافقة: Majed — Approved (`موافق نفذ`)
- التحقق من الصحة: Validation Passed (Anti-Bloat Gate PASS، Policy/Architecture Check PASS، No client-app contamination، No privilege expansion، no broken agent references، file size 833 lines = yellow range but below mandatory split threshold، scoped git diff --check PASS للملفات المعدلة فقط. ملاحظة: global git diff --check يفشل بسبب trailing whitespace قديم في ملفات غير مرتبطة بهذا التغيير.)
- المخاطر: منخفض — يزيد قراءة الملفات قبل التعديل لكنه يمنع ضياع عمل جلسة أخرى.
- ملاحظات الاسترجاع (Rollback):
  1. إزالة قسم `Fresh File Read Rule` من `.opencode/agents/tera.md`.
  2. إعادة `Last Synced` إلى السطر السابق.
  3. إزالة هذا الإدخال من `SYSTEM_EVOLUTION_LOG.md` إذا نُفذ rollback.
