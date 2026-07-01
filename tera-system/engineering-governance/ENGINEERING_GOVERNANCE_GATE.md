# ENGINEERING_GOVERNANCE_GATE.md

## Tera Engineering Governance Gate

| Metadata | |
|---|---|
| **Status** | Active system gate |
| **Source** | `tera-system/engineering-governance/ENGINEERING_BEST_PRACTICES.md` |
| **Used By** | Tera Pre-Execution Gate, Post-Execution Review, Engineering agents, Auditor, Monitor, QA |
| **Last Updated** | 2026-07-01 |

---

## 1. Purpose

This gate turns engineering best practices into operational checks before and after implementation tasks.

It does not replace:

- `TeraPreExecutionGate.md`;
- active Technology Profiles;
- Design Governance;
- Security review;
- QA acceptance review.

It adds maintainability and architecture checks to prevent structural decay.

---

## 2. When the Gate Applies

Apply this gate when a task creates or modifies any of the following:

- application source code;
- modules/features;
- UI components with logic;
- API/server actions/controllers;
- services/use cases/domain rules;
- validation logic;
- permissions/security checks;
- database schema/migrations/repositories;
- shared utilities/components;
- tests;
- architecture or coding standards.

For documentation-only edits, apply only if the document defines engineering rules.

---

## 3. Project Size Adjustment

| Project Level | Gate Strictness |
|---|---|
| Compact | Check obvious bloat, module clarity, no UI/business logic mixing for important rules |
| Standard | Check module boundaries, service/use-case separation, validation, permissions, tests when needed |
| Full | Check layered architecture, cross-module coupling, audit/logging, performance, integration boundaries, stronger tests |

Do not force Full architecture on Compact projects unless approved by Tera and the user.

---

## 4. Pre-Execution Checks

Before approving or delegating an implementation task, Tera must verify:

| # | Check | Required Result |
|---|---|---|
| 1 | Does the task identify the affected module or shared/core area? | Yes / N/A |
| 2 | Is the task small enough to avoid architectural mixing? | Yes |
| 3 | Could the task create a file likely to exceed responsibility or size limits? | No / Mitigated |
| 4 | Is business logic kept out of UI where applicable? | Yes / N/A |
| 5 | Is module-specific logic kept out of `shared/`? | Yes / N/A |
| 6 | Are validation rules placed at the correct layer? | Yes / N/A |
| 7 | Are permissions enforced outside frontend when relevant? | Yes / N/A |
| 8 | Are database changes traceable through the approved migration/schema task path? | Yes / N/A |
| 9 | Are API response/error expectations consistent with the project? | Yes / N/A |
| 10 | Are tests required for important logic? If yes, are they in scope or deferred with reason? | Yes / N/A |
| 11 | Does the task avoid unnecessary libraries, folders, abstractions, or docs? | Yes |
| 12 | Does the task reference active Technology Profile rules when stack-specific structure matters? | Yes / N/A |

If a required check fails, the task cannot receive `Pre-Execution Gate: PASS` until corrected, split, or explicitly approved as a documented exception.

---

## 5. Post-Execution Checks

Before accepting or closing an implementation task, Tera or an independent reviewer must verify:

| # | Check | Required Result |
|---|---|---|
| 1 | Changed files are in the approved module/shared/core targets. | Yes |
| 2 | No file became obviously multi-responsibility or oversized without reason. | Yes |
| 3 | UI code did not absorb business rules that belong in services/use cases/domain. | Yes / N/A |
| 4 | Shared utilities/components were not used as dumping grounds. | Yes |
| 5 | Validation exists at backend/service/API layer when required. | Yes / N/A |
| 6 | Permissions are not frontend-only when security matters. | Yes / N/A |
| 7 | Database changes match the approved schema/migration scope. | Yes / N/A |
| 8 | API/error behavior is consistent with project standards. | Yes / N/A |
| 9 | Important logic has tests, or missing tests are recorded as an issue/deferred item. | Yes / Deferred with reason |
| 10 | No unnecessary abstractions, libraries, or folders were introduced. | Yes |
| 11 | Any engineering deviation is recorded in task review, `ISSUES_AND_GAPS.md`, or `DECISIONS_LOG.md` as appropriate. | Yes / N/A |

---

## 6. Failure Results

| Result | Meaning | Required Action |
|---|---|---|
| PASS | Engineering structure is acceptable for the project level | Continue review/acceptance path |
| NEEDS_FIX | Issue can be corrected within same task scope | Return to responsible agent with exact fixes |
| BLOCKED | Requires user/Tera decision, architecture change, or task split | Stop affected task and record blocker |
| DEFERRED_WITH_RECORD | Not blocking now, but must be tracked | Record in `ISSUES_AND_GAPS.md` or future task |

---

## 7. Exception Rule

Engineering ideals may be relaxed only when:

- the project is intentionally Compact;
- the task is a limited scaffold or spike;
- the exception is temporary and recorded;
- Tera documents the reason and follow-up if needed.

Unrecorded exceptions are not valid.
