# TASK-COD-FIX-001 — Critical & Important Bug Fixes

> **Status:** Approved
> **Type:** Fix / Remediation
> **Assigned Agent:** engineering-agent
> **Batch:** FIX (pre-B7)
> **Created:** 2026-07-13
> **Preceded By:** TASK-COD-001 through TASK-COD-014 (all Accepted)
> **Blocking:** TASK-COD-019 (IIS Setup), TASK-COD-020 (Syncfusion License), TASK-COD-021 (UAT)

---

## 1. Objective

Fix all critical and important gaps identified during the comprehensive self-audit of the WarehouseDashboard project. These fixes must be completed before B7 deployment tasks can proceed.

## 2. Build Status

**Baseline:** `dotnet build -c Release` → 0 Warnings / 0 Errors
**Expected after fixes:** Same — 0 Warnings / 0 Errors

---

## 3. Critical Fixes (C1–C4)

### C1: Create `web.config` for IIS Hosting

**Problem:** Both projects lack `web.config` — IIS cannot launch the .NET 8 process without it.

**Action:**
- Create `WarehouseDashboard.Web/web.config` with `<aspNetCore processPath="dotnet" arguments=".\WarehouseDashboard.Web.dll" stdoutLogEnabled="false" hostingModel="inprocess" />`
- Create `WarehouseDashboard.Api/web.config` with `<aspNetCore processPath="dotnet" arguments=".\WarehouseDashboard.Api.dll" stdoutLogEnabled="false" hostingModel="inprocess" />`
- Use standard ASP.NET Core IIS hosting configuration.

**Files to create:**
- `WarehouseDashboard.Web/web.config`
- `WarehouseDashboard.Api/web.config`

**Verification:** Build still passes. Files are well-formed XML.

---

### C2: Update `SyncSettings.LastSyncTimestamp` After Successful Sync

**Problem:** `SyncEngineService.RunSyncOnceAsync()` completes successfully but never writes back `LastSyncTimestamp` to the database. The dashboard's "last sync" indicator is always stale.

**Action:**
- At the end of the success path in `RunSyncOnceAsync()` (after line 188, before the closing of the try block), add an ADO.NET `UPDATE SyncSettings SET LastSyncTimestamp = @now WHERE Id = 1` call.
- Use `ConnectionStringHelper.ResolveSql(_configuration)` for the connection string.
- Wrap in try/catch — if the update fails, log a warning but do not fail the sync cycle (the data was already loaded successfully).

**File to modify:**
- `WarehouseDashboard.Api/Services/SyncEngineService.cs`

**Verification:** Build passes. Logic review: timestamp is updated only on success.

---

### C3: Add CORS Policy to Api Project

**Problem:** The Web project (port 5000) calls the Api project (port 5001) via fetch. These are different origins. No CORS policy is configured — browsers will block these requests in production.

**Action:**
- In `Api/Program.cs`, add:
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowWeb", policy =>
      {
          policy.WithOrigins("https://localhost:5000", "http://localhost:5000")
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
  });
  ```
- Before `app.MapControllers()`, add `app.UseCors("AllowWeb");`

**File to modify:**
- `WarehouseDashboard.Api/Program.cs`

**Verification:** Build passes. CORS middleware is registered before endpoints.

---

### C4: Make SyncStatusBar API Base URL Configurable

**Problem:** `SyncStatusBar/Default.cshtml` hardcodes `data-api-base="https://localhost:5001"`. This won't work behind IIS where both apps share a domain.

**Action:**
- In `WarehouseDashboard.Web/appsettings.json`, add a configuration key:
  ```json
  "SyncApi": {
    "BaseUrl": "https://localhost:5001"
  }
  ```
- In `_DashboardLayout.cshtml` or `Index.cshtml`, inject `IConfiguration` and render the URL into a `<script>` block or `data-api-base` attribute:
  ```html
  <div id="sync-status-bar" class="sync-bar" data-api-base="@Model.SyncApiBaseUrl" ...>
  ```
- The `IndexModel` (or a shared base model) should read `Configuration["SyncApi:BaseUrl"]` and pass it to the view.

**Files to modify:**
- `WarehouseDashboard.Web/appsettings.json` (add SyncApi:BaseUrl)
- `WarehouseDashboard.Web/Pages/Index.cshtml` (pass URL to component)
- `WarehouseDashboard.Web/Pages/Index.cshtml.cs` (read config, expose property)
- `WarehouseDashboard.Web/Pages/Shared/Components/SyncStatusBar/Default.cshtml` (keep data-api-base as fallback, but receive from model)

**Alternative simpler approach:** Use a relative path if both apps are behind the same IIS site with path routing. But since they're separate apps on different ports, the config-based approach is correct.

**Verification:** Build passes. URL is configurable, not hardcoded.

---

## 4. Important Fixes (I1, I2, I4, I5, I6)

### I1: Register `DashboardService` in DI Container

**Problem:** `DashboardService` is instantiated manually with `new DashboardService(_db, _config)` — bypasses DI, not testable.

**Action:**
- In `Web/Program.cs`, add: `builder.Services.AddScoped<DashboardService>();`
- In `Pages/Api/Dashboard/Card.cshtml.cs`, change from `new DashboardService(...)` to constructor injection of `DashboardService`.

**Files to modify:**
- `WarehouseDashboard.Web/Program.cs`
- `WarehouseDashboard.Web/Pages/Api/Dashboard/Card.cshtml.cs` (or wherever DashboardService is manually instantiated)

**Verification:** Build passes. DashboardService resolves from DI.

---

### I2: Populate `SyncEngineService._mappings` from Configuration

**Problem:** `_mappings` list is permanently empty — the sync engine processes zero tables. There is no mechanism to configure Oracle→SQL Server table mappings.

**Action:**
- Add a `TableMappings` section to `Api/appsettings.json`:
  ```json
  "TableMappings": [
    {
      "OracleSource": "NATEJSOFT.WAREHOUSE_STOCK",
      "SourceType": "Table",
      "SqlTargetTable": "stg_WarehouseStock"
    }
  ]
  ```
- In `SyncEngineService` constructor, read this configuration and populate `_mappings`:
  ```csharp
  var mappings = _configuration.GetSection("TableMappings").Get<List<TableMapping>>();
  if (mappings != null)
      _mappings.AddRange(mappings);
  ```
- Log the number of mappings loaded at startup.

**Files to modify:**
- `WarehouseDashboard.Api/Services/SyncEngineService.cs`
- `WarehouseDashboard.Api/appsettings.json` (add empty TableMappings section with example comment)

**Verification:** Build passes. On startup, engine logs "X mapping(s) loaded from configuration."

---

### I4: Add Logging to Empty Catch Blocks

**Problem:** `IndexModel.OnGetAsync` and `DrillModel.OnGetAsync` silently swallow exceptions.

**Action:**
- In `Pages/Index.cshtml.cs`, change:
  ```csharp
  catch (Exception) { ConfigError = true; }
  ```
  to:
  ```csharp
  catch (Exception ex) { _logger.LogWarning(ex, "Failed to load dashboard config."); ConfigError = true; }
  ```
- Same pattern in `Pages/Dashboard/Drill.cshtml.cs`.
- Ensure both page models have `ILogger<T>` injected via constructor.

**Files to modify:**
- `WarehouseDashboard.Web/Pages/Index.cshtml.cs`
- `WarehouseDashboard.Web/Pages/Dashboard/Drill.cshtml.cs`

**Verification:** Build passes. Exceptions are logged, not swallowed.

---

### I5: Extract Duplicated Code to Shared Utilities

**Problem:** `ConvertCell()` and `Sanitize()` are duplicated across `DashboardService.cs` and `Drill.cshtml.cs`. JS helper functions (`toNum`, `formatNum`, `escapeHtml`, etc.) are duplicated between `Index.cshtml` and `Drill.cshtml`.

**Action (C#):**
- Create `WarehouseDashboard.Web/Infrastructure/DataHelper.cs` with static methods:
  ```csharp
  public static class DataHelper
  {
      public static string ConvertCell(object value) { ... }
      public static string Sanitize(string input) { ... }
  }
  ```
- Update `DashboardService.cs` and `Drill.cshtml.cs` to use `DataHelper.ConvertCell()` and `DataHelper.Sanitize()`.

**Action (JavaScript):**
- Create `WarehouseDashboard.Web/wwwroot/js/dashboard-utils.js` with the shared functions.
- Reference it in `_DashboardLayout.cshtml` via `<script src="~/js/dashboard-utils.js"></script>`.
- Remove inline duplicates from `Index.cshtml` and `Drill.cshtml`.

**Files to create:**
- `WarehouseDashboard.Web/Infrastructure/DataHelper.cs`
- `WarehouseDashboard.Web/wwwroot/js/dashboard-utils.js`

**Files to modify:**
- `WarehouseDashboard.Web/Pages/Shared/DashboardService.cs`
- `WarehouseDashboard.Web/Pages/Dashboard/Drill.cshtml.cs`
- `WarehouseDashboard.Web/Pages/Index.cshtml`
- `WarehouseDashboard.Web/Pages/Dashboard/Drill.cshtml`
- `WarehouseDashboard.Web/Pages/Shared/_DashboardLayout.cshtml`

**Verification:** Build passes. No duplicated logic remains.

---

### I6: Use `ILogger` Instead of `Console.WriteLine`

**Problem:** `Login.cshtml.cs` uses `Console.WriteLine` for error logging.

**Action:**
- Inject `ILogger<LoginModel>` via constructor.
- Replace `Console.WriteLine(...)` with `_logger.LogWarning(...)` or `_logger.LogError(...)`.

**File to modify:**
- `WarehouseDashboard.Web/Pages/admin-secure-panel/Login.cshtml.cs`

**Verification:** Build passes. No `Console.WriteLine` in production code.

---

## 5. Acceptance Criteria

| # | Criterion | Must Pass |
|---|-----------|-----------|
| AC1 | `dotnet build -c Release` → 0 Errors, 0 Warnings | ✅ |
| AC2 | `web.config` exists in both Web and Api projects | ✅ |
| AC3 | `SyncSettings.LastSyncTimestamp` is updated after successful sync | ✅ |
| AC4 | CORS policy registered and applied in Api `Program.cs` | ✅ |
| AC5 | SyncStatusBar API base URL reads from configuration (not hardcoded) | ✅ |
| AC6 | `DashboardService` registered in DI and resolved via constructor injection | ✅ |
| AC7 | `SyncEngineService._mappings` populated from `appsettings.json` at startup | ✅ |
| AC8 | No empty `catch (Exception)` blocks without logging | ✅ |
| AC9 | No duplicated `ConvertCell`/`Sanitize` methods (extracted to `DataHelper`) | ✅ |
| AC10 | No `Console.WriteLine` in production code (replaced with `ILogger`) | ✅ |
| AC11 | No hardcoded `https://localhost:5001` in source files (configurable) | ✅ |

---

## 6. Non-Goals (Explicitly NOT in Scope)

- Creating `WarehouseDashboardLogContext` (deferred to Phase 2)
- Thread-safety fix for `SyncRunLogStore` (nice-to-have, Phase 2)
- Per-card auto-refresh using `RefreshInterval` (Phase 2)
- Responsive `@media` breakpoints (Phase 2)
- Deployment scripts (Phase 2)
- Admin page CSS consolidation into `blue-theme.css` (retrofit, Phase 2)

---

## 7. Allowed Write Targets

```
WarehouseDashboard.Api/Program.cs
WarehouseDashboard.Api/Services/SyncEngineService.cs
WarehouseDashboard.Api/appsettings.json
WarehouseDashboard.Web/Program.cs
WarehouseDashboard.Web/appsettings.json
WarehouseDashboard.Web/web.config
WarehouseDashboard.Api/web.config
WarehouseDashboard.Web/Infrastructure/DataHelper.cs (NEW)
WarehouseDashboard.Web/wwwroot/js/dashboard-utils.js (NEW)
WarehouseDashboard.Web/Pages/Index.cshtml
WarehouseDashboard.Web/Pages/Index.cshtml.cs
WarehouseDashboard.Web/Pages/Dashboard/Drill.cshtml
WarehouseDashboard.Web/Pages/Dashboard/Drill.cshtml.cs
WarehouseDashboard.Web/Pages/Shared/DashboardService.cs
WarehouseDashboard.Web/Pages/Shared/_DashboardLayout.cshtml
WarehouseDashboard.Web/Pages/admin-secure-panel/Login.cshtml.cs
WarehouseDashboard.Web/Pages/Api/Dashboard/Card.cshtml.cs (if exists)
```

Any file NOT listed above must NOT be modified.

---

## 8. Estimated Effort

**Total:** 3–4 hours
- C1 (web.config): 20 min
- C2 (LastSyncTimestamp): 20 min
- C3 (CORS): 20 min
- C4 (API Base URL): 45 min
- I1 (DI registration): 15 min
- I2 (Table Mappings): 45 min
- I4 (Logging): 15 min
- I5 (Code dedup): 45 min
- I6 (ILogger): 10 min

---

## 9. Delegation Instructions

**Agent:** engineering-agent
**Allowed Write Targets:** See Section 7 above ONLY
**Verification:** After each fix, run `dotnet build -c Release` to confirm 0 errors. After all fixes complete, run final full build as acceptance verification.
**Continuous Improvement:** If you notice a systemic gap not listed here, log it in `project-control/AGENT_GAPS_LOG.md` but do NOT expand scope.

---

> **Prepared by:** TeraAgent — 2026-07-13
> **Approved by:** Majed — 2026-07-13
