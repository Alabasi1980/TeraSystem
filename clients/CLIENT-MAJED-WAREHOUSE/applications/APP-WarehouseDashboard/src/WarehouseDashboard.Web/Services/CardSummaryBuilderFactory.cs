using WarehouseDashboard.Web.Infrastructure;

namespace WarehouseDashboard.Web.Services;

/// <summary>
/// Resolves the correct <see cref="ICardSummaryBuilder"/> by chart type.
/// Receives all registered builders via DI and selects by <see cref="ICardSummaryBuilder.ChartType"/>.
/// </summary>
public class CardSummaryBuilderFactory
{
    private readonly Dictionary<string, ICardSummaryBuilder> _builders;

    public CardSummaryBuilderFactory(IEnumerable<ICardSummaryBuilder> builders)
    {
        _builders = builders.ToDictionary(
            b => b.ChartType,
            StringComparer.OrdinalIgnoreCase);

        if (_builders.Count == 0)
            throw new InvalidOperationException(
                "No ICardSummaryBuilder implementations registered.");
    }

    /// <summary>
    /// Returns the builder matching <paramref name="chartType"/>.
    /// Falls back to the first available builder when no exact match exists.
    /// </summary>
    public ICardSummaryBuilder GetBuilder(string chartType)
    {
        if (_builders.TryGetValue(chartType, out var builder))
            return builder;

        // Generic fallback marker
        if (_builders.TryGetValue("*", out var fallback))
            return fallback;

        // If no "*" registered, return the first available builder as a safe fallback
        return _builders.Values.First();
    }
}
