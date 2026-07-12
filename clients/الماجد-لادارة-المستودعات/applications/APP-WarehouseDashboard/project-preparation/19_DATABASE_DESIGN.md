# 19_DATABASE_DESIGN.md — WarehouseDashboard

> **النوع:** Database Design Document — التصميم المرجعي لقاعدة بيانات SQL Server
> **الحالة:** `Draft` (مشتق من وثائق `Module Baseline Approved`: 06، 08 — يراجَع بسبب تعارض في نطاق جدولين، انظر §9)
> **Baseline Module:** WarehouseDashboard
> **تاريخ الإعداد:** 2026-07-12
> **الجهة المُعدّة:** Software Designer Agent (مُصمم) — TASK-PREP-013
> **الملفات المرجعية:** `06_DATA_MODEL_PREPARATION.md` (MBA)، `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `tera-system/profiles/dotnet-razorpages-adonet.md`

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Draft` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | بعد حسم تعارض `AdminUsers` / `AuditLog` (انظر §9) |
| **Document Type** | Database Design |
| **Version** | 1.0.0 (draft) |

> **ملاحظة حوكمة:** هذا المستند مشتق بالكامل من `06_DATA_MODEL_PREPARATION.md` و `08_TECHNICAL_ARCHITECTURE.md` (كلاهما `Module Baseline Approved`). الجداول الستة المعتمدة في الـ Baseline هي: `DashboardCards`, `CardDrillDownLevels`, `SyncSettings`, `AdminPassword`, `SyncLogs`, `ErrorLogs`. قائمة المهمة (TASK-PREP-013) ذكرت `AdminUsers` و `AuditLog` بدلاً من `AdminPassword` و `SyncSettings` — وهذا تعارض مُوثَّق في §9 ويجب حسمه قبل اعتماد هذا المستند.

---

## نبذة عن الوثيقة

هذا المستند هو **التصميم المرجعي (authoritative)** لقاعدة بيانات SQL Server الخاصة بتطبيق **WarehouseDashboard**. قاعدة البيانات تُسمّى `WarehouseDashboard` وتحتوي على ثلاث مجموعات:

1. **Config Tables** — تُدار عبر **EF Core Migrations** (مشروع `WarehouseDashboard.Web`).
2. **Sync Logs Tables** — تُدار عبر **EF Core Migrations** (مشروع `WarehouseDashboard.Api` عبر `WarehouseDashboardLogContext`).
3. **Data Tables** — تُنقل من Oracle عبر **ADO.NET SqlBulkCopy** (تُنشأ يدوياً، هيكلها مؤجّل إلى وقت التنفيذ — انظر §5).

جميع الجداول ضمن Schema الافتراضي `dbo`. الأنواع النصية تستخدم `nvarchar` (دعم Unicode للعربية)، والتواريخ تستخدم `datetime2`.

---

## 1. جداول SQL Server المعتمدة (من الـ Baseline)

### 1.1 جدول `DashboardCards` (Config — EF Core)

**الغرض:** تخزين تكوين كل بطاقة في Dashboard — النوع، مصدر البيانات، الموقع، الحجم، خيارات العرض.

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `Title` | `nvarchar(200)` | — | ✅ | — | عنوان البطاقة |
| `ChartType` | `nvarchar(50)` | — | ✅ | — | `Bar`,`Line`,`Pie`,`KPI`,`Table`,`Gauge` — CHECK |
| `SqlQuery` | `nvarchar(max)` | — | ✅ | — | استعلام SQL أو اسم View |
| `DataSourceType` | `nvarchar(50)` | — | ✅ | `'SQL Query'` | `'SQL Query'` أو `'View'` — CHECK |
| `GridPositionX` | `int` | — | ✅ | `0` | موقع الشبكة X (من 0) |
| `GridPositionY` | `int` | — | ✅ | `0` | موقع الشبكة Y (من 0) |
| `GridWidth` | `int` | — | ✅ | `4` | 1–12 — CHECK |
| `GridHeight` | `int` | — | ✅ | `2` | 1–6 — CHECK |
| `RefreshInterval` | `int` | — | ✅ | `0` | ثوانٍ؛ `0` = بلا تحديث تلقائي — CHECK ≥ 0 |
| `IsActive` | `bit` | — | ✅ | `1` | `1`=معروضة، `0`=مخفية |
| `CreatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت الإنشاء |
| `UpdatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت آخر تعديل |

**CHECK Constraints:**
- `CK_DashboardCards_ChartType`: `ChartType IN ('Bar','Line','Pie','KPI','Table','Gauge')`
- `CK_DashboardCards_DataSourceType`: `DataSourceType IN ('SQL Query','View')`
- `CK_DashboardCards_GridWidth`: `GridWidth BETWEEN 1 AND 12`
- `CK_DashboardCards_GridHeight`: `GridHeight BETWEEN 1 AND 6`
- `CK_DashboardCards_RefreshInterval`: `RefreshInterval >= 0`

---

### 1.2 جدول `CardDrillDownLevels` (Config — EF Core)

**الغرض:** مستويات Drill Down لكل بطاقة — استعلامات التعمق ونوع الرسم في كل مستوى.

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `ParentCardId` | `int` | FK → `DashboardCards.Id` | ✅ | — | ON DELETE CASCADE |
| `Level` | `int` | — | ✅ | `1` | يبدأ من 1 — CHECK ≥ 1 |
| `DisplayName` | `nvarchar(200)` | — | ✅ | — | الاسم المعروض |
| `DrillDownQuery` | `nvarchar(max)` | — | ✅ | — | استعلام يقبل معامل واحد على الأقل |
| `TargetChartType` | `nvarchar(50)` | — | ✅ | — | نفس قيم `ChartType` — CHECK |

**CHECK Constraints:**
- `CK_CardDrillDownLevels_Level`: `Level >= 1`
- `CK_CardDrillDownLevels_TargetChartType`: `TargetChartType IN ('Bar','Line','Pie','KPI','Table','Gauge')`

**Foreign Keys:**
- `FK_CardDrillDownLevels_DashboardCards`: `ParentCardId` ← `DashboardCards.Id` — **ON DELETE CASCADE**

---

### 1.3 جدول `SyncSettings` (Config — EF Core) — *مُدرَج في الـ Baseline، غير مذكور في قائمة المهمة*

**الغرض:** إعدادات المزامنة العامة (فترة التكرار، التشغيل التلقائي، آخر وقت نجاح). Singleton row.

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `IntervalMinutes` | `int` | — | ✅ | `30` | دقائق — CHECK ≥ 1 |
| `IsAutoSyncEnabled` | `bit` | — | ✅ | `1` | `1`=مفعّلة |
| `LastSyncTimestamp` | `datetime2` | — | ❌ | `NULL` | آخر مزامنة ناجحة |

**CHECK Constraint:** `CK_SyncSettings_IntervalMinutes`: `IntervalMinutes >= 1`

> ملاحظة: هذا الجدول جزء من التصميم المعتمد لكنه **لم يُذكر** في قائمة المهمة (TASK-PREP-013). تم إدراجه هنا لاكتمال التصميم المرجعي.

---

### 1.4 جدول `AdminPassword` (Config — EF Core) — *المعتمد في الـ Baseline بدلاً من `AdminUsers`*

**الغرض:** كلمة مرور Admin Panel المشفرة (BCrypt Hash) — صف واحد فقط (Singleton) لأن Phase 1 لا يحتوي نظام Users/Roles.

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `PasswordHash` | `nvarchar(500)` | — | ✅ | — | BCrypt hash |
| `UpdatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت آخر تحديث |

> ⚠️ **تعارض مع المهمة:** المهمة طلبت جدول `AdminUsers`، لكن الـ Baseline المعتمد يستخدم `AdminPassword` (Singleton) ويؤجّل نظام Users إلى Phase 2 (RBAC). راجع §9 و §2.1 للمقترح البديل.

---

### 1.5 جدول `SyncLogs` (Sync Logs — EF Core / Api)

**الغرض:** تسجيل كل عملية مزامنة — التوقيت، الحالة، عدد السجلات، المدة، الخطأ.

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `bigint` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `StartTime` | `datetime2` | — | ✅ | — | وقت البدء |
| `EndTime` | `datetime2` | — | ❌ | `NULL` | `NULL`=قيد التشغيل |
| `Status` | `nvarchar(20)` | — | ✅ | `'Running'` | `Running`,`Success`,`Failed` — CHECK |
| `RecordCount` | `int` | — | ✅ | `0` | عدد الصفوف — CHECK ≥ 0 |
| `Duration` | `bigint` | — | ❌ | `NULL` | ميلي ثانية |
| `TriggerType` | `nvarchar(20)` | — | ✅ | `'Auto'` | `Auto`,`Manual` — CHECK |
| `ErrorMessage` | `nvarchar(max)` | — | ❌ | `NULL` | رسالة الخطأ |

**CHECK Constraints:**
- `CK_SyncLogs_Status`: `Status IN ('Running','Success','Failed')`
- `CK_SyncLogs_TriggerType`: `TriggerType IN ('Auto','Manual')`
- `CK_SyncLogs_RecordCount`: `RecordCount >= 0`
- `CK_SyncLogs_Duration`: `Duration >= 0 OR Duration IS NULL`
- `CK_SyncLogs_Dates`: `EndTime IS NULL OR EndTime >= StartTime`

---

### 1.6 جدول `ErrorLogs` (Sync Logs — EF Core / Api)

**الغرض:** الأخطاء التفصيلية أثناء المزامنة — مرتبطة بسجل Sync واسم الجدول الفاشل.

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `bigint` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `SyncLogId` | `bigint` | FK → `SyncLogs.Id` | ✅ | — | ON DELETE CASCADE |
| `TableName` | `nvarchar(200)` | — | ✅ | — | الجدول الفاشل |
| `ErrorMessage` | `nvarchar(max)` | — | ✅ | — | رسالة الخطأ التفصيلية |
| `Timestamp` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت الخطأ |

**Foreign Keys:**
- `FK_ErrorLogs_SyncLogs`: `SyncLogId` ← `SyncLogs.Id` — **ON DELETE CASCADE**

---

## 2. جداول مطلوبة في المهمة وغير مُعتمدة في الـ Baseline (Design Gaps — مقترحات)

> الأقسام التالية هي **مقترحات غير معتمدة** تلبيةً لطلب المهمة بذكر `AdminUsers` و `AuditLog`. لا تُعتبر جزءاً من التصميم المرجعي حتى يُحسم التعارض في §9.

### 2.1 جدول `AdminUsers` (مقترح — للمراجعة)

**الغرض المقترح:** استبدال `AdminPassword` بنظام مستخدمين يدعم RBAC المستقبلي (Phase 2). إذا اعتُمد، يُرشَّح إيقاف/دمج جدول `AdminPassword`.

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `Username` | `nvarchar(100)` | — | ✅ | — | فريد — UQ |
| `PasswordHash` | `nvarchar(500)` | — | ✅ | — | BCrypt hash |
| `Role` | `nvarchar(50)` | — | ✅ | `'Admin'` | `Admin`,`Viewer` — CHECK |
| `IsActive` | `bit` | — | ✅ | `1` | `1`=مفعّل |
| `CreatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت الإنشاء |
| `UpdatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت آخر تعديل |

**CHECK Constraint:** `CK_AdminUsers_Role`: `Role IN ('Admin','Viewer')`
**Unique Index:** `UQ_AdminUsers_Username`: `Username`

---

### 2.2 جدول `AuditLog` (مقترح — للمراجعة)

**الغرض المقترح:** سجل تدقيق للتغييرات على جداول التكوين (إنشاء/تعديل/حذف بطاقة، تغيير كلمة المرور، تعديل إعدادات المزامنة). غير موجود في الـ Baseline (الذي أجّل الأعمدة التدقيقية إلى Phase 2).

| العمود | النوع | المفتاح | إلزامي | افتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:-------:|-------------------|
| `Id` | `bigint` | PK, Identity(1,1) | ✅ | — | Auto-increment |
| `TableName` | `nvarchar(200)` | — | ✅ | — | الجدول المُغيَّر |
| `RecordId` | `nvarchar(100)` | — | ❌ | `NULL` | مفتاح السجل المُغيَّر |
| `Action` | `nvarchar(20)` | — | ✅ | — | `Insert`,`Update`,`Delete` — CHECK |
| `ChangedBy` | `nvarchar(100)` | — | ❌ | `NULL` | المستخدم (Phase 1 = Admin) |
| `ChangedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت التغيير |
| `OldValues` | `nvarchar(max)` | — | ❌ | `NULL` | JSON للقيم القديمة |
| `NewValues` | `nvarchar(max)` | — | ❌ | `NULL` | JSON للقيم الجديدة |

**CHECK Constraint:** `CK_AuditLog_Action`: `Action IN ('Insert','Update','Delete')`

---

## 3. الفهارس (Indexes)

| الجدول | اسم الـ Index | الأعمدة | النوع | الغرض |
|--------|--------------|---------|:-----:|-------|
| `DashboardCards` | `IX_DashboardCards_IsActive` | `IsActive` | Non-clustered | تسريع `WHERE IsActive = 1` |
| `DashboardCards` | `IX_DashboardCards_GridPositionX_GridPositionY` | `GridPositionX`, `GridPositionY` | Non-clustered | ترتيب البطاقات في الشبكة |
| `CardDrillDownLevels` | `IX_CardDrillDownLevels_ParentCardId_Level` | `ParentCardId`, `Level` | **Unique** | منع تكرار المستويات لنفس البطاقة |
| `SyncLogs` | `IX_SyncLogs_StartTime` | `StartTime DESC` | Non-clustered | ترتيب السجلات (الأحدث أولاً) |
| `SyncLogs` | `IX_SyncLogs_Status` | `Status` | Non-clustered | تصفية حسب الحالة |
| `SyncLogs` | `IX_SyncLogs_TriggerType_StartTime` | `TriggerType`, `StartTime DESC` | Non-clustered | أحدث السجلات حسب نوع التفعيل |
| `ErrorLogs` | `IX_ErrorLogs_SyncLogId` | `SyncLogId` | Non-clustered | ربط الأخطاء بسجل المزامنة |
| `ErrorLogs` | `IX_ErrorLogs_Timestamp` | `Timestamp DESC` | Non-clustered | عرض أحدث الأخطاء |
| `AdminUsers` *(مقترح)* | `UQ_AdminUsers_Username` | `Username` | **Unique** | منع تكرار اسم المستخدم |
| `AuditLog` *(مقترح)* | `IX_AuditLog_TableName_ChangedAt` | `TableName`, `ChangedAt DESC` | Non-clustered | استعلامات التدقيق |

---

## 4. مخطط العلاقات (Relationships Diagram — نصي)

```
┌─────────────────────────┐          ┌──────────────────────────────┐
│      DashboardCards     │ 1      N │      CardDrillDownLevels     │
│─────────────────────────│◄─────────│──────────────────────────────│
│ Id (PK, int, Identity)  │          │ Id (PK, int, Identity)        │
│ Title (nvarchar(200))   │          │ ParentCardId (FK, int) ───────┤─ ON DELETE CASCADE
│ ChartType (nvarchar50)  │          │ Level (int)                   │
│ SqlQuery (nvarchar max) │          │ DisplayName (nvarchar200)     │
│ DataSourceType (nv50)   │          │ DrillDownQuery (nvarchar max) │
│ GridPositionX/Y (int)   │          │ TargetChartType (nvarchar50)  │
│ GridWidth/Height (int)  │          └──────────────────────────────┘
│ RefreshInterval (int)   │
│ IsActive (bit)          │          ┌──────────────────────────────┐
│ CreatedAt (datetime2)   │          │         SyncSettings          │
│ UpdatedAt (datetime2)   │          │──────────────────────────────│
└─────────────────────────┘          │ Id (PK, int, Identity)        │
                                     │ IntervalMinutes (int)          │
                                     │ IsAutoSyncEnabled (bit)        │
                                     │ LastSyncTimestamp (datetime2?) │
┌─────────────────────────┐          └──────────────────────────────┘
│       AdminPassword     │
│─────────────────────────│          ┌──────────────────────────────┐
│ Id (PK, int, Identity)  │          │          SyncLogs             │
│ PasswordHash (nv500)    │          │──────────────────────────────│
│ UpdatedAt (datetime2)   │ 1      N │ Id (PK, bigint, Identity)     │
└─────────────────────────┘          │ StartTime (datetime2)         │
                                     │ EndTime (datetime2?)          │
                                     │ Status (nvarchar20)           │
┌──────────── (مقترح) ────┐          │ RecordCount (int)            │
│        AdminUsers        │          │ Duration (bigint?)           │
│─────────────────────────│          │ TriggerType (nvarchar20)     │
│ Id (PK, int)            │          │ ErrorMessage (nvarchar max?) │
│ Username (nvarchar100)  │          └───────────────┬──────────────┘
│ PasswordHash (nv500)    │ 1                    N   │
│ Role (nvarchar50)       │ ◄────────────────────────┘ ON DELETE CASCADE
│ IsActive (bit)          │          ┌──────────────────────────────┐
│ CreatedAt/UpdatedAt     │          │          ErrorLogs            │
└─────────────────────────┘          │──────────────────────────────│
                                     │ Id (PK, bigint, Identity)     │
┌──────────── (مقترح) ────┐          │ SyncLogId (FK, bigint) ───────┤
│         AuditLog         │          │ TableName (nvarchar200)       │
│─────────────────────────│          │ ErrorMessage (nvarchar max)   │
│ Id (PK, bigint)         │          │ Timestamp (datetime2)         │
│ TableName (nvarchar200) │          └──────────────────────────────┘
│ RecordId (nvarchar100?) │
│ Action (nvarchar20)     │
│ ChangedBy (nvarchar100?)│
│ ChangedAt (datetime2)   │
│ OldValues (nvarchar max)│
│ NewValues (nvarchar max)│
└─────────────────────────┘
```

**ملخص Foreign Keys (المعتمدة فقط):**

| FK | Source | Column | Target | Column | Delete Rule |
|----|--------|--------|--------|--------|:-----------:|
| `FK_CardDrillDownLevels_DashboardCards` | `CardDrillDownLevels` | `ParentCardId` | `DashboardCards` | `Id` | CASCADE |
| `FK_ErrorLogs_SyncLogs` | `ErrorLogs` | `SyncLogId` | `SyncLogs` | `Id` | CASCADE |

---

## 5. جداول بيانات Oracle (مرجعية فقط — تُنقل عبر Sync)

جداول البيانات (Data Tables) **تُنشأ يدوياً** عبر ADO.NET ولا تُدار بواسطة EF Core. هيكلها **مؤجَّل إلى وقت التنفيذ** بناءً على جداول Oracle الفعلية التي يحددها العميل.

**قواعد الإدارة:**
- لا تُضاف إلى أي `DbContext`.
- تُنشأ بـ `CREATE TABLE` يدوي في SQL Server (يفضّل prefix اختياري `stg_`).
- تُطابق هيكل جدول Oracle بعد تحويل الأنواع (انظر الخريطة أدناه).
- DELETE + SqlBulkCopy في Transaction واحدة لكل جدول (Full Refresh).
- تُربط بـ Sync Engine عبر `TableMapping`.

**خريطة تحويل الأنواع (Oracle → SQL Server):**

| Oracle | .NET | SQL Server | ملاحظات |
|--------|------|-----------|---------|
| `NUMBER(p,0)` | `long`/`int` | `int`/`bigint` | حسب precision |
| `NUMBER(p,s>0)` | `decimal` | `decimal(p,s)` | يحافظ الدقة |
| `VARCHAR2(n)` | `string` | `nvarchar(n)`/`nvarchar(max)` | Unicode |
| `NVARCHAR2(n)` | `string` | `nvarchar(n)` | |
| `CHAR(n)` | `string` | `nchar(n)` | |
| `DATE` | `DateTime` | `datetime2` | دقة أعلى |
| `TIMESTAMP` | `DateTime` | `datetime2` | |
| `CLOB` | `string` | `nvarchar(max)` | |
| `BLOB` | `byte[]` | `varbinary(max)` | |
| `RAW(n)` | `byte[]` | `varbinary(n)` | |
| `FLOAT` | `double` | `float` | |

> مثال توضيحي (لا يُنفَّذ): `WMS.WAREHOUSE_STOCK` في Oracle → `dbo.stg_WarehouseStock` في SQL Server بنفس الأعمدة بعد التحويل.

---

## 6. نطاق EF Core (Config + Logs فقط — لا تحميل بيانات)

| الجانب | EF Core (Migrations) | ADO.NET (يدوي) |
|--------|:-------------------:|:--------------:|
| **الجداول المُدارة** | `DashboardCards`, `CardDrillDownLevels`, `SyncSettings`, `AdminPassword` (والمقترح `AdminUsers`,`AuditLog` إن اعتُمدت), `SyncLogs`, `ErrorLogs` | جميع Data Tables المنقولة من Oracle |
| **المشروع** | `WarehouseDashboard.Web` (Config) + `WarehouseDashboard.Api` (`WarehouseDashboardLogContext` للسجلات) | `WarehouseDashboard.Api` |
| **DbContext** | `WarehouseDashboardDbContext` (Web) + `WarehouseDashboardLogContext` (Api) | لا DbContext — ADO.NET مباشر |
| **الإنشاء** | `dotnet ef migrations add` + `dotnet ef database update` | `CREATE TABLE` يدوي |
| **الكتابة** | `SaveChanges()` | `SqlBulkCopy.WriteToServer(DataTable)` |
| **القراءة** | LINQ | `SqlCommand` + `SqlDataReader` |
| **الحزمة** | `Microsoft.EntityFrameworkCore.SqlServer` | `Microsoft.Data.SqlClient` |

**قواعد الحوكمة:**
- **G1:** لا EF Core لـ Data Tables.
- **G2:** لا ADO.NET لـ Config/Lookups (استثناء: استعلام `SqlQuery` المخصص في `DashboardService` — قراءة فقط).
- **G3:** لا EF Core لـ Bulk Insert (→ SqlBulkCopy).
- **G4:** DbContext منفصل لكل مشروع.
- **G5:** Data Tables تُنشأ يدوياً.
- **G6:** Dashboard يقرأ Data Tables عبر ADO.NET.

> **الخلاصة:** EF Core يُستخدم حصراً لتعريف/تحديث هيكل جداول التكوين والسجلات وتنفيذ CRUD بسيط عليها. تحميل بيانات Oracle → SQL Server يتم عبر ADO.NET فقط، خارج نطاق EF Core.

---

## 7. اتفاقيات التسمية (Naming Conventions)

| العنصر | الاتفاقية | أمثلة |
|--------|-----------|-------|
| **اسم الجدول** | `PascalCase` مفرد/جمع حسب الدلالة | `DashboardCards`, `SyncLogs`, `ErrorLogs` |
| **اسم العمود** | `PascalCase` | `GridPositionX`, `RefreshInterval` |
| **Primary Key** | `Id` (Identity) واسم الـ Constraint `PK_<TableName>` | `PK_DashboardCards` |
| **Foreign Key** | `FK_<ChildTable>_<ParentTable>` | `FK_ErrorLogs_SyncLogs` |
| **Index** | `IX_<TableName>_<Columns>` | `IX_SyncLogs_StartTime` |
| **Unique Index** | `UQ_<TableName>_<Column>` | `UQ_AdminUsers_Username` |
| **Check Constraint** | `CK_<TableName>_<Field>` | `CK_DashboardCards_GridWidth` |
| **Data Tables (Oracle)** | بادئة اختيارية `stg_` | `stg_WarehouseStock` |
| **Schema** | `dbo` (الافتراضي) | `dbo.DashboardCards` |
| **الأنواع** | `nvarchar` للنص، `datetime2` للتاريخ، `bigint` للسجلات عالية الحجم | — |
| **CommandText** | استعلامات `SqlQuery` تُخزَّن كما هي (Oracle-style → تُترجَم وقت التنفيذ عبر ADO.NET) | — |

---

## 8. DDL مرجعي مختصر (Documentation Only)

```sql
-- Config Tables
CREATE TABLE dbo.DashboardCards (
    Id              INT            IDENTITY(1,1) PRIMARY KEY,
    Title           NVARCHAR(200)  NOT NULL,
    ChartType       NVARCHAR(50)   NOT NULL,
    SqlQuery        NVARCHAR(MAX)  NOT NULL,
    DataSourceType  NVARCHAR(50)   NOT NULL DEFAULT 'SQL Query',
    GridPositionX   INT            NOT NULL DEFAULT 0,
    GridPositionY   INT            NOT NULL DEFAULT 0,
    GridWidth       INT            NOT NULL DEFAULT 4,
    GridHeight      INT            NOT NULL DEFAULT 2,
    RefreshInterval INT            NOT NULL DEFAULT 0,
    IsActive        BIT            NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT CK_DashboardCards_ChartType     CHECK (ChartType IN ('Bar','Line','Pie','KPI','Table','Gauge')),
    CONSTRAINT CK_DashboardCards_DataSourceType CHECK (DataSourceType IN ('SQL Query','View')),
    CONSTRAINT CK_DashboardCards_GridWidth     CHECK (GridWidth BETWEEN 1 AND 12),
    CONSTRAINT CK_DashboardCards_GridHeight    CHECK (GridHeight BETWEEN 1 AND 6),
    CONSTRAINT CK_DashboardCards_RefreshInterval CHECK (RefreshInterval >= 0)
);
CREATE INDEX IX_DashboardCards_IsActive ON dbo.DashboardCards(IsActive);
CREATE INDEX IX_DashboardCards_GridPositionX_GridPositionY ON dbo.DashboardCards(GridPositionX, GridPositionY);

CREATE TABLE dbo.CardDrillDownLevels (
    Id              INT            IDENTITY(1,1) PRIMARY KEY,
    ParentCardId    INT            NOT NULL,
    Level           INT            NOT NULL DEFAULT 1,
    DisplayName     NVARCHAR(200)  NOT NULL,
    DrillDownQuery  NVARCHAR(MAX)  NOT NULL,
    TargetChartType NVARCHAR(50)   NOT NULL,
    CONSTRAINT FK_CardDrillDownLevels_DashboardCards FOREIGN KEY (ParentCardId)
        REFERENCES dbo.DashboardCards(Id) ON DELETE CASCADE,
    CONSTRAINT CK_CardDrillDownLevels_Level CHECK (Level >= 1),
    CONSTRAINT CK_CardDrillDownLevels_TargetChartType CHECK (TargetChartType IN ('Bar','Line','Pie','KPI','Table','Gauge'))
);
CREATE UNIQUE INDEX IX_CardDrillDownLevels_ParentCardId_Level ON dbo.CardDrillDownLevels(ParentCardId, Level);

CREATE TABLE dbo.SyncSettings (
    Id                 INT       IDENTITY(1,1) PRIMARY KEY,
    IntervalMinutes    INT       NOT NULL DEFAULT 30,
    IsAutoSyncEnabled  BIT       NOT NULL DEFAULT 1,
    LastSyncTimestamp  DATETIME2 NULL,
    CONSTRAINT CK_SyncSettings_IntervalMinutes CHECK (IntervalMinutes >= 1)
);

CREATE TABLE dbo.AdminPassword (
    Id           INT       IDENTITY(1,1) PRIMARY KEY,
    PasswordHash NVARCHAR(500) NOT NULL,
    UpdatedAt    DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Sync Logs Tables
CREATE TABLE dbo.SyncLogs (
    Id          BIGINT     IDENTITY(1,1) PRIMARY KEY,
    StartTime   DATETIME2  NOT NULL,
    EndTime     DATETIME2  NULL,
    Status      NVARCHAR(20) NOT NULL DEFAULT 'Running',
    RecordCount INT        NOT NULL DEFAULT 0,
    Duration    BIGINT     NULL,
    TriggerType NVARCHAR(20) NOT NULL DEFAULT 'Auto',
    ErrorMessage NVARCHAR(MAX) NULL,
    CONSTRAINT CK_SyncLogs_Status CHECK (Status IN ('Running','Success','Failed')),
    CONSTRAINT CK_SyncLogs_TriggerType CHECK (TriggerType IN ('Auto','Manual')),
    CONSTRAINT CK_SyncLogs_RecordCount CHECK (RecordCount >= 0),
    CONSTRAINT CK_SyncLogs_Duration CHECK (Duration >= 0 OR Duration IS NULL),
    CONSTRAINT CK_SyncLogs_Dates CHECK (EndTime IS NULL OR EndTime >= StartTime)
);
CREATE INDEX IX_SyncLogs_StartTime ON dbo.SyncLogs(StartTime DESC);
CREATE INDEX IX_SyncLogs_Status ON dbo.SyncLogs(Status);
CREATE INDEX IX_SyncLogs_TriggerType_StartTime ON dbo.SyncLogs(TriggerType, StartTime DESC);

CREATE TABLE dbo.ErrorLogs (
    Id          BIGINT     IDENTITY(1,1) PRIMARY KEY,
    SyncLogId   BIGINT     NOT NULL,
    TableName   NVARCHAR(200) NOT NULL,
    ErrorMessage NVARCHAR(MAX) NOT NULL,
    Timestamp   DATETIME2  NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ErrorLogs_SyncLogs FOREIGN KEY (SyncLogId)
        REFERENCES dbo.SyncLogs(Id) ON DELETE CASCADE
);
CREATE INDEX IX_ErrorLogs_SyncLogId ON dbo.ErrorLogs(SyncLogId);
CREATE INDEX IX_ErrorLogs_Timestamp ON dbo.ErrorLogs(Timestamp DESC);
```

> **ملاحظة:** أقسام `AdminUsers` و `AuditLog` مقترحة ولم تُدرَج في DDL أعلاه حتى يُحسم التعارض (§9).

---

## 9. ملخص الفجوات (Design Gaps Summary)

| # | الفجوة | التفاصيل | التأثير | القرار المطلوب |
|:-:|--------|----------|:-------:|----------------|
| DG-1 | **`AdminUsers` vs `AdminPassword`** | المهمة طلبت `AdminUsers`، لكن الـ Baseline المعتمد يستخدم `AdminPassword` (Singleton) ويؤجّل Users/Roles إلى Phase 2 | متوسط — تعارض في نطاق التصميم المعتمد | هل نعتمد `AdminUsers` (مع إيقاف `AdminPassword`) أم نبقي `AdminPassword`؟ |
| DG-2 | **`AuditLog` غير موجود** | لا يوجد جدول `AuditLog` في أي وثيقة معتمدة؛ الأعمدة التدقيقية أُجّلت إلى Phase 2 | منخفض–متوسط | هل نضيف `AuditLog` في Phase 1 أم نؤجّله؟ |
| DG-3 | **`SyncSettings` غير مذكور في قائمة المهمة** | الجدول موجود في الـ Baseline لكنه لم يُذكر في طلب المهمة | منخفض | إبقاؤه ضمنياً (تم إدراجه في §1.3) — لا إجراء مطلوب |
| DG-4 | **هيكل Data Tables مؤجّل** | جداول Oracle الفعلية غير معروفة | متوسط | تُحدَّد أثناء التنفيذ عند توفر جداول العميل |

> **الحالة:** هذا المستند بوضع `Draft` ريثما يُحسم DG-1 و DG-2. باقي الأقسام (§1.1–1.6، §3–§8) مستمدة مباشرة من وثائق `Module Baseline Approved` وهي جاهزة للاعتماد.

---

> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-013
> **تاريخ:** 2026-07-12
> **المرجع:** `06_DATA_MODEL_PREPARATION.md` (MBA)، `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `dotnet-razorpages-adonet.md`
