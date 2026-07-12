# 06_DATA_MODEL_PREPARATION.md — WarehouseDashboard

> **النوع:** Data Model Document
> **الحالة:** `Module Baseline Approved`
> **Baseline Module:** WarehouseDashboard
> **تاريخ الاعتماد:** 2026-07-12
> **الجهة المعتمدة:** Software Designer Agent (مُصمم) — بالاستناد إلى `08_TECHNICAL_ARCHITECTURE.md` (الحالة: MBA)
> **الملف المرجعي:** `08_TECHNICAL_ARCHITECTURE.md` (§3.2), `dotnet-razorpages-adonet.md`
> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-007

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Module Baseline Approved` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | N/A — ثابت طوال Phase 1 |
| **Document Type** | Data Model Preparation |
| **Version** | 1.0.0 |

---

## نبذة عن الوثيقة

توثق هذه الوثيقة **نموذج البيانات** (Data Model) النهائي لتطبيق **WarehouseDashboard** — وهو نظام محلي لاستخراج البيانات من Oracle وعرضها في Dashboard ديناميكي مع إمكانية التكوين عبر Admin Panel.

تستند هذه الوثيقة إلى `08_TECHNICAL_ARCHITECTURE.md` (§3.2) الذي يحدد هيكل قاعدة البيانات في SQL Server، وتفصل هنا جميع الجداول والعلاقات والقيود والفهارس (Indexes) بالإضافة إلى الحدود الواضحة بين قسمين متميزين:

1. **EF Core Managed Tables** — Config Tables + Sync Logs Tables (تُدار عبر EF Core Migrations)
2. **ADO.NET Managed Tables** — Data Tables (تُنشأ وتُدار يدوياً)

> **ملاحظة:** هذه الوثيقة تركز على **SQL Server (Destination)** فقط. قاعدة Oracle (Source) هي نظام خارجي يُقرأ فقط ولا يُدار بواسطة هذا التطبيق — سياقها موجود في `08_TECHNICAL_ARCHITECTURE.md` §5-6.

---

## 1. Config Tables (EF Core Managed)

الجداول التالية تُدار بالكامل عبر **Entity Framework Core Migrations**، وتُعرِّفها الكلاسات في `WarehouseDashboard.Web/Models/Entities/`. يتم إنشاء هذه الجداول وتحديثها عبر `dotnet ef migrations add` و `dotnet ef database update`.

### 1.1 جدول DashboardCards

**الغرض:** تخزين تكوين كل بطاقة في Dashboard — النوع، مصدر البيانات، الموقع، الحجم، وخيارات العرض.

**مسار الكيان:** `WarehouseDashboard.Web/Models/Entities/DashboardCard.cs`
**DbContext:** `WarehouseDashboardDbContext` (في `WarehouseDashboard.Web/Data/`)

| العمود | النوع في SQL Server | المفتاح | إلزامي | القيمة الافتراضية | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:-----------------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `Title` | `nvarchar(200)` | — | ✅ | — | عنوان البطاقة (يُعرض في الـ Header) |
| `ChartType` | `nvarchar(50)` | — | ✅ | — | القيم المسموحة: `Bar`, `Line`, `Pie`, `KPI`, `Table`, `Gauge` — يُفرض CHECK constraint |
| `SqlQuery` | `nvarchar(max)` | — | ✅ | — | استعلام SQL أو اسم View يتم تنفيذه لجلب بيانات البطاقة |
| `DataSourceType` | `nvarchar(50)` | — | ✅ | `'SQL Query'` | القيم المسموحة: `'SQL Query'`, `'View'` |
| `GridPositionX` | `int` | — | ✅ | `0` | موقع البطاقة في الشبكة (X) — يبدأ من 0 |
| `GridPositionY` | `int` | — | ✅ | `0` | موقع البطاقة في الشبكة (Y) — يبدأ من 0 |
| `GridWidth` | `int` | — | ✅ | `4` | عرض البطاقة (1-12) — يمثل عدد أعمدة الشبكة |
| `GridHeight` | `int` | — | ✅ | `2` | ارتفاع البطاقة (1-6) — يمثل عدد صفوف الشبكة |
| `RefreshInterval` | `int` | — | ✅ | `0` | فترة التحديث التلقائي بالثواني. `0` = لا تحديث تلقائي |
| `IsActive` | `bit` | — | ✅ | `1` | `1` = معروضة في Dashboard، `0` = مخفية |
| `CreatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت إنشاء البطاقة |
| `UpdatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت آخر تعديل |

**CHECK Constraints:**
- `CK_DashboardCards_ChartType`: `ChartType IN ('Bar', 'Line', 'Pie', 'KPI', 'Table', 'Gauge')`
- `CK_DashboardCards_DataSourceType`: `DataSourceType IN ('SQL Query', 'View')`
- `CK_DashboardCards_GridWidth`: `GridWidth BETWEEN 1 AND 12`
- `CK_DashboardCards_GridHeight`: `GridHeight BETWEEN 1 AND 6`
- `CK_DashboardCards_RefreshInterval`: `RefreshInterval >= 0`

**Indexes:**
- `IX_DashboardCards_IsActive` — `IsActive` (غير متفرد) — لتسريع استعلام `WHERE IsActive = 1`
- `IX_DashboardCards_GridPositionX_GridPositionY` — `(GridPositionX, GridPositionY)` (غير متفرد) — لترتيب البطاقات في الشبكة

---

### 1.2 جدول CardDrillDownLevels

**الغرض:** تخزين مستويات Drill Down لكل بطاقة — تعريف استعلامات التعمق ونوع الرسم البياني في كل مستوى.

**مسار الكيان:** `WarehouseDashboard.Web/Models/Entities/CardDrillDownLevel.cs`
**DbContext:** `WarehouseDashboardDbContext`

| العمود | النوع في SQL Server | المفتاح | إلزامي | القيمة الافتراضية | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:-----------------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `ParentCardId` | `int` | FK → DashboardCards.Id | ✅ | — | **مطلوب.** معرف البطاقة الأم. ON DELETE CASCADE |
| `Level` | `int` | — | ✅ | `1` | ترتيب المستوى (يبدأ من 1). `Level = 1` يعني أول مستوى تعمق |
| `DisplayName` | `nvarchar(200)` | — | ✅ | — | الاسم المعروض للمستخدم (مثل: "تفاصيل المنطقة") |
| `DrillDownQuery` | `nvarchar(max)` | — | ✅ | — | استعلام SQL لهذا المستوى. يجب أن يقبل معامل (parameter) واحد على الأقل |
| `TargetChartType` | `nvarchar(50)` | — | ✅ | — | القيم المسموحة: `Bar`, `Line`, `Pie`, `KPI`, `Table`, `Gauge` |

**CHECK Constraints:**
- `CK_CardDrillDownLevels_Level`: `Level >= 1`
- `CK_CardDrillDownLevels_TargetChartType`: `TargetChartType IN ('Bar', 'Line', 'Pie', 'KPI', 'Table', 'Gauge')`

**Indexes:**
- `IX_CardDrillDownLevels_ParentCardId_Level` — `(ParentCardId, Level)` (متفرد) — يضمن عدم وجود مستويين بنفس الترتيب لنفس البطاقة
- `FK_CardDrillDownLevels_ParentCardId` — `ParentCardId` — foreign key index (يُنشأ تلقائياً مع FK)

**Foreign Keys:**
- `FK_CardDrillDownLevels_DashboardCards` — `ParentCardId` ← `DashboardCards.Id` — ON DELETE CASCADE

> **سبب اختيار ON DELETE CASCADE:** عند حذف بطاقة، جميع مستويات Drill Down الخاصة بها تُحذف تلقائياً — لا معنى لوجود مستويات لبطاقة غير موجودة.

---

### 1.3 جدول SyncSettings

**الغرض:** تخزين إعدادات المزامنة العامة — فترة المزامنة، حالة التشغيل التلقائي، وآخر وقت مزامنة ناجحة.

**مسار الكيان:** `WarehouseDashboard.Web/Models/Entities/SyncSetting.cs`
**DbContext:** `WarehouseDashboardDbContext`

| العمود | النوع في SQL Server | المفتاح | إلزامي | القيمة الافتراضية | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:-----------------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `IntervalMinutes` | `int` | — | ✅ | `30` | الفترة بين المزامنات التلقائية بالدقائق. الحد الأدنى: 1 |
| `IsAutoSyncEnabled` | `bit` | — | ✅ | `1` | `1` = المزامنة التلقائية مفعّلة |
| `LastSyncTimestamp` | `datetime2` | — | ❌ | `NULL` | آخر وقت تمت فيه مزامنة ناجحة. `NULL` = لم تتم مزامنة بعد |

**CHECK Constraints:**
- `CK_SyncSettings_IntervalMinutes`: `IntervalMinutes >= 1`

**ملاحظات:**
- هذا الجدول يحتوي عادةً على **صف واحد فقط** (Singleton row). يُسمح تقنياً بوجود عدة صفوف ولكن التطبيق يقرأ الصف الأول (`WHERE Id = 1`).
- يمكن إضافة `UQ_SyncSettings_Singleton` (اختياري) لحصر الصفوف عبر `Id = 1` فقط.

---

### 1.4 جدول AdminPassword

**الغرض:** تخزين كلمة مرور Admin Panel بشكل آمن (BCrypt Hash).

**مسار الكيان:** `WarehouseDashboard.Web/Models/Entities/AdminPassword.cs`
**DbContext:** `WarehouseDashboardDbContext`

| العمود | النوع في SQL Server | المفتاح | إلزامي | القيمة الافتراضية | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:-----------------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `PasswordHash` | `nvarchar(500)` | — | ✅ | — | BCrypt hash لكلمة المرور. الطول 500 لدعم Salts المتغيرة |
| `UpdatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت آخر تحديث لكلمة المرور |

**ملاحظات:**
- هذا الجدول يحتوي على **صف واحد فقط** (Singleton row) — لا يوجد سوى كلمة مرور واحدة نشطة.
- عند تغيير كلمة المرور، يتم تحديث `PasswordHash` و `UpdatedAt` في الصف نفسه.
- لا يتم حفظ Plain Text Password أبداً — التشفير يتم عبر `BCrypt.Net` قبل الحفظ.
- الطول `nvarchar(500)` يسمح BCrypt Hash حتى 255 حرفاً مع إمكانية التوسع المستقبلي.

**Indexes:**
- `UQ_AdminPassword_Singleton` — `Id` — ضمان وجود صف واحد فقط (اختياري — يُدار عبر التطبيق).

---

## 2. Sync Logs Tables (EF Core Managed)

الجداول التالية تُدار عبر **Entity Framework Core Migrations** في مشروع `WarehouseDashboard.Api` (وليس Web). يتم تسجيل عمليات المزامنة بشكل تلقائي من Sync Engine.

**DbContext:** `WarehouseDashboardLogContext` (في `WarehouseDashboard.Api/Data/`)

> **لماذا DbContext منفصل؟** لأن Sync Logs تُكتب من `WarehouseDashboard.Api` بينما Config Tables تُقرأ/تُكتب من `WarehouseDashboard.Web`. كل مشروع له DbContext منفصل (لنفس قاعدة البيانات) لمنع التبعيات غير الضرورية.

### 2.1 جدول SyncLogs

**الغرض:** تسجيل كل عملية مزامنة — التوقيت، الحالة، عدد السجلات، المدة، والأخطاء.

**مسار الكيان:** `WarehouseDashboard.Api/Models/SyncLog.cs`

| العمود | النوع في SQL Server | المفتاح | إلزامي | القيمة الافتراضية | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:-----------------:|-------------------|
| `Id` | `bigint` | PK, Identity(1,1) | ✅ | — | Auto-increment. `bigint` لاستيعاب آلاف السجلات على المدى الطويل |
| `StartTime` | `datetime2` | — | ✅ | — | وقت بدء المزامنة |
| `EndTime` | `datetime2` | — | ❌ | `NULL` | وقت انتهاء المزامنة. `NULL` = المزامنة لا تزال قيد التشغيل |
| `Status` | `nvarchar(20)` | — | ✅ | `'Running'` | القيم المسموحة: `'Running'`, `'Success'`, `'Failed'` |
| `RecordCount` | `int` | — | ✅ | `0` | عدد الصفوف المنقولة في هذه المزامنة |
| `Duration` | `bigint` | — | ❌ | `NULL` | مدة المزامنة بالميلي ثانية. `NULL` = لم تنتهِ بعد |
| `TriggerType` | `nvarchar(20)` | — | ✅ | `'Auto'` | القيم المسموحة: `'Auto'`, `'Manual'` |
| `ErrorMessage` | `nvarchar(max)` | — | ❌ | `NULL` | رسالة الخطأ إذا فشلت المزامنة. `NULL` = لا خطأ |

**CHECK Constraints:**
- `CK_SyncLogs_Status`: `Status IN ('Running', 'Success', 'Failed')`
- `CK_SyncLogs_TriggerType`: `TriggerType IN ('Auto', 'Manual')`
- `CK_SyncLogs_RecordCount`: `RecordCount >= 0`
- `CK_SyncLogs_Duration`: `Duration >= 0 OR Duration IS NULL`
- `CK_SyncLogs_Dates`: `EndTime IS NULL OR EndTime >= StartTime`

**Indexes:**
- `IX_SyncLogs_StartTime` — `StartTime DESC` (غير متفرد) — لترتيب السجلات من الأحدث إلى الأقدم في صفحة Sync Logs
- `IX_SyncLogs_Status` — `Status` (غير متفرد) — لتصفية السجلات حسب الحالة
- `IX_SyncLogs_TriggerType_StartTime` — `(TriggerType, StartTime DESC)` (غير متفرد) — لعرض أحدث السجلات حسب نوع التفعيل

---

### 2.2 جدول ErrorLogs

**الغرض:** تسجيل الأخطاء التفصيلية أثناء المزامنة — ربط بكل عملية Sync وكل جدول فشلت مزامنته.

**مسار الكيان:** `WarehouseDashboard.Api/Models/ErrorLog.cs`

| العمود | النوع في SQL Server | المفتاح | إلزامي | القيمة الافتراضية | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:-----------------:|-------------------|
| `Id` | `bigint` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `SyncLogId` | `bigint` | FK → SyncLogs.Id | ✅ | — | معرف سجل المزامنة المرتبط. ON DELETE CASCADE |
| `TableName` | `nvarchar(200)` | — | ✅ | — | اسم الجدول الذي فشلت مزامنته |
| `ErrorMessage` | `nvarchar(max)` | — | ✅ | — | رسالة الخطأ التفصيلية |
| `Timestamp` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت حدوث الخطأ |

**Indexes:**
- `IX_ErrorLogs_SyncLogId` — `SyncLogId` (غير متفرد) — لتسريع استعلام `WHERE SyncLogId = X`
- `IX_ErrorLogs_Timestamp` — `Timestamp DESC` (غير متفرد) — لعرض أحدث الأخطاء

**Foreign Keys:**
- `FK_ErrorLogs_SyncLogs` — `SyncLogId` ← `SyncLogs.Id` — ON DELETE CASCADE

> **سبب اختيار ON DELETE CASCADE:** عند حذف سجل مزامنة (أو تنظيف السجلات القديمة)، جميع أخطائه تُحذف تلقائياً.

---

## 3. Data Tables (ADO.NET Managed)

### 3.1 المبدأ العام

جداول البيانات (Data Tables) هي الجداول التي تُنقل إليها البيانات من Oracle. هذه الجداول **تُدار يدوياً عبر ADO.NET** — لا تستخدم EF Core Migrations ولا يتم تضمينها في DbContext.

**المسؤولية:** فريق التطوير يقوم بإنشاء هذه الجداول في SQL Server بناءً على جداول Oracle المقابلة.

### 3.2 قواعد إدارة Data Tables

| القاعدة | الشرح |
|---------|-------|
| **لا EF Core** | لا تُضاف هذه الجداول إلى `WarehouseDashboardDbContext` أو `WarehouseDashboardLogContext` |
| **إنشاء يدوي** | تُنشأ عبر `CREATE TABLE` يدوياً في SQL Server Management Studio أو عبر سكريبت SQL |
| **مطابقة لـ Oracle** | هيكل كل جدول يطابق هيكل الجدول المقابل في Oracle بعد تحويل أنواع البيانات |
| **نفس قاعدة البيانات** | جميع الجداول في نفس قاعدة البيانات `WarehouseDashboard` (ضمن Schema الافتراضي `dbo`) |
| **أسماء موحدة** | يفضل استخدام `stg_` prefix (اختياري) للتمييز عن Config/Logs Tables |
| **نوع البيانات** | استخدام أنواع SQL Server المناسبة (انظر §3.3) |
| **Primary Key** | يُوصى بإضافة PK لكل جدول إذا كان موجوداً في Oracle |
| **Indexes** | تُضاف يدوياً حسب الحاجة — لا توجد قاعدة آلية |
| **Transactional** | DELETE + INSERT في Transaction واحدة لكل جدول |

### 3.3 خريطة تحويل أنواع البيانات (Oracle → SQL Server)

يتم تحويل أنواع Oracle إلى أنواع SQL Server المقابلة أثناء نقل البيانات:

| نوع Oracle | نوع .NET (في DataTable) | نوع SQL Server المقترح | ملاحظات |
|------------|------------------------|------------------------|---------|
| `NUMBER(p,0)` | `long` أو `int` | `int` أو `bigint` | يعتمد على precision |
| `NUMBER(p,s)` حيث s > 0 | `decimal` | `decimal(p,s)` | يحافظ على الدقة |
| `VARCHAR2(n)` | `string` | `nvarchar(n)` أو `nvarchar(max)` | `nvarchar` لدعم Unicode |
| `NVARCHAR2(n)` | `string` | `nvarchar(n)` | |
| `CHAR(n)` | `string` | `nchar(n)` | |
| `DATE` | `DateTime` | `datetime2` | `datetime2` له دقة أعلى |
| `TIMESTAMP` | `DateTime` | `datetime2` | |
| `CLOB` | `string` | `nvarchar(max)` | |
| `BLOB` | `byte[]` | `varbinary(max)` | |
| `RAW(n)` | `byte[]` | `binary(n)` أو `varbinary(n)` | |
| `FLOAT` | `double` | `float` | |

**مبدأ التحويل:**
1. قراءة البيانات من Oracle عبر `OracleDataReader` ← `DataTable`
2. استخدام `GetValue()` والتحويل الضمني في ODP.NET
3. `SqlBulkCopy` يقرأ `DataTable` مباشرة ويكتب في SQL Server
4. العمود "يتطابق بالاسم" بين `DataTable.Columns` والجدول الهدف في SQL Server

### 3.4 هيكل الجداول — مؤجل (Deferred)

> **هام:** الهياكل الدقيقة لجداول البيانات تُحدد أثناء التنفيذ بناءً على:
> - جداول Oracle الفعلية التي يحددها العميل
> - متطلبات التقارير والـ Dashboard
> - استعلامات SQL التي سيكتبها الـ Admin في Admin Panel

**النهج الموصى به أثناء التنفيذ:**

```
1. العميل يُحدد جداول Oracle المطلوبة
2. لكل جدول Oracle:
   a. تحديد اسم الجدول الهدف في SQL Server
   b. تحويل أنواع البيانات حسب خريطة التحويل في §3.3
   c. إنشاء الجدول يدوياً: CREATE TABLE [TableName] ( ... )
   d. إضافة Primary Key (إن وُجد في Oracle)
   e. إضافة Indexes حسب الاستعلامات المتوقعة
3. إضافة الجدول إلى TableMapping في Sync Engine
4. اختبار المزامنة
```

### 3.5 مثال توضيحي (لن يتم تنفيذه فعلياً)

> المثال التالي لتوضيح النهج فقط — لا يُعتبر جزءاً من المشروع:

```sql
-- Oracle Table:
-- CREATE TABLE WMS.WAREHOUSE_STOCK (
--     WAREHOUSE_ID   NUMBER(10),
--     WAREHOUSE_NAME VARCHAR2(200),
--     REGION         VARCHAR2(100),
--     TOTAL_QTY      NUMBER(12,2),
--     LAST_UPDATED   DATE
-- );

-- SQL Server Target Table (يُنشأ يدوياً):
CREATE TABLE dbo.stg_WarehouseStock (
    WarehouseId   INT            NOT NULL,
    WarehouseName NVARCHAR(200)  NOT NULL,
    Region        NVARCHAR(100)  NULL,
    TotalQty      DECIMAL(12,2)  NOT NULL DEFAULT 0,
    LastUpdated   DATETIME2      NULL,
    CONSTRAINT PK_stg_WarehouseStock PRIMARY KEY (WarehouseId)
);
```

---

## 4. Entity Relationships, Indexes, and Constraints

### 4.1 مخطط العلاقات (ER Diagram — نصي)

```
┌───────────────────────────────────┐
│          DashboardCards           │
├───────────────────────────────────┤
│ Id (PK, int, Identity)           │◄────┐
│ Title (nvarchar(200))            │     │
│ ChartType (nvarchar(50))         │     │
│ SqlQuery (nvarchar(max))         │     │
│ DataSourceType (nvarchar(50))    │     │
│ GridPositionX (int)              │     │
│ GridPositionY (int)              │     │
│ GridWidth (int)                  │     │
│ GridHeight (int)                 │     │
│ RefreshInterval (int)            │     │
│ IsActive (bit)                   │     │
│ CreatedAt (datetime2)            │     │
│ UpdatedAt (datetime2)            │     │
└───────────────────────────────────┘     │
        │                                │
        │ 1                              │ N
        │                                │
        ▼                                │
┌───────────────────────────────────┐     │
│       CardDrillDownLevels         │     │
├───────────────────────────────────┤     │
│ Id (PK, int, Identity)           │     │
│ ParentCardId (FK, int)           │─────┘
│ Level (int)                      │     ON DELETE CASCADE
│ DisplayName (nvarchar(200))      │
│ DrillDownQuery (nvarchar(max))   │
│ TargetChartType (nvarchar(50))   │
└───────────────────────────────────┘

┌───────────────────────────────────┐
│          SyncSettings             │
├───────────────────────────────────┤
│ Id (PK, int, Identity)           │
│ IntervalMinutes (int)            │
│ IsAutoSyncEnabled (bit)          │
│ LastSyncTimestamp (datetime2?)   │
└───────────────────────────────────┘

┌───────────────────────────────────┐
│          AdminPassword            │
├───────────────────────────────────┤
│ Id (PK, int, Identity)           │
│ PasswordHash (nvarchar(500))     │
│ UpdatedAt (datetime2)            │
└───────────────────────────────────┘

┌───────────────────────────────────┐
│            SyncLogs               │
├───────────────────────────────────┤
│ Id (PK, bigint, Identity)        │◄────┐
│ StartTime (datetime2)            │     │
│ EndTime (datetime2?)             │     │
│ Status (nvarchar(20))            │     │
│ RecordCount (int)                │     │
│ Duration (bigint?)               │     │
│ TriggerType (nvarchar(20))       │     │
│ ErrorMessage (nvarchar(max)?)    │     │
└───────────────────────────────────┘     │
        │                                │
        │ 1                              │ N
        │                                │
        ▼                                │
┌───────────────────────────────────┐     │
│           ErrorLogs               │     │
├───────────────────────────────────┤     │
│ Id (PK, bigint, Identity)        │     │
│ SyncLogId (FK, bigint)           │─────┘
│ TableName (nvarchar(200))        │     ON DELETE CASCADE
│ ErrorMessage (nvarchar(max))     │
│ Timestamp (datetime2)            │
└───────────────────────────────────┘
```

### 4.2 ملخص العلاقات (Foreign Keys)

| FK name | Source Table | Source Column | Target Table | Target Column | Delete Rule |
|---------|-------------|---------------|-------------|---------------|:-----------:|
| `FK_CardDrillDownLevels_DashboardCards` | `CardDrillDownLevels` | `ParentCardId` | `DashboardCards` | `Id` | CASCADE |
| `FK_ErrorLogs_SyncLogs` | `ErrorLogs` | `SyncLogId` | `SyncLogs` | `Id` | CASCADE |

### 4.3 ملخص الفهارس (Indexes)

| الجدول | اسم الـ Index | الأعمدة | النوع | الغرض |
|--------|--------------|---------|:-----:|-------|
| `DashboardCards` | `IX_DashboardCards_IsActive` | `IsActive` | Non-clustered | تسريع `WHERE IsActive = 1` (تحميل البطاقات النشطة) |
| `DashboardCards` | `IX_DashboardCards_GridPositionX_GridPositionY` | `GridPositionX`, `GridPositionY` | Non-clustered | ترتيب البطاقات في الشبكة |
| `CardDrillDownLevels` | `IX_CardDrillDownLevels_ParentCardId_Level` | `ParentCardId`, `Level` | **Unique** | منع تكرار المستويات لنفس البطاقة |
| `SyncLogs` | `IX_SyncLogs_StartTime` | `StartTime DESC` | Non-clustered | ترتيب سجلات المزامنة (الأحدث أولاً) |
| `SyncLogs` | `IX_SyncLogs_Status` | `Status` | Non-clustered | تصفية السجلات حسب الحالة |
| `SyncLogs` | `IX_SyncLogs_TriggerType_StartTime` | `TriggerType`, `StartTime DESC` | Non-clustered | عرض أحدث السجلات حسب نوع التفعيل |
| `ErrorLogs` | `IX_ErrorLogs_SyncLogId` | `SyncLogId` | Non-clustered | ربط الأخطاء بسجل المزامنة |
| `ErrorLogs` | `IX_ErrorLogs_Timestamp` | `Timestamp DESC` | Non-clustered | عرض أحدث الأخطاء |

### 4.4 ملخص CHECK Constraints

| الجدول | اسم الـ Constraint | التعبير |
|--------|-------------------|---------|
| `DashboardCards` | `CK_DashboardCards_ChartType` | `ChartType IN ('Bar', 'Line', 'Pie', 'KPI', 'Table', 'Gauge')` |
| `DashboardCards` | `CK_DashboardCards_DataSourceType` | `DataSourceType IN ('SQL Query', 'View')` |
| `DashboardCards` | `CK_DashboardCards_GridWidth` | `GridWidth BETWEEN 1 AND 12` |
| `DashboardCards` | `CK_DashboardCards_GridHeight` | `GridHeight BETWEEN 1 AND 6` |
| `DashboardCards` | `CK_DashboardCards_RefreshInterval` | `RefreshInterval >= 0` |
| `CardDrillDownLevels` | `CK_CardDrillDownLevels_Level` | `Level >= 1` |
| `CardDrillDownLevels` | `CK_CardDrillDownLevels_TargetChartType` | `TargetChartType IN ('Bar', 'Line', 'Pie', 'KPI', 'Table', 'Gauge')` |
| `SyncSettings` | `CK_SyncSettings_IntervalMinutes` | `IntervalMinutes >= 1` |
| `SyncLogs` | `CK_SyncLogs_Status` | `Status IN ('Running', 'Success', 'Failed')` |
| `SyncLogs` | `CK_SyncLogs_TriggerType` | `TriggerType IN ('Auto', 'Manual')` |
| `SyncLogs` | `CK_SyncLogs_RecordCount` | `RecordCount >= 0` |
| `SyncLogs` | `CK_SyncLogs_Duration` | `Duration >= 0 OR Duration IS NULL` |
| `SyncLogs` | `CK_SyncLogs_Dates` | `EndTime IS NULL OR EndTime >= StartTime` |

---

## 5. EF Core vs ADO.NET Boundary

### 5.1 حدود المسؤوليات — جدول حاسم

| الجانب | EF Core (Migrations) | ADO.NET (يدوي) |
|--------|:-------------------:|:--------------:|
| **الجداول المُدارة** | `DashboardCards`, `CardDrillDownLevels`, `SyncSettings`, `AdminPassword`, `SyncLogs`, `ErrorLogs` | جميع Data Tables (البيانات المنقولة من Oracle) |
| **عدد الجداول** | 6 جداول ثابتة | متغير (حسب جداول Oracle) |
| **إنشاء الجداول** | تلقائي عبر `dotnet ef migrations add` + `dotnet ef database update` | يدوي عبر SQL Script |
| **تعديل الهيكل** | تلقائي عبر Migrations | يدوي عبر ALTER TABLE |
| **قراءة البيانات** | LINQ عبر DbContext | SqlCommand + SqlDataReader |
| **كتابة البيانات** | EF Core SaveChanges() | SqlBulkCopy.WriteToServer(DataTable) |
| **المشروع المسؤول** | `WarehouseDashboard.Web` (Config) + `WarehouseDashboard.Api` (Logs) | `WarehouseDashboard.Api` |
| **DbContext** | `WarehouseDashboardDbContext` (Web) + `WarehouseDashboardLogContext` (Api) | لا DbContext — ADO.NET مباشر |
| **Package** | `Microsoft.EntityFrameworkCore.SqlServer` | `Microsoft.Data.SqlClient` |
| **مناسب لـ** | CRUD بسيط، علاقات، Validation | Bulk Insert، أداء عالٍ |
| **غير مناسب لـ** | Bulk Insert (بطيء) | CRUD معقد مع علاقات متعددة |

### 5.2 تدفق الوصول للجداول

```
                         SQL Server Database: WarehouseDashboard
                                    │
         ┌──────────────────────────┴──────────────────────────┐
         │                                                     │
         ▼                                                     ▼
┌─────────────────────┐                           ┌─────────────────────┐
│ WarehouseDashboard  │                           │ WarehouseDashboard  │
│       .Web          │                           │       .Api          │
│                     │                           │                     │
│ EF Core DbContext:  │                           │ EF Core DbContext:  │
│ WarehouseDashboard  │                           │ WarehouseDashboard  │
│ DbContext           │                           │ LogContext          │
│                     │                           │                     │
│ يقرأ/يكتب:          │                           │ يقرأ/يكتب:          │
│ ├── DashboardCards  │                           │ ├── SyncLogs        │
│ ├── CardDrillDown.. │                           │ └── ErrorLogs       │
│ ├── SyncSettings    │                           │                     │
│ └── AdminPassword   │                           │ ADO.NET (مباشر):    │
│                     │                           │ ├── DELETE FROM     │
│                     │                           │ ├── SqlBulkCopy     │
│                     │                           │ → Data Tables       │
└─────────────────────┘                           └─────────────────────┘
```

### 5.3 قواعد الحوكمة

| # | القاعدة | التفاصيل | المخالفة تؤدي إلى |
|:-:|---------|----------|:-----------------:|
| G1 | **لا EF Core لـ Data Tables** | لا يُسمح بإضافة Data Tables إلى أي DbContext | مراجعة معمارية |
| G2 | **لا ADO.NET لـ Config/Lookups** | Config Tables تُقرأ/تُكتب عبر EF Core فقط | استثناء: استعلامات Dashboard المخصصة (SQL مباشر) |
| G3 | **لا EF Core لـ Bulk Insert** | أي عملية Bulk Insert للبيانات → ADO.NET SqlBulkCopy | مراجعة أداء |
| G4 | **DbContext منفصل لكل مشروع** | `WarehouseDashboardDbContext` (Web) ≠ `WarehouseDashboardLogContext` (Api) | إعادة هيكلة |
| G5 | **Data Tables تنشأ يدوياً** | لا `dotnet ef migrations add` لـ Data Tables | فشل في Pre-Execution Gate |
| G6 | **Dashboard يقرأ Data Tables عبر ADO.NET** | DashboardService يستخدم `SqlCommand` لتنفيذ `SqlQuery` من `DashboardCards` | هذه هي القاعدة — لا EF Core here |

### 5.4 حالة خاصة: DashboardService وقراءة Data Tables

عند عرض Dashboard، يقوم `DashboardService` (في `WarehouseDashboard.Web`) بقراءة `SqlQuery` من جدول `DashboardCards` (عبر EF Core) ثم تنفيذ هذا الاستعلام ضد SQL Server (عبر ADO.NET) — هذا الاستعلام قد يقرأ من Data Tables.

```
DashboardService.GetCards():
  1. EF Core: SELECT * FROM DashboardCards WHERE IsActive = 1
  2. لكل بطاقة:
     a. ADO.NET: SqlCommand.ExecuteReader(card.SqlQuery)
     b. ربط النتيجة في ViewModel
     c. تمريرها إلى Razor Page

🔹 Config → EF Core
🔹 Data Query → ADO.NET
```

هذا المزيج مقصود ومعتمد — **لا يُعتبر خرقاً للقاعدة G2** لأن الاستعلام المخصص (SqlQuery) هو Query فقط وليس Write، وهو يُدار عبر `DashboardService` وليس عبر DbContext.

---

## 6. Design Decisions and Rationale

### 6.1 Why `bigint` for SyncLogs.Id and ErrorLogs.Id?

| الخيار | المبرر |
|--------|--------|
| `int` (32-bit) | كافٍ لـ 2 مليار سجل — ولكن مع مزامنة كل 30 دقيقة، قد نصل إلى 17,520 سجل/سنة. مع 10 سنوات = 175,200 سجل. `int` كافٍ. |
| `bigint` (64-bit) | **الاختيار المعتمد.** استباقي — لا يوجد سبب مقنع لاستخدام `int` عندما يكون `bigint` بنفس التكلفة التخزينية تقريباً (8 بايت vs 4 بايت) مع عدم وجود حد عملي. |

**القرار:** `bigint` لكلا الجدولين — استباقي لضمان عدم الحاجة لتغيير النوع لاحقاً.

### 6.2 Why `nvarchar` instead of `varchar`?

العميل يستخدم اللغة العربية بشكل أساسي. `nvarchar` يدعم Unicode (عربي، إنجليزي، رموز خاصة) بدون مشاكل تشفير. جميع الحقول النصية تستخدم `nvarchar` حتى لو كانت القيم باللغة الإنجليزية حالياً.

### 6.3 Why `datetime2` instead of `datetime`?

| الميزة | `datetime` | `datetime2` |
|--------|:----------:|:-----------:|
| الدقة | ~3.33ms | 100ns (7000x أدق) |
| نطاق السنوات | 1753-9998 | 0001-9999 |
| حجم التخزين | 8 bytes | 6-8 bytes |
| متوافق مع .NET DateTime | ✅ | ✅ |

**القرار:** `datetime2` — دقة أعلى، حجم أصغر أو مماثل، توافق كامل مع .NET.

### 6.4 Why `ON DELETE CASCADE` for both FKs?

- **CardDrillDownLevels → DashboardCards:** إذا حُذفت بطاقة، فجميع مستويات Drill Down الخاصة بها غير صالحة — لا معنى لوجودها.
- **ErrorLogs → SyncLogs:** إذا حُذف سجل مزامنة (أو تم تنظيف السجلات القديمة)، فأخطاؤه غير صالحة.

إذا احتجنا لاحقاً إلى الاحتفاظ بسجلات الأخطاء لمراجعة تاريخية حتى بعد حذف SyncLogs، يمكن تغيير `ErrorLogs → SyncLogs` إلى `ON DELETE SET NULL` مع جعل `SyncLogId` nullable.

### 6.5 Why two DbContexts instead of one?

| الخيار | التحليل |
|--------|---------|
| **DbContext واحد** | مشاركة الكلاسات بين المشروعين = تبعية غير ضرورية. Api يحتاج فقط إلى `SyncLog` و `ErrorLog`، بينما Web يحتاج إلى `DashboardCard` و `CardDrillDownLevel` و `SyncSettings` و `AdminPassword`. |
| **DbContext منفصل (المعتمد)** | كل مشروع لديه DbContext خاص به مع الكلاسات التي يحتاجها فقط. لا تبعية بين المشروعين. |

> **ملاحظة:** كلا الـ DbContexts يشيران إلى **نفس قاعدة البيانات** (`WarehouseDashboard`). الفصل هو على مستوى التطبيق، ليس على مستوى قاعدة البيانات.

### 6.6 Why no separate Audit Columns (CreatedBy, UpdatedBy)?

في Phase 1، لا يوجد نظام Users/Roles (Viewer مؤجل إلى Phase 2). المستخدم الوحيد هو Admin (مستخدم واحد مشترك). إضافة `CreatedBy` و `UpdatedBy` لا تضيف قيمة حالياً — يمكن إضافتها في Phase 2 مع نظام RBAC.

---

## 7. Data Model Compliance Checklist

| البند | الحالة | المرجع |
|-------|:------:|--------|
| الجداول مغطاة بالكامل | ✅ | جميع الجداول الـ 6 محددة (4 Config + 2 Logs) |
| العلاقات موثقة | ✅ | 2 FK relationships مع ON DELETE CASCADE |
| الفهارس محددة | ✅ | 9 Indexes (7 Non-clustered + 1 Unique + 1 FK auto) |
| CHECK Constraints محددة | ✅ | 13 CHECK Constraints |
| خريطة تحويل Oracle → SQL Server | ✅ | 10 أنواع Oracle موثقة |
| حدود EF Core vs ADO.NET | ✅ | 6 قواعد حوكمة + 1 حالة خاصة |
| جداول البيانات (Data Tables) | ✅ | مؤجلة — منهجية الإنشاء موثقة |
| Lifecycle Header | ✅ | الحالة: Module Baseline Approved |

---

## 8. Open Issues

| # | المشكلة | التأثير | الحل المقترح |
|:-:|---------|:-------:|-------------|
| 1 | تفاصيل جداول Oracle غير معروفة بعد | يمنع تحديد هيكل Data Tables | تُحدد أثناء التنفيذ — العميل متاح |
| 2 | هل `SyncLogId` في `ErrorLogs` يجب أن يكون `ON DELETE SET NULL`؟ | منخفض — CASCADE كافٍ حالياً | يمكن تغييره لاحقاً عند الحاجة |
| 3 | هل نحتاج Unique Index على `SyncSettings.Id` لمنع صفوف متعددة؟ | منخفض — التطبيق يقرأ `WHERE Id = 1` | يمكن إضافة UQ عند أول migration |

---

> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-007
> **تاريخ:** 2026-07-12
> **الحالة:** `Module Baseline Approved` ✅
> **الاعتماد:** بالاستناد إلى `08_TECHNICAL_ARCHITECTURE.md` (§3.2) و `dotnet-razorpages-adonet.md`
