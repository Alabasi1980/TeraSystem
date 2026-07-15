# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-025

Title:
Phased Tera Agent Governance Cleanup

Request Type:
Owner Improvement Request / Anti-Bloat Review / Policy Consistency Cleanup

Problem:
`tera-system/TeraAgent.md` and `.opencode/agents/tera.md` have accumulated overlapping operational rules, repeated gate summaries, and mixed identity/runtime responsibilities.

This creates four governance risks:

1. Sensitive rules exist in more than one place, especially around Pre-Execution Gate, Design Governance, and Plan Mode / Build Mode.
2. `TeraAgent.md` contains more runtime-operational detail than is appropriate for a source-of-truth identity file.
3. `.opencode/agents/tera.md` is longer than ideal for an active runtime file and carries some content that should be summarized rather than repeated.
4. Future maintenance risk is high because updating one rule may leave stale or conflicting copies elsewhere.

Evidence:
- `TeraPolicyMap.md` says policies define rules and runtime files summarize operational triggers only.
- `TeraSystemMaintenanceChecklist.md` explicitly says:
  - update the source of truth first
  - keep runtime summaries compact
  - do not copy full policy sections into runtime
  - do not define the same mandatory gate in two sources
- `TeraArchitectureMap.md` says `.opencode/agents/tera.md` should answer what Tera must load now, what is forbidden now, and which policy should be read for details; it should not duplicate full policies.
- The current Tera file pair still contains repeated operational summaries for:
  - Pre-Execution Gate
  - Plan Mode / Build Mode
  - Design Governance
  - Phase workflow behavior
  - runtime loading behavior
- Recent cleanup fixed obvious local issues, but structural duplication remains.

Affected Files:
- `tera-system/TeraAgent.md`
- `.opencode/agents/tera.md`
- `tera-system/TeraPolicyMap.md` (only if source-of-truth references need refinement)
- `tera-system/TeraArchitectureMap.md` (only if runtime/source boundaries need wording sync)
- `project-control/SYSTEM_EVOLUTION_LOG.md` (after approved execution only)

Proposed Change:
Execute the cleanup in three controlled passes under one approved change stream.

### Pass A — Source-of-Truth Stabilization
- Keep full Pre-Execution Gate detail only in `tera-system/TeraPreExecutionGate.md`.
- Reduce `TeraAgent.md` to a short authority-level reference for Pre-Execution Gate.
- Keep full Design Governance detail only in `tera-system/design-system/`.
- Reduce Tera file pair to the three high rules only:
  - No UI without Design Source Decision.
  - No frontend acceptance without UI Acceptance Gate.
  - Engineering must not invent visual rules.
- Keep full Plan Mode / Build Mode operating behavior in `.opencode/agents/tera.md`.
- Reduce `TeraAgent.md` Plan/Build wording to a short governance-level statement.
- Search and reduce duplicate copies of the most sensitive rules rather than rewriting them everywhere.

### Pass B — TeraAgent Source Slimming
- Reshape `TeraAgent.md` so it functions primarily as:
  - identity
  - authority
  - high-level workflow map
  - critical constraints
  - official references
- Compress heavy operational sections, especially where runtime or policy files already hold the detailed behavior.
- Keep stage definitions, but shorten them to:
  - purpose
  - critical inputs
  - official outputs
  - major blocking rules
  - reference file

### Pass C — Runtime Compression and Clarity
- Keep `.opencode/agents/tera.md` as a compact active runtime file.
- Convert long loading prose into a clearer compact structure.
- Improve distinction between:
  - mandatory operating rules
  - reference/load-this-when-needed guidance
- Reduce runtime repetition of rules already owned by policy/runtime source files.
- Preserve all operational triggers needed for safe live behavior.

Why This Is Necessary:
- It lowers drift risk without weakening Tera.
- It aligns the actual system with `TeraPolicyMap.md` and `TeraArchitectureMap.md`.
- It makes future changes safer because sensitive rules will have clearer ownership.
- It improves maintainability without removing capability.
- It reduces the chance that runtime and source files disagree later.

Rejected Alternatives:
1. **Do nothing**
   - Rejected because duplication around sensitive gates will keep increasing maintenance risk.

2. **Rewrite both files from scratch**
   - Rejected because it is too risky and could weaken Tera’s operational accuracy.

3. **Compress only `.opencode/agents/tera.md` and leave `TeraAgent.md` as-is**
   - Rejected because the deeper problem is source/runtime overlap, not runtime size alone.

4. **Move large sections immediately without phased validation**
   - Rejected because sensitive rule movement should be done incrementally with duplicate checks after each pass.

Anti-Bloat Check:
- What problem does this solve?
  - Conflicting or repeated rule ownership across source, runtime, and policy layers.
- Why is editing existing files enough?
  - The issue is structural duplication, not missing capability; no new architecture is needed.
- Why is no new agent needed?
  - This is a Tera system governance cleanup, not a capability gap requiring another agent.
- Will this reduce or increase complexity?
  - Reduce complexity by clarifying one owner per sensitive rule.
- Token impact?
  - Positive. Smaller runtime instructions and cleaner source boundaries should lower unnecessary load.
- Is there a smaller way?
  - Yes: phased cleanup rather than full rewrite. That is the chosen approach.

Risk:
- Medium if executed carelessly because removing repeated text may accidentally remove an active trigger.
- Low-to-medium if executed in phased passes with duplicate search and runtime validation after each pass.
- Main risk: over-compressing runtime and making Tera too dependent on deep file loading for routine actions.

Rollback Plan:
1. Execute changes in small passes only.
2. Keep each pass reviewable in git diff before continuing to the next pass.
3. If runtime clarity degrades, restore the affected section from git and keep a slightly larger runtime summary.
4. If any rule ownership becomes unclear, revert that pass and restate ownership in `TeraPolicyMap.md` before retrying.

Approval Required:
Majed approval required before executing Pass A, Pass B, or Pass C.

Execution Discipline After Approval:
- Do not execute all passes blindly in one edit batch.
- Complete Pass A first and validate duplicate-sensitive rules.
- Only then continue to Pass B.
- Only then continue to Pass C.
- Update `SYSTEM_EVOLUTION_LOG.md` only after execution is actually completed.
