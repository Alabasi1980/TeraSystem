namespace WarehouseDashboard.Web.Infrastructure;

public class AnalyzeRequest
{
    public int CardId { get; set; }
    public string Mode { get; set; } = "explain";
    public int DepthLevel { get; set; } = 1;
}
