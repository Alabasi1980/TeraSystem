namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Individual messages within an AI Assistant conversation.
/// Each message is linked to an optional SavedQuery and captures the SQL snapshot at the time of the message.
/// </summary>
public class AiConversation
{
    public long Id { get; set; }
    public int? SavedQueryId { get; set; }
    public string Role { get; set; } = string.Empty;  // 'user', 'assistant', 'system'
    public string Message { get; set; } = string.Empty;
    public string? SqlSnapshot { get; set; }  // SQL snapshot at time of message
    public DateTime CreatedAt { get; set; }

    public SavedQuery? SavedQuery { get; set; }
}
