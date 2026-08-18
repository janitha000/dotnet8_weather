using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCityIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_Name",
                table: "Cities");
        }
    }
}
