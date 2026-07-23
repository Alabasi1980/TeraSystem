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

    public DbSet<Dashboard> Dashboards => Set<Dashboard>();
    public DbSet<DashboardCard> DashboardCards => Set<DashboardCard>();
    public DbSet<CardDrillDownLevel> CardDrillDownLevels => Set<CardDrillDownLevel>();
    public DbSet<SyncSetting> SyncSettings => Set<SyncSetting>();
    public DbSet<AdminPassword> AdminPasswords => Set<AdminPassword>();
    public DbSet<TableMappingConfig> TableMappings => Set<TableMappingConfig>();
    public DbSet<ColumnMapping> ColumnMappings => Set<ColumnMapping>();
    public DbSet<SyncRun> SyncRuns => Set<SyncRun>();
    public DbSet<SyncRunDetail> SyncRunDetails => Set<SyncRunDetail>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportColumn> ReportColumns => Set<ReportColumn>();
    public DbSet<ReportFilter> ReportFilters => Set<ReportFilter>();
    public DbSet<ReportLayout> ReportLayouts => Set<ReportLayout>();
    public DbSet<AssistantInsightLog> AssistantInsightLogs => Set<AssistantInsightLog>();
    public DbSet<AssistantUsageStat> AssistantUsageStats => Set<AssistantUsageStat>();
    public DbSet<SavedQuery> SavedQueries => Set<SavedQuery>();
    public DbSet<AiConversation> AiConversations => Set<AiConversation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------------------------------------------------------
        // Dashboards (TASK-DASH-001 / DASH-002)
        // -------------------------------------------------------------------
        modelBuilder.Entity<Dashboard>(entity =>
        {
            entity.ToTable("Dashboards");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(500)")
                .HasDefaultValue("");

            entity.Property(e => e.Icon)
                .HasMaxLength(10)
                .HasDefaultValue("\U0001F4CA");

            entity.Property(e => e.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // Unique slug (case-insensitive enforced at service level)
            entity.HasIndex(e => e.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Dashboards_Slug");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Dashboards_IsActive");
            entity.HasIndex(e => e.SortOrder)
                .HasDatabaseName("IX_Dashboards_SortOrder");
        });

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

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(500)")
                .HasDefaultValue("");

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

            // AI Assistant settings
            entity.Property(e => e.AssistantEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.AssistantPrompt)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            // Indexes
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_DashboardCards_IsActive");
            entity.HasIndex(e => new { e.GridPositionX, e.GridPositionY })
                .HasDatabaseName("IX_DashboardCards_GridPositionX_GridPositionY");

            // FK → Dashboards (SET NULL on delete: orphan cards keep working)
            entity.HasOne(e => e.Dashboard)
                .WithMany()
                .HasForeignKey(e => e.DashboardId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.DashboardId)
                .HasDatabaseName("IX_DashboardCards_DashboardId");
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

            // Drill Down parameter contract (TASK-DRILL-SCHEMA-001):
            // - ParameterColumn: column whose value is passed as @p0 to the next level.
            // - LabelColumn: human-readable column for breadcrumb labels.
            // - RequiresParentValue: when true, this level needs a parent value (Level > 1).
            // Note: no CHECK constraints on ParameterColumn/LabelColumn — values are validated
            // at runtime in the Drill API (case-insensitive lookup against the result schema).
            entity.Property(e => e.ParameterColumn)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(e => e.LabelColumn)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(e => e.ColumnAliases)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.RequiresParentValue)
                .IsRequired()
                .HasDefaultValue(false);

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
                .HasMaxLength(4000);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(4000);

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
                .HasMaxLength(20)
                .HasDefaultValue("Full");

            entity.Property(e => e.IncrementalColumn)
                .HasMaxLength(128);

            entity.Property(e => e.InitialSyncStartDate)
                .HasColumnType("datetime2")
                .IsRequired(false);

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

        // -------------------------------------------------------------------
        // SyncRuns (TASK-SYNC-LOG-01) — persistent sync cycle log
        // -------------------------------------------------------------------
        modelBuilder.Entity<SyncRun>(entity =>
        {
            entity.ToTable("SyncRuns");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.StartTime)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(e => e.EndTime)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            entity.Property(e => e.TriggerType)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            entity.Property(e => e.TotalRecordCount)
                .IsRequired(false);

            entity.Property(e => e.TotalDurationSeconds)
                .HasColumnType("float")
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // Index: list runs newest-first
            entity.HasIndex(e => e.StartTime)
                .IsDescending()
                .HasDatabaseName("IX_SyncRuns_StartTime");
        });

        // -------------------------------------------------------------------
        // SyncRunDetails (TASK-SYNC-LOG-01) — per-mapping detail within a run
        // -------------------------------------------------------------------
        modelBuilder.Entity<SyncRunDetail>(entity =>
        {
            entity.ToTable("SyncRunDetails");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.SyncRunId)
                .IsRequired();

            entity.Property(e => e.TableMappingId)
                .IsRequired(false);

            entity.Property(e => e.TargetTable)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.SyncMode)
                .IsRequired()
                .HasColumnType("nvarchar(20)")
                .HasDefaultValue("Full");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            entity.Property(e => e.RowsExtracted)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.RowsLoaded)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.Attempts)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.DurationSeconds)
                .HasColumnType("float")
                .IsRequired(false);

            entity.Property(e => e.ErrorMessage)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // FK → SyncRuns (CASCADE delete)
            entity.HasOne(e => e.SyncRun)
                .WithMany(s => s.Details)
                .HasForeignKey(e => e.SyncRunId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK → TableMappings (SET NULL on delete to preserve historical data)
            entity.HasOne(e => e.TableMapping)
                .WithMany()
                .HasForeignKey(e => e.TableMappingId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index for detail lookups by parent run
            entity.HasIndex(e => e.SyncRunId)
                .HasDatabaseName("IX_SyncRunDetails_SyncRunId");
        });

        // -------------------------------------------------------------------
        // Reports (TASK-REPORT-001) — نظام التقارير التفاعلي
        // -------------------------------------------------------------------
        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("Reports");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.ViewName)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(500)")
                .IsRequired(false);

            entity.Property(e => e.Icon)
                .HasColumnType("nvarchar(50)")
                .IsRequired(false);

            entity.Property(e => e.IsEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => e.IsEnabled)
                .HasDatabaseName("IX_Reports_IsEnabled");
            entity.HasIndex(e => e.SortOrder)
                .HasDatabaseName("IX_Reports_SortOrder");
        });

        // -------------------------------------------------------------------
        // ReportColumns (TASK-REPORT-001)
        // -------------------------------------------------------------------
        modelBuilder.Entity<ReportColumn>(entity =>
        {
            entity.ToTable("ReportColumns");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.ColumnName)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.DataType)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            entity.Property(e => e.Width)
                .HasDefaultValue(150);

            entity.Property(e => e.IsVisible)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.IsSortable)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.IsFilterable)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.IsImageColumn)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.ImageBaseUrl)
                .HasColumnType("nvarchar(500)")
                .IsRequired(false);

            entity.Property(e => e.DateFormat)
                .HasColumnType("nvarchar(50)")
                .IsRequired(false);

            entity.Property(e => e.NumberFormat)
                .HasColumnType("nvarchar(50)")
                .IsRequired(false);

            entity.Property(e => e.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // FK → Reports
            entity.HasOne(e => e.Report)
                .WithMany(r => r.Columns)
                .HasForeignKey(e => e.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ReportId)
                .HasDatabaseName("IX_ReportColumns_ReportId");

            entity.HasIndex(e => new { e.ReportId, e.SortOrder })
                .HasDatabaseName("IX_ReportColumns_ReportId_SortOrder");
        });

        // -------------------------------------------------------------------
        // ReportFilters (TASK-REPORT-001)
        // -------------------------------------------------------------------
        modelBuilder.Entity<ReportFilter>(entity =>
        {
            entity.ToTable("ReportFilters");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.ColumnName)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.FilterType)
                .IsRequired()
                .HasColumnType("nvarchar(50)")
                .HasDefaultValue("Text");

            entity.Property(e => e.Label)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.IsRequired)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.DefaultValue)
                .HasColumnType("nvarchar(500)")
                .IsRequired(false);

            entity.Property(e => e.OptionsQuery)
                .HasColumnType("nvarchar(1000)")
                .IsRequired(false);

            entity.Property(e => e.ValueColumn)
                .HasColumnType("nvarchar(200)")
                .IsRequired(false);

            entity.Property(e => e.TextColumn)
                .HasColumnType("nvarchar(200)")
                .IsRequired(false);

            entity.Property(e => e.Placeholder)
                .HasColumnType("nvarchar(200)")
                .IsRequired(false);

            entity.Property(e => e.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // FK → Reports
            entity.HasOne(e => e.Report)
                .WithMany(r => r.Filters)
                .HasForeignKey(e => e.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ReportId)
                .HasDatabaseName("IX_ReportFilters_ReportId");
        });

        // -------------------------------------------------------------------
        // ReportLayouts (TASK-REPORT-001)
        // -------------------------------------------------------------------
        modelBuilder.Entity<ReportLayout>(entity =>
        {
            entity.ToTable("ReportLayouts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.LayoutName)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.ColumnOrder)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.VisibleColumns)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.ColumnWidths)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.FilterValues)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.SortState)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // FK → Reports
            entity.HasOne(e => e.Report)
                .WithMany(r => r.Layouts)
                .HasForeignKey(e => e.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ReportId)
                .HasDatabaseName("IX_ReportLayouts_ReportId");
        });

        // -------------------------------------------------------------------
        // AssistantInsightLogs (TASK-AI-E01)
        // -------------------------------------------------------------------
        modelBuilder.Entity<AssistantInsightLog>(entity =>
        {
            entity.ToTable("AssistantInsightLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.CardId).IsRequired();

            entity.Property(e => e.Mode)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.DepthLevel).IsRequired();

            entity.Property(e => e.RequestedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(e => e.PromptVersion)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.CardPromptUsed).IsRequired();

            entity.Property(e => e.DataScopeLabel)
                .HasMaxLength(50);

            entity.Property(e => e.IsFullDataReached).IsRequired();

            entity.Property(e => e.WasCached).IsRequired();

            entity.Property(e => e.ResponseTimeMs).IsRequired();

            entity.Property(e => e.ErrorCode)
                .HasMaxLength(50);

            entity.HasOne(e => e.Card)
                .WithMany()
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CardId);
            entity.HasIndex(e => e.RequestedAt);
        });

        // -------------------------------------------------------------------
        // AssistantUsageStats (TASK-AI-E01)
        // -------------------------------------------------------------------
        modelBuilder.Entity<AssistantUsageStat>(entity =>
        {
            entity.ToTable("AssistantUsageStats");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.CardId).IsRequired();

            entity.Property(e => e.TotalRequests).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.ExplainRequests).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.DeepRequests).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.DeepenClicks).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.MostUsedDepth).IsRequired().HasDefaultValue(1);

            entity.Property(e => e.LastUsedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.AverageResponseTimeMs).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.CacheHitCount).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.CacheMissCount).IsRequired().HasDefaultValue(0);

            entity.HasOne(e => e.Card)
                .WithMany()
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CardId).IsUnique();
        });

        // -------------------------------------------------------------------
        // SavedQueries (TASK-AIQ-001) — saved SQL queries for AI Assistant
        // -------------------------------------------------------------------
        modelBuilder.Entity<SavedQuery>(entity =>
        {
            entity.ToTable("SavedQueries", t =>
            {
                t.HasCheckConstraint("CK_SavedQueries_DataSourceType", "DataSourceType IN ('SqlServer', 'Oracle')");
            });
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(500)")
                .IsRequired(false);

            entity.Property(e => e.SqlQuery)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.DataSourceType)
                .IsRequired()
                .HasColumnType("nvarchar(50)")
                .HasDefaultValue("SqlServer");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => e.Name)
                .HasDatabaseName("IX_SavedQueries_Name");

            entity.HasIndex(e => e.UpdatedAt)
                .IsDescending()
                .HasDatabaseName("IX_SavedQueries_UpdatedAt");
        });

        // -------------------------------------------------------------------
        // AiConversations (TASK-AIQ-001) — AI Assistant conversation messages
        // -------------------------------------------------------------------
        modelBuilder.Entity<AiConversation>(entity =>
        {
            entity.ToTable("AiConversations", t =>
            {
                t.HasCheckConstraint("CK_AiConversations_Role", "Role IN ('user', 'assistant', 'system')");
            });
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.SavedQueryId)
                .IsRequired(false);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasColumnType("nvarchar(20)");

            entity.Property(e => e.Message)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.SqlSnapshot)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // FK → SavedQueries (ON DELETE CASCADE)
            entity.HasOne(e => e.SavedQuery)
                .WithMany(s => s.Conversations)
                .HasForeignKey(e => e.SavedQueryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index on (SavedQueryId, CreatedAt) for efficient conversation lookups
            entity.HasIndex(e => new { e.SavedQueryId, e.CreatedAt })
                .HasDatabaseName("IX_AiConversations_SavedQueryId_CreatedAt");
        });
    }
}
