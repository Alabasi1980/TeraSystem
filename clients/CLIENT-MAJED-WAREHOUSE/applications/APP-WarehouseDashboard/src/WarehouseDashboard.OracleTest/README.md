# WarehouseDashboard.OracleTest

**Oracle Connectivity Test** — .NET 8 Console Application using ODP.NET Managed Driver.

## Purpose

This project verifies that a .NET application can connect to an Oracle Database via `Oracle.ManagedDataAccess.Core`. It performs a simple `SELECT SYSDATE FROM DUAL` query to confirm:

- Oracle server is reachable
- Authentication credentials are valid
- ODP.NET is correctly configured
- Basic SQL query execution works

## How to Run

Before running, set the `ORACLE_PASSWORD` environment variable to your Oracle password:

```powershell
# PowerShell
$env:ORACLE_PASSWORD = "COGNOS"

# CMD
set ORACLE_PASSWORD=COGNOS
```

Then run the application:

```bash
dotnet run
```

> **Note:** The connection string in `appsettings.json` uses the placeholder `{ORACLE_PASSWORD}`. The application reads the `ORACLE_PASSWORD` environment variable at startup and substitutes it in — the password is **never** hardcoded in source code.

---

## Prerequisites

| Requirement | Version / Notes |
|-------------|-----------------|
| .NET SDK | 8.0 or later (LTS) |
| Oracle Database | Any version 11g or later |
| Oracle Client | **Not required** — ODP.NET Managed Driver is self-contained |
| NuGet packages | Restored automatically via `dotnet restore` |

## Connection String Format

Edit `appsettings.json` with your Oracle credentials. The project uses **Easy Connect** format (no `tnsnames.ora` required):

```json
{
  "OracleConnection": "User Id=NATEJSOFT;Password={ORACLE_PASSWORD};Data Source=//10.10.1.1:1521/NATEJSOFT;"
}
```

| Parameter | Value | Notes |
|-----------|-------|-------|
| `User Id` | `NATEJSOFT` | Oracle database username |
| `Password` | `{ORACLE_PASSWORD}` | Placeholder — resolved from env var at runtime |
| `Data Source` | `//10.10.1.1:1521/NATEJSOFT` | Easy Connect: `//host:port/service` |

### How the Password Works

1. `appsettings.json` contains the placeholder `{ORACLE_PASSWORD}` — **not** the real password.
2. Before running, set the `ORACLE_PASSWORD` environment variable to the actual password.
3. `Program.cs` reads the env var and substitutes it into the connection string at startup.
4. If the env var is not set, the application prints a clear error and exits.

### Easy Connect Format (alternatives)

```
//host:port/service_name
//10.10.1.1:1521/NATEJSOFT
```

### Security Notes

- **Never commit real passwords** to source control.
- The `{ORACLE_PASSWORD}` placeholder in `appsettings.json` is safe to commit.
- Set `ORACLE_PASSWORD` as an environment variable on each machine where the app runs.
- Consider using Oracle Wallet for production deployments.

## Run Steps (detailed)

```bash
# 1. Set the ORACLE_PASSWORD environment variable
#    PowerShell: $env:ORACLE_PASSWORD = "COGNOS"
#    CMD:        set ORACLE_PASSWORD=COGNOS

# 2. Restore NuGet packages
dotnet restore

# 3. Build the project
dotnet build

# 4. Run
dotnet run
```

## Expected Output (Success)

```
==================================================
  WarehouseDashboard — Oracle Connectivity Test
==================================================

Target  : Oracle Database (ODP.NET Managed Driver)
Query   : SELECT SYSDATE FROM DUAL
Time    : 2026-07-12 15:30:00

[INFO] Opening connection to Oracle...
[OK]   Connection established successfully.
  Server Version : 19.0.0.0.0
  Database       : ORCL
  Connection ID  : 1a2b3c4d5e6f7g8h

[INFO] Executing query...
[OK]   Query executed successfully.
  SYSDATE value : 12-JUL-26 03:30:00 PM
  Type          : DateTime

[OK]   Oracle connectivity test completed successfully.

Press any key to exit...
```

## Troubleshooting

| Symptom | Likely Cause | Fix |
|---------|-------------|-----|
| ORA-01017 | Invalid username/password | Check that `ORACLE_PASSWORD` env var is set correctly |
| ORA-12154 | TNS alias not resolved | Use Easy Connect format `//host:port/service` (not TNS alias) |
| ORA-12541 | TNS listener down | Verify Oracle listener is running (`lsnrctl status`) |
| ORA-28000 / ORA-28001 | Account locked / password expired | Contact DBA |
| Connection refused (no Oracle error) | Wrong host/port in Easy Connect | Verify host:port/SERVICE_NAME is correct |
| Oracle.ManagedDataAccess.Core not found | Package not restored | Run `dotnet restore` |

## Project Structure

```
src/WarehouseDashboard.OracleTest/
├── WarehouseDashboard.OracleTest.csproj   # .NET 8 project file
├── Program.cs                             # Main entry point
├── appsettings.json                       # Connection string configuration
└── README.md                              # This file
```

## References

- [Oracle.ManagedDataAccess.Core NuGet](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/)
- [ODP.NET Managed Driver Documentation](https://docs.oracle.com/en/database/oracle/oracle-data-access-components/)
- [WarehouseDashboard Integration Spec](../project-preparation/14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md)
