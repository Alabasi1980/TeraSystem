namespace WarehouseDashboard.Web.Models;

/// <summary>
/// Singleton admin password row (Id = 1) holding a BCrypt password hash.
/// Stored in the <c>AdminPassword</c> config table (see 06_DATA_MODEL_PREPARATION.md §1.4).
/// </summary>
public class AdminPassword
{
    /// <summary>Primary key (identity). The single row uses Id = 1.</summary>
    public int Id { get; set; }

    /// <summary>BCrypt hash of the admin password (max 500 chars).</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Record update timestamp (DB default GETUTCDATE()).</summary>
    public DateTime UpdatedAt { get; set; }
}
