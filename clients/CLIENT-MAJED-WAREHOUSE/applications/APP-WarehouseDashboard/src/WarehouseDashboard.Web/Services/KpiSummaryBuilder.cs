using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Builds structured <see cref="CardSummary"/> for KPI-type dashboard cards.
/// Handles date-scoped aggregation, series data, top/bottom categories,
/// depth-based date ranges, and trend detection.
/// </summary>
public class KpiSummaryBuilder : ICardSummaryBuilder
{
    private readonly ReadOnlyQueryHelper _readOnly;
    private readonly ILogger<KpiSummaryBuilder> _logger;

    // Depth level → (Arabic label, months back, years back). Months and years are exclusive.
    private static readonly Dictionary<int, (string Label, int Months, int Years)> DepthConfig = new()
    {
        [1] = ("3 أشهر",    3, 0),
        [2] = ("6 أشهر",    6, 0),
        [3] = ("12 شهر",   12, 0),
        [4] = ("3 سنوات",   0, 3),
        [5] = ("5 سنوات",   0, 5),
        [6] = ("10 سنوات",  0, 10),
    };

    public string ChartType => "KPI";

    public KpiSummaryBuilder(ReadOnlyQueryHelper readOnly, ILogger<KpiSummaryBuilder> logger)
    {
        _readOnly = readOnly;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Public entry point
    // ─────────────────────────────────────────────────────────────────

    public async Task<CardSummary> BuildSummaryAsync(DashboardCard card, int depthLevel, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var summary = new CardSummary
        {
            CardId          = card.Id,
            Title           = card.Title,
            ChartType       = card.ChartType,
            Description     = string.IsNullOrWhiteSpace(card.Description) ? null : card.Description,
            AssistantPrompt = card.AssistantPrompt,
            DepthLevel      = depthLevel,
        };

        // ── Depth label ────────────────────────────────────────────
        summary.DepthLabel = DepthConfig.TryGetValue(depthLevel, out var dc)
            ? dc.Label
            : $"المستوى {depthLevel}";

        // ── Parameters from card config ────────────────────────────
        string agg       = NormaliseAggregation(card.AggregationType);
        string valueCol  = card.ValueColumn;
        string dateCol   = card.DateColumn;
        string sqlQuery  = card.SqlQuery;

        bool hasDate = !string.IsNullOrWhiteSpace(dateCol);

        // ── Validate essential fields ──────────────────────────────
        if (string.IsNullOrWhiteSpace(sqlQuery))
        {
            summary.DataQualityNotes.Add("استعلام SQL غير محدد للبطاقة.");
            return summary;
        }
        if (string.IsNullOrWhiteSpace(valueCol))
        {
            summary.DataQualityNotes.Add("عمود القيمة غير محدد للبطاقة.");
            return summary;
        }

        // ── Date scope ─────────────────────────────────────────────
        summary.HasDateColumn = hasDate;
        DateTime today   = DateTime.Today;
        DateTime? dateFrom = null;
        DateTime? dateTo   = null;

        if (hasDate)
        {
            summary.DateColumn = dateCol;

            if (DepthConfig.TryGetValue(depthLevel, out dc))
            {
                dateFrom = dc.Years > 0
                    ? today.AddYears(-dc.Years)
                    : today.AddMonths(-dc.Months);
                dateTo = today;

                summary.DateFrom = dateFrom;
                summary.DateTo   = dateTo;

                ct.ThrowIfCancellationRequested();
                await DetectDataBoundsAsync(summary, dateCol, sqlQuery, dateFrom.Value, dateTo.Value);
            }
        }
        else
        {
            summary.DataQualityNotes.Add("لا يوجد بُعد زمني");
        }

        // ── Current value ──────────────────────────────────────────
        ct.ThrowIfCancellationRequested();
        summary.CurrentValue = await QueryAggregateValueAsync(
            sqlQuery, valueCol, dateCol, agg, dateFrom, dateTo);

        // ── Previous value ─────────────────────────────────────────
        if (hasDate && dateFrom.HasValue && dateTo.HasValue
            && DepthConfig.TryGetValue(depthLevel, out dc))
        {
            (DateTime prevFrom, DateTime prevTo) = PreviousRange(dateFrom.Value, dateTo.Value, dc);
            ct.ThrowIfCancellationRequested();
            summary.PreviousValue = await QueryAggregateValueAsync(
                sqlQuery, valueCol, dateCol, agg, prevFrom, prevTo);
        }

        // ── Change % ───────────────────────────────────────────────
        if (summary.CurrentValue.HasValue
            && summary.PreviousValue.HasValue
            && summary.PreviousValue.Value != 0)
        {
            summary.ChangePercent =
                ((summary.CurrentValue.Value - summary.PreviousValue.Value)
                 / summary.PreviousValue.Value) * 100.0;
        }

        // ── Series data (monthly group, max 24 points) ─────────────
        if (hasDate && dateFrom.HasValue && dateTo.HasValue)
        {
            string seriesAgg = IsNone(agg) ? "Sum" : agg;
            ct.ThrowIfCancellationRequested();
            await QuerySeriesDataAsync(summary, sqlQuery, valueCol, dateCol,
                seriesAgg, dateFrom.Value, dateTo.Value);
        }

        // ── Top/Bottom categories ──────────────────────────────────
        string catCol = card.CategoryColumn;
        if (!string.IsNullOrWhiteSpace(catCol))
        {
            string itemAgg = IsNone(agg) ? "Sum" : agg;
            ct.ThrowIfCancellationRequested();
            await QueryTopBottomAsync(summary, sqlQuery, valueCol, catCol, dateCol,
                itemAgg, dateFrom, dateTo);
        }

        // ── Trend direction ────────────────────────────────────────
        if (summary.SeriesData.Count >= 2)
        {
            double first = summary.SeriesData[0].Value;
            double last  = summary.SeriesData[^1].Value;
            summary.TrendDirection = last > first ? "up"
                                   : last < first ? "down"
                                   : "stable";
        }

        return summary;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Private helpers
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Queries MIN(date) to determine IsFullDataReached and HasDeeperData.
    /// </summary>
    private async Task DetectDataBoundsAsync(
        CardSummary summary, string dateCol, string sqlQuery,
        DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            var rows = await _readOnly.QueryAsync(
                $"SELECT MIN({dateCol}) AS MinDate FROM ({sqlQuery}) sub");

            if (rows.Count > 0
                && rows[0].TryGetValue("MinDate", out var raw)
                && raw is DateTime minDate)
            {
                // Full data reached = earliest available row is within (or after) our range start
                summary.IsFullDataReached = minDate >= dateFrom;
                // Has deeper data = there is data older than our range start
                summary.HasDeeperData = minDate < dateFrom;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "DetectDataBounds failed for card {CardId}", summary.CardId);
            summary.DataQualityNotes.Add("تعذر تحديد مدى البيانات المتاحة.");
        }
    }

    /// <summary>
    /// Runs the aggregate (or TOP‑1) query for a single scalar value.
    /// </summary>
    private async Task<double?> QueryAggregateValueAsync(
        string sqlQuery, string valueCol, string dateCol, string agg,
        DateTime? dateFrom, DateTime? dateTo)
    {
        try
        {
            string sql;
            Dictionary<string, object>? prms = null;

            bool hasDateFilter = !string.IsNullOrWhiteSpace(dateCol)
                                 && dateFrom.HasValue && dateTo.HasValue;

            if (IsNone(agg))
            {
                sql = hasDateFilter
                    ? $"SELECT TOP 1 {valueCol} AS cv FROM ({sqlQuery}) sub WHERE {dateCol} >= @from AND {dateCol} <= @to"
                    : $"SELECT TOP 1 {valueCol} AS cv FROM ({sqlQuery}) sub";
            }
            else
            {
                string func = SqlAggFunction(agg);
                sql = hasDateFilter
                    ? $"SELECT {func}({valueCol}) AS cv FROM ({sqlQuery}) sub WHERE {dateCol} >= @from AND {dateCol} <= @to"
                    : $"SELECT {func}({valueCol}) AS cv FROM ({sqlQuery}) sub";
            }

            if (hasDateFilter)
            {
                prms = new Dictionary<string, object>
                {
                    ["@from"] = dateFrom!.Value,
                    ["@to"]   = dateTo!.Value,
                };
            }

            var rows = await _readOnly.QueryAsync(sql, prms);
            if (rows.Count > 0 && rows[0].TryGetValue("cv", out var cv) && cv != null)
            {
                return Convert.ToDouble(cv);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "QueryAggregateValue failed (agg={Agg})", agg);
        }
        return null;
    }

    /// <summary>
    /// Queries monthly-aggregated series data (max 24 points).
    /// </summary>
    private async Task QuerySeriesDataAsync(
        CardSummary summary, string sqlQuery, string valueCol, string dateCol,
        string agg, DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            string func = SqlAggFunction(agg);
            string sql = $@"
SELECT FORMAT({dateCol},'yyyy-MM') AS period, {func}({valueCol}) AS val
FROM ({sqlQuery}) sub
WHERE {dateCol} >= @from AND {dateCol} <= @to
GROUP BY FORMAT({dateCol},'yyyy-MM')
ORDER BY period";

            var prms = new Dictionary<string, object>
            {
                ["@from"] = dateFrom,
                ["@to"]   = dateTo,
            };

            var rows = await _readOnly.QueryAsync(sql, prms);

            int taken = 0;
            foreach (var row in rows)
            {
                if (taken >= 24) break;

                if (row.TryGetValue("period", out var p)
                    && row.TryGetValue("val", out var v)
                    && v != null)
                {
                    summary.SeriesData.Add(new SeriesPoint
                    {
                        Period = p?.ToString() ?? string.Empty,
                        Value  = Convert.ToDouble(v),
                    });
                    taken++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "QuerySeriesData failed for card {CardId}", summary.CardId);
            summary.DataQualityNotes.Add("تعذر تحميل بيانات السلاسل الزمنية.");
        }
    }

    /// <summary>
    /// Queries top‑5 and bottom‑5 items by category column.
    /// </summary>
    private async Task QueryTopBottomAsync(
        CardSummary summary, string sqlQuery, string valueCol, string catCol,
        string dateCol, string agg, DateTime? dateFrom, DateTime? dateTo)
    {
        try
        {
            string func = SqlAggFunction(agg);
            bool hasDate = !string.IsNullOrWhiteSpace(dateCol)
                           && dateFrom.HasValue && dateTo.HasValue;

            string where = hasDate
                ? $"WHERE {dateCol} >= @from AND {dateCol} <= @to"
                : "";

            var prms = hasDate
                ? new Dictionary<string, object>
                {
                    ["@from"] = dateFrom!.Value,
                    ["@to"]   = dateTo!.Value,
                }
                : null;

            // ── Top 5 ────────────────────────────────────────────
            string topSql = $@"
SELECT TOP 5 {catCol} AS name, {func}({valueCol}) AS val
FROM ({sqlQuery}) sub
{where}
GROUP BY {catCol}
ORDER BY val DESC";

            var topRows = await _readOnly.QueryAsync(topSql, prms);
            double topTotal = 0;
            foreach (var row in topRows)
            {
                if (row.TryGetValue("name", out var n)
                    && row.TryGetValue("val", out var v)
                    && v != null)
                {
                    double d = Convert.ToDouble(v);
                    topTotal += d;
                    summary.TopItems.Add(new CategoryItem
                    {
                        Name  = n?.ToString() ?? string.Empty,
                        Value = d,
                    });
                }
            }
            // Calculate percent of top total for each item
            foreach (var item in summary.TopItems)
                item.Percent = topTotal > 0 ? (item.Value / topTotal) * 100.0 : 0;

            // ── Bottom 5 ────────────────────────────────────────
            string bottomSql = $@"
SELECT TOP 5 {catCol} AS name, {func}({valueCol}) AS val
FROM ({sqlQuery}) sub
{where}
GROUP BY {catCol}
ORDER BY val ASC";

            var bottomRows = await _readOnly.QueryAsync(bottomSql, prms);
            double bottomTotal = 0;
            foreach (var row in bottomRows)
            {
                if (row.TryGetValue("name", out var n)
                    && row.TryGetValue("val", out var v)
                    && v != null)
                {
                    double d = Convert.ToDouble(v);
                    bottomTotal += d;
                    summary.BottomItems.Add(new CategoryItem
                    {
                        Name  = n?.ToString() ?? string.Empty,
                        Value = d,
                    });
                }
            }
            foreach (var item in summary.BottomItems)
                item.Percent = bottomTotal > 0 ? (item.Value / bottomTotal) * 100.0 : 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "QueryTopBottom failed for card {CardId}", summary.CardId);
            summary.DataQualityNotes.Add("تعذر تحميل الفئات الأعلى والأدنى.");
        }
    }

    // ── Utility helpers ────────────────────────────────────────────

    /// <returns>The previous date range of equal length, ending just before <paramref name="dateFrom"/>.</returns>
    private static (DateTime prevFrom, DateTime prevTo) PreviousRange(
        DateTime dateFrom, DateTime dateTo, (string, int Months, int Years) dc)
    {
        DateTime prevTo   = dateFrom.AddDays(-1);
        DateTime prevFrom = dc.Years > 0
            ? prevTo.AddYears(-dc.Years).AddDays(1)
            : prevTo.AddMonths(-dc.Months).AddDays(1);
        return (prevFrom, prevTo);
    }

    private static string NormaliseAggregation(string? agg)
    {
        if (string.IsNullOrWhiteSpace(agg)) return "Sum";
        return agg;
    }

    private static bool IsNone(string agg) =>
        agg.Equals("None", StringComparison.OrdinalIgnoreCase);

    private static string SqlAggFunction(string agg) => agg.ToUpperInvariant() switch
    {
        "SUM"   => "SUM",
        "COUNT" => "COUNT",
        "AVG"   => "AVG",
        "MIN"   => "MIN",
        "MAX"   => "MAX",
        _       => "SUM",
    };
}
