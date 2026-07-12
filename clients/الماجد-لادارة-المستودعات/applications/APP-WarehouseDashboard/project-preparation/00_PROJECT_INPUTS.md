# 00_PROJECT_INPUTS.md — WarehouseDashboard

> **Purpose:** Normalized summary derived from client engagement documents and APPLICATION_BLUEPRINT.md.
> **Source:** TCEA Handoff → ApplicationBlueprintAgent → TeraAgent normalization
> **Date:** 2026-07-12

---

## 1. Application Identity

| Field | Value |
|---|---|
| **Application Name** | WarehouseDashboard |
| **Client** | الماجد لادارة المستودعات |
| **Business Domain** | Warehouse Management |
| **App Type** | Internal — Local deployment on IIS |
| **Language** | C# (.NET 8 LTS) |
| **Interface** | ASP.NET Core Razor Pages |
| **Database** | Oracle (source, read-only) → SQL Server (destination, write + config + logs) |

---

## 2. Problem Statement

العميل يحتاج نظاماً محلياً لاستخراج البيانات من قاعدة Oracle وتحويلها إلى SQL Server، ثم عرضها في Dashboard ديناميكي مع ~20 بطاقة قابلة للتكوين عبر Admin Panel.

---

## 3. Target Users

| Role | Description | Permissions | Phase |
|---|---|---|---|
| **Admin** | مسؤول النظام الرئيسي | Full access: Admin Panel + Card CRUD + Sync trigger | Phase 1 |
| **Viewer** | مستخدم عادي | Read-only Dashboard | Phase 2 (deferred) |

---

## 4. Core Capabilities (from Blueprint)

### 4.1 Sync Engine (API)

| # | Capability | Description |
|---|---|---|
| M1.1 | Oracle Data Extraction | Read Oracle tables via ODP.NET with Data Type Mapping |
| M1.2 | SQL Server Full Refresh | Delete all rows + bulk insert in single transaction |
| M1.3 | SQL Server Incremental Sync (Ready) | Engine designed for Incremental from day one (not activated in Phase 1) |
| M1.4 | Auto-Sync Scheduler | Background Service with configurable interval |
| M1.5 | Manual Sync API | POST /api/sync/trigger |
| M1.6 | Sync Status API | GET /api/sync/status |
| M1.7 | Sync Logs | Log every operation (start/end time, status, record count, duration) |

### 4.2 Dashboard (Razor)

| # | Capability | Description |
|---|---|---|
| M2.1 | Dynamic Card Rendering | Read config from DB + build cards dynamically |
| M2.2 | Drill Down | Multi-level configurable drill-down |
| M2.3 | Breadcrumb Navigation | Navigation bar showing current level |
| M2.4 | Sync Status Display | Last sync time + status indicator + manual sync button |
| M2.5 | Data Binding | Execute SQL Queries/Views from config |
| M2.6 | Blue Theme | 11-color visual identity |
| M2.7 | Responsive Layout | Responsive grid layout |

### 4.3 Admin Panel (Razor)

| # | Capability | Description |
|---|---|---|
| M3.1 | Card CRUD | Create / Edit / Delete cards |
| M3.2 | Data Source Config | SQL Query / View selection |
| M3.3 | Chart Type Selection | Bar, Line, Pie, KPI, Table, Gauge |
| M3.4 | Drill Down Config | Drill-down levels and linking |
| M3.5 | Card Layout Config | Grid size and position |
| M3.6 | Config Save/Load | Save config to SQL Server |
| M3.7 | Query Tester | Test query before save |

### 4.4 Infrastructure

| # | Capability | Description |
|---|---|---|
| M4.1 | .NET 8 Solution | Two-project solution (API + Razor) |
| M4.2 | Config DB Schema | Card configuration tables |
| M4.3 | Sync Logs Schema | Sync operation logs |
| M4.4 | Data Tables Schema | Transferred data tables (TBD) |
| M4.5 | IIS Hosting | Local IIS deployment |

---

## 5. Feature Classification (MVP_DEFINITION_PROTOCOL)

### Core MVP (Phase 1A) — Smallest useful version

| Feature | Rationale |
|---|---|
| Oracle Connection & Data Extraction (API-01) | Essential — no data without it |
| SQL Server Data Loading / Full Refresh (API-02) | Essential — primary workflow |
| Synchronization Engine — Auto + Manual (API-03) | Essential — core purpose |
| Synchronization Logs (API-04) | Essential — visibility into sync operations |
| Dashboard Layout & Rendering (RZR-01) | Essential — primary interface |
| Drill Down Functionality (RZR-02) | Essential — key value proposition |
| Data Binding & SQL Execution (RZR-03) | Essential — connects data to dashboard |
| Sync Status & Controls (RZR-04) | Essential — user needs sync visibility |
| Admin Panel — CRUD + Config (RZR-05) | Essential — dynamic card configuration |
| Project Structure (.NET 8) (INF-01) | Essential — foundation |
| Database Setup — Config + Logs (INF-02) | Essential — data persistence |
| IIS Hosting (INF-03) | Essential — deployment target |

### Extended MVP (Phase 1B) — Important but non-blocking

None identified — all features are either Core MVP or Phase 2.

### Phase 2 — Depends on Core MVP stability

| Feature | Rationale |
|---|---|
| Data editing screens | Depends on data being synced and stable |
| User roles & authentication | Depends on core workflow being operational |
| Export to Excel/PDF | Enhancement, not core |
| Advanced analytics | Depends on accumulated data |

### Out of Scope

| Feature | Reason |
|---|---|
| Cloud Hosting | Local deployment only |
| Mobile App | Not requested |
| Microservices | Not required — simple architecture |
| CI/CD Pipeline | Local project |
| Automated Testing | Can be added later |

---

## 6. Technical Decisions (Confirmed by Majed)

| # | Decision | Status |
|---|---|---|
| 1 | .NET 8 (LTS) | ✅ Approved |
| 2 | Razor Pages (not Blazor) | ✅ Approved |
| 3 | Syncfusion + UnlockKey | ✅ Approved |
| 4 | ODP.NET (Oracle.ManagedDataAccess) | ✅ Approved |
| 5 | ADO.NET SqlBulkCopy for bulk insert | ✅ Approved — strategic decision |
| 6 | EF Core for Config Tables + Sync Logs only | ✅ Approved |
| 7 | Full Refresh mode (Phase 1) | ✅ Approved |
| 8 | Incremental Sync ready (not activated) | ✅ Approved — risk mitigation |
| 9 | Oracle testing in first 3 days | ✅ Approved — risk mitigation |
| 10 | Admin Panel: password-protected, hidden URL | ✅ Approved |
| 11 | Blue Theme (11 colors) | ✅ Approved |
| 12 | IIS local hosting | ✅ Approved |

---

## 7. Pricing & Commercial

| Item | Value |
|---|---|
| Pricing Model | Time & Material |
| Rate | 4 JOD / hour |
| Estimated Range | 430 — 625 hours |
| Estimated Cost | 1,720 — 2,500 JOD |
| Complexity Index | 45.95% (Medium) |
| Deadline | None — natural pace |
| Post-delivery | Maintenance + bug fixes + development |

---

## 8. Risks

| # | Risk | Severity | Mitigation |
|---|---|---|---|
| 1 | Full Refresh may be slow with large data | High | Incremental Sync ready from day one |
| 2 | Oracle table details deferred | Medium | Client available during execution |
| 3 | EF Core not suitable for bulk insert | Medium | ADO.NET SqlBulkCopy for data + EF Core for config only |
| 4 | Admin Panel password-only security | Low | Phase 2 adds Role-Based Access |
| 5 | Drill Down complexity | Medium | Clear structural design + early testing |

---

## 9. Deferred Items

| # | Item | When |
|---|---|---|
| D1 | Oracle table details | During execution — client available |
| D2 | Data extraction mechanics | During execution |
| D3 | User count | During development |
| D4 | Phase 2 features | After Phase 1 completion |
| D5 | Sync interval default | 30 min assumed, configurable |

---

## 10. Workspace Location

```
clients/الماجد-لادارة-المستودعات/applications/APP-WarehouseDashboard/
├── client-engagement/       ← TCEA files (complete)
├── client-documents/        ← Client documents (pending)
├── client-approval/         ← Client approvals (to be created)
├── client-assets/           ← Client assets (pending)
├── delivery/                ← Final delivery package (pending)
└── project-preparation/     ← Blueprint + Open Questions
```

---

## 11. Source Documents

| Document | Path | Status |
|---|---|---|
| CLIENT_INTAKE.md | `client-engagement/CLIENT_INTAKE.md` | ✅ |
| FEATURE_LIST.md | `client-engagement/FEATURE_LIST.md` | ✅ |
| DISCOVERY_COVERAGE_SUMMARY.md | `client-engagement/DISCOVERY_COVERAGE_SUMMARY.md` | ✅ |
| CLIENT_DECISION_LOG.md | `client-engagement/CLIENT_DECISION_LOG.md` | ✅ (23 decisions) |
| DRAFT_QUOTATION.md | `client-engagement/DRAFT_QUOTATION.md` | ✅ |
| TERA_HANDOFF_PACKAGE.md | `client-engagement/TERA_HANDOFF_PACKAGE.md` | ✅ |
| APPLICATION_BLUEPRINT.md | `project-preparation/APPLICATION_BLUEPRINT.md` | ✅ approved_for_preparation |
| BLUEPRINT_OPEN_QUESTIONS.md | `project-preparation/BLUEPRINT_OPEN_QUESTIONS.md` | ✅ |

---

> **Normalization source:** APPLICATION_BLUEPRINT.md + TERA_HANDOFF_PACKAGE.md + CLIENT_DECISION_LOG.md + FEATURE_LIST.md + DRAFT_QUOTATION.md
> **Prepared by:** TeraAgent — 2026-07-12
