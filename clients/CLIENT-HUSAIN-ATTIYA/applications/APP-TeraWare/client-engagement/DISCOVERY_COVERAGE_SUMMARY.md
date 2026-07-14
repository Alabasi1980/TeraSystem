# DISCOVERY_COVERAGE_SUMMARY.md — TeraWare

## 1. Metadata

| Field | Value |
|---|---|
| Client | شركة حسين عطية للمقاولات |
| Application | TeraWare |
| Prepared by | TCEA |
| Date | 2026-07-08 |
| Last Updated | 2026-07-08 |

---

## 2. Domain Coverage Matrix

> الترقيم والتسمية حسب المصدر الرسمي: `tera-system/client-helpers/tera-client-engagement-discovery-domains.md`

| # | Domain | Status | Reason if not Complete | Impact | Risk | Blocks Pricing? | Blocks Handoff? | **Source of Info** | **Confirmed by Majed?** | **Risk if Wrong** |
|:-:|--------|:------:|------------------------|--------|:----:|:---------------:|:---------------:|:-------------------:|:----------------------:|:-----------------:|
| 1 | Business & Goals | ✅ Complete | — | مشروع مساعد لموديول المستودعات في ERP NatejSoft | L | لا | نعم | Majed | Yes | L |
| 2 | Users, Roles & Access | ⚠️ Partial | عدد المستخدمين غير محدد بعد — الصلاحيات تُقرأ من Oracle | يحدد عدد التراخيص إذا وجد | M | لا | نعم | Majed | Partially | M |
| 3 | Process & Workflow | ✅ Complete | — | عرض/تعديل مواد + استعلامات + متصفح + طلبات مواد | L | لا | لا | Majed | Yes | L |
| 4 | Data & Content | ✅ Complete | — | Oracle: 8 جداول للقراءة + 1 للتعديل (المواد). SQL Server: طلبات مواد + إعدادات | L | لا | لا | Majed | Yes | L |
| 5 | Scope & MVP | ✅ Complete | — | Phase 1: Desktop + Phase 2: Web. مؤجل: إشعارات | L | لا | لا | Majed | Yes | L |
| 6 | Screens & UX | ⚠️ Partial | لا توجد Wireframes — التصميم من الصفر بناءً على المرجع (NatejSoft) | يحدد عدد الشاشات | M | لا | نعم | Majed + Screenshot | Partially | M |
| 7 | Notifications Engine | ⏳ Deferred | مؤجل لمرحلة لاحقة (واتساب + بريد) | لا يؤثر على MVP | L | لا | لا | Majed | Yes | L |
| 8 | Reports & Dashboards | ⚠️ Partial | جداول ديناميكية + تصدير Excel + فلاتر — التقارير الدقيقة لم تُحدد | لا يؤثر على MVP حالياً | M | لا | لا | Majed | Partially | M |
| 9 | Design & Branding | ❌ Missing | لم يُناقش — لا ألوان، لا شعار، لا توجيه بصري | لا يؤثر على Phase 1 | L | لا | لا | — | No | L |
| 10 | Technical, Hosting & Compliance | ✅ Complete | — | .NET C#, WPF, WebView2, ASP.NET Core, IIS, Oracle.ManagedDataAccess, SQL Server | L | لا | لا | Majed | Yes | L |
| 11 | Security & Audit | ⚠️ Partial | المستخدمون والصلاحيات يُقرأون من Oracle — لا تفاصيل أكثر بعد | لا يؤثر على Phase 1 | M | لا | لا | Majed | Partially | M |
| 12 | Integrations & APIs | ✅ Complete | — | اتصال مباشر Oracle (ODP.NET) + SQL Server + WebView2 JS injection لـ APEX | L | لا | لا | Majed | Yes | L |
| 13 | Acceptance, Commercials & Warranty | ⚠️ Partial | الميزانية مفتوحة — معايير القبول والضمان لم تُحدد | لا يؤثر على بدء العمل | L | لا | نعم | Majed | Partially | M |

> **ملاحظة المجال 13:** يحتاج تغطية 3 جوانب على الأقل: (أ) معايير القبول والاختبارات, (ب) الميزانية وخطة الدفع, (ج) الضمان والصيانة.

---

## 3. Coverage Decision

| Item | Value |
|---|---|
| **Overall Status** | Ready for Scope |
| **Missing Critical Domains** | لا يوجد مجال حرج مفقود — جميع المجالات المفقودة أو الجزئية غير مانعة |
| **Uncertainty Notices** | 1 — صور المواد: من جدول المرفقات في Oracle (يحتاج تأكيد هيكل الجدول) |
| **Next Action** | إعداد FEATURE_LIST.md أو SCOPE_SUMMARY.md بناءً على المرحلة الأولى |
| **Approved by Majed** | ✅ Yes (2026-07-09) |

---

## 4. Notes

### قرارات مسجلة (مرجع: CLIENT_DECISION_LOG.md)
1. تقسيم المشروع إلى مرحلتين (Desktop + Web)
2. Oracle.ManagedDataAccess — اتصال مباشر بدون API Layer
3. التعديل على Oracle محدود بـ UPDATE على جدول المواد فقط
4. متابعة طلب المواد من NatejSoft ERP الأصلي
5. الإشعارات مؤجلة
6. المتصفح المخصص: ملف JavaScript شامل يُحقن عند التشغيل + تصفح صفحات APEX عبر URL parameters
7. صور المواد تُقرأ من جدول المرفقات في Oracle

### أرقام مهمة (للمتابعة)
- **ERP URL:** `https://erp.hae.com.jo:444/erp/f?p=101:PAGE_ID:SESSION_ID:APP_ID::APP_ID:PARAMS`
- **APEX App ID:** `101`
- **URL Pattern:** `f?p=101:PAGE:SESSION:APP::APP:PARAMS`

### غير محدد بعد (يُ讨论 أثناء التطوير)
- عدد المستخدمين الفعلي
- التفاصيل الدقيقة لتصميم الشاشات (Wireframes)
- التقارير التفصيلية والفلاتر
- Branding والهوية البصرية
- معايير القبول النهائية والضمان
