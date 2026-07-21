# TASK_REGISTRY.md — WarehouseDashboard

> **Purpose:** Central registry of all TASK-IDs created during the project lifecycle.
> **Last Updated:** 2026-07-21 (Sync Page Professional Redesign — TASK-UI-SYNC-REDESIGN-001 ✅)

---

## Phase 4 — Preparation Tasks

| TASK-ID | File | Assigned Agent | Status | Batch | Created |
|---|---|---|---|---|---|
| TASK-PREP-001 | `01_PROJECT_BRIEF.md` | General | ✅ Accepted | A | 2026-07-12 |
| TASK-PREP-002 | `08_TECHNICAL_ARCHITECTURE.md` | tera-software-designer | ✅ Accepted | A | 2026-07-12 |
| TASK-PREP-003 | `02_SCOPE_AND_BOUNDARIES.md` | General | ✅ Accepted | B | 2026-07-12 |
| TASK-PREP-004 | `03_MODULES_AND_FEATURES.md` | General | ✅ Accepted | B | 2026-07-12 |
| TASK-PREP-005 | `04_USERS_ROLES_PERMISSIONS.md` | General | ✅ Accepted | B | 2026-07-12 |
| TASK-PREP-006 | `05_BUSINESS_WORKFLOWS.md` | General | ✅ Accepted | C | 2026-07-12 |
| TASK-PREP-007 | `06_DATA_MODEL_PREPARATION.md` | tera-software-designer | ✅ Accepted | C | 2026-07-12 |
| TASK-PREP-008 | `07_SCREENS_AND_UI_STRUCTURE.md` | ui-designer | ✅ Accepted | D | 2026-07-12 |
| TASK-PREP-009 | `13_REPORTS_AND_DASHBOARDS.md` | tera-software-designer | ✅ Accepted | D | 2026-07-12 |
| TASK-PREP-010 | `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` | tera-software-designer | ✅ Accepted | D | 2026-07-12 |
| TASK-PREP-011 | `28_UI_UX_GUIDELINES.md` | ui-designer | ✅ Accepted | D | 2026-07-12 |
| TASK-PREP-012 | `16_AUDIT_LOG_AND_ACTIVITY_TRACKING.md` | tera-software-designer | ✅ Accepted | E | 2026-07-12 |
| TASK-PREP-013 | `19_DATABASE_DESIGN.md` | tera-software-designer | ✅ Accepted (DGs resolved) | E | 2026-07-12 |
| TASK-PREP-014 | `20_API_CONTRACTS.md` | tera-software-designer | ✅ Accepted (typo+auth fixed) | E | 2026-07-12 |
| TASK-PREP-015 | `22_DEPLOYMENT_AND_ENVIRONMENTS.md` | tera-software-designer | ✅ Accepted | E | 2026-07-12 |
| TASK-PREP-016 | `09_IMPLEMENTATION_PLAN.md` | General | ✅ Accepted | F | 2026-07-12 |
| TASK-PREP-017 | `24_CLIENT_REVIEW_NOTES.md` | General | ✅ Accepted | F | 2026-07-12 |
| TASK-PREP-018 | `25_CHANGE_REQUESTS.md` | General | ✅ Accepted | F | 2026-07-12 |
| TASK-PREP-019 | `35_ROADMAP_AND_FUTURE_PHASES.md` | General | ✅ Accepted | F | 2026-07-12 |

**All 19 Required preparation files COMPLETE ✅**

---

## Phase 6 — Implementation Tasks

| TASK-ID | Description | Assigned Agent | Status | Batch | Created |
|---|---|---|---|---|---|
| TASK-COD-001 | Oracle Connection Test | engineering-agent | ✅ Accepted (Test PASS) | B1 | 2026-07-12 |
| TASK-COD-003 | Project Scaffolding (.NET 8 + Syncfusion) | engineering-agent | ✅ Accepted (build PASS) | B1 | 2026-07-13 |
| TASK-COD-002 | SQL Server Database + EF Migrations | engineering-agent | 🟢 Code Ready (corrected migration, client applies) | B1 | 2026-07-13 |
| TASK-COD-004 | Oracle Extraction Service | engineering-agent | ✅ Accepted (build PASS) | B2 | 2026-07-13 |
| TASK-COD-005 | SqlBulkCopy Load + TableMapping + Orchestration | engineering-agent | ✅ Accepted (build PASS) | B2 | 2026-07-13 |
| TASK-COD-007 | Config-Driven Schedule + Status Tracking | engineering-agent | ✅ Accepted (build PASS) | B3 | 2026-07-13 |
| TASK-COD-008 | Admin Panel BCrypt Auth | engineering-agent | ✅ Accepted (build PASS) | B3 | 2026-07-13 |
| TASK-COD-006 | Sync API Endpoints | engineering-agent | ✅ Accepted (build PASS) | B3 | 2026-07-13 |
| TASK-COD-009 | DashboardCards CRUD (List + Editor) | engineering-agent | ✅ Accepted (build PASS) | B4 | 2026-07-13 |
| TASK-COD-010 | Query Tester + Drill Down Config | engineering-agent | ✅ Accepted (build PASS) | B4 | 2026-07-13 |
| TASK-COD-011 | Dashboard Main Page (Grid of Cards) | engineering-agent | ✅ Accepted (build PASS) | B5 | 2026-07-13 |
| TASK-COD-013 | Sync Status Bar + Manual Refresh | engineering-agent | ✅ Accepted (build PASS) | B5 | 2026-07-13 |
| TASK-COD-012 | Drill Down Pages | engineering-agent | ✅ Accepted (build PASS) | B5 | 2026-07-13 |
| TASK-COD-014 | Filtering + Search | engineering-agent | ✅ Accepted (build PASS) | B5 | 2026-07-13 |
| TASK-COD-015 | Skeleton Loading / Shimmer | ui-designer | ✅ Covered by B5 (011/012/013) | B6 | 2026-07-13 |
| TASK-COD-016 | Empty States + Error States | ui-designer | ✅ Covered by B5 (011/012/013) | B6 | 2026-07-13 |
| TASK-COD-017 | Toast + Micro-animations | ui-designer | ✅ Covered by B5 (011/012/013/014) | B6 | 2026-07-13 |
| TASK-COD-018 | Connection Status Indicator | ui-designer | ✅ Covered by B5 (013) | B6 | 2026-07-13 |
| TASK-COD-019 | IIS Setup + Environment Config | engineering-agent | ✅ Accepted (build PASS, deploy guide done) | B7 | 2026-07-13 |
| TASK-COD-020 | Syncfusion License Verification | TeraAgent | ✅ Accepted (license registered, key in appsettings) | B7 | 2026-07-13 |
| TASK-COD-021 | UAT + Deployment Testing | engineering-agent | ✅ Accepted (97 scenarios, 8 categories) | B7 | 2026-07-13 |
| TASK-COD-FIX-001 | Critical & Important Bug Fixes (self-audit) | engineering-agent | ✅ Accepted (build PASS, 11/11 AC) | FIX | 2026-07-13 |
| TASK-COD-022 | Admin Panel Home + Navigation + Layout Unification | engineering-agent + ui-designer | ✅ Accepted (build PASS) | B8 | 2026-07-13 |
| TASK-COD-023 | Sync Logs Page | engineering-agent | ✅ Accepted (build PASS) | B8 | 2026-07-13 |
| TASK-COD-024 | Sync Settings Admin Page | engineering-agent | ✅ Accepted (build PASS) | B8 | 2026-07-13 |
| TASK-COD-025 | Dynamic Table Mappings (CRUD + Schema Diff + SyncEngine) | engineering-agent | ✅ Accepted (build PASS, 0 errors) | B9 | 2026-07-14 |
| TASK-COD-026 | Visual Card Builder — Wizard + Live Preview + Templates + Clone | ui-designer + engineering-agent | ✅ Accepted (UI complete) | B10 | 2026-07-14 |
| TASK-COD-FIX-002 | Razor Encoding Normalization | engineering-agent | ✅ Resolved (UTF-8 verified 2026-07-21) | FIX | 2026-07-14 |
| TASK-COD-027 | Fix Builder page HTTP 500 (IHttpClientFactory not registered) | engineering-agent | ✅ Accepted (build 0 errors) | FIX | 2026-07-14 |
| TASK-COD-028 | Fix Builder Styles section mismatch (_CardsLayout missing RenderSection) | engineering-agent | ✅ Accepted (build PASS) | FIX | 2026-07-14 |
| TASK-COD-029 | Oracle Source Mapping Wizard (4-step wizard + Oracle discovery + Query validation + Preview) | engineering-agent | ✅ Accepted (build PASS) | B11 | 2026-07-14 |
| PLAN-CARD-BUILDER | Card Builder UX Plan (visual, mouse-friendly) | TeraAgent | 🟡 Draft | PREP | 2026-07-14 |
| TASK-COD-FIX-021 | Fix Card Builder Save Antiforgery Token | engineering-agent | ✅ Accepted (fallback build PASS) | FIX | 2026-07-18 |
| TASK-COD-FIX-022A | Fix Card Builder ModelState Save Return + Safe Save Diagnostics | engineering-agent | ✅ Accepted (build PASS) | FIX | 2026-07-18 |
| TASK-COD-FIX-022B | Fix Optional BindProperty Required Validation | engineering-agent | ✅ Accepted (fallback build PASS) | FIX | 2026-07-18 |
| TASK-COD-FIX-024 | Fix SqlTable DataSourceType Mapping and Legacy Render Guard | engineering-agent | ✅ Accepted (fallback build PASS) | FIX | 2026-07-18 |
| TASK-COD-FIX-025 | Fix TableMappings Anti-Forgery Token | engineering-agent | ✅ Accepted (build PASS) | FIX | 2026-07-18 |
| TASK-COD-COL-001 | ColumnMapping Entity + DbContext + Migration | engineering-agent | ✅ Accepted (build PASS) | COL | 2026-07-18 |
| TASK-COD-COL-002 | Column Mapping Editor UI — Wizard Tab + JS Logic | engineering-agent | ✅ Accepted (build PASS) | COL | 2026-07-18 |
| TASK-COD-COL-003 | Backend: Save/Load Column Mappings + Schema Generation Updates | engineering-agent | ✅ Accepted (build PASS) | COL | 2026-07-18 |

---

## Card Design Execution Tasks

| TASK-ID | Description | Assigned Agent | Status | Batch | Created |
|---|---|---|---|---|---|
| TASK-CARD-BEH-001 | Dashboard Card Metadata Bridge (`Description`, `ColorPalette`, `RefreshInterval`) | engineering-agent-dotnet | ✅ Accepted (build PASS) | CARD-A | 2026-07-19 |
| TASK-CARD-UX-001 | Card Header Description Hint | ui-designer | ✅ Accepted (build PASS) | CARD-A | 2026-07-19 |
| TASK-CARD-FIX-001 | Normalize Description Metadata Output | engineering-agent-dotnet | ✅ Accepted (build PASS + audit PASS) | CARD-FIX | 2026-07-19 |
| TASK-CARD-UX-002 | Apply Per-Card ColorPalette to Chart / Gauge / Sparkline | ui-designer | ✅ Accepted (build PASS + audit PASS) | CARD-A | 2026-07-19 |
| TASK-CARD-BEH-002 | Per-Card Auto-Refresh with Visual Indicator | ui-designer | ✅ Accepted (Auditor PASS) | CARD-B | 2026-07-19 |
| TASK-CARD-BEH-003 | DateFilterMode: Wire Dashboard Preset → SQL Queries | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | CARD-B | 2026-07-19 |
| TASK-CARD-FIX-002 | Fix SqlQuery Storage: Store Table Name, Not Pre-Aggregated SQL | engineering-agent-dotnet | ✅ Accepted | CARD-FIX | 2026-07-19 |
| TASK-CARD-FIX-003 | Cache-busting with asp-append-version | engineering-agent-dotnet | ✅ Accepted | CARD-FIX | 2026-07-19 |
| TASK-CARD-FIX-004 | Fix Custom Date Filter (dateFrom/dateTo) | engineering-agent-dotnet | ✅ Accepted | CARD-FIX | 2026-07-19 |
| TASK-CARD-BEH-004 | DateFilterMode: Fixed and Relative Modes | engineering-agent-dotnet | ✅ Accepted | CARD-B | 2026-07-19 |
| TASK-CARD-UX-006 | Visual DateFilterMode Indicator in Card Header | engineering-agent-dotnet | ✅ Accepted (build PASS) | CARD-A | 2026-07-19 |
| TASK-CARD-KPI-01 | Improve KPI Change Presentation (value size + change badge + comparison text) | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | CARD-C | 2026-07-19 |
| TASK-CARD-KPI-01-FIX-01 | Fix Missing KPI Change Percentage | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | CARD-C-FIX | 2026-07-19 |
| TASK-CARD-KPI-02 | Improve Sparkline Rendering (height + gradient + endpoint dot + tooltip) | ui-designer | ✅ Accepted (Auditor PASS after cleanup fix) | CARD-C | 2026-07-19 |
| TASK-CARD-KPI-03 | Fix Sparkline Date Range + Add "Last Month" Filter | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | CARD-C | 2026-07-19 |
| TASK-CARD-KPI-04 | CategoryColumn Breakdown Table (Top 5) | engineering-agent-dotnet | ✅ Accepted (Auditor PASS after escHtml fix) | CARD-C | 2026-07-19 |
| TASK-CARD-KPI-05 | GrandTotalSource: All-Time + Year-to-Date | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | CARD-C | 2026-07-19 |
| TASK-CARD-UX-05 | Chart / Table / Gauge Card Shell Polish | ui-designer | ✅ Accepted (build PASS) | CARD-A | 2026-07-19 |
| TASK-CARD-BUILDER-01 | Builder Preview Alignment (ApexCharts + HTML Table) | ui-designer | ✅ Accepted (build PASS) | CARD-D | 2026-07-19 |
| TASK-CARD-KPI-REDESIGN-01 | No-scroll professional KPI dashboard card layout | ui-designer | ✅ Accepted (Auditor PASS after FIX-01) | CARD-FIX | 2026-07-19 |
| TASK-CARD-KPI-REDESIGN-01-FIX-01 | Drill modal regression fix after KPI redesign | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | CARD-FIX | 2026-07-19 |

---

## Oracle Query Lab Enhancement Tasks

| TASK-ID | Description | Assigned Agent | Status | Batch | Created |
|---|---|---|---|---|---|
| TASK-ORALAB-001 | Oracle Error Messages + N+1 Schema Fix | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | ORALAB-P0 | 2026-07-19 |
| TASK-ORALAB-002 | Connection Status + Ctrl+Enter + Cancel | engineering-agent-dotnet | ✅ Accepted (Auditor PASS) | ORALAB-P0 | 2026-07-19 |
| TASK-ORALAB-003 | Query History + 10K Limit + CSV Download | engineering-agent-dotnet | ✅ Accepted | ORALAB-P0 | 2026-07-19 |
| TASK-ORALAB-004 | Horizontal Scroll for Wide Results | engineering-agent-dotnet | ✅ Accepted (build PASS) | ORALAB-P1 | 2026-07-19 |
| TASK-ORALAB-005 | Syntax Highlighting + Line Numbers (CodeMirror) | engineering-agent-dotnet | ✅ Accepted (build PASS) | ORALAB-P2 | 2026-07-19 |

**All 5 OracleQueryLab tasks COMPLETE ✅**

---

## Sync Log Enhancement (TASK-SYNC-LOG)

| TASK-ID | Description | Assigned Agent | Status | Created |
|---|---|---|---|---|
| TASK-SYNC-LOG-01 | Backend: Entities + Migration + DbContext | engineering-agent-dotnet | ✅ Accepted (جزئي) | 2026-07-20 |
| TASK-SYNC-LOG-01-FIX | API: SyncEngine DB writing + Controller DB reading | engineering-agent-dotnet | ✅ Accepted (build PASS) | 2026-07-20 |
| TASK-SYNC-LOG-02 | Frontend: Rich advanced SyncLogs page redesign | ui-designer | ✅ Accepted (build PASS) | 2026-07-20 |

---

## Sync Export Excel (TASK-SYNC-EXCEL)

| TASK-ID | Description | Assigned Agent | Status | Created |
|---|---|---|---|---|
| TASK-SYNC-EXCEL-001 | Backend: ClosedXML + ExportExcelService + API Endpoint | engineering-agent-dotnet | ✅ Accepted | 2026-07-21 |
| TASK-SYNC-EXCEL-002 | Frontend: Download Excel button in Sync Dashboard (Admin) | ui-designer | ✅ Accepted | 2026-07-21 |
| TASK-SYNC-EXCEL-003 | Backend: GET /api/sync/exportable-mappings endpoint | engineering-agent-dotnet | ✅ Accepted | 2026-07-21 |
| TASK-SYNC-EXCEL-004 | Frontend: Public DataExports page + Dashboard link | ui-designer | ✅ Accepted | 2026-07-21 |

---

## AI Dashboard Assistant Tasks

| TASK-ID | Description | Assigned Agent | Status | Batch | Created |
|---|---|---|---|---|---|
| TASK-AI-B01 | Migration: Add AssistantEnabled + AssistantPrompt to DashboardCard | engineering-agent-dotnet | ✅ Accepted (minor drift: SyncMode nvarchar expanded) | AI-B-1 | 2026-07-21 |
| TASK-AI-B02 | AI Provider Config: appsettings section + AIAssistantOptions model | engineering-agent-dotnet | ✅ Accepted (build errors pre-existing from B01) | AI-B-1 | 2026-07-21 |
| TASK-AI-B03 | Read-Only DB Connection + ReadOnlyQueryHelper | engineering-agent-dotnet | ✅ Accepted | AI-B-1 | 2026-07-21 |
| TASK-AI-B04 | Provider Abstraction: IAIProvider + OpenCodeGoAdapter + DTOs | engineering-agent-dotnet | ✅ Accepted | AI-B-2 | 2026-07-21 |
| TASK-AI-B05 | CardInsightService Foundation (wiring only, no AI calls yet) | engineering-agent-dotnet | ✅ Accepted | AI-B-3 | 2026-07-21 |
| TASK-AI-C01 | Summary Builder Interface + CardSummary DTO | engineering-agent-dotnet | ✅ Accepted | AI-C-1 | 2026-07-21 |
| TASK-AI-C02 | KPI Summary Builder + Factory | engineering-agent-dotnet | ✅ Accepted | AI-C-2 | 2026-07-21 |
| TASK-AI-C03 | Chart Summary Builder (Bar/Line/Pie/Gauge) | engineering-agent-dotnet | ✅ Accepted | AI-C-2 | 2026-07-21 |
| TASK-AI-C04 | Table Summary Builder | engineering-agent-dotnet | ✅ Accepted | AI-C-2 | 2026-07-21 |
| TASK-AI-C05 | Generic/No-Date Summary Builder (fallback *) | engineering-agent-dotnet | ✅ Accepted | AI-C-2 | 2026-07-21 |
| TASK-AI-C-FIX01 | Fix: Register missing Table + Generic builders in DI | engineering-agent-dotnet | ✅ Accepted | AI-C-FIX | 2026-07-21 |
| TASK-AI-D01 | POST /api/card-insights/analyze endpoint | engineering-agent-dotnet | ✅ Accepted | D-1 | 2026-07-21 |
| TASK-AI-D02 | Side Panel partial view + CSS (RTL, responsive) | ui-designer | ✅ Accepted | D-1 | 2026-07-21 |
| TASK-AI-D03 | Assistant icon in cards + open/close JS | ui-designer | ✅ Accepted | D-2 | 2026-07-21 |
| TASK-AI-D04 | Mode buttons: AJAX wiring to API | ui-designer | ✅ Accepted | D-3 | 2026-07-21 |
| TASK-AI-D05 | States polish: deepening, full-data, disabled, loading/error | ui-designer | ✅ Accepted | D-4 | 2026-07-21 |
| TASK-AI-E01 | Migration: AssistantInsightLogs + AssistantUsageStats tables | engineering-agent-dotnet | ✅ Accepted | E-1 | 2026-07-21 |
| TASK-AI-E02 | AssistantLogService (standalone logging) | engineering-agent-dotnet | ✅ Accepted | E-2 | 2026-07-21 |
| TASK-AI-E03 | AssistantCacheService (IMemoryCache, 10min TTL) | engineering-agent-dotnet | ✅ Accepted | E-2 | 2026-07-21 |
| TASK-AI-E04 | Integrate Log+Cache into CardInsightService + API | engineering-agent-dotnet | ✅ Accepted | E-3 | 2026-07-21 |
| TASK-AI-F01 | QA Audit: Build, Security, Architecture, Acceptance | qa-agent | ✅ Complete (F1 fixed, F2-F5 noted) | F | 2026-07-21 |
| TASK-AI-F-FIX01 | Fix: Register ReadOnlyQueryHelper in DI | engineering-agent-dotnet | ✅ Accepted | F-FIX | 2026-07-21 |

**Plan Reference:** `project-preparation/AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md`

---

## Sync Page Professional Redesign

| TASK-ID | Description | Assigned Agent | Status | Created |
|---|---|---|---|---|
| TASK-UI-SYNC-REDESIGN-001 | إعادة تصميم كاملة لصفحة /admin-secure-panel/Sync — glassmorphism، search، sorting، keyboard shortcuts، stagger animations، connection bar، shimmer محسّن، responsive | UI Designer | ✅ Accepted (All Vitality 9/9 PASS) | 2026-07-21 |
| TASK-UI-SYNC-FIXES-001 | إصلاح 6 مشاكل UX: Query truncation + Modal، Sticky progress bar، Per-mapping sync info، Toggle enable/disable، Filter dropdowns، Disable auto-refresh | UI Designer | ✅ Accepted (All Vitality 8/8 PASS) | 2026-07-21 |

---

## Status Key

| Status | Meaning |
|---|---|
| **Draft** | Created but not yet approved |
| **Approved** | Approved for execution |
| **In Progress** | Assigned to agent and being worked on |
| **Submitted** | Agent completed — awaiting Tera review |
| **Needs Fix** | Returned to agent for revision |
| **Accepted** | Fully accepted and closed |
| **Code Ready** | Code complete — ينتظر بيانات/بيئة خارجية للتشغيل |
| **Testing** | Under QA-Agent verification |
| **Blocked** | Cannot proceed — needs external input |
| **Cancelled** | No longer needed |
| **Deferred** | Postponed to later phase |

---

> **Prepared by:** TeraAgent — 2026-07-13
