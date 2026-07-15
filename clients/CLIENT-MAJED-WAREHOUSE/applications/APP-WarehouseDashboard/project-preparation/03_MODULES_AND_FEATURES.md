# 03_MODULES_AND_FEATURES.md — WarehouseDashboard

**Status:** `preparation`
**Client:** الماجد لادارة المستودعات
**Application:** WarehouseDashboard
**Prepared by:** TeraAgent — Task TASK-PREP-004
**Date:** 2026-07-12
**Source Documents:** 01_PROJECT_BRIEF.md, APPLICATION_BLUEPRINT.md, FEATURE_LIST.md, CLIENT_DECISION_LOG.md

---

## 1. Module 1: Sync Engine (API)

| البند | التفاصيل |
|-------|----------|
| **الغرض** | استخراج البيانات من Oracle (source, read-only) وتحويلها إلى SQL Server (destination) |
| **التقنية** | ASP.NET Core Web API + ODP.NET (Oracle.ManagedDataAccess) + ADO.NET SqlBulkCopy |
| **الأولوية** | P1 — Core MVP (مطلوب أولاً قبل Dashboard) |
| **التبعية** | لا توجد — مستقل (إلا أنه يعتمد على M4.1, M4.2, M4.3, M4.4 للبنية التحتية) |

| الكود | المكون | الوصف | الأولوية |
|:-----:|--------|-------|:--------:|
| M1.1 | **Oracle Data Extraction** | قراءة جداول Oracle عبر ODP.NET مع Data Type Mapping (Oracle → SQL Server) | P1 — Must-have |
| M1.2 | **Full Refresh Engine** | حذف جميع الصفوف + إدخال مجمّع (SqlBulkCopy) في معاملة واحدة لكل جدول، مع إدارة ترتيب الجداول حسب FK dependencies | P1 — Must-have |
| M1.3 | **Incremental Sync Ready** | المحرك مصمم لدعم Incremental Sync من البداية (Compare + Merge + Upsert) — غير مفعّل في Phase 1، جاهز للتفعيل لاحقاً بدون إعادة كتابة | P1 — Must-have (جاهزية هيكلية) |
| M1.4 | **Auto-Sync Scheduler** | Background Service (IHostedService + PeriodicTimer) للمزامنة التلقائية بفترة قابلة للتكوين (افتراضياً 30 دقيقة) | P1 — Must-have |
| M1.5 | **Manual Sync API** | `POST /api/sync/trigger` — تفعيل المزامنة يدوياً عبر API | P1 — Must-have |
| M1.6 | **Sync Status API** | `GET /api/sync/status` — عرض حالة المزامنة الحالية (Idle, Running, Failed, LastSyncTime) | P1 — Must-have |
| M1.7 | **Sync Logs** | تسجيل كل عملية (وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة، رسالة الخطأ) + `GET /api/sync/logs` لجلب السجلات | P1 — Must-have |

**API Endpoints Summary:**

| Method | Endpoint | الوصف |
|:------:|----------|-------|
| POST | `/api/sync/trigger` | تفعيل مزامنة يدوية |
| GET | `/api/sync/status` | حالة المزامنة الحالية |
| GET | `/api/sync/logs` | سجلات المزامنة |
| GET | `/api/sync/config` | إعدادات المزامنة |
| PUT | `/api/sync/config` | تحديث إعدادات المزامنة |

---

## 2. Module 2: Dashboard (Razor)

| البند | التفاصيل |
|-------|----------|
| **الغرض** | عرض البيانات في Dashboard احترافي ديناميكي مع ~20 بطاقة قابلة للتكوين ودعم Drill Down |
| **التقنية** | ASP.NET Core Razor Pages + Syncfusion Essential Studio (UnlockKey متوفر) |
| **الأولوية** | P1 — Core MVP |
| **التبعية** | **يعتمد على Module 1 (Sync Engine)** — يجب أن تعمل المزامنة أولاً لتتوفر البيانات. يعتمد أيضاً على M4.1, M4.2, M4.4 |

| الكود | المكون | الوصف | الأولوية |
|:-----:|--------|-------|:--------:|
| M2.1 | **Dynamic Card Rendering** | قراءة التكوين من SQL Server (جدول DashboardCards) + بناء البطاقات ديناميكياً في تخطيط شبكي | P1 — Must-have |
| M2.2 | **Drill Down** | التعمق في البيانات عبر مستويات قابلة للتكوين لكل بطاقة (2+ مستويات) — النقر على عنصر ينتقل للمستوى التالي | P1 — Must-have |
| M2.3 | **Breadcrumb Navigation** | شريط تنقل يعرض المستوى الحالي مع إمكانية العودة للمستوى السابق | P1 — Must-have |
| M2.4 | **Sync Status Display** | عرض آخر وقت مزامنة + مؤشر حالة (نجاح/فشل/قيد التنفيذ) + زر مزامنة يدوية + عرض سجلات المزامنة | P1 — Must-have |
| M2.5 | **Data Binding** | تنفيذ SQL Queries أو Views من التكوين وربط النتائج بالبطاقات حسب نوع الرسم البياني | P1 — Must-have |
| M2.6 | **Blue Theme** | تطبيق الهوية البصرية المحددة (11 لوناً أزرق) على جميع مكونات Dashboard | P1 — Must-have |
| M2.7 | **Responsive Layout** | تخطيط شبكي متجاوب لجميع أحجام الشاشات (Desktop, Tablet, Mobile) باستخدام CSS Grid/Flexbox | P1 — Must-have |

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

## 3. Module 3: Admin Panel (Razor)

| البند | التفاصيل |
|-------|----------|
| **الغرض** | لوحة تحكم ديناميكية لإدارة تكوين البطاقات — إنشاء، تعديل، حذف، وتخصيص |
| **التقنية** | ASP.NET Core Razor Pages (صفحة منفصلة عن Dashboard) |
| **الأمان** | محمية بكلمة مرور — غير مرئية في القائمة — وصول عبر الرابط المباشر فقط |
| **الأولوية** | P1 — Core MVP |
| **التبعية** | يعتمد على M4.1, M4.2 للبنية التحتية. مستقل عن Sync Engine (يمكن العمل بالتوازي بعد إعداد DB Schema) |

| الكود | المكون | الوصف | الأولوية |
|:-----:|--------|-------|:--------:|
| M3.1 | **Card CRUD** | إنشاء / تعديل / حذف بطاقات — واجهة كاملة لإدارة البطاقات | P1 — Must-have |
| M3.2 | **Data Source Config** | تحديد مصدر البيانات لكل بطاقة (SQL Query أو View) مع إدخال النص | P1 — Must-have |
| M3.3 | **Chart Type Selection** | اختيار نوع العرض من القائمة (Bar, Line, Pie, KPI, Table, Gauge) | P1 — Must-have |
| M3.4 | **Drill Down Config** | تحديد مستويات الـ Drill Down لكل بطاقة — ربط بطاقة بمستوى تالٍ مع استعلام مخصص | P1 — Must-have |
| M3.5 | **Card Layout Config** | تحديد حجم (عرض/ارتفاع) وترتيب البطاقات في الشبكة | P1 — Must-have |
| M3.6 | **Config Save/Load** | حفظ التكوين في SQL Server وتحميله — تطبيق التغييرات فوراً على Dashboard | P1 — Must-have |
| M3.7 | **Query Tester** | اختبار الاستعلام قبل الحفظ — تنفيذ SQL وعرض النتائج المسبقة مع مساحة معاينة | P1 — Must-have |

**شاشات Admin Panel المقترحة:**

| الشاشة | الوصف |
|:------:|-------|
| Admin Login | صفحة دخول بكلمة مرور |
| Card List | قائمة البطاقات مع خيارات التعديل/الحذف |
| Card Editor | إنشاء/تعديل بطاقة (مصدر البيانات، النوع، التكوين) |
| Drill Down Config | تحديد مستويات الـ Drill Down والربط بين البطاقات |
| Query Tester | اختبار الاستعلام مع عرض النتائج |

---

## 4. Module 4: Infrastructure

| البند | التفاصيل |
|-------|----------|
| **الغرض** | هيكل المشروع، قاعدة البيانات، وإعدادات النشر |
| **التقنية** | .NET 8 (LTS) + SQL Server + IIS |
| **الأولوية** | P1 — Core MVP (أساس لجميع الوحدات) |
| **التبعية** | **لا توجد — هذا هو الأساس.** جميع الوحدات الأخرى (M1, M2, M3) تعتمد على M4 |

| الكود | المكون | الوصف | الأولوية |
|:-----:|--------|-------|:--------:|
| M4.1 | **.NET 8 Solution** | حل Visual Studio يحتوي على مشروعين: `WarehouseDashboard.API` (Web API) + `WarehouseDashboard.Web` (Razor Pages). بنية نظيفة مع فصل الاهتمامات | P1 — Must-have |
| M4.2 | **Config DB Schema** | جداول تخزين تكوين البطاقات: `DashboardCards` (Id, Title, ChartType, SqlQuery, Position, Size, CreatedAt), `CardDrillDownLevels` (Id, CardId, Level, ParentCardId, DrillDownQuery), `SyncSettings` (Id, IntervalMinutes, IsAutoSyncEnabled, LastSyncTime) | P1 — Must-have |
| M4.3 | **Sync Logs Schema** | جدول `SyncLogs` (Id, StartTime, EndTime, Status, RecordCount, Duration, ErrorMessage) | P1 — Must-have |
| M4.4 | **Data Tables Schema** | جداول تخزين البيانات المنقولة من Oracle — تُحدد أثناء التنفيذ حسب جداول العميل (TBD) | P1 — Must-have |
| M4.5 | **IIS Hosting** | إعداد النشر المحلي على IIS — إنشاء Application Pool مناسب، تكوين المسارات (URL Routing)، إعدادات الأمان | P1 — Must-have |

**Entity Framework Core Usage:**

| الجدول | ORM | السبب |
|:------:|:---:|-------|
| DashboardCards | EF Core | بيانات صغيرة — تكوين البطاقات |
| CardDrillDownLevels | EF Core | بيانات صغيرة — تكوين Drill Down |
| SyncSettings | EF Core | بيانات صغيرة — إعدادات المزامنة |
| SyncLogs | EF Core | بيانات متوسطة — سجلات العمليات |
| Data Tables (منقولة) | **ADO.NET SqlBulkCopy** | بيانات كبيرة — أداء عالي مطلوب |

---

## 5. Feature Prioritization

### 5.1 Core MVP — P1 (Must-have)

هذه الميزات تمثل أصغر نسخة قابلة للاستخدام (Minimum Viable Product) ويجب تسليمها كاملة قبل أي توسع. إجمالي المكونات: **33 مكوناً فرعياً في 12 ميزة رئيسية**.

| الكود | الميزة | الوحدة | الأولوية |
|:-----:|--------|:------:|:--------:|
| API-01 | Oracle Connection & Data Extraction | Sync Engine (M1.1) | P1 — Must-have |
| API-02 | SQL Server Data Loading / Full Refresh | Sync Engine (M1.2, M1.3) | P1 — Must-have |
| API-03 | Synchronization Engine — Auto + Manual | Sync Engine (M1.4, M1.5, M1.6) | P1 — Must-have |
| API-04 | Synchronization Logs | Sync Engine (M1.7) | P1 — Must-have |
| RZR-01 | Dashboard Layout & Rendering | Dashboard (M2.1, M2.6, M2.7) | P1 — Must-have |
| RZR-02 | Drill Down Functionality | Dashboard (M2.2, M2.3) | P1 — Must-have |
| RZR-03 | Data Binding & SQL Execution | Dashboard (M2.5) | P1 — Must-have |
| RZR-04 | Sync Status & Controls | Dashboard (M2.4) | P1 — Must-have |
| RZR-05 | Admin Panel — CRUD + Config | Admin Panel (M3.1-M3.7) | P1 — Must-have |
| INF-01 | Project Structure (.NET 8) | Infrastructure (M4.1) | P1 — Must-have |
| INF-02 | Database Setup — Config + Logs | Infrastructure (M4.2, M4.3, M4.4) | P1 — Must-have |
| INF-03 | IIS Hosting | Infrastructure (M4.5) | P1 — Must-have |

### 5.2 Phase 2 — مؤجل (Deferred)

ميزات مؤجلة بعد استقرار Core MVP. لا تُطور أو تُخطط في Phase 1.

| الكود | الميزة | الوصف | السبب |
|:-----:|--------|-------|-------|
| D-1 | **Data Editing Screens** | إضافة، تعديل، حذف بيانات في SQL Server عبر واجهة مستخدم | مؤجل مبكراً بتأكيد Majed |
| D-2 | **User Roles & Authentication** | نظام login + Role-Based Access (Admin/Viewer) | غير محدد — سقف زمني في Phase 1 |
| D-3 | **Export to Excel/PDF** | تصدير البيانات والرسوم البيانية من Dashboard | يمكن إضافته لاحقاً |
| D-4 | **Advanced Analytics** | تحليلات متقدمة وتقارير إحصائية | يمكن إضافته لاحقاً |
| D-5 | **Shared Library (.NET Class Lib)** | مكتبة مشتركة للـ Models والـ DTOs | اختياري — يمكن إضافته عند الحاجة |

### 5.3 Out of Scope — خارج النطاق

ميزات غير مطلوبة وليست ضمن خريطة الطريق.

| الكود | الميزة | السبب |
|:-----:|--------|-------|
| O-1 | **Cloud Hosting (Azure/AWS)** | المشروع محلي على IIS — لا حاجة للسحابة |
| O-2 | **Mobile App** | لا يوجد طلب من العميل |
| O-3 | **Microservices Architecture** | غير مطلوب — بنية بسيطة (2 مشاريع) كافية |
| O-4 | **CI/CD Pipeline** | مشروع محلي — لا حاجة للتسليم المستمر |
| O-5 | **Automated Testing Suite** | يمكن إضافته لاحقاً — خارج النطاق الحالي |

### 5.4 Prioritization Rationale

```
P1 (Core MVP):       ⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛  12 ميزة  ← تسليم إلزامي
Phase 2 (Deferred):  ⬛⬛⬛⬛                       4 ميزات  ← بعد استقرار Phase 1
Out of Scope:        ⬛⬛⬛⬛⬛                     5 ميزات  ← غير مطروحة
```

**مبدأ التحديد:** كل ميزة في P1 ضرورية لتشغيل التطبيق بشكل متكامل — بدون أي منها لا يمكن اعتبار النظام قابلاً للاستخدام. Phase 2 يضيف تحسينات غير حرجة. Out of Scope إما لا تناسب طبيعة المشروع المحلي أو لم يطلبها العميل.

---

## 6. Dependencies Between Modules

### 6.1 Dependency Graph

```
┌─────────────────────────────────────────────────────────────────┐
│                    Dependency Graph                              │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐     │
│  │                    M4: Infrastructure                    │     │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌────────┐  │     │
│  │  │ M4.1     │  │ M4.2     │  │ M4.3     │  │ M4.4   │  │     │
│  │  │ .NET 8   │  │ Config   │  │ Sync     │  │ Data   │  │     │
│  │  │ Solution │  │ DB       │  │ Logs DB  │  │ Tables │  │     │
│  │  └──────────┘  └──────────┘  └──────────┘  └────────┘  │     │
│  └────────────────────────┬────────────────────────────────┘     │
│                           │                                      │
│           ┌───────────────┼───────────────────┐                  │
│           ▼               ▼                    ▼                  │
│  ┌────────────────┐ ┌────────────────┐ ┌──────────────────┐      │
│  │  M1: Sync      │ │  M3: Admin     │ │  M2: Dashboard   │      │
│  │  Engine (API)  │ │  Panel (Razor) │ │  (Razor)         │      │
│  │                │ │                │ │                  │      │
│  │ يعتمد على:     │ │ يعتمد على:     │ │ يعتمد على:       │      │
│  │ ├─ M4.1        │ │ ├─ M4.1        │ │ ├─ M4.1          │      │
│  │ ├─ M4.2        │ │ ├─ M4.2        │ │ ├─ M4.2          │      │
│  │ ├─ M4.3        │ │ └─ M4.4        │ │ ├─ M4.4          │      │
│  │ └─ M4.4        │ │                │ │ └─ **M1** ⬅      │      │
│  └────────────────┘ └────────────────┘ └──────────────────┘      │
│                                           ▲                      │
│                                           │                      │
│                               M1 (Sync Engine) يجب أن يعمل      │
│                               أولاً حتى تتوفر البيانات           │
│                               لعرضها في Dashboard               │
└─────────────────────────────────────────────────────────────────┘
```

### 6.2 Dependency Matrix

| الوحدة | يعتمد على | ملاحظات |
|--------|-----------|---------|
| **M1: Sync Engine** | M4.1 (.NET 8), M4.2 (Config DB), M4.3 (Logs DB), M4.4 (Data Tables) | يحتاج البنية التحتية كاملة |
| **M2: Dashboard** | M4.1, M4.2, M4.4, **M1 (Sync Engine)** | ⚠️ لا يمكن عرض البيانات بدون مزامنة |
| **M3: Admin Panel** | M4.1, M4.2, M4.4 | يمكن العمل بالتوازي مع M1 بعد تجهيز DB |
| **M4: Infrastructure** | لا توجد | الأساس — يُبنى أولاً |

### 6.3 Critical Path (التسلسل الإلزامي)

```
الخطوة 1: M4.1 (.NET 8 Solution) + M4.2, M4.3, M4.4 (Database Schema)
    ↓
الخطوة 2: M1 (Sync Engine) + M3 (Admin Panel) — يمكن العمل بالتوازي
    ↓
الخطوة 3: M2 (Dashboard) — بعد توفر البيانات من M1
    ↓
الخطوة 4: M4.5 (IIS Hosting) — نشر التطبيق الكامل
```

**ملاحظة هامة:**
- يمكن تطوير **Admin Panel (M3)** بالتوازي مع **Sync Engine (M1)** لأن كليهما يعتمد على نفس البنية التحتية.
- **Dashboard (M2)** يجب أن ينتظر حتى يكتمل **Sync Engine (M1)** جزئياً على الأقل — يحتاج بيانات حقيقية للاختبار.
- **IIS Hosting (M4.5)** هو الخطوة الأخيرة بعد اكتمال جميع المكونات.

---

## 7. Module Summary

| الوحدة | الكود | عدد المكونات | الأولوية | التقنية | التبعية |
|:------:|:-----:|:------------:|:--------:|---------|:-------:|
| Sync Engine | M1 | 7 (M1.1-M1.7) | P1 | ASP.NET Core API + ODP.NET + SqlBulkCopy | M4 |
| Dashboard | M2 | 7 (M2.1-M2.7) | P1 | Razor Pages + Syncfusion | M4, **M1** |
| Admin Panel | M3 | 7 (M3.1-M3.7) | P1 | Razor Pages | M4 |
| Infrastructure | M4 | 5 (M4.1-M4.5) | P1 | .NET 8 + SQL Server + IIS | لا توجد |
| **المجموع** | — | **26** | P1 | — | — |
| **Phase 2** | D | **5** | P2 (مؤجل) | — | بعد Phase 1 |
| **Out of Scope** | O | **5** | — | — | غير مطروح |

---

## References

| الوثيقة | المسار |
|---------|--------|
| 01_PROJECT_BRIEF.md | `project-preparation/01_PROJECT_BRIEF.md` |
| APPLICATION_BLUEPRINT.md | `project-preparation/APPLICATION_BLUEPRINT.md` |
| FEATURE_LIST.md | `client-engagement/FEATURE_LIST.md` |
| CLIENT_DECISION_LOG.md | `client-engagement/CLIENT_DECISION_LOG.md` |

---

> **Prepared by:** TeraAgent — TASK-PREP-004
> **Date:** 2026-07-12
> **Status:** `preparation`
