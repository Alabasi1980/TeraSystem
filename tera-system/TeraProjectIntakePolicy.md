# Project Intake Policy

## 1. Purpose

This policy defines the required intake gate before Tera starts formal project preparation for any new project.

## 2. Required Intake Files

The minimum intake package is:

- `project-inputs/01_APPLICATION_IDEA.md`
- `project-inputs/02_TECHNICAL_CONTEXT.md`

For external client projects, Tera must also collect or create the client workspace records defined in `tera-system/TeraClientEngagementPolicy.md`:

- `clients/CLIENT-[client-name-or-id]/CLIENT_PROFILE.md`
- `clients/CLIENT-[client-name-or-id]/CONTACTS.md`
- `clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/client-approval/`

## 3. Application Idea File

`project-inputs/01_APPLICATION_IDEA.md` should capture at minimum:

- application description
- problem being solved
- expected users
- main workflows
- expected modules or screens
- required outputs
- MVP scope
- out-of-scope items
- user notes and open questions

## 4. Technical Context File

`project-inputs/02_TECHNICAL_CONTEXT.md` should capture at minimum:

- programming language
- framework
- application type
- database
- ORM / data access approach
- package manager / CLI
- UI framework or design system if known
- required external libraries
- forbidden libraries or technologies
- runtime environment
- deployment or hosting notes if known
- technical or security constraints
- technology profile candidate

## 5. Intake Readiness Status

Use these statuses:

- `Missing`
- `Partial`
- `Complete`

## 6. Intake Collection Mode

If one or both intake files are missing or materially incomplete, Tera must enter:

```text
Intake Collection Mode
```

Rules in this mode:

- Do not start formal `project-preparation/` output.
- Do not create `TERA_PROJECT_DECISION.md`.
- Do not choose a final active Technology Profile.
- Do not generate sub-agents for implementation work.
- Do not create implementation `TASK-ID`s.
- Ask short, direct questions only to close missing intake gaps.
- Document the answers inside the intake files.
- For external client projects, ask short client-facing questions that Majed can forward to the client.
- Do not treat any client approval as final unless the approving contact and authority are documented.

## 7. Minimum Questions

When intake is incomplete, Tera should ask only the shortest useful questions, such as:

- What is the application idea?
- Who will use it?
- What are the three most important workflows?
- Is the technology already decided?
- If not, should Tera propose it later?
- What database is required, if any?
- Is there any preferred or forbidden UI/design direction?

For external client projects, also ask the shortest useful client questions:

- What is the client name?
- Is the client an individual, company, or organization?
- Who are the client contacts, and who can approve decisions?
- What phone numbers or emails are available, if any?
- What language should client documents use? Default is Arabic.
- Does the client have a logo, colors, references, or examples they like?
- Are there examples the client dislikes?
- Does the client need a proposal, prototype, or design approval before implementation?

## 8. When Tera Can Proceed

Tera may proceed to formal preparation only when:

- `01_APPLICATION_IDEA.md` exists and is acceptable at minimum level.
- `02_TECHNICAL_CONTEXT.md` exists and is acceptable at minimum level, or clearly documents that the stack is still undecided.
- Tera has determined whether an existing Technology Profile can be used, or whether a new profile draft will be needed later.

For external client projects, Tera may proceed only to client approval preparation when:

- client profile and contacts are documented at minimum level.
- approval authority is known or explicitly marked as unknown.
- the client application folder under `clients/` is identified.
- Tera has enough information to produce a client-facing approval package.

Tera may not proceed to implementation until `tera-system/TeraClientApprovalPolicy.md` is satisfied.

## 9. Relationship with 00_PROJECT_INPUTS.md

`project-preparation/00_PROJECT_INPUTS.md` is not a replacement for the intake files.

It is Tera's normalized preparation summary derived from:

- `project-inputs/01_APPLICATION_IDEA.md`
- `project-inputs/02_TECHNICAL_CONTEXT.md`

## 10. Final Rule

```text
No Intake = No Project Preparation
No Technical Context = No Active Technology Profile
No Active Technology Profile = No Implementation
No documented client context = No client project preparation
No Client Approval Package = No Implementation
```
