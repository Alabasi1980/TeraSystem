using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardCardDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AggregationType",
                table: "DashboardCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DashboardCards",
                type: "nvarchar(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginalSourceId",
                table: "DashboardCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginalSourceType",
                table: "DashboardCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AggregationType",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "OriginalSourceId",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "OriginalSourceType",
                table: "DashboardCards");
        }
    }
}
