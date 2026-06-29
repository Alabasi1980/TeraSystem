# Tera Architecture Map

## 1. Purpose

This file describes Tera as an operating system for client and software project delivery.

It is a map, not a policy source. Rules remain in the files listed in `TeraPolicyMap.md`.

## 2. Architectural Layers

| Layer | Responsibility | Main files/folders |
|---|---|---|
| Identity and authority | Defines Tera role, authority, runtime order, and decision ownership | `TeraAgent.md`, `.opencode/agents/tera.md` |
| Project intake | Captures raw idea, technical context, missing information, and readiness | `TeraProjectIntakePolicy.md`, `project-inputs/` |
| Client engagement and approval | Manages client profile, contacts, approval package, and change control | `TeraClient*.md`, `clients/` |
| Preparation and analysis | Converts intake into internal project planning and execution-ready files | `Tera_Project_Preparation_Files.md`, `project-preparation/` |
| Orchestration and gates | Controls delegation, task readiness, pre/post execution review, and build mode | `TeraPreExecutionGate.md`, `runtime/`, `project-control/` |
| Technical specialization | Keeps stack-specific behavior outside the generic Tera system | `tera-system/profiles/` |
| Sub-agent lifecycle | Defines, generates, narrows, activates, and reviews specialized agents | `TeraSubAgents.md`, `AGENT_GENERATION_TEMPLATE.md`, `generated-agents/`, `.opencode/agents/` |
| Delivery and handoff | Produces final docs, client delivery material, and operational handover | `project-preparation/11_DELIVERY_AND_HANDOVER.md`, `clients/.../delivery/` |

## 3. Folder Roles

| Folder | Role | Must not be used for |
|---|---|---|
| `tera-system/` | System reference and policies | Project-specific outputs during normal execution |
| `.opencode/agents/` | Active runtime agents | Long policy details or generated drafts |
| `project-inputs/` | Raw or normalized intake inputs | Formal preparation replacements |
| `project-preparation/` | Internal Tera preparation outputs | Client-facing approval package |
| `clients/` | Client records, approval packages, assets, communications, delivery | Internal-only execution planning |
| `project-control/` | Task, issue, decision, activity, and state tracking | Requirements or design source of truth |
| `generated-agents/opencode/` | Draft generated agents | Active runtime without review/activation |
| `design-source/` | Raw design references | Final executable UI rules |
| `tera-system/profiles/` | Technology-specific execution rules | Generic project policy |

## 4. Core Flow

```text
Client / User Idea
-> Project Intake
-> Client Discovery and Approval Package when external client
-> Tera Project Decision
-> Internal Preparation
-> Implementation Planning
-> Pre-Execution Gate
-> Delegated Execution
-> Post-Execution Review
-> Milestone / Client Approval
-> Delivery and Handoff
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
