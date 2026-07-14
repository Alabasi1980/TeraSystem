using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeraQuotation.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitAndQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "QuotationItems",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "QuotationItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Items",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Items");
        }
    }
}
