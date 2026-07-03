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
| Operating model overview | `tera-system/TeraAgent.md` (§1.6) | Not applicable | Merged from Tera Operating Model.md (now deprecated). |
| System architecture | `tera-system/TeraArchitectureMap.md` | `.opencode/agents/tera.md` | Runtime should only link or summarize. |
| Application workspace isolation | `tera-system/TeraArchitectureMap.md` + `tera-system/TeraClientPolicy.md` | `.opencode/agents/tera.md`, `clients/README.md` | New applications use `clients/CLIENT-*/applications/APP-*/` as the canonical isolated workspace. |
| Project intake | `tera-system/TeraProjectIntakePolicy.md` | `.opencode/agents/tera-client-engagement.md`, `TERA_RUNTIME_PROTOCOLS.md` | Do not duplicate full intake rules elsewhere. TeraAgent consumes approved handoff rather than running intake directly. |
| Application question bank | `tera-system/TeraApplicationQuestionBank.md` | `TERA_RUNTIME_PROTOCOLS.md` (Section 18), `.opencode/agents/tera-client-engagement.md` | Reference bank of categorized questions for Client Discovery + Smart Interview. Includes assumption handling rules. |
| Client discovery + smart interview | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` (Section 18) | `.opencode/agents/tera-client-engagement.md` | Two-stage intake: open conversation → structured questioning if needed. TeraAgent receives the result as handoff. |
| Domain intelligence + research | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` (Section 12) | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Covers real-time search during discovery, on-demand research, and the No-Guessing Rule. |
| Client policy (unified) | `tera-system/TeraClientPolicy.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md`, `TERA_RUNTIME_TEMPLATES.md` | Merged from 4 separate client files (now deprecated). Covers engagement, approval, change control, and facing content. |
| Preparation file catalog | `tera-system/Tera_Project_Preparation_Files.md` | `TERA_PROJECT_DECISION.md` | Catalog only; avoid copying full templates. |
| Project decision template | `tera-system/TERA_PROJECT_DECISION.md` | Not applicable | 13-section merged structure: intake readiness, classification, initial scope, tech, client readiness, files, sub-agents, risks, model tier, token policy, final decision, post-decision protocol. |
| 7-phase Tera workflow | `tera-system/TeraAgent.md` §4 (Phases 2-7) | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Runtime must stay synced because `.opencode/agents/tera.md` has higher active priority. TeraAgent starts from Phase 2 after receiving handoff from TCEA. |
| Phase 3 preparation planning output | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 27 | `TeraAgent.md`, `TERA_RUNTIME_CHECKLISTS.md` | Produces `project-control/PREPARATION_PLAN.md`; no file creation or agent generation in Phase 3. |
| Phase 4 agent delegation output | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 28 | `TeraAgent.md`, `TERA_RUNTIME_CHECKLISTS.md` | Produces `project-control/AGENT_DELEGATION_PLAN.md`; preparation-file delegation only. |
| Phase 5 execution planning outputs | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Sections 29-31 | `TeraAgent.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TeraPreExecutionGate.md` | Produces `PROJECT_MASTER_PLAN.md`, `PROJECT_DETAILED_EXECUTION_PLAN.md`, and `EXECUTION_BATCH_PLAN.md`. |
| Fast Path for low-risk tasks | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` Section 5.0 + `TERA_RUNTIME_CHECKLISTS.md` + `TERA_RUNTIME_TEMPLATES.md` Section 31 | `.opencode/agents/tera.md` | Fast Path reduces orchestration overhead only. It never replaces physical post-execution review. |
| Phase 6 post-execution review template | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` Section 32 | `TASK_TEMPLATE.md`, `TeraPreExecutionGate.md`, `TERA_RUNTIME_CHECKLISTS.md` | Used inside `TASK-COD-*` before final acceptance or closure. |
| Phase 7 delivery, handover, and closure | `tera-system/TeraAgent.md` §4.6 + `TERA_RUNTIME_TEMPLATES.md` Section 34 | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TERA_RUNTIME_PROTOCOLS.md` | Version/application closure is separate from last TASK-COD closure. Phase 7 does not execute code. |
| Version lifecycle and release management | `tera-system/runtime/VERSION_LIFECYCLE_PROTOCOL.md` | `TeraAgent.md`, `TERA_RUNTIME_PROTOCOLS.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TERA_RUNTIME_TEMPLATES.md` | Defines Version Management Layer: version registry, hotfix/patch/minor/major cycles, versioned TASK-ID fields, release notes, and next-version handoff. Level 3 expansion is deferred until needed. |
| Git release tagging, GitHub Releases, and version repository handling | `tera-system/runtime/VERSION_LIFECYCLE_PROTOCOL.md` Section 7 + `TOOLING_AND_MCP_POLICY.md` Section 3.3 | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TERA_RUNTIME_TEMPLATES.md` | Tera handles commit/push/tag/GitHub Release workflow and repository safety; user approves push/tag/release actions. No force push or tag rewrite without explicit emergency approval. |
| Sub-agent registry | `tera-system/TeraSubAgents.md` | `.opencode/agents/tera.md`, `.opencode/agents/tera-software-designer.md` | Runtime names triggers only. |
| Sub-agent trust metadata | `tera-system/TeraSubAgents.md` §3.5 (Single Source of Truth) + `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` §17 + `project-control/SUB_AGENT_STATUS.md` | `.opencode/agents/tera.md` | Trust metadata guides delegation planning only. It does not change permission level and never replaces physical post-execution review. Trust Level re-evaluation triggers after 2 `Needs Fix` in 5 tasks or after 15 tasks without review. |
| Sub-agent intervention logging | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` Section 17 + `TERA_RUNTIME_TEMPLATES.md` Section 38.2 + `project-control/SUB_AGENT_STATUS.md` | `.opencode/agents/tera.md` | Records explicit Tera interventions on sub-agents (`Stop`, `Narrow`, `Restrict`, `Suspend`, `Reinstate`). Does not replace Emergency Response or physical acceptance review. |
| Scoped runtime override | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` + `TERA_RUNTIME_TEMPLATES.md` Section 38.3 + task templates | `.opencode/agents/tera.md` | Allows Tera to adjust delegation boundaries inside the approved task scope only. Does not replace re-planning when task nature changes, and never replaces physical acceptance review. |
| Governance session agents | `tera-system/TeraSubAgents.md` + project `WORKSPACE_GOVERNANCE_MODEL.md` | `.opencode/agents/tera.md` | Independent OpenCode sessions such as Auditor, Monitor, and Design Reviewer are user-triggered and advisory unless explicit permission is granted. |
| Software Designer Agent definition | `.opencode/agents/tera-software-designer.md` + `tera-system/TeraSubAgents.md` §6.9 | `tera-system/AGENT_ACTIVATION_MATRIX.md`, `tera-system/TeraPreExecutionGate.md`, `project-control/tasks/TASK_TEMPLATE.md` | Core mandatory sub-agent for technical design per task. Replaces `ExecutionPreparationAgent`. |
| Agent generation template | `tera-system/AGENT_GENERATION_TEMPLATE.md` | `TERA_RUNTIME_PROTOCOLS.md` | Draft agents use this source. |
| Pre/Post execution gates | `tera-system/TeraPreExecutionGate.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_PROTOCOLS.md` | Gate details stay in the policy file. |
| Token/context policy | `tera-system/TeraTokenPolicy.md` | `.opencode/agents/tera.md` | Runtime contains only startup/loading rules. |
| Runtime protocols | `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | `.opencode/agents/tera.md` | Detailed operational behavior. |
| Runtime templates | `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` | Not applicable | Formal output formats only. |
| Runtime checklists | `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` | `.opencode/agents/tera.md` | Checklists stay outside runtime. |
| MVP classification | `tera-system/runtime/MVP_DEFINITION_PROTOCOL.md` | `.opencode/agents/tera.md` | Runtime references when to load it. |
| Technology profiles | `tera-system/profiles/` | `.opencode/agents/tera.md` | Stack-specific rules stay in profiles. |
| Design governance layer | `tera-system/design-system/` | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TERA_RUNTIME_PROTOCOLS.md` | Governs Design Source Decision, DESIGN.md integration, internal kits, design tokens, component rules, and UI Acceptance Gate. |
| Project UI/UX executable guide | `project-preparation/28_UI_UX_GUIDELINES.md` using `TERA_RUNTIME_TEMPLATES.md` Section 33 | `TASK_TEMPLATE.md`, `TeraPreExecutionGate.md` | Final executable visual design rules for EngineeringAgent; raw sources stay in `project-preparation/design-source/`. |
| Engineering governance and maintainability | `tera-system/engineering-governance/` | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TeraPreExecutionGate.md`, `AGENT_GENERATION_TEMPLATE.md` | Governs module structure, file-size responsibility, UI/business-logic separation, validation, errors, permissions, testing, database/API maintainability, and agent review responsibilities. Runtime summaries must stay compact. |
| Application proposal template | `tera-workshop/client-templates/commercial/APPLICATION_PROPOSAL_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §9 | Operational Markdown template used by TCEA after Client Discovery. HTML design source currently exists in `tera-workshop/temp/APPLICATION_PROPOSAL_TEMPLATE.html`. |
| Scope of Work template | `tera-workshop/client-templates/commercial/SCOPE_OF_WORK_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | Formal SOW document — scope, deliverables, timeline, payment, assumptions, sign-off. |
| Technical Proposal template | `tera-workshop/client-templates/commercial/TECHNICAL_PROPOSAL_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | Technical architecture, stack, hosting, security, testing, CI/CD. |
| Change Request form | `tera-workshop/client-templates/contractual/CHANGE_REQUEST_FORM.md` | `.opencode/agents/tera-client-engagement.md` §12 | Change management — description, impact analysis, revised estimates, approval. |
| Software Services Agreement | `tera-workshop/client-templates/contractual/SOFTWARE_SERVICES_AGREEMENT_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | 18-section legal agreement covering scope, obligations, payment, IP, confidentiality, liability, termination. Arabic, Jordanian law. |
| Quotation | `tera-workshop/client-templates/commercial/QUOTATION_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §9 | Formal price quotation — client-facing, JOD, no taxes/fees unless explicitly stated. |
| Client Brief template (Level 1) | `tera-workshop/client-templates/pre-contract/CLIENT_BRIEF_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §9 | معلومات المشروع الأولية بعد أول مقابلة — تصنيف، تقدير مبدئي. يُنتج قبل Level 2. |
| Scope Summary template | `tera-workshop/client-templates/commercial/SCOPE_SUMMARY_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §9 | تفصيل النطاق، الميزات، Risk Buffer. يُنتج قبل Level 2. |
| Draft Quotation template (Level 2) | `tera-workshop/client-templates/commercial/DRAFT_QUOTATION_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §9 | مسودة عرض سعر — يحتاج اعتماد Majed. بنود التسعير مع تفاصيل احتساب داخلية. |
| Client Document Library | `tera-system/TeraClientEngagement.md` §12 + `tera-workshop/client-templates/` | `.opencode/agents/tera-client-engagement.md` §8 | مكتبة وثائق الزبون الرسمية — 4 فئات (pre-contract, commercial, contractual, handover). TCEA يملأ المسودات. Majed يعتمد النهائي. |
| Client Intake Form | `tera-workshop/client-templates/pre-contract/CLIENT_INTAKE_FORM.md` | `.opencode/agents/tera-client-engagement.md` §12 | استبيان أولي لجمع معلومات الشركة والمشروع قبل المقابلة. |
| Meeting Report | `tera-workshop/client-templates/pre-contract/MEETING_REPORT_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | محضر اجتماع منظم لتوثيق الحضور والقرارات والمهام. |
| NDA | `tera-workshop/client-templates/pre-contract/NDA_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | قالب اتفاقية سرية المعلومات — يتطلب مراجعة قانونية عند الاستخدام الرسمي. |
| User Persona Matrix | `tera-workshop/client-templates/pre-contract/USER_PERSONA_MATRIX_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | مصفوفة الأدوار والشخصيات لتحديد المستخدمين والميزات والصلاحيات. |
| Risk Register | `tera-workshop/client-templates/pre-contract/RISK_REGISTER_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | سجل المخاطر وخطط التخفيف مرتبط بالتحليل والـ Risk Buffer. |
| Status Report | `tera-workshop/client-templates/contractual/STATUS_REPORT_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | دوريّة التحديث للمشاريع المتوسطة/الكبيرة — نسخة تشغيلية Markdown. |
| Handover Report | `tera-workshop/client-templates/handover/HANDOVER_REPORT_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | يستخدم عند التسليم النهائي — نسخة تشغيلية Markdown. |
| Completion Certificate | `tera-workshop/client-templates/handover/COMPLETION_CERTIFICATE_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | يثبت الإغلاق والقبول النهائي — نسخة تشغيلية Markdown. |
| Project Charter | `tera-workshop/client-templates/commercial/PROJECT_CHARTER_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | وثيقة إطلاق المشروع للمشاريع المتوسطة/الكبيرة — نسخة تشغيلية Markdown. |
| Gap Analysis | `tera-workshop/client-templates/pre-contract/GAP_ANALYSIS_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | يستخدم عند تقييم نظام قائم أو ERP — نسخة تشغيلية Markdown. |
| SLA | `tera-workshop/client-templates/contractual/SLA_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | اتفاق مستوى خدمة عند وجود دعم/صيانة — نسخة تشغيلية Markdown. |
| Client Satisfaction Survey | `tera-workshop/client-templates/handover/CLIENT_SATISFACTION_SURVEY_TEMPLATE.md` | `.opencode/agents/tera-client-engagement.md` §12 | استبيان رضا العميل بعد التسليم — نسخة تشغيلية Markdown. |
| User guide | `tera-system/TERA_USER_GUIDE.md` | Not applicable | User-facing prompts and usage examples. |
| System maintenance | `tera-system/TeraSystemMaintenanceChecklist.md` | `.opencode/agents/tera.md`, `.opencode/agents/tera-system-evolution.md` | Use when modifying Tera itself. |
| Preparation documentation governance | `tera-system/TeraPreparationDocumentationGovernance.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md`, `TeraAgent.md` | Defines document taxonomy, lifecycle states, maker/checker/owner model, partial baselines, impact analysis, and consumption readiness. References from `Tera_Project_Preparation_Files.md`. |
| System evolution governance | `project-control/SYSTEM_EVOLUTION_LOG.md` + `.opencode/agents/tera-system-evolution.md` + `TeraSystemMaintenanceChecklist.md` | `.opencode/agents/tera.md` | Governs how Tera improves itself: approval-before-change, anti-bloat gate, evolution logging, and agent improvement rules. Only `TeraSystemEvolutionAgent` executes system evolution changes after owner approval. |
| Client engagement and lifecycle management | "tera-system/TeraClientEngagement.md" + ".opencode/agents/tera-client-engagement.md" | ".opencode/agents/tera.md" | TeraClientEngagementAgent يدير دورة حياة الزبون من البداية إلى النهاية، وينتج TERA_HANDOFF_PACKAGE.md لـ Tera |
| Pricing system (v0.1 Draft) | `tera-system/TeraPricingPolicy.md` | `.opencode/agents/tera-client-engagement.md` (Section 8) | نظام تسعير موحد: Feature-Based + Complexity Rubric + Risk Buffer. TCEA يصدر Draft فقط — Majed يعتمد. لا يُعرض للزبون. أسس داخلية. |
| Agent self-improvement gaps | `project-control/AGENT_GAPS_LOG.md` | `.opencode/agents/tera-system-evolution.md`, `.opencode/agents/auditor.md`, `.opencode/agents/monitor.md`, `.opencode/agents/design-reviewer.md` | Central log for core governance agents to self-report gaps, bugs, and improvement suggestions. TeraSystemEvolutionAgent reviews entries and controls their status. |
| Scenario stress tests | `tera-system/TeraScenarioStressTests.md` | Not applicable | Used for validation, not daily runtime. |
| Client/application workspace guide | `clients/README.md` | Not applicable | Folder usage guide only; policies remain in `tera-system/`. |

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
