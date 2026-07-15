# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-020

**Change Title:** TCEA Mandatory 13-Domain Client Discovery Framework

## 1. Executive Summary

This proposal studies a structured upgrade to `TeraClientEngagementAgent (TCEA)` so that external-client discovery cannot jump too quickly from basic understanding into scope, quotation, or handoff outputs.

The proposed change introduces a **fixed mandatory discovery framework** across 13 required domains, while preserving **flexible execution depth** based on project size.

The goal is not to turn discovery into a heavy questionnaire.
The goal is to prevent silent gaps.

Core proposal:

```text
Fixed Mandatory Framework
+
Flexible Execution Inside the Framework
```

Main additions under study:
- `TCEA Mandatory 13-Domain Client Discovery Framework`
- `DISCOVERY_COVERAGE_SUMMARY.md` as the operational gate artifact
- `Discovery Completeness Matrix`
- `Discovery Coverage Gate`
- `Quotation Readiness Gate`
- `Tera Handoff Readiness Gate`
- `Depth Scaling Rule`

Recommendation at this stage:

```text
Implement With Adjustments
```

because the change is needed, but should be designed to avoid small-project bloat.

---

## 2. Current Problem

Current TCEA behavior has one important governance improvement already applied:
- `Understanding Confirmation Gate`

But this gate only protects against moving forward on an **unconfirmed understanding summary**.

It does **not** guarantee that discovery coverage across the essential business, operational, technical, commercial, and handoff domains is complete enough before TCEA creates:
- `CLIENT_BRIEF.md`
- `SCOPE_SUMMARY.md`
- `FEATURE_LIST.md`
- `DRAFT_QUOTATION.md`
- `TERA_HANDOFF_PACKAGE.md`

As a result, TCEA can still move too quickly from:

```text
confirmed basic understanding
→ scope packaging
→ quotation draft
→ handoff package
```

without an explicit operational rule proving that all mandatory discovery areas were either:
- covered,
- deferred safely,
- or marked not applicable with reason.

---

## 3. Evidence From Trial

Trial reference reviewed:

```text
clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/
```

Observed strengths in the trial:
- business problem captured
- users and roles roughly captured
- scope and MVP roughly captured
- states/workflow partially captured
- preliminary pricing output produced
- handoff package produced in usable narrative form

Observed structural gaps from the same trial:

1. `CLIENT_INTAKE.md`
   - captured basics well
   - but did not use a mandatory discovery matrix
   - missing explicit per-domain completeness status
   - no structured “does this block quotation / handoff?” logic

2. `CLIENT_BRIEF.md`
   - was able to classify screens, roles, reports, integrations, notifications
   - but without a formal discovery coverage gate proving all required domains were checked

3. `SCOPE_SUMMARY.md`
   - included scope, modules, roles, dashboard, assumptions, risk buffer
   - but still lacked a formal completeness decision artifact before becoming the basis for quotation

4. `DRAFT_QUOTATION.md`
   - included price lines and exclusions
   - but the readiness basis for pricing was implicit, not governed through a quotation readiness gate

5. `TERA_HANDOFF_PACKAGE.md`
   - included scope, users, workflows, screens, data entities, reports, integrations, technical context, assumptions, risks, open questions
   - but open questions were not classified as:
     - `Blocking`
     - `Non-blocking`
     - `Deferred`
     - `Assumption`
   - and there was no explicit handoff readiness gate proving all mandatory handoff inputs were covered

6. Cross-file evidence
   - current TCEA source defines `Understanding Confirmation Gate`
   - current TCEA source does **not** define a 13-domain mandatory coverage framework
   - current pricing flow allows `Level 2` after “documented scope”, but without a structured discovery completeness matrix

---

## 4. Root Cause

The root cause is not “a missing question.”

The root cause is:

```text
No mandatory discovery coverage framework
+
No completeness matrix
+
No gate that blocks scope / quotation / handoff until mandatory domains are addressed
```

More specifically:

1. `TeraClientEngagement.md`
   - defines discovery generally
   - but does not define required domain-by-domain coverage before downstream outputs

2. `.opencode/agents/tera-client-engagement.md`
   - summarizes current workflow
   - but does not enforce coverage gates beyond understanding confirmation

3. `TeraApplicationQuestionBank.md`
   - is adaptive and broad
   - but organized in 7 domains, not the new 13-domain operational model
   - and currently acts as a reference bank, not as a completeness contract

4. `TeraPricingPolicy.md`
   - explains Level 2 Draft Quotation timing
   - but does not define a discovery-derived quotation readiness threshold

5. `TeraApplicationBlueprint.md`
   - depends on confirmed handoff quality
   - but current TCEA handoff governance does not guarantee sufficiently classified discovery coverage upstream

---

## 5. Proposed Change

Study and implement a new TCEA governance layer called:

```text
TCEA Mandatory 13-Domain Client Discovery Framework
```

This framework should become the mandatory precondition before TCEA is allowed to create or treat as operationally ready:
- scope outputs
- quotation outputs
- handoff outputs

Recommended operating model:

```text
Open client conversation
→ understanding summary + confirmation
→ adaptive discovery rounds
→ 13-domain coverage evaluation
→ DISCOVERY_COVERAGE_SUMMARY.md
→ Discovery Coverage Gate decision
→ Scope / Quotation / Handoff readiness gates
→ downstream files only after approval
```

The framework must preserve this principle:

```text
Mandatory Coverage ≠ Mandatory Deep Interview
```

---

## 6. Mandatory 13-Domain Framework

The following 13 domains should become mandatory discovery checkpoints:

1. **Business Context & Value**
2. **Integrations & APIs**
3. **Users & Roles**
4. **Workflow & Operations**
5. **Scope & MVP**
6. **Data & Content**
7. **Notifications Engine**
8. **Screens & UX**
9. **Design & Branding**
10. **Reports & Dashboards**
11. **Technical, Hosting & Compliance**
12. **Security & Audit**
13. **Acceptance, Commercials & Warranty**

Mandatory rule under study:

```text
No domain may be silently skipped.
```

Each domain must end as one of:
- `Complete`
- `Partial`
- `Missing`
- `Deferred`
- `Not Applicable`

For each non-complete domain, TCEA must record:
- reason
- impact
- blocks quotation? yes/no
- blocks handoff? yes/no
- next question needed
- temporary assumption if any
- risk level: `Low / Medium / High`

---

## 7. New Gates Required

### 7.1 Discovery Coverage Gate

Before TCEA is allowed to proceed to scope, quotation, or handoff outputs, it must produce:

```text
DISCOVERY_COVERAGE_SUMMARY.md
```

and a gate decision of one of:
- `Ready for Scope`
- `Needs More Discovery`
- `Ready for Quotation`
- `Ready for Handoff`
- `Blocked`

Transition rule:

```text
TCEA does not proceed downstream without Majed approval of the gate outcome.
```

### 7.2 Quotation Readiness Gate

`DRAFT_QUOTATION.md` must not be produced unless at minimum the following are clear enough:
- MVP Scope
- Out of Scope
- Screens estimate
- Reports / Dashboards estimate
- Integrations included / excluded
- Notifications included / excluded
- Design direction
- Technical / Hosting assumption
- Security assumptions
- Commercial risks
- Delivery assumptions

Unknowns may remain only if recorded explicitly as:
- assumption
- risk
- deferred commercial item

### 7.3 Tera Handoff Readiness Gate

`TERA_HANDOFF_PACKAGE.md` must not be produced or treated as ready unless the following are sufficiently clear:
- Business Goal
- Approved Scope
- MVP Scope
- Out of Scope
- Users / Roles
- Workflow
- Screens
- Data Entities
- Integrations
- Notifications
- Design Direction
- Reports / Dashboards
- Technical Context
- Security Notes
- Acceptance Criteria
- Commercial / Delivery Notes
- Open Questions Classification

Open questions must be tagged as:
- `Blocking`
- `Non-blocking`
- `Deferred`
- `Assumption`

---

## 8. New Rules Required

### A. Mandatory Discovery Coverage Rule

TCEA must not create or update the following as scope / pricing / handoff-ready outputs before discovery coverage is evaluated and approved:
- `CLIENT_BRIEF.md`
- `SCOPE_SUMMARY.md`
- `FEATURE_LIST.md`
- `DRAFT_QUOTATION.md`
- `TERA_HANDOFF_PACKAGE.md`

Recommended nuance:
- Draft note-taking inside `CLIENT_INTAKE.md` remains allowed
- `Level 1 Preliminary Estimate` may remain allowed for early rough commercial orientation if explicitly marked non-binding
- but `Level 2 Draft Quotation` must be blocked by readiness gate

### B. Discovery Completeness Matrix Rule

TCEA must maintain a matrix for all 13 domains.

### C. Depth Scaling Rule

All 13 domains are mandatory.
The depth is not mandatory.

Recommended scaling:
- `Small Project` → concise coverage
- `Medium Project` → moderate coverage
- `Large / Complex Project` → detailed coverage
- `Ambiguous Project` → paid discovery or separate analysis before quotation

### D. Not Applicable Rule

If a domain truly does not apply, TCEA must record:
- `Not Applicable`
- reason
- why it does not block quotation or handoff

### E. Assumption Visibility Rule

If a client answer is missing but TCEA needs to move forward with a working proposal, the point must be shown as an assumption, not silent inference.

### F. Blocking Clarity Rule

Every major gap must answer two questions:
- does it block quotation?
- does it block handoff?

---

## 9. Expected File Changes

If approved later, likely file changes include:

1. `tera-system/TeraClientEngagement.md`
   - source-of-truth update for the 13-domain framework and new gates

2. `.opencode/agents/tera-client-engagement.md`
   - compact runtime summary and mandatory operational triggers

3. `tera-system/TeraApplicationQuestionBank.md`
   - align or remap current question bank structure to the new 13-domain model

4. `tera-system/TeraClientPolicy.md`
   - add or clarify discovery coverage and approval dependencies for external client work

5. `tera-system/TeraPricingPolicy.md`
   - add quotation-readiness dependency and allowed assumptions/risk treatment

6. `tera-system/TeraApplicationBlueprint.md`
   - clarify expectations on handoff quality and open question classification from TCEA

7. `tera-system/TeraPolicyMap.md`
   - update source-of-truth references if a new file/template is introduced

8. `tera-system/TeraArchitectureMap.md`
   - update external client flow if a new mandatory discovery artifact becomes part of the flow

9. potentially `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`
   - if a formal template is introduced for discovery coverage summary or gate recording

10. potentially `tera-workshop/` templates
   - only if client-facing proposal or quotation templates need aligned placeholders

---

## 10. New Files or Templates Proposed

### Recommended new operational file

```text
clients/CLIENT-*/applications/APP-*/client-engagement/DISCOVERY_COVERAGE_SUMMARY.md
```

Recommended content:
- discovery overview
- project classification
- 13-domain completeness matrix
- unresolved questions table
- assumptions table
- quotation blockers vs handoff blockers
- gate outcome
- Majed approval status

### Optional template work

Potential template additions only if needed later:
- `DISCOVERY_COVERAGE_SUMMARY` template
- gate decision snippet template

### Anti-bloat recommendation

Do **not** introduce many new discovery files.

Preferred design:

```text
Raw discovery conversation stays in CLIENT_INTAKE.md
Coverage/gate decision lives in one operational summary file
```

This is smaller than splitting the framework into many per-domain files.

---

## 11. Impact on TCEA Runtime Behavior

Expected runtime impact if later approved:

1. TCEA will still start with open conversation
2. TCEA will still use adaptive questioning
3. TCEA will no longer be allowed to jump directly from “confirmed understanding” to scope / quotation / handoff
4. TCEA will need to explicitly evaluate discovery coverage before downstream outputs
5. TCEA will need to record blockers and assumptions visibly
6. TCEA will need to scale question depth by project size instead of using one fixed interview length

Runtime behavior shift:

```text
from:
confirmed understanding → scope docs quickly

to:
confirmed understanding → coverage evaluation → gate → downstream docs
```

---

## 12. Impact on TeraAgent

Positive impact expected:
- better handoff quality
- fewer missing requirements discovered during formal preparation
- fewer clarification loops back to Majed/TCEA
- better initial quality for `TERA_PROJECT_DECISION.md`

Operational impact:
- TeraAgent may receive richer handoff packages
- open questions may arrive better classified
- fewer downstream assumptions should leak into formal preparation unnoticed

Potential cost:
- small delay before handoff in some projects

---

## 13. Impact on ApplicationBlueprintAgent

Positive impact expected:
- stronger upstream handoff quality
- clearer business/UX/data/security context before blueprinting
- fewer `open questions` discovered too late
- less risk that blueprinting starts on weak commercial/discovery foundations

Likely specific improvement:
- handoff open questions become explicitly classified as `Blocking / Non-blocking / Deferred / Assumption`

---

## 14. Impact on Pricing Workflow

This change should **not** eliminate early commercial orientation.

Recommended pricing behavior after implementation:

### Keep allowed
- `Level 1 Preliminary Estimate`
- early rough commercial range
- explicit disclaimer that it is non-binding

### Newly constrained
- `Level 2 Draft Quotation`
- cannot proceed until quotation readiness gate passes

### Commercial gain
- fewer under-scoped quotations
- better visibility of excluded integrations/notifications/reports/security effort
- less silent commercial risk

---

## 15. Impact on Client Files Structure

Recommended structure impact is minimal:

```text
client-engagement/
├── CLIENT_INTAKE.md
├── DISCOVERY_COVERAGE_SUMMARY.md   ← proposed new operational file
├── CLIENT_BRIEF.md
├── SCOPE_SUMMARY.md
├── FEATURE_LIST.md
├── DRAFT_QUOTATION.md
├── TERA_HANDOFF_PACKAGE.md
├── CLIENT_DECISION_LOG.md
└── CHANGE_REQUEST_LOG.md
```

This keeps the new governance visible without creating multiple new per-domain files.

---

## 16. Anti-Bloat Controls

This proposal should only be implemented with the following controls:

1. **One new operational artifact maximum** for discovery coverage
2. No per-domain standalone files
3. No mandatory deep questionnaire for small projects
4. Small projects may satisfy some domains with 1–2 short answers
5. `Not Applicable` is allowed but must be justified
6. Keep client-facing outputs concise; the matrix remains internal unless needed
7. Preserve adaptive questioning; do not require all questions from the bank
8. Do not duplicate full question bank logic into runtime

Governing principle:

```text
Mandatory coverage.
Adaptive depth.
Minimal file growth.
Visible blockers.
```

---

## 17. Risks of the Change

1. **Small-project friction**
   - risk of making simple projects feel over-processed

2. **Discovery fatigue**
   - if implemented badly, TCEA may turn into a long questionnaire

3. **Runtime bloat**
   - if too many rules are copied into `.opencode/agents/tera-client-engagement.md`

4. **Commercial slowdown**
   - if Level 1 rough pricing gets blocked unnecessarily

5. **False completeness**
   - if agents mechanically mark domains “Complete” without quality checks

6. **Template overproduction**
   - if the system creates too many files for every small project

---

## 18. Risks of Not Applying the Change

1. TCEA may continue to produce scope/quotation/handoff outputs with hidden discovery gaps
2. pricing may continue to omit real effort drivers silently
3. blueprinting may begin from handoff packages that look complete but are structurally weak
4. TeraAgent may discover missing domain context later during formal preparation
5. client expectation mismatches may increase
6. small early successes may hide a bigger failure mode on medium/complex projects

---

## 19. Implementation Plan

If approved later, recommended phased implementation:

### Phase 1 — Source of Truth Design
- update `TeraClientEngagement.md`
- define the 13 domains
- define matrix statuses
- define coverage, quotation, and handoff gates
- define depth scaling rule

### Phase 2 — Runtime Sync
- update `.opencode/agents/tera-client-engagement.md`
- keep runtime compact
- add operational trigger points only

### Phase 3 — Supporting References
- update `TeraApplicationQuestionBank.md`
- update `TeraPricingPolicy.md`
- update `TeraClientPolicy.md`
- update `TeraApplicationBlueprint.md` if needed

### Phase 4 — Template / Artifact Addition
- add template or standard structure for `DISCOVERY_COVERAGE_SUMMARY.md`

### Phase 5 — Trial Validation
- test on:
  - one small clear project
  - one medium project
  - one ambiguous project

### Phase 6 — Calibration
- simplify if too heavy
- tighten if still too permissive

---

## 20. Testing Plan

Test scenarios recommended:

### Test A — Small Project
- verify all 13 domains can be covered concisely
- verify TCEA does not become bloated
- verify Level 1 estimate still possible quickly

### Test B — Medium Project
- verify matrix catches missing integrations / reports / notifications / security assumptions
- verify quotation gate blocks premature `DRAFT_QUOTATION.md`

### Test C — Complex Project
- verify deeper coverage is demanded where needed
- verify handoff open questions are classified correctly

### Test D — Ambiguous Project
- verify TCEA recommends paid discovery / separate analysis instead of forced low-confidence quotation

### Test E — Handoff Quality
- verify `ApplicationBlueprintAgent` receives cleaner handoff and fewer unresolved blockers

---

## 21. Acceptance Criteria

The change should be considered successful only if all of the following become true in trial use:

1. TCEA cannot skip any of the 13 domains silently
2. small projects remain lightweight
3. `DRAFT_QUOTATION.md` cannot be produced without documented quotation readiness
4. `TERA_HANDOFF_PACKAGE.md` cannot be produced without documented handoff readiness
5. unresolved discovery items are visible and classified
6. assumptions are recorded rather than implied
7. TeraAgent and ApplicationBlueprintAgent receive cleaner upstream inputs
8. the new process adds clarity without multiplying unnecessary files

---

## 22. Rollback Plan if Needed

If implementation later proves too heavy:

1. remove the mandatory gate enforcement
2. keep only the most useful matrix concepts as optional guidance
3. revert to understanding confirmation + adaptive discovery
4. archive or deprecate `DISCOVERY_COVERAGE_SUMMARY.md` if it becomes too heavy
5. preserve any useful question-bank improvements independently

Recommended rollback principle:

```text
Keep the insight.
Remove the excess process.
```

---

## 23. Recommendation

```text
Implement With Adjustments
```

Reason:
- the problem is real
- the requested 13-domain framework is justified
- but implementation must explicitly protect small-project speed and avoid questionnaire bloat

Recommended design stance:
- yes to mandatory coverage
- yes to coverage matrix
- yes to quotation and handoff readiness gates
- yes to adaptive depth scaling
- no to many new files
- no to mandatory deep interview on every project

---

## 24. Open Questions for Majed

1. **Level 1 Pricing Question**
   - Do you want `Level 1 Preliminary Estimate` to remain allowed before full discovery coverage, as long as it stays clearly non-binding?

2. **Coverage Artifact Question**
   - Do you want `DISCOVERY_COVERAGE_SUMMARY.md` as a separate mandatory file, or do you prefer embedding the matrix inside `CLIENT_INTAKE.md` for very small projects?

3. **Question Bank Question**
   - Do you want the question bank to be fully restructured into 13 domains, or kept internally broader while mapping operationally to the 13-domain framework?

4. **Paid Discovery Threshold Question**
   - For `Ambiguous Project`, do you want a stricter rule that blocks even `Level 2 Draft Quotation` until paid discovery is completed?

5. **Handoff Strictness Question**
   - Do you want `TERA_HANDOFF_PACKAGE.md` to be blocked by any unresolved `Blocking` question only, or also by too many `Non-blocking` unresolved items?

6. **Client-Facing Proposal Relationship Question**
   - Should the proposed discovery coverage framework remain internal to TCEA only, or should parts of it also influence the client-facing proposal package structure?

---

## 25. Practical Design Notes

Recommended practical implementation shape:

```text
CLIENT_INTAKE.md
→ raw discovery + understanding + notes

DISCOVERY_COVERAGE_SUMMARY.md
→ matrix + blockers + assumptions + gate outcome

CLIENT_BRIEF / SCOPE_SUMMARY / FEATURE_LIST
→ only after discovery coverage approval

DRAFT_QUOTATION.md
→ only after quotation readiness approval

TERA_HANDOFF_PACKAGE.md
→ only after handoff readiness approval
```

This keeps the framework operational, reviewable, and narrow enough to avoid unnecessary file sprawl.
