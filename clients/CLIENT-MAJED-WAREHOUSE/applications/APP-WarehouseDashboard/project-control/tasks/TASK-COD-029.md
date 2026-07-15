# TASK-COD-029 — Oracle Source Mapping Wizard

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-029 |
| **المجموعة** | Enhancement — TableMappings Admin |
| **المرحلة** | Phase 6 |
| **الأولوية** | High |
| **الوكيل** | engineering-agent (backend + UI) |
| **التبعية** | TASK-COD-025 (基础 TableMappings) |
| **الحالة** | 🟡 Assigned |

---

## 1. السبب

مودال "إضافة تعيين" الحالي بسيط جداً: حقل نصي + dropdown + حقل هدف. لا يوجد:
- بحث أو تصفح لجداول/فيوهات Oracle
- فحص صحة المصدر قبل الحفظ
- معاينة بيانات
- عرض أعمدة وأنواعها
- اقتراح اسم SQL Target
- دعم Query بشكل احترافي

المطلوب: تحويله إلى **Oracle Source Mapping Wizard** احترافي بـ 4 خطوات.

---

## 2. النطاق

### Stage A: Backend — OracleSchemaService + APIs

#### 2.1. خدمات جديدة في `OracleSchemaService.cs`

```csharp
// List all tables visible to the Oracle user
Task<List<OracleObject>> ListOracleTablesAsync(CancellationToken ct = default)

// List all views visible to the Oracle user
Task<List<OracleObjects>> ListOracleViewsAsync(CancellationToken ct = default)

// Preview first N rows from any source
Task<DataPreviewResult> PreviewDataAsync(string oracleSource, string sourceType, int limit = 10, CancellationToken ct = default)

// Validate a query and return its column metadata
Task<QueryValidationResult> ValidateQueryAsync(string query, CancellationToken ct = default)
```

**OracleObject** model:
```csharp
public class OracleObject
{
    public string Owner { get; set; }      // e.g. "NATEJSOFT"
    public string ObjectName { get; set; }  // e.g. "WAREHOUSE_STOCK"
    public string ObjectType { get; set; }  // "TABLE" or "VIEW"
    public int ColumnCount { get; set; }
    public bool IsAlreadyMapped { get; set; }  // exists in TableMappings?
}
```

**DataPreviewResult** model:
```csharp
public class DataPreviewResult
{
    public List<string> Columns { get; set; }     // column names
    public List<string> ColumnTypes { get; set; }  // Oracle types
    public List<Dictionary<string, object?>> Rows { get; set; }  // preview data
    public int TotalRows { get; set; }             // total rows in source
    public string? ErrorMessage { get; set; }
}
```

**QueryValidationResult** model:
```csharp
public class QueryValidationResult
{
    public bool IsValid { get; set; }
    public List<ColumnInfo> Columns { get; set; }  // reuse existing ColumnInfo
    public string? ErrorMessage { get; set; }
    public bool IsReadOnly { get; set; }
}
```

**Implementation notes:**
- `ListOracleTablesAsync`: Query `ALL_TABLES WHERE OWNER = :owner` (parse owner from connection string or use current schema). Return table name, owner, column count from `ALL_TAB_COLUMNS`.
- `ListOracleViewsAsync`: Same but from `ALL_VIEWS`.
- Both methods: check `ALL_TAB_COLUMNS` for column count. Check `TableMappings` table for `IsAlreadyMapped`.
- `PreviewDataAsync`: Use existing Oracle connection. Execute `SELECT * FROM source WHERE ROWNUM <= limit` for Table/View, or the query itself for Query. Return column names, types, and rows as dictionaries.
- `ValidateQueryAsync`: Execute `SELECT * FROM (query) WHERE ROWNUM <= 0` to get column metadata without data. Check `IsReadOnlyQuery()` (reuse existing guard from `OracleExtractionService` pattern — only SELECT/WITH allowed).
- All methods: short timeout (10-15 seconds), proper error handling, return error messages in result objects (don't throw).

#### 2.2. API endpoints — New Razor Pages

Create under `Pages/Api/OracleBrowser/`:

**`ListObjects.cshtml.cs`** — GET handler
- Input: `?type=table` or `?type=view`
- Calls `OracleSchemaService.ListOracleTablesAsync()` or `ListOracleViewsAsync()`
- Returns JSON array of `OracleObject`

**`Preview.cshtml.cs`** — POST handler
- Input: `{ source, sourceType, limit }`
- Calls `OracleSchemaService.PreviewDataAsync()`
- Returns JSON `DataPreviewResult`

**`ValidateQuery.cshtml.cs`** — POST handler
- Input: `{ query }`
- Calls `OracleSchemaService.ValidateQueryAsync()`
- Returns JSON `QueryValidationResult`

All endpoints must:
- Require admin session (check `HttpContext.Session.GetString("AdminAuthenticated")`)
- Return 401 if not authenticated
- Use `[IgnoreAntiforgeryToken]` attribute (API endpoints called via fetch)
- Have proper error handling

---

### Stage B: UI — Wizard Redesign

#### 2.3. Replace current modal in `TableMappings/Index.cshtml`

Replace the current `<!-- Add/Edit Modal -->` section (lines ~385-473) with a professional Wizard Modal.

**Modal specs:**
- Width: `max-width: 960px` (wider than current 560px)
- Max height: `90vh` with overflow-y scroll
- Backdrop blur effect
- Smooth step transitions (CSS transform/opacity)
- ESC key closes wizard (with confirmation if in progress)
- Click outside does NOT close (to prevent data loss)

**Wizard Steps:**

##### Step 1: Source Type Selection
Three large clickable cards in a row:
1. **جدول Oracle** (icon: database) — Table
2. **عرض Oracle** (icon: eye) — View
3. **استعلام مخصص** (icon: code) — Query

Each card:
- Icon + Arabic name + brief description
- Hover effect (border color change, slight scale)
- Selected state (primary color border, check mark)
- Clicking advances to Step 2

##### Step 2: Oracle Source Selection

**For Table/View:**
- Search input at top (Arabic placeholder: "ابحث عن جدول أو عرض...")
- Loading spinner while fetching from Oracle
- List of objects displayed as rows:
  - Owner badge (e.g., "NATEJSOFT")
  - Object name (monospace)
  - Column count badge
  - "مستخدم مسبقاً" badge if already mapped (red/orange)
- Click to select → advances to Step 3
- "تحديث" button to re-fetch from Oracle

**For Query:**
- Large textarea (monospace, line numbers if possible)
- Placeholder: `SELECT COLUMN1, COLUMN2 FROM TABLE WHERE ...`
- Below textarea:
  - زر **فحص الاستعلام** (validates and shows columns)
  - زر **معاينة البيانات** (shows first 10 rows)
  - زر **تنسيق** (basic SQL formatting)
- Validation status indicator:
  - ✅ صالح + عدد الأعمدة
  - ❌ خطأ + رسالة الخطأ
  - ⚠️ تحذير (مثل SELECT *)

##### Step 3: Preview & Validate

Three sub-sections (tabs or stacked):

**Tab 1: معاينة البيانات**
- Grid/table showing first 10 rows
- Column headers with Oracle type badges
- Loading skeleton while fetching
- Empty state if no data
- Error state if preview fails

**Tab 2: الأعمدة / Schema**
- Table with columns:
  | العمود | نوع Oracle | قابل للـ Null | النوع المقترح في SQL Server |
- Type mapping uses existing `OracleSchemaService.MapOracleToSqlServer()`
- Highlight any columns that might be problematic (BLOB, CLOB, very large VARCHAR2)

**Tab 3: مقارنة الهدف** (shown only if SQL Target is filled)
- If target table exists in SQL Server: show diff (add/modify/remove)
- If target doesn't exist: show "سيتم إنشاء جدول جديد"
- Uses existing `CompareSchemasAsync()`

##### Step 4: SQL Target Configuration

- **اسم الجدول الهدف** input
  - Auto-suggested name based on source:
    - `NATEJSOFT.WAREHOUSE_STOCK` → `stg_WAREHOUSE_STOCK`
    - `NATEJSOFT.VW_STOCK` → `stg_VW_STOCK`
    - Query → `stg_QUERY_<timestamp>` or user-chosen name
  - User can override the suggestion
  - Validation: no spaces, no special chars, unique in TableMappings
- **الـ Schema** dropdown: `dbo` (default) or other
- **خيارات الحفظ:**
  - ☐ إنشاء الجدول تلقائياً إذا غير موجود
  - ☐ تطبيق المخطط بعد الحفظ
- Summary card showing all choices before final save

**Final save button:** "حفظ التعيين"

#### 2.4. Edit Mode

The wizard should also work for EDIT (existing mapping):
- Pre-fill all steps with existing data
- Step 1: Show current source type (selected)
- Step 2: Show current source (selected)
- Step 3: Show current schema comparison
- Step 4: Show current target + allow changes

---

### Stage C: JavaScript — `table-mapping-wizard.js`

Create `wwwroot/js/table-mapping-wizard.js` with class `TableMappingWizard`.

**Responsibilities:**
- Step navigation (next/prev/jump)
- API calls to OracleBrowser endpoints
- Search/filter for table/view list
- Query validation flow
- Data preview rendering
- Column metadata rendering
- Schema diff rendering
- SQL target name auto-suggestion
- Form submission (Add/Edit)
- Loading states / error states / empty states
- Keyboard navigation (ESC to close, Enter to advance)

**API endpoints used:**
```
GET  /api/oracle-browser/list-objects?type=table
GET  /api/oracle-browser/list-objects?type=view
POST /api/oracle-browser/preview     { source, sourceType, limit }
POST /api/oracle-browser/validate-query { query }
```

---

## 3. Allowed Write Targets

```
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Services\OracleSchemaService.cs
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\OracleBrowser\ListObjects.cshtml
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\OracleBrowser\ListObjects.cshtml.cs
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\OracleBrowser\Preview.cshtml
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\OracleBrowser\Preview.cshtml.cs
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\OracleBrowser\ValidateQuery.cshtml
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\OracleBrowser\ValidateQuery.cshtml.cs
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\TableMappings\Index.cshtml
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\TableMappings\Index.cshtml.cs
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\table-mapping-wizard.js
```

---

## 4. Acceptance Criteria

- [ ] `dotnet build -c Release` = 0 errors / 0 warnings
- [ ] Wizard opens when clicking "إضافة تعيين"
- [ ] Step 1: Three source type cards render correctly
- [ ] Step 2 (Table): Oracle tables list loads from live Oracle, searchable
- [ ] Step 2 (View): Oracle views list loads from live Oracle, searchable
- [ ] Step 2 (Query): Editor validates query, shows column metadata
- [ ] Step 3: Data preview shows first 10 rows from Oracle
- [ ] Step 3: Column metadata table shows Oracle type → SQL Server type mapping
- [ ] Step 3: Schema comparison shows diff (if target table exists)
- [ ] Step 4: SQL target name auto-suggested
- [ ] Step 4: Save creates/updates mapping in DB
- [ ] Edit mode: existing mapping pre-fills wizard correctly
- [ ] Already-mapped sources show warning badge
- [ ] Error states: Oracle connection failure, invalid query, preview failure
- [ ] Loading states: spinner/skeleton during API calls
- [ ] ESC closes wizard (with confirmation if in progress)
- [ ] All Arabic labels correct, RTL layout works
- [ ] Admin session check on all API endpoints (401 if not authenticated)

---

## 5. Not in Scope (Phase 2)

- Visual relationship designer between mappings
- Parent/Child mapping relationships
- Join key configuration
- ERD/Graph visualization

---

## 6. Model Capability Note

This task involves:
- Oracle database integration (complex, real data)
- Multi-step UI wizard (moderate complexity)
- Multiple API endpoints (moderate)
- Live data preview (sensitive — short timeouts, error handling)

Recommended model tier: **Strong** (risk of Oracle connection issues, multi-file coordination needed).
