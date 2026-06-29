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
