using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Sync Settings admin page — read and update the singleton <see cref="SyncSetting"/> row.
/// Controls sync interval and auto-sync toggle. Also fetches live data from the Sync API.
/// </summary>
public class SyncSettingsModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ILogger<SyncSettingsModel> _logger;
    private readonly IConfiguration _configuration;
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

    public SyncSettingsModel(WarehouseDashboardDbContext db, ILogger<SyncSettingsModel> logger, IConfiguration configuration)
    {
        _db = db;
        _logger = logger;
        _configuration = configuration;
    }

    [BindProperty]
    public int IntervalMinutes { get; set; } = 30;

    [BindProperty]
    public bool IsAutoSyncEnabled { get; set; }

    public DateTime? LastSyncTimestamp { get; set; }

    // API data
    public SyncInfo? SyncStatus { get; set; }
    public SyncConfigInfo? SyncConfig { get; set; }
    public List<MappingItem> Mappings { get; set; } = new();
    public string? LoadError { get; set; }

    // Computed
    public int ActiveMappingsCount => Mappings.Count(m => m.IsActive);
    public int TotalMappingsCount => Mappings.Count;
    public bool HasSyncError => SyncStatus?.LastStatus == "error";
    public string? LastSyncStatusText => SyncStatus?.LastStatus switch
    {
        "success" => "نجاح",
        "error" => "فشل",
        _ => null
    };

    public string? ToastMessage { get; set; }
    public string ToastType { get; set; } = "success";

    public async Task OnGetAsync()
    {
        // 1. Load from DB (existing code)
        var setting = await GetOrCreateSettingAsync();
        IntervalMinutes = setting.IntervalMinutes;
        IsAutoSyncEnabled = setting.IsAutoSyncEnabled;
        LastSyncTimestamp = setting.LastSyncTimestamp;

        // 2. Fetch from Sync API in parallel
        var apiBase = _configuration["SyncApi:BaseUrl"] ?? _configuration.GetValue<string>("SyncApiBaseUrl") ?? string.Empty;
        var client = _httpClient;

        var statusTask = FetchAsync<SyncInfo>(client, $"{apiBase}/api/sync/status");
        var configTask = FetchAsync<SyncConfigInfo>(client, $"{apiBase}/api/sync/config");
        var mappingsTask = FetchAsync<List<MappingItem>>(client, $"{apiBase}/api/sync/mappings");

        await Task.WhenAll(statusTask, configTask, mappingsTask);

        SyncStatus = statusTask.Result;
        SyncConfig = configTask.Result;
        if (mappingsTask.Result is { } mappings)
            Mappings = mappings;

        if (statusTask.Exception != null || configTask.Exception != null || mappingsTask.Exception != null)
        {
            LoadError = "تعذر الاتصال بخدمة المزامنة.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validate interval range
        if (IntervalMinutes < 1 || IntervalMinutes > 1440)
        {
            IntervalMinutes = 30;
        }

        try
        {
            var setting = await GetOrCreateSettingAsync();
            setting.IntervalMinutes = IntervalMinutes;
            setting.IsAutoSyncEnabled = IsAutoSyncEnabled;

            await _db.SaveChangesAsync();

            ToastMessage = "تم حفظ الإعدادات بنجاح.";
            ToastType = "success";

            // Re-read to show updated LastSyncTimestamp
            LastSyncTimestamp = setting.LastSyncTimestamp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save sync settings.");
            ToastMessage = "تعذر حفظ الإعدادات. يرجى المحاولة لحقاً.";
            ToastType = "error";
        }

        return Page();
    }

    private async Task<SyncSetting> GetOrCreateSettingAsync()
    {
        // Use FirstOrDefault — the table has a single row (singleton),
        // and the Id column is auto-increment (identity), so we cannot
        // hardcode Id=1 when inserting.
        var setting = await _db.SyncSettings.FirstOrDefaultAsync();
        if (setting is not null)
        {
            return setting;
        }

        // Row does not exist — create with defaults (let DB assign Id)
        setting = new SyncSetting
        {
            IntervalMinutes = 30,
            IsAutoSyncEnabled = false,
            LastSyncTimestamp = null
        };
        _db.SyncSettings.Add(setting);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Another request may have created the row concurrently; re-read
            setting = await _db.SyncSettings.FirstOrDefaultAsync()
                      ?? throw new InvalidOperationException("SyncSettings row could not be created or found.");
        }

        return setting;
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

public class MappingItem
{
    public int Id { get; set; }
    public string OracleSource { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string SqlTargetTable { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
