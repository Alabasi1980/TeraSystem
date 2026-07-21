# QUAUD — AI Assistant Architecture & Dependency Injection Audit

**Audit ID:** QUAUD-AI-ARCHITECTURE-2026-07-21
**Task Reviewed:** TASK-AI-C02 / AI Dashboard Assistant Architecture & DI
**Invoked By:** TeraAgent
**Audit Mode:** Full Risk-Based
**Scope:** Architecture verification — interface compliance, DI wiring, factory resolution, API endpoint flow
**Report Path:** `project-control/audit-reports/QUAUD-AI-ARCHITECTURE-2026-07-21.md`
**Evidence Sources Used:**
- `src/WarehouseDashboard.Web/Infrastructure/ICardSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/KpiSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/ChartSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/TableSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/GenericSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/CardSummaryBuilderFactory.cs`
- `src/WarehouseDashboard.Web/Program.cs`
- `src/WarehouseDashboard.Web/Pages/Api/CardInsightAnalyze.cshtml.cs`

---

## Overall Quality Gate: PASS

All four architectural checks pass without blocking findings. Two FLAG-level observations noted.

---

## Findings Summary

| Severity | Count |
|---|---|
| STOP | 0 |
| CAUTION | 0 |
| FLAG | 2 |
| BASELINE_DEBT | 0 |

---

## Check 1: Interface Compliance — PASS

### Verification

| Builder | Implements ICardSummaryBuilder | ChartType | Status |
|---|---|---|---|
| KpiSummaryBuilder (line 11) | Yes | `"KPI"` (line 27) | ✅ |
| ChartSummaryBuilder (line 11) | Yes | `"Bar"` (line 32) | ✅ |
| TableSummaryBuilder (line 13) | Yes | `"Table"` (line 31) | ✅ |
| GenericSummaryBuilder (line 10) | Yes | `"*"` (line 14) | ✅ |

All four builders implement the `ICardSummaryBuilder` interface (defined in `Infrastructure/ICardSummaryBuilder.cs`) with the `ChartType` property and `BuildSummaryAsync(DashboardCard, int, CancellationToken)` method signature.

**Result: PASS.**

---

## Check 2: CardSummaryBuilderFactory Resolution — PASS

### Verification

**File:** `src/WarehouseDashboard.Web/Services/CardSummaryBuilderFactory.cs`

| Criterion | Location | Evidence | Status |
|---|---|---|---|
| Accepts `IEnumerable<ICardSummaryBuilder>` | Constructor (line 13) | `public CardSummaryBuilderFactory(IEnumerable<ICardSummaryBuilder> builders)` | ✅ |
| Builds dictionary keyed by `ChartType` | Lines 15–17 | `builders.ToDictionary(b => b.ChartType, StringComparer.OrdinalIgnoreCase)` | ✅ |
| Exact match resolution | Line 30–31 | `_builders.TryGetValue(chartType, out var builder)` | ✅ |
| Fallback to `"*"` wildcard | Lines 34–35 | `_builders.TryGetValue("*", out var fallback)` | ✅ |
| Ultimate fallback to first available | Line 38 | `return _builders.Values.First()` | ✅ |
| Guards against zero builders | Lines 19–21 | Throws `InvalidOperationException` when empty | ✅ |

**Resolution chain:** exact ChartType match → `"*"` wildcard (GenericSummaryBuilder) → first registered builder. This is a robust three-tier fallback strategy.

**Result: PASS.**

---

## Check 3: DI Registrations in Program.cs — PASS

### Verification

**File:** `src/WarehouseDashboard.Web/Program.cs`

| # | Registration | Location | Status |
|---|---|---|---|
| 1 | `IAIProvider` → `OpenCodeGoAdapter` | Line 58: `AddHttpClient<IAIProvider, OpenCodeGoAdapter>()` | ✅ |
| 2 | `ICardSummaryBuilder` → `KpiSummaryBuilder` | Line 73: `AddScoped<ICardSummaryBuilder, KpiSummaryBuilder>()` | ✅ |
| 3 | `ICardSummaryBuilder` → `ChartSummaryBuilder` | Line 74: `AddScoped<ICardSummaryBuilder, ChartSummaryBuilder>()` | ✅ |
| 4 | `ICardSummaryBuilder` → `TableSummaryBuilder` | Line 75: `AddScoped<ICardSummaryBuilder, TableSummaryBuilder>()` | ✅ |
| 5 | `ICardSummaryBuilder` → `GenericSummaryBuilder` | Line 76: `AddScoped<ICardSummaryBuilder, GenericSummaryBuilder>()` | ✅ |
| 6 | `CardSummaryBuilderFactory` | Line 77: `AddScoped<CardSummaryBuilderFactory>()` | ✅ |
| 7 | `CardInsightService` | Line 60: `AddScoped<CardInsightService>()` | ✅ |
| 8 | `ReadOnlyQueryHelper` | Line 63: `AddScoped<ReadOnlyQueryHelper>()` | ✅ |
| 9 | `AssistantLogService` | Line 66: `AddScoped<AssistantLogService>()` | ✅ |
| 10 | `AssistantCacheService` + `AddMemoryCache` | Lines 69–70: `AddMemoryCache()` + `AddSingleton<AssistantCacheService>()` | ✅ |

All registrations use appropriate lifetimes: services are `Scoped` (per-request), cache dependencies are `Singleton` with `MemoryCache`, and `IAIProvider` is registered via `AddHttpClient` for proper `HttpClient` lifecycle management.

**Result: PASS.**

---

## Check 4: API Endpoint Flow — PASS

### Verification

**File:** `src/WarehouseDashboard.Web/Pages/Api/CardInsightAnalyze.cshtml.cs`

| Criterion | Location | Evidence | Status |
|---|---|---|---|
| Accepts JSON body with `cardId`, `mode`, `depthLevel` | Lines 34–36, 115–120 | `[FromBody] AnalyzeRequest` with `CardId`, `Mode` (default `"explain"`), `DepthLevel` (default `1`) | ✅ |
| Loads card from DB | Line 48 | `_db.DashboardCards.FindAsync(new object[] { request.CardId }, cancellationToken)` | ✅ |
| Returns error for missing card | Lines 49–56 | JsonResult with `Success = false`, `ErrorMessage = "البطاقة غير موجودة."` | ✅ |
| Checks `AssistantEnabled` | Lines 58–65 | `if (!card.AssistantEnabled)` → returns error response | ✅ |
| Calls `CardSummaryBuilderFactory` and builder | Lines 71–72 | `_builderFactory.GetBuilder(card.ChartType)` → `builder.BuildSummaryAsync(card, request.DepthLevel, cancellationToken)` | ✅ |
| Graceful builder failure handling | Lines 74–81 | Catches exception, logs warning, continues with fallback depth metadata | ✅ |
| Calls `CardInsightService` | Lines 88–95 | `_cardInsightService.AnalyzeCardWithCacheAsync(cardId, mode, depthLevel, ...)` | ✅ |
| Returns `CardInsightResponse` JSON | Line 97 | `return new JsonResult(result)` | ✅ |
| Cancellation token propagation | Lines 36, 48, 72, 95 | `CancellationToken` passed through entire chain | ✅ |

**Flow summary:**
```
POST /api/card-insights/analyze  { cardId, mode, depthLevel }
  → Validate request (null check)
  → Load DashboardCard from DB
  → Check AssistantEnabled flag
  → CardSummaryBuilderFactory.GetBuilder(chartType) → builder.BuildSummaryAsync()
  → CardInsightService.AnalyzeCardWithCacheAsync()
  → Return CardInsightResponse as JSON
```

Builder failure is handled gracefully — if `BuildSummaryAsync` throws (e.g., SQL error), the endpoint logs the warning and falls back to basic depth metadata before calling the AI service. This avoids a hard failure.

**Result: PASS.**

---

## Observations (FLAG)

### Finding F-001: ChartSummaryBuilder aliasing mismatch

| Field | Detail |
|---|---|
| **Finding ID:** | QUAUD-AI-ARCH-001 |
| **Rule ID:** | N/A (Design observation) |
| **Domain:** | Architecture / DI |
| **Severity:** | FLAG |
| **Location:** | `ChartSummaryBuilder.cs` line 51 comment: `"Factory primary key; aliased for Line/Pie/Gauge"` |
| **Evidence:** | `ChartSummaryBuilder.ChartType` returns `"Bar"`. `CardSummaryBuilderFactory` resolves by exact match first. Cards with `ChartType = "Line"`, `"Pie"`, or `"Gauge"` will NOT match `"Bar"` and will fall through to `GenericSummaryBuilder` (`"*"`). |
| **Expected Standard:** | Either register separate builders for Line/Pie/Gauge, or confirm via documentation that GenericSummaryBuilder is the intended handler for these chart types. |
| **Observed Condition:** | The comment implies ChartSummaryBuilder should handle Line/Pie/Gauge, but the factory's exact-match resolution prevents this. Line/Pie/Gauge cards receive the generic summary, not the chart-specific distribution/series analysis. |
| **Impact:** | Line, Pie, and Gauge cards miss specialized chart analysis (category distribution, top-category series, outlier detection) and receive only GenericSummaryBuilder's basic row-count/column-aggregate analysis. |
| **Recommended Action:** | Either: (a) remove the misleading comment and document that Line/Pie/Gauge are intentionally handled by GenericSummaryBuilder, or (b) register additional ICardSummaryBuilder implementations or aliases so Line/Pie/Gauge route to ChartSummaryBuilder's richer analysis pipeline. |
| **Changed Code / Baseline:** | Baseline — design decision in initial architecture |
| **Confidence:** | High |
| **Blocking:** | No |
| **Waiver Allowed:** | Yes |
| **Required Owner:** | TeraAgent / System Designer |
| **Referral:** | None — Auditor observation only |
| **Status:** | Open |

### Finding F-002: [IgnoreAntiforgeryToken] on API endpoint

| Field | Detail |
|---|---|
| **Finding ID:** | QUAUD-AI-ARCH-002 |
| **Rule ID:** | QG-SEC-005 (token validation bypass) |
| **Domain:** | Security Hygiene |
| **Severity:** | FLAG |
| **Location:** | `CardInsightAnalyze.cshtml.cs` line 14: `[IgnoreAntiforgeryToken]` |
| **Evidence:** | The `CardInsightAnalyzeModel` PageModel is decorated with `[IgnoreAntiforgeryToken]`, which disables CSRF validation for this endpoint. This is a common pattern for API-style POST endpoints in Razor Pages served to JavaScript clients. |
| **Expected Standard:** | CSRF protection should be in place unless the endpoint is inherently safe from CSRF (e.g., uses custom auth headers, is read-only in effect, or the cookie uses `SameSite=Strict`). |
| **Observed Condition:** | The session cookie uses `SameSite=Strict` (Program.cs line 30), which provides partial CSRF protection in modern browsers. However, `[IgnoreAntiforgeryToken]` removes the server-side defense layer. |
| **Impact:** | Low — the session cookie's `SameSite=Strict` mitigates most CSRF risk for this endpoint. The endpoint is read-only (no state mutation aside from logging/caching). |
| **Recommended Action:** | Document the CSRF posture explicitly: note that `SameSite=Strict` is the primary defense and that the endpoint is effectively read-only. Consider adding a custom header requirement or explicit `[ValidateAntiForgeryToken]` if the cookie SameSite policy ever changes. |
| **Changed Code / Baseline:** | Baseline |
| **Confidence:** | Medium |
| **Blocking:** | No |
| **Waiver Allowed:** | Yes |
| **Required Owner:** | TeraAgent |
| **Referral:** | SecurityAgent (optional, for deeper CSRF posture review) |
| **Status:** | Open |

---

## Positive Findings

1. **Robust SQL injection defense:** `TableSummaryBuilder.ValidateIdentifier()` (lines 452–472) explicitly rejects SQL injection characters (`;`, `--`, `/*`, `*/`, `'`, `"`, `xp_`, `EXEC`) in column identifiers before interpolation. `GenericSummaryBuilder.SanitiseColumn()` strips bracket-wrapping. All value parameters use parameterized queries throughout all builders.

2. **Three-tier fallback factory:** The `CardSummaryBuilderFactory` implements a resilient resolution strategy: exact match → `"*"` wildcard → first available builder. This prevents runtime failures for unknown chart types.

3. **Graceful degradation in API endpoint:** Builder failure (e.g., SQL error) is caught, logged, and the AI analysis continues with fallback depth metadata — the user still receives an analysis rather than a hard error.

4. **Cancellation token propagation:** `CancellationToken` is threaded through the entire call chain from the HTTP endpoint through the factory, builder, and AI service.

5. **Clean DI composition:** All services are registered with appropriate lifetimes (`Scoped` for request-scoped services, `Singleton` for cache, `AddHttpClient` for HTTP-backed adapter). No anti-patterns like `ServiceLocator` or manual `new` of injected dependencies.

---

## Handback to Orchestrator

- **Status:** PASS
- **Report Path:** `project-control/audit-reports/QUAUD-AI-ARCHITECTURE-2026-07-21.md`
- **Blocking Findings:** None
- **Recommended Next Action:** Review FLAG findings F-001 (ChartSummaryBuilder routing gap) and F-002 (CSRF token posture). Neither blocks acceptance. Proceed with task closure unless TeraAgent determines F-001 warrants a design adjustment task.
