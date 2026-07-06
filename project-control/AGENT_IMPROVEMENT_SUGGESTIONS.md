# AGENT_IMPROVEMENT_SUGGESTIONS.md

## Purpose

Central log for **Agent Improvement Suggestions (AIS)** — structured, non-active improvement proposals submitted by core agents based on real work experience.

This is **not** a bug or gap log. For bugs, gaps, and missing capabilities, use `AGENT_GAPS_LOG.md`.

---

## Rules

1. Protocol reference: `tera-system/AIS_PROTOCOL.md`
2. Only core agents listed in §2 of the protocol may submit suggestions
3. Each suggestion must follow the official template
4. Maximum 3 suggestions per task/session (unless critical conflict)
5. Suggestions are NOT active rules — they require review and formal implementation
6. Statuses: Proposed → Under Review → Approved for SCP → Rejected → Deferred → Implemented → Verified

---

## Suggestion Template

```markdown
## AIS-NNNN — [Agent Name] — [Short Title]

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

## سجل الاقتراحات

<!-- تبدأ الاقتراحات من هنا -->

## AIS-0001 — System — AIS Protocol Initialization

**Date:** 2026-07-06
**Agent:** TeraSystemEvolutionAgent (حارس)
**Related Task / Session:** SYSTEM — Audit Response
**Severity:** Low
**Type:** Workflow Improvement

### Observation
The external audit body identified that agents have no structured channel to propose self-improvements from real work. The system had only AGENT_GAPS_LOG.md for bugs and gaps.

### Evidence
Audit finding, confirmed by Majed and analyzed by Hares.

### Impact
Without AIS, valuable experiential knowledge from daily work is lost — recurring patterns, skill gaps, and efficiency opportunities go undocumented.

### Proposed Improvement
Implement the full AIS Protocol as defined in `tera-system/AIS_PROTOCOL.md`.

### Suggested Target File
- `tera-system/AIS_PROTOCOL.md`
- `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md`
- All core agent definition files (add AIS section)

### Execution Authority
This suggestion is NOT active.
It requires review by Majed and formal implementation through TeraSystemEvolutionAgent (Hares) after approval.

---

**Status:** Implemented (SCP-2026-07-06-083)
**Verified by:** Majed approval on 2026-07-06
