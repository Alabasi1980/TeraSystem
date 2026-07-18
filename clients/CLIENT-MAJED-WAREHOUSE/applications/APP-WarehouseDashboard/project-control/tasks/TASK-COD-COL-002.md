# TASK-COD-COL-002 — Column Mapping Editor UI (Wizard Tab)

> **Status:** Approved — Ready for Engineering Delegation
> **Created:** 2026-07-18
> **Approved By:** Majed
> **Owner:** TeraAgent
> **Assigned Agent:** EngineeringAgent
> **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`

---

## 1. Objective

Add a **Column Mapping Editor** as a 4th tab inside Step 3 (Preview & Validate) of the TableMappings wizard. Users see every Oracle column and can configure its SQL Server name, data type, length, precision, scale, nullability, exclusion, and default value.

---

## 2. Scope

### In Scope

1. **Index.cshtml**: Add 4th tab "تعيين الأعمدة" in Step 3 alongside existing Preview/Columns/Schema tabs
2. **table-mapping-wizard.js**: Column mapping editor logic
3. **Index.cshtml**: Add hidden textarea `<textarea id="wm-h-columnMappingsJson">` inside the wizard hidden form, for serialized column mapping data
4. Auto-generate suggested column mappings when preview data loads
5. "إعادة تعيين إلى الافتراضي" button per column and "تطبيق الاقتراح التلقائي لجميع الأعمدة" button

### Out of Scope

1. Do **not** modify `Index.cshtml.cs` (server-side save/load handled in TASK-COD-COL-003)
2. Do **not** modify `OracleSchemaService.cs`
3. Do **not** modify `SchemaManagementService.cs`
4. Do **not** update `GenerateCreateTableSql` or `GenerateAlterStatements`
5. Do **not** run database commands or migrations
6. Do **not** add packages

---

## 3. UI Design — Column Mapping Tab

### Layout

```
┌─────────────────────────────────────────────────────────┐
│  [Preview]  [Columns]  [Schema]  [تعيين الأعمدة] ← New  │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  ┌────────────────────────────────────────────────────┐  │
│  │ Oracle Column    │ SQL Column Name   │ SQL Type  … │  │
│  ├────────────────────────────────────────────────────┤  │
│  │ ORD_DATE         │ [OrderDate......] │ [DATETIME2] │  │
│  │                  │                   │ ▼           │  │
│  │ ITEM_CODE        │ [ItemCode.......] │ [NVARCHAR]  │  │
│  │                  │                   │  50         │  │
│  │ QUANTITY         │ [Quantity.......] │ [INT......] │  │
│  │                  │                   │             │  │
│  └────────────────────────────────────────────────────┘  │
│                                                          │
│  [⟳ إعادة تعيين الكل إلى الافتراضي]                      │
└─────────────────────────────────────────────────────────┘
```

### Each Row Shows

| العمود | نوع التحكم | ملاحظة |
|---|---|---|
| Oracle Column Name | `<span>` نص ثابت | من Oracle |
| Oracle Type | `<span>` نص ثابت صغير | نوع Oracle الأصلي |
| Sql Column Name | `<input type="text">` | قابل للتعديل، الافتراضي = Oracle name |
| Sql Data Type | `<select>` | 20+ نوع SQL Server مع خيار تخصيص |
| Length | `<input type="number">` | يظهر فقط للـ nvarchar/varchar/nchar |
| Precision | `<input type="number">` | يظهر فقط للـ decimal/numeric |
| Scale | `<input type="number">` | يظهر فقط للـ decimal/numeric |
| Nullable | `<input type="checkbox">` | |
| Excluded | `<input type="checkbox">` | إذا مفعّل، يتغير لون الصف |
| Default Value | `<input type="text">` | optional |
| Reset | `<button>` سهم دائري | يعيد تعيين الصف إلى الافتراضي |

### SqlDataType Dropdown Options

```
NVARCHAR(n), VARCHAR(n), NCHAR(n), CHAR(n),
INT, BIGINT, SMALLINT, TINYINT,
DECIMAL(p,s), NUMERIC(p,s), FLOAT, REAL,
DATETIME2, DATE, TIME, DATETIMEOFFSET,
BIT,
NVARCHAR(MAX), VARCHAR(MAX), VARBINARY(MAX),
TEXT, NTEXT, MONEY, UNIQUEIDENTIFIER
```

### Conditional Fields

- Length field: visible only for NVARCHAR, VARCHAR, NCHAR, CHAR, VARBINARY
- Precision/Scale fields: visible only for DECIMAL, NUMERIC
- When user changes type dropdown, show/hide these accordingly

---

## 4. JavaScript — table-mapping-wizard.js

### New State Properties

```javascript
this.state.columnMappings = [];
// Array of { oracleColumnName, sqlColumnName, sqlDataType, sqlMaxLength,
//            sqlPrecision, sqlScale, isNullable, isExcluded,
//            defaultValue, sortOrder, oracleDataType }
```

### Auto-Generation (when preview loads)

After `loadPreview()` completes (in `handlePreviewResult` or equivalent), call `generateColumnMappings(columns, columnTypes)`:

For each column, suggest:
- `sqlColumnName` = Oracle column name (user can change)
- `sqlDataType` = auto-mapped from the existing `OracleSchemaService.MapOracleToSqlServer()` equivalent logic, implemented on the client side as `autoSuggestSqlType(oracleType)`
- `sqlMaxLength`, `sqlPrecision`, `sqlScale` = from preview metadata
- `isNullable` = true
- `isExcluded` = false

### Auto-Suggest Logic (client-side)

Implement a JS function `autoSuggestSqlType(oracleTypeName)` that mirrors the server-side mapping:
```
VARCHAR2 → NVARCHAR(MAX) if length > 4000 else NVARCHAR({length})
CHAR → NCHAR({length})
NUMBER(p,0) with p<=9 → INT
NUMBER(p,0) with p<=18 → BIGINT
NUMBER(p,s) with s>0 → DECIMAL(p,s)
NUMBER with no precision → DECIMAL(18,2)
DATE → DATETIME2
TIMESTAMP → DATETIME2
CLOB → NVARCHAR(MAX)
BLOB → VARBINARY(MAX)
default → NVARCHAR(MAX)
```

### New Methods

| Method | Description |
|---|---|
| `generateColumnMappings(columns, columnTypes)` | Creates initial columnMappings array from preview |
| `renderColumnMappingEditor()` | Renders the tab content into the DOM |
| `renderColumnMappingRow(mapping, index)` | Renders a single editable row |
| `updateColumnMappingRow(index, field, value)` | Updates state when user changes a field |
| `resetColumnMappingToDefault(index)` | Resets a single row to auto-suggested values |
| `resetAllColumnMappings()` | Resets all rows to auto-suggested values |
| `getColumnMappingsJson()` | Serializes columnMappings to JSON string |
| `syncColumnMappingsHiddenField()` | Writes JSON to `#wm-h-columnMappingsJson` |

### Integration Points

1. After `loadPreview()` completes → call `generateColumnMappings()` + `renderColumnMappingEditor()`
2. In the edit mode `bootstrapEditMode()` → column mappings should be loaded from the saved JSON (requires TASK-COD-COL-003, but stub for now)
3. In `save()` method → call `syncColumnMappingsHiddenField()` before form submit

---

## 5. Allowed Sources

- `ClientAppPath/project-control/tasks/TASK-COD-COL-002.md`
- `ClientAppPath/src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml`
- `ClientAppPath/src/WarehouseDashboard.Web/wwwroot/js/table-mapping-wizard.js`
- `ClientAppPath/project-control/PHASE_A_COLUMN_MAPPING_PLAN.md`
- `ClientAppPath/project-control/audit-reports/QUAUD-TABLEMAPPINGS-001-2026-07-18-001.md`
- `tera-system/profiles/dotnet-razorpages-adonet.md`

## 6. Allowed Write Targets

- `ClientAppPath/src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml`
- `ClientAppPath/src/WarehouseDashboard.Web/wwwroot/js/table-mapping-wizard.js`

## 7. Acceptance Criteria

1. New tab "تعيين الأعمدة" appears in Step 3 alongside Preview/Columns/Schema
2. Tab shows a table with one row per Oracle column
3. Each row has editable controls: SqlColumnName, SqlDataType dropdown, Length, Precision, Scale, Nullable, Excluded, Default
4. Conditional length/precision/scale fields show/hide based on selected data type
5. Auto-suggest types are generated when preview loads
6. "إعادة تعيين إلى الافتراضي" button works per row
7. "تطبيق الاقتراح التلقائي لجميع الأعمدة" button works
8. Hidden textarea `#wm-h-columnMappingsJson` stores serialized column mappings before save
9. Build succeeds
10. No server-side code, controllers, services, or DB changes

---

## 8. Security Sensitivity

- **Level:** Low
- **Reason:** Frontend UI only; no secrets, auth, or API changes.

---

## 9. Pre-Execution Gate Result

**Gate Status:** PASS

---

## 10. Delegation Notes

EngineeringAgent must:

1. Read current files from disk before editing.
2. Preserve unrelated changes.
3. Keep the existing wizard navigation working — the new tab is just an additional tab inside Step 3.
4. The tab button should be styled consistently with the other tab buttons.
5. Ensure the column mapping data survives `resetState()` properly (clear on new wizard, keep on edit — implement a `clearColumnMappings` parameter).
6. For the data type dropdown, include the most common SQL Server types but keep it open for custom typing if possible.
7. When a column is marked as Excluded, dim the row visually (reduce opacity).
8. Add a summary count at the top of the tab: "عدد الأعمدة: 15 | نشط: 13 | مستبعد: 2".

---

## 11. Handback Placeholder

Pending EngineeringAgent handback.
