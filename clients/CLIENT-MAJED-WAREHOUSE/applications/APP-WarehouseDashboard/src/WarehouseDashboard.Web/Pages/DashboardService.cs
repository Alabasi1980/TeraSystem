using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages;

/// <summary>
/// Approved DashboardService pattern (06_DATA_MODEL_PREPARATION.md §5.4):
/// <list type="bullet">
///   <item><description><b>Config</b> is read via EF Core (<see cref="WarehouseDashboardDbContext"/>).</description></item>
///   <item><description><b>Data</b> is read via ADO.NET (<see cref="Microsoft.Data.SqlClient"/>) by executing the
///     admin-configured <see cref="DashboardCard.SqlQuery"/> against SQL Server.</description></item>
/// </list>
/// Every card query is wrapped so a failure or empty result is reported per card
/// (status = error / empty) and never throws out of the request.
/// </summary>
public class DashboardService
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly IConfiguration _config;

    public DashboardService(WarehouseDashboardDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>Active cards ordered for layout (GridPositionY, then GridPositionX).</summary>
    public Task<List<DashboardCard>> GetActiveCardsAsync(CancellationToken ct = default)
    {
        return _db.DashboardCards
            .Where(c => c.IsActive)
            .OrderBy(c => c.GridPositionY)
            .ThenBy(c => c.GridPositionX)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Looks up a card by id and executes its data query. Returns an error result if the card
    /// is not found. This is the single-entry point for the API layer (Card.cshtml.cs).
    /// </summary>
    public async Task<CardDataResult> GetCardDataByIdAsync(int cardId, CancellationToken ct = default)
    {
        var card = await _db.DashboardCards.FindAsync(new object[] { cardId }, ct);
        if (card is null)
        {
            return new CardDataResult
            {
                CardId = cardId,
                Status = "error",
                ErrorMessage = "البطاقة غير موجودة أو غير نشطة."
            };
        }

        return await GetCardDataAsync(card, ct);
    }

    /// <summary>
    /// Executes a single card's data query via ADO.NET and returns a resilient
    /// <see cref="CardDataResult"/>. Exceptions are caught and surfaced as
    /// <c>Status = "error"</c> with a secret-free message.
    /// </summary>
    public async Task<CardDataResult> GetCardDataAsync(DashboardCard card, CancellationToken ct = default)
    {
        var result = new CardDataResult
        {
            CardId = card.Id,
            ChartType = card.ChartType,
            Title = card.Title,
            Status = "error"
        };

        try
        {
            var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
            var connString = ConnectionStringHelper.Resolve(connTemplate);

            if (string.IsNullOrWhiteSpace(connString))
            {
                result.ErrorMessage = "لم يتم ضبط سلسلة الاتصال بقاعدة البيانات (SQL_PASSWORD).";
                return result;
            }

            var sql = BuildSql(card);

            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 60 };
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var colCount = reader.FieldCount;
            var columns = new List<string>(colCount);
            for (var i = 0; i < colCount; i++)
            {
                columns.Add(reader.GetName(i) is { Length: > 0 } name ? name : $"Column{i + 1}");
            }
            result.Columns = columns;

            // Cap payloads so a single heavy query can't break the page.
            var rowLimit = card.ChartType.Equals("Table", StringComparison.OrdinalIgnoreCase) ? 500 : 200;
            var rows = new List<Dictionary<string, object?>>(rowLimit);
            var count = 0;
            while (count < rowLimit && await reader.ReadAsync(ct))
            {
                var row = new Dictionary<string, object?>(colCount, StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < colCount; i++)
                {
                    row[columns[i]] = DataHelper.ConvertCell(reader.GetValue(i));
                }
                rows.Add(row);
                count++;
            }
            result.Rows = rows;

            if (rows.Count == 0)
            {
                result.Status = "empty";
                return result;
            }

            // Use selected ValueColumn when available (KPI4-005); fallback to first cell.
            if (!string.IsNullOrEmpty(card.ValueColumn))
            {
                var valueCol = card.ValueColumn.Trim('[', ']').Trim();
                if (rows[0].TryGetValue(valueCol, out var val))
                {
                    result.KpiValue = val;
                }
                else
                {
                    result.KpiValue = rows[0].Values.FirstOrDefault();
                }
            }
            else
            {
                result.KpiValue = rows[0].Values.FirstOrDefault();
            }

            // Advanced KPI: Execute additional queries if needed
            if (card.ChartType.Equals("KPI", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(card.KpiMode)
                && card.KpiMode != "simple")
            {
                result.KpiMode = card.KpiMode;

                // Extract main KPI value from the base query result
                if (rows.Count > 0 && !string.IsNullOrEmpty(card.ValueColumn))
                {
                    var valueCol = card.ValueColumn.Trim('[', ']').Trim();
                    if (rows[0].TryGetValue(valueCol, out var mainVal))
                    {
                        result.KpiMainValue = mainVal;
                    }
                    else
                    {
                        // Fallback: use first numeric value
                        result.KpiMainValue = rows[0].Values.FirstOrDefault();
                    }
                }

                // Build and execute additional KPI queries
                var kpiQueries = KpiQueryBuilder.Build(card);

                // Execute change query
                if (kpiQueries.ChangeSql != null)
                {
                    try
                    {
                        var prevValue = await ExecuteScalarQueryAsync(kpiQueries.ChangeSql, ct);
                        if (prevValue != null && prevValue != DBNull.Value)
                        {
                            var current = Convert.ToDouble(result.KpiMainValue ?? 0);
                            var previous = Convert.ToDouble(prevValue);

                            if (previous != 0)
                            {
                                var changePercent = ((current - previous) / Math.Abs(previous)) * 100;
                                result.KpiChangePercent = Math.Round((decimal)changePercent, 1);
                                result.KpiChangeDirection = changePercent > 0 ? "up" : changePercent < 0 ? "down" : "flat";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail the whole card
                        result.ErrorMessage = $"Change query failed: {DataHelper.Sanitize(ex.Message)}";
                    }
                }

                // Execute sparkline query
                if (kpiQueries.SparklineSql != null)
                {
                    try
                    {
                        result.KpiSparklineData = await ExecuteQueryAsync(kpiQueries.SparklineSql, ct);
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage = $"Sparkline query failed: {DataHelper.Sanitize(ex.Message)}";
                    }
                }

                // Execute grand total query
                if (kpiQueries.GrandTotalSql != null)
                {
                    try
                    {
                        var grandTotal = await ExecuteScalarQueryAsync(kpiQueries.GrandTotalSql, ct);
                        result.KpiGrandTotal = grandTotal;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage = $"Grand total query failed: {DataHelper.Sanitize(ex.Message)}";
                    }
                }
            }

            result.Status = "success";
        }
        catch (Exception ex)
        {
            result.Status = "error";
            result.ErrorMessage = DataHelper.Sanitize(ex.Message);
        }

        return result;
    }

    /// <summary>
    /// Executes a preview query with a small row limit for the Card Builder live preview.
    /// Reuses the same ADO.NET execution pattern as <see cref="GetCardDataAsync"/>.
    /// </summary>
    /// <param name="sql">The SQL query to execute (already resolved for View vs Query).</param>
    /// <param name="chartType">Chart type to determine row limit.</param>
    /// <param name="rowLimit">Maximum rows to return (default 10 for preview).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Preview result with columns, rows, and status.</returns>
    public async Task<CardDataResult> GetPreviewAsync(string sql, string chartType, int rowLimit = 10, CancellationToken ct = default)
    {
        var result = new CardDataResult
        {
            ChartType = chartType,
            Status = "error"
        };

        try
        {
            var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
            var connString = ConnectionStringHelper.Resolve(connTemplate);

            if (string.IsNullOrWhiteSpace(connString))
            {
                result.ErrorMessage = "لم يتم ضبط سلسلة الاتصال بقاعدة البيانات (SQL_PASSWORD).";
                return result;
            }

            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 30 };
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var colCount = reader.FieldCount;
            var columns = new List<string>(colCount);
            for (var i = 0; i < colCount; i++)
            {
                columns.Add(reader.GetName(i) is { Length: > 0 } name ? name : $"Column{i + 1}");
            }
            result.Columns = columns;

            var rows = new List<Dictionary<string, object?>>(rowLimit);
            var count = 0;
            while (count < rowLimit && await reader.ReadAsync(ct))
            {
                var row = new Dictionary<string, object?>(colCount, StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < colCount; i++)
                {
                    row[columns[i]] = DataHelper.ConvertCell(reader.GetValue(i));
                }
                rows.Add(row);
                count++;
            }
            result.Rows = rows;

            if (rows.Count == 0)
            {
                result.Status = "empty";
                return result;
            }

            result.KpiValue = rows[0].Values.FirstOrDefault();
            result.Status = "success";
        }
        catch (Exception ex)
        {
            result.Status = "error";
            result.ErrorMessage = DataHelper.Sanitize(ex.Message);
        }

        return result;
    }

    /// <summary>
    /// Resolves the actual SQL to run:
    /// <list type="bullet">
    ///   <item><description><b>View</b> → <c>SELECT * FROM [ViewName]</c>.</description></item>
    ///   <item><description><b>SQL Query</b> → run the configured query verbatim.</description></item>
    /// </list>
    /// The SQL is admin-controlled configuration (read-only dashboard usage).
    /// </summary>
    private static string BuildSql(DashboardCard card)
    {
        string baseSql;
        if (card.DataSourceType.Equals("View", StringComparison.OrdinalIgnoreCase))
        {
            var viewName = card.SqlQuery.Trim().TrimEnd(';').Trim();
            var safe = viewName.StartsWith("[", StringComparison.Ordinal) ? viewName : $"[{viewName}]";
            baseSql = $"SELECT * FROM {safe}";
        }
        else
        {
            baseSql = card.SqlQuery;
        }

        // KPI aggregation: wrap base query when AggregationType is not "None"
        if (card.ChartType.Equals("KPI", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(card.AggregationType)
            && card.AggregationType != "None"
            && !string.IsNullOrEmpty(card.ValueColumn))
        {
            var col = card.ValueColumn.Trim('[', ']').Trim();
            var aggFunc = card.AggregationType.ToUpperInvariant(); // SUM, COUNT, AVG, MIN, MAX
            return $"SELECT {aggFunc}([{col}]) AS [{col}] FROM ({baseSql.TrimEnd(';')}) AS _agg_src";
        }

        return baseSql;
    }

    /// <summary>
    /// Executes a SQL query and returns the first column of the first row (scalar value).
    /// Used by KPI queries that return a single aggregate value.
    /// </summary>
    private async Task<object?> ExecuteScalarQueryAsync(string sql, CancellationToken ct)
    {
        var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
        var connString = ConnectionStringHelper.Resolve(connTemplate);

        if (string.IsNullOrWhiteSpace(connString))
            return null;

        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 30 };
        var result = await cmd.ExecuteScalarAsync(ct);
        return result;
    }

    /// <summary>
    /// Executes a SQL query and returns all rows as a list of dictionaries.
    /// Used by KPI sparkline queries that return multiple rows.
    /// </summary>
    private async Task<List<Dictionary<string, object?>>> ExecuteQueryAsync(string sql, CancellationToken ct)
    {
        var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
        var connString = ConnectionStringHelper.Resolve(connTemplate);

        if (string.IsNullOrWhiteSpace(connString))
            return new List<Dictionary<string, object?>>();

        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 30 };
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var colCount = reader.FieldCount;
        var columns = new List<string>(colCount);
        for (var i = 0; i < colCount; i++)
        {
            columns.Add(reader.GetName(i) is { Length: > 0 } name ? name : $"Column{i + 1}");
        }

        var rows = new List<Dictionary<string, object?>>();
        while (await reader.ReadAsync(ct))
        {
            var row = new Dictionary<string, object?>(colCount, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < colCount; i++)
            {
                row[columns[i]] = DataHelper.ConvertCell(reader.GetValue(i));
            }
            rows.Add(row);
        }

        return rows;
    }
}
