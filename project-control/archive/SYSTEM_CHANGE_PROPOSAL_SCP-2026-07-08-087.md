# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-08-087

## Title
Agent File Maintenance — `engineering-agent.md` content order fix (identity first)

## Request Type
Agent file maintenance / content reorder

## Problem
The file opened with credentials (شهادتي: دكتوراه) and quotes BEFORE the identity section (§1 من أنا). Violates "most important for the agent first" principle.

## Evidence
- Full read of `.opencode/agents/engineering-agent.md`
- Lines 25-35 (شهادتي + quotes) preceded lines 38-47 (§1 من أنا)

## Affected Files
- `.opencode/agents/engineering-agent.md`

## Proposed Change
Swap order: §1 (من أنا) comes immediately after CONDUCT GATE, then شهادتي + quotes, then §2 (مبادئي الهندسية).

## Why This Is Necessary
- Identity (من أنا) is more important for the agent than credentials.
- Consistent with the content-ordering principle applied to `ui-designer.md`.

## Rejected Alternatives
- Leave as-is: minor but inconsistent pattern.

## Anti-Bloat Check
- ✅ No new files
- ✅ Net line count unchanged

## Risk
Low — content unchanged, only order.

## Rollback Plan
- `git checkout HEAD -- .opencode/agents/engineering-agent.md`

## Approval Required
Approved by Majed during maintenance session.
