---
document_type: discovery_coverage_summary
client_name: "مؤسسة الفارس للصيانة"
application_name: "نظام إدارة طلبات الصيانة الداخلي"
version: "1.0"
date: "2026-07-04"
language: "ar"
direction: "rtl"
status: "draft"
prepared_by: "TeraClientEngagementAgent"
approved_by: ""
gate_decision: "Needs More Discovery"
---

# DISCOVERY_COVERAGE_SUMMARY — نظام إدارة طلبات الصيانة الداخلي

> مصفوفة تغطية الاكتشاف الإلزامية وفق إطار TCEA Mandatory 13-Domain Client Discovery Framework.

---

## 1. ملخص سريع

| الحقل | القيمة |
|-------|--------|
| تصنيف المشروع | Small-Medium |
| Understanding Confirmation | **Confirmed by Majed** |
| Discovery Coverage Decision | **Needs More Discovery** |
| هل يمكن اعتماد ملفات Scope؟ | **No — ليس بعد** |
| هل يمكن اعتماد Draft Quotation كـ baseline؟ | **No** |
| هل يمكن اعتماد TERA_HANDOFF_PACKAGE كـ baseline؟ | **No** |

---

## 2. Discovery Completeness Matrix

| # | المجال | الحالة | السبب / الملاحظات | يمنع التسعير؟ | يمنع Handoff؟ | السؤال التالي المطلوب | الافتراض المؤقت | الخطر |
|---|--------|--------|-------------------|:-------------:|:-------------:|------------------------|-----------------|:------:|
| 1 | Business Context & Value | Complete | المشكلة، الهدف، القيمة التشغيلية، الجهة، وصاحب القرار موثقة جيداً | No | No | — | — | Low |
| 2 | Integrations & APIs | Complete | تم التحقق أن MVP الحالي بدون تكاملات خارجية | No | No | هل أي تكامل مطلوب لاحقاً في Phase 2؟ | لا تكاملات في MVP | Low |
| 3 | Users & Roles | Complete | الأدوار الثلاثة والصلاحيات الرئيسية موثقة | No | No | هل نحتاج Dynamic Roles لاحقاً؟ | الصلاحيات ثابتة في MVP | Low |
| 4 | Workflow & Operations | Partial | Happy Path واضح، لكن الاستثناءات وحدود الإلغاء/الرفض والتأخير غير مفصلة | No | Yes | ما الذي يحدث عند تجاهل الطلب أو التأخير الطويل؟ | Workflow MVP بسيط بدون approvals | Medium |
| 5 | Scope & MVP | Complete | داخل/خارج النطاق وMVP موثقة بوضوح مقبول | No | No | — | — | Low |
| 6 | Data & Content | Partial | الكيانات والحقول الأساسية معروفة، لكن الحقول الإلزامية/الاختيارية والقوائم وexport غير مكتملة بالكامل | No | Yes | ما الحقول الإلزامية الدقيقة؟ وهل نحتاج Excel/PDF لاحقاً؟ | لا migration ولا import/export في MVP | Medium |
| 7 | Notifications Engine | Deferred | تم توثيق أن الإشعارات مؤجلة خارج MVP | No | No | هل Phase 2 تحتاج Email أم WhatsApp؟ | لا إشعارات في MVP | Low |
| 8 | Screens & UX | Partial | الشاشات الرئيسية واضحة، لكن تجربة كل دور وأولوية Desktop/Mobile ليست موثقة بعمق | No | No | ما أول شاشة لكل دور؟ وهل mobile responsive وحده كافٍ؟ | Web Responsive داخلي | Low |
| 9 | Design & Branding | Partial | يوجد اتجاه عام: عربي RTL + واجهة بسيطة، لكن لا يوجد توثيق كافٍ للشعار/الألوان/المراجع/ما لا يعجب العميل | Yes | Yes | هل توجد هوية بصرية محددة أو أمثلة واجهات مفضلة/مرفوضة؟ | استخدام اتجاه بصري بسيط ورسمي | Medium |
| 10 | Reports & Dashboards | Partial | Dashboard الأساسي معروف، لكن تعريفات المؤشرات والمعادلات غير مكتملة | No | Yes | ما تعريف الطلب المتأخر؟ وهل توجد تقارير Excel/PDF لاحقاً؟ | لا مؤشرات معقدة في MVP | Medium |
| 11 | Technical, Hosting & Compliance | Partial | نوع التطبيق معروف، لكن الاستضافة النهائية والبيئة والامتثال والنسخ الاحتياطي غير مكتملة | Yes | Yes | أين سيُستضاف النظام؟ ومن يدير البيئة والنسخ الاحتياطي؟ | استضافة داخلية/محلية مبدئياً | High |
| 12 | Security & Audit | Partial | login + roles معروفة، لكن سياسة كلمات المرور، audit log، والعمليات الحساسة غير محددة بما يكفي | Yes | Yes | هل نحتاج Audit Log؟ وما سياسة إدارة المستخدمين وكلمات المرور؟ | لا audit log تفصيلي في MVP ما لم يطلب العميل | High |
| 13 | Acceptance, Commercials & Warranty | Partial | السعر، خطة الدفع، والضمان موثقة، لكن معايير القبول واختبار الـ MVP غير مكتملة | No | Yes | من يعتمد MVP عملياً؟ وما سيناريو القبول التشغيلي؟ | القبول سيكون عبر Majed/صاحب القرار بعد عرض النسخة الأولى | Medium |

---

## 3. Quotation Blockers

الحالة الحالية: **يوجد مانعات تجعل Level 2 Draft Quotation غير صالح كـ baseline معتمد بعد**.

### المانعات الرئيسية
1. Design & Branding غير مكتمل بما يكفي
2. Technical / Hosting assumptions تحتاج توثيق أوضح
3. Security assumptions غير موثقة كفاية
4. Delivery assumptions تحتاج صياغة أوضح

---

## 4. Handoff Blockers

الحالة الحالية: **يوجد مانعات تجعل TERA_HANDOFF_PACKAGE غير صالح كـ baseline معتمد بعد**.

### المانعات الرئيسية
1. Workflow exceptions غير مكتملة
2. Data detail غير مكتمل بالكامل
3. Design direction غير مكتمل كفاية
4. Reports definitions غير مكتملة
5. Technical / Hosting / Backup context غير مكتمل
6. Security / Audit notes غير مكتملة
7. Acceptance criteria غير مكتملة
8. Open questions غير مصنفة رسمياً بعد

---

## 5. Open Questions Classification

| السؤال | التصنيف | السبب |
|--------|---------|-------|
| أين ستتم استضافة النظام فعلياً؟ | Blocking | يؤثر على handoff وtechnical assumptions |
| هل نحتاج Audit Log للعمليات الحساسة؟ | Blocking | يؤثر على scope/security/handoff |
| من الشخص المخول باعتماد النسخة الأولى عملياً؟ | Blocking | يؤثر على acceptance criteria |
| هل توجد ألوان/هوية/مرجع واجهات معتمد؟ | Non-blocking | يمكن بدء التحضير، لكنه يؤثر على جودة التصميم | 
| هل نحتاج Excel/PDF في Phase 2؟ | Deferred | ليس من MVP الحالي |
| هل نحتاج Email notifications لاحقاً؟ | Deferred | مؤجل خارج MVP |
| هل الاستضافة الداخلية مناسبة كبداية؟ | Assumption | افتراض مؤقت حتى قرار العميل |

---

## 6. Gate Decision

**القرار الحالي:** `Needs More Discovery`

### السبب
الفهم الأساسي مؤكد، لكن تغطية مجالات التصميم، التقنية/الاستضافة، الأمن/التدقيق، ومعايير القبول لا تزال غير كافية لاعتماد ملفات Scope / Quotation / Handoff كخط أساس نهائي.

### المطلوب قبل الإغلاق
1. جولة أسئلة قصيرة مركزة لإغلاق المانعات
2. تحديث هذه المصفوفة
3. إعادة عرض النتيجة على Majed

---

## 7. التنبيه الحوكمي الحالي

```text
Current downstream files may remain as working drafts,
but they are not approved baseline artifacts under the new framework
until this Discovery Coverage Summary is approved.
```

---

*هذا المستند مسودة تشغيلية داخلية بانتظار مراجعة واعتماد Majed.*
