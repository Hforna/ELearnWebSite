using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTimeOnLessonToDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeInMinutes",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "Lessons",
                newName: "VideoId");

            migrationBuilder.AddColumn<double>(
                name: "Duration",
                table: "Lessons",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CourseType",
                table: "courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "totalVisits",
                table: "courses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "CourseType",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "totalVisits",
                table: "courses");

            migrationBuilder.RenameColumn(
                name: "VideoId",
                table: "Lessons",
                newName: "VideoUrl");

            migrationBuilder.AddColumn<int>(
                name: "TimeInMinutes",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
