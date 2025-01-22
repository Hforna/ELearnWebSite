using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRatingForOtherService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
