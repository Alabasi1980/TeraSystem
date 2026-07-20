using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseDashboard.Web.Models;

public class ReportFilter
{
    public int Id { get; set; }

    public int ReportId { get; set; }

    [Required, MaxLength(200)]
    public string ColumnName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string FilterType { get; set; } = "Text";   // Text | Date | DateRange | Dropdown | Number | NumberRange

    [Required, MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    public bool IsRequired { get; set; } = false;

    [MaxLength(500)]
    public string? DefaultValue { get; set; }

    [MaxLength(1000)]
    public string? OptionsQuery { get; set; }

    [MaxLength(200)]
    public string? Placeholder { get; set; }

    public int SortOrder { get; set; }

    public Report? Report { get; set; }
}
