using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    public partial class AddNameToTableMappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TableMappings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("UPDATE TableMappings SET Name = OracleSource WHERE Name IS NULL");

            // Use nvarchar(max) because OracleSource (used as Name seed data)
            // can contain long SQL queries. A unique index on a max-length
            // column is not supported, so the Name uniqueness constraint is
            // enforced at the application layer (C# validation).
            // The column stays NOT NULL as required by the model.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TableMappings_Name",
                table: "TableMappings");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TableMappings");
        }
    }
}
