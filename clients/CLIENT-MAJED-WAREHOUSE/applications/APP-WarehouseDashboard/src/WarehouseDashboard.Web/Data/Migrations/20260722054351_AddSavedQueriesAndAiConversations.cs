using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <inheritdoc />
    /// <summary>
    /// Creates SavedQueries and AiConversations tables for the AI Query Assistant.
    ///
    /// NOTE: This migration also syncs the EF Core snapshot for ValueFormatType and
    /// ValueUnit columns on DashboardCards — these columns already exist in the
    /// database (added by another task/session), so they are NOT included in Up().
    /// The snapshot is updated by the Designer.cs generated from the current model,
    /// which already includes these properties on DashboardCard.
    /// </summary>
    public partial class AddSavedQueriesAndAiConversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ValueFormatType and ValueUnit on DashboardCards already exist in DB.
            // They are omitted here — the model snapshot (Designer.cs) already
            // reflects them so EF Core knows the model is in sync.

            migrationBuilder.CreateTable(
                name: "SavedQueries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    SqlQuery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataSourceType = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValue: "SqlServer"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedQueries", x => x.Id);
                    table.CheckConstraint("CK_SavedQueries_DataSourceType", "DataSourceType IN ('SqlServer', 'Oracle')");
                });

            migrationBuilder.CreateTable(
                name: "AiConversations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SavedQueryId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SqlSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiConversations", x => x.Id);
                    table.CheckConstraint("CK_AiConversations_Role", "Role IN ('user', 'assistant', 'system')");
                    table.ForeignKey(
                        name: "FK_AiConversations_SavedQueries_SavedQueryId",
                        column: x => x.SavedQueryId,
                        principalTable: "SavedQueries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiConversations_SavedQueryId_CreatedAt",
                table: "AiConversations",
                columns: new[] { "SavedQueryId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedQueries_Name",
                table: "SavedQueries",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SavedQueries_UpdatedAt",
                table: "SavedQueries",
                column: "UpdatedAt",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiConversations");

            migrationBuilder.DropTable(
                name: "SavedQueries");

            // NOTE: Do NOT drop ValueFormatType or ValueUnit columns here —
            // they existed in the database before this migration and are
            // managed by a different migration history.
        }
    }
}
