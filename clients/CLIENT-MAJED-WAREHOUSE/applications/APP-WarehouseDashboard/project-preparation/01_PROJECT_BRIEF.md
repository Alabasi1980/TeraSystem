# 01_PROJECT_BRIEF.md — WarehouseDashboard

**Status:** `preparation`
**Client:** الماجد لادارة المستودعات
**Application:** WarehouseDashboard
**Prepared by:** TeraAgent — Task TASK-PREP-001
**Date:** 2026-07-12
**Source Documents:** 00_PROJECT_INPUTS.md, APPLICATION_BLUEPRINT.md (approved_for_preparation), FEATURE_LIST.md, CLIENT_DECISION_LOG.md

---

## 1. Application Overview

**WarehouseDashboard** هو نظام محلي لإدارة وعرض بيانات المستودعات، يتكون من مكونين رئيسيين:

1. **Sync Engine (API)** — يستخرج البيانات من Oracle (source, read-only) ويحوّلها إلى SQL Server (destination, write + config + logs) باستخدام أسلوب Full Refresh (حذف جميع الصفوف + إدخال مجمّع). يدعم المزامنة التلقائية عبر Background Service واليدوية عبر API endpoint، مع تسجيل كامل لجميع العمليات.

2. **Dashboard (Razor Pages)** — يعرض البيانات في Dashboard احترافي ديناميكي يحتوي على ~20 بطاقة قابلة للتكوين عبر Admin Panel منفصل. يدعم Drill Down عبر مستويات متعددة مع Breadcrumb Navigation، و Syncfusion Charts، ولوحة ألوان زرقاء محددة (11 لوناً).

**الهدف:** توفير رؤية واضحة وتفاعلية للبيانات المخزنة في المستودعات عبر Dashboard ديناميكي يُبنى تلقائياً من تكوين مخزن في SQL Server.

---

## 2. Problem Statement

العميل يحتاج نظاماً محلياً لاستخراج البيانات من قاعدة Oracle وتحويلها إلى SQL Server، ثم عرضها في Dashboard ديناميكي مع بطاقات قابلة للتكوين عبر Admin Panel. لا يوجد حالياً حل متكامل يربط بين Oracle و SQL Server مع واجهة عرض تفاعلية تدعم Drill Down وتخصيص البطاقات ديناميكياً دون تدخل برمجي.

المشاكل الحالية:
- **غياب التكامل:** لا توجد آلية منتظمة لنقل البيانات من Oracle إلى SQL Server.
- **غياب الرؤية:** لا توجد واجهة عرض موحّدة للاطلاع على بيانات المستودعات.
- **صعوبة التحليل:** لا يمكن التعمق في البيانات (Drill Down) لتحليل التفاصيل.
- **جمود العرض:** أي تغيير في طريقة عرض البيانات يتطلب تعديل برمجي.
- **غياب التتبع:** لا توجد سجلات لعمليات نقل البيانات لضمان الشفافية.

---

## 3. Target Users

| الدور | الوصف | الصلاحيات | المرحلة |
|:-----:|-------|:----------:|:-------:|
| **Admin** | مسؤول النظام الرئيسي — يدير التطبيق بالكامل | Full access: Admin Panel + Card CRUD + Sync trigger + Sync Logs | Phase 1 |
| **Viewer** (مؤجل) | مستخدم عادي لعرض Dashboard فقط | Read-only Dashboard — لا تعديل أو وصول للإدارة | Phase 2 (deferred) |

> **ملاحظة:** عدد المستخدمين الفعلي غير محدد بعد — يُحدد أثناء التطوير. العميل متاح للإجابة.

---

## 4. Core Capabilities

### 4.1 Sync Engine (API) — استخراج البيانات ومزامنتها

| # | القدرة | الوصف |
|:-:|--------|-------|
| M1.1 | Oracle Data Extraction | قراءة جداول Oracle عبر ODP.NET مع Data Type Mapping |
| M1.2 | SQL Server Full Refresh | حذف جميع الصفوف + إدخال مجمّع في معاملة واحدة (ADO.NET SqlBulkCopy) |
| M1.3 | Incremental Sync Ready | المحرك مصمم لدعم Incremental من البداية (غير مفعّل في Phase 1) |
| M1.4 | Auto-Sync Scheduler | Background Service مع فترة قابلة للتكوين (افتراضياً 30 دقيقة) |
| M1.5 | Manual Sync API | POST /api/sync/trigger — تفعيل المزامنة يدوياً |
| M1.6 | Sync Status API | GET /api/sync/status — عرض حالة المزامنة الحالية |
| M1.7 | Sync Logs | تسجيل كل عملية (وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة) |

### 4.2 Dashboard (Razor) — عرض البيانات

| # | القدرة | الوصف |
|:-:|--------|-------|
| M2.1 | Dynamic Card Rendering | قراءة التكوين من DB + بناء البطاقات ديناميكياً |
| M2.2 | Drill Down | التعمق في البيانات عبر مستويات قابلة للتكوين لكل بطاقة |
| M2.3 | Breadcrumb Navigation | شريط تنقل يعرض المستوى الحالي مع إمكانية العودة |
| M2.4 | Sync Status Display | عرض آخر وقت مزامنة + مؤشر حالة + زر مزامنة يدوية |
| M2.5 | Data Binding | تنفيذ SQL Queries/Views من التكوين وربط النتائج بالبطاقات |
| M2.6 | Blue Theme | تطبيق الهوية البصرية المحددة (11 لوناً أزرق) |
| M2.7 | Responsive Layout | تخطيط شبكي متجاوب لجميع أحجام الشاشات |

### 4.3 Admin Panel (Razor) — إدارة التكوين

| # | القدرة | الوصف |
|:-:|--------|-------|
| M3.1 | Card CRUD | إنشاء / تعديل / حذف بطاقات |
| M3.2 | Data Source Config | تحديد مصدر البيانات (SQL Query / View) |
| M3.3 | Chart Type Selection | اختيار نوع العرض (Bar, Line, Pie, KPI, Table, Gauge) |
| M3.4 | Drill Down Config | تحديد مستويات الـ Drill Down والربط بين البطاقات |
| M3.5 | Card Layout Config | تحديد حجم وترتيب البطاقات في الشبكة |
| M3.6 | Config Save/Load | حفظ التكوين في SQL Server وتحميله |
| M3.7 | Query Tester | اختبار الاستعلام مع عرض النتائج قبل الحفظ |

### 4.4 Infrastructure — البنية التحتية

| # | القدرة | الوصف |
|:-:|--------|-------|
| M4.1 | .NET 8 Solution | حل يحتوي على مشروعين (API + Razor) |
| M4.2 | Config DB Schema | جداول تخزين تكوين البطاقات (DashboardCards, CardDrillDownLevels, SyncSettings) |
| M4.3 | Sync Logs Schema | جداول تخزين سجلات المزامنة (SyncLogs) |
| M4.4 | Data Tables Schema | جداول تخزين البيانات المنقولة من Oracle (تُحدد أثناء التنفيذ) |
| M4.5 | IIS Hosting | نشر محلي على IIS |

---

## 5. MVP Scope — Phase 1 (Core MVP)

Phase 1 يغطي 12 ميزة أساسية مصنفة كـ **Core MVP** وفقاً لبروتوكول MVP_DEFINITION_PROTOCOL. هذه الميزات تمثل أصغر نسخة قابلة للاستخدام ويجب تسليمها كاملة قبل أي توسع.

| # | الميزة | الكود | الفئة | الأولوية |
|:-:|--------|:-----:|:-----:|:--------:|
| 1 | Oracle Connection & Data Extraction | API-01 | Sync Engine | P1 — Must-have |
| 2 | SQL Server Data Loading / Full Refresh | API-02 | Sync Engine | P1 — Must-have |
| 3 | Synchronization Engine — Auto + Manual | API-03 | Sync Engine | P1 — Must-have |
| 4 | Synchronization Logs | API-04 | Sync Engine | P1 — Must-have |
| 5 | Dashboard Layout & Rendering | RZR-01 | Dashboard | P1 — Must-have |
| 6 | Drill Down Functionality | RZR-02 | Dashboard | P1 — Must-have |
| 7 | Data Binding & SQL Execution | RZR-03 | Dashboard | P1 — Must-have |
| 8 | Sync Status & Controls | RZR-04 | Dashboard | P1 — Must-have |
| 9 | Admin Panel — CRUD + Config | RZR-05 | Admin Panel | P1 — Must-have |
| 10 | Project Structure (.NET 8) | INF-01 | Infrastructure | P1 — Must-have |
| 11 | Database Setup — Config + Logs | INF-02 | Infrastructure | P1 — Must-have |
| 12 | IIS Hosting | INF-03 | Infrastructure | P1 — Must-have |

### خارج النطاق Phase 1

| الميزة | السبب |
|--------|-------|
| Phase 2 (Data Editing, User Roles, Export, Advanced Analytics) | مؤجل — يعتمد على استقرار Core MVP |
| Cloud Hosting | محلي فقط |
| Mobile App | غير مطلوب |
| Microservices | بنية بسيطة كافية |
| CI/CD Pipeline | مشروع محلي |
| Automated Testing | يمكن إضافته لاحقاً |

---

## 6. Technology Stack

| المكوّن | التقنية | المصدر |
|:-------:|---------|:------:|
| **Framework** | .NET 8 (LTS) | CLIENT_DECISION_LOG.md #1 — ✅ Approved |
| **UI Pattern** | ASP.NET Core Razor Pages | CLIENT_DECISION_LOG.md #2 — ✅ Approved |
| **Dashboard Components** | Syncfusion Essential Studio (UnlockKey متوفر) | CLIENT_DECISION_LOG.md #3 — ✅ Approved |
| **Oracle Driver** | ODP.NET (Oracle.ManagedDataAccess) | CLIENT_DECISION_LOG.md #3 — ✅ Approved |
| **Bulk Insert** | ADO.NET SqlBulkCopy | CLIENT_DECISION_LOG.md #21 — ✅ Approved (قرار استراتيجي) |
| **Config + Logs ORM** | Entity Framework Core (للتكوين والسجلات فقط) | CLIENT_DECISION_LOG.md #21 — ✅ Approved |
| **Source Database** | Oracle (read-only) | CLIENT_DECISION_LOG.md #7 — ✅ Approved |
| **Destination Database** | SQL Server (write + config + logs) | CLIENT_DECISION_LOG.md #7 — ✅ Approved |
| **Deployment** | IIS (محلي) | CLIENT_DECISION_LOG.md #6 — ✅ Approved |
| **Visual Identity** | Blue Theme — 11 لوناً محدداً | CLIENT_DECISION_LOG.md #9 — ✅ Approved |
| **Sync Engine** | IHostedService + PeriodicTimer | APPLICATION_BLUEPRINT.md §9 — Candidate |

> **ملاحظة:** القرارات التقنية النهائية مفصلة في `08_TECHNICAL_ARCHITECTURE.md`. هذا القسم يوثق القرارات المعتمدة فقط.

---

## 7. Success Criteria

### معايير النجاح الوظيفي

| # | المعيار | الوصف |
|:-:|---------|-------|
| SC1 | **Oracle Connection ثابت** | الاتصال بقاعدة Oracle يعمل بشكل موثوق مع Error Handling مناسب |
| SC2 | **Full Refresh يعمل بكفاءة** | نقل البيانات من Oracle إلى SQL Server يتم بشكل كامل ضمن معاملة واحدة |
| SC3 | **مزامنة تلقائية ويدوية** | Auto-Sync تعمل حسب الجدول الزمني، و Manual Sync تستجيب فوراً |
| SC4 | **سجلات مزامنة كاملة** | كل عملية Sync مسجلة مع وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة |
| SC5 | **Dashboard ديناميكي** | ~20 بطاقة تُبنى تلقائياً من تكوين SQL Server مع عرض صحيح |
| SC6 | **Drill Down متعدد المستويات** | التعمق في البيانات عبر 2+ مستوى مع Breadcrumb Navigation |
| SC7 | **Admin Panel كامل** | CRUD للبطاقات، تكوين مصدر البيانات، نوع الرسم البياني، Drill Down، تخطيط، Query Tester |
| SC8 | **Admin Panel آمن** | محمي بكلمة مرور، غير مرئي في القائمة، وصول عبر الرابط المباشر فقط |
| SC9 | **الموضوع الأزرق (11 لوناً)** | جميع مكونات Dashboard تطابق الهوية البصرية المحددة |
| SC10 | **تخطيط متجاوب** | Dashboard يعمل بشكل صحيح على مختلف أحجام الشاشات |

### معايير النجاح التجاري

| # | المعيار | القيمة |
|:-:|---------|:------:|
| SC11 | **التسليم ضمن النطاق** | النطاق التقديري: 430 — 625 ساعة |
| SC12 | **رضا العميل** | موافقة العميل على الاستلام بعد اختبار الميزات الأساسية |
| SC13 | **جاهزية الصيانة** | كود نظيف + توثيق أساسي يسمح بالصيانة والتطوير المستقبلي |

---

## 8. Key Constraints

| # | القيد | التفاصيل |
|:-:|-------|----------|
| C1 | **نشر محلي فقط** | التطبيق يُنصب على IIS داخل شبكة العميل — لا سحابة |
| C2 | **Admin Panel بكلمة مرور فقط** | أمان: كلمة مرور فقط — غير مرئية في القائمة — وصول عبر الرابط المباشر. سقف زمني للمشروع (Phase 2 يضيف Role-Based Access) |
| C3 | **الموضوع الأزرق (11 لوناً)** | الهوية البصرية محددة بالكامل ويجب تطبيقها على جميع المكونات |
| C4 | **Full Refresh فقط (Phase 1)** | المزامنة تعمل بحذف الكل + إعادة الإدخال. المحرك مصمم لـ Incremental Sync لاحقاً ولكن غير مفعّل |
| C5 | **Oracle على نفس السيرفر** | Oracle و SQL Server على نفس السيرفر المحلي |
| C6 | **ODP.NET فقط لل Oracle** | لا Oracle Client — نستخدم Managed Driver |
| C7 | **تفاصيل جداول Oracle مؤجلة** | تُحدد أثناء التنفيذ — العميل متاح |
| C8 | **أولوية: API أولاً ← Razor Dashboard** | التطوير يبدأ من Sync Engine، ثم Dashboard، ثم Admin Panel |
| C9 | **بدون CI/CD** | مشروع محلي — لا حاجة لـ CI/CD |
| C10 | **بدون Automated Testing (مبدئياً)** | يمكن إضافته لاحقاً |

---

## 9. Risks

| # | الخطر | التأثير | Severity | التخفيف |
|:-:|-------|---------|:--------:|---------|
| R1 | Full Refresh قد يكون بطيئاً مع بيانات كبيرة | أداء المزامنة | **High** | المحرك مصمم لـ Incremental Sync من البداية |
| R2 | تفاصيل جداول Oracle مؤجلة | يحدد تعقيد Sync Engine | Medium | العميل متاح أثناء التنفيذ |
| R3 | EF Core غير مناسب لـ Bulk Insert | أداء الإدخال | Medium | ADO.NET SqlBulkCopy للبيانات + EF Core للتكوين فقط (قرار معتمد) |
| R4 | Admin Panel بكلمة مرور فقط | أمان منخفض | Low | سقف زمني — Phase 2 يضيف Role-Based Access |
| R5 | Drill Down قد يكون معقداً تقنياً | تعقيد التصميم | Medium | تصميم هيكلي واضح + اختبار مبكر |

---

## 10. Open Questions (Deferred)

| # | السؤال | التأثير | يُحدد |
|:-:|--------|:-------:|:-----:|
| Q1 | تفاصيل جداول Oracle (الاسم، الهيكل، عدد الصفوف) | High | أثناء التنفيذ |
| Q2 | آليات السحب التفصيلية (معاملات، ترتيب، شروط) | Medium | أثناء التنفيذ |
| Q3 | فترة المزامنة التلقائية (افتراضياً 30 دقيقة) | Low | أثناء التنفيذ |
| Q4 | عدد البطاقات الفعلي (~20 تقريباً) | Low | أثناء التطوير |
| Q5 | هل الجداول الكبيرة (100K+ صف) موجودة؟ | High | أثناء التنفيذ — يحدد ما إذا كان Incremental Sync مطلوباً فوراً |

---

## References

| الوثيقة | المسار |
|---------|--------|
| 00_PROJECT_INPUTS.md | `project-preparation/00_PROJECT_INPUTS.md` |
| APPLICATION_BLUEPRINT.md | `project-preparation/APPLICATION_BLUEPRINT.md` |
| FEATURE_LIST.md | `client-engagement/FEATURE_LIST.md` |
| CLIENT_DECISION_LOG.md | `client-engagement/CLIENT_DECISION_LOG.md` |
| BLUEPRINT_OPEN_QUESTIONS.md | `project-preparation/BLUEPRINT_OPEN_QUESTIONS.md` |

---

> **Prepared by:** TeraAgent — TASK-PREP-001
> **Date:** 2026-07-12
> **Status:** `preparation`
