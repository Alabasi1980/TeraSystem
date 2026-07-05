---
document_type: discovery_coverage_summary
client_name: "مؤسسة الموثوق لصيانة الأجهزة"
application_name: "نظام إدارة طلبات الصيانة"
version: "1.0"
date: "2026-07-05"
language: "ar"
direction: "rtl"
status: "draft"
prepared_by: "TCEA (مُستشار)"
project_classification: "Medium"
understanding_confirmed_by_Majed: "Yes"
---

# DISCOVERY_COVERAGE_SUMMARY

## 1. Metadata

| Field | Value |
|-------|-------|
| Client | مؤسسة الموثوق لصيانة الأجهزة |
| Application | نظام إدارة طلبات الصيانة |
| Prepared by | TCEA (مُستشار) |
| Date | 2026-07-05 |
| Last Updated | 2026-07-05 |
| Project Classification | Medium |
| Understanding Confirmed by Majed | ✅ Yes |

## 2. Domain Coverage Matrix

| #   | Domain                                 | Status     | Self-Check Source | Self-Check Confirmed? | Self-Check Risk | Reason / Notes                                                                                                                                                                                            | Impact | Blocks Quotation? | Blocks Handoff? |
| --- | -------------------------------------- | ---------- | ----------------- | --------------------- | --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------ | ----------------- | --------------- |
| 1   | **Business Context & Value**           | ✅ Complete | Majed (صراحة)     | Yes                   | Low             | اسم الزبون، مجال العمل، المشكلة، الميزانية (800-1,500 JOD)، الجدول (4-6 أسابيع)، صاحب القرار هو صاحب المؤسسة                                                                                              | —      | No                | No              |
| 2   | **Integrations & APIs**                | ⚡ Partial  | Majed (صراحة)     | Yes                   | Low             | واتساب API: مؤجل للمرحلة اللاحقة. حالياً يكتفون بحفظ رقم واتساب العميل وإمكانية فتح المحادثة يدوياً. لا بوابات دفع إلكتروني أو تكاملات خارجية في MVP                                                      | Low    | No                | No              |
| 3   | **Users & Roles**                      | ✅ Complete | Majed (صراحة)     | Yes                   | Low             | 4 أدوار محددة بوضوح: مدير (صلاحية كاملة)، استقبال (عملاء + طلبات)، فني (طلباته فقط + الحالة الفنية)، محاسب (مدفوعات + تقارير مالية). ~7 مستخدمين                                                          | —      | No                | No              |
| 4   | **Workflow & Operations**              | ✅ Complete | Majed (صراحة)     | Yes                   | Low             | دورة كاملة: استقبال طلب ← فحص ← عرض تكلفة ← موافقة عميل ← صيانة ← تسليم ← دفع ← إغلاق. تفاصيل الموافقة موثقة                                                                                              | —      | No                | No              |
| 5   | **Scope & MVP**                        | ✅ Complete | Majed (صراحة)     | Yes                   | Low             | 3 مراحل متفق عليها: Phase 1 (Core: عملاء، طلبات، حالات، فنيين، دفع)، Phase 2 (قطع غيار)، Phase 3 (تقارير + تنبيهات). Out of scope موثق                                                                    | —      | No                | No              |
| 6   | **Data & Content**                     | ⚡ Partial  | Majed (صراحة)     | Partially             | Medium          | الكيانات الرئيسية معروفة (عميل، جهاز، طلب، قطعة غيار، دفعة). ترحيل بيانات: أسماء + هواتف + عناوين من Excel فقط. أنواع الأجهزة (6-8) وماركات (15-25). تفاصيل نموذج البيانات الدقيق يحتاج تأكيد أثناء Scope | Medium | No                | No              |
| 7   | **Notifications Engine**               | ⚡ Partial  | Majed (صراحة)     | Yes                   | Low             | إشعارات داخلية (In-app): تغيير حالة، إسناد لفني، طلبات متأخرة، جهاز جاهز. واتساب/SMS: مؤجل                                                                                                                | Low    | No                | No              |
| 8   | **Screens & UX**                       | ✅ Complete | Majed (صراحة)     | Yes                   | Low             | الشاشات محددة: Dashboard، عملاء، إنشاء طلب، قائمة طلبات، تفاصيل طلب، فنيين، تقارير، تتبع طلب، إعدادات. ويب متجاوب مع الجوال                                                                               | —      | No                | No              |
| 9   | **Design & Branding**                  | ⚡ Partial  | Majed (صراحة)     | Yes                   | Low             | يوجد شعار (بسيط). لا يوجد دليل هوية بصرية. Majed يثق بتوصية Tera في الألوان والتصميم. يحتاج تأكيد لاحق على التصميم المقترح                                                                                | Low    | No                | No              |
| 10  | **Reports & Dashboards**               | ✅ Complete | Majed (صراحة)     | Yes                   | Low             | التقارير محددة بالترتيب: (1) طلبات متأخرة (2) يومي بعدد الطلبات (3) شهري بالإيرادات (4) أداء فنيين (5) القطع الأكثر استخداماً. Dashboard ملخص                                                             | —      | No                | No              |
| 11  | **Technical, Hosting & Compliance**    | ⚡ Partial  | Majed (صراحة)     | Partially             | Medium          | Hosting: سحابي (يحدد لاحقاً)، يريد معرفة التكلفة الشهرية. لا متطلبات تقنية محددة. Tech stack مفتوح المصدر. لا متطلبات امتثال خاصة                                                                         | Medium | No                | No              |
| 12  | **Security & Audit**                   | ⚡ Partial  | Majed (صراحة)     | Yes                   | Low             | تسجيل دخول: اسم مستخدم/هاتف + كلمة مرور. المدير ينشئ الحسابات. Audit log أساسي للتغييرات المهمة. بيانات عادية — حماية قياسية كافية                                                                        | Low    | No                | No              |
| 13  | **Acceptance, Commercials & Warranty** | ✅ Complete | Majed (صراحة)     | Yes                   | Low             | (أ) معايير القبول: اختبار MVP قبل الدفعة الثانية. (ب) خطة الدفع: 30%-40%-30%. (ج) الضمان: 3 أشهر مجاناً. صيانة اختيارية بعدها                                                                             | —      | No                | No              |

> **ملاحظة المجال 13:** تمت تغطية الجوانب الثلاثة: (أ) معايير القبول والاختبارات, (ب) الميزانية وخطة الدفع, (ج) الضمان والصيانة.

## 3. Self-Check Protocol — تفاصيل المجالات غير المكتملة

| # | Domain | Source | Confirmed by Majed? | Risk if Wrong | Action Required |
|---|--------|--------|---------------------|---------------|-----------------|
| 2 | Integrations & APIs | Majed (صراحة) | Yes | Low | مؤجل — لا يؤثر على المضي قدماً |
| 6 | Data & Content | Majed (صراحة) | **Partially** | Medium | تفاصيل نموذج البيانات ستنضج أثناء Scope — يحتاج مراجعة لاحقة |
| 7 | Notifications | Majed (صراحة) | Yes | Low | يمكن تحديد التفاصيل أثناء التطوير |
| 9 | Design & Branding | Majed (صراحة) | Yes | Low | سيتم عرض مقترحات التصميم لاحقاً للموافقة |
| 11 | Technical & Hosting | Majed (صراحة) | **Partially** | Medium | قرار الاستضافة سيأخذ لاحقاً — سيتم عرض خيارات مع التكلفة الشهرية |
| 12 | Security & Audit | Majed (صراحة) | Yes | Low | التفاصيل التقنية ستحدد أثناء التنفيذ |

**لا توجد أي Domain تستوفي شرط "Inference/Unknown + High Risk"** ← لا حاجة لـ Uncertainty Notice. ✅

## 4. Coverage Decision

| Item | Value |
|------|-------|
| **Overall Status** | ✅ **Ready for Scope** |
| Missing Critical Domains | None |
| Quotation Blocker | لا توجد — جميع الـ Blockers هي `No` |
| Handoff Blocker | لا توجد — جميع الـ Blockers هي `No` |
| Next Action | إنتاج ملفات النطاق: CLIENT_BRIEF.md, SCOPE_SUMMARY.md, FEATURE_LIST.md |
| **Approved by Majed** | **✅ Yes (2026-07-05)** |

## 5. Open Questions Summary

| السؤال | التصنيف | المجال |
|--------|---------|--------|
| ما هو خيار الاستضافة المناسب (مع التكلفة الشهرية)؟ | Non-blocking | Technical |
| هل ترغب في مراجعة نموذج البيانات المقترح (الكيانات والعلاقات)؟ | Non-blocking | Data |
| هل تصميم الألوان المقترح من Tera يناسبك؟ | Non-blocking | Design |
| تأكيد خطة الدفع المقترحة (30%-40%-30%) | Non-blocking | Commercial |
