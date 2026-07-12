# WarehouseDashboard.OracleTest

**Oracle Connectivity Test** — .NET 8 Console Application using ODP.NET Managed Driver.

## Purpose

This project verifies that a .NET application can connect to an Oracle Database via `Oracle.ManagedDataAccess.Core`. It performs a simple `SELECT SYSDATE FROM DUAL` query to confirm:

- Oracle server is reachable
- Authentication credentials are valid
- ODP.NET is correctly configured
- Basic SQL query execution works

## Prerequisites

| Requirement | Version / Notes |
|-------------|-----------------|
| .NET SDK | 8.0 or later (LTS) |
| Oracle Database | Any version 11g or later |
| Oracle Client | **Not required** — ODP.NET Managed Driver is self-contained |
| NuGet packages | Restored automatically via `dotnet restore` |

## Connection String Format

Edit `appsettings.json` with your Oracle credentials:

### 1. TNS Names (requires `tnsnames.ora`)

```json
{
  "OracleConnection": "User Id=YOUR_USER;Password=YOUR_PASSWORD;Data Source=YOUR_TNS_ALIAS;"
}
```

- `YOUR_TNS_ALIAS` must be defined in `tnsnames.ora`
- Set `TNS_ADMIN` environment variable to the directory containing `tnsnames.ora`, or place it in the default Oracle home location.

### 2. Easy Connect (no `tnsnames.ora` needed)

```json
{
  "OracleConnection": "User Id=YOUR_USER;Password=YOUR_PASSWORD;Data Source=//host:1521/SERVICE_NAME;"
}
```

Example:

```json
{
  "OracleConnection": "User Id=warehouse_user;Password=MyP@ssw0rd;Data Source=//192.168.1.100:1521/ORCL;"
}
```

### Security Notes

- **Never commit real passwords** to source control.
- Use environment variables or user secrets for production:
  ```
  $env:ORACLE_PASS="YourRealPassword"
  ```
  Then modify connection string: `Password=%ORACLE_PASS%`
- Consider using Oracle Wallet for production deployments.

## How to Run

```bash
# 1. Edit appsettings.json with your Oracle credentials

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
| ORA-01017 | Invalid username/password | Check credentials in `appsettings.json` |
| ORA-12154 | TNS alias not resolved | Check `tnsnames.ora` or switch to Easy Connect format |
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
