// ======================================================================
// DTOs for SavedQuery service (TASK-AIQ-002)
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace WarehouseDashboard.Web.Models.Dto;

/// <summary>Saved query list item with preview and conversation count.</summary>
public class SavedQueryListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SqlPreview { get; set; } = string.Empty;  // first 100 chars
    public string DataSourceType { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public int ConversationCount { get; set; }
}

/// <summary>Full saved query with conversations.</summary>
public class SavedQueryFull
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SqlQuery { get; set; } = string.Empty;
    public string DataSourceType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ConversationMessage> Conversations { get; set; } = new();
}

/// <summary>Single conversation message within a saved query.</summary>
public class ConversationMessage
{
    public long Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? SqlSnapshot { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Request model for creating a saved query.</summary>
public class SavedQueryCreate
{
    [Required] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required] public string SqlQuery { get; set; } = string.Empty;
    public string DataSourceType { get; set; } = "SqlServer";
}

/// <summary>Request model for updating a saved query.</summary>
public class SavedQueryUpdate
{
    [Required] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required] public string SqlQuery { get; set; } = string.Empty;
}
