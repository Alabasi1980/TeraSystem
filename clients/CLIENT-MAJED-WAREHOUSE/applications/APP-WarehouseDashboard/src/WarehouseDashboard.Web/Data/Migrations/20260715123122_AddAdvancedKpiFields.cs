using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedKpiFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryColumn",
                table: "DashboardCards",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChangeSource",
                table: "DashboardCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "previousPeriod");

            migrationBuilder.AddColumn<string>(
                name: "DateColumn",
                table: "DashboardCards",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateFilterMode",
                table: "DashboardCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "dashboard");

            migrationBuilder.AddColumn<string>(
                name: "FixedEndDate",
                table: "DashboardCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FixedStartDate",
                table: "DashboardCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalSource",
                table: "DashboardCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "sameTable");

            migrationBuilder.AddColumn<string>(
                name: "KpiMode",
                table: "DashboardCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "simple");

            migrationBuilder.AddColumn<int>(
                name: "RelativeDays",
                table: "DashboardCards",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<bool>(
                name: "ShowChange",
                table: "DashboardCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowGrandTotal",
                table: "DashboardCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowSparkline",
                table: "DashboardCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SparklineMonths",
                table: "DashboardCards",
                type: "int",
                nullable: false,
                defaultValue: 6);

            migrationBuilder.AddColumn<string>(
                name: "ValueColumn",
                table: "DashboardCards",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryColumn",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "ChangeSource",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "DateColumn",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "DateFilterMode",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "FixedEndDate",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "FixedStartDate",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "GrandTotalSource",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "KpiMode",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "RelativeDays",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "ShowChange",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "ShowGrandTotal",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "ShowSparkline",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "SparklineMonths",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "ValueColumn",
                table: "DashboardCards");
        }
    }
}
