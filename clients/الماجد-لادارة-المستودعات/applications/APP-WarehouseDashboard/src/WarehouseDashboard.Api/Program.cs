using WarehouseDashboard.Api.Infrastructure;
using WarehouseDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Connection strings are stored as TEMPLATES in appsettings.json.
// Passwords are resolved from environment variables at runtime (never hardcoded).
// ---------------------------------------------------------------------------
if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQL_PASSWORD")))
    Console.WriteLine("[WARN] SQL_PASSWORD env var is not set. SQL Server access will fail.");

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ORACLE_PASSWORD")))
    Console.WriteLine("[WARN] ORACLE_PASSWORD env var is not set. Oracle access will fail.");

// ---------------------------------------------------------------------------
// Service registration.
// OracleExtractionService: SINGLETON (stateless, fresh OracleConnection per call).
// SqlServerLoadService : SINGLETON (stateless, fresh SqlConnection per call; resolves the
//                        SQL connection string from IConfiguration + SQL_PASSWORD env var).
// SyncEngineService    : registered as a SINGLETON and ALSO run as the hosted background
//                        service. The hosted-service registration resolves that same
//                        singleton instance (factory) so the controller-injected engine and
//                        the running engine are one and the same — sharing runtime status.
// SyncRunLogStore      : SINGLETON in-memory ring buffer for recent sync runs (temporary;
//                        see SyncController — replaced by DB logging in a later task).
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<OracleExtractionService>();
builder.Services.AddSingleton<SqlServerLoadService>();
builder.Services.AddSingleton<SyncEngineService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<SyncEngineService>());
builder.Services.AddSingleton<SyncRunLogStore>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
