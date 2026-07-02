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
# ملخص فكرة المشروع

اسم العميل:
اسم التطبيق:
وصف مختصر للمشروع:
المشكلة التي يحلها:
المستخدمون المستهدفون:
الأهداف الرئيسية:
القيمة المتوقعة للعميل:
حدود النسخة الأولى:
ملاحظات أو قرارات معلقة:
حالة الاعتماد:
```

### 25.2 `02_CLIENT_PROPOSAL.md`

```text
# عرض مشروع

مقدمة:
فهمنا لاحتياج العميل:
نطاق العمل المقترح:
المخرجات المتوقعة:
مراحل العمل:
ما هو خارج النطاق:
الافتراضات:
المتطلبات من العميل:
آلية الاعتماد والتغييرات:
حالة الاعتماد:
```

### 25.3 `03_SCOPE_OF_WORK.md`

```text
# نطاق العمل

داخل النطاق:
- ...

خارج النطاق:
- ...

مؤجل لمرحلة لاحقة:
- ...

قيود مهمة:
افتراضات:
قرارات معلقة:
حالة الاعتماد:
```

### 25.4 `04_FEATURE_SCOPE_MATRIX.md`

```text
# مصفوفة نطاق الميزات

| الميزة | التصنيف | الأولوية | الملاحظات |
|---|---|---|---|
|  | داخل النطاق / مؤجل / خارج النطاق / يحتاج قرار | عالية / متوسطة / منخفضة |  |

حالة الاعتماد:
```

### 25.5 `05_USER_FLOWS.md`

```text
# مسارات الاستخدام

## المسار الأول: [الاسم]

المستخدم:
الهدف:
الخطوات:
1. ...

الحالات البديلة:
النتيجة المتوقعة:

حالة الاعتماد:
```

### 25.6 `06_SCREEN_MAP.md`

```text
# خريطة الشاشات

| الشاشة | الغرض | المستخدمون | ملاحظات |
|---|---|---|---|
|  |  |  |  |

شاشات غير مطلوبة في النسخة الأولى:
حالة الاعتماد:
```

### 25.7 `07_DESIGN_DIRECTION.md`

```text
# التوجه البصري

اللغة والأسلوب العام:
الألوان أو الهوية:
مراجع تعجب العميل:
مراجع لا تعجب العميل:
انطباع التصميم المطلوب: رسمي / حديث / بسيط / فاخر / شبابي / آخر
قيود التصميم:
ما لا يجب استخدامه:
حالة الاعتماد:
```

### 25.8 `08_PROTOTYPE_PLAN.md`

```text
# خطة البروتوتايب

هل البروتوتايب مطلوب؟ نعم / لا
سبب القرار:
الشاشات أو التدفقات التي يجب تمثيلها:
مستوى التفصيل: منخفض / متوسط / عالي
الأداة المقترحة إن وجدت:
معايير قبول البروتوتايب:
حالة الاعتماد:
```

### 25.9 `09_ACCEPTANCE_CRITERIA.md`

```text
# معايير القبول

| الميزة أو الشاشة | معيار القبول | طريقة التحقق | الحالة |
|---|---|---|---|
|  |  |  | مقبول / يحتاج تعديل / معلق |

معايير قبول عامة:
معايير لا تعتبر ضمن التسليم الحالي:
حالة الاعتماد:
```

### 25.10 `11_CHANGE_CONTROL.md`

```text
# سجل طلبات التغيير

| Change ID | التاريخ | مقدم الطلب | الملخص | التصنيف | القرار | حالة الاعتماد |
|---|---|---|---|---|---|---|
| CHG-001 |  |  |  | Clarification / Minor Adjustment / Enhancement / New Scope / Phase 2 / Rejected |  |  |

ملاحظات عامة:
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
- [ ] Plan approved → Proceed to Phase 4
- [ ] Plan rejected → Revise and resubmit
- [ ] Plan blocked → Reason: ...
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
| **Generate** | Agent does not exist — create from `AGENT_GENERATION_TEMPLATE.md` |
| **Use Existing** | Agent exists and is suitable — use directly |
| **Specialize** | Agent exists but is too generic — narrow sources/targets/constraints |

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
- [ ] Plan approved → Proceed to agent generation
- [ ] Plan rejected → Revise and resubmit
- [ ] Plan blocked → Reason: ...
```

---

## 29. Project Master Plan (Phase 5 — Output)

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
| 1 | [e.g. Technical Foundation] | [e.g. Scaffold project, init ORM, verify startup] | Core MVP / Foundation | — | Planned |
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
- [ ] Approved → Ready for Detailed Planning
- [ ] Needs revision
```

---

## 30. Detailed Execution Plan (Phase 5 — Output)

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
| P1-01 | [e.g. Scaffold Next.js project] | TASK-COD-001 | — | Planned | |
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
- [ ] Approved → Ready for batch planning
- [ ] Needs revision
```

---

## 31. Execution Batch Plan (Phase 5 — Output)

This template defines the current approved batch only — not the full project.
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
- [ ] Approved → Begin execution (Phase 6)
- [ ] Rejected → Revise
- [ ] Blocked → Reason: ...

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

## 33. UI/UX Guidelines Template (Design Governance Output)

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
| Phase | 7 — Delivery, Handover & Closure |
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

## vX.Y - YYYY-MM-DD

- Release Name:
- Release Type: Initial / Hotfix / Patch / Minor / Major
- Status: Draft / Released / Blocked
- Git Tag:
- Commit Hash:
- GitHub Release URL:
- GitHub Release Status: Draft / Published / Deferred / N/A
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

## Verification

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
| Closure Type | Version Closure / Maintenance Closure / Hotfix Closure / Final Application Closure |
| Closed Version |  |
| Git Tag |  |
| Commit Hash |  |
| GitHub Release URL |  |
| GitHub Release Status | Published / Deferred / N/A |
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
| Version Registry | Complete / N/A |  |
| Next Version Handoff | Complete / N/A |  |
| Git Commit / Push | Complete / Deferred / N/A |  |
| Git Release Tag | Complete / Deferred / N/A |  |
| GitHub Release | Complete / Deferred / N/A |  |
| Client Handover Package | Complete / N/A |  |

## 4. Open Issues / Deferred Items

| Item | Status | Recommended Action |
|---|---|---|
|  | Deferred / Closed / Won't Fix |  |

## 5. Final Decision

Project Closure Status: VERSION_CLOSED / MAINTENANCE_CLOSED / HOTFIX_CLOSED / FINAL_APPLICATION_CLOSED / BLOCKED / NEEDS_PHASE_6_FIX

## 6. Next Version / Maintenance Decision

- Recommended Next Version:
- Recommended Cycle: Hotfix / Patch / Minor / Major / Final Closure / None
- NEXT_VERSION_HANDOFF.md Updated: Yes / No / N/A
- Git Tag Created / Pushed: Yes / No / Deferred / N/A
- GitHub Release Created: Yes / No / Deferred / N/A
```

### 34.6 Client Handover Package

Generated file for external client projects:

```text
clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/delivery/CLIENT_HANDOVER_PACKAGE.md
```

```markdown
# CLIENT_HANDOVER_PACKAGE.md

## 1. تسليم المشروع

- اسم العميل:
- اسم التطبيق:
- تاريخ التسليم:
- نسخة التسليم:

## 2. ما تم تسليمه

- ...

## 3. طريقة التشغيل / الوصول

- لا تكتب أسرارًا أو كلمات مرور.
- أشر فقط إلى أن البيانات الحساسة محفوظة كـ local environment secrets أو عبر قناة آمنة.

## 4. العناصر المؤجلة أو غير المشمولة

- ...

## 5. القبول النهائي

- Accepted / Needs Fix / Deferred
- اسم صاحب الاعتماد:
- التاريخ:
```

## 35. Version Management Layer Templates

Use `tera-system/runtime/VERSION_LIFECYCLE_PROTOCOL.md` as the source of truth.

### 35.1 Version Registry

Generated file: `project-control/VERSION_REGISTRY.md`

```markdown
# VERSION_REGISTRY.md

## Current Version

- Active Version:
- Status: Planned / In Development / Released / Maintenance / Closed
- Release Type: Initial / Hotfix / Patch / Minor / Major
- Release Date:
- Support Status: Not Started / Active / Maintenance Only / Deprecated / Ended
- Git Tag:
- Commit Hash:
- GitHub Release URL:
- GitHub Release Status: Published / Deferred / N/A

## Version History

| Version | Type | Status | Start Date | Release Date | Git Tag | Commit Hash | GitHub Release | Scope Summary | Support Status |
|---|---|---|---|---|---|---|---|---|---|
|  |  |  |  |  |  |  | Published / Deferred / N/A |  |  |

## Deferred Features by Version

| Feature / Item | Target Version | Source | Status | Notes |
|---|---|---|---|---|
|  |  |  |  |  |

## Maintenance / Hotfix History

| Version | Affected Version | Type | Status | Summary | Linked Task(s) | Release Notes Updated |
|---|---|---|---|---|---|---|
|  |  | Hotfix / Patch |  |  |  | Yes / No / N/A |

## Git Release Tags

| Version | Git Tag | Commit Hash | Push Status | Tag Push Status | GitHub Release Status | GitHub Release URL | Approved By | Notes |
|---|---|---|---|---|---|---|---|---|
|  |  |  | Pending / Pushed / Deferred | Pending / Pushed / Deferred | Published / Deferred / N/A |  |  |  |
```

### 35.2 Next Version Handoff

Generated file: `project-control/NEXT_VERSION_HANDOFF.md`

```markdown
# NEXT_VERSION_HANDOFF.md

## Closed Version

- Version:
- Closure Type: Version Closure / Maintenance Closure / Hotfix Closure / Final Application Closure
- Status: Released / Closed / Blocked / Needs Phase 6 Fix
- Release Date:
- Approved By:
- Git Tag:
- Commit Hash:
- GitHub Release URL:

## Deferred Items

| Item | Suggested Version | Reason | Source |
|---|---|---|---|
|  |  |  |  |

## Known Issues

| Issue | Severity | Recommended Action | Suggested Cycle |
|---|---|---|---|
|  | Critical / High / Medium / Low |  | Hotfix / Patch / Minor / Major / Won't Fix |

## Recommended Next Version

- Suggested Version:
- Suggested Release Type: Hotfix / Patch / Minor / Major / None
- Reason:
- Readiness: Ready / Needs Discovery / Needs Client Approval / Deferred

## Level 3 Expansion Note

Per-version folders and version slash commands remain deferred until large-project, frequent-release, formal client approval, or parallel-version triggers exist.
```

### 35.3 Version Decision

```markdown
# VERSION_DECISION.md

## Version Metadata

- Target Version:
- Release Type: Minor / Major
- Source Version:
- Decision Date:
- Approved By:

## Scope

### Included
-

### Deferred
-

### Out of Scope
-

## Required Planning Path

- Phase 2 update: Required / Not Required
- Phase 3: Required / Compact / Not Required
- Phase 4: Required / Existing agents only / Not Required
- Phase 5: Required
- Phase 6: Required
- Phase 7: Required
```

### 35.4 Hotfix Decision

```markdown
# HOTFIX_DECISION.md

## Hotfix Metadata

- Hotfix Version:
- Affected Version:
- Severity:
- Decision Date:
- Approved By:

## Issue

- Description:
- Impact:
- Affected Files / Areas:

## Scope Guard

- New features included? No
- Risk Level:
- Required Verification:
```

---

## 36. Implementation Agent Strategy (Phase 5.1 Output)

This template is used for the mandatory Phase 5.1 output (Implementation Agent Strategy Gate).
The generated file is saved to `project-control/IMPLEMENTATION_AGENT_STRATEGY.md`.

Reference: `TeraAgent.md` Section 5.1

> **Rules:**
> - No Build Mode, no Phase 6, and no `TASK-COD-*` delegation before this strategy is approved.
> - This strategy must answer all 6 mandatory questions (Sections 2–7 below).
> - The 6 questions are screens against generic planning. Each question must have a clear answer.

```markdown
# IMPLEMENTATION_AGENT_STRATEGY.md

## [Application Name] — استراتيجية العملاء للتنفيذ

| Metadata | |
|----------|-|
| **Phase** | 5.1 — Implementation Agent Strategy Gate |
| **Status** | Draft / Approved — Option A / Approved — Option B / Needs Revision |
| **Required Before** | Phase 6 / Build Mode / Any `TASK-COD-*` delegation |
| **Created By** | Tera Agent |
| **Date** | YYYY-MM-DD |
| **System Rule** | `TeraAgent.md` Section 5.1 |

---

## 1. Executive Decision

[Summarize the core decision: single general agent vs. multiple specialized agents, and why.]

---

## 2. السؤال الإلزامي 1 — Agent: من نحتاج الآن؟

| Agent | القرار | الدفعات/المراحل | السبب | الصلاحية |
|-------|--------|-----------------|-------|----------|
| [Agent name] | Required Now / Required Next / Optional | [B1, B2, etc.] | [Why this agent is needed] | [Permission level] |

### حالة العملاء الموجودين سابقاً

| Existing Agent | الحالة | القرار |
|----------------|--------|--------|
| [name] | Generated + Activated / Generated only | Keep / Specialize / Disable / Do not use |

---

## 3. السؤال الإلزامي 2 — Agent: من نؤجل؟

| Agent | يؤجل إلى | سبب التأجيل |
|-------|----------|-------------|
| [name] | [phase/batch] | [reason] |

---

## 4. السؤال الإلزامي 3 — Who writes: من ينفذ؟

| TASK-ID | الدفعة | التنفيذ المقترح | ملاحظات |
|---------|-------|----------------|---------|
| TASK-COD-XXX | [batch] | [Agent name] | [scope notes] |

**قاعدة الكاتب الأساسي:** كل `TASK-COD-*` يجب أن يحدد Writer واحداً فقط. إذا احتاجت المهمة أكثر من اختصاص، يجب تقسيمها أو تحديد Agent واحد ككاتب والآخر كمراجع.

---

## 5. السؤال الإلزامي 4 — Who reviews: من يراجع؟

| نوع المهمة | المراجع الأساسي | مراجعة مستقلة مطلوبة؟ | السبب |
|------------|----------------|------------------------|-------|
| [task type] | [reviewer] | Yes / No | [reason] |

---

## 6. السؤال الإلزامي 5 — Activation plan: متى يُفعّل كل عميل وبأي صلاحيات؟

| Agent | تفعيل | الصلاحية | Allowed Write Targets الافتراضية | ملاحظات |
|-------|-------|----------|----------------------------------|---------|
| [name] | now / before TASK-XXX | [permission level] | [paths] | [notes] |

---

## 7. السؤال الإلزامي 6 — Exceptions: هل يوجد استثناء للتنفيذ المباشر؟

| الحالة | القرار |
|--------|--------|
| هل ينفذ Tera كود التطبيق مباشرة؟ | No / Yes with documented reason |
| هل توجد استثناءات حالية؟ | [list or None] |
| ما الذي ينفذه Tera مباشرة؟ | Control files, task files, logs, strategy, reviews only |

---

## 8. توصية إنشاء/تعديل العملاء

### الإجراء المقترح بعد اعتماد هذه الاستراتيجية

1. ...
2. ...

**توصية Tera:** [Option A / Option B / Custom]

---

## 9. Current Batch Decision

| Field | Decision |
|-------|----------|
| Batch | [e.g., B1] |
| TASK-ID | TASK-COD-XXX |
| Required Agent | [Agent name] |
| Reviewer | [reviewer] |
| SecurityAgent | Required / Not required for this batch |
| QAAgent | Required / Not required for this batch |
| ProjectControlAgent | Required / Not required / Tera handles |
| Build Mode | Remains blocked until this strategy is approved |

---

## 10. Approval

| Role | Status |
|------|--------|
| Prepared by | Tera Agent |
| Reviewed by | Tera Agent |
| Approved by | [User / Client] — [Option selected] |

### Approval Options

| Option | Meaning |
|--------|---------|
| **Option A** | Use existing agent(s) after narrowing scope |
| **Option B** | Create new dedicated agent(s), disable generic ones |
| **Request Changes** | Modify agent split, timing, or review responsibilities |

---

## 11. Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| v1 | YYYY-MM-DD | Tera | Initial Implementation Agent Strategy |
```

---

## 37. Generated Agents Manifest

This template is used every time sub-agents are generated for a project.
The generated file is saved to `generated-agents/opencode/GENERATED_AGENTS_MANIFEST.md`.

Reference: `TERA_RUNTIME_PROTOCOLS.md` Section 2 (Generated Sub-Agent Lifecycle Protocol)

```markdown
# GENERATED_AGENTS_MANIFEST.md

Project: [Project Name]
Runtime Environment: OpenCode
Generated Date: YYYY-MM-DD
Generated By: Tera Agent

## Agents Generated

- Agent: [Agent Name]
  - Reason: [Why this agent was generated]
  - File: `generated-agents/opencode/[agent-file-name].md`
  - Active Copy: `.opencode/agents/[agent-file-name].md`
  - Category: [Specialized Implementation Agent / Basic Implementation Agent / Research Agent / Review Agent / Control Agent / Domain Agent]
  - Status: Active / Disabled / Awaiting Activation
  - Allowed Write Targets: [paths]
  - Notes: [any restrictions or scope notes]

[repeat for each generated agent]

## Agents Not Generated

- [Agent Name]: [Reason why not needed now or ever]
- ...

## Notes

- [General notes about agent usage, restrictions, and sequencing]
- [List of currently active implementation agents]
- Agents must not execute without a task file containing `Pre-Execution Gate Result: PASS`.
- OpenCode must be restarted after activation for the agent changes to be available in the running session.
```

---

## 38. Agent Deactivation Record

This template is used when deactivating a sub-agent that was previously activated in `.opencode/agents/`.
The generated file is saved to `project-control/AGENT_DEACTIVATION_RECORD.md` when a formal record is needed, or recorded in `GENERATED_AGENTS_MANIFEST.md` for routine deactivations.

Reference: `TERA_RUNTIME_PROTOCOLS.md` Section 19 (Agent Deactivation Protocol)

```markdown
# AGENT_DEACTIVATION_RECORD.md

## Deactivation Metadata

| Field | Value |
|-------|-------|
| Agent Name | [name] |
| Agent File | `generated-agents/opencode/[file].md` |
| Active Copy | `.opencode/agents/[file].md` |
| Deactivation Date | YYYY-MM-DD |
| Deactivated By | Tera Agent |
| Reason | [reason] |
| Replacement Agent (if any) | [name or None] |

## Impact Assessment

- OpenCode restart needed to apply changes: Yes / No
- Remaining active agents covering this scope: [list or None]
- Generated draft kept for future use: Yes / No

## Cleanup Actions

- [ ] `disable: true` added to active copy frontmatter.
- [ ] Manifest updated in `GENERATED_AGENTS_MANIFEST.md`.
- [ ] Active copy removed from `.opencode/agents/` (optional — disabled is safer).
- [ ] PROJECT_STATE.md updated.
- [ ] PROJECT_ACTIVITY_LOG.md updated.
- [ ] User notified of deactivation.

## Approval

- Deactivation approved by: [Tera / User]
- Notes:
```

---

## 39. Task Cross-Verification Report

This template is used when cross-verifying consistency between preparation files before execution.
The generated file is saved to `project-control/tasks/TASK-CROSS-XXX.md` or recorded inline in the task file.

Reference: `TERA_RUNTIME_CHECKLISTS.md` Section 20 (Cross-Verification Checklist)

```markdown
# TASK-CROSS-XXX: Cross-Verification Report

## Scope

- Files verified:
  - [file path 1]
  - [file path 2]
  - ...

## Verification Items

| Item | Count in File A | Count in File B | Match? |
|------|----------------|-----------------|--------|
| [e.g., Models] | [X] | [Y] | Yes / No — corrected |
| [e.g., Screens] | [X] | [Y] | Yes / No — corrected |
| [e.g., API endpoints] | [X] | [Y] | Yes / No — corrected |

## Files Corrected

| File | Before | After | Change |
|------|--------|-------|--------|
| [path] | [old value] | [new value] | [description] |

## Result

- [ ] All verified — no inconsistencies found
- [ ] Inconsistencies found and corrected
- [ ] Inconsistencies found — needs Tera/user decision before execution
```

---

## 40. Workspace Governance Model

This template is used during Project Intake Gate (Phase 1) to define the governance model for a new application workspace.
The generated file is saved to `project-control/WORKSPACE_GOVERNANCE_MODEL.md`.

Reference: `TeraAgent.md` Section 39 (Governance Session Agents) and Section 2.3 (Project Intake Gate).

**Rule:** No new project workspace is complete without this file.

```markdown
# WORKSPACE_GOVERNANCE_MODEL.md

## [Application Name] — نموذج حوكمة مساحة العمل

| Metadata | |
|----------|-|
| **Created** | YYYY-MM-DD |
| **Created By** | Tera Agent |
| **Template** | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 40 |
| **Project** | [Application Name] |
| **Active Workspace** | `clients/CLIENT-[name]/applications/APP-[name]/` |

---

## 1. Governance Session Agents

هذا المشروع يخضع لنموذج جلسات الحوكمة المستقلة التالية:

| Agent | المعرف | يقرأ | ينتج | ملاحظات خاصة بالمشروع |
|-------|--------|------|------|----------------------|
| **Auditor** | `AUDITOR_AGENT` | `PROJECT_STATE.md`, `PROJECT_ACTIVITY_LOG.md`, `TASK_REGISTRY.md`, ملفات المهام | Quality Review Report, Git commit محلي بعد قبول المالك | [ملاحظات إن وجدت] |
| **Monitor** | `MONITOR_AGENT` | `PROJECT_MASTER_PLAN.md`, `PROJECT_DETAILED_EXECUTION_PLAN.md`, `EXECUTION_BATCH_PLAN.md`, `TASK_REGISTRY.md` | Plan Compliance Report | [ملاحظات إن وجدت] |
| **Design Reviewer** | `DESIGN_REVIEWER_AGENT` | `28_UI_UX_GUIDELINES.md`, `07_SCREENS_AND_UI_STRUCTURE.md`, `design-source/`, ملفات UI | Design Review Report | [ملاحظات إن وجدت] |

## 2. Governance Rules for this Project

### 2.1 Independence

- Auditor, Monitor, and Design Reviewer are **independent sessions**, not sub-agents under Tera.
- They are **parallel to Tera**, not below or above.
- Tera does not control when they run or what they review.
- The owner (Majed) decides when to open each governance session.

### 2.2 Reporting Line

- Governance agents report directly to the **owner (Majed)**.
- Reports are NOT submitted to Tera. Tera receives requests for correction or approval **via the owner**.
- Governance agents do not communicate directly with Tera's sub-agents.

### 2.3 Final Authority

- **Final review and acceptance authority belongs to the owner (Majed).**
- Tera reviews sub-agent output, but the final project acceptance is the owner's decision.
- Governance agents are advisory and oversight layers only — they cannot approve scope, code, or phase transitions.

### 2.4 Permissions

| Agent | Default Permission | Can Be Raised To | Notes |
|-------|-------------------|------------------|-------|
| Auditor | `READ_ONLY` + `bash: ask` (local commit after owner approval) | `WRITE_CONTROL` for specific report | No push, no code edit, no commit before owner approval |
| Monitor | `READ_ONLY` | `WRITE_DOCS` for report delivery | Reviews plan compliance, does not correct execution |
| Design Reviewer | `READ_ONLY` | `RUN_TESTS` for visual inspection after owner approval | Does not design, does not implement UI |

### 2.5 Boundaries

- Governance agents do not replace Pre-Execution Gate or Post-Execution Review Gate.
- Governance agents do not execute code, change scope, or modify plans.
- Governance agents do not modify colors, components, or UI implementation.
- Governance agents do not push to remote or force changes.
- Tera and its sub-agents continue normal operation regardless of governance session activity.

## 3. Communication Protocol

```text
Owner opens session
  → Agent reads project files
  → Agent produces report
  → Agent returns to owner
  → Owner reviews and decides
  → If correction needed: owner requests from Tera
  → Tera handles correction via sub-agent task
```

## 4. Project-Specific Governance Overrides

[Any project-specific rules that override the general governance model. Leave empty if none.]

- ...

## 5. Approval

| Role | Status |
|------|--------|
| Created by | Tera Agent |
| Governance model applies | [Project Name] |
| Date | YYYY-MM-DD |
```
