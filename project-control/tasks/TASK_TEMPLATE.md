# TASK-[ID]: [Task Title]

## 1. Task Information

| Field | Value |
|---|---|
| **TASK-ID** | TASK- |
| **Task Type** | Preparation / Coding / Review / Control |
| **Phase** | |
| **Status** | Draft |
| **Assigned To** | |
| **Created** | YYYY-MM-DD |
| **Linked Plan Item** | PROJECT_DETAILED_EXECUTION_PLAN item / N/A |
| **Linked Batch** | EXECUTION_BATCH_PLAN batch / N/A |
| **Active Technology Profile** | Profile name / N/A |
| **Design Source Decision** | Tera-Decided / User-Provided / Existing Brand / External Reference / No UI / N/A |

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

## 6.1 Execution Gates

| Gate | Result | Notes |
|---|---|---|
| Orchestration Decision Matrix | _Direct / Helper Agent / Multi-Agent / Blocked_ | |
| Model Capability Gate | _Current model sufficient / Safeguards / Stronger recommended / Stronger required_ | |
| Pre-Execution Gate | _PASS / NEEDS_REVISION / BLOCKED_ | |

## 6.2 CLI / Tool Side Effects

| Command / Tool | Allowed? | Expected Side Effects | Approval Needed? |
|---|---|---|---|
|  | _Yes / No / N/A_ |  | _Yes / No_ |

## 7. TASK-ID Size Check

```md
Requested Work:
Can it fit one TASK-ID? Yes/No
Reason:
Proposed Split:
- TASK-XXXX:
- TASK-XXXX:
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
