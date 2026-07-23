using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Pages.Api.SqlViews;

/// <summary>
/// GET /api/sqlviews — returns list of all SQL Server Views in the warehouse database.
/// Used by the Card Builder wizard Step 2 (عرض SQL (View) source type).
/// Reads from INFORMATION_SCHEMA.VIEWS to discover all user-defined views.
/// </summary>
public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var connectionString = ConnectionStringHelper.Resolve(_configuration.GetConnectionString("SqlServer"));
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return new JsonResult(Array.Empty<object>());
        }

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT 
                    SCHEMA_NAME(schema_id) AS [Schema],
                    name AS [Name],
                    OBJECT_DEFINITION(object_id) AS [Definition],
                    create_date AS [CreatedAt],
                    modify_date AS [UpdatedAt]
                FROM sys.views
                WHERE is_ms_shipped = 0
                ORDER BY SCHEMA_NAME(schema_id), name
                """;

            var views = new List<object>();
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                views.Add(new
                {
                    schema = reader.GetString(reader.GetOrdinal("Schema")),
                    name = reader.GetString(reader.GetOrdinal("Name")),
                    definition = reader.IsDBNull(reader.GetOrdinal("Definition"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Definition")),
                    created = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    updated = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return new JsonResult(views);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list SQL Server views.");
            HttpContext.Response.StatusCode = 500;
            return new JsonResult(new { error = "Failed to read SQL Server views." });
        }
    }
}
