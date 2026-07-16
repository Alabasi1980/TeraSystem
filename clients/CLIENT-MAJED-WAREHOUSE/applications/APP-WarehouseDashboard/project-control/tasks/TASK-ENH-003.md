# TASK-ENH-003 — شريط تقدم حي (Live Progress API)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ENH-003 |
| **المجموعة** | ENH (Sync Enhancement P0) |
| **النوع** | Backend API |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | 🟡 Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

حالياً لا توجد أي تغذية راجعة أثناء المزامنة. المستخدم يضغط على زر المزامنة ولا يعرف كم تقدمت العملية ولا كم تبقى من الوقت ولا أي جدول تتم مزامنته الآن.

---

## 2. الهدف

إضافة نظام تتبع التقدم (Progress Tracking) يمكن UI من عرض شريط تقدم حي (progress bar) لكل تعيين يتم مزامنته.

---

## 3. النطاق

### المطلوب

- [x] **إنشاء `SyncRunProgressStore.cs`** في مجلد Services في API project:
  - مخزن في الذاكرة (ConcurrentDictionary<Guid, SyncRunProgress>)
  - دوال: `CreateRun()`, `GetRun(Guid)`, `UpdateMapping()`, `CompleteRun()`
  - تنظيف تلقائي للـ runs القديمة (> 5 دقائق) عبر Timer دوري

- [x] **إنشاء كلاسات التقدم** في `SyncModels.cs`:
  - `SyncRunProgress` — حالة التشغيل الكلية
  - `MappingProgress` — حالة كل تعيين على حدة

- [x] **تعديل `RunSelectedMappingsAsync`** في `SyncEngineService.cs`:
  - استقبال `SyncRunProgress` وتحديثه أثناء المعالجة
  - لكل mapping: update → processing → completed/failed → update

- [x] **تعديل `POST /api/sync/trigger-selected`** في `SyncController.cs`:
  - إنشاء `SyncRunProgress` وإرجاع `runId` فوراً
  - تشغيل المزامنة في الخلفية (Task.Run)
  - إرجاع: `{ runId: "guid", message: "started" }`

- [x] **إضافة endpoint جديد:**
  - `GET /api/sync/progress?runId=GUID` — يرجع حالة التقدم الحالية

### غير المطلوب

- لا UI (سيتم في ENH-001)
- لا تغيير في Web project
- لا SignalR (polling بسيط يكفي للمرحلة الأولى)
- لا تخزين دائم في DB (مؤقت في الذاكرة)

---

## 4. تفاصيل تقنية

### 4.1 هيكل البيانات

```csharp
public class SyncRunProgress
{
    public Guid RunId { get; set; }
    public string OverallStatus { get; set; } = "running"; // running | completed | failed
    public int OverallPercent { get; set; }
    public int TotalRowsSoFar { get; set; }
    public DateTime StartedAt { get; set; }
    public double ElapsedSeconds { get; set; }
    public List<MappingProgress> Mappings { get; set; } = new();
}

public class MappingProgress
{
    public int MappingId { get; set; }
    public string TargetTable { get; set; } = "";
    public string Status { get; set; } = "pending"; // pending | running | completed | failed
    public int RowsSoFar { get; set; }
    public int Percent { get; set; }
    public string? Error { get; set; }
}
```

### 4.2 تدفق العمل

```
1. UI → POST /api/sync/trigger-selected { mappingIds: [1, 3, 5] }
   ↓
2. SyncController:
   - ينشئ SyncRunProgress (GUID جديد)
   - يخزنه في ProgressStore
   - يرجع { runId: "xxx", status: "started" }
   - يشغل المزامنة في الخلفية
   ↓
3. UI → polling GET /api/sync/progress?runId=xxx كل 2 ثانية
   ↓
4. ProgressStore يرجع:
   {
     "runId": "xxx",
     "overallPercent": 42,
     "overallStatus": "running",
     "totalRowsSoFar": 12500,
     "elapsedSeconds": 18,
     "mappings": [
       { "mappingId": 1, "targetTable": "StItems", "status": "completed", "rowsSoFar": 20000, "percent": 100 },
       { "mappingId": 3, "targetTable": "Items2", "status": "running", "rowsSoFar": 5230, "percent": 55 },
       { "mappingId": 5, "targetTable": "StSales", "status": "pending", "rowsSoFar": 0, "percent": 0 }
     ]
   }
   ↓
5. عند الوصول إلى 100% ← UI يوقف polling ويعرض النتيجة النهائية
```

### 4.3 ملف `SyncRunProgressStore.cs`

```csharp
public class SyncRunProgressStore
{
    private readonly ConcurrentDictionary<Guid, SyncRunProgress> _runs = new();
    private readonly ILogger<SyncRunProgressStore> _logger;
    private static readonly TimeSpan MaxAge = TimeSpan.FromMinutes(5);
    private Timer? _cleanupTimer;

    public SyncRunProgress CreateRun(List<int> mappingIds, List<TableMapping> mappings)
    {
        var run = new SyncRunProgress
        {
            RunId = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            OverallStatus = "running",
            Mappings = mappingIds.Select(id =>
            {
                var mapping = mappings.FirstOrDefault(m => m.Id == id);
                return new MappingProgress
                {
                    MappingId = id,
                    TargetTable = mapping?.SqlTargetTable ?? $"Mapping#{id}",
                    Status = "pending"
                };
            }).ToList()
        };
        _runs[run.RunId] = run;
        return run;
    }

    public SyncRunProgress? GetRun(Guid runId)
    {
        _runs.TryGetValue(runId, out var run);
        return run;
    }

    public void UpdateMapping(Guid runId, int mappingId, string status, int rowsSoFar, string? error = null) { ... }
    public void CompleteRun(Guid runId, string overallStatus) { ... }
    private void CleanupOldRuns(object? state) { ... }
}
```

### 4.4 التعديل على `SyncEngineService.RunSelectedMappingsAsync`

تعديل التوقيع لاستقبال مرجع Progress:

```csharp
public async Task<SelectedSyncResult> RunSelectedMappingsAsync(
    List<int> mappingIds,
    SyncRunProgress? progress = null,
    CancellationToken ct = default)
```

داخل الحلقة:
```csharp
progress?.UpdateMapping(runId, mapping.Id, "running", 0);
var data = await _oracle.ExtractAsync(oracleSql, ct);
progress?.UpdateMapping(runId, mapping.Id, "running", data.Rows.Count);
await _load.LoadTableAsync(target, data, ct);
progress?.UpdateMapping(runId, mapping.Id, "completed", data.Rows.Count);
```

### 4.5 التعديل على `SyncController.TriggerSelected`

```csharp
[HttpPost("trigger-selected")]
public async Task<IActionResult> TriggerSelected([FromBody] SelectedSyncRequest request, CancellationToken ct)
{
    // Validation...
    
    var mappings = await _syncEngine.LoadMappingsByIdsAsync(request.MappingIds, ct);
    var run = _progressStore.CreateRun(request.MappingIds, mappings);
    
    // Run in background
    _ = Task.Run(async () =>
    {
        try
        {
            var result = await _syncEngine.RunSelectedMappingsAsync(request.MappingIds, run, ct);
            _progressStore.CompleteRun(run.RunId, result.Status);
        }
        catch (Exception ex)
        {
            _progressStore.CompleteRun(run.RunId, "failed");
            _logger.LogError(ex, "Background sync run {RunId} failed.", run.RunId);
        }
    }, ct);
    
    return Ok(new { runId = run.RunId, status = "started" });
}
```

### 4.6 Endpoint جديد

```csharp
[HttpGet("progress")]
public IActionResult Progress([FromQuery] Guid runId)
{
    var run = _progressStore.GetRun(runId);
    if (run is null)
        return NotFound(new { status = "error", message = "Run not found." });
    
    run.ElapsedSeconds = (DateTime.UtcNow - run.StartedAt).TotalSeconds;
    return Ok(run);
}
```

---

## 5. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncRunProgressStore.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Models\SyncModels.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Controllers\SyncController.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Program.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-ENH-003.md
```

---

## 6. معايير القبول

| # | المعيار | الحالة |
|---|---|---|
| AC-1 | `POST /api/sync/trigger-selected` يرجع runId فوراً بدون انتظار | ✅ |
| AC-2 | `GET /api/sync/progress?runId=...` يرجع حالة التقدم | ✅ |
| AC-3 | التقدم يتضمن: overallPercent, mappings (كل mapping بحالته) | ✅ |
| AC-4 | `ProgressStore` ينظف الـ runs القديمة تلقائياً | ✅ |
| AC-5 | `dotnet build -c Release` = 0 errors / 0 warnings (API project) | ✅ |
| AC-6 | لا أسرار أو connection strings حقيقية | ✅ |

---

## 7. ملاحظات

- `SyncRunProgressStore` يحتاج تسجيل في `Program.cs` كـ `Singleton`
- ProgressStore يحتاج `ILogger<SyncRunProgressStore>` في constructor
- Timer التنظيف: `new Timer(CleanupOldRuns, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(1))`
- **مهم:** `RunSelectedMappingsAsync` يتطلب تعديل بسيط لقبول `SyncRunProgress?` (parameter optional)
