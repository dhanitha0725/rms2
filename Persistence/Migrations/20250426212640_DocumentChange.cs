using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DocumentChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Payments",
                newName: "ReservationUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_UserID",
                table: "Payments",
                newName: "IX_Payments_ReservationUserID");

            migrationBuilder.RenameColumn(
                name: "ReservationID",
                table: "Documents",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ReservationID",
                table: "Documents",
                newName: "IX_Documents_ReservationId");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "Documents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PaymentId",
                table: "Documents",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Payments",
                table: "Documents",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_ReservationUserDetails_ReservationUserID",
                table: "Payments",
                column: "ReservationUserID",
                principalTable: "ReservationUserDetails",
                principalColumn: "ReservationUserDetailID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Payments",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_ReservationUserDetails_ReservationUserID",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Documents_PaymentId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "ReservationUserID",
                table: "Payments",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_ReservationUserID",
                table: "Payments",
                newName: "IX_Payments_UserID");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "Documents",
                newName: "ReservationID");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ReservationId",
                table: "Documents",
                newName: "IX_Documents_ReservationID");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Reservations",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReservationID",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
