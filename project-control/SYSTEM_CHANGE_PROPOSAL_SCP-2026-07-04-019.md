# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-019

**Title:** Add `ApplicationBlueprintAgent` as a Primary Session Agent for blueprinting only

**Request Type:** Owner improvement request / Core agent governance / Architecture evolution

## 1. Problem
There is a governance gap between:
- `TeraClientEngagementAgent` completing confirmed client handoff
- and `TeraAgent` starting formal preparation

Current flow lacks a dedicated agent whose sole job is to convert confirmed client handoff into a structured, high-level application blueprint before detailed preparation begins.

Without this layer:
- high-level application structure may be fragmented
- Tera may begin formal preparation without a unified blueprint artifact
- assumptions may appear too early as if they were baseline truth
- blueprinting and formal preparation may become mixed

## 2. Why This Agent Is Needed
`ApplicationBlueprintAgent` is needed to:
- bridge confirmed client handoff and governed formal preparation
- create one explicit blueprint stage
- surface open questions and decision candidates early
- reduce ambiguity in medium/complex projects
- prevent premature promotion of assumptions into baseline preparation files

## 3. Role and Boundaries
**Role:**
`ApplicationBlueprintAgent` is a **Primary Session Agent for blueprinting only**.

It:
- reads confirmed handoff
- synthesizes a high-level application blueprint
- proposes structure, risks, and decision candidates
- may produce limited draft seeds when justified

It does **not**:
- manage project execution
- replace `TeraAgent`
- replace `SolutionArchitectureAgent`
- replace `SoftwareDesignerAgent`
- finalize technical decisions

Authority model:

```text
Advisory + structuring + blueprint synthesis
Not final approval
Not baseline authority
Not execution authority
```

## 4. Activation Criteria
Activate only when:
1. `TeraClientEngagementAgent` handoff exists
2. handoff understanding is confirmed
3. project needs blueprinting before formal preparation
4. Majed explicitly starts the blueprinting session

If handoff is not confirmed, the agent must return:

```text
BLOCKED_BY_UNCONFIRMED_HANDOFF
```

Typical use:
- medium/complex external client projects
- multi-module or workflow-heavy applications
- projects where direct jump from handoff to formal preparation is risky

## 5. Inputs
Primary inputs:
- `client-engagement/CLIENT_INTAKE.md`
- `client-engagement/CLIENT_BRIEF.md` when present
- `client-engagement/SCOPE_SUMMARY.md` when present
- `client-engagement/TERA_HANDOFF_PACKAGE.md`
- approved client context and attachments
- relevant `project-inputs/` material if present

If handoff confirmation state is missing or unconfirmed, blueprinting must not proceed.

## 6. Outputs
### Primary output
```text
project-preparation/APPLICATION_BLUEPRINT.md
```

### Optional outputs
```text
project-preparation/BLUEPRINT_OPEN_QUESTIONS.md
project-preparation/BLUEPRINT_DECISION_CANDIDATES.md
```

### Optional Draft Seeds
```text
project-preparation/draft-seeds/
```

Rules:
- optional only
- must be justified
- recommended limit: **3 files maximum**
- exceeding 3 requires explicit approval
- each file must be labeled:
  - `Draft Seed`
  - `Not Baseline`
  - `Not approved for downstream execution`

## 7. Minimum Content of APPLICATION_BLUEPRINT.md
Minimum required sections:

1. Application Overview
2. Confirmed Handoff Reference
3. Blueprint Status
4. Proposed Modules / Major Capabilities
5. Proposed User Roles / Operational Actors
6. Proposed Workflow Shape
7. Proposed Screen / Interface Landscape
8. Proposed Data / Entity Landscape
9. Technical Decision Candidates
10. Risks and Constraints
11. Open Questions
12. Recommended Next Preparation Focus

Important rule:
- `APPLICATION_BLUEPRINT.md` starts as:

```text
draft
```

- it is **not** an approved formal preparation basis until status becomes:

```text
approved_for_preparation
```

## 8. Forbidden Actions
`ApplicationBlueprintAgent` must not:
- write code
- create execution tasks
- approve final technical stack
- approve final database choice
- approve final hosting choice
- approve final architecture
- manage sequencing or delegation
- run execution gates
- convert its outputs into baseline truth by itself

## 9. No Stack Finalization Rule
Programming language, framework, database, hosting, and architecture must be expressed as:

```text
candidates / recommendations / tradeoff options
```

not as:

```text
final approved implementation decisions
```

Finalization remains outside this agent and must stay with later governed review/decision flow.

## 10. Blueprint Confirmation Gate
A new gate is required.

### Rule
`TeraAgent` must not use `APPLICATION_BLUEPRINT.md` for formal preparation unless its status is:

```text
approved_for_preparation
```

### Allowed statuses
```text
draft
pending_confirmation
approved_for_preparation
revision_required
blocked_by_unconfirmed_handoff
```

### Gate question
```text
This is the proposed application blueprint based on the confirmed handoff.
Do you approve using it as the basis for formal preparation?
```

### Gate behavior
- `draft` → not usable
- `pending_confirmation` → not usable
- `revision_required` → revise first
- `blocked_by_unconfirmed_handoff` → stop; return to TCEA/Majed
- `approved_for_preparation` → usable by Tera for formal preparation

### Recording requirement
The gate result must be recorded in:
- `project-preparation/APPLICATION_BLUEPRINT.md`
- appropriate `project-control/` record(s), likely:
  - `DECISIONS_LOG.md`
  - and/or another designated control file if defined during implementation

## 11. Relationship With Other Agents
### With `TeraClientEngagementAgent`
- TCEA owns client understanding and handoff
- Blueprint Agent starts only after confirmed handoff
- if handoff is unconfirmed → `BLOCKED_BY_UNCONFIRMED_HANDOFF`

### With `TeraAgent`
- Blueprint Agent does not orchestrate
- Tera remains owner of formal preparation, sequencing, delegation, and gates
- Tera may consume blueprint only after approval

### With `SolutionArchitectureAgent`
- Blueprint Agent may propose architecture candidates
- `SolutionArchitectureAgent` remains the deeper architecture specialist
- Blueprint Agent must not finalize architecture

### With `SoftwareDesignerAgent`
- Blueprint Agent works at application level
- `SoftwareDesignerAgent` works at per-task technical specification level
- no overlap in execution-ready task design

## 12. Anti-Bloat Check
**Problem solved:** missing blueprint bridge between handoff and formal preparation

**Why not update an existing file only?**
Because this is a workflow/agent/gate gap, not a wording gap.

**Why not use an existing agent?**
No current agent fits this exact bridge role without role overlap.

**Complexity impact:**
Adds one agent and one gate, but reduces hidden ambiguity.

**Token impact:**
Moderate increase upfront; likely lower downstream rework.

**Smallest acceptable form:**
One primary blueprint agent, one main blueprint file, optional tightly-limited supporting files.

## 13. Affected Files
Likely affected files:
1. `tera-system/TeraApplicationBlueprint.md` (new)
2. `tera-system/TeraClientEngagement.md`
3. `tera-system/TeraAgent.md`
4. `tera-system/TeraArchitectureMap.md`
5. `tera-system/TeraPolicyMap.md`
6. `tera-system/Tera_Project_Preparation_Files.md`
7. `tera-system/TeraPreparationDocumentationGovernance.md`
8. `.opencode/agents/tera.md`
9. `.opencode/agents/tera-client-engagement.md`
10. `.opencode/agents/application-blueprint.md` (new)
11. `project-control/SYSTEM_EVOLUTION_LOG.md`
12. `project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-019.md`

## 14. Risks and Rollback Plan
### Risks
- overlap with `TeraAgent`
- overlap with `SolutionArchitectureAgent`
- misuse of draft outputs as baseline
- unnecessary slowdown on simple projects
- draft-seed sprawl if not constrained

### Rollback Plan
- remove `ApplicationBlueprintAgent`
- remove Blueprint Confirmation Gate
- revert map/runtime/system references
- restore flow to:

```text
TeraClientEngagementAgent → TeraAgent formal preparation
```

- keep any existing blueprint files as archival project artifacts only

## Approval Required
Yes — explicit Majed approval required before execution.
