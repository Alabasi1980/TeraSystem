# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-097

Title: Add Fresh File Read Rule for Concurrent Tera Sessions

Request Type: Owner improvement request / Agent runtime governance

Problem:
Majed may run more than one TeraAgent session against the same application at the same time. If one session edits a file based on stale memory or an older snapshot, it may overwrite, remove, or silently revert changes made by another TeraAgent session or sub-agent.

Evidence:
- Majed explicitly warned two TeraAgent sessions that each may edit different files and must not delete another session's changes.
- `.opencode/agents/tera.md` already has context minimization and task-size controls, but it does not explicitly require re-reading an existing file from disk immediately before modifying it.
- Current risk is operational: stale session context can cause accidental rollback or overwrite of unrelated changes.

Affected Files:
- `.opencode/agents/tera.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md` after approved implementation

Proposed Change:
1. Update `.opencode/agents/tera.md` `Last Synced` line to reference this SCP after implementation.
2. Add a compact subsection under `## 12. Execution Orchestration Core`, after the Sub-Agent Delegation Size Rule and before Mid-Task Compliance Checkpoint, titled:
   ```text
   ### Fresh File Read Rule — قاعدة قراءة الملف قبل تعديله
   ```
3. Add the following operational behavior:
   - Before editing any existing file, Tera must read the current file from disk.
   - Tera must not rely on chat memory, previous reads, or assumptions about the file's current content.
   - Delegations to sub-agents must include the same instruction.
   - Any unrelated changes discovered must be preserved.
   - If unexpected changes conflict with the current task, stop the affected part, review the difference, and ask Majed or record a decision before overwriting.

Suggested Runtime Text:
```markdown
### Fresh File Read Rule — قاعدة قراءة الملف قبل تعديله

When editing an existing file, Tera must treat session memory as stale.

Before Tera edits any existing file, or before delegating a sub-agent to edit one, the current version of that file must be read from disk again.

Mandatory delegation instruction:
```text
Before editing any existing file, read the current file from disk first. Preserve unrelated changes, including changes made by another Tera session or sub-agent. Do not overwrite, revert, or delete unrelated changes based on memory or an older snapshot.
```

If the current file contains unexpected changes:
- do not remove them silently,
- do not replace the whole file from an older version,
- limit the edit to the approved task scope,
- if there is a conflict, stop the affected part and ask Majed or record the required decision before overwriting.

Reason: concurrent Tera sessions may work on the same application. Fresh reads prevent accidental deletion of another session's work.
```

Why This Is Necessary:
- Prevents stale-context overwrites in concurrent Tera sessions.
- Protects work done by other agents without adding a new workflow layer.
- Reinforces safe editing and review discipline during Phase 6.

Rejected Alternatives:
- Create a new policy file: rejected as unnecessary bloat for a compact runtime guardrail.
- Modify all sub-agent files: rejected because Tera controls delegation packages and can pass this instruction at task time.
- Rely only on chat warning: rejected because the rule should persist across sessions.

Anti-Bloat Check:
- What problem does it solve? Prevents overwriting/deleting changes from another active Tera session.
- Why not a new file? Existing `.opencode/agents/tera.md` §12 is the correct runtime location.
- Why not a new agent? This is an orchestration/editing discipline, not a new role.
- Complexity impact? Reduces operational risk with one compact rule.
- Token impact? Minimal; adds one short subsection.
- Smaller path? Yes: one runtime subsection and log entry only.

Risk:
- Low. Adds one extra read-before-edit discipline; may slightly slow editing but improves safety.

Rollback Plan:
- Remove the added subsection from `.opencode/agents/tera.md`.
- Restore the previous `Last Synced` line.
- Add rollback note to `SYSTEM_EVOLUTION_LOG.md` if rollback is executed.

Approval Required:
Approved by Majed in chat: `موافق نفذ`.
