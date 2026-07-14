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

            // First cell drives KPI / Gauge cards.
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
        if (card.DataSourceType.Equals("View", StringComparison.OrdinalIgnoreCase))
        {
            var viewName = card.SqlQuery.Trim().TrimEnd(';').Trim();
            var safe = viewName.StartsWith("[", StringComparison.Ordinal) ? viewName : $"[{viewName}]";
            return $"SELECT * FROM {safe}";
        }

        return card.SqlQuery;
    }
}
