using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TeraQuotation.Data;

namespace TeraQuotation.Services;

/// <summary>
/// Checks SQLite database connectivity by opening a raw ADO.NET connection.
/// Uses the same connection string as EF Core's AppDbContext.
/// </summary>
public class DbHealthService : IDbHealthService, IDisposable
{
    private readonly string _connectionString;
    private bool _isConnected;
    private bool _disposed;

    public bool IsConnected => _isConnected;
    public event Action<bool>? ConnectionStatusChanged;

    public DbHealthService(AppDbContext dbContext)
    {
        _connectionString = dbContext.Database.GetConnectionString()
            ?? throw new InvalidOperationException("Could not resolve connection string from AppDbContext.");
    }

    public async Task CheckConnectionAsync()
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Quick reachability check
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1;";
            await command.ExecuteScalarAsync();

            SetConnected(true);
        }
        catch
        {
            SetConnected(false);
        }
    }

    private void SetConnected(bool value)
    {
        if (_isConnected == value) return;
        _isConnected = value;
        ConnectionStatusChanged?.Invoke(_isConnected);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        ConnectionStatusChanged = null;
    }
}
