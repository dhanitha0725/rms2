using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addFacilityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityPackages_Facilities_FacilityID",
                table: "FacilityPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityPackages_Packages_PackageID",
                table: "FacilityPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicePayments_Invoices_InvoiceID",
                table: "InvoicePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicePayments_Payments_PaymentID",
                table: "InvoicePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Reservations_ReservationID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Pricing_Packages_PackageID",
                table: "Pricing");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedPackages_Packages_PackageID",
                table: "ReservedPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedPackages_Reservations_ReservationID",
                table: "ReservedPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedRooms_Reservations_ReservationID",
                table: "ReservedRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedRooms_Rooms_RoomID",
                table: "ReservedRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Facilities_FacilityID",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "FacilityType",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Facilities",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Facilities",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "FacilityType",
                columns: table => new
                {
                    FacilityTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityType", x => x.FacilityTypeID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityPackages_Facilities_FacilityID",
                table: "FacilityPackages",
                column: "FacilityID",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityPackages_Packages_PackageID",
                table: "FacilityPackages",
                column: "PackageID",
                principalTable: "Packages",
                principalColumn: "PackageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicePayments_Invoices_InvoiceID",
                table: "InvoicePayments",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "InvoiceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicePayments_Payments_PaymentID",
                table: "InvoicePayments",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Reservations_ReservationID",
                table: "Invoices",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ReservationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pricing_Packages_PackageID",
                table: "Pricing",
                column: "PackageID",
                principalTable: "Packages",
                principalColumn: "PackageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedPackages_Packages_PackageID",
                table: "ReservedPackages",
                column: "PackageID",
                principalTable: "Packages",
                principalColumn: "PackageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedPackages_Reservations_ReservationID",
                table: "ReservedPackages",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ReservationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedRooms_Reservations_ReservationID",
                table: "ReservedRooms",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ReservationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedRooms_Rooms_RoomID",
                table: "ReservedRooms",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Facilities_FacilityID",
                table: "Rooms",
                column: "FacilityID",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityPackages_Facilities_FacilityID",
                table: "FacilityPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityPackages_Packages_PackageID",
                table: "FacilityPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicePayments_Invoices_InvoiceID",
                table: "InvoicePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicePayments_Payments_PaymentID",
                table: "InvoicePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Reservations_ReservationID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Pricing_Packages_PackageID",
                table: "Pricing");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedPackages_Packages_PackageID",
                table: "ReservedPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedPackages_Reservations_ReservationID",
                table: "ReservedPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedRooms_Reservations_ReservationID",
                table: "ReservedRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservedRooms_Rooms_RoomID",
                table: "ReservedRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Facilities_FacilityID",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "FacilityType");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Facilities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Facilities",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.AddColumn<string>(
                name: "FacilityType",
                table: "Facilities",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityPackages_Facilities_FacilityID",
                table: "FacilityPackages",
                column: "FacilityID",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityPackages_Packages_PackageID",
                table: "FacilityPackages",
                column: "PackageID",
                principalTable: "Packages",
                principalColumn: "PackageID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicePayments_Invoices_InvoiceID",
                table: "InvoicePayments",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "InvoiceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicePayments_Payments_PaymentID",
                table: "InvoicePayments",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Reservations_ReservationID",
                table: "Invoices",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ReservationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pricing_Packages_PackageID",
                table: "Pricing",
                column: "PackageID",
                principalTable: "Packages",
                principalColumn: "PackageID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedPackages_Packages_PackageID",
                table: "ReservedPackages",
                column: "PackageID",
                principalTable: "Packages",
                principalColumn: "PackageID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedPackages_Reservations_ReservationID",
                table: "ReservedPackages",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ReservationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedRooms_Reservations_ReservationID",
                table: "ReservedRooms",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ReservationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservedRooms_Rooms_RoomID",
                table: "ReservedRooms",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Facilities_FacilityID",
                table: "Rooms",
                column: "FacilityID",
                principalTable: "Facilities",
                principalColumn: "FacilityID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
