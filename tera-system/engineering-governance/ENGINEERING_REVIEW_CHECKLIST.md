# ENGINEERING_REVIEW_CHECKLIST.md

## Engineering Review Checklist

| Metadata | |
|---|---|
| **Status** | Active checklist |
| **Used By** | Auditor, Tera, Monitor, QA, Engineering reviewers |
| **Source** | `ENGINEERING_BEST_PRACTICES.md` + `ENGINEERING_GOVERNANCE_GATE.md` |
| **Last Updated** | 2026-07-01 |

---

## 1. Review Scope

Use this checklist when reviewing code, implementation batches, task outputs, or release readiness.

Review only the relevant areas. Do not expand a small review into a full audit unless Tera or the owner asks for it.

---

## 2. Architecture and Structure

- [ ] Modules/features are clear.
- [ ] Code is not organized only by random generic folders.
- [ ] `core/`, `shared/`, and `modules/` boundaries are respected where applicable.
- [ ] Module-specific logic is not placed in `shared/`.
- [ ] Cross-module dependencies are intentional and not circular/random.
- [ ] New abstractions are justified by current need, not speculation.

---

## 3. File and Responsibility Health

- [ ] No changed file is clearly handling unrelated responsibilities.
- [ ] Files approaching `300–400` lines are reviewed for responsibility creep.
- [ ] Functions/components have clear names and one primary purpose.
- [ ] Large orchestration functions are split into readable operations when useful.

---

## 4. UI / Application / Domain Separation

- [ ] UI components are not owners of business rules.
- [ ] Important business operations live in services/use cases/domain/rules according to project size.
- [ ] Data mapping/formatting is separated when it becomes non-trivial.
- [ ] UI changes do not introduce hidden API/security/database behavior.

---

## 5. Shared Code and DRY

- [ ] Shared code is genuinely shared by more than one module.
- [ ] No `utils` dumping ground was created or expanded without structure.
- [ ] Duplicate logic is left, watched, or extracted according to practical repetition and risk.
- [ ] Extracted helpers have clear names and narrow responsibility.

---

## 6. Validation, Errors, and Permissions

- [ ] Validation is not frontend-only when data integrity matters.
- [ ] Backend/API/service validation exists for important inputs.
- [ ] Error format and messages follow project conventions.
- [ ] Permissions are enforced beyond UI hiding when security matters.
- [ ] Security-sensitive changes are escalated to SecurityAgent or Tera as required.

---

## 7. Database and API

- [ ] Database schema changes match the approved task and migration strategy.
- [ ] No manual or untraceable database change is implied.
- [ ] Naming and relations are clear.
- [ ] Indexes/pagination are considered for important query paths.
- [ ] API response and error behavior is consistent.

---

## 8. Testing and Acceptance

- [ ] Important business rules have tests or a documented reason for deferral.
- [ ] Permission and validation behavior is testable.
- [ ] API or integration behavior has appropriate verification.
- [ ] UI/E2E tests are used only when they add justified confidence.
- [ ] Acceptance criteria are mapped to actual output.

---

## 9. Documentation and Decisions

- [ ] Architecture-affecting changes are documented.
- [ ] Deviations are recorded in `DECISIONS_LOG.md` or task review notes.
- [ ] Risks/gaps are recorded in `ISSUES_AND_GAPS.md`.
- [ ] The task handback names changed files and meaningful decisions.

---

## 10. Review Output Classification

Classify every finding as one of:

| Classification | Meaning |
|---|---|
| Must Fix Now | Blocks acceptance or creates serious maintainability/security risk |
| Should Fix Soon | Important but can be scheduled safely |
| Defer With Record | Not blocking current phase, must be tracked |
| Observation | Useful note, no action required now |
| Not Applicable | Outside current review scope |

Auditors and reviewers must not turn every observation into a blocking issue.
