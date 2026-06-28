# PROJECT_MASTER_PLAN.md

## Purpose

This file is the high-level execution roadmap for the project.

It defines the main implementation phases and their sub-phases without detailed task-level breakdown.

Tera must use this file to understand the overall project path and update phase and sub-phase status as the project progresses.

---

## Status Values

| Status | Meaning |
|---|---|
| Planned | Planned but not started |
| In Progress | Currently being worked on |
| Implemented | Built but not fully accepted yet |
| Accepted | Completed and accepted |
| Needs Fix | Implemented but blocked by issue or required fix |
| Deferred | Intentionally postponed |
| Cancelled | Intentionally removed |
| Blocked | Cannot proceed until dependency or decision is resolved |
| Out of Scope | Not part of the current project or version |
| Moved to Later Phase | Moved to a future phase |
| Status unclear | Current state is not yet confirmed |

---

## Master Plan

| Phase ID | Main Phase | Sub-Phases | MVP Scope | Status | Current Decision / Notes |
|---|---|---|---|---|---|
| PHASE-01 | Foundation | Project setup, Prisma setup, local database, authentication and roles | Yes | Accepted | `TASK-0001` through `TASK-0005` completed and closed. |
| PHASE-02 | Core Data | Core schema, reference entities, workflow rules, validation base | Yes | Accepted | Schema and workflow preparation completed. No status-history or audit tables in MVP. |
| PHASE-03 | Core Screens | Main navigation, checks, banks, parties, users | Yes | Accepted | All five MVP screens were implemented and accepted through `TASK-0015`. |
| PHASE-04 | Quality & Fixes | Security review, periodic quality review, validation fixes, post-review corrections | Yes | Accepted | `ISSUE-0006`, `ISSUE-0007`, and `ISSUE-0008` resolved. Reviews and follow-up fixes completed. |
| PHASE-05 | Final Review & Handover | Plan compliance review, handover documentation, final MVP readiness summary | Yes | Planned | Enabled by `DEC-0017`, but not yet executed as a project phase. |
| PHASE-06 | Future Enhancements | Status history, audit log, export/import, richer reporting | No | Deferred | Explicitly outside MVP per approved preparation files. |

---

## Usage Rules

- Read this file before choosing the next major task when roadmap tracking is active.
- Update this file when a phase or sub-phase is started, accepted, deferred, cancelled, blocked, or moved.
- Do not treat `Deferred`, `Cancelled`, or `Out of Scope` items as missing work.
- Link detailed follow-up work through `PROJECT_DETAILED_EXECUTION_PLAN.md`.
