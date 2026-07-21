# TASK-AI-C05 — Generic / No-Date-Column Summary Builder

> **Status:** Draft → Approved  
> **Batch:** AI-C-2 (Parallel with C02, C03, C04)  
> **Depends On:** TASK-AI-C01  
> **Phase:** AI Assistant — Phase C  

---

## 1. Objective

Create `GenericSummaryBuilder : ICardSummaryBuilder` — handles cards with unknown/unusual ChartType and cards with no date column. Also acts as the fallback builder.

---

## 2. ClientAppPath

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Services\GenericSummaryBuilder.cs` — **NEW**
2. No Program.cs changes.

---

## 4. Technical Spec

Implements `ICardSummaryBuilder`. Injects `ReadOnlyQueryHelper`.

**Logic:**

1. Populate card metadata.
2. Check DateColumn:
   - If EXISTS: apply depth scope (same as C02 pattern).
   - If NOT: set `HasDateColumn=false`, add quality note "هذه البطاقة لا تحتوي على بُعد زمني واضح".
3. Query basic info:
   - `SELECT * FROM ({card.SqlQuery}) sub` (with date WHERE if applicable, or no filter).
   - COUNT(*) for TotalRowCount.
   - TOP 10 for SampleRows.
4. Detect numeric columns and calculate ColumnSummaries.
5. If CategoryColumn exists: group by it, populate TopItems.
6. Set `IsFullDataReached=true` when no DateColumn (all data is available).
7. Set `HasDeeperData=false` when no DateColumn.
8. Add quality notes for:
   - Unknown ChartType.
   - No date column.
   - Insufficient data.

**Data limits:** 10 samples, 5 items.

---

## 5. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `GenericSummaryBuilder` implements `ICardSummaryBuilder` |
| AC-2 | No-date cards: HasDateColumn=false, quality note added |
| AC-3 | Date cards: depth scope applied |
| AC-4 | Basic row count + sample rows populated |
| AC-5 | Column summaries for numeric columns |
| AC-6 | `dotnet build` passes with 0 errors and 0 warnings |

---

## 6. Pre-Execution Gate

```
Gate: PASS | Security: Medium | Model: Medium | Technology Profile: ✅
```

## 7. Handback

1. Files created.
2. Full GenericSummaryBuilder.cs.
3. Build result.
