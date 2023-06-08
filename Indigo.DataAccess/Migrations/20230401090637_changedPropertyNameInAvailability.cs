using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Indigo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changedPropertyNameInAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_AspNetUsers_UserId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Availabilities",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_UserId",
                table: "Availabilities",
                newName: "IX_Availabilities_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_AspNetUsers_ApplicationUserId",
                table: "Availabilities",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_AspNetUsers_ApplicationUserId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Availabilities",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_ApplicationUserId",
                table: "Availabilities",
                newName: "IX_Availabilities_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_AspNetUsers_UserId",
                table: "Availabilities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
