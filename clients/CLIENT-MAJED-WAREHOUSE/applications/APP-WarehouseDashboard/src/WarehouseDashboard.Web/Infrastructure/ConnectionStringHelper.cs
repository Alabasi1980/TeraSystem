using Microsoft.Extensions.Configuration;

namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Resolves connection-string password placeholders from environment variables.
/// If the connection string contains hardcoded passwords (no placeholders),
/// it is returned as-is — no environment variables required.
/// For production, use placeholders and supply passwords via env vars.
/// </summary>
public static class ConnectionStringHelper
{
    public static string Resolve(string? template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

        // If passwords are already hardcoded (no placeholders), return as-is
        if (!template.Contains("{SQL_PASSWORD}") && !template.Contains("{ORACLE_PASSWORD}"))
            return template;

        var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;
        var oraclePassword = Environment.GetEnvironmentVariable("ORACLE_PASSWORD") ?? string.Empty;

        return template
            .Replace("{SQL_PASSWORD}", sqlPassword, StringComparison.Ordinal)
            .Replace("{ORACLE_PASSWORD}", oraclePassword, StringComparison.Ordinal);
    }

    /// <summary>
    /// Reads the <c>ConnectionStrings:Oracle</c> template from configuration and
    /// resolves the <c>{ORACLE_PASSWORD}</c> placeholder from the
    /// <c>ORACLE_PASSWORD</c> environment variable.
    /// </summary>
    public static string ResolveOracle(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var template = configuration.GetConnectionString("Oracle");
        return Resolve(template);
    }

    /// <summary>
    /// Reads the <c>ConnectionStrings:SqlServer</c> template from configuration and
    /// resolves the <c>{SQL_PASSWORD}</c> placeholder from the
    /// <c>SQL_PASSWORD</c> environment variable.
    /// </summary>
    public static string ResolveSql(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var template = configuration.GetConnectionString("SqlServer");
        return Resolve(template);
    }
}
