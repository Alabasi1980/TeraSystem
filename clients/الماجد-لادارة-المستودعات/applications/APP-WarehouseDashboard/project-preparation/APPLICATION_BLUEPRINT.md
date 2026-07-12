# APPLICATION_BLUEPRINT.md — WarehouseDashboard

**Status:** `approved_for_preparation` ✅
**Approved by:** Majed (2026-07-12)
**Conditions:** 3 تعديلات استراتيجية مطبّقة (Incremental Sync Ready, ADO.NET, Oracle Testing Early)
**Client:** الماجد لادارة المستودعات
**Application:** WarehouseDashboard
**Prepared by:** ApplicationBlueprintAgent (مُهندس)
**Date:** 2026-07-12
**Last Updated:** 2026-07-12

---

## Self-Verification Gate (§13)

```
Self-Verification Gate:
- High confidence sections: [Overview, Handoff Reference, Status, Modules, Workflow, Screens, Technical, Risks, Next Focus]
- Medium confidence sections: [User Roles, Open Questions]
- Low confidence sections: [Data/Entity Landscape — deferred by Majed]
- Assumptions made: 4 (Sync interval TBD, Admin password storage, Config DB schema, Incremental Sync ready)
- Gate result: PASS
```

### Majed Feedback Applied (2026-07-12)

| التعديل | التفاصيل | الحالة |
|:-------:|---------|:------:|
| Incremental Sync Ready | المحرك مصمم لدعم Incremental من البداية (ولو لم يُفعّل) | ✅ تم |
| ADO.NET for Bulk Insert | SqlBulkCopy للبيانات + EF Core للتكوين فقط | ✅ تم |
| Oracle Testing Early | اختبار الاتصال في أول 3 أيام | ✅ تم |
| Risk Assessment Update | R1 = High (Full Refresh) | ✅ تم |

---

## 1. Application Overview

**WarehouseDashboard** هو نظام محلي لإدارة المستودعات يتكون من تطبيقين:

1. **API Application** — يستخرج جداول من قاعدة Oracle ويحوّلها إلى SQL Server باستخدام طريقة Full Refresh (حذف + إعادة إدخال). يدعم المزامنة التلقائية واليدوية مع تسجيل العمليات.

2. **Razor Dashboard** — يعرض البيانات في Dashboard احترافي مع ~20 بطاقة ديناميكية قابلة للتكوين عبر Admin Panel منفصل. يدعم Drill Down عبر مستويات متعددة.

**الهدف:** توفير رؤية واضحة وتفاعلية للبيانات المخزنة في المستودعات عبر Dashboard ديناميكي يُبنى تلقائياً من تكوين مخزن في قاعدة البيانات.

---

## 2. Confirmed Handoff Reference

| الوثيقة | المسار | الحالة |
|---------|--------|:------:|
| TERA_HANDOFF_PACKAGE.md | `client-engagement/TERA_HANDOFF_PACKAGE.md` | ✅ معتمدة |
| CLIENT_INTAKE.md | `client-engagement/CLIENT_INTAKE.md` | ✅ |
| FEATURE_LIST.md | `client-engagement/FEATURE_LIST.md` | ✅ |
| DISCOVERY_COVERAGE_SUMMARY.md | `client-engagement/DISCOVERY_COVERAGE_SUMMARY.md` | ✅ |
| CLIENT_DECISION_LOG.md | `client-engagement/CLIENT_DECISION_LOG.md` | ✅ (19 قراراً) |
| DRAFT_QUOTATION.md | `client-engagement/DRAFT_QUOTATION.md` | ✅ |

**Handoff Status:** ✅ مؤكد من Majed (2026-07-12)

---

## 3. Blueprint Status

| البند | القيمة |
|:-----:|--------|
| **Current Status** | `draft` |
| **Next Status** | `pending_confirmation` → `approved_for_preparation` |
| **Target** | `approved_for_preparation` (بعد اعتماد Majed) |

---

## 4. Proposed Modules / Major Capabilities

### Module 1: Sync Engine (API)

| البند | التفاصيل |
|-------|----------|
| **الغرض** | استخراج البيانات من Oracle وتحويلها إلى SQL Server |
| **التقنية** | ASP.NET Core Web API + ODP.NET + Entity Framework Core |
| **الوضع** | `Confirmed by Majed` |

**القدرات الرئيسية:**

| # | القدرة | الوصف |
|:-:|--------|-------|
| M1.1 | Oracle Data Extraction | قراءة جداول Oracle عبر ODP.NET مع Data Type Mapping |
| M1.2 | SQL Server Full Refresh | حذف جميع الصفوف + إدخال مجمّع جديد في معاملة واحدة |
| M1.3 | **SQL Server Incremental Sync (جاهز)** | **المحرك مصمم لدعم Incremental Sync من البداية (ولو لم يُفعّل في Phase 1)** — يدعم Compare + Merge + Upsert |
| M1.4 | Auto-Sync Scheduler | Background Service للمزامنة التلقائية بفترة قابلة للتكوين |
| M1.5 | Manual Sync API | POST /api/sync/trigger لتفعيل المزامنة يدوياً |
| M1.6 | Sync Status API | GET /api/sync/status لعرض حالة المزامنة الحالية |
| M1.7 | Sync Logs | تسجيل كل عملية (وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة) |

**API Endpoints المقترحة:**

| Method | Endpoint | الوصف |
|:------:|----------|-------|
| POST | `/api/sync/trigger` | تفعيل مزامنة يدوية |
| GET | `/api/sync/status` | حالة المزامنة الحالية |
| GET | `/api/sync/logs` | سجلات المزامنة |
| GET | `/api/sync/config` | إعدادات المزامنة |
| PUT | `/api/sync/config` | تحديث إعدادات المزامنة |

---

### Module 2: Dashboard (Razor)

| البند | التفاصيل |
|-------|----------|
| **الغرض** | عرض البيانات في Dashboard احترافي مع Drill Down |
| **التقنية** | ASP.NET Core Razor Pages + Syncfusion |
| **الوضع** | `Confirmed by Majed` |

**القدرات الرئيسية:**

| # | القدرة | الوصف |
|:-:|--------|-------|
| M2.1 | Dynamic Card Rendering | قراءة التكوين من DB + بناء البطاقات ديناميكياً |
| M2.2 | Drill Down | التعمق في البيانات عبر مستويات قابلة للتكوين |
| M2.3 | Breadcrumb Navigation | شريط تنقل يعرض المستوى الحالي مع إمكانية العودة |
| M2.4 | Sync Status Display | عرض آخر وقت مزامنة + مؤشر الحالة + زر المزامنة |
| M2.5 | Data Binding | تنفيذ SQL Queries/Views من التكوين وربط النتائج بالبطاقات |
| M2.6 | Blue Theme | تطبيق الهوية البصرية المحددة (11 لوناً) |
| M2.7 | Responsive Layout | تخطيط شبكي متجاوب لجميع أحجام الشاشات |

**أنواع البطاقات المدعومة:**

| النوع | الاستخدام |
|:-----:|-----------|
| Bar Chart | مقارنة بين فئات |
| Line Chart | اتجاهات زمنية |
| Pie Chart | توزيع نسبي |
| KPI Card | رقم رئيسي مع مؤشر |
| Data Table | جدول تفصيلي |
| Gauge | مؤشر أداء |

---

### Module 3: Admin Panel (Razor)

| البند | التفاصيل |
|-------|----------|
| **الغرض** | لوحة تحكم ديناميكية لإدارة تكوين البطاقات |
| **التقنية** | ASP.NET Core Razor Pages (صفحة منفصلة) |
| **الوضع** | `Confirmed by Majed` |
| **الأمان** | كلمة مرور فقط — غير مرئية في القائمة — وصول عبر الرابط المباشر |

**القدرات الرئيسية:**

| # | القدرة | الوصف |
|:-:|--------|-------|
| M3.1 | Card CRUD | إنشاء / تعديل / حذف بطاقات |
| M3.2 | Data Source Config | تحديد مصدر البيانات (SQL Query / View) |
| M3.3 | Chart Type Selection | اختيار نوع العرض (Bar, Line, Pie, KPI, Table, Gauge) |
| M3.4 | Drill Down Config | تحديد مستويات الـ Drill Down والربط بين البطاقات |
| M3.5 | Card Layout Config | تحديد حجم وترتيب البطاقات في الشبكة |
| M3.6 | Config Save/Load | حفظ التكوين في SQL Server وتحميله |
| M3.7 | Query Tester | اختبار الاستعلام قبل الحفظ |

---

### Module 4: Infrastructure (Shared)

| البند | التفاصيل |
|-------|----------|
| **الغرض** | هيكل المشروع وقاعدة البيانات والنشر |
| **التقنية** | .NET 8 + SQL Server + IIS |
| **الوضع** | `Confirmed by Majed` |

**القدرات الرئيسية:**

| # | القدرة | الوصف |
|:-:|--------|-------|
| M4.1 | .NET 8 Solution | حل يحتوي على مشروعين (API + Razor) |
| M4.2 | Config DB Schema | جداول تخزين تكوين البطاقات |
| M4.3 | Sync Logs Schema | جداول تخزين سجلات المزامنة |
| M4.4 | Data Tables Schema | جداول تخزين البيانات المنقولة (TBD) |
| M4.5 | IIS Hosting | إعداد النشر على IIS داخلي |

---

## 5. Proposed User Roles / Operational Actors

| الدور | الوصف | الصلاحيات | المصدر |
|:-----:|-------|:----------:|:------:|
| **Admin** | مسؤول النظام الرئيسي | الوصول لل Admin Panel + إدارة البطاقات + تفعيل المزامنة | `Confirmed by Majed` |
| **Viewer** (مؤجل) | مستخدم عادي لعرض Dashboard | عرض فقط — لا تعديل | Phase 2 |

> **ملاحظة:** عدد المستخدمين الفعلي غير محدد بعد — يُحدد أثناء التطوير. الزبون متاح للإجابة.

---

## 6. Proposed Workflow Shape

### Workflow 1: Data Synchronization

```
┌─────────────────────────────────────────────────────────────┐
│                    Data Sync Workflow                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ Trigger  │───▶│ Extract from │───▶│ Transform    │      │
│  │ (Auto/   │    │ Oracle       │    │ Data Types   │      │
│  │ Manual)  │    │              │    │              │      │
│  └──────────┘    └──────────────┘    └──────┬───────┘      │
│                                             │              │
│                                             ▼              │
│  ┌──────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ Log      │◀───│ Bulk Insert  │◀───│ Delete All   │      │
│  │ Result   │    │ to SQL Server│    │ (Full Refresh│      │
│  │          │    │              │    │  per table)  │      │
│  └──────────┘    └──────────────┘    └──────────────┘      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Workflow 2: Dashboard Rendering

```
┌─────────────────────────────────────────────────────────────┐
│                   Dashboard Workflow                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ Page     │───▶│ Read Card    │───▶│ Execute SQL  │      │
│  │ Load     │    │ Configs from │    │ Query/View   │      │
│  │          │    │ DB           │    │              │      │
│  └──────────┘    └──────────────┘    └──────┬───────┘      │
│                                             │              │
│                                             ▼              │
│  ┌──────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ User     │◀───│ Render Card  │◀───│ Bind Data    │      │
│  │ Interacts│    │ (Syncfusion) │    │ to Chart     │      │
│  │ (Drill)  │    │              │    │              │      │
│  └──────────┘    └──────────────┘    └──────────────┘      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Workflow 3: Admin Panel Card Configuration

```
┌─────────────────────────────────────────────────────────────┐
│                  Admin Config Workflow                       │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ Login    │───▶│ Create/Edit  │───▶│ Configure    │      │
│  │ (Pass-   │    │ Card         │    │ Data Source  │      │
│  │  word)   │    │              │    │ (SQL/View)   │      │
│  └──────────┘    └──────────────┘    └──────┬───────┘      │
│                                             │              │
│                                             ▼              │
│  ┌──────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ Dashboard│◀───│ Save Config  │◀───│ Test Query   │      │
│  │ Updated  │    │ to DB        │    │ (Preview)    │      │
│  │          │    │              │    │              │      │
│  └──────────┘    └──────────────┘    └──────────────┘      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 7. Proposed Screen / Interface Landscape

### Dashboard (RZR-01)

| الشاشة | الوصف | النوع |
|:------:|-------|:-----:|
| Dashboard Main | عرض ~20 بطاقة في تخطيط شبكي | Syncfusion Grid |
| Card Detail | عرض تفاصيل بطاقة مع Drill Down | Syncfusion Chart |
| Sync Status Bar | شريط عرض حالة المزامنة + زر يدوي | Component |

### Admin Panel (RZR-05)

| الشاشة | الوصف | النوع |
|:------:|-------|:-----:|
| Admin Login | صفحة دخول بكلمة مرور | Razor Page |
| Card List | قائمة البطاقات الموجودة مع خيارات التعديل/الحذف | Razor Page |
| Card Editor | إنشاء/تعديل بطاقة (مصدر البيانات، النوع، التكوين) | Razor Page |
| Query Tester | اختبار الاستعلام مع عرض النتائج المسبقة | Razor Page |
| Drill Down Config | تحديد مستويات الـ Drill Down والربط | Razor Page |

### Sync Logs

| الشاشة | الوصف | النوع |
|:------:|-------|:-----:|
| Logs List | عرض سجلات المزامنة مع التفاصيل | Razor Page |

---

## 8. Proposed Data / Entity Landscape

> ⚠️ **ملاحظة:** تفاصيل الجداول وهياكلها مؤجلة لوقت التنفيذ بتأكيد Majed. ما يلي هو الإطار العام فقط.

### SQL Server — Config Tables (مؤكد)

| الجدول | الغرض | الحقول الأساسية |
|:------:|-------|-----------------|
| `DashboardCards` | تكوين البطاقات | Id, Title, ChartType, SqlQuery, Position, Size, CreatedAt |
| `CardDrillDownLevels` | مستويات الـ Drill Down | Id, CardId, Level, ParentCardId, DrillDownQuery |
| `SyncSettings` | إعدادات المزامنة | Id, IntervalMinutes, IsAutoSyncEnabled, LastSyncTime |

### SQL Server — Sync Logs (مؤكد)

| الجدول | الغرض | الحقول الأساسية |
|:------:|-------|-----------------|
| `SyncLogs` | سجلات المزامنة | Id, StartTime, EndTime, Status, RecordCount, Duration, ErrorMessage |

### SQL Server — Data Tables (مؤجل)

> جداول البيانات المنقولة من Oracle تُحدد أثناء التنفيذ حسب احتياج العميل.

### Oracle — Source Tables (مؤجل)

> جداول المصدر في Oracle تُحدد أثناء التنفيذ — الزبون متاح للإجابة.

---

## 9. Technical Decision Candidates

> **ملاحظة:** القرارات التالية هي candidates/recommendations فقط — لا تُعتبر قرارات نهائية.

### Candidate 1: .NET 8 (LTS)

| البند | التفاصيل |
|-------|----------|
| **التوصية** | .NET 8 — الأحدث والأكثر دعماً |
| **المبرر** | LTS support حتى 2026، أداء أعلى، مكتبات أحدث |
| **الحالة** | `Confirmed by Majed` |

### Candidate 2: Syncfusion Dashboard

| البند | التفاصيل |
|-------|----------|
| **التوصية** | Syncfusion Essential Studio مع Community License |
| **المبرر** | 1,600+ مكون، دعم ممتاز لـ ASP.NET Core، Drill Down مدمج |
| **البديل** | Chart.js + D3.js (إذا لم تنطبق الرخصة) |
| **الحالة** | `Confirmed by Majed` — مفتاح UnlockKey متوفر |

### Candidate 3: ODP.NET for Oracle

| البند | التفاصيل |
|-------|----------|
| **التوصية** | Oracle.ManagedDataAccess (ODP.NET Managed Driver) |
| **المبرر** | اتصال مباشر مع Oracle — لا يحتاج Oracle Client |
| **البديل** | Oracle.EntityFrameworkCore (إذا احتجنا EF Core) |
| **الحالة** | `Confirmed by Majed` |

### Candidate 4: ADO.NET for Bulk Insert (مُعتمد من Majed)

| البند | التفاصيل |
|-------|----------|
| **التوصية** | **ADO.NET مع SqlBulkCopy** للإدخال المجمّع |
| **المبرر** | أسرع بـ 3-5x من EF Core للبيانات الكبيرة — الأفضل لهذا المهمة تحديداً |
| **الاستخدام** | Bulk Insert فقط (ndata Loading) |
| **EF Core** | يُستخدم فقط لـ Config Tables + Sync Logs (البيانات الصغيرة) |
| **الحالة** | `Confirmed by Majed` — قرار استراتيجي |

### Candidate 5: Background Service for Auto-Sync

| البند | التفاصيل |
|-------|----------|
| **التوصية** | `IHostedService` مع `PeriodicTimer` |
| **المبرر** | جزء من .NET 8 بدون اعتمادات إضافية |
| **البديل** | Hangfire (إذا احتجنا scheduling متقدم) |
| **الحالة** | `Candidate` — IHostedService كافٍ حالياً |

### Candidate 5.1: Oracle Connection Testing Early (توصية Majed)

| البند | التفاصيل |
|-------|----------|
| **التوصية** | **اختبار الاتصال بـ Oracle في أول 3 أيام من التحضير** |
| **المبرر** | يقلل مخاطر اكتشاف مشاكل الاتصال متأخراً |
| **الحالة** | `Confirmed by Majed` — توصية استراتيجية |

### Candidate 6: Password Hashing for Admin Panel

| البند | التفاصيل |
|-------|----------|
| **التوصية** | BCrypt.Net أو PBKDF2 |
| **المبرر** | تشفير كلمة المرور قبل الحفظ |
| **البديل** | Hashids (إذا كنا نحفظ المعرف فقط) |
| **الحالة** | `Candidate` |

---

## 10. Risks and Constraints

| # | الخطر | التأثير | المخاطرة | التخفيف |
|:-:|-------|---------|:--------:|---------|
| 1 | تفاصيل الجداول مؤجلة | يحدد تعقيد Sync Engine و DB Schema | Medium | الزبون متاح أثناء التنفيذ |
| 2 | عدد المستخدمين غير محدد | لا يؤثر على Phase 1 | Low | يُحدد لاحقاً |
| 3 | Syncfusion Community License | يجب التحقق من الأهلية | Low | مفتاح UnlockKey متوفر فعلياً |
| 4 | **Full Refresh قد يكون بطيئاً مع بيانات كبيرة** | أداء المزامنة | **High** | **المحرك مصمم لـ Incremental Sync من البداية (ولو لم يُفعّل) — قرار Majed** |
| 5 | Admin Panel بكلمة مرور فقط | أمان منخفض | Low | Phase 2 يضيف Role-Based Access — **ضع سقفاً زمنياً** |
| 6 | **EF Core غير مناسب لـ Bulk Insert** | أداء الإدخال | **Medium** | **ADO.NET SqlBulkCopy للبيانات + EF Core للتكوين فقط — قرار Majed** |
| 7 | Drill Down قد يكون معقداً تقنياً | تعقيد التصميم | Medium | تصميم هيكلي واضح + اختبار مبكر |
| 8 | IIS Hosting محلي فقط | لا يمكن الوصول من خارج الشبكة | Low | مقصود — المشروع محلي |

---

## 11. Open Questions

> تفصيل كامل في `BLUEPRINT_OPEN_QUESTIONS.md`

| # | السؤال | التأثير | الحالة |
|:-:|-------|:-------:|:------:|
| 1 | تفاصيل جداول Oracle (الاسم، الهيكل، عدد الصفوف) | High | يُحدد أثناء التنفيذ |
| 2 | آليات السحب التفصيلية (参数، ترتيب، شروط) | Medium | يُحدد أثناء التنفيذ |
| 3 | فترة المزامنة التلقائية (كل كم دقيقة/ساعة) | Low | يُحدد أثناء التنفيذ |
| 4 | عدد البطاقات الفعلي (~20 تقريباً) | Low | يُحدد أثناء التطوير |
| 5 | هل يحتاج Dashboard لـ Authentication؟ | Low | يُحدد أثناء التطوير |
| 6 | **هل الجداول الكبيرة (100K+ صف)؟** | **High** | **يُحدد أثناء التنفيذ — يحدد إذا كان Incremental Sync مطلوباً فوراً** |

---

## 12. Recommended Next Preparation Focus

بناءً على Blueprint، الخطوات التالية المقترحة:

| الترتيب | الخطوة | المسؤول |
|:-------:|--------|:-------:|
| 1 | **تجهيز Solution Structure** — إنشاء .NET 8 Solution مع مشروعين | TeraAgent |
| 2 | **Configure DB Schema** — تصميم جداول Config + Logs في SQL Server | TeraAgent |
| 3 | **Oracle Connection Setup** — تكوين اتصال ODP.NET مع тест اتصال | TeraAgent |
| 4 | **Sync Engine Core** — بناء محرك المزامنة الأساسي (Full Refresh) | TeraAgent |
| 5 | **Dashboard Layout** — إعداد التخطيط الأساسي مع Syncfusion | TeraAgent |
| 6 | **Admin Panel Basics** — صفحة الدخول + CRUD للبطاقات | TeraAgent |

> **الأولوية:** API أولاً ← ثم Razor Dashboard (كما في `Confirmed by Majed`)

---

## 13. Files Produced

| الملف | الموقع | الغرض |
|-------|--------|-------|
| APPLICATION_BLUEPRINT.md | `project-preparation/` | هذا الملف — Blueprint الرئيسي |
| BLUEPRINT_OPEN_QUESTIONS.md | `project-preparation/` | الأسئلة المفتوحة والافتراضات |

---

> **Blueprint Status:** `draft` — بانتظار اعتماد Majed
> **Next Gate:** Blueprint Confirmation Gate (§10)
> **Produced by:** ApplicationBlueprintAgent (مُهندس) — 2026-07-12
