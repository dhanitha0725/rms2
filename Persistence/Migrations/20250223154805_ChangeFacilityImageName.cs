using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFacilityImageName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityImages_Facilities_FacilityID",
                table: "FacilityImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FacilityImages",
                table: "FacilityImages");

            migrationBuilder.RenameTable(
                name: "FacilityImages",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityImages_FacilityID",
                table: "Images",
                newName: "IX_Images_FacilityID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Facilities_FacilityID",
                table: "Images",
                column: "FacilityID",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Facilities_FacilityID",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "FacilityImages");

            migrationBuilder.RenameIndex(
                name: "IX_Images_FacilityID",
                table: "FacilityImages",
                newName: "IX_FacilityImages_FacilityID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FacilityImages",
                table: "FacilityImages",
                column: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityImages_Facilities_FacilityID",
                table: "FacilityImages",
                column: "FacilityID",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
