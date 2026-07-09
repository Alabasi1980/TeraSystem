# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-08-085

## Title
Agent File Maintenance — `tera-client-engagement.md` (TCEA) mislabeled reference

## Request Type
Agent file maintenance / stale reference fix

## Problem
During agent file maintenance scan, `tera-client-engagement.md` (737 lines) had:
- Line 507 listed `project-control/PRICING_SCORECARD_APPLICATION_MAWTHOOQ.md` in the **🟢 Required Now** table (read immediately at start of any Pricing session).
- But this file is **client-specific** (MAWTHOOQ application) and described as "مثال تطبيقي" (practical example).
- Contradiction: a client-specific file should not be "required" for every new client's pricing session.

## Evidence
- Full read of `.opencode/agents/tera-client-engagement.md`
- Confirmed file exists: `project-control/PRICING_SCORECARD_APPLICATION_MAWTHOOQ.md` → True
- Content ordering of file is already excellent (A.0 command center placed first after CONDUCT GATE)

## Affected Files
- `.opencode/agents/tera-client-engagement.md`

## Proposed Change
1. Remove the MAWTHOOQ scorecard line from the **🟢 Required Now** table (line 507).
2. Add it to the **🔵 Reference Only** table with explicit label: "مثال فقط — دراسة حالة لعميل سابق (MAWTHOOQ)، لا يُقرأ لعملاء جدد".

## Why This Is Necessary
- Prevents TCEA from reading a wrong-client scorecard when starting pricing for a new client.
- Resolves the contradiction between "Required Now" category and "example" description.

## Rejected Alternatives
- Delete the reference entirely: rejects the value of having a worked example available for reference.
- Keep in Required Now: violates the "most important first / no client-specific leakage" principle.

## Anti-Bloat Check
- ✅ No new files
- ✅ Net line count unchanged (removed 1, added 1 with clearer label)
- ✅ No duplicated mandatory rules

## Risk
Low — TCEA pricing behavior unchanged; only reference categorization corrected.

## Rollback Plan
- `git checkout HEAD -- .opencode/agents/tera-client-engagement.md`

## Approval Required
Approved by Majed during maintenance session (verbal per-agent approval).
