using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColumnMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableMappingConfigId = table.Column<int>(type: "int", nullable: false),
                    OracleColumnName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SqlColumnName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SqlDataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SqlMaxLength = table.Column<int>(type: "int", nullable: true),
                    SqlPrecision = table.Column<int>(type: "int", nullable: true),
                    SqlScale = table.Column<int>(type: "int", nullable: true),
                    IsNullable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsExcluded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransformationExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColumnMappings_TableMappings_TableMappingConfigId",
                        column: x => x.TableMappingConfigId,
                        principalTable: "TableMappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnMappings_TableMappingConfigId_OracleColumnName",
                table: "ColumnMappings",
                columns: new[] { "TableMappingConfigId", "OracleColumnName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnMappings");
        }
    }
}
