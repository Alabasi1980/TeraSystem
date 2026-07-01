# ISSUES_AND_GAPS.md
## CockingApp — Recipe Web Application

| Metadata | |
|----------|-|
| **Purpose** | Track all issues, gaps, risks, and open decisions discovered during the project lifecycle |
| **Last Updated** | 2026-06-30 |

---

## Open Items

| # | Severity | Category | Description | Status | Recommended Action | Found In | Target Phase |
|---|----------|----------|-------------|--------|-------------------|----------|--------------|
| IS-001 | Low | Deployment | On-premise deployment details (server specs, OS, network) not yet collected from client | 🟡 Open | Document known requirements in `22_DEPLOYMENT_AND_ENVIRONMENTS.md`; mark gaps for client clarification | Phase 1 Intake | Phase 6 |
| IS-002 | Low | Design | Claude design system `DESIGN.md` may have gaps in component states (hover, active, disabled, error) | 🟡 Open | Document any missing states in `28_UI_UX_GUIDELINES.md` as `Design Gaps`; use reasonable defaults | Phase 1 Intake | Phase 3 (Batch 4) |
| IS-003 | Info | Process | Majed requested Phase 4 formal entry via AGENT_DELEGATION_PLAN.md even when no agents needed | ✅ Documented | `AGENT_DELEGATION_PLAN.md` created and approved. This confirms Phase 4 phase discipline is maintained without unnecessary agent overhead. | Phase 3 Review | Phase 4 |
| IS-004 | Medium | Dependency Security | `npm audit` after TASK-COD-001 reports 5 moderate vulnerabilities in current Next/Prisma dependency tree; suggested `npm audit fix --force` would introduce breaking downgrades/changes | 🟡 Open | Do not run force fix now. Re-check after dependency stabilization or before production release; prefer safe package updates when available | TASK-COD-001 | Phase 6 / Before Release |

---

## Closed Items

| # | Severity | Description | Resolution | Closed Date |
|---|----------|-------------|------------|-------------|
| — | — | No closed items yet | — | — |

---

## Process

- **Critical**: Stop affected execution; inform Majed immediately
- **High**: Show to Majed before opening a new phase
- **Medium/Low**: May be deferred if linked to a later phase or TASK-ID
- **Info**: Documented for reference; no action required
