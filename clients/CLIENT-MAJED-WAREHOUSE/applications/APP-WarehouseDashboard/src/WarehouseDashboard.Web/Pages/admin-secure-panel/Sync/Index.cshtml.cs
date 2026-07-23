using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel.Sync;

/// <summary>
/// Page model for the unified Sync Dashboard (/admin-secure-panel/Sync).
/// Loads mappings, engine status, and config from the Sync API on GET.
/// </summary>
public class SyncDashboardModel : PageModel
{
    private readonly IConfiguration _configuration;
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

    public SyncDashboardModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>Base URL of the Sync API, from config or default.</summary>
    public string ApiBaseUrl { get; set; } = string.Empty;

    /// <summary>Active table mappings loaded from the API.</summary>
    public List<MappingItem> Mappings { get; set; } = new();

    /// <summary>Current engine status from GET /api/sync/status.</summary>
    public SyncInfo? SyncStatus { get; set; }

    /// <summary>Current sync configuration from GET /api/sync/config.</summary>
    public SyncConfigInfo? SyncConfig { get; set; }

    /// <summary>Error message if any API call failed during page load.</summary>
    public string? LoadError { get; set; }

    public async Task OnGetAsync()
    {
        ApiBaseUrl = _configuration["SyncApi:BaseUrl"] ?? _configuration.GetValue<string>("SyncApiBaseUrl") ?? string.Empty;

        var client = _httpClient;

        // Load mappings, status, and config in parallel
        var mappingsTask = FetchAsync<List<MappingItem>>(client, $"{ApiBaseUrl}/api/sync/mappings");
        var statusTask = FetchAsync<SyncInfo>(client, $"{ApiBaseUrl}/api/sync/status");
        var configTask = FetchAsync<SyncConfigInfo>(client, $"{ApiBaseUrl}/api/sync/config");

        await Task.WhenAll(mappingsTask, statusTask, configTask);

        if (mappingsTask.Result is { } mappings)
            Mappings = mappings;

        SyncStatus = statusTask.Result;
        SyncConfig = configTask.Result;

        if (mappingsTask.Exception != null || statusTask.Exception != null || configTask.Exception != null)
        {
            LoadError = "تعذر تحميل بيانات المزامنة. تأكد من أن خدمة API شغالة.";
        }
    }

    private static async Task<T?> FetchAsync<T>(HttpClient client, string url) where T : class
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }
        catch
        {
            return null;
        }
    }

    // ─── Helper models ─────────────────────────────────────────────

    public class MappingItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OracleSource { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string SqlTargetTable { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastSyncAt { get; set; }
        public int SyncRecordCount { get; set; }
    }

    public class SyncInfo
    {
        public bool IsRunning { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public string? LastStatus { get; set; }
        public int LastRecordCount { get; set; }
    }

    public class SyncConfigInfo
    {
        public int IntervalMinutes { get; set; } = 30;
        public bool IsAutoSyncEnabled { get; set; }
        public DateTime? LastSyncTimestamp { get; set; }
    }
}
