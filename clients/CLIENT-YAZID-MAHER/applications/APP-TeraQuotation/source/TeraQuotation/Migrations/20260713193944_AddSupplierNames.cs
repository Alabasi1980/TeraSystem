using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeraQuotation.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Supplier1Name",
                table: "QuotationItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supplier2Name",
                table: "QuotationItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supplier3Name",
                table: "QuotationItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Supplier1Name",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "Supplier2Name",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "Supplier3Name",
                table: "QuotationItems");
        }
    }
}
