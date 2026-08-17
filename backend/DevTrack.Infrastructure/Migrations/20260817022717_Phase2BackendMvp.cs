using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase2BackendMvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventKey",
                table: "Reminders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"Reminders\" SET \"EventKey\" = 'legacy:' || \"Id\"::text WHERE \"EventKey\" = '';");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Reminders",
                type: "text",
                nullable: false,
                defaultValue: "DueToday");

            migrationBuilder.CreateTable(
                name: "AiRecommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    ProviderIdentifier = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiRecommendations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AiRescheduleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AiRecommendationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LearningTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposedPlannedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProposedDeadline = table.Column<DateOnly>(type: "date", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiRescheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiRescheduleItems_AiRecommendations_AiRecommendationId",
                        column: x => x.AiRecommendationId,
                        principalTable: "AiRecommendations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_LearningTaskId",
                table: "Reminders",
                column: "LearningTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId_EventKey",
                table: "Reminders",
                columns: new[] { "UserId", "EventKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiRecommendations_UserId",
                table: "AiRecommendations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AiRescheduleItems_AiRecommendationId",
                table: "AiRescheduleItems",
                column: "AiRecommendationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_LearningTasks_LearningTaskId",
                table: "Reminders",
                column: "LearningTaskId",
                principalTable: "LearningTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_LearningTasks_LearningTaskId",
                table: "Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders");

            migrationBuilder.DropTable(
                name: "AiRescheduleItems");

            migrationBuilder.DropTable(
                name: "AiRecommendations");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_LearningTaskId",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_UserId_EventKey",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "EventKey",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reminders");
        }
    }
}
