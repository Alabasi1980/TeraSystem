using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WarehouseDashboard.Web.Infrastructure;
using WarehouseDashboard.Web.Models.Dto;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Core orchestration service for AI chat flow.
/// Ties together SavedQueryService, AiQueryContext, and IAIProvider.
/// Created for TASK-AIQ-004.
/// </summary>
public class AiQueryService
{
    private readonly IAIProvider _aiProvider;
    private readonly AiQueryContext _aiContext;
    private readonly SavedQueryService _savedQueryService;
    private readonly ILogger<AiQueryService> _logger;
    private readonly IOptions<AIAssistantOptions> _aiOptions;

    public AiQueryService(
        IAIProvider aiProvider,
        AiQueryContext aiContext,
        SavedQueryService savedQueryService,
        ILogger<AiQueryService> logger,
        IOptions<AIAssistantOptions> aiOptions)
    {
        _aiProvider = aiProvider;
        _aiContext = aiContext;
        _savedQueryService = savedQueryService;
        _logger = logger;
        _aiOptions = aiOptions;
    }

    /// <summary>
    /// Main AI chat flow. Creates or loads a saved query, builds system prompt
    /// with schema context, formats conversation history, sends to AI provider,
    /// saves messages, and extracts any SQL from the response.
    /// </summary>
    public async Task<AiChatResult> ChatAsync(AiChatRequest request, CancellationToken ct = default)
    {
        // ──────────────────────────────────────────────────────────────
        // Step 1 & 2: Determine saved query (create new or load existing)
        // ──────────────────────────────────────────────────────────────
        int savedQueryId;
        List<ConversationMessage> conversations;

        if (!request.SavedQueryId.HasValue)
        {
            var createRequest = new SavedQueryCreate
            {
                Name = $"محادثة جديدة {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
                Description = null,
                SqlQuery = request.CurrentSql ?? string.Empty,
                DataSourceType = request.Source
            };

            var created = await _savedQueryService.CreateAsync(createRequest, ct);
            savedQueryId = created.Id;
            conversations = new List<ConversationMessage>();

            _logger.LogInformation("Created new SavedQuery #{Id} for AI chat", savedQueryId);
        }
        else
        {
            savedQueryId = request.SavedQueryId.Value;
            var existing = await _savedQueryService.GetByIdAsync(savedQueryId, ct);

            if (existing is null)
            {
                _logger.LogWarning("ChatAsync: SavedQuery #{Id} not found", savedQueryId);
                return new AiChatResult
                {
                    Success = false,
                    ErrorMessage = "لم يتم العثور على المحادثة. قد تكون قد حُذفت."
                };
            }

            conversations = existing.Conversations;
        }

        // ──────────────────────────────────────────────────────────────
        // Step 3: Build system prompt with schema context
        // ──────────────────────────────────────────────────────────────
        var systemPrompt = await _aiContext.BuildSystemPromptAsync(request.CurrentSql, request.Source, ct);

        // ──────────────────────────────────────────────────────────────
        // Step 4: Format conversation history
        // ──────────────────────────────────────────────────────────────
        var formattedHistory = await _aiContext.FormatConversationHistoryAsync(conversations, ct);

        // ──────────────────────────────────────────────────────────────
        // Step 5 & 6: Build AI request and send to provider
        // ──────────────────────────────────────────────────────────────
        var aiRequest = new AIAssistantRequest
        {
            SystemPrompt = systemPrompt,
            UserMessage = request.Message,
            Messages = formattedHistory,
            MaxOutputTokens = _aiOptions.Value.MaxOutputTokens
        };

        var stopwatch = Stopwatch.StartNew();
        var response = await _aiProvider.SendAsync(aiRequest, ct);
        stopwatch.Stop();

        var responseTimeMs = response.ResponseTimeMs > 0
            ? response.ResponseTimeMs
            : stopwatch.ElapsedMilliseconds;

        // ──────────────────────────────────────────────────────────────
        // Step 7: Save user message (always, regardless of AI outcome)
        // ──────────────────────────────────────────────────────────────
        await _savedQueryService.AddConversationAsync(
            savedQueryId, "user", request.Message, request.CurrentSql, ct);

        // ──────────────────────────────────────────────────────────────
        // Step 8: AI response successful — save reply, extract SQL
        // ──────────────────────────────────────────────────────────────
        if (response.Success)
        {
            await _savedQueryService.AddConversationAsync(
                savedQueryId, "assistant", response.Content, null, ct);

            var suggestedSql = ExtractSqlFromResponse(response.Content);

            return new AiChatResult
            {
                Success = true,
                Reply = response.Content,
                SuggestedSql = suggestedSql,
                UpdateEditor = suggestedSql is not null,
                SavedQueryId = savedQueryId,
                TokensUsed = response.TokensUsed,
                ResponseTimeMs = responseTimeMs
            };
        }

        // ──────────────────────────────────────────────────────────────
        // Step 9: AI response failed
        // ──────────────────────────────────────────────────────────────
        _logger.LogWarning(
            "ChatAsync: AI provider returned failure for SavedQuery #{Id}: {Error}",
            savedQueryId, response.ErrorMessage);

        return new AiChatResult
        {
            Success = false,
            Reply = response.Content,
            ErrorMessage = response.ErrorMessage ?? "حدث خطأ أثناء التواصل مع المساعد.",
            SavedQueryId = savedQueryId,
            TokensUsed = response.TokensUsed,
            ResponseTimeMs = responseTimeMs
        };
    }

    /// <summary>
    /// Extracts the first SQL block from an AI response.
    /// Searches for content between ```sql and ``` markers.
    /// </summary>
    public string? ExtractSqlFromResponse(string aiResponse)
    {
        if (string.IsNullOrEmpty(aiResponse))
            return null;

        var match = Regex.Match(
            aiResponse,
            @"```sql\s*(.*?)\s*```",
            RegexOptions.Singleline);

        if (match.Success)
        {
            var sql = match.Groups[1].Value.Trim();
            return string.IsNullOrEmpty(sql) ? null : sql;
        }

        return null;
    }

    /// <summary>
    /// Quick SQL suggestion without saving any conversation.
    /// Builds prompt, sends to AI, extracts SQL, returns result.
    /// </summary>
    public async Task<AiChatResult> SuggestSqlAsync(
        string userMessage,
        string? currentSql = null,
        string source = "SqlServer",
        CancellationToken ct = default)
    {
        var systemPrompt = await _aiContext.BuildSystemPromptAsync(currentSql, source, ct);

        var aiRequest = new AIAssistantRequest
        {
            SystemPrompt = systemPrompt,
            UserMessage = userMessage,
            MaxOutputTokens = 2000
        };

        var stopwatch = Stopwatch.StartNew();
        var response = await _aiProvider.SendAsync(aiRequest, ct);
        stopwatch.Stop();

        var responseTimeMs = response.ResponseTimeMs > 0
            ? response.ResponseTimeMs
            : stopwatch.ElapsedMilliseconds;

        if (response.Success)
        {
            var suggestedSql = ExtractSqlFromResponse(response.Content);

            return new AiChatResult
            {
                Success = true,
                Reply = response.Content,
                SuggestedSql = suggestedSql,
                UpdateEditor = suggestedSql is not null,
                SavedQueryId = null,
                TokensUsed = response.TokensUsed,
                ResponseTimeMs = responseTimeMs
            };
        }

        return new AiChatResult
        {
            Success = false,
            ErrorMessage = response.ErrorMessage ?? "حدث خطأ أثناء إنشاء الاقتراح.",
            TokensUsed = response.TokensUsed,
            ResponseTimeMs = responseTimeMs
        };
    }
}

/// <summary>
/// Request DTO for the AI chat flow.
/// </summary>
public class AiChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? CurrentSql { get; set; }
    public int? SavedQueryId { get; set; }
    public string Source { get; set; } = "SqlServer";
}

/// <summary>
/// Result DTO returned from AI chat operations.
/// </summary>
public class AiChatResult
{
    public bool Success { get; set; }
    public string Reply { get; set; } = string.Empty;
    public string? SuggestedSql { get; set; }
    public bool UpdateEditor { get; set; }
    public int? SavedQueryId { get; set; }
    public string? ErrorMessage { get; set; }
    public int TokensUsed { get; set; }
    public long ResponseTimeMs { get; set; }
}
