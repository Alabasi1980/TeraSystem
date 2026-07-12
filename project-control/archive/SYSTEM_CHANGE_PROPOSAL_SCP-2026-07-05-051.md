# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-05-051

## Title
Phase 5 — Merge TeraClientEngagement.md into tera-client-engagement.md (Dual-to-Single File)

## Request Type
Anti-Bloat / Architecture Simplification

## Problem
TCEA (وسيط) has dual files like the other agents did before merging:
- **Source of truth:** `tera-system/TeraClientEngagement.md` (974 lines, 13 sections)
- **Execution:** `.opencode/agents/tera-client-engagement.md` (452 lines, 10 sections)

This causes:
1. Maintenance burden — every change must be made in two files
2. Risk of drift — the files can fall out of sync
3. Confusion about which file is authoritative
4. Token waste — both files must be loaded
5. Inconsistency with the 4 already-merged agents (Monitor, Auditor, DesignReviewer, ApplicationBlueprint)

## Evidence
- 10 cross-references from `tera-client-engagement.md` point back to `TeraClientEngagement.md` as external source-of-truth
- `TeraPolicyMap.md` line 32 lists `tera-system/TeraClientEngagement.md` as source of truth
- `tera.md` line 52 lists `TeraClientEngagement.md` in the system reference list
- The execution file already contains operational summaries of most sections

## Content Analysis

| TeraClientEngagement.md Section | Status in tera-client-engagement.md |
|---|---|
| §1 Identity | ✅ Already covered (exec §1) |
| §2 Location in system | ✅ Already covered (exec §1) |
| §3 Responsibilities (9 groups) | ✅ Covered in exec §2 (role table) |
| §3.2.1 Understanding Confirmation Gate | ✅ Covered in exec §3 workflow |
| §3.2.2-3.2.5 Framework/Matrix/Gates/Depth | ✅ Referenced in exec §3 |
| §3.2.6 Self-Check Protocol | ✅ Fully covered (exec §5.1) |
| §3.2.7 Uncertainty Protocol | ✅ Fully covered (exec §5.2) |
| §3.2.8 Consultation Response Protocol | ✅ Fully covered (exec §5.3) |
| §3.3.1-3.3.3 Scope/Budget/Decision Gates | 🔶 Referenced in exec §3, details to add |
| §3.4-3.5 Documents/Change Request | ✅ Covered in exec §7 |
| §3.6 Workspace & Handoff | ✅ Covered in exec §3 |
| §3.6.1 Handoff Readiness Gate | 🔶 Referenced, details to add |
| §3.7-3.8 Delivery/Maintenance | ✅ Referenced in exec §3 |
| §3.9 Commercial Estimation | ✅ Fully covered (exec §10 — more detailed) |
| §3.9.1-3.9.2 Quotation Readiness Gate | 🔶 Referenced in exec §3 |
| §4 Limits | ✅ Fully covered (exec §6) |
| §5 Relationship with TeraAgent | ✅ Covered in exec §1 |
| §6 Handoff Package | 🔶 Partially covered (exec §7 lists files) |
| §7 Document rules | 🔶 Partially covered |
| §8 Websearch Protocol | ✅ Fully covered (exec §4) |
| §9 Change Request Management | 🔶 Referenced |
| §10 Mandatory Files | ✅ Covered (exec §7) |
| §11 Pricing System | ✅ Fully covered (exec §10 — more detailed!) |
| **§12 Client Document Library** | ❌ **MISSING — full activation matrix** |
| **§13 Self-Improvement & Gap Reporting** | ❌ **MISSING — governance protocol** |

## Affected Files

| File | Change |
|---|---|
| `.opencode/agents/tera-client-engagement.md` | UPDATE — add §12 Client Document Library + §13 Self-Improvement; internalize all external refs to `TeraClientEngagement.md` |
| `tera-system/TeraClientEngagement.md` | DELETE — content merged into execution file |
| `tera-system/TeraPolicyMap.md` | UPDATE — change source-of-truth from `tera-system/TeraClientEngagement.md` to `.opencode/agents/tera-client-engagement.md` |
| `.opencode/agents/tera.md` | UPDATE — remove `TeraClientEngagement.md` from system reference list |
| Other files with cross-references | UPDATE — TBD after full grep |

## Proposed Change

### Step 1: Add §12 Client Document Library to tera-client-engagement.md
Copy the full Activation Matrix (templates, triggers, responsibility, approval) from TeraClientEngagement.md §12.

### Step 2: Add §13 Self-Improvement & Gap Reporting to tera-client-engagement.md
Copy the gap reporting protocol from TeraClientEngagement.md §13.

### Step 3: Internalize all external references
Change all `tera-system/TeraClientEngagement.md §X.Y.Z` references in `tera-client-engagement.md` to internal section references (e.g., `§X.Y.Z`).

### Step 4: Remove `System Reference: tera-system/TeraClientEngagement.md` from frontmatter

### Step 5: Delete `tera-system/TeraClientEngagement.md`

### Step 6: Update all cross-references in other files

## Why This Is Necessary
- Completes the dual-to-single file migration pattern established in Phases 1-4
- Eliminates maintenance burden of keeping 974-line + 452-line files in sync
- Removes ambiguity about which file is the source of truth
- Saves token consumption (one file instead of two)
- Consistent with the architectural decision to have each agent's execution file be its own source of truth

## Rejected Alternatives
1. **Keep dual files** — Rejected: inconsistent with the 4 already-merged agents, continues maintenance burden
2. **Reverse merge (exec into truth)** — Rejected: execution file has operational context (frontmatter, protocols) that truth file lacks; previous phases established execution-file-as-base pattern
3. **Partial merge** — Rejected: leaves ambiguity and maintenance burden; full merge is cleaner

## Anti-Bloat Check

| Question | Answer |
|---|---|
| What problem does this solve? | Dual-file maintenance burden, token waste, sync risk, inconsistency with other agents |
| Why not just update one file? | The two files have different but overlapping content; having both is the problem |
| Why not use an existing agent? | This IS about the agent's own file — merging its dual definitions |
| Does this reduce complexity? | ✅ Yes — 1 file instead of 2, 0 sync issues |
| Negative token impact? | ✅ Positive — eliminates loading 974-line file separately |
| Smaller way to achieve same goal? | No — full merge is the minimal complete solution |

## Risk
- **Medium** — TCEA is a large agent with extensive cross-references. Must ensure no broken references.
- Mitigation: Full grep before and after, update all cross-references, validate with git diff --check.

## Rollback Plan
1. `git checkout -- .opencode/agents/tera-client-engagement.md`
2. `git checkout -- tera-system/TeraPolicyMap.md`
3. `git checkout -- .opencode/agents/tera.md`
4. `git checkout HEAD -- tera-system/TeraClientEngagement.md` (restore deleted file)
5. Revert any other updated cross-reference files

## Approval Required
✅ Majed — Please approve or provide feedback.
