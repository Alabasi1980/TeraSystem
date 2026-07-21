# TASK-AI-B02 — AI Provider Configuration Model + appsettings Section

> **Status:** Draft → Approved  
> **Batch:** AI-B-1 (Backend Foundation — Parallel with B01, B03)  
> **Phase:** AI Assistant — Phase B (Backend Foundation)  
> **Plan Reference:** `project-preparation/AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` §7.4  

---

## 1. Objective

Create an AI provider configuration model class and add a corresponding configuration section to `appsettings.json`. This allows swapping the AI provider per client without changing code.

Required configuration keys:

| Key | Type | Example Value |
|---|---|---|
| `ProviderName` | `string` | `"OpenCodeGo"` |
| `BaseUrl` | `string` | `"https://opencode.ai/zen/go/v1/chat/completions"` |
| `ModelId` | `string` | `"deepseek-v4-flash"` |
| `ApiKey` | `string` | `""` (empty placeholder) |
| `TimeoutSeconds` | `int` | `30` |
| `MaxOutputTokens` | `int` | `300` |
| `PromptVersion` | `string` | `"1.0"` |

---

## 2. Context

- **Technology Profile:** `dotnet-razorpages-adonet`
- **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Sources

- `ClientAppPath\src\WarehouseDashboard.Web\appsettings.json` — existing config
- `ClientAppPath\src\WarehouseDashboard.Web\Program.cs` — service registration pattern
- `ClientAppPath\project-preparation\AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` — §7.4

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\AIAssistantOptions.cs` — **NEW** config model class
2. `ClientAppPath\src\WarehouseDashboard.Web\appsettings.json` — add `"AIAssistant"` section
3. `ClientAppPath\src\WarehouseDashboard.Web\Program.cs` — register `IOptions<AIAssistantOptions>` (1-3 lines addition)

---

## 5. Forbidden

- Must NOT include a real API key in appsettings.json — use empty string placeholder.
- Must NOT modify existing config sections (ConnectionStrings, SyncApi, etc.).
- Must NOT delete or change any other file.
- Must NOT add business logic or service classes.
- Must NOT hardcode any values in Program.cs — only config binding.

---

## 6. Technical Notes

**File 1: `Infrastructure/AIAssistantOptions.cs`**

```csharp
namespace WarehouseDashboard.Web.Infrastructure;

public class AIAssistantOptions
{
    public const string SectionName = "AIAssistant";
    
    public string ProviderName { get; set; } = "OpenCodeGo";
    public string BaseUrl { get; set; } = "https://opencode.ai/zen/go/v1/chat/completions";
    public string ModelId { get; set; } = "deepseek-v4-flash";
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxOutputTokens { get; set; } = 300;
    public string PromptVersion { get; set; } = "1.0";
}
```

**File 2: `appsettings.json`** — add this section before the closing `}`:

```json
  "AIAssistant": {
    "ProviderName": "OpenCodeGo",
    "BaseUrl": "https://opencode.ai/zen/go/v1/chat/completions",
    "ModelId": "deepseek-v4-flash",
    "ApiKey": "",
    "TimeoutSeconds": 30,
    "MaxOutputTokens": 300,
    "PromptVersion": "1.0"
  }
```

**File 3: `Program.cs`** — add ONE LINE after existing builder.Services lines:

```csharp
builder.Services.Configure<AIAssistantOptions>(builder.Configuration.GetSection(AIAssistantOptions.SectionName));
```

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `AIAssistantOptions.cs` exists in `Infrastructure/` with all 7 properties and correct defaults |
| AC-2 | `appsettings.json` has `AIAssistant` section with all 7 keys |
| AC-3 | `Program.cs` registers `IOptions<AIAssistantOptions>` via `Configure<T>()` |
| AC-4 | `dotnet build` passes with 0 errors and 0 warnings |
| AC-5 | No real API key in any file |
| AC-6 | Existing config sections unchanged |

---

## 8. Pre-Execution Gate Result

```
Gate: PASS
Orchestration Level: Task Delegation Only
Model Tier: Light (config class + JSON section, no logic)
Security Sensitivity: Low (config model only, no real secrets)
Design Governance: N/A (no UI)
Technology Profile: dotnet-razorpages-adonet ✅
```

---

## 9. Handback Requirements

1. List of all files created or modified with line counts.
2. Contents of `AIAssistantOptions.cs`.
3. The added `appsettings.json` section.
4. The added line(s) in `Program.cs`.
5. Build result (must be 0 errors, 0 warnings).
