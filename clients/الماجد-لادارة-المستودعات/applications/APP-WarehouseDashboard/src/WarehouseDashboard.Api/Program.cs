using WarehouseDashboard.Api.Infrastructure;
using WarehouseDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Connection strings with passwords are stored directly in appsettings.json
// for development/testing. For production, use environment variables with
// {SQL_PASSWORD} and {ORACLE_PASSWORD} placeholders.
// ---------------------------------------------------------------------------

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", policy =>
    {
        policy.WithOrigins("https://localhost:5000", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowWeb");

app.MapControllers();

app.Run();
