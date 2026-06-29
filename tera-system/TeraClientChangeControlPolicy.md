# Tera Client Change Control Policy

## 1. Purpose

This policy defines how Tera handles client requests after scope, design direction, prototype, or execution authorization has been approved.

## 2. Core Rule

```text
No undocumented change requests during execution.
```

Any new client request after approval must be classified and recorded before it changes work.

## 3. Change Log Location

Record client changes in:

```text
clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/client-approval/11_CHANGE_CONTROL.md
```

If a change affects execution tasks, also record the related decision or issue in `project-control/`.

## 4. Change Types

Classify every request as one of:

| Type | Meaning | Default action |
|---|---|---|
| Clarification | Explains an existing approved item without changing scope | Document and continue |
| Minor Adjustment | Small change inside approved scope | Document and allow if low risk |
| Enhancement | Improves an approved feature but adds work | Ask user before adding |
| New Scope | Adds a new feature, screen, workflow, integration, or data area | Requires explicit approval |
| Phase 2 | Valuable but not needed for current delivery | Defer |
| Rejected | Conflicts with project goal, approved scope, or constraints | Do not implement |

## 5. Change Record Format

Each change record must include:

- change ID
- date
- requester
- request summary
- affected approved file or gate
- classification
- scope impact
- design impact
- technical impact
- time/cost impact if known
- decision: approve / defer / reject / needs client decision
- approval authority
- related task or issue if any

## 6. Implementation Restriction

Tera must not implement a change classified as `Enhancement` or `New Scope` until Majed confirms the required client approval and the approval is documented.

For `New Scope`, approval from the documented client approval authority is required unless Majed explicitly records that the item is deferred to a later phase and will not affect the current build.

## 7. Final Rule

```text
Approved project scope can only change through an approved change record.
```
