using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalHub.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedPhoto2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedPhotoId",
                table: "CarInfoModel",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedPhotoId",
                table: "CarInfoModel");
        }
    }
}
