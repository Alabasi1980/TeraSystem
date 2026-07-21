namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Immutable log of every AI assistant interaction, created at request time.
/// Used for analytics, debugging, and performance monitoring (TASK-AI-E01).
/// </summary>
public class AssistantInsightLog
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public string Mode { get; set; } = string.Empty;   // "explain" / "deep"
    public int DepthLevel { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string PromptVersion { get; set; } = "1.0";
    public bool CardPromptUsed { get; set; }
    public string? DataScopeLabel { get; set; }
    public bool IsFullDataReached { get; set; }
    public bool WasCached { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? ErrorCode { get; set; }

    public DashboardCard? Card { get; set; }
}
