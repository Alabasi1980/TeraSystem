# TERA_HANDOFF_PACKAGE.md — TeraWare

**العميل:** شركة حسين عطية للمقاولات
**التطبيق:** TeraWare — نظام إدارة المستودعات المساعد
**تاريخ الحزمة:** 2026-07-09
**الحالة:** ✅ معتمد — جاهز للتسليم إلى ApplicationBlueprintAgent (2026-07-09)

---

## 1. نظرة عامة

زبون من شركة مقاولات يملك نظام NatejSoft ERP (Oracle APEX) لكن موديول المستودعات لا يلبي احتياجاته.
المطلوب: **تطبيق مساعد (Sidecar)** من مكونين يسد الفجوة.

| المكون | الغرض |
|--------|-------|
| **TeraWare.Desktop** (WPF + WebView2) | عرض/تعديل المواد، استعلامات Oracle، متصفح APEX مع JS |
| **TeraWare.Web** (Razor Pages + IIS) | طلبات مواد (ويب + موبايل)، ربط مع Oracle، SQL Server |

---

## 2. قائمة الوثائق المُسلمة

| # | الملف | الحالة |
|:-:|------|:------:|
| 1 | CLIENT_PROFILE.md | ✅ |
| 2 | CONTACTS.md | ✅ (ماجد خير الدين) |
| 3 | CLIENT_INTAKE.md | ✅ |
| 4 | DISCOVERY_COVERAGE_SUMMARY.md | ✅ (معتمد) |
| 5 | FEATURE_LIST.md | ✅ (مُسوّم — 24 ميزة In Scope) |
| 6 | CLIENT_DECISION_LOG.md | ✅ (18 قراراً — صفر Pending) |
| 7 | DRAFT_QUOTATION.md | ✅ (Level 2 — ~930 JOD — معتمد) |
| 8 | DOMAIN_RESEARCH_REPORT_ORACLE_APEX.md | ✅ (بحث APEX + WebView2) |

---

## 3. ملخص النطاق — الميزات الرئيسية

### Phase 1: TeraWare.Desktop (P1: 7 ميزات, P2: 4, P3: 0)

| # | الميزة | الأولوية |
|:-:|--------|:--------:|
| 1 | طبقة الوصول إلى Oracle (Oracle.ManagedDataAccess) | P1 |
| 2 | واجهة عرض المواد مع صور | P1 |
| 3 | تعديل فردي على المواد (UPDATE) | P1 |
| 4 | تعديل جماعي على المواد | P1 |
| 5 | استعلام أرصدة المواد لكل مستودع + Excel | P1 |
| 6 | استعلام الحركات اليومية + Excel | P2 |
| 7 | استعلام بيانات المواد + Excel | P2 |
| 8 | تصدير Excel للجداول | P2 |
| 9 | متصفح WebView2 لـ Oracle APEX | P1 |
| 10 | حقن JavaScript شامل (Labels، أزرار، قراءة، حفظ) | P1 |
| 11 | حفظ بيانات DOM إلى SQL Server | P2 |
| 12 | إضافة أزرار وأحداث في صفحات APEX | P2 |
| 13 | إدارة جلسة APEX (Session) | P3 (Pending) |

### Phase 2: TeraWare.Web (P1: 7 ميزات, P2: 4, P3: 0)

| # | الميزة | الأولوية |
|:-:|--------|:--------:|
| 1 | هيكل SQL Server (MaterialRequests + BrowserSettings) | P1 |
| 2 | طبقة الوصول إلى SQL Server | P1 |
| 3 | طبقة الوصول إلى Oracle للويب | P1 |
| 4 | إنشاء طلب مواد (Master-Detail + صور) | P1 |
| 5 | تصفح/اختيار المواد مع صور من Oracle | P1 |
| 6 | قائمة طلبات المواد مع فلاتر | P1 |
| 7 | عرض تفاصيل طلب المواد | P1 |
| 8 | طباعة طلب المواد PDF | P2 |
| 9 | ربط طلب المواد مع Oracle (يدوي) | P2 |
| 10 | إلغاء/رفض طلب المواد | P2 |
| 11 | تكامل متقدم مع Oracle (مستقبلي) | P3 (Deferred) |
| 12 | تجربة موبايل Responsive | P1 |
| 13 | صلاحيات من Oracle | P2 |

### مؤجل
- إشعارات واتساب وبريد إلكتروني

---

## 4. ملخص البنية التحتية

| العنصر | القيمة |
|--------|--------|
| لغة التطوير | .NET C# |
| Desktop | WPF + WebView2 (Microsoft.Web.WebView2) |
| Web | ASP.NET Core Razor Pages + Bootstrap |
| Oracle | Oracle.ManagedDataAccess — اتصال مباشر |
| SQL Server | ADO.NET / Entity Framework Core |
| المتصفح المخصص | WebView2 مع JS injection + Two-way communication |
| النشر | IIS داخل موقع الشركة |
| ERP | Oracle APEX (erp.hae.com.jo:444/erp/f?p=101:...) |
| الوصول الخارجي | VPN / Reverse Proxy — جاهز مسبقاً |

---

## 5. قواعد الوصول إلى Oracle

| الجدول | نوع الوصول |
|--------|:----------:|
| المواد | ✅ قراءة + UPDATE (فردي وجماعي) |
| المقاسات | ✅ قراءة فقط |
| مجموعات المواد | ✅ قراءة فقط |
| حركات السندات (رئيسي + تفصيلي) | ✅ قراءة فقط |
| المستودعات | ✅ قراءة فقط |
| المشاريع | ✅ قراءة فقط |
| طلبات المواد | ✅ قراءة فقط (الربط يدوي) |
| المستخدمين والصلاحيات | ✅ قراءة فقط |
| المرفقات (صور المواد) | ✅ قراءة فقط |

---

## 6. ملخص التسعير (Level 2 Draft — معتمد)

| البند | القيمة |
|------|:------:|
| مؤشر التعقيد | 63.24% (كبير — يحتاج تحليل مدفوع) |
| السعر الأساسي التقديري | ~930 JOD |
| الإضافات | 0 |
| **المجموع** | **~930 JOD** |
| المهلة | ~شهر |
| الدعم بعد التسليم | مفتوح (إصلاح أخطاء + تطوير) |

---

## 7. المخاطر المتبقية

| الخطر | التأثير | الحالة |
|-------|---------|:------:|
| إصدار Oracle APEX غير معروف | بعض JS APIs قد تختلف | يحتاج اختبار |
| عدد المستخدمين غير محدد | يؤثر على الترخيص إن وجد | غير مانع |
| Same-Origin Policy في WebView2 | قد لا يحقن JS في Iframes Cross-Origin | يحتاج اختبار |
| Session Management | طريقة كشف Session Expiry غير مؤكدة | Pending Decision |

---

## 8. خطة العمل المقترحة

1. **ApplicationBlueprintAgent** ← يستلم الحزمة ← ينتج APPLICATION_BLUEPRINT.md
2. **BluePrint Confirmation Gate** ← مراجعة واعتماد من Majed
3. **TeraAgent** ← يستلم الـ Blueprint ← يبدأ من Phase 2 — Project Decision

---

## 9. هيكل مساحة العمل (معتمد)

`
clients/CLIENT-HUSAIN-ATTIYA/
├── CLIENT_PROFILE.md
├── CONTACTS.md
└── applications/
    └── APP-TeraWare/
        ├── client-engagement/   ← ملفات TCEA
        ├── client-documents/    ← وثائق العميل
        ├── client-approval/     ← موافقات العميل
        ├── client-assets/       ← أصول العميل
        ├── client-communications/
        └── delivery/            ← حزمة التسليم النهائية
`

---

> **ملاحظة:** جميع العناصر في هذه الحزمة تحمل وسم [Confirmed by Majed].
> الميزة F-1.13 (Session Management) بحالة Pending — تم إيضاحها.
> تم الإعداد بواسطة TCEA — 2026-07-09
