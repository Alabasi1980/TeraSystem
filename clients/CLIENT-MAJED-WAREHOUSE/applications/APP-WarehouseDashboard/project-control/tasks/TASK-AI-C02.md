# TASK-AI-C02 — KPI Card Summary Builder

> **Status:** Draft → Approved  
> **Batch:** AI-C-2 (Parallel with C03, C04, C05)  
> **Depends On:** TASK-AI-C01 (ICardSummaryBuilder + CardSummary)  
> **Phase:** AI Assistant — Phase C (Data Summary Builder)  

---

## 1. Objective

Create `KpiSummaryBuilder : ICardSummaryBuilder` — builds structured data summaries for KPI-type dashboard cards.

---

## 2. ClientAppPath

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Sources (Read Only)

- `src\WarehouseDashboard.Web\Infrastructure\ICardSummaryBuilder.cs`
- `src\WarehouseDashboard.Web\Infrastructure\CardSummary.cs`
- `src\WarehouseDashboard.Web\Infrastructure\ReadOnlyQueryHelper.cs`
- `src\WarehouseDashboard.Web\Models\DashboardCard.cs`

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Services\KpiSummaryBuilder.cs` — **NEW**
2. `ClientAppPath\src\WarehouseDashboard.Web\Program.cs` — add DI registration (1 line)

---

## 5. Technical Spec

Class implements `ICardSummaryBuilder`. Constructor injects `ReadOnlyQueryHelper`.

**BuildSummaryAsync logic:**

1. Populate card metadata (Title, ChartType, Description, AssistantPrompt, CardId).
2. Determine depth scope:
   - If card has `DateColumn` (not empty): calculate `DateFrom` based on depthLevel.
     - depth 1: -3 months, 2: -6 months, 3: -12 months, 4: -3 years, 5: -5 years, 6: -10 years
   - If no DateColumn: set `HasDateColumn=false`, skip time-based queries, add quality note.
3. Query current KPI value using ReadOnlyQueryHelper:
   - Build SQL: `SELECT {AggregationType}({ValueColumn}) AS CurrentValue FROM ({card.SqlQuery}) sub WHERE {DateColumn} >= @from AND {DateColumn} <= @to`
   - If no DateColumn: no WHERE clause.
   - Use parameterized queries.
   - Default AggregationType="Sum". If "None": just take the first row's ValueColumn.
4. Query previous period value (same aggregation, previous equal period).
5. Calculate ChangePercent if both values exist.
6. If HasDateColumn: query series data (monthly aggregation) using ReadOnlyQueryHelper.
   - Group by month, limit to 24 points max.
   - Populate SeriesData.
7. If card has CategoryColumn: query top 5 and bottom 5 by value, populate TopItems/BottomItems.
8. Determine IsFullDataReached: query MIN(DateColumn) from the data source. If it's within the selected range, set true.
9. Determine HasDeeperData: if there is data older than the current depth range.
10. Determine TrendDirection from series data (simple: last point > first = "up", < first = "down", else "stable").
11. Add data quality notes where needed (no previous period, insufficient series, etc).

**Data limits per plan §11:**
- Top/Bottom: 5 items each
- Series: max 24 points
- No raw table transfers

**Program.cs registration:**
```csharp
builder.Services.AddScoped<ICardSummaryBuilder, KpiSummaryBuilder>();
```

Wait — multiple implementations of ICardSummaryBuilder can't all be registered the same way. Use a factory or keyed registration. The cleanest approach: register the builder resolver in C02 only, and C03/C04/C05 don't touch Program.cs.

Actually, best approach: create a simple factory in C02 that C02-C05 share. Or use named services.

Simplest: register a `CardSummaryBuilderFactory` that returns the right builder by ChartType.

Let's do it in C02: create `CardSummaryBuilderFactory.cs` + register it in Program.cs. It receives all builders via DI.

For now (C02 only has KpiSummaryBuilder), the factory just returns it. C03-C05 will add more builders later.

---

## 6. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `KpiSummaryBuilder` implements `ICardSummaryBuilder` |
| AC-2 | Depth date ranges calculated correctly for all 6 levels |
| AC-3 | KPI value queried with correct aggregation and date scope |
| AC-4 | Previous period value queried for comparison |
| AC-5 | Series data populated when DateColumn exists |
| AC-6 | Top/Bottom items populated when CategoryColumn exists |
| AC-7 | IsFullDataReached and HasDeeperData detected correctly |
| AC-8 | Cards without DateColumn handled gracefully (HasDateColumn=false + quality note) |
| AC-9 | All queries parameterized — NO string concatenation for values |
| AC-10 | `dotnet build` passes with 0 errors and 0 warnings |
| AC-11 | Data limits respected (5 top, 5 bottom, 24 series) |

---

## 7. Pre-Execution Gate

```
Gate: PASS | Security: Medium (read-only SQL, parameterized) | Model: Medium
Technology Profile: dotnet-razorpages-adonet ✅
```

## 8. Handback

1. Files created/modified with line counts.
2. Full KpiSummaryBuilder.cs.
3. Build result (0 errors, 0 warnings).
4. Confirmation: all queries parameterized, data limits respected.
