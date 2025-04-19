using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRelationshipUserDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Documents",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_UserID",
                table: "Documents",
                newName: "IX_Documents_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Documents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_UserId",
                table: "Documents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_UserId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Documents",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                newName: "IX_Documents_UserID");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users",
                table: "Documents",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
