using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseDashboard.Web.Models;

public class ReportLayout
{
    public int Id { get; set; }

    public int ReportId { get; set; }

    [Required, MaxLength(200)]
    public string LayoutName { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;

    [Column(TypeName = "nvarchar(max)")]
    public string? ColumnOrder { get; set; }    // JSON

    [Column(TypeName = "nvarchar(max)")]
    public string? VisibleColumns { get; set; } // JSON

    [Column(TypeName = "nvarchar(max)")]
    public string? ColumnWidths { get; set; }   // JSON

    [Column(TypeName = "nvarchar(max)")]
    public string? FilterValues { get; set; }   // JSON

    [Column(TypeName = "nvarchar(max)")]
    public string? SortState { get; set; }      // JSON

    public DateTime CreatedAt { get; set; }

    public Report? Report { get; set; }
}
