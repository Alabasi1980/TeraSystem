# Tera Runtime Templates

These templates are official runtime support material for Tera Agent.
Use them when the compact runtime file requests a formal output format.

Authority rule:
If this file conflicts with `.opencode/agents/tera.md`, the active runtime file wins until the conflict is reviewed and corrected.

---

## 1. Tera Decision

```text
Tera Decision:
System files: tera-system is read-only during project execution.
Project output path: project-preparation/
Generated agents path: generated-agents/opencode/
Files to create: ...
Files not needed now: ...
Sub-agents to generate: ...
Sub-agents not needed now: ...
Reason: ...
Next step: ...
```

---

## 2. Delegation Context Format

For every delegated task, specify:

```text
Task ID:
Requested Agent:
Stage:
Objective:
Context Type:
Reference Files:
Required Sections:
Allowed Write Targets:
Forbidden Files / Actions:
Token Budget: Low / Medium / High / Critical
Model Tier Recommendation: Light / Medium / Strong
Minimum Acceptable Model Tier: Light / Medium / Strong
Current Model Assessment: sufficient / acceptable with safeguards / stronger recommended / stronger required / split first
Cost Note: no user approval needed / user approval recommended / user approval required
Reason:
Expected Output Limit:
Acceptance Criteria:
Return Status Required:
```

---

## 3. Model Capability Assessment

```text
Model Capability Assessment
Current Model: [name or "current runtime model"]
Task Complexity: [Low/Medium/High/Critical]
Risk Level: [Low/Medium/High/Critical]
Required Reasoning: [Low/Medium/High/Critical]
Context Size: [Low/Medium/High/Critical]
Verification Difficulty: [Low/Medium/High/Critical]
Historical Fit: [Good/Mixed/Weak/Unknown]
Recommended Model Tier: [Light/Medium/Strong]
Minimum Acceptable Model Tier: [Light/Medium/Strong]
Cost-Saving Option: [use weaker model / split task / reduce context / shorten output / not recommended]
User-Facing Recommendation Required: [Yes/No]
Decision: [sufficient / acceptable with safeguards / stronger recommended / stronger required / split]
Reason: [short reason]
Required Safeguards: [list]
User Approval Needed: Yes / No
Notes: [short notes]
```

Rules:
- Never claim a model is guaranteed or 100% capable.
- Use the weakest sufficient model that preserves safety, traceability, and quality.
- Ask the user about stronger models only when the risk, cost, or verification difficulty justifies it.

---

## 4. Tera Self-Diagnosis Record

For major delegation, phase transition, new agent activation, or risky decision, record briefly:

```text
Tera Self-Diagnosis: PASS / UNCLEAR / BLOCKED
Reason:
Action:
```

---

## 5. Emergency Report

```text
Emergency Report
Level:
Incident:
Affected files/areas:
Likely cause:
Current containment status:
Recommended action:
User approval required: Yes / No
```

---

## 6. Contradiction Detected Notice

```text
Tera Decision Needed: Contradiction Detected

Source A:
Source B:
Conflict:
Risk if A is followed:
Risk if B is followed:

Please choose:
A. [summary]
B. [summary]

Until resolved, I will hold the affected task.
```

---

## 7. Task Prioritization Record

For non-obvious task selection decisions, record briefly:

```text
Selected next task:
Reason:
Skipped ready tasks:
```

---

## 8. Domain Research Brief

Use this before any source-grounded domain research. No open-ended domain research is allowed without this brief.

```text
Domain Research Brief

Topic:
Domain:
Project context:
Research objective:
Allowed sources:
Forbidden sources:
Reference systems:
Depth: Low / Medium / High
Output limit:
Required focus:
Excluded topics:
Return format:
```

---

## 9. Domain Research Report

```text
Domain Research Report

Topic:
Domain:
Sources used:
Source tier:
Key findings:
Common workflows:
Common fields:
Common business rules:
Common roles:
Integration points:
Risks / caveats:
Conflicting findings:
Source confidence:
```

---

## 10. Domain Intelligence Report

```text
Domain Intelligence Report

Topic:
Domain:
Project size:
Reference style:
Sources used:

Core concept:
Business purpose:
Workflow:
Recommended fields:
Business rules:
Validation rules:
Roles and permissions:
Statuses:
Integration points:
Reports / outputs:
Risks if ignored:

MVP recommendation:
Include now:
Recommended:
Defer:
Out of Scope:
Needs User Decision:

Anti-bloat notes:
Tera decision recommendation:
```

---

## 11. Application Discovery Notes

Use this while collecting and normalizing a new application idea.

```text
Application Discovery Notes

Session date:
User raw idea summary:
Normalized application idea:
Problem / need:
Target users:
Main capabilities mentioned:
User preferences:
Technical notes:
Potential domain areas:
MVP candidates:
Later candidates:
Out-of-scope candidates:
Assumptions:
Open questions:
Information documented in:
```

---

## 12. Application Understanding Summary

Use before leaving discovery or moving to project preparation.

```text
Application Understanding Summary

Application name:
Core idea:
Problem solved:
Target users:
Primary goals:
Main workflows:

--- Classified Scope (per MVP_DEFINITION_PROTOCOL) ---
Core MVP (Phase 1A): [features essential for primary workflow]
Extended MVP (Phase 1B): [important additions, non-blocking]
Phase 2: [depends on Core MVP stability]
Phase 3: [advanced capabilities, lower urgency]
Later / Enterprise: [deferred complex features]
Out of scope: [explicitly excluded]

User preferences:
Technical context:
Domain assumptions:
Open questions:
Documented files:
Tera readiness: Ready / Partially Ready / Not Ready
User confirmation needed: Yes / No
Feature classification status: Completed / Pending
```

---

## 13. Discovery User Confirmation Request

```text
Discovery Confirmation Request

This is my current understanding of the application:

[summary]

Please confirm one option:
1. Approved as the basis for project preparation.
2. Mostly correct, with these corrections: ...
3. Not correct; continue discovery.

Until confirmed, I will not move to project preparation or implementation.
```

---

## 14. Research-Based Improvements Review

Use after Domain Intelligence or external research changes, improves, or challenges the initial understanding.

```text
Research-Based Improvements Review

Research / domain source:
What changed or improved:
Suggested additions:
Suggested simplifications:
Suggested deferrals:
Suggested exclusions:
Risks if ignored:
Needs user decision:
Tera recommendation:
User decision:
```

---

## 15. Phased Application Roadmap

Use before execution planning.

```text
Phased Application Roadmap

Phase 1 / MVP:
  Core (Phase 1A):
  - ...
  Extended (Phase 1B):
  - ...

Phase 2:
- ...

Phase 3:
- ...

Later / Enterprise:
- ...

Out of scope:
- ...

Needs user decision:
- ...

Approval status: Pending / Approved / Needs Revision
```

---

## 16. Final Discovery Approval Summary

```text
Final Discovery Approval Summary

Application understanding: Approved / Needs Revision
Project inputs documented: Yes / No
Domain Intelligence applied: Yes / No / Not Needed
Research-based changes reviewed with user: Yes / No / Not Needed
Phased roadmap approved: Yes / No
Remaining open questions:
Approved next mode: Project Preparation / Continue Discovery / Blocked
```

---

## 17. Project Readiness Summary

Use after discovery, optional Domain Intelligence, and phased roadmap drafting, before moving into project preparation or execution planning.

```text
Project Readiness Summary

Application understanding:
Confirmed by user: Yes / No
Feature classification completed: Yes / No
Project inputs documented: Yes / No
Materially important chat-only information remaining: Yes / No
Domain Intelligence status: Completed / Not Needed / Deferred
Research-based improvements reviewed: Yes / No / Not Needed
Approved MVP scope (Core + Extended):
Approved later phases:
Out-of-scope items:
Open questions:
Risks:
Next step:
User approval required: Yes / No
```
