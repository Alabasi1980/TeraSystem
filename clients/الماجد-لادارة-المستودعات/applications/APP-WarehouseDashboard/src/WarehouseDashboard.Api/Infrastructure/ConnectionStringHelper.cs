using Microsoft.Extensions.Configuration;

namespace WarehouseDashboard.Api.Infrastructure;

/// <summary>
/// Resolves connection-string password placeholders from environment variables.
/// Passwords are NEVER stored in source or configuration files; they are supplied
/// at runtime via the SQL_PASSWORD and ORACLE_PASSWORD environment variables.
/// </summary>
public static class ConnectionStringHelper
{
    public static string Resolve(string? template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

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
    /// The password is read at runtime only — never from a file.
    /// </summary>
    public static string ResolveOracle(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var template = configuration.GetConnectionString("Oracle");
        return Resolve(template);
    }

    /// <summary>
    /// Reads the <c>ConnectionStrings:SqlServer</c> template from configuration and
    /// resolves the <c>{SQL_PASSWORD}</c> placeholder from the <c>SQL_PASSWORD</c>
    /// environment variable.
    /// The password is read at runtime only — never from a file.
    /// </summary>
    public static string ResolveSql(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var template = configuration.GetConnectionString("SqlServer");
        return Resolve(template);
    }
}
