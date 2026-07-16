# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-098.md

## Title
Convert Auditor into a Tera-Managed Quality Gate Sub-Agent — Diff-First, Evidence-Based Quality Review

## Revision
- **Version:** v2
- **Revision Source:** Majed clarification + expert review
- **Revision Result:** Previous draft `NEEDS_FIX` findings incorporated.

## Request Type
Agent Role Conversion + Quality Gate Governance + Runtime Integration

## Problem

The current Auditor agent is defined as an independent governance session agent, manually activated by Majed. That no longer matches the agreed operating model.

Majed clarified that Auditor should become a **sub-agent under Tera or another authorized primary agent**, not a separately summoned agent by Majed during normal work. Majed may still request an Auditor review indirectly through Monitor when he does not trust Tera's own review.

The previous SCP draft also had quality-governance issues:

1. It treated Auditor as both manually activated by Majed and invoked by Tera, creating an authority contradiction.
2. It treated several research thresholds as hard universal pass/fail rules.
3. It did not define a strict evidence model for metrics that require analyzers or artifacts.
4. It did not make the audit scope explicitly `diff-first`.
5. It blurred `STOP` and `CAUTION` severity behavior.
6. It overstated what a read-only Auditor can prove without tests, CI history, or analyzers.
7. It used imprecise UI accessibility language such as "missing ARIA".
8. It mentioned implementation waves without defining their content.

## Evidence

### Current Auditor State

- `.opencode/agents/auditor.md` currently defines Auditor as independent and manually activated by Majed.
- `AGENT_ACTIVATION_MATRIX.md` does not include Auditor as a Tera-managed quality sub-agent.
- `AGENT_DEPENDENCY_MAP.md` lists `auditor.md` as invoked by Majed only.
- `TeraPreExecutionGate.md` Post-Execution Review Gate references independent review decisions for ProjectControlAgent, SecurityAgent, and QAAndAcceptanceAgent, but not Auditor.

### Research Basis

The research report archived at:

```text
project-control/archive/RESEARCH_TO_SYSTEM_CHANGE_REPORT_AUDITOR_TRANSFORMATION.md
```

reviewed 20 sources, including SonarQube, SEI/CMU ATAM, OWASP, Google Engineering Practices, Microsoft Maintainability Index, WCAG 2.2, ISO/IEC 25010, McCabe complexity, and Hatton module-size research.

The research supports using quality thresholds as **default heuristics and evidence-backed indicators**, not as automatic universal failure rules except for hard safety/security violations.

## Affected Files

| File | Change Type | Description |
|:-----|:-----------:|:------------|
| `.opencode/agents/auditor.md` | Major role conversion | Convert from primary/manual agent to quality-review sub-agent; add diff-first scope, evidence model, severity model, report format, and referral protocol. |
| `.opencode/agents/tera.md` | Runtime integration | Add compact Auditor quality-gate invocation logic after important implementation tasks. |
| `.opencode/agents/monitor.md` | Governance path update | Allow Monitor, when explicitly asked by Majed, to request Auditor review as an independent check of Tera's work. |
| `tera-system/TeraSubAgents.md` | Registry update | Register Auditor as a quality gate sub-agent and define authorized orchestrators. |
| `tera-system/AGENT_ACTIVATION_MATRIX.md` | Trigger update | Add Auditor triggers using risk, architecture, change-size, quality-signal, and explicit-request categories. |
| `tera-system/AGENT_DEPENDENCY_MAP.md` | Relationship update | Change Auditor invocation from Majed-only to authorized orchestrators: Tera by default, Monitor by Majed request. |
| `tera-system/TeraPreExecutionGate.md` | Gate update | Add Auditor to Post-Execution independent review decision as a quality-review gate, separate from QA/Security/ProjectControl. |
| `tera-system/engineering-governance/QUALITY_GATE_THRESHOLDS.md` | New reference file | Store rule classes, evidence requirements, thresholds, calibration notes, and examples. |
| `tera-system/TeraPolicyMap.md` | Map update | Register the new threshold reference as the source for quality-gate thresholds. |

**No `TeraArchitectureMap.md` change expected** because the new threshold file remains inside the existing engineering-governance area and does not create a new architectural layer.

## Proposed Change

### 1. Authority Model Correction

Convert Auditor from:

```text
Independent primary/manual governance session agent
```

to:

```text
Tera-managed Quality Gate Sub-Agent
```

Authorized orchestrators:

| Orchestrator | When Allowed |
|:-------------|:-------------|
| `TeraAgent` | Normal post-execution quality review for application tasks. |
| `Monitor` | When Majed asks Monitor to verify or challenge Tera's work and Monitor needs Auditor quality review. |
| Other primary agents | Not allowed by default; must be added explicitly through future SCP if needed. |

Majed does not normally activate Auditor directly. Majed may request an authorized primary agent, such as Monitor, to obtain an Auditor review.

### 2. Activation Decision Model

Tera, or another authorized orchestrator, must classify every completed implementation task as one of:

```text
AUDITOR_REVIEW_REQUIRED
AUDITOR_REVIEW_RECOMMENDED
AUDITOR_REVIEW_NOT_REQUIRED
AUDITOR_REVIEW_WAIVED_BY_MAJED
```

Rules:

- `REQUIRED` means the orchestrator must invoke Auditor before final acceptance or must record an explicit Majed waiver.
- `RECOMMENDED` means the orchestrator may proceed without Auditor only with a documented reason.
- `NOT_REQUIRED` must include a short reason.
- `WAIVED_BY_MAJED` must name the reason and the risk accepted.

### 3. Activation Triggers

File count alone is not a sufficient trigger. Use compound triggers:

| Trigger Class | Required When | Recommended When |
|:--------------|:--------------|:-----------------|
| Risk trigger | Auth, authorization, payments, secrets, migrations, data mutation, config/security-sensitive logic | Non-sensitive config or environment handling |
| Architecture trigger | New module/service, shared component, public API, dependency direction change, cross-layer change | Refactor affecting one bounded module |
| Change-size trigger | Large diff, many files, or multi-module change; file count is supporting evidence only | 4-10 files or moderate diff |
| Quality-signal trigger | Post-execution review found complexity, missing tests, suspicious coupling, or scope side effect | Heuristic threshold exceeded without clear impact |
| Explicit request | Majed asks through Tera/Monitor or another approved orchestrator | Majed asks for advisory check |

Examples:

- One-file authentication change can be `REQUIRED`.
- Twenty-file rename-only change can be `NOT_REQUIRED` or `RECOMMENDED` depending on risk.

### 4. Diff-First Audit Scope

Auditor must review in this order:

1. Changed code and newly added code.
2. Directly affected neighboring units.
3. Wider architecture only when the diff introduces architectural risk.
4. Existing issues outside the diff are recorded as `BASELINE_DEBT`.

Acceptance rule:

- A task must not be blocked for old debt it did not create, except when the change exposes, worsens, or relies on a critical existing risk such as leaked secrets or unsafe authorization.

### 5. Rule Classes

Quality rules must be split into three classes:

| Rule Class | Meaning | Examples | Blocking Behavior |
|:-----------|:--------|:---------|:------------------|
| Hard rules | Safety/security/governance failures with direct risk | Real secrets, unauthorized permission expansion, circular dependency forbidden by architecture, unsafe raw query with user input | Can produce `STOP` |
| Default heuristics | Useful quality indicators, not universal failures | Function length, file length, parameter count, TODO count, god-object suspicion | Produce findings; severity depends on context and impact |
| Project-calibrated rules | Rules that require project baseline, language, or artifact | Coverage, duplication %, CBO, churn, dependency vulnerability status | Cannot be asserted without evidence artifact |

Core rule:

```text
Exceeding a threshold creates a finding, not an automatic severity.
Severity depends on impact, context, evidence quality, and whether the issue is new in the diff.
```

### 6. Evidence Model

Auditor is read-only and must not invent metrics. Every numeric finding must state its evidence source.

| Metric / Finding Type | Required Evidence Source |
|:----------------------|:-------------------------|
| File/function size | Direct file reading is enough. |
| Function complexity | Analyzer report, AST-based evidence, or clearly traceable manual reasoning. |
| Duplication percentage | Static analyzer report only; otherwise report as suspected duplication, not a percentage. |
| Coverage | QA report or coverage artifact only. |
| Flaky tests | QA/CI history only. |
| Vulnerable dependency | SecurityAgent report, dependency scanner, or authoritative advisory. |
| Dependency freshness | Not a finding by itself; only vulnerability, EOL, incompatibility, or unused dependency matters. |
| Technical Debt Ratio | Tool report only. |
| CBO / coupling metrics | Analyzer or explicit import/dependency evidence. |
| Circular dependency | Dependency analyzer or clear import-chain evidence. |
| Code churn | Git history evidence. |

### 7. Severity Model

Use finding-level severity:

| Severity | Meaning | Acceptance Behavior |
|:---------|:--------|:--------------------|
| `STOP` | Critical violation; ordinary waiver not allowed | Overall gate becomes `BLOCKED` |
| `CAUTION` | Significant risk needing fix, explicit acceptance, or documented waiver | Overall gate becomes `NEEDS_FIX` while open; multiple CAUTIONs may escalate |
| `FLAG` | Advisory improvement or baseline debt | Does not block `PASS` by itself |

Overall gate result:

| Condition | Result |
|:----------|:-------|
| Any open `STOP` | `BLOCKED` |
| No STOP, but open `CAUTION` | `NEEDS_FIX` |
| Only `FLAG` or resolved findings | `PASS` |
| Required evidence missing | `DEFERRED` or `NEEDS_FIX`, depending on whether the missing evidence is required for acceptance |

### 8. Quality Domains

Auditor will review these domains, with boundaries:

| Domain | Auditor Checks | Boundaries |
|:-------|:---------------|:-----------|
| Code Quality | Directly visible size, nesting, naming, obvious complexity, analyzer-provided complexity when available | Does not run linters/tests; does not invent metrics |
| Architecture Health | New module boundaries, dependency direction, circular imports, shared-component misuse, layer violations | Full ATAM or architecture decisions stay outside scope |
| File Structure | Oversized files, misplaced code/config, mixed responsibilities, unsafe generated files | Size thresholds are heuristics |
| Security Hygiene | Secrets, unsafe raw queries, unsafe eval/deserialization, weak crypto, unsafe config patterns | Deep auth logic or penetration/security testing goes to SecurityAgent |
| Testing Adequacy | Whether behavior changed, relevant tests exist, QA artifact exists, critical paths have visible test coverage | Does not run tests; test-to-code ratio is advisory only |
| UI Code Accessibility | Semantic HTML, accessible names, form labels, keyboard access, focus/loading/error states, invalid ARIA usage | Visual aesthetics go to DesignReviewer |
| Maintainability | TODO/FIXME/HACK in changed code, deprecated APIs, suspicious workarounds, baseline debt notes | TDR/churn only with evidence artifacts |

### 9. Testing Review Reset

Auditor must not treat `test-to-code ratio >= 0.5` as a hard rule.

Auditor should ask:

- Did the task change behavior that should be testable?
- Are relevant tests present for the changed behavior?
- Do tests cover normal, failure, and edge paths when applicable?
- Is there evidence from QA or coverage artifacts?
- Is there critical logic with no visible related test?
- Are assertions meaningful, or is the test only superficial?

`Zero assertions` is not automatically `STOP`; snapshot tests, property-based tests, exception-based tests, fixtures, and setup files require context.

### 10. UI Accessibility Reset

Replace "missing ARIA" with accurate accessibility checks:

- Missing accessible name for interactive controls.
- Non-semantic elements used as controls without keyboard support.
- Missing or incorrect label-field relationships.
- Incorrect or conflicting ARIA.
- Missing focus, loading, error, or disabled-state handling where relevant.
- Responsive breakpoints or touch targets only when visible from code or design rules.

### 11. Audit Modes by Change Type

| Change Type | Audit Mode | Domains |
|:------------|:----------:|:--------|
| Documentation only | Governance/document consistency | No code quality scan |
| Configuration | Light | Security hygiene + config correctness + scope compliance |
| Small code change | Light/Standard | Code quality + testing adequacy |
| UI change | Standard | Code quality + UI accessibility + testing adequacy |
| Architecture/security-sensitive | Full risk-based | All applicable domains |

Rename `Full = Exhaustive` to:

```text
Expanded risk-based review of all applicable domains.
```

No claim of exhaustive proof is allowed.

### 12. Output Format

Auditor output should use a structured `QUAUD` report:

```text
Audit ID:
Task Reviewed:
Invoked By: Tera / Monitor / Other approved orchestrator
Audit Mode: Documentation / Light / Standard / Full Risk-Based
Scope: Changed Code / Affected Units / Expanded Risk Scope
Evidence Sources Used:

Overall Quality Gate: PASS / NEEDS_FIX / BLOCKED / DEFERRED

Findings Summary:
- STOP:
- CAUTION:
- FLAG:
- BASELINE_DEBT:

Finding:
  Finding ID:
  Rule ID:
  Domain:
  Severity:
  Location:
  Evidence:
  Expected Standard:
  Observed Condition:
  Impact:
  Recommended Action:
  Changed Code / Baseline:
  Confidence: High / Medium / Low
  Blocking: Yes / No
  Blocking Reason:
  Waiver Allowed: Yes / No
  Required Owner:
  Referral:
  Status: Open / Accepted / Deferred / Resolved
```

### 13. Implementation Phases

Do not implement all quality domains at once. Use staged rollout:

| Phase | Content | Acceptance Criteria |
|:------|:--------|:--------------------|
| P1 Foundation | Sub-agent conversion, authorized orchestrators, severity model, diff-first scope, QUAUD report | Auditor can be invoked by Tera/Monitor and produce structured evidence-based reports |
| P2 Core Checks | Secrets, obvious unsafe patterns, file/function size heuristics, test existence, circular import evidence | High-value checks work without heavy tooling |
| P3 Architecture/UI | Layering, dependency direction, semantic accessibility, state/loading/error patterns | Risk findings are contextual and referral-safe |
| P4 Evidence Integration | Consume QA, SecurityAgent, coverage, analyzer, and CI artifacts when available | Auditor stops inventing unavailable metrics and cites artifacts |
| P5 Calibration | Legacy baseline, false-positive review, language/profile adjustments | Thresholds become calibrated per project type and stack |

## Why This Is Necessary

1. Auditor's current independent/manual model no longer matches Majed's desired operating model.
2. Tera needs a specialized quality sub-agent because Tera's Post-Execution Gate checks operational compliance, not deep quality signals.
3. QA, Monitor, DesignReviewer, and SecurityAgent each cover different concerns; Auditor fills the quality-review gap without taking over their work.
4. Research-backed thresholds are useful only when evidence, context, and diff scope are respected.
5. The revised model prevents quality review from becoming noisy, over-blocking, or falsely precise.

## Rejected Alternatives

| Alternative | Why Rejected |
|:------------|:-------------|
| Keep Auditor as Majed-manual primary agent | Conflicts with Majed's clarified model: Auditor should be a sub-agent managed by Tera or an authorized primary agent. |
| Let Tera only classify and wait for Majed direct activation | Too slow and contradicts the sub-agent model. Majed can still waive or request review through Monitor. |
| Expand QA Agent into quality auditor | QA runs functional tests; Auditor reads and analyzes quality evidence. Different role and permission model. |
| Create a new QualityAuditorAgent | Anti-bloat violation; existing Auditor is the correct agent to evolve. |
| Make thresholds hard global rules | Produces false failures; many thresholds are heuristics or project-calibrated. |
| Full all-domain review after every task | Too expensive and noisy; diff-first + trigger-based review is safer. |

## Anti-Bloat Check

| Question | Answer |
|:---------|:-------|
| What problem does this solve? | No sub-agent currently performs evidence-based code quality, architecture, maintainability, UI-code, and security-hygiene review after implementation. |
| Why not use an existing file only? | Auditor file needs role conversion; thresholds need a compact central reference to avoid bloating the agent prompt. |
| Why not use an existing agent? | QA tests, Monitor checks plan compliance, DesignReviewer checks visual design, SecurityAgent performs deep security. None owns quality gate auditing. |
| Does this reduce or increase complexity? | It increases capability but controls complexity through authorized orchestrators, diff-first scope, staged rollout, and evidence rules. |
| Token impact? | Controlled by audit modes and compact threshold reference. No full-project scan by default. |
| Smaller alternative? | Only adding a note to Tera would be insufficient; quality review needs a defined agent contract. |
| File size impact? | Auditor likely remains under the 700-line split threshold if written compactly and thresholds stay in a reference file. |

## Risk

| Risk | Likelihood | Impact | Mitigation |
|:-----|:----------:|:------:|:-----------|
| Over-broad invocation by "any main agent" | Medium | High | Only Tera and Monitor are authorized in this SCP; others require future SCP. |
| Auditor over-blocks due to heuristics | Medium | High | Thresholds create findings, not automatic severity. |
| Metrics invented without tools | Medium | High | Evidence model prohibits unsupported numeric claims. |
| Auditor overlaps QA/Security/DesignReviewer | Medium | Medium | Referral protocol and boundaries define ownership. |
| Legacy debt blocks unrelated work | Medium | Medium | Diff-first scope and `BASELINE_DEBT` rule. |
| Runtime bloat in Tera | Low | Medium | Tera receives compact trigger summary only; details stay in Auditor and threshold reference. |

## Rollback Plan

If approved changes cause operational friction:

1. Revert `.opencode/agents/auditor.md` to independent/manual model.
2. Remove Auditor entries from `TeraSubAgents.md` and `AGENT_ACTIVATION_MATRIX.md`.
3. Remove Auditor invocation logic from `.opencode/agents/tera.md` and `.opencode/agents/monitor.md`.
4. Revert `AGENT_DEPENDENCY_MAP.md` and `TeraPreExecutionGate.md` updates.
5. Remove `QUALITY_GATE_THRESHOLDS.md` and its `TeraPolicyMap.md` entry if no longer used.

This change is **not purely additive**; it changes Auditor's authority model and runtime invocation path.

## Approval Required
Yes — Majed approval is required before implementation.

## Decisions Required Before Implementation

1. Confirm Auditor frontmatter should change from `mode: primary` to `mode: subagent`.
2. Confirm authorized orchestrators for v1 are only `TeraAgent` and `Monitor`.
3. Confirm whether Auditor writes no files and returns reports to the orchestrator, or may write audit reports to `project-control/audit-reports/` in a later phase.
4. Confirm whether P1 only should be implemented first, or P1 + P2 together.

## Research Reference
Full research findings archived in:

```text
project-control/archive/RESEARCH_TO_SYSTEM_CHANGE_REPORT_AUDITOR_TRANSFORMATION.md
```

Research agents used:
- `domain-research-agent` — data gathering from 20 sources
- `domain-expert-agent` — analysis and recommendations
