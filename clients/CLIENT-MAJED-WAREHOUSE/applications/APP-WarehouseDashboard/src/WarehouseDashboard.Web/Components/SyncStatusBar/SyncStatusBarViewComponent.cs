using Microsoft.AspNetCore.Mvc;

namespace WarehouseDashboard.Web.Components;

/// <summary>
/// Sync Status Bar — compact, self-contained component shown at the top of the
/// public Dashboard. Renders live sync state (from GET /api/sync/status) and a
/// Manual Refresh button (POST /api/sync/trigger). All behaviour lives in the
/// browser via fetch (the Sync API has no auth in Phase 1).
///
/// <para>
/// Include from any Razor page with:
/// <c>@await Component.InvokeAsync("SyncStatusBar")</c>
/// </para>
/// </summary>
public class SyncStatusBarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
