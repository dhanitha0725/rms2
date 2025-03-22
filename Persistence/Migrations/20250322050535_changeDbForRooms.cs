using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeDbForRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ReservedPackages");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "ReservedPackages",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateTable(
                name: "RoomPricing",
                columns: table => new
                {
                    RoomPricingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Sector = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    RoomType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    FacilityID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPricing", x => x.RoomPricingID);
                    table.ForeignKey(
                        name: "FK_RoomPricing_Facilities_FacilityID",
                        column: x => x.FacilityID,
                        principalTable: "Facilities",
                        principalColumn: "FacilityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomPricing_FacilityID",
                table: "RoomPricing",
                column: "FacilityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomPricing");

            migrationBuilder.DropColumn(
                name: "status",
                table: "ReservedPackages");

            migrationBuilder.DropColumn(
                name: "HasPackages",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HasRooms",
                table: "Facilities");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ReservedPackages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
