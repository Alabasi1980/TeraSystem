# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-16-100

```text
Title:          Add Production ERP Expert Agent and Manufacturing Knowledge Base Structure
Request Type:   Owner Improvement Request / New Domain Specialist Agent
Date:           2026-07-16
Author:         TeraSystemEvolutionAgent (حارس)
Status:         APPROVED — Majed approved execution
```

---

## Problem

Majed will work professionally as an ERP Consultant specialized in production/manufacturing and needs a dedicated personal assistant for Production ERP work. Existing Tera domain agents can research and analyze domains, but they are generic and do not provide a ready, persistent Production ERP consulting contract, interaction modes, source discipline, discovery framework, blueprint review checklist, risk rules, and manufacturing-specific output formats.

The same capability should also be available to Tera system agents when a client project includes production/manufacturing ERP modules.

---

## Evidence

Majed provided a draft agent definition for `production-erp-expert` with clear scope and intended use:

1. Personal ERP consulting support for Majed in production/manufacturing.
2. Tera system support when production modules appear in client projects.
3. Explicit non-leadership boundary: the agent does not approve final scope, pricing, accounting policy, implementation decisions, or client commitments.
4. Specialist coverage: BOM, routing, work centers, production orders, MRP, WIP, costing, inventory, quality, rework, scrap, reports, and test scenarios.
5. Local manufacturing knowledge base requirement with 14 research files that Majed will prepare.

Existing files reviewed before this proposal:

- `tera-system/TERA_AGENT_CONDUCT.md`
- `tera-system/TeraSystemMaintenanceChecklist.md`
- `tera-system/TeraPolicyMap.md`
- `tera-system/TeraArchitectureMap.md`
- `tera-system/TERA_CONTINUOUS_IMPROVEMENT_POLICY.md`
- `tera-system/AIS_PROTOCOL.md`
- `project-control/AGENT_GAPS_LOG.md`
- `tera-system/AGENT_DEPENDENCY_MAP.md`
- `tera-system/TeraSubAgents.md`
- `tera-system/AGENT_ACTIVATION_MATRIX.md`
- `tera-system/AGENT_PERMISSION_MODEL.md`
- `.opencode/agents/domain-expert-agent.md`

---

## Affected Files

| File / Folder | Change Type | Purpose |
|---|---:|---|
| `.opencode/agents/production-erp-expert.md` | Add | Active Production ERP specialist sub-agent definition |
| `tera-system/knowledge-base/manufacturing/00_INDEX.md` | Add | Manufacturing KB index and usage status |
| `tera-system/knowledge-base/manufacturing/01_MANUFACTURING_ERP_CORE_CONCEPTS.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/02_SAP_MANUFACTURING_RESEARCH.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/03_DYNAMICS_365_MANUFACTURING_RESEARCH.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/04_ORACLE_MANUFACTURING_RESEARCH.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/05_ODOO_MANUFACTURING_RESEARCH.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/06_ERPNEXT_MANUFACTURING_RESEARCH.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/07_MANUFACTURING_COSTING_GUIDE.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/08_PRODUCTION_DISCOVERY_QUESTIONS.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/09_MANUFACTURING_BLUEPRINT_CHECKLIST.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/10_PRODUCTION_TEST_SCENARIOS.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/11_QUALITY_REWORK_AND_SCRAP_GUIDE.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/12_MRP_AND_PLANNING_GUIDE.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/13_LOCAL_AND_REGIONAL_MANUFACTURING_CONTEXT.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/knowledge-base/manufacturing/14_VENDOR_COMPARISON_MATRIX.md` | Add skeleton | Local research file prepared by Majed |
| `tera-system/TeraSubAgents.md` | Update | Register the new conditional specialist agent |
| `tera-system/AGENT_ACTIVATION_MATRIX.md` | Update | Add activation trigger and ERP project matrix row |
| `tera-system/AGENT_DEPENDENCY_MAP.md` | Update | Add dependency and file-size tracking entry |
| `tera-system/AGENT_PERMISSION_MODEL.md` | Update | Add permission model for `PRODUCTION_ERP_EXPERT` |
| `.opencode/agents/tera-client-engagement.md` | Update | Allow direct TCEA use of ProductionERPExpert under existing domain-specialist exception |
| `.opencode/agents/application-blueprint.md` | Update | Allow Blueprint Mode use of ProductionERPExpert for manufacturing ERP blueprinting |
| `tera-system/TeraPolicyMap.md` | Update | Add source-of-truth entry for domain knowledge base |
| `tera-system/TeraArchitectureMap.md` | Update | Add domain knowledge layer/folder role |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | Update after execution | Record approved system change |

---

## Proposed Change

### 1. Add `production-erp-expert` as an active OpenCode sub-agent

Create `.opencode/agents/production-erp-expert.md` with:

- Conduct Gate reference.
- Identity and mission.
- Personal Mode for Majed.
- Discovery Mode for `tera-client-engagement`.
- Blueprint Mode for `application-blueprint`.
- Support Mode for `TeraAgent`, `EngineeringAgent`, and `QA/Test Agent`.
- Scope covering manufacturing models, master data, execution, inventory, costing, quality/rework, planning, and reporting.
- Source discipline labels: `Confirmed`, `Research-Based`, `Recommendation`, `Assumption`, `Open Question`, `Risk`, `Constraint`, `Decision Needed`.
- Knowledge priority: local KB first, official vendor docs second, professional standards third, targeted research fourth.
- Strict non-decision boundaries.
- Required output formats and response structure.

### 2. Add manufacturing knowledge-base structure

Create:

```text
tera-system/knowledge-base/manufacturing/
```

with `00_INDEX.md` plus 14 skeleton files. Each skeleton must clearly state:

```text
Status: DRAFT_PLACEHOLDER — Content pending Majed research.
Rule: This file is not a sufficient source until Majed fills and marks it as READY.
```

This prevents the new agent from treating empty placeholders as verified knowledge.

### 3. Register the agent in system governance files

Update:

- `TeraSubAgents.md` under conditional / Domain Intelligence agents.
- `AGENT_ACTIVATION_MATRIX.md` with a clear trigger:
  - Production/manufacturing ERP module.
  - Manufacturing discovery / blueprint review.
  - BOM/routing/MRP/WIP/costing/quality/rework questions.
- `AGENT_DEPENDENCY_MAP.md` with callers and dependencies.
- `AGENT_PERMISSION_MODEL.md` with default `READ_ONLY`, raised to `WRITE_DOCS` only for approved analysis outputs.

### 4. Update architecture/policy maps

Because this adds a new system folder role (`tera-system/knowledge-base/`), update:

- `TeraPolicyMap.md` — add source-of-truth row for domain knowledge base.
- `TeraArchitectureMap.md` — add domain knowledge/reference layer and folder role.

---

## Why This Is Necessary

1. **Professional need:** Majed needs a personal Production ERP consulting assistant for real daily ERP consulting work.
2. **System value:** Tera can reuse the specialist in manufacturing/production ERP client applications.
3. **Existing agents are too generic:** `DomainResearchAgent` researches and `DomainExpertAgent` analyzes, but neither is a persistent Production ERP specialist with built-in boundaries, discovery frameworks, costing/inventory/quality awareness, and local KB discipline.
4. **Accuracy protection:** The local-first source discipline and labels prevent unsupported guessing.
5. **Controlled integration:** The new agent remains advisory, does not lead projects, does not approve decisions, and does not execute code.

---

## Rejected Alternatives

| Alternative | Reason for Rejection |
|---|---|
| Use only `DomainExpertAgent` | Too generic for Majed's professional daily Production ERP consulting need. Would require repeating context and research each time. |
| Create only one Production ERP knowledge file | Insufficient for official work; Majed needs 14 structured research files for a serious consulting knowledge base. |
| Put knowledge files at repository root `knowledge-base/` | Creates a new root-level folder outside current system map. `tera-system/knowledge-base/` preserves architectural containment. |
| Allow EngineeringAgent to call it freely | Risk of implementation-time scope expansion. Calls must be targeted and Tera-governed unless explicitly authorized in task scope. |
| Make the agent a primary agent | Unnecessary authority expansion. It is a specialist consultant, not an orchestrator or final decision owner. |

---

## Anti-Bloat Check

| Question | Answer |
|---|---|
| What problem does this solve? | Adds a dedicated Production ERP consulting specialist for Majed and Tera production-module projects. |
| Why not just edit an existing file? | Existing generic agents cannot become domain specialists without bloating them and weakening separation of responsibilities. |
| Why not just use an existing agent? | `DomainExpertAgent` and `DomainResearchAgent` remain useful, but they do not provide persistent Production ERP consulting behavior and local KB discipline. |
| Does this reduce or increase complexity? | Adds one specialist and one domain KB folder, but reduces repeated research, repeated prompting, and production ERP analysis errors. Net benefit is justified by professional use. |
| Token impact? | Moderate. Agent file target should remain under 700 lines; KB files are loaded only on targeted need, not at runtime globally. |
| Smaller viable path? | For system-only use, fewer files would be enough. For Majed's official work, 14 research files are justified. Skeletons avoid premature content bloat. |

---

## Risk

| Risk | Mitigation |
|---|---|
| Agent overlaps with `DomainExpertAgent` | Define it as Production ERP specialist; use `DomainExpertAgent` for generic or cross-domain analysis. |
| EngineeringAgent uses it to expand implementation scope | Agent outputs are advisory; Tera remains decision owner; source labels and Decision Needed rules are mandatory. |
| Placeholder KB files are mistaken for real sources | Each skeleton states `DRAFT_PLACEHOLDER` and not sufficient until marked `READY`. |
| Knowledge base grows into unmanaged bloat | `00_INDEX.md` tracks file status and owner; no extra files beyond approved 14 without future SCP. |
| Vendor-specific behavior treated as universal ERP | Agent must separate common ERP patterns from SAP/Dynamics/Oracle/Odoo/ERPNext-specific behavior. |

---

## Rollback Plan

If the change needs rollback:

1. Delete `.opencode/agents/production-erp-expert.md`.
2. Delete `tera-system/knowledge-base/manufacturing/` skeleton files if unused.
3. Remove the Production ERP Expert entries from:
   - `TeraSubAgents.md`
   - `AGENT_ACTIVATION_MATRIX.md`
   - `AGENT_DEPENDENCY_MAP.md`
   - `AGENT_PERMISSION_MODEL.md`
   - `TeraPolicyMap.md`
   - `TeraArchitectureMap.md`
4. Add rollback note to `SYSTEM_EVOLUTION_LOG.md`.

---

## Post-Change Validation

- [ ] Anti-Bloat Gate — PASS
- [ ] Policy Map Check — PASS
- [ ] Architecture Map Check — PASS
- [ ] No client-app contamination — PASS
- [ ] No unauthorized privilege expansion — PASS
- [ ] No stale/deprecated agent references — PASS
- [ ] No duplicated mandatory rules — PASS
- [ ] Agent file size below split threshold — PASS / Plan if exceeded
- [ ] `git diff --check` — PASS
- [ ] `git status --short` reviewed
- [ ] `SYSTEM_EVOLUTION_LOG.md` updated

---

## Approval Required

Yes — Majed approval required before executing file creation and governance updates.
