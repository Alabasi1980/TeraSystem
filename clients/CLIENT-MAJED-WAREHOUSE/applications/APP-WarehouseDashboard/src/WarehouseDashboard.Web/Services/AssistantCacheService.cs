using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace WarehouseDashboard.Web.Services;

public class AssistantCacheService
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(10);

    public AssistantCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>Builds a cache key from card/mode/depth.</summary>
    public static string BuildCacheKey(int cardId, string mode, int depthLevel)
    {
        return $"ai_assist_{cardId}_{mode}_{depthLevel}";
    }

    /// <summary>Attempts to get a cached response.</summary>
    public bool TryGetCached(int cardId, string mode, int depthLevel, out string? content)
    {
        content = _cache.Get<string>(BuildCacheKey(cardId, mode, depthLevel));
        return content != null;
    }

    /// <summary>Stores a response in cache.</summary>
    public void Set(int cardId, string mode, int depthLevel, string content, TimeSpan? duration = null)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(duration ?? DefaultCacheDuration)
            .SetSize(1);
        _cache.Set(BuildCacheKey(cardId, mode, depthLevel), content, options);
    }

    /// <summary>Invalidates all cached responses for a card.</summary>
    public void InvalidateCard(int cardId)
    {
        // IMemoryCache doesn't support prefix invalidation easily.
        // For now, cache entries expire naturally after 10 minutes.
        // Full invalidation can be added later if needed.
    }
}
