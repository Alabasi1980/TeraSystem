# TASK-SYNC-WATERMARK-001 — فصل LastSyncAt (عرض) عن IncrementalWatermarkAt (مائي)

> **التاريخ:** 2026-07-22
> **الحالة:** ✅ Approved for execution
> **المكلّف:** engineering-agent-dotnet
> **أولوية:** High

---

## المشكلة

`LastSyncAt` له غرضان متعارضان:
1. **عرض** "آخر مزامنة" في الواجهة — يجب أن يُحدّث لكل أنواع المزامنة
2. **Watermark** للمزامنة المتزايدة — يجب ألا يُحدّث بواسطة Full sync

## الحل

إضافة عمود جديد `IncrementalWatermarkAt` خاص بالـ watermark فقط. `LastSyncAt` يبقى للعرض ويُحدّث دائماً.

---

## التغييرات

### 1. إضافة الحقل للنموذج

**ملف:** `Models/TableMappingConfig.cs`

أضف:
```csharp
/// <summary>
/// Timestamp of the last successful INCREMENTAL sync for this mapping.
/// Used as the watermark boundary for incremental syncs.
/// Null if no incremental sync has ever run.
/// </summary>
public DateTime? IncrementalWatermarkAt { get; set; }
```

### 2. إضافة Migration

```bash
dotnet ef migrations add AddIncrementalWatermarkAt
dotnet ef database update
```

### 3. تحديث `UpdateLastSyncAtAsync` في `SyncEngineService.cs`

```csharp
private async Task UpdateLastSyncAtAsync(int mappingId, int recordCount, bool isIncremental, CancellationToken ct)
{
    try
    {
        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        // LastSyncAt is always updated (display).
        // IncrementalWatermarkAt is updated only for incremental syncs.
        cmd.CommandText = isIncremental
            ? """
              UPDATE TableMappings
              SET LastSyncAt = GETUTCDATE(),
                  IncrementalWatermarkAt = GETUTCDATE(),
                  SyncRecordCount = @RecordCount,
                  UpdatedAt = GETUTCDATE()
              WHERE Id = @Id
              """
            : """
              UPDATE TableMappings
              SET LastSyncAt = GETUTCDATE(),
                  SyncRecordCount = @RecordCount,
                  UpdatedAt = GETUTCDATE()
              WHERE Id = @Id
              """;
        cmd.Parameters.AddWithValue("@Id", mappingId);
        cmd.Parameters.AddWithValue("@RecordCount", recordCount);
        await cmd.ExecuteNonQueryAsync(ct);
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Failed to update LastSyncAt for mapping {Id}.", mappingId);
    }
}
```

### 4. تحديث استعلامات تحميل الـ Mappings

في `LoadMappingsAsync` و `LoadMappingsByIdsAsync`، أضف `IncrementalWatermarkAt` إلى الـ SELECT:

```sql
SELECT Id, OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn, 
       LastSyncAt, InitialSyncStartDate, IncrementalWatermarkAt, ...
```

وفي قراءة الـ DataReader:
```csharp
IncrementalWatermarkAt = reader.IsDBNull(reader.GetOrdinal("IncrementalWatermarkAt"))
    ? null
    : reader.GetDateTime(reader.GetOrdinal("IncrementalWatermarkAt")),
```

### 5. تحديث `BuildOracleQuery`

استخدم `IncrementalWatermarkAt` بدلاً من `lastSyncAt`:

```csharp
private static string BuildOracleQuery(TableMapping mapping, DateTime? lastSyncAt, DateTime? incrementalWatermarkAt)
```

وغيّر منطق الـ watermark:
```csharp
// For incremental syncs, use IncrementalWatermarkAt (watermark from previous incremental)
// or fall back to InitialSyncStartDate (for the first incremental run).
DateTime? watermark = incrementalWatermarkAt ?? mapping.InitialSyncStartDate;
```

### 6. تحديث استدعاءات `BuildOracleQuery`

في كل مكان يُنادى فيه `BuildOracleQuery`، مرّر `mapping.IncrementalWatermarkAt`:

```csharp
var oracleSql = BuildOracleQuery(mapping, mapping.LastSyncAt, mapping.IncrementalWatermarkAt);
```

### 7. تحديث استدعاءات `UpdateLastSyncAtAsync`

مرّر `isIncremental`:
```csharp
await UpdateLastSyncAtAsync(mapping.Id, data.Rows.Count, 
    mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase), ct);
```

---

## الملفات المتأثرة

1. `Models/TableMappingConfig.cs` — إضافة `IncrementalWatermarkAt`
2. `WarehouseDashboard.Web.csproj` — (مجلد المشروع لـ EF migration)
3. Migration files (تتولّد تلقائياً)
4. `Services/SyncEngineService.cs` — تحديث `UpdateLastSyncAtAsync`, `BuildOracleQuery`, `LoadMappingsAsync`, `LoadMappingsByIdsAsync`

---

## Acceptance Criteria

1. ✅ `IncrementalWatermarkAt` مضاف لـ `TableMappingConfig`
2. ✅ Migration generated and applied
3. ✅ `UpdateLastSyncAtAsync` يحدّث `LastSyncAt` دائماً + `IncrementalWatermarkAt` فقط لـ Incremental
4. ✅ `BuildOracleQuery` يستخدم `IncrementalWatermarkAt` أو `InitialSyncStartDate` للـ watermark
5. ✅ `dotnet build` PASS
