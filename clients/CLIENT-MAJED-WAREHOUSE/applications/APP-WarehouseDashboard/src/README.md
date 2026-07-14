# WarehouseDashboard — Solution Scaffold

.NET 8 solution for the Warehouse Dashboard application (client: الماجد لادارة المستودعات).

## Projects

| Project | Type | Purpose |
|---------|------|---------|
| `WarehouseDashboard.Web` | ASP.NET Core Razor Pages (.NET 8) | Dashboard UI + Admin Panel |
| `WarehouseDashboard.Api` | ASP.NET Core Web API (.NET 8) | Sync Engine (background service) + REST endpoints |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Runtime + ASP.NET Core Hosting Bundle for IIS)
- SQL Server 2019+ (or Express) — database `WarehouseDashboard`
- Oracle client access (ODP.NET Managed Driver is bundled — no Oracle client install needed)
- A valid **Syncfusion** license key (Community or paid)

## Environment Variables (REQUIRED — no secrets in files)

Passwords and the Syncfusion license key are supplied at runtime via environment
variables. **None of these values are stored in `appsettings.json`** — only `{SQL_PASSWORD}`
and `{ORACLE_PASSWORD}` placeholders are.

| Variable | Description |
|----------|-------------|
| `SQL_PASSWORD` | Password for the SQL Server `sa` account used by both projects. |
| `ORACLE_PASSWORD` | Password for the Oracle `NATEJSOFT` account used by the Api. |
| `SYNCFUSION_LICENSE_KEY` | Syncfusion UnlockKey (registered in `Program.cs` via `SyncfusionLicenseProvider.RegisterLicense`). |

### Setting the variables (PowerShell)

```powershell
$env:SQL_PASSWORD      = "your-sql-sa-password"
$env:ORACLE_PASSWORD   = "your-oracle-password"
$env:SYNCFUSION_LICENSE_KEY = "your-syncfusion-key"
```

### Setting the variables (CMD)

```cmd
set SQL_PASSWORD=your-sql-sa-password
set ORACLE_PASSWORD=your-oracle-password
set SYNCFUSION_LICENSE_KEY=your-syncfusion-key
```

For production/IIS, set these as **system environment variables** (or use IIS
configuration encryption) so they persist and are not visible in files.

## Building

```powershell
cd src
dotnet build WarehouseDashboard.sln
```

## Running (development)

Open two terminals (after setting the env vars above):

```powershell
# Terminal 1 — API / Sync Engine (http://localhost:5001)
cd src/WarehouseDashboard.Api
dotnet run

# Terminal 2 — Web / Dashboard (http://localhost:5000)
cd src/WarehouseDashboard.Web
dotnet run
```

- API health check: `GET http://localhost:5001/api/health` → `{ "status": "healthy", ... }`
- Web home page: `http://localhost:5000/`

## Notes

- This is the **Phase A — Foundation** scaffold. No business logic, dashboard cards,
  sync code, or database migrations are included yet (those belong to later tasks).
- Connection strings live in each project's `appsettings.json` as templates; the
  password placeholders are resolved by `Infrastructure/ConnectionStringHelper.cs`.
- The Sync Engine background service (`Services/SyncEngineService.cs`) is registered
  but currently performs no synchronization.

## IIS Deployment (outline)

```powershell
dotnet publish src/WarehouseDashboard.Api  -c Release -o C:\WarehouseDashboard\api
dotnet publish src/WarehouseDashboard.Web  -c Release -o C:\WarehouseDashboard\web
```

Create two IIS applications (`/api` → Api pool, `/` → Web pool), set the environment
variables on the server, and install the .NET 8 Hosting Bundle. See the technical
architecture document for full details.
