using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Models;

namespace WarehouseDashboard.Web.Data;

/// <summary>
/// EF Core DbContext for the WarehouseDashboard configuration tables
/// (DashboardCards, CardDrillDownLevels, SyncSettings, AdminPassword).
/// Managed via EF Core Migrations (TASK-COD-002, aligned to 06_DATA_MODEL_PREPARATION.md §1).
/// </summary>
public class WarehouseDashboardDbContext : DbContext
{
    public WarehouseDashboardDbContext(DbContextOptions<WarehouseDashboardDbContext> options)
        : base(options)
    {
    }

    public DbSet<DashboardCard> DashboardCards => Set<DashboardCard>();
    public DbSet<CardDrillDownLevel> CardDrillDownLevels => Set<CardDrillDownLevel>();
    public DbSet<SyncSetting> SyncSettings => Set<SyncSetting>();
    public DbSet<AdminPassword> AdminPasswords => Set<AdminPassword>();
    public DbSet<TableMappingConfig> TableMappings => Set<TableMappingConfig>();
    public DbSet<ColumnMapping> ColumnMappings => Set<ColumnMapping>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------------------------------------------------------
        // DashboardCards (spec §1.1)
        // -------------------------------------------------------------------
        modelBuilder.Entity<DashboardCard>(entity =>
        {
            entity.ToTable("DashboardCards", t =>
            {
                t.HasCheckConstraint("CK_DashboardCards_ChartType", "ChartType IN ('Bar','Line','Pie','KPI','Table','Gauge')");
                t.HasCheckConstraint("CK_DashboardCards_DataSourceType", "DataSourceType IN ('SQL Query','View')");
                t.HasCheckConstraint("CK_DashboardCards_GridWidth", "GridWidth BETWEEN 1 AND 12");
                t.HasCheckConstraint("CK_DashboardCards_GridHeight", "GridHeight BETWEEN 1 AND 6");
                t.HasCheckConstraint("CK_DashboardCards_RefreshInterval", "RefreshInterval >= 0");
            });
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.ChartType)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            entity.Property(e => e.SqlQuery)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.DataSourceType)
                .IsRequired()
                .HasColumnType("nvarchar(50)")
                .HasDefaultValueSql("'SQL Query'");

            entity.Property(e => e.GridPositionX)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.GridPositionY)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.GridWidth)
                .IsRequired()
                .HasDefaultValue(4);

            entity.Property(e => e.GridHeight)
                .IsRequired()
                .HasDefaultValue(2);

            entity.Property(e => e.RefreshInterval)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.ColorPalette)
                .HasMaxLength(50)
                .HasDefaultValue("primary");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // Advanced KPI: Column Mappings
            entity.Property(e => e.ValueColumn)
                .HasMaxLength(100)
                .HasDefaultValue("");

            entity.Property(e => e.DateColumn)
                .HasMaxLength(100)
                .HasDefaultValue("");

            entity.Property(e => e.CategoryColumn)
                .HasMaxLength(100)
                .HasDefaultValue("");

            // Advanced KPI: Mode & Change
            entity.Property(e => e.KpiMode)
                .HasMaxLength(50)
                .HasDefaultValue("simple");

            entity.Property(e => e.ShowChange)
                .HasDefaultValue(false);

            entity.Property(e => e.ChangeSource)
                .HasMaxLength(50)
                .HasDefaultValue("previousPeriod");

            // Advanced KPI: Sparkline
            entity.Property(e => e.ShowSparkline)
                .HasDefaultValue(false);

            entity.Property(e => e.SparklineMonths)
                .HasDefaultValue(6);

            // Advanced KPI: Grand Total
            entity.Property(e => e.ShowGrandTotal)
                .HasDefaultValue(false);

            entity.Property(e => e.GrandTotalSource)
                .HasMaxLength(50)
                .HasDefaultValue("sameTable");

            // Advanced KPI: Date Filter
            entity.Property(e => e.DateFilterMode)
                .HasMaxLength(50)
                .HasDefaultValue("dashboard");

            entity.Property(e => e.FixedStartDate)
                .HasMaxLength(50)
                .HasDefaultValue("");

            entity.Property(e => e.FixedEndDate)
                .HasMaxLength(50)
                .HasDefaultValue("");

            entity.Property(e => e.RelativeDays)
                .HasDefaultValue(30);

            // Indexes
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_DashboardCards_IsActive");
            entity.HasIndex(e => new { e.GridPositionX, e.GridPositionY })
                .HasDatabaseName("IX_DashboardCards_GridPositionX_GridPositionY");
        });

        // -------------------------------------------------------------------
        // CardDrillDownLevels (spec §1.2)
        // -------------------------------------------------------------------
        modelBuilder.Entity<CardDrillDownLevel>(entity =>
        {
            entity.ToTable("CardDrillDownLevels", t =>
            {
                t.HasCheckConstraint("CK_CardDrillDownLevels_Level", "Level >= 1");
                t.HasCheckConstraint("CK_CardDrillDownLevels_TargetChartType", "TargetChartType IN ('Bar','Line','Pie','KPI','Table','Gauge')");
            });
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.ParentCardId).IsRequired();

            entity.Property(e => e.Level)
                .IsRequired()
                .HasDefaultValue(1);

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.DrillDownQuery)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.TargetChartType)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            // FK → DashboardCards (CASCADE delete).
            entity.HasOne(e => e.Card)
                .WithMany(c => c.DrillDownLevels)
                .HasForeignKey(e => e.ParentCardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index: one level per (card, ordinal).
            entity.HasIndex(e => new { e.ParentCardId, e.Level })
                .IsUnique()
                .HasDatabaseName("IX_CardDrillDownLevels_ParentCardId_Level");
        });

        // -------------------------------------------------------------------
        // SyncSettings (singleton: Id = 1) (spec §1.3)
        // -------------------------------------------------------------------
        modelBuilder.Entity<SyncSetting>(entity =>
        {
            entity.ToTable("SyncSettings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.IntervalMinutes)
                .IsRequired()
                .HasDefaultValue(30);
            entity.Property(e => e.IsAutoSyncEnabled)
                .IsRequired()
                .HasDefaultValue(false);
            entity.Property(e => e.LastSyncTimestamp)
                .HasColumnType("datetime2")
                .IsRequired(false);
        });

        // -------------------------------------------------------------------
        // AdminPassword (singleton: Id = 1) (spec §1.4)
        // -------------------------------------------------------------------
        modelBuilder.Entity<AdminPassword>(entity =>
        {
            entity.ToTable("AdminPassword");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasColumnType("nvarchar(500)");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // -------------------------------------------------------------------
        // TableMappings (spec TASK-COD-025) — dynamic Oracle→SQL sync config
        // -------------------------------------------------------------------
        modelBuilder.Entity<TableMappingConfig>(entity =>
        {
            entity.ToTable("TableMappings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.OracleSource)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.SourceType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Table");

            entity.Property(e => e.SqlTargetTable)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.LastSyncAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.Property(e => e.SyncRecordCount)
                .HasDefaultValue(0);

            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(1000)
                .IsRequired(false);

            entity.Property(e => e.SyncMode)
                .HasMaxLength(10)
                .HasDefaultValue("Full");

            entity.Property(e => e.IncrementalColumn)
                .HasMaxLength(128);

            entity.HasIndex(e => e.OracleSource)
                .IsUnique()
                .HasDatabaseName("IX_TableMappings_OracleSource");

            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("IX_TableMappings_Name");

            entity.HasIndex(e => e.SqlTargetTable)
                .IsUnique()
                .HasDatabaseName("IX_TableMappings_SqlTargetTable");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_TableMappings_IsActive");
        });

        // -------------------------------------------------------------------
        // ColumnMappings (TASK-COD-COL-001) — per-column type overrides
        // -------------------------------------------------------------------
        modelBuilder.Entity<ColumnMapping>(entity =>
        {
            entity.ToTable("ColumnMappings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.OracleColumnName)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(e => e.SqlColumnName)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(e => e.SqlDataType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SqlMaxLength);
            entity.Property(e => e.SqlPrecision);
            entity.Property(e => e.SqlScale);

            entity.Property(e => e.IsNullable)
                .HasDefaultValue(true);

            entity.Property(e => e.IsExcluded)
                .HasDefaultValue(false);

            entity.Property(e => e.IsNumericText)
                .HasDefaultValue(false);

            entity.Property(e => e.DefaultValue)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(e => e.TransformationExpression)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.SortOrder)
                .HasDefaultValue(0);

            // FK → TableMappings (CASCADE delete)
            entity.HasOne(e => e.TableMappingConfig)
                .WithMany(t => t.ColumnMappings)
                .HasForeignKey(e => e.TableMappingConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index per mapping + column
            entity.HasIndex(e => new { e.TableMappingConfigId, e.OracleColumnName })
                .IsUnique()
                .HasDatabaseName("IX_ColumnMappings_TableMappingConfigId_OracleColumnName");
        });
    }
}
