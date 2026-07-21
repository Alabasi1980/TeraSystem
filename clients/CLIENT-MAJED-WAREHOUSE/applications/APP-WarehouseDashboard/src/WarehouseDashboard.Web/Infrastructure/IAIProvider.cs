namespace WarehouseDashboard.Web.Infrastructure;

public interface IAIProvider
{
    Task<AIAssistantResponse> SendAsync(AIAssistantRequest request, CancellationToken ct = default);
}
