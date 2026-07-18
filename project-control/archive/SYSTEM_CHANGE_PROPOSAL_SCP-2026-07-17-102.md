# SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-17-102

```text
Title:          Engineering Agent Split — Shared Core + First Specialized .NET Agent
Request Type:   Owner Improvement Request / Architecture Restructuring
Date:           2026-07-17
Author:         TeraSystemEvolutionAgent (حارس)
Status:         PENDING — Awaits Majed Approval
```

---

## Problem

EngineeringAgent currently handles ALL programming languages in one file. It claims expertise in:

- .NET (C#, ASP.NET Core, EF Core, Blazor)
- JavaScript/TypeScript (React, Next.js, Node.js, Prisma)
- Python (Django, FastAPI)
- All databases (PostgreSQL, SQL Server, MySQL, MongoDB)

This breadth prevents deep expertise in any single language. The agent has too much surface area to maintain correctly, leading to:

1. **Code quality gaps** — language-specific best practices, patterns, and pitfalls are not consistently applied.
2. **Bug discovery late** — issues are only found when Majed tests the code manually.
3. **Long fix cycles** — fixing code that should have been correct from the start.
4. **No natural path for specialization** — adding more languages to the same file would make it worse.

---

## Solution — Hybrid Specialization

Split into three layers:

```text
engineering-agent-core.md                    ← Shared rules (ALL engineers read this)
engineering-agent.md (reduced)                ← Fallback agent for languages without a specialist
engineering-agent-dotnet.md  (NEW)           ← .NET/C# specialist (FIRST specialist)
```

### Why Hybrid, Not Fully Independent

Every engineering agent shares ~70% common content:
- SOLID principles, Clean Code, 12-Factor App
- Security basics (no secrets in code, SQL injection prevention)
- API integration patterns, error/loading/empty state handling
- Activation flow, task scope discipline, handback protocol
- Path Validation Gate, self-review checklist
- AIS protocol

Putting this in `engineering-agent-core.md` eliminates duplication when we create more specialists later.

---

## Affected Files

| File / Folder | Action | Purpose |
|---|---|---|
| `tera-system/engineering-helpers/` | **Create folder** | New folder for shared engineering references |
| `tera-system/engineering-helpers/engineering-agent-core.md` | **Create** | Shared rules for ALL engineering agents — no language-specific content |
| `.opencode/agents/engineering-agent-dotnet.md` | **Create** | .NET/C# specialized engineering agent |
| `.opencode/agents/engineering-agent.md` | **Reduce** | Remove .NET-specific content; keep as fallback generalist + reference core |
| `tera-system/AGENT_DEPENDENCY_MAP.md` | **Update** | Add engineering-agent-dotnet row; update engineering-agent references |
| `tera-system/TeraSubAgents.md` | **Update** | Add engineering-agent-dotnet entry; update engineering-agent entry |
| `tera-system/TeraPolicyMap.md` | **Update** | Add engineering-agent-core as source of truth entry |
| `tera-system/TeraArchitectureMap.md` | **Update** | Add engineering-helpers folder role |
| `.opencode/agents/tera.md` | **Update** | Add engineering-agent-dotnet to sub-agent list; update references |
| `tera-system/AGENT_ACTIVATION_MATRIX.md` | **Update** | Add dotnet agent activation trigger |
| `project-control/SYSTEM_EVOLUTION_LOG.md` | **Update after execution** | Record change |

---

## Content Plan — What Goes Where

### 1. `tera-system/engineering-helpers/engineering-agent-core.md` (NEW)

Shared reference — NO language-specific content.

| Section | Content | Source (from current engineering-agent.md) |
|---|---|---|
| §1 | Purpose (this is the shared core) | New |
| §2 | Core Engineering Identity | §1 (من أنا — adapted to "I am an implementer engineer") |
| §3 | Universal Engineering Principles | §2 (12 Factor App), §3 (Clean Code) |
| §4 | Universal Technical Responsibilities (conceptual) | §4 (abstracted — API, State, Logic, DB, Security, States) |
| §5 | Workflow (ربط التصميم بالكود الخلفي) | §5 |
| §6 | Activation Flow | §6 |
| §7 | Self-Review Checklist | §9 |
| §8 | Path Validation Gate | §10.1 (preserved as-is) |
| §9 | What I Don't Do | §11 |
| §10 | Difference Between Agents | §12 |
| §11 | AIS Protocol | §13 |

**Language removed from core:**
- §9 from original "How I handle different technologies" → moves to specialized agents
- Language-specific libraries, tools, patterns → moves to specialized agents
- Language-specific security patterns → moves to specialized agents

### 2. `.opencode/agents/engineering-agent-dotnet.md` (NEW)

.NET/C# SPECIALIST — compact, focused.

| Section | Content |
|---|---|
| §1 | Identity: "I am a .NET specialist with deep C# expertise" |
| §2 | What I Read: `engineering-agent-core.md` + `tera-system/profiles/[ACTIVE_PROFILE].md` + task files |
| §3 | .NET Ecosystem Expertise (C# 12, ASP.NET Core 8, EF Core 8, Blazor, Minimal APIs, MAUI) |
| §4 | .NET-Specific Engineering Practices (idiomatic C#, async/await patterns, dependency injection, middleware pipeline, configuration system, logging) |
| §5 | .NET Security (JWT configuration, Data Protection API, anti-forgery tokens, SQL injection in ADO.NET vs EF Core, secret management) |
| §6 | .NET Common Mistakes that this agent MUST NOT make (sync-over-async, missing CancellationToken, over-catching exceptions, massive constructors, primitive obsession, no async in controllers, missing Using statements) |
| §7 | .NET Tools & Libraries (NuGet ecosystem, Serilog/NLog, FluentValidation, AutoMapper trade-offs, BenchmarkDotNet, xUnit/NUnit) |
| §8 | .NET Code Review Checklist (specific to C# — disposed objects, async void avoidance, struct vs class awareness, LINQ performance awareness) |
| §9 | What I Produce (.NET-specific — Controllers, Services, Background Services, Middleware, Migrations, etc.) |
| §10 | Exclusions (I don't touch Node.js, Python, etc. — defer to fallback engineering-agent) |

**Target size:** ~300 lines (compact, no bloat)

### 3. `.opencode/agents/engineering-agent.md` (REDUCED)

Becomes a general/fallback engineering agent — used for any language that does NOT yet have a specialized agent.

**Changes:**
- Remove §9 (How I handle different technologies — the .NET/JS/Python/DB sections)
- Remove language-specific references throughout
- Add reference to `engineering-agent-core.md` at start
- Keep general multi-language awareness (can still work with any language, just with less depth)

**Target size:** reduces from 373 lines → ~250 lines

---

## Why This Is Necessary

1. **Deep expertise requires specialization** — No single agent can be expert in .NET, Node.js, Python, SQL Server, AND PostgreSQL without surface-level knowledge in some.

2. **Bug prevention** — The .NET specialist will know patterns like sync-over-async, missing CancellationToken, and LINQ N+1 that a generalist may overlook.

3. **Scalable pattern** — Once the core + .NET template is established, adding more specialists (Node.js, Python, etc.) takes minimal effort.

4. **No duplication** — 70% shared content lives once in `engineering-agent-core.md`

5. **Clear boundaries** — TeraAgent selects the right engineer per task: `.NET task → engineering-agent-dotnet`

---

## Rejected Alternatives

| Alternative | Reason Rejected |
|---|---|
| Keep one general agent + improve it | Already tried — the breadth prevents depth |
| Create separate agents WITHOUT a core | Would duplicate 70% of content across N agents |
| Create profiles only, keep one agent | Profiles don't change agent behavior, just settings |
| Create ALL specialists at once | Too big. Start with .NET (most used) and iterate |
| Create new folder `helpers/` not `engineering-helpers/` | Better to be explicit about purpose |

---

## Anti-Bloat Check

| Question | Answer |
|---|---|
| What problem does this solve? | Generalist agent produces buggy code across multiple languages |
| Why not enough to edit existing file? | The file must be split — one language per specialized agent |
| Is there a smaller way? | No — the split is the point |
| Does this reduce or increase complexity? | Initially increases files, but ELIMINATES confusion and bug-fix cycles |
| Token impact? | engineering-agent-dotnet: ~300 lines. Core: ~250 lines. Total: ~550 new lines across 2 files. Both well below split threshold. |
| Future-proof? | Yes — pattern for adding Node, Python, etc. later |

---

## Risk

| Risk | Mitigation |
|---|---|
| TeraAgent may still use the wrong agent for a task | Clear naming & TeraSubAgents & AGENT_ACTIVATION_MATRIX will guide selection |
| Core file needs updating when patterns change | Single source of truth — easier than updating N files |
| Fallback agent may still produce shallow code | Acknowledge — that's acceptable for rare/unusual languages |
| Breaking existing references to engineering-agent.md | Update ALL references in system files in the same pass |

---

## Rollback Plan

1. Delete `engineering-agent-core.md` and `engineering-agent-dotnet.md`
2. Restore `engineering-agent.md` from HEAD (git checkout)
3. Revert system file updates (TeraSubAgents, Dependency Map, Policy Map, etc.)
4. Revert `TeraArchitectureMap.md` — remove engineering-helpers folder role

---

## Validation Plan

- [ ] Anti-Bloat Gate — PASS
- [ ] engineering-agent-dotnet.md reads core + profile before task — reference verified
- [ ] No broken references in any agent file — grep check PASS
- [ ] tera.md correctly lists engineering-agent-dotnet as sub-agent
- [ ] TeraSubAgents.md updated with both agents
- [ ] AGENT_DEPENDENCY_MAP.md updated
- [ ] File sizes: core < 300 lines, dotnet < 350 lines
- [ ] `git diff --check` — PASS
- [ ] SYSTEM_EVOLUTION_LOG.md — Updated

---

## Approval Required

Yes — Majed approval required before any edits.
