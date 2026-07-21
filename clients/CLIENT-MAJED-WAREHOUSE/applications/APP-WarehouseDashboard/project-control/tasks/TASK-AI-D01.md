# TASK-AI-D01 — Assistant API Endpoint

> **Status:** Draft → Approved  
> **Batch:** D-1 (Parallel with D02)  
> **Depends On:** Phase B + C complete  
> **Phase:** AI Assistant — Phase D (Frontend + API)  

---

## 1. Objective

Create a Razor Pages API endpoint that the Side Panel calls via AJAX: `POST /api/card-insights/analyze`

---

## 2. ClientAppPath

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Sources

- `src\WarehouseDashboard.Web\Services\CardInsightService.cs`
- `src\WarehouseDashboard.Web\Services\CardSummaryBuilderFactory.cs`
- `src\WarehouseDashboard.Web\Infrastructure\AIAssistantRequest.cs`
- Existing API page patterns in `src\WarehouseDashboard.Web\Pages\Api\`

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Pages\Api\CardInsightAnalyze.cshtml` — **NEW**
2. `ClientAppPath\src\WarehouseDashboard.Web\Pages\Api\CardInsightAnalyze.cshtml.cs` — **NEW**

---

## 5. Technical Spec

**PageModel** (`CardInsightAnalyze.cshtml.cs`):
- Route: `POST /api/card-insights/analyze`
- AntiForgeryToken: NOT required (AJAX internal call)
- Accepts JSON body: `{ "cardId": 5, "mode": "explain", "depthLevel": 1 }`
- Flow:
  1. Read JSON body
  2. Load DashboardCard via EF Core to build summary (can share DbContext)
  3. Call `CardSummaryBuilderFactory.GetBuilder(card.ChartType).BuildSummaryAsync(card, depthLevel)`
  4. Build a user message string from the card summary (simple: include key fields)
  5. Call `CardInsightService.AnalyzeCardAsync(cardId, mode, depthLevel)`
     - Actually, modify CardInsightService to accept a pre-built CardSummary instead of querying internally
     - Better: Create an overload `AnalyzeCardWithSummaryAsync(cardId, mode, depthLevel, CardSummary summary)`
  6. Return JSON: `{ "content": "...", "success": true, "isFullDataReached": true, "hasDeeperData": false, "depthLevel": 1, "depthLabel": "آخر 3 أشهر" }`

**Response DTO** (add to AIAssistantRequest.cs or create separate):
```csharp
public class CardInsightResponse
{
    public string Content { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsFullDataReached { get; set; }
    public bool HasDeeperData { get; set; }
    public int DepthLevel { get; set; }
    public string DepthLabel { get; set; } = string.Empty;
}
```

**CardInsightService modification:**
Add overload:
```csharp
public async Task<CardInsightResponse> AnalyzeCardWithSummaryAsync(
    int cardId, string mode, int depthLevel, CardSummary summary, CancellationToken ct = default)
```
This method uses the pre-built summary to construct the user message, then calls IAIProvider.

---

## 6. Forbidden

- No UI (just JSON endpoint)
- No authentication changes
- No modification to CardSummary or builders
- Must check AssistantEnabled before calling AI

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | POST endpoint accepts JSON and returns JSON |
| AC-2 | Calls CardSummaryBuilderFactory + CardInsightService |
| AC-3 | Returns IsFullDataReached + HasDeeperData |
| AC-4 | Checks AssistantEnabled — returns error if disabled |
| AC-5 | `dotnet build` passes with 0 errors and 0 warnings |

---

## 8. Pre-Execution Gate

```
Gate: PASS | Security: Medium (read-only, internal endpoint) | Model: Medium
```

## 9. Handback

1. Files created/modified
2. Endpoint URL and sample request/response
3. Build result
