# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-08-086

## Title
Agent File Maintenance — `ui-designer.md` (مصمم) surface critical rules first

## Request Type
Agent file maintenance / content reorder

## Problem
During agent file maintenance scan, `ui-designer.md` (324 lines) had its two HARDEST operational rules buried:
- Research Protocol (mandatory) at §6
- Vitality Self-Check Gate (mandatory before handback) at §7.5

The file opened with playful persona ("PhD honorary") and design philosophy (Rams principles, Refactoring UI tactics) before the critical operational constraints. Violates the "most important first" principle for agent files.

## Evidence
- Full read of `.opencode/agents/ui-designer.md`
- The rules WERE present and marked mandatory (consistent with SCP-079/082), but positioned low in the file.

## Affected Files
- `.opencode/agents/ui-designer.md`

## Proposed Change
Insert a new section **🔴 القواعد الصلبة (الأهم — اقرأني أولاً)** directly after §1 (من أنا), before §2 (فلسفتي). It summarizes:
1. Research Protocol mandatory (with the rejection rule)
2. Vitality Self-Check Gate mandatory (full checklist)

The detailed §6 and §7.5 remain later in the file for full context.

## Why This Is Necessary
- Surfaces the two non-negotiable operational constraints at the top where the agent reads first.
- Reinforces SCP-079/082 intent: research and vitality are not optional.

## Rejected Alternatives
- Delete §6/§7.5 and keep only the summary: loses detailed guidance the agent needs during execution.
- Move the full §6/§7.5 to the top: too long; a concise summary at top + full detail later is better.

## Anti-Bloat Check
- ✅ No new files
- ✅ Net line increase minimal (summary block ~30 lines; acceptable trade for clarity)
- ✅ No duplicated mandatory rules — summary points to detailed sections

## Risk
Low — behavior unchanged; only ordering improved.

## Rollback Plan
- `git checkout HEAD -- .opencode/agents/ui-designer.md`

## Approval Required
Approved by Majed during maintenance session (verbal per-agent approval).
