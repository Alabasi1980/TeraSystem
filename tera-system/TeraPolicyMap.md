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
| Tera identity and authority | `.opencode/agents/tera.md` | `.opencode/agents/tera.md` | Runtime must stay compact. |
| Owner strategic advisory | `.opencode/agents/tera-strategic-advisor.md` | Not applicable | Independent advisor for Majed only. Advice is not approval or execution authorization. |
| Core agent conduct gate | `tera-system/TERA_AGENT_CONDUCT.md` | `.opencode/agents/*.md` | Short runtime reference only. Do not duplicate the conduct rules in each agent file. |
| Runtime authority order | `.opencode/agents/tera.md` | Not applicable | Active runtime wins until conflict is corrected. |
| Operating model overview | `.opencode/agents/tera.md` (Section 36) | Not applicable | Merged from Tera Operating Model.md (now deprecated). |
| System architecture | `tera-system/TeraArchitectureMap.md` | `.opencode/agents/tera.md` | Runtime should only link or summarize. |
| Project intake | `tera-system/TeraProjectIntakePolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Do not duplicate full intake rules elsewhere. |
| Application question bank | `tera-system/TeraApplicationQuestionBank.md` | `TERA_RUNTIME_PROTOCOLS.md` (Section 18) | Reference bank of categorized questions for Client Discovery + Smart Interview. Includes assumption handling rules. |
| Client discovery + smart interview | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` (Section 18) | `.opencode/agents/tera.md` | Two-stage intake: open conversation → structured questioning if needed. |
| Domain intelligence + research | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` (Section 12) | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Covers real-time search during discovery, on-demand research, and the No-Guessing Rule. |
| Client policy (unified) | `tera-system/TeraClientPolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md`, `TERA_RUNTIME_TEMPLATES.md` | Merged from 4 separate client files (now deprecated). Covers engagement, approval, change control, and facing content. |
| TCEA discovery coverage framework | `.opencode/agents/tera-client-engagement.md` | `.opencode/agents/tera.md` | Governs the mandatory 13-domain discovery framework, completeness matrix, Discovery Coverage Gate, Quotation Readiness Gate, and Tera Handoff Readiness Gate for external client work. |
| TCEA discovery domains (canonical) | `tera-system/client-helpers/tera-client-engagement-discovery-domains.md` | Not applicable | Canonical source for the 13 domains numbering, naming, aliases, coverage, and blocking rules. All other files reference this source. |
| TCEA commercial value discovery (MR6) | `.opencode/agents/tera-client-engagement.md` (A.6.0 MR6) | `.opencode/agents/tera.md` | 6th Master Rule: TCEA must actively discover value-added opportunities — propose as options, not commitments. |
| TCEA value-added proposals protocol | `tera-system/client-helpers/tera-client-engagement-protocols.md` (A.6.10) | Not applicable | Detailed protocol for commercial proposals: types, rules, template, connection to Future-Proof Discovery. |
| Application blueprinting | `.opencode/agents/application-blueprint.md` | `.opencode/agents/tera.md` | Governs pre-preparation blueprinting only. `APPLICATION_BLUEPRINT.md` starts as Draft and must reach `approved_for_preparation` before Tera formal preparation uses it. |
| Preparation file catalog | `tera-system/Tera_Project_Preparation_Files.md` | `TERA_PROJECT_DECISION.md` | Catalog only; avoid copying full templates. |
| Project decision template | `tera-system/TERA_PROJECT_DECISION.md` | Not applicable | 13-section merged structure: intake readiness, classification, initial scope, tech, client readiness, files, sub-agents, risks, model tier, token policy, final decision, post-decision protocol. |
| 7-phase Tera workflow | `.opencode/agents/tera.md` Section 5 | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Runtime must stay synced because `.opencode/agents/tera.md` has higher active priority. |
| Phase 3 preparation planning output | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 27 | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Produces `project-control/PREPARATION_PLAN.md`; no file creation or agent generation in Phase 3. |
| Phase 4 agent delegation output | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 28 | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Produces `project-control/AGENT_DELEGATION_PLAN.md`; preparation-file delegation only. |
| Phase 5 execution planning outputs | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Sections 29-31 | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TeraPreExecutionGate.md` | Produces `PROJECT_MASTER_PLAN.md`, `PROJECT_DETAILED_EXECUTION_PLAN.md`, and `EXECUTION_BATCH_PLAN.md`. |
| Phase 6 post-execution review template | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 32 | `TASK_TEMPLATE.md`, `TeraPreExecutionGate.md`, `TERA_RUNTIME_CHECKLISTS.md` | Used inside `TASK-COD-*` before final acceptance or closure. |
| Phase 7 delivery, handover, and closure | `.opencode/agents/tera.md` Section 5 + `TERA_RUNTIME_TEMPLATES.md` Section 34 | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TERA_RUNTIME_PROTOCOLS.md` | Project closure is separate from last TASK-COD closure. Phase 7 does not execute code. |
| Sub-agent registry | `tera-system/TeraSubAgents.md` | `.opencode/agents/tera.md` | Runtime names triggers only. |
| Agent generation template | `tera-system/AGENT_GENERATION_TEMPLATE.md` | `TERA_RUNTIME_PROTOCOLS.md` | Draft agents use this source. |
| Pre/Post execution gates | `tera-system/TeraPreExecutionGate.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Gate details stay in the policy file. |
| Token/context policy | `tera-system/TeraTokenPolicy.md` | `.opencode/agents/tera.md` | Runtime contains only startup/loading rules. |
| Runtime protocols | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | `.opencode/agents/tera.md` | Detailed operational behavior. |
| Runtime templates | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | Not applicable | Formal output formats only. |
| Runtime checklists | `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` | `.opencode/agents/tera.md` | Checklists stay outside runtime. |
| MVP classification | `tera-system/runtime/MVP_DEFINITION_PROTOCOL.md` | `.opencode/agents/tera.md` | Runtime references when to load it. |
| Technology profiles | `tera-system/profiles/` | `.opencode/agents/tera.md` | Stack-specific rules stay in profiles. |
| Design governance layer | `tera-system/design-system/` | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TERA_RUNTIME_PROTOCOLS.md` | Governs Design Source Decision, DESIGN.md integration, internal kits, design tokens, component rules, and UI Acceptance Gate. |
| Monitor audit framework | `.opencode/agents/monitor.md` | `ENGINEERING_AGENT_RESPONSIBILITIES.md` | Governs the 7 immutable audit rules, reference hierarchy, plan rejection authority, and cumulative audit for plan-compliance checking. |
| Auditor audit framework | `.opencode/agents/auditor.md` | `ENGINEERING_AGENT_RESPONSIBILITIES.md` | Governs the 6-stage graded audit methodology, results classification, uncertainty protocol, cumulative audit, and engineering governance review scope. |
| Project UI/UX executable guide | `project-preparation/28_UI_UX_GUIDELINES.md` using `TERA_RUNTIME_TEMPLATES.md` Section 33 | `TASK_TEMPLATE.md`, `TeraPreExecutionGate.md` | Final executable visual design rules for EngineeringAgent; raw sources stay in `project-preparation/design-source/`. |
| Application proposal template | `tera-workshop/client-templates/commercial/APPLICATION_PROPOSAL_TEMPLATE.md` | `TERA_RUNTIME_TEMPLATES.md` (Section 26) | Formal proposal document. Generated after Client Discovery before formal preparation. |
| Scope of Work template | `tera-workshop/client-templates/commercial/SCOPE_OF_WORK_TEMPLATE.md` | — | Formal SOW document — scope, deliverables, timeline, payment, assumptions, sign-off. Generated after proposal approval. |
| Technical Proposal template | `tera-workshop/client-templates/commercial/TECHNICAL_PROPOSAL_TEMPLATE.md` | — | Technical architecture, stack, hosting, security, testing, CI/CD. Generated after SOW approval. |
| Change Request form | `tera-workshop/client-templates/contractual/CHANGE_REQUEST_FORM.md` | — | Change management — description, impact analysis, revised estimates, approval. Used during execution. |
| Software Services Agreement | `tera-workshop/client-templates/contractual/SOFTWARE_SERVICES_AGREEMENT_TEMPLATE.md` | — | 18-section legal agreement covering scope, obligations, payment, IP, confidentiality, liability, termination. Arabic, Jordanian law. |
| Quotation | `tera-workshop/client-templates/commercial/QUOTATION_TEMPLATE.md` | — | Formal price quotation — items, totals, payment terms, sign-off. |
| Meeting Report | `tera-workshop/client-templates/pre-contract/MEETING_REPORT_TEMPLATE.md` | — | Meeting minutes — attendees, agenda, decisions, action items. |
| Status Report | `tera-workshop/client-templates/contractual/STATUS_REPORT_TEMPLATE.md` | — | Periodic status — progress, achievements, risks, next steps. |
| Handover Report | `tera-workshop/client-templates/handover/HANDOVER_REPORT_TEMPLATE.md` | — | Delivery and acceptance — deliverables checklist, access info, sign-off. |
| Completion Certificate | `tera-workshop/client-templates/handover/COMPLETION_CERTIFICATE_TEMPLATE.md` | — | Formal project completion certificate with seal and signatures. |
| Client Intake Form | `tera-workshop/client-templates/pre-contract/CLIENT_INTAKE_FORM.md` | — | Pre-interview questionnaire — company, project, goals, budget, tech. |
| Project Charter | `tera-workshop/client-templates/commercial/PROJECT_CHARTER_TEMPLATE.md` | — | Project initiation document — objectives, scope, team, risks, success criteria. |
| User Persona Matrix | `tera-workshop/client-templates/pre-contract/USER_PERSONA_MATRIX_TEMPLATE.md` | — | User roles and persona cards — goals, pain points, permissions, features. |
| Gap Analysis | `tera-workshop/client-templates/pre-contract/GAP_ANALYSIS_TEMPLATE.md` | — | As-is vs to-be analysis, gap identification, recommendations. |
| Risk Register | `tera-workshop/client-templates/pre-contract/RISK_REGISTER_TEMPLATE.md` | — | Risk tracking — impact, probability, mitigation, owner, status. |
| SLA | `tera-workshop/client-templates/contractual/SLA_TEMPLATE.md` | — | Service Level Agreement — support hours, response times, priorities. |
| NDA | `tera-workshop/client-templates/pre-contract/NDA_TEMPLATE.md` | — | Non-Disclosure Agreement — confidentiality, exclusions, governing law. |
| Client Satisfaction Survey | `tera-workshop/client-templates/handover/CLIENT_SATISFACTION_SURVEY_TEMPLATE.md` | — | Post-project feedback — ratings, open questions, recommendation. |
| Agent improvement suggestions (AIS) | `tera-system/AIS_PROTOCOL.md` | `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` | Protocol + central log for agent self-improvement suggestions from real work. Agents record; Hares processes; Majed approves. |
| User guide | `tera-system/TERA_USER_GUIDE.md` | Not applicable | User-facing prompts and usage examples. |
| System maintenance | `tera-system/TeraSystemMaintenanceChecklist.md` | `.opencode/agents/tera.md` | Use when modifying Tera itself. |
| Agent dependency map | `tera-system/AGENT_DEPENDENCY_MAP.md` | `tera-system/AGENT_DEPENDENCY_MAP.md` | Cross-references between agent files; safe edit order; file size alerts. Read before editing any agent file. |
| Scenario stress tests | `tera-system/TeraScenarioStressTests.md` | Not applicable | Used for validation, not daily runtime. |
| Client workspace guide | `clients/README.md` | Not applicable | Folder usage guide only; policies remain in `tera-system/`. |

## 4. Duplication Policy

Allowed duplication:

- Short critical rules in `.opencode/agents/tera.md`.
- Short conduct-gate references in `.opencode/agents/*.md`.
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
