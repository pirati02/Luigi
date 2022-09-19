using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Settings.Infrastructure.Migrations
{
    public partial class ChangeSpot_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RowsAndColumns",
                table: "Sessions",
                newName: "Spot");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Spot",
                table: "Sessions",
                newName: "RowsAndColumns");
        }
    }
}
