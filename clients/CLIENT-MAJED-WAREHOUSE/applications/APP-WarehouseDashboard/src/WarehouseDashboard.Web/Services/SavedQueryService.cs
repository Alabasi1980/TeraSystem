using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;
using WarehouseDashboard.Web.Models.Dto;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Service for CRUD operations on SavedQueries and AiConversations (via EF Core).
/// Created for TASK-AIQ-002.
/// </summary>
public class SavedQueryService
{
    private readonly ILogger<SavedQueryService> _logger;
    private readonly WarehouseDashboardDbContext _db;

    public SavedQueryService(
        ILogger<SavedQueryService> logger,
        WarehouseDashboardDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Returns all saved queries ordered by UpdatedAt DESC.
    /// Supports optional search filter on Name.
    /// Includes conversation count and a 100-char SQL preview.
    /// </summary>
    public async Task<List<SavedQueryListItem>> ListAsync(string? search, CancellationToken ct = default)
    {
        var query = _db.SavedQueries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(q => q.Name.Contains(search));
        }

        return await query
            .OrderByDescending(q => q.UpdatedAt)
            .Select(q => new SavedQueryListItem
            {
                Id = q.Id,
                Name = q.Name,
                Description = q.Description,
                SqlPreview = q.SqlQuery.Length > 100
                    ? q.SqlQuery.Substring(0, 100)
                    : q.SqlQuery,
                DataSourceType = q.DataSourceType,
                UpdatedAt = q.UpdatedAt,
                ConversationCount = q.Conversations.Count
            })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Gets a single saved query with its last 50 conversations ordered by CreatedAt ASC.
    /// Returns null if not found.
    /// </summary>
    public async Task<SavedQueryFull?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var savedQuery = await _db.SavedQueries
            .Include(q => q.Conversations
                .OrderByDescending(c => c.CreatedAt)
                .Take(50))
            .FirstOrDefaultAsync(q => q.Id == id, ct);

        if (savedQuery is null) return null;

        return new SavedQueryFull
        {
            Id = savedQuery.Id,
            Name = savedQuery.Name,
            Description = savedQuery.Description,
            SqlQuery = savedQuery.SqlQuery,
            DataSourceType = savedQuery.DataSourceType,
            CreatedAt = savedQuery.CreatedAt,
            UpdatedAt = savedQuery.UpdatedAt,
            Conversations = savedQuery.Conversations
                .OrderBy(c => c.CreatedAt)
                .Select(c => new ConversationMessage
                {
                    Id = c.Id,
                    Role = c.Role,
                    Message = c.Message,
                    SqlSnapshot = c.SqlSnapshot,
                    CreatedAt = c.CreatedAt
                })
                .ToList()
        };
    }

    /// <summary>
    /// Creates a new saved query with timestamps and returns the full object.
    /// </summary>
    public async Task<SavedQueryFull> CreateAsync(SavedQueryCreate request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var entity = new SavedQuery
        {
            Name = request.Name,
            Description = request.Description,
            SqlQuery = request.SqlQuery,
            DataSourceType = request.DataSourceType,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.SavedQueries.Add(entity);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Created SavedQuery #{Id}: {Name}", entity.Id, entity.Name);

        return new SavedQueryFull
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            SqlQuery = entity.SqlQuery,
            DataSourceType = entity.DataSourceType,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// Updates Name, Description, SqlQuery and sets UpdatedAt = UtcNow.
    /// Returns null if the entity is not found.
    /// </summary>
    public async Task<SavedQueryFull?> UpdateAsync(int id, SavedQueryUpdate request, CancellationToken ct = default)
    {
        var entity = await _db.SavedQueries.FirstOrDefaultAsync(q => q.Id == id, ct);

        if (entity is null) return null;

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.SqlQuery = request.SqlQuery;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Updated SavedQuery #{Id}", entity.Id);

        return new SavedQueryFull
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            SqlQuery = entity.SqlQuery,
            DataSourceType = entity.DataSourceType,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// Deletes a saved query (conversations cascade-deleted by FK).
    /// Returns true if deleted, false if not found.
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.SavedQueries.FindAsync(new object[] { id }, ct);

        if (entity is null) return false;

        _db.SavedQueries.Remove(entity);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted SavedQuery #{Id}", id);
        return true;
    }

    /// <summary>
    /// Creates a new AiConversation linked to the given saved query.
    /// No return value.
    /// </summary>
    public async Task AddConversationAsync(int savedQueryId, string role, string message, string? sqlSnapshot, CancellationToken ct = default)
    {
        var conversation = new AiConversation
        {
            SavedQueryId = savedQueryId,
            Role = role,
            Message = message,
            SqlSnapshot = sqlSnapshot,
            CreatedAt = DateTime.UtcNow
        };

        _db.AiConversations.Add(conversation);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes all conversations for a saved query but keeps the query itself.
    /// Returns true if the query existed and conversations were cleared, false if not found.
    /// </summary>
    public async Task<bool> ClearConversationAsync(int savedQueryId, CancellationToken ct = default)
    {
        var exists = await _db.SavedQueries.AnyAsync(q => q.Id == savedQueryId, ct);

        if (!exists) return false;

        await _db.AiConversations
            .Where(c => c.SavedQueryId == savedQueryId)
            .ExecuteDeleteAsync(ct);

        _logger.LogInformation("Cleared conversations for SavedQuery #{SavedQueryId}", savedQueryId);
        return true;
    }
}
