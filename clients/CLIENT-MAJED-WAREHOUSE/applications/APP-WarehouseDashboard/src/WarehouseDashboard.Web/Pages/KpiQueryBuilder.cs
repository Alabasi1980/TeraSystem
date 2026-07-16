using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages;

/// <summary>
/// Builds additional SQL queries for Advanced KPI cards.
/// All queries are derived from the card's base SqlQuery by modifying WHERE clauses.
/// This ensures consistency — the same table and columns are always used.
/// </summary>
public static class KpiQueryBuilder
{
    /// <summary>
    /// Builds all needed KPI queries based on card configuration.
    /// Returns null for any query that isn't needed.
    /// </summary>
    public static KpiQueries Build(DashboardCard card)
    {
        var queries = new KpiQueries();

        // Only build queries for KPI chart type with non-simple mode
        if (!card.ChartType.Equals("KPI", StringComparison.OrdinalIgnoreCase))
            return queries;

        if (card.KpiMode == "simple")
            return queries;

        // Change query — needed for "withChange" and "composite"
        if (card.ShowChange && !string.IsNullOrEmpty(card.ValueColumn) && !string.IsNullOrEmpty(card.DateColumn))
        {
            queries.ChangeSql = BuildChangeQuery(card);
        }

        // Sparkline and Grand Total — only for "composite"
        if (card.KpiMode == "composite")
        {
            if (card.ShowSparkline && !string.IsNullOrEmpty(card.ValueColumn) && !string.IsNullOrEmpty(card.DateColumn))
            {
                queries.SparklineSql = BuildSparklineQuery(card);
            }

            if (card.ShowGrandTotal && !string.IsNullOrEmpty(card.ValueColumn))
            {
                queries.GrandTotalSql = BuildGrandTotalQuery(card);
            }
        }

        return queries;
    }

    /// <summary>
    /// Builds a query to get the value from the previous period for change calculation.
    /// Uses the same table but filters to the previous time period.
    /// </summary>
    private static string BuildChangeQuery(DashboardCard card)
    {
        var baseQuery = card.SqlQuery.Trim().TrimEnd(';');
        var valueCol = SanitizeIdentifier(card.ValueColumn);
        var dateCol = SanitizeIdentifier(card.DateColumn);

        // Determine the date range for the previous period
        var (prevStart, prevEnd) = GetPreviousPeriodRange(card);

        // Wrap the base query and add date filter
        return $"SELECT SUM({valueCol}) AS PreviousValue FROM ({baseQuery}) AS _base " +
               $"WHERE {dateCol} >= '{prevStart:yyyy-MM-dd}' AND {dateCol} < '{prevEnd:yyyy-MM-dd}'";
    }

    /// <summary>
    /// Builds a query to get monthly aggregated data for sparkline chart.
    /// Groups by month and returns the last N months of data.
    /// </summary>
    private static string BuildSparklineQuery(DashboardCard card)
    {
        var baseQuery = card.SqlQuery.Trim().TrimEnd(';');
        var valueCol = SanitizeIdentifier(card.ValueColumn);
        var dateCol = SanitizeIdentifier(card.DateColumn);
        var months = card.SparklineMonths > 0 ? card.SparklineMonths : 6;

        // Calculate the start date (N months ago from today)
        var startDate = DateTime.UtcNow.AddMonths(-months);

        return $"SELECT FORMAT({dateCol}, 'yyyy-MM') AS Month, SUM({valueCol}) AS MonthlyValue " +
               $"FROM ({baseQuery}) AS _base " +
               $"WHERE {dateCol} >= '{startDate:yyyy-MM-dd}' " +
               $"GROUP BY FORMAT({dateCol}, 'yyyy-MM') " +
               $"ORDER BY Month";
    }

    /// <summary>
    /// Builds a query to get the grand total (all-time, no date filter).
    /// </summary>
    private static string BuildGrandTotalQuery(DashboardCard card)
    {
        var baseQuery = card.SqlQuery.Trim().TrimEnd(';');
        var valueCol = SanitizeIdentifier(card.ValueColumn);

        // For grand total, we use the base query without any date filter
        return $"SELECT SUM({valueCol}) AS GrandTotal FROM ({baseQuery}) AS _base";
    }

    /// <summary>
    /// Determines the previous period date range based on card's ChangeSource setting.
    /// </summary>
    private static (DateTime Start, DateTime End) GetPreviousPeriodRange(DashboardCard card)
    {
        var now = DateTime.UtcNow;

        return card.ChangeSource switch
        {
            "previousMonth" => (
                new DateTime(now.Year, now.Month, 1).AddMonths(-1),
                new DateTime(now.Year, now.Month, 1)
            ),
            "previousYear" => (
                new DateTime(now.Year - 1, now.Month, 1),
                new DateTime(now.Year, now.Month, 1)
            ),
            // Default: previousPeriod — last 30 days before today
            _ => (
                now.AddDays(-60),
                now.AddDays(-30)
            )
        };
    }

    /// <summary>
    /// Sanitizes a column name to prevent SQL injection.
    /// Wraps in square brackets and removes dangerous characters.
    /// </summary>
    private static string SanitizeIdentifier(string name)
    {
        var cleaned = name.Replace("[", "").Replace("]", "").Replace(";", "").Trim();
        return $"[{cleaned}]";
    }
}

/// <summary>
/// Holds the additional SQL queries needed for an Advanced KPI card.
/// </summary>
public class KpiQueries
{
    /// <summary>SQL for change percentage calculation (previous period value).</summary>
    public string? ChangeSql { get; set; }

    /// <summary>SQL for sparkline monthly data.</summary>
    public string? SparklineSql { get; set; }

    /// <summary>SQL for grand total (all-time).</summary>
    public string? GrandTotalSql { get; set; }
}
