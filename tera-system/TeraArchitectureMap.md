# Tera Architecture Map

## 1. Purpose

This file describes Tera as an operating system for client and software project delivery.

It is a map, not a policy source. Rules remain in the files listed in `TeraPolicyMap.md`.

## 2. Architectural Layers

| Layer | Responsibility | Main files/folders |
|---|---|---|
| Identity and authority | Defines Tera role, authority, runtime order, and decision ownership | `TeraAgent.md`, `.opencode/agents/tera.md` |
| Project intake | Captures raw idea, technical context, missing information, and readiness | `TeraProjectIntakePolicy.md`, active application workspace `project-inputs/` |
| Client engagement and approval | Manages client profile, contacts, approval package, and change control | `TeraClient*.md`, `clients/` |
| Preparation and analysis | Converts intake into internal project planning and execution-ready files | `Tera_Project_Preparation_Files.md`, active application workspace `project-preparation/` |
| Design governance | Controls design source decisions, design tokens, component rules, internal kits, and UI acceptance | `tera-system/design-system/`, active application workspace `project-preparation/28_UI_UX_GUIDELINES.md`, `project-preparation/design-source/` |
| Orchestration and gates | Controls delegation, task readiness, pre/post execution review, build mode, and project closure gates | `TeraPreExecutionGate.md`, `runtime/`, active application workspace `project-control/` |
| Technical specialization | Keeps stack-specific behavior outside the generic Tera system | `tera-system/profiles/` |
| Sub-agent lifecycle | Defines, generates, narrows, activates, and reviews specialized agents | `TeraSubAgents.md`, `AGENT_GENERATION_TEMPLATE.md`, `generated-agents/`, `.opencode/agents/` |
| Application workspace isolation | Keeps every generated application removable/exportable without polluting the Tera system root | `clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/` |
| Delivery, handoff, and closure | Produces final delivery readiness, release notes, client handover material, acceptance records, and closure reports | active application workspace `project-control/DELIVERY_READINESS_REPORT.md`, `project-control/PROJECT_CLOSURE_REPORT.md`, `delivery/` |

## 3. Folder Roles

| Folder | Role | Must not be used for |
|---|---|---|
| `tera-system/` | System reference and policies | Project-specific outputs during normal execution |
| `.opencode/agents/` | Active runtime agents | Long policy details or generated drafts |
| `clients/` | Client records and isolated application workspaces | Tera system policies |
| `clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/` | Canonical workspace for one application, including intake, preparation, control, generated agents, source code, approvals, assets, communications, and delivery | Multiple unrelated applications or Tera system files |
| `project-inputs/` | Root-level template/bootstrap area only unless explicitly operating without an application workspace | New application-specific intake after an application workspace is identified |
| `project-preparation/` | Root-level template/bootstrap area only unless explicitly operating without an application workspace | New application-specific preparation after an application workspace is identified |
| `project-preparation/design-source/` | Root-level template/bootstrap design-source area only | New application-specific design sources after an application workspace is identified |
| `project-control/` | Root-level Tera/system maintenance control and bootstrap state | New application task/control records after an application workspace is identified |
| `generated-agents/opencode/` | Root-level draft generated agents for system maintenance or bootstrap only | New application-specific agents after an application workspace is identified |
| `tera-system/design-system/` | System design governance, schemas, gates, and internal kits | Project-specific design decisions or client raw assets |
| `tera-system/profiles/` | Technology-specific execution rules | Generic project policy |
| `tera-workshop/` | System development and tooling files (templates, experiments, system-level outputs) | Core policy, project files, or runtime agents |

## 4. Core Flow

For every new application, Tera must first identify or create the isolated application workspace:

```text
clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/
```

All application-specific files then live under that folder, including `project-inputs/`, `project-preparation/`, `project-control/`, `generated-agents/opencode/`, client approval folders, delivery folders, and the application source code. Removing or exporting this folder must remove/export the application without modifying the Tera system root.

```text
Client / User Idea
-> Application Workspace Identification / Creation
-> Project Intake
-> Client Discovery and Approval Package when external client
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
-> Client Approval Package
-> Approval Record
-> Execution Authorization
-> Internal Preparation and Implementation Planning
-> Build Mode
-> Phase 7 Delivery, Handover, and Closure
-> Change Control for later requests
```

## 6. Runtime Design Principle

`.opencode/agents/tera.md` should answer:

- What must Tera load now?
- What is forbidden now?
- Which policy should Tera read for details?
- Is Build Mode allowed?

It should not duplicate full policies, templates, or long agent contracts.

## 7. Stability Rule

When the system grows, add new policies only when there is a clear operating gap. Prefer updating maps and existing policies over creating new layers.
