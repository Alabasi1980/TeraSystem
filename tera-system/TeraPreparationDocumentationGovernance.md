# Tera Preparation Documentation Governance Model

## 1. Purpose

This file defines how application preparation documents are **classified, created, reviewed, approved, consumed, and changed** within the Tera system.

It is the source of truth for **document lifecycle governance**. It applies to all files under `project-preparation/` and all agent activity that produces or consumes them.

### Relation to other files

| File | Role |
|---|---|
| `Tera_Project_Preparation_Files.md` | Catalog of available preparation files |
| `.opencode/agents/application-blueprint.md` | Governs blueprint artifacts before formal preparation begins |
| `.opencode/agents/tera.md` (§4.2–4.4) | Phase 3/4/5 workflow using this governance model |
| `TeraSubAgents.md` | Agent definitions that follow maker/checker rules |
| `TERA_RUNTIME_TEMPLATES.md` | Templates that include lifecycle fields |
| `AGENT_ACTIVATION_MATRIX.md` | Activation triggers respecting document readiness |

---

## 1.1 Pre-Baseline Blueprint Artifacts

Before formal preparation begins, `ApplicationBlueprintAgent` may create:

```text
project-preparation/APPLICATION_BLUEPRINT.md
project-preparation/BLUEPRINT_OPEN_QUESTIONS.md
project-preparation/BLUEPRINT_DECISION_CANDIDATES.md
project-preparation/draft-seeds/*
```

These are **pre-baseline blueprint artifacts**, not formal preparation baseline documents.

Special rules:

- `APPLICATION_BLUEPRINT.md` starts as `Draft`.
- Allowed blueprint statuses are:
  - `draft`
  - `pending_confirmation`
  - `approved_for_preparation`
  - `revision_required`
  - `blocked_by_unconfirmed_handoff`
- No preparation agent may consume blueprint artifacts downstream unless `APPLICATION_BLUEPRINT.md` has reached `approved_for_preparation`.
- `draft-seeds/` are never baseline by themselves and must not be consumed as execution-ready preparation files.

Once blueprinting is approved, Tera uses the blueprint as an upstream advisory source to create formal preparation documents, which then follow the normal lifecycle below.

---

## 2. Document Taxonomy

Preparation documents are classified along **two orthogonal dimensions**:

### 2.1 By Role in the Project Lifecycle

| Class | Meaning | Examples |
|---|---|---|
| **Intake / Client Understanding** | Captures client idea, context, and scope | `00_PROJECT_INPUTS.md`, `01_PROJECT_BRIEF.md`, `02_SCOPE_AND_BOUNDARIES.md` |
| **Structural Analysis** | Defines modules, users, workflows, data, screens | `03_MODULES_AND_FEATURES.md`, `04_USERS_ROLES_PERMISSIONS.md`, `05_BUSINESS_WORKFLOWS.md`, `06_DATA_MODEL_PREPARATION.md`, `07_SCREENS_AND_UI_STRUCTURE.md`, `12_BUSINESS_RULES.md` |
| **Cross-Cutting Rules** | Define shared constraints across all modules | `08_TECHNICAL_ARCHITECTURE.md`, `15_SECURITY_AND_ACCESS_CONTROL.md`, `21_VALIDATION_AND_ERROR_HANDLING.md`, `28_UI_UX_GUIDELINES.md` |
| **Executable Design** | Drive implementation directly | `19_DATABASE_DESIGN.md`, `20_API_CONTRACTS.md`, `28_UI_UX_GUIDELINES.md` (executable rules) |
| **Planning & Control** | Govern the execution process | `09_IMPLEMENTATION_PLAN.md`, `25_CHANGE_REQUESTS.md`, `24_CLIENT_REVIEW_NOTES.md` |
| **Late-Closure / Delivery** | Complete only before or after delivery | `10_TESTING_AND_ACCEPTANCE.md`, `11_DELIVERY_AND_HANDOVER.md`, `30_USER_MANUAL_DRAFT.md`, `31_MAINTENANCE_AND_SUPPORT.md` |

### 2.2 By Dependency Flow

| Dependency Profile | Meaning | Rules |
|---|---|---|
| **Foundation** | No dependency on other prep files | Can be created first, approved first |
| **Consumer** | Depends on one or more Foundation files | Must wait until its dependencies reach at least `Module Baseline Approved` |
| **Derived** | Built by transforming/reconciling other files | Must wait until all sources are `Module Baseline Approved` or higher |
| **Living** | Updated throughout the project lifecycle | May start early but closes late; partial baselines allowed per module |
| **Late-Bound** | Created only after execution begins or before delivery | Not needed during early phases; closes last |

---

## 3. Document Lifecycle States

Every preparation document passes through these states. Not all states are mandatory for every document — the closure condition determines its final state.

```text
                       ┌──────────────┐
                       │    Draft     │
                       └──────┬───────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Under Cross-    │
                    │ Review          │
                    └──────┬──────────┘
                           │
                    ┌──────▼──────────┐
                    │ Module Baseline │
                    │ Approved (MBA)  │
                    └──────┬──────────┘
                           │
              ┌────────────┼────────────┐
              │            │            │
              ▼            ▼            ▼
      ┌────────────┐ ┌──────────┐ ┌──────────┐
      │ System     │ │ Change   │ │ Superseded│
      │ Pending    │ │ Requested│ │          │
      │ Integration│ └────┬─────┘ └──────────┘
      └──────┬─────┘      │
             │            │
             ▼            │
      ┌────────────┐      │
      │ System     │◄─────┘
      │ Approved   │
      └──────┬─────┘
             │
             ▼
      ┌────────────┐
      │   Locked   │
      └────────────┘
```

### 3.1 State Definitions

| State | Meaning | Who Can Advance | Consumption Allowed By |
|---|---|---|---|
| **Draft** | Being written; content may be incomplete | Maker agent + Tera | No consumer agent |
| **Under Cross-Review** | Submitted for cross-document review by Checker agent(s) | Tera submits; Checker returns findings | Reviewers only |
| **Module Baseline Approved** | A defined module/wave of this document is approved for that module | Tera → Owner (for sensitive items only) | SoftwareDesignerAgent for that module; execution planning for that module |
| **System Pending Integration** | Approved at module level but not yet reconciled with all system-level documents | Tera | Limited: execution planning for the module only |
| **System Approved** | All modules approved; system-level consistency confirmed | Tera → Owner (summary only) | All consumer agents; full execution planning |
| **Locked** | No further changes expected; only Change Request can reopen | Owner | All consumers (read-only) |
| **Change Requested** | Formal change requested after baseline; impact analysis required | Owner → Tera | Only the impact analysis |
| **Superseded** | Replaced by a newer version of the same document | Tera | None (read archived copy only) |

### 3.2 Closure Conditions

| Document Type | Minimal Usable State | Final Target State |
|---|---|---|
| Intake / Client Understanding | Module Baseline Approved (single module) | System Approved |
| Structural Analysis | Module Baseline Approved (per module) | System Approved |
| Cross-Cutting Rules | Module Baseline Approved (initial version) | System Approved |
| Executable Design | Module Baseline Approved (per module) | Locked |
| Planning & Control | Module Baseline Approved | System Approved |
| Late-Closure | Draft (early) → Locked (delivery) | Locked (at delivery) |

---

## 4. Maker / Checker / Orchestrator / Owner Model

### 4.1 Roles

| Role | Who | Responsibility |
|---|---|---|
| **Maker** | Specialized preparation agent (e.g., `DataDesignAgent`, `UIUXStructureAgent`) | Writes the document following its template and assigned allowed-write-targets |
| **Checker** | A different preparation agent or coordination agent | Reviews the document for: cross-document consistency, dependency alignment, completeness against its template |
| **Orchestrator** | Tera | Plans the document, assigns Maker, assigns Checker, tracks state, detects contradictions, prepares approval packages |
| **Owner** | Majed | Approves only sensitive decisions (module boundaries, scope changes, critical Cross-Cutting Rules, System Approved promotion). Does not review every field. |

### 4.2 Cross-Review Rules

1. **Every preparation document** that advances from `Draft` to `Module Baseline Approved` must pass through `Under Cross-Review`.
2. The Checker must be an agent **different from the Maker**.
3. The Checker reviews for:
   - Consistency with upstream documents (e.g., `06_DATA_MODEL_PREPARATION.md` must align with `03_MODULES_AND_FEATURES.md` and `05_BUSINESS_WORKFLOWS.md`)
   - Completeness against the document's own required sections (from the template)
   - Edge cases and missing dependencies
4. If the Checker finds issues, the document returns to `Draft` with documented findings.
5. If the Checker approves, Tera advances it to the next state.

### 4.3 Owner Approval Points

The Owner (Majed) approves only these decision points:

| Approval Point | What Is Approved | Document State After |
|---|---|---|
| **Module scope** | What is inside/outside scope for a module | Module Baseline Approved |
| **Critical Cross-Cutting Rule** | Architecture, security model, compliance | System Approved (section) |
| **System Integration** | All modules reconciled and consistent | System Approved |
| **Change After Baseline** | A formal change request with impact analysis | Change Requested → (back to MBA or higher) |

**The Owner does not approve:**
- Every field in every document
- Routine updates after initial baseline (unless the update changes scope or cross-cutting rules)
- Checker findings or routine state transitions

---

## 5. Partial Approval / Module Baseline

### 5.1 Principle

Large projects may have multiple modules (e.g., Inventory, Sales, HR). Each module can reach `Module Baseline Approved` independently, **without waiting for all other modules**.

### 5.2 Rules

1. A `Module Baseline Approved` state means: *this module's content in this document is stable and approved for consumption.*
2. Changes to an `MBA` section require a **Documentation Change Request** with impact analysis.
3. Each module's baseline must identify:
   - Which sections/entities are covered
   - Which other documents depend on this module
   - Any open assumptions specific to this module
4. When all modules reach `MBA`, the entire document may advance to `System Pending Integration`.

### 5.3 Cross-Module Consistency

Before advancing to `System Approved`:
1. Tera orchestrates a **system-level reconciliation check**:
   - No duplicate entities across modules
   - Consistent naming and relationships
   - No orphan references (e.g., a screen that refers to a removed entity)
2. Any conflict found is resolved either by adjusting the affected modules or by raising a Documentation Gap.

---

## 6. Documentation Impact Analysis

### 6.1 When Required

A formal impact analysis is required **after** any document has reached `Module Baseline Approved` and a change is proposed that affects:

- A table, field, or relationship in the data model
- A screen, navigation path, or UI component
- A business workflow state or transition
- A business rule (validation, calculation, authorization)
- An API endpoint, request, or response
- A security permission or role
- A cross-cutting rule (architecture, design token, compliance)

### 6.2 Impact Analysis Format

When a change is proposed after baseline, the agent requesting the change must produce:

```text
Affected Documents: [list of documents that must be updated]
Affected Modules: [list of modules impacted]
Nature of Change: [addition / modification / deletion / deprecation]
Dependency Impact: [list of documents that depend on the changed content]
Open Questions: [any unresolved questions before approval]
```

### 6.3 Approval Path for Changes

| Change Severity | Approval Required |
|---|---|
| Addition within existing scope (no breaking change) | Tera (recorded in `CHANGE_REQUESTS.md`) |
| Modification affecting cross-module consistency | Tera + Owner (summary only) |
| Deletion or deprecation of existing content | Owner |
| Change affecting architecture, security, or compliance | Owner |

---

## 7. Focused Research for Documentation Gaps

### 7.1 When Research Is Allowed

Targeted web research is permitted **only** when all of the following are true:

1. A **specific documentation gap** exists (e.g., "What are the standard chart of accounts categories for a Saudi retail company?")
2. The gap **blocks** a decision required for document completeness
3. No existing Tera system knowledge, profile, or domain intelligence can resolve it

### 7.2 Research Protocol

1. The requesting agent (Maker or Checker) produces a `Research Request` with:
   - **Gap Description**: what is missing and why it is needed
   - **Research Question**: a single, precise question
   - **Source Criteria**: what kind of sources are acceptable
2. Tera or `DomainResearchAgent` executes the search.
3. The result is documented as:

```text
Research Topic: [the gap]
Sources Reviewed: [list of sources]
Findings: [what the research found]
Applicability: [whether it applies to this project's context]
Adoption Decision: Adopted / Rejected / Deferred
Reason: [why it was adopted, rejected, or deferred]
```

4. The decision is recorded in the relevant preparation document or in `project-control/DECISIONS_LOG.md`.

### 7.3 Anti-Bloat for Research

- No research without a gap.
- No research without a specific question.
- No research that duplicates prior findings (check `DECISIONS_LOG.md` first).
- No adoption without context evaluation.

---

## 8. Consumption Readiness Rules

### 8.1 Who May Consume What

| Consumer | May Consume Documents At State | Notes |
|---|---|---|
| Tera (planning) | Draft (for planning only) | Tera must not delegate execution based on Draft content |
| Preparation agents (Checker) | Draft + Under Cross-Review | For review purposes only |
| Preparation agents (Maker of downstream docs) | Module Baseline Approved (of upstream docs only) | Must check dependency profile |
| **SoftwareDesignerAgent** | **Module Baseline Approved** or higher | Must check that the document's baseline covers the module in the task |
| Execution agents (EngineeringAgent, etc.) | System Approved or Locked | Draft or MBA content must not reach implementation without Tera approval |
| QA / Testing | System Approved or Locked | Test cases derived from approved content |

### 8.2 SoftwareDesignerAgent Specific Rules

SoftwareDesignerAgent:
- Reads preparation documents that are at `Module Baseline Approved` or higher
- If a required document is at `Draft` or `Under Cross-Review`, it raises a **Design Gap** — it does not guess
- If a document is at `MBA` but does not cover the required module, it raises a **Module Coverage Gap**
- Documents at `System Approved` or `Locked` are treated as authoritative

### 8.3 What "Approved" Means for Each Phase

| Phase | Minimum Document Readiness |
|---|---|
| Phase 3 (Preparation Planning) | Documents are classified; no readiness required yet |
| Phase 4 (Delegation) | At least `Draft` for the delegation batch; `Under Cross-Review` for dependent files |
| Phase 5 (Execution Planning) | At least `Module Baseline Approved` for the modules in the execution batch |
| Phase 6 (Implementation) | At least `Module Baseline Approved`; `System Approved` preferred for cross-cutting rules |
| Phase 7 (Delivery) | All required documents at `System Approved` or `Locked` |

---

## 9. Exceptions for Small Projects

For small projects (as classified in `TERA_PROJECT_DECISION.md`):

- The full lifecycle may be simplified: `Draft` → `System Approved` → `Locked`, skipping intermediate states
- Cross-review may be done by Tera itself rather than a separate Checker agent
- Partial baselines are not needed (single module)
- The Owner may approve a single bulk approval instead of per-decision-point approvals

The project classification determines which simplification applies. The governance model is not bypassed — it is scaled.

---

## 10. Anti-Bloat Gate for Documents

Before creating any preparation document, answer:

| Question | Required |
|---|---|
| What specific gap does this document fill? | Yes |
| Can this content be merged into an existing document? | Yes |
| What is the minimal usable state for this document? | Yes |
| Is the document Foundation, Consumer, Derived, Living, or Late-Bound? | Yes |
| Does this document duplicate an existing system policy? | Must not duplicate |
| Is the document needed for the current project's classification (Small/Medium/Large/ERP)? | Yes |

**Rule:** No document is created just because it exists in the catalog. A document is created only when the gap it fills is real and cannot be absorbed by an existing document.

---

## 11. Policy Map Entry

```text
| Topic | Source of truth | Runtime summary allowed in | Notes |
|---|---|---|---|
| Preparation documentation governance | `tera-system/TeraPreparationDocumentationGovernance.md` | `.opencode/agents/tera.md`, `TERA_RUNTIME_CHECKLISTS.md` | Defines document taxonomy, lifecycle states, maker/checker/owner model, partial baselines, impact analysis, and consumption readiness |
```

## 12. Architecture Map Entry

```text
| Layer | Responsibility | Main files/folders |
|---|---|---|
| Preparation documentation governance | Governs document lifecycle, cross-review, baselines, and consumption readiness for all application preparation documents | `TeraPreparationDocumentationGovernance.md`, active application workspace `project-preparation/` |
```

---

*Governance model v1.0 — 2026-07-03*
