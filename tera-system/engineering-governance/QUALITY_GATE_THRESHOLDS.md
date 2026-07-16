# QUALITY_GATE_THRESHOLDS.md

## 1. Purpose

This file defines default quality-gate rule classes, evidence requirements, and P1/P2 thresholds for Auditor.

It is a reference for `.opencode/agents/auditor.md` and must not be treated as a universal automatic pass/fail table.

Core rule:

```text
Threshold exceeded = finding candidate.
Severity depends on impact, context, confidence, evidence quality, and whether the issue is new in the diff.
```

## 2. Rule Classes

| Rule Class | Meaning | Examples | Blocking Behavior |
|---|---|---|---|
| Hard rules | Direct safety/security/governance failures | secrets, unredacted credentials, unsafe raw query with user input, unauthorized permission expansion | May produce STOP |
| Default heuristics | Useful indicators, not universal failures | file size, function size, parameter count, nesting, TODO count | Finding only; context decides severity |
| Project-calibrated rules | Need artifact, baseline, language, or analyzer | coverage, duplication %, churn, CBO, vulnerability status | Cannot be asserted without evidence |

## 3. Evidence Requirements

| Finding Type | Required Evidence |
|---|---|
| File/function size | Direct file read is enough |
| Function complexity | Analyzer report, AST evidence, or clearly traceable manual reasoning |
| Duplication percentage | Static analyzer report only; otherwise call it suspected duplication |
| Coverage | QA report or coverage artifact only |
| Flaky tests | QA/CI history only |
| Vulnerable dependency | SecurityAgent report, dependency scanner, or authoritative advisory |
| Dependency freshness | Not a finding by itself; require vulnerability, EOL, incompatibility, or unused dependency |
| Technical Debt Ratio | Tool report only |
| CBO/coupling metric | Analyzer or explicit dependency evidence |
| Circular dependency | Analyzer or clear import-chain evidence |
| Code churn | Git history evidence |

## 4. Default Heuristics

These are default signals, not hard failures.

| Metric | Flag Candidate | Caution Candidate | Notes |
|---|---:|---:|---|
| Function length | 50+ lines | 100+ lines | Context matters; generated or algorithmic code may differ |
| File length (production) | 300+ lines | 500+ lines | New large files need stronger justification |
| Parameter count | 7+ | 10+ | Prefer parameter object or clearer data model where appropriate |
| Nesting depth | 4+ | 6+ | Manual readability signal |
| TODO/FIXME/HACK in changed code | any | repeated or critical path | Must distinguish temporary note from hidden debt |
| Suspected duplication | visible repeated block | repeated business logic | No percentage without analyzer |

## 5. Hard Rule Examples

| Rule ID | Condition | Typical Severity |
|---|---|---|
| QG-SEC-001 | Real secret or credential in code/task/log/report | STOP |
| QG-SEC-002 | Unredacted connection string outside approved local env files | STOP |
| QG-SEC-003 | Unsafe raw query or string-concatenated query using user input | STOP / CAUTION |
| QG-SEC-004 | Unsafe eval/command execution/deserialization in changed code | STOP / CAUTION |
| QG-GOV-001 | Unauthorized permission expansion or scope expansion in changed work | STOP / CAUTION |
| QG-ARCH-001 | New circular dependency proven by import chain or analyzer | CAUTION / STOP if architecture-critical |

## 6. Testing Adequacy Rules

Auditor does not run tests.

Review questions:
- Did behavior change?
- Are relevant tests present or clearly not applicable?
- Is there QA evidence when acceptance requires execution?
- Are normal, failure, and edge paths covered where relevant?
- Is critical logic changed without visible test evidence?

Avoid hard use of test-to-code ratio. It is only an advisory smell and can be gamed.

`Zero assertions` is not automatically STOP because snapshots, property-based tests, exception tests, fixtures, and setup files require context.

## 7. UI Code Accessibility Baseline

Use code-level checks only:
- missing accessible name for interactive controls
- non-semantic interactive elements without keyboard support
- missing/incorrect label-field relationship
- incorrect/conflicting ARIA
- missing focus/loading/error/disabled-state handling where relevant

Do not require ARIA where semantic HTML already provides the correct role/name/value.

## 8. Diff-First and Baseline Debt

Auditor reviews changed code first. Existing debt outside the diff is `BASELINE_DEBT`.

Do not block a task for unrelated baseline debt unless it is critical and the current change exposes, worsens, or relies on it.

## 9. Audit Modes

| Mode | Use When | Scope |
|---|---|---|
| Documentation | Documentation-only change | governance/document consistency |
| Light | config/small code | P1 + relevant P2 checks |
| Standard | normal feature/UI change | P1 + P2 plus applicable quality domains |
| Full Risk-Based | architecture/security-sensitive | expanded risk-based review of all applicable domains; not exhaustive proof |

## 10. Calibration

P3/P4/P5 will calibrate thresholds after operational feedback.

Until then:
- apply strict blocking only to hard rules with clear evidence
- treat size and complexity thresholds as heuristics
- cite artifacts for numeric claims
- prefer actionable findings over broad criticism
