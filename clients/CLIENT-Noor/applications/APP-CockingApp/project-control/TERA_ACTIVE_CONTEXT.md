# TERA_ACTIVE_CONTEXT.md
## CockingApp — Session Handoff

| Metadata | |
|----------|-|
| **Application** | APP-CockingApp |
| **Client** | CLIENT-Noor |
| **Session Date** | 2026-06-30 |
| **Last Phase** | Phase 5 Complete — Execution Planning ready |
| **Next Phase** | Phase 6 — TASK-COD-001 after OpenCode restart |
| **User** | Majed (Owner) |
| **Status** | 🟢 TASK-COD-001 Accepted / Closed; ready to prepare TASK-COD-002 |

---

## 1. Current State

- **Phases 1–5**: ✅ Complete
- **Phase 3 Execution**: ✅ Complete — All 22 preparation files created across 7 batches
- **Phase 4 Delegation**: ✅ Complete — No sub-agents needed for preparation
- **Phase 5 Execution Planning**: ✅ Complete — Master, detailed, and batch plans created
- **Phase 5.1 Implementation Agent Strategy**: ✅ Approved — Option B
- **Active B1 Agent**: `cockingapp-foundation-engineering`
- **Disabled Agent**: `cockingapp-engineering`
- **TASK-COD-001**: ✅ Accepted / Closed
- **TASK-COD-002**: 🟡 Draft / Pre-Execution Gate PASS — awaiting delegation approval
- **Agent**: `cockingapp-data-prisma` ✅ Activated in `.opencode/agents/`
- **Open Issue**: IS-004 dependency audit (Medium) — non-blocking before production
- **Next**: Restart OpenCode → delegate TASK-COD-002 to `cockingapp-data-prisma`

---

## 2. Key Files at a Glance

| File | Path |
|------|------|
| Application Idea | `project-inputs/01_APPLICATION_IDEA.md` |
| Technical Context | `project-inputs/02_TECHNICAL_CONTEXT.md` |
| Design System | `project-preparation/design-source/DESIGN.md` |
| Project Decision | `project-control/TERA_PROJECT_DECISION.md` |
| Preparation Plan | `project-control/PREPARATION_PLAN.md` (✅ Approved v2) |
| Agent Delegation | `project-control/AGENT_DELEGATION_PLAN.md` |
| Project State | `project-control/PROJECT_STATE.md` |
| Activity Log | `project-control/PROJECT_ACTIVITY_LOG.md` |
| Client Proposal | `client-approval/APPLICATION_PROPOSAL.html` (✅ Client Approved) |

---

## 3. Preparation Files Created (22/22 ✅)

| Batch | Files | Status |
|-------|-------|--------|
| 1 — Foundation | PROJECT_RULES, 00_INPUTS, 01_BRIEF, 02_SCOPE | ✅ |
| 2 — Core Structure | 03_MODULES, 04_USERS, 05_WORKFLOWS | ✅ |
| 3 — Data Layer | 06_DATA_MODEL, 19_DATABASE | ✅ |
| 4 — Screens & Design | 07_SCREENS, 28_UI_UX | ✅ |
| 5 — Architecture & Rules | 08_ARCHITECTURE, 12_BUSINESS_RULES, 15_SECURITY, 21_VALIDATION | ✅ |
| 6 — Operations | 13_REPORTS, 22_DEPLOYMENT, 18_IMPORT_EXPORT | ✅ |
| 7 — Plans & Delivery | 09_IMPL_PLAN, 10_TESTING, 11_DELIVERY, 35_ROADMAP | ✅ |

## 4. Phase 5 Files Created ✅

| File | Status | Purpose |
|------|--------|---------|
| `PROJECT_MASTER_PLAN.md` | ✅ Created | الخطة الرئيسية، milestones، و 18 TASK-ID |
| `PROJECT_DETAILED_EXECUTION_PLAN.md` | ✅ Created | تفاصيل كل TASK-ID والملفات المستهدفة ومعايير القبول |
| `EXECUTION_BATCH_PLAN.md` | ✅ Created | تقسيم التنفيذ إلى 15 دفعة قابلة للمراجعة |

## 4.1 Next Action

**Delegate TASK-COD-002**

Ready state:
1. ✅ `TASK-COD-001` closed
2. ✅ `cockingapp-data-prisma` agent activated in `.opencode/agents/`
3. ✅ `TASK-COD-002.md` created with full definition
4. ✅ Pre-Execution Gate: PASS
5. ✅ User approval received
6. 🔜 **Restart OpenCode** → then Tera will delegate TASK-COD-002

---

## 5. Critical Reminders

- All files in Arabic (RTL)
- Design system: Claude (cream #faf9f5 / coral #cc785c / dark navy #181715)
- Tech: Next.js App Router + TypeScript + Prisma + PostgreSQL
- MVP: 18 TASK-IDs — Core 1A (12 tasks) + Extended 1B (6 tasks)
- Phase 5 is complete
- `IMPLEMENTATION_AGENT_STRATEGY.md` approved with Option B
- First implementation batch: B1 / `TASK-COD-001` only
- Use only `cockingapp-foundation-engineering` for B1
- Do not use disabled `cockingapp-engineering`
- `nextjs-prisma` profile rule: first task must not add Prisma models, migrations, db push, UI, API, or Auth
