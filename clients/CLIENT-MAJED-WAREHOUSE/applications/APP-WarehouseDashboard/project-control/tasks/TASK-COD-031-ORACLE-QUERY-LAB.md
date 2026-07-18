# TASK-COD-031: Oracle Query Lab

> **Status:** Submitted
> **Assigned To:** EngineeringAgent
> **Created:** 2026-07-18
> **Phase:** 6 Implementation
> **Priority:** High

---

## Objective

Create a new admin page "مختبِّر استعلامات Oracle" (Oracle Query Lab) that allows the admin to write, test, and validate Oracle SQL queries before assigning them to dashboard cards. The page connects directly to the Oracle database (not SQL Server) and provides a complete query testing environment.

---

## Scope

### In Scope
1. **SQL Editor** — monospace textarea for writing Oracle SQL
2. **Execute Button** — runs query against Oracle, shows loading state
3. **Results Grid** — dynamic table rendering with auto-detected columns
4. **Execution Info** — row count + elapsed time display
5. **Read-Only Guard** — server-side protection blocking DML/DDL
6. **Saved Queries** — localStorage-based save/load/delete of frequently used queries
7. **Schema Browser** — sidebar/panel showing Oracle tables and their columns

### Out of Scope
- Query syntax highlighting (future enhancement)
- Query auto-complete (future enhancement)
- Export to file (copy to clipboard is sufficient)
- Query sharing between users

---

## Files to Create

| File | Purpose |
|---|---|
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/OracleQueryLab/Index.cshtml` | Razor page UI |
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/OracleQueryLab/Index.cshtml.cs` | Page model + handlers |
| `src/WarehouseDashboard.Web/Infrastructure/OracleReadonlyGuard.cs` | Server-side read-only guard for Oracle SQL |

## Files to Modify

| File | Change |
|---|---|
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Index.cshtml` | Add OracleQueryLab navigation card |

---

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\OracleQueryLab\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\OracleQueryLab\Index.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Infrastructure\OracleReadonlyGuard.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Index.cshtml
```

---

## Technical Design

### Backend (Index.cshtml.cs)

**Namespace:** `WarehouseDashboard.Web.Pages.AdminSecurePanel.OracleQueryLab`

**Dependencies:**
- `IConfiguration` — for Oracle connection string
- `ILogger<OracleQueryLabModel>` — for error logging

**Handlers:**

1. `OnGet()` — renders the page
2. `OnPostRunAsync([FromBody] OracleQueryRunRequest)` — executes Oracle query
3. `OnPostSchemaAsync()` — returns list of Oracle tables + columns

**Oracle Connection:**
```csharp
using var connection = new OracleConnection(ConnectionStringHelper.ResolveOracle(_configuration));
```

**Query Execution Rules:**
- Use `OracleCommand` with `CommandTimeout = 30`
- Max rows returned: 10,000 (use `OracleDataReader` with manual row counting)
- Read-only guard: `OracleReadonlyGuard.IsReadOnly(sql)` before execution
- Return JSON: `{ success, columns[], rows[], rowCount, elapsedMilliseconds, errorMessage }`

**Schema Browser Handler (`OnPostSchemaAsync`):**
```sql
-- Get tables
SELECT OWNER, TABLE_NAME FROM ALL_TABLES WHERE OWNER = USER ORDER BY TABLE_NAME

-- Get columns for a table
SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE, DATA_DEFAULT
FROM ALL_TAB_COLUMNS
WHERE OWNER = USER AND TABLE_NAME = :tableName
ORDER BY COLUMN_ID
```

**Column Info Response:**
```csharp
public class OracleColumnInfo
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public int? DataLength { get; set; }
    public string Nullable { get; set; }
}
```

### Frontend (Index.cshtml)

**Layout:** Use `admin-secure-panel/_ViewStart.cshtml` layout (consistent with other admin pages)

**Page Structure:**
```
┌─────────────────────────────────────────────────┐
│ Breadcrumb: لوحة الإدارة « مختبِّر استعلامات Oracle │
│ Title: مختبِّر استعلامات Oracle                     │
│ Subtitle: اختبار استعلامات Oracle قبل إسنادها للبطاقات │
├─────────────────────────────────────────────────┤
│ ┌──────────────────────────┐ ┌────────────────┐ │
│ │ [SQL Editor Section]     │ │ [Schema Panel] │ │
│ │ ┌──────────────────────┐ │ │ Tables list    │ │
│ │ │ textarea (monospace) │ │ │ Click → columns│ │
│ │ └──────────────────────┘ │ │                │ │
│ │ [تشغيل] [مسح] [Saved ▼]  │ │                │ │
│ │ Exec info: X rows, Y ms  │ │                │ │
│ └──────────────────────────┘ └────────────────┘ │
├─────────────────────────────────────────────────┤
│ [Results Grid]                                   │
│ Dynamic table with columns from query result     │
└─────────────────────────────────────────────────┘
```

**UI Components:**

1. **SQL Editor Card:**
   - Label: "استعلام Oracle SQL"
   - Textarea: `direction: ltr; font-family: "Courier New", monospace; min-height: 160px`
   - Placeholder: `"SELECT * FROM TABLE_NAME WHERE ROWNUM <= 100"`
   - Badge: "Oracle" (green badge to distinguish from SQL Server tester)

2. **Action Bar:**
   - Button "تشغيل" (primary) — `onclick="runQuery()"`
   - Button "مسح" (ghost) — `onclick="clearEditor()"`
   - Dropdown "استعلامات محفوظة" (ghost) — populated from localStorage
   - Button "حفظ" (ghost) — saves current query to localStorage
   - Span `#meta` — shows row count + elapsed time

3. **Schema Browser Panel (right side on desktop, below on mobile):**
   - Header: "مخطط قاعدة البيانات"
   - Search/filter input for tables
   - List of table names (clickable)
   - When table clicked: shows columns below with data types
   - Click on column name → inserts into editor at cursor position

4. **Results Grid:**
   - Same pattern as existing QueryTester (wd-table styling)
   - Dynamic columns from query result
   - Number alignment: right
   - String alignment: left
   - Alternating row backgrounds
   - Empty state: "نفِّذ استعلاماً لعرض النتائج هنا."
   - Loading skeleton during execution

5. **Toast Notifications:**
   - Success: green
   - Error: red
   - Warning: amber

**JavaScript Functions:**

```javascript
// Core
async function runQuery()           // Execute Oracle query via POST ?handler=Run
function clearEditor()              // Clear editor + results
function renderGrid(columns, rows)  // Render results table
function mapType(clrType)           // Map Oracle CLR types to alignment hints

// Schema Browser
async function loadSchema()         // Fetch table list via POST ?handler=Schema
async function loadTableColumns(tableName)  // Fetch columns for a table
function insertAtCursor(text)       // Insert text at textarea cursor position

// Saved Queries
function loadSavedQueries()         // Load from localStorage
function saveCurrentQuery()         // Save to localStorage
function deleteSavedQuery(index)    // Delete from localStorage
function loadSavedQuery(index)      // Load saved query into editor
```

**CSS:** Inline `<style>` block (consistent with other admin pages). Use existing CSS variables:
- `--c-text`, `--c-text-muted`, `--c-surface`, `--c-surface-muted`
- `--c-border`, `--c-primary`, `--c-secondary`, `--c-success`, `--c-error`
- `--radius-md`, `--radius-lg`
- `--shadow-sm`, `--shadow-md`
- `--dur-fast`, `--dur-norm`, `--ease`
- `--font-ar`

### OracleReadonlyGuard.cs

**Location:** `src/WarehouseDashboard.Web/Infrastructure/OracleReadonlyGuard.cs`

**Blocked Keywords (first-word check):**
```
INSERT, UPDATE, DELETE, MERGE, DROP, CREATE, ALTER, TRUNCATE,
EXEC, EXECUTE, GRANT, REVOKE, DENY, BACKUP, RESTORE, BULK,
USE, SHUTDOWN, KILL, COMMIT, ROLLBACK, SAVEPOINT, LOCK, EXPLAIN
```

**Allowed Patterns:**
- Statements starting with `SELECT`
- Statements starting with `WITH` (CTE) that contain `SELECT`
- Statements starting with `(` (subquery wrapper)

**Rules:**
- Strip comments (`--` and `/* */`) before analysis
- Strip string literals (`'...'`) before analysis
- Split on `;` and check each statement
- Return `{ bool isReadOnly, string reason }`

---

## Acceptance Criteria

1. ✅ Page loads at `/admin-secure-panel/OracleQueryLab`
2. ✅ Navigation card appears in admin panel linking to OracleQueryLab
3. ✅ SQL editor accepts Oracle SQL queries
4. ✅ "تشغيل" button executes query against Oracle and displays results
5. ✅ Results grid renders with correct columns and data
6. ✅ Execution info shows row count and elapsed time
7. ✅ Read-only guard blocks INSERT/UPDATE/DELETE/MERGE/DDL
8. ✅ Saved queries persist in browser localStorage
9. ✅ Saved queries can be loaded, deleted
10. ✅ Schema browser shows Oracle tables
11. ✅ Clicking a table shows its columns with data types
12. ✅ Clicking a column name inserts it into the editor
13. ✅ Schema browser has search/filter for tables
14. ✅ Loading skeleton appears during query execution
15. ✅ Toast notifications for success/error
16. ✅ Empty state message when no query executed
17. ✅ Copy results to clipboard (CSV format)
18. ✅ Mobile responsive layout
19. ✅ No compilation errors (Web project builds successfully)

---

## Reference Files

| File | Why |
|---|---|
| `Pages/admin-secure-panel/QueryTester/Index.cshtml` | UI pattern reference (SQL Server version) |
| `Pages/admin-secure-panel/QueryTester/Index.cshtml.cs` | Backend pattern reference |
| `Infrastructure/SqlReadonlyGuard.cs` | Guard pattern to adapt for Oracle |
| `Infrastructure/ConnectionStringHelper.cs` | `ResolveOracle()` method |
| `Pages/admin-secure-panel/Index.cshtml` | Navigation card pattern |
| `appsettings.json` | Oracle connection string already configured |

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Single page + guard + nav update = one cohesive feature |
| One objective only | PASS | Oracle Query Lab is one clear feature |
| No deferrable work included | PASS | All items required for the feature |
| No UI unless explicitly requested | PASS | User explicitly requested a new screen |
| No API unless explicitly requested | PASS | Handlers are part of the page, not standalone API |
| No Auth unless explicitly requested | PASS | Uses existing admin auth middleware |
| No schema/migration unless explicitly requested | PASS | No DB changes needed |
| No real secrets outside approved local environment files | PASS | Connection string from config, not hardcoded |
| Secret handling plan documented and redacted | PASS | Uses `ConnectionStringHelper.ResolveOracle()` |
| CLI side effects checked | PASS | No CLI commands needed |
| No internal contradiction between constraints and outputs | PASS | |
| Allowed Write Targets are narrow | PASS | 4 specific files only |
| Acceptance criteria are testable | PASS | All 19 criteria are verifiable |

**Gate Status:** PASS

---

## Vitality & Polish Checklist

- [ ] ✅ Skeleton Loading — loading state during query execution
- [ ] ✅ Toast Notifications — success, error, warning feedback
- [ ] ✅ Empty States — editor empty state + results empty state
- [ ] ✅ Realistic Data — placeholder query and sample content
- [ ] N/A — Connection Status Indicator (not applicable for page-level)
- [ ] N/A — Search (schema browser has search, not applicable for main grid)
- [ ] N/A — Micro-animations (not critical for tool page)
