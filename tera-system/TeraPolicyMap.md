# Tera Policy Map

## 1. Purpose

This file maps each major Tera operating topic to its official source of truth.

It prevents rule duplication, unclear authority, and runtime bloat as the system grows.

## 2. Rule

```text
Policies define rules.
Runtime files summarize operational triggers.
Guides explain usage.
Project files record project-specific decisions.
```

## 3. Source Of Truth Map

| Topic | Source of truth | Runtime summary allowed in | Notes |
|---|---|---|---|
| Tera identity and authority | `tera-system/TeraAgent.md` | `.opencode/agents/tera.md` | Runtime must stay compact. |
| Runtime authority order | `.opencode/agents/tera.md` | Not applicable | Active runtime wins until conflict is corrected. |
| Operating model overview | `tera-system/Tera Operating Model.md` | Not applicable | Conceptual overview only; not a detailed policy source. |
| System architecture | `tera-system/TeraArchitectureMap.md` | `.opencode/agents/tera.md` | Runtime should only link or summarize. |
| Project intake | `tera-system/TeraProjectIntakePolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Do not duplicate full intake rules elsewhere. |
| Client engagement | `tera-system/TeraClientEngagementPolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Covers client profile, contacts, and discovery. |
| Client approval | `tera-system/TeraClientApprovalPolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Mandatory for external client projects. |
| Client change control | `tera-system/TeraClientChangeControlPolicy.md` | `TERA_RUNTIME_PROTOCOLS.md` | Change records live under `clients/.../client-approval/`. |
| Client-facing content | `tera-system/TeraClientFacingContentPolicy.md` | `TERA_RUNTIME_TEMPLATES.md` | Arabic by default. |
| Preparation file catalog | `tera-system/Tera_Project_Preparation_Files.md` | `TERA_PROJECT_DECISION.md` | Catalog only; avoid copying full templates. |
| Project decision template | `tera-system/TERA_PROJECT_DECISION.md` | Not applicable | Template for project-level decision. |
| Sub-agent registry | `tera-system/TeraSubAgents.md` | `.opencode/agents/tera.md` | Runtime names triggers only. |
| Agent generation template | `tera-system/AGENT_GENERATION_TEMPLATE.md` | `TERA_RUNTIME_PROTOCOLS.md` | Draft agents use this source. |
| Pre/Post execution gates | `tera-system/TeraPreExecutionGate.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Gate details stay in the policy file. |
| Token/context policy | `tera-system/TeraTokenPolicy.md` | `.opencode/agents/tera.md` | Runtime contains only startup/loading rules. |
| Runtime protocols | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | `.opencode/agents/tera.md` | Detailed operational behavior. |
| Runtime templates | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | Not applicable | Formal output formats only. |
| Runtime checklists | `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` | `.opencode/agents/tera.md` | Checklists stay outside runtime. |
| MVP classification | `tera-system/runtime/MVP_DEFINITION_PROTOCOL.md` | `.opencode/agents/tera.md` | Runtime references when to load it. |
| Technology profiles | `tera-system/profiles/` | `.opencode/agents/tera.md` | Stack-specific rules stay in profiles. |
| User guide | `tera-system/TERA_USER_GUIDE.md` | Not applicable | User-facing prompts and usage examples. |
| System maintenance | `tera-system/TeraSystemMaintenanceChecklist.md` | `.opencode/agents/tera.md` | Use when modifying Tera itself. |
| Scenario stress tests | `tera-system/TeraScenarioStressTests.md` | Not applicable | Used for validation, not daily runtime. |
| Client workspace guide | `clients/README.md` | Not applicable | Folder usage guide only; policies remain in `tera-system/`. |

## 4. Duplication Policy

Allowed duplication:

- Short critical rules in `.opencode/agents/tera.md`.
- Checklist references in runtime files.
- User-facing summaries in `TERA_USER_GUIDE.md`.

Forbidden duplication:

- Copying full policy sections into runtime.
- Rewriting the same mandatory file list in multiple catalog files.
- Defining conflicting gates in more than one source.
- Adding new agents in runtime without updating `TeraSubAgents.md`.

## 5. Change Rule

When a rule changes, update the source of truth first, then update only the summaries that are operationally required.

If the change affects runtime behavior, sync `.opencode/agents/tera.md` and update its `Last Synced` date.

If a new policy file, folder role, or lifecycle layer is added, update this file and `TeraArchitectureMap.md` in the same maintenance pass.
