using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureRoomTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "RoomPricing");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "RoomTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeID",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeID",
                table: "RoomPricing",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeID",
                table: "Rooms",
                column: "RoomTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPricing_RoomTypeID",
                table: "RoomPricing",
                column: "RoomTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomPricing_RoomTypes_RoomTypeID",
                table: "RoomPricing",
                column: "RoomTypeID",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomTypes_RoomTypeID",
                table: "Rooms",
                column: "RoomTypeID",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomPricing_RoomTypes_RoomTypeID",
                table: "RoomPricing");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomTypes_RoomTypeID",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTypeID",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_RoomPricing_RoomTypeID",
                table: "RoomPricing");

            migrationBuilder.DropColumn(
                name: "RoomTypeID",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomTypeID",
                table: "RoomPricing");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "RoomTypes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Rooms",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "RoomPricing",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}
