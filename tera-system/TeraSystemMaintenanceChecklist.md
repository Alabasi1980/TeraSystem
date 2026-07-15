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

## 7. Completion Record

When the maintenance change is complete, summarize:

- source files changed
- runtime sync needed: yes / no
- duplicate rules removed or accepted
- validation performed
- remaining risks
