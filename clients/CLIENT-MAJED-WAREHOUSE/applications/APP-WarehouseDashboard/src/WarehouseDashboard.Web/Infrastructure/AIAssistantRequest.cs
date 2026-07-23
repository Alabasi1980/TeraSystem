using System.Collections.Generic;

namespace WarehouseDashboard.Web.Infrastructure;

public class AIAssistantRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public string? CardAssistantPrompt { get; set; }
    public int MaxOutputTokens { get; set; } = 300;

    /// <summary>Optional conversation history to inject between system prompt and user message.</summary>
    public List<object>? Messages { get; set; }
}

public class AIAssistantResponse
{
    public string Content { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TokensUsed { get; set; }
    public long ResponseTimeMs { get; set; }
}

public class CardInsightResponse
{
    public string Content { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsFullDataReached { get; set; }
    public bool HasDeeperData { get; set; }
    public int DepthLevel { get; set; }
    public string DepthLabel { get; set; } = string.Empty;
    public bool HasDateColumn { get; set; }
    public List<DrillLevelInfo>? AvailableDrillLevels { get; set; }
}

/// <summary>معلومات مستوى تعمق لعرضه في القائمة الجانبية.</summary>
public class DrillLevelInfo
{
    public int Level { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string TargetChartType { get; set; } = string.Empty;
}
