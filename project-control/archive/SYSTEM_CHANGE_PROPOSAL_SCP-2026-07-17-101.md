# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-17-101

```text
Title:          P3 Auditor Architecture and File-Structure Audit Integration
Request Type:   Owner Improvement Request / Auditor Capability Upgrade
Date:           2026-07-17
Author:         TeraSystemEvolutionAgent (حارس)
Status:         PENDING — Awaits Majed Approval
```

---

## Problem

Majed asked whether Auditor currently audits application structure, folder/file organization, file bloat, helper extraction, and separation of responsibilities according to professional/global engineering standards.

Current state: Auditor partially covers this through generic P2 heuristics:

- oversized files/functions;
- obvious mixed responsibilities;
- suspected duplicate blocks;
- circular dependency evidence;
- code-level UI accessibility.

But Auditor does not yet have an explicit P3 architecture/file-structure audit layer that guides it to evaluate:

1. module/folder boundaries;
2. UI/business/domain/data separation;
3. helper/service extraction when files accumulate unrelated code;
4. shared/utils dumping-ground misuse;
5. feature/module structure drift;
6. architecture-sensitive UI implementation patterns;
7. proportional architecture by project size: Compact / Standard / Full.

This creates a quality gap: Auditor can see symptoms of bloat, but does not yet consistently assess whether the application structure itself is degrading.

---

## Evidence

### User Question

Majed asked:

```text
هل المدقق Auditor يدقق على هيكلية التطبيق وبناء المجلدات والملفات بشكل معيار عالمي منظم
وان الملفات لا يجب ان تتضخم وتصبح متراكمة
ومثلا يجب فصل الاكواد المساعدة في ملفات منفصلة وغير ذلك ؟
```

### Current Auditor Evidence

`.opencode/agents/auditor.md` currently states:

- §3: Auditor checks "Core architecture and file-structure health signals".
- §10.2: checks large files/functions, deep nesting, obvious mixed responsibilities, duplicate blocks.
- §10.4: checks circular imports only when evidence exists.
- §11: code-level accessibility only, not visual design.

This is useful, but not sufficient for systematic architecture/file-structure review.

### Existing Governance Evidence

The needed standards already exist in current files:

- `ENGINEERING_BEST_PRACTICES.md` §4 — Structure by Responsibility.
- `ENGINEERING_BEST_PRACTICES.md` §5 — File Size and Responsibility.
- `ENGINEERING_BEST_PRACTICES.md` §6 — Single Responsibility.
- `ENGINEERING_BEST_PRACTICES.md` §7 — UI and Business Logic Separation.
- `ENGINEERING_BEST_PRACTICES.md` §8 — DRY With Judgment.
- `ENGINEERING_REVIEW_CHECKLIST.md` §2 — Architecture and Structure.
- `ENGINEERING_REVIEW_CHECKLIST.md` §3 — File and Responsibility Health.
- `ENGINEERING_REVIEW_CHECKLIST.md` §4 — UI / Application / Domain Separation.
- `ENGINEERING_REVIEW_CHECKLIST.md` §5 — Shared Code and DRY.
- `ENGINEERING_AGENT_RESPONSIBILITIES.md` §5.2 — says Auditor must review module/file structure, oversized files, UI/business separation, shared/utils misuse.

Therefore, the gap is not missing principles. The gap is that Auditor's active contract does not yet operationalize them as an explicit P3 audit section with rule IDs, evidence rules, and severity guidance.

---

## Affected Files

| File | Change Type | Purpose |
|---|---:|---|
| `.opencode/agents/auditor.md` | Update | Add P3 Architecture and File-Structure Audit section and audit mode guidance |
| `tera-system/engineering-governance/QUALITY_GATE_THRESHOLDS.md` | Update | Add architecture/file-structure rule examples and severity guidance |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | Update after execution | Record approved system change |

No new files, agents, folders, MCPs, or layers are proposed.

---

## Proposed Change

### Change 1 — Add P3 section to Auditor

Add a new section after P2 checks in `.opencode/agents/auditor.md`:

```text
## 11. P3 Architecture and File-Structure Audit

Apply for Standard and Full Risk-Based audits when changed files touch application code, UI pages/components, services, data access, APIs, or module structure.

Review only changed files and directly affected neighboring units unless the orchestrator explicitly allows expanded architecture scope.

Check:
1. Module/folder boundary health.
2. UI/business/data separation.
3. File responsibility and bloat risk.
4. Helper/service extraction need.
5. Shared/utils dumping-ground misuse.
6. Cross-module dependency direction.
7. Architecture-sensitive UI implementation patterns.

Severity guidance:
- STOP: new architecture-forbidden dependency, cross-layer write outside approved path, or security-sensitive logic hidden in UI only.
- CAUTION: changed file combines unrelated responsibilities, business rules buried in UI/server page, helper extraction clearly needed, shared/utils misuse introduced, or module boundary violated.
- FLAG: large but cohesive files, minor naming/placement smell, helper extraction candidate without immediate risk.

Evidence rule:
Do not claim full architecture compliance without project-level architecture artifacts. Cite concrete file paths, changed functions/classes, folder placement, imports/dependencies, or visible responsibility mixing.
```

The existing UI accessibility section would be renumbered after this insertion.

### Change 2 — Add architecture rule examples to QUALITY_GATE_THRESHOLDS

Add a compact section under rule examples:

```text
## Architecture and File-Structure Rule Examples

| Rule ID | Condition | Typical Severity |
|---|---|---|
| QG-ARCH-002 | Changed code places business rules in UI/page/component when service/domain layer exists or is required by project size | CAUTION / STOP if security-sensitive |
| QG-ARCH-003 | New or changed file mixes unrelated responsibilities such as UI rendering + validation + DB access + permissions + notifications | CAUTION |
| QG-ARCH-004 | Module-specific logic moved into shared/utils without reuse across modules | CAUTION / FLAG |
| QG-ARCH-005 | Helper/service extraction clearly needed because repeated or supporting code obscures the primary file responsibility | CAUTION / FLAG |
| QG-ARCH-006 | Cross-module or cross-layer dependency direction is newly violated with clear import/reference evidence | CAUTION / STOP if architecture-critical |
| QG-ARCH-007 | New generic dumping-ground file/folder (`helper`, `common`, `utils`, `manager`) introduced without narrow responsibility | CAUTION / FLAG |
```

### Change 3 — Audit mode calibration

Update Auditor audit modes:

- Small code change: P2 plus P3 only when structural risk appears.
- UI change: Standard mode includes UI accessibility + architecture-sensitive UI implementation.
- Architecture/security-sensitive: Full Risk-Based includes P3 by default.

### Change 4 — Maintain boundaries

Clarify that Auditor does not become SolutionArchitectureAgent:

- Auditor detects structural risks in changed work.
- Auditor does not redesign the application.
- Auditor does not force enterprise architecture into compact projects.
- Refactoring recommendations must be proportional and actionable.

---

## Why This Is Necessary

1. **It directly answers Majed's requirement.** Auditor should detect when code accumulates inside oversized files, when helper code should be extracted, and when folder/module structure drifts.

2. **It activates existing standards.** The principles already exist in `ENGINEERING_BEST_PRACTICES.md` and `ENGINEERING_REVIEW_CHECKLIST.md`; this change makes them operational inside Auditor.

3. **It prevents bloat early.** File bloat and structure drift are easier to fix when caught during task review, not after many tasks.

4. **It stays proportional.** The proposed rules explicitly prevent over-engineering and require project-size context.

---

## Rejected Alternatives

| Alternative | Reason Rejected |
|---|---|
| Create a new ArchitectureAuditor agent | Bloat. Auditor already has quality-gate role and existing engineering-scope boundary. |
| Add a new architecture policy file | Not needed. Existing `ENGINEERING_BEST_PRACTICES.md` and checklist already contain the principles. |
| Make all large files blocking | Too rigid. P5 already calibrated large cohesive files as FLAG. |
| Require full codebase architecture scan every audit | Too expensive and violates diff-first scope. P3 should remain changed-code-first. |
| Force Clean Architecture everywhere | Over-engineering. Compact projects need proportional structure. |

---

## Anti-Bloat Check

| Question | Answer |
|---|---|
| What problem does this solve? | Auditor lacks explicit operational architecture/file-structure review despite existing governance standards. |
| Why not enough to edit an existing file? | We are editing existing files only; no new file is proposed. |
| Why not create a new agent? | Existing Auditor is the right quality-gate owner; a new agent would duplicate responsibility. |
| Does this reduce or increase complexity? | Slightly increases Auditor guidance but reduces application bloat risk and avoids creating a new layer. |
| Token impact? | Low. Estimated +50–80 lines in `auditor.md`, +15–25 lines in thresholds. Auditor remains below split threshold. |
| Smaller way? | Yes: only add compact P3 rules to Auditor and thresholds, no new policy. This proposal uses that smaller path. |

---

## Risk

Low to medium.

Primary risk: Auditor may over-report architecture smells if rules are too broad.

Mitigation:

- Keep diff-first scope.
- Require concrete evidence.
- Make project-size proportionality explicit.
- Treat extraction and structure findings mostly as CAUTION/FLAG unless hard-rule evidence exists.
- Do not allow Auditor to redesign or expand task scope.

---

## Rollback Plan

Revert the added P3 section in `.opencode/agents/auditor.md` and the architecture rule examples in `QUALITY_GATE_THRESHOLDS.md`. No file/folder structure changes are involved.

---

## Validation Plan After Approval

- [ ] Anti-Bloat Gate — PASS.
- [ ] Consistency with `ENGINEERING_BEST_PRACTICES.md` — Verified.
- [ ] Consistency with `ENGINEERING_REVIEW_CHECKLIST.md` — Verified.
- [ ] No broken references in Auditor or governance files — Grep check PASS.
- [ ] Auditor file size remains below split threshold — expected < 700 lines.
- [ ] `git diff --check` — PASS or report unrelated pre-existing whitespace separately.
- [ ] `project-control/SYSTEM_EVOLUTION_LOG.md` — Updated.

---

## Approval Required

Yes — Majed approval required before any edits.
