# TASK-[ID]: [Task Title]

## 1. Task Information

| Field | Value |
|---|---|
| **TASK-ID** | TASK- |
| **Task Type** | Preparation / Coding / Review / Control |
| **Phase** | |
| **Build Mode Approved** | Yes / No / N/A (preparation tasks) |
| **Status** | Draft |
| **Assigned To** | |
| **Created** | YYYY-MM-DD |
| **Linked Plan Item** | PROJECT_DETAILED_EXECUTION_PLAN item / N/A |
| **Linked Batch** | EXECUTION_BATCH_PLAN batch / N/A |
| **Active Technology Profile** | Profile name / N/A |
| **Target Version** | v1.0 / v1.1 / v2.0 / v1.0.1 / N/A |
| **Release Type** | Initial / Hotfix / Patch / Minor / Major / N/A |
| **Version Scope** | In Scope / Deferred / Out of Scope / N/A |
| **Version Registry Updated** | Yes / No / N/A |
| **Release Notes Required** | Yes / No / N/A |
| **Design Source Decision** | INTERNAL_TERA_KIT / GETDESIGN_MD / FIGMA_DESIGN_FILE / USER_PROVIDED_REFERENCE / EXTERNAL_URL_ANALYSIS / HYBRID / NO_UI / N/A |
| **UI Acceptance Gate Required** | Yes / No / N/A |

## 2. Objective

_What is the goal of this task?_

## 3. Reference Files

- 
- 

## 4. Allowed Write Targets

- 

## 5. Forbidden Files / Actions

- 

## 6. Acceptance Criteria

1. 
2. 
3. 

## 6.1 Technical Specification (SoftwareDesignerAgent)

| Field | Value |
|---|---|
| Review Required | **Yes — Mandatory** (SoftwareDesignerAgent for every task) |
| Review Agent | SoftwareDesignerAgent |
| Technical Specification File | `[active application workspace]/project-control/task-engineering-reviews/[TASK-ID]_TECHNICAL_SPECIFICATION.md` |
| Task Engineering Review Decision | APPROVED_FOR_GATE / REVISION_REQUIRED / SPLIT_REQUIRED / BLOCKED_BY_MISSING_DECISION / WRONG_AGENT / NEEDS_PRE_REVIEW / REJECTED_OUT_OF_SCOPE / N/A |
| Risk Level | Low / Medium / High / Critical |
| Approved for Pre-Execution Gate | Yes / No |
| Design Gaps | List any gaps if preparation files were insufficient |

## 6.2 Execution Gates

| Gate | Result | Notes |
|---|---|---|
| Orchestration Decision Matrix | _Direct / Helper Agent / Multi-Agent / Blocked_ | |
| Model Capability Gate | _Current model sufficient / Safeguards / Stronger recommended / Stronger required_ | |
| Pre-Execution Gate | _PASS / NEEDS_REVISION / BLOCKED_ | |

## 6.2.1 Scoped Runtime Override

| Field | Value |
|---|---|
| Runtime Override Used? | Yes / No |
| Override Type | None / Narrow Targets / Expand Targets (In Scope) / Reduce Context / Escalate Review / Freeze Current Agent Path / Reassign Writer |
| Reason | |
| Scope Still Approved? | Yes / No |
| Extra Approval Needed? | Yes / No |
| Logged In | Task File / PROJECT_ACTIVITY_LOG / SUB_AGENT_STATUS / N/A |

## 6.3 CLI / Tool Side Effects

| Command / Tool | Allowed? | Expected Side Effects | Approval Needed? |
|---|---|---|---|
|  | _Yes / No / N/A_ |  | _Yes / No_ |

## 6.4 UI / Frontend Requirements

Required for any UI, Frontend, layout, style, or component task.

| Item | Value |
|---|---|
| UI Source | `28_UI_UX_GUIDELINES.md` / DESIGN.md reference / FIGMA_DESIGN_FILE / Internal Kit / No UI |
| UI Rules | Tokens / Layout / Component Rules / RTL-LTR / Accessibility |
| UI Acceptance | `tera-system/design-system/UI_ACCEPTANCE_GATE.md` required? Yes / No |
| Design Gap Handling | Raise Design Gap / Ask Tera / N/A |

## 7. TASK-ID Size Check

```md
Requested Work:
Can it fit one TASK-ID? Yes/No
Reason:
Proposed Split:
- TASK-XXXX:
- TASK-XXXX:
```

## 7.1 Version Scope Check

```md
Target Version:
Release Type:
Is this work allowed in the target version? Yes/No
If Hotfix: Does it include any new feature? Yes/No/N/A
VERSION_REGISTRY.md update required? Yes/No
RELEASE_NOTES.md update required? Yes/No
```

## 8. Sub-Agent Output Review

| Item | Result |
|---|---|
| Output is actionable | _Yes / No_ |
| Files reviewed or modified are listed | _Yes / No_ |
| Completed work is explicit | _Yes / No_ |
| Constraints or risks are stated | _Yes / No / N/A_ |
| Maps to acceptance criteria | _Yes / No_ |
| Stayed within TASK-ID scope | _Yes / No_ |
| Acceptance Decision | _Accept / Reject / Needs Fix_ |
| Rejection Reasons | |

## 9. Execution Report / Agent Handback

```text
Task ID:
Agent:
Status: Done / Blocked / Needs Clarification / Rework Needed
Files Created:
Files Modified:
Commands Run:
Summary:
Assumptions:
Issues or Missing Information:
Decisions Needed from Tera:
Recommendation:
```

## 10. Tera Review

> Use `TERA_RUNTIME_TEMPLATES.md` Section 32 as the official Post-Execution Review template.

| Check | Result | Notes |
|---|---|---|
| TASK objective completed? | PASS / FAIL | |
| Output matches approved scope? | PASS / FAIL | |
| No files outside Allowed Write Targets? | PASS / FAIL | |
| No forbidden files created? | PASS / FAIL | |
| No extra libraries added? | PASS / FAIL | |
| No secrets or real `.env`? | PASS / FAIL | |
| Technology Profile respected? | PASS / FAIL | |
| UI/UX rules respected if UI exists? | PASS / FAIL / N/A | |
| UI Acceptance Gate passed if UI exists? | PASS / FAIL / N/A | |
| Acceptance Criteria passed? | PASS / FAIL | |
| Rollback needed? | Yes / No | |

## 11. Notes

## 12. Post-Execution Review Result

| Item | Status |
|---|---|
| Gate Result | _PASS / NEEDS_FIX / BLOCKED_ |
| Reviewer | |
| Review Date | |
| Notes | |

## 13. Final Tera Decision

| Item | Value |
|---|---|
| Final Status | Accepted / Needs Fix / Blocked / Rework Needed / Deferred / Cancelled |
| Registry Updated | Yes / No |
| Activity Log Updated | Yes / No |
| Project State Updated | Yes / No / N/A |
| Issues/Gaps Updated | Yes / No / N/A |
| Next Action | |
