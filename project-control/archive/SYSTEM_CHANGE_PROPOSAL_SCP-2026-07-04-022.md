# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-04-022

**Change Title:** Final Proposal — TCEA Mandatory 13-Domain Client Discovery Framework

**Request Type:** Process Evolution / Governance Enhancement / Consulting Capability Upgrade

**Date:** 2026-07-04

**Status:** Final proposal for review before implementation

---

## 1. Executive Summary

This proposal defines the final recommended upgrade for `TeraClientEngagementAgent (TCEA)` so it can no longer move from basic client understanding into:
- scope outputs,
- quotation outputs,
- or handoff outputs

before mandatory client discovery coverage is explicitly evaluated and approved.

The proposal introduces:

```text
TCEA Mandatory 13-Domain Client Discovery Framework
```

with this governing principle:

```text
Fixed Mandatory Framework
+
Flexible Execution Inside the Framework
```

This means:
- all 13 discovery domains are mandatory
- depth is adaptive by project size
- no domain may be silently skipped
- `Not Applicable` is allowed only with reason
- gaps must state whether they block quotation or handoff
- TCEA must pass a discovery coverage gate before downstream outputs

This proposal recommends:

```text
Implement
```

because the problem is real, recurring, and now structurally clear.

---

## 2. Current Problem

TCEA currently has an `Understanding Confirmation Gate`, which protects against moving forward on a wrong high-level understanding.

However, it still lacks a **mandatory operational discovery framework** that proves all required discovery areas were reviewed before creating:
- `CLIENT_BRIEF.md`
- `SCOPE_SUMMARY.md`
- `FEATURE_LIST.md`
- `DRAFT_QUOTATION.md`
- `TERA_HANDOFF_PACKAGE.md`

Current risk pattern:

```text
basic understanding confirmed
→ scope packaging starts
→ quotation draft starts
→ handoff package starts
```

without a structured proof that:
- business value is covered
- integrations were checked
- data/content was checked
- design/UX was checked
- technical/hosting/compliance was checked
- security/audit was checked
- acceptance/commercial/warranty was checked

The problem is therefore not “one missing question”.
It is:

```text
missing discovery governance between understanding confirmation and downstream client outputs
```

---

## 3. Evidence From Trial

Reviewed trial:

```text
clients/CLIENT-alfares-maintenance/applications/APP-maintenance-requests/client-engagement/
```

### What worked
- business problem captured
- users and roles roughly captured
- workflow/statuses partially captured
- initial scope captured
- preliminary pricing produced
- handoff package produced in readable form

### What was structurally weak

1. **Design & Branding**
   - largely absent as a governed discovery area

2. **Technical / Hosting / Compliance**
   - not covered deeply enough before quotation/handoff

3. **Security & Audit**
   - absent as a required discovery gate area

4. **Acceptance Criteria**
   - not explicitly gathered as a mandatory discovery domain

5. **Data / Content detail**
   - partially covered, but without a completeness evaluation

6. **Open Questions Classification**
   - open items existed, but were not classified into:
     - `Blocking`
     - `Non-blocking`
     - `Deferred`
     - `Assumption`

7. **Quotation Readiness**
   - quotation logic existed, but readiness was implicit rather than governed

8. **Handoff Readiness**
   - handoff package was usable, but not protected by a domain completeness gate

---

## 4. Root Cause

The root cause is:

```text
No mandatory discovery coverage framework
+
No completeness matrix
+
No discovery coverage gate
+
No quotation readiness gate derived from discovery coverage
+
No handoff readiness gate derived from discovery coverage
```

Additional root-cause detail:

1. `TeraClientEngagement.md`
   - defines discovery generally, not as a mandatory 13-domain operating framework

2. `.opencode/agents/tera-client-engagement.md`
   - reflects current flow, but does not enforce coverage-based progression

3. `TeraApplicationQuestionBank.md`
   - is a useful bank, but not a discovery completeness contract

4. `TeraPricingPolicy.md`
   - defines quotation mechanics, but not the discovery readiness threshold for Level 2

5. `TeraApplicationBlueprint.md`
   - depends on upstream handoff quality, but current TCEA governance does not guarantee it strongly enough

---

## 5. Proposed Change

Introduce a mandatory TCEA framework called:

```text
TCEA Mandatory 13-Domain Client Discovery Framework
```

Recommended future flow:

```text
Open Discovery Conversation
→ Understanding Summary
→ Understanding Confirmation Gate
→ Adaptive Discovery Rounds
→ Discovery Completeness Matrix
→ DISCOVERY_COVERAGE_SUMMARY.md
→ Discovery Coverage Gate
→ Scope outputs (if approved)
→ Quotation Readiness Gate
→ DRAFT_QUOTATION.md (if approved)
→ Tera Handoff Readiness Gate
→ TERA_HANDOFF_PACKAGE.md (if approved)
```

Core principle preserved:

```text
Mandatory Coverage ≠ Mandatory Deep Interview
```

---

## 6. Mandatory 13-Domain Framework

The following 13 domains become mandatory discovery checkpoints:

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

### Domain rule

```text
No domain may be silently skipped.
```

Each domain must end in one of these states:
- `Complete`
- `Partial`
- `Missing`
- `Deferred`
- `Not Applicable`

For each domain that is not `Complete`, TCEA must record:
- reason
- impact
- blocks quotation? yes/no
- blocks handoff? yes/no
- next question needed
- temporary assumption if any
- risk level: `Low / Medium / High`

---

## 7. New Gates Required

### 7.1 Understanding Confirmation Gate

Already exists and remains required.

Purpose:
- confirm TCEA understood the client’s idea correctly before deeper downstream work

### 7.2 Discovery Coverage Gate

New required gate after understanding confirmation and before scope outputs.

Required artifact:

```text
DISCOVERY_COVERAGE_SUMMARY.md
```

Required decision:
- `Ready for Scope`
- `Needs More Discovery`
- `Ready for Quotation`
- `Ready for Handoff`
- `Blocked`

Transition rule:

```text
No scope output progresses without Majed approving discovery coverage status.
```

### 7.3 Quotation Readiness Gate

Required before `DRAFT_QUOTATION.md`.

Minimum clarity required:
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

Unknowns may remain only if explicitly recorded as:
- `Assumption`
- `Risk`
- `Deferred`

### 7.4 Tera Handoff Readiness Gate

Required before `TERA_HANDOFF_PACKAGE.md`.

Minimum clarity required:
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

Blocking rule:

```text
No handoff if unresolved Blocking questions remain.
```

---

## 8. New Rules Required

### A. Mandatory Discovery Coverage Rule

TCEA must not create or update the following as scope/pricing/handoff-ready outputs before discovery coverage is evaluated and approved:
- `CLIENT_BRIEF.md`
- `SCOPE_SUMMARY.md`
- `FEATURE_LIST.md`
- `DRAFT_QUOTATION.md`
- `TERA_HANDOFF_PACKAGE.md`

### B. Discovery Completeness Matrix Rule

TCEA must maintain a matrix covering all 13 domains.

### C. Blocking Clarity Rule

For every important gap, TCEA must answer:
- does this block quotation?
- does this block handoff?

### D. Not Applicable Rule

If a domain does not apply, TCEA must record:
- `Not Applicable`
- reason
- why it does not block quotation or handoff

### E. Assumption Visibility Rule

If the client cannot answer, TCEA may propose an assumption, but must label it explicitly.

### F. Open Questions Classification Rule

All unresolved items relevant to handoff must be classified as:
- `Blocking`
- `Non-blocking`
- `Deferred`
- `Assumption`

### G. Level 1 vs Level 2 Pricing Rule

This proposal recommends the following distinction:

#### Allowed before full coverage completion
- `Level 1 Preliminary Estimate`
- only as rough, non-binding orientation
- must remain clearly marked as non-final

#### Not allowed before quotation readiness
- `Level 2 Draft Quotation`

This protects small-project speed while still governing real pricing outputs.

---

## 9. Expected File Changes

If approved for implementation, likely files to change:

1. `tera-system/TeraClientEngagement.md`
2. `.opencode/agents/tera-client-engagement.md`
3. `tera-system/TeraApplicationQuestionBank.md`
4. `tera-system/TeraClientPolicy.md`
5. `tera-system/TeraPricingPolicy.md`
6. `tera-system/TeraApplicationBlueprint.md`
7. `tera-system/TeraPolicyMap.md`
8. `tera-system/TeraArchitectureMap.md`
9. possibly `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md`
10. possibly `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md`
11. `project-control/AGENT_GAPS_LOG.md`
12. `project-control/SYSTEM_EVOLUTION_LOG.md`

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

### Optional alternative

For very small projects only, Majed may later decide whether the matrix can be embedded into `CLIENT_INTAKE.md` instead of a separate file.

### Recommended template support

If needed later:
- template for `DISCOVERY_COVERAGE_SUMMARY.md`
- compact gate decision snippet

---

## 11. Impact on TCEA Runtime Behavior

Expected change in runtime behavior:

```text
from:
confirmed understanding → scope docs quickly

to:
confirmed understanding → discovery coverage evaluation → gate → downstream docs
```

Practical effect:
- TCEA still starts with open conversation
- TCEA still uses adaptive questioning
- TCEA still scales by project size
- TCEA can no longer skip mandatory discovery domains silently
- TCEA must show blockers and assumptions before quotation/handoff

---

## 12. Impact on TeraAgent

Expected positive impact:
- stronger handoff quality
- fewer missing requirements during formal preparation
- fewer clarification loops back to Majed/TCEA
- better initial quality for `TERA_PROJECT_DECISION.md`

Expected neutral constraint:
- TeraAgent workflow itself does not need major redesign
- but it should receive cleaner upstream discovery outputs

---

## 13. Impact on ApplicationBlueprintAgent

Expected positive impact:
- cleaner upstream handoff
- clearer business/data/UX/security/commercial context
- fewer weak assumptions during blueprinting
- better open-question classification before blueprint work starts

Recommended relationship:

```text
DISCOVERY_COVERAGE_SUMMARY.md should be available to ApplicationBlueprintAgent as an advisory upstream input.
```

It should help blueprinting, not replace the handoff package.

---

## 14. Impact on Pricing Workflow

Recommended pricing behavior after implementation:

### Keep allowed
- `Level 1 Preliminary Estimate`
- rough early commercial range
- explicit non-binding disclaimer

### Newly constrained
- `Level 2 Draft Quotation`
- blocked until quotation readiness gate passes

### Expected commercial benefit
- fewer under-scoped quotations
- more visible integration/report/security/design cost drivers
- more honest risk visibility

---

## 15. Impact on Client Files Structure

Recommended structure impact:

```text
client-engagement/
├── CLIENT_INTAKE.md
├── DISCOVERY_COVERAGE_SUMMARY.md
├── CLIENT_BRIEF.md
├── SCOPE_SUMMARY.md
├── FEATURE_LIST.md
├── DRAFT_QUOTATION.md
├── TERA_HANDOFF_PACKAGE.md
├── CLIENT_DECISION_LOG.md
└── CHANGE_REQUEST_LOG.md
```

This adds only one new operational file, which is acceptable if it replaces hidden ambiguity.

---

## 16. Anti-Bloat Controls

This proposal should only be implemented with these protections:

1. **One new discovery governance file maximum**
2. No per-domain standalone files
3. No forced deep interview for small projects
4. Small projects may satisfy some domains with 1–2 concise answers
5. `Not Applicable` must stay allowed with reason
6. Keep matrix internal; do not turn it into client-facing overload
7. Keep runtime compact; do not duplicate full framework into `.opencode/agents/tera-client-engagement.md`
8. Preserve adaptive question selection from the question bank

Governing principle:

```text
Mandatory coverage.
Adaptive depth.
Minimal file growth.
Visible blockers.
```

---

## 17. Risks of the Change

1. **Small-project slowdown**
2. **Discovery fatigue if misused as questionnaire**
3. **Runtime bloat**
4. **Over-documentation in simple projects**
5. **False completeness if domains are checked mechanically**
6. **Commercial delay if Level 1 and Level 2 are not distinguished clearly**

Mitigations:
- Question Budget
- Depth Scaling Rule
- one-file matrix design
- preserve Level 1 rough estimate path

---

## 18. Risks of Not Applying the Change

1. TCEA may continue to skip important discovery areas silently
2. pricing may continue to miss real effort drivers
3. handoff quality may remain inconsistent
4. ApplicationBlueprintAgent may continue building from incomplete upstream context
5. Majed may continue closing discovery gaps manually
6. medium/complex client projects may expose bigger failures later

---

## 19. Implementation Plan

### Phase 1 — Source of Truth
- update `TeraClientEngagement.md`
- define the 13 domains formally
- define matrix statuses
- define all new gates
- define depth scaling rule

### Phase 2 — Runtime Sync
- update `.opencode/agents/tera-client-engagement.md`
- add compact mandatory operational rules only

### Phase 3 — Supporting References
- update `TeraApplicationQuestionBank.md`
- update `TeraPricingPolicy.md`
- update `TeraClientPolicy.md`
- update `TeraApplicationBlueprint.md` if needed
- update `TeraPolicyMap.md` / `TeraArchitectureMap.md` if the new artifact becomes part of formal flow

### Phase 4 — Template / Artifact Design
- define standard shape for `DISCOVERY_COVERAGE_SUMMARY.md`

### Phase 5 — Remediation Trial
- apply to one small project
- apply to one medium project
- apply to one ambiguous project

### Phase 6 — Calibration
- simplify if too heavy
- tighten if still too permissive

---

## 20. Testing Plan

### Test A — Small Project
- verify all 13 domains can be covered concisely
- verify discovery remains lightweight
- verify Level 1 estimate remains fast

### Test B — Medium Project
- verify missing domains are caught before quotation
- verify quotation readiness gate blocks premature Level 2

### Test C — Complex Project
- verify deeper coverage is demanded only where needed

### Test D — Ambiguous Project
- verify TCEA escalates toward paid discovery / separate analysis rather than weak pricing

### Test E — Handoff Quality
- verify open questions arrive classified
- verify downstream handoff quality improves

---

## 21. Acceptance Criteria

The change is accepted only if:

1. all 13 domains become mandatory checkpoints
2. no domain can be silently skipped
3. small projects remain practically lightweight
4. `DRAFT_QUOTATION.md` cannot be produced without quotation readiness
5. `TERA_HANDOFF_PACKAGE.md` cannot be produced without handoff readiness
6. blockers vs assumptions are visibly recorded
7. downstream Tera/Blueprint inputs improve in quality
8. the system adds at most one main new operational discovery file

---

## 22. Rollback Plan if Needed

If future implementation proves too heavy:

1. remove strict gate enforcement
2. keep only the most useful completeness concepts
3. revert to understanding confirmation + adaptive discovery
4. archive `DISCOVERY_COVERAGE_SUMMARY.md` if needed
5. preserve any useful question-bank improvements separately

Rollback principle:

```text
Keep the insight.
Remove the excess process.
```

---

## 23. Recommendation

```text
Implement
```

with these execution conditions:
- preserve small-project speed
- preserve Level 1 rough estimate path
- block Level 2 quotation until readiness
- add only one main discovery governance artifact
- keep runtime compact

---

## 24. Open Questions for Majed

1. Do you want `DISCOVERY_COVERAGE_SUMMARY.md` as a separate mandatory file, or embeddable inside `CLIENT_INTAKE.md` for very small projects?
2. Do you want `Level 1 Preliminary Estimate` to remain allowed before full coverage completion, as long as it stays clearly non-binding?
3. Do you want the question bank restructured explicitly around 13 domains, or kept broader with operational mapping only?
4. For ambiguous projects, should `Level 2 Draft Quotation` be blocked completely until paid discovery is completed?
5. Should `DISCOVERY_COVERAGE_SUMMARY.md` become an advisory upstream input for `ApplicationBlueprintAgent` by default?
6. Do you want strict blocking on any unresolved `Blocking` handoff question only, or also when too many `Non-blocking` items remain unresolved?

---

## 25. Practical Final Design Shape

Recommended operational shape:

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

This is the recommended final proposal to use as the implementation basis.
