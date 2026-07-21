namespace WarehouseDashboard.Web.Infrastructure;

public interface ICardSummaryBuilder
{
    /// <summary>
    /// The chart type this builder handles (e.g., "KPI", "Bar", "Line").
    /// Used by <see cref="Services.CardSummaryBuilderFactory"/> to resolve the correct builder.
    /// </summary>
    string ChartType { get; }

    /// <summary>
    /// Builds a data summary for a dashboard card at a given depth.
    /// </summary>
    /// <param name="card">Card configuration from the database.</param>
    /// <param name="depthLevel">1-6 per the depth map.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Structured summary ready to send to the AI model.</returns>
    Task<CardSummary> BuildSummaryAsync(Models.DashboardCard card, int depthLevel, CancellationToken ct = default);
}
