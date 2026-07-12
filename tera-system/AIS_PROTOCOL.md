---
description: Agent Improvement Suggestions (AIS) Protocol — allows core agents to propose self-improvements from real work experience, without self-modification.
---

# Agent Improvement Suggestions Protocol (AIS)

## 1. Purpose

This protocol enables every core agent in the Tera system to propose improvements to their own operating instructions or related system files — based on **real work experience** — without modifying themselves or any governance file.

It is a **structured suggestion channel**, not a self-modification permission.

---

## 2. Scope

Applies to all core agents:

- TeraAgent
- TeraClientEngagementAgent
- TeraSystemEvolutionAgent (حارس)
- Monitor (رقيب)
- Auditor (مدقق)
- DesignReviewer (ناقد)
- ApplicationBlueprintAgent
- SoftwareDesignerAgent

---

## 3. What an Agent May Suggest

| Type | Example |
|------|---------|
| **Ambiguity** | Instruction unclear about what to do when data is missing |
| **Missing Rule** | No rule exists for a recurring operational situation |
| **Conflict** | Two files or instructions contradict each other |
| **Workflow Improvement** | Task order could be more efficient |
| **Quality Risk** | Current process may produce inaccurate output |
| **Cost Control** | Pricing or scope estimation lacks a necessary guardrail |
| **Client Handling** | Client interaction pattern reveals a gap in the engagement process |
| **Skill Gap** | Agent frequently needs a piece of knowledge it does not have (cheatsheet, quick reference, comparison table) |
| **Pattern Discovery** | Recurring pattern observed across multiple projects that could be documented as a best practice |

---

## 4. What an Agent Must NOT Suggest

The agent must NOT:

- Modify its own operating file or any governance file
- Create new rules as active rules
- Bypass current rules because it thinks an improvement is needed
- Generate excessive suggestions (see §6 Anti-Spam)
- Suggest cosmetic wording changes unless they affect execution
- Propose changes without evidence from the current task

---

## 5. When to Record — Conditions

An agent may record a suggestion **only if** at least one of these conditions is met:

| Condition | Meaning |
|-----------|---------|
| **Repeated Friction** | The issue occurred more than once |
| **Blocking Ambiguity** | Ambiguity stopped work or required a decision |
| **Quality Risk** | Risk to output quality exists |
| **Scope Risk** | Risk of scope creep or unsupported estimation |
| **Missing Rule** | No clear rule for a situation that appeared |
| **Conflict** | Two files or instructions contradict each other |
| **Client Confusion** | Client was confused or asked clarifying questions due to weak process |

---

## 6. Anti-Spam Rule

Each agent may record **at most 3 suggestions per task/session**, unless a critical conflict is found.

---

## 7. AIS vs GAP — Boundary Rule

```
GAP (AGENT_GAPS_LOG.md)
  → Something is broken, missing, or prevents correct execution
  → e.g., missing permission, conflicting policy, stale reference

AIS (AGENT_IMPROVEMENT_SUGGESTIONS.md)
  → Execution is correct but could be better, faster, clearer, or more skilled
  → e.g., recurring pattern, skill gap, workflow efficiency, template improvement
```

If a situation fits both, use **GAP** (broken takes priority over improvement).

---

## 8. Suggestion Template

```markdown
## AIS-0001 — [Agent Name] — [Short Title]

**Date:** YYYY-MM-DD
**Agent:** [Agent Name]
**Related Task / Session:** TASK-ID or DRYRUN-ID
**Severity:** Low / Medium / High
**Type:** Ambiguity / Missing Rule / Conflict / Workflow Improvement / Quality Risk / Cost Control / Client Handling / Skill Gap / Pattern Discovery

### Observation
What happened during the work?

### Evidence
Quote the exact instruction, file, output, or situation that caused the issue.

### Impact
What risk does this create if not fixed?

### Proposed Improvement
What should be changed or added?

### Suggested Target File
Which file likely needs update?

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.
```

---

## 9. Suggestion Lifecycle

```
Proposed → Under Review → Approved for SCP → Implemented
                                        → Rejected
                                        → Deferred
                                        → Verified
```

| Status | Meaning | Who Sets It |
|--------|---------|------------|
| **Proposed** | New suggestion, not yet reviewed | The agent |
| **Under Review** | Being analyzed by TeraSystemEvolutionAgent | Hares |
| **Approved for SCP** | Accepted — will become a SYSTEM_CHANGE_PROPOSAL | Hares after Majed direction |
| **Rejected** | Rejected with a clear reason | Hares |
| **Deferred** | Postponed to a later review cycle | Hares |
| **Implemented** | Change executed and logged | Hares |
| **Verified** | Post-implementation validation passed | Hares or Majed |

---

## 10. Processing Cycle

```text
Agent notices improvement opportunity during work
  → Records AIS in project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md
  → Continues work without interruption
  → Majed reviews periodically or on request
  → TeraSystemEvolutionAgent (Hares) validates the suggestion
  → If Approved for SCP → Hares produces SYSTEM_CHANGE_PROPOSAL
  → Majed approves/rejects the proposal
  → Hares implements approved changes
  → Hares logs in SYSTEM_EVOLUTION_LOG.md
  → Hares updates AIS status to Implemented / Verified
```

---

## 11. Relationship with Other System Files

| File | Relationship |
|------|-------------|
| `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` | Central log for all AIS entries |
| `project-control/AGENT_GAPS_LOG.md` | AIS is complementary — GAPS = broken, AIS = improvement |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | Approved AIS changes are logged here after implementation |
| `tera-system/TERA_CONTINUOUS_IMPROVEMENT_POLICY.md` | General improvement awareness — AIS is a specific mechanism under it |
| `tera-system/TeraPolicyMap.md` | Maps this protocol as source of truth for AIS |

---

## 12. Authority

This protocol is governed by `TeraSystemEvolutionAgent (حارس)`.

No agent may use this protocol to justify self-modification.
All approved changes must pass through SYSTEM_CHANGE_PROPOSAL → Majed approval → Hares execution.

---

> *"العميل يرى النمط. حارس ينفذ التغيير. Majed يقرر."*
