# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-098.md

## Title
Transform Auditor into Quality Gate Auditor — World-Class Quality Review with Research-Backed Thresholds

## Request Type
Agent Role Expansion + System Integration

## Problem

The Auditor agent currently operates as a documentation/compliance reviewer with6 roles and a6-phase methodology focused on:
- Documentation completeness
- Compliance record verification
- Basic engineering governance (unit boundaries, bloat, UI/Logic separation)
- Handback reconciliation

**Missing capabilities:**
1. No code quality metrics (complexity, duplication, function/file size)
2. No architecture smell detection (god objects, circular deps, layering violations)
3. No security hygiene patterns (OWASP-based hardcoded secrets, injection, crypto checks)
4. No testing adequacy verification (coverage indicators, test-to-code ratio)
5. No file/module structure health analysis
6. No UI code-level audit (ARIA, responsive, component reuse)
7. No maintainability assessment (TODO tracking, deprecated patterns, churn signals)
8. No tiered severity model (STOP/CAUTION/FLAG)
9. No activation triggers in AGENT_ACTIVATION_MATRIX — only manual activation
10. No integration with Tera's task completion workflow

Meanwhile, the system has:
- QA Agent handling **functional testing** (build/test/run) — not quality patterns
- Monitor handling **plan compliance** — not code quality
- DesignReviewer handling **visual design** — not UI code patterns
- SecurityAgent handling **deep security analysis** — not hygiene patterns
- Tera's Post-Execution Gate handling **operational checks** — not quality metrics

There is a clear gap: no agent performs **world-class quality auditing** with research-backed thresholds.

## Evidence

### Research Sources (20 references, 8 Tier-1 authorities)
- SonarQube "Sonar Way" & "Sonar Way for AI Code" (2026)
- SEI/CMU ATAM Method
- Nielsen Norman Group — 10 Usability Heuristics
- OWASP Secure Code Review Cheat Sheet
- Google Engineering Practices
- Microsoft Maintainability Index
- WCAG 2.2 (W3C/Deque)
- ISO/IEC 25010:2023
- McCabe (1976) — Cyclomatic Complexity
- Hatton (1997) — Optimal Module Size
- And10 additional Tier-2 sources

### Key Thresholds (Industry Consensus)

| Metric | Warning | Critical | Source |
|:-------|:-------:|:--------:|:------:|
| Cyclomatic Complexity | 10 | 20+ | McCabe, SonarQube |
| Code Duplication | 5% | 10%+ | SonarQube |
| Function Length | 50 lines | 100+ | Kiuwan, Matklad |
| File Length (production) | 300 lines | 500+ | Microsoft Monodex |
| Function Parameters | 7 | 10+ | KindaTechnical |
| Technical Debt Ratio | 10% | 20%+ | SonarQube SQALE |

### Current State Gap Analysis

| Capability | Auditor Current | Research Standard | Gap |
|:-----------|:---------------:|:-----------------:|:---:|
| Code Quality Metrics | ❌ | Full SonarQube thresholds | 🔴 Large |
| Architecture Review | ❌ | Fitness functions + smell detection | 🔴 Large |
| Security Hygiene | ❌ | OWASP patterns | 🔴 Large |
| Testing Adequacy | ❌ | Coverage indicators + ratio checks | 🟡 Medium |
| File Structure | Partial (bloat only) | Size thresholds + split criteria | 🟡 Medium |
| UI Code Audit | ❌ | ARIA + responsive + component checks | 🟡 Medium |
| Maintainability | ❌ | TDR + deprecated patterns + churn | 🟡 Medium |
| Severity Model | 4-level (PASS/NEEDS_FIX/BLOCKED/DEFERRED) | STOP/CAUTION/FLAG tiering | 🟡 Needs upgrade |
| Activation Triggers | ❌ Manual only | 5 defined triggers | 🔴 Large |
| System Integration | ❌ Not in activation matrix | Integrated with Tera workflow | 🔴 Large |

## Affected Files

| File | Change Type | Description |
|:-----|:-----------:|:------------|
| `.opencode/agents/auditor.md` | **Major Expansion** | Add 7 quality check domains, severity model, quality output format, referral protocol. Target: ~550 lines (from 363) |
| `.opencode/agents/tera.md` | **Minor Addition** | Add Auditor Quality Gate step in post-task workflow section |
| `tera-system/AGENT_ACTIVATION_MATRIX.md` | **New Entry** | Add Auditor row with 5 activation triggers |
| `tera-system/AGENT_DEPENDENCY_MAP.md` | **Update** | Update Auditor row: now invoked by Tera (not just Majed), adds referrals to SecurityAgent/DesignReviewer |
| `tera-system/engineering-governance/QUALITY_GATE_THRESHOLDS.md` | **New File** | Reference document with research-backed thresholds, project-phase adjustments, intensity matrix |

## Proposed Change

### 1. Auditor Agent Expansion (`.opencode/agents/auditor.md`)

**Keep intact:** Identity, position in system, reference hierarchy, 6-phase methodology, commit protocol, self-improvement suggestions, forbidden actions.

**Add new sections:**

#### A. Quality Gate Check Domains (7 domains)

| # | Domain | What It Checks | Severity |
|:--|:-------|:---------------|:--------:|
| 1 | **Code Quality** | Complexity (≤10/≤20), duplication (≤5%/≤10%), function length (≤50/≤100), file length (≤300/≤500), parameters (≤7/≤10), nesting (≤4/≤6) | STOP/CAUTION/FLAG |
| 2 | **Architecture Health** | God objects (>500 lines, >20 methods), circular deps, layering violations, tight coupling (CBO>7), feature envy, shotgun surgery | STOP/CAUTION/FLAG |
| 3 | **File Structure** | Size thresholds per category, split criteria, naming consistency, config vs code separation | CAUTION/FLAG |
| 4 | **Security Hygiene** | Hardcoded credentials, SQL concatenation, unsafe deserialization, missing input validation, weak crypto, missing HTTPS | STOP/CAUTION |
| 5 | **Testing Adequacy** | Test existence, test-to-code ratio (≥0.5), coverage indicators, missing test categories, flaky test signals | CAUTION/FLAG |
| 6 | **UI Code Quality** | ARIA attributes, responsive breakpoints, component duplication, state management patterns, loading/error states | CAUTION/FLAG |
| 7 | **Maintainability** | TODO/FIXME/HACK count, deprecated API usage, configuration drift, dependency freshness | FLAG |

#### B. Severity Model (STOP/CAUTION/FLAG)

| Signal | Meaning | Action |
|:-------|:--------|:-------|
| **STOP** | Critical: hardcoded secret, zero test assertions, architecture-breaking violation | Block acceptance — must fix before close |
| **CAUTION** | High: complexity >20, file >500, missing ARIA, test ratio <0.2 | Block acceptance — fix or override with justification |
| **FLAG** | Medium: complexity 10-20, file 300-500, TODO count high, coverage gap | Report only — no block, recommended improvements |

#### C. Quality Output Format (QUAUD Report)

```text
Audit ID: QUAUD-YYYY-MM-DD-NNN
Task Reviewed: [TASK-ID]
Audit Mode: Light / Standard / Full

Quality Gate Results:
| # | Domain | Result | Findings Count | STOP | CAUTION | FLAG |
Findings:
| Finding ID | Domain | Severity | Description | File:Line | Recommended Action |

Summary:
- Total Findings: X
- STOP: X (must fix)
- CAUTION: X (fix or override)
- FLAG: X (recommended)

Overall Quality Gate: PASS / NEEDS_FIX / BLOCKED
```

#### D. Intensity Matrix

| Task Complexity | Audit Mode | Domains Checked | Detail Level |
|:---------------:|:----------:|:---------------:|:------------:|
| Simple (1-3 files, documentation, config) | Light | Code Quality + File Structure | Quick scan |
| Standard (4-10 files, feature implementation) | Standard | All 7 domains | Full scan |
| Critical (auth, payments, multi-file, architectural) | Full | All 7 domains + deep analysis | Exhaustive |

#### E. Referral Protocol

| Finding Type | Refer To | Action |
|:-------------|:---------|:-------|
| Deep security vulnerability (auth logic, crypto design) | SecurityAgent | Flag + referral note |
| Visual design/UX quality | DesignReviewer | Flag + referral note |
| Functional test failure | QA Agent | Flag + referral note (Auditor doesn't run tests) |
| Plan compliance issue | Monitor | Flag + referral note |
| Architecture decision needed | Majed | Flag + recommendation |

### 2. Tera Agent Integration (`.opencode/agents/tera.md`)

Add to post-task workflow:

```text
### Auditor Quality Gate (after important tasks)

After completing any of the following task types, Tera should consider
invoking Auditor for quality gate review:

- Tasks touching > 3 files
- Tasks involving auth, payments, or security-sensitive code
- Tasks creating new modules or services
- Tasks modifying architecture or shared components
- Tasks the user explicitly requests quality review for

The quality gate review is separate from Tera's Post-Execution Review Gate.
Tera's gate checks operational compliance (scope, secrets, write targets).
Auditor's gate checks quality metrics (complexity, patterns, maintainability).

Decision: invoke Auditor or skip (with reason documented).
```

### 3. Activation Matrix Entry

```text
| مُدقق | `AUDITOR` | `TASK_COMPLETED`: بعد مهمة تنفيذية مهمة (>3 ملفات، auth/payments، module جديد، تغيير معماري) | 6 | إذا كانت المهمة بسيطة (1-3 ملفات، وثائق، config) ويمكن لـ Tera فحصها مباشرة | ملف المهمة + git diff + ملفات مرجعية Auditor |
```

### 4. Dependency Map Update

```text
| **auditor.md** | — (مستقل — يستدعيه Majed) + `tera.md` (quality gate بعد مهام مهمة) | يراجع مخرجات `tera.md`, `engineering-agent.md` — يحيل إلى `security-agent.md` (أمان عميق)، `design-reviewer.md` (تصميم بصري)، `qa-agent.md` (اختبار وظيفي)، `monitor.md` (مطابقة الخطة) | `project-control/*.md`, `tera-system/*.md`, ملفات الكود المُعدَّلة |
```

### 5. Threshold Reference File (New)

Create `tera-system/engineering-governance/QUALITY_GATE_THRESHOLDS.md` containing:
- Consolidated threshold table with research sources
- Project-phase adjustments (new project = strict, legacy = baseline + improve)
- Project-size intensity matrix
- Language-specific adjustments note
- Calibration protocol (quarterly review)

## Why This Is Necessary

1. **System truth**: Auditor currently does not reflect what a quality auditor should do in 2026. The6 roles are process-oriented; quality auditing requires metric-oriented checks.

2. **Research-backed**: All thresholds come from 20 authoritative sources (8 Tier-1). This is not opinion — it's industry consensus.

3. **Clear gap**: No other agent handles code quality metrics, architecture smells, security hygiene patterns, or testing adequacy verification. QA does functional testing. Monitor does plan compliance. There's a hole.

4. **AI code quality**: SonarQube now has a specific "Sonar Way for AI Code" gate. Our system generates code via AI agents — we need quality gates calibrated for that.

5. **Defense in depth**: Auditor overlapping slightly with Post-Execution Gate on security patterns is intentional — multiple checkpoints catch more issues.

6. **Proportional cost**: The intensity matrix (Light/Standard/Full) prevents Auditor from being heavy on simple tasks.

## Rejected Alternatives

| Alternative | Why Rejected |
|:------------|:-------------|
| Expand QA Agent to include quality auditing | QA is functional testing (runs CLI commands). Quality auditing is pattern analysis (reads code). Different capabilities, different permissions. |
| Create separate "QualityAuditorAgent" | Anti-bloat: we already have Auditor. Expanding it is simpler than adding a new agent + activation triggers + dependency mapping. |
| Let Tera self-audit quality in Post-Execution Gate | Tera already handles operational checks (scope, secrets, write targets). Adding quality metrics would make Tera's gate too heavy and violate separation of concerns. |
| Skip quality auditing entirely | Unacceptable: AI-generated code needs quality gates. Industry best practice is mandatory. |
| Implement all thresholds immediately | Too heavy. P1-P6 priority with4 implementation waves: Foundation → Core → Advanced → Tooling. |

## Anti-Bloat Check

| Question | Answer |
|:---------|:-------|
| What problem does this solve? | Auditor lacks code quality, architecture, security hygiene, and testing adequacy review capabilities |
| Why not modify existing engineering governance files? | Those files are process checklists; quality thresholds need a dedicated reference with research sources |
| Why not expand QA agent? | QA = functional testing (bash/CLI), Auditor = quality analysis (read/grep). Different capabilities. |
| Will this reduce or increase complexity? | Net increase in Auditor scope, but bounded by intensity matrix and clear exclusions |
| Negative token impact? | Mitigated: Light mode for simple tasks, Full only for critical. Proportional to task complexity. |
| Smaller alternative? | No — quality auditing is inherently multi-domain. Splitting would create coordination overhead. |
| File size impact? | Auditor: 363 → ~550 lines. Still under 700 threshold. No split needed. |

## Risk

| Risk | Likelihood | Impact | Mitigation |
|:-----|:----------:|:------:|:-----------|
| Auditor becomes too heavy | Medium | Medium | Intensity matrix: Light for simple tasks, Full only for critical |
| Overlap with QA agent | Low | Medium | Clear boundary: Auditor = quality patterns (read), QA = functional testing (run) |
| Overlap with Post-Execution Gate | Medium | Low | Intentional defense-in-depth on security patterns only |
| Too many STOP findings on existing code | High | High | New code only for strict thresholds; legacy gets advisory/FLAG |
| False positives | Medium | Medium | < 15% false positive budget; quarterly calibration |
| File size exceeds 700 | Low | Low | Current: 363 → ~550. Safe margin. Monitor after changes. |

## Rollback Plan

1. Revert `.opencode/agents/auditor.md` to pre-SCP-098 version
2. Remove Auditor row from `AGENT_ACTIVATION_MATRIX.md`
3. Remove Auditor quality gate step from `.opencode/agents/tera.md`
4. Revert `AGENT_DEPENDENCY_MAP.md` to previous version
5. Delete `QUALITY_GATE_THRESHOLDS.md` if created
6. All changes are additive — no existing content is modified, only additions

## Approval Required
Yes — Majed approval required before any file modifications.

## Research Reference
Full research findings archived in:
`project-control/archive/RESEARCH_TO_SYSTEM_CHANGE_REPORT_AUDITOR_TRANSFORMATION.md`

Research agents used:
- `domain-research-agent` — data gathering (20 sources)
- `domain-expert-agent` — analysis and recommendations (8 sections)
