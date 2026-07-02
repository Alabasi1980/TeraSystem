# Tera System Maintenance Checklist

## 1. Purpose

Use this checklist when modifying Tera system files, runtime files, policies, templates, agents, or folder architecture.

## 2. Before Editing

Check:

- Is the change for Tera itself, not a normal client project?
- Which file is the source of truth according to `TeraPolicyMap.md`?
- Is this a new rule, a clarification, or a runtime summary?
- Can an existing file be updated instead of creating a new file?
- Will this affect `.opencode/agents/tera.md` runtime behavior?
- Does this require updating `TeraPolicyMap.md` or `TeraArchitectureMap.md`?

## 3. During Editing

Maintain these rules:

- Update the source of truth first.
- Keep runtime summaries compact.
- Do not copy full policy sections into runtime.
- Do not define the same mandatory gate in two sources.
- Do not add sub-agent roles outside `TeraSubAgents.md`.
- Do not add client approval rules outside the client policies unless they are short summaries.
- Preserve folder boundaries from `TeraArchitectureMap.md`.
- When adding a policy, folder, layer, or lifecycle stage, update the policy and architecture maps in the same pass.

## 4. Runtime Sync Check

Sync `.opencode/agents/tera.md` only if the change affects:

- file paths or folder roles
- intake/build gates
- client approval gates
- sub-agent generation or activation
- pre/post execution gates
- runtime loading rules
- authority order
- emergency or contradiction handling
- policy map, architecture map, or source-of-truth behavior

If synced, update `Last Synced` in `.opencode/agents/tera.md`.

## 5. Anti-Bloat Check

Before finishing, verify:

- Did a rule get copied into more files than needed?
- Is there a new file that could be a section in an existing file?
- Is a checklist duplicating a policy?
- Is a policy duplicating a template?
- Is the runtime file becoming a full manual?
- Are client-facing rules duplicated outside client policies without need?

## 6. Validation Commands

Recommended checks:

```text
git diff --check
git status --short
git diff --stat
```

Also search for the changed rule name to detect duplicates or conflicts.

## 7. TeraSystemEvolutionAgent-Specific Checks

If the change is executed by `TeraSystemEvolutionAgent`:

- Was a `SYSTEM_CHANGE_PROPOSAL` produced and approved before any edit?
- Was the change logged in `project-control/SYSTEM_EVOLUTION_LOG.md`?
- Did the change pass the Anti-Bloat Gate?
- Does the change require `TeraPolicyMap.md` or `TeraArchitectureMap.md` update?
- Did the change avoid modifying client application files?
- Did the change avoid unauthorized tool/MCP additions?
- Does the change preserve folder boundaries from `TeraArchitectureMap.md`?

### 7.1 SoftwareDesignerAgent-Specific Checks

If introducing or modifying `SoftwareDesignerAgent`:

- Is it defined as **mandatory** (not conditional) for every `TASK-COD-*`?
- Does `TeraSubAgents.md` §6.9 correctly replace `ExecutionPreparationAgent`?
- Does `AGENT_ACTIVATION_MATRIX.md` show mandatory activation for all task types?
- Does `AGENT_PERMISSION_MODEL.md` assign the correct permission level (`PLAN_ONLY`)?
- Does `TeraPreExecutionGate.md` §3.6 require `TECHNICAL_SPECIFICATION.md` before Pre-Execution Gate?
- Does `TASK_TEMPLATE.md` §6.1 reference `SoftwareDesignerAgent` and `TECHNICAL_SPECIFICATION.md`?
- Is the `TECHNICAL_SPECIFICATION.md` template available in `TERA_RUNTIME_TEMPLATES.md`?
- Have all references to `ExecutionPreparationAgent` been removed or updated?
- Does the agent definition include a **No-Guessing Rule** (Design Gap instead)?
- Is the agent a consumer (not producer) of preparation files?

### 7.2 TeraClientEngagementAgent-Specific Checks

If the change is executed by `TeraClientEngagementAgent`:

- Was the `SYSTEM_CHANGE_PROPOSAL` produced and approved before any edit?
- Is the change within the allowed scope (client-engagement/ folder and documents only)?
- Did the change avoid modifying client application source code?
- Did the change avoid modifying Tera system files (tera-system/, .opencode/agents/tera.md) unless authorized?
- Did the change pass the Handoff Validation (TERA_HANDOFF_PACKAGE.md completeness)?
- Were all pricing/contract changes kept as Draft-only?
- Was Majed's explicit approval obtained for any commitment?
- Does the change respect the boundary: TeraClientEngagementAgent does not communicate with TeraAgent directly?

## 8. Completion Record

When the maintenance change is complete, summarize:

- source files changed
- runtime sync needed: yes / no
- duplicate rules removed or accepted
- validation performed
- remaining risks
