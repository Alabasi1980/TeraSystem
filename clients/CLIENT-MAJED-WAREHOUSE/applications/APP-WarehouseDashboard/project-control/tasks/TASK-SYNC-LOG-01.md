# TASK-SYNC-LOG-01 — Sync Logs Backend: Entities + Migration + API

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-LOG-01 |
| **النوع** | Backend — EF Entities + DB Migration + API Changes |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## 1. الهدف

إنشاء جدولين في SQL Server لحفظ سجلات المزامنة بشكل دائم، وتعديل SyncEngineService و SyncController ليكتبوا ويقرأوا من قاعدة البيانات بدلاً من الذاكرة المؤقتة.

---

## 2. التصميم

### 2.1 Entity Models (في Web/Models/)

**SyncRun.cs:**
```csharp
public class SyncRun
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }      // بداية الدورة
    public DateTime? EndTime { get; set; }        // نهاية الدورة (null أثناء التشغيل)
    public string Status { get; set; }             // Running / Success / Partial / Failed / Cancelled
    public string TriggerType { get; set; }        // Manual / Auto / "Manual (selected)"
    public int? TotalRecordCount { get; set; }      // إجمالي الصفوف
    public double? TotalDurationSeconds { get; set; } // المدة بالثواني
    public DateTime CreatedAt { get; set; }
}
```

**SyncRunDetail.cs:**
```csharp
public class SyncRunDetail
{
    public int Id { get; set; }
    public int SyncRunId { get; set; }             // FK → SyncRun
    public int? TableMappingId { get; set; }        // FK → TableMappingConfig
    public string TargetTable { get; set; }         // اسم جدول SQL الهدف
    public string SyncMode { get; set; }            // Full / Incremental
    public string Status { get; set; }              // Success / Failed / Skipped
    public int RowsExtracted { get; set; }           // عدد الصفوف المستخرجة
    public int RowsLoaded { get; set; }              // عدد الصفوف المحملة
    public int Attempts { get; set; }               // عدد المحاولات
    public double? DurationSeconds { get; set; }     // مدة معالجة هذا الجدول
    public string? ErrorMessage { get; set; }        // نص الخطأ
    public DateTime CreatedAt { get; set; }

    // Navigation
    public SyncRun SyncRun { get; set; } = null!;
    public TableMappingConfig? TableMapping { get; set; }
}
```

### 2.2 DbContext إضافات

أضف إلى `WarehouseDashboardDbContext`:
```csharp
public DbSet<SyncRun> SyncRuns => Set<SyncRun>();
public DbSet<SyncRunDetail> SyncRunDetails => Set<SyncRunDetail>();
```

مع Fluent API configuration للجدولين (أنظر النمط الموجود في DbContext).

### 2.3 API Changes

**SyncEngineService.cs:**
- تعديل `RunSyncOnceAsync()` لكتابة `SyncRun` + `SyncRunDetail` في قاعدة البيانات بعد كل دورة.
- تعديل `RunSelectedMappingsAsync()` لكتابة `SyncRun` + `SyncRunDetail` لكل mapping.
- استخدم ADO.NET (SqlConnection) للكتابة — لا تستخدم EF في API project.
- لا تحذف `SyncRunLogStore` — يمكن بقاؤها احتياطاً أو إزالتها لاحقاً.

**SyncController.cs:**
- تعديل `GET /api/sync/logs` لقراءة السجلات من قاعدة البيانات (جدول SyncRuns مع SyncRunDetails) وليس من الذاكرة.
- إضافة `GET /api/sync/logs/detail?runId=...` endpoint لقراءة تفاصيل دورة معينة.

### 2.4 SQL Schema

```sql
CREATE TABLE SyncRuns (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    Status NVARCHAR(50) NOT NULL,
    TriggerType NVARCHAR(50) NOT NULL,
    TotalRecordCount INT NULL,
    TotalDurationSeconds FLOAT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE SyncRunDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SyncRunId INT NOT NULL REFERENCES SyncRuns(Id) ON DELETE CASCADE,
    TableMappingId INT NULL REFERENCES TableMappingConfigs(Id),
    TargetTable NVARCHAR(200) NOT NULL,
    SyncMode NVARCHAR(20) NOT NULL DEFAULT 'Full',
    Status NVARCHAR(50) NOT NULL,
    RowsExtracted INT NOT NULL DEFAULT 0,
    RowsLoaded INT NOT NULL DEFAULT 0,
    Attempts INT NOT NULL DEFAULT 0,
    DurationSeconds FLOAT NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_SyncRuns_StartTime ON SyncRuns(StartTime DESC);
CREATE INDEX IX_SyncRunDetails_SyncRunId ON SyncRunDetails(SyncRunId);
```

---

## 3. Allowed Sources

- `WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs`
- `WarehouseDashboard.Web/Models/*.cs`
- `WarehouseDashboard.Api/Services/SyncEngineService.cs`
- `WarehouseDashboard.Api/Controllers/SyncController.cs`
- `WarehouseDashboard.Api/Models/SyncModels.cs`

---

## 4. Allowed Write Targets

- `WarehouseDashboard.Web/Models/SyncRun.cs` (جديد)
- `WarehouseDashboard.Web/Models/SyncRunDetail.cs` (جديد)
- `WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs`
- `WarehouseDashboard.Api/Services/SyncEngineService.cs`
- `WarehouseDashboard.Api/Controllers/SyncController.cs`
- `WarehouseDashboard.Api/Models/SyncModels.cs` (اختياري — إضافة DTOs)

المسارات كاملة:
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\SyncRun.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\SyncRunDetail.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Controllers\SyncController.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Models\SyncModels.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\Migrations\` (للميغريشن)

---

## 5. Acceptance Criteria

| # | المعيار |
|---|---|
| AC-1 | `SyncRun` entity موجود مع كل الحقول المطلوبة |
| AC-2 | `SyncRunDetail` entity موجود مع كل الحقول المطلوبة |
| AC-3 | DbContext معرف للجدولين مع Fluent API |
| AC-4 | `dotnet ef migrations add` ينتج migration سليم |
| AC-5 | `SyncEngineService.RunSyncOnceAsync()` يكتب SyncRun + SyncRunDetail لكل mapping |
| AC-6 | `SyncEngineService.RunSelectedMappingsAsync()` يكتب SyncRun + SyncRunDetail |
| AC-7 | `GET /api/sync/logs` يقرأ من قاعدة البيانات وليس من الذاكرة |
| AC-8 | `GET /api/sync/logs/detail?runId=...` يرجع تفاصيل دورة معينة |
| AC-9 | `dotnet build` ينجح للمشروعين |

---

## 6. التنفيذ

1. اقرأ الملفات الموجودة من القرص قبل التعديل.
2. أنشئ ملفي النماذج (SyncRun.cs, SyncRunDetail.cs).
3. أضف DbSet و Fluent API إلى DbContext.
4. شغّل `dotnet ef migrations add AddSyncRunLogs` في مجلد Web.
5. عدّل SyncEngineService ليكتب إلى DB في نهاية كل دورة (واستخدم ADO.NET SqlConnection + SqlCommand للإدراج).
6. عدّل SyncController.ReadLogs endpoint ليقرأ من DB.
7. أضف DTOs لـ `/api/sync/logs/detail` إذا احتجت.
8. ابنِ المشروعين وتأكد من `dotnet build` ينجح.
9. إذا `dotnet ef` مش موجود، استخدم `dotnet tool install --global dotnet-ef` أولاً.

---

## 7. Handback

| البند | القيمة |
|---|---|
| **الحالة** | Accepted (جزئي — entity + DbContext فقط) |
| **التاريخ** | 2026-07-20 |
| **استكمل بـ** | TASK-SYNC-LOG-01-FIX (API changes) |

### تم إنشاؤه

- `SyncRun.cs` — entity مع Id, StartTime, EndTime, Status, TriggerType, TotalRecordCount, TotalDurationSeconds, CreatedAt
- `SyncRunDetail.cs` — entity مع Id, SyncRunId, TableMappingId, TargetTable, SyncMode, Status, RowsExtracted, RowsLoaded, Attempts, DurationSeconds, ErrorMessage, CreatedAt
- `WarehouseDashboardDbContext.cs` — DbSet + Fluent API مع العلاقات
- `20260720083000_AddSyncRunLogs.cs` — manual migration

### لم يكتمل (نُقل إلى FIX)

- SyncEngineService DB writing
- SyncController DB reading
- `/api/sync/logs/{runId}` endpoint
