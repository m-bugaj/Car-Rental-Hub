using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalHub.Migrations.CarAvailability
{
    /// <inheritdoc />
    public partial class Correctionofnameofmailandphone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "phone",
                table: "CarAvailability",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "CarAvailability",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "CarAvailability",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "CarAvailability",
                newName: "email");
        }
    }
}
