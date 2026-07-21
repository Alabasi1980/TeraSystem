namespace WarehouseDashboard.Web.Infrastructure;

public class AIAssistantOptions
{
    public const string SectionName = "AIAssistant";
    
    public string ProviderName { get; set; } = "OpenCodeGo";
    public string BaseUrl { get; set; } = "https://opencode.ai/zen/go/v1/chat/completions";
    public string ModelId { get; set; } = "deepseek-v4-flash";
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxOutputTokens { get; set; } = 300;
    public string PromptVersion { get; set; } = "1.0";
}
