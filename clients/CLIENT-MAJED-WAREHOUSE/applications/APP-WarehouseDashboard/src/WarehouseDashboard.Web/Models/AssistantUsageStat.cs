namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Aggregated usage statistics per dashboard card, updated on each assistant
/// interaction (TASK-AI-E01). One row per card (CardId unique).
/// </summary>
public class AssistantUsageStat
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public int TotalRequests { get; set; }
    public int ExplainRequests { get; set; }
    public int DeepRequests { get; set; }
    public int DeepenClicks { get; set; }
    public int MostUsedDepth { get; set; } = 1;
    public DateTime? LastUsedAt { get; set; }
    public long AverageResponseTimeMs { get; set; }
    public int CacheHitCount { get; set; }
    public int CacheMissCount { get; set; }

    public DashboardCard? Card { get; set; }
}
