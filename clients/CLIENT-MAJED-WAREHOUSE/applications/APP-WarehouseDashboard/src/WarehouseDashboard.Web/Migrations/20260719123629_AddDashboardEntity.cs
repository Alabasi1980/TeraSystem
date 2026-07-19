using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent migration: check existence before creating, because
            // DashboardId column may already exist (manually added to the database)
            // while the Dashboards table + FK need to be created via EF Core.

            // ── Dashboards table (only if not already present) ──────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Dashboards]') IS NULL
BEGIN
    CREATE TABLE [Dashboards] (
        [Id] int NOT NULL IDENTITY(1, 1),
        [Name] nvarchar(200) NOT NULL,
        [Slug] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NOT NULL DEFAULT N'',
        [Icon] nvarchar(10) NOT NULL DEFAULT N'📊',
        [SortOrder] int NOT NULL DEFAULT 0,
        [IsActive] bit NOT NULL DEFAULT CONVERT(bit, 1),
        [IsDefault] bit NOT NULL DEFAULT CONVERT(bit, 0),
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Dashboards] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Dashboards_Slug] ON [Dashboards] ([Slug]);
    CREATE INDEX [IX_Dashboards_IsActive] ON [Dashboards] ([IsActive]);
    CREATE INDEX [IX_Dashboards_SortOrder] ON [Dashboards] ([SortOrder]);
END
");

            // ── DashboardId index on DashboardCards (only if missing) ──────
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_DashboardCards_DashboardId'
      AND object_id = OBJECT_ID(N'[DashboardCards]')
)
BEGIN
    CREATE INDEX [IX_DashboardCards_DashboardId] ON [DashboardCards] ([DashboardId]);
END
");

            // ── FK constraint (only if missing) ────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[FK_DashboardCards_Dashboards_DashboardId]') IS NULL
BEGIN
    ALTER TABLE [DashboardCards] ADD CONSTRAINT [FK_DashboardCards_Dashboards_DashboardId]
        FOREIGN KEY ([DashboardId]) REFERENCES [Dashboards] ([Id]) ON DELETE SET NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the operations safely — only if the objects exist.
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[FK_DashboardCards_Dashboards_DashboardId]') IS NOT NULL
    ALTER TABLE [DashboardCards] DROP CONSTRAINT [FK_DashboardCards_Dashboards_DashboardId];
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_DashboardCards_DashboardId'
      AND object_id = OBJECT_ID(N'[DashboardCards]')
)
    DROP INDEX [IX_DashboardCards_DashboardId] ON [DashboardCards];
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Dashboards]') IS NOT NULL
    DROP TABLE [Dashboards];
");
        }
    }
}
