# Tera Runtime Checklists

These checklists are official runtime support material for Tera Agent.
Use them when the compact runtime file requests a verification list.

Authority rule:
If this file conflicts with `.opencode/agents/tera.md`, the active runtime file wins until the conflict is reviewed and corrected.

---

## 1. First Action in Any New Project

When the user provides a project idea and technical information:

### Phase 1: Handoff Intake & Validation (or Client Discovery for internal projects)
1. Identify or create the isolated application workspace under `clients/CLIENT-*/applications/APP-*/`.
2. Read the required system references in `tera-system/`.
3. **Governance initialization:** Immediately after workspace creation:
   a. Create `[active application workspace]/project-control/WORKSPACE_GOVERNANCE_MODEL.md` using `TERA_RUNTIME_TEMPLATES.md` Section 40.
   b. Create or update `[active application workspace]/project-preparation/PROJECT_RULES.md` with governance rules: Auditor/Monitor/Design-Reviewer are independent sessions parallel to Tera, final authority belongs to the owner, no direct agent-to-agent communication.
   c. Rule: No new project without `WORKSPACE_GOVERNANCE_MODEL.md` + updated `PROJECT_RULES.md`.
4. Check `[active application workspace]/project-inputs/01_APPLICATION_IDEA.md` and `[active application workspace]/project-inputs/02_TECHNICAL_CONTEXT.md`.
5. If intake is incomplete, enter `Client Discovery Mode` and complete intake first.
6. If the user provides project-specific rules, create or update `[active application workspace]/project-preparation/PROJECT_RULES.md`.
7. If the project has UI, collect design preferences and sources: colors, screenshots, Figma, CSS, reference sites, RTL/LTR, brand notes.
8. Before leaving Phase 1, verify both intake files have an intake status of `Complete` or a documented Tera-approved exception explaining what remains undecided and why it does not block formal preparation.
9. For external/client-facing work, generate `client-approval/APPLICATION_PROPOSAL.html` from `tera-workshop/APPLICATION_PROPOSAL_TEMPLATE.html`; do not copy the template file itself into the application workspace.
10. Record Phase 1 creation/completion, intake file updates, proposal generation, assumptions, and remaining gaps in `project-control/PROJECT_ACTIVITY_LOG.md`.
11. Update `project-control/PROJECT_STATE.md` and `project-control/TERA_ACTIVE_CONTEXT.md` when present or needed for handoff.

### Phase 2: Project Decision Formation
1. Create or update `project-preparation/00_PROJECT_INPUTS.md` as a normalized summary derived from `project-inputs/`.
2. Create or update `project-preparation/TERA_PROJECT_DECISION.md`.

### Phase 3: Project Preparation Planning
1. Read `TERA_PROJECT_DECISION.md` — verify Decision is `Proceed to Project Preparation`.
2. Read `tera-system/Tera_Project_Preparation_Files.md` as the file catalog.
3. Create `project-control/PREPARATION_PLAN.md` using the template in `TERA_RUNTIME_TEMPLATES.md` Section 27:
   - Classify each file: Required / Conditional / Deferred / Not Required.
   - Determine creation order and dependencies.
    - Assign each file to the appropriate sub-agent.
     - Identify user approval points.
     - If UI exists, decide whether `28_UI_UX_GUIDELINES.md`, `project-preparation/design-source/`, or `UIVisualDesignerAgent` are required.
     - Decide the project Engineering Governance Level: Compact / Standard / Full using `tera-system/engineering-governance/ENGINEERING_BEST_PRACTICES.md`.
4. **No file creation. No agent generation.**
5. Present the plan for user approval.

### Phase 4: Sub-Agent Generation & Preparation Delegation
1. Verify `PREPARATION_PLAN.md` is approved. If not → do not start Phase 4.
2. Determine which preparation agents are needed now from the plan.
   - Include `UIVisualDesignerAgent` when visual design tokens/component rules must be prepared now.
3. For each agent, check status: **Use Existing** / **Specialize** / **Generate**.
4. If generating: create draft in `generated-agents/opencode/` using `AGENT_GENERATION_TEMPLATE.md`.
5. For each agent, set:
    - `Allowed Sources` and `Allowed Write Targets`.
    - `Token Budget` (Light / Medium / Strong) and `Context Rules` (Task / Summary / Full).
    - `Forbidden Actions` and `Acceptance Criteria`.
6. Create `project-control/AGENT_DELEGATION_PLAN.md` using template in `TERA_RUNTIME_TEMPLATES.md` Section 28.
7. Create or update `generated-agents/opencode/GENERATED_AGENTS_MANIFEST.md`.
8. Present delegation plan for user approval.
9. After approval: activate agents in `.opencode/agents/` per current preparation batch.
10. If activation happens, ask user to restart OpenCode.
11. Delegate **preparation-file creation only**:
    - Create `TASK-PREP-XXX`.
    - Apply Pre-Execution Gate for the preparation-file task.
    - Assign to the approved preparation agent.
12. Receive **preparation handback only**:
    - Review generated preparation files.
    - Accept, reject, or request rework.
13. **This is not application implementation.**

### Phase 5: Execution Planning
1. Run **Execution Readiness Check**:
    - [ ] All required preparation files complete and approved.
    - [ ] `AGENT_DELEGATION_PLAN.md` approved.
    - [ ] Active Technology Profile confirmed.
    - [ ] Engineering Governance Level confirmed when application code will be implemented.
    - [ ] No blocking Issues.
    - [ ] Design Source Decision resolved for any incoming UI tasks.
    - [ ] `28_UI_UX_GUIDELINES.md` exists for incoming visual UI tasks.
    - [ ] Target Version and Release Type are identified for incoming execution tasks.
    - [ ] `VERSION_REGISTRY.md` is present or intentionally N/A for one-off non-versioned work.
2. Create `project-control/PROJECT_MASTER_PLAN.md` using template Section 29:
    - Define execution phases with objectives and dependencies.
    - Include the formal phased roadmap (Core MVP / Extended MVP / Phase 2 / Later / Out of Scope).
    - Define transition conditions between phases.
    - Record Design Source Decision per phase.
3. Create `project-control/PROJECT_DETAILED_EXECUTION_PLAN.md` using template Section 30:
    - Break each phase into traceable items.
    - Link each item to a planned TASK-ID.
4. Define **First Batch** only — not the full project:
    - Select which items from the detailed plan form the first executable batch.
5. Create `project-control/EXECUTION_BATCH_PLAN.md` using template Section 31:
    - List included TASK-IDs with assigned agents and Allowed Write Targets.
    - List deferred items with reasons.
    - Record Design Source Decision for this batch.
6. For each TASK-ID in the batch:
    - Apply Orchestration Decision Matrix.
    - Apply Model Capability Gate.
    - Create task file in `project-control/tasks/TASK-COD-XXX.md`.
    - Add Target Version, Release Type, Version Scope, and Release Notes requirement.
    - Run **Pre-Execution Gate** (checklist from `TeraPreExecutionGate.md`, including Design Governance items for UI tasks).
    - Record `Pre-Execution Gate Result: PASS` in the task file.
     - For UI tasks, include UI Source / UI Rules / UI Acceptance / Design Gap Handling and link `UI_ACCEPTANCE_GATE.md`.
     - For implementation tasks that touch code architecture, modules, API, validation, permissions, database, shared utilities, or tests, apply `tera-system/engineering-governance/ENGINEERING_GOVERNANCE_GATE.md`.
6.5 **Create `IMPLEMENTATION_AGENT_STRATEGY.md`** (مطلب إلزامي قبل الانتقال إلى Phase 6):
    - أنشئ الملف في `project-control/IMPLEMENTATION_AGENT_STRATEGY.md`.
    - أجب عن جميع الأسئلة الإلزامية:
      - **Agent: من نحتاج الآن؟** — أي عميل تنفيذي مطلوب للـ Batch الحالي.
      - **Agent: من نؤجل؟** — أي عميل سنحتاجه في Batches لاحقة.
      - **Who writes: من ينفذ؟** — لكل TASK-ID في الدفعة.
      - **Who reviews: من يراجع؟** — لكل TASK-ID، هل يحتاج مراجعة مستقلة أم Tera يراجعه.
      - **Activation plan: متى يُفعّل كل عميل وبأي صلاحيات؟**
      - **Exceptions: هل يوجد استثناء للتنفيذ المباشر؟** — إذا كان Tera سينفذ كوداً مباشرة، وثق السبب.
    - سجّل القرار في `PROJECT_ACTIVITY_LOG.md`.
7. Present to user: Master Plan + Detailed Plan + Batch Plan + **IMPLEMENTATION_AGENT_STRATEGY.md** + first TASK-IDs.
8. Wait for user approval before moving to Phase 6:
    - [ ] Master Plan approved.
    - [ ] Detailed Plan approved.
    - [ ] Batch Plan approved.
    - [ ] **IMPLEMENTATION_AGENT_STRATEGY.md approved.**
9. **No coding. No UI without Design Source Decision. No TASK-ID without Pre-Execution Gate PASS. No Phase 6 without approved Implementation Agent Strategy.**

### Phase 6: Implementation

0. **Request Build Mode:** Ask the user explicitly: "Do you approve entering Build Mode for TASK-COD-XXX?" before any implementation delegation. Remain in Plan Mode until approved.
1. Select one approved `TASK-COD-XXX` from the approved `EXECUTION_BATCH_PLAN.md`.
2. Confirm:
   - [ ] Build Mode approved by user.
   - [ ] **IMPLEMENTATION_AGENT_STRATEGY.md** approved.
   - [ ] Task status is `Approved` or `Assigned`.
   - [ ] Responsible agent is active and appropriate.
     - [ ] Active Technology Profile is loaded.
     - [ ] Engineering Governance Gate is applicable or explicitly N/A for the current task.
    - [ ] `Pre-Execution Gate Result: PASS` exists in the task file.
    - [ ] User approval exists for the batch or task.
    - [ ] Target Version / Release Type exists in the task file.
    - [ ] If Hotfix, no new features are included.
3. Delegate only the current task package:
   - Task ID.
   - Objective.
   - Allowed Sources.
   - Allowed Write Targets.
   - Forbidden Actions.
   - Technology Profile.
   - Expected Output.
   - Acceptance Criteria.
4. Execute only inside `Allowed Write Targets`.
5. Require structured agent handback:
   - Task ID, Agent, Status.
   - Files Created / Modified.
   - Commands Run.
   - Summary, Assumptions, Issues, Decisions Needed, Recommendation.
6. Record handback in `project-control/tasks/TASK-COD-XXX.md`; it must not remain only in chat.
7. Run `Post-Execution Review Gate` from `TeraPreExecutionGate.md` before any acceptance/closure.
   - Include Engineering Governance review for code/module/API/database/validation/permission/test tasks.
8. Decide final task status: Accepted / Needs Fix / Blocked / Rework Needed / Deferred / Cancelled.
9. Update `TASK_REGISTRY.md`, `PROJECT_ACTIVITY_LOG.md`, `PROJECT_STATE.md`, and `ISSUES_AND_GAPS.md` when needed.
9a. Update `VERSION_REGISTRY.md` and `RELEASE_NOTES.md` when the task affects delivered scope, fixed issues, known issues, or deferred items.
10. Do not open the next task unless the current task is accepted or explicitly handled.
11. **Self-Diagnosis Checkpoint:** After every 3 closed tasks, record a compact self-diagnosis (see §1.3 in `.opencode/agents/tera.md`) before opening the 4th task.
12. When all approved implementation tasks are closed or explicitly deferred, prepare Phase 7 entry review.
13. **No implementation without approved TASK-ID. No closure without Post-Execution Review. No scope expansion. Implementation completion does not equal project closure.**

### Phase 7: Delivery, Handover & Closure

Entry Gate:

- [ ] All approved `TASK-COD-*` tasks are Closed / Accepted, or incomplete items are documented as Deferred Items.
- [ ] No undocumented Critical blockers.
- [ ] Post-Execution Reviews are complete for all accepted implementation tasks.
- [ ] Engineering governance findings are closed, accepted as known issues, or deferred with records.
- [ ] `TASK_REGISTRY.md`, `PROJECT_STATE.md`, and `ISSUES_AND_GAPS.md` are current.
- [ ] Closure Type is classified: Version / Maintenance / Hotfix / Final Application.
- [ ] `VERSION_REGISTRY.md` and `RELEASE_NOTES.md` are current when version management is active.

Allowed activities:

- Final QA Review.
- Smoke / Regression Review.
- Delivery Readiness Review.
- Documentation finalization.
- Release Notes.
- Version Registry update.
- Next Version Handoff.
- Client/User Acceptance.
- Handover Package.
- Post-Implementation Review.
- Version / Maintenance / Hotfix / Final Application Closure Decision.

Forbidden in Phase 7:

- [ ] No new scope.
- [ ] No code changes.
- [ ] No direct edits to implementation files.
- [ ] No hidden open issues.
- [ ] No undocumented Deferred Items.

Exit Gate:

- [ ] Delivery Readiness Report complete or intentionally minimized for small internal projects.
- [ ] Final Acceptance Checklist complete.
- [ ] Release Notes complete.
- [ ] Project Closure Report complete.
- [ ] VERSION_REGISTRY.md current, or N/A explicitly justified.
- [ ] NEXT_VERSION_HANDOFF.md complete unless Final Application Closure is recorded.
- [ ] Git release tag created/pushed, or explicitly deferred with reason.
- [ ] GitHub Release created, or explicitly Deferred/N/A with reason.
- [ ] Client Handover Package complete for external client projects.
- [ ] Deferred Items documented.
- [ ] Final closure decision recorded in `PROJECT_ACTIVITY_LOG.md` and `PROJECT_STATE.md`.

If Phase 7 finds a blocking issue:

1. Do not fix it inside Phase 7.
2. Create `TASK-COD-FIX-*`.
3. Return to Phase 6 for fix + review.
4. Re-enter Phase 7 after the fix is accepted.

### Version Management Checklist

Before opening a new version or post-release fix:

- [ ] Read `tera-system/runtime/VERSION_LIFECYCLE_PROTOCOL.md`.
- [ ] Read `project-control/VERSION_REGISTRY.md` if it exists.
- [ ] Read `project-control/NEXT_VERSION_HANDOFF.md` if it exists.
- [ ] Classify request: Initial / Hotfix / Patch / Minor / Major.
- [ ] Confirm affected version and target version.
- [ ] Confirm user/client approval is needed or already recorded.
- [ ] For Hotfix: verify no new features are included.
- [ ] Update `PROJECT_STATE.md` Version State.
- [ ] Create version-compatible TASK-ID format such as `TASK-COD-v1.0-001` or `TASK-COD-FIX-v1.0.1-001`.
- [ ] Update `TASK_REGISTRY.md` with Target Version and Release Type.
- [ ] Plan `RELEASE_NOTES.md` update before closure.
- [ ] If releasing a version, verify `GIT_REMOTE.md`, inspect git status/diff/log, prepare commit/tag/GitHub Release notes, and ask user approval before push/tag push/GitHub Release creation.
- [ ] Do not activate Level 3 expansion unless its trigger exists and the user approves it.

---

## 2. Tera Self-Diagnosis Checklist

Run self-diagnosis when:

- before creating or activating a sub-agent
- before starting a new project phase
- before a major delegation or multi-agent task
- when user instructions conflict with official files or prior decisions
- after 2 failed or corrected task attempts in a row
- when current state, scope, next step, or authority is unclear
- when Tera is about to ask for broad context, a stronger model, or a costly operation

Checklist:

```text
Tera Self-Diagnosis:
- Current phase:
- Current task / decision:
- Have I read PROJECT_STATE.md or TERA_ACTIVE_CONTEXT.md this session?
- Am I using official files, not chat memory only?
- Is the next step aligned with PROJECT_MASTER_PLAN.md / PROJECT_DETAILED_EXECUTION_PLAN.md when they exist?
- Are there unresolved issues, contradictions, or blocked decisions?
- Are my assumptions documented?
- Is this the smallest safe next step?
- Do I need task split, stronger model, or specialist review?
- Do I need user clarification before proceeding?
Result: PASS / UNCLEAR / BLOCKED
```

Result rules:
- `PASS`: continue.
- `UNCLEAR`: do not delegate yet; clarify, read the needed official file, or update the task package.
- `BLOCKED`: stop and ask only for the missing decision or information.

---

## 3. Surgical Editing Rules

When modifying an existing file:

- Touch only what must be changed.
- Do not refactor unrelated code, rename files, or restructure unless required.
- Do not change existing behavior unless the task explicitly requires it.
- Preserve the current style, structure, indentation, and naming conventions.
- Explain what changed and why in the handback or task file.
- Violation = `Surgical Editing Violation`; change must be reverted unless approved.

---

## 4. MVP Anti-Bloat Checklist

For small MVP projects, start with the smallest sufficient structure.

Default MVP behavior:

- Prefer one management screen per main entity.
- Combine list, filters, add, edit, details, printing, and actions into one screen unless a clear reason requires separation.
- Do not create separate add/edit/detail/status screens unless there is a clear reason.
- Generate only sub-agents required for the current approved phase.
- Delay architecture, engineering, QA, deployment, performance, compliance, and handover agents until their phase is approved.
- Avoid separate lookup, status, history, or audit tables when fixed values or simple fields are enough for the MVP.
- Treat the first draft as reducible, not final.
- After drafting preparation files, perform a reduction pass: remove, merge, or postpone anything unnecessary.

For MVP projects, report:

- initial proposed screen count
- reduced final screen count
- files merged or postponed
- agents postponed
- data structures simplified
- items moved to future phase

---

## 5. MVP Definition Classification Checklist

Use after discovery information is collected, before finalizing the MVP scope in `project-inputs/01_APPLICATION_IDEA.md`.

**Golden rule: User-selected features during discovery are not automatically MVP.**

Checklist:

- [ ] All candidate features have been collected from discovery.
- [ ] Each feature has been classified by: necessity, dependency, risk, cost, implementation size.
- [ ] Each feature is assigned to exactly one tier: Core MVP / Extended MVP / Phase 2 / Phase 3 / Later / Out of Scope.
- [ ] Core MVP features form the smallest version that delivers the primary workflow end-to-end.
- [ ] Core MVP is small enough to build in a reasonable timeframe (days to weeks).
- [ ] Extended MVP features are clearly non-blocking — the app works without them.
- [ ] Phase 2+ features depend on Core MVP being stable first.
- [ ] Later/Enterprise features are deferred unless explicitly approved for current scope.
- [ ] Out-of-scope items are documented to prevent future scope creep.
- [ ] The classification is visible in the Application Understanding Summary for user review.
- [ ] The user has approved or reclassified — Tera's default is not automatic MVP inclusion.

Reference: `tera-system/runtime/MVP_DEFINITION_PROTOCOL.md`

---

## 6. Design Governance Checklist

Tera must not allow random or inconsistent UI styling.

Official reference:

```text
tera-system/design-system/
```

Before any frontend execution planning or UI implementation, decide the design source mode:

```text
INTERNAL_TERA_KIT / GETDESIGN_MD / FIGMA_DESIGN_FILE / USER_PROVIDED_REFERENCE / EXTERNAL_URL_ANALYSIS / HYBRID / NO_UI / N/A
```

Checklist:

- [ ] Project design level selected: None / Compact / Full.
- [ ] Design Source Decision recorded.
- [ ] Raw sources saved in `project-preparation/design-source/` when applicable.
- [ ] `project-preparation/28_UI_UX_GUIDELINES.md` created for visual UI work.
- [ ] `28_UI_UX_GUIDELINES.md` includes tokens, layout rules, component rules, RTL/LTR, accessibility, forbidden styling, and implementation instructions.
- [ ] UI tasks include UI Source / UI Rules / UI Acceptance / Design Gap Handling.
- [ ] UI tasks link `tera-system/design-system/UI_ACCEPTANCE_GATE.md`.
- [ ] EngineeringAgent is instructed to raise Design Gap instead of guessing.

Separation rule:

```text
07_SCREENS_AND_UI_STRUCTURE.md = screen structure and UX/navigation
28_UI_UX_GUIDELINES.md = final executable visual design rules
project-preparation/design-source/ = raw design sources
```

Default rule:

```text
No Frontend Execution Planning without Design Source Decision.
No UI Implementation without 28_UI_UX_GUIDELINES.md when visual style matters.
```

---

## 6.5 Engineering Governance Checklist

Official reference:

```text
tera-system/engineering-governance/
```

Use this checklist before implementation planning, before delegating code tasks, during post-execution review, and before delivery readiness when the project includes application code.

Checklist:

- [ ] Engineering Governance Level selected: Compact / Standard / Full.
- [ ] Active Technology Profile loaded when stack-specific structure matters.
- [ ] Module or feature ownership is clear for the task.
- [ ] Business logic is not planned inside UI components unless explicitly justified.
- [ ] Module-specific logic is not planned inside `shared/` or generic `utils`.
- [ ] File-size / responsibility risk is considered for large components, services, schemas, or handlers.
- [ ] Validation layer is clear: UI only / backend/API / service/domain / database integrity.
- [ ] Permissions are not frontend-only when security matters.
- [ ] Database changes are traceable through approved schema/migration task path.
- [ ] API response/error behavior follows project standards when applicable.
- [ ] Important business logic has tests or a documented deferral.
- [ ] Engineering deviations are recorded as task notes, `ISSUES_AND_GAPS.md`, or `DECISIONS_LOG.md`.

Default rule:

```text
No code implementation task should PASS when it silently violates the approved engineering governance level.
```

Do not apply Full governance to Compact projects unless Tera and the user explicitly approve the extra structure.

---

## 7. Security Sensitivity Levels

Decide security sensitivity before delegation.

| Level | Meaning | Default Action |
|---|---|---|
| Low | UI-only, text/layout, no Auth/API/Server Actions/Data Mutations | `SecurityAgent` not needed by default; only required if Tera identifies a real security gap |
| Medium | Standard Server Actions, CRUD with requireAdmin, Data Mutations within existing permissions | Tera explicitly decides: required / optional but skipped / not needed |
| High | Auth flow, JWT, cookies, sessions, passwords, secrets, config, middleware, permissions model, public API endpoints | `SecurityAgent` is default; cannot skip without strong documented reason |

Rules:
- `Server Actions` and `Data Mutations` count as independent security surfaces.
- `Security Sensitivity` does not replace the post-execution `Independent Review Decision`; both are required.

---

## 8. Pre-Execution Gate Checklist

Before any implementation task is approved, assigned, or executed, apply:

```text
tera-system/TeraPreExecutionGate.md
```

Mandatory rule:

```text
No implementation delegation without Pre-Execution Gate PASS.
```

Tera must add a `Pre-Execution Gate Result` section to every implementation task.

If `NEEDS_REVISION`, Tera revises by itself before asking user approval.
If `BLOCKED`, Tera stops and asks only for the missing decision or information.
Tera must not require the user to discover detailed technical scope mistakes.

Default first technical task, scaffold restrictions, ORM/schema rules, and database apply limits must come from the active Technology Profile.

General database-layer rule:

```text
Schema definitions may define field types and relations.
Business validation rules such as amount > 0 must not be implemented as database constraints unless explicitly approved.
```

General secret rule:

```text
Real secrets are allowed only in approved local environment files or environment variables.
They must never be written into task files, logs, handbacks, or config/code fallback values.
```

---

## 9. Task Prioritization Matrix

When multiple tasks are ready and the user has not specified an order, Tera chooses the next task using this priority order.

| Priority | Meaning | Examples |
|---|---|---|
| P0 Critical | Blocks all or protects the project | broken auth, exposed secret, failed build, blocking setup |
| P1 High | Core MVP / unlocks later work | main data model, auth, core screen, required validation |
| P2 Medium | Important but not blocking | secondary filters, reports, quality fixes |
| P3 Low | polish or minor improvement | visual polish, small UX improvement |
| P4 Deferred | later phase | future enhancements, nice-to-have features |

Ordering rules:

1. Follow explicit user order when provided.
2. Fix P0 blockers before feature work.
3. Prefer dependencies that unblock more work.
4. Prefer higher-risk or security-sensitive tasks before lower-risk polish.
5. Prefer MVP acceptance blockers before enhancements.
6. If two tasks are equal, choose the smaller safer task first.
7. If a high-priority task is blocked, choose the next highest unblocked task.

---

## 10. PROJECT_STATE.md Minimum Content

`project-control/PROJECT_STATE.md` is the compact project memory.

Must contain at minimum:

- Current phase.
- Approved decisions.
- Active technical stack.
- Completed tasks and screens.
- Active and inactive sub-agents.
- Open risks or issues.
- Current roadmap position.
- Next recommended step.

Rules:
- It is a context gateway, not a replacement for detailed files.
- Update it before leaving any project phase, after closing a significant task, accepting a phase, registering a significant issue/decision, changing the roadmap, running `PlanComplianceReviewAgent`, or performing phase compaction/summary.

---

## 11. Domain Intelligence and Research Trigger Checklist

### When to trigger formal Domain Intelligence

Tera may trigger full Domain Intelligence (Research Brief + agents) when one or more conditions exist:

- ERP module or deep business domain.
- Screen or feature has workflow, approval, permissions, or cross-module integration.
- Domain is procurement, inventory, accounting, manufacturing, HR, projects, compliance, or another rule-heavy business area.
- User requests best practices.
- User requests alignment with SAP, Oracle, Odoo, Dynamics, or another reference system.
- Current domain understanding is incomplete and wrong analysis could cause major rework.
- Feature depends on current or source-grounded external knowledge.

Do not trigger Domain Intelligence for:
- simple CRUD
- small UI edits
- bug fixes
- adding a filter or button
- purely technical tasks
- domains already sufficiently documented in official project files

### When to trigger Quick Search (during Client Discovery)

A Quick Search (real-time, no formal brief) is triggered when:
- The client mentions an integration, API, or third-party service.
- The client asks about hosting providers, pricing, or regional availability.
- The question in the Question Bank is marked `🔍 Research recommended` and the client defers or asks for a recommendation.
- Majed explicitly says "ابحث عن [موضوع]" during the conversation.
- Tera does not have source-grounded knowledge about a specific technology, library, or approach relevant to the client's context.

### When to trigger Focused Research (between discovery and preparation)

Focused Research is triggered when:
- A specific decision needs comparison (e.g., which payment gateway, which cloud provider).
- The user requests a comparison with sources.
- The topic is bounded and can be resolved in one research round.

---

## 12. Source Reliability Checklist

Source tiers:

| Tier | Source Type |
|---|---|
| Tier 1 | Official documentation: SAP, Oracle, Microsoft, Odoo, standards, government/business authorities |
| Tier 2 | Books, whitepapers, professional articles, reputable implementation guides |
| Tier 3 | General blogs, practitioner notes, community examples |
| Forbidden | Sources without links, weak forums, unverified marketing content, copied or unclear material |

Rules:

- Tier 3 cannot define mandatory scope alone.
- Prefer Tier 1 when reference-system alignment is requested.
- Record source confidence and conflicting findings.
- Do not use unverifiable claims as requirements.

---

## 13. Reference-System Safety Checklist

SAP, Oracle, Odoo, Dynamics, and similar systems may be used as reference sources, not mandatory blueprints.

Allowed reference use:

- terminology
- common workflow concepts
- statuses
- approval patterns
- integration points
- best-practice warnings

Do not copy by default:

- complete enterprise workflows
- full MRP behavior
- multi-level release strategies
- budget commitment accounting
- advanced supplier or contract automation
- any feature that exceeds approved project scope

Enterprise-grade features default to `Later` unless explicitly approved.

---

## 14. Domain Anti-Bloat Checklist

Before accepting any domain recommendation, ask:

- Is it required for the current approved phase?
- Is it essential for MVP?
- Is it enterprise-only?
- Does it require integration not yet approved?
- Can it be deferred safely?
- Does it conflict with `PROJECT_RULES.md`?
- Does it need a user decision?

Every recommendation must be classified as:

```text
Include now / Recommended / Defer / Out of Scope / Needs User Decision
```

---

## 15. Application Discovery Questions Checklist

Ask in short batches. Do not ask all questions at once.

Core discovery areas:

- What is the application idea?
- What problem does it solve?
- Who will use it?
- What should the first useful version do?
- What is explicitly not needed now?
- What workflows, approvals, statuses, or reports are expected?
- Are there roles or permissions?
- Are there external systems or integrations?
- Are there design, language, RTL/LTR, color, or UX preferences?
- Is there a preferred technology stack?
- Is the app internal, customer-facing, public, or multi-tenant?
- Are there reference systems or examples such as SAP, Odoo, Oracle, Dynamics, or screenshots?

Smart domain questions may be asked after the app is sufficiently understood, not before.

---

## 16. Discovery Documentation Checklist

Before leaving Application Discovery, confirm:

- Application idea is documented.
- Problem / purpose is documented.
- Target users are documented.
- Main workflows or expected actions are documented.
- MVP candidates are documented.
- Later-phase candidates are documented.
- Out-of-scope items are documented when known.
- Technical context is documented or explicitly marked missing.
- User preferences are documented.
- Assumptions are documented.
- Open questions are documented.
- Materially important chat-only information has been normalized into official `project-inputs` files.
- `01_APPLICATION_IDEA.md` and `02_TECHNICAL_CONTEXT.md` are marked `Complete`, or any exception is explicitly documented with status, blocker, owner, and reason it does not block formal preparation.
- `PROJECT_ACTIVITY_LOG.md` records discovery completion and all material intake/proposal updates.
- `PROJECT_STATE.md` records the current phase result and next recommended step.

Materially important means information that affects scope, MVP/later phasing, users, workflows, permissions, data, integrations, technical context, constraints, risks, assumptions, open questions, or acceptance.

Do not block discovery just to preserve every minor phrase, aside, preference wording, or non-impactful detail.

If materially important application information remains only in chat, discovery cannot close.

---

## 17. Suggestion Timing Checklist

Before Tera proposes significant improvements, confirm:

- Tera can summarize the application accurately.
- The user has had a chance to correct the understanding.
- The suggestion is tied to the user's goals or a real domain risk.
- The suggestion is classified as MVP / Later / Out of Scope / Needs User Decision.
- The suggestion is presented as an option, not as automatic scope.

Do not propose major scope expansions before sufficient understanding.

---

## 18. Post-Research Review Checklist

After Domain Intelligence or external research:

- Did the research introduce new useful ideas?
- Did it reveal missing workflow, role, validation, or integration issues?
- Did it suggest enterprise features that should be deferred?
- Did any finding conflict with user intent or `PROJECT_RULES.md`?
- Did Tera classify findings into Include now / Recommended / Defer / Out of Scope / Needs User Decision?
- Did Tera return to the user to review meaningful changes before adopting them?

Research findings cannot become approved scope without user/Tera decision.

---

## 19. Phased Roadmap / Master Plan Readiness Checklist

Before detailed execution planning or `TASK-COD-*` generation, confirm:

- [ ] Feature classification (`tera-system/runtime/MVP_DEFINITION_PROTOCOL.md`) has been completed before MVP scope was set.
- [ ] Phase 1 / MVP is clear and small enough.
- [ ] Core MVP and Extended MVP are distinguished (if Extended MVP features exist).
- [ ] Later phases are separated from MVP.
- [ ] Enterprise or complex features are deferred unless explicitly approved.
- [ ] Out-of-scope items are listed where relevant.
- [ ] User decisions are captured.
- [ ] The preliminary phased roadmap from discovery has been reviewed.
- [ ] `PROJECT_MASTER_PLAN.md` includes the formal phased roadmap.
- [ ] The user approved `PROJECT_MASTER_PLAN.md`.

No detailed execution planning or `TASK-COD-*` generation before `PROJECT_MASTER_PLAN.md` approval.

---

## 20. Cross-Verification Checklist

Before execution planning (Phase 5) and again before each new batch execution (Phase 6), cross-verify consistency between preparation files and control records.

### Why

During CockingApp development, a discrepancy was discovered: `EXECUTION_BATCH_PLAN.md` stated "9 models" while `19_DATABASE_DESIGN.md` contained 7 models + 1 enum. This required correcting 8 files. This checklist prevents such discrepancies from reaching execution.

### When to run

- **Checkpoint A:** After all preparation files are complete (end of Phase 3/4), before execution planning (Phase 5).
- **Checkpoint B:** Before each new batch execution (Phase 6), after the previous batch may have changed plans.
- **Checkpoint C:** Before Phase 7 closure, to ensure final records match reality.

### Checklist

| # | Check | Reference Files | What to verify |
|---|-------|----------------|----------------|
| 1 | Model/entity count | `06_DATA_MODEL_PREPARATION.md`, `19_DATABASE_DESIGN.md`, `EXECUTION_BATCH_PLAN.md`, `PROJECT_MASTER_PLAN.md` | All files agree on the total number of models/entities/tables. |
| 2 | Screen count | `07_SCREENS_AND_UI_STRUCTURE.md`, `PROJECT_MASTER_PLAN.md`, `EXECUTION_BATCH_PLAN.md` | Screen count matches across structure definition, roadmap, and batch plan. |
| 3 | Module/feature count | `03_MODULES_AND_FEATURES.md`, `09_IMPLEMENTATION_PLAN.md`, `PROJECT_MASTER_PLAN.md` | All modules listed in scope appear in implementation plan and master plan. |
| 4 | User roles | `04_USERS_ROLES_PERMISSIONS.md`, `07_SCREENS_AND_UI_STRUCTURE.md`, `15_SECURITY_AND_ACCESS_CONTROL.md` | Roles defined in security match screen-level role assignments. |
| 5 | API endpoints | `08_TECHNICAL_ARCHITECTURE.md`, `09_IMPLEMENTATION_PLAN.md` | Endpoint inventory matches implementation scope. |
| 6 | TASK-ID coverage | `PROJECT_DETAILED_EXECUTION_PLAN.md`, `TASK_REGISTRY.md` | All planned TASK-IDs exist in registry and vice versa. |
| 7 | Agent-task mapping | `IMPLEMENTATION_AGENT_STRATEGY.md`, `EXECUTION_BATCH_PLAN.md` | Every TASK-ID has an assigned agent and the agent is activated or scheduled. |
| 8 | Design source vs UI work | `07_SCREENS_AND_UI_STRUCTURE.md`, `PROJECT_MASTER_PLAN.md` | Every UI batch/phase has a Design Source Decision. |
| 9 | Version scope | `PROJECT_MASTER_PLAN.md`, `VERSION_REGISTRY.md` | Items listed as "in scope" for current version match the roadmap scope. |
| 10 | Out-of-scope items preserved | `02_SCOPE_AND_BOUNDARIES.md`, `35_ROADMAP_AND_FUTURE_PHASES.md`, `PROJECT_MASTER_PLAN.md` | Items explicitly excluded from scope are not accidentally re-included. |

### Recording

- For Checkpoint A: Record results in `PROJECT_ACTIVITY_LOG.md`.
- For Checkpoint B: Record results in the task file or batch plan notes.
- For Checkpoint C: Record results in `DELIVERY_READINESS_REPORT.md`.

### Result rules

```text
ALL MATCH:  Proceed.
MISMATCH FOUND:
  - If clearly wrong value in one file → correct it, log the correction.
  - If unclear which file is correct → stop, investigate, confirm with user.
  - If discrepancy affects scope or feasibility → treat as ISSUES_AND_GAPS item.
  - Must resolve before execution if the mismatch affects the current batch.
```

### Correction record format

When correcting a cross-verification mismatch, record this in the activity log:

```text
Cross-Verification Correction
- Item: [check #]
- Discrepancy: [file A says X, file B says Y]
- Correction applied to: [list of files]
- Resolution: [accepted value]
- Impact: [none / minor / affected batch plan]
```
