# Tera Architecture Map

## 1. Purpose

This file describes Tera as an operating system for client and software project delivery.

It is a map, not a policy source. Rules remain in the files listed in `TeraPolicyMap.md`.

## 2. Architectural Layers

| Layer | Responsibility | Main files/folders |
|---|---|---|
| Identity and authority | Defines Tera role, authority, runtime order, decision ownership, and core agent conduct gate | `TeraAgent.md`, `TERA_AGENT_CONDUCT.md`, `.opencode/agents/tera.md` |
| Project intake | Captures raw idea, technical context, missing information, and readiness | `TeraProjectIntakePolicy.md`, `project-inputs/` |
| Client engagement and approval | Manages client profile, contacts, approval package, and change control | `TeraClient*.md`, `clients/` |
| Application blueprinting | Converts confirmed client handoff into a high-level internal application blueprint before formal preparation | `TeraApplicationBlueprint.md`, `project-preparation/APPLICATION_BLUEPRINT.md`, `project-preparation/draft-seeds/` |
| Preparation and analysis | Converts intake into internal project planning and execution-ready files | `Tera_Project_Preparation_Files.md`, `project-preparation/` |
| Design governance | Controls design source decisions, design tokens, component rules, internal kits, and UI acceptance | `tera-system/design-system/`, `project-preparation/28_UI_UX_GUIDELINES.md`, `project-preparation/design-source/` |
| Orchestration and gates | Controls delegation, task readiness, pre/post execution review, build mode, and project closure gates | `TeraPreExecutionGate.md`, `runtime/`, `project-control/` |
| Technical specialization | Keeps stack-specific behavior outside the generic Tera system | `tera-system/profiles/` |
| Sub-agent lifecycle | Defines, generates, narrows, activates, and reviews specialized agents | `TeraSubAgents.md`, `AGENT_GENERATION_TEMPLATE.md`, `generated-agents/`, `.opencode/agents/` |
| Delivery, handoff, and closure | Produces final delivery readiness, release notes, client handover material, acceptance records, and closure reports | `project-control/DELIVERY_READINESS_REPORT.md`, `project-control/PROJECT_CLOSURE_REPORT.md`, `clients/.../delivery/` |

## 3. Folder Roles

| Folder | Role | Must not be used for |
|---|---|---|
| `tera-system/` | System reference and policies | Project-specific outputs during normal execution |
| `.opencode/agents/` | Active runtime agents | Long policy details or generated drafts |
| `project-inputs/` | Raw or normalized intake inputs | Formal preparation replacements |
| `project-preparation/` | Internal Tera preparation outputs | Client-facing approval package |
| `project-preparation/design-source/` | Raw project design references selected by Tera | Final executable UI rules |
| `clients/` | Client records, approval packages, assets, communications, delivery | Internal-only execution planning |
| `project-control/` | Task, issue, decision, activity, and state tracking | Requirements or design source of truth |
| `generated-agents/opencode/` | Draft generated agents | Active runtime without review/activation |
| `tera-system/design-system/` | System design governance, schemas, gates, and internal kits | Project-specific design decisions or client raw assets |
| `tera-system/profiles/` | Technology-specific execution rules | Generic project policy |
| `tera-workshop/` | System development and tooling files (templates, experiments, system-level outputs) | Core policy, project files, or runtime agents |

## 4. Core Flow

```text
Client / User Idea
-> Project Intake
-> Client Discovery and Approval Package when external client
-> Application Blueprinting when required
-> Tera Project Decision
-> Internal Preparation
-> Implementation Planning
-> Pre-Execution Gate
-> Delegated Execution
-> Post-Execution Review
-> Milestone / Client Approval
-> Delivery, Handoff, and Closure
```

## 5. External Client Flow

```text
Client Registration
-> Contacts and Approval Authority
-> Application Intake
-> Client Questions through Majed
-> Discovery Coverage Evaluation
-> Client Approval Package
-> Approval Record
-> Execution Authorization
-> Application Blueprinting when required
-> Internal Preparation and Implementation Planning
-> Build Mode
-> Phase 7 Delivery, Handover, and Closure
-> Change Control for later requests
```

## 6. Runtime Design Principle

All core runtime agents must pass the conduct gate in:

```text
tera-system/TERA_AGENT_CONDUCT.md
```

before non-trivial actions such as file edits, shell commands, or approval-sensitive changes.

`.opencode/agents/tera.md` should answer:

- What must Tera load now?
- What is forbidden now?
- Which policy should Tera read for details?
- Is Build Mode allowed?

It should not duplicate full policies, templates, or long agent contracts.

## 7. Stability Rule

When the system grows, add new policies only when there is a clear operating gap. Prefer updating maps and existing policies over creating new layers.
