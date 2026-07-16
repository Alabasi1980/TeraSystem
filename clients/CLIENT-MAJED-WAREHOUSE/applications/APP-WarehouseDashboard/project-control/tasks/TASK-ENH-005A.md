# TASK-ENH-005A — Incremental Sync Backend (DB + EF + API)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ENH-005A |
| **المجموعة** | ENH (Sync Enhancement P1) |
| **النوع** | Backend (DB + EF + API) |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | 🟡 Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

حالياً كل مزامنة تجلب **كل البيانات** من Oracle (SELECT * FROM ...). المستخدم يحتاج مزامنة تزايدية (Incremental) تأخذ فقط البيانات الجديدة أو المعدّلة من آخر مزامنة، بناءً على حقل تاريخ يختاره لكل Mapping.

---

## 2. الهدف

بناء البنية التحتية في قاعدة البيانات و API لدعم المزامنة التزايدية (Full / Incremental) مع إضافة أعمدة جديدة لجدول `TableMappings` وتعديل `SyncEngineService`.

---

## 3. قاعدة البيانات — الأعمدة الجديدة

نضيف 3 أعمدة لجدول `TableMappings`:

| العمود | النوع | القيد | الشرح |
|---|---|---|---|
| `SyncMode` | `NVARCHAR(10)` | NOT NULL DEFAULT 'Full' | 'Full' أو 'Incremental' |
| `IncrementalColumn` | `NVARCHAR(128)` | NULL | اسم حقل التاريخ (مثلاً CREATED_DATE) |
| `LastSyncTimestamp` | `DATETIME2` | NULL | يتم تحديثه تلقائياً بعد كل دورة مزامنة |

---

## 4. النطاق

### المطلوب

- [ ] **تحديث `TableMappingConfig.cs`** (Web\Models):
  - إضافة خاصية `SyncMode` (string, default "Full", MaxLength 10)
  - إضافة خاصية `IncrementalColumn` (string? nullable, MaxLength 128)

- [ ] **تحديث `WarehouseDashboardDbContext.cs`**:
  - إضافة `HasColumnType("nvarchar(10)")` و `HasDefaultValue("Full")` لـ SyncMode
  - إضافة `HasColumnType("nvarchar(128)")` لـ IncrementalColumn

- [ ] **إنشاء migration EF**:
  - تشغيل `dotnet ef migrations add AddSyncModeToTableMappings`
  - تطبيق: `dotnet ef database update`
  - (**ملاحظة:** تأكد من تعيين `SQL_PASSWORD` أو الاتصال بقاعدة البيانات)

- [ ] **تحديث `TableMapping.cs`** (Api\Models):
  - إضافة `SyncMode` (string, default "Full")
  - إضافة `IncrementalColumn` (string?)

- [ ] **تحديث `SyncEngineService.cs`**:
  - `LoadMappingsFromDbAsync`: إضافة `SyncMode` و `IncrementalColumn` إلى SELECT
  - `LoadMappingsByIdsAsync`: إضافة نفس الحقول
  - إضافة دالة `BuildOracleQueryWithIncremental`:
    ```csharp
    private static string BuildOracleQueryWithIncremental(
        TableMapping mapping, DateTime? lastSyncTimestamp)
    {
        var baseSql = BuildOracleQuery(mapping); // SQL الأساسي
        
        if (mapping.SyncMode != "Incremental" ||
            string.IsNullOrWhiteSpace(mapping.IncrementalColumn) ||
            lastSyncTimestamp is null)
            return baseSql;
        
        // بناء WHERE إضافي
        var formattedDate = lastSyncTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss");
        
        if (mapping.SourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
        {
            // لـ Query: نلفها كـ subquery ونضيف WHERE خارجي
            return $"""
                SELECT * FROM ({baseSql})
                WHERE {QuoteOracleIdentifier(mapping.IncrementalColumn)} > TO_DATE('{formattedDate}', 'YYYY-MM-DD HH24:MI:SS')
                """;
        }
        else
        {
            return $"{baseSql} WHERE {QuoteOracleIdentifier(mapping.IncrementalColumn)} > TO_DATE('{formattedDate}', 'YYYY-MM-DD HH24:MI:SS')";
        }
    }
    
    private static string QuoteOracleIdentifier(string identifier)
    {
        // Oracle quoted identifier
        return $"\"{identifier}\"";
    }
    ```

  - تعديل `RunSyncOnceAsync` و `RunSelectedMappingsAsync`:
    - لكل mapping، إذا `SyncMode == "Incremental"` و `IncrementalColumn` موجود:
      - استخدم `BuildOracleQueryWithIncremental` بدل `BuildOracleQuery`
      - اقرأ `LastSyncAt` من `TableMapping` (ملاحظة: هذا الحقل موجود حالياً في الـ DB)
    - بعد نجاح المزامنة، حدّث `LastSyncTimestamp` للتعيين:
      ```sql
      UPDATE TableMappings SET LastSyncTimestamp = GETUTCDATE() WHERE Id = @Id
      ```

### غير المطلوب

- لا تغيير في واجهة TableMappings (wizard UI) — سيكون في ENH-005B
- لا تغيير في Sync Dashboard (سيكون في مهمة منفصلة)
- لا تغيير في SqlServerLoadService أو OracleExtractionService

---

## 5. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\TableMappingConfig.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Models\TableMapping.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-ENH-005A.md
```

---

## 6. معايير القبول

| # | المعيار | الحالة |
|---|---|---|
| AC-1 | جدول `TableMappings` فيه الأعمدة الجديدة (SyncMode, IncrementalColumn, LastSyncTimestamp) | ⬜ |
| AC-2 | EF Migration تنشأ وتُطبق بنجاح | ⬜ |
| AC-3 | API تقرأ `SyncMode` و `IncrementalColumn` من الـ DB | ⬜ |
| AC-4 | `RunSelectedMappingsAsync` تطبق WHERE للـ Incremental | ⬜ |
| AC-5 | بعد المزامنة، `LastSyncTimestamp` يتحدّث في الـ DB | ⬜ |
| AC-6 | إذا `SyncMode = 'Full'`، نفس السلوك الحالي (بدون WHERE) | ⬜ |
| AC-7 | إذا `IncrementalColumn = NULL`، نفس السلوك الحالي | ⬜ |
| AC-8 | `dotnet build -c Release` = 0 errors / 0 warnings | ⬜ |
| AC-9 | لا أسرار أو connection strings حقيقية | ⬜ |

---

## 7. ملاحظات تقنية مهمة

### Quote للـ Oracle Identifier
Oracle identifiers حساسة لحالة الأحرف إذا استُخدمت علامات الاقتباس المزدوجة. غالباً الحقول في Oracle تكون كبيرة (CREATED_DATE) أو صغيرة. الأفضل استخدامها بدون اقتباس (افتراضياً Oracle يحولها لكبيرة):
```csharp
// بدون اقتباس — Oracle يحول الحروف لكبيرة تلقائياً
return $"{baseSql} WHERE {mapping.IncrementalColumn} > TO_DATE(...)";
```

### الـ Query source والمزامنة التزايدية
للاستعلامات (Query)، الحل الأفضل هو لفها كـ subquery:
```sql
SELECT * FROM (
  SELECT ITEM_CODE, ITEM_DESC, CREATED_DATE FROM ST_ITEM_CARD WHERE ITEM_CODE LIKE '03%'
) WHERE CREATED_DATE > TO_DATE('2026-07-14 14:30', 'YYYY-MM-DD HH24:MI:SS')
```

### تحديث `LastSyncTimestamp`
بعد نجاح كل Mapping، تشغيل UPDATE:
```sql
UPDATE TableMappings SET LastSyncTimestamp = GETUTCDATE() WHERE Id = @MappingId
```
(استخدم معامل @MappingId للسلامة)

### `TableMapping.cs` (API) يجب أن يعكس الـ DB
```csharp
public class TableMapping
{
    public int Id { get; set; }
    public string OracleSource { get; set; } = "";
    public string SourceType { get; set; } = "Table";
    public string SqlTargetTable { get; set; } = "";
    public string SyncMode { get; set; } = "Full";
    public string? IncrementalColumn { get; set; }
    public DateTime? LastSyncAt { get; set; } // موجود مسبقاً في الـ DB
}
```

### `LoadMappingsFromDbAsync` — تحديث SELECT
```sql
SELECT Id, OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn, LastSyncAt
FROM TableMappings WHERE IsActive = 1
```

لا تنسَ `LastSyncAt` — الحقل موجود في قاعدة البيانات حالياً ولكنه ليس في `TableMapping.cs` API model.

### `OnGetMappingAsync` في TableMappingsModel
هذا الـ handler يحتاج تضمين الحقول الجديدة لتعمل الويزرد للـ Edit:
```csharp
Select(m => new
{
    editId = m.Id,
    oracleSource = m.OracleSource,
    sourceType = m.SourceType,
    sqlTargetTable = m.SqlTargetTable,
    syncMode = m.SyncMode,
    incrementalColumn = m.IncrementalColumn
})
```
