# ISSUES_AND_GAPS.md

> **Purpose:** Track open issues, gaps, and risks during the project.

## Required Gap Format

```md
## GAP-XXXX - [Short Title]

- Source Task:
- Discovered By:
- Severity: Critical / High / Medium / Low
- Status: Open / Deferred / In Progress / Resolved / Cancelled
- Description:
- Impact:
- Recommended Action:
- Target Task / Phase:
- Owner:
```

**Severity:** Critical / High / Medium / Low
**Status:** Open / Deferred / In Progress / Resolved / Cancelled

**Phase 7 Rule:** No hidden open issues. Any unresolved issue must be documented as Resolved / Deferred / Won't Fix / Requires TASK-COD-FIX before project closure.

## Open Issues and Gaps

## GAP-SYS-0001 - System Template Copying Ambiguity

- Source Task: Tera system maintenance request / Tera 2 analysis
- Discovered By: User + Tera 2
- Severity: Medium
- Status: Resolved
- Description: Tera could misinterpret client-facing template usage and copy `*_TEMPLATE.*` files from `tera-workshop/` into application workspaces as deliverables.
- Impact: Application workspaces may contain duplicated system templates instead of generated client-facing outputs.
- Recommended Action: Clarify that `tera-workshop/*_TEMPLATE.*` files are source templates only and generated outputs must be created under the active application workspace.
- Target Task / Phase: Tera system maintenance / Phase 1 intake and client approval
- Owner: Tera

## GAP-SYS-0002 - Phase 1 Logging and State Closure Weakness

- Source Task: Tera system maintenance request / Tera 2 analysis
- Discovered By: User + Tera 2
- Severity: Medium
- Status: Resolved
- Description: Phase 1 checklist did not explicitly require `PROJECT_ACTIVITY_LOG.md`, `PROJECT_STATE.md`, and `TERA_ACTIVE_CONTEXT.md` updates before leaving intake/discovery.
- Impact: A project could advance with stale operational memory and missing traceability even though individual rules existed elsewhere.
- Recommended Action: Add explicit phase-transition control-record update requirements to system rules and runtime checklist.
- Target Task / Phase: Tera system maintenance / all phase transitions
- Owner: Tera

## GAP-SYS-0003 - Intake Status Completion Not Explicitly Checked

- Source Task: Tera system maintenance request / Tera 2 analysis
- Discovered By: User + Tera 2
- Severity: Low
- Status: Resolved
- Description: Discovery documentation checks did not explicitly require `01_APPLICATION_IDEA.md` and `02_TECHNICAL_CONTEXT.md` status verification before Phase 1 closure.
- Impact: Intake files could remain marked `Partial` while the project proceeds to formal preparation.
- Recommended Action: Add explicit `Complete` or documented exception requirement before leaving Phase 1/Application Discovery.
- Target Task / Phase: Tera system maintenance / Phase 1 intake closure
- Owner: Tera
