namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Resolves connection-string password placeholders from environment variables.
/// If the connection string contains hardcoded passwords (no placeholders),
/// it is returned as-is — no environment variables required.
/// </summary>
public static class ConnectionStringHelper
{
    public static string Resolve(string? template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

        // If password is already hardcoded (no placeholder), return as-is
        if (!template.Contains("{SQL_PASSWORD}"))
            return template;

        var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;
        return template.Replace("{SQL_PASSWORD}", sqlPassword, StringComparison.Ordinal);
    }
}
