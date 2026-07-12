# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-05-052

## Title
Phase 6 — Merge TeraAgent.md into tera.md (Dual-to-Single File — Final Phase)

## Request Type
Anti-Bloat / Architecture Simplification

## Problem
TeraAgent (مرشد) is the last remaining dual-file agent. It has:
- **Source of truth:** `tera-system/TeraAgent.md` (1,401 lines, 39 sections, ~60KB)
- **Execution:** `.opencode/agents/tera.md` (558 lines, 20 sections, ~27KB)

This causes:
1. Maintenance burden — every change to Tera's identity/rules requires two-file sync
2. Drift risk — the files evolved separately and have different structures
3. Token waste — both files must be loaded (~87KB total)
4. Authority ambiguity — tera.md has higher runtime priority but TeraAgent.md is the source
5. Inconsistency with all 5 already-merged agents

## Content Analysis

| TeraAgent.md § | Title | In tera.md? | Action |
|---|---|---|---|
| §1 | Identity | ✅ §1 + frontmatter | Keep compact |
| §2 | System References | ✅ §2 | Keep compact |
| §2.1 | Session Startup | ✅ §4 | Keep compact |
| §2.2 | Tech Profile Policy | ✅ §5 | Keep compact |
| §2.3 | Project Intake Gate | ✅ §6 | Keep compact |
| §3 | Scope | ✅ §3 + §9 | Keep compact |
| §4 | Inputs | ✅ §7 | Keep compact |
| §5 | 7-Phase Sequence | ✅ §11 | Keep compact |
| §6 | First Mandatory Output | 🔶 Partial | Keep compact |
| §7 | Client Record vs Actual Client | 🔶 Partial in §8 | Keep compact |
| §8-16 | Sub-Agent Lifecycle | ✅ §8 | Keep compact |
| §17 | OpenCode Agent Update Instr. | ❌ Not needed in runtime | Skip — runtime file IS the agent |
| §18 | Target Workspace Env. | 🔶 §8 covers | Keep compact |
| §19 | No Inventing Outside Registry | ✅ §8 + §9 | Keep compact |
| §20 | Tools & Sources | ✅ §9 | Keep compact |
| §21 | Agent Count Policy | 🔶 Partially in §10 | Keep compact |
| §22 | Anti-Bloat Policy | ✅ §10 | Keep compact |
| §23 | Anti-Conflict Policy | 🔶 Partially | Keep compact |
| §24-27 | Governance Protocols | ✅ §12-14 | Keep compact |
| §28-30 | Agent Manifest/Protocols | 🔶 Referenced | Keep compact |
| §31 | **Final Rule** | ❌ **Missing** | **ADD — short philosophical closure** |
| §32-35 | Token/State/Delegation | ✅ §15 | Keep compact |
| §36 | Cost Approval | ✅ §15 | Keep compact |
| §37 | Sub-Agent Governance | ✅ §12-14 | Keep compact |
| §38 | Operating Model | ✅ §7 | Keep compact |
| §39 | **Continuous Improvement** | ❌ **Missing** | **ADD — governance standard** |

## Affected Files

| File | Change |
|---|---|
| `.opencode/agents/tera.md` | UPDATE — add §19 (Final Rule) + §20 (Continuous Improvement); update System Reference; internalize refs |
| `tera-system/TeraAgent.md` | DELETE — content merged into execution file |
| `tera-system/TeraPolicyMap.md` | UPDATE — change 4 source-of-truth refs from `tera-system/TeraAgent.md` to `.opencode/agents/tera.md` |
| `.opencode/agents/tera.md` (line 52) | UPDATE — remove `TeraAgent.md` from system reference list |
| `tera-system/TeraSubAgents.md` | UPDATE — change reference |
| `tera-system/TeraArchitectureMap.md` | UPDATE — change reference |
| `tera-system/TeraPreExecutionGate.md` | UPDATE — change reference |
| `tera-system/TeraPreparationDocumentationGovernance.md` | UPDATE — change reference |
| `tera-system/TERA_USER_GUIDE.md` | UPDATE — change reference |
| `tera-system/Tera_Project_Preparation_Files.md` | UPDATE — change reference |
| Other active files with refs | UPDATE — TBD after full grep |

## Proposed Change

### Step 1: Update tera.md frontmatter
- Remove `System Reference: tera-system/TeraAgent.md (v1.0)`
- Add `Source of Truth: This file` note

### Step 2: Add §19 — Final Rule (from TeraAgent.md §31)
Short philosophical closure summarizing Tera's mission.

### Step 3: Add §20 — Continuous Improvement & Gap Reporting (from TeraAgent.md §39)
Critical governance section covering gap reporting obligation.

### Step 4: Renumber existing §20 to §21
Currently tera.md has §20 (Plan Mode and Build Mode). After adding §19 and §20, it becomes §21.

### Step 5: Internalize remaining references
Update any `tera-system/TeraAgent.md` references within tera.md to point internally.

### Step 6: Delete `tera-system/TeraAgent.md`

### Step 7: Update all cross-references in other files

## Why This Is Necessary
- Final phase of the dual-to-single file migration (5 agents already merged)
- TeraAgent is the largest and most-referenced file; completing this eliminates the biggest maintenance burden
- All agent files will follow the same pattern: single file in `.opencode/agents/` is both source of truth and runtime
- Eliminates ~60KB of redundant file loading

## Rejected Alternatives
1. **Keep dual files** — Inconsistent with 5 already-merged agents; highest maintenance burden
2. **Full content copy (add all 39 sections)** — Would bloat tera.md from 558 to 1300+ lines, defeating the purpose of a compact runtime agent. Selective addition is better.
3. **Reverse merge (tera.md into TeraAgent.md)** — Loses OpenCode frontmatter and compact runtime structure

## Anti-Bloat Check

| Question | Answer |
|---|---|
| What problem does this solve? | Dual-file maintenance, token waste, inconsistency with other agents |
| Why not just update one file? | Both exist and need to become one |
| Why not use an existing agent? | This IS the agent file |
| Does this reduce complexity? | ✅ Yes — 1 file instead of 2 |
| Negative token impact? | ✅ Positive — eliminates loading separate 60KB file |
| Smaller way to achieve same goal? | Selective add (only 2 critical sections) is the minimal approach |

## Risk
- **High** — TeraAgent.md is referenced from ~15+ files across the system. Must update all cross-references.
- Mitigation: Full grep before and after, systematic file-by-file update, git diff --check validation.

## Rollback Plan
1. `git checkout -- .opencode/agents/tera.md`
2. `git checkout -- tera-system/TeraPolicyMap.md`
3. `git checkout -- tera-system/TeraSubAgents.md`
4. `git checkout -- tera-system/TeraArchitectureMap.md`
5. `git checkout -- tera-system/TeraPreExecutionGate.md`
6. `git checkout -- tera-system/TeraPreparationDocumentationGovernance.md`
7. `git checkout -- tera-system/TERA_USER_GUIDE.md`
8. `git checkout -- tera-system/Tera_Project_Preparation_Files.md`
9. `git checkout HEAD -- tera-system/TeraAgent.md` (restore deleted file)
10. Revert any other updated cross-reference files

## Approval Required
✅ Majed — Please approve or provide feedback.
