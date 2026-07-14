using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TeraQuotation.Data;

/// <summary>
/// Design-time factory for EF Core migrations.
/// Required because AppDbContext is registered via DI at runtime.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=TeraQuotation.db");
        return new AppDbContext(optionsBuilder.Options);
    }
}
