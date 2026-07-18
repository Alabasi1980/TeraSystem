using System.Text.Json;
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
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public string SqlTargetTable { get; set; } = string.Empty;

    [BindProperty]
    public int EditId { get; set; }

    /// <summary>JSON-serialized array of column mapping overrides from the wizard.</summary>
    [BindProperty]
    public string? ColumnMappingsJson { get; set; }

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
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnGetMappingAsync(int id)
    {
        var mapping = await _db.TableMappings
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new
            {
                editId = m.Id,
                name = m.Name,
                oracleSource = m.OracleSource,
                sourceType = m.SourceType,
                sqlTargetTable = m.SqlTargetTable
            })
            .FirstOrDefaultAsync();

        if (mapping is null)
        {
            return NotFound();
        }

        // Load column mappings for edit mode
        var columnMappings = await _db.ColumnMappings
            .AsNoTracking()
            .Where(cm => cm.TableMappingConfigId == id)
            .OrderBy(cm => cm.SortOrder)
            .Select(cm => new
            {
                oracleColumnName = cm.OracleColumnName,
                sqlColumnName = cm.SqlColumnName,
                sqlDataType = cm.SqlDataType,
                sqlMaxLength = cm.SqlMaxLength,
                sqlPrecision = cm.SqlPrecision,
                sqlScale = cm.SqlScale,
                isNullable = cm.IsNullable,
                isExcluded = cm.IsExcluded,
                defaultValue = cm.DefaultValue,
                sortOrder = cm.SortOrder
            })
            .ToListAsync();

        return new JsonResult(new
        {
            mapping.editId,
            mapping.name,
            mapping.oracleSource,
            mapping.sourceType,
            mapping.sqlTargetTable,
            columnMappings
        });
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ValidateInput())
        {
            await ReloadMappingsAsync();
            return Page();
        }

        // Check uniqueness
        if (await _db.TableMappings.AnyAsync(m => m.Name == Name))
        {
            ToastMessage = $"اسم التعيين '{Name}' مسجل مسبقاً.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

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
            Name = Name,
            OracleSource = OracleSource,
            SourceType = SourceType,
            SqlTargetTable = SqlTargetTable,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.TableMappings.Add(mapping);
        await _db.SaveChangesAsync();

        // Save column mappings from wizard JSON
        await SaveColumnMappingsAsync(mapping.Id, ColumnMappingsJson);

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

        // Remove existing column mappings — they will be replaced by wizard JSON
        var existingMappings = await _db.ColumnMappings
            .Where(cm => cm.TableMappingConfigId == EditId)
            .ToListAsync();
        _db.ColumnMappings.RemoveRange(existingMappings);
        // Commit deletions first so the unique index does not conflict with new inserts
        await _db.SaveChangesAsync();

        // Check uniqueness (excluding self)
        if (await _db.TableMappings.AnyAsync(m => m.Id != EditId && m.Name == Name))
        {
            ToastMessage = $"اسم التعيين '{Name}' مسجل مسبقاً.";
            ToastType = "error";
            await ReloadMappingsAsync();
            return Page();
        }

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

        mapping.Name = Name;
        mapping.OracleSource = OracleSource;
        mapping.SourceType = SourceType;
        mapping.SqlTargetTable = SqlTargetTable;
        mapping.UpdatedAt = DateTime.UtcNow;

        // Save column mappings from wizard JSON (replaces old ones)
        await SaveColumnMappingsAsync(EditId, ColumnMappingsJson);

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
            // Load column overrides for accurate diff
            var columnOverrides = await _db.ColumnMappings
                .Where(cm => cm.TableMappingConfigId == id)
                .ToListAsync();

            SchemaDiff = await _oracleSchema.CompareSchemasAsync(
                mapping.OracleSource,
                mapping.SqlTargetTable,
                mapping.SourceType,
                columnOverrides);

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
            // Load column overrides for schema generation
            var columnOverrides = await _db.ColumnMappings
                .Where(cm => cm.TableMappingConfigId == id)
                .ToListAsync();

            // Check if target table exists
            var tableExists = await _schemaManagement.TableExistsAsync(mapping.SqlTargetTable);

            if (!tableExists)
            {
                // Create the table from Oracle schema (respects overrides)
                await _schemaManagement.CreateTableFromOracleAsync(
                    mapping.OracleSource,
                    mapping.SqlTargetTable,
                    mapping.SourceType,
                    columnOverrides);

                ToastMessage = $"تم إنشاء الجدول '{mapping.SqlTargetTable}' بنجاح.";
            }
            else
            {
                // Compare and apply changes (respects overrides)
                var diff = await _oracleSchema.CompareSchemasAsync(
                    mapping.OracleSource,
                    mapping.SqlTargetTable,
                    mapping.SourceType,
                    columnOverrides);

                if (diff.HasChanges)
                {
                    await _schemaManagement.ApplySchemaChangesAsync(mapping.SqlTargetTable, diff, columnOverrides);
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
        if (string.IsNullOrWhiteSpace(Name))
        {
            ToastMessage = "اسم التعيين مطلوب.";
            ToastType = "error";
            return false;
        }

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
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Parses column mapping JSON from the wizard and creates <see cref="ColumnMapping"/> entities.
    /// Skips parsing when JSON is null/empty (no column overrides provided).
    /// </summary>
    private async Task SaveColumnMappingsAsync(int mappingId, string? columnMappingsJson)
    {
        if (string.IsNullOrWhiteSpace(columnMappingsJson))
            return;

        var dtos = JsonSerializer.Deserialize<List<ColumnMappingDto>>(columnMappingsJson);
        if (dtos?.Count > 0)
        {
            foreach (var dto in dtos)
            {
                // Skip entries with empty OracleColumnName to avoid unique index violation
                if (string.IsNullOrWhiteSpace(dto.OracleColumnName))
                    continue;

                _db.ColumnMappings.Add(new ColumnMapping
                {
                    TableMappingConfigId = mappingId,
                    OracleColumnName = dto.OracleColumnName ?? "",
                    SqlColumnName = dto.SqlColumnName ?? dto.OracleColumnName ?? "",
                    SqlDataType = dto.SqlDataType ?? "NVARCHAR(MAX)",
                    SqlMaxLength = dto.SqlMaxLength,
                    SqlPrecision = dto.SqlPrecision,
                    SqlScale = dto.SqlScale,
                    IsNullable = dto.IsNullable,
                    IsExcluded = dto.IsExcluded,
                    DefaultValue = dto.DefaultValue,
                    SortOrder = dto.SortOrder
                });
            }
        }
    }
}

/// <summary>
/// DTO for deserializing column mapping data sent from the wizard JS.
/// </summary>
public class ColumnMappingDto
{
    public string OracleColumnName { get; set; } = "";
    public string? SqlColumnName { get; set; }
    public string? SqlDataType { get; set; }
    public int? SqlMaxLength { get; set; }
    public int? SqlPrecision { get; set; }
    public int? SqlScale { get; set; }
    public bool IsNullable { get; set; } = true;
    public bool IsExcluded { get; set; }
    public string? DefaultValue { get; set; }
    public int SortOrder { get; set; }
}
