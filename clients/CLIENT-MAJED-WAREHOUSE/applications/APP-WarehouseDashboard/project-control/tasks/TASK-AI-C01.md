# TASK-AI-C01 — Summary Builder Interface + CardSummary DTO

> **Status:** Draft → Approved  
> **Batch:** AI-C-1 (Data Summary — Foundation, then C02-C06 parallel)  
> **Depends On:** Phase B complete (B01-B05)  
> **Phase:** AI Assistant — Phase C (Data Summary Builder)  
> **Plan Reference:** `project-preparation/AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` §8, §9, §10  

---

## 1. Objective

Create the `ICardSummaryBuilder` interface and the `CardSummary` data transfer object. Every card type (KPI, Chart, Table, Generic) will have its own implementation of this interface.

---

## 2. Context

- **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Sources

- `ClientAppPath\src\WarehouseDashboard.Web\Models\DashboardCard.cs`
- `ClientAppPath\project-preparation\AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` — §8, §9, §10, §11

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\CardSummary.cs` — **NEW**
2. `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\ICardSummaryBuilder.cs` — **NEW**

---

## 5. Forbidden

- No SQL execution — only DTO + interface.
- No modification of any existing file.
- No Program.cs changes (registration happens when implementations are created).

---

## 6. Technical Specification

### File 1: `Infrastructure/CardSummary.cs`

```csharp
namespace WarehouseDashboard.Web.Infrastructure;

public class CardSummary
{
    // Card metadata
    public int CardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ChartType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AssistantPrompt { get; set; }

    // Scope
    public int DepthLevel { get; set; }
    public string DepthLabel { get; set; } = string.Empty;
    public bool HasDateColumn { get; set; }
    public string? DateColumn { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public bool IsFullDataReached { get; set; }
    public bool HasDeeperData { get; set; }

    // Value & Trend
    public double? CurrentValue { get; set; }
    public double? PreviousValue { get; set; }
    public double? ChangePercent { get; set; }
    public string? TrendDirection { get; set; } // "up", "down", "stable", null

    // Series (time-based)
    public List<SeriesPoint> SeriesData { get; set; } = new();

    // Aggregates
    public int? TotalRowCount { get; set; }
    public Dictionary<string, NumericColumnSummary> ColumnSummaries { get; set; } = new();

    // Top / Bottom
    public List<CategoryItem> TopItems { get; set; } = new();
    public List<CategoryItem> BottomItems { get; set; } = new();

    // Samples
    public List<Dictionary<string, object?>> SampleRows { get; set; } = new();

    // Quality notes
    public List<string> DataQualityNotes { get; set; } = new();
}

public class SeriesPoint
{
    public string Period { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class CategoryItem
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public double? Percent { get; set; }
}

public class NumericColumnSummary
{
    public double Sum { get; set; }
    public double Average { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
}
```

### File 2: `Infrastructure/ICardSummaryBuilder.cs`

```csharp
namespace WarehouseDashboard.Web.Infrastructure;

public interface ICardSummaryBuilder
{
    /// <summary>
    /// Builds a data summary for a dashboard card at a given depth.
    /// </summary>
    /// <param name="card">Card configuration from the database.</param>
    /// <param name="depthLevel">1-6 per the depth map.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Structured summary ready to send to the AI model.</returns>
    Task<CardSummary> BuildSummaryAsync(Models.DashboardCard card, int depthLevel, CancellationToken ct = default);
}
```

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `CardSummary` class exists with all required fields |
| AC-2 | `SeriesPoint`, `CategoryItem`, `NumericColumnSummary` inner types exist |
| AC-3 | `ICardSummaryBuilder` interface has `BuildSummaryAsync` method |
| AC-4 | Interface accepts `DashboardCard` model and returns `CardSummary` |
| AC-5 | `dotnet build` passes with 0 errors and 0 warnings |
| AC-6 | No existing files modified |

---

## 8. Pre-Execution Gate Result

```
Gate: PASS
Orchestration Level: Task Delegation Only
Model Tier: Light (DTO + interface, no logic)
Security Sensitivity: Low
Technology Profile: dotnet-razorpages-adonet ✅
```

---

## 9. Handback Requirements

1. Files created with line counts.
2. Full content of CardSummary.cs.
3. Full content of ICardSummaryBuilder.cs.
4. Build result.
