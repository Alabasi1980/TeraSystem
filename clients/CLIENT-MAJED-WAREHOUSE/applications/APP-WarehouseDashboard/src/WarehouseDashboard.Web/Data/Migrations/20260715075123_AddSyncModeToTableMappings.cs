using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncModeToTableMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncrementalColumn",
                table: "TableMappings",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncMode",
                table: "TableMappings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Full");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncrementalColumn",
                table: "TableMappings");

            migrationBuilder.DropColumn(
                name: "SyncMode",
                table: "TableMappings");
        }
    }
}
