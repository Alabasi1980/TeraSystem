## فصل حالات TCEA الرسمية عن دورة حياة الفجوات

Title:
Separate TCEA document/decision statuses from gap lifecycle statuses

Request Type:
Owner improvement request based on external audit finding

Problem:
`C.5` in `.opencode/agents/tera-client-engagement.md` defines the canonical statuses for client files and decision logs, while `C.7` uses a different set of statuses for `AGENT_GAPS_LOG.md` lifecycle handling. Although these serve different objects, the current presentation can make weaker models treat them as one shared status system.

Evidence:
- `.opencode/agents/tera-client-engagement.md` `C.5` defines: `Draft`, `Pending Approval`, `Approved`, `Deferred`, `Conditional`, `Rejected`
- `.opencode/agents/tera-client-engagement.md` `C.7.3` uses: `Pending`, `Under Review`, `Approved`, `Rejected`, `Duplicate`, `Deferred`, `Applied`
- External review explicitly identified this as a runtime ambiguity risk.

Affected Files:
- `.opencode/agents/tera-client-engagement.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:
1. Narrow `C.5` so it explicitly governs only **client documents and decision logs**.
2. Rename the status table label in `C.5` to make its scope explicit.
3. Add a short note in `C.5` that gap statuses are governed separately in `C.7.3`.
4. Rename `C.7.3` to make it explicit that those are **gap lifecycle statuses**, not client-document statuses.
5. Leave all actual status values unchanged; clarify scope only.

Why This Is Necessary:
This removes a real interpretation hazard without changing authority, workflow, or policy semantics. It improves runtime clarity for weaker models at minimal scope.

Rejected Alternatives:
- Merge both status systems into one list: rejected because they govern different objects.
- Create a new standalone policy file for statuses: rejected as unnecessary bloat.
- Leave as-is and rely on human interpretation: rejected because the ambiguity is already externally observed.

Anti-Bloat Check:
- What problem does this solve? Runtime ambiguity between client-document statuses and gap lifecycle statuses.
- Why not just edit an existing file? That is exactly the chosen approach.
- Why not use an existing agent? No new agent is needed; this is a narrow clarification inside TCEA.
- Does this reduce complexity? Yes — clearer boundaries with minimal text.
- Token impact? Negligible increase; likely net reduction in confusion.
- Smaller way? Yes — a scoped clarification only, with no policy/file proliferation.

Risk:
Low — wording clarification only; no permissions, gates, or decisions are expanded.

Rollback Plan:
1. Revert `.opencode/agents/tera-client-engagement.md`
2. Revert `project-control/SYSTEM_EVOLUTION_LOG.md`
3. Delete this proposal file if the change is abandoned

Approval Required:
Approved by Majed in-session on 2026-07-06
