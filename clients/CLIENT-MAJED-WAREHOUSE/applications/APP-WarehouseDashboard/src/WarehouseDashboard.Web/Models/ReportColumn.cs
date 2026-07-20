using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseDashboard.Web.Models;

public class ReportColumn
{
    public int Id { get; set; }

    public int ReportId { get; set; }

    [Required, MaxLength(200)]
    public string ColumnName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string DataType { get; set; } = string.Empty;

    public int? Width { get; set; } = 150;

    public bool IsVisible { get; set; } = true;

    public bool IsSortable { get; set; } = true;

    public bool IsFilterable { get; set; } = true;

    public bool IsImageColumn { get; set; } = false;

    [MaxLength(500)]
    public string? ImageBaseUrl { get; set; }

    [MaxLength(50)]
    public string? DateFormat { get; set; }

    [MaxLength(50)]
    public string? NumberFormat { get; set; }

    public int SortOrder { get; set; }

    public Report? Report { get; set; }
}
