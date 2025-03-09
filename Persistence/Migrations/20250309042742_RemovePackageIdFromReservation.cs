using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePackageIdFromReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageID",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Reservations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "PackageID",
                table: "Reservations",
                type: "integer",
                nullable: true);
        }
    }
}
