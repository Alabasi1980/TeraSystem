using Microsoft.EntityFrameworkCore;
using TeraQuotation.Models;

namespace TeraQuotation.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
    public DbSet<Signature> Signatures => Set<Signature>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // QuotationItem - relationships
        modelBuilder.Entity<QuotationItem>()
            .HasOne(qi => qi.Quotation)
            .WithMany(q => q.Items)
            .HasForeignKey(qi => qi.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuotationItem>()
            .HasOne(qi => qi.Item)
            .WithMany()
            .HasForeignKey(qi => qi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quotation - unique QuoteNumber
        modelBuilder.Entity<Quotation>()
            .HasIndex(q => q.QuoteNumber)
            .IsUnique();

        // Setting - Key as primary key
        modelBuilder.Entity<Setting>()
            .HasKey(s => s.Key);

        // Signature - index on OrderIndex
        modelBuilder.Entity<Signature>()
            .HasIndex(s => s.OrderIndex);
    }
}
