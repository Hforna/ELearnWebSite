using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureRelationShips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_quiz_responses_AttemptId",
                table: "user_quiz_responses",
                column: "AttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_quiz_responses_quiz_attempts_AttemptId",
                table: "user_quiz_responses",
                column: "AttemptId",
                principalTable: "quiz_attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_quiz_responses_quiz_attempts_AttemptId",
                table: "user_quiz_responses");

            migrationBuilder.DropIndex(
                name: "IX_user_quiz_responses_AttemptId",
                table: "user_quiz_responses");
        }
    }
}
