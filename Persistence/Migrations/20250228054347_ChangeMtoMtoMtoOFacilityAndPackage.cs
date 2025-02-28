using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMtoMtoMtoOFacilityAndPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityPackages");

            migrationBuilder.AddColumn<int>(
                name: "FacilityID",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_FacilityID",
                table: "Packages",
                column: "FacilityID");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Facilities_FacilityID",
                table: "Packages",
                column: "FacilityID",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Facilities_FacilityID",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_FacilityID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "FacilityID",
                table: "Packages");

            migrationBuilder.CreateTable(
                name: "FacilityPackages",
                columns: table => new
                {
                    FacilityID = table.Column<int>(type: "int", nullable: false),
                    PackageID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityPackages", x => new { x.FacilityID, x.PackageID });
                    table.ForeignKey(
                        name: "FK_FacilityPackages_Facilities_FacilityID",
                        column: x => x.FacilityID,
                        principalTable: "Facilities",
                        principalColumn: "FacilityID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityPackages_Packages_PackageID",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityPackages_PackageID",
                table: "FacilityPackages",
                column: "PackageID");
        }
    }
}
