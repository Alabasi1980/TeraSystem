# TASK-COD-COL-001 — ColumnMapping Entity + DbContext + Migration

> **Status:** Approved — Ready for Engineering Delegation  
> **Created:** 2026-07-18  
> **Approved By:** Majed  
> **Owner:** TeraAgent  
> **Assigned Agent:** EngineeringAgent  
> **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## 1. Objective

Create the data model for column-level type mapping as part of Phase A. This establishes the `ColumnMappings` table with a foreign key to `TableMappings`, so each mapping can have per-column type overrides.

---

## 2. Definition

- **New Table:** `ColumnMappings`
- **Relationship:** Many-to-one with `TableMappingConfig` (CASCADE delete)
- **Purpose:** Store user-defined overrides for each column's SQL Server name, data type, length, precision, scale, nullability, exclusion, and default value.

---

## 3. Scope

### In Scope

1. Create new file: `Models/ColumnMapping.cs`
2. Update `Models/TableMappingConfig.cs`: add `ICollection<ColumnMapping>? ColumnMappings` navigation property
3. Update `Data/WarehouseDashboardDbContext.cs`: add `DbSet<ColumnMapping>` and Fluent API config
4. Run `dotnet ef migrations add AddColumnMappings` to generate migration
5. Verify build succeeds

### Out of Scope

1. Do **not** modify any UI (wizard, tables, forms)
2. Do **not** modify `table-mapping-wizard.js`
3. Do **not** modify `Index.cshtml` or `Index.cshtml.cs`
4. Do **not** modify `OracleSchemaService.cs` or `SchemaManagementService.cs`
5. Do **not** modify `TableMappingController.cs`
6. Do **not** add API endpoints
7. Do **not** update the `GenerateCreateTableSql` or `GenerateAlterStatements`

---

## 4. ColumnMapping Entity Definition

| Property | C# Type | SQL Column | Constraints |
|---|---|---|---|
| Id | int | Id | PK, auto-increment |
| TableMappingConfigId | int | TableMappingConfigId | FK → TableMappings.Id, CASCADE |
| OracleColumnName | string | OracleColumnName | nvarchar(128), required |
| SqlColumnName | string | SqlColumnName | nvarchar(128), required, default = OracleColumnName |
| SqlDataType | string | SqlDataType | nvarchar(50), required |
| SqlMaxLength | int? | SqlMaxLength | nullable |
| SqlPrecision | int? | SqlPrecision | nullable |
| SqlScale | int? | SqlScale | nullable |
| IsNullable | bool | IsNullable | default true |
| IsExcluded | bool | IsExcluded | default false |
| DefaultValue | string? | DefaultValue | nvarchar(500), nullable |
| TransformationExpression | string? | TransformationExpression | nvarchar(max), nullable (for future use) |
| SortOrder | int | SortOrder | default 0 |

### Indexes
- `IX_ColumnMappings_TableMappingConfigId` on `(TableMappingConfigId, OracleColumnName)` — unique per-column

### Navigation
- `TableMappingConfig` → `ICollection<ColumnMapping>`

---

## 5. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-COD-COL-001.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\TableMappingConfig.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\PHASE_A_COLUMN_MAPPING_PLAN.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\tera-system\profiles\dotnet-razorpages-adonet.md`

## 6. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\ColumnMapping.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\TableMappingConfig.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs`
- Migration files created by `dotnet ef migrations add`
- `WarehouseDashboardDbContextModelSnapshot.cs` (updated by migration command)

---

## 7. Acceptance Criteria

1. `ColumnMapping.cs` entity exists with all specified properties
2. `TableMappingConfig.cs` has navigation property `ColumnMappings`
3. `WarehouseDashboardDbContext.cs` has `DbSet<ColumnMapping>` and Fluent config
4. `dotnet ef migrations add AddColumnMappings` generates a valid migration
5. `dotnet build` succeeds
6. No UI, JS, service, controller, or unrelated file is modified

---

## 8. Security Sensitivity

- **Level:** Low
- **Reason:** Data model only; no execution logic, secrets, or auth changes.

---

## 9. Pre-Execution Gate Result

**Gate Status:** PASS

---

## 10. Delegation Notes

EngineeringAgent must:

1. Follow the exact entity definition above.
2. Use the same Fluent API style as the existing `TableMappingConfig` in `DbContext`.
3. Make `(TableMappingConfigId, OracleColumnName)` a unique index.
4. Set CASCADE delete on the FK.
5. After creating/updating the entity files, run:
   ```bash
   dotnet ef migrations add AddColumnMappings
   ```
   from workdir: `ClientAppPath/src/WarehouseDashboard.Web`
6. Do NOT run `dotnet ef database update`.
7. Verify build after migration generation.

---

## 11. Handback Placeholder

Pending EngineeringAgent handback.
