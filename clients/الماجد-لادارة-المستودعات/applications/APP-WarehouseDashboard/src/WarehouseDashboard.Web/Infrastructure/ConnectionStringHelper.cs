namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Resolves the connection-string password placeholder from the SQL_PASSWORD
/// environment variable. Passwords are NEVER stored in source or config files.
/// </summary>
public static class ConnectionStringHelper
{
    public static string Resolve(string? template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

        var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;

        return template.Replace("{SQL_PASSWORD}", sqlPassword, StringComparison.Ordinal);
    }
}
