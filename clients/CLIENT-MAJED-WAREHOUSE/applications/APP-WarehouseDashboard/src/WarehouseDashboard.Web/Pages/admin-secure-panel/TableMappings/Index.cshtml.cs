using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;
using WarehouseDashboard.Web.Services;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin page for managing dynamic Oracle-to-SQL-Server table mappings.
/// CRUD operations on the TableMappings config table plus schema introspection.
/// </summary>
public class TableMappingsModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly OracleSchemaService _oracleSchema;
    private readonly SchemaManagementService _schemaManagement;
    private readonly ILogger<TableMappingsModel> _logger;

    public TableMappingsModel(
        WarehouseDashboardDbContext db,
        OracleSchemaService oracleSchema,
        SchemaManagementService schemaManagement,
        ILogger<TableMappingsModel> logger)
    {
        _db = db;
        _oracleSchema = oracleSchema;
        _schemaManagement = schemaManagement;
        _logger = logger;
    }

    public List<TableMappingConfig> Mappings { get; set; } = new();

    [BindProperty]
    public string OracleSource { get; set; } = string.Empty;

    [BindProperty]
    public string SourceType { get; set; } = "Table";

    [BindProperty]
    public string SqlTargetTable { get; set; } = string.Empty;

    [BindProperty]
    public int EditId { get; set; }

    public bool IsEditing => EditId > 0;

    public string? ToastMessage { get; set; }
    public string ToastType { get; set; } = "success";

    /// <summary>Schema diff result for preview (null until explicitly requested).</summary>
    public SchemaDiffResult? SchemaDiff { get; set; }

    /// <summary>Whether the schema diff preview is visible.</summary>
    public bool ShowSchemaPreview { get; set; }

    public async Task OnGetAsync()
    {
        Mappings = await _db.TableMappings
            .OrderBy(m => m.OracleSource)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ValidateInput())
        {
            await ReloadMappingsAsync();
            return Page();
        }

        // Check uniqueness
        if (await _db.TableMappings.AnyAsync(m => m.OracleSource == OracleSource))
        {
            ToastMessage = $"مصدر Oracle '{OracleSource}' مسجل مسبقاً.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        if (await _db.TableMappings.AnyAsync(m => m.SqlTargetTable == SqlTargetTable))
        {
            ToastMessage = $"جدول SQL Server '{SqlTargetTable}' مسجل مسبقاً.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        var mapping = new TableMappingConfig
        {
            OracleSource = OracleSource,
            SourceType = SourceType,
            SqlTargetTable = SqlTargetTable,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.TableMappings.Add(mapping);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Table mapping added: '{Source}' -> '{Target}'.", OracleSource, SqlTargetTable);

        ToastMessage = "تمت إضافة التعيين بنجاح.";
        ToastType = "success";

        await ReloadMappingsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
        if (EditId <= 0 || !ValidateInput())
        {
            await ReloadMappingsAsync();
            return Page();
        }

        var mapping = await _db.TableMappings.FindAsync(EditId);
        if (mapping is null)
        {
            ToastMessage = "التعيين غير موجود.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        // Check uniqueness (excluding self)
        if (await _db.TableMappings.AnyAsync(m => m.Id != EditId && m.OracleSource == OracleSource))
        {
            ToastMessage = $"مصدر Oracle '{OracleSource}' مسجل مسبقاً.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        if (await _db.TableMappings.AnyAsync(m => m.Id != EditId && m.SqlTargetTable == SqlTargetTable))
        {
            ToastMessage = $"جدول SQL Server '{SqlTargetTable}' مسجل مسبقاً.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        mapping.OracleSource = OracleSource;
        mapping.SourceType = SourceType;
        mapping.SqlTargetTable = SqlTargetTable;
        mapping.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Table mapping {Id} updated: '{Source}' -> '{Target}'.", EditId, OracleSource, SqlTargetTable);

        ToastMessage = "تم تحديث التعيين بنجاح.";
        ToastType = "success";

        await ReloadMappingsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var mapping = await _db.TableMappings.FindAsync(id);
        if (mapping is null)
        {
            ToastMessage = "التعيين غير موجود.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        mapping.IsActive = !mapping.IsActive;
        mapping.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var statusText = mapping.IsActive ? "تفعيل" : "تعطيل";
        _logger.LogInformation("Table mapping {Id} {Status}.", id, statusText);

        ToastMessage = $"تم {statusText} التعيين.";
        ToastType = "success";

        await ReloadMappingsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostPreviewSchemaAsync(int id)
    {
        var mapping = await _db.TableMappings.FindAsync(id);
        if (mapping is null)
        {
            ToastMessage = "التعيين غير موجود.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        try
        {
            SchemaDiff = await _oracleSchema.CompareSchemasAsync(
                mapping.OracleSource,
                mapping.SqlTargetTable,
                mapping.SourceType);

            ShowSchemaPreview = true;
            EditId = id;

            // Re-populate form fields
            OracleSource = mapping.OracleSource;
            SourceType = mapping.SourceType;
            SqlTargetTable = mapping.SqlTargetTable;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to preview schema for mapping {Id}.", id);
            ToastMessage = $"فشل في قراءة المخطط: {ex.Message}";
            ToastType = "error";
        }

        await ReloadMappingsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostApplySchemaAsync(int id)
    {
        var mapping = await _db.TableMappings.FindAsync(id);
        if (mapping is null)
        {
            ToastMessage = "التعيين غير موجود.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        try
        {
            // Check if target table exists
            var tableExists = await _schemaManagement.TableExistsAsync(mapping.SqlTargetTable);

            if (!tableExists)
            {
                // Create the table from Oracle schema
                await _schemaManagement.CreateTableFromOracleAsync(
                    mapping.OracleSource,
                    mapping.SqlTargetTable,
                    mapping.SourceType);

                ToastMessage = $"تم إنشاء الجدول '{mapping.SqlTargetTable}' بنجاح.";
            }
            else
            {
                // Compare and apply changes
                var diff = await _oracleSchema.CompareSchemasAsync(
                    mapping.OracleSource,
                    mapping.SqlTargetTable,
                    mapping.SourceType);

                if (diff.HasChanges)
                {
                    await _schemaManagement.ApplySchemaChangesAsync(mapping.SqlTargetTable, diff);
                    ToastMessage = $"تم تطبيق {diff.Summary} على '{mapping.SqlTargetTable}'.";
                }
                else
                {
                    ToastMessage = "المخطط متطابق. لا توجد تغييرات.";
                }
            }

            ToastType = "success";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply schema for mapping {Id}.", id);
            ToastMessage = $"فشل في تطبيق المخطط: {ex.Message}";
            ToastType = "error";
        }

        await ReloadMappingsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var mapping = await _db.TableMappings.FindAsync(id);
        if (mapping is null)
        {
            ToastMessage = "التعيين غير موجود.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

        _db.TableMappings.Remove(mapping);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Table mapping {Id} deleted: '{Source}' -> '{Target}'.", id, mapping.OracleSource, mapping.SqlTargetTable);

        ToastMessage = "تم حذف التعيين بنجاح.";
        ToastType = "success";

        await ReloadMappingsAsync();
        return Page();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(OracleSource))
        {
            ToastMessage = "مصدر Oracle مطلوب.";
            ToastType = "error";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SqlTargetTable))
        {
            ToastMessage = "جدول SQL Server مطلوب.";
            ToastType = "error";
            return false;
        }

        if (!new[] { "Table", "View", "Query" }.Contains(SourceType))
        {
            ToastMessage = "نوع المصدر غير صالح.";
            ToastType = "error";
            return false;
        }

        return true;
    }

    private async Task ReloadMappingsAsync()
    {
        Mappings = await _db.TableMappings
            .OrderBy(m => m.OracleSource)
            .ToListAsync();
    }
}
