using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDrillDownParameterContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TASK-DRILL-SCHEMA-001: Add Drill Down parameter contract columns.
            // ParameterColumn: column whose value is passed as @p0 to the next level.
            // LabelColumn: human-readable column for breadcrumb labels.
            // RequiresParentValue: when true, this level needs a parent value (Level > 1).

            migrationBuilder.AddColumn<string>(
                name: "LabelColumn",
                table: "CardDrillDownLevels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParameterColumn",
                table: "CardDrillDownLevels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresParentValue",
                table: "CardDrillDownLevels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LabelColumn",
                table: "CardDrillDownLevels");

            migrationBuilder.DropColumn(
                name: "ParameterColumn",
                table: "CardDrillDownLevels");

            migrationBuilder.DropColumn(
                name: "RequiresParentValue",
                table: "CardDrillDownLevels");
        }
    }
}
