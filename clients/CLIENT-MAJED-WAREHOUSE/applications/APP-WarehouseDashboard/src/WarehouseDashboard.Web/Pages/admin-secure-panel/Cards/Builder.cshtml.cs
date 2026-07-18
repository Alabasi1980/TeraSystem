using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Services;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.admin_secure_panel.Cards
{
    /// <summary>
    /// PageModel for the Visual Card Builder Wizard.
    /// Handles: Clone mode detection, Template pre-fill, Oracle Tables API proxy, Save/Preview endpoints.
    /// </summary>
    public class BuilderModel : PageModel
    {
        private readonly CardBuilderService _cardBuilderService;
        private readonly ILogger<BuilderModel> _logger;
        private readonly WarehouseDashboardDbContext _db;

        public BuilderModel(CardBuilderService cardBuilderService, ILogger<BuilderModel> logger, WarehouseDashboardDbContext db)
        {
            _cardBuilderService = cardBuilderService;
            _logger = logger;
            _db = db;
        }

        /// <summary>
        /// Card type selected in Step 1: KPI, Bar, Line, Pie, Table, Gauge
        /// </summary>
        [BindProperty]
        [JsonPropertyName("cardType")]
        public string CardType { get; set; } = "KPI";

        /// <summary>
        /// Source type selected in Step 2: Template, SavedQuery, OracleTable, CustomSQL
        /// </summary>
        [BindProperty]
        [JsonPropertyName("sourceType")]
        public string SourceType { get; set; } = "Template";

        /// <summary>
        /// Selected source identifier (template ID, query ID, table name, or SQL)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("sourceId")]
        public string SourceId { get; set; } = string.Empty;

        /// <summary>
        /// Custom SQL query (Advanced accordion)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("customSql")]
        public string CustomSql { get; set; } = string.Empty;

        /// <summary>
        /// Card title (required)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("title")]
        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(100, ErrorMessage = "العنوان لا يتجاوز 100 حرف")]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Display name shown on card (required)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("displayName")]
        [Required(ErrorMessage = "اسم العرض مطلوب")]
        [StringLength(80, ErrorMessage = "اسم العرض لا يتجاوز 80 حرف")]
        [Display(Name = "اسم العرض")]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Grid column span (1-12)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("gridWidth")]
        [Range(1, 12, ErrorMessage = "العرض بين 1 و 12")]
        public int GridWidth { get; set; } = 4;

        /// <summary>
        /// Grid row span (1-6)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("gridHeight")]
        [Range(1, 6, ErrorMessage = "الارتفاع بين 1 و 6")]
        public int GridHeight { get; set; } = 2;

        /// <summary>
        /// Grid X position (0-11, auto = -1)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("gridX")]
        public int GridX { get; set; } = -1;

        /// <summary>
        /// Grid Y position (0+, auto = -1)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("gridY")]
        public int GridY { get; set; } = -1;

        /// <summary>
        /// Chart color palette selection
        /// </summary>
        [BindProperty]
        [JsonPropertyName("colorPalette")]
        public string ColorPalette { get; set; } = "primary";

        /// <summary>
        /// Refresh interval in seconds (0 = off)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("refreshInterval")]
        public int RefreshInterval { get; set; } = 0;

        /// <summary>SQL query to execute for card data.</summary>
        [BindProperty]
        [JsonPropertyName("sqlQuery")]
        public string SqlQuery { get; set; } = string.Empty;

        // === Advanced KPI Fields ===

        /// <summary>
        /// KPI calculation mode: simple, comparison, breakdown
        /// </summary>
        [BindProperty]
        [JsonPropertyName("kpiMode")]
        public string KpiMode { get; set; } = "simple";

        /// <summary>
        /// Column name containing the numeric value for KPI calculation
        /// </summary>
        [BindProperty]
        [JsonPropertyName("valueColumn")]
        public string ValueColumn { get; set; } = string.Empty;

        /// <summary>
        /// Column name containing date data for time-based filtering
        /// </summary>
        [BindProperty]
        [JsonPropertyName("dateColumn")]
        public string DateColumn { get; set; } = string.Empty;

        /// <summary>
        /// Column name used for category grouping/breakdown
        /// </summary>
        [BindProperty]
        [JsonPropertyName("categoryColumn")]
        public string CategoryColumn { get; set; } = string.Empty;

        /// <summary>
        /// Whether to display change percentage on the KPI card
        /// </summary>
        [BindProperty]
        [JsonPropertyName("showChange")]
        public bool ShowChange { get; set; } = false;

        /// <summary>
        /// Source for change calculation: previousPeriod, fixedBaseline, custom
        /// </summary>
        [BindProperty]
        [JsonPropertyName("changeSource")]
        public string ChangeSource { get; set; } = "previousPeriod";

        /// <summary>
        /// Whether to display a sparkline trend line on the KPI card
        /// </summary>
        [BindProperty]
        [JsonPropertyName("showSparkline")]
        public bool ShowSparkline { get; set; } = false;

        /// <summary>
        /// Number of months of historical data for sparkline rendering
        /// </summary>
        [BindProperty]
        [JsonPropertyName("sparklineMonths")]
        public int SparklineMonths { get; set; } = 6;

        /// <summary>
        /// Whether to display a grand total row below the chart
        /// </summary>
        [BindProperty]
        [JsonPropertyName("showGrandTotal")]
        public bool ShowGrandTotal { get; set; } = false;

        /// <summary>
        /// Data source for grand total: sameTable, separateQuery, manual
        /// </summary>
        [BindProperty]
        [JsonPropertyName("grandTotalSource")]
        public string GrandTotalSource { get; set; } = "sameTable";

        /// <summary>
        /// Date filter mode: dashboard, fixed, relative, none
        /// </summary>
        [BindProperty]
        [JsonPropertyName("dateFilterMode")]
        public string DateFilterMode { get; set; } = "dashboard";

        /// <summary>
        /// Fixed start date for date filter (yyyy-MM-dd)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("fixedStartDate")]
        public string FixedStartDate { get; set; } = string.Empty;

        /// <summary>
        /// Fixed end date for date filter (yyyy-MM-dd)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("fixedEndDate")]
        public string FixedEndDate { get; set; } = string.Empty;

        /// <summary>
        /// Number of days for relative date filter
        /// </summary>
        [BindProperty]
        [JsonPropertyName("relativeDays")]
        public int RelativeDays { get; set; } = 30;

        /// <summary>
        /// Clone mode: pre-filled from existing card ID
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string CloneId { get; set; } = string.Empty;

        /// <summary>
        /// Template ID to pre-fill from (Step 2)
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string TemplateId { get; set; } = string.Empty;

        /// <summary>
        /// Available Oracle tables for Step 2 dropdown
        /// </summary>
        public List<SelectListItem> OracleTables { get; set; } = new();

        /// <summary>
        /// Available color palettes
        /// </summary>
        public List<ColorPaletteOption> ColorPalettes { get; set; } = new()
        {
            new() { Id = "primary", Name = "الرئيسي", Colors = new[] { "#1F4E79", "#2E6DA4", "#8FBCDE" } },
            new() { Id = "secondary", Name = "الثانوي", Colors = new[] { "#2E6DA4", "#1F4E79", "#8FBCDE" } },
            new() { Id = "accent", Name = "التمييز", Colors = new[] { "#0A2540", "#1F4E79", "#2E6DA4" } },
            new() { Id = "success", Name = "النجاح", Colors = new[] { "#1E9E6A", "#28A745", "#4CD97B" } },
            new() { Id = "warning", Name = "التحذير", Colors = new[] { "#E0A106", "#FFC107", "#FFD54F" } },
            new() { Id = "info", Name = "المعلومات", Colors = new[] { "#2E6DA4", "#17A2B8", "#4FC3F7" } },
            new() { Id = "custom", Name = "مخصص", Colors = new[] { "#1F4E79", "#E0A106", "#1E9E6A", "#D64545", "#2E6DA4", "#8FBCDE" } }
        };

        /// <summary>
        /// Available refresh intervals
        /// </summary>
        public List<RefreshIntervalOption> RefreshIntervals { get; set; } = new()
        {
            new() { Value = 0, Label = "إيقاف التحديث التلقائي" },
            new() { Value = 60, Label = "كل دقيقة" },
            new() { Value = 300, Label = "كل 5 دقائق" },
            new() { Value = 900, Label = "كل 15 دقيقة" },
            new() { Value = 3600, Label = "كل ساعة" }
        };

        /// <summary>
        /// Card type definitions for Step 1
        /// </summary>
        public List<CardTypeOption> CardTypes { get; set; } = new()
        {
            new() { Id = "KPI", Name = "بطاقة KPI", Icon = "speedometer", Description = "مؤشر أداء رئيسي واحد مع اتجاه", Category = "simple" },
            new() { Id = "Bar", Name = "رسم أعمدة", Icon = "bar-chart-2", Description = "مقارنة قيم عبر فئات", Category = "chart" },
            new() { Id = "Line", Name = "رسم خطي", Icon = "activity", Description = "اتجاه عبر الزمن", Category = "chart" },
            new() { Id = "Pie", Name = "رسم دائري", Icon = "pie-chart", Description = "توزيع نسبي", Category = "chart" },
            new() { Id = "Table", Name = "جدول بيانات", Icon = "grid", Description = "بيانات جدولية قابلة للفرز", Category = "table" },
            new() { Id = "Gauge", Name = "مقياس", Icon = "gauge", Description = "مقياس تقدم نحو هدف", Category = "simple" }
        };

        /// <summary>
        /// Template categories for Step 2
        /// </summary>
        public List<TemplateCategory> TemplateCategories { get; set; } = new()
        {
            new() { Id = "Template", Name = "قوالب جاهزة", Icon = "file-text", Description = "بطاقات معدة مسبقاً لسيناريوهات المخزن الشائعة" },
            new() { Id = "SavedQuery", Name = "استعلامات محفوظة", Icon = "database", Description = "استعلامات SQL محفوظة مسبقاً" },
            new() { Id = "SqlTable", Name = "جداول SQL Server", Icon = "server", Description = "جداول وعروض قاعدة البيانات المتاحة" },
            new() { Id = "CustomSQL", Name = "SQL مخصص (متقدم)", Icon = "code", Description = "كتابة استعلام SQL يدوي", IsAdvanced = true }
        };

        public async Task OnGetAsync()
        {
            await LoadOracleTablesAsync();
            await LoadCloneDataAsync();
            await LoadTemplateDataAsync();
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            // Load reference data for validation errors
            await LoadOracleTablesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var card = BuildDashboardCard();

                if (action == "save" || action == "saveAndAddAnother")
                {
                    var dto = card;
                    var entity = new DashboardCard
                    {
                        Title = dto.Title?.Trim() ?? "",
                        ChartType = dto.CardType ?? "",
                        DataSourceType = dto.SourceType == "SqlTable" ? "View" : "SQL Query",
                        SqlQuery = dto.SqlQuery,
                        GridPositionX = dto.GridX ?? 0,
                        GridPositionY = dto.GridY ?? 0,
                        GridWidth = dto.GridWidth > 0 ? dto.GridWidth : 4,
                        GridHeight = dto.GridHeight > 0 ? dto.GridHeight : 2,
                        RefreshInterval = dto.RefreshIntervalSeconds,
                        ColorPalette = dto.ColorPalette ?? "primary",
                        IsActive = dto.IsActive,
                        ValueColumn = dto.ValueColumn ?? "",
                        DateColumn = dto.DateColumn ?? "",
                        CategoryColumn = dto.CategoryColumn ?? "",
                        KpiMode = dto.KpiMode ?? "simple",
                        ShowChange = dto.ShowChange,
                        ChangeSource = dto.ChangeSource ?? "previousPeriod",
                        ShowSparkline = dto.ShowSparkline,
                        SparklineMonths = dto.SparklineMonths,
                        ShowGrandTotal = dto.ShowGrandTotal,
                        GrandTotalSource = dto.GrandTotalSource ?? "sameTable",
                        DateFilterMode = dto.DateFilterMode ?? "dashboard",
                        FixedStartDate = dto.FixedStartDate ?? "",
                        FixedEndDate = dto.FixedEndDate ?? "",
                        RelativeDays = dto.RelativeDays > 0 ? dto.RelativeDays : 30
                    };
                    _db.DashboardCards.Add(entity);
                    await _db.SaveChangesAsync();

                    TempData["ToastMessage"] = action == "saveAndAddAnother"
                        ? "تم حفظ البطاقة. جاري إنشاء بطاقة جديدة..."
                        : "تم حفظ البطاقة بنجاح.";
                    TempData["ToastType"] = "success";

                    if (action == "saveAndAddAnother")
                    {
                        return RedirectToPage("/admin-secure-panel/Cards/Builder");
                    }

                    return RedirectToPage("/admin-secure-panel/Cards/Index");
                }
                else if (action == "preview")
                {
                    // Return partial preview HTML/JSON
                    return Partial("_CardPreviewPartial", card);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving card from builder");
                ModelState.AddModelError(string.Empty, "حدث خطأ أثناء الحفظ. يرجى المحاولة مرة أخرى.");
            }

            return Page();
        }

        /// <summary>
        /// API endpoint for live preview - called via AJAX from card-builder.js
        /// </summary>
        public async Task<IActionResult> OnPostPreviewAsync([FromBody] PreviewRequest request)
        {
            try
            {
                var card = BuildCardFromRequest(request);
                var previewHtml = RenderPreview(card);
                return Content(previewHtml, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating preview");
                return Content($"<div class='wb-preview-error'><i class='lucide lucide-alert-triangle'></i><p>خطأ في المعاينة: {ex.Message}</p></div>", "text/html");
            }
        }



        private async Task LoadOracleTablesAsync()
        {
            try
            {
                var tables = await _cardBuilderService.GetAvailableTablesAsync(HttpContext.RequestAborted);
                OracleTables = tables.Select(t => new SelectListItem
                {
                    Value = t.SqlTargetTable,
                    Text = $"{t.Name} ({t.SqlTargetTable})"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load table mappings for Card Builder");
                OracleTables = new List<SelectListItem>();
            }
        }

        private List<SelectListItem> GetFallbackTables()
        {
            return new List<SelectListItem>
            {
                new() { Value = "INVENTORY_ITEMS", Text = "أصناف المخزون (INVENTORY_ITEMS)" },
                new() { Value = "STOCK_MOVEMENTS", Text = "حركات المخزون (STOCK_MOVEMENTS)" },
                new() { Value = "WAREHOUSES", Text = "المستودعات (WAREHOUSES)" },
                new() { Value = "SUPPLIERS", Text = "الموردين (SUPPLIERS)" },
                new() { Value = "PURCHASE_ORDERS", Text = "أوامر الشراء (PURCHASE_ORDERS)" },
                new() { Value = "STOCK_LEVELS", Text = "مستويات المخزون (STOCK_LEVELS)" }
            };
        }

        private async Task LoadCloneDataAsync()
        {
            if (string.IsNullOrEmpty(CloneId))
                return;

            if (!int.TryParse(CloneId, out var cloneId))
                return;

            try
            {
                var req = await _cardBuilderService.CloneFromCardAsync(cloneId, HttpContext.RequestAborted);
                if (req is null)
                    return;

                CardType   = req.ChartType;
                SourceType = req.DataSourceType;
                SourceId   = req.SqlQuery;
                CustomSql  = req.SqlQuery;
                Title      = req.Title;
                DisplayName= req.Title;
                GridWidth  = req.GridWidth;
                GridHeight = req.GridHeight;
                GridX      = req.GridPositionX;
                GridY      = req.GridPositionY;
                ColorPalette = "primary";
                RefreshInterval = req.RefreshInterval;

                // Mark as clone (new card, not update)
                CloneId = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load clone data for {CloneId}", CloneId);
            }
        }

        private async Task LoadTemplateDataAsync()
        {
            if (string.IsNullOrEmpty(TemplateId))
                return;

            // Templates are loaded from card-templates.js on client side
            // This is a server-side fallback
            await Task.CompletedTask;
        }

        private DashboardCardDto BuildDashboardCard()
        {
            return new DashboardCardDto
            {
                CardType = CardType,
                SourceType = SourceType,
                SourceId = SourceId,
                CustomSql = SourceType == "CustomSQL" ? CustomSql : null,
                SqlQuery = SqlQuery,
                Title = Title,
                DisplayName = DisplayName,
                GridWidth = GridWidth,
                GridHeight = GridHeight,
                GridX = GridX >= 0 ? GridX : null,
                GridY = GridY >= 0 ? GridY : null,
                ColorPalette = ColorPalette,
                RefreshIntervalSeconds = RefreshInterval,
                // Advanced KPI
                KpiMode = KpiMode,
                ValueColumn = ValueColumn,
                DateColumn = DateColumn,
                CategoryColumn = CategoryColumn,
                ShowChange = ShowChange,
                ChangeSource = ChangeSource,
                ShowSparkline = ShowSparkline,
                SparklineMonths = SparklineMonths,
                ShowGrandTotal = ShowGrandTotal,
                GrandTotalSource = GrandTotalSource,
                DateFilterMode = DateFilterMode,
                FixedStartDate = FixedStartDate,
                FixedEndDate = FixedEndDate,
                RelativeDays = RelativeDays,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private DashboardCardDto BuildCardFromRequest(PreviewRequest request)
        {
            return new DashboardCardDto
            {
                CardType = request.CardType,
                SourceType = request.SourceType,
                SourceId = request.SourceId,
                CustomSql = request.CustomSql,
                SqlQuery = request.SqlQuery,
                Title = request.Title,
                DisplayName = request.DisplayName,
                GridWidth = request.GridWidth,
                GridHeight = request.GridHeight,
                GridX = request.GridX,
                GridY = request.GridY,
                ColorPalette = request.ColorPalette,
                RefreshIntervalSeconds = request.RefreshInterval,
                // Advanced KPI
                KpiMode = request.KpiMode,
                ValueColumn = request.ValueColumn,
                DateColumn = request.DateColumn,
                CategoryColumn = request.CategoryColumn,
                ShowChange = request.ShowChange,
                ChangeSource = request.ChangeSource,
                ShowSparkline = request.ShowSparkline,
                SparklineMonths = request.SparklineMonths,
                ShowGrandTotal = request.ShowGrandTotal,
                GrandTotalSource = request.GrandTotalSource,
                DateFilterMode = request.DateFilterMode,
                FixedStartDate = request.FixedStartDate,
                FixedEndDate = request.FixedEndDate,
                RelativeDays = request.RelativeDays
            };
        }

        private string RenderPreview(DashboardCardDto card)
        {
            // In real implementation, this would render a partial view with chart component
            // For now, return a placeholder that the JS will replace with actual component
            return $@"
                <div class='wb-preview-card' data-card-type='{card.CardType}'>
                    <div class='wb-preview-skeleton' aria-hidden='true'>
                        <div class='wb-skeleton-title'></div>
                        <div class='wb-skeleton-chart'></div>
                    </div>
                    <div class='wb-preview-content' style='display:none;'>
                        <!-- Chart component rendered by card-builder.js -->
                    </div>
                </div>";
        }
    }

    // DTOs
    public class CardTypeOption
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class TemplateCategory
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAdvanced { get; set; } = false;
    }

    public class ColorPaletteOption
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string[] Colors { get; set; } = Array.Empty<string>();
    }

    public class RefreshIntervalOption
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
    }



    public class DashboardCardDto
    {
        public string CardType { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string SourceId { get; set; } = string.Empty;
        public string? CustomSql { get; set; }
        public string SqlQuery { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int? GridX { get; set; }
        public int? GridY { get; set; }
        public string ColorPalette { get; set; } = string.Empty;
        public int RefreshIntervalSeconds { get; set; }

        // === Advanced KPI ===
        public string KpiMode { get; set; } = "simple";
        public string ValueColumn { get; set; } = string.Empty;
        public string DateColumn { get; set; } = string.Empty;
        public string CategoryColumn { get; set; } = string.Empty;
        public bool ShowChange { get; set; } = false;
        public string ChangeSource { get; set; } = "previousPeriod";
        public bool ShowSparkline { get; set; } = false;
        public int SparklineMonths { get; set; } = 6;
        public bool ShowGrandTotal { get; set; } = false;
        public string GrandTotalSource { get; set; } = "sameTable";
        public string DateFilterMode { get; set; } = "dashboard";
        public string FixedStartDate { get; set; } = string.Empty;
        public string FixedEndDate { get; set; } = string.Empty;
        public int RelativeDays { get; set; } = 30;

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PreviewRequest
    {
        public string CardType { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string SourceId { get; set; } = string.Empty;
        public string CustomSql { get; set; } = string.Empty;
        public string SqlQuery { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        public string ColorPalette { get; set; } = string.Empty;
        public int RefreshInterval { get; set; }

        // Advanced KPI
        public string KpiMode { get; set; } = "simple";
        public string ValueColumn { get; set; } = string.Empty;
        public string DateColumn { get; set; } = string.Empty;
        public string CategoryColumn { get; set; } = string.Empty;
        public bool ShowChange { get; set; } = false;
        public string ChangeSource { get; set; } = "previousPeriod";
        public bool ShowSparkline { get; set; } = false;
        public int SparklineMonths { get; set; } = 6;
        public bool ShowGrandTotal { get; set; } = false;
        public string GrandTotalSource { get; set; } = "sameTable";
        public string DateFilterMode { get; set; } = "dashboard";
        public string FixedStartDate { get; set; } = string.Empty;
        public string FixedEndDate { get; set; } = string.Empty;
        public int RelativeDays { get; set; } = 30;
    }
}