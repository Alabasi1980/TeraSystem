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
| Operating model overview | `tera-system/TeraAgent.md` (Section 36) | Not applicable | Merged from Tera Operating Model.md (now deprecated). |
| System architecture | `tera-system/TeraArchitectureMap.md` | `.opencode/agents/tera.md` | Runtime should only link or summarize. |
| Project intake | `tera-system/TeraProjectIntakePolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Do not duplicate full intake rules elsewhere. |
| Application question bank | `tera-system/TeraApplicationQuestionBank.md` | `TERA_RUNTIME_PROTOCOLS.md` (Section 18) | Reference bank of categorized questions for Client Discovery + Smart Interview. Includes assumption handling rules. |
| Client discovery + smart interview | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` (Section 18) | `.opencode/agents/tera.md` | Two-stage intake: open conversation → structured questioning if needed. |
| Domain intelligence + research | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` (Section 12) | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Covers real-time search during discovery, on-demand research, and the No-Guessing Rule. |
| Client policy (unified) | `tera-system/TeraClientPolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md`, `TERA_RUNTIME_TEMPLATES.md` | Merged from 4 separate client files (now deprecated). Covers engagement, approval, change control, and facing content. |
| Preparation file catalog | `tera-system/Tera_Project_Preparation_Files.md` | `TERA_PROJECT_DECISION.md` | Catalog only; avoid copying full templates. |
| Project decision template | `tera-system/TERA_PROJECT_DECISION.md` | Not applicable | 13-section merged structure: intake readiness, classification, initial scope, tech, client readiness, files, sub-agents, risks, model tier, token policy, final decision, post-decision protocol. |
| 6-phase Tera workflow | `tera-system/TeraAgent.md` Section 5 | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Runtime must stay synced because `.opencode/agents/tera.md` has higher active priority. |
| Phase 3 preparation planning output | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 27 | `TeraAgent.md`, `TERA_RUNTIME_CHECKLISTS.md` | Produces `project-control/PREPARATION_PLAN.md`; no file creation or agent generation in Phase 3. |
| Phase 4 agent delegation output | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 28 | `TeraAgent.md`, `TERA_RUNTIME_CHECKLISTS.md` | Produces `project-control/AGENT_DELEGATION_PLAN.md`; preparation-file delegation only. |
| Phase 5 execution planning outputs | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Sections 29-31 | `TeraAgent.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TeraPreExecutionGate.md` | Produces `PROJECT_MASTER_PLAN.md`, `PROJECT_DETAILED_EXECUTION_PLAN.md`, and `EXECUTION_BATCH_PLAN.md`. |
| Phase 6 post-execution review template | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 32 | `TASK_TEMPLATE.md`, `TeraPreExecutionGate.md`, `TERA_RUNTIME_CHECKLISTS.md` | Used inside `TASK-COD-*` before final acceptance or closure. |
| Sub-agent registry | `tera-system/TeraSubAgents.md` | `.opencode/agents/tera.md` | Runtime names triggers only. |
| Agent generation template | `tera-system/AGENT_GENERATION_TEMPLATE.md` | `TERA_RUNTIME_PROTOCOLS.md` | Draft agents use this source. |
| Pre/Post execution gates | `tera-system/TeraPreExecutionGate.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Gate details stay in the policy file. |
| Token/context policy | `tera-system/TeraTokenPolicy.md` | `.opencode/agents/tera.md` | Runtime contains only startup/loading rules. |
| Runtime protocols | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | `.opencode/agents/tera.md` | Detailed operational behavior. |
| Runtime templates | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | Not applicable | Formal output formats only. |
| Runtime checklists | `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` | `.opencode/agents/tera.md` | Checklists stay outside runtime. |
| MVP classification | `tera-system/runtime/MVP_DEFINITION_PROTOCOL.md` | `.opencode/agents/tera.md` | Runtime references when to load it. |
| Technology profiles | `tera-system/profiles/` | `.opencode/agents/tera.md` | Stack-specific rules stay in profiles. |
| Application proposal template | `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html` | `TERA_RUNTIME_TEMPLATES.md` (Section 26) | Self-contained HTML page, RTL, printable. Generated after Client Discovery before formal preparation. |
| Scope of Work template | `tera-workshop/SCOPE_OF_WORK_TEMPLATE.html` | — | Formal SOW document — scope, deliverables, timeline, payment, assumptions, sign-off. Generated after proposal approval. |
| Technical Proposal template | `tera-workshop/TECHNICAL_PROPOSAL_TEMPLATE.html` | — | Technical architecture, stack, hosting, security, testing, CI/CD. Generated after SOW approval. |
| Change Request form | `tera-workshop/CHANGE_REQUEST_FORM.html` | — | Change management — description, impact analysis, revised estimates, approval. Used during execution. |
| Software Services Agreement | `tera-workshop/SOFTWARE_SERVICES_AGREEMENT_TEMPLATE.html` | — | 18-section legal agreement covering scope, obligations, payment, IP, confidentiality, liability, termination. Arabic, Jordanian law. |
| Quotation | `tera-workshop/QUOTATION_TEMPLATE.html` | — | Formal price quotation — items, totals, payment terms, sign-off. |
| Meeting Report | `tera-workshop/MEETING_REPORT_TEMPLATE.html` | — | Meeting minutes — attendees, agenda, decisions, action items. |
| Status Report | `tera-workshop/STATUS_REPORT_TEMPLATE.html` | — | Periodic status — progress, achievements, risks, next steps. |
| Handover Report | `tera-workshop/HANDOVER_REPORT_TEMPLATE.html` | — | Delivery and acceptance — deliverables checklist, access info, sign-off. |
| Completion Certificate | `tera-workshop/COMPLETION_CERTIFICATE_TEMPLATE.html` | — | Formal project completion certificate with seal and signatures. |
| Client Intake Form | `tera-workshop/CLIENT_INTAKE_FORM.html` | — | Pre-interview questionnaire — company, project, goals, budget, tech. |
| Project Charter | `tera-workshop/PROJECT_CHARTER_TEMPLATE.html` | — | Project initiation document — objectives, scope, team, risks, success criteria. |
| User Persona Matrix | `tera-workshop/USER_PERSONA_MATRIX_TEMPLATE.html` | — | User roles and persona cards — goals, pain points, permissions, features. |
| Gap Analysis | `tera-workshop/GAP_ANALYSIS_TEMPLATE.html` | — | As-is vs to-be analysis, gap identification, recommendations. |
| Risk Register | `tera-workshop/RISK_REGISTER_TEMPLATE.html` | — | Risk tracking — impact, probability, mitigation, owner, status. |
| SLA | `tera-workshop/SLA_TEMPLATE.html` | — | Service Level Agreement — support hours, response times, priorities. |
| NDA | `tera-workshop/NDA_TEMPLATE.html` | — | Non-Disclosure Agreement — confidentiality, exclusions, governing law. |
| Client Satisfaction Survey | `tera-workshop/CLIENT_SATISFACTION_SURVEY_TEMPLATE.html` | — | Post-project feedback — ratings, open questions, recommendation. |
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
