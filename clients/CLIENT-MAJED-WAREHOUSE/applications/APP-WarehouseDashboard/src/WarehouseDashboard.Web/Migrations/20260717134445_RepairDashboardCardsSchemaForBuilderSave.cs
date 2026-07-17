using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Migrations
{
    /// <inheritdoc />
    public partial class RepairDashboardCardsSchemaForBuilderSave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('DashboardCards', 'ColorPalette') IS NULL
    ALTER TABLE [DashboardCards] ADD [ColorPalette] nvarchar(50) NOT NULL CONSTRAINT [DF_DashboardCards_ColorPalette] DEFAULT N'primary';

IF COL_LENGTH('DashboardCards', 'ValueColumn') IS NULL
    ALTER TABLE [DashboardCards] ADD [ValueColumn] nvarchar(100) NOT NULL CONSTRAINT [DF_DashboardCards_ValueColumn] DEFAULT N'';

IF COL_LENGTH('DashboardCards', 'DateColumn') IS NULL
    ALTER TABLE [DashboardCards] ADD [DateColumn] nvarchar(100) NOT NULL CONSTRAINT [DF_DashboardCards_DateColumn] DEFAULT N'';

IF COL_LENGTH('DashboardCards', 'CategoryColumn') IS NULL
    ALTER TABLE [DashboardCards] ADD [CategoryColumn] nvarchar(100) NOT NULL CONSTRAINT [DF_DashboardCards_CategoryColumn] DEFAULT N'';

IF COL_LENGTH('DashboardCards', 'KpiMode') IS NULL
    ALTER TABLE [DashboardCards] ADD [KpiMode] nvarchar(50) NOT NULL CONSTRAINT [DF_DashboardCards_KpiMode] DEFAULT N'simple';

IF COL_LENGTH('DashboardCards', 'ShowChange') IS NULL
    ALTER TABLE [DashboardCards] ADD [ShowChange] bit NOT NULL CONSTRAINT [DF_DashboardCards_ShowChange] DEFAULT CONVERT(bit, 0);

IF COL_LENGTH('DashboardCards', 'ChangeSource') IS NULL
    ALTER TABLE [DashboardCards] ADD [ChangeSource] nvarchar(50) NOT NULL CONSTRAINT [DF_DashboardCards_ChangeSource] DEFAULT N'previousPeriod';

IF COL_LENGTH('DashboardCards', 'ShowSparkline') IS NULL
    ALTER TABLE [DashboardCards] ADD [ShowSparkline] bit NOT NULL CONSTRAINT [DF_DashboardCards_ShowSparkline] DEFAULT CONVERT(bit, 0);

IF COL_LENGTH('DashboardCards', 'SparklineMonths') IS NULL
    ALTER TABLE [DashboardCards] ADD [SparklineMonths] int NOT NULL CONSTRAINT [DF_DashboardCards_SparklineMonths] DEFAULT 6;

IF COL_LENGTH('DashboardCards', 'ShowGrandTotal') IS NULL
    ALTER TABLE [DashboardCards] ADD [ShowGrandTotal] bit NOT NULL CONSTRAINT [DF_DashboardCards_ShowGrandTotal] DEFAULT CONVERT(bit, 0);

IF COL_LENGTH('DashboardCards', 'GrandTotalSource') IS NULL
    ALTER TABLE [DashboardCards] ADD [GrandTotalSource] nvarchar(50) NOT NULL CONSTRAINT [DF_DashboardCards_GrandTotalSource] DEFAULT N'sameTable';

IF COL_LENGTH('DashboardCards', 'DateFilterMode') IS NULL
    ALTER TABLE [DashboardCards] ADD [DateFilterMode] nvarchar(50) NOT NULL CONSTRAINT [DF_DashboardCards_DateFilterMode] DEFAULT N'dashboard';

IF COL_LENGTH('DashboardCards', 'FixedStartDate') IS NULL
    ALTER TABLE [DashboardCards] ADD [FixedStartDate] nvarchar(50) NOT NULL CONSTRAINT [DF_DashboardCards_FixedStartDate] DEFAULT N'';

IF COL_LENGTH('DashboardCards', 'FixedEndDate') IS NULL
    ALTER TABLE [DashboardCards] ADD [FixedEndDate] nvarchar(50) NOT NULL CONSTRAINT [DF_DashboardCards_FixedEndDate] DEFAULT N'';

IF COL_LENGTH('DashboardCards', 'RelativeDays') IS NULL
    ALTER TABLE [DashboardCards] ADD [RelativeDays] int NOT NULL CONSTRAINT [DF_DashboardCards_RelativeDays] DEFAULT 30;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally left empty: this repair migration only guarantees that
            // DashboardCards columns required by the builder exist and should not
            // remove data-bearing columns on rollback.
        }
    }
}
