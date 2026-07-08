# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-08-084

## Title
Agent File Maintenance — `tera.md` (TeraAgent) content health & ordering

## Request Type
Agent file maintenance / content ordering / stale reference fix

## Problem
During full system health scan (SYSTEM_HEALTH_REPORT_2026-07-08), `tera.md` had:
1. Stale reference to `APPLICATION_PROPOSAL.html` from `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html` (line 178) — despite C-2 fix in TeraPolicyMap.md, this agent file was not updated.
2. The most critical rule (Code Boundary / no code writing) was buried at §9.1 (line 262) instead of near the top.
3. The "TeraAgent does NOT write code" rule was repeated 4 times (lines 19, 244, 262, 408) — redundancy.
4. Authority Order #5 was redundant with #3.

## Evidence
- Full read of `.opencode/agents/tera.md` (703 lines before fix)
- Confirmed via grep: `APPLICATION_PROPOSAL.html` no longer exists; correct path is `tera-workshop/client-templates/commercial/APPLICATION_PROPOSAL_TEMPLATE.md`

## Affected Files
- `.opencode/agents/tera.md`

## Proposed Change
1. Line 178: fix stale reference → `.md` path
2. Move full CODE BOUNDARY rule from §9.1 to directly after CONDUCT GATE (top of file)
3. Reduce §9.1 to a pointer; reduce §12 Code Writing Delegation Rule to a short summary
4. Remove redundant Authority Order #5; renumber #6–#8 to #5–#7

## Why This Is Necessary
- Agent file must surface its most important constraint first (content ordering principle from Majed).
- Stale reference causes wrong template path lookup.
- Redundancy wastes tokens and risks drift between duplicate statements.

## Rejected Alternatives
- Keep §9.1 as-is: rejects the "most important first" principle.
- Delete all code-boundary mentions except one: too risky — the rule is the agent's hardest boundary and benefits from reinforcement at top + operational context in §12.

## Anti-Bloat Check
- ✅ No new files created
- ✅ Net line reduction (703 → 492)
- ✅ No duplicated mandatory rules remaining

## Risk
Low — TeraAgent behavior unchanged; only ordering and reference accuracy improved.

## Rollback Plan
- `git checkout HEAD -- .opencode/agents/tera.md` (or restore from git history)

## Approval Required
Approved by Majed during maintenance session (verbal per-agent approval).
