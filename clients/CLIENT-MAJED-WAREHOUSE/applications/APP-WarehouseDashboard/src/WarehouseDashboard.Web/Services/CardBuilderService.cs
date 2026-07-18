using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models;
using WarehouseDashboard.Web.Pages;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Service for the Card Builder wizard.
/// Encapsulates logic for building DashboardCard from wizard data,
/// generating live previews, listing available Oracle tables, managing templates,
/// and cloning existing cards.
/// </summary>
public class CardBuilderService
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<CardBuilderService> _logger;
    private readonly DashboardService _dashboardService;

    // Predefined templates for the builder wizard Step 1.
    private static readonly IReadOnlyList<CardTemplate> _templates = new[]
    {
        new CardTemplate
        {
            Id = "total-stock",
            Name = "إجمالي المخزون",
            Description = "بطاقة KPI تعرض إجمالي كمية المخزون من جدول stg_WarehouseStock",
            ChartType = "KPI",
            DataSourceType = "SQL Query",
            SqlQueryTemplate = "SELECT COUNT(*) AS TotalItems FROM [{TableName}]",
            DefaultGridWidth = 3,
            DefaultGridHeight = 2,
            DefaultRefreshInterval = 300,
            RequiredOracleSource = "WarehouseStock",
            SuggestedDrillDowns = new List<CardDrillDownInput>
            {
                new() { Level = 1, DisplayName = "تفاصيل الأصناف", DrillDownQuery = "SELECT ItemCode, ItemName, Quantity FROM [{TableName}] WHERE ItemCode = @p0", TargetChartType = "Table" }
            }
        },
        new CardTemplate
        {
            Id = "sales-trend",
            Name = "اتجاه المبيعات",
            Description = "رسم بياني عمودي للمبيعات الشهرية من جدول stg_Sales",
            ChartType = "Bar",
            DataSourceType = "SQL Query",
            SqlQueryTemplate = "SELECT FORMAT(SaleDate, 'yyyy-MM') AS Month, SUM(Amount) AS TotalSales FROM [{TableName}] GROUP BY FORMAT(SaleDate, 'yyyy-MM') ORDER BY Month",
            DefaultGridWidth = 6,
            DefaultGridHeight = 4,
            DefaultRefreshInterval = 3600,
            RequiredOracleSource = "Sales",
            SuggestedDrillDowns = new List<CardDrillDownInput>
            {
                new() { Level = 1, DisplayName = "تفاصيل المبيعات", DrillDownQuery = "SELECT SaleDate, ItemCode, Amount FROM [{TableName}] WHERE FORMAT(SaleDate, 'yyyy-MM') = @p0 ORDER BY SaleDate", TargetChartType = "Table" }
            }
        },
        new CardTemplate
        {
            Id = "items-distribution",
            Name = "توزيع الأصناف",
            Description = "رسم بياني دائري يوضح توزيع الأصناف حسب الفئة من جدول stg_Items",
            ChartType = "Pie",
            DataSourceType = "SQL Query",
            SqlQueryTemplate = "SELECT Category, COUNT(*) AS ItemCount FROM [{TableName}] GROUP BY Category ORDER BY ItemCount DESC",
            DefaultGridWidth = 4,
            DefaultGridHeight = 4,
            DefaultRefreshInterval = 3600,
            RequiredOracleSource = "Items",
            SuggestedDrillDowns = new List<CardDrillDownInput>
            {
                new() { Level = 1, DisplayName = "أصناف الفئة", DrillDownQuery = "SELECT ItemCode, ItemName, Category FROM [{TableName}] WHERE Category = @p0", TargetChartType = "Table" }
            }
        },
        new CardTemplate
        {
            Id = "stock-movement",
            Name = "حركة المخزون",
            Description = "جدول يعرض أحدث حركات المخزون من جدول stg_StockMovement",
            ChartType = "Table",
            DataSourceType = "SQL Query",
            SqlQueryTemplate = "SELECT TOP 50 MovementDate, ItemCode, MovementType, Quantity, ReferenceNo FROM [{TableName}] ORDER BY MovementDate DESC",
            DefaultGridWidth = 12,
            DefaultGridHeight = 6,
            DefaultRefreshInterval = 60,
            RequiredOracleSource = "StockMovement",
            SuggestedDrillDowns = new List<CardDrillDownInput>
            {
                new() { Level = 1, DisplayName = "تفاصيل الحركة", DrillDownQuery = "SELECT * FROM [{TableName}] WHERE ReferenceNo = @p0", TargetChartType = "Table" }
            }
        },
        new CardTemplate
        {
            Id = "low-stock-alert",
            Name = "تنبيه مخزون منخفض",
            Description = "بطاقة KPI تعرض عدد الأصناف التي وصلت للحد الأدنى من جدول stg_WarehouseStock",
            ChartType = "KPI",
            DataSourceType = "SQL Query",
            SqlQueryTemplate = "SELECT COUNT(*) AS LowStockCount FROM [{TableName}] WHERE Quantity <= MinQuantity AND MinQuantity > 0",
            DefaultGridWidth = 3,
            DefaultGridHeight = 2,
            DefaultRefreshInterval = 300,
            RequiredOracleSource = "WarehouseStock",
            SuggestedDrillDowns = new List<CardDrillDownInput>
            {
                new() { Level = 1, DisplayName = "الأصناف منخفضة المخزون", DrillDownQuery = "SELECT ItemCode, ItemName, Quantity, MinQuantity FROM [{TableName}] WHERE Quantity <= MinQuantity AND MinQuantity > 0", TargetChartType = "Table" }
            }
        },
        new CardTemplate
        {
            Id = "top-selling-items",
            Name = "الأكثر مبيعاً",
            Description = "رسم بياني عمودي لأعلى 10 أصناف مبيعاً من جدول stg_Sales",
            ChartType = "Bar",
            DataSourceType = "SQL Query",
            SqlQueryTemplate = "SELECT TOP 10 ItemCode, SUM(Quantity) AS TotalQty FROM [{TableName}] GROUP BY ItemCode ORDER BY TotalQty DESC",
            DefaultGridWidth = 6,
            DefaultGridHeight = 4,
            DefaultRefreshInterval = 3600,
            RequiredOracleSource = "Sales",
            SuggestedDrillDowns = new List<CardDrillDownInput>
            {
                new() { Level = 1, DisplayName = "مبيعات الصنف", DrillDownQuery = "SELECT SaleDate, Quantity, Amount FROM [{TableName}] WHERE ItemCode = @p0 ORDER BY SaleDate DESC", TargetChartType = "Line" }
            }
        }
    };

    public CardBuilderService(
        WarehouseDashboardDbContext db,
        IConfiguration config,
        ILogger<CardBuilderService> logger,
        DashboardService dashboardService)
    {
        _db = db;
        _config = config;
        _logger = logger;
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Builds a <see cref="DashboardCard"/> entity from the wizard request data.
    /// Does NOT persist — caller decides when to save.
    /// </summary>
        public DashboardCard BuildCard(CardBuilderRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var card = new DashboardCard
            {
                Title = request.Title.Trim(),
                ChartType = request.ChartType,
                DataSourceType = request.DataSourceType,
                SqlQuery = request.SqlQuery.Trim(),
                GridPositionX = request.GridPositionX,
                GridPositionY = request.GridPositionY,
                GridWidth = request.GridWidth,
                GridHeight = request.GridHeight,
                RefreshInterval = request.RefreshInterval,
                IsActive = request.IsActive,
                // Advanced KPI
                ValueColumn = request.ValueColumn ?? "",
                DateColumn = request.DateColumn ?? "",
                CategoryColumn = request.CategoryColumn ?? "",
                KpiMode = request.KpiMode ?? "simple",
                ShowChange = request.ShowChange,
                ChangeSource = request.ChangeSource ?? "previousPeriod",
                ShowSparkline = request.ShowSparkline,
                SparklineMonths = request.SparklineMonths,
                ShowGrandTotal = request.ShowGrandTotal,
                GrandTotalSource = request.GrandTotalSource ?? "sameTable",
                DateFilterMode = request.DateFilterMode ?? "dashboard",
                FixedStartDate = request.FixedStartDate ?? "",
                FixedEndDate = request.FixedEndDate ?? "",
                RelativeDays = request.RelativeDays > 0 ? request.RelativeDays : 30
            };

            return card;
        }

    /// <summary>
    /// Generates a live preview for the builder wizard Step 3/Preview pane.
    /// Executes the provided query (limited rows) and builds a chart config.
    /// </summary>
    public async Task<CardPreviewResult> GetPreviewAsync(CardPreviewRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = new CardPreviewResult
        {
            ChartType = request.ChartType,
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

            var sql = BuildPreviewSql(request);
            var rowLimit = request.PreviewRowLimit > 0 ? request.PreviewRowLimit : 10;

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
            result.SampleData = rows;

            if (rows.Count == 0)
            {
                result.Status = "empty";
                sw.Stop();
                result.ExecutionTimeMs = sw.ElapsedMilliseconds;
                return result;
            }

            result.KpiValue = rows[0].Values.FirstOrDefault();
            result.ChartConfig = BuildChartConfig(request.ChartType, columns, rows);
            result.Status = "success";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Preview query failed for chart type {ChartType}", request.ChartType);
            result.Status = "error";
            result.ErrorMessage = DataHelper.Sanitize(ex.Message);
        }
        finally
        {
            sw.Stop();
            result.ExecutionTimeMs = sw.ElapsedMilliseconds;
        }

        return result;
    }

    /// <summary>
    /// Returns all active table mappings for the Step 2 Oracle Tables dropdown.
    /// </summary>
    public async Task<List<TableMappingConfig>> GetAvailableTablesAsync(CancellationToken ct = default)
    {
        return await _db.TableMappings
            .Include(t => t.ColumnMappings)
            .Where(t => t.IsActive)
            .OrderBy(t => t.OracleSource)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Gets a predefined template by its ID.
    /// Returns null if not found.
    /// </summary>
    public CardTemplate? GetTemplate(string templateId)
    {
        if (string.IsNullOrWhiteSpace(templateId))
        {
            return null;
        }

        return _templates.FirstOrDefault(t => t.Id.Equals(templateId, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all predefined templates for the Step 1 template gallery.
    /// </summary>
    public IReadOnlyList<CardTemplate> GetAllTemplates() => _templates;

    /// <summary>
    /// Creates a builder request pre-filled from an existing card (for Clone action).
    /// </summary>
    public async Task<CardBuilderRequest?> CloneFromCardAsync(int cardId, CancellationToken ct = default)
    {
        var card = await _db.DashboardCards
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cardId, ct);

        if (card is null)
        {
            return null;
        }

        var drillDowns = await _db.CardDrillDownLevels
            .AsNoTracking()
            .Where(d => d.ParentCardId == cardId)
            .OrderBy(d => d.Level)
            .Select(d => new CardDrillDownInput
            {
                Level = d.Level,
                DisplayName = d.DisplayName,
                DrillDownQuery = d.DrillDownQuery,
                TargetChartType = d.TargetChartType
            })
            .ToListAsync(ct);

            return new CardBuilderRequest
            {
                Title = card.Title + " (نسخة)",
                ChartType = card.ChartType,
                DataSourceType = card.DataSourceType,
                SqlQuery = card.SqlQuery,
                GridPositionX = card.GridPositionX,
                GridPositionY = card.GridPositionY,
                GridWidth = card.GridWidth,
                GridHeight = card.GridHeight,
                RefreshInterval = card.RefreshInterval,
                IsActive = card.IsActive,
                DrillDownLevels = drillDowns,
                // Advanced KPI
                ValueColumn = card.ValueColumn,
                DateColumn = card.DateColumn,
                CategoryColumn = card.CategoryColumn,
                KpiMode = card.KpiMode,
                ShowChange = card.ShowChange,
                ChangeSource = card.ChangeSource,
                ShowSparkline = card.ShowSparkline,
                SparklineMonths = card.SparklineMonths,
                ShowGrandTotal = card.ShowGrandTotal,
                GrandTotalSource = card.GrandTotalSource,
                DateFilterMode = card.DateFilterMode,
                FixedStartDate = card.FixedStartDate,
                FixedEndDate = card.FixedEndDate,
                RelativeDays = card.RelativeDays,
                AggregationType = card.AggregationType ?? "Sum"
            };
    }

    /// <summary>
    /// Builds the actual SQL to execute for preview, applying row limit.
    /// </summary>
    private static string BuildPreviewSql(CardPreviewRequest request)
    {
        var sql = request.SqlQuery.Trim().TrimEnd(';');

        if (request.DataSourceType.Equals("View", StringComparison.OrdinalIgnoreCase))
        {
            var safe = sql.StartsWith("[", StringComparison.Ordinal) ? sql : $"[{sql}]";
            sql = $"SELECT * FROM {safe}";
        }

        // Apply row limit for preview (SQL Server syntax)
        if (!sql.StartsWith("SELECT TOP", StringComparison.OrdinalIgnoreCase) &&
            !sql.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
        {
            sql = $"SELECT TOP ({request.PreviewRowLimit}) * FROM ({sql}) AS PreviewSub";
        }

        return sql;
    }

    /// <summary>
    /// Builds a minimal chart configuration object for the preview.
    /// Returns an anonymous object that serializes to the expected JSON shape.
    /// </summary>
    private static object BuildChartConfig(string chartType, List<string> columns, List<Dictionary<string, object?>> rows)
    {
        if (columns.Count == 0 || rows.Count == 0)
        {
            return new { };
        }

        // Determine X (category) and Y (value) columns
        // Heuristic: first non-numeric column = X, first numeric column = Y
        var xColumn = columns.FirstOrDefault(c =>
        {
            var sample = rows[0].GetValueOrDefault(c);
            return sample is string;
        }) ?? columns[0];

        var yColumn = columns.FirstOrDefault(c =>
        {
            var sample = rows[0].GetValueOrDefault(c);
            return sample is int or long or decimal or double or float;
        }) ?? (columns.Count > 1 ? columns[1] : columns[0]);

        var seriesData = rows.Select(r => new
        {
            x = r.GetValueOrDefault(xColumn)?.ToString() ?? string.Empty,
            y = Convert.ToDouble(r.GetValueOrDefault(yColumn) ?? 0)
        }).ToArray();

        return chartType switch
        {
            "Pie" => new
            {
                series = new[]
                {
                    new
                    {
                        type = "Pie",
                        dataSource = seriesData.Select(s => new { x = s.x, y = s.y }),
                        xName = "x",
                        yName = "y",
                        dataLabel = new { visible = true, name = "x" }
                    }
                },
                legendSettings = new { visible = true, position = "Bottom" },
                tooltip = new { enable = true, format = "${x}: ${y}" }
            },

            "KPI" or "Gauge" => new
            {
                series = new[]
                {
                    new
                    {
                        type = chartType == "KPI" ? "Column" : "LinearGauge",
                        dataSource = seriesData,
                        xName = "x",
                        yName = "y"
                    }
                }
            },

            "Table" => new
            {
                columns = columns.Select(c => new { field = c, headerText = c, width = "120" }).ToArray(),
                dataSource = rows
            },

            _ => new // Bar, Line, Area, etc.
            {
                series = new[]
                {
                    new
                    {
                        type = chartType,
                        dataSource = seriesData,
                        xName = "x",
                        yName = "y",
                        name = yColumn
                    }
                },
                primaryXAxis = new { valueType = "Category", title = xColumn },
                primaryYAxis = new { title = yColumn },
                legendSettings = new { visible = false },
                tooltip = new { enable = true, format = "${x}: ${y}" }
            }
        };
    }
}
