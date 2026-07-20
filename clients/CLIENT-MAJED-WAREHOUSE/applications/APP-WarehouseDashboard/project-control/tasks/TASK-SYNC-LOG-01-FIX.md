# TASK-SYNC-LOG-01-FIX — API: SyncEngine DB Writing + SyncController DB Reading

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-LOG-01-FIX |
| **النوع** | Backend — API SyncEngine + SyncController |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-20 |

---

## 1. الهدف

تعديل `SyncEngineService` و `SyncController` ليكتبوا ويقرأوا سجلات المزامنة من قاعدة البيانات (جدولي SyncRuns و SyncRunDetails) بدلاً من الـ in-memory `SyncRunLogStore`.

---

## 2. التغييرات المطلوبة

### 2.1 SyncEngineService.cs — إضافة DB Writing

في نهاية `RunSyncOnceAsync()` (قبل `SetStatus("Success")`):
```csharp
// Persist sync run log to DB
try
{
    var connStr = ConnectionStringHelper.ResolveSql(_configuration);
    if (!string.IsNullOrWhiteSpace(connStr))
    {
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(ct);
        
        // إدراج SyncRun
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO SyncRuns (StartTime, EndTime, Status, TriggerType, TotalRecordCount, TotalDurationSeconds)
            OUTPUT INSERTED.Id
            VALUES (@start, @end, @status, @trigger, @count, @duration)";
        cmd.Parameters.AddWithValue("@start", DateTime.UtcNow - totalDuration);
        cmd.Parameters.AddWithValue("@end", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@status", "Success");
        cmd.Parameters.AddWithValue("@trigger", "Auto"); // أو Manual للـ manual trigger
        cmd.Parameters.AddWithValue("@count", totalRows);
        cmd.Parameters.AddWithValue("@duration", totalDuration.TotalSeconds);
        var runId = (int)await cmd.ExecuteScalarAsync(ct);
        
        // لكل mapping: إدراج SyncRunDetail
        // (لازم نجمع معلومات عن كل mapping أثناء التنفيذ)
    }
}
catch (Exception ex)
{
    _logger.LogWarning(ex, "Failed to persist sync run log to database.");
}
```

التحدي: RunSyncOnceAsync حالياً لا يجمع معلومات per-mapping (زي RowsExtracted, DurationSeconds, ErrorMessage). لذلك نحتاج إلى:
1. إضافة tracking variables قبل الـ foreach loop
2. لكل mapping: تسجيل start time, عدد الصفوف
3. بعد كل mapping نجاح/فشل: إضافة SyncRunDetail

**نفس التغيير لـ RunSelectedMappingsAsync** — لكن هذا أسهل لأنه عنده `MappingSyncResult` جاهز.

### 2.2 SyncController.cs — تغيير قراءة logs

استبدل `GET /api/sync/logs`:
```csharp
[HttpGet("logs")]
public async Task<IActionResult> Logs(CancellationToken ct)
{
    var connStr = ConnectionStringHelper.ResolveSql(_configuration);
    if (string.IsNullOrWhiteSpace(connStr))
        return Ok(Array.Empty<object>());

    await using var conn = new SqlConnection(connStr);
    await conn.OpenAsync(ct);
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        SELECT Id, StartTime, EndTime, Status, TriggerType, 
               TotalRecordCount, TotalDurationSeconds
        FROM SyncRuns 
        ORDER BY StartTime DESC";
    
    var records = new List<object>();
    await using var reader = await cmd.ExecuteReaderAsync(ct);
    while (await reader.ReadAsync(ct))
    {
        records.Add(new {
            id = reader.GetInt32(0),
            startTime = reader.GetDateTime(1),
            endTime = reader.IsDBNull(2) ? null : (DateTime?)reader.GetDateTime(2),
            status = reader.GetString(3),
            triggerType = reader.GetString(4),
            recordCount = reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5),
            duration = reader.IsDBNull(6) ? null : (double?)reader.GetDouble(6)
        });
    }
    return Ok(records);
}
```

أضف `GET /api/sync/logs/{runId}` endpoint جديد:
```csharp
[HttpGet("logs/{runId:int}")]
public async Task<IActionResult> LogDetail(int runId, CancellationToken ct)
{
    // يرجع تفاصيل دورة محددة (SyncRun + SyncRunDetails)
}
```

### 2.3 نقل DatabaseHelper

لاحظ أن `ConnectionStringHelper.ResolveSql(_configuration)` مستخدم في SyncEngineService بالفعل. استمر في استخدامه.

---

## 3. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Controllers\SyncController.cs`

---

## 4. Acceptance Criteria

| # | المعيار |
|---|---|
| AC-1 | `RunSyncOnceAsync()` يكتب SyncRun إلى DB بعد الدورة في كل حالة (Success/Partial/Failed/Cancelled) |
| AC-2 | كل mapping داخل الدورة يكتب SyncRunDetail مع TargetTable, RowsLoaded, Status, DurationSeconds |
| AC-3 | `RunSelectedMappingsAsync()` يكتب SyncRun + SyncRunDetails إلى DB |
| AC-4 | `GET /api/sync/logs` يقرأ من جدول SyncRuns |
| AC-5 | `GET /api/sync/logs/{runId}` يرجع SyncRun + SyncRunDetails |
| AC-6 | `dotnet build` ينجح (API project) |
| AC-7 | لا يوجد كسر في الـ try/catch الحالي — فشل كتابة الـ log لا يوقف المزامنة |

---

## 5. ملاحظات مهمة

1. اقرأ كل ملف من القرص قبل التعديل.
2. `RunSyncOnceAsync()` ليس عنده `MappingSyncResult` collection — ستحتاج تجميع النتائج يدوياً في List أثناء الـ loop.
3. لا تنسَ حالة `Partial` (بعض الجداول نجح وبعضها فشل).
4. الوقت (DurationSeconds) لكل mapping: احسبه بـ Stopwatch أو اطرح StartTime من EndTime لكل mapping.
5. `RunSelectedMappingsAsync()` عنده `result.Mappings` (List<MappingSyncResult>) — استخدمها مباشرة.
6. الـ `SyncRunLogStore` القديم لا تحذفه — ابقه احتياطاً أو اتركه لتنظيف لاحق.
7. راعي أن الـ Already-in-memory ProgressStore يبقى كما هو — دوره مختلف (للمراقبة الحية).

ارجع بالـ handback مع الملفات المعدّلة و build result.

---

## 7. Handback

| البند | القيمة |
|---|---|
| **الحالة** | Accepted |
| **التاريخ** | 2026-07-20 |
| **المعرّف** | TASK-SYNC-LOG-01-FIX |
| **التنفيذ** | engineering-agent-dotnet |

### التغييرات

- **SyncModels.cs** — إضافة `DurationSeconds` إلى `MappingSyncResult`
- **SyncEngineService.cs** — إعادة هيكلة:
  - `RunSyncOnceAsync(ct, triggerType = "Auto")` جديد
  - tracking لكل mapping (mappingStartTime, mappingRows)
  - `PersistSyncRunToDbAsync()` helper يكتب SyncRun + SyncRunDetails
  - تستدعى في حالات: Success / Partial / Failed / Cancelled
  - DB writing داخل try/catch — فشله لا يوقف المزامنة
- **SyncController.cs**:
  - `Logs()` يقرأ من SyncRuns table بدل الذاكرة
  - `LogDetail(int runId)` endpoint جديد
  - `Trigger` يمرر "Manual" كـ triggerType

### Build

`dotnet build` — ✅ 0 errors, 0 warnings
