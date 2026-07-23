namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Saved SQL queries for the AI Assistant.
/// Each saved query can have multiple AI conversations attached to it.
/// </summary>
public class SavedQuery
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SqlQuery { get; set; } = string.Empty;
    public string DataSourceType { get; set; } = "SqlServer";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<AiConversation> Conversations { get; set; } = new List<AiConversation>();
}
