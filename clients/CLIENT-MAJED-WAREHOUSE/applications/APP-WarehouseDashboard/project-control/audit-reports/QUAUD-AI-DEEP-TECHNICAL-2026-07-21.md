# QUAUD Report: AI Dashboard Assistant — Deep Technical Audit

**Audit ID:** QUAUD-AI-DEEP-TECHNICAL-2026-07-21
**Task Reviewed:** Full AI Assistant system audit (CLIENT-MAJED-WAREHOUSE / APP-WarehouseDashboard)
**Invoked By:** Majed (direct)
**Audit Mode:** Full Risk-Based
**ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard`
**Report Path:** `project-control/audit-reports/QUAUD-AI-DEEP-TECHNICAL-2026-07-21.md`
**Date:** 2026-07-21

**Evidence Sources Used:**
- `src/WarehouseDashboard.Web/appsettings.json`
- `src/WarehouseDashboard.Web/Infrastructure/AIAssistantOptions.cs`
- `src/WarehouseDashboard.Web/Infrastructure/OpenCodeGoAdapter.cs`
- `src/WarehouseDashboard.Web/Program.cs`
- `src/WarehouseDashboard.Web/Infrastructure/ReadOnlyQueryHelper.cs`
- `src/WarehouseDashboard.Web/Pages/Api/CardInsightAnalyze.cshtml.cs`
- `src/WarehouseDashboard.Web/Services/CardInsightService.cs`
- `src/WarehouseDashboard.Web/Services/CardSummaryBuilderFactory.cs`
- `src/WarehouseDashboard.Web/Infrastructure/ICardSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/KpiSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/ChartSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/TableSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Services/GenericSummaryBuilder.cs`
- `src/WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs`
- `src/WarehouseDashboard.Web/Infrastructure/CardSummary.cs`
- `src/WarehouseDashboard.Web/Infrastructure/IAIProvider.cs`
- `src/WarehouseDashboard.Web/Infrastructure/AIAssistantRequest.cs`
- `src/WarehouseDashboard.Web/Infrastructure/AnalyzeRequest.cs`
- `src/WarehouseDashboard.Web/Services/AssistantCacheService.cs`
- `src/WarehouseDashboard.Web/Services/AssistantLogService.cs`
- `src/WarehouseDashboard.Web/Infrastructure/ConnectionStringHelper.cs`
- All entity models under `src/WarehouseDashboard.Web/Models/`

**Project state files consulted:**
- `tera-system/engineering-governance/QUALITY_GATE_THRESHOLDS.md`
- `tera-system/engineering-governance/ENGINEERING_REVIEW_CHECKLIST.md`

---

## Overall Quality Gate: **BLOCKED**

| Severity | Count |
|---|---|
| **STOP** | 3 |
| **CAUTION** | 2 |
| **FLAG** | 5 |
| **BASELINE_DEBT** | 2 |

> **This audit identifies 3 STOP-level findings that must be resolved before production deployment.**
> The most critical issue is the **data bridge gap** (Phase C -> Phase D) which renders the AI assistant functionally incomplete.
## Finding Register

### STOP-01: API Key in Plaintext in appsettings.json

| Field | Value |
|---|---|
| **Finding ID** | STOP-01 |
| **Rule ID** | T-QG-HARD-SEC-001 |
| **Domain** | Security Hygiene |
| **Severity** | **STOP** |
| **Location** | `src/WarehouseDashboard.Web/appsettings.json` line 24 |
| **Evidence** | Direct file read: `"ApiKey": "sk-KO5RFeV35PHGEsx5UkzTeZamx84M4V3X4DqQ1OeT4i3ulhwlROD5vBmxOe5XzyXi"` |
| **Expected Standard** | Secrets must NEVER be stored in source-controlled configuration files. Must use secure key management. |
| **Observed Condition** | A valid API key for the OpenCodeGo AI provider is hardcoded in plaintext in appsettings.json, committed to source control. |
| **Impact** | Any developer with repository access can use this key. If repository is public or compromised, key is leaked. |
| **Recommended Action** | 1. Revoke current key. 2. Move to environment variable or Azure Key Vault. 3. Use placeholder in appsettings. |
| **Confidence** | High |
| **Blocking** | Yes |
| **Blocking Reason** | Real secrets must not exist in source control per hard rule T-QG-HARD-SEC-001 |
| **Waiver Allowed** | No |
| **Required Owner** | Tera / Majed |
| **Status** | Open |

### STOP-02: SQL Server and Oracle Passwords in Plaintext in appsettings.json

| Field | Value |
|---|---|
| **Finding ID** | STOP-02 |
| **Rule ID** | T-QG-HARD-SEC-001 |
| **Domain** | Security Hygiene |
| **Severity** | **STOP** |
| **Location** | appsettings.json lines 3-5 |
| **Evidence** | Connection strings contain `Password=013590` and `Password=COGNOS` in plaintext. |
| **Expected Standard** | Database passwords must never be in source-controlled config. Use environment variables or Key Vault. |
| **Observed Condition** | Both SQL Server and Oracle connection strings have hardcoded passwords. ConnectionStringHelper exists with placeholder support but current config bypasses it. |
| **Impact** | Complete database credential exposure. Repository access grants DB connection to production servers. |
| **Recommended Action** | 1. Change passwords on both databases. 2. Use placeholder format and env vars. |
| **Confidence** | High |
| **Blocking** | Yes |
| **Waiver Allowed** | No |
| **Required Owner** | Tera / Majed |
| **Status** | Open |

### STOP-03: Phase C Data Summary Built But NEVER Sent to AI (Critical Data Gap)

| Field | Value |
|---|---|
| **Finding ID** | STOP-03 |
| **Rule ID** | T-QG-HARD-FUNC-001 |
| **Domain** | Data Flow / Functional Completeness |
| **Severity** | **STOP** |
| **Location** | CardInsightAnalyze.cshtml.cs lines 83-100, CardInsightService.cs lines 100-102, 193 |
| **Evidence** | 1. Builder runs and produces CardSummary with rich data, but only IsFullDataReached/HasDeeperData/depthLabel extracted. 2. CardInsightService.cs line 101: "Phase C tasks will add actual data summaries" but Phase C is already implemented. 3. BuildUserMessage sends only title/chartType/description/mode/depthLabel. 4. CardSummary never passed to AnalyzeCardWithCacheAsync. |
| **Expected Standard** | If data summaries are built (Phase C), they must be included in the AI user message. |
| **Observed Condition** | Entire Phase C data pipeline operates in isolation. Rich data is computed but discarded when calling AI. AI receives zero actual data points. |
| **Impact** | **Critical.** AI assistant cannot meaningfully analyze any card. Prompt says "rely only on data provided" but almost none is provided. Phase C investment is wasted. |
| **Recommended Action** | Refactor BuildUserMessage to accept and serialize CardSummary into user message. Include all computed fields. |
| **Confidence** | High |
| **Blocking** | Yes |
| **Waiver Allowed** | No (unless Majed confirms metadata-only output is sufficient) |
| **Required Owner** | Tera |
| **Status** | Open |
### CAUTION-01: PromptVersion Hardcoded and Never Actually Used

| Field | Value |
|---|---|
| **Finding ID** | CAUTION-01 |
| **Domain** | Configuration / Maintainability |
| **Severity** | **CAUTION** |
| **Location** | appsettings.json line 27, AIAssistantOptions.cs line 13, CardInsightService.cs line 247 |
| **Evidence** | 1. PromptVersion = "1.0" defined in both config and model. 2. LogAndUpdateStats hardcodes "1.0" string literal. 3. No code reads AIAssistantOptions.PromptVersion. 4. Cache key does not include PromptVersion. |
| **Expected Standard** | Config values consumed programmatically. PromptVersion in cache key for prompt versioning. |
| **Observed Condition** | PromptVersion defined but never read; hardcoded in logging. Cache key unchanged when prompt changes. |
| **Impact** | No mechanism to track which prompt version was used. Cache serves old-prompt results after prompt updates. |
| **Recommended Action** | Inject IOptions into CardInsightService, use _options.Value.PromptVersion. Add to cache key. |
| **Confidence** | High |
| **Blocking** | No |
| **Required Owner** | Tera |
| **Status** | Open |

### CAUTION-02: KPI/Chart Builders Lack Column Identifier Validation

| Field | Value |
|---|---|
| **Finding ID** | CAUTION-02 |
| **Domain** | Security Hygiene / SQL Injection |
| **Severity** | **CAUTION** |
| **Location** | KpiSummaryBuilder.cs, ChartSummaryBuilder.cs |
| **Evidence** | 1. KpiSummaryBuilder interpolates dateCol/valueCol/catCol directly into SQL at lines 178, 214-223, 258-263. 2. ChartSummaryBuilder does same at lines 179-186, 292-299. 3. TableSummaryBuilder has ValidateIdentifier() but is not used by others. |
| **Expected Standard** | Column identifiers interpolated into raw SQL must be validated. |
| **Observed Condition** | Column names interpolated without validation. TableSummaryBuilder already implements ValidateIdentifier pattern. |
| **Impact** | Low in current trusted-admin config; Medium if config can be modified by non-admins. Inconsistency with TableSummaryBuilder. |
| **Recommended Action** | Extract ValidateIdentifier into shared utility class; apply in all builders. |
| **Confidence** | High |
| **Blocking** | No |
| **Required Owner** | Tera |
| **Status** | Open |
### FLAG-01: MaxOutputTokens Inconsistency (300 vs 2000)

| Field | Value |
|---|---|
| **Finding ID** | FLAG-01 |
| **Domain** | Configuration |
| **Severity** | **FLAG** |
| **Location** | appsettings.json line 26 (2000), AIAssistantOptions.cs line 12 (300), AIAssistantRequest.cs line 8 (300) |
| **Evidence** | appsettings has 2000. AIAssistantOptions default is 300. AIAssistantRequest default is 300. Adapter uses request > 0 ? request : options. CardInsightService never sets request.MaxOutputTokens = 300 is used, ignoring appsettings 2000. |
| **Expected Standard** | Config values should be coherent and effective value should match configured intent. |
| **Observed Condition** | Three conflicting defaults. Actual effective value is 300 because CardInsightService never sets it. |
| **Impact** | Output limited to 300 tokens (~200 Arabic words), insufficient for deep analysis with data summaries. |
| **Recommended Action** | Inject IOptions into CardInsightService and set request.MaxOutputTokens from config. |
| **Confidence** | High |
| **Blocking** | No |
| **Required Owner** | Tera |
| **Status** | Open |

### FLAG-02: Cache Invalidation Not Implemented

| Field | Value |
|---|---|
| **Finding ID** | FLAG-02 |
| **Domain** | Caching |
| **Severity** | **FLAG** |
| **Location** | AssistantCacheService.cs lines 39-44 |
| **Evidence** | InvalidateCard(int cardId) has empty body. Comment explains limitation. |
| **Expected Standard** | When card data/config changes, cached AI analyses should be invalidated. |
| **Observed Condition** | No invalidation mechanism. Stale analysis served for up to 10 minutes after changes. |
| **Impact** | Stale analysis for up to 10 minutes after card config changes. |
| **Recommended Action** | Implement invalidation using secondary lookup structure mapping cardId to cache keys. |
| **Confidence** | Medium |
| **Blocking** | No |
| **Required Owner** | Tera |
| **Status** | Open |

### FLAG-03: MaxOutputTokens Missing from AIAssistantRequest When Calling AI

| Field | Value |
|---|---|
| **Finding ID** | FLAG-03 |
| **Domain** | Completeness |
| **Severity** | **FLAG** |
| **Location** | CardInsightService.cs lines 104-109 and 195-202 |
| **Evidence** | In both AnalyzeCardAsync and AnalyzeCardWithCacheAsync, AIAssistantRequest is created without setting MaxOutputTokens. |
| **Expected Standard** | Configured MaxOutputTokens should be passed to AI request. |
| **Observed Condition** | request.MaxOutputTokens left at C# default (300). |
| **Recommended Action** | Inject IOptions into CardInsightService and set value from config. |
| **Confidence** | High |
| **Blocking** | No |
| **Required Owner** | Tera |
| **Status** | Open |

### FLAG-04: Default 300 Tokens Insufficient

| Field | Value |
|---|---|
| **Finding ID** | FLAG-04 |
| **Domain** | Prompt Design / Output Quality |
| **Severity** | **FLAG** |
| **Location** | AIAssistantRequest.cs line 8 |
| **Evidence** | 300 tokens = ~200-250 Arabic words. Prompt requests 5-7 lines of structured analysis with data. |
| **Expected Standard** | Token limit calibrated to expected output length. |
| **Observed Condition** | When data summaries are added, 300 tokens will be severely limiting. |
| **Recommended Action** | Increase default to 1500 tokens; use 2000 in config for deep mode. |
| **Confidence** | Medium |
| **Blocking** | No |
| **Required Owner** | Tera |
| **Status** | Open |

### FLAG-05: 30-Second Timeout May Be Insufficient

| Field | Value |
|---|---|
| **Finding ID** | FLAG-05 |
| **Domain** | Performance / Reliability |
| **Severity** | **FLAG** |
| **Location** | appsettings.json line 25: TimeoutSeconds: 30 |
| **Evidence** | 30 seconds for entire request (network + AI processing). May be tight for deep+data summaries. |
| **Expected Standard** | Timeout should account for worst-case AI processing latency at max tokens. |
| **Observed Condition** | 30s timeout includes HTTP overhead and model inference. |
| **Recommended Action** | Increase to 60s. Consider tiered timeout (30s explain, 60s deep). |
| **Confidence** | Medium |
| **Blocking** | No |
| **Required Owner** | Tera |
| **Status** | Open |

### BASELINE_DEBT-01: AdminAuth Bypass Enabled

| Field | Value |
|---|---|
| **Finding ID** | BASELINE_DEBT-01 |
| **Severity** | **BASELINE_DEBT** |
| **Location** | appsettings.json line 18: AdminAuth.Bypass = true |
| **Evidence** | Direct config read. |
| **Observed Condition** | No admin authentication enforced regardless of environment. |
| **Recommended Action** | Enable auth in non-development environments. |

### BASELINE_DEBT-02: No Retry/Resilience Pattern

| Field | Value |
|---|---|
| **Finding ID** | BASELINE_DEBT-02 |
| **Severity** | **BASELINE_DEBT** |
| **Location** | OpenCodeGoAdapter.cs |
| **Evidence** | Single-shot HTTP call. No Polly or retry logic. |
| **Observed Condition** | Transient AI provider errors cause visible failures. |
| **Recommended Action** | Add retry with exponential backoff for HTTP 429/503 and timeouts. |
## Detailed Analysis Sections

### Section 1: Configuration Audit

#### 1.1 AI Assistant Settings (appsettings.json)

```json
"AIAssistant": {
    "ProviderName": "OpenCodeGo",
    "BaseUrl": "https://opencode.ai/zen/go/v1/chat/completions",
    "ModelId": "deepseek-v4-flash",
    "ApiKey": "[REDACTED]",
    "TimeoutSeconds": 30,
    "MaxOutputTokens": 2000,
    "PromptVersion": "1.0"
}
```

**Evaluation:**

| Setting | Assessment |
|---|---|
| ProviderName | ✅ Appropriate |
| BaseUrl | ⚠️ Valid format. Verify endpoint for deepseek-v4-flash |
| ModelId | ✅ Capable model for analytical tasks |
| ApiKey | ❌ **STOP-01**: Hardcoded plaintext |
| TimeoutSeconds: 30 | ⚠️ **FLAG-05**: Adequate now; tight with data summaries |
| MaxOutputTokens: 2000 | ⚠️ **FLAG-01**: Config says 2000 but effective = 300 |
| PromptVersion: "1.0" | ❌ **CAUTION-01**: Never read from config |

#### 1.2 AIAssistantOptions Model Default Mismatch

Model default `MaxOutputTokens = 300` conflicts with `appsettings.json` `2000`. Config binding overwrites the model default with 2000, but `CardInsightService` creates `AIAssistantRequest` with its own default (300) — so config value is never consumed by the request path.

#### 1.3 OpenCodeGoAdapter Quality

**Strengths:** Comprehensive error handling for all failure modes (HTTP errors, timeouts, cancellation, empty responses, refusals, JSON parse errors). Token usage tracking. Response time tracking.

**Weakness:** No retry logic (**BASELINE_DEBT-02**).

#### 1.4 DI Wiring (Program.cs lines 57-77)

All services correctly registered:
- `IOptions<AIAssistantOptions>` — proper config binding
- `AddHttpClient<IAIProvider, OpenCodeGoAdapter>` — correct HttpClient management
- All services scoped appropriately (Scoped for per-request, Singleton for cache)

---

### Section 2: Data Flow Audit — The Critical Gap

#### 2.1 Current Flow

```
Front-end AJAX POST
    |
    v
CardInsightAnalyze.OnPostAsync()
    |
    +--> Load DashboardCard from EF Core
    |
    +--> builder.BuildSummaryAsync(card, depthLevel)
    |       |
    |       +--> CardSummary (RICH DATA):
    |       |    - CurrentValue, PreviousValue, ChangePercent
    |       |    - TrendDirection (up/down/stable)
    |       |    - SeriesData (time series points)
    |       |    - TopItems, BottomItems (categories + values + %)
    |       |    - SampleRows (raw data samples)
    |       |    - ColumnSummaries (SUM/AVG/MIN/MAX)
    |       |    - DataQualityNotes (nulls, outliers, imbalances)
    |       |    - TotalRowCount
    |       |
    |       +--> ONLY extracts: IsFullDataReached, HasDeeperData, depthLabel
    |       +--> **THE REST IS DISCARDED** 
    |
    +--> CardInsightService.AnalyzeCardWithCacheAsync(cardId, mode, ...)
            |
            +--> BuildUserMessage(card, mode, depthLevel)
            |       ONLY SENDS: title, chartType, description, modeLabel, depthLabel
            |       **NO DATA AT ALL**
            |
            +--> OpenCodeGoAdapter.SendAsync()
                    |
                    +--> AI Provider receives metadata only
                         +--> Generates generic analysis with zero data basis
```

#### 2.2 What the AI Actually Receives

**System Prompt:** Full GeneralPrompt (~50 lines of Arabic instructions) — well-written
**Card-Specific Prompt:** Optional card.AssistantPrompt
**User Message:** Only:
```
تحليل بطاقة:

العنوان: {card.Title}
النوع: {card.ChartType}
الوصف: {card.Description ?? "لا يوجد وصف"}
نوع التحليل: {modeLabel}
النطاق الزمني: {depthLabel}
```

**The AI has NO access to:**
- Current/previous value or change percentage
- Trend direction
- Time series data
- Top/bottom categories
- Sample rows
- Numeric summaries
- Data quality notes
- Total row count
- **Any actual business data**

#### 2.3 The BuildUserMessage Bottleneck

`CardInsightService.cs:114-143` is the bottleneck. The 5-field message has no data. `CardSummary` exists in the caller but is never passed.

**The needed bridge:**
Build a `BuildDataAugmentedUserMessage(CardSummary)` method that serializes the rich CardSummary into structured text.

### Section 3: Data Access Audit

#### 3.1 Available SQL Server Tables

| Table | Relevance to AI |
|---|---|
| Dashboards | Low (org metadata) |
| DashboardCards | **HIGH** (currently used) |
| CardDrillDownLevels | Medium (deep analysis context) |
| SyncSettings | Low |
| AdminPassword | None (security) |
| TableMappings | Medium (data provenance) |
| ColumnMappings | Low |
| SyncRuns | **Medium** (data freshness context) |
| SyncRunDetails | **Medium** (data quality signals) |
| Reports | Medium (cross-reference) |
| ReportColumns | Low |
| ReportFilters | Low |
| ReportLayouts | Low |
| AssistantInsightLogs | Medium (usage analytics) |
| AssistantUsageStats | Medium (popularity analysis) |

#### 3.2 Current Data Access Pattern

- **ReadOnlyQueryHelper**: SQL Server only (SqlServerReadOnly conn string with ApplicationIntent=ReadOnly). Parameterized queries only.
- **Oracle data**: Accessed indirectly via TableMappings -> synced SQL tables. The card's SqlQuery references these synced tables.
- **EF Core**: Used only for card metadata (DashboardCard, logs, stats).

#### 3.3 Untapped Data Sources for Deeper Analysis

1. **SyncRuns / SyncRunDetails**: AI could mention data freshness: "آخر مزامنة: 3 ساعات مضت" or warn if overdue.
2. **Reports**: Cross-reference between dashboard cards and formal report definitions.
3. **AssistantUsageStats**: Most-used cards, peak usage times — for usage pattern insights.
4. **CardDrillDownLevels**: Available drill paths for deeper exploration suggestions.

---

### Section 4: Prompt Analysis

#### 4.1 GeneralPrompt Assessment

**Strengths:**
- ✅ Clear professional Arabic
- ✅ Well-structured numbered rules (12 rules)
- ✅ Clear role boundaries
- ✅ Prohibits data invention
- ✅ Specifies output structure (5 sections)
- ✅ Asks for practical business insights
- ✅ Appropriate hedging language
- ✅ Emojis for visual structure

**Weaknesses:**
- ❌ **Rule 2 contradiction**: Says "rely only on data provided" but user message provides no data
- ❌ No data format specification for when structured data arrives
- ❌ No confidence scoring for statements
- ⚠️ Current length ~50 lines is good but will grow with data summaries

#### 4.2 User Message Assessment

**Current (inadequate — 5 fields, no data):**
```
تحليل بطاقة:
العنوان: ...
النوع: ...
الوصف: ...
نوع التحليل: ...
النطاق الزمني: ...
```

**Proposed enhanced message (after STOP-03 fix):**

For KPI cards:
```
تحليل بطاقة:
العنوان: ...
النوع: KPI
الوصف: ...
نوع التحليل: {mode}
النطاق الزمني: {depthLabel}

بيانات البطاقة:
- القيمة الحالية: {CurrentValue:N0}
- القيمة السابقة: {PreviousValue:N0}
- نسبة التغير: {ChangePercent:F1}%
- الاتجاه: {TrendDirection}

أفضل الفئات:
  • {TopItem.Name}: {TopItem.Value:N0} ({TopItem.Percent:F1}%)
  ...

ملاحظات جودة البيانات:
  ⚠️ {DataQualityNote}
```

For Table cards:
```
بيانات البطاقة:
- إجمالي الصفوف: {TotalRowCount}

ملخص الأعمدة:
  • {col}: SUM={Sum:N0}, AVG={Average:N1}, MIN={Min:N0}, MAX={Max:N0}

عينة بيانات (أول 3 صفوف):
  • {SampleRow}

ملاحظات جودة البيانات:
  ⚠️ {DataQualityNote}
```

#### 4.3 Data Quality Notes Already Built But Not Used

The builders already produce valuable quality notes:
- "توزيع غير متوازن: الفاعة الأعلى X تمثل Y% من الإجمالي"
- "انحراف ملحوظ: الفئة X تتجاوز ضعف المتوسط"
- "العمود Y يحتوي على قيم فارغة (NULL)"
- "تفاوت كبير في القيم للعمود Z"
- "لا توجد بيانات سلاسل زمنية ضمن النطاق المحدد"

These are computed but **never sent to the AI**.

### Section 5: Behavior Analysis

#### 5.1 Caching Analysis

| Aspect | Status |
|---|---|
| Cache key format | ✅ ai_assist_{cardId}_{mode}_{depthLevel} |
| Cache duration | ✅ 10 minutes absolute expiration |
| Cache success only | ✅ Only successful responses cached |
| Cache invalidation | ❌ **FLAG-02**: Empty method |
| PromptVersion in key | ❌ **CAUTION-01**: Not included |
| Size limit per entry | ⚠️ SetSize(1) but no global limit |
| Thread safety | ✅ IMemoryCache is thread-safe |

#### 5.2 Logging Analysis

| Aspect | Status |
|---|---|
| Request logging | ✅ Every request logged to AssistantInsightLogs |
| Usage stats | ✅ Per-card rolling stats in AssistantUsageStats |
| Error logging | ✅ Errors with errorCode field |
| Response time | ✅ Log and stats |
| Cache hit/miss | ✅ Tracked in stats |
| CardPromptUsed flag | ✅ Tracked |
| PromptVersion in logs | ⚠️ Hardcoded "1.0" — should read from config |

#### 5.3 Error Handling Analysis

| Scenario | Behavior | Assessment |
|---|---|---|
| Builder SQL error | Caught, logged, continues with basic card info | ✅ Graceful |
| AI HTTP error | Logged, user-friendly error returned | ✅ |
| AI timeout (30s) | Logged, timeout error returned | ✅ |
| Empty AI response | Logged with finish_reason | ✅ |
| AI refusal | Logged, refusal error | ✅ |
| JSON parse error | Logged with truncated body | ✅ |
| Cancellation | Clean cancellation error | ✅ |
| Network error | Logged, network error message | ✅ |
| Retry on transient failure | Not implemented | ❌ **BASELINE_DEBT-02** |

#### 5.4 What Happens When...

**API key is incorrect?**
-> HTTP 401 from provider -> user sees "The AI service returned an error (HTTP 401)"

**SQL query in card config is invalid?**
-> Builder throws SQL exception -> caught in endpoint -> logged -> continues with basic metadata only

**Builder succeeds but AI call fails?**
-> Builder data discarded -> AI error returned -> builder work wasted

---

### Section 6: Future Readiness — SQL Generation

#### 6.1 Current Architecture Foundation

| Component | Status for SQL Gen |
|---|---|
| ReadOnlyQueryHelper | ✅ Read-only parameterized queries |
| OpenCodeGoAdapter | ⚠️ Would need separate write adapter |
| Builder pattern | ✅ Extensible strategy pattern |
| Audit logging | ✅ AssistantInsightLogs foundation |
| Authentication | ❌ AdminAuth.Bypass=true |

#### 6.2 Prerequisites for SQL Generation

**Before enabling SQL generation, these MUST be in place:**

1. **Separate DB user** with SELECT-only on specific tables/views
2. **SQL validation middleware** to enforce SELECT-only, block DDL/DML
3. **Whitelist** of allowed tables/views per user role
4. **Query guards**: MAXDOP 1, MAXRECURSION 0, max rows limit
5. **Rate limiting**: max queries per minute, max execution time
6. **Human approval** for any write operations
7. **Separate AI provider config** (smaller model, different API key)
8. **PII sanitization** on query results before returning

#### 6.3 Recommended Architecture Evolution

```
Current:
User -> CardInsightAnalyze -> Builders -> (data discarded) -> AI -> Analysis

Phase D (Data Bridge — immediate):
User -> CardInsightAnalyze -> Builders -> CardSummary -> AI -> Data-Driven Analysis

SQL Generation (future):
User -> NaturalLanguageQuery -> Classifier -> {
    "analysis": -> CardInsightService (existing flow)
    "data_question": -> SQLGenAgent -> SQLValidator -> ReadOnlyQueryHelper -> Results -> AI -> Answer
}
```

---

### Section 7: Priority Scoring

| Priority | Finding | Effort | Risk |
|---|---|---|---|
| **P0** | STOP-03: Bridge Phase C data to AI | 4-6h | AI assistant broken |
| **P0** | STOP-01: Move API key to env var | 1h | Key leakage |
| **P0** | STOP-02: Move DB passwords to env var | 1h | DB credential exposure |
| **P1** | FLAG-01/03: Fix MaxOutputTokens | 30m | Output truncated at 300 |
| **P1** | CAUTION-01: Use PromptVersion from config | 1h | Stale cache after prompts |
| **P1** | FLAG-05: Increase timeout to 60s | 15m | Timeout failures |
| **P2** | CAUTION-02: Column validation in builders | 2h | SQL injection surface |
| **P2** | FLAG-02: Cache invalidation | 2h | Stale analysis 10min |
| **P2** | FLAG-04: Increase default to 1500 tokens | 15m | Output too short |
| **P2** | BASELINE_DEBT-02: Add retry with Polly | 2h | Transient failures visible |
| **P2** | BASELINE_DEBT-01: AdminAuth | 3h | No admin security |

---

## Handback to Orchestrator

| Field | Value |
|---|---|
| **Status** | **BLOCKED** (3 STOP findings) |
| **Report Path** | project-control/audit-reports/QUAUD-AI-DEEP-TECHNICAL-2026-07-21.md |
| **Blocking Findings** | STOP-01 (API key), STOP-02 (DB passwords), STOP-03 (Data bridge) |
| **Recommended Next Action** | Resolve all STOP findings. Start with STOP-03 (data bridge) — the core functional gap. Create BuildDataAugmentedUserMessage() that accepts CardSummary and serializes it into the user message. |

**Key insight:** The architecture is well-designed with clear separation of concerns. The single critical gap is the missing data bridge between Phase C (data builders) and Phase D (AI call). Once bridged, the AI assistant transforms from a generic text generator to a genuinely data-driven analytical tool. The builder pattern, caching, logging, and error handling are all solid foundations.

**The code even acknowledges this gap in a comment at CardInsightService.cs:101:**
```csharp
// For now (Phase B), just pass basic card info.
// Phase C tasks will add actual data summaries.
```

**Phase C builders are already implemented but the bridge was never completed.** This is the single most impactful fix in the entire system.
