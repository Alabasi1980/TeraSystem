# TERA_ACTIVE_CONTEXT.md
## CockingApp â€” Session Handoff

| Metadata | |
|----------|-|
| **Application** | APP-CockingApp |
| **Client** | CLIENT-Noor |
| **Session Date** | 2026-07-01 |
| **Last Phase** | Phase 5 Complete â€” Execution Planning ready |
| **Next Phase** | Phase 6 â€” TASK-COD-001 after OpenCode restart |
| **User** | Majed (Owner) |
| **Status** | ًںں¢ TASK-COD-001 Accepted / Closed; ready to prepare TASK-COD-002 |

---

## 1. Current State

- **Phases 1â€“5**: âœ… Complete
- **Phase 3 Execution**: âœ… Complete â€” All 22 preparation files created across 7 batches
- **Phase 4 Delegation**: âœ… Complete â€” No sub-agents needed for preparation
- **Phase 5 Execution Planning**: âœ… Complete â€” Master, detailed, and batch plans created
- **Phase 5.1 Implementation Agent Strategy**: âœ… Approved â€” Option B
- **Active B1 Agent**: `cockingapp-foundation-engineering`
- **Disabled Agent**: `cockingapp-engineering`
- **TASK-COD-001**: âœ… Accepted / Closed
- **TASK-COD-002**: ًںں، Draft / Pre-Execution Gate PASS â€” awaiting delegation approval
- **Agent**: `cockingapp-data-prisma` âœ… Activated in `.opencode/agents/`
- **Governance Sessions**: `auditor`, `monitor`, `design-reviewer` added and controlled manually by Majed
- **Open Issue**: IS-004 dependency audit (Medium) â€” non-blocking before production
- **Next**: Restart OpenCode â†’ delegate TASK-COD-002 to `cockingapp-data-prisma`

---

## 2. Key Files at a Glance

| File | Path |
|------|------|
| Application Idea | `project-inputs/01_APPLICATION_IDEA.md` |
| Technical Context | `project-inputs/02_TECHNICAL_CONTEXT.md` |
| Design System | `project-preparation/design-source/DESIGN.md` |
| Project Decision | `project-control/TERA_PROJECT_DECISION.md` |
| Preparation Plan | `project-control/PREPARATION_PLAN.md` (âœ… Approved v2) |
| Agent Delegation | `project-control/AGENT_DELEGATION_PLAN.md` |
| Project State | `project-control/PROJECT_STATE.md` |
| Activity Log | `project-control/PROJECT_ACTIVITY_LOG.md` |
| Workspace Governance | `project-control/WORKSPACE_GOVERNANCE_MODEL.md` |
| Client Proposal | `client-approval/APPLICATION_PROPOSAL.html` (âœ… Client Approved) |

---

## 3. Preparation Files Created (22/22 âœ…)

| Batch | Files | Status |
|-------|-------|--------|
| 1 â€” Foundation | PROJECT_RULES, 00_INPUTS, 01_BRIEF, 02_SCOPE | âœ… |
| 2 â€” Core Structure | 03_MODULES, 04_USERS, 05_WORKFLOWS | âœ… |
| 3 â€” Data Layer | 06_DATA_MODEL, 19_DATABASE | âœ… |
| 4 â€” Screens & Design | 07_SCREENS, 28_UI_UX | âœ… |
| 5 â€” Architecture & Rules | 08_ARCHITECTURE, 12_BUSINESS_RULES, 15_SECURITY, 21_VALIDATION | âœ… |
| 6 â€” Operations | 13_REPORTS, 22_DEPLOYMENT, 18_IMPORT_EXPORT | âœ… |
| 7 â€” Plans & Delivery | 09_IMPL_PLAN, 10_TESTING, 11_DELIVERY, 35_ROADMAP | âœ… |

## 4. Phase 5 Files Created âœ…

| File | Status | Purpose |
|------|--------|---------|
| `PROJECT_MASTER_PLAN.md` | âœ… Created | ط§ظ„ط®ط·ط© ط§ظ„ط±ط¦ظٹط³ظٹط©طŒ milestonesطŒ ظˆ 18 TASK-ID |
| `PROJECT_DETAILED_EXECUTION_PLAN.md` | âœ… Created | طھظپط§طµظٹظ„ ظƒظ„ TASK-ID ظˆط§ظ„ظ…ظ„ظپط§طھ ط§ظ„ظ…ط³طھظ‡ط¯ظپط© ظˆظ…ط¹ط§ظٹظٹط± ط§ظ„ظ‚ط¨ظˆظ„ |
| `EXECUTION_BATCH_PLAN.md` | âœ… Created | طھظ‚ط³ظٹظ… ط§ظ„طھظ†ظپظٹط° ط¥ظ„ظ‰ 15 ط¯ظپط¹ط© ظ‚ط§ط¨ظ„ط© ظ„ظ„ظ…ط±ط§ط¬ط¹ط© |

## 4.1 Next Action

**Delegate TASK-COD-002**

Ready state:
1. âœ… `TASK-COD-001` closed
2. âœ… `cockingapp-data-prisma` agent activated in `.opencode/agents/`
3. âœ… `TASK-COD-002.md` created with full definition
4. âœ… Pre-Execution Gate: PASS
5. âœ… User approval received
6. ًں”œ **Restart OpenCode** â†’ then Tera will delegate TASK-COD-002

---

## 5. Critical Reminders

- All files in Arabic (RTL)
- Design system: Claude (cream #faf9f5 / coral #cc785c / dark navy #181715)
- Tech: Next.js App Router + TypeScript + Prisma + PostgreSQL
- MVP: 18 TASK-IDs â€” Core 1A (12 tasks) + Extended 1B (6 tasks)
- Phase 5 is complete
- `IMPLEMENTATION_AGENT_STRATEGY.md` approved with Option B
- `WORKSPACE_GOVERNANCE_MODEL.md` approved by Majed; governance sessions are manual, not automatic
- New OpenCode agents added: `auditor`, `monitor`, `design-reviewer`; restart OpenCode before use
- First implementation batch: B1 / `TASK-COD-001` only
- Use only `cockingapp-foundation-engineering` for B1
- Do not use disabled `cockingapp-engineering`
- `nextjs-prisma` profile rule: first task must not add Prisma models, migrations, db push, UI, API, or Auth
