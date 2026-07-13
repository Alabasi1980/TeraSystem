using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Syncfusion license is read from the SYNCFUSION_LICENSE_KEY environment variable.
// It is NEVER hardcoded in source or configuration files.
// ---------------------------------------------------------------------------
var syncLicense = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
if (!string.IsNullOrWhiteSpace(syncLicense))
{
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncLicense);
}
else
{
    Console.WriteLine("[WARN] SYNCFUSION_LICENSE_KEY env var is not set. " +
                      "Syncfusion components will display a license warning.");
}

// ---------------------------------------------------------------------------
// EF Core DbContext (placeholder).
// Config + Logs entities (DashboardCards, SyncSettings, SyncLogs, ...) are added
// in a later task. Connection string password is resolved from env var.
// ---------------------------------------------------------------------------
var sqlConnection = ConnectionStringHelper.Resolve(builder.Configuration.GetConnectionString("SqlServer"));
builder.Services.AddDbContext<WarehouseDashboardDbContext>(options =>
    options.UseSqlServer(sqlConnection));

// ---------------------------------------------------------------------------
// Admin Panel authentication (TASK-COD-008).
// Auth state is kept in an HttpContext.Session flag — never in a cookie value
// or on disk. The session cookie is hardened: HttpOnly, SameSite=Strict, and
// Secure over HTTPS (SameAsRequest so local http dev still works; set to
// Always in production behind TLS).
// ---------------------------------------------------------------------------
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "WarehouseDashboard.AdminSession";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
// Protects /admin-secure-panel/* (Login/Logout excluded) — see AdminAuthMiddleware.
app.UseMiddleware<AdminAuthMiddleware>();
app.MapRazorPages();

app.Run();
