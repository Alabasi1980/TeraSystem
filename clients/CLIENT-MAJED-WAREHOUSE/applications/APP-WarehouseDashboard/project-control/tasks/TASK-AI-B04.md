# TASK-AI-B04 — Provider Abstraction: IAIProvider + OpenCodeGoAdapter

> **Status:** Draft → Approved  
> **Batch:** AI-B-2 (Backend Foundation — Sequential after B01, B02, B03)  
> **Depends On:** TASK-AI-B02 (AIAssistantOptions config model)  
> **Phase:** AI Assistant — Phase B (Backend Foundation)  
> **Plan Reference:** `project-preparation/AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` §7.4, §12  

---

## 1. Objective

Create an internal AI provider abstraction so the assistant can swap providers without changing business logic. The first provider adapter targets OpenCode Go / DeepSeek V4 Flash.

Two files:

1. **IAIProvider interface** — defines the contract for any AI provider
2. **OpenCodeGoAdapter** — first implementation, calls OpenCode Go API

---

## 2. Context

- **Technology Profile:** `dotnet-razorpages-adonet`
- **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`
- **Depends on:** `AIAssistantOptions` (created in B02), must inject via `IOptions<AIAssistantOptions>`

---

## 3. Allowed Sources

- `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\AIAssistantOptions.cs` — config model
- `ClientAppPath\src\WarehouseDashboard.Web\Program.cs` — registration pattern
- `ClientAppPath\project-preparation\AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` — §12 request package + §13 prompt

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\IAIProvider.cs` — **NEW**
2. `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\OpenCodeGoAdapter.cs` — **NEW**
3. `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\AIAssistantRequest.cs` — **NEW** (request/response DTOs)
4. `ClientAppPath\src\WarehouseDashboard.Web\Program.cs` — add DI registration (1-2 lines)

---

## 5. Forbidden

- Must NOT include real API key in source code — read from `AIAssistantOptions`.
- Must NOT log the API key.
- Must NOT create a service class — only interface + adapter + DTOs.
- Must NOT modify existing files except Program.cs (add registration).
- Must NOT hardcode model name or URL — read from config.

---

## 6. Technical Specification

### File 1: `Infrastructure/AIAssistantRequest.cs` (DTOs)

```csharp
namespace WarehouseDashboard.Web.Infrastructure;

public class AIAssistantRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public string? CardAssistantPrompt { get; set; }
    public int MaxOutputTokens { get; set; } = 300;
}

public class AIAssistantResponse
{
    public string Content { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TokensUsed { get; set; }
    public long ResponseTimeMs { get; set; }
}
```

### File 2: `Infrastructure/IAIProvider.cs`

```csharp
namespace WarehouseDashboard.Web.Infrastructure;

public interface IAIProvider
{
    Task<AIAssistantResponse> SendAsync(AIAssistantRequest request, CancellationToken ct = default);
}
```

### File 3: `Infrastructure/OpenCodeGoAdapter.cs`

Implementation requirements:
- Constructor injects `IOptions<AIAssistantOptions>` and `IHttpClientFactory`.
- Uses `HttpClient` from factory (not `new HttpClient()`).
- Sends POST to `{BaseUrl}` (from config) with OpenAI-compatible JSON body.
- Request body structure:
  ```json
  {
    "model": "{ModelId}",
    "messages": [
      {"role": "system", "content": "{SystemPrompt}"},
      {"role": "user", "content": "{UserMessage}"}
    ]
  }
  ```
- If `CardAssistantPrompt` is not null/empty, appends it to the system message: `"{SystemPrompt}\n\nتعليمات خاصة لهذه البطاقة:\n{CardAssistantPrompt}"`
- Adds HTTP header: `Authorization: Bearer {ApiKey}`
- Reads response: `choices[0].message.content`
- Timeout from config: `TimeoutSeconds`
- Error handling: catch HttpRequestException and TaskCanceledException, return `AIAssistantResponse` with `Success=false` and safe error message (no raw stack traces or API keys).
- NEVER log the API key anywhere — not in catch blocks, not in error messages.
- `ResponseTimeMs`: measures total round-trip time with Stopwatch.

### File 4: `Program.cs`

Add registration:
```csharp
builder.Services.AddHttpClient<IAIProvider, OpenCodeGoAdapter>();
```

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `AIAssistantRequest` and `AIAssistantResponse` DTOs exist |
| AC-2 | `IAIProvider` interface has single `SendAsync` method |
| AC-3 | `OpenCodeGoAdapter` implements `IAIProvider` |
| AC-4 | Adapter reads config from `AIAssistantOptions` — no hardcoded values |
| AC-5 | Adapter sends correct OpenAI-compatible JSON payload |
| AC-6 | Card-specific prompt is appended to system prompt when provided |
| AC-7 | API key NEVER appears in logs or error messages |
| AC-8 | Timeout and HttpClient from factory are respected |
| AC-9 | Error handling returns safe `AIAssistantResponse` (no raw exceptions) |
| AC-10 | `Program.cs` registers `IAIProvider` → `OpenCodeGoAdapter` |
| AC-11 | `dotnet build` passes with 0 errors and 0 warnings |

---

## 8. Pre-Execution Gate Result

```
Gate: PASS
Orchestration Level: Task Delegation Only
Model Tier: Light-Medium (interface + HttpClient adapter + DTOs, ~120 lines)
Security Sensitivity: Medium (handles API key, HTTP outbound) — must never log key
Design Governance: N/A (no UI)
Technology Profile: dotnet-razorpages-adonet ✅
```

---

## 9. Handback Requirements

1. List of files created/modified with line counts.
2. Full content of IAIProvider.cs.
3. Full content of OpenCodeGoAdapter.cs (verify NO key logged).
4. Full content of AIAssistantRequest.cs.
5. The added DI registration in Program.cs.
6. Build result (must be 0 errors, 0 warnings).
7. Confirmation: API key never appears in error messages or logs.
