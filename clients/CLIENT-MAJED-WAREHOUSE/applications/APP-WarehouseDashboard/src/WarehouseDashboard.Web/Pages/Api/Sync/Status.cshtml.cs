using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Pages.Api.Sync;

/// <summary>
/// GET /api/sync/status
/// Lightweight SQL Server connectivity probe used by the public dashboard's
/// connection-status indicator. Runs a trivial <c>SELECT 1</c> against the
/// resolved connection string (password from SQL_PASSWORD env var, never inline).
/// </summary>
public class StatusModel : PageModel
{
    private readonly IConfiguration _config;

    public StatusModel(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
        var connString = ConnectionStringHelper.Resolve(connTemplate);

        var connected = false;
        var detail = string.Empty;

        if (string.IsNullOrWhiteSpace(connString))
        {
            detail = "لم يتم ضبط سلسلة الاتصال (SQL_PASSWORD).";
        }
        else
        {
            try
            {
                await using var conn = new SqlConnection(connString);
                await conn.OpenAsync(cancellationToken);
                await using var cmd = new SqlCommand("SELECT 1", conn);
                await cmd.ExecuteScalarAsync(cancellationToken);
                connected = true;
            }
            catch (Exception ex)
            {
                var password = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;
                detail = ex.Message.Replace("{SQL_PASSWORD}", "***", StringComparison.Ordinal);
                if (password.Length > 0)
                {
                    detail = detail.Replace(password, "***", StringComparison.Ordinal);
                }
            }
        }

        return new JsonResult(new
        {
            connected,
            detail,
            checkedAt = DateTime.UtcNow
        });
    }
}
