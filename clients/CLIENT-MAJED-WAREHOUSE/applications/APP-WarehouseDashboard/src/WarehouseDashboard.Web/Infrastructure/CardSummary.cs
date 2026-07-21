namespace WarehouseDashboard.Web.Infrastructure;

public class CardSummary
{
    // Card metadata
    public int CardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ChartType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AssistantPrompt { get; set; }

    // Scope
    public int DepthLevel { get; set; }
    public string DepthLabel { get; set; } = string.Empty;
    public bool HasDateColumn { get; set; }
    public string? DateColumn { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public bool IsFullDataReached { get; set; }
    public bool HasDeeperData { get; set; }

    // Value & Trend
    public double? CurrentValue { get; set; }
    public double? PreviousValue { get; set; }
    public double? ChangePercent { get; set; }
    public string? TrendDirection { get; set; } // "up", "down", "stable", null

    // Series (time-based)
    public List<SeriesPoint> SeriesData { get; set; } = new();

    // Aggregates
    public int? TotalRowCount { get; set; }
    public Dictionary<string, NumericColumnSummary> ColumnSummaries { get; set; } = new();

    // Top / Bottom
    public List<CategoryItem> TopItems { get; set; } = new();
    public List<CategoryItem> BottomItems { get; set; } = new();

    // Samples
    public List<Dictionary<string, object?>> SampleRows { get; set; } = new();

    // Quality notes
    public List<string> DataQualityNotes { get; set; } = new();
}

public class SeriesPoint
{
    public string Period { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class CategoryItem
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public double? Percent { get; set; }
}

public class NumericColumnSummary
{
    public double Sum { get; set; }
    public double Average { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
}
