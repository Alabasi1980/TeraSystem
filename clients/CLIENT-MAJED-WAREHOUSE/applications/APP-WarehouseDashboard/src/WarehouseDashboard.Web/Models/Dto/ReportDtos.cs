// ======================================================================
// DTOs for Report services
// Extracted from ReportService.cs during refactoring (TASK-FIX-REFACTOR-001)
// ======================================================================

namespace WarehouseDashboard.Web.Models.Dto;

/// <summary>Information about a SQL Server View.</summary>
public class ViewInfo
{
    public string Schema { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

/// <summary>Column metadata from INFORMATION_SCHEMA.COLUMNS.</summary>
public class ViewColumnInfo
{
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public byte? NumericPrecision { get; set; }
    public int? NumericScale { get; set; }
}

/// <summary>Basic report info for list display.</summary>
public class ReportListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsEnabled { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>Full report definition with columns, filters, layouts.</summary>
public class ReportFullDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsEnabled { get; set; }
    public int SortOrder { get; set; }
    public List<ReportColumnDto> Columns { get; set; } = new();
    public List<ReportFilterDto> Filters { get; set; } = new();
    public List<ReportLayoutDto> Layouts { get; set; } = new();
}

/// <summary>Report column DTO.</summary>
public class ReportColumnDto
{
    public int Id { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public int? Width { get; set; }
    public bool IsVisible { get; set; }
    public bool IsSortable { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsImageColumn { get; set; }
    public string? ImageBaseUrl { get; set; }
    public string? DateFormat { get; set; }
    public string? NumberFormat { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>Report filter DTO.</summary>
public class ReportFilterDto
{
    public int Id { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string FilterType { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsQuery { get; set; }
    public string? ValueColumn { get; set; }
    public string? TextColumn { get; set; }
    public string? Placeholder { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>Report layout DTO.</summary>
public class ReportLayoutDto
{
    public int Id { get; set; }
    public string LayoutName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? ColumnOrder { get; set; }
    public string? VisibleColumns { get; set; }
    public string? ColumnWidths { get; set; }
    public string? FilterValues { get; set; }
    public string? SortState { get; set; }
}

/// <summary>Request model for creating/updating a report.</summary>
public class ReportCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public List<ReportColumnDto> Columns { get; set; } = new();
    public List<ReportFilterDto> Filters { get; set; } = new();
}

/// <summary>Result of executing a report.</summary>
public class ReportDataResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<ReportDataColumn> Columns { get; set; } = new();
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public int RowCount { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public bool ReachedMaxRows { get; set; }
}

/// <summary>Column metadata for report data results.</summary>
public class ReportDataColumn
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string DataType { get; set; } = string.Empty;
    public int? Width { get; set; }
    public bool IsImageColumn { get; set; }
    public string? ImageBaseUrl { get; set; }
    public string? DateFormat { get; set; }
    public string? NumberFormat { get; set; }
}

/// <summary>Request model for saving a layout.</summary>
public class ReportLayoutSaveRequest
{
    public string LayoutName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? ColumnOrder { get; set; }    // JSON
    public string? VisibleColumns { get; set; } // JSON
    public string? ColumnWidths { get; set; }   // JSON
    public string? FilterValues { get; set; }   // JSON
    public string? SortState { get; set; }      // JSON
}

/// <summary>Option for a report filter parameter (Value/Text pair).</summary>
public class ParameterOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
