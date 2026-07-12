# Technology Profile: dotnet-razorpages-adonet

## 1. Profile Identity

- Profile ID: `dotnet-razorpages-adonet`
- Language: C#
- Framework: ASP.NET Core Razor Pages (.NET 8 LTS)
- Database: SQL Server (destination) + Oracle (source, read-only via ODP.NET)
- ORM: ADO.NET SqlBulkCopy (data loading), Entity Framework Core (config + logs only)
- Package Manager / CLI: dotnet CLI, NuGet
- UI Library: Syncfusion (Blazor/Razor components via TagHelpers or JS interop)
- Default Project Type: Internal business application with data integration

## 2. Applicability

Use this profile when:
- The approved architecture uses ASP.NET Core Razor Pages (not Blazor Server/WebAssembly)
- The application needs to connect to Oracle as a data source (read-only)
- Data is loaded into SQL Server via ADO.NET SqlBulkCopy for performance
- EF Core is used only for application-level configuration and logging
- The application is deployed on IIS (local or on-premise)

This profile covers:
- ASP.NET Core Razor Pages (server-rendered, not SPA)
- Oracle.ManagedDataAccess (ODP.NET) for Oracle connectivity
- ADO.NET SqlBulkCopy for high-performance data transfer
- EF Core for config/logs tables only
- Syncfusion components for dashboard/report UI
- IIS hosting model

## 3. Default Execution Order

1. Scaffold the .NET solution structure (API project + Razor Pages project).
2. Add only the required base packages (ODP.NET, EF Core, Syncfusion license key).
3. Create config/logs database schema (EF Core entities + migrations).
4. Build Oracle connection and data extraction layer (ODP.NET).
5. Build SQL Server data loading layer (SqlBulkCopy).
6. Build Sync Engine (full refresh + incremental-ready architecture).
7. Build Sync Logs and monitoring.
8. Build Dashboard layout and rendering (Razor Pages + Syncfusion).
9. Build Drill Down and navigation.
10. Build Admin Panel (Card CRUD + config).
11. Integrate Sync Status display in Dashboard.
12. IIS deployment configuration.

## 4. First Task Rule

Safe first task:

```text
Scaffold .NET 8 solution structure with two projects:
  WarehouseDashboard.Api     (Console / Worker Service for sync engine)
  WarehouseDashboard.Web      (ASP.NET Core Razor Pages)
+ Add base packages: Oracle.ManagedDataAccess.Core, Microsoft.Data.SqlClient,
  Microsoft.EntityFrameworkCore.SqlServer, Syncfusion.Blazor (for license)
+ Verify solution builds cleanly
```

**Do NOT** in the first task:
- Add EF Core entities
- Add migrations or database update
- Add Oracle connection logic
- Add any business logic
- Add any UI rendering
- Add authentication/authorization

## 5. Scaffold Rules

- Use `dotnet new razor` for the Web project, `dotnet new console` or `dotnet new webapi` for the API project.
- Solution file should be at root with two project references.
- Keep the first task limited to project structure and minimum approved packages.
- Syncfusion license key is set via `Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense()` in `Program.cs`.
- IIS hosting: configure `appsettings.json` with `"Urls": "http://localhost:5000"` as default, configurable via IIS.

## 6. ORM / Database Rules

**Entity Framework Core (Config + Logs only):**
- Used exclusively for `WarehouseDashboardDbContext` with Config entities (CardConfig, DataSourceConfig, etc.) and Log entities (SyncLog, ErrorLog, etc.).
- `dotnet ef migrations add` belongs to a separate migration task.
- `dotnet ef database update` is forbidden unless the task is explicitly a database apply task.
- Do not use EF Core for Oracle data entities or bulk operations.

**ADO.NET / SqlBulkCopy (Data Loading):**
- Used for all Oracle-to-SQL Server data transfer.
- SqlBulkCopy belongs to a dedicated data-loading task.
- Oracle connection is via `Oracle.ManagedDataAccess.Client.OracleConnection`.
- Full Refresh = DELETE all rows + SqlBulkCopy.WriteToServer in single transaction.
- Incremental Sync ready architecture = track LastSyncTimestamp, support Upsert pattern.

**Oracle (Read-Only Source):**
- ODP.NET connections are read-only by convention.
- Oracle table details are deferred — client provides during execution.
- Oracle Managed Data Access (NuGet: Oracle.ManagedDataAccess.Core).

## 7. CLI Side Effects

- `dotnet new razor` creates Razor Pages boilerplate files.
- `dotnet new console` creates console app boilerplate.
- `dotnet add package` modifies `.csproj`.
- `dotnet ef migrations add` creates migration files.
- `dotnet ef database update` changes database state.
- `dotnet build` / `dotnet publish` produce output files.

## 8. Forbidden Defaults

Do not add by default in the first implementation task:

- EF Core entities or DbContext
- Migrations or database update
- Oracle connection logic
- SyncEngine logic
- Dashboard UI
- Admin Panel
- Authentication/Authorization
- Custom UI styling beyond Syncfusion defaults
- Repository layer without clear need
- SignalR or real-time features
- Background services (unless explicitly delegated)

## 9. Pre-Execution Gate Additions

- Confirm scaffold task does not silently add Identity, EF migrations, or extra hosting layers.
- Confirm ODP.NET is isolated from EF Core DbContext (separate concerns).
- Confirm SqlBulkCopy tasks are not combined with Oracle extraction tasks unless the task explicitly combines them with approval.
- Confirm database update commands are isolated in dedicated tasks.
- Confirm Syncfusion license key is registered before any component usage.
- Confirm IIS hosting model is configured correctly (no Kestrel-only assumptions).

## 10. Acceptance Criteria Patterns

- .NET 8 solution builds cleanly with both projects.
- Required packages are installed and version-compatible.
- Solution can be published for IIS deployment (`dotnet publish -c Release`).
- Oracle connection can be established (tested in dedicated task).
- SQL Server config/logs schema can be created (tested in dedicated task).
- No unapproved packages, entities, migrations, or auth were added.
- No unapproved database changes were applied.
