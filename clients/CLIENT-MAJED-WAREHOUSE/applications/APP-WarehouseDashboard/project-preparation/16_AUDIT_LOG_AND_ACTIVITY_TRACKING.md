# 16_AUDIT_LOG_AND_ACTIVITY_TRACKING.md — WarehouseDashboard

> **النوع:** Audit & Activity Tracking Document — تتبع عمليات المزامنة والأخطاء وسجل التدقيق
> **الحالة:** `Module Baseline Approved`
> **Baseline Module:** WarehouseDashboard
> **تاريخ الاعتماد:** 2026-07-12
> **الجهة المعتمدة:** Software Designer Agent (مُصمم) — بالاستناد إلى `06_DATA_MODEL_PREPARATION.md` (MBA)، `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `19_DATABASE_DESIGN.md`
> **الملفات المرجعية:** `06_DATA_MODEL_PREPARATION.md` (§2.1–2.2)، `08_TECHNICAL_ARCHITECTURE.md` (§5, §6.7, §12)، `19_DATABASE_DESIGN.md` (§1.5–1.6)
> **إعداد:** Software Designer Agent (مُصمم) — بتكليف من TeraAgent

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Module Baseline Approved` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | N/A — ثابت طوال Phase 1 |
| **Document Type** | Audit & Activity Tracking |
| **Version** | 1.0.0 |

---

## نبذة عن الوثيقة

توثق هذه الوثيقة **نظام تتبع الأنشطة والتدقيق** (Audit Log & Activity Tracking) لتطبيق **WarehouseDashboard** — وهي تغطي ثلاثة نطاقات متميزة:

| النطاق | الحالة | المسؤول |
|--------|:------:|---------|
| **1. سجلات المزامنة (SyncLogs)** | ✅ **Phase 1 — نشط** | SyncEngineService يُسجّل كل عملية مزامنة |
| **2. سجلات الأخطاء (ErrorLogs)** | ✅ **Phase 1 — نشط** | SyncEngineService يُسجّل الأخطاء التفصيلية |
| **3. سجل تدقيق الإدارة (AuditLog)** | ⏳ **Phase 2 — مقترح** | يُؤجّل إلى Phase 2 بقرار D-BE-2 |

> **قرارات التصميم المعتمدة (نهائية — لا تُعتبر فجوات):**
> - **D-BE-2:** جدول `AuditLog` مؤجّل إلى Phase 2. Phase 1 يكتفي بـ `SyncLogs` و `ErrorLogs`.
> - **D-BE-1:** `AdminPassword` (Singleton) يُستخدم في Phase 1; `AdminUsers` مؤجّل إلى Phase 2.

تعتمد هذه الوثيقة على `06_DATA_MODEL_PREPARATION.md` (MBA) و `19_DATABASE_DESIGN.md` كهياكل مرجعية، وتُقدّمها من منظور **التدقيق والمراقبة** مع إضافات خاصة بالتتبع (ErrorType، RowData، SourceTable).

---

## 1. سجلات المزامنة — SyncLogs

### 1.1 الغرض

تسجل **SyncLogs** كل عملية مزامنة (Sync) تُنفّذها `SyncEngineService` — سواء كانت تلقائية (Auto) أو يدوية (Manual). يمثل كل سجل دورة كاملة من المزامنة تشمل جدولاً واحداً أو أكثر حسب `TableMapping`.

### 1.2 هيكل الجدول (من منظور التدقيق)

| العمود | النوع في SQL Server | المفتاح | إلزامي | الافتراضي | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:---------:|-------------------|
| `SyncId` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment. معرف فريد لكل دورة مزامنة |
| `StartTime` | `datetime2` | — | ✅ | — | وقت بدء دورة المزامنة |
| `EndTime` | `datetime2` | — | ❌ | `NULL` | وقت انتهاء المزامنة. `NULL` = المزامنة لا تزال قيد التشغيل |
| `Status` | `nvarchar(50)` | — | ✅ | `'Running'` | القيم المسموحة: `'Running'`, `'Success'`, `'Failed'` |
| `RecordCount` | `int` | — | ✅ | `0` | إجمالي عدد الصفوف المنقولة في هذه الدورة |
| `SourceTable` | `nvarchar(200)` | — | ❌ | `NULL` | اسم الجدول المصدر في Oracle لهذه الدورة (إذا كانت الدورة لجدول واحد) |
| `ErrorMessage` | `nvarchar(max)` | — | ❌ | `NULL` | رسالة الخطأ العامة إذا فشلت المزامنة |
| `CreatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت إنشاء السجل (مرادف لـ StartTime من ناحية القيمة ولكن يُستخدم كـ audit timestamp) |

**CHECK Constraints:**
- `CK_SyncLogs_Audit_Status`: `Status IN ('Running', 'Success', 'Failed')`
- `CK_SyncLogs_Audit_RecordCount`: `RecordCount >= 0`
- `CK_SyncLogs_Audit_Dates`: `EndTime IS NULL OR EndTime >= StartTime`

**Indexes:**
- `IX_SyncLogs_Audit_StartTime` — `StartTime DESC` (غير متفرد) — لعرض أحدث السجلات أولاً في صفحة Sync Logs
- `IX_SyncLogs_Audit_Status` — `Status` (غير متفرد) — لتصفية السجلات حسب الحالة (Success/Failed/Running)
- `IX_SyncLogs_Audit_SourceTable` — `SourceTable` (غير متفرد) — للبحث عن سجلات مزامنة لجدول معين

### 1.3 العلاقة مع جدول SyncLogs في 06_DATA_MODEL_PREPARATION.md

> **ملاحظة توافق:** الهيكل أعلاه يُعرض من منظور التدقيق والمراقبة. الجدول الفعلي في قاعدة البيانات (`dbo.SyncLogs`) مُعرَّف بشكل كامل في `06_DATA_MODEL_PREPARATION.md` (§2.1) وقد يحتوي على أعمدة إضافية (مثل `Duration`, `TriggerType`). الحقول المذكورة هنا هي **المجموعة الأساسية** المطلوبة لصفحة عرض سجلات المزامنة وتحليل النشاط.

### 1.4 حالات Status ومعانيها

| الحالة | المعنى | الإجراء المطلوب |
|--------|--------|----------------|
| `Running` | المزامنة قيد التشغيل حاليًا | انتظار — لا إجراء |
| `Success` | اكتملت المزامنة بنجاح | لا إجراء |
| `Failed` | فشلت المزامنة — راجع `ErrorMessage` و `ErrorLogs` | تحقيق — راجع الأخطاء التفصيلية |

---

## 2. سجلات الأخطاء — ErrorLogs

### 2.1 الغرض

تسجل **ErrorLogs** الأخطاء التفصيلية التي تحدث أثناء عملية المزامنة — لكل جدول فشلت مزامنته يتم تسجيل سجل خطأ منفصل، مما يسمح بتحديد مصدر المشكلة بدقة.

### 2.2 هيكل الجدول (من منظور التدقيق)

| العمود | النوع في SQL Server | المفتاح | إلزامي | الافتراضي | القيود والملاحظات |
|--------|--------------------|:-------:|:------:|:---------:|-------------------|
| `ErrorId` | `int` | PK, Identity(1,1) | ✅ | — | Auto-increment. معرف فريد لكل خطأ |
| `SyncId` | `int` | FK → SyncLogs.SyncId | ✅ | — | معرف دورة المزامعة التي حدث خلالها الخطأ. ON DELETE CASCADE |
| `TableName` | `nvarchar(200)` | — | ✅ | — | اسم الجدول (في SQL Server) الذي فشلت مزامنته |
| `ErrorMessage` | `nvarchar(max)` | — | ✅ | — | رسالة الخطأ التفصيلية من قاعدة البيانات أو التطبيق |
| `ErrorType` | `nvarchar(100)` | — | ❌ | `'General'` | تصنيف الخطأ: `'Connection'`, `'Timeout'`, `'DataMapping'`, `'Constraint'`, `'General'`, `'Oracle'`, `'SqlBulkCopy'` |
| `RowData` | `nvarchar(max)` | — | ❌ | `NULL` | JSON يمثل بيانات الصف الذي تسبب بالخطأ (إن أمكن التقاطه). مفيد لتحليل أخطاء تحويل البيانات |
| `CreatedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت تسجيل الخطأ |

**CHECK Constraints:**
- `CK_ErrorLogs_Audit_ErrorType`: `ErrorType IN ('Connection', 'Timeout', 'DataMapping', 'Constraint', 'General', 'Oracle', 'SqlBulkCopy')`

**Indexes:**
- `IX_ErrorLogs_Audit_SyncId` — `SyncId` (غير متفرد) — لتسريع ربط الأخطاء بسجل المزامنة
- `IX_ErrorLogs_Audit_CreatedAt` — `CreatedAt DESC` (غير متفرد) — لعرض أحدث الأخطاء أولاً
- `IX_ErrorLogs_Audit_ErrorType` — `ErrorType` (غير متفرد) — لتصفية الأخطاء حسب النوع

**Foreign Keys:**
- `FK_ErrorLogs_Audit_SyncLogs` — `SyncId` ← `SyncLogs.SyncId` — **ON DELETE CASCADE**

> **سبب ON DELETE CASCADE:** عند تنظيف سجلات المزامنة القديمة (حسب سياسة الاحتفاظ)، تُحذف الأخطاء المرتبطة تلقائياً.

### 2.3 تصنيف ErrorType

| ErrorType | أمثلة | مصدره |
|-----------|-------|-------|
| `Connection` | فشل الاتصال بـ Oracle أو SQL Server | SqlException, OracleException |
| `Timeout` | تجاوز timeout أثناء الاستعلام أو BulkCopy | TimeoutException |
| `DataMapping` | تعارض أنواع البيانات بين Oracle و SQL Server | InvalidCastException |
| `Constraint` | انتهاك CONSTRAINT (PK, FK, CHECK) في SQL Server | SqlException (2627, 547) |
| `Oracle` | خطأ من Oracle (استعلام خاطئ، جدول غير موجود) | OracleException |
| `SqlBulkCopy` | خطأ أثناء SqlBulkCopy.WriteToServer | SqlException |
| `General` | أي خطأ آخر غير مصنف | Exception |

### 2.4 العلاقة مع جدول ErrorLogs في 06_DATA_MODEL_PREPARATION.md

> **ملاحظة توافق:** الهيكل أعلاه يُعرض من منظور التدقيق والتحليل. الجدول الفعلي في قاعدة البيانات (`dbo.ErrorLogs`) مُعرَّف بشكل كامل في `06_DATA_MODEL_PREPARATION.md` (§2.2). تمت إضافة `ErrorType` و `RowData` كحقول تدقيق إضافية لتسهيل تحليل الأخطاء وتصنيفها في واجهة Sync Logs.

---

## 3. العلاقة بين SyncLogs و ErrorLogs

```
┌──────────────────────────────────────────────────────────┐
│                    دورة المزامنة                           │
│                                                          │
│  ┌──────────────────────────────────────────────┐        │
│  │              SyncLogs                         │        │
│  │──────────────────────────────────────────────│        │
│  │ SyncId    = 1047                             │        │
│  │ Status    = 'Failed'                         │        │
│  │ StartTime = 2026-07-12 14:30:00              │        │
│  │ EndTime   = 2026-07-12 14:30:45              │        │
│  │ SourceTable = 'WMS.WAREHOUSE_STOCK'          │        │
│  │ RecordCount = 15000                          │        │
│  │ ErrorMessage = 'فشلت مزامنة الجدول ...'       │        │
│  └──────────────────┬───────────────────────────┘        │
│                     │ 1                                  │
│                     │                                    │
│                     │ N (ON DELETE CASCADE)              │
│                     ▼                                    │
│  ┌──────────────────────────────────────────────┐        │
│  │              ErrorLogs                        │        │
│  │──────────────────────────────────────────────│        │
│  │ ErrorId   = 1 │ SyncId = 1047               │        │
│  │ TableName = 'stg_WarehouseStock'             │        │
│  │ ErrorType = 'SqlBulkCopy'                    │        │
│  │ ErrorMessage = 'Violation of PK constraint'  │        │
│  │ RowData   = { "WarehouseId": 9999, ... }     │        │
│  │──────────────────────────────────────────────│        │
│  │ ErrorId   = 2 │ SyncId = 1047               │        │
│  │ TableName = 'stg_ProductCatalog'             │        │
│  │ ErrorType = 'Connection'                     │        │
│  │ ErrorMessage = 'Network timeout'             │        │
│  │ RowData   = NULL                             │        │
│  └──────────────────────────────────────────────┘        │
└──────────────────────────────────────────────────────────┘
```

---

## 4. سجل تدقيق الإدارة — AuditLog (Phase 2 — مقترح)

### 4.1 النطاق

بناءً على القرار المعماري **D-BE-2**، تم تأجيل جدول `AuditLog` إلى **Phase 2**. القسم التالي يُوثّق التصميم المقترح كمرجع للمستقبل فقط — **ليس جزءاً من نطاق Phase 1**.

### 4.2 المبرر

في Phase 1:
- المستخدم الوحيد هو Admin (مستخدم واحد مشترك عبر `AdminPassword` Singleton).
- لا توجد عملية تدقيق مطلوبة على تغييرات Config Tables لأن Admin هو المسؤول الوحيد.
- سجلات SyncLogs + ErrorLogs تغطي جميع الأنشطة المتعلقة بالبيانات.

في Phase 2 (عند إضافة RBAC و AdminUsers):
- تصبح الحاجة إلى AuditLog ضرورية لتتبع: من غيّر ماذا ومتى.
- يُصبح الجدول أداة تدقيق أساسية للامتثال والمساءلة.

### 4.3 هيكل AuditLog المقترح (Phase 2)

| العمود | النوع | المفتاح | إلزامي | الافتراضي | القيود والملاحظات |
|--------|-------|:-------:|:------:|:---------:|-------------------|
| `AuditId` | `bigint` | PK, Identity(1,1) | ✅ | — | Auto-increment. `bigint` لاستيعاب آلاف السجلات |
| `TableName` | `nvarchar(200)` | — | ✅ | — | اسم الجدول الذي تم تغييره (مثل `DashboardCards`, `SyncSettings`, `AdminPassword`) |
| `RecordId` | `nvarchar(100)` | — | ❌ | `NULL` | معرف السجل الذي تم تغييره (قيمة PK كـ string) |
| `Action` | `nvarchar(20)` | — | ✅ | — | `'Insert'`, `'Update'`, `'Delete'` — CHECK constraint |
| `ChangedBy` | `nvarchar(100)` | — | ❌ | `'Admin'` | المستخدم الذي أجرى التغيير (في Phase 1 = Admin, في Phase 2 = اسم المستخدم) |
| `ChangedAt` | `datetime2` | — | ✅ | `GETUTCDATE()` | وقت التغيير |
| `OldValues` | `nvarchar(max)` | — | ❌ | `NULL` | JSON للقيم قبل التغيير (لـ Update/Delete). `NULL` لـ Insert |
| `NewValues` | `nvarchar(max)` | — | ❌ | `NULL` | JSON للقيم بعد التغيير (لـ Insert/Update). `NULL` لـ Delete |

**CHECK Constraints:**
- `CK_AuditLog_Action`: `Action IN ('Insert', 'Update', 'Delete')`

**Indexes:**
- `IX_AuditLog_ChangedAt` — `ChangedAt DESC` (غير متفرد) — لعرض أحدث التغييرات
- `IX_AuditLog_TableName_ChangedAt` — `(TableName, ChangedAt DESC)` (غير متفرد) — لاستعلامات التدقيق حسب الجدول
- `IX_AuditLog_ChangedBy` — `ChangedBy` (غير متفرد) — للبحث حسب المستخدم (مفيد في Phase 2 مع RBAC)

### 4.4 الأحداث المقترح تسجيلها (عند التفعيل في Phase 2)

| الحدث | الجدول | Action | OldValues | NewValues |
|-------|--------|:------:|:---------:|:---------:|
| إنشاء بطاقة جديدة | `DashboardCards` | Insert | NULL | JSON لجميع الحقول |
| تعديل استعلام بطاقة | `DashboardCards` | Update | JSON للقيم القديمة | JSON للقيم الجديدة |
| حذف بطاقة | `DashboardCards` | Delete | JSON للقيم قبل الحذف | NULL |
| تغيير كلمة مرور Admin | `AdminPassword` | Update | JSON (يتضمن `UpdatedAt` فقط، وليس `PasswordHash`) | JSON (يتضمن `UpdatedAt` فقط) |
| تعديل إعدادات المزامنة | `SyncSettings` | Update | JSON للقيم القديمة | JSON للقيم الجديدة |
| إضافة/إزالة مستخدم | `AdminUsers` | Insert/Delete | NULL / JSON | JSON / NULL |

> **ملاحظة:** لا يتم تسجيل `PasswordHash` في `OldValues` أو `NewValues` لأسباب أمنية — يُسجّل `UpdatedAt` فقط لتأكيد حدوث التغيير.

### 4.5 شروط التفعيل

1. بدء Phase 2 من المشروع
2. إضافة نظام RBAC (AdminUsers)
3. وجود أكثر من مستخدم (Admin + Viewer)
4. تعديل `AdminAuthService` و `CardConfigService` و `SyncSettingsService` لإضافة AuditLog عند كل عملية كتابة

---

## 5. سياسة الاحتفاظ (Retention Policy)

### 5.1 المدة

| الجدول | فترة الاحتفاظ | الأساس | الإجراء |
|--------|:-------------:|--------|---------|
| **SyncLogs** | **90 يوم** | تاريخ `StartTime` | يُحذف تلقائياً عبر Cleanup Job |
| **ErrorLogs** | **180 يوم** | تاريخ `CreatedAt` | يُحذف تلقائياً عبر Cleanup Job (فترة أطول للأخطاء لأغراض التحقيق) |
| **AuditLog** (Phase 2) | **365 يوم** | تاريخ `ChangedAt` | يُحذف تلقائياً عبر Cleanup Job |

### 5.2 آلية التنظيف (Cleanup Job)

يتم تشغيل عملية التنظيف عبر **SyncEngineService** في `WarehouseDashboard.Api`:

```
Cleanup Workflow (يُشغّل مرة يومياً):
┌──────────────────────────────────────────────────────┐
│  1. فتح SqlConnection إلى SQL Server                  │
│  2. DELETE FROM SyncLogs                              │
│     WHERE StartTime < DATEADD(day, -90, GETUTCDATE()) │
│  3. DELETE FROM ErrorLogs                             │
│     WHERE CreatedAt < DATEADD(day, -180, GETUTCDATE())│
│  4. تسجيل في Console Log: "Cleaned X sync logs,       │
│     Y error logs older than retention period"         │
│  5. Close connection                                  │
└──────────────────────────────────────────────────────┘
```

**ملاحظات التنظيف:**
- عملية الحذف تستفيد من `ON DELETE CASCADE` (عند حذف SyncLog قديم، تُحذف ErrorLogs المرتبطة به تلقائياً).
- مع ذلك، فترة الاحتفاظ لـ ErrorLogs أطول (180 يوماً) من SyncLogs (90 يوماً)، لذا يجب الحذف المباشر من `ErrorLogs` أولاً لضمان بقاء الأخطاء القديمة في حال حُذف الـ SyncLogs الأقدم من 90 يوماً.
- **الترتيب الصحيح:** احذف من `ErrorLogs` أولاً (حسب `CreatedAt < -180`)، ثم احذف من `SyncLogs` (حسب `StartTime < -90`).

### 5.3 إعدادات التنظيف في appsettings.json (مقترحة)

```json
{
  "RetentionPolicy": {
    "SyncLogsDays": 90,
    "ErrorLogsDays": 180,
    "CleanupHourUTC": 2,
    "IsCleanupEnabled": true
  }
}
```

### 5.4 تحذير قبل الحذف

لتفادي فقدان سجلات مهمة، يُنصح بإضافة رسالة تحذير في Console Log عند كل عملية تنظيف:

```
[Cleanup] Starting cleanup of sync logs older than 2026-04-13...
[Cleanup] Deleted 1,247 sync logs
[Cleanup] Deleted 3,892 error logs
[Cleanup] Cleanup completed successfully (1.2s)
```

---

## 6. أسلوب العرض — Display Approach

### 6.1 صفحة Sync Logs

يتم عرض سجلات المزامنة في صفحة Razor Pages على المسار `/SyncLogs/Index` (كما هو مُحدّد في `08_TECHNICAL_ARCHITECTURE.md` §6.2).

**العناصر المعروضة:**

| العنصر | المصدر | الوصف |
|--------|--------|-------|
| جدول السجلات | `SyncLogs` | جدول يعرض السجلات مرتبة من الأحدث إلى الأقدم |
| Status Badge | `Status` | شارة ملونة تعبر عن حالة المزامنة |
| زر التحديث اليدوي | POST `/api/sync/trigger` | تشغيل مزامنة يدوية |
| مؤشر الحالة الحالية | GET `/api/sync/status` | شريط يظهر في أعلى الصفحة |
| رابط عرض الأخطاء | `ErrorLogs` (حسب `SyncId`) | زر "عرض الأخطاء" لكل سجل فاشل |

### 6.2 Status Badges — الألوان

| الحالة | اللون | الخلفية | النص | مثال |
|--------|:-----:|:-------:|:----:|------|
| **Success** | 🟢 أخضر | `#28a745` | أبيض | `[ Success ]` |
| **Failed** | 🔴 أحمر | `#dc3545` | أبيض | `[ Failed ]` |
| **Running** | 🔵 أزرق | `#007bff` | أبيض | `[ Running ]` |

### 6.3 تخطيط الصفحة (مبدئي)

```
┌──────────────────────────────────────────────────────────────┐
│  Ware house Dashboard  [🏠]  [📊]  [📋 Sync Logs]  [🔧]    │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ⏱ آخر مزامنة: 12 يوليو 2026, 02:30 م  [🔄 تحديث الآن]      │
│  الحالة: ✅ Success — 15,432 سجل                               │
│                                                              │
│  ┌─────┬──────────────────┬──────────┬────────┬──────────┐   │
│  │  #  │ الوقت             │ الحالة   │ السجلات│ الجدول    │   │
│  ├─────┼──────────────────┼──────────┼────────┼──────────┤   │
│  │1047 │02:30 م 12-07-2026│ 🟢 نجاح  │ 15,432 │ Warehouse│   │
│  │1046 │02:00 م 12-07-2026│ 🔴 فشل   │  8,200 │ Products │   │
│  │     │                  │          │        │ [⚠ أخطاء]│   │
│  │1045 │01:30 م 12-07-2026│ 🟢 نجاح  │ 12,100 │ Stock    │   │
│  │1044 │01:00 م 12-07-2026│ 🔵 جارٍ  │      — │ —        │   │
│  └─────┴──────────────────┴──────────┴────────┴──────────┘   │
│                                                              │
│  [1] [2] [3] ... [10]  —  عرض 50 من 1,247 سجلاً              │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### 6.4 نافذة عرض الأخطاء التفصيلية (للحالة Failed)

عند النقر على `[⚠ أخطاء]` لسجل فاشل، تظهر نافذة منبثقة (Modal) تعرض:

```
┌────────────────────────────────────────────────────────┐
│  📋 أخطاء المزامنة #1046                                │
├────────────────────────────────────────────────────────┤
│                                                        │
│  ┌──────────┬────────────┬──────────────────────┐      │
│  │ الجدول   │ النوع       │ رسالة الخطأ          │      │
│  ├──────────┼────────────┼──────────────────────┤      │
│  │stg_Prods│ SqlBulkCopy│ Violation of PK      │      │
│  │          │            │ constraint 'PK_stg_..│      │
│  │          │            │ Duplicate key: 9999  │      │
│  │          │            │ [🔄 إعادة المحاولة]  │      │
│  └──────────┴────────────┴──────────────────────┘      │
│                                                        │
│  [📄 عرض RowData JSON]  (إن وُجد)                      │
│                                                        │
└────────────────────────────────────────────────────────┘
```

### 6.5 State Management

| الحالة | السلوك في واجهة Sync Logs |
|--------|--------------------------|
| **Loading** | عرض Spinner أثناء تحميل السجلات |
| **Empty** | رسالة "لا توجد سجلات مزامنة بعد" مع زر "تشغيل أول مزامنة" |
| **Error** | رسالة "تعذر تحميل سجلات المزامنة" مع زر إعادة المحاولة |
| **Success** | عرض جدول السجلات بشكل طبيعي |
| **Sync Running** | تحديث تلقائي للصفحة كل 5 ثوانٍ (Auto-refresh) حتى تنتهي المزامنة |
| **No Results** | رسالة "لا توجد نتائج تطابق معايير البحث" (في حالة التصفية/البحث) |

---

## 7. الأمان — Security

### 7.1 صلاحيات الوصول في Phase 1

| الصفحة/البيانات | الوصول | الآلية |
|-----------------|:------:|--------|
| **صفحة Sync Logs** (`/SyncLogs/Index`) | Admin فقط (محمية عبر `AdminAuthMiddleware`) | Session-based — نفس حماية Admin Panel |
| **API `/api/sync/status`** | أي تطبيق على السيرفر المحلي | لا Auth — داخلي |
| **API `/api/sync/logs`** | أي تطبيق على السيرفر المحلي | لا Auth — داخلي |
| **API `/api/sync/trigger`** | Admin فقط (يتطلب Session) | يتحقق من Cookie/Session قبل التنفيذ |
| **قراءة SyncLogs/ErrorLogs من DB** | تطبيق Api فقط | SqlConnection في Api Service |

### 7.2 مبدأ Read-Only لغير Admin

في Phase 1 مع Admin وحيد، لا توجد حاجة لفصل الصلاحيات. عند إضافة Viewer Role في Phase 2:

| الدور | صلاحية Sync Logs | صلاحية عرض الأخطاء |
|-------|:-----------------:|:------------------:|
| **Admin** | ✅ قراءة + حذف يدوي | ✅ قراءة |
| **Viewer** | ✅ قراءة فقط (بدون حذف) | ✅ قراءة فقط |
| **غير مسجّل** | ❌ ممنوع | ❌ ممنوع |

> **ملاحظة:** تنفيذ صلاحيات Viewer مؤجّل إلى Phase 2 مع RBAC (القرار D-BE-1).

### 7.3 حماية AuditLog (عند التفعيل في Phase 2)

- **الكتابة:** فقط من Services (CardConfigService, SyncSettingsService, AdminAuthService) — لا كتابة مباشرة من UI.
- **القراءة:** Admin فقط — لا يُسمح لـ Viewer بالاطلاع على AuditLog.
- **التعديل/الحذف:** ممنوع — AuditLog هو سجل للقراءة فقط بعد الكتابة (Append-only).

---

## 8. قائمة الامتثال (Compliance Checklist)

| البند | الحالة | المرجع |
|-------|:------:|--------|
| جدول SyncLogs مُوثَّق | ✅ | §1.2 |
| جدول ErrorLogs مُوثَّق | ✅ | §2.2 |
| AuditLog مُوثَّق كمقترح لـ Phase 2 | ✅ | §4 (قرار D-BE-2) |
| سياسة الاحتفاظ مُحدَّدة | ✅ | §5 (SyncLogs=90d, ErrorLogs=180d) |
| آلية التنظيف مُوثَّقة | ✅ | §5.2–5.3 |
| واجهة عرض Sync Logs مُحدَّدة | ✅ | §6 |
| State Management للصفحة مُحدَّد | ✅ | §6.5 |
| صلاحيات الوصول مُحدَّدة | ✅ | §7 |
| ألوان Status Badges مُحدَّدة | ✅ | §6.2 (Success=🟢, Failed=🔴, Running=🔵) |
| Lifecycle Header معتمد | ✅ | `Module Baseline Approved` |
| لا تعارض مع D-BE-1 | ✅ | مؤكّد (AdminPassword لـ Phase 1) |
| لا تعارض مع D-BE-2 | ✅ | مؤكّد (AuditLog مؤجّل لـ Phase 2) |

---

## 9. الفجوات — Design Gaps

| # | الفجوة | التفاصيل | التأثير | الحالة |
|:-:|--------|----------|:-------:|:------:|
| DG-1 | **حقول `SourceTable` في SyncLogs** | الحقل `SourceTable` مضافة لسياق التدقيق — الجدول الحالي في `06_DATA_MODEL_PREPARATION.md` لا يحتويه | منخفض — حقل إضافي غير مطلوب لـ Sync Engine الأساسي | يُضاف في أول الترحيل (Migration) إذا لزم الأمر |
| DG-2 | **حقول `ErrorType` و `RowData` في ErrorLogs** | حقول إضافية لتصنيف الأخطاء وتحليلها — غير موجودة في `06_DATA_MODEL_PREPARATION.md` | منخفض — تحسين لتحليل الأخطاء وليس ضرورياً للمزامنة | يُضاف في أول الترحيل إذا لزم الأمر |
| DG-3 | **ربط AuditLog بـ AdminUsers** | `ChangedBy` في AuditLog يرتبط بنظام المستخدمين — غير موجود في Phase 1 | متوسط — يتطلب إما قيمة ثابتة `'Admin'` أو تأجيل كامل إلى Phase 2 | مؤجّل بموجب D-BE-2 و D-BE-1 |
| DG-4 | **واجهة مستخدم AuditLog** | لا توجد واجهة مخططة لعرض AuditLog في Phase 1 | منخفض — الجدول غير موجود أصلاً في Phase 1 | مؤجّل إلى Phase 2 |
| DG-5 | **آلية التنظيف اليدوي** | لا توجد واجهة Admin لتنظيف السجلات يدوياً أو تعديل فترة الاحتفاظ | منخفض — يمكن إضافتها في Phase 2 | إضافة اختيارية في Phase 2 |

---

## 10. Task Engineering Review Decision

| البند | القيمة |
|-------|--------|
| **Decision** | `APPROVED_FOR_GATE` |
| **التقييم** | ✅ الوثيقة جاهزة — جميع قرارات التصميم معتمدة (D-BE-1, D-BE-2) |
| **الملاحظات** | لا توجد عوائق تمنع بدء Pre-Execution Gate. وثيقة متوافقة مع `06_DATA_MODEL_PREPARATION.md` (MBA) و `08_TECHNICAL_ARCHITECTURE.md` (MBA) |
| **المراجعون المقترحون** | TeraAgent (تسلسل المهام) + Software Designer Agent (مُصمم) — للتحقق من الاتساق مع الـ Baseline |

---

> **إعداد:** Software Designer Agent (مُصمم) — بتكليف من TeraAgent
> **تاريخ:** 2026-07-12
> **الحالة:** `Module Baseline Approved` ✅
> **الملفات المرجعية:** `06_DATA_MODEL_PREPARATION.md` (MBA)، `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `19_DATABASE_DESIGN.md`
> **القرارات المعتمدة:** D-BE-1 (AdminPassword لـ Phase 1)، D-BE-2 (AuditLog مؤجّل لـ Phase 2)
