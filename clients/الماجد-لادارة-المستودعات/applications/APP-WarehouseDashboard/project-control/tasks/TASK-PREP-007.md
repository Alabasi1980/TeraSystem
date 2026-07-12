# TASK-PREP-007: Create 06_DATA_MODEL_PREPARATION.md

## 1. Task Information

| Field | Value |
|---|---|
| **TASK-ID** | TASK-PREP-007 |
| **Task Type** | Preparation |
| **Phase** | 4 — Batch C |
| **Status** | ✅ Approved / In Progress |
| **Assigned To** | tera-software-designer |

## 2. Objective

Create `06_DATA_MODEL_PREPARATION.md` — preliminary data model covering SQL Server config tables (DashboardCards, CardDrillDownLevels, SyncSettings, AdminPassword), Sync Logs tables (SyncLogs, ErrorLogs), and the Oracle-to-SQL Server table mapping approach.

## 3. Reference Files

- `08_TECHNICAL_ARCHITECTURE.md`, `01_PROJECT_BRIEF.md`, `02_SCOPE_AND_BOUNDARIES.md`
- `APPLICATION_BLUEPRINT.md`, `dotnet-razorpages-adonet.md`

## 4. Allowed Write Targets

- `project-preparation/06_DATA_MODEL_PREPARATION.md`

## 5. Acceptance Criteria

1. SQL Server Config tables defined (DashboardCards, CardDrillDownLevels, SyncSettings, AdminPassword)
2. Sync Logs tables defined (SyncLogs, ErrorLogs)
3. Oracle Data Tables mapping approach documented
4. Entity relationships clear
5. EF Core entities for Config+Logs separated from ADO.NET managed Data Tables

## 6. Final Tera Decision

| Item | Value |
|---|---|
| Final Status | _Pending_ |
