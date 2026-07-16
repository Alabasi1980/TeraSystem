# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-099

```text
Title:          P5 Calibration — Auditor Rule Classes and Thresholds After Operational Tests
Request Type:   Calibration / Threshold Adjustment
Date:           2026-07-16
Author:         TeraSystemEvolutionAgent (حارس)
Status:         PENDING — Awaits Majed Approval
```

---

## Problem

Two operational tests of the Auditor sub-agent (Test 1: backend SyncEngine, Test 2: UI TableMappings) revealed that the current rule classes and thresholds in `QUALITY_GATE_THRESHOLDS.md` and `auditor.md` lack explicit guidance for three recurring patterns observed in both tests:

1. **Migration rollback bugs** — A correctness bug in `Down()` that calls `DropIndex` on an index never created by `Up()`. Classified as CAUTION but arguably a correctness-level violation that should escalate to a higher rule class.
2. **Large but coherent files** — Files exceeding 500+ lines (1070, 1045) that are structurally coherent single-responsibility units. Current heuristic doesn't distinguish between "large because bloated" and "large because cohesive."
3. **Materially-changed components with pre-existing issues** — Accessibility gaps that existed before the diff but are made worse by the diff (e.g., wizard modal expanded to 5 steps without adding focus trap). Current baseline debt rules don't explicitly address this escalation pattern.

---

## Evidence

### Test 1 Results (TASK-COD-007 — Backend)

```text
Result:        NEEDS_FIX
STOP:          0
CAUTION:       4  — function length, duplicated retry, duplicated persistence, concurrency race
FLAG:          4  — advisory findings
BASELINE_DEBT: 1  — correct usage
```

All severity classifications were appropriate. No calibration issues identified.

### Test 2 Results (TASK-COD-TABLEMAPPINGS-UI)

```text
Result:        NEEDS_FIX
STOP:          0
CAUTION:       5  — F-A1, F-A2, F-A3, F-S3, F-T1
FLAG:          8  — advisory findings
```

One classification requires calibration discussion:

**F-S3 — Migration `Down()` calls `DropIndex("IX_TableMappings_Name")` but `Up()` never creates this index.**

- Auditor classified: CAUTION (rollback doesn't affect forward migration)
- Evidence: `Down()` will throw runtime exception. `Up()` comment explicitly says "index not supported, uniqueness enforced at application layer."
- The bug is **definitively provable from code alone** — no analyzer or external tool needed.
- The affected operation (migration rollback) is a **supported EF Core operation** used in production deployments.
- Impact: Runtime failure during database rollback, requiring manual intervention.

This finding sits at the boundary between "Default heuristic" and "Hard rule." The current rule classes don't have a category for "definitively provable correctness bugs in supported operations."

### File Size Classification (F-D1, F-D2)

- Index.cshtml: 1070 lines — flagged as FLAG (coherent Razor page with embedded CSS/HTML/JS)
- table-mapping-wizard.js: 1045 lines — flagged as FLAG (cohesive single-class wizard controller)

The Auditor correctly applied judgment: large but cohesive files got FLAG instead of CAUTION. But the thresholds in §4 don't explicitly support this distinction — they say "500+ lines = Caution candidate" without a mechanism to downgrade based on cohesion.

### Baseline Debt in Changed Components (F-A3)

- Wizard modal had no focus trap BEFORE the diff
- The diff expanded the modal from ~3 steps to 5 steps
- Auditor classified: CAUTION (baseline gap, but modal materially changed)

The current §8 (Diff-First and Baseline Debt) says "Do not block for unrelated baseline debt unless it is critical and the current change exposes, worsens, or relies on it." The Auditor applied this correctly, but the rule doesn't explicitly say "worsening a baseline gap in a materially changed component may escalate severity."

---

## Affected Files

| File | Change |
|------|--------|
| `tera-system/engineering-governance/QUALITY_GATE_THRESHOLDS.md` | Add §5.1 Migration Integrity Hard Rule, add §4 cohesion note, add §8 escalation note |
| `.opencode/agents/auditor.md` | Add §7.3 Migration Integrity rule reference, add §8 baseline escalation guidance |

---

## Proposed Change

### Change 1 — Migration Integrity Hard Rule

**In `QUALITY_GATE_THRESHOLDS.md` §5 (Hard Rule Examples), add:**

```text
| QG-DB-001   | Migration Down() is not the inverse of Up() (e.g., drops index/constraint/column never created by Up()) | STOP |
| QG-DB-002   | Migration alters data in a way that cannot be reversed (destructive UPDATE/DELETE without backup logic) | STOP / CAUTION |
```

**Rationale:** These are definitively provable from code, affect a supported operation (migration rollback), and can cause runtime failure in production. They meet the "Direct safety/security/governance failures" bar — specifically "governance failures" since migration integrity is a governance concern.

**Impact on F-S3:** With this rule, the same finding would be classified as **STOP** instead of CAUTION. This is the correct escalation: the bug is provable, the operation is supported, and the failure is runtime.

### Change 2 — File Size Cohesion Note

**In `QUALITY_GATE_THRESHOLDS.md` §4 (Default Heuristics), add after the file length row:**

```text
Context override: Files that exceed the Caution threshold but are structurally coherent
(single responsibility, cohesive domain, no unrelated concerns mixed in) may be classified
as FLAG instead of Caution. The heuristic is a signal for review, not an automatic severity.
```

**Rationale:** Both test files exceeded 500+ lines but were correctly classified as FLAG by the Auditor. This makes the implicit judgment explicit and reusable.

**Impact on future audits:** No change to actual thresholds — just clarifies that the Auditor's contextual judgment is sanctioned.

### Change 3 — Baseline Escalation in Materially Changed Components

**In `QUALITY_GATE_THRESHOLDS.md` §8 (Diff-First and Baseline Debt), add:**

```text
Escalation: When a diff materially changes a component that already has a baseline gap
(e.g., accessibility, error handling), and the change worsens the gap's impact or exposure,
the Auditor may escalate the finding from BASELINE_DEBT to CAUTION or higher.

Example: A modal with no focus trap that is expanded from 3 to 5 steps — the baseline gap
now affects more user flows and is harder to escape.
```

**Rationale:** The Auditor already applied this logic correctly in F-A3, but the rule didn't explicitly support it. This makes the pattern documented and consistent.

**Impact on F-A3:** No change to the actual finding — it was already classified as CAUTION. This just formalizes the reasoning.

### Change 4 — Auditor Rule Reference Update

**In `auditor.md` §7.1 (Rule Classes), add to the Hard rules examples:**

```text
| Hard rules | ... migration rollback integrity (Down ≠ inverse of Up), destructive migration without reversal |
```

**In `auditor.md` §10.2 (Code and Structure Heuristics), add note:**

```text
File size exceeding threshold is a finding candidate. Cohesive single-responsibility files
that exceed the Caution threshold may be classified as FLAG at Auditor judgment.
```

**In `auditor.md` §8 (Severity Model), add after the overall result table:**

```text
Baseline escalation: Pre-existing issues in materially changed components may be
escalated from BASELINE_DEBT to CAUTION if the diff worsens their impact or exposure.
```

---

## Why This Is Necessary

1. **F-S3 is a real bug** — Migration rollback will fail at runtime. The current rule classes force a CAUTION classification for a definitively provable correctness violation. A Hard Rule for migration integrity ensures correct severity.

2. **The cohesion judgment needs documentation** — The Auditor made the right call on both large files. Without explicit guidance, a future Auditor might classify all 500+ line files as CAUTION, creating noise.

3. **Baseline escalation is a real pattern** — Expanding a component with known gaps (no focus trap, no error handling) is a common development pattern. The rule should explicitly support escalating severity when the gap is worsened.

---

## Rejected Alternatives

| Alternative | Reason for Rejection |
|---|---|
| Make ALL migration bugs STOP | Too broad — some migration issues (e.g., missing optional index) are genuinely CAUTION. The rule targets specific provable patterns. |
| Raise file size Caution threshold to 700+ | Would mask real bloat. Better to keep 500+ as the review trigger and allow contextual downgrade. |
| Remove BASELINE_DEBT category entirely | BASELINE_DEBT is still correct for truly unrelated old issues. The change only adds an escalation path for worsened gaps. |
| Add a new "Correctness" rule class between Hard and Default | Would add complexity without clear benefit. Migration integrity fits under "Hard rules" as a governance failure. |

---

## Anti-Bloat Check

| Question | Answer |
|---|---|
| What problem does this solve? | Three calibration gaps identified by two operational tests — migration bugs classified too low, large cohesive files classified too high, baseline escalation undocumented |
| Why not just edit an existing file? | Changes are additions/annotations to existing rules, not new files or layers |
| Does this reduce or increase complexity? | Net neutral — 3 small additions to existing files, no new files, no new agents, no new layers |
| Any negative token impact? | Minimal — ~30 lines added across 2 files |
| Is there a smaller way? | No — these are the minimum changes needed to close the observed calibration gaps |

---

## Risk

Low. All changes are annotations to existing rules, not structural changes. The Auditor already applied this logic correctly in both tests — the change simply documents and formalizes what it was already doing.

---

## Rollback Plan

Revert the additions to `QUALITY_GATE_THRESHOLDS.md` and `auditor.md`. No dependencies broken.

---

## Files to Change (Execution Plan)

1. `tera-system/engineering-governance/QUALITY_GATE_THRESHOLDS.md`
   - §4: Add cohesion note after file length row
   - §5: Add QG-DB-001 and QG-DB-002 to Hard Rule Examples
   - §8: Add baseline escalation note

2. `.opencode/agents/auditor.md`
   - §7.1: Add migration integrity to Hard rules examples
   - §8: Add baseline escalation note after overall result table
   - §10.2: Add cohesion note to code heuristics

## Post-Change Validation

- [ ] `git diff --check` — PASS
- [ ] No stale references to old rule classes
- [ ] All three calibration findings (F-S3, F-D1/D2, F-A3) are now explicitly supported by rules
- [ ] Auditor file size still below split threshold (465 lines → ~480 lines estimated)

---

## Approval Required

Yes — Majed approval required before any edits.
