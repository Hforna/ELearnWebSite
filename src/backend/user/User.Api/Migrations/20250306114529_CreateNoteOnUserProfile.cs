using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateNoteOnUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Note",
                table: "profiles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "profiles");
        }
    }
}
