# TASK-AI-B03 — Read-Only DB Connection + Simple Query Helper

> **Status:** Draft → Approved  
> **Batch:** AI-B-1 (Backend Foundation — Parallel with B01, B02)  
> **Phase:** AI Assistant — Phase B (Backend Foundation)  
> **Plan Reference:** `project-preparation/AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` §7.1  

---

## 1. Objective

Add a dedicated read-only SQL Server connection string and a minimal query helper class that the AI assistant data layer will use. The helper must only support SELECT operations — no INSERT, UPDATE, DELETE, or schema changes.

---

## 2. Context

- **Technology Profile:** `dotnet-razorpages-adonet`
- **ClientAppPath:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`
- **Important:** The existing `SqlServer` connection string uses `sa` with full privileges. The assistant needs a separate read-only string. For now, add a second connection string named `SqlServerReadOnly` that points to the same server/database but conceptually represents read-only intent. The actual read-only DB user creation is a separate infrastructure task outside this code task.

---

## 3. Allowed Sources

- `ClientAppPath\src\WarehouseDashboard.Web\appsettings.json` — existing connection strings
- `ClientAppPath\project-preparation\AI_DASHBOARD_ASSISTANT_IMPLEMENTATION_PLAN.md` — §7 Security

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\appsettings.json` — add `SqlServerReadOnly` connection string
2. `ClientAppPath\src\WarehouseDashboard.Web\Infrastructure\ReadOnlyQueryHelper.cs` — **NEW** minimal helper class

---

## 5. Forbidden

- Must NOT modify the existing `SqlServer` connection string.
- Must NOT add INSERT, UPDATE, DELETE, DROP, ALTER, or EXEC capabilities to the helper.
- Must NOT create a second DbContext — the helper is ADO.NET based, lightweight.
- Must NOT add any business logic — only connection string + query execution helper.
- Must NOT modify Program.cs, models, migrations, or any other file.

---

## 6. Technical Notes

**File 1: `appsettings.json`** — add under `ConnectionStrings`:

```json
"SqlServerReadOnly": "Server=MAJED\\MJDSQLSERVER;Database=WarehouseDashboard;User Id=sa;Password=013590;TrustServerCertificate=True;ApplicationIntent=ReadOnly;"
```

Note: `ApplicationIntent=ReadOnly;` signals read-only intent. The actual read-only SQL login is a future infrastructure task.

**File 2: `Infrastructure/ReadOnlyQueryHelper.cs`** — A minimal helper that:

- Takes `IConfiguration` in constructor.
- Reads the `SqlServerReadOnly` connection string.
- Has a single method: `Task<List<Dictionary<string, object>>> QueryAsync(string sql, Dictionary<string, object>? parameters = null)`
- Uses `SqlConnection` + `SqlCommand` with plain ADO.NET (no EF Core, no tracking).
- Always uses parameterized queries (never string concatenation).
- Disposes connection and command after each call.
- Sets `CommandTimeout` to 30 seconds.

Structure:

```csharp
namespace WarehouseDashboard.Web.Infrastructure;

public class ReadOnlyQueryHelper
{
    private readonly string _connectionString;
    
    public ReadOnlyQueryHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServerReadOnly")
            ?? throw new InvalidOperationException("SqlServerReadOnly connection string not found.");
    }
    
    public async Task<List<Dictionary<string, object>>> QueryAsync(
        string sql, Dictionary<string, object>? parameters = null)
    {
        // open connection → create command → add parameters → execute reader → map to dictionaries → return
    }
}
```

No registration in Program.cs needed yet — it will be registered in TASK-AI-B05 when the service is created.

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | `appsettings.json` has `SqlServerReadOnly` connection string |
| AC-2 | `SqlServerReadOnly` includes `ApplicationIntent=ReadOnly;` |
| AC-3 | Existing `SqlServer` connection string unchanged |
| AC-4 | `ReadOnlyQueryHelper.cs` exists in `Infrastructure/` |
| AC-5 | `ReadOnlyQueryHelper` has `QueryAsync` method with parameterized query support |
| AC-6 | No INSERT/UPDATE/DELETE/DROP/ALTER anywhere in the helper |
| AC-7 | `dotnet build` passes with 0 errors and 0 warnings |
| AC-8 | No real secrets exposed in code (uses IConfiguration) |

---

## 8. Pre-Execution Gate Result

```
Gate: PASS
Orchestration Level: Task Delegation Only
Model Tier: Light (connection string + simple ADO.NET helper, ~40 lines)
Security Sensitivity: Low (read-only by intent, no write operations)
Design Governance: N/A (no UI)
Technology Profile: dotnet-razorpages-adonet ✅
```

---

## 9. Handback Requirements

1. List of files created/modified.
2. The added `SqlServerReadOnly` connection string.
3. Full contents of `ReadOnlyQueryHelper.cs`.
4. Build result (must be 0 errors, 0 warnings).
5. Confirmation that no write operations exist in the helper.
