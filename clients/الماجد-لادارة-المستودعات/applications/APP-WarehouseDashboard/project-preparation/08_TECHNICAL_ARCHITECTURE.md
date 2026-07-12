# 08_TECHNICAL_ARCHITECTURE.md — WarehouseDashboard

> **النوع:** Technical Architecture Document — قرارات معمارية نهائية
> **الحالة:** `Module Baseline Approved`
> **Baseline Module:** WarehouseDashboard
> **تاريخ الاعتماد:** 2026-07-12
> **الجهة المعتمدة:** Majed (عبر CLIENT_DECISION_LOG.md — 23 قراراً)
> **الملف المرجعي للتكنولوجيا:** `tera-system/profiles/dotnet-razorpages-adonet.md`
> **إعداد:** Software Designer Agent (مُصمم)

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Module Baseline Approved` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | N/A — ثابت طوال Phase 1 |
| **Document Type** | Technical Blueprint — ثابت بعد الاعتماد |
| **Version** | 1.0.0 |

---

## نبذة عن الوثيقة

توثق هذه الوثيقة البنية المعمارية النهائية لتطبيق **WarehouseDashboard** — وهو نظام محلي لاستخراج البيانات من Oracle وعرضها في Dashboard ديناميكي مع إمكانية التكوين عبر Admin Panel. جميع القرارات التكنولوجية والمعمارية في هذه الوثيقة معتمدة من Majed عبر `CLIENT_DECISION_LOG.md` (23 قراراً) ومتوافقة مع `dotnet-razorpages-adonet` Technology Profile.

---

## 1. Solution Architecture Overview

### 1.1 الهيكل العام

الحل يتكون من **مشروعين** ضمن Solution واحد (.NET 8):

| المشروع | النوع | الوظيفة |
|---------|-------|---------|
| `WarehouseDashboard.Api` | Worker Service / Console Application | **Sync Engine** — استخراج البيانات من Oracle ← تحويل ← تحميل إلى SQL Server |
| `WarehouseDashboard.Web` | ASP.NET Core Razor Pages | **Dashboard + Admin Panel** — عرض البيانات + إدارة التكوين |

### 1.2 مبدأ الفصل بين المسؤوليات (Separation of Concerns)

```
┌─────────────────────────────────────────────────────────────────────┐
│                        WarehouseDashboard Solution                   │
├─────────────────────────────┬───────────────────────────────────────┤
│   WarehouseDashboard.Api    │       WarehouseDashboard.Web          │
│   (Sync Engine)             │       (Dashboard + Admin)             │
│                             │                                       │
│   ┌───────────────────┐     │   ┌─────────────────────────┐         │
│   │ Oracle Extraction │     │   │ Dashboard Razor Pages   │         │
│   │ (ODP.NET)         │     │   │ (Syncfusion Components) │         │
│   └────────┬──────────┘     │   └───────────┬─────────────┘         │
│            │                │               │                       │
│            ▼                │               ▼                       │
│   ┌───────────────────┐     │   ┌─────────────────────────┐         │
│   │ Data Transform    │     │   │ Admin Panel Razor Pages │         │
│   │ (Type Mapping)    │     │   │ (Password Protected)    │         │
│   └────────┬──────────┘     │   └───────────┬─────────────┘         │
│            │                │               │                       │
│            ▼                │               ▼                       │
│   ┌───────────────────┐     │   ┌─────────────────────────┐         │
│   │ SqlBulkCopy Load  │     │   │ DashboardService        │         │
│   │ (SQL Server)      │     │   │ (Data Query + Binding)  │         │
│   └───────────────────┘     │   └─────────────────────────┘         │
│                             │                                       │
│      ──── يعمل بشكل ────    │         ──── يعمل بشكل ────           │
│      مستقل كـ Background    │         متصل مع SQL Server             │
│      Service                │         (قراءة تكوين + بيانات)        │
└─────────────────────────────┴───────────────────────────────────────┘
                                │
                                ▼
                     ┌─────────────────────┐
                     │     SQL Server      │
                     │  (Config + Logs +   │
                     │   Data Tables)      │
                     └─────────────────────┘
```

### 1.3 قواعد الفصل بين المشروعين

| القاعدة | التفاصيل |
|---------|----------|
| **عدم الاعتماد المباشر** | الـ Web لا يستدعي Api مباشر — كلاهما يقرأ/يكتب في SQL Server |
| **مشاركة قاعدة البيانات فقط** | النقطة المشتركة الوحيدة هي SQL Server |
| **لا SignalR أو اتصال مباشر** | Dashboard يعرض آخر بيانات متزامنة — لا يحتاج اتصال مباشر مع Api |
| **الـ Api لا يحتوي واجهة مستخدم** | Api هو Background Service بدون UI — فقط REST endpoints للتحكم |
| **الـ Web لا يحتوي منطق مزامنة** | Web يقرأ البيانات فقط — لا ينسق أو يشغّل Sync |

---

## 2. Technology Stack

### 2.1 الجدول الرئيسي

| المكون | التقنية | الإصدار / ملاحظات |
|--------|---------|-------------------|
| **لغة البرمجة** | C# | .NET 8 LTS |
| **Framework** | ASP.NET Core | .NET 8 LTS |
| **نمط الواجهة** | Razor Pages | Server-rendered — بدون SPA |
| **مكتبة Dashboard** | Syncfusion Essential Studio | Community License (UnlockKey موجود) |
| **Oracle Connector** | ODP.NET (Managed) | `Oracle.ManagedDataAccess.Core` |
| **SQL Server Connector** | Microsoft.Data.SqlClient | مدمج مع .NET 8 |
| **Bulk Insert** | ADO.NET SqlBulkCopy | `Microsoft.Data.SqlClient.SqlBulkCopy` |
| **ORM (Config + Logs)** | Entity Framework Core | `Microsoft.EntityFrameworkCore.SqlServer` |
| **Database — مصدر** | Oracle Database | قراءة فقط — عبر ODP.NET |
| **Database — وجهة** | SQL Server | Config + Logs + Data Tables |
| **التوقيت والجدولة** | `System.Threading.PeriodicTimer` | مدمج — لا حاجة لـ Hangfire |
| **تشفير كلمة المرور** | BCrypt.Net | لإدارة كلمة مرور Admin Panel |
| **Hosting** | IIS | Local deployment |
| **Build/Runtime** | .NET Runtime 8.x + ASP.NET Core 8.x Hosting Bundle | مثبت على سيرفر العميل |

### 2.2 الإصدارات المحددة للحزم (NuGet)

| الحزمة | الإصدار المستهدف | الغرض |
|--------|------------------|-------|
| `Oracle.ManagedDataAccess.Core` | 3.x (أحدث مستقر) | اتصال Oracle |
| `Microsoft.Data.SqlClient` | 5.x (أحدث مستقر) | اتصال SQL Server + SqlBulkCopy |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.x (متوافق مع .NET 8) | ORM للـ Config + Logs |
| `Microsoft.EntityFrameworkCore.Design` | 8.x | أدوات Migrations |
| `BCrypt.Net-Next` | 4.x | تشفير كلمة مرور Admin |
| `Syncfusion.Licensing` | أحدث إصدار | تسجيل رخصة Syncfusion |
| `Syncfusion.Blazor` | أحدث إصدار | مكونات Syncfusion (لتسجيل الترخيص فقط — لا يُستخدم Blazor فعلياً) |

> **ملاحظة:** `Syncfusion.Blazor` يُضاف فقط لغرض تسجيل الترخيص عبر `SyncfusionLicenseProvider`. الواجهة الفعلية هي Razor Pages.

### 2.3 إصدارات البنية التحتية

| المكون | الإصدار |
|--------|---------|
| .NET SDK | 8.0.x |
| SQL Server | 2019+ (أو Express) |
| Oracle | أي إصدار مدعوم من ODP.NET Managed Driver |
| IIS | 10+ (Windows Server 2019+) |

---

## 3. Project Structure

### 3.1 الهيكل الكامل للمشروع

```
WarehouseDashboard/
│
├── WarehouseDashboard.sln
│
├── src/
│   │
│   ├── WarehouseDashboard.Api/                    # Sync Engine
│   │   ├── WarehouseDashboard.Api.csproj
│   │   ├── Program.cs                              # Host builder + DI + Syncfusion license
│   │   ├── appsettings.json                        # Connection strings + sync interval
│   │   ├── appsettings.Development.json
│   │   │
│   │   ├── Services/
│   │   │   ├── OracleExtractionService.cs          # ODP.NET — قراءة من Oracle
│   │   │   ├── SqlServerLoadService.cs             # SqlBulkCopy — كتابة إلى SQL Server
│   │   │   ├── SyncEngineService.cs                # BackgroundService — تنسيق المزامنة
│   │   │   └── SyncLogService.cs                   # EF Core — تسجيل عمليات المزامنة
│   │   │
│   │   ├── Models/
│   │   │   ├── SyncStatus.cs                       # نموذج حالة المزامنة
│   │   │   ├── TableMapping.cs                     # ربط جداول Oracle ← SQL Server
│   │   │   └── SyncResult.cs                       # نتيجة المزامنة (سجلات، مدة، حالة)
│   │   │
│   │   ├── Data/
│   │   │   └── WarehouseDashboardLogContext.cs     # EF Core DbContext — فقط لسجلات المزامنة
│   │   │
│   │   └── Controllers/ (أو Minimal APIs)
│   │       └── SyncController.cs                   # REST endpoints: trigger, status, logs
│   │
│   ├── WarehouseDashboard.Web/                     # Dashboard + Admin
│   │   ├── WarehouseDashboard.Web.csproj
│   │   ├── Program.cs                              # Builder + Syncfusion license + Middleware
│   │   ├── appsettings.json                        # Connection strings (SQL Server فقط)
│   │   ├── appsettings.Development.json
│   │   │
│   │   ├── Pages/
│   │   │   ├── _ViewStart.cshtml
│   │   │   ├── _ViewImports.cshtml
│   │   │   ├── _Layout.cshtml                      # Layout مع شريط الحالة
│   │   │   │
│   │   │   ├── Index.cshtml                        # Dashboard الرئيسي
│   │   │   ├── Index.cshtml.cs
│   │   │   │
│   │   │   ├── Dashboard/
│   │   │   │   ├── Index.cshtml                    # عرض البطاقات الديناميكية
│   │   │   │   ├── Index.cshtml.cs
│   │   │   │   ├── DrillDown.cshtml                # صفحة Drill Down
│   │   │   │   └── DrillDown.cshtml.cs
│   │   │   │
│   │   │   ├── Admin/
│   │   │   │   ├── Login.cshtml                    # صفحة دخول Admin
│   │   │   │   ├── Login.cshtml.cs
│   │   │   │   ├── DashboardCards.cshtml           # قائمة البطاقات (CRUD)
│   │   │   │   ├── DashboardCards.cshtml.cs
│   │   │   │   ├── CardEditor.cshtml               # إنشاء/تعديل بطاقة
│   │   │   │   ├── CardEditor.cshtml.cs
│   │   │   │   ├── QueryTester.cshtml              # اختبار الاستعلام
│   │   │   │   ├── QueryTester.cshtml.cs
│   │   │   │   ├── DrillDownConfig.cshtml          # تكوين مستويات Drill Down
│   │   │   │   └── DrillDownConfig.cshtml.cs
│   │   │   │
│   │   │   ├── SyncLogs/
│   │   │   │   ├── Index.cshtml                    # عرض سجلات المزامنة
│   │   │   │   └── Index.cshtml.cs
│   │   │   │
│   │   │   └── Shared/
│   │   │       ├── _SyncStatusPartial.cshtml        # شريط حالة المزامنة
│   │   │       └── _BreadcrumbPartial.cshtml        # شريط التنقل في Drill Down
│   │   │
│   │   ├── Data/
│   │   │   └── WarehouseDashboardDbContext.cs       # EF Core DbContext — Config + Logs
│   │   │
│   │   ├── Models/
│   │   │   ├── Entities/
│   │   │   │   ├── DashboardCard.cs                # كيان بطاقة Dashboard
│   │   │   │   ├── CardDrillDownLevel.cs           # كيان مستويات Drill Down
│   │   │   │   ├── SyncSetting.cs                  # كيان إعدادات المزامنة
│   │   │   │   └── SyncLog.cs                      # كيان سجل المزامنة
│   │   │   ├── ViewModels/
│   │   │   │   ├── DashboardViewModel.cs           # نموذج عرض الصفحة الرئيسية
│   │   │   │   ├── CardViewModel.cs                # نموذج عرض البطاقة
│   │   │   │   └── AdminLoginViewModel.cs          # نموذج دخول Admin
│   │   │
│   │   ├── Services/
│   │   │   ├── DashboardService.cs                 # منطق قراءة التكوين + البيانات
│   │   │   ├── CardConfigService.cs                # CRUD للبطاقات (Admin)
│   │   │   └── AdminAuthService.cs                 # التحقق من كلمة مرور Admin
│   │   │
│   │   ├── Middleware/
│   │   │   └── AdminAuthMiddleware.cs              # Middleware لحماية /Admin/*
│   │   │
│   │   ├── wwwroot/
│   │   │   ├── css/
│   │   │   │   ├── site.css                        # الأنماط الأساسية
│   │   │   │   └── blue-theme.css                  # الهوية البصرية الزرقاء (11 لوناً)
│   │   │   ├── js/
│   │   │   │   └── dashboard.js                    # تفاعلات Dashboard (اختياري)
│   │   │   └── lib/                               # مكتبات خارجية (إن وجدت)
│   │   │
│   │   └── Properties/
│   │       └── launchSettings.json
│   │
│   └── WarehouseDashboard.Shared/                   # (اختياري — للموديلات المشتركة)
│       └── (يُضاف لاحقاً إذا تطلب الأمر)
│
├── tests/
│   ├── WarehouseDashboard.Api.Tests/
│   └── WarehouseDashboard.Web.Tests/
│
└── docs/                                            # وثائق إضافية
    └── deployment-guide.md
```

### 3.2 هيكل قاعدة البيانات — SQL Server

```
SQL Server Database: WarehouseDashboard
│
├── Config Tables (EF Core — Migrations)
│   ├── DashboardCards
│   │   ├── Id (int, PK, Identity)
│   │   ├── Title (nvarchar(200))
│   │   ├── ChartType (nvarchar(50))        -- Bar, Line, Pie, KPI, Table, Gauge
│   │   ├── SqlQuery (nvarchar(max))
│   │   ├── DataSourceType (nvarchar(50))   -- SQL Query / View
│   │   ├── GridPositionX (int)
│   │   ├── GridPositionY (int)
│   │   ├── GridWidth (int)
│   │   ├── GridHeight (int)
│   │   ├── RefreshInterval (int)           -- ثوانٍ (0 = لا تحديث تلقائي)
│   │   ├── IsActive (bit)
│   │   ├── CreatedAt (datetime2)
│   │   └── UpdatedAt (datetime2)
│   │
│   ├── CardDrillDownLevels
│   │   ├── Id (int, PK, Identity)
│   │   ├── ParentCardId (int, FK → DashboardCards.Id)
│   │   ├── Level (int)                     -- ترتيب المستوى
│   │   ├── DisplayName (nvarchar(200))
│   │   ├── DrillDownQuery (nvarchar(max))  -- SQL query لهذا المستوى
│   │   └── TargetChartType (nvarchar(50))  -- نوع الرسم البياني في هذا المستوى
│   │
│   ├── SyncSettings
│   │   ├── Id (int, PK, Identity)
│   │   ├── IntervalMinutes (int)           -- default 30
│   │   ├── IsAutoSyncEnabled (bit)
│   │   └── LastSyncTimestamp (datetime2, nullable)
│   │
│   └── AdminPassword
│       ├── Id (int, PK, Identity)
│       ├── PasswordHash (nvarchar(500))    -- BCrypt hash
│       └── UpdatedAt (datetime2)
│
├── Sync Logs Tables (EF Core — Migrations)
│   ├── SyncLogs
│   │   ├── Id (bigint, PK, Identity)
│   │   ├── StartTime (datetime2)
│   │   ├── EndTime (datetime2, nullable)
│   │   ├── Status (nvarchar(20))           -- Running, Success, Failed
│   │   ├── RecordCount (int)
│   │   ├── Duration (bigint, nullable)     -- milliseconds
│   │   ├── TriggerType (nvarchar(20))      -- Auto, Manual
│   │   └── ErrorMessage (nvarchar(max), nullable)
│   │
│   └── ErrorLogs
│       ├── Id (bigint, PK, Identity)
│       ├── SyncLogId (bigint, FK → SyncLogs.Id)
│       ├── TableName (nvarchar(200))
│       ├── ErrorMessage (nvarchar(max))
│       └── Timestamp (datetime2)
│
└── Data Tables (ADO.NET managed — يتم إنشاؤها يدوياً حسب جداول Oracle)
    └── [يُحدد أثناء التنفيذ حسب جداول Oracle]
```

---

## 4. Data Flow Architecture

### 4.1 تدفق البيانات الأساسي

```
┌──────────────────────────────────────────────────────────────────────────┐
│                             DATA FLOW                                     │
│                                                                          │
│  ┌──────────────┐          ┌──────────────────┐      ┌──────────────┐   │
│  │    Oracle    │   ODP.NET│  WarehouseDashboard│ADO.NET│ SQL Server  │   │
│  │   Database   │─────────▶│   .Api            │───────▶│ Destination  │   │
│  │   (Source)   │ Extract  │   (Sync Engine)   │ Load   │             │   │
│  │              │          │                   │        │ ┌─────────┐ │   │
│  │  • Tables    │          │  1. Read Oracle   │        │ │Config   │ │   │
│  │  • Views     │          │  2. Map Data Types│        │ │Tables   │ │   │
│  │  • Queries   │          │  3. DELETE target │        │ ├─────────┤ │   │
│  │              │          │  4. SqlBulkCopy   │        │ │Sync Logs│ │   │
│  └──────────────┘          │  5. Log Result    │        │ ├─────────┤ │   │
│                            └──────────────────┘        │ │Data     │ │   │
│                                                         │ │Tables   │ │   │
│                                                         │ └─────────┘ │   │
│                                                         └──────────────┘   │
│                                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐│
│  │                WarehouseDashboard.Web                                ││
│  │  ┌─────────────────────────────────────────────────────────────────┐ ││
│  │  │  Razor Pages قراءة Config ← تنفيذ SQL ← ربط بيانات ← عرض بطاقة │ ││
│  │  └─────────────────────────────────────────────────────────────────┘ ││
│  └──────────────────────────────────────────────────────────────────────┘│
└──────────────────────────────────────────────────────────────────────────┘
```

### 4.2 مراحل تدفق البيانات بالتفصيل

| المرحلة | الوصف | التقنية | المكان |
|---------|-------|---------|--------|
| **1. الاستخراج (Extract)** | قراءة البيانات من Oracle باستخدام SQL Queries أو Views | `OracleCommand` + `OracleDataReader` عبر `Oracle.ManagedDataAccess` | WarehouseDashboard.Api |
| **2. التحويل (Transform)** | تعيين أنواع البيانات (Oracle Types → .NET Types) | Manual mapping في `OracleExtractionService` | WarehouseDashboard.Api |
| **3. الحذف (Delete)** | حذف جميع الصفوف الموجودة في الجدول الهدف في SQL Server | `SqlCommand` مع `DELETE FROM [TableName]` داخل Transaction | WarehouseDashboard.Api |
| **4. التحميل (Load)** | إدخال البيانات بشكل مجمّع في SQL Server | `SqlBulkCopy.WriteToServer(DataTable)` داخل نفس Transaction | WarehouseDashboard.Api |
| **5. التسجيل (Log)** | تسجيل نتيجة المزامنة (الوقت، العدد، الحالة، المدة) | EF Core — `WarehouseDashboardLogContext` | WarehouseDashboard.Api |
| **6. العرض (Display)** | Dashboard يقرأ Config + Data Tables من SQL Server ويعرضها | Razor Pages + Syncfusion Components | WarehouseDashboard.Web |

### 4.3 قواعد تدفق البيانات

| القاعدة | الشرح |
|---------|-------|
| **قراءة فقط من Oracle** | Api لا يكتب أبداً في Oracle — فقط يقرأ |
| **معاملة واحدة (Transaction)** | DELETE + BulkInsert في Transaction واحدة — ضمان Atomicity |
| **عدم التحميل المزدوج** | لا يتم تحميل البيانات أثناء وجود Sync قيد التشغيل |
| **فصل Config عن Data** | Config تدار عبر EF Core Migrations — Data Tables تُنشأ يدوياً |
| **نفس SQL Server** | Config و Logs و Data Tables في نفس قاعدة البيانات (WarehouseDashboard) |

---

## 5. Sync Engine Design

### 5.1 نظرة عامة

محرك المزامنة هو **Background Service** يعمل داخل `WarehouseDashboard.Api` بشكل مستقل ومتواصل. يستخدم نمط `BackgroundService` مع `PeriodicTimer` من .NET 8.

```
┌─────────────────────────────────────────────────────────────────┐
│                      SyncEngineService                           │
│                  (BackgroundService)                             │
│                                                                  │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐       │
│  │   Timer      │───▶│  ExecuteAsync │───▶│  ProcessAll  │       │
│  │  (Periodic)  │    │  (Main Loop) │    │  Tables()    │       │
│  └──────────────┘    └──────────────┘    └──────┬───────┘       │
│                                                  │               │
│                                                  ▼               │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐       │
│  │  Log Result  │◀───│  SqlBulkCopy │◀───│  Extract     │       │
│  │  (EF Core)   │    │  Load        │    │  From Oracle │       │
│  └──────────────┘    └──────────────┘    └──────────────┘       │
│                                                                  │
│  ┌──────────────────────────────────────────────────────┐        │
│  │  Manual Override: POST /api/sync/trigger             │        │
│  │  Status Check:    GET  /api/sync/status              │        │
│  └──────────────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────────────┘
```

### 5.2 الخدمات الداخلية

| الخدمة | الوظيفة | التفاصيل التقنية |
|--------|---------|------------------|
| `OracleExtractionService` | قراءة البيانات من Oracle | `OracleConnection` → `OracleCommand` → `OracleDataReader` → `DataTable` |
| `SqlServerLoadService` | تحميل البيانات إلى SQL Server | `SqlConnection` → `SqlBulkCopy` → `WriteToServer(DataTable)` مع Transaction |
| `SyncEngineService` | تنسيق عملية المزامنة الكاملة | `BackgroundService.ExecuteAsync(CancellationToken)` مع `PeriodicTimer` |
| `SyncLogService` | تسجيل عمليات المزامنة | EF Core — `WarehouseDashboardLogContext` |

### 5.3 وضع Full Refresh (Phase 1)

```
Full Refresh Workflow:
┌──────────────────────────────────────────────────────────────┐
│  1. Start sync                                               │
│  2. Log: Status = "Running", StartTime = now                 │
│  3. For each table in mapping:                               │
│      a. Open Oracle connection → Execute SELECT query        │
│      b. Read into DataTable (OracleDataReader → DataTable)   │
│      c. Close Oracle connection                              │
│      d. Open SQL Server connection → BEGIN TRANSACTION       │
│      e. DELETE FROM [TargetTable]                            │
│      f. SqlBulkCopy.WriteToServer(DataTable)                 │
│      g. COMMIT TRANSACTION                                   │
│      h. Append to ErrorLog (if failed, ROLLBACK per table)   │
│  4. Log: Status = "Success"/"Failed", EndTime = now          │
│  5. Update LastSyncTimestamp                                 │
│  6. Wait for next interval (default 30 min)                  │
└──────────────────────────────────────────────────────────────┘
```

### 5.4 وضع Incremental Ready (معماري — غير مفعّل في Phase 1)

المحرك **مصمم لدعم Incremental Sync** من البداية كقرار استراتيجي لتجنب إعادة الكتابة لاحقاً:

```
Incremental Architecture (جاهز — غير مفعّل):
┌──────────────────────────────────────────────────────────────┐
│  1. لكل جدول، يتم تتبع LastSyncTimestamp                     │
│  2. الاستعلام من Oracle: SELECT * FROM [Table]               │
│     WHERE [LastModifiedColumn] > @LastSyncTimestamp          │
│  3. في SQL Server:                                           │
│      a. UPSERT: MERGE INTO [Target] USING [Source]           │
│         ON [Key] MATCH → UPDATE / NOT MATCH → INSERT         │
│      b. أو: DELETE changed rows → INSERT updated rows        │
│  4. تحديث LastSyncTimestamp بعد النجاح                       │
│                                                              │
│  ⚠️ يتطلب:                                                    │
│  - وجود LastModifiedColumn في جداول Oracle                    │
│  - Primary Key لكل جدول للمقارنة                             │
│  - اختبار أداء MERGE مقابل DELETE+INSERT                      │
└──────────────────────────────────────────────────────────────┘
```

**شروط التفعيل المستقبلي:**
1. حجم البيانات يتجاوز 100,000 صف
2. أداء Full Refresh يصبح غير مقبول
3. العميل يحتاج تكرار مزامنة أعلى من مرة كل 30 دقيقة

### 5.5 الجدولة والتوقيت

| الخاصية | القيمة |
|---------|--------|
| **آلية الجدولة** | `PeriodicTimer` (متوفر في .NET 8) |
| **الفترة الافتراضية** | 30 دقيقة |
| **قابلة للتكوين** | نعم — عبر `appsettings.json` (قسم `SyncSettings.IntervalMinutes`) |
| **منع التشغيل المتداخل** | ` SemaphoreSlim` — إذا كانت المزامنة قيد التشغيل، يتم تخطي الدورة |
| **الإيقاف الآمن** | `CancellationToken` — يتوقف خلال 5 ثوانٍ من إيقاف الخدمة |

### 5.6 API Endpoints

| Method | Endpoint | الوظيفة | Response |
|--------|----------|---------|----------|
| `POST` | `/api/sync/trigger` | تفعيل مزامنة يدوية | `{ "status": "triggered", "message": "..." }` |
| `GET` | `/api/sync/status` | حالة المزامنة الحالية | `{ "isRunning": bool, "lastSyncTime": datetime, "lastStatus": string, "lastRecordCount": int }` |
| `GET` | `/api/sync/logs` | سجلات المزامنة (آخر 100) | `[{ "startTime": ..., "endTime": ..., "status": ..., "recordCount": ..., "duration": ... }]` |
| `GET` | `/api/sync/config` | إعدادات المزامنة الحالية | `{ "intervalMinutes": int, "isAutoSyncEnabled": bool, "lastSyncTimestamp": datetime }` |

> **ملاحظة:** الـ Api يعمل على Port مختلف (مثلاً 5001) ويتم ربطه في IIS كـ Application منفصل تحت نفس الموقع أو موقع منفصل.

---

## 6. Dashboard Architecture (Razor Pages)

### 6.1 مبدأ العرض الديناميكي

Dashboard لا يحتوي على بطاقات ثابتة — كل شيء يُقرأ من قاعدة البيانات وقت التشغيل:

```
Page Load
    │
    ▼
DashboardService.GetCards()
    │
    ├── 1. Read DashboardCards from SQL Server (EF Core)
    │       └── WHERE IsActive = true
    │
    ├── 2. For each active card:
    │       ├── Execute SqlQuery against SQL Server (ADO.NET)
    │       └── Store result in CardViewModel (DataTable + ChartType + Position)
    │
    └── 3. Pass List<CardViewModel> to Razor Page
            │
            ▼
        Razor View renders each card using Syncfusion components:
            ├── Bar → Syncfusion Bar Chart
            ├── Line → Syncfusion Line Chart
            ├── Pie → Syncfusion Pie Chart
            ├── KPI → Custom KPI Card (HTML + CSS + Syncfusion Sparkline)
            ├── Table → HTML Table
            └── Gauge → Syncfusion Circular Gauge
```

### 6.2 هيكل الصفحات

| الصفحة | المسار | الوظيفة |
|--------|--------|---------|
| Dashboard الرئيسية | `/` أو `/Dashboard/Index` | عرض جميع البطاقات النشطة |
| Drill Down | `/Dashboard/DrillDown?cardId=X&level=Y&params=Z` | عرض تفاصيل مستوى Drill Down |
| Sync Logs | `/SyncLogs/Index` | عرض سجلات المزامنة (لـ Admin) |

### 6.3 مكونات Dashboard

| المكون | الوصف | المرجع في Syncfusion |
|--------|-------|---------------------|
| **Card Container** | حاوية البطاقة مع العنوان والحجم والموقع | CSS Grid أو自定义 |
| **Bar Chart** | رسم بياني أعمدة | `Syncfusion` Chart Component (Column/Bar) |
| **Line Chart** | رسم بياني خطي | `Syncfusion` Chart Component (Line) |
| **Pie Chart** | رسم بياني دائري | `Syncfusion` Chart Component (Pie) |
| **KPI Card** | رقم كبير مع مؤشر (سهم صعود/هبوط) | Custom HTML + `Syncfusion` Sparkline |
| **Data Table** | جدول بيانات تفصيلي | HTML Table مع `Syncfusion` Grid (اختياري) |
| **Gauge** | مؤشر أداء | `Syncfusion` Circular Gauge |
| **Breadcrumb** | شريط التنقل لـ Drill Down | Custom Partial View |
| **Sync Status Bar** | شريط حالة المزامنة | Custom Partial View (top of page) |
| **Sync Button** | زر التحديث اليدوي | HTML Button → POST /api/sync/trigger |

### 6.4 مبدأ Drill Down

```
Level 0 (Dashboard Main)
    │
    ├── Card A (Bar Chart: "المستودعات حسب المنطقة")
    │       │
    │       ▼
    │   Level 1 (عند النقر على عمود "الرياض")
    │       │
    │       ├── Card A-1 (Pie Chart: "تفاصيل مستودعات الرياض")
    │       └── Card A-2 (Table: "قائمة المنتجات في مستودعات الرياض")
    │              │
    │              ▼
    │          Level 2 (عند النقر على منتج معين)
    │              │
    │              └── Card A-2-1 (Line Chart: "حركة المنتج شهرياً")
    │
    └── Card B (KPI: "إجمالي المخزون")
            │
            ▼
        Level 1 (KPI التفصيلي)
```

**آلية العمل:**
1. كل بطاقة رئيسية يمكن أن تحتوي على `DrillDownLevels` متعددة
2. عند النقر على عنصر في البطاقة، يتم إرسال `cardId` + `level` + `parameter` إلى صفحة Drill Down
3. صفحة Drill Down تقرأ `DrillDownQuery` من `CardDrillDownLevels` وتنفذه مع الـ parameter
4. Breadcrumb يُحدّث ليعكس المسار الحالي
5. يمكن العودة إلى أي مستوى سابق عبر Breadcrumb

### 6.5 State Management

| الحالة | السلوك |
|--------|--------|
| **Loading** | عرض `Syncfusion` Skeleton Loader أو Spinner أثناء تحميل البطاقات |
| **Empty** | عرض رسالة "لا توجد بيانات" للبطاقة مع إبقاء الإطار |
| **Error** | عرض رسالة الخطأ في البطاقة مع إمكانية إعادة المحاولة |
| **Success** | عرض البطاقة بشكل طبيعي مع البيانات |
| **Sync Running** | شريط الحالة يظهر "جاري المزامنة..." + تعطيل زر المزامنة |

---

## 7. Admin Panel Architecture

### 7.1 مبدأ الحماية

Admin Panel محمي بـ:
1. **Hidden URL** — المسار ليس `/Admin` بل `/admin-secure-panel` (قابل للتغيير)
2. **Password Protection** — صفحة دخول بكلمة مرور → تخزين `Session` أو `Cookie`

### 7.2 تدفق الدخول

```
User visits /admin-secure-panel
    │
    ▼
AdminAuthMiddleware checks for valid session
    │
    ├── No session → Redirect to /admin-secure-panel/Login
    │                        │
    │                        ▼
    │                   User enters password
    │                        │
    │                        ▼
    │                   AdminAuthService.VerifyPassword(password)
    │                        │
    │                        ├── Correct → Set session/cookie → Redirect to Card List
    │                        └── Wrong → Show error, retry
    │
    └── Valid session → Allow access to Admin pages
```

### 7.3 صفحات Admin Panel

| الصفحة | المسار | الوظيفة |
|--------|--------|---------|
| Admin Login | `/admin-secure-panel/Login` | إدخال كلمة المرور |
| Card List | `/admin-secure-panel/DashboardCards` | عرض كل البطاقات مع زر تعديل/حذف |
| Card Editor | `/admin-secure-panel/CardEditor` | إنشاء بطاقة جديدة |
| Card Editor | `/admin-secure-panel/CardEditor/{id}` | تعديل بطاقة موجودة |
| Query Tester | `/admin-secure-panel/QueryTester` | كتابة واختبار SQL query |
| Drill Down Config | `/admin-secure-panel/DrillDownConfig/{cardId}` | تكوين مستويات Drill Down للبطاقة |

### 7.4 Card Editor Form

| الحقل | النوع | الوصف |
|-------|-------|-------|
| Title | Text | عنوان البطاقة |
| ChartType | Select (Bar, Line, Pie, KPI, Table, Gauge) | نوع العرض |
| DataSourceType | Select (SQL Query / View) | نوع مصدر البيانات |
| SqlQuery | TextArea (Multi-line) | استعلام SQL أو اسم View |
| GridPositionX | Number | موقع البطاقة في الشبكة (X) |
| GridPositionY | Number | موقع البطاقة في الشبكة (Y) |
| GridWidth | Number | عرض البطاقة (1-12) |
| GridHeight | Number | ارتفاع البطاقة (1-6) |
| RefreshInterval | Number | فترة التحديث التلقائي (ثوانٍ) |
| IsActive | Checkbox | تفعيل/تعطيل البطاقة |

### 7.5 Query Tester

```
┌─────────────────────────────────────────────────────┐
│  Query Tester                                       │
├─────────────────────────────────────────────────────┤
│                                                     │
│  SQL Query:                                         │
│  ┌─────────────────────────────────────────────┐    │
│  │  SELECT Region, SUM(Quantity) AS Total      │    │
│  │  FROM vw_warehouse_stock                    │    │
│  │  GROUP BY Region                            │    │
│  └─────────────────────────────────────────────┘    │
│                                                     │
│  [🔍 Test Query]                                    │
│                                                     │
│  Result: ✅ 5 rows returned (152ms)                 │
│  ┌──────────┬───────────┐                           │
│  │ Region   │ Total     │                           │
│  ├──────────┼───────────┤                           │
│  │ الرياض   │ 15,234    │                           │
│  │ جدة      │ 12,876    │                           │
│  │ الدمام   │ 8,432     │                           │
│  │ ...      │ ...       │                           │
│  └──────────┴───────────┘                           │
│                                                     │
│  [💾 Save Card]                                     │
└─────────────────────────────────────────────────────┘
```

---

## 8. Security Approach

### 8.1 Phase 1 (الحالية)

| الإجراء | الوصف |
|---------|-------|
| **Admin Panel URL مخفي** | المسار الفعلي غير `/Admin` — قابل للتغيير عبر `appsettings.json` |
| **حماية بكلمة مرور** | صفحة دخول تتحقق من كلمة المرور (مشفرة بـ BCrypt) |
| **Session-based Auth** | استخدام `ISession` أو `Cookie` للحفاظ على حالة الدخول |
| **Middleware** | `AdminAuthMiddleware` يفحص session قبل كل صفحة Admin |
| **لا Auth للـ Dashboard** | Dashboard متاح لجميع المستخدمين على الشبكة المحلية |
| **Api ليس له Auth** | Api داخلي — يمكن تقييد الوصول عبر IIS (IP and Domain Restrictions) |
| **Oracle اتصال قراءة فقط** | `read-only` permissions في Oracle |

### 8.2 Phase 2 (مستقبلية)

| الإجراء | الوصف |
|---------|-------|
| **RBAC** | إضافة Roles (Admin, Viewer) مع صلاحيات مختلفة |
| **Dashboard Auth** | إمكانية حماية Dashboard بصلاحية Viewer |
| **Api Auth** | إضافة JWT أو API Key للـ Api endpoints |

### 8.3 تخزين كلمة المرور

```
AdminPassword Table:
┌──────────┬──────────────────────────────────┬──────────────────────┐
│    Id    │          PasswordHash            │      UpdatedAt       │
├──────────┼──────────────────────────────────┼──────────────────────┤
│    1     │  $2a$11$...BCrypt Hash...       │  2026-07-12 10:00:00 │
└──────────┴──────────────────────────────────┴──────────────────────┘

- التقنية: BCrypt.Net (Salt = 11 rounds)
- لا يُحفظ Plain Text أبداً
- التغيير: عبر سكريبت أو صفحة إعدادات Admin
```

### 8.4 Connection Strings Protection

```
في IIS:
├── appsettings.json: لا يحتوي كلمات مرور حقيقية
├── Environment Variables: (مستحسن) OraclePass, SqlServerPass
└── IIS Configuration: Encryption عبر aspnet_regiis -pe
```

---

## 9. IIS Deployment

### 9.1 هيكل النشر

```
IIS Server (Windows Server 2019+)
│
├── Sites
│   └── WarehouseDashboard (Default Web Site أو موقع منفصل)
│       │
│       ├── /api                        → Application: WarehouseDashboard.Api
│       │   └── (Pool: WDApiPool, No Managed Code)
│       │
│       └── /                           → Application: WarehouseDashboard.Web
│           └── (Pool: WDWebPool, No Managed Code)
│
├── Application Pools
│   ├── WDApiPool (.NET CLR: No Managed Code, Identity: ApplicationPoolIdentity)
│   └── WDWebPool (.NET CLR: No Managed Code, Identity: ApplicationPoolIdentity)
│
└── File System
    └── C:\WarehouseDashboard\
        ├── api\             ← dotnet publish output for Api
        └── web\             ← dotnet publish output for Web
```

### 9.2 خطوات النشر

| الخطوة | الأمر/الإجراء |
|--------|--------------|
| 1. Build + Publish Api | `dotnet publish src/WarehouseDashboard.Api -c Release -o C:\WarehouseDashboard\api` |
| 2. Build + Publish Web | `dotnet publish src/WarehouseDashboard.Web -c Release -o C:\WarehouseDashboard\web` |
| 3. إنشاء Application Pools | عبر IIS Manager — CLR: No Managed Code |
| 4. إنشاء مواقع IIS | ربط api بالـ Api pool، web بالـ Web pool |
| 5. تعيين Connection Strings | عبر Environment Variables أو `appsettings.json` المشفر |
| 6. تثبيت Hosting Bundle | تثبيت `dotnet-hosting-8.x.x-win.exe` على السيرفر |
| 7. اختبار | `http://localhost/api/sync/status` + `http://localhost/` |

### 9.3 متطلبات السيرفر

| المتطلب | التفاصيل |
|---------|----------|
| **نظام التشغيل** | Windows Server 2019+ أو Windows 10/11 Pro |
| **IIS** | IIS 10+ مع Activation: ASP.NET Core Hosting |
| **.NET Runtime** | .NET 8 Runtime + ASP.NET Core 8.x Hosting Bundle |
| **SQL Server** | SQL Server 2019+ (أو Express) |
| **Oracle** | Oracle Instant Client (ليس ضرورياً — Managed Driver لا يحتاجه) |
| **الذاكرة (RAM)** | 4GB كحد أدنى (8GB موصى به) |
| **المساحة التخزينية** | 50GB + مساحة بيانات Oracle/SQL Server |

---

## 10. Key Technical Decisions

### 10.1 جدول القرارات النهائية

| # | القرار | البديل المتاح | الاختيار | المبرر | المصدر |
|:-:|--------|--------------|---------|--------|--------|
| 1 | **Framework** | .NET 6, .NET 8, .NET 9 Preview | **.NET 8 LTS** | أحدث LTS — دعم حتى 2026 | Decision #1 |
| 2 | **نمط الواجهة** | Blazor Server, Blazor WASM, MVC | **Razor Pages** | أبسط بدون SPA complexity | Decision #2 |
| 3 | **مكتبة Dashboard** | Chart.js, D3.js, DevExpress | **Syncfusion** | Community License متوفرة، 1600+ مكون، Drill Down مدمج | Decision #3 |
| 4 | **Oracle Connector** | Oracle.EntityFrameworkCore | **ODP.NET Managed** | لا يحتاج Oracle Client، أداء أفضل | Decision #4 |
| 5 | **طريقة تحميل البيانات** | EF Core BulkInsert, Dapper | **ADO.NET SqlBulkCopy** | أسرع بـ 3-5x من EF Core — قرار استراتيجي | Decision #21 |
| 6 | **ORM للـ Config + Logs** | ADO.NET, Dapper | **EF Core** | CRUD أسهل، Migrations، استعلامات بسيطة | Decision #6 |
| 7 | **وضع المزامنة (Phase 1)** | Incremental أولاً | **Full Refresh** | أبسط للتنفيذ والتحقق | Decision #5 |
| 8 | **دعم Incremental (معماري)** | لا ندعمه حالياً | **دعم معماري جاهز** | منع إعادة الكتابة — قرار استراتيجي | Decision #20 |
| 9 | **جدولة المزامنة** | Hangfire, Quartz.NET | **PeriodicTimer (BackgroundService)** | مدمج، لا اعتماد خارجي، كافٍ للحاجة | Blueprint §9 |
| 10 | **حماية Admin Panel** | Windows Auth, JWT, Identity | **Password (BCrypt) + Hidden URL** | أبسط حل — Phase 2 يضيف RBAC | Decision #4 |
| 11 | **تشفير كلمة المرور** | MD5, SHA256, Plain Text | **BCrypt.Net** | أفضل الممارسات، Salt مدمج | Blueprint §9 |
| 12 | **Hosting** | Kestrel فقط, Docker | **IIS** | محلي — IIS هو المطلوب | Decision #12 |
| 13 | **اختبار Oracle** | لاحقاً | **في أول 3 أيام** | تقليل مخاطر التواصل المتأخر — قرار استراتيجي | Decision #22 |

### 10.2 تحليل القرارات الاستراتيجية

#### القرار: ADO.NET SqlBulkCopy بدلاً من EF Core للبيانات

```
لماذا؟
┌────────────────────────────────────────────────────────────────────┐
│ EF Core BulkInsert: 30,000 صف → ~15-20 ثانية                     │
│ ADO.NET SqlBulkCopy: 30,000 صف → ~3-5 ثوانٍ                      │
│                                                                   │
│ الفرق: 3x إلى 5x أسرع                                             │
│ التأثير: مع 100,000+ صف، الفرق يصبح دقائق بدلاً من ثوانٍ         │
└────────────────────────────────────────────────────────────────────┘

التقسيم:
├── ADO.NET SqlBulkCopy → Data Loading (بيانات Oracle → SQL Server)
└── EF Core → Config + Logs (CRUD بسيط + Migrations)
```

#### القرار: Razor Pages بدلاً من Blazor

```
لماذا؟
┌────────────────────────────────────────────────────────────────────┐
│ Razor Pages:                                                      │
│   ✅ Server-rendered — لا JavaScript ثقيل                         │
│   ✅ أبسط deploy — لا WASM files                                  │
│   ✅ Connection-less — لا SignalR مطلوب                           │
│   ✅ مناسب لـ Dashboard داخلي                                     │
│                                                                   │
│ Blazor Server:                                                    │
│   ❌ يتطلب SignalR — قد يكون مشكلة في الشبكات الداخلية             │
│   ❌ استهلاك ذاكرة أعلى لكل اتصال                                 │
│   ❌ تعقيد أعلى للنشر والإدارة                                     │
│                                                                   │
│ Blazor WASM:                                                      │
│   ❌ تحميل أولي كبير                                               │
│   ❌ تعقيد إضافي في التفاعل مع Syncfusion                          │
└────────────────────────────────────────────────────────────────────┘
```

#### القرار: Full Refresh أولاً مع دعم Incremental معماري

```
┌────────────────────────────────────────────────────────────────────┐
│ Full Refresh (Phase 1):                                           │
│   ✅ أسهل في التنفيذ والتحقق                                      │
│   ✅ لا يحتاج LastModifiedColumn في Oracle                         │
│   ✅ لا يحتاج Primary Key mapping                                 │
│   ✅ يضمن تطابق تام مع Oracle في كل مزامنة                        │
│                                                                   │
│ Incremental Ready (معماري):                                       │
│   ✅ لا يحتاج إعادة كتابة المحرك لاحقاً                           │
│   ✅ الفرق: إضافة WHERE clause + MERGE بدلاً من DELETE+INSERT     │
│   ✅ جاهز للتفعيل عندما: حجم البيانات > 100K صف أو أداء غير مقبول │
└────────────────────────────────────────────────────────────────────┘
```

---

## 11. Performance Considerations

| الجانب | الاعتبار | الإجراء |
|--------|---------|---------|
| **حجم بيانات Oracle** | غير معروف مسبقاً | Full Refresh معقول لغاية 100K صف. Incremental جاهز للتفعيل |
| **SqlBulkCopy Batch Size** | الافتراضي قد لا يكون مثالياً | تعيين `BatchSize = 5000` وتجربة تحسين الأداء |
| **Transaction حجم** | معاملة واحدة قد تسبب ضغطاً | لكل جدول معاملة مستقلة — فشل جدول لا يؤثر على الآخرين |
| **Dashboard Query أداء** | استعلامات بطيئة تؤثر على تحميل الصفحة | إضافة timeout للاستعلام + عرض Loading State |
| **Sync أثناء عرض Dashboard** | لا يؤثر — كل منهما يقرأ من SQL Server بشكل مستقل | لا مانع من التشغيل المتزامن (قراءة متزامنة مع كتابة) |
| **صفحة Dashboard تحميل** | بطاقات متعددة تعني استعلامات متعددة | تنفيذ الاستعلامات بشكل متوازٍ `Task.WhenAll` |

---

## 12. Monitoring and Logging

### 12.1 أنواع التسجيل

| النوع | المكان | المحتوى |
|-------|--------|---------|
| **Sync Logs** | SQL Server — `SyncLogs` Table | وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة |
| **Error Logs** | SQL Server — `ErrorLogs` Table | اسم الجدول، رسالة الخطأ، الوقت |
| **Console Logs** | `Console.WriteLine` أو `ILogger` | أثناء التطوير — يمكن إيقافها في Production |
| **IIS Logs** | IIS Log Files | طلبات HTTP — للمساعدة في debugging |

### 12.2 Sync Status Indicator

Dashboard يحتوي على شريط يعرض:
- آخر وقت مزامنة (LastSyncTime)
- حالة آخر مزامنة (Success / Failed)
- مؤشر "قيد التشغيل" (Running)
- عدد السجلات المنقولة في آخر مزامنة
- زر "تحديث الآن" (Manual Trigger)

---

## 13. Constraints and Assumptions

### 13.1 Constraints (قيود)

| # | القيد | التأثير |
|:-:|-------|---------|
| C1 | Oracle و SQL Server على نفس السيرفر المحلي | زمن انتقال منخفض — لا مشاكل Network latency |
| C2 | تفاصيل جداول Oracle مؤجلة | لا يمكن تحديد Data Tables Schema مسبقاً |
| C3 | لا Authentication لـ Dashboard في Phase 1 | أي مستخدم على الشبكة يمكنه الوصول للـ Dashboard |
| C4 | Admin Panel محمي بكلمة مرور فقط | أمان محدود — Phase 2 يضيف RBAC |
| C5 | Syncfusion Community License | بعض المكونات المتقدمة قد لا تكون متاحة |

### 13.2 Assumptions (افتراضات)

| # | الافتراض | المبرر |
|:-:|---------|--------|
| A1 | فترة المزامنة الافتراضية = 30 دقيقة | قيمة معقولة — قابلة للتعديل |
| A2 | كلمة مرور Admin تُشفّر بـ BCrypt | أفضل الممارسات |
| A3 | Oracle لا يحتاج Instant Client | Managed Driver يعمل بدون Oracle Client |
| A4 | SQL Server Express كافٍ | 10GB حد قاعدة البيانات — ما لم تكن البيانات ضخمة |
| A5 | عدد البطاقات ~20 | تقدير معقول — النظام ديناميكي ويدعم أي عدد |

---

## 14. Deployment Configuration Files

### 14.1 `appsettings.json` — WarehouseDashboard.Api

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=...;Password=...;Data Source=...",
    "SqlServerConnection": "Server=localhost;Database=WarehouseDashboard;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "SyncSettings": {
    "IntervalMinutes": 30,
    "IsAutoSyncEnabled": true,
    "TransactionPerTable": true,
    "BulkCopyBatchSize": 5000,
    "BulkCopyTimeout": 300
  },
  "Urls": "http://localhost:5001"
}
```

### 14.2 `appsettings.json` — WarehouseDashboard.Web

```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost;Database=WarehouseDashboard;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AdminPanel": {
    "HiddenPath": "admin-secure-panel",
    "SessionTimeoutMinutes": 60
  },
  "SyncDisplay": {
    "ApiBaseUrl": "http://localhost:5001",
    "AutoRefreshSeconds": 60
  },
  "Urls": "http://localhost:5000",
  "Syncfusion": {
    "LicenseKey": "YOUR_SYNCFUSION_UNLOCK_KEY"
  }
}
```

---

## 15. Testing Strategy

| المستوى | التركيز | الأدوات |
|---------|---------|---------|
| **Unit Tests** | Services (OracleExtraction, SqlServerLoad, DashboardService) | xUnit + Moq |
| **Integration Tests** | Oracle Connection, SqlBulkCopy, EF Core Context | xUnit + Test Containers (اختياري) |
| **Manual Testing** | Dashboard UI, Drill Down, Admin CRUD | المتصفح + IIS |
| **Oracle Testing (3 أيام)** | الاتصال، الاستعلامات، أنواع البيانات | Oracle.ManagedDataAccess |

> **ملاحظة:** الاختبارات الآلية مؤجلة — يمكن إضافتها في Phase 2 حسب اتفاقية `Out of Scope` في `00_PROJECT_INPUTS.md`.

---

## 16. Phase Transition Plan

### 16.1 من Phase 1 إلى Phase 2

```
Phase 1 (Core MVP)                    Phase 2 (توسعة)
─────────────────────────             ─────────────────────────
├── Full Refresh Sync                 ├── Incremental Sync (تفعيل)
├── Dashboard + Drill Down            ├── Dashboard Authentication (RBAC)
├── Admin Panel (Password)            ├── Export to Excel/PDF
├── Sync Logs                         ├── Data Editing Screens
└── Blue Theme (11 لوناً)            └── Advanced Analytics
```

### 16.2 شروط الترقية إلى Phase 2

1. استقرار Phase 1 لمدة أسبوعين على الأقل
2. موافقة العميل على Phase 2
3. معالجة جميع تقارير الأخطاء في Phase 1
4. توفر بيانات كافية لاختبار Incremental Sync

---

## 17. Document References

| الوثيقة | المسار | العلاقة |
|---------|--------|---------|
| Technology Profile | `tera-system/profiles/dotnet-razorpages-adonet.md` | الملف التقني المعتمد |
| Project Inputs | `project-preparation/00_PROJECT_INPUTS.md` | ملخص المدخلات |
| Application Blueprint | `project-preparation/APPLICATION_BLUEPRINT.md` | المخطط المعتمد |
| Client Decisions | `client-engagement/CLIENT_DECISION_LOG.md` | 23 قراراً معتمداً |
| Open Questions | `project-preparation/BLUEPRINT_OPEN_QUESTIONS.md` | الأسئلة المفتوحة والافتراضات |
| Project Decision | `project-preparation/TERA_PROJECT_DECISION.md` | قرار بدء التحضير |

---

## 18. Design Gaps

| # | الفجوة | التأثير | الإجراء المطلوب |
|:-:|--------|---------|-----------------|
| G1 | تفاصيل جداول Oracle غير معروفة (أسماء، هياكل، أحجام) | عالي — يحدد Data Tables Schema في SQL Server | تُحدد أثناء التنفيذ مع العميل |
| G2 | آلية السحب التفصيلية من Oracle (استعلامات، Views، parameters) | متوسط — يحدد تعقيد OracleExtractionService | تُحدد أثناء التنفيذ |
| G3 | عدد المستخدمين الفعلي | منخفض — لا يؤثر على Phase 1 Architecture | يُحدد أثناء التطوير |
| G4 | هل يحتاج Dashboard لـ Authentication في المستقبل؟ | منخفض — لا يؤثر على Phase 1 | يُحدد لاحقاً |

> **ملاحظة:** جميع الفجوات أعلاه مؤجلة بتأكيد Majed ولا تمنع بدء التنفيذ. تم توثيقها في `BLUEPRINT_OPEN_QUESTIONS.md`.

---

## 19. Task Engineering Review Decision

| البند | القيمة |
|-------|--------|
| **Decision** | `APPROVED_FOR_GATE` |
| **التقييم** | ✅ الوثيقة جاهزة — جميع القرارات المعمارية معتمدة وموثقة |
| **الملاحظات** | لا توجد عوائق تمنع بدء Pre-Execution Gate |
| **المراجعون المقترحون** | Majed (اعتماد معماري) + TeraAgent (تسلسل المهام) |

---

> **النهاية:** وثيقة البنية المعمارية النهائية — WarehouseDashboard
> **الحالة:** `Module Baseline Approved` ✅
> **الإصدار:** 1.0.0
> **تاريخ آخر تحديث:** 2026-07-12
> **إعداد:** Software Designer Agent (مُصمم) — بتكليف من TeraAgent (TASK-PREP-002)
