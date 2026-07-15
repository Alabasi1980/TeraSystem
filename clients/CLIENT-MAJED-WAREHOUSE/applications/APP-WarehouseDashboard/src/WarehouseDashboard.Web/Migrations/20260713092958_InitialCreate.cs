using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminPassword",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminPassword", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DashboardCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ChartType = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    SqlQuery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataSourceType = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "'SQL Query'"),
                    GridPositionX = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GridPositionY = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GridWidth = table.Column<int>(type: "int", nullable: false, defaultValue: 4),
                    GridHeight = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    RefreshInterval = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardCards", x => x.Id);
                    table.CheckConstraint("CK_DashboardCards_ChartType", "ChartType IN ('Bar','Line','Pie','KPI','Table','Gauge')");
                    table.CheckConstraint("CK_DashboardCards_DataSourceType", "DataSourceType IN ('SQL Query','View')");
                    table.CheckConstraint("CK_DashboardCards_GridHeight", "GridHeight BETWEEN 1 AND 6");
                    table.CheckConstraint("CK_DashboardCards_GridWidth", "GridWidth BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_DashboardCards_RefreshInterval", "RefreshInterval >= 0");
                });

            migrationBuilder.CreateTable(
                name: "SyncSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntervalMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    IsAutoSyncEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastSyncTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardDrillDownLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCardId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    DrillDownQuery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetChartType = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDrillDownLevels", x => x.Id);
                    table.CheckConstraint("CK_CardDrillDownLevels_Level", "Level >= 1");
                    table.CheckConstraint("CK_CardDrillDownLevels_TargetChartType", "TargetChartType IN ('Bar','Line','Pie','KPI','Table','Gauge')");
                    table.ForeignKey(
                        name: "FK_CardDrillDownLevels_DashboardCards_ParentCardId",
                        column: x => x.ParentCardId,
                        principalTable: "DashboardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardDrillDownLevels_ParentCardId_Level",
                table: "CardDrillDownLevels",
                columns: new[] { "ParentCardId", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DashboardCards_GridPositionX_GridPositionY",
                table: "DashboardCards",
                columns: new[] { "GridPositionX", "GridPositionY" });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardCards_IsActive",
                table: "DashboardCards",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminPassword");

            migrationBuilder.DropTable(
                name: "CardDrillDownLevels");

            migrationBuilder.DropTable(
                name: "SyncSettings");

            migrationBuilder.DropTable(
                name: "DashboardCards");
        }
    }
}
