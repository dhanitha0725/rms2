using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "IssuedDate",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.CreateTable(
                name: "FinancialReport",
                columns: table => new
                {
                    FacilityId = table.Column<int>(type: "integer", nullable: false),
                    FacilityName = table.Column<string>(type: "text", nullable: false),
                    TotalReservations = table.Column<int>(type: "integer", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ReservationReport",
                columns: table => new
                {
                    FacilityId = table.Column<int>(type: "integer", nullable: false),
                    FacilityName = table.Column<string>(type: "text", nullable: false),
                    TotalReservations = table.Column<int>(type: "integer", nullable: false),
                    TotalCompletedReservations = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialReport");

            migrationBuilder.DropTable(
                name: "ReservationReport");

            migrationBuilder.AlterColumn<DateTime>(
                name: "IssuedDate",
                table: "Invoices",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
