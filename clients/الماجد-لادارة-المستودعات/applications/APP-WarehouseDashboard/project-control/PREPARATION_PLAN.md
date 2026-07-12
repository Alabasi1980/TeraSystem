# PREPARATION_PLAN.md — WarehouseDashboard

## 1. Preparation Decision

**Decision:** ✅ Proceed to Project Preparation

> Reference: `project-preparation/TERA_PROJECT_DECISION.md` — Decision: Proceed

**Conditions:**
- Monitor Condition 1: PROJECT_MASTER_PLAN.md first in Phase 5 (before any TASK-COD-*)
- Monitor Condition 2: PROJECT_STATE.md fully populated before first TASK-COD
- Monitor Condition 3: First TASK-COD = Oracle connection test

---

## 2. File Catalog

### Classification Key

| Label | Meaning |
|---|---|
| **Required** | Must be created now in Phase 4 |
| **Conditional** | Create only if trigger condition is met |
| **Deferred** | Postponed to a later phase |
| **Not Required** | Not needed for this project |

### 2.1 Required Preparation Files

| File | Reason | Owner Agent | Order |
|---|---|---|---|
| `01_PROJECT_BRIEF.md` | Core understanding — reference for all downstream work | General | 1 |
| `02_SCOPE_AND_BOUNDARIES.md` | Essential for external client — protects from scope creep | General | 2 |
| `03_MODULES_AND_FEATURES.md` | Medium project needs structured module breakdown (12 features, 33 sub-components) | General | 3 |
| `04_USERS_ROLES_PERMISSIONS.md` | Admin (Phase 1) + Viewer (Phase 2) — affects screens, security, workflows | General | 4 |
| `05_BUSINESS_WORKFLOWS.md` | Sync workflow (Oracle→SQL Server), Admin Panel card management, Drill-down flow | General | 5 |
| `06_DATA_MODEL_PREPARATION.md` | Oracle data mapping + SQL Server config tables — core to architecture | tera-software-designer | 6 |
| `07_SCREENS_AND_UI_STRUCTURE.md` | Dashboard (~20 dynamic cards), Admin Panel, drill-down screens | ui-designer | 9 |
| `08_TECHNICAL_ARCHITECTURE.md` | Multiple technology decisions documented — critical for implementation | tera-software-designer | 7 |
| `09_IMPLEMENTATION_PLAN.md` | Bridge between preparation and execution planning (preliminary — formal plans in Phase 5) | Tera | 14 |
| `13_REPORTS_AND_DASHBOARDS.md` | Dashboard IS the product — card types, drill-down levels, KPI definitions, chart types | tera-software-designer | 10 |
| `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` | Oracle integration is the foundation of the project | tera-software-designer | 8 |
| `16_AUDIT_LOG_AND_ACTIVITY_TRACKING.md` | Sync logs (M1.7) are Core MVP — need schema and retention policy | tera-software-designer | 12 |
| `19_DATABASE_DESIGN.md` | SQL Server schema: Config tables, Sync Logs, Oracle data mapping | tera-software-designer | 11 |
| `20_API_CONTRACTS.md` | Sync trigger API (POST) + Sync status API (GET) — Core MVP | tera-software-designer | 13 |
| `22_DEPLOYMENT_AND_ENVIRONMENTS.md` | IIS deployment on client's server — must be documented before delivery | tera-software-designer | 15 |
| `24_CLIENT_REVIEW_NOTES.md` | External client project — need formal review tracking | Tera | 16 |
| `25_CHANGE_REQUESTS.md` | Commercial engagement — need change management process | Tera | 17 |
| `28_UI_UX_GUIDELINES.md` | Blue theme (11 colors), Syncfusion component rules, dashboard layout | ui-designer | 18 |
| `35_ROADMAP_AND_FUTURE_PHASES.md` | Phase 2 features already defined (4 features) — formalize the roadmap | General | 19 |

**Total Required: 19 files**
- Already created/covered: 00_PROJECT_INPUTS.md ✅, CLIENT_DECISION_LOG.md ✅
- New files to create in Phase 4: 19

### 2.2 Conditional Files

| File | Trigger Condition | Owner Agent | Order |
|---|---|---|---|
| `10_TESTING_AND_ACCEPTANCE.md` | If Monitor requires formal test plan before Phase 6 | Tera | Before Phase 6 |
| `21_VALIDATION_AND_ERROR_HANDLING.md` | If validation rules become complex enough for a separate file | tera-software-designer | After Phase 4 |
| `23_BACKUP_AND_RECOVERY.md` | Before production deployment — if client requires formal policy | tera-software-designer | Before deployment |
| `29_SAMPLE_DATA_AND_SEEDING.md` | Before Phase 6 if development/testing needs realistic seed data | General | Before Phase 6 |
| `32_PERFORMANCE_REQUIREMENTS.md` | If Full Refresh speed testing reveals performance issues | tera-software-designer | During Phase 6 |

### 2.3 Deferred Files

| File | Reason | Trigger for Activation |
|---|---|---|
| `11_DELIVERY_AND_HANDOVER.md` | Phase 7 deliverable — too early now | Phase 7 entry |
| `15_SECURITY_AND_ACCESS_CONTROL.md` | Phase 1 security is simple (password + hidden URL). Phase 2 adds RBAC | Phase 5 when auth is planned |
| `18_IMPORT_EXPORT_DATA.md` | Phase 2 feature (Export to Excel/PDF) | Phase 5 when Phase 2 is planned |
| `26_RISKS_AND_ASSUMPTIONS.md` | Already covered in PROJECT_STATE.md and TERA_PROJECT_DECISION.md | Not needed as separate file |
| `27_DECISIONS_LOG.md` | Already covered in CLIENT_DECISION_LOG.md (23 decisions recorded) | Not needed as separate file |
| `30_USER_MANUAL_DRAFT.md` | Before delivery to client | Phase 7 |
| `31_MAINTENANCE_AND_SUPPORT.md` | Post-delivery support agreement | Phase 7 |

### 2.4 Not Required Files

| File | Reason |
|---|---|
| `12_BUSINESS_RULES.md` | No complex business rules — sync+dashboard system |
| `17_NOTIFICATIONS_AND_ALERTS.md` | No notifications in this project |
| `33_MULTI_TENANCY_OR_COMPANY_STRUCTURE.md` | Single client system |
| `34_COMPLIANCE_AND_LEGAL_NOTES.md` | No regulatory requirements |

---

## 3. Preparation Sequence

```
Batch A (Foundation — parallel):
  01_PROJECT_BRIEF.md (General)
  08_TECHNICAL_ARCHITECTURE.md (tera-software-designer)

Batch B (Scope — depends on A):
  02_SCOPE_AND_BOUNDARIES.md (General)
  03_MODULES_AND_FEATURES.md (General)
  04_USERS_ROLES_PERMISSIONS.md (General)

Batch C (Workflow & Data — depends on B):
  05_BUSINESS_WORKFLOWS.md (General)
  06_DATA_MODEL_PREPARATION.md (tera-software-designer)

Batch D (Integration & Design — depends on B+C):
  07_SCREENS_AND_UI_STRUCTURE.md (ui-designer)
  13_REPORTS_AND_DASHBOARDS.md (tera-software-designer)
  14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md (tera-software-designer)
  28_UI_UX_GUIDELINES.md (ui-designer)

Batch E (Technical Deep — depends on C+D):
  16_AUDIT_LOG_AND_ACTIVITY_TRACKING.md (tera-software-designer)
  19_DATABASE_DESIGN.md (tera-software-designer)
  20_API_CONTRACTS.md (tera-software-designer)
  22_DEPLOYMENT_AND_ENVIRONMENTS.md (tera-software-designer)

Batch F (Planning & Governance — depends on all above):
  09_IMPLEMENTATION_PLAN.md (Tera)
  24_CLIENT_REVIEW_NOTES.md (Tera)
  25_CHANGE_REQUESTS.md (Tera)
  35_ROADMAP_AND_FUTURE_PHASES.md (General)
```

---

## 4. Suggested Sub-Agents

| Agent | Needed Now | Reason |
|---|---|---|
| `General` | Yes | Core preparation files (01, 02, 03, 04, 05, 35) |
| `tera-software-designer` | Yes | Technical files (06, 08, 14, 16, 19, 20, 22) |
| `ui-designer` | Yes | UI files (07, 28) |
| `TeraAgent` | Yes | Planning/governance files (09, 24, 25) |
| `engineering-agent` | No | Not needed in preparation phase — deferred to Phase 6 |
| `domain-expert-agent` | No | No domain research needed at this stage |
| `domain-research-agent` | No | No research needed at this stage |

### Agent Status in Phase 4

| Agent | Status |
|---|---|
| `General` | Use Existing (no specialized generation needed) |
| `tera-software-designer` | Use Existing (available as standard sub-agent) |
| `ui-designer` | Use Existing (available as standard sub-agent) |
| `TeraAgent` | N/A — Tera handles these files directly |

> **Note:** These agents may need specialization (narrowed sources/targets/constraints) in Phase 4. If specialization is required, update `generated-agents/opencode/` or create specialized versions.

---

## 5. User Approval Points

| Point | What Needs Approval | Before Moving To |
|---|---|---|
| P1 | **This plan (Preparation Decision)** | Phase 4: Sub-Agent Generation & Preparation Delegation |
| P2 | Scope and boundaries (02) | File creation for downstream files |
| P3 | Technical architecture (08) | Implementation planning |
| P4 | Implementation plan (09) | Phase 5: Execution Planning |
| P5 | Client approval package | Phase 5/6: Build Mode |
| P6 | Design direction (28) | UI implementation |

> **Rule:** No file creation happens in Phase 3. No agent generation happens before this plan is approved.

---

## 6. Technology Profile

**Status:** ⏳ Pending — custom profile needed

**Issue:** No existing Technology Profile matches ASP.NET Core Razor Pages. The closest profile `dotnet-blazor-ef` targets Blazor (SPA component model), not Razor Pages (server-rendered page model).

**Action:** Create custom Technology Profile in Phase 3 alongside this plan:
- Profile ID: `dotnet-razorpages-adonet`
- Framework: ASP.NET Core Razor Pages (.NET 8)
- ORM: ADO.NET SqlBulkCopy (data) + EF Core (config/logs only)
- Oracle: ODP.NET (Oracle.ManagedDataAccess)
- UI Library: Syncfusion
- Hosting: IIS

---

## 7. Approval Status

- [x] Plan submitted
- [ ] Plan approved → Proceed to Phase 4
- [ ] Plan rejected → Revise and resubmit
- [ ] Plan blocked → Reason: ...

---

> **Phase:** 3 — Project Preparation Planning
> **Prepared by:** TeraAgent — 2026-07-12
> **Client:** الماجد لادارة المستودعات — WarehouseDashboard
