using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Sync Settings admin page — read and update the singleton <see cref="SyncSetting"/> row (Id = 1).
/// Controls sync interval and auto-sync toggle.
/// </summary>
public class SyncSettingsModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ILogger<SyncSettingsModel> _logger;

    public SyncSettingsModel(WarehouseDashboardDbContext db, ILogger<SyncSettingsModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    [BindProperty]
    public int IntervalMinutes { get; set; } = 30;

    [BindProperty]
    public bool IsAutoSyncEnabled { get; set; }

    public DateTime? LastSyncTimestamp { get; set; }

    public string? ToastMessage { get; set; }
    public string ToastType { get; set; } = "success";

    public async Task OnGetAsync()
    {
        var setting = await GetOrCreateSettingAsync();
        IntervalMinutes = setting.IntervalMinutes;
        IsAutoSyncEnabled = setting.IsAutoSyncEnabled;
        LastSyncTimestamp = setting.LastSyncTimestamp;
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
            ToastMessage = "تعذر حفظ الإعدادات. يرجى المحاولة لاحقاً.";
            ToastType = "error";
        }

        return Page();
    }

    private async Task<SyncSetting> GetOrCreateSettingAsync()
    {
        var setting = await _db.SyncSettings.FindAsync(1);
        if (setting is not null)
        {
            return setting;
        }

        // Row does not exist — create with defaults
        setting = new SyncSetting
        {
            Id = 1,
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
            setting = await _db.SyncSettings.FindAsync(1)
                      ?? throw new InvalidOperationException("SyncSettings row (Id=1) could not be created or found.");
        }

        return setting;
    }
}
