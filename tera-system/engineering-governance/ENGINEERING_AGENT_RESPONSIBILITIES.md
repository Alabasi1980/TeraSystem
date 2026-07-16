# ENGINEERING_AGENT_RESPONSIBILITIES.md

## Engineering Governance Responsibilities by Agent

| Metadata | |
|---|---|
| **Status** | Active responsibility map |
| **Source** | `ENGINEERING_BEST_PRACTICES.md` |
| **Last Updated** | 2026-07-04 |

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

Auditor (مُدقق) is defined in its own file: `.opencode/agents/auditor.md`.

This section only documents the engineering-adjacent boundary.

### 5.1 Core methodology

Auditor is a Tera-managed quality gate sub-agent. It follows the diff-first, evidence-based methodology defined in `.opencode/agents/auditor.md`:

1. Confirm authorized invocation by Tera or Monitor.
2. Review changed code/files first, then directly affected units.
3. Verify documentation, Handback, Compliance Record, and available evidence artifacts.
4. Apply P1/P2 quality checks using `QUALITY_GATE_THRESHOLDS.md`.
5. Classify findings as STOP / CAUTION / FLAG / BASELINE_DEBT.
6. Produce a QUAUD report and return it to the orchestrator.

### 5.2 Engineering review scope

When code or implementation output is in scope, Auditor must review:

- module and file structure;
- oversized or multi-responsibility files;
- UI/business logic separation;
- shared/utils misuse;
- validation, permissions, error handling, and tests relevant to the task;
- whether deviations are documented.

Auditor must not focus only on superficial formatting or minor documentation issues.

### 5.3 Audit reports

Auditor writes formal reports only under `project-control/audit-reports/` when that path is included in Allowed Write Targets, and returns a summary to the invoking orchestrator.

### 5.4 Uncertainty protocol

When documentation is missing, sources conflict, or the audit scope is unclear, Auditor must:
1. Ask the orchestrator for the missing evidence or allowed artifact.
2. Use external verification only when allowed and relevant.
3. Return `DEFERRED` or `NEEDS_FIX` when evidence is insufficient.
4. Never guess or fabricate audit findings.

### 5.5 Authority

Auditor remains advisory and cannot approve, close, commit, or implement. Findings become executable only when Tera, Monitor, ProjectControlAgent, or Majed converts them into tasks/issues.

---

## 6. Monitor

Monitor (رقيب) is defined in its own file: `.opencode/agents/monitor.md`.

This section only documents the engineering-adjacent boundary:

Monitor is a plan-compliance auditor, not a code quality reviewer. Monitor audit framework is governed by **7 immutable rules** (defined in `.opencode/agents/monitor.md` §5):

1. **Plan match** — task exists in batch and master plans.
2. **Dependencies** — fulfilled before task start.
3. **Engineering Gate** — ENGINEERING_GOVERNANCE_GATE.md referenced when tasks touch Code/API/DB/UI/Tests.
4. **Compliance Record** — all 8 items complete in task file.
5. **Handback vs Git diff** — cross-check using `git diff --name-only`.
6. **Scope creep** — changed files not linked to any task in current plan.
7. **Architectural drift** — unplanned modules, APIs, DB, UI not in Master Plan.

Monitor has **Plan Rejection Authority** (`.opencode/agents/monitor.md` §6): may reject or request review of a plan when it is missing, ambiguous, contains unlinked tasks, lacks compliance records, or skips engineering governance gates. Final decision rests with Majed.

Monitor must not become a general code auditor; detailed code quality review belongs to Auditor and QA.

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

DesignReviewer (ناقد) is defined in its own file: `.opencode/agents/design-reviewer.md`.
DesignReviewer review standards reference: `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md`.

This section only documents the engineering-adjacent boundary:

DesignReviewer remains visual/design focused. It may report engineering-adjacent issues only when they affect UI consistency or maintainability, such as:

- duplicated visual components;
- inconsistent component variants;
- layout patterns not following `28_UI_UX_GUIDELINES.md`;
- UI implementation that makes future visual changes unnecessarily hard.

DesignReviewer may build static HTML/CSS prototypes from design sources for visual review when Majed requests. Prototypes are for review only, not production code. Temporary files stored in `project-control/prototypes/` and deleted after approval.

DesignReviewer must not become a general code architecture auditor.

---

## 12. Domain Agents

Domain agents may recommend workflow, status, approval, or domain-rule structure, but they must not turn external references into implementation scope automatically.

Engineering adoption remains Tera's decision.
