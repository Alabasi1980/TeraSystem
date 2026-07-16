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
builder.Services.AddSingleton<SyncRunProgressStore>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", policy =>
    {
        // Read allowed origins from configuration (CORS:Origins array).
        // Fall back to hardcoded defaults when the config section is missing.
        var configuredOrigins = builder.Configuration.GetSection("CORS:Origins").Get<string[]>();
        var allowedOrigins = configuredOrigins is { Length: > 0 }
            ? configuredOrigins
            : new[]
            {
                "https://localhost:5000",
                "http://localhost:5000",
                "https://10.10.1.1",
                "http://10.10.1.1"
            };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Start the in-memory progress-store cleanup timer so that expired runs (older than 5 min)
// are evicted every 60 seconds. Must happen after the service provider is built.
var progressStore = app.Services.GetRequiredService<SyncRunProgressStore>();
progressStore.Initialize();

app.UseCors("AllowWeb");

app.MapControllers();

app.Run();
