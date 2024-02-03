using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AODashboard.Migrations
{
    /// <inheritdoc />
    public partial class VehicleEtag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ETag",
                table: "Vehicles",
                type: "nvarchar(44)",
                maxLength: 44,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETag",
                table: "Vehicles");
        }
    }
}
