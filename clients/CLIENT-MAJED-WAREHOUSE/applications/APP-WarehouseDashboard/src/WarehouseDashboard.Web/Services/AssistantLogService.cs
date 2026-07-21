using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Services;

public class AssistantLogService
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ILogger<AssistantLogService> _logger;

    public AssistantLogService(WarehouseDashboardDbContext db, ILogger<AssistantLogService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Records a completed AI request to the insight log table.</summary>
    public async Task LogRequestAsync(int cardId, string mode, int depthLevel,
        string promptVersion, bool cardPromptUsed, string? dataScopeLabel,
        bool isFullDataReached, bool wasCached, long responseTimeMs, string? errorCode)
    {
        var entry = new AssistantInsightLog
        {
            CardId = cardId,
            Mode = mode,
            DepthLevel = depthLevel,
            RequestedAt = DateTime.UtcNow,
            PromptVersion = promptVersion,
            CardPromptUsed = cardPromptUsed,
            DataScopeLabel = dataScopeLabel,
            IsFullDataReached = isFullDataReached,
            WasCached = wasCached,
            ResponseTimeMs = responseTimeMs,
            ErrorCode = errorCode
        };
        _db.AssistantInsightLogs.Add(entry);
        await _db.SaveChangesAsync();
    }

    /// <summary>Updates or creates usage stats for a card.</summary>
    public async Task UpdateUsageStatsAsync(int cardId, string mode, int depthLevel,
        bool isDeepenClick, long responseTimeMs, bool wasCached)
    {
        var stats = await _db.AssistantUsageStats
            .FirstOrDefaultAsync(s => s.CardId == cardId);

        if (stats == null)
        {
            stats = new AssistantUsageStat { CardId = cardId };
            _db.AssistantUsageStats.Add(stats);
        }

        stats.TotalRequests++;
        if (mode == "explain") stats.ExplainRequests++;
        else if (mode == "deep") stats.DeepRequests++;
        if (isDeepenClick) stats.DeepenClicks++;
        if (depthLevel > stats.MostUsedDepth) stats.MostUsedDepth = depthLevel;
        stats.LastUsedAt = DateTime.UtcNow;

        // Rolling average
        if (stats.TotalRequests == 1)
            stats.AverageResponseTimeMs = responseTimeMs;
        else
            stats.AverageResponseTimeMs = (stats.AverageResponseTimeMs * (stats.TotalRequests - 1) + responseTimeMs) / stats.TotalRequests;

        if (wasCached) stats.CacheHitCount++;
        else stats.CacheMissCount++;

        await _db.SaveChangesAsync();
    }
}
