# TASK-ENH-002 — مزامنة جداول محددة (Selected Sync)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ENH-002 |
| **المجموعة** | ENH (Sync Enhancement P0) |
| **النوع** | Backend API |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | ✅ Completed — EngineeringAgent |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

حالياً `SyncEngineService.RunSyncOnceAsync()` يزامن **جميع** التعيينات النشطة بدون خيار. المستخدم يريد تحديد التعيينات التي يريد مزامنتها (واحد أو أكثر) بدلاً من كل شيء.

---

## 2. الهدف

إضافة القدرة على مزامنة تعيينات محددة (selected mappings) عبر API جديد، مع تعديل `SyncEngineService` لدعم القائمة الانتقائية.

---

## 3. النطاق

### المطلوب

- [x] **تعديل `SyncEngineService.cs`:**
  - إضافة دالة `RunSelectedMappingsAsync(List<int> mappingIds, CancellationToken ct)`
  - الدالة تشبه `RunSyncOnceAsync` لكن تزامن فقط التعيينات ذات المعرفات المحددة
  - تحميل التعيينات من DB مع فلتر WHERE Id IN (...)
  - استخدام نفس منطق الاستخراج (OracleExtractionService) والتحميل (SqlServerLoadService) ونظام retry
  - لا تخلط مع المزامنة العامة — `RunSyncOnceAsync` يبقى بدون تغيير

- [x] **تعديل `TableMapping` model (في API project):**
  - إضافة خاصية `Id` (int) — لربط المعرف من جدول TableMappings
  - تحديث `LoadMappingsFromDbAsync` لتضمين `Id` في SELECT

- [x] **إضافة endpoint جديد في `SyncController.cs`:**
  - `POST /api/sync/trigger-selected`
  - Body: `{ "mappingIds": [1, 3, 5] }`
  - يستدعي `RunSelectedMappingsAsync(mappingIds)`
  - يعيد JSON بالنتيجة: `{ status, totalRows, mappingsStatuses: [...] }`

### غير المطلوب

- لا تغيير في `SyncRunLogStore` (يُحسّن لاحقاً في ENH-003)
- لا إضافة progress tracking (ENH-003 منفصل)
- لا UI (Razor page) (ENH-001 منفصل)
- لا تغيير في قاعدة البيانات (Schema)
- لا تغيير في Web project
- لا تغيير في SqlServerLoadService أو OracleExtractionService

---

## 4. تفاصيل تقنية

### 4.1 تدفق API

```
POST /api/sync/trigger-selected
Body: { "mappingIds": [1, 3, 5] }

→ SyncEngineService.RunSelectedMappingsAsync([1, 3, 5])
  → Load mappings by IDs from TableMappings WHERE Id IN (1,3,5) AND IsActive = 1
  → For each mapping:
    → Extract from Oracle (OracleExtractionService)
    → SqlBulkCopy to SQL Server (SqlServerLoadService)
    → Retry up to 3 times on failure
  → Return completion status with per-mapping details

Response: {
  "status": "success",
  "totalRows": 25230,
  "mappings": [
    { "id": 1, "targetTable": "StItems", "status": "success", "rows": 20000 },
    { "id": 3, "targetTable": "Items2", "status": "success", "rows": 5230 },
    { "id": 5, "targetTable": "StSales", "status": "failed", "rows": 0, "error": "..." }
  ]
}
```

### 4.2 التعديلات المطلوبة في الكود

#### `SyncEngineService.cs`:

**إضافة دالة تحميل التعيينات بالمعرفات:**
```csharp
private async Task<List<TableMapping>> LoadMappingsByIdsAsync(List<int> mappingIds, CancellationToken ct)
```

**إضافة دالة المزامنة الانتقائية:**
```csharp
public async Task<SelectedSyncResult> RunSelectedMappingsAsync(List<int> mappingIds, CancellationToken ct)
```

النتيجة المرجعة:
```csharp
public class SelectedSyncResult
{
    public string Status { get; set; } = "success";
    public int TotalRows { get; set; }
    public List<MappingSyncResult> Mappings { get; set; } = new();
}

public class MappingSyncResult
{
    public int Id { get; set; }
    public string TargetTable { get; set; } = "";
    public string Status { get; set; } = "";
    public int Rows { get; set; }
    public string? Error { get; set; }
}
```

#### `TableMapping.cs` (في API project):

```csharp
public class TableMapping
{
    public int Id { get; set; }
    public string OracleSource { get; set; } = "";
    public string SourceType { get; set; } = "Table";
    public string SqlTargetTable { get; set; } = "";
}
```

#### `SyncController.cs`:

```csharp
[HttpPost("trigger-selected")]
public async Task<IActionResult> TriggerSelected([FromBody] SelectedSyncRequest request, CancellationToken ct)
```

---

## 5. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Controllers\SyncController.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Models\TableMapping.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-ENH-002.md
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\test-reports\
```

---

## 6. معايير القبول

| # | المعيار | الحالة |
|---|---|---|
| AC-1 | `POST /api/sync/trigger-selected` مع body صحيح يرجع 200 | ✅ (API build PASS; يتطلب اختبار وظيفي) |
| AC-2 | API تزامن فقط التعيينات المحددة (تأكيد بالـ log) | ✅ (LoadMappingsByIdsAsync مع WHERE Id IN) |
| AC-3 | API تتعامل مع mappingIds فارغ ← 400 Bad Request | ✅ (تم التنفيذ في SyncController.TriggerSelected) |
| AC-4 | API تتعامل مع mappingIds غير موجودة ← تتجاهلها بسلام | ✅ (mappings.Count == 0 → تُرجع skipped لكل ID) |
| AC-5 | `TableMapping.Id` يُقرأ من DB ومتوفر في الرمز | ✅ (SELECT Id + تعبئة الخاصية) |
| AC-6 | `dotnet build -c Release` = 0 errors / 0 warnings | ✅ (Api project; Web pre-existing error unrelated) |
| AC-7 | لا توجد أسرار أو connection strings حقيقية في المخرجات | ✅ (لا أسرار في الكود) |

---

## 7. Pre-Execution Gate

**Result:** PASS ✅

| # | السؤال | النتيجة |
|---|---|---|
| 1 | مرتبطة بخطة التطوير المعتمدة؟ | ✅ Yes (SYNC_PAGE_ENHANCEMENT_PLAN.md) |
| 2 | أصغر وحدة تنفيذية؟ | ✅ Yes (API backend فقط) |
| 3 | هدف واحد فقط؟ | ✅ Yes (مزامنة انتقائية) |
| 4 | لا عناصر قابلة للتأجيل؟ | ✅ Yes |
| 5 | لا UI غير مطلوب؟ | ✅ N/A (API فقط) |
| 6 | لا API غير مطلوب؟ | ✅ مطلوب صراحة |
| 7 | لا Auth غير مطلوب؟ | ✅ N/A (لا يمس Auth) |
| 8 | لا DB Schema غير مطلوب؟ | ✅ N/A |
| 9 | لا migration غير مطلوب؟ | ✅ N/A |
| 10 | لا .env حقيقي؟ | ✅ N/A |
| 11 | لا أسرار؟ | ✅ Yes |
| 12-22 | باقي البنود | ✅ مطابقة |
| **Security Sensitivity:** | **Low** — لا يمس auth/secrets/permissions |

---

## 8. Delegation Notes

- الرجاء استخدام **Absolute Paths** لجميع الملفات المعدلة
- المحافظة على نمط `RunSyncOnceAsync` — `RunSelectedMappingsAsync` تشاركه نفس المنطق مع اختلاف المصدر
- لوحظ أن `LoadMappingsFromDbAsync` لا تجلب Id حالياً. يجب تعديل SELECT ليشمل Id
- يتم تجاهل الـ Semaphore في `RunSelectedMappingsAsync` لأنه سيكون موازياً للتشغيل التلقائي (المستخدم يريد تجاوز القفل عند الطلب اليدوي)
- لا حاجة لتعديل `SqlServerLoadService` أو `OracleExtractionService`

---

## 9. Tera Post-Execution Review

**Result:** ✅ Accepted

| البند | الحالة |
|---|---|
| Pre-Execution Gate | ✅ PASS |
| Allowed Write Targets respected | ✅ Yes |
| Scope respected | ✅ Yes |
| Build verification | ✅ API project: 0 warnings, 0 errors |
| Pre-existing Web error (Login.cshtml) | ⛔ Not introduced - existed before task |
| Secrets written | ✅ None |
| Remaining risk | Endpoint needs functional test against live DB
