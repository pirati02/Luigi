using Microsoft.EntityFrameworkCore.Migrations;
using Reservation.Postgres;
using Reservation.Settings.Infrastructure;

#nullable disable

namespace Reservation.Settings.Infrastructure.Migrations
{
    public partial class ChangeSpot_Column1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Spot",
                table: "Sessions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<SessionSpots>(
                name: "Spot",
                table: "Sessions",
                type: "jsonb",
                nullable: false);
        }
    }
}
