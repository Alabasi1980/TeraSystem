using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssistantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SyncMode",
                table: "TableMappings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Full",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldDefaultValue: "Full");

            migrationBuilder.AddColumn<bool>(
                name: "AssistantEnabled",
                table: "DashboardCards",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "AssistantPrompt",
                table: "DashboardCards",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssistantEnabled",
                table: "DashboardCards");

            migrationBuilder.DropColumn(
                name: "AssistantPrompt",
                table: "DashboardCards");

            migrationBuilder.AlterColumn<string>(
                name: "SyncMode",
                table: "TableMappings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Full",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Full");
        }
    }
}
