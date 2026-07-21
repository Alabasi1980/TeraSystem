# TASK-SYNC-INC-01 — تحسين المزامنة التزايدية مع تاريخ بداية

**Created:** 2026-07-21
**Status:** Draft → Approved
**Priority:** High
**Type:** Enhancement — Sync Engine
**Phase:** 6 (Implementation)
**Invoked By:** TeraAgent

---

## 1. Objective

تحسين آلية المزامنة التزايدية (Incremental Sync) لتكون عملية وموثوقة. الإضافة الرئيسية هي **تاريخ بداية المزامنة** — يسمح للمستخدم بتحديد من أي تاريخ تبدأ أول مزامنة، بدلاً من جلب كل البيانات的历史.

## 2. Problem Statement

المستخدم لديه جداول مثل `stg_ST_INVOICE` تحتوي 4 سنوات من البيانات. المزامنة الكاملة تحذف كل شيء وتُعيده. المزامنة التزايدية موجودة لكن:
1. لا يوجد "تاريخ بداية" — أول مزامنة تجلب كل الصفحات
2. لا يوجد deduplication — تكرار المزامنة يُسبب تكرار صفوف
3. لا يوجد validation على العمود التاريخي
4. الحقل لا يُحفظ بشكل صحيح في بعض الأحيان

## 3. Acceptance Criteria

### Model Layer
- [ ] `TableMappingConfig.cs` — إضافة خاصية `public DateTime? InitialSyncStartDate { get; set; }` مع `[MaxLength(50)]` nullable
- [ ] `TableMapping.cs` (API side) — إضافة خاصية `public DateTime? InitialSyncStartDate { get; set; }`
- [ ] EF Core configuration في `WarehouseDashboardDbContext.cs` — تكوين العمود الجديد

### Migration
- [ ] Migration جديدة تضيف `InitialSyncStartDate` (datetime2, nullable) إلى `TableMappings`

### Sync Engine
- [ ] `BuildOracleQuery()` — تعديل الشرط: إذا كان `LastSyncAt == null` و `InitialSyncStartDate != null`، استخدم `InitialSyncStartDate` كبداية بدلاً من جلب كل شيء
- [ ] `BuildOracleQuery()` — إذا كان `LastSyncAt != null`، استخدم `LastSyncAt` كالمعتاد
- [ ] `LoadTableIncrementalAsync()` — إضافة deduplication: استخدام `MERGE` أو `DELETE WHERE date >= min_date THEN INSERT` بدلاً من INSERT فقط
- [ ] `LoadMappingsFromDbAsync()` — إضافة `InitialSyncStartDate` إلى الاستعلام

### API Layer
- [ ] `TableMappingController.cs` — `CreateMappingRequest` و `UpdateMappingRequest` يجب أن يتضمن `InitialSyncStartDate`
- [ ] `TableMappingController.cs` — `GetAll` و `GetActive` يجب أن يُعيده

### UI Layer
- [ ] `Index.cshtml` Step 5 — إضافة حقل تاريخ `InitialSyncStartDate` يظهر فقط عند اختيار Incremental
- [ ] `Index.cshtml.cs` — `OnPostAddAsync` و `OnPostEditAsync` يحفظان `InitialSyncStartDate`
- [ ] `table-mapping-wizard.js` — إدارة الحقل الجديد في `save()` و `openWizardForEdit()`
- [ ] Validation في Step 5: إذا اختار Incremental يجب أن يختار عمود التاريخ

### Build
- [ ] `dotnet build` — 0 Errors, 0 Warnings

## 4. Detailed Specifications

### 4.1 Model Changes

**TableMappingConfig.cs** (Web side):
```csharp
[MaxLength(50)]
public DateTime? InitialSyncStartDate { get; set; }
```

**TableMapping.cs** (API side):
```csharp
public DateTime? InitialSyncStartDate { get; set; }
```

### 4.2 BuildOracleQuery Logic

```csharp
private static string BuildOracleQuery(TableMapping mapping, DateTime? lastSyncAt)
{
    // ... existing query source handling ...
    
    string query = $"SELECT * FROM {mapping.OracleSource}";
    
    if (mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(mapping.IncrementalColumn) &&
        IsSafeOracleIdentifier(mapping.IncrementalColumn))
    {
        // Determine the effective start date
        DateTime? effectiveStartDate = lastSyncAt ?? mapping.InitialSyncStartDate;
        
        if (effectiveStartDate.HasValue)
        {
            var ts = effectiveStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            query += $" WHERE {mapping.IncrementalColumn} > TIMESTAMP '{ts}'";
        }
        // If no effectiveStartDate, fetch all (first sync without InitialSyncStartDate)
    }
    
    return query;
}
```

### 4.3 LoadTableIncrementalAsync (Deduplication)

Replace simple `SqlBulkCopy` with a pattern that prevents duplicates:

```csharp
public async Task LoadTableIncrementalAsync(string targetTable, DataTable data, CancellationToken ct)
{
    // ... ensure table exists (same as current) ...
    
    // Get the min date from the data for deduplication
    // Use MERGE or DELETE+INSERT for the date range
    
    using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
    bulkCopy.DestinationTableName = targetTable;
    // ... same column mapping ...
    await bulkCopy.WriteToServerAsync(data, ct);
}
```

**Approach:** For simplicity and safety, use `DELETE WHERE date >= min_date_in_batch AND date <= max_date_in_batch` before inserting the batch. This ensures no duplicates for the same date range while preserving older data.

### 4.4 UI Changes (Step 5)

Add after the date column dropdown:
```html
<div class="wd-field" id="wm-start-date-group" style="display:none">
    <label class="wd-label">تاريخ بداية المزامنة</label>
    <input type="date" id="wm-start-date" class="wd-input" />
    <div class="wd-hint">فقط للمزامنة الأولى — بعدها يتحدث تلقائياً</div>
</div>
```

## 5. Allowed Write Targets

- `src/WarehouseDashboard.Web/Models/TableMappingConfig.cs`
- `src/WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs`
- `src/WarehouseDashboard.Web/Data/Migrations/` (new migration only)
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml`
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml.cs`
- `src/WarehouseDashboard.Web/wwwroot/js/table-mapping-wizard.js`
- `src/WarehouseDashboard.Api/Models/TableMapping.cs`
- `src/WarehouseDashboard.Api/Services/SyncEngineService.cs`
- `src/WarehouseDashboard.Api/Services/SqlServerLoadService.cs`
- `src/WarehouseDashboard.Api/Controllers/TableMappingController.cs`
- `project-control/tasks/TASK-SYNC-INC-01.md`

## 6. Not in Scope

- Deduplication by primary key (only by date range)
- Lookback window (e.g., "always include last 7 days")
- Progress tracking improvements
- Real-time sync triggers
- Permission system for sync operations

## 7. Risk Assessment

| Risk | Severity | Mitigation |
|---|---|---|
| Migration fails on existing data | Medium | InitialSyncStartDate is nullable, no data impact |
| Deduplication DELETE removes wrong data | Medium | Use exact date range from DataTable min/max |
| Oracle TIMESTAMP literal timezone issues | Low | Acceptable for admin tool |

## 8. Post-Execution Review Gate

After implementation:
- [ ] `dotnet build` passes
- [ ] Migration applies cleanly
- [ ] Wizard shows date field for Incremental mode
- [ ] Date field saves and loads correctly in edit mode
- [ ] BuildOracleQuery uses InitialSyncStartDate on first sync
- [ ] Incremental sync does not create duplicates
- [ ] Full sync mode is unaffected
