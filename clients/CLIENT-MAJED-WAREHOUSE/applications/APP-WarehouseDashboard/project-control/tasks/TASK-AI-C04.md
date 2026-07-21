# TASK-AI-C04 — Table Card Summary Builder

> **Status:** Draft → Approved  
> **Batch:** AI-C-2 (Parallel with C02, C03, C05)  
> **Depends On:** TASK-AI-C01  
> **Phase:** AI Assistant — Phase C  

---

## 1. Objective

Create `TableSummaryBuilder : ICardSummaryBuilder` — builds summaries for table-type cards. Highlights row count, column patterns, sample rows, and numeric summaries.

---

## 2. ClientAppPath

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Services\TableSummaryBuilder.cs` — **NEW**
2. No Program.cs changes.

---

## 4. Technical Spec

Implements `ICardSummaryBuilder`. Injects `ReadOnlyQueryHelper`.

**Logic:**

1. Populate card metadata.
2. Handle date depth if DateColumn exists (same pattern as C02).
3. Query total row count: `SELECT COUNT(*) FROM ({card.SqlQuery}) sub` (with date WHERE if applicable).
4. Query sample rows: `SELECT TOP 10 * FROM ({card.SqlQuery}) sub` (with optional ORDER BY if DateColumn exists, otherwise arbitrary).
   - Populate SampleRows (list of dictionaries).
5. Detect numeric columns in the results: for each numeric column, run:
   - `SUM`, `AVG`, `MIN`, `MAX` and populate ColumnSummaries.
6. If CategoryColumn exists: query top/bottom 5.
7. If DateColumn: series data (optional for table cards — only if clearly temporal).
8. Detect missing values: if any column has NULLs in sample, add quality note.
9. Detect unusual value ranges: if MAX is > 10x MIN, add quality note.

**Data limits:** 10 sample rows, 5 top, 5 bottom.

---

## 5. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `TableSummaryBuilder` implements `ICardSummaryBuilder` |
| AC-2 | Row count queried correctly |
| AC-3 | Sample rows populated (max 10) |
| AC-4 | Numeric column summaries calculated (Sum/Avg/Min/Max) |
| AC-5 | Missing/unusual values detected |
| AC-6 | All queries parameterized |
| AC-7 | `dotnet build` passes with 0 errors and 0 warnings |

---

## 6. Pre-Execution Gate

```
Gate: PASS | Security: Medium | Model: Medium | Technology Profile: ✅
```

## 7. Handback

1. Files created.
2. Full TableSummaryBuilder.cs.
3. Build result.
4. Confirmation: parameterized, limits respected.
