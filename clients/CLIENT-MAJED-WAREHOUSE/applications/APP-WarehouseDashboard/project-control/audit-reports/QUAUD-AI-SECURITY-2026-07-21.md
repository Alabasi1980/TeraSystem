# QUAUD-AI-SECURITY-2026-07-21

## Audit Metadata

| Field | Value |
|---|---|
| **Audit ID** | QUAUD-AI-SECURITY-2026-07-21 |
| **Task Reviewed** | AI Assistant: Security & Data Access Layer |
| **Invoked By** | TeraAgent (via Majed) |
| **Audit Mode** | Full Risk-Based |
| **Scope** | Changed Code + Security-sensitive data-access and API integration paths |
| **ClientAppPath** | `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard` |
| **Report Path** | `project-control/audit-reports/QUAUD-AI-SECURITY-2026-07-21.md` |
| **Evidence Sources Used** | Direct file reading of 8 source files + 1 config file; governance thresholds |

---

## Overall Quality Gate: NEEDS_FIX

| Finding Severity | Count |
|---|---|
| STOP | 0 |
| CAUTION | 1 |
| FLAG | 4 |
| BASELINE_DEBT | 0 |

**Verdict:** All five primary security checks **PASS**. One CAUTION finding (information leakage in GenericSummaryBuilder error messages) requires a fix or explicit waiver. Four FLAG-level observations are recorded for tracking.

---

## Findings Summary

| # | Severity | Check | Summary |
|---|---|---|---|
| CAUTION-01 | CAUTION | Check 3 | GenericSummaryBuilder exposes raw exception messages in user-facing DataQualityNotes |
| FLAG-01 | FLAG | Check 1 | ReadOnlyQueryHelper accepts raw SQL without SELECT validation (relies on DB-level ReadOnly intent) |
| FLAG-02 | FLAG | Check 3 | Inconsistent column-identifier validation across builders |
| FLAG-03 | FLAG | Check 5 | AnalyzeCardWithCacheAsync serves cached response before checking AssistantEnabled |
| FLAG-04 | FLAG | Check 4 | Plaintext database passwords in appsettings.json (baseline observation) |

---

## Check 1: ReadOnlyQueryHelper — Write Operation Audit

**Result: PASS** ✅

**File:** `src\WarehouseDashboard.Web\Infrastructure\ReadOnlyQueryHelper.cs`

### Evidence

- The file contains exactly **one public method**: `QueryAsync` (lines 27–59).
- It uses `ExecuteReaderAsync()` (line 45) which is a read-only ADO.NET operation.
- **Zero instances** of INSERT, UPDATE, DELETE, DROP, ALTER, EXEC, TRUNCATE, MERGE, or any write statements found anywhere in the file (60 lines, fully read).
- All user-supplied values are passed via `Dictionary<string, object>? parameters` and bound with `AddWithValue` (line 40). No string concatenation of user values.
- Connection string `SqlServerReadOnly` (appsettings.json line 4) includes `ApplicationIntent=ReadOnly`, which causes SQL Server to reject any write operation at the database engine level — providing defense-in-depth.

### FLAG-01: No SQL Content Validation

| Attribute | Detail |
|---|---|
| **Finding ID** | FLAG-01 |
| **Rule ID** | QG-SEC-003 (proximity) |
| **Domain** | Security — Defense in Depth |
| **Severity** | FLAG |
| **Location** | `ReadOnlyQueryHelper.cs:27-28` |
| **Evidence** | Method signature `QueryAsync(string sql, ...)` accepts any SQL string with no check that it begins with SELECT |
| **Expected Standard** | A class named "ReadOnlyQueryHelper" should validate or restrict its `sql` parameter to SELECT-like operations |
| **Observed Condition** | The class trusts callers to pass only SELECT queries; defense relies entirely on `ApplicationIntent=ReadOnly` at the connection-string level |
| **Impact** | If a caller is compromised or a bug introduces a write statement, the DB-level guard catches it — but the class itself provides no application-layer validation |
| **Recommended Action** | Consider adding a lightweight guard: `if (!sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Only SELECT queries are allowed.");` |
| **Changed Code / Baseline** | Changed code |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes — DB-level guard is active and effective |
| **Required Owner** | Engineering |
| **Referral** | None |
| **Status** | Open |

---

## Check 2: OpenCodeGoAdapter — API Key Safety

**Result: PASS** ✅

**File:** `src\WarehouseDashboard.Web\Infrastructure\OpenCodeGoAdapter.cs`

### Evidence: Every Catch and Log Path Audited

| Location | Code | Key in Log? |
|---|---|---|
| Line 59 | `new AuthenticationHeaderValue("Bearer", _options.ApiKey)` | N/A — correct usage |
| Lines 68–71 | `LogError("OpenCodeGo API returned HTTP {StatusCode}. Response body length: {BodyLength}", ...)` | **No** — only status code and body length |
| Line 108 | `LogError(ex, "HTTP error contacting the AI provider.")` | **No** |
| Line 120 | `LogError(ex, "AI provider request timed out after {TimeoutSeconds}s.", ...)` | **No** |
| Line 141 | `LogError(ex, "Unexpected error in AI provider adapter.")` | **No** |

### Evidence: All Error Messages (returned to caller)

| Line(s) | ErrorMessage Content |
|---|---|
| 75 | `"The AI service returned an error (HTTP {status})."` |
| 112 | `"A network error occurred while contacting the AI service."` |
| 124 | `"The AI service request timed out."` |
| 134 | `"The AI request was cancelled."` |
| 145 | `"An unexpected error occurred while processing the AI request."` |

- **Zero instances** of `_options.ApiKey`, `ApiKey`, or the key value itself in any log message, exception, or error string (150 lines fully read).
- Key source: `IOptions<AIAssistantOptions>` (line 16), bound from `appsettings.json` `AIAssistant:ApiKey` — confirmed config-based, not hardcoded.
- Confirmed model: `AIAssistantOptions.cs` line 10, property `ApiKey` defaults to `string.Empty`.

### No Findings

---

## Check 3: All Builders — SQL Injection Prevention

**Result: PASS** ✅ (with one CAUTION finding)

### 3a. KpiSummaryBuilder.cs — PASS ✅

**File:** `src\WarehouseDashboard.Web\Services\KpiSummaryBuilder.cs` (418 lines)

| Query Location | User Values | Method | Verdict |
|---|---|---|---|
| Lines 214–223 (`QueryAggregateValueAsync`) | `dateFrom`, `dateTo` → `@from`, `@to` | `AddWithValue` via `QueryAsync(sql, prms)` | ✅ Parameterized |
| Lines 258–263 (`QuerySeriesDataAsync`) | `dateFrom`, `dateTo` → `@from`, `@to` | `AddWithValue` via `QueryAsync(sql, prms)` | ✅ Parameterized |
| Lines 324–329 (Top 5) | `dateFrom`, `dateTo` → `@from`, `@to` | `AddWithValue` via `QueryAsync(sql, prms)` | ✅ Parameterized |
| Lines 353–358 (Bottom 5) | `dateFrom`, `dateTo` → `@from`, `@to` | `AddWithValue` via `QueryAsync(sql, prms)` | ✅ Parameterized |
| Line 178 (`DetectDataBoundsAsync`) | None (only `dateCol` from config) | N/A — no user values | ✅ |

Column names (`valueCol`, `dateCol`, `catCol`) and `sqlQuery` come from admin-configured card metadata (trusted source per audit scope). No user-input string concatenation for SQL construction.

### 3b. ChartSummaryBuilder.cs — PASS ✅

**File:** `src\WarehouseDashboard.Web\Services\ChartSummaryBuilder.cs` (476 lines)

| Query Location | User Values | Method | Verdict |
|---|---|---|---|
| Lines 179–186 (`BuildCategoryDistributionAsync`) | `dateFrom`, `dateTo` → `@dFrom`, `@dTo` | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 292–298 (`BuildSeriesDataAsync`) | `dateFrom`, `dateTo` → `@sFrom`, `@sTo` | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 380–399 (`BuildTopCategorySeriesAsync`) | `dateFrom`, `dateTo` → `@tFrom`, `@tTo`; category names → `@cat0`, `@cat1`, `@cat2` | `AddWithValue` via `QueryAsync` | ✅ Parameterized (including category values!) |
| Line 334 (minDate) | None | N/A | ✅ |

Notable: `BuildTopCategorySeriesAsync` (lines 380–386) properly parameterizes **actual data values** (category names) with `@cat0`, `@cat1`, `@cat2` — going beyond the minimum requirement.

### 3c. TableSummaryBuilder.cs — PASS ✅

**File:** `src\WarehouseDashboard.Web\Services\TableSummaryBuilder.cs` (473 lines)

| Query Location | User Values | Method | Verdict |
|---|---|---|---|
| Line 174 (`QueryTotalRowCountAsync`) | `dateParams` (dates) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 197–202 (`QuerySampleRowsAsync`) | `dateParams` (dates) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 220–224 (`QueryNumericSummariesAsync`) | `dateParams` (dates) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 292–296, 321–325 (`QueryTopBottomAsync`) | `dateParams` (dates) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 361–365 (`QuerySeriesDataAsync`) | `dateParams` (dates) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |

Extra defense: `ValidateIdentifier` (lines 452–472) blocks common SQL injection characters (`;`, `--`, `/*`, `*/`, `'`, `"`, `xp_`, `EXEC`) in column identifiers before interpolation. This is a **blocklist** approach (not allowlist), but provides meaningful defense against config-compromise injection.

### 3d. GenericSummaryBuilder.cs — PASS ✅ (with CAUTION)

**File:** `src\WarehouseDashboard.Web\Services\GenericSummaryBuilder.cs` (338 lines)

| Query Location | User Values | Method | Verdict |
|---|---|---|---|
| Line 89 (COUNT query) | `dateParams` (`@dateFrom`, `@dateTo`) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Line 107 (TOP 10 sample) | `dateParams` (`@dateFrom`, `@dateTo`) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 222–234 (`BuildColumnSummariesAsync`) | `dateParams` (dates) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |
| Lines 281–285 (`BuildTopItemsAsync`) | `dateParams` (dates) | `AddWithValue` via `QueryAsync` | ✅ Parameterized |

All user values parameterized. Column names from card config (trusted source).

### CAUTION-01: Exception Messages Exposed in User-Facing DataQualityNotes

| Attribute | Detail |
|---|---|
| **Finding ID** | CAUTION-01 |
| **Rule ID** | QG-SEC-INFO-LEAK |
| **Domain** | Security — Information Leakage |
| **Severity** | CAUTION |
| **Location** | `GenericSummaryBuilder.cs:100`, `:122`, `:255`, `:313` |
| **Evidence** | Four catch blocks append `ex.Message` directly to `summary.DataQualityNotes`: <br>• Line 100: `$"تعذر حساب عدد الصفوف: {ex.Message}"` <br>• Line 122: `$"تعذر جلب عينة البيانات: {ex.Message}"` <br>• Line 255: `$"تعذر حساب ملخصات الأعمدة: {ex.Message}"` <br>• Line 313: `$"تعذر حساب أعلى الفئات: {ex.Message}"` |
| **Expected Standard** | Exception messages should not be exposed to end users; use generic messages (as done by KpiSummaryBuilder and ChartSummaryBuilder) |
| **Observed Condition** | `ex.Message` from `SqlException` or other ADO.NET exceptions could leak table names, column names, data types, or partial query fragments |
| **Impact** | Information disclosure to end users if SQL errors occur — could aid reconnaissance in multi-tenant or shared-access scenarios |
| **Recommended Action** | Replace `ex.Message` with generic Arabic messages (matching the pattern used in KpiSummaryBuilder lines 193, 294, 383) |
| **Changed Code / Baseline** | Changed code — these are new methods in the AI assistant data layer |
| **Confidence** | High |
| **Blocking** | No (does not block deployment, but should be fixed in next iteration) |
| **Waiver Allowed** | Yes — if internal-only dashboard with trusted users |
| **Required Owner** | Engineering |
| **Referral** | None |
| **Status** | Open |

---

## Check 4: appsettings.json — No Real API Key

**Result: PASS** ✅

**File:** `src\WarehouseDashboard.Web\appsettings.json`

### Evidence

- Line 24: `"ApiKey": ""` — **Empty string**. Confirmed. ✅
- Scanned all 29 lines for long alphanumeric strings (>20 chars) resembling API keys: **None found.**
- The `BaseUrl` (line 22), `ModelId` (line 23), and `PromptVersion` (line 27) are normal configuration values, not secrets.
- All three connection strings (lines 3–5) contain database credentials (plaintext passwords), which is a security concern but outside the scope of this specific API-key audit.

### FLAG-04: Plaintext Database Passwords (Baseline Observation)

| Attribute | Detail |
|---|---|
| **Finding ID** | FLAG-04 |
| **Rule ID** | QG-SEC-001 (proximity) |
| **Domain** | Security — Credential Management |
| **Severity** | FLAG |
| **Location** | `appsettings.json:3-5` |
| **Evidence** | Connection strings `SqlServer`, `SqlServerReadOnly`, and `Oracle` contain plaintext `Password=` values |
| **Expected Standard** | Secrets should use User Secrets (development) or Azure Key Vault / environment variables (production) |
| **Observed Condition** | Three database passwords embedded in plaintext in committed config file |
| **Impact** | Credential exposure in version control and deployment artifacts |
| **Recommended Action** | Migrate to `dotnet user-secrets` for development; use secure configuration provider for production |
| **Changed Code / Baseline** | **BASELINE_DEBT** — pre-existing, not introduced by AI assistant work |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes — internal non-production environment |
| **Required Owner** | DevOps / Engineering |
| **Referral** | SecurityAgent for full credential audit |
| **Status** | Open (Baseline) |

---

## Check 5: CardInsightService — AssistantEnabled Guard

**Result: PASS** ✅

**File:** `src\WarehouseDashboard.Web\Services\CardInsightService.cs`

### Evidence

**Path A: `AnalyzeCardAsync` (lines 69–112)**

```
Line 73:  var card = await _db.DashboardCards.FindAsync(...)
Line 83:  if (!card.AssistantEnabled)           ← GUARD
Line 84:      return new AIAssistantResponse { Success = false, ErrorMessage = "..." }
...
Line 111: return await _aiProvider.SendAsync(request, ct);  ← AI call AFTER guard
```

The `!card.AssistantEnabled` check at line 83 is the **first decision** after loading the card. If disabled, the method returns immediately without any AI provider call. ✅

**Path B: `AnalyzeCardWithCacheAsync` (lines 145–238)**

```
Line 157: if (_cacheService.TryGetCached(...))      ← CACHE CHECK FIRST
Line 159:     return cached response                 ← Returns WITHOUT checking AssistantEnabled
...
Line 175: var card = await _db.DashboardCards.FindAsync(...)
Line 176: if (card == null || !card.AssistantEnabled)  ← GUARD
Line 177:     return error response
...
Line 202: var aiResponse = await _aiProvider.SendAsync(request, ct);  ← AI call AFTER guard
```

The `!card.AssistantEnabled` guard at line 176 is correctly placed **before** the AI provider call at line 202. ✅

### FLAG-03: Cache Served Before AssistantEnabled Check

| Attribute | Detail |
|---|---|
| **Finding ID** | FLAG-03 |
| **Rule ID** | N/A (design observation) |
| **Domain** | Security — Access Control Consistency |
| **Severity** | FLAG |
| **Location** | `CardInsightService.cs:157-172` |
| **Evidence** | `TryGetCached` (line 157) returns cached AI response before `!card.AssistantEnabled` check (line 176). If a card is disabled after caching, stale cached AI responses are still served until cache TTL expires. |
| **Expected Standard** | Disabling a card's assistant should immediately stop serving AI-generated content for that card |
| **Observed Condition** | Cache-first design serves previously cached AI responses for disabled cards within the cache window |
| **Impact** | Low — cached content is stale but was previously approved/generated; requires cache invalidation on disable |
| **Recommended Action** | Consider cache invalidation when `AssistantEnabled` is set to `false`, or move the `AssistantEnabled` check before the cache lookup |
| **Changed Code / Baseline** | Changed code |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes — intentional cache-first performance design |
| **Required Owner** | Engineering |
| **Referral** | None |
| **Status** | Open |

---

## Additional Cross-Cutting Finding

### FLAG-02: Inconsistent Column Identifier Validation Across Builders

| Attribute | Detail |
|---|---|
| **Finding ID** | FLAG-02 |
| **Rule ID** | N/A (consistency / defense-in-depth) |
| **Domain** | Security — SQL Injection Defense Consistency |
| **Severity** | FLAG |
| **Location** | `TableSummaryBuilder.cs:452-472` vs `KpiSummaryBuilder.cs`, `ChartSummaryBuilder.cs`, `GenericSummaryBuilder.cs` |
| **Evidence** | `TableSummaryBuilder.ValidateIdentifier` applies a blocklist validation (`;`, `--`, `/*`, `*/`, `'`, `"`, `xp_`, `EXEC`) before interpolating column names. The other three builders interpolate column names from card config directly without this validation. |
| **Expected Standard** | Consistent defense-in-depth across all data-access components |
| **Observed Condition** | Only one of four builders validates column identifiers; the other three trust card config values unconditionally |
| **Impact** | Low — column names come from admin-configured card metadata (trusted source per audit scope). If card config is compromised, `KpiSummaryBuilder`, `ChartSummaryBuilder`, and `GenericSummaryBuilder` have no application-layer SQL injection guard on column names. |
| **Recommended Action** | Extract `ValidateIdentifier` to a shared helper and apply it consistently across all builders |
| **Changed Code / Baseline** | Changed code (new builders) |
| **Confidence** | Medium |
| **Blocking** | No |
| **Waiver Allowed** | Yes — card config is admin-trusted |
| **Required Owner** | Engineering |
| **Referral** | None |
| **Status** | Open |

---

## Handback to Orchestrator

| Field | Value |
|---|---|
| **Status** | NEEDS_FIX |
| **Report Path** | `project-control/audit-reports/QUAUD-AI-SECURITY-2026-07-21.md` |
| **Blocking Findings** | None (0 STOP findings) |
| **Open CAUTIONs** | 1 — CAUTION-01 (exception messages in GenericSummaryBuilder DataQualityNotes) |
| **Open FLAGs** | 4 — FLAG-01 through FLAG-04 |
| **Recommended Next Action** | Address CAUTION-01 (replace `ex.Message` with generic Arabic messages in GenericSummaryBuilder). Review FLAG findings and decide accept/defer per finding. All five primary security checks PASS — the implementation is safe to deploy with the CAUTION finding tracked for next iteration. |
