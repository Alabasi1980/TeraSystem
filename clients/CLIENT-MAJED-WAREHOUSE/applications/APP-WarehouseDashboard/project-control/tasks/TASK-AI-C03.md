# TASK-AI-C03 — Chart Card Summary Builder

> **Status:** Draft → Approved  
> **Batch:** AI-C-2 (Parallel with C02, C04, C05)  
> **Depends On:** TASK-AI-C01 (ICardSummaryBuilder + CardSummary)  
> **Phase:** AI Assistant — Phase C (Data Summary Builder)  

---

## 1. Objective

Create `ChartSummaryBuilder : ICardSummaryBuilder` — builds structured data summaries for chart-type cards (Bar, Line, Pie, Gauge).

---

## 2. ClientAppPath

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Sources

- Same as C02: ICardSummaryBuilder, CardSummary, ReadOnlyQueryHelper, DashboardCard

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Services\ChartSummaryBuilder.cs` — **NEW**
2. Do NOT modify Program.cs (the factory in C02 handles registration)

---

## 5. Technical Spec

Implements `ICardSummaryBuilder`. Injects `ReadOnlyQueryHelper`.

**Logic:**

1. Populate card metadata.
2. Handle date depth (same as C02 pattern):
   - If DateColumn exists: scope by date.
   - If not: HasDateColumn=false.
3. Query category distribution:
   - If CategoryColumn exists: `SELECT {CategoryColumn}, SUM({ValueColumn}) FROM sub GROUP BY {CategoryColumn} ORDER BY SUM DESC`
   - Populate TopItems (top 5) and BottomItems (bottom 5) with values and percentages.
   - Calculate distribution balance: if top item > 50% of total, add quality note about imbalance.
4. If DateColumn + CategoryColumn both exist: also query series data (monthly aggregation).
5. Calculate TotalValue as sum of all category values.
6. Set CurrentValue = TotalValue for consistency.
7. Detect outliers: if any category differs > 2x from average, add quality note.
8. Data limits: 5 top, 5 bottom, 24 series.

---

## 6. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `ChartSummaryBuilder` implements `ICardSummaryBuilder` |
| AC-2 | Category distribution queried and populated correctly |
| AC-3 | Percentages calculated per category |
| AC-4 | Top 5 and Bottom 5 items populated |
| AC-5 | Distribution balance signal detected |
| AC-6 | Date scoping applied when DateColumn exists |
| AC-7 | No-DateColumn cards handled gracefully |
| AC-8 | All queries parameterized |
| AC-9 | `dotnet build` passes with 0 errors and 0 warnings |

---

## 7. Pre-Execution Gate

```
Gate: PASS | Security: Medium | Model: Medium
Technology Profile: dotnet-razorpages-adonet ✅
```

## 8. Handback

1. Files created.
2. Full ChartSummaryBuilder.cs.
3. Build result (0 errors, 0 warnings).
4. Confirmation: parameterized queries, data limits.
