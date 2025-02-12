using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class configureFacilityTypeAndFacility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacilityTypeId",
                table: "Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "FacilityType",
                columns: new[] { "FacilityTypeID", "TypeName" },
                values: new object[,]
                {
                    { 1, "Auditorium" },
                    { 2, "Bungalow" },
                    { 3, "Hall" },
                    { 4, "Hostel" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_FacilityTypeId",
                table: "Facilities",
                column: "FacilityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_FacilityType_FacilityTypeId",
                table: "Facilities",
                column: "FacilityTypeId",
                principalTable: "FacilityType",
                principalColumn: "FacilityTypeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_FacilityType_FacilityTypeId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_FacilityTypeId",
                table: "Facilities");

            migrationBuilder.DeleteData(
                table: "FacilityType",
                keyColumn: "FacilityTypeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FacilityType",
                keyColumn: "FacilityTypeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FacilityType",
                keyColumn: "FacilityTypeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FacilityType",
                keyColumn: "FacilityTypeID",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "FacilityTypeId",
                table: "Facilities");
        }
    }
}
