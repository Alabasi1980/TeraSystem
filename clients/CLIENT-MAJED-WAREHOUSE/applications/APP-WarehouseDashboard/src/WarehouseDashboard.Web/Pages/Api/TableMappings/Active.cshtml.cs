using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Pages.Api.TableMappings;

/// <summary>
/// GET /api/tablemappings/active
/// Returns list of active <see cref="TableMappingConfig"/> (IsActive = true) for the Card Builder Step 2 dropdown.
/// </summary>
public class ActiveModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;

    public ActiveModel(WarehouseDashboardDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var mappings = await _db.TableMappings
            .AsNoTracking()
            .Include(t => t.ColumnMappings)
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var result = mappings.Select(t => new
            {
                id = t.Id,
                name = t.Name,
                oracleSource = t.OracleSource,
                sourceType = t.SourceType,
                sqlTargetTable = t.SqlTargetTable,
                numericTextColumns = (t.ColumnMappings ?? new List<ColumnMapping>())
                    .Where(cm => cm.IsNumericText && !cm.IsExcluded)
                    .Select(cm => cm.SqlColumnName)
                    .ToList()
            })
            .ToList();

        return new JsonResult(result);
    }
}
