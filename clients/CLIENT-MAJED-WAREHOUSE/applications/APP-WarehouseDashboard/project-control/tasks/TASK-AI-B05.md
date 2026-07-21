# TASK-AI-B05 — CardInsightService Foundation

> **Status:** Draft  
> **Batch:** AI-B-3 (Sequential after B04)  
> **Depends On:** TASK-AI-B01 (model fields), B02 (config), B03 (read-only DB), B04 (provider)  
> **Phase:** AI Assistant — Phase B (Backend Foundation)  
> **Plan Reference:** `project-preparation/AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` §8, §12  

---

## 1. Objective

Create the `CardInsightService` class — the main orchestrator for the AI assistant. It reads card configuration, builds the AI request payload, calls the AI provider, and returns the response.

**This task creates ONLY the service class and its DI registration. No AI calls yet — the service is wired but not connected to any UI endpoint.**

---

## 2. Context

- **Technology Profile:** `dotnet-razorpages-adonet`
- **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`
- **Depends on:** All previous AI-B tasks

---

## 3. Allowed Sources

- `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\AIAssistantOptions.cs`
- `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\AIAssistantRequest.cs`
- `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\IAIProvider.cs`
- `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\ReadOnlyQueryHelper.cs`
- `ClientAppPath\src\WarehouseDashboard.Web\Models\DashboardCard.cs`
- `ClientAppPath\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs`
- `ClientAppPath\project-preparation\AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` — §8, §12, §13.1 prompt

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Services\CardInsightService.cs` — **NEW**
2. `ClientAppPath\src\WarehouseDashboard.Web\Program.cs` — add DI registration (1-2 lines)

---

## 5. Forbidden

- Must NOT create an API endpoint or controller — service only.
- Must NOT create UI files.
- Must NOT modify DashboardCard.cs, DbContext, or appsettings.
- Must NOT make real AI API calls during build.
- Must NOT hardcode the system prompt — store it as a `private const string` in the service.

---

## 6. Technical Specification

### CardInsightService

```csharp
namespace WarehouseDashboard.Web.Services;

public class CardInsightService
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ReadOnlyQueryHelper _readOnly;
    private readonly IAIProvider _aiProvider;
    private readonly ILogger<CardInsightService> _logger;

    // Constructor: inject all 4 dependencies
    public CardInsightService(
        WarehouseDashboardDbContext db,
        ReadOnlyQueryHelper readOnly,
        IAIProvider aiProvider,
        ILogger<CardInsightService> logger) { ... }

    // Main method — returns the AI response
    public async Task<AIAssistantResponse> AnalyzeCardAsync(
        int cardId, string mode, int depthLevel, CancellationToken ct = default)
    {
        // 1. Load card from EF Core
        // 2. Build system prompt: GeneralPrompt (const) + optional card.AssistantPrompt
        // 3. Prepare data summary via ReadOnlyQueryHelper
        //    For THIS task: just build the request with card metadata.
        //    The actual data querying goes in Phase C tasks.
        // 4. Build AIAssistantRequest DTO
        // 5. Call IAIProvider.SendAsync()
        // 6. Return response
    }
}
```

### General System Prompt (private const)

Store the prompt from §13.1 of the plan as a `private const string GeneralPrompt`. The full Arabic prompt approved in the plan.

### DI Registration in Program.cs

```csharp
builder.Services.AddScoped<CardInsightService>();
```

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `CardInsightService.cs` exists in `Services/` |
| AC-2 | Service injects 4 dependencies (DbContext, ReadOnlyQueryHelper, IAIProvider, ILogger) |
| AC-3 | `AnalyzeCardAsync` method signature exists with cardId, mode, depthLevel |
| AC-4 | General system prompt stored as `private const string` in the service |
| AC-5 | Card-specific `AssistantPrompt` is appended to system prompt when not null/empty |
| AC-6 | `Program.cs` registers `CardInsightService` as Scoped |
| AC-7 | `dotnet build` passes with 0 errors and 0 warnings |
| AC-8 | No API endpoint, controller, or UI code created |

---

## 8. Pre-Execution Gate Result

```
Gate: PASS
Orchestration Level: Task Delegation Only
Model Tier: Light (service class wiring, ~80 lines)
Security Sensitivity: Low (wiring only, no secrets, no outbound calls yet)
Design Governance: N/A (no UI)
Technology Profile: dotnet-razorpages-adonet ✅
```

---

## 9. Handback Requirements

1. Files created/modified.
2. Full content of CardInsightService.cs.
3. The added DI registration in Program.cs.
4. Build result (must be 0 errors, 0 warnings).
