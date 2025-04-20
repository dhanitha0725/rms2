using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFacility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPackages",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HasRooms",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Facilities",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentFacilityId",
                table: "Facilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ParentFacilityId",
                table: "Facilities",
                column: "ParentFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Facilities_ParentFacilityId",
                table: "Facilities",
                column: "ParentFacilityId",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Facilities_ParentFacilityId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_ParentFacilityId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "ParentFacilityId",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Facilities",
                type: "varchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPackages",
                table: "Facilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRooms",
                table: "Facilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
