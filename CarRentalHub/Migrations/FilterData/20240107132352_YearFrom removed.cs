using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalHub.Migrations.FilterData
{
    /// <inheritdoc />
    public partial class YearFromremoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfProductionFrom",
                table: "FilterData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearOfProductionFrom",
                table: "FilterData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
