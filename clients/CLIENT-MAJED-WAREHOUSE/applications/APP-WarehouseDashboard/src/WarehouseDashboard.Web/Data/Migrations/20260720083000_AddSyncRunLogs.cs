using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <summary>
    /// Creates SyncRuns and SyncRunDetails tables for persistent sync cycle logging
    /// (TASK-SYNC-LOG-01). Previously sync logs were stored only in an in-memory
    /// ring buffer (SyncRunLogStore) and were lost on process restart.
    /// </summary>
    public partial class AddSyncRunLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    TriggerType = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    TotalRecordCount = table.Column<int>(type: "int", nullable: true),
                    TotalDurationSeconds = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncRunDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyncRunId = table.Column<int>(type: "int", nullable: false),
                    TableMappingId = table.Column<int>(type: "int", nullable: true),
                    TargetTable = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    SyncMode = table.Column<string>(type: "nvarchar(20)", nullable: false, defaultValue: "Full"),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    RowsExtracted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RowsLoaded = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Attempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DurationSeconds = table.Column<double>(type: "float", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncRunDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncRunDetails_SyncRuns_SyncRunId",
                        column: x => x.SyncRunId,
                        principalTable: "SyncRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SyncRunDetails_TableMappingConfigs_TableMappingId",
                        column: x => x.TableMappingId,
                        principalTable: "TableMappingConfigs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncRuns_StartTime",
                table: "SyncRuns",
                column: "StartTime")
                .Annotation("SqlServer:Descending", new[] { true });

            migrationBuilder.CreateIndex(
                name: "IX_SyncRunDetails_SyncRunId",
                table: "SyncRunDetails",
                column: "SyncRunId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncRunDetails");

            migrationBuilder.DropTable(
                name: "SyncRuns");
        }
    }
}
