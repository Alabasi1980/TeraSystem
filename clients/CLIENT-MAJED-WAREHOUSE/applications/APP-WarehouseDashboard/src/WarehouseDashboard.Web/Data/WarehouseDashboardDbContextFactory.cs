using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WarehouseDashboard.Web.Data;

/// <summary>
/// Design-time DbContext factory for EF Core CLI tools (dotnet ef migrations).
/// Reads the "SqlServer" connection string from appsettings.json, bypassing
/// the full app DI setup that may fail at design time due to missing runtime
/// service registrations (e.g., ReadOnlyQueryHelper).
/// </summary>
public class WarehouseDashboardDbContextFactory : IDesignTimeDbContextFactory<WarehouseDashboardDbContext>
{
    public WarehouseDashboardDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("SqlServer");

        var optionsBuilder = new DbContextOptionsBuilder<WarehouseDashboardDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new WarehouseDashboardDbContext(optionsBuilder.Options);
    }
}
