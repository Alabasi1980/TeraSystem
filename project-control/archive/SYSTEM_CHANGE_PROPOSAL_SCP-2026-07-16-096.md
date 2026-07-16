# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-096

Title: Add Sub-Agent Delegation Size Guardrail to TeraAgent Runtime

Request Type: Owner improvement request / Agent runtime governance

Problem:
TeraAgent may delegate an overly large or broad task to a sub-agent in one handoff. Large sub-agent tasks can produce many changes at once, making review, audit, rollback, and error detection difficult. If the sub-agent makes a wrong assumption or acts outside the intended direction, the damage may not be noticed until after a large body of work is complete.

Evidence:
- Majed requested that TeraAgent always understand, before distributing work, that sub-agents should receive small or medium tasks rather than huge tasks.
- `.opencode/agents/tera.md` already contains `TASK-ID Size Control Rule` and Phase 6 rules, but it does not explicitly prohibit oversized sub-agent delegation packages.
- Current runtime already states: "Tera breaks approved plan into small tasks" and "execute one approved TASK-COD-* or a small approved batch only", but this should be sharpened for sub-agent task handoffs.

Affected Files:
- `.opencode/agents/tera.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md` after approved implementation

Proposed Change:
1. Update `.opencode/agents/tera.md` `Last Synced` line to reference this SCP after implementation.
2. Add a compact subsection under `## 12. Execution Orchestration Core`, immediately after `### TASK-ID Size Control Rule`, titled:
   ```text
   ### Sub-Agent Delegation Size Rule — قاعدة حجم تفويض العملاء الفرعيين
   ```
3. Add the following operational meaning:
   - Tera must not give any sub-agent a huge, multi-stage, or broad implementation task in one delegation.
   - Each sub-agent task must be small or medium, reviewable, and bounded by one clear objective.
   - A delegation must not combine independent modules/screens/APIs/DB changes/UI work/security work unless explicitly justified and still reviewable.
   - Tera must receive and review the handback before assigning the next task.
   - If the work is large, Tera must split it into sequential TASK-IDs or a small approved batch.
   - Goal: early error detection, auditable diffs, controlled rollback, and easier Post-Execution Review.

Suggested Runtime Text:
```markdown
### Sub-Agent Delegation Size Rule — قاعدة حجم تفويض العملاء الفرعيين

Tera must not give a sub-agent a huge, multi-stage, or broad implementation task in one delegation.

Each sub-agent delegation must be:
- small or medium in size,
- tied to one clear `TASK-ID`,
- bounded by one main objective,
- limited to explicit Allowed Sources and Allowed Write Targets,
- reviewable through a manageable diff/handback before the next delegation.

If the requested work contains multiple independent modules, screens, APIs, database changes, UI work, security work, or several execution phases, Tera must split it into sequential `TASK-ID`s or a small approved batch.

Rule:
```text
Delegate → receive handback → review actual output → accept/fix/block/defer → only then delegate the next unit.
```

Reason: oversized delegation hides errors, makes review difficult, increases rollback risk, and lets wrong assumptions accumulate before Tera can detect them.
```

Why This Is Necessary:
- Directly reduces execution risk from unreviewable large sub-agent outputs.
- Strengthens TeraAgent's existing orchestrator role without adding a new agent, file, or workflow layer.
- Aligns with the current task-size rule and Post-Execution Review Gate.

Rejected Alternatives:
- Create a new policy file: rejected as unnecessary bloat for a compact runtime guardrail.
- Modify every sub-agent file: rejected because the orchestration responsibility belongs to TeraAgent, not each sub-agent.
- Only rely on existing `TASK-ID Size Control Rule`: rejected because it controls task records generally but does not explicitly constrain sub-agent delegation package size.

Anti-Bloat Check:
- What problem does it solve? Prevents oversized sub-agent delegations that are hard to audit and unsafe to review.
- Why not a new file? Existing `.opencode/agents/tera.md` §12 is the correct active runtime location.
- Why not a new agent? No new capability is needed; this is orchestration discipline.
- Complexity impact? Reduces operational complexity by forcing smaller reviewable units.
- Token impact? Minimal; adds a compact subsection only.
- Smaller path? Yes: one runtime subsection, no new system layer.

Risk:
- Low. The rule may make execution slightly slower due to more handback/review cycles, but improves traceability and safety.

Rollback Plan:
- Remove the added subsection from `.opencode/agents/tera.md`.
- Restore the previous `Last Synced` line.
- Add rollback note to `SYSTEM_EVOLUTION_LOG.md` if rollback is executed.

Approval Required:
Yes — Majed approval required before editing `.opencode/agents/tera.md`.
