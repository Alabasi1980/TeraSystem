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

## 10. Domain Intelligence Report (Software Mode)

Software Mode template â€” ظٹظڈط³طھط®ط¯ظ… ط¹ظ†ط¯ظ…ط§ ظٹط³طھط¯ط¹ظٹ TeraAgent ط§ظ„ظ€ DomainExpertAgent ظ„طھط­ظ„ظٹظ„ ظ…ط¬ط§ظ„ ظˆطھطµظ†ظٹظپظ‡ ط­ط³ط¨ MVP.

```text
Domain Intelligence Report (Software)

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

## 10.1 Domain Intelligence Report (Consulting Mode)

Consulting Mode template â€” ظٹظڈط³طھط®ط¯ظ… ط¹ظ†ط¯ظ…ط§ ظٹط³طھط¯ط¹ظٹ TCEA ط§ظ„ظ€ DomainExpertAgent ظ„طھط­ظ„ظٹظ„ ظ…ط¬ط§ظ„ ظˆط¥ظ†طھط§ط¬ظ‡ ط¨طھطµظ†ظٹظپ ظ…ط¹ط±ظپظٹ.

```text
Domain Intelligence Report (Consulting)

Topic:
Domain:
Mode: consulting
Date:
Requested by: TCEA â€” [client name if available]
Sources Used:
[R01, R02, ... or specific report titles]

Domain Overview:
[ظ†ط¸ط±ط© ط¹ط§ظ…ط© ط¹ظ„ظ‰ ط§ظ„ظ…ط¬ط§ظ„ ظˆظ†ط·ط§ظ‚ظ‡]

Core Processes:
[ط§ظ„ط¹ظ…ظ„ظٹط§طھ ط§ظ„ط£ط³ط§ط³ظٹط© â€” ط¬ظˆظ‡ط± ط§ظ„ظ…ط¬ط§ظ„]
- Process 1 [ظ…طµط¯ط±: R01]
- Process 2 [ظ…طµط¯ط±: R02]

Supporting Activities:
[ط§ظ„ط£ظ†ط´ط·ط© ط§ظ„ظ…ط³ط§ظ†ط¯ط© â€” طھط¯ط¹ظ… ط§ظ„ط¹ظ…ظ„ظٹط§طھ ط§ظ„ط£ط³ط§ط³ظٹط©]
- Activity 1 [ظ…طµط¯ط±: R01]

Structural Elements:
[ط§ظ„ط¹ظ†ط§طµط± ط§ظ„ظ‡ظٹظƒظ„ظٹط© ظˆط§ظ„طھظ†ط¸ظٹظ…ظٹط© â€” ط£ط¯ظˆط§ط±طŒ ط£ظ‚ط³ط§ظ…طŒ ظ‡ظٹظƒظ„ظٹط§طھ]
- Element 1 [ظ…طµط¯ط±: R03]

Contextual Knowledge:
[ط§ظ„ظ…ط¹ظ„ظˆظ…ط§طھ ط§ظ„ط³ظٹط§ظ‚ظٹط© â€” طھط§ط±ظٹط®طŒ ط§طھط¬ط§ظ‡ط§طھطŒ ظ…ط¹ظ„ظˆظ…ط§طھ ط®ظ„ظپظٹط©]
- Knowledge 1 [ظ…طµط¯ط±: R02]

Cross-Cutting Topics:
[ط§ظ„ظ…ظˆط§ط¶ظٹط¹ ط§ظ„ط¹ط±ط¶ظٹط© â€” طھظ…ط³ ط¹ط¯ظ‘ط© ط£ظ‚ط³ط§ظ…]
- Topic 1 [ظ…طµط¯ط±: R01, R02]

Relationships & Dependencies:
[ط§ظ„ط¹ظ„ط§ظ‚ط§طھ ظˆط§ظ„طھط±ط§ط¨ط·ط§طھ ط¨ظٹظ† ط§ظ„ط¹ظ†ط§طµط±]

Risks & Considerations:
[ط§ظ„ظ…ط®ط§ط·ط± ظˆط§ظ„ط§ط¹طھط¨ط§ط±ط§طھ ط§ظ„طھظٹ ط¸ظ‡ط±طھ ظ…ظ† ط§ظ„طھط­ظ„ظٹظ„]

---
This report contains [Research Hint] information.
Does NOT define scope, pricing, or commitments.
```

---

## 10.2 Knowledge Structure (Consulting Mode)

Consulting Mode template â€” ظ‡ظٹظƒظ„ ظ‡ط±ظ…ظٹ ظ„ظ„ظ…ط¹ط±ظپط© (3 ظ…ط³طھظˆظٹط§طھ ظƒط­ط¯ ط£ظ‚طµظ‰)طŒ ظٹطµظ†ظپ ظƒظ„ ط¹ظ†طµط± ط­ط³ط¨ ط§ظ„طھطµظ†ظٹظپ ط§ظ„ظ…ط¹ط±ظپظٹ ظˆظٹط±ط¨ط·ظ‡ ط¨ظ…طµط¯ط±ظ‡.

```text
Knowledge Structure

Domain:
Study Title:
Date:
Classification Key: [Core] Core Process / [Supp] Supporting Activity / [Str] Structural Element / [Ctx] Contextual Knowledge / [CC] Cross-Cutting

1. [Chapter 1 Title â€” ط§ظ„ظ…ط¹ط±ظپ ط§ظ„ط£ط³ط§ط³ظٹ ظپظٹ ط§ظ„ظ…ط¬ط§ظ„]
   1.1 [Section 1.1]
       - [Part / Topic 1] â€” [Core/Supp/Str/Ctx/CC] â€” ظ…طµط¯ط±: [R01]
       - [Part / Topic 2] â€” [Core/Supp/Str/Ctx/CC] â€” ظ…طµط¯ط±: [R02]
   1.2 [Section 1.2]
       - [Part / Topic] â€” [Core/Supp/Str/Ctx/CC] â€” ظ…طµط¯ط±: [R01]

2. [Chapter 2 Title]
   2.1 [Section 2.1]
       - [Part / Topic] â€” [Core/Supp/Str/Ctx/CC] â€” ظ…طµط¯ط±: [R03]
   2.2 [Section 2.2]
       - [Part / Topic] â€” [Core/Supp/Str/Ctx/CC] â€” ظ…طµط¯ط±: [R01, R02]

N. [Chapter N Title â€” ط§ظ„ظ…ظˆط§ط¶ظٹط¹ ط§ظ„ط¹ط±ط¶ظٹط© / ط§ظ„ظ…ط´طھط±ظƒط©]
   N.1 [Section N.1]
       - [Part / Topic] â€” [CC] â€” ظ…طµط¯ط±: [R01, R02, R03]

---

Structural Notes:
- Chapters follow logical domain progression: core processes â†’ supporting â†’ structural â†’ contextual
- Cross-cutting topics can be a separate chapter or distributed
- Max depth: 3 levels (Chapter â†’ Section â†’ Part/Topic)
- Every element references its source report
```

---

## 10.3 Gap Analysis (Consulting Mode)

Consulting Mode template â€” طھط­ظ„ظٹظ„ ط§ظ„ظپط¬ظˆط§طھ ظپظٹ ط§ظ„ظ…ط¹ط±ظپط© ط¨ط¹ط¯ ظ…ظ‚ط§ط±ظ†ط© ط¬ظ…ظٹط¹ ط§ظ„طھظ‚ط§ط±ظٹط± ط§ظ„ط¨ط­ط«ظٹط© ظ…ط¹ ظ…ط§ طھط­طھط§ط¬ظ‡ ط§ظ„ط¯ط±ط§ط³ط©.

```text
Gap Analysis

Domain:
Date:
Sources analyzed: [R01, R02, ...]

Coverage Summary:
- Fully covered: [X]%
- Partially covered: [Y]%
- Missing: [Z]%

| # | Topic / Element | Classification | Current Coverage | Gap Description | Priority | Suggested Research | Source |
|:-:|:---------------|:-------------:|:----------------:|:---------------|:--------:|:-----------------|:------:|
| 1 | [Topic name] | [Core/Supp/Str/Ctx/CC] | Covered / Partial / Missing | [ظˆطµظپ ط§ظ„ظپط¬ظˆط©] | H/M/L | [ط§ظ‚طھط±ط§ط­ ط¨ط­ط« ط¥ط¶ط§ظپظٹ] | R01 |
| 2 | [Topic name] | [Core/Supp/Str/Ctx/CC] | Covered / Partial / Missing | [ظˆطµظپ ط§ظ„ظپط¬ظˆط©] | H/M/L | [ط§ظ‚طھط±ط§ط­ ط¨ط­ط« ط¥ط¶ط§ظپظٹ] | R02 |

Priority Legend:
- H (High): ط§ظ„ظپط¬ظˆط© طھظ…ظ†ط¹ ط¥ظƒظ…ط§ظ„ ط§ظ„ط¯ط±ط§ط³ط© ط£ظˆ طھط¤ط«ط± ط¹ظ„ظ‰ ط¬ظˆط¯ط© ط§ظ„ظ…ط®ط±ط¬ط§طھ
- M (Medium): ط§ظ„ظپط¬ظˆط© طھط¤ط«ط± ط¹ظ„ظ‰ ط§ظ„ط¹ظ…ظ‚ ظˆظ„ظƒظ† ظٹظ…ظƒظ† طھط¬ط§ظˆط²ظ‡ط§ ظ…ط¤ظ‚طھط§ظ‹
- L (Low): ط§ظ„ظپط¬ظˆط© ظپظٹ ظ…ط¹ظ„ظˆظ…ط§طھ ط³ظٹط§ظ‚ظٹط© ط£ظˆ طھظƒظ…ظٹظ„ظٹط©

Coverage Details:
- Covered: ط§ظ„ظ…ط¹ظ„ظˆظ…ط© ظƒط§ظ…ظ„ط© ظˆظ…ظپظ‡ظˆظ…ط© ظ…ظ† ط§ظ„ظ…طµط§ط¯ط±
- Partial: ط§ظ„ظ…ط¹ظ„ظˆظ…ط© ظ…ظˆط¬ظˆط¯ط© ظ„ظƒظ†ظ‡ط§ ظ†ط§ظ‚طµط© ط£ظˆ ط؛ظٹط± ظ…ط¤ظƒط¯ط© â€” ظٹط­طھط§ط¬ طھظˆط«ظٹظ‚ط§ظ‹ ط¥ط¶ط§ظپظٹط§ظ‹
- Missing: ظ„ط§ طھظˆط¬ط¯ ظ…ط¹ظ„ظˆظ…ط§طھ â€” ظٹط­طھط§ط¬ ط¨ط­ط«ط§ظ‹ ط¥ط¶ط§ظپظٹط§ظ‹ ط£ظˆ ط§ط³طھط´ط§ط±ط© ط¹ظ…ظٹظ„

Recommended Next Actions:
1. [ط¥ط¬ط±ط§ط، ظ…ظ‚طھط±ط­ ظ„ظ„ظپط¬ظˆط§طھ ط¹ط§ظ„ظٹط© ط§ظ„ط£ظˆظ„ظˆظٹط©]
2. [ط¥ط¬ط±ط§ط، ظ…ظ‚طھط±ط­ ظ„ظ„ظپط¬ظˆط§طھ ظ…طھظˆط³ط·ط© ط§ظ„ط£ظˆظ„ظˆظٹط©]
3. [ط¥ط¬ط±ط§ط، ظ…ظ‚طھط±ط­ ظ„ظ„ظپط¬ظˆط§طھ ظ…ظ†ط®ظپط¶ط© ط§ظ„ط£ظˆظ„ظˆظٹط©]
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

---

## 18. Client Question Set

Use when Majed needs questions to forward to the client.

```text
Client Question Set

Purpose:
Questions to send to the client:
1. ...
2. ...
3. ...

Why these questions matter:
Expected next step after answers:
```

---

## 19. Client Profile Template

```text
# Client Profile

Client name:
Client type: Individual / Company / Organization / Unknown
Business domain:
Default client-facing language: Arabic
Technical familiarity: Low / Medium / High / Unknown
Decision style:
Communication notes:
Project sensitivity: Low / Medium / High / Critical
Preferred approval method:
General notes:
```

---

## 20. Client Contacts Template

```text
# Client Contacts

| Name | Role | Decision Authority | Phone | Email | Preferred Channel | Approval Authority | Notes |
|---|---|---|---|---|---|---|---|
|  |  | Decision maker / Reviewer / Technical / Finance / Other |  |  |  | Yes / No / Unknown |  |
```

---

## 21. Client Approval Package Checklist

```text
Client Approval Package Checklist

Client:
Application:
Package path:

Required files:
- 01_CLIENT_PROJECT_BRIEF.md: Present / Missing / Not applicable with reason
- 02_CLIENT_PROPOSAL.md: Present / Missing / Not applicable with reason
- 03_SCOPE_OF_WORK.md: Present / Missing / Not applicable with reason
- 04_FEATURE_SCOPE_MATRIX.md: Present / Missing / Not applicable with reason
- 05_USER_FLOWS.md: Present / Missing / Not applicable with reason
- 06_SCREEN_MAP.md: Present / Missing / Not applicable with reason
- 07_DESIGN_DIRECTION.md: Present / Missing / Not applicable with reason
- 08_PROTOTYPE_PLAN.md: Present / Missing / Not applicable with reason
- 09_ACCEPTANCE_CRITERIA.md: Present / Missing / Not applicable with reason
- 10_CLIENT_APPROVAL_RECORD.md: Present / Missing
- 11_CHANGE_CONTROL.md: Present / Missing

Approval gates:
- Idea Approval: Approved / Pending / Needs Revision
- Scope Approval: Approved / Pending / Needs Revision
- Flow Approval: Approved / Pending / Needs Revision
- Screen Approval: Approved / Pending / Needs Revision
- Design Direction Approval: Approved / Pending / Needs Revision
- Prototype Approval: Approved / Pending / Not Applicable / Needs Revision
- Execution Authorization: Approved / Pending / Blocked

Build Mode allowed: Yes / No
Reason:
```

---

## 22. Client Approval Record

```text
# Client Approval Record

Client:
Application:
Approval date:
Approving contact:
Approval authority: Confirmed / Unknown / User-confirmed

Approved documents:
- ...

Approval gates:
| Gate | Status | Notes |
|---|---|---|
| Idea Approval |  |  |
| Scope Approval |  |  |
| Flow Approval |  |  |
| Screen Approval |  |  |
| Design Direction Approval |  |  |
| Prototype Approval |  |  |
| Execution Authorization |  |  |

Pending decisions:
Rejected or deferred items:
Execution authorization status: Approved / Pending / Blocked
```

---

## 23. Client Change Request Record

```text
Change Request

Change ID:
Date:
Requester:
Request summary:
Affected approved file or gate:
Classification: Clarification / Minor Adjustment / Enhancement / New Scope / Phase 2 / Rejected
Scope impact:
Design impact:
Technical impact:
Time/cost impact if known:
Decision: Approve / Defer / Reject / Needs Client Decision
Approval authority:
Related task or issue:
```

---

## 24. Client Decision Needed

```text
Client Decision Needed

Decision topic:
Why it matters:
Options:
1. ...
2. ...
3. ...

Tera recommendation:
Impact if delayed:
Can implementation continue without this decision? Yes / No
```

---

## 25. Client Approval File Outlines

Use these outlines when creating files under `clients/.../client-approval/`. Client-facing content is Arabic by default.

### 25.1 `01_CLIENT_PROJECT_BRIEF.md`

```text
# ظ…ظ„ط®طµ ظپظƒط±ط© ط§ظ„ظ…ط´ط±ظˆط¹

ط§ط³ظ… ط§ظ„ط¹ظ…ظٹظ„:
ط§ط³ظ… ط§ظ„طھط·ط¨ظٹظ‚:
ظˆطµظپ ظ…ط®طھطµط± ظ„ظ„ظ…ط´ط±ظˆط¹:
ط§ظ„ظ…ط´ظƒظ„ط© ط§ظ„طھظٹ ظٹط­ظ„ظ‡ط§:
ط§ظ„ظ…ط³طھط®ط¯ظ…ظˆظ† ط§ظ„ظ…ط³طھظ‡ط¯ظپظˆظ†:
ط§ظ„ط£ظ‡ط¯ط§ظپ ط§ظ„ط±ط¦ظٹط³ظٹط©:
ط§ظ„ظ‚ظٹظ…ط© ط§ظ„ظ…طھظˆظ‚ط¹ط© ظ„ظ„ط¹ظ…ظٹظ„:
ط­ط¯ظˆط¯ ط§ظ„ظ†ط³ط®ط© ط§ظ„ط£ظˆظ„ظ‰:
ظ…ظ„ط§ط­ط¸ط§طھ ط£ظˆ ظ‚ط±ط§ط±ط§طھ ظ…ط¹ظ„ظ‚ط©:
ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.2 `02_CLIENT_PROPOSAL.md`

```text
# ط¹ط±ط¶ ظ…ط´ط±ظˆط¹

ظ…ظ‚ط¯ظ…ط©:
ظپظ‡ظ…ظ†ط§ ظ„ط§ط­طھظٹط§ط¬ ط§ظ„ط¹ظ…ظٹظ„:
ظ†ط·ط§ظ‚ ط§ظ„ط¹ظ…ظ„ ط§ظ„ظ…ظ‚طھط±ط­:
ط§ظ„ظ…ط®ط±ط¬ط§طھ ط§ظ„ظ…طھظˆظ‚ط¹ط©:
ظ…ط±ط§ط­ظ„ ط§ظ„ط¹ظ…ظ„:
ظ…ط§ ظ‡ظˆ ط®ط§ط±ط¬ ط§ظ„ظ†ط·ط§ظ‚:
ط§ظ„ط§ظپطھط±ط§ط¶ط§طھ:
ط§ظ„ظ…طھط·ظ„ط¨ط§طھ ظ…ظ† ط§ظ„ط¹ظ…ظٹظ„:
ط¢ظ„ظٹط© ط§ظ„ط§ط¹طھظ…ط§ط¯ ظˆط§ظ„طھط؛ظٹظٹط±ط§طھ:
ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.3 `03_SCOPE_OF_WORK.md`

```text
# ظ†ط·ط§ظ‚ ط§ظ„ط¹ظ…ظ„

ط¯ط§ط®ظ„ ط§ظ„ظ†ط·ط§ظ‚:
- ...

ط®ط§ط±ط¬ ط§ظ„ظ†ط·ط§ظ‚:
- ...

ظ…ط¤ط¬ظ„ ظ„ظ…ط±ط­ظ„ط© ظ„ط§ط­ظ‚ط©:
- ...

ظ‚ظٹظˆط¯ ظ…ظ‡ظ…ط©:
ط§ظپطھط±ط§ط¶ط§طھ:
ظ‚ط±ط§ط±ط§طھ ظ…ط¹ظ„ظ‚ط©:
ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.4 `04_FEATURE_SCOPE_MATRIX.md`

```text
# ظ…طµظپظˆظپط© ظ†ط·ط§ظ‚ ط§ظ„ظ…ظٹط²ط§طھ

| ط§ظ„ظ…ظٹط²ط© | ط§ظ„طھطµظ†ظٹظپ | ط§ظ„ط£ظˆظ„ظˆظٹط© | ط§ظ„ظ…ظ„ط§ط­ط¸ط§طھ |
|---|---|---|---|
|  | ط¯ط§ط®ظ„ ط§ظ„ظ†ط·ط§ظ‚ / ظ…ط¤ط¬ظ„ / ط®ط§ط±ط¬ ط§ظ„ظ†ط·ط§ظ‚ / ظٹط­طھط§ط¬ ظ‚ط±ط§ط± | ط¹ط§ظ„ظٹط© / ظ…طھظˆط³ط·ط© / ظ…ظ†ط®ظپط¶ط© |  |

ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.5 `05_USER_FLOWS.md`

```text
# ظ…ط³ط§ط±ط§طھ ط§ظ„ط§ط³طھط®ط¯ط§ظ…

## ط§ظ„ظ…ط³ط§ط± ط§ظ„ط£ظˆظ„: [ط§ظ„ط§ط³ظ…]

ط§ظ„ظ…ط³طھط®ط¯ظ…:
ط§ظ„ظ‡ط¯ظپ:
ط§ظ„ط®ط·ظˆط§طھ:
1. ...

ط§ظ„ط­ط§ظ„ط§طھ ط§ظ„ط¨ط¯ظٹظ„ط©:
ط§ظ„ظ†طھظٹط¬ط© ط§ظ„ظ…طھظˆظ‚ط¹ط©:

ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.6 `06_SCREEN_MAP.md`

```text
# ط®ط±ظٹط·ط© ط§ظ„ط´ط§ط´ط§طھ

| ط§ظ„ط´ط§ط´ط© | ط§ظ„ط؛ط±ط¶ | ط§ظ„ظ…ط³طھط®ط¯ظ…ظˆظ† | ظ…ظ„ط§ط­ط¸ط§طھ |
|---|---|---|---|
|  |  |  |  |

ط´ط§ط´ط§طھ ط؛ظٹط± ظ…ط·ظ„ظˆط¨ط© ظپظٹ ط§ظ„ظ†ط³ط®ط© ط§ظ„ط£ظˆظ„ظ‰:
ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.7 `07_DESIGN_DIRECTION.md`

```text
# ط§ظ„طھظˆط¬ظ‡ ط§ظ„ط¨طµط±ظٹ

ط§ظ„ظ„ط؛ط© ظˆط§ظ„ط£ط³ظ„ظˆط¨ ط§ظ„ط¹ط§ظ…:
ط§ظ„ط£ظ„ظˆط§ظ† ط£ظˆ ط§ظ„ظ‡ظˆظٹط©:
ظ…ط±ط§ط¬ط¹ طھط¹ط¬ط¨ ط§ظ„ط¹ظ…ظٹظ„:
ظ…ط±ط§ط¬ط¹ ظ„ط§ طھط¹ط¬ط¨ ط§ظ„ط¹ظ…ظٹظ„:
ط§ظ†ط·ط¨ط§ط¹ ط§ظ„طھطµظ…ظٹظ… ط§ظ„ظ…ط·ظ„ظˆط¨: ط±ط³ظ…ظٹ / ط­ط¯ظٹط« / ط¨ط³ظٹط· / ظپط§ط®ط± / ط´ط¨ط§ط¨ظٹ / ط¢ط®ط±
ظ‚ظٹظˆط¯ ط§ظ„طھطµظ…ظٹظ…:
ظ…ط§ ظ„ط§ ظٹط¬ط¨ ط§ط³طھط®ط¯ط§ظ…ظ‡:
ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.8 `08_PROTOTYPE_PLAN.md`

```text
# ط®ط·ط© ط§ظ„ط¨ط±ظˆطھظˆطھط§ظٹط¨

ظ‡ظ„ ط§ظ„ط¨ط±ظˆطھظˆطھط§ظٹط¨ ظ…ط·ظ„ظˆط¨طں ظ†ط¹ظ… / ظ„ط§
ط³ط¨ط¨ ط§ظ„ظ‚ط±ط§ط±:
ط§ظ„ط´ط§ط´ط§طھ ط£ظˆ ط§ظ„طھط¯ظپظ‚ط§طھ ط§ظ„طھظٹ ظٹط¬ط¨ طھظ…ط«ظٹظ„ظ‡ط§:
ظ…ط³طھظˆظ‰ ط§ظ„طھظپطµظٹظ„: ظ…ظ†ط®ظپط¶ / ظ…طھظˆط³ط· / ط¹ط§ظ„ظٹ
ط§ظ„ط£ط¯ط§ط© ط§ظ„ظ…ظ‚طھط±ط­ط© ط¥ظ† ظˆط¬ط¯طھ:
ظ…ط¹ط§ظٹظٹط± ظ‚ط¨ظˆظ„ ط§ظ„ط¨ط±ظˆطھظˆطھط§ظٹط¨:
ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.9 `09_ACCEPTANCE_CRITERIA.md`

```text
# ظ…ط¹ط§ظٹظٹط± ط§ظ„ظ‚ط¨ظˆظ„

| ط§ظ„ظ…ظٹط²ط© ط£ظˆ ط§ظ„ط´ط§ط´ط© | ظ…ط¹ظٹط§ط± ط§ظ„ظ‚ط¨ظˆظ„ | ط·ط±ظٹظ‚ط© ط§ظ„طھط­ظ‚ظ‚ | ط§ظ„ط­ط§ظ„ط© |
|---|---|---|---|
|  |  |  | ظ…ظ‚ط¨ظˆظ„ / ظٹط­طھط§ط¬ طھط¹ط¯ظٹظ„ / ظ…ط¹ظ„ظ‚ |

ظ…ط¹ط§ظٹظٹط± ظ‚ط¨ظˆظ„ ط¹ط§ظ…ط©:
ظ…ط¹ط§ظٹظٹط± ظ„ط§ طھط¹طھط¨ط± ط¶ظ…ظ† ط§ظ„طھط³ظ„ظٹظ… ط§ظ„ط­ط§ظ„ظٹ:
ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:
```

### 25.10 `11_CHANGE_CONTROL.md`

```text
# ط³ط¬ظ„ ط·ظ„ط¨ط§طھ ط§ظ„طھط؛ظٹظٹط±

| Change ID | ط§ظ„طھط§ط±ظٹط® | ظ…ظ‚ط¯ظ… ط§ظ„ط·ظ„ط¨ | ط§ظ„ظ…ظ„ط®طµ | ط§ظ„طھطµظ†ظٹظپ | ط§ظ„ظ‚ط±ط§ط± | ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯ |
|---|---|---|---|---|---|---|
| CHG-001 |  |  |  | Clarification / Minor Adjustment / Enhancement / New Scope / Phase 2 / Rejected |  |  |

ظ…ظ„ط§ط­ط¸ط§طھ ط¹ط§ظ…ط©:
```

---

## 26. Client-Facing Application Proposal

This is not a text template but an **HTML document**. The proposal is generated as a self-contained HTML page with embedded CSS for professional presentation, RTL support, and print optimization.

Reference file: `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html`

After the Client Discovery + Smart Interview process completes, Tera populates the template with:
- Application name, date, client name
- Problem and solution description
- Users, roles, and permissions
- Core MVP features and out-of-scope items
- Requirements by domain (functional, technical, data, design, security, ops)
- Assumptions table with status
- Proposed roadmap phases
- Approval section

The generated proposal is saved to:
- `clients/.../client-approval/` (external clients)
- `project-inputs/` (internal projects)

See `TERA_RUNTIME_PROTOCOLS.md` Section 18, Client Discovery Step 7 for the protocol.

---

## 27. Project Preparation Plan (Phase 3 Output)

This template is used for the formal output of Phase 3 (Project Preparation Planning).
The generated file is saved to `project-control/PREPARATION_PLAN.md`.

```markdown
# PREPARATION_PLAN.md

## 1. Preparation Decision

Decision: Proceed / Blocked / Needs More Intake

> Reference: `project-preparation/TERA_PROJECT_DECISION.md`

## 2. Required Preparation Files

| File | Required | Reason | Owner Agent | Order |
|---|---|---|---|---|
| `01_PROJECT_BRIEF.md` | Yes | Core understanding | RequirementsScopeAgent | 1 |
| `02_SCOPE_AND_BOUNDARIES.md` | Yes | Scope discipline | RequirementsScopeAgent | 2 |
| `03_MODULES_AND_FEATURES.md` | Conditional | Medium+ projects | RequirementsScopeAgent | 3 |
| ... | ... | ... | ... | ... |

### Classification Key

| Label | Meaning |
|---|---|
| **Required** | Must be created now |
| **Conditional** | Create only if the trigger condition is met |
| **Deferred** | Postponed to a later phase |
| **Not Required** | Not needed for this project |

## 3. Deferred Files

| File | Reason | Trigger for Activation |
|---|---|---|
| `14_INTEGRATIONS_...` | No external services yet | When integration is confirmed |
| `22_DEPLOYMENT_...` | Deployment not imminent | Before first deployment |
| ... | ... | ... |

## 4. Not Required Files

| File | Reason |
|---|---|
| `23_BACKUP_AND_RECOVERY.md` | Internal prototype, no production data |
| `34_COMPLIANCE_...` | No regulatory requirements |
| ... | ... |

## 5. Suggested Sub-Agents

| Agent | Needed Now | Reason |
|---|---|---|
| `RequirementsScopeAgent` | Yes | Core scope files (01, 02, 03, 04) |
| `BusinessWorkflowAgent` | Conditional | Only if workflows are complex |
| `DataDesignAgent` | Conditional | Only if data model is non-trivial |
| `UIUXStructureAgent` | Conditional | Only if screens need structured definition |
| `UIVisualDesignerAgent` | Conditional | Only if visual design tokens/component rules are needed |
| `SolutionArchitectureAgent` | Conditional | Only if architecture decisions are risky |
| ... | ... | ... |

## 6. Preparation Sequence

```
Batch A (no dependencies):
  01_PROJECT_BRIEF.md (RequirementsScopeAgent)
  08_TECHNICAL_ARCHITECTURE.md (SolutionArchitectureAgent or Tera)

Batch B (depends on Batch A):
  02_SCOPE_AND_BOUNDARIES.md (RequirementsScopeAgent)
  04_USERS_ROLES_PERMISSIONS.md (RequirementsScopeAgent)

Batch C (depends on Batch B):
  05_BUSINESS_WORKFLOWS.md (BusinessWorkflowAgent)
  07_SCREENS_AND_UI_STRUCTURE.md (UIUXStructureAgent)

Batch D (depends on Batch C):
  06_DATA_MODEL_PREPARATION.md (DataDesignAgent)
  09_IMPLEMENTATION_PLAN.md (Tera)
```

## 7. User Approval Points

| Point | What Needs Approval | Before Moving To |
|---|---|---|
| P1 | This plan (Preparation Decision) | Phase 4: Sub-Agent Generation & Preparation Delegation |
| P2 | Scope and boundaries (02) | File creation for downstream files |
| P3 | Technical architecture (08) | Implementation planning |
| P4 | Implementation plan (09) | Phase 5: Execution Planning |

> **Rule:** No file creation happens in Phase 3. No agent generation happens before this plan is approved.

## 8. Approval Status

- [ ] Plan submitted
- [ ] Plan approved â†’ Proceed to Phase 4
- [ ] Plan rejected â†’ Revise and resubmit
- [ ] Plan blocked â†’ Reason: ...
```

---

## 28. Agent Delegation Plan (Phase 4 Output)

This template is used for the formal output of Phase 4 (Sub-Agent Generation & Preparation Delegation).
The generated file is saved to `project-control/AGENT_DELEGATION_PLAN.md`.

```markdown
# AGENT_DELEGATION_PLAN.md

## 1. Delegation Decision

Decision: Proceed / Needs User Approval / Blocked

> Reference: `project-control/PREPARATION_PLAN.md`

## 2. Agents Needed Now

| Agent | Reason | Status | Assigned Files |
|---|---|---|---|
| `RequirementsScopeAgent` | Core scope files (01, 02, 03, 04) | Generate / Use Existing / Specialize | `01_PROJECT_BRIEF.md`, `02_SCOPE_...`, ... |
| `BusinessWorkflowAgent` | Business workflows (05) | Generate / Use Existing / Specialize | `05_BUSINESS_WORKFLOWS.md` |
| ... | ... | ... | ... |

### Agent Status Key

| Status | Meaning |
|---|---|
| **Generate** | Agent does not exist â€” create from `AGENT_GENERATION_TEMPLATE.md` |
| **Use Existing** | Agent exists and is suitable â€” use directly |
| **Specialize** | Agent exists but is too generic â€” narrow sources/targets/constraints |

## 3. Agents Not Needed Now

| Agent | Reason | Defer Until |
|---|---|---|
| `SecurityAgent` | No sensitive data yet | Phase 5 or when auth is implemented |
| `DevOpsDeploymentAgent` | No deployment imminent | Before first deployment |
| ... | ... | ... |

## 4. Agent Generation Actions

| Agent | Action | Output File | Token Budget | Context Rules |
|---|---|---|---|---|
| `RequirementsScopeAgent` | Generate | `generated-agents/opencode/RequirementsScopeAgent.md` | Medium | Task Context |
| `BusinessWorkflowAgent` | Specialize (exists) | Update existing file | Medium | Task Context |
| ... | ... | ... | ... | ... |

## 5. Delegation Map

| Preparation File | Assigned Agent | Allowed Sources | Allowed Write Targets |
|---|---|---|---|
| `01_PROJECT_BRIEF.md` | RequirementsScopeAgent | `00_PROJECT_INPUTS.md`, `01_APPLICATION_IDEA.md` | `project-preparation/01_PROJECT_BRIEF.md` |
| `02_SCOPE_AND_BOUNDARIES.md` | RequirementsScopeAgent | `01_PROJECT_BRIEF.md` | `project-preparation/02_SCOPE_...` |
| `05_BUSINESS_WORKFLOWS.md` | BusinessWorkflowAgent | `02_SCOPE_...`, `04_USERS_ROLES_...` | `project-preparation/05_BUSINESS_...` |
| ... | ... | ... | ... |

## 6. Activation Plan

| Agent | Activate Now? | Reason |
|---|---|---|
| `RequirementsScopeAgent` | Yes | Batch A: core scope files |
| `SolutionArchitectureAgent` | Yes | Batch A: technical architecture (parallel) |
| `BusinessWorkflowAgent` | No | Deferred until Batch C |

## 7. User Approval Points

| Point | What Needs Approval | Before Moving To |
|---|---|---|
| A1 | Agent Generation Plan (this document) | Generating/activating agents |
| A2 | Activating agents in `.opencode/agents/` | Start of delegation |
| A3 | Scope files (01, 02) from first delegation batch | Remaining preparation files |

> **Rules:**
> - No approved PREPARATION_PLAN.md = No Sub-Agent Generation.
> - No generated/approved agent = No delegated preparation file.
> - No active need = No active sub-agent.

## 8. Approval Status

- [ ] Plan submitted
- [ ] Plan approved â†’ Proceed to agent generation
- [ ] Plan rejected â†’ Revise and resubmit
- [ ] Plan blocked â†’ Reason: ...
```

---

## 29. Project Master Plan (Phase 5 â€” Output)

This template is used for the first formal output of Phase 5 (Execution Planning).
The generated file is saved to `project-control/PROJECT_MASTER_PLAN.md`.

```markdown
# PROJECT_MASTER_PLAN.md

## 1. Plan Metadata

| Item | Value |
|---|---|
| Project | [NAME] |
| Version | 1.0 |
| Status | Draft / Approved / Active |
| Reference | `project-preparation/09_IMPLEMENTATION_PLAN.md` |

## 1.1 Relationship to Preparation Plan

| File | Role |
|---|---|
| `project-preparation/09_IMPLEMENTATION_PLAN.md` | Preliminary implementation plan produced during preparation |
| `project-control/PROJECT_MASTER_PLAN.md` | Official execution roadmap after preparation approval |
| `project-control/PROJECT_DETAILED_EXECUTION_PLAN.md` | Detailed traceable execution items |
| `project-control/EXECUTION_BATCH_PLAN.md` | Next approved executable batch only |
| `project-control/tasks/TASK-COD-XXX.md` | Actual executable unit |

## 2. Execution Phases

| Phase | Name | Objective | Roadmap Tier | Depends On | Status |
|---|---|---|---|---|---|
| 1 | [e.g. Technical Foundation] | [e.g. Scaffold project, init ORM, verify startup] | Core MVP / Foundation | â€” | Planned |
| 2 | [e.g. Database Schema] | [e.g. Define models, create migration, seed] | Core MVP | Phase 1 | Planned |
| 3 | [e.g. Core Feature] | [e.g. Main workflow implementation] | Core MVP / Extended MVP | Phase 1, 2 | Planned |
| ... | ... | ... | ... | ... | ... |

## 2.1 Formal Phased Roadmap

| Roadmap Tier | Included Phases / Features | Explicitly Excluded / Later |
|---|---|---|
| Core MVP | ... | ... |
| Extended MVP | ... | ... |
| Phase 2 | ... | ... |
| Later / Out of Scope | ... | ... |

> **Rule:** No detailed execution planning or `TASK-COD-*` generation before this master plan, including the formal phased roadmap, is approved.

## 3. Transition Conditions

| From | To | Condition |
|---|---|---|
| Phase 1 | Phase 2 | Project starts, dev env works, ORM connects to DB |
| Phase 2 | Phase 3 | Schema applied, seed data verified |
| ... | ... | ... |

## 4. Design Source Decisions

| Phase / Batch | Design Source | Decided? |
|---|---|---|
| All UI phases | [INTERNAL_TERA_KIT / GETDESIGN_MD / FIGMA_DESIGN_FILE / USER_PROVIDED_REFERENCE / EXTERNAL_URL_ANALYSIS / HYBRID / NO_UI / N/A] | Yes / No |
| Phase 3 (UI) | [source] | Yes / No |

> **Rule:** No UI phase/batch without a Design Source Decision.

## 5. Deferred Items

| Item | Reason | Phase |
|---|---|---|
| ... | ... | ... |

## 6. Approval

- [ ] Submitted
- [ ] Approved â†’ Ready for Detailed Planning
- [ ] Needs revision
```

---

## 30. Detailed Execution Plan (Phase 5 â€” Output)

This template breaks each phase into traceable execution items.
The generated file is saved to `project-control/PROJECT_DETAILED_EXECUTION_PLAN.md`.

```markdown
# PROJECT_DETAILED_EXECUTION_PLAN.md

## 1. Source Reference

- Master Plan: `project-control/PROJECT_MASTER_PLAN.md`
- Implementation Plan: `project-preparation/09_IMPLEMENTATION_PLAN.md`

## 2. Phase Breakdown

### Phase [N]: [Phase Name]

| Item ID | Description | Linked TASK-ID | Depends On | Status | Notes |
|---|---|---|---|---|---|
| P1-01 | [e.g. Scaffold Next.js project] | TASK-COD-001 | â€” | Planned | |
| P1-02 | [e.g. Init Prisma + connect to DB] | TASK-COD-002 | P1-01 | Planned | See profile: nextjs-prisma |
| P2-01 | [e.g. Define User model] | TASK-COD-003 | P1-02 | Planned | |
| P2-02 | [e.g. Create migration + apply] | TASK-COD-004 | P2-01 | Planned | |
| ... | ... | ... | ... | ... | ... |

### Item Status Legend

| Status | Meaning |
|---|---|
| Planned | Defined, not yet assigned |
| In Progress | Assigned to agent, being executed |
| Completed | Executed and accepted |
| Blocked | Cannot proceed without resolution |
| Deferred | Moved to later phase |

## 3. Approval

- [ ] Submitted
- [ ] Approved â†’ Ready for batch planning
- [ ] Needs revision
```

---

## 31. Execution Batch Plan (Phase 5 â€” Output)

This template defines the current approved batch only â€” not the full project.
The generated file is saved to `project-control/EXECUTION_BATCH_PLAN.md`.

```markdown
# EXECUTION_BATCH_PLAN.md

## 1. Batch Metadata

| Item | Value |
|---|---|
| Batch | [Number / Name] |
| Phase | [Phase from Master Plan] |
| Status | Draft / Approved / In Progress / Completed |
| Source Plan | `project-control/PROJECT_DETAILED_EXECUTION_PLAN.md` |

## 2. Included Tasks

| TASK-ID | Description | Assigned Agent | Allowed Write Targets | Pre-Execution Gate |
|---|---|---|---|---|
| TASK-COD-001 | [e.g. Scaffold project] | EngineeringAgent | `.` (project root) | PASS |
| TASK-COD-002 | [e.g. Init ORM + DB] | EngineeringAgent | `prisma/schema.prisma`, `.env.example` | PASS |
| ... | ... | ... | ... | ... |

## 3. Not Included (Deferred to Later Batches)

| Item | Reason | Expected Batch |
|---|---|---|
| Schema design | Depends on scaffold completion | Batch 2 |
| UI components | No Design Source Decision yet | Batch 3 |

## 4. Design Source Decision (for this batch)

- [ ] INTERNAL_TERA_KIT
- [ ] GETDESIGN_MD
- [ ] FIGMA_DESIGN_FILE
- [ ] USER_PROVIDED_REFERENCE
- [ ] EXTERNAL_URL_ANALYSIS
- [ ] HYBRID
- [ ] NO_UI
- [ ] N/A

## 5. User Approval

- [ ] Batch plan submitted
- [ ] Approved â†’ Begin execution (Phase 6)
- [ ] Rejected â†’ Revise
- [ ] Blocked â†’ Reason: ...

---

> **Rules:**
> - No Implementation without Execution Plan.
> - No UI Task without Design Source Decision.
> - No UI Task without `28_UI_UX_GUIDELINES.md` when visual style matters.
> - UI tasks must link `tera-system/design-system/UI_ACCEPTANCE_GATE.md`.
> - No TASK-ID without Pre-Execution Gate PASS.
> - No batch execution without user approval.
```

---

## 32. Post-Execution Review (Phase 6 Review Template)

This template is used inside `project-control/tasks/TASK-COD-XXX.md` after an agent handback and before any task acceptance or closure.

```markdown
## Post-Execution Review

| Check | Result | Notes |
|---|---|---|
| TASK objective completed? | PASS / FAIL | ... |
| Output matches approved scope? | PASS / FAIL | ... |
| No files outside Allowed Write Targets? | PASS / FAIL | ... |
| No forbidden files created? | PASS / FAIL | ... |
| No unexpected libraries added? | PASS / FAIL | ... |
| No secrets, tokens, passwords, or real `.env` values? | PASS / FAIL | ... |
| Technology Profile respected? | PASS / FAIL | ... |
| UI/UX rules respected if UI exists? | PASS / FAIL / N/A | ... |
| UI Acceptance Gate passed if UI exists? | PASS / FAIL / N/A | ... |
| Acceptance Criteria passed? | PASS / FAIL | ... |
| CLI / tool side effects reviewed? | PASS / FAIL / N/A | ... |
| Rollback needed? | Yes / No | ... |

Gate Result:
PASS / NEEDS_FIX / BLOCKED

Final Tera Decision:
Accepted / Needs Fix / Blocked / Rework Needed / Deferred / Cancelled

Required Record Updates:
- [ ] `project-control/tasks/TASK-COD-XXX.md`
- [ ] `project-control/TASK_REGISTRY.md`
- [ ] `project-control/PROJECT_ACTIVITY_LOG.md`
- [ ] `project-control/PROJECT_STATE.md`
- [ ] `project-control/ISSUES_AND_GAPS.md` if needed
```

---

## 33. Compliance Record (Task Closure Governance Summary)

ظ‡ط°ط§ ط§ظ„ظ‚ط³ظ… ظٹظڈط¶ط§ظپ ط¥ظ„ظ‰ `project-control/tasks/TASK-COD-XXX.md` ظƒط¢ط®ط± ظ‚ط³ظ… ظ‚ط¨ظ„ ط§ظ„ط¥ط؛ظ„ط§ظ‚ (ط¨ط¹ط¯ `Post-Execution Review`).
ظˆظ‡ظˆ ط§ظ„ظ…ط±ط¬ط¹ ط§ظ„ظ…ط¹طھظ…ط¯ ظ„ظ€ Monitor ظ„ظ„طھط­ظ‚ظ‚ ظ…ظ† ظ…ط·ط§ط¨ظ‚ط© Handback + Git diff + ط§ظ„ظ‚ظˆط§ط¹ط¯.

```markdown
## Compliance Record

| # | Check | Result | Verified By |
|---|---|---|---|
| 1 | Pre-Execution Gate: PASS documented in task file | PASS / N/A | Tera |
| 2 | Allowed Write Targets respected | PASS / FAIL | Tera |
| 3 | No secrets/tokens/passwords in outputs or logs | PASS / FAIL | Tera |
| 4 | Design Source Decision documented (if UI exists) | PASS / N/A | Tera |
| 5 | Post-Execution Review: PASS | PASS / FAIL | Tera |
| 6 | PROJECT_ACTIVITY_LOG.md updated | PASS / FAIL | Tera |
| 7 | Handback recorded in TASK-ID file | PASS / FAIL | Tera |
| 8 | Git diff matches Handback description | PASS / FAIL / PENDING | Monitor\* |
| 9 | CLI/commands documented (if any) | Done / N/A | Tera |

\* Item 8: Monitor ظٹطھط­ظ‚ظ‚ ط¹ظ†ط¯ ظ†ط´ط§ط·ظ‡. ط¥ط°ط§ ظ„ظ… ظٹظƒظ† Monitor ظ†ط´ط·ط§ظ‹طŒ ظٹظˆط«ظ‚ Tera ط§ظ„ظپط­طµ ط§ظ„ط°ط§طھظٹ.

Compliance Status: COMPLIANT / NON-COMPLIANT / PENDING_MONITOR_REVIEW
```

---

## 34. UI/UX Guidelines Template (Design Governance Output)

This template defines the required structure of `project-preparation/28_UI_UX_GUIDELINES.md`.

Required sections:

```markdown
# 28_UI_UX_GUIDELINES.md

## 1. Design Source Decision
## 2. Approved Design Direction
## 3. Raw Design Sources
## 4. Client Branding Overrides
## 5. Design Tokens
## 6. Layout System
## 7. Component Rules
## 8. RTL/LTR Rules
## 9. Responsive Rules
## 10. Accessibility Rules
## 11. Motion Rules
## 12. Forbidden Styling
## 13. Engineering Implementation Instructions
## 14. UI Acceptance Checklist
## 15. Figma Source Mapping (when FIGMA_DESIGN_FILE is active)
## 16. Open Design Gaps
```

Rule:

```text
EngineeringAgent implements UI from this file first. Missing rules become Design Gaps, not guesses.
```

---

## 34. Phase 7 Delivery, Handover & Closure Templates

These templates are used after Phase 6 implementation is complete. Phase 7 does not execute code. Blocking findings return to Phase 6 as `TASK-COD-FIX-*`.

### 34.1 Delivery Readiness Report

Generated file: `project-control/DELIVERY_READINESS_REPORT.md`

```markdown
# DELIVERY_READINESS_REPORT.md

## 1. Metadata

| Field | Value |
|---|---|
| Project |  |
| Phase | 7 â€” Delivery, Handover & Closure |
| Date |  |
| Prepared By | Tera / QAAndAcceptanceAgent / DevOpsAgent |
| Status | Draft / Ready / Blocked |

## 2. Entry Gate

| Check | Result | Notes |
|---|---|---|
| All approved TASK-COD closed or deferred | PASS / FAIL |  |
| Post-Execution Reviews complete | PASS / FAIL |  |
| No undocumented Critical blockers | PASS / FAIL |  |
| TASK_REGISTRY current | PASS / FAIL |  |
| PROJECT_STATE current | PASS / FAIL |  |
| ISSUES_AND_GAPS current | PASS / FAIL / N/A |  |

## 3. Delivery Readiness

| Area | Result | Notes |
|---|---|---|
| Core workflows | Ready / Not Ready |  |
| Final QA / smoke | PASS / FAIL / N/A |  |
| Regression review | PASS / FAIL / N/A |  |
| Documentation | Ready / Needs Work |  |
| Deployment readiness | Ready / Not Ready / N/A |  |
| Security closure | PASS / FAIL / N/A |  |

## 4. Blockers

| Blocker | Severity | Required Action | Return to Phase 6? |
|---|---|---|---|
|  | Critical / High / Medium / Low |  | Yes / No |

## 5. Result

Delivery Readiness Status: READY / NEEDS_FIX / BLOCKED
```

### 34.2 Final Acceptance Checklist

Generated file: `project-control/FINAL_ACCEPTANCE_CHECKLIST.md`

```markdown
# FINAL_ACCEPTANCE_CHECKLIST.md

| Acceptance Area | Result | Evidence / Notes |
|---|---|---|
| Approved scope delivered | PASS / FAIL |  |
| Deferred items documented | PASS / FAIL / N/A |  |
| Open issues reviewed | PASS / FAIL |  |
| User/client acceptance recorded | PASS / FAIL |  |
| No hidden blockers | PASS / FAIL |  |
| Handover package ready if client project | PASS / FAIL / N/A |  |

Final Acceptance Status: ACCEPTED / NEEDS_FIX / BLOCKED
```

### 34.3 Release Notes

Generated file: `project-control/RELEASE_NOTES.md`

```markdown
# RELEASE_NOTES.md

## Version / Release

- Release Name:
- Date:
- Scope:

## Delivered

- ...

## Changed

- ...

## Fixed

- ...

## Deferred / Not Included

- ...

## Known Issues

- ...
```

### 34.4 Post-Implementation Review

Generated file: `project-control/POST_IMPLEMENTATION_REVIEW.md`

```markdown
# POST_IMPLEMENTATION_REVIEW.md

## What Went Well
- ...

## What Was Difficult
- ...

## Scope / Quality Notes
- ...

## Process Improvements
- ...

## Follow-up Recommendations
- ...
```

### 34.5 Project Closure Report

Generated file: `project-control/PROJECT_CLOSURE_REPORT.md`

```markdown
# PROJECT_CLOSURE_REPORT.md

## 1. Closure Metadata

| Field | Value |
|---|---|
| Project |  |
| Closure Date |  |
| Closure Decision | Closed / Needs Final Fix / Deferred / Blocked |
| Approved By | User / Client / Tera |

## 2. Completion Summary

- Delivered:
- Deferred:
- Out of Scope:

## 3. Required Outputs

| Output | Status | Notes |
|---|---|---|
| Delivery Readiness Report | Complete / N/A |  |
| Final Acceptance Checklist | Complete / N/A |  |
| Release Notes | Complete / N/A |  |
| Client Handover Package | Complete / N/A |  |

## 4. Open Issues / Deferred Items

| Item | Status | Recommended Action |
|---|---|---|
|  | Deferred / Closed / Won't Fix |  |

## 5. Final Decision

Project Closure Status: CLOSED / BLOCKED / NEEDS_PHASE_6_FIX
```

### 34.6 Client Handover Package

Generated file for external client projects:

```text
clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/delivery/CLIENT_HANDOVER_PACKAGE.md
```

```markdown
# CLIENT_HANDOVER_PACKAGE.md

## 1. طھط³ظ„ظٹظ… ط§ظ„ظ…ط´ط±ظˆط¹

- ط§ط³ظ… ط§ظ„ط¹ظ…ظٹظ„:
- ط§ط³ظ… ط§ظ„طھط·ط¨ظٹظ‚:
- طھط§ط±ظٹط® ط§ظ„طھط³ظ„ظٹظ…:
- ظ†ط³ط®ط© ط§ظ„طھط³ظ„ظٹظ…:

## 2. ظ…ط§ طھظ… طھط³ظ„ظٹظ…ظ‡

- ...

## 3. ط·ط±ظٹظ‚ط© ط§ظ„طھط´ط؛ظٹظ„ / ط§ظ„ظˆطµظˆظ„

- ظ„ط§ طھظƒطھط¨ ط£ط³ط±ط§ط±ظ‹ط§ ط£ظˆ ظƒظ„ظ…ط§طھ ظ…ط±ظˆط±.
- ط£ط´ط± ظپظ‚ط· ط¥ظ„ظ‰ ط£ظ† ط§ظ„ط¨ظٹط§ظ†ط§طھ ط§ظ„ط­ط³ط§ط³ط© ظ…ط­ظپظˆط¸ط© ظƒظ€ local environment secrets ط£ظˆ ط¹ط¨ط± ظ‚ظ†ط§ط© ط¢ظ…ظ†ط©.

## 4. ط§ظ„ط¹ظ†ط§طµط± ط§ظ„ظ…ط¤ط¬ظ„ط© ط£ظˆ ط؛ظٹط± ط§ظ„ظ…ط´ظ…ظˆظ„ط©

- ...

## 5. ط§ظ„ظ‚ط¨ظˆظ„ ط§ظ„ظ†ظ‡ط§ط¦ظٹ

- Accepted / Needs Fix / Deferred
- ط§ط³ظ… طµط§ط­ط¨ ط§ظ„ط§ط¹طھظ…ط§ط¯:
- ط§ظ„طھط§ط±ظٹط®:
```

---

## 35. Discovery Coverage Summary (TCEA â€” Phase 2 Mandatory Output)

This template defines `client-engagement/DISCOVERY_COVERAGE_SUMMARY.md`.
It is produced by TCEA after completing the 13-domain discovery and before any scope/quotation/handoff.

```markdown
# DISCOVERY_COVERAGE_SUMMARY.md

## 1. Metadata

| Field | Value |
|---|---|
| Client | [name] |
| Application | [name] |
| Prepared by | TCEA |
| Date | YYYY-MM-DD |
| Last Updated | YYYY-MM-DD |

## 2. Domain Coverage Matrix

> ط§ظ„طھط±ظ‚ظٹظ… ظˆط§ظ„طھط³ظ…ظٹط© ط­ط³ط¨ ط§ظ„ظ…طµط¯ط± ط§ظ„ط±ط³ظ…ظٹ: `tera-system/client-helpers/tera-client-engagement-discovery-domains.md`

| # | Domain | Status | Reason if not Complete | Impact | Risk | Blocks Pricing? | Blocks Handoff? | **Source of Info** | **Confirmed by Majed?** | **Risk if Wrong** |
|---|--------|--------|----------------------|--------|------|----------------|-----------------|--------------------|-------------------------|-------------------|
| 1 | Business & Goals | Complete / Partial / Missing / Deferred / N/A | ... | ... | L/M/H | Yes/No | Yes/No | Majed / Websearch / Inference / Unknown | Yes / No / Partially | L / M / H |
| 2 | Users, Roles & Access | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 3 | Process & Workflow | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 4 | Data & Content | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 5 | Scope & MVP | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 6 | Screens & UX | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 7 | Notifications Engine | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 8 | Reports & Dashboards | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 9 | Design & Branding | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 10 | Technical, Hosting & Compliance | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 11 | Security & Audit | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 12 | Integrations & APIs | ... | ... | ... | ... | ... | ... | ... | ... | ... |
| 13 | Acceptance, Commercials & Warranty | ... | ... | ... | ... | ... | ... | ... | ... | ... |

> **ظ…ظ„ط§ط­ط¸ط© ط§ظ„ظ…ط¬ط§ظ„ 13:** ظٹط­طھط§ط¬ طھط؛ط·ظٹط© 3 ط¬ظˆط§ظ†ط¨ ط¯ط§ط®ظ„ظٹط© ط¹ظ„ظ‰ ط§ظ„ط£ظ‚ظ„: (ط£) ظ…ط¹ط§ظٹظٹط± ط§ظ„ظ‚ط¨ظˆظ„ ظˆط§ظ„ط§ط®طھط¨ط§ط±ط§طھ, (ط¨) ط§ظ„ظ…ظٹط²ط§ظ†ظٹط© ظˆط®ط·ط© ط§ظ„ط¯ظپط¹, (ط¬) ط§ظ„ط¶ظ…ط§ظ† ظˆط§ظ„طµظٹط§ظ†ط©.

## 3. Coverage Decision

| Item | Value |
|---|---|
| Overall Status | Ready for Scope / Needs More Discovery / Ready for Quotation / Ready for Handoff / Blocked |
| Missing Critical Domains | [list or none] |
| Next Action | [explicit next step] |
| Approved by Majed | Yes / No / Pending |

---

## 36. TERA_HANDOFF_PACKAGE.md â€” TCEA Output (Phase 3 Mandatory Output)

This is the official template for `client-engagement/TERA_HANDOFF_PACKAGE.md`.
It is produced by TCEA after scope/quotation approval, packaging everything for delivery to ApplicationBlueprintAgent â†’ TeraAgent.

**SCP-038 Compliance:** This template includes a dedicated compliance section (آ§1). Every handoff MUST include this section â€” do NOT remove it.

```markdown
# TERA_HANDOFF_PACKAGE.md

> **ط­ط²ظ…ط© ط§ظ„طھط³ظ„ظٹظ… ط§ظ„ط±ط³ظ…ظٹط© â€” ظ…طھظˆط§ظپظ‚ط© ظ…ط¹ SCP-038**
> **ط§ظ„ط¹ظ…ظٹظ„:** [Client Name]
> **ط§ظ„طھط·ط¨ظٹظ‚:** [Application Name]
> **ط§ظ„ظ…ط±ط­ظ„ط©:** Phase 1 (MVP) / Phase 2 / Phase 3
> **طھط§ط±ظٹط® ط§ظ„ط¥ط¹ط¯ط§ط¯:** YYYY-MM-DD
> **ط§ظ„ط­ط§ظ„ط©:** [Draft / Approved]
> **SCP-038 Compliance:** âœ… ظ…طھظˆط§ظپظ‚ط© â€” طھظ… طھط·ط¨ظٹظ‚ ط§ظ„ظ‚ظˆط§ط¹ط¯ ط§ظ„ط£ط±ط¨ط¹ (Reconciliation, Budget-to-Scope, Decision Register, Approval Consistency)
> **ط¥ط¹ط¯ط§ط¯:** TCEA

---

## ظپظ‡ط±ط³ ط§ظ„ظ…ط­طھظˆظٹط§طھ

| ط§ظ„ظ‚ط³ظ… | ط§ظ„ظ…ط­طھظˆظ‰ |
|:------|:--------|
| 1 | ط§ظ„طھظˆط§ظپظ‚ ظ…ط¹ SCP-038 |
| 2 | ط®ظ„ط§طµط© طھظ†ظپظٹط°ظٹط© |
| 3 | ظ…ط¹ظ„ظˆظ…ط§طھ ط§ظ„ط¹ظ…ظٹظ„ |
| 4 | ظ…ظ„ط®طµ ط§ظ„ظ†ط·ط§ظ‚ |
| 5 | ظ‚ط§ط¦ظ…ط© ط§ظ„ظ…ظٹط²ط§طھ ط§ظ„ظ…ط¹طھظ…ط¯ط© |
| 6 | ط®ط§ط±ط¬ ط§ظ„ظ†ط·ط§ظ‚ + ط§ظ„ظ…ط¤ط¬ظ„ |
| 7 | ظ…ظ„ط®طµ ط§ظ„طھط³ط¹ظٹط± ظˆط§ظ„ط´ط±ظˆط· ط§ظ„طھط¬ط§ط±ظٹط© |
| 8 | ظ…ظ„ط®طµ ط³ط¬ظ„ ط§ظ„ظ‚ط±ط§ط±ط§طھ |
| 9 | ط§ظ„ظ†ظ‚ط§ط· ط§ظ„ظ…ظپطھظˆط­ط© ظˆط§ظ„ظ…ط¹ظ„ظ‚ط© |
| 10 | ط§ظ„ظ…ط±ط§ط¬ط¹ ظˆط§ظ„ظ…ظ„ظپط§طھ |
| 11 | ظ‚ط±ط§ط± ط§ظ„ط¬ط§ظ‡ط²ظٹط© ظ„ظ„طھط³ظ„ظٹظ… |

---

## 1. ط§ظ„طھظˆط§ظپظ‚ ظ…ط¹ SCP-038

طھظ… طھط·ط¨ظٹظ‚ ظ‚ظˆط§ط¹ط¯ SCP-038 ط§ظ„ط£ط±ط¨ط¹ ط¹ظ„ظ‰ ط­ط²ظ…ط© ط§ظ„طھط³ظ„ظٹظ…:

| ط§ظ„ظ‚ط§ط¹ط¯ط© | ط§ظ„ط­ط§ظ„ط© | ط§ظ„طھظپط§طµظٹظ„ |
|---------|:------:|----------|
| **آ§3.3.1 Final Scope Reconciliation Gate** | âœ… ظ…ط·ط¨ظ‘ظ‚ / âڑ ï¸ڈ ط؛ظٹط± ظ…ط·ط¨ظ‘ظ‚ | [FEATURE_LIST.md: ظƒظ„ ظ…ظٹط²ط© ظ…طµظ†ظپط© ط¨ط­ط§ظ„طھظ‡ط§ ط§ظ„ظ†ظ‡ط§ط¦ظٹط© âœ… Included / â—‰ Optional / âڈ³ Phase 2 / â‌Œ Out of Scope] |
| **آ§3.3.2 Budget-to-Scope Control Rule** | âœ… ظ…ط·ط¨ظ‘ظ‚ / âڑ ï¸ڈ ط؛ظٹط± ظ…ط·ط¨ظ‘ظ‚ | [ظƒظ„ ظ…ظٹط²ط© ظ…طµظ†ظپط© Essential/Important/Nice-to-have ط¶ظ…ظ† ط§ظ„ظ…ظٹط²ط§ظ†ظٹط© ط§ظ„ظ…ط­ط¯ط¯ط©] |
| **آ§3.3.3 Client Decision Register** | âœ… ظ…ط·ط¨ظ‘ظ‚ / âڑ ï¸ڈ ط؛ظٹط± ظ…ط·ط¨ظ‘ظ‚ | [CLIENT_DECISION_LOG.md: ط¬ظ…ظٹط¹ ط§ظ„ظ‚ط±ط§ط±ط§طھ ظ…ظˆط«ظ‚ط© ط¨ط­ط§ظ„ط§طھ ظ…ظˆط­ط¯ط©] |
| **آ§3.6.1 Approval Consistency Rule** | âœ… ظ…ط·ط¨ظ‘ظ‚ / âڑ ï¸ڈ ط؛ظٹط± ظ…ط·ط¨ظ‘ظ‚ | [ط§ظ„ط­ط²ظ…ط© ظ…طھط³ظ‚ط© ظ…ط¹ ط£ظ‚ظ„ ط­ط§ظ„ط© ظ…طµط¯ط± â€” ط¬ظ…ظٹط¹ ط§ظ„ظ…طµط§ط¯ط± ظ…ط¹طھظ…ط¯ط© / ط¨ط¹ط¶ظ‡ط§ Draft] |

> **ظ…ظ„ط§ط­ط¸ط©:** ط¥ط°ط§ ظƒط§ظ†طھ ط£ظٹ ظ‚ط§ط¹ط¯ط© ط؛ظٹط± ظ…ط·ط¨ظ‘ظ‚ط©طŒ ط§ط°ظƒط± ط§ظ„ط³ط¨ط¨ ظˆط§ظ„ط®ط·ط© ظ„طھظپط¹ظٹظ„ظ‡ط§ ظپظٹ ظ‚ط³ظ… ط§ظ„ظ†ظ‚ط§ط· ط§ظ„ظ…ظپطھظˆط­ط© (آ§9).

---

## 2. ط®ظ„ط§طµط© طھظ†ظپظٹط°ظٹط©

[2-3 ط¬ظ…ظ„ طھظ„ط®طµ: ظ…ظ† ظ‡ظˆ ط§ظ„ط¹ظ…ظٹظ„طŒ ط§ظ„ظ…ط´ظƒظ„ط© ط§ظ„طھظٹ ظٹط­ظ„ظ‡ط§ ط§ظ„طھط·ط¨ظٹظ‚طŒ ط§ظ„ط­ظ„ ط§ظ„ظ…ظ‚طھط±ط­طŒ ظˆط­ط§ظ„ط© ط§ظ„طھط³ظ„ظٹظ…]

---

## 3. ظ…ط¹ظ„ظˆظ…ط§طھ ط§ظ„ط¹ظ…ظٹظ„

| ط§ظ„ط­ظ‚ظ„ | ط§ظ„ظ‚ظٹظ…ط© |
|-------|--------|
| **Client name** | [Full Name] |
| **Application name** | [Full Name] |
| **Business goal** | [Goal statement] |
| **Problem statement** | [Problem statement] |
| **Client approval status** | âœ… Approved / âڈ³ Pending |
| **SCP-038 Compliance** | âœ… ظ…طھظˆط§ظپظ‚ط© |

---

## 4. ظ…ظ„ط®طµ ط§ظ„ظ†ط·ط§ظ‚

| ط§ظ„ط­ظ‚ظ„ | ط§ظ„ظ‚ظٹظ…ط© |
|-------|--------|
| **Approved scope** | [Core scope description] |
| **MVP scope** | [Bullet list of MVP features] |
| **Phase 2** | [Items for Phase 2] |
| **Phase 3** | [Items for Phase 3] |
| **Out of scope** | [Items explicitly out of scope] |

---

## 5. ظ‚ط§ط¦ظ…ط© ط§ظ„ظ…ظٹط²ط§طھ ط§ظ„ظ…ط¹طھظ…ط¯ط©

| # | ط§ظ„ظ…ظٹط²ط© | ط§ظ„ط­ط§ظ„ط© ط§ظ„ظ†ظ‡ط§ط¦ظٹط© Stack | ط§ظ„ط£ظˆظ„ظˆظٹط© ظ„ظ„ظ…ظٹط²ط§ظ†ظٹط© | ظ…ظ„ط§ط­ط¸ط§طھ |
|---|--------|----------------------|-------------------|---------|
| 1 | [Feature] | âœ… Included / â—‰ Optional / âڈ³ Phase 2 / â‌Œ Out of Scope | Essential / Important / Nice-to-have | [Notes] |

> **ط§ظ„ظ…ظپطھط§ط­:** âœ… Included = ظ…ط´ظ…ظˆظ„ط© ظپظٹ ظ‡ط°ظ‡ ط§ظ„ظ…ط±ط­ظ„ط© آ· â—‰ Optional = ط§ط®طھظٹط§ط±ظٹط© آ· âڈ³ Phase 2/3 = ظ…ط¤ط¬ظ„ط© آ· â‌Œ Out of Scope = ط؛ظٹط± ظ…ط´ظ…ظˆظ„ط© ط£ط¨ط¯ط§ظ‹

---

## 6. ط®ط§ط±ط¬ ط§ظ„ظ†ط·ط§ظ‚ + ط§ظ„ظ…ط¤ط¬ظ„

| ط§ظ„ط¨ظ†ط¯ | ط§ظ„ط­ط§ظ„ط© | ط§ظ„ط³ط¨ط¨ | ط§ظ„طھط§ط±ظٹط® ط§ظ„ظ…ط³طھظ‡ط¯ظپ |
|-------|--------|-------|:----------------:|
| [Item] | ط®ط§ط±ط¬ ط§ظ„ظ†ط·ط§ظ‚ / ظ…ط¤ط¬ظ„ ظ„ظ€ Phase 2/3 | [Reason] | [Target date or TBD] |

---

## 7. ظ…ظ„ط®طµ ط§ظ„طھط³ط¹ظٹط± ظˆط§ظ„ط´ط±ظˆط· ط§ظ„طھط¬ط§ط±ظٹط©

| ط§ظ„ط­ظ‚ظ„ | ط§ظ„ظ‚ظٹظ…ط© |
|-------|--------|
| **ط§ظ„ط³ط¹ط± ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ** | [Amount] JOD |
| **ط³ظٹط§ط³ط© ط§ظ„طھط³ط¹ظٹط±** | [e.g. Tera Pricing Policy v4.2] |
| **ظ†ط·ط§ظ‚ ط§ظ„ط¯ظپط¹** | [Payment terms] |
| **ط§ظ„ط¶ظ…ط§ظ†** | [Warranty terms] |
| **ط§ظ„ظ…ط±ط¬ط¹** | DRAFT_QUOTATION.md |

---

## 8. ظ…ظ„ط®طµ ط³ط¬ظ„ ط§ظ„ظ‚ط±ط§ط±ط§طھ

| # | ط§ظ„طھط§ط±ظٹط® | ط§ظ„ظ‚ط±ط§ط± | ط§ظ„ط­ط§ظ„ط© |
|---|---------|--------|:------:|
| 1 | YYYY-MM-DD | [Decision description] | âœ… Approved / âڈ³ Deferred / âڑ ï¸ڈ Conditional / â‌“ Not Finalized |

> **ط§ظ„ظ…ط±ط¬ط¹:** CLIENT_DECISION_LOG.md ظ„ظ„طھظپط§طµظٹظ„ ط§ظ„ظƒط§ظ…ظ„ط©

---

## 9. ط§ظ„ظ†ظ‚ط§ط· ط§ظ„ظ…ظپطھظˆط­ط© ظˆط§ظ„ظ…ط¹ظ„ظ‚ط©

| # | ط§ظ„ظ†ظ‚ط·ط© | ط§ظ„طھط£ط«ظٹط± | ظ…ط·ظ„ظˆط¨ ظ…ظ† | ط§ظ„طھط§ط±ظٹط® ط§ظ„ظ…ط³طھظ‡ط¯ظپ |
|---|--------|---------|----------|:----------------:|
| 1 | [Description] | [Impact] | Majed / Client / TCEA | YYYY-MM-DD |

---

## 10. ط§ظ„ظ…ط±ط§ط¬ط¹ ظˆط§ظ„ظ…ظ„ظپط§طھ

| ط§ظ„ظ…ظ„ظپ | ط§ظ„ظ…ط³ط§ط± | ط§ظ„ط¥طµط¯ط§ط± | ظ…ظ„ط§ط­ط¸ط§طھ |
|-------|--------|:-------:|---------|
| CLIENT_INTAKE.md | `client-engagement/` | v1.0 | [Notes] |
| CLIENT_BRIEF.md | `client-engagement/` | v1.0 | [Notes] |
| SCOPE_SUMMARY.md | `client-engagement/` | v1.0 | [Notes] |
| FEATURE_LIST.md | `client-engagement/` | v1.0 | [Notes] |
| DRAFT_QUOTATION.md | `client-engagement/` | v1.0 | [Notes] |
| CLIENT_DECISION_LOG.md | `client-engagement/` | v1.0 | [Notes] |
| DISCOVERY_COVERAGE_SUMMARY.md | `client-engagement/` | v1.0 | [Notes] |
| GATE_COMPLIANCE_RECORD.md | `client-engagement/` | v1.0 | [Notes] |

---

## 11. ظ‚ط±ط§ط± ط§ظ„ط¬ط§ظ‡ط²ظٹط© ظ„ظ„طھط³ظ„ظٹظ…

| ط§ظ„ط­ظ‚ظ„ | ط§ظ„ظ‚ظٹظ…ط© |
|-------|--------|
| **Handoff Readiness Gate** | ًںں¢ ط¬ط§ظ‡ط² / ًںں، ط¬ط§ظ‡ط² ظ…ط¹ ظ…ظ„ط§ط­ط¸ط§طھ / ًں”´ ط؛ظٹط± ط¬ط§ظ‡ط² |
| âœ… | [Checklist item 1] |
| âœ… | [Checklist item 2] |
| **ط§ظ„ط®ط·ظˆط© ط§ظ„طھط§ظ„ظٹط©:** | [Next step after handoff] |
| **طھط§ط±ظٹط® ط§ظ„طھط³ظ„ظٹظ…:** | YYYY-MM-DD |

---

> **طھظ… ط¥ط¹ط¯ط§ط¯ ظ‡ط°ظ‡ ط§ظ„ط­ط²ظ…ط© ط¨ظˆط§ط³ط·ط©:** TCEA
> **طھط§ط±ظٹط® ط§ظ„طھط­ط¯ظٹط«:** YYYY-MM-DD
> **ط­ط§ظ„ط© ط§ظ„ط§ط¹طھظ…ط§ط¯:** Draft / Approved
```
```

