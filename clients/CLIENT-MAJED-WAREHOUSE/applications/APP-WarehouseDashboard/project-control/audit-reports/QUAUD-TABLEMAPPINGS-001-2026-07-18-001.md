# QUAUD Report: TableMappings Page — Full Production Audit

**Audit ID:** QUAUD-TABLEMAPPINGS-001-2026-07-18-001  
**Task Reviewed:** AUDIT-TABLEMAPPINGS-001 — Full Risk-Based audit of TableMappings admin page  
**Invoked By:** TeraAgent  
**Audit Mode:** Full Risk-Based  
**Scope:** All files: Index.cshtml, Index.cshtml.cs, table-mapping-wizard.js, TableMappingConfig.cs, TableMapping.cs (API model), Active.cshtml.cs, ListObjects.cshtml.cs, Preview.cshtml.cs, ValidateQuery.cshtml.cs, TableMappingController.cs, OracleSchemaService.cs, SchemaManagementService.cs  

**Report Path:** `project-control/audit-reports/QUAUD-TABLEMAPPINGS-001-2026-07-18-001.md`  
**Evidence Sources Used:** All 12 source files read in full (see Scope); QUALITY_GATE_THRESHOLDS.md

---

## Overall Quality Gate: **BLOCKED**

Two `STOP` severity findings block production release. Multiple `CAUTION` findings require fix or explicit waiver before go-live.

---

## Findings Summary

| Severity | Count | Blocking |
|---|---|---|
| **STOP** | 2 | Gate is BLOCKED |
| **CAUTION** | 12 | Requires fix or waiver |
| **FLAG** | 14 | Advisory — does not block |
| **BASELINE_DEBT** | 0 | (first audit, no baseline) |

---

## STOP Findings

### F-001: Missing Anti-Forgery Token on All POST Forms (STOP)

- **Rule ID:** QG-SEC-001 / QG-SEC-002  
- **Domain:** Security — CSRF  
- **Severity:** `STOP`  
- **Location:** `Index.cshtml` lines 522, 563, 569, 576, 897  
- **Evidence:**
  - The hidden POST form at line 897 (`<form id="wm-form" method="post" style="display:none;">`) contains **no `@Html.AntiForgeryToken()`** field. The form is submitted via JS (`wizard.save()`) for Add/Edit operations.
  - Three inline forms (Toggle, PreviewSchema, ApplySchema, Delete) at lines 522-583 also lack anti-forgery tokens.
  - By default, ASP.NET Core Razor Pages auto-validate anti-forgery tokens on all `POST` handler requests.
  - Without the token: either all POST submissions fail silently (400 Bad Request), or if auto-validation is disabled globally, the endpoints are vulnerable to CSRF attacks.
- **Impact:** Either the wizard save/edit is completely broken (form submits but handler returns 400) or the application has a CSRF vulnerability.  
- **Recommended Action:** Add `@Html.AntiForgeryToken()` inside the hidden form (line 897) and inside each inline form (Toggle, PreviewSchema, ApplySchema, Delete). For JS-based submission, the token must be included in the form or as a request header.  
- **Confidence:** High  
- **Blocking:** Yes — **STOP**  
- **Waiver Allowed:** No  
- **Required Owner:** TeraAgent / Majed

### F-002: No Column-Level Type Mapping Customization — Missing Entire Feature (STOP)

- **Rule ID:** QG-SEC-003 (unable to configure data type constraints for safety)  
- **Domain:** Data Model / Schema Governance  
- **Severity:** `STOP`  
- **Location:**  
  - `TableMappingConfig.cs` — entity has no column-level mapping data  
  - `OracleSchemaService.cs` lines 280-332 (`MapOracleToSqlServer`) — hard-coded static mapping  
  - `table-mapping-wizard.js` lines 666-689 (`renderColumnMetadata`) — read-only display of auto-mapped types  
  - `Index.cshtml` — no UI for column type override in any of the 5 wizard steps  
- **Evidence:**
  - The `TableMappingConfig` model stores only table-level data: `OracleSource`, `SqlTargetTable`, `SourceType`, `SyncMode`, `IncrementalColumn`. There is no `ColumnMappings` collection/navigation property, no `ColumnOverride` table, and no serialized column-level configuration.
  - `OracleSchemaService.MapOracleToSqlServer()` is a static method with a `switch` statement that maps Oracle types to SQL Server types programmatically. The user has **no way to override**:
    - Which SQL Server data type a column maps to
    - Column nullability (inherited blindly from Oracle)
    - Column size/precision
    - Whether to use NVARCHAR vs VARCHAR
  - The wizard Step 3 (Columns tab) renders a **read-only table** showing Oracle type → auto-mapped SQL Server type with no editable controls.
  - Type mapping edge cases that cannot be handled:
    - NUMBER without precision → `FLOAT` (approximate, data loss risk for integer/currency data)
    - NUMBER(2,5) → `DECIMAL(2,5)` (SQL Server requires precision >= scale, this would fail)
    - TIMESTAMP(9) → DATETIME2 (microsecond/nanosecond precision loss)
    - CLOB > 2GB → NVARCHAR(MAX) (overflow risk for very large CLOB data)
    - BLOB > 2GB → VARBINARY(MAX) (overflow risk)
- **Impact:** The feature the user explicitly requested (column-level type mapping customization) does not exist. This is not a minor gap — the entire data pipeline depends on automatic type inference that cannot be corrected by the user. For a production ETL tool, this is a fundamental missing capability.  
- **Recommended Action:** A new `ColumnMapping` entity/tracking system must be designed and implemented. Minimum requirements:
  1. A `ColumnMappings` table with columns: `MappingId`, `TableMappingConfigId`, `OracleColumnName`, `SqlColumnName`, `SqlDataType`, `SqlMaxLength`, `SqlPrecision`, `SqlScale`, `IsNullable`, `IsExcluded`, `DefaultValue`, `TransformationExpression`
  2. UI: A column-level editor in Step 3 or as a sub-step allowing per-column override of SQL Server type, name, nullability, exclusions, and simple transformations
  3. The schema generation (`GenerateCreateTableSql`, `GenerateAlterStatements`) must respect the overrides
  4. The schema comparison (`CompareSchemasAsync`) must use the overridden SQL types, not the raw Oracle types
- **Confidence:** High  
- **Blocking:** Yes — **STOP** (core user requirement missing)  
- **Waiver Allowed:** No (unless the system is only ever used for trivial pass-through mappings with no type concerns)  
- **Required Owner:** TeraAgent / EngineeringAgent

---

## CAUTION Findings

### F-003: SQL Server Column Names Forced to Match Oracle Column Names (CAUTION)

- **Domain:** Data Model  
- **Location:** `OracleSchemaService.cs` lines 236-258 (`CompareSchemasAsync`), `SchemaManagementService.cs` lines 171-187 (`GenerateCreateTableSql`)  
- **Evidence:** Column identity is matched by `ColumnName` (case-insensitive). In `GenerateCreateTableSql`, the Oracle `col.ColumnName` is used directly as the SQL Server column name. There is no `SqlColumnName` override in `ColumnInfo`.  
- **Impact:** If the Oracle column is `ORD_DATE` but the business expects `OrderDate`, there is no way to rename. If Oracle columns are renamed upstream, all downstream references break.  
- **Recommended Action:** Add `SqlColumnName` to `ColumnInfo` (or a new `ColumnMapping` model) with Oracle→SQL name mapping. Default to Oracle name, allow user override.  
- **Confidence:** High

### F-004: No Column Exclusion / Skip Mechanism (CAUTION)

- **Domain:** Data Model  
- **Location:** `OracleSchemaService.cs` lines 236-258, `SchemaManagementService.cs` lines 171-187  
- **Evidence:** `CompareSchemasAsync` compares ALL Oracle columns against ALL SQL Server columns. `GenerateCreateTableSql` includes every column. There is no way to mark a column as "excluded from sync."  
- **Impact:** Users cannot skip sensitive or unnecessary columns (e.g., audit columns, password hashes, large BLOBs that are not needed in the warehouse).  
- **Recommended Action:** Add `IsExcluded` flag to a `ColumnMapping` model; exclude flagged columns from CREATE TABLE, ALTER TABLE, and sync SELECT.  
- **Confidence:** High

### F-005: No Data Transformation Pipeline (CAUTION)

- **Domain:** Data Model / Sync Engine  
- **Location:** No transformation infrastructure exists in any reviewed file.  
- **Evidence:** The data flows Oracle → SQL Server as raw column values. `OracleSchemaService.PreviewDataAsync` just reads rows. `SchemaManagementService` only handles DDL, not DML. The `SyncEngineService` (not reviewed but referenced) presumably does direct bulk copy. There is no `TransformationExpression` or `DerivedColumn` concept.  
- **Impact:** Users cannot:
  - Hash or mask sensitive columns
  - Concatenate/split columns
  - Apply default values
  - Perform type casting beyond the automatic mapping
  - Add computed/dervied columns (e.g., `FullName = FirstName + ' ' + LastName`)  
- **Recommended Action:** Add a `TransformationExpression` column to `ColumnMapping` (or a separate `ColumnTransformation` entity) allowing simple SQL expressions or configurable transformations (e.g., `HASH`, `CONCAT`, `COALESCE`, `SUBSTRING`).  
- **Confidence:** High

### F-006: Hard Delete on Admin Page vs Soft Delete on API — Inconsistency (CAUTION)

- **Domain:** Data Integrity  
- **Location:** `Index.cshtml.cs` line 339 (`_db.TableMappings.Remove(mapping)`) vs `TableMappingController.cs` lines 347-383 (UPDATE IsActive=0)  
- **Evidence:** The admin page performs a hard `Remove()` + `SaveChangesAsync()` which permanently deletes the mapping row. The REST API performs an `UPDATE IsActive=0` soft-delete. These are inconsistent. A hard delete:
  - Loses the audit trail (no record of the mapping ever existing)
  - Breaks foreign key references if Card Builder or SyncLogs reference the deleted row
  - Cannot be undone  
- **Impact:** Accidental deletion from the admin page causes permanent data loss.  
- **Recommended Action:** Change the admin page to use soft-delete (set `IsActive=0` and possibly add a `DeletedAt` timestamp). Add a "restore" capability.  
- **Confidence:** High

### F-007: Uniqueness Checks Too Granular — Prevents Legitimate Use Cases (CAUTION)

- **Domain:** Data Validation  
- **Location:** `Index.cshtml.cs` lines 100-122  
- **Evidence:**  
  - Line 108: `AnyAsync(m => m.OracleSource == OracleSource)` — prevents the SAME Oracle source from being used in multiple mappings
  - Line 116: `AnyAsync(m => m.SqlTargetTable == SqlTargetTable)` — prevents the SAME SQL target from being used in multiple mappings
  - The combination `(OracleSource, SqlTargetTable)` is never checked together  
- **Impact:** A user cannot map the same Oracle table to two different SQL targets (for splitting data), or map two different Oracle tables to the same SQL target (for consolidation).  
- **Recommended Action:** Change uniqueness to check `(OracleSource, SqlTargetTable)` as a composite key instead of each field separately.  
- **Confidence:** High

### F-008: Schema Change SQL Not Previewed Before Execution (CAUTION)

- **Domain:** Production Readiness / UX  
- **Location:** `SchemaManagementService.cs` lines 107-142 (`GenerateAlterStatements`) vs page preview at `Index.cshtml` lines 592-653  
- **Evidence:** `GenerateAlterStatements()` exists and generates the actual `ALTER TABLE ... ADD/ALTER/DROP` SQL, but this method is **never called for display**. The `PreviewSchemaAsync` handler only shows a human-readable list of columns to add/modify/remove, not the actual SQL statements. The `ApplySchemaAsync` handler executes the SQL without showing it to the user first.  
- **Impact:** Users must trust the system is generating correct SQL. They cannot manually verify the ALTER statements before execution. For destructive operations (DROP COLUMN), this is especially dangerous.  
- **Recommended Action:** Call `GenerateAlterStatements()` and display the SQL in the preview. Add a confirmation step showing the exact SQL before applying.  
- **Confidence:** High

### F-009: Backward-Incompatible Schema Changes Can Cause Data Loss (CAUTION)

- **Domain:** Production Readiness  
- **Location:** `SchemaManagementService.cs` lines 107-142 (DROP COLUMN), `ApplySchemaChangesAsync` lines 56-101  
- **Evidence:**  
  - Columns are dropped unconditionally: `ALTER TABLE {safeName} DROP COLUMN [{colName}];`
  - The diff includes `ColumnsToRemove` which flags columns in SQL Server that are no longer in Oracle
  - No backup of dropped column data is taken
  - No confirmation of data loss severity is shown
  - DDL changes are in a transaction, but the transaction only protects the DDL atomicity, not the data  
- **Impact:** If the Oracle schema changes (column removed upstream), the SQL Server column is dropped with permanent data loss.  
- **Recommended Action:**  
  1. Show data loss warning with row count per column before dropping
  2. Offer to archive data before dropping (e.g., INSERT INTO backup table)
  3. Consider soft-deleting columns (mark as unused) instead of hard DROP
  4. Require explicit user acknowledgment for destructive operations  
- **Confidence:** High

### F-010: Edit Mode Preview Not Reliably Loaded (CAUTION)

- **Domain:** UX / Data Integrity  
- **Location:** `table-mapping-wizard.js` lines 144-146 (`bootstrapEditMode`)  
- **Evidence:**  
  - Line 144-146: `if (this.state.sourceType && this.state.oracleSource) { this.loadPreview(this.state.oracleSource, this.state.sourceType); }` is called during `init()` which fires at page load, before the wizard is opened.
  - `openWizardForEdit()` calls `wizard.resetState()` which clears `previewResult`, THEN calls `wizard.open(editData)` which calls `bootstrapEditMode` which calls `loadPreview` asynchronously.
  - When the user navigates to Step 3, the preview may not have loaded yet.
  - No loading state is shown for the preview in edit mode on initial wizard open.
  - For Query mode edits, the full Oracle query is re-executed which could be expensive.
- **Impact:** Users editing an existing mapping may see stale or empty preview data.  
- **Recommended Action:** Ensure preview is loaded synchronously (or with clear loading indicator) before the user can access Step 3 in edit mode. Cache preview results for Query mode to avoid re-executing expensive queries.  
- **Confidence:** Medium

### F-011: SQL Injection Risk via String Concatenation in Schema Introspection (CAUTION)

- **Rule ID:** QG-SEC-003  
- **Domain:** Security  
- **Location:** `OracleSchemaService.cs` lines 63-74, 457-461  
- **Evidence:**  
  - Lines 63-74: `$"WHERE TABLE_NAME = '{EscapeSql(tableName)}'"` — the table/owner name is concatenated into the SQL string with only `EscapeSql` (single-quote doubling). While the table name comes from the database listing (not directly user-supplied), this is not parameterized.
  - Lines 457, 461: `$"SELECT * FROM ({SanitizeQuery(oracleSource)}) WHERE ROWNUM <= {limit}"` — for Query mode, `oracleSource` is the user query which is only validated as starting with `SELECT` or `WITH` (`IsReadOnlyQuery`). A query like `SELECT * FROM (SELECT ... UNION ALL SELECT ... FROM SENSITIVE_TABLE)` passes validation.
  - Lines 717-726: `SanitizeQuery` only checks if the query starts with `SELECT` or `WITH`. It does not parse or restrict the query content.
- **Impact:** For Table/View mode, risk is low since object names come from the database. For Query mode, a user could potentially write a query that accesses data they shouldn't have access to (though Oracle's own permissions would gate this).  
- **Recommended Action:**  
  1. Use parameterized queries for ALL_TAB_COLUMNS lookups instead of string concatenation
  2. For Query mode, add stricter validation (e.g., parse the query AST, restrict to specific tables using a connection with minimal permissions)
  3. Consider using a dedicated read-only Oracle connection/user for the schema browser  
- **Confidence:** Medium

### F-012: No Loading States for Initial Page Load (CAUTION)

- **Domain:** UX  
- **Location:** `Index.cshtml` lines 454-590  
- **Evidence:** The main page renders server-side, so the initial table is present on page load. However, the **schema preview panel** (line 592-653) requires a POST to `PreviewSchemaAsync` which is a full page reload. During this reload, there is no loading indicator.  
- **Impact:** Poor user experience when previewing schema — the page freezes during the POST round-trip.  
- **Recommended Action:** Convert schema preview to an AJAX call with proper loading state.  
- **Confidence:** High

### F-013: Wizard Does Not Show Schema Diff in Wizard (CAUTION)

- **Domain:** UX / Workflow  
- **Location:** `table-mapping-wizard.js` lines 691-713 (`loadSchemaDiff`)  
- **Evidence:** The schema diff tab in the wizard (Step 3) shows only a placeholder: `"سيتم عرض مقارنة المخطط بعد حفظ التعيين."` (Schema comparison will be displayed after saving the mapping). The actual schema comparison (via `CompareSchemasAsync`) is only available after the mapping is created, through the page-level "مخطط" (Schema) button.  
- **Impact:** Users cannot see what the target table will look like before they commit to creating the mapping. This is a critical workflow gap — they must create the mapping, then preview the schema, then apply it.  
- **Recommended Action:** Load the schema diff during wizard Step 3 by calling `PreviewSchemaAsync` (or a new API) once the target table name is entered. Show the comparison in the wizard before the user clicks Save.  
- **Confidence:** High

### F-014: No Support for Partitioned Tables or Indexes (CAUTION)

- **Domain:** Production Readiness  
- **Location:** `SchemaManagementService.cs` lines 171-187 (`GenerateCreateTableSql`)  
- **Evidence:** The CREATE TABLE statement includes only `[ColumnName] DataType NULL/NOT NULL` with no support for:
  - Primary keys
  - Indexes (clustered/non-clustered)
  - Partitioning scheme
  - Filegroup specification
  - Constraints beyond nullability
  - Identity columns
- **Impact:** For large tables (common in warehousing), the lack of partitioning and indexes will cause severe performance degradation in production.  
- **Recommended Action:** Add optional configuration for:
  - Primary key column selection
  - Clustered index specification
  - Partitioning scheme
  - Filegroup
  - Identity columns (e.g., surrogate keys)  
- **Confidence:** Medium

---

## FLAG Findings

### F-015: Schema Dropdown Only Contains "dbo" (FLAG)

- **Location:** `Index.cshtml` line 792-794  
- **Impact:** Users cannot select a non-dbo schema for the target table.  
- **Recommendation:** Load available schemas from SQL Server and populate the dropdown dynamically.

### F-016: No Search/Filter for Main Mapping List (FLAG)

- **Location:** `Index.cshtml` lines 484-588  
- **Impact:** With many mappings, users must scroll through the entire list.  
- **Recommendation:** Add a search box above the mapping table.

### F-017: No Bulk Selection/Action Support (FLAG)

- **Location:** `Index.cshtml` lines 484-588  
- **Impact:** Users must toggle/delete mappings one at a time.  
- **Recommendation:** Add checkboxes and bulk action buttons.

### F-018: SyncRecordCount Not Tracked in Detail (FLAG)

- **Location:** `TableMappingConfig.cs` lines 54-55  
- **Impact:** Only stores the count of the last sync. No history, no error count, no duration, no row-level stats.  
- **Recommendation:** Create a `SyncHistory` table to track per-sync metrics.

### F-019: No Undo/Rollback for Schema Changes (FLAG)

- **Location:** `SchemaManagementService.cs` lines 56-101  
- **Impact:** Once schema changes are applied, they cannot be reverted.  
- **Recommendation:** Add a schema versioning/rollback mechanism.

### F-020: TIMESTAMP(n) Precision Loss with DATETIME2 (FLAG)

- **Location:** `OracleSchemaService.cs` lines 296-300  
- **Evidence:** Oracle TIMESTAMP can have fractional seconds precision up to 9 digits (nanoseconds). DATETIME2 has 7 digits (100ns precision).  
- **Impact:** Sub-microsecond precision is lost for TIMESTAMP(9) columns.  
- **Recommendation:** Map to `DATETIME2(7)` with a documentation note about precision limits.

### F-021: NUMBER Without Precision Maps to FLOAT (FLAG)

- **Location:** `OracleSchemaService.cs` lines 339-341  
- **Evidence:** `NUMBER` with no precision → `FLOAT`. FLOAT is an approximate type; for financial or integer data this could cause rounding errors.  
- **Impact:** Potential silent data corruption for NUMBER columns without explicit precision.  
- **Recommendation:** Default to `DECIMAL(38,10)` instead of `FLOAT`, or allow user override.

### F-022: No Computed/Derived Column Support (FLAG)

- **Domain:** Data Model  
- **Location:** No infrastructure in any reviewed file  
- **Impact:** Cannot add columns like `HASH(OrderId)` or `FullName = FirstName + ' ' + LastName`.  
- **Recommendation:** Add as part of the ColumnMapping feature (see F-002).

### F-023: Generated Target Table Names May Conflict (FLAG)

- **Location:** `table-mapping-wizard.js` lines 725-744 (`suggestTargetName`)  
- **Evidence:** Name is derived from the Oracle object name with `stg_` prefix. Two different Oracle schemas with the same table name would produce the same suggested target name.  
- **Recommendation:** Include owner/schema in the suggested name (e.g., `stg_NATEJSOFT_WAREHOUSE_STOCK`).

### F-024: No Keyboard Shortcuts or Accessibility Enhancements (FLAG)

- **Location:** `Index.cshtml` (entire file)  
- **Impact:** Wizard can be exited with Escape but otherwise no keyboard navigation.  
- **Recommendation:** Add keyboard shortcuts for Save (Ctrl+Enter), Next (Ctrl+Right), Prev (Ctrl+Left).

### F-025: No Confirmation on Schema Apply with Data Loss Risk (FLAG)

- **Location:** `Index.cshtml` line 569 (`onsubmit="return confirm('هل تريد تطبيق المخطط على SQL Server؟');"`)  
- **Evidence:** The confirmation message does not mention potential data loss from column drops or type changes.  
- **Recommendation:** Include a summary of destructive changes in the confirmation dialog.

### F-026: Error Messages Are User-Facing But Not Localized Consistently (FLAG)

- **Location:** Various locations in C# and JS  
- **Impact:** Most messages are in Arabic (good), but some English messages leak through (e.g., API error responses).  
- **Recommendation:** Audit all error paths for consistent Arabic localization.

### F-027: IgnoreAntiforgeryToken on OracleBrowser API Pages (FLAG — Documented)

- **Location:** `ListObjects.cshtml.cs` line 11, `Preview.cshtml.cs` line 12, `ValidateQuery.cshtml.cs` line 12  
- **Impact:** These API endpoints use session-based admin authentication instead. As long as the session check is present, this is acceptable.  
- **Recommendation:** Add an XML comment documenting why `[IgnoreAntiforgeryToken]` is used.

### F-028: REST API Has No Authentication (FLAG — Documented Risk)

- **Location:** `TableMappingController.cs` lines 12-15  
- **Evidence:** The controller comment states: "SECURITY (Phase 1, internal API): these endpoints intentionally have NO authentication."  
- **Impact:** If IIS IP & Domain Restrictions are not configured, the API is fully open.  
- **Recommendation:** Add API key or bearer token authentication even for internal APIs.

---

## Baseline Debt (None Identified)

No baseline debt findings — this is the first audit of this component.

---

## Critical Questions Answered

| # | Question | Answer |
|---|---|---|
| 1 | Where does the column type mapping happen? | `OracleSchemaService.MapOracleToSqlServer()` (line 280) — a static switch statement with no override. |
| 2 | Is the type mapping overrideable per column? | **No.** Not at all. This is the central finding (F-002). |
| 3 | Can users customize SQL Server column names separately? | **No.** SQL Server column names are forced to match Oracle column names (F-003). |
| 4 | Is there a transformation pipeline? | **No.** No mechanism for any column transformation (F-005). |
| 5 | What happens if Oracle schema changes after SQL Server table is created? | Schema diff (`CompareSchemasAsync`) + ALTER TABLE via `ApplySchemaChangesAsync`. Works, but with data loss risk (F-009). |
| 6 | Is column diff reliable for type changes? | Partially. NUMBER(10,0)→INT works, but NUMBER(12,2) would succeed. However, NUMBER(p,s) where s>p would fail on SQL Server (F-002). |
| 7 | Data type mapping edge cases causing data loss? | Several: FLOAT for imprecise NUMBER, precision/scale mismatch, CLOB/BLOB overflow, TIMESTAMP(9) precision truncation. |
| 8 | Missing `@Html.AntiForgeryToken()` like Card Builder? | **Yes.** Same issue exists here (F-001). The hidden form at line 897 has no token. |
| 9 | Does wizard properly wire up for editing existing mappings? | Partially (F-010). Edit mode exists but preview data may not load reliably. |
| 10 | SQL injection risks in dynamic SQL? | **Yes** (F-011). Query validation is minimal (only checks SELECT/WITH prefix). Table/View names use string concatenation with only single-quote escaping. |

---

## Prioritized Remediation

### Must Fix Before Production (BLOCKED)
1. **F-001** — Anti-forgery tokens on all POST forms (blocks wizard save)
2. **F-002** — Column-level type mapping customization (core user requirement)

### Should Fix Before Production (CAUTION — needs waiver)
3. **F-003** — SQL column name override
4. **F-004** — Column exclusion capability
5. **F-005** — Transformation pipeline
6. **F-006** — Soft-delete consistency
7. **F-007** — Correct uniqueness checks
8. **F-008** — SQL statement preview
9. **F-009** — Data loss protection for DROP COLUMN
10. **F-011** — SQL injection hardening
11. **F-013** — Schema diff in wizard
12. **F-014** — Partitioning/index support

### Nice to Have
13. F-010 — Edit mode preview reliability
14. F-012 — Loading states
15. F-015 through F-028 — All FLAG findings

---

## Handback to TeraAgent

- **Status:** `BLOCKED` — Two STOP findings prevent production release
- **Report Path:** `project-control/audit-reports/QUAUD-TABLEMAPPINGS-001-2026-07-18-001.md`
- **Blocking Findings:** F-001 (Missing Anti-Forgery Token) and F-002 (No Column-Level Type Mapping)
- **Recommended Next Action:**
  1. **Immediately**: Fix F-001 (add anti-forgery tokens) — this is a well-understood, quick fix
  2. **Design Phase**: F-002 requires a new `ColumnMapping` entity and UI — significant design and implementation effort. Create a separate task (TASK-COD-NNN) for this feature.
  3. **Review**: Address CAUTION findings F-003 through F-009 as part of the same feature task or separate follow-ups.
  4. **After fixes**: Request re-audit for verification.

