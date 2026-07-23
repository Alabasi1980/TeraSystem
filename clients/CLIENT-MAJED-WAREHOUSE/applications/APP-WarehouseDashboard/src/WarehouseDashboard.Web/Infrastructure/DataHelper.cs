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
    /// Wraps a base SQL query with a date range filter on the specified date column.
    /// Uses &lt; next_day for exclusive end boundary (handles datetime with time components correctly).
    /// </summary>
    public static string ApplyDateFilter(string baseSql, string dateColumn, DateTime from, DateTime to)
    {
        var dateCol = SanitizeIdentifier(dateColumn);
        var fromStr = from.ToString("yyyy-MM-dd");
        var nextDay = to.AddDays(1).ToString("yyyy-MM-dd");
        return $"SELECT * FROM ({baseSql.TrimEnd(';')}) AS _datefiltered " +
               $"WHERE {dateCol} >= '{fromStr}' AND {dateCol} < '{nextDay}'";
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
