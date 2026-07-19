using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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
        public string? SourceId { get; set; }

        /// <summary>
        /// Custom SQL query (Advanced accordion)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("customSql")]
        public string? CustomSql { get; set; }

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
        /// Display name shown on card (optional — maps to Title)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Optional description shown as tooltip on the card
        /// </summary>
        [BindProperty]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

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
        /// Grid X position (0-11, auto = null)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("gridX")]
        public int? GridX { get; set; }

        /// <summary>
        /// Grid Y position (0+, auto = null)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("gridY")]
        public int? GridY { get; set; }

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
        public string? ValueColumn { get; set; }

        /// <summary>
        /// Column name containing date data for time-based filtering
        /// </summary>
        [BindProperty]
        [JsonPropertyName("dateColumn")]
        public string? DateColumn { get; set; }

        /// <summary>
        /// Column name used for category grouping/breakdown
        /// </summary>
        [BindProperty]
        [JsonPropertyName("categoryColumn")]
        public string? CategoryColumn { get; set; }

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
        public string? FixedStartDate { get; set; }

        /// <summary>
        /// Fixed end date for date filter (yyyy-MM-dd)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("fixedEndDate")]
        public string? FixedEndDate { get; set; }

        /// <summary>
        /// Number of days for relative date filter
        /// </summary>
        [BindProperty]
        [JsonPropertyName("relativeDays")]
        public int RelativeDays { get; set; } = 30;

        /// <summary>
        /// Aggregation method for KPI ValueColumn: Sum, Count, Avg, Min, Max, None
        /// </summary>
        [BindProperty]
        [JsonPropertyName("aggregationType")]
        public string AggregationType { get; set; } = "Sum";

        // === Builder Original Source Type (TASK-COD-028) ===

        /// <summary>
        /// Original SourceType from Step 2: "Template", "SavedQuery", "SqlTable", "CustomSQL".
        /// Default: "SqlTable". Persisted so the Builder wizard can be reconstructed on edit.
        /// </summary>
        [BindProperty]
        [JsonPropertyName("originalSourceType")]
        public string OriginalSourceType { get; set; } = "SqlTable";

        /// <summary>
        /// Source-specific identifier: table name for SqlTable, template ID for Template, query ID for SavedQuery.
        /// Empty for CustomSQL. Default: "".
        /// </summary>
        [BindProperty]
        [JsonPropertyName("originalSourceId")]
        public string OriginalSourceId { get; set; } = "";

        /// <summary>
        /// Dashboard ID to assign the card to (nullable FK)
        /// </summary>
        [BindProperty]
        [JsonPropertyName("dashboardId")]
        public int? DashboardId { get; set; }

        /// <summary>
        /// Clone mode: pre-filled from existing card ID
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? CloneId { get; set; }

        /// <summary>
        /// Edit mode: load existing card ID into the wizard
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? EditId { get; set; }

        /// <summary>
        /// Template ID to pre-fill from (Step 2)
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? TemplateId { get; set; }

        /// <summary>
        /// Available Oracle tables for Step 2 dropdown
        /// </summary>
        public List<SelectListItem> OracleTables { get; set; } = new();

        /// <summary>
        /// Available dashboards for the dashboard selector dropdown
        /// </summary>
        public List<SelectListItem> AvailableDashboards { get; set; } = new();

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
            NormalizeGetAliases();
            await LoadOracleTablesAsync();
            await LoadDashboardsAsync();
            await LoadCloneDataAsync();
            await LoadEditDataAsync();
            await LoadTemplateDataAsync();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] string saveAction)
        {
            _logger.LogInformation("CARD SAVE TRACE: saveAction={SaveAction} | CardType={CardType} | SourceType={SourceType} | Title={Title} | SqlQueryLen={SqlQueryLen} | ModelStateValid={IsValid}",
                string.IsNullOrWhiteSpace(saveAction) ? "<empty>" : saveAction,
                CardType, SourceType, Title,
                (SqlQuery ?? "").Length,
                ModelState.IsValid);
            _logger.LogInformation("Card Builder POST started");
            _logger.LogInformation("Card Builder POST action: {Action}", string.IsNullOrWhiteSpace(saveAction) ? "<empty>" : saveAction);
            _logger.LogInformation("Card Builder POST SqlQuery present: {SqlQueryPresent}", !string.IsNullOrWhiteSpace(SqlQuery));

            // Load reference data for validation errors
            await LoadOracleTablesAsync();
            await LoadDashboardsAsync();

            ValidateConditionalPostFields();

            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                DisplayName = Title;
            }

            ModelState.Remove(nameof(DisplayName));

            _logger.LogInformation("Card Builder POST ModelState valid: {IsValid}", ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                ModelState.AddModelError(string.Empty, "تعذر حفظ البطاقة. يرجى مراجعة الحقول المطلوبة والقيم المدخلة ثم المحاولة مرة أخرى.");
                return Page();
            }

            try
            {
                var card = BuildDashboardCard();

                if (saveAction == "save" || saveAction == "saveAndAddAnother")
                {
                    var dto = card;

                    if (!string.IsNullOrWhiteSpace(EditId) && int.TryParse(EditId, out var editId))
                    {
                        var existing = await _db.DashboardCards.FindAsync(editId);
                        if (existing is null)
                        {
                            ModelState.AddModelError(string.Empty, "البطاقة المطلوب تعديلها غير موجودة.");
                            return Page();
                        }

                        var originalIsActive = existing.IsActive;
                        var originalDashboardId = existing.DashboardId;
                        MapDtoToEntity(dto, existing);
                        existing.IsActive = originalIsActive;
                        existing.DashboardId ??= originalDashboardId;
                        existing.UpdatedAt = DateTime.UtcNow;

                        _logger.LogInformation("Card Builder SaveChangesAsync starting for edit {EditId}", editId);
                        await _db.SaveChangesAsync();
                        _logger.LogInformation("Card Builder SaveChangesAsync completed. Updated DashboardCard id: {DashboardCardId}", existing.Id);

                        TempData["ToastMessage"] = "تم تحديث البطاقة بنجاح.";
                        TempData["ToastType"] = "success";
                        return RedirectToPage("/admin-secure-panel/Cards/Index");
                    }

                    var entity = new DashboardCard
                    {
                        Title = dto.Title?.Trim() ?? "",
                        Description = dto.Description ?? "",
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
                        RelativeDays = dto.RelativeDays > 0 ? dto.RelativeDays : 30,
                        AggregationType = dto.AggregationType ?? "Sum",
                        OriginalSourceType = dto.OriginalSourceType,
                        OriginalSourceId = dto.OriginalSourceId ?? "",
                        DashboardId = dto.DashboardId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _db.DashboardCards.Add(entity);
                    _logger.LogInformation("Card Builder SaveChangesAsync starting for action {Action}", saveAction);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Card Builder SaveChangesAsync completed. Inserted DashboardCard id: {DashboardCardId}", entity.Id);

                    TempData["ToastMessage"] = saveAction == "saveAndAddAnother"
                        ? "تم حفظ البطاقة. جاري إنشاء بطاقة جديدة..."
                        : "تم حفظ البطاقة بنجاح.";
                    TempData["ToastType"] = "success";

                    if (saveAction == "saveAndAddAnother")
                    {
                        return RedirectToPage("/admin-secure-panel/Cards/Builder");
                    }

                    return RedirectToPage("/admin-secure-panel/Cards/Index");
                }
                else if (saveAction == "preview")
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



        private void NormalizeGetAliases()
        {
            if (string.IsNullOrWhiteSpace(EditId)
                && Request.Query.TryGetValue("edit", out var editValues))
            {
                EditId = editValues.ToString();
            }

            if (string.IsNullOrWhiteSpace(CloneId)
                && Request.Query.TryGetValue("clone", out var cloneValues))
            {
                CloneId = cloneValues.ToString();
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

        private async Task LoadDashboardsAsync()
        {
            AvailableDashboards = await _db.Dashboards
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = string.IsNullOrEmpty(d.Icon) ? d.Name : $"{d.Icon} {d.Name}"
                })
                .ToListAsync();
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
                AggregationType = req.AggregationType ?? "Sum";

                // Load DashboardId from the original card
                var originalCard = await _db.DashboardCards.AsNoTracking()
                    .Where(c => c.Id == cloneId)
                    .Select(c => c.DashboardId)
                    .FirstOrDefaultAsync();
                DashboardId = originalCard;

                // Mark as clone (new card, not update)
                CloneId = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load clone data for {CloneId}", CloneId);
            }
        }

        private async Task LoadEditDataAsync()
        {
            if (string.IsNullOrWhiteSpace(EditId))
                return;

            if (!int.TryParse(EditId, out var editId))
            {
                _logger.LogWarning("Invalid edit id {EditId}", EditId);
                return;
            }

            try
            {
                var card = await _db.DashboardCards
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == editId);

                if (card is null)
                {
                    _logger.LogWarning("Card {EditId} not found for edit", editId);
                    return;
                }

                CardType = card.ChartType;
                Title = card.Title;
                Description = card.Description;
                DisplayName = card.Title;
                GridWidth = card.GridWidth;
                GridHeight = card.GridHeight;
                GridX = card.GridPositionX;
                GridY = card.GridPositionY;
                ColorPalette = card.ColorPalette;
                RefreshInterval = card.RefreshInterval;
                SqlQuery = card.SqlQuery;
                KpiMode = card.KpiMode;
                ValueColumn = card.ValueColumn;
                DateColumn = card.DateColumn;
                CategoryColumn = card.CategoryColumn;
                ShowChange = card.ShowChange;
                ChangeSource = card.ChangeSource;
                ShowSparkline = card.ShowSparkline;
                SparklineMonths = card.SparklineMonths;
                ShowGrandTotal = card.ShowGrandTotal;
                GrandTotalSource = card.GrandTotalSource;
                DateFilterMode = card.DateFilterMode;
                FixedStartDate = card.FixedStartDate;
                FixedEndDate = card.FixedEndDate;
                RelativeDays = card.RelativeDays;
                AggregationType = card.AggregationType;
                DashboardId = card.DashboardId;

                // OriginalSourceType mapping: use card.OriginalSourceType to reconstruct Step 2 exactly.
                OriginalSourceType = card.OriginalSourceType;
                OriginalSourceId = card.OriginalSourceId;

                if (!string.IsNullOrEmpty(card.OriginalSourceType))
                {
                    SourceType = card.OriginalSourceType;

                    if (card.OriginalSourceType == "SqlTable")
                    {
                        SourceId = card.OriginalSourceId;
                        CustomSql = string.Empty;
                    }
                    else if (card.OriginalSourceType == "CustomSQL")
                    {
                        SourceId = string.Empty;
                        CustomSql = card.SqlQuery;
                    }
                    else // Template / SavedQuery
                    {
                        SourceId = card.OriginalSourceId;
                        CustomSql = card.SqlQuery;
                    }
                }
                else
                {
                    // Fallback for old cards (before migration): parse from DataSourceType
                    if (string.Equals(card.DataSourceType, "View", StringComparison.OrdinalIgnoreCase))
                    {
                        var match = Regex.Match(card.SqlQuery ?? "", @"FROM\s+\[([^\]]+)\]", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        if (match.Success)
                        {
                            SourceType = "SqlTable";
                            SourceId = match.Groups[1].Value;
                            CustomSql = string.Empty;
                        }
                        else
                        {
                            SourceType = "CustomSQL";
                            SourceId = string.Empty;
                            CustomSql = card.SqlQuery;
                        }
                    }
                    else
                    {
                        SourceType = "CustomSQL";
                        SourceId = string.Empty;
                        CustomSql = card.SqlQuery;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load edit data for {EditId}", EditId);
            }
        }

        private static void MapDtoToEntity(DashboardCardDto dto, DashboardCard entity)
        {
            entity.Title = dto.Title?.Trim() ?? "";
            entity.Description = dto.Description ?? "";
            entity.ChartType = dto.CardType ?? "";
            entity.DataSourceType = dto.SourceType == "SqlTable" ? "View" : "SQL Query";
            entity.SqlQuery = dto.SqlQuery;
            entity.GridPositionX = dto.GridX ?? 0;
            entity.GridPositionY = dto.GridY ?? 0;
            entity.GridWidth = dto.GridWidth > 0 ? dto.GridWidth : 4;
            entity.GridHeight = dto.GridHeight > 0 ? dto.GridHeight : 2;
            entity.RefreshInterval = dto.RefreshIntervalSeconds;
            entity.ColorPalette = dto.ColorPalette ?? "primary";
            entity.IsActive = dto.IsActive;
            entity.ValueColumn = dto.ValueColumn ?? "";
            entity.DateColumn = dto.DateColumn ?? "";
            entity.CategoryColumn = dto.CategoryColumn ?? "";
            entity.KpiMode = dto.KpiMode ?? "simple";
            entity.ShowChange = dto.ShowChange;
            entity.ChangeSource = dto.ChangeSource ?? "previousPeriod";
            entity.ShowSparkline = dto.ShowSparkline;
            entity.SparklineMonths = dto.SparklineMonths;
            entity.ShowGrandTotal = dto.ShowGrandTotal;
            entity.GrandTotalSource = dto.GrandTotalSource ?? "sameTable";
            entity.DateFilterMode = dto.DateFilterMode ?? "dashboard";
            entity.FixedStartDate = dto.FixedStartDate ?? "";
            entity.FixedEndDate = dto.FixedEndDate ?? "";
            entity.RelativeDays = dto.RelativeDays > 0 ? dto.RelativeDays : 30;
            entity.AggregationType = dto.AggregationType ?? "Sum";
            entity.OriginalSourceType = dto.OriginalSourceType ?? "SqlTable";
            entity.OriginalSourceId = dto.OriginalSourceId ?? "";
            entity.DashboardId = dto.DashboardId;
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
                SourceId = SourceId ?? string.Empty,
                CustomSql = SourceType == "CustomSQL" ? CustomSql?.Trim() : null,
                SqlQuery = SqlQuery,
                Title = Title,
                DisplayName = DisplayName ?? Title,
                Description = Description ?? string.Empty,
                GridWidth = GridWidth,
                GridHeight = GridHeight,
                GridX = GridX.HasValue && GridX.Value >= 0 ? GridX.Value : null,
                GridY = GridY.HasValue && GridY.Value >= 0 ? GridY.Value : null,
                ColorPalette = ColorPalette,
                RefreshIntervalSeconds = RefreshInterval,
                // Advanced KPI
                KpiMode = KpiMode,
                ValueColumn = ValueColumn ?? string.Empty,
                DateColumn = DateColumn ?? string.Empty,
                CategoryColumn = CategoryColumn ?? string.Empty,
                ShowChange = ShowChange,
                ChangeSource = ChangeSource,
                ShowSparkline = ShowSparkline,
                SparklineMonths = SparklineMonths,
                ShowGrandTotal = ShowGrandTotal,
                GrandTotalSource = GrandTotalSource,
                DateFilterMode = DateFilterMode,
                FixedStartDate = FixedStartDate ?? string.Empty,
                FixedEndDate = FixedEndDate ?? string.Empty,
                RelativeDays = RelativeDays,
                AggregationType = AggregationType ?? "Sum",
                OriginalSourceType = OriginalSourceType ?? SourceType,  // fallback to current sourceType
                OriginalSourceId = OriginalSourceId ?? SourceId ?? "",
                DashboardId = DashboardId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private void ValidateConditionalPostFields()
        {
            if (string.Equals(SourceType, "CustomSQL", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(CustomSql))
            {
                ModelState.AddModelError(nameof(CustomSql), "استعلام SQL المخصص مطلوب عند اختيار مصدر SQL مخصص.");
            }

            if (!string.Equals(DateFilterMode, "fixed", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(FixedStartDate))
            {
                ModelState.AddModelError(nameof(FixedStartDate), "تاريخ البداية مطلوب عند اختيار نطاق زمني ثابت.");
            }

            if (string.IsNullOrWhiteSpace(FixedEndDate))
            {
                ModelState.AddModelError(nameof(FixedEndDate), "تاريخ النهاية مطلوب عند اختيار نطاق زمني ثابت.");
            }
        }

        private void LogModelStateErrors()
        {
            foreach (var entry in ModelState.Where(e => e.Value?.Errors.Count > 0))
            {
                var fieldName = string.IsNullOrWhiteSpace(entry.Key) ? "<model>" : entry.Key;
                var sanitizedErrors = entry.Value!.Errors
                    .Select(error => SanitizeLogMessage(error.ErrorMessage))
                    .Where(message => !string.IsNullOrWhiteSpace(message))
                    .ToArray();

                _logger.LogWarning(
                    "Card Builder ModelState invalid field {FieldName}: {Errors}",
                    fieldName,
                    sanitizedErrors.Length == 0 ? "<no message>" : string.Join(" | ", sanitizedErrors));
            }
        }

        private static string SanitizeLogMessage(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return string.Empty;
            }

            var sanitized = message.Replace("\r", " ").Replace("\n", " ").Trim();
            return sanitized.Length <= 300 ? sanitized : sanitized[..300];
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
                Description = request.Description,
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
                RelativeDays = request.RelativeDays,
                AggregationType = request.AggregationType,

                // Builder Original Source Type
                OriginalSourceType = request.OriginalSourceType,
                OriginalSourceId = request.OriginalSourceId
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
        public string Description { get; set; } = string.Empty;
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
        public string AggregationType { get; set; } = "Sum";

        // === Builder Original Source Type (TASK-COD-028) ===
        public string OriginalSourceType { get; set; } = "SqlTable";
        public string OriginalSourceId { get; set; } = "";

        // === Dashboard assignment ===
        public int? DashboardId { get; set; }

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
        public string Description { get; set; } = string.Empty;
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
        public string AggregationType { get; set; } = "Sum";

        // === Builder Original Source Type (TASK-COD-028) ===
        public string OriginalSourceType { get; set; } = "SqlTable";
        public string OriginalSourceId { get; set; } = "";
    }
}
