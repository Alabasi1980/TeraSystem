# RESEARCH_TO_SYSTEM_CHANGE REPORT — Auditor → Quality Gate Auditor Transformation

## Research Topic
Converting Tera's Auditor agent from a documentation/compliance reviewer into a world-class Quality Gate Auditor with code quality, architecture, security governance, UI/UX, maintainability, and testing adequacy review capabilities.

## Sources Reviewed

| # | Source | Authority Level | Key Contribution |
|:--|:-------|:---------------:|:-----------------|
| 1 | SonarQube "Sonar Way" & "Sonar Way for AI Code" (2026) | Tier 1 | De-facto quality gate standard, concrete thresholds |
| 2 | SEI/CMU — ATAM Method | Tier 1 | Architecture review methodology |
| 3 | Nielsen Norman Group — 10 Usability Heuristics | Tier 1 | UX evaluation framework |
| 4 | OWASP Secure Code Review Cheat Sheet | Tier 1 | Security pattern detection |
| 5 | Google Engineering Practices — Code Review | Tier 1 | Code review "what to look for" |
| 6 | Microsoft — Maintainability Index | Tier 1 | MI formula and thresholds |
| 7 | WCAG 2.2 (W3C/Deque) | Tier 1 | Accessibility standards |
| 8 | ISO/IEC 25010:2023 | Tier 1 | Official software quality model |
| 9 | McCabe (1976) — Cyclomatic Complexity | Tier 1 | Empirically validated complexity thresholds |
| 10 | Hatton (1997) — Optimal Module Size | Tier 1 | Defect density vs. module size research |
| 11 | JetBrains Blog — Top 6 Code Quality Metrics | Tier 2 | Practical metric recommendations |
| 12 | Sourcegraph — Technical Debt Metrics | Tier 2 | 8 key metrics with thresholds |
| 13 | Kiuwan — Code Maintainability | Tier 2 | Practical thresholds |
| 14 | KindaTechnical — Code Quality Standards | Tier 2 | Comprehensive threshold tables |
| 15 | Microsoft Monodex — Code Organization | Tier 1 | File size thresholds |
| 16 | Codacy/Schreyack — Code Organization | Tier 2 | AI-specific file limits |
| 17 | Matklad — Size Matters | Tier 2 | Function/file size philosophy |
| 18 | Neal Ford/Rebecca Parsons — Building Evolutionary Architectures | Tier 1 | Architectural fitness functions |
| 19 | LinearB — DORA Metrics (2025) | Tier 2 | Engineering benchmarks from 3000+ teams |
| 20 | Autonoma — QA Metrics That Predict Quality | Tier 2 | 6 predictive quality metrics |

## Relevant Findings

### A. Code Quality Thresholds (Industry Consensus)

| Metric | Excellent | Acceptable | Warning | Critical | Source |
|:-------|:---------:|:----------:|:-------:|:--------:|:------:|
| Cyclomatic Complexity / function | < 10 | 10–20 | 20–40 | > 40 | McCabe, SonarQube |
| Cognitive Complexity / function | < 8 | 8–15 | 15–25 | > 25 | SonarSource |
| Code Duplication | < 3% | 3–5% | 5–10% | > 10% | SonarQube "Sonar Way" |
| Function Length (lines) | < 40 | 40–60 | 60–100 | > 100 | Kiuwan, Matklad |
| File Length (lines, production) | < 200 | 200–300 | 300–500 | > 500 | Microsoft Monodex |
| Function Parameters | < 5 | 5–7 | 7–10 | > 10 | KindaTechnical |
| Nesting Depth | < 3 | 3–4 | 4–6 | > 6 | Kiuwan |
| Technical Debt Ratio | < 5% | 5–10% | 10–20% | > 20% | SonarQube SQALE |
| Dependency Freshness (versions behind) | 0 | 1 | 2–3 | > 3 | CodeDebtCost |

### B. Architecture Smells (Detection Patterns)

| Smell | Detection Pattern | Source |
|:------|:------------------|:------:|
| God Object/Module | > 500 lines, > 20 methods, low cohesion | SEI/CMU, SQALE |
| Circular Dependencies | Dependency graph cycle detection | Ford/Parsons fitness functions |
| Layering Violations | Import/dependency analysis across layers | SEI/CMU |
| Tight Coupling | CBO > 7 | SQALE |
| Feature Envy | Cross-module reference analysis | Fowler refactoring catalog |
| Shotgun Surgery | Change coupling / code churn correlation | Fowler |

### C. File/Module Size Thresholds (Hatton 1997 + Microsoft)

| Category | Target | Review | Split-or-Flag | Hard Ceiling |
|:---------|:------:|:------:|:-------------:|:------------:|
| Source Code | < 300 lines | 300 | 500 | 1000 |
| Command Handler | 150–350 | 500 | 700 | 2000 |
| Test Files | < 500 lines | 500 | 800 | 1200 |
| Configuration | < 200 lines | 200 | — | — |

### D. Security Patterns (OWASP-Aligned)

| Pattern | Detection | Severity | CWE |
|:--------|:----------|:--------:|:---:|
| Hardcoded credentials | `grep -ri "password.*=" src/` | Critical | CWE-798 |
| SQL string concatenation | `grep -r "SELECT.*+" src/` | Critical | CWE-89 |
| Unsafe deserialization | `eval()`, `pickle.loads`, `yaml.load` | Critical | CWE-502 |
| Missing input validation | No server-side validation | High | CWE-20 |
| Weak cryptography | MD5, SHA1, DES | High | CWE-327 |
| Missing HTTPS | `http://` in production URLs | High | CWE-319 |

### E. Testing Adequacy Indicators

| Indicator | Excellent | Needs Intervention | Source |
|:----------|:---------:|:------------------:|:------:|
| Coverage (new code) | ≥ 80% | < 60% | SonarQube |
| Test-to-Code Ratio | > 0.5 | < 0.2 | Autonoma |
| Flaky Test Ratio | < 1% | > 5% | LinearB/DORA |

### F. Quality Gate Tiering Model

| Signal | Gate Response | When |
|:-------|:-------------|:-----|
| **STOP** | Block acceptance | Critical CVE, hardcoded secret, zero assertions |
| **CAUTION** | Block + require override | High-severity issue, architecture boundary violation |
| **FLAG** | Report only, no block | Medium-severity, coverage delta below threshold |

### G. Conflicting Recommendations Resolved

| Topic | Conflict | Resolution for Tera |
|:------|:---------|:--------------------|
| Code coverage | 80% (Sonar) vs 70% (DeviQA) | 80% for new code, 70% overall minimum |
| Function length | 40 lines (Kiuwan) vs 66 lines (Matklad) | 50 lines warning, 100 lines critical |
| File size | 300/500 (Codacy) vs 200–400 (Hatton) | 300 warning, 500 critical, 1000 hard ceiling |
| Cyclomatic complexity | ≤ 10 (McCabe) vs ≤ 15 (Microsoft) | 10 warning, 20 critical |

## What Applies to Tera

### Directly Adoptable
1. **Code quality thresholds** — All SonarQube/industry thresholds can be checked by an AI auditor reading code
2. **Architecture smell detection** — God objects, circular deps, layering violations are pattern-detectable
3. **File/module size checks** — Trivially measurable
4. **Security hygiene patterns** — OWASP patterns are grep-able
5. **Testing adequacy indicators** — Test existence, ratio, structure checks
6. **Tiered severity model** — STOP/CAUTION/FLAG fits Auditor's reporting style
7. **UI code-level audit** — ARIA attributes, responsive breakpoints, component duplication

### Partially Adoptable
1. **ATAM methodology** — Can detect patterns but cannot run full 3-4 day human evaluation
2. **Nielsen heuristics** — Some require semantic UI understanding; code-level checks more practical
3. **Maintainability Index formula** — Complex; simplified thresholds more practical for AI
4. **Fitness functions** — Concept applicable but thresholds less standardized than code metrics

### Not Adoptable
1. **Penetration testing** — Requires SecurityAgent or human
2. **Visual design review** — Requires DesignReviewer
3. **Runtime security testing** — Requires SecurityAgent
4. **Business logic flaw detection** — Requires human judgment
5. **Perceptual quality measures** (developer satisfaction) — Requires human input

## What Should NOT Be Adopted

| Rejected | Reason |
|:---------|:-------|
| Full ATAM process (3-4 day human evaluation) | Impractical for AI agent, overlaps with architecture decisions |
| Mutation testing execution | Overlaps with QA agent's functional testing |
| CI/CD pipeline quality gates | Infrastructure concern, not auditor scope |
| Code formatting/style enforcement | Linter territory, not quality auditor |
| Performance benchmarking | Requires runtime execution, belongs to PerformanceAgent |
| Visual design aesthetics | DesignReviewer territory |
| Business logic correctness verification | Requires domain understanding beyond code patterns |

## Recommended System Change

### Summary
Transform Auditor into a Quality Gate Auditor with 7 check domains, 5 activation triggers, a tiered STOP/CAUTION/FLAG severity model, and research-backed thresholds. The transformation modifies 4 existing files and adds 1 reference document.

### Specific Changes

#### 1. `.opencode/agents/auditor.md` — Major Expansion
- Add 7 quality check domains with concrete thresholds:
  - **Code Quality**: complexity, duplication, function/file size, parameters, nesting
  - **Architecture Health**: god objects, circular deps, layering, coupling
  - **File Structure**: size thresholds, split criteria, naming
  - **Security Hygiene**: OWASP patterns (secrets, injection, crypto)
  - **Testing Adequacy**: coverage indicators, test-to-code ratio, missing categories
  - **UI Code Quality**: ARIA, responsive, component reuse, state management
  - **Maintainability**: TODO/FIXME tracking, deprecated patterns, churn signals
- Add STOP/CAUTION/FLAG severity model
- Add quality-specific output format (QUAUD report)
- Keep existing documentation/compliance roles intact
- Add referral protocol (what to hand off to SecurityAgent/DesignReviewer/QA)

#### 2. `.opencode/agents/tera.md` — Add Auditor Trigger
- In Post-Execution section: add "Auditor Quality Gate" step after important task completion
- Define when Auditor review is mandatory vs optional

#### 3. `tera-system/AGENT_ACTIVATION_MATRIX.md` — Add Auditor Triggers
- Add Auditor row with 5 triggers (T-AUD-01 through T-AUD-05)
- Define input requirements for each trigger

#### 4. `tera-system/AGENT_DEPENDENCY_MAP.md` — Add Auditor Relationships
- Add Auditor ↔ Tera invocation link
- Add Auditor → SecurityAgent referral
- Add Auditor → DesignReviewer referral

#### 5. `tera-system/engineering-governance/QUALITY_GATE_AUDITOR_THRESHOLDS.md` — New Reference
- Consolidated threshold table with research sources
- Project-phase adjustments (new vs legacy)
- Project-size intensity matrix (Light/Standard/Full)

## Risk of Adoption

| Risk | Likelihood | Impact | Mitigation |
|:-----|:----------:|:------:|:-----------|
| Auditor becomes too heavy (token cost) | Medium | Medium | Intensity matrix: Light for small tasks, Full only for critical |
| Overlap with QA agent | Low | Medium | Clear boundary: Auditor = quality patterns, QA = functional testing |
| Overlap with Tera's Post-Execution Gate | Medium | Low | Intentional defense-in-depth on security patterns only |
| Too many STOP findings on existing code | High | High | New code only for strict thresholds; legacy gets advisory |
| False positives frustrate workflow | Medium | Medium | < 15% false positive budget; calibration protocol |

## Anti-Bloat Check

| Question | Answer |
|:---------|:-------|
| What problem does this solve? | Auditor lacks code quality, architecture, security, and testing adequacy review |
| Why not modify existing files? | The6 engineering governance files are process-oriented; quality thresholds need a dedicated reference |
| Why not expand QA agent? | QA is functional testing (build/run); Auditor is quality patterns (read/analyze) — different capabilities |
| Will this reduce or increase complexity? | Net increase in Auditor scope, but bounded by clear exclusions and intensity matrix |
| Negative token impact? | Mitigated by intensity matrix — Light mode for simple tasks |
| Smaller alternative? | No — quality auditing is inherently multi-domain; splitting into separate agents would create coordination overhead |

## Decisions Required from Majed

1. **Threshold calibration**: Use standard thresholds or calibrate to our typical project types?
2. **Intensity levels**: Light/Standard/Full matrix — any adjustments?
3. **File location**: Where should the threshold reference file live?
4. **Overlap with Post-Execution Gate**: Accept defense-in-depth or eliminate overlap?
5. **Implementation waves**: Follow P1-P6 priority or adjust?

## Approval Required
Yes — this research informs a SYSTEM_CHANGE_PROPOSAL that requires Majed approval before any file modifications.

---

**Research completed:** 2026-07-16
**Research agents:** domain-research-agent (data gathering) + domain-expert-agent (analysis & recommendations)
**Next step:** SYSTEM_CHANGE_PROPOSAL based on these findings
