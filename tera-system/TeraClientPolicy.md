# Tera Client Policy

> Merged from: `TeraClientEngagementPolicy.md`, `TeraClientApprovalPolicy.md`, `TeraClientChangeControlPolicy.md`, `TeraClientFacingContentPolicy.md` (v1.0)

## 1. Purpose

This policy defines how Tera handles external client projects — from discovery and engagement through approval, change control, and client-facing documentation.

Tera must treat client work as a documented relationship, not only a technical build request.

---

## 2. Core Rules

```text
Spoken client input is not final until documented.
No documented client context = No client project preparation.
No documented approval authority = No final client approval.
No Client Approval Package = No Implementation.
No Approved Scope = No Build Mode.
No Approved Design Direction = No Final UI Implementation.
No Approved Change Request = No Scope Expansion.
No Client Handover Package = No client project closure.
Client approval must be visible in files, not only in chat.
```

---

## 3. Client Workspace Structure

Client-facing and client-management files belong under:

```text
clients/
```

Required structure:

```text
clients/
  CLIENT-[client-name-or-id]/
    CLIENT_PROFILE.md
    CONTACTS.md
    applications/
      APP-[app-name-or-id]/
        client-approval/
        client-assets/
        client-communications/
        project-inputs/
        project-preparation/
        project-control/
        generated-agents/opencode/
        app-source/
        delivery/
```

Rules:

- Each client must have a dedicated folder.
- Each client application must have a dedicated application folder.
- The application folder is the canonical isolated workspace for that application.
- Application-specific intake, preparation, control records, generated agents, source code, approval material, assets, communications, and delivery records must stay inside the application folder.
- Removing or exporting `clients/CLIENT-*/applications/APP-*/` should remove or export that application without polluting the Tera system root.
- Do not mix client-facing approval material with internal `project-preparation/` files.
- Do not store secrets, passwords, access tokens, or private credentials in client files.
- Final client handover material belongs in `clients/CLIENT-*/applications/APP-*/delivery/`.

---

## 4. Required Client Profile

```text
CLIENT_PROFILE.md
```

Minimum content:

- client name
- client type: individual / company / organization
- business domain
- default client-facing language: Arabic unless the user decides otherwise
- technical familiarity: low / medium / high / unknown
- decision style and communication notes
- project sensitivity: low / medium / high / critical
- preferred approval method
- general relationship notes

---

## 5. Required Contacts File

```text
CONTACTS.md
```

Minimum content per contact:

- name
- role at client side
- decision authority: decision maker / reviewer / technical contact / finance / other
- phone number if available
- email if available
- preferred communication channel
- approval authority: yes / no / unknown
- communication notes

Tera must not treat approval from a contact as final unless that contact has documented approval authority or the user explicitly confirms it.

### Client Discovery Output — Application Proposal

After the Client Discovery + Smart Interview process completes (see `TERA_RUNTIME_PROTOCOLS.md` Section 18), Tera generates a **professional client-facing Application Proposal** as an HTML page using `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html`.

The proposal captures: understanding, users & roles, scope (MVP + out-of-scope), requirements by domain, assumptions, and proposed roadmap. Generated from `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html`.

The proposal is saved under `clients/.../client-approval/` and **must be approved by the client** before formal preparation begins. The approved proposal becomes the official scope reference for all subsequent gates.

---

## 6. Client Discovery

When a new external client project starts, Tera must enter Client Discovery before implementation.

Client Discovery gathers:

- client identity
- contacts and approval authority
- application idea
- client goals
- client pain points
- preferred style and visual direction
- reference apps, sites, documents, colors, or brand assets
- non-preferred examples
- expected approval process
- budget/time sensitivity if the user chooses to provide it
- client-specific risks

### Majed as Client Intermediary

The normal operating model is:

- Tera asks Majed short, direct client questions.
- Majed asks the client.
- Majed returns the answers to Tera.
- Tera documents material answers in the correct files.

Tera must phrase questions so they can be forwarded to the client with minimal editing.

### Improvement Suggestions

Tera may suggest improvements, additions, or simplifications only if they:

- support the client's original goal
- do not change the core project idea
- are clearly marked as suggestions
- are separated from confirmed scope
- are offered for Majed to review with the client

Suggestions do not become scope until approved and documented.

---

## 7. Client Approval Package

### Location

```text
clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/client-approval/
```

### Required Files

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

### Approval Gates

| Gate | Required approval |
|---|---|
| Gate 1: Idea Approval | The client confirms the project idea is understood correctly. |
| Gate 2: Scope Approval | The client confirms what is in scope, out of scope, and deferred. |
| Gate 3: Flow Approval | The client confirms the main user and business flows. |
| Gate 4: Screen Approval | The client confirms the screen list and screen purposes. |
| Gate 5: Design Direction Approval | The client confirms visual direction, tone, and references. |
| Gate 6: Prototype Approval | The client confirms the prototype or prototype plan when applicable. |
| Gate 7: Execution Authorization | The client authorizes moving into implementation. |

### Approval Record

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

### Relationship to Internal Implementation

Implementation tasks must trace back to approved scope and acceptance criteria.

Tera must not create implementation `TASK-ID`s for client work before Gate 7 passes.

If Majed explicitly authorizes a pre-approval technical spike, it must be treated as non-deliverable research only, must not modify final application code, must not become client scope, and must be documented separately from Build Mode.

---

## 8. Change Control

```text
No undocumented change requests during execution.
```

### Change Log Location

```text
clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/client-approval/11_CHANGE_CONTROL.md
```

If a change affects execution tasks, also record the related decision or issue in `project-control/`.

### Change Types

| Type | Meaning | Default action |
|---|---|---|
| Clarification | Explains an existing approved item without changing scope | Document and continue |
| Minor Adjustment | Small change inside approved scope | Document and allow if low risk |
| Enhancement | Improves an approved feature but adds work | Ask user before adding |
| New Scope | Adds a new feature, screen, workflow, integration, or data area | Requires explicit approval |
| Phase 2 | Valuable but not needed for current delivery | Defer |
| Rejected | Conflicts with project goal, approved scope, or constraints | Do not implement |

### Change Record Format

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

### Implementation Restriction

Tera must not implement a change classified as `Enhancement` or `New Scope` until Majed confirms the required client approval and the approval is documented.

For `New Scope`, approval from the documented client approval authority is required unless Majed explicitly records that the item is deferred to a later phase and will not affect the current build.

### Final Rule

```text
Approved project scope can only change through an approved change record.
```

---

## 9. Client-Facing Content

### Default Language

Client-facing documents are written in Arabic by default.

Use another language only when Majed explicitly requests it.

### Content Rules

Client-facing content must be:

- clear and readable by non-technical clients
- concise but complete enough for approval
- free of internal Tera implementation details
- free of sub-agent names, internal orchestration details, token policies, or runtime mechanics
- explicit about scope, assumptions, exclusions, and pending decisions
- careful not to promise uncertain timelines, costs, integrations, or capabilities

### Separation Rule

Do not expose internal Tera files directly as client documents unless Tera has rewritten them into client-facing form.

Examples:

- `project-preparation/08_TECHNICAL_ARCHITECTURE.md` is internal.
- `clients/.../client-approval/02_CLIENT_PROPOSAL.md` is client-facing.

### Approval Language

Every approval document must include a clear approval section, such as:

```text
حالة الاعتماد: معتمد / يحتاج تعديل / مرفوض / بانتظار قرار
ملاحظات العميل:
الشخص المعتمد:
تاريخ الاعتماد:
```

### Suggestion Language

Suggestions must be marked clearly:

```text
اقتراح من تيرا - يحتاج موافقة العميل قبل اعتباره ضمن النطاق.
```

### Final Rule

```text
Client-facing content must protect clarity, scope, and trust.
```

---

## 10. Relationship to Internal Tera Files

Client files are official relationship and approval records.

Internal Tera files remain responsible for execution planning:

- `project-inputs/`
- `project-preparation/`
- `project-control/`

If a client-facing file conflicts with an internal Tera file, Tera must stop affected work and ask the user to resolve the contradiction.
