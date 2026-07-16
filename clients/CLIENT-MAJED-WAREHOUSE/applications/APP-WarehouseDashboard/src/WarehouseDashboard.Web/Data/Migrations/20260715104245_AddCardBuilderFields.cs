using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCardBuilderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorPalette",
                table: "DashboardCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "primary");

            migrationBuilder.AddColumn<string>(
                name: "CustomLabelsJson",
                table: "DashboardCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "DrillDownConfigJson",
                table: "DashboardCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "FiltersJson",
                table: "DashboardCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorPalette",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "CustomLabelsJson",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "DrillDownConfigJson",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "FiltersJson",
                table: "DashboardCards");
        }
    }
}
