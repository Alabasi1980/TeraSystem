# TASK-PREP-005: Create 04_USERS_ROLES_PERMISSIONS.md

## 1. Task Information

| Field | Value |
|---|---|
| **TASK-ID** | TASK-PREP-005 |
| **Task Type** | Preparation |
| **Phase** | 4 — Sub-Agent Generation & Preparation Delegation (Batch B) |
| **Build Mode Approved** | N/A (preparation task) |
| **Status** | ✅ Approved / In Progress |
| **Assigned To** | General Agent |
| **Created** | 2026-07-12 |
| **Linked Plan Item** | PREPARATION_PLAN.md Batch B |
| **Linked Batch** | Batch B |
| **Active Technology Profile** | `dotnet-razorpages-adonet` |
| **Design Source Decision** | N/A |

## 2. Objective

Create `04_USERS_ROLES_PERMISSIONS.md` — define the users, roles, and permissions for the WarehouseDashboard application. This affects screens, data access, security, and workflows.

## 3. Reference Files

- `project-preparation/01_PROJECT_BRIEF.md` (now completed)
- `project-preparation/APPLICATION_BLUEPRINT.md` — Blueprint §5 (User Roles)
- `client-engagement/CLIENT_DECISION_LOG.md` — Decisions #4, #8, #9

## 4. Allowed Write Targets

- `clients/الماجد-لادارة-المستودعات/applications/APP-WarehouseDashboard/project-preparation/04_USERS_ROLES_PERMISSIONS.md`

## 5. Forbidden Files / Actions

- Do NOT modify any existing files
- Do NOT create files other than 04_USERS_ROLES_PERMISSIONS.md
- Do NOT write code
- Do NOT add roles not approved in CLIENT_DECISION_LOG.md

## 6. Acceptance Criteria

1. Documents Admin role (Phase 1): full access — Admin Panel, card CRUD, sync trigger, log viewing
2. Documents Viewer role (Phase 2 deferred): read-only dashboard
3. Documents Admin Panel security: password-protected + hidden URL
4. Defines what each role can and cannot do
5. Phase 2 RBAC upgrade path is noted

## 7. TASK-ID Size Check

```
Single file, only 2 roles, simple permissions. Fits one TASK-ID. Yes.
```

## 8. Final Tera Decision

| Item | Value |
|---|---|
| Final Status | _Pending (to be filled after review)_ |
