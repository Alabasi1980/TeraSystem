# QUAUD Report — ReportBuilder & Reports System Comprehensive Technical Audit

**Audit ID:** QUAUD-REPORTBUILDER-COMPREHENSIVE-2026-07-21
**Task Reviewed:** AUDIT-REPORTBUILDER-COMPREHENSIVE
**Invoked By:** TeraAgent (بطلب مباشر من Majed)
**Audit Mode:** Full Risk-Based (تدقيق شامل معمق)
**Scope:** ReportBuilder + Reports Page + ReportService + API Endpoints + Models + Migrations
**Report Path:** project-control/audit-reports/QUAUD-REPORTBUILDER-COMPREHENSIVE-2026-07-21.md
**Evidence Sources:** Direct code review of 12 source files, 2 migration files, 2 previous audit reports, 3 governance files

---

## Executive Summary

تم إجراء تدقيق تقني شامل وعميق على نظام التقارير بكامله — من صفحة منشئ التقارير (ReportBuilder) إلى صفحة عرض التقارير (Reports) وخدمة التقارير الخلفية (ReportService) وواجهات API وإدارة قواعد البيانات.

**النتيجة الإجمالية: NEEDS_FIX**

### الإنجازات الإيجابية
1. ✅ **تم إصلاح F-01 من التدقيق السابق** — ReportFilterDto الآن يتضمن ValueColumn/TextColumn ويتم تعيينهما في CreateReportAsync و UpdateReportAsync
2. ✅ هيكلة قاعدة البيانات سليمة — العلاقات مع CASCADE صحيحة، و Down() هي inverse لـ Up() في كل migration
3. ✅ نظام الـ Toast notifications موجود ويعمل في كلتا الصفحتين (بدلاً من alert())
4. ✅ AbortController يستخدم لإلغاء الطلبات في Reports page
5. ✅ escapeHtml() يستخدم في معظم الأماكن التي يتم فيها إدراج بيانات المستخدم
6. ✅ Admin session check موجود في جميع API endpoints
7. ✅ Skeleton loading موجود في Reports page

### المخاطر الرئيسية
1. 🔴 ثغرة SQL injection محتملة في OnPostExecuteQueryAsync — تنفذ استعلامات SQL مباشرة من الـ request body
2. 🔴 ثغرة SQL injection في OnPostPreviewAsync — viewName يُبنى في الاستعلام بدون parameterization كافٍ
3. 🔴 ثغرة XSS في unPreview() — رسالة الخطأ تُدرج في innerHTML بدون escapeHtml
4. 🔴 كشف تفاصيل الأخطاء التقنية للمستخدم في 3 endpoints
5. 🔴 ReportService.cs (956 سطر) يقوم بـ 5+ مسؤوليات مختلفة
6. 🔴 15 DTO في نفس ملف ReportService.cs
7. 🔴 450+ سطر JavaScript مضمّن في ReportBuilder.cshtml و 785+ سطر في Reports.cshtml

---

## Overall Quality Gate: NEEDS_FIX

| Condition | Met? |
|---|---|
| Any open STOP | No |
| Any open CAUTION | **Yes (7 CAUTION)** |
| Only FLAG or resolved | No |

---

## Findings Summary

| Severity | Count |
|---|---|
| STOP | 0 |
| CAUTION | 7 |
| FLAG | 11 |
| BASELINE_DEBT | 2 |
| RESOLVED (from previous) | 1 |

---

## 🔴 CAUTION Findings

### RB-SEC-001: ExecuteQuery Endpoint — تنفيذ SQL عشوائي من الـ Request Body

- **Domain:** Security — SQL Injection
- **Severity:** CAUTION
- **Location:** Pages/Api/Reports/ReportData.cshtml.cs lines 291-340 (OnPostExecuteQueryAsync)
- **Evidence:**
  `csharp
  await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(request.Query, conn);
  `
  يقبل استعلام SQL مباشرة من request body بدون أي parameterization أو validation.
- **Expected Standard:** Even admin-only endpoints should use parameterized queries, or at minimum validate/restrict to SELECT-only queries.
- **Observed Condition:** Any SQL string sent to this endpoint is executed. While admin-auth protected, this is a significant risk surface.
- **Impact:** If admin session is compromised, attacker can execute arbitrary SQL on the SQL Server (SELECT, INSERT, UPDATE, DELETE, DROP).
- **Recommendation:**
  1. Add SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED and SET TRANSACTION READ ONLY at the start of the command
  2. Validate that only SELECT statements are allowed (basic startsWith("SELECT") check)
  3. Consider adding query logging for audit trail
  4. Add rate limiting to this endpoint
- **Confidence:** High
- **Blocking:** Yes — this is a significant security risk surface
- **Waiver Allowed:** No (per QG-SEC-003: unsafe raw query with user input)
- **Referral:** SecurityAgent for deeper analysis
- **Status:** Open

---

### RB-SEC-002: Preview Endpoint — viewName يُبنى في SQL بدون Parameterization كافٍ

- **Domain:** Security — SQL Injection
- **Severity:** CAUTION
- **Location:** Pages/Api/Reports/ReportData.cshtml.cs line 244
- **Evidence:**
  `csharp
  var sql = $"SELECT TOP {top} * FROM {request.ViewName} WITH (NOLOCK)";
  `
  iewName يأتي من request body ويدخل مباشرة في الاستعلام. فقط ParseViewName() (الذي يزيل الأقواس المربعة) يفصل بينه وبين SQL injection كامل.
- **Expected Standard:** Even when table/view names cannot be parameterized, at minimum validate that the view exists in INFORMATION_SCHEMA.VIEWS before executing.
- **Observed Condition:** No validation that the view actually exists before querying. An attacker could inject SELECT * FROM sys.objects for example.
- **Impact:** Limited by bracket stripping, but a view name like [dbo].[Users] would still pass through correctly and could expose arbitrary data.
- **Recommended Action:**
  1. Validate that the view name actually exists in INFORMATION_SCHEMA.VIEWS before execution (using the existing parameterized query in GetViewColumnsAsync or similar)
  2. Use SqlCommandBuilder.EnquoteIdentifier() or SqlConnection.GetSchema("Tables") instead of manual bracket wrapping
- **Confidence:** High
- **Blocking:** Yes — direct SQL concatenation with user input
- **Waiver Allowed:** No
- **Status:** Open

---

### RB-SEC-003: XSS Gap — رسالة الخطأ في Preview تُدرج بدون escapeHtml

- **Domain:** Security — Cross-Site Scripting (XSS)
- **Severity:** CAUTION
- **Location:** Pages/admin-secure-panel/ReportBuilder/Index.cshtml line 631
- **Evidence:**
  `javascript
  document.getElementById('previewArea').innerHTML = '<div style="text-align:center;padding:40px;color:var(--c-warning);">⚠️ ' + (data.errorMessage || 'لا توجد بيانات') + '</div>';
  `
  data.errorMessage يتم إدراجه مباشرة في innerHTML بدون استخدام escapeHtml().
- **Expected Standard:** All user-facing data from API responses must be escaped before insertion into innerHTML.
- **Observed Condition:** errorMessage from API could contain HTML/JS if the backend echoes user input in error messages.
- **Impact:** Stored/Reflected XSS — if an attacker can control data that surfaces in the error path.
- **Recommended Action:** Add escapeHtml() wrapper: escapeHtml(data.errorMessage || 'لا توجد بيانات')
- **Confidence:** High
- **Blocking:** Yes
- **Waiver Allowed:** No
- **Status:** Open

---

### RB-SEC-004: Error Messages Expose Internal Technical Details

- **Domain:** Security — Information Disclosure
- **Severity:** CAUTION
- **Locations:**
  1. Pages/Api/Reports/ReportData.cshtml.cs line 281: eturn new JsonResult(new { success = false, errorMessage = ex.Message });
  2. Pages/Api/Reports/ReportData.cshtml.cs line 338: eturn new JsonResult(new { success = false, errorMessage = ex.Message });
  3. Services/ReportService.cs line 623: esult.ErrorMessage = "حدث خطأ أثناء تنفيذ التقرير: " + ex.Message;
- **Evidence:** Raw ex.Message is returned to the client, which may contain stack traces, SQL error details, table names, column names, or connection information.
- **Expected Standard:** Error messages should be user-safe and not expose internal implementation details. Log full details server-side, return generic messages to client.
- **Observed Condition:** SQL Server error messages (like invalid column names, constraint violations) could leak schema information.
- **Impact:** Information disclosure — helps attackers understand the database schema.
- **Recommended Action:** Replace ex.Message with generic messages like "حدث خطأ أثناء تنفيذ الاستعلام." and log the full exception details.
- **Confidence:** High
- **Blocking:** Yes (cumulative — 3 locations)
- **Waiver Allowed:** No
- **Status:** Open

---

### RB-CQ-001: ReportService.cs — 956 سطر مع 5+ مسؤوليات (انتهاك SRP)

- **Domain:** Code Quality — Single Responsibility Principle
- **Severity:** CAUTION
- **Location:** Services/ReportService.cs (956 lines)
- **Evidence:** The service handles all of:
  1. View Discovery (GetAvailableViewsAsync, GetViewColumnsAsync)
  2. CRUD Operations (Create, Read, Update, Delete, Toggle)
  3. Dynamic SQL Query Builder + Execution (ExecuteReportAsync)
  4. Filter Options (GetFilterOptionsAsync, GetParameterOptionsAsync)
  5. Layout Management (SaveLayoutAsync, GetLayoutsAsync, DeleteLayoutAsync)
  6. 15 DTO classes defined in the same file (lines 805-956)
- **Expected Standard (per ENGINEERING_BEST_PRACTICES.md §5):** Files approaching 300-400 lines should be reviewed for responsibility creep. ReportService exceeds the Caution threshold of 500+ lines.
- **Observed Condition:** File is 956 lines combining data access, query construction, and DTO definitions.
- **Impact:** High maintenance cost, difficult to test, any change risks breaking unrelated functionality.
- **Recommended Action:** Split into:
  - ReportViewService.cs — View Discovery + Schema Introspection
  - ReportCrudService.cs — CRUD operations
  - ReportExecutionService.cs — Dynamic query building + execution
  - ReportLayoutService.cs — Layout management
  - Move DTOs to separate Models/Dto/ folder (e.g., ReportDtos.cs)
- **Confidence:** High
- **Blocking:** Yes (Caution threshold exceeded with multiple responsibilities)
- **Waiver Allowed:** Yes (if maintained as-is with documentation of the architectural debt)
- **Status:** Open

---

### RB-FE-001: CSS Hardcoded Colors + Wrong Layout (من التدقيق السابق — لم يُصلح بالكامل)

- **Domain:** Frontend Quality — Design System Compliance
- **Severity:** CAUTION (was STOP in previous audit)
- **Location:**
  1. Reports/Index.cshtml lines 10-290 — inline CSS with raw hex colors
  2. ReportBuilder/Index.cshtml lines 7-107 — inline CSS with raw hex colors
- **Evidence:** CSS uses raw colors like #2E6DA4, #1F4E79, #F0F4F8 instead of CSS custom properties like ar(--c-primary), ar(--c-surface-muted) defined in lue-theme.css.
- **Previous Finding:** QUAUD-REPORT-DESIGN-QUALITY-2026-07-20-001 found STOP-001 (wrong layout) and STOP-002 (hardcoded colors).
- **Observed Condition:** The Layout override issue (STOP-001) appears to be fixed — both pages now inherit from the default layout without explicit Layout override. However, hardcoded colors persist.
- **Impact:** Theme switching broken. Dark mode impossible. Design system inconsistency.
- **Recommended Action:**
  1. Replace ALL raw hex values with ar(--c-*) design tokens
  2. Extract shared inline CSS (290 + 107 = ~397 lines) into a shared stylesheet under wwwroot/css/
  3. Use spacing tokens ar(--sp-*) instead of raw pixel values
- **Confidence:** High
- **Blocking:** Yes — breaks theme system and design consistency
- **Waiver Allowed:** Yes (if theming is not prioritized)
- **Status:** Open (partially fixed)

---

### RB-CQ-002: Double API Call for Dropdown Filters (F-03 من التدقيق السابق — لم يُصلح)

- **Domain:** Performance / Redundancy
- **Severity:** CAUTION (was FLAG in previous audit; escalated due to ongoing performance impact)
- **Location:** Reports/Index.cshtml — enderFilters() line 677 + selectReport() line 512
- **Evidence:**
  1. enderFilters() calls loadDropdownOptions(f.columnName, inputId) which hits old API /api/reports-data/options
  2. Then selectReport() calls loadParameterOptions(currentReportDef.filters) which hits new API /api/reports-data/parameterOptions
  3. Both target the same dropdown elements. The second call overwrites the first.
- **Impact:** Two API calls per Dropdown filter instead of one. Performance degradation for reports with multiple Dropdown filters.
- **Previous Finding:** F-03 from QUAUD-TASK-REPORT-PARAM-2025-07-21-001 — remained open.
- **Recommended Action:** Remove or skip loadDropdownOptions() call for filters that have optionsQuery. The new loadParameterOptions() handles it.
- **Confidence:** High
- **Blocking:** Yes (escalated from FLAG due to ongoing unresolved performance issue)
- **Waiver Allowed:** Yes
- **Status:** Open

---

## 🟡 FLAG Findings

### RB-FLAG-001: 450+ سطر JavaScript مضمّن في ReportBuilder.cshtml

- **Domain:** Code Quality — Maintainability
- **Location:** ReportBuilder/Index.cshtml lines 229-702
- **Evidence:** All JavaScript logic for the 4-step wizard is inline in the cshtml file.
- **Impact:** Difficult to maintain, test, or reuse. Mixes UI structure with application logic.
- **Recommended Action:** Extract to wwwroot/js/report-builder.js
- **Confidence:** High

---

### RB-FLAG-002: 785+ سطر JavaScript مضمّن في Reports.cshtml

- **Domain:** Code Quality — Maintainability
- **Location:** Reports/Index.cshtml lines 420-1205
- **Evidence:** All JavaScript logic for AG Grid, modals, filters, layouts is inline.
- **Recommended Action:** Extract to wwwroot/js/reports-page.js. This file alone is 1205 lines total.
- **Confidence:** High

---

### RB-FLAG-003: escapeHtml() مكرر في صفحتين

- **Domain:** Code Quality — DRY
- **Locations:**
  1. ReportBuilder/Index.cshtml lines 401-406
  2. Reports/Index.cshtml lines 1199-1204
- **Evidence:** Exact same function defined in both pages.
- **Recommended Action:** Extract to a shared JS file (e.g., wwwroot/js/shared-utils.js).
- **Confidence:** High

---

### RB-FLAG-004: [IgnoreAntiforgeryToken] على جميع API endpoints

- **Domain:** Security — CSRF
- **Locations:**
  1. ReportManage.cshtml.cs line 17
  2. ReportData.cshtml.cs line 20
- **Evidence:** All POST endpoints lack CSRF protection. This is a system-wide pattern in the admin panel, not specific to reports.
- **Impact:** CSRF attacks possible if admin user visits malicious site while authenticated.
- **Recommended Action:** Evaluate if CSRF protection is needed given the admin-only + local network deployment model. Document the decision.
- **Confidence:** Medium

---

### RB-FLAG-005: Admin Auth Bypass Flag — تكرار كود التحقق 19 مرة

- **Domain:** Code Quality — DRY / Security
- **Locations:** 19 occurrences across ReportManage.cshtml.cs and ReportData.cshtml.cs
- **Evidence:**
  `csharp
  var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
  if (!config.GetValue<bool>("AdminAuth:Bypass") &&
      HttpContext.Session.GetString("AdminAuthenticated") != "true")
  { return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 }; }
  `
- **Recommended Action:** Create an [AdminAuthorize] filter attribute or extension method to reduce boilerplate and prevent copy-paste errors.
- **Confidence:** High

---

### RB-FLAG-006: ParseViewName() لا تتحقق من وجود الـ View فعلياً

- **Domain:** Architecture — Input Validation
- **Location:** Services/ReportService.cs lines 139-152
- **Evidence:** Only strips brackets and splits by dot. No validation that the view exists.
- **Impact:** Low — the subsequent SQL will fail if view doesn't exist, but the error message could leak schema info.
- **Recommended Action:** Add validation against INFORMATION_SCHEMA.VIEWS or use SqlCommandBuilder.EnquoteIdentifier().
- **Confidence:** Medium

---

### RB-FLAG-007: GetFilterOptionsAsync تستخدم String Concatenation لأسماء الأعمدة

- **Domain:** Security / Architecture
- **Location:** Services/ReportService.cs line 673
- **Evidence:**
  `csharp
  var sql = $"SELECT DISTINCT [{columnName}] FROM {viewName} WITH (NOLOCK) WHERE [{columnName}] IS NOT NULL ORDER BY [{columnName}]";
  `
- **Impact:** Column name comes from query parameter. Brackets provide some protection but [col]] could theoretically break out.
- **Recommended Action:** Validate column name exists in the view schema before building the query.
- **Confidence:** Medium

---

### RB-FLAG-008: GetParameterOptionsAsync تنفذ OptionsQuery مخزنة كـ SQL خام

- **Domain:** Security — Stored SQL Injection
- **Location:** Services/ReportService.cs line 716
- **Evidence:** 
ew SqlCommand(filter.OptionsQuery, conn) — executes arbitrary stored SQL.
- **Context:** The query is stored in the DB by admin users. Known design, not a new risk.
- **Recommended Action:** Add read-only transaction mode and query validation.
- **Confidence:** Medium

---

### RB-FLAG-009: لا يوجد Seed Data للتقارير

- **Domain:** Data — Completeness
- **Location:** N/A — missing feature
- **Evidence:** Reports system plan (TASK-REPORT-025) mentions seed data for 3-5 initial reports, but no seed migration exists.
- **Impact:** After first deployment, admin must create reports manually.
- **Recommended Action:** Create seed data migration or script for initial reports.
- **Confidence:** High

---

### RB-FLAG-010: Accessibility Issues

- **Domain:** Frontend — Accessibility
- **Locations:** Multiple across both pages
- **Evidence:**
  1. Step 1 search input (iewSearch) has placeholder only, no accessible label
  2. Modals close buttons use × without ria-label
  3. Image modal img has generic lt="معاينة الصورة"
  4. No focus trap in modals — keyboard navigation can escape behind overlay
- **Recommended Action:** Add ria-label attributes, labels for all interactive elements, and focus trapping in modals.
- **Confidence:** High

---

### RB-FLAG-011: العمودان sortOrder و isEnabled في Reports ليس لديهما Indexes مركبة

- **Domain:** Data — Performance
- **Location:** Migration 20260720122504_AddReportSystemTables.cs
- **Evidence:** Two separate indexes: IX_Reports_IsEnabled and IX_Reports_SortOrder. Most queries filter by IsEnabled AND SortBy SortOrder. No composite index.
- **Recommended Action:** Add composite index on (IsEnabled, SortOrder).
- **Confidence:** Medium

---

## 🟢 BASELINE_DEBT Findings

### RB-BASE-001: 15 DTOs في نفس ملف الخدمة

- **Domain:** Code Quality — Organization
- **Location:** Services/ReportService.cs lines 805-956
- **Evidence:** 12 DTO classes + 3 request DTOs all defined at the bottom of the service file.
- **Recommended Action:** Move to Models/Dto/ folder as separate files.
- **Confidence:** High

---

### RB-BASE-002: NOLOCK يستخدم في جميع استعلامات التقارير

- **Domain:** Architecture — Data Consistency
- **Location:** ReportService.cs lines 438, 673; ReportData.cshtml.cs line 244
- **Evidence:** All report queries use WITH (NOLOCK) hint
- **Impact:** Dirty reads possible. Acceptable for reporting but should be documented as a design decision.
- **Recommended Action:** Document NOLOCK usage as intentional for report performance, potentially make it configurable per report.
- **Confidence:** Medium

---

## ✅ RESOLVED — Fixes from Previous Audit

### RB-RESOLVED-001: F-01 — ReportFilterDto Missing ValueColumn/TextColumn (CAUTION → RESOLVED)

- **Previous Audit:** QUAUD-TASK-REPORT-PARAM-2025-07-21-001, F-01
- **Current Status:** ✅ **FIXED**
- **Evidence:**
  1. ReportService.cs lines 882-884: public string? ValueColumn { get; set; } and public string? TextColumn { get; set; } are now present in ReportFilterDto
  2. ReportService.cs lines 282-283: ValueColumn = f.ValueColumn, TextColumn = f.TextColumn mapped in CreateReportAsync()
  3. ReportService.cs lines 344-345: Mapped in UpdateReportAsync() as well
- **This finding is now CLOSED.** The parameter system can now persist ValueColumn/TextColumn through the full save flow.

---

## Detailed File-by-File Assessment

### 1. ReportBuilder/Index.cshtml (702 lines)
| Aspect | Assessment |
|---|---|
| Structure | Good — 4-step wizard with step indicators. Clean HTML with BEM-like CSS classes. |
| JavaScript | **Caution** — 450+ lines inline (should be externalized). |
| XSS Protection | Good — escapeHtml() used for user input. But gap at line 631 (error message). |
| Accessibility | **Flag** — Search has no accessible label. Some inputs lack labels. |
| Error Handling | Adequate — try-catch with user-friendly messages. |
| UX | Toast notifications work. Empty states present. Search for views works. |

### 2. ReportBuilder/Index.cshtml.cs (11 lines)
| Aspect | Assessment |
|---|---|
| Minimal | Just OnGet() — all work done client-side. Acceptable for this pattern. |

### 3. ReportService.cs (956 lines)
| Aspect | Assessment |
|---|---|
| SRP | **Caution** — 5+ responsibilities in one file. |
| DTOs | **Baseline** — 15 DTOs in same file. |
| SQL Safety | **Caution** — Parameterized values but dynamic column/view names. GetFilterOptionsAsync uses concatenation. |
| Error Handling | **Flag** — Inconsistent. Some methods have detailed try-catch, others (CreateReportAsync) have none. |
| Logging | Good — ILogger used consistently. |
| Timeouts | Good — 15s for metadata, 30s for options, 60s for reports. |

### 4. ReportManage.cshtml.cs (133 lines)
| Aspect | Assessment |
|---|---|
| Auth check | Present in all 6 handlers. Boilerplate repeated. |
| Input validation | Basic null/empty checks for required fields. |
| CSRF | [IgnoreAntiforgeryToken] — system pattern. |

### 5. ReportData.cshtml.cs (379 lines)
| Aspect | Assessment |
|---|---|
| Auth check | Present in all 9 handlers. |
| executeQuery | **Caution** — Raw SQL from request body. |
| preview | **Caution** — View name concatenated into SQL. |
| Error handling | **Caution** — Raw ex.Message exposed in 2+ places. |
| Request DTOs | 4 DTO classes defined in same file. |

### 6. Reports/Index.cshtml (1205 lines)
| Aspect | Assessment |
|---|---|
| AG Grid Integration | Good — Proper RTL, Arabic locale, pagination, column management. |
| JavaScript | **Caution** — 785+ lines inline. |
| Layout Management | Good — Save/restore layouts with JSON serialization. |
| AbortController | Good — Cancel search properly aborts pending requests. |
| Skeleton Loading | Good — Present for data loading. |
| CSS Inline | **Flag** — 290 lines inline with hardcoded colors. |
| Duplicate API calls | **Caution** — F-03 still open (double Dropdown loading). |

### 7. Models (4 files)
| Model | Assessment |
|---|---|
| Report.cs | Clean. Proper annotations. Navigation properties. |
| ReportColumn.cs | Clean. All fields present. |
| ReportFilter.cs | Clean. ValueColumn and TextColumn present (from migration). |
| ReportLayout.cs | Clean. JSON columns with 
varchar(max). |

### 8. Migrations
| Migration | Assessment |
|---|---|
| 20260720122504_AddReportSystemTables | ✅ Down() is inverse of Up(). Indexes present. Cascade delete correct. |
| 20260721045735_AddValueTextColumnsToReportFilter | ✅ Up adds 2 columns, Down drops them. Clean. |

---

## Risk Register

| # | Risk | Severity | Mitigation |
|---|---|---|---|
| R-01 | executeQuery endpoint allows arbitrary SQL | **HIGH** | Auth-protected but no query restriction. Add SELECT-only check + read-only mode |
| R-02 | Preview SQL injection via viewName | **HIGH** | Validate view exists before querying |
| R-03 | XSS via error message display | **MEDIUM** | Add escapeHtml to line 631 |
| R-04 | Error messages leak schema info | **MEDIUM** | Replace ex.Message with generic messages |
| R-05 | ReportService violates SRP (956 lines) | **MEDIUM** | Split into multiple services |
| R-06 | F-03 double API call | **LOW** | Remove redundant loadDropdownOptions call |
| R-07 | Hardcoded CSS colors break theming | **MEDIUM** | Replace with design tokens |
| R-08 | No seed data for reports | **LOW** | Create seed migration |
| R-09 | CSRF vulnerability on all POST endpoints | **LOW** (admin-only network) | Evaluate anti-forgery needs |
| R-10 | No server-side pagination for report data | **LOW** | 10K row cap exists but not server-side paging |

---

## Remediation Recommendations

### Required (Before Gate Can Pass)

1. **RB-SEC-001** — Add SELECT-only validation and read-only mode to OnPostExecuteQueryAsync
2. **RB-SEC-002** — Validate view name against INFORMATION_SCHEMA.VIEWS before executing preview
3. **RB-SEC-003** — Add escapeHtml() to error message display in preview (line 631)
4. **RB-SEC-004** — Replace ex.Message with generic error messages in all 3 locations
5. **RB-CQ-001** — Split ReportService.cs into focused services (at minimum, extract DTOs to separate files)

### Strongly Recommended (Next Sprint)

6. **RB-FE-001** — Replace all hardcoded CSS colors with design tokens
7. **RB-CQ-002 (F-03)** — Remove redundant loadDropdownOptions() call for parameterized filters
8. **RB-FLAG-001/002** — Extract JavaScript to external files
9. **RB-FLAG-003** — Extract escapeHtml() to shared utility

### Nice to Have (Tracked Debt)

10. **RB-FLAG-005** — Create [AdminAuthorize] filter attribute
11. **RB-FLAG-009** — Create seed data migration
12. **RB-FLAG-010** — Fix accessibility issues (aria-labels, focus traps)
13. **RB-FLAG-011** — Add composite index on Reports(IsEnabled, SortOrder)

---

## Handback to Orchestrator

- **Status:** NEEDS_FIX
- **Report Path:** project-control/audit-reports/QUAUD-REPORTBUILDER-COMPREHENSIVE-2026-07-21.md
- **Blocking Findings:** 7 CAUTION (RB-SEC-001 through RB-SEC-004, RB-CQ-001, RB-FE-001, RB-CQ-002)
- **Previously Blocking Finding Resolved:** ✅ F-01 (ValueColumn/TextColumn in ReportFilterDto) is now fixed
- **Recommended Next Action:**
  1. Fix the 4 security findings (RB-SEC-001 to RB-SEC-004) — highest priority
  2. Begin refactoring ReportService.cs (RB-CQ-001)
  3. Fix CSS theming (RB-FE-001)
  4. Address F-03 double API call
  5. After fixes, re-run this audit for PASS verification

---

*Auditor Agent (مُدقق) — Quality Gate Report*
*Generated: 2026-07-21*
*Audit Mode: Full Risk-Based | Scope: 12 source files + 2 migrations + 2 previous audits + 3 governance docs*
*Confidence: High (all findings based on direct code evidence)*
