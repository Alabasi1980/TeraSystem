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
    public static KpiQueries Build(DashboardCard card, DashboardService.DateRange? dateRange = null)
    {
        var queries = new KpiQueries();

        // Only build queries for KPI chart type with non-simple mode
        if (!card.ChartType.Equals("KPI", StringComparison.OrdinalIgnoreCase))
            return queries;

        if (card.KpiMode == "simple")
            return queries;

        var hasChangeMode = string.Equals(card.KpiMode, "withChange", StringComparison.OrdinalIgnoreCase)
            || string.Equals(card.KpiMode, "composite", StringComparison.OrdinalIgnoreCase);

        // Change query — needed for "withChange" and "composite" even if ShowChange was not saved correctly.
        // customQuery is intentionally not implemented yet; leave the percentage unavailable for the UI fallback.
        if ((card.ShowChange || hasChangeMode)
            && !string.Equals(card.ChangeSource, "customQuery", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(card.ValueColumn)
            && !string.IsNullOrEmpty(card.DateColumn))
        {
            queries.ChangeSql = BuildChangeQuery(card, dateRange);
        }

        // Sparkline and Grand Total — only for "composite"
        if (card.KpiMode == "composite")
        {
            if (card.ShowSparkline && !string.IsNullOrEmpty(card.ValueColumn) && !string.IsNullOrEmpty(card.DateColumn))
            {
                queries.SparklineSql = BuildSparklineQuery(card, dateRange);
            }

            if (card.ShowGrandTotal && !string.IsNullOrEmpty(card.ValueColumn))
            {
                queries.GrandTotalSql = BuildGrandTotalQuery(card);
            }
        }

        // Category breakdown — automatic when CategoryColumn and ValueColumn are both set
        if (!string.IsNullOrEmpty(card.CategoryColumn) && !string.IsNullOrEmpty(card.ValueColumn))
        {
            queries.BreakdownSql = BuildCategoryBreakdownQuery(card, dateRange);
        }

        return queries;
    }

    /// <summary>
    /// Builds a query to get the value from the previous period for change calculation.
    /// Uses the same table but filters to the previous time period.
    /// </summary>
    private static string BuildChangeQuery(DashboardCard card, DashboardService.DateRange? dateRange = null)
    {
        var baseQuery = NormalizeBaseQuery(card.SqlQuery);
        var valueCol = NumericExpression(card.ValueColumn);
        var dateCol = SanitizeIdentifier(card.DateColumn);

        DateTime prevStart, prevEnd;
        if (dateRange is not null)
        {
            (prevStart, prevEnd) = GetComparisonRange(card, dateRange);
        }
        else
        {
            // Fallback: use existing GetPreviousPeriodRange logic
            (prevStart, prevEnd) = GetPreviousPeriodRange(card);
        }

        return $"SELECT SUM({valueCol}) AS PreviousValue FROM ({baseQuery}) AS _base " +
               $"WHERE {dateCol} >= '{prevStart:yyyy-MM-dd}' AND {dateCol} < '{prevEnd:yyyy-MM-dd}'";
    }

    /// <summary>
    /// Builds a query to get monthly aggregated data for sparkline chart.
    /// Groups by month and returns the last N months of data.
    /// </summary>
    private static string BuildSparklineQuery(DashboardCard card, DashboardService.DateRange? dateRange = null)
    {
        var baseQuery = NormalizeBaseQuery(card.SqlQuery);
        var valueCol = NumericExpression(card.ValueColumn);
        var dateCol = SanitizeIdentifier(card.DateColumn);

        var sparklineMonths = card.SparklineMonths > 0 ? card.SparklineMonths : 6;

        // Sparkline ALWAYS goes back SparklineMonths from today (or from range start),
        // never limited to the dashboard filter range.
        DateTime startDate;
        if (dateRange is not null)
        {
            // Go back SparklineMonths from the filter's start date
            startDate = dateRange.From.AddMonths(-sparklineMonths);
        }
        else
        {
            startDate = DateTime.UtcNow.AddMonths(-sparklineMonths);
        }

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
        var baseQuery = NormalizeBaseQuery(card.SqlQuery);
        var valueCol = NumericExpression(card.ValueColumn);

        // For grand total, we use the base query without any date filter
        return $"SELECT SUM({valueCol}) AS GrandTotal FROM ({baseQuery}) AS _base";
    }

    /// <summary>
    /// Builds a query to get top 5 categories by value for the category breakdown table.
    /// </summary>
    private static string BuildCategoryBreakdownQuery(DashboardCard card, DashboardService.DateRange? dateRange = null)
    {
        var baseQuery = NormalizeBaseQuery(card.SqlQuery);
        var valueCol = NumericExpression(card.ValueColumn);
        var categoryCol = SanitizeIdentifier(card.CategoryColumn);

        string dateFilter = "";
        if (dateRange is not null && !string.IsNullOrEmpty(card.DateColumn))
        {
            var dateCol = SanitizeIdentifier(card.DateColumn);
            dateFilter = $" WHERE {dateCol} >= '{dateRange.From:yyyy-MM-dd}' AND {dateCol} < '{dateRange.To:yyyy-MM-dd}'";
        }

        return $"SELECT TOP 5 {categoryCol} AS Category, SUM({valueCol}) AS CategoryValue " +
               $"FROM ({baseQuery}) AS _base" +
               $"{dateFilter} " +
               $"GROUP BY {categoryCol} " +
               $"ORDER BY CategoryValue DESC";
    }

    /// <summary>
    /// Determines the comparison date range when the card has an active current range.
    /// </summary>
    private static (DateTime Start, DateTime End) GetComparisonRange(DashboardCard card, DashboardService.DateRange dateRange)
    {
        return card.ChangeSource switch
        {
            "previousMonth" => (dateRange.From.AddMonths(-1), dateRange.To.AddMonths(-1)),
            "previousYear" => (dateRange.From.AddYears(-1), dateRange.To.AddYears(-1)),
            // Default previousPeriod = same duration, immediately before the filtered range.
            _ => GetPreviousPeriodRange(dateRange)
        };
    }

    private static (DateTime Start, DateTime End) GetPreviousPeriodRange(DashboardService.DateRange dateRange)
    {
        var duration = dateRange.To - dateRange.From;
        var prevEnd = dateRange.From;
        var prevStart = prevEnd - duration;
        return (prevStart, prevEnd);
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
    /// Normalizes the configured card SQL for KPI helper queries.
    /// Bare table/view tokens are wrapped the same way DashboardService does before helper queries wrap them again.
    /// </summary>
    private static string NormalizeBaseQuery(string sqlQuery)
    {
        var trimmed = sqlQuery.Trim().TrimEnd(';').Trim();
        var isFullQuery = trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
            || trimmed.Contains(" FROM ", StringComparison.OrdinalIgnoreCase);

        if (isFullQuery)
            return trimmed;

        var safe = trimmed.StartsWith("[", StringComparison.Ordinal) ? trimmed : $"[{trimmed}]";
        return $"SELECT * FROM {safe}";
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

    private static string NumericExpression(string name)
    {
        return $"TRY_CAST({SanitizeIdentifier(name)} AS DECIMAL(28,6))";
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

    /// <summary>SQL for category breakdown (top 5 categories by value).</summary>
    public string? BreakdownSql { get; set; }
}
