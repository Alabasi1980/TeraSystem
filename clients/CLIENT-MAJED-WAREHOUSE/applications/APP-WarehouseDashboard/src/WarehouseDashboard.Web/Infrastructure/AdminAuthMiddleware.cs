using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Protects the hidden admin area (/admin-secure-panel/*).
///
/// Authentication approach (documented):
/// - Auth state is held in an <see cref="ISession"/> flag ("AdminAuthenticated"),
///   so no password or secret is ever written to a cookie or to disk.
/// - The session cookie itself is configured HttpOnly + SameSite=Strict in Program.cs,
///   and Secure over HTTPS (SameAsRequest so local dev over http still works).
/// - Any request under /admin-secure-panel/* (except Login and Logout) without a
///   valid session flag is redirected to the Login page.
/// - **Bypass**: If appsettings.json has <c>AdminAuth:Bypass = true</c>, all admin
///   pages are accessible without authentication. Use for local dev/testing only.
///
/// This is sufficient for Phase 1 (single shared password, local network).
/// Phase 2 can replace it with ASP.NET Core Identity / RBAC.
/// </summary>
public class AdminAuthMiddleware
{
    private readonly RequestDelegate _next;

    private const string AreaPrefix = "/admin-secure-panel";
    private const string LoginPath = "/admin-secure-panel/Login";
    private const string LogoutPath = "/admin-secure-panel/Logout";
    private const string SessionKey = "AdminAuthenticated";

    public AdminAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (path.StartsWith(AreaPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var isLogin = path.Equals(LoginPath, StringComparison.OrdinalIgnoreCase);
            var isLogout = path.Equals(LogoutPath, StringComparison.OrdinalIgnoreCase);

            // Login and Logout must always be reachable.
            if (!isLogin && !isLogout)
            {
                // Bypass: if AdminAuth:Bypass = true in config, skip auth check.
                var config = context.RequestServices.GetRequiredService<IConfiguration>();
                if (config.GetValue<bool>("AdminAuth:Bypass"))
                {
                    await _next(context);
                    return;
                }

                if (context.Session.GetString(SessionKey) != "true")
                {
                    context.Response.Redirect(LoginPath);
                    return;
                }
            }
        }

        await _next(context);
    }
}
