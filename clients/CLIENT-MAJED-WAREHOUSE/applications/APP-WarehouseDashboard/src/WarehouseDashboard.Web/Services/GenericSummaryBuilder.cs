using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Fallback summary builder — handles unknown/unusual ChartTypes and cards
/// with no date column. Acts as the factory wildcard ("*").
/// </summary>
public class GenericSummaryBuilder : ICardSummaryBuilder
{
    private readonly ReadOnlyQueryHelper _readOnly;

    public string ChartType => "*";

    private static readonly HashSet<string> KnownChartTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Bar", "Line", "Pie", "Gauge", "KPI", "Table"
    };

    private static readonly HashSet<Type> NumericTypes = new()
    {
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
        typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(decimal)
    };

    public GenericSummaryBuilder(ReadOnlyQueryHelper readOnly)
    {
        _readOnly = readOnly;
    }

    /// <inheritdoc />
    public async Task<CardSummary> BuildSummaryAsync(
        DashboardCard card, int depthLevel, CancellationToken ct = default)
    {
        var summary = new CardSummary();

        // ── 1. Card metadata ──────────────────────────────────────────────
        summary.CardId = card.Id;
        summary.Title = card.Title;
        summary.ChartType = card.ChartType;
        summary.Description = NullIfEmpty(card.Description);
        summary.AssistantPrompt = NullIfEmpty(card.AssistantPrompt);
        summary.DepthLevel = depthLevel;
        summary.DepthLabel = GetDepthLabel(depthLevel);

        // ── 2. Date column: scope or no-date fallback ─────────────────────
        bool hasDateColumn = !string.IsNullOrWhiteSpace(card.DateColumn);
        summary.HasDateColumn = hasDateColumn;
        summary.DateColumn = hasDateColumn ? card.DateColumn : null;

        DateTime? dateFrom = null;
        DateTime? dateTo = null;
        string dateWhere = "";
        Dictionary<string, object>? dateParams = null;

        if (hasDateColumn)
        {
            dateTo = DateTime.UtcNow;
            dateFrom = GetDateFrom(depthLevel, dateTo.Value);
            summary.DateFrom = dateFrom;
            summary.DateTo = dateTo;

            dateWhere = $" WHERE [{card.DateColumn}] >= @dateFrom AND [{card.DateColumn}] <= @dateTo";
            dateParams = new Dictionary<string, object>
            {
                ["@dateFrom"] = dateFrom.Value,
                ["@dateTo"] = dateTo.Value
            };
        }
        else
        {
            summary.HasDateColumn = false;
            summary.IsFullDataReached = true;
            summary.HasDeeperData = false;
            summary.DataQualityNotes.Add("هذه البطاقة لا تحتوي على بُعد زمني واضح");
        }

        // ── 3. Quality note for unknown ChartType ─────────────────────────
        if (!KnownChartTypes.Contains(card.ChartType))
        {
            summary.DataQualityNotes.Add("نوع الرسم البياني غير معتاد");
        }

        // ── 4. COUNT(*) → TotalRowCount ───────────────────────────────────
        try
        {
            string countSql = $"SELECT COUNT(*) AS TotalCount FROM ({card.SqlQuery}) sub{dateWhere}";
            var countResult = await _readOnly.QueryAsync(countSql, dateParams);
            if (countResult.Count > 0
                && countResult[0].TryGetValue("TotalCount", out var countObj)
                && countObj is not null)
            {
                summary.TotalRowCount = Convert.ToInt32(countObj);
            }
        }
        catch (Exception)
        {
            summary.DataQualityNotes.Add("تعذر استخراج البيانات من المصدر.");
        }

        // ── 5. TOP 10 → SampleRows ────────────────────────────────────────
        // Reuse dateParams (which may be null for no-date cards).
        try
        {
            string sampleSql = $"SELECT TOP 10 * FROM ({card.SqlQuery}) sub{dateWhere}";
            var sampleRows = await _readOnly.QueryAsync(sampleSql, dateParams);

            foreach (var row in sampleRows)
            {
                summary.SampleRows.Add(row);
            }

            if (sampleRows.Count < 5)
            {
                summary.DataQualityNotes.Add("بيانات محدودة");
            }
        }
        catch (Exception)
        {
            summary.DataQualityNotes.Add("تعذر استخراج البيانات من المصدر.");
        }

        // ── 6. Numeric columns → ColumnSummaries (SUM/AVG/MIN/MAX) ────────
        // Must have at least one sample row to detect column types.
        if (summary.SampleRows.Count > 0)
        {
            var numericColumns = DetectNumericColumns(summary.SampleRows);

            if (numericColumns.Count > 0)
            {
                await BuildColumnSummariesAsync(card, summary, numericColumns, dateWhere, dateParams);
            }

            // ── 7. CurrentValue: ValueColumn if numeric, else first detected ──
            if (numericColumns.Count > 0)
            {
                string valueCol = !string.IsNullOrWhiteSpace(card.ValueColumn)
                    && numericColumns.Any(c => string.Equals(c, card.ValueColumn, StringComparison.OrdinalIgnoreCase))
                        ? card.ValueColumn
                        : numericColumns[0];

                if (summary.ColumnSummaries.TryGetValue(valueCol, out var cv))
                {
                    summary.CurrentValue = cv.Sum;
                }
            }
        }

        // ── 8. CategoryColumn → GROUP BY → TopItems (5) ──────────────────
        await BuildTopItemsAsync(card, summary, dateWhere, dateParams);

        return summary;
    }

    // ═══════════════════════════════════════════════════════════════
    //  Private helpers
    // ═══════════════════════════════════════════════════════════════

    private static string GetDepthLabel(int depthLevel) => depthLevel switch
    {
        1 => "آخر 3 أشهر",
        2 => "آخر 6 أشهر",
        3 => "آخر سنة",
        4 => "آخر 3 سنوات",
        5 => "آخر 5 سنوات",
        6 => "آخر 10 سنوات / كل البيانات المتاحة",
        _ => "النطاق الافتراضي"
    };

    private static DateTime GetDateFrom(int depthLevel, DateTime dateTo) => depthLevel switch
    {
        1 => dateTo.AddMonths(-3),
        2 => dateTo.AddMonths(-6),
        3 => dateTo.AddMonths(-12),
        4 => dateTo.AddYears(-3),
        5 => dateTo.AddYears(-5),
        6 => dateTo.AddYears(-10),
        _ => dateTo.AddMonths(-3)
    };

    private static List<string> DetectNumericColumns(List<Dictionary<string, object>> rows)
    {
        var candidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            foreach (var kvp in row)
            {
                if (kvp.Value is not null && NumericTypes.Contains(kvp.Value.GetType()))
                {
                    candidates.Add(kvp.Key);
                }
            }
        }

        // Preserve a stable order (first occurrence across all rows).
        var ordered = new List<string>();
        foreach (var row in rows)
        {
            foreach (var kvp in row)
            {
                if (candidates.Contains(kvp.Key) && !ordered.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase))
                {
                    ordered.Add(kvp.Key);
                }
            }
        }

        return ordered;
    }

    private async Task BuildColumnSummariesAsync(
        DashboardCard card,
        CardSummary summary,
        List<string> numericColumns,
        string dateWhere,
        Dictionary<string, object>? dateParams)
    {
        // Build one aggregate query for all numeric columns.
        var aggParts = new List<string>();
        foreach (var col in numericColumns)
        {
            var safe = SanitiseColumn(col);
            aggParts.Add($"SUM([{safe}]) AS [Sum_{safe}]");
            aggParts.Add($"AVG(CAST([{safe}] AS FLOAT)) AS [Avg_{safe}]");
            aggParts.Add($"MIN([{safe}]) AS [Min_{safe}]");
            aggParts.Add($"MAX([{safe}]) AS [Max_{safe}]");
        }

        try
        {
            string aggSql = $"SELECT {string.Join(", ", aggParts)} FROM ({card.SqlQuery}) sub{dateWhere}";
            var aggResult = await _readOnly.QueryAsync(aggSql, dateParams);

            if (aggResult.Count > 0)
            {
                var aggRow = aggResult[0];
                foreach (var col in numericColumns)
                {
                    var safe = SanitiseColumn(col);
                    summary.ColumnSummaries[col] = new NumericColumnSummary
                    {
                        Sum = GetDouble(aggRow, $"Sum_{safe}"),
                        Average = GetDouble(aggRow, $"Avg_{safe}"),
                        Min = GetDouble(aggRow, $"Min_{safe}"),
                        Max = GetDouble(aggRow, $"Max_{safe}")
                    };
                }
            }
        }
        catch (Exception)
        {
            summary.DataQualityNotes.Add("تعذر حساب ملخص البيانات.");
        }
    }

    private async Task BuildTopItemsAsync(
        DashboardCard card,
        CardSummary summary,
        string dateWhere,
        Dictionary<string, object>? dateParams)
    {
        if (string.IsNullOrWhiteSpace(card.CategoryColumn))
            return;

        // Determine aggregation: prefer ValueColumn, otherwise COUNT(*).
        string aggExpr;
        if (!string.IsNullOrWhiteSpace(card.ValueColumn))
        {
            aggExpr = $"SUM([{card.ValueColumn}])";
        }
        else
        {
            aggExpr = "COUNT(*)";
        }

        try
        {
            string groupSql = $@"
SELECT TOP 5 [{card.CategoryColumn}] AS CategoryName, {aggExpr} AS TotalValue
FROM ({card.SqlQuery}) sub{dateWhere}
GROUP BY [{card.CategoryColumn}]
ORDER BY TotalValue DESC";

            var groupResult = await _readOnly.QueryAsync(groupSql, dateParams);

            if (groupResult.Count == 0)
                return;

            double totalSum = 0;
            foreach (var row in groupResult)
            {
                totalSum += GetDouble(row, "TotalValue");
            }

            foreach (var row in groupResult)
            {
                double val = GetDouble(row, "TotalValue");
                summary.TopItems.Add(new CategoryItem
                {
                    Name = row.TryGetValue("CategoryName", out var nameObj) && nameObj is not null
                        ? nameObj.ToString()!
                        : "",
                    Value = val,
                    Percent = totalSum > 0 ? Math.Round(val / totalSum * 100, 1) : null
                });
            }
        }
        catch (Exception)
        {
            summary.DataQualityNotes.Add("تعذر حساب ملخص البيانات.");
        }
    }

    /// <summary>Strips existing bracket-wrapping so a fresh [ ] can be applied safely.</summary>
    private static string SanitiseColumn(string col)
    {
        var trimmed = col.Trim();
        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            trimmed = trimmed[1..^1].Trim();
        return trimmed;
    }

    private static double GetDouble(Dictionary<string, object> row, string key)
    {
        if (row.TryGetValue(key, out var val) && val is not null)
        {
            try { return Convert.ToDouble(val); }
            catch { }
        }
        return 0;
    }

    private static string? NullIfEmpty(string? s)
        => string.IsNullOrWhiteSpace(s) ? null : s;
}
