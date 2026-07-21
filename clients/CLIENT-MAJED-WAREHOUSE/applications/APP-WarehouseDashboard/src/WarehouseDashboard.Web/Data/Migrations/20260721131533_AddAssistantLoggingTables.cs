using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDashboard.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssistantLoggingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssistantInsightLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepthLevel = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PromptVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CardPromptUsed = table.Column<bool>(type: "bit", nullable: false),
                    DataScopeLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsFullDataReached = table.Column<bool>(type: "bit", nullable: false),
                    WasCached = table.Column<bool>(type: "bit", nullable: false),
                    ResponseTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistantInsightLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssistantInsightLogs_DashboardCards_CardId",
                        column: x => x.CardId,
                        principalTable: "DashboardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssistantUsageStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    TotalRequests = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ExplainRequests = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DeepRequests = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DeepenClicks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MostUsedDepth = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AverageResponseTimeMs = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    CacheHitCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CacheMissCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistantUsageStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssistantUsageStats_DashboardCards_CardId",
                        column: x => x.CardId,
                        principalTable: "DashboardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssistantInsightLogs_CardId",
                table: "AssistantInsightLogs",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_AssistantInsightLogs_RequestedAt",
                table: "AssistantInsightLogs",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AssistantUsageStats_CardId",
                table: "AssistantUsageStats",
                column: "CardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssistantInsightLogs");

            migrationBuilder.DropTable(
                name: "AssistantUsageStats");
        }
    }
}
