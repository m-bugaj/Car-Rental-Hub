using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalHub.Migrations
{
    /// <inheritdoc />
    public partial class Correctmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CarID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservation_CarInfoModel_CarID",
                        column: x => x.CarID,
                        principalTable: "CarInfoModel",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CarID",
                table: "Reservation",
                column: "CarID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservation");
        }
    }
}
