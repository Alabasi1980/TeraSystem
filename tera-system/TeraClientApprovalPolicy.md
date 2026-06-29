# Tera Client Approval Policy

## 1. Purpose

This policy defines the mandatory client approval package required before implementation starts for external client projects.

## 2. Mandatory Rule

```text
No Client Approval Package = No Implementation
No Approved Scope = No Build Mode
No Approved Design Direction = No Final UI Implementation
No Approved Change Request = No Scope Expansion
```

## 3. Client Approval Package Location

For each client application, create approval files under:

```text
clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/client-approval/
```

## 4. Required Approval Files

The default mandatory package is:

- `01_CLIENT_PROJECT_BRIEF.md`
- `02_CLIENT_PROPOSAL.md`
- `03_SCOPE_OF_WORK.md`
- `04_FEATURE_SCOPE_MATRIX.md`
- `05_USER_FLOWS.md`
- `06_SCREEN_MAP.md`
- `07_DESIGN_DIRECTION.md`
- `08_PROTOTYPE_PLAN.md`
- `09_ACCEPTANCE_CRITERIA.md`
- `10_CLIENT_APPROVAL_RECORD.md`
- `11_CHANGE_CONTROL.md`

If a file is not applicable to a very small project, Tera must still create an explicit section in `10_CLIENT_APPROVAL_RECORD.md` explaining why it is not applicable and what replaced it.

## 5. Approval Gates

Tera must track the following gates:

| Gate | Required approval |
|---|---|
| Gate 1: Idea Approval | The client confirms the project idea is understood correctly. |
| Gate 2: Scope Approval | The client confirms what is in scope, out of scope, and deferred. |
| Gate 3: Flow Approval | The client confirms the main user and business flows. |
| Gate 4: Screen Approval | The client confirms the screen list and screen purposes. |
| Gate 5: Design Direction Approval | The client confirms visual direction, tone, and references. |
| Gate 6: Prototype Approval | The client confirms the prototype or prototype plan when applicable. |
| Gate 7: Execution Authorization | The client authorizes moving into implementation. |

## 6. Approval Record

`10_CLIENT_APPROVAL_RECORD.md` must document:

- client name
- application name
- approval date or pending status
- approving contact
- approval authority evidence or user confirmation
- approved files
- pending decisions
- rejected or deferred items
- execution authorization status

## 7. Client-Facing Language

Default client-facing language is Arabic.

Client-facing documents must be written in Arabic unless Majed explicitly requests another language.

## 8. Relationship to Internal Implementation

Implementation tasks must trace back to approved scope and acceptance criteria.

Tera must not create implementation `TASK-ID`s for client work before Gate 7 passes.

If Majed explicitly authorizes a pre-approval technical spike, it must be treated as non-deliverable research only, must not modify final application code, must not become client scope, and must be documented separately from Build Mode.

## 9. Final Rule

```text
Client approval must be visible in files, not only in chat.
```
