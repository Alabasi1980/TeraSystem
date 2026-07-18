# TASK-COD-COL-003 — Backend: Save/Load Column Mappings + Schema Generation Updates

> **Status:** Approved — Ready for Engineering Delegation
> **Created:** 2026-07-18
> **Approved By:** Majed
> **Owner:** TeraAgent
> **Assigned Agent:** EngineeringAgent
> **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## 1. Objective

Connect the Column Mapping Editor (TASK-COD-COL-002) to the backend:
1. Save/load `ColumnMappings` from the wizard form
2. Update schema generation to respect column-level overrides

---

## 2. Scope

### In Scope

1. **`Index.cshtml.cs`**: Parse `ColumnMappingsJson` on Add/Edit, create/update `ColumnMapping` entities
2. **`Index.cshtml.cs`**: Load column mappings for edit mode
3. **`SchemaManagementService.cs`**: Update `GenerateCreateTableSql()` and `GenerateAlterStatements()` to accept optional `ColumnMapping` overrides
4. **`OracleSchemaService.cs`**: Update `CompareSchemasAsync()` to use overridden SQL types
5. Build verification

### Out of Scope

1. Do **not** modify `Index.cshtml` (UI already done)
2. Do **not** modify `table-mapping-wizard.js` (UI already done)
3. Do **not** modify `TableMappingController.cs` (API already works with the TableMappingConfig)
4. Do **not** add packages
5. Do **not** run database update (`dotnet ef database update`)

---

## 3. Detailed Implementation

### 3.1 Index.cshtml.cs — Save/Load ColumnMappings

**Add BindProperty:**
```csharp
[BindProperty]
public string? ColumnMappingsJson { get; set; }
```

**OnPostAddAsync() changes:**

After creating the `TableMappingConfig` entity and calling `_db.SaveChangesAsync()`, parse `ColumnMappingsJson` and add `ColumnMapping` entities:

```csharp
// After _db.SaveChangesAsync() that created the TableMappingConfig
if (!string.IsNullOrWhiteSpace(ColumnMappingsJson))
{
    var columnMappings = JsonSerializer.Deserialize<List<ColumnMappingDto>>(ColumnMappingsJson);
    if (columnMappings?.Count > 0)
    {
        foreach (var cm in columnMappings)
        {
            var entity = new ColumnMapping
            {
                TableMappingConfigId = mapping.Id,
                OracleColumnName = cm.OracleColumnName ?? "",
                SqlColumnName = cm.SqlColumnName ?? cm.OracleColumnName ?? "",
                SqlDataType = cm.SqlDataType ?? "NVARCHAR(MAX)",
                SqlMaxLength = cm.SqlMaxLength,
                SqlPrecision = cm.SqlPrecision,
                SqlScale = cm.SqlScale,
                IsNullable = cm.IsNullable,
                IsExcluded = cm.IsExcluded,
                DefaultValue = cm.DefaultValue,
                SortOrder = cm.SortOrder
            };
            _db.ColumnMappings.Add(entity);
        }
        await _db.SaveChangesAsync();
    }
}
```

**OnPostEditAsync() changes:**

Delete existing ColumnMappings, then create from new JSON:
```csharp
// At the start: remove existing column mappings for this mapping
var existingMappings = await _db.ColumnMappings
    .Where(cm => cm.TableMappingConfigId == EditId)
    .ToListAsync();
_db.ColumnMappings.RemoveRange(existingMappings);

// After updating TableMappingConfig, parse JSON and add new column mappings
// Same logic as OnPostAddAsync
```

**Edit mode — load column mappings:**

In `OnGetMappingAsync()` or a new handler, include column mappings serialized as JSON so the JS can load them in edit mode.

**Create a small DTO class for JSON deserialization:**
```csharp
public class ColumnMappingDto
{
    public string OracleColumnName { get; set; } = "";
    public string? SqlColumnName { get; set; }
    public string? SqlDataType { get; set; }
    public int? SqlMaxLength { get; set; }
    public int? SqlPrecision { get; set; }
    public int? SqlScale { get; set; }
    public bool IsNullable { get; set; } = true;
    public bool IsExcluded { get; set; }
    public string? DefaultValue { get; set; }
    public int SortOrder { get; set; }
}
```

### 3.2 SchemaManagementService — Use Column Overrides

**Update `GenerateCreateTableSql()`:**

Add an overload or parameter `List<ColumnMapping>? columnOverrides`:
```csharp
public static string GenerateCreateTableSql(List<ColumnInfo> columns, string tableName, List<ColumnMapping>? columnOverrides = null)
```

When a `ColumnMapping` override exists for a column (matched by `OracleColumnName`):
- Use `override.SqlColumnName` for the SQL column name
- Use `override.SqlDataType` + `override.SqlMaxLength`/`SqlPrecision`/`SqlScale` for the type
- Use `override.IsNullable` for nullability
- Skip columns where `override.IsExcluded == true`

**Update `GenerateAlterStatements()`:**

Add `List<ColumnMapping>? columnOverrides` parameter to apply overrides:
- ADD: Use overridden sql data type and name
- MODIFY: Use overridden type
- DROP: Skip excluded columns

**Update `ApplySchemaChangesAsync()`:**

Pass column overrides from the `TableMappingConfig` to the schema methods.

### 3.3 OracleSchemaService — Type Comparison with Overrides

**Update `CompareSchemasAsync()`:**

Add `List<ColumnMapping>? columnOverrides` parameter. When comparing Oracle columns to SQL Server columns, use the overridden SQL types instead of the auto-mapped types for a more accurate diff.

---

## 4. Allowed Sources

- `ClientAppPath/project-control/tasks/TASK-COD-COL-003.md`
- `ClientAppPath/src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml.cs`
- `ClientAppPath/src/WarehouseDashboard.Web/Services/SchemaManagementService.cs`
- `ClientAppPath/src/WarehouseDashboard.Web/Services/OracleSchemaService.cs`
- `ClientAppPath/src/WarehouseDashboard.Web/Models/ColumnMapping.cs`
- `ClientAppPath/project-control/PHASE_A_COLUMN_MAPPING_PLAN.md`

## 5. Allowed Write Targets

- `ClientAppPath/src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml.cs`
- `ClientAppPath/src/WarehouseDashboard.Web/Services/SchemaManagementService.cs`
- `ClientAppPath/src/WarehouseDashboard.Web/Services/OracleSchemaService.cs`

## 6. Acceptance Criteria

1. Creating a new mapping with column mapping JSON saves `ColumnMapping` entities
2. Editing an existing mapping replaces old column mappings with new ones
3. Edit mode loads existing column mappings
4. `GenerateCreateTableSql()` respects overrides (name, type, nullable, excluded)
5. `GenerateAlterStatements()` respects overrides
6. Build succeeds
7. No UI/JS files modified

## 7. Pre-Execution Gate Result

**Gate Status:** PASS

## 8. Handback Placeholder

Pending EngineeringAgent handback.
