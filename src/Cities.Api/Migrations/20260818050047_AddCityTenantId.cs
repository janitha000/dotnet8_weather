using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCityTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_Name",
                table: "Cities");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Cities",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_TenantId_Name",
                table: "Cities",
                columns: new[] { "TenantId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_TenantId_Name",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Cities");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);
        }
    }
}
