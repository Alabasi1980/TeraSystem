# RESEARCH_TO_SYSTEM_CHANGE_REPORT — .NET Engineering Agent Rebuild

```text
Research Topic:      Professional operating contract for a specialized .NET/C# implementation agent
Date:                2026-07-18
Requested By:        Majed
Prepared By:         TeraSystemEvolutionAgent (حارس)
Research Agents:     DomainResearchAgent + DomainExpertAgent
Scope:               .NET/C#, ASP.NET Core, EF Core, Razor/MVC, Blazor, ADO.NET, security, testing, and operational verification
Status:              Research complete — no corrective implementation approved yet
```

---

## Sources Reviewed

Primary sources reviewed by DomainResearchAgent:

1. .NET Support Policy — https://dotnet.microsoft.com/platform/support/policy/dotnet-core
2. Microsoft Dependency Injection Guidelines — https://learn.microsoft.com/dotnet/core/extensions/dependency-injection/guidelines
3. C# Async Scenarios — https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/async-scenarios
4. ASP.NET Core Options — https://learn.microsoft.com/aspnet/core/fundamentals/configuration/options
5. ASP.NET Core Logging — https://learn.microsoft.com/aspnet/core/fundamentals/logging
6. ASP.NET Core API Error Handling / ProblemDetails — https://learn.microsoft.com/aspnet/core/fundamentals/error-handling-api
7. ASP.NET Core Secret Storage — https://learn.microsoft.com/aspnet/core/security/app-secrets
8. EF Core DbContext Configuration — https://learn.microsoft.com/ef/core/dbcontext-configuration
9. EF Core Tracking — https://learn.microsoft.com/ef/core/querying/tracking
10. EF Core Efficient Querying — https://learn.microsoft.com/ef/core/performance/efficient-querying
11. EF Core Transactions — https://learn.microsoft.com/ef/core/saving/transactions
12. Applying EF Core Migrations — https://learn.microsoft.com/ef/core/managing-schemas/migrations/applying
13. ASP.NET Core Model Validation — https://learn.microsoft.com/aspnet/core/mvc/models/validation
14. ASP.NET Core Antiforgery — https://learn.microsoft.com/aspnet/core/security/anti-request-forgery
15. JWT Bearer Authentication — https://learn.microsoft.com/aspnet/core/security/authentication/configure-jwt-bearer-authentication
16. Blazor Security — https://learn.microsoft.com/aspnet/core/blazor/security
17. ASP.NET Core Integration Tests — https://learn.microsoft.com/aspnet/core/test/integration-tests
18. dotnet build / dotnet test — https://learn.microsoft.com/dotnet/core/tools/
19. xUnit documentation — https://xunit.net/docs/getting-started/v3/getting-started
20. OWASP Top 10 2025 — https://owasp.org/Top10/2025/

---

## Relevant Findings

### 1. A specialist operating file must drive decisions, not claim expertise

Statements such as “15 years of experience,” “expert in the ecosystem,” motivational quotations, and long tool lists do not create verifiable agent behavior.

The operating file must instead define:

- what the agent reads and what has precedence;
- how it classifies the active .NET application type;
- which rules are unconditional versus conditional;
- when it must stop and ask;
- what verification evidence it must return.

### 2. .NET has real context-dependent rules

Rules vary by active stack:

| Context | Required behavior |
|---|---|
| Web API | Preserve contract; validate input; enforce authorization server-side; return safe consistent errors |
| MVC / Razor Pages | Validate ModelState before mutation; never mutate through GET; apply antiforgery in cookie/form contexts |
| Blazor | Client-side authorization is not security enforcement; hosting/circuit lifetime affects state and DbContext patterns |
| EF Core | DbContext is short-lived and non-thread-safe; queries need bounded/projection/tracking decisions; migrations need review |
| ADO.NET | Parameterized SQL and resource disposal are mandatory |

Therefore no generic “always use X” rule is valid for all .NET applications.

### 3. High-risk changes require STOP/ASK

The agent needs explicit stop conditions for:

- applying migrations or SQL to shared, staging, or production databases;
- destructive or ambiguous migrations;
- auth/JWT/OIDC/cookie/CORS/CSRF changes without a stated contract;
- tracked secrets or secret rotation;
- Target Framework, SDK, or major NuGet upgrades;
- public API/schema breaking changes;
- disabling security middleware or running irreversible environmental commands.

### 4. Verification must be profile-aware

`dotnet build` success alone cannot prove correct DI lifetimes, EF query shape, antiforgery, authorization, or API compatibility.

The agent must provide evidence matched to the change:

- build/test result or explicit reason it could not run;
- API validation/auth behavior when endpoints change;
- migration/SQL review without applying it;
- DI lifetime review for DI changes;
- query-risk review for EF/ADO.NET changes.

### 5. The core/specialist/profile split is still correct

| Location | Correct ownership |
|---|---|
| Shared engineering core | Authority, path/scope safety, generic delivery discipline, generic handback, common verification discipline |
| .NET specialist agent | C# async, DI/resource ownership, .NET security/configuration/observability, task classification, STOP/ASK, .NET verification matrix |
| Active .NET technology profile | Rules tied to Web API, Razor/MVC, Blazor, EF Core, ADO.NET, WPF/MAUI, and approved CLI behavior |
| Task documents | Actual goal, allowed paths, approved architecture, contracts, acceptance criteria, environment constraints |

---

## What Must NOT Be Adopted

The following are not universal .NET rules and must not be mandated by the agent:

- Clean Architecture, CQRS, Repository, or Unit of Work for every project;
- interfaces for every class;
- `AsNoTracking()` for every query;
- a manual transaction for every write;
- `ConfigureAwait(false)` everywhere;
- “latest” packages/framework versions by default;
- any prescribed library (Serilog, FluentValidation, AutoMapper, Polly, etc.) without task/project justification.

---

## Assessment of SCP-102 Current Implementation

The **Core + Specialist** architecture is sound, but the first implementation is not yet sufficiently professional:

| Issue | Why it is inadequate |
|---|---|
| Marketing/experience claims | Not operational and cannot be validated |
| Generic ecosystem/tool lists | Increase tokens without guiding a real task decision |
| Unqualified “always” rules | Can be wrong across Web API, Razor, Blazor, EF, and ADO.NET contexts |
| Missing structured STOP/ASK | Leaves dangerous changes open to agent judgment |
| Missing task classification | Cannot reliably select the relevant stack rules |
| Missing evidence-quality handback | Does not prove what was or was not verified |
| Shared core overlaps fallback agent | Creates duplication and rule-drift risk |

---

## Recommended System Change

Rebuild the .NET specialist as a lean decision contract (~180–210 lines) and normalize the shared core/fallback boundaries.

The rebuilt agent should contain only:

1. identity and mission;
2. authority/scope and STOP/ASK;
3. input precedence;
4. task and app-type classification;
5. C# async/cancellation safety;
6. DI/resource ownership;
7. security/configuration/observability;
8. conditional profile application;
9. EF Core/ADO.NET and database-change controls;
10. execution workflow;
11. verification/evidence;
12. handback format;
13. explicit non-rules;
14. authoritative references.

---

## Risk of Adoption

| Risk | Mitigation |
|---|---|
| Excessive STOP/ASK could slow harmless tasks | Stop conditions remain limited to security, contracts, irreversible effects, and environment risk |
| A generic agent file becomes duplicated with core | Normalize fallback agent to reference the core instead of reproducing its rules |
| Profiles conflict with the specialist | Define precedence: task docs → core safety/authority → specialist implementation method → active profile details |
| Agent is accepted based on prose alone | Validate with adversarial scenarios before declaring it active |

---

## Anti-Bloat Check

- No new agent or permanent layer is necessary.
- The existing Core + Specialist structure is retained.
- The replacement reduces generic claims and duplicate content.
- The .NET operating file targets less than 210 lines.
- Existing project technology profiles remain the only stack-specific project references.

---

## Approval Required

Yes. A corrective SCP is required before rewriting active agent/system files.
