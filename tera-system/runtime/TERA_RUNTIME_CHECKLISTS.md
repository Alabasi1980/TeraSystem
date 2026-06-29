# Tera Runtime Checklists

These checklists are official runtime support material for Tera Agent.
Use them when the compact runtime file requests a verification list.

Authority rule:
If this file conflicts with `.opencode/agents/tera.md`, the active runtime file wins until the conflict is reviewed and corrected.

---

## 1. First Action in Any New Project

When the user provides a project idea and technical information:

1. Read the required system references in `tera-system/`.
2. Check `project-inputs/01_APPLICATION_IDEA.md` and `project-inputs/02_TECHNICAL_CONTEXT.md`.
3. If intake is incomplete, enter `Intake Collection Mode` and complete intake first.
4. If the user provides project-specific rules, create or update `project-preparation/PROJECT_RULES.md`.
5. Create or update `project-preparation/00_PROJECT_INPUTS.md` as a normalized summary derived from `project-inputs/`.
6. Create or update `project-preparation/TERA_PROJECT_DECISION.md`.
7. Decide which preparation files are required from `tera-system/Tera_Project_Preparation_Files.md`.
8. Create only the required files inside `project-preparation/`.
9. Decide which sub-agents are needed now and which are likely needed later.
10. If needed, generate only required draft sub-agents inside `generated-agents/opencode/`.
11. Activate inside `.opencode/agents/` only agents needed for the current approved work.
12. If a new activation happens, ask the user to restart OpenCode.
13. Wait for user approval before application implementation.

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

Reference: `tera-system/MVP_DEFINITION_PROTOCOL.md`

---

## 6. UI Design Source Protocol

Tera must not allow random or inconsistent UI styling.

Before any UI implementation, decide the UI design source with the user.

Supported modes:

| Mode | When Used | Output |
|---|---|---|
| `Tera-Decided Design` | User has no specific design. Tera asks: formal/simple/modern? Primary color? Light/Dark? RTL/LTR? | Uses clean minimal administrative UI if no preferences given |
| `User-Provided Style Files` | User provides CSS, theme files, design tokens, screenshots, or UI guidelines | Files stored under `design-source/`; agents must follow them |
| `External Design Spec` | User provides `getdesign.md`, Figma analysis, or external spec | Raw spec stored under `design-source/`; Tera summarizes rules in `28_UI_UX_GUIDELINES.md` |

If any visual design source exists or the user asks for a specific look, create or update:

```text
project-preparation/28_UI_UX_GUIDELINES.md
```

Separation rule:

```text
07_SCREENS_AND_UI_STRUCTURE.md = screen structure
28_UI_UX_GUIDELINES.md = approved UI style guide
```

`28_UI_UX_GUIDELINES.md` must define: design source mode, colors, typography, layout/spacing principles, component style rules, RTL/LTR rules, forbidden styling, and how implementation agents must apply the design.

Engineering agents must not invent new colors, spacing systems, component styles, or visual patterns unless Tera explicitly approves them.

Default rule:

```text
No UI implementation before UI design source is decided.
```

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
- Update it after closing a significant task, accepting a phase, registering a significant issue/decision, changing the roadmap, running `PlanComplianceReviewAgent`, or performing phase compaction/summary.

---

## 11. Domain Intelligence Trigger Checklist

Tera may trigger Domain Intelligence when one or more conditions exist:

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

## 19. Phased Roadmap Readiness Checklist

Before execution planning, confirm:

- [ ] Feature classification (`MVP_DEFINITION_PROTOCOL.md`) has been completed before MVP scope was set.
- [ ] Phase 1 / MVP is clear and small enough.
- [ ] Core MVP and Extended MVP are distinguished (if Extended MVP features exist).
- [ ] Later phases are separated from MVP.
- [ ] Enterprise or complex features are deferred unless explicitly approved.
- [ ] Out-of-scope items are listed where relevant.
- [ ] User decisions are captured.
- [ ] The user approved the phased roadmap.

No execution planning before phased roadmap approval.
