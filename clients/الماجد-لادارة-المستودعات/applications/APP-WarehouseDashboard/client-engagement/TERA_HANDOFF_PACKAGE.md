# TERA_HANDOFF_PACKAGE.md — WarehouseDashboard

**العميل:** الماجد لادارة المستودعات
**التطبيق:** WarehouseDashboard — نظام مزامنة البيانات والداشبورد
**تاريخ الحزمة:** 2026-07-12
**الحالة:** ⏳ بانتظار اعتماد Majed — جاهز للتسليم إلى ApplicationBlueprintAgent

---

## 1. نظرة عامة

عميل في مجال إدارة المستودعات يريد نظاماً محلياً مؤلفاً من تطبيقين:
1. **API** — يستخرج جداول من Oracle ويحوّلها إلى SQL Server (Full Refresh)
2. **Razor Dashboard** — يعرض البيانات في Dashboard احترافي مع ~20 بطاقة ديناميكية + Drill Down + Admin Panel

| المكون | الغرض |
|--------|-------|
| **WarehouseDashboard.API** | Oracle→SQL Server Sync + Synchronization Logs + Auto/Manual Sync |
| **WarehouseDashboard.Web** | Dashboard ديناميكي (~20 بطاقة) + Drill Down + Admin Panel |

---

## 2. قائمة الوثائق المُسلمة

| # | الملف | الحالة |
|:-:|------|:------:|
| 1 | CLIENT_INTAKE.md | ✅ |
| 2 | FEATURE_LIST.md | ✅ (33 مكوناً فرعياً — 9 ميزات رئيسية) |
| 3 | DISCOVERY_COVERAGE_SUMMARY.md | ✅ (B.1 = PASS) |
| 4 | CLIENT_DECISION_LOG.md | ✅ (19 قراراً — صفر Pending) |
| 5 | DRAFT_QUOTATION.md | ✅ (Level 2 — Time & Material — معتمد) |

---

## 3. ملخص النطاق — الميزات الرئيسية

### Phase 1: MVP (33 مكوناً فرعياً)

#### API Application

| # | الميزة | الأولوية | المكونات |
|:-:|--------|:--------:|:--------:|
| API-01 | Oracle Connection & Data Extraction | P1 | 4 |
| API-02 | SQL Server Data Loading (Full Refresh) | P1 | 4 |
| API-03 | Synchronization Engine (Auto + Manual) | P1 | 4 |
| API-04 | Synchronization Logs | P1 | 3 |

#### Razor Dashboard

| # | الميزة | الأولوية | المكونات |
|:-:|--------|:--------:|:--------:|
| RZR-01 | Dashboard Layout & Rendering | P1 | 4 |
| RZR-02 | Drill Down Functionality | P1 | 3 |
| RZR-03 | Data Binding & SQL Execution | P1 | 3 |
| RZR-04 | Sync Status & Controls | P1 | 4 |
| RZR-05 | Admin Panel (CRUD + Config) | P1 | 9 |

#### Infrastructure

| # | الميزة | الأولوية | المكونات |
|:-:|--------|:--------:|:--------:|
| INF-01 | Project Structure (.NET 8) | P1 | 4 |
| INF-02 | Database Setup (Config + Logs) | P1 | 3 |
| INF-03 | IIS Hosting | P1 | 3 |

### Phase 2 (مؤجل)
- شاشات تعديل البيانات
- صلاحيات المستخدمين
- Export to Excel/PDF
- Advanced Analytics

### خارج النطاق
- Cloud Hosting, Mobile App, Microservices, CI/CD, Automated Testing

---

## 4. ملخص البنية التحتية

| العنصر | القيمة |
|--------|--------|
| لغة التطوير | **C# على .NET 8 (LTS)** |
| نمط الواجهة | **ASP.NET Core Razor Pages** |
| مكتبة الداشبورد | **Syncfusion** (Community License — متوفرة) |
| قاعدة بيانات المصدر | **Oracle** (ODP.NET / Oracle.ManagedDataAccess) |
| قاعدة بيانات الهدف | **SQL Server** (Entity Framework Core) |
| الخادم | **نفس السيرفر** (Oracle + SQL Server محلياً) |
| النشر | **IIS داخلي** على سيرفر الزبون |
| الهوية البصرية | **Blue Theme** (11 لوناً محدداً) |

---

## 5. قواعد الاتصال بالبيانات

| العنصر | التفاصيل |
|--------|----------|
| **Oracle** | قراءة فقط — جداول TBD أثناء التنفيذ |
| **SQL Server — بيانات** | Full Refresh (حذف + إعادة إدخال) |
| **SQL Server — تكوين** | حفظ تكوين البطاقات (Admin Panel) |
| **SQL Server — سجلات** | تسجيل عمليات المزامنة |

---

## 6. ملخص التسعير (Level 2 — معتمد)

| البند | القيمة |
|------|:------:|
| مؤشر التعقيد | 45.95% (متوسط) |
| طريقة التسعير | **Time & Material** |
| السعر | **4 JOD / ساعة** |
| النطاق المقدر | 430 — 625 ساعة |
| التكلفة التقديرية | **1,720 — 2,500 JOD** |
| المهلة | بدون deadline — السرعة الطبيعية |
| الدعم بعد التسليم | صيانة + إصلاح أخطاء + تطوير |

---

## 7. المخاطر المتبقية

| الخطر | التأثير | الحالة |
|-------|---------|:------:|
| تفاصيل الجداول مؤجلة | يحدد تعقيد Sync Engine | يُحدد أثناء التنفيذ |
| عدد المستخدمين غير محدد | لا يؤثر على Phase 1 | يُحدد أثناء التطوير |
| Syncfusion Community License | يجب التحقق من الأهلية | مفتاح UnlockKey متوفر |
| Phase 2 غير محددة | لا يؤثر على MVP | مؤجل مبكراً |

---

## 8. خطة العمل المقترحة

1. **ApplicationBlueprintAgent** ← يستلم الحزمة ← ينتج APPLICATION_BLUEPRINT.md
2. **BluePrint Confirmation Gate** ← مراجعة واعتماد من Majed
3. **TeraAgent** ← يستلم الـ Blueprint ← يبدأ التنفيذ
4. **الأولوية:** API أولاً ← ثم Razor Dashboard

---

## 9. هيكل مساحة العمل (معتمد)

```
clients/الماجد-لادارة-المستودعات/
└── applications/
    └── APP-WarehouseDashboard/
        ├── client-engagement/   ← ملفات TCEA
        │   ├── CLIENT_INTAKE.md
        │   ├── FEATURE_LIST.md
        │   ├── DISCOVERY_COVERAGE_SUMMARY.md
        │   ├── CLIENT_DECISION_LOG.md
        │   ├── DRAFT_QUOTATION.md
        │   └── TERA_HANDOFF_PACKAGE.md
        ├── client-documents/    ← وثائق العميل (قادم)
        ├── client-approval/     ← موافقات العميل (قادم)
        ├── client-assets/       ← أصول العميل (قادم)
        └── delivery/            ← حزمة التسليم النهائية (قادم)
```

---

## 10. القرارات المسجلة (19 قراراً)

> تفصيل كامل في `CLIENT_DECISION_LOG.md`

| # | القرار | الحالة |
|:-:|--------|:------:|
| 1 | التقنية: .NET 8 (LTS) | ✅ Approved |
| 2 | نمط الواجهة: Razor Pages | ✅ Approved |
| 3 | الداشبورد: Syncfusion + Drill Down + ~20 بطاقة | ✅ Approved |
| 4 | Admin Panel: صفحة منفصلة محمية بكلمة مرور | ✅ Approved |
| 5 | المزامنة: Full Refresh (حذف + إعادة إدخال) | ✅ Approved |
| 6 | الأولوية: API أولاً، ثم Razor Dashboard | ✅ Approved |
| 7 | الخادم: Oracle + SQL Server على نفس السيرفر | ✅ Approved |
| 8 | المجلد: clients/الماجد-لادارة-المستودعات/ | ✅ Approved |
| 9 | الهوية البصرية: Blue Theme (11 لوناً) | ✅ Approved |
| 10 | تفاصيل الجداول: مؤجلة لوقت التنفيذ | ✅ Approved |
| 11 | اسم التطبيق: WarehouseDashboard | ✅ Approved |
| 12 | Discovery Coverage Gate: PASS مبدئي | ✅ Approved |
| 13 | Syncfusion: مفتاح UnlockKey متوفر | ✅ Approved |
| 14 | Level 1 Estimate: 1,200-2,000 JOD | ✅ Approved |
| 15 | الميزانية: مفتوحة | ✅ Approved |
| 16 | المطلوب بعد التسليم: صيانة فقط | ✅ Approved |
| 17 | المدة الزمنية: بدون deadline | ✅ Approved |
| 18 | حدود الدعم: إصلاح أخطاء + تطوير | ✅ Approved |
| 19 | طريقة التسعير: Time & Material @ 4 JOD/hr | ✅ Approved |

---

## 11. غير محدد بعد (يُحدد أثناء التنفيذ)

- تفاصيل الجداول وهياكلها في Oracle
- آليات السحب التفصيلية
- عدد المستخدمين الفعلي
- شاشات Phase 2 (التعديل + الصلاحيات)

---

> **ملاحظة:** جميع العناصر في هذه الحزمة تحمل وسم `[Confirmed by Majed]`.
> تم الإعداد بواسطة TCEA — 2026-07-12
