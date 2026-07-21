using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Builds structured data summaries for chart-type dashboard cards
/// (Bar, Line, Pie, Gauge). Uses read-only ADO.NET queries via
/// <see cref="ReadOnlyQueryHelper"/> — all user-supplied values are parameterized.
/// </summary>
public class ChartSummaryBuilder : ICardSummaryBuilder
{
    private readonly ReadOnlyQueryHelper _readOnly;

    // ------------------------------------------------------------------
    // Depth map (same convention as KpiSummaryBuilder)
    // ------------------------------------------------------------------
    private static readonly Dictionary<int, (string Label, int Months, int Years)> DepthConfig = new()
    {
        [1] = ("آخر 3 أشهر",                     3,  0),
        [2] = ("آخر 6 أشهر",                     6,  0),
        [3] = ("آخر سنة",                       12,  0),
        [4] = ("آخر 3 سنوات",                    0,  3),
        [5] = ("آخر 5 سنوات",                    0,  5),
        [6] = ("آخر 10 سنوات / كل البيانات المتاحة", 0, 10),
    };

    private const int TopBottomLimit = 5;
    private const int MaxSeriesPoints = 24;
    private const int TopCategorySeriesCount = 3;

    public string ChartType => "Bar";

    public ChartSummaryBuilder(ReadOnlyQueryHelper readOnly)
    {
        _readOnly = readOnly;
    }

    // ------------------------------------------------------------------
    // Main entry point
    // ------------------------------------------------------------------
    public async Task<CardSummary> BuildSummaryAsync(
        DashboardCard card, int depthLevel, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var summary = new CardSummary
        {
            CardId          = card.Id,
            Title           = card.Title,
            ChartType       = "Bar", // Factory primary key; aliased for Line/Pie/Gauge
            Description     = string.IsNullOrWhiteSpace(card.Description) ? null : card.Description,
            AssistantPrompt = card.AssistantPrompt,
            DepthLevel      = depthLevel,
        };

        // Depth label
        summary.DepthLabel = DepthConfig.TryGetValue(depthLevel, out var dc)
            ? dc.Label
            : $"المستوى {depthLevel}";

        // Essential validations
        string sqlQuery = card.SqlQuery;
        string valueCol = card.ValueColumn;

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

        // ── Date scope (step 2) ────────────────────────────────────
        string? dateCol = card.DateColumn;
        bool hasDate = !string.IsNullOrWhiteSpace(dateCol);
        summary.HasDateColumn = hasDate;
        summary.DateColumn = hasDate ? dateCol : null;

        DateTime today = DateTime.Today;
        DateTime? dateFrom = null;
        DateTime? dateTo = null;

        if (hasDate && DepthConfig.TryGetValue(depthLevel, out dc))
        {
            dateFrom = dc.Years > 0
                ? today.AddYears(-dc.Years)
                : today.AddMonths(-dc.Months);
            dateTo = today;

            summary.DateFrom = dateFrom;
            summary.DateTo   = dateTo;
        }
        else if (!hasDate)
        {
            summary.DataQualityNotes.Add("لا يوجد بُعد زمني");
        }

        // ── Category column ────────────────────────────────────────
        string? catCol = card.CategoryColumn;
        bool hasCategory = !string.IsNullOrWhiteSpace(catCol);

        // ── Step 3-5: Category distribution ────────────────────────
        if (hasCategory)
        {
            ct.ThrowIfCancellationRequested();
            await BuildCategoryDistributionAsync(
                summary, sqlQuery, valueCol, catCol!, dateCol, dateFrom, dateTo);
        }

        // ── Step 6: Series data (monthly aggregation) ──────────────
        if (hasDate && dateFrom.HasValue && dateTo.HasValue)
        {
            ct.ThrowIfCancellationRequested();
            await BuildSeriesDataAsync(
                summary, sqlQuery, valueCol, dateCol!, dateFrom.Value, dateTo.Value);
        }

        // ── Step 7: Top 3 categories monthly series ────────────────
        if (hasDate && hasCategory
            && dateFrom.HasValue && dateTo.HasValue
            && summary.TopItems.Count > 0)
        {
            ct.ThrowIfCancellationRequested();
            await BuildTopCategorySeriesAsync(
                summary, sqlQuery, valueCol, dateCol!, catCol!,
                dateFrom.Value, dateTo.Value);
        }

        // ── Step 8: Outlier detection ──────────────────────────────
        DetectOutliers(summary);

        // ── Step 9: Trend direction ────────────────────────────────
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

    // ------------------------------------------------------------------
    // Step 3-5: Category distribution
    // ------------------------------------------------------------------
    private async Task BuildCategoryDistributionAsync(
        CardSummary summary,
        string sqlQuery,
        string valueCol,
        string catCol,
        string? dateCol,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        try
        {
            bool hasDate = !string.IsNullOrWhiteSpace(dateCol)
                           && dateFrom.HasValue && dateTo.HasValue;

            string where = hasDate
                ? $"WHERE {dateCol} >= @dFrom AND {dateCol} <= @dTo"
                : "";

            var prms = hasDate
                ? new Dictionary<string, object>
                {
                    ["@dFrom"] = dateFrom!.Value,
                    ["@dTo"]   = dateTo!.Value,
                }
                : null;

            // Query: category + SUM(value), ordered descending
            var sql = $"""
                SELECT {catCol} AS cat,
                       SUM({valueCol}) AS val
                FROM ({sqlQuery}) sub
                {where}
                GROUP BY {catCol}
                ORDER BY val DESC
                """;

            var rows = await _readOnly.QueryAsync(sql, prms);

            if (rows.Count == 0)
            {
                summary.DataQualityNotes.Add("لا توجد بيانات فئات متاحة للبطاقة.");
                return;
            }

            // Parse all category values
            var categoryValues = new List<(string Name, double Value)>();
            double totalSum = 0;

            foreach (var row in rows)
            {
                if (row.TryGetValue("cat", out var catObj)
                    && row.TryGetValue("val", out var valObj)
                    && valObj is not null)
                {
                    string name = catObj?.ToString() ?? "";
                    double value = Convert.ToDouble(valObj);

                    if (!string.IsNullOrEmpty(name))
                    {
                        categoryValues.Add((name, value));
                        totalSum += value;
                    }
                }
            }

            if (categoryValues.Count == 0)
            {
                summary.DataQualityNotes.Add("لا توجد بيانات فئات صالحة.");
                return;
            }

            // Step 4: TotalRowCount = number of distinct categories
            summary.TotalRowCount = categoryValues.Count;

            // Step 5: CurrentValue = sum of all categories
            summary.CurrentValue = totalSum;

            // Top 5
            for (int i = 0; i < Math.Min(TopBottomLimit, categoryValues.Count); i++)
            {
                var (name, value) = categoryValues[i];
                summary.TopItems.Add(new CategoryItem
                {
                    Name    = name,
                    Value   = value,
                    Percent = totalSum > 0 ? Math.Round((value / totalSum) * 100, 2) : 0,
                });
            }

            // Bottom 5 (last N items — already ordered DESC)
            int bottomStart = Math.Max(0, categoryValues.Count - TopBottomLimit);
            double bottomTotal = 0;
            for (int i = categoryValues.Count - 1; i >= bottomStart; i--)
            {
                var (name, value) = categoryValues[i];
                bottomTotal += value;
                summary.BottomItems.Add(new CategoryItem
                {
                    Name  = name,
                    Value = value,
                });
            }
            // Calculate bottom percentages
            foreach (var item in summary.BottomItems)
            {
                item.Percent = bottomTotal > 0
                    ? Math.Round((item.Value / bottomTotal) * 100, 2)
                    : 0;
            }

            // Distribution balance: if top item > 50% of total
            if (summary.TopItems.Count > 0 && totalSum > 0)
            {
                var topPercent = summary.TopItems[0].Percent;
                if (topPercent > 50)
                {
                    summary.DataQualityNotes.Add(
                        $"توزيع غير متوازن: الفئة الأعلى ({summary.TopItems[0].Name}) تمثل {topPercent:F1}% من الإجمالي.");
                }
            }
        }
        catch (Exception)
        {
            summary.DataQualityNotes.Add("تعذر تحميل توزيع الفئات.");
        }
    }

    // ------------------------------------------------------------------
    // Step 6: Monthly series data (all categories aggregated)
    // ------------------------------------------------------------------
    private async Task BuildSeriesDataAsync(
        CardSummary summary,
        string sqlQuery,
        string valueCol,
        string dateCol,
        DateTime dateFrom,
        DateTime dateTo)
    {
        try
        {
            var sql = $"""
                SELECT FORMAT({dateCol},'yyyy-MM') AS period,
                       SUM({valueCol}) AS val
                FROM ({sqlQuery}) sub
                WHERE {dateCol} >= @sFrom AND {dateCol} <= @sTo
                GROUP BY FORMAT({dateCol},'yyyy-MM')
                ORDER BY period
                """;

            var prms = new Dictionary<string, object>
            {
                ["@sFrom"] = dateFrom,
                ["@sTo"]   = dateTo,
            };

            var rows = await _readOnly.QueryAsync(sql, prms);

            int taken = 0;
            foreach (var row in rows)
            {
                if (taken >= MaxSeriesPoints)
                    break;

                if (row.TryGetValue("period", out var pObj)
                    && row.TryGetValue("val", out var vObj)
                    && vObj is not null)
                {
                    summary.SeriesData.Add(new SeriesPoint
                    {
                        Period = pObj?.ToString() ?? string.Empty,
                        Value  = Convert.ToDouble(vObj),
                    });
                    taken++;
                }
            }

            if (taken == 0)
            {
                summary.DataQualityNotes.Add("لا توجد بيانات سلاسل زمنية ضمن النطاق المحدد.");
            }

            // Detect IsFullDataReached / HasDeeperData
            var minSql = $"SELECT MIN({dateCol}) AS minDate FROM ({sqlQuery}) sub";
            var minRows = await _readOnly.QueryAsync(minSql);

            if (minRows.Count > 0
                && minRows[0].TryGetValue("minDate", out var raw)
                && raw is DateTime minDate)
            {
                summary.IsFullDataReached = minDate >= dateFrom;
                summary.HasDeeperData     = minDate < dateFrom;
            }
        }
        catch (Exception)
        {
            summary.DataQualityNotes.Add("تعذر تحميل بيانات السلاسل الزمنية.");
        }
    }

    // ------------------------------------------------------------------
    // Step 7: Top 3 categories monthly series
    // ------------------------------------------------------------------
    private async Task BuildTopCategorySeriesAsync(
        CardSummary summary,
        string sqlQuery,
        string valueCol,
        string dateCol,
        string catCol,
        DateTime dateFrom,
        DateTime dateTo)
    {
        var topCategoryNames = summary.TopItems
            .Take(TopCategorySeriesCount)
            .Select(t => t.Name)
            .ToList();

        if (topCategoryNames.Count == 0)
            return;

        try
        {
            // Parameterize category name values
            var prms = new Dictionary<string, object>
            {
                ["@tFrom"] = dateFrom,
                ["@tTo"]   = dateTo,
            };

            var catParams = new List<string>();
            for (int i = 0; i < topCategoryNames.Count; i++)
            {
                var pName = $"@cat{i}";
                prms[pName] = topCategoryNames[i];
                catParams.Add(pName);
            }

            var catList = string.Join(", ", catParams);

            var sql = $"""
                SELECT {catCol} AS cat,
                       FORMAT({dateCol},'yyyy-MM') AS period,
                       SUM({valueCol}) AS val
                FROM ({sqlQuery}) sub
                WHERE {dateCol} >= @tFrom AND {dateCol} <= @tTo
                  AND {catCol} IN ({catList})
                GROUP BY {catCol}, FORMAT({dateCol},'yyyy-MM')
                ORDER BY cat, period
                """;

            var rows = await _readOnly.QueryAsync(sql, prms);

            // Group series by category
            var catSeries = new Dictionary<string, List<(string Period, double Value)>>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                if (row.TryGetValue("cat", out var cObj)
                    && row.TryGetValue("period", out var pObj)
                    && row.TryGetValue("val", out var vObj)
                    && vObj is not null)
                {
                    string c = cObj?.ToString() ?? "";
                    string p = pObj?.ToString() ?? "";
                    double v = Convert.ToDouble(vObj);

                    if (!string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(p))
                    {
                        if (!catSeries.ContainsKey(c))
                            catSeries[c] = new List<(string, double)>();
                        catSeries[c].Add((p, v));
                    }
                }
            }

            // Add per-category trend observations as quality notes
            foreach (var catName in topCategoryNames)
            {
                if (catSeries.TryGetValue(catName, out var points) && points.Count >= 2)
                {
                    var sorted = points.OrderBy(p => p.Period).ToList();
                    double first = sorted[0].Value;
                    double last  = sorted[^1].Value;
                    string dir = last > first ? "ارتفاع" : last < first ? "انخفاض" : "استقرار";
                    summary.DataQualityNotes.Add(
                        $"اتجاه الفئة \"{catName}\" خلال الفترة: {dir}.");
                }
            }
        }
        catch (Exception)
        {
            summary.DataQualityNotes.Add("تعذر تحميل سلاسل الفئات الفرعية.");
        }
    }

    // ------------------------------------------------------------------
    // Step 8: Outlier detection (> 2x average of Top + Bottom items)
    // ------------------------------------------------------------------
    private static void DetectOutliers(CardSummary summary)
    {
        var allItems = summary.TopItems
            .Concat(summary.BottomItems)
            .DistinctBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (allItems.Count < 2)
            return;

        double avg = allItems.Average(i => i.Value);
        if (avg <= 0)
            return;

        foreach (var item in allItems)
        {
            if (item.Value > avg * 2)
            {
                summary.DataQualityNotes.Add(
                    $"انحراف ملحوظ: الفئة \"{item.Name}\" تتجاوز ضعف المتوسط ({avg:F1}).");
            }
        }

        // Deduplicate
        summary.DataQualityNotes = summary.DataQualityNotes.Distinct().ToList();
    }
}
