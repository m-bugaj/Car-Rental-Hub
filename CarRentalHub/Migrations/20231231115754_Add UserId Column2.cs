using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalHub.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdColumn2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
            name: "UserId",
            table: "CarInfoModel",
            nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "UserId",
            table: "CarInfoModel");
        }
    }
}
