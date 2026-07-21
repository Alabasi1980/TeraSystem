# QA Report — AI Dashboard Assistant

**Task:** TASK-AI-F01 — QA: Build + Code Review + Acceptance Criteria Audit
**Agent:** QA Agent (Execution Mode)
**Date:** 2026-07-21
**ClientAppPath:** `clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`
**Scope:** All AI Assistant code from TASK-AI-B01 through TASK-AI-E04

---

## 1. Build Result

| Test | Result | Details |
|---|---|---|
| `dotnet build` | ✅ **PASS** | 0 errors, 0 warnings |

Build command executed:
```
dotnet build src\WarehouseDashboard.Web\WarehouseDashboard.Web.csproj
```
Output: `Build succeeded. 0 Warning(s) 0 Error(s)`

---

## 2. Security Scan

### 2a. API Key Exposure

| Check | Result | Details |
|---|---|---|
| Hardcoded API keys in source | ✅ **PASS** | `AIAssistantOptions.ApiKey` defaults to `string.Empty` |
| ApiKey in appsettings.json | ✅ **PASS** | `"ApiKey": ""` — empty, no real key |
| `Bearer` token usage | ✅ **PASS** | Only in `OpenCodeGoAdapter` line 59, reads from `_options.ApiKey` |
| Real passwords in appsettings.json | ⚠️ **FLAG** | SQL Server password (`013590`) and Oracle password (`COGNOS`) are in plain text. Pre-existing, not introduced by AI tasks. |

### 2b. ReadOnlyQueryHelper Safety

| Check | Result | Details |
|---|---|---|
| Write operations (INSERT, UPDATE, DELETE) | ✅ **PASS** | Zero write keywords anywhere in `ReadOnlyQueryHelper.cs` (60 lines). Only `ExecuteReaderAsync()` — no `ExecuteNonQuery`. |
| DDL operations (DROP, ALTER) | ✅ **PASS** | None present |
| Stored procedure execution | ✅ **PASS** | No `EXEC` keywords |
| Parameterized queries | ✅ **PASS** | Uses `command.Parameters.AddWithValue()` — all values are dictionary-parameterized |
| Connection string | ✅ **PASS** | Uses `SqlServerReadOnly` connection string with `ApplicationIntent=ReadOnly` |

Supporting evidence: `SqlReadonlyGuard.cs` exists as an additional defense-in-depth layer for admin query testing (blocks INSERT/UPDATE/DELETE/MERGE/DROP/CREATE/ALTER/TRUNCATE/EXEC/EXECUTE/INTO and more). However, the AI builders use `ReadOnlyQueryHelper`, not the guard. The guard is for admin query tester only.

### 2c. SQL Injection in Summary Builders

All four builders construct SQL strings via interpolation. Analysis:

| Builder | Column Interpolation | Value Parameterization | Safe? |
|---|---|---|---|
| `KpiSummaryBuilder` | `dateCol`, `valueCol`, `catCol` → interpolated **without** `ValidateIdentifier` | ✅ Uses `@from`, `@to` parameters | ⚠️ Column injection possible if admin config is compromised |
| `ChartSummaryBuilder` | `dateCol`, `valueCol`, `catCol` → interpolated **without** `ValidateIdentifier` | ✅ Uses `@dFrom`, `@dTo`, `@catN` parameters | ⚠️ Same as above |
| `TableSummaryBuilder` | All columns → validated via `ValidateIdentifier()` (rejects `;`, `--`, `/*`, `*/`, `''`, `xp_`, `EXEC`) | ✅ Uses `@dFrom`, `@dTo` parameters | ✅ Strong |
| `GenericSummaryBuilder` | Columns → `SanitiseColumn()` (strips bracket-wrapping only) | ✅ No user values interpolated | ⚠️ Weak sanitization |

**Verdict:** Values are always parameterized (✅). Column names come from admin-configured card metadata (not end-user input). `TableSummaryBuilder` has the strongest column validation. `KpiSummaryBuilder` and `ChartSummaryBuilder` lack `ValidateIdentifier()`. Risk is **low** because column names are admin-configured, but defense-in-depth is inconsistent.

### 2d. AssistantEnabled Guard

| Location | Check | Result |
|---|---|---|
| `CardInsightAnalyze.OnPostAsync` (API endpoint) | Line 58: `if (!card.AssistantEnabled)` | ✅ Returns error before any AI processing |
| `CardInsightService.AnalyzeCardWithCacheAsync` | Line 176: `if (card == null \|\| !card.AssistantEnabled)` | ✅ Double-guard (redundant, but safe) |
| `CardInsightService.AnalyzeCardAsync` (legacy) | Line 83: `if (!card.AssistantEnabled)` | ✅ Guarded |

---

## 3. Architecture Verification

### 3a. Builder → ICardSummaryBuilder + ChartType

| Builder | `ChartType` | Implements `ICardSummaryBuilder`? |
|---|---|---|
| `KpiSummaryBuilder` | `"KPI"` | ✅ |
| `ChartSummaryBuilder` | `"Bar"` | ✅ |
| `TableSummaryBuilder` | `"Table"` | ✅ |
| `GenericSummaryBuilder` | `"*"` | ✅ |

### 3b. CardSummaryBuilderFactory

| Check | Result |
|---|---|
| Receives `IEnumerable<ICardSummaryBuilder>` from DI | ✅ Line 13 |
| Builds dictionary by `ChartType` (case-insensitive) | ✅ Lines 15-18 |
| Fallback to `"*"` (GenericSummaryBuilder) | ✅ Lines 34-35 |
| Fallback to first builder if no `"*"` | ✅ Line 38 |

### 3c. Cache-Before-AI

| Check | Result |
|---|---|
| `AnalyzeCardWithCacheAsync` checks cache first | ✅ Line 157: `_cacheService.TryGetCached(...)` |
| Cache hit → returns immediately (no AI call) | ✅ Lines 159-172 |
| Cache miss → calls AI → caches on success | ✅ Lines 207-208 |

### 3d. AssistantLogService Coverage

| Scenario | Logged? | Lines |
|---|---|---|
| Cache hit | ✅ | 159-163 |
| Card not found / disabled | ✅ | 178-180 |
| AI success | ✅ | 215-216 |
| AI error | ✅ | 215-216 (with `errorCode = "AI_ERROR"`) |
| Exception | ✅ | 234-235 (with `errorCode = "EXCEPTION"`) |

### 3e. Program.cs DI Registrations

| Registration | Present? | Line |
|---|---|---|
| `IAIProvider → OpenCodeGoAdapter` | ✅ | 58 |
| `ICardSummaryBuilder → KpiSummaryBuilder` | ✅ | 70 |
| `ICardSummaryBuilder → ChartSummaryBuilder` | ✅ | 71 |
| `ICardSummaryBuilder → TableSummaryBuilder` | ✅ | 72 |
| `ICardSummaryBuilder → GenericSummaryBuilder` | ✅ | 73 |
| `CardSummaryBuilderFactory` | ✅ | 74 |
| `CardInsightService` | ✅ | 60 |
| `AssistantLogService` | ✅ | 63 |
| `AssistantCacheService` (Singleton) | ✅ | 67 |
| `IMemoryCache` | ✅ | 66 |
| `AIAssistantOptions` (Config binding) | ✅ | 57 |
| **`ReadOnlyQueryHelper`** | ❌ **MISSING** | — |

---

## 4. Acceptance Criteria (Static Verification)

| AC | Criterion | Result | Evidence |
|---|---|---|---|
| AC-10 | No full raw tables sent to AI | ✅ **PASS** | All builders use TOP (max 10-24 rows) and aggregates (SUM/AVG/MIN/MAX). No `SELECT *` without LIMIT for AI data. |
| AC-11 | SQL access is read-only | ✅ **PASS** | `ReadOnlyQueryHelper` has zero write operations. `SqlServerReadOnly` connection string with `ApplicationIntent=ReadOnly`. |
| AC-12 | AI has no direct DB access | ✅ **PASS** | `OpenCodeGoAdapter` uses HTTP only. No SQL Server connection strings, no `SqlConnection`, no `DbContext`. |
| AC-14 | Card-specific prompt optional + subordinate | ✅ **PASS** | `تعليمات خاصة لهذه البطاقة` prefix used in `CardInsightService` lines 96 and 191, and `OpenCodeGoAdapter` line 35. General prompt rule (line 47): "إذا تعارضت تعليمات البطاقة مع القواعد العامة، تجاهل الجزء المتعارض واتبع القواعد العامة." |
| AC-19 | No secrets in logs/prompts/UI | ✅ **PASS** | `ApiKey` defaults to `string.Empty`. `DataHelper.Sanitize()` strips SQL_PASSWORD from error messages. AI prompt contains no credentials. |

---

## 5. Findings

### 🔴 STOP (F1) — Missing `ReadOnlyQueryHelper` DI Registration

**Severity:** Critical — Runtime crash
**File:** `src\WarehouseDashboard.Web\Program.cs`
**Details:** `ReadOnlyQueryHelper` is injected as a constructor dependency into:
- `CardInsightService` (line 55 of CardInsightService.cs)
- `KpiSummaryBuilder` (line 29)
- `ChartSummaryBuilder` (line 34)
- `TableSummaryBuilder` (line 33)
- `GenericSummaryBuilder` (line 28)

But it is **never registered** in the DI container (`Program.cs`). The application will throw `InvalidOperationException: Unable to resolve service for type 'WarehouseDashboard.Web.Infrastructure.ReadOnlyQueryHelper'` on the first request to any AI endpoint or card builder endpoint.

**Fix:** Add the following line to `Program.cs` (after line 52, alongside other service registrations):
```csharp
builder.Services.AddScoped<ReadOnlyQueryHelper>();
```

---

### 🟡 CAUTION (F2) — Duplicate Card-Specific Prompt

**Severity:** Medium — Functional defect (duplicate content in AI prompt)
**Files:** `CardInsightService.cs` (line 191) + `OpenCodeGoAdapter.cs` (line 35)

**Flow:**
1. `CardInsightService.AnalyzeCardWithCacheAsync` appends `تعليمات خاصة لهذه البطاقة:\n{card.AssistantPrompt}` to `systemPrompt` (line 191)
2. Also passes `CardAssistantPrompt = card.AssistantPrompt` separately (line 199)
3. `OpenCodeGoAdapter.SendAsync` then appends AGAIN: `تعليمات خاصة لهذه البطاقة:\n{request.CardAssistantPrompt}` (line 35)

**Result:** The card-specific prompt appears **twice** in the final message sent to the AI model.

**Fix:** Remove the duplicate. Either:
- (Option A) Remove lines 32-36 in `OpenCodeGoAdapter.cs` (adapter trusts what it receives), OR
- (Option B) Remove lines 189-191 in `CardInsightService.cs` (let adapter handle it)

Since `CardInsightService` also does this for the legacy `AnalyzeCardAsync` path (line 96), the cleanest fix is Option A.

---

### 🟡 FLAG (F3) — Data Summary Built But Not Sent to AI

**Severity:** Medium — Functional gap
**File:** `CardInsightAnalyze.cshtml.cs` (lines 67-88)

**Details:** The API controller (`CardInsightAnalyze.OnPostAsync`) uses `CardSummaryBuilderFactory` to build a full data summary (series, aggregates, sample rows, etc.) via `builder.BuildSummaryAsync(card, request.DepthLevel, ct)`. However, it only extracts depth metadata (`isFullDataReached`, `hasDeeperData`, `depthLabel`) from the summary — the actual data content is **never passed** to `CardInsightService.AnalyzeCardWithCacheAsync`.

The `AnalyzeCardWithCacheAsync` method only receives `cardId`, `mode`, `depthLevel`, `dataScopeLabel`, `isFullDataReached`, `hasDeeperData` — no structured data summary. Internally, it calls `BuildUserMessage()` which only sends the card title, type, description, and depth labels.

**Impact:** The AI model receives only card metadata, not the actual data it should analyze. The Phase C data summary infrastructure is built but not wired to the Phase B AI calling pipeline.

**Fix:** `AnalyzeCardWithCacheAsync` needs to accept a `CardSummary` parameter so the data content can be included in the user message sent to the AI.

---

### 🟡 CAUTION (F4) — Inconsistent Column Name Validation

**Severity:** Low — Defense-in-depth gap
**Files:** `KpiSummaryBuilder.cs`, `ChartSummaryBuilder.cs` vs. `TableSummaryBuilder.cs`

`TableSummaryBuilder` has a thorough `ValidateIdentifier()` method that rejects SQL injection patterns (`;`, `--`, `/*`, `*/`, `'`, `"`, `xp_`, `EXEC`). `KpiSummaryBuilder` and `ChartSummaryBuilder` interpolate column names directly without this validation. Column names originate from admin-configured card metadata (not end-user input), so the risk is low, but consistency would improve security posture.

---

### 🔵 FLAG (F5) — Plaintext Passwords in appsettings.json

**Severity:** Low (pre-existing, out of AI scope)
**File:** `appsettings.json`
**Details:** SQL Server password (`013590`) and Oracle password (`COGNOS`) are hardcoded. The `ConnectionStringHelper` supports `{SQL_PASSWORD}` and `{ORACLE_PASSWORD}` placeholders from environment variables, but the current `appsettings.json` has real passwords directly. This is pre-existing and not introduced by AI tasks.

---

## 6. Overall Verdict

### ❌ FAIL — Must Fix Before Acceptance

| Category | Status |
|---|---|
| Build | ✅ PASS |
| Security — API Keys | ✅ PASS |
| Security — Read-Only SQL | ✅ PASS |
| Security — Parameterized Queries | ✅ PASS |
| AssistantEnabled Guards | ✅ PASS |
| Architecture — Builders/Factory | ✅ PASS |
| Architecture — Caching | ✅ PASS |
| Architecture — Logging Coverage | ✅ PASS |
| Architecture — DI Registrations | ❌ **FAIL** (F1 — Missing ReadOnlyQueryHelper) |
| Acceptance Criteria (AC-10,11,12,14,19) | ✅ PASS |
| Functional — Prompt Duplication | ⚠️ CAUTION (F2) |
| Functional — Summary Not Sent to AI | ⚠️ FLAG (F3) |

**The F1 issue is a blocker:** the application will crash at runtime when any AI endpoint or card builder is invoked. The application cannot function without this fix.

**F2** and **F3** are functional issues that degrade AI response quality but do not block basic operation (once F1 is fixed).

### Minimum Fix Required to Change Verdict to PASS:
```csharp
// In Program.cs, add before or after line 74:
builder.Services.AddScoped<ReadOnlyQueryHelper>();
```

---

## QA Agent Signature

```
Agent: QA Agent (Execution Mode)
Task: TASK-AI-F01
Date: 2026-07-21
Verdict: FAIL — 1 STOP finding (F1), 2 CAUTION findings (F2, F3)
Handback: project-control/test-reports/TASK-AI-F01-TEST-REPORT.md
```
