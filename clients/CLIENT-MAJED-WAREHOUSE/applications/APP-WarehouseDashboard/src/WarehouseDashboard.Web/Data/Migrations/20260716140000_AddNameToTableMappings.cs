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
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql("UPDATE TableMappings SET Name = OracleSource WHERE Name IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TableMappings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableMappings_Name",
                table: "TableMappings",
                column: "Name",
                unique: true);
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
