using System.Globalization;

namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Shared helper methods for dashboard data processing. Extracted from
/// <see cref="Pages.DashboardService"/> to eliminate duplication across
/// the dashboard and drill-down pages.
/// </summary>
public static class DataHelper
{
    /// <summary>Converts a DataReader cell into a JSON-friendly value.</summary>
    public static object? ConvertCell(object value)
    {
        if (value is DBNull or null)
        {
            return null;
        }

        if (value is DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        if (value is byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        // Numbers, bool, string pass through untouched.
        return value;
    }

    /// <summary>
    /// Strips any accidental secret leakage (resolved password) from an error
    /// message before it is sent to the browser.
    /// </summary>
    public static string Sanitize(string message)
    {
        var password = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;
        var cleaned = message.Replace("{SQL_PASSWORD}", "***", StringComparison.Ordinal);
        if (password.Length > 0)
        {
            cleaned = cleaned.Replace(password, "***", StringComparison.Ordinal);
        }

        return cleaned;
    }

    /// <summary>
    /// Applies a date range filter to a SQL query by injecting into the WHERE clause.
    /// Strips ORDER BY first, then adds the date condition to the existing WHERE clause
    /// (or appends a new WHERE clause if none exists). Preserves @p0 parameters.
    /// </summary>
    public static string ApplyDateFilter(string baseSql, string dateColumn, DateTime from, DateTime to)
    {
        baseSql = StripOrderBy(baseSql);
        var dateCol = SanitizeIdentifier(dateColumn);
        var fromStr = from.ToString("yyyy-MM-dd");
        var nextDay = to.AddDays(1).ToString("yyyy-MM-dd");
        var dateCondition = $"{dateCol} >= '{fromStr}' AND {dateCol} < '{nextDay}'";

        var trimmed = baseSql.TrimEnd(';', ' ', '\t', '\r', '\n');
        var whereIdx = trimmed.LastIndexOf(" WHERE ", StringComparison.OrdinalIgnoreCase);

        if (whereIdx < 0)
        {
            // No WHERE clause — append one
            // Ensure there's a space before WHERE
            return $"{trimmed} WHERE {dateCondition}";
        }

        // Has WHERE clause — append date condition with AND
        var beforeWhere = trimmed[..(whereIdx + 7)]; // include " WHERE "
        var afterWhere = trimmed[(whereIdx + 7)..];
        return $"{beforeWhere}{dateCondition} AND {afterWhere}";
    }

    /// <summary>
    /// Strips trailing ORDER BY clause from a SQL query.
    /// Required before wrapping in a subquery (SQL Server forbids ORDER BY in derived tables without TOP/OFFSET).
    /// </summary>
    public static string StripOrderBy(string sql)
    {
        var trimmed = sql.TrimEnd(';', ' ', '\t', '\r', '\n');
        var orderByIdx = trimmed.LastIndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
        if (orderByIdx < 0)
            return sql;

        // Verify it's a top-level ORDER BY (not inside a subquery or function)
        // Simple heuristic: count parentheses before ORDER BY
        var beforeOrderBy = trimmed[..orderByIdx];
        var parenCount = 0;
        foreach (var c in beforeOrderBy)
        {
            if (c == '(') parenCount++;
            else if (c == ')') parenCount--;
        }
        if (parenCount == 0)
        {
            // Top-level ORDER BY — strip it
            return beforeOrderBy.TrimEnd();
        }

        return sql;
    }

    /// <summary>
    /// Sanitizes a column name to prevent SQL injection.
    /// </summary>
    public static string SanitizeIdentifier(string name)
    {
        var cleaned = name.Replace("[", "").Replace("]", "").Replace(";", "").Trim();
        return $"[{cleaned}]";
    }
}
