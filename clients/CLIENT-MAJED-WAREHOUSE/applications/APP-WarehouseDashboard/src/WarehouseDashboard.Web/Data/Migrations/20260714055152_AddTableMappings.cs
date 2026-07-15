using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OracleSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Table"),
                    SqlTargetTable = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncRecordCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableMappings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableMappings_IsActive",
                table: "TableMappings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TableMappings_OracleSource",
                table: "TableMappings",
                column: "OracleSource",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableMappings_SqlTargetTable",
                table: "TableMappings",
                column: "SqlTargetTable",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableMappings");
        }
    }
}
