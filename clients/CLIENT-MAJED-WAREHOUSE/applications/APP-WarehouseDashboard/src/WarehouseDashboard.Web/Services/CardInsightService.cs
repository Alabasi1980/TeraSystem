using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Infrastructure;
using System.Diagnostics;
using System.Text;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

public class CardInsightService
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ReadOnlyQueryHelper _readOnly;
    private readonly IAIProvider _aiProvider;
    private readonly AssistantLogService _logService;
    private readonly AssistantCacheService _cacheService;
    private readonly ILogger<CardInsightService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IOptions<AIAssistantOptions> _options;

    // ======== GENERAL SYSTEM PROMPT — شخصية المساعد ========
    private const string GeneralPrompt = @"أنت مساعد بيانات تنفيذي داخل Dashboard. جمهورك هيئة إدارية تطلع على تحليلات مختصرة ودقيقة.

مهمتك: اشرح البطاقة الحالية بناءً على البيانات المُرسلة إليك فقط. لا تخترع ولا تفلسف.

قواعد صارمة:
1. اعتمد فقط على البيانات المرسلة إليك. لا تخترع أرقاماً أو أسباباً أو اتجاهات.
2. إذا لم تكن هناك بيانات كافية للتحليل، اذكر ذلك بصراحة وبدون مبالغة. مثلاً: ""هذه البطاقة تعرض عدد الوحدات المعرفة فقط. لا توجد بيانات رقمية أو تاريخية للتحليل.""
3. إذا كانت هناك بيانات حقيقية (قيمة حالية، تغير، اتجاه، فئات)، حلّلها بشكل مباشر وواضح.
4. لا تكتب فلسفة زائدة أو كلاماً عاماً غير مفيد. لا تقل ""يُفضل"" أو ""يُنصح"" بدون دليل من البيانات.
5. لا تفرض هيكل إجابة ثابتاً. أجب حسب ما تستدعيه البيانات: أحياناً جملة واحدة تكفي، أحياناً 3-4 أسطر.
6. إذا طلب المستخدم تعمقاً ووصلت إلى نهاية البيانات، قل ببساطة: ""هذه كل البيانات المتوفرة حالياً.""
7. الجمهور المستهدف هيئة إدارية. كن واقعياً، مختصراً، دقيقاً. لا مصطلحات فنية غير ضرورية.

أسلوب الرد: العربية الواضحة. نبرة تنفيذية مباشرة. طول مناسب حسب المعلومات المتاحة. استخدم رموزاً بسيطة عند الحاجة فقط.";

    public CardInsightService(
        WarehouseDashboardDbContext db,
        ReadOnlyQueryHelper readOnly,
        IAIProvider aiProvider,
        AssistantLogService logService,
        AssistantCacheService cacheService,
        IMemoryCache memoryCache,
        IOptions<AIAssistantOptions> options,
        ILogger<CardInsightService> logger)
    {
        _db = db;
        _readOnly = readOnly;
        _aiProvider = aiProvider;
        _logService = logService;
        _cacheService = cacheService;
        _memoryCache = memoryCache;
        _options = options;
        _logger = logger;
    }

    public async Task<AIAssistantResponse> AnalyzeCardAsync(
        int cardId, string mode, int depthLevel, CancellationToken ct = default)
    {
        // Load card configuration from EF Core
        var card = await _db.DashboardCards.FindAsync(new object[] { cardId }, ct);
        if (card is null)
        {
            return new AIAssistantResponse
            {
                Success = false,
                ErrorMessage = "البطاقة غير موجودة."
            };
        }

        if (!card.AssistantEnabled)
        {
            return new AIAssistantResponse
            {
                Success = false,
                ErrorMessage = "المساعد غير مفعّل لهذه البطاقة."
            };
        }

        // Build system prompt: general + optional card-specific
        var systemPrompt = GeneralPrompt;
        if (!string.IsNullOrWhiteSpace(card.AssistantPrompt))
        {
            systemPrompt = $"{GeneralPrompt}\n\nتعليمات خاصة لهذه البطاقة:\n{card.AssistantPrompt}";
        }

        // Build user message with card context
        // For now (Phase B), just pass basic card info.
        // Phase C tasks will add actual data summaries.
        var userMessage = BuildUserMessage(card, mode, depthLevel);

        var request = new AIAssistantRequest
        {
            SystemPrompt = systemPrompt,
            UserMessage = userMessage,
            CardAssistantPrompt = card.AssistantPrompt
        };

        return await _aiProvider.SendAsync(request, ct);
    }

    private static string BuildUserMessage(DashboardCard card, string mode, int depthLevel)
    {
        var depthLabel = depthLevel switch
        {
            1 => "آخر 3 أشهر",
            2 => "آخر 6 أشهر",
            3 => "آخر سنة",
            4 => "آخر 3 سنوات",
            5 => "آخر 5 سنوات",
            6 => "آخر 10 سنوات / كل البيانات المتاحة",
            _ => "النطاق الافتراضي"
        };

        var modeLabel = mode switch
        {
            "explain" => "شرح البطاقة",
            "deep" => "شرح عميق",
            _ => "تحليل البطاقة"
        };

        return $@"تحليل بطاقة:

العنوان: {card.Title}
النوع: {card.ChartType}
الوصف: {card.Description ?? "لا يوجد وصف"}
نوع التحليل: {modeLabel}
النطاق الزمني: {depthLabel}

        يرجى تحليل هذه البطاقة حسب التعليمات.";
    }

    /// <summary>
    /// Builds a rich user message containing actual data from CardSummary.
    /// Used when a builder successfully produced a summary with data.
    /// Falls back gracefully for missing/partial data.
    /// </summary>
    private static string BuildRichUserMessage(DashboardCard card, string mode, int depthLevel, CardSummary summary)
    {
        var depthLabel = depthLevel switch
        {
            1 => "آخر 3 أشهر",
            2 => "آخر 6 أشهر",
            3 => "آخر سنة",
            4 => "آخر 3 سنوات",
            5 => "آخر 5 سنوات",
            6 => "آخر 10 سنوات / كل البيانات المتاحة",
            _ => "النطاق الافتراضي"
        };

        var seriesText = summary.SeriesData.Count > 0
            ? string.Join("\n", summary.SeriesData.Select(p => $"- {p.Period}: {p.Value}"))
            : "لا توجد بيانات سلاسل زمنية";

        var topCount = Math.Min(summary.TopItems.Count, 5);
        var topText = topCount > 0
            ? string.Join("\n", summary.TopItems.Take(topCount).Select(i =>
                $"- {i.Name}: {i.Value} ({i.Percent?.ToString("F1") ?? "N/A"}%)"))
            : "لا توجد";

        var bottomCount = Math.Min(summary.BottomItems.Count, 5);
        var bottomText = bottomCount > 0
            ? string.Join("\n", summary.BottomItems.Take(bottomCount).Select(i =>
                $"- {i.Name}: {i.Value} ({i.Percent?.ToString("F1") ?? "N/A"}%)"))
            : "لا توجد";

        var qualityNotes = summary.DataQualityNotes.Count > 0
            ? string.Join("\n", summary.DataQualityNotes.Select(n => $"- {n}"))
            : "لا توجد ملاحظات";

        return $@"تحليل بطاقة: {card.Title} ({card.ChartType})
الوصف: {card.Description ?? "لا يوجد وصف"}

بيانات البطاقة الفعلية:
- القيمة الحالية: {summary.CurrentValue?.ToString("N2") ?? "غير متوفرة"}
- القيمة السابقة: {summary.PreviousValue?.ToString("N2") ?? "غير متوفرة"}
- نسبة التغير: {summary.ChangePercent?.ToString("F1") ?? "غير متوفرة"}%
- الاتجاه: {summary.TrendDirection ?? "غير متوفر"}

السلسلة الزمنية ({depthLabel}):
{seriesText}

أعلى {topCount} فئات:
{topText}

أدنى {bottomCount} فئات:
{bottomText}

إجمالي الصفوف: {summary.TotalRowCount?.ToString("N0") ?? "غير متوفر"}

ملاحظات الجودة:
{qualityNotes}";
    }

    /// <summary>
    /// ينفّذ كويري البطاقة وكويريات التعمق حسب مستوى العمق، ويعيد النتائج كنص منسق.
    /// </summary>
    private async Task<string> BuildRawDataMessage(DashboardCard card, int depthLevel, CancellationToken ct)
    {
        var sb = new StringBuilder();
        const int noDateLimit = 200; // حد أمان للبطاقات بدون تاريخ

        // تحديد تاريخ البداية حسب مستوى العمق (فقط للبطاقات التي تحتوي DateColumn)
        string? fromDate = null;
        bool allData = false;
        if (!string.IsNullOrEmpty(card.DateColumn))
        {
            if (depthLevel >= 6)
                allData = true; // مستوى 6 = كل البيانات
            else
                fromDate = depthLevel switch
                {
                    1 => DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd"),
                    2 => DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"),
                    3 => DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"),
                    4 => DateTime.Now.AddYears(-3).ToString("yyyy-MM-dd"),
                    5 => DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd"),
                    _ => DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd")
                };
        }

        // 1. كويري البطاقة الأساسي
        string mainQuery;
        var parameters = new Dictionary<string, object>();
        if (fromDate != null)
        {
            // تصفية زمنية حسب العمق مع باراميتر
            mainQuery = $"SELECT * FROM ({card.SqlQuery}) _data WHERE {card.DateColumn} >= @from ORDER BY {card.DateColumn}";
            parameters["@from"] = fromDate;
        }
        else if (allData)
        {
            // كل البيانات (بدون تصفية)
            mainQuery = $"SELECT * FROM ({card.SqlQuery}) _data";
        }
        else
        {
            // لا يوجد عمود تاريخ — نحدد عدد الصفوف حماية
            mainQuery = $"SELECT TOP {noDateLimit} * FROM ({card.SqlQuery}) _data";
        }

        try
        {
            var rows = await _readOnly.QueryAsync(mainQuery, parameters.Count > 0 ? parameters : null);
            sb.AppendLine($"--- بيانات البطاقة ({rows.Count} صف{(fromDate != null ? $", من {fromDate}" : "")}) ---");
            if (rows.Count > 0)
                sb.AppendLine(FormatRowsAsText(rows));
        }
        catch (Exception ex)
        {
            sb.AppendLine("(تعذر تنفيذ كويري البطاقة)");
            _logger.LogWarning(ex, "Failed to execute card SQL for card {CardId}", card.Id);
        }

        // 2. كويريات التعمق غير المرتبطة (لا تحتاج Parameter)
        if (card.DrillDownLevels?.Any() == true)
        {
            var rootLevels = card.DrillDownLevels.Where(d => !d.RequiresParentValue).OrderBy(d => d.Level).ToList();
            foreach (var level in rootLevels)
            {
                string drillQuery;
                var drillParams = new Dictionary<string, object>();
                if (fromDate != null)
                {
                    drillQuery = $"SELECT * FROM ({level.DrillDownQuery}) _data WHERE {card.DateColumn} >= @from";
                    drillParams["@from"] = fromDate;
                }
                else if (!string.IsNullOrEmpty(card.DateColumn))
                {
                    drillQuery = $"SELECT * FROM ({level.DrillDownQuery}) _data";
                }
                else
                {
                    drillQuery = $"SELECT TOP {noDateLimit} * FROM ({level.DrillDownQuery}) _data";
                }

                try
                {
                    var rows = await _readOnly.QueryAsync(drillQuery, drillParams.Count > 0 ? drillParams : null);
                    sb.AppendLine($"--- تعمق مستوى {level.Level}: {level.DisplayName} ({rows.Count} صف) ---");
                    if (rows.Count > 0)
                        sb.AppendLine(FormatRowsAsText(rows));
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"(تعذر تنفيذ تعمق مستوى {level.Level})");
                    _logger.LogWarning(ex, "Failed to execute drill level {Level} for card {CardId}", level.Level, card.Id);
                }
            }

            // الكويريات التي تحتاج Parameter
            var dependentLevels = card.DrillDownLevels.Where(d => d.RequiresParentValue).OrderBy(d => d.Level).ToList();
            if (dependentLevels.Any())
            {
                sb.AppendLine("--- تعمق يحتاج اختيار قيمة ---");
                foreach (var level in dependentLevels)
                    sb.AppendLine($"- مستوى {level.Level}: {level.DisplayName} (يحتاج اختيار قيمة)");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// يحول قائمة الصفوف إلى نص مقروء (أعمدة + قيم).
    /// </summary>
    private static string FormatRowsAsText(List<Dictionary<string, object?>> rows)
    {
        var sb = new StringBuilder();
        foreach (var row in rows)
        {
            var parts = row.Select(kvp => $"{kvp.Key}: {kvp.Value ?? "(فارغ)"}");
            sb.AppendLine("  " + string.Join(" | ", parts));
        }
        return sb.ToString();
    }

    public async Task<CardInsightResponse> AnalyzeCardWithCacheAsync(
        int cardId, string mode, int depthLevel,
        string dataScopeLabel, bool isFullDataReached, bool hasDeeperData,
        CardSummary? summary = null,
        CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        string? errorCode = null;
        bool wasCached = false;

        try
        {
            // 1. Load card first (need UpdatedAt for enhanced cache key)
            var card = await _db.DashboardCards.FindAsync(new object[] { cardId }, ct);
            if (card == null || !card.AssistantEnabled)
            {
                sw.Stop();
                await LogAndUpdateStats(cardId, mode, depthLevel, dataScopeLabel,
                    isFullDataReached, wasCached, sw.ElapsedMilliseconds, "CARD_NOT_FOUND");
                return new CardInsightResponse
                {
                    Success = false,
                    ErrorMessage = "البطاقة غير موجودة أو المساعد غير مفعّل."
                };
            }

            // 2. Check cache with enhanced key (includes PromptVersion + UpdatedAt)
            var cacheKey = BuildCacheKey(cardId, mode, depthLevel, card.UpdatedAt);
            var cachedContent = _memoryCache.Get<string>(cacheKey);
            if (cachedContent != null)
            {
                wasCached = true;
                sw.Stop();
                await LogAndUpdateStats(cardId, mode, depthLevel, dataScopeLabel,
                    isFullDataReached, wasCached, sw.ElapsedMilliseconds, null);
                return new CardInsightResponse
                {
                    Content = cachedContent,
                    Success = true,
                    IsFullDataReached = isFullDataReached,
                    HasDeeperData = hasDeeperData,
                    DepthLevel = depthLevel,
                    DepthLabel = dataScopeLabel
                };
            }

            // 3. Build prompt
            var systemPrompt = GeneralPrompt;
            if (!string.IsNullOrWhiteSpace(card.AssistantPrompt))
                systemPrompt = $"{GeneralPrompt}\n\nتعليمات خاصة لهذه البطاقة:\n{card.AssistantPrompt}";

            // 4. Build user message — use rich data when summary is available
            string userMessage;
            if (summary != null && summary.CurrentValue.HasValue)
            {
                userMessage = BuildRichUserMessage(card, mode, depthLevel, summary);
            }
            else
            {
                userMessage = BuildUserMessage(card, mode, depthLevel);
            }

            // 5. Add raw query data (only in deep mode, with depth-based filtering)
            if (mode == "deep")
            {
                var rawData = await BuildRawDataMessage(card, depthLevel, ct);
                if (!string.IsNullOrEmpty(rawData))
                {
                    userMessage += $"\n\n{rawData}";
                }
            }

            var request = new AIAssistantRequest
            {
                SystemPrompt = systemPrompt,
                UserMessage = userMessage,
                CardAssistantPrompt = card.AssistantPrompt
            };

            var aiResponse = await _aiProvider.SendAsync(request, ct);
            sw.Stop();

            if (aiResponse.Success)
            {
                // Cache successful response with enhanced key
                _memoryCache.Set(cacheKey, aiResponse.Content, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSize(1));
            }
            else
            {
                errorCode = "AI_ERROR";
            }

            await LogAndUpdateStats(cardId, mode, depthLevel, dataScopeLabel,
                isFullDataReached, wasCached, sw.ElapsedMilliseconds, errorCode);

            return new CardInsightResponse
            {
                Content = aiResponse.Content,
                Success = aiResponse.Success,
                ErrorMessage = aiResponse.ErrorMessage,
                IsFullDataReached = isFullDataReached,
                HasDeeperData = hasDeeperData,
                DepthLevel = depthLevel,
                DepthLabel = dataScopeLabel
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            errorCode = "EXCEPTION";
            _logger.LogError(ex, "Error in AnalyzeCardWithCacheAsync for card {CardId}", cardId);
            await LogAndUpdateStats(cardId, mode, depthLevel, dataScopeLabel,
                isFullDataReached, wasCached, sw.ElapsedMilliseconds, errorCode);
            return new CardInsightResponse { Success = false, ErrorMessage = "حدث خطأ غير متوقع." };
        }
    }

    /// <summary>
    /// Builds an enhanced cache key that includes PromptVersion and card UpdatedAt.
    /// This ensures cache is invalidated when the prompt version changes or card data is updated.
    /// </summary>
    private string BuildCacheKey(int cardId, string mode, int depthLevel, DateTime? updatedAt)
    {
        return $"ai_assist_{cardId}_{mode}_{depthLevel}_{_options.Value.PromptVersion}_{updatedAt?.Ticks ?? 0}";
    }

    private async Task LogAndUpdateStats(int cardId, string mode, int depthLevel,
        string dataScopeLabel, bool isFullDataReached, bool wasCached,
        long responseTimeMs, string? errorCode)
    {
        try
        {
            await _logService.LogRequestAsync(cardId, mode, depthLevel,
                _options.Value.PromptVersion, false, dataScopeLabel, isFullDataReached, wasCached, responseTimeMs, errorCode);
            await _logService.UpdateUsageStatsAsync(cardId, mode, depthLevel,
                false, responseTimeMs, wasCached);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log assistant request for card {CardId}", cardId);
        }
    }
}
