using System;
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
            migrationBuilder.AddColumn<int>(
                name: "DashboardId",
                table: "DashboardCards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Dashboards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false, defaultValue: ""),
                    Icon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "📊"),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardCards_DashboardId",
                table: "DashboardCards",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_IsActive",
                table: "Dashboards",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_Slug",
                table: "Dashboards",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_SortOrder",
                table: "Dashboards",
                column: "SortOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_DashboardCards_Dashboards_DashboardId",
                table: "DashboardCards",
                column: "DashboardId",
                principalTable: "Dashboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DashboardCards_Dashboards_DashboardId",
                table: "DashboardCards");

            migrationBuilder.DropTable(
                name: "Dashboards");

            migrationBuilder.DropIndex(
                name: "IX_DashboardCards_DashboardId",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "DashboardId",
                table: "DashboardCards");
        }
    }
}
