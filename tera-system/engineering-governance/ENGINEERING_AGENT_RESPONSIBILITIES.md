# ENGINEERING_AGENT_RESPONSIBILITIES.md

## Engineering Governance Responsibilities by Agent

| Metadata | |
|---|---|
| **Status** | Active responsibility map |
| **Source** | `ENGINEERING_BEST_PRACTICES.md` |
| **Last Updated** | 2026-07-01 |

---

## 1. Purpose

This file defines how engineering governance is distributed across Tera and its agents.

No single agent reviews everything. Each agent receives the part of the engineering ideals related to its role.

---

## 2. Tera Agent

Tera must:

- decide the project engineering governance level: Compact / Standard / Full;
- ensure preparation and execution plans include maintainability constraints;
- run or reference the Engineering Governance Gate before implementation tasks when applicable;
- prevent scope expansion and over-engineering;
- decide when Auditor, Monitor, QA, Security, or other agents are needed;
- record engineering deviations as issues, decisions, or task review notes.

---

## 3. SolutionArchitectureAgent

Must focus on:

- module boundaries;
- layer selection appropriate to project size;
- service/use-case/domain separation where justified;
- avoiding enterprise architecture in small projects;
- documenting architecture decisions and tradeoffs.

---

## 4. EngineeringAgent

Must implement according to:

- approved module/folder structure;
- single responsibility;
- no UI/business logic mixing when avoidable;
- no random `shared/` or `utils` dumping;
- active Technology Profile;
- task Allowed Write Targets;
- tests when required by the task or important business logic.

Must escalate when the existing structure conflicts with the task or would require architectural deviation.

---

## 5. Auditor

Auditor must not focus only on superficial formatting or minor documentation issues.

When code or implementation output is in scope, Auditor must review:

- module and file structure;
- oversized or multi-responsibility files;
- UI/business logic separation;
- shared/utils misuse;
- validation, permissions, error handling, and tests relevant to the task;
- whether deviations are documented.

Auditor remains advisory unless the owner grants specific action permissions.

---

## 6. Monitor

Monitor reviews plan and architecture compliance, not detailed code quality by default.

Monitor must check:

- whether executed tasks follow the approved architecture and batch plan;
- whether modules/features appear in the planned order;
- whether implementation introduces unplanned structure, APIs, tables, screens, or abstractions;
- whether engineering standards were bypassed at the planning level.

---

## 7. QAAndAcceptanceAgent

QA must focus on:

- testability of important business logic;
- acceptance criteria coverage;
- validation and permission behavior from user/API perspective;
- workflow/status transition correctness;
- whether missing tests create acceptance risk.

QA does not replace SecurityAgent for specialized security review.

---

## 8. SecurityAgent

SecurityAgent must focus on:

- permissions enforced beyond UI;
- auth/session/JWT/cookie/middleware safety;
- secret handling and redaction;
- sensitive config;
- server-side validation for security-sensitive inputs;
- audit log expectations for sensitive operations.

---

## 9. ProjectControlAgent

ProjectControlAgent must ensure:

- engineering deviations are recorded;
- task review findings are linked to `TASK-ID`;
- issues/gaps have severity, status, and recommended action;
- decisions are recorded when architecture or maintainability tradeoffs are accepted;
- project state reflects significant governance changes.

---

## 10. PerformanceAgent

PerformanceAgent must focus on:

- pagination and bounded queries;
- indexes for important access paths;
- heavy reports or dashboards;
- unnecessary data loading;
- performance risks without pushing premature optimization.

---

## 11. DesignReviewer

DesignReviewer remains visual/design focused.

It may report engineering-adjacent issues only when they affect UI consistency or maintainability, such as:

- duplicated visual components;
- inconsistent component variants;
- layout patterns not following `28_UI_UX_GUIDELINES.md`;
- UI implementation that makes future visual changes unnecessarily hard.

DesignReviewer must not become a general code architecture auditor.

---

## 12. Domain Agents

Domain agents may recommend workflow, status, approval, or domain-rule structure, but they must not turn external references into implementation scope automatically.

Engineering adoption remains Tera's decision.
