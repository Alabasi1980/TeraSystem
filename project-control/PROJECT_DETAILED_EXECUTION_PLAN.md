# PROJECT_DETAILED_EXECUTION_PLAN.md

## Purpose

This file is the detailed execution plan for the project.

It expands the main phases and sub-phases from `PROJECT_MASTER_PLAN.md` into trackable implementation items.

Tera must update this file when tasks are created, completed, accepted, deferred, cancelled, blocked, or moved.

`PlanComplianceReviewAgent` must use this file as a primary source when reviewing whether implementation matches the approved plan.

---

## Status Values

| Status | Meaning |
|---|---|
| Planned | Planned but not started |
| In Progress | Work started |
| Implemented | Built but not fully accepted |
| Accepted | Completed and accepted |
| Needs Fix | Implemented but requires fix before acceptance |
| Deferred | Intentionally postponed |
| Cancelled | Removed by decision |
| Blocked | Waiting for dependency or decision |
| Out of Scope | Not part of the current project or version |
| Moved to Later Phase | Moved to a future phase |
| Status unclear | Current state is not yet confirmed |

---

## Detailed Execution Plan

| Phase ID | Main Phase | Sub-Phase ID | Sub-Phase | Required Work | Status | Linked Tasks | Linked Issues | Linked Decisions | Notes |
|---|---|---|---|---|---|---|---|---|---|
| PHASE-01 | Foundation | P01-S01 | Project Setup | Scaffold Next.js + TypeScript project and base local structure | Accepted | `TASK-0001` |  |  | Base project created and cleaned through post-execution review. |
| PHASE-01 | Foundation | P01-S02 | Prisma and Local Database | Define core Prisma setup, create local database, run first migration | Accepted | `TASK-0002`, `TASK-0003` | `ISSUE-0003` | `DEC-0004` | Secret-handling incident resolved and rules tightened. |
| PHASE-01 | Foundation | P01-S03 | Authentication and Roles | Login, session, role checks, protected routes, security review | Accepted | `TASK-0004`, `TASK-0005` | `ISSUE-0004`, `ISSUE-0005` | `DEC-0006` | Security review completed before moving forward. |
| PHASE-02 | Core Data | P02-S01 | Core Schema | Users, banks, parties, checks models and relations | Accepted | `TASK-0002`, `TASK-0003` |  |  | Prisma schema and initial migration completed. |
| PHASE-02 | Core Data | P02-S02 | Workflow and Screen Preparation | Workflow rules and UI screen specifications for implementation | Accepted | `TASK-0006` |  | `DEC-0008` | Prepared workflow and UI structure before larger screen work. |
| PHASE-02 | Core Data | P02-S03 | Validation and Reference Integrity | Server-side validation and protected reference-entity behavior | Accepted | `TASK-0007`, `TASK-0008`, `TASK-0011` | `ISSUE-0006` | `DEC-0011` | Banks and parties actions now enforce server-side validation and admin checks. |
| PHASE-03 | Core Screens | P03-S01 | Main Navigation | Main navigation hub for the MVP screens | Accepted | `TASK-0009` |  |  | Replaced placeholder root page with RTL navigation hub. |
| PHASE-03 | Core Screens | P03-S02 | Checks Screen | Checks CRUD, filters, status workflow, print | Accepted | `TASK-0012`, `TASK-0013`, `TASK-0014` | `ISSUE-0007`, `ISSUE-0008` |  | Main checks workflow completed, then hardened with two follow-up fixes. |
| PHASE-03 | Core Screens | P03-S03 | Banks Screen | Banks management screen | Accepted | `TASK-0007` | `ISSUE-0004`, `ISSUE-0005` | `DEC-0011` | Includes admin-only management and protected delete behavior. |
| PHASE-03 | Core Screens | P03-S04 | Parties Screen | Parties management screen | Accepted | `TASK-0008`, `TASK-0011` | `ISSUE-0006` | `DEC-0011` | Includes server-side validation after quality follow-up. |
| PHASE-03 | Core Screens | P03-S05 | Users Screen | Users management and role assignment | Accepted | `TASK-0015` |  |  | Final MVP screen accepted. |
| PHASE-04 | Quality & Fixes | P04-S01 | Security Review and Hardening | Independent security review plus direct security follow-up fixes | Accepted | `TASK-0005`, `TASK-0007`, `TASK-0013`, `TASK-0014` | `ISSUE-0004`, `ISSUE-0005`, `ISSUE-0007`, `ISSUE-0008` | `DEC-0006`, `DEC-0011` | Security findings were tracked and resolved. |
| PHASE-04 | Quality & Fixes | P04-S02 | Periodic Quality Review | Cross-domain quality review of recent work | Accepted | `TASK-0010` | `ISSUE-0006` | `DEC-0013` | Quality review produced actionable follow-up before checks completion. |
| PHASE-04 | Quality & Fixes | P04-S03 | Post-Review Validation Fixes | Resolve validation and stability issues found during reviews | Accepted | `TASK-0011`, `TASK-0013`, `TASK-0014` | `ISSUE-0006`, `ISSUE-0007`, `ISSUE-0008` |  | All tracked quality/security follow-up items are resolved. |
| PHASE-05 | Final Review & Handover | P05-S01 | Plan Compliance Review | Compare delivered MVP against master and detailed plans | Planned |  |  | `DEC-0017` | Enabled by current system-maintenance upgrade; review not yet run. |
| PHASE-05 | Final Review & Handover | P05-S02 | Handover Documentation | Delivery and handover documents, user guidance, maintenance summary | Planned |  |  |  | Draft handover files exist, but final handover flow is not yet executed. |
| PHASE-05 | Final Review & Handover | P05-S03 | Final MVP Readiness Summary | Final acceptance-oriented summary for MVP closure | Planned |  |  | `DEC-0017` | Should follow plan-compliance review and handover readiness confirmation. |
| PHASE-06 | Future Enhancements | P06-S01 | Status History | Track check status transitions historically | Deferred |  |  |  | Deferred from MVP per approved anti-bloat rules. |
| PHASE-06 | Future Enhancements | P06-S02 | Audit Log | Track user activity and audit events | Deferred |  |  |  | Deferred from MVP per approved anti-bloat rules. |
| PHASE-06 | Future Enhancements | P06-S03 | Export Excel/PDF | Export reports and data views | Deferred |  |  |  | Deferred from MVP per approved anti-bloat rules. |

---

## Update Rules

- Update linked task, issue, and decision references when official records exist.
- Leave link fields blank when no official record exists; do not invent IDs.
- Use `Needs Fix` when a plan item is implemented but still blocked by an open issue or failed acceptance.
- Use `Deferred`, `Cancelled`, or `Out of Scope` exactly as decided; do not collapse them into missing work.
- If a state cannot be confirmed safely, use `Status unclear` instead of guessing.
