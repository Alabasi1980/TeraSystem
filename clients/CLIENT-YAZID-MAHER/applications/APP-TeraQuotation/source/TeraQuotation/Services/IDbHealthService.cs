namespace TeraQuotation.Services;

/// <summary>
/// Service that monitors the SQLite database connection health.
/// Provides an event-driven pattern for UI to react to connection status changes.
/// </summary>
public interface IDbHealthService
{
    /// <summary>Whether the database is currently reachable.</summary>
    bool IsConnected { get; }

    /// <summary>Fired whenever the connection status changes.</summary>
    event Action<bool> ConnectionStatusChanged;

    /// <summary>Check the database connection asynchronously and update status.</summary>
    Task CheckConnectionAsync();
}
