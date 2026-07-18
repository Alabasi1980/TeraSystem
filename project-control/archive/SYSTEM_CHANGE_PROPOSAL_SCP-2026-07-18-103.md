# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-18-103

```text
Title:          Corrective Rebuild — Professional .NET/C# Specialized Engineering Agent
Request Type:   Owner Correction Request / Research-Based Agent Quality Upgrade
Date:           2026-07-18
Author:         TeraSystemEvolutionAgent (حارس)
Status:         IMPLEMENTED — Self-Audit Passed; SYSTEM_EVOLUTION_LOG encoding follow-up remains open
Related Change: SCP-2026-07-17-102 (Core + Specialist structure retained; first .NET implementation corrected)
Research Basis: RESEARCH_TO_SYSTEM_CHANGE_REPORT_DOTNET_ENGINEERING_AGENT_2026-07-18.md
```

---

## Problem

The first `.NET` specialist created under SCP-102 has the correct high-level structure but not the required operating quality. It uses generic/marketing language, broad library lists, and insufficiently conditional rules instead of a concise, evidence-based implementation contract.

It therefore does not yet reliably prevent the real errors that motivated specialization: unsafe async usage, DI lifetime errors, EF Core misuse, unsafe migration/application behavior, auth/configuration changes, and insufficient verification evidence.

---

## Evidence

See:

```text
project-control/archive/RESEARCH_TO_SYSTEM_CHANGE_REPORT_DOTNET_ENGINEERING_AGENT_2026-07-18.md
```

Research was completed by `DomainResearchAgent` using Microsoft/OWASP/xUnit primary sources and analyzed by `DomainExpertAgent` into a lean operating model.

Key evidence:

1. .NET rules vary by application type (Web API, Razor/MVC, Blazor, EF Core, ADO.NET).
2. Dangerous changes need explicit `STOP/ASK` behavior.
3. Build success is insufficient evidence for security, DI, data, or contract behavior.
4. Generic architecture/pattern mandates create over-engineering and should be rejected.

---

## Affected Files

| File | Action | Purpose |
|---|---|---|
| `.opencode/agents/engineering-agent-dotnet.md` | Rewrite | Replace generic prose with the professional .NET decision contract |
| `tera-system/engineering-helpers/engineering-agent-core.md` | Rewrite narrowly | Retain only genuinely shared rules; remove .NET/architecture assumptions and duplicate operational text |
| `.opencode/agents/engineering-agent.md` | Normalize | Keep it as fallback, reference the core, remove duplicated core rules and unsupported expertise claims |
| `tera-system/TeraSubAgents.md` | Update narrowly | Correct any wording/line-size metadata affected by the rebuilt contract |
| `tera-system/AGENT_DEPENDENCY_MAP.md` | Update narrowly | Correct file sizes and retain dependency references |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | Update after execution | Record corrective implementation and validation |

No new agent, folder, profile family, permission, MCP, or architecture layer is proposed.

---

## Proposed Change

### 1. Rewrite `.opencode/agents/engineering-agent-dotnet.md`

Replace its current content with a concise 180–210 line operating contract containing:

1. **Identity and Mission** — .NET/C# implementation specialist, not architect/scope owner.
2. **Authority, Scope, and STOP/ASK** — explicit high-risk stop conditions.
3. **Inputs and Precedence** — task docs, shared core, active profile, then agent method.
4. **Task Classification** — Web API / MVC-Razor / Blazor / EF Core / ADO.NET / Worker-Library.
5. **C# and Async Safety** — async/await, cancellation, disposal boundaries.
6. **Dependency Injection and Resource Ownership** — lifetime and DI ownership rules.
7. **Security, Configuration, and Observability** — secrets, server-side enforcement, safe logging.
8. **Technology Profile Application** — only apply active, relevant profile rules.
9. **Data Access and Database Change Controls** — EF/ADO.NET; migration review and no unapproved apply.
10. **Implementation Workflow** — intake → classify → gates → smallest implementation → verification → handback.
11. **Verification and Evidence** — change-specific evidence requirements.
12. **Handback Format** — changed files, behavior, evidence, unverified items, risks, STOP/ASK decisions.
13. **Explicit Non-Rules** — forbid automatic Clean Architecture/CQRS/repository/interface/transaction/package choices.
14. **Authoritative References** — compact source list.

### 2. Narrow `engineering-agent-core.md`

Keep only rules genuinely shared by all engineering agents:

- conduct and authority references;
- task scope and allowed-write-target discipline;
- source precedence and smallest-sufficient-change rule;
- generic secret protection and no hidden scope expansion;
- generic build/test evidence and handback discipline;
- continuous-improvement route.

Remove implementation-language examples, prescribed folder layouts, generic tool lists, and detailed framework behavior. The core must not duplicate specialist rules.

### 3. Normalize fallback `engineering-agent.md`

Make the fallback agent a compact general implementation contract that:

- reads the shared core;
- states it is not a substitute for an available language specialist;
- does not claim “expert” depth across all languages;
- routes .NET tasks to `engineering-agent-dotnet.md`;
- keeps only fallback-specific activation/scope behavior.

### 4. Validation before declaring the .NET agent operational

Run a focused adversarial review against the rebuilt agent contract. It must correctly respond to at least these scenarios:

| Scenario | Expected behavior |
|---|---|
| `.Result` in ASP.NET async path | Identify/reject as unsafe |
| Singleton consuming scoped DbContext/service | Identify/reject lifetime violation |
| Razor POST without ModelState / state-changing GET | Require correction |
| Blazor client-only authorization | Require server-side enforcement |
| EF migration dropping/changing data | STOP/ASK; review only, no apply |
| `dotnet ef database update` against shared DB | STOP/ASK |
| Unbounded EF query / visible N+1 risk | Require risk review and appropriate correction |
| Tracked secret or sensitive structured log | Reject/redact |
| Major SDK/NuGet upgrade without approval | STOP/ASK |
| Request to add CQRS/repository by default | Reject as a non-rule unless task/baseline justifies it |

The final self-audit will state each scenario result, not just claim that the file is professional.

---

## Why This Is Necessary

1. Majed correctly identified that the current implementation is not precise or serious enough.
2. A specialist agent must reduce defects through explicit decisions and verification, not by declaring expertise.
3. The correction retains the useful Core + Specialist model while removing bloat and unsupported generalizations.
4. The model establishes a professional template for future language specialists without copying the .NET file blindly.

---

## Rejected Alternatives

| Alternative | Reason Rejected |
|---|---|
| Add more .NET facts to the current file | Makes the existing generic list longer without fixing its decision model |
| Keep generic expertise statements | They are not testable and do not prevent defects |
| Force Clean Architecture/CQRS/repository rules | Over-engineering; existing architecture and task context govern |
| Add a new .NET profile layer | Unneeded. Existing project stack profiles plus the specialist contract are sufficient |
| Create another agent to review .NET code | Premature bloat; Auditor/QA remain separate quality gates |

---

## Anti-Bloat Check

| Question | Answer |
|---|---|
| What problem does this solve? | The current .NET agent is verbose but operationally weak; errors are not prevented reliably. |
| Why change existing files rather than add files? | The correction strengthens the established Core + Specialist pattern without another layer. |
| Does it reduce complexity? | Yes. It replaces marketing, duplication, and generic lists with shorter conditional rules and gates. |
| Token impact? | Net reduction expected: .NET agent ~180–210 lines; core and fallback also reduced. |
| Smallest sufficient method? | Yes — rewrite the three affected contracts, retain mappings unless metadata changes. |

---

## Risk

| Risk | Mitigation |
|---|---|
| Too many stops slow normal .NET work | STOP/ASK limited to irreversible, security, contract, dependency, and environment changes |
| Agent under-applies a conditional rule | Mandatory task classification and active-profile selection before coding |
| Cross-file inconsistency | One ownership model; grep/dependency validation; adversarial scenario audit |
| Existing fallback loses useful behavior | Preserve only generic implementation behavior and route known specialist stacks correctly |

---

## Rollback Plan

1. Restore the three contract files to their SCP-102 versions using git.
2. Restore any metadata wording/size entries changed in `TeraSubAgents.md` and `AGENT_DEPENDENCY_MAP.md`.
3. Remove this corrective log entry if the rollback is approved.

---

## Post-Change Validation Gates

- [x] Conduct and authority rules remain intact.
- [x] Core / specialist / active profile / task-document ownership is non-duplicated and clear.
- [x] .NET agent is 209 lines; core is 127 and fallback is 56 — all below split threshold.
- [x] All ten adversarial scenarios have explicit expected behavior and pass a self-audit.
- [x] `TeraSubAgents.md`, `AGENT_DEPENDENCY_MAP.md`, and activation routing remain accurate.
- [x] No privilege expansion, client-app modification, MCP, or extra agent is introduced.
- [x] Scoped `git diff --check` passes for all files changed by SCP-103.
- [ ] `SYSTEM_EVOLUTION_LOG.md` records the corrective change and evidence — blocked: current reference/worktree representation is encoding-corrupted; unsafe to overwrite without a separate recovery decision.

## Self-Audit — 2026-07-18

| # | Adversarial scenario | Contract evidence | Result |
|---:|---|---|---|
| 1 | `.Result` / `.Wait()` / `Thread.Sleep()` in async path | §5 | PASS — explicitly prohibited |
| 2 | Singleton captures scoped dependency | §6 | PASS — explicitly prohibited |
| 3 | Razor mutation lacks ModelState / GET changes state | §8 | PASS — ModelState and no state-changing GET required conditionally |
| 4 | Blazor client-side authorization used as enforcement | §8 | PASS — server/API enforcement required |
| 5 | Destructive migration or shared DB apply | §3, §9 | PASS — STOP/ASK and review-only behavior |
| 6 | Unbounded query or visible N+1 risk | §9, §11 | PASS — query-risk review required |
| 7 | Tracked secret or sensitive logging | §3, §7 | PASS — prohibited and STOP/ASK protected |
| 8 | Major SDK/NuGet upgrade without approval | §3 | PASS — STOP/ASK required |
| 9 | CQRS/Repository/pattern imposed by default | §9, §13 | PASS — explicit non-rule |
| 10 | Unsupported claim of verification | §11, §12 and shared core §7 | PASS — evidence / unverified-risk reporting required |

### Independent Audit Attempt

An Auditor invocation was attempted with bounded system-file scope. Auditor correctly declined because its active contract authorizes only TeraAgent, or Monitor under the Majed-requested challenge flow. No authority was bypassed.

This reveals a separate governance inconsistency: Hares is permitted to request Auditor system-file audits, while Auditor does not list Hares as an authorized invoker. It does not affect the .NET agent contract but needs a separate approved resolution before an independent Auditor review can occur.

---

## Approval Required

Yes — Majed approval is required before rewriting active agent/system files.
