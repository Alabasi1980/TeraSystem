using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Builds structured data summaries for Table-type dashboard cards.
/// Highlights row count, column patterns, sample rows, numeric summaries,
/// and data quality signals.
/// All user-supplied values are parameterized; column identifiers are
/// validated before interpolation.
/// </summary>
public class TableSummaryBuilder : ICardSummaryBuilder
{
    private const int SampleRowLimit = 10;
    private const int TopBottomLimit = 5;

    private readonly ReadOnlyQueryHelper _readOnly;

    // Depth level → (label, months back)
    private static readonly Dictionary<int, (string Label, int Months)> DepthConfig = new()
    {
        [1] = ("آخر 3 أشهر", 3),
        [2] = ("آخر 6 أشهر", 6),
        [3] = ("آخر سنة", 12),
        [4] = ("آخر 3 سنوات", 36),
        [5] = ("آخر 5 سنوات", 60),
        [6] = ("آخر 10 سنوات / كل البيانات المتاحة", 120),
    };

    public string ChartType => "Table";

    public TableSummaryBuilder(ReadOnlyQueryHelper readOnly)
    {
        _readOnly = readOnly;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Public entry point
    // ─────────────────────────────────────────────────────────────────

    public async Task<CardSummary> BuildSummaryAsync(
        DashboardCard card, int depthLevel, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var summary = new CardSummary
        {
            CardId = card.Id,
            Title = card.Title,
            ChartType = "Table",
            Description = string.IsNullOrWhiteSpace(card.Description) ? null : card.Description,
            AssistantPrompt = string.IsNullOrWhiteSpace(card.AssistantPrompt) ? null : card.AssistantPrompt,
            DepthLevel = depthLevel,
            DepthLabel = DepthConfig.TryGetValue(depthLevel, out var dc)
                ? dc.Label
                : "النطاق الافتراضي"
        };

        // ── Date scope ────────────────────────────────────────────
        bool hasDate = !string.IsNullOrWhiteSpace(card.DateColumn);
        summary.HasDateColumn = hasDate;
        summary.DateColumn = hasDate ? card.DateColumn : null;

        string? dateWhereClause = null;
        Dictionary<string, object>? dateParams = null;

        if (hasDate)
        {
            var dateCol = ValidateIdentifier(card.DateColumn!);
            var months = DepthConfig.TryGetValue(depthLevel, out var dc2)
                ? dc2.Months
                : 3;

            summary.DateTo = DateTime.UtcNow;
            summary.DateFrom = summary.DateTo.Value.AddMonths(-months);

            dateWhereClause = $" WHERE [{dateCol}] >= @dFrom AND [{dateCol}] <= @dTo";
            dateParams = new Dictionary<string, object>
            {
                ["@dFrom"] = summary.DateFrom.Value,
                ["@dTo"] = summary.DateTo.Value
            };
        }

        // ──────────────────────────────────────────────────────────
        // 1. Total row count
        // ──────────────────────────────────────────────────────────
        ct.ThrowIfCancellationRequested();
        await QueryTotalRowCountAsync(summary, card.SqlQuery, dateWhereClause, dateParams);

        // ──────────────────────────────────────────────────────────
        // 2. Sample rows (max 10)
        // ──────────────────────────────────────────────────────────
        ct.ThrowIfCancellationRequested();
        var sampleRowsRaw = await QuerySampleRowsAsync(
            card.SqlQuery, hasDate ? card.DateColumn : null, dateWhereClause, dateParams);

        foreach (var row in sampleRowsRaw)
        {
            var converted = new Dictionary<string, object?>();
            foreach (var kvp in row)
            {
                converted[kvp.Key] = kvp.Value;
            }
            summary.SampleRows.Add(converted);
        }

        if (sampleRowsRaw.Count == 0)
        {
            summary.DataQualityNotes.Add("لا توجد بيانات عينة متاحة للتحليل.");
            return summary;
        }

        // ──────────────────────────────────────────────────────────
        // 3. Detect numeric columns and compute summaries
        // ──────────────────────────────────────────────────────────
        ct.ThrowIfCancellationRequested();
        var numericColumns = DetectNumericColumns(sampleRowsRaw);
        var columnNames = sampleRowsRaw[0].Keys.ToList();

        foreach (var col in columnNames)
        {
            if (!numericColumns.Contains(col))
                continue;

            var safeCol = ValidateIdentifier(col);

            ct.ThrowIfCancellationRequested();
            await QueryNumericSummariesAsync(
                summary, card.SqlQuery, safeCol, col, dateWhereClause, dateParams);
        }

        // ──────────────────────────────────────────────────────────
        // 4. Detect NULLs in sample → quality note
        // ──────────────────────────────────────────────────────────
        DetectNullColumns(summary, sampleRowsRaw, columnNames);

        // ──────────────────────────────────────────────────────────
        // 5. CategoryColumn: top/bottom 5
        // ──────────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(card.CategoryColumn))
        {
            var safeCatCol = ValidateIdentifier(card.CategoryColumn!);
            ct.ThrowIfCancellationRequested();
            await QueryTopBottomAsync(
                summary, card.SqlQuery, safeCatCol, dateWhereClause, dateParams);
        }

        // ──────────────────────────────────────────────────────────
        // 6. DateColumn: series data (monthly row count)
        // ──────────────────────────────────────────────────────────
        if (hasDate)
        {
            var dateCol = ValidateIdentifier(card.DateColumn!);
            ct.ThrowIfCancellationRequested();
            await QuerySeriesDataAsync(
                summary, card.SqlQuery, dateCol, dateWhereClause!, dateParams!);
        }

        return summary;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Step 1 — Total row count
    // ─────────────────────────────────────────────────────────────────

    private async Task QueryTotalRowCountAsync(
        CardSummary summary,
        string sqlQuery,
        string? dateWhere,
        Dictionary<string, object>? dateParams)
    {
        var sql = $"SELECT COUNT(*) AS cnt FROM ({sqlQuery}) sub{dateWhere ?? ""}";
        var rows = await _readOnly.QueryAsync(sql, dateParams);

        if (rows.Count > 0 && rows[0].TryGetValue("cnt", out var raw) && raw is not null)
        {
            summary.TotalRowCount = Convert.ToInt32(raw);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Step 2 — Sample rows
    // ─────────────────────────────────────────────────────────────────

    private async Task<List<Dictionary<string, object>>> QuerySampleRowsAsync(
        string sqlQuery,
        string? dateCol,
        string? dateWhere,
        Dictionary<string, object>? dateParams)
    {
        string sql;
        if (dateCol is not null)
        {
            var safeDateCol = ValidateIdentifier(dateCol);
            sql = $"SELECT TOP {SampleRowLimit} * FROM ({sqlQuery}) sub{dateWhere} " +
                  $"ORDER BY [{safeDateCol}] DESC";
        }
        else
        {
            sql = $"SELECT TOP {SampleRowLimit} * FROM ({sqlQuery}) sub";
        }

        return await _readOnly.QueryAsync(sql, dateParams);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Step 3 — Numeric column summaries (SUM/AVG/MIN/MAX)
    // ─────────────────────────────────────────────────────────────────

    private async Task QueryNumericSummariesAsync(
        CardSummary summary,
        string sqlQuery,
        string safeCol,
        string originalColName,
        string? dateWhere,
        Dictionary<string, object>? dateParams)
    {
        var sql = $"SELECT SUM([{safeCol}]) AS sum_val, " +
                  $"AVG(CAST([{safeCol}] AS float)) AS avg_val, " +
                  $"MIN([{safeCol}]) AS min_val, " +
                  $"MAX([{safeCol}]) AS max_val " +
                  $"FROM ({sqlQuery}) sub{dateWhere ?? ""}";

        var rows = await _readOnly.QueryAsync(sql, dateParams);

        if (rows.Count == 0)
            return;

        var row = rows[0];

        double sum = TryGetDouble(row, "sum_val");
        double avg = TryGetDouble(row, "avg_val");
        double min = TryGetDouble(row, "min_val");
        double max = TryGetDouble(row, "max_val");

        var colSummary = new NumericColumnSummary
        {
            Sum = sum,
            Average = avg,
            Min = min,
            Max = max
        };

        summary.ColumnSummaries[originalColName] = colSummary;

        // Quality note: MAX > 10x MIN
        if (min > 0 && max > min * 10)
        {
            summary.DataQualityNotes.Add(
                $"تفاوت كبير في القيم للعمود '{originalColName}': " +
                $"الحد الأدنى {min:F2} والحد الأقصى {max:F2}");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Step 4 — NULL detection in sample
    // ─────────────────────────────────────────────────────────────────

    private static void DetectNullColumns(
        CardSummary summary,
        List<Dictionary<string, object>> sampleRows,
        List<string> columnNames)
    {
        foreach (var col in columnNames)
        {
            foreach (var row in sampleRows)
            {
                if (!row.TryGetValue(col, out var val) || val is null)
                {
                    summary.DataQualityNotes.Add(
                        $"العمود '{col}' يحتوي على قيم فارغة (NULL) في العينة.");
                    break;
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Step 5 — CategoryColumn top/bottom 5
    // ─────────────────────────────────────────────────────────────────

    private async Task QueryTopBottomAsync(
        CardSummary summary,
        string sqlQuery,
        string safeCatCol,
        string? dateWhere,
        Dictionary<string, object>? dateParams)
    {
        // ── Top 5 ────────────────────────────────────────────────
        var topSql = $"SELECT TOP {TopBottomLimit} [{safeCatCol}] AS name, " +
                     $"CAST(COUNT(*) AS float) AS val " +
                     $"FROM ({sqlQuery}) sub{dateWhere ?? ""} " +
                     $"GROUP BY [{safeCatCol}] " +
                     $"ORDER BY val DESC";

        var topRows = await _readOnly.QueryAsync(topSql, dateParams);
        double topTotal = 0;

        foreach (var row in topRows)
        {
            if (row.TryGetValue("name", out var n)
                && row.TryGetValue("val", out var v)
                && v is not null)
            {
                double d = Convert.ToDouble(v);
                topTotal += d;
                summary.TopItems.Add(new CategoryItem
                {
                    Name = n?.ToString() ?? string.Empty,
                    Value = d,
                });
            }
        }

        foreach (var item in summary.TopItems)
            item.Percent = topTotal > 0 ? Math.Round((item.Value / topTotal) * 100, 1) : 0;

        // ── Bottom 5 ────────────────────────────────────────────
        var bottomSql = $"SELECT TOP {TopBottomLimit} [{safeCatCol}] AS name, " +
                        $"CAST(COUNT(*) AS float) AS val " +
                        $"FROM ({sqlQuery}) sub{dateWhere ?? ""} " +
                        $"GROUP BY [{safeCatCol}] " +
                        $"ORDER BY val ASC";

        var bottomRows = await _readOnly.QueryAsync(bottomSql, dateParams);
        double bottomTotal = 0;

        foreach (var row in bottomRows)
        {
            if (row.TryGetValue("name", out var n)
                && row.TryGetValue("val", out var v)
                && v is not null)
            {
                double d = Convert.ToDouble(v);
                bottomTotal += d;
                summary.BottomItems.Add(new CategoryItem
                {
                    Name = n?.ToString() ?? string.Empty,
                    Value = d,
                });
            }
        }

        foreach (var item in summary.BottomItems)
            item.Percent = bottomTotal > 0 ? Math.Round((item.Value / bottomTotal) * 100, 1) : 0;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Step 6 — Series data (monthly row count)
    // ─────────────────────────────────────────────────────────────────

    private async Task QuerySeriesDataAsync(
        CardSummary summary,
        string sqlQuery,
        string safeDateCol,
        string dateWhere,
        Dictionary<string, object> dateParams)
    {
        var sql = $"SELECT FORMAT([{safeDateCol}], 'yyyy-MM') AS period, " +
                  $"CAST(COUNT(*) AS float) AS val " +
                  $"FROM ({sqlQuery}) sub{dateWhere} " +
                  $"GROUP BY FORMAT([{safeDateCol}], 'yyyy-MM') " +
                  $"ORDER BY period";

        var rows = await _readOnly.QueryAsync(sql, dateParams);

        foreach (var row in rows)
        {
            if (row.TryGetValue("period", out var p)
                && row.TryGetValue("val", out var v)
                && v is not null)
            {
                summary.SeriesData.Add(new SeriesPoint
                {
                    Period = p?.ToString() ?? string.Empty,
                    Value = Convert.ToDouble(v)
                });
            }
        }

        if (summary.SeriesData.Count == 0)
        {
            summary.DataQualityNotes.Add("لا توجد بيانات متسلسلة زمنياً ضمن النطاق المحدد.");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Detects which columns contain numeric values by inspecting sample rows.
    /// A column is considered numeric if at least one non-null value is a
    /// numeric CLR type.
    /// </summary>
    private static HashSet<string> DetectNumericColumns(
        List<Dictionary<string, object>> sampleRows)
    {
        var numericSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (sampleRows.Count == 0)
            return numericSet;

        var columnNames = sampleRows[0].Keys.ToList();

        var numericTypes = new HashSet<Type>
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };

        foreach (var col in columnNames)
        {
            foreach (var row in sampleRows)
            {
                if (row.TryGetValue(col, out var val)
                    && val is not null
                    && numericTypes.Contains(val.GetType()))
                {
                    numericSet.Add(col);
                    break;
                }
            }
        }

        return numericSet;
    }

    /// <summary>
    /// Safely extracts a double value from a dictionary row.
    /// Returns 0 if the key is missing, the value is null, or conversion fails.
    /// </summary>
    private static double TryGetDouble(Dictionary<string, object> row, string key)
    {
        if (row.TryGetValue(key, out var val) && val is not null)
        {
            try { return Convert.ToDouble(val); }
            catch { return 0; }
        }
        return 0;
    }

    /// <summary>
    /// Validates that a column identifier is safe for SQL interpolation.
    /// Column names come from admin-configured card metadata and are trusted
    /// at that level, but we still guard against injection through
    /// compromised config values.
    /// </summary>
    private static string ValidateIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Column identifier cannot be empty.", nameof(identifier));

        // Reject common SQL injection characters
        if (identifier.Contains(';') ||
            identifier.Contains("--") ||
            identifier.Contains("/*") ||
            identifier.Contains("*/") ||
            identifier.Contains('\'') ||
            identifier.Contains('"') ||
            identifier.Contains("xp_") ||
            identifier.IndexOf("EXEC ", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            throw new ArgumentException(
                $"Column identifier contains suspicious characters: '{identifier}'", nameof(identifier));
        }

        return identifier;
    }
}
